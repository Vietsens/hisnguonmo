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

namespace HIS.UC.MaterialTypeInStock
{
    public class MaterialTypeInStockInitADO
    {
        public List<ColumnButtonEditADO> ColumnButtonEdits { get; set; }
        public List<MaterialTypeInStockColumn> MaterialTypeInStockColumns { get; set; }
        public List<HisMaterialTypeInStockSDO> MaterialTypeInStocks { get; set; }      

        public bool? IsShowSearchPanel { get; set; }
        public bool? IsShowButtonAdd { get; set; }
        public bool? IsShowCheckNode { get; set; }
        public bool? IsAutoWidth { get; set; }
        public MaterialTypeInStock_NodeCellStyle MaterialTypeInStockNodeCellStyle { get; set; }
        public MaterialTypeInStockHandler MaterialTypeInStockClick { get; set; }
        public MaterialTypeInStockHandler MaterialTypeInStockDoubleClick { get; set; }
        public MaterialTypeInStockHandler MaterialTypeInStockRowEnter { get; set; }
        public MaterialTypeInStock_GetStateImage MaterialTypeInStock_GetStateImage { get; set; }
        public MaterialTypeInStock_GetSelectImage MaterialTypeInStock_GetSelectImage { get; set; }
        public MaterialTypeInStockHandler MaterialTypeInStock_StateImageClick { get; set; }
        public MaterialTypeInStockHandler MaterialTypeInStock_SelectImageClick { get; set; }
        public MaterialTypeInStock_CustomUnboundColumnData MaterialTypeInStock_CustomUnboundColumnData { get; set; }
        public MaterialTypeInStock_AfterCheck MaterialTypeInStock_AfterCheck { get; set; }
        public MaterialTypeInStock_BeforeCheck MaterialTypeInStock_BeforeCheck { get; set; }
        public MaterialTypeInStock_CheckAllNode MaterialTypeInStock_CheckAllNode { get; set; }
        public MaterialTypeInStock_CustomDrawNodeCell MaterialTypeInStock_CustomDrawNodeCell { get; set; }
        public MaterialTypeInStock_NewClick MaterialTypeInStock_NewClick { get; set; }
        public MaterialTypeInStock_RefeshData MaterialTypeInStock_RefeshData { get; set; }
        public DevExpress.Utils.ImageCollection StateImageCollection { get; set; }
        public DevExpress.Utils.ImageCollection SelectImageCollection { get; set; }

        public MaterialTypeInStockHandler UpdateSingleRow { get; set; }
        public MenuItems MenuItems { get; set; }

        public bool? IsCreateParentNodeWithMaterialTypeInStockExpend { get; set; }

        public string LayoutMaterialTypeInStockExpend { get; set; }
        public string Keyword_NullValuePrompt { get; set; }
        public string KeyFieldName { get; set; }
        public string ParentFieldName { get; set; }
    }
}
