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
using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.Plugins.SdaConfigApp.entity;
using HIS.Desktop.Plugins.SdaConfigApp.Resources;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.Message;
using Inventec.UC.Paging;
using SDA.EFMODEL.DataModels;
using SDA.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.SdaConfigApp
{
    public partial class frmSdaConfigApp : HIS.Desktop.Utility.FormBase
    {
        Inventec.Desktop.Common.Modules.Module moduleData;
        int start = 0;
        int limit = 0;
        int rowCount = 0;
        int dataTotal = 0;
        int ActionType = -1;
        int positionHandle = -1;
        PagingGrid pagingGrid;
        SDA_CONFIG_APP currentData;
        List<SdaConfigAppADO> listSdaConfigAppADO { get; set; }

        Action delegateRefresh;

        public frmSdaConfigApp(Inventec.Desktop.Common.Modules.Module moduleData, Inventec.Common.WebApiClient.ApiConsumer sdaConsumer, Action delegateRefresh, long numPageSize, string applicationCode, string iconPath, List<ACS_APPLICATION> acsApplication):base(moduleData)
        {
            try
            {
                InitializeComponent();

                pagingGrid = new PagingGrid();
                this.moduleData = moduleData;
                this.delegateRefresh = delegateRefresh;
                ConfigApplications.NumPageSize = numPageSize;
                GlobalVariables.APPLICATION_CODE = applicationCode;
                ApiConsumers.SdaConsumer = sdaConsumer;
                RamData.acsAppication = acsApplication;

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

        private void frmSdaConfigApp_Load(object sender, EventArgs e)
        {
            try
            {
                if (this.moduleData != null)
                    this.Text = this.moduleData.text;
                FillDataToGridControl();
                LoadDataToCombo();
                ValidateForm();
                this.ActionType = GlobalVariables.ActionEdit;
                EnabledControlChanged(this.ActionType);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #region LoadDataToCombo
        private void LoadDataToCombo()
        {
            try
            {
                LoadDataComboAppCode();
                LoadDataToComboValueType();
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataComboAppCode()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("APPLICATION_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("APPLICATION_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("APPLICATION_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboAppCode, RamData.acsAppication, controlEditorADO);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToComboValueType()
        {
            try
            {
                List<Status> status = new List<Status>();
                status.Add(new Status(1, "long"));
                status.Add(new Status(2, "string"));
                status.Add(new Status(3, "int"));
                status.Add(new Status(4, "short"));
                status.Add(new Status(5, "decimal"));
                status.Add(new Status(6, "double"));

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("statusName", "", 300, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("statusName", "statusName", columnInfos, false, 350);
                ControlEditorLoader.Load(cboValueType, status, controlEditorADO);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        private void EnabledControlChanged(int action)
        {
            try
            {
                btnEdit.Enabled = (action == GlobalVariables.ActionEdit);
                btnAdd.Enabled = (action == GlobalVariables.ActionAdd);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridControl()
        {
            try
            {
                int numPageSize;
                if (ucPaging1.pagingGrid != null)
                {
                    numPageSize = ucPaging1.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = (int)ConfigApplications.NumPageSize;
                }
                FillDataToGridSdaConfigApp(new CommonParam(0, numPageSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(FillDataToGridSdaConfigApp, param, numPageSize, this.gridControlSdaConfigApp);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //Load data lên grid
        private void FillDataToGridSdaConfigApp(object data)
        {
            try
            {
                WaitingManager.Show();
                this.start = ((CommonParam)data).Start ?? 0;
                this.limit = ((CommonParam)data).Limit ?? 0;
                CommonParam param = new CommonParam(this.start, this.limit);
                this.listSdaConfigAppADO = new List<SdaConfigAppADO>();
                Inventec.Core.ApiResultObject<List<SDA_CONFIG_APP>> apiResult = null;
                SdaConfigAppFilter filter = new SdaConfigAppFilter();
                filter.KEY_WORD = txtFind.Text;
                filter.APP_CODE_ACCEPT = GlobalVariables.APPLICATION_CODE;
                apiResult = new BackendAdapter(param).GetRO<List<SDA_CONFIG_APP>>("api/SdaConfigApp/Get", ApiConsumers.SdaConsumer, filter, param);
                if (apiResult != null)
                {
                    this.listSdaConfigAppADO = (from m in ((List<SDA_CONFIG_APP>)apiResult.Data) select new SdaConfigAppADO(m)).ToList();
                    gridControlSdaConfigApp.DataSource = null;
                    gridControlSdaConfigApp.DataSource = this.listSdaConfigAppADO;
                    this.rowCount = (this.listSdaConfigAppADO == null ? 0 : this.listSdaConfigAppADO.Count);
                    this.dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //xử lí click 1 dòng trên grid
        private void gridViewSdaConfigApp_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                this.currentData = (SDA_CONFIG_APP)gridViewSdaConfigApp.GetFocusedRow();
                if (this.currentData != null)
                {
                    ChangDataRow(this.currentData);
                }
                
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewSdaConfigApp_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                this.currentData = (SDA_CONFIG_APP)gridViewSdaConfigApp.GetFocusedRow();
                if (this.currentData != null)
                {
                    ChangDataRow(this.currentData);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ChangDataRow(SDA_CONFIG_APP data)
        {
            try
            {
                if (data != null)
                {
                    FillDataToEditorControl(data);
                    this.ActionType = GlobalVariables.ActionEdit;
                    if (data.IS_ACTIVE == 0)
                    {
                        this.ActionType = -1;
                    }
                    EnabledControlChanged(this.ActionType);
                }
                else
                {
                    ResetFormData();
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToEditorControl(SDA_CONFIG_APP data)
        {
            try
            {
                txtAppCode.EditValue = data.APP_CODE;
                if (data.APP_CODE != null)
                {
                    cboAppCode.EditValue = RamData.acsAppication.SingleOrDefault(o => o.APPLICATION_CODE == data.APP_CODE).ID;
                }
                else
                {
                    cboAppCode.EditValue = null;
                }
                txtDefaultValue.EditValue = data.DEFAULT_VALUE;
                txtDescription.EditValue = data.DESCRIPTION;
                txtValueAllowIn.EditValue = data.VALUE_ALLOW_IN;
                txtValueAllowMax.EditValue = data.VALUE_ALLOW_MAX;
                txtValueAllowMin.EditValue = data.VALUE_ALLOW_MIN;
                cboValueType.EditValue = data.VALUE_TYPE;
                txtKey.EditValue = data.KEY;
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #region vailidation
        private void dxValidationProviderEditorInfo_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
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

        private void ValidateForm()
        {
            try
            {
                ValidationSingleControl(txtKey);
                ValidationControlMaxLength(txtDescription,4000);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidationSingleControl(BaseEdit control)
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = control;
                validRule.ErrorText = String.Format(ResourceMessage.ChuaNhapTruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditorInfo.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationControlMaxLength(BaseEdit control, int? maxLength)
        {
            try
            {
                ControlMaxLengthValidationRule validate = new ControlMaxLengthValidationRule();
                validate.editor = control;
                validate.maxLength = maxLength;
                //validate.IsRequired = true;
                validate.ErrorText = "Nhập quá kí tự cho phép";
                validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProviderEditorInfo.SetValidationRule(control, validate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }
        #endregion

        #region Xử lý phím tắt
        private void barButtonItemFind_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnFind_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barButtonItemCancel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        private void barButtonItemAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (this.ActionType == GlobalVariables.ActionAdd && btnAdd.Enabled)
                {
                    btnAdd.Focus();
                    btnAdd_Click(null, null);
                }
                else
                {
                    return;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barButtonItemEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (this.ActionType == GlobalVariables.ActionEdit && btnEdit.Enabled)
                {
                    btnEdit.Focus();
                    btnEdit_Click(null, null);
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        private void btnFind_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                FillDataToGridControl();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
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

                Inventec.Common.Logging.LogSystem.Error(ex);
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

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //Xử lý lưu
        private void SaveProcess()
        {
            try
            {
                bool success = false;
                positionHandle = -1;
                if (!dxValidationProviderEditorInfo.Validate())
                    return;
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                SDA_CONFIG_APP updateDTO = new SDA_CONFIG_APP();

                UpdateDTOFromDataForm(ref updateDTO);
                if (ActionType == GlobalVariables.ActionAdd)
                {
                    var resultData = new BackendAdapter(param).Post<SDA_CONFIG_APP>(
                        "api/SdaConfigApp/Create", ApiConsumers.SdaConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;
                        FillDataToGridControl();
                        ResetFormData();
                        if (this.delegateRefresh != null)
                            this.delegateRefresh();
                    }
                }
                else
                {
                    if (currentData.ID > 0 && currentData.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE)
                    {
                        updateDTO.ID = currentData.ID;
                        var resultData = new BackendAdapter(param).Post<SDA_CONFIG_APP>(
                        "api/SdaConfigApp/Update", ApiConsumers.SdaConsumer, updateDTO, param);
                        if (resultData != null)
                        {
                            success = true;
                            UpdateRowDataAfterEdit(resultData);
                            if (this.delegateRefresh != null)
                                this.delegateRefresh();
                        }
                    }
                    else
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Dữ liệu đang bị khóa", "Thông báo");
                        return;
                    }
                }

                #region Hien thi message thong bao
                MessageManager.Show(this, param, success);
                #endregion

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //Update dòng dữ liệu thay đổi
        private void UpdateRowDataAfterEdit(SDA_CONFIG_APP data)
        {
            try
            {
                if (data == null)
                    throw new ArgumentNullException("data(SDA.EFMODEL.DataModels.SDA_CONFIG_APP) is null");
                var rowData = (SDA_CONFIG_APP)gridViewSdaConfigApp.GetFocusedRow();
                if (rowData != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<SDA_CONFIG_APP>(rowData, data);
                    gridViewSdaConfigApp.RefreshRow(gridViewSdaConfigApp.FocusedRowHandle);
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //update dữ liệu từ form để lưu
        private void UpdateDTOFromDataForm(ref SDA_CONFIG_APP updateDTO)
        {
            try
            {
                updateDTO.VALUE_ALLOW_IN = (string)txtValueAllowIn.EditValue;
                updateDTO.APP_CODE = (string)txtAppCode.EditValue;
                updateDTO.KEY = (string)txtKey.EditValue;
                updateDTO.DEFAULT_VALUE = (string)txtDefaultValue.EditValue;
                updateDTO.DESCRIPTION = (string)txtDescription.EditValue;
                updateDTO.VALUE_TYPE = (string)cboValueType.EditValue;
                if (txtValueAllowMin.EditValue != null)
                {
                    updateDTO.VALUE_ALLOW_MIN = (txtValueAllowMin.EditValue).ToString();
                }
                if (txtValueAllowMax.EditValue != null)
                {
                    updateDTO.VALUE_ALLOW_MAX = (txtValueAllowMax.EditValue).ToString();
                }

            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                this.ActionType = GlobalVariables.ActionAdd;
                EnabledControlChanged(this.ActionType);
                positionHandle = -1;
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderEditorInfo, dxErrorProvider);
                ResetFormData();
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //reset form
        private void ResetFormData()
        {
            try
            {
                txtKey.EditValue = null;
                txtAppCode.EditValue = null;
                cboAppCode.EditValue = null;
                txtDefaultValue.EditValue = null;
                txtDescription.EditValue = null;
                txtFind.EditValue = null;
                txtValueAllowIn.EditValue = null;
                txtValueAllowMax.EditValue = null;
                txtValueAllowMin.EditValue = null;
                cboValueType.EditValue = null;
                this.ActionType = GlobalVariables.ActionAdd;
                EnabledControlChanged(this.ActionType);

            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewSdaConfigApp_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    SDA_CONFIG_APP data = (SDA_CONFIG_APP)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            try
                            {
                                e.Value = e.ListSourceRowIndex + 1 + start;
                            }
                            catch (Exception ex)
                            {

                                Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri STT", ex);
                            }
                        }
                        else if (e.Column.FieldName == "CREATE_TIME_STR")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString((long)data.CREATE_TIME);
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
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString((long)data.MODIFY_TIME);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "IS_ACTIVE_STR")
                        {
                            try
                            {
                                if (data.IS_ACTIVE == 1)
                                    e.Value = "Hoạt động";
                                else e.Value = "Tạm khóa";
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

        private void gridViewSdaConfigApp_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    short isActive = Inventec.Common.TypeConvert.Parse.ToInt16((gridViewSdaConfigApp.GetRowCellValue(e.RowHandle, "IS_ACTIVE") ?? "").ToString());
                    if (e.Column.FieldName == "LOCK")
                    {
                        e.RepositoryItem = (isActive == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE ? btnUnLock : btnLock);
                    }
                    else if (e.Column.FieldName == "DELETE")
                    {
                        e.RepositoryItem = (isActive == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE ? btnDELETE : btnDELETE_Disable);
                    }
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnUnLock_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show(String.Format(ResourceMessage.BanCoMuonKhoaDuLieuKhong), "", MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    WaitingManager.Show();
                    LockDataProcess();
                }
                else
                {
                    return;
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnLock_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show(String.Format(ResourceMessage.BanCoMuonMoKhoaDuLieuKhong), "", MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    WaitingManager.Show();
                    LockDataProcess();
                }
                else
                {
                    return;
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //hàm khóa và mở khóa
        private void LockDataProcess()
        {
            try
            {
                CommonParam param = new CommonParam();
                SDA_CONFIG_APP sdaConfigApp = new SDA_CONFIG_APP();
                bool notHandler = false;
                WaitingManager.Show();
                sdaConfigApp = new BackendAdapter(param).Post<SDA_CONFIG_APP>(
                    "api/SdaConfigApp/ChangeLock", ApiConsumers.SdaConsumer, currentData, param);
                if (sdaConfigApp != null)
                {
                    notHandler = true;
                    txtFind.Focus();
                    UpdateRowDataAfterEdit(sdaConfigApp);
                    if (this.delegateRefresh != null)
                        this.delegateRefresh();
                }
                MessageManager.Show(this.ParentForm, param, notHandler);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //hàm xóa dữ liệu
        private void btnDELETE_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show(String.Format(ResourceMessage.BanCoMuonXoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    WaitingManager.Show();
                    bool success = false;
                    CommonParam param = new CommonParam();
                    success = new BackendAdapter(param).Post<bool>("api/SdaConfigApp/Delete", ApiConsumers.SdaConsumer, currentData.ID, param);
                    if (success)
                    {
                        FillDataToGridControl();
                        if (this.delegateRefresh != null)
                            this.delegateRefresh();
                    }
                    MessageManager.Show(this, param, success);
                }
                else
                {
                    return;
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #region Xử lý focus
        private void txtFind_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnFind.Focus();
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtKey_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtDefaultValue.Focus();
                    txtDefaultValue.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtDefaultValue_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboValueType.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtValueType_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtValueAllowMin.Focus();
                    txtValueAllowMin.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtValueAllowMin_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtValueAllowMax.Focus();
                    txtValueAllowMax.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtValueAllowMax_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtValueAllowIn.Focus();
                    txtValueAllowIn.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtValueAllowIn_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtDescription.Focus();
                    txtDescription.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtDescription_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnEdit.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Xử lý sự kiện combo
        private void txtAppCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrEmpty(txtAppCode.Text))
                    {
                        string key = Inventec.Common.String.Convert.UnSignVNese(this.txtAppCode.Text.ToLower().Trim());
                        var data = RamData.acsAppication.Where(o => Inventec.Common.String.Convert.UnSignVNese(o.APPLICATION_CODE.ToLower()).Contains(key)).ToList();

                        List<ACS_APPLICATION> result = (data != null ? ((data.Count == 1) ? data : data.Where(o => o.APPLICATION_CODE == key).ToList()) : null);
                        if (result != null && result.Count == 1)
                        {
                            cboAppCode.EditValue = result[0].ID;
                            txtAppCode.Text = result[0].APPLICATION_CODE;
                            cboAppCode.Focus();
                            cboAppCode.ShowPopup();
                        }
                        else
                        {
                            cboAppCode.EditValue = null;
                            cboAppCode.Focus();
                            cboAppCode.ShowPopup();
                        }
                    }
                    cboAppCode.Focus();
                    cboAppCode.ShowPopup();
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboAppCode_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboAppCode.EditValue != null && cboAppCode.EditValue != cboAppCode.OldEditValue)
                    {
                        ACS_APPLICATION gt = RamData.acsAppication.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboAppCode.EditValue.ToString()));
                        if (gt != null)
                        {
                            cboAppCode.Properties.Buttons[1].Visible = true;
                            txtAppCode.Text = gt.APPLICATION_CODE;
                            txtKey.Focus();
                        }
                    }
                    else
                    {
                        txtKey.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboAppCode_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboAppCode.Properties.Buttons[1].Visible = false;
                    cboAppCode.EditValue = null;
                    txtAppCode.Text = "";
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
                    if (cboAppCode.EditValue != null)
                    {
                        txtKey.Focus();
                    }
                    else
                    {
                        cboAppCode.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboValueType_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (cboValueType.EditValue != null)
                {
                    cboValueType.Properties.Buttons[1].Visible = true;
                    txtValueAllowMin.Focus();
                    txtValueAllowMin.SelectAll();
                }
                else
                    txtValueAllowMin.Focus();
                txtValueAllowMin.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboValueType_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboValueType.EditValue != null)
                    {
                        txtValueAllowMin.Focus();
                        txtValueAllowMin.SelectAll();
                    }
                    else
                    {
                        cboValueType.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboValueType_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboValueType.Properties.Buttons[1].Visible = false;
                    cboValueType.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        private void gridViewSdaConfigApp_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    SDA_CONFIG_APP data = (SDA_CONFIG_APP)((IList)((BaseView)sender).DataSource)[e.RowHandle];
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
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}
