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
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.Material.ADO
{
    public class MaterialInitADO
    {
        public List<MaterialADO> ListMaterial { get; set; }
        public List<MaterialColumn> ListMaterialColumn { get; set; }

        public bool? IsShowSearchPanel { get; set; }

        public Grid_CustomUnboundColumnData MaterialGrid_CustomUnboundColumnData { get; set; }
        public btn_Radio_Enable_Click_Mate btn_Radio_Enable_Click_Mate { get; set; }
        public gridViewMaterial_MouseDownMate gridViewMaterial_MouseDownMate { get; set; }
        public Check__Enable_CheckedChanged Check__Enable_CheckedChanged { get; set; }

        public Grid_RowCellClick ListMaterialGrid_RowCellClick { get; set; }

        public GridView_MouseRightClick gridView_MouseRightClick { get; set; }
    }
}
