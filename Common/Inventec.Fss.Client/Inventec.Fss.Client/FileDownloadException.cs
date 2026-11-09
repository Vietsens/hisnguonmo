using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Inventec.Fss.Client
{
    public class FileDownloadException: Exception
    {
        public HttpStatusCode StatusCode { get; set; }

        public FileDownloadException()
        {
        }

        public FileDownloadException(string message, Exception inner)
            : base(message, inner)
        {
        }

        public FileDownloadException(HttpStatusCode statusCode, string message)
            : base(message)
        {
            this.StatusCode = statusCode;
        }

        /// <summary>vCong 62522: kèm exception gốc đã phân loại nguồn lỗi (IntegrationException) để lớp trên không phải đoán lại.</summary>
        public FileDownloadException(HttpStatusCode statusCode, string message, Exception inner)
            : base(message, inner)
        {
            this.StatusCode = statusCode;
        }
    }
}
