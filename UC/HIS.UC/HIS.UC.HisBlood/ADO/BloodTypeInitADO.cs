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

namespace HIS.UC.HisBlood
{
    public class HisBloodInitADO
    {
        public List<ColumnButtonEditADO> ColumnButtonEdits { get; set; }
        public List<HisBloodColumn> HisBloodColumns { get; set; }
        public List<MOS.EFMODEL.DataModels.V_HIS_BLOOD> HisBloods { get; set; }      

        public bool? IsShowSearchPanel { get; set; }
        public bool? IsShowButtonAdd { get; set; }
        public bool? IsShowCheckNode { get; set; }
        public HisBlood_NodeCellStyle HisBloodNodeCellStyle { get; set; }
        public HisBloodHandler HisBloodClick { get; set; }
        public HisBloodHandler HisBloodDoubleClick { get; set; }
        public HisBloodHandler HisBloodRowEnter { get; set; }
        public HisBlood_GetStateImage HisBlood_GetStateImage { get; set; }
        public HisBlood_GetSelectImage HisBlood_GetSelectImage { get; set; }
        public HisBloodHandler HisBlood_StateImageClick { get; set; }
        public HisBloodHandler HisBlood_SelectImageClick { get; set; }
        public HisBlood_CustomUnboundColumnData HisBlood_CustomUnboundColumnData { get; set; }
        public HisBlood_AfterCheck HisBlood_AfterCheck { get; set; }
        public HisBlood_BeforeCheck HisBlood_BeforeCheck { get; set; }
        public HisBlood_CheckAllNode HisBlood_CheckAllNode { get; set; }
        public HisBlood_CustomDrawNodeCell HisBlood_CustomDrawNodeCell { get; set; }
        public DevExpress.Utils.ImageCollection StateImageCollection { get; set; }
        public DevExpress.Utils.ImageCollection SelectImageCollection { get; set; }

        public HisBloodHandler UpdateSingleRow { get; set; }
        public MenuItems MenuItems { get; set; }

        public bool? IsCreateParentNodeWithHisBloodExpend { get; set; }

        public string LayoutHisBloodExpend { get; set; }
    }
}
