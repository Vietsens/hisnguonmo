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

namespace HIS.UC.HisMestPeriodMety
{
    public class HisMestPeriodMetyInitADO
    {
        public List<ColumnButtonEditADO> ColumnButtonEdits { get; set; }
        public List<HisMestPeriodMetyColumn> HisMestPeriodMetyColumns { get; set; }
        public List<MOS.EFMODEL.DataModels.V_HIS_MEST_PERIOD_METY> HisMestPeriodMetys { get; set; }      

        public bool? IsShowSearchPanel { get; set; }
        public bool? IsShowButtonAdd { get; set; }
        public bool? IsShowCheckNode { get; set; }
        public HisMestPeriodMety_NodeCellStyle HisMestPeriodMetyNodeCellStyle { get; set; }
        public HisMestPeriodMetyHandler HisMestPeriodMetyClick { get; set; }
        public HisMestPeriodMetyHandler HisMestPeriodMetyDoubleClick { get; set; }
        public HisMestPeriodMetyHandler HisMestPeriodMetyRowEnter { get; set; }
        public HisMestPeriodMety_GetStateImage HisMestPeriodMety_GetStateImage { get; set; }
        public HisMestPeriodMety_GetSelectImage HisMestPeriodMety_GetSelectImage { get; set; }
        public HisMestPeriodMetyHandler HisMestPeriodMety_StateImageClick { get; set; }
        public HisMestPeriodMetyHandler HisMestPeriodMety_SelectImageClick { get; set; }
        public HisMestPeriodMety_CustomUnboundColumnData HisMestPeriodMety_CustomUnboundColumnData { get; set; }
        public HisMestPeriodMety_AfterCheck HisMestPeriodMety_AfterCheck { get; set; }
        public HisMestPeriodMety_BeforeCheck HisMestPeriodMety_BeforeCheck { get; set; }
        public HisMestPeriodMety_CheckAllNode HisMestPeriodMety_CheckAllNode { get; set; }
        public HisMestPeriodMety_CustomDrawNodeCell HisMestPeriodMety_CustomDrawNodeCell { get; set; }
        public DevExpress.Utils.ImageCollection StateImageCollection { get; set; }
        public DevExpress.Utils.ImageCollection SelectImageCollection { get; set; }

        public HisMestPeriodMetyHandler UpdateSingleRow { get; set; }
        public MenuItems MenuItems { get; set; }

        public bool? IsCreateParentNodeWithHisMestPeriodMetyExpend { get; set; }

        public string LayoutHisMestPeriodMetyExpend { get; set; }
    }
}
