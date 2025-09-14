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

namespace HIS.Desktop.Plugins.CallPatientExamV2
{
    public class ServiceReqGateADO
    {
        public List<RoomGateSDO> roomGateSDOs { get; set; } = new List<RoomGateSDO>();
        public long timeReload { get; set; }
        public long sizeTitle { get; set; }
        public string colorTitle { get; set; }
        public string bgColorTitle { get; set; }
        public long sizeDangKham { get; set; }
        public long sizeContentNumber { get; set; }
        public string colorContent { get; set; }
        public long sizeChoKham { get; set; }
        public long sizeEndTitle { get; set; }
        public string colorEnd { get; set; }
        public string bgColorEnd { get; set; }
        public bool isAutoOpenWaitingScreen { get; set; }
        public string configNotify { get; set; }
    }
}
