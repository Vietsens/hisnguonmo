using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Inventec.Fss.Client
{
    public class FileDeleteException: Exception
    {
        public HttpStatusCode StatusCode { get; set; }

        public FileDeleteException()
        {
        }

        public FileDeleteException(string message, Exception inner)
            : base(message, inner)
        {
        }

        public FileDeleteException(HttpStatusCode statusCode, string message)
            : base(message)
        {
            this.StatusCode = statusCode;
        }
    }
}
