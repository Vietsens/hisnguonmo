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
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.HisExamSchedule.ADO;
using HIS.Desktop.Plugins.HisExamSchedule.Validate;
using HIS.Desktop.Utilities.Extensions;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
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

namespace HIS.Desktop.Plugins.HisExamSchedule.SetupWorkRoom
{
    public partial class frmSetupWorkRoom : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        int ActionType = -1;
        int positionHandle = -1;
        List<HIS_EXAM_SCHEDULE> lstHisExamSchedule = new List<HIS_EXAM_SCHEDULE>();
        HIS_EXAM_SCHEDULE currentData;
        List<DayADO> LstDay;
        List<DoctorADO> lstDoctorADO;

        long roomID;

        #endregion

        #region Construct
        public frmSetupWorkRoom(long _roomID)
        {
            InitializeComponent();
            this.roomID = _roomID;
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        private void frmSetupWorkRoom_Load(object sender, EventArgs e)
        {
            try
            {
                //tao du lieu cho lstday
                loadlstday();
                //tao du lieu cho lstDoctorADO
                loadllstDoctorADO();

                LoadComboDoctor();

                LoadComboDate();

                FillDataToGridControl();

                SetDefaultValue();

                //Set enable control default
                EnableControlChanged(this.ActionType);

                //Set validate rule
                ValidateForm();

                //Focus default
                SetDefaultFocus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        /// <summary>
        /// Gan focus vao control mac dinh
        /// </summary>
        private void SetDefaultFocus()
        {
            try
            {
                txtKeyWord.Focus();
                txtKeyWord.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private void ValidateForm()
        {
            try
            {
                ValidateDoctor();
                ValidateGridLookupWithTextEdit(cboRoomName, txtRoomCode, dxValidationProvider1);
                ValidationSingleControl(cboWorkingDay, dxValidationProvider1);
                ValidationTime();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }


        private void ValidateDoctor()
        {
            try
            {
                ValidationDoctor validateDoctpr = new ValidationDoctor();
                validateDoctpr.cboDoctor = this.cboDoctorName;
                dxValidationProvider1.SetValidationRule(cboDoctorName, validateDoctpr);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        private void ValidationTime()
        {
            try
            {
                ValidationTime validateTimeFromAndTimeTo = new ValidationTime();
                validateTimeFromAndTimeTo.tmTimeFrom = tmTimeFrom;
                validateTimeFromAndTimeTo.tmTimeTo = tmTimeTo;
                dxValidationProvider1.SetValidationRule(tmTimeFrom, validateTimeFromAndTimeTo);
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
                //ResetFormData();
                txtKeyWord.Text = "";

                cboWorkingDay.EditValue = null;
                tmTimeFrom.EditValue = null;
                tmTimeTo.EditValue = null;

                GridCheckMarksSelection gridCheckMark = cboDoctorName.Properties.Tag as GridCheckMarksSelection;
                gridCheckMark.ClearSelection(cboDoctorName.Properties.View);

                cboDoctorName.EditValue = null;

                var Room = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.roomID);
                LoadComboRoom(Room);
                if (Room != null)
                {
                    txtRoomCode.Text = Room.ROOM_CODE;
                    cboRoomName.EditValue = Room.ID;
                } 

                //Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider1);

                EnableControlChanged(this.ActionType);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
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

        private void loadllstDoctorADO()
        {
            try
            {
                lstDoctorADO = new List<DoctorADO>();

                CommonParam param = new CommonParam();

                //HIS_USER_ROOM 

                HisUserRoomFilter filter = new HisUserRoomFilter();
                filter.ROOM_ID = roomID;
                var userRooms = new BackendAdapter(param).Get<List<HIS_USER_ROOM>>(
                                HIS.Desktop.ApiConsumer.HisRequestUriStore.HIS_USER_ROOM_GET,
                                ApiConsumers.MosConsumer,
                                filter,
                                param);

                if (userRooms != null && userRooms.Count > 0)
                {
                    HisEmployeeFilter EmployeeFilter = new HisEmployeeFilter();
                    EmployeeFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    EmployeeFilter.LOGINNAMEs = userRooms.Select(o => o.LOGINNAME).ToList();

                    var hisEmployee = new BackendAdapter(param).Get<List<HIS_EMPLOYEE>>("api/HisEmployee/Get", ApiConsumers.MosConsumer, EmployeeFilter, param);

                    //var hisEmployee = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_EMPLOYEE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.IS_DOCTOR == 1).ToList();

                    if (hisEmployee != null && hisEmployee.Count > 0)
                    {
                        foreach (var item in hisEmployee)
                        {
                            var user = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().FirstOrDefault(o => o.LOGINNAME.ToUpper() == item.LOGINNAME.ToUpper());
                            if (user != null)
                            {
                                DoctorADO Doctor = new DoctorADO();

                                Doctor.LoginName = item.LOGINNAME;
                                Doctor.UseName = user.USERNAME;

                                lstDoctorADO.Add(Doctor);
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


        private void loadlstday()
        {
            try
            {
                LstDay = new List<DayADO>()
                {
                    new DayADO(1,"Chủ nhật"),
                    new DayADO(2,"Thứ 2"),
                    new DayADO(3,"Thứ 3"),
                    new DayADO(4,"Thứ 4"),
                    new DayADO(5,"Thứ 5"),
                    new DayADO(6,"Thứ 6"),
                    new DayADO(7,"Thứ 7"),
                };
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadComboDate()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("NAME", "", 200, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("NAME", "ID", columnInfos, false, 200);
                ControlEditorLoader.Load(cboWorkingDay, LstDay, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadComboDoctor()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LoginName", "", 100, 1));
                columnInfos.Add(new ColumnInfo("UseName", "", 200, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("UseName", "LoginName", columnInfos, false, 300);
                ControlEditorLoader.Load(cboDoctorName, lstDoctorADO, controlEditorADO);

                InitCheck(cboDoctorName, SelectionGrid__Doctor);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadComboRoom(V_HIS_ROOM data)
        {
            try
            {
                List<V_HIS_ROOM> lstdata = new List<V_HIS_ROOM>();
                lstdata.Add(data);

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("ROOM_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("ROOM_NAME", "", 200, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("ROOM_NAME", "ID", columnInfos, false, 300);
                ControlEditorLoader.Load(cboRoomName, lstdata, controlEditorADO);
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
                lstHisExamSchedule = new List<HIS_EXAM_SCHEDULE>();
                CommonParam param = new CommonParam();

                HisExamScheduleFilter filter = new HisExamScheduleFilter();
                filter.ROOM_ID = this.roomID;

                if (!String.IsNullOrEmpty(txtKeyWord.Text))
                {
                    filter.KEY_WORD = txtKeyWord.Text;
                }
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";

                lstHisExamSchedule = new BackendAdapter(param).Get<List<HIS_EXAM_SCHEDULE>>(HisRequestUriStore.HIS_EXAM_SCHEDULE_GET, ApiConsumers.MosConsumer, filter, param);
              
                gridView1.GridControl.DataSource = lstHisExamSchedule;
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
                CommonParam param = new CommonParam();
                
                    bool success = false;
                    if (!btnEdit.Enabled && !btnAdd.Enabled)
                        return;

                    positionHandle = -1;
                    if (!dxValidationProvider1.Validate())
                        return;

                    WaitingManager.Show();
                    MOS.EFMODEL.DataModels.HIS_EXAM_SCHEDULE updateDTO = new MOS.EFMODEL.DataModels.HIS_EXAM_SCHEDULE();

                    if (this.currentData != null && this.currentData.ID > 0)
                    {
                        LoadCurrent(this.currentData.ID, ref updateDTO);
                    }

                    UpdateDTOFromDataForm(ref updateDTO);

                    if (ActionType == GlobalVariables.ActionAdd)
                    {
                        updateDTO.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                        var resultData = new BackendAdapter(param).Post<HIS_EXAM_SCHEDULE>(HisRequestUriStore.HIS_EXAM_SCHEDULE_CREATE, ApiConsumers.MosConsumer, updateDTO, param);
                        if (resultData != null)
                        {
                            success = true;
                            FillDataToGridControl();
                            SetDefaultValue();
                        }
                    }
                    else
                    {
                        var resultData = new BackendAdapter(param).Post<HIS_EXAM_SCHEDULE>(HisRequestUriStore.HIS_EXAM_SCHEDULE_UPDATE, ApiConsumers.MosConsumer, updateDTO, param);
                        if (resultData != null)
                        {
                            success = true;
                            FillDataToGridControl();
                        }
                    }

                    if (success)
                    {
                        HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Reset<HIS_EXAM_SCHEDULE>();
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
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        private void UpdateDTOFromDataForm(ref HIS_EXAM_SCHEDULE updateDTO)
        {
            try
            {

                updateDTO.ROOM_ID = this.roomID;

                GridCheckMarksSelection gridCheckMark = cboDoctorName.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null && gridCheckMark.SelectedCount > 0)
                {
                    List<string> LoginNames = new List<string>();
                    List<string> UserNames = new List<string>();
                    foreach (DoctorADO rv in gridCheckMark.Selection)
                    {
                        if (rv != null && !LoginNames.Contains(rv.LoginName))
                            LoginNames.Add(rv.LoginName);

                        if (rv != null && !UserNames.Contains(rv.UseName))
                            UserNames.Add(rv.UseName);
                    }

                    updateDTO.LOGINNAME = String.Join(";", LoginNames); ;

                    updateDTO.USERNAME = String.Join(";", UserNames); ;
                }

                cboDoctorName.ResetText();

                
                if (cboWorkingDay.EditValue != null)
                {
                    updateDTO.DAY_OF_WEEK = (long)cboWorkingDay.EditValue;
                }

                tmTimeFrom.DeselectAll();
                if (tmTimeFrom.EditValue != null)
                    updateDTO.TIME_FROM = String.Format("{0:00}{1:00}", tmTimeFrom.TimeSpan.Hours, tmTimeFrom.TimeSpan.Minutes);
                else
                    updateDTO.TIME_FROM = "0000";

                tmTimeTo.DeselectAll();
                if (tmTimeTo.EditValue != null)
                    updateDTO.TIME_TO = String.Format("{0:00}{1:00}", tmTimeTo.TimeSpan.Hours, tmTimeTo.TimeSpan.Minutes);
                else
                    updateDTO.TIME_TO = "0000";

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadCurrent(long currentId, ref HIS_EXAM_SCHEDULE currentDTO)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisExamScheduleFilter filter = new HisExamScheduleFilter();
                filter.ID = currentId;
                currentDTO = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_EXAM_SCHEDULE>>(HisRequestUriStore.HIS_EXAM_SCHEDULE_GET, ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
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

        private void btnReset_Click(object sender, EventArgs e)
        {
            try
            {
                SetDefaultValue();
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider1);
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
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        private void bbtnSearch_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSearch_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
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
                Inventec.Common.Logging.LogSystem.Error(ex);
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
                Inventec.Common.Logging.LogSystem.Error(ex);
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
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDoctorName_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    GridCheckMarksSelection gridCheckMark = cboDoctorName.Properties.Tag as GridCheckMarksSelection;
                    gridCheckMark.ClearSelection(cboDoctorName.Properties.View);

                    cboDoctorName.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboWorkingDay_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboWorkingDay.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void tmTimeFrom_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    tmTimeFrom.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void tmTimeTo_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    tmTimeTo.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnGDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            btnEdit.Enabled = false;
            try
            {
                CommonParam param = new CommonParam();
                var rowData = (HIS_EXAM_SCHEDULE)gridView1.GetFocusedRow();
                if (MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {


                    if (rowData != null)
                    {
                        bool success = false;
                        success = new BackendAdapter(param).Post<bool>(HisRequestUriStore.HIS_EXAM_SCHEDULE_DELETE, ApiConsumers.MosConsumer, rowData.ID, param);
                        if (success)
                        {
                            FillDataToGridControl();
                            currentData = ((List<HIS_EXAM_SCHEDULE>)gridControl1.DataSource).FirstOrDefault();

                            HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Reset<HIS_EXAM_SCHEDULE>();
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


        private void cboWorkingDay_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboWorkingDay.EditValue != null)
                {
                    cboWorkingDay.Properties.Buttons[1].Visible = true;
                }
                else
                {
                    cboWorkingDay.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void tmTimeFrom_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (tmTimeFrom.EditValue != null)
                {
                    tmTimeFrom.Properties.Buttons[1].Visible = true;
                }
                else
                {
                    tmTimeFrom.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void tmTimeTo_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (tmTimeTo.EditValue != null)
                {
                    tmTimeTo.Properties.Buttons[1].Visible = true;
                }
                else
                {
                    tmTimeTo.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView1_Click(object sender, EventArgs e)
        {
            try
            {
                var rowData = (MOS.EFMODEL.DataModels.HIS_EXAM_SCHEDULE)gridView1.GetFocusedRow();
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
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void ChangedDataRow(HIS_EXAM_SCHEDULE data)
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

        private void FillDataToEditorControl(HIS_EXAM_SCHEDULE data)
        {
            try
            {
                if (data != null)
                {
                    //cboDoctorName.EditValue = data.LOGINNAME;

                    GridCheckMarksSelection gridCheckDoctor = cboDoctorName.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheckDoctor != null)
                    {
                        gridCheckDoctor.ClearSelection(cboDoctorName.Properties.View);

                        cboDoctorName.EditValue = null;
                    }
                    if (!String.IsNullOrWhiteSpace(data.LOGINNAME) && gridCheckDoctor != null)
                    {
                        ProcessSelectService(data, gridCheckDoctor);
                    }

                    cboWorkingDay.EditValue = data.DAY_OF_WEEK;

                    if (!String.IsNullOrWhiteSpace(data.TIME_FROM) && data.TIME_FROM.Length == 4)
                        tmTimeFrom.EditValue = String.Format("{0:00}:{1:00}", data.TIME_FROM.Substring(0, 2), data.TIME_FROM.Substring(2, 2));
                    else
                        tmTimeFrom.EditValue = "0000";

                    if (!String.IsNullOrWhiteSpace(data.TIME_TO) && data.TIME_TO.Length == 4)
                        tmTimeTo.EditValue = String.Format("{0:00}:{1:00}", data.TIME_TO.Substring(0, 2), data.TIME_TO.Substring(2, 2));
                    else
                        tmTimeTo.EditValue = "0000";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        private void gridView1_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {

                    HIS_EXAM_SCHEDULE data = (HIS_EXAM_SCHEDULE)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    
                    if (e.Column.FieldName == "DELETE")
                    {
                        e.RepositoryItem = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? btnGDelete : btnGEnable);

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
                    HIS_EXAM_SCHEDULE pData = (HIS_EXAM_SCHEDULE)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;

                    var Room = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == pData.ROOM_ID);

                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }

                    if (e.Column.FieldName == "ROOM_CODE_STR")
                    {
                        e.Value = Room != null ? Room.ROOM_CODE : null;
                    }

                    if (e.Column.FieldName == "ROOM_NAME_STR")
                    {
                        e.Value = Room != null ? Room.ROOM_NAME : null;
                    }

                    if (e.Column.FieldName == "DEPARTMENT_NAME_STR")
                    {
                        e.Value = Room != null ? Room.DEPARTMENT_NAME : null;
                    }

                    if (e.Column.FieldName == "DOCTOR_STR")
                    {
                        string[] arraysLoginname = pData.LOGINNAME.Split(';');
                        string[] arraysUsername = pData.USERNAME.Split(';');

                        if (arraysLoginname != null && arraysLoginname.Length > 0 && arraysUsername != null && arraysUsername.Length > 0)
                        {
                            for (int i = 0; i < arraysLoginname.Length; i++)
                            {
                                e.Value += arraysLoginname[i] + " - " + arraysUsername[i] + "; ";
                            }
                        }
                    }

                    if (e.Column.FieldName == "DAY_OF_WEEK_STR")
                    {
                        try
                        {
                            e.Value = LstDay.FirstOrDefault(o => o.ID == pData.DAY_OF_WEEK).NAME;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error("Lỗi khi set giá trị DAY_OF_WEEK_STR", ex);
                        }
                    }

                    if (e.Column.FieldName == "TIME_FROM_STR")
                    {
                        e.Value = !String.IsNullOrWhiteSpace(pData.TIME_FROM) && pData.TIME_FROM.Length == 4 ? String.Format("{0}:{1}", pData.TIME_FROM.Substring(0, 2), pData.TIME_FROM.Substring(2, 2)) : "";
                    }

                    if (e.Column.FieldName == "TIME_TO_STR")
                    {
                        e.Value = !String.IsNullOrWhiteSpace(pData.TIME_TO) && pData.TIME_TO.Length == 4 ? String.Format("{0}:{1}", pData.TIME_TO.Substring(0, 2), pData.TIME_TO.Substring(2, 2)) : "";
                    }

                    if (e.Column.FieldName == "CREATE_TIME_STR")
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

                    if (e.Column.FieldName == "MODIFY_TIME_STR")
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

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        private void gridView1_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    var rowData = (MOS.EFMODEL.DataModels.HIS_EXAM_SCHEDULE)gridView1.GetFocusedRow();
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

        private void txtKeyWord_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    FillDataToGridControl();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtRoomCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboRoomName.Focus();
                    cboRoomName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            } 
        }

        private void cboRoomName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboDoctorName.Focus();
                    cboDoctorName.SelectAll();
                    cboDoctorName.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            } 
        }

        private void cboWorkingDay_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    tmTimeFrom.Focus();
                    tmTimeFrom.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }


        private void tmTimeFrom_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    tmTimeTo.Focus();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void tmTimeTo_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.ActionType == GlobalVariables.ActionAdd)
                        btnAdd.Focus();
                    else
                        btnEdit.Focus();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDoctorName_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                //if (e.CloseMode == PopupCloseMode.Normal)
                //{
                    //if (cboDoctorName.EditValue != null)
                    //{
                    //    var aDoctor = lstDoctorADO.FirstOrDefault(o => o.LoginName.ToUpper() == cboDoctorName.EditValue.ToString().ToUpper());
                    //    if (aDoctor != null)
                    //    {
                    //        cboWorkingDay.Focus();
                    //        cboWorkingDay.ShowPopup();
                    //    }
                    //    else
                    //        cboDoctorName.ShowPopup();
                    //}

                    GridCheckMarksSelection gridCheckMark = cboDoctorName.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheckMark != null && gridCheckMark.SelectedCount > 0)
                    {
                        cboWorkingDay.Focus();
                    }
                    else
                    {
                       //cboDoctorName.ShowPopup();
                    }
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboWorkingDay_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboWorkingDay.EditValue != null)
                    {
                        tmTimeFrom.Focus();
                        tmTimeFrom.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void tmTimeTo_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (this.ActionType == GlobalVariables.ActionAdd)
                        btnAdd.Focus();
                    else
                        btnEdit.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDoctorName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboDoctorName.EditValue != null)
                    {
                        var aDoctor = lstDoctorADO.FirstOrDefault(o => o.LoginName.ToUpper() == cboDoctorName.EditValue.ToString().ToUpper());
                        if (aDoctor != null)
                        {
                            cboWorkingDay.Focus();
                            cboWorkingDay.ShowPopup();
                        }
                        else
                        {
                            cboWorkingDay.Focus();
                            cboWorkingDay.ShowPopup();
                        }
                    }
                    else
                    {
                        cboWorkingDay.Focus();
                        cboWorkingDay.ShowPopup();
                    }
                }
                else if (e.KeyCode == Keys.Down)
                {
                    cboDoctorName.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }


        private void InitCheck(GridLookUpEdit cbo, GridCheckMarksSelection.SelectionChangedEventHandler eventSelect)
        {
            try
            {
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cbo.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(eventSelect);
                cbo.Properties.Tag = gridCheck;
                cbo.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cbo.Properties.View);

                    cboDoctorName.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SelectionGrid__Doctor(object sender, EventArgs e)
        {
            try
            {
                string DoctorUserName = "";
                var gridCheckMark = (sender as GridCheckMarksSelection);
                if (gridCheckMark != null && gridCheckMark.SelectedCount > 0)
                {
                    foreach (DoctorADO rv in gridCheckMark.Selection)
                    {
                        if (rv == null)
                            continue;
                        DoctorUserName += rv.UseName + ";";
                    }
                }
                cboDoctorName.Text = DoctorUserName;
                cboDoctorName.ToolTip = DoctorUserName;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDoctorName_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = "";
                string DoctorUserName = "";

                GridCheckMarksSelection gridCheckMark = cboDoctorName.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null && gridCheckMark.SelectedCount > 0)
                {
                    foreach (DoctorADO rv in gridCheckMark.Selection)
                    {
                        if (rv == null)
                            continue;
                        DoctorUserName += rv.UseName + "; ";
                        
                    }
                }
                e.DisplayText = DoctorUserName;
                cboDoctorName.ToolTip = DoctorUserName;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessSelectService(HIS_EXAM_SCHEDULE data, GridCheckMarksSelection gridCheckMark)
        {
            try
            {
                List<DoctorADO> ds = cboDoctorName.Properties.DataSource as List<DoctorADO>;
                string[] arrays = data.LOGINNAME.Split(';');
                if (arrays != null && arrays.Length > 0)
                {
                    List<DoctorADO> selects = new List<DoctorADO>();
                    List<string> ids = new List<string>();
                    foreach (var item in arrays)
                    {
                        var row = ds != null ? ds.FirstOrDefault(o => o.LoginName.ToUpper() == item.ToUpper()) : null;
                        if (row != null)
                        {
                            selects.Add(row);
                        }
                    }
                    gridCheckMark.SelectAll(selects);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDoctorName_EditValueChanged(object sender, EventArgs e)
        {
            //try
            //{
            //    if (cboDoctorName.EditValue != null)
            //    {
            //        cboDoctorName.Properties.Buttons[1].Visible = true;
            //    }
            //    else
            //    {
            //        cboDoctorName.Properties.Buttons[1].Visible = false;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Error(ex);                
            //}
        }



    }
}
