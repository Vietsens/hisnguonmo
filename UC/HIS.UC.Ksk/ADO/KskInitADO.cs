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

namespace HIS.UC.Ksk.ADO
{
    public class KskInitADO
    {
        public List<KskADO> ListKsk { get; set; }
        public List<KskColumn> ListKskColumn { get; set; }
        public bool? IsShowSearchPanel { get; set; }
        public Grid_CustomUnboundColumnData KskGrid_CustomUnboundColumnData { get; set; }
        public btn_Radio_Enable_Click btn_Radio_Enable_Click { get; set; }
        public gridViewKsk_MouseDownKsk gridViewKsk_MouseDownKsk { get; set; }
        public GridView_RowCellClick gridView_RowCellClick { get; set; }
        public GridView_CellValueChanged gridViewKsk_CellValueChanged { get; set; }
        public Check__Enable_CheckedChanged Check__Enable_CheckedChanged2 { get; set; }
        public GridView_MouseRightClick gridView_MouseRightClick { get; set; }
        public DeleteClick deleteClick { get; set; }
        public LockClick lockClick { get; set; }
        public UnLockClick unLockClick { get; set; }
    }
}
