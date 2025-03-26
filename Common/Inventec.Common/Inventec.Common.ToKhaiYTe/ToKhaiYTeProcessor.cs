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
using Inventec.Common.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.ToKhaiYTe
{
    public class ToKhaiYTeProcessor
    {
        public static TkytUserADO GetUserInfo(string qrcode, string phoneNumber, string serverUrl)
        {
            TkytUserADO result = null;
            try
            {
                if (String.IsNullOrWhiteSpace(qrcode) || String.IsNullOrWhiteSpace(phoneNumber) || String.IsNullOrWhiteSpace(serverUrl))
                {
                    LogSystem.Error("QrCode or PhoneNumber or ServerUrl is null or empty");
                    return null;
                }

                string userId = GetUserId(qrcode);

                if (String.IsNullOrWhiteSpace(userId))
                {
                    LogSystem.Error("UserId is null or empty");
                    return null;
                }

                TkytRegisterResponseADO token = Register(phoneNumber, serverUrl);
                if (token == null || token.User == null || String.IsNullOrWhiteSpace(token.User.token))
                {
                    LogSystem.Error("Khong lay duoc token theo sdt: " + LogUtil.TraceData("token", token));
                }

                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(serverUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Add("Authorization", token.User.token);

                    string requestedUrl = String.Format("/{0}/user_health_declarations?from=0&to=10&type=all", userId);

                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                    HttpResponseMessage resp = client.GetAsync(requestedUrl).Result;
                    if (resp.IsSuccessStatusCode)
                    {
                        string responseData = resp.Content.ReadAsStringAsync().Result;
                        try
                        {
                            LogSystem.Info("responseData: " + responseData);

                            TkytListKhaiBaoADO lstToKhai = JsonConvert.DeserializeObject<TkytListKhaiBaoADO>(responseData);
                            if (lstToKhai == null || lstToKhai.User == null)
                            {
                                LogSystem.Error("User info is empty: \n" + LogUtil.TraceData("lstToKhai", lstToKhai));
                                return null;
                            }
                            result = lstToKhai.User;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                    else
                    {
                        try
                        {
                            string responseData = resp.Content.ReadAsStringAsync().Result;
                            LogSystem.Warn("GetUserInfo that bai: " + responseData);
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }

        private static string GetUserId(string qrcode)
        {
            string result = null;
            try
            {
                string[] array = qrcode.Split('|');
                if (array.Length != 3 || array[0] != "user")
                {
                    LogSystem.Warn("qrcode is invalid: " + qrcode);
                    return null;
                }

                result = array[1];
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }

        private static TkytRegisterResponseADO Register(string phoneNumber, string serverUrl)
        {
            TkytRegisterResponseADO result = null;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(serverUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Add("Authorization", "eyJ0eXBlIjoiand0IiwiYWxnIjoiUlM1MTIiLCJraWQiOiIwIn0.eyJzdWIiOiI0OTI3MTg2Njc4NzU1MDgiLCJhdWQiOiIxMjM0LTU2NzgtOTEwIiwibmJmIjoxNjI3ODcyODUxLCJpc3MiOiJzc28uc253LmNvbSIsImFsaWFzIjoiNDkyNzE4NjY3ODc1NTA4LTk4NiIsImlkIjoiNDkyNzE4NjY3ODc1NTA4IiwiZXhwIjoxNzIyNTY3MjgxLCJpYXQiOjE2Mjc4NzI4NTEsImVtYWlsIjoiVENETCJ9.hu8cGJZUrvjKSXI8iRK65uARh9faxYeuatWvT9kr_4ZyespGcbTt60CgzKgFP2O6CEiWyHyOHz2TFytqa7ns9KOw9XgXbFd_ZmC2baeCRfygbn0nsfYydZGpO6DjtxZBmAo-3XjiZeLOLxsU1M0Zc_BFhcPFvIEaoqqndpLGQjZOJxHex0-RsknQn0TGYKQV9C5tTKRHo50DpVTjCn5pLDzS5RoBV1OYX5aM2FjFcsuOyU0N_pANCPTXUOnHnkvU6NygCbzFn1MX2wyO4kUB4wua7SIsGEQHiFbQeuGHdlxjdbjpCT1B4GnrAdewnVS8FzDajUQB65-fOfAdtIZIFA");

                    TkytRegisterADO ado = new TkytRegisterADO();
                    ado.phoneNumber = phoneNumber;

                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                    HttpResponseMessage resp = client.PostAsJsonAsync("/register", ado).Result;
                    if (resp.IsSuccessStatusCode)
                    {
                        string responseData = resp.Content.ReadAsStringAsync().Result;
                        try
                        {
                            result = JsonConvert.DeserializeObject<TkytRegisterResponseADO>(responseData);
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                    else
                    {
                        try
                        {
                            string responseData = resp.Content.ReadAsStringAsync().Result;
                            LogSystem.Warn("Register that bai: " + responseData);
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }
    }
}
