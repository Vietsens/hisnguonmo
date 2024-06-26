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
using HIS.Desktop.ADO;
using EMR.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMR.UC.EmrFlow.ADO
{
    public class EmrFlowInitADO
    {
        public List<EmrFlowADO> ListEmrFlow { get; set; }
        public List<EmrFlowColumn> ListEmrFlowColumn { get; set; }

        public bool? IsShowSearchPanel { get; set; }

        public Grid_CustomUnboundColumnData EmrFlowGrid_CustomUnboundColumnData { get; set; }
        public btn_Radio_Enable_Click btn_Radio_Enable_Click { get; set; }
        public gridViewEmrFlow_MouseDownEmrFlow gridViewEmrFlow_MouseDownEmrFlow { get; set; }
        public Rooom_MouseRightClick rooom_MouseRightClick { get; set; }
        public bool? IsShowMenuPopup { get; set; }
    }
}
