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
using HIS.UC.HisMediInStockByExpireDate.ADO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.HisMediInStockByExpireDate
{
    public delegate void HisMediInStockByExpireDate_NodeCellStyle(object data, DevExpress.Utils.AppearanceObject appearanceObject);
    public delegate void HisMediInStockByExpireDateHandler(HisMediInStockByExpireDateADO data);
    public delegate void HisMediInStockByExpireDate_GetStateImage(HisMediInStockByExpireDateADO data, DevExpress.XtraTreeList.GetStateImageEventArgs e);
    public delegate void HisMediInStockByExpireDate_GetSelectImage(HisMediInStockByExpireDateADO data, DevExpress.XtraTreeList.GetSelectImageEventArgs e);
    public delegate void HisMediInStockByExpireDate_CustomUnboundColumnData(HisMediInStockByExpireDateADO data, DevExpress.XtraTreeList.TreeListCustomColumnDataEventArgs e);
    public delegate void HisMediInStockByExpireDate_AfterCheck(TreeListNode node, HisMediInStockByExpireDateADO data);
    public delegate void HisMediInStockByExpireDate_BeforeCheck(TreeListNode node);
    public delegate void HisMediInStockByExpireDate_CheckAllNode(TreeListNodes node);
    public delegate void HisMediInStockByExpireDate_CustomDrawNodeCell(HisMediInStockByExpireDateADO data,DevExpress.XtraTreeList.CustomDrawNodeCellEventArgs e);
    public delegate List<DevExpress.Utils.Menu.DXMenuItem> MenuItems(HisMediInStockByExpireDateADO data);
}
