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
using DevExpress.XtraGrid.Views.Base;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.UC.Ksk
{
    public delegate void Grid_CustomUnboundColumnData(HIS_KSK data, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e);
    public delegate void Grid_CustomRowCellEdit(HIS_KSK data, DevExpress.XtraGrid.Views.Base.CustomRowCellEventArgs e);
    public delegate void btn_Radio_Enable_Click(HIS_KSK data);
    public delegate void GridView_RowCellClick(KskADO data);
    public delegate void gridViewKsk_MouseDownKsk(object sender, MouseEventArgs e);
    public delegate void GridView_CellValueChanged(object sender, CellValueChangedEventArgs e);
    public delegate void Check__Enable_CheckedChanged(KskADO ado);
    public delegate void GridView_MouseRightClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e);
    public delegate void DeleteClick(KskADO data);
    public delegate void LockClick(KskADO data);
    public delegate void UnLockClick(KskADO data);
}
