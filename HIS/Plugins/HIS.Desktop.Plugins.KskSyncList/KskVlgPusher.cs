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
using System.Text;

namespace HIS.Desktop.Plugins.KskSyncList
{
    /// <summary>
    /// Cau hinh cong KDLYT Vinh Long (Cong tiep nhan — https://congtiepnhan.kdlyt.vinhlong.vn),
    /// khoa <c>MOS.HIS_KSK_SYNC.VLG_2062_CONNECTION_INFO</c>. Vien nao KHONG cau hinh khoa nay thi
    /// cong Vinh Long khong hien/khong day — khong anh huong vien khac.
    /// Dinh dang (cac truong cach '|'):
    /// <code>
    /// MaDonVi|Username|Password|TokenUrl|PushUrl
    /// </code>
    /// <list type="bullet">
    /// <item>MaDonVi: ma don vi do tinh cap (vd 86001) — dung lam THONGTINDONVI/MACSKCB cua ban tin;
    /// PHAI khop ma_don_vi cua tai khoan token (lech -> cong tra 403 MACSKCB_MISMATCH).</item>
    /// <item>Username/Password: tai khoan tich hop do tinh cap (POST /api/xac-thuc/token).</item>
    /// <item>TokenUrl/PushUrl: URL DAY DU; bo trong -> URL cong chinh thuc Vinh Long
    /// (moi truong dev: doi 2 URL nay sang https://dev-congtiepnhan.kdlyt.vinhlong.vn/...).</item>
    /// </list>
    /// Toi thieu 3 truong dau (MaDonVi, Username, Password) — thieu thi tra null (coi nhu chua cau hinh).
    /// </summary>
    internal class KskVlgConfig
    {
        internal string MaDonVi { get; set; }
        internal string Username { get; set; }
        internal string Password { get; set; }
        internal string TokenUrl { get; set; }
        internal string PushUrl { get; set; }
    }

    internal static class KskVlgConfigParser
    {
        internal const string DEFAULT_BASE_URL = "https://congtiepnhan.kdlyt.vinhlong.vn";
        internal const string DEFAULT_TOKEN_URL = DEFAULT_BASE_URL + "/api/xac-thuc/token";
        internal const string DEFAULT_PUSH_URL = DEFAULT_BASE_URL + "/api/kham-suc-khoe/qd-2062/tiep-nhan";
        private const int MIN_FIELD_COUNT = 3;

        internal static KskVlgConfig Parse(string configValue)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(configValue)) return null;
                string[] f = configValue.Split('|');
                if (f.Length < MIN_FIELD_COUNT) return null;

                string maDonVi = Get(f, 0);
                string username = Get(f, 1);
                string password = Get(f, 2);
                if (string.IsNullOrWhiteSpace(maDonVi) || string.IsNullOrWhiteSpace(username)
                    || string.IsNullOrWhiteSpace(password))
                    return null;

                KskVlgConfig cfg = new KskVlgConfig();
                cfg.MaDonVi = maDonVi;
                cfg.Username = username;
                cfg.Password = password;

                string tokenUrl = Get(f, 3);
                cfg.TokenUrl = !string.IsNullOrWhiteSpace(tokenUrl) ? tokenUrl : DEFAULT_TOKEN_URL;

                string pushUrl = Get(f, 4);
                cfg.PushUrl = !string.IsNullOrWhiteSpace(pushUrl) ? pushUrl : DEFAULT_PUSH_URL;

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

    /// <summary>Ket qua day 1 ho so len cong Vinh Long (chuan hoa de gop voi ket qua cac cong khac).</summary>
    internal class KskVlgPushResult
    {
        internal bool Success { get; set; }
        internal string Message { get; set; }
        internal string TrackingId { get; set; }   // data.tracking_id — ma tra cuu phia cong
        internal string Status { get; set; }       // data.status (QUEUED = da tiep nhan, xu ly bat dong bo)
        // Luu y khi THANH CONG (code ACCEPTED_WITH_WARNING / warnings[] cua cong) — hien len ket qua
        // de nhan vien biet ho so co luu y can xu ly (khong chi nam trong log ky thuat).
        internal string Warning { get; set; }

        internal static KskVlgPushResult Failure(string message)
        {
            return new KskVlgPushResult { Success = false, Message = message };
        }
    }

