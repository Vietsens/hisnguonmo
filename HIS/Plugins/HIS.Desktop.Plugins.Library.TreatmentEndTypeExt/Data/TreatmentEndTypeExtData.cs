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

namespace HIS.Desktop.Plugins.Library.TreatmentEndTypeExt.Data
{
    public class TreatmentEndTypeExtData
    {
        public long? TreatmentEndTypeExtId { get; set; }
        public decimal? SickLeaveDay { get; set; }
        public long? SickLeaveFrom { get; set; }
        public long? SickLeaveTo { get; set; }
        public long? WorkPlaceId { get; set; }
        public string Loginname { get; set; }
        public string Username { get; set; }
        public string PatientWorkPlace { get; set; }
        public string MotherName { get; set; }
        public string FatherName { get; set; }
        public string PatientRelativeName { get; set; }
        public string PatientRelativeType { get; set; }
        public string SickHeinCardNumber { get; set; }
        public List<BabyADO> Babes { get; set; }
        public long? DocumentBookId { get; set; }

        public long? EndTime { get; set; }

        //SurgeryAppointment
        public long? SurgeryAppointmentTime { get; set; }
        public string Advise { get; set; }
        public string AppointmentSurgery { get; set; }
        public string SocialInsuranceNumber { get; set; }
        public string EndTypeExtNote { get; set; }
        public string ExtraEndCode { get; set; }
        public bool? IsPregnancyTermination
        {
            get;
            set;
        }

        public long? GestationalAge
        {
            get;
            set;
        }

        public string PregnancyTerminationReason
        {
            get;
            set;
        }
        public string TreatmentMethod
        {
            get;
            set;
        }
        public long? PregnancyTerminationTime
        {
            get;
            set;
        }

    }
}
