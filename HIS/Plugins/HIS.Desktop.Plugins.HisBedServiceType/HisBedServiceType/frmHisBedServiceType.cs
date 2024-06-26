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
using MOS.SDO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using Inventec.Desktop.Common.Controls.ValidationRule;
using DevExpress.XtraEditors.DXErrorProvider;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using DevExpress.XtraEditors.Controls;

namespace HIS.Desktop.Plugins.HisBedServiceType
{
    public partial class frmHisBedServiceType : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        PagingGrid pagingGrid;
        int ActionType = -1;
        int positionHandle = -1;
        MOS.EFMODEL.DataModels.V_HIS_BED_SERVICE_TYPE currentData;
        List<string> arrControlEnableNotChange = new List<string>();
        Dictionary<string, int> dicOrderTabIndexControl = new Dictionary<string, int>();
        Inventec.Desktop.Common.Modules.Module moduleData;
        #endregion

        #region Construct
        public frmHisBedServiceType(Inventec.Desktop.Common.Modules.Module moduleData)
		:base(moduleData)
        {
            try
            {
                InitializeComponent();

                pagingGrid = new PagingGrid();
                this.moduleData = moduleData;
                //gridControlFormList.ToolTipController = toolTipControllerGrid;

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
        #endregion

        #region Private method
        private void frmHisBedServiceType_Load(object sender, EventArgs e)
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
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisBedServiceType.Resources.Lang", typeof(HIS.Desktop.Plugins.HisBedServiceType.frmHisBedServiceType).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("frmHisBedServiceType.layoutControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("frmHisBedServiceType.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn1.Caption = Inventec.Common.Resource.Get.Value("frmHisBedServiceType.treeListColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn1.ToolTip = Inventec.Common.Resource.Get.Value("frmHisBedServiceType.treeListColumn1.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn2.Caption = Inventec.Common.Resource.Get.Value("frmHisBedServiceType.treeListColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn2.ToolTip = Inventec.Common.Resource.Get.Value("frmHisBedServiceType.treeListColumn2.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn3.Caption = Inventec.Common.Resource.Get.Value("frmHisBedServiceType.treeListColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn3.ToolTip = Inventec.Common.Resource.Get.Value("frmHisBedServiceType.treeListColumn3.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn4.Caption = Inventec.Common.Resource.Get.Value("frmHisBedServiceType.treeListColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn5.Caption = Inventec.Common.Resource.Get.Value("frmHisBedServiceType.treeListColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn6.Caption = Inventec.Common.Resource.Get.Value("frmHisBedServiceType.treeListColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn7.Caption = Inventec.Common.Resource.Get.Value("frmHisBedServiceType.treeListColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn8.Caption = Inventec.Common.Resource.Get.Value("frmHisBedServiceType.treeListColumn8.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn8.ToolTip = Inventec.Common.Resource.Get.Value("frmHisBedServiceType.treeListColumn8.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn9.Caption = Inventec.Common.Resource.Get.Value("frmHisBedServiceType.treeListColumn9.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn9.ToolTip = Inventec.Common.Resource.Get.Value("frmHisBedServiceType.treeListColumn9.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn10.Caption = Inventec.Common.Resource.Get.Value("frmHisBedServiceType.treeListColumn10.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn10.ToolTip = Inventec.Common.Resource.Get.Value("frmHisBedServiceType.treeListColumn10.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn11.Caption = Inventec.Common.Resource.Get.Value("frmHisBedServiceType.treeListColumn11.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn11.ToolTip = Inventec.Common.Resource.Get.Value("frmHisBedServiceType.treeListColumn11.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn12.Caption = Inventec.Common.Resource.Get.Value("frmHisBedServiceType.treeListColumn12.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn12.ToolTip = Inventec.Common.Resource.Get.Value("frmHisBedServiceType.treeListColumn12.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn13.Caption = Inventec.Common.Resource.Get.Value("frmHisBedServiceType.treeListColumn13.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn13.ToolTip = Inventec.Common.Resource.Get.Value("frmHisBedServiceType.treeListColumn13.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn14.Caption = Inventec.Common.Resource.Get.Value("frmHisBedServiceType.treeListColumn14.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn14.ToolTip = Inventec.Common.Resource.Get.Value("frmHisBedServiceType.treeListColumn14.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn15.Caption = Inventec.Common.Resource.Get.Value("frmHisBedServiceType.treeListColumn15", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn15.ToolTip = Inventec.Common.Resource.Get.Value("frmHisBedServiceType.treeListColumn15", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn16.Caption = Inventec.Common.Resource.Get.Value("frmHisTestServiceType.treeListColumn16", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn16.ToolTip = Inventec.Common.Resource.Get.Value("frmHisTestServiceType.treeListColumn16", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem1.Text = Inventec.Common.Resource.Get.Value("frmHisTestServiceType.layout1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("frmHisBedServiceType.treeListColumn15.Layout", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyword.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmHisBedServiceType.txtKeyword.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcEditorInfo.Text = Inventec.Common.Resource.Get.Value("frmHisBedServiceType.lcEditorInfo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboBhyt.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisBedServiceType.cboBhyt.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmHisBedServiceType.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnSearch.Caption = Inventec.Common.Resource.Get.Value("frmHisBedServiceType.bbtnSearch.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnEdit.Caption = Inventec.Common.Resource.Get.Value("frmHisBedServiceType.bbtnEdit.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnAdd.Caption = Inventec.Common.Resource.Get.Value("frmHisBedServiceType.bbtnAdd.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnReset.Caption = Inventec.Common.Resource.Get.Value("frmHisBedServiceType.bbtnReset.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnFocusDefault.Caption = Inventec.Common.Resource.Get.Value("frmHisBedServiceType.bbtnFocusDefault.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboType.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisBedServiceType.cboType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboDVT.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisBedServiceType.cboDVT.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboParent.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisBedServiceType.cboParent.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("frmHisBedServiceType.btnAdd.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnRefresh.Text = Inventec.Common.Resource.Get.Value("frmHisBedServiceType.btnRefresh.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("frmHisBedServiceType.btnEdit.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.dnNavigation.Text = Inventec.Common.Resource.Get.Value("frmHisBedServiceType.dnNavigation.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciTypeCode.Text = Inventec.Common.Resource.Get.Value("frmHisBedServiceType.lciTypeCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciTypeName.Text = Inventec.Common.Resource.Get.Value("frmHisBedServiceType.lciTypeName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciNumOrder.Text = Inventec.Common.Resource.Get.Value("frmHisBedServiceType.lciNumOrder.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem12.Text = Inventec.Common.Resource.Get.Value("frmHisBedServiceType.layoutControlItem12.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem14.Text = Inventec.Common.Resource.Get.Value("frmHisBedServiceType.layoutControlItem14.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem15.Text = Inventec.Common.Resource.Get.Value("frmHisBedServiceType.layoutControlItem15.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem13.Text = Inventec.Common.Resource.Get.Value("frmHisBedServiceType.layoutControlItem11.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.spCogss.Text = Inventec.Common.Resource.Get.Value("frmHisBedServiceType.spCogss.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.spEstimateDurations.Text = Inventec.Common.Resource.Get.Value("frmHisBedServiceType.spEstimateDurations.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmHisBedServiceType.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                if (this.moduleData != null && !String.IsNullOrEmpty(this.moduleData.text))
                {
                    this.Text = this.moduleData.text;
                }

                //Chạy tool sinh resource key language sinh tự động đa ngôn ngữ -> copy vào đây
                //TODO
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDefaultValue()
        {
            try
            {
                this.ActionType = GlobalVariables.ActionAdd;
                txtKeyword.Text = "";
                txtBedServiceTypeCode.EditValue = null;
                txtBedServiceTypeName.EditValue = null;
                txtBedServiceTypeCode.Focus();
                txtBedServiceTypeCode.SelectAll();
                txtKeyword.EditValue = null;
                spCogs.EditValue = null;
                spEstimateDuration.EditValue = null;
                spNumOrder.EditValue = null;
                cboBhyt.EditValue = null;
                cboDVT.EditValue = null;
                cboType.EditValue = null;
                cboParent.EditValue = null;
                cboBillOption.EditValue = null;
                cboBillOption.EditValue = null;
                cboPatientType.EditValue = null;
                chkCPNgoaiGoi.Checked = false;
                positionHandle = -1;
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderEditorInfo, dxErrorProvider);
                //ResetFormData();
                EnableControlChanged(this.ActionType);

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
                dicOrderTabIndexControl.Add("txtBedServiceTypeCode", 0);
                dicOrderTabIndexControl.Add("txtBedServiceTypeName", 1);
                dicOrderTabIndexControl.Add("lkServiceId", 2);
                dicOrderTabIndexControl.Add("lkParentId", 3);
                dicOrderTabIndexControl.Add("chkIsLeaf", 4);
                dicOrderTabIndexControl.Add("spNumOrder", 5);
                dicOrderTabIndexControl.Add("spImpPrice", 6);
                dicOrderTabIndexControl.Add("spImpVatRatio", 7);
                dicOrderTabIndexControl.Add("lkPackingTypeId", 8);
                dicOrderTabIndexControl.Add("spInternalPrice", 9);
                dicOrderTabIndexControl.Add("spAlertExpiredDate", 10);
                dicOrderTabIndexControl.Add("lkBloodGroupId", 11);
                dicOrderTabIndexControl.Add("lkBloodVolumeId", 12);
                dicOrderTabIndexControl.Add("txtElement", 13);


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
                InitComboHeinServiceTypeId();
                InitComboServiceUnit();
                InitComboHeinServiceBhytId();
                InitComboParentID();
                //InitComboBillOption();
                LoadComboStatus();
                LoadComboPatientType();

                //TODO
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region Init combo
        private void InitComboHeinServiceTypeId()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisHeinServiceTypeFilter filter = new HisHeinServiceTypeFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                var data = new BackendAdapter(param).Get<List<HIS_HEIN_SERVICE_TYPE>>("api/HisHeinServiceType/Get", ApiConsumers.MosConsumer, filter, null).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("HEIN_SERVICE_TYPE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("HEIN_SERVICE_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("HEIN_SERVICE_TYPE_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboType, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void InitComboServiceUnit()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisServiceUnitFilter filter = new HisServiceUnitFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                var data = new BackendAdapter(param).Get<List<HIS_SERVICE_UNIT>>("api/HisServiceUnit/Get", ApiConsumers.MosConsumer, filter, null).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("SERVICE_UNIT_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("SERVICE_UNIT_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("SERVICE_UNIT_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboDVT, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboHeinServiceBhytId()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisHeinServiceBhytFilter filter = new HisHeinServiceBhytFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                var data = new BackendAdapter(param).Get<List<HIS_HEIN_SERVICE_BHYT>>("api/HisHeinServiceBhyt/Get", ApiConsumers.MosConsumer, filter, null).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("HEIN_SERVICE_BHYT_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("HEIN_SERVICE_BHYT_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("HEIN_SERVICE_BHYT_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboBhyt, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboBillOption()
        {
            try
            {
                List<BillOption> billOption = new List<BillOption>();
                billOption.Add(new BillOption(1, "Hóa đơn thường"));
                billOption.Add(new BillOption(2, "Hóa đơn dịch vụ"));
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("billName", "", 235, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("billName", "id", columnInfos, false, 235);
                ControlEditorLoader.Load(cboBillOption, billOption, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadComboStatus()
        {
            try
            {
                List<Status> status = new List<Status>();
                status.Add(new Status(1, "Hóa đơn thường"));
                status.Add(new Status(2, "Tách chênh lệch vào hóa đơn dịch vụ"));
                status.Add(new Status(3, "Hóa đơn dịch vụ"));

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("statusName", "", 300, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("statusName", "id", columnInfos, false, 350);
                ControlEditorLoader.Load(cboBillOption, status, controlEditorADO);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadComboPatientType()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisPatientTypeFilter filter = new HisPatientTypeFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                var data = BackendDataWorker.Get<HIS_PATIENT_TYPE>();
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

        private void InitComboParentID()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisBedServiceTypeFilter filter = new HisBedServiceTypeFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                var data = new BackendAdapter(param).Get<List<HIS_BED_SERVICE_TYPE>>(HisRequestUriStore.MOSHIS_BED_SERVICE_TYPE_GET, ApiConsumers.MosConsumer, filter, null).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("BED_SERVICE_TYPE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("BED_SERVICE_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("BED_SERVICE_TYPE_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboParent, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion


        private void SetFilterNavBar(ref HisBedServiceTypeViewFilter filter)
        {
            try
            {
                filter.KEY_WORD = txtKeyword.Text.Trim();
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

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void ChangedDataRow(MOS.EFMODEL.DataModels.V_HIS_BED_SERVICE_TYPE data)
        {
            try
            {
                if (data != null)
                {
                    FillDataToEditorControl(data);
                    this.ActionType = GlobalVariables.ActionEdit;
                    EnableControlChanged(this.ActionType);
                    //Disable nút sửa nếu dữ liệu đã bị khóa
                    btnEdit.Enabled = (this.currentData.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);

                    positionHandle = -1;
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderEditorInfo, dxErrorProvider);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToEditorControl(MOS.EFMODEL.DataModels.V_HIS_BED_SERVICE_TYPE data)
        {
            try
            {
                if (data != null)
                {
                    txtBedServiceTypeCode.Text = data.BED_SERVICE_TYPE_CODE;
                    txtBedServiceTypeName.Text = data.BED_SERVICE_TYPE_NAME;
                    spNumOrder.EditValue = data.NUM_ORDER;
                    spCogs.EditValue = data.COGS;
                    spEstimateDuration.EditValue = data.ESTIMATE_DURATION;
                    cboParent.EditValue = data.PARENT_ID;
                    cboDVT.EditValue = data.SERVICE_UNIT_ID;
                    cboBhyt.EditValue = data.HEIN_SERVICE_BHYT_ID;
                    cboType.EditValue = data.HEIN_SERVICE_TYPE_ID;
                    if (data.BILL_OPTION == null)
                    {
                        cboBillOption.EditValue = 1;
                    }
                    else if (data.BILL_OPTION == 1)
                    {
                        cboBillOption.EditValue = 2;
                    }
                    else if (data.BILL_OPTION == 2)
                    {
                        cboBillOption.EditValue = 3;
                    }
                    cboPatientType.EditValue = data.BILL_PATIENT_TYPE_ID;
                    chkCPNgoaiGoi.Checked = data.IS_OUT_PARENT_FEE == 1 ? true : false;
                    //cboBillOption.EditValue = ((data.BILL_OPTION == null) ? 1 : 2);
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

        private void LoadCurrent(long currentId, ref MOS.EFMODEL.DataModels.HIS_BED_SERVICE_TYPE currentDTO)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisBedServiceTypeFilter filter = new HisBedServiceTypeFilter();
                filter.ID = currentId;
                currentDTO = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_BED_SERVICE_TYPE>>(HisRequestUriStore.MOSHIS_BED_SERVICE_TYPE_GET, ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
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
                LoaddataToTreeList(this);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void unLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            bool result = false;
            HIS_BED_SERVICE_TYPE success = new HIS_BED_SERVICE_TYPE();
            //bool notHandler = false;
            try
            {
                var data = treeList1.GetDataRecordByNode(treeList1.FocusedNode);
                V_HIS_BED_SERVICE_TYPE rowData = data as V_HIS_BED_SERVICE_TYPE;
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    HisBedServiceTypeFilter filter = new HisBedServiceTypeFilter();
                    filter.ID = rowData.ID;
                    var islock = new BackendAdapter(param).Get<List<HIS_BED_SERVICE_TYPE>>(HisRequestUriStore.MOSHIS_BED_SERVICE_TYPE_GET, ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
                    WaitingManager.Show();
                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_BED_SERVICE_TYPE>(HisRequestUriStore.MOSHIS_BED_SERVICE_TYPE_CHANGE_LOCK, ApiConsumers.MosConsumer, islock, param);
                    WaitingManager.Hide();
                    if (success != null)
                    {
                        result = true;
                        LoaddataToTreeList(this);
                        InitComboParentID();
                    }
                    MessageManager.Show(this, param, result);
                }

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Lock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            bool result = false;
            HIS_BED_SERVICE_TYPE success = new HIS_BED_SERVICE_TYPE();
            //bool notHandler = false;
            try
            {
                var data = treeList1.GetDataRecordByNode(treeList1.FocusedNode);
                V_HIS_BED_SERVICE_TYPE rowData = data as V_HIS_BED_SERVICE_TYPE;
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    HisBedServiceTypeFilter filter = new HisBedServiceTypeFilter();
                    filter.ID = rowData.ID;
                    var islock = new BackendAdapter(param).Get<List<HIS_BED_SERVICE_TYPE>>(HisRequestUriStore.MOSHIS_BED_SERVICE_TYPE_GET, ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
                    WaitingManager.Show();
                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_BED_SERVICE_TYPE>(HisRequestUriStore.MOSHIS_BED_SERVICE_TYPE_CHANGE_LOCK, ApiConsumers.MosConsumer, islock, param);
                    WaitingManager.Hide();
                    if (success != null)
                    {
                        result = true;
                        LoaddataToTreeList(this);
                        InitComboParentID();
                    }
                    MessageManager.Show(this, param, result);
                }

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }



        private void btnRefesh_Click(object sender, EventArgs e)
        {
            try
            {
                SetDefaultValue();
                //LoaddataToTreeList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnGDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong), "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    var data = treeList1.GetDataRecordByNode(treeList1.FocusedNode);
                    V_HIS_BED_SERVICE_TYPE rowData = data as V_HIS_BED_SERVICE_TYPE;
                    HIS_BED_SERVICE_TYPE row = new HIS_BED_SERVICE_TYPE();
                    row.ID = rowData.ID;
                    if (rowData != null)
                    {
                        bool success = false;
                        CommonParam param = new CommonParam();
                        success = new BackendAdapter(param).Post<bool>(HisRequestUriStore.MOSHIS_BED_SERVICE_TYPE_DELETE, ApiConsumers.MosConsumer, row, param);
                        if (success)
                        {
                            LoaddataToTreeList(this);
                            InitComboParentID();
                            currentData = ((List<V_HIS_BED_SERVICE_TYPE>)treeList1.DataSource).FirstOrDefault();
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

        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                this.ActionType = GlobalVariables.ActionAdd;
                EnableControlChanged(this.ActionType);
                positionHandle = -1;
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderEditorInfo, dxErrorProvider);
                ResetFormData();
                SetFocusEditor();
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
                HisBedServiceTypeSDO sdo = new HisBedServiceTypeSDO();
                MOS.EFMODEL.DataModels.HIS_BED_SERVICE_TYPE updateDTO = new MOS.EFMODEL.DataModels.HIS_BED_SERVICE_TYPE();
                HIS_SERVICE service = new HIS_SERVICE();
                if (cboType.EditValue != null) service.HEIN_SERVICE_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboType.EditValue ?? "0").ToString());
                if (cboBhyt.EditValue != null) service.HEIN_SERVICE_BHYT_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboBhyt.EditValue ?? "0").ToString());
                if (cboDVT.EditValue != null) service.SERVICE_UNIT_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboDVT.EditValue ?? "0").ToString());
                if (cboPatientType.EditValue != null) service.BILL_PATIENT_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboPatientType.EditValue ?? "0").ToString());
                if (Inventec.Common.TypeConvert.Parse.ToInt64((cboBillOption.EditValue ?? 0).ToString()) == 1 || cboBillOption.EditValue == null) service.BILL_OPTION = null;
                else if (Inventec.Common.TypeConvert.Parse.ToInt64((cboBillOption.EditValue ?? 0).ToString()) == 2)
                    service.BILL_OPTION = 1;
                else if (Inventec.Common.TypeConvert.Parse.ToInt64((cboBillOption.EditValue ?? 0).ToString()) == 3)
                    service.BILL_OPTION = 2;
                if (spCogs.EditValue != null) service.COGS = (decimal)spCogs.Value;
                if (spEstimateDuration.EditValue != null) service.ESTIMATE_DURATION = (decimal)spEstimateDuration.Value;
                service.IS_OUT_PARENT_FEE = (short)(chkCPNgoaiGoi.Checked ? 1 : 0);
                if (ActionType == GlobalVariables.ActionEdit)
                {
                    if (this.currentData != null && this.currentData.ID > 0)
                    {
                        LoadCurrent(this.currentData.ID, ref updateDTO);
                    }
                }

                UpdateDTOFromDataForm(ref updateDTO);
                //updateDTO.ID = 1;
                sdo.HisBedServiceType = updateDTO;
                sdo.HisService = service;

                if (ActionType == GlobalVariables.ActionAdd)
                {
                    sdo.HisBedServiceType.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    var resultData = new BackendAdapter(param).Post<HisBedServiceTypeSDO>(HisRequestUriStore.MOSHIS_BED_SERVICE_TYPE_CREATE, ApiConsumers.MosConsumer, sdo, param);
                    if (resultData != null)
                    {
                        success = true;
                        LoaddataToTreeList(this);
                        InitComboParentID();
                        ResetFormData();
                    }
                }
                else
                {
                    sdo.HisService.ID = currentData.SERVICE_ID;
                    sdo.HisService.SERVICE_TYPE_ID = currentData.SERVICE_TYPE_ID;
                    var resultData = new BackendAdapter(param).Post<HisBedServiceTypeSDO>(HisRequestUriStore.MOSHIS_BED_SERVICE_TYPE_UPDATE, ApiConsumers.MosConsumer, sdo, param);
                    if (resultData != null)
                    {
                        success = true;
                        LoaddataToTreeList(this);
                        InitComboParentID();

                    }
                }

                if (success)
                {
                    SetFocusEditor();
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


        private void UpdateDTOFromDataForm(ref MOS.EFMODEL.DataModels.HIS_BED_SERVICE_TYPE currentDTO)
        {
            try
            {
                if (!String.IsNullOrEmpty(txtBedServiceTypeCode.Text))
                    currentDTO.BED_SERVICE_TYPE_CODE = txtBedServiceTypeCode.Text.Trim();
                if (!String.IsNullOrEmpty(txtBedServiceTypeName.Text))
                    currentDTO.BED_SERVICE_TYPE_NAME = txtBedServiceTypeName.Text.Trim();
                if (spNumOrder.EditValue != null) currentDTO.NUM_ORDER = (long)spNumOrder.Value;
                else
                    currentDTO.NUM_ORDER = null;
                if (cboParent.EditValue != null) currentDTO.PARENT_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboParent.EditValue ?? "0").ToString());
                else
                    currentDTO.PARENT_ID = null;
            }
            catch (Exception ex)
            {
                currentDTO = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Validate
        private void ValidateForm()
        {
            try
            {
                ValidationSingleControl(txtBedServiceTypeCode);
                ValidationSingleControl(txtBedServiceTypeName);
                //ValidationSingleControl(cboPatientType);
                ValidationSingleControl(cboDVT);
                //ValidationSingleControl(cboParent);
                ValidationSingleControl1(spNumOrder);
                ValidationSingleControl1(spCogs);
                ValidationSingleControl1(spEstimateDuration);
                //ValidationSingleControl(lkPackingTypeId);
                //ValidationSingleControl1(spInternalPrice);
                //alidationSingleControl1(spEstimateDuration);
                //ValidationSingleControl(lkBloodGroupId);
                //ValidationSingleControl(lkBloodVolumeId);
                //ValidationSingleControl(txtElement);

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

                //Load du lieu
                LoaddataToTreeList(this);

                //Load ngon ngu label control
                SetCaptionByLanguageKey();

                //Set tabindex control
                InitTabIndex();

                //Set validate rule
                ValidateForm();

                //Focus default
                SetDefaultFocus();
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

        private void spImpVatRatio_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void gridviewFormList_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    V_HIS_BED_SERVICE_TYPE data = (V_HIS_BED_SERVICE_TYPE)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "IS_ACTIVE_STR")
                        {
                            if (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE)
                                e.Appearance.ForeColor = Color.Red;
                            else
                                e.Appearance.ForeColor = Color.Green;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtCode_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtBedServiceTypeName.Focus();
                    txtBedServiceTypeName.SelectAll();
                }//e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void txtName_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboDVT.Focus();
                    if (cboDVT.EditValue == null)
                    {
                        cboDVT.ShowPopup();
                    }
                }//e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void spNumOrder_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkCPNgoaiGoi.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spEstimateDuration_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spNumOrder.Focus();
                    spNumOrder.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spCogs_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spEstimateDuration.Focus();
                    spEstimateDuration.SelectAll();
                }//e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void cboBhyt_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboBillOption.Focus();
                    if (cboBillOption.EditValue == null)
                    {
                        cboBillOption.ShowPopup();
                    }
                }//e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void cboParent_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spCogs.Focus();
                    spCogs.SelectAll();
                }//e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void cboDVT_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboType.Focus();
                    if (cboType.EditValue == null)
                    {
                        cboType.ShowPopup();
                    }
                }//e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void cboType_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboBhyt.Focus();
                    if (cboBhyt.EditValue == null)
                    {
                        cboBhyt.ShowPopup();
                    }
                }//e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void panelControl1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void treeList1_Click(object sender, EventArgs e)
        {
            try
            {
                //InitComboParentID();
                var data = treeList1.GetDataRecordByNode(treeList1.FocusedNode);
                V_HIS_BED_SERVICE_TYPE rowData = data as V_HIS_BED_SERVICE_TYPE;
                if (rowData != null)
                {
                    currentData = rowData;
                    ChangedDataRow(rowData);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void treeList1_CustomNodeCellEdit(object sender, DevExpress.XtraTreeList.GetCustomNodeCellEditEventArgs e)
        {
            try
            {
                var data = treeList1.GetDataRecordByNode(e.Node);
                if (data != null && data is V_HIS_BED_SERVICE_TYPE)
                {
                    V_HIS_BED_SERVICE_TYPE rowData = data as V_HIS_BED_SERVICE_TYPE;
                    if (rowData == null) return;

                    else if (e.Column.FieldName == "IS_LOCK")
                    {
                        e.RepositoryItem = (rowData.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? unLockT : LockT);

                    }
                    else if (e.Column.FieldName == "Delete")
                    {
                        try
                        {
                            if (rowData.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                                e.RepositoryItem = DeleteE;
                            else
                                e.RepositoryItem = DeleteD;
                        }
                        catch (Exception ex)
                        {

                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeList1_CustomUnboundColumnData(object sender, DevExpress.XtraTreeList.TreeListCustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData)
                {
                    V_HIS_BED_SERVICE_TYPE pData = (V_HIS_BED_SERVICE_TYPE)e.Row;
                    //var pData = data as V_HIS_BED_SERVICE_TYPE;
                    short status = Inventec.Common.TypeConvert.Parse.ToInt16((pData.IS_ACTIVE ?? -1).ToString());
                    if (pData == null || this.treeList1 == null) return;

                    if (e.Column.FieldName == "CREATE_TIME_STR")
                    {
                        try
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Inventec.Common.TypeConvert.Parse.ToInt64(pData.CREATE_TIME.ToString()));

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao CREATE_TIME", ex);
                        }
                    }
                    else if (e.Column.FieldName == "CPNG")
                    {
                        try
                        {
                            e.Value = pData.IS_OUT_PARENT_FEE == 1 ? true : false;

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay sua CPNG", ex);
                        }
                    }
                    else if (e.Column.FieldName == "ACTIVE_ITEM")
                    {
                        try
                        {
                            if (status == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                                e.Value = Inventec.Common.Resource.Get.Value("frmHisBedServiceType.HoatDong", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                            else
                                e.Value = Inventec.Common.Resource.Get.Value("frmHisBedServiceType.TamKhoa", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                    else if (e.Column.FieldName == "MODIFY_TIME_STR")
                    {
                        try
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Inventec.Common.TypeConvert.Parse.ToInt64(pData.MODIFY_TIME.ToString()));

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay sua MODIFY_TIME", ex);
                        }
                    }

                    else if (e.Column.FieldName == "BILL_OPTION_STR")
                    {
                        try
                        {
                            if (pData.BILL_OPTION == null)
                                e.Value = "Hóa đơn thường";
                            else if (pData.BILL_OPTION == 1)
                                e.Value = "Tách chênh lệch vào hóa đơn dịch vụ";
                            else if (pData.BILL_OPTION == 2)
                                e.Value = "Hóa đơn dịch vụ";
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay sua BILL_OPTION", ex);
                        }
                    }


                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void LoaddataToTreeList(frmHisBedServiceType control)
        {
            try
            {

                CommonParam param = new CommonParam();
                MOS.Filter.HisBedServiceTypeViewFilter filter = new MOS.Filter.HisBedServiceTypeViewFilter();
                filter.KEY_WORD = txtKeyword.Text;
                //filter.IsTree = true;
                var currentDataStore = new BackendAdapter(param).Get<List<V_HIS_BED_SERVICE_TYPE>>(HisRequestUriStore.MOSHIS_BED_SERVICE_TYPE_GETVIEW, ApiConsumers.MosConsumer, filter, param).ToList();
                if (currentDataStore != null)
                {
                    treeList1.KeyFieldName = "ID";
                    treeList1.ParentFieldName = "PARENT_ID";
                    treeList1.DataSource = currentDataStore;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeList1_NodeCellStyle(object sender, DevExpress.XtraTreeList.GetCustomNodeCellStyleEventArgs e)
        {
            try
            {
                var data = treeList1.GetDataRecordByNode(e.Node);
                if (data != null && data is V_HIS_BED_SERVICE_TYPE)
                {
                    V_HIS_BED_SERVICE_TYPE rowData = data as V_HIS_BED_SERVICE_TYPE;
                    if (rowData == null) return;

                    else if (e.Column.FieldName == "ACTIVE_ITEM")
                    {
                        if (rowData.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE)
                            e.Appearance.ForeColor = Color.Red;
                        else
                            e.Appearance.ForeColor = Color.Green;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void treeList1_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    var data = treeList1.GetDataRecordByNode(treeList1.FocusedNode);
                    V_HIS_BED_SERVICE_TYPE rowData = data as V_HIS_BED_SERVICE_TYPE;
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

        private void cboBillOption_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboPatientType.Focus();
                    if (cboPatientType.EditValue == null)
                    {
                        cboPatientType.ShowPopup();
                    }
                }//e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void cboPatientType_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboParent.Focus();
                    if (cboParent.EditValue == null)
                    {
                        cboParent.ShowPopup();
                    }
                }//e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void cboParent_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboParent.Properties.Buttons[1].Visible = true;
                    cboParent.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void cboType_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboType.Properties.Buttons[1].Visible = true;
                    cboType.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboBhyt_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboBhyt.Properties.Buttons[1].Visible = true;
                    cboBhyt.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboBillOption_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboBillOption.Properties.Buttons[1].Visible = true;
                    cboBillOption.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPatientType_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboPatientType.Properties.Buttons[1].Visible = true;
                    cboPatientType.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkCPNgoaiGoi_KeyUp(object sender, KeyEventArgs e)
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
                if (e.KeyCode == Keys.Space)
                {
                    chkCPNgoaiGoi.Checked = !chkCPNgoaiGoi.Checked;
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

    }
}
