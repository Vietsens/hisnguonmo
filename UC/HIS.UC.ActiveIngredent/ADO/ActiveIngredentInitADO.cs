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

namespace HIS.UC.ActiveIngredent.ADO
{
    public class ActiveIngredentInitADO
    {
        public List<ActiveIngredentADO> ListActiveIngredent { get; set; }
        public List<ActiveIngredentColumn> ListActiveIngredentColumn { get; set; }
        public Grid_CustomUnboundColumnData ActiveIngredentGrid_CustomUnboundColumnData { get; set; }
        public btn_Radio_Enable_Click btn_Radio_Enable_Click { get; set; }
        public Grid_CellValueChanged ActiveIngredentGrid_CellValueChanged { get; set; }
        public Grid_MouseDown ActiveIngredentGrid_MouseDown { get; set; }
    }
}
