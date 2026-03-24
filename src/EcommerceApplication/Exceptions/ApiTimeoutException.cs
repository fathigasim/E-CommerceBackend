using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceApplication.Exceptions
{
    public class ApiTimeoutException : Exception
    {
        public ApiTimeoutException(string message) : base(message) { }
    }
}
