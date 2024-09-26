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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.Hospitalize.ADO
{
    public delegate string DelegateGetIcdSubCode();
    public class HospitalizeInitADO
    {
        public long? DepartmentId { get; set; }
        public long? TreatmentId { get; set; }
        public long? RoomId { get; set; }
        public Desktop.Plugins.Library.CheckIcd.DelegateRefeshIcd dlgRefeshIcd { get; set; }
        public DelegateGetIcdSubCode dlgSendIcd { get; set; }
        public string InCode { get; set; }

        public bool isAutoCheckChkHospitalizeExam { get; set; }
        public long? FinishTime { get; set; }
        public long? StartTime { get; set; }
        public long? InTime { get; set; }
        public long? OutTime { get; set; } // TG ket thuc dieu tri
        public CheckEdit_CheckChange CheckEditSign_CheckChange { get; set; }
        public CheckEdit_CheckChange CheckEditPrintDocumentSign_CheckChange { get; set; }
        public string ModuleLink { get; set; }
        public HIS_TREATMENT Treatment {get;set;}
        public string IcdCode { get; set; }
        public string IcdName { get; set; }
        public string IcdSubCode { get; set; }
        public string IcdText { get; set; }
        public string TraditionalIcdCode { get; set; }
        public string TraditionalIcdName { get; set; }
        public string TraditionalIcdSubCode { get; set; }
        public string TraditionalIcdText { get; set; }

        public bool isEmergency  { get; set; }

        public string RelativeName { get; set; }
        public string RelativePhone { get; set; }
        public string RelativeAddress { get; set; }
        public long? CareerId { get; set; }
        public string InHospitalizationReasonCode { get; set; }
        public string InHospitalizationReasonName { get; set; }
        public string Note { get; set; }
    }
}
