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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.SurgServiceReqExecute.Base
{
    public class PtttMethodADO
    {
        public long ID { get; set; }
        public string PTTT_METHOD_CODE { get; set; }
        public string PTTT_METHOD_NAME { get; set; }
        public decimal? AMOUNT { get; set; }
        public bool IS_SELECTION { get; set; }
        public bool IS_COMBO { get; set; }
        public long? PTTT_GROUP_ID { get; set; }
        public string PTTT_GROUP_NAME { get; set; }
        public long SERE_SERV_ID { get; set; }
        public long? EKIP_ID { get; set; }
        public long SERVICE_REQ_ID { get; set; }
        public EkipUsersADO EkipUsersADO { get; set; }
    }
}
