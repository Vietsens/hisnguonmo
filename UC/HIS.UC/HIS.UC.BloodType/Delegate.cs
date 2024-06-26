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
using HIS.UC.BloodType.ADO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.BloodType
{
    public delegate void BloodType_NodeCellStyle(object data, DevExpress.Utils.AppearanceObject appearanceObject);
    public delegate void BloodTypeHandler(BloodTypeADO data);
    public delegate void BloodType_GetStateImage(BloodTypeADO data, DevExpress.XtraTreeList.GetStateImageEventArgs e);
    public delegate void BloodType_GetSelectImage(BloodTypeADO data, DevExpress.XtraTreeList.GetSelectImageEventArgs e);
    public delegate void BloodType_CustomUnboundColumnData(BloodTypeADO data, DevExpress.XtraTreeList.TreeListCustomColumnDataEventArgs e);
    public delegate void BloodType_AfterCheck(TreeListNode node, BloodTypeADO data);
    public delegate void BloodType_BeforeCheck(TreeListNode node);
    public delegate void BloodType_CheckAllNode(TreeListNodes node);
    public delegate void BloodType_CustomDrawNodeCell(BloodTypeADO data,DevExpress.XtraTreeList.CustomDrawNodeCellEventArgs e);
    public delegate List<DevExpress.Utils.Menu.DXMenuItem> MenuItems(BloodTypeADO data);
}
