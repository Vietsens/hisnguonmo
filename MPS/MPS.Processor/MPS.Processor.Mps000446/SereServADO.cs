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

namespace MPS.Processor.Mps000446
{
    class SereServADO : HIS_SERE_SERV
    {
        public string SERVICE_TYPE_CODE { get; set; }
        public string SERVICE_TYPE_NAME { get; set; }
        public long? SERVICE_TYPE_NUM_ORDER { get; set; }

        public long? SERVICE_NUM_ORDER { get; set; }
        public long? SERVICE_PARENT_NUM_ORDER { get; set; }

        public string REQUEST_ROOM_CODE { get; set; }
        public string REQUEST_ROOM_NAME { get; set; }
        public long STT { get; set; }

        public SereServADO(HIS_SERE_SERV data, List<V_HIS_ROOM> hisRoom, List<V_HIS_SERVICE> hisService, List<HIS_SERVICE_TYPE> hisServiceType)
        {
            try
            {
                Inventec.Common.Mapper.DataObjectMapper.Map<SereServADO>(this, data);

                var serviceType = hisServiceType.FirstOrDefault(o => o.ID == data.TDL_SERVICE_TYPE_ID);
                if (serviceType != null)
                {
                    this.SERVICE_TYPE_CODE = serviceType.SERVICE_TYPE_CODE;
                    this.SERVICE_TYPE_NAME = serviceType.SERVICE_TYPE_NAME;
                    this.SERVICE_TYPE_NUM_ORDER = serviceType.NUM_ORDER;
                }

                V_HIS_SERVICE service = hisService.FirstOrDefault(o => o.ID == data.SERVICE_ID);
                if (service != null && service.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN)
                {
                    this.SERVICE_NUM_ORDER = service.NUM_ORDER;
                    if (service.PARENT_ID.HasValue)
                    {
                        V_HIS_SERVICE parent = hisService.FirstOrDefault(o => o.ID == service.PARENT_ID.Value);
                        if (parent != null)
                        {
                            this.SERVICE_PARENT_NUM_ORDER = parent.NUM_ORDER ?? -1;
                        }
                    }
                }
                else
                {
                    this.SERVICE_NUM_ORDER = -1;
                    this.SERVICE_PARENT_NUM_ORDER = -1;
                }

                var reqRoom = hisRoom.FirstOrDefault(o => o.ID == data.TDL_REQUEST_ROOM_ID);
                if (reqRoom != null)
                {
                    this.REQUEST_ROOM_CODE = reqRoom.ROOM_CODE;
                    this.REQUEST_ROOM_NAME = reqRoom.ROOM_NAME;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public SereServADO(SereServADO sereServADO)
        {
            Inventec.Common.Mapper.DataObjectMapper.Map<SereServADO>(this, sereServADO);
        }

        public decimal? TOTAL_PRICE_SERVICE_GROUP { get; set; }

        public decimal? TOTAL_HEIN_PRICE_SERVICE_GROUP { get; set; }

        public decimal? TOTAL_PATIENT_PRICE_SERVICE_GROUP { get; set; }
    }
}
