/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 */
using Inventec.Common.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ExportXmlQD130.Base
{
    /// <summary>
    /// Worker đồng bộ dữ liệu KCB (Kết thúc khám/Xuất viện) lên CỔNG TIẾP NHẬN — Kho dữ liệu
    /// y tế tỉnh Vĩnh Long: POST /api/kham-chua-benh/hoan-tat, body = XML GIAMDINHHS trực tiếp
    /// (đúng file XML QĐ 130/4210 plugin đang sinh — NOIDUNGFILE đã Base64 sẵn trong batch).
    /// Đăng nhập POST /api/xac-thuc/token (JSON) -&gt; data.access_token (hạn expires_in giây).
    /// Cổng tiếp nhận BẤT ĐỒNG BỘ: ACCEPTED/QUEUED = đã vào hàng đợi, tra kết quả xử lý thật
    /// bằng màn "Tra cứu Cổng KDLYT Vĩnh Long" (loại hồ sơ KCB) theo MA_LK = mã điều trị.
    /// Khóa cấu hình DÙNG CHUNG với liên thông KSK VLG:
    ///   MOS.HIS_KSK_SYNC.VLG_2062_CONNECTION_INFO = MaDonVi|Username|Password|TokenUrl|PushUrl
    /// (2 URL bỏ trống = cổng chính thức; PushUrl là API KSK nên KHÔNG dùng ở đây — chỉ lấy
    /// BaseUrl suy từ TokenUrl, đường dẫn hoan-tat cố định theo tài liệu).
    /// </summary>
    public class VlgKcbHoanTatWorker
    {
        private const string DEFAULT_BASE_URL = "https://congtiepnhan.kdlyt.vinhlong.vn";
        private const string TOKEN_PATH = "/api/xac-thuc/token";
        private const string PUSH_PATH = "/api/kham-chua-benh/hoan-tat";
        private const int TOKEN_SAFETY_MARGIN_SECOND = 60;
        private const long TOKEN_TTL_DEFAULT_SECONDS = 10800;
        private const int HTTP_TIMEOUT_SECOND = 60;
        private const int MAX_BODY_BYTES = 10 * 1024 * 1024;   //tài liệu: body tối đa 10 MiB

        private readonly string maDonVi;
        private readonly string username;
        private readonly string password;
        private readonly string baseUrl;

        private string token;
        private DateTime tokenExpireTime = DateTime.MinValue;
        //Lỗi không tự hết giữa lô (mất mạng, sai tài khoản, rate-limit) -> các hồ sơ sau trả lỗi ngay.
        private string batchFatalError;

        //Nối tuần tự các lần đẩy trên cùng worker (nhiều hồ sơ đẩy song song) — tránh đua token/đăng nhập trùng.
        private readonly System.Threading.SemaphoreSlim pushGate = new System.Threading.SemaphoreSlim(1, 1);

        /// <summary>True khi khóa cấu hình VLG có đủ MaDonVi | Username | Password.</summary>
        public bool IsValidConfig { get; private set; }

        public VlgKcbHoanTatWorker(string connectionInfo)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(connectionInfo))
                {
                    //MaDonVi|Username|Password|TokenUrl|PushUrl
                    string[] parts = connectionInfo.Split('|');
                    this.maDonVi = GetPart(parts, 0);
                    this.username = GetPart(parts, 1);
                    this.password = GetPart(parts, 2);
                    string tokenUrl = GetPart(parts, 3);
                    //BaseUrl suy từ TokenUrl (bỏ đuôi /api/xac-thuc/token); không khớp dạng -> cổng chính thức.
                    this.baseUrl = DEFAULT_BASE_URL;
                    string t = string.IsNullOrWhiteSpace(tokenUrl) ? "" : tokenUrl.Trim().TrimEnd('/');
                    int idx = t.Length - TOKEN_PATH.Length;
                    if (idx > 0 && string.Compare(t, idx, TOKEN_PATH, 0, TOKEN_PATH.Length,
                            StringComparison.OrdinalIgnoreCase) == 0)
                        this.baseUrl = t.Substring(0, idx);
                }

                this.IsValidConfig = !string.IsNullOrEmpty(this.maDonVi)
                    && !string.IsNullOrEmpty(this.username)
                    && !string.IsNullOrEmpty(this.password);

                if (!this.IsValidConfig)
                {
                    LogSystem.Warn("VlgKcbHoanTatWorker - Khoa MOS.HIS_KSK_SYNC.VLG_2062_CONNECTION_INFO khong hop le. Can dinh dang: MaDonVi|Username|Password|TokenUrl|PushUrl");
                }

                //Bật TLS 1.2 (.NET 4.5 chưa bật mặc định)
                try { ServicePointManager.SecurityProtocol |= (SecurityProtocolType)3072; }
                catch (Exception exTls) { LogSystem.Warn(exTls); }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                this.IsValidConfig = false;
            }
        }

        /// <summary>
        /// Đẩy 1 hồ sơ hoàn tất KCB (XML GIAMDINHHS) lên cổng tiếp nhận. Tự lấy lại token khi hết hạn/401.
        /// </summary>
        /// <param name="xmlBytes">Nội dung file XML GIAMDINHHS (QĐ 130/4210) nguyên bản.</param>
        /// <param name="maLk">Mã điều trị (ghi log + tra cứu sau này trên cổng theo MA_LK).</param>
        public async Task<Csdl4750ImportResult> ImportXmlAsync(byte[] xmlBytes, string maLk)
        {
            Csdl4750ImportResult ret = new Csdl4750ImportResult();
            try
            {
                if (!this.IsValidConfig)
                {
                    ret.Message = "Khóa cấu hình Cổng tiếp nhận VLG không hợp lệ";
                    return ret;
                }
                if (xmlBytes == null || xmlBytes.Length == 0)
                {
                    ret.Message = "Không có dữ liệu XML để đồng bộ";
                    LogSystem.Warn("VlgKcbHoanTatWorker - Khong co du lieu XML. MA_LK: " + maLk);
                    return ret;
                }
                if (xmlBytes.Length > MAX_BODY_BYTES)
                {
                    ret.Message = "Bản tin vượt giới hạn 10 MiB của cổng (" + xmlBytes.Length + " byte)";
                    return ret;
                }
                await this.pushGate.WaitAsync();
                try
                {
                    if (this.batchFatalError != null)
                    {
                        ret.Message = this.batchFatalError;
                        return ret;
                    }
                    if (!await EnsureTokenAsync())
                    {
                        ret.Message = this.batchFatalError ?? "Đăng nhập cổng tiếp nhận VLG thất bại";
                        return ret;
                    }

                    PushResult result = await PostHoanTatAsync(xmlBytes, maLk);
                    //Token hết hạn giữa lô (401) -> lấy lại token và gửi lại 1 lần
                    if (result.Unauthorized)
                    {
                        LogSystem.Info("VlgKcbHoanTatWorker - Token bi tu choi (401), lay lai token va gui lai. MA_LK: " + maLk);
                        this.token = null; this.tokenExpireTime = DateTime.MinValue;
                        if (!await LoginAsync())
                        {
                            ret.Message = this.batchFatalError ?? "Đăng nhập lại cổng thất bại (401)";
                            return ret;
                        }
                        result = await PostHoanTatAsync(xmlBytes, maLk);
                    }

                    ret.Success = result.Ok;
                    ret.Message = result.Message;
                    LogSystem.Info("VlgKcbHoanTatWorker - Dong bo hoan-tat VLG MA_LK: " + maLk
                        + " thanh cong: " + result.Ok + ". Message: " + result.Message);
                    return ret;
                }
                finally { this.pushGate.Release(); }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                ret.Success = false;
                ret.Message = "Lỗi ngoại lệ khi gửi: " + ex.Message;
                return ret;
            }
        }

        private async Task<bool> EnsureTokenAsync()
        {
            if (!string.IsNullOrEmpty(this.token) && DateTime.Now < this.tokenExpireTime)
                return true;
            return await LoginAsync();
        }

        /// <summary>POST /api/xac-thuc/token (JSON username/password) -&gt; data.access_token + expires_in giây.</summary>
        private async Task<bool> LoginAsync()
        {
            try
            {
                this.token = null;
                this.tokenExpireTime = DateTime.MinValue;

                string loginJson = Newtonsoft.Json.JsonConvert.SerializeObject(
                    new { username = this.username, password = this.password });
                using (HttpClient client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(HTTP_TIMEOUT_SECOND);
                    var content = new StringContent(loginJson, Encoding.UTF8, "application/json");
                    //KHÔNG log body đăng nhập (chứa mật khẩu).
                    LogSystem.Info("VlgKcbHoanTatWorker - Login: " + this.baseUrl + TOKEN_PATH + "; username=" + this.username);
                    HttpResponseMessage response;
                    try { response = await client.PostAsync(this.baseUrl + TOKEN_PATH, content); }
                    catch (Exception exNet)
                    {
                        this.batchFatalError = "không kết nối được cổng tiếp nhận (" + this.baseUrl + ") — " + exNet.Message;
                        LogSystem.Error("VlgKcbHoanTatWorker - " + this.batchFatalError, exNet);
                        return false;
                    }
                    string body = await response.Content.ReadAsStringAsync();
                    JObject json = null;
                    try { json = string.IsNullOrEmpty(body) ? null : JObject.Parse(body); }
                    catch { }
                    //Body lỗi dạng {"data": null}: json["data"] là JValue(Null) khác null reference —
                    //index tiếp sẽ ném exception và làm trượt khối latch 401/403/429 bên dưới.
                    JObject jd = (json != null) ? json["data"] as JObject : null;
                    string tk = (jd != null) ? (string)jd["access_token"] : null;
                    if (!response.IsSuccessStatusCode || string.IsNullOrEmpty(tk))
                    {
                        string reason = "HTTP " + (int)response.StatusCode
                            + ((json != null) ? (" " + (string)json["code"] + " — " + (string)json["message"]) : "");
                        //Sai tài khoản/khóa/rate-limit không tự hết -> chặn các hồ sơ sau của lô.
                        int sc = (int)response.StatusCode;
                        if (sc == 401 || sc == 403 || sc == 429)
                            this.batchFatalError = "đăng nhập cổng tiếp nhận thất bại — " + reason;
                        LogSystem.Warn("VlgKcbHoanTatWorker - Dang nhap that bai. " + reason);
                        return false;
                    }
                    long ttl = TOKEN_TTL_DEFAULT_SECONDS;
                    try { if (jd["expires_in"] != null) ttl = (long)jd["expires_in"]; }
                    catch { }
                    if (ttl <= 0) ttl = TOKEN_TTL_DEFAULT_SECONDS;
                    this.token = tk;
                    this.tokenExpireTime = DateTime.Now.AddSeconds(Math.Max(60, ttl - TOKEN_SAFETY_MARGIN_SECOND));
                    LogSystem.Info("VlgKcbHoanTatWorker - Dang nhap thanh cong. Token het han: "
                        + this.tokenExpireTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    return true;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                return false;
            }
        }

        /// <summary>POST hoan-tat: body XML trực tiếp (Content-Type application/xml), Bearer token.</summary>
        private async Task<PushResult> PostHoanTatAsync(byte[] xmlBytes, string maLk)
        {
            PushResult result = new PushResult();
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(HTTP_TIMEOUT_SECOND);
                    var content = new ByteArrayContent(xmlBytes);
                    content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/xml");
                    var request = new HttpRequestMessage(HttpMethod.Post, this.baseUrl + PUSH_PATH) { Content = content };
                    request.Headers.TryAddWithoutValidation("Authorization", "Bearer " + (this.token ?? ""));

                    HttpResponseMessage response;
                    try { response = await client.SendAsync(request); }
                    catch (Exception exNet)
                    {
                        this.batchFatalError = "không kết nối được cổng tiếp nhận (" + this.baseUrl + ") — " + exNet.Message;
                        result.Ok = false;
                        result.Message = this.batchFatalError;
                        LogSystem.Error("VlgKcbHoanTatWorker - " + this.batchFatalError, exNet);
                        return result;
                    }
                    string body = await response.Content.ReadAsStringAsync();
                    int status = (int)response.StatusCode;
                    if (status == 401)
                    {
                        result.Unauthorized = true;
                        result.Message = "401 - Token bị từ chối";
                        return result;
                    }

                    JObject j = null;
                    try { j = string.IsNullOrEmpty(body) ? null : JObject.Parse(body); }
                    catch { }
                    string code = (j != null) ? ((string)j["code"] ?? "") : "";
                    string message = (j != null) ? ((string)j["message"] ?? "") : Cut(body, 500);
                    //{"data": null} -> cast JObject, tránh InvalidOperationException nuốt mất code/message thật.
                    JObject jd = (j != null) ? j["data"] as JObject : null;
                    string trackingId = (jd != null) ? (string)jd["tracking_id"] : null;
                    //ACCEPTED / ACCEPTED_WITH_WARNING / ACCEPTED_DUPLICATE = cổng đã tiếp nhận vào hàng đợi.
                    bool accepted = response.IsSuccessStatusCode && (j != null)
                        && ((bool?)j["success"] == true)
                        && code.StartsWith("ACCEPTED", StringComparison.OrdinalIgnoreCase);

                    LogSystem.Info(string.Format(
                        "VlgKcbHoanTatWorker - [KET QUA] MA_LK: {0}; HTTP {1}; code={2}; tracking={3}; message={4}",
                        maLk, status, code, trackingId, Cut(message, 300)));

                    if (accepted)
                    {
                        result.Ok = true;
                        result.Message = "VLG: đã tiếp nhận (" + code
                            + (string.IsNullOrEmpty(trackingId) ? "" : (" — " + trackingId))
                            + ") — kết quả xử lý tra ở màn Tra cứu Cổng KDLYT Vĩnh Long";
                    }
                    else
                    {
                        result.Ok = false;
                        result.Message = "VLG: cổng từ chối (HTTP " + status + " " + code + ") — " + Cut(message, 500);
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                result.Ok = false;
                result.Message = "Lỗi ngoại lệ khi gửi: " + ex.Message;
                return result;
            }
        }

        private static string GetPart(string[] arr, int index)
        {
            return (arr != null && index < arr.Length && arr[index] != null) ? arr[index].Trim() : null;
        }

        private static string Cut(string s, int max)
        {
            if (string.IsNullOrEmpty(s)) return "";
            s = s.Trim();
            return (s.Length <= max) ? s : s.Substring(0, max) + "...";
        }

        private class PushResult
        {
            public bool Ok { get; set; }
            public bool Unauthorized { get; set; }
            public string Message { get; set; }
        }
    }
}
