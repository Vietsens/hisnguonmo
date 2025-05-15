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
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HIS.Desktop.DelegateRegister;

namespace HIS.UC.ServiceRoom.ADO
{
    public enum TemplateDesign
    {
        T11,
        T20
    }

    public class RoomExamServiceInitADO
    {
        public V_HIS_PATIENT_TYPE_ALTER CurrentPatientTypeAlter { get; set; }
        public List<HIS_PATIENT_TYPE> CurrentPatientTypes { get; set; }
        public List<V_HIS_SERVICE_ROOM> HisServiceRooms { get; set; }
        public List<V_HIS_EXECUTE_ROOM_1> HisExecuteRooms { get; set; }
        public List<L_HIS_ROOM_COUNTER_2> LHisRoomCounters { get; set; }

        public string LciRoomName { get; set; }
        public string LciExamServiceName { get; set; }
        public string UcName { get; set; }
        public bool IsInit { get; set; }
        public bool IsFocusCombo { get; set; }
        public string UserControlItemName { get; set; }
        public V_HIS_SERE_SERV SereServExam { get; set; }
        public CultureInfo CurrentCulture { get; set; }

        public RemoveRoomExamService RemoveUC { get; set; }
        public DelegateFocusNextUserControl FocusOutUC;
        public TemplateDesign TemplateDesign { get; set; }
        public Action RegisterPatientWithRightRouteBHYT { get; set; }
        public Action ChangeRoomNotEmergency { get; set; }
        public Action<long> ChangeServiceProcessPrimaryPatientType { get; set; }
        public DelegateGetIntructionTime GetIntructionTime { get; set; }
        public MOS.SDO.HisPatientSDO patientSDO { get; set; }
        public long? PatientClassifyId { get; set; }
        public Action<bool> ChangeDisablePrimaryPatientType { get; set; }
        public RoomExamServiceInitADO() { }

        public RoomExamServiceInitADO(List<V_HIS_EXECUTE_ROOM_1> executeRooms, List<V_HIS_SERVICE_ROOM> serviceRooms)
        {
            this.HisExecuteRooms = executeRooms;
            this.HisServiceRooms = serviceRooms;
        }

        public RoomExamServiceInitADO(List<L_HIS_ROOM_COUNTER_2> executeRooms, List<V_HIS_SERVICE_ROOM> serviceRooms)
        {
            this.LHisRoomCounters = executeRooms;
            this.HisServiceRooms = serviceRooms;
        }
    }
}
