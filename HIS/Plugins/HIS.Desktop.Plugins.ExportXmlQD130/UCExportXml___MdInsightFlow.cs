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
using HIS.Desktop.Plugins.ExportXmlQD130.ADO;
using HIS.Desktop.Plugins.ExportXmlQD130.Base;
using Inventec.Common.Logging;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ExportXmlQD130
{
    /// <summary>
    /// vCong XXXXX - Hai luong chay cua tinh nang soat loi MDInsight.
    ///
    /// Diem mau chot ve tinh dung dan: he ngoai tra ve "khong co ho so dang tim kiem" cho CA BA
    /// truong hop - chua gui, dang giam dinh do, va ho so sach. Phan hoi khong du de ket luan.
    /// Vi vay trang thai duoc suy tu PHAN HOI CONG VOI THOI DIEM GUI ma phan mem tu luu
    /// (<see cref="ApplyCheckOutcome"/>) - PTTK muc A.2.6, day la nguon duy nhat.
    /// </summary>
    public partial class UCExportXml : HIS.Desktop.Utility.UserControlBase
    {
        #region Luong chay - MOT nut duy nhat

        /// <summary>
        /// Mot luot bam nut. Phan mem TU QUYET DINH tung ho so nen gui hay chi tra,
        /// dua vao trang thai da luu (chot ngay 2026-09-16):
        ///
        ///  - Chua tung gui, hoac lan gui truoc hong (trang thai 5) -> GUI moi
        ///  - Da gui, chua co ket qua (trang thai 4)                -> CHI TRA, khong gui lai
        ///  - Da co ket qua (trang thai 1/2/3)                      -> HOI, dong y thi gui lai
        ///
        /// Ly do khong tu dong gui lai ho so da co ket qua: tich ca trang roi bam nut se day
        /// hang tram tep trung len he ngoai, trong khi phan lon truong hop nguoi dung chi muon
        /// xem lai ket qua cu.
        /// </summary>
        private async Task<List<MdInsightResultADO>> RunMdInsightAsync(List<V_HIS_TREATMENT_1> treatments)
        {
            List<MdInsightResultADO> results = new List<MdInsightResultADO>();

            try
            {
                Dictionary<long, V_HIS_TREATMENT_1> treatmentById =
                    treatments.GroupBy(o => o.ID).ToDictionary(g => g.Key, g => g.First());

                //Doc trang thai da luu de biet tung ho so dang o dau
                results = BuildResultsFromStored(treatments);

                List<MdInsightResultADO> toSend;
                if (!PartitionSendAndQuery(results, out toSend))
                {
                    //Nguoi dung dong hop xac nhan gui lai
                    return new List<MdInsightResultADO>();
                }

                if (!ConfirmMdInsightBatch(treatments.Count, toSend.Count > 0))
                {
                    return new List<MdInsightResultADO>();
                }

                MdInsightWorker worker = new MdInsightWorker(this.mdInsightConfig);

                using (frmTienGiamDinhProgress progress = new frmTienGiamDinhProgress(
                    treatments.Count,
                    Resources.ResourceMessageLang.MdInsightTieuDeTienTrinh,
                    Resources.ResourceMessageLang.MdInsightNutHuy))
                {
                    this.Enabled = false;
                    progress.Show(this.ParentForm);

                    try
                    {
                        Dictionary<string, string> tokenByMediOrg = new Dictionary<string, string>();
                        bool hasSent = false;

                        if (toSend.Count > 0)
                        {
                            //Quy tac QT-05: kiem hien trang MOT LAN, va CHI khi co tep can gui
                            if (!await worker.HealthCheckAsync(progress.CancelToken))
                            {
                                XtraMessageBox.Show(
                                    Resources.ResourceMessageLang.MdInsightHeNgoaiKhongPhanHoi,
                                    Resources.ResourceMessageLang.ThongBao,
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return results;
                            }

                            //Quy tac QT-09: nhom theo co so, moi nhom mot tai khoan rieng
                            foreach (var group in toSend.GroupBy(o => (o.MediOrgCode ?? "").Trim()))
                            {
                                if (progress.IsCancelled)
                                {
                                    break;
                                }

                                List<MdInsightResultADO> groupResults = group.ToList();

                                await SendOneMediOrgGroupAsync(
                                    worker, group.Key, groupResults, treatmentById, tokenByMediOrg, progress);

                                //Quy tac QT-09: ghi nhan da gui PHAI hoan tat truoc khi kiem tin hieu huy
                                SaveMdInsightResults(groupResults);

                                hasSent |= groupResults.Any(o => o.Status == EnumXmlPrecheckStatus.Pending);
                            }

                            if (hasSent)
                            {
                                //Cho he ngoai kip bat dau giam dinh
                                await WaitBeforeQueryAsync(progress, this.mdInsightConfig.WaitAfterUploadSecond);
                            }
                        }

                        //Dang nhap not cho cac co so chi tra ma khong gui
                        await EnsureTokensAsync(worker, results, tokenByMediOrg, progress);

                        await QueryResultsOnceAsync(worker, results, tokenByMediOrg, progress);
                    }
                    catch (OperationCanceledException)
                    {
                        LogSystem.Info("MdInsight - Nguoi dung huy giua chung luot chay.");
                    }
                    finally
                    {
                        progress.Close();
                        this.Enabled = true;
                    }
                }

            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }

            LogRunSummary(results);
            return results;
        }

        /// <summary>
        /// Mot dong tong ket cuoi luot chay: dem ho so theo tung trang thai.
        /// Doc dong nay la biet ngay luot vua roi ra sao ma khong phai lan nguoc hang tram dong nhat ky.
        /// Chi ghi ma dieu tri va con so - khong ho ten, khong ma benh nhan (quy tac QT-28).
        /// </summary>
        private void LogRunSummary(List<MdInsightResultADO> results)
        {
            try
            {
                if (results == null || results.Count == 0)
                {
                    return;
                }

                int noError = results.Count(o => o.Status == EnumXmlPrecheckStatus.NoError);
                int warning = results.Count(o => o.Status == EnumXmlPrecheckStatus.Warning);
                int critical = results.Count(o => o.Status == EnumXmlPrecheckStatus.Critical);
                int pending = results.Count(o => o.Status == EnumXmlPrecheckStatus.Pending);
                int failed = results.Count(o => o.Status == EnumXmlPrecheckStatus.CheckFailed);

                LogSystem.Info("MdInsight - TONG KET luot chay: " + results.Count + " ho so"
                    + " | khong loi=" + noError
                    + " | canh bao=" + warning
                    + " | nghiem trong=" + critical
                    + " | chua co ket qua=" + pending
                    + " | khong kiem tra duoc=" + failed);

                //Ho so khong kiem tra duoc thi liet ke ma dieu tri kem ly do, gop theo ly do
                //de lo lon khong sinh ra hang tram dong giong het nhau.
                if (failed > 0)
                {
                    var byReason = results
                        .Where(o => o.Status == EnumXmlPrecheckStatus.CheckFailed)
                        .GroupBy(o => o.Reason ?? "(khong ro ly do)");

                    foreach (var group in byReason)
                    {
                        List<string> ids = group.Take(20).Select(o => o.TreatmentId.ToString()).ToList();
                        LogSystem.Warn("MdInsight - Khong kiem tra duoc " + group.Count() + " ho so."
                            + " Ly do: " + group.Key
                            + " | ma dieu tri: " + String.Join(", ", ids.ToArray())
                            + (group.Count() > ids.Count ? " (+" + (group.Count() - ids.Count) + " ho so nua)" : ""));
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        /// <summary>Dung ket qua ban dau tu trang thai DA LUU trong co so du lieu</summary>
        private List<MdInsightResultADO> BuildResultsFromStored(List<V_HIS_TREATMENT_1> treatments)
        {
            List<MdInsightResultADO> results = new List<MdInsightResultADO>();

            Dictionary<long, HisTreatmentXmlPrecheckDetailADO> stored =
                MdInsightApiAdapter.GetDetails(treatments.Select(o => o.ID).ToList());

            foreach (V_HIS_TREATMENT_1 treatment in treatments)
            {
                MdInsightResultADO result = CreateResultShell(treatment);

                HisTreatmentXmlPrecheckDetailADO detail;
                if (stored.TryGetValue(treatment.ID, out detail) && detail != null)
                {
                    result.SentFileName = detail.XmlPrecheckFileName;
                    result.SendTime = detail.XmlPrecheckSendTime;
                    result.CheckTime = detail.XmlPrecheckTime;
                    result.ErrorNum = detail.XmlPrecheckErrNum;
                    result.CriticalNum = detail.XmlPrecheckCrtNum;
                    result.Status = detail.XmlPrecheckResult.HasValue
                        ? (EnumXmlPrecheckStatus)detail.XmlPrecheckResult.Value
                        : EnumXmlPrecheckStatus.NotChecked;
                }

                results.Add(result);
            }

            return results;
        }

        /// <summary>
        /// Chia ho so thanh nhom CAN GUI va nhom CHI TRA.
        /// Tra ve false khi nguoi dung dong hop xac nhan gui lai - khi do khong lam gi ca.
        /// </summary>
        private bool PartitionSendAndQuery(List<MdInsightResultADO> results, out List<MdInsightResultADO> toSend)
        {
            toSend = new List<MdInsightResultADO>();

            //Da co ket qua roi - phai hoi truoc khi gui lai
            List<MdInsightResultADO> hasResult = results.Where(o => o.IsFinalStatus
                && o.Status != EnumXmlPrecheckStatus.CheckFailed).ToList();

            bool resendExisting = false;

            if (hasResult.Count > 0)
            {
                DialogResult answer = XtraMessageBox.Show(
                    string.Format(Resources.ResourceMessageLang.MdInsightXacNhanGuiLai, hasResult.Count),
                    Resources.ResourceMessageLang.ThongBao,
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                if (answer == DialogResult.Cancel)
                {
                    return false;
                }

                resendExisting = answer == DialogResult.Yes;
            }

            foreach (MdInsightResultADO result in results)
            {
                bool neverSent = !result.SendTime.HasValue || string.IsNullOrEmpty(result.SentFileName);

                if (neverSent
                    || result.Status == EnumXmlPrecheckStatus.CheckFailed
                    || (resendExisting && result.IsFinalStatus))
                {
                    toSend.Add(result);
                }
            }

            LogSystem.Info("MdInsight - Phan loai lo: " + toSend.Count + " ho so se gui, "
                + (results.Count - toSend.Count) + " ho so chi tra ket qua.");

            return true;
        }

        /// <summary>
        /// Dang nhap cho nhung co so chua co chuoi xac thuc - la cac co so chi tra ma khong gui.
        /// Co so dang nhap hong thi rieng ho so cua no mang Khong kiem tra duoc (quy tac QT-09b).
        /// </summary>
        private async Task EnsureTokensAsync(
            MdInsightWorker worker, List<MdInsightResultADO> results,
            Dictionary<string, string> tokenByMediOrg, frmTienGiamDinhProgress progress)
        {
            //Chi can dang nhap cho ho so thuc su se duoc tra
            List<IGrouping<string, MdInsightResultADO>> groups = results
                .Where(o => !string.IsNullOrEmpty(o.SentFileName) && o.SendTime.HasValue)
                .Where(o => o.Status == EnumXmlPrecheckStatus.Pending
                            || o.Status == EnumXmlPrecheckStatus.NotChecked)
                .GroupBy(o => (o.MediOrgCode ?? "").Trim())
                .ToList();

            foreach (var group in groups)
            {
                if (progress.IsCancelled || tokenByMediOrg.ContainsKey(group.Key))
                {
                    continue;
                }

                MdInsightAccountADO account = this.mdInsightConfig.GetAccount(group.Key);
                if (account == null)
                {
                    LogNoAccountForMediOrg(group.Key);
                    MarkGroupFailed(group.ToList(), EnumMdInsightFailReason.NoAccountForMediOrg,
                        Resources.ResourceMessageLang.MdInsightLyDoChuaKhaiTaiKhoan);
                    continue;
                }

                MdInsightSignInResult signIn = await worker.SignInAsync(account, progress.CancelToken);

                if (signIn.Outcome != EnumMdInsightCallOutcome.Success)
                {
                    MarkGroupFailed(group.ToList(), EnumMdInsightFailReason.Unauthorized,
                        Resources.ResourceMessageLang.MdInsightLyDoSaiXacThuc);
                    continue;
                }

                string mismatchReason;
                if (!IsSignInMediOrgMatched(group.Key, account, signIn, out mismatchReason))
                {
                    MarkGroupFailed(group.ToList(), EnumMdInsightFailReason.Unauthorized, mismatchReason);
                    continue;
                }

                tokenByMediOrg[group.Key] = signIn.Token;
            }
        }

        /// <summary>
        /// Dang nhap, ket xuat tep va gui ca nhom ho so cua MOT co so kham chua benh.
        /// Tuyet doi khong gui tep cua co so nay bang tai khoan cua co so khac - quy tac QT-09.
        /// </summary>
        private async Task SendOneMediOrgGroupAsync(
            MdInsightWorker worker, string mediOrgCode, List<MdInsightResultADO> results,
            Dictionary<long, V_HIS_TREATMENT_1> treatmentById,
            Dictionary<string, string> tokenByMediOrg, frmTienGiamDinhProgress progress)
        {
            try
            {
                //Gui lai la mot lan kiem MOI - xoa sach ket qua lan truoc (quy tac QT-07)
                foreach (MdInsightResultADO item in results)
                {
                    item.SentFileName = null;
                    item.SendTime = null;
                    item.CheckTime = null;
                    item.ErrorNum = null;
                    item.CriticalNum = null;
                    item.Reason = null;
                    item.Errors = new List<MdInsightErrorADO>();
                    item.Status = EnumXmlPrecheckStatus.NotChecked;
                }

                //Quy tac QT-09b: co so chua khai tai khoan thi rieng nhom nay khong kiem duoc,
                //CAC NHOM KHAC VAN CHAY BINH THUONG
                MdInsightAccountADO account = this.mdInsightConfig.GetAccount(mediOrgCode);
                if (account == null)
                {
                    LogNoAccountForMediOrg(mediOrgCode);
                    MarkGroupFailed(results, EnumMdInsightFailReason.NoAccountForMediOrg,
                        Resources.ResourceMessageLang.MdInsightLyDoChuaKhaiTaiKhoan);
                    return;
                }

                MdInsightSignInResult signIn = await worker.SignInAsync(account, progress.CancelToken);
                if (signIn.Outcome != EnumMdInsightCallOutcome.Success)
                {
                    MarkGroupFailed(results, EnumMdInsightFailReason.Unauthorized,
                        Resources.ResourceMessageLang.MdInsightLyDoSaiXacThuc);
                    return;
                }

                //Quy tac QT-28: tai khoan phai dung thuoc co so cua ho so, neu khong thi KHONG gui
                string mismatchReason;
                if (!IsSignInMediOrgMatched(mediOrgCode, account, signIn, out mismatchReason))
                {
                    MarkGroupFailed(results, EnumMdInsightFailReason.Unauthorized, mismatchReason);
                    return;
                }

                tokenByMediOrg[mediOrgCode] = signIn.Token;

                //Ho so goc cua nhom - buoc nap du lieu dung tep XML can chinh cac doi tuong nay
                List<V_HIS_TREATMENT_1> groupTreatments = new List<V_HIS_TREATMENT_1>();
                foreach (MdInsightResultADO item in results)
                {
                    V_HIS_TREATMENT_1 treatment;
                    if (treatmentById.TryGetValue(item.TreatmentId, out treatment))
                    {
                        groupTreatments.Add(treatment);
                    }
                }

                //Ket xuat tep cho ca nhom, dung LAI duong nap du lieu cua luong gui Cong BHXH
                List<MdInsightUploadItem> uploadItems = BuildXmlFilesForGroup(groupTreatments, results, progress);
                if (uploadItems.Count == 0)
                {
                    return;
                }

                progress.SetProgress(0, groupTreatments.Count,
                    string.Format(Resources.ResourceMessageLang.MdInsightDangGui, 0, uploadItems.Count));

                MdInsightUploadResult upload = await worker.UploadFilesAsync(
                    signIn.Token, uploadItems, progress.CancelToken);

                long sendTime = GetNowTimeNumber();

                foreach (MdInsightResultADO result in results)
                {
                    if (result.Status == EnumXmlPrecheckStatus.CheckFailed
                        || string.IsNullOrEmpty(result.SentFileName))
                    {
                        continue;
                    }

                    bool failed = upload.Outcome != EnumMdInsightCallOutcome.Success
                        || upload.FailedFileNames.Contains(result.SentFileName);

                    if (failed)
                    {
                        //Gui that bai: XOA ten tep va thoi diem gui de lan "Lay ket qua" sau
                        //khong tra nham sang tep cu - PTTK muc B.3.2.1 buoc 5
                        result.SentFileName = null;
                        result.SendTime = null;
                        result.Status = EnumXmlPrecheckStatus.CheckFailed;
                        result.FailReason = EnumMdInsightFailReason.UploadFailed;
                        result.Reason = Resources.ResourceMessageLang.MdInsightLyDoGuiThatBai;
                        result.CheckTime = GetNowTimeNumber();
                    }
                    else
                    {
                        result.Status = EnumXmlPrecheckStatus.Pending;
                        result.FailReason = EnumMdInsightFailReason.None;
                        result.Reason = Resources.ResourceMessageLang.MdInsightLyDoChoKetQua;
                        result.SendTime = sendTime;
                        result.CheckTime = null;
                    }
                }
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                MarkGroupFailed(results, EnumMdInsightFailReason.SystemError,
                    Resources.ResourceMessageLang.MdInsightLyDoHeNgoaiLoi);
            }

            return;
        }

        /// <summary>
        /// Ket xuat tep XML cho ca nhom ho so cua mot co so.
        /// Ho so thieu du lieu bat buoc thi ghi nhan Khong kiem tra duoc va BO QUA RIENG ho so do,
        /// cac ho so con lai van chay tiep - quy tac QT-08.
        /// </summary>
        private List<MdInsightUploadItem> BuildXmlFilesForGroup(
            List<V_HIS_TREATMENT_1> groupTreatments, List<MdInsightResultADO> results,
            frmTienGiamDinhProgress progress)
        {
            List<MdInsightUploadItem> items = new List<MdInsightUploadItem>();

            try
            {
                //Tham so ket noi Cong tiep nhan bao hiem xa hoi: luong soat loi KHONG gui cong nay,
                //nhung thu vien dung tep van doc mot so tham so tu day nen phai truyen day du.
                string[] qd130 = ReadQd130ConnectionParts();

                XmlBuildContextADO ctx = LoadXmlBatchData(
                    groupTreatments, qd130[3], qd130[1], qd130[2], qd130[0], qd130[4], qd130[5]);

                if (ctx == null || this.HisTreatments == null)
                {
                    MarkGroupFailed(results, EnumMdInsightFailReason.ExportFailed,
                        Resources.ResourceMessageLang.MdInsightLyDoKhongKetXuatDuoc);
                    return items;
                }

                Dictionary<long, MdInsightResultADO> resultById = results.ToDictionary(o => o.TreatmentId);
                int done = 0;

                foreach (V_HIS_TREATMENT_12 treatment in this.HisTreatments)
                {
                    if (progress.IsCancelled)
                    {
                        break;
                    }

                    MdInsightResultADO result;
                    if (!resultById.TryGetValue(treatment.ID, out result))
                    {
                        continue;
                    }

                    bool sendXml12;
                    InputADO ado = BuildInputAdoForXml(ctx, treatment, out sendXml12);

                    if (ado == null)
                    {
                        SetExportFailed(result);
                        continue;
                    }

                    CreateXmlProcessor processor = new CreateXmlProcessor(ado);
                    string error = "";
                    MemoryStream stream = processor.Run(ref error);

                    if (stream == null || stream.Length == 0)
                    {
                        //Hoi thu vien xem thieu truong nao. Khong co cau nay thi nguoi dung chi biet
                        //"chua du du lieu bat buoc" ma khong biet phai bo sung gi - quy tac QT-08.
                        string detail = DescribeExportFailure(processor, treatment);

                        LogSystem.Warn("MdInsight - Khong ket xuat duoc tep XML cho ho so " + treatment.ID
                            + (String.IsNullOrWhiteSpace(detail) ? "" : ". Thieu: " + detail)
                            + (String.IsNullOrWhiteSpace(error) ? "" : ". Thu vien bao: " + error));

                        SetExportFailed(result, detail);
                        continue;
                    }

                    result.SentFileName = processor.GetFileName();
                    items.Add(new MdInsightUploadItem
                    {
                        FileName = result.SentFileName,
                        Content = stream.ToArray()
                    });

                    done++;
                    progress.SetProgress(done, groupTreatments.Count,
                        string.Format(Resources.ResourceMessageLang.MdInsightDangGui, done, groupTreatments.Count));
                }

                //Ho so nam trong lo nhung khong duoc thu vien tra ve thi cung la khong ket xuat duoc
                foreach (MdInsightResultADO result in results)
                {
                    if (string.IsNullOrEmpty(result.SentFileName)
                        && result.Status != EnumXmlPrecheckStatus.CheckFailed)
                    {
                        SetExportFailed(result);
                    }
                }

                //Thong ke buoc ket xuat. Lo gui rong thi phai phan biet duoc la "khong ho so nao
                //ket xuat duoc" hay "da gui nhung he ngoai khong nhan" - hai viec khac han nhau.
                long totalBytes = 0;
                foreach (MdInsightUploadItem item in items)
                {
                    totalBytes += (item.Content != null ? item.Content.Length : 0);
                }
                //Ca lo nay chung mot co so nen lay ma o ban ghi dau tien la du
                string groupMediOrgCode = results.Count > 0 ? (results[0].MediOrgCode ?? "") : "";

                LogSystem.Info("MdInsight - Ket xuat XML co so " + groupMediOrgCode + ": "
                    + items.Count + "/" + groupTreatments.Count + " ho so ra duoc tep, tong "
                    + (totalBytes / 1024) + " KB, " + (groupTreatments.Count - items.Count)
                    + " ho so khong ket xuat duoc.");
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                MarkGroupFailed(results, EnumMdInsightFailReason.ExportFailed,
                    Resources.ResourceMessageLang.MdInsightLyDoKhongKetXuatDuoc);
            }

            return items;
        }

        #endregion


        #region Vong tra ket qua va suy trang thai

        /// <summary>
        /// Tra ket qua DUNG MOT VONG, moi ho so MOT luot goi rieng.
        ///
        /// Khong gop lo: goi ca lo thi he ngoai chi tra ket qua cua nhung tep no co va AM THAM
        /// BO QUA cac tep con lai, khong bao tep nao bi bo - khi do khong phan biet duoc
        /// "ho so sach" voi "ho so chua giam dinh xong" (quy tac QT-10).
        ///
        /// Cham han muc ca vong thi tra quyen dieu khien lai cho nguoi dung; cac ho so chua kip tra
        /// GIU NGUYEN trang thai Chua co ket qua - khong mat mat gi, lay lai duoc bang nut Lay ket qua.
        /// </summary>
        private async Task QueryResultsOnceAsync(
            MdInsightWorker worker, List<MdInsightResultADO> results,
            Dictionary<string, string> tokenByMediOrg, frmTienGiamDinhProgress progress)
        {
            Stopwatch roundClock = Stopwatch.StartNew();
            List<MdInsightResultADO> pendingSave = new List<MdInsightResultADO>();

            //Moi co so chi duoc dang nhap lai DUNG MOT LAN trong ca luot chay - bang nguong dong 6
            HashSet<string> reSignedInMediOrgs = new HashSet<string>();

            List<MdInsightResultADO> queryable = results
                .Where(o => !string.IsNullOrEmpty(o.SentFileName) && o.SendTime.HasValue)
                .Where(o => o.Status == EnumXmlPrecheckStatus.Pending
                            || o.Status == EnumXmlPrecheckStatus.NotChecked)
                .ToList();

            int done = 0;

            try
            {
                foreach (MdInsightResultADO result in queryable)
                {
                    if (progress.IsCancelled)
                    {
                        break;
                    }

                    if (roundClock.Elapsed.TotalSeconds >= MDINSIGHT_ROUND_LIMIT_SECOND)
                    {
                        LogSystem.Info("MdInsight - Cham han muc ca vong tra sau " + done
                            + "/" + queryable.Count + " ho so. Cac ho so con lai giu trang thai Chua co ket qua.");
                        break;
                    }

                    string mediOrgCode = (result.MediOrgCode ?? "").Trim();

                    string token;
                    if (!tokenByMediOrg.TryGetValue(mediOrgCode, out token))
                    {
                        continue;
                    }

                    MdInsightCheckResult check = await worker.CheckErrorAsync(token, result.SentFileName, progress.CancelToken);

                    //Quy tac QT-12: chuoi xac thuc bi tu choi giua luot chay thi tu dang nhap lai
                    //DUNG MOT LAN cho moi co so roi tra lai. Gioi han mot lan la bat buoc: mat khau
                    //bi doi giua chung se tao vong lap tu choi - dang nhap lai - tu choi khong diem dung.
                    if (check.Outcome == EnumMdInsightCallOutcome.Unauthorized
                        && !reSignedInMediOrgs.Contains(mediOrgCode))
                    {
                        reSignedInMediOrgs.Add(mediOrgCode);

                        MdInsightAccountADO account = this.mdInsightConfig.GetAccount(mediOrgCode);
                        if (account != null)
                        {
                            LogSystem.Info("MdInsight - Chuoi xac thuc bi tu choi giua luot chay,"
                                + " dang nhap lai mot lan cho co so " + mediOrgCode + ".");

                            string mismatchReason;
                            MdInsightSignInResult again = await worker.SignInAsync(account, progress.CancelToken);
                            if (again.Outcome == EnumMdInsightCallOutcome.Success
                                && IsSignInMediOrgMatched(mediOrgCode, account, again, out mismatchReason))
                            {
                                tokenByMediOrg[mediOrgCode] = again.Token;
                                check = await worker.CheckErrorAsync(
                                    again.Token, result.SentFileName, progress.CancelToken);
                            }
                        }
                    }

                    ApplyCheckOutcome(result, check);

                    pendingSave.Add(result);
                    done++;

                    progress.SetProgress(done, queryable.Count,
                        string.Format(Resources.ResourceMessageLang.MdInsightDangLayKetQua, done, queryable.Count));

                    //Quy tac QT-11: luu ngay theo tung nhom, khong doi het lo
                    if (pendingSave.Count >= MDINSIGHT_SAVE_BATCH)
                    {
                        SaveMdInsightResults(pendingSave);
                        pendingSave.Clear();
                    }

                    if (this.mdInsightConfig.RestBetweenCallSecond > 0)
                    {
                        await Task.Delay(
                            TimeSpan.FromSeconds(this.mdInsightConfig.RestBetweenCallSecond), progress.CancelToken);
                    }
                }
            }
            finally
            {
                //Luu not phan du khi ket thuc vong tra hoac khi nguoi dung bam huy
                if (pendingSave.Count > 0)
                {
                    SaveMdInsightResults(pendingSave);
                }
            }
        }

        /// <summary>
        /// Suy trang thai tong the cua mot ho so tu PHAN HOI CONG VOI THOI DIEM GUI da luu.
        ///
        /// Day la hien thuc cua bang suy trang thai tai PTTK muc A.2.6 - nguon duy nhat.
        /// Tram tu dem so dong loi va so dong nghiem trong TRUOC khi goi giao dien luu;
        /// may chu khong tu dem, khong tu suy (quy tac QT-13).
        /// </summary>
        private void ApplyCheckOutcome(MdInsightResultADO result, MdInsightCheckResult check)
        {
            long now = GetNowTimeNumber();

            switch (check.Outcome)
            {
                case EnumMdInsightCallOutcome.Success:
                    //Phan hoi co dong loi: dem va quy ra muc do - quy tac QT-12, QT-14
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
                    //Nguoi dung huy - giu nguyen trang thai da gui, khong ket luan gi
                    break;

                default:
                    SetCheckFailed(result, EnumMdInsightFailReason.SystemError,
                        Resources.ResourceMessageLang.MdInsightLyDoHeNgoaiLoi, now);
                    break;
            }
        }

        /// <summary>
        /// Xu ly nhanh NHAP NHANG nhat: he ngoai bao "khong co ho so dang tim kiem".
        ///
        /// Cau tra loi nay dung cho ca ba truong hop - chua gui, dang giam dinh do, va ho so sach -
        /// nen phai dua vao thoi diem gui da luu de quyet dinh.
        ///
        /// ⚠️ THU TU KIEM TRA LA CO CHU Y: kiem han giu 24 gio TRUOC, roi moi kiem thoi gian
        /// giam dinh toi thieu. Ho so nam o trang thai Chua co ket qua qua 24 gio nghia la lan gui do
        /// da hong (he ngoai khong nhan duoc tep), ket luan "khong co loi" luc do la SAI VA NGUY HIEM -
        /// se de mot ho so loi lot qua roi nop len co quan bao hiem. Bao "khong kiem tra duoc" thi
        /// nguoi dung chi phai gui lai, mat mat nho hon nhieu.
        /// </summary>
        private void ApplyEmptyOutcome(MdInsightResultADO result, long now)
        {
            if (!result.SendTime.HasValue)
            {
                //Chua tung gui - cot trang thai de trong
                result.Status = EnumXmlPrecheckStatus.NotChecked;
                result.Reason = null;
                return;
            }

            double elapsedSecond = GetElapsedSecond(result.SendTime.Value, now);

            //Ghi ro can cu ket luan: da troi qua bao lau, so voi nguong nao.
            //Chi ghi ma dot dieu tri, khong ghi du lieu benh nhan (quy tac QT-28).
            LogSystem.Info("MdInsight - Ho so " + result.TreatmentId
                + ": he ngoai tra ve rong, da gui " + (int)elapsedSecond + "s truoc"
                + " (nguong giam dinh toi thieu " + this.mdInsightConfig.MinCheckDurationSecond + "s"
                + ", han giu " + this.mdInsightConfig.PendingHoldSecond + "s).");

            if (elapsedSecond >= this.mdInsightConfig.PendingHoldSecond)
            {
                SetCheckFailed(result, EnumMdInsightFailReason.PendingExpired,
                    Resources.ResourceMessageLang.MdInsightLyDoQuaHanChoKetQua, now);
                return;
            }

            if (elapsedSecond >= this.mdInsightConfig.MinCheckDurationSecond)
            {
                //Da du thoi gian giam dinh toi thieu ma khong co dong loi nao -> ho so sach
                result.Status = EnumXmlPrecheckStatus.NoError;
                result.ErrorNum = 0;
                result.CriticalNum = 0;
                result.Errors = new List<MdInsightErrorADO>();
                result.FailReason = EnumMdInsightFailReason.None;
                result.Reason = null;
                result.CheckTime = now;
                return;
            }

            //Chua du thoi gian giam dinh - chua ket luan duoc, de nguoi dung lay lai sau
            result.Status = EnumXmlPrecheckStatus.Pending;
            result.FailReason = EnumMdInsightFailReason.NotReadyYet;
            result.Reason = Resources.ResourceMessageLang.MdInsightLyDoChuaDuThoiGian;
            result.CheckTime = null;
        }

        #endregion

        #region Ham dung chung

        private MdInsightResultADO CreateResultShell(V_HIS_TREATMENT_1 treatment)
        {
            return new MdInsightResultADO
            {
                TreatmentId = treatment.ID,
                TreatmentCode = treatment.TREATMENT_CODE,
                PatientName = treatment.TDL_PATIENT_NAME,
                MediOrgCode = GetMediOrgCodeOfTreatment(treatment),
                Status = EnumXmlPrecheckStatus.NotChecked
            };
        }

        /// <summary>
        /// Doi chieu ma co so DA KHAI trong cau hinh voi ma co so DOC DUOC tu chuoi xac thuc -
        /// quy tac QT-28, bat loi khai nham tai khoan cua co so khac.
        ///
        /// Gui tep cua co so A bang tai khoan co so B thi he ngoai VAN NHAN TEP nhung KHONG BAO GIO
        /// tra ket qua (tai khoan bi co lap theo co so). Ho so se bi ket luan "khong co loi" oan sau khi
        /// qua nguong thoi gian giam dinh. Vi vay phat hien lech la DUNG, khong gui.
        ///
        /// ⚠️ Chi ket luan lech khi DOC DUOC ma tu chuoi xac thuc. Khong doc duoc thi coi nhu
        /// khong xac minh duoc va VAN CHAY TIEP - neu khong, nha cung cap doi khuon the mot cai
        /// la ca tinh nang chet o moi vien.
        /// </summary>
        private bool IsSignInMediOrgMatched(
            string recordMediOrgCode, MdInsightAccountADO account, MdInsightSignInResult signIn,
            out string mismatchReason)
        {
            mismatchReason = null;

            try
            {
                string fromToken = (signIn.MediOrgCode ?? "").Trim();
                string declared = (account.MediOrgCode ?? "").Trim();
                string ofRecord = (recordMediOrgCode ?? "").Trim();

                if (String.IsNullOrEmpty(fromToken))
                {
                    LogSystem.Info("MdInsight - Khong doc duoc ma co so tu chuoi xac thuc, bo qua buoc doi chieu.");
                    return true;
                }

                //⚠️ PHEP DOI CHIEU QUAN TRONG NHAT: ma co so cua HO SO phai trung co so cua TAI KHOAN.
                //Tai khoan bi co lap theo co so, nen tep mang ma co so khac se KHONG BAO GIO duoc
                //he ngoai xu ly - no tra ve "khong co ho so" mai mai. Neu khong chan o day thi sau khi
                //qua nguong thoi gian giam dinh, phan mem se ket luan ho so SACH trong khi thuc te
                //no chua he duoc giam dinh - sai lam nguy hiem nhat cua tinh nang nay.
                if (!String.IsNullOrEmpty(ofRecord)
                    && !String.Equals(ofRecord, fromToken, StringComparison.OrdinalIgnoreCase))
                {
                    LogSystem.Warn("MdInsight - LECH CO SO: ho so thuoc co so " + ofRecord
                        + " nhung tai khoan dang dung thuoc co so " + fromToken
                        + ". He ngoai se khong xu ly duoc cac tep nay.");
                    mismatchReason = Resources.ResourceMessageLang.MdInsightLyDoLechCoSo;
                    return false;
                }

                //Cau hinh co khai ma thi ma khai cung phai dung - bat loi khai nham tai khoan
                if (!String.IsNullOrEmpty(declared)
                    && !String.Equals(declared, fromToken, StringComparison.OrdinalIgnoreCase))
                {
                    LogSystem.Warn("MdInsight - KHAI NHAM TAI KHOAN: cau hinh khai ma co so " + declared
                        + " nhung tai khoan thuc te thuoc co so " + fromToken
                        + ". Kiem tra lai khoa HIS.MDINSIGHT.CONNECTION_INFO.");
                    mismatchReason = Resources.ResourceMessageLang.MdInsightLyDoKhaiNhamCoSo;
                    return false;
                }

                if (String.IsNullOrEmpty(declared))
                {
                    LogSystem.Info("MdInsight - Tai khoan dung chung, thuoc co so " + fromToken + ".");
                }

                return true;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                return true;
            }
        }

        /// <summary>Ma co so kham chua benh theo bao hiem, tra cuu theo ma chi nhanh. Dung chung ca luot chay.</summary>
        private Dictionary<long, string> mdInsightMediOrgByBranch;

        /// <summary>
        /// Lay ma co so kham chua benh cua MOT ho so - dung DUNG gia tri se nam trong the MA_CSKCB
        /// cua tep XML gui di, vi tai khoan MDInsight duoc cap theo chinh ma do.
        ///
        /// ⚠️ Gia tri nay lay tu CHI NHANH cua ho so (<c>HIS_BRANCH.HEIN_MEDI_ORG_CODE</c>) -
        /// giong het cach bo sinh tep dat <c>MA_CSKCB</c> (xem Xml1Processor).
        ///
        /// KHONG duoc dung hai truong de nham lan sau day cua ho so dieu tri:
        ///  - <c>MEDI_ORG_CODE</c>: ma to chuc NOI BO cua HIS, khong phai ma co so KCB theo bao hiem.
        ///  - <c>TDL_HEIN_MEDI_ORG_CODE</c>: noi DANG KY KHAM CHUA BENH BAN DAU ghi tren the BHYT
        ///    cua benh nhan (tuc MA_DKBD), khong phai noi dang dieu tri.
        ///
        /// Danh muc chi nhanh nam san trong bo nho dem nen khong phat sinh truy van nao.
        /// </summary>
        private string GetMediOrgCodeOfTreatment(V_HIS_TREATMENT_1 treatment)
        {
            try
            {
                if (this.mdInsightMediOrgByBranch == null)
                {
                    this.mdInsightMediOrgByBranch = BackendDataWorker.Get<HIS_BRANCH>()
                        .Where(o => o != null && !string.IsNullOrWhiteSpace(o.HEIN_MEDI_ORG_CODE))
                        .GroupBy(o => o.ID)
                        .ToDictionary(g => g.Key, g => g.First().HEIN_MEDI_ORG_CODE.Trim());
                }

                string code;
                return this.mdInsightMediOrgByBranch.TryGetValue(treatment.BRANCH_ID, out code) ? code : "";
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                return "";
            }
        }

        /// <summary>
        /// Ghi ro co so nao thieu tai khoan va khoa cau hinh dang khai nhung ma nao.
        /// Khong co dong log nay thi ho so chi im lang mang trang thai Khong kiem tra duoc,
        /// nhin log khong the phan biet la sai khoa cau hinh hay sai du lieu chi nhanh.
        /// </summary>
        private void LogNoAccountForMediOrg(string mediOrgCode)
        {
            try
            {
                List<string> declared = this.mdInsightConfig != null
                    ? this.mdInsightConfig.DeclaredMediOrgCodes
                    : new List<string>();

                LogSystem.Warn("MdInsight - THIEU TAI KHOAN: ho so thuoc co so "
                    + (String.IsNullOrWhiteSpace(mediOrgCode) ? "(trong)" : mediOrgCode)
                    + " nhung khoa cau hinh khong khai tai khoan cho co so nay. Dang khai: "
                    + (declared.Count > 0 ? String.Join(", ", declared.ToArray()) : "(khong co)")
                    + ". Bo ma co so trong khoa cau hinh de dung mot tai khoan chung cho moi co so.");
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void MarkGroupFailed(
            List<MdInsightResultADO> results, EnumMdInsightFailReason reason, string message)
        {
            long now = GetNowTimeNumber();
            foreach (MdInsightResultADO result in results)
            {
                SetCheckFailed(result, reason, message, now);
            }
        }

        /// <summary>
        /// Ghi nhan mot ho so khong ket xuat duoc tep.
        /// XOA ca ten tep va thoi diem gui cu, neu khong thi nut "Lay ket qua" se tra nham
        /// sang tep cua lan kiem truoc va bao ve mot ket qua khong con dung - PTTK muc B.3.2.1 buoc 5.
        /// </summary>
        private void SetExportFailed(MdInsightResultADO result)
        {
            SetExportFailed(result, null);
        }

        /// <summary>
        /// Danh dau ho so khong ket xuat duoc. <paramref name="detail"/> la danh sach thieu sot
        /// cu the do thu vien ket xuat tra ve - co thi ghep vao Ly do de nguoi dung biet phai
        /// bo sung truong nao, khong co thi chi bao chung chung.
        /// </summary>
        private void SetExportFailed(MdInsightResultADO result, string detail)
        {
            result.SentFileName = null;
            result.SendTime = null;

            string reason = Resources.ResourceMessageLang.MdInsightLyDoKhongKetXuatDuoc;
            if (!String.IsNullOrWhiteSpace(detail))
            {
                reason = reason + " " + detail;
            }

            SetCheckFailed(result, EnumMdInsightFailReason.ExportFailed, reason, GetNowTimeNumber());
        }

        /// <summary>
        /// Hoi thu vien ket xuat xem ho so thieu nhung gi. Cac cau tra ve deu la ten truong du lieu,
        /// khong kem ho ten hay ma benh nhan.
        ///
        /// ⚠️ Rieng mot cau co kem MA BENH (ICD) - ma benh la du lieu lam sang cua benh nhan nen
        /// phai che truoc khi ghi vao Ly do va xuong cot XML_PRECHECK_DESC (quy tac QT-28, ca KT-39).
        /// </summary>
        private static string DescribeExportFailure(
            CreateXmlProcessor processor, V_HIS_TREATMENT_12 treatment)
        {
            try
            {
                if (processor == null)
                {
                    return null;
                }

                List<string> reasons = processor.CheckHoSo();
                if (reasons == null || reasons.Count == 0)
                {
                    return null;
                }

                string icdCode = treatment != null ? (treatment.ICD_CODE ?? "").Trim() : "";

                List<string> safe = reasons
                    .Where(o => !String.IsNullOrWhiteSpace(o))
                    .Select(o => String.IsNullOrEmpty(icdCode) ? o : o.Replace(icdCode, "***"))
                    .Take(MAX_EXPORT_REASON)
                    .ToList();

                if (safe.Count == 0)
                {
                    return null;
                }

                return String.Join("; ", safe.ToArray())
                    + (reasons.Count > safe.Count ? "; (còn " + (reasons.Count - safe.Count) + " lỗi nữa)" : "");
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
                return null;
            }
        }

        private void SetCheckFailed(
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

        /// <summary>Cho he ngoai kip bat dau giam dinh, van cho nguoi dung bam huy</summary>
        private async Task WaitBeforeQueryAsync(frmTienGiamDinhProgress progress, int waitSecond)
        {
            for (int i = 0; i < waitSecond; i++)
            {
                if (progress.IsCancelled)
                {
                    return;
                }

                progress.SetProgress(i, waitSecond,
                    string.Format(Resources.ResourceMessageLang.MdInsightDangCho, waitSecond - i));

                await Task.Delay(TimeSpan.FromSeconds(1), progress.CancelToken);
            }
        }

        /// <summary>Ghi nhan ket qua xuong CSDL, va giu ban day du trong bo nho de hien thi</summary>
        private void SaveMdInsightResults(List<MdInsightResultADO> results)
        {
            try
            {
                if (results == null || results.Count == 0)
                {
                    return;
                }

                Dictionary<long, string> failMessages;
                MdInsightApiAdapter.SaveResults(results, out failMessages);

                foreach (MdInsightResultADO result in results)
                {
                    //Ban trong bo nho giu ca phan dien giai day du - phan do khong luu xuong CSDL
                    this.mdInsightSessionResults[result.TreatmentId] = result;
                }

                if (failMessages.Count > 0)
                {
                    LogSystem.Warn("MdInsight - Co " + failMessages.Count + " ho so ghi nhan ket qua that bai.");
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        /// <summary>Cap nhat ba cot ket qua tren luoi ma khong phai nap lai ca danh sach</summary>
        private void ApplyResultsToGrid(List<MdInsightResultADO> results)
        {
            try
            {
                if (this.listTreatment1 == null || this.listTreatment1.Count == 0)
                {
                    return;
                }

                Dictionary<long, MdInsightResultADO> byId = results
                    .GroupBy(o => o.TreatmentId).ToDictionary(g => g.Key, g => g.Last());

                this.gridViewTreatment.BeginUpdate();
                try
                {
                    foreach (V_HIS_TREATMENT_1 row in this.listTreatment1)
                    {
                        MdInsightResultADO result;
                        if (!byId.TryGetValue(row.ID, out result))
                        {
                            continue;
                        }

                        row.XML_PRECHECK_RESULT = (short)result.Status;
                        row.XML_PRECHECK_ERR_NUM = result.ErrorNum;
                        row.XML_PRECHECK_CRT_NUM = result.CriticalNum;
                        row.XML_PRECHECK_TIME = result.CheckTime;
                        row.XML_PRECHECK_FILE_NAME = result.SentFileName;
                        row.XML_PRECHECK_SEND_TIME = result.SendTime;
                    }
                }
                finally
                {
                    this.gridViewTreatment.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Dung lai ket qua de hien thi khi nguoi dung bam vao o trang thai tren luoi.
        /// Uu tien ban trong phien vi ban do con giu phan DIEN GIAI DAY DU; khong co thi doc lai
        /// tu CSDL - khi do phan dien giai trong, kem goi y bam Lay ket qua de tra lai.
        /// </summary>
        private List<MdInsightResultADO> LoadResultsForDisplay(List<V_HIS_TREATMENT_1> treatments)
        {
            List<MdInsightResultADO> results = new List<MdInsightResultADO>();

            try
            {
                List<long> missingIds = new List<long>();

                foreach (V_HIS_TREATMENT_1 treatment in treatments)
                {
                    MdInsightResultADO inSession;
                    if (this.mdInsightSessionResults.TryGetValue(treatment.ID, out inSession))
                    {
                        results.Add(inSession);
                    }
                    else
                    {
                        missingIds.Add(treatment.ID);
                    }
                }

                if (missingIds.Count == 0)
                {
                    return results;
                }

                Dictionary<long, HisTreatmentXmlPrecheckDetailADO> stored =
                    MdInsightApiAdapter.GetDetails(missingIds);

                foreach (V_HIS_TREATMENT_1 treatment in treatments.Where(o => missingIds.Contains(o.ID)))
                {
                    MdInsightResultADO result = CreateResultShell(treatment);

                    HisTreatmentXmlPrecheckDetailADO detail;
                    if (stored.TryGetValue(treatment.ID, out detail) && detail != null)
                    {
                        result.SentFileName = detail.XmlPrecheckFileName;
                        result.SendTime = detail.XmlPrecheckSendTime;
                        result.CheckTime = detail.XmlPrecheckTime;
                        result.ErrorNum = detail.XmlPrecheckErrNum;
                        result.CriticalNum = detail.XmlPrecheckCrtNum;
                        result.Status = detail.XmlPrecheckResult.HasValue
                            ? (EnumXmlPrecheckStatus)detail.XmlPrecheckResult.Value
                            : EnumXmlPrecheckStatus.NotChecked;

                        string reason;
                        int truncated;
                        result.Errors = MdInsightDescCodec.Decode(detail.XmlPrecheckDesc, out reason, out truncated);
                        result.Reason = reason;
                        result.IsTruncated = truncated > 0;
                        result.TruncatedCount = truncated;
                    }

                    results.Add(result);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }

            return results;
        }

        private void ShowMdInsightResultWindow(List<MdInsightResultADO> results)
        {
            try
            {
                if (results == null || results.Count == 0)
                {
                    return;
                }

                using (frmMdInsightResult form = new frmMdInsightResult(results, this))
                {
                    form.ShowDialog(this.ParentForm);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Doc sau thanh phan cua khoa HIS.QD_130_BYT.CONNECTION_INFO theo dung thu tu ban goc:
        /// dia chi, tai khoan, mat khau, loai tep XML, duong dan XML 130, duong dan XML giam dinh y khoa.
        /// Thieu thanh phan nao thi tra chuoi rong cho thanh phan do.
        /// </summary>
        private static string[] ReadQd130ConnectionParts()
        {
            string[] parts = new string[6] { "", "", "", "", "", "" };

            try
            {
                string raw = HisConfigCFG.QD_130_BYT__CONNECTION_INFO;
                if (string.IsNullOrWhiteSpace(raw))
                {
                    return parts;
                }

                string[] source = raw.Split('|');
                for (int i = 0; i < parts.Length && i < source.Length; i++)
                {
                    parts[i] = (source[i] ?? "").Trim();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }

            return parts;
        }

        private static long GetNowTimeNumber()
        {
            return Int64.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));
        }

        /// <summary>So giay da troi qua ke tu thoi diem gui, tinh tu hai moc dang yyyyMMddHHmmss</summary>
        private static double GetElapsedSecond(long sendTime, long now)
        {
            try
            {
                DateTime? from = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(sendTime);
                DateTime? to = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(now);

                if (!from.HasValue || !to.HasValue)
                {
                    //Doc khong ra moc thoi gian thi coi nhu CHUA du thoi gian - thien ve phia an toan,
                    //khong ket luan ho so sach khi khong chac chan
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
