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
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace HIS.Desktop.Plugins.Library.ElectronicBill.Base
{
    class ApiConsumerV2
    {
        static int TIME_OUT = 90;
        /// <summary>
        /// Gọi API
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="method"></param>
        /// <param name="api"></param>
        /// <param name="headers"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static T CallWebRequest<T>(string method, string api, string username, string password, Dictionary<string, string> headers, string contentType, string parameter)
        {
            T result = default(T);
            System.Net.ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            if (api.EndsWith("&"))
            {
                api = api.Substring(0, api.Length - 1);
            }

            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("FULL API:", api));

            System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)(System.Net.WebRequest.Create(api));
            request.Method = method;
            request.KeepAlive = true;
            request.Timeout = TIME_OUT * 1000;

            request.ContentType = contentType;

            if (headers != null)
            {
                foreach (var item in headers)
                {
                    request.Headers.Add(item.Key, item.Value);
                }
            }

            if (method.ToLower() != System.Net.WebRequestMethods.Http.Get.ToLower() && !String.IsNullOrWhiteSpace(parameter))
            {
                //string strParam = JsonConvert.SerializeObject(parameter);
                byte[] byteArray = (new System.Text.UTF8Encoding()).GetBytes(parameter);
                request.ContentLength = byteArray.Length;
                using (Stream dataStream = request.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                }
            }
            try
            {
                using (System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)(request.GetResponse()))
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                        {
                            string resultstring = sr.ReadToEnd();

                            if (!(string.IsNullOrWhiteSpace(resultstring)))
                            {
                                result = JsonConvert.DeserializeObject<T>(resultstring);
                            }
                        }
                    }else if(response.StatusCode == HttpStatusCode.NotFound)
                    {
                        Inventec.Common.Logging.LogSystem.Debug(String.Format("{0} {1} {2}: {3} - {4}", "_______Api Response Result: ",response.StatusCode.ToString(), api, username, password));
                    }
                }
            }
            catch (WebException ex)
            {
                if (ex.Response == null)
                {
                    // Không có response = chưa tới được cổng (timeout, DNS, tường lửa, SSL...)
                    throw ElectronicBillErrorUtil.WrapHttpCall(ex, null, api);
                }
                using (WebResponse response = ex.Response)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)response;
                    string text = "";
                    using (Stream data = response.GetResponseStream())
                    using (var reader = new StreamReader(data))
                    {
                        text = reader.ReadToEnd();
                    }
                    throw ElectronicBillErrorUtil.HttpStatusError(null, api, (int)httpResponse.StatusCode, httpResponse.StatusDescription, text);
                }
            }

            return result;
        }

        public static T CreateRequest<T>(string method, string baseUri, string requestUri, string token, string maDvcs, string sendJsonData)
        {
            System.Net.ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                if (!String.IsNullOrWhiteSpace(token) && !String.IsNullOrWhiteSpace(maDvcs))
                {
                    client.DefaultRequestHeaders.Add("Authorization", string.Format("Bear {0};{1}", token, maDvcs));
                }else if (!String.IsNullOrWhiteSpace(token))
                {
                    client.DefaultRequestHeaders.Add("Authorization", string.Format("Bearer {0}", token));
                }

                client.Timeout = new TimeSpan(0, 0, 90);

                HttpResponseMessage resp = null;

                string fullrequestUri = requestUri;
                int index = baseUri.IndexOf('/', baseUri.IndexOf("//") + 2);
                if (index > 0)
                {
                    string extension = baseUri.Substring(index);
                    if (!requestUri.Contains(extension))
                    {
                        fullrequestUri = extension + requestUri;
                    }
                }
                Inventec.Common.Logging.LogSystem.Info(string.Format("API: {0}. sendJsonData: {1}", fullrequestUri, sendJsonData));
                try
                {
                    if (method.Equals(System.Net.WebRequestMethods.Http.Get))
                        resp = client.GetAsync(fullrequestUri).Result;
                    else
                        resp = client.PostAsync(fullrequestUri, new StringContent(sendJsonData, Encoding.UTF8, "application/json")).Result;
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                    // Timeout / không kết nối được / SSL → nguồn lỗi KẾT NỐI, giữ nguyên exception gốc bên trong
                    throw ElectronicBillErrorUtil.WrapHttpCall(ex, null, requestUri);
                }

                if (resp == null)
                {
                    throw ElectronicBillErrorUtil.NoResponse(null, "gọi " + requestUri);
                }

                if (!resp.IsSuccessStatusCode)
                {
                    int statusCode = resp.StatusCode.GetHashCode();
                    string errorData = "";
                    try
                    {
                        errorData = resp.Content != null ? resp.Content.ReadAsStringAsync().Result : "";
                    }
                    catch (Exception exRead)
                    {
                        Inventec.Common.Logging.LogSystem.Warn(exRead);
                    }
                    Inventec.Common.Logging.LogSystem.Error(string.Format("Loi khi goi API: {0}{1}. StatusCode: {2}. Response: {3}", baseUri, requestUri, statusCode, errorData));
                    throw ElectronicBillErrorUtil.HttpStatusError(null, requestUri, statusCode, resp.ReasonPhrase, errorData);
                }
                T data = default(T);
                string responseData = resp.Content.ReadAsStringAsync().Result;
                Inventec.Common.Logging.LogSystem.Info("__________________api responseData: " + responseData);
                try
                {
                    data = JsonConvert.DeserializeObject<T>(responseData);
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                    throw ElectronicBillErrorUtil.ParseError(null, requestUri, responseData, ex);
                }
                if (data == null)
                {
                    throw ElectronicBillErrorUtil.ParseError(null, requestUri, responseData, null);
                }
                return data;
            }
        }
        public static T CreateRequest<T>(string method, string baseUri, string requestUri, string token, string sendJsonData)
        {
            return CreateRequest<T>(method, baseUri, requestUri, token, null, sendJsonData);
        }
        public static byte[] CreateRequestGetByte(string method, string baseUri, string requestUri, string token, string maDvcs)
        {
            System.Net.ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                if (!String.IsNullOrWhiteSpace(token))
                {
                    client.DefaultRequestHeaders.Add("Authorization", string.Format("Bear {0};{1}", token, maDvcs));
                }

                client.Timeout = new TimeSpan(0, 0, 90);

                HttpResponseMessage resp = null;

                string fullrequestUri = requestUri;
                int index = baseUri.IndexOf('/', baseUri.IndexOf("//") + 2);
                if (index > 0)
                {
                    string extension = baseUri.Substring(index);
                    if (!requestUri.Contains(extension))
                    {
                        fullrequestUri = extension + requestUri;
                    }
                }

                try
                {
                    if (method.Equals(System.Net.WebRequestMethods.Http.Get))
                        resp = client.GetAsync(fullrequestUri).Result;
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                    throw ElectronicBillErrorUtil.WrapHttpCall(ex, null, requestUri);
                }
                if (resp == null)
                {
                    throw ElectronicBillErrorUtil.NoResponse(null, "tải file từ " + requestUri);
                }
                if (!resp.IsSuccessStatusCode)
                {
                    int statusCode = resp.StatusCode.GetHashCode();
                    string errorData = "";
                    try { errorData = resp.Content != null ? resp.Content.ReadAsStringAsync().Result : ""; }
                    catch (Exception exRead) { Inventec.Common.Logging.LogSystem.Warn(exRead); }
                    throw ElectronicBillErrorUtil.HttpStatusError(null, requestUri, statusCode, resp.ReasonPhrase, errorData);
                }

                byte[] responseData = resp.Content.ReadAsByteArrayAsync().Result;
                return responseData;
            }
        }
    }
}
