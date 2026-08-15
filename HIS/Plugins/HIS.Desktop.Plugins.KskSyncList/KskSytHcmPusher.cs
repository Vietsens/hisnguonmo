/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 *
 * Gửi gói dữ liệu khám sức khỏe lên Nền tảng KSK Sở Y tế TP.HCM — mẫu phiếu M3
 * (người từ 18 đến 59 tuổi). Đây là cổng liên thông THỨ NĂM, cơ chế khác hoàn toàn
 * 4 cổng theo QĐ 1551: không dùng envelope 12 khối, không base64, mà là JSON phẳng
 * theo khối, ký bằng chữ ký số đặt trong phần đầu bản tin.
 *
 * TRÌNH TỰ MỘT LƯỢT GỬI
 *   1. Lấy phiếu truy cập: POST {địa chỉ xác thực}/hin-auth/getToken  {username, password}
 *      -> result.access_token, hiệu lực 2 giờ.
 *   2. Rút gọn bản tin JSON (bỏ hết khoảng trắng định dạng).
 *   3. Tính chữ ký (mục 7 của đặc tả):
 *          A = SHA256( mã đơn vị | thời điểm | số dùng một lần )   -> hex VIẾT HOA
 *          B = SHA256( bản tin đã rút gọn )                        -> hex VIẾT HOA
 *          C = A + "." + B
 *          chữ ký = RSA-SHA256(C) bằng khóa riêng của đơn vị        -> hex VIẾT HOA
 *   4. POST {địa chỉ nghiệp vụ}/hin-api-service/create-mau-phieu-m3
 *
 * LƯU Ý ĐỘ CHÍNH XÁC: giá trị B băm trên ĐÚNG chuỗi được gửi đi. Vì vậy chuỗi JSON được
 * dựng MỘT LẦN rồi dùng cho cả việc băm lẫn việc gửi — không dựng lại lần thứ hai, vì chỉ
 * lệch một dấu cách là chữ ký không khớp và cổng trả 400 toàn bộ hồ sơ.
 *
 * BẢO MẬT: KHÔNG ghi nhật ký mật khẩu, phiếu truy cập, khóa riêng, và KHÔNG ghi nội dung
 * bản tin (chứa thông tin bệnh nhân). Chỉ ghi mã đơn vị, thời điểm, số dùng một lần, giá trị
 * băm và mã trạng thái trả về — đủ để đối chiếu với Sở khi có sự cố.
 */
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Text;

namespace HIS.Desktop.Plugins.KskSyncList
{
    /// <summary>
    /// Thông tin kết nối cổng Sở Y tế TP.HCM, đọc từ bản ghi cấu hình
    /// `MOS.HIS_KSK_SYNC.SYT_HCM_CONNECTION_INFO`, các trường cách nhau bằng "|".
    /// </summary>
    internal class KskSytHcmConfig
    {
        public string BranchCode { get; set; }      // 1 - mã cơ sở khám chữa bệnh
        public string Username { get; set; }        // 2 - tài khoản
        public string Password { get; set; }        // 3 - mật khẩu
        public string AuthBaseUrl { get; set; }     // 4 - địa chỉ dịch vụ xác thực
        public string ApiBaseUrl { get; set; }      // 5 - địa chỉ dịch vụ nghiệp vụ
        public string ClientId { get; set; }        // 6 - mã đơn vị gọi (X-Client-Id)
        public string PrivateKeyPem { get; set; }   // 7 - khóa riêng để ký

        /// <summary>Đủ thông tin để LẤY PHIẾU TRUY CẬP (chưa cần khóa ký).</summary>
        public bool CanAuthenticate
        {
            get
            {
                return !string.IsNullOrWhiteSpace(Username)
                    && !string.IsNullOrWhiteSpace(Password)
                    && !string.IsNullOrWhiteSpace(AuthBaseUrl);
            }
        }

        /// <summary>Đủ thông tin để GỬI bản tin — phải có thêm mã đơn vị và khóa ký.</summary>
        public bool CanPush
        {
            get
            {
                return CanAuthenticate
                    && !string.IsNullOrWhiteSpace(ApiBaseUrl)
                    && !string.IsNullOrWhiteSpace(ClientId)
                    && !string.IsNullOrWhiteSpace(PrivateKeyPem);
            }
        }