    /// <summary>
    /// Model response CHUNG cua Cong tiep nhan Vinh Long (tai lieu "API Document CongTiepNhan" V1.3):
    /// { success, code, message, data:{...}, warnings[], errors[] }. Data dung chung cho ca 2 API
    /// (token: access_token/expires_in/ma_don_vi; push: tracking_id/status/signature_status).
    /// </summary>
    internal class KskVlgApiResponse
    {
        [Newtonsoft.Json.JsonProperty("success")]
        public bool Success { get; set; }
        [Newtonsoft.Json.JsonProperty("code")]
        public string Code { get; set; }
        [Newtonsoft.Json.JsonProperty("message")]
        public string Message { get; set; }
        [Newtonsoft.Json.JsonProperty("data")]
        public KskVlgApiData Data { get; set; }
        [Newtonsoft.Json.JsonProperty("warnings")]
        public Newtonsoft.Json.Linq.JToken Warnings { get; set; }
        [Newtonsoft.Json.JsonProperty("errors")]
        public Newtonsoft.Json.Linq.JToken Errors { get; set; }
    }

    internal class KskVlgApiData
    {
        [Newtonsoft.Json.JsonProperty("access_token")]
        public string AccessToken { get; set; }
        [Newtonsoft.Json.JsonProperty("token_type")]
        public string TokenType { get; set; }
        [Newtonsoft.Json.JsonProperty("expires_in")]
        public long ExpiresIn { get; set; }
        [Newtonsoft.Json.JsonProperty("expires_at")]
        public string ExpiresAt { get; set; }
        [Newtonsoft.Json.JsonProperty("ma_don_vi")]
        public string MaDonVi { get; set; }
        [Newtonsoft.Json.JsonProperty("tracking_id")]
        public string TrackingId { get; set; }
        [Newtonsoft.Json.JsonProperty("status")]
        public string Status { get; set; }
        [Newtonsoft.Json.JsonProperty("data_type")]
        public string DataType { get; set; }
        [Newtonsoft.Json.JsonProperty("signature_status")]
        public string SignatureStatus { get; set; }
        [Newtonsoft.Json.JsonProperty("received_at")]
        public string ReceivedAt { get; set; }
    }

    /// <summary>
    /// Day ban tin KSK len Cong tiep nhan — Kho du lieu y te tinh Vinh Long theo tai lieu
    /// "API Document CongTiepNhan" V1.3 (21/07/2026). Cau hinh: MOS.HIS_KSK_SYNC.VLG_2062_CONNECTION_INFO.
    ///
    /// Giao thuc KHAC truc BYT (khong tai su dung Qd1551Consumer duoc):
    ///   1. POST {TokenUrl} (/api/xac-thuc/token)  {username,password}
    ///      -> { success, data:{ access_token, token_type:"Bearer", expires_in, ma_don_vi } }
    ///   2. POST {PushUrl} (/api/kham-suc-khoe/qd-2062/tiep-nhan)  Authorization: Bearer;
    ///      body = JSON wrapper (Content-Type: application/json — tai lieu muc 5.1 dang b):
    ///        { "data_type": "xml", "data": "&lt;KHAMSUCKHOE&gt;...", "metadata": {...} }
    ///      data = chuoi XML KHAMSUCKHOE (KHONG base64); chu ky CKS_* nam TRONG XML (CHUKYDONVI).
    ///      metadata (tat ca optional): ma_yeu_cau = "&lt;ma dieu tri&gt;-&lt;12 hex SHA256 noi dung&gt;"
    ///      — PHAI kem hash: cung ma + noi dung DOI -> cong tra 409 REQUEST_ID_CONFLICT va TU CHOI
    ///      xu ly (kiem chung cong dev 10/08/2026); kem hash thi noi dung y nguyen -> ACCEPTED_DUPLICATE
    ///      (tra tracking_id cu), noi dung sua -> ma moi, khong bao gio 409. treatment_code = ma dieu
    ///      tri tran de tinh doi soat; sender_id/receiver_id/msg_type/txn_type lay theo cau hinh cong
    ///      BYT (MOS.HIS_KSK_SYNC.CONNECTION_INFO); msg_id/send_datetime sinh moi lan gui.
    ///   3. Thanh cong: HTTP 200 + success=true; code = ACCEPTED / ACCEPTED_WITH_WARNING /
    ///      ACCEPTED_DUPLICATE; data.status = QUEUED (tiep nhan BAT DONG BO — "da tiep nhan",
    ///      chua phai "da xu ly xong"); tracking_id de tra cuu.
    ///   4. Gioi han body 10 MiB (413 PAYLOAD_TOO_LARGE) — kiem truoc khi POST.
    /// </summary>
    internal class KskVlgPusher
    {
        private const long TOKEN_TTL_DEFAULT_SECONDS = 10800;   // tai lieu muc 2.1: expires_in = 10800
        private const long TOKEN_TTL_SAFETY_SECONDS = 60;       // tru bien de khong het han giua lo
        private const long TOKEN_TTL_MIN_SECONDS = 60;
        private const int MAX_ATTEMPT = 2;                      // 401 khi push -> dang nhap lai 1 lan
        private const int HTTP_OK = 200;
        private const int HTTP_UNAUTHORIZED = 401;
        private const int HTTP_TOO_MANY_REQUESTS = 429;         // RATE_LIMITED -> KHONG day lai ngay
        private const long MAX_BODY_BYTES = 10L * 1024 * 1024;  // tai lieu muc 1: toi da 10 MiB / lan gui
        private const int HTTP_TIMEOUT_MS = 120000;

