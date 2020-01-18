using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nest;
using SearchApi.Services;

namespace SearchApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IndexController : ControllerBase
    {
        private readonly SearchIndex _searchIndex;
        private readonly IElasticClient _elasticClient;

        public IndexController(SearchIndex searchIndex, IElasticClient elasticClient)
        {
            _searchIndex = searchIndex;
            _elasticClient = elasticClient;
        }

        [HttpPut("reindex")]
        public async Task<IActionResult> ReIndex()
        {
            var count = await _searchIndex.ReindexAll();
            return Ok($"{count} post(s) reindexed");
        }
    }
}