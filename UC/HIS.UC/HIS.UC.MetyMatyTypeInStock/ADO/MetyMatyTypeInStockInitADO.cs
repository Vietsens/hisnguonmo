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

namespace HIS.UC.MetyMatyTypeInStock
{
    public class MetyMatyTypeInStockInitADO
    {
        public List<ColumnButtonEditADO> ColumnButtonEdits { get; set; }
        public List<MetyMatyTypeInStockColumn> MetyMatyTypeInStockColumns { get; set; }
        public List<MOS.EFMODEL.DataModels.D_HIS_MEDI_STOCK_1> MetyMatyTypeInStocks { get; set; }      

        public bool? IsShowSearchPanel { get; set; }
        public bool? IsShowButtonAdd { get; set; }
        public bool? IsShowCheckNode { get; set; }
        public bool? IsAutoWidth { get; set; }
        public bool? IsEnableAppearanceFocusedCell { get; set; }
        public bool? IsEnableAppearanceFocusedRow { get; set; }
        public MetyMatyTypeInStock_NodeCellStyle MetyMatyTypeInStockNodeCellStyle { get; set; }
        public MetyMatyTypeInStockHandler MetyMatyTypeInStockClick { get; set; }
        public MetyMatyTypeInStockHandler MetyMatyTypeInStockDoubleClick { get; set; }
        public MetyMatyTypeInStockHandler MetyMatyTypeInStockRowEnter { get; set; }
        public MetyMatyTypeInStock_GetStateImage MetyMatyTypeInStock_GetStateImage { get; set; }
        public MetyMatyTypeInStock_GetSelectImage MetyMatyTypeInStock_GetSelectImage { get; set; }
        public MetyMatyTypeInStockHandler MetyMatyTypeInStock_StateImageClick { get; set; }
        public MetyMatyTypeInStockHandler MetyMatyTypeInStock_SelectImageClick { get; set; }
        public MetyMatyTypeInStock_CustomUnboundColumnData MetyMatyTypeInStock_CustomUnboundColumnData { get; set; }
        public MetyMatyTypeInStock_AfterCheck MetyMatyTypeInStock_AfterCheck { get; set; }
        public MetyMatyTypeInStock_BeforeCheck MetyMatyTypeInStock_BeforeCheck { get; set; }
        public MetyMatyTypeInStock_CheckAllNode MetyMatyTypeInStock_CheckAllNode { get; set; }
        public MetyMatyTypeInStock_CustomDrawNodeCell MetyMatyTypeInStock_CustomDrawNodeCell { get; set; }
        public DevExpress.Utils.ImageCollection StateImageCollection { get; set; }
        public DevExpress.Utils.ImageCollection SelectImageCollection { get; set; }

        public MetyMatyTypeInStockHandler UpdateSingleRow { get; set; }
        public MenuItems MenuItems { get; set; }

        public bool? IsCreateParentNodeWithMetyMatyTypeInStockExpend { get; set; }

        public string LayoutMetyMatyTypeInStockExpend { get; set; }
        public string Keyword_NullValuePrompt { get; set; }
        public string KeyFieldName { get; set; }
        public string ParentFieldName { get; set; }
        public bool? IsAutoSaveLayoutToRegistry { get; set; }
        public string KeySaveLayoutToRegistry { get; set; }
    }
}
