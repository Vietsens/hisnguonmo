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

namespace MPS.Processor.Mps000196.PDO
{
    public partial class Mps000196PDO : RDOBase
    {

        public Mps000196PDO(
            V_HIS_PATIENT _patient,
            List<HIS_SERE_SERV> _sereServ,
            V_HIS_TREATMENT _treatment,
            List<V_HIS_EXP_MEST> _expMests,
            List<HIS_HEIN_SERVICE_TYPE> _heinServiceTypes,
            List<V_HIS_TREATMENT_FEE> _treatmentFees,
            List<V_HIS_DEPARTMENT_TRAN> _departmentTrans,
            List<V_HIS_ROOM> _rooms,
            List<V_HIS_SERVICE> _services,
            HeinServiceTypeCFG _heinServiceType,
            PatientTypeCFG _patientType,
            BordereauSingleValue _bordereauSingleValue
            )
        {
            try
            {
                this.patient = _patient;
                this.sereServs = _sereServ;
                this.treatment = _treatment;
                this.heinServiceTypeCfg = _heinServiceType;
                this.patientType = _patientType;
                this.departmentTrans = _departmentTrans;
                this.treatmentFees = _treatmentFees;
                this.bordereauSingleValue = _bordereauSingleValue;
                this.expMests = _expMests;
                this.HeinServiceTypes = _heinServiceTypes;
                this.Rooms = _rooms;
                this.Services = _services;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
