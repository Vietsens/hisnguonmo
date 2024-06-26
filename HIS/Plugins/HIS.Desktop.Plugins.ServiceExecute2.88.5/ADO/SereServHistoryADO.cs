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

namespace HIS.Desktop.Plugins.ServiceExecute.ADO
{
    class SereServHistoryADO : V_HIS_SERE_SERV_1
    {
        public DateTime? Intruction_Date { get; set; }
        public HIS_SERE_SERV_EXT HisSereServExt { get; set; }
        public long TIME_END { get; set; }

        public SereServHistoryADO(V_HIS_SERE_SERV_1 data, HIS_SERE_SERV_EXT Ext)
        {
            try
            {
                if (data != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<SereServHistoryADO>(this, data);

                    Intruction_Date = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(data.FINISH_TIME ?? data.INTRUCTION_TIME);
                    TIME_END = data.FINISH_TIME ?? data.INTRUCTION_TIME;
                }

                HisSereServExt = Ext;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public SereServHistoryADO(V_HIS_SERE_SERV_1 data)
        {
            try
            {
                if (data != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<SereServHistoryADO>(this, data);
                    TIME_END = data.FINISH_TIME ?? data.INTRUCTION_TIME;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
