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
using DevExpress.XtraBars;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.ListDepositRequest.ADO
{
    public class ListDepositRequestInitADO
    {
        public List<V_HIS_DEPOSIT_REQ> ListDepositReq { get; set; }
        public List<ListDepositRequestColumn> ListDepositReqColumn { get; set; }
        public V_HIS_TRANSACTION_5 transaction5 { get; set; }
        public bool? IsShowSearchPanel { get; set; }
        public bool? IsShowPagingPanel { get; set; }
        public bool? visibleColumn { get; set; }

        public Grid_CustomUnboundColumnData ListDepositReqGrid_CustomUnboundColumnData { get; set; }
        public Grid_RowCellClick ListDepositReqGrid_RowCellClick { get; set; }
        public Grid_KeyUp ListDepositReqGrid_KeyUp { get; set; }
        public EventHandle _btnDelete_Click { get; set; }
        public EventHandle _btnPrint_Click { get; set; }
        public EventHandle _btnQR_Click { get; set; }
        public Grid_CustomRowCellEdit ListDepositReqGrid_CustomRowCellEdit { get; set; }
        public Grid_RowCellStyle ListDepositReqGrid_RowCellStyle { get; set; }
        public BarManager barManager { get; set; }
        //public Grid_CustomRowCellEdit 
    }
}
