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

namespace HIS.UC.HisMedicineInStock.ADO
{
    public class HisMedicineInStockInitADO
    {
        public List<MOS.SDO.HisMedicineInStockSDO> hisMedicineInStockSDOs { get; set; }
        public List<MedicineInStockColumn> MedicineInStockColumns { get; set; }
        public Grid_CustomUnboundColumnData MedicineInStockGrid_CustomUnboundColumnData { get; set; }
        public btn_Radio_Enable_Click btnRadioEnable_Click { get; set; }
        public gridViewService_MouseDown gridView_MouseDown { get; set; }
        public GridView_Click gridView_Click { get; set; }
        public TxtKeyword_KeyDown txtKeyword_KeyDown { get; set; }
        public GridView_KeyDown gridView_KeyDown { get; set; }
    }
}
