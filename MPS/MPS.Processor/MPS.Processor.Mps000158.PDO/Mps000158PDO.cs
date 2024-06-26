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
using MPS.Processor.Mps000158.PDO.Config;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000158.PDO
{
    public partial class Mps000158PDO : RDOBase
    {
        public SingleKeyValue SingleKeyValue { get; set; }
        public HeinServiceTypeCFG HeinServiceTypeCFG { get; set; }
        public PatientTypeCFG PatientTypeCFG { get; set; }
        public List<HIS_HEIN_SERVICE_TYPE> HeinServiceTypes { get; set; }
        public List<V_HIS_SERVICE> Services { get; set; }
        public List<V_HIS_ROOM> Rooms { get; set; }
        public HIS_BRANCH Branch { get; set; }
        public List<HIS_MATERIAL_TYPE> materialTypes { get; set; }
        public List<HIS_TREATMENT_TYPE> TreatmentType;
        public V_HIS_PATIENT_TYPE_ALTER CurrentPatyAlter { get; set; }
        public List<HIS_SERE_SERV_EXT> SereServExts { get; set; }

        public Mps000158PDO(
            V_HIS_PATIENT_TYPE_ALTER _currentPatyAlter,
            List<HIS_PATIENT_TYPE_ALTER> _patyAlters,
            List<V_HIS_DEPARTMENT_TRAN> _departmentTrans,
            List<V_HIS_TREATMENT_FEE> _treatmentFees,
            HeinServiceTypeCFG _heinServiceTypeCfg,
            PatientTypeCFG _patientTypeCfg,
            List<HIS_SERE_SERV> _sereServ,
            List<HIS_SERE_SERV_EXT> _sereServExts,
            V_HIS_TREATMENT _treatment,
            List<HIS_HEIN_SERVICE_TYPE> _heinServiceTypes,
            List<V_HIS_ROOM> _rooms,
            List<V_HIS_SERVICE> _services,
            List<HIS_TREATMENT_TYPE> _treatmentType,
            HIS_BRANCH _branch,
            List<HIS_MATERIAL_TYPE> _materialTypes,
            SingleKeyValue _singleKeyValue
            )
        {
            try
            {
                this.PatyAlters = _patyAlters;
                this.SereServs = _sereServ;
                this.Treatment = _treatment;
                this.DepartmentTrans = _departmentTrans;
                this.TreatmentFees = _treatmentFees;
                this.HeinServiceTypeCFG = _heinServiceTypeCfg;
                this.SingleKeyValue = _singleKeyValue;
                this.HeinServiceTypes = _heinServiceTypes;
                this.Services = _services;
                this.Rooms = _rooms;
                this.PatientTypeCFG = _patientTypeCfg;
                this.Branch = _branch;
                this.TreatmentType = _treatmentType;
                this.CurrentPatyAlter = _currentPatyAlter;
                this.SereServExts = _sereServExts;
                this.materialTypes = _materialTypes;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
