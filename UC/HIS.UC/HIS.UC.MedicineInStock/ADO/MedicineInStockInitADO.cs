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

namespace HIS.UC.HisMedicineInStock
{
    public class HisMedicineInStockInitADO
    {
        public List<ColumnButtonEditADO> ColumnButtonEdits { get; set; }
        public List<HisMedicineInStockColumn> HisMedicineInStockColumns { get; set; }
        public List<MOS.SDO.HisMedicineInStockSDO> HisMedicineInStocks { get; set; }

        public bool? IsShowSearchPanel { get; set; }
        public bool? IsShowButtonAdd { get; set; }
        public bool? IsShowCheckNode { get; set; }
        public bool? IsAutoWidth { get; set; }
        public HisMedicineInStock_NodeCellStyle HisMedicineInStockNodeCellStyle { get; set; }
        public HisMedicineInStockHandler HisMedicineInStockClick { get; set; }
        public HisMedicineInStockHandler HisMedicineInStockDoubleClick { get; set; }
        public HisMedicineInStockHandler HisMedicineInStockRowEnter { get; set; }
        public HisMedicineInStock_GetStateImage HisMedicineInStock_GetStateImage { get; set; }
        public HisMedicineInStock_GetSelectImage HisMedicineInStock_GetSelectImage { get; set; }
        public HisMedicineInStockHandler HisMedicineInStock_StateImageClick { get; set; }
        public HisMedicineInStockHandler HisMedicineInStock_SelectImageClick { get; set; }
        public HisMedicineInStock_CustomUnboundColumnData HisMedicineInStock_CustomUnboundColumnData { get; set; }
        public HisMedicineInStock_AfterCheck HisMedicineInStock_AfterCheck { get; set; }
        public HisMedicineInStock_BeforeCheck HisMedicineInStock_BeforeCheck { get; set; }
        public HisMedicineInStock_CheckAllNode HisMedicineInStock_CheckAllNode { get; set; }
        public HisMedicineInStock_CustomDrawNodeCell HisMedicineInStock_CustomDrawNodeCell { get; set; }
        public btnLock_buttonClick btnLock_buttonClick { get; set; }
        public btnUnLock_buttonClick btnUnLock_buttonClick { get; set; }
        public DevExpress.Utils.ImageCollection StateImageCollection { get; set; }
        public DevExpress.Utils.ImageCollection SelectImageCollection { get; set; }

        public HisMedicineInStockHandler UpdateSingleRow { get; set; }
        public MenuItems MenuItems { get; set; }

        public bool? IsCreateParentNodeWithHisMedicineInStockExpend { get; set; }

        public string LayoutHisMedicineInStockExpend { get; set; }
        public string Keyword_NullValuePrompt { get; set; }
        public string KeyFieldName { get; set; }
        public string ParentFieldName { get; set; }
        public string NameButtonClose { get; set; }
        public string NameButtonOpen { get; set; }
    }
}
