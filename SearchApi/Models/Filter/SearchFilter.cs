using TypeGen.Core.TypeAnnotations;

namespace SearchApi.Models.Filter
{

    public class Paging
    {
        public int page { get; set; }   // 0 base
        public int limit { get; set; }
    }

    public class Sort
    {
        public string price { get; set; }

        public string viewCnt { get; set; }

        public string saleCnt { get; set; }

    }

    public class Filters
    {
        public int minPrice { get; set; }

        public int maxPrice { get; set; }

        public string category1 { get; set; }

        public string category2 { get; set; }

        public string category3 { get; set; }

        public string tag { get; set; }
    }

    [ExportTsInterface(OutputDir = "wwwroot/ts/in")]
    public class SearchFilter
    {
        public Paging paging { get; set; }

        public Sort sort { get; set; }

        public Filters filters { get; set; }

        public string keyword { get; set; }
    }
}
