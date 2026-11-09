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
using Inventec.Common.IntegrationError;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace HIS.Desktop.Plugins.TestServiceExecute.PACS
{
    class ApiConsumerRaw
    {
        public ApiConsumerRaw() { }
        const int TIME_OUT = 30;
        internal T PostRaw<T>(string uri, object data, int userTimeout, params object[] listParam)
        {
            T result = default(T);
            using (var client = new HttpClient())
            {
                string requestedUrl = "";
                this.HttpRequestBuilder(client, uri, ref requestedUrl, userTimeout, listParam);
                // vCong 62522: loi goi PACS mang nguon loi (ket noi / PACS tra loi / cau hinh dia chi)
                IntegrationPartner partner = IntegrationPartner.Pacs("hệ thống PACS");
                string fullUri = (client.BaseAddress != null ? client.BaseAddress.AbsoluteUri : "") + uri;
                HttpResponseMessage resp = null;
                try
                {
                    resp = client.PostAsJsonAsync(requestedUrl, data).Result;
                }
                catch (Exception ex)
                {
                    throw IntegrationHttpError.WrapCall(ex, partner, fullUri);
                }
                if (resp == null || !resp.IsSuccessStatusCode)
                {
                    Inventec.Common.Logging.LogSystem.Error(string.Format("Loi khi goi API: {0}{1}. StatusCode: {2}. Input: {3}.", client.BaseAddress.AbsoluteUri, uri, resp != null ? resp.StatusCode.GetHashCode() : 0, JsonConvert.SerializeObject(data)));
                    throw IntegrationHttpError.FromStatus(partner, fullUri, resp);
                }
                string responseData = IntegrationHttpError.ReadBody(resp);
                try
                {
                    result = JsonConvert.DeserializeObject<T>(responseData);
                }
                catch (Exception ex)
                {
                    throw IntegrationHttpError.ParseError(partner, fullUri, responseData, ex);
                }
            }
            return result;
        }

        /// <summary>
        /// Tao HttpRequest voi cac tham so cho truoc
        /// </summary>
        /// <param name="client"></param>
        /// <param name="uri"></param>
        /// <param name="requestedUrl"></param>
        /// <param name="listParam"></param>
        private void HttpRequestBuilder(HttpClient client, string uri, ref string requestedUrl, int userTimeout, params object[] listParam)
        {
            client.DefaultRequestHeaders.Add(HeaderConstants.APPLICATION_CODE_PARAM, HIS.Desktop.LocalStorage.LocalData.GlobalVariables.APPLICATION_CODE);
            client.BaseAddress = new Uri(HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_PACS);
            client.DefaultRequestHeaders.Accept.Clear();
            client.Timeout = new TimeSpan(0, 0, (userTimeout > 0 ? userTimeout : TIME_OUT));

            requestedUrl = string.Format("{0}?", uri);
            if (listParam != null && listParam.Length > 0)
            {
                if (listParam.Length % 2 != 0)
                {
                    throw new ArgumentException("Danh sach param khong hop le. So luong param phai la so chan");
                }
                for (int i = 0; i < listParam.Length; )
                {
                    requestedUrl += string.Format("{0}={1}&", HttpUtility.UrlEncode(listParam[i] + ""), HttpUtility.UrlEncode(listParam[i + 1] + ""));
                    i = i + 2;
                }
            }
        }

        public class HeaderConstants
        {
            public const string ADDRESS_PARAM = "Address";
            public const string APPLICATION_CODE_PARAM = "ApplicationCode";
            public const string BASIC_AUTH_HEADER = "Authorization";
            public const string BASIC_AUTH_HEADER_RESPONSE = "WWW-Authenticate";
            public const string BASIC_AUTH_SCHEME = "Basic";
            public const string DATA_KEY_PARAM = "DataKey";
            public const string PASS_PARAM = "Password";
        }
    }
}
