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
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using TYT.Desktop.Plugins.Nerves.ADO;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TYT.EFMODEL.DataModels;
using HIS.Desktop.ADO;
using Inventec.Common.Controls.EditorLoader;
using Newtonsoft.Json;
using TYT.Desktop.Plugins.Nerves.Validtion;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;

namespace TYT.Desktop.Plugins.Nerves
{
    public partial class frmTYTNerves : FormBase
    {

        public enum TYPE
        {
            VIEW,
            CREATE,
            UPDATE
        }

        TYPE actionType { get; set; }
        int positionHandleControl = -1;
        V_HIS_PATIENT patient { get; set; }
        Inventec.Desktop.Common.Modules.Module moduleData;
        TYT_NERVES currentData { get; set; }
        long nervesId { get; set; }
        DelegateSelectData refeshData { get; set; }
        //TytNerverADO nerverADO { get; set; }
        int positionHandle = -1;
        int rowCount;
        int dataTotal;
        int startPage;
        int limit;
        bool isUpdateOne = false;

        public frmTYTNerves(Inventec.Desktop.Common.Modules.Module moduleData, V_HIS_PATIENT patient)
            : base(moduleData)
        {
            InitializeComponent();
            try
            {
                this.patient = patient;
                this.moduleData = moduleData;
                this.actionType = TYPE.CREATE;
                isUpdateOne = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public frmTYTNerves(Inventec.Desktop.Common.Modules.Module moduleData, TYT_NERVES currentData, DelegateSelectData refeshData)
        {
            InitializeComponent();
            try
            {
                //this.nerverADO = nerverADO;
                this.moduleData = moduleData;
                this.refeshData = refeshData;
                this.actionType = TYPE.UPDATE;
                this.currentData = currentData;
                this.isUpdateOne = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public frmTYTNerves(Inventec.Desktop.Common.Modules.Module moduleData, long nervesId, DelegateSelectData refeshData)
        {
            InitializeComponent();
            try
            {
                this.nervesId = nervesId;
                this.moduleData = moduleData;
                this.refeshData = refeshData;
                this.actionType = TYPE.UPDATE;
                this.isUpdateOne = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmTYTNerves_Load(object sender, EventArgs e)
        {
            try
            {
                //Load Icon
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
                if (this.moduleData != null)
                {
                    this.Text = this.moduleData.text;
                }
                FillDataToGridControl();
                LoadDataDefault();
                ValidateControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ResetControl()
        {
            try
            {
                txtYear.Text = "";
                mmTamThanPhanLiet.Text = "";
                mmDongKinh.Text = "";
                mmTramCam.Text = "";
                chk1.CheckState = CheckState.Unchecked;
                chk2.CheckState = CheckState.Unchecked;
                chk3.CheckState = CheckState.Unchecked;
                chk4.CheckState = CheckState.Unchecked;
                chk5.CheckState = CheckState.Unchecked;
                chk6.CheckState = CheckState.Unchecked;
                chk7.CheckState = CheckState.Unchecked;
                chk8.CheckState = CheckState.Unchecked;
                chk9.CheckState = CheckState.Unchecked;
                chk10.CheckState = CheckState.Unchecked;
                chk11.CheckState = CheckState.Unchecked;
                chk12.CheckState = CheckState.Unchecked;
                chkHomeCheck.CheckState = CheckState.Unchecked;
                cboPHCN.EditValue = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToGridControl()
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
            //ucPaging.Init(loadPaging, param);
            ucPaging.Init(LoadPaging, param, numPageSize, gridControlNerves);

            WaitingManager.Hide();
        }

        private void LoadPaging(object param)
        {
            startPage = ((CommonParam)param).Start ?? 0;
            limit = ((CommonParam)param).Limit ?? 0;

            CommonParam paramCommon = new CommonParam(startPage, limit);

            var currentRoom = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.moduleData.RoomId);

            TYT.Filter.TytNervesFilter filterSearch = new TYT.Filter.TytNervesFilter();
            filterSearch.KEY_WORD = txtKeyword.Text.Trim();
            filterSearch.ORDER_FIELD = "YEAR";
            filterSearch.ORDER_DIRECTION = "DESC";

            if (currentRoom != null)
            {
                filterSearch.BRANCH_CODE__EXACT = currentRoom.BRANCH_CODE;
            }

            if (this.patient != null)
            {
                filterSearch.PATIENT_CODE__EXACT = this.patient.PATIENT_CODE;
            }

            if ((this.nervesId > 0 || (this.currentData != null && this.currentData.ID > 0)) && this.isUpdateOne == true)
            {
                filterSearch.ID = this.nervesId > 0 ? this.nervesId : this.currentData.ID;
                btnNew.Enabled = false;
                this.actionType = TYPE.UPDATE;
            }
            EnableControlChanged(this.actionType);
            Inventec.Core.ApiResultObject<List<TYT.EFMODEL.DataModels.TYT_NERVES>> apiResult = null;
            apiResult = new BackendAdapter(paramCommon).GetRO<List<TYT.EFMODEL.DataModels.TYT_NERVES>>
              ("api/TytNerves/Get", ApiConsumers.TytConsumer, filterSearch, paramCommon);
            if (apiResult != null)
            {
                var data = (List<TYT.EFMODEL.DataModels.TYT_NERVES>)apiResult.Data;
                gridViewNerves.GridControl.DataSource = data;
                rowCount = (data == null ? 0 : data.Count);
                dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
            }
        }

        void ValidateControl()
        {
            try
            {
                ValidMaxlength(500, mmTamThanPhanLiet);
                ValidMaxlength(500, mmTramCam);
                ValidMaxlength(500, mmDongKinh);
                ValidSpinYear(false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void ValidMaxlength(int maxlength, DevExpress.XtraEditors.BaseEdit baseEdit)
        {
            try
            {
                ValidateMaxLength validateMaxLength = new ValidateMaxLength();
                validateMaxLength.textEdit = baseEdit;
                validateMaxLength.maxLength = maxlength;
                validateMaxLength.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider.SetValidationRule(baseEdit, validateMaxLength);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void ValidSpinYear(bool visibleControl)
        {
            try
            {
                SpinYearValidation validateMaxLength = new SpinYearValidation();
                validateMaxLength.spin = txtYear;
                validateMaxLength.visibleControl = visibleControl;
                validateMaxLength.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider.SetValidationRule(txtYear, validateMaxLength);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void ValidCboYear(bool visibleControl)
        {
            try
            {
                cboYearValidation validateMaxLength = new cboYearValidation();
                validateMaxLength.visibleControl = visibleControl;
                validateMaxLength.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                //dxValidationProvider.SetValidationRule(cboYear, validateMaxLength);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InItNerves(List<TYT.EFMODEL.DataModels.TYT_NERVES> result)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("YEAR", "", 100, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("YEAR", "YEAR", columnInfos, false, 100);
                //ControlEditorLoader.Load(this.cboYear, result, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<TYT.EFMODEL.DataModels.TYT_NERVES> GetNerverByPatient(string branchCode, string patientCode)
        {
            List<TYT.EFMODEL.DataModels.TYT_NERVES> result = new List<TYT_NERVES>();
            try
            {
                CommonParam param = new CommonParam();
                TYT.Filter.TytNervesFilter filter = new Filter.TytNervesFilter();
                filter.PATIENT_CODE__EXACT = patientCode;
                filter.BRANCH_CODE__EXACT = branchCode;
                result = new BackendAdapter(param).Get<List<TYT_NERVES>>("api/TytNerves/Get", ApiConsumers.TytConsumer, filter, param);
                if (result != null && result.Count() > 0)
                {
                    result = result != null && result.Count() > 0 ? result.OrderByDescending(o => o.YEAR).ToList() : result;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private List<TYT.EFMODEL.DataModels.TYT_NERVES> GetNerverByID(long id)
        {
            List<TYT.EFMODEL.DataModels.TYT_NERVES> result = new List<TYT_NERVES>();
            try
            {
                CommonParam param = new CommonParam();
                TYT.Filter.TytNervesFilter filter = new Filter.TytNervesFilter();
                filter.ID = id;
                result = new BackendAdapter(param).Get<List<TYT_NERVES>>("api/TytNerves/Get", ApiConsumers.TytConsumer, filter, param);
                if (result != null && result.Count() > 0)
                {
                    result = result != null && result.Count() > 0 ? result.OrderByDescending(o => o.YEAR).ToList() : result;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private MOS.EFMODEL.DataModels.V_HIS_PATIENT GetPatient(string patientCode)
        {
            MOS.EFMODEL.DataModels.V_HIS_PATIENT result = new MOS.EFMODEL.DataModels.V_HIS_PATIENT();
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisPatientViewFilter filter = new MOS.Filter.HisPatientViewFilter();
                filter.PATIENT_CODE__EXACT = patientCode;
                var resultApi = new BackendAdapter(param).Get<List<V_HIS_PATIENT>>("api/HisPatient/GetView", ApiConsumers.MosConsumer, filter, param);
                if (resultApi != null && resultApi.Count() > 0)
                {
                    result = resultApi.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void barButtonItemCtrlS_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (btnSave.Enabled)
                    btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessSave()
        {
            try
            {
                bool success = false;
                this.positionHandleControl = -1;
                if (!dxValidationProvider.Validate())
                    return;

                TYT_NERVES nerves = new TYT_NERVES();
                if (this.currentData != null && this.currentData.ID > 0)
                {
                    nerves = this.currentData;
                }

                MakeDataNerves(ref nerves);

                WaitingManager.Show();
                CommonParam param = new CommonParam();
                TYT_NERVES result = new BackendAdapter(param)
                    .Post<TYT_NERVES>(
                    actionType == TYPE.CREATE ? "api/TytNerves/Create" : "api/TytNerves/Update",
                    ApiConsumers.TytConsumer, nerves, param);
                WaitingManager.Hide();
                if (result != null)
                {
                    success = true;
                    //nervesId = 0;
                    //this.currentData = new TYT_NERVES();
                    this.actionType = TYPE.UPDATE;
                    FillDataToGridControl();
                    EnableControlChanged(this.actionType);
                    btnEdit.Enabled = false;

                    if (refeshData != null)
                    {
                        this.refeshData(result);
                    }
                }

                MessageManager.Show(this, param, success);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            ProcessSave();
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

        private void cboThang_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPHCN_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboPHCN.Properties.Buttons[1].Visible = false;
                    cboPHCN.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPHCN_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (cboPHCN.EditValue != null)
                {
                    cboPHCN.Properties.Buttons[1].Visible = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void bbtnNew_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnNew_Click(null, null);
        }

        private void ResetCheckYear()
        {
            try
            {
                chk1.CheckState = CheckState.Unchecked;
                chk2.CheckState = CheckState.Unchecked;
                chk3.CheckState = CheckState.Unchecked;
                chk4.CheckState = CheckState.Unchecked;
                chk5.CheckState = CheckState.Unchecked;
                chk6.CheckState = CheckState.Unchecked;
                chk7.CheckState = CheckState.Unchecked;
                chk8.CheckState = CheckState.Unchecked;
                chk9.CheckState = CheckState.Unchecked;
                chk10.CheckState = CheckState.Unchecked;
                chk11.CheckState = CheckState.Unchecked;
                chk12.CheckState = CheckState.Unchecked;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                ResetCheckYear();
                chkHomeCheck.CheckState = CheckState.Unchecked;
                cboPHCN.EditValue = null;
                txtYear.Text = "";
                mmTamThanPhanLiet.Text = "";
                mmDongKinh.Text = "";
                mmTramCam.Text = "";
                btnDelete.Enabled = false;
                this.actionType = TYPE.CREATE;
                this.currentData = null;
                ValidSpinYear(true);
                EnableControlChanged(this.actionType);
                mmTamThanPhanLiet.Focus();
                mmTamThanPhanLiet.SelectAll();
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
                WaitingManager.Show();
                bool success = false;
                CommonParam param = new CommonParam();
                success = new BackendAdapter(param).Post<bool>("api/TytNerves/Delete", ApiConsumers.TytConsumer, currentData.ID, param);
                WaitingManager.Hide();
                if (success)
                {
                    this.Dispose();
                    this.Close();
                }
                if (refeshData != null && success)
                {
                    this.refeshData(success);
                }

                MessageManager.Show(this, param, success);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            ProcessSave();
        }

        private void bbtnEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnEdit_Click(null, null);
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            FillDataToGridControl();
        }

        private void ChangedDataRow()
        {
            currentData = new TYT_NERVES();
            currentData = (TYT_NERVES)gridViewNerves.GetFocusedRow();

            if (currentData != null)
            {
                this.actionType = TYPE.UPDATE;
                EnableControlChanged(this.actionType);

                btnEdit.Enabled = (this.currentData.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);
                positionHandle = -1;
                LoadDataDefault();
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError
                  (dxValidationProvider, dxErrorProvider);
            }
        }

        private void EnableControlChanged(TYPE action)
        {
            btnSave.Enabled = (action == TYPE.CREATE);
            btnEdit.Enabled = (action == TYPE.UPDATE);
        }

        private void btnSearch_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                    btnSearch_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewNerves_Click(object sender, EventArgs e)
        {
            ChangedDataRow();
        }

        private void gridViewNerves_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    TYT_NERVES data = (TYT_NERVES)gridViewNerves.GetRow(e.RowHandle);
                    if (e.Column.FieldName == "DELETE1")
                        e.RepositoryItem = data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? ButtonEditDeleteEnable : ButtonEditDeleteDisable;
                    else if (e.Column.FieldName == "IS_HOME_CHECK_STR")
                    {
                        if (data.IS_HOME_CHECK == 1)
                            e.RepositoryItem = ButtonEdit_CoKTTaiNha;
                        else
                            e.RepositoryItem = null;
                    }
                    else if (e.Column.FieldName == "LOCK")
                        e.RepositoryItem = data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? ButtonEdit_Lock : ButtonEdit_Unlock;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewNerves_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {

                TYT_NERVES data = (TYT_NERVES)gridViewNerves.GetRow(e.ListSourceRowIndex);
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;

                if (e.Column.FieldName == "STT")
                    e.Value = e.ListSourceRowIndex + 1 + startPage;
                if (e.Column.FieldName == "STATUS")
                    e.Value = data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? "đang hoạt động" : "đã khóa";

                if (e.Column.FieldName == "CREATE_TIME_STR")
                {
                    string createTime = (view.GetRowCellValue(e.ListSourceRowIndex, "CREATE_TIME") ?? "").ToString();
                    e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString
                      (Inventec.Common.TypeConvert.Parse.ToInt64(createTime));
                }
                if (e.Column.FieldName == "MODIFY_TIME_STR")
                {
                    string mobdifyTime = (view.GetRowCellValue(e.ListSourceRowIndex, "MODIFY_TIME") ?? "").ToString();
                    e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString
                      (Inventec.Common.TypeConvert.Parse.ToInt64(mobdifyTime));
                }

                if (e.Column.FieldName == "PHCN_RESULT_STR")
                {
                    if (data.PHCN_RESULT == 1)
                    {
                        e.Value = "Tốt";
                    }
                    else if (data.PHCN_RESULT == 2)
                    {
                        e.Value = "Trung bình";
                    }
                    else if (data.PHCN_RESULT == 3)
                    {
                        e.Value = "Kém";
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
            if (e.KeyCode == Keys.Enter)
            {
                btnSearch_Click(null, null);
            }
        }

        private void bbtnSearch_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnSearch_Click(null, null);
        }

        private void ButtonEditDeleteEnable_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {

                bool success = false;
                CommonParam param = new CommonParam();
                currentData = (TYT.EFMODEL.DataModels.TYT_NERVES)gridViewNerves.GetFocusedRow();
                if (currentData != null)
                {
                    WaitingManager.Show();
                    success = new BackendAdapter(param).Post<bool>("api/TytNerves/Delete", ApiConsumers.TytConsumer, currentData.ID, param);
                    WaitingManager.Hide();
                }

                if (success)
                {
                    currentData = new TYT_NERVES();
                    FillDataToGridControl();
                }

                if (refeshData != null && success)
                {
                    this.refeshData(success);
                }

                MessageManager.Show(this, param, success);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ButtonEdit_Lock_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                TYT.EFMODEL.DataModels.TYT_NERVES resultLock = new TYT.EFMODEL.DataModels.TYT_NERVES();
                bool notHandler = false;

                TYT.EFMODEL.DataModels.TYT_NERVES currentLock = (TYT.EFMODEL.DataModels.TYT_NERVES)gridViewNerves.GetFocusedRow();
                if (MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong),
                  "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    WaitingManager.Show();
                    resultLock = new BackendAdapter(param).Post<TYT.EFMODEL.DataModels.TYT_NERVES>
                      ("api/TytNerves/ChangeLock", ApiConsumers.TytConsumer, currentLock, param);

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

        private void ButtonEdit_Unlock_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                TYT.EFMODEL.DataModels.TYT_NERVES resultLock = new TYT.EFMODEL.DataModels.TYT_NERVES();
                bool notHandler = false;

                TYT.EFMODEL.DataModels.TYT_NERVES currentLock = (TYT.EFMODEL.DataModels.TYT_NERVES)gridViewNerves.GetFocusedRow();
                if (MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong),
                  "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    WaitingManager.Show();
                    resultLock = new BackendAdapter(param).Post<TYT.EFMODEL.DataModels.TYT_NERVES>
                      ("api/TytNerves/ChangeLock", ApiConsumers.TytConsumer, currentLock, param);

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
    }
}
