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

namespace MPS.Processor.Mps000164.PDO
{
    public partial class Mps000164PDO : RDOBase
    {
        public const string printTypeCode = "Mps000164";

        public Mps000164PDO(
            V_HIS_PATIENT _patient,
            HIS_PATIENT_TYPE_ALTER _patyAlterBhyt,
            List<V_HIS_SERE_SERV> _sereServ,
            V_HIS_TREATMENT _treatment,
            List<MOS.EFMODEL.DataModels.V_HIS_DEPARTMENT_TRAN> _departmentTrans,
            string _departmentName,
            string _currentDateSeparateFullTime,
            long _today,
            HeinServiceTypeCFG _heinServiceType,
            PatientTypeCFG _patientType
            )
        {
            try
            {
                this.patient = _patient;
                this.sereServs = _sereServ;
                this.treatment = _treatment;
                this.today = _today;
                this.heinServiceType = _heinServiceType;
                this.patientType = _patientType;
                this.departmentTrans = _departmentTrans;
                this.departmentName = _departmentName;
                this.currentDateSeparateFullTime = _currentDateSeparateFullTime;
                this.patyAlterBHYT = _patyAlterBhyt;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
