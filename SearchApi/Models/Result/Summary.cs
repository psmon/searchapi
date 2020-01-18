using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SearchApi.Models.Result
{
    public class FilterCount
    {
        public string fieldName { get; set; }

        public string name { get; set; }

        public int count { get; set; }
    }

    public class Summary
    {
        public List<string> tags { get; set; }

        public List<FilterCount> filterCounts { get; set; }

    }
}
