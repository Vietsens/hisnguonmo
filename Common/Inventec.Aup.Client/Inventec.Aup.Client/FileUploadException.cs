using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Aup.Client
{
    public class FileUploadException : Exception
    {
        public HttpStatusCode StatusCode { get; set; }

        public FileUploadException()
        {
        }

        public FileUploadException(string message, Exception inner)
            : base(message, inner)
        {
        }

        public FileUploadException(HttpStatusCode statusCode, string message)
            : base(message)
        {
            this.StatusCode = statusCode;
        }
    }
}
