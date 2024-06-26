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
using EMR.UC.EmrFlow.Popup;
using EMR.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EMR.UC.EmrFlow
{
    public delegate void Grid_CustomUnboundColumnData(V_EMR_FLOW data, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e);
    public delegate void Grid_CustomRowCellEdit(V_EMR_FLOW data, DevExpress.XtraGrid.Views.Base.CustomRowCellEventArgs e);
    public delegate void btn_Radio_Enable_Click(V_EMR_FLOW data);
    public delegate void gridViewEmrFlow_MouseDownEmrFlow(object sender, MouseEventArgs e);
    public delegate void Rooom_MouseRightClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e);
}
