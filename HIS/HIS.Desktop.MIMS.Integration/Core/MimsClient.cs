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
            var sw = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                Inventec.Common.Logging.LogSystem.Debug(string.Format(
                    "MimsClient.PostXml - start, url={0}, requestLength={1}",
                    url, xml == null ? 0 : xml.Length));

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
                    string respText = reader.ReadToEnd();
                    sw.Stop();
                    Inventec.Common.Logging.LogSystem.Debug(string.Format(
                        "MimsClient.PostXml - success, responseLength={0}, elapsed={1}ms",
                        respText == null ? 0 : respText.Length, sw.ElapsedMilliseconds));
                    return respText;
                }
            }
            catch (WebException ex)
            {
                sw.Stop();
                Inventec.Common.Logging.LogSystem.Error(string.Format(
                    "MimsClient.PostXml WebException - status={0}, elapsed={1}ms, url={2}",
                    ex.Status, sw.ElapsedMilliseconds, url), ex);

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
                sw.Stop();
                Inventec.Common.Logging.LogSystem.Error(string.Format(
                    "MimsClient.PostXml Exception - elapsed={0}ms, url={1}",
                    sw.ElapsedMilliseconds, url), ex);
                return null;
            }
        }
    }
}
