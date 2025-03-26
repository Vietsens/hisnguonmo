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
using Inventec.Common.ElectronicBill.Misa.Model;
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

namespace Inventec.Common.ElectronicBill.Misa.Base
{
    class ApiConsumerV2
    {
        /// <summary>
        /// Key header chứa chuỗi Token bảo mật
        /// </summary>
        public const string Authorization = "Authorization";

        /// <summary>
        /// Key header chứa taxcode
        /// </summary>
        public const string CompanyTaxCode = "CompanyTaxCode";

        private string baseUri;
        private string UserName;
        private string Password;
        private string AppID;
        private string TaxCode;
        private static Dictionary<string, string> DicAuthorization = new Dictionary<string, string>();

        public ApiConsumerV2(string uri, string appID, string taxCode, string userName, string password)
        {
            this.baseUri = uri;
            this.UserName = userName;
            this.Password = password;
            this.AppID = appID;
            this.TaxCode = taxCode;
        }

        public T CreateRequest<T>(string requestUri, object sendData)
        {
            System.Net.ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            string authInfo = GetAuthInfo();

            //string strApiURL = CombileUrl(this.baseUri, requestUri);

            //Dictionary<string, string> headers = new Dictionary<string, string>();
            //if (!string.IsNullOrWhiteSpace(authInfo))
            //{
            //    headers.Add(Authorization, string.Format("{0} {1}", "Bearer", authInfo));
            //}
            //if (!string.IsNullOrEmpty(this.TaxCode))
            //{
            //    headers.Add(CompanyTaxCode, this.TaxCode);
            //}

            //return CallWebRequest<T>(HttpMethod.Post.ToString(), strApiURL, headers, sendData);

            using (var client = new HttpClient())
            {
                string extension = this.baseUri.Substring(this.baseUri.IndexOf('/', this.baseUri.IndexOf("//") + 2));
                string uri = this.baseUri.Replace(extension, "");
                string url = extension + requestUri;

                client.BaseAddress = new Uri(uri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.Timeout = new TimeSpan(0, 0, 90);
                if (!string.IsNullOrWhiteSpace(authInfo))
                {
                    client.DefaultRequestHeaders.Add(Authorization, string.Format("{0} {1}", "Bearer", authInfo));
                }

                if (!string.IsNullOrWhiteSpace(this.TaxCode))
                {
                    client.DefaultRequestHeaders.Add(CompanyTaxCode, this.TaxCode);
                }

                HttpResponseMessage resp = null;
                try
                {
                    string sendJsonData = JsonConvert.SerializeObject(sendData);
                    Inventec.Common.Logging.LogSystem.Info("_____sendJsonData : " + sendJsonData);
                    resp = client.PostAsync(url, new StringContent(sendJsonData, Encoding.UTF8, "application/json")).Result;
                }
                catch (HttpException ex)
                {
                    if (ex.ErrorCode == 401)
                    {
                        throw new Exception("Thông tin đăng nhập không đúng");
                    }
                    else if (ex.ErrorCode == 404)
                    {
                        throw new Exception("Không tìm thấy tài nguyên");
                    }
                    else if (ex.ErrorCode == 400)
                    {
                        throw new Exception("Thông tin tham số không đúng");
                    }
                    else if (ex.ErrorCode == 500)
                    {
                        throw new Exception("Lỗi trên server. Gặp lỗi này vui lòng liên hệ với MISA để được hỗ trợ");
                    }
                    else
                    {
                        throw ex;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                if (resp == null || !resp.IsSuccessStatusCode)
                {
                    int statusCode = resp.StatusCode.GetHashCode();
                    Inventec.Common.Logging.LogSystem.Error(string.Format("Loi khi goi API: {0}{1}. StatusCode: {2}", uri, url, statusCode));
                }

                string responseData = resp.Content.ReadAsStringAsync().Result;
                //Inventec.Common.Logging.LogSystem.Info("__________________api responseData: " + responseData);

                T data = JsonConvert.DeserializeObject<T>(responseData);
                if (data == null)
                {
                    throw new Exception(string.Format("Loi khi goi API. Response {0}:", responseData));
                }
                return data;
            }
        }

        internal static T CallWebRequest<T>(string method, string api, Dictionary<string, string> headers, object parameter)
        {
            T result = default(T);
            System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)(System.Net.WebRequest.Create(api));
            request.Method = method;
            request.KeepAlive = true;
            request.Timeout = 30000;
            request.ContentType = "application/json; charset=utf-8";
            if (headers != null)
            {
                foreach (var item in headers)
                {
                    request.Headers.Add(item.Key, item.Value);
                }
            }

            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.DateTimeZoneHandling = DateTimeZoneHandling.Local;

            if (method.ToLower() != System.Net.WebRequestMethods.Http.Get.ToLower() && parameter != null)
            {
                string strParam = JsonConvert.SerializeObject(parameter, settings);

                Inventec.Common.Logging.LogSystem.Info("_____sendJsonData : " + strParam);
                byte[] byteArray = (new System.Text.UTF8Encoding()).GetBytes(strParam);
                //request.ContentLength = byteArray.Length;
                using (Stream dataStream = request.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                }
            }

            using (System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)(request.GetResponse()))
            {
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                    {
                        string resultstring = sr.ReadToEnd();
                        if (!(string.IsNullOrWhiteSpace(resultstring)))
                        {
                            result = JsonConvert.DeserializeObject<T>(resultstring, settings);
                        }
                    }
                }
            }
            return result;
        }

