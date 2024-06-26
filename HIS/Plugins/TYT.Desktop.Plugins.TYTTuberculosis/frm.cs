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
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.Location;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using Inventec.UC.Paging;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TYT.Desktop.Plugins.TYTTuberculosis.ADO;
using TYT.Desktop.Plugins.TYTTuberculosis.Validtion;
using TYT.EFMODEL.DataModels;
using TYT.Filter;

namespace TYT.Desktop.Plugins.TYTTuberculosis
{
    public partial class frm : HIS.Desktop.Utility.FormBase
    {

        internal Inventec.Desktop.Common.Modules.Module currentModule;

        V_HIS_PATIENT Patient { get; set; }

        TYT_TUBERCULOSIS currentData;
        int positionHandleControl = -1;

        ActionType actionType = ActionType.VIEW;

        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;

        V_HIS_ROOM room = null;

        enum ActionType
        {
            VIEW = 0,
            CREATE = 1,
            UPDATE = 2
        }

        public frm()
        {
            InitializeComponent();
        }

        public frm(Inventec.Desktop.Common.Modules.Module _currentModule, TYT_TUBERCULOSIS _Tuberculosis)
            : base(_currentModule)
        {
            InitializeComponent();
            SetIcon();
            this.currentModule = _currentModule;
            this.currentData = _Tuberculosis;
            if (this.currentModule != null)
            {
                this.Text = this.currentModule.text;
            }
            this.actionType = ActionType.UPDATE;
            btnRefresh.Enabled = false;
        }

