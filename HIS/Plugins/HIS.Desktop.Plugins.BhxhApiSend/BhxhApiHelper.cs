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
using HIS.Desktop.Plugins.BhxhApiSend.Entity;
using Inventec.Common.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.BhxhApiSend
{
    internal static class BhxhApiHelper
    {
        private static BhxhTokenResultADO cachedToken = null;
        private static string cachedUsername = null;
        private static readonly int TIMEOUT_SECONDS = 60;

        internal static string ConvertStringToMD5(string password)
        {
            string s_PasswordMD5 = string.Empty;
            try
            {
                byte[] encodedPassword = new UTF8Encoding().GetBytes(password);
                byte[] hash = ((HashAlgorithm)CryptoConfig.CreateFromName("MD5")).ComputeHash(encodedPassword);
                s_PasswordMD5 = BitConverter.ToString(hash).Replace("-", string.Empty);
            }
            catch (Exception ex)
            {
                LogSystem.Error("Loi khi convert chuoi sang dang ma hoa md5.", ex);
            }
            return s_PasswordMD5;
        }

        internal static async Task<BhxhTokenResultADO> Authenticate(string baseAddress, string username, string md5Password)
        {
            BhxhTokenResultADO result = null;
            try
            {
                // Kiem tra cache token
                if (cachedToken != null
                    && cachedToken.APIKey != null
                    && cachedUsername == username
                    && cachedToken.maKetQua == "200")
                {
                    try
                    {
                        var expiresIn = Convert.ToDateTime(cachedToken.APIKey.expires_in);
                        if (DateTime.Now < expiresIn)
                        {
                            return cachedToken;
                        }
                    }
                    catch (Exception ex)
                    {
                        LogSystem.Warn("Loi khi parse expires_in, se lay token moi.", ex);
                    }
                }

                try
                {
                    ServicePointManager.ServerCertificateValidationCallback +=
                        (sender, cert, chain, sslPolicyErrors) => true;
                }
                catch (Exception exx)
                {
                    LogSystem.Warn("ServicePointManager.ServerCertificateValidationCallback error:", exx);
                }

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(baseAddress);
                    client.Timeout = TimeSpan.FromSeconds(TIMEOUT_SECONDS);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var values = new Dictionary<string, string>
                    {
                        { "username", username },
                        { "password", md5Password }
                    };
                    var content = new FormUrlEncodedContent(values);

                    LogSystem.Info("BhxhApiHelper.Authenticate - Bat dau dang ky token");
                    HttpResponseMessage response = await client.PostAsync("api/token/take", content);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        result = JsonConvert.DeserializeObject<BhxhTokenResultADO>(responseBody);

                        // Cache token
                        cachedToken = result;
                        cachedUsername = username;
                    }
                    else
                    {
                        LogSystem.Error("BhxhApiHelper.Authenticate - Dang nhap that bai. StatusCode: " + response.StatusCode);
                        result = new BhxhTokenResultADO { maKetQua = ((int)response.StatusCode).ToString() };
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error("BhxhApiHelper.Authenticate - Exception: ", ex);
                result = new BhxhTokenResultADO { maKetQua = "500" };
            }
            return result;
        }

        internal static async Task<BhxhCategoryResultADO> SendCategory(
            string baseAddress,
            BhxhTokenResultADO token,
            CategoryTypeADO categoryType,
            string username,
            string md5Password,
            string maTinh,
            string maCsKCB,
            byte[] xmlFileBytes,
            string kyQuetToan)
        {
            BhxhCategoryResultADO result = null;
            try
            {
                var key = token.APIKey;
                string fileBase64 = Convert.ToBase64String(xmlFileBytes);

                try
                {
                    ServicePointManager.ServerCertificateValidationCallback +=
                        (sender, cert, chain, sslPolicyErrors) => true;
                }
                catch (Exception exx)
                {
                    LogSystem.Warn("ServicePointManager.ServerCertificateValidationCallback error:", exx);
                }

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(baseAddress);
                    client.Timeout = TimeSpan.FromSeconds(TIMEOUT_SECONDS);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    // Headers
                    client.DefaultRequestHeaders.Add("accessToken", key.access_token);
                    client.DefaultRequestHeaders.Add("tokenId", key.id_token);
                    client.DefaultRequestHeaders.Add("passwordHash", md5Password);

                    // Body
                    var bodyValues = new Dictionary<string, string>
                    {
                        { "username", username },
                        { "loaiHs", categoryType.LoaiHs },
                        { "maTinh", maTinh },
                        { "maCsKCB", maCsKCB },
                        { "fileHsBase64", fileBase64 }
                    };

                    if (categoryType.RequireKyQT && !string.IsNullOrEmpty(kyQuetToan))
                    {
                        bodyValues.Add("kyQT", kyQuetToan);
                    }

                    var content = new FormUrlEncodedContent(bodyValues);

                    LogSystem.Info("BhxhApiHelper.SendCategory - Bat dau gui " + categoryType.Code + " den " + categoryType.EndpointPath);
                    HttpResponseMessage response = await client.PostAsync(categoryType.EndpointPath.TrimStart('/'), content);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        result = JsonConvert.DeserializeObject<BhxhCategoryResultADO>(responseBody);
                    }
                    else
                    {
                        LogSystem.Error("BhxhApiHelper.SendCategory - Gui that bai. StatusCode: " + response.StatusCode);
                        string responseBody = await response.Content.ReadAsStringAsync();
                        try
                        {
                            result = JsonConvert.DeserializeObject<BhxhCategoryResultADO>(responseBody);
                        }
                        catch
                        {
                            result = new BhxhCategoryResultADO
                            {
                                maKetQua = ((int)response.StatusCode).ToString(),
                                thongDiep = "Loi ket noi den Cong BHXH. StatusCode: " + response.StatusCode
                            };
                        }
                    }
                }
            }
            catch (TaskCanceledException)
            {
                LogSystem.Error("BhxhApiHelper.SendCategory - Timeout khi gui danh muc");
                result = new BhxhCategoryResultADO
                {
                    maKetQua = "500",
                    thongDiep = "Timeout khi gửi dữ liệu đến Cổng BHXH. Vui lòng thử lại."
                };
            }
            catch (Exception ex)
            {
                LogSystem.Error("BhxhApiHelper.SendCategory - Exception: ", ex);
                result = new BhxhCategoryResultADO
                {
                    maKetQua = "500",
                    thongDiep = "Lỗi khi gửi dữ liệu: " + ex.Message
                };
            }
            return result;
        }

        internal static void ClearTokenCache()
        {
            cachedToken = null;
            cachedUsername = null;
        }
    }
}
