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

namespace HIS.UC.CashierRoom
{
  public delegate void GridCustomUnboundColumnDataCashierRoom(MOS.EFMODEL.DataModels.HIS_CASHIER_ROOM data, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e);
  public delegate void BtnRadioEnableClickCashierRoom(MOS.EFMODEL.DataModels.HIS_CASHIER_ROOM data);
  public delegate void GridViewCashierRoomMouseDownCashierRoom(object sender, System.Windows.Forms.MouseEventArgs e);
}
