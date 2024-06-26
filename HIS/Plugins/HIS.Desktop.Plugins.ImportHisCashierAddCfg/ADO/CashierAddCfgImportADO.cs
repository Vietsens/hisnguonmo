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

namespace HIS.Desktop.Plugins.ImportHisCashierAddCfg.ADO
{
    public class CashierAddCfgImportADO : HIS_CASHIER_ADD_CONFIG
    {

        public string REQUEST_ROOM_CODE_STR { get; set; }
        public string REQUEST_ROOM_NAME_STR { get; set; }

        public string EXECUTE_ROOM_CODE_STR { get; set; }
        public string EXECUTE_ROOM_NAME_STR { get; set; }

        public string CASHIER_ROOM_CODE_STR { get; set; }
        public string CASHIER_ROOM_NAME_STR { get; set; }

        public string INSTR_DAY_FROM_STR { get; set; }
        public string INSTR_DAY_TO_STR { get; set; }

        public string INSTR_TIME_FROM_STR { get; set; }
        public string INSTR_TIME_TO_STR { get; set; }

        public string IS_NOT_PRIORITY_STR { get; set; }

        public string ERROR { get; set; }

        public CashierAddCfgImportADO()
        {
        }

        public CashierAddCfgImportADO(HIS_CASHIER_ADD_CONFIG data)
        {
            Inventec.Common.Mapper.DataObjectMapper.Map<CashierAddCfgImportADO>(this, data);
        }
    }
}
