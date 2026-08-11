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
using HIS.Desktop.Plugins.TreatmentFinish.ADO;
using Inventec.Common.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.TreatmentFinish.Base
{
    /// <summary>
    /// Worker tra cuu loi ho so tren he thong tien giam dinh.
    /// - Doc thong tin ket noi tu config HIS.TIEN_GIAM_DINH.CONNECTION_INFO:
    ///     BaseURL | token [| timeout]
    /// - Goi GET /api/order-check/violations?treatment_code={ma dieu tri}
    /// - Chi truyen ma dieu tri, khong gui bat ky thong tin benh nhan nao khac.
    ///
    /// He ngoai gioi han 60 luot goi moi phut va chi tra mot ho so moi luot.
    /// Khi bi bao qua tai tam thoi (429) thi cho roi thu lai mot lan.
    ///
    /// Tham chieu: PTTK_53286 - muc B.3.1.1
    /// </summary>
    public class TienGiamDinhWorker
    {
        /// <summary>Duong dan tra cuu theo dac ta API. Dung khi config khong khai bao</summary>
        private const string DEFAULT_VIOLATION_PATH = "api/order-check/violations";

        /// <summary>Thoi gian cho mac dinh cho mot luot goi (giay)</summary>
        private const int DEFAULT_TIMEOUT_SECOND = 30;

        /// <summary>Thoi gian cho toi thieu/toi da - chan gia tri cau hinh vo ly</summary>
        private const int MIN_TIMEOUT_SECOND = 5;
        private const int MAX_TIMEOUT_SECOND = 180;

        /// <summary>So giay cho truoc khi thu lai khi he ngoai bao qua tai tam thoi</summary>
        private const int RATE_LIMIT_RETRY_DELAY_SECOND = 3;

        /// <summary>
        /// So ky tu toi da cua phan hoi duoc ghi vao nhat ky.
        /// Mot ho so co the mang toi 500 dong loi moi nhom nen phai cat bot, tranh phinh tep log.
        /// </summary>
        private const int LOG_BODY_MAX_LENGTH = 4000;

        private readonly string baseUrl;
        private readonly string token;
        private readonly int timeoutSecond;

        /// <summary>
        /// True khi config co du dia chi may chu va chuoi xac thuc.
        /// False thi toan bo tinh nang khong hoat dong, khong phat sinh luot goi nao.
        /// </summary>
        public bool IsValidConfig { get; private set; }

        /// <summary>
        /// Khoi tao tu chuoi cau hinh dang: BaseURL | token [| timeout]
        /// </summary>
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

                    //Thanh phan thu ba la thoi gian cho, tuy chon
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
                        + Config.ConfigKey.TIEN_GIAM_DINH__CONNECTION_INFO
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
        /// de ben goi quyet dinh hanh vi.
        /// </summary>
        /// <param name="treatmentCode">Ma dot dieu tri, trung voi ma lien ket trong tep XML</param>
        /// <param name="cancelToken">Token huy - dung khi tra cuu lo lon</param>
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

                //Lan goi dau
                HttpResult httpResult = await SendAsync(treatmentCode, cancelToken);

                //He ngoai bao qua tai tam thoi - cho roi thu lai mot lan
                if (httpResult.StatusCode == 429)
                {
                    LogSystem.Info("TienGiamDinhWorker - He ngoai bao qua tai tam thoi, cho "
                        + RATE_LIMIT_RETRY_DELAY_SECOND + " giay roi thu lai. Ma dieu tri: " + treatmentCode);

                    await Task.Delay(TimeSpan.FromSeconds(RATE_LIMIT_RETRY_DELAY_SECOND), cancelToken);
                    httpResult = await SendAsync(treatmentCode, cancelToken);
                }

                return BuildResult(result, httpResult);
            }
            catch (OperationCanceledException)
            {
                //Nguoi dung bam huy - khong phai loi, khong ghi nhat ky muc Error
                result.Status = EnumTienGiamDinhStatus.NotChecked;
                return result;
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
        }

        private async Task<HttpResult> SendAsync(string treatmentCode, CancellationToken cancelToken)
        {
            HttpResult httpResult = new HttpResult();
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(this.timeoutSecond);

                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, BuildUrl(treatmentCode));
                    //Chuoi xac thuc truyen qua tieu de, khong dua vao duong dan
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", this.token);

                    HttpResponseMessage response = await client.SendAsync(request, cancelToken);

                    httpResult.StatusCode = (int)response.StatusCode;
                    httpResult.Body = await response.Content.ReadAsStringAsync();

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
                //Nguoi dung huy - de ben goi xu ly
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
        /// Doc phan hoi cua he ngoai va xac dinh muc do loi cua ho so.
        /// Quy tac chan - tham chieu PTTK_53286 muc B.3.1.1:
        ///  - Bang tong hop bao co loi nghiem trong, hoac
        ///  - Nhom loi tra the BHYT co it nhat mot dong (QT-05: he ngoai khong xep muc do cho nhom nay), hoac
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
                //Loi cau hinh, khong phai loi nghiep vu cua nguoi dung
                result.Status = EnumTienGiamDinhStatus.CheckFailed;
                result.FailReason = EnumTienGiamDinhFailReason.Unauthorized;
                LogSystem.Warn("TienGiamDinhWorker - Chuoi xac thuc khong hop le (401). Ma dieu tri: "
                    + result.TreatmentCode);
                return result;
            }

            if (httpResult.StatusCode == 429)
            {
                //Da thu lai mot lan van bi tu choi
                result.Status = EnumTienGiamDinhStatus.CheckFailed;
                result.FailReason = EnumTienGiamDinhFailReason.RateLimited;
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

                //Ma dinh danh luot goi - ghi nhat ky de doi chieu khi co tranh chap
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

                //Danh sach bi cat bot - khong biet con loi nghiem trong nao bi cat, phai coi la nghiem trong
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
