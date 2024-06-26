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

namespace MPS.Processor.Mps000060.PDO
{
    public partial class Mps000060PDO : RDOBase
    {
        public const string printTypeCode = "Mps000060";

        public Mps000060PDO(
            PatientADO patient,
            PatyAlterBhytADO patyAlterBhyt,
            List<SereServADO> sereServADO,
            V_HIS_TREATMENT treatment,

             List<ServiceGroupPrintADO> HighTechServiceReports,
            List<SereServADO> HightTechServices,
            List<V_HIS_SERE_SERV> HighTechDepartments,
            List<SereServADO> ServiceInHightTechs,

            List<ServiceGroupPrintADO> hisServiceReports,
            List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT_FEE> treatmentFees,
            MOS.EFMODEL.DataModels.V_HIS_TRAN_PATI hisTranPati,
            List<MOS.EFMODEL.DataModels.V_HIS_DEPARTMENT_TRAN> departmentTrans
            )
        {
            try
            {
                this.Patient = patient;
                this.PatyAlterBhyt = patyAlterBhyt;
                this.PatientTypeAlter = PatientTypeAlter;
                this.sereServADO = sereServADO;
                this.currentTreatment = treatment;
                this.HeinServiceTypes = hisServiceReports;
                this.departments = departments;
                this.treatmentFees = treatmentFees;
                this.hisTranPati = hisTranPati;
                this.departmentTrans = departmentTrans;

                this.highTechServiceReports = HighTechServiceReports;
                this.hightTechServices = HightTechServices;
                this.highTechDepartments = HighTechDepartments;
                this.serviceInHightTechs = ServiceInHightTechs;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
