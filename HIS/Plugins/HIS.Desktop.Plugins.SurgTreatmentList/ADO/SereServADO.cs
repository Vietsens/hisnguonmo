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

namespace HIS.Desktop.Plugins.SurgTreatmentList.ADO
{
    public class SereServADO : V_HIS_SERE_SERV_8
    {
        public bool Fee { get; set; }
        public bool GatherData { get; set; }
        public string END_TIME_STR { get; set; }
        public string EXECUTE_ROLE_NAME_1 { get; set; }
        public string EXECUTE_ROLE_NAME_2 { get; set; }
        public string EXECUTE_ROLE_NAME_3 { get; set; }
        public string EXECUTE_ROLE_NAME_4 { get; set; }
        public string EXECUTE_ROLE_NAME_5 { get; set; }
        public string EXECUTE_ROLE_NAME_6 { get; set; }
        public string EXECUTE_ROLE_NAME_7 { get; set; }
        public string EXECUTE_ROLE_NAME_8 { get; set; }
        public string EXECUTE_ROLE_NAME_9 { get; set; }
        public string EXECUTE_ROLE_NAME_10 { get; set; }

        public string REMUNERATION_PRICE_1 { get; set; }
        public string REMUNERATION_PRICE_2 { get; set; }
        public string REMUNERATION_PRICE_3 { get; set; }
        public string REMUNERATION_PRICE_4 { get; set; }
        public string REMUNERATION_PRICE_5 { get; set; }
        public string REMUNERATION_PRICE_6 { get; set; }
        public string REMUNERATION_PRICE_7 { get; set; }
        public string REMUNERATION_PRICE_8 { get; set; }
        public string REMUNERATION_PRICE_9 { get; set; }
        public string REMUNERATION_PRICE_10 { get; set; }

        public SereServADO() { }

        public SereServADO(V_HIS_SERE_SERV_8 data)
        {
            if (data != null)
            {
                Inventec.Common.Mapper.DataObjectMapper.Map<SereServADO>(this, data);
                this.Fee = data.IS_FEE == 1;
                this.GatherData = data.IS_GATHER_DATA == 1;
                this.END_TIME_STR = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.END_TIME ?? 0);
            }
        }
    }
}