        /// <summary>Nêu rõ còn thiếu trường nào — để người triển khai biết phải khai thêm gì.</summary>
        public string DescribeMissing()
        {
            System.Collections.Generic.List<string> m = new System.Collections.Generic.List<string>();
            if (string.IsNullOrWhiteSpace(Username)) m.Add("tai khoan (truong 2)");
            if (string.IsNullOrWhiteSpace(Password)) m.Add("mat khau (truong 3)");
            if (string.IsNullOrWhiteSpace(AuthBaseUrl)) m.Add("dia chi dich vu xac thuc (truong 4)");
            if (string.IsNullOrWhiteSpace(ApiBaseUrl)) m.Add("dia chi dich vu nghiep vu (truong 5)");
            if (string.IsNullOrWhiteSpace(ClientId)) m.Add("ma don vi goi (truong 6)");
            if (string.IsNullOrWhiteSpace(PrivateKeyPem)) m.Add("khoa rieng de ky (truong 7)");
            return string.Join(", ", m.ToArray());
        }

        internal static KskSytHcmConfig Parse(string raw)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(raw)) return null;
                string[] p = raw.Split('|');
                KskSytHcmConfig c = new KskSytHcmConfig();
                c.BranchCode = At(p, 0);
                c.Username = At(p, 1);
                c.Password = At(p, 2);
                c.AuthBaseUrl = TrimUrl(At(p, 3));
                c.ApiBaseUrl = TrimUrl(At(p, 4));
                c.ClientId = At(p, 5);
                c.PrivateKeyPem = At(p, 6);

