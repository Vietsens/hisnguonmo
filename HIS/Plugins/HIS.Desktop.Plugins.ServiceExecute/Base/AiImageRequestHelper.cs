using Inventec.Common.IntegrationError;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;

namespace HIS.Desktop.Plugins.ServiceExecute.Base
{
    /// <summary>
    /// vCong 62522: gọi dịch vụ AI phân tích hình ảnh (key HIS.Desktop.AI.ConnectionInfo), lỗi ném IntegrationException có nguồn.
    /// </summary>
    internal static class AiImageRequestHelper
    {
        internal const string CONFIG_KEY = "HIS.Desktop.AI.ConnectionInfo";

        internal static IntegrationPartner Partner
        {
            get { return IntegrationPartner.Ai("dịch vụ AI phân tích hình ảnh"); }
        }

        /// <summary>Thông báo khi chưa cấu hình địa chỉ dịch vụ AI.</summary>
        internal static string MissingConfigMessage
        {
            get { return IntegrationErrorMessage.ConfigError(Partner, "Chưa thiết lập địa chỉ dịch vụ AI phân tích hình ảnh (cấu hình hệ thống " + CONFIG_KEY + ")"); }
        }

        internal static T Post<T>(string requestUri, Dictionary<string, object> sendData)
        {
            Uri baseUri;
            if (!Uri.TryCreate(requestUri, UriKind.Absolute, out baseUri))
                throw new IntegrationException(ErrorSource.Config, Partner, "Địa chỉ dịch vụ AI phân tích hình ảnh (" + CONFIG_KEY + ") sai định dạng: " + requestUri);

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            using (var client = new HttpClient())
            {
                client.BaseAddress = baseUri;
                client.Timeout = new TimeSpan(0, 5, 0);
                client.DefaultRequestHeaders.Accept.Clear();
                var stringPayload = JsonConvert.SerializeObject(sendData);
                var content = new StringContent(stringPayload, Encoding.UTF8, "application/json");
                HttpResponseMessage resp = null;
                try
                {
                    resp = client.PostAsync(requestUri, content).Result;
                }
                catch (Exception ex)
                {
                    throw IntegrationHttpError.WrapCall(ex, Partner, requestUri);
                }
                if (resp == null || !resp.IsSuccessStatusCode)
                {
                    IntegrationException httpEx = IntegrationHttpError.FromStatus(Partner, requestUri, resp);
                    Inventec.Common.Logging.LogSystem.Error("errorData: " + httpEx.Message);
                    throw httpEx;
                }
                string responseData = IntegrationHttpError.ReadBody(resp);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => responseData), responseData));
                T data;
                try
                {
                    data = JsonConvert.DeserializeObject<T>(responseData);
                }
                catch (Exception ex)
                {
                    throw IntegrationHttpError.ParseError(Partner, requestUri, responseData, ex);
                }
                if (data == null)
                {
                    throw IntegrationHttpError.ParseError(Partner, requestUri, responseData, null);
                }
                return data;
            }
        }
    }
}
