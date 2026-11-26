/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 */
using His.Bhyt.ExportXml.XML130.XML8.QD130.XML;
using Inventec.Common.IntegrationError;
using Inventec.Common.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace HIS.Desktop.Plugins.ExportXmlQD130.Base
{
    /// <summary>
    /// Thong tin ket noi Kho du lieu dung chung tinh Lao Cai (viec 62218/62313).
    /// Khoa HIS.KDL_LAOCAI.CONNECTION_INFO:
    ///   ClientID;ClientSecret|MaCoSo;IdNguonDuLieu;ApiKey|MaCoSo;IdNguonDuLieu;ApiKey...
    /// - Cum dau: cap ma truy cap LGSP, dung chung cho moi co so.
    /// - Moi cum sau: 1 co so, 3 gia tri chep nguyen van tu dong cua co so trong tep khoa So Y te cap
    ///   (tai lieu Kho: "mot co so - mot api-key - mot IdNguonDuLieu").
    /// Dia chi lay token / gui du lieu mac dinh theo tai lieu Kho (ban 17/11/2026); cum dau co the khai
    /// them truong 3, 4 = TokenUrl;PushUrl khi Kho doi dia chi.
    /// </summary>
    public class KdlLaoCaiConfig
    {
        public const string DEFAULT_TOKEN_URL = "https://am.yenbai.gov.vn/token";
        public const string DEFAULT_PUSH_URL = "https://am.yenbai.gov.vn/lgsp/khodl/soyte/v1/data-receiver/v1/api-push-xml";

        public string ClientId { get; private set; }
        public string ClientSecret { get; private set; }
        public string TokenUrl { get; private set; }
        public string PushUrl { get; private set; }
        /// <summary>Ma co so -> thong tin co so. Ma co so so khop voi HEIN_MEDI_ORG_CODE cua ho so.</summary>
        public Dictionary<string, KdlLaoCaiFacility> Facilities { get; private set; }
        /// <summary>Mo ta loi dinh dang khoa (null = hop le).</summary>
        public string Error { get; private set; }

        public bool IsValid { get { return string.IsNullOrEmpty(this.Error); } }

        public KdlLaoCaiConfig(string connectionInfo)
        {
            this.Facilities = new Dictionary<string, KdlLaoCaiFacility>(StringComparer.OrdinalIgnoreCase);
            this.TokenUrl = DEFAULT_TOKEN_URL;
            this.PushUrl = DEFAULT_PUSH_URL;
            try
            {
                if (string.IsNullOrWhiteSpace(connectionInfo))
                {
                    this.Error = "Chưa khai báo cấu hình " + HIS.Desktop.Plugins.ExportXmlQD130.ADO.HisConfigCFG.HIS_KDL_LAOCAI__CONNECTION_INFO;
                    return;
                }

                string[] groups = connectionInfo.Split('|');
                string[] lgsp = groups[0].Split(';');
                this.ClientId = GetPart(lgsp, 0);
                this.ClientSecret = GetPart(lgsp, 1);
                if (!string.IsNullOrEmpty(GetPart(lgsp, 2))) this.TokenUrl = GetPart(lgsp, 2);
                if (!string.IsNullOrEmpty(GetPart(lgsp, 3))) this.PushUrl = GetPart(lgsp, 3);
                if (string.IsNullOrEmpty(this.ClientId) || string.IsNullOrEmpty(this.ClientSecret))
                {
                    this.Error = "Cụm đầu tiên phải là ClientID;ClientSecret";
                    return;
                }

                for (int i = 1; i < groups.Length; i++)
                {
                    if (string.IsNullOrWhiteSpace(groups[i])) continue;
                    string[] f = groups[i].Split(';');
                    string ma = GetPart(f, 0);
                    string idNguon = GetPart(f, 1);
                    string apiKey = GetPart(f, 2);
                    if (string.IsNullOrEmpty(ma) || string.IsNullOrEmpty(idNguon) || string.IsNullOrEmpty(apiKey))
                    {
                        this.Error = string.Format("Cụm thứ {0} không đủ 3 giá trị MaCoSo;IdNguonDuLieu;ApiKey", i + 1);
                        return;
                    }
                    if (this.Facilities.ContainsKey(ma))
                    {
                        this.Error = "Mã cơ sở " + ma + " được khai nhiều lần";
                        return;
                    }
                    this.Facilities[ma] = new KdlLaoCaiFacility(ma, idNguon, apiKey);
                }

                if (this.Facilities.Count == 0)
                {
                    this.Error = "Chưa khai cơ sở nào (MaCoSo;IdNguonDuLieu;ApiKey) sau cụm ClientID;ClientSecret";
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                this.Error = "Không đọc được cấu hình: " + ex.Message;
            }
        }

        public KdlLaoCaiFacility GetFacility(string maCoSo)
        {
            if (string.IsNullOrWhiteSpace(maCoSo)) return null;
            KdlLaoCaiFacility f;
            return this.Facilities.TryGetValue(maCoSo.Trim(), out f) ? f : null;
        }

        private static string GetPart(string[] arr, int index)
        {
            return (arr != null && index < arr.Length && arr[index] != null) ? arr[index].Trim() : null;
        }
    }

    public class KdlLaoCaiFacility
    {
        public string MaCoSo { get; private set; }
        public string IdNguonDuLieu { get; private set; }
        public string ApiKey { get; private set; }

        public KdlLaoCaiFacility(string maCoSo, string idNguonDuLieu, string apiKey)
        {
            this.MaCoSo = maCoSo;
            this.IdNguonDuLieu = idNguonDuLieu;
            this.ApiKey = apiKey;
        }
    }

    /// <summary>
    /// Gui du lieu kham benh, chua benh (data-type YT_khambenhchuabenh = 22 truong XML8 QD 130)
    /// len Kho du lieu dung chung tinh Lao Cai qua truc LGSP, theo tai lieu
    /// HUONG_DAN_SU_DUNG_API_PUSH_XML_LGSP (ban 17/11/2026):
    ///   1) POST TokenUrl, Basic ClientID:ClientSecret, grant_type=client_credentials -> access_token
    ///   2) POST PushUrl, Bearer token + api-key + data-type + request-id + request-time, than XML
    /// Moi lan goi gui 1 ban ghi (ROOT khong co BanGhi). Goi tuan tu - tai lieu cam gui song song
    /// bang cung api-key.
    /// </summary>
    public class KdlLaoCaiWorker
    {
        private static readonly IntegrationPartner PARTNER = IntegrationPartner.HealthPortal("Kho dữ liệu dùng chung tỉnh Lào Cai");

        public const string DATA_TYPE = "YT_khambenhchuabenh";
        private const int HTTP_TIMEOUT_SECOND = 60;
        private const int TOKEN_SAFETY_MARGIN_SECOND = 60;
        private const long TOKEN_TTL_DEFAULT_SECONDS = 3600;
        //Moc tinh PhienBan: so giay ke tu 01/01/2026 tai thoi diem gui -> luon tang, nam trong
        //khoang 1..2.147.483.647 cua Kho (den nam 2094), khong can luu phien ban xuong CSDL.
        private static readonly DateTime VERSION_EPOCH = new DateTime(2026, 1, 1, 0, 0, 0);

        private readonly KdlLaoCaiConfig config;
        private string token;
        private DateTime tokenExpireTime = DateTime.MinValue;
        //Loi khong tu het giua lo (sai ClientID/Secret, mat mang) -> cac ho so sau tra loi ngay
        private string batchFatalError;

        public KdlLaoCaiWorker(KdlLaoCaiConfig config)
        {
            this.config = config;
            //Bat TLS 1.2 (.NET Framework 4.5 chua bat mac dinh)
            try { ServicePointManager.SecurityProtocol |= (SecurityProtocolType)3072; }
            catch (Exception exTls) { LogSystem.Warn(exTls); }
        }

        /// <summary>Gui 1 ho so. maDieuTri chi dung de ghi nhat ky.</summary>
        public async Task<KdlLaoCaiPushResult> PushAsync(KdlLaoCaiFacility facility, XML8Data xml8, long? modifyTime, string maDieuTri)
        {
            KdlLaoCaiPushResult ret = new KdlLaoCaiPushResult();
            try
            {
                ret.IdBanGhi = facility.MaCoSo + "_" + (xml8.MA_LK ?? "");
                if (this.batchFatalError != null)
                {
                    ret.Message = PortalErrorHelper.FromRaw(PARTNER, this.batchFatalError, "Kho dữ liệu dùng chung tỉnh Lào Cai từ chối");
                    return ret;
                }
                if (!await EnsureTokenAsync())
                {
                    ret.Message = this.batchFatalError != null
                        ? PortalErrorHelper.FromRaw(PARTNER, this.batchFatalError, "Kho dữ liệu dùng chung tỉnh Lào Cai từ chối")
                        : PortalErrorHelper.Login(PARTNER, this.lastLoginError, this.lastLoginException);
                    return ret;
                }

                byte[] body = BuildMessage(facility, ret.IdBanGhi, xml8, modifyTime);
                PostResult result = await PostAsync(facility, body, maDieuTri);
                //Token LGSP het han giua lo -> lay lai token va gui lai dung 1 lan
                if (result.TokenRejected)
                {
                    LogSystem.Info("KdlLaoCaiWorker - Token LGSP bi tu choi (401), lay lai token va gui lai. Ma dieu tri: " + maDieuTri);
                    this.token = null;
                    this.tokenExpireTime = DateTime.MinValue;
                    if (!await LoginAsync())
                    {
                        ret.Message = this.batchFatalError != null
                            ? PortalErrorHelper.FromRaw(PARTNER, this.batchFatalError, "Kho dữ liệu dùng chung tỉnh Lào Cai từ chối")
                            : PortalErrorHelper.Login(PARTNER, this.lastLoginError, this.lastLoginException);
                        return ret;
                    }
                    body = BuildMessage(facility, ret.IdBanGhi, xml8, modifyTime);
                    result = await PostAsync(facility, body, maDieuTri);
                }

                ret.Success = result.Ok;
                ret.Message = result.Message;
                return ret;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                ret.Success = false;
                ret.Message = IntegrationErrorMessage.FromException(ex, PARTNER, "gửi hồ sơ lên Kho dữ liệu dùng chung tỉnh Lào Cai");
                return ret;
            }
        }

        /// <summary>
        /// Dung ban tin gui don le (tai lieu muc 5.1, mau TEMPLATE_YT_khambenhchuabenh.xml):
        /// Root/NguonDuLieu + TrangThaiDuLieu + DuLieuTiepNhan (IdBanGhi + du 22 truong, truong rong
        /// van gui phan tu rong). Ten phan tu phan biet hoa/thuong, viet dung nhu mau.
        /// </summary>
        private static byte[] BuildMessage(KdlLaoCaiFacility facility, string idBanGhi, XML8Data x, long? modifyTime)
        {
            DateTime now = DateTime.Now;
            long phienBan = (long)(now - VERSION_EPOCH).TotalSeconds;
            if (phienBan < 1) phienBan = 1;
            DateTime capNhat = ToDateTime(modifyTime) ?? now;

            XElement duLieu = new XElement("DuLieuTiepNhan",
                new XElement("IdBanGhi", idBanGhi),
                new XElement("MA_LK", x.MA_LK ?? ""),
                new XElement("MA_LOAI_KCB", x.MA_LOAI_KCB ?? ""),
                new XElement("HO_TEN_CHA", x.HO_TEN_CHA ?? ""),
                new XElement("HO_TEN_ME", x.HO_TEN_ME ?? ""),
                new XElement("NGUOI_GIAM_HO", x.NGUOI_GIAM_HO ?? ""),
                new XElement("DON_VI", x.DON_VI ?? ""),
                new XElement("NGAY_VAO", x.NGAY_VAO ?? ""),
                new XElement("NGAY_RA", x.NGAY_RA ?? ""),
                new XElement("CHAN_DOAN_VAO", x.CHAN_DOAN_VAO ?? ""),
                new XElement("CHAN_DOAN_RV", x.CHAN_DOAN_RV ?? ""),
                new XElement("QT_BENHLY", x.QT_BENHLY ?? ""),
                new XElement("TOMTAT_KQ", CData(x.TOMTAT_KQ)),
                new XElement("PP_DIEUTRI", CData(x.PP_DIEUTRI)),
                new XElement("NGAY_SINHCON", x.NGAY_SINHCON ?? ""),
                new XElement("NGAY_CONCHET", x.NGAY_CONCHET ?? ""),
                new XElement("SO_CONCHET", x.SO_CONCHET ?? ""),
                new XElement("KET_QUA_DTRI", CData(x.KET_QUA_DTRI)),
                new XElement("GHI_CHU", x.GHI_CHU ?? ""),
                new XElement("MA_TTDV", x.MA_TTDV ?? ""),
                new XElement("NGAY_CT", x.NGAY_CT ?? ""),
                new XElement("MA_THE_TAM", x.MA_THE_TAM ?? ""),
                new XElement("DU_PHONG", x.DU_PHONG ?? ""));

            XDocument doc = new XDocument(new XDeclaration("1.0", "UTF-8", null),
                new XElement("Root",
                    new XElement("NguonDuLieu",
                        new XElement("IdNguonDuLieu", facility.IdNguonDuLieu),
                        new XElement("MaLoaiDuLieu", DATA_TYPE)),
                    new XElement("TrangThaiDuLieu",
                        new XElement("SuKien", "NEW"),
                        new XElement("PhienBan", phienBan.ToString()),
                        new XElement("ThoiGianCapNhat", capNhat.ToString("yyyy-MM-ddTHH:mm:ss"))),
                    duLieu));

            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                XmlWriterSettings st = new XmlWriterSettings();
                st.Encoding = new UTF8Encoding(false);
                st.Indent = false;
                using (XmlWriter w = XmlWriter.Create(ms, st))
                {
                    doc.Save(w);
                }
                return ms.ToArray();
            }
        }

        private static string CData(XmlCDataSection s)
        {
            return (s != null && s.Value != null) ? s.Value : "";
        }

        /// <summary>yyyyMMddHHmmss (kieu so cua HIS) -> DateTime.</summary>
        private static DateTime? ToDateTime(long? t)
        {
            try
            {
                if (!t.HasValue || t.Value <= 0) return null;
                return DateTime.ParseExact(t.Value.ToString(), "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture);
            }
            catch
            {
                return null;
            }
        }

        private string lastLoginError;
        private Exception lastLoginException;

        private async Task<bool> EnsureTokenAsync()
        {
            if (!string.IsNullOrEmpty(this.token) && DateTime.Now < this.tokenExpireTime)
                return true;
            return await LoginAsync();
        }

        /// <summary>
        /// Buoc 1 - lay token LGSP: POST TokenUrl, Basic ClientID:ClientSecret,
        /// body grant_type=client_credentials -> { access_token, expires_in }.
        /// </summary>
        private async Task<bool> LoginAsync()
        {
            try
            {
                this.token = null;
                this.tokenExpireTime = DateTime.MinValue;
                this.lastLoginError = null;
                this.lastLoginException = null;

                using (HttpClient client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(HTTP_TIMEOUT_SECOND);
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, this.config.TokenUrl);
                    string basic = Convert.ToBase64String(Encoding.UTF8.GetBytes(this.config.ClientId + ":" + this.config.ClientSecret));
                    request.Headers.TryAddWithoutValidation("Authorization", "Basic " + basic);
                    request.Content = new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("grant_type", "client_credentials") });

                    //KHONG log ClientSecret
                    LogSystem.Info("KdlLaoCaiWorker - Lay token LGSP: " + this.config.TokenUrl);

                    HttpResponseMessage response;
                    try
                    {
                        response = await client.SendAsync(request);
                    }
                    catch (Exception exNet)
                    {
                        this.batchFatalError = "không kết nối được trục LGSP tỉnh (" + this.config.TokenUrl + ") — " + exNet.Message;
                        LogSystem.Error("KdlLaoCaiWorker - " + this.batchFatalError, exNet);
                        this.lastLoginException = exNet;
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
                        string reason = "HTTP " + sc + " " + Cut(body, 300);
                        //Sai ClientID/ClientSecret hoac bi chan -> khong tu het, chan cac ho so sau cua lo
                        if (sc == 400 || sc == 401 || sc == 403 || sc == 429)
                            this.batchFatalError = "lấy mã truy cập LGSP thất bại (kiểm tra ClientID/ClientSecret) — " + reason;
                        LogSystem.Warn("KdlLaoCaiWorker - Lay token that bai. " + reason);
                        this.lastLoginError = reason;
                        return false;
                    }

                    long ttl = TOKEN_TTL_DEFAULT_SECONDS;
                    try { if (json["expires_in"] != null) ttl = (long)json["expires_in"]; }
                    catch { }
                    if (ttl <= 0) ttl = TOKEN_TTL_DEFAULT_SECONDS;

                    this.token = accessToken;
                    this.tokenExpireTime = DateTime.Now.AddSeconds(Math.Max(60, ttl - TOKEN_SAFETY_MARGIN_SECOND));
                    LogSystem.Info("KdlLaoCaiWorker - Lay token thanh cong. Het han: " + this.tokenExpireTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    return true;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                this.lastLoginException = ex;
                return false;
            }
        }

        /// <summary>
        /// Buoc 2 - gui ban tin. Bien nhan XML: Root/statusCode, data, responseTime.
        /// Thanh cong khi HTTP 200 VA statusCode 200 (tai lieu muc 6).
        /// </summary>
        private async Task<PostResult> PostAsync(KdlLaoCaiFacility facility, byte[] xmlBytes, string maDieuTri)
        {
            PostResult result = new PostResult();
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(HTTP_TIMEOUT_SECOND);

                    ByteArrayContent content = new ByteArrayContent(xmlBytes);
                    content.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/xml; charset=utf-8");

                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, this.config.PushUrl);
                    request.Content = content;
                    request.Headers.TryAddWithoutValidation("Authorization", "Bearer " + (this.token ?? ""));
                    request.Headers.TryAddWithoutValidation("Accept", "application/xml");
                    request.Headers.TryAddWithoutValidation("api-key", facility.ApiKey);
                    request.Headers.TryAddWithoutValidation("data-type", DATA_TYPE);
                    request.Headers.TryAddWithoutValidation("request-id", "HIS-" + Guid.NewGuid().ToString("N"));
                    request.Headers.TryAddWithoutValidation("request-time", DateTime.Now.ToString("ddMMyyyyHHmmss"));

                    HttpResponseMessage response;
                    try { response = await client.SendAsync(request); }
                    catch (Exception exNet)
                    {
                        this.batchFatalError = "không kết nối được Kho dữ liệu dùng chung tỉnh Lào Cai (" + this.config.PushUrl + ") — " + exNet.Message;
                        result.Message = this.batchFatalError;
                        LogSystem.Error("KdlLaoCaiWorker - " + this.batchFatalError, exNet);
                        return result;
                    }

                    string body = await response.Content.ReadAsStringAsync();
                    int status = (int)response.StatusCode;

                    string statusCode = null;
                    string data = null;
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(body) && body.TrimStart().StartsWith("<"))
                        {
                            XDocument rx = XDocument.Parse(body);
                            XElement sc = rx.Descendants().FirstOrDefault(o => string.Equals(o.Name.LocalName, "statusCode", StringComparison.OrdinalIgnoreCase));
                            XElement dt = rx.Descendants().FirstOrDefault(o => string.Equals(o.Name.LocalName, "data", StringComparison.OrdinalIgnoreCase));
                            statusCode = sc != null ? sc.Value.Trim() : null;
                            data = dt != null ? dt.Value.Trim() : null;
                        }
                    }
                    catch { }

                    LogSystem.Info(string.Format("KdlLaoCaiWorker - [KET QUA] Ma dieu tri: {0}; co so: {1}; HTTP {2}; statusCode={3}; data={4}",
                        maDieuTri, facility.MaCoSo, status, statusCode, Cut(data ?? body, 500)));

                    //401 do LGSP (khong co bien nhan cua Kho) = token het han -> caller lay lai token
                    if (status == 401 && statusCode == null)
                    {
                        result.TokenRejected = true;
                        result.Message = "401 - Mã truy cập LGSP bị từ chối";
                        return result;
                    }

                    if (status == 200 && statusCode == "200")
                    {
                        result.Ok = true;
                        result.Message = "";
                    }
                    else
                    {
                        result.Ok = false;
                        result.Message = "HTTP " + status
                            + (string.IsNullOrEmpty(statusCode) ? "" : " · " + statusCode)
                            + " – " + (string.IsNullOrEmpty(data) ? Cut(body, 500) : Cut(data, 1000));
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                result.Ok = false;
                result.Message = IntegrationErrorMessage.FromException(ex, PARTNER, "gửi hồ sơ lên Kho dữ liệu dùng chung tỉnh Lào Cai");
                return result;
            }
        }

        private static string Cut(string s, int max)
        {
            if (string.IsNullOrEmpty(s)) return "";
            s = s.Trim();
            return (s.Length <= max) ? s : s.Substring(0, max) + "...";
        }

        private class PostResult
        {
            public bool Ok { get; set; }
            public bool TokenRejected { get; set; }
            public string Message { get; set; }
        }
    }

    public class KdlLaoCaiPushResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string IdBanGhi { get; set; }
    }
}
