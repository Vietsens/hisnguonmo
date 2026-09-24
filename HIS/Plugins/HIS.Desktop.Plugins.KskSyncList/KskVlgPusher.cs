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
    /// MaDonVi|Username|Password|TokenUrl|PushUrl|SenderGtin
    /// </code>
    /// <list type="bullet">
    /// <item>MaDonVi: ma don vi 5 so do tinh cap (vd 83009) — doi chieu voi ma_don_vi cua token.</item>
    /// <item>Username/Password: tai khoan tich hop do tinh cap (POST /api/xac-thuc/token).</item>
    /// <item>TokenUrl/PushUrl: URL DAY DU; bo trong -> URL cong chinh thuc Vinh Long
    /// (moi truong dev: doi TokenUrl sang https://dev-congtiepnhan.kdlyt.vinhlong.vn/api/xac-thuc/token).
    /// PushUrl cu tro /api/kham-suc-khoe/qd-2062/tiep-nhan (API V1.3 — cong da tat, tra 410) duoc TU
    /// chuyen sang /api/platform/data-sync/push (API V1.5) cung host voi TokenUrl.</item>
    /// <item>SenderGtin: ma dinh danh co so KCB 13 so (GTIN/GLN, QD 2062 Phu luc 02 muc 5.2) — dung cho
    /// header.sender_id, tien to msg_id, THONGTINDONVI/MACSKCB va MA_GTIN_CSKCB. Bo trong -> lay SenderId
    /// cua cong BYT/HSSK/HCC neu co.</item>
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
        internal string SenderGtin { get; set; }
    }

    internal static class KskVlgConfigParser
    {
        internal const string DEFAULT_BASE_URL = "https://congtiepnhan.kdlyt.vinhlong.vn";
        internal const string TOKEN_PATH = "/api/xac-thuc/token";
        internal const string PROXY_PUSH_PATH = "/api/platform/data-sync/push";
        internal const string DEFAULT_TOKEN_URL = DEFAULT_BASE_URL + TOKEN_PATH;
        internal const string DEFAULT_PUSH_URL = DEFAULT_BASE_URL + PROXY_PUSH_PATH;
        private const int MIN_FIELD_COUNT = 3;

        /// <summary>
        /// Goc host tu TokenUrl (cat duoi /api/xac-thuc/token). TokenUrl khac dang -> lay scheme://host[:port]
        /// cua chinh TokenUrl (KHONG roi ve cong chinh thuc: cau hinh dev viet la se day nham du lieu that).
        /// Chi khi TokenUrl rong / khong phai URL tuyet doi moi dung cong chinh thuc.
        /// Dung chung cho URL day (proxy) va URL tra cuu de moi truong dev/prod luon cung host.
        /// </summary>
        internal static string DeriveBaseUrl(string tokenUrl)
        {
            string t = (tokenUrl ?? "").Trim().TrimEnd('/');
            if (t.EndsWith(TOKEN_PATH, StringComparison.OrdinalIgnoreCase))
                return t.Substring(0, t.Length - TOKEN_PATH.Length);
            Uri u;
            if (!string.IsNullOrEmpty(t) && Uri.TryCreate(t, UriKind.Absolute, out u)
                && (u.Scheme == Uri.UriSchemeHttps || u.Scheme == Uri.UriSchemeHttp))
                return u.GetLeftPart(UriPartial.Authority);
            return DEFAULT_BASE_URL;
        }

        /// <summary>
        /// URL day KSK: PushUrl da tro /platform/data-sync/push -> giu nguyen; con lai (trong / API cu
        /// /tiep-nhan da bi tat / URL la) -> base(TokenUrl) + /api/platform/data-sync/push.
        /// </summary>
        internal static string ResolvePushUrl(string pushUrl, string tokenUrl)
        {
            string p = (pushUrl ?? "").Trim().TrimEnd('/');
            if (p.EndsWith(PROXY_PUSH_PATH, StringComparison.OrdinalIgnoreCase)) return p;
            return DeriveBaseUrl(tokenUrl) + PROXY_PUSH_PATH;
        }

        /// <summary>Ma GTIN/GLN hop le = dung 13 chu so.</summary>
        internal static bool IsGtin13(string s)
        {
            if (string.IsNullOrEmpty(s) || s.Length != 13) return false;
            foreach (char c in s) if (c < '0' || c > '9') return false;
            return true;
        }

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

                // API V1.3 (/tiep-nhan) da bi Kho tat (410 KSK_LEGACY_API_DISABLED tu 18/09/2026) ->
                // chuoi cau hinh cu van dung duoc, URL day tu nang len API V1.5 cung host voi TokenUrl.
                cfg.PushUrl = ResolvePushUrl(Get(f, 4), cfg.TokenUrl);

                cfg.SenderGtin = Get(f, 5);

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

    /// <summary>
    /// Ket qua TRA CUU 1 ho so tren cong Vinh Long (GET /api/kham-suc-khoe/qd-2062/ho-so/trang-thai).
    /// Ok = goi API thanh cong; Found = ho so ton tai tren cong; IsValid/IsInvalid theo
    /// ho_so.validation_status (VALID = Kho DAT kiem tra, INVALID = co loi — chi tiet o ErrorSummary).
    /// API V1.5: lan gui moi nhat (requests[] co received_at lon nhat) co them trang thai Kho
    /// (hoc_status/hoc_validation_status) va ket qua Cong Bo Y te (byt_status/byt_res_code).
    /// </summary>
    internal class KskVlgStatusResult
    {
        internal bool Ok { get; set; }
        internal bool Found { get; set; }
        internal string ValidationStatus { get; set; }
        internal string Message { get; set; }
        internal string ErrorSummary { get; set; }
        internal string FailReason { get; set; }
        internal string LatestSourceChannel { get; set; }   // BYT_PROXY / LEGACY_API / EXCEL_IMPORT
        internal string LatestHocStatus { get; set; }
        internal string LatestBytStatus { get; set; }       // vd PENDING, BYT_ACCEPTED
        internal string LatestBytResCode { get; set; }      // res_code Cong Bo tra ve (CM_SUCCESS...)
        internal string LatestTrackingId { get; set; }
        /// <summary>Trang thai TUNG lan gui theo msg_id (requests[]) — xet dung lan gui HIS quan tam, khong chi lan moi nhat.</summary>
        internal Dictionary<string, KskVlgRequestInfo> RequestsByMsgId { get; set; }

        /// <summary>
        /// Tim lan gui theo msg_id. Chap nhan ca msg_id RUT GON (chi phan UUID cuoi, khi TRANSACTION_CODE
        /// ghep nhieu cong phai cat cho vua cot 100 ky tu) — so khop hau to.
        /// </summary>
        internal KskVlgRequestInfo FindRequest(string msgIdOrTail)
        {
            if (string.IsNullOrEmpty(msgIdOrTail) || this.RequestsByMsgId == null) return null;
            KskVlgRequestInfo exact;
            if (this.RequestsByMsgId.TryGetValue(msgIdOrTail, out exact)) return exact;
            foreach (var kv in this.RequestsByMsgId)
                if (kv.Key.EndsWith(msgIdOrTail, StringComparison.OrdinalIgnoreCase)) return kv.Value;
            return null;
        }

        internal bool IsValid
        {
            get { return Found && string.Equals(ValidationStatus, "VALID", StringComparison.OrdinalIgnoreCase); }
        }
        internal bool IsInvalid
        {
            get { return Found && string.Equals(ValidationStatus, "INVALID", StringComparison.OrdinalIgnoreCase); }
        }
        /// <summary>Cong Bo da tiep nhan lan gui moi nhat.</summary>
        internal bool IsBytAccepted
        {
            get
            {
                return string.Equals(LatestBytStatus, "BYT_ACCEPTED", StringComparison.OrdinalIgnoreCase)
                    || KskVlgBytResCode.IsSuccess(LatestBytResCode);
            }
        }
        /// <summary>Cong Bo tu choi lan gui moi nhat (ban tin khong dat — can sua du lieu roi gui lai).</summary>
        internal bool IsBytRejected
        {
            get
            {
                return KskVlgBytResCode.IsRejected(LatestBytResCode)
                    || (!string.IsNullOrEmpty(LatestBytStatus)
                        && LatestBytStatus.IndexOf("REJECT", StringComparison.OrdinalIgnoreCase) >= 0);
            }
        }

        internal static KskVlgStatusResult Failure(string reason)
        {
            return new KskVlgStatusResult { Ok = false, FailReason = reason };
        }
    }

    /// <summary>
    /// Trang thai 1 lan gui (1 msg_id) tren Kho: requests[] cua /ho-so/trang-thai hoac data.item cua
    /// /doi-soat-byt/trang-thai (tai lieu V1.5 muc 5.4, 5.8.1).
    /// </summary>
    internal class KskVlgRequestInfo
    {
        internal string MsgId { get; set; }
        internal string TrackingId { get; set; }
        internal string SourceChannel { get; set; }
        internal string HocStatus { get; set; }
        internal string HocValidationStatus { get; set; }
        internal string BytStatus { get; set; }
        internal string BytResCode { get; set; }
        internal string BytResMsg { get; set; }

        internal bool IsHocInvalid
        {
            get { return string.Equals(HocValidationStatus, "INVALID", StringComparison.OrdinalIgnoreCase); }
        }
        internal bool IsBytAccepted
        {
            get
            {
                return string.Equals(BytStatus, "BYT_ACCEPTED", StringComparison.OrdinalIgnoreCase)
                    || KskVlgBytResCode.IsSuccess(BytResCode);
            }
        }
        internal bool IsBytRejected
        {
            get
            {
                return KskVlgBytResCode.IsRejected(BytResCode)
                    || (!string.IsNullOrEmpty(BytStatus) && BytStatus.IndexOf("REJECT", StringComparison.OrdinalIgnoreCase) >= 0);
            }
        }

        /// <summary>Doc 1 lan gui tu JSON (requests[i] / data.item). Tra null neu khong phai object.</summary>
        internal static KskVlgRequestInfo FromJson(Newtonsoft.Json.Linq.JToken r)
        {
            var o = r as Newtonsoft.Json.Linq.JObject;
            if (o == null) return null;
            return new KskVlgRequestInfo
            {
                MsgId = (string)o["msg_id"],
                TrackingId = (string)o["tracking_id"],
                SourceChannel = (string)o["source_channel"],
                HocStatus = (string)o["hoc_status"] ?? (string)o["status"],
                HocValidationStatus = (string)o["hoc_validation_status"] ?? (string)o["validation_status"],
                BytStatus = (string)o["byt_status"],
                BytResCode = (string)o["byt_res_code"] ?? (string)o["res_code"],
                BytResMsg = (string)o["byt_res_msg"] ?? (string)o["res_msg"]
            };
        }
    }

    /// <summary>Ket qua tra 1 ban tin theo sender_id + msg_id (API doi soat Bo, tai lieu V1.5 muc 5.8.1).</summary>
    internal class KskVlgMessageLookup
    {
        internal bool Ok { get; set; }          // tra cuu duoc (ke ca "khong co")
        internal bool Found { get; set; }       // Kho CO ban tin nay
        internal KskVlgRequestInfo Info { get; set; }
        internal string FailReason { get; set; }
    }

    /// <summary>
    /// Bang ma header.res_code cua API KSK tuong thich Cong Bo Y te (tai lieu V1.5 muc 5.1, QD 2062
    /// Phu luc 02 muc 5.2) va cac ma rieng cua Kho du lieu.
    /// </summary>
    internal static class KskVlgBytResCode
    {
        internal const string FORWARD_PAUSED = "HOC_BYT_FORWARD_PAUSED";   // 202 — Kho giu, chua chuyen Bo
        internal const string BYT_TIMEOUT = "HOC_BYT_TIMEOUT";             // 504 — Kho da ghi nhan, cho Bo
        internal const string SIGNING_UNAVAILABLE = "HOC_SIGNING_UNAVAILABLE";
        internal const string SAVE_FAIL = "PS_DS_SAVE_FAIL";               // Bo loi luu — Kho tu gui lai
        internal const string AUTH_ACCOUNT_FAIL = "CM_AUTH_ACCOUNT_FAIL";  // Kho chua dang nhap duoc Bo
        internal const string CHUA_RO = "VLG_CHUA_RO";                     // HIS mat phan hoi sau khi da gui

        private static readonly HashSet<string> SuccessCodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "CM_SUCCESS", "PS_SYNC_SUCCESS", "PS_DS_VERIFY_SUCCESS", "PS_DS_SAVE_SUCCESS"
        };
        private static readonly HashSet<string> RejectedCodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "CM_INVALID_REQUEST", "PS_DS_RSA_SIGNATURE_INVALID", "PS_DS_RSA_SIGNATURE_MISSING",
            "PS_DS_CA_SIGNATURE_INVALID", "PS_DS_CA_SIGNATURE_MISSING", "PS_DS_VERIFY_FAIL"
        };

        internal static bool IsSuccess(string code) { return !string.IsNullOrEmpty(code) && SuccessCodes.Contains(code); }
        internal static bool IsRejected(string code) { return !string.IsNullOrEmpty(code) && RejectedCodes.Contains(code); }
        internal static bool IsCaSignature(string code)
        {
            return !string.IsNullOrEmpty(code) && code.StartsWith("PS_DS_CA_SIGNATURE", StringComparison.OrdinalIgnoreCase);
        }
    }

    /// <summary>Ket qua day 1 ho so len cong Vinh Long (chuan hoa de gop voi ket qua cac cong khac).</summary>
    internal class KskVlgPushResult
    {
        internal bool Success { get; set; }
        internal string Message { get; set; }
        /// <summary>
        /// Ma tra cuu luu vao TRANSACTION_CODE: header phan hoi X-HOC-Tracking-Id; khong co thi
        /// "MSG:" + msg_id HIS da gui (tra cuu duoc theo sender_id + msg_id).
        /// </summary>
        internal string TrackingId { get; set; }
        /// <summary>Ma trang thai ngan luu vao REGISTRATION_NO: header.res_code (vd CM_SUCCESS, HOC_BYT_FORWARD_PAUSED) hoac VLG_CHUA_RO.</summary>
        internal string Status { get; set; }
        internal string MsgId { get; set; }
        /// <summary>Ghi chu hien tren dialog ket qua khi thanh cong (vd Kho giu ban tin, khong day lai).</summary>
        internal string Note { get; set; }
        // Luu y khi THANH CONG — hien len ket qua de nhan vien biet ho so co luu y can xu ly.
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
    /// "API Document CongTiepNhan" V1.5 (18/09/2026) muc 5.1. Cau hinh: MOS.HIS_KSK_SYNC.VLG_2062_CONNECTION_INFO.
    /// API V1.3 (/api/kham-suc-khoe/qd-2062/tiep-nhan) da bi Kho tat tren ca dev lan chinh thuc
    /// (410 KSK_LEGACY_API_DISABLED, kiem chung 24/09/2026) -> chi con giao thuc duoi day.
    ///
    ///   1. POST {TokenUrl} (/api/xac-thuc/token)  {username,password}
    ///      -> { success, data:{ access_token, token_type:"Bearer", expires_in, ma_don_vi } }
    ///   2. POST {PushUrl} (/api/platform/data-sync/push) — cau truc request cua truc Bo Y te (QD 2062
    ///      Phu luc 02 muc 5.2). Header: Authorization Bearer, Content-Type application/json, Accept *&#47;*,
    ///      Accept-Encoding gzip,deflate,br, service-type 100. Body:
    ///        { header:{ version "1.0.6", sender_id &lt;GTIN 13 so&gt;, receiver_id "TTYQG", txn_type
    ///          "sync_checkup", msg_id sender_id+yyMMdd+UUID, msg_type "101", data_type "xml/base64",
    ///          send_datetime unix ms }, data: base64(XML KHAMSUCKHOE), signature: "" }
    ///      HIS KHONG ky envelope (Kho ky bang khoa So); 2 chu ky CKS_ trong XML giu nguyen.
    ///   3. Phan hoi = envelope Bo: header.res_code/res_msg/txn_id, data.data_state_msg...; ma tra cuu
    ///      cua Kho nam o HTTP header X-HOC-Tracking-Id. Phan loai xem ClassifyProxyResponse.
    ///   4. Chong trung: Kho nhan dien lan gui bang sender_id + msg_id; gui lai NGUYEN raw body -> tra ket
    ///      qua cu. Vi vay body serialize MOT LAN, 401/503 gui lai dung mang byte do.
    ///   5. Gioi han body 10 MiB (sau base64) — kiem truoc khi POST.
    /// </summary>
    internal class KskVlgPusher
    {
        private const long TOKEN_TTL_DEFAULT_SECONDS = 10800;   // tai lieu muc 2.1: expires_in = 10800
        private const long TOKEN_TTL_SAFETY_SECONDS = 60;       // tru bien de khong het han giua lo
        private const long TOKEN_TTL_MIN_SECONDS = 60;
        private const int MAX_ATTEMPT = 2;                      // 401/503 khi push -> gui lai 1 lan
        private const int HTTP_OK = 200;
        private const int HTTP_UNAUTHORIZED = 401;
        private const int HTTP_FORBIDDEN = 403;
        private const int HTTP_TOO_MANY_REQUESTS = 429;         // RATE_LIMITED -> KHONG day lai ngay
        private const int HTTP_SERVICE_UNAVAILABLE = 503;
        private const int HTTP_GATEWAY_TIMEOUT = 504;
        private const long MAX_BODY_BYTES = 10L * 1024 * 1024;  // tai lieu muc 1: toi da 10 MiB / lan gui
        private const int HTTP_TIMEOUT_MS = 120000;
        // API proxy cho Kho chuyen tiep sang Bo roi moi tra loi -> cho lau hon de nhan duoc 504 co nghia
        // (HOC_BYT_TIMEOUT) thay vi tu het gio phia may tram (vung "chua ro Kho da nhan chua").
        private const int PROXY_TIMEOUT_MS = 180000;
        private const string HEADER_TRACKING_ID = "X-HOC-Tracking-Id";
        private const string ENVELOPE_VERSION = "1.0.6";
        private const string ENVELOPE_RECEIVER_ID = "TTYQG";
        private const string ENVELOPE_TXN_TYPE = "sync_checkup";
        private const string ENVELOPE_MSG_TYPE = "101";
        private const string ENVELOPE_DATA_TYPE = "xml/base64";
        private const string SERVICE_TYPE = "100";

        private readonly KskVlgConfig config;
        // Ma dinh danh co so KCB 13 so (header.sender_id) — da resolve o KskSyncProcessor
        // (truong 6 khoa VLG -> SenderId cong BYT/HSSK/HCC). Chi dung cho Push; tra cuu khong can.
        private readonly string senderGtin;
        private string cachedToken;
        private DateTime tokenExpireAt = DateTime.MinValue;
        private string lastAuthError;
        // FAIL-FAST cho ca lo: login fail voi loi KHONG TU HET (0 = khong ket noi, 401 sai tai khoan,
        // 403 khoa tai khoan/thieu mapping, 429 rate-limit) -> cac Push() con lai cua lo tra Failure NGAY,
        // khong goi mang nua. Neu khong: vien cau hinh sai mat khau + tich N ho so = N lan POST token sai
        // lien tiep -> cong co the KHOA tai khoan tich hop (tai lieu muc 2.1: 403 ACCOUNT_LOCKED / 429
        // RATE_LIMITED); mat ket noi thi moi ho so treo den 120s timeout. Loi 5xx KHONG latch (co the tu het).
        private string batchAuthFatalError;
        // So ho so LIEN TIEP bi "chua ro" (mat phan hoi sau khi gui) — dat nguong thi latch dung lo.
        private int consecutiveUnknown;
        private const int MAX_CONSECUTIVE_UNKNOWN = 2;

        /// <summary>Plugin .NET 4.5 khong bat TLS 1.2 mac dinh — cong Vinh Long HTTPS doi TLS >= 1.2.</summary>
        static KskVlgPusher()
        {
            try
            {
                System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        /// <summary>Chi dung de TRA CUU (GetStatus) — khong can ma 13 so.</summary>
        internal KskVlgPusher(KskVlgConfig config)
            : this(config, null)
        {
        }

        internal KskVlgPusher(KskVlgConfig config, string senderGtin)
        {
            this.config = config;
            this.senderGtin = (senderGtin ?? "").Trim();
        }

        /// <summary>
        /// Day XML KHAMSUCKHOE (da ky) cua MOT ho so (SOLUONGHOSO = 1) len Kho qua API
        /// /api/platform/data-sync/push. Token cache trong instance nay (dung chung ca lo); 401 -> dang
        /// nhap lai, 503 -> gui lai 1 lan, ca 2 truong hop dung NGUYEN mang byte da gui (Kho coi la cung
        /// ban tin, khong tao phien ban moi).
        /// </summary>
        internal KskVlgPushResult Push(string xmlContent, string treatmentCode)
        {
            // msg_id cua ban tin DA ghi len mang (neu co) — loi bat ky sau thoi diem nay deu la "CHUA RO",
            // khong duoc coi la that bai thuong (se day lai = phien ban trung gui Bo).
            string sentMsgId = null;
            try
            {
                string configError = ValidateConfig();
                if (configError != null) return KskVlgPushResult.Failure(configError);
                // Lo nay da gap loi xac thuc/ket noi KHONG TU HET -> fail-fast, khong goi mang cho tung ho so.
                if (this.batchAuthFatalError != null)
                    return KskVlgPushResult.Failure("VLG: " + this.batchAuthFatalError
                        + " (hồ sơ này CHƯA gửi — bỏ qua phần còn lại của lô, tránh cổng khóa tài khoản / treo chờ).");
                if (!KskVlgConfigParser.IsGtin13(this.senderGtin))
                    return KskVlgPushResult.Failure(DescribeMissingGtin());
                if (string.IsNullOrEmpty(xmlContent))
                    return KskVlgPushResult.Failure("VLG: không dựng được dữ liệu đẩy.");

                string msgId = GenerateMsgId(this.senderGtin);
                byte[] body = BuildEnvelopeBody(xmlContent, this.senderGtin, msgId);
                if (body == null)
                    return KskVlgPushResult.Failure("VLG: không đóng gói được bản tin gửi cổng.");
                if (body.LongLength > MAX_BODY_BYTES)
                    return KskVlgPushResult.Failure(string.Format(
                        "VLG: bản tin {0:N0} byte (sau mã hóa base64) vượt giới hạn 10 MiB của cổng (PAYLOAD_TOO_LARGE)."
                        + " Kiểm tra ảnh chữ ký điện tử (CKDT_) / dữ liệu CLS của hồ sơ.", body.LongLength));

                for (int attempt = 0; attempt < MAX_ATTEMPT; attempt++)
                {
                    string token = GetToken();
                    if (string.IsNullOrWhiteSpace(token))
                        return KskVlgPushResult.Failure("VLG: đăng nhập cổng thất bại"
                            + (string.IsNullOrEmpty(this.lastAuthError) ? " (kiểm tra tài khoản tích hợp)." : (" — " + this.lastAuthError)));

                    ProxyHttpResult hr = HttpPostProxy(this.config.PushUrl, body, token);
                    if (hr.RequestSent) sentMsgId = msgId;
                    bool canRetry = attempt + 1 < MAX_ATTEMPT;

                    // Token het han giua chung -> dang nhap lai va gui lai DUNG 1 lan (cung byte).
                    if (hr.Status == HTTP_UNAUTHORIZED && canRetry)
                    {
                        Inventec.Common.Logging.LogSystem.Warn("VLG: cong tra 401 khi push -> dang nhap lai va gui lai. msg_id=" + msgId);
                        ResetToken();
                        continue;
                    }
                    // Kho tam khong ky duoc ban tin gui Bo (HOC_SIGNING_UNAVAILABLE) -> gui lai cung byte 1 lan.
                    if (hr.Status == HTTP_SERVICE_UNAVAILABLE && string.IsNullOrEmpty(hr.TrackingId) && canRetry)
                    {
                        Inventec.Common.Logging.LogSystem.Warn("VLG: cong tra 503 khi push -> gui lai 1 lan. msg_id=" + msgId);
                        continue;
                    }

                    if (hr.Status == 0)
                    {
                        if (!hr.RequestSent)
                        {
                            // Chua gui duoc byte nao (khong ket noi / DNS / TLS) -> chac chan Kho chua nhan.
                            // Token cache con han nen GetToken khong cham mang -> latch tai day cho ca lo.
                            this.batchAuthFatalError = "không kết nối được cổng (kiểm tra mạng / URL đẩy dữ liệu: "
                                + this.config.PushUrl + ")";
                            return KskVlgPushResult.Failure("VLG: " + this.batchAuthFatalError + ".");
                        }
                        // Da gui xong body nhung mat phan hoi (het gio / dut ket noi) -> CHUA RO Kho da nhan chua.
                        return UnknownResult(msgId, treatmentCode, "mất phản hồi sau khi đã gửi (" + hr.NetStatus + ")");
                    }

                    KskVlgPushResult result = ClassifyProxyResponse(hr, msgId);
                    if (result == null)
                    {
                        // 5xx khong co dau hieu cua Kho (khong res_code, khong X-HOC-Tracking-Id) — thuong do
                        // proxy/nginx phia truoc tra. Kho CO THE da nhan -> CHUA RO, doi soat truoc khi gui lai.
                        Inventec.Common.Logging.LogSystem.Warn("VLG: HTTP " + hr.Status + " khong kem ma cua Kho. Body (cat 500): "
                            + Cut(hr.Body, 500));
                        return UnknownResult(msgId, treatmentCode, "cổng trả HTTP " + hr.Status + " không kèm mã của Kho dữ liệu");
                    }
                    this.consecutiveUnknown = 0;
                    Inventec.Common.Logging.LogSystem.Info(string.Format(
                        "VLG: push ma dieu tri={0}; HTTP {1}; res_code={2}; tracking={3}; msg_id={4}; ok={5}",
                        treatmentCode, hr.Status, result.Status, hr.TrackingId, msgId, result.Success));
                    if (!result.Success)
                        Inventec.Common.Logging.LogSystem.Warn("VLG: push that bai. " + result.Message
                            + " Body (cat 2000 ky tu): " + Cut(hr.Body, 2000));
                    return result;
                }
                return KskVlgPushResult.Failure("VLG: gửi lại vẫn thất bại (token hết hạn sau khi đăng nhập lại / Kho tạm không ký được bản tin).");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                if (sentMsgId != null)
                    return UnknownResult(sentMsgId, treatmentCode, "lỗi xử lý phản hồi sau khi đã gửi (" + ex.Message + ")");
                return KskVlgPushResult.Failure("VLG: " + ex.Message);
            }
        }

        /// <summary>
        /// Ket qua "CHUA RO Kho da nhan chua": that bai tam, REGISTRATION_NO = VLG_CHUA_RO,
        /// TRANSACTION_CODE = "MSG:" + msg_id — lan dong bo sau tra doi soat theo sender_id + msg_id
        /// truoc khi gui lai (tai lieu V1.5 muc 7). Mat phan hoi LIEN TIEP -> dung ca lo (cong treo,
        /// moi ho so con lai se cho het 180s).
        /// </summary>
        private KskVlgPushResult UnknownResult(string msgId, string treatmentCode, string detail)
        {
            this.consecutiveUnknown++;
            Inventec.Common.Logging.LogSystem.Warn("VLG: " + detail + " -> CHUA RO. msg_id=" + msgId
                + "; ma dieu tri=" + treatmentCode + "; lien tiep=" + this.consecutiveUnknown);
            if (this.consecutiveUnknown >= MAX_CONSECUTIVE_UNKNOWN && this.batchAuthFatalError == null)
                this.batchAuthFatalError = "cổng không phản hồi " + this.consecutiveUnknown
                    + " hồ sơ liên tiếp — dừng lô, đồng bộ lại sau khi cổng ổn định";
            return new KskVlgPushResult
            {
                Success = false,
                Status = KskVlgBytResCode.CHUA_RO,
                TrackingId = "MSG:" + msgId,
                MsgId = msgId,
                Message = "VLG: " + detail + " — CHƯA RÕ Kho dữ liệu đã nhận hay chưa."
                    + " Lần đồng bộ sau hệ thống tự đối soát trên cổng rồi mới gửi lại (không gửi trùng)."
            };
        }

        /// <summary>
        /// Ket luan cho 1 lan gui DA CO tren Kho (tim duoc qua doi soat / requests[]) — dung khi lan day truoc
        /// mat phan hoi. Kho khong dat / Bo tu choi -> that bai (ma khac CHUA_RO -> lan sau duoc gui ban sua);
        /// Bo da nhan -> thanh cong; con lai (dang xu ly / cho Bo) -> thanh cong "Kho da giu", KHONG gui lai.
        /// </summary>
        internal static KskVlgPushResult FromKnownRequest(KskVlgRequestInfo info, string msgId)
        {
            if (info == null) return null;
            string trk = !string.IsNullOrEmpty(info.TrackingId) ? info.TrackingId : ("MSG:" + msgId);
            var r = new KskVlgPushResult { MsgId = msgId, TrackingId = trk };
            string bo = (info.BytResCode ?? info.BytStatus) ?? "";
            if (info.IsHocInvalid)
            {
                r.Success = false;
                r.Status = "INVALID";
                r.Message = "VLG: lần gửi trước (mất phản hồi) đã vào Kho dữ liệu nhưng KHÔNG ĐẠT kiểm tra"
                    + " — bấm \"Cập nhật KQ cổng\" xem lỗi, sửa hồ sơ rồi đẩy lại.";
            }
            else if (info.IsBytRejected)
            {
                r.Success = false;
                r.Status = !string.IsNullOrEmpty(info.BytResCode) ? info.BytResCode : "BYT_REJECTED";
                r.Message = "VLG: lần gửi trước (mất phản hồi) bị Cổng Bộ Y tế TỪ CHỐI (" + bo + ")"
                    + (string.IsNullOrEmpty(info.BytResMsg) ? "" : (": " + info.BytResMsg))
                    + " — sửa hồ sơ rồi đẩy lại.";
            }
            else if (info.IsBytAccepted)
            {
                r.Success = true;
                r.Status = !string.IsNullOrEmpty(info.BytResCode) ? info.BytResCode : "BYT_ACCEPTED";
                r.Note = "VLG: lần gửi trước (mất phản hồi) ĐÃ vào Kho và Cổng Bộ Y tế đã tiếp nhận (" + bo + ") — không gửi lại.";
            }
            else
            {
                r.Success = true;
                r.Status = "KHO_DA_NHAN";
                r.Note = "VLG: lần gửi trước (mất phản hồi) ĐÃ vào Kho dữ liệu (" + (info.HocStatus ?? "")
                    + (string.IsNullOrEmpty(info.BytStatus) ? "" : (", Bộ: " + info.BytStatus))
                    + ") — không gửi lại. Bấm \"Cập nhật KQ cổng\" để xem kết quả Bộ Y tế.";
            }
            return r;
        }

        /// <summary>Thong bao khi chua co ma 13 so — nguoi trien khai sua cau hinh, khong phai loi du lieu ho so.</summary>
        internal string DescribeMissingGtin()
        {
            return "VLG: chưa khai mã định danh cơ sở KCB 13 số (GTIN/GLN) dùng làm người gửi bản tin"
                + (string.IsNullOrEmpty(this.senderGtin) ? "" : (" — giá trị hiện tại '" + this.senderGtin + "' không đủ 13 chữ số"))
                + ". Khai ở trường thứ 6 của khóa MOS.HIS_KSK_SYNC.VLG_2062_CONNECTION_INFO"
                + " (MaDonVi|Username|Password|TokenUrl|PushUrl|MaGtin13). Mã đơn vị 5 số chỉ dùng để đăng nhập.";
        }

        /// <summary>
        /// Phan loai phan hoi API proxy (tai lieu V1.5 muc 5.1 + muc 6, QD 2062 Phu luc 02). Xet
        /// header.res_code TRUOC (Kho giu nguyen res_code cua Bo bat ke ma HTTP), roi toi ma rieng cua Kho.
        /// Success = true nghia la Kho DA GIU ban tin — gom ca cac ma "dang cho" (tam dung chuyen Bo,
        /// qua gio cho Bo...) vi day lai se tao phien ban moi gui Bo. Tra null khi 5xx khong co dau hieu
        /// nao cua Kho (khong res_code, khong tracking) — Push coi la CHUA RO.
        /// </summary>
        private KskVlgPushResult ClassifyProxyResponse(ProxyHttpResult hr, string msgId)
        {
            Newtonsoft.Json.Linq.JObject jo = TryParseJObject(hr.Body);
            Newtonsoft.Json.Linq.JObject jh = (jo != null) ? jo["header"] as Newtonsoft.Json.Linq.JObject : null;
            Newtonsoft.Json.Linq.JObject jd = (jo != null) ? jo["data"] as Newtonsoft.Json.Linq.JObject : null;
            string resCode = (jh != null) ? (string)jh["res_code"] : null;
            string resMsg = (jh != null) ? (string)jh["res_msg"] : null;
            string txnId = (jh != null) ? (string)jh["txn_id"] : null;
            string dataMsg = (jd != null) ? ((string)jd["data_state_msg"] ?? (string)jd["message_notify"]) : null;
            string checkCa = (jd != null) ? (string)jd["message_check_ca"] : null;
            // Dang phan hoi rieng cua Kho (401/403/429...): { success, code, message }.
            string kCode = (jo != null) ? (string)jo["code"] : null;
            string kMsg = (jo != null) ? (string)jo["message"] : null;
            string code = !string.IsNullOrEmpty(resCode) ? resCode : kCode;
            string msg = !string.IsNullOrEmpty(resMsg) ? resMsg : (!string.IsNullOrEmpty(kMsg) ? kMsg : dataMsg);
            bool is2xx = hr.Status >= 200 && hr.Status < 300;

            var r = new KskVlgPushResult
            {
                MsgId = msgId,
                Status = !string.IsNullOrEmpty(code) ? code : ("HTTP_" + hr.Status),
                TrackingId = !string.IsNullOrEmpty(hr.TrackingId) ? hr.TrackingId : ("MSG:" + msgId)
            };

            if (is2xx && KskVlgBytResCode.IsSuccess(code))
            {
                r.Success = true;
                r.Note = "VLG: Cổng Bộ Y tế đã tiếp nhận (" + code + ")"
                    + (string.IsNullOrEmpty(txnId) ? "" : (", mã giao dịch Bộ " + txnId)) + ".";
                return r;
            }
            if (string.Equals(code, KskVlgBytResCode.FORWARD_PAUSED, StringComparison.OrdinalIgnoreCase))
            {
                r.Success = true;
                r.Note = "VLG: Kho dữ liệu đã tiếp nhận; chuyển tiếp sang Bộ Y tế đang tạm dừng — Kho tự gửi khi mở lại."
                    + " KHÔNG đẩy lại; bấm \"Cập nhật KQ cổng\" để xem kết quả Bộ.";
                return r;
            }
            if (string.Equals(code, KskVlgBytResCode.SAVE_FAIL, StringComparison.OrdinalIgnoreCase))
            {
                r.Success = true;
                r.Note = "VLG: Bộ Y tế lỗi khi lưu bản tin, Kho dữ liệu tự đưa vào hàng đợi gửi lại. KHÔNG đẩy lại.";
                return r;
            }
            if (string.Equals(code, KskVlgBytResCode.AUTH_ACCOUNT_FAIL, StringComparison.OrdinalIgnoreCase))
            {
                r.Success = true;
                r.Note = "VLG: Kho dữ liệu chưa đăng nhập được Cổng Bộ Y tế (lỗi phía Sở, không phải dữ liệu bệnh viện);"
                    + " Kho sẽ đăng nhập lại ở lần xử lý tiếp theo. KHÔNG đẩy lại; báo Sở nếu kéo dài.";
                return r;
            }
            if (string.Equals(code, KskVlgBytResCode.BYT_TIMEOUT, StringComparison.OrdinalIgnoreCase)
                || (hr.Status == HTTP_GATEWAY_TIMEOUT && !string.IsNullOrEmpty(hr.TrackingId)))
            {
                // 504 HOC_BYT_TIMEOUT (hoac 504 kem X-HOC-Tracking-Id): Kho DA ghi nhan (tai lieu muc 5.1)
                // — tuyet doi khong tu tao lan gui moi. 504 tran (proxy phia truoc) -> xu ly o cuoi ham.
                r.Success = true;
                r.Status = KskVlgBytResCode.BYT_TIMEOUT;
                r.Note = "VLG: quá thời gian chờ Cổng Bộ Y tế — Kho dữ liệu đã ghi nhận và tự xử lý tiếp."
                    + " KHÔNG đẩy lại; bấm \"Cập nhật KQ cổng\" sau ít phút.";
                return r;
            }
            if (!string.IsNullOrEmpty(code) && code.StartsWith("HOC_BYT_", StringComparison.OrdinalIgnoreCase))
            {
                // 502 HOC_BYT_*: ma do Kho sinh -> Kho DA giu ban tin, loi tam thoi Kho->Bo, Kho tu gui lai.
                r.Success = true;
                r.Note = "VLG: Cổng Bộ Y tế lỗi tạm thời (" + code + "), Kho dữ liệu sẽ tự gửi lại. KHÔNG đẩy lại.";
                return r;
            }
            if (KskVlgBytResCode.IsRejected(code))
            {
                r.Success = false;
                r.Message = "VLG: Bộ Y tế TỪ CHỐI bản tin — " + code + (string.IsNullOrEmpty(msg) ? "" : (": " + msg))
                    + (string.IsNullOrEmpty(checkCa) ? "" : (" (" + checkCa + ")"))
                    + (KskVlgBytResCode.IsCaSignature(code)
                        ? ". Chữ ký số CKS_BENH_VIEN / CKS_NGUOI_KET_LUAN trong hồ sơ không hợp lệ hoặc thiếu — kiểm tra tích Ký số, chứng thư HSM/USB token rồi đẩy lại."
                        : ". Sửa dữ liệu hồ sơ rồi đẩy lại.");
                return r;
            }
            if (is2xx)
            {
                // Tai lieu muc 5.1: ma khac kem HTTP 2xx = "phan hoi chua nhan dien" — Kho van luu nguyen
                // trang, KHONG ket luan Bo tu choi -> coi la Kho da giu, cho doi soat.
                r.Success = true;
                r.Note = "VLG: Kho dữ liệu đã nhận nhưng phản hồi của Bộ chưa xác định (" + r.Status + ")"
                    + " — bấm \"Cập nhật KQ cổng\" để xem kết quả, KHÔNG đẩy lại.";
                return r;
            }

            if (hr.Status >= 500 && !string.IsNullOrEmpty(hr.TrackingId)
                && !string.Equals(code, KskVlgBytResCode.SIGNING_UNAVAILABLE, StringComparison.OrdinalIgnoreCase))
            {
                // 5xx KEM X-HOC-Tracking-Id: Kho da ghi nhan lan gui (co ma theo doi) — loi tam thoi Kho tu
                // gui tiep; day lai se thanh phien ban moi gui Bo -> coi la Kho da giu, cho doi soat.
                r.Success = true;
                r.Note = "VLG: Kho dữ liệu đã ghi nhận (mã theo dõi " + hr.TrackingId + ") nhưng báo lỗi tạm thời HTTP "
                    + hr.Status + (string.IsNullOrEmpty(code) ? "" : (" " + code))
                    + " — Kho tự xử lý tiếp. KHÔNG đẩy lại; bấm \"Cập nhật KQ cổng\" sau ít phút.";
                return r;
            }
            // 5xx khong kem ma nao cua Kho va khong co X-HOC-Tracking-Id: khong du can cu ket luan -> null
            // (Push tra ket qua CHUA RO, lan sau doi soat roi moi gui lai).
            if (hr.Status >= 500 && string.IsNullOrEmpty(code) && string.IsNullOrEmpty(hr.TrackingId))
                return null;

            // Tu day: Kho KHONG giu ban tin -> that bai, duoc day lai sau khi xu ly nguyen nhan.
            r.Success = false;
            string detail = code + (string.IsNullOrEmpty(msg) ? "" : (" — " + msg));
            if (hr.Status == HTTP_UNAUTHORIZED)
            {
                this.batchAuthFatalError = "xác thực thất bại khi gửi (HTTP 401 " + detail + ") — kiểm tra tài khoản tích hợp";
                r.Message = "VLG: " + this.batchAuthFatalError + ".";
            }
            else if (hr.Status == HTTP_FORBIDDEN)
            {
                this.batchAuthFatalError = "tài khoản tích hợp chưa được cấp quyền gửi dữ liệu KSK sang Bộ qua Kho (HTTP 403 "
                    + detail + ") — liên hệ Sở Y tế";
                r.Message = "VLG: " + this.batchAuthFatalError + ".";
            }
            else if (hr.Status == HTTP_TOO_MANY_REQUESTS)
            {
                this.batchAuthFatalError = "cổng đang giới hạn tần suất (HTTP 429) — chờ ít phút rồi đẩy lại";
                r.Message = "VLG: " + this.batchAuthFatalError + ".";
            }
            else if (hr.Status == HTTP_SERVICE_UNAVAILABLE)
                r.Message = "VLG: Kho dữ liệu tạm thời không ký được bản tin gửi Bộ (HTTP 503 " + detail + ") — đẩy lại sau ít phút.";
            else if (hr.Status == 400)
                r.Message = "VLG: Kho dữ liệu từ chối bản tin (" + detail + ") — lỗi dựng bản tin phía HIS, báo bộ phận IT.";
            else if (hr.Status == 413)
                r.Message = "VLG: bản tin vượt giới hạn 10 MiB của cổng (PAYLOAD_TOO_LARGE) — kiểm tra ảnh chữ ký (CKDT_) / dữ liệu CLS.";
            else if (hr.Status == 410)
                r.Message = "VLG: cổng báo API đã ngừng (HTTP 410 " + detail + ") — kiểm tra URL đẩy dữ liệu trong cấu hình.";
            else
                r.Message = "VLG: HTTP " + hr.Status + " " + detail + (string.IsNullOrEmpty(dataMsg) || dataMsg == msg ? "" : ("; " + dataMsg));
            return r;
        }

        /// <summary>
        /// Envelope Phu luc 02 QD 2062: { header{version, sender_id, receiver_id, txn_type, msg_id, msg_type,
        /// data_type, send_datetime}, data, signature }. Thu tu truong co dinh; serialize MOT lan.
        /// </summary>
        private static byte[] BuildEnvelopeBody(string xmlContent, string senderGtin, string msgId)
        {
            try
            {
                var header = new Newtonsoft.Json.Linq.JObject();
                header["version"] = ENVELOPE_VERSION;
                header["sender_id"] = senderGtin;
                header["receiver_id"] = ENVELOPE_RECEIVER_ID;
                header["txn_type"] = ENVELOPE_TXN_TYPE;
                header["msg_id"] = msgId;
                header["msg_type"] = ENVELOPE_MSG_TYPE;
                header["data_type"] = ENVELOPE_DATA_TYPE;
                header["send_datetime"] = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;

                var envelope = new Newtonsoft.Json.Linq.JObject();
                envelope["header"] = header;
                envelope["data"] = Convert.ToBase64String(Encoding.UTF8.GetBytes(xmlContent));
                envelope["signature"] = "";   // Kho ky ban tin gui Bo bang khoa So — HIS khong ky envelope
                return Encoding.UTF8.GetBytes(envelope.ToString(Newtonsoft.Json.Formatting.None));
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); return null; }
        }

        /// <summary>msg_id = sender_id + yyMMdd + UUIDv4 bo gach (QD 2062 Phu luc 02 muc 5.2, truong 1.5).</summary>
        private static string GenerateMsgId(string senderGtin)
        {
            return (senderGtin ?? "")
                 + DateTime.Now.ToString("yyMMdd", System.Globalization.CultureInfo.InvariantCulture)
                 + Guid.NewGuid().ToString("N");
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
                Inventec.Common.Logging.LogSystem.Warn("VLG: phan hoi khong phai JSON hop le: " + Cut(body, 300) + " — " + ex.Message);
                return null;
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

                // Doi chieu ma don vi cua TOKEN voi MaDonVi cau hinh: lech = khai nham tai khoan cua don vi
                // khac -> ho so se ghi nhan vao Kho sai don vi. Canh bao som de sua cau hinh.
                string maDonViToken = resp.Data.MaDonVi;
                if (!string.IsNullOrWhiteSpace(maDonViToken)
                    && !string.Equals(maDonViToken.Trim(), this.config.MaDonVi, StringComparison.OrdinalIgnoreCase))
                    Inventec.Common.Logging.LogSystem.Warn(string.Format(
                        "VLG: ma_don_vi cua token ({0}) KHAC MaDonVi cau hinh ({1}) — kiem tra lai tai khoan tich hop"
                        + " / truong 1 cua MOS.HIS_KSK_SYNC.VLG_2062_CONNECTION_INFO.",
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

        /// <summary>Ket qua 1 lan POST len API proxy (can them header phan hoi + biet da gui xong body chua).</summary>
        private sealed class ProxyHttpResult
        {
            internal int Status;            // 0 = khong co phan hoi HTTP
            internal string Body;
            internal string TrackingId;     // header X-HOC-Tracking-Id (co ca khi 4xx/5xx)
            internal bool RequestSent;      // da ghi xong body len mang
            internal System.Net.WebExceptionStatus NetStatus;
        }

        /// <summary>
        /// POST envelope len /api/platform/data-sync/push voi du 5 header bat buoc (tai lieu V1.5 muc 5.1).
        /// Doc ca header X-HOC-Tracking-Id. RequestSent = false -> chac chan Kho chua nhan (an toan gui lai);
        /// RequestSent = true + Status = 0 -> mat phan hoi, CHUA RO Kho da nhan chua.
        /// </summary>
        private static ProxyHttpResult HttpPostProxy(string url, byte[] body, string bearerToken)
        {
            var r = new ProxyHttpResult();
            try
            {
                var request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.Accept = "*/*";
                request.Headers.Add("Accept-Encoding", "gzip,deflate,br");
                request.Headers.Add("service-type", SERVICE_TYPE);
                // .NET 4.5 chi giai nen gzip/deflate — Kho (kiem chung dev 24/09) tra JSON khong nen.
                request.AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate;
                request.Timeout = PROXY_TIMEOUT_MS;
                request.ReadWriteTimeout = PROXY_TIMEOUT_MS;
                if (!string.IsNullOrEmpty(bearerToken))
                    request.Headers.Add("Authorization", "Bearer " + bearerToken);
                request.ContentLength = (body != null) ? body.Length : 0;
                try
                {
                    using (var stream = request.GetRequestStream())
                        if (body != null && body.Length > 0) stream.Write(body, 0, body.Length);
                    r.RequestSent = true;
                    using (var response = (System.Net.HttpWebResponse)request.GetResponse())
                    {
                        r.Status = (int)response.StatusCode;
                        r.TrackingId = response.Headers[HEADER_TRACKING_ID];
                        r.Body = ReadProxyBody(response);
                    }
                }
                catch (System.Net.WebException wex)
                {
                    r.NetStatus = wex.Status;
                    var errResponse = wex.Response as System.Net.HttpWebResponse;
                    if (errResponse == null)
                    {
                        Inventec.Common.Logging.LogSystem.Error("VLG: khong nhan duoc phan hoi tu " + url + " — " + wex.Status
                            + " " + wex.Message + " (da gui body: " + r.RequestSent + ")", wex);
                        return r;
                    }
                    using (errResponse)
                    {
                        r.Status = (int)errResponse.StatusCode;
                        r.TrackingId = errResponse.Headers[HEADER_TRACKING_ID];
                        r.Body = ReadProxyBody(errResponse);
                    }
                }
            }
            catch (Exception ex)
            {
                // Loi IO/khac: giu nguyen RequestSent de phan biet "chua gui" / "chua ro".
                Inventec.Common.Logging.LogSystem.Error("VLG: loi khi gui " + url + " (da gui body: " + r.RequestSent + ")", ex);
                r.Status = 0;
            }
            return r;
        }

        /// <summary>
        /// Doc body phan hoi proxy. Kho tra Content-Encoding br (Brotli) -> .NET 4.5 khong giai nen duoc:
        /// bo body (phan loai theo HTTP status + X-HOC-Tracking-Id) thay vi doc ra chuoi rac.
        /// </summary>
        private static string ReadProxyBody(System.Net.HttpWebResponse response)
        {
            try
            {
                string ce = response.Headers["Content-Encoding"];
                if (!string.IsNullOrEmpty(ce) && ce.IndexOf("br", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    Inventec.Common.Logging.LogSystem.Warn("VLG: phan hoi nen Brotli (Content-Encoding: " + ce
                        + ") — .NET 4.5 khong giai nen, bo qua body; phan loai theo HTTP status + tracking.");
                    return null;
                }
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
            return ReadBody(response);
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
        /// TRA CUU ket qua xu ly that cua 1 ho so theo ma lien ket (= ma dieu tri):
        /// GET {base}/api/kham-suc-khoe/qd-2062/ho-so/trang-thai?ma_lk=... (Bearer token).
        /// URL suy tu goc host cua TokenUrl (xem BuildStatusUrl).
        /// Tra ve validation_status cua HO SO + loi cua LAN GUI MOI NHAT. 404 -> Found=false
        /// (ho so chua co tren cong — chua tung day / khac moi truong).
        /// </summary>
        internal KskVlgStatusResult GetStatus(string maLk)
        {
            try
            {
                string configError = ValidateConfig();
                if (configError != null) return KskVlgStatusResult.Failure(configError);
                if (this.batchAuthFatalError != null)
                    return KskVlgStatusResult.Failure("VLG: " + this.batchAuthFatalError + " (bỏ qua tra cứu).");
                if (string.IsNullOrWhiteSpace(maLk))
                    return KskVlgStatusResult.Failure("VLG: hồ sơ không có mã điều trị để tra cứu.");

                string statusUrl = BuildStatusUrl() + "?ma_lk=" + Uri.EscapeDataString(maLk.Trim());
                for (int attempt = 0; attempt < MAX_ATTEMPT; attempt++)
                {
                    string token = GetToken();
                    if (string.IsNullOrWhiteSpace(token))
                        return KskVlgStatusResult.Failure("VLG: đăng nhập cổng thất bại"
                            + (string.IsNullOrEmpty(this.lastAuthError) ? "." : (" — " + this.lastAuthError)));

                    int status;
                    string respBody = HttpGet(statusUrl, token, out status);
                    if (status == HTTP_UNAUTHORIZED && attempt + 1 < MAX_ATTEMPT) { ResetToken(); continue; }
                    if (status == 0)
                    {
                        // Mat ket noi giua lo tra cuu -> latch fail-fast nhu Push (token cache con han nen
                        // GetToken khong cham mang — khong latch o day thi moi ho so sau treo toi 120s).
                        this.batchAuthFatalError = "không kết nối được cổng (URL tra cứu: " + statusUrl + ")";
                        return KskVlgStatusResult.Failure("VLG: " + this.batchAuthFatalError + ".");
                    }
                    if (status == 404)
                        return new KskVlgStatusResult { Ok = true, Found = false, Message = "Chưa có hồ sơ trên cổng" };
                    if (status != HTTP_OK || string.IsNullOrWhiteSpace(respBody))
                        return KskVlgStatusResult.Failure("VLG: tra cứu thất bại (HTTP " + status + ").");

                    return ParseStatusResponse(respBody);
                }
                return KskVlgStatusResult.Failure("VLG: xác thực thất bại khi tra cứu.");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return KskVlgStatusResult.Failure("VLG: " + ex.Message);
            }
        }

        /// <summary>
        /// URL tra cuu = goc host cua TokenUrl + /api/kham-suc-khoe/qd-2062/ho-so/trang-thai (nhu 3 plugin VLG
        /// khac) — PushUrl gio la /platform/data-sync/push nen khong suy tu PushUrl duoc nua.
        /// </summary>
        private string BuildStatusUrl()
        {
            string tokenUrl = (this.config != null) ? this.config.TokenUrl : null;
            return KskVlgConfigParser.DeriveBaseUrl(tokenUrl) + "/api/kham-suc-khoe/qd-2062/ho-so/trang-thai";
        }

        /// <summary>
        /// Parse response tra cuu (JObject vi cau truc long: data.ho_so + data.requests[].errors[]).
        /// Loi lay tu LAN GUI MOI NHAT (received_at lon nhat), chi muc severity=ERROR, toi da 5 dong.
        /// </summary>
        private static KskVlgStatusResult ParseStatusResponse(string body)
        {
            try
            {
                var jo = Newtonsoft.Json.Linq.JObject.Parse(body);
                // {"data":null} -> JValue(Null) khac null: phai "as JObject" (index tiep se nem loi).
                var jd = jo["data"] as Newtonsoft.Json.Linq.JObject;
                var hoSo = (jd != null) ? jd["ho_so"] as Newtonsoft.Json.Linq.JObject : null;
                if (hoSo == null)
                    return new KskVlgStatusResult { Ok = true, Found = false, Message = "Cổng không trả thông tin hồ sơ" };

                var result = new KskVlgStatusResult
                {
                    Ok = true,
                    Found = true,
                    ValidationStatus = (string)hoSo["validation_status"],
                    Message = (string)hoSo["message"],
                    RequestsByMsgId = new Dictionary<string, KskVlgRequestInfo>(StringComparer.OrdinalIgnoreCase)
                };

                // Lan gui moi nhat -> gom loi ERROR de hien thi ly do khong dat.
                var requests = jd["requests"] as Newtonsoft.Json.Linq.JArray;
                if (requests != null && requests.Count > 0)
                {
                    Newtonsoft.Json.Linq.JToken latest = null;
                    DateTime latestAt = DateTime.MinValue;
                    foreach (var r in requests)
                    {
                        if (!(r is Newtonsoft.Json.Linq.JObject)) continue;
                        string mid = (string)r["msg_id"];
                        if (!string.IsNullOrEmpty(mid))
                        {
                            result.RequestsByMsgId[mid] = KskVlgRequestInfo.FromJson(r);
                        }
                        // received_at: Newtonsoft da doi chuoi ISO thanh kieu Date -> so sanh theo thoi gian,
                        // KHONG so sanh chuoi (dang "MM/dd/yyyy HH:mm:ss" sai thu tu qua nam). Thieu -> MinValue.
                        DateTime at = ReadTime(r["received_at"]);
                        if (latest == null || at > latestAt)
                        {
                            latest = r; latestAt = at;
                        }
                    }
                    if (latest != null)
                    {
                        // API V1.5: trang thai Kho + ket qua Cong Bo cua lan gui moi nhat.
                        result.LatestSourceChannel = (string)latest["source_channel"];
                        result.LatestHocStatus = (string)latest["hoc_status"] ?? (string)latest["status"];
                        result.LatestBytStatus = (string)latest["byt_status"];
                        result.LatestBytResCode = (string)latest["byt_res_code"];
                        result.LatestTrackingId = (string)latest["tracking_id"];
                    }
                    var errs = (latest != null) ? latest["errors"] as Newtonsoft.Json.Linq.JArray : null;
                    if (errs != null && errs.Count > 0)
                    {
                        var parts = new List<string>();
                        foreach (var e in errs)
                        {
                            if (!string.Equals((string)e["severity"], "ERROR", StringComparison.OrdinalIgnoreCase)) continue;
                            if (parts.Count >= 5) { parts.Add("..."); break; }
                            parts.Add(((string)e["code"] ?? "") + " (" + ((string)e["field_path"] ?? "") + "): "
                                + ((string)e["message"] ?? ""));
                        }
                        if (parts.Count > 0) result.ErrorSummary = string.Join(" | ", parts.ToArray());
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn("VLG: khong parse duoc response tra cuu: " + ex.Message);
                return KskVlgStatusResult.Failure("VLG: không đọc được phản hồi tra cứu từ cổng.");
            }
        }

        /// <summary>Thoi diem tu JSON (kieu Date da parse san hoac chuoi ISO). Khong doc duoc -> MinValue.</summary>
        private static DateTime ReadTime(Newtonsoft.Json.Linq.JToken t)
        {
            try
            {
                if (t == null || t.Type == Newtonsoft.Json.Linq.JTokenType.Null) return DateTime.MinValue;
                if (t.Type == Newtonsoft.Json.Linq.JTokenType.Date) return (DateTime)t;
                DateTime dt;
                if (DateTime.TryParse((string)t, System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.RoundtripKind, out dt)) return dt;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
            return DateTime.MinValue;
        }

        /// <summary>
        /// DOI SOAT 1 ban tin theo sender_id + msg_id HIS da gui (tai lieu V1.5 muc 5.8.1 — cach tai lieu
        /// yeu cau khi het gio / mat ket noi): GET {base}/api/kham-suc-khoe/doi-soat-byt/trang-thai.
        /// 200 -> Found + trang thai Kho/Bo cua ban tin; 404 NOT_FOUND -> Kho CHUA co ban tin (an toan gui lai);
        /// loi khac -> Ok = false (khong ket luan). Kiem chung dev + prod 24/09/2026.
        /// </summary>
        internal KskVlgMessageLookup LookupMessage(string senderId, string msgId)
        {
            try
            {
                string configError = ValidateConfig();
                if (configError != null) return new KskVlgMessageLookup { FailReason = configError };
                if (this.batchAuthFatalError != null)
                    return new KskVlgMessageLookup { FailReason = "VLG: " + this.batchAuthFatalError + " (bỏ qua đối soát)." };
                if (string.IsNullOrWhiteSpace(senderId) || string.IsNullOrWhiteSpace(msgId))
                    return new KskVlgMessageLookup { FailReason = "VLG: thiếu sender_id/msg_id để đối soát." };

                string url = KskVlgConfigParser.DeriveBaseUrl(this.config.TokenUrl) + "/api/kham-suc-khoe/doi-soat-byt/trang-thai"
                    + "?sender_id=" + Uri.EscapeDataString(senderId.Trim()) + "&msg_id=" + Uri.EscapeDataString(msgId.Trim());
                for (int attempt = 0; attempt < MAX_ATTEMPT; attempt++)
                {
                    string token = GetToken();
                    if (string.IsNullOrWhiteSpace(token))
                        return new KskVlgMessageLookup
                        {
                            FailReason = "VLG: đăng nhập cổng thất bại" + (string.IsNullOrEmpty(this.lastAuthError) ? "." : (" — " + this.lastAuthError))
                        };
                    int status;
                    string body = HttpGet(url, token, out status);
                    if (status == HTTP_UNAUTHORIZED && attempt + 1 < MAX_ATTEMPT) { ResetToken(); continue; }
                    if (status == 0)
                    {
                        this.batchAuthFatalError = "không kết nối được cổng (URL đối soát: " + url + ")";
                        return new KskVlgMessageLookup { FailReason = "VLG: " + this.batchAuthFatalError + "." };
                    }
                    Newtonsoft.Json.Linq.JObject jo = TryParseJObject(body);
                    if (status == 404)
                    {
                        // Chi tin 404 khi dung ma NOT_FOUND cua Kho (khong phai 404 do sai duong dan/proxy).
                        string code = (jo != null) ? (string)jo["code"] : null;
                        if (string.Equals(code, "NOT_FOUND", StringComparison.OrdinalIgnoreCase))
                            return new KskVlgMessageLookup { Ok = true, Found = false };
                        return new KskVlgMessageLookup { FailReason = "VLG: đối soát trả HTTP 404 không rõ nguồn." };
                    }
                    var jd = (jo != null) ? jo["data"] as Newtonsoft.Json.Linq.JObject : null;
                    var item = (jd != null) ? jd["item"] as Newtonsoft.Json.Linq.JObject : null;
                    if (status != HTTP_OK || item == null)
                        return new KskVlgMessageLookup { FailReason = "VLG: đối soát thất bại (HTTP " + status + ")." };
                    KskVlgRequestInfo info = KskVlgRequestInfo.FromJson(item);
                    if (info != null && string.IsNullOrEmpty(info.MsgId)) info.MsgId = msgId;
                    return new KskVlgMessageLookup { Ok = true, Found = true, Info = info };
                }
                return new KskVlgMessageLookup { FailReason = "VLG: xác thực thất bại khi đối soát." };
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return new KskVlgMessageLookup { FailReason = "VLG: " + ex.Message };
            }
        }

        /// <summary>GET voi Bearer token — dung cho API tra cuu. Hanh vi doc body/loi nhu HttpPost.</summary>
        private static string HttpGet(string url, string bearerToken, out int statusCode)
        {
            statusCode = 0;
            var request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);
            request.Method = "GET";
            request.Accept = "application/json";
            request.Timeout = HTTP_TIMEOUT_MS;
            request.ReadWriteTimeout = HTTP_TIMEOUT_MS;
            if (!string.IsNullOrEmpty(bearerToken))
                request.Headers.Add("Authorization", "Bearer " + bearerToken);
            try
            {
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
