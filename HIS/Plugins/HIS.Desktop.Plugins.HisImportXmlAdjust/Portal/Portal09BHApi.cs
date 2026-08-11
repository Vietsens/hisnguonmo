using Inventec.Common.Logging;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;

namespace HIS.Desktop.Plugins.HisImportXmlAdjust.Portal
{
    /// <summary>
    /// Token phiên làm việc do dịch vụ lấy token cấp (tài liệu mục I).
    /// Token dùng cho các lời gọi nghiệp vụ tiếp theo, thời hạn theo cấu hình phiên (mặc định 10 phút).
    /// </summary>
    public class Portal09BHToken
    {
        /// <summary>access_token - gửi ở header accessToken.</summary>
        public string AccessToken { get; set; }

        /// <summary>id_token - gửi ở header tokenId.</summary>
        public string IdToken { get; set; }

        /// <summary>token_type (Bearer).</summary>
        public string TokenType { get; set; }

        public string Username { get; set; }

        /// <summary>expires_in - thời điểm hết hạn (giờ UTC theo cổng trả về).</summary>
        public DateTime? ExpiresInUtc { get; set; }

        /// <summary>Mã kết quả của dịch vụ lấy token (tài liệu mục I.4).</summary>
        public string MaKetQua { get; set; }

        public string ErrorMessage { get; set; }

        /// <summary>Token có đủ dữ liệu để gọi dịch vụ nghiệp vụ hay không.</summary>
        public bool HasToken()
        {
            return !string.IsNullOrEmpty(this.AccessToken);
        }

