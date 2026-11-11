/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 */
using HIS.Desktop.Plugins.Library.KhoDuLieuYTe.Config;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace HIS.Desktop.Plugins.Library.KhoDuLieuYTe.Api
{
    /// <summary>Kết quả của 1 bản ghi trong 1 request gửi kho.</summary>
    internal class KdlRecordResult
    {
        internal bool Success { get; set; }
        internal bool Existed { get; set; }
        internal string Message { get; set; }
    }

    /// <summary>
    /// Client Kho dữ liệu ngành Y tế (đặc tả KDL-YT-API-POST v1.0):
    /// POST {BaseUrl}/auth/token {username,password} -> access_token (3 giờ, không có refresh token);
    /// POST {BaseUrl}{path} body {"data":[...]} + Bearer. Kho kiểm tra từng bản ghi độc lập:
    /// HTTP 200 vẫn có thể có bản ghi bị từ chối (totalError, data[].stt/errors[]).
    /// </summary>
    internal class KdlApiClient
    {
        private const string TOKEN_PATH = "/auth/token";
        private const long TOKEN_TTL_DEFAULT_SECONDS = 10800;
        private const long TOKEN_TTL_SAFETY_SECONDS = 60;
        private const int HTTP_TIMEOUT_MS = 60000;

        // Token dùng chung trong phiên làm việc, theo BaseUrl + tài khoản
        private static readonly Dictionary<string, KeyValuePair<string, DateTime>> tokenCache = new Dictionary<string, KeyValuePair<string, DateTime>>();
        private static readonly object tokenLock = new object();

        private readonly KdlConfig config;
        private string lastAuthError;

        /// <summary>(vCong 62522) Thông báo đăng nhập thất bại đã phân loại nguồn lỗi (hiện cho người dùng)</summary>
        private string lastAuthMessage;

        /// <summary>(vCong 62522) Lỗi kết nối của lần HttpSend gần nhất (trước đây chỉ ghi log, trả status 0)</summary>
        [ThreadStatic]
        private static Exception lastSendError;

        internal static Inventec.Common.IntegrationError.IntegrationPartner Partner
        {
            get { return Inventec.Common.IntegrationError.IntegrationPartner.HealthPortal("kho dữ liệu y tế"); }
        }

        static KdlApiClient()
        {
            try { ServicePointManager.SecurityProtocol |= (SecurityProtocolType)3072; }   // TLS 1.2
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        internal KdlApiClient(KdlConfig config)
        {
            this.config = config;
        }

        /// <summary>Gửi 1 lô bản ghi, trả kết quả theo đúng thứ tự của <paramref name="records"/>.</summary>
        internal List<KdlRecordResult> Push(string path, List<object> records)
        {
            List<KdlRecordResult> results = new List<KdlRecordResult>();
            try
            {
                string token = GetToken(false);
                if (string.IsNullOrEmpty(token))
                    return AllFailed(records.Count, this.lastAuthMessage ?? ("Đăng nhập Kho dữ liệu thất bại: " + (this.lastAuthError ?? ""))); // vCong 62522

                string json = JsonConvert.SerializeObject(new { data = records });
                byte[] body = new UTF8Encoding(false).GetBytes(json);
                string url = this.config.BaseUrl + path;
                Inventec.Common.Logging.LogSystem.Info(string.Format("KDL-SYT PUSH REQ: url={0}; records={1}; size={2} bytes", url, records.Count, body.Length));

                int status;
                string respBody = HttpSend(url, body, token, out status);
                if (status == 401)
                {
                    // Token hết hạn giữa chừng -> lấy token mới, gửi lại 1 lần
                    token = GetToken(true);
                    if (string.IsNullOrEmpty(token))
                        return AllFailed(records.Count, this.lastAuthMessage ?? ("Đăng nhập lại Kho dữ liệu thất bại: " + (this.lastAuthError ?? ""))); // vCong 62522
                    respBody = HttpSend(url, body, token, out status);
                }
                if (status == 0)
                    return AllFailed(records.Count, ConnectionMessage(url, "gửi dữ liệu")); // vCong 62522: kèm lý do thật (timeout, DNS, SSL...)

                JObject jo = null;
                try { jo = string.IsNullOrEmpty(respBody) ? null : JObject.Parse(respBody); }
                catch { }
                string message = (jo != null) ? ((string)jo["message"] ?? "") : Truncate(respBody);
                Inventec.Common.Logging.LogSystem.Info(string.Format("KDL-SYT PUSH RESP: status={0}; total={1}; totalError={2}; message={3}",
                    status, jo != null ? (string)jo["total"] : "", jo != null ? (string)jo["totalError"] : "", Truncate(message)));

                if (status != 200 && status != 400)
                    return AllFailed(records.Count, Inventec.Common.IntegrationError.IntegrationErrorMessage.FromException(Inventec.Common.IntegrationError.IntegrationHttpError.FromStatus(Partner, url, status, null, string.IsNullOrEmpty(message) ? respBody : message), Partner, "gửi dữ liệu")); // vCong 62522

                // data[] chỉ liệt kê bản ghi bị từ chối, stt tính từ 1 theo vị trí trong mảng đã gửi
                Dictionary<int, JObject> errorByStt = new Dictionary<int, JObject>();
                JArray errData = (jo != null) ? jo["data"] as JArray : null;
                if (errData != null)
                {
                    foreach (JToken item in errData)
                    {
                        JObject e = item as JObject;
                        if (e == null || e["stt"] == null) continue;
                        int stt;
                        if (int.TryParse((string)e["stt"], out stt)) errorByStt[stt] = e;
                    }
                }

                // HTTP 400 không kèm lỗi từng bản ghi = lỗi cấp request (sai cấu trúc, vượt giới hạn, đơn vị chưa cấu hình API...)
                if (status == 400 && errorByStt.Count == 0)
                    return AllFailed(records.Count, Inventec.Common.IntegrationError.IntegrationErrorMessage.PartnerError(Partner, Truncate(message), "Kho dữ liệu từ chối yêu cầu")); // vCong 62522

                for (int i = 0; i < records.Count; i++)
                {
                    JObject e;
                    if (!errorByStt.TryGetValue(i + 1, out e))
                    {
                        results.Add(new KdlRecordResult { Success = true });
                        continue;
                    }
                    if (IsOnlyExistedError(e))
                    {
                        results.Add(new KdlRecordResult { Success = true, Existed = true, Message = (string)e["message"] });
                        continue;
                    }
                    results.Add(new KdlRecordResult { Success = false, Message = Inventec.Common.IntegrationError.IntegrationErrorMessage.PartnerError(Partner, JoinErrors(e), "Kho dữ liệu từ chối bản ghi") }); // vCong 62522
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return AllFailed(records.Count, Inventec.Common.IntegrationError.IntegrationErrorMessage.FromException(ex, Partner, "gửi dữ liệu")); // vCong 62522
            }
            return results;
        }

        /// <summary>Lỗi duy nhất là RECORD "Bản ghi đã tồn tại" -> kho đã có giấy này, coi như đã gửi.</summary>
        private static bool IsOnlyExistedError(JObject e)
        {
            JArray errors = e["errors"] as JArray;
            if (errors == null || errors.Count == 0)
                return ContainsExisted((string)e["message"]);
            foreach (JToken err in errors)
            {
                if (!string.Equals((string)err["rule"], "RECORD", StringComparison.OrdinalIgnoreCase)
                    || !ContainsExisted((string)err["message"]))
                    return false;
            }
            return true;
        }

        private static bool ContainsExisted(string message)
        {
            return !string.IsNullOrEmpty(message) && message.IndexOf("đã tồn tại", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static string JoinErrors(JObject e)
        {
            List<string> messages = new List<string>();
            JArray errors = e["errors"] as JArray;
            if (errors != null)
            {
                foreach (JToken err in errors)
                {
                    string msg = (string)err["message"];
                    string field = (string)err["field"];
                    if (string.IsNullOrEmpty(msg)) continue;
                    messages.Add(string.IsNullOrEmpty(field) ? msg : field + ": " + msg);
                }
            }
            if (messages.Count == 0)
                messages.Add((string)e["message"] ?? "Bản ghi bị từ chối");
            return string.Join("; ", messages);
        }

        private string GetToken(bool forceNew)
        {
            string cacheKey = this.config.BaseUrl + "|" + this.config.Username;
            lock (tokenLock)
            {
                KeyValuePair<string, DateTime> cached;
                if (!forceNew && tokenCache.TryGetValue(cacheKey, out cached) && DateTime.Now < cached.Value)
                    return cached.Key;
                tokenCache.Remove(cacheKey);
            }

            this.lastAuthError = null;
            this.lastAuthMessage = null;
            try
            {
                string loginJson = JsonConvert.SerializeObject(new { username = this.config.Username, password = this.config.Password });
                Inventec.Common.Logging.LogSystem.Info("KDL-SYT LOGIN REQ: url=" + this.config.BaseUrl + TOKEN_PATH + "; username=" + this.config.Username + "; password=***");
                int status;
                string body = HttpSend(this.config.BaseUrl + TOKEN_PATH, Encoding.UTF8.GetBytes(loginJson), null, out status);
                if (status == 0)
                {
                    this.lastAuthError = "không kết nối được " + this.config.BaseUrl;
                    this.lastAuthMessage = ConnectionMessage(this.config.BaseUrl + TOKEN_PATH, "đăng nhập"); // vCong 62522
                    return null;
                }
                JObject jo = null;
                try { jo = string.IsNullOrEmpty(body) ? null : JObject.Parse(body); }
                catch { }
                string token = (jo != null) ? (string)jo["access_token"] : null;
                if (status != 200 || string.IsNullOrEmpty(token))
                {
                    this.lastAuthError = "HTTP " + status + ((jo != null) ? (" - " + (string)jo["message"]) : "");
                    Inventec.Common.Logging.LogSystem.Warn("KDL-SYT LOGIN FAIL: " + this.lastAuthError);
                    // (vCong 62522) 400/401/403 = tài khoản thiết lập tại viện; 5xx = phía kho/kết nối; 200 không có token = kho trả sai cấu trúc
                    if (status == 400 || status == 401 || status == 403)
                        this.lastAuthMessage = Inventec.Common.IntegrationError.IntegrationErrorMessage.LoginError(Partner, this.lastAuthError);
                    else if (status == 200)
                        this.lastAuthMessage = Inventec.Common.IntegrationError.IntegrationErrorMessage.PartnerError(Partner, "Phản hồi đăng nhập không có access_token: " + Truncate(body), "Kho dữ liệu trả kết quả đăng nhập không hợp lệ");
                    else
                        this.lastAuthMessage = Inventec.Common.IntegrationError.IntegrationErrorMessage.FromException(Inventec.Common.IntegrationError.IntegrationHttpError.FromStatus(Partner, this.config.BaseUrl + TOKEN_PATH, status, null, body), Partner, "đăng nhập");
                    return null;
                }
                long ttl = TOKEN_TTL_DEFAULT_SECONDS;
                try { if (jo["expires_in"] != null) ttl = (long)jo["expires_in"]; }
                catch { }
                if (ttl <= 0) ttl = TOKEN_TTL_DEFAULT_SECONDS;
                DateTime expireAt = DateTime.Now.AddSeconds(Math.Max(60, ttl - TOKEN_TTL_SAFETY_SECONDS));
                lock (tokenLock)
                {
                    tokenCache[cacheKey] = new KeyValuePair<string, DateTime>(token, expireAt);
                }
                Inventec.Common.Logging.LogSystem.Info("KDL-SYT LOGIN OK: token het han " + expireAt.ToString("HH:mm:ss"));
                return token;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                this.lastAuthError = ex.Message;
                this.lastAuthMessage = Inventec.Common.IntegrationError.IntegrationErrorMessage.FromException(ex, Partner, "đăng nhập"); // vCong 62522
                return null;
            }
        }

        private static string HttpSend(string url, byte[] bodyBytes, string bearerToken, out int statusCode)
        {
            statusCode = 0;
            lastSendError = null;
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.Accept = "application/json";
            request.ContentType = "application/json; charset=UTF-8";
            request.Timeout = HTTP_TIMEOUT_MS;
            request.ReadWriteTimeout = HTTP_TIMEOUT_MS;
            if (!string.IsNullOrEmpty(bearerToken))
                request.Headers.Add("Authorization", "Bearer " + bearerToken);
            try
            {
                request.ContentLength = bodyBytes.Length;
                using (var stream = request.GetRequestStream())
                    stream.Write(bodyBytes, 0, bodyBytes.Length);
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
                    Inventec.Common.Logging.LogSystem.Error("KDL-SYT: khong ket noi duoc " + url + " - " + wex.Message, wex);
                    lastSendError = wex; // vCong 62522
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

        /// <summary>Thông báo không kết nối được (status 0) kèm lý do thật của WebException (vCong 62522)</summary>
        private static string ConnectionMessage(string url, string action)
        {
            if (lastSendError == null) return Inventec.Common.IntegrationError.IntegrationErrorMessage.NoResponse(Partner, action);
            return Inventec.Common.IntegrationError.IntegrationErrorMessage.FromException(Inventec.Common.IntegrationError.IntegrationHttpError.WrapCall(lastSendError, Partner, url), Partner, action);
        }

        private static List<KdlRecordResult> AllFailed(int count, string message)
        {
            List<KdlRecordResult> results = new List<KdlRecordResult>();
            for (int i = 0; i < count; i++)
                results.Add(new KdlRecordResult { Success = false, Message = message });
            return results;
        }

        private static string Truncate(string s)
        {
            if (string.IsNullOrEmpty(s)) return "";
            s = s.Trim();
            return (s.Length > 500) ? s.Substring(0, 500) : s;
        }
    }
}
