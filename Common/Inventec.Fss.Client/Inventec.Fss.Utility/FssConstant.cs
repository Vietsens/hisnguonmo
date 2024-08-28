using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Fss.Utility
{
    public class FssConstant
    {
        public static string BASE_URI = ConfigurationManager.AppSettings["fss.uri.base"];
        public static string UPLOAD_URI = ConfigurationManager.AppSettings["fss.uri.upload"];
        public static string DELETE_URI = (ConfigurationManager.AppSettings["fss.uri.delete"] ?? "api/File/Delete");
        public static int TIME_OUT = int.Parse(ConfigurationManager.AppSettings["fss.timeout"]);
        public const string HEADER_CLIENT_CODE = "fss-client-code";
        public const string HEADER_FILE_STORE_LOCATION = "fss-file-store-location";
    }
}
