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
using HIS.Desktop.LocalStorage.SdaConfigKey.Config;
using MOS.SDO;
using HIS.Desktop.Plugins.HisSuimServiceType.Resources;
using Inventec.Desktop.Common.LanguageManager;
using System.Resources;

namespace HIS.Desktop.Plugins.HisSuimServiceType.HisSuimServiceType
{
    public partial class frmHisSuimServiceType : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        PagingGrid pagingGrid;
        int ActionType = -1;
        int positionHandle = -1;
        MOS.EFMODEL.DataModels.V_HIS_SUIM_SERVICE_TYPE currentData;
        List<string> arrControlEnableNotChange = new List<string>();
        Dictionary<string, int> dicOrderTabIndexControl = new Dictionary<string, int>();
        Inventec.Desktop.Common.Modules.Module moduleData;
        private const short IS_ACTIVE_TRUE = 1;
        private const short IS_ACTIVE_FALSE = 0;
        internal long serviceId;
        internal long suimId;
        V_HIS_SUIM_SERVICE_TYPE currentDataSet = null;
        #endregion

        #region Construct
        public frmHisSuimServiceType(Inventec.Desktop.Common.Modules.Module moduleData)
		:base(moduleData)
        {
            try
            {
                InitializeComponent();

                pagingGrid = new PagingGrid();
                this.moduleData = moduleData;
                TreeListSuimServiceType.ToolTipController = toolTipControllerGrid;

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
        private void frmHisSuimServiceType_Load(object sender, EventArgs e)
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
                if (this.moduleData != null && !String.IsNullOrEmpty(this.moduleData.text))
                {
                    this.Text = this.moduleData.text;
                }
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisSuimServiceType.Resources.Lang", typeof(HIS.Desktop.Plugins.HisSuimServiceType.HisSuimServiceType.frmHisSuimServiceType).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("frmHisSuimServiceType.layoutControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl7.Text = Inventec.Common.Resource.Get.Value("frmHisSuimServiceType.layoutControl7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn1.Caption = Inventec.Common.Resource.Get.Value("frmHisSuimServiceType.treeListColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn2.Caption = Inventec.Common.Resource.Get.Value("frmHisSuimServiceType.treeListColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn3.Caption = Inventec.Common.Resource.Get.Value("frmHisSuimServiceType.treeListColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn4.Caption = Inventec.Common.Resource.Get.Value("frmHisSuimServiceType.treeListColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn5.Caption = Inventec.Common.Resource.Get.Value("frmHisSuimServiceType.treeListColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn6.Caption = Inventec.Common.Resource.Get.Value("frmHisSuimServiceType.treeListColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn7.Caption = Inventec.Common.Resource.Get.Value("frmHisSuimServiceType.treeListColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn8.Caption = Inventec.Common.Resource.Get.Value("frmHisSuimServiceType.treeListColumn8.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn9.Caption = Inventec.Common.Resource.Get.Value("frmHisSuimServiceType.treeListColumn9.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn10.Caption = Inventec.Common.Resource.Get.Value("frmHisSuimServiceType.treeListColumn10.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn11.Caption = Inventec.Common.Resource.Get.Value("frmHisSuimServiceType.treeListColumn11.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn12.Caption = Inventec.Common.Resource.Get.Value("frmHisSuimServiceType.treeListColumn12.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn13.Caption = Inventec.Common.Resource.Get.Value("frmHisSuimServiceType.treeListColumn13.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn14.Caption = Inventec.Common.Resource.Get.Value("frmHisSuimServiceType.treeListColumn14.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn15.Caption = Inventec.Common.Resource.Get.Value("frmHisSuimServiceType.treeListColumn15.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn18.Caption = Inventec.Common.Resource.Get.Value("frmHisSuimServiceType.treeListColumn18.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("frmHisSuimServiceType.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyword.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmHisSuimServiceType.txtKeyword.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcEditorInfo.Text = Inventec.Common.Resource.Get.Value("frmHisSuimServiceType.lcEditorInfo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmHisSuimServiceType.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnSearch.Caption = Inventec.Common.Resource.Get.Value("frmHisSuimServiceType.bbtnSearch.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnEdit.Caption = Inventec.Common.Resource.Get.Value("frmHisSuimServiceType.bbtnEdit.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnAdd.Caption = Inventec.Common.Resource.Get.Value("frmHisSuimServiceType.bbtnAdd.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnReset.Caption = Inventec.Common.Resource.Get.Value("frmHisSuimServiceType.bbtnReset.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnFocusDefault.Caption = Inventec.Common.Resource.Get.Value("frmHisSuimServiceType.bbtnFocusDefault.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboDV.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisSuimServiceType.cboDV.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboBill.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisSuimServiceType.cboBill.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboDVT.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisSuimServiceType.cboDVT.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboParent.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisSuimServiceType.cboParent.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnCancel.Text = Inventec.Common.Resource.Get.Value("frmHisSuimServiceType.btnCancel.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("frmHisSuimServiceType.btnAdd.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("frmHisSuimServiceType.btnEdit.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkIsLeaf.Properties.Caption = Inventec.Common.Resource.Get.Value("frmHisSuimServiceType.chkIsLeaf.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciSuimServiceTypeCode.Text = Inventec.Common.Resource.Get.Value("frmHisSuimServiceType.lciSuimServiceTypeCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciSuimServiceTypeName.Text = Inventec.Common.Resource.Get.Value("frmHisSuimServiceType.lciSuimServiceTypeName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciServiceId.Text = Inventec.Common.Resource.Get.Value("frmHisSuimServiceType.lciServiceId.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciParentId.Text = Inventec.Common.Resource.Get.Value("frmHisSuimServiceType.lciParentId.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciIsLeaf.Text = Inventec.Common.Resource.Get.Value("frmHisSuimServiceType.lciIsLeaf.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciNumOrder.Text = Inventec.Common.Resource.Get.Value("frmHisSuimServiceType.lciNumOrder.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem6.Text = Inventec.Common.Resource.Get.Value("frmHisSuimServiceType.layoutControlItem6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem7.Text = Inventec.Common.Resource.Get.Value("frmHisSuimServiceType.layoutControlItem7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem9.Text = Inventec.Common.Resource.Get.Value("frmHisSuimServiceType.layoutControlItem9.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem10.Text = Inventec.Common.Resource.Get.Value("frmHisSuimServiceType.layoutControlItem10.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem12.Text = Inventec.Common.Resource.Get.Value("frmHisSuimServiceType.layoutControlItem12.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmHisSuimServiceType.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
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
                spCogs.EditValue = null;
                spTime.EditValue = null;
                spNumOrder.EditValue = null;
                cboBill.EditValue = 1;
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

        private void FillDataToControlsForm()
        {
            try
            {
                InitComboServiceId();
                initComboCha();
                InitComboDV();
                initComboBill();
                InitComboDTTT();
                InitComboLoaiBH();
                //TODO
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region Init combo
        private void InitComboServiceId()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("SERVICE_UNIT_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("SERVICE_UNIT_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("SERVICE_UNIT_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboDVT, BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_SERVICE_UNIT>(), controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void InitComboDV()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("HEIN_SERVICE_BHYT_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("HEIN_SERVICE_BHYT_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("HEIN_SERVICE_BHYT_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboDV, BackendDataWorker.Get<HIS_HEIN_SERVICE>(), controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void InitComboLoaiBH()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("HEIN_SERVICE_TYPE_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("HEIN_SERVICE_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("HEIN_SERVICE_TYPE_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboLoaiBH, BackendDataWorker.Get<HIS_HEIN_SERVICE_TYPE>(), controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void InitComboDTTT()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PATIENT_TYPE_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("PATIENT_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PATIENT_TYPE_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboPatientType, BackendDataWorker.Get<HIS_PATIENT_TYPE>(), controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void initComboCha()
        {
            //List<ColumnInfo> columnInfos = new List<ColumnInfo>();
            //columnInfos.Add(new ColumnInfo("SUIM_SERVICE_TYPE_CODE", "", 150, 1));
            //columnInfos.Add(new ColumnInfo("SUIM_SERVICE_TYPE_NAME", "", 250, 2));
            try
            {
                CommonParam param = new CommonParam();
                HisSuimServiceTypeViewFilter filter = new HisSuimServiceTypeViewFilter();
                filter.IS_ACTIVE = 1;
                var serviceTypeParents = new BackendAdapter(param)
                    .Get<List<V_HIS_SUIM_SERVICE_TYPE>>(HisRequestUriStore.MOSHIS_SUIM_SERVICE_TYPE_GET, ApiConsumers.MosConsumer, filter, param);
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("SUIM_SERVICE_TYPE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("SUIM_SERVICE_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("SUIM_SERVICE_TYPE_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboParent, serviceTypeParents, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }
        private void initComboBill()
        {
            try
            {
                List<Status> status = new List<Status>();
                status.Add(new Status(1, "Hóa đơn thường"));
                status.Add(new Status(2, "Tách tiền chênh lệch vào hóa đơn dịch vụ"));
                status.Add(new Status(3, "Hóa đơn dịch vụ"));

                List<Inventec.Common.Controls.EditorLoader.ColumnInfo> columnInfos = new List<Inventec.Common.Controls.EditorLoader.ColumnInfo>();
                columnInfos.Add(new Inventec.Common.Controls.EditorLoader.ColumnInfo("statusName", "", 300, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("statusName", "id", columnInfos, false, 350);
                ControlEditorLoader.Load(cboBill, status, controlEditorADO);
                cboBill.EditValue = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion


        private void SetFilterNavBar(ref HisSuimServiceTypeViewFilter filter)
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
        private void ChangedDataRow(MOS.EFMODEL.DataModels.V_HIS_SUIM_SERVICE_TYPE data)
        {
            try
            {
                if (data != null)
                {
                    FillDataToEditorControl(data);
                    this.ActionType = GlobalVariables.ActionEdit;
                    EnableControlChanged(this.ActionType);

                    //Disable nút sửa nếu dữ liệu đã bị khóa
                    if (currentDataSet != null)
                    {
                        btnEdit.Enabled = (this.currentDataSet.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);
                    }
                    positionHandle = -1;
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderEditorInfo, dxErrorProvider);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToEditorControl(MOS.EFMODEL.DataModels.V_HIS_SUIM_SERVICE_TYPE data)
        {
            try
            {
                if (data != null)
                {
                    serviceId = data.SERVICE_ID;
                    suimId = data.ID;
                    txtSuimServiceTypeCode.Text = data.SUIM_SERVICE_TYPE_CODE;
                    txtSuimServiceTypeName.Text = data.SUIM_SERVICE_TYPE_NAME;
                    cboDVT.EditValue = data.SERVICE_UNIT_ID;
                    cboDV.EditValue = data.HEIN_SERVICE_ID;
                    cboParent.EditValue = data.PARENT_ID;
                    chkIsLeaf.Checked = (data.IS_MULTI_REQUEST == 1 ? true : false);
                    chkOutPack.Checked = (data.IS_OUT_PARENT_FEE == 1 ? true : false);
                    spNumOrder.EditValue = data.NUM_ORDER;
                    spTime.EditValue = data.ESTIMATE_DURATION;
                    cboPatientType.EditValue = data.BILL_PATIENT_TYPE_ID;
                    spCogs.EditValue = data.COGS;
                    cboLoaiBH.EditValue = data.HEIN_SERVICE_TYPE_ID;
                    if (data.BILL_OPTION == null)
                    {
                        cboBill.EditValue = 1;
                    }
                    else if (data.BILL_OPTION == 1)
                    {
                        cboBill.EditValue = 2;
                    }
                    else if (data.BILL_OPTION == 2)
                    {
                        cboBill.EditValue = 3;
                    }
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

        private void LoadCurrent(long currentId, ref MOS.EFMODEL.DataModels.V_HIS_SUIM_SERVICE_TYPE currentDTO)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisSuimServiceTypeViewFilter filter = new HisSuimServiceTypeViewFilter();
                filter.ID = currentId;
                currentData = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_SUIM_SERVICE_TYPE>>(HisRequestUriStore.MOSHIS_SUIM_SERVICE_TYPE_GET, ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
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
                LoaddataToTreeList();
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
                LoaddataToTreeList();
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
                txtSuimServiceTypeCode.Focus();
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
                MOS.SDO.HisSuimServiceTypeSDO serviceSDO = new MOS.SDO.HisSuimServiceTypeSDO();
                MOS.SDO.HisSuimServiceTypeSDO suimServiceTypeResultSDO = new MOS.SDO.HisSuimServiceTypeSDO();

                serviceSDO.HisService = setDataService();
                serviceSDO.HisSuimServiceType = setDataSuimServiceType();
                if (ActionType == GlobalVariables.ActionAdd)
                {
                    suimServiceTypeResultSDO = new BackendAdapter(param).Post<HisSuimServiceTypeSDO>(HisRequestUriStore.MOSHIS_SUIM_SERVICE_TYPE_CREATE, ApiConsumers.MosConsumer, serviceSDO, param);
                }
                else
                {
                    if (serviceId > 0 && suimId > 0)
                    {
                        serviceSDO.HisService.ID = serviceId;
                        serviceSDO.HisSuimServiceType.ID = suimId;
                        suimServiceTypeResultSDO = new BackendAdapter(param).Post<HisSuimServiceTypeSDO>(HisRequestUriStore.MOSHIS_SUIM_SERVICE_TYPE_UPDATE, ApiConsumers.MosConsumer, serviceSDO, param);
                    }
                }

                if (suimServiceTypeResultSDO != null)
                {
                    success = true;
                    LoaddataToTreeList();
                    ResetFormData();
                    initComboCha();
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
        #endregion

        #region Validate
        private void ValidateForm()
        {
            try
            {
                ValidationSingleControl(txtSuimServiceTypeCode);
                ValidationSingleControl(txtSuimServiceTypeName);
                ValidationSingleControl(cboDVT);
                //ValidationSingleControl(cboPatientType);

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
                validRule.ErrorText = String.Format(ResourceLanguageManager.TruongDuLieuBatBuoc);
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
                validRule.ErrorText = String.Format(ResourceLanguageManager.TruongDuLieuBatBuoc);
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
                validRule.ErrorText = String.Format(ResourceLanguageManager.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditorInfo.SetValidationRule(control, validRule);
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
                //FillDataToGridControl();
                LoaddataToTreeList();
                //Load ngon ngu label control
                SetCaptionByLanguageKey();

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

        #region TreeList
        private void LoaddataToTreeList()
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisSuimServiceTypeViewFilter filter = new MOS.Filter.HisSuimServiceTypeViewFilter();
                filter.KEY_WORD = txtKeyword.Text;
                var currentData = new BackendAdapter(param).Get<List<V_HIS_SUIM_SERVICE_TYPE>>("api/HisSuimServiceType/GetView", ApiConsumers.MosConsumer, filter, param).ToList();
                if (currentData != null)
                {

                    TreeListSuimServiceType.KeyFieldName = "ID";
                    TreeListSuimServiceType.ParentFieldName = "PARENT_ID";
                    TreeListSuimServiceType.DataSource = currentData;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void TreeListSuimServiceType_CustomNodeCellEdit(object sender, DevExpress.XtraTreeList.GetCustomNodeCellEditEventArgs e)
        {
            try
            {
                var data = TreeListSuimServiceType.GetDataRecordByNode(e.Node);
                if (data != null && data is V_HIS_SUIM_SERVICE_TYPE)
                {
                    V_HIS_SUIM_SERVICE_TYPE rowData = data as V_HIS_SUIM_SERVICE_TYPE;
                    if (rowData == null) return;

                    else if (e.Column.FieldName == "Lock")
                    {
                        e.RepositoryItem = (rowData.IS_ACTIVE == IS_ACTIVE_FALSE ? btnUnlock : btnLock);

                    }
                    else if (e.Column.FieldName == "Delete")
                    {
                        try
                        {
                            if (rowData.IS_ACTIVE == IS_ACTIVE_TRUE)
                                e.RepositoryItem = btnDelete;
                            else
                                e.RepositoryItem = btnDelete_Disable;
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

        private void TreeListSuimServiceType_CustomUnboundColumnData(object sender, DevExpress.XtraTreeList.TreeListCustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData)
                {
                    V_HIS_SUIM_SERVICE_TYPE pData = e.Row as V_HIS_SUIM_SERVICE_TYPE;
                    if (pData == null || this.TreeListSuimServiceType == null) return;
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
                    else if (e.Column.FieldName == "IS_MULTI_REQUEST_STR")
                    {
                        try
                        {
                            e.Value = pData != null && pData.IS_MULTI_REQUEST == 1 ? true : false;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi cho phep nhap chi dinh lon hon 1 IS_MULTI_REQUEST", ex);
                        }
                    }
                    else if (e.Column.FieldName == "CPNG")
                    {
                        try
                        {
                            e.Value = pData != null && pData.IS_OUT_PARENT_FEE == 1 ? true : false;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi CPNG", ex);
                        }
                    }
                    else if (e.Column.FieldName == "BILL_OTPION_STR")
                    {
                        try
                        {
                            e.Value = pData.BILL_OPTION;
                            if (pData.BILL_OPTION == null)
                            {
                                e.Value = "Hóa đơn thường";
                            }
                            else if (pData.BILL_OPTION == 1)
                            {
                                e.Value = "Tách tiền chênh lệch vào hóa đơn dịch vụ";
                            }
                            else if (pData.BILL_OPTION == 2)
                            {
                                e.Value = "Hóa đơn dịch vụ";
                            }
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("BILL_OTPION_STR", ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void TreeListSuimServiceType_Click(object sender, EventArgs e)
        {
            try
            {
                var data = TreeListSuimServiceType.GetDataRecordByNode(TreeListSuimServiceType.FocusedNode);
                V_HIS_SUIM_SERVICE_TYPE rowData = data as V_HIS_SUIM_SERVICE_TYPE;
                if (rowData != null)
                {
                    currentDataSet = rowData;
                    ChangedDataRow(currentDataSet);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region UpdateData
        private HIS_SERVICE setDataService()
        {
            HIS_SERVICE service = new HIS_SERVICE();
            try
            {
                if (cboDVT.EditValue != null) service.SERVICE_UNIT_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboDVT.EditValue ?? null).ToString());
                service.SERVICE_TYPE_ID = HisServiceTypeCFG.SERVICE_TYPE_ID__SUIM;
                if (cboDV.EditValue != null && !String.IsNullOrEmpty(cboDV.Text)) service.HEIN_SERVICE_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboDV.EditValue ?? null).ToString());
                if (cboPatientType.EditValue != null) service.BILL_PATIENT_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboPatientType.EditValue ?? null).ToString());
                if (cboBill.EditValue != null)
                {
                    if (Inventec.Common.TypeConvert.Parse.ToInt64((cboBill.EditValue ?? 0).ToString()) == 1)
                    {
                        service.BILL_OPTION = null;
                    }
                    else if (Inventec.Common.TypeConvert.Parse.ToInt64((cboBill.EditValue ?? 0).ToString()) == 2)
                    {
                        service.BILL_OPTION = 1;
                    }
                    else if (Inventec.Common.TypeConvert.Parse.ToInt64((cboBill.EditValue ?? 0).ToString()) == 3)
                    {
                        service.BILL_OPTION = 2;
                    }
                }
                if (spCogs.EditValue != null)
                {
                    service.COGS = (long)spCogs.Value;
                }
                if (spTime.EditValue != null)
                {
                    service.ESTIMATE_DURATION = (long)spTime.Value;
                }
                service.IS_MULTI_REQUEST = (short)(chkIsLeaf.Checked ? 1 : 0);
                service.IS_OUT_PARENT_FEE = (short)(chkOutPack.Checked ? 1 : 0);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return service;
        }
        private HIS_SUIM_SERVICE_TYPE setDataSuimServiceType()
        {
            HIS_SUIM_SERVICE_TYPE suimServiceType = new HIS_SUIM_SERVICE_TYPE();
            try
            {
                if (!String.IsNullOrEmpty(txtSuimServiceTypeCode.Text))
                    suimServiceType.SUIM_SERVICE_TYPE_CODE = txtSuimServiceTypeCode.Text;
                if (!String.IsNullOrEmpty(txtSuimServiceTypeName.Text))
                    suimServiceType.SUIM_SERVICE_TYPE_NAME = txtSuimServiceTypeName.Text;
                if (cboParent.EditValue != null) suimServiceType.PARENT_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboParent.EditValue ?? "0").ToString());
                if (suimServiceType.PARENT_ID == 0)
                {
                    suimServiceType.PARENT_ID = null;
                }
                if (spNumOrder.EditValue != null)
                {
                    suimServiceType.NUM_ORDER = (long)spNumOrder.Value;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return suimServiceType;
        }

        #endregion

        #region Event
        private void txtSuimServiceTypeCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtSuimServiceTypeName.Focus();
                    txtSuimServiceTypeName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtSuimServiceTypeName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboDVT.Focus();
                    cboDVT.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDVT_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboDVT.EditValue != null)
                    {
                        if (cboPatientType.EditValue == null)
                        {
                            cboPatientType.Focus();
                            cboPatientType.ShowPopup();
                        }
                        else
                        {
                            cboPatientType.Focus();
                        }
                    }
                    else
                    {
                        cboDVT.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDVT_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboDVT.EditValue == null)
                    {
                        cboDVT.ShowPopup();
                    }
                    else
                    {
                        if (cboPatientType.EditValue == null)
                        {
                            cboPatientType.Focus();
                            cboPatientType.ShowPopup();
                        }
                        else
                        {
                            cboPatientType.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboBill_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboDV.EditValue == null)
                    {
                        cboDV.Focus();
                        cboDV.ShowPopup();
                    }
                    else
                    {
                        cboDV.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboBill_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboBill.EditValue == null)
                    {
                        cboBill.ShowPopup();
                    }
                    else
                    {
                        if (cboBill.EditValue == "")
                        {
                            cboBill.EditValue = null;
                            cboBill.ShowPopup();
                        }
                        else
                        {
                            cboDV.Focus();
                            cboDV.ShowPopup();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDV_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboParent.EditValue == null)
                    {
                        cboParent.Focus();
                        cboParent.ShowPopup();
                    }
                    else
                    {
                        cboParent.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDV_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboDV.EditValue == null)
                    {
                        cboDV.ShowPopup();
                    }
                    else
                    {
                        if (cboParent.EditValue != null)
                        {
                            cboParent.Focus();
                        }
                        else
                        {
                            cboParent.Focus();
                            cboParent.ShowPopup();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboParent_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (txtSuimServiceTypeName.Text != "")
                    {
                        if (cboParent.Text == txtSuimServiceTypeName.Text)
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show("Không được chọn chính nó làm cha.Vui lòng chọn lại", "Thông báo");
                            cboParent.ShowPopup();
                            return;
                        }
                    }
                    spCogs.Focus();
                    spCogs.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboParent_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboParent.EditValue == null)
                    {
                        cboParent.ShowPopup();
                    }
                    else
                    {
                        if (cboParent.EditValue == "")
                        {
                            cboParent.EditValue = null;
                            cboParent.ShowPopup();
                        }
                        else
                        {
                            spCogs.Focus();
                            spCogs.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spCogs_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spTime.Focus();
                    spTime.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spTime_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkIsLeaf.Properties.FullFocusRect = true;
                    chkIsLeaf.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkIsLeaf_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkOutPack.Focus();

                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spNumOrder_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (btnAdd.Enabled == true)
                    {
                        btnAdd.Focus();
                    }
                    else
                    {
                        btnEdit.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            V_HIS_SUIM_SERVICE_TYPE hisMediStock = new V_HIS_SUIM_SERVICE_TYPE();
            bool notHandler = false;
            try
            {
                WaitingManager.Show();
                var data = TreeListSuimServiceType.GetDataRecordByNode(TreeListSuimServiceType.FocusedNode);
                V_HIS_SUIM_SERVICE_TYPE dataMediStock = data as V_HIS_SUIM_SERVICE_TYPE;
                if (MessageBox.Show(String.Format(ResourceLanguageManager.BanCoMuonKhoaDuLieu), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    V_HIS_SUIM_SERVICE_TYPE data1 = new V_HIS_SUIM_SERVICE_TYPE();
                    data1.ID = dataMediStock.ID;
                    WaitingManager.Show();
                    hisMediStock = new BackendAdapter(param).Post<V_HIS_SUIM_SERVICE_TYPE>("api/HisSuimServiceType/ChangeLock", ApiConsumers.MosConsumer, data1, param);
                    WaitingManager.Hide();
                    if (hisMediStock != null)
                    {
                        LoaddataToTreeList();
                        notHandler = true;
                    }

                    btnEdit.Enabled = false;
                    MessageManager.Show(this.ParentForm, param, notHandler);
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnUnlock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            V_HIS_SUIM_SERVICE_TYPE hisMediStock = new V_HIS_SUIM_SERVICE_TYPE();
            bool notHandler = false;
            try
            {
                WaitingManager.Show();
                var data = TreeListSuimServiceType.GetDataRecordByNode(TreeListSuimServiceType.FocusedNode);
                V_HIS_SUIM_SERVICE_TYPE dataMediStock = data as V_HIS_SUIM_SERVICE_TYPE;
                if (MessageBox.Show(String.Format(ResourceLanguageManager.MoKhoaDuLieu), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    V_HIS_SUIM_SERVICE_TYPE data1 = new V_HIS_SUIM_SERVICE_TYPE();
                    data1.ID = dataMediStock.ID;
                    WaitingManager.Show();
                    hisMediStock = new BackendAdapter(param).Post<V_HIS_SUIM_SERVICE_TYPE>("api/HisSuimServiceType/ChangeLock", ApiConsumers.MosConsumer, data1, param);
                    WaitingManager.Hide();
                    if (hisMediStock != null)
                    {
                        LoaddataToTreeList();
                        notHandler = true;
                    }
                    btnEdit.Enabled = false;
                    MessageManager.Show(this.ParentForm, param, notHandler);
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                var data = TreeListSuimServiceType.GetDataRecordByNode(TreeListSuimServiceType.FocusedNode);
                V_HIS_SUIM_SERVICE_TYPE rowData = data as V_HIS_SUIM_SERVICE_TYPE;
                if (MessageBox.Show(String.Format(ResourceLanguageManager.BanCoMuonXoaDuLieu), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)

                    if (rowData != null)
                    {
                        bool success = false;
                        CommonParam param = new CommonParam();
                        success = new BackendAdapter(param).Post<bool>("api/HisSuimServiceType/Delete", ApiConsumers.MosConsumer, rowData, param);
                        if (success)
                        {
                            LoaddataToTreeList();
                        }
                        MessageManager.Show(this, param, success);
                    }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtKeyword_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPatientType_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    //if (cboPatientType.EditValue != null)
                    //{
                    if (cboBill.EditValue == null)
                    {
                        cboBill.Focus();
                        cboBill.ShowPopup();
                    }
                    else
                    {
                        cboBill.Focus();
                    }
                    //}
                    //else
                    //{
                    //    cboDVT.ShowPopup();
                    //}
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPatientType_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
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
                        cboBill.Focus();
                        cboBill.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        private void cboPatientType_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
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

        private void cboBill_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboBill.Properties.Buttons[1].Visible = true;
                    cboBill.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDV_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboDV.Properties.Buttons[1].Visible = true;
                    cboDV.EditValue = null;
                }
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
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
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

        private void chkOutPack_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
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

        private void cboLoaiBH_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboLoaiBH.Properties.Buttons[1].Visible = true;
                    cboLoaiBH.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboLoaiBH_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboDV.EditValue == null)
                    {
                        cboDV.Focus();
                        cboDV.ShowPopup();
                    }
                    else
                    {
                        cboDV.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboLoaiBH_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboLoaiBH.EditValue == null)
                    {
                        cboLoaiBH.ShowPopup();
                    }
                    else
                    {
                        if (cboDV.EditValue == null)
                        {
                            cboDV.Focus();
                            cboDV.ShowPopup();
                        }
                        else
                        {
                            cboDV.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
