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

namespace HIS.UC.TreeSereServ7V2
{
    public class TreeSereServ7ADO
    {
        public List<ColumnButtonEditADO> ColumnButtonEdits { get; set; }
        public List<TreeSereServ7V2Column> TreeSereServ7Columns { get; set; }
        public List<SereServADO> SereServs { get; set; }      

        public bool? IsShowSearchPanel { get; set; }
        public bool? IsShowCheckNode { get; set; }
        public bool? IsAutoWidth { get; set; }
        public TreeSereServ7_NodeCellStyle SereServNodeCellStyle { get; set; }
        public SereServHandler TreeSereServ7Click { get; set; }
        public TreeSereServ7_GetStateImage TreeSereServ7_GetStateImage { get; set; }
        public TreeSereServ7_GetSelectImage TreeSereServ7_GetSelectImage { get; set; }
        public TreeSereServ7_ReloadFilter TreeSereServ7_ReloadFilter { get; set; }
        public SereServHandler TreeSereServ7_StateImageClick { get; set; }
        public SereServHandler TreeSereServ7_DoubleClick { get; set; }
        public SereServHandler TreeSereServ7_SelectImageClick { get; set; }
        public TreeSereServ7_CustomUnboundColumnData TreeSereServ7_CustomUnboundColumnData { get; set; }
        public TreeSereServ7_CustomNodeCellEdit TreeSereServ7_CustomNodeCellEdit { get; set; }
        public TreeSereServ7_AfterCheck TreeSereServ7_AfterCheck { get; set; }
        public TreeSereServ7_BeforeCheck TreeSereServ7_BeforeCheck { get; set; }
        public TreeSereServ7_CheckAllNode TreeSereServ7_CheckAllNode { get; set; }
        public TreeSereServ7_CustomDrawNodeCell TreeSereServ7_CustomDrawNodeCell { get; set; }
        public DevExpress.Utils.ImageCollection StateImageCollection { get; set; }
        public DevExpress.Utils.ImageCollection SelectImageCollection { get; set; }

        public SereServHandler UpdateSingleRow { get; set; }
        public MenuItems MenuItems { get; set; }

        public bool? IsCreateParentNodeWithSereServExpend { get; set; }

        public string LayoutSereServExpend { get; set; }
        public string KeyFieldName { get; set; }
        public string ParentFieldName { get; set; }
        public string Keyword_NullValuePrompt { get; set; }
        public long? DepartmentID { get; set; }
        public bool IsNotGroupServiceType { get; set; }
    }
}
