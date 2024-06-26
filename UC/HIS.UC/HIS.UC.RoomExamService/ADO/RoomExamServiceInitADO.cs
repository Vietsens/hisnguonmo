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
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.RoomExamService.ADO
{
    public enum TemplateDesign
    {
        T01,
        T11,
        T20
    }

    public class RoomExamServiceInitADO
    {
        public V_HIS_PATIENT_TYPE_ALTER CurrentPatientTypeAlter { get; set; }
        public List<HIS_PATIENT_TYPE> CurrentPatientTypes { get; set; }
        public List<V_HIS_SERVICE_ROOM> HisServiceRooms { get; set; }
        public List<V_HIS_EXECUTE_ROOM> HisExecuteRooms { get; set; }

        public string LciRoomName { get; set; }
        public string LciExamServiceName { get; set; }
        public string UcName { get; set; }
        public bool IsInit { get; set; }
        public string UserControlItemName { get; set; }
        public V_HIS_SERE_SERV SereServExam { get; set; }
        public CultureInfo CurrentCulture { get; set; }

        public RemoveRoomExamService RemoveUC { get; set; }
        public FocusMoveOutRoomExamService FocusOutUC { get; set; }
        public TemplateDesign TemplateDesign { get; set; }
        public Action RegisterPatientWithRightRouteBHYT { get; set; }
        public Action ChangeRoomNotEmergency { get; set; }
        public Size TextSize { get; set; }
        public RoomExamServiceInitADO() { }

        public RoomExamServiceInitADO(List<V_HIS_EXECUTE_ROOM> executeRooms, List<V_HIS_SERVICE_ROOM> serviceRooms)
        {
            this.HisExecuteRooms = executeRooms;
            this.HisServiceRooms = serviceRooms;
        }
    }
}
