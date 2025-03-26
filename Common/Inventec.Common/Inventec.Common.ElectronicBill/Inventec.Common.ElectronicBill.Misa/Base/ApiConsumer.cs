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
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Inventec.Common.ElectronicBill.Misa.Base
{
    class ApiConsumer
    {
        private string baseUri;
        private string UserName;
        private string Password;
        private string AppID;
        private string TaxCode;
        private static Dictionary<string, string> DicAuthorization = new Dictionary<string, string>();

        public ApiConsumer(string uri, string appID, string taxCode, string userName, string password)
        {
            this.baseUri = uri;
            this.UserName = userName;
            this.Password = password;
            this.AppID = appID;
            this.TaxCode = taxCode;
        }

        public T CreateRequest<T>(string requestUri, object sendData)
        {
            string authInfo = GetAuthInfo();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.baseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.Timeout = new TimeSpan(0, 0, 90);
                client.DefaultRequestHeaders.Add("Authorization", string.Format("{0} {1}", "Bearer", authInfo));

                HttpResponseMessage resp = null;
                try
                {
                    string extension = this.baseUri.Substring(this.baseUri.IndexOf('/', this.baseUri.IndexOf("//") + 2));
                    string sendJsonData = JsonConvert.SerializeObject(sendData);
                    Inventec.Common.Logging.LogSystem.Info("_____sendJsonData : " + sendJsonData);
                    resp = client.PostAsync(extension + requestUri, new StringContent(sendJsonData, Encoding.UTF8, "application/json")).Result;
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
                    Inventec.Common.Logging.LogSystem.Error(string.Format("Loi khi goi API: {0}{1}. StatusCode: {2}", this.baseUri, requestUri, statusCode));
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
            try
            {
                if (DicAuthorization.ContainsKey(UserName) && !String.IsNullOrWhiteSpace(DicAuthorization[UserName]))
                {
                    result = DicAuthorization[UserName];
                }
                else
                {
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(this.baseUri);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.Timeout = new TimeSpan(0, 0, 90);

                        string extension = this.baseUri.Substring(this.baseUri.IndexOf('/', this.baseUri.IndexOf("//") + 2));
                        HttpResponseMessage resp = null;
                        string uri = extension + string.Format(RequestUriStore.ApiLogin, HttpUtility.UrlEncode(AppID), HttpUtility.UrlEncode(TaxCode), HttpUtility.UrlEncode(UserName), HttpUtility.UrlEncode(Password));
                        Inventec.Common.Logging.LogSystem.Info(uri);
                        try
                        {
                            resp = client.PostAsync(uri, null).Result;
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
                            throw new Exception(string.Format("Loi khi goi API: {0}{1}", this.baseUri, uri));
                        }

                        string responseData = resp.Content.ReadAsStringAsync().Result;
                        if (!resp.IsSuccessStatusCode)
                        {
                            int statusCode = resp.StatusCode.GetHashCode();
                            Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => responseData), responseData));
                            throw new Exception(string.Format("Loi khi goi API: {0}{1}. StatusCode: {2}", this.baseUri, uri, statusCode));
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
            }
            catch (Exception ex)
            {
                result = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }
}