        private readonly KskVlgConfig config;
        // Thong tin dinh tuyen/doi soat cho metadata — lay tu cau hinh cong BYT
        // (MOS.HIS_KSK_SYNC.CONNECTION_INFO: SenderId/ReceiverId/MsgType/TxnType). Rong -> bo qua truong do.
        private readonly string metaSenderId;
        private readonly string metaReceiverId;
        private readonly string metaMsgType;
        private readonly string metaTxnType;
        private string cachedToken;
        private DateTime tokenExpireAt = DateTime.MinValue;
        private string lastAuthError;
        // FAIL-FAST cho ca lo: login fail voi loi KHONG TU HET (0 = khong ket noi, 401 sai tai khoan,
        // 403 khoa tai khoan/thieu mapping, 429 rate-limit) -> cac Push() con lai cua lo tra Failure NGAY,
        // khong goi mang nua. Neu khong: vien cau hinh sai mat khau + tich N ho so = N lan POST token sai
        // lien tiep -> cong co the KHOA tai khoan tich hop (tai lieu muc 2.1: 403 ACCOUNT_LOCKED / 429
        // RATE_LIMITED); mat ket noi thi moi ho so treo den 120s timeout. Loi 5xx KHONG latch (co the tu het).
        private string batchAuthFatalError;