                // Khai một địa chỉ thì dùng chung cho cả hai dịch vụ.
                if (string.IsNullOrWhiteSpace(c.ApiBaseUrl)) c.ApiBaseUrl = c.AuthBaseUrl;
                // Không khai mã đơn vị thì lấy theo mã cơ sở — đặc tả ví dụ là "CSKCB_79001".
                if (string.IsNullOrWhiteSpace(c.ClientId) && !string.IsNullOrWhiteSpace(c.BranchCode))
                    c.ClientId = "CSKCB_" + c.BranchCode;
                return c;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); return null; }
        }

        private static string At(string[] p, int i)
        {
            return (p != null && i < p.Length) ? p[i].Trim() : "";
        }

        private static string TrimUrl(string u)
        {
            return string.IsNullOrEmpty(u) ? u : u.TrimEnd('/');
        }
    }

    /// <summary>Kết quả một lượt gửi.</summary>
    internal class KskSytHcmPushResult
    {
        public bool Success { get; set; }
        /// <summary>Mã trạng thái nghiệp vụ do cổng trả (200 / 400 / 500...).</summary>
        public string Code { get; set; }
        public string Message { get; set; }
        /// <summary>Mã trạng thái HTTP, 0 nếu không gọi tới được.</summary>
        public int HttpStatus { get; set; }

        public override string ToString()
        {
            return "HTTP " + HttpStatus + " · ma " + (Code ?? "-") + " · " + (Message ?? "");
        }
    }

    internal static class KskSytHcmPusher
    {
        #region ===== Khai báo =====

        private const string SERVICE_CODE__M3 = "create-mau-phieu-m3";
        private const string URI__TOKEN = "/hin-auth/getToken";
        private const string URI__PUSH = "/hin-api-service/" + SERVICE_CODE__M3;

        private const int HTTP_TIMEOUT_MS = 60000;

        /// <summary>Phiếu truy cập dùng lại trong bộ nhớ. Hiệu lực 2 giờ, trừ biên 5 phút cho chắc.</summary>
        private static string accessToken;
        private static DateTime accessTokenExpire = DateTime.MinValue;
        private const int TOKEN_SAFETY_SECONDS = 300;

        /// <summary>
        /// Ghi nguyên văn bản tin ra tệp để đối chiếu khi cổng trả 400.
        /// MẶC ĐỊNH TẮT vì bản tin chứa thông tin bệnh nhân — chỉ bật tạm khi cần dò lỗi.
        /// </summary>
        private const bool DUMP_BODY_FOR_DEBUG = false;

        private static bool tlsEnabled = false;

        /// <summary>
        /// .NET Framework 4.5 mặc định chỉ bật SSL3 và TLS 1.0, máy chủ của Sở yêu cầu TLS 1.2.
        /// Dùng phép HỢP để THÊM, giữ nguyên giao thức các tích hợp khác đang dùng.
        /// </summary>
        private static void EnsureTls()
        {
            try
            {
                if (tlsEnabled) return;
                ServicePointManager.SecurityProtocol |=
                    SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11;
                tlsEnabled = true;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        #endregion

        #region ===== Lấy phiếu truy cập =====

        /// <summary>Lấy phiếu truy cập, dùng lại bản còn hiệu lực. Trả null nếu không lấy được.</summary>
        internal static string GetToken(KskSytHcmConfig cfg)
        {
            try
            {
                if (cfg == null || !cfg.CanAuthenticate) return null;
                if (!string.IsNullOrEmpty(accessToken) && DateTime.Now < accessTokenExpire)
                    return accessToken;

                EnsureTls();
                string body = JsonConvert.SerializeObject(
                    new { username = cfg.Username, password = cfg.Password }, Formatting.None);

                int status;
                string res = HttpSend(cfg.AuthBaseUrl + URI__TOKEN, "POST", body, null, null, out status);
                if (string.IsNullOrEmpty(res)) return null;

                JObject o = JObject.Parse(res);
                JToken result = o["result"];
                string token = (result != null) ? (string)result["access_token"] : null;
                if (string.IsNullOrEmpty(token))
                {
                    // KHÔNG ghi nội dung trả về vì có thể chứa phiếu truy cập.
                    string msg = (result != null) ? (string)result["message"] : null;
                    Inventec.Common.Logging.LogSystem.Warn(
                        "SytHcm: KHONG lay duoc phieu truy cap — " + (msg ?? "dich vu khong tra ve phieu"));
                    return null;
                }

                int ttl = 7200;
                if (result["expired_time"] != null)
                {
                    int v;
                    if (int.TryParse(result["expired_time"].ToString(), out v) && v > 0) ttl = v;
                }
                accessToken = token;
                accessTokenExpire = DateTime.Now.AddSeconds(Math.Max(60, ttl - TOKEN_SAFETY_SECONDS));
                Inventec.Common.Logging.LogSystem.Info(
                    "SytHcm: da lay phieu truy cap, hieu luc " + ttl + " giay");
                return accessToken;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); return null; }
        }

        /// <summary>Bỏ phiếu truy cập đang giữ — dùng khi cổng trả 401 để lấy lại phiếu mới.</summary>
        internal static void ResetToken()
        {
            accessToken = null;
            accessTokenExpire = DateTime.MinValue;
        }

        #endregion

        #region ===== Gửi bản tin =====

        /// <summary>
        /// Gửi một bản tin. `body` là đối tượng sẽ được chuyển thành JSON (các khối tthc, tien_su,
        /// kham_the_luc, kham_lam_san, can_lam_san, ket_luan).
        /// </summary>
        internal static KskSytHcmPushResult Push(KskSytHcmConfig cfg, object body)
        {
            KskSytHcmPushResult r = new KskSytHcmPushResult();
            try
            {
                if (cfg == null || !cfg.CanPush)
                {
                    r.Message = "Cau hinh cong SYT thieu truong: "
                        + ((cfg != null) ? cfg.DescribeMissing() : "chua khai bao");
                    Inventec.Common.Logging.LogSystem.Warn("SytHcm: " + r.Message);
                    return r;
                }

                string token = GetToken(cfg);
                if (string.IsNullOrEmpty(token))
                {
                    r.Message = "Khong lay duoc phieu truy cap";
                    return r;
                }

                EnsureTls();

                // Dựng chuỗi JSON MỘT LẦN rồi dùng cho cả băm lẫn gửi.
                string json = JsonConvert.SerializeObject(body, Formatting.None);
                string timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'");
                string nonce = Guid.NewGuid().ToString("N").Substring(0, 12);

                string hashA = KskPemUtil.Sha256HexUpperForSyt(
                    cfg.ClientId + "|" + timestamp + "|" + nonce);
                string hashB = KskPemUtil.Sha256HexUpperForSyt(json);
                string signature = KskPemUtil.SignRsaSha256HexUpper(hashA + "." + hashB, cfg.PrivateKeyPem);
                if (string.IsNullOrEmpty(signature))
                {
                    r.Message = "Khong ky duoc ban tin — kiem tra khoa rieng trong cau hinh";
                    Inventec.Common.Logging.LogSystem.Warn("SytHcm: " + r.Message);
                    return r;
                }

                // Ghi lại đủ thứ để đối chiếu với Sở, TRỪ bản tin và khóa.
                Inventec.Common.Logging.LogSystem.Info(string.Format(
                    "SytHcm: gui ban tin M3 — X-Client-Id={0}; X-Timestamp={1}; X-Nonce={2}; "
                    + "A={3}; B={4}; do dai ban tin={5} byte",
                    cfg.ClientId, timestamp, nonce, hashA, hashB,
                    Encoding.UTF8.GetByteCount(json)));

                if (DUMP_BODY_FOR_DEBUG) DumpBody(json);

                var headers = new System.Collections.Generic.Dictionary<string, string>();
                headers["X-Client-Id"] = cfg.ClientId;
                headers["X-Timestamp"] = timestamp;
                headers["X-Nonce"] = nonce;
                headers["X-API-Signature"] = signature;

                int status;
                string res = HttpSend(cfg.ApiBaseUrl + URI__PUSH, "POST", json, token, headers, out status);
                r.HttpStatus = status;

                if (status == 401)
                {
                    // Phiếu truy cập hết hiệu lực giữa lượt gửi -> bỏ phiếu để lượt sau lấy lại.
                    ResetToken();
                    r.Message = "Xac thuc that bai (401) — da bo phieu truy cap, thu lai";
                    Inventec.Common.Logging.LogSystem.Warn("SytHcm: " + r.Message);
                    return r;
                }

                ParseResult(res, r);
                Inventec.Common.Logging.LogSystem.Info("SytHcm: ket qua gui — " + r.ToString());
                return r;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                r.Message = "Loi khi gui: " + ex.GetType().Name;
                return r;
            }
        }

        /// <summary>
        /// THỬ KẾT NỐI: gửi một bản tin RỖNG để kiểm tra riêng phần hạ tầng — phiếu truy cập,
        /// chữ ký, danh sách IP cho phép.
        ///
        /// Cách đọc kết quả:
        ///   - mã 400 "lỗi body" => hạ tầng ĐÃ THÔNG (cổng đã nhận và xác thực được bản tin,
        ///     chỉ thiếu dữ liệu nghiệp vụ). Đây là kết quả MONG ĐỢI của bước thử này.
        ///   - 401 => sai tài khoản hoặc phiếu truy cập.
        ///   - 403 => địa chỉ IP chưa được Sở cho phép, hoặc chữ ký không khớp khóa đã đăng ký.
        /// Bản tin rỗng nên KHÔNG chứa thông tin bệnh nhân nào.
        /// </summary>
        internal static KskSytHcmPushResult TestConnection(KskSytHcmConfig cfg)
        {
            return Push(cfg, new
            {
                tthc = new { },
                tien_su = new { },
                kham_the_luc = new { },
                kham_lam_san = new { },
                can_lam_san = new { },
                ket_luan = new { }
            });
        }

        /// <summary>
        /// Đọc kết quả. Đặc tả ghi tên trường không thống nhất giữa hai mục — mục 1.3 ghi
        /// `isSucceeded`, bảng mục 5 ghi `isSuccessed` — nên nhận cả hai cách viết.
        /// </summary>
        private static void ParseResult(string res, KskSytHcmPushResult r)
        {
            try
            {
                if (string.IsNullOrEmpty(res)) { r.Message = "Khong nhan duoc tra loi"; return; }

                JObject o = JObject.Parse(res);
                JToken result = o["result"];
                if (result == null) { r.Message = "Tra loi khong dung cau truc"; return; }

                r.Code = (result["code"] != null) ? result["code"].ToString() : null;
                r.Message = (result["message"] != null) ? result["message"].ToString() : null;

                // Thông điệp của cổng rất chung chung ("Lỗi trả ra từ hệ thống..."), chi tiết trường
                // nào sai nằm ở phần `data`. Ghi lại để không phải đoán. Cắt bớt cho khỏi tràn nhật ký.
                JToken data = result["data"];
                if (data != null && data.Type != JTokenType.Null)
                {
                    string detail = data.ToString(Newtonsoft.Json.Formatting.None);
                    if (!string.IsNullOrEmpty(detail) && detail != "[]" && detail != "{}")
                    {
                        if (detail.Length > 4000) detail = detail.Substring(0, 4000) + "...(cat bot)";
                        Inventec.Common.Logging.LogSystem.Warn("SytHcm: chi tiet loi tu cong -> " + detail);
                        r.Message = (r.Message ?? "") + " | " + detail;
                    }
                }

                JToken ok = result["isSucceeded"] ?? result["isSuccessed"];
                bool flag = (ok != null) && ok.Type == JTokenType.Boolean && (bool)ok;
                r.Success = flag && (r.Code == null || r.Code == "200");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                r.Message = "Khong doc duoc tra loi";
            }
        }

        #endregion

        #region ===== Gọi dịch vụ =====

        private static string HttpSend(string url, string method, string body, string bearerToken,
            System.Collections.Generic.Dictionary<string, string> headers, out int status)
        {
            status = 0;
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                req.Method = method;
                req.ContentType = "application/json";
                req.Accept = "application/json";
                req.Timeout = HTTP_TIMEOUT_MS;
                req.ReadWriteTimeout = HTTP_TIMEOUT_MS;
                if (!string.IsNullOrEmpty(bearerToken))
                    req.Headers["Authorization"] = "Bearer " + bearerToken;
                if (headers != null)
                {
                    foreach (var h in headers) req.Headers[h.Key] = h.Value;
                }

                if (!string.IsNullOrEmpty(body))
                {
                    byte[] data = Encoding.UTF8.GetBytes(body);
                    req.ContentLength = data.Length;
                    using (Stream s = req.GetRequestStream()) { s.Write(data, 0, data.Length); }
                }

                using (HttpWebResponse res = (HttpWebResponse)req.GetResponse())
                {
                    status = (int)res.StatusCode;
                    using (StreamReader sr = new StreamReader(res.GetResponseStream(), Encoding.UTF8))
                        return sr.ReadToEnd();
                }
            }
            catch (WebException wex)
            {
                // Cổng trả 400/403 KÈM nội dung giải thích -> phải đọc nội dung đó, không bỏ đi.
                HttpWebResponse res = wex.Response as HttpWebResponse;
                if (res != null)
                {
                    status = (int)res.StatusCode;
                    try
                    {
                        using (StreamReader sr = new StreamReader(res.GetResponseStream(), Encoding.UTF8))
                            return sr.ReadToEnd();
                    }
                    catch (Exception exRead) { Inventec.Common.Logging.LogSystem.Warn(exRead); }
                    return null;
                }

                Inventec.Common.Logging.LogSystem.Warn(
                    "SytHcm: goi dich vu that bai — " + DescribeWebError(wex));
                return null;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); return null; }
        }

        /// <summary>Mô tả lỗi mạng cho dễ chẩn đoán. KHÔNG ghi nội dung bản tin.</summary>
        internal static string DescribeWebError(WebException wex)
        {
            try
            {
                if (wex.Status == WebExceptionStatus.TrustFailure)
                    return "chung thu so cua may chu khong duoc tin cay (TrustFailure)";
                if (wex.Status == WebExceptionStatus.SecureChannelFailure)
                    return "bat tay TLS that bai (SecureChannelFailure)";
                if (wex.Status == WebExceptionStatus.NameResolutionFailure)
                    return "khong phan giai duoc ten mien — kiem tra dia chi trong cau hinh";
                if (wex.Status == WebExceptionStatus.Timeout)
                    return "qua han cho (Timeout)";
                return "loi mang (" + wex.Status + ")";
            }
            catch { return "loi mang"; }
        }

        private static void DumpBody(string json)
        {
            try
            {
                string dir = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    Path.Combine("HIS", "KskSytHcm"));
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                File.WriteAllText(Path.Combine(dir, "BanTin_M3.json"), json, Encoding.UTF8);
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        #endregion
    }
}
