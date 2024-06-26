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

namespace HIS.UC.HisMediInStockByExpireDate
{
    public class HisMediInStockByExpireDateInitADO
    {
        public List<ColumnButtonEditADO> ColumnButtonEdits { get; set; }
        public List<HisMediInStockByExpireDateColumn> HisMediInStockByExpireDateColumns { get; set; }
        public List<List<MOS.SDO.HisMedicineInStockSDO>> HisMediInStockByExpireDates { get; set; }      

        public bool? IsShowSearchPanel { get; set; }
        public bool? IsShowButtonAdd { get; set; }
        public bool? IsShowCheckNode { get; set; }
        public HisMediInStockByExpireDate_NodeCellStyle HisMediInStockByExpireDateNodeCellStyle { get; set; }
        public HisMediInStockByExpireDateHandler HisMediInStockByExpireDateClick { get; set; }
        public HisMediInStockByExpireDateHandler HisMediInStockByExpireDateDoubleClick { get; set; }
        public HisMediInStockByExpireDateHandler HisMediInStockByExpireDateRowEnter { get; set; }
        public HisMediInStockByExpireDate_GetStateImage HisMediInStockByExpireDate_GetStateImage { get; set; }
        public HisMediInStockByExpireDate_GetSelectImage HisMediInStockByExpireDate_GetSelectImage { get; set; }
        public HisMediInStockByExpireDateHandler HisMediInStockByExpireDate_StateImageClick { get; set; }
        public HisMediInStockByExpireDateHandler HisMediInStockByExpireDate_SelectImageClick { get; set; }
        public HisMediInStockByExpireDate_CustomUnboundColumnData HisMediInStockByExpireDate_CustomUnboundColumnData { get; set; }
        public HisMediInStockByExpireDate_AfterCheck HisMediInStockByExpireDate_AfterCheck { get; set; }
        public HisMediInStockByExpireDate_BeforeCheck HisMediInStockByExpireDate_BeforeCheck { get; set; }
        public HisMediInStockByExpireDate_CheckAllNode HisMediInStockByExpireDate_CheckAllNode { get; set; }
        public HisMediInStockByExpireDate_CustomDrawNodeCell HisMediInStockByExpireDate_CustomDrawNodeCell { get; set; }
        public DevExpress.Utils.ImageCollection StateImageCollection { get; set; }
        public DevExpress.Utils.ImageCollection SelectImageCollection { get; set; }

        public HisMediInStockByExpireDateHandler UpdateSingleRow { get; set; }
        public MenuItems MenuItems { get; set; }

        public bool? IsCreateParentNodeWithHisMediInStockByExpireDateExpend { get; set; }

        public string LayoutHisMediInStockByExpireDateExpend { get; set; }
    }
}
