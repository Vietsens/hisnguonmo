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
using HIS.Desktop.Plugins.VlgPortalLookup.ADO;

namespace HIS.Desktop.Plugins.VlgPortalLookup
{
    /// <summary>
    /// Client TRA CUU (chi GET) Cong tiep nhan — Kho du lieu y te tinh Vinh Long, theo tai lieu
    /// "API Document CongTiepNhan" V1.3. Dung chung khoa cau hinh voi man Dong bo KSK:
    /// <c>MOS.HIS_KSK_SYNC.VLG_2062_CONNECTION_INFO</c> = MaDonVi|Username|Password|TokenUrl|PushUrl
    /// (2 URL bo trong -> cong chinh thuc). BaseUrl suy tu TokenUrl de moi API GET tu dong theo
    /// DUNG moi truong (dev/chinh thuc) ma khoa dang tro.
    /// API dung: POST /api/xac-thuc/token; GET /api/xac-thuc/thong-tin;
    /// GET /api/kham-suc-khoe/qd-2062/ho-so; GET /api/kham-suc-khoe/qd-2062/ho-so/trang-thai.
    /// </summary>
    internal class VlgPortalClient
    {
        internal const string CONFIG_KEY = "MOS.HIS_KSK_SYNC.VLG_2062_CONNECTION_INFO";
        internal const string DEFAULT_BASE_URL = "https://congtiepnhan.kdlyt.vinhlong.vn";
        private const string TOKEN_PATH = "/api/xac-thuc/token";
        private const long TOKEN_TTL_DEFAULT_SECONDS = 10800;
        private const long TOKEN_TTL_SAFETY_SECONDS = 60;
        private const int HTTP_OK = 200;
        private const int HTTP_UNAUTHORIZED = 401;
        private const int MAX_ATTEMPT = 2;
        private const int HTTP_TIMEOUT_MS = 60000;
        private const int PAGE_SIZE = 100;
        private const int MAX_PAGES = 50;   // chan vong lap vo han neu cong tra total bat thuong

        private readonly string maDonVi;
        private readonly string username;
        private readonly string password;
        private readonly string baseUrl;
        private readonly bool configured;
        private string cachedToken;
        private DateTime tokenExpireAt = DateTime.MinValue;
        private string lastAuthError;
        // Loi khong tu het giua MOT thao tac (mat mang, sai tai khoan) -> cac call sau cua thao tac do
        // tra loi ngay, khong treo tung call. UI PHAI goi ResetBatchError() dau moi thao tac nguoi dung
        // (client song ca doi man hinh — khong reset thi mot lan mat mang se khoa man vinh vien).
        private string batchFatalError;
        // Serialize viec lay token: check ket noi tu dong luc Load co the chay song song voi Tim kiem.
        private readonly object tokenSync = new object();

        /// <summary>Danh sach lan lay gan nhat co bi cat o MAX_PAGES khong (de UI canh bao).</summary>
        internal bool LastListTruncated { get; private set; }

        /// <summary>Xoa loi chot cua thao tac truoc — goi dau moi thao tac nguoi dung.</summary>
        internal void ResetBatchError() { this.batchFatalError = null; }

        /// <summary>Plugin .NET 4.5 khong bat TLS 1.2 mac dinh — cong doi TLS >= 1.2.</summary>
        static VlgPortalClient()
        {
            try
            {
                System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        internal VlgPortalClient(string connectionInfo)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(connectionInfo))
                {
                    string[] f = connectionInfo.Split('|');
                    this.maDonVi = Get(f, 0);
                    this.username = Get(f, 1);
                    this.password = Get(f, 2);
                    string tokenUrl = Get(f, 3);
                    // BaseUrl suy tu TokenUrl (bo duoi /api/xac-thuc/token) -> GET theo dung moi truong cua khoa.
                    // Bat buoc khop CA dau '/' truoc duoi (khong thi cat lech 1 ky tu voi URL dang la).
                    string t = string.IsNullOrWhiteSpace(tokenUrl) ? "" : tokenUrl.Trim().TrimEnd('/');
                    int idx = t.Length - TOKEN_PATH.Length;
                    if (idx > 0 && string.Compare(t, idx, TOKEN_PATH, 0, TOKEN_PATH.Length,
                            StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        this.baseUrl = t.Substring(0, idx);
                    }
                    else
                    {
                        if (t.Length > 0)
                            Inventec.Common.Logging.LogSystem.Warn(
                                "VlgPortalLookup: TokenUrl trong khoa cau hinh khong dung dang .../api/xac-thuc/token"
                                + " -> dung cong CHINH THUC mac dinh. TokenUrl=" + t);
                        this.baseUrl = DEFAULT_BASE_URL;
                    }
                    this.configured = !string.IsNullOrWhiteSpace(this.maDonVi)
                        && !string.IsNullOrWhiteSpace(this.username) && !string.IsNullOrWhiteSpace(this.password);
                }
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); }
        }

