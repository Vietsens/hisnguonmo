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
using DevExpress.XtraTreeList.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.TreeSereServ7V2
{
    public delegate void TreeSereServ7_NodeCellStyle(SereServADO data, DevExpress.XtraTreeList.GetCustomNodeCellStyleEventArgs e);
    public delegate void TreeSereServ7_CustomNodeCellEdit(SereServADO data, DevExpress.XtraTreeList.GetCustomNodeCellEditEventArgs e);
    public delegate void SereServHandler(SereServADO data);
    public delegate void TreeSereServ7_GetStateImage(SereServADO data, DevExpress.XtraTreeList.GetStateImageEventArgs e);
    public delegate void TreeSereServ7_GetSelectImage(SereServADO data, DevExpress.XtraTreeList.GetSelectImageEventArgs e);
    public delegate void TreeSereServ7_CustomUnboundColumnData(SereServADO data, DevExpress.XtraTreeList.TreeListCustomColumnDataEventArgs e);
    public delegate void TreeSereServ7_AfterCheck(TreeListNode node, SereServADO data);
    public delegate void TreeSereServ7_BeforeCheck(TreeListNode node);
    public delegate void TreeSereServ7_CheckAllNode(TreeListNodes node);
    public delegate void TreeSereServ7_CustomDrawNodeCell(SereServADO data,DevExpress.XtraTreeList.CustomDrawNodeCellEventArgs e);
    public delegate void TreeSereServ7_DoubleClick(SereServADO data);
    public delegate void TreeSereServ7_ReloadFilter();
    public delegate List<DevExpress.Utils.Menu.DXMenuItem> MenuItems(SereServADO data);
}
