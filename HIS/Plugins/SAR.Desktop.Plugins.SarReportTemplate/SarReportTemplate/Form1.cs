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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using SAR.EFMODEL.DataModels;
using SAR.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Resources;
using Inventec.UC.Paging;
using HIS.Desktop.LibraryMessage;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using Inventec.Desktop.Common.Controls.ValidationRule;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.Data;
using System.Drawing;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.LocalStorage.BackendData;
using System.IO;
using DevExpress.Spreadsheet;
using DevExpress.Utils;
using Inventec.Fss.Client;
using DevExpress.Entity.Model.Metadata;
using SAR.SDO;
using HIS.Desktop.Common;
using SAR.Desktop.Plugins.SarReportTemplate.ADO;

namespace SAR.Desktop.Plugins.SarReportTemplate
{
    public partial class Form1 : HIS.Desktop.Utility.FormBase
    {
        #region Reclare
        Inventec.Desktop.Common.Modules.Module moduleData;
        private Inventec.Desktop.Common.Modules.Module module;
        private SarReportTemplate.SarReportTemplateADO SarReportTemplateADO;
        SAR.EFMODEL.DataModels.V_SAR_REPORT_TEMPLATE currentData;
        int start = 0;
        int limit = 0;
        int rowCount = 0;
        int dataTotal = 0;
        int ActionType = -1;
        SaveFileDialog saveFileDialog1 = new SaveFileDialog();
        int startPage = 0;
        int positionHandle = -1;
        PagingGrid pagingGrid;

        List<Inventec.Fss.Utility.FileUploadInfo> fileUploadInfos;
        List<SAR.EFMODEL.DataModels.V_SAR_REPORT_TEMPLATE> DataChecks { get; set; }
        List<long> listID = new List<long>();
        #endregion

        public Form1()
        {
            InitializeComponent();
        }