        internal bool IsConfigured { get { return this.configured; } }
        internal string BaseUrl { get { return this.baseUrl ?? DEFAULT_BASE_URL; } }
        internal string MaDonVi { get { return this.maDonVi; } }

        /// <summary>Moi truong dang tro theo khoa cau hinh — hien tren UI de khoi doan mo.</summary>
        internal string MoiTruong
        {
            get
            {
                return BaseUrl.IndexOf("dev-", StringComparison.OrdinalIgnoreCase) >= 0
                    ? "THỬ NGHIỆM (dev)" : "CHÍNH THỨC";
            }
        }

        /// <summary>
        /// GET /api/xac-thuc/thong-tin — kiem tra ket noi + tai khoan/don vi con hieu luc.
        /// Tra text hien thi (kem ten don vi khi thanh cong); ok = false khi loi.
        /// </summary>
        internal string CheckConnection(out bool ok)
        {
            ok = false;
            try
            {
                if (!this.configured) return "Chưa cấu hình khóa " + CONFIG_KEY;
                this.batchFatalError = null;   // kiem tra ket noi = co hoi thu lai sau khi mat mang
                string token = GetToken();
                if (string.IsNullOrEmpty(token))
                    return "Đăng nhập cổng thất bại — " + (this.lastAuthError ?? "không rõ nguyên nhân");
                int status;
                string body = HttpGet(BaseUrl + "/api/xac-thuc/thong-tin", token, out status);
                if (status != HTTP_OK || string.IsNullOrEmpty(body))
                    return "Cổng phản hồi lỗi (HTTP " + status + ")";
                var jo = Newtonsoft.Json.Linq.JObject.Parse(body);
                var data = jo["data"] as Newtonsoft.Json.Linq.JObject;
                ok = true;
                return string.Format("Kết nối OK — {0} (mã {1}) — môi trường {2}",
                    (data != null) ? (string)data["ten_don_vi"] : "",
                    (data != null) ? (string)data["ma_don_vi"] : "",
                    MoiTruong);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return "Lỗi kiểm tra kết nối: " + ex.Message;
            }
        }

        /// <summary>
        /// GET /api/kham-suc-khoe/qd-2062/ho-so — danh sach ho so KSK cua don vi theo NGAY KHAM
        /// (khong phai ngay day!). Tu dong lat het trang (page_size 100). errorStatus: all/has_error/no_error.
        /// error != null khi call loi (mang/dang nhap/khoang ngay qua 3 thang...).
        /// </summary>
        internal List<VlgHoSoADO> GetKskHoSoList(DateTime fromDate, DateTime toDate, string errorStatus, out string error)
        {
            error = null;
            this.LastListTruncated = false;
            var result = new List<VlgHoSoADO>();
            try
            {
                if (!this.configured) { error = "Chưa cấu hình khóa " + CONFIG_KEY; return result; }
                if (this.batchFatalError != null) { error = this.batchFatalError; return result; }
                string token = GetToken();
                if (string.IsNullOrEmpty(token))
                { error = "Đăng nhập cổng thất bại — " + (this.lastAuthError ?? ""); return result; }

                int page = 1, total = -1;
                while (true)
                {
                    string url = BaseUrl + "/api/kham-suc-khoe/qd-2062/ho-so"
                        + "?from_date=" + fromDate.ToString("yyyy-MM-dd")
                        + "&to_date=" + toDate.ToString("yyyy-MM-dd")
                        + "&page=" + page + "&page_size=" + PAGE_SIZE
                        + (string.IsNullOrEmpty(errorStatus) ? "" : ("&error_status=" + errorStatus));
                    int status;
                    string body = HttpGetWithRetry(url, ref token, out status);
                    // Coalesce: latch co the vua bi thread check-ket-noi xoa — khong duoc tra error=null kem list cut.
                    if (status == 0)
                    { error = this.batchFatalError ?? ("không kết nối được cổng (" + BaseUrl + ") — kiểm tra mạng/firewall"); return result; }
                    if (status != HTTP_OK || string.IsNullOrEmpty(body))
                    { error = "Cổng trả lỗi (HTTP " + status + ") — " + Cut(body, 300); return result; }

                    var jo = Newtonsoft.Json.Linq.JObject.Parse(body);
                    var data = jo["data"] as Newtonsoft.Json.Linq.JObject;
                    if (data == null) break;
                    if (total < 0) total = ToInt(data["total"]);
                    var items = data["items"] as Newtonsoft.Json.Linq.JArray;
                    if (items == null || items.Count == 0) break;
                    foreach (var it in items) result.Add(VlgHoSoADO.FromPortalItem(it));
                    // total = 0/thieu (cong doi schema) van phai lat tiep khi trang con day.
                    if ((total > 0 && result.Count >= total) || items.Count < PAGE_SIZE) break;
                    if (page >= MAX_PAGES) { this.LastListTruncated = true; break; }
                    page++;
                }
                return result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                error = ex.Message;
                return result;
            }
        }

