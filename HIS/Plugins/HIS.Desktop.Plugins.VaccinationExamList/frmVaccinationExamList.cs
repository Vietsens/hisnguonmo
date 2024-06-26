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
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
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
using HIS.Desktop.Common;
using Inventec.Common.Controls.EditorLoader;
using HIS.Desktop.Utilities.Extensions;
using HIS.Desktop.LocalStorage.BackendData;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Controls;
using MOS.SDO;

namespace HIS.Desktop.Plugins.VaccinationExamList
{
    public partial class frmVaccinationExamList : FormBase
    {
        List<HIS_SERVICE_REQ_STT> serviceReqSttSelecteds;
        Inventec.Desktop.Common.Modules.Module moduleData;
        MOS.SDO.WorkPlaceSDO currentWorkPlace;
        V_HIS_ROOM currentRoom;

        public frmVaccinationExamList(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            InitializeComponent();
            this.moduleData = moduleData;
        }

        private void frmVaccinationExamList_Load(object sender, EventArgs e)
        {
            try
            {
                MeShow();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void InitComboOptionTimeFilter()
        {
            try
            {
                List<FilterTypeADO> listFilterType = new List<FilterTypeADO>();

                listFilterType.Add(new FilterTypeADO(0, "Thời gian yêu cầu"));
                listFilterType.Add(new FilterTypeADO(1, "Thời gian hẹn tiêm"));
                cboOptionTime.Properties.DataSource = listFilterType;
                cboOptionTime.Properties.DisplayMember = "FilterTypeName";
                cboOptionTime.Properties.ValueMember = "ID";
                cboOptionTime.Properties.ForceInitialize();
                cboOptionTime.Properties.Columns.Clear();
                cboOptionTime.Properties.Columns.Add(new DevExpress.XtraEditors.Controls.LookUpColumnInfo("FilterTypeName", "", 200));
                cboOptionTime.Properties.ShowHeader = false;
                cboOptionTime.Properties.ImmediatePopup = true;
                cboOptionTime.Properties.DropDownRows = 2;
                cboOptionTime.Properties.PopupWidth = 220;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void LoadDataCboFilterType()
        {
            try
            {
                List<FilterTypeADO> listFilterType = new List<FilterTypeADO>();

                listFilterType.Add(new FilterTypeADO(0, "Tôi tạo"));
                listFilterType.Add(new FilterTypeADO(1, "Phòng chỉ định"));
                listFilterType.Add(new FilterTypeADO(2, "Khoa chỉ định"));
                listFilterType.Add(new FilterTypeADO(3, "Khoa thực hiện"));
                listFilterType.Add(new FilterTypeADO(4, "Tất cả"));

                cboFilter.Properties.DataSource = listFilterType;
                cboFilter.Properties.DisplayMember = "FilterTypeName";
                cboFilter.Properties.ValueMember = "ID";
                cboFilter.Properties.ForceInitialize();
                cboFilter.Properties.Columns.Clear();
                cboFilter.Properties.Columns.Add(new DevExpress.XtraEditors.Controls.LookUpColumnInfo("FilterTypeName", "", 200));
                cboFilter.Properties.ShowHeader = false;
                cboFilter.Properties.ImmediatePopup = true;
                cboFilter.Properties.DropDownRows = 5;
                cboFilter.Properties.PopupWidth = 220;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadComboExcuteRoom()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisExecuteRoomFilter filter = new HisExecuteRoomFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                var data = new BackendAdapter(param).Get<List<HIS_EXECUTE_ROOM>>("api/HisExecuteRoom/Get", HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, filter, null).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("EXECUTE_ROOM_CODE", "", 50, 1));
                columnInfos.Add(new ColumnInfo("EXECUTE_ROOM_NAME", "", 200, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("EXECUTE_ROOM_NAME", "ROOM_ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboExecuteRoom, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultValueControl()
        {
            try
            {
                txtVaccinationExamCode.Text = "";
                cboFilter.EditValue = (long)0;
                cboOptionTime.EditValue = (long)0;
                txtKeyword.Text = "";
                cboReqTime.DateTime = DateTime.Now;

                //load mặc định tôi tạo, phòng chỉ định
                currentWorkPlace = WorkPlace.WorkPlaceSDO.FirstOrDefault(o => o.RoomId == currentModuleBase.RoomId && o.RoomTypeId == currentModuleBase.RoomTypeId);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void MeShow()
        {
            try
            {
                this.currentRoom = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.currentModuleBase.RoomId);
                InitComboOptionTimeFilter();
                SetDefaultValueControl();
                LoadDataCboFilterType();
                LoadDataCboStatus();
                LoadComboExcuteRoom();
                FillDataFormList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataCboStatus()
        {
            try
            {
                List<StatusADO> listFilterType = new List<StatusADO>();

                listFilterType.Add(new StatusADO(0, "Chưa kết luận"));
                listFilterType.Add(new StatusADO(1, "Đã điều kiện"));
                listFilterType.Add(new StatusADO(2, "Chưa đủ điều kiện"));

                cboServiceReqStt.Properties.DataSource = listFilterType;
                cboServiceReqStt.Properties.DisplayMember = "StatusName";
                cboServiceReqStt.Properties.ValueMember = "ID";
                cboServiceReqStt.Properties.ForceInitialize();
                cboServiceReqStt.Properties.Columns.Clear();
                cboServiceReqStt.Properties.Columns.Add(new DevExpress.XtraEditors.Controls.LookUpColumnInfo("StatusName", "", 200));
                cboServiceReqStt.Properties.ShowHeader = false;
                cboServiceReqStt.Properties.ImmediatePopup = true;
                cboServiceReqStt.Properties.DropDownRows = 5;
                cboServiceReqStt.Properties.PopupWidth = 220;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataFormList()
        {
            try
            {
                HisVaccinationExamViewFilter filter = new HisVaccinationExamViewFilter();
                SetFilter(ref filter);
                Inventec.Common.Logging.LogSystem.Error(Inventec.Common.Logging.LogUtil.TraceData("filter________", filter));
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                var lstVaccinationExam = new BackendAdapter(new CommonParam()).Get<List<V_HIS_VACCINATION_EXAM>>("api/HisVaccinationExam/GetView", ApiConsumers.MosConsumer, filter, null);
                grdVaccinationExam.DataSource = null;
                grdVaccinationExam.DataSource = lstVaccinationExam;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetFilter(ref HisVaccinationExamViewFilter filter)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtVaccinationExamCode.Text))
                {
                    var codeVaccinationExam = txtVaccinationExamCode.Text;
                    if (codeVaccinationExam.Length < 12 && checkDigit(codeVaccinationExam))
                    {
                        codeVaccinationExam = string.Format("{0:000000000000}", Convert.ToInt64(codeVaccinationExam));
                        txtVaccinationExamCode.Text = codeVaccinationExam;
                    }
                    filter.VACCINATION_EXAM_CODE__EXACT = codeVaccinationExam;
                }
                else
                {
                    if (cboOptionTime.EditValue != null && (long)cboOptionTime.EditValue == 0)
                    {
                        if (cboReqTime.EditValue != null && cboReqTime.DateTime != DateTime.MinValue)
                        {
                            filter.REQUEST_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(
                                Convert.ToDateTime(cboReqTime.EditValue).ToString("yyyyMMdd") + "000000");
                            filter.REQUEST_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(
                                Convert.ToDateTime(cboReqTime.EditValue).ToString("yyyyMMdd") + "235959");
                        }
                    }
                    else
                    {
                        if (cboReqTime.EditValue != null && cboReqTime.DateTime != DateTime.MinValue)
                        {
                            filter.APPOINTMENT_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(
                                Convert.ToDateTime(cboReqTime.EditValue).ToString("yyyyMMdd") + "000000");
                        }
                        if (dtAppointmentTimeTo.EditValue != null && dtAppointmentTimeTo.DateTime != DateTime.MinValue)
                        {
                            filter.APPOINTMENT_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(
                                Convert.ToDateTime(dtAppointmentTimeTo.EditValue).ToString("yyyyMMdd") + "235959");
                        }
                    }
                    if (cboExecuteRoom.EditValue != null && cboExecuteRoom.EditValue.ToString() != "")
                    {
                        filter.EXECUTE_ROOM_ID = (long)cboExecuteRoom.EditValue;
                    }
                    if (cboServiceReqStt.EditValue != null)
                    {
                        filter.CONCLUDE = Convert.ToInt64(cboServiceReqStt.EditValue);
                    }
                    var patientCode = txtPatientCode.Text;
                    if (!string.IsNullOrEmpty(patientCode))
                    {
                        if (patientCode.Length < 10 && checkDigit(patientCode))
                        {
                            patientCode = string.Format("{0:0000000000}", Convert.ToInt64(patientCode));
                            txtPatientCode.Text = patientCode;
                        }
                        filter.TDL_PATIENT_CODE__EXACT = patientCode;
                    }
                    if (!string.IsNullOrEmpty(txtKeyword.Text))
                    {
                        filter.KEY_WORD = txtKeyword.Text;
                    }

                    int value = Convert.ToInt32(cboFilter.EditValue ?? "0");
                    if (value == 0)//tôi tạo
                    {
                        filter.CREATOR = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                    }
                    else if (value == 1) // phòng làm việc
                    {
                        if (currentModuleBase != null && currentModuleBase.RoomId > 0)
                            filter.REQUEST_ROOM_ID = currentModuleBase.RoomId;
                    }
                    else if (value == 2) //Khoa chỉ định
                    {
                        filter.REQUEST_DEPARTMENT_ID = currentWorkPlace.DepartmentId;
                    }
                    else if (value == 3) // Khoa thực hiện
                    {
                        filter.EXECUTE_DEPARTMENT_ID = currentWorkPlace.DepartmentId;
                    }

                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool checkDigit(string s)
        {
            bool result = false;
            try
            {
                for (int i = 0; i < s.Length; i++)
                {
                    if (char.IsDigit(s[i]) == true) result = true;
                    else result = false;
                }
                return result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return result;
            }
        }

        private void grvVaccinationExam_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    V_HIS_VACCINATION_EXAM pData = (V_HIS_VACCINATION_EXAM)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }

                    else if (e.Column.FieldName == "REQ_TIME_STR")
                    {
                        var time = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(pData.REQUEST_TIME).ToString();
                        e.Value = time;
                    }
                    else if (e.Column.FieldName == "APPOINTMENT_TIME_STR")
                    {
                        var time = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(pData.APPOINTMENT_TIME ?? 0).ToString();
                        e.Value = time;
                    }
                    else if (e.Column.FieldName == "REQUEST_LOGINNAME_STR")
                    {
                        e.Value = pData.REQUEST_LOGINNAME + "-" + pData.REQUEST_USERNAME;
                    }
                    else if (e.Column.FieldName == "EXECUTE_LOGINNAME_STR")
                    {
                        e.Value = pData.EXECUTE_LOGINNAME + "-" + pData.EXECUTE_USERNAME;
                    }
                    else if (e.Column.FieldName == "TDL_PATIENT_MOBILE_STR")
                    {
                        if (!String.IsNullOrEmpty(pData.TDL_PATIENT_MOBILE))
                            e.Value = pData.TDL_PATIENT_MOBILE;
                        else
                            e.Value = pData.TDL_PATIENT_PHONE;
                    }
                    else if (e.Column.FieldName == "STATUS_DISPLAY")
                    {
                        try
                        {
                            if (pData.CONCLUDE == null)
                            {
                                e.Value = imageListIcon.Images[0];
                            }
                            else if (pData.CONCLUDE == 1)
                            {
                                e.Value = imageListIcon.Images[1];
                            }
                            else if (pData.CONCLUDE == 2)
                            {
                                e.Value = imageListIcon.Images[2];
                            }
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot icon trang thai yeu cau dich vu IMG", ex);
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButtonEdit_Click(object sender, EventArgs e)
        {
            try
            {
                var rowData = (V_HIS_VACCINATION_EXAM)grvVaccinationExam.GetFocusedRow();
                List<object> listArgs = new List<object>();
                listArgs.Add(rowData.ID);
                listArgs.Add((RefeshReference)FillDataFormList);
                HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.UpdateVaccinationExam", this.moduleData.RoomId, this.moduleData.RoomTypeId, listArgs);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void grvVaccinationExam_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                    V_HIS_VACCINATION_EXAM pData = (V_HIS_VACCINATION_EXAM)grvVaccinationExam.GetRow(e.RowHandle);
                    if (e.Column.FieldName == "EDIT") // sửa
                    {
                        if ((loginName == pData.REQUEST_LOGINNAME || HIS.Desktop.IsAdmin.CheckLoginAdmin.IsAdmin(loginName) || pData.REQUEST_DEPARTMENT_ID == this.currentRoom.DEPARTMENT_ID) && pData.CONCLUDE == null)
                        {
                            e.RepositoryItem = repositoryItemButtonEdit;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemButtonEditDis;
                        }
                    }
                    else if (e.Column.FieldName == "DELETE") // xoa
                    {
                        if ((loginName == pData.REQUEST_LOGINNAME || HIS.Desktop.IsAdmin.CheckLoginAdmin.IsAdmin(loginName) || pData.REQUEST_DEPARTMENT_ID == this.currentRoom.DEPARTMENT_ID) && pData.CONCLUDE == null)
                        {
                            e.RepositoryItem = repositoryItemButtonDelete;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemButtonDeleteDis;
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnFind_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnFind_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show(Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonHuyDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    var rowData = (V_HIS_VACCINATION_EXAM)grvVaccinationExam.GetFocusedRow();
                    if (rowData != null)
                    {
                        HisVaccinationExamDeleteSDO sdo = new HisVaccinationExamDeleteSDO();
                        sdo.VaccinationExamId = rowData.ID;
                        sdo.RequestRoomId = currentWorkPlace.RoomId;
                        bool success = false;
                        CommonParam param = new CommonParam();
                        success = new BackendAdapter(param).Post<bool>("api/HisVaccinationExam/Delete", ApiConsumers.MosConsumer, sdo, param);
                        if (success != null)
                        {
                            FillDataFormList();
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

        private void btnFind_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataFormList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExecuteRoom_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboExecuteRoom.EditValue != null)
                    {
                        cboExecuteRoom.Properties.Buttons[1].Visible = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExecuteRoom_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboExecuteRoom.EditValue = null;
                    cboExecuteRoom.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtVaccinationExamCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrEmpty(txtVaccinationExamCode.Text))
                    {
                        FillDataFormList();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboOptionTime_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if ((long)cboOptionTime.EditValue == (long)0)
                {
                    lciAppointmentTimeTo.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }
                else
                {
                    lciAppointmentTimeTo.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    dtAppointmentTimeTo.DateTime = DateTime.Now;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
