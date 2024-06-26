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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Inventec.Desktop.Common.Message;
using HTC.EFMODEL.DataModels;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using HTC.SDO;
using Inventec.Core;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Plugins.ImportRevenue.Base;
using HIS.Desktop.Controls.Session;
using System.IO;
using HIS.Desktop.Plugins.ImportRevenue.ADO;

namespace HIS.Desktop.Plugins.ImportRevenue
{
    public partial class UCImportRevenue : HIS.Desktop.Utility.UserControlBase
    {
        Inventec.Common.ExcelImport.Import import;

        List<HtcImportRevenueADO> ListRevenueImport = new List<HtcImportRevenueADO>();

        List<ImportRevenueSDO> ListResult = new List<ImportRevenueSDO>();

        public UCImportRevenue(Inventec.Desktop.Common.Modules.Module _moduleData)
            : base(_moduleData)
        {
            InitializeComponent();
            try
            {
                ResourceLangManager.InitResourceLanguageManager();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UCImportRevenue_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                RefreshControl();
                LoadKeyUCLanguage();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void RefreshControl()
        {
            try
            {
                ListRevenueImport = new List<HtcImportRevenueADO>();
                ListResult = new List<ImportRevenueSDO>();
                gridControlRevenue.DataSource = null;
                gridControlImportRevenueSdo.DataSource = null;
                btnChoiceFile.Enabled = true;
                btnImport.Enabled = false;
                btnRefresh.Enabled = true;
                btnExportExcel.Enabled = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewRevenue_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.ListSourceRowIndex >= 0 && e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var data = (HtcImportRevenueADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            try
                            {
                                e.Value = e.ListSourceRowIndex + 1;
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "IN_TIME_STR")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.IN_TIME ?? 0);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "OUT_TIME_STR")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.OUT_TIME ?? 0);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewImportRevenueSdo_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.ListSourceRowIndex >= 0 && e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var data = (ImportRevenueSDO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            try
                            {
                                e.Value = e.ListSourceRowIndex + 1;
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "DOB_STR")
                        {
                            try
                            {
                                e.Value = data.DOB.HasValue ? (data.DOB.Value.ToString().Substring(0, 4)) : "";
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "IN_TIME_STR")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.IN_TIME ?? 0);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "OUT_TIME_STR")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.OUT_TIME ?? 0);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewImportRevenueSdo_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    var data = (ImportRevenueSDO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (data != null)
                    {
                        if (data.IsError)
                        {
                            e.Appearance.BackColor = Color.Yellow;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnChoiceFile_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnChoiceFile.Enabled)
                    return;
                CommonParam param = new CommonParam();
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Multiselect = false;
                ofd.Filter = "Excel Files (*.xlsx)|*.xlsx";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    bool success = false;
                    WaitingManager.Show();
                    import = new Inventec.Common.ExcelImport.Import();
                    if (import.ReadFileExcel(ofd.FileName))
                    {
                        ListRevenueImport = import.GetPlus<HtcImportRevenueADO>();
                        if (ListRevenueImport != null && ListRevenueImport.Count > 0)
                        {
                            btnImport.Enabled = true;
                            btnChoiceFile.Enabled = false;
                            ProcessDataBeforeImport();
                            gridControlRevenue.BeginUpdate();
                            gridControlRevenue.DataSource = ListRevenueImport;
                            gridControlRevenue.EndUpdate();
                            success = true;
                        }
                    }
                    WaitingManager.Hide();
                    MessageManager.Show(param, success);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnImport.Enabled)
                    return;
                if (ListRevenueImport.Count > 0)
                {
                    CommonParam param = new CommonParam();
                    bool success = false;
                    WaitingManager.Show();
                    btnChoiceFile.Enabled = false;
                    btnImport.Enabled = false;
                    btnRefresh.Enabled = false;
                    var datas = ListRevenueImport.ToList<HTC_REVENUE>();
                    var listData = new Inventec.Common.Adapter.BackendAdapter(param).Post<List<ImportRevenueSDO>>(HtcRequestUriStore.HTC_REVENUE__CREATET, ApiConsumers.HtcConsumer, datas, param);
                    if (listData != null && listData.Count > 0)
                    {
                        success = true;
                        ListResult = listData;
                        gridControlImportRevenueSdo.BeginUpdate();
                        gridControlImportRevenueSdo.DataSource = listData;
                        gridControlImportRevenueSdo.EndUpdate();
                        btnRefresh.Enabled = true;
                        btnExportExcel.Enabled = true;
                    }
                    else
                    {
                        ListResult = new List<ImportRevenueSDO>();
                        btnImport.Enabled = true;
                        btnRefresh.Enabled = true;
                        btnExportExcel.Enabled = false;
                    }
                    WaitingManager.Hide();
                    MessageManager.Show(param, success);
                    SessionManager.ProcessTokenLost(param);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnRefresh.Enabled)
                    return;
                RefreshControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnExportExcel_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnExportExcel.Enabled || this.ListResult == null || this.ListResult.Count == 0)
                    return;
                var listData = ListResult.Where(o => o.IsError).ToList();
                if (listData == null || listData.Count == 0)
                    return;
                WaitingManager.Show();
                var listExport = (from r in listData select new HtcImportRevenueADO(r)).ToList();
                bool exportSuccess = true;
                Inventec.Common.FlexCellExport.Store store = new Inventec.Common.FlexCellExport.Store();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();
                exportSuccess = exportSuccess && store.ReadTemplate("PrintTemplate\\HTC_IMPORT_REVENUE_ERROR.xlsx");
                exportSuccess = exportSuccess && objectTag.AddObjectData(store, "Revenue", listExport);
                exportSuccess = exportSuccess && store.SetCommonFunctions();
                MemoryStream stream = store.OutStream();
                exportSuccess = exportSuccess && (stream != null && stream.Length > 0);
                if (exportSuccess)
                {
                    stream.Position = 0;
                    SaveFileDialog saveFile = new SaveFileDialog();
                    saveFile.Filter = "Excel (*.xlsx)|*.xlsx";
                    saveFile.FileName = "DuLieuDoanhThuImportLoi.xlsx";
                    WaitingManager.Hide();
                    if (saveFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        var fileStreeam = new FileStream(@"" + saveFile.FileName, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                        if (fileStreeam != null)
                        {
                            try
                            {
                                stream.CopyTo(fileStreeam);
                                DevExpress.XtraEditors.XtraMessageBox.Show("Lưu thành công");
                                if (System.Windows.Forms.MessageBox.Show("Bạn có muốn mở file bây giờ không?", "Hỏi đáp", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                                    System.Diagnostics.Process.Start(saveFile.FileName);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        fileStreeam.Dispose();
                    }
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessDataBeforeImport()
        {
            try
            {
                if (ListRevenueImport != null && ListRevenueImport.Count > 0)
                {
                    foreach (var item in ListRevenueImport)
                    {
                        if (item.AMOUNT > 0 && item.VIR_TOTAL_HEIN_PRICE.HasValue && item.VIR_TOTAL_HEIN_PRICE.Value > 0)
                        {
                            item.HEIN_PRICE = Math.Round((item.VIR_TOTAL_HEIN_PRICE.Value / item.AMOUNT), 4);
                            item.VIR_TOTAL_HEIN_PRICE = null;
                        }
                        if (item.VIR_TOTAL_HEIN_PRICE.HasValue)
                        {
                            item.VIR_TOTAL_HEIN_PRICE = null;
                        }
                        if (!String.IsNullOrEmpty(item.YEAR_STR) && item.YEAR_STR.Trim().Length == 4)
                        {
                            long year = 0;
                            if (long.TryParse(item.YEAR_STR.Trim(), out year))
                            {
                                item.DOB = Convert.ToInt64(year + "0101000000");
                            }
                        }

                        if (item.IN_DATE_TIME.HasValue && item.IN_DATE_TIME.Value != DateTime.MinValue)
                        {
                            item.IN_TIME = Convert.ToInt64(item.IN_DATE_TIME.Value.ToString("yyyyMMddHHmmss"));
                        }
                        if (item.OUT_DATE_TIME.HasValue && item.OUT_DATE_TIME.Value != DateTime.MinValue)
                        {
                            item.OUT_TIME = Convert.ToInt64(item.OUT_DATE_TIME.Value.ToString("yyyyMMddHHmmss"));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadKeyUCLanguage()
        {
            try
            {
                this.btnChoiceFile.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMPORT_REVENUE__BTN_CHOICE_FILE", Base.ResourceLangManager.LanguageUCImportRevenue, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.btnImport.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMPORT_REVENUE__BTN_IMPORT", Base.ResourceLangManager.LanguageUCImportRevenue, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.btnRefresh.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMPORT_REVENUE__BTN_REFRESH", Base.ResourceLangManager.LanguageUCImportRevenue, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                //gridControlRevenue
                this.gridColumn_Revenue_Amount.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMPORT_REVENUE__GRID_CONTROL__COLUMN_AMOUNT", Base.ResourceLangManager.LanguageUCImportRevenue, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Revenue_HeinPrice.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMPORT_REVENUE__GRID_CONTROL__COLUMN_HEIN_PRICE", Base.ResourceLangManager.LanguageUCImportRevenue, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Revenue_PatientTypeCode.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMPORT_REVENUE__GRID_CONTROL__COLUMN_PATIENT_TYPE_CODE", Base.ResourceLangManager.LanguageUCImportRevenue, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Revenue_Price.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMPORT_REVENUE__GRID_CONTROL__COLUMN_PRICE", Base.ResourceLangManager.LanguageUCImportRevenue, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Revenue_RequestDepartmentCode.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMPORT_REVENUE__GRID_CONTROL__COLUMN_REQUEST_DEPARTMENT_CODE", Base.ResourceLangManager.LanguageUCImportRevenue, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Revenue_RequestRoomCode.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMPORT_REVENUE__GRID_CONTROL__COLUMN_REQUEST_ROOM_CODE",
Base.ResourceLangManager.LanguageUCImportRevenue, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Revenue_ServiceCode.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMPORT_REVENUE__GRID_CONTROL__COLUMN_SERVICE_CODE", Base.ResourceLangManager.LanguageUCImportRevenue, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Revenue_ServiceTypeCode.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMPORT_REVENUE__GRID_CONTROL__COLUMN_SERVICE_TYPE_CODE", Base.ResourceLangManager.LanguageUCImportRevenue, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                //gridControlImportSdo
                this.gridColumn_Sdo_Amount.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMPORT_REVENUE__GRID_CONTROL__COLUMN_AMOUNT", Base.ResourceLangManager.LanguageUCImportRevenue, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Sdo_Description.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMPORT_REVENUE__GRID_CONTROL__COLUMN_DESCRIPTION", Base.ResourceLangManager.LanguageUCImportRevenue, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Sdo_ExecuteDepartmentName.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMPORT_REVENUE__GRID_CONTROL__COLUMN_EXECUTE_DEPARTMENT_NAME", Base.ResourceLangManager.LanguageUCImportRevenue, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Sdo_HeinPrice.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMPORT_REVENUE__GRID_CONTROL__COLUMN_HEIN_PRICE", Base.ResourceLangManager.LanguageUCImportRevenue, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Sdo_PatientTypeCode.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMPORT_REVENUE__GRID_CONTROL__COLUMN_PATIENT_TYPE_CODE", Base.ResourceLangManager.LanguageUCImportRevenue, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Sdo_PatientTypeName.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMPORT_REVENUE__GRID_CONTROL__COLUMN_PATIENT_TYPE_NAME", Base.ResourceLangManager.LanguageUCImportRevenue, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Sdo_Price.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMPORT_REVENUE__GRID_CONTROL__COLUMN_PRICE", Base.ResourceLangManager.LanguageUCImportRevenue, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Sdo_RequestDepartmentName.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMPORT_REVENUE__GRID_CONTROL__COLUMN_REQUEST_DEPARTMENT_NAME", Base.ResourceLangManager.LanguageUCImportRevenue, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Sdo_RevenueCode.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMPORT_REVENUE__GRID_CONTROL__COLUMN_REVENUE_CODE", Base.ResourceLangManager.LanguageUCImportRevenue, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Sdo_ServiceCode.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMPORT_REVENUE__GRID_CONTROL__COLUMN_SERVICE_CODE", Base.ResourceLangManager.LanguageUCImportRevenue, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Sdo_ServiceName.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMPORT_REVENUE__GRID_CONTROL__COLUMN_SERVICE_NAME", Base.ResourceLangManager.LanguageUCImportRevenue, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Sdo_ServiceTypeCode.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMPORT_REVENUE__GRID_CONTROL__COLUMN_SERVICE_TYPE_CODE", Base.ResourceLangManager.LanguageUCImportRevenue, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void BtnChoiceFile()
        {
            try
            {
                btnChoiceFile_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void BtnImport()
        {
            try
            {
                btnImport_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void BtnRefresh()
        {
            try
            {
                btnRefresh_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
