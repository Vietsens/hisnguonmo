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

namespace HIS.UC.MedicineTypeInStock
{
    public class MedicineTypeInStockInitADO
    {
        public List<ColumnButtonEditADO> ColumnButtonEdits { get; set; }
        public List<MedicineTypeInStockColumn> MedicineTypeInStockColumns { get; set; }
        public List<MOS.SDO.HisMedicineTypeInStockSDO> MedicineTypeInStocks { get; set; }      

        public bool? IsShowSearchPanel { get; set; }
        public bool? IsShowButtonAdd { get; set; }
        public bool? IsShowCheckNode { get; set; }
        public bool? IsAutoWidth { get; set; }
        public MedicineTypeInStock_NodeCellStyle MedicineTypeInStockNodeCellStyle { get; set; }
        public MedicineTypeInStockHandler MedicineTypeInStockClick { get; set; }
        public MedicineTypeInStockHandler MedicineTypeInStockDoubleClick { get; set; }
        public MedicineTypeInStockHandler MedicineTypeInStockRowEnter { get; set; }
        public MedicineTypeInStock_GetStateImage MedicineTypeInStock_GetStateImage { get; set; }
        public MedicineTypeInStock_GetSelectImage MedicineTypeInStock_GetSelectImage { get; set; }
        public MedicineTypeInStockHandler MedicineTypeInStock_StateImageClick { get; set; }
        public MedicineTypeInStockHandler MedicineTypeInStock_SelectImageClick { get; set; }
        public MedicineTypeInStock_CustomUnboundColumnData MedicineTypeInStock_CustomUnboundColumnData { get; set; }
        public MedicineTypeInStock_AfterCheck MedicineTypeInStock_AfterCheck { get; set; }
        public MedicineTypeInStock_BeforeCheck MedicineTypeInStock_BeforeCheck { get; set; }
        public MedicineTypeInStock_CheckAllNode MedicineTypeInStock_CheckAllNode { get; set; }
        public MedicineTypeInStock_CustomDrawNodeCell MedicineTypeInStock_CustomDrawNodeCell { get; set; }
        public DevExpress.Utils.ImageCollection StateImageCollection { get; set; }
        public DevExpress.Utils.ImageCollection SelectImageCollection { get; set; }

        public MedicineTypeInStockHandler UpdateSingleRow { get; set; }
        public MenuItems MenuItems { get; set; }

        public bool? IsCreateParentNodeWithMedicineTypeInStockExpend { get; set; }

        public string LayoutMedicineTypeInStockExpend { get; set; }
        public string Keyword_NullValuePrompt { get; set; }
        public string KeyFieldName { get; set; }
        public string ParentFieldName { get; set; }
    }
}
