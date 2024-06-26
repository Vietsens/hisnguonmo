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
using ACS.EFMODEL.DataModels;
using ACS.Filter;
using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.LibraryMessage;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using Inventec.UC.Paging;
using Inventec.VoiceCommand;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Vva.Desktop.Plugins.VvaVoiceCommand.Resources;
using VVA.Desktop.Plugins.VvaVoiceCommand.ADO;
using VVA.Desktop.Plugins.VvaVoiceCommand.Validation;
using VVA.EFMODEL.DataModels;
using VVA.Filter;

namespace Vva.Desktop.Plugins.VvaVoiceCommand.VvaVoiceCommand
{
    public partial class frmVvaVoiceCommand : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        PagingGrid pagingGrid;
        int ActionType = -1;
        int positionHandle = -1;
        VVA_VOICE_COMMAND currentData;
        List<string> arrControlEnableNotChange = new List<string>();
        Dictionary<string, int> dicOrderTabIndexControl = new Dictionary<string, int>();
        Inventec.Desktop.Common.Modules.Module moduleData;
        Action<Type> delegateRefresh;

        Action ActionRefresh;

        List<VVA_COMMAND_TYPE> CommandTypeList;
        List<ACS_APPLICATION> AcsApplicationList;

        List<ListObject> lstVoiceText;

        #endregion

        #region Construct

