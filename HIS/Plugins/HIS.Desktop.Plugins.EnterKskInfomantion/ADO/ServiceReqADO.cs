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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.EnterKskInfomantion.ADO
{
    public class ServiceReqADO : V_HIS_SERVICE_REQ_2
    {
        public bool IsChecked { get; set; }

        public HIS_KSK_GENERAL KSK_GENERAL { get; set; }
        public HIS_KSK_OCCUPATIONAL KSK_OCCUPATIONAL { get; set; }

        public ServiceReqADO()
        {

        }

        public ServiceReqADO(V_HIS_SERVICE_REQ_2 data)
        {
            try
            {
                if (data != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<ServiceReqADO>(this, data);

                    var serviceReqStt = BackendDataWorker.Get<HIS_SERVICE_REQ_STT>().FirstOrDefault(o => o.ID == data.SERVICE_REQ_STT_ID);
                    if (serviceReqStt != null)
                    {
                        this.SERVICE_REQ_STT_CODE = serviceReqStt.SERVICE_REQ_STT_CODE;
                        this.SERVICE_REQ_STT_NAME = serviceReqStt.SERVICE_REQ_STT_NAME;
                    }
                    var serviceReqType = BackendDataWorker.Get<HIS_SERVICE_REQ_TYPE>().FirstOrDefault(o => o.ID == data.SERVICE_REQ_TYPE_ID);
                    if (serviceReqType != null)
                    {
                        this.SERVICE_REQ_TYPE_CODE = serviceReqType.SERVICE_REQ_TYPE_CODE;
                        this.SERVICE_REQ_TYPE_NAME = serviceReqType.SERVICE_REQ_TYPE_NAME;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
