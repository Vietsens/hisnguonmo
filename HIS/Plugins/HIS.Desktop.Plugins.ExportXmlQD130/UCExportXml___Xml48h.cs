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
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using His.Bhyt.ExportXml.XML130;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.ExportXmlQD130.ADO;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ExportXmlQD130
{
    /// <summary>
    /// Viec 61380 - Xuat XML 130 cua ho so dieu tri ngoai tru, noi tru co so vao vien khi benh nhan
    /// vao khoa dieu tri du 48 gio (tinh tu CLINICAL_IN_TIME), ghi ra Thu muc luu XML ho so du 48 gio (nut Thu muc luu).
    ///
    /// Hai cach chay, dung chung bo ham xuat:
    /// - Tu dong: tich "Tu dong xuat XML ho so vao khoa dieu tri du 48 gio" trong Thiet lap gui ho so tu dong
    ///   (nhu cac o dong bo QD 3176) roi bat Gui tu dong. Moi chu ky Gui tu dong quet mot luot.
    /// - Xuat tay: chuot phai ho so dang chon, chon Xuat XML ho so du 48 gio. Dung khi ho so loi khong xuat duoc tu dong,
    ///   ho so qua 7 ngay ma luot tu dong khong quet toi, hoac may khong bat Gui tu dong.
    ///
    /// Chi co tac dung khi vien bat HIS.QD_130_BYT.AUTO_EXPORT_XML_AFTER_48H = 1.
    /// Ket qua ghi vao HIS_TREATMENT qua api/HisTreatment/UpdateXml48hResult; luot tu dong chi lay ho so
    /// chua co ket qua thanh cong (ho so loi tu xuat lai).
    ///
    /// Nap du lieu vao ban giu RIENG (LoadXmlBatchDataIsolated) nen khong dung cham cac luong
    /// Xuat Xml, Gui tu dong, Kiem tra ho so dang dung bo truong du lieu cua man hinh.
    /// </summary>
    public partial class UCExportXml : HIS.Desktop.Utility.UserControlBase
    {
        #region Khai bao

        /// <summary>So gio ke tu thoi gian vao dieu tri thi ho so du dieu kien xuat</summary>
        private const int XML48H_HOURS = 48;

        /// <summary>Luot tu dong chi quet ho so du 48 gio trong bay nhieu ngay gan nhat - ho so cu hon thi xuat tay</summary>
        private const int XML48H_SCAN_DAYS = 7;

        /// <summary>So ho so toi da moi luot tu dong, cung la co lo nap du lieu khi xuat tay</summary>
        private const int XML48H_MAX_PER_ROUND = 50;

        /// <summary>So ket qua toi da moi lan goi UpdateXml48hResult (gioi han cua may chu)</summary>
        private const int XML48H_MAX_RESULT_PER_CALL = 200;

        /// <summary>So dong chi tiet toi da trong thong bao ket qua xuat tay, phan con lai xem log</summary>
        private const int XML48H_MAX_MESSAGE_LINES = 30;

        private const short XML48H_RESULT__SUCCESS = 1;
        private const short XML48H_RESULT__FAIL = 2;

        private const string URI_UPDATE_XML48H_RESULT = "api/HisTreatment/UpdateXml48hResult";

        /// <summary>Dang co luot xuat (tu dong hoac xuat tay) - khong chay chong de khong ghi trung file</summary>
        private volatile bool xml48hRunning = false;
        private GridColumn gridColXml48h;
        private ToolTipController toolTipXml48h;

        private bool IsXml48hEnabled
        {
            get { return HisConfigCFG.QD_130_BYT__AUTO_EXPORT_XML_AFTER_48H == "1"; }
        }

        /// <summary>Thu muc luu XML ho so du 48 gio dang khai, null khi chua khai</summary>
        private string Xml48hFolder
        {
            get
            {
                return this.savePathADO != null && !String.IsNullOrWhiteSpace(this.savePathADO.pathXml48h)
                    ? this.savePathADO.pathXml48h.Trim()
                    : null;
            }
        }

        #endregion

        #region Khoi tao

        /// <summary>
        /// Goi cuoi su kien Load. Vien khong bat cau hinh thi khong tao cot - man hinh giu nguyen.
        /// Tien trinh tu dong khong khoi dong o day: chay theo chu ky Gui tu dong (DispatchXml48hAutoRound).
        /// </summary>
        internal void InitXml48h()
        {
            try
            {
                if (!this.IsXml48hEnabled)
                {
                    return;
                }

                CreateXml48hColumn();
                this.Disposed += UCExportXml_Xml48hDisposed;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void UCExportXml_Xml48hDisposed(object sender, EventArgs e)
        {
            try
            {
                if (this.toolTipXml48h != null)
                {
                    this.toolTipXml48h.Dispose();
                    this.toolTipXml48h = null;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Tu dong - chay theo chu ky Gui tu dong

        /// <summary>
        /// Goi tu autoSync_Tick (luong giao dien) moi chu ky Gui tu dong, doc lap voi luong gui cong BHYT:
        /// chu ky khong co ho so can gui cong van quet XML 48 gio.
        /// Chi chot tham so roi day viec nang sang luong nen. Luot truoc chua xong thi bo qua luot den han.
        /// </summary>
        internal void DispatchXml48hAutoRound()
        {
            try
            {
                if (!this.IsXml48hEnabled || this.configSync == null || !this.configSync.isAutoXml48h || this.cancelAutoSyncRequested)
                {
                    return;
                }

                string folder = this.Xml48hFolder;
                if (folder == null)
                {
                    LogSystem.Info("Xml48h - Chua khai Thu muc luu XML ho so du 48 gio, bo qua luot tu dong.");
                    return;
                }

                if (this.xml48hRunning)
                {
                    LogSystem.Info("Xml48h - Luot truoc chua xong, bo qua luot nay.");
                    return;
                }

                //Chi nhanh theo Thiet lap gui ho so tu dong; de trong thi lay chi nhanh cua phong dang lam viec
                List<long> branchIds = this.configSync.branchIds != null && this.configSync.branchIds.Count > 0
                    ? new List<long>(this.configSync.branchIds)
                    : new List<long> { WorkPlace.GetBranchId() };
                //Theo o XML 3176 cua Thiet lap gui ho so tu dong, giong luong gui cong
                bool isXml3176 = this.configSync.isXML3176;

                this.xml48hRunning = true;
                Task.Run(() =>
                {
                    try
                    {
                        RunXml48hAutoRound(folder, branchIds, isXml3176);
                    }
                    catch (Exception ex)
                    {
                        LogSystem.Error(ex);
                    }
                    finally
                    {
                        this.xml48hRunning = false;
                    }
                });
            }
            catch (Exception ex)
            {
                this.xml48hRunning = false;
                LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Goi luc bat Gui tu dong: da tich xuat XML 48 gio ma chua khai thu muc thi nhac mot lan, khong chan Gui tu dong.
        /// </summary>
        internal void WarnXml48hFolderMissingOnAutoStart()
        {
            try
            {
                if (this.IsXml48hEnabled && this.configSync != null && this.configSync.isAutoXml48h && this.Xml48hFolder == null)
                {
                    XtraMessageBox.Show(Resources.ResourceMessageLang.Xml48hCanhBaoTuDongChuaKhaiThuMuc, Resources.ResourceMessageLang.ThongBao);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        /// <summary>Mot luot tu dong: lay ho so du dieu kien, xuat va ghi ket qua</summary>
        private void RunXml48hAutoRound(string folder, List<long> branchIds, bool isXml3176)
        {
            List<V_HIS_TREATMENT_1> treatments = GetXml48hTreatments(branchIds);
            if (treatments == null || treatments.Count == 0)
            {
                LogSystem.Info("Xml48h - Khong co ho so nao du 48 gio can xuat.");
                return;
            }

            LogSystem.Info("Xml48h - Luot tu dong xuat " + treatments.Count + " ho so vao thu muc " + folder);
            ExportAndSaveXml48h(treatments, folder, isXml3176);
        }

        /// <summary>
        /// Ho so dieu tri ngoai tru (2), noi tru (3) co so vao vien, thuoc cac chi nhanh truyen vao,
        /// vao dieu tri du 48 gio trong 7 ngay gan nhat va chua xuat thanh cong.
        /// Bo ho so ra vien truoc khi du 48 gio. Xep vao dieu tri som truoc, lay toi da 50 ho so.
        /// </summary>
        private List<V_HIS_TREATMENT_1> GetXml48hTreatments(List<long> branchIds)
        {
            List<V_HIS_TREATMENT_1> result = null;
            try
            {
                DateTime dueTime = DateTime.Now.AddHours(-XML48H_HOURS);

                HisTreatmentView1Filter filter = new HisTreatmentView1Filter();
                List<long> validBranchIds = branchIds != null ? branchIds.Where(o => o > 0).ToList() : null;
                if (validBranchIds != null && validBranchIds.Count > 0)
                {
                    filter.BRANCH_IDs = validBranchIds;
                }
                filter.TDL_TREATMENT_TYPE_IDs = new List<long>
                {
                    IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNGOAITRU,
                    IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU
                };
                filter.HAS_IN_CODE = true;
                filter.CLINICAL_IN_TIME_FROM = Convert.ToInt64(dueTime.AddDays(-XML48H_SCAN_DAYS).ToString("yyyyMMddHHmmss"));
                filter.CLINICAL_IN_TIME_TO = Convert.ToInt64(dueTime.ToString("yyyyMMddHHmmss"));
                filter.HAS_XML_48H_SUCCESS = false;

                CommonParam param = new CommonParam();
                List<V_HIS_TREATMENT_1> data = new BackendAdapter(param).Get<List<V_HIS_TREATMENT_1>>("api/HisTreatment/GetView1", ApiConsumers.MosConsumer, filter, param);
                if (data == null || data.Count == 0)
                {
                    return data;
                }

                result = data
                    .Where(o => o.CLINICAL_IN_TIME.HasValue && !IsOutBeforeXml48h(o))
                    .GroupBy(o => o.ID)
                    .Select(g => g.First())
                    .OrderBy(o => o.CLINICAL_IN_TIME.Value)
                    .ThenBy(o => o.ID)
                    .Take(XML48H_MAX_PER_ROUND)
                    .ToList();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                result = null;
            }
            return result;
        }

        #endregion

        #region Xuat tay - chuot phai tren luoi

        /// <summary>
        /// Xuat XML 48 gio cho cac ho so dang chon, khong can bat Gui tu dong.
        /// Ho so chua du dieu kien thi bo qua kem ly do; ho so da xuat thanh cong thi hoi truoc khi xuat lai (ghi de file).
        /// </summary>
        private async void MenuItemClick_XuatXml48h(object sender, EventArgs e)
        {
            try
            {
                if (!this.IsXml48hEnabled || this.listSelection == null || this.listSelection.Count == 0)
                {
                    return;
                }

                if (this.xml48hRunning)
                {
                    XtraMessageBox.Show(Resources.ResourceMessageLang.Xml48hDangChay, Resources.ResourceMessageLang.ThongBao);
                    return;
                }

                //Chua khai thu muc thi mo Thu muc luu nhu nut Xuat Xml
                if (this.Xml48hFolder == null)
                {
                    btnSavePath_Click(null, null);
                }
                string folder = this.Xml48hFolder;
                if (folder == null)
                {
                    XtraMessageBox.Show(Resources.ResourceMessageLang.Xml48hChuaKhaiThuMuc, Resources.ResourceMessageLang.ThongBao);
                    return;
                }

                DateTime now = DateTime.Now;
                List<V_HIS_TREATMENT_1> eligible = new List<V_HIS_TREATMENT_1>();
                List<string> skipLines = new List<string>();
                foreach (V_HIS_TREATMENT_1 treatment in this.listSelection.GroupBy(o => o.ID).Select(g => g.First()))
                {
                    string reason = GetXml48hIneligibleReason(treatment, now);
                    if (reason == null)
                    {
                        eligible.Add(treatment);
                    }
                    else
                    {
                        skipLines.Add(treatment.TREATMENT_CODE + ": " + reason);
                        LogSystem.Info("Xml48h - Xuat tay bo qua ho so " + treatment.TREATMENT_CODE + ": " + reason);
                    }
                }

                int exportedCount = eligible.Count(o => o.XML_48H_RESULT == XML48H_RESULT__SUCCESS);
                if (exportedCount > 0
                    && XtraMessageBox.Show(String.Format(Resources.ResourceMessageLang.Xml48hHoiXuatLai, exportedCount),
                        Resources.ResourceMessageLang.ThongBao, MessageBoxButtons.YesNo) != DialogResult.Yes)
                {
                    return;
                }

                List<HisTreatmentXml48hResultSDO> results = new List<HisTreatmentXml48hResultSDO>();
                if (eligible.Count > 0)
                {
                    bool isXml3176 = chkXML3176.Checked;
                    this.xml48hRunning = true;
                    WaitingManager.Show();
                    try
                    {
                        LogSystem.Info("Xml48h - Xuat tay " + eligible.Count + " ho so vao thu muc " + folder);
                        results = await Task.Run(() => ExportAndSaveXml48h(eligible, folder, isXml3176));
                    }
                    finally
                    {
                        this.xml48hRunning = false;
                        WaitingManager.Hide();
                    }
                }

                ShowXml48hManualResult(eligible, results, skipLines);
            }
            catch (Exception ex)
            {
                this.xml48hRunning = false;
                WaitingManager.Hide();
                LogSystem.Error(ex);
            }
        }

        /// <summary>Ly do ho so chua du dieu kien xuat XML 48 gio (cung dieu kien luot tu dong), null khi du dieu kien</summary>
        private static string GetXml48hIneligibleReason(V_HIS_TREATMENT_1 treatment, DateTime now)
        {
            bool isTreatment = treatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNGOAITRU
                || treatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU;
            if (!isTreatment || String.IsNullOrWhiteSpace(treatment.IN_CODE))
            {
                return Resources.ResourceMessageLang.Xml48hLyDoKhongPhaiDieuTri;
            }

            DateTime? inTime = treatment.CLINICAL_IN_TIME.HasValue
                ? Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(treatment.CLINICAL_IN_TIME.Value)
                : null;
            if (!inTime.HasValue)
            {
                return Resources.ResourceMessageLang.Xml48hLyDoChuaVaoKhoa;
            }

            DateTime dueTime = inTime.Value.AddHours(XML48H_HOURS);
            if (now < dueTime)
            {
                return String.Format(Resources.ResourceMessageLang.Xml48hLyDoChuaDu48h,
                    Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Convert.ToInt64(dueTime.ToString("yyyyMMddHHmmss"))));
            }

            if (IsOutBeforeXml48h(treatment))
            {
                return Resources.ResourceMessageLang.Xml48hLyDoRaVienTruoc48h;
            }

            return null;
        }

        /// <summary>Thong bao ket qua xuat tay: dong tong, roi tung ho so (loi, khong du dieu kien, thanh cong)</summary>
        private void ShowXml48hManualResult(List<V_HIS_TREATMENT_1> eligible, List<HisTreatmentXml48hResultSDO> results, List<string> skipLines)
        {
            List<string> failLines = new List<string>();
            List<string> okLines = new List<string>();
            foreach (HisTreatmentXml48hResultSDO result in results)
            {
                V_HIS_TREATMENT_1 treatment = eligible.FirstOrDefault(o => o.ID == result.TreatmentId);
                string code = treatment != null ? treatment.TREATMENT_CODE : result.TreatmentId.ToString();
                if (result.IsSuccess)
                {
                    okLines.Add(code + ": " + result.FilePath);
                }
                else
                {
                    failLines.Add(code + ": " + result.Description);
                }
            }

            List<string> lines = failLines.Concat(skipLines).Concat(okLines).ToList();
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(String.Format(Resources.ResourceMessageLang.Xml48hKetQuaXuatTay, okLines.Count, failLines.Count, skipLines.Count));
            sb.AppendLine();
            foreach (string line in lines.Take(XML48H_MAX_MESSAGE_LINES))
            {
                sb.AppendLine(line);
            }
            if (lines.Count > XML48H_MAX_MESSAGE_LINES)
            {
                sb.AppendLine(String.Format(Resources.ResourceMessageLang.Xml48hVaConLai, lines.Count - XML48H_MAX_MESSAGE_LINES));
            }

            XtraMessageBox.Show(sb.ToString().Trim(), Resources.ResourceMessageLang.ThongBao);
        }

        #endregion

        #region Xuat file va ghi ket qua - dung chung tu dong va xuat tay

        /// <summary>Ho so da ra vien truoc khi du 48 gio ke tu thoi gian vao dieu tri</summary>
        private static bool IsOutBeforeXml48h(V_HIS_TREATMENT_1 treatment)
        {
            if (!treatment.OUT_TIME.HasValue || !treatment.CLINICAL_IN_TIME.HasValue)
            {
                return false;
            }

            DateTime? inTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(treatment.CLINICAL_IN_TIME.Value);
            DateTime? outTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(treatment.OUT_TIME.Value);
            if (!inTime.HasValue || !outTime.HasValue)
            {
                return false;
            }

            return outTime.Value < inTime.Value.AddHours(XML48H_HOURS);
        }

        /// <summary>
        /// Chay tren luong nen: kiem thu muc, xuat theo lo 50 ho so, ghi log tung ho so,
        /// ghi ket qua len may chu va cap nhat cot XML 48h tren luoi.
        /// </summary>
        private List<HisTreatmentXml48hResultSDO> ExportAndSaveXml48h(List<V_HIS_TREATMENT_1> treatments, string folder, bool isXml3176)
        {
            List<HisTreatmentXml48hResultSDO> results = new List<HisTreatmentXml48hResultSDO>();
            string folderError = CheckXml48hFolder(folder);
            if (!String.IsNullOrEmpty(folderError))
            {
                foreach (V_HIS_TREATMENT_1 treatment in treatments)
                {
                    results.Add(Xml48hFail(treatment.ID, folderError));
                }
            }
            else
            {
                for (int skip = 0; skip < treatments.Count; skip += XML48H_MAX_PER_ROUND)
                {
                    results.AddRange(ExportXml48hFiles(treatments.Skip(skip).Take(XML48H_MAX_PER_ROUND).ToList(), folder, isXml3176));
                }
            }

            foreach (HisTreatmentXml48hResultSDO result in results)
            {
                V_HIS_TREATMENT_1 treatment = treatments.FirstOrDefault(o => o.ID == result.TreatmentId);
                string code = treatment != null ? treatment.TREATMENT_CODE : result.TreatmentId.ToString();
                if (result.IsSuccess)
                {
                    LogSystem.Info("Xml48h - Ho so " + code + ": xuat thanh cong " + result.FilePath);
                }
                else
                {
                    LogSystem.Warn("Xml48h - Ho so " + code + ": xuat loi. " + result.Description);
                }
            }

            SaveXml48hResults(results);
            ApplyXml48hResultsToGrid(results);
            return results;
        }

        /// <summary>
        /// Thu muc phai ton tai va ghi duoc (ghi roi xoa mot tep thu).
        /// Tra ve ly do loi, hoac null khi dung duoc.
        /// </summary>
        private static string CheckXml48hFolder(string folder)
        {
            try
            {
                if (!Directory.Exists(folder))
                {
                    return String.Format(Resources.ResourceMessageLang.Xml48hLyDoKhongTruyCapThuMuc, folder);
                }

                string probe = Path.Combine(folder, ".xml48h_" + Guid.NewGuid().ToString("N") + ".tmp");
                File.WriteAllBytes(probe, new byte[] { 0 });
                File.Delete(probe);
                return null;
            }
            catch (Exception ex)
            {
                LogSystem.Warn("Xml48h - Khong ghi duoc thu muc " + folder + ": " + ex.Message);
                return String.Format(Resources.ResourceMessageLang.Xml48hLyDoKhongTruyCapThuMuc, folder);
            }
        }

        /// <summary>
        /// Nap du lieu ca lo vao ban giu rieng, dung dau vao bang dung ham cua luong Gui tu dong / MDInsight,
        /// sinh tep bang che do ho so dang dieu tri (RunInTreatment) roi ghi vao thu muc.
        /// Ho so loi thi ghi ly do va lam tiep ho so sau.
        /// </summary>
        private List<HisTreatmentXml48hResultSDO> ExportXml48hFiles(List<V_HIS_TREATMENT_1> treatments, string folder, bool isXml3176)
        {
            List<HisTreatmentXml48hResultSDO> results = new List<HisTreatmentXml48hResultSDO>();
            Dictionary<long, V_HIS_TREATMENT_12> loadedById = new Dictionary<long, V_HIS_TREATMENT_12>();
            XmlBuildContextADO ctx = null;

            try
            {
                //Tham so Cong tiep nhan BHXH: luong nay KHONG gui cong, nhung thu vien dung tep doc mot so tham so tu day
                string[] qd130 = ReadQd130ConnectionParts();
                this.NewConfig = GetNewConfig();

                List<V_HIS_TREATMENT_12> loaded;
                ctx = LoadXmlBatchDataIsolated(treatments, qd130[3], qd130[1], qd130[2], qd130[0], qd130[4], qd130[5], out loaded);
                if (loaded != null)
                {
                    foreach (V_HIS_TREATMENT_12 item in loaded)
                    {
                        loadedById[item.ID] = item;
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }

            foreach (V_HIS_TREATMENT_1 treatment in treatments)
            {
                try
                {
                    V_HIS_TREATMENT_12 treatment12;
                    if (ctx == null || !loadedById.TryGetValue(treatment.ID, out treatment12))
                    {
                        results.Add(Xml48hFail(treatment.ID, Resources.ResourceMessageLang.Xml48hLyDoKhongNapDuLieu));
                        continue;
                    }

                    bool sendXml12;
                    InputADO ado = BuildInputAdoForXml(ctx, treatment12, out sendXml12);
                    if (ado == null)
                    {
                        results.Add(Xml48hFail(treatment.ID, Resources.ResourceMessageLang.Xml48hLyDoKhongCoDichVu));
                        continue;
                    }

                    //Chot tu luong giao dien truoc khi chay nen, khong doc lai control tu luong nen
                    ado.IS_3176 = isXml3176;

                    CreateXmlProcessor processor = new CreateXmlProcessor(ado);
                    string error = "";
                    using (MemoryStream stream = processor.RunInTreatment(ref error))
                    {
                        if (stream == null || stream.Length == 0)
                        {
                            results.Add(Xml48hFail(treatment.ID, String.IsNullOrWhiteSpace(error) ? Resources.ResourceMessageLang.Xml48hLyDoKhongSinhDuocFile : error));
                            continue;
                        }

                        string filePath = Path.Combine(folder, processor.GetFileName());
                        try
                        {
                            File.WriteAllBytes(filePath, stream.ToArray());
                        }
                        catch (Exception exWrite)
                        {
                            LogSystem.Warn("Xml48h - Ghi tep loi " + filePath + ": " + exWrite.Message);
                            results.Add(Xml48hFail(treatment.ID, String.Format(Resources.ResourceMessageLang.Xml48hLyDoGhiFileLoi, exWrite.Message)));
                            continue;
                        }

                        results.Add(new HisTreatmentXml48hResultSDO
                        {
                            TreatmentId = treatment.ID,
                            IsSuccess = true,
                            FilePath = filePath
                        });
                    }
                }
                catch (Exception ex)
                {
                    LogSystem.Error(ex);
                    results.Add(Xml48hFail(treatment.ID, Resources.ResourceMessageLang.Xml48hLyDoKhongSinhDuocFile));
                }
            }

            return results;
        }

        private static HisTreatmentXml48hResultSDO Xml48hFail(long treatmentId, string reason)
        {
            return new HisTreatmentXml48hResultSDO
            {
                TreatmentId = treatmentId,
                IsSuccess = false,
                Description = reason
            };
        }

        /// <summary>
        /// Ghi ket qua theo lo 200 (gioi han cua may chu). Goi loi thi chi ghi log: ho so chua co ket qua
        /// thanh cong nen luot tu dong sau tu xuat lai.
        /// </summary>
        private void SaveXml48hResults(List<HisTreatmentXml48hResultSDO> results)
        {
            try
            {
                if (results == null || results.Count == 0)
                {
                    return;
                }

                for (int skip = 0; skip < results.Count; skip += XML48H_MAX_RESULT_PER_CALL)
                {
                    List<HisTreatmentXml48hResultSDO> batch = results.Skip(skip).Take(XML48H_MAX_RESULT_PER_CALL).ToList();
                    CommonParam param = new CommonParam();
                    bool success = new BackendAdapter(param).Post<bool>(URI_UPDATE_XML48H_RESULT, ApiConsumers.MosConsumer, batch, param);
                    if (!success)
                    {
                        LogSystem.Warn("Xml48h - Ghi ket qua " + batch.Count + " ho so that bai. "
                            + (param.Messages != null ? String.Join("; ", param.Messages) : ""));
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Cap nhat cot XML 48h cua cac dong dang hien tren luoi, khong phai tim lai.
        /// Day ve luong giao dien vi luoi dang doc chinh cac doi tuong nay.
        /// </summary>
        private void ApplyXml48hResultsToGrid(List<HisTreatmentXml48hResultSDO> results)
        {
            try
            {
                if (results == null || results.Count == 0 || this.IsDisposed || !this.IsHandleCreated)
                {
                    return;
                }

                long now = Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));
                this.BeginInvoke(new Action(() =>
                {
                    try
                    {
                        if (this.listTreatment1 == null || this.listTreatment1.Count == 0)
                        {
                            return;
                        }

                        bool changed = false;
                        foreach (HisTreatmentXml48hResultSDO result in results)
                        {
                            foreach (V_HIS_TREATMENT_1 row in this.listTreatment1.Where(o => o.ID == result.TreatmentId))
                            {
                                //Da xuat thanh cong thi khong de bang ket qua loi - dung quy tac cua may chu
                                if (!result.IsSuccess && row.XML_48H_RESULT == XML48H_RESULT__SUCCESS)
                                {
                                    continue;
                                }
                                row.XML_48H_RESULT = result.IsSuccess ? XML48H_RESULT__SUCCESS : XML48H_RESULT__FAIL;
                                row.XML_48H_TIME = now;
                                row.XML_48H_FILE_PATH = result.IsSuccess ? result.FilePath : null;
                                row.XML_48H_DESC = result.IsSuccess ? null : result.Description;
                                changed = true;
                            }
                        }

                        if (changed)
                        {
                            this.gridControlTreatment.RefreshDataSource();
                        }
                    }
                    catch (Exception ex)
                    {
                        LogSystem.Warn(ex);
                    }
                }));
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Cot XML 48h tren luoi

        /// <summary>Them cot XML 48h ngay sau hai cot Check-in</summary>
        private void CreateXml48hColumn()
        {
            try
            {
                this.gridColXml48h = this.gridViewTreatment.Columns.AddField("XML_48H_RESULT");
                this.gridColXml48h.Caption = Resources.ResourceMessageLang.Xml48hCotKetQua;
                this.gridColXml48h.Name = "gridColXml48h";
                this.gridColXml48h.OptionsColumn.AllowEdit = false;
                this.gridColXml48h.Width = 70;
                this.gridColXml48h.Visible = true;
                int checkInIndex = Math.Max(this.gridColumn2.VisibleIndex, this.gridColumn1.VisibleIndex);
                this.gridColXml48h.VisibleIndex = checkInIndex >= 0 ? checkInIndex + 1 : this.gridViewTreatment.VisibleColumns.Count;

                //Dang ky them - khong go bo trinh xu ly nao san co cua man hinh
                this.gridViewTreatment.CustomColumnDisplayText += GridViewTreatment_Xml48hDisplayText;
                this.gridViewTreatment.RowCellStyle += GridViewTreatment_Xml48hRowCellStyle;

                if (this.gridControlTreatment.ToolTipController == null)
                {
                    this.toolTipXml48h = new ToolTipController();
                    this.gridControlTreatment.ToolTipController = this.toolTipXml48h;
                }
                this.gridControlTreatment.ToolTipController.GetActiveObjectInfo += GridControlTreatment_Xml48hGetActiveObjectInfo;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void GridViewTreatment_Xml48hDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
        {
            try
            {
                if (e.Column == null || e.Column != this.gridColXml48h)
                {
                    return;
                }

                short? value = e.Value as short?;
                if (value == XML48H_RESULT__SUCCESS)
                {
                    e.DisplayText = Resources.ResourceMessageLang.Xml48hDaXuat;
                }
                else if (value == XML48H_RESULT__FAIL)
                {
                    e.DisplayText = Resources.ResourceMessageLang.Xml48hLoi;
                }
                else
                {
                    e.DisplayText = "";
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void GridViewTreatment_Xml48hRowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            try
            {
                if (e.Column != this.gridColXml48h)
                {
                    return;
                }

                V_HIS_TREATMENT_1 row = this.gridViewTreatment.GetRow(e.RowHandle) as V_HIS_TREATMENT_1;
                if (row == null || !row.XML_48H_RESULT.HasValue)
                {
                    return;
                }

                e.Appearance.ForeColor = row.XML_48H_RESULT == XML48H_RESULT__SUCCESS ? Color.Green : Color.Red;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        /// <summary>Re chuot vao o XML 48h: thoi diem va duong dan tep (da xuat) hoac ly do (loi)</summary>
        private void GridControlTreatment_Xml48hGetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {
                if (e.SelectedControl != this.gridControlTreatment)
                {
                    return;
                }

                GridHitInfo info = this.gridViewTreatment.CalcHitInfo(e.ControlMousePosition);
                if (info == null || !info.InRowCell || info.Column != this.gridColXml48h)
                {
                    return;
                }

                V_HIS_TREATMENT_1 row = this.gridViewTreatment.GetRow(info.RowHandle) as V_HIS_TREATMENT_1;
                if (row == null || !row.XML_48H_RESULT.HasValue)
                {
                    return;
                }

                string time = row.XML_48H_TIME.HasValue
                    ? Inventec.Common.DateTime.Convert.TimeNumberToTimeString(row.XML_48H_TIME.Value)
                    : "";
                string text = row.XML_48H_RESULT == XML48H_RESULT__SUCCESS
                    ? String.Format(Resources.ResourceMessageLang.Xml48hGoiYDaXuatLuc, time) + Environment.NewLine + (row.XML_48H_FILE_PATH ?? "")
                    : String.Format(Resources.ResourceMessageLang.Xml48hGoiYLoiLuc, time) + Environment.NewLine + (row.XML_48H_DESC ?? "");

                e.Info = new ToolTipControlInfo(new CellToolTipInfo(info.RowHandle, info.Column, "xml48h"), text);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        #endregion
    }
}
