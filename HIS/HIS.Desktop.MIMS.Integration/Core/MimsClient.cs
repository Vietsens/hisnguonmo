using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;

namespace HIS.Desktop.MIMS.Integration.Core
{
    public static class MimsClient
    {
        [ThreadStatic]
        private static string lastErrorMessage;

        /// <summary>
        /// vCong 62522: thông báo lỗi (đã phân loại nguồn) của lần gọi PostXml gần nhất trên luồng hiện tại; null nếu thành công.
        /// </summary>
        public static string LastErrorMessage
        {
            get { return lastErrorMessage; }
        }

        public static Inventec.Common.IntegrationError.IntegrationPartner Partner
        {
            get { return Inventec.Common.IntegrationError.IntegrationPartner.Ai("MIMS"); }
        }

        /// <summary>
        /// Thông báo cho người dùng khi PostXml trả về null/rỗng: dùng lỗi đã phân loại nếu có, không thì "không nhận được phản hồi".
        /// </summary>
        public static string BuildErrorMessage()
        {
            if (!String.IsNullOrEmpty(lastErrorMessage)) return lastErrorMessage;
            return Inventec.Common.IntegrationError.IntegrationErrorMessage.NoResponse(Partner, "tra cứu cảnh báo thuốc");
        }
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
            return PostXml(url, xml, null, out isTimeoutOrConnectionError);
        }

        /// <summary>
        /// Gửi request tới MIMS kèm các tham số form tuỳ chọn của FT Web Service
        /// ("alertfilterbydrug", "alertfilterbyseverity" — việc 52540).
        /// extraFormParams null/rỗng → post y hệt như trước, không đổi 1 byte.
        /// </summary>
        public static string PostXml(string url, string xml,
            Dictionary<string, string> extraFormParams, out bool isTimeoutOrConnectionError)
        {
            isTimeoutOrConnectionError = false;
            lastErrorMessage = null;
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

                var postDataBuilder = new StringBuilder();
                postDataBuilder.Append("prescriptionquery=").Append(HttpUtility.UrlEncode(xml));
                postDataBuilder.Append("&responsetype=xml");

                if (extraFormParams != null && extraFormParams.Count > 0)
                {
                    foreach (var param in extraFormParams)
                    {
                        if (string.IsNullOrEmpty(param.Key) || string.IsNullOrEmpty(param.Value))
                            continue;

                        postDataBuilder.Append("&")
                            .Append(param.Key)
                            .Append("=")
                            .Append(HttpUtility.UrlEncode(param.Value));
                    }

                    Inventec.Common.Logging.LogSystem.Debug(string.Format(
                        "MimsClient.PostXml - extraFormParams: {0}",
                        string.Join(", ", extraFormParams.Keys)));
                }

                var postData = postDataBuilder.ToString();

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

                // vCong 62522: 401 (sai tai khoan MIMS) = thiet lap; timeout/DNS = ket noi; HTTP khac = phia MIMS (kem body)
                lastErrorMessage = BuildMessage(Inventec.Common.IntegrationError.IntegrationHttpError.WrapCall(ex, Partner, url));
                return null;
            }
            catch (Exception ex)
            {
                sw.Stop();
                Inventec.Common.Logging.LogSystem.Error(string.Format(
                    "MimsClient.PostXml Exception - elapsed={0}ms, url={1}",
                    sw.ElapsedMilliseconds, url), ex);
                lastErrorMessage = BuildMessage(ex is UriFormatException || ex is NotSupportedException || ex is ArgumentNullException
                    ? new Inventec.Common.IntegrationError.IntegrationException(Inventec.Common.IntegrationError.ErrorSource.Config, Partner, "Địa chỉ MIMS trong cấu hình không hợp lệ: " + url, ex)
                    : ex);
                return null;
            }
        }

        private static string BuildMessage(Exception ex)
        {
            try
            {
                return Inventec.Common.IntegrationError.IntegrationErrorMessage.FromException(ex, Partner, "tra cứu cảnh báo thuốc");
            }
            catch (Exception exx)
            {
                Inventec.Common.Logging.LogSystem.Warn(exx);
                return null;
            }
        }
    }
}
