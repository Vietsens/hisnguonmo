/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 */
using Inventec.Common.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.XMLViewer130.Bhxh
{
    internal static class BhxhApiHelper
    {
        private static BhxhTokenResultADO cachedToken = null;
        private static string cachedUsername = null;
        private static readonly int TIMEOUT_SECONDS = 60;

        internal static string ConvertStringToMD5(string password)
        {
            string result = string.Empty;
            try
            {
                byte[] encodedPassword = new UTF8Encoding().GetBytes(password);
                byte[] hash = ((HashAlgorithm)CryptoConfig.CreateFromName("MD5")).ComputeHash(encodedPassword);
                result = BitConverter.ToString(hash).Replace("-", string.Empty);
            }
            catch (Exception ex)
            {
                LogSystem.Error("Loi khi convert chuoi sang dang ma hoa MD5.", ex);
            }
            return result;
        }

        internal static async Task<BhxhTokenResultADO> Authenticate(string baseAddress, string username, string md5Password)
        {
            BhxhTokenResultADO result = null;
            try
            {
                // Use cached token if still valid
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
                catch (Exception ex)
                {
                    LogSystem.Warn("ServicePointManager.ServerCertificateValidationCallback error:", ex);
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

                    LogSystem.Info("BhxhApiHelper.Authenticate - Bat dau dang ky token. baseAddress=" + baseAddress + "; username=" + username + "password =" + md5Password);
                    HttpResponseMessage response = await client.PostAsync("api/token/take", content);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        LogSystem.Debug("BhxhApiHelper.Authenticate - responseBody=" + responseBody);
                        result = JsonConvert.DeserializeObject<BhxhTokenResultADO>(responseBody);

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
                catch (Exception ex)
                {
                    LogSystem.Warn("ServicePointManager.ServerCertificateValidationCallback error:", ex);
                }

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(baseAddress);
                    client.Timeout = TimeSpan.FromSeconds(TIMEOUT_SECONDS);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    client.DefaultRequestHeaders.Add("accessToken", key.access_token);
                    client.DefaultRequestHeaders.Add("tokenId", key.id_token);
                    client.DefaultRequestHeaders.Add("passwordHash", md5Password);

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

                    LogSystem.Info("BhxhApiHelper.SendCategory - Bat dau gui " + categoryType.Code
                        + " den " + categoryType.EndpointPath
                        + "; loaiHs=" + categoryType.LoaiHs
                        + "; maTinh=" + maTinh
                        + "; maCsKCB=" + maCsKCB
                        + "; fileSizeBytes=" + xmlFileBytes.Length
                        + "; base64Length=" + fileBase64.Length
                        + "; fileHsBase64=" + fileBase64);

                    HttpResponseMessage response = await client.PostAsync(categoryType.EndpointPath.TrimStart('/'), content);

                    string responseBody = await response.Content.ReadAsStringAsync();
                    LogSystem.Debug("BhxhApiHelper.SendCategory - StatusCode=" + response.StatusCode + "; responseBody=" + responseBody);

                    if (response.IsSuccessStatusCode)
                    {
                        result = JsonConvert.DeserializeObject<BhxhCategoryResultADO>(responseBody);
                    }
                    else
                    {
                        LogSystem.Error("BhxhApiHelper.SendCategory - Gui that bai. StatusCode: " + response.StatusCode);
                        try
                        {
                            result = JsonConvert.DeserializeObject<BhxhCategoryResultADO>(responseBody);
                        }
                        catch
                        {
                            result = new BhxhCategoryResultADO
                            {
                                maKetQua = ((int)response.StatusCode).ToString(),
                                thongDiep = "Lỗi kết nối đến Cổng BHXH. StatusCode: " + response.StatusCode
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
