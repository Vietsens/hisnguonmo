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

namespace HIS.UC.BloodType
{
    public class BloodTypeInitADO
    {
        public List<ColumnButtonEditADO> ColumnButtonEdits { get; set; }
        public List<BloodTypeColumn> BloodTypeColumns { get; set; }
        public List<MOS.EFMODEL.DataModels.V_HIS_BLOOD_TYPE> BloodTypes { get; set; }      

        public bool? IsShowSearchPanel { get; set; }
        public bool? IsShowButtonAdd { get; set; }
        public bool? IsShowCheckNode { get; set; }
        public bool? IsAutoWidth { get; set; }
        public BloodType_NodeCellStyle BloodTypeNodeCellStyle { get; set; }
        public BloodTypeHandler BloodTypeClick { get; set; }
        public BloodTypeHandler BloodTypeDoubleClick { get; set; }
        public BloodTypeHandler BloodTypeRowEnter { get; set; }
        public BloodType_GetStateImage BloodType_GetStateImage { get; set; }
        public BloodType_GetSelectImage BloodType_GetSelectImage { get; set; }
        public BloodTypeHandler BloodType_StateImageClick { get; set; }
        public BloodTypeHandler BloodType_SelectImageClick { get; set; }
        public BloodType_CustomUnboundColumnData BloodType_CustomUnboundColumnData { get; set; }
        public BloodType_AfterCheck BloodType_AfterCheck { get; set; }
        public BloodType_BeforeCheck BloodType_BeforeCheck { get; set; }
        public BloodType_CheckAllNode BloodType_CheckAllNode { get; set; }
        public BloodType_CustomDrawNodeCell BloodType_CustomDrawNodeCell { get; set; }
        public DevExpress.Utils.ImageCollection StateImageCollection { get; set; }
        public DevExpress.Utils.ImageCollection SelectImageCollection { get; set; }
        public BloodTypeHandler BloodTypeRowClick { get; set; }

        public BloodTypeHandler UpdateSingleRow { get; set; }
        public MenuItems MenuItems { get; set; }

        public bool? IsCreateParentNodeWithBloodTypeExpend { get; set; }

        public string LayoutBloodTypeExpend { get; set; }

        public string Keyword_NullValuePrompt { get; set; }
        public string KeyFieldName { get; set; }
        public string ParentFieldName { get; set; }
    }
}
