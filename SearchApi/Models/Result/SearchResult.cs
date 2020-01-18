using System.Collections.Generic;
using SearchApi.Entity;
using TypeGen.Core.TypeAnnotations;

namespace SearchApi.Models.Result
{
    [ExportTsInterface(OutputDir = "wwwroot/ts/out")]
    public class SearchResult
    {
        public List<SearchGoods> list {get;set;}

        public int total { get; set; }

        public int size { get; set; }

        public Summary summary { get; set; }
    }
}
