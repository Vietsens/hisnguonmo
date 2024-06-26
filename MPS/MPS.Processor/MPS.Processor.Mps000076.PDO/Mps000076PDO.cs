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
using MPS.Processor.Mps000076.PDO;
using MOS.SDO;

namespace MPS.Processor.Mps000076.PDO
{
    public partial class Mps000076PDO : RDOBase
    {
        public Mps000076PDO(
            PatientADO patient,
            PatyAlterBhytADO patyAlterBhyt,

            List<V_HIS_SERE_SERV> HisSereServ_Bordereaus,

            List<ServiceGroupPrintADO> HighTechServiceReports,
            List<SereServADO> HightTechServices,
            List<V_HIS_SERE_SERV> HighTechDepartments,
            List<SereServADO> ServiceInHightTechs,

            List<ServiceGroupPrintADO> ListGroupServiceTypeADO,
            List<SereServADO> Departments,
            List<SereServADO> SereServADOs,

            List<V_HIS_TREATMENT_FEE> treatmentFees,
            V_HIS_TRAN_PATI hisTranPati,
            List<V_HIS_DEPARTMENT_TRAN> departmentTrans
            )
        {
            try
            {
                this.Patient = patient;
                this.PatyAlterBhyt = patyAlterBhyt;

                this.hisSereServ_Bordereaus = HisSereServ_Bordereaus;

                this.highTechServiceReports = HighTechServiceReports;
                this.hightTechServices = HightTechServices;
                this.highTechDepartments = HighTechDepartments;
                this.serviceInHightTechs = ServiceInHightTechs;

                this.ListGroupServiceTypeADO = ListGroupServiceTypeADO;
                this.departmentADOs = Departments;
                this.sereServADOs = SereServADOs;

                this.treatmentFees = treatmentFees;
                this.hisTranPati = hisTranPati;
                this.departmentTrans = departmentTrans;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
