/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *  
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *  
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * GNU General Public License for more details.
 *  
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.DrugInterventionInfo
{
    class ApiConsumer
    {
        /// <summary>vCong 62522: hệ thống kiểm tra/can thiệp dược lý (cấu hình ConnectionInfo).</summary>
        internal static Inventec.Common.IntegrationError.IntegrationPartner Partner
        {
            get { return Inventec.Common.IntegrationError.IntegrationPartner.Ai("hệ thống kiểm tra tương tác thuốc"); }
        }

        public static T CreateRequest<T>(string baseUri, string requestUri, object sendData)
        {
            Uri baseAddress;
            if (String.IsNullOrWhiteSpace(baseUri) || !Uri.TryCreate(baseUri, UriKind.Absolute, out baseAddress))
                throw new Inventec.Common.IntegrationError.IntegrationException(Inventec.Common.IntegrationError.ErrorSource.Config, Partner, "Địa chỉ hệ thống kiểm tra tương tác thuốc trong cấu hình chưa có hoặc sai định dạng: " + baseUri);
            using (var client = new HttpClient())
            {
                client.BaseAddress = baseAddress;
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.Timeout = new TimeSpan(0, 0, 90);

                HttpResponseMessage resp = null;
                string sendJsonData = JsonConvert.SerializeObject(sendData);
                Inventec.Common.Logging.LogSystem.Info("_____sendJsonData : " + sendJsonData);

                string extension = baseUri.Substring(baseUri.IndexOf('/', baseUri.IndexOf("//") + 2));
                string fullrequestUri = requestUri;
                if (!requestUri.Contains(extension))
                {
                    fullrequestUri = extension + requestUri;
                }

                string uriForMessage = baseUri.TrimEnd('/') + "/" + requestUri.TrimStart('/');
                try
                {
                    resp = client.PostAsync(fullrequestUri, new StringContent(sendJsonData, Encoding.UTF8, "application/json")).Result;
                }
                catch (Exception ex)
                {
                    // vCong 62522: timeout/DNS/SSL/khong ket noi -> exception co nguon
                    throw Inventec.Common.IntegrationError.IntegrationHttpError.WrapCall(ex, Partner, uriForMessage);
                }

                if (resp == null || !resp.IsSuccessStatusCode)
                {
                    Inventec.Common.IntegrationError.IntegrationException httpEx = Inventec.Common.IntegrationError.IntegrationHttpError.FromStatus(Partner, uriForMessage, resp);
                    Inventec.Common.Logging.LogSystem.Error(string.Format("Loi khi goi API: {0}{1}. {2}", baseUri, requestUri, httpEx.Message));
                    throw httpEx;
                }

                string responseData = Inventec.Common.IntegrationError.IntegrationHttpError.ReadBody(resp);
                Inventec.Common.Logging.LogSystem.Info("HIS.Desktop.Plugins.Library.DrugInterventionInfo__________________api responseData: " + responseData);

                T data;
                try
                {
                    data = JsonConvert.DeserializeObject<T>(responseData);
                }
                catch (Exception ex)
                {
                    throw Inventec.Common.IntegrationError.IntegrationHttpError.ParseError(Partner, uriForMessage, responseData, ex);
                }
                if (data == null)
                {
                    throw Inventec.Common.IntegrationError.IntegrationHttpError.ParseError(Partner, uriForMessage, responseData, null);
                }
                return data;
            }
        }
    }
}
