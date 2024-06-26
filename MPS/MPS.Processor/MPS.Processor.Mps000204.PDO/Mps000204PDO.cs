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

namespace MPS.Processor.Mps000204.PDO
{
    public class Mps000204PDO : RDOBase
    {
        public List<V_HIS_SERE_SERV> SereServs { get; set; }
        public V_HIS_SERE_SERV_1 currentSereServ { get; set; }
        public V_HIS_TREATMENT currentTreatment { get; set; }
        public V_HIS_SERE_SERV_PTTT pttt { get; set; }
        public List<V_HIS_EKIP_USER> listEkipUser { get; set; }
        public HIS_SERVICE_REQ currentServiceReq { get; set; }
        public HIS_SERE_SERV_EXT sereServExt { get; set; }

        public List<HIS_EXECUTE_ROLE> HisExecuteRoles { get; set; }

        public Mps000204PDO(
            V_HIS_SERE_SERV_1 currentPatient,
            V_HIS_TREATMENT currentTreatment,
            V_HIS_SERE_SERV_PTTT pttt,
            List<V_HIS_EKIP_USER> listEkipUser,
            HIS_SERVICE_REQ currentServiceReq,
            HIS_SERE_SERV_EXT sereServExtCtr
            )
        {
            this.currentSereServ = currentPatient;
            this.currentTreatment = currentTreatment;
            this.pttt = pttt;
            this.listEkipUser = listEkipUser;
            this.currentServiceReq = currentServiceReq;
            this.sereServExt = sereServExtCtr;
        }

        public Mps000204PDO(
           V_HIS_SERE_SERV_1 currentPatient,
           V_HIS_TREATMENT currentTreatment,
           V_HIS_SERE_SERV_PTTT pttt,
           List<V_HIS_EKIP_USER> listEkipUser,
           HIS_SERVICE_REQ currentServiceReq
           )
        {
            this.currentSereServ = currentPatient;
            this.currentTreatment = currentTreatment;
            this.pttt = pttt;
            this.listEkipUser = listEkipUser;
            this.currentServiceReq = currentServiceReq;
        }

        public Mps000204PDO(
            V_HIS_SERE_SERV_1 currentPatient,
            V_HIS_TREATMENT currentTreatment,
            V_HIS_SERE_SERV_PTTT pttt,
            List<V_HIS_EKIP_USER> listEkipUser,
            HIS_SERVICE_REQ currentServiceReq,
            HIS_SERE_SERV_EXT sereServExtCtr,
            List<HIS_EXECUTE_ROLE> _HisExecuteRoles
            )
        {
            this.currentSereServ = currentPatient;
            this.currentTreatment = currentTreatment;
            this.pttt = pttt;
            this.listEkipUser = listEkipUser;
            this.currentServiceReq = currentServiceReq;
            this.sereServExt = sereServExtCtr;
            this.HisExecuteRoles = _HisExecuteRoles;
        }

        public Mps000204PDO(
           V_HIS_SERE_SERV_1 currentPatient,
           V_HIS_TREATMENT currentTreatment,
           V_HIS_SERE_SERV_PTTT pttt,
           List<V_HIS_EKIP_USER> listEkipUser,
           HIS_SERVICE_REQ currentServiceReq,
            List<HIS_EXECUTE_ROLE> _HisExecuteRoles
           )
        {
            this.currentSereServ = currentPatient;
            this.currentTreatment = currentTreatment;
            this.pttt = pttt;
            this.listEkipUser = listEkipUser;
            this.currentServiceReq = currentServiceReq;
            this.HisExecuteRoles = _HisExecuteRoles;
        }
    }

}
