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
using HIS.UC.ServiceRoomInfo.ADO;
using HIS.UC.ServiceRoomInfo.Delegate;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.ServiceRoomInfo.ADO
{
    public class ServiceRoomInfoInitADO
    {
        public ServiceRoomInfoInitADO()
        {

        }

        public ServiceRoomInfoInitADO(CultureInfo Culture
            , List<MOS.EFMODEL.DataModels.HIS_EXAM_SERVICE_TYPE> HisExamServiceTypes
            , List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_PATY> HisVServicePatyInBranchs
            , List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_ROOM> VHisServiceRooms
            )
        {
            this.Culture = Culture;
            this.HisExamServiceTypes = HisExamServiceTypes;
            this.HisVServicePatyInBranchs = HisVServicePatyInBranchs;
            this.VHisServiceRooms = VHisServiceRooms;
        }
        public CultureInfo Culture { get; set; }
        public List<MOS.EFMODEL.DataModels.HIS_EXAM_SERVICE_TYPE> HisExamServiceTypes { get; set; }
        public List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_PATY> HisVServicePatyInBranchs { get; set; }
        public List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_ROOM> VHisServiceRooms { get; set; }
        public long HIS_ROOM_TYPE_ID__DV { get; set; }      
        public MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER currentHisPatientTypeAlter { get; set; }
        public List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE> currentPatientTypeWithPatientTypeAlter { get; set; }

        public RemoveServiceRoomInfo DeleteServiceRoomInfo { get; set; }
        public FoucusMoveOutServiceRoomInfo FoucusMoveOutServiceRoomInfo { get; set; }
        public ServiceRoomInfoGenerateADO ServiceRoomInfoGenerateADO { get; set; }
    }
}
