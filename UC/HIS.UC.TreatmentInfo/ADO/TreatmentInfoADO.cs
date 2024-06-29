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

namespace HIS.UC.TreatmentInfo.ADO
{
    public class TreatmentInfoADO
    {
        //public string TreatmentCode { get; set; }
        public string PatientTypeName { get; set; }
        //public string PatientCode { get; set; }
        //public string VirPatientName { get; set; }
        //public string Dob { get; set; }
        //public string GenderName { get; set; }
        public string HeinCardNumber { get; set; }
        public string HeinCardFromTime { get; set; }
        public string HeinCardToTime { get; set; }

        public TreatmentInfoADO() { }

        public TreatmentInfoADO(string patientTypeName, string heinCardNumber, string heinCardFromTime, string heinCardToTime)
        {
            this.PatientTypeName = patientTypeName;
            this.HeinCardNumber = heinCardNumber;
            this.HeinCardFromTime = heinCardFromTime;
            this.HeinCardToTime = heinCardToTime;

        }
    }
}
