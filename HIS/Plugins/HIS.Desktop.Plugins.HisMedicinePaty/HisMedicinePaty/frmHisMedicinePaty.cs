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
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraNavBar;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using Inventec.UC.Paging;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utilities;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using Inventec.Desktop.Common.Controls.ValidationRule;
using DevExpress.XtraEditors.DXErrorProvider;
using HIS.Desktop.Plugins.HisMedicinePaty;
using Inventec.Desktop.Common.LanguageManager;
using System.Resources;
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.ADO;
using DevExpress.XtraGrid.Views.Grid;

namespace HIS.Desktop.Plugins.HisMedicinePaty
{
    public partial class frmHisMedicinePaty : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        PagingGrid pagingGrid;
        int ActionType = -1;
        int positionHandle = -1;
        MOS.EFMODEL.DataModels.V_HIS_MEDICINE_PATY currentData;
        MOS.EFMODEL.DataModels.V_HIS_MEDICINE currentMedicine;
        List<string> arrControlEnableNotChange = new List<string>();
        Dictionary<string, int> dicOrderTabIndexControl = new Dictionary<string, int>();
        Inventec.Desktop.Common.Modules.Module moduleData;
        List<V_HIS_MEDICINE> listMedicine = new List<V_HIS_MEDICINE>();
        #endregion

