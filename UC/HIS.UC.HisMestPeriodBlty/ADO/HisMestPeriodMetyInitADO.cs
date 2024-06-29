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

namespace HIS.UC.HisMestPeriodBlty
{
    public class HisMestPeriodBltyInitADO
    {
        public List<ColumnButtonEditADO> ColumnButtonEdits { get; set; }
        public List<HisMestPeriodBltyColumn> HisMestPeriodBltyColumns { get; set; }
        public List<MOS.EFMODEL.DataModels.V_HIS_MEST_PERIOD_BLTY> HisMestPeriodBltys { get; set; }      

        public bool? IsShowSearchPanel { get; set; }
        public bool? IsShowButtonAdd { get; set; }
        public bool? IsShowCheckNode { get; set; }
        public HisMestPeriodBlty_NodeCellStyle HisMestPeriodBltyNodeCellStyle { get; set; }
        public HisMestPeriodBltyHandler HisMestPeriodBltyClick { get; set; }
        public HisMestPeriodBltyHandler HisMestPeriodBltyDoubleClick { get; set; }
        public HisMestPeriodBltyHandler HisMestPeriodBltyRowEnter { get; set; }
        public HisMestPeriodBlty_GetStateImage HisMestPeriodBlty_GetStateImage { get; set; }
        public HisMestPeriodBlty_GetSelectImage HisMestPeriodBlty_GetSelectImage { get; set; }
        public HisMestPeriodBltyHandler HisMestPeriodBlty_StateImageClick { get; set; }
        public HisMestPeriodBltyHandler HisMestPeriodBlty_SelectImageClick { get; set; }
        public HisMestPeriodBlty_CustomUnboundColumnData HisMestPeriodBlty_CustomUnboundColumnData { get; set; }
        public HisMestPeriodBlty_AfterCheck HisMestPeriodBlty_AfterCheck { get; set; }
        public HisMestPeriodBlty_BeforeCheck HisMestPeriodBlty_BeforeCheck { get; set; }
        public HisMestPeriodBlty_CheckAllNode HisMestPeriodBlty_CheckAllNode { get; set; }
        public HisMestPeriodBlty_CustomDrawNodeCell HisMestPeriodBlty_CustomDrawNodeCell { get; set; }
        public DevExpress.Utils.ImageCollection StateImageCollection { get; set; }
        public DevExpress.Utils.ImageCollection SelectImageCollection { get; set; }

        public HisMestPeriodBltyHandler UpdateSingleRow { get; set; }
        public MenuItems MenuItems { get; set; }

        public bool? IsCreateParentNodeWithHisMestPeriodBltyExpend { get; set; }

        public string LayoutHisMestPeriodBltyExpend { get; set; }
    }
}
