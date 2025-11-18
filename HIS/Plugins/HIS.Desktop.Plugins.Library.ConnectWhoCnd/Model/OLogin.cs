
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.ConnectWhoCnd.Model
{
    public class Response
    {
        public string responseType { get; set; }
        public string key { get; set; }
        public long expire { get; set; }
    }

    public class OLogin
    {
        public string httpStatus { get; set; }
        public int httpStatusCode { get; set; }
        public Response response { get; set; }
    }
}