        /// <summary>
        /// GET /api/kham-suc-khoe/qd-2062/ho-so/trang-thai?ma_lk= — chi tiet 1 ho so: tung lan gui,
        /// tien trinh (events) va loi tung truong. Tra ADO tom tat + DetailText da render de hien memo.
        /// found = false khi cong tra 404 (chua tung tiep nhan thanh cong).
        /// </summary>
        internal VlgHoSoADO GetKskTrangThai(string maLk, out bool found, out string error)
        {
            found = false; error = null;
            try
            {
                if (!this.configured) { error = "Chưa cấu hình khóa " + CONFIG_KEY; return null; }
                if (this.batchFatalError != null) { error = this.batchFatalError; return null; }
                if (string.IsNullOrWhiteSpace(maLk)) { error = "Chưa nhập mã điều trị."; return null; }
                string token = GetToken();
                if (string.IsNullOrEmpty(token))
                { error = "Đăng nhập cổng thất bại — " + (this.lastAuthError ?? ""); return null; }

                string url = BaseUrl + "/api/kham-suc-khoe/qd-2062/ho-so/trang-thai?ma_lk="
                    + Uri.EscapeDataString(maLk.Trim());
                int status;
                string body = HttpGetWithRetry(url, ref token, out status);
                if (status == 0)
                { error = this.batchFatalError ?? ("không kết nối được cổng (" + BaseUrl + ") — kiểm tra mạng/firewall"); return null; }
                if (status == 404) { found = false; return null; }
                if (status != HTTP_OK || string.IsNullOrEmpty(body))
                { error = "Cổng trả lỗi (HTTP " + status + ") — " + Cut(body, 300); return null; }

                found = true;
                return ParseTrangThai(body);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                error = ex.Message;
                return null;
            }
        }

        #region Nhom KCB (kham chua benh)
        /// <summary>
        /// GET /api/kham-chua-benh/ho-so — danh sach ho so KCB cua don vi theo ngay tiep nhan (sent_date).
        /// includeCancelled: true -> gom ca ho so da huy. Tu lat het trang nhu KSK.
        /// </summary>
        internal List<VlgHoSoADO> GetKcbHoSoList(DateTime fromDate, DateTime toDate, string errorStatus,
            bool includeCancelled, out string error)
        {
            return GetPagedList("/api/kham-chua-benh/ho-so",
                "?from_date=" + fromDate.ToString("yyyy-MM-dd")
                + "&to_date=" + toDate.ToString("yyyy-MM-dd")
                + (includeCancelled ? "&include_cancelled=true" : "")
                + (string.IsNullOrEmpty(errorStatus) ? "" : ("&error_status=" + errorStatus)),
                VlgHoSoADO.FromKcbItem, out error);
        }

