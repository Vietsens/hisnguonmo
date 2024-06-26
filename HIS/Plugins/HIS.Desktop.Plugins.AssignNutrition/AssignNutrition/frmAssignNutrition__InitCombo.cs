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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.AssignNutrition.ADO;
using HIS.Desktop.Plugins.AssignNutrition.Config;
using HIS.Desktop.Utilities.Extensions;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.AssignNutrition.AssignNutrition
{
    public partial class frmAssignNutrition : HIS.Desktop.Utility.FormBase
    {
        //Load người chỉ định
        private void InitComboUser()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "", 150, 1));
                columnInfos.Add(new ColumnInfo("USERNAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos, false, 400);
                ControlEditorLoader.Load(this.cboUser, BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>(), controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboPatientType(GridLookUpEdit cboPatientType, List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE> data)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PATIENT_TYPE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("PATIENT_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PATIENT_TYPE_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboPatientType, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboRepositoryPatientType(List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE> data)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PATIENT_TYPE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("PATIENT_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PATIENT_TYPE_NAME", "ID", columnInfos, false, 350);
                if (data != null)
                {
                    ControlEditorLoader.Load(this.repositoryItemGridLookUpEdit_PatientType, data, controlEditorADO);
                }
                else
                    ControlEditorLoader.Load(this.repositoryItemGridLookUpEdit_PatientType, this.currentPatientTypeWithPatientTypeAlter, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool ConvertDayOfWeek(DateTime dayOfWeekInstructionTimeDt, int dayInRoomTime)
        {
            bool result = false;
            try
            {
                int dayOfWeekInstructionTime = (int)dayOfWeekInstructionTimeDt.DayOfWeek;
                if (dayOfWeekInstructionTime == 0 && dayInRoomTime == 1)
                {
                    result = true;
                }
                else if (dayOfWeekInstructionTime == 1 && dayInRoomTime == 2)
                {
                    result = true;
                }
                else if (dayOfWeekInstructionTime == 2 && dayInRoomTime == 3)
                {
                    result = true;
                }
                else if (dayOfWeekInstructionTime == 3 && dayInRoomTime == 4)
                {
                    result = true;
                }
                else if (dayOfWeekInstructionTime == 4 && dayInRoomTime == 5)
                {
                    result = true;
                }
                else if (dayOfWeekInstructionTime == 5 && dayInRoomTime == 6)
                {
                    result = true;
                }
                else if (dayOfWeekInstructionTime == 6 && dayInRoomTime == 7)
                {
                    result = true;
                }
                else
                {
                    result = false;
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private List<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM> ProcessExecuteRoom()
        {
            List<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM> result = new List<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM>();
            CommonParam param = new CommonParam();
            long instructionDate = 0;
            List<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM> executeRoomAlls = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM>();
            try
            {
                // không cho phép chỉ định dịch vụ vào các phòng đang tạm ngừng chỉ định Feature #10457
                executeRoomAlls = executeRoomAlls.Where(o => o.IS_PAUSE_ENCLITIC == null || o.IS_PAUSE_ENCLITIC != 1).ToList();
                //+ "Phòng đó phải không giới hạn thời gian hoạt động (IS_RESTRICT_TIME trong HIS_ROOM null)"                HOẶC "Phòng đó có giới hạn thời gian hoạt động và thời gian chỉ định nằm trong danh sách thời gian hoạt động của phòng đấy(có trong bảng HIS_ROOM_TIME)"


                List<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM> roomWithRoomTimeFilter = new List<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM>();
                // phòng không giới hạn thời gian hoạt động
                List<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM> roomIsRestrictTimes = executeRoomAlls.Where(o => o.IS_RESTRICT_TIME == null).ToList();
                roomWithRoomTimeFilter.AddRange(roomIsRestrictTimes);

                //phòng có giới hạn thời gian hoạt động
                List<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM> roomIsNotRestrictTimes = executeRoomAlls.Where(o => o.IS_RESTRICT_TIME != null).ToList();
                DateTime dayOfWeekInstructionTimeDt = DateTime.Now;
                if (this.intructionTimeSelecteds != null && this.intructionTimeSelecteds.Count > 0)
                {
                    instructionDate = Convert.ToInt64((this.intructionTimeSelecteds.First().ToString()).Substring(8, 6)); //20180101212244
                    dayOfWeekInstructionTimeDt = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.intructionTimeSelecteds.First()) ?? DateTime.Now;
                }

                if (this.roomTimes != null && this.roomTimes.Count > 0)
                {
                    foreach (var executeRoom in roomIsNotRestrictTimes)
                    {
                        var roomTime = this.roomTimes.FirstOrDefault(o => o.ROOM_ID == executeRoom.ROOM_ID);
                        if (roomTime != null)
                        {
                            if (Convert.ToInt64(roomTime.FROM_TIME) <= instructionDate && instructionDate <= Convert.ToInt64(roomTime.TO_TIME) && ConvertDayOfWeek(dayOfWeekInstructionTimeDt, roomTime.DAY))
                            {
                                roomWithRoomTimeFilter.Add(executeRoom);
                            }
                        }
                    }
                }

                // + Nếu phòng đang người dùng đang làm việc có check "Giới hạn chỉ định phòng thực hiện" (IS_RESTRICT_EXECUTE_ROOM trong HIS_ROOM), thì lọc tiếp, chỉ lấy các phòng nằm trong danh sách các phòng xử lý mà phòng đang làm việc được phép chỉ định (lấy theo bảng HIS_EXRO_ROOM với IS_ALLOW_REQUEST = 1)

                if (roomWithRoomTimeFilter != null && roomWithRoomTimeFilter.Count > 0)
                {
                    var currentWorkingRoom = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.currentModule.RoomId);
                    if (currentWorkingRoom != null && currentWorkingRoom.IS_RESTRICT_EXECUTE_ROOM == 1)
                    {
                        //if (this.exroRooms != null && this.exroRooms.Count > 0)
                        //{
                        //    var listAllow = this.exroRooms.Where(o => o.ROOM_ID == currentWorkingRoom.ID && o.IS_ALLOW_REQUEST == 1).ToList();
                        //    roomWithRoomTimeFilter = roomWithRoomTimeFilter.Where(o => listAllow.Exists(e => e.EXECUTE_ROOM_ID == o.ID)).ToList();
                        //}
                        //else
                        //{
                        //    roomWithRoomTimeFilter = new List<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM>();
                        //}
                    }
                    result.AddRange(roomWithRoomTimeFilter);
                }
            }
            catch (Exception ex)
            {
                result = new List<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM>();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void InitComboExecuteRoom(DevExpress.XtraEditors.GridLookUpEdit excuteRoomCombo, List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_ROOM> datas)
        {
            try
            {
                //List<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM> executeRoomFilters = ProcessExecuteRoom();
                //data = (executeRoomFilters != null && executeRoomFilters.Count > 0) ? data.Where(p => executeRoomFilters.Select(o => o.ROOM_ID).Distinct().Contains(p.ROOM_ID)).ToList() : null;
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("ROOM_CODE", "", 50, 1));
                columnInfos.Add(new ColumnInfo("ROOM_NAME", "", 100, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("ROOM_NAME", "ROOM_ID", columnInfos, false, 150);
                ControlEditorLoader.Load(excuteRoomCombo, datas, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboRepositoryServiceRoom(List<V_HIS_SERVICE_ROOM> datas)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("ROOM_CODE", "", 50, 1));
                columnInfos.Add(new ColumnInfo("ROOM_NAME", "", 100, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("ROOM_NAME", "ROOM_ID", columnInfos, false, 150);
                ControlEditorLoader.Load(this.repositoryItemGridLookUpEdit__Room, datas, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //Load nhóm dịch vụ, chỉ load ra các nhóm dịch vụ do người dùng tạo hoặc is_public = 1
        private void InitComboServiceGroup()
        {
            try
            {
                string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                var serviceGroups = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_SERVICE_GROUP>();

                serviceGroups = (serviceGroups != null && serviceGroups.Count > 0) ?
                    serviceGroups.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE &&
                        ((o.IS_PUBLIC ?? -1) == 1 || o.CREATOR.ToLower() == loginName.ToLower()))
                        .ToList()
                : serviceGroups;

                // order tăng dần theo num_order
                if (serviceGroups != null && serviceGroups.Count > 0)
                {
                    var serviceGroup1s = serviceGroups.Where(u => u.NUM_ORDER != null).OrderBy(u => u.NUM_ORDER).ThenBy(o => o.SERVICE_GROUP_NAME);
                    var serviceGroup2s = serviceGroups.Where(u => u.NUM_ORDER == null).OrderBy(o => o.SERVICE_GROUP_NAME);
                    var serviceGroupConcats = serviceGroup1s.Concat(serviceGroup2s);
                    cboServiceGroup.Properties.DataSource = serviceGroupConcats.ToList();
                }
                else
                {
                    cboServiceGroup.Properties.DataSource = null;
                }

                cboServiceGroup.Properties.DisplayMember = "SERVICE_GROUP_NAME";
                cboServiceGroup.Properties.ValueMember = "ID";
                DevExpress.XtraGrid.Columns.GridColumn col2 = cboServiceGroup.Properties.View.Columns.AddField("SERVICE_GROUP_CODE");
                col2.VisibleIndex = 1;
                col2.Width = 100;
                col2.Caption = "Mã";
                DevExpress.XtraGrid.Columns.GridColumn col3 = cboServiceGroup.Properties.View.Columns.AddField("SERVICE_GROUP_NAME");
                col3.VisibleIndex = 2;
                col3.Width = 200;
                col3.Caption = "Tên";
                cboServiceGroup.Properties.PopupFormWidth = 320;
                cboServiceGroup.Properties.View.OptionsView.ShowColumnHeaders = false;
                cboServiceGroup.Properties.View.OptionsSelection.MultiSelect = true;
                this.selectedSeviceGroups = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboRationTime()
        {
            try
            {
                this.repositoryItemGridLookUp__BuaAn.DataSource = this.__curentRationTimes;

                this.repositoryItemGridLookUp__BuaAn.DisplayMember = "RATION_TIME_NAME";
                this.repositoryItemGridLookUp__BuaAn.ValueMember = "ID";
                DevExpress.XtraGrid.Columns.GridColumn col2 = this.repositoryItemGridLookUp__BuaAn.View.Columns.AddField("RATION_TIME_CODE");
                col2.VisibleIndex = 1;
                col2.Width = 50;
                col2.Caption = "Mã";
                DevExpress.XtraGrid.Columns.GridColumn col3 = this.repositoryItemGridLookUp__BuaAn.View.Columns.AddField("RATION_TIME_NAME");
                col3.VisibleIndex = 2;
                col3.Width = 100;
                col3.Caption = "Tên";
                this.repositoryItemGridLookUp__BuaAn.PopupFormWidth = 170;
                this.repositoryItemGridLookUp__BuaAn.View.OptionsView.ShowColumnHeaders = false;
                this.repositoryItemGridLookUp__BuaAn.View.OptionsSelection.MultiSelect = true;
                this.selectedRationTimes = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboRationTime(DevExpress.XtraEditors.GridLookUpEdit cbo, SSServiceADO data)
        {
            try
            {
                cbo.Properties.DataSource = this.__curentRationTimes;

                cbo.Properties.DisplayMember = "RATION_TIME_NAME";
                cbo.Properties.ValueMember = "ID";
                DevExpress.XtraGrid.Columns.GridColumn col2 = cbo.Properties.View.Columns.AddField("RATION_TIME_CODE");
                col2.VisibleIndex = 1;
                col2.Width = 50;
                col2.Caption = "Mã";
                DevExpress.XtraGrid.Columns.GridColumn col3 = cbo.Properties.View.Columns.AddField("RATION_TIME_NAME");
                col3.VisibleIndex = 2;
                col3.Width = 100;
                col3.Caption = "Tên";
                cbo.Properties.PopupFormWidth = 150;
                cbo.Properties.View.OptionsView.ShowColumnHeaders = false;
                cbo.Properties.View.OptionsSelection.MultiSelect = true;
                this.selectedRationTimes = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        async Task InitComboTracking(long treatmentId)
        {
            try
            {
                List<TrackingAdo> result = new List<TrackingAdo>();

                CommonParam param = new CommonParam();
                MOS.Filter.HisTrackingViewFilter trackingFilter = new HisTrackingViewFilter();
                trackingFilter.TREATMENT_ID = treatmentId;
                trackingFilter.DEPARTMENT_ID = this.requestRoom != null ? (long?)this.requestRoom.DEPARTMENT_ID : null;

                this.trackings = await new BackendAdapter(param).GetAsync<List<V_HIS_TRACKING>>("api/HisTracking/GetView", ApiConsumer.ApiConsumers.MosConsumer, trackingFilter, param);
                this.trackings = this.trackings != null && this.trackings.Count > 0 ? this.trackings.OrderByDescending(o => o.TRACKING_TIME).ToList() : trackings;
                foreach (var tracking in this.trackings)
                {
                    var trackingAdo = new TrackingAdo(tracking);
                    result.Add(trackingAdo);
                }

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("TrackingTimeStr", "", 150, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("TrackingTimeStr", "ID", columnInfos, false, 200);
                ControlEditorLoader.Load(this.cboTracking, result, controlEditorADO);

                LoadTrackingDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}
