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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HIS.UC.UCRelativeInfo.ADO;
using Inventec.Common.QrCodeBHYT;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;

namespace HIS.UC.UCPatientRaw.ADO
{
    public class DataResultADO
    {
        public enum SearchType
        {
            MABN = 1,
            MAHK = 2,
            MACT = 3,
            SOTHE = 4,
            MAMS = 5,
        };

        public DataResultADO() { }

        public HisPatientSDO HisPatientSDO { get; set; }
        public HeinCardData HeinCardData { get; set; }
        public HisCardSDO HisCardSDO { get; set; }
        public V_HIS_PATIENT_PROGRAM V_HIS_PATIENT_PROGRAM { get; set; }
        public List<V_HIS_PATIENT_PROGRAM> LIST_PATIENT_PROGRAM { get; set; }
        public UCRelativeADO UCRelativeADO { get; set; }
        public bool OldPatient { get; set; }
        public int SearchTypePatient { get; set;}
        public string AppointmentCode { get; set; }
        public long TreatmnetIdByAppointmentCode { get; set; }
        public long? TreatmentTypeId { get; set; }
        public bool IsReadQr { get; set; }       
    }
}
