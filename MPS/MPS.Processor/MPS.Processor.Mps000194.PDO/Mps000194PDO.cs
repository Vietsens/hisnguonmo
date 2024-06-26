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
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000194.PDO
{
    public partial class Mps000194PDO : RDOBase
    {
        public HIS_BRANCH Branch { get; set; }
        public List<HIS_TREATMENT_TYPE> TreatmentType;
        public PatientTypeCFG PatientTypeCFG { get; set; }
        public V_HIS_PATIENT_TYPE_ALTER CurrentPatyAlter { get; set; }
        public List<HIS_SERE_SERV_EXT> SereServExts { get; set; }
        public List<HIS_MATERIAL_TYPE> MaterialTypes { get; set; }

        public Mps000194PDO(
            V_HIS_PATIENT_TYPE_ALTER _currentPatyAlter,
            V_HIS_PATIENT _patient,
            List<HIS_PATIENT_TYPE_ALTER> _patyAlters,
            List<V_HIS_DEPARTMENT_TRAN> _departmentTrans,
            List<V_HIS_TREATMENT_FEE> _treatmentFees,
            HeinServiceTypeCFG _heinServiceTypeCfg,
            PatientTypeCFG _patientTypeCFG,
            List<HIS_SERE_SERV> _sereServ,
            List<HIS_SERE_SERV_EXT> _sereServExts,
            List<V_HIS_EXP_MEST> _expMests,
            List<HIS_HEIN_SERVICE_TYPE> _heinServiceTypes,
            List<V_HIS_ROOM> _rooms,
            List<V_HIS_SERVICE> _services,
            V_HIS_TREATMENT _treatment,
            List<HIS_TREATMENT_TYPE> _treatmentType,
            HIS_BRANCH _branch,
            List<HIS_MATERIAL_TYPE> _materialTypes,
            BordereauSingleValue _bordereauSingleValue
            )
        {
            try
            {
                this.patient = _patient;
                this.patyAlters = _patyAlters;
                this.sereServs = _sereServ;
                this.treatment = _treatment;
                this.departmentTrans = _departmentTrans;
                this.treatmentFees = _treatmentFees;
                this.heinServiceTypeCfg = _heinServiceTypeCfg;
                this.expMests = _expMests;
                this.HeinServiceTypes = _heinServiceTypes;
                this.bordereauSingleValue = _bordereauSingleValue;
                this.Rooms = _rooms;
                this.Services = _services;
                this.Branch = _branch;
                this.TreatmentType = _treatmentType;
                this.PatientTypeCFG = _patientTypeCFG;
                this.CurrentPatyAlter = _currentPatyAlter;
                this.SereServExts = _sereServExts;
                this.MaterialTypes = _materialTypes;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
