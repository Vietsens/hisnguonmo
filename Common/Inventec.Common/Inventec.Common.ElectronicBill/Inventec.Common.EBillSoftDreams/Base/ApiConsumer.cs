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
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.EBillSoftDreams.Base
{
    internal class ApiConsumer
    {
        internal static int TIME_OUT = 90;

        private string baseUriAcs;
        private string UserName;
        private string Password;

        public ApiConsumer(string uri, string userName, string password)
        {
            this.baseUriAcs = uri;
            this.UserName = userName;
            this.Password = password;
        }

        /// <summary>
        /// {signature}:{nonce}:{timestamp}:{username}:{password}
        /// {nonce}: là chuỗi sinh ra chỉ 1 lần duy nhất (ví dụ:Guid.NewGuid().ToString("N").ToLower()).
        /// </summary>
        /// <param name="user"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        private string CreateAuthenString(string httpMethod)
        {
            DateTime epochStart = new DateTime(1970, 01, 01, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan timeSpan = DateTime.UtcNow - epochStart;
            string timestamp = Convert.ToUInt64(timeSpan.TotalSeconds).ToString();
            string nonce = Guid.NewGuid().ToString("N").ToUpper();
            string signatureRawData = string.Format("{0}{1}{2}", httpMethod.ToUpper(), timestamp, nonce);

            using (MD5 md5 = MD5.Create())
            {
                var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(signatureRawData));
                var signature = Convert.ToBase64String(hash);
                return string.Format("{0}:{1}:{2}:{3}:{4}", signature, nonce, timestamp, UserName, Password);
            }
        }

        internal T PostData<T>(string requestUri, object sendData)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.baseUriAcs);
                client.DefaultRequestHeaders.Accept.Clear();
                client.Timeout = new TimeSpan(0, 0, TIME_OUT);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                string authen = CreateAuthenString("POST");
                client.DefaultRequestHeaders.Add("Authentication", authen);

                string sendJsonData = JsonConvert.SerializeObject(sendData);
                HttpResponseMessage resp = null;
                try
                {
                    resp = client.PostAsync(requestUri, new StringContent(sendJsonData, Encoding.UTF8, "application/json")).Result;
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                if (resp == null || !resp.IsSuccessStatusCode)
                {
                    int statusCode = resp.StatusCode.GetHashCode();
                    throw new Exception(string.Format("Loi khi goi API: {0}{1}. StatusCode: {2}. Authentication: {3}. sendJsonData: {4}", this.baseUriAcs, requestUri, statusCode, authen, sendJsonData));
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
    }
}
