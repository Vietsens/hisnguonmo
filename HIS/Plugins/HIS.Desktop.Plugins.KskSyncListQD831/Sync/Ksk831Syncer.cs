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
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using HIS.Desktop.Plugins.KskSyncListQD831.Sync.Model;
using Newtonsoft.Json;

namespace HIS.Desktop.Plugins.KskSyncListQD831.Sync
{
    /// <summary>Kết quả đẩy 1 hồ sơ lên cổng HSSK QĐ831.</summary>
    internal class Ksk831PushResult
    {
        internal bool Success { get; set; }
        internal string Message { get; set; }
    }

    /// <summary>
    /// Đồng bộ hồ sơ QĐ831 lên cổng HSSK: login lấy token (Bearer), đẩy từng hồ sơ (multipart: xmlFile + nguoi_gui).
    /// Token có hạn 3 giờ -&gt; chủ động login lại khi đã dùng &gt;= 2 giờ 30 phút (dùng lại syncer cho cả lô để
    /// login 1 lần, tự làm mới token giữa chừng khi đẩy nhiều hồ sơ).
    /// </summary>
    internal class Ksk831Syncer
    {
        private const int TOKEN_REFRESH_MINUTES = 150;   // 2h30 (token hạn 3h)

        private readonly Ksk831SyncConfig cfg;
        private string token;
        private DateTime tokenTimeUtc;

        internal Ksk831Syncer(Ksk831SyncConfig cfg)
        {
            this.cfg = cfg;
            EnsureTls12();
        }

        /// <summary>Lấy token còn hiệu lực; hết 2h30 thì login lại.</summary>
        private string EnsureToken()
        {
            if (!string.IsNullOrEmpty(token) && (DateTime.UtcNow - tokenTimeUtc).TotalMinutes < TOKEN_REFRESH_MINUTES)
                return token;
            Inventec.Common.Logging.LogSystem.Info("HSSK831 TOKEN: đăng nhập lấy/làm mới token (ngưỡng " + TOKEN_REFRESH_MINUTES + " phút).");
            token = Login();
            tokenTimeUtc = DateTime.UtcNow;
            return token;
        }

        /// <summary>POST /get-token (multipart username/password) -&gt; Ksk831TokenResponse.token. Lỗi -&gt; ném exception.</summary>
        private string Login()
        {
            var reqLogin = new Ksk831LoginRequest { Username = cfg.Username, Password = cfg.Password };
            // LOG INPUT (ẩn mật khẩu)
            Inventec.Common.Logging.LogSystem.Info(string.Format(
                "HSSK831 LOGIN REQ: url={0}; username={1}; password=***", cfg.LoginUrl, reqLogin.Username));
            using (var client = new HttpClient())
            using (var form = new MultipartFormDataContent())
            {
                form.Add(new StringContent(reqLogin.Username ?? ""), "username");
                form.Add(new StringContent(reqLogin.Password ?? ""), "password");
                HttpResponseMessage resp = client.PostAsync(cfg.LoginUrl, form).Result;
                string body = resp.Content.ReadAsStringAsync().Result;
                // LOG CHI TIẾT (TEMP): full body
                Inventec.Common.Logging.LogSystem.Info("HSSK831 LOGIN RESP BODY (chi tiết): status=" + (int)resp.StatusCode + "; body=" + body);
                if (!resp.IsSuccessStatusCode)
                {
                    Inventec.Common.Logging.LogSystem.Info("HSSK831 LOGIN RESP FAIL: status=" + (int)resp.StatusCode + "; body=" + Truncate(body));
                    throw new Exception("Đăng nhập cổng HSSK thất bại (" + (int)resp.StatusCode + "): " + Truncate(body));
                }

                Ksk831TokenResponse tokenResp = JsonConvert.DeserializeObject<Ksk831TokenResponse>(body);
                // LOG OUTPUT (che token)
                Inventec.Common.Logging.LogSystem.Info(string.Format(
                    "HSSK831 LOGIN RESP OK: status={0}; success={1}; message={2}; time={3}phut; token={4}",
                    (int)resp.StatusCode, tokenResp != null && tokenResp.Success, tokenResp != null ? tokenResp.Message : "",
                    tokenResp != null ? tokenResp.Time : 0, MaskToken(tokenResp != null ? tokenResp.Token : null)));
                if (tokenResp == null || string.IsNullOrEmpty(tokenResp.Token))
                    throw new Exception("Cổng HSSK không trả về token: " + Truncate(body));
                return tokenResp.Token;
            }
        }

