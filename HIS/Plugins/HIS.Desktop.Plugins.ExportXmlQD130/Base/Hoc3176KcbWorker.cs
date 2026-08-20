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
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ExportXmlQD130.Base
{
    /// <summary>
    /// Worker day du lieu Kham chua benh len TRUNG TAM DIEU HANH Y TE (cong du lieu y te - HOC)
    /// theo Quyet dinh 3176/QD-BYT. Hai diem gui:
    ///   - Ban tin trang thai KCB (check-in): POST {base}/hoc-130-ck/checkin
    ///   - Ho so KCB (XML1..XML15):           POST {base}/hoc-130/khamchuabenh130
    /// Xac thuc Keycloak: POST {tokenUrl} dang application/x-www-form-urlencoded
    /// (client_id, username, password, grant_type) -> access_token + expires_in (giay).
    /// Moi lan gui kem header 'tinh' / 'csyt' theo quy uoc cua cong.
    ///
    /// KHOA CAU HINH DUNG CHUNG voi lien thong KSK cua CUNG cong nay (cung tai khoan, cung cong):
    ///   MOS.HIS_KSK_SYNC.HSSK_HOC_2062_CONNECTION_INFO
    ///   = MaCsyt|Username|Password|ClientId|MaTinh|GrantType|TokenUrl|PushUrl|PrivateKey
    /// 5 truong dau BAT BUOC. TokenUrl bo trong -> mac dinh cong.
    /// PushUrl (parts[7]) la duong gui cua KSK -> KHONG dung o day: 2 duong gui cua KCB duoc
    /// CO DINH trong ma nguon (hang so RECEIVER_BASE) theo yeu cau giai doan nay.
    ///
    /// Worker KHONG goi va KHONG sua thu vien lien thong KSK -> vien dang dong bo KSK
    /// khong phai test hoi quy (xem PTTK_TBD_Lien_Thong_KCB_3176_Trung_Tam_Dieu_Hanh_Y_Te muc 3.2).
    /// Ket qua gui KHONG luu xuong CSDL o giai doan nay: caller hien cua so ket qua + ghi nhat ky.
    /// </summary>
    public class Hoc3176KcbWorker
    {
        //Duong dan mac dinh theo tai lieu dac ta Trung tam dieu hanh y te
        private const string DEFAULT_TOKEN_URL = "https://ptsso.vncare.vn/auth/realms/hsskv3/protocol/openid-connect/token";
        //Duong gui CO DINH trong ma nguon (KHONG lay tu truong PushUrl cua khoa cau hinh - PushUrl la
        //duong gui cua lien thong KSK, khong dung cho KCB). Doi moi truong thu/that -> sua o day.
        private const string RECEIVER_BASE = "https://mocapi.congdulieuyte.vn/hoc-receiver/api/receiver";
        private const string PATH_CHECKIN = "hoc-130-ck/checkin";
        private const string PATH_KCB = "hoc-130/khamchuabenh130";
        private const string DEFAULT_GRANT_TYPE = "password";

        //Cong tra ve { code:200, message:"success", result:"{ma giao dich}" }
        private const int SUCCESS_CODE = 200;
        private const string SUCCESS_MESSAGE = "success";

        //Tru hao vai giay de chu dong lay lai token truoc khi het han
        private const int TOKEN_SAFETY_MARGIN_SECOND = 60;
        //Cong khong tra expires_in -> gioi han cache 15 phut (bang tran cache cua lien thong KSK cung cong)
        private const long TOKEN_TTL_DEFAULT_SECONDS = 900;
        private const int HTTP_TIMEOUT_SECOND = 60;

        /// <summary>
        /// Cach goi than ban tin cho 2 diem gui cua KCB.
        /// false = XML nguyen ban (Content-Type application/xml) - theo vi du lenh goi trong tai lieu doi tac.
        /// true  = XML ma hoa Base64 dat trong than JSON { ma_tinh, ma_csyt, du_lieu_chi_tiet } - dung
        ///         kieu ma cong nay dang nhan du lieu KSK.
        /// Tai lieu doi tac ghi KHONG THONG NHAT giua 2 muc (cau hoi Q11 trong PTTK) -> de hang so o day
        /// de doi 1 dong khi co phan hoi chinh thuc.
        /// </summary>
        private static readonly bool USE_JSON_BASE64_BODY = false;

        private readonly string maCsyt;
        private readonly string username;
        private readonly string password;
        private readonly string clientId;
        private readonly string maTinh;
        //Gia tri THUC SU gui o header 'csyt' - xem GetMaCsytHeader()
        private readonly string maCsytHeader;
        private readonly string grantType;
        private readonly string tokenUrl;

        private string token;
        private DateTime tokenExpireTime = DateTime.MinValue;
        //Loi khong tu het giua lo (sai tai khoan, mat mang, rate-limit) -> cac ho so sau tra loi ngay,
        //khong dap cong vo ich.
        private string batchFatalError;

        //Noi tuan tu cac lan gui tren cung worker (nhieu ho so day song song) - tranh dua token.
        private readonly System.Threading.SemaphoreSlim pushGate = new System.Threading.SemaphoreSlim(1, 1);

        /// <summary>True khi khoa cau hinh co du 5 truong bat buoc dau.</summary>
        public bool IsValidConfig { get; private set; }

        public Hoc3176KcbWorker(string connectionInfo)
        {
            try
            {
                this.grantType = DEFAULT_GRANT_TYPE;
                this.tokenUrl = DEFAULT_TOKEN_URL;

                if (!string.IsNullOrWhiteSpace(connectionInfo))
                {
                    //MaCsyt|Username|Password|ClientId|MaTinh|GrantType|TokenUrl|PushUrl|PrivateKey
                    string[] parts = connectionInfo.Split('|');
                    this.maCsyt = GetPart(parts, 0);
                    this.username = GetPart(parts, 1);
                    this.password = GetPart(parts, 2);
                    this.clientId = GetPart(parts, 3);
                    this.maTinh = GetPart(parts, 4);

                    string cfgGrantType = GetPart(parts, 5);
                    if (!string.IsNullOrWhiteSpace(cfgGrantType)) this.grantType = cfgGrantType;

                    string cfgTokenUrl = GetPart(parts, 6);
                    if (!string.IsNullOrWhiteSpace(cfgTokenUrl)) this.tokenUrl = cfgTokenUrl;

                    //parts[7] = PushUrl: duong gui cua lien thong KSK -> KHONG dung cho KCB (duong gui KCB
                    //co dinh o RECEIVER_BASE). parts[8] = PrivateKey: cong hien khong yeu cau ky request.
                }

                this.maCsytHeader = GetMaCsytHeader(this.maCsyt, this.username);

                this.IsValidConfig = !string.IsNullOrEmpty(this.maCsyt)
                    && !string.IsNullOrEmpty(this.username)
                    && !string.IsNullOrEmpty(this.password)
                    && !string.IsNullOrEmpty(this.clientId)
                    && !string.IsNullOrEmpty(this.maTinh);

                if (!this.IsValidConfig)
                {
                    LogSystem.Warn("Hoc3176KcbWorker - Khoa MOS.HIS_KSK_SYNC.HSSK_HOC_2062_CONNECTION_INFO khong hop le. Can dinh dang: MaCsyt|Username|Password|ClientId|MaTinh|GrantType|TokenUrl|PushUrl|PrivateKey");
                }
                else
                {
                    LogSystem.Info("Hoc3176KcbWorker - Cau hinh OK. csyt(header)=" + this.maCsytHeader
                        + (this.maCsytHeader == this.maCsyt ? " (lay tu khoa)" : " (lay tu Username - truong MaCsyt cua khoa la '" + this.maCsyt + "', khong phai ma CSKCB 5 ky tu)")
                        + "; tinh=" + this.maTinh
                        + "; tokenUrl=" + this.tokenUrl + "; receiverBase=" + RECEIVER_BASE + " (co dinh trong ma nguon)");
                }

                //Bat TLS 1.2 (.NET Framework 4.5 chua bat mac dinh)
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
        /// Gui ban tin trang thai KCB (check-in) cua MOT ho so.
        /// </summary>
        /// <param name="xmlBytes">Noi dung XML CHI_TIEU_TRANG_THAI_KCB nguyen ban.</param>
        /// <param name="maLk">Ma dieu tri - dung de ghi nhat ky va hien ket qua.</param>
        public async Task<Hoc3176PushResult> PushCheckInAsync(byte[] xmlBytes, string maLk)
        {
            return await PushAsync(PATH_CHECKIN, "Check-in", xmlBytes, maLk);
        }

        /// <summary>
        /// Gui ho so KCB (XML GIAMDINHHS theo QD 3176) cua MOT ho so.
        /// </summary>
        public async Task<Hoc3176PushResult> PushKcbAsync(byte[] xmlBytes, string maLk)
        {
            return await PushAsync(PATH_KCB, "Ho so KCB", xmlBytes, maLk);
        }

        private async Task<Hoc3176PushResult> PushAsync(string path, string tag, byte[] xmlBytes, string maLk)
        {
            Hoc3176PushResult ret = new Hoc3176PushResult();
            try
            {
                if (!this.IsValidConfig)
                {
                    ret.Message = "Cấu hình kết nối Trung tâm điều hành y tế không hợp lệ";
                    return ret;
                }
                if (xmlBytes == null || xmlBytes.Length == 0)
                {
                    ret.Message = "Không có dữ liệu XML để gửi";
                    LogSystem.Warn("Hoc3176KcbWorker - Khong co du lieu XML. " + tag + ". MA_LK: " + maLk);
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
                        ret.Message = this.batchFatalError ?? "Đăng nhập Trung tâm điều hành y tế thất bại";
                        return ret;
                    }

                    PushResult result = await PostAsync(path, tag, xmlBytes, maLk);
                    //Token bi tu choi giua lo -> lay lai token va gui lai dung 1 lan
                    if (result.Unauthorized)
                    {
                        LogSystem.Info("Hoc3176KcbWorker - Token bi tu choi (401), lay lai token va gui lai. " + tag + ". MA_LK: " + maLk);
                        this.token = null;
                        this.tokenExpireTime = DateTime.MinValue;
                        if (!await LoginAsync())
                        {
                            ret.Message = this.batchFatalError ?? "Đăng nhập lại thất bại (401)";
                            return ret;
                        }
                        result = await PostAsync(path, tag, xmlBytes, maLk);
                    }

                    ret.Success = result.Ok;
                    ret.Message = result.Message;
                    ret.TransactionCode = result.TransactionCode;
                    LogSystem.Info("Hoc3176KcbWorker - " + tag + " MA_LK: " + maLk
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

        /// <summary>
        /// Lay phien lam viec: POST {tokenUrl} dang x-www-form-urlencoded
        /// (client_id, username, password, grant_type) -> access_token + expires_in (giay).
        /// </summary>
        private async Task<bool> LoginAsync()
        {
            try
            {
                this.token = null;
                this.tokenExpireTime = DateTime.MinValue;

                using (HttpClient client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(HTTP_TIMEOUT_SECOND);
                    var form = new List<KeyValuePair<string, string>>();
                    form.Add(new KeyValuePair<string, string>("client_id", this.clientId ?? ""));
                    form.Add(new KeyValuePair<string, string>("username", this.username ?? ""));
                    form.Add(new KeyValuePair<string, string>("password", this.password ?? ""));
                    form.Add(new KeyValuePair<string, string>("grant_type", this.grantType ?? DEFAULT_GRANT_TYPE));

                    //KHONG log mat khau
                    LogSystem.Info("Hoc3176KcbWorker - Login: " + this.tokenUrl
                        + "; client_id=" + this.clientId + "; username=" + this.username
                        + "; grant_type=" + this.grantType);

                    HttpResponseMessage response;
                    try
                    {
                        response = await client.PostAsync(this.tokenUrl, new FormUrlEncodedContent(form));
                    }
                    catch (Exception exNet)
                    {
                        this.batchFatalError = "không kết nối được cổng dữ liệu y tế (" + this.tokenUrl + ") — " + exNet.Message;
                        LogSystem.Error("Hoc3176KcbWorker - " + this.batchFatalError, exNet);
                        return false;
                    }

                    string body = await response.Content.ReadAsStringAsync();
                    JObject json = null;
                    try { json = string.IsNullOrEmpty(body) ? null : JObject.Parse(body); }
                    catch { }

                    string accessToken = (json != null) ? (string)json["access_token"] : null;
                    if (!response.IsSuccessStatusCode || string.IsNullOrEmpty(accessToken))
                    {
                        int sc = (int)response.StatusCode;
                        string reason = "HTTP " + sc
                            + ((json != null) ? (" " + (string)json["error"] + " — " + (string)json["error_description"]) : (" " + Cut(body, 300)));
                        //Sai tai khoan / bi chan -> khong tu het, chan cac ho so sau cua lo
                        if (sc == 400 || sc == 401 || sc == 403 || sc == 429)
                            this.batchFatalError = "đăng nhập cổng dữ liệu y tế thất bại — " + reason;
                        LogSystem.Warn("Hoc3176KcbWorker - Dang nhap that bai. " + reason);
                        return false;
                    }

                    long ttl = TOKEN_TTL_DEFAULT_SECONDS;
                    try { if (json["expires_in"] != null) ttl = (long)json["expires_in"]; }
                    catch { }
                    if (ttl <= 0) ttl = TOKEN_TTL_DEFAULT_SECONDS;
                    //Khong giu token qua tran mac dinh -> tranh dung token da bi cong thu hoi som
                    if (ttl > TOKEN_TTL_DEFAULT_SECONDS) ttl = TOKEN_TTL_DEFAULT_SECONDS;

                    this.token = accessToken;
                    this.tokenExpireTime = DateTime.Now.AddSeconds(Math.Max(60, ttl - TOKEN_SAFETY_MARGIN_SECOND));
                    LogSystem.Info("Hoc3176KcbWorker - Dang nhap thanh cong. Token het han: "
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

        /// <summary>
        /// Gui 1 ban tin: POST {receiverBase}/{path}, kem header 'tinh' / 'csyt' + Authorization Bearer.
        /// Tra Unauthorized=true khi 401 de caller lay lai token.
        /// </summary>
        private async Task<PushResult> PostAsync(string path, string tag, byte[] xmlBytes, string maLk)
        {
            PushResult result = new PushResult();
            try
            {
                string url = RECEIVER_BASE.TrimEnd('/') + "/" + path.TrimStart('/');
                using (HttpClient client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(HTTP_TIMEOUT_SECOND);

                    HttpContent content;
                    if (USE_JSON_BASE64_BODY)
                    {
                        //Than JSON: { ma_tinh, ma_csyt, du_lieu_chi_tiet(base64 XML) }
                        JObject bodyObj = new JObject();
                        bodyObj["ma_tinh"] = this.maTinh ?? "";
                        bodyObj["ma_csyt"] = this.maCsyt ?? "";
                        bodyObj["du_lieu_chi_tiet"] = Convert.ToBase64String(xmlBytes);
                        content = new StringContent(bodyObj.ToString(Newtonsoft.Json.Formatting.None),
                            Encoding.UTF8, "application/json");
                    }
                    else
                    {
                        //Than XML nguyen ban
                        content = new ByteArrayContent(xmlBytes);
                        content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/xml");
                    }

                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url);
                    request.Content = content;
                    request.Headers.TryAddWithoutValidation("Authorization", "Bearer " + (this.token ?? ""));
                    request.Headers.TryAddWithoutValidation("tinh", this.maTinh ?? "");
                    request.Headers.TryAddWithoutValidation("csyt", this.maCsytHeader ?? "");

                    HttpResponseMessage response;
                    try { response = await client.SendAsync(request); }
                    catch (Exception exNet)
                    {
                        this.batchFatalError = "không kết nối được cổng dữ liệu y tế (" + url + ") — " + exNet.Message;
                        result.Ok = false;
                        result.Message = this.batchFatalError;
                        LogSystem.Error("Hoc3176KcbWorker - " + this.batchFatalError, exNet);
                        return result;
                    }

                    string body = await response.Content.ReadAsStringAsync();
                    int status = (int)response.StatusCode;

                    if (status == 401)
                    {
                        result.Unauthorized = true;
                        result.Message = "401 - Token bị từ chối";
                        LogSystem.Warn("Hoc3176KcbWorker - " + tag + " bi tu choi (401). MA_LK: " + maLk);
                        return result;
                    }

                    JObject j = null;
                    try { j = string.IsNullOrEmpty(body) ? null : JObject.Parse(body); }
                    catch { }

                    int code = 0;
                    try { if (j != null && j["code"] != null) code = (int)j["code"]; }
                    catch { }
                    string message = (j != null) ? ((string)j["message"] ?? "") : Cut(body, 500);
                    //result la ma giao dich cong tra (kieu chuoi); phong truong hop cong tra object
                    string transactionCode = "";
                    try
                    {
                        if (j != null && j["result"] != null)
                        {
                            transactionCode = (j["result"].Type == JTokenType.Object || j["result"].Type == JTokenType.Array)
                                ? j["result"].ToString(Newtonsoft.Json.Formatting.None)
                                : (string)j["result"];
                        }
                    }
                    catch { }

                    bool accepted = response.IsSuccessStatusCode
                        && code == SUCCESS_CODE
                        && !string.IsNullOrEmpty(message)
                        && string.Equals(message.Trim(), SUCCESS_MESSAGE, StringComparison.OrdinalIgnoreCase);

                    LogSystem.Info(string.Format(
                        "Hoc3176KcbWorker - [KET QUA] {0}; MA_LK: {1}; HTTP {2}; code={3}; result={4}; message={5}",
                        tag, maLk, status, code, Cut(transactionCode, 100), Cut(message, 300)));

                    if (accepted)
                    {
                        result.Ok = true;
                        result.TransactionCode = transactionCode;
                        result.Message = "HOC: đã tiếp nhận"
                            + (string.IsNullOrEmpty(transactionCode) ? "" : (" — mã giao dịch " + transactionCode));
                    }
                    else
                    {
                        result.Ok = false;
                        result.Message = "HOC: cổng từ chối (HTTP " + status
                            + (code > 0 ? (" code " + code) : "") + ") — " + Cut(message, 500);
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

        /// <summary>
        /// Chon gia tri gui o header 'csyt'.
        /// Cong yeu cau MA CO SO KCB 5 KY TU (trung MA_CSKCB trong ban tin XML), KHONG phai ma don vi
        /// 13 so cua lien thong Kham suc khoe. Khoa cau hinh dung chung dang giu ma 13 so cua KSK
        /// -> uu tien truong MaCsyt neu no dung 5 ky tu, nguoc lai lay Username (tai khoan cong cap
        /// chinh la ma CSKCB). Khong sua khoa cau hinh de lien thong KSK khong bi anh huong.
        /// </summary>
        private static string GetMaCsytHeader(string maCsytConfig, string username)
        {
            if (IsMaCskcb(maCsytConfig)) return maCsytConfig;
            if (IsMaCskcb(username)) return username;
            return maCsytConfig;   //khong suy duoc -> giu nguyen de con thay trong nhat ky
        }

        /// <summary>Ma co so KCB do BHXH cap: dung 5 ky tu.</summary>
        private static bool IsMaCskcb(string value)
        {
            return !string.IsNullOrWhiteSpace(value) && value.Trim().Length == 5;
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
            public string TransactionCode { get; set; }
        }
    }

    /// <summary>Ket qua gui len Trung tam dieu hanh y te tra cho caller (hien ra man hinh + ghi nhat ky).</summary>
    public class Hoc3176PushResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string TransactionCode { get; set; }
    }
}
