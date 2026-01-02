using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web;

namespace HIS.Desktop.MIMS.Integration.Core
{
    public static class MimsClient
    {
        /// <summary>
        /// Gửi request tới MIMS (đơn giản, không phân biệt timeout).
        /// </summary>
        public static string PostXml(string url, string xml)
        {
            bool _;
            return PostXml(url, xml, out _);
        }

        /// <summary>
        /// Gửi request tới MIMS, trả về chuỗi XML và cờ timeout/kết nối.
        /// </summary>
        public static string PostXml(string url, string xml, out bool isTimeoutOrConnectionError)
        {
            isTimeoutOrConnectionError = false;

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

                // Timeout hợp lý để UI không bị treo lâu (ms)
                request.Timeout = 15000;         // 15s cho kết nối
                request.ReadWriteTimeout = 15000; // 15s cho đọc/ghi

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
            catch (WebException ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);

                if (ex.Status == WebExceptionStatus.Timeout ||
                    ex.Status == WebExceptionStatus.ConnectFailure ||
                    ex.Status == WebExceptionStatus.NameResolutionFailure)
                {
                    isTimeoutOrConnectionError = true;
                }

                return null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }
    }
}
