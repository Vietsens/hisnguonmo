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
using HIS.UC.HisMedicineInStock.ADO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.HisMedicineInStock
{
    public delegate void HisMedicineInStock_NodeCellStyle(HisMedicineInStockADO data, DevExpress.XtraTreeList.GetCustomNodeCellStyleEventArgs e);
    public delegate void HisMedicineInStockHandler(HisMedicineInStockADO data);
    public delegate void HisMedicineInStock_GetStateImage(HisMedicineInStockADO data, DevExpress.XtraTreeList.GetStateImageEventArgs e);
    public delegate void HisMedicineInStock_GetSelectImage(HisMedicineInStockADO data, DevExpress.XtraTreeList.GetSelectImageEventArgs e);
    public delegate void HisMedicineInStock_CustomUnboundColumnData(HisMedicineInStockADO data, DevExpress.XtraTreeList.TreeListCustomColumnDataEventArgs e);
    public delegate void HisMedicineInStock_AfterCheck(TreeListNode node, HisMedicineInStockADO data);
    public delegate void HisMedicineInStock_BeforeCheck(TreeListNode node);
    public delegate void HisMedicineInStock_CheckAllNode(TreeListNodes node);
    public delegate void HisMedicineInStock_CustomDrawNodeCell(HisMedicineInStockADO data,DevExpress.XtraTreeList.CustomDrawNodeCellEventArgs e);

    public delegate void btnLock_buttonClick(HisMedicineInStockADO data);
    public delegate void btnUnLock_buttonClick(HisMedicineInStockADO data);

    public delegate List<DevExpress.Utils.Menu.DXMenuItem> MenuItems(HisMedicineInStockADO data, TreeListNode node);
}
