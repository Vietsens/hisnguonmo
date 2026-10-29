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
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ExportXmlQD130
{
    /// <summary>
    /// Viec 61380 - Tu dong xuat XML 130 cua ho so dieu tri noi tru, ngoai tru co so vao vien khi benh nhan
    /// vao khoa dieu tri du 48 gio (tinh tu CLINICAL_IN_TIME), ghi ra Thu muc luu XML ho so du 48 gio.
    ///
    /// Chi chay khi vien bat HIS.QD_130_BYT.AUTO_EXPORT_XML_AFTER_48H = 1, man hinh dang mo va da khai thu muc.
    /// Moi ho so chi xuat thanh cong mot lan: ket qua ghi vao HIS_TREATMENT qua api/HisTreatment/UpdateXml48hResult,
    /// lan quet sau chi lay ho so chua co ket qua thanh cong (ho so loi tu xuat lai).
    ///
    /// Chay nen, nap du lieu vao ban giu RIENG (LoadXmlBatchDataIsolated) nen khong dung cham cac luong
    /// Xuat Xml, Gui tu dong, Kiem tra ho so dang dung bo truong du lieu cua man hinh.
    /// </summary>
    public partial class UCExportXml : HIS.Desktop.Utility.UserControlBase
    {
        #region Khai bao

        /// <summary>Chu ky quet, tinh bang phut</summary>
        private const int XML48H_INTERVAL_MINUTE = 10;

        /// <summary>So gio ke tu thoi gian vao dieu tri thi ho so du dieu kien xuat</summary>
        private const int XML48H_HOURS = 48;

        /// <summary>Chi quet ho so du 48 gio trong bay nhieu ngay gan nhat - khong xuat bu ho so cu hon</summary>
        private const int XML48H_SCAN_DAYS = 7;

        /// <summary>So ho so toi da moi luot, phan con lai de luot sau</summary>
        private const int XML48H_MAX_PER_ROUND = 50;

        private const short XML48H_RESULT__SUCCESS = 1;
        private const short XML48H_RESULT__FAIL = 2;

        private const string URI_UPDATE_XML48H_RESULT = "api/HisTreatment/UpdateXml48hResult";

        private System.Windows.Forms.Timer timerXml48h;
        private volatile bool xml48hRunning = false;
        private GridColumn gridColXml48h;
        private ToolTipController toolTipXml48h;

        private bool IsXml48hEnabled
        {
            get { return HisConfigCFG.QD_130_BYT__AUTO_EXPORT_XML_AFTER_48H == "1"; }
        }

        #endregion

        #region Khoi tao

        /// <summary>
        /// Goi cuoi su kien Load (sau khi da doc thu muc luu tu trang thai dieu khien).
        /// Vien khong bat cau hinh thi khong tao cot, khong chay tien trinh - man hinh giu nguyen.
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
                RestartXml48hTimer();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Chay lai tien trinh theo thu muc dang khai: goi khi mo man va moi lan luu thu muc.
        /// Chua khai thu muc (hoac xoa trang) thi dung tien trinh.
        /// </summary>
        internal void RestartXml48hTimer()
        {
            try
            {
                if (!this.IsXml48hEnabled)
                {
                    return;
                }

                string folder = this.savePathADO != null ? this.savePathADO.pathXml48h : null;
                if (String.IsNullOrWhiteSpace(folder))
                {
                    if (this.timerXml48h != null)
                    {
                        this.timerXml48h.Stop();
                    }
                    LogSystem.Info("Xml48h - Chua khai Thu muc luu XML ho so du 48 gio, khong chay tien trinh tu dong xuat.");
                    return;
                }

                if (this.timerXml48h == null)
                {
                    this.timerXml48h = new System.Windows.Forms.Timer();
                    this.timerXml48h.Interval = XML48H_INTERVAL_MINUTE * 60000;
                    this.timerXml48h.Tick += TimerXml48h_Tick;
                }

                this.timerXml48h.Stop();
                this.timerXml48h.Start();
                LogSystem.Info("Xml48h - Bat tien trinh tu dong xuat XML ho so du 48 gio, chu ky " + XML48H_INTERVAL_MINUTE + " phut.");

                //Luot dau chay ngay, khong doi het chu ky dau tien
                TimerXml48h_Tick(null, null);
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
                if (this.timerXml48h != null)
                {
                    this.timerXml48h.Stop();
                    this.timerXml48h.Dispose();
                    this.timerXml48h = null;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }

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

        #region Tien trinh tu dong xuat

        /// <summary>
        /// Chay tren luong giao dien (Timer cua WinForms): chi chot tham so roi day viec nang sang luong nen.
        /// Luot truoc chua xong thi bo qua luot den han.
        /// </summary>
        private void TimerXml48h_Tick(object sender, EventArgs e)
        {
            try
            {
                if (this.xml48hRunning)
                {
                    LogSystem.Info("Xml48h - Luot truoc chua xong, bo qua luot nay.");
                    return;
                }

                string folder = this.savePathADO != null ? this.savePathADO.pathXml48h : null;
                if (!this.IsXml48hEnabled || String.IsNullOrWhiteSpace(folder))
                {
                    return;
                }

                long branchId = WorkPlace.GetBranchId();
                bool isXml3176 = chkXML3176.Checked;

                this.xml48hRunning = true;
                Task.Run(() =>
                {
                    try
                    {
                        RunXml48hRound(folder, branchId, isXml3176);
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

        /// <summary>Mot luot quet: lay ho so du dieu kien, xuat tung ho so, ghi ket qua mot lan cuoi luot</summary>
        private void RunXml48hRound(string folder, long branchId, bool isXml3176)
        {
            List<V_HIS_TREATMENT_1> treatments = GetXml48hTreatments(branchId);
            if (treatments == null || treatments.Count == 0)
            {
                LogSystem.Info("Xml48h - Khong co ho so nao du 48 gio can xuat.");
                return;
            }

            LogSystem.Info("Xml48h - Luot nay xuat " + treatments.Count + " ho so vao thu muc " + folder);

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
                results = ExportXml48hFiles(treatments, folder, isXml3176);
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
        }

        /// <summary>
        /// Ho so dieu tri ngoai tru (2), noi tru (3) co so vao vien, thuoc chi nhanh cua phong dang lam viec,
        /// vao dieu tri du 48 gio trong 7 ngay gan nhat va chua xuat thanh cong.
        /// Bo ho so ra vien truoc khi du 48 gio. Xep vao dieu tri som truoc, lay toi da 50 ho so.
        /// </summary>
        private List<V_HIS_TREATMENT_1> GetXml48hTreatments(long branchId)
        {
            List<V_HIS_TREATMENT_1> result = null;
            try
            {
                DateTime dueTime = DateTime.Now.AddHours(-XML48H_HOURS);

                HisTreatmentView1Filter filter = new HisTreatmentView1Filter();
                if (branchId > 0)
                {
                    filter.BRANCH_IDs = new List<long> { branchId };
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

                    //Theo o XML 3176 tai thoi diem bat dau luot, khong doc lai control tu luong nen
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
        /// Ghi ket qua ca luot mot lan. Goi loi thi chi ghi log: ho so chua co ket qua thanh cong
        /// nen luot sau tu xuat lai.
        /// </summary>
        private void SaveXml48hResults(List<HisTreatmentXml48hResultSDO> results)
        {
            try
            {
                if (results == null || results.Count == 0)
                {
                    return;
                }

                CommonParam param = new CommonParam();
                bool success = new BackendAdapter(param).Post<bool>(URI_UPDATE_XML48H_RESULT, ApiConsumers.MosConsumer, results, param);
                if (!success)
                {
                    LogSystem.Warn("Xml48h - Ghi ket qua " + results.Count + " ho so that bai. "
                        + (param.Messages != null ? String.Join("; ", param.Messages) : ""));
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