        /// <summary>GET /api/kham-chua-benh/ho-so/trang-thai?ma_lk= — trang thai + cac lan gui cua 1 ho so KCB.</summary>
        internal VlgHoSoADO GetKcbTrangThai(string maLk, out bool found, out string error)
        {
            found = false; error = null;
            try
            {
                if (!this.configured) { error = "Chưa cấu hình khóa " + CONFIG_KEY; return null; }
                if (this.batchFatalError != null) { error = this.batchFatalError; return null; }
                if (string.IsNullOrWhiteSpace(maLk)) { error = "Chưa nhập mã liên kết."; return null; }
                string token = GetToken();
                if (string.IsNullOrEmpty(token))
                { error = "Đăng nhập cổng thất bại — " + (this.lastAuthError ?? ""); return null; }

                string url = BaseUrl + "/api/kham-chua-benh/ho-so/trang-thai?ma_lk="
                    + Uri.EscapeDataString(maLk.Trim());
                int status;
                string body = HttpGetWithRetry(url, ref token, out status);
                if (status == 0)
                { error = this.batchFatalError ?? ("không kết nối được cổng (" + BaseUrl + ") — kiểm tra mạng/firewall"); return null; }
                if (status == 404) { found = false; return null; }
                if (status != HTTP_OK || string.IsNullOrEmpty(body))
                { error = "Cổng trả lỗi (HTTP " + status + ") — " + Cut(body, 300); return null; }

                found = true;
                return ParseKcbTrangThai(maLk, body);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                error = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// POST /api/kham-chua-benh/huy-ho-so (restore=false) / khoi-phuc-ho-so (restore=true).
        /// Cong xu ly NGAY (PROCESSED, khong QUEUED). 404 = ho so chua tung co tren cong.
        /// Tra message ket qua; error != null khi that bai.
        /// </summary>
        internal string PostKcbAction(bool restore, string maLk, string lyDo, out string error)
        {
            error = null;
            try
            {
                if (!this.configured) { error = "Chưa cấu hình khóa " + CONFIG_KEY; return null; }
                if (this.batchFatalError != null) { error = this.batchFatalError; return null; }
                string token = GetToken();
                if (string.IsNullOrEmpty(token))
                { error = "Đăng nhập cổng thất bại — " + (this.lastAuthError ?? ""); return null; }

                maLk = (maLk ?? "").Trim();
                lyDo = (lyDo ?? "").Trim();
                string action = restore ? "khoi-phuc-ho-so" : "huy-ho-so";
                var payload = new Newtonsoft.Json.Linq.JObject();
                payload["MA_LK"] = maLk;
                payload["MA_CSKCB"] = this.maDonVi;
                payload[restore ? "LY_DO" : "LY_DO_HUY"] = lyDo;
                // MA_YEU_CAU phai DUY NHAT theo lan bam (khong hash noi dung): huy -> khoi phuc -> huy lai
                // cung ly do se trung ma va bi cong dedup (khong xu ly) trong khi UI bao thanh cong.
                // Retry 401 trong cung lan bam van dung lai json nay -> van idempotent cho 1 thao tac.
                payload["MA_YEU_CAU"] = "HIS-" + (restore ? "RESTORE" : "CANCEL") + "-" + maLk + "-"
                    + DateTime.Now.ToString("yyyyMMddHHmmss");
                string json = payload.ToString(Newtonsoft.Json.Formatting.None);

                string url = BaseUrl + "/api/kham-chua-benh/" + action;
                int status;
                string body = HttpPostJsonWithRetry(url, json, ref token, out status);
                if (status == 0)
                { error = this.batchFatalError ?? ("không kết nối được cổng (" + BaseUrl + ") — kiểm tra mạng/firewall"); return null; }
                if (status == 404)
                { error = "Cổng KHÔNG có hồ sơ mã " + maLk + " thuộc đơn vị — hồ sơ chưa từng được tiếp nhận."; return null; }

                var jo = string.IsNullOrEmpty(body) ? null : Newtonsoft.Json.Linq.JObject.Parse(body);
                bool ok = status == HTTP_OK && jo != null && ((bool?)jo["success"] == true);
                string msg = (jo != null) ? ((string)jo["message"] ?? "") : "";
                string code = (jo != null) ? ((string)jo["code"] ?? "") : "";
                if (!ok)
                { error = "Cổng từ chối (HTTP " + status + " " + code + ") — " + (string.IsNullOrEmpty(msg) ? Cut(body, 300) : msg); return null; }
                var jdAction = jo["data"] as Newtonsoft.Json.Linq.JObject;
                string trackingId = (jdAction != null) ? (string)jdAction["tracking_id"] : null;
                return (restore ? "Đã KHÔI PHỤC" : "Đã HỦY") + " hồ sơ " + maLk + " trên cổng."
                    + (string.IsNullOrEmpty(trackingId) ? "" : (" Tracking: " + trackingId));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                error = ex.Message;
                return null;
            }
        }

        /// <summary>Render chi tiet trang-thai KCB (ho_so.status + tung lan gui + events + errors).</summary>
        private static VlgHoSoADO ParseKcbTrangThai(string maLk, string body)
        {
            var jo = Newtonsoft.Json.Linq.JObject.Parse(body);
            var jd = jo["data"] as Newtonsoft.Json.Linq.JObject;
            var hs = (jd != null) ? jd["ho_so"] : null;
            var ado = new VlgHoSoADO();
            var sb = new StringBuilder();
            ado.MaLk = (hs != null) ? ((string)hs["ma_lk"] ?? maLk) : maLk;
            ado.ValidationStatus = (hs != null) ? (string)hs["status"] : null;
            sb.AppendLine("HỒ SƠ KCB " + ado.MaLk + " — trạng thái: " + (ado.ValidationStatus ?? "(không rõ)"));
            sb.AppendLine();
            var requests = (jd != null) ? jd["requests"] as Newtonsoft.Json.Linq.JArray : null;
            if (requests != null && requests.Count > 0)
            {
                sb.AppendLine("CÁC LẦN GỬI (" + requests.Count + "):");
                bool first = true;
                foreach (var r in requests)
                {
                    sb.AppendLine("  ▶ " + (string)r["tracking_id"] + " | trạng thái: " + (string)r["status"]);
                    if (first)
                    {
                        ado.TrackingId = (string)r["tracking_id"];
                        ado.LatestStatus = (string)r["status"];
                        first = false;
                    }
                    AppendEventsAndErrors(sb, r);
                }
            }
            ado.DetailText = sb.ToString();
            return ado;
        }
        #endregion

        #region Nhom HSSK 831
        /// <summary>
        /// GET /api/ho-so-suc-khoe/qd-831-2017/ho-so — danh sach request HSSK cua don vi.
        /// maLk / maDinhDanh: loc them theo ma lien ket / ma dinh danh (bo trong = khong loc).
        /// </summary>
        internal List<VlgHoSoADO> GetHssk831List(DateTime fromDate, DateTime toDate, string errorStatus,
            string maLk, string maDinhDanh, out string error)
        {
            return GetPagedList("/api/ho-so-suc-khoe/qd-831-2017/ho-so",
                "?from_date=" + fromDate.ToString("yyyy-MM-dd")
                + "&to_date=" + toDate.ToString("yyyy-MM-dd")
                + (string.IsNullOrEmpty(errorStatus) ? "" : ("&error_status=" + errorStatus))
                + (string.IsNullOrWhiteSpace(maLk) ? "" : ("&ma_lk=" + Uri.EscapeDataString(maLk.Trim())))
                + (string.IsNullOrWhiteSpace(maDinhDanh) ? "" : ("&ma_dinh_danh=" + Uri.EscapeDataString(maDinhDanh.Trim()))),
                VlgHoSoADO.FromHssk831Item, out error);
        }

        /// <summary>GET /api/ho-so-suc-khoe/qd-831-2017/ho-so/trang-thai?tracking_id= — tien trinh + loi cua 1 request HSSK.</summary>
        internal VlgHoSoADO GetHssk831TrangThai(string trackingId, out bool found, out string error)
        {
            found = false; error = null;
            try
            {
                if (!this.configured) { error = "Chưa cấu hình khóa " + CONFIG_KEY; return null; }
                if (this.batchFatalError != null) { error = this.batchFatalError; return null; }
                if (string.IsNullOrWhiteSpace(trackingId)) { error = "Chưa có tracking_id."; return null; }
                string token = GetToken();
                if (string.IsNullOrEmpty(token))
                { error = "Đăng nhập cổng thất bại — " + (this.lastAuthError ?? ""); return null; }

                string url = BaseUrl + "/api/ho-so-suc-khoe/qd-831-2017/ho-so/trang-thai?tracking_id="
                    + Uri.EscapeDataString(trackingId.Trim());
                int status;
                string body = HttpGetWithRetry(url, ref token, out status);
                if (status == 0)
                { error = this.batchFatalError ?? ("không kết nối được cổng (" + BaseUrl + ") — kiểm tra mạng/firewall"); return null; }
                if (status == 404) { found = false; return null; }
                if (status != HTTP_OK || string.IsNullOrEmpty(body))
                { error = "Cổng trả lỗi (HTTP " + status + ") — " + Cut(body, 300); return null; }

                found = true;
                return ParseHssk831TrangThai(body);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                error = ex.Message;
                return null;
            }
        }

        /// <summary>Render chi tiet trang-thai HSSK 831 (status + timestamps + events + errors).</summary>
        private static VlgHoSoADO ParseHssk831TrangThai(string body)
        {
            var jo = Newtonsoft.Json.Linq.JObject.Parse(body);
            var data = jo["data"] as Newtonsoft.Json.Linq.JObject;
            var ado = new VlgHoSoADO();
            var sb = new StringBuilder();
            if (data != null)
            {
                ado.TrackingId = (string)data["tracking_id"];
                ado.ValidationStatus = (string)data["status"];
                ado.LatestReceivedText = VlgHoSoADO.IsoToTimeText((string)data["received_at"]);
                sb.AppendLine("REQUEST HSSK 831 " + ado.TrackingId);
                sb.AppendLine("Trạng thái: " + ado.ValidationStatus
                    + " | nhận: " + ado.LatestReceivedText
                    + " | xong: " + VlgHoSoADO.IsoToTimeText((string)data["completed_at"]));
                sb.AppendLine();
                AppendEventsAndErrors(sb, data);
            }
            ado.DetailText = sb.ToString();
            return ado;
        }
        #endregion

        /// <summary>
        /// Lat het trang 1 API danh sach (items[] + total) — dung chung cho KSK/KCB/HSSK.
        /// LastListTruncated bat khi cham MAX_PAGES.
        /// </summary>
        private List<VlgHoSoADO> GetPagedList(string path, string query,
            Func<Newtonsoft.Json.Linq.JToken, VlgHoSoADO> mapItem, out string error)
        {
            error = null;
            this.LastListTruncated = false;
            var result = new List<VlgHoSoADO>();
            try
            {
                if (!this.configured) { error = "Chưa cấu hình khóa " + CONFIG_KEY; return result; }
                if (this.batchFatalError != null) { error = this.batchFatalError; return result; }
                string token = GetToken();
                if (string.IsNullOrEmpty(token))
                { error = "Đăng nhập cổng thất bại — " + (this.lastAuthError ?? ""); return result; }

                int page = 1, total = -1;
                while (true)
                {
                    string url = BaseUrl + path + query + "&page=" + page + "&page_size=" + PAGE_SIZE;
                    int status;
                    string body = HttpGetWithRetry(url, ref token, out status);
                    if (status == 0)
                    { error = this.batchFatalError ?? ("không kết nối được cổng (" + BaseUrl + ") — kiểm tra mạng/firewall"); return result; }
                    if (status != HTTP_OK || string.IsNullOrEmpty(body))
                    { error = "Cổng trả lỗi (HTTP " + status + ") — " + Cut(body, 300); return result; }

                    var jo = Newtonsoft.Json.Linq.JObject.Parse(body);
                    var data = jo["data"] as Newtonsoft.Json.Linq.JObject;
                    if (data == null) break;
                    if (total < 0) total = ToInt(data["total"]);
                    var items = data["items"] as Newtonsoft.Json.Linq.JArray;
                    if (items == null || items.Count == 0) break;
                    foreach (var it in items) result.Add(mapItem(it));
                    if ((total > 0 && result.Count >= total) || items.Count < PAGE_SIZE) break;
                    if (page >= MAX_PAGES) { this.LastListTruncated = true; break; }
                    page++;
                }
                return result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                error = ex.Message;
                return result;
            }
        }

        /// <summary>Ghep events[] + errors[] cua 1 node (request KCB / data HSSK) vao chi tiet.</summary>
        private static void AppendEventsAndErrors(StringBuilder sb, Newtonsoft.Json.Linq.JToken node)
        {
            try
            {
                var events = node["events"] as Newtonsoft.Json.Linq.JArray;
                if (events != null && events.Count > 0)
                {
                    foreach (var ev in events)
                        sb.AppendLine("     • " + (string)ev["event_type"]
                            + ((string)ev["to_status"] != null ? (" → " + (string)ev["to_status"]) : "")
                            + ": " + (string)ev["message"]);
                }
                var errs = node["errors"] as Newtonsoft.Json.Linq.JArray;
                if (errs != null && errs.Count > 0)
                {
                    foreach (var e in errs)
                        sb.AppendLine("     [" + ((string)e["severity"] ?? "LỖI") + "] " + (string)e["code"]
                            + (((string)e["field_path"]) != null ? (" (" + (string)e["field_path"] + ")") : "")
                            + ": " + (string)e["message"]);
                }
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        /// <summary>Parse response trang-thai -> ADO (tom tat + DetailText nhieu dong cho memo).</summary>
        private static VlgHoSoADO ParseTrangThai(string body)
        {
            var jo = Newtonsoft.Json.Linq.JObject.Parse(body);
            var jd = jo["data"] as Newtonsoft.Json.Linq.JObject;
            var hs = (jd != null) ? jd["ho_so"] : null;
            var ado = new VlgHoSoADO();
            var sb = new StringBuilder();
            if (hs != null)
            {
                ado.MaLk = (string)hs["ma_lk"];
                ado.HoTen = (string)hs["ho_ten"];
                ado.SoCccd = (string)hs["so_cccd"];
                ado.NgayKhamText = VlgHoSoADO.IsoToDateText((string)hs["ngay_kham"]);
                ado.FormName = (string)hs["form_name"];
                ado.ValidationStatus = (string)hs["validation_status"];
                sb.AppendLine("HỒ SƠ " + ado.MaLk + " — " + ado.HoTen
                    + " — mẫu phiếu: " + (ado.FormName ?? "(chưa phân loại)"));
                sb.AppendLine("KẾT QUẢ KIỂM TRA: " + ado.ValidationStatus + " — " + (string)hs["message"]);
                sb.AppendLine();
            }
            var requests = (jd != null) ? jd["requests"] as Newtonsoft.Json.Linq.JArray : null;
            if (requests != null)
            {
                // Sap xep moi nhat truoc (received_at ISO — so sanh chuoi Ordinal la du).
                var sorted = new List<Newtonsoft.Json.Linq.JToken>();
                foreach (var r in requests) sorted.Add(r);
                sorted.Sort((a, b) => string.CompareOrdinal((string)b["received_at"] ?? "", (string)a["received_at"] ?? ""));
                sb.AppendLine("CÁC LẦN GỬI (" + sorted.Count + " — mới nhất trước):");
                bool first = true;
                foreach (var r in sorted)
                {
                    sb.AppendLine("  ▶ " + (string)r["tracking_id"]);
                    sb.AppendLine("     gửi lúc: " + VlgHoSoADO.IsoToTimeText((string)r["received_at"])
                        + " | trạng thái: " + (string)r["status"]
                        + " | kiểm tra: " + (string)r["validation_status"]);
                    if (first)
                    {
                        ado.TrackingId = (string)r["tracking_id"];
                        ado.LatestStatus = (string)r["status"];
                        ado.LatestReceivedText = VlgHoSoADO.IsoToTimeText((string)r["received_at"]);
                        first = false;
                    }
                    var errs = r["errors"] as Newtonsoft.Json.Linq.JArray;
                    if (errs != null && errs.Count > 0)
                    {
                        int nErr = 0, nWarn = 0;
                        foreach (var e in errs)
                        {
                            if (string.Equals((string)e["severity"], "ERROR", StringComparison.OrdinalIgnoreCase)) nErr++;
                            else nWarn++;
                        }
                        sb.AppendLine("     lỗi: " + nErr + " | cảnh báo: " + nWarn);
                        foreach (var e in errs)
                        {
                            if (!string.Equals((string)e["severity"], "ERROR", StringComparison.OrdinalIgnoreCase)) continue;
                            sb.AppendLine("       [LỖI] " + (string)e["code"] + " (" + (string)e["field_path"] + "): "
                                + (string)e["message"]);
                        }
                    }
                }
            }
            ado.DetailText = sb.ToString();
            return ado;
        }

        #region token + http
        private string GetToken()
        {
            // Lock de 2 worker (check ket noi luc Load + Tim kiem) khong dang nhap trung nhau
            // — cong co rate-limit dang nhap (429).
            lock (this.tokenSync)
            {
            if (!string.IsNullOrWhiteSpace(this.cachedToken) && DateTime.Now < this.tokenExpireAt)
                return this.cachedToken;
            this.lastAuthError = null;
            try
            {
                string loginJson = Newtonsoft.Json.JsonConvert.SerializeObject(
                    new { username = this.username, password = this.password });
                int status;
                string body = HttpSend("POST", BaseUrl + TOKEN_PATH, "application/json; charset=utf-8",
                    Encoding.UTF8.GetBytes(loginJson), null, out status);
                if (status == 0)
                {
                    this.lastAuthError = "không kết nối được cổng (" + BaseUrl + ")";
                    this.batchFatalError = this.lastAuthError;
                    return null;
                }
                var jo = string.IsNullOrEmpty(body) ? null : Newtonsoft.Json.Linq.JObject.Parse(body);
                // Body loi dang {"data": null}: jo["data"] la JValue(Null) KHAC null reference —
                // index tiep se nem InvalidOperationException va lam truot khoi latch 401/403/429.
                var jd = (jo != null) ? jo["data"] as Newtonsoft.Json.Linq.JObject : null;
                string token = (jd != null) ? (string)jd["access_token"] : null;
                if (status != HTTP_OK || string.IsNullOrEmpty(token))
                {
                    this.lastAuthError = "HTTP " + status
                        + ((jo != null) ? (" " + (string)jo["code"] + " — " + (string)jo["message"]) : "");
                    // Sai tai khoan/khoa/rate-limit khong tu het -> chan cac call sau cua lo.
                    if (status == HTTP_UNAUTHORIZED || status == 403 || status == 429)
                        this.batchFatalError = "đăng nhập cổng thất bại — " + this.lastAuthError;
                    return null;
                }
                long ttl = TOKEN_TTL_DEFAULT_SECONDS;
                if (jd["expires_in"] != null) ttl = ToInt(jd["expires_in"]);
                if (ttl <= 0) ttl = TOKEN_TTL_DEFAULT_SECONDS;
                this.cachedToken = token;
                this.tokenExpireAt = DateTime.Now.AddSeconds(Math.Max(60, ttl - TOKEN_TTL_SAFETY_SECONDS));
                return this.cachedToken;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                this.lastAuthError = ex.Message;
                return null;
            }
            }
        }

        /// <summary>GET kem retry 401 dung 1 lan (token het han giua lo); status 0 -> latch batchFatalError.</summary>
        private string HttpGetWithRetry(string url, ref string token, out int status)
        {
            string body = HttpGet(url, token, out status);
            if (status == HTTP_UNAUTHORIZED)
            {
                this.cachedToken = null; this.tokenExpireAt = DateTime.MinValue;
                token = GetToken();
                if (!string.IsNullOrEmpty(token)) body = HttpGet(url, token, out status);
            }
            if (status == 0 && this.batchFatalError == null)
                this.batchFatalError = "không kết nối được cổng (" + BaseUrl + ") — kiểm tra mạng/firewall";
            return body;
        }

        /// <summary>POST JSON kem retry 401 dung 1 lan; status 0 -> latch batchFatalError (nhu GET).</summary>
        private string HttpPostJsonWithRetry(string url, string json, ref string token, out int status)
        {
            byte[] bodyBytes = Encoding.UTF8.GetBytes(json ?? "");
            string body = HttpSend("POST", url, "application/json; charset=utf-8", bodyBytes, token, out status);
            if (status == HTTP_UNAUTHORIZED)
            {
                this.cachedToken = null; this.tokenExpireAt = DateTime.MinValue;
                token = GetToken();
                if (!string.IsNullOrEmpty(token))
                    body = HttpSend("POST", url, "application/json; charset=utf-8", bodyBytes, token, out status);
            }
            if (status == 0 && this.batchFatalError == null)
                this.batchFatalError = "không kết nối được cổng (" + BaseUrl + ") — kiểm tra mạng/firewall";
            return body;
        }

        /// <summary>12 hex dau SHA256 — dung sinh MA_YEU_CAU idempotency (giong KskVlgPusher).</summary>
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

        private static string HttpGet(string url, string bearerToken, out int statusCode)
        {
            return HttpSend("GET", url, null, null, bearerToken, out statusCode);
        }

        private static string HttpSend(string method, string url, string contentType, byte[] bodyBytes,
            string bearerToken, out int statusCode)
        {
            statusCode = 0;
            var request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);
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
                    Inventec.Common.Logging.LogSystem.Error("VlgPortal: khong ket noi duoc " + url + " — " + wex.Message, wex);
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
        #endregion

        #region helper
        private static string Get(string[] arr, int index)
        {
            return (arr != null && index < arr.Length && arr[index] != null) ? arr[index].Trim() : null;
        }

        private static int ToInt(Newtonsoft.Json.Linq.JToken token)
        {
            try { return (token == null) ? 0 : (int)token; }
            catch { return 0; }
        }

        private static string Cut(string s, int max)
        {
            if (string.IsNullOrEmpty(s)) return "";
            return (s.Length <= max) ? s : s.Substring(0, max) + "...";
        }
        #endregion
    }
}
