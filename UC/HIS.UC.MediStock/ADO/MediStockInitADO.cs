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

namespace HIS.UC.MediStock.ADO
{
    public class MediStockInitADO
    {
        public List<MediStockADO> ListMediStock { get; set; }
        public List<MediStockColumn> ListMediStockColumn { get; set; }

        public bool? IsShowSearchPanel { get; set; }
        public bool? IsShowMenuPopup { get; set; }

        public Grid_CustomUnboundColumnData MediStockGrid_CustomUnboundColumnData { get; set; }
        public btn_Radio_Enable_Click1 btn_Radio_Enable_Click1 { get; set; }
        public gridViewMediStock_MouseDownMest gridViewMediStock_MouseDownMest { get; set; }
        public GridView_MouseRightClick GridView_MouseRightClick { get; set; }

        public Grid_RowCellClick ListMediStockGrid_RowCellClick { get; set; }
    }
}
