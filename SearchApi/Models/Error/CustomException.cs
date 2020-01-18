using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SearchApi.Models.Error
{
    public class CustomException : Exception
    {
        public ErrorDetails errorDetails { get; set; }

        public CustomException(ErrorDetails _errorDetails, string message)
            : base(message)
        {
            errorDetails = _errorDetails;
        }
    }
}