        #region Construct
        public frmHisMedicinePaty(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            try
            {
                InitializeComponent();

                pagingGrid = new PagingGrid();
                this.moduleData = moduleData;
                gridControlFormList.ToolTipController = toolTipControllerGrid;

                try
                {
                    string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                    this.Icon = Icon.ExtractAssociatedIcon(iconPath);
                }
                catch (Exception ex)
                {
                    LogSystem.Warn(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public frmHisMedicinePaty(Inventec.Desktop.Common.Modules.Module moduleData, MOS.EFMODEL.DataModels.V_HIS_MEDICINE medicine)
        {
            try
            {
                InitializeComponent();

                pagingGrid = new PagingGrid();
                this.moduleData = moduleData;
                gridControlFormList.ToolTipController = toolTipControllerGrid;

                try
                {
                    if (medicine != null)
                    {
                        currentMedicine = medicine;
                    }
                    string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                    this.Icon = Icon.ExtractAssociatedIcon(iconPath);
                }
                catch (Exception ex)
                {
                    LogSystem.Warn(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Private method
        private void frmHisKskContract_Load(object sender, EventArgs e)
        {
            try
            {
                MeShow();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {

                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisMedicinePaty.Resources.Lang", typeof(HIS.Desktop.Plugins.HisMedicinePaty.frmHisMedicinePaty).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("frmHisMedicinePaty.layoutControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl7.Text = Inventec.Common.Resource.Get.Value("frmHisMedicinePaty.layoutControl7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("frmHisMedicinePaty.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.ToolTip = Inventec.Common.Resource.Get.Value("frmHisMedicinePaty.gridColumn1.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnEdit.ToolTip = Inventec.Common.Resource.Get.Value("frmHisMedicinePaty.gridColumnEdit.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCode.Caption = Inventec.Common.Resource.Get.Value("frmHisMedicinePaty.grdColCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCode.ToolTip = Inventec.Common.Resource.Get.Value("frmHisMedicinePaty.grdColCode.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColRatio.Caption = Inventec.Common.Resource.Get.Value("frmHisMedicinePaty.grdColRatio.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColRatio.ToolTip = Inventec.Common.Resource.Get.Value("frmHisMedicinePaty.grdColRatio.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColMaxFee.Caption = Inventec.Common.Resource.Get.Value("frmHisMedicinePaty.grdColMaxFee.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColMaxFee.ToolTip = Inventec.Common.Resource.Get.Value("frmHisMedicinePaty.grdColMaxFee.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColSignTime.Caption = Inventec.Common.Resource.Get.Value("frmHisMedicinePaty.grdColSignTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColSignTime.ToolTip = Inventec.Common.Resource.Get.Value("frmHisMedicinePaty.grdColSignTime.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCustomerName.Caption = Inventec.Common.Resource.Get.Value("frmHisMedicinePaty.grdColCustomerName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCustomerName.ToolTip = Inventec.Common.Resource.Get.Value("frmHisMedicinePaty.grdColCustomerName.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColDelegateName.Caption = Inventec.Common.Resource.Get.Value("frmHisMedicinePaty.grdColDelegateName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColDelegateName.ToolTip = Inventec.Common.Resource.Get.Value("frmHisMedicinePaty.grdColDelegateName.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPhone.Caption = Inventec.Common.Resource.Get.Value("frmHisMedicinePaty.grdColPhone.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPhone.ToolTip = Inventec.Common.Resource.Get.Value("frmHisMedicinePaty.grdColPhone.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColFax.Caption = Inventec.Common.Resource.Get.Value("frmHisMedicinePaty.grdColFax.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColFax.ToolTip = Inventec.Common.Resource.Get.Value("frmHisMedicinePaty.grdColFax.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColTaxCode.Caption = Inventec.Common.Resource.Get.Value("frmHisMedicinePaty.grdColTaxCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColTaxCode.ToolTip = Inventec.Common.Resource.Get.Value("frmHisMedicinePaty.grdColTaxCode.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColAccountNumber.Caption = Inventec.Common.Resource.Get.Value("frmHisMedicinePaty.grdColAccountNumber.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColAccountNumber.ToolTip = Inventec.Common.Resource.Get.Value("frmHisMedicinePaty.grdColAccountNumber.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColExpireTimeFrom.Caption = Inventec.Common.Resource.Get.Value("frmHisMedicinePaty.grdColExpireTimeFrom.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColExpireTimeFrom.ToolTip = Inventec.Common.Resource.Get.Value("frmHisMedicinePaty.grdColExpireTimeFrom.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColExpireTimeTo.Caption = Inventec.Common.Resource.Get.Value("frmHisMedicinePaty.grdColExpireTimeTo.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColExpireTimeTo.ToolTip = Inventec.Common.Resource.Get.Value("frmHisMedicinePaty.grdColExpireTimeTo.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("frmHisMedicinePaty.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.ToolTip = Inventec.Common.Resource.Get.Value("frmHisMedicinePaty.gridColumn2.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("frmHisMedicinePaty.gridColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("frmHisMedicinePaty.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("frmHisMedicinePaty.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("frmHisMedicinePaty.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.ToolTip = Inventec.Common.Resource.Get.Value("frmHisMedicinePaty.gridColumn3.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn8.Caption = Inventec.Common.Resource.Get.Value("frmHisMedicinePaty.gridColumn8.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("frmHisMedicinePaty.gridColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreateTime.Caption = Inventec.Common.Resource.Get.Value("frmHisMedicinePaty.grdColCreateTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreateTime.ToolTip = Inventec.Common.Resource.Get.Value("frmHisMedicinePaty.grdColCreateTime.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreator.Caption = Inventec.Common.Resource.Get.Value("frmHisMedicinePaty.grdColCreator.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreator.ToolTip = Inventec.Common.Resource.Get.Value("frmHisMedicinePaty.grdColCreator.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifyTime.Caption = Inventec.Common.Resource.Get.Value("frmHisMedicinePaty.grdColModifyTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifyTime.ToolTip = Inventec.Common.Resource.Get.Value("frmHisMedicinePaty.grdColModifyTime.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifier.Caption = Inventec.Common.Resource.Get.Value("frmHisMedicinePaty.grdColModifier.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifier.ToolTip = Inventec.Common.Resource.Get.Value("frmHisMedicinePaty.grdColModifier.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyword.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmHisMedicinePaty.txtKeyword.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcEditorInfo.Text = Inventec.Common.Resource.Get.Value("frmHisMedicinePaty.lcEditorInfo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboMedicine.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisMedicinePaty.cboMedicine.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmHisMedicinePaty.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnSearch.Caption = Inventec.Common.Resource.Get.Value("frmHisMedicinePaty.bbtnSearch.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnEdit.Caption = Inventec.Common.Resource.Get.Value("frmHisMedicinePaty.bbtnEdit.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnAdd.Caption = Inventec.Common.Resource.Get.Value("frmHisMedicinePaty.bbtnAdd.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnReset.Caption = Inventec.Common.Resource.Get.Value("frmHisMedicinePaty.bbtnReset.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnFocusDefault.Caption = Inventec.Common.Resource.Get.Value("frmHisMedicinePaty.bbtnFocusDefault.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboPatientType.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisMedicinePaty.cboPatientType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnRefresh.Text = Inventec.Common.Resource.Get.Value("frmHisMedicinePaty.btnRefresh.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("frmHisMedicinePaty.btnAdd.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("frmHisMedicinePaty.btnEdit.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.dnNavigation.Text = Inventec.Common.Resource.Get.Value("frmHisMedicinePaty.dnNavigation.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciRatio.Text = Inventec.Common.Resource.Get.Value("frmHisMedicinePaty.lciRatio.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.lciExpireTimeTo.Text = Inventec.Common.Resource.Get.Value("frmHisMedicinePaty.lciExpireTimeTo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.lciExpireTimeFrom.Text = Inventec.Common.Resource.Get.Value("frmHisMedicinePaty.lciExpireTimeFrom.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.lciMaxFee.Text = Inventec.Common.Resource.Get.Value("frmHisMedicinePaty.lciMaxFee.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem11.Text = Inventec.Common.Resource.Get.Value("frmHisMedicinePaty.layoutControlItem11.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem13.Text = Inventec.Common.Resource.Get.Value("frmHisMedicinePaty.layoutControlItem12.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem10.Text = Inventec.Common.Resource.Get.Value("frmHisMedicinePaty.layoutControlItem10.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.TreatmentFrom.Text = Inventec.Common.Resource.Get.Value("frmHisMedicinePaty.TreatmentFrom.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                ///.TreatmentTo.Text = Inventec.Common.Resource.Get.Value("frmHisMedicinePaty.TreatmentTo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmHisMedicinePaty.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                if (this.moduleData != null && !String.IsNullOrEmpty(this.moduleData.text))
                {
                    this.Text = this.moduleData.text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDefaultMedicinePaty(MOS.EFMODEL.DataModels.V_HIS_MEDICINE medicinePaty)
        {
            try
            {
                if (medicinePaty != null)
                {
                    txtKeyword.Text = medicinePaty.MEDICINE_TYPE_CODE;
                    txtMedicineCode.Text = medicinePaty.MEDICINE_TYPE_CODE;
                    cboMedicine.EditValue = medicinePaty.ID;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private void SetDefaultValue()
        {
            try
            {
                this.ActionType = GlobalVariables.ActionAdd;
                //txtKeyword.Text = "";
                positionHandle = -1;
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderEditorInfo, dxErrorProvider);
                ResetFormData();
                spVATs.EditValue = 0;
                EnableControlChanged(this.ActionType);
                cboMedicine.Properties.DataSource = listMedicine;
                txtMedicineCode.Focus();
                txtMedicineCode.SelectAll();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Gan focus vao control mac dinh
        /// </summary>
        private void SetDefaultFocus()
        {
            try
            {
                txtKeyword.Focus();
                txtKeyword.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private void InitTabIndex()
        {
            try
            {



                if (dicOrderTabIndexControl != null)
                {
                    foreach (KeyValuePair<string, int> itemOrderTab in dicOrderTabIndexControl)
                    {
                        SetTabIndexToControl(itemOrderTab, lcEditorInfo);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool SetTabIndexToControl(KeyValuePair<string, int> itemOrderTab, DevExpress.XtraLayout.LayoutControl layoutControlEditor)
        {
            bool success = false;
            try
            {
                if (!layoutControlEditor.IsInitialized) return success;
                layoutControlEditor.BeginUpdate();
                try
                {
                    foreach (DevExpress.XtraLayout.BaseLayoutItem item in layoutControlEditor.Items)
                    {
                        DevExpress.XtraLayout.LayoutControlItem lci = item as DevExpress.XtraLayout.LayoutControlItem;
                        if (lci != null && lci.Control != null)
                        {
                            BaseEdit be = lci.Control as BaseEdit;
                            if (be != null)
                            {
                                //Cac control dac biet can fix khong co thay doi thuoc tinh enable
                                if (itemOrderTab.Key.Contains(be.Name))
                                {
                                    be.TabIndex = itemOrderTab.Value;
                                }
                            }
                        }
                    }
                }
                finally
                {
                    layoutControlEditor.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return success;
        }

        private void FillDataToControlsForm()
        {
            try
            {
                InitComboMedicine();
                InitComboPatientType();
                //TODO
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region Init combo

        private void InitComboMedicine()
        {
            try
            {

                CommonParam param = new CommonParam();
                HisMedicineViewFilter filter = new HisMedicineViewFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                listMedicine = new BackendAdapter(param).Get<List<V_HIS_MEDICINE>>("api/HisMedicine/GetView", ApiConsumers.MosConsumer, filter, null).ToList();

                cboMedicine.Properties.DataSource = listMedicine;
                cboMedicine.Properties.DisplayMember = "MEDICINE_TYPE_NAME";
                cboMedicine.Properties.ValueMember = "ID";
                cboMedicine.Properties.ForceInitialize();
                cboMedicine.Properties.Columns.Clear();
                cboMedicine.Properties.Columns.Add(new LookUpColumnInfo("MEDICINE_TYPE_CODE", "Mã", 50));
                cboMedicine.Properties.Columns.Add(new LookUpColumnInfo("MEDICINE_TYPE_NAME", "Tên", 150));
                cboMedicine.Properties.Columns.Add(new LookUpColumnInfo("BID_NAME", "Gói thầu", 100));
                cboMedicine.Properties.Columns.Add(new LookUpColumnInfo("BID_NUM_ORDER", "STT gói thầu", 100));
                cboMedicine.Properties.Columns.Add(new LookUpColumnInfo("PACKAGE_NUMBER", "Số lô", 50));
                cboMedicine.Properties.Columns.Add(new LookUpColumnInfo("EXPIRED_DATE", "HSD", 100));
                cboMedicine.Properties.Columns.Add(new LookUpColumnInfo("IMP_TIME", "Thời gian nhập", 100));
                cboMedicine.Properties.Columns.Add(new LookUpColumnInfo("IMP_PRICE", "Giá nhập", 100));
                cboMedicine.Properties.Columns.Add(new LookUpColumnInfo("IMP_VAT_RATIO", "VAT nhập", 70));
                cboMedicine.Properties.Columns.Add(new LookUpColumnInfo("INTERNAL_PRICE", "Giá nội bộ", 100));
                cboMedicine.Properties.ShowHeader = true;
                cboMedicine.Properties.ImmediatePopup = true;
                cboMedicine.Properties.DropDownRows = 10;
                cboMedicine.Properties.PopupWidth = 900;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboPatientType()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisPatientTypeFilter filter = new HisPatientTypeFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                var data = new BackendAdapter(param).Get<List<HIS_PATIENT_TYPE>>("api/HisPatientType/Get", ApiConsumers.MosConsumer, filter, null).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PATIENT_TYPE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("PATIENT_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PATIENT_TYPE_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboPatientType, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        /// <summary>
        /// Ham lay du lieu theo dieu kien tim kiem va gan du lieu vao danh sach
        /// </summary>
        public void FillDataToGridControl()
        {
            try
            {
                WaitingManager.Show();
                int numPageSize = 0;
                if (ucPaging.pagingGrid != null)
                {
                    numPageSize = ucPaging.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }
                LoadPaging(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging.Init(LoadPaging, param, numPageSize, this.gridControlFormList);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        /// <summary>
        /// Ham goi api lay du lieu phan trang
        /// </summary>
        /// <param name="param"></param>
        private void LoadPaging(object param)
        {
            try
            {
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);
                Inventec.Core.ApiResultObject<List<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_PATY>> apiResult = null;
                HisMedicinePatyViewFilter filter = new HisMedicinePatyViewFilter();
                SetFilterNavBar(ref filter);
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                dnNavigation.DataSource = null;
                gridviewFormList.BeginUpdate();
                apiResult = new BackendAdapter(paramCommon).GetRO<List<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_PATY>>(HisRequestUriStore.HIS_MEDICINE_PATY_GETVIEW, ApiConsumers.MosConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    var data = (List<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_PATY>)apiResult.Data;
                    if (data != null)
                    {
                        dnNavigation.DataSource = data;
                        gridviewFormList.GridControl.DataSource = data;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                }
                gridviewFormList.EndUpdate();

                #region Process has exception
                SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
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

        private void SetFilterNavBar(ref HisMedicinePatyViewFilter filter)
        {
            try
            {

                filter.KEY_WORD = txtKeyword.Text.Trim();
                if (this.currentMedicine != null)
                {
                    filter.MEDICINE_ID = this.currentMedicine.ID;
                }
                //if (!String.IsNullOrEmpty(txtSearchCode.Text))
                //{
                //    string code = txtSearchCode.Text.Trim();
                //    if (code.Length < 12 && checkDigit(code))
                //    {
                //        code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                //        txtSearchCode.Text = code;
                //    }
                //    filter. = code;
                //}
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void txtKeyWord_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch_Click(null, null);
                }
                else if (e.KeyCode == Keys.Down)
                {
                    gridviewFormList.Focus();
                    gridviewFormList.FocusedRowHandle = 0;
                    var rowData = (MOS.EFMODEL.DataModels.V_HIS_MEDICINE_PATY)gridviewFormList.GetFocusedRow();
                    if (rowData != null)
                    {
                        ChangedDataRow(rowData);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtKeyword_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch_Click(null, null);
                }
                else if (e.KeyCode == Keys.Down)
                {
                    gridviewFormList.Focus();
                    gridviewFormList.FocusedRowHandle = 0;
                    var rowData = (MOS.EFMODEL.DataModels.V_HIS_MEDICINE_PATY)gridviewFormList.GetFocusedRow();
                    if (rowData != null)
                    {
                        ChangedDataRow(rowData);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridviewFormList_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MOS.EFMODEL.DataModels.V_HIS_MEDICINE_PATY pData = (MOS.EFMODEL.DataModels.V_HIS_MEDICINE_PATY)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    short status = Inventec.Common.TypeConvert.Parse.ToInt16((pData.IS_ACTIVE ?? -1).ToString());
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + startPage; //+ ((pagingGrid.CurrentPage - 1) * pagingGrid.PageSize);
                    }
                    else if (e.Column.FieldName == "CREATE_TIME_STR")
                    {
                        try
                        {
                            string createTime = (view.GetRowCellValue(e.ListSourceRowIndex, "CREATE_TIME") ?? "").ToString();
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Inventec.Common.TypeConvert.Parse.ToInt64(createTime));

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao CREATE_TIME", ex);
                        }
                    }
                    else if (e.Column.FieldName == "MODIFY_TIME_STR")
                    {
                        try
                        {
                            string MODIFY_TIME = (view.GetRowCellValue(e.ListSourceRowIndex, "MODIFY_TIME") ?? "").ToString();
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Inventec.Common.TypeConvert.Parse.ToInt64(MODIFY_TIME));

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao MODIFY_TIME", ex);
                        }
                    }

                    else if (e.Column.FieldName == "FROM_TIME_STR")
                    {
                        try
                        {
                            string FROM_TIME = (view.GetRowCellValue(e.ListSourceRowIndex, "FROM_TIME") ?? "").ToString();
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Inventec.Common.TypeConvert.Parse.ToInt64(FROM_TIME));

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao FROM_TIME_STR", ex);
                        }
                    }
                    else if (e.Column.FieldName == "TO_TIME_STR")
                    {
                        try
                        {
                            string TO_TIME = (view.GetRowCellValue(e.ListSourceRowIndex, "TO_TIME") ?? "").ToString();
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Inventec.Common.TypeConvert.Parse.ToInt64(TO_TIME));

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao TO_TIME", ex);
                        }
                    }
                    else if (e.Column.FieldName == "EXPIRED_DATE_STR")
                    {
                        try
                        {
                            string EXPIRED_DATE = (view.GetRowCellValue(e.ListSourceRowIndex, "EXPIRED_DATE") ?? "").ToString();
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Inventec.Common.TypeConvert.Parse.ToInt64(EXPIRED_DATE));

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao EXPIRED_DATE", ex);
                        }
                    }

                    else if (e.Column.FieldName == "IMP_TIME_STR")
                    {
                        try
                        {
                            string IMP_TIME = (view.GetRowCellValue(e.ListSourceRowIndex, "IMP_TIME") ?? "").ToString();
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Inventec.Common.TypeConvert.Parse.ToInt64(IMP_TIME));

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao IMP_TIME", ex);
                        }
                    }
                    else if (e.Column.FieldName == "TREATMENT_FROM_TIME_STR")
                    {
                        try
                        {
                            string TREATMENT_FROM_TIME = (view.GetRowCellValue(e.ListSourceRowIndex, "TREATMENT_FROM_TIME") ?? "").ToString();
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Inventec.Common.TypeConvert.Parse.ToInt64(TREATMENT_FROM_TIME));

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao TREATMENT_FROM_TIME", ex);
                        }
                    }
                    if (e.Column.FieldName == "PRICE_AFTER_VAT")

                        e.Value = getPriceAfterVat(view, e.ListSourceRowIndex);
                    else if (e.Column.FieldName == "TREATMENT_TO_TIME_STR")
                    {
                        try
                        {
                            string TREATMENT_TO_TIME = (view.GetRowCellValue(e.ListSourceRowIndex, "TREATMENT_TO_TIME") ?? "").ToString();
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Inventec.Common.TypeConvert.Parse.ToInt64(TREATMENT_TO_TIME));

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao TREATMENT_TO_TIME", ex);
                        }
                    }

                    else if (e.Column.FieldName == "EXP_VAT_RATIO_STR")
                    {
                        try
                        {
                            decimal EXP_VAT_RATIO = Inventec.Common.TypeConvert.Parse.ToDecimal((view.GetRowCellValue(e.ListSourceRowIndex, "EXP_VAT_RATIO") ?? "").ToString());
                            e.Value = (EXP_VAT_RATIO * 100).ToString();

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao EXP_VAT_RATIO", ex);
                        }
                    }

                    else if (e.Column.FieldName == "IMP_VAT_RATIO_STR")
                    {
                        try
                        {
                            decimal IMP_VAT_RATIO = Inventec.Common.TypeConvert.Parse.ToDecimal((view.GetRowCellValue(e.ListSourceRowIndex, "IMP_VAT_RATIO") ?? "").ToString());
                            e.Value = (IMP_VAT_RATIO * 100).ToString();

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao IMP_VAT_RATIO", ex);
                        }
                    }

                    else if (e.Column.FieldName == "IS_ACTIVE_STR")
                    {
                        try
                        {
                            if (status == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                                e.Value = "Hoạt động";
                            else
                                e.Value = "Tạm khóa";
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }

                    gridControlFormList.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridControlFormList_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                var rowData = (MOS.EFMODEL.DataModels.V_HIS_MEDICINE_PATY)gridviewFormList.GetFocusedRow();
                if (rowData != null)
                {
                    currentData = rowData;
                    ChangedDataRow(rowData);

                    //Set focus vào control editor đầu tiên
                    SetFocusEditor();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridviewFormList_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    var rowData = (MOS.EFMODEL.DataModels.V_HIS_MEDICINE_PATY)gridviewFormList.GetFocusedRow();
                    if (rowData != null)
                    {
                        ChangedDataRow(rowData);

                        //Set focus vào control editor đầu tiên
                        SetFocusEditor();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dnNavigation_PositionChanged(object sender, EventArgs e)
        {
            try
            {
                this.currentData = (MOS.EFMODEL.DataModels.V_HIS_MEDICINE_PATY)(gridControlFormList.DataSource as List<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_PATY>)[dnNavigation.Position];
                if (this.currentData != null)
                {
                    ChangedDataRow(this.currentData);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ChangedDataRow(MOS.EFMODEL.DataModels.V_HIS_MEDICINE_PATY data)
        {
            try
            {
                if (data != null)
                {
                    FillDataToEditorControl(data);
                    this.ActionType = GlobalVariables.ActionEdit;
                    EnableControlChanged(this.ActionType);

                    //Disable nút sửa nếu dữ liệu đã bị khóa

                    positionHandle = -1;
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderEditorInfo, dxErrorProvider);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToEditorControl(MOS.EFMODEL.DataModels.V_HIS_MEDICINE_PATY data)
        {
            try
            {
                if (data != null)
                {
                    cboMedicine.EditValue = (long)data.MEDICINE_ID;
                    txtMedicineCode.Text = data.MEDICINE_TYPE_CODE;
                    cboPatientType.EditValue = (long)data.PATIENT_TYPE_ID;
                    if (data.EXP_PRICE != null)
                        spPrice.EditValue = (decimal)data.EXP_PRICE;
                    else
                        spPrice.EditValue = null;
                    if (data.EXP_VAT_RATIO != null)
                        spVATs.EditValue = (decimal)data.EXP_VAT_RATIO * 100;
                    else
                        spVATs.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Gan focus vao control mac dinh
        /// </summary>
        private void SetFocusEditor()
        {
            try
            {
                //TODO

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private void ResetFormData()
        {
            try
            {
                if (!lcEditorInfo.IsInitialized) return;
                lcEditorInfo.BeginUpdate();
                try
                {
                    foreach (DevExpress.XtraLayout.BaseLayoutItem item in lcEditorInfo.Items)
                    {
                        DevExpress.XtraLayout.LayoutControlItem lci = item as DevExpress.XtraLayout.LayoutControlItem;
                        if (lci != null && lci.Control != null && lci.Control is BaseEdit)
                        {
                            DevExpress.XtraEditors.BaseEdit fomatFrm = lci.Control as DevExpress.XtraEditors.BaseEdit;
                            fomatFrm.ResetText();
                            fomatFrm.EditValue = null;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                finally
                {
                    lcEditorInfo.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadCurrent(long currentId, ref MOS.EFMODEL.DataModels.HIS_MEDICINE_PATY currentDTO)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisMedicinePatyFilter filter = new HisMedicinePatyFilter();
                filter.ID = currentId;
                currentDTO = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_MEDICINE_PATY>>(HisRequestUriStore.HIS_MEDICINE_PATY_GET, ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void EnableControlChanged(int action)
        {
            try
            {
                btnEdit.Enabled = (action == GlobalVariables.ActionEdit);
                btnAdd.Enabled = (action == GlobalVariables.ActionAdd);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dxValidationProvider_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandle == -1)
                {
                    positionHandle = edit.TabIndex;
                    edit.SelectAll();
                    edit.Focus();
                }
                if (positionHandle > edit.TabIndex)
                {
                    positionHandle = edit.TabIndex;
                    edit.SelectAll();
                    edit.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region Button handler
        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataToGridControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnRefesh_Click(object sender, EventArgs e)
        {
            try
            {
                SetDefaultValue();
                //FillDataToGridControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                SetDefaultValue();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                SaveProcess();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                SaveProcess();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SaveProcess()
        {
            CommonParam param = new CommonParam();
            try
            {
                bool success = false;
                if (!btnEdit.Enabled && !btnAdd.Enabled)
                    return;

                positionHandle = -1;
                if (!dxValidationProviderEditorInfo.Validate())
                    return;

                WaitingManager.Show();
                MOS.EFMODEL.DataModels.HIS_MEDICINE_PATY updateDTO = new MOS.EFMODEL.DataModels.HIS_MEDICINE_PATY();

                if (this.currentData != null && this.currentData.ID > 0)
                {
                    if (this.ActionType == GlobalVariables.ActionEdit)
                    {
                        LoadCurrent(this.currentData.ID, ref updateDTO);
                    }
                }
                UpdateDTOFromDataForm(ref updateDTO);

                if (ActionType == GlobalVariables.ActionAdd)
                {
                    updateDTO.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;

                    var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_MEDICINE_PATY>(HisRequestUriStore.HIS_MEDICINE_PATY_CREATE, ApiConsumers.MosConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        BackendDataWorker.Reset<HIS_MEDICINE_PATY>();
                        success = true;
                        FillDataToGridControl();
                        SetDefaultValue();
                    }
                }
                else
                {

                    var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_MEDICINE_PATY>(HisRequestUriStore.HIS_MEDICINE_PATY_UPDATE, ApiConsumers.MosConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;
                        FillDataToGridControl();
                        //UpdateRowDataAfterEdit(resultData);
                    }
                    if (success)
                    {
                        BackendDataWorker.Reset<HIS_MEDICINE_PATY>();
                        SetFocusEditor();
                    }
                }

                WaitingManager.Hide();

                #region Hien thi message thong bao
                MessageManager.Show(this, param, success);
                #endregion

                #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                SessionManager.ProcessTokenLost(param);
                #endregion


            }

            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateRowDataAfterEdit(MOS.EFMODEL.DataModels.HIS_MEDICINE_PATY data)
        {
            try
            {
                if (data == null)
                    throw new ArgumentNullException("data(MOS.EFMODEL.DataModels.V_HIS_MEDICINE_PATY) is null");
                var rowData = (MOS.EFMODEL.DataModels.V_HIS_MEDICINE_PATY)gridviewFormList.GetFocusedRow();
                if (rowData != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_PATY>(rowData, data);
                    gridviewFormList.RefreshRow(gridviewFormList.FocusedRowHandle);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateDTOFromDataForm(ref MOS.EFMODEL.DataModels.HIS_MEDICINE_PATY currentDTO)
        {
            try
            {
                if (cboMedicine.EditValue != null)
                {
                    currentDTO.MEDICINE_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboMedicine.EditValue ?? "0").ToString());
                }
                if (cboPatientType.EditValue != null)
                {
                    currentDTO.PATIENT_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboPatientType.EditValue ?? "0").ToString());
                }
                currentDTO.EXP_VAT_RATIO = (decimal)spVATs.Value / 100;
                currentDTO.EXP_PRICE = (decimal)spPrice.Value;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Validate
        private void ValidateForm()
        {
            try
            {
                ValidationSingleControl(cboPatientType);
                ValidateLookupWithTextEdit(cboMedicine, txtMedicineCode);
                ValidationSingleControl1(spVATs);
                ValidationSingleControl2(spPrice);
                //ValidationSingleControl3(spPriority);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateLookupWithTextEdit(LookUpEdit cbo, TextEdit textEdit)
        {
            try
            {
                LookupEditWithTextEditValidationRule validRule = new LookupEditWithTextEditValidationRule();
                validRule.txtTextEdit = textEdit;
                validRule.cbo = cbo;
                validRule.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditorInfo.SetValidationRule(textEdit, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateGridLookupWithTextEdit(GridLookUpEdit cbo, TextEdit textEdit)
        {
            try
            {
                GridLookupEditWithTextEditValidationRule validRule = new GridLookupEditWithTextEditValidationRule();
                validRule.txtTextEdit = textEdit;
                validRule.cbo = cbo;
                validRule.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditorInfo.SetValidationRule(textEdit, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationSingleControl(BaseEdit control)
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = control;
                validRule.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditorInfo.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationSingleControl1(SpinEdit control)
        {
            try
            {
                ValidateSpin1 validate = new ValidateSpin1();
                validate.spin = control;
                //validate.ErrorText = MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBTruongDuLieuBatBuocPhaiNhap);
                validate.ErrorType = ErrorType.Warning;
                this.dxValidationProviderEditorInfo.SetValidationRule(control, validate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationSingleControl2(SpinEdit control)
        {
            try
            {
                ValidateSpin2 validate = new ValidateSpin2();
                validate.spin = control;
                //validate.ErrorText = MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBTruongDuLieuBatBuocPhaiNhap);
                validate.ErrorType = ErrorType.Warning;
                this.dxValidationProviderEditorInfo.SetValidationRule(control, validate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationSingleControl3(SpinEdit control)
        {
            try
            {
                ValidateSpin3 validate = new ValidateSpin3();
                validate.spin = control;
                //validate.ErrorText = MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBTruongDuLieuBatBuocPhaiNhap);
                validate.ErrorType = ErrorType.Warning;
                this.dxValidationProviderEditorInfo.SetValidationRule(control, validate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Tooltip
        private void toolTipControllerGrid_GetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {
                //TODO

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #endregion

        #region Public method
        public void MeShow()
        {
            try
            {
                //Gan gia tri mac dinh
                SetDefaultValue();
                //Set enable control default
                EnableControlChanged(this.ActionType);

                //Fill data into datasource combo
                FillDataToControlsForm();

                //Load ngon ngu label control
                SetCaptionByLanguageKey();

                //Set tabindex control
                InitTabIndex();

                //Set validate rule
                ValidateForm();

                //Focus default
                SetDefaultFocus();
                SetDefaultMedicinePaty(this.currentMedicine);

                //Load du lieu
                FillDataToGridControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Shortcut
        private void bbtnSearch_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSearch_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (this.ActionType == GlobalVariables.ActionEdit && btnEdit.Enabled)
                {
                    btnEdit_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (this.ActionType == GlobalVariables.ActionAdd && btnAdd.Enabled)
                    btnAdd_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnReset_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnCancel_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnFocusDefault_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                txtKeyword.Focus();
                txtKeyword.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        private void gridviewFormList_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    V_HIS_MEDICINE_PATY data = (V_HIS_MEDICINE_PATY)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (e.Column.FieldName == "IS_ACTIVE_STR")
                    {
                        if (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE)
                            e.Appearance.ForeColor = Color.Red;
                        else
                            e.Appearance.ForeColor = Color.Green;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void gridviewFormList_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {

                    V_HIS_MEDICINE_PATY data = (V_HIS_MEDICINE_PATY)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (e.Column.FieldName == "IS_LOCK")
                    {
                        e.RepositoryItem = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? unLock : Lock);

                    }

                    if (e.Column.FieldName == "Delete")
                    {
                        e.RepositoryItem = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? btnGEdit : Delete);

                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Lock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            HIS_MEDICINE_PATY success = new HIS_MEDICINE_PATY();
            //bool notHandler = false;
            try
            {

                V_HIS_MEDICINE_PATY data = (V_HIS_MEDICINE_PATY)gridviewFormList.GetFocusedRow();
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    HIS_MEDICINE_PATY data1 = new HIS_MEDICINE_PATY();
                    data1.ID = data.ID;
                    WaitingManager.Show();
                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_MEDICINE_PATY>(HisRequestUriStore.HIS_MEDICINE_PATY_CHANGELOCK, ApiConsumers.MosConsumer, data1, param);
                    WaitingManager.Hide();
                    if (success != null)
                    {
                        BackendDataWorker.Reset<HIS_MEDICINE_PATY>();
                        FillDataToGridControl();
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void unLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            HIS_MEDICINE_PATY success = new HIS_MEDICINE_PATY();
            //bool notHandler = false;
            try
            {

                V_HIS_MEDICINE_PATY data = (V_HIS_MEDICINE_PATY)gridviewFormList.GetFocusedRow();
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    HIS_MEDICINE_PATY data1 = new HIS_MEDICINE_PATY();
                    data1.ID = data.ID;
                    WaitingManager.Show();
                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_MEDICINE_PATY>(HisRequestUriStore.HIS_MEDICINE_PATY_CHANGELOCK, ApiConsumers.MosConsumer, data1, param);
                    WaitingManager.Hide();
                    if (success != null)
                    {
                        BackendDataWorker.Reset<HIS_MEDICINE_PATY>();
                        FillDataToGridControl();

                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnGEdit_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    CommonParam param = new CommonParam();
                    var rowData = (MOS.EFMODEL.DataModels.V_HIS_MEDICINE_PATY)gridviewFormList.GetFocusedRow();
                    HisMedicinePatyFilter filter = new HisMedicinePatyFilter();
                    filter.ID = rowData.ID;
                    var data = new BackendAdapter(param).Get<List<HIS_MEDICINE_PATY>>(HisRequestUriStore.HIS_MEDICINE_PATY_GET, ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
                    if (rowData != null)
                    {
                        bool success = false;
                        success = new BackendAdapter(param).Post<bool>(HisRequestUriStore.HIS_MEDICINE_PATY_DELETE, ApiConsumers.MosConsumer, data, param);
                        if (success)
                        {
                            BackendDataWorker.Reset<HIS_MEDICINE_PATY>();
                            FillDataToGridControl();
                            currentData = ((List<V_HIS_MEDICINE_PATY>)gridControlFormList.DataSource).FirstOrDefault();
                        }
                        MessageManager.Show(this, param, success);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtTreatmentTo_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void cboMedicine_KeyUp(object sender, KeyEventArgs e)
        {
            //try
            //{
            //    if (e.KeyCode == Keys.Enter)
            //    {
            //        if (cboMedicine.EditValue != null)
            //        {
            //            var data = listMedicine.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboMedicine.EditValue ?? "").ToString()));
            //            if (data != null)
            //            {
            //                txtMedicineCode.Text = data.MEDICINE_TYPE_CODE;
            //            }
            //            cboPatientType.Focus();
            //            cboPatientType.ShowPopup();
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Warn(ex);
            //}
        }

        private void cboPatientType_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboPatientType.EditValue == null)
                    {
                        cboPatientType.ShowPopup();
                    }
                    else
                    {
                        spPrice.Focus();
                        spPrice.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spPrice_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spVATs.Focus();
                    spVATs.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spVATs_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.ActionType == GlobalVariables.ActionAdd)
                    {
                        btnAdd.Focus();
                    }
                    else
                        btnEdit.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtSearchCode_KeyUp(object sender, KeyEventArgs e)
        {
            //try
            //{
            //    if (e.KeyCode == Keys.Enter)
            //    {
            //        if (!String.IsNullOrEmpty(txtSearchCode.Text))
            //        {
            //            FillDataToGridControl();
            //            txtSearchCode.SelectAll();
            //        }
            //        else
            //        {
            //            txtKeyword.Focus();
            //            txtKeyword.SelectAll();
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Error(ex);
            //}
        }

        private void txtMedicineCode_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!string.IsNullOrEmpty(txtMedicineCode.Text))
                    {
                        var data = listMedicine.Where(o => o.MEDICINE_TYPE_CODE.ToUpper().Contains(txtMedicineCode.Text.ToUpper())).ToList();
                        if (data != null)
                        {
                            if (data.Count == 1)
                            {
                                cboMedicine.EditValue = data[0].ID;
                                txtMedicineCode.Text = data[0].MEDICINE_TYPE_CODE;
                                cboPatientType.Focus();
                                cboPatientType.ShowPopup();
                                cboMedicine.Properties.Buttons[1].Visible = true;
                            }
                            else if (data.Count > 1)
                            {
                                cboMedicine.Properties.DataSource = data;
                                cboMedicine.EditValue = null;
                                cboMedicine.Focus();
                                cboMedicine.ShowPopup();
                                //PopupProcess.SelectFirstRowPopup(control.cboDepartment);
                            }
                        }
                    }
                    else
                    {
                        cboMedicine.Focus();
                        cboMedicine.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void cboMedicine_KeyUp_1(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboMedicine.EditValue != null)
                    {
                        var data = listMedicine.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboMedicine.EditValue ?? "").ToString()));
                        if (data != null)
                        {
                            txtMedicineCode.Text = data.MEDICINE_TYPE_CODE;
                            cboPatientType.Focus();
                            cboPatientType.ShowPopup();
                        }
                    }
                }

                //if (cboMedicine.EditValue != null)
                //{
                //    txtMedicineCode.Text = listMedicine.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboMedicine.EditValue ?? 0).ToString())).MEDICINE_TYPE_CODE;
                //    cboPatientType.Focus();
                //    cboPatientType.ShowPopup();
                //}
                //else
                //{
                //    cboMedicine.Focus();
                //    cboMedicine.ShowPopup();

                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMedicine_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (cboMedicine.EditValue != null)
                {
                    var data = listMedicine.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboMedicine.EditValue ?? "").ToString()));
                    if (data != null)
                    {
                        cboMedicine.Properties.DataSource = listMedicine;
                        txtMedicineCode.Text = data.MEDICINE_TYPE_CODE;
                        cboPatientType.Focus();
                        cboPatientType.SelectAll();

                    }
                }

                //if (cboMedicine.EditValue != null)
                //{
                //    txtMedicineCode.Text = listMedicine.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboMedicine.EditValue ?? 0).ToString())).MEDICINE_TYPE_CODE;
                //    cboPatientType.Focus();
                //    cboPatientType.ShowPopup();
                //}
                //else
                //{
                //    cboMedicine.Focus();
                //    cboMedicine.ShowPopup();

                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            try
            {
                //WaitingManager.Show();
                //OpenFileDialog ofd = new OpenFileDialog();
                //ofd.Multiselect = false;
                //if (ofd.ShowDialog() == DialogResult.OK)
                //{
                //    ImportADO importAdo = new ImportADO();

                //    WaitingManager.Show();
                //    var import = new Inventec.Common.ExcelImport.Import();
                //    if (import.ReadFileExcel(ofd.FileName))
                //    {
                //        var hisMedicinePatyImport = import.GetWithCheck<MedicinePatyImportADO>(0);
                //        if (hisMedicinePatyImport != null && hisMedicinePatyImport.Count > 0)
                //        {
                //            foreach (var item in hisMedicinePatyImport)
                //            {
                //                bool checkNull =
                //                    string.IsNullOrEmpty(item.MEDICINE_TYPE_CODE)
                //                    && string.IsNullOrEmpty(item.PATIENT_TYPE_CODE)
                //                    && (item.EXP_PRICE == null)
                //                    && (item.EXP_VAT_RATIO == null);
                //                if (checkNull)
                //                {
                //                    hisMedicinePatyImport.Remove(item);
                //                }
                //            }

                //            importAdo.medicinePatyAdos = hisMedicinePatyImport;

                //List<object> listArgs = new List<object>();
                ////listArgs.Add(importAdo);
                ////listArgs.Add((RefeshReference)ImportReload);
                //CallModule callModule = new CallModule(CallModule.Import, this.currentModuleBase.RoomId, this.currentModuleBase.RoomTypeId, listArgs);
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == CallModule.Import).FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.HisIcdImport");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, 0, 0), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).ShowDialog();
                }

                // WaitingManager.Hide();
                //        }
                //        else
                //        {
                //            WaitingManager.Hide();
                //            DevExpress.XtraEditors.XtraMessageBox.Show("Import thất bại");
                //        }
                //    }
                //    else
                //    {
                //        WaitingManager.Hide();
                //        DevExpress.XtraEditors.XtraMessageBox.Show("Không đọc được file");
                //    }
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void ImportReload()
        {
            try
            {
                FillDataToGridControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //private void cboMedicine_Closed(object sender, ClosedEventArgs e)
        //{
        //    try
        //    {
        //        if (e.CloseMode == PopupCloseMode.Normal)
        //        {
        //            if (cboMedicine.EditValue != null)
        //            {
        //                var data = listMedicine.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboMedicine.EditValue ?? "").ToString()));
        //                if (data != null)
        //                {
        //                    txtMedicineCode.Text = data.MEDICINE_TYPE_CODE;
        //                    cboPatientType.Focus();
        //                    cboPatientType.ShowPopup();
        //                }
        //            }
        //            else
        //            {
        //                cboMedicine.Focus();
        //                cboMedicine.ShowPopup();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }

        //}
        decimal getPriceAfterVat(GridView view, int listSourceRowIndex)
        {
            decimal EXP_VAT_RATIO = Inventec.Common.TypeConvert.Parse.ToDecimal((view.GetRowCellValue(listSourceRowIndex, "EXP_VAT_RATIO") ?? "").ToString());
            decimal EXP_PRICE = Inventec.Common.TypeConvert.Parse.ToDecimal((view.GetRowCellValue(listSourceRowIndex, "EXP_PRICE") ?? "").ToString());
            return EXP_PRICE * (1 + EXP_VAT_RATIO);

        }

    }
}
