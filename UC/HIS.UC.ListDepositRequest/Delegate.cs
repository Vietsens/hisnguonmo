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

namespace HIS.UC.ListDepositRequest
{
    public delegate void Grid_CustomUnboundColumnData(V_HIS_DEPOSIT_REQ data, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e);
    public delegate void Grid_RowCellClick(V_HIS_DEPOSIT_REQ data);
    public delegate void Grid_KeyUp(V_HIS_DEPOSIT_REQ data);
    public delegate void EventHandle(V_HIS_DEPOSIT_REQ data);
    public delegate void Grid_CustomRowCellEdit(V_HIS_DEPOSIT_REQ data,DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e);
    public delegate void Grid_RowCellStyle(V_HIS_DEPOSIT_REQ data, RowCellStyleEventArgs e);

}
