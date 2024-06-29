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
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.UC.ServiceRoom.ADO;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using HIS.Desktop.LocalStorage.BackendData;
using System.Threading;
using Inventec.Common.Adapter;
using Inventec.Core;
using HIS.Desktop.ApiConsumer;

namespace HIS.UC.ServiceRoom
{
    public partial class UCRoomExamService : UserControl
    {
        private void SetDataInit(RoomExamServiceInitADO ado)
        {
            try
            {
                if (ado != null)
                {
                    this.hisRooms = BackendDataWorker.Get<V_HIS_ROOM>();
                    this.roomExamServiceInitADO = ado;
                    this.currentPatientTypeAlter = ado.CurrentPatientTypeAlter;
                    this.currentPatientTypes = ado.CurrentPatientTypes;
                    if (ado.HisExecuteRooms != null && ado.HisExecuteRooms.Count > 0)
                    {
                        this.roomExts = (from m in ado.HisExecuteRooms select new RoomExtADO(m, this.hisRooms)).ToList();
                        this.roomExts = OrderRoom(this.roomExts);
                    }
                    else if (ado.LHisRoomCounters != null && ado.LHisRoomCounters.Count > 0)
                    {
                        this.roomExts = (from m in ado.LHisRoomCounters select new RoomExtADO(m, this.hisRooms)).ToList();
                        this.roomExts = OrderRoom(this.roomExts);
                    }
                    this.hisServiceRooms = ado.HisServiceRooms;

                    this.isInit = ado.IsInit;
                    this.layoutExamServiceName = ado.LciExamServiceName;
                    this.layoutRoomName = ado.LciRoomName;
                    this.sereServExam = ado.SereServExam;
                    this.dlgRemoveUC = ado.RemoveUC;
                    this.dlgFocusNextUserControl = ado.FocusOutUC;
                    this.ucName = ado.UcName;
                    this.currentCulture = ado.CurrentCulture;
                    this.registerPatientWithRightRouteBHYT = ado.RegisterPatientWithRightRouteBHYT;
                    this.changeServiceProcessPrimaryPatientType = ado.ChangeServiceProcessPrimaryPatientType;
                    this.changeDisablePrimaryPatientType = ado.ChangeDisablePrimaryPatientType;
                    this.changeRoomNotEmergency = ado.ChangeRoomNotEmergency;
                    this.isFocusCombo = ado.IsFocusCombo;
                    this.GetIntructionTime = ado.GetIntructionTime;
                    this.dicNumOrderBlock = new Dictionary<long, Desktop.ADO.ResultChooseNumOrderBlockADO>();
                    this.PatientSDO = ado.patientSDO;
                    this.PatientClassifyId = ado.PatientClassifyId;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void SetDataSourceCboExamService()
        {
            try
            {
                this.currentServiceRooms = null;
                var currentRoomExts = this.roomExts.Where(o => o.IsChecked).ToList();
                if (currentRoomExts != null && currentRoomExts.Count > 0)
                {
                    this.currentServiceRooms = new List<V_HIS_SERVICE_ROOM>();
                    var curRoomIds = currentRoomExts.Select(t => t.ROOM_ID).ToList();
                    //- Combobox "Yêu cầu", sửa để chỉ hiển thị ra các dịch vụ khám cho phép thực hiện ở TẤT CẢ CÁC PHÒNG ĐƯỢC CHỌN.
                    //this.currentServiceRooms = hisServiceRooms != null ? hisServiceRooms.Where(o => curRoomIds.Contains(o.ROOM_ID)).ToList() : null;
                    var gServices = hisServiceRooms != null ? hisServiceRooms.GroupBy(o => o.SERVICE_ID).ToList() : null;
                    if (gServices != null)
                    {
                        foreach (var gsItem in gServices)
                        {
                            var gCheckCount = (from m in currentRoomExts
                                               from n in gsItem.ToList()
                                               where m.ROOM_ID == n.ROOM_ID
                                               select m.ROOM_ID
                                        ).Distinct().Count();
                            if (gCheckCount == currentRoomExts.Count)
                            {
                                this.currentServiceRooms.Add(gsItem.First());
                            }
                        }
                    }
                }
                this.currentServiceRooms = this.currentServiceRooms.OrderBy(o => o.SERVICE_ID).ToList();
                this.cboExamService.Properties.DataSource = this.currentServiceRooms;
                this.cboExamService.EditValue = null;
                if (this.serviceId.HasValue &&
                    this.currentServiceRooms != null
                    && this.currentServiceRooms.Count > 0
                    && this.currentServiceRooms.Any(o => o.SERVICE_ID == this.serviceId.Value))
                {
                    this.cboExamService.EditValue = this.serviceId.Value;
                    this.serviceId = null;
                }
                else if (this.currentServiceRooms != null && this.currentServiceRooms.Count == 1)
                {
                    this.cboExamService.EditValue = this.currentServiceRooms.FirstOrDefault().SERVICE_ID;
                }
                else if (this.currentServiceRooms != null && this.currentServiceRooms.Count > 1)
                {
                    var curRoomIds = currentRoomExts.Select(t => t.ROOM_ID).ToList();
                    var checkDfRoom = this.hisRooms.Where(o => curRoomIds.Contains(o.ID)).ToList();
                    if (checkDfRoom != null && checkDfRoom.Count > 0)
                    {
                        var lstServiceRoom = this.currentServiceRooms.Where(o => checkDfRoom.Exists(p => p.DEFAULT_SERVICE_ID == o.SERVICE_ID)).ToList();

                        if (lstServiceRoom != null && lstServiceRoom.Count > 0)
                        {
                            this.cboExamService.EditValue = lstServiceRoom.First().SERVICE_ID;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void InitData()
        {
            try
            {
                this.dicPatientType = new Dictionary<long, HIS_PATIENT_TYPE>();
                this.dicExecuteRoom = new Dictionary<long, RoomExtADO>();
                this.dicRoomService = new Dictionary<long, List<V_HIS_SERVICE_ROOM>>();

                if (this.currentPatientTypes != null && this.currentPatientTypes.Count > 0)
                {
                    this.dicPatientType = this.currentPatientTypes.ToDictionary(o => o.ID, o => o);
                }

                if (this.roomExts != null && this.roomExts.Count > 0)
                {
                    this.dicExecuteRoom = this.roomExts.ToDictionary(o => o.ROOM_ID, o => o);
                }

                if (this.hisServiceRooms != null && this.hisServiceRooms.Count > 0)
                {
                    DayOfWeek wk = DateTime.Today.DayOfWeek;
                    int dayOfWeek = (int)wk + 1;
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dayOfWeek), dayOfWeek));
                    this.hisServiceRooms = this.hisServiceRooms.Where(o => BranchDataWorker.ServicePatyWithListPatientType(o.SERVICE_ID, this.dicPatientType.Select(t => t.Key).ToList()).Exists(k => (k.DAY_FROM == null || k.DAY_FROM <= dayOfWeek) && (k.DAY_TO == null || dayOfWeek <= k.DAY_TO))).ToList();

                    this.dicRoomService = this.hisServiceRooms
                    .GroupBy(o => o.ROOM_ID)
                    .ToDictionary(o => o.Key, o => o.ToList());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public List<ServiceReqDetailSDO> GetDetailSDO()
        {
            List<ServiceReqDetailSDO> datas = new List<ServiceReqDetailSDO>();
            try
            {
                var currentRoomExts = this.roomExts.Where(o => o.IsChecked).ToList();
                if (currentRoomExts != null && currentRoomExts.Count > 0)
                {
                    long? serviceId = null;
                    if (this.cboExamService.EditValue != null)
                    {
                        serviceId = Convert.ToInt64(this.cboExamService.EditValue);
                    }
                    foreach (var room in currentRoomExts)
                    {
                        ServiceReqDetailSDO detail = new ServiceReqDetailSDO();

                        //nếu chỉ chọn phòng & không chọn dịch vụ thì vẫn thêm vào
                        //-> Trường hợp này chỗ sử dụng uc này sẽ đưa ra cảnh báo với người dùng, nếu vẫn muốn tiếp tục thì chỉ tạo hồ sơ, ngược lại sẽ đưa ra cảnh báo lỗi
                        //if (roomId.HasValue && roomId.Value > 0)
                        //{
                        detail.Amount = 1;
                        detail.ServiceId = (serviceId.HasValue ? serviceId.Value : 0);
                        detail.RoomId = room.ROOM_ID;
                        detail.AssignedExecuteLoginName = room.RESPONSIBLE_LOGINNAME;
                        detail.AssignedExecuteUserName = room.RESPONSIBLE_USERNAME;

                        //nếu có thiết lập lịch khám bác sĩ thì sẽ có thông tin tại WORKING_LOGINNAME
                        if (String.IsNullOrWhiteSpace(room.RESPONSIBLE_LOGINNAME) && !String.IsNullOrWhiteSpace(room.WORKING_LOGINNAME))
                        {
                            detail.AssignedExecuteLoginName = GetFirstName(room.WORKING_LOGINNAME);
                            detail.AssignedExecuteUserName = GetFirstName(room.WORKING_USERNAME);
                        }

                        detail.PatientTypeId = this.currentPatientTypeAlter.PATIENT_TYPE_ID;
                        if (this.dicNumOrderBlock.ContainsKey(room.ROOM_ID) && this.dicNumOrderBlock[room.ROOM_ID].NumOrderBlock != null)
                        {
                            //detail.NumOrder = this.dicNumOrderBlock[room.ROOM_ID].NumOrderBlock.NUM_ORDER;
                            detail.NumOrderBlockId = this.dicNumOrderBlock[room.ROOM_ID].NumOrderBlock.NUM_ORDER_BLOCK_ID;
                            //detail.NumOrderIssueId = this.dicNumOrderBlock[room.ROOM_ID].NumOrderBlock.ROOM_TIME_ID;
                        }

                        datas.Add(detail);
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                datas = new List<ServiceReqDetailSDO>();
            }
            return datas;
        }

        private string GetFirstName(string data)
        {
            string result = null;
            try
            {
                if (!String.IsNullOrWhiteSpace(data))
                {
                    result = data.Split(';').FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
        public void SetPatientClassifyId(long? PatientClassifyId)
        {
            try
            {
                this.PatientClassifyId = PatientClassifyId;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void SetValueByPatient(V_HIS_PATIENT patient)
        {
            try
            {
                this.serviceId = null;
                txtExamServiceCode.Text = txtRoomCode.Text = "";
                cboExamService.EditValue = null;
                this.roomExts.ForEach(o => o.IsChecked = false);
                if (patient != null && patient.RECENT_ROOM_ID.HasValue)
                {
                    if (!this.dicExecuteRoom.ContainsKey(patient.RECENT_ROOM_ID.Value))
                        return;
                    this.serviceId = patient.RECENT_SERVICE_ID;
                    //this.cboRoom.EditValue = patient.RECENT_ROOM_ID.Value;
                    var room = this.roomExts != null ? this.roomExts.Where(o => o.ROOM_ID == patient.RECENT_ROOM_ID.Value).FirstOrDefault() : null;
                    if (room != null)
                    {
                        room.IsChecked = true;
                        isShowContainerMediMaty = false;
                        isShowContainerMediMatyForChoose = true;
                        this.SetDataSourceCboExamService();
                    }
                }
                ProcessDisplayRoomWithData();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
