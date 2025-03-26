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
using System.Web;

namespace Inventec.Common.ElectronicBillMoit.Base
{
    internal class ApiConsumer
    {
        internal static int TIME_OUT = 90;

        private string baseUriAcs;
        private string UserName;
        private string Password;
        private string Taxcode;

        public ApiConsumer(string uri, string userName, string password, string taxcode)
        {
            this.baseUriAcs = uri;
            this.UserName = userName;
            this.Password = password;
            this.Taxcode = taxcode;
        }

        /// <summary>
        /// {authenString} = {username}:{password}:{nonce}
        /// {nonce}: là chuỗi sinh ra chỉ 1 lần duy nhất (ví dụ:Guid.NewGuid().ToString("N").ToLower()).
        /// </summary>
        /// <param name="user"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        private string CreateAuthenString()
        {
            if (string.IsNullOrWhiteSpace(UserName) || string.IsNullOrWhiteSpace(Password))
            {
                throw new Exception("user, pass khong duoc de trong");
            }

            string nonce = Guid.NewGuid().ToString("N").ToLower();
            string authenString = string.Format("{0}:{1}:{2}", UserName, Password, nonce);

            return Convert.ToBase64String(Encoding.Default.GetBytes(authenString));
        }

        internal T PostData<T>(string requestUri, object sendData)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.baseUriAcs);
                client.DefaultRequestHeaders.Accept.Clear();
                client.Timeout = new TimeSpan(0, 0, TIME_OUT);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Add("taxcode", Taxcode);
                client.DefaultRequestHeaders.Add("Authentication", CreateAuthenString());

                HttpResponseMessage resp = null;
                try
                {
                    string sendJsonData = JsonConvert.SerializeObject(sendData);
                    resp = client.PostAsync(requestUri, new StringContent(sendJsonData, Encoding.UTF8, "application/json")).Result;
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                if (resp == null || !resp.IsSuccessStatusCode)
                {
                    int statusCode = resp.StatusCode.GetHashCode();
                    throw new Exception(string.Format("Loi khi goi API: {0}{1}. StatusCode: {2}", this.baseUriAcs, requestUri, statusCode));
                }
                string responseData = resp.Content.ReadAsStringAsync().Result;

                T data = JsonConvert.DeserializeObject<T>(responseData);
                if (data == null)
                {
                    throw new Exception(string.Format("Loi khi goi API. Response {0}:", responseData));
                }
                return data;
            }
        }

        internal T GetData<T>(string requestUri, object sendData)
        {
            if (requestUri != null && sendData != null)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(this.baseUriAcs);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.Timeout = new TimeSpan(0, 0, TIME_OUT);
                    client.DefaultRequestHeaders.Add("Accept", "application/json");
                    client.DefaultRequestHeaders.Add("taxcode", Taxcode);
                    client.DefaultRequestHeaders.Add("Authentication", CreateAuthenString());

                    List<string> paramHttp = new List<string>();
                    System.Reflection.PropertyInfo[] pi = Inventec.Common.Repository.Properties.Get(sendData.GetType());
                    foreach (var item in pi)
                    {
                        var val = item.GetValue(sendData);
                        if (val != null)
                        {
                            paramHttp.Add(string.Format("{0}={1}", item.Name, HttpUtility.UrlEncode(val.ToString())));
                        }
                    }

                    string url = string.Format("{0}?{1}", requestUri, string.Join("&", paramHttp));
                    Inventec.Common.Logging.LogSystem.Info(url);

                    HttpResponseMessage resp = null;
                    try
                    {
                        resp = client.GetAsync(url).Result;
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                    if (resp == null || !resp.IsSuccessStatusCode)
                    {
                        int statusCode = resp.StatusCode.GetHashCode();
                        throw new Exception(string.Format("Loi khi goi API: {0}{1}. StatusCode: {2}", this.baseUriAcs, requestUri, statusCode));
                    }
                    string responseData = resp.Content.ReadAsStringAsync().Result;

                    T data = JsonConvert.DeserializeObject<T>(responseData);
                    if (data == null)
                    {
                        throw new Exception(string.Format("Loi khi goi API. Response {0}:", responseData));
                    }
                    return data;
                }
            }
            else return default(T);
        }
    }
}
