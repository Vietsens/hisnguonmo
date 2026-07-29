using Inventec.Common.Logging;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;

namespace HIS.Desktop.Plugins.HisImportXmlAdjust.Portal
{
    /// <summary>
    /// Kết quả đẩy hồ sơ điều chỉnh mẫu 09/BH lên cổng tiếp nhận BHXH.
    /// </summary>
    public class Portal09BHResult
    {
        public bool Success { get; set; }
        public string MaKetQua { get; set; }
        public string MaGiaoDich { get; set; }
        public string ThongDiep { get; set; }
        public string ThoiGianTiepNhan { get; set; }
        public string ErrorMessage { get; set; }
    }

    /// <summary>
    /// Client đẩy hồ sơ điều chỉnh mẫu 09/BH (loại hồ sơ 73) lên cổng tiếp nhận BHXH.
    /// Theo tài liệu MoTaAPI_GuiHoSoDieuChinh09BH: body JSON, mỗi lần gửi 01 hồ sơ.
    /// Luồng: api/token/take (JSON) → api/HSDCTT12/GuiHoSoDieuChinh09BH (JSON).
    /// </summary>
    public class Portal09BHApi
    {
        private const int LOAI_HO_SO = 73;
        private const string TOKEN_URI = "api/token/take";
        private const string SEND_URI = "api/HSDCTT12/GuiHoSoDieuChinh09BH";

        private class TokenInfo
        {
            public string access_token { get; set; }
            public string id_token { get; set; }
            public string token_type { get; set; }
            public string username { get; set; }
            public string expires_in { get; set; }
        }

        private class LoginResult
        {
            public string maKetQua { get; set; }
            public TokenInfo APIKey { get; set; }
        }

        private class SendResponse
        {
            public string maKetQua { get; set; }
            public string maGiaoDich { get; set; }
            public string thongDiep { get; set; }
            public string thoiGianTiepNhan { get; set; }
        }

        /// <summary>
        /// Đăng nhập cổng và gửi 01 hồ sơ điều chỉnh 09/BH.
        /// </summary>
        /// <param name="address">Địa chỉ cổng (VD: https://daotaogdbhyt.baohiemxahoi.gov.vn)</param>
        /// <param name="username">Tài khoản đăng nhập CSKCB (kết thúc bằng "BV")</param>
        /// <param name="password">Mật khẩu (gửi thô trong body, băm MD5 ở header passwordHash)</param>
        /// <param name="maCsKCB">Mã cơ sở KCB (phải trùng MA_CSKCB trong XML)</param>
        /// <param name="kyQT">Kỳ quyết toán yyyyMM</param>
        /// <param name="maTinh">Mã tỉnh/thành phố của CSKCB</param>
        /// <param name="fileHsBase64">Chuỗi base64 nội dung XML đã ký</param>
        public Portal09BHResult Send(string address, string username, string password, string maCsKCB, string kyQT, string maTinh, string fileHsBase64)
        {
            Portal09BHResult result = new Portal09BHResult();
            try
            {
                LogSystem.Info(string.Format(
                    "[DAY_CONG_09BH] BẮT ĐẦU đẩy cổng. address={0}, username={1}, maCsKCB={2}, kyQT={3}, maTinh={4}",
                    address, username, maCsKCB ?? "", kyQT ?? "", maTinh ?? ""));
                EnsureTls();

                if (string.IsNullOrEmpty(address) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    result.ErrorMessage = "Chưa cấu hình thông tin kết nối cổng BHXH.";
                    return result;
                }

                LoginResult plv = RegisToken(username, password, maCsKCB, address);
                LogSystem.Info(string.Format(
                    "[DAY_CONG_09BH] Token result: maKetQua={0}, hasAccessToken={1}, hasTokenId={2}",
                    plv != null ? plv.maKetQua : "(null)",
                    plv != null && plv.APIKey != null && !string.IsNullOrEmpty(plv.APIKey.access_token),
                    plv != null && plv.APIKey != null && !string.IsNullOrEmpty(plv.APIKey.id_token)));
                if (plv == null || plv.APIKey == null || string.IsNullOrEmpty(plv.APIKey.access_token))
                {
                    result.ErrorMessage = "Không đăng nhập được cổng BHXH. Kiểm tra tài khoản/mật khẩu cấu hình.";
                    return result;
                }

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(address);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("accessToken", plv.APIKey.access_token);
                    client.DefaultRequestHeaders.Add("tokenId", plv.APIKey.id_token);
                    client.DefaultRequestHeaders.Add("passwordHash", ConvertStringToMD5(password));

                    var bodyObj = new
                    {
                        username = username,
                        password = password,
                        maCskcb = maCsKCB ?? "",
                        loaiHs = LOAI_HO_SO,
                        kyQT = kyQT ?? "",
                        maTinh = maTinh ?? "",
                        fileHsBase64 = fileHsBase64 ?? ""
                    };
                    string json = JsonConvert.SerializeObject(bodyObj);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    // Log INPUT (không log token/passwordHash và không log full base64 ở mức Info)
                    LogSystem.Info(string.Format(
                        "[DAY_CONG_09BH] REQUEST -> URL={0}{1} | username={2} | loaiHs={3} | maCskcb={4} | kyQT={5} | maTinh={6} | fileHsBase64.Length={7}",
                        address.TrimEnd('/') + "/", SEND_URI, username, LOAI_HO_SO, maCsKCB ?? "", kyQT ?? "", maTinh ?? "",
                        (fileHsBase64 ?? "").Length));
                    LogSystem.Debug("[DAY_CONG_09BH] REQUEST fileHsBase64=" + (fileHsBase64 ?? ""));

                    HttpResponseMessage resp = client.PostAsync(SEND_URI, content).ConfigureAwait(false).GetAwaiter().GetResult();
                    string body = resp.Content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult();

                    LogSystem.Info(string.Format(
                        "[DAY_CONG_09BH] RESPONSE <- HTTP {0} ({1}) | body={2}",
                        (int)resp.StatusCode, resp.StatusCode, body));

                    if (!resp.IsSuccessStatusCode)
                    {
                        if ((int)resp.StatusCode == 401)
                            result.ErrorMessage = "Không đăng nhập được cổng (HTTP 401).";
                        else
                            result.ErrorMessage = "Lỗi gọi API gửi hồ sơ. Mã HTTP: " + (int)resp.StatusCode;
                        return result;
                    }

                    SendResponse sr = null;
                    try { sr = JsonConvert.DeserializeObject<SendResponse>(body); }
                    catch (Exception exParse) { LogSystem.Warn(exParse); }

                    if (sr == null)
                    {
                        result.ErrorMessage = "Không đọc được phản hồi từ cổng. Response: " + body;
                        return result;
                    }

                    result.MaKetQua = sr.maKetQua;
                    result.MaGiaoDich = sr.maGiaoDich;
                    result.ThongDiep = sr.thongDiep;
                    result.ThoiGianTiepNhan = sr.thoiGianTiepNhan;
                    result.Success = sr.maKetQua == "200";
                    if (!result.Success)
                        result.ErrorMessage = !string.IsNullOrEmpty(sr.thongDiep) ? sr.thongDiep : MapErrorCode(sr.maKetQua);

                    LogSystem.Info(string.Format(
                        "[DAY_CONG_09BH] KẾT QUẢ: Success={0} | maKetQua={1} | maGiaoDich={2} | thoiGianTiepNhan={3} | thongDiep={4}",
                        result.Success, result.MaKetQua, result.MaGiaoDich, result.ThoiGianTiepNhan, result.ThongDiep));
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                result.ErrorMessage = "Lỗi khi đẩy cổng: " + ex.Message;
            }
            return result;
        }

