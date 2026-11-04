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
    /// - Hoi xac nhan truoc cac thao tac xuat/gui XML 130 bang tay: kiem moi the cua dot dieu tri (nhu XML1 noi the bang ';').
    ///   Co = xuat/gui nhu cu; Khong = mo danh sach canh bao theo ho so roi dung. Chi canh bao, khong chan.
    /// - Hoi truoc menu "Xuat lai file XML check-in server": kiem theo the tren dong ho so (check-in lay the cuoi).
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
        /// Hoi truoc khi xuat/gui XML 130 bang tay. Tra ve true = tiep tuc.
        /// Kiem moi the cua dot dieu tri (nap qua api/HisPatientTypeAlter/GetView theo lo) dung nhu XML1;
        /// lo nao nap loi thi ho so cua lo do kiem theo the tren dong. Loi phia kiem tra khong chan xuat/gui.
        /// </summary>
        /// <param name="is3176">Che do XML 3176 cua luong xuat/gui (SO_CCCD lui sang CMND, ho chieu)</param>
        private async Task<bool> ConfirmCardInfoBeforeXmlAsync(List<V_HIS_TREATMENT_1> treatments, bool is3176)
        {
            try
            {
                if (!this.IsCardInfoWarningEnabled || treatments == null || treatments.Count == 0)
                {
                    return true;
                }

                List<V_HIS_TREATMENT_1> distinctTreatments = treatments.Where(o => o != null).GroupBy(o => o.ID).Select(g => g.First()).ToList();
                List<long> treatmentIds = distinctTreatments.Select(o => o.ID).ToList();

                WaitingManager.Show();
                Dictionary<long, List<V_HIS_PATIENT_TYPE_ALTER>> patientTypeAlters;
                try
                {
                    patientTypeAlters = await Task.Run(() => LoadPatientTypeAltersForCardCheck(treatmentIds));
                }
                finally
                {
                    WaitingManager.Hide();
                }

                List<TienGiamDinhResultADO> warned = new List<TienGiamDinhResultADO>();
                foreach (V_HIS_TREATMENT_1 treatment in distinctTreatments)
                {
                    List<V_HIS_PATIENT_TYPE_ALTER> treatmentPatientTypeAlters;
                    List<V_HIS_PATIENT_TYPE_ALTER> cards = patientTypeAlters.TryGetValue(treatment.ID, out treatmentPatientTypeAlters)
                        ? CardInfoChecker.GetXml1Cards(treatmentPatientTypeAlters)
                        : CardInfoChecker.GetCurrentCard(treatment);
                    AddCardInfoResult(warned, treatment, CardInfoChecker.Check(treatment, cards, is3176, HisConfigCFG.PATIENT_TYPE_ID__BHYT));
                }

                return ConfirmCardInfoWarnings(warned, distinctTreatments.Count);
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
        /// Hoi truoc khi xuat lai file check-in. Check-in lay the cuoi cua ho so nen kiem theo the tren dong ho so (khong goi API).
        /// shortcut: SO_CCCD kiem theo CCCD; check-in that lui sang ho chieu khi khong co CCCD - nguoi nuoc ngoai chi co ho chieu
        /// se bi canh bao thua. Nang cap khi vien bao.
        /// </summary>
        private bool ConfirmCardInfoBeforeCheckIn(List<V_HIS_TREATMENT_1> treatments)
        {
            try
            {
                if (!this.IsCardInfoWarningEnabled || treatments == null || treatments.Count == 0)
                {
                    return true;
                }

                List<V_HIS_TREATMENT_1> distinctTreatments = treatments.Where(o => o != null).GroupBy(o => o.ID).Select(g => g.First()).ToList();
                List<TienGiamDinhResultADO> warned = new List<TienGiamDinhResultADO>();
                foreach (V_HIS_TREATMENT_1 treatment in distinctTreatments)
                {
                    AddCardInfoResult(warned, treatment, CardInfoChecker.Check(treatment, CardInfoChecker.GetCurrentCard(treatment), false, HisConfigCFG.PATIENT_TYPE_ID__BHYT));
                }

                return ConfirmCardInfoWarnings(warned, distinctTreatments.Count);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                return true;
            }
        }

        /// <summary>Nap dien dieu tri cua cac ho so theo lo, gom theo ID ho so. Lo loi thi bo qua (ho so cua lo do kiem theo the tren dong).</summary>
        private Dictionary<long, List<V_HIS_PATIENT_TYPE_ALTER>> LoadPatientTypeAltersForCardCheck(List<long> treatmentIds)
        {
            Dictionary<long, List<V_HIS_PATIENT_TYPE_ALTER>> result = new Dictionary<long, List<V_HIS_PATIENT_TYPE_ALTER>>();
            try
            {
                int skip = 0;
                while (treatmentIds.Count - skip > 0)
                {
                    List<long> limit = treatmentIds.Skip(skip).Take(GlobalVariables.MAX_REQUEST_LENGTH_PARAM).ToList();
                    skip = skip + GlobalVariables.MAX_REQUEST_LENGTH_PARAM;

                    CommonParam param = new CommonParam();
                    HisPatientTypeAlterViewFilter filter = new HisPatientTypeAlterViewFilter();
                    filter.TREATMENT_IDs = limit;
                    List<V_HIS_PATIENT_TYPE_ALTER> patientTypeAlters = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_PATIENT_TYPE_ALTER>>("api/HisPatientTypeAlter/GetView", ApiConsumers.MosConsumer, filter, param);
                    if (patientTypeAlters == null)
                    {
                        LogSystem.Warn("CardInfoWarning - Khong nap duoc dien dieu tri cua " + limit.Count + " ho so, kiem theo the tren dong ho so.");
                        continue;
                    }

                    foreach (IGrouping<long, V_HIS_PATIENT_TYPE_ALTER> group in patientTypeAlters.GroupBy(o => o.TREATMENT_ID))
                    {
                        result[group.Key] = group.ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            return result;
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
                //Dat ngay sau cot XML 48h (neu co), khong thi sau hai cot Check-in
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
