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

namespace MPS.Processor.Mps000433.PDO
{
    public class Mps000433PDO : RDOBase
    {
        public V_HIS_TREATMENT Treatment { get; set; }
        public V_HIS_PATIENT_TYPE_ALTER PatientTypeAlter { get; set; }
        public V_HIS_SERVICE_REQ ServiceReq { get; set; }
        public V_HIS_SERVICE_CHANGE_REQ ReqChange { get; set; }
        public List<HIS_SERE_SERV> ListSereServ { get; set; }
        public V_HIS_PATIENT_TYPE_ALTER patientTypeAlert { get; set; }

        public Mps000433PDO(V_HIS_TREATMENT treatment, V_HIS_PATIENT_TYPE_ALTER patientTypeAlter, V_HIS_SERVICE_REQ serviceReq, V_HIS_SERVICE_CHANGE_REQ reqChange, List<HIS_SERE_SERV> listSereServ)
        {
            this.Treatment = treatment;
            this.PatientTypeAlter = patientTypeAlter;
            this.ServiceReq = serviceReq;
            this.ReqChange = reqChange;
            this.ListSereServ = listSereServ;
        }
        public Mps000433PDO(V_HIS_SERVICE_REQ serviceReq, List<HIS_SERE_SERV> listSereServ, V_HIS_TREATMENT treatment, V_HIS_PATIENT_TYPE_ALTER patientTypeAlter)
        {
            this.Treatment = treatment;
            this.PatientTypeAlter = patientTypeAlter;
            this.ServiceReq = serviceReq;
            this.ListSereServ = listSereServ;
        }
    }
}
