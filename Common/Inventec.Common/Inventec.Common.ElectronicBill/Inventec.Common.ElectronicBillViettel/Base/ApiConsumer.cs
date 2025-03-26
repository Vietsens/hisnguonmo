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
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Inventec.Common.ElectronicBillViettel.Base
{
    internal class ApiConsumer
    {
        private string baseUriAcs;
        private string UserName;
        private string Password;

        public ApiConsumer(string uri, string userName, string password)
        {
            this.baseUriAcs = uri;
            this.UserName = userName;
            this.Password = password;
        }

        public T CreateRequest<T>(string requestUri, object sendData)
        {
            using (var client = new HttpClient())
            {
                string authInfo = string.Format("{0}:{1}", this.UserName, this.Password);
                client.BaseAddress = new Uri(this.baseUriAcs);
                client.DefaultRequestHeaders.Accept.Clear();
                client.Timeout = new TimeSpan(0, 0, Constants.TIME_OUT);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Add("Authorization", string.Format("{0} {1}", "Basic", Convert.ToBase64String(Encoding.Default.GetBytes(authInfo))));

                HttpResponseMessage resp = null;
                try
                {
                    string sendJsonData = JsonConvert.SerializeObject(sendData);
                    Inventec.Common.Logging.LogSystem.Debug("_____sendJsonData : " + sendJsonData);
                    resp = client.PostAsync(requestUri, new StringContent(sendJsonData, Encoding.UTF8, "application/json")).Result;
                }
                catch (HttpRequestException ex)
                {
                    throw new Exception("Lỗi kết nối đến cổng hóa đơn điện tử");
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                if (resp == null || !resp.IsSuccessStatusCode)
                {
                    int statusCode = resp.StatusCode.GetHashCode();
                    if (resp.Content != null)
                    {
                        try
                        {
                            string errorData = resp.Content.ReadAsStringAsync().Result;
                            Inventec.Common.Logging.LogSystem.Error("errorData: " + errorData);
                        }
                        catch { }
                    }

                    throw new Exception(string.Format("Cổng hóa đơn điện tử trả về thông tin lỗi. Mã lỗi: {0}", statusCode));
                }
                string responseData = resp.Content.ReadAsStringAsync().Result;

                T data = JsonConvert.DeserializeObject<T>(responseData);
                if (data == null)
                {
                    throw new Exception("Dữ liệu cổng hóa đơn điện tử trả về không đúng");
                }
                return data;
            }
        }

        public T RequestFormData<T>(string requestUri, object postData)
        {
            if (requestUri != null && postData != null)
            {
                using (var client = new HttpClient())
                {
                    string authInfo = string.Format("{0}:{1}", this.UserName, this.Password);
                    client.BaseAddress = new Uri(this.baseUriAcs);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.Timeout = new TimeSpan(0, 0, Constants.TIME_OUT);
                    client.DefaultRequestHeaders.Add("Accept", "application/json");
                    client.DefaultRequestHeaders.Add("Authorization", string.Format("{0} {1}", "Basic", Convert.ToBase64String(Encoding.Default.GetBytes(authInfo))));

                    string url = "";
                    url += requestUri + "?";
                    System.Reflection.PropertyInfo[] pi = Inventec.Common.Repository.Properties.Get(postData.GetType());
                    foreach (var item in pi)
                    {
                        var val = item.GetValue(postData);
                        if (val != null)
                        {
                            url += string.Format("{0}={1}&", item.Name, HttpUtility.UrlEncode(val.ToString()));
                        }
                    }

                    if (url.EndsWith("&"))
                    {
                        url = url.Substring(0, url.Length - 1);
                    }
                    Inventec.Common.Logging.LogSystem.Debug("___GET URL: " + url);

                    HttpResponseMessage resp = null;
                    try
                    {
                        resp = client.GetAsync(url).Result;
                    }
                    catch (HttpRequestException ex)
                    {
                        throw new Exception("Lỗi kết nối đến cổng hóa đơn điện tử");
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                    if (resp == null || !resp.IsSuccessStatusCode)
                    {
                        int statusCode = resp.StatusCode.GetHashCode();
                        throw new Exception(string.Format("Cổng hóa đơn điện tử trả về thông tin lỗi. Mã lỗi: {0}", statusCode));
                    }
                    string responseData = resp.Content.ReadAsStringAsync().Result;

                    T data = JsonConvert.DeserializeObject<T>(responseData);
                    if (data == null)
                    {
                        throw new Exception("Dữ liệu cổng hóa đơn điện tử trả về không đúng");
                    }
                    return data;
                }
            }
            else return default(T);
        }

    }
}
