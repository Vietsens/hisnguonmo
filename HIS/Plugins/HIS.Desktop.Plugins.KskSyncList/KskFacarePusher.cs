/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 */
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json.Linq;

namespace HIS.Desktop.Plugins.KskSyncList
{
    /// <summary>
    /// Cấu hình cổng CSDL khám sức khỏe Sở Y tế Cần Thơ (nền tảng FaCare-AIoT, tài liệu FAPI_MED_SYNC 1.0.3),
    /// khóa <c>MOS.HIS_KSK_SYNC.FACARE_CONNECTION_INFO</c> (đọc theo chi nhánh). Viện không khai khóa này thì
    /// cổng không hiện, không đẩy. Định dạng (các trường cách '|'):
    /// <code>
    /// APIUsername|APIPassword|HmacSecretKey[|BaseUrl]
    /// </code>
    /// BaseUrl bỏ trống = môi trường chính thức https://api-csdlksk.soytecantho.vn/admin-api.
    /// </summary>
    internal class KskFacareConfig
    {
        internal string Username { get; set; }
        internal string Password { get; set; }
        internal string SecretKey { get; set; }
        internal string BaseUrl { get; set; }
    }

    internal static class KskFacareConfigParser
    {
        internal const string CONFIG_KEY = "MOS.HIS_KSK_SYNC.FACARE_CONNECTION_INFO";
        internal const string DEFAULT_BASE_URL = "https://api-csdlksk.soytecantho.vn/admin-api";

