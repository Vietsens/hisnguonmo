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
using HIS.UC.LisBacterium.Popup;
using LIS.EFMODEL.DataModels;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.UC.LisBacterium
{
    public delegate void Grid_CustomUnboundColumnData(LIS_BACTERIUM data, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e);
    public delegate void Grid_CustomRowCellEdit(LIS_BACTERIUM data, DevExpress.XtraGrid.Views.Base.CustomRowCellEventArgs e);
    public delegate void btn_Radio_Enable_Click(LIS_BACTERIUM data);
    public delegate void gridViewRoom_MouseDownRoom(object sender, MouseEventArgs e);
    public delegate void Rooom_MouseRightClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e);
}
