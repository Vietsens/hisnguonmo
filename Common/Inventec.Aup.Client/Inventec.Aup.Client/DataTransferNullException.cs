using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Aup.Client
{
    public class DataTransferNullException : Exception
    {
        public HttpStatusCode StatusCode { get; set; }

        public DataTransferNullException()
        {
        }

        public DataTransferNullException(string message, Exception inner)
            : base(message, inner)
        {
        }

        public DataTransferNullException(HttpStatusCode statusCode, string message)
            : base(message)
        {
            this.StatusCode = statusCode;
        }
    }
}
