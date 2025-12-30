using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web;

namespace HIS.Desktop.MIMS.Integration.Core
{
    public static class MimsClient
    {
        public static string PostXml(string url, string xml)
        {
            try
            {
                // Đảm bảo sử dụng TLS 1.2 khi kết nối tới server MIMS
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.Credentials = new NetworkCredential(
                    MimsConfig.Username,
                    MimsConfig.Password);

                var postData =
                    "prescriptionquery=" + HttpUtility.UrlEncode(xml) +
                    "&responsetype=xml";

                using (var stream = request.GetRequestStream())
                {
                    var bytes = Encoding.UTF8.GetBytes(postData);
                    stream.Write(bytes, 0, bytes.Length);
                }

                using (var response = request.GetResponse())
                using (var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }
    }
}
