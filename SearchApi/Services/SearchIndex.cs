using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nest;
using SearchApi.Entity;
using SearchApi.Models.Filter;
using SearchApi.Models.Result;
using SearchApi.Repositories;
using Microsoft.EntityFrameworkCore;
using SearchApi.Config;
using Microsoft.Extensions.Configuration;
using Elasticsearch.Net;

namespace SearchApi.Services
{
    public class SearchIndex
    {
        private readonly SearchRepository _searchRepository;
        private readonly IElasticClient _elasticClient;
        private readonly string _indexName;

        public object FirstNameToFind { get; private set; }

        public SearchIndex(SearchRepository searchRepository, IElasticClient elasticClient,
            IConfiguration configuration)
        {
            _searchRepository = searchRepository;
            _elasticClient = elasticClient;
            _indexName = configuration["elasticsearch:index"];
        }

        public async Task<List<SearchGoods>> FindAll()
        {
            var goods = await _searchRepository.searchGoods
                .Select(s => s)
                .ToAsyncEnumerable()
                .ToList();
            return goods;
        }

        public async Task<SearchGoods> FindByGoodsNo(string goodsNo)
        {
            var goods = await _searchRepository.searchGoods
                .Select(s => s)
                .Where(s => s.goodsNo == goodsNo)
                .SingleAsync();

            return goods;
        }

        public async Task<int> ReindexAll()
        {
            await _elasticClient.DeleteByQueryAsync<SearchGoods>(q => q.MatchAll());
            var allGoods = await FindAll();
            foreach (var item in allGoods)
            {
                // 키워드 검색대상 추가
                item.terms = $"{item.nameKr} {item.category1} {item.category2} {item.category3} {item.terms}";
                await _elasticClient.IndexDocumentAsync(item);
            }            
            return allGoods.Count;
        }

        public async Task<int> UpdateItem(SearchGoods item)
        {
            await _elasticClient.UpdateAsync<SearchGoods>(item.no, u => u.Doc(item));

            await _elasticClient.UpdateByQueryAsync<SearchGoods>(u => u
            .Query(q => q
            .Match(m => m
                .Field(p => p.goodsNo)
                .Query(item.goodsNo)
            ))
            .Script($"ctx._source.price = {item.price}")
            .Conflicts(Conflicts.Proceed)
            .Refresh(true));

            await _searchRepository.SaveChangesAsync();
            return 0;
        }

