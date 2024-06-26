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

namespace HIS.UC.HisMaterialInStock
{
    public class HisMaterialInStockInitADO
    {
        public List<ColumnButtonEditADO> ColumnButtonEdits { get; set; }
        public List<HisMaterialInStockColumn> HisMaterialInStockColumns { get; set; }
        public List<MOS.SDO.HisMaterialInStockSDO> HisMaterialInStocks { get; set; }

        public bool? IsShowSearchPanel { get; set; }
        public bool? IsShowButtonAdd { get; set; }
        public bool? IsShowCheckNode { get; set; }
        public HisMaterialInStock_NodeCellStyle HisMaterialInStockNodeCellStyle { get; set; }
        public HisMaterialInStockHandler HisMaterialInStockClick { get; set; }
        public HisMaterialInStockHandler HisMaterialInStockDoubleClick { get; set; }
        public HisMaterialInStockHandler HisMaterialInStockRowEnter { get; set; }
        public HisMaterialInStock_GetStateImage HisMaterialInStock_GetStateImage { get; set; }
        public HisMaterialInStock_GetSelectImage HisMaterialInStock_GetSelectImage { get; set; }
        public HisMaterialInStockHandler HisMaterialInStock_StateImageClick { get; set; }
        public HisMaterialInStockHandler HisMaterialInStock_SelectImageClick { get; set; }
        public HisMaterialInStock_CustomUnboundColumnData HisMaterialInStock_CustomUnboundColumnData { get; set; }
        public HisMaterialInStock_AfterCheck HisMaterialInStock_AfterCheck { get; set; }
        public HisMaterialInStock_BeforeCheck HisMaterialInStock_BeforeCheck { get; set; }
        public HisMaterialInStock_CheckAllNode HisMaterialInStock_CheckAllNode { get; set; }
        public HisMaterialInStock_CustomDrawNodeCell HisMaterialInStock_CustomDrawNodeCell { get; set; }
        public btnLock_buttonClick btnLock_buttonClick { get; set; }
        public btnUnLock_buttonClick btnUnLock_buttonClick { get; set; }
        public DevExpress.Utils.ImageCollection StateImageCollection { get; set; }
        public DevExpress.Utils.ImageCollection SelectImageCollection { get; set; }

        public HisMaterialInStockHandler UpdateSingleRow { get; set; }
        public MenuItems MenuItems { get; set; }

        public bool? IsCreateParentNodeWithHisMaterialInStockExpend { get; set; }

        public string LayoutHisMaterialInStockExpend { get; set; }
        public string KeyFieldName { get; set; }
        public string ParentFieldName { get; set; }
        public string NameButtonClose { get; set; }
        public string NameButtonOpen { get; set; }
    }
}
