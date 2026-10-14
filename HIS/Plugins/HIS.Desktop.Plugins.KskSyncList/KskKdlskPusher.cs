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
using His.Ksk.QD2062.Sign;

namespace HIS.Desktop.Plugins.KskSyncList
{
    /// <summary>
    /// Cau hinh cong Kho du lieu suc khoe toan dan (https://api.hososuckhoe.com.vn), khoa
    /// <c>MOS.HIS_KSK_SYNC.KDLSK_2062_CONNECTION_INFO</c> (doc theo chi nhanh). Vien nao KHONG khai khoa nay thi
    /// cong Kho khong hien / khong day — khong anh huong vien khac. Dinh dang (cac truong cach '|'):
    /// <code>
    /// Username|Password|SenderId|PrivateKey|TokenUrl|PushUrl|DataType|Version|ReceiverId
    /// </code>
    /// <list type="bullet">
    /// <item>Username/Password: tai khoan Kho do So Y te cap.</item>
    /// <item>SenderId: ma dinh danh co so 13 so do Bo Y te cap (header.sender_id, tien to msg_id, MACSKCB, MA_GTIN_CSKCB).</item>
    /// <item>PrivateKey (TUY CHON): khoa bi mat PEM (1 dong) Bo Y te cap — ky "signature" (nha cung cap Kho: "dung sender
    /// id va private-key cua BYT"); bo trong -> signature rong. KHONG doc khoa cong BYT MOS.HIS_KSK_SYNC.CONNECTION_INFO.</item>
    /// <item>TokenUrl/PushUrl/DataType/Version/ReceiverId: bo trong -> mac dinh theo Phu luc huong dan cua So Y te.</item>
    /// </list>
    /// </summary>
    internal class KskKdlskConfig
    {
        internal string Username { get; set; }
        internal string Password { get; set; }
        internal string SenderId { get; set; }
        internal string PrivateKey { get; set; }
        internal string TokenUrl { get; set; }
        internal string PushUrl { get; set; }
        internal string DataType { get; set; }
        internal string Version { get; set; }
        internal string ReceiverId { get; set; }

        internal bool IsJson()
        {
            return string.Equals(this.DataType, KskKdlskConfigParser.DATA_TYPE_JSON, StringComparison.OrdinalIgnoreCase);
        }
    }

    internal static class KskKdlskConfigParser
    {
        internal const string CONFIG_KEY = "MOS.HIS_KSK_SYNC.KDLSK_2062_CONNECTION_INFO";
        internal const string DEFAULT_TOKEN_URL = "https://api.hososuckhoe.com.vn/api/v1/resource/authentication/login";
        internal const string DEFAULT_PUSH_URL = "https://api.hososuckhoe.com.vn/api/v1/medical-record/ksk-lien-thong/kham-suc-khoe";
        internal const string DATA_TYPE_XML = "xml/base64";
        internal const string DATA_TYPE_JSON = "json/base64";
        internal const string DEFAULT_VERSION = "1.0";
        internal const string DEFAULT_RECEIVER_ID = "VTS";

        /// <summary>
        /// Tach chuoi cau hinh. Chuoi rong -> null (chua cau hinh). Thieu truong bat buoc VAN tra doi tuong:
        /// KskKdlskPusher.ValidateConfig noi ro truong nao thieu thay vi bao chung "sai dinh dang".
        /// </summary>
        internal static KskKdlskConfig Parse(string configValue)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(configValue)) return null;
                string[] f = configValue.Split('|');
                KskKdlskConfig cfg = new KskKdlskConfig();
                cfg.Username = Get(f, 0);
                cfg.Password = Get(f, 1);
                cfg.SenderId = Get(f, 2);
                cfg.PrivateKey = Get(f, 3);
                cfg.TokenUrl = Or(Get(f, 4), DEFAULT_TOKEN_URL);
                cfg.PushUrl = Or(Get(f, 5), DEFAULT_PUSH_URL);
                string dataType = Get(f, 6);
                if (string.IsNullOrWhiteSpace(dataType))
                    cfg.DataType = DATA_TYPE_XML;
                else if (string.Equals(dataType, DATA_TYPE_JSON, StringComparison.OrdinalIgnoreCase))
                    cfg.DataType = DATA_TYPE_JSON;
                else
                {
                    if (!string.Equals(dataType, DATA_TYPE_XML, StringComparison.OrdinalIgnoreCase))
                        Inventec.Common.Logging.LogSystem.Warn("KDLSK: kieu du lieu '" + dataType + "' khong hop le (chi nhan "
                            + DATA_TYPE_XML + " / " + DATA_TYPE_JSON + ") -> dung " + DATA_TYPE_XML + ".");
                    cfg.DataType = DATA_TYPE_XML;
                }
                cfg.Version = Or(Get(f, 7), DEFAULT_VERSION);
                cfg.ReceiverId = Or(Get(f, 8), DEFAULT_RECEIVER_ID);
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

