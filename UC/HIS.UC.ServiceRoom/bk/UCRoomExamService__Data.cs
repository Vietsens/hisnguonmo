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
                    this.roomExamServiceInitADO = ado;
                    this.currentPatientTypeAlter = ado.CurrentPatientTypeAlter;
                    this.currentPatientTypes = ado.CurrentPatientTypes;                   
                    if (ado.HisExecuteRooms != null && ado.HisExecuteRooms.Count > 0)
                    {
                        this.roomExts = (from m in ado.HisExecuteRooms select new RoomExtADO(m)).OrderByDescending(o => o.NUM_ORDER).ToList();
                    }
                    else if (ado.LHisRoomCounters != null && ado.LHisRoomCounters.Count > 0)
                    {
                        this.roomExts = (from m in ado.LHisRoomCounters select new RoomExtADO(m)).OrderByDescending(o => o.NUM_ORDER).ToList();
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
                    this.changeRoomNotEmergency = ado.ChangeRoomNotEmergency;
                    this.isFocusCombo = ado.IsFocusCombo;
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
                this.cboExamService.Properties.DataSource = this.currentServiceRooms;
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
                    this.dicRoomService = this.hisServiceRooms
                    .Where(o => BranchDataWorker.HasServicePatyWithListPatientType(o.SERVICE_ID, this.dicPatientType.Select(t => t.Key).ToList()))
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
                ServiceReqDetailSDO detail = new ServiceReqDetailSDO();
                long? roomId = null;
                long? serviceId = null;
                if (this.cboRoom.EditValue != null)
                {
                    roomId = Convert.ToInt64(this.cboRoom.EditValue);
                }
                if (this.cboExamService.EditValue != null)
                {
                    serviceId = Convert.ToInt64(this.cboExamService.EditValue);
                }
                //if (roomId.HasValue && serviceId.HasValue && roomId.Value > 0 && serviceId.Value > 0)
                //nếu chỉ chọn phòng & không chọn dịch vụ thì vẫn thêm vào
                //-> Trường hợp này chỗ sử dụng uc này sẽ đưa ra cảnh báo với người dùng, nếu vẫn muốn tiếp tục thì chỉ tạo hồ sơ, ngược lại sẽ đưa ra cảnh báo lỗi
                if (roomId.HasValue && roomId.Value > 0)
                {
                    detail.Amount = 1;
                    detail.ServiceId = (serviceId.HasValue ? serviceId.Value : 0);
                    detail.RoomId = roomId.Value;
                    detail.PatientTypeId = this.currentPatientTypeAlter.PATIENT_TYPE_ID;
                    datas.Add(detail);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                datas = new List<ServiceReqDetailSDO>();
            }
            return datas;
        }

        public void SetValueByPatient(V_HIS_PATIENT patient)
        {
            try
            {
                this.serviceId = null;
                this.cboRoom.EditValue = null;

                if (patient != null && patient.RECENT_ROOM_ID.HasValue)
                {
                    if (!this.dicExecuteRoom.ContainsKey(patient.RECENT_ROOM_ID.Value))
                        return;
                    this.serviceId = patient.RECENT_SERVICE_ID;
                    this.cboRoom.EditValue = patient.RECENT_ROOM_ID.Value;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
