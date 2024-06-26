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
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.Message;
using Inventec.UC.Paging;
using SCN.EFMODEL.DataModels;
using SCN.Filter;
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

namespace HIS.Desktop.Plugins.ScnVaccination.Run
{
    public partial class frmScnVaccination : HIS.Desktop.Utility.FormBase
    {
        internal Inventec.Desktop.Common.Modules.Module currentModule;
        internal string _PatientCode = "";

        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        PagingGrid pagingGrid;
        int ActionType = -1;
        int positionHandle = -1;

        V_SCN_VACCINATION currentData;

        public frmScnVaccination()
        {
            InitializeComponent();
        }

        public frmScnVaccination(Inventec.Desktop.Common.Modules.Module _currentModule, string _patientCode)
		:base(_currentModule)
        {
            InitializeComponent();
            try
            {
                this.currentModule = _currentModule;
                this._PatientCode = _patientCode;
                lblPatientCode.Text = _patientCode;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmScnVaccination_Load(object sender, EventArgs e)
        {
            try
            {
                this.ActionType = GlobalVariables.ActionAdd;
                txtKeyword.Text = "";

                SetIcon();
                FillDataToGridControl();

                FillDataToControlsForm();

                //Set enable control default
                EnableControlChanged(this.ActionType);

                ValidateForm();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void SetIcon()
        {
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);

                if (this.currentModule != null && !String.IsNullOrEmpty(this.currentModule.text))
                {
                    this.Text = this.currentModule.text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void FillDataToControlsForm()
        {
            try
            {
                InitComboVaccinationType();
                InitComboVaccinationGroup();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboVaccinationType()
        {
            try
            {
                SCN.Filter.ScnVaccinationTypeFilter filter = new ScnVaccinationTypeFilter();
                var rs = new BackendAdapter(new CommonParam()).Get<List<SCN_VACCINATION_TYPE>>("api/ScnVaccinationType/Get", ApiConsumers.ScnConsumer, filter, new CommonParam());
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("VACCINATION_TYPE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("VACCINATION_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("VACCINATION_TYPE_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cbo, rs, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboVaccinationGroup()
        {
            try
            {
                SCN.Filter.ScnVaccinationGroupFilter filter = new ScnVaccinationGroupFilter();
                var rs = new BackendAdapter(new CommonParam()).Get<List<SCN_VACCINATION_GROUP>>("api/ScnVaccinationGroup/Get", ApiConsumers.ScnConsumer, filter, new CommonParam());
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("VACCINATION_GROUP_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("VACCINATION_GROUP_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("VACCINATION_GROUP_NAME", "VACCINATION_GROUP_CODE", columnInfos, false, 350);
                ControlEditorLoader.Load(cboVacinGroup, rs, controlEditorADO);
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
                ValidationSingleControl(cbo);
                ValidationSingleControl(cboVacinGroup);
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

        /// <summary>
        /// Ham lay du lieu theo dieu kien tim kiem va gan du lieu vao danh sach
        /// </summary>
        public void FillDataToGridControl()
        {
            try
            {
                WaitingManager.Show();
                int pageSize = 0;
                if (ucPaging1.pagingGrid != null)
                {
                    pageSize = ucPaging1.pagingGrid.PageSize;
                }
                else
                {
                    pageSize = (int)ConfigApplications.NumPageSize;
                }
                LoadPaging(new CommonParam(0, pageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(LoadPaging, param, pageSize);
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
                Inventec.Core.ApiResultObject<List<SCN.EFMODEL.DataModels.V_SCN_VACCINATION>> apiResult = null;
                ScnVaccinationViewFilter filter = new ScnVaccinationViewFilter();
                SetFilterNavBar(ref filter);
                dnNavigation.DataSource = null;
                gridViewVaccination.BeginUpdate();
                apiResult = new BackendAdapter(paramCommon).GetRO<List<SCN.EFMODEL.DataModels.V_SCN_VACCINATION>>(HisRequestUriStore.SCN_VACCINATION_GET_VIEW, ApiConsumers.ScnConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    var data = (List<SCN.EFMODEL.DataModels.V_SCN_VACCINATION>)apiResult.Data;
                    if (data != null)
                    {

                        dnNavigation.DataSource = data;
                        gridViewVaccination.GridControl.DataSource = data;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                }
                gridViewVaccination.EndUpdate();

                #region Process has exception
                //SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void SetFilterNavBar(ref ScnVaccinationViewFilter filter)
        {
            try
            {
                filter.KEY_WORD = txtKeyword.Text.Trim();
                filter.PATIENT_CODE__EXACT = this._PatientCode;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void gridViewVaccination_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    SCN.EFMODEL.DataModels.V_SCN_VACCINATION pData = (SCN.EFMODEL.DataModels.V_SCN_VACCINATION)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + startPage; //+ ((pagingGrid.CurrentPage - 1) * pagingGrid.PageSize);
                    }
                    else if (e.Column.FieldName == "CREATE_TIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(pData.CREATE_TIME ?? 0);
                    }
                    else if (e.Column.FieldName == "MODIFY_TIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(pData.MODIFY_TIME ?? 0);
                    }
                    else if (e.Column.FieldName == "EXECUTE_TIME_DISPLAY")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(pData.EXECUTE_TIME ?? 0);
                    }
                    else if (e.Column.FieldName == "APPOINTMENT_TIME_DISPLAY")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(pData.APPOINTMENT_TIME ?? 0);
                    }
                    else if (e.Column.FieldName == "IS_DONE_DISPLAY")
                    {
                        if (pData.IS_DONE > 0 && pData.IS_DONE == 1)
                        {
                            e.Value = "Đã tiêm";
                        }
                        else
                            e.Value = "Chưa tiêm";
                    }

                    gridControlVaccination.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewVaccination_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                this.currentData = (V_SCN_VACCINATION)gridViewVaccination.GetFocusedRow();
                if (this.currentData != null)
                {
                    ChangedDataRow(this.currentData);
                    this.ActionType = GlobalVariables.ActionEdit;
                    //Set focus vào control editor đầu tiên
                    SetFocusEditor();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void ChangedDataRow(V_SCN_VACCINATION data)
        {
            try
            {
                if (data != null)
                {
                    FillDataToEditorControl(data);
                    this.ActionType = GlobalVariables.ActionEdit;
                    EnableControlChanged(this.ActionType);

                    //Disable nút sửa nếu dữ liệu đã bị khóa
                    btnEdit.Enabled = (this.currentData.IS_ACTIVE == IMSys.DbConfig.SCN_RS.COMMON.IS_ACTIVE__TRUE);

                    positionHandle = -1;
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderEditorInfo, dxErrorProvider);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToEditorControl(V_SCN_VACCINATION data)
        {
            try
            {
                if (data != null)
                {
                    dtNgayHenTiem.EditValue = null;
                    dtThoiGianTiem.EditValue = null;
                    //spinThangThai.Value = null;

                    lblPatientCode.Text = data.PATIENT_CODE;
                    cbo.EditValue = data.VACCINATION_TYPE_ID;
                    txtPhanUng.Text = data.REACTION;
                    if (data.APPOINTMENT_TIME > 0)
                        dtNgayHenTiem.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(data.APPOINTMENT_TIME ?? 0) ?? DateTime.Now;
                    if (data.EXECUTE_TIME > 0)
                        dtThoiGianTiem.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(data.EXECUTE_TIME ?? 0) ?? DateTime.Now;
                    if (data.IS_DONE == IMSys.DbConfig.SCN_RS.SCN_VACCINATION.IS_DONE__TRUE)
                        chkIsDone.Checked = true;
                    else
                        chkIsDone.Checked = false;
                    spinThangThai.Value = Inventec.Common.TypeConvert.Parse.ToDecimal(data.PREGNANCY_MONTH.ToString());
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
                SCN.EFMODEL.DataModels.SCN_VACCINATION updateDTO = new SCN.EFMODEL.DataModels.SCN_VACCINATION();


                UpdateDTOFromDataForm(ref updateDTO);
                if (ActionType == GlobalVariables.ActionAdd)
                {
                    var resultData = new BackendAdapter(param).Post<SCN.EFMODEL.DataModels.SCN_VACCINATION>(HisRequestUriStore.SCN_VACCINATION_CREATE, ApiConsumers.ScnConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;
                        FillDataToGridControl();
                        this.ActionType = GlobalVariables.ActionEdit;
                        EnableControlChanged(this.ActionType);
                        Inventec.Common.Mapper.DataObjectMapper.Map<V_SCN_VACCINATION>(this.currentData, resultData);
                    }
                }
                else
                {
                    if (this.currentData != null && this.currentData.ID > 0)
                    {
                        updateDTO.ID = this.currentData.ID;
                    }
                    var resultData = new BackendAdapter(param).Post<SCN.EFMODEL.DataModels.SCN_VACCINATION>(HisRequestUriStore.SCN_VACCINATION_UPDATE, ApiConsumers.ScnConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;
                        UpdateRowDataAfterEdit(resultData);
                        gridControlVaccination.RefreshDataSource();
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
                //SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateRowDataAfterEdit(SCN.EFMODEL.DataModels.SCN_VACCINATION data)
        {
            try
            {
                if (data == null)
                    throw new ArgumentNullException("data(SCN.EFMODEL.DataModels.SCN_VACCINATION) is null");
                var rowData = (V_SCN_VACCINATION)gridViewVaccination.GetFocusedRow();
                if (rowData != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<V_SCN_VACCINATION>(rowData, data);
                    gridViewVaccination.RefreshRow(gridViewVaccination.FocusedRowHandle);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateDTOFromDataForm(ref SCN.EFMODEL.DataModels.SCN_VACCINATION currentDTO)
        {
            try
            {
                currentDTO.PATIENT_CODE = lblPatientCode.Text.Trim();
                currentDTO.GROUP_CODE = cboVacinGroup.EditValue.ToString();
                if (cbo.EditValue != null) currentDTO.VACCINATION_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cbo.EditValue ?? "0").ToString());
                if (dtThoiGianTiem.EditValue != null && dtThoiGianTiem.DateTime != DateTime.MinValue)
                {
                    currentDTO.EXECUTE_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtThoiGianTiem.DateTime);
                }
                if (dtNgayHenTiem.EditValue != null && dtNgayHenTiem.DateTime != DateTime.MinValue)
                {
                    currentDTO.APPOINTMENT_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtNgayHenTiem.DateTime);
                }
                if (chkIsDone.Checked)
                {
                    currentDTO.IS_DONE = IMSys.DbConfig.SCN_RS.SCN_VACCINATION.IS_DONE__TRUE;
                }
                if (spinThangThai.Value > 0)
                {
                    currentDTO.PREGNANCY_MONTH = Inventec.Common.TypeConvert.Parse.ToInt16(spinThangThai.Value.ToString());
                }
                currentDTO.REACTION = txtPhanUng.Text;
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

        private void gridViewVaccination_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    SCN.EFMODEL.DataModels.V_SCN_VACCINATION data = (SCN.EFMODEL.DataModels.V_SCN_VACCINATION)gridViewVaccination.GetRow(e.RowHandle);
                    if (data == null)
                        return;
                    if (e.Column.FieldName == "DELETE")
                    {
                        var creator = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                        if (creator.Trim() == data.CREATOR && data.IS_ACTIVE == IMSys.DbConfig.SCN_RS.COMMON.IS_ACTIVE__TRUE)
                        {
                            e.RepositoryItem = repositoryItemButton__Delete;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemButton__Delete_Disable;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void dnNavigation_PositionChanged(object sender, EventArgs e)
        {
            try
            {
                this.currentData = (V_SCN_VACCINATION)(gridViewVaccination.DataSource as List<V_SCN_VACCINATION>)[dnNavigation.Position];
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

        private void gridViewVaccination_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    var rowData = (V_SCN_VACCINATION)gridViewVaccination.GetFocusedRow();
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

        private void txtKeyword_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch_Click(null, null);
                }
                else if (e.KeyCode == Keys.Down)
                {
                    gridViewVaccination.Focus();
                    gridViewVaccination.FocusedRowHandle = 0;
                    var rowData = (V_SCN_VACCINATION)gridViewVaccination.GetFocusedRow();
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

        private void repositoryItemButton__Delete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                var data = (V_SCN_VACCINATION)gridViewVaccination.GetFocusedRow();
                if (data != null)
                {
                    bool success = false;
                    success = new BackendAdapter(param).Post<bool>(HisRequestUriStore.SCN_VACCINATION_DELETE, ApiConsumers.ScnConsumer, data.ID, param);
                    if (success)
                    {
                        FillDataToGridControl();
                        currentData = ((List<V_SCN_VACCINATION>)gridControlVaccination.DataSource).FirstOrDefault();
                    }
                    MessageManager.Show(this, param, success);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (btnSearch.Enabled)
                    btnSearch_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (btnAdd.Enabled)
                    btnAdd_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (btnEdit.Enabled)
                    btnEdit_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (btnCancel.Enabled)
                    btnCancel_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
