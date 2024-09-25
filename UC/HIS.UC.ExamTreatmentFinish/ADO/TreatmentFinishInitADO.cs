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

namespace HIS.UC.ExamTreatmentFinish.ADO
{
    public delegate string DelegateGetIcdSubCode();
    public class TreatmentFinishInitADO
    {
        public HIS_TREATMENT Treatment { get; set; }
        public string CmndNumber { get; set; }
        public string CmndPlace { get; set; }
        public long? CmndDate { get; set; }
        public string BranchName { get; set; }
        public string DocumentType { get; set; }
        public DelegateGetIcdSubCode dlgGetIcdSubCode { get; set; }
        public List<HIS_TREATMENT_END_TYPE_EXT> TreatmentEndTypeExts { get; set; }
        public List<HIS_PATIENT_PROGRAM> PatientPrograms { get; set; }
        public List<V_HIS_DATA_STORE> DataStores { get; set; }
        public HIS_MEDI_RECORD MediRecord { get; set; }
        public Inventec.Desktop.Common.Modules.Module moduleData { get; set; }

        public HIS_SEVERE_ILLNESS_INFO SevereIllNessInfo { get; set; }
        public List<HIS_EVENTS_CAUSES_DEATH> ListEventsCausesDeath { get; set; }

        public string IcdCode { get; set; }
        public string IcdName { get; set; }
        public string IcdSubCode { get; set; }
        public string IcdText { get; set; }
        public long? CareerId { get; set; }

        public string TraditionalIcdCode { get; set; }
        public string TraditionalIcdName { get; set; }
        public string TraditionalIcdSubCode { get; set; }
        public string TraditionalIcdText { get; set; }

        public bool IsBlockNumOrder { get; set; }
        public bool IsAutoSetIcdWhenFinishInOtherExam { get; set; }

        public string Advise { get; set; }
        public string Conclusion { get; set; }
        public string Note { get; set; }
    }
}
