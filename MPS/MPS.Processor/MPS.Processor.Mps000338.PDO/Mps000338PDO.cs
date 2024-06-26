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
using MPS.Processor.Mps000338.PDO.Config;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000338.PDO
{
    public partial class Mps000338PDO : RDOBase
    {
        public V_HIS_PATIENT_TYPE_ALTER CurrentPatyAlter { get; set; }
        public V_HIS_SERE_SERV SereServ { get; set; }
        public List<V_HIS_SERE_SERV> ListSereServ { get; set; }
        public HIS_SERVICE_REQ ServiceReq { get; set; }
        public V_HIS_BED_LOG HisBedLog { get; set; }

        public Mps000338PDO() { }

        public Mps000338PDO(
            V_HIS_SERE_SERV sereServ,
            List<V_HIS_SERE_SERV> listSereServ,
            V_HIS_TREATMENT_FEE treatment,
            V_HIS_PATIENT_TYPE_ALTER currentPatientTypeAlter,
            HIS_SERVICE_REQ serviceReq
            )
        {
            this.SereServ = sereServ;
            this.ListSereServ = listSereServ;
            this.Treatment = treatment;
            this.CurrentPatyAlter = currentPatientTypeAlter;
            this.ServiceReq = serviceReq;
        }

        public Mps000338PDO(
             V_HIS_SERE_SERV sereServ,
             List<V_HIS_SERE_SERV> listSereServ,
             V_HIS_TREATMENT_FEE treatment,
             V_HIS_PATIENT_TYPE_ALTER currentPatientTypeAlter,
             HIS_SERVICE_REQ serviceReq,
             V_HIS_BED_LOG bedLog
             )
        {
            this.SereServ = sereServ;
            this.ListSereServ = listSereServ;
            this.Treatment = treatment;
            this.CurrentPatyAlter = currentPatientTypeAlter;
            this.ServiceReq = serviceReq;
            this.HisBedLog = bedLog;
        }
    }
}
