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
    class ThreadChiDinhDichVuADO
    {
        public HIS_TREATMENT hisTreatment { get; set; }
        public List<V_HIS_SERE_SERV> listVHisSereServ { get; set; }
        public V_HIS_PATIENT_TYPE_ALTER vHisPatientTypeAlter { get; set; }
        public ADO.ServiceReqADO vHisServiceReq2Print { get; set; }

        public List<HIS_SERE_SERV_DEPOSIT> ListSereServDeposit { get; set; }
        public List<HIS_SERE_SERV_BILL> ListSereServBill { get; set; }

        public ThreadChiDinhDichVuADO() { }

        public ThreadChiDinhDichVuADO(ADO.ServiceReqADO data)
        {
            try
            {
                if (data != null)
                {
                    this.vHisServiceReq2Print = data;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
