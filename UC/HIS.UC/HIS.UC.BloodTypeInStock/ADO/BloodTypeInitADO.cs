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
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.HisBloodTypeInStock
{
    public class HisBloodTypeInStockInitADO
    {
        public List<ColumnButtonEditADO> ColumnButtonEdits { get; set; }
        public List<HisBloodTypeInStockColumn> HisBloodTypeInStockColumns { get; set; }
        public List<HisBloodTypeInStockSDO> HisBloodTypeInStocks { get; set; }
        public List<HisBloodInStockSDO> HisBloodInStocks { get; set; }

        public bool? IsShowSearchPanel { get; set; }
        public bool? IsShowButtonAdd { get; set; }
        public bool? IsShowCheckNode { get; set; }
        public bool? IsAutoWidth { get; set; }
        public bool? IsShowButtonExpand { get; set; }
        public HisBloodTypeInStock_NodeCellStyle HisBloodTypeInStockNodeCellStyle { get; set; }
        public HisBloodTypeInStockHandler HisBloodTypeInStockClick { get; set; }
        public HisBloodTypeInStockHandler HisBloodTypeInStockDoubleClick { get; set; }
        public HisBloodTypeInStockHandler HisBloodTypeInStockRowEnter { get; set; }
        public HisBloodTypeInStock_GetStateImage HisBloodTypeInStock_GetStateImage { get; set; }
        public HisBloodTypeInStock_GetSelectImage HisBloodTypeInStock_GetSelectImage { get; set; }
        public HisBloodTypeInStockHandler HisBloodTypeInStock_StateImageClick { get; set; }
        public HisBloodTypeInStockHandler HisBloodTypeInStock_SelectImageClick { get; set; }
        public HisBloodTypeInStock_CustomUnboundColumnData HisBloodTypeInStock_CustomUnboundColumnData { get; set; }
        public HisBloodTypeInStock_AfterCheck HisBloodTypeInStock_AfterCheck { get; set; }
        public HisBloodTypeInStock_BeforeCheck HisBloodTypeInStock_BeforeCheck { get; set; }
        public HisBloodTypeInStock_CheckAllNode HisBloodTypeInStock_CheckAllNode { get; set; }
        public HisBloodTypeInStock_CustomDrawNodeCell HisBloodTypeInStock_CustomDrawNodeCell { get; set; }
        public DevExpress.Utils.ImageCollection StateImageCollection { get; set; }
        public DevExpress.Utils.ImageCollection SelectImageCollection { get; set; }

        public HisBloodTypeInStockHandler UpdateSingleRow { get; set; }
        public MenuItems MenuItems { get; set; }

        public bool? IsCreateParentNodeWithHisBloodTypeInStockExpend { get; set; }

        public string LayoutHisBloodTypeInStockExpend { get; set; }
        public string Keyword_NullValuePrompt { get; set; }
        public string KeyFieldName { get; set; }
        public string ParentFieldName { get; set; }
        public string NameButtonClose { get; set; }
        public string NameButtonOpen { get; set; }
    }
}
