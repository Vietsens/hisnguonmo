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
using Inventec.Common.ElectronicBillViettel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.ElectronicBillViettel
{
    class LoginProcess
    {
        class tokenTime
        {
            public string token { get; set; }
            public DateTime LastTime { get; set; }
        }

        static Dictionary<string, tokenTime> DicToken = new Dictionary<string, tokenTime>();

        public static string Login(string address, string username, string password)
        {
            string result = "";
            try
            {
                //luôn  đăng nhập lại do chưa lưu token dạng cookies
                //result = GetToken(username, password);

                //không có thì đăng nhập
                if (String.IsNullOrWhiteSpace(result))
                {
                    LoginData ado = new LoginData();
                    ado.username = username;
                    ado.password = password;

                    string uri = address;
                    if (address.IndexOf('/', address.IndexOf("//") + 2) > 0)
                    {
                        string extension = address.Substring(address.IndexOf('/', address.IndexOf("//") + 2));
                        uri = address.Replace(extension, "");
                    }

                    string fullUrl = Base.RequestUriStore.CombileUrl(uri, Base.RequestUriStore.Login);
                    string strParam = Newtonsoft.Json.JsonConvert.SerializeObject(ado);

                    var apiLoginResult = Base.ApiConsumerV2.CallWebRequest<Model.TokenData>(System.Net.WebRequestMethods.Http.Post, fullUrl, username, password, null, "application/json", strParam);
                    if (apiLoginResult != null)
                    {
                        result = apiLoginResult.access_token;
                    }
                }
            }
            catch (Exception ex)
            {
                result = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            finally
            {
                SetToken(username, password, result);
            }

            return result;
        }

        private static string GetToken(string username, string password)
        {
            string result = "";
            try
            {
                string key = string.Format("{0}_{1}", username, password);
                if (DicToken.ContainsKey(key))
                {
                    tokenTime lastToken = DicToken[key];
                    if (lastToken != null && (lastToken.LastTime - DateTime.Now).TotalSeconds < 30)
                    {
                        result = lastToken.token;
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

        public static void SetToken(string username, string password, string token, bool isDelete = false)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(token) || isDelete)
                {
                    string key = string.Format("{0}_{1}", username, password);

                    tokenTime lastToken = new tokenTime();
                    lastToken.token = token;
                    lastToken.LastTime = DateTime.Now;
                    DicToken[key] = lastToken;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
