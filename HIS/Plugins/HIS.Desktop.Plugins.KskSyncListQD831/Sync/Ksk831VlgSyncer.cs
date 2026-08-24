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
using System.Net;
using System.Text;
using HIS.Desktop.Plugins.KskSyncListQD831.Xml831;
using Newtonsoft.Json.Linq;

namespace HIS.Desktop.Plugins.KskSyncListQD831.Sync
{
    /// <summary>
    /// Đồng bộ hồ sơ QĐ831 lên CỔNG TIẾP NHẬN — Kho dữ liệu y tế tỉnh Vĩnh Long
    /// (POST /api/ho-so-suc-khoe/qd-831-2017/tiep-nhan — body XML trực tiếp, Bearer token).
    /// Dùng chung khóa cấu hình với liên thông KSK VLG:
    /// <c>MOS.HIS_KSK_SYNC.VLG_2062_CONNECTION_INFO</c> = MaDonVi|Username|Password|TokenUrl|PushUrl
    /// (2 URL bỏ trống = cổng chính thức; PushUrl của khóa là API KSK nên KHÔNG dùng ở đây —
    /// đường dẫn HSSK cố định theo tài liệu, chỉ lấy BaseUrl suy từ TokenUrl).
    /// Trước khi gửi: HEADER/SENDER_CODE bắt buộc = MaDonVi (khớp token, sai bị 403 ORG_MISMATCH);
    /// HEADER/REQUEST_ID = mã hồ sơ + hash nội dung (idempotency: gửi lại y nguyên -&gt;
    /// ACCEPTED_DUPLICATE giữ tracking cũ; đổi nội dung -&gt; mã mới, tránh 409 REQUEST_ID_CONFLICT).
    /// </summary>
    internal class Ksk831VlgSyncer
    {
        private const string DEFAULT_BASE_URL = "https://congtiepnhan.kdlyt.vinhlong.vn";
        private const string TOKEN_PATH = "/api/xac-thuc/token";
        private const string PUSH_PATH = "/api/ho-so-suc-khoe/qd-831-2017/tiep-nhan";
        private const long TOKEN_TTL_DEFAULT_SECONDS = 10800;
        private const long TOKEN_TTL_SAFETY_SECONDS = 60;
        private const int HTTP_TIMEOUT_MS = 60000;
        private const int MAX_BODY_BYTES = 10 * 1024 * 1024;   // tai lieu: body toi da 10 MiB

        private readonly string maDonVi;
        private readonly string username;
        private readonly string password;
        private readonly string baseUrl;
        private string cachedToken;
        private DateTime tokenExpireAt = DateTime.MinValue;
        private string lastAuthError;
        // Loi khong tu het giua lo (mat mang, sai tai khoan, rate-limit) -> cac ho so sau tra loi ngay.
        private string batchFatalError;

