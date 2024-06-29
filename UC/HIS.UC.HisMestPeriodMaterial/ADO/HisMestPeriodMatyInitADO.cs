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

namespace HIS.UC.HisMestPeriodMaterial
{
    public class HisMestPeriodMaterialInitADO
    {
        public List<ColumnButtonEditADO> ColumnButtonEdits { get; set; }
        public List<HisMestPeriodMaterialColumn> HisMestPeriodMaterialColumns { get; set; }
        public List<MOS.EFMODEL.DataModels.V_HIS_MEST_PERIOD_MATY> HisMestPeriodMaterials { get; set; }      

        public bool? IsShowSearchPanel { get; set; }
        public bool? IsShowButtonAdd { get; set; }
        public bool? IsShowCheckNode { get; set; }
        public HisMestPeriodMaterial_NodeCellStyle HisMestPeriodMaterialNodeCellStyle { get; set; }
        public HisMestPeriodMaterialHandler HisMestPeriodMaterialClick { get; set; }
        public HisMestPeriodMaterialHandler HisMestPeriodMaterialDoubleClick { get; set; }
        public HisMestPeriodMaterialHandler HisMestPeriodMaterialRowEnter { get; set; }
        public HisMestPeriodMaterial_GetStateImage HisMestPeriodMaterial_GetStateImage { get; set; }
        public HisMestPeriodMaterial_GetSelectImage HisMestPeriodMaterial_GetSelectImage { get; set; }
        public HisMestPeriodMaterialHandler HisMestPeriodMaterial_StateImageClick { get; set; }
        public HisMestPeriodMaterialHandler HisMestPeriodMaterial_SelectImageClick { get; set; }
        public HisMestPeriodMaterial_CustomUnboundColumnData HisMestPeriodMaterial_CustomUnboundColumnData { get; set; }
        public HisMestPeriodMaterial_AfterCheck HisMestPeriodMaterial_AfterCheck { get; set; }
        public HisMestPeriodMaterial_BeforeCheck HisMestPeriodMaterial_BeforeCheck { get; set; }
        public HisMestPeriodMaterial_CheckAllNode HisMestPeriodMaterial_CheckAllNode { get; set; }
        public HisMestPeriodMaterial_CustomDrawNodeCell HisMestPeriodMaterial_CustomDrawNodeCell { get; set; }
        public DevExpress.Utils.ImageCollection StateImageCollection { get; set; }
        public DevExpress.Utils.ImageCollection SelectImageCollection { get; set; }

        public HisMestPeriodMaterialHandler UpdateSingleRow { get; set; }
        public MenuItems MenuItems { get; set; }

        public bool? IsCreateParentNodeWithHisMestPeriodMaterialExpend { get; set; }

        public string LayoutHisMestPeriodMaterialExpend { get; set; }
    }
}
