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
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using Inventec.Desktop.Common.Controls.ValidationRule;
using DevExpress.XtraEditors.DXErrorProvider;
using Inventec.Desktop.Common.LanguageManager;
using System.Resources;
using ACS.EFMODEL.DataModels;
using ACS.Filter;
using DevExpress.XtraEditors.Controls;

namespace ACS.Desktop.Plugins.AcsModule
{
    public partial class frmAcsModule : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        PagingGrid pagingGrid;
        int ActionType = -1;
        int positionHandle = -1;
        ACS_MODULE currentData;
        List<string> arrControlEnableNotChange = new List<string>();
        Dictionary<string, int> dicOrderTabIndexControl = new Dictionary<string, int>();
        List<ACS_MODULE_GROUP> ListModuleGroup = new List<ACS_MODULE_GROUP>();
        Inventec.Desktop.Common.Modules.Module moduleData;
        Action<Type> delegateRefresh;
        #endregion

        #region Construct
        public frmAcsModule(Inventec.Desktop.Common.Modules.Module moduleData, Inventec.Common.WebApiClient.ApiConsumer sdaConsumer, Inventec.Common.WebApiClient.ApiConsumer acsConsumer, Action<Type> delegateRefresh, long numPageSize, string applicationCode, string iconPath, List<ACS_APPLICATION> listAcsApplication)
            : base(moduleData)
        {
            try
            {
                InitializeComponent();

                this.moduleData = moduleData;
                this.delegateRefresh = delegateRefresh;
                ConfigApplications.NumPageSize = numPageSize;
                GlobalVariables.APPLICATION_CODE = applicationCode;
                ApiConsumers.SdaConsumer = sdaConsumer;
                ApiConsumers.AcsConsumer = acsConsumer;
                RamData.acsAppication = listAcsApplication;
                pagingGrid = new PagingGrid();
                this.moduleData = moduleData;
                gridControl1.ToolTipController = toolTipControllerGrid;

                try
                {
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
        private void frmAcsModule_Load(object sender, EventArgs e)
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
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("ACS.Desktop.Plugins.AcsModule.Resources.Lang", typeof(ACS.Desktop.Plugins.AcsModule.frmAcsModule).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("frmAcsModule.layoutControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl7.Text = Inventec.Common.Resource.Get.Value("frmAcsModule.layoutControl7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboApplicationFind.Properties.NullText = Inventec.Common.Resource.Get.Value("frmAcsModule.cboApplicationFind.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmAcsModule.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnSearch.Caption = Inventec.Common.Resource.Get.Value("frmAcsModule.bbtnSearch.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnEdit.Caption = Inventec.Common.Resource.Get.Value("frmAcsModule.bbtnEdit.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnAdd.Caption = Inventec.Common.Resource.Get.Value("frmAcsModule.bbtnAdd.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnReset.Caption = Inventec.Common.Resource.Get.Value("frmAcsModule.bbtnReset.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnFocusDefault.Caption = Inventec.Common.Resource.Get.Value("frmAcsModule.bbtnFocusDefault.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("frmAcsModule.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmAcsModule.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grclSTT.Caption = Inventec.Common.Resource.Get.Value("frmAcsModule.grclSTT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grclLock.Caption = Inventec.Common.Resource.Get.Value("frmAcsModule.grclLock.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("frmAcsModule.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grclLink.Caption = Inventec.Common.Resource.Get.Value("frmAcsModule.grclLink.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grclUrl.Caption = Inventec.Common.Resource.Get.Value("frmAcsModule.grclUrl.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grclIcon.Caption = Inventec.Common.Resource.Get.Value("frmAcsModule.grclIcon.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grclModuleGroup.Caption = Inventec.Common.Resource.Get.Value("frmAcsModule.grclModuleGroup.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grclOrder.Caption = Inventec.Common.Resource.Get.Value("frmAcsModule.grclOrder.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grclApplication.Caption = Inventec.Common.Resource.Get.Value("frmAcsModule.grclApplication.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grclStatus.Caption = Inventec.Common.Resource.Get.Value("frmAcsModule.grclStatus.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn11.Caption = Inventec.Common.Resource.Get.Value("frmAcsModule.gridColumn11.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn12.Caption = Inventec.Common.Resource.Get.Value("frmAcsModule.gridColumn12.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn13.Caption = Inventec.Common.Resource.Get.Value("frmAcsModule.gridColumn13.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn14.Caption = Inventec.Common.Resource.Get.Value("frmAcsModule.gridColumn14.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyword.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmAcsModule.txtKeyword.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem20.Text = Inventec.Common.Resource.Get.Value("frmAcsModule.layoutControlItem20.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcEditorInfo.Text = Inventec.Common.Resource.Get.Value("frmAcsModule.lcEditorInfo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.cboParent.Properties.NullText = Inventec.Common.Resource.Get.Value("frmAcsModule.cboParent.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkIsAnonymous.Properties.Caption = Inventec.Common.Resource.Get.Value("frmAcsModule.chkIsAnonymous.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkIsVisible.Properties.Caption = Inventec.Common.Resource.Get.Value("frmAcsModule.chkIsVisible.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboModuleGroup.Properties.NullText = Inventec.Common.Resource.Get.Value("frmAcsModule.cboModuleGroup.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboApplication.Properties.NullText = Inventec.Common.Resource.Get.Value("frmAcsModule.cboApplication.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("frmAcsModule.btnEdit.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("frmAcsModule.btnAdd.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnRefresh.Text = Inventec.Common.Resource.Get.Value("frmAcsModule.btnRefresh.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciSampleRoomName.Text = Inventec.Common.Resource.Get.Value("frmAcsModule.lciSampleRoomName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem11.Text = Inventec.Common.Resource.Get.Value("frmAcsModule.layoutControlItem11.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem10.Text = Inventec.Common.Resource.Get.Value("frmAcsModule.layoutControlItem10.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem12.Text = Inventec.Common.Resource.Get.Value("frmAcsModule.layoutControlItem12.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem13.Text = Inventec.Common.Resource.Get.Value("frmAcsModule.layoutControlItem13.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem14.Text = Inventec.Common.Resource.Get.Value("frmAcsModule.layoutControlItem14.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem15.Text = Inventec.Common.Resource.Get.Value("frmAcsModule.layoutControlItem15.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem16.Text = Inventec.Common.Resource.Get.Value("frmAcsModule.layoutControlItem16.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem17.Text = Inventec.Common.Resource.Get.Value("frmAcsModule.layoutControlItem17.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.LayoutParent.Text = Inventec.Common.Resource.Get.Value("frmAcsModule.LayoutParent.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem8.Text = Inventec.Common.Resource.Get.Value("frmAcsModule.layoutControlItem8.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem19.Text = Inventec.Common.Resource.Get.Value("frmAcsModule.layoutControlItem19.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmAcsModule.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
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

        private void SetDefaultValue()
        {
            try
            {
                this.ActionType = GlobalVariables.ActionAdd;
                this.currentData = null;
                txtKeyword.Text = "";
                cboApplicationFind.EditValue = null;
                ResetFormData();
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

        private void FillDataToControlsForm()
        {
            try
            {
                InitComboApplication();
                InitComboModuleGroup();
                InitComboParent();
                InnitComboApplicationFind();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region Init combo
        private void InnitComboApplicationFind()
        {
            try
            {
                var data = RamData.acsAppication;
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("APPLICATION_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("APPLICATION_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("APPLICATION_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboApplicationFind, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboApplication()
        {
            try
            {
                var data = RamData.acsAppication;
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("APPLICATION_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("APPLICATION_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("APPLICATION_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboApplication, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboModuleGroup()
        {
            try
            {
                CommonParam param = new CommonParam();
                AcsModuleGroupFilter filter = new AcsModuleGroupFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                var data = new BackendAdapter(param).Get<List<ACS_MODULE_GROUP>>("api/AcsModuleGroup/Get", ApiConsumers.AcsConsumer, filter, null).ToList();
                foreach (var item in data)
                {
                    ListModuleGroup.Add(item);
                }
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("MODULE_GROUP_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("MODULE_GROUP_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("MODULE_GROUP_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboModuleGroup, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboParent(long applicationId = 0, long moduleId = 0)
        {
            try
            {
                try
                {
                    List<ACS_MODULE> datas = null;
                    if (applicationId > 0)
                    {
                        CommonParam param = new CommonParam();
                        AcsModuleFilter filter = new AcsModuleFilter();
                        filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                        filter.APPLICATION_ID = applicationId;
                        datas = new BackendAdapter(param).Get<List<ACS_MODULE>>("api/AcsModule/Get", ApiConsumers.AcsConsumer, filter, null);
                        if (datas != null && datas.Count > 0 && moduleId > 0)
                        {
                            datas = datas.Where(o => o.ID != moduleId).ToList();
                        }
                    }

                    List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                    columnInfos.Add(new ColumnInfo("MODULE_LINK", "", 200, 1, true));
                    columnInfos.Add(new ColumnInfo("MODULE_NAME", "", 350, 2, true));
                    ControlEditorADO controlEditorADO = new ControlEditorADO("MODULE_NAME", "ID", columnInfos, false, 550);
                    ControlEditorLoader.Load(cboParent, datas, controlEditorADO);
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
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
                    numPageSize = (int)ConfigApplications.NumPageSize;
                }

                LoadPaging(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging.Init(LoadPaging, param, numPageSize, this.gridControl1);
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
                CommonParam commonparam = new CommonParam(startPage, limit);
                AcsModuleFilter filter = new AcsModuleFilter();
                filter.KEY_WORD = txtKeyword.Text.Trim();
                if (cboApplicationFind.EditValue != null)
                {
                    filter.APPLICATION_ID = Convert.ToInt64(cboApplicationFind.EditValue);
                }
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.ORDER_DIRECTION = "DESC";
                var currentDataStore = new BackendAdapter(commonparam).GetRO<List<ACS_MODULE>>(AcsRequestUriStore.ACS_MODULE_GET, ApiConsumers.AcsConsumer, filter, commonparam);
                if (currentDataStore != null)
                {
                    var data = (List<ACS_MODULE>)currentDataStore.Data;
                    gridControl1.DataSource = data;
                    rowCount = (data == null ? 0 : data.Count);
                    dataTotal = (currentDataStore.Param == null ? 0 : currentDataStore.Param.Count ?? 0);
                }
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
                    gridControl1.Focus();
                    gridView2.FocusedRowHandle = 0;
                    var rowData = (ACS_MODULE)gridView2.GetFocusedRow();
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
                    gridControl1.Focus();
                    gridView2.FocusedRowHandle = 0;
                    var rowData = (ACS_MODULE)gridView2.GetFocusedRow();
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

        private void gridControlFormList_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                var rowData = (ACS_MODULE)gridView2.GetFocusedRow();
                if (rowData != null)
                {
                    this.currentData = rowData;
                    ChangedDataRow(rowData);
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
                    var rowData = (ACS_MODULE)gridView2.GetFocusedRow();
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

        private void ChangedDataRow(ACS_MODULE data)
        {
            try
            {
                if (data != null)
                {
                    InitComboParent(data.APPLICATION_ID, data.ID);
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

        private void FillDataToEditorControl(ACS_MODULE data)
        {
            try
            {
                if (data != null)
                {
                    txtModuleName.EditValue = data.MODULE_NAME;
                    cboApplication.EditValue = data.APPLICATION_ID;
                    txtIcon.EditValue = data.ICON_LINK;
                    cboModuleGroup.EditValue = data.MODULE_GROUP_ID;
                    txtLink.EditValue = data.MODULE_LINK;
                    txtUrl.EditValue = data.MODULE_URL;
                    txtVideourl.EditValue = data.VIDEO_URLS;
                    if (data.IS_VISIBLE == 1)
                    {
                        chkIsVisible.Checked = true;
                        chkIsNotShowDialog.Enabled = true;
                    }
                    else
                    {
                        chkIsVisible.Checked = false;
                        chkIsNotShowDialog.Enabled = false;
                    }
                    if (data.IS_ANONYMOUS == 1)
                    {
                        chkIsAnonymous.Checked = true;
                    }
                    else
                    {
                        chkIsAnonymous.Checked = false;
                    }
                    if (data.IS_NOT_SHOW_DIALOG == 1)
                    {
                        chkIsNotShowDialog.Checked = true;
                    }
                    else
                    {
                        chkIsNotShowDialog.Checked = false;
                    }

                    spNum.EditValue = data.NUM_ORDER;
                    cboParent.EditValue = data.PARENT_ID;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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
                    cboParent.EditValue = null;
                    txtModuleName.Focus();
                    txtModuleName.SelectAll();
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

        private void LoadCurrent(long currentId, ref ACS_MODULE currentDTO)
        {
            try
            {
                CommonParam param = new CommonParam();
                AcsModuleFilter filter = new AcsModuleFilter();
                filter.ID = currentId;
                currentDTO = new BackendAdapter(param).Get<List<ACS_MODULE>>(AcsRequestUriStore.ACS_MODULE_GET, ApiConsumers.AcsConsumer, filter, param).FirstOrDefault();
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
                FillDataToGridControl();
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
                ACS_MODULE updateDTO = new ACS_MODULE();

                if (this.currentData != null && this.currentData.ID > 0)
                {
                    LoadCurrent(this.currentData.ID, ref updateDTO);
                }

                UpdateDTOFromDataForm(ref updateDTO);

                if (ActionType == GlobalVariables.ActionAdd)
                {
                    updateDTO.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    var resultData = new BackendAdapter(param).Post<ACS_MODULE>(AcsRequestUriStore.ACS_MODULE_CREATE, ApiConsumers.AcsConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;
                        FillDataToGridControl();
                        ResetFormData();
                    }
                }
                else
                {
                    var resultData = new BackendAdapter(param).Post<ACS_MODULE>(AcsRequestUriStore.ACS_MODULE_UPDATE, ApiConsumers.AcsConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;
                        FillDataToGridControl();
                    }
                }

                if (success)
                {
                    if (this.delegateRefresh != null)
                        this.delegateRefresh(new ACS_MODULE().GetType());
                }

                WaitingManager.Hide();

                #region Hien thi message thong bao
                MessageManager.Show(this, param, success);
                #endregion

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateDTOFromDataForm(ref ACS_MODULE currentDTO)
        {
            try
            {
                currentDTO.MODULE_NAME = txtModuleName.Text.Trim();
                currentDTO.APPLICATION_ID = Convert.ToInt64(cboApplication.EditValue);
                currentDTO.ICON_LINK = txtIcon.Text.Trim();
                if (cboModuleGroup.EditValue != null)
                {
                    currentDTO.MODULE_GROUP_ID = Convert.ToInt64(cboModuleGroup.EditValue);
                }
                else
                {
                    currentDTO.MODULE_GROUP_ID = null;
                }

                currentDTO.MODULE_LINK = txtLink.Text.Trim();
                currentDTO.MODULE_URL = txtUrl.Text.Trim();
                currentDTO.VIDEO_URLS = txtVideourl.Text.Trim();
                if (chkIsVisible.Checked)
                {
                    currentDTO.IS_VISIBLE = 1;
                }
                else
                {
                    currentDTO.IS_VISIBLE = null;
                }

                if (chkIsNotShowDialog.Checked)
                {
                    currentDTO.IS_NOT_SHOW_DIALOG = 1;
                }
                else
                {
                    currentDTO.IS_NOT_SHOW_DIALOG = null;
                }

                if (chkIsAnonymous.Checked)
                {
                    currentDTO.IS_ANONYMOUS = 1;
                }
                else
                {
                    currentDTO.IS_ANONYMOUS = null;
                }

                if (cboParent.EditValue != null)
                {
                    currentDTO.PARENT_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboParent.EditValue.ToString());
                }
                else
                    currentDTO.PARENT_ID = null;

                if (spNum.EditValue != null)
                {
                    currentDTO.NUM_ORDER = Convert.ToInt64(spNum.EditValue);
                }
                else
                    currentDTO.NUM_ORDER = null;
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
                ValidationSingleControl(txtModuleName);
                ValidationSingleControl(cboApplication);
                ValidationSingleControl(txtApplicationCode);
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
                validRule.ErrorText = "Trường dữ liệu bắt buộc";
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
                validRule.ErrorText = "Trường dữ liệu bắt buộc";
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
                FillDataToGridControl();

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

        private void unLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            ACS_MODULE success = new ACS_MODULE();
            bool notHandler = false;
            try
            {
                var rowData = (ACS_MODULE)gridView2.GetFocusedRow();
                if (MessageBox.Show("Bạn có muốn khóa dữ liệu không", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    ACS_MODULE data1 = new ACS_MODULE();
                    data1.ID = rowData.ID;
                    WaitingManager.Show();
                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<ACS_MODULE>(AcsRequestUriStore.ACS_MODULE_CHANGELOCK, ApiConsumers.AcsConsumer, data1, param);
                    WaitingManager.Hide();
                    if (success != null)
                    {
                        notHandler = true;
                        FillDataToGridControl();
                    }
                    MessageManager.Show(this, param, notHandler);
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
            ACS_MODULE success = new ACS_MODULE();
            bool notHandler = false;
            try
            {
                var rowData = (ACS_MODULE)gridView2.GetFocusedRow();
                if (MessageBox.Show("Bạn có muốn bỏ khóa dữ liệu không", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    ACS_MODULE data1 = new ACS_MODULE();
                    data1.ID = rowData.ID;
                    WaitingManager.Show();
                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<ACS_MODULE>(AcsRequestUriStore.ACS_MODULE_CHANGELOCK, ApiConsumers.AcsConsumer, data1, param);
                    WaitingManager.Hide();
                    if (success != null)
                    {
                        notHandler = true;
                        FillDataToGridControl();
                    }
                    MessageManager.Show(this, param, notHandler);
                }

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridviewFormList_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    ACS_MODULE data = (ACS_MODULE)((IList)((BaseView)sender).DataSource)[e.RowHandle];
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

        #region focus
        private void txtSampleRoomCode_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtIcon.Focus();
                    txtIcon.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void txtBedCode_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtApplicationCode.Focus();
                    txtApplicationCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridControl1_NodeCellStyle(object sender, DevExpress.XtraTreeList.GetCustomNodeCellStyleEventArgs e)
        {
            try
            {
                var data = (ACS_MODULE)gridView2.GetFocusedRow();
                if (data != null && data is ACS_MODULE)
                {
                    ACS_MODULE rowData = data as ACS_MODULE;
                    if (rowData == null) return;

                    else if (e.Column.FieldName == "IS_ACTIVE_STR")
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

        private void cboApplication_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    txtIcon.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboApplication_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtIcon.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtIcon_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtModuleGroupCode.Focus();
                    txtModuleGroupCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtModuleGroupCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (!String.IsNullOrEmpty(txtModuleGroupCode.Text))
                {
                    string key = Inventec.Common.String.Convert.UnSignVNese(this.txtModuleGroupCode.Text.ToLower().Trim());
                    var data = ListModuleGroup.Where(o => Inventec.Common.String.Convert.UnSignVNese(o.MODULE_GROUP_CODE.ToLower()).Contains(key)).ToList();
                    List<ACS_MODULE_GROUP> result = (data != null ? ((data.Count == 1) ? data : data.Where(o => o.MODULE_GROUP_CODE.ToLower() == txtModuleGroupCode.Text).ToList()) : null);
                    if (result != null && result.Count == 1)
                    {
                        cboModuleGroup.EditValue = result[0].ID;
                        txtModuleGroupCode.Text = result[0].MODULE_GROUP_CODE;
                        cboModuleGroup.Focus();
                        txtLink.Focus();
                    }
                    else
                    {
                        cboModuleGroup.EditValue = null;
                        cboModuleGroup.ShowPopup();
                        cboModuleGroup.Focus();
                    }
                }
                else
                {
                    cboModuleGroup.EditValue = null;
                    cboModuleGroup.Focus();
                    cboModuleGroup.ShowPopup();
                }
            }
        }

        private void cboModuleGroup_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    txtLink.Focus();
                    txtLink.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboModuleGroup_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtLink.Focus();
                    txtLink.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtLink_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtUrl.Focus();
                    txtUrl.SelectAll();
                    txtVideourl.Focus();
                    txtVideourl.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtUrl_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkIsVisible.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        private void cboApplicationFind_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    FillDataToGridControl();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView2_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    ACS_MODULE data = (ACS_MODULE)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (e.Column.FieldName == "IS_LOCK")
                    {
                        e.RepositoryItem = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? unLock : Lock);

                    }
                    else if (e.Column.FieldName == "IS_Anonymous")
                    {
                        if (data.IS_ANONYMOUS == 1)
                        {
                            e.RepositoryItem = btnIsAnonymous;
                        }
                    }
                    else if (e.Column.FieldName == "DELETE")
                    {
                        e.RepositoryItem = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? btnDelete : btnDisDelete);
                    }
                    else if (e.Column.FieldName == "IS_MENU")
                    {
                        if (data.IS_VISIBLE == 1)
                        {
                            e.RepositoryItem = btnIsMenu;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView2_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    ACS_MODULE pData = (ACS_MODULE)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    short status = Inventec.Common.TypeConvert.Parse.ToInt16((pData.IS_ACTIVE ?? -1).ToString());
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + startPage;
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
                    else if (e.Column.FieldName == "IS_NOT_SHOW_DIALOG_OBJ")
                    {
                        e.Value = pData.IS_NOT_SHOW_DIALOG == 1 ? true : false;
                    }
                    else if (e.Column.FieldName == "MODULE_GROUP_NAME")
                    {
                        ACS_MODULE_GROUP group = ListModuleGroup != null ? ListModuleGroup.FirstOrDefault(o => o.ID == pData.MODULE_GROUP_ID) : null;
                        e.Value = group != null ? group.MODULE_GROUP_NAME : "";
                    }
                    else if (e.Column.FieldName == "APPLICATION_NAME")
                    {
                        ACS_APPLICATION app = RamData.acsAppication != null ? RamData.acsAppication.FirstOrDefault(o => o.ID == pData.APPLICATION_ID) : null;
                        e.Value = app != null ? app.APPLICATION_NAME : "";
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboApplicationFind_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboApplicationFind.Properties.Buttons[1].Visible = true;
                    cboApplicationFind.EditValue = null;
                    FillDataToGridControl();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("Bạn có muốn xóa dữ liệu không", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    var rowData = (ACS_MODULE)gridView2.GetFocusedRow();
                    if (rowData != null)
                    {
                        bool success = false;
                        CommonParam param = new CommonParam();
                        success = new BackendAdapter(param).Post<bool>(AcsRequestUriStore.ACS_MODULE_DELETE, ApiConsumers.AcsConsumer, rowData, param);
                        if (success)
                        {
                            FillDataToGridControl();
                            btnRefesh_Click(null, null);
                            currentData = ((List<ACS_MODULE>)gridView2.DataSource).FirstOrDefault();
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

        private void chkIsVisible_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkIsNotShowDialog.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkIsAnonymous_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spNum.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtUrl_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkIsVisible.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spNum_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (ActionType == GlobalVariables.ActionEdit)
                    {
                        btnEdit.Focus();
                    }
                    else
                    {
                        btnAdd.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboModuleGroup_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboModuleGroup.EditValue = null;
                    txtModuleGroupCode.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtApplicationCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrEmpty(txtApplicationCode.Text))
                    {
                        string key = Inventec.Common.String.Convert.UnSignVNese(this.txtApplicationCode.Text.ToLower().Trim());
                        var data = RamData.acsAppication.Where(o => Inventec.Common.String.Convert.UnSignVNese(o.APPLICATION_CODE.ToLower()).Contains(key)).ToList();

                        List<ACS_APPLICATION> result = (data != null ? ((data.Count == 1) ? data : data.Where(o => o.APPLICATION_CODE.ToLower() == txtApplicationCode.Text).ToList()) : null);
                        if (result != null && result.Count == 1)
                        {
                            cboApplication.EditValue = result[0].ID;
                            txtApplicationCode.Text = result[0].APPLICATION_CODE;
                            cboApplication.Focus();
                            txtIcon.Focus();
                        }
                        else
                        {
                            cboApplication.EditValue = null;
                            cboApplication.Focus();
                            cboApplication.ShowPopup();
                        }
                    }
                    else
                    {
                        cboApplication.EditValue = null;
                        cboApplication.Focus();
                        cboApplication.ShowPopup();

                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtVideourl_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkIsVisible.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtVideourl_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkIsVisible.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkIsNotShowDialog_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkIsAnonymous.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkIsVisible_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkIsVisible.Checked)
                {
                    chkIsNotShowDialog.Enabled = true;
                }
                else
                {
                    chkIsNotShowDialog.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboParent_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboParent.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboParent_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                cboParent.Properties.Buttons[1].Visible = (cboParent.EditValue != null && !String.IsNullOrEmpty(cboParent.Text));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboParent_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Control && e.KeyCode == Keys.A)
                {
                    this.cboParent.Focus();
                    this.cboParent.SelectAll();
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    //e.Handled = true;
                }
                else
                {
                    this.cboParent.ShowPopup();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboParent_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(cboParent.Text))
                {
                    cboParent.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboApplication_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboApplication.EditValue != null)
                {
                    InitComboParent(Inventec.Common.TypeConvert.Parse.ToInt64(cboApplication.EditValue.ToString()));
                    ACS_APPLICATION gt = RamData.acsAppication.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboApplication.EditValue.ToString()));
                    if (gt != null)
                    {
                        txtApplicationCode.Text = gt.APPLICATION_CODE;
                    }
                    else
                    {
                        txtApplicationCode.Text = "";
                    }
                }
                else
                {
                    InitComboParent();
                    txtApplicationCode.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboModuleGroup_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboModuleGroup.EditValue != null)
                {
                    ACS_MODULE_GROUP gt = ListModuleGroup.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboModuleGroup.EditValue.ToString()));
                    if (gt != null)
                    {
                        txtModuleGroupCode.Text = gt.MODULE_GROUP_CODE;
                    }
                }
                else
                {
                    txtModuleGroupCode.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