        static Ksk831VlgSyncer()
        {
            try { ServicePointManager.SecurityProtocol |= (SecurityProtocolType)3072; }   // TLS 1.2
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        private Ksk831VlgSyncer(string maDonVi, string username, string password, string baseUrl)
        {
            this.maDonVi = maDonVi;
            this.username = username;
            this.password = password;
            this.baseUrl = baseUrl;
        }

        internal string MaDonVi { get { return this.maDonVi; } }

        /// <summary>Parse khóa cấu hình VLG. Thiếu MaDonVi/tài khoản/mật khẩu -&gt; null (chưa cấu hình).</summary>
        internal static Ksk831VlgSyncer Create(string connectionInfo)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(connectionInfo)) return null;
                string[] f = connectionInfo.Split('|');
                string maDonVi = Get(f, 0), username = Get(f, 1), password = Get(f, 2), tokenUrl = Get(f, 3);
                if (string.IsNullOrEmpty(maDonVi) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                    return null;
                // BaseUrl suy tu TokenUrl (bo duoi /api/xac-thuc/token); khong khop dang -> cong chinh thuc.
                string baseUrl = DEFAULT_BASE_URL;
                string t = string.IsNullOrWhiteSpace(tokenUrl) ? "" : tokenUrl.Trim().TrimEnd('/');
                int idx = t.Length - TOKEN_PATH.Length;
                if (idx > 0 && string.Compare(t, idx, TOKEN_PATH, 0, TOKEN_PATH.Length,
                        StringComparison.OrdinalIgnoreCase) == 0)
                    baseUrl = t.Substring(0, idx);
                return new Ksk831VlgSyncer(maDonVi, username, password, baseUrl);
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); return null; }
        }

        /// <summary>
        /// Đẩy 1 hồ sơ 831 lên cổng tiếp nhận. Tự set SENDER_CODE + REQUEST_ID vào header TRƯỚC khi
        /// serialize (không đụng bản XML đã gửi cổng CSDL dùng chung). maHoSo: mã điều trị/hồ sơ để ghi log.
        /// </summary>
        internal Ksk831PushResult Push(Data data, string maHoSo)
        {
            try
            {
                if (data == null)
                    return new Ksk831PushResult { Success = false, Message = "Không có dữ liệu hồ sơ" };
                if (this.batchFatalError != null)
                    return new Ksk831PushResult { Success = false, Message = this.batchFatalError };

                string token = GetToken();
                if (string.IsNullOrEmpty(token))
                    return new Ksk831PushResult { Success = false, Message = "Đăng nhập cổng thất bại — " + (this.lastAuthError ?? "") };

                if (data.Header == null) data.Header = new Header();
                data.Header.SenderCode = this.maDonVi;
                // Hash noi dung khi REQUEST_ID de trong -> ma on dinh: gui lai y nguyen = cung REQUEST_ID.
                data.Header.RequestId = "";
                string xmlForHash = Ksk831Serializer.ToXml(data);
                string requestId = "HIS831-" + (maHoSo ?? "") + "-" + Sha256Hex12(xmlForHash);
                if (requestId.Length > 100) requestId = requestId.Substring(0, 100);   // tai lieu: toi da 100 ky tu
                data.Header.RequestId = requestId;
                string xml = Ksk831Serializer.ToXml(data);

                byte[] body = new UTF8Encoding(false).GetBytes(xml ?? "");
                if (body.Length > MAX_BODY_BYTES)
                    return new Ksk831PushResult { Success = false, Message = "Bản tin vượt giới hạn 10 MiB của cổng (" + body.Length + " byte)" };

                Inventec.Common.Logging.LogSystem.Info(string.Format(
                    "HSSK831-VLG PUSH REQ: url={0}{1}; maHoSo={2}; request_id={3}; size={4} bytes",
                    this.baseUrl, PUSH_PATH, maHoSo, requestId, body.Length));

                int status;
                string respBody = HttpSend("POST", this.baseUrl + PUSH_PATH, "application/xml; charset=utf-8", body, token, out status);
                if (status == 401)
                {
                    // Token het han giua lo -> login lai va gui lan cuoi.
                    this.cachedToken = null; this.tokenExpireAt = DateTime.MinValue;
                    token = GetToken();
                    if (string.IsNullOrEmpty(token))
                        return new Ksk831PushResult { Success = false, Message = "Đăng nhập lại cổng thất bại (401) — " + (this.lastAuthError ?? "") };
                    respBody = HttpSend("POST", this.baseUrl + PUSH_PATH, "application/xml; charset=utf-8", body, token, out status);
                }
                if (status == 0)
                {
                    this.batchFatalError = "không kết nối được cổng tiếp nhận (" + this.baseUrl + ") — kiểm tra mạng/firewall";
                    return new Ksk831PushResult { Success = false, Message = this.batchFatalError };
                }

                JObject jo = null;
                try { jo = string.IsNullOrEmpty(respBody) ? null : JObject.Parse(respBody); }
                catch { }
                string code = (jo != null) ? ((string)jo["code"] ?? "") : "";
                string message = (jo != null) ? ((string)jo["message"] ?? "") : Truncate(respBody);
                // Body loi dang {"data": null}: jo["data"] la JValue(Null) khac null reference — phai cast JObject.
                var jd = (jo != null) ? jo["data"] as JObject : null;
                string trackingId = (jd != null) ? (string)jd["tracking_id"] : null;
                bool accepted = (status == 200) && (jo != null) && ((bool?)jo["success"] == true)
                    && code.StartsWith("ACCEPTED", StringComparison.OrdinalIgnoreCase);

                Inventec.Common.Logging.LogSystem.Info(string.Format(
                    "HSSK831-VLG PUSH RESP: maHoSo={0}; status={1}; code={2}; tracking={3}; message={4}",
                    maHoSo, status, code, trackingId, Truncate(message)));

                if (accepted)
                    return new Ksk831PushResult
                    {
                        Success = true,
                        Message = "đã tiếp nhận (" + code + (string.IsNullOrEmpty(trackingId) ? "" : (" — " + trackingId)) + ")"
                    };
                return new Ksk831PushResult
                {
                    Success = false,
                    Message = "Cổng từ chối (HTTP " + status + " " + code + "): " + Truncate(message)
                };
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return new Ksk831PushResult { Success = false, Message = ex.Message };
            }
        }

        /// <summary>POST /api/xac-thuc/token (JSON username/password) -&gt; data.access_token; cache theo expires_in − 60s.</summary>
        private string GetToken()
        {
            if (!string.IsNullOrWhiteSpace(this.cachedToken) && DateTime.Now < this.tokenExpireAt)
                return this.cachedToken;
            this.lastAuthError = null;
            try
            {
                string loginJson = Newtonsoft.Json.JsonConvert.SerializeObject(
                    new { username = this.username, password = this.password });
                Inventec.Common.Logging.LogSystem.Info("HSSK831-VLG LOGIN REQ: url=" + this.baseUrl + TOKEN_PATH
                    + "; username=" + this.username + "; password=***");
                int status;
                string body = HttpSend("POST", this.baseUrl + TOKEN_PATH, "application/json; charset=utf-8",
                    Encoding.UTF8.GetBytes(loginJson), null, out status);
                if (status == 0)
                {
                    this.lastAuthError = "không kết nối được cổng (" + this.baseUrl + ")";
                    this.batchFatalError = this.lastAuthError;
                    return null;
                }
                JObject jo = null;
                try { jo = string.IsNullOrEmpty(body) ? null : JObject.Parse(body); }
                catch { }
                // Body loi dang {"data": null}: jo["data"] la JValue(Null) — index tiep se nem exception
                // va lam truot khoi latch 401/403/429 ben duoi -> phai cast JObject.
                var jd = (jo != null) ? jo["data"] as JObject : null;
                string token = (jd != null) ? (string)jd["access_token"] : null;
                if (status != 200 || string.IsNullOrEmpty(token))
                {
                    this.lastAuthError = "HTTP " + status
                        + ((jo != null) ? (" " + (string)jo["code"] + " — " + (string)jo["message"]) : "");
                    // Sai tai khoan/khoa/rate-limit khong tu het -> chan cac ho so sau cua lo.
                    if (status == 401 || status == 403 || status == 429)
                        this.batchFatalError = "đăng nhập cổng thất bại — " + this.lastAuthError;
                    Inventec.Common.Logging.LogSystem.Warn("HSSK831-VLG LOGIN FAIL: " + this.lastAuthError);
                    return null;
                }
                long ttl = TOKEN_TTL_DEFAULT_SECONDS;
                try { if (jd["expires_in"] != null) ttl = (long)jd["expires_in"]; }
                catch { }
                if (ttl <= 0) ttl = TOKEN_TTL_DEFAULT_SECONDS;
                this.cachedToken = token;
                this.tokenExpireAt = DateTime.Now.AddSeconds(Math.Max(60, ttl - TOKEN_TTL_SAFETY_SECONDS));
                Inventec.Common.Logging.LogSystem.Info("HSSK831-VLG LOGIN OK: token het han " + this.tokenExpireAt.ToString("HH:mm:ss"));
                return this.cachedToken;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                this.lastAuthError = ex.Message;
                return null;
            }
        }

        private static string HttpSend(string method, string url, string contentType, byte[] bodyBytes,
            string bearerToken, out int statusCode)
        {
            statusCode = 0;
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = method;
            request.Accept = "application/json";
            request.Timeout = HTTP_TIMEOUT_MS;
            request.ReadWriteTimeout = HTTP_TIMEOUT_MS;
            if (!string.IsNullOrEmpty(contentType)) request.ContentType = contentType;
            if (!string.IsNullOrEmpty(bearerToken))
                request.Headers.Add("Authorization", "Bearer " + bearerToken);
            try
            {
                if (bodyBytes != null && bodyBytes.Length > 0)
                {
                    request.ContentLength = bodyBytes.Length;
                    using (var stream = request.GetRequestStream())
                        stream.Write(bodyBytes, 0, bodyBytes.Length);
                }
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    statusCode = (int)response.StatusCode;
                    return ReadBody(response);
                }
            }
            catch (WebException wex)
            {
                var errResponse = wex.Response as HttpWebResponse;
                if (errResponse == null)
                {
                    Inventec.Common.Logging.LogSystem.Error("HSSK831-VLG: khong ket noi duoc " + url + " — " + wex.Message, wex);
                    return null;
                }
                using (errResponse)
                {
                    statusCode = (int)errResponse.StatusCode;
                    return ReadBody(errResponse);
                }
            }
        }

        private static string ReadBody(HttpWebResponse response)
        {
            try
            {
                using (var stream = response.GetResponseStream())
                {
                    if (stream == null) return null;
                    using (var reader = new System.IO.StreamReader(stream, Encoding.UTF8))
                        return reader.ReadToEnd();
                }
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); return null; }
        }

        private static string Get(string[] arr, int index)
        {
            return (arr != null && index < arr.Length && arr[index] != null) ? arr[index].Trim() : null;
        }

        /// <summary>12 hex đầu SHA256 — sinh REQUEST_ID idempotency ổn định theo nội dung.</summary>
        private static string Sha256Hex12(string input)
        {
            try
            {
                using (var sha = System.Security.Cryptography.SHA256.Create())
                {
                    byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(input ?? ""));
                    var sb = new StringBuilder();
                    for (int i = 0; i < 6 && i < hash.Length; i++) sb.Append(hash[i].ToString("x2"));
                    return sb.ToString();
                }
            }
            catch { return DateTime.Now.Ticks.ToString("x12"); }
        }

        private static string Truncate(string s)
        {
            if (string.IsNullOrEmpty(s)) return "";
            s = s.Trim();
            return (s.Length > 500) ? s.Substring(0, 500) : s;
        }
    }
}