        public frmVvaVoiceCommand(Inventec.Desktop.Common.Modules.Module moduleData, Inventec.Common.WebApiClient.ApiConsumer VvaConsumer, Inventec.Common.WebApiClient.ApiConsumer AcsConsumer, Action actionRefresh, long numPageSize, string applicationCode, string iconPath)
            : base(moduleData)
        {
            try
            {
                InitializeComponent();
                this.moduleData = moduleData;
                // this.delegateRefresh = delegateRefresh;
                this.ActionRefresh = actionRefresh;
                ConfigApplications.NumPageSize = numPageSize;
                GlobalVariables.APPLICATION_CODE = applicationCode;
                ApiConsumers.VvaConsumer = VvaConsumer;
                ApiConsumers.AcsConsumer = AcsConsumer;
                pagingGrid = new PagingGrid();
                gridControl1.ToolTipController = toolTipControllerGrid;

                try
                {
                    this.Icon = Icon.ExtractAssociatedIcon(iconPath);
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

        #region Private method
        private void frmVvaVoiceCommand_Load(object sender, EventArgs e)
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
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("VVA.Desktop.Plugins.VvaVoiceCommand.Resources.Lang", typeof(Vva.Desktop.Plugins.VvaVoiceCommand.VvaVoiceCommand.frmVvaVoiceCommand).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frmVvaVoiceCommand.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("frmVvaVoiceCommand.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("frmVvaVoiceCommand.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("frmVvaVoiceCommand.btnAdd.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("frmVvaVoiceCommand.btnEdit.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnReset.Text = Inventec.Common.Resource.Get.Value("frmVvaVoiceCommand.btnReset.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnSearch.Caption = Inventec.Common.Resource.Get.Value("frmVvaVoiceCommand.bbtnSearch.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnEdit.Caption = Inventec.Common.Resource.Get.Value("frmVvaVoiceCommand.bbtnEdit.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnAdd.Caption = Inventec.Common.Resource.Get.Value("frmVvaVoiceCommand.bbtnAdd.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnReset.Caption = Inventec.Common.Resource.Get.Value("frmVvaVoiceCommand.bbtnReset.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnFocusDefault.Caption = Inventec.Common.Resource.Get.Value("frmVvaVoiceCommand.bbtnFocusDefault.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmVvaVoiceCommand.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.ToolTip = Inventec.Common.Resource.Get.Value("frmVvaVoiceCommand.gridColumn1.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("frmVvaVoiceCommand.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.ToolTip = Inventec.Common.Resource.Get.Value("frmVvaVoiceCommand.gridColumn2.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("frmVvaVoiceCommand.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.ToolTip = Inventec.Common.Resource.Get.Value("frmVvaVoiceCommand.gridColumn3.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("frmVvaVoiceCommand.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.ToolTip = Inventec.Common.Resource.Get.Value("frmVvaVoiceCommand.gridColumn4.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("frmVvaVoiceCommand.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.ToolTip = Inventec.Common.Resource.Get.Value("frmVvaVoiceCommand.gridColumn5.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("frmVvaVoiceCommand.gridColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn6.ToolTip = Inventec.Common.Resource.Get.Value("frmVvaVoiceCommand.gridColumn6.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("frmVvaVoiceCommand.gridColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn7.ToolTip = Inventec.Common.Resource.Get.Value("frmVvaVoiceCommand.gridColumn7.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn8.Caption = Inventec.Common.Resource.Get.Value("frmVvaVoiceCommand.gridColumn8.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn8.ToolTip = Inventec.Common.Resource.Get.Value("frmVvaVoiceCommand.gridColumn8.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn10.Caption = Inventec.Common.Resource.Get.Value("frmVvaVoiceCommand.gridColumn10.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn10.ToolTip = Inventec.Common.Resource.Get.Value("frmVvaVoiceCommand.gridColumn10.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn11.Caption = Inventec.Common.Resource.Get.Value("frmVvaVoiceCommand.gridColumn11.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn11.ToolTip = Inventec.Common.Resource.Get.Value("frmVvaVoiceCommand.gridColumn11.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn12.Caption = Inventec.Common.Resource.Get.Value("frmVvaVoiceCommand.gridColumn12.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn12.ToolTip = Inventec.Common.Resource.Get.Value("frmVvaVoiceCommand.gridColumn12.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn13.Caption = Inventec.Common.Resource.Get.Value("frmVvaVoiceCommand.gridColumn13.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn13.ToolTip = Inventec.Common.Resource.Get.Value("frmVvaVoiceCommand.gridColumn13.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());


                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmVvaVoiceCommand.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyword.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmVvaVoiceCommand.txtKeyword.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.layoutControlItem6.Text = Inventec.Common.Resource.Get.Value("frmVvaVoiceCommand.layoutControlItem6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem7.Text = Inventec.Common.Resource.Get.Value("frmVvaVoiceCommand.layoutControlItem7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.groupBox1.Text = Inventec.Common.Resource.Get.Value("frmVvaVoiceCommand.groupBox1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem13.Text = Inventec.Common.Resource.Get.Value("frmVvaVoiceCommand.layoutControlItem13.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem14.Text = Inventec.Common.Resource.Get.Value("frmVvaVoiceCommand.layoutControlItem14.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.layoutControlItem15.Text = Inventec.Common.Resource.Get.Value("frmVvaVoiceCommand.layoutControlItem15.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());


                this.Text = Inventec.Common.Resource.Get.Value("frmVvaVoiceCommand.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

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
                txtKeyword.Text = "";
                lstVoiceText = new List<ListObject>();
                cboAppCode.EditValue = null;
                cboCommandType.EditValue = null;
                cboModuleLink.EditValue = null;
                txtCommandAction.Text = "";
                ResetFormData();
                EnableControlChanged(this.ActionType);

                gridView2.BeginUpdate();
                gridView2.GridControl.DataSource = null;
                gridView2.EndUpdate();

                txtVoiceText.Tag = (Action<ResultCommandADO>)ProcessResultCommand;//thay txtvoiceText bằng ô text văn bản thoại
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void ProcessResultCommand(ResultCommandADO resultCommand)
        {
            try
            {
                //lấy text từ resultCommand.text để thêm dòng vào gridcontrol2
                txtVoiceText.Text = resultCommand.text;
                btnAddcboVoiceText_Click(null, null);
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

        private void ResetFormData()
        {
            try
            {
                if (!layoutControl3.IsInitialized) return;
                layoutControl3.BeginUpdate();
                try
                {
                    foreach (DevExpress.XtraLayout.BaseLayoutItem item in layoutControl3.Items)
                    {
                        DevExpress.XtraLayout.LayoutControlItem lci = item as DevExpress.XtraLayout.LayoutControlItem;
                        if (lci != null && lci.Control != null && lci.Control is BaseEdit)
                        {
                            DevExpress.XtraEditors.BaseEdit fomatFrm = lci.Control as DevExpress.XtraEditors.BaseEdit;
                            fomatFrm.ResetText();
                            fomatFrm.EditValue = null;
                        }
                    }
                    cboAppCode.Focus();
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                finally
                {
                    layoutControl3.EndUpdate();
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
                LoadCboCommandType();
                LoadCboAppCode();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadCboModuleLink()
        {
            try
            {
                CommonParam param = new CommonParam();
                AcsModuleViewFilter filter = new AcsModuleViewFilter();
                filter.APPLICATION_ID = AcsApplicationList.Where(o => o.APPLICATION_CODE == cboAppCode.EditValue.ToString()).First().ID;
                var ModuleLinklist = new BackendAdapter(param).Get<List<ACS_MODULE>>("/api/AcsModule/Get", ApiConsumers.AcsConsumer, filter, param);

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("MODULE_LINK", "", 300, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("MODULE_LINK", "MODULE_LINK", columnInfos, false, 300);
                ControlEditorLoader.Load(this.cboModuleLink, ModuleLinklist, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadCboAppCode()
        {
            try
            {
                AcsApplicationList = new List<ACS_APPLICATION>();
                CommonParam param = new CommonParam();
                AcsApplicationViewFilter filter = new AcsApplicationViewFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                AcsApplicationList = new BackendAdapter(param).Get<List<ACS_APPLICATION>>("api/AcsApplication/Get", ApiConsumers.AcsConsumer, filter, param);

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("APPLICATION_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("APPLICATION_NAME", "", 200, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("APPLICATION_NAME", "APPLICATION_CODE", columnInfos, false, 300);
                ControlEditorLoader.Load(this.cboAppCode, AcsApplicationList, controlEditorADO);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
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
                //TODO
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region Init combo

        private void LoadCboCommandType()
        {
            try
            {
                CommandTypeList = new List<VVA_COMMAND_TYPE>();
                CommonParam param = new CommonParam();
                VvaCommandTypeFilter filter = new VvaCommandTypeFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                CommandTypeList = new BackendAdapter(param).Get<List<VVA_COMMAND_TYPE>>("/api/VvaCommandType/Get", ApiConsumers.VvaConsumer, filter, param);

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("COMMAND_TYPE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("COMMAND_TYPE_NAME", "", 200, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("COMMAND_TYPE_NAME", "ID", columnInfos, false, 300);
                ControlEditorLoader.Load(this.cboCommandType, CommandTypeList, controlEditorADO);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
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
                if (ucPaging1.pagingGrid != null)
                {
                    numPageSize = ucPaging1.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = (int)ConfigApplications.NumPageSize;
                }

                LoadPaging(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(LoadPaging, param, numPageSize);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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
                Inventec.Core.ApiResultObject<List<VVA_VOICE_COMMAND>> apiResult = null;
                VvaVoiceCommandFilter filter = new VvaVoiceCommandFilter();

                SetFilterNavBar(ref filter);
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                gridView1.BeginUpdate();
                apiResult = new BackendAdapter(paramCommon).GetRO<List<VVA_VOICE_COMMAND>>(VvaRequestUriStore.VVA_VOICE_COMMAND_GET, ApiConsumers.VvaConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    var data = (List<VVA_VOICE_COMMAND>)apiResult.Data;
                    if (data != null)
                    {
                        gridView1.GridControl.DataSource = data;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                }
                gridView1.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetFilterNavBar(ref VvaVoiceCommandFilter filter)
        {
            try
            {
                filter.KEY_WORD = txtKeyword.Text.Trim();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ChangedDataRow(VVA_VOICE_COMMAND data)
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

        private void FillDataToEditorControl(VVA_VOICE_COMMAND data)
        {
            try
            {
                if (data != null)
                {
                    cboAppCode.EditValue = data.APP_CODE;
                    cboModuleLink.EditValue = data.MODULE_LINK;
                    txtCommandAction.EditValue = data.COMMAND_ACTION;
                    cboCommandType.EditValue = data.COMMAND_TYPE;
                    loadcboVoicetext(data.VOICE_TEXT);
                    txtVoiceText.EditValue = null;
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

        private void LoadCurrent(long currentId, ref VVA_VOICE_COMMAND currentDTO)
        {
            try
            {
                CommonParam param = new CommonParam();
                VvaVoiceCommandFilter filter = new VvaVoiceCommandFilter();
                filter.ID = currentId;
                currentDTO = new BackendAdapter(param).Get<List<VVA_VOICE_COMMAND>>(VvaRequestUriStore.VVA_VOICE_COMMAND_GET, ApiConsumers.VvaConsumer, filter, param).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region Validate
        private void ValidateForm()
        {
            ValidationSingleControl(cboAppCode);
            ValidationSingleControl(cboCommandType);

            ValidationSingleControl(txtCommandAction);
            ValidationVoiceText();
        }

        private void ValidationSingleControl(BaseEdit control)
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = control;
                validRule.ErrorText = MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditorInfo.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationVoiceText()
        {
            try
            {
                VoiceTextValidationRule validRule = new VoiceTextValidationRule();
                validRule.gridview = gridView2;
                validRule.ErrorText = MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditorInfo.SetValidationRule(txtVoiceText, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
       
        #endregion

        #region Tooltip
        private void toolTipControllerGrid_GetActiveObjectInfo(object sender, DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventArgs e)
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

        #region Button handler

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
                VVA_VOICE_COMMAND updateDTO = new VVA_VOICE_COMMAND();
                if (this.currentData != null && this.currentData.ID > 0)
                {
                    LoadCurrent(this.currentData.ID, ref updateDTO);
                }
                UpdateDTOFromDataForm(ref updateDTO);

                if (ActionType == GlobalVariables.ActionAdd)
                {
                    updateDTO.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    var resultData = new BackendAdapter(param).Post<VVA_VOICE_COMMAND>(VvaRequestUriStore.VVA_VOICE_COMMAND_CREATE, ApiConsumers.VvaConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;
                        FillDataToGridControl();
                        ResetFormData();
                    }
                }
                else
                {
                    //sdo.HisRoom.ID = currentData.ROOM_ID;
                    var resultData = new BackendAdapter(param).Post<VVA_VOICE_COMMAND>(VvaRequestUriStore.VVA_VOICE_COMMAND_UPDATE, ApiConsumers.VvaConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;
                        FillDataToGridControl();
                    }
                }

                if (success)
                {
                    if (this.delegateRefresh != null)
                        this.delegateRefresh(new VVA_VOICE_COMMAND().GetType());
                    SetFocusEditor();
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

        private void UpdateDTOFromDataForm(ref VVA_VOICE_COMMAND currentDTO)
        {
            try
            {
                currentDTO.APP_CODE = cboAppCode.EditValue.ToString();
                if (cboModuleLink.EditValue != null && cboModuleLink.EditValue != "")
                {
                    currentDTO.MODULE_LINK = cboModuleLink.EditValue.ToString();
                }
                else
                {
                    currentDTO.MODULE_LINK = null;
                }

                currentDTO.VOICE_TEXT = VoiceTextTXT();
                currentDTO.COMMAND_ACTION = txtCommandAction.Text.Trim();
                currentDTO.COMMAND_TYPE = (long)cboCommandType.EditValue;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            try
            {
                SetDefaultValue();
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError
                (dxValidationProviderEditorInfo, dxErrorProvider);
                //FillDataToGridControl();                
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

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


        private void btnGLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            VVA_VOICE_COMMAND success = new VVA_VOICE_COMMAND();
            //bool notHandler = false;
            try
            {

                VVA_VOICE_COMMAND data = (VVA_VOICE_COMMAND)gridView1.GetFocusedRow();
                if (MessageBox.Show(ResourceMessage.BanCoMuonBoKhoaDuLieu, "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    VVA_VOICE_COMMAND data1 = new VVA_VOICE_COMMAND();
                    data1.ID = data.ID;
                    WaitingManager.Show();
                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<VVA_VOICE_COMMAND>(VvaRequestUriStore.VVA_VOICE_COMMAND_UNLOCK, ApiConsumers.VvaConsumer, data1, param);
                    WaitingManager.Hide();
                    if (success != null)
                    {
                        if (this.delegateRefresh != null)
                            this.delegateRefresh(new VVA_VOICE_COMMAND().GetType());
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

        private void btnGUnLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            VVA_VOICE_COMMAND success = new VVA_VOICE_COMMAND();
            try
            {

                VVA_VOICE_COMMAND data = (VVA_VOICE_COMMAND)gridView1.GetFocusedRow();
                if (MessageBox.Show(ResourceMessage.BanCoMuonKhoaDuLieu, "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    VVA_VOICE_COMMAND data1 = new VVA_VOICE_COMMAND();
                    data1.ID = data.ID;
                    WaitingManager.Show();
                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<VVA_VOICE_COMMAND>(VvaRequestUriStore.VVA_VOICE_COMMAND_LOCK, ApiConsumers.VvaConsumer, data1, param);
                    WaitingManager.Hide();
                    if (success != null)
                    {
                        if (this.delegateRefresh != null)
                            this.delegateRefresh(new VVA_VOICE_COMMAND().GetType());
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

        private void btnGDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (MessageBox.Show(ResourceMessage.BanCoMuonXoaDuLieu, "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    var rowData = (VVA_VOICE_COMMAND)gridView1.GetFocusedRow();
                    if (rowData != null)
                    {
                        bool success = false;
                        CommonParam param = new CommonParam();
                        success = new BackendAdapter(param).Post<bool>(VvaRequestUriStore.VVA_VOICE_COMMAND_DELETE, ApiConsumers.VvaConsumer, rowData.ID, param);
                        if (success)
                        {
                            FillDataToGridControl();
                            currentData = ((List<VVA_VOICE_COMMAND>)gridControl1.DataSource).FirstOrDefault();
                            if (this.delegateRefresh != null)
                                this.delegateRefresh(new VVA_VOICE_COMMAND().GetType());
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
                btnEdit_Click(null, null);
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
                btnReset_Click(null, null);
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


        #endregion


        private void txtVoiceText_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.Handled = true;
                    e.SuppressKeyPress = false;
                    btnAddcboVoiceText_Click(null, null);

                    txtVoiceText.Focus();
                    txtVoiceText.SelectAll();
                    //txtCommandAction.Focus();
                    //txtCommandAction.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtCommandAction_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.Handled = true;
                    e.SuppressKeyPress = false;
                    cboCommandType.Focus();
                    cboCommandType.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboCommandType_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    //txtGroupCode.Focus();
                    //txtGroupCode.SelectAll();
                    if (e.KeyCode == Keys.Enter)
                    {
                        if (ActionType == GlobalVariables.ActionEdit)
                        {
                            btnEdit.Focus();
                        }
                        if (ActionType == GlobalVariables.ActionAdd)
                        {
                            btnAdd.Focus();
                        }
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
                    gridView1.Focus();
                    gridView1.FocusedRowHandle = 0;
                    var rowData = (VVA_VOICE_COMMAND)gridView1.GetFocusedRow();
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

        private void gridControl1_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                var rowData = (VVA_VOICE_COMMAND)gridView1.GetFocusedRow();
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

        private void gridView1_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {

                    VVA_VOICE_COMMAND data = (VVA_VOICE_COMMAND)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (e.Column.FieldName == "LOCK")
                    {
                        e.RepositoryItem = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? btnGUnLock : btnGLock);

                    }

                    if (e.Column.FieldName == "Delete")
                    {
                        e.RepositoryItem = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? btnGEnable : btnGDelete);

                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView1_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    VVA_VOICE_COMMAND pData = (VVA_VOICE_COMMAND)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    short status = Inventec.Common.TypeConvert.Parse.ToInt16((pData.IS_ACTIVE ?? -1).ToString());
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + startPage; //+ ((pagingGrid.CurrentPage - 1) * pagingGrid.PageSize);
                    }
                    else if (e.Column.FieldName == "COMMAND_TYPE_STR")
                    {
                        try
                        {
                            e.Value = CommandTypeList.Where(o => o.ID == pData.COMMAND_TYPE).First().COMMAND_TYPE_NAME ?? null;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao COMMAND_TYPE", ex);
                        }
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

                    gridControl1.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView1_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    var rowData = (VVA_VOICE_COMMAND)gridView1.GetFocusedRow();
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


        private void gridView1_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    GridView view = sender as GridView;
                    GridHitInfo hi = view.CalcHitInfo(e.Location);
                    if (hi.InRowCell)
                    {
                        var row = (VVA_VOICE_COMMAND)gridView1.GetRow(hi.RowHandle);
                        if (hi.Column.FieldName == "VOICE_TEXT_STR")
                        {
                            loadcboVoicetext(row.VOICE_TEXT);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void loadcboVoicetext(string VoiceText)
        {
            var voiceTextList = VoiceText.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            lstVoiceText = new List<ListObject>();
            foreach (var item in voiceTextList)
            {
                ListObject voice = new ListObject();
                voice.voiceTextlst = item;
                lstVoiceText.Add(voice);
            }

            gridView2.BeginUpdate();
            gridView2.GridControl.DataSource = null;
            gridView2.GridControl.DataSource = lstVoiceText;
            gridView2.EndUpdate();
        }

        private void btnAddcboVoiceText_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtVoiceText.Text))
                {
                    bool existsText = false;
                    string voicelist = txtVoiceText.Text;

                    existsText = (lstVoiceText != null && lstVoiceText.Count > 0) ? lstVoiceText.Any(item => item.voiceTextlst == voicelist) : false;
                    if (!existsText)
                    {
                        ListObject voice = new ListObject();
                        voice.voiceTextlst = voicelist;

                        if (lstVoiceText == null)
                        {
                            lstVoiceText = new List<ListObject>();
                        }
                        lstVoiceText.Add(voice);

                        gridView2.BeginUpdate();
                        gridView2.GridControl.DataSource = lstVoiceText;
                        gridView2.EndUpdate();
                        txtVoiceText.EditValue = null;
                    }
                    else
                    {
                        #region Hien thi message thong bao
                        MessageManager.ShowAlert(this, Resources.ResourceMessage.ThongBao, String.Format(ResourceMessage.ThongBaoDuLieuDaCo, voicelist));
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private string VoiceTextTXT()
        {
            string txtVoice = null;

            var lst = (List<ListObject>)gridView2.GridControl.DataSource;

            Inventec.Common.Logging.LogSystem.Warn("Dữ liệu gridView2: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => lst), lst));

            if (lst != null && lst.Count > 0)
            {
                foreach (var item in lst)
                {
                    txtVoice += item.voiceTextlst + ",";
                }
            }
            else
            {
                txtVoice = "";
            }

            if (txtVoice.EndsWith(","))
            {
                txtVoice = txtVoice.Remove(txtVoice.Length - 1);
            }

            return txtVoice;
        }

        private void repositoryItemButtonEdit1_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                var rowData = (ListObject)gridView2.GetFocusedRow();
                if (rowData != null)
                {
                    lstVoiceText.Remove(rowData);
                    if (lstVoiceText != null && lstVoiceText.Count > 0)
                    {
                        gridView2.BeginUpdate();
                        gridView2.GridControl.DataSource = null;
                        gridView2.GridControl.DataSource = lstVoiceText;
                        gridView2.EndUpdate();
                    }
                    else
                    {
                        lstVoiceText = null;
                        gridView2.BeginUpdate();
                        gridView2.GridControl.DataSource = lstVoiceText;
                        gridView2.EndUpdate();
                        //ValidationSingleControl(this.txtVoiceText);
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView2_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                bool existsText = false;
                GridView view = sender as GridView;
                if (view == null) return;
                if (e.Column.Caption != "voiceTextlst") return;

                string valueEdit = (e.Value ?? "").ToString();
                existsText = (lstVoiceText != null && lstVoiceText.Count > 0) ? lstVoiceText.Any(item => item.voiceTextlst == valueEdit) : false;
                if (!existsText)
                {
                    ListObject voice = new ListObject();
                    voice.voiceTextlst = valueEdit;
                    lstVoiceText.Add(voice);

                    gridView2.BeginUpdate();
                    gridView2.GridControl.DataSource = null;
                    gridView2.GridControl.DataSource = lstVoiceText;
                    gridView2.EndUpdate();
                }

                if (existsText)
                {
                    #region Hien thi message thong bao
                    MessageManager.ShowAlert(this, Resources.ResourceMessage.ThongBao, String.Format(ResourceMessage.ThongBaoDuLieuDaCo, valueEdit));
                    #endregion

                    DevExpress.XtraGrid.Columns.GridColumn col = view.Columns.ColumnByFieldName("voiceTextlst");
                    if (col == null) return;

                    //Khi sửa trực tiếp trong grid có dữ liệu trùng thì thông báo tự tắt & tự động quay trở lại giá trị trước khi sửa
                    string oldValue = (view.ActiveEditor.OldEditValue ?? "").ToString();
                    gridView2.SetRowCellValue(e.RowHandle, col, oldValue);

                    lstVoiceText = gridView2.DataSource as List<ListObject>;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboAppCode_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboAppCode.EditValue != null && cboAppCode.EditValue != "")
                {
                    LoadCboModuleLink();
                }
                else
                {
                    List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                    columnInfos.Add(new ColumnInfo("MODULE_LINK", "", 300, 1));
                    ControlEditorADO controlEditorADO = new ControlEditorADO("MODULE_LINK", "MODULE_LINK", columnInfos, false, 300);
                    ControlEditorLoader.Load(this.cboModuleLink, null, controlEditorADO);

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboModuleLink_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtVoiceText.Focus();
                    txtVoiceText.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboAppCode_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboModuleLink.Focus();
                    cboModuleLink.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}
