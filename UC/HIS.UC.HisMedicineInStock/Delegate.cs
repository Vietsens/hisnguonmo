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
using DevExpress.XtraGrid.Views.Grid;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.UC.HisMedicineInStock
{
    public delegate void Grid_CustomUnboundColumnData(MOS.SDO.HisMedicineInStockSDO data, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e);
    public delegate void Grid_CustomRowCellEdit(MOS.SDO.HisMedicineInStockSDO data, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e);
    public delegate void btn_Radio_Enable_Click(MOS.SDO.HisMedicineInStockSDO data);
    public delegate void gridViewService_MouseDown(object sender, MouseEventArgs e);
    public delegate void GridView_Click(MOS.SDO.HisMedicineInStockSDO data);
    public delegate void TxtKeyword_KeyDown(string keyWordText);
    public delegate void GridView_KeyDown(object sender, KeyEventArgs e, MOS.SDO.HisMedicineInStockSDO data);
}
