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

using MPS;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPS.ProcessorBase.Core;
using MPS.ProcessorBase;
using MPS.Processor.Mps000045.PDO;

namespace MPS.Processor.Mps000045.PDO
{
    public partial class Mps000045PDO : RDOBase
    {
        public Mps000045PDO(
           PatientADO patientADO,
           PatyAlterBhytADO patyAlterBhytADO,
           List<SereServADO> SereServAdos,
            List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV> SereServ2s,

            List<ServiceGroupPrintADO> highTechServiceReports,
            List<SereServADO> hightTechServices,
            List<V_HIS_SERE_SERV> highTechDepartments,
            List<SereServADO> serviceInHightTechs,

           List<V_HIS_TREATMENT_FEE> treatmentFees,
           string departmentName,
           List<MOS.EFMODEL.DataModels.V_HIS_DEPARTMENT_TRAN> departmentTrans,
           MOS.EFMODEL.DataModels.V_HIS_TRAN_PATI hisTranPati,
           V_HIS_TREATMENT currentTreatment,
           string currentDateSeparateFullTime,
            List<ServiceGroupPrintADO> HeinServiceTypes
            )
        {
            try
            {
                this.patientADO = patientADO;
                this.patyAlterBhytADO = patyAlterBhytADO;
                this.sereServAdos = SereServAdos;
                this.sereServ2s = SereServ2s;

                this.departmentName = departmentName;
                this.treatmentFees = treatmentFees;
                this.departmentTrans = departmentTrans;
                this.hisTranPati = hisTranPati;
                this.currentDateSeparateFullTime = currentDateSeparateFullTime;
                this.currentTreatment = currentTreatment;
                this.HeinServiceTypes = HeinServiceTypes;

                this.HighTechServiceReports = highTechServiceReports;
                this.HightTechServices = hightTechServices;
                this.HighTechDepartments = highTechDepartments;
                this.ServiceInHightTechs = serviceInHightTechs;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
