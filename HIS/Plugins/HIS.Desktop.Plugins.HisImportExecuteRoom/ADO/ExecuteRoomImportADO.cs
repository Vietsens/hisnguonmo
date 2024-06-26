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

namespace HIS.Desktop.Plugins.HisImportExecuteRoom.ADO
{
    public class ExecuteRoomImportADO : V_HIS_EXECUTE_ROOM
    {
        public string NUM_ORDER_STR { get; set; }
        public string ERROR { get; set; }
        public string MAX_REQUEST_BY_DAY_STR { get; set; }
        public string MAX_REQ_BHYT_BY_DAY_STR { get; set; }
        public string HOLD_ORDER_STR { get; set; }
        public string EMERGENCY { get; set; }
        public string EXAM { get; set; }
        public string PAUSE { get; set; }
        public string RESTRICT_EXECUTE_ROOM { get; set; }
        public string RESTRICT_MEDICINE_TYPE { get; set; }
        public string RESTRICT_TIME { get; set; }
        public string SPECIALITY { get; set; }
        public string SURGERY { get; set; }
        public string USE_KIOSK { get; set; }

        public ExecuteRoomImportADO()
        {
        }

        public ExecuteRoomImportADO(V_HIS_EXECUTE_ROOM data)
        {
            Inventec.Common.Mapper.DataObjectMapper.Map<ExecuteRoomImportADO>(this, data);
        }
    }
}