        /// <summary>
        /// Tách chuỗi cấu hình. Chuỗi rỗng -> null (chưa cấu hình). Thiếu trường bắt buộc VẪN trả đối tượng:
        /// KskFacarePusher.ValidateConfig nói rõ trường nào thiếu cho từng hồ sơ.
        /// </summary>
        internal static KskFacareConfig Parse(string configValue)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(configValue)) return null;
                string[] f = configValue.Split('|');
                KskFacareConfig cfg = new KskFacareConfig();
                cfg.Username = Get(f, 0);
                cfg.Password = Get(f, 1);
                cfg.SecretKey = Get(f, 2);
                string baseUrl = Get(f, 3);
                cfg.BaseUrl = (string.IsNullOrWhiteSpace(baseUrl) ? DEFAULT_BASE_URL : baseUrl).TrimEnd('/');
                return cfg;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }

        private static string Get(string[] arr, int index)
        {
            return (arr != null && index < arr.Length && arr[index] != null) ? arr[index].Trim() : null;
        }
    }

    /// <summary>Kết quả cổng FaCare của 1 hồ sơ (chuẩn hóa để gộp với kết quả các cổng khác).</summary>
    internal class KskFacarePushResult
    {
        // Trạng thái lưu vào REGISTRATION_NO (đoạn "FACARE:").
        internal const string STATE_LINKED = "DA_LIEN_THONG";        // cổng đã nhận hồ sơ, đã ghi nhận hồ sơ lên cổng Bộ
        internal const string STATE_WAIT_LINK = "CHO_LIEN_THONG";    // cổng đã nhận hồ sơ, chưa ghi nhận lên cổng Bộ
        internal const string STATE_REJECTED = "KHONG_TIEP_NHAN";    // cổng trả lỗi, hồ sơ chưa có trên cổng
        internal const string STATE_NOT_SENT = "CHUA_GUI";           // bị chặn trước khi gửi
        internal const string STATE_UNKNOWN = "CHUA_RO";             // đã gửi nhưng không nhận được kết quả

        internal bool Success { get; set; }
        /// <summary>Lý do thất bại (ghi vào SYNC_FAILD_REASON).</summary>
        internal string Message { get; set; }
        /// <summary>Ghi chú khi thành công (cửa sổ kết quả).</summary>
        internal string Note { get; set; }
        internal string State { get; set; }
        /// <summary>X-Request-Id của lần GỬI HỒ SƠ này (null = lượt này không gửi hồ sơ, giữ mã lần trước).</summary>
        internal string RequestId { get; set; }

        internal static KskFacarePushResult NotSent(string message)
        {
            return new KskFacarePushResult { Success = false, Message = message, State = STATE_NOT_SENT };
        }

        /// <summary>Trạng thái cho biết hồ sơ ĐÃ có trên cổng FaCare (không được gửi lại).</summary>
        internal static bool IsOnPortal(string state)
        {
            return string.Equals(state, STATE_LINKED, StringComparison.OrdinalIgnoreCase)
                || string.Equals(state, STATE_WAIT_LINK, StringComparison.OrdinalIgnoreCase);
        }
    }

    /// <summary>
    /// Gọi cổng FaCare theo tài liệu FAPI_MED_SYNC 1.0.3 và trả lời của FaCare ngày 10/11/2026:
    ///   1. POST {BaseUrl}/api/token/api-token {username, password} -> Data.access_token, api_account_id, expires.
    ///      Thành công CHỈ khi Data.access_token khác rỗng. Không có refresh token.
    ///   2. POST {BaseUrl}/api/medical-data-sync/import (1 hồ sơ / lần) và
    ///      POST {BaseUrl}/api/medical-data-sync/import/update-status ({ma_lk}).
    ///      Header: Authorization Bearer, X-FC-Timestamp (giây Unix UTC), X-FC-Nonce, X-Request-Id,
    ///      X-FC-Signature = Base64(HMAC-SHA256(UTF8(secret_key), UTF8(chuỗi ký 8 dòng nối LF))):
    ///      v1 / api_account_id / POST / đường dẫn / timestamp / nonce / request id / sha256 hex chữ thường của byte thân.
    ///      Đường dẫn ký = toàn bộ phần đường dẫn của URL gọi, GỒM phần /admin-api của Base URL (FaCare xác nhận).
    ///   3. KHÔNG tự gửi lại import khi hết thời gian chờ / 500 / 503 (dữ liệu có thể đã ghi). Chỉ gửi lại 1 lần
    ///      sau 401 Token-Expired với token mới, nonce/timestamp mới, giữ nguyên byte thân và X-Request-Id.
    /// Code mẫu của tài liệu dùng hàm .NET 5+; ở đây viết lại cho .NET Framework 4.5 (HttpWebRequest).
    /// </summary>
    internal class KskFacarePusher
    {
        internal const string PATH_TOKEN = "/api/token/api-token";
        internal const string PATH_IMPORT = "/api/medical-data-sync/import";
        internal const string PATH_UPDATE_STATUS = "/api/medical-data-sync/import/update-status";
        private const int HTTP_TIMEOUT_MS = 120000;
        private const int TOKEN_EXPIRE_SAFETY_SECONDS = 60;
        private const int TOKEN_DEFAULT_LIFETIME_MINUTES = 25;   // phản hồi thiếu "expires"
        private const string LOG_PREFIX = "FACARE: ";

        private readonly KskFacareConfig config;
        private bool configChecked;
        private string configError;
        // Đăng nhập thất bại -> dừng cổng FaCare cho cả lượt (không gọi mạng cho các hồ sơ còn lại).
        private string batchAuthFatalError;
        private string accessToken;
        private string apiAccountId;
        private DateTime tokenExpireAt;

        /// <summary>Plugin .NET 4.5 không bật TLS 1.2 mặc định.</summary>
        static KskFacarePusher()
        {
            try
            {
                System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        internal KskFacarePusher(KskFacareConfig config)
        {
            this.config = config;
        }

        internal KskFacareConfig Config { get { return this.config; } }

        /// <summary>Kiểm cấu hình 1 lần / lượt. Hợp lệ -> null, ngược lại câu báo nêu rõ trường thiếu.</summary>
        internal string ValidateConfig()
        {
            if (this.configChecked) return this.configError;
            this.configChecked = true;
            if (this.config == null)
            {
                this.configError = "FACARE: Cấu hình " + KskFacareConfigParser.CONFIG_KEY + " sai định dạng.";
            }
            else
            {
                var missing = new List<string>();
                if (string.IsNullOrWhiteSpace(this.config.Username)) missing.Add("API username");
                if (string.IsNullOrWhiteSpace(this.config.Password)) missing.Add("API password");
                if (string.IsNullOrWhiteSpace(this.config.SecretKey)) missing.Add("HMAC secret_key");
                if (missing.Count > 0)
                    this.configError = "FACARE: Cấu hình " + KskFacareConfigParser.CONFIG_KEY + " thiếu "
                        + string.Join(", ", missing.ToArray()) + ".";
            }
            if (this.configError != null) Inventec.Common.Logging.LogSystem.Warn(this.configError);
            return this.configError;
        }

        #region gửi hồ sơ
        /// <summary>
        /// Gửi 1 hồ sơ. body = thân JSON đã chuyển sang byte UTF-8 đúng 1 lần (ký và gửi chính mảng byte này).
        /// linked = giá trị visit.profile_sync_status đã gửi -> quyết định trạng thái khi cổng nhận.
        /// </summary>
        internal KskFacarePushResult Import(byte[] body, string requestId, bool linked)
        {
            try
            {
                string cfgError = ValidateConfig();
                if (cfgError != null) return KskFacarePushResult.NotSent(cfgError);
                if (body == null || body.Length == 0) return KskFacarePushResult.NotSent("FACARE: Không dựng được bản tin hồ sơ.");

                string authError;
                if (!EnsureToken(false, out authError)) return KskFacarePushResult.NotSent(authError);

                HttpResult http = SendSigned(PATH_IMPORT, body, requestId);
                if (http.TokenExpired)
                {
                    Inventec.Common.Logging.LogSystem.Warn(LOG_PREFIX + "token het han (401 Token-Expired) -> lay token moi, gui lai 1 lan; X-Request-Id=" + requestId);
                    if (!EnsureToken(true, out authError))
                    {
                        // Lần gửi đầu đã bị cổng từ chối vì token, chưa ghi dữ liệu -> chưa gửi.
                        return KskFacarePushResult.NotSent(authError);
                    }
                    http = SendSigned(PATH_IMPORT, body, requestId);
                }
                return BuildImportResult(http, requestId, linked);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return KskFacarePushResult.NotSent("FACARE: " + ex.Message);
            }
        }

        private static KskFacarePushResult BuildImportResult(HttpResult http, string requestId, bool linked)
        {
            var r = new KskFacarePushResult { RequestId = requestId };
            string traceText = string.IsNullOrEmpty(http.TraceId) ? "trống" : http.TraceId;
            if (http.Status == 0)
            {
                if (!http.Sent)
                {
                    // Chưa ghi được byte nào lên mạng -> chắc chắn chưa gửi, giữ mã yêu cầu lần trước.
                    r.RequestId = null;
                    r.State = KskFacarePushResult.STATE_NOT_SENT;
                    r.Message = "FACARE: Không kết nối được cổng FaCare (kiểm tra mạng / Base URL trong khóa "
                        + KskFacareConfigParser.CONFIG_KEY + ").";
                    return r;
                }
                r.State = KskFacarePushResult.STATE_UNKNOWN;
                r.Message = UnknownMessage(requestId, traceText);
                return r;
            }

            string messageCode = ReadString(http.Json, "MessageCode");
            JObject data = (http.Json != null) ? http.Json.GetValue("Data", StringComparison.OrdinalIgnoreCase) as JObject : null;
            int successCount = ReadInt(data, "success_count");
            int errorCount = ReadInt(data, "error_count");

            if (http.Status >= 200 && http.Status < 300)
            {
                if (http.Json == null)
                {
                    r.State = KskFacarePushResult.STATE_UNKNOWN;
                    r.Message = UnknownMessage(requestId, traceText);
                    return r;
                }
                if (successCount == 1 && errorCount == 0)
                {
                    r.Success = true;
                    r.State = linked ? KskFacarePushResult.STATE_LINKED : KskFacarePushResult.STATE_WAIT_LINK;
                    r.Note = linked
                        ? "FACARE: Cổng đã tiếp nhận (mã yêu cầu " + requestId + ")"
                        : "FACARE: Cổng đã tiếp nhận, chưa ghi nhận liên thông Bộ (mã yêu cầu " + requestId + ")";
                    return r;
                }
                r.State = KskFacarePushResult.STATE_REJECTED;
                r.Message = "FACARE: Cổng không tiếp nhận hồ sơ — " + RejectDetail(http.Json, data, messageCode);
                return r;
            }

            switch (http.Status)
            {
                case 400:
                    r.State = KskFacarePushResult.STATE_REJECTED;
                    r.Message = "FACARE: Cổng không tiếp nhận hồ sơ — " + RejectDetail(http.Json, data, messageCode);
                    return r;
                case 401:
                    r.State = KskFacarePushResult.STATE_REJECTED;
                    if (string.Equals(messageCode, "SYNC_TIMESTAMP_OUT_OF_RANGE", StringComparison.OrdinalIgnoreCase))
                        r.Message = "FACARE: Đồng hồ máy trạm lệch quá 5 phút so với cổng. Chỉnh lại giờ máy rồi đồng bộ lại.";
                    else
                        r.Message = "FACARE: " + (messageCode ?? "HTTP 401") + DescribeSecurityCode(messageCode);
                    return r;
                case 413:
                    r.State = KskFacarePushResult.STATE_REJECTED;
                    r.Message = "FACARE: Hồ sơ vượt dung lượng 5 MB cổng cho phép.";
                    return r;
            }
            if (http.Status >= 500)
            {
                r.State = KskFacarePushResult.STATE_UNKNOWN;
                r.Message = UnknownMessage(requestId, traceText);
                return r;
            }
            // 403, 404, 409, 415, 429... -> cổng từ chối, dữ liệu chưa ghi.
            r.State = KskFacarePushResult.STATE_REJECTED;
            r.Message = "FACARE: " + (messageCode ?? ("HTTP " + http.Status)) + DescribeSecurityCode(messageCode);
            return r;
        }

        private static string UnknownMessage(string requestId, string traceText)
        {
            return "FACARE: Đã gửi nhưng không nhận được kết quả (mã yêu cầu " + requestId + ", mã truy vết " + traceText
                + "). Nhờ nhóm hỗ trợ API FaCare kiểm tra trước khi đồng bộ lại để tránh trùng lượt khám.";
        }

        /// <summary>Lý do từ chối: Data.errors[0].error -> MessageCode -> ExtraInfo.</summary>
        private static string RejectDetail(JObject json, JObject data, string messageCode)
        {
            JArray errors = (data != null) ? data.GetValue("errors", StringComparison.OrdinalIgnoreCase) as JArray : null;
            if (errors != null && errors.Count > 0)
            {
                JObject first = errors[0] as JObject;
                string err = (first != null) ? ReadString(first, "error") : errors[0].ToString();
                if (!string.IsNullOrWhiteSpace(err)) return err.Trim();
            }
            if (!string.IsNullOrWhiteSpace(messageCode)) return messageCode;
            string extra = ReadString(json, "ExtraInfo");
            return string.IsNullOrWhiteSpace(extra) ? "không rõ lý do" : extra;
        }

        /// <summary>Gợi ý cho mã lỗi bảo mật hay gặp (tài liệu mục 10.1).</summary>
        private static string DescribeSecurityCode(string messageCode)
        {
            if (string.IsNullOrEmpty(messageCode)) return "";
            if (messageCode.IndexOf("SIGNATURE", StringComparison.OrdinalIgnoreCase) >= 0)
                return " (chữ ký HMAC không khớp — kiểm tra HMAC secret_key trong khóa " + KskFacareConfigParser.CONFIG_KEY + ")";
            if (messageCode.IndexOf("NONCE", StringComparison.OrdinalIgnoreCase) >= 0)
                return " (trùng hoặc sai mã nonce)";
            if (messageCode.IndexOf("AUTHENTICATION", StringComparison.OrdinalIgnoreCase) >= 0)
                return " (xác thực không hợp lệ — kiểm tra tài khoản API)";
            return "";
        }
        #endregion

        #region xác nhận liên thông
        /// <summary>
        /// Báo FaCare hồ sơ đã lên cổng Bộ (API xác nhận liên thông). Gửi ma_lk, không gửi patient_visit_id.
        /// Thành công: UPDATE_MEDICAL_DATA_SUCCESS hoặc ALREADY_SYNCED. Thất bại giữ CHO_LIEN_THONG để lượt sau gọi lại.
        /// </summary>
        internal KskFacarePushResult UpdateStatus(string maLk, string requestId)
        {
            var r = new KskFacarePushResult { State = KskFacarePushResult.STATE_WAIT_LINK };
            try
            {
                string cfgError = ValidateConfig();
                if (cfgError != null) { r.Message = cfgError; return r; }
                if (string.IsNullOrWhiteSpace(maLk))
                {
                    r.Message = "FACARE: Xác nhận liên thông thất bại (hồ sơ không có mã liên kết).";
                    return r;
                }

                string authError;
                if (!EnsureToken(false, out authError)) { r.Message = authError; return r; }

                var bodyObj = new JObject();
                bodyObj["ma_lk"] = maLk.Trim();
                byte[] body = new UTF8Encoding(false).GetBytes(bodyObj.ToString(Newtonsoft.Json.Formatting.None));

                HttpResult http = SendSigned(PATH_UPDATE_STATUS, body, requestId);
                if (http.TokenExpired)
                {
                    if (!EnsureToken(true, out authError)) { r.Message = authError; return r; }
                    http = SendSigned(PATH_UPDATE_STATUS, body, requestId);
                }

                string messageCode = ReadString(http.Json, "MessageCode");
                JObject data = (http.Json != null) ? http.Json.GetValue("Data", StringComparison.OrdinalIgnoreCase) as JObject : null;
                string status = ReadString(data, "status");
                bool ok = http.Status >= 200 && http.Status < 300
                    && (IsUpdateOk(messageCode) || IsUpdateOk(status));
                if (ok)
                {
                    r.Success = true;
                    r.State = KskFacarePushResult.STATE_LINKED;
                    r.Note = "FACARE: Đã xác nhận liên thông Bộ.";
                    return r;
                }
                string code = messageCode ?? status;
                if (string.Equals(code, "AMBIGUOUS_MA_LK", StringComparison.OrdinalIgnoreCase))
                {
                    r.Message = "FACARE: Cổng có nhiều lượt khám cùng mã liên kết " + maLk.Trim()
                        + ". Cập nhật trạng thái liên thông trực tiếp trên web FaCare.";
                    return r;
                }
                string detail = !string.IsNullOrWhiteSpace(code) ? code
                    : (http.Status == 0 ? "không nhận được phản hồi của cổng" : "HTTP " + http.Status);
                r.Message = "FACARE: Xác nhận liên thông thất bại (" + detail + ").";
                return r;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                r.Message = "FACARE: Xác nhận liên thông thất bại (" + ex.Message + ").";
                return r;
            }
        }

        private static bool IsUpdateOk(string code)
        {
            return string.Equals(code, "UPDATE_MEDICAL_DATA_SUCCESS", StringComparison.OrdinalIgnoreCase)
                || string.Equals(code, "ALREADY_SYNCED", StringComparison.OrdinalIgnoreCase);
        }
        #endregion

        #region token
        /// <summary>
        /// Lấy token dùng cho cả lượt bấm Đồng bộ. forceNew = true khi cổng báo Token-Expired.
        /// Thất bại -> dừng cổng FaCare cho cả lượt (các hồ sơ còn lại báo luôn, không gọi mạng).
        /// </summary>
        private bool EnsureToken(bool forceNew, out string error)
        {
            error = null;
            if (this.batchAuthFatalError != null) { error = this.batchAuthFatalError; return false; }
            if (!forceNew && !string.IsNullOrEmpty(this.accessToken) && DateTime.Now < this.tokenExpireAt) return true;

            this.accessToken = null;
            var login = new JObject();
            login["username"] = this.config.Username;
            login["password"] = this.config.Password;
            byte[] body = new UTF8Encoding(false).GetBytes(login.ToString(Newtonsoft.Json.Formatting.None));
            HttpResult http = Post(this.config.BaseUrl + PATH_TOKEN, body, null);

            JObject data = (http.Json != null) ? http.Json.GetValue("Data", StringComparison.OrdinalIgnoreCase) as JObject : null;
            string token = ReadString(data, "access_token");
            string accountId = ReadString(data, "api_account_id");
            string messageCode = ReadString(http.Json, "MessageCode");
            Inventec.Common.Logging.LogSystem.Info(string.Format(LOG_PREFIX + "lay token {0}{1} -> HTTP {2}; MessageCode={3}; co access_token={4}; X-Trace-Id={5}",
                this.config.BaseUrl, PATH_TOKEN, http.Status, messageCode ?? "", !string.IsNullOrWhiteSpace(token), http.TraceId ?? ""));

            if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(accountId))
            {
                string reason = !string.IsNullOrWhiteSpace(messageCode) ? messageCode
                    : (http.Status == 0 ? "không kết nối được cổng — kiểm tra mạng / Base URL" : "HTTP " + http.Status);
                this.batchAuthFatalError = "FACARE: Đăng nhập cổng thất bại (" + reason + ").";
                error = this.batchAuthFatalError;
                return false;
            }

            this.accessToken = token.Trim();
            Guid g;
            this.apiAccountId = Guid.TryParse(accountId.Trim(), out g) ? g.ToString("D") : accountId.Trim();
            this.tokenExpireAt = DateTime.Now.AddMinutes(TOKEN_DEFAULT_LIFETIME_MINUTES);
            string expires = ReadString(data, "expires");
            DateTimeOffset exp;
            if (!string.IsNullOrWhiteSpace(expires)
                && DateTimeOffset.TryParse(expires, CultureInfo.InvariantCulture, DateTimeStyles.None, out exp))
                this.tokenExpireAt = exp.LocalDateTime.AddSeconds(-TOKEN_EXPIRE_SAFETY_SECONDS);
            return true;
        }
        #endregion

        #region ký + http
        private sealed class HttpResult
        {
            internal int Status;
            internal bool Sent;
            internal string Body;
            internal JObject Json;
            internal string TraceId;
            internal bool TokenExpired;
        }

        /// <summary>Gửi 1 lần (không tự gửi lại): ký HMAC trên đúng mảng byte thân, đường dẫn ký lấy từ URL gọi.</summary>
        private HttpResult SendSigned(string path, byte[] body, string requestId)
        {
            string url = this.config.BaseUrl + path;
            string signPath = new Uri(url).AbsolutePath;
            string timestamp = ((long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds)
                .ToString(CultureInfo.InvariantCulture);
            string nonce = Guid.NewGuid().ToString("N");
            string payloadHash = Sha256Hex(body);
            string canonical = string.Join("\n", new[] { "v1", this.apiAccountId, "POST", signPath, timestamp, nonce, requestId ?? "", payloadHash });
            string signature;
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(this.config.SecretKey)))
                signature = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(canonical)));

            var headers = new Dictionary<string, string>();
            headers["X-FC-Timestamp"] = timestamp;
            headers["X-FC-Nonce"] = nonce;
            headers["X-FC-Signature"] = signature;
            if (!string.IsNullOrEmpty(requestId)) headers["X-Request-Id"] = requestId;

            DateTime sentAt = DateTime.Now;
            HttpResult http = Post(url, body, headers);
            JObject data = (http.Json != null) ? http.Json.GetValue("Data", StringComparison.OrdinalIgnoreCase) as JObject : null;
            // Không ghi mật khẩu, khóa bí mật, token, chữ ký.
            Inventec.Common.Logging.LogSystem.Info(string.Format(
                LOG_PREFIX + "goi {0} -> HTTP {1}; MessageCode={2}; X-Request-Id={3}; X-Trace-Id={4}; gui {5:yyyy-MM-dd HH:mm:ss}, nhan {6:yyyy-MM-dd HH:mm:ss};"
                + " success_count={7}; error_count={8}; sha256 than gui={9}; {10} byte; Token-Expired={11}; phan hoi: {12}",
                signPath, http.Status, ReadString(http.Json, "MessageCode") ?? "", requestId ?? "", http.TraceId ?? "", sentAt, DateTime.Now,
                ReadString(data, "success_count") ?? "", ReadString(data, "error_count") ?? "", payloadHash, body.Length,
                http.TokenExpired, Cut(http.Body, 500)));
            return http;
        }

        /// <summary>
        /// POST JSON (HttpWebRequest — .NET 4.5) với mảng byte có sẵn. Status = 0 khi không có phản hồi HTTP;
        /// Sent = đã ghi xong thân lên mạng (phân biệt "chưa gửi" với "mất phản hồi").
        /// </summary>
        private HttpResult Post(string url, byte[] body, Dictionary<string, string> extraHeaders)
        {
            var result = new HttpResult();
            var request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/json; charset=utf-8";
            request.Accept = "application/json";
            request.Timeout = HTTP_TIMEOUT_MS;
            request.ReadWriteTimeout = HTTP_TIMEOUT_MS;
            if (!string.IsNullOrEmpty(this.accessToken) && extraHeaders != null)
                request.Headers.Add("Authorization", "Bearer " + this.accessToken);
            if (extraHeaders != null)
                foreach (var kv in extraHeaders) request.Headers.Add(kv.Key, kv.Value);
            request.ContentLength = (body != null) ? body.Length : 0;
            try
            {
                if (body != null && body.Length > 0)
                    using (var stream = request.GetRequestStream())
                        stream.Write(body, 0, body.Length);
                result.Sent = true;
                using (var response = (System.Net.HttpWebResponse)request.GetResponse())
                    Fill(result, response);
            }
            catch (System.Net.WebException wex)
            {
                var errResponse = wex.Response as System.Net.HttpWebResponse;
                if (errResponse == null)
                {
                    Inventec.Common.Logging.LogSystem.Error(LOG_PREFIX + "khong nhan duoc phan hoi tu " + url + " — " + wex.Status + ": " + wex.Message, wex);
                    return result;
                }
                using (errResponse) Fill(result, errResponse);
            }
            return result;
        }

        private static void Fill(HttpResult result, System.Net.HttpWebResponse response)
        {
            result.Status = (int)response.StatusCode;
            result.TraceId = response.Headers["X-Trace-Id"];
            result.TokenExpired = result.Status == 401
                && string.Equals((response.Headers["Token-Expired"] ?? "").Trim(), "true", StringComparison.OrdinalIgnoreCase);
            using (var stream = response.GetResponseStream())
            {
                if (stream != null)
                    using (var reader = new System.IO.StreamReader(stream, Encoding.UTF8))
                        result.Body = reader.ReadToEnd();
            }
            result.Json = TryParseJObject(result.Body);
        }

        internal static string Sha256Hex(byte[] data)
        {
            using (var sha = SHA256.Create())
            {
                byte[] hash = sha.ComputeHash(data ?? new byte[0]);
                var sb = new StringBuilder(hash.Length * 2);
                foreach (byte b in hash) sb.Append(b.ToString("x2", CultureInfo.InvariantCulture));
                return sb.ToString();
            }
        }
        #endregion

        #region json
        private static JObject TryParseJObject(string body)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(body)) return null;
                string t = body.TrimStart();
                if (!t.StartsWith("{")) return null;
                return JObject.Parse(t);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(LOG_PREFIX + "phan hoi khong phai JSON hop le: " + Cut(body, 300) + " — " + ex.Message);
                return null;
            }
        }

        private static string ReadString(JObject jo, string name)
        {
            if (jo == null) return null;
            JToken v = jo.GetValue(name, StringComparison.OrdinalIgnoreCase);
            if (v == null || v.Type == JTokenType.Null) return null;
            string s = v.ToString();
            return string.IsNullOrWhiteSpace(s) ? null : s.Trim();
        }

        private static int ReadInt(JObject jo, string name)
        {
            int n;
            string s = ReadString(jo, name);
            return (s != null && int.TryParse(s, out n)) ? n : -1;
        }

        private static string Cut(string s, int max)
        {
            if (string.IsNullOrEmpty(s)) return s;
            return (s.Length <= max) ? s : s.Substring(0, max) + "…";
        }
        #endregion
    }
}