        public async Task<SearchResult> FindByFilter(SearchFilter filterOpt)
        {
            var result = new SearchResult();

            int page = filterOpt.paging == null ? 0 : filterOpt.paging.page;
            int limit = Math.Min(100,filterOpt.paging == null ? 10 : filterOpt.paging.limit);

            //Paging
            var searchDes = new SearchDescriptor<SearchGoods>()
                .From(page)
                .Size(limit);

            //Sort
            var sortDescriptor = new SortDescriptor<SearchGoods> ();            
            if(!String.IsNullOrWhiteSpace(filterOpt.sort?.price))
            {
                if (filterOpt.sort.price == "desc")
                {
                    sortDescriptor.Field(f => f.price, Nest.SortOrder.Descending);
                }
                else if(filterOpt.sort.price == "asc")
                {
                    sortDescriptor.Field(f => f.price, Nest.SortOrder.Ascending);
                }
            }

            if (!String.IsNullOrWhiteSpace(filterOpt.sort?.saleCnt))
            {
                if (filterOpt.sort.saleCnt == "desc")
                {
                    sortDescriptor.Field(f => f.saleCnt, Nest.SortOrder.Descending);
                }
                else if (filterOpt.sort.saleCnt == "asc")
                {
                    sortDescriptor.Field(f => f.saleCnt, Nest.SortOrder.Ascending);
                }
            }

            if (!String.IsNullOrWhiteSpace(filterOpt.sort?.viewCnt))
            {
                if (filterOpt.sort.viewCnt == "desc")
                {
                    sortDescriptor.Field(f => f.viewCnt, Nest.SortOrder.Descending);
                }
                else if (filterOpt.sort.viewCnt == "asc")
                {
                    sortDescriptor.Field(f => f.viewCnt, Nest.SortOrder.Ascending);
                }
            }

            searchDes = searchDes.Sort( s=>sortDescriptor);

            //Filter
            var filters = new List<Func<QueryContainerDescriptor<SearchGoods>, QueryContainer>>();

            //Category Filter
            if ( !String.IsNullOrWhiteSpace(filterOpt.filters?.category1) )
            {
                filters.Add(fq => fq.Match(t => t.Field(f => f.category1).Query(filterOpt.filters.category1)));                
            }

            if ( !String.IsNullOrWhiteSpace(filterOpt.filters?.category2) )
            {
                filters.Add(fq => fq.Match(t => t.Field(f => f.category2).Query(filterOpt.filters.category2)));
            }

            if ( !String.IsNullOrWhiteSpace(filterOpt.filters?.category3) )
            {
                filters.Add(fq => fq.Match(t => t.Field(f => f.category3).Query(filterOpt.filters.category3)));
            }

            //Tag Filter
            if (!String.IsNullOrWhiteSpace(filterOpt.filters?.tag))
            {
                filters.Add(fq => fq.Match(t => t.Field(f => f.tags).Query(filterOpt.filters.tag)));
            }

            //Query
            if (!String.IsNullOrWhiteSpace(filterOpt.keyword))
            {
                var keywords = filterOpt.keyword.Split(' ');
                filters.Add(fq => fq.Terms(t => t.Field(f => f.terms).Terms(keywords)));
            }

            //Price
            if (filterOpt.filters?.minPrice > 0 && filterOpt.filters?.maxPrice > 0)
            {
                filters.Add(fq => fq.Range(c => c
                    .Name("named_query")
                    .Boost(1.1)
                    .Field(p => p.price)
                    .GreaterThanOrEquals(filterOpt.filters.minPrice)
                    .LessThanOrEquals(filterOpt.filters.maxPrice)
                    .Relation(RangeRelation.Within)
                ));
            }

            searchDes = searchDes.Query(q=>q                
                .Bool(bq => bq.Filter(filters)));

            //Aggregations(검색 결과 집계 처리)
            searchDes.Aggregations(aggs => aggs
                    .Average("average_per_price", avg => avg.Field(p => p.price))
                    .Max("max_per_price", avg => avg.Field(p => p.price))
                    .Min("min_per_price", avg => avg.Field(p => p.price))
                    .Terms("category1_cnt", st => st
                        .Field(p => p.category1.Suffix("keyword"))
                        .MinimumDocumentCount(1)
                        .Size(100000)
                        .ShardSize(100)
                        .ExecutionHint(TermsAggregationExecutionHint.Map)
                        .Missing("na")
                        .Order(o => o
                            .KeyAscending()
                            .CountDescending()
                        )
                        .Meta(m => m
                            .Add("foo", "bar")
                        )
                    )
                    .Terms("category2_cnt", st => st
                        .Field(p => p.category2.Suffix("keyword"))
                        .MinimumDocumentCount(1)
                        .Size(100000)
                        .ShardSize(100)
                        .ExecutionHint(TermsAggregationExecutionHint.Map)
                        .Missing("na")
                        .Order(o => o
                            .KeyAscending()
                            .CountDescending()
                        )
                        .Meta(m => m
                            .Add("foo", "bar")
                        )
                    )
            );

            /*
             .Cardinality("state_count", c => c
                        .Field(p => p.category1)
                        .PrecisionThreshold(100)
                    )
             */

            var engine_result = await _elasticClient.SearchAsync<SearchGoods>(searchDes);
            result.list = engine_result.Documents.ToList();
            result.total = (int)engine_result.Total;
            result.size = result.list.Count;

            var agg = engine_result.Aggregations;
            var category1_aggs = agg.Terms("category1_cnt");
            var category2_aggs = agg.Terms("category2_cnt");


            // 검색내 재 검색을 위한 summary : 아직 미구현
            result.summary = new Summary
            {
                tags = new List<string>()
                {
                    "여성","의류","원피스"
                },
                filterValues = new List<FilterValue>()
                {
                    new FilterValue(){fieldName="average_per_price",name="",value=agg.ValueCount("average_per_price").Value},
                    new FilterValue(){fieldName="max_per_price",name="",value=agg.ValueCount("max_per_price").Value},
                    new FilterValue(){fieldName="min_per_price",name="",value=agg.ValueCount("min_per_price").Value}
                    
                }
            };

            foreach(var category1_item in category1_aggs.Buckets)
            {
                result.summary.filterValues.Add(new FilterValue() { fieldName = "category1", name = category1_item.Key, value = category1_item.DocCount });                
            }

            foreach (var category2_item in category2_aggs.Buckets)
            {
                result.summary.filterValues.Add(new FilterValue() { fieldName = "category2", name = category2_item.Key, value = category2_item.DocCount });
            }

            return result;            
        }
    }
}
