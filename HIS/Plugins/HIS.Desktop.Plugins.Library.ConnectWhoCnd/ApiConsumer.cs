using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.ConnectWhoCnd
{
    internal class ApiConsumers
    {
        /// <summary>vCong 62522: cổng WHO CND (quản lý bệnh không lây nhiễm).</summary>
        internal static Inventec.Common.IntegrationError.IntegrationPartner Partner
        {
            get { return Inventec.Common.IntegrationError.IntegrationPartner.Ai("WHO CND"); }
        }

        /// <summary>vCong 62522: che mật khẩu/token trước khi ghi log.</summary>
        static string MaskSecret(string json)
        {
            if (String.IsNullOrEmpty(json)) return json;
            return System.Text.RegularExpressions.Regex.Replace(json, "\"(password|token|key)\"\\s*:\\s*\"[^\"]*\"", "\"$1\":\"***\"", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        }

        public static T CreateRequest<T>(string method, string baseUri, string requestUri, object data)
        {
            System.Net.ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            Uri baseAddress;
            if (String.IsNullOrWhiteSpace(baseUri) || !Uri.TryCreate(baseUri, UriKind.Absolute, out baseAddress))
                throw new Inventec.Common.IntegrationError.IntegrationException(Inventec.Common.IntegrationError.ErrorSource.Config, Partner, "Địa chỉ cổng WHO CND trong cấu hình chưa có hoặc sai định dạng: " + baseUri);
            using (var client = new HttpClient())
            {
                client.BaseAddress = baseAddress;
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.Timeout = new TimeSpan(0, 0, 90);

                HttpResponseMessage resp = null;

                string fullrequestUri = CombileUrl(baseUri, requestUri);

                string sendJsonData = JsonConvert.SerializeObject(data);

                Inventec.Common.Logging.LogSystem.Info(string.Format("API: {0}. sendJsonData: {1}", fullrequestUri, MaskSecret(sendJsonData)));
                try
                {
                    if (method.Equals(System.Net.WebRequestMethods.Http.Get))
                        resp = client.GetAsync(fullrequestUri).Result;
                    else
                        resp = client.PostAsync(fullrequestUri, new StringContent(sendJsonData, Encoding.UTF8, "application/json")).Result;
                }
                catch (Exception ex)
                {
                    // vCong 62522: timeout/DNS/SSL/khong ket noi -> exception co nguon
                    throw Inventec.Common.IntegrationError.IntegrationHttpError.WrapCall(ex, Partner, fullrequestUri);
                }

                if (resp == null || !resp.IsSuccessStatusCode)
                {
                    Inventec.Common.IntegrationError.IntegrationException httpEx = Inventec.Common.IntegrationError.IntegrationHttpError.FromStatus(Partner, fullrequestUri, resp);
                    Inventec.Common.Logging.LogSystem.Error(string.Format("Loi khi goi API: {0}{1}. {2}", baseUri, requestUri, httpEx.Message));
                    throw httpEx;
                }
                T result = default(T);
                string responseData = Inventec.Common.IntegrationError.IntegrationHttpError.ReadBody(resp);
                Inventec.Common.Logging.LogSystem.Info("__________________api responseData: " + MaskSecret(responseData));
                try
                {
                    result = JsonConvert.DeserializeObject<T>(responseData);
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                    throw Inventec.Common.IntegrationError.IntegrationHttpError.ParseError(Partner, fullrequestUri, MaskSecret(responseData), ex);
                }
                return result;
            }
        }
        static string CombileUrl(params string[] data)
        {
            string result = "";
            List<string> pathUrl = new List<string>();
            for (int i = 0; i < data.Length; i++)
            {
                pathUrl.Add(data[i].Trim('/'));
            }

            result = string.Join("/", pathUrl);

            return result;
        }
    }
}
