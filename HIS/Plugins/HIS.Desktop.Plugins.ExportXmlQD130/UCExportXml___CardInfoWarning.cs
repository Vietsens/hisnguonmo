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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.ExportXmlQD130.ADO;
using HIS.Desktop.Plugins.ExportXmlQD130.Base;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ExportXmlQD130
{
    /// <summary>
    /// Viec 60261 (BV Da khoa tinh Dien Bien) - canh bao ho so thieu/sai thong tin the, ho so chinh cua XML1
    /// (SO_CCCD, NGAY_SINH, MA_THE_BHYT, MA_DKBD, GT_THE_TU, GT_THE_DEN) de sua truoc, khong phai doi cong BHYT bao loi.
    ///
    /// - Hoi xac nhan truoc cac thao tac xuat/gui XML 130 bang tay: nap lai ho so luc bam, kiem moi the cua dot dieu tri
    ///   (nhu XML1 noi the bang ';'). Co = xuat/gui nhu cu; Khong = mo danh sach canh bao theo ho so roi dung. Chi canh bao, khong chan.
    /// - Hoi truoc menu "Xuat lai file XML check-in server": nap lai ho so, kiem the hien tai (check-in lay the cuoi).
    /// - Cot "Canh bao the, ho so" tren luoi (the hien tai cua ho so): cho thay ho so check-in / gui tu dong can sua,
    ///   vi hai luong do chay tren may chu hoac tien trinh tu dong, khong co ai de hoi.
    ///
    /// Chi co tac dung khi vien bat HIS.QD_130_BYT.XML1.CARD_INFO_WARNING = 1. Quy tac kiem o Base/CardInfoChecker.
    /// </summary>
    public partial class UCExportXml
    {
        #region Khai bao

        private const string CARD_INFO_WARNING_FIELD = "CARD_INFO_WARNING_STR";

        private GridColumn gridColCardInfoWarning;
        //Chi tao khi luoi chua co ToolTipController (cot XML 48h co the da tao truoc)
        private ToolTipController toolTipCardInfoWarning;
        //Canh bao tinh san cho cac dong dang nap tren luoi, theo ID ho so - khong tinh trong su kien ve o
        private readonly Dictionary<long, string> cardInfoWarningCodes = new Dictionary<long, string>();
        private readonly Dictionary<long, string> cardInfoWarningTexts = new Dictionary<long, string>();

        private bool IsCardInfoWarningEnabled
        {
            get { return HisConfigCFG.QD_130_BYT__XML1_CARD_INFO_WARNING == "1"; }
        }

        #endregion

        #region Hoi truoc khi xuat/gui

        /// <summary>
        /// Hoi truoc thao tac xuat/gui bang tay. Tra ve true = tiep tuc.
        /// - Nap lai ho so (api/HisTreatment/GetView1) theo lo de kiem dung du lieu luc bam: sua xong khong can Tim kiem lai.
        /// - XML 130: nap them dien dieu tri (api/HisPatientTypeAlter/GetView) de kiem moi the cua dot dieu tri nhu XML1 noi bang ';'.
        /// - Check-in: kiem the hien tai cua ho so (check-in lay the cuoi), khong nap dien dieu tri.
        /// Lo nao nap loi thi ho so cua lo do kiem theo dong luoi. Loi phia kiem tra khong chan xuat/gui.
        /// shortcut: check-in kiem SO_CCCD theo CCCD; check-in that lui sang ho chieu khi khong co CCCD - nguoi nuoc ngoai chi co
        /// ho chieu se bi canh bao thua. Nang cap khi vien bao.
        /// </summary>
        /// <param name="is3176">Che do XML 3176 cua luong xuat/gui (SO_CCCD lui sang CMND, ho chieu)</param>
        /// <param name="isCheckIn">Xuat lai file XML check-in</param>
        private async Task<bool> ConfirmCardInfoAsync(List<V_HIS_TREATMENT_1> treatments, bool is3176, bool isCheckIn)
        {
            try
            {
                if (!this.IsCardInfoWarningEnabled || treatments == null || treatments.Count == 0)
                {
                    return true;
                }

                List<V_HIS_TREATMENT_1> gridRows = treatments.Where(o => o != null).GroupBy(o => o.ID).Select(g => g.First()).ToList();
                List<long> treatmentIds = gridRows.Select(o => o.ID).ToList();
                Dictionary<long, V_HIS_TREATMENT_1> freshTreatments = new Dictionary<long, V_HIS_TREATMENT_1>();
                Dictionary<long, List<V_HIS_PATIENT_TYPE_ALTER>> patientTypeAlters = new Dictionary<long, List<V_HIS_PATIENT_TYPE_ALTER>>();

                WaitingManager.Show();
                try
                {
                    await Task.Run(() => LoadCardCheckData(treatmentIds, !isCheckIn, freshTreatments, patientTypeAlters));
                }
                finally
                {
                    WaitingManager.Hide();
                }

                List<TienGiamDinhResultADO> warned = new List<TienGiamDinhResultADO>();
                foreach (V_HIS_TREATMENT_1 gridRow in gridRows)
                {
                    V_HIS_TREATMENT_1 treatment;
                    if (!freshTreatments.TryGetValue(gridRow.ID, out treatment))
                    {
                        treatment = gridRow;
                    }

                    List<V_HIS_PATIENT_TYPE_ALTER> treatmentPatientTypeAlters;
                    List<V_HIS_PATIENT_TYPE_ALTER> cards = !isCheckIn && patientTypeAlters.TryGetValue(treatment.ID, out treatmentPatientTypeAlters)
                        ? CardInfoChecker.GetXml1Cards(treatmentPatientTypeAlters)
                        : CardInfoChecker.GetCurrentCard(treatment);
                    AddCardInfoResult(warned, treatment, CardInfoChecker.Check(treatment, cards, is3176 && !isCheckIn, HisConfigCFG.PATIENT_TYPE_ID__BHYT));
                }

                return ConfirmCardInfoWarnings(warned, gridRows.Count);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                //Loi phia kiem tra khong duoc lam tac viec xuat/gui
                LogSystem.Error(ex);
                return true;
            }
        }

        /// <summary>
        /// Nap lai ho so (va dien dieu tri khi can) theo lo vao 2 tu dien theo ID ho so.
        /// Lo nao loi thi bo qua - ho so cua lo do kiem theo dong luoi.
        /// </summary>
        private void LoadCardCheckData(List<long> treatmentIds, bool loadPatientTypeAlters, Dictionary<long, V_HIS_TREATMENT_1> freshTreatments, Dictionary<long, List<V_HIS_PATIENT_TYPE_ALTER>> patientTypeAlters)
        {
            try
            {
                int skip = 0;
                while (treatmentIds.Count - skip > 0)
                {
                    List<long> limit = treatmentIds.Skip(skip).Take(GlobalVariables.MAX_REQUEST_LENGTH_PARAM).ToList();
                    skip = skip + GlobalVariables.MAX_REQUEST_LENGTH_PARAM;

                    CommonParam paramTreatment = new CommonParam();
                    HisTreatmentView1Filter treatmentFilter = new HisTreatmentView1Filter();
                    treatmentFilter.IDs = limit;
                    List<V_HIS_TREATMENT_1> treatments = new Inventec.Common.Adapter.BackendAdapter(paramTreatment).Get<List<V_HIS_TREATMENT_1>>("api/HisTreatment/GetView1", ApiConsumers.MosConsumer, treatmentFilter, paramTreatment);
                    if (treatments == null)
                    {
                        LogSystem.Warn("CardInfoWarning - Khong nap lai duoc " + limit.Count + " ho so, kiem theo dong luoi.");
                    }
                    else
                    {
                        foreach (V_HIS_TREATMENT_1 treatment in treatments.Where(o => o != null))
                        {
                            freshTreatments[treatment.ID] = treatment;
                        }
                    }

                    if (!loadPatientTypeAlters)
                    {
                        continue;
                    }

                    CommonParam paramPatientTypeAlter = new CommonParam();
                    HisPatientTypeAlterViewFilter patientTypeAlterFilter = new HisPatientTypeAlterViewFilter();
                    patientTypeAlterFilter.TREATMENT_IDs = limit;
                    List<V_HIS_PATIENT_TYPE_ALTER> list = new Inventec.Common.Adapter.BackendAdapter(paramPatientTypeAlter).Get<List<V_HIS_PATIENT_TYPE_ALTER>>("api/HisPatientTypeAlter/GetView", ApiConsumers.MosConsumer, patientTypeAlterFilter, paramPatientTypeAlter);
                    if (list == null)
                    {
                        LogSystem.Warn("CardInfoWarning - Khong nap duoc dien dieu tri cua " + limit.Count + " ho so, kiem theo the tren dong ho so.");
                        continue;
                    }

                    foreach (IGrouping<long, V_HIS_PATIENT_TYPE_ALTER> group in list.GroupBy(o => o.TREATMENT_ID))
                    {
                        patientTypeAlters[group.Key] = group.ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private static void AddCardInfoResult(List<TienGiamDinhResultADO> warned, V_HIS_TREATMENT_1 treatment, List<TienGiamDinhErrorADO> warnings)
        {
            try
            {
                if (warnings == null || warnings.Count == 0)
                {
                    return;
                }

                TienGiamDinhResultADO result = new TienGiamDinhResultADO();
                result.TreatmentCode = treatment.TREATMENT_CODE;
                result.PatientName = treatment.TDL_PATIENT_NAME;
                result.Status = EnumTienGiamDinhStatus.Warning;
                result.StatusName = Resources.ResourceMessageLang.CardInfoTrangThai;
                result.Errors = warnings;
                warned.Add(result);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        /// <summary>Co ho so canh bao thi hoi: Co = tiep tuc; Khong = mo danh sach canh bao theo ho so roi dung</summary>
        private bool ConfirmCardInfoWarnings(List<TienGiamDinhResultADO> warned, int totalCount)
        {
            try
            {
                if (warned == null || warned.Count == 0)
                {
                    return true;
                }

                //Chi ghi ma dieu tri va ten truong - khong ghi so CCCD, so the vao log
                LogSystem.Info("CardInfoWarning - " + warned.Count + "/" + totalCount + " ho so thieu/sai thong tin the, ho so: "
                    + String.Join("; ", warned.Take(20).Select(o => o.TreatmentCode + ":" + String.Join(",", o.Errors.Select(p => p.Code).Distinct())))
                    + (warned.Count > 20 ? "; ..." : ""));

                if (XtraMessageBox.Show(String.Format(Resources.ResourceMessageLang.CardInfoHoiTiepTuc, warned.Count, totalCount),
                    Resources.ResourceMessageLang.ThongBao, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    LogSystem.Info("CardInfoWarning - Nguoi dung chon tiep tuc xuat/gui.");
                    return true;
                }

                LogSystem.Info("CardInfoWarning - Nguoi dung dung lai de sua ho so.");
                ShowCardInfoWarnings(warned, totalCount);
                return false;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                return true;
            }
        }

        /// <summary>Danh sach canh bao theo ho so - dung lai cua so ket qua tien giam dinh (co nut xuat Excel)</summary>
        private void ShowCardInfoWarnings(List<TienGiamDinhResultADO> warned, int totalCount)
        {
            try
            {
                //Nut xuat danh sach ra Excel can dang ky ban quyen Aspose truoc (nhu cua so tien giam dinh)
                try { SetLicenseForAsposeCell(); } catch (Exception exLicense) { LogSystem.Warn(exLicense); }

                using (frmTienGiamDinhResult form = new frmTienGiamDinhResult(warned,
                    Resources.ResourceMessageLang.CardInfoTieuDeCuaSo,
                    String.Format(Resources.ResourceMessageLang.CardInfoTongKet, totalCount, warned.Count),
                    Resources.ResourceMessageLang.CardInfoTenTepMacDinh))
                {
                    form.ShowDialog(this.ParentForm);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        #endregion

        #region Cot canh bao tren luoi

        /// <summary>Goi cuoi su kien Load. Vien khong bat cau hinh thi khong tao cot - man hinh giu nguyen.</summary>
        internal void InitCardInfoWarning()
        {
            try
            {
                if (!this.IsCardInfoWarningEnabled)
                {
                    return;
                }

                this.gridColCardInfoWarning = this.gridViewTreatment.Columns.AddField(CARD_INFO_WARNING_FIELD);
                this.gridColCardInfoWarning.UnboundType = DevExpress.Data.UnboundColumnType.String;
                this.gridColCardInfoWarning.Caption = Resources.ResourceMessageLang.CardInfoCotCanhBao;
                this.gridColCardInfoWarning.ToolTip = Resources.ResourceMessageLang.CardInfoTieuDeCuaSo;
                this.gridColCardInfoWarning.Name = "gridColCardInfoWarning";
                this.gridColCardInfoWarning.OptionsColumn.AllowEdit = false;
                this.gridColCardInfoWarning.Width = 120;
                this.gridColCardInfoWarning.Visible = true;
                //Xin dat sau cot XML 48h (neu co), khong thi sau hai cot Check-in. Hai cot Check-in la cot co dinh trai nen thuc te
                //cot moi (khong co dinh) dung ngay sau khoi cot co dinh - cot cuon dau tien, khong can keo ngang moi thay
                int afterIndex = this.gridColXml48h != null && this.gridColXml48h.VisibleIndex >= 0
                    ? this.gridColXml48h.VisibleIndex
                    : Math.Max(this.gridColumn2.VisibleIndex, this.gridColumn1.VisibleIndex);
                this.gridColCardInfoWarning.VisibleIndex = afterIndex >= 0 ? afterIndex + 1 : this.gridViewTreatment.VisibleColumns.Count;

                //Dang ky them - khong go bo trinh xu ly nao san co cua man hinh
                this.gridViewTreatment.CustomUnboundColumnData += GridViewTreatment_CardInfoWarningUnboundData;
                this.gridViewTreatment.RowCellStyle += GridViewTreatment_CardInfoWarningRowCellStyle;
                this.gridViewTreatment.DataSourceChanged += GridViewTreatment_CardInfoWarningDataSourceChanged;
                //SO_CCCD o che do XML 3176 lui sang CMND, ho chieu -> doi o XML 3176 thi tinh lai
                this.chkXML3176.CheckedChanged += ChkXML3176_CardInfoWarningCheckedChanged;

                if (this.gridControlTreatment.ToolTipController == null)
                {
                    this.toolTipCardInfoWarning = new ToolTipController();
                    this.gridControlTreatment.ToolTipController = this.toolTipCardInfoWarning;
                }
                this.gridControlTreatment.ToolTipController.GetActiveObjectInfo += GridControlTreatment_CardInfoWarningGetActiveObjectInfo;
                this.Disposed += UCExportXml_CardInfoWarningDisposed;

                //Luoi da nap trang dau truoc khi tao cot
                RefreshCardInfoWarnings();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void UCExportXml_CardInfoWarningDisposed(object sender, EventArgs e)
        {
            try
            {
                if (this.toolTipCardInfoWarning != null)
                {
                    this.toolTipCardInfoWarning.Dispose();
                    this.toolTipCardInfoWarning = null;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        /// <summary>Tinh canh bao cho cac dong dang nap tren luoi theo thong tin the tren dong (the hien tai cua ho so)</summary>
        private void RefreshCardInfoWarnings()
        {
            try
            {
                this.cardInfoWarningCodes.Clear();
                this.cardInfoWarningTexts.Clear();

                List<V_HIS_TREATMENT_1> rows = this.gridControlTreatment.DataSource as List<V_HIS_TREATMENT_1>;
                if (rows == null || rows.Count == 0)
                {
                    return;
                }

                bool is3176 = ReadXml3176CheckedSafe();
                foreach (V_HIS_TREATMENT_1 row in rows)
                {
                    if (row == null || this.cardInfoWarningCodes.ContainsKey(row.ID))
                    {
                        continue;
                    }

                    List<TienGiamDinhErrorADO> warnings = CardInfoChecker.Check(row, CardInfoChecker.GetCurrentCard(row), is3176, HisConfigCFG.PATIENT_TYPE_ID__BHYT);
                    if (warnings.Count == 0)
                    {
                        continue;
                    }

                    this.cardInfoWarningCodes[row.ID] = String.Join(", ", warnings.Select(o => o.Code).Distinct());
                    this.cardInfoWarningTexts[row.ID] = String.Join(Environment.NewLine, warnings.Select(o => "- " + o.Description));
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void GridViewTreatment_CardInfoWarningDataSourceChanged(object sender, EventArgs e)
        {
            try
            {
                RefreshCardInfoWarnings();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void ChkXML3176_CardInfoWarningCheckedChanged(object sender, EventArgs e)
        {
            try
            {
                RefreshCardInfoWarnings();
                this.gridViewTreatment.RefreshData();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void GridViewTreatment_CardInfoWarningUnboundData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.Column != this.gridColCardInfoWarning || !e.IsGetData)
                {
                    return;
                }

                string codes = "";
                List<V_HIS_TREATMENT_1> rows = this.gridControlTreatment.DataSource as List<V_HIS_TREATMENT_1>;
                if (rows != null && e.ListSourceRowIndex >= 0 && e.ListSourceRowIndex < rows.Count && rows[e.ListSourceRowIndex] != null)
                {
                    this.cardInfoWarningCodes.TryGetValue(rows[e.ListSourceRowIndex].ID, out codes);
                }
                e.Value = codes ?? "";
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void GridViewTreatment_CardInfoWarningRowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            try
            {
                if (e.Column != this.gridColCardInfoWarning)
                {
                    return;
                }

                if (!String.IsNullOrEmpty(e.CellValue as string))
                {
                    e.Appearance.ForeColor = Color.OrangeRed;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        /// <summary>Re chuot vao o canh bao: tung truong thieu/sai cua ho so</summary>
        private void GridControlTreatment_CardInfoWarningGetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {
                if (e.SelectedControl != this.gridControlTreatment)
                {
                    return;
                }

                GridHitInfo info = this.gridViewTreatment.CalcHitInfo(e.ControlMousePosition);
                if (info == null || !info.InRowCell || info.Column != this.gridColCardInfoWarning)
                {
                    return;
                }

                V_HIS_TREATMENT_1 row = this.gridViewTreatment.GetRow(info.RowHandle) as V_HIS_TREATMENT_1;
                string text;
                if (row == null || !this.cardInfoWarningTexts.TryGetValue(row.ID, out text))
                {
                    return;
                }

                e.Info = new ToolTipControlInfo(new CellToolTipInfo(info.RowHandle, info.Column, "cardInfoWarning"), text);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        #endregion
    }
}
