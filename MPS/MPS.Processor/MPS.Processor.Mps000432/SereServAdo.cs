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

namespace MPS.Processor.Mps000432
{
    class SereServAdo : V_HIS_SERE_SERV
    {

        public SereServAdo(V_HIS_SERE_SERV data, List<V_HIS_SERVICE> lstService, List<HIS_SERVICE_CONDITION> lstCondition)
        {
            if (data != null)
            {
                Inventec.Common.Mapper.DataObjectMapper.Map<SereServAdo>(this, data);

                V_HIS_SERVICE service = lstService != null ? lstService.FirstOrDefault(o => o.ID == data.SERVICE_ID) : null;
                if (service != null)
                {
                    this.SERVICE_NUM_ORDER = service.NUM_ORDER;
                    this.ESTIMATE_DURATION = service.ESTIMATE_DURATION;
                    this.SERVICE_ORDER = service.NUM_ORDER ?? -1;
                    if (service.PARENT_ID.HasValue)
                    {
                        V_HIS_SERVICE parent = lstService != null ? lstService.FirstOrDefault(o => o.ID == service.PARENT_ID.Value) : null;
                        if (parent != null)
                        {
                            this.SERVICE_PARENT_ORDER = parent.NUM_ORDER ?? -1;
                        }
                    }
                }
                else
                {
                    this.SERVICE_ORDER = -1;
                    this.SERVICE_PARENT_ORDER = -1;
                }

                HIS_SERVICE_CONDITION Condition = lstCondition != null ? lstCondition.FirstOrDefault(o => o.ID == data.SERVICE_CONDITION_ID) : null;
                if (Condition != null)
                {
                    this.SERVICE_CONDITION_CODE = Condition.SERVICE_CONDITION_CODE;
                    this.SERVICE_CONDITION_NAME = Condition.SERVICE_CONDITION_NAME;
                }
            }
        }

        public long? SERVICE_NUM_ORDER { get; set; }
        public long? SERVICE_ORDER { get; set; }
        public long? SERVICE_PARENT_ORDER { get; set; }

        public decimal? ESTIMATE_DURATION { get; set; }

        public string SERVICE_CONDITION_CODE { get; set; }
        public string SERVICE_CONDITION_NAME { get; set; }
    }
}
