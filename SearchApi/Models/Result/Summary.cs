using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SearchApi.Models.Result
{
    public class FilterValue
    {
        public string fieldName { get; set; }

        public string name { get; set; }

        public double? value { get; set; }
    }

    public class Summary
    {
        public List<string> tags { get; set; }

        public List<FilterValue> filterValues { get; set; }

    }
}
