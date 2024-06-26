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

namespace MPS.Processor.Mps000324.PDO
{
    public partial class Mps000324PDO
    {
        public Mps000324PDO() { }

        public Mps000324PDO(
            PatientADO patient,
            V_HIS_DEPARTMENT_TRAN departmentTran,
            V_HIS_SERVICE_REQ ServiceReqPrint,
            V_HIS_SERE_SERV_5 sereServ,
            HIS_SERE_SERV_EXT sereServExt,
            V_HIS_SERE_SERV_PTTT sereServsPttt,
            V_HIS_TREATMENT treatment,
            List<V_HIS_EKIP_USER> ekipUsers,
            List<HIS_SERVICE_TYPE> serviceTypes,
            List<HIS_SERVICE_UNIT> serviceUnit,
            List<V_HIS_SERE_SERV> sereServFollows
            )
        {
            try
            {
                this.Patient = patient;
                this.departmentTran = departmentTran;
                this.ServiceReqPrint = ServiceReqPrint;
                this.sereServsPttt = sereServsPttt;
                this.treatment = treatment;
                this.departmentName = departmentName;
                this.ekipUsers = ekipUsers;
                this.SereServExt = sereServExt;
                this.sereServ = sereServ;
                this.ServiceTypes = serviceTypes;
                this.ServiceUnit = serviceUnit;
                this.SereServFollows = sereServFollows;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
