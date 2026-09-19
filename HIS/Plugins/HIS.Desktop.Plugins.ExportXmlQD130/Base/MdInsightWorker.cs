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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ExportXmlQD130.Base
{
    /// <summary>Ket cuc cua mot luot goi he ngoai</summary>
    public enum EnumMdInsightCallOutcome
    {
        /// <summary>Goi thanh cong, co du lieu tra ve</summary>
        Success = 0,

        /// <summary>
        /// He ngoai tra ve rong. CHU Y: day la truong hop NHAP NHANG -
        /// vua co the la ho so chua giam dinh xong, vua co the la ho so sach.
        /// Ben goi phai ket hop voi thoi diem gui da luu de ket luan (quy tac QT-16).
        /// </summary>
        EmptyResult = 1,

        /// <summary>He ngoai tu choi xac thuc (HTTP 401) - sai tai khoan hoac chuoi xac thuc het han</summary>
        Unauthorized = 2,

        /// <summary>Luot goi qua han muc thoi gian</summary>
        Timeout = 3,

        /// <summary>He ngoai bao loi nghiep vu trong than phan hoi, hoac loi ket noi</summary>
        Failed = 4,

        /// <summary>Nguoi dung bam huy</summary>
        Cancelled = 5
    }

    /// <summary>Ket qua mot luot dang nhap</summary>
    public class MdInsightSignInResult
    {
        public EnumMdInsightCallOutcome Outcome { get; set; }

        /// <summary>Chuoi xac thuc - CHI giu trong bo nho, khong luu, khong ghi nhat ky (QT-24, QT-26)</summary>
        public string Token { get; set; }

        public DateTime? ExpiredAt { get; set; }

        /// <summary>
        /// Ma co so kham chua benh doc duoc TU CHINH chuoi xac thuc.
        /// Rong khi khong doc duoc - khi do KHONG duoc ket luan la khai sai.
        /// </summary>
        public string MediOrgCode { get; set; }

        /// <summary>Thong bao nghiep vu de hien cho nguoi dung - khong chua du lieu nhay cam</summary>
        public string Message { get; set; }
    }

    /// <summary>Ket qua mot luot gui lo tep</summary>
    public class MdInsightUploadResult
    {
        public MdInsightUploadResult()
        {
            this.FailedFileNames = new List<string>();
        }

        public EnumMdInsightCallOutcome Outcome { get; set; }

        public string Message { get; set; }

        /// <summary>Ten cac tep he ngoai bao gui that bai</summary>
        public List<string> FailedFileNames { get; set; }
    }

    /// <summary>Ket qua tra loi cua MOT tep</summary>
    public class MdInsightCheckResult
    {
        public MdInsightCheckResult()
        {
            this.Errors = new List<MdInsightErrorADO>();
        }

        public EnumMdInsightCallOutcome Outcome { get; set; }

        public string Message { get; set; }

        public List<MdInsightErrorADO> Errors { get; set; }
    }

    /// <summary>
    /// Mot tep gui len he ngoai.
    /// </summary>
    public class MdInsightUploadItem
    {
        public string FileName { get; set; }

        public byte[] Content { get; set; }
    }

    /// <summary>
    /// Goi bon giao dien cua he thong soat loi MDInsight.
    ///
    /// BON DAC DIEM DA KIEM CHUNG BANG GOI THU THAT ngay 2026-08-17 va 2026-09-14,
    /// deu duoc ma hoa vao lop nay:
    ///
    ///  1. Ma loi nghiep vu nam trong THAN phan hoi, HTTP van la 200.
    ///     Rieng loi xac thuc tra HTTP 401 voi khuon KHAC HAN ({"status":401}, khong co truong code).
    ///  2. Tra ket qua theo LO thi he ngoai AM THAM BO QUA cac tep no khong co, khong bao tep nao
    ///     bi bo. Vi vay <see cref="CheckErrorAsync"/> chi nhan MOT ten tep moi luot - quy tac QT-10.
    ///  3. Truong errorDesc chua HO TEN, MA BENH NHAN va CHAN DOAN. Khong bao gio ghi ra nhat ky,
    ///     khong luu xuong CSDL - quy tac QT-28.
    ///  4. Tai khoan BI CO LAP theo ma co so kham chua benh: tai khoan co so nay khong doc duoc
    ///     tep cua co so khac. Vi vay moi nhom co so phai dang nhap bang tai khoan cua chinh no.
    ///
    /// Tham chieu: dac ta API cua nha cung cap; PTTK muc A.2.5, A.2.8, B.4.1.1 muc b/f/g.
    /// </summary>
    public class MdInsightWorker
    {
        private const string PATH_HEALTH_CHECK = "api/HealthCheck";
        private const string PATH_SIGN_IN = "api/SignIn";
        private const string PATH_UPLOAD_FILE = "api/UploadFile";
        private const string PATH_CHECK_ERROR = "api/CheckError";

        /// <summary>Chuoi he ngoai tra ve khi con song</summary>
        private const string HEALTH_CHECK_EXPECTED = "MDInsight API Running";

        /// <summary>Ma nghiep vu bao thanh cong trong than phan hoi</summary>
        private const int CODE_SUCCESS = 200;

        /// <summary>Ma nghiep vu bao "khong co ho so dang tim kiem" - NHAP NHANG, khong phai loi</summary>
        private const int CODE_NO_RECORD = 400;

        /// <summary>
        /// Do dai cho phep cua mot ma co so kham chua benh khi do tim trong chuoi xac thuc.
        ///
        /// Ma chuan cua BHXH la 5 chu so (25012, 25377, 00001), nhung co so thu nghiem
        /// do MDInsight cap lai dung 6 chu so (340001) - nen phai nhan ca hai do dai.
        /// De qua rong thi de nham voi cac so khac trong the; khi co tu hai ung vien tro len
        /// thi ham doc tra ve rong va phan mem BO QUA buoc doi chieu, khong ket luan bua.
        /// </summary>
        private const int MEDI_ORG_CODE_MIN_LENGTH = 5;
        private const int MEDI_ORG_CODE_MAX_LENGTH = 6;

        /// <summary>
        /// So tep toi da moi luot gui. Nha cung cap CHUA cong bo gioi han (cau hoi B4 con treo),
        /// nen dat tran phong ve o day: vuot thi tu chia nho, tot hon la ca nhom gui hong.
        /// </summary>
        public const int MAX_FILE_PER_UPLOAD = 100;

        private readonly MdInsightConfig config;

        public MdInsightWorker(MdInsightConfig config)
        {
            this.config = config;

            try
            {
                //Bat tay TLS 1.2 voi cong HTTPS tren .NET Framework 4.5
                ServicePointManager.SecurityProtocol |= (SecurityProtocolType)3072;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private string BuildUrl(string path)
        {
            string root = (this.config.BaseUrl ?? "").TrimEnd('/');

            //Quan tri co the da khai san duoi /api trong dia chi - khong noi them lan nua
            if (root.EndsWith("/api", StringComparison.OrdinalIgnoreCase))
            {
                root = root.Substring(0, root.Length - 4);
            }

            return root + "/" + path;
        }

        /// <summary>
        /// Kiem tra hien trang he ngoai. Chi ap cho nut Kiem tra loi XML - quy tac QT-05.
        /// Giao dien nay KHONG can chuoi xac thuc.
        /// </summary>
        public async Task<bool> HealthCheckAsync(CancellationToken cancelToken)
        {
            try
            {
                using (HttpClient client = CreateClient())
                using (CancellationTokenSource linked = CreateCallScope(cancelToken))
                {
                    if (LogSystem.IsDebugEnabled())
                    {
                        LogSystem.Debug("MdInsight GUI DI___" + PATH_HEALTH_CHECK + "___(khong co tham so)");
                    }

                    Stopwatch callClock = Stopwatch.StartNew();
                    HttpResponseMessage response = await client.GetAsync(BuildUrl(PATH_HEALTH_CHECK), linked.Token);
                    string body = await response.Content.ReadAsStringAsync();
                    callClock.Stop();

                    if (LogSystem.IsDebugEnabled())
                    {
                        //Phan hoi cua giao dien nay la chuoi ngan, khong chua du lieu benh nhan
                        LogSystem.Debug("MdInsight NHAN VE___" + PATH_HEALTH_CHECK
                            + "___HTTP " + (int)response.StatusCode
                            + "___" + callClock.ElapsedMilliseconds + " ms"
                            + "___" + (body ?? "").Trim());
                    }

                    bool alive = response.IsSuccessStatusCode
                        && !String.IsNullOrEmpty(body)
                        && body.IndexOf(HEALTH_CHECK_EXPECTED, StringComparison.OrdinalIgnoreCase) >= 0;

                    if (!alive)
                    {
                        LogSystem.Warn("MdInsightWorker - Hien trang he ngoai khong dat. Ma HTTP: "
                            + (int)response.StatusCode);
                    }

                    return alive;
                }
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                return false;
            }
        }

        /// <summary>
        /// Dang nhap bang tai khoan cua MOT co so kham chua benh.
        /// Dang nhap MOI o moi luot chay - quy tac QT-24.
        /// </summary>
        public async Task<MdInsightSignInResult> SignInAsync(MdInsightAccountADO account, CancellationToken cancelToken)
        {
            MdInsightSignInResult result = new MdInsightSignInResult { Outcome = EnumMdInsightCallOutcome.Failed };

            try
            {
                if (account == null)
                {
                    result.Message = "Chua khai tai khoan cho co so kham chua benh nay";
                    return result;
                }

                //Khuon yeu cau theo dac ta: { email, password }
                JObject request = new JObject
                {
                    { "email", account.UserName },
                    { "password", account.Password }
                };

                MdInsightRawResponse raw = await PostAsync(PATH_SIGN_IN, request, cancelToken);

                if (raw.Outcome != EnumMdInsightCallOutcome.Success)
                {
                    result.Outcome = raw.Outcome;
                    result.Message = raw.Message;
                    return result;
                }

                if (raw.Body == null)
                {
                    //Dang nhap ma khong co noi dung tra ve thi khong lay duoc chuoi xac thuc
                    result.Outcome = EnumMdInsightCallOutcome.Failed;
                    result.Message = "He ngoai khong tra ve chuoi xac thuc";
                    LogSystem.Warn("MdInsightWorker - Dang nhap cho co so " + account.MediOrgCode
                        + " khong co noi dung tra ve.");
                    return result;
                }

                int code = ReadInt(raw.Body, "code");
                if (code != 200)
                {
                    //Dac ta: 401 kem "Dang nhap that bai" khi sai thong tin dang nhap
                    result.Outcome = code == 401
                        ? EnumMdInsightCallOutcome.Unauthorized
                        : EnumMdInsightCallOutcome.Failed;
                    result.Message = ReadString(raw.Body, "message");

                    LogSystem.Warn("MdInsightWorker - Dang nhap that bai cho co so "
                        + account.MediOrgCode + ". Ma nghiep vu: " + code);
                    return result;
                }

                //TUYET DOI khong ghi chuoi xac thuc ra nhat ky - quy tac QT-26
                result.Token = ReadString(raw.Body, "token");

                if (String.IsNullOrEmpty(result.Token))
                {
                    result.Outcome = EnumMdInsightCallOutcome.Failed;
                    result.Message = "He ngoai khong tra ve chuoi xac thuc";
                    return result;
                }

                result.ExpiredAt = ParseExpiredAt(ReadString(raw.Body, "expiredAt"));
                result.MediOrgCode = ReadMediOrgCodeFromToken(result.Token);
                result.Outcome = EnumMdInsightCallOutcome.Success;

                LogSystem.Info("MdInsightWorker - Dang nhap thanh cong cho co so " + account.MediOrgCode + ".");
                return result;
            }
            catch (OperationCanceledException)
            {
                result.Outcome = EnumMdInsightCallOutcome.Cancelled;
                return result;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                result.Outcome = EnumMdInsightCallOutcome.Failed;
                result.Message = "Loi ket noi toi he thong soat loi";
                return result;
            }
        }

        /// <summary>
        /// Gui mot lo tep cua CUNG mot co so kham chua benh, bang chuoi xac thuc cua chinh co so do.
        /// Tuyet doi khong gui tep cua co so nay bang tai khoan cua co so khac - quy tac QT-09.
        /// </summary>
        public async Task<MdInsightUploadResult> UploadFilesAsync(
            string token, List<MdInsightUploadItem> items, CancellationToken cancelToken)
        {
            MdInsightUploadResult result = new MdInsightUploadResult { Outcome = EnumMdInsightCallOutcome.Failed };

            try
            {
                if (items == null || items.Count == 0)
                {
                    result.Outcome = EnumMdInsightCallOutcome.Success;
                    return result;
                }

                if (items.Count > MAX_FILE_PER_UPLOAD)
                {
                    LogSystem.Warn("MdInsightWorker - Lo gui co " + items.Count
                        + " tep, vuot tran phong ve " + MAX_FILE_PER_UPLOAD + ". Tu chia nho.");
                    return await UploadInChunksAsync(token, items, cancelToken);
                }

                JArray arrayFiles = new JArray();
                foreach (MdInsightUploadItem item in items)
                {
                    arrayFiles.Add(new JObject
                    {
                        { "fileName", item.FileName },
                        { "fileBytes", Convert.ToBase64String(item.Content ?? new byte[0]) }
                    });
                }

                JObject request = new JObject
                {
                    { "token", token },
                    { "arrayFiles", arrayFiles }
                };

                MdInsightRawResponse raw = await PostAsync(PATH_UPLOAD_FILE, request, cancelToken);

                if (raw.Outcome != EnumMdInsightCallOutcome.Success)
                {
                    result.Outcome = raw.Outcome;
                    result.Message = raw.Message;
                    return result;
                }

                if (raw.Body == null)
                {
                    //He ngoai nhan tep nhung khong tra noi dung (vi du 204 No Content).
                    //Coi la da tiep nhan ca lo - khong biet tep nao that bai nen de danh sach rong.
                    result.Outcome = EnumMdInsightCallOutcome.Success;
                    LogSystem.Info("MdInsightWorker - Da gui " + items.Count
                        + " tep, he ngoai tiep nhan nhung khong tra noi dung.");
                    return result;
                }

                int code = ReadInt(raw.Body, "code");
                if (code != 200)
                {
                    result.Outcome = EnumMdInsightCallOutcome.Failed;
                    result.Message = ReadString(raw.Body, "message");
                    LogSystem.Warn("MdInsightWorker - Gui lo that bai. Ma nghiep vu: " + code
                        + ", so tep: " + items.Count);
                    return result;
                }

                //Truong uploadStatus la chuoi tieng Viet tu do (cau hoi B1 con treo), khong dung
                //lam can cu tu dong. Chi doc failedFiles de biet tep nao that bai.
                JToken data = raw.Body == null ? null : raw.Body["data"];
                if (data != null)
                {
                    JToken failedFiles = data["failedFiles"];
                    if (failedFiles is JArray)
                    {
                        foreach (JToken failed in (JArray)failedFiles)
                        {
                            string name = failed.Type == JTokenType.String
                                ? failed.ToString()
                                : ReadString(failed as JObject, "fileName");

                            if (!String.IsNullOrEmpty(name))
                            {
                                result.FailedFileNames.Add(name);
                            }
                        }
                    }

                    result.Message = ReadString(data as JObject, "failedMessage");
                }

                result.Outcome = EnumMdInsightCallOutcome.Success;
                LogSystem.Info("MdInsightWorker - Da gui " + items.Count + " tep, he ngoai bao "
                    + result.FailedFileNames.Count + " tep that bai.");
                return result;
            }
            catch (OperationCanceledException)
            {
                result.Outcome = EnumMdInsightCallOutcome.Cancelled;
                return result;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                result.Outcome = EnumMdInsightCallOutcome.Failed;
                result.Message = "Loi ket noi khi gui ho so";
                return result;
            }
        }

        private async Task<MdInsightUploadResult> UploadInChunksAsync(
            string token, List<MdInsightUploadItem> items, CancellationToken cancelToken)
        {
            MdInsightUploadResult merged = new MdInsightUploadResult { Outcome = EnumMdInsightCallOutcome.Success };

            for (int i = 0; i < items.Count; i += MAX_FILE_PER_UPLOAD)
            {
                int size = Math.Min(MAX_FILE_PER_UPLOAD, items.Count - i);
                List<MdInsightUploadItem> chunk = items.GetRange(i, size);

                MdInsightUploadResult part = await UploadFilesAsync(token, chunk, cancelToken);
                merged.FailedFileNames.AddRange(part.FailedFileNames);

                if (part.Outcome != EnumMdInsightCallOutcome.Success)
                {
                    //Mot phan hong thi dung ca lo - cac tep chua gui se mang trang thai Khong kiem tra duoc
                    merged.Outcome = part.Outcome;
                    merged.Message = part.Message;
                    return merged;
                }
            }

            return merged;
        }

        /// <summary>
        /// Tra ket qua cua MOT tep.
        ///
        /// CHI MOT TEN TEP moi luot goi. Ly do: gui ca lo thi he ngoai chi tra ket qua cua nhung
        /// tep no co va AM THAM BO QUA cac tep con lai - khong co cach nao biet tep nao bi bo,
        /// nen khong phan biet duoc "ho so sach" voi "ho so chua giam dinh xong" (quy tac QT-10).
        ///
        /// Tra ve <see cref="EnumMdInsightCallOutcome.EmptyResult"/> khi he ngoai khong co du lieu.
        /// Ben goi PHAI ket hop voi thoi diem gui da luu de ket luan - quy tac QT-16.
        /// </summary>
        public async Task<MdInsightCheckResult> CheckErrorAsync(
            string token, string fileName, CancellationToken cancelToken)
        {
            MdInsightCheckResult result = new MdInsightCheckResult { Outcome = EnumMdInsightCallOutcome.Failed };

            try
            {
                if (String.IsNullOrWhiteSpace(fileName))
                {
                    result.Outcome = EnumMdInsightCallOutcome.EmptyResult;
                    return result;
                }

                JObject request = new JObject
                {
                    { "token", token },
                    { "fileName", new JArray { fileName } }
                };

                MdInsightRawResponse raw = await PostAsync(PATH_CHECK_ERROR, request, cancelToken);

                if (raw.Outcome != EnumMdInsightCallOutcome.Success)
                {
                    result.Outcome = raw.Outcome;
                    result.Message = raw.Message;
                    return result;
                }

                if (raw.Body == null)
                {
                    //Thanh cong nhung khong co noi dung = khong co dong loi nao tra ve.
                    //Van la truong hop NHAP NHANG nhu ma 400 - ben goi quyet dinh theo thoi diem gui.
                    result.Outcome = EnumMdInsightCallOutcome.EmptyResult;
                    return result;
                }

                int code = ReadInt(raw.Body, "code");

                if (code == CODE_NO_RECORD)
                {
                    //Ma 400 kem "Khong co ho so dang tim kiem" la truong hop NHAP NHANG:
                    //chua giam dinh xong, hoac da xong va khong co loi. Ben goi quyet dinh.
                    result.Outcome = EnumMdInsightCallOutcome.EmptyResult;
                    result.Message = ReadString(raw.Body, "message");
                    return result;
                }

                if (code != CODE_SUCCESS)
                {
                    //⚠️ MA LA chua duoc liet ke: TUYET DOI khong duoc coi nhu "khong co ho so".
                    //Coi nham thi ben goi se cho qua nguong thoi gian giam dinh roi ket luan ho so SACH,
                    //trong khi thuc te he ngoai bao loi - day la sai lam nguy hiem nhat cua tinh nang
                    //(PTTK muc A.2.6, kich ban 17). Gan Khong kiem tra duoc va giu lai thong bao lam ly do.
                    result.Outcome = EnumMdInsightCallOutcome.Failed;
                    result.Message = ReadString(raw.Body, "message");
                    LogSystem.Warn("MdInsightWorker - Tra ket qua tra ve ma nghiep vu chua duoc liet ke: " + code);
                    return result;
                }

                JToken data = raw.Body == null ? null : raw.Body["data"];
                if (!(data is JArray) || ((JArray)data).Count == 0)
                {
                    result.Outcome = EnumMdInsightCallOutcome.EmptyResult;
                    return result;
                }

                foreach (JToken row in (JArray)data)
                {
                    JObject error = row as JObject;
                    if (error == null)
                    {
                        continue;
                    }

                    result.Errors.Add(new MdInsightErrorADO
                    {
                        //errorMode: true = nghiem trong, false = canh bao.
                        //Dong KHONG mang truong nay thi xep canh bao, khong tu suy ra nghiem trong - QT-12.
                        IsCritical = ReadBool(error, "errorMode"),
                        FileName = ReadString(error, "errorAtXMLFile"),
                        TagName = ReadString(error, "errorAtXMLTag"),
                        RowIndex = ReadString(error, "xeErrorRow"),

                        //errorConts KHONG co trong bang giai thich tham so cua dac ta, chi xuat hien
                        //trong mau JSON. Dang tam coi la "gia tri hien tai cua the" - cau hoi E1 con treo.
                        //Co loi nha cung cap thi sua DUNG MOT DONG nay.
                        CurrentValue = ReadString(error, "errorConts"),

                        ErrorContent = ReadString(error, "errorContent"),

                        //errorDesc CHUA HO TEN, MA BENH NHAN, CHAN DOAN.
                        //Chi giu trong bo nho de hien thi; khong luu CSDL, khong ghi nhat ky - QT-28.
                        FullDescription = ReadString(error, "errorDesc")
                    });
                }

                result.Outcome = EnumMdInsightCallOutcome.Success;
                //Chi ghi SO LUONG dong loi - khong ghi noi dung
                LogSystem.Info("MdInsightWorker - Tra ket qua xong mot ho so, so dong loi: " + result.Errors.Count);
                return result;
            }
            catch (OperationCanceledException)
            {
                result.Outcome = EnumMdInsightCallOutcome.Cancelled;
                return result;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                result.Outcome = EnumMdInsightCallOutcome.Failed;
                result.Message = "Loi ket noi khi tra ket qua";
                return result;
            }
        }

        #region Ha tang goi HTTP

        private class MdInsightRawResponse
        {
            public EnumMdInsightCallOutcome Outcome { get; set; }
            public JObject Body { get; set; }
            public string Message { get; set; }
        }

        private HttpClient CreateClient()
        {
            //Timeout cua HttpClient dat rong; han muc that su ap bang CancellationTokenSource
            //de phan biet duoc "qua han" voi "nguoi dung huy".
            return new HttpClient { Timeout = Timeout.InfiniteTimeSpan };
        }

        /// <summary>
        /// Gop tin hieu huy cua nguoi dung voi han muc mot luot goi lay tu cau hinh.
        /// </summary>
        private CancellationTokenSource CreateCallScope(CancellationToken cancelToken)
        {
            CancellationTokenSource source = CancellationTokenSource.CreateLinkedTokenSource(cancelToken);
            source.CancelAfter(TimeSpan.FromSeconds(this.config.CallTimeoutSecond));
            return source;
        }

        private async Task<MdInsightRawResponse> PostAsync(string path, JObject request, CancellationToken cancelToken)
        {
            MdInsightRawResponse result = new MdInsightRawResponse { Outcome = EnumMdInsightCallOutcome.Failed };

            try
            {
                using (HttpClient client = CreateClient())
                using (CancellationTokenSource linked = CreateCallScope(cancelToken))
                {
                    //Noi dung yeu cau chua mat khau va noi dung tep ho so - khong bao gio ghi nhat ky
                    StringContent content = new StringContent(
                        JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

                    //Nhat ky DAU VAO - da loc bo mat khau, chuoi xac thuc va noi dung tep
                    if (LogSystem.IsDebugEnabled())
                    {
                        LogSystem.Debug("MdInsight GUI DI___" + path + "___" + DescribeRequest(request));
                    }

                    Stopwatch callClock = Stopwatch.StartNew();
                    HttpResponseMessage response = await client.PostAsync(BuildUrl(path), content, linked.Token);
                    string body = await response.Content.ReadAsStringAsync();
                    callClock.Stop();

                    //Nhat ky DAU RA - chi ghi ma va thong bao, TUYET DOI khong ghi danh sach loi
                    if (LogSystem.IsDebugEnabled())
                    {
                        LogSystem.Debug("MdInsight NHAN VE___" + path
                            + "___HTTP " + (int)response.StatusCode
                            + "___" + callClock.ElapsedMilliseconds + " ms"
                            + "___" + DescribeResponse(body));
                    }

                    //Loi xac thuc tra HTTP 401 voi khuon KHAC HAN: {"status":401}, khong co truong code.
                    //Phai bat theo ma HTTP chu khong doi doc duoc truong code trong than.
                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        result.Outcome = EnumMdInsightCallOutcome.Unauthorized;
                        result.Message = "Chuoi xac thuc khong hop le hoac da het han";
                        return result;
                    }

                    if (String.IsNullOrWhiteSpace(body))
                    {
                        //⚠️ Ma giao thuc 2xx KEM THAN RONG van la THANH CONG - vi du 204 No Content.
                        //Khong duoc coi la that bai chi vi khong doc duoc than phan hoi: he ngoai da
                        //nhan tep roi, bao that bai se khien nguoi dung gui lai mot cach vo ich.
                        //Ben goi tu quyet dinh y nghia cua "thanh cong ma khong co noi dung" theo tung
                        //giao dien, vi the de Body = null.
                        if (response.IsSuccessStatusCode)
                        {
                            result.Outcome = EnumMdInsightCallOutcome.Success;
                            result.Body = null;
                            LogSystem.Info("MdInsightWorker - " + path + " tra ve ma HTTP "
                                + (int)response.StatusCode + " khong kem noi dung. Coi la thanh cong.");
                            return result;
                        }

                        result.Message = "He ngoai khong tra ve noi dung";
                        LogSystem.Warn("MdInsightWorker - Phan hoi rong tu " + path
                            + ", ma HTTP: " + (int)response.StatusCode);
                        return result;
                    }

                    try
                    {
                        result.Body = JObject.Parse(body);
                    }
                    catch (JsonException)
                    {
                        //Khong ghi than phan hoi ra nhat ky - co the chua du lieu benh nhan
                        LogSystem.Warn("MdInsightWorker - Phan hoi tu " + path
                            + " khong phai JSON hop le, ma HTTP: " + (int)response.StatusCode);
                        result.Message = "He ngoai tra ve du lieu khong doc duoc";
                        return result;
                    }

                    result.Outcome = EnumMdInsightCallOutcome.Success;
                    return result;
                }
            }
            catch (OperationCanceledException)
            {
                //Phan biet nguoi dung huy voi qua han muc mot luot goi
                if (cancelToken.IsCancellationRequested)
                {
                    result.Outcome = EnumMdInsightCallOutcome.Cancelled;
                    result.Message = "Nguoi dung da huy";
                }
                else
                {
                    result.Outcome = EnumMdInsightCallOutcome.Timeout;
                    result.Message = "Qua han muc thoi gian cho he ngoai tra loi";
                    LogSystem.Warn("MdInsightWorker - Qua han " + this.config.CallTimeoutSecond
                        + " giay khi goi " + path + ".");
                }
                return result;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                result.Message = "Loi ket noi toi he thong soat loi";
                return result;
            }
        }

        #endregion

        #region Nhat ky dau vao / dau ra - da loc du lieu nhay cam

        /// <summary>
        /// Mo ta noi dung GUI DI duoi dang an toan de ghi nhat ky.
        ///
        /// ⚠️ Ba thu KHONG BAO GIO duoc ghi ra nhat ky:
        ///  - <c>password</c> va <c>email</c>: thong tin dang nhap (quy tac QT-26).
        ///  - <c>token</c>: chuoi xac thuc (quy tac QT-24, QT-26).
        ///  - <c>fileBytes</c>: noi dung tep XML ho so benh nhan.
        ///
        /// Ten tep cung bi che mot phan vi no chua MA BENH NHAN - xem <see cref="MaskFileName"/>.
        /// </summary>
        private static string DescribeRequest(JObject request)
        {
            try
            {
                if (request == null)
                {
                    return "(rong)";
                }

                List<string> parts = new List<string>();

                foreach (JProperty property in request.Properties())
                {
                    switch (property.Name)
                    {
                        case "password":
                        case "email":
                        case "token":
                            //Khong ghi ca do dai - do dai mat khau cung la thong tin
                            parts.Add(property.Name + "=***");
                            break;

                        case "arrayFiles":
                            parts.Add(DescribeUploadFiles(property.Value as JArray));
                            break;

                        case "fileName":
                            JArray names = property.Value as JArray;
                            parts.Add(names == null
                                ? "fileName=?"
                                : "fileName=[" + String.Join(", ", names.Select(o => MaskFileName(o.ToString()))) + "]");
                            break;

                        default:
                            parts.Add(property.Name + "=" + property.Value);
                            break;
                    }
                }

                return String.Join(" | ", parts);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
                return "(khong mo ta duoc)";
            }
        }

        /// <summary>So tep, tong dung luong va ten tep da che - KHONG ghi noi dung tep</summary>
        private static string DescribeUploadFiles(JArray arrayFiles)
        {
            if (arrayFiles == null)
            {
                return "arrayFiles=?";
            }

            long totalByte = 0;
            List<string> names = new List<string>();

            foreach (JToken item in arrayFiles)
            {
                JObject file = item as JObject;
                if (file == null)
                {
                    continue;
                }

                names.Add(MaskFileName(ReadString(file, "fileName")));

                string content = ReadString(file, "fileBytes");
                //Base64 no khoang 4/3 so voi du lieu goc
                totalByte += content == null ? 0 : (long)(content.Length * 3L / 4L);
            }

            return "arrayFiles=" + names.Count + " tep, ~" + (totalByte / 1024) + " KB"
                   + " [" + String.Join(", ", names) + "]";
        }

        /// <summary>
        /// Che phan MA BENH NHAN trong ten tep.
        ///
        /// Ten tep do phan mem sinh theo khuon: ngay.gio_MADIEUTRI_MABENHNHAN.xml
        /// Ma dot dieu tri DUOC PHEP ghi nhat ky, ma benh nhan thi KHONG (quy tac QT-28),
        /// nen giu phan dau va thay phan cuoi bang dau sao.
        /// </summary>
        private static string MaskFileName(string fileName)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(fileName))
                {
                    return "?";
                }

                int lastUnderscore = fileName.LastIndexOf('_');
                if (lastUnderscore <= 0)
                {
                    return fileName;
                }

                int dot = fileName.LastIndexOf('.');
                string extension = dot > lastUnderscore ? fileName.Substring(dot) : "";

                return fileName.Substring(0, lastUnderscore + 1) + "***" + extension;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
                return "?";
            }
        }

        /// <summary>
        /// Mo ta noi dung NHAN VE duoi dang an toan.
        ///
        /// ⚠️ CHI ghi ma nghiep vu, thong bao va SO LUONG dong du lieu.
        /// Tuyet doi khong ghi noi dung tung dong loi: truong errorDesc chua ho ten benh nhan,
        /// ma benh nhan va chan doan (quy tac QT-28).
        /// </summary>
        private static string DescribeResponse(string body)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(body))
                {
                    return "(than phan hoi rong)";
                }

                JObject parsed;
                try
                {
                    parsed = JObject.Parse(body);
                }
                catch (JsonException)
                {
                    //Khong phai JSON - chi ghi do dai, khong ghi noi dung
                    return "(khong phai JSON, " + body.Length + " ky tu)";
                }

                List<string> parts = new List<string>();

                //Loi xac thuc tra khuon khac han: {"status":401}
                if (parsed["code"] != null) parts.Add("code=" + parsed["code"]);
                if (parsed["status"] != null) parts.Add("status=" + parsed["status"]);
                if (parsed["message"] != null) parts.Add("message=" + parsed["message"]);

                JToken data = parsed["data"];
                if (data is JArray)
                {
                    //CHI so dong, khong ghi noi dung dong nao
                    parts.Add("data=" + ((JArray)data).Count + " dong");

                    //Kem ban tom tat KY THUAT cua cac dong loi - xem DescribeErrorRows
                    string summary = DescribeErrorRows((JArray)data);
                    if (!String.IsNullOrEmpty(summary))
                    {
                        parts.Add(summary);
                    }
                }
                else if (data is JObject)
                {
                    JObject dataObject = (JObject)data;
                    parts.Add("uploadStatus=" + ReadString(dataObject, "uploadStatus"));

                    JToken failed = dataObject["failedFiles"];
                    parts.Add("failedFiles=" + (failed is JArray ? ((JArray)failed).Count : 0));

                    string failedMessage = ReadString(dataObject, "failedMessage");
                    if (!String.IsNullOrWhiteSpace(failedMessage))
                    {
                        parts.Add("failedMessage=" + failedMessage);
                    }
                }

                if (parsed["token"] != null) parts.Add("token=***");
                if (parsed["expiredAt"] != null) parts.Add("expiredAt=" + parsed["expiredAt"]);

                return parts.Count == 0 ? "(khong doc duoc truong nao)" : String.Join(" | ", parts);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
                return "(khong mo ta duoc)";
            }
        }

        /// <summary>
        /// Tom tat cac dong loi bang cac truong KY THUAT, de nhin nhat ky la biet he ngoai
        /// tra ve loai loi gi ma khong phai mo cua so ket qua.
        ///
        /// ⚠️ CHI dung: xeErrorCode (ma loi), errorMode (co nghiem trong),
        /// errorAtXMLFile (tep thanh phan), errorAtXMLTag (the du lieu).
        /// TUYET DOI khong dung: errorContent, errorDesc, errorConts, errorComment
        /// - deu co the chua ho ten benh nhan, ma benh nhan va chan doan (quy tac QT-28).
        /// </summary>
        private static string DescribeErrorRows(JArray rows)
        {
            try
            {
                if (rows == null || rows.Count == 0)
                {
                    return "";
                }

                Dictionary<string, int> countByCode = new Dictionary<string, int>();
                Dictionary<string, int> countByFile = new Dictionary<string, int>();
                int criticalCount = 0;

                foreach (JToken row in rows)
                {
                    JObject item = row as JObject;
                    if (item == null)
                    {
                        continue;
                    }

                    string code = ReadString(item, "xeErrorCode");
                    if (String.IsNullOrWhiteSpace(code))
                    {
                        code = "(khong co ma)";
                    }
                    countByCode[code] = countByCode.ContainsKey(code) ? countByCode[code] + 1 : 1;

                    string atFile = ReadString(item, "errorAtXMLFile");
                    if (!String.IsNullOrWhiteSpace(atFile))
                    {
                        countByFile[atFile] = countByFile.ContainsKey(atFile) ? countByFile[atFile] + 1 : 1;
                    }

                    if (ReadBool(item, "errorMode"))
                    {
                        criticalCount++;
                    }
                }

                //Chi liet ke toi 8 ma loi pho bien nhat - lo lon co the co hang tram ma khac nhau
                List<string> topCodes = countByCode
                    .OrderByDescending(o => o.Value).ThenBy(o => o.Key).Take(8)
                    .Select(o => o.Value > 1 ? o.Key + " x" + o.Value : o.Key)
                    .ToList();

                List<string> parts = new List<string>();
                parts.Add("nghiem trong=" + criticalCount);
                parts.Add("ma loi: " + String.Join(", ", topCodes.ToArray())
                    + (countByCode.Count > topCodes.Count
                        ? " (+" + (countByCode.Count - topCodes.Count) + " ma khac)" : ""));

                if (countByFile.Count > 0)
                {
                    List<string> files = countByFile
                        .OrderByDescending(o => o.Value).ThenBy(o => o.Key).Take(5)
                        .Select(o => o.Key + " x" + o.Value)
                        .ToList();
                    parts.Add("tep thanh phan: " + String.Join(", ", files.ToArray()));
                }

                return String.Join(" | ", parts);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
                return "";
            }
        }

        #endregion

        #region Doc gia tri tu JSON - deu chiu duoc truong thieu hoac sai kieu

        private static string ReadString(JObject source, string name)
        {
            try
            {
                if (source == null)
                {
                    return null;
                }

                JToken token = source[name];
                return token == null || token.Type == JTokenType.Null ? null : token.ToString();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                return null;
            }
        }

        private static int ReadInt(JObject source, string name)
        {
            try
            {
                JToken token = source == null ? null : source[name];
                if (token == null || token.Type == JTokenType.Null)
                {
                    return 0;
                }

                int value;
                return Int32.TryParse(token.ToString(), out value) ? value : 0;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                return 0;
            }
        }

        private static bool ReadBool(JObject source, string name)
        {
            try
            {
                JToken token = source == null ? null : source[name];
                if (token == null || token.Type == JTokenType.Null)
                {
                    //Khong co truong muc do thi xep canh bao - quy tac QT-12
                    return false;
                }

                bool value;
                return Boolean.TryParse(token.ToString(), out value) && value;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                return false;
            }
        }

        /// <summary>
        /// Doc ma co so kham chua benh nam trong chuoi xac thuc.
        ///
        /// Chuoi xac thuc la mot the JWT: phan_dau.phan_than.chu_ky, phan than la JSON ma hoa base64url.
        /// Nha cung cap KHONG cong bo ten truong chua ma co so, nen thay vi doan ten, ham nay duyet moi
        /// gia tri trong phan than va nhan ra gia tri co dang MA CO SO (dung 5 chu so).
        ///
        /// ⚠️ Chi tra ve khi tim thay DUNG MOT gia tri nhu vay. Tim thay nhieu, hoac khong tim thay,
        /// deu tra ve rong - nghia la KHONG XAC MINH DUOC. Ben goi tuyet doi khong duoc coi
        /// "khong xac minh duoc" la "khai sai": lam vay thi chi can nha cung cap doi khuon the
        /// la ca tinh nang chet o moi vien.
        /// </summary>
        private static string ReadMediOrgCodeFromToken(string token)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(token))
                {
                    return null;
                }

                string[] parts = token.Split('.');
                if (parts.Length < 2)
                {
                    return null;
                }

                string payload = parts[1].Replace('-', '+').Replace('_', '/');
                switch (payload.Length % 4)
                {
                    case 2: payload += "=="; break;
                    case 3: payload += "="; break;
                }

                JObject body = JObject.Parse(Encoding.UTF8.GetString(Convert.FromBase64String(payload)));

                List<string> candidates = body.Properties()
                    .Select(o => o.Value == null || o.Value.Type == JTokenType.Null ? null : o.Value.ToString())
                    .Where(IsMediOrgCodeShape)
                    .Distinct()
                    .ToList();

                if (candidates.Count == 1)
                {
                    return candidates[0];
                }

                //Do khong ra thi ghi TEN cac truong co trong the de lan sau doc dung ten,
                //khong phai do tim theo hinh dang nua. CHI ghi ten truong, KHONG ghi gia tri:
                //trong the co dia chi thu dien tu va cac thong tin dinh danh khac.
                LogSystem.Warn("MdInsightWorker - Khong xac dinh duoc ma co so trong chuoi xac thuc ("
                    + (candidates.Count == 0 ? "khong truong nao dung dang" : candidates.Count + " truong cung dang")
                    + "). Cac truong co trong the: "
                    + String.Join(", ", body.Properties().Select(o => o.Name).ToArray())
                    + ". Bo qua buoc doi chieu co so.");

                return null;
            }
            catch (Exception ex)
            {
                //Doc khong ra thi thoi, khong lam vo luong nghiep vu
                LogSystem.Warn("MdInsightWorker - Khong doc duoc ma co so tu chuoi xac thuc.", ex);
                return null;
            }
        }

        /// <summary>Dung 5 chu so - khuon cua ma co so kham chua benh</summary>
        private static bool IsMediOrgCodeShape(string value)
        {
            return !String.IsNullOrEmpty(value)
                && value.Length >= MEDI_ORG_CODE_MIN_LENGTH
                && value.Length <= MEDI_ORG_CODE_MAX_LENGTH
                && value.All(Char.IsDigit);
        }

        /// <summary>
        /// Doc thoi diem het han chuoi xac thuc.
        ///
        /// Phai doc PHONG VE: tren mdinsight.vn truong nay ve theo chuan quoc te
        /// (vi du 2027-02-13T22:47:32Z), nhung ban trien khai eGDBH nam 2022 lai ve dang dd/MM/yyyy.
        /// Doc khong ra thi tra null - luong nghiep vu van chay vi moi luot deu dang nhap moi (QT-24).
        /// </summary>
        private static DateTime? ParseExpiredAt(string value)
        {
            if (String.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            DateTime parsed;

            if (DateTime.TryParse(value, CultureInfo.InvariantCulture,
                DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out parsed))
            {
                return parsed;
            }

            string[] formats = { "dd/MM/yyyy HH:mm:ss", "dd/MM/yyyy HH:mm", "dd/MM/yyyy" };
            if (DateTime.TryParseExact(value, formats, CultureInfo.InvariantCulture,
                DateTimeStyles.None, out parsed))
            {
                return parsed;
            }

            //He ngoai co the tra theo dinh dang Viet Nam co "SA"/"CH" thay cho AM/PM
            //(vi du 15/03/2027 4:07:53 CH) - InvariantCulture khong doc duoc dang nay.
            try
            {
                if (DateTime.TryParse(value, new CultureInfo("vi-VN"), DateTimeStyles.None, out parsed))
                {
                    return parsed;
                }
            }
            catch (CultureNotFoundException)
            {
                //May khong cai ngon ngu do - bo qua, coi nhu khong doc duoc
            }

            LogSystem.Warn("MdInsightWorker - Khong doc duoc dinh dang thoi diem het han cua chuoi xac thuc.");
            return null;
        }

        #endregion
    }
}