        /// <summary>
        /// Còn hạn dùng hay không. Trừ hao 30 giây để không gửi hồ sơ bằng token sắp hết hạn giữa chừng.
        /// Cổng không trả expires_in thì coi như còn hạn, hết hạn thật sẽ bị bắt bằng HTTP 401 rồi lấy token mới.
        /// </summary>
        public bool IsAlive()
        {
            if (!HasToken()) return false;
            if (!this.ExpiresInUtc.HasValue) return true;
            return this.ExpiresInUtc.Value.AddSeconds(-30) > DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Kết quả đẩy hồ sơ điều chỉnh mẫu 09/BH lên cổng tiếp nhận BHXH (tài liệu mục II.3).
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
    /// Theo tài liệu MoTaAPI_GuiHoSoDieuChinh09BH - quy trình 2 bước:
    ///   (1) POST api/token/take                        -> lấy token phiên làm việc
    ///   (2) POST api/HSDCTT12/GuiHoSoDieuChinh09BH     -> gửi 01 hồ sơ điều chỉnh kèm token
    /// Cả hai đều là POST, body JSON utf-8, trả về JSON.
    ///
    /// Token được giữ lại trong instance và dùng lại cho cả lô hồ sơ (token có hạn ~10 phút), chỉ lấy mới khi
    /// hết hạn hoặc khi cổng trả HTTP 401 - tránh đăng nhập lại cho từng hồ sơ.
    /// </summary>
    public class Portal09BHApi
    {
        /// <summary>Loại hồ sơ cố định = 73 (hồ sơ điều chỉnh 09/BH) - tài liệu mục II.2.</summary>
        private const int LOAI_HO_SO = 73;
        private const string TOKEN_URI = "api/token/take";
        private const string SEND_URI = "api/HSDCTT12/GuiHoSoDieuChinh09BH";
        private const string MA_KET_QUA_THANH_CONG = "200";

        /// <summary>Token đang dùng cho cả lô. Chỉ lấy lại khi hết hạn hoặc cổng trả 401.</summary>
        private Portal09BHToken currentToken;

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
        /// BƯỚC 1 - Lấy token phiên làm việc (tài liệu mục I).
        /// Luôn gọi cổng để lấy token mới, dùng khi bắt đầu lô hoặc khi token cũ hết hạn.
        /// </summary>
        /// <param name="address">Địa chỉ cổng (VD: https://daotaogdbhyt.baohiemxahoi.gov.vn)</param>
        /// <param name="username">Tên đăng nhập của CSKCB (kết thúc bằng "BV")</param>
        /// <param name="password">Mật khẩu</param>
        /// <param name="maCsKCB">Mã cơ sở khám chữa bệnh</param>
        public Portal09BHToken TakeToken(string address, string username, string password, string maCsKCB)
        {
            Portal09BHToken token = new Portal09BHToken();
            try
            {
                EnsureTls();

                if (string.IsNullOrEmpty(address) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    token.ErrorMessage = "Chưa cấu hình thông tin kết nối cổng BHXH.";
                    return token;
                }

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(address);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var bodyObj = new { username = username, password = password, maCSKCB = maCsKCB ?? "" };
                    var content = new StringContent(JsonConvert.SerializeObject(bodyObj), Encoding.UTF8, "application/json");

                    LogSystem.Info(string.Format("[DAY_CONG_09BH] TOKEN REQUEST -> URL={0}{1} | username={2} | maCSKCB={3}",
                        address.TrimEnd('/') + "/", TOKEN_URI, username, maCsKCB ?? ""));

                    HttpResponseMessage response = client.PostAsync(TOKEN_URI, content).ConfigureAwait(false).GetAwaiter().GetResult();
                    string body = response.Content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult();

                    LogSystem.Info(string.Format("[DAY_CONG_09BH] TOKEN RESPONSE <- HTTP {0} ({1})",
                        (int)response.StatusCode, response.StatusCode));

                    LoginResult plv = null;
                    try { plv = JsonConvert.DeserializeObject<LoginResult>(body); }
                    catch (Exception exParse) { LogSystem.Warn(exParse); }

                    if (plv != null) token.MaKetQua = plv.maKetQua;

                    if (plv != null && plv.APIKey != null && !string.IsNullOrEmpty(plv.APIKey.access_token))
                    {
                        token.AccessToken = plv.APIKey.access_token;
                        token.IdToken = plv.APIKey.id_token;
                        token.TokenType = plv.APIKey.token_type;
                        token.Username = plv.APIKey.username;
                        token.ExpiresInUtc = ParseExpires(plv.APIKey.expires_in);
                    }
                    else
                    {
                        // Cổng trả mã lỗi trong maKetQua (401/402/403/500), HTTP có thể vẫn là 200
                        token.ErrorMessage = !string.IsNullOrEmpty(token.MaKetQua)
                            ? MapTokenErrorCode(token.MaKetQua)
                            : "Không đăng nhập được cổng BHXH (HTTP " + (int)response.StatusCode + "). Kiểm tra tài khoản/mật khẩu/mã CSKCB cấu hình.";
                    }

                    LogSystem.Info(string.Format(
                        "[DAY_CONG_09BH] TOKEN KẾT QUẢ: maKetQua={0} | hasAccessToken={1} | hasTokenId={2} | expires_in={3}",
                        token.MaKetQua ?? "(null)", token.HasToken(), !string.IsNullOrEmpty(token.IdToken),
                        plv != null && plv.APIKey != null ? plv.APIKey.expires_in : ""));
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                token.ErrorMessage = "Lỗi khi lấy token cổng BHXH: " + ex.Message;
            }
            return token;
        }

        /// <summary>
        /// Lấy token dùng cho lô: còn hạn thì dùng lại, hết hạn/chưa có thì lấy mới.
        /// </summary>
        public Portal09BHToken EnsureToken(string address, string username, string password, string maCsKCB)
        {
            if (this.currentToken != null && this.currentToken.IsAlive())
                return this.currentToken;

            this.currentToken = TakeToken(address, username, password, maCsKCB);
            return this.currentToken;
        }

        /// <summary>Bỏ token đang giữ, lần gửi sau sẽ đăng nhập lại.</summary>
        public void ResetToken()
        {
            this.currentToken = null;
        }

        /// <summary>
        /// BƯỚC 2 - Gửi 01 hồ sơ điều chỉnh 09/BH (tài liệu mục II).
        /// Tự lấy token nếu chưa có/hết hạn; cổng trả HTTP 401 thì lấy token mới và gửi lại đúng 1 lần.
        /// </summary>
        /// <param name="address">Địa chỉ cổng (VD: https://daotaogdbhyt.baohiemxahoi.gov.vn)</param>
        /// <param name="username">Tài khoản đăng nhập CSKCB (kết thúc bằng "BV")</param>
        /// <param name="password">Mật khẩu (gửi trong body, băm MD5 ở header passwordHash)</param>
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
                    "[DAY_CONG_09BH] BẮT ĐẦU gửi hồ sơ. address={0}, username={1}, maCsKCB={2}, kyQT={3}, maTinh={4}",
                    address, username, maCsKCB ?? "", kyQT ?? "", maTinh ?? ""));
                EnsureTls();

                if (string.IsNullOrEmpty(address) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    result.ErrorMessage = "Chưa cấu hình thông tin kết nối cổng BHXH.";
                    return result;
                }
                if (string.IsNullOrEmpty(fileHsBase64))
                {
                    result.ErrorMessage = "Nội dung hồ sơ rỗng, không gửi được.";
                    return result;
                }

                Portal09BHToken token = EnsureToken(address, username, password, maCsKCB);
                if (token == null || !token.HasToken())
                {
                    result.ErrorMessage = (token != null && !string.IsNullOrEmpty(token.ErrorMessage))
                        ? token.ErrorMessage
                        : "Không đăng nhập được cổng BHXH.";
                    return result;
                }

