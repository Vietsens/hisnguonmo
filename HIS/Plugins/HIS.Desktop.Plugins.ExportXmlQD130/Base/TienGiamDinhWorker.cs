/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */
using HIS.Desktop.Plugins.ExportXmlQD130.ADO;
using Inventec.Common.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ExportXmlQD130.Base
{
    /// <summary>
    /// Worker tra cuu loi ho so tren he thong tien giam dinh.
    /// - Doc thong tin ket noi tu config HIS.TIEN_GIAM_DINH.CONNECTION_INFO:
    ///     BaseURL | token [| timeout]
    /// - Goi GET /api/order-check/violations?treatment_code={ma dieu tri}
    /// - Chi truyen ma dieu tri, khong gui bat ky thong tin benh nhan nao khac.
    ///
    /// He ngoai gioi han 60 luot goi moi phut va chi tra mot ho so moi luot,
    /// nen man hinh phai goi tuan tu trong luong nen, co tien trinh va nut huy.
    /// Khi bi bao qua tai tam thoi (429) thi cho roi thu lai mot lan.
    ///
    /// Tham chieu: PTTK_53286 muc B.3.1.1 va B.4.2
    /// </summary>
    public class TienGiamDinhWorker
    {
        /// <summary>Duong dan tra cuu theo dac ta API</summary>
        private const string DEFAULT_VIOLATION_PATH = "api/order-check/violations";

        /// <summary>Thoi gian cho mac dinh cho mot luot goi (giay)</summary>
        private const int DEFAULT_TIMEOUT_SECOND = 30;

        private const int MIN_TIMEOUT_SECOND = 5;
        private const int MAX_TIMEOUT_SECOND = 180;

        /// <summary>
        /// So giay cho truoc khi thu lai khi he ngoai bao qua tai tam thoi.
        /// Chi dung khi cong khong gui header Retry-After.
        /// De 15 giay vi cua so gioi han cua cong la mot phut: cho qua ngan thi thu lai gan nhu
        /// chac chan van bi tu choi, vua mat thoi gian vua mat luon ket qua cua ho so do.
        /// Da giai nhip nen 429 la truong hop hiem, cho lau mot chut khong anh huong toc do chung.
        /// </summary>
        private const int RATE_LIMIT_RETRY_DELAY_SECOND = 15;

        /// <summary>Tran tren cho header Retry-After - cong khai bao vo ly thi khong treo man hinh ca phut</summary>
        private const int MAX_RETRY_AFTER_SECOND = 60;

        /// <summary>
        /// Khoang cach toi thieu giua hai lan BAT DAU goi (mili giay).
        /// Cong cho 60 luot moi phut, tuc mot luot moi giay; de 1100ms lay bien an toan
        /// vi dac ta khong noi ro cong tinh theo cua so co dinh hay cua so truot.
        /// </summary>
        private const int MIN_REQUEST_INTERVAL_MILLISECOND = 1100;

        /// <summary>
        /// So ky tu toi da cua phan hoi duoc ghi vao nhat ky.
        /// Man nay chay theo lo nen phai cat bot, tranh mot luot kiem tra sinh ra tep log qua lon.
        /// </summary>
        private const int LOG_BODY_MAX_LENGTH = 4000;

        private readonly string baseUrl;
        private readonly string token;
        private readonly int timeoutSecond;

        /// <summary>
        /// Dong ho do khoang cach giua cac luot goi.
        /// Dung Stopwatch chu khong dung gio he thong de khong bi lech khi may dong bo lai dong ho.
        /// </summary>
        private readonly System.Diagnostics.Stopwatch clock = System.Diagnostics.Stopwatch.StartNew();

        /// <summary>Moc bat dau cua luot goi gan nhat theo dong ho tren. So am = chua goi lan nao.</summary>
        private long lastRequestStartMillisecond = -1;

        /// <summary>
        /// True khi config co du dia chi may chu va chuoi xac thuc.
        /// False thi toan bo tinh nang khong hoat dong, khong phat sinh luot goi nao.
        /// </summary>
        public bool IsValidConfig { get; private set; }

        /// <summary>Khoi tao tu chuoi cau hinh dang: BaseURL | token [| timeout]</summary>
        public TienGiamDinhWorker(string connectionInfo)
        {
            try
            {
                this.timeoutSecond = DEFAULT_TIMEOUT_SECOND;

                if (!string.IsNullOrWhiteSpace(connectionInfo))
                {
                    string[] parts = connectionInfo.Split('|');
                    if (parts.Length >= 2)
                    {
                        this.baseUrl = (parts[0] ?? "").Trim();
                        this.token = (parts[1] ?? "").Trim();
                    }

                    if (parts.Length > 2 && !string.IsNullOrWhiteSpace(parts[2]))
                    {
                        int configTimeout;
                        if (int.TryParse(parts[2].Trim(), out configTimeout)
                            && configTimeout >= MIN_TIMEOUT_SECOND
                            && configTimeout <= MAX_TIMEOUT_SECOND)
                        {
                            this.timeoutSecond = configTimeout;
                        }
                        else
                        {
                            LogSystem.Warn("TienGiamDinhWorker - Thoi gian cho khai bao khong hop le, dung mac dinh "
                                + DEFAULT_TIMEOUT_SECOND + " giay.");
                        }
                    }
                }

                this.IsValidConfig = !string.IsNullOrEmpty(this.baseUrl)
                    && !string.IsNullOrEmpty(this.token);

                if (!this.IsValidConfig && !string.IsNullOrWhiteSpace(connectionInfo))
                {
                    //Chi canh bao khi co khai bao nhung khai thieu.
                    //Khong khai bao gi = vien chua dau noi, la trang thai binh thuong.
                    LogSystem.Warn("TienGiamDinhWorker - Cau hinh "
                        + HisConfigCFG.HIS_TIEN_GIAM_DINH__CONNECTION_INFO
                        + " khong hop le. Can dinh dang: BaseURL | token [| timeout]");
                }

                //Dam bao bat tay duoc TLS 1.2 voi cong HTTPS tren .NET Framework 4.5
                try
                {
                    ServicePointManager.SecurityProtocol |= (SecurityProtocolType)3072;
                }
                catch (Exception exTls)
                {
                    LogSystem.Warn(exTls);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                this.IsValidConfig = false;
            }
        }

        private string BuildUrl(string treatmentCode)
        {
            return this.baseUrl.TrimEnd('/')
                + "/" + DEFAULT_VIOLATION_PATH.TrimStart('/')
                + "?treatment_code=" + Uri.EscapeDataString(treatmentCode ?? "");
        }

        /// <summary>
        /// Tra cuu loi cua mot dot dieu tri.
        /// Khong bao gio nem exception - moi that bai deu tra ve ket qua co Status = CheckFailed
        /// de ben goi quyet dinh hanh vi. Rieng viec nguoi dung bam huy thi nem
        /// OperationCanceledException de vong lap ngoai dung ngay.
        /// </summary>
        public async Task<TienGiamDinhResultADO> CheckAsync(string treatmentCode, CancellationToken cancelToken)
        {
            TienGiamDinhResultADO result = new TienGiamDinhResultADO();
            result.TreatmentCode = treatmentCode;

            try
            {
                if (!this.IsValidConfig)
                {
                    result.Status = EnumTienGiamDinhStatus.CheckFailed;
                    result.FailReason = EnumTienGiamDinhFailReason.NotConfigured;
                    return result;
                }

                if (string.IsNullOrWhiteSpace(treatmentCode))
                {
                    result.Status = EnumTienGiamDinhStatus.CheckFailed;
                    result.FailReason = EnumTienGiamDinhFailReason.SystemError;
                    LogSystem.Warn("TienGiamDinhWorker - Ma dieu tri rong, bo qua tra cuu.");
                    return result;
                }

                HttpResult httpResult = await SendAsync(treatmentCode, cancelToken);

                //He ngoai bao qua tai tam thoi - cho roi thu lai mot lan.
                //Uu tien khoang cho do chinh cong chi dinh, khong co thi dung mac dinh.
                if (httpResult.StatusCode == 429)
                {
                    int retryDelaySecond = httpResult.RetryAfterSecond > 0
                        ? httpResult.RetryAfterSecond
                        : RATE_LIMIT_RETRY_DELAY_SECOND;

                    LogSystem.Info("TienGiamDinhWorker - He ngoai bao qua tai tam thoi, cho "
                        + retryDelaySecond + " giay roi thu lai"
                        + (httpResult.RetryAfterSecond > 0 ? " (theo header Retry-After)" : "")
                        + ". Ma dieu tri: " + treatmentCode);

                    await Task.Delay(TimeSpan.FromSeconds(retryDelaySecond), cancelToken);
                    httpResult = await SendAsync(treatmentCode, cancelToken);
                }

                return BuildResult(result, httpResult);
            }
            catch (OperationCanceledException) when (cancelToken.IsCancellationRequested)
            {
                //Nguoi dung bam huy - de vong lap ngoai dung ngay
                throw;
            }
            catch (Exception ex)
            {
                LogSystem.Error("TienGiamDinhWorker - Tra cuu that bai. Ma dieu tri: " + treatmentCode, ex);
                result.Status = EnumTienGiamDinhStatus.CheckFailed;
                result.FailReason = EnumTienGiamDinhFailReason.SystemError;
                return result;
            }
        }

        /// <summary>Ket qua tho cua mot luot goi HTTP</summary>
        private class HttpResult
        {
            public int StatusCode { get; set; }
            public string Body { get; set; }
            public bool IsTimeout { get; set; }
            public bool IsNetworkError { get; set; }

            /// <summary>So giay cong yeu cau cho truoc khi goi lai (header Retry-After). 0 = cong khong chi dinh.</summary>
            public int RetryAfterSecond { get; set; }
        }

        /// <summary>
        /// Cho du khoang cach toi thieu roi moi bat dau luot goi tiep theo.
        ///
        /// Co y goi CHAM lai cho khop tran cua cong. Truoc day vong lap goi het toc do, vuot tran
        /// thi bi tu choi (429), phai cho roi thu lai - vua lau hon vua lam ho so bi tra ve
        /// "khong kiem tra duoc" trong khi ban than ho so khong co van de gi.
        ///
        /// Do theo moc BAT DAU cua luot truoc chu khong phai luc no ket thuc, nen thoi gian
        /// ban than luot goi da nam trong khoang cho: cong tra loi cham thi gan nhu khong phai cho them.
        /// </summary>
        private async Task WaitForNextSlotAsync(CancellationToken cancelToken)
        {
            if (this.lastRequestStartMillisecond >= 0)
            {
                long elapsed = this.clock.ElapsedMilliseconds - this.lastRequestStartMillisecond;
                long waitMillisecond = MIN_REQUEST_INTERVAL_MILLISECOND - elapsed;

                if (waitMillisecond > 0)
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(waitMillisecond), cancelToken);
                }
            }

            this.lastRequestStartMillisecond = this.clock.ElapsedMilliseconds;
        }

        private async Task<HttpResult> SendAsync(string treatmentCode, CancellationToken cancelToken)
        {
            HttpResult httpResult = new HttpResult();
            try
            {
                await WaitForNextSlotAsync(cancelToken);

                using (HttpClient client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(this.timeoutSecond);

                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, BuildUrl(treatmentCode));
                    //Chuoi xac thuc truyen qua tieu de, khong dua vao duong dan
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", this.token);

                    HttpResponseMessage response = await client.SendAsync(request, cancelToken);

                    httpResult.StatusCode = (int)response.StatusCode;
                    httpResult.Body = await response.Content.ReadAsStringAsync();
                    httpResult.RetryAfterSecond = ReadRetryAfterSecond(response);

                    //Ghi nguyen van phan hoi de doi chieu voi he ngoai khi hai ben khong thong nhat ket qua,
                    //va de biet cong co that su tra du lieu cua vien hay tra rong moi ho so.
                    //Phan hoi khong chua thong tin dinh danh benh nhan - ma the BHYT da duoc he ngoai che,
                    //chi con bon so cuoi (dac ta API muc 5).
                    LogSystem.Debug("TienGiamDinhWorker - Phan hoi tho. Ma dieu tri: " + treatmentCode
                        + ". HttpStatus: " + httpResult.StatusCode
                        + ". Body: " + CutForLog(httpResult.Body));

                    return httpResult;
                }
            }
            catch (OperationCanceledException) when (cancelToken.IsCancellationRequested)
            {
                throw;
            }
            catch (OperationCanceledException)
            {
                //HttpClient nem OperationCanceledException khi qua thoi gian cho
                httpResult.IsTimeout = true;
                LogSystem.Warn("TienGiamDinhWorker - Qua thoi gian cho " + this.timeoutSecond
                    + " giay. Ma dieu tri: " + treatmentCode);
                return httpResult;
            }
            catch (HttpRequestException exHttp)
            {
                httpResult.IsNetworkError = true;
                LogSystem.Warn("TienGiamDinhWorker - Khong ket noi duoc he tien giam dinh. Ma dieu tri: "
                    + treatmentCode + ". " + exHttp.Message);
                return httpResult;
            }
        }

        /// <summary>
        /// Doc header Retry-After neu cong co gui. Cong duoc phep gui so giay hoac moc thoi gian.
        /// Khong co hoac khong doc duoc thi tra ve 0 de ben goi dung khoang cho mac dinh.
        /// </summary>
        private static int ReadRetryAfterSecond(HttpResponseMessage response)
        {
            try
            {
                if (response == null || response.Headers == null || response.Headers.RetryAfter == null)
                {
                    return 0;
                }

                RetryConditionHeaderValue retryAfter = response.Headers.RetryAfter;
                double second = 0;

                if (retryAfter.Delta.HasValue)
                {
                    second = retryAfter.Delta.Value.TotalSeconds;
                }
                else if (retryAfter.Date.HasValue)
                {
                    second = (retryAfter.Date.Value - DateTimeOffset.UtcNow).TotalSeconds;
                }

                if (second <= 0)
                {
                    return 0;
                }

                return second > MAX_RETRY_AFTER_SECOND
                    ? MAX_RETRY_AFTER_SECOND
                    : (int)Math.Ceiling(second);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
            return 0;
        }

        /// <summary>
        /// Doc phan hoi cua he ngoai va xac dinh muc do loi cua ho so.
        /// Quy tac chan - tham chieu PTTK_53286:
        ///  - Bang tong hop bao co loi nghiem trong, hoac
        ///  - Nhom loi tra the BHYT co it nhat mot dong (QT-05), hoac
        ///  - Bang tong hop bao danh sach bi cat bot (QT-06).
        /// </summary>
        private TienGiamDinhResultADO BuildResult(TienGiamDinhResultADO result, HttpResult httpResult)
        {
            if (httpResult.IsTimeout)
            {
                result.Status = EnumTienGiamDinhStatus.CheckFailed;
                result.FailReason = EnumTienGiamDinhFailReason.Timeout;
                return result;
            }

            if (httpResult.IsNetworkError)
            {
                result.Status = EnumTienGiamDinhStatus.CheckFailed;
                result.FailReason = EnumTienGiamDinhFailReason.SystemError;
                return result;
            }

            if (httpResult.StatusCode == 401)
            {
                result.Status = EnumTienGiamDinhStatus.CheckFailed;
                result.FailReason = EnumTienGiamDinhFailReason.Unauthorized;
                LogSystem.Warn("TienGiamDinhWorker - Chuoi xac thuc khong hop le (401). Ma dieu tri: "
                    + result.TreatmentCode);
                return result;
            }

            if (httpResult.StatusCode == 429)
            {
                result.Status = EnumTienGiamDinhStatus.CheckFailed;
                result.FailReason = EnumTienGiamDinhFailReason.RateLimited;

                //Phai ghi ro: ho so nay KHONG duoc kiem, khong phai la ho so sach.
                //Truoc day nhanh nay im lang nen khong dem duoc bao nhieu ho so chet vi qua tai.
                LogSystem.Warn("TienGiamDinhWorker - Van bi tu choi vi qua tai sau khi da thu lai, "
                    + "ho so nay khong duoc kiem. Ma dieu tri: " + result.TreatmentCode);
                return result;
            }

            if (httpResult.StatusCode != 200)
            {
                result.Status = EnumTienGiamDinhStatus.CheckFailed;
                result.FailReason = EnumTienGiamDinhFailReason.SystemError;
                LogSystem.Warn("TienGiamDinhWorker - He ngoai tra ve HttpStatus " + httpResult.StatusCode
                    + ". Ma dieu tri: " + result.TreatmentCode
                    + ". Body: " + CutForLog(httpResult.Body));
                return result;
            }

            try
            {
                JObject json = JObject.Parse(httpResult.Body ?? "");

                JObject meta = json["meta"] as JObject;
                if (meta != null)
                {
                    result.RequestId = meta.Value<string>("request_id");
                }

                bool success = json.Value<bool?>("success") ?? false;
                if (!success)
                {
                    result.Status = EnumTienGiamDinhStatus.CheckFailed;
                    result.FailReason = EnumTienGiamDinhFailReason.SystemError;
                    JObject error = json["error"] as JObject;
                    LogSystem.Warn("TienGiamDinhWorker - He ngoai tra ve that bai. Ma dieu tri: "
                        + result.TreatmentCode
                        + ". Ma loi: " + (error == null ? "" : error.Value<string>("code"))
                        + ". Mo ta: " + (error == null ? "" : error.Value<string>("message"))
                        + ". Chi tiet: " + (error == null ? "" : error.Value<string>("details"))
                        + ". RequestId: " + result.RequestId);
                    return result;
                }

                JObject data = json["data"] as JObject;
                JObject summary = json["summary"] as JObject;

                ReadOrderCheckGroup(result, data);
                ReadHeinCardGroup(result, data);
                ReadXml3176Group(result, data);

                if (summary != null)
                {
                    result.IsTruncated = summary.Value<bool?>("truncated") ?? false;
                }

                bool hasCritical = result.CriticalErrorCount > 0 || result.IsTruncated;

                if (hasCritical)
                {
                    result.Status = EnumTienGiamDinhStatus.Critical;
                }
                else if (result.TotalErrorCount > 0)
                {
                    result.Status = EnumTienGiamDinhStatus.Warning;
                }
                else
                {
                    result.Status = EnumTienGiamDinhStatus.NoError;
                }

                LogResponseSummary(result, summary);
                return result;
            }
            catch (Exception ex)
            {
                LogSystem.Error("TienGiamDinhWorker - Khong doc duoc phan hoi cua he ngoai. Ma dieu tri: "
                    + result.TreatmentCode
                    + ". Body: " + CutForLog(httpResult.Body), ex);
                result.Status = EnumTienGiamDinhStatus.CheckFailed;
                result.FailReason = EnumTienGiamDinhFailReason.SystemError;
                return result;
            }
        }

        /// <summary>
        /// Ghi lai bang tong hop do he ngoai tra ve, dat canh so lieu HIS tu dem duoc.
        /// Hai con so lech nhau la dau hieu doc sai phan hoi - tim ra ngay tu nhat ky,
        /// khong phai dung lai hien truong.
        /// </summary>
        private void LogResponseSummary(TienGiamDinhResultADO result, JObject summary)
        {
            try
            {
                LogSystem.Info("TienGiamDinhWorker - Ket qua. Ma dieu tri: " + result.TreatmentCode
                    + ". Trang thai: " + result.Status
                    + ". He ngoai bao: tong=" + GetSummaryValue(summary, "total")
                    + ", y lenh=" + GetSummaryValue(summary, "order_check")
                    + ", tra the=" + GetSummaryValue(summary, "hein_card")
                    + ", ho so XML=" + GetSummaryValue(summary, "xml3176")
                    + ", nghiem trong=" + GetSummaryValue(summary, "critical")
                    + ", co loi=" + GetSummaryValue(summary, "has_error")
                    + ", cat bot=" + GetSummaryValue(summary, "truncated")
                    + ". HIS doc duoc: " + result.TotalErrorCount + " dong, trong do "
                    + result.CriticalErrorCount + " nghiem trong"
                    + ". RequestId: " + result.RequestId);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Doc mot o cua bang tong hop. Khong co thi tra dau gach de phan biet
        /// "he ngoai khong gui truong nay" voi "he ngoai gui so khong".
        /// </summary>
        private static string GetSummaryValue(JObject summary, string key)
        {
            if (summary == null)
            {
                return "-";
            }

            JToken token = summary[key];
            return token == null ? "-" : token.ToString();
        }

        /// <summary>Cat bot phan hoi truoc khi ghi nhat ky, van noi ro do dai that de biet da cat bao nhieu</summary>
        private static string CutForLog(string body)
        {
            if (string.IsNullOrEmpty(body))
            {
                return "(rong)";
            }

            return body.Length <= LOG_BODY_MAX_LENGTH
                ? body
                : body.Substring(0, LOG_BODY_MAX_LENGTH) + "... (da cat, tong " + body.Length + " ky tu)";
        }

        /// <summary>Nhom sai sot y lenh - muc do lay tu truong severity</summary>
        private void ReadOrderCheckGroup(TienGiamDinhResultADO result, JObject data)
        {
            JArray items = data == null ? null : data["order_check"] as JArray;
            if (items == null)
            {
                return;
            }

            foreach (JToken item in items)
            {
                TienGiamDinhErrorADO error = new TienGiamDinhErrorADO();
                error.Group = EnumTienGiamDinhErrorGroup.OrderCheck;
                error.Code = item.Value<string>("rule_code");
                error.Description = item.Value<string>("message");
                error.IsCritical = string.Equals(item.Value<string>("severity"), "critical",
                    StringComparison.OrdinalIgnoreCase);
                result.Errors.Add(error);
            }
        }

        /// <summary>
        /// Nhom loi tra the BHYT.
        /// He ngoai khong xep muc do cho nhom nay va khong tinh vao so loi nghiem trong,
        /// nhung the het han thi chac chan bi xuat toan - nen HIS coi moi dong la nghiem trong (QT-05).
        /// </summary>
        private void ReadHeinCardGroup(TienGiamDinhResultADO result, JObject data)
        {
            JArray items = data == null ? null : data["hein_card"] as JArray;
            if (items == null)
            {
                return;
            }

            foreach (JToken item in items)
            {
                TienGiamDinhErrorADO error = new TienGiamDinhErrorADO();
                error.Group = EnumTienGiamDinhErrorGroup.HeinCard;
                error.Code = item.Value<string>("ma_tracuu");
                error.Description = item.Value<string>("ma_ketqua");

                string note = item.Value<string>("ghi_chu");
                if (!string.IsNullOrWhiteSpace(note))
                {
                    error.Description = error.Description + " (" + note + ")";
                }

                error.IsCritical = true;
                result.Errors.Add(error);
            }
        }

        /// <summary>Nhom loi ho so XML - muc do lay tu co critical_error</summary>
        private void ReadXml3176Group(TienGiamDinhResultADO result, JObject data)
        {
            JArray items = data == null ? null : data["xml3176"] as JArray;
            if (items == null)
            {
                return;
            }

            foreach (JToken item in items)
            {
                TienGiamDinhErrorADO error = new TienGiamDinhErrorADO();
                error.Group = EnumTienGiamDinhErrorGroup.Xml3176;
                error.Code = item.Value<string>("error_code");

                //Ten loi co the rong khi ma loi chua co trong danh muc - dong loi van phai tra ve
                string name = item.Value<string>("error_name");
                string description = item.Value<string>("description");
                string xmlType = item.Value<string>("xml");

                string text = string.IsNullOrWhiteSpace(name) ? description : name;
                if (string.IsNullOrWhiteSpace(text))
                {
                    text = error.Code;
                }
                if (!string.IsNullOrWhiteSpace(xmlType))
                {
                    text = xmlType + " - " + text;
                }

                error.Description = text;
                error.IsCritical = item.Value<bool?>("critical_error") ?? false;
                result.Errors.Add(error);
            }
        }
    }
}