        private LoginResult RegisToken(string username, string password, string maCSKCB, string address)
        {
            LoginResult plv = null;
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(address);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var bodyObj = new { username = username, password = password, maCSKCB = maCSKCB ?? "" };
                    var content = new StringContent(JsonConvert.SerializeObject(bodyObj), Encoding.UTF8, "application/json");

                    LogSystem.Info(string.Format("[DAY_CONG_09BH] TOKEN REQUEST -> URL={0}{1} | username={2}",
                        address.TrimEnd('/') + "/", TOKEN_URI, username));
                    HttpResponseMessage response = client.PostAsync(TOKEN_URI, content).ConfigureAwait(false).GetAwaiter().GetResult();
                    LogSystem.Info(string.Format("[DAY_CONG_09BH] TOKEN RESPONSE <- HTTP {0} ({1})",
                        (int)response.StatusCode, response.StatusCode));
                    if (response.IsSuccessStatusCode)
                    {
                        string body = response.Content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult();
                        plv = JsonConvert.DeserializeObject<LoginResult>(body);
                    }
                    else
                    {
                        LogSystem.Error("[DAY_CONG_09BH] Đăng nhập cổng BHXH thất bại. Mã HTTP: " + (int)response.StatusCode);
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            return plv;
        }

        /// <summary>Chuyển mã kết quả cổng trả về thành thông điệp tiếng Việt (theo tài liệu mục 4).</summary>
        private static string MapErrorCode(string ma)
        {
            switch (ma)
            {
                case "200": return "Tiếp nhận thành công";
                case "204": return "Không mở được file XML hoặc hồ sơ không có nội dung điều chỉnh";
                case "205": return "Đầu vào không đúng / không giải mã được base64 / file trống";
                case "123": return "File sai cấu trúc XSD / thiếu thẻ bắt buộc / MAU_SO không phải 09/BH / mã CSKCB không khớp / thiếu CHUKYDONVI";
                case "124": return "Lỗi nghiệp vụ nội dung (TRANGTHAI, KY_QT, NGAY_RA < NGAY_VAO, thiếu LYDO_DIEUCHINH, SOBANG_XML không hợp lệ...)";
                case "202": return "NGAY_VAO/NGAY_RA sai định dạng (yêu cầu 12 ký tự yyyyMMddHHmm)";
                case "125": return "Lỗi chữ ký số (không ký / ký sai / sai serial / chứng thư chưa đăng ký)";
                case "401": return "Lỗi xác thực tài khoản / token không hợp lệ";
                case "1001": return "File vượt quá dung lượng cho phép";
                case "500": return "Lỗi hệ thống";
                default: return "Cổng trả về mã kết quả: " + ma;
            }
        }

        private static void EnsureTls()
        {
            try
            {
                ServicePointManager.ServerCertificateValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private static string ConvertStringToMD5(string password)
        {
            string result = string.Empty;
            try
            {
                byte[] encoded = new UTF8Encoding().GetBytes(password ?? "");
                byte[] hash = ((HashAlgorithm)CryptoConfig.CreateFromName("MD5")).ComputeHash(encoded);
                result = BitConverter.ToString(hash).Replace("-", string.Empty);
            }
            catch (Exception)
            {
                LogSystem.Error("Lỗi khi convert chuỗi sang MD5.");
            }
            return result;
        }
    }
}