        /// <summary>Plugin .NET 4.5 khong bat TLS 1.2 mac dinh — cong Vinh Long HTTPS doi TLS >= 1.2.</summary>
        static KskVlgPusher()
        {
            try
            {
                System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        internal KskVlgPusher(KskVlgConfig config)
            : this(config, null, null, null, null)
        {
        }

        internal KskVlgPusher(KskVlgConfig config, string metaSenderId, string metaReceiverId,
            string metaMsgType, string metaTxnType)
        {
            this.config = config;
            this.metaSenderId = metaSenderId;
            this.metaReceiverId = metaReceiverId;
            this.metaMsgType = metaMsgType;
            this.metaTxnType = metaTxnType;
        }

        /// <summary>
        /// Day XML KHAMSUCKHOE cua MOT ho so (SOLUONGHOSO = 1) len cong Vinh Long, dong goi JSON wrapper
        /// kem metadata (ma_yeu_cau/treatment_code/dinh tuyen). Token cache trong instance nay (dung chung
        /// ca lo); push gap 401 thi dang nhap lai va day lai 1 lan.
        /// </summary>
        internal KskVlgPushResult Push(string xmlContent, string treatmentCode)
        {
            try
            {
                string configError = ValidateConfig();
                if (configError != null) return KskVlgPushResult.Failure(configError);
                // Lo nay da gap loi xac thuc/ket noi KHONG TU HET -> fail-fast, khong goi mang cho tung ho so.
                if (this.batchAuthFatalError != null)
                    return KskVlgPushResult.Failure("VLG: " + this.batchAuthFatalError
                        + " (bỏ qua — không thử lại từng hồ sơ để tránh bị cổng khóa tài khoản/giới hạn tần suất).");
                if (string.IsNullOrEmpty(xmlContent))
                    return KskVlgPushResult.Failure("VLG: không dựng được dữ liệu đẩy.");

                string maYeuCau;
                byte[] body = BuildJsonBody(xmlContent, treatmentCode, out maYeuCau);
                if (body == null)
                    return KskVlgPushResult.Failure("VLG: không đóng gói được bản tin (JSON wrapper).");
                if (body.LongLength > MAX_BODY_BYTES)
                    return KskVlgPushResult.Failure(string.Format(
                        "VLG: bản tin {0:N0} byte vượt giới hạn 10 MiB của cổng (PAYLOAD_TOO_LARGE)."
                        + " Kiểm tra ảnh chữ ký điện tử (CKDT_) / dữ liệu CLS của hồ sơ.", body.LongLength));

                for (int attempt = 0; attempt < MAX_ATTEMPT; attempt++)
                {
                    string token = GetToken();
                    if (string.IsNullOrWhiteSpace(token))
                        return KskVlgPushResult.Failure("VLG: đăng nhập cổng thất bại"
                            + (string.IsNullOrEmpty(this.lastAuthError) ? " (kiểm tra tài khoản tích hợp)." : (" — " + this.lastAuthError)));

                    int status;
                    string respBody = HttpPost(this.config.PushUrl, "application/json; charset=utf-8", body, token, out status);

                    // Token het han giua chung -> dang nhap lai va day lai DUNG 1 lan.
                    if (status == HTTP_UNAUTHORIZED && attempt + 1 < MAX_ATTEMPT)
                    {
                        Inventec.Common.Logging.LogSystem.Warn("VLG: cong tra 401 khi push -> dang nhap lai va day lai.");
                        ResetToken();
                        continue;
                    }

                    if (status == 0)
                        return KskVlgPushResult.Failure(
                            "VLG: không kết nối được cổng (kiểm tra mạng / URL đẩy dữ liệu: " + this.config.PushUrl + ").");

                    KskVlgApiResponse resp = ParseResponse(respBody);
                    if (resp == null)
                        return KskVlgPushResult.Failure("VLG: không đọc được phản hồi từ cổng (HTTP " + status + ").");

                    // Thanh cong = HTTP 200 + success=true. ACCEPTED_DUPLICATE (gui lai) cung la thanh cong.
                    if (status == HTTP_OK && resp.Success)
                    {
                        string trackingId = (resp.Data != null) ? resp.Data.TrackingId : null;
                        string state = (resp.Data != null) ? resp.Data.Status : null;
                        Inventec.Common.Logging.LogSystem.Info(string.Format(
                            "VLG: cong tiep nhan OK. code={0}; tracking_id={1}; status={2}; signature_status={3};"
                            + " ma_yeu_cau={4}",
                            resp.Code, trackingId, state,
                            (resp.Data != null) ? resp.Data.SignatureStatus : null, maYeuCau));
                        return new KskVlgPushResult
                        {
                            Success = true,
                            TrackingId = trackingId,
                            Status = state,
                            // Tiep nhan CO LUU Y (ACCEPTED_WITH_WARNING / warnings[]) -> dua len ket qua
                            // cho nhan vien thay, khong de troi im lang trong log.
                            Warning = BuildSuccessWarning(resp)
                        };
                    }

                    string reason = DescribeError(status, resp);
                    if (status == HTTP_TOO_MANY_REQUESTS)
                        reason += " Cổng đang giới hạn tần suất — chờ ít phút rồi đẩy lại.";
                    Inventec.Common.Logging.LogSystem.Warn("VLG: push that bai. " + reason
                        + " Body (cat 2000 ky tu): " + Cut(respBody, 2000));
                    return KskVlgPushResult.Failure("VLG: " + reason);
                }
                return KskVlgPushResult.Failure("VLG: xác thực thất bại (token hết hạn sau khi đăng nhập lại).");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return KskVlgPushResult.Failure("VLG: " + ex.Message);
            }
        }

        /// <summary>
        /// Kiem tra cau hinh du de day: ma don vi + tai khoan + 2 URL. Tra null neu hop le,
        /// nguoc lai tra thong bao loi.
        /// </summary>
        private string ValidateConfig()
        {
            if (this.config == null)
                return "VLG: chưa cấu hình kết nối (MOS.HIS_KSK_SYNC.VLG_2062_CONNECTION_INFO).";
            var missing = new List<string>();
            if (string.IsNullOrWhiteSpace(this.config.MaDonVi)) missing.Add("mã đơn vị");
            if (string.IsNullOrWhiteSpace(this.config.Username)) missing.Add("tài khoản");
            if (string.IsNullOrWhiteSpace(this.config.Password)) missing.Add("mật khẩu");
            if (string.IsNullOrWhiteSpace(this.config.TokenUrl)) missing.Add("URL lấy token");
            if (string.IsNullOrWhiteSpace(this.config.PushUrl)) missing.Add("URL đẩy dữ liệu");
            if (missing.Count == 0) return null;
            return "VLG: cấu hình kết nối thiếu " + string.Join(", ", missing.ToArray())
                 + " (MOS.HIS_KSK_SYNC.VLG_2062_CONNECTION_INFO).";
        }

        /// <summary>
        /// Token cache theo expires_in cong tra ve (mac dinh 10800s, tru 60s an toan, toi thieu 60s).
        /// Login that bai -> null + ly do o lastAuthError (401 INVALID_CREDENTIALS / 403 ACCOUNT_LOCKED /
        /// 403 MISSING_ORG_MAPPING / 429 RATE_LIMITED... — cac loi nay KHONG retry duoc bang dang nhap lai).
        /// </summary>
        private string GetToken()
        {
            if (!string.IsNullOrWhiteSpace(this.cachedToken) && DateTime.Now < this.tokenExpireAt)
                return this.cachedToken;

            this.lastAuthError = null;
            try
            {
                string loginJson = Newtonsoft.Json.JsonConvert.SerializeObject(
                    new { username = this.config.Username, password = this.config.Password });
                int status;
                string respBody = HttpPost(this.config.TokenUrl, "application/json; charset=utf-8",
                    Encoding.UTF8.GetBytes(loginJson), null, out status);
                KskVlgApiResponse resp = ParseResponse(respBody);
                string token = (resp != null && resp.Data != null) ? resp.Data.AccessToken : null;

                if (status != HTTP_OK || resp == null || !resp.Success || string.IsNullOrWhiteSpace(token))
                {
                    this.lastAuthError = (status == 0)
                        ? "không kết nối được cổng (kiểm tra mạng / URL lấy token: " + this.config.TokenUrl + ")"
                        : DescribeError(status, resp);
                    Inventec.Common.Logging.LogSystem.Error("VLG login that bai. " + this.lastAuthError);
                    // Loi KHONG TU HET giua lo -> latch fail-fast cho cac ho so con lai (xem batchAuthFatalError).
                    if (status == 0 || status == HTTP_UNAUTHORIZED || status == 403 || status == HTTP_TOO_MANY_REQUESTS)
                        this.batchAuthFatalError = "đăng nhập cổng thất bại — " + this.lastAuthError;
                    ResetToken();
                    return null;
                }

                // Doi chieu ma don vi cua TOKEN voi MaDonVi cau hinh (= MACSKCB trong XML): lech ->
                // cong se tra 403 MACSKCB_MISMATCH cho MOI ho so — canh bao som de sua cau hinh.
                string maDonViToken = resp.Data.MaDonVi;
                if (!string.IsNullOrWhiteSpace(maDonViToken)
                    && !string.Equals(maDonViToken.Trim(), this.config.MaDonVi, StringComparison.OrdinalIgnoreCase))
                    Inventec.Common.Logging.LogSystem.Warn(string.Format(
                        "VLG: ma_don_vi cua token ({0}) KHAC MaDonVi cau hinh ({1}) -> MACSKCB trong XML se lech"
                        + " ma don vi token, cong se tra 403 MACSKCB_MISMATCH. Sua truong 1 cua"
                        + " MOS.HIS_KSK_SYNC.VLG_2062_CONNECTION_INFO cho khop tai khoan.",
                        maDonViToken, this.config.MaDonVi));

                long ttl = (resp.Data.ExpiresIn > 0) ? resp.Data.ExpiresIn : TOKEN_TTL_DEFAULT_SECONDS;
                ttl = Math.Max(TOKEN_TTL_MIN_SECONDS, ttl - TOKEN_TTL_SAFETY_SECONDS);
                this.cachedToken = token;
                this.tokenExpireAt = DateTime.Now.AddSeconds(ttl);
                return this.cachedToken;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                this.lastAuthError = ex.Message;
                ResetToken();
                return null;
            }
        }

        private void ResetToken()
        {
            this.cachedToken = null;
            this.tokenExpireAt = DateTime.MinValue;
        }

        /// <summary>
        /// POST body raw len URL (HttpWebRequest — .NET 4.5, khong can them reference). Tra body response
        /// (ke ca khi HTTP loi 4xx/5xx — cong tra JSON mo ta loi trong body); statusCode = HTTP status
        /// (0 = khong ket noi duoc / khong co response).
        /// </summary>
        private static string HttpPost(string url, string contentType, byte[] body, string bearerToken, out int statusCode)
        {
            statusCode = 0;
            var request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = contentType;
            request.Accept = "application/json";
            request.Timeout = HTTP_TIMEOUT_MS;
            request.ReadWriteTimeout = HTTP_TIMEOUT_MS;
            if (!string.IsNullOrEmpty(bearerToken))
                request.Headers.Add("Authorization", "Bearer " + bearerToken);
            request.ContentLength = (body != null) ? body.Length : 0;
            try
            {
                if (body != null && body.Length > 0)
                    using (var stream = request.GetRequestStream())
                        stream.Write(body, 0, body.Length);
                using (var response = (System.Net.HttpWebResponse)request.GetResponse())
                {
                    statusCode = (int)response.StatusCode;
                    return ReadBody(response);
                }
            }
            catch (System.Net.WebException wex)
            {
                // 4xx/5xx roi vao day — van doc body de lay code/message loi cua cong.
                var errResponse = wex.Response as System.Net.HttpWebResponse;
                if (errResponse == null)
                {
                    Inventec.Common.Logging.LogSystem.Error("VLG: khong ket noi duoc " + url + " — " + wex.Message, wex);
                    return null;
                }
                using (errResponse)
                {
                    statusCode = (int)errResponse.StatusCode;
                    return ReadBody(errResponse);
                }
            }
        }

        private static string ReadBody(System.Net.HttpWebResponse response)
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

        /// <summary>Parse JSON response chung cua cong. Body rong / khong phai JSON -> null.</summary>
        private static KskVlgApiResponse ParseResponse(string body)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(body)) return null;
                return Newtonsoft.Json.JsonConvert.DeserializeObject<KskVlgApiResponse>(body);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn("VLG: response khong phai JSON hop le: " + Cut(body, 500)
                    + " — " + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Dong goi body JSON wrapper theo tai lieu muc 5.1 dang (b):
        /// { data_type:"xml", data:&lt;chuoi XML&gt;, metadata:{ ma_yeu_cau, treatment_code, sender_id,
        ///   receiver_id, msg_id, msg_type, txn_type, send_datetime } }.
        /// Truong metadata nao khong co gia tri thi BO QUA (tat ca deu optional theo tai lieu).
        /// </summary>
        private byte[] BuildJsonBody(string xmlContent, string treatmentCode, out string maYeuCau)
        {
            maYeuCau = null;
            try
            {
                maYeuCau = BuildMaYeuCau(treatmentCode, xmlContent);
                var metadata = new Dictionary<string, object>();
                metadata["ma_yeu_cau"] = maYeuCau;
                if (!string.IsNullOrWhiteSpace(treatmentCode))
                    metadata["treatment_code"] = treatmentCode.Trim();   // ma dieu tri tran de tinh doi soat
                if (!string.IsNullOrWhiteSpace(this.metaSenderId)) metadata["sender_id"] = this.metaSenderId;
                if (!string.IsNullOrWhiteSpace(this.metaReceiverId)) metadata["receiver_id"] = this.metaReceiverId;
                metadata["msg_id"] = GenerateMsgId();
                if (!string.IsNullOrWhiteSpace(this.metaMsgType)) metadata["msg_type"] = this.metaMsgType;
                if (!string.IsNullOrWhiteSpace(this.metaTxnType)) metadata["txn_type"] = this.metaTxnType;
                metadata["send_datetime"] = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz");

                var bodyObj = new Dictionary<string, object>();
                bodyObj["data_type"] = "xml";
                bodyObj["data"] = xmlContent;
                bodyObj["metadata"] = metadata;
                return Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(bodyObj));
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); return null; }
        }