        public frm(Inventec.Desktop.Common.Modules.Module _currentModule, V_HIS_PATIENT _patient)
            : base(_currentModule)
        {
            InitializeComponent();
            SetIcon();
            this.currentModule = _currentModule;
            this.Patient = _patient;
            if (this.currentModule != null)
            {
                this.Text = this.currentModule.text;
            }

            this.actionType = ActionType.CREATE;
        }

        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frm_Load(object sender, EventArgs e)
        {
            try
            {
                SetDefault();

                SetCaptionByLanguageKey();

                CommonParam param = new CommonParam();
                if (this.currentModule != null)
                {
                    MOS.Filter.HisRoomViewFilter roomFilter = new MOS.Filter.HisRoomViewFilter();
                    roomFilter.ID = this.currentModule.RoomId;
                    var ListRoom = new BackendAdapter(param).Get<List<V_HIS_ROOM>>("api/HisRoom/GetView", ApiConsumers.MosConsumer, roomFilter, param);
                    room = ListRoom != null && ListRoom.Count() > 0 ? ListRoom.FirstOrDefault() : null;
                }

                if (room != null)
                {
                    FillDataToGridControl();
                }

                if (this.Patient == null || this.currentData != null)
                {
                    this.Patient = new V_HIS_PATIENT();
                    this.Patient.PATIENT_CODE = this.currentData.PATIENT_CODE;
                    this.Patient.PERSON_CODE = this.currentData.PERSON_CODE;
                    this.Patient.FIRST_NAME = this.currentData.FIRST_NAME;
                    this.Patient.GENDER_NAME = this.currentData.GENDER_NAME;
                    this.Patient.LAST_NAME = this.currentData.LAST_NAME;
                    this.Patient.DOB = this.currentData.DOB;
                }

                EnableControlChanged(this.actionType);

                List<DiseaseTypeADO> result = GetDiseasseType();

                InItComboResult(result);

                //FillDataToControl();

                ValidateControl();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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

                LoadPaging(new CommonParam(0, (int)ConfigApplications.NumPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging.Init(LoadPaging, param);
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
                Inventec.Core.ApiResultObject<List<TYT.EFMODEL.DataModels.TYT_TUBERCULOSIS>> apiResult = null;
                TYT.Filter.TytTuberculosisFilter filter = new TytTuberculosisFilter();
                SetFilterNavBar(ref filter);
                gridView.BeginUpdate();

                Inventec.Common.Logging.LogSystem.Info("Du lieu dau vao TytTuberculosis/Get:" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => filter), filter));

                apiResult = new BackendAdapter(paramCommon).GetRO<List<TYT.EFMODEL.DataModels.TYT_TUBERCULOSIS>>("api/TytTuberculosis/Get", ApiConsumers.TytConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    var data = (List<TYT.EFMODEL.DataModels.TYT_TUBERCULOSIS>)apiResult.Data;
                    if (data != null)
                    {
                        gridView.GridControl.DataSource = data;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                }
                gridView.EndUpdate();

                #region Process has exception
                SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void SetFilterNavBar(ref TytTuberculosisFilter filter)
        {
            try
            {
                filter.ORDER_FIELD = "TYT_IN_TIME";
                filter.ORDER_DIRECTION = "DESC";
                filter.BRANCH_CODE__EXACT = this.room.BRANCH_CODE;
                if (this.Patient != null && this.room != null)
                {
                    filter.PATIENT_CODE__EXACT = this.Patient.PATIENT_CODE;
                    filter.BRANCH_CODE__EXACT = this.room.BRANCH_CODE;
                }
                if (this.currentData != null && this.currentData.ID > 0 && !btnRefresh.Enabled)
                {
                    filter.ID = this.currentData.ID;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private TYT_TUBERCULOSIS GetTubeculosisByPatient()
        {
            TYT_TUBERCULOSIS result = null;
            try
            {
                if (this.Patient == null || String.IsNullOrWhiteSpace(this.Patient.PATIENT_CODE))
                {
                    return null;
                }
                CommonParam param = new CommonParam();
                TYT.Filter.TytTuberculosisFilter filter = new Filter.TytTuberculosisFilter();
                filter.PATIENT_CODE__EXACT = this.Patient.PATIENT_CODE;
                var resultApi = new BackendAdapter(param).Get<List<TYT_TUBERCULOSIS>>("api/TytTuberculosis/Get", ApiConsumers.TytConsumer, filter, param);
                if (resultApi != null && resultApi.Count() > 0)
                {
                    result = resultApi.FirstOrDefault();
                }
                else
                {
                    result = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void InItComboResult(List<DiseaseTypeADO> result)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("Name", "", 150, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("Name", "ID", columnInfos, false, 200);
                ControlEditorLoader.Load(this.cboResult, result, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<DiseaseTypeADO> GetDiseasseType()
        {
            List<DiseaseTypeADO> result = new List<DiseaseTypeADO>();
            try
            {
                DiseaseTypeADO ado1 = new DiseaseTypeADO();
                ado1.ID = 1;
                ado1.Name = "Khỏi";
                result.Add(ado1);

                DiseaseTypeADO ado2 = new DiseaseTypeADO();
                ado2.ID = 2;
                ado2.Name = "Hoàn thành";
                result.Add(ado2);

                DiseaseTypeADO ado3 = new DiseaseTypeADO();
                ado3.ID = 3;
                ado3.Name = "Chuyển";
                result.Add(ado3);

                DiseaseTypeADO ado4 = new DiseaseTypeADO();
                ado4.ID = 4;
                ado4.Name = "Bỏ";
                result.Add(ado4);

                DiseaseTypeADO ado5 = new DiseaseTypeADO();
                ado5.ID = 5;
                ado5.Name = "Chết";
                result.Add(ado5);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void SetDefault()
        {
            try
            {
                if (this.actionType == ActionType.UPDATE)
                {
                    btnDelete.Enabled = true;
                }
                else
                {
                    btnDelete.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ChangedDataRow()
        {
            currentData = new TYT_TUBERCULOSIS();
            currentData = (TYT_TUBERCULOSIS)gridView.GetFocusedRow();

            if (currentData != null)
            {
                FillDataToControl();
                this.actionType = ActionType.UPDATE;
                EnableControlChanged(this.actionType);

                btnUpdate.Enabled = (this.currentData.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);

            }
        }

        private void EnableControlChanged(ActionType action)
        {
            btnSave.Enabled = (action == ActionType.CREATE);
            btnUpdate.Enabled = (action == ActionType.UPDATE);
        }

        private void FillDataToControl()
        {
            try
            {
                List<DiseaseTypeADO> dataSource = new List<DiseaseTypeADO>();

                if (this.currentData != null)
                {
                    if (this.currentData.TYT_IN_TIME != null)
                    {
                        dteTYTInTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.currentData.TYT_IN_TIME ?? 0) ?? DateTime.Now;
                    }
                    if (this.currentData.DTCKS_CODE != null)
                    {
                        txtDicks.EditValue = this.currentData.DTCKS_CODE;
                    }
                    if (this.currentData.DISEASE_TYPE != null)
                    {
                        if (this.currentData.DISEASE_TYPE == 1)
                        {
                            cboDiseaseType.EditValue = "AFB(+)";
                        }
                        if (this.currentData.DISEASE_TYPE == 2)
                        {
                            cboDiseaseType.EditValue = "AFB(-)";

                        }
                        if (this.currentData.DISEASE_TYPE == 3)
                        {
                            cboDiseaseType.EditValue = "NgPh";
                        }
                    }
                    if (this.currentData.TREATMENT_RESULT != null)
                    {
                        if (this.currentData.TREATMENT_RESULT == 1)
                        {
                            cboResult.EditValue = 1;
                        }
                        if (this.currentData.TREATMENT_RESULT == 2)
                        {
                            cboResult.EditValue = 2;
                        }
                        if (this.currentData.TREATMENT_RESULT == 3)
                        {
                            cboResult.EditValue = 3;
                        }
                        if (this.currentData.TREATMENT_RESULT == 4)
                        {
                            cboResult.EditValue = 4;
                        }
                        if (this.currentData.TREATMENT_RESULT == 5)
                        {
                            cboResult.EditValue = 5;
                        }
                    }
                    txtGhiChu.Text = this.currentData.NOTE;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SaveProcess()
        {
            try
            {
                if (!btnSave.Enabled && !btnUpdate.Enabled)
                {
                    return;
                }
                bool success = false;
                CommonParam param = new CommonParam();
                this.positionHandleControl = -1;
                if (!dxValidationProvider.Validate())
                    return;

                WaitingManager.Show();
                btnSave.Focus();

                TYT_TUBERCULOSIS updateDTO = new TYT_TUBERCULOSIS();
                if (this.currentData != null && this.currentData.ID > 0)
                {
                    updateDTO = this.currentData;
                }

                if (this.Patient != null)
                {
                    updateDTO.PATIENT_CODE = this.Patient.PATIENT_CODE;
                    updateDTO.PERSON_CODE = this.Patient.PERSON_CODE;
                    updateDTO.FIRST_NAME = this.Patient.FIRST_NAME;
                    updateDTO.LAST_NAME = this.Patient.LAST_NAME;
                    updateDTO.GENDER_NAME = this.Patient.GENDER_NAME;
                    updateDTO.DOB = this.Patient.DOB;
                }

                if (dteTYTInTime.EditValue != null)
                {
                    updateDTO.TYT_IN_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dteTYTInTime.DateTime);
                }
                else
                {
                    updateDTO.TYT_IN_TIME = null;
                }
                updateDTO.DTCKS_CODE = txtDicks.Text;
                if (cboDiseaseType.EditValue != null)
                {
                    if (cboDiseaseType.EditValue == "AFB(+)")
                    { updateDTO.DISEASE_TYPE = 1; }

                    if (cboDiseaseType.EditValue == "AFB(-)")
                    { updateDTO.DISEASE_TYPE = 2; }

                    if (cboDiseaseType.EditValue == "NgPh")
                    { updateDTO.DISEASE_TYPE = 3; }
                }
                else
                {
                    updateDTO.DISEASE_TYPE = null;
                }
                if (cboResult.EditValue != null)
                {
                    updateDTO.TREATMENT_RESULT = Int64.Parse(cboResult.EditValue.ToString());
                }
                else
                {
                    updateDTO.TREATMENT_RESULT = null;
                }
                updateDTO.NOTE = txtGhiChu.Text;
                updateDTO.BRANCH_CODE = WorkPlace.WorkPlaceSDO.FirstOrDefault(p => p.RoomId == this.currentModule.RoomId).BranchCode;

                TYT_TUBERCULOSIS _result = new TYT_TUBERCULOSIS();

                if (this.actionType == ActionType.CREATE)
                {
                    _result = new BackendAdapter(param).Post<TYT_TUBERCULOSIS>("api/TYTTuberculosis/Create", ApiConsumers.TytConsumer, updateDTO, param);
                }
                else if (updateDTO != null)
                {
                    _result = new BackendAdapter(param).Post<TYT_TUBERCULOSIS>("api/TYTTuberculosis/Update", ApiConsumers.TytConsumer, updateDTO, param);
                }

                if (_result != null && _result.ID > 0)
                {
                    success = true;
                    updateDTO = _result;
                    FillDataToGridControl();
                    this.actionType = ActionType.VIEW;
                    EnableControlChanged(this.actionType);
                }
                WaitingManager.Hide();
                MessageManager.Show(this, param, success);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveProcess();
        }

        private void barButtonItem__Save_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnRefesh_Click(object sender, EventArgs e)
        {
            try
            {
                this.actionType = ActionType.CREATE;
                this.SetDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonI__Refesh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnRefresh_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtDicks_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                cboDiseaseType.Focus();
                cboDiseaseType.ShowPopup();
            }
        }

        private void cboDiseaseType_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtDicks.Focus();
                txtDicks.SelectAll();
            }
        }

        private void cboResult_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtGhiChu.Focus();
            }
        }

        private void txtGhiChu_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

        }

        private void dteTYTInTime_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                cboResult.Focus();
            }
        }

        private void cboDiseaseType_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboDiseaseType.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void cboResult_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboResult.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.actionType == ActionType.UPDATE && this.currentData != null)
                {
                    bool success = false;
                    CommonParam param = new CommonParam();
                    WaitingManager.Show();
                    success = new BackendAdapter(param).Post<bool>("api/TYTTuberculosis/Delete", ApiConsumers.TytConsumer, this.currentData.ID, param);
                    WaitingManager.Hide();
                    MessageManager.Show(this, param, success);

                    if (success)
                    {
                        this.Dispose();
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void ValidateControl()
        {
            ValidMaxlengthTxtTick();
            ValidMaxlengthTxtGhiChu();
        }

        void ValidMaxlengthTxtTick()
        {
            try
            {
                ValidateMaxLength validateMaxLength = new ValidateMaxLength();
                validateMaxLength.textEdit = txtDicks;
                validateMaxLength.maxLength = 20;
                dxValidationProvider.SetValidationRule(txtDicks, validateMaxLength);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void ValidMaxlengthTxtGhiChu()
        {
            try
            {
                ValidateMaxLength validateMaxLength = new ValidateMaxLength();
                validateMaxLength.textEdit = txtGhiChu;
                validateMaxLength.maxLength = 100;
                dxValidationProvider.SetValidationRule(txtGhiChu, validateMaxLength);
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

                if (positionHandleControl == -1)
                {
                    positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandleControl > edit.TabIndex)
                {
                    positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridLookUpEdit1_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboResult.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboResult1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                cboDiseaseType.Focus();
            }
        }

        private void gridView_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    TYT.EFMODEL.DataModels.TYT_TUBERCULOSIS data = (TYT.EFMODEL.DataModels.TYT_TUBERCULOSIS)gridView.GetRow(e.RowHandle);

                    if (e.Column.FieldName == "DELETE")
                    {
                        if (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                        {
                            e.RepositoryItem = ButtonEditDeleteEnable;
                        }
                        else
                        {
                            e.RepositoryItem = ButtonEditDeleteDisable;
                        }
                    }
                    if (e.Column.FieldName == "LOCK")
                        e.RepositoryItem = data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? ButtonEditLock : ButtonEditUnlock;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    TYT.EFMODEL.DataModels.TYT_TUBERCULOSIS pData = (TYT.EFMODEL.DataModels.TYT_TUBERCULOSIS)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + startPage; //+ ((pagingGrid.CurrentPage - 1) * pagingGrid.PageSize);
                    }
                    else if (e.Column.FieldName == "CREATE_TIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(pData.CREATE_TIME ?? 0);
                    }
                    else if (e.Column.FieldName == "MODIFIER_TIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(pData.MODIFY_TIME ?? 0);
                    }
                    else if (e.Column.FieldName == "TYT_IN_TIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(pData.TYT_IN_TIME ?? 0);
                    }
                    else if (e.Column.FieldName == "DOB_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(pData.DOB);
                    }
                    else if (e.Column.FieldName == "TREATMENT_RESULT_STR")
                    {
                        if (pData.TREATMENT_RESULT == 1)
                            e.Value = "Khỏi";
                        else if (pData.TREATMENT_RESULT == 2)
                            e.Value = "Hoàn thành";
                        else if (pData.TREATMENT_RESULT == 3)
                            e.Value = "Chuyển";
                        else if (pData.TREATMENT_RESULT == 4)
                            e.Value = "Bỏ";
                        else if (pData.TREATMENT_RESULT == 5)
                            e.Value = "Chết";
                    }
                    else if (e.Column.FieldName == "DISEASE_TYPE_STR")
                    {
                        if (pData.DISEASE_TYPE == 1)
                            e.Value = "AFB(+)";
                        else if (pData.DISEASE_TYPE == 2)
                            e.Value = "AFB(-)";
                        else if (pData.DISEASE_TYPE == 3)
                            e.Value = "NgPh";
                    }
                    gridControl.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {

        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                bool success = false;
                CommonParam param = new CommonParam();
                this.positionHandleControl = -1;
                if (!dxValidationProvider.Validate())
                    return;

                WaitingManager.Show();
                btnUpdate.Focus();
                if (this.currentData != null)
                {
                    this.actionType = ActionType.UPDATE;
                }
                else
                {
                    this.currentData = new TYT_TUBERCULOSIS();
                    Inventec.Common.Mapper.DataObjectMapper.Map<TYT_TUBERCULOSIS>(this.currentData, this.Patient);
                    this.currentData.PERSON_ADDRESS = this.Patient.VIR_ADDRESS;
                    this.currentData.VIR_PERSON_NAME = this.Patient.VIR_PATIENT_NAME;
                }
                if (dteTYTInTime.EditValue != null)
                {
                    this.currentData.TYT_IN_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dteTYTInTime.DateTime);
                }
                else
                {
                    this.currentData.TYT_IN_TIME = null;
                }
                this.currentData.DTCKS_CODE = txtDicks.Text;
                if (cboDiseaseType.EditValue != null)
                {
                    if (cboDiseaseType.EditValue == "AFB(+)")
                    { this.currentData.DISEASE_TYPE = 1; }

                    if (cboDiseaseType.EditValue == "AFB(-)")
                    { this.currentData.DISEASE_TYPE = 2; }

                    if (cboDiseaseType.EditValue == "NgPh")
                    { this.currentData.DISEASE_TYPE = 3; }
                }
                else
                {
                    this.currentData.DISEASE_TYPE = null;
                }
                if (cboResult.EditValue != null)
                {
                    this.currentData.TREATMENT_RESULT = Int64.Parse(cboResult.EditValue.ToString());
                    //if (cboResult.EditValue == "Khỏi")
                    //{ this.Tuberculosis.TREATMENT_RESULT = 1; }
                    //if (cboResult.EditValue == "Hoàn thành")
                    //{ this.Tuberculosis.TREATMENT_RESULT = 2; }
                    //if (cboResult.EditValue == "Chuyển")
                    //{ this.Tuberculosis.TREATMENT_RESULT = 3; }
                    //if (cboResult.EditValue == "Bỏ")
                    //{ this.Tuberculosis.TREATMENT_RESULT = 4; }
                    //if (cboResult.EditValue == "Chết")
                    //{ this.Tuberculosis.TREATMENT_RESULT = 5; }
                }
                else
                {
                    this.currentData.TREATMENT_RESULT = null;
                }
                this.currentData.NOTE = txtGhiChu.Text;
                this.currentData.BRANCH_CODE = WorkPlace.WorkPlaceSDO.FirstOrDefault(p => p.RoomId == this.currentModule.RoomId).BranchCode;

                TYT_TUBERCULOSIS _result = new TYT_TUBERCULOSIS();

                _result = new BackendAdapter(param).Post<TYT_TUBERCULOSIS>("api/TYTTuberculosis/Update", ApiConsumers.TytConsumer, this.currentData, param);

                if (_result != null && _result.ID > 0)
                {
                    success = true;
                    this.currentData = _result;
                    this.actionType = ActionType.UPDATE;
                }
                WaitingManager.Hide();
                MessageManager.Show(this, param, success);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                this.actionType = ActionType.CREATE;
                this.currentData = new TYT_TUBERCULOSIS();
                EnableControlChanged(this.actionType);
                ResetFormData();
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError
                (dxValidationProvider, dxErrorProvider);
                dteTYTInTime.Focus();
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
                dteTYTInTime.EditValue = null;
                cboResult.EditValue = null;
                cboDiseaseType.EditValue = null;
                txtDicks.Text = "";
                txtGhiChu.Text = "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView_Click(object sender, EventArgs e)
        {
            try
            {
                var focus = (TYT_TUBERCULOSIS)gridView.GetFocusedRow();
                if (focus != null)
                {
                    this.actionType = ActionType.UPDATE;
                    this.currentData = focus;
                    EnableControlChanged(this.actionType);
                    this.btnUpdate.Enabled = (this.currentData.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);
                    FillDataToControl();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ButtonEditDeleteEnable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                this.currentData = (TYT.EFMODEL.DataModels.TYT_TUBERCULOSIS)gridView.GetFocusedRow();
                if (this.currentData != null)
                {
                    bool success = false;
                    CommonParam param = new CommonParam();
                    WaitingManager.Show();
                    success = new BackendAdapter(param).Post<bool>("api/TYTTuberculosis/Delete", ApiConsumers.TytConsumer, this.currentData.ID, param);
                    WaitingManager.Hide();
                    MessageManager.Show(this, param, success);

                    if (success)
                    {
                        FillDataToGridControl();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtGhiChu_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Tab)
                {
                    if (btnSave.Enabled)
                    {
                        btnSave.Focus();
                    }
                    else
                    {
                        btnUpdate.Focus();
                    }
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnUpdate_Click(null, null);
        }

        private void ButtonEditLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                TYT.EFMODEL.DataModels.TYT_TUBERCULOSIS resultLock = new TYT.EFMODEL.DataModels.TYT_TUBERCULOSIS();
                bool notHandler = false;

                TYT.EFMODEL.DataModels.TYT_TUBERCULOSIS currentLock = (TYT.EFMODEL.DataModels.TYT_TUBERCULOSIS)gridView.GetFocusedRow();
                if (MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong),
                  "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    WaitingManager.Show();
                    resultLock = new BackendAdapter(param).Post<TYT.EFMODEL.DataModels.TYT_TUBERCULOSIS>
                      ("api/TytTuberculosis/ChangeLock", ApiConsumers.TytConsumer, currentLock, param);

                    if (resultLock != null)
                    {
                        notHandler = true;
                        FillDataToGridControl();
                    }

                    MessageManager.Show(this.ParentForm, param, notHandler);
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ButtonEditUnlock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                TYT.EFMODEL.DataModels.TYT_TUBERCULOSIS resultLock = new TYT.EFMODEL.DataModels.TYT_TUBERCULOSIS();
                bool notHandler = false;

                TYT.EFMODEL.DataModels.TYT_TUBERCULOSIS currentLock = (TYT.EFMODEL.DataModels.TYT_TUBERCULOSIS)gridView.GetFocusedRow();
                if (MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong),
                  "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    WaitingManager.Show();
                    resultLock = new BackendAdapter(param).Post<TYT.EFMODEL.DataModels.TYT_TUBERCULOSIS>
                      ("api/TytTuberculosis/ChangeLock", ApiConsumers.TytConsumer, currentLock, param);

                    if (resultLock != null)
                    {
                        notHandler = true;
                        FillDataToGridControl();
                    }
                    WaitingManager.Hide();

                    MessageManager.Show(this.ParentForm, param, notHandler);
                    
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