        public Form1(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            InitializeComponent();
            try
            {
                pagingGrid = new PagingGrid();
                this.moduleData = moduleData;
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                InitRestoreLayoutGridViewFromXml(gridView1);
                EnableControlChanged(this.ActionType);
                //Fill data into datasource combo
                Filldatatocombo();
                
                FillDataToComboStatus();
                ValidateForm();
                SetDefaultFocus();
                SetDefaultValue();
                this.ActionType = GlobalVariables.ActionAdd;//actionedit
                // EnabledControlChanged(this.ActionType);
                FillDataToGridControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultValue()
        {
            try
            {
                this.ActionType = GlobalVariables.ActionAdd;
                //txtSearch.Text = "";
                ResetFormData();
                EnableControlChanged(this.ActionType);
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
                txtID.Text = "";
                txtName.Text = "";
                txtEXTENSION_RECEIVE.Text = "";
                lkREPORT_TYPE_ID.EditValue = null;
                memoEdit1.Text = "";

                txtID.Focus();
                txtID.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDefaultFocus()
        {
            try
            {
                txtSearch.Focus();
                txtSearch.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private void Filldatatocombo()
        {
            try
            {
                
                CommonParam param = new CommonParam();
                string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                SarUserReportTypeFilter uFilter = new SarUserReportTypeFilter();
                uFilter.LOGINNAME = loginName;
                var listReport = new BackendAdapter(param).Get<List<SAR_USER_REPORT_TYPE>>("/api/SarUserReportType/Get", ApiConsumers.SarConsumer, uFilter, param);
                
                if(listReport != null && listReport.Count > 0)
                    this.listID = listReport.Select(s=>s.REPORT_TYPE_ID).Distinct().ToList();
                SarReportTemplateFilter filter = new SarReportTemplateFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                List<SAR_REPORT_TYPE> rs = new List<SAR_REPORT_TYPE>();
                if (this.listID != null)
                {
                    var value = BackendDataWorker.Get<SAR_REPORT_TYPE>().ToList();
                    rs = value.Where(s => this.listID.Contains(s.ID)).ToList();
                }
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("REPORT_TYPE_CODE", "", 100, 1, true));
                columnInfos.Add(new ColumnInfo("REPORT_TYPE_NAME", "", 400, 1, true));
                ControlEditorADO controlEditorADO = new ControlEditorADO("REPORT_TYPE_NAME", "ID", columnInfos, false, 500);
                ControlEditorLoader.Load(lkREPORT_TYPE_ID, rs, controlEditorADO);
                
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
                ValidationSingleControl(lkREPORT_TYPE_ID);
                validatemaxl(txtName, 200, true);
                validatemaxl(txtID, 100, true);
                validatemaxl(txtEXTENSION_RECEIVE, 20, false);
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
                validRule.ErrorText = "Trường dữ liệu bắt buộc nhập";
                validRule.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void validatemaxl(BaseEdit control, int maxlength, bool isRequired)
        {
            try
            {
                Inventec.Desktop.Common.Controls.ValidationRule.ControlMaxLengthValidationRule validRule = new Inventec.Desktop.Common.Controls.ValidationRule.ControlMaxLengthValidationRule();
                validRule.editor = control;
                validRule.maxLength = maxlength;
                validRule.IsRequired = isRequired;
                validRule.ErrorText = "Trường dữ liệu vượt quá kí tự cho phép";
                validRule.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToGridControl()
        {
            try
            {
                int numPageSize;
                if (ucpaging1.pagingGrid != null)
                {
                    numPageSize = ucpaging1.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                    //mPageSize = (int)ConfigApplications.NumPageSize;
                }
                LoadPaging(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucpaging1.Init(LoadPaging, param, numPageSize, this.gridControl1);

            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        
        private void LoadPaging(object param)
        {
            try
            {
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);
                Inventec.Core.ApiResultObject<List<SAR.EFMODEL.DataModels.V_SAR_REPORT_TEMPLATE>> apiResult = null;
                //List<V_SAR_REPORT_TEMPLATE> listData = new List<V_SAR_REPORT_TEMPLATE>();
                SarReportTemplateViewFilter filter = new SarReportTemplateViewFilter();

                SetFilterNavBar(ref filter);
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                // dataNavigator1.DataSource = null;
                gridView1.BeginUpdate();
                apiResult = new BackendAdapter(paramCommon).GetRO<List<SAR.EFMODEL.DataModels.V_SAR_REPORT_TEMPLATE>>(SarRequestUriStore.SAR_REPORT_TEMPLATE_GETVIEW, ApiConsumers.SarConsumer, filter, paramCommon);
                CommonParam paramCom = new CommonParam();
                DataChecks = new BackendAdapter(paramCommon).Get<List<SAR.EFMODEL.DataModels.V_SAR_REPORT_TEMPLATE>>(SarRequestUriStore.SAR_REPORT_TEMPLATE_GETVIEW, ApiConsumers.SarConsumer, filter, paramCom);
                if (apiResult != null)
                {
                    var data = (List<SAR.EFMODEL.DataModels.V_SAR_REPORT_TEMPLATE>)apiResult.Data;
                    if (data != null)
                    {
                        //  dataNavigator1.DataSource = data;
                        gridView1.GridControl.DataSource = data;

                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                }
                
                gridView1.EndUpdate();

                #region Process has exception
                SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void SetFilterNavBar(ref SarReportTemplateViewFilter filter)
        {
            try
            {
                filter.KEY_WORD = txtSearch.Text.Trim();
                if (CboStatus.EditValue != null)
                {
                    long IDStauts = Int64.Parse(this.CboStatus.EditValue.ToString());
                    if (IDStauts == 2)
                    {
                        filter.IS_ACTIVE = 0;
                    }
                    else if (IDStauts == 3)
                    {
                        filter.IS_ACTIVE = 1;
                    }
                    else
                    {
                        filter.IS_ACTIVE = null;
                    }
                }
                else
                    filter.IS_ACTIVE = null;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void txtSearch_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnFind_Click(null, null);
                }
                else if (e.KeyCode == Keys.Down) ;
                {
                    var rowData = (SAR.EFMODEL.DataModels.V_SAR_REPORT_TEMPLATE)gridView1.GetFocusedRow();
                    if (rowData != null)
                    {
                        Changedatarow(rowData);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void Changedatarow(SAR.EFMODEL.DataModels.V_SAR_REPORT_TEMPLATE data)
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
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider1);
                }
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

        private void FillDataToEditorControl(V_SAR_REPORT_TEMPLATE data)
        {
            try
            {
                if (data != null)
                {
                    txtName.Text = data.REPORT_TEMPLATE_NAME;
                    txtID.EditValue = data.REPORT_TEMPLATE_CODE;
                    txtEXTENSION_RECEIVE.EditValue = data.EXTENSION_RECEIVE;
                    lkREPORT_TYPE_ID.EditValue = data.REPORT_TYPE_ID;
                    // cboType.EditValue = data.REPORT_TYPE_ID;
                    memoEdit1.EditValue = data.TUTORIAL;
                    //chkPause.Checked = (data.IS_PAUSE == 1 ? true : false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (btnAdd.Enabled)
            {
                btnAdd.Focus();
                btnAdd_Click(null, null);
            }
        }

        private void btnFind_Click(object sender, EventArgs e)
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

        private void gridView1_Click(object sender, EventArgs e)
        {
            try
            {
                var rowData = (SAR.EFMODEL.DataModels.V_SAR_REPORT_TEMPLATE)gridView1.GetFocusedRow();
                if (rowData != null)
                {
                    currentData = rowData;
                    Changedatarow(rowData);
                    //Set focus vào control editor đầu tiên
                    SetFocusEditor();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

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

        private void gridView1_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                //DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    short isActive = Inventec.Common.TypeConvert.Parse.ToInt16((gridView1.GetRowCellValue(e.RowHandle, "IS_ACTIVE") ?? "").ToString());
                    if (e.Column.FieldName == "lock")
                    {
                        e.RepositoryItem = (isActive == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? btnlock : btnUnlock);
                    }

                    if (e.Column.FieldName == "delete")
                    {
                        e.RepositoryItem = (isActive == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? delete1 : btnHidedelete);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView1_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    SAR.EFMODEL.DataModels.V_SAR_REPORT_TEMPLATE pData = (SAR.EFMODEL.DataModels.V_SAR_REPORT_TEMPLATE)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    if (pData == null) return;

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

                    gridControl1.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView1_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            if (e.RowHandle >= 0)
            {
                short isActive = Inventec.Common.TypeConvert.Parse.ToInt16((gridView1.GetRowCellValue(e.RowHandle, "IS_ACTIVE") ?? "").ToString());
                if (e.Column.FieldName == "IS_ACTIVE_STR")
                {
                    if (isActive == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE)
                        e.Appearance.ForeColor = Color.Red;
                    else
                        e.Appearance.ForeColor = Color.Green;
                }
            }
        }

        private void LoadCurrent(long currentId, ref SAR.EFMODEL.DataModels.SAR_REPORT_TEMPLATE currentDTO)
        {
            try
            {
                CommonParam param = new CommonParam();
                SarReportTemplateFilter filter = new SarReportTemplateFilter();

                filter.ID = currentId;
                var dTO = new BackendAdapter(param).Get<List<SAR.EFMODEL.DataModels.SAR_REPORT_TEMPLATE>>(SarRequestUriStore.SAR_REPORT_TEMPLATE_GET, ApiConsumers.SarConsumer, filter, param);
                if (dTO != null && dTO.Count > 0)
                {
                    currentDTO = dTO.FirstOrDefault();
                }
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
                if (!dxValidationProvider1.Validate())
                    return;

                WaitingManager.Show();
                SAR.EFMODEL.DataModels.SAR_REPORT_TEMPLATE updateDTO = new SAR.EFMODEL.DataModels.SAR_REPORT_TEMPLATE();

                if (this.currentData != null && this.currentData.ID > 0)
                {
                    LoadCurrent(this.currentData.ID, ref updateDTO);
                }

                UpdateDTOFromDataForm(ref updateDTO);
                if (ActionType == GlobalVariables.ActionAdd)
                {
                    updateDTO.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    var resultData = new BackendAdapter(param).Post<SAR.EFMODEL.DataModels.SAR_REPORT_TEMPLATE>(SarRequestUriStore.SAR_REPORT_TEMPLATE_CREATE, ApiConsumers.SarConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;
                        FillDataToGridControl();
                        ResetFormData();
                    }
                }
                else
                {
                    var resultData = new BackendAdapter(param).Post<SAR.EFMODEL.DataModels.SAR_REPORT_TEMPLATE>(SarRequestUriStore.SAR_REPORT_TEMPLATE_UPDATE, ApiConsumers.SarConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;
                        // UpdateRowDataAfterEdit(resultData);
                        FillDataToGridControl();
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

        private void UpdateRowDataAfterEdit(SAR_REPORT_TEMPLATE data)
        {
            try
            {
                if (data == null)
                    throw new ArgumentNullException("data(SAR.EFMODEL.DataModels.SAR_REPORT_TEMPLATE) is null");
                var rowData = (SAR.EFMODEL.DataModels.SAR_REPORT_TEMPLATE)gridView1.GetFocusedRow();
                if (rowData != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<SAR.EFMODEL.DataModels.SAR_REPORT_TEMPLATE>(rowData, data);
                    gridView1.RefreshRow(gridView1.FocusedRowHandle);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateDTOFromDataForm(ref SAR_REPORT_TEMPLATE currentDTO)
        {
            try
            {
                currentDTO.REPORT_TEMPLATE_CODE = txtID.Text.Trim();
                currentDTO.REPORT_TEMPLATE_NAME = txtName.Text.Trim();
                currentDTO.TUTORIAL = memoEdit1.Text.Trim();

                currentDTO.EXTENSION_RECEIVE = txtEXTENSION_RECEIVE.Text.Trim();
                //if (lkIcdChapterId.EditValue != null) currentDTO.ICD_CHAPTER_ID = Inventec.Common.TypeConvert.Parse.ToInt64((lkIcdChapterId.EditValue ?? "0").ToString());
                if (lkREPORT_TYPE_ID.EditValue != null) currentDTO.REPORT_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64((lkREPORT_TYPE_ID.EditValue ?? "0").ToString());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void simpleButton3_Click(object sender, EventArgs e)
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

        private void btnRf_Click(object sender, EventArgs e)
        {
            try
            {
                this.currentData = null;
                SetDefaultValue();
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider1);
                FillDataToGridControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void delete1_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var rowData = (SAR.EFMODEL.DataModels.V_SAR_REPORT_TEMPLATE)gridView1.GetFocusedRow();
                CommonParam param = new CommonParam();
                if (MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    SarReportTemplateFilter filter = new SarReportTemplateFilter();
                    filter.ID = rowData.ID;
                    var data = new BackendAdapter(param).Get<System.Collections.Generic.List<SAR_REPORT_TEMPLATE>>(SarRequestUriStore.SAR_REPORT_TEMPLATE_GET, ApiConsumers.SarConsumer, filter, param).FirstOrDefault();
                    if (rowData != null)
                    {
                        bool success = false;

                        success = new BackendAdapter(param).Post<bool>(SarRequestUriStore.SAR_REPORT_TEMPLATE_DELETE, ApiConsumers.SarConsumer, data, param);
                        if (success)
                        {
                            FillDataToGridControl();
                            SetDefaultValue();
                            currentData = ((List<V_SAR_REPORT_TEMPLATE>)gridControl1.DataSource).FirstOrDefault();
                            BackendDataWorker.Reset<SAR_REPORT_TEMPLATE>();
                            BackendDataWorker.Reset<V_SAR_REPORT_TEMPLATE>();
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

        private void btnlock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            SAR_REPORT_TEMPLATE success = new SAR_REPORT_TEMPLATE();
            bool result = false;
            try
            {
                V_SAR_REPORT_TEMPLATE data = (V_SAR_REPORT_TEMPLATE)gridView1.GetFocusedRow();
                if (MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    SAR_REPORT_TEMPLATE data1 = new SAR_REPORT_TEMPLATE();
                    data1.ID = data.ID;
                    WaitingManager.Show();
                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<SAR_REPORT_TEMPLATE>(SarRequestUriStore.SAR_REPORT_TEMPLATE_CHANGELOCK, ApiConsumers.SarConsumer, data1, param);
                    WaitingManager.Hide();
                    if (success != null)
                    {
                        result = true;
                        FillDataToGridControl();
                    }

                    #region Hien thi message thong bao
                    MessageManager.Show(this, param, result);
                    #endregion
                    #region hiện thị thông báo mất session
                    SessionManager.ProcessTokenLost(param);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnUnlock_ButtonClick_1(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            SAR_REPORT_TEMPLATE success = new SAR_REPORT_TEMPLATE();
            bool notHandler = false;
            try
            {
                V_SAR_REPORT_TEMPLATE data = (V_SAR_REPORT_TEMPLATE)gridView1.GetFocusedRow();
                if (MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    SAR_REPORT_TEMPLATE data1 = new SAR_REPORT_TEMPLATE();
                    data1.ID = data.ID;
                    WaitingManager.Show();
                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<SAR_REPORT_TEMPLATE>(SarRequestUriStore.SAR_REPORT_TEMPLATE_CHANGELOCK, ApiConsumers.SarConsumer, data1, param);
                    WaitingManager.Hide();
                    if (success != null)
                    {
                        notHandler = true;
                        FillDataToGridControl();
                    }

                    #region Hien thi message thong bao
                    MessageManager.Show(this, param, notHandler);
                    #endregion
                    #region hiện thị thông báo mất session
                    SessionManager.ProcessTokenLost(param);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtName_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtEXTENSION_RECEIVE.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void memoEdit1_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (btnEdit1.Enabled)
            {
                btnEdit1.Focus();
                simpleButton3_Click(null, null);
            }
        }

        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnFind_Click(null, null);
        }

        private void barButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnRf_Click(null, null);
        }

        private void txtSearch_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            //    try
            //    {
            //        if (e.KeyCode == Keys.Enter)
            //        {
            //            btnFind_Click(null, null);
            //        }
            //        else if (e.KeyCode == Keys.Down)
            //        {
            //            gridView1.Focus();
            //            gridView1.FocusedRowHandle = 0;
            //            var rowData = (SAR.EFMODEL.DataModels.V_SAR_REPORT_TEMPLATE)gridView1.GetFocusedRow();
            //            if (rowData != null)
            //            {
            //                Changedatarow(rowData);
            //            }
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        Inventec.Common.Logging.LogSystem.Warn(ex);
            //    }
        }

        private void txtID_KeyUp_1(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtName.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtEXTENSION_RECEIVE_KeyUp_1(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    lkREPORT_TYPE_ID.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void lkREPORT_TYPE_ID_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    memoEdit1.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void down1_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                string url = "";
                var row = (SAR.EFMODEL.DataModels.V_SAR_REPORT_TEMPLATE)gridView1.GetFocusedRow();

                if (!string.IsNullOrEmpty(row.REPORT_TEMPLATE_URL))
                {
                    var a = Newtonsoft.Json.JsonConvert.DeserializeObject<TemplateADO>(row.REPORT_TEMPLATE_URL);
                    //var cat1 = row.REPORT_TEMPLATE_URL.Split(',');
                    //string cat1Str = cat1[1];

                    //var cat2 = cat1Str.Split(':');
                    //var cat2Str = cat2[1];
                    //url = cat2Str.Remove(0, 1);
                    //url = url.Remove(url.LastIndexOf('"'), 1);
                    url = a.URL;
                }

                if (!string.IsNullOrEmpty(url))
                {
                    using (MemoryStream stream = Inventec.Fss.Client.FileDownload.GetFile(url))
                    {
                        if (stream == null)
                        {
                            // MessageBox.Show("dữ liệu file rỗng");
                            DevExpress.XtraEditors.XtraMessageBox.Show("Dữ liệu file rỗng");
                            return;
                        }

                        var extensionCheck = row.EXTENSION_RECEIVE;
                        if (extensionCheck == null) extensionCheck = "xlsx";
                        if (extensionCheck.ToLower() == "pdf")
                        {
                            saveFileDialog1.Filter = "pdf files (*.pdf)|*.pdf";
                            saveFileDialog1.FileName = url.Split('\\').LastOrDefault();
                            saveFileDialog1.FileOk += saveFileDialog1_FileOk;
                        }
                        else
                        {
                            saveFileDialog1.Filter = "Excel 2007 later file (*.xlsx)|*.xlsx|Excel 97-2003 file(*.xls)|*.xls|Pdf file(*.pdf)|*.pdf";
                            saveFileDialog1.FileName = url.Split('\\').LastOrDefault();
                        }

                        if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            using (var fileStream = new FileStream(@"" + saveFileDialog1.FileName, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                            {
                                if (fileStream == null)
                                {
                                    DevExpress.XtraEditors.XtraMessageBox.Show("Dữ liệu rỗng ");
                                    return;
                                }

                                var extension = Path.GetExtension(saveFileDialog1.FileName);
                                if (extension.ToLower() == ".xlsx" || extension.ToLower() == ".xls")
                                {
                                    stream.CopyTo(fileStream);
                                }
                                else if (extension.ToLower() == ".pdf")
                                {
                                    using (Workbook workbook = new Workbook())
                                    {
                                        bool rs = workbook.LoadDocument(stream, DocumentFormat.OpenXml);
                                        workbook.ExportToPdf(fileStream);
                                    }
                                }

                                DevExpress.XtraEditors.XtraMessageBox.Show("Tải thành công");
                                if (System.Windows.Forms.MessageBox.Show("Bạn có muốn mở file bây giờ không?", "Hỏi đáp", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                                    System.Diagnostics.Process.Start(saveFileDialog1.FileName);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DevExpress.XtraEditors.XtraMessageBox.Show("Tải thất bại");
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void saveFileDialog1_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            System.Windows.Forms.SaveFileDialog sv = (sender as System.Windows.Forms.SaveFileDialog);
            if (Path.GetExtension(saveFileDialog1.FileName).ToLower() != ".pdf")
            {
                e.Cancel = true;
                DevExpress.XtraEditors.XtraMessageBox.Show("Hãy nhập định dạng file là'pdf'");
                return;
            }
        }

        private bool UploadFile(SarReportTemplateSDO entity)
        {
            bool success = false;
            //string DownloadReports ;
            try
            {
                List<FileHolder> fileHolders = new List<FileHolder>();
                if (entity.FileUpload != null && entity.FileUpload.Count > 0)
                {
                    SarReportTypeFilter filter = new SarReportTypeFilter();
                    filter.ID = entity.REPORT_TYPE_ID;
                    SAR_REPORT_TYPE reportType = new BackendAdapter(new CommonParam()).Get<List<SAR_REPORT_TYPE>>("api/SarReportType/Get", ApiConsumers.SarConsumer, filter, new CommonParam()).FirstOrDefault();
                    for (int i = 0; i < entity.FileUpload.Count; i++)
                    {
                        string duoi = entity.FileUpload[i].FileName.Split('.').Last();
                        FileHolder file = new FileHolder();
                        if (reportType != null)
                        {
                            file.FileName = reportType.REPORT_TYPE_CODE + "_" + reportType.REPORT_TYPE_NAME + "_" + entity.REPORT_TEMPLATE_CODE + "_" + entity.REPORT_TEMPLATE_NAME + "." + duoi;
                        }
                        else
                        {
                            file.FileName = "" + "_" + "" + "_" + entity.REPORT_TEMPLATE_CODE + "_" + entity.REPORT_TEMPLATE_NAME + "." + duoi;
                        }
                        file.Content = new System.IO.MemoryStream();
                        entity.FileUpload[i].InputStream.CopyTo(file.Content);
                        file.Content.Position = 0;
                        fileHolders.Add(file);
                    }

                    // fileUploadInfos = FileUpload.UploadFile("SAR", ReportTemplate.SAR_REPORT_TEMPLATE, fileHolders, true);
                    if (fileUploadInfos != null && fileUploadInfos.Count > 0)
                    {
                        success = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return success;
        }

        private void upload1_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (SAR.EFMODEL.DataModels.V_SAR_REPORT_TEMPLATE)gridView1.GetFocusedRow();
                SAR_REPORT_TEMPLATE x = new SAR_REPORT_TEMPLATE();
                Inventec.Common.Mapper.DataObjectMapper.Map<SAR_REPORT_TEMPLATE>(x, row);
                frmup frm = new frmup(ResultFileUpload, x);
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ResultFileUpload()
        {
            FillDataToGridControl();
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                List<object> listArgs = new List<object>();
                listArgs.Add((RefeshReference)FillDataToGridControl);
                if (this.moduleData != null)
                {
                    CallModule callModule = new CallModule(CallModule.SarImportReportTemplate, moduleData.RoomId, moduleData.RoomTypeId, listArgs);
                }
                else
                {
                    CallModule callModule = new CallModule(CallModule.SarImportReportTemplate, 0, 0, listArgs);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnImportFile_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                List<object> listArgs = new List<object>();
                listArgs.Add((RefeshReference)FillDataToGridControl);
                if (this.moduleData != null)
                {
                    CallModule callModule = new CallModule(CallModule.SarImportFileReportTemplate, moduleData.RoomId, moduleData.RoomTypeId, listArgs);
                }
                else
                {
                    CallModule callModule = new CallModule(CallModule.SarImportFileReportTemplate, 0, 0, listArgs);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnImport_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                saveFileDialog.FileName = "";
                if (this.DataChecks != null && this.DataChecks.Count > 0)
                {
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        WaitingManager.Show();
                        bool success = false;
                        CommonParam param = new CommonParam();
                        Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                        Inventec.Common.FlexCellExport.Store store = new Inventec.Common.FlexCellExport.Store();
                        Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();


                        string fileName = System.IO.Path.Combine(Application.StartupPath + "\\Tmp\\Exp\\", "EXPORT_REPORT_TEMPLATE.xls");
                        store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                        store.SetCommonFunctions();
                        objectTag.AddObjectData(store, "ExportResult", this.DataChecks);

                        WaitingManager.Hide();
                        success = store.OutFile(saveFileDialog.FileName);
                        if (!success)
                            return;
                        DevExpress.XtraEditors.XtraMessageBox.Show("Xuất file thành công");
                        if (DevExpress.XtraEditors.XtraMessageBox.Show("Bạn có muốn mở file ngay?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            System.Diagnostics.Process.Start(saveFileDialog.FileName);
                        }
                    }
                }
                else
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Dữ liệu mẫu báo cáo rỗng", "Thông báo");
                    return;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkLock_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                FillDataToGridControl();
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void chkLock_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            try
            {
                FillDataToGridControl();
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void btnExportFile_Click(object sender, EventArgs e)
        {
            try
            {
                bool success = false;
                CommonParam param = new CommonParam();
                if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string path = folderBrowserDialog.SelectedPath.ToString();
                    WaitingManager.Show();

                    foreach (var item in this.DataChecks)
                    {
                        try
                        {
                            string url = "";
                            string filename;
                            var cat1 = item.REPORT_TEMPLATE_URL.Split(',');
                            string cat1Str = cat1[1];

                            var cat2 = cat1Str.Split(':');
                            var cat2Str = cat2[1];
                            url = cat2Str.Remove(0, 1);
                            url = url.Remove(url.LastIndexOf('"'), 1);
                            using (MemoryStream stream = Inventec.Fss.Client.FileDownload.GetFile(url))
                            {
                                if (stream != null)
                                {
                                    saveFileDialog1.Filter = "Excel 2007 later file (*.xlsx)|*.xlsx|Excel 97-2003 file(*.xls)|*.xls|Pdf file(*.pdf)|*.pdf";
                                    filename = url.Split('.').LastOrDefault();
                                    saveFileDialog1.FileName = item.REPORT_TEMPLATE_CODE + "." + filename;
                                }

                                using (var fileStream = new FileStream(path + @"\" + saveFileDialog1.FileName, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                                {
                                    stream.CopyTo(fileStream);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            LogSystem.Warn(ex);
                        }
                    }

                    success = true;
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

        private void FillDataToComboStatus()
        {
            try
            {
                List<Status> cboStauts = new List<Status>();
                Status all = new Status();
                all.ID = 1;
                all.Name = "Tất cả";
                cboStauts.Add(all);

                Status Lock = new Status();
                Lock.ID = 2;
                Lock.Name = "Khóa";
                cboStauts.Add(Lock);

                Status Unlock = new Status();
                Unlock.ID = 3;
                Unlock.Name = "Mở khóa";
                cboStauts.Add(Unlock);

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("Name", "", 200, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("Name", "ID", columnInfos, false, 220);
                ControlEditorLoader.Load(this.CboStatus, cboStauts, controlEditorADO);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void CboStatus_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            if (e.CloseMode == PopupCloseMode.Normal)
            {
                FillDataToGridControl();
            }
        }
    }
}
