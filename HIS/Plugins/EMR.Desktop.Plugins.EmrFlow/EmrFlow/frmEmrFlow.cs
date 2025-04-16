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
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Inventec.Desktop.Common.Modules;
using Inventec.UC.Paging;
using Inventec.Common.Logging;
using HIS.Desktop.LocalStorage.LocalData;
using EMR.Desktop.Plugins.EmrFlow.Validate;
using HIS.Desktop.LibraryMessage;
using DevExpress.XtraEditors.DXErrorProvider;
using EMR.Filter;
using EMR.EFMODEL.DataModels;
using Inventec.Core;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.BackendData;
using DevExpress.XtraEditors;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using Inventec.Common.Controls.EditorLoader;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using HIS.Desktop.Utility;
using EMR.Desktop.Plugins.EmrFlow.Resources;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Controls.ValidationRule;
using DevExpress.XtraEditors.Controls;
using EMR.URI;


namespace EMR.Desktop.Plugins.EmrFlow.EmrFlow
{
    public partial class frmEmrFlow : FormBase
    {
        #region Declare avariable
        PagingGrid paging;
        Module module;
        int ActionType = -1;
        int positionHandler = -1;
        int startPage = 0;
        int dataTotal = 0;
        int rowdata = 0;
        long EmrFlowID;
        V_EMR_FLOW EmrFlowData;
        EMR_BUSINESS EmrBusinessData;
        long EmrBusinessID;
        #endregion
        public frmEmrFlow(Module moduleData)
            : base(moduleData)
        {
            try
            {
                InitializeComponent();
                SetCaptionByLangueageKey();
                try
                {
                    paging = new PagingGrid();
                    this.module = moduleData;
                    try
                    {
                        string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                        this.Icon = Icon.ExtractAssociatedIcon(iconPath);
                        this.ActionType = GlobalVariables.ActionAdd;
                    }
                    catch (Exception ex)
                    {

                        LogSystem.Warn(ex);
                    }

                }
                catch (Exception ex)
                {
                    LogSystem.Warn(ex);
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        public frmEmrFlow(Module moduleData, V_EMR_FLOW EmrFlow)
            : base(moduleData)
        {
            InitializeComponent();
            try
            {
                paging = new PagingGrid();
                this.module = moduleData;
                SetCaptionByLangueageKey();
                try
                {
                    string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                    this.Icon = Icon.ExtractAssociatedIcon(iconPath);
                    this.ActionType = GlobalVariables.ActionAdd;
                    if (EmrFlow != null)
                    {
                        EmrFlowData = EmrFlow;

                    }
                }
                catch (Exception ex)
                {

                    LogSystem.Warn(ex);
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        public frmEmrFlow(Module moduleData, EMR_BUSINESS EmrBusiness)
            : base(moduleData)
        {
            InitializeComponent();
            try
            {
                paging = new PagingGrid();
                this.module = moduleData;
                SetCaptionByLangueageKey();
                try
                {
                    string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                    this.Icon = Icon.ExtractAssociatedIcon(iconPath);
                    this.ActionType = GlobalVariables.ActionAdd;
                    if (EmrBusiness != null)
                    {
                        EmrBusinessData = EmrBusiness;

                    }
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
        #region ---Private Method
        private void frmEmrFlow_Load(object sender, EventArgs e)
        {
            try
            {
                MeShow();
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void MeShow()
        {
            try
            {
                SetDefautValue();
                EnabledControlChange(this.ActionType);
                ValidateForm();
                LoadDataToForm(EmrBusinessData);
                LoadDataToCombox();
                SetCaptionByLangueageKey();
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        #region Load data to combox
        private void LoadDataToCombox()
        {
            LoadDataTo_cboBusinessID();
            LoadDataTo_cboRoomCode();
        }

        private void LoadDataTo_cboBusinessID()
        {
            try
            {
                CommonParam param = new CommonParam();
                EmrBusinessFilter filter = new EmrBusinessFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                var data = new BackendAdapter(param).Get<List<EMR_BUSINESS>>(EMR.URI.EmrBusiness.GET, ApiConsumers.EmrConsumer, filter, param);
                List<ColumnInfo> columnInfor = new List<ColumnInfo>();
                columnInfor.Add(new ColumnInfo("BUSINESS_CODE", "Mã nghiệp vụ", 150, 1));
                columnInfor.Add(new ColumnInfo("BUSINESS_NAME", "Tên nghiệp vụ", 200, 2));
                ControlEditorADO controleditor = new ControlEditorADO("BUSINESS_NAME", "ID", columnInfor, true, 350);
                ControlEditorLoader.Load(cboBusinessID, data, controleditor);
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void LoadDataTo_cboRoomCode()
        {
            try
            {

                var data = BackendDataWorker.Get<V_HIS_ROOM>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && (EmrBusinessData != null ? o.DEPARTMENT_CODE == EmrBusinessData.DEPARTMENT_CODE: true)).ToList();
                List<ColumnInfo> columInfor = new List<ColumnInfo>();
                columInfor.Add(new ColumnInfo("ROOM_CODE", "Mã phòng ký", 150, 1));
                columInfor.Add(new ColumnInfo("ROOM_NAME", "Tên phòng ký", 200, 1));
                ControlEditorADO controlEditort = new ControlEditorADO("ROOM_NAME", "ID", columInfor, true, 350);
                ControlEditorLoader.Load(cboRoomCode, data, controlEditort);
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }


        #endregion

        private void SetCaptionByLangueageKey()
        {
            try
            {
                if (this.module != null && !String.IsNullOrEmpty(this.module.text))
                {
                    this.Text = this.module.text;
                }
                ResourceLanguageManager.LanguageResource = new ResourceManager("EMR.Desktop.Plugins.EmrFlow.Resources.Lang", typeof(EMR.Desktop.Plugins.EmrFlow.EmrFlow.frmEmrFlow).Assembly);
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("frmEmrFlow.btnAdd.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("frmEmrFlow.btnEdit.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("frmEmrFlow.btnSearch.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnRest.Text = Inventec.Common.Resource.Get.Value("frmEmrFlow.btnRest.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcFlowCode.Text = Inventec.Common.Resource.Get.Value("frmEmrFlow.lcFlowCode.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcFlowName.Text = Inventec.Common.Resource.Get.Value("frmEmrFlow.lcFlowName.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcNumOder.Text = Inventec.Common.Resource.Get.Value("frmEmrFlow.lcNumOder.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcRoom.Text = Inventec.Common.Resource.Get.Value("frmEmrFlow.lcRoom.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.STT.Caption = Inventec.Common.Resource.Get.Value("frmEmrFlow.STT.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.STT.ToolTip = Inventec.Common.Resource.Get.Value("frmEmrFlow.STT.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColFlowCode.Caption = Inventec.Common.Resource.Get.Value("frmEmrFlow.grdColFlowCode.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColFlowCode.ToolTip = Inventec.Common.Resource.Get.Value("frmEmrFlow.grdColFlowCode.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColFlowName.Caption = Inventec.Common.Resource.Get.Value("frmEmrFlow.grdColFlowName.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColFlowName.ToolTip = Inventec.Common.Resource.Get.Value("frmEmrFlow.grdColFlowName.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColBusinessId.Caption = Inventec.Common.Resource.Get.Value("frmEmrFlow.grdColBusinessId.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColBusinessId.ToolTip = Inventec.Common.Resource.Get.Value("frmEmrFlow.grdColBusinessId.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColNumOder.Caption = Inventec.Common.Resource.Get.Value("frmEmrFlow.grdColNumOder.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColNumOder.ToolTip = Inventec.Common.Resource.Get.Value("frmEmrFlow.grdColNumOder.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColRoomCode.Caption = Inventec.Common.Resource.Get.Value("frmEmrFlow.grdColRoomCode.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColRoomCode.ToolTip = Inventec.Common.Resource.Get.Value("frmEmrFlow.grdColRoomCode.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColRoomName.Caption = Inventec.Common.Resource.Get.Value("frmEmrFlow.grdColRoomName.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColRoomName.ToolTip = Inventec.Common.Resource.Get.Value("frmEmrFlow.grdColRoomName.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColRoomTypeID.Caption = Inventec.Common.Resource.Get.Value("frmEmrFlow.grdColRoomTypeID.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColRoomTypeID.ToolTip = Inventec.Common.Resource.Get.Value("frmEmrFlow.grdColRoomTypeID.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreateTime.Caption = Inventec.Common.Resource.Get.Value("frmEmrFlow.grdColCreateTime.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreateTime.ToolTip = Inventec.Common.Resource.Get.Value("frmEmrFlow.grdColCreateTime.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreator.Caption = Inventec.Common.Resource.Get.Value("frmEmrFlow.grdColCreator.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreator.ToolTip = Inventec.Common.Resource.Get.Value("frmEmrFlow.grdColCreator.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifyTime.Caption = Inventec.Common.Resource.Get.Value("frmEmrFlow.grdColModifyTime.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifyTime.ToolTip = Inventec.Common.Resource.Get.Value("frmEmrFlow.grdColModifyTime.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifier.Caption = Inventec.Common.Resource.Get.Value("frmEmrFlow.grdColModifier.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifier.ToolTip = Inventec.Common.Resource.Get.Value("frmEmrFlow.grdColModifier.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void SetDefautValue()
        {
            try
            {
                this.ActionType = GlobalVariables.ActionAdd;
                txtFlowCode.Text = "";
                txtFlowName.Text = "";
                spNumOder.EditValue = null;
                cboBusinessID.EditValue = null;
                txtRoomCode.Text = "";
                cboRoomCode.EditValue = null;
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void EnabledControlChange(int action)
        {
            try
            {
                btnAdd.Enabled = (action == GlobalVariables.ActionAdd);
                btnEdit.Enabled = (action == GlobalVariables.ActionEdit);
                txtFlowCode.ReadOnly = !(action == GlobalVariables.ActionAdd);
                spNumOder.ReadOnly = !(action == GlobalVariables.ActionAdd);
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }
        #region ---Validate

        private void ValidateForm()
        {
            try
            {
                Validate_FlowCode();
                Validate_FlowName();
                Validate_EditSPinNull();
                Validate_EditcboboxNull();
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }
        private void Validate_FlowCode()
        {
            try
            {
                ValidateMaxLength validate = new ValidateMaxLength();
                validate.Maxlangth = 6;
                validate.txtControl = txtFlowCode;
                validate.ErrorText = MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validate.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(txtFlowCode, validate);
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }
        private void Validate_FlowName()
        {
            try
            {
                ValidateMaxLength validate = new ValidateMaxLength();
                validate.Maxlangth = 100;
                validate.ErrorText = MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validate.ErrorType = ErrorType.Warning;
                validate.txtControl = txtFlowName;
                dxValidationProvider1.SetValidationRule(txtFlowName, validate);
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }
        private void Validate_EditSPinNull()
        {
            try
            {
                ValidateSpinNull vali = new ValidateSpinNull();
                vali.txtControl = spNumOder;
                vali.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(spNumOder, vali);

            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }
        private void Validate_EditcboboxNull()
        {
            try
            {

                ValidateCoboBox validRule = new ValidateCoboBox();
                validRule.txtControl = cboBusinessID;
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(cboBusinessID, validRule);
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        #endregion
        private void LoadDataToForm(EMR_BUSINESS data)
        {
            try
            {
                if (data != null)
                {
                    EmrBusinessID = data.ID;
                    cboBusinessID.EditValue = data.ID;
                    FillDataToControl();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
            }
        }

        private void FillDataToControl()
        {
            try
            {
                WaitingManager.Show();
                int pagingSize = 0;
                if (ucPaging2.pagingGrid != null)
                {
                    pagingSize = ucPaging2.pagingGrid.PageSize;
                }
                else
                {
                    pagingSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }
                LoadPaging(new CommonParam(0, pagingSize));
                CommonParam param = new CommonParam();
                param.Limit = rowdata;
                param.Count = dataTotal;
                ucPaging2.Init(LoadPaging, param, pagingSize, this.grdControlEmrFlow);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void LoadPaging(object param)
        {
            try
            {
                startPage = ((CommonParam)param).Start ?? 0;
                //limit=pageSize
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramcommon = new CommonParam(startPage, limit);
                ApiResultObject<List<V_EMR_FLOW>> apiResult = null;
                EmrFlowViewFilter filter = new EmrFlowViewFilter();
                SetFilter(ref filter);
                grdControlEmrFlow.DataSource = null;
                grdViewEmrFlow.BeginUpdate();
                apiResult = new BackendAdapter(paramcommon).GetRO<List<V_EMR_FLOW>>(EMR.URI.EmrFlow.GET_VIEW, ApiConsumers.EmrConsumer, filter, paramcommon);
                if (apiResult != null)
                {
                    var data = (List<V_EMR_FLOW>)apiResult.Data;
                    if (data != null && data.Count > 0)
                    {
                        grdControlEmrFlow.DataSource = data;
                        rowdata = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }

                }

                grdViewEmrFlow.EndUpdate();
                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(paramcommon);
                #endregion

            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetFilter(ref EmrFlowViewFilter filter)
        {
            try
            {
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.BUSINESS_ID = this.EmrBusinessID;
                filter.KEY_WORD = txtSearch.Text.Trim();
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void SaveProcessor()
        {
            try
            {
                CommonParam param = new CommonParam();
                try
                {
                    bool success = false;
                    if (!btnEdit.Enabled && !btnAdd.Enabled)
                        return;
                    this.positionHandler = -1;

                    if (!dxValidationProvider1.Validate())
                        return;
                    EMR_FLOW updateDTO = new EMR_FLOW();
                    UpdateDTOfromDataToForm(ref updateDTO);
                    if (this.ActionType == GlobalVariables.ActionAdd)
                    {
                        if (!CheckNum_Oder())
                        {
                            MessageBox.Show("Xử lý thất bại. Số thứ tự ký đã tồn tại", "Thông báo");
                            return;
                        }
                        var resultData = new BackendAdapter(param).Post<EMR_FLOW>(EMR.URI.EmrFlow.CREATE, ApiConsumers.EmrConsumer, updateDTO, param);
                        if (resultData != null)
                        {
                            success = true;
                            BackendDataWorker.Reset<EMR_FLOW>();
                            LoadDataToForm(EmrBusinessData);
                            RestFormData();
                        }
                    }
                    else
                    {
                        if (this.EmrFlowID > 0)
                        {
                            updateDTO.ID = EmrFlowID;
                            var ResultData = new BackendAdapter(param).Post<EMR_FLOW>(EMR.URI.EmrFlow.UPDATE, ApiConsumers.EmrConsumer, updateDTO, param);
                            if (ResultData != null)
                            {
                                success = true;
                                LoadDataToForm(EmrBusinessData);
                                BackendDataWorker.Reset<EMR_FLOW>();

                            }
                        }
                    }
                    WaitingManager.Hide();
                    MessageManager.Show(this.ParentForm, param, success);
                    SessionManager.ProcessTokenLost(param);
                }
                catch (Exception ex)
                {
                    WaitingManager.Hide();
                    LogSystem.Warn(ex);
                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void RestFormData()
        {
            try
            {
                if (!lcEditorInfo.IsInitialized)
                    return;
                lcEditorInfo.BeginUpdate();
                try
                {
                    foreach (DevExpress.XtraLayout.BaseLayoutItem item in lcEditorInfo.Items)
                    {
                        DevExpress.XtraLayout.LayoutControlItem lci = item as DevExpress.XtraLayout.LayoutControlItem;
                        if (lci != null && lci.Control != null && lci.Control is BaseEdit)
                        {
                            DevExpress.XtraEditors.BaseEdit fomatFrm = lci.Control as DevExpress.XtraEditors.BaseEdit;
                            if (fomatFrm != cboBusinessID)
                            {
                                fomatFrm.ResetText();
                                fomatFrm.EditValue = null;
                            }
                            txtFlowCode.Focus();
                            txtFlowCode.SelectAll();
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

        private void UpdateDTOfromDataToForm(ref EMR_FLOW data)
        {
            try
            {
                data.FLOW_CODE = txtFlowCode.Text.Trim();
                data.FLOW_NAME = txtFlowName.Text.Trim();
                if (cboBusinessID.EditValue != null)
                {
                    data.BUSINESS_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboBusinessID.EditValue ?? "0").ToString());
                }
                if (spNumOder.EditValue != null)
                {
                    data.NUM_ORDER = Inventec.Common.TypeConvert.Parse.ToInt64((spNumOder.EditValue ?? "0").ToString());
                }

                if (cboRoomCode.EditValue != null)
                {

                    var dataRoom = BackendDataWorker.Get<V_HIS_ROOM>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboRoomCode.EditValue ?? "").ToString()));
                    var dataRoomTypevar = BackendDataWorker.Get<HIS_ROOM_TYPE>().SingleOrDefault(o => o.ID == dataRoom.ROOM_TYPE_ID);
                    data.ROOM_CODE = dataRoom.ROOM_CODE;
                    data.ROOM_NAME = dataRoom.ROOM_NAME;
                    data.ROOM_TYPE_CODE = dataRoomTypevar.ROOM_TYPE_CODE;
                }

            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }


        private void ChangeDataRow(V_EMR_FLOW dataEditor)
        {
            try
            {
                if (dataEditor != null)
                {
                    FillDataEditorControl(dataEditor);
                    this.ActionType = GlobalVariables.ActionEdit;
                    EnabledControlChange(this.ActionType);
                    btnEdit.Enabled = (dataEditor.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);
                    positionHandler = -1;
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider1);
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void FillDataEditorControl(V_EMR_FLOW Data)
        {
            try
            {
                if (Data != null)
                {
                    EmrFlowID = Data.ID;
                    spNumOder.EditValue = Data.NUM_ORDER;
                    txtFlowCode.Text = Data.FLOW_CODE;
                    txtFlowName.Text = Data.FLOW_NAME;
                    txtRoomCode.Text = Data.ROOM_CODE;
                    cboBusinessID.EditValue = Data.BUSINESS_ID;
                    if (Data.ROOM_CODE != null)
                    {
                        LoadtxtRoomCode1(Data);
                    }
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        #endregion

        #region ---Button Click
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

                LogSystem.Warn(ex);
            }
        }

        private void bbtnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (this.ActionType == GlobalVariables.ActionAdd && btnAdd.Enabled)
                {
                    btnAdd_Click(null, null);
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void bbtnRest_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (btnSearch.Enabled)
                {
                    btnRest_Click(null, null);
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void bbtnSearch_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                LoadDataToForm(EmrBusinessData);
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void bbtnRestFocus_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                txtFlowCode.Focus();
                txtFlowCode.SelectAll();
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                SaveProcessor();
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                SaveProcessor();
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void btnRest_Click(object sender, EventArgs e)
        {
            try
            {
                this.ActionType = GlobalVariables.ActionAdd;
                positionHandler = -1;
                EnabledControlChange(this.ActionType);
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider1);
                RestFormData();
                cboBusinessID.EditValue = EmrBusinessData.ID;
                txtFlowCode.Focus();
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void btnLock_Click(object sender, EventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                V_EMR_FLOW EmrFlow = new V_EMR_FLOW();
                V_EMR_FLOW RowData = (V_EMR_FLOW)grdViewEmrFlow.GetFocusedRow();
                bool Success = false;
                if (MessageBox.Show(Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    WaitingManager.Show();
                    EmrFlow = new BackendAdapter(param).Post<V_EMR_FLOW>(EMR.URI.EmrFlow.LOCK, ApiConsumers.EmrConsumer, RowData, null);
                    WaitingManager.Hide();
                    if (EmrFlow != null)
                    {
                        Success = true;
                        LoadDataToForm(EmrBusinessData);
                        BackendDataWorker.Reset<V_EMR_FLOW>();

                    }
                    MessageManager.Show(this.ParentForm, param, Success);
                }

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Warn(ex);
            }
        }

        private void btnUnLock_Click(object sender, EventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                V_EMR_FLOW RowData = (V_EMR_FLOW)grdViewEmrFlow.GetFocusedRow();
                bool success = false;
                V_EMR_FLOW EmrFlow = new V_EMR_FLOW();
                if (MessageBox.Show(Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    WaitingManager.Show();
                    EmrFlow = new BackendAdapter(param).Post<V_EMR_FLOW>(EMR.URI.EmrFlow.UNLOCK, ApiConsumers.EmrConsumer, RowData, null);
                    WaitingManager.Hide();
                    if (EmrFlow != null)
                    {
                        LoadDataToForm(EmrBusinessData);
                        BackendDataWorker.Reset<V_EMR_FLOW>();
                        success = true;
                    }
                    MessageManager.Show(this.ParentForm, param, success);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                LoadDataToForm(EmrBusinessData);
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void btnDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                bool success = false;
                V_EMR_FLOW RowData = (V_EMR_FLOW)grdViewEmrFlow.GetFocusedRow();
                if (MessageBox.Show(Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    WaitingManager.Show();
                    success = new BackendAdapter(param).Post<bool>(EMR.URI.EmrFlow.DELETE, ApiConsumers.EmrConsumer, RowData.ID, null);
                    if (success)
                    {
                        LoadDataToForm(EmrBusinessData);
                        BackendDataWorker.Reset<V_EMR_FLOW>();
                        btnRest_Click(null, null);
                    }
                    WaitingManager.Hide();
                    MessageManager.Show(this.ParentForm, param, success);

                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Warn(ex);
            }
        }
        #endregion

        #region ---Even GridView
        private void grdViewEmrFlow_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    V_EMR_FLOW data = (V_EMR_FLOW)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1 + startPage;
                        }
                        else if (e.Column.FieldName == "CREATE_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "MODIFY_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);

                        }
                        else if (e.Column.FieldName == "IS_ACTIVE_STR")
                        {
                            e.Value = data.IS_ACTIVE == 1 ? "Hoạt động" : "Tạm khóa";
                        }
                    }
                }


            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void grdViewEmrFlow_Click(object sender, EventArgs e)
        {
            try
            {
                var dataRow = (V_EMR_FLOW)grdViewEmrFlow.GetFocusedRow();
                if (dataRow != null)
                {
                    ChangeDataRow(dataRow);
                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void grdViewEmrFlow_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    V_EMR_FLOW data = (V_EMR_FLOW)grdViewEmrFlow.GetRow(e.RowHandle);
                    if (e.Column.FieldName == "IS_ACTIVE_STR")
                    {
                        if (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE)
                        {
                            e.Appearance.ForeColor = Color.Red;
                        }
                        else
                            e.Appearance.ForeColor = Color.Green;
                    }
                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void grdViewEmrFlow_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    V_EMR_FLOW data = (V_EMR_FLOW)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (e.Column.FieldName == "LOCK")
                    {
                        e.RepositoryItem = (data.IS_ACTIVE == 0 ? btnUnLock : btnLock);
                    }
                    if (e.Column.FieldName == "DELETE")
                    {
                        e.RepositoryItem = (data.IS_ACTIVE == 0 ? btnEnableDelete : btnDelete);
                    }
                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }
        #endregion

        #region --keyDown
        private void txtSearch_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    LoadDataToForm(EmrBusinessData);
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void txtFlowCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtFlowName.Focus();
                    txtFlowName.SelectAll();
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void txtFlowName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spNumOder.Focus();
                    spNumOder.SelectAll();

                }
                e.Handled = true;
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void txtBusinessID_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    var text = (sender as DevExpress.XtraEditors.TextEdit).Text;
                    loadtxtBusinessCode(text);
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void loadtxtBusinessCode(string text)
        {
            try
            {
                List<EMR_BUSINESS> ListResult = new List<EMR_BUSINESS>();
                ListResult = BackendDataWorker.Get<EMR_BUSINESS>().Where(o => (o.BUSINESS_CODE != null && o.BUSINESS_CODE.StartsWith(text))).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("BUSINESS_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("BUSINESS_NAME", "", 250, 2));
                if (ListResult.Count == 1)
                {
                    cboBusinessID.EditValue = ListResult[0].ID;
                    spNumOder.Focus();
                    spNumOder.SelectAll();
                }
                else
                {
                    cboBusinessID.EditValue = null;
                    cboBusinessID.Focus();
                    cboBusinessID.ShowPopup();
                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }
        private void spNumOder_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtRoomCode.Focus();
                    txtRoomCode.SelectAll();
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void LoadtxtRoomCode1(V_EMR_FLOW Data)
        {
            try
            {
                List<V_HIS_ROOM> listResult = new List<V_HIS_ROOM>();
                listResult = BackendDataWorker.Get<V_HIS_ROOM>().Where(o => o.ROOM_CODE != null
                        && o.ROOM_CODE == Data.ROOM_CODE
                        && o.ROOM_TYPE_CODE == Data.ROOM_TYPE_CODE) // Lọc theo khoa nếu có
                .ToList();

                cboRoomCode.EditValue = listResult[0].ID;
                txtRoomCode.Text = listResult[0].ROOM_CODE;
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void LoadtxtRoomCode(string _roomCode)
        {
            try
            {
                List<V_HIS_ROOM> listResult = new List<V_HIS_ROOM>();
                listResult = BackendDataWorker.Get<V_HIS_ROOM>().Where(o => (o.ROOM_CODE != null && o.ROOM_CODE.StartsWith(_roomCode, StringComparison.OrdinalIgnoreCase))).ToList();

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("ROOM_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("ROOM_NAME", "", 250, 2));

                if (listResult.Count == 1)
                {
                    cboRoomCode.EditValue = listResult[0].ID;
                    txtRoomCode.Text = listResult[0].ROOM_CODE;
                    cboRoomCode.Focus();
                    cboRoomCode.SelectAll();
                }
                else if (listResult.Count > 1)
                {
                    cboRoomCode.EditValue = null;
                    cboRoomCode.Focus();
                    cboRoomCode.ShowPopup();
                }
                else
                {
                    cboRoomCode.EditValue = null;
                    cboRoomCode.Focus();
                    cboRoomCode.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
        #endregion

        #region ---Even Combobox
        private void cboBusinessID_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboBusinessID.EditValue != null && cboBusinessID.EditValue != cboBusinessID.OldEditValue)
                    {
                        spNumOder.Focus();
                        spNumOder.SelectAll();
                    }
                    else
                    {
                        spNumOder.Focus();
                        spNumOder.SelectAll();
                    }

                }
            }
            catch (Exception ex)
            {

                LogSession.Error(ex);
            }
        }

        private void cboBusinessID_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboBusinessID != null)
                    {
                        spNumOder.Focus();
                        spNumOder.SelectAll();
                        cboBusinessID.ShowPopup();
                    }
                    else
                    {
                        cboBusinessID.ShowPopup();
                    }
                }
                else
                    cboBusinessID.ShowPopup();

                e.Handled = true;
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private bool CheckNum_Oder()
        {
            bool result = true;
            try
            {
                if (cboBusinessID.EditValue != null && spNumOder.EditValue != null)
                {
                    List<EMR_FLOW> FlowBusinessID = new List<EMR_FLOW>();
                    Int64 NumOrder = Inventec.Common.TypeConvert.Parse.ToInt64(spNumOder.EditValue.ToString());
                    FlowBusinessID = BackendDataWorker.Get<EMR_FLOW>().Where(o => o.BUSINESS_ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboBusinessID.EditValue.ToString() ?? "")).ToList();
                    for (int i = 0; i < FlowBusinessID.Count(); i++)
                    {
                        if (FlowBusinessID[i].NUM_ORDER == NumOrder)
                        {
                            result = false;
                            break;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
            return result;
        }

        private void cboRoomCode_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboRoomCode.EditValue != null)
                    {
                        var Data = BackendDataWorker.Get<V_HIS_ROOM>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboRoomCode.EditValue ?? "").ToString()));
                        if (Data != null)
                        {
                            txtRoomCode.Text = Data.ROOM_CODE;
                            cboRoomCode.Properties.Buttons[1].Visible = true;
                            btnAdd.Focus();
                        }
                        else
                        {
                            cboRoomCode.Focus();
                            cboRoomCode.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void cboRoomTypeID_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {

                    if (this.ActionType == GlobalVariables.ActionEdit && btnEdit.Enabled)
                    {
                        btnEdit.Focus();
                    }
                    else if (this.ActionType == GlobalVariables.ActionAdd && btnAdd.Enabled)
                    {
                        btnAdd.Focus();
                    }
                    else
                    {
                        btnRest.Focus();
                    }
                }
                else
                {
                    if (this.ActionType == GlobalVariables.ActionEdit && btnEdit.Enabled)
                    {
                        btnEdit.Focus();
                    }
                    else if (this.ActionType == GlobalVariables.ActionAdd && btnAdd.Enabled)
                    {
                        btnAdd.Focus();
                    }
                    else
                    {
                        btnRest.Focus();
                    }
                }


            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }



        #endregion
        private void txtRoomCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string srtValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                    LoadtxtRoomCode(srtValue);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboRoomCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    bool valid = false;
                    if (!String.IsNullOrEmpty(cboRoomCode.Text))
                    {
                        string key = cboRoomCode.Text.ToLower();
                        var listData = BackendDataWorker.Get<V_EMR_FLOW>().Where(o => o.ROOM_CODE.ToLower().Contains(key) || o.ROOM_NAME.ToLower().Contains(key)).ToList();
                        if (listData != null && listData.Count == 1)
                        {
                            valid = true;
                            cboRoomCode.EditValue = listData.First().ID;
                            txtRoomCode.Text = listData.First().ROOM_CODE;
                            btnAdd.Focus();
                        }
                    }
                    if (!valid)
                    {
                        cboRoomCode.Focus();
                        cboRoomCode.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboRoomCode_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboRoomCode.Properties.Buttons[1].Visible = true;
                    txtRoomCode.Text = null;
                    cboRoomCode.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
