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

namespace HIS.Desktop.Plugins.ServiceReqList.ADO
{
    class ThreadPtttADO
    {
        public V_HIS_PATIENT patient { get; set; }
        public V_HIS_TREATMENT vhisTreatment { get; set; }
        public V_HIS_DEPARTMENT_TRAN departmentTran { get; set; }
        public V_HIS_SERVICE_REQ serviceReq { get; set; }
        public List<V_HIS_EKIP_USER> ekipUsers { get; set; }
        public V_HIS_SERE_SERV_PTTT sereServPttts { get; set; }
        public V_HIS_SERE_SERV_5 sereServ5Print { get; set; }
        public HIS_SERE_SERV sereServPrint { get; set; }
        public List<HIS_SERE_SERV_FILE> sereServFile { get; set; }

        public ThreadPtttADO() { }

        public ThreadPtttADO(HIS_SERE_SERV data)
        {
            if (data != null)
            {
                this.sereServPrint = data;
            }
        }
    }
}