        internal static string CombileUrl(params string[] data)
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

        public T Sign<T>(string requestUri, object sendData)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.baseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.Timeout = new TimeSpan(0, 0, 90);
                client.DefaultRequestHeaders.Add("MisaTokenKey", "491CB943-E466-4D25-B0A9-7042594F59F2");

                HttpResponseMessage resp = null;
                try
                {
                    string sendJsonData = JsonConvert.SerializeObject(sendData);
                    Inventec.Common.Logging.LogSystem.Info("_____sendJsonData : " + sendJsonData);
                    resp = client.PostAsync(requestUri, new StringContent(sendJsonData, Encoding.UTF8, "application/json")).Result;
                }
                catch (HttpException ex)
                {
                    if (ex.ErrorCode == 401)
                    {
                        throw new Exception("Thông tin đăng nhập không đúng");
                    }
                    else if (ex.ErrorCode == 404)
                    {
                        throw new Exception("Không tìm thấy tài nguyên");
                    }
                    else if (ex.ErrorCode == 400)
                    {
                        throw new Exception("Thông tin tham số không đúng");
                    }
                    else if (ex.ErrorCode == 500)
                    {
                        throw new Exception("Lỗi trên server. Gặp lỗi này vui lòng liên hệ với MISA để được hỗ trợ");
                    }
                    else
                    {
                        throw ex;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                if (resp == null || !resp.IsSuccessStatusCode)
                {
                    int statusCode = resp.StatusCode.GetHashCode();
                    throw new Exception(string.Format("Loi khi goi API: {0}{1}. StatusCode: {2}", this.baseUri, requestUri, statusCode));
                }
                string responseData = resp.Content.ReadAsStringAsync().Result;
                Inventec.Common.Logging.LogSystem.Info("__________________api responseData: " + responseData);

                T data = JsonConvert.DeserializeObject<T>(responseData);
                if (data == null)
                {
                    throw new Exception(string.Format("Loi khi goi API. Response {0}:", responseData));
                }
                return data;
            }
        }

        private string GetAuthInfo()
        {
            string result = "";
            if (DicAuthorization.ContainsKey(UserName) && !String.IsNullOrWhiteSpace(DicAuthorization[UserName]))
            {
                result = DicAuthorization[UserName];
            }
            else
            {
                using (var client = new HttpClient())
                {
                    string extension = this.baseUri.Substring(this.baseUri.IndexOf('/', this.baseUri.IndexOf("//") + 2));
                    string uri = this.baseUri.Replace(extension, "");

                    if (extension.ToLower().Contains("/code"))
                    {
                        extension = extension.ToLower().Replace("/code", "");
                    }

                    string url = extension + RequestUriStore.ApiLoginV2;

                    client.BaseAddress = new Uri(uri);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.Timeout = new TimeSpan(0, 0, 90);
                    if (!string.IsNullOrWhiteSpace(this.TaxCode))
                    {
                        client.DefaultRequestHeaders.Add(CompanyTaxCode, this.TaxCode);
                    }

                    HttpResponseMessage resp = null;
                    //string uri = extension + string.Format(RequestUriStore.ApiLogin, HttpUtility.UrlEncode(AppID), HttpUtility.UrlEncode(TaxCode), HttpUtility.UrlEncode(UserName), HttpUtility.UrlEncode(Password));
                    try
                    {
                        GetTokenParameter data = new GetTokenParameter();
                        data.AppID = this.AppID;
                        data.TaxCode = this.TaxCode;
                        data.Password = this.Password;
                        data.UserName = this.UserName;

                        string sendJsonData = JsonConvert.SerializeObject(data);
                        resp = client.PostAsync(url, new StringContent(sendJsonData, Encoding.UTF8, "application/json")).Result;
                    }
                    catch (HttpException ex)
                    {
                        if (ex.ErrorCode == 401)
                        {
                            throw new Exception("Thông tin đăng nhập không đúng");
                        }
                        else if (ex.ErrorCode == 404)
                        {
                            throw new Exception("Không tìm thấy tài nguyên");
                        }
                        else if (ex.ErrorCode == 400)
                        {
                            throw new Exception("Thông tin tham số không đúng");
                        }
                        else if (ex.ErrorCode == 500)
                        {
                            throw new Exception("Lỗi trên server. Gặp lỗi này vui lòng liên hệ với MISA để được hỗ trợ");
                        }
                        else
                        {
                            throw ex;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                    if (resp == null)
                    {
                        throw new Exception(string.Format("Loi khi goi API: {0}{1}", uri, url));
                    }

                    string responseData = resp.Content.ReadAsStringAsync().Result;
                    if (!resp.IsSuccessStatusCode)
                    {
                        int statusCode = resp.StatusCode.GetHashCode();
                        Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => responseData), responseData));
                        throw new Exception(string.Format("Loi khi goi API: {0}{1}. StatusCode: {2}", uri, url, statusCode));
                    }

                    if (!String.IsNullOrWhiteSpace(responseData))
                    {
                        ApiResult data = JsonConvert.DeserializeObject<ApiResult>(responseData);
                        if (data.Success && !string.IsNullOrWhiteSpace(data.Data))
                        {
                            result = data.Data;
                            DicAuthorization[UserName] = data.Data;
                        }
                    }
                }
            }
            return result;
        }
    }
}