        /// <summary>
        /// ma_yeu_cau = "&lt;ma dieu tri&gt;-&lt;12 hex SHA256(noi dung)&gt;" — PHAI kem hash noi dung:
        /// cong khoa ma_yeu_cau voi noi dung (kiem chung cong dev 10/08/2026: cung ma + noi dung doi
        /// -> 409 REQUEST_ID_CONFLICT, TU CHOI xu ly). Kem hash: gui lai y nguyen -> ACCEPTED_DUPLICATE
        /// (khong tao lan xu ly moi); ho so sua roi day lai -> hash doi -> ma moi, khong bao gio 409.
        /// </summary>
        private static string BuildMaYeuCau(string treatmentCode, string content)
        {
            string prefix = string.IsNullOrWhiteSpace(treatmentCode) ? "KSK" : treatmentCode.Trim();
            return prefix + "-" + Sha256Hex12(content);
        }

        private static string Sha256Hex12(string s)
        {
            try
            {
                using (var sha = System.Security.Cryptography.SHA256.Create())
                {
                    byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(s ?? ""));
                    var sb = new StringBuilder();
                    for (int i = 0; i < 6; i++) sb.Append(hash[i].ToString("X2"));
                    return sb.ToString();
                }
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); return Guid.NewGuid().ToString("N").Substring(0, 12); }
        }

        /// <summary>msg_id = sender_id + yyMMdd + UUIDv4 (bo gach) — duy nhat moi lan gui (nhu cac cong truc BYT).</summary>
        private string GenerateMsgId()
        {
            string prefix = !string.IsNullOrWhiteSpace(this.metaSenderId)
                ? this.metaSenderId.Trim()
                : ((this.config != null && !string.IsNullOrWhiteSpace(this.config.MaDonVi)) ? this.config.MaDonVi.Trim() : "");
            return prefix + DateTime.Now.ToString("yyMMdd", System.Globalization.CultureInfo.InvariantCulture)
                 + Guid.NewGuid().ToString("N");
        }

        /// <summary>
        /// Luu y khi cong TIEP NHAN THANH CONG nhung co canh bao: code khac ACCEPTED (vd
        /// ACCEPTED_WITH_WARNING / ACCEPTED_DUPLICATE) va/hoac warnings[] co noi dung.
        /// Tra null khi tiep nhan "sach" (ACCEPTED, khong warnings).
        /// </summary>
        private static string BuildSuccessWarning(KskVlgApiResponse resp)
        {
            try
            {
                if (resp == null) return null;
                var parts = new List<string>();
                if (!string.IsNullOrEmpty(resp.Code)
                    && !string.Equals(resp.Code, "ACCEPTED", StringComparison.OrdinalIgnoreCase))
                    parts.Add(resp.Code);
                string warns = FormatErrors(resp.Warnings);
                if (!string.IsNullOrEmpty(warns)) parts.Add(warns);
                return (parts.Count > 0) ? string.Join(" — ", parts.ToArray()) : null;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); return null; }
        }

        /// <summary>
        /// Mo ta loi ngan gon tu HTTP status + response: "HTTP 400 MISSING_MACSKCB — <message>; <errors>".
        /// errors[] cua cong la mang object {severity, code, message...} hoac chuoi — ghep toi da 3 muc.
        /// </summary>
        private static string DescribeError(int status, KskVlgApiResponse resp)
        {
            var sb = new StringBuilder();
            sb.Append("HTTP ").Append(status > 0 ? status.ToString() : "(không kết nối được)");
            if (resp != null)
            {
                if (!string.IsNullOrEmpty(resp.Code)) sb.Append(" ").Append(resp.Code);
                if (!string.IsNullOrEmpty(resp.Message)) sb.Append(" — ").Append(resp.Message);
                string errs = FormatErrors(resp.Errors);
                if (!string.IsNullOrEmpty(errs)) sb.Append("; ").Append(errs);
            }
            return sb.ToString();
        }

        private static string FormatErrors(Newtonsoft.Json.Linq.JToken errors)
        {
            try
            {
                var arr = errors as Newtonsoft.Json.Linq.JArray;
                if (arr == null || arr.Count == 0) return null;
                var parts = new List<string>();
                foreach (var item in arr)
                {
                    if (parts.Count >= 3) { parts.Add("..."); break; }
                    var obj = item as Newtonsoft.Json.Linq.JObject;
                    if (obj != null)
                    {
                        string code = (string)obj["code"];
                        string msg = (string)obj["message"];
                        parts.Add((code ?? "") + (string.IsNullOrEmpty(msg) ? "" : (": " + msg)));
                    }
                    else
                        parts.Add(item.ToString());
                }
                return string.Join(" | ", parts.ToArray());
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); return null; }
        }

        private static string Cut(string s, int max)
        {
            if (string.IsNullOrEmpty(s)) return "(rong)";
            return (s.Length <= max) ? s : s.Substring(0, max) + "...";
        }
    }
}
