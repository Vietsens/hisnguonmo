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

namespace MPS.Processor.Mps000035.PDO
{
    public partial class Mps000035PDO : RDOBase
    {
        public Mps000035PDO(
            V_HIS_PATIENT _patient,
            V_HIS_DEPARTMENT_TRAN _departmentTran,
            V_HIS_SERVICE_REQ _serviceReq,
            V_HIS_TREATMENT _treatment
            )
        {
            try
            {
                this.Patient = _patient;
                this.DepartmentTran = _departmentTran;
                this.ServiceReq = _serviceReq;
                this.Treatment = _treatment;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