        /// <summary>
        /// Đẩy 1 hồ sơ (xml). nguoiGui: thông tin người đồng bộ (ghi log). Tự làm mới token khi cần;
        /// nếu 401 (token hết hạn) -&gt; login lại 1 lần và đẩy lại.
        /// </summary>
        internal Ksk831PushResult Push(string xml, string nguoiGui)
        {
            try
            {
                string tk = EnsureToken();
                Ksk831PushResult r = DoPush(xml, nguoiGui, tk);
                if (r != null && !r.Success && r.Message == "401")
                {
                    // Token hết hạn giữa chừng -> login lại và thử lần cuối.
                    token = null;
                    tk = EnsureToken();
                    r = DoPush(xml, nguoiGui, tk);
                }
                return r;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return new Ksk831PushResult { Success = false, Message = ex.Message };
            }
        }

        private Ksk831PushResult DoPush(string xml, string nguoiGui, string tk)
        {
            var reqPush = new Ksk831ImportRequest
            {
                XmlFile = new UTF8Encoding(false).GetBytes(xml ?? ""),
                FileName = "hoso831.xml",
                NguoiGui = nguoiGui
            };
            // LOG INPUT (tóm tắt)
            Inventec.Common.Logging.LogSystem.Info(string.Format(
                "HSSK831 PUSH REQ: url={0}; nguoi_gui={1}; xmlFile={2} ({3} bytes); token={4}",
                cfg.PushUrl, reqPush.NguoiGui, reqPush.FileName, reqPush.XmlFile.Length, MaskToken(tk)));
            // LOG CHI TIẾT (TEMP): full token + full XML (chứa dữ liệu bệnh nhân) — gỡ khi xong debug.
            Inventec.Common.Logging.LogSystem.Info("HSSK831 PUSH REQ TOKEN (chi tiết): " + (tk ?? ""));
            Inventec.Common.Logging.LogSystem.Info("HSSK831 PUSH REQ XML (chi tiết):" + Environment.NewLine + (xml ?? ""));
            using (var client = new HttpClient())
            using (var form = new MultipartFormDataContent())
            {
                var xmlContent = new ByteArrayContent(reqPush.XmlFile);
                xmlContent.Headers.ContentType = new MediaTypeHeaderValue("text/xml");
                form.Add(xmlContent, "xmlFile", reqPush.FileName);
                form.Add(new StringContent(reqPush.NguoiGui ?? ""), "nguoi_gui");

                var req = new HttpRequestMessage(HttpMethod.Post, cfg.PushUrl) { Content = form };
                req.Headers.Authorization = new AuthenticationHeaderValue("bearer", tk);

                HttpResponseMessage resp = client.SendAsync(req).Result;
                string body = resp.Content.ReadAsStringAsync().Result;

                // LOG OUTPUT (chi tiết: full body)
                Inventec.Common.Logging.LogSystem.Info(string.Format(
                    "HSSK831 PUSH RESP: status={0}; body={1}", (int)resp.StatusCode, body));

                if ((int)resp.StatusCode == 401)
                    return new Ksk831PushResult { Success = false, Message = "401" };  // báo hiệu cần login lại
                if (resp.IsSuccessStatusCode)
                    return new Ksk831PushResult { Success = true, Message = ExtractValue(body) };

                return new Ksk831PushResult
                {
                    Success = false,
                    Message = "Đẩy thất bại (" + (int)resp.StatusCode + "): " + Truncate(body)
                };
            }
        }

        /// <summary>Lấy trường "Value" (Ksk831ImportResponse) trong JSON kết quả (nếu có).</summary>
        private static string ExtractValue(string body)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(body)) return "Thành công";
                Ksk831ImportResponse r = JsonConvert.DeserializeObject<Ksk831ImportResponse>(body);
                return (r != null && !string.IsNullOrEmpty(r.Value)) ? r.Value : "Thành công";
            }
            catch { return "Thành công"; }
        }

        private static string Truncate(string s)
        {
            if (string.IsNullOrEmpty(s)) return "";
            s = s.Trim();
            return (s.Length > 500) ? s.Substring(0, 500) : s;
        }

        /// <summary>Che token khi ghi log: chỉ hiện 6 ký tự đầu + độ dài.</summary>
        private static string MaskToken(string tk)
        {
            if (string.IsNullOrEmpty(tk)) return "(rỗng)";
            string head = tk.Length > 6 ? tk.Substring(0, 6) : tk;
            return head + "***(len=" + tk.Length + ")";
        }

        private static void EnsureTls12()
        {
            try
            {
                // .NET 4.5 mặc định chưa bật TLS 1.2 -> bổ sung để gọi https cổng.
                ServicePointManager.SecurityProtocol |= (SecurityProtocolType)3072; // Tls12
                if (ServicePointManager.DefaultConnectionLimit < 20) ServicePointManager.DefaultConnectionLimit = 20;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }
    }
}
