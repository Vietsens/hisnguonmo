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

namespace HIS.UC.SereServTree
{
    public class SereServTreeADO
    {
        public List<ColumnButtonEditADO> ColumnButtonEdits { get; set; }
        public List<SereServTreeColumn> SereServTreeColumns { get; set; }
        public List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_5> SereServs { get; set; }
        public List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_DEPOSIT> SereServDeposits { get; set; }

        public bool? IsShowSearchPanel { get; set; }
        public bool? IsShowCheckNode { get; set; }
        public bool? IsAutoWidth { get; set; }
        public SereServTree_NodeCellStyle SereServNodeCellStyle { get; set; }
        public SereServHandler SereServTreeClick { get; set; }
        public SereServTree_GetStateImage SereServTree_GetStateImage { get; set; }
        public SereServTree_GetSelectImage SereServTree_GetSelectImage { get; set; }
        public SereServHandler SereServTree_StateImageClick { get; set; }
        public SereServHandler SereServTree_SelectImageClick { get; set; }
        public SereServTree_CustomUnboundColumnData SereServTree_CustomUnboundColumnData { get; set; }
        public SereServTree_AfterCheck SereServTree_AfterCheck { get; set; }
        public SereServTree_BeforeCheck SereServTree_BeforeCheck { get; set; }
        public SereServTreeForBill_BeforeCheck SereServTreeForBill_BeforeCheck { get; set; }
        public SereServTree_ShowingEditor sereServTree_ShowingEditor { get; set; }
        public SereServTree_CheckAllNode SereServTree_CheckAllNode { get; set; }
        public SereServTree_CustomDrawNodeCell SereServTree_CustomDrawNodeCell { get; set; }
        public SereServTree_CustomDrawNodeCheckBox SereServTree_CustomDrawNodeCheckBox { get; set; }
        public DevExpress.Utils.ImageCollection StateImageCollection { get; set; }
        public DevExpress.Utils.ImageCollection SelectImageCollection { get; set; }

        public SereServHandler UpdateSingleRow { get; set; }
        public MenuItems MenuItems { get; set; }

        public bool? IsCreateParentNodeWithSereServExpend { get; set; }

        public string LayoutSereServExpend { get; set; }
        public string KeyFieldName { get; set; }
        public string ParentFieldName { get; set; }
        public string Keyword_NullValuePrompt { get; set; }
        public bool? isAdvance { get; set; }

        public bool IsShowForRegisterV2 { get; set; }
    }
}
