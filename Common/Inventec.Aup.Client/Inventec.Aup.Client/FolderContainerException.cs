using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Aup.Client
{
    public class FolderContainerException : Exception
    {
        public HttpStatusCode StatusCode { get; set; }

        public FolderContainerException()
        {
        }

        public FolderContainerException(string message, Exception inner)
            : base(message, inner)
        {
        }

        public FolderContainerException(HttpStatusCode statusCode, string message)
            : base(message)
        {
            this.StatusCode = statusCode;
        }
    }
}
