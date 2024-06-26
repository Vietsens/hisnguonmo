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
using MPS.Processor.Mps000128.PDO.Config;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000128.PDO
{
    public partial class Mps000128PDO : RDOBase
    {
        public SingleKeyValue SingleKeyValue { get; set; }
        public HeinServiceTypeCFG HeinServiceTypeCFG { get; set; }
        public List<HIS_HEIN_SERVICE_TYPE> HeinServiceTypes { get; set; }
        public List<V_HIS_SERVICE> Services { get; set; }
        public List<V_HIS_ROOM> Rooms { get; set; }
        public List<HIS_DEPARTMENT> Department { get; set; }
        public long? currentDepartmentId { get; set; }

        public Mps000128PDO(
            HIS_PATIENT_TYPE_ALTER _patyAlter,
            List<V_HIS_DEPARTMENT_TRAN> _departmentTrans,
            List<V_HIS_TREATMENT_FEE> _treatmentFees,
            HeinServiceTypeCFG _heinServiceTypeCFG,
            HIS_SERE_SERV _sereServKTC,
            List<HIS_SERE_SERV> _sereServs,
            V_HIS_TREATMENT _treatment,
            List<HIS_HEIN_SERVICE_TYPE> _heinServiceTypes,
            List<V_HIS_ROOM> _rooms,
            List<V_HIS_SERVICE> _services,
            List<HIS_DEPARTMENT> _department,
            SingleKeyValue _singleKeyValue,
            long _currentDepartmentId
            )
        {
            try
            {
                this.PatyAlter = _patyAlter;
                this.SereServs = _sereServs;
                this.Treatment = _treatment;
                this.DepartmentTrans = _departmentTrans;
                this.TreatmentFees = _treatmentFees;
                this.SingleKeyValue = _singleKeyValue;
                this.HeinServiceTypes = _heinServiceTypes;
                this.Services = _services;
                this.Rooms = _rooms;
                this.SereServKTC = _sereServKTC;
                this.Department = _department;
                this.HeinServiceTypeCFG = _heinServiceTypeCFG;
                this.currentDepartmentId = _currentDepartmentId;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
