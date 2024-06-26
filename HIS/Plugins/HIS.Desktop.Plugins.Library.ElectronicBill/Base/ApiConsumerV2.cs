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
                using (WebResponse response = ex.Response)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)response;
                    using (Stream data = response.GetResponseStream())
                    using (var reader = new StreamReader(data))
                    {
                        string text = reader.ReadToEnd();
                        throw new Exception(text);
                    }
                }
            }

            return result;
        }

        public static T CreateRequest<T>(string method, string baseUri, string requestUri, string token, string maDvcs, string sendJsonData)
        {
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
                if (method.Equals(System.Net.WebRequestMethods.Http.Get))
                    resp = client.GetAsync(fullrequestUri).Result;
                else
                    resp = client.PostAsync(fullrequestUri, new StringContent(sendJsonData, Encoding.UTF8, "application/json")).Result;

                if (resp == null || !resp.IsSuccessStatusCode)
                {
                    int statusCode = resp.StatusCode.GetHashCode();
                    throw new Exception(string.Format("Loi khi goi API: {0}{1}. StatusCode: {2}", baseUri, requestUri, statusCode));
                }
                T data = default(T);
                string responseData = resp.Content.ReadAsStringAsync().Result;
                Inventec.Common.Logging.LogSystem.Info("__________________api responseData: " + responseData);
                try
                {
                    data = JsonConvert.DeserializeObject<T>(responseData);
                    if (data == null)
                    {
                        throw new Exception(string.Format("Loi khi goi API. Response {0}:", responseData));
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                    throw new Exception(responseData);
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

                if (method.Equals(System.Net.WebRequestMethods.Http.Get))
                    resp = client.GetAsync(fullrequestUri).Result;
                if (resp == null || !resp.IsSuccessStatusCode)
                {
                    int statusCode = resp.StatusCode.GetHashCode();
                    throw new Exception(string.Format("Loi khi goi API: {0}{1}. StatusCode: {2}", baseUri, requestUri, statusCode));
                }

                byte[] responseData = resp.Content.ReadAsByteArrayAsync().Result;
                return responseData;
            }
        }
    }
}
