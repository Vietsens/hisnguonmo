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
using HIS.Desktop.LocalStorage.BackendData;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.TreeSereServ7V2
{
    public class SereServADO : V_HIS_SERE_SERV_7
    {
        public string CONCRETE_ID__IN_SETY { get; set; }
        public string PARENT_ID__IN_SETY { get; set; }
        public string NOTE_ADO { get; set; }
        public bool? IsLeaf { get; set; }
        public bool? IsExpend { get; set; }
        public bool? IsUsed { get; set; }
        public bool? IsServiceParent { get; set; }
        public string STT { get; set; }
        public long? SAMPLE_TIME { get; set; }
        public long? RECEIVE_SAMPLE_TIME { get; set; }
        public SereServADO()
        {
        }

        public SereServADO(V_HIS_SERE_SERV_7 service)
        {
            Inventec.Common.Mapper.DataObjectMapper.Map<SereServADO>(this, service);
            IsExpend = (service.IS_EXPEND == 1);// IMSys.DbConfig.HIS_RS.HIS_SERE_SERV.IS_EXPEND__TRUE);
        }

        public SereServADO(SereServADO service)
        {
            Inventec.Common.Mapper.DataObjectMapper.Map<SereServADO>(this, service);
            IsExpend = (service.IS_EXPEND == 1);
        }

        public SereServADO(DHisSereServ2 service)
        {
            if (service != null)
            {
                Inventec.Common.Mapper.DataObjectMapper.Map<SereServADO>(this, service);
                this.TDL_SERVICE_CODE = service.SERVICE_CODE;
                this.TDL_SERVICE_NAME = service.SERVICE_NAME;
                if (service.REQUEST_DEPARTMENT_ID.HasValue)
                {
                    this.TDL_REQUEST_DEPARTMENT_ID = service.REQUEST_DEPARTMENT_ID.Value;
                }
                if (service.SERE_SERV_ID.HasValue)
                {
                    this.ID = service.SERE_SERV_ID.Value;
                }
                this.TDL_SERVICE_REQ_CODE = service.SERVICE_REQ_CODE;
                this.IS_NO_EXECUTE = service.IS_NO_EXECUTE;
                this.IS_ANTIBIOTIC_RESISTANCE = service.IS_ANTIBIOTIC_RESISTANCE;
                var serviceType = BackendDataWorker.Get<HIS_SERVICE_TYPE>().FirstOrDefault(p => p.ID == service.TDL_SERVICE_TYPE_ID);
                this.SERVICE_TYPE_NAME = serviceType != null ? serviceType.SERVICE_TYPE_NAME : null;
                this.SERVICE_TYPE_CODE = serviceType != null ? serviceType.SERVICE_TYPE_CODE : null;
                IsUsed = (service.IS_USED == 1);// IMSys.DbConfig.HIS_RS.HIS_SERE_SERV.IS_EXPEND__TRUE);
            }
        }
    }
}
