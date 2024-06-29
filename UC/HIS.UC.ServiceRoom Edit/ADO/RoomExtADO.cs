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
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.ServiceRoom.ADO
{
    public class RoomExtADO // : V_HIS_EXECUTE_ROOM_1
    {
        public string EXECUTE_ROOM_CODE { get; set; }
        public string EXECUTE_ROOM_NAME { get; set; }
        public long ROOM_ID { get; set; }
        public long? NUM_ORDER { get; set; }
        public decimal? TOTAL_NEW_SERVICE_REQ { get; set; }
        public decimal? TOTAL_TODAY_SERVICE_REQ { get; set; }
        public decimal? TOTAL_WAIT_TODAY_SERVICE_REQ { get; set; }
        public decimal? TOTAL_OPEN_SERVICE_REQ { get; set; }
        public decimal? MAX_REQ_BHYT_BY_DAY { get; set; }
        public decimal? MAX_REQUEST_BY_DAY { get; set; }

        public string AMOUNT_COMBO { get; set; }
        public long IS_WARN { get; set; }

        public RoomExtADO() : base() { }
        public RoomExtADO(L_HIS_ROOM_COUNTER data)
            : base()
        {
            this.EXECUTE_ROOM_CODE = data.EXECUTE_ROOM_CODE;
            this.EXECUTE_ROOM_NAME = data.EXECUTE_ROOM_NAME;
            this.NUM_ORDER = data.NUM_ORDER;
            this.ROOM_ID = data.ROOM_ID;
            this.TOTAL_TODAY_SERVICE_REQ = data.TOTAL_TODAY_SERVICE_REQ;
            this.TOTAL_NEW_SERVICE_REQ = data.TOTAL_NEW_SERVICE_REQ;
            this.TOTAL_WAIT_TODAY_SERVICE_REQ = data.TOTAL_WAIT_TODAY_SERVICE_REQ;
            this.MAX_REQ_BHYT_BY_DAY = data.MAX_REQ_BHYT_BY_DAY;
            this.MAX_REQUEST_BY_DAY = data.MAX_REQUEST_BY_DAY;
            this.TOTAL_OPEN_SERVICE_REQ = data.TOTAL_TODAY_SERVICE_REQ - data.TOTAL_NEW_SERVICE_REQ;

            long TOTAL_TODAY_1 = Convert.ToInt64(data.TOTAL_TODAY_SERVICE_REQ ?? 0);
            long MAX_BY_DAY_1 = data.MAX_REQUEST_BY_DAY ?? 0;

            if (MAX_BY_DAY_1 < TOTAL_TODAY_1)
            {
                this.IS_WARN = 1;
            }
        }

        public RoomExtADO(V_HIS_EXECUTE_ROOM_1 data)
            : base()
        {
            this.EXECUTE_ROOM_CODE = data.EXECUTE_ROOM_CODE;
            this.EXECUTE_ROOM_NAME = data.EXECUTE_ROOM_NAME;
            this.NUM_ORDER = data.NUM_ORDER;
            this.ROOM_ID = data.ROOM_ID;

            long TOTAL_OPEN_1 = Convert.ToInt64(data.TOTAL_OPEN_SERVICE_REQ ?? 0);
            long TOTAL_TODAY_1 = Convert.ToInt64(data.TOTAL_TODAY_SERVICE_REQ ?? 0);
            long MAX_BY_DAY_1 = data.MAX_REQUEST_BY_DAY ?? 0;
            this.AMOUNT_COMBO = TOTAL_OPEN_1 + "/" + TOTAL_TODAY_1 + "(" + MAX_BY_DAY_1 + ")";

            if (MAX_BY_DAY_1 < TOTAL_TODAY_1)
            {
                this.IS_WARN = 1;
            }
        }
    }
}
