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
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Inventec.Common.ElectronicBillViettel.Base
{
    class ApiConsumerV2
    {
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
            request.Timeout = Constants.TIME_OUT * 1000;

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

                            if (!api.Contains(Base.RequestUriStore.GetFileInvoiceV2))
                            {
                                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("_______Api Response Result: ", resultstring));
                            }

                            if (!(string.IsNullOrWhiteSpace(resultstring)))
                            {
                                result = JsonConvert.DeserializeObject<T>(resultstring);
                            }
                        }
                    }
                    else if (response.StatusCode == HttpStatusCode.InternalServerError)
                    {
                        LoginProcess.SetToken(username, password, null, true);
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
    }
}
