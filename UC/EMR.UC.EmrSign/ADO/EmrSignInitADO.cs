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

namespace EMR.UC.EmrSign.ADO
{
    public class EmrSignInitADO
    {
        public List<EmrSignADO> ListEmrSign { get; set; }
        public List<EmrSignColumn> ListEmrSignColumn { get; set; }
        public Grid_CustomUnboundColumnData EmrSignGrid_CustomUnboundColumnData { get; set; }
        public btn_Radio_Enable_Click1 btn_Radio_Enable_Click1 { get; set; }
        public Grid_CellValueChanged EmrSignGrid_CellValueChanged { get; set; }
        public Grid_MouseDown EmrSignGrid_MouseDown { get; set; }
    }
}