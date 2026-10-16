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
using DevExpress.XtraEditors;
using His.Bhyt.ExportXml.XML130;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.Library.XmlPrecheck.ADO;
using HIS.Desktop.Plugins.Library.XmlPrecheck.Base;
using Inventec.Common.Logging;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.Library.XmlPrecheck
{
    /// <summary>
    /// vCong MDInsight ban 3.0 - Luong kiem tra loi XML cho MOT ho so, dung tai man Ket thuc dieu tri.
    ///
    /// Ban mot-ho-so cua hai luong tai man Xuat XML 130 (UCExportXml___MdInsightFlow.cs):
    ///  - <see cref="CheckPreliminaryAsync"/>: sinh tep o che do SO BO (bo qua khoa vien phi) -> gui -> cho -> tra MOT luot.
    ///  - <see cref="FetchAsync"/>: khong gui, chi tra lai ket qua cua lan gui gan nhat.
    ///
    /// Cung quy tac suy trang thai (QT-12...QT-22), cung cach luu (QT-07, QT-11), cung xu ly xac thuc (QT-23).
    /// Khac biet: mot ho so, mot co so - khong co buoc nhom theo co so; co so chua khai tai khoan thi bao
    /// loi cau hinh va KHONG ghi trang thai (QT-34); ho so khong du dieu kien sinh tep thi bao ly do
    /// va KHONG ghi trang thai 5 (kich ban 36).
    ///
    /// Ket qua tra ve chi la SO BO - nguoi goi phai hien dung nhan (QT-32). Tham chieu: PTTK muc B.4.1.2.
    /// </summary>
    public class XmlPrecheckRunner
    {
        /// <summary>
        /// So thieu sot toi da ghep vao ly do khi khong sinh duoc tep - cung nguong voi man Xuat XML 130.
        /// </summary>
        private const int MAX_EXPORT_REASON = 4;

        private readonly MdInsightConfig config;

        /// <summary>Ket qua trong phien, giu ca phan dien giai day du (khong luu CSDL - QT-28)</summary>
        private readonly Dictionary<long, MdInsightResultADO> sessionResults = new Dictionary<long, MdInsightResultADO>();

        private Dictionary<long, string> mediOrgByBranch;

        /// <summary>
        /// Thong bao gan nhat da hien cho nguoi dung trong luot chay vua roi (ly do khong chay duoc: loi cau hinh,
        /// he ngoai hong, ho so thieu du lieu...). Ben goi dung de ghi them vao danh sach canh bao cua minh.
        /// Rong khi luot chay khong phai hien thong bao nao.
        /// </summary>
        public string LastShownMessage { get; private set; }

        public XmlPrecheckRunner(MdInsightConfig config)
        {
            this.config = config;
        }

        #region Doc ket qua da luu

        /// <summary>
        /// Dung ket qua tu trang thai DA LUU: uu tien ban trong phien (co dien giai day du), khong co thi doc
        /// tu CHINH ho so dang mo - bay cot ket qua da nam san tren ban ghi dieu tri, KHONG them truy van nao
        /// (PTTK muc B.4.1.2 a). Tra ve ket qua rong (NotChecked) khi ho so chua tung gui.
        /// </summary>
        public MdInsightResultADO LoadStored(HIS_TREATMENT treatment)
        {
            MdInsightResultADO result = CreateShell(treatment);
            if (treatment == null)
            {
                return result;
            }

            try
            {
                MdInsightResultADO inSession;
                if (this.sessionResults.TryGetValue(treatment.ID, out inSession))
                {
                    return inSession;
                }

                if (!treatment.XML_PRECHECK_RESULT.HasValue)
                {
                    return result;
                }

                result.SentFileName = treatment.XML_PRECHECK_FILE_NAME;
                result.SendTime = treatment.XML_PRECHECK_SEND_TIME;
                result.CheckTime = treatment.XML_PRECHECK_TIME;
                result.ErrorNum = treatment.XML_PRECHECK_ERR_NUM;
                result.CriticalNum = treatment.XML_PRECHECK_CRT_NUM;
                result.Status = (EnumXmlPrecheckStatus)treatment.XML_PRECHECK_RESULT.Value;

                string reason;
                int truncated;
                result.Errors = MdInsightDescCodec.Decode(treatment.XML_PRECHECK_DESC, out reason, out truncated);
                result.Reason = reason;
                result.IsTruncated = truncated > 0;
                result.TruncatedCount = truncated;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }

            return result;
        }

        #endregion

        #region Mot lenh duy nhat - phan mem tu quyet gui hay chi tra

        /// <summary>
        /// MOT lenh, phan mem TU QUYET DINH nhu man Xuat XML 130 (chot ngay 2026-09-16), dua vao trang thai da luu:
        ///  - Chua tung gui, hoac lan gui truoc hong (trang thai 5) -> GUI moi
        ///  - Da gui, chua co ket qua (trang thai 4)                -> CHI TRA, khong gui lai
        ///  - Da co ket qua (trang thai 1/2/3)                      -> HOI: Co = gui lai, Khong = chi tra, Huy = thoi
        ///
        /// <paramref name="allowSend"/> = false khi ho so da khoa vien phi: luc do chi con duong tra lai ket qua,
        /// gui moi phai lam o man Xuat XML 130.
        /// Tra ve null khi khong chay duoc hoac nguoi dung huy - khi do khong ghi trang thai nao.
        /// </summary>
        public async Task<MdInsightResultADO> CheckAutoAsync(HIS_TREATMENT treatment, Form owner, bool allowSend)
        {
            this.LastShownMessage = null;

            if (treatment == null || this.config == null || !this.config.IsValidConfig)
            {
                return null;
            }

            MdInsightResultADO stored = LoadStored(treatment);
            bool neverSent = string.IsNullOrEmpty(stored.SentFileName) || !stored.SendTime.HasValue;

            bool needSend;
            if (neverSent || stored.Status == EnumXmlPrecheckStatus.CheckFailed)
            {
                needSend = true;
            }
            else if (stored.Status == EnumXmlPrecheckStatus.Pending)
            {
                needSend = false;
            }
            else
            {
                DialogResult answer = XtraMessageBox.Show(owner,
                    Resources.ResourceMessageLang.XmlPrecheckXacNhanGuiLai,
                    Resources.ResourceMessageLang.ThongBao,
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (answer == DialogResult.Cancel)
                {
                    return null;
                }
                needSend = answer == DialogResult.Yes;
            }

            if (needSend && !allowSend)
            {
                Show(owner, Resources.ResourceMessageLang.XmlPrecheckDaKhoaVienPhi, MessageBoxIcon.Information);
                return null;
            }

            LogSystem.Info("XmlPrecheck - Ho so " + treatment.ID + ": trang thai da luu " + (int)stored.Status
                + " -> " + (needSend ? "gui kiem tra so bo" : "chi tra lai ket qua") + ".");

            return needSend
                ? await CheckPreliminaryAsync(treatment, owner)
                : await FetchAsync(treatment, owner);
        }

        /// <summary>
        /// Kiem tra SO BO ho so dang mo: sinh tep bo qua khoa vien phi -> gui -> cho -> tra mot luot.
        /// Tra ve null khi khong chay duoc (loi cau hinh, he ngoai hong, ho so chua du dieu kien) -
        /// khi do da bao nguoi dung va KHONG ghi trang thai nao.
        /// </summary>
        public async Task<MdInsightResultADO> CheckPreliminaryAsync(HIS_TREATMENT treatment, Form owner)
        {
            if (treatment == null || this.config == null || !this.config.IsValidConfig)
            {
                return null;
            }

            MdInsightResultADO result = CreateShell(treatment);

            //Quy tac QT-34: tai khoan theo ma co so cua ho so; chua khai thi bao loi cau hinh, khong gui
            MdInsightAccountADO account = this.config.GetAccount(result.MediOrgCode);
            if (account == null)
            {
                LogNoAccountForMediOrg(result.MediOrgCode);
                Show(owner, string.Format(Resources.ResourceMessageLang.XmlPrecheckLoiCauHinhTaiKhoan,
                    string.IsNullOrWhiteSpace(result.MediOrgCode) ? "?" : result.MediOrgCode), MessageBoxIcon.Warning);
                return null;
            }

            MdInsightWorker worker = new MdInsightWorker(this.config);

            using (frmTienGiamDinhProgress progress = new frmTienGiamDinhProgress(
                5, Resources.ResourceMessageLang.MdInsightTieuDeTienTrinh, Resources.ResourceMessageLang.MdInsightNutHuy))
            {
                progress.Show(owner);
                try
                {
                    //Quy tac QT-05: kiem hien trang truoc khi gui
                    progress.SetProgress(0, 5, Resources.ResourceMessageLang.XmlPrecheckDangChuanBi);
                    if (!await worker.HealthCheckAsync(progress.CancelToken))
                    {
                        Show(owner, Resources.ResourceMessageLang.MdInsightHeNgoaiKhongPhanHoi, MessageBoxIcon.Warning);
                        return null;
                    }

                    //Nap du lieu va sinh tep o che do so bo (QT-31)
                    progress.SetProgress(1, 5, Resources.ResourceMessageLang.XmlPrecheckDangChuanBi);
                    MdInsightUploadItem item = BuildPreliminaryFile(treatment, owner, result);
                    if (item == null)
                    {
                        return null;
                    }

                    //Dang nhap bang tai khoan cua co so (QT-24), doi chieu co so (kich ban 28)
                    progress.SetProgress(2, 5, Resources.ResourceMessageLang.XmlPrecheckDangGuiMot);
                    MdInsightSignInResult signIn = await worker.SignInAsync(account, progress.CancelToken);
                    if (signIn.Outcome != EnumMdInsightCallOutcome.Success)
                    {
                        Show(owner, Resources.ResourceMessageLang.MdInsightLyDoSaiXacThuc, MessageBoxIcon.Warning);
                        return null;
                    }

                    string mismatchReason;
                    if (!IsSignInMediOrgMatched(result.MediOrgCode, account, signIn, out mismatchReason))
                    {
                        Show(owner, mismatchReason, MessageBoxIcon.Warning);
                        return null;
                    }

                    //Gui mot tep, ghi nhan NGAY (QT-07: lan gui moi xoa ket qua cu)
                    MdInsightUploadResult upload = await worker.UploadFilesAsync(
                        signIn.Token, new List<MdInsightUploadItem> { item }, progress.CancelToken);

                    long sendTime = GetNowTimeNumber();
                    result.SentFileName = item.FileName;
                    result.CheckTime = null;
                    result.ErrorNum = null;
                    result.CriticalNum = null;
                    result.Errors = new List<MdInsightErrorADO>();

                    bool failed = upload.Outcome != EnumMdInsightCallOutcome.Success
                        || upload.FailedFileNames.Contains(item.FileName);
                    if (failed)
                    {
                        result.SentFileName = null;
                        result.SendTime = null;
                        SetCheckFailed(result, EnumMdInsightFailReason.UploadFailed,
                            Resources.ResourceMessageLang.MdInsightLyDoGuiThatBai, sendTime);
                        Save(result);
                        return result;
                    }

                    result.Status = EnumXmlPrecheckStatus.Pending;
                    result.FailReason = EnumMdInsightFailReason.None;
                    result.Reason = Resources.ResourceMessageLang.MdInsightLyDoChoKetQua;
                    result.SendTime = sendTime;
                    Save(result);

                    //Cho he ngoai kip bat dau giam dinh, roi tra DUNG MOT luot (QT-10)
                    await WaitBeforeQueryAsync(progress, this.config.WaitAfterUploadSecond);
                    if (progress.IsCancelled)
                    {
                        return result;
                    }

                    progress.SetProgress(4, 5, string.Format(Resources.ResourceMessageLang.MdInsightDangLayKetQua, 0, 1));
                    await QueryOnceAsync(worker, account, signIn.Token, result, progress);
                    return result;
                }
                catch (OperationCanceledException)
                {
                    LogSystem.Info("XmlPrecheck - Nguoi dung huy giua chung luot kiem so bo ho so " + treatment.ID + ".");
                    return result.SendTime.HasValue ? result : null;
                }
                catch (MissingMethodException ex)
                {
                    //Bo ket xuat tren may chua co RunPreliminary - DLL dung chung chua duoc cap nhat
                    LogSystem.Error(ex);
                    Show(owner, Resources.ResourceMessageLang.XmlPrecheckBoKetXuatChuaCapNhat, MessageBoxIcon.Error);
                    return null;
                }
                catch (Exception ex)
                {
                    LogSystem.Error(ex);
                    Show(owner, Resources.ResourceMessageLang.MdInsightLyDoHeNgoaiLoi, MessageBoxIcon.Error);
                    return null;
                }
                finally
                {
                    progress.Close();
                }
            }
        }

        /// <summary>
        /// Lay lai ket qua cua lan gui gan nhat - khong gui tep. Ho so chua tung gui thi bao va tra ve null.
        /// </summary>
        public async Task<MdInsightResultADO> FetchAsync(HIS_TREATMENT treatment, Form owner)
        {
            if (treatment == null || this.config == null || !this.config.IsValidConfig)
            {
                return null;
            }

            MdInsightResultADO result = LoadStored(treatment);
            if (string.IsNullOrEmpty(result.SentFileName) || !result.SendTime.HasValue)
            {
                Show(owner, Resources.ResourceMessageLang.XmlPrecheckChuaGui, MessageBoxIcon.Information);
                return null;
            }

            MdInsightAccountADO account = this.config.GetAccount(result.MediOrgCode);
            if (account == null)
            {
                LogNoAccountForMediOrg(result.MediOrgCode);
                Show(owner, string.Format(Resources.ResourceMessageLang.XmlPrecheckLoiCauHinhTaiKhoan,
                    string.IsNullOrWhiteSpace(result.MediOrgCode) ? "?" : result.MediOrgCode), MessageBoxIcon.Warning);
                return null;
            }

            MdInsightWorker worker = new MdInsightWorker(this.config);

            using (frmTienGiamDinhProgress progress = new frmTienGiamDinhProgress(
                1, Resources.ResourceMessageLang.MdInsightTieuDeTienTrinh, Resources.ResourceMessageLang.MdInsightNutHuy))
            {
                progress.Show(owner);
                try
                {
                    //Quy tac QT-05: luong lay ket qua KHONG kiem hien trang
                    MdInsightSignInResult signIn = await worker.SignInAsync(account, progress.CancelToken);
                    if (signIn.Outcome != EnumMdInsightCallOutcome.Success)
                    {
                        Show(owner, Resources.ResourceMessageLang.MdInsightLyDoSaiXacThuc, MessageBoxIcon.Warning);
                        return null;
                    }

                    string mismatchReason;
                    if (!IsSignInMediOrgMatched(result.MediOrgCode, account, signIn, out mismatchReason))
                    {
                        Show(owner, mismatchReason, MessageBoxIcon.Warning);
                        return null;
                    }

                    progress.SetProgress(0, 1, string.Format(Resources.ResourceMessageLang.MdInsightDangLayKetQua, 0, 1));
                    await QueryOnceAsync(worker, account, signIn.Token, result, progress);
                    return result;
                }
                catch (OperationCanceledException)
                {
                    return null;
                }
                catch (Exception ex)
                {
                    LogSystem.Error(ex);
                    Show(owner, Resources.ResourceMessageLang.MdInsightLyDoHeNgoaiLoi, MessageBoxIcon.Error);
                    return null;
                }
                finally
                {
                    progress.Close();
                }
            }
        }

        #endregion

        #region Hien thi

        /// <summary>Mo cua so ket qua cho mot ho so, tieu de ghi ro la so bo</summary>
        public void ShowResultWindow(MdInsightResultADO result, Form owner, string title, string note)
        {
            try
            {
                if (result == null)
                {
                    return;
                }

                using (frmMdInsightResult form = new frmMdInsightResult(new List<MdInsightResultADO> { result }))
                {
                    if (!string.IsNullOrEmpty(title))
                    {
                        form.Text = title;
                    }
                    form.HeaderNote = note;
                    form.ShowDialog(owner);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Mot dong mo ta ket qua gan nhat, vi du "Kiem so bo luc 09:41 12/10/2026 - Co canh bao (3 loi, 0 nghiem trong)".
        /// Chua tung kiem thi tra chuoi rong.
        /// </summary>
        public string DescribeLatest(MdInsightResultADO result, HIS_TREATMENT treatment)
        {
            try
            {
                if (result == null || result.Status == EnumXmlPrecheckStatus.NotChecked)
                {
                    return "";
                }

                long? mark = result.CheckTime ?? result.SendTime;
                string when = mark.HasValue ? Inventec.Common.DateTime.Convert.TimeNumberToTimeString(mark.Value) : "";

                string status = XmlPrecheckLabel.Describe(result,
                    treatment != null ? treatment.FEE_LOCK_TIME : null,
                    treatment != null ? treatment.MODIFY_TIME : null);

                string text = string.Format(Resources.ResourceMessageLang.XmlPrecheckKetQuaGanNhat, when, status);
                if (result.ErrorNum.HasValue && result.ErrorNum.Value > 0)
                {
                    text += string.Format(Resources.ResourceMessageLang.XmlPrecheckKetQuaGanNhatSoLoi,
                        result.ErrorNum.Value, result.CriticalNum ?? 0);
                }
                return text;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
                return "";
            }
        }

        #endregion

        #region Sinh tep so bo

        /// <summary>
        /// Nap du lieu ho so va sinh tep XML o che do so bo. Khong sinh duoc thi bao dung ly do thieu
        /// va tra ve null - KHONG ghi trang thai 5 (ho so chua du dieu kien kiem, khong phai luot kiem hong).
        /// </summary>
        private MdInsightUploadItem BuildPreliminaryFile(HIS_TREATMENT treatment, Form owner, MdInsightResultADO result)
        {
            XmlPrecheckSingleLoader loader = new XmlPrecheckSingleLoader();
            InputADO ado = loader.Load(treatment.ID);
            if (ado == null)
            {
                Show(owner, string.Format(Resources.ResourceMessageLang.XmlPrecheckKhongKetXuatDuocThieu,
                    loader.FailReason ?? ""), MessageBoxIcon.Warning);
                return null;
            }

            //Ho so vua nap la ban moi nhat - cap nhat ma dieu tri, ten de hien thi
            if (loader.Treatment != null)
            {
                result.TreatmentCode = loader.Treatment.TREATMENT_CODE;
                result.PatientName = loader.Treatment.TDL_PATIENT_NAME;
            }

            CreateXmlProcessor processor = new CreateXmlProcessor(ado);
            string error = "";
            MemoryStream stream = processor.RunPreliminary(ref error);

            if (stream == null || stream.Length == 0)
            {
                string detail = DescribeExportFailure(processor, loader.Treatment);
                LogSystem.Warn("XmlPrecheck - Khong sinh duoc tep so bo cho ho so " + treatment.ID
                    + (string.IsNullOrWhiteSpace(detail) ? "" : ". Thieu: " + detail)
                    + (string.IsNullOrWhiteSpace(error) ? "" : ". Thu vien bao: " + error));

                Show(owner, string.Format(Resources.ResourceMessageLang.XmlPrecheckKhongKetXuatDuocThieu,
                    string.IsNullOrWhiteSpace(detail) ? (error ?? "") : detail), MessageBoxIcon.Warning);
                return null;
            }

            MdInsightUploadItem item = new MdInsightUploadItem();
            item.FileName = processor.GetFileName();
            item.Content = stream.ToArray();

            LogSystem.Info("XmlPrecheck - Sinh tep so bo cho ho so " + treatment.ID + ": " + item.FileName
                + ", " + (item.Content.Length / 1024) + " KB.");
            return item;
        }

        /// <summary>
        /// Hoi bo ket xuat (che do so bo) xem ho so thieu gi. Ma benh la du lieu lam sang nen che truoc
        /// khi hien va ghi - quy tac QT-28.
        /// </summary>
        private static string DescribeExportFailure(CreateXmlProcessor processor, V_HIS_TREATMENT_12 treatment)
        {
            try
            {
                List<string> reasons = processor.CheckHoSoPreliminary();
                if (reasons == null || reasons.Count == 0)
                {
                    return null;
                }

                string icdCode = treatment != null ? (treatment.ICD_CODE ?? "").Trim() : "";

                List<string> safe = reasons
                    .Where(o => !string.IsNullOrWhiteSpace(o))
                    .Select(o => string.IsNullOrEmpty(icdCode) ? o : o.Replace(icdCode, "***"))
                    .Take(MAX_EXPORT_REASON)
                    .ToList();

                if (safe.Count == 0)
                {
                    return null;
                }

                return string.Join("; ", safe.ToArray())
                    + (reasons.Count > safe.Count ? "; (còn " + (reasons.Count - safe.Count) + " lỗi nữa)" : "");
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
                return null;
            }
        }

        #endregion

        #region Tra ket qua va suy trang thai - cung quy tac voi man Xuat XML 130

        private async Task QueryOnceAsync(
            MdInsightWorker worker, MdInsightAccountADO account, string token,
            MdInsightResultADO result, frmTienGiamDinhProgress progress)
        {
            MdInsightCheckResult check = await worker.CheckErrorAsync(token, result.SentFileName, progress.CancelToken);

            //Quy tac QT-23: chuoi xac thuc bi tu choi thi dang nhap lai DUNG MOT LAN
            if (check.Outcome == EnumMdInsightCallOutcome.Unauthorized)
            {
                LogSystem.Info("XmlPrecheck - Chuoi xac thuc bi tu choi, dang nhap lai mot lan cho co so " + result.MediOrgCode + ".");
                string mismatchReason;
                MdInsightSignInResult again = await worker.SignInAsync(account, progress.CancelToken);
                if (again.Outcome == EnumMdInsightCallOutcome.Success
                    && IsSignInMediOrgMatched(result.MediOrgCode, account, again, out mismatchReason))
                {
                    check = await worker.CheckErrorAsync(again.Token, result.SentFileName, progress.CancelToken);
                }
            }

            ApplyCheckOutcome(result, check);
            Save(result);

            progress.SetProgress(1, 1, string.Format(Resources.ResourceMessageLang.MdInsightDangLayKetQua, 1, 1));
        }

        /// <summary>Hien thuc bang suy trang thai tai PTTK muc A.2.6 - giong het man Xuat XML 130</summary>
        private void ApplyCheckOutcome(MdInsightResultADO result, MdInsightCheckResult check)
        {
            long now = GetNowTimeNumber();

            switch (check.Outcome)
            {
                case EnumMdInsightCallOutcome.Success:
                    result.Errors = check.Errors;
                    result.ErrorNum = check.Errors.Count;
                    result.CriticalNum = check.Errors.Count(o => o.IsCritical);
                    result.Status = result.CriticalNum > 0
                        ? EnumXmlPrecheckStatus.Critical
                        : EnumXmlPrecheckStatus.Warning;
                    result.FailReason = EnumMdInsightFailReason.None;
                    result.Reason = null;
                    result.CheckTime = now;
                    break;

                case EnumMdInsightCallOutcome.EmptyResult:
                    ApplyEmptyOutcome(result, now);
                    break;

                case EnumMdInsightCallOutcome.Unauthorized:
                    SetCheckFailed(result, EnumMdInsightFailReason.Unauthorized,
                        Resources.ResourceMessageLang.MdInsightLyDoSaiXacThuc, now);
                    break;

                case EnumMdInsightCallOutcome.Timeout:
                    SetCheckFailed(result, EnumMdInsightFailReason.Timeout,
                        Resources.ResourceMessageLang.MdInsightLyDoQuaHanGoi, now);
                    break;

                case EnumMdInsightCallOutcome.Cancelled:
                    break;

                default:
                    SetCheckFailed(result, EnumMdInsightFailReason.SystemError,
                        Resources.ResourceMessageLang.MdInsightLyDoHeNgoaiLoi, now);
                    break;
            }
        }

        /// <summary>
        /// He ngoai bao "khong co ho so dang tim kiem" - nhap nhang nhat, phai dua vao thoi diem gui.
        /// ⚠️ Thu tu co chu y: kiem han giu 24 gio TRUOC roi moi kiem thoi gian giam dinh toi thieu.
        /// </summary>
        private void ApplyEmptyOutcome(MdInsightResultADO result, long now)
        {
            if (!result.SendTime.HasValue)
            {
                result.Status = EnumXmlPrecheckStatus.NotChecked;
                result.Reason = null;
                return;
            }

            double elapsedSecond = GetElapsedSecond(result.SendTime.Value, now);

            LogSystem.Info("XmlPrecheck - Ho so " + result.TreatmentId
                + ": he ngoai tra ve rong, da gui " + (int)elapsedSecond + "s truoc"
                + " (nguong giam dinh toi thieu " + this.config.MinCheckDurationSecond + "s"
                + ", han giu " + this.config.PendingHoldSecond + "s).");

            if (elapsedSecond >= this.config.PendingHoldSecond)
            {
                SetCheckFailed(result, EnumMdInsightFailReason.PendingExpired,
                    Resources.ResourceMessageLang.MdInsightLyDoQuaHanChoKetQua, now);
                return;
            }

            if (elapsedSecond >= this.config.MinCheckDurationSecond)
            {
                result.Status = EnumXmlPrecheckStatus.NoError;
                result.ErrorNum = 0;
                result.CriticalNum = 0;
                result.Errors = new List<MdInsightErrorADO>();
                result.FailReason = EnumMdInsightFailReason.None;
                result.Reason = null;
                result.CheckTime = now;
                return;
            }

            result.Status = EnumXmlPrecheckStatus.Pending;
            result.FailReason = EnumMdInsightFailReason.NotReadyYet;
            result.Reason = Resources.ResourceMessageLang.MdInsightLyDoChuaDuThoiGian;
            result.CheckTime = null;
        }

        private static void SetCheckFailed(
            MdInsightResultADO result, EnumMdInsightFailReason reason, string message, long now)
        {
            result.Status = EnumXmlPrecheckStatus.CheckFailed;
            result.FailReason = reason;
            result.Reason = message;
            result.CheckTime = now;
            result.ErrorNum = null;
            result.CriticalNum = null;
            result.Errors = new List<MdInsightErrorADO>();
        }

        private static async Task WaitBeforeQueryAsync(frmTienGiamDinhProgress progress, int waitSecond)
        {
            for (int i = 0; i < waitSecond; i++)
            {
                if (progress.IsCancelled)
                {
                    return;
                }

                progress.SetProgress(3, 5, string.Format(Resources.ResourceMessageLang.MdInsightDangCho, waitSecond - i));
                await Task.Delay(TimeSpan.FromSeconds(1), progress.CancelToken);
            }
        }

        #endregion

        #region Ham dung chung

        private MdInsightResultADO CreateShell(HIS_TREATMENT treatment)
        {
            MdInsightResultADO result = new MdInsightResultADO();
            if (treatment == null)
            {
                return result;
            }

            result.TreatmentId = treatment.ID;
            result.TreatmentCode = treatment.TREATMENT_CODE;
            result.PatientName = treatment.TDL_PATIENT_NAME;
            result.MediOrgCode = GetMediOrgCodeOfBranch(treatment.BRANCH_ID);
            result.Status = EnumXmlPrecheckStatus.NotChecked;
            return result;
        }

        /// <summary>
        /// Ma co so KCB theo bao hiem cua chi nhanh - dung gia tri se nam trong the MA_CSKCB cua tep.
        /// Khong dung MEDI_ORG_CODE (ma noi bo) hay TDL_HEIN_MEDI_ORG_CODE (noi dang ky ban dau) cua ho so.
        /// </summary>
        private string GetMediOrgCodeOfBranch(long branchId)
        {
            try
            {
                if (this.mediOrgByBranch == null)
                {
                    this.mediOrgByBranch = BackendDataWorker.Get<HIS_BRANCH>()
                        .Where(o => o != null && !string.IsNullOrWhiteSpace(o.HEIN_MEDI_ORG_CODE))
                        .GroupBy(o => o.ID)
                        .ToDictionary(g => g.Key, g => g.First().HEIN_MEDI_ORG_CODE.Trim());
                }

                string code;
                return this.mediOrgByBranch.TryGetValue(branchId, out code) ? code : "";
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                return "";
            }
        }

        /// <summary>
        /// Doi chieu ma co so DA KHAI voi ma co so DOC DUOC tu chuoi xac thuc; va ma co so cua HO SO
        /// voi co so cua tai khoan. Lech thi KHONG gui - cung quy tac voi man Xuat XML 130.
        /// </summary>
        private static bool IsSignInMediOrgMatched(
            string recordMediOrgCode, MdInsightAccountADO account, MdInsightSignInResult signIn,
            out string mismatchReason)
        {
            mismatchReason = null;
            try
            {
                string fromToken = (signIn.MediOrgCode ?? "").Trim();
                string declared = (account.MediOrgCode ?? "").Trim();
                string ofRecord = (recordMediOrgCode ?? "").Trim();

                if (string.IsNullOrEmpty(fromToken))
                {
                    LogSystem.Info("XmlPrecheck - Khong doc duoc ma co so tu chuoi xac thuc, bo qua buoc doi chieu.");
                    return true;
                }

                if (!string.IsNullOrEmpty(ofRecord)
                    && !string.Equals(ofRecord, fromToken, StringComparison.OrdinalIgnoreCase))
                {
                    LogSystem.Warn("XmlPrecheck - LECH CO SO: ho so thuoc co so " + ofRecord
                        + " nhung tai khoan dang dung thuoc co so " + fromToken + ".");
                    mismatchReason = Resources.ResourceMessageLang.MdInsightLyDoLechCoSo;
                    return false;
                }

                if (!string.IsNullOrEmpty(declared)
                    && !string.Equals(declared, fromToken, StringComparison.OrdinalIgnoreCase))
                {
                    LogSystem.Warn("XmlPrecheck - KHAI NHAM TAI KHOAN: cau hinh khai ma co so " + declared
                        + " nhung tai khoan thuc te thuoc co so " + fromToken + ".");
                    mismatchReason = Resources.ResourceMessageLang.MdInsightLyDoKhaiNhamCoSo;
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                return true;
            }
        }

        private void LogNoAccountForMediOrg(string mediOrgCode)
        {
            try
            {
                List<string> declared = this.config != null ? this.config.DeclaredMediOrgCodes : new List<string>();
                LogSystem.Warn("XmlPrecheck - THIEU TAI KHOAN: ho so thuoc co so "
                    + (string.IsNullOrWhiteSpace(mediOrgCode) ? "(trong)" : mediOrgCode)
                    + " nhung khoa cau hinh khong khai tai khoan cho co so nay. Dang khai: "
                    + (declared.Count > 0 ? string.Join(", ", declared.ToArray()) : "(khong co)") + ".");
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        /// <summary>Ghi ket qua xuong CSDL va giu ban trong phien (co dien giai day du)</summary>
        private void Save(MdInsightResultADO result)
        {
            try
            {
                Dictionary<long, string> failMessages;
                MdInsightApiAdapter.SaveResults(new List<MdInsightResultADO> { result }, out failMessages);
                this.sessionResults[result.TreatmentId] = result;

                if (failMessages.Count > 0)
                {
                    LogSystem.Warn("XmlPrecheck - Ghi nhan ket qua that bai cho ho so " + result.TreatmentId + ".");
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void Show(Form owner, string message, MessageBoxIcon icon)
        {
            try
            {
                this.LastShownMessage = message;
                XtraMessageBox.Show(owner, message, Resources.ResourceMessageLang.ThongBao, MessageBoxButtons.OK, icon);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private static long GetNowTimeNumber()
        {
            return Int64.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));
        }

        private static double GetElapsedSecond(long sendTime, long now)
        {
            try
            {
                DateTime? from = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(sendTime);
                DateTime? to = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(now);
                if (!from.HasValue || !to.HasValue)
                {
                    return 0;
                }
                return (to.Value - from.Value).TotalSeconds;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                return 0;
            }
        }

        #endregion
    }
}
