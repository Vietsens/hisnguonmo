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

namespace HIS.Desktop.Plugins.ExamServiceAdd.ADO
{
    class ThreadDataADO
    {
        public ThreadDataADO(V_HIS_SERVICE_REQ ServiceReqPrint)
        {
            this.VHisServiceReq_print = ServiceReqPrint;
        }

        public V_HIS_SERVICE_REQ VHisServiceReq_print { get; set; }
        public V_HIS_PATIENT VHisPatient { get; set; }
        public V_HIS_PATIENT_TYPE_ALTER VHisPatientTypeAlter { get; set; }
        public V_HIS_SERE_SERV VHisSereServ { get; set; }
        public decimal Ratio { get; set; }
        public string FirstExamRoom { get; set; }
    }
}
