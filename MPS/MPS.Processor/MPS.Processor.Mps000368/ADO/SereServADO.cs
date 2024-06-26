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

namespace MPS.Processor.Mps000368.ADO
{
    class SereServADO : V_HIS_SERE_SERV
    {
        public long? SERVICE_NUM_ORDER { get; set; }
        public decimal? ESTIMATE_DURATION { get; set; }

        public string CONCLUDE { get; set; }
        public long? BEGIN_TIME { get; set; }
        public long? END_TIME { get; set; }
        public string INSTRUCTION_NOTE { get; set; }
        public string NOTE { get; set; }

        public SereServADO(V_HIS_SERE_SERV sereServADO, PDO.Mps000368PDO rdo)
        {
            if (sereServADO != null)
            {
                Inventec.Common.Mapper.DataObjectMapper.Map<SereServADO>(this, sereServADO);
            }

            if (rdo != null)
            {
                if (rdo.Services != null && rdo.Services.Count > 0)
                {
                    var service = rdo.Services.FirstOrDefault(o => o.ID == this.SERVICE_ID);
                    if (service != null)
                    {
                        this.SERVICE_NUM_ORDER = service.NUM_ORDER;
                        this.ESTIMATE_DURATION = service.ESTIMATE_DURATION;
                    }
                }

                if (rdo.SereServExts != null && rdo.SereServExts.Count > 0)
                {
                    var ext = rdo.SereServExts.FirstOrDefault(o => o.SERE_SERV_ID == this.ID);
                    if (ext != null)
                    {
                        this.CONCLUDE = ext.CONCLUDE;
                        this.BEGIN_TIME = ext.BEGIN_TIME;
                        this.END_TIME = ext.END_TIME;
                        this.INSTRUCTION_NOTE = ext.INSTRUCTION_NOTE;
                        this.NOTE = ext.NOTE;
                    }
                }
            }
        }
    }
}
