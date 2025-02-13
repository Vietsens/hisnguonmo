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

namespace HIS.UC.HisMateInStockByExpireDate
{
    public class HisMateInStockByExpireDateInitADO
    {
        public List<ColumnButtonEditADO> ColumnButtonEdits { get; set; }
        public List<HisMateInStockByExpireDateColumn> HisMateInStockByExpireDateColumns { get; set; }
        public List<List<MOS.SDO.HisMaterialInStockSDO>> HisMateInStockByExpireDates { get; set; }      

        public bool? IsShowSearchPanel { get; set; }
        public bool? IsShowButtonAdd { get; set; }
        public bool? IsShowCheckNode { get; set; }
        public HisMateInStockByExpireDate_NodeCellStyle HisMateInStockByExpireDateNodeCellStyle { get; set; }
        public HisMateInStockByExpireDateHandler HisMateInStockByExpireDateClick { get; set; }
        public HisMateInStockByExpireDateHandler HisMateInStockByExpireDateDoubleClick { get; set; }
        public HisMateInStockByExpireDateHandler HisMateInStockByExpireDateRowEnter { get; set; }
        public HisMateInStockByExpireDate_GetStateImage HisMateInStockByExpireDate_GetStateImage { get; set; }
        public HisMateInStockByExpireDate_GetSelectImage HisMateInStockByExpireDate_GetSelectImage { get; set; }
        public HisMateInStockByExpireDateHandler HisMateInStockByExpireDate_StateImageClick { get; set; }
        public HisMateInStockByExpireDateHandler HisMateInStockByExpireDate_SelectImageClick { get; set; }
        public HisMateInStockByExpireDate_CustomUnboundColumnData HisMateInStockByExpireDate_CustomUnboundColumnData { get; set; }
        public HisMateInStockByExpireDate_AfterCheck HisMateInStockByExpireDate_AfterCheck { get; set; }
        public HisMateInStockByExpireDate_BeforeCheck HisMateInStockByExpireDate_BeforeCheck { get; set; }
        public HisMateInStockByExpireDate_CheckAllNode HisMateInStockByExpireDate_CheckAllNode { get; set; }
        public HisMateInStockByExpireDate_CustomDrawNodeCell HisMateInStockByExpireDate_CustomDrawNodeCell { get; set; }
        public DevExpress.Utils.ImageCollection StateImageCollection { get; set; }
        public DevExpress.Utils.ImageCollection SelectImageCollection { get; set; }

        public HisMateInStockByExpireDateHandler UpdateSingleRow { get; set; }
        public MenuItems MenuItems { get; set; }

        public bool? IsCreateParentNodeWithHisMateInStockByExpireDateExpend { get; set; }

        public string LayoutHisMateInStockByExpireDateExpend { get; set; }
    }
}
