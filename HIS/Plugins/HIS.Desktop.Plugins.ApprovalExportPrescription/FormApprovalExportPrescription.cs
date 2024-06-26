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
using DevExpress.Data;
using DevExpress.Utils.Menu;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Print;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ApprovalExportPrescription
{
    public partial class FormApprovalExportPrescription : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        Inventec.Desktop.Common.Modules.Module moduleData = null;
        System.Globalization.CultureInfo cultureLang;
        string congstr = "";
        CPA.WCFClient.CallPatientClient.CallPatientClientManager clienttManager = null;

        List<long> ListExpMestType = new List<long>()
        {
            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DDT,
            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DPK,
            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DTT,
            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__THPK
        };
        List<long> ListExpMestStt = new List<long>() {
            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DRAFT,
            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST
        };

        List<ADO.PrescriptionADO> listDataGrid;
        List<HIS_TREATMENT> histreatment; 
        ADO.ThreadDataADO DataThread;

        long MediStockId = 0;

        bool IsNotAutoApproval = false;
        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        string moduleLink = "HIS.Desktop.Plugins.ApprovalExportPrescription";
        bool isNotLoadWhileChangeControlStateInFirst;
        #endregion

        #region Construct
        public FormApprovalExportPrescription()
            : this(null)
        {

        }

        public FormApprovalExportPrescription(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            try
            {
                InitializeComponent();
                try
                {
                    Resources.ResourceLanguageManager.InitResourceLanguageManager();
                    cultureLang = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
                    congstr = HisConfigs.Get<string>(Config.CALL_PATIENT_CONG);
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
                this.moduleData = module;
                this.Text = module.text;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Private method
        #region load
        private void FormApprovalExportPrescription_Load(object sender, EventArgs e)
        {
            try
            {
                SetIcon();

                LoadKeysFromlanguage();

                SetDefaultValueControl();

                InitMenuToButtonPrint();

                this.InitControlState();

                var mediStock = BackendDataWorker.Get<HIS_MEDI_STOCK>().FirstOrDefault(o => o.ROOM_ID == moduleData.RoomId);
                if (mediStock != null)
                {
                    this.MediStockId = mediStock.ID;
                }

                var autoApproval = HisConfigs.Get<string>(Config.AutoApproval) ?? "0";
                IsNotAutoApproval = autoApproval.Trim() == "1";

                BtnApproval.Enabled = IsNotAutoApproval;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadKeysFromlanguage()
        {
            try
            {
                //layout
                this.btnCall.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_APPROVAL_EXPORT_PRESCRIPTION__BTN_CALL",
                    Resources.ResourceLanguageManager.LanguageFormApprovalExportPrescription,
                    cultureLang);
                this.Gc_Amount.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_APPROVAL_EXPORT_PRESCRIPTION__GC_AMOUNT",
                    Resources.ResourceLanguageManager.LanguageFormApprovalExportPrescription,
                    cultureLang);
                this.Gc_Discount.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_APPROVAL_EXPORT_PRESCRIPTION__GC_DISCOUNT",
                    Resources.ResourceLanguageManager.LanguageFormApprovalExportPrescription,
                    cultureLang);
                this.Gc_ExpVatRatio.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_APPROVAL_EXPORT_PRESCRIPTION__GC_EXP_VAT_RATIO",
                    Resources.ResourceLanguageManager.LanguageFormApprovalExportPrescription,
                    cultureLang);
                this.Gc_ImpPrice.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_APPROVAL_EXPORT_PRESCRIPTION__GC_IMP_PRICE",
                    Resources.ResourceLanguageManager.LanguageFormApprovalExportPrescription,
                    cultureLang);
                this.Gc_ImpVatRatio.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_APPROVAL_EXPORT_PRESCRIPTION__GC_IMP_VAT_RATIO",
                    Resources.ResourceLanguageManager.LanguageFormApprovalExportPrescription,
                    cultureLang);
                this.Gc_DetailCode.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_APPROVAL_EXPORT_PRESCRIPTION__GC_DETAIL_CODE",
                    Resources.ResourceLanguageManager.LanguageFormApprovalExportPrescription,
                    cultureLang);
                this.Gc_DetailName.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_APPROVAL_EXPORT_PRESCRIPTION__GC_DETAIL_NAME",
                    Resources.ResourceLanguageManager.LanguageFormApprovalExportPrescription,
                    cultureLang);
                this.Gc_Price.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_APPROVAL_EXPORT_PRESCRIPTION__GC_PRICE",
                    Resources.ResourceLanguageManager.LanguageFormApprovalExportPrescription,
                    cultureLang);
                this.Gc_ServiceUnitName.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_APPROVAL_EXPORT_PRESCRIPTION__GC_SERVICE_UNIT_NAME",
                    Resources.ResourceLanguageManager.LanguageFormApprovalExportPrescription,
                    cultureLang);
                this.Gc_STT.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_APPROVAL_EXPORT_PRESCRIPTION__GC_STT",
                    Resources.ResourceLanguageManager.LanguageFormApprovalExportPrescription,
                    cultureLang);
                //this.Gc_SumInStock.Caption = Inventec.Common.Resource.Get.Value(
                //    "IVT_LANGUAGE_KEY__FORM_APPROVAL_EXPORT_PRESCRIPTION__GC_SUM_IN_STOCK",
                //    Resources.ResourceLanguageManager.LanguageFormApprovalExportPrescription,
                //    cultureLang);
                this.Gc_TotalPrice.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_APPROVAL_EXPORT_PRESCRIPTION__GC_TOTAL_PRICE",
                    Resources.ResourceLanguageManager.LanguageFormApprovalExportPrescription,
                    cultureLang);
                this.groupControlInfo.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_APPROVAL_EXPORT_PRESCRIPTION__GROUP_CONTROL_INFO",
                    Resources.ResourceLanguageManager.LanguageFormApprovalExportPrescription,
                    cultureLang);
                this.lciDob.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_APPROVAL_EXPORT_PRESCRIPTION__LCI_DOB",
                    Resources.ResourceLanguageManager.LanguageFormApprovalExportPrescription,
                    cultureLang);
                this.lciGender.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_APPROVAL_EXPORT_PRESCRIPTION__LCI_GENDER",
                    Resources.ResourceLanguageManager.LanguageFormApprovalExportPrescription,
                    cultureLang);
                this.lciPatientCode.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_APPROVAL_EXPORT_PRESCRIPTION__LCI_PATIENT_CODE",
                    Resources.ResourceLanguageManager.LanguageFormApprovalExportPrescription,
                    cultureLang);
                this.lciTotalPrice.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_APPROVAL_EXPORT_PRESCRIPTION__LCI_TOTAL_PRICE",
                    Resources.ResourceLanguageManager.LanguageFormApprovalExportPrescription,
                    cultureLang);
                this.lciUseTime.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_APPROVAL_EXPORT_PRESCRIPTION__LCI_USE_TIME",
                    Resources.ResourceLanguageManager.LanguageFormApprovalExportPrescription,
                    cultureLang);
                this.lciVirAddress.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_APPROVAL_EXPORT_PRESCRIPTION__LCI_VIR_ADDRESS",
                    Resources.ResourceLanguageManager.LanguageFormApprovalExportPrescription,
                    cultureLang);
                this.lciVirPatientName.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_APPROVAL_EXPORT_PRESCRIPTION__LCI_VIR_PATIENT_NAME",
                    Resources.ResourceLanguageManager.LanguageFormApprovalExportPrescription,
                    cultureLang);
                this.txtCong.Properties.NullValuePrompt = congstr;
                this.txtExpMestCode.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_APPROVAL_EXPORT_PRESCRIPTION__TXT_EXP_MEST_CODE__NULL_VALUE",
                    Resources.ResourceLanguageManager.LanguageFormApprovalExportPrescription,
                    cultureLang);
                this.TxtTreatmentCode.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_APPROVAL_EXPORT_PRESCRIPTION__TXT_TREATMENT_CODE__NULL_VALUE",
                    Resources.ResourceLanguageManager.LanguageFormApprovalExportPrescription,
                    cultureLang);
                this.txtTDLServiceReqCode.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_APPROVAL_EXPORT_PRESCRIPTION__TXT_TDL_SERVICE_REQ_CODE__NULL_VALUE",
                    Resources.ResourceLanguageManager.LanguageFormApprovalExportPrescription,
                    cultureLang);

                this.BtnApproval.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_APPROVAL_EXPORT_PRESCRIPTION__BTN_APPROVAL",
                    Resources.ResourceLanguageManager.LanguageFormApprovalExportPrescription,
                    cultureLang);
                this.btnExported.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_APPROVAL_EXPORT_PRESCRIPTION__BTN_EXPORT",
                    Resources.ResourceLanguageManager.LanguageFormApprovalExportPrescription,
                    cultureLang);
                this.lciIcdName.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_APPROVAL_EXPORT_PRESCRIPTION__LCI_NAME",
                    Resources.ResourceLanguageManager.LanguageFormApprovalExportPrescription,
                    cultureLang);
                this.lciIcdText.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_APPROVAL_EXPORT_PRESCRIPTION__LCI_TEXT",
                    Resources.ResourceLanguageManager.LanguageFormApprovalExportPrescription,
                    cultureLang);
                this.Gc_Tutorial.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_APPROVAL_EXPORT_PRESCRIPTION__GC_TUTORIAL",
                    Resources.ResourceLanguageManager.LanguageFormApprovalExportPrescription,
                    cultureLang);
                this.Gc_Tutorial.ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_APPROVAL_EXPORT_PRESCRIPTION__GC_TUTORIAL_TOOLTIP",
                    Resources.ResourceLanguageManager.LanguageFormApprovalExportPrescription,
                    cultureLang);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultValueControl()
        {
            try
            {
                this.lblDob.Text = "";
                this.lblGender.Text = "";
                this.lblPatientCode.Text = "";
                this.lblTotalPrice.Text = "";
                this.lblVirAddress.Text = "";
                this.lblVirPatientName.Text = "";
                this.lblUseTime.Text = "";
                this.lblIcdText.Text = "";
                this.lblIcdName.Text = "";
                DataThread = null;

                gridControl1.BeginUpdate();
                gridControl1.DataSource = null;
                gridControl1.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitMenuToButtonPrint()
        {
            try
            {
                WaitingManager.Show();
                DXPopupMenu menu = new DXPopupMenu();

                DXMenuItem itemInHuongDanSuDung = new DXMenuItem("Hướng dẫn sử dụng", new EventHandler(OnClickInThucXuatThuoc));
                itemInHuongDanSuDung.Tag = "HUONG_DAN_SU_DUNG";
                menu.Items.Add(itemInHuongDanSuDung);

                DXMenuItem itemDonThuocVatTu = new DXMenuItem("Đơn thuốc / vật tư", new EventHandler(OnClickInThucXuatThuoc));
                itemDonThuocVatTu.Tag = "DON_THUOC_VAT_TU";
                menu.Items.Add(itemDonThuocVatTu);

                cboPrint.DropDownControl = menu;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void OnClickInThucXuatThuoc(object sender, EventArgs e)
        {
            var bbtnItem = sender as DXMenuItem;
            if (bbtnItem.Tag == "HUONG_DAN_SU_DUNG")
            {
                ProcessPrintHDSD();
            }
            if (bbtnItem.Tag == "DON_THUOC_VAT_TU")
            {
                ProcessPrint(false);
            }
        }

        #endregion

        #region event

        private void txtTDLServiceReqCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrEmpty(txtTDLServiceReqCode.Text))
                    {
                        txtExpMestCode.Text = "";
                        TxtTreatmentCode.Text = "";
                        var TDLServiceReqCode = txtTDLServiceReqCode.Text.Trim();
                        if (TDLServiceReqCode.Length < 12 && checkDigit(TDLServiceReqCode))
                        {
                            TDLServiceReqCode = string.Format("{0:000000000000}", Convert.ToInt64(TDLServiceReqCode));
                            txtTDLServiceReqCode.Text = TDLServiceReqCode;
                        }
                        LoadDataExpMest();
                        txtTDLServiceReqCode.SelectAll();
                    }
                    else
                    {
                        txtCong.Focus();
                        txtCong.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtTDLServiceReqCode_KeyDown(object sender, KeyEventArgs e)
        {
            //try
            //{
            //    if (e.KeyCode == Keys.Enter)
            //    {
            //        if (!String.IsNullOrEmpty(txtTDLServiceReqCode.Text))
            //        {
            //            txtTDLServiceReqCode.Text = "";
            //            var TDLServiceReqCode = txtTDLServiceReqCode.Text.Trim();
            //            if (TDLServiceReqCode.Length < 12 && checkDigit(TDLServiceReqCode))
            //            {
            //                TDLServiceReqCode = string.Format("{0:000000000000}", Convert.ToInt64(TDLServiceReqCode));
            //                txtTDLServiceReqCode.Text = TDLServiceReqCode;
            //            }
            //            LoadDataExpMest();
            //            txtTDLServiceReqCode.SelectAll();
            //        }
            //        else
            //        {
            //            txtCong.Focus();
            //            txtCong.SelectAll();
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Error(ex);
            //}

        }

        private void TxtTreatmentCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrEmpty(TxtTreatmentCode.Text))
                    {
                        txtExpMestCode.Text = "";
                        txtTDLServiceReqCode.Text = "";
                        var treatmentCode = TxtTreatmentCode.Text.Trim();
                        if (treatmentCode.Length < 12 && checkDigit(treatmentCode))
                        {
                            treatmentCode = string.Format("{0:000000000000}", Convert.ToInt64(treatmentCode));
                            TxtTreatmentCode.Text = treatmentCode;
                        }
                        LoadDataExpMest();
                        TxtTreatmentCode.SelectAll();
                    }
                    else
                    {
                        txtExpMestCode.Focus();
                        txtExpMestCode.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtExpMestCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrEmpty(txtExpMestCode.Text))
                    {
                        txtTDLServiceReqCode.Text = "";
                        TxtTreatmentCode.Text = "";
                        var expMestCode = txtExpMestCode.Text.Trim();
                        if (expMestCode.Length < 12 && checkDigit(expMestCode))
                        {
                            expMestCode = string.Format("{0:000000000000}", Convert.ToInt64(expMestCode));
                            txtExpMestCode.Text = expMestCode;
                        }
                        LoadDataExpMest();
                        txtExpMestCode.SelectAll();
                    }
                    else
                    {
                        txtTDLServiceReqCode.Focus();
                        txtTDLServiceReqCode.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtCong_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (Char.IsDigit(e.KeyChar) || Char.IsControl(e.KeyChar))
                {
                    e.Handled = false;
                }
                else
                    e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnCall_Click(object sender, EventArgs e)
        {
            try
            {
                if (DataThread != null && DataThread.ListHisExpMest != null && DataThread.ListHisExpMest.Count > 0)
                    CallPatient(DataThread.ListHisExpMest.FirstOrDefault());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnExported_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnExported.Enabled) return;

                if (ProcessExport())
                {
                    if (chkInDon.Checked)
                    {
                        ProcessPrint(true);
                    }
                    if (chkInHDSD.Checked)
                    {
                        ProcessPrintHDSD();
                    }
                    SetDefaultValueControl();
                    //ProcessUncheck();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    ADO.PrescriptionADO dataRow = (ADO.PrescriptionADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (dataRow != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        else if (e.Column.FieldName == "EXP_VAT_RATIO_DISPLAY")
                        {
                            try
                            {
                                e.Value = Inventec.Common.Number.Convert.NumberToString((dataRow.VAT_RATIO ?? 0) * 100, ConfigApplications.NumberSeperator);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho truong VAT_RATIO", ex);
                            }
                        }
                        else if (e.Column.FieldName == "IMP_VAT_RATIO_DISPLAY")
                        {
                            try
                            {
                                e.Value = Inventec.Common.Number.Convert.NumberToString(dataRow.IMP_VAT_RATIO * 100, ConfigApplications.NumberSeperator);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho truong IMP_VAT_RATIO", ex);
                            }
                        }
                        else if (e.Column.FieldName == "EXP_MEST_ID_STR")
                        {
                            try
                            {
                                e.Value = string.Format("{0} - Mã y lệnh: {1} - Ngày y lệnh: {2} - Phòng khám: {3} - Người kê đơn: {4}({5})", dataRow.EXP_MEST_CODE, dataRow.SERVICE_REQ_CODE, dataRow.INTRUCTION_TIME_STR, dataRow.REQUEST_ROOM_NAME, dataRow.REQUEST_USERNAME, dataRow.REQUEST_LOGINNAME);
                            }
                            catch (Exception ex)
                            {
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

        private void gridView_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    var kind = Inventec.Common.TypeConvert.Parse.ToInt16((view.GetRowCellValue(e.RowHandle, "type") ?? "").ToString());
                    if (kind == 2)
                    {
                        e.Appearance.ForeColor = System.Drawing.Color.Blue;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemCheck_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                var row = (ADO.PrescriptionADO)gridView.GetFocusedRow();
                if (row != null && listDataGrid != null)
                {
                    var checkedit = (DevExpress.XtraEditors.CheckEdit)sender;
                    foreach (var item in listDataGrid)
                    {
                        if (item.EXP_MEST_ID == row.EXP_MEST_ID)
                        {
                            item.IsCheck = checkedit.Checked;
                        }
                    }

                    gridControl1.RefreshDataSource();
                    ProcessTotalPrice();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BtnApproval_Click(object sender, EventArgs e)
        {
            try
            {
                if (!BtnApproval.Enabled) return;

                var keyWarningModifiedPrescriptionOption = HisConfigs.Get<string>(Config.WarningModifiedPrescriptionOption) ?? "0";
                bool isWarningModifiedPrescriptionOption = keyWarningModifiedPrescriptionOption.Trim() == "1";

                var listDataExport = new List<V_HIS_EXP_MEST>();
                if (listDataGrid != null && listDataGrid.Count > 0)
                {
                    var expMestIds = listDataGrid.Where(s => s.IsCheck).ToList();
                    if (expMestIds != null && expMestIds.Count > 0)
                    {
                        var listExecute = DataThread.ListHisExpMest.Where(o => expMestIds.Select(s => s.EXP_MEST_ID).Contains(o.ID)).ToList();
                        if (listExecute != null && listExecute.Count > 0)
                        {
                            listDataExport.AddRange(listExecute);

                            var aggr = listExecute.Select(s => s.AGGR_EXP_MEST_ID ?? 0).Distinct().ToList();// luôn có id = 0
                            var lstAggr = DataThread.ListHisExpMest.Where(o => aggr.Contains(o.ID)).ToList();
                            if (lstAggr != null && lstAggr.Count > 0) listDataExport.AddRange(lstAggr);
                        }
                    }
                }

                if (listDataExport != null && listDataExport.Count > 0)
                {
                    bool isApproval = true;
                    foreach (var item in listDataExport)
                    {
                        var createTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(item.CREATE_TIME ?? 0);
                        var modifyTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(item.MODIFY_TIME ?? 0);
                        TimeSpan oneMinute = new System.TimeSpan(0, 0, 1, 0);

                        if (isWarningModifiedPrescriptionOption && item.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DPK && (modifyTime - createTime) > oneMinute)
                        {
                            string messageStr = string.Format("Phiếu xuất {0} đã có sự chỉnh sửa. Bạn có chắc muốn duyệt không? ", item.EXP_MEST_CODE);
                            if (MessageBox.Show(messageStr, "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                            {
                                isApproval = true;
                                break;
                            }
                            else
                            {
                                isApproval = false;
                                break;
                            }
                        }
                    }

                    if (isApproval)
                    {
                        bool success = true;
                        CommonParam param = new CommonParam();
                        param.Messages = new List<string>();
                        param.BugCodes = new List<string>();

                        foreach (var item in listDataExport)
                        {
                            bool resultExport = false;
                            CommonParam paramrResult = new CommonParam();
                            ApprovalPrescription(item, ref resultExport, ref paramrResult);

                            success = success & resultExport;
                            if (paramrResult.Messages != null && paramrResult.Messages.Count > 0)
                            {
                                param.Messages.AddRange(paramrResult.Messages);
                            }
                            if (paramrResult.BugCodes != null && paramrResult.BugCodes.Count > 0)
                            {
                                param.BugCodes.AddRange(paramrResult.BugCodes);
                            }
                        }

                        if (param.Messages != null && param.Messages.Count > 0)
                        {
                            param.Messages = param.Messages.Distinct().ToList();
                        }

                        if (param.BugCodes != null && param.BugCodes.Count > 0)
                        {
                            param.BugCodes = param.BugCodes.Distinct().ToList();
                        }

                        #region Show message
                        Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                        #endregion

                        #region Process has exception
                        HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                        #endregion
                    }
                }
                else
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(Resources.ResourceMessage.VuiLongChonPhieuXuat);
                    Inventec.Common.Logging.LogSystem.Error("listDataExport null");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region lưu trạng thái
        private void InitControlState()
        {
            try
            {
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(moduleLink);
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == chkInDon.Name)
                        {
                            chkInDon.Checked = item.VALUE == "1";
                        }
                        if (item.KEY == chkInHDSD.Name)
                        {
                            chkInHDSD.Checked = item.VALUE == "1";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkInDon_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkInDon.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkInDon.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkInDon.Name;
                    csAddOrUpdate.VALUE = (chkInDon.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = moduleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkInHDSD_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkInHDSD.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkInHDSD.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkInHDSD.Name;
                    csAddOrUpdate.VALUE = (chkInHDSD.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = moduleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region action
        private void LoadDataExpMest()
        {
            try
            {
                //    if (String.IsNullOrEmpty(TxtTreatmentCode.Text) && String.IsNullOrEmpty(txtExpMestCode.Text))
                //    {
                //        DevExpress.XtraEditors.XtraMessageBox.Show(Resources.ResourceMessage.VuiLongNhapMaXuat);
                //        return;
                //    }
                //load data;

                CommonParam param = new CommonParam();
                MOS.Filter.HisExpMestViewFilter filter = new MOS.Filter.HisExpMestViewFilter();
                if (!String.IsNullOrEmpty(TxtTreatmentCode.Text))
                {
                    filter.TDL_TREATMENT_CODE__EXACT = TxtTreatmentCode.Text;
                }
                else if (!String.IsNullOrEmpty(txtExpMestCode.Text))
                {
                    filter.EXP_MEST_CODE__EXACT = txtExpMestCode.Text;
                    filter.EXP_MEST_TYPE_IDs = ListExpMestType;
                }
                else if (!string.IsNullOrEmpty(txtTDLServiceReqCode.Text))
                {
                    filter.TDL_SERVICE_REQ_CODE__EXACT = txtTDLServiceReqCode.Text;
                }

                filter.IS_NOT_TAKEN = false;
                filter.EXP_MEST_STT_IDs = ListExpMestStt;
                if (this.MediStockId > 0)
                {
                    filter.MEDI_STOCK_ID = this.MediStockId;
                }
                var result = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST>>("api/HisExpMest/GetView", ApiConsumer.ApiConsumers.MosConsumer, filter, param);

                if (result != null && result.Count > 0)
                {
                    FillDataToControl(result);
                }
                else
                {
                    SetDefaultValueControl();
                    if (!String.IsNullOrWhiteSpace(TxtTreatmentCode.Text))
                        DevExpress.XtraEditors.XtraMessageBox.Show(string.Format(Resources.ResourceMessage.MaDieuTriKhongHopLe, TxtTreatmentCode.Text));
                    else if (!String.IsNullOrWhiteSpace(txtExpMestCode.Text))
                        DevExpress.XtraEditors.XtraMessageBox.Show(string.Format(Resources.ResourceMessage.MaXuatKhongHopLe, txtExpMestCode.Text));
                    else if (!String.IsNullOrWhiteSpace(txtTDLServiceReqCode.Text))
                        DevExpress.XtraEditors.XtraMessageBox.Show(string.Format(Resources.ResourceMessage.MaYLenhKhongHopLe, txtTDLServiceReqCode.Text));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool checkDigit(string s)
        {
            bool result = false;
            try
            {
                for (int i = 0; i < s.Length; i++)
                {
                    if (char.IsDigit(s[i]) == true) result = true;
                    else result = false;
                }
                return result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return result;
            }
        }

        private void FillDataToControl(List<V_HIS_EXP_MEST> expMestData)
        {
            try
            {
                if (expMestData != null && expMestData.Count > 0)
                {
                    WaitingManager.Show();
                    //th tìm theo mã xuất sẽ có 1
                    if (expMestData.Count == 1 && expMestData.First().EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__THPK)
                    {
                        MOS.Filter.HisExpMestViewFilter filter = new MOS.Filter.HisExpMestViewFilter();
                        filter.AGGR_EXP_MEST_ID = expMestData.First().ID;
                        var lstExpChild = new BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST>>("api/HisExpMest/GetView", ApiConsumer.ApiConsumers.MosConsumer, filter, null);
                        expMestData.AddRange(lstExpChild);
                    }

                    DataThread = new ADO.ThreadDataADO(expMestData);

                    CreateThreadLoadData(DataThread);

                    if ((DataThread.ListHisServiceReq == null || DataThread.ListHisServiceReq.Count <= 0)
                        || ((DataThread.ListExpMestMaterial == null || DataThread.ListExpMestMaterial.Count <= 0)
                        && (DataThread.ListExpMestMedicine == null || DataThread.ListExpMestMedicine.Count <= 0)))
                    {
                        Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => DataThread), DataThread));
                        return;
                    }

                    FillDataToInfo(DataThread.ListHisServiceReq.FirstOrDefault());

                    List<ADO.PrescriptionADO> ado = ProcessDataGrid(DataThread);

                    WaitingManager.Hide();
                    if (ado != null && ado.Count > 0)
                    {
                        ado = ado.OrderBy(o => o.EXP_MEST_CODE).ToList();

                        FillDataToGrid(ado);

                        ProcessAutoApproval(expMestData);
                    }
                    else
                    {
                        if (!String.IsNullOrWhiteSpace(TxtTreatmentCode.Text))
                            DevExpress.XtraEditors.XtraMessageBox.Show(string.Format(Resources.ResourceMessage.MaDieuTriKhongHopLe, TxtTreatmentCode.Text));
                        else if (!String.IsNullOrWhiteSpace(txtExpMestCode.Text))
                            DevExpress.XtraEditors.XtraMessageBox.Show(string.Format(Resources.ResourceMessage.MaXuatKhongHopLe, txtExpMestCode.Text));
                        else if (!String.IsNullOrWhiteSpace(txtTDLServiceReqCode.Text))
                            DevExpress.XtraEditors.XtraMessageBox.Show(string.Format(Resources.ResourceMessage.MaYLenhKhongHopLe, txtTDLServiceReqCode.Text));
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessAutoApproval(List<V_HIS_EXP_MEST> expMestData)
        {
            try
            {
                if (expMestData != null && expMestData.Count > 0)
                {
                    //tu dong duyet
                    if (!IsNotAutoApproval)
                    {
                        bool success = true;
                        bool execute = expMestData.Exists(s => !s.AGGR_EXP_MEST_ID.HasValue && s.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST);
                        CommonParam param = new CommonParam();

                        foreach (var prescription in expMestData)
                        {
                            if (!prescription.AGGR_EXP_MEST_ID.HasValue && prescription.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST)
                            {
                                bool resultExport = false;
                                CommonParam paramrResult = new CommonParam();
                                ApprovalPrescription(prescription, ref resultExport, ref paramrResult);

                                success = success & resultExport;
                                if (paramrResult.Messages != null && paramrResult.Messages.Count > 0)
                                {
                                    if (param.Messages == null) param.Messages = new List<string>();

                                    param.Messages.AddRange(paramrResult.Messages);
                                }
                                if (paramrResult.BugCodes != null && paramrResult.BugCodes.Count > 0)
                                {
                                    if (param.BugCodes == null) param.BugCodes = new List<string>();

                                    param.BugCodes.AddRange(paramrResult.BugCodes);
                                }
                            }
                        }

                        if (execute)
                        {
                            if (param.Messages != null && param.Messages.Count > 0)
                            {
                                param.Messages = param.Messages.Distinct().ToList();
                            }

                            if (param.BugCodes != null && param.BugCodes.Count > 0)
                            {
                                param.BugCodes = param.BugCodes.Distinct().ToList();
                            }

                            #region Show message
                            Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                            #endregion

                            #region Process has exception
                            HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                            #endregion
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<ADO.PrescriptionADO> ProcessDataGrid(ADO.ThreadDataADO DataThread)
        {
            List<ADO.PrescriptionADO> result = new List<ADO.PrescriptionADO>();
            try
            {
                Dictionary<long, List<string>> dicServiceReqCode = new Dictionary<long, List<string>>();
                Dictionary<long, List<string>> dicIntructionTimeStr = new Dictionary<long, List<string>>();
                Dictionary<long, List<string>> dicRequestLoginname = new Dictionary<long, List<string>>();
                Dictionary<long, List<string>> dicRequestUsername = new Dictionary<long, List<string>>();
                Dictionary<long, List<string>> dicRequestRoomName = new Dictionary<long, List<string>>();

                if (DataThread != null)
                {
                    if (DataThread.ListExpMestMaterial != null && DataThread.ListExpMestMaterial.Count > 0)
                    {
                        var expMestMatyGroups = DataThread.ListExpMestMaterial.GroupBy(o => new { o.TDL_MATERIAL_TYPE_ID, o.EXP_MEST_ID, o.PRICE }).ToList();
                        foreach (var item in expMestMatyGroups)
                        {
                            if (!item.First().EXP_MEST_ID.HasValue) continue;

                            ADO.PrescriptionADO pres = new ADO.PrescriptionADO(item.First());

                            //gán bằng phiếu tổng hợp để duyệt và thực xuất trên phiếu tổng hợp
                            if (item.First().AGGR_EXP_MEST_ID.HasValue)
                            {
                                pres.EXP_MEST_ID = item.First().AGGR_EXP_MEST_ID;
                            }

                            var expMest = DataThread.ListHisExpMest.FirstOrDefault(o => o.ID == pres.EXP_MEST_ID);
                            if (expMest == null) continue;

                            pres.EXP_MEST_CODE = expMest.EXP_MEST_CODE;

                            pres.type = 2;
                            pres.NUM_ORDER = item.First().NUM_ORDER ?? 999999;
                            pres.AMOUNT = item.Sum(o => o.AMOUNT);
                            pres.TOTAL_PRICE = ((pres.PRICE ?? 0) * pres.AMOUNT * (1 + (pres.VAT_RATIO ?? 0))) - (pres.DISCOUNT ?? 0);

                            pres.DETAIL_CODE = item.First().MATERIAL_TYPE_CODE;
                            pres.DETAIL_NAME = item.First().MATERIAL_TYPE_NAME;
                            pres.TUTORIAL = item.First().TUTORIAL;

                            var serviceReq = DataThread.ListHisServiceReq.FirstOrDefault(o => o.ID == item.First().TDL_SERVICE_REQ_ID);
                            if (serviceReq != null)
                            {
                                pres.SERVICE_REQ_CODE = serviceReq.SERVICE_REQ_CODE;
                                pres.INTRUCTION_TIME = serviceReq.INTRUCTION_TIME;
                                pres.INTRUCTION_TIME_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(serviceReq.INTRUCTION_TIME);
                                pres.REQUEST_LOGINNAME = serviceReq.REQUEST_LOGINNAME;
                                pres.REQUEST_USERNAME = serviceReq.REQUEST_USERNAME;
                                var room = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == serviceReq.REQUEST_ROOM_ID);
                                if (room != null)
                                {
                                    pres.REQUEST_ROOM_NAME = room.ROOM_NAME;
                                }

                                #region dicData
                                if (!dicServiceReqCode.ContainsKey(pres.EXP_MEST_ID ?? 0))
                                {
                                    dicServiceReqCode[pres.EXP_MEST_ID ?? 0] = new List<string>();
                                }

                                dicServiceReqCode[pres.EXP_MEST_ID ?? 0].Add(pres.SERVICE_REQ_CODE);

                                if (!dicIntructionTimeStr.ContainsKey(pres.EXP_MEST_ID ?? 0))
                                {
                                    dicIntructionTimeStr[pres.EXP_MEST_ID ?? 0] = new List<string>();
                                }

                                dicIntructionTimeStr[pres.EXP_MEST_ID ?? 0].Add(pres.INTRUCTION_TIME_STR);

                                if (!dicRequestLoginname.ContainsKey(pres.EXP_MEST_ID ?? 0))
                                {
                                    dicRequestLoginname[pres.EXP_MEST_ID ?? 0] = new List<string>();
                                }

                                dicRequestLoginname[pres.EXP_MEST_ID ?? 0].Add(pres.REQUEST_LOGINNAME);

                                if (!dicRequestUsername.ContainsKey(pres.EXP_MEST_ID ?? 0))
                                {
                                    dicRequestUsername[pres.EXP_MEST_ID ?? 0] = new List<string>();
                                }

                                dicRequestUsername[pres.EXP_MEST_ID ?? 0].Add(pres.REQUEST_USERNAME);

                                if (!dicRequestRoomName.ContainsKey(pres.EXP_MEST_ID ?? 0))
                                {
                                    dicRequestRoomName[pres.EXP_MEST_ID ?? 0] = new List<string>();
                                }

                                dicRequestRoomName[pres.EXP_MEST_ID ?? 0].Add(pres.REQUEST_ROOM_NAME);
                                #endregion
                            }

                            result.Add(pres);
                        }
                    }

                    if (DataThread.ListExpMestMedicine != null && DataThread.ListExpMestMedicine.Count > 0)
                    {
                        var expMestMetyGroups = DataThread.ListExpMestMedicine.GroupBy(o => new { o.TDL_MEDICINE_TYPE_ID, o.EXP_MEST_ID, o.PRICE }).ToList();
                        foreach (var item in expMestMetyGroups)
                        {
                            if (!item.First().EXP_MEST_ID.HasValue) continue;

                            ADO.PrescriptionADO pres = new ADO.PrescriptionADO(item.First());

                            //gán bằng phiếu tổng hợp để duyệt và thực xuất trên phiếu tổng hợp
                            if (item.First().AGGR_EXP_MEST_ID.HasValue)
                            {
                                pres.EXP_MEST_ID = item.First().AGGR_EXP_MEST_ID;
                            }

                            var expMest = DataThread.ListHisExpMest.FirstOrDefault(o => o.ID == pres.EXP_MEST_ID);
                            if (expMest == null) continue;

                            pres.EXP_MEST_CODE = expMest.EXP_MEST_CODE;

                            pres.type = 1;
                            pres.NUM_ORDER = item.First().NUM_ORDER ?? 999999;
                            pres.AMOUNT = item.Sum(o => o.AMOUNT);
                            pres.TOTAL_PRICE = ((pres.PRICE ?? 0) * pres.AMOUNT * (1 + (pres.VAT_RATIO ?? 0))) - (pres.DISCOUNT ?? 0);

                            pres.DETAIL_NAME = item.First().MEDICINE_TYPE_NAME;
                            pres.DETAIL_CODE = item.First().MEDICINE_TYPE_CODE;
                            pres.TUTORIAL = item.First().TUTORIAL;

                            var serviceReq = DataThread.ListHisServiceReq.FirstOrDefault(o => o.ID == item.First().TDL_SERVICE_REQ_ID);
                            if (serviceReq != null)
                            {
                                pres.SERVICE_REQ_CODE = serviceReq.SERVICE_REQ_CODE;
                                pres.INTRUCTION_TIME = serviceReq.INTRUCTION_TIME;
                                pres.INTRUCTION_TIME_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(serviceReq.INTRUCTION_TIME);
                                pres.REQUEST_LOGINNAME = serviceReq.REQUEST_LOGINNAME;
                                pres.REQUEST_USERNAME = serviceReq.REQUEST_USERNAME;
                                var room = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == serviceReq.REQUEST_ROOM_ID);
                                if (room != null)
                                {
                                    pres.REQUEST_ROOM_NAME = room.ROOM_NAME;
                                }
                            }

                            #region dicData
                            if (!dicServiceReqCode.ContainsKey(pres.EXP_MEST_ID ?? 0))
                            {
                                dicServiceReqCode[pres.EXP_MEST_ID ?? 0] = new List<string>();
                            }

                            dicServiceReqCode[pres.EXP_MEST_ID ?? 0].Add(pres.SERVICE_REQ_CODE);

                            if (!dicIntructionTimeStr.ContainsKey(pres.EXP_MEST_ID ?? 0))
                            {
                                dicIntructionTimeStr[pres.EXP_MEST_ID ?? 0] = new List<string>();
                            }

                            dicIntructionTimeStr[pres.EXP_MEST_ID ?? 0].Add(pres.INTRUCTION_TIME_STR);

                            if (!dicRequestLoginname.ContainsKey(pres.EXP_MEST_ID ?? 0))
                            {
                                dicRequestLoginname[pres.EXP_MEST_ID ?? 0] = new List<string>();
                            }

                            dicRequestLoginname[pres.EXP_MEST_ID ?? 0].Add(pres.REQUEST_LOGINNAME);

                            if (!dicRequestUsername.ContainsKey(pres.EXP_MEST_ID ?? 0))
                            {
                                dicRequestUsername[pres.EXP_MEST_ID ?? 0] = new List<string>();
                            }

                            dicRequestUsername[pres.EXP_MEST_ID ?? 0].Add(pres.REQUEST_USERNAME);

                            if (!dicRequestRoomName.ContainsKey(pres.EXP_MEST_ID ?? 0))
                            {
                                dicRequestRoomName[pres.EXP_MEST_ID ?? 0] = new List<string>();
                            }

                            dicRequestRoomName[pres.EXP_MEST_ID ?? 0].Add(pres.REQUEST_ROOM_NAME);
                            #endregion

                            result.Add(pres);
                        }
                    }
                }

                if (result != null && result.Count > 0)
                {
                    foreach (var item in result)
                    {
                        if (dicServiceReqCode.ContainsKey(item.EXP_MEST_ID ?? 0))
                        {
                            List<string> lstServiceReqCode = dicServiceReqCode[item.EXP_MEST_ID ?? 0].Distinct().ToList();
                            item.SERVICE_REQ_CODE = string.Join(",", lstServiceReqCode);
                        }

                        if (dicIntructionTimeStr.ContainsKey(item.EXP_MEST_ID ?? 0))
                        {
                            List<string> lstIntructionTime = dicIntructionTimeStr[item.EXP_MEST_ID ?? 0].Distinct().ToList();
                            item.INTRUCTION_TIME_STR = string.Join(",", lstIntructionTime);
                        }

                        if (dicRequestLoginname.ContainsKey(item.EXP_MEST_ID ?? 0))
                        {
                            List<string> lstReqLoginname = dicRequestLoginname[item.EXP_MEST_ID ?? 0].Distinct().ToList();
                            item.REQUEST_LOGINNAME = string.Join(",", lstReqLoginname);
                        }

                        if (dicRequestUsername.ContainsKey(item.EXP_MEST_ID ?? 0))
                        {
                            List<string> lstReqUserName = dicRequestUsername[item.EXP_MEST_ID ?? 0].Distinct().ToList();
                            item.REQUEST_USERNAME = string.Join(",", lstReqUserName);
                        }

                        if (dicRequestRoomName.ContainsKey(item.EXP_MEST_ID ?? 0))
                        {
                            List<string> lstReqRoomName = dicRequestRoomName[item.EXP_MEST_ID ?? 0].Distinct().ToList();
                            item.REQUEST_ROOM_NAME = string.Join(",", lstReqRoomName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = new List<ADO.PrescriptionADO>();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void CreateThreadLoadData(ADO.ThreadDataADO dataThread)
        {
            Thread serviceReq = new Thread(ThreadLoadServiceReq);
            Thread Material = new Thread(ThreadLoadMaterial);
            Thread Medicine = new Thread(ThreadLoadMedicine);

            //serviceReq.Priority = ThreadPriority.Highest;
            //Material.Priority = ThreadPriority.Highest;
            //Medicine.Priority = ThreadPriority.Highest;
            try
            {
                serviceReq.Start(dataThread);
                Material.Start(dataThread);
                Medicine.Start(dataThread);

                serviceReq.Join();
                Material.Join();
                Medicine.Join();
            }
            catch (Exception ex)
            {
                serviceReq.Abort();
                Material.Abort();
                Medicine.Abort();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ThreadLoadMedicine(object obj)
        {
            try
            {
                if (obj != null && obj.GetType() == typeof(ADO.ThreadDataADO))
                {
                    var data = (ADO.ThreadDataADO)obj;
                    if (data.ListHisExpMest != null && data.ListHisExpMest.Count > 0)
                    {
                        var paramCommon = new CommonParam();
                        HisExpMestMedicineViewFilter mediFilter = new HisExpMestMedicineViewFilter();
                        mediFilter.EXP_MEST_IDs = data.ListHisExpMest.Select(s => s.ID).ToList();
                        var medicines = new BackendAdapter(paramCommon).Get<List<V_HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/GetView", ApiConsumer.ApiConsumers.MosConsumer, mediFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                        if (medicines != null && medicines.Count > 0)
                        {
                            data.ListExpMestMedicine = medicines;
                            //var expMestMetyGroups = medicines.GroupBy(o => new { o.TDL_MEDICINE_TYPE_ID, o.EXP_MEST_ID, o.PRICE }).ToList();
                            //var ado = new List<ADO.PrescriptionADO>();
                            //foreach (var item in expMestMetyGroups)
                            //{
                            //    if (!item.First().EXP_MEST_ID.HasValue) continue;

                            //    ADO.PrescriptionADO pres = new ADO.PrescriptionADO(item.First());

                            //    //Inventec.Common.Mapper.DataObjectMapper.Map<ADO.PrescriptionADO>(pres, item.First());
                            //    pres.EXP_MEST_CODE = data.ListHisExpMest.FirstOrDefault(o => o.ID == pres.EXP_MEST_ID).EXP_MEST_CODE;
                            //    pres.SERVICE_REQ_ID = data.ListHisExpMest.FirstOrDefault(o => o.ID == pres.EXP_MEST_ID).SERVICE_REQ_ID;

                            //    pres.type = 1;
                            //    pres.NUM_ORDER = item.First().NUM_ORDER ?? 999999;
                            //    //pres.IMP_PRICE = item.First().IMP_PRICE;
                            //    //pres.IMP_VAT_RATIO = item.First().IMP_VAT_RATIO;
                            //    pres.AMOUNT = item.Sum(o => o.AMOUNT);
                            //    pres.PRICE = item.First().PRICE;
                            //    pres.VAT_RATIO = item.First().VAT_RATIO;
                            //    pres.DISCOUNT = item.First().DISCOUNT;
                            //    pres.TOTAL_PRICE = ((pres.PRICE ?? 0) * pres.AMOUNT * (1 + (pres.VAT_RATIO ?? 0))) - (pres.DISCOUNT ?? 0);

                            //    var mety = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(o => o.ID == item.First().TDL_MEDICINE_TYPE_ID);
                            //    if (mety != null)
                            //    {
                            //        pres.DETAIL_NAME = mety.MEDICINE_TYPE_NAME;
                            //        pres.SERVICE_UNIT_NAME = mety.SERVICE_UNIT_NAME;
                            //        pres.DETAIL_CODE = mety.MEDICINE_TYPE_CODE;
                            //        pres.ACTIVE_INGR_BHYT_CODE = mety.ACTIVE_INGR_BHYT_CODE;
                            //        pres.ACTIVE_INGR_BHYT_NAME = mety.ACTIVE_INGR_BHYT_NAME;
                            //    }
                            //    ado.Add(pres);
                            //}
                            //data.PrescriptionMedicines = ado;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ThreadLoadMaterial(object obj)
        {
            try
            {
                if (obj != null && obj.GetType() == typeof(ADO.ThreadDataADO))
                {
                    var data = (ADO.ThreadDataADO)obj;
                    if (data.ListHisExpMest != null && data.ListHisExpMest.Count > 0)
                    {
                        var paramCommon = new CommonParam();
                        HisExpMestMaterialViewFilter mateFilter = new HisExpMestMaterialViewFilter();
                        mateFilter.EXP_MEST_IDs = data.ListHisExpMest.Select(s => s.ID).ToList();
                        var materials = new BackendAdapter(paramCommon).Get<List<V_HIS_EXP_MEST_MATERIAL>>("api/HisExpMestMaterial/GetView", ApiConsumer.ApiConsumers.MosConsumer, mateFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                        if (materials != null && materials.Count > 0)
                        {
                            data.ListExpMestMaterial = materials;
                            //var expMestMatyGroups = materials.GroupBy(o => new { o.TDL_MATERIAL_TYPE_ID, o.EXP_MEST_ID, o.PRICE }).ToList();
                            //var ado = new List<ADO.PrescriptionADO>();
                            //foreach (var item in expMestMatyGroups)
                            //{
                            //    if (!item.First().EXP_MEST_ID.HasValue) continue;

                            //    ADO.PrescriptionADO pres = new ADO.PrescriptionADO(item.First());

                            //    //Inventec.Common.Mapper.DataObjectMapper.Map<ADO.PrescriptionADO>(pres, item.First());
                            //    pres.EXP_MEST_CODE = data.ListHisExpMest.FirstOrDefault(o => o.ID == pres.EXP_MEST_ID).EXP_MEST_CODE;
                            //    pres.SERVICE_REQ_ID = data.ListHisExpMest.FirstOrDefault(o => o.ID == pres.EXP_MEST_ID).SERVICE_REQ_ID;

                            //    pres.type = 2;
                            //    pres.NUM_ORDER = item.First().NUM_ORDER ?? 999999;
                            //    //pres.IMP_PRICE = item.First().IMP_PRICE;
                            //    //pres.IMP_VAT_RATIO = item.First().IMP_VAT_RATIO;
                            //    pres.AMOUNT = item.Sum(o => o.AMOUNT);
                            //    pres.PRICE = item.First().PRICE;
                            //    pres.VAT_RATIO = item.First().VAT_RATIO;
                            //    pres.DISCOUNT = item.First().DISCOUNT;
                            //    pres.TOTAL_PRICE = ((pres.PRICE ?? 0) * pres.AMOUNT * (1 + (pres.VAT_RATIO ?? 0))) - (pres.DISCOUNT ?? 0);

                            //    var maty = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(o => o.ID == item.First().TDL_MATERIAL_TYPE_ID);
                            //    if (maty != null)
                            //    {
                            //        pres.DETAIL_CODE = maty.MATERIAL_TYPE_CODE;
                            //        pres.DETAIL_NAME = maty.MATERIAL_TYPE_NAME;
                            //        pres.SERVICE_UNIT_NAME = maty.SERVICE_UNIT_NAME;
                            //    }
                            //    ado.Add(pres);
                            //}
                            //data.PrescriptionMaterials = ado;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ThreadLoadServiceReq(object obj)
        {
            try
            {
                if (obj != null && obj.GetType() == typeof(ADO.ThreadDataADO))
                {
                    var data = (ADO.ThreadDataADO)obj;
                    if (data.ListHisExpMest != null && data.ListHisExpMest.Count > 0)
                    {
                        var paramCommon = new CommonParam();
                        HisServiceReqFilter filter = new HisServiceReqFilter();
                        filter.IDs = data.ListHisExpMest.Select(s => s.SERVICE_REQ_ID ?? 0).Distinct().ToList();
                        var apiResult = new BackendAdapter(paramCommon).Get<List<HIS_SERVICE_REQ>>(RequestUriStore.HIS_SERVICE_REQ_GET, ApiConsumer.ApiConsumers.MosConsumer, filter, paramCommon);
                        if (apiResult != null && apiResult.Count > 0)
                        {
                            if (data.ListHisServiceReq == null) data.ListHisServiceReq = new List<HIS_SERVICE_REQ>();

                            data.ListHisServiceReq.AddRange(apiResult);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToInfo(HIS_SERVICE_REQ data)
        {
            try
            {
                if (data != null)
                {
                    this.lblDob.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.TDL_PATIENT_DOB);
                    this.lblGender.Text = data.TDL_PATIENT_GENDER_NAME;
                    this.lblPatientCode.Text = data.TDL_PATIENT_CODE;
                    this.lblVirAddress.Text = data.TDL_PATIENT_ADDRESS;
                    this.lblVirPatientName.Text = data.TDL_PATIENT_NAME;
                    this.lblUseTime.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.INTRUCTION_TIME) + " - " + Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.USE_TIME_TO ?? 0);

                    CommonParam par = new CommonParam();
                    MOS.Filter.HisTreatmentFilter _TreatmentFilter = new HisTreatmentFilter();
                    _TreatmentFilter.ID = data.TREATMENT_ID;
                    histreatment = new BackendAdapter(par).Get<List<HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_GET, ApiConsumers.MosConsumer, _TreatmentFilter, par);
                    var _Treatment = histreatment != null && histreatment.Count > 0 ? histreatment.FirstOrDefault(o => o.ID == data.TREATMENT_ID) : null;
                    if (_Treatment != null)
                    {
                        lblIcdName.Text = _Treatment.ICD_CODE + " - " + _Treatment.ICD_NAME;
                        lblIcdText.Text = _Treatment.ICD_SUB_CODE + " - " + _Treatment.ICD_TEXT;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGrid(List<ADO.PrescriptionADO> data)
        {
            try
            {
                listDataGrid = new List<ADO.PrescriptionADO>();

                if (data != null)
                {
                    listDataGrid.AddRange(data);
                }

                listDataGrid = listDataGrid.OrderBy(o => o.type).ThenBy(o => o.NUM_ORDER ?? 99999).ThenBy(o => o.DETAIL_NAME).ToList();

                gridControl1.BeginUpdate();
                gridControl1.DataSource = listDataGrid;
                gridControl1.EndUpdate();
                gridView.ExpandAllGroups();
                ProcessTotalPrice();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessTotalPrice()
        {
            try
            {
                if (listDataGrid != null && listDataGrid.Count > 0)
                {
                    var selectData = listDataGrid.Where(o => o.IsCheck).ToList();
                    if (selectData != null && selectData.Count > 0)
                    {
                        lblTotalPrice.Text = Inventec.Common.Number.Convert.NumberToString(selectData.Sum(o => o.TOTAL_PRICE), ConfigApplications.NumberSeperator);
                    }
                    else
                    {
                        lblTotalPrice.Text = "0";
                    }
                }
                else
                {
                    lblTotalPrice.Text = "0";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ApprovalPrescription(V_HIS_EXP_MEST prescription, ref bool success, ref CommonParam param)
        {
            try
            {
                if (prescription != null)
                {
                    HisExpMestResultSDO apiResult = null;
                    WaitingManager.Show();

                    //phiếu được tổng hợp thì ko duyệt
                    if (prescription.AGGR_EXP_MEST_ID.HasValue)
                    {
                        success = true;
                        return;
                    }

                    MOS.SDO.HisExpMestApproveSDO hisExpMestApproveSDO = new MOS.SDO.HisExpMestApproveSDO();

                    hisExpMestApproveSDO.ExpMestId = prescription.ID;
                    hisExpMestApproveSDO.IsFinish = true;
                    hisExpMestApproveSDO.ReqRoomId = this.moduleData.RoomId;

                    if (prescription.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DDT)
                    {
                        apiResult = new Inventec.Common.Adapter.BackendAdapter(param).Post<HisExpMestResultSDO>("api/HisExpMest/InPresApprove", ApiConsumer.ApiConsumers.MosConsumer, hisExpMestApproveSDO, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                    }
                    else if (prescription.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__THPK)
                    {
                        HisExpMestSDO hisExpMestSDO = new MOS.SDO.HisExpMestSDO();

                        hisExpMestSDO.ExpMestId = prescription.ID;
                        hisExpMestSDO.ReqRoomId = this.moduleData.RoomId;

                        var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<List<HIS_EXP_MEST>>("api/HisExpMest/AggrExamApprove", ApiConsumers.MosConsumer, hisExpMestSDO, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                        if (rs != null)
                        {
                            apiResult = new HisExpMestResultSDO();
                        }
                    }
                    else
                    {
                        apiResult = new Inventec.Common.Adapter.BackendAdapter(param).Post<HisExpMestResultSDO>(RequestUriStore.HIS_EXP_MEST_APPROVA, ApiConsumer.ApiConsumers.MosConsumer, hisExpMestApproveSDO, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                    }

                    if (apiResult != null)
                    {
                        success = true;
                    }

                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void CallPatient(V_HIS_EXP_MEST prescription)
        {
            try
            {
                if (prescription == null || prescription.ID == 0)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(Resources.ResourceMessage.ThongTinBenhNhanKhongHopLe);
                    txtExpMestCode.Focus();
                    txtExpMestCode.SelectAll();
                    return;
                }

                //gọi bệnh nhân
                string moiBenhNhanStr = HisConfigs.Get<string>(Config.CALL_PATIENT_MOI_BENH_NHAN);
                string denStr = HisConfigs.Get<string>(Config.CALL_PATIENT_DEN);

                if (String.IsNullOrEmpty(txtCong.Text.Trim()))
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(Resources.ResourceMessage.BanChuaNhapSo + congstr);
                    txtCong.Focus();
                    txtCong.SelectAll();
                    return;
                }
                WaitingManager.Show();
                //truyền tên, năm sinh, cổng

                int cong = Inventec.Common.TypeConvert.Parse.ToInt32(txtCong.Text.Trim().ToString());
                string name = prescription.TDL_PATIENT_NAME;
                int dob = Inventec.Common.TypeConvert.Parse.ToInt32(prescription.TDL_PATIENT_DOB.ToString().Substring(0, 4));

                if (this.clienttManager == null)
                {
                    this.clienttManager = new CPA.WCFClient.CallPatientClient.CallPatientClientManager();
                }
                this.clienttManager.CallNameAndDob(cong, name, dob);

                WaitingManager.Hide();
                btnExported.Focus();
                //thực xuất
                //ExportPrescription();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ExportPrescription(V_HIS_EXP_MEST prescription, ref bool success, ref CommonParam param)
        {
            try
            {
                if (prescription != null && prescription.ID > 0)
                {
                    //phiếu được tổng hợp thì ko xuất
                    if (prescription.AGGR_EXP_MEST_ID.HasValue)
                    {
                        success = true;
                        return;
                    }

                    if (prescription.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DDT)
                    {
                        MOS.SDO.HisExpMestSDO sdo = new MOS.SDO.HisExpMestSDO();
                        sdo.ExpMestId = prescription.ID;
                        sdo.ReqRoomId = this.moduleData.RoomId;
                        //sdo.IsFinish = true;
                        var apiresult = new Inventec.Common.Adapter.BackendAdapter
                            (param).Post<MOS.EFMODEL.DataModels.HIS_EXP_MEST>
                            ("api/HisExpMest/InPresExport", ApiConsumer.ApiConsumers.MosConsumer, sdo, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                        if (apiresult != null)
                        {
                            success = true;
                        }
                    }
                    else if (prescription.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__THPK)
                    {
                        HisExpMestSDO sdo = new HisExpMestSDO();
                        sdo.ExpMestId = prescription.ID;
                        sdo.ReqRoomId = this.moduleData.RoomId;
                        //sdo.IsFinish = true;
                        var apiresult = new Inventec.Common.Adapter.BackendAdapter
                            (param).Post<MOS.EFMODEL.DataModels.HIS_EXP_MEST>
                            ("api/HisExpMest/AggrExamExport", ApiConsumer.ApiConsumers.MosConsumer, sdo, param);
                        if (apiresult != null)
                        {
                            success = true;
                        }
                    }
                    else
                    {
                        MOS.SDO.HisExpMestExportSDO sdo = new MOS.SDO.HisExpMestExportSDO();
                        sdo.ExpMestId = prescription.ID;
                        sdo.ReqRoomId = this.moduleData.RoomId;
                        sdo.IsFinish = true;
                        var apiresult = new Inventec.Common.Adapter.BackendAdapter
                            (param).Post<MOS.EFMODEL.DataModels.HIS_EXP_MEST>
                            (ApiConsumer.HisRequestUriStore.HIS_EXP_MEST_EXPORT, ApiConsumer.ApiConsumers.MosConsumer, sdo, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                        if (apiresult != null)
                        {
                            success = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool ProcessExport()
        {
            bool result = false;
            try
            {
                var listDataExport = new List<V_HIS_EXP_MEST>();
                if (listDataGrid != null && listDataGrid.Count > 0)
                {
                    var expMestIds = listDataGrid.Where(s => s.IsCheck).ToList();
                    if (expMestIds != null && expMestIds.Count > 0)
                    {
                        var listExecute = DataThread.ListHisExpMest.Where(o => expMestIds.Select(s => s.EXP_MEST_ID).Contains(o.ID)).ToList();
                        if (listExecute != null && listExecute.Count > 0)
                        {
                            listDataExport.AddRange(listExecute);

                            var aggr = listExecute.Select(s => s.AGGR_EXP_MEST_ID ?? 0).Distinct().ToList();// luôn có id = 0
                            var lstAggr = DataThread.ListHisExpMest.Where(o => aggr.Contains(o.ID)).ToList();
                            if (lstAggr != null && lstAggr.Count > 0) listDataExport.AddRange(lstAggr);
                        }
                    }
                }

                if (listDataExport != null && listDataExport.Count > 0)
                {
                    bool success = true;
                    CommonParam param = new CommonParam();
                    param.Messages = new List<string>();
                    param.BugCodes = new List<string>();

                    foreach (var item in listDataExport)
                    {
                        bool resultExport = false;
                        CommonParam paramrResult = new CommonParam();
                        ExportPrescription(item, ref resultExport, ref paramrResult);

                        success = success & resultExport;
                        if (paramrResult.Messages != null && paramrResult.Messages.Count > 0)
                        {
                            param.Messages.AddRange(paramrResult.Messages);
                        }
                        if (paramrResult.BugCodes != null && paramrResult.BugCodes.Count > 0)
                        {
                            param.BugCodes.AddRange(paramrResult.BugCodes);
                        }
                    }

                    if (param.Messages != null && param.Messages.Count > 0)
                    {
                        param.Messages = param.Messages.Distinct().ToList();
                    }

                    if (param.BugCodes != null && param.BugCodes.Count > 0)
                    {
                        param.BugCodes = param.BugCodes.Distinct().ToList();
                    }
                    result = success;
                    if (success)
                        SetFocus();
                    #region Show message
                    Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                    #endregion

                    #region Process has exception
                    HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                    #endregion
                }
                else
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(Resources.ResourceMessage.VuiLongChonPhieuXuat);
                    Inventec.Common.Logging.LogSystem.Error("listDataExport null");
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        #region in HDSD
        Inventec.Common.SignLibrary.ADO.InputADO inputADO = new Inventec.Common.SignLibrary.ADO.InputADO();
        string printTypeCode = "";
        string printerName = "";

        private void ProcessPrintHDSD()
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__HuongDanSuDungThuoc_MPS000099, DelegateRunPrinter);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool DelegateRunPrinter(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                InHuongDanSuDung(printTypeCode, fileName, ref result);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void InHuongDanSuDung(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                CommonParam param = new CommonParam();
                string genderName = "";
                long? dob = null;
                if (DataThread == null || DataThread.ListHisExpMest == null || DataThread.ListHisServiceReq == null) return;

                var listDataExport = new List<V_HIS_EXP_MEST>();
                if (listDataGrid != null && listDataGrid.Count > 0)
                {
                    var cineChecked = listDataGrid.Where(s => s.IsCheck && s.TUTORIAL != null).ToList();
                    Inventec.Common.Logging.LogSystem.Warn("hướng dẫn sử dụng thuốc: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => cineChecked), cineChecked));
                    if (cineChecked != null && cineChecked.Count > 0)
                    {
                        var listExecute = this.DataThread.ListHisExpMest.Where(o => cineChecked.Select(s => s.EXP_MEST_ID).Contains(o.ID)).ToList();

                        foreach (var expMest in listExecute)
                        {
                            if (expMest.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__THPK)
                            {
                                var listExpChild = this.DataThread.ListHisExpMest.Where(o => o.AGGR_EXP_MEST_ID == expMest.ID).ToList();
                                foreach (var item in listExpChild)
                                {
                                    V_HIS_EXP_MEST exp = new V_HIS_EXP_MEST();
                                    Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_EXP_MEST>(exp, item);
                                    listDataExport.Add(exp);
                                }
                            }
                            else
                            {
                                V_HIS_EXP_MEST exp = new V_HIS_EXP_MEST();
                                Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_EXP_MEST>(exp, expMest);
                                listDataExport.Add(exp);
                            }
                        }
                    }
                }
                if (listDataExport != null && listDataExport.Count > 0)
                {
                    V_HIS_EXP_MEST ExpMests = listDataExport.FirstOrDefault();

                    if (ExpMests.TDL_PATIENT_ID != null)
                    {
                        genderName = ExpMests.TDL_PATIENT_GENDER_NAME;
                        dob = ExpMests.TDL_PATIENT_DOB;
                    }

                    inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((ExpMests != null ? ExpMests.TDL_TREATMENT_CODE : ""), printTypeCode, this.moduleData != null ? this.moduleData.RoomId : 0);

                    List<V_HIS_EXP_MEST_MEDICINE> medicines = new List<V_HIS_EXP_MEST_MEDICINE>();

                    if (DataThread.ListExpMestMedicine != null && DataThread.ListExpMestMedicine.Count > 0)
                    {
                        medicines = DataThread.ListExpMestMedicine.Where(o => listDataExport.Select(s => s.ID).Contains(o.EXP_MEST_ID ?? 0)).ToList();
                        //if (lstMedicine != null && lstMedicine.Count > 0)
                        //{
                        //    foreach (var item in lstMedicine)
                        //    {
                        //        V_HIS_EXP_MEST_MEDICINE medi = new V_HIS_EXP_MEST_MEDICINE();
                        //        Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_EXP_MEST_MEDICINE>(medi, item);
                        //        medicines.Add(medi);
                        //    }
                        //}
                    }

                    List<V_HIS_EXP_MEST_MATERIAL> materials = new List<V_HIS_EXP_MEST_MATERIAL>();
                    if (DataThread.ListExpMestMaterial != null && DataThread.ListExpMestMaterial.Count > 0)
                    {
                        materials = DataThread.ListExpMestMaterial.Where(o => listDataExport.Select(s => s.ID).Contains(o.EXP_MEST_ID ?? 0)).ToList();
                        //if (lstMaterial != null && lstMaterial.Count > 0)
                        //{
                        //    foreach (var item in lstMaterial)
                        //    {
                        //        V_HIS_EXP_MEST_MATERIAL mate = new V_HIS_EXP_MEST_MATERIAL();
                        //        Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_EXP_MEST_MATERIAL>(mate, item);
                        //        materials.Add(mate);
                        //    }
                        //}
                    }

                    Inventec.Common.Logging.LogSystem.Warn("hướng dẫn sử dụng thuốc2: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => medicines), medicines));

                    MPS.Processor.Mps000099.PDO.Mps000099PDO pdo = new MPS.Processor.Mps000099.PDO.Mps000099PDO(
                    listDataExport,
                    medicines,
                    materials);
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO };
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO };
                    }

                    WaitingManager.Hide();
                    result = MPS.MpsPrinter.Run(PrintData);


                    Inventec.Common.Logging.LogSystem.Warn("hướng dẫn sử Vật tư1: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => DataThread.ListExpMestMaterial), DataThread.ListExpMestMaterial));

                    Inventec.Common.Logging.LogSystem.Warn("hướng dẫn sử Vật tư2: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => materials), materials));
                    Inventec.Common.Logging.LogSystem.Warn("hướng dẫn sử listDataExport: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => listDataExport), listDataExport));

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        private void SetFocus()
        {
            try
            {
                if (!string.IsNullOrEmpty(TxtTreatmentCode.Text))
                {
                    TxtTreatmentCode.Focus();
                    TxtTreatmentCode.SelectAll();
                }
                else if (!string.IsNullOrEmpty(txtExpMestCode.Text))
                {
                    txtExpMestCode.Focus();
                    txtExpMestCode.SelectAll();
                }
                else if (!string.IsNullOrEmpty(txtTDLServiceReqCode.Text))
                {
                    txtTDLServiceReqCode.Focus();
                    txtTDLServiceReqCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessPrint(bool printNow)
        {
            try
            {
                if (DataThread == null || DataThread.ListHisExpMest == null || DataThread.ListHisServiceReq == null) return;

                var listDataExport = new List<HIS_EXP_MEST>();
                if (listDataGrid != null && listDataGrid.Count > 0)
                {
                    var cineChecked = listDataGrid.Where(s => s.IsCheck).ToList();
                    if (cineChecked != null && cineChecked.Count > 0)
                    {
                        var listExecute = this.DataThread.ListHisExpMest.Where(o => cineChecked.Select(s => s.EXP_MEST_ID).Contains(o.ID)).ToList();
                        foreach (var expMest in listExecute)
                        {
                            if (expMest.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__THPK)
                            {
                                var listExpChild = this.DataThread.ListHisExpMest.Where(o => o.AGGR_EXP_MEST_ID == expMest.ID).ToList();
                                foreach (var item in listExpChild)
                                {
                                    HIS_EXP_MEST exp = new HIS_EXP_MEST();
                                    Inventec.Common.Mapper.DataObjectMapper.Map<HIS_EXP_MEST>(exp, item);
                                    listDataExport.Add(exp);
                                }
                            }
                            else
                            {
                                HIS_EXP_MEST exp = new HIS_EXP_MEST();
                                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_EXP_MEST>(exp, expMest);
                                listDataExport.Add(exp);
                            }
                        }
                    }
                }

                if (listDataExport != null && listDataExport.Count > 0)
                {
                    List<MOS.SDO.OutPatientPresResultSDO> listOutPatientPresResultSDO = new List<OutPatientPresResultSDO>();
                    MOS.SDO.OutPatientPresResultSDO outPatientPresResultSDO = new OutPatientPresResultSDO();
                    outPatientPresResultSDO.ExpMests = listDataExport;

                    List<HIS_EXP_MEST_MEDICINE> medicines = new List<HIS_EXP_MEST_MEDICINE>();
                    List<HIS_EXP_MEST_MATERIAL> material = new List<HIS_EXP_MEST_MATERIAL>();
                    List<HIS_SERVICE_REQ> ServiceReqs = new List<HIS_SERVICE_REQ>();

                    ServiceReqs = DataThread.ListHisServiceReq.Where(o => listDataExport.Select(s => s.SERVICE_REQ_ID).Contains(o.ID)).ToList();

                    if (DataThread.ListExpMestMaterial != null && DataThread.ListExpMestMaterial.Count > 0)
                    {
                        var lstMaterial = DataThread.ListExpMestMaterial.Where(o => listDataExport.Select(s => s.ID).Contains(o.EXP_MEST_ID ?? 0)).ToList();
                        if (lstMaterial != null && lstMaterial.Count > 0)
                        {
                            foreach (var item in lstMaterial)
                            {
                                HIS_EXP_MEST_MATERIAL mate = new HIS_EXP_MEST_MATERIAL();
                                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_EXP_MEST_MATERIAL>(mate, item);
                                material.Add(mate);
                            }
                        }
                    }

                    if (DataThread.ListExpMestMedicine != null && DataThread.ListExpMestMedicine.Count > 0)
                    {
                        var lstMedicine = DataThread.ListExpMestMedicine.Where(o => listDataExport.Select(s => s.ID).Contains(o.EXP_MEST_ID ?? 0)).ToList();
                        if (lstMedicine != null && lstMedicine.Count > 0)
                        {
                            foreach (var item in lstMedicine)
                            {
                                HIS_EXP_MEST_MEDICINE medi = new HIS_EXP_MEST_MEDICINE();
                                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_EXP_MEST_MEDICINE>(medi, item);
                                medicines.Add(medi);
                            }
                        }
                    }

                    outPatientPresResultSDO.ServiceReqs = ServiceReqs;
                    outPatientPresResultSDO.Medicines = medicines;
                    outPatientPresResultSDO.Materials = material;

                    listOutPatientPresResultSDO.Add(outPatientPresResultSDO);

                    var PrintPresProcessor = new Library.PrintPrescription.PrintPrescriptionProcessor(listOutPatientPresResultSDO);
                    PrintPresProcessor.Print("Mps000296", printNow);
                }
                else
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(Resources.ResourceMessage.VuiLongChonPhieuXuat);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessUncheck()
        {
            try
            {
                foreach (var item in listDataGrid)
                {
                    if (item.IsCheck)
                    {
                        item.IsCheck = false;
                    }
                }

                gridControl1.RefreshDataSource();
                ProcessTotalPrice();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion
        #endregion

        #region shotcut
        private void barButtonF8_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                txtTDLServiceReqCode.Focus();
                txtTDLServiceReqCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void barButtonItemFocus_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                txtExpMestCode.Focus();
                txtExpMestCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItemCall_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnCall_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnExported_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private void barBtnTreatmentCode_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                TxtTreatmentCode.Focus();
                TxtTreatmentCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                BtnApproval_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion














    }
}