                bool tokenRefreshed = false;
                while (true)
                {
                    HttpStatusCode statusCode;
                    string body = PostHoSo(address, username, password, maCsKCB, kyQT, maTinh, fileHsBase64, token, out statusCode);

                    // Token hết hạn giữa lô -> lấy token mới và gửi lại đúng 1 lần
                    if (statusCode == HttpStatusCode.Unauthorized && !tokenRefreshed)
                    {
                        LogSystem.Warn("[DAY_CONG_09BH] Cổng trả HTTP 401 -> lấy token mới rồi gửi lại hồ sơ.");
                        tokenRefreshed = true;
                        ResetToken();
                        token = EnsureToken(address, username, password, maCsKCB);
                        if (token == null || !token.HasToken())
                        {
                            result.ErrorMessage = (token != null && !string.IsNullOrEmpty(token.ErrorMessage))
                                ? token.ErrorMessage
                                : "Không đăng nhập lại được cổng BHXH.";
                            return result;
                        }
                        continue;
                    }

                    if (body == null)
                    {
                        result.ErrorMessage = "Lỗi gọi API gửi hồ sơ. Mã HTTP: " + (int)statusCode;
                        return result;
                    }

                    SendResponse sr = null;
                    try { sr = JsonConvert.DeserializeObject<SendResponse>(body); }
                    catch (Exception exParse) { LogSystem.Warn(exParse); }

                    if (sr == null || string.IsNullOrEmpty(sr.maKetQua))
                    {
                        result.ErrorMessage = "Không đọc được phản hồi từ cổng. Response: " + body;
                        return result;
                    }

                    result.MaKetQua = sr.maKetQua;
                    result.MaGiaoDich = sr.maGiaoDich;
                    result.ThongDiep = sr.thongDiep;
                    result.ThoiGianTiepNhan = sr.thoiGianTiepNhan;
                    result.Success = sr.maKetQua == MA_KET_QUA_THANH_CONG;
                    if (!result.Success)
                    {
                        // Ghép cả mô tả mã lỗi theo tài liệu và thông điệp cổng trả về để người dùng biết đường sửa
                        string moTaMa = MapErrorCode(sr.maKetQua);
                        result.ErrorMessage = string.IsNullOrEmpty(sr.thongDiep)
                            ? moTaMa
                            : string.Format("{0} ({1})", sr.thongDiep, moTaMa);
                    }

                    LogSystem.Info(string.Format(
                        "[DAY_CONG_09BH] KẾT QUẢ: Success={0} | maKetQua={1} | maGiaoDich={2} | thoiGianTiepNhan={3} | thongDiep={4}",
                        result.Success, result.MaKetQua, result.MaGiaoDich, result.ThoiGianTiepNhan, result.ThongDiep));
                    return result;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                result.ErrorMessage = "Lỗi khi đẩy cổng: " + ex.Message;
            }
            return result;
        }

        /// <summary>
        /// Gọi API gửi hồ sơ. Trả về body JSON; null khi HTTP lỗi (statusCode trả ra để caller xử lý 401).
        /// Header: accessToken / tokenId / passwordHash (tài liệu mục II.2.a).
        /// </summary>
        private string PostHoSo(string address, string username, string password, string maCsKCB, string kyQT,
            string maTinh, string fileHsBase64, Portal09BHToken token, out HttpStatusCode statusCode)
        {
            statusCode = 0;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(address);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("accessToken", token.AccessToken);
                client.DefaultRequestHeaders.Add("tokenId", token.IdToken ?? "");
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
                statusCode = resp.StatusCode;
                string body = resp.Content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult();

                LogSystem.Info(string.Format(
                    "[DAY_CONG_09BH] RESPONSE <- HTTP {0} ({1}) | body={2}",
                    (int)resp.StatusCode, resp.StatusCode, body));

                // Cổng có thể trả mã nghiệp vụ kèm HTTP 4xx nhưng vẫn có body JSON -> vẫn đọc để lấy thongDiep
                if (!resp.IsSuccessStatusCode && string.IsNullOrEmpty(body))
                    return null;
                return body;
            }
        }

        /// <summary>Đọc expires_in (VD "2026-07-27T01:37:07.9726Z") thành DateTime UTC.</summary>
        private static DateTime? ParseExpires(string expiresIn)
        {
            if (string.IsNullOrEmpty(expiresIn)) return null;
            try
            {
                DateTime value;
                if (DateTime.TryParse(expiresIn, CultureInfo.InvariantCulture,
                        DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out value))
                    return value;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
            return null;
        }

        /// <summary>Mã kết quả dịch vụ lấy token (tài liệu mục I.4).</summary>
        private static string MapTokenErrorCode(string ma)
        {
            switch (ma)
            {
                case "200": return "Lấy token thành công";
                case "401": return "Tài khoản không tồn tại hoặc sai thông tin đăng nhập";
                case "402": return "Mã cơ sở KCB không đúng";
                case "403": return "Tài khoản đã bị khóa";
                case "500": return "Lỗi hệ thống cổng BHXH";
                default: return "Cổng trả về mã kết quả khi lấy token: " + ma;
            }
        }

        /// <summary>Mã kết quả dịch vụ gửi hồ sơ điều chỉnh (tài liệu mục II.4).</summary>
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