        private static string Or(string value, string fallback)
        {
            return string.IsNullOrWhiteSpace(value) ? fallback : value;
        }
    }

    /// <summary>Ket qua day 1 ho so len Kho (chuan hoa de gop voi ket qua cac cong khac).</summary>
    internal class KskKdlskPushResult
    {
        // Trang thai luu vao REGISTRATION_NO (doan "KDLSK:").
        internal const string STATE_ACCEPTED = "DA_TIEP_NHAN";      // Kho tra status 200 — moi la "da tiep nhan cho xu ly"
        internal const string STATE_REJECTED = "KHONG_TIEP_NHAN";   // Kho tra loi (status != 200)
        internal const string STATE_NOT_SENT = "CHUA_GUI";          // chan truoc khi gui (cau hinh / du lieu / ky so / dang nhap)
        internal const string STATE_UNKNOWN = "CHUA_RO";            // da gui nhung khong doc duoc phan hoi

        internal bool Success { get; set; }
        internal string Message { get; set; }
        internal string TxnCode { get; set; }   // maGiaoDich
        internal string State { get; set; }

        internal static KskKdlskPushResult NotSent(string message)
        {
            return new KskKdlskPushResult { Success = false, Message = message, State = STATE_NOT_SENT };
        }
    }

    /// <summary>
    /// Day ban tin KSK (QD 2062) len Kho du lieu suc khoe toan dan theo Phu luc "Huong dan ket noi lien thong
    /// du lieu KSK toan dan" cua So Y te:
    ///   1. POST {TokenUrl} application/json {username, password} -> { code, access_token }.
    ///   2. POST {PushUrl} application/json, Authorization: Bearer
    ///      body = { header{version, sender_id, receiver_id "VTS", txn_type "sync_checkup", msg_id, msg_type "101",
    ///      data_type, send_datetime(ms)}, data{ content = base64(file XML/JSON) }, signature }
    ///      -> { maGiaoDich, message, status }. status 200 CHI la "da tiep nhan cho xu ly"; hop le hay khong
    ///      tra cuu tren web hososuckhoe.com.vn theo maGiaoDich.
    ///   3. signature = Base64(RSA-SHA256( UPPER(SHA256(header)) + "." + UPPER(SHA256(file XML)) )) — khac truc BYT
    ///      o phan B: bam FILE XML (dung chuoi byte dem base64 vao data.content), KHONG bam chuoi base64.
    /// Token cache TINH theo (TokenUrl, Username) suot thoi gian chuong trinh mo — Phu luc yeu cau chi xin lai khi
    /// het han; khong co thoi han trong phan hoi -> doc "exp" neu token la JWT, khong thi dung den khi gap 401.
    /// </summary>
    internal class KskKdlskPusher
    {
        private const string TXN_TYPE = "sync_checkup";
        private const string MSG_TYPE = "101";
        private const int HTTP_TIMEOUT_MS = 60000;
        private const int HTTP_UNAUTHORIZED = 401;
        private const int CODE_OK = 200;
        private const int MAX_ATTEMPT = 2;                  // 401 -> dang nhap lai va gui lai 1 lan
        private const int TOKEN_EXPIRE_SAFETY_SECONDS = 60;
        private const string LOG_PREFIX = "KDLSK: ";

        private static readonly object TOKEN_LOCK = new object();
        private static readonly Dictionary<string, TokenEntry> TOKEN_CACHE = new Dictionary<string, TokenEntry>();

        private sealed class TokenEntry
        {
            internal string Token;
            internal DateTime ExpireAt;
        }

        private readonly KskKdlskConfig config;
        private readonly EnvelopeSigner signer = new EnvelopeSigner();
        private string privateKeyPkcs8;
        private bool configChecked;
        private string configError;
        // Loi dang nhap KHONG tu het (mat ket noi / sai tai khoan / bi chan IP) -> cac ho so con lai cua lo bao
        // that bai ngay, khong goi mang (Phu luc: khong request token lien tuc).
        private string batchAuthFatalError;

        /// <summary>Plugin .NET 4.5 khong bat TLS 1.2 mac dinh.</summary>
        static KskKdlskPusher()
        {
            try
            {
                System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        internal KskKdlskPusher(KskKdlskConfig config)
        {
            this.config = config;
        }

        internal KskKdlskConfig Config { get { return this.config; } }

        /// <summary>
        /// Kiem tra cau hinh du de gui (kiem 1 lan / lo): du 4 truong bat buoc, ma don vi 13 chu so, khoa bi mat
        /// ky duoc. Tra null neu hop le, nguoc lai thong bao noi ro truong sai.
        /// </summary>
        internal string ValidateConfig()
        {
            if (this.configChecked) return this.configError;
            this.configChecked = true;
            this.configError = CheckConfig();
            if (this.configError != null)
                Inventec.Common.Logging.LogSystem.Warn(this.configError);
            return this.configError;
        }

        private string CheckConfig()
        {
            if (this.config == null)
                return "KDLSK: chuỗi cấu hình sai định dạng (" + KskKdlskConfigParser.CONFIG_KEY + ").";

            var missing = new List<string>();
            if (string.IsNullOrWhiteSpace(this.config.Username)) missing.Add("tài khoản (trường 1)");
            if (string.IsNullOrWhiteSpace(this.config.Password)) missing.Add("mật khẩu (trường 2)");
            if (string.IsNullOrWhiteSpace(this.config.SenderId)) missing.Add("mã đơn vị gửi (trường 3)");
            if (missing.Count > 0)
                return "KDLSK: chuỗi cấu hình sai định dạng — thiếu " + string.Join(", ", missing.ToArray())
                     + " (" + KskKdlskConfigParser.CONFIG_KEY + ").";

            if (!IsGtin13(this.config.SenderId))
                return "KDLSK: Mã đơn vị gửi '" + this.config.SenderId + "' phải là mã 13 chữ số do Bộ Y tế cấp"
                     + " (trường 3 khóa " + KskKdlskConfigParser.CONFIG_KEY + ").";

            // Khoa bi mat TUY CHON (chua xac nhan Kho co kiem chu ky): bo trong -> gui signature rong.
            if (string.IsNullOrWhiteSpace(this.config.PrivateKey))
            {
                Inventec.Common.Logging.LogSystem.Warn(LOG_PREFIX + "chua khai khoa bi mat (truong 4) -> gui signature RONG."
                    + " Kho bao sai/thieu chu ky thi khai khoa bi mat Bo Y te cap.");
                return null;
            }

            // Khoa PKCS#1 (BEGIN RSA PRIVATE KEY) -> PKCS#8 cho thu vien; ky thu 1 lan de chac khoa doc duoc.
            this.privateKeyPkcs8 = KskPemUtil.EnsurePkcs8(this.config.PrivateKey);
            string probe = this.signer.Sign("{}", "KDLSK", this.privateKeyPkcs8);
            if (string.IsNullOrEmpty(probe))
                return "KDLSK: Khóa bí mật không đúng định dạng PEM, không ký được (trường 4 khóa "
                     + KskKdlskConfigParser.CONFIG_KEY + "; định dạng đang khai: "
                     + KskPemUtil.DescribeKey(this.config.PrivateKey) + ").";
            return null;
        }

        internal static bool IsGtin13(string value)
        {
            string v = (value ?? "").Trim();
            if (v.Length != 13) return false;
            foreach (char c in v)
                if (c < '0' || c > '9') return false;
            return true;
        }

        /// <summary>
        /// Gui 1 ho so. fileContent = noi dung file XML (hoac JSON) KHAMSUCKHOE da ky CKS (neu co). Chuoi byte UTF-8
        /// cua fileContent.Trim() vua la phan dem bam (B) vua la phan dem base64 vao data.content.
        /// </summary>
        internal KskKdlskPushResult Push(string fileContent)
        {
            try
            {
                string cfgError = ValidateConfig();
                if (cfgError != null) return KskKdlskPushResult.NotSent(cfgError);
                if (this.batchAuthFatalError != null)
                    return KskKdlskPushResult.NotSent("KDLSK: " + this.batchAuthFatalError
                        + " (hồ sơ này CHƯA gửi — bỏ qua phần còn lại của lô).");
                if (string.IsNullOrWhiteSpace(fileContent))
                    return KskKdlskPushResult.NotSent("KDLSK: không dựng được bản tin hồ sơ.");

                string data = fileContent.Trim();
                string contentBase64 = Convert.ToBase64String(new UTF8Encoding(false).GetBytes(data));

                for (int attempt = 0; attempt < MAX_ATTEMPT; attempt++)
                {
                    string authError;
                    string token = GetToken(out authError);
                    if (string.IsNullOrWhiteSpace(token))
                        return KskKdlskPushResult.NotSent("KDLSK: đăng nhập thất bại — " + authError);

                    string msgId = GenerateMsgId(this.config.SenderId);
                    Newtonsoft.Json.Linq.JObject header = BuildHeader(msgId);
                    string headerJson = header.ToString(Newtonsoft.Json.Formatting.None);
                    string signature = "";
                    if (this.privateKeyPkcs8 != null)
                    {
                        signature = this.signer.Sign(headerJson, data, this.privateKeyPkcs8);
                        if (string.IsNullOrEmpty(signature))
                            return KskKdlskPushResult.NotSent("KDLSK: ký thông điệp (signature) thất bại — kiểm tra Khóa bí mật"
                                + " (trường 4 khóa " + KskKdlskConfigParser.CONFIG_KEY + ").");
                    }

                    var dataObj = new Newtonsoft.Json.Linq.JObject();
                    dataObj["content"] = contentBase64;
                    var envelope = new Newtonsoft.Json.Linq.JObject();
                    envelope["header"] = header;
                    envelope["data"] = dataObj;
                    envelope["signature"] = signature;
                    string body = envelope.ToString(Newtonsoft.Json.Formatting.None);
                    // Header trong body PHAI trung khit chuoi da bam, neu khong Kho tinh lai A se khac.
                    if (body.IndexOf(headerJson, StringComparison.Ordinal) < 0)
                        Inventec.Common.Logging.LogSystem.Warn(LOG_PREFIX + "chuoi header trong body KHAC chuoi da ky — Kho co the bao sai chu ky.");

                    int status;
                    bool sent;
                    string respBody = HttpPost(this.config.PushUrl, Encoding.UTF8.GetBytes(body), token, out status, out sent);
                    Inventec.Common.Logging.LogSystem.Info(string.Format(
                        LOG_PREFIX + "gui ho so msg_id={0} -> HTTP {1}; phan hoi: {2}", msgId, status, Cut(respBody, 500)));

                    Newtonsoft.Json.Linq.JObject jo = TryParseJObject(respBody);
                    if (attempt + 1 < MAX_ATTEMPT && IsAuthExpired(status, jo))
                    {
                        Inventec.Common.Logging.LogSystem.Warn(LOG_PREFIX + "ma phien het han/khong hop le (401) -> dang nhap lai va gui lai.");
                        ResetToken();
                        continue;
                    }
                    return BuildPushResult(status, sent, jo, respBody);
                }
                return KskKdlskPushResult.NotSent("KDLSK: xác thực thất bại (mã phiên hết hạn sau khi đăng nhập lại).");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return KskKdlskPushResult.NotSent("KDLSK: " + ex.Message);
            }
        }

        private Newtonsoft.Json.Linq.JObject BuildHeader(string msgId)
        {
            // Thu tu truong theo mau Phu luc muc 2.1.
            var header = new Newtonsoft.Json.Linq.JObject();
            header["version"] = this.config.Version;
            header["sender_id"] = this.config.SenderId.Trim();
            header["receiver_id"] = this.config.ReceiverId;
            header["txn_type"] = TXN_TYPE;
            header["msg_id"] = msgId;
            header["msg_type"] = MSG_TYPE;
            header["data_type"] = this.config.DataType;
            header["send_datetime"] = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
            return header;
        }

        private static KskKdlskPushResult BuildPushResult(int status, bool sent, Newtonsoft.Json.Linq.JObject jo, string respBody)
        {
            var r = new KskKdlskPushResult();
            if (status == 0)
            {
                r.State = sent ? KskKdlskPushResult.STATE_UNKNOWN : KskKdlskPushResult.STATE_NOT_SENT;
                r.Message = sent
                    ? "KDLSK: đã gửi nhưng không nhận được phản hồi của Kho — tra cứu trên hososuckhoe.com.vn trước khi gửi lại."
                    : "KDLSK: không kết nối được Kho (kiểm tra mạng / URL gửi hồ sơ / IP đã đăng ký với Sở).";
                return r;
            }

            // Kho tra 2 kieu phan hoi: kieu Phu luc {maGiaoDich, message, status} va kieu truc Bo Y te
            // {txn_id, res_code (CM_SUCCESS), res_msg, data{data_state, data_state_msg}} — cong that tra kieu sau
            // (14/10/2026: HTTP 200, res_code CM_SUCCESS, "Successfully inserted data to DATA LAKE", signature rong).
            r.TxnCode = EmptyToNull(ReadString(jo, "maGiaoDich")) ?? EmptyToNull(ReadString(jo, "txn_id"));
            string resCode = EmptyToNull(ReadString(jo, "res_code"));
            int code = ReadInt(jo, "status", "code");
            bool http2xx = status >= 200 && status < 300;
            r.Success = http2xx && (code == CODE_OK
                || (resCode != null && resCode.EndsWith("SUCCESS", StringComparison.OrdinalIgnoreCase)));
            if (r.Success)
            {
                r.State = KskKdlskPushResult.STATE_ACCEPTED;
                return r;
            }
            if (http2xx && jo == null)
            {
                r.State = KskKdlskPushResult.STATE_UNKNOWN;
                r.Message = "KDLSK: không đọc được phản hồi của Kho (HTTP " + status + ") — tra cứu trên hososuckhoe.com.vn trước khi gửi lại.";
                return r;
            }
            r.State = KskKdlskPushResult.STATE_REJECTED;
            // Ly do lay tu TRUONG thong bao cua Kho (khong lay ca body: body luon co khoa "signature" -> goi y sai).
            string message = EmptyToNull(ReadString(jo, "res_msg")) ?? EmptyToNull(ReadString(jo, "message"))
                ?? EmptyToNull(ReadString(jo, "data_state_msg"));
            string detail = message ?? Cut(respBody, 200);
            r.Message = "KDLSK: Kho từ chối (HTTP " + status
                + (resCode != null ? ", " + resCode : (code != 0 ? ", status " + code : "")) + ")"
                + (string.IsNullOrEmpty(detail) ? "" : ": " + detail);
            if (message != null
                && (message.IndexOf("signature", StringComparison.OrdinalIgnoreCase) >= 0
                    || message.IndexOf("chữ ký", StringComparison.OrdinalIgnoreCase) >= 0))
                r.Message += " — kiểm tra Mã đơn vị gửi / Khóa bí mật trong khóa " + KskKdlskConfigParser.CONFIG_KEY + ".";
            return r;
        }

        /// <summary>Ma phien het han / khong hop le: HTTP 401, status/code 401 hoac res_code CM_AUTH_* (kieu truc BYT).</summary>
        private static bool IsAuthExpired(int status, Newtonsoft.Json.Linq.JObject jo)
        {
            if (status == HTTP_UNAUTHORIZED || ReadInt(jo, "status", "code") == HTTP_UNAUTHORIZED) return true;
            string resCode = ReadString(jo, "res_code");
            return string.Equals(resCode, "CM_AUTH_PERMISSION", StringComparison.OrdinalIgnoreCase)
                || string.Equals(resCode, "CM_AUTH_EXPIRED", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>msg_id = sender_id + yyMMdd + UUID bo gach (dung mau Phu luc).</summary>
        private static string GenerateMsgId(string senderId)
        {
            return (senderId ?? "").Trim()
                 + DateTime.Now.ToString("yyMMdd", System.Globalization.CultureInfo.InvariantCulture)
                 + Guid.NewGuid().ToString("N");
        }

        #region token
        private string TokenCacheKey()
        {
            return (this.config.TokenUrl ?? "") + "\u0001" + (this.config.Username ?? "") + "\u0001" + (this.config.Password ?? "");
        }

        private void ResetToken()
        {
            lock (TOKEN_LOCK) { TOKEN_CACHE.Remove(TokenCacheKey()); }
        }

        private string GetToken(out string authError)
        {
            authError = null;
            string key = TokenCacheKey();
            lock (TOKEN_LOCK)
            {
                TokenEntry cached;
                if (TOKEN_CACHE.TryGetValue(key, out cached) && cached != null && DateTime.Now < cached.ExpireAt)
                    return cached.Token;
            }

            try
            {
                string loginJson = Newtonsoft.Json.JsonConvert.SerializeObject(
                    new { username = this.config.Username, password = this.config.Password });
                int status;
                bool sent;
                string respBody = HttpPost(this.config.TokenUrl, Encoding.UTF8.GetBytes(loginJson), null, out status, out sent);
                Newtonsoft.Json.Linq.JObject jo = TryParseJObject(respBody);
                string token = EmptyToNull(ReadString(jo, "access_token"));
                int code = ReadInt(jo, "code", "status");
                bool ok = status >= 200 && status < 300 && (code == CODE_OK || code == 0) && token != null;
                if (!ok)
                {
                    string message = ReadString(jo, "message");
                    authError = (status == 0)
                        ? "không kết nối được Kho (kiểm tra mạng / URL lấy token / IP đã đăng ký với Sở)"
                        : "HTTP " + status + (code != 0 ? ", code " + code : "")
                          + (string.IsNullOrWhiteSpace(message) ? "" : ": " + message.Trim())
                          + " (kiểm tra tài khoản / IP đã đăng ký với Sở)";
                    Inventec.Common.Logging.LogSystem.Error(LOG_PREFIX + "dang nhap that bai. " + authError);
                    if (status == 0 || status == HTTP_UNAUTHORIZED || status == 403 || status == 429 || (status >= 200 && status < 300))
                        this.batchAuthFatalError = "đăng nhập Kho thất bại — " + authError;
                    return null;
                }

                DateTime expireAt = ReadJwtExpire(token) ?? DateTime.MaxValue;
                if (expireAt != DateTime.MaxValue) expireAt = expireAt.AddSeconds(-TOKEN_EXPIRE_SAFETY_SECONDS);
                lock (TOKEN_LOCK) { TOKEN_CACHE[key] = new TokenEntry { Token = token, ExpireAt = expireAt }; }
                Inventec.Common.Logging.LogSystem.Info(LOG_PREFIX + "dang nhap thanh cong; ma phien dung den "
                    + (expireAt == DateTime.MaxValue ? "khi Kho bao het han (401)" : expireAt.ToString("yyyy-MM-dd HH:mm:ss")) + ".");
                return token;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                authError = ex.Message;
                return null;
            }
        }

        /// <summary>Doc "exp" (giay unix) trong payload neu token la JWT; khong phai JWT / khong co -> null.</summary>
        private static DateTime? ReadJwtExpire(string token)
        {
            try
            {
                string[] parts = (token ?? "").Split('.');
                if (parts.Length != 3) return null;
                string p = parts[1].Replace('-', '+').Replace('_', '/');
                switch (p.Length % 4) { case 2: p += "=="; break; case 3: p += "="; break; }
                var payload = Newtonsoft.Json.Linq.JObject.Parse(Encoding.UTF8.GetString(Convert.FromBase64String(p)));
                var exp = payload["exp"];
                if (exp == null) return null;
                long seconds = (long)exp;
                return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(seconds).ToLocalTime();
            }
            catch { return null; }
        }
        #endregion

        #region http / json
        /// <summary>
        /// POST JSON (HttpWebRequest — .NET 4.5). Tra body phan hoi (ca khi 4xx/5xx); statusCode = 0 khi khong co
        /// phan hoi HTTP; sent = da ghi xong body len mang (de phan biet "chua gui" voi "mat phan hoi").
        /// </summary>
        private static string HttpPost(string url, byte[] body, string bearerToken, out int statusCode, out bool sent)
        {
            statusCode = 0;
            sent = false;
            var request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/json; charset=utf-8";
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
                sent = true;
                using (var response = (System.Net.HttpWebResponse)request.GetResponse())
                {
                    statusCode = (int)response.StatusCode;
                    return ReadBody(response);
                }
            }
            catch (System.Net.WebException wex)
            {
                var errResponse = wex.Response as System.Net.HttpWebResponse;
                if (errResponse == null)
                {
                    Inventec.Common.Logging.LogSystem.Error(LOG_PREFIX + "khong ket noi duoc " + url + " — " + wex.Message, wex);
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
            using (var stream = response.GetResponseStream())
            {
                if (stream == null) return null;
                using (var reader = new System.IO.StreamReader(stream, Encoding.UTF8))
                    return reader.ReadToEnd();
            }
        }

        private static Newtonsoft.Json.Linq.JObject TryParseJObject(string body)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(body)) return null;
                string t = body.TrimStart();
                if (!t.StartsWith("{")) return null;
                return Newtonsoft.Json.Linq.JObject.Parse(t);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(LOG_PREFIX + "phan hoi khong phai JSON hop le: " + Cut(body, 300) + " — " + ex.Message);
                return null;
            }
        }

        /// <summary>Doc truong o goc, khong co thi tim trong "data" (Kho co the boc ket qua trong data).</summary>
        private static Newtonsoft.Json.Linq.JToken Find(Newtonsoft.Json.Linq.JObject jo, string name)
        {
            if (jo == null) return null;
            var v = jo.GetValue(name, StringComparison.OrdinalIgnoreCase);
            if (v != null && v.Type != Newtonsoft.Json.Linq.JTokenType.Null) return v;
            var data = jo.GetValue("data", StringComparison.OrdinalIgnoreCase) as Newtonsoft.Json.Linq.JObject;
            if (data == null) return null;
            v = data.GetValue(name, StringComparison.OrdinalIgnoreCase);
            return (v != null && v.Type != Newtonsoft.Json.Linq.JTokenType.Null) ? v : null;
        }

        private static string ReadString(Newtonsoft.Json.Linq.JObject jo, string name)
        {
            var v = Find(jo, name);
            return (v != null) ? v.ToString() : null;
        }

        /// <summary>Doc so nguyen (chap nhan ca so dang chuoi) theo ten dau tien co mat; khong co -> 0.</summary>
        private static int ReadInt(Newtonsoft.Json.Linq.JObject jo, params string[] names)
        {
            foreach (string name in names)
            {
                var v = Find(jo, name);
                if (v == null) continue;
                int n;
                if (int.TryParse(v.ToString().Trim(), out n)) return n;
            }
            return 0;
        }

        private static string EmptyToNull(string s)
        {
            return string.IsNullOrWhiteSpace(s) ? null : s.Trim();
        }

        private static string Cut(string s, int max)
        {
            if (string.IsNullOrEmpty(s)) return s;
            return (s.Length <= max) ? s : s.Substring(0, max) + "…";
        }
        #endregion
    }
}
