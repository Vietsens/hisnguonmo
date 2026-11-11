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
        [ThreadStatic]
        private static string lastErrorMessage;

        /// <summary>vCong 62522: thông báo lỗi (đã phân loại nguồn) của lần GetUserInfo gần nhất; null nếu thành công/không có lỗi.</summary>
        public static string LastErrorMessage
        {
            get { return lastErrorMessage; }
        }

        private static Inventec.Common.IntegrationError.IntegrationPartner Partner
        {
            get { return Inventec.Common.IntegrationError.IntegrationPartner.HealthPortal("hệ thống tờ khai y tế"); }
        }

        public static TkytUserADO GetUserInfo(string qrcode, string phoneNumber, string serverUrl)
        {
            TkytUserADO result = null;
            lastErrorMessage = null;
            try
            {
                if (String.IsNullOrWhiteSpace(qrcode) || String.IsNullOrWhiteSpace(phoneNumber) || String.IsNullOrWhiteSpace(serverUrl))
                {
                    LogSystem.Error("QrCode or PhoneNumber or ServerUrl is null or empty");
                    lastErrorMessage = String.IsNullOrWhiteSpace(serverUrl)
                        ? Inventec.Common.IntegrationError.IntegrationErrorMessage.ConfigError(Partner, "Chưa thiết lập địa chỉ hệ thống tờ khai y tế (KBYT_URL)")
                        : Inventec.Common.IntegrationError.IntegrationErrorMessage.BusinessError(Partner, "Thiếu mã QR hoặc số điện thoại khai báo y tế");
                    return null;
                }

                string userId = GetUserId(qrcode);

                if (String.IsNullOrWhiteSpace(userId))
                {
                    LogSystem.Error("UserId is null or empty");
                    lastErrorMessage = Inventec.Common.IntegrationError.IntegrationErrorMessage.BusinessError(Partner, "Mã QR không phải mã QR tờ khai y tế (định dạng user|id|...)");
                    return null;
                }

                TkytRegisterResponseADO token = Register(phoneNumber, serverUrl);
                if (token == null || token.User == null || String.IsNullOrWhiteSpace(token.User.token))
                {
                    LogSystem.Error("Khong lay duoc token theo sdt: " + LogUtil.TraceData("token", token));
                    // vCong 62522: truoc day roi vao NullReference o dong duoi
                    if (lastErrorMessage == null)
                        lastErrorMessage = Inventec.Common.IntegrationError.IntegrationErrorMessage.LoginError(Partner, "Hệ thống tờ khai y tế không trả về mã xác thực (token) cho số điện thoại " + phoneNumber);
                    return null;
                }

                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(serverUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Add("Authorization", token.User.token);

                    string requestedUrl = String.Format("/{0}/user_health_declarations?from=0&to=10&type=all", userId);

                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                    HttpResponseMessage resp = null;
                    try
                    {
                        resp = client.GetAsync(requestedUrl).Result;
                    }
                    catch (Exception exCall)
                    {
                        throw Inventec.Common.IntegrationError.IntegrationHttpError.WrapCall(exCall, Partner, serverUrl + requestedUrl);
                    }
                    if (resp != null && resp.IsSuccessStatusCode)
                    {
                        string responseData = resp.Content.ReadAsStringAsync().Result;
                        try
                        {
                            LogSystem.Info("responseData: " + responseData);

                            TkytListKhaiBaoADO lstToKhai = JsonConvert.DeserializeObject<TkytListKhaiBaoADO>(responseData);
                            if (lstToKhai == null || lstToKhai.User == null)
                            {
                                LogSystem.Error("User info is empty: \n" + LogUtil.TraceData("lstToKhai", lstToKhai));
                                lastErrorMessage = Inventec.Common.IntegrationError.IntegrationErrorMessage.PartnerError(Partner, null, "Hệ thống tờ khai y tế không trả về thông tin người khai báo");
                                return null;
                            }
                            result = lstToKhai.User;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                            lastErrorMessage = Inventec.Common.IntegrationError.IntegrationErrorMessage.FromException(
                                Inventec.Common.IntegrationError.IntegrationHttpError.ParseError(Partner, serverUrl + requestedUrl, responseData, ex), Partner, "đọc tờ khai y tế");
                        }
                    }
                    else
                    {
                        string responseData = Inventec.Common.IntegrationError.IntegrationHttpError.ReadBody(resp);
                        LogSystem.Warn("GetUserInfo that bai: " + responseData);
                        lastErrorMessage = Inventec.Common.IntegrationError.IntegrationErrorMessage.FromException(
                            Inventec.Common.IntegrationError.IntegrationHttpError.FromStatus(Partner, serverUrl + requestedUrl, resp), Partner, "đọc tờ khai y tế"); // vCong 62522
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                lastErrorMessage = Inventec.Common.IntegrationError.IntegrationErrorMessage.FromException(ex, Partner, "đọc tờ khai y tế");
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

                    HttpResponseMessage resp = null;
                    try
                    {
                        resp = client.PostAsJsonAsync("/register", ado).Result;
                    }
                    catch (Exception exCall)
                    {
                        throw Inventec.Common.IntegrationError.IntegrationHttpError.WrapCall(exCall, Partner, serverUrl + "/register");
                    }
                    if (resp != null && resp.IsSuccessStatusCode)
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
                        string responseData = Inventec.Common.IntegrationError.IntegrationHttpError.ReadBody(resp);
                        LogSystem.Warn("Register that bai: " + responseData);
                        lastErrorMessage = Inventec.Common.IntegrationError.IntegrationErrorMessage.FromException(
                            Inventec.Common.IntegrationError.IntegrationHttpError.FromStatus(Partner, serverUrl + "/register", resp), Partner, "đăng ký số điện thoại với hệ thống tờ khai y tế"); // vCong 62522
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                lastErrorMessage = Inventec.Common.IntegrationError.IntegrationErrorMessage.FromException(ex, Partner, "đăng ký số điện thoại với hệ thống tờ khai y tế");
                result = null;
            }
            return result;
        }
    }
}
