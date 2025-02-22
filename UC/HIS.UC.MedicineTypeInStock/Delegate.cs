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
using HIS.UC.MedicineTypeInStock.ADO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.MedicineTypeInStock
{
    public delegate void MedicineTypeInStock_NodeCellStyle(object data, DevExpress.Utils.AppearanceObject appearanceObject);
    public delegate void MedicineTypeInStockHandler(MedicineTypeInStockADO data);
    public delegate void MedicineTypeInStock_GetStateImage(MedicineTypeInStockADO data, DevExpress.XtraTreeList.GetStateImageEventArgs e);
    public delegate void MedicineTypeInStock_GetSelectImage(MedicineTypeInStockADO data, DevExpress.XtraTreeList.GetSelectImageEventArgs e);
    public delegate void MedicineTypeInStock_CustomUnboundColumnData(MedicineTypeInStockADO data, DevExpress.XtraTreeList.TreeListCustomColumnDataEventArgs e);
    public delegate void MedicineTypeInStock_AfterCheck(TreeListNode node, MedicineTypeInStockADO data);
    public delegate void MedicineTypeInStock_BeforeCheck(TreeListNode node);
    public delegate void MedicineTypeInStock_CheckAllNode(TreeListNodes node);
    public delegate void MedicineTypeInStock_CustomDrawNodeCell(MedicineTypeInStockADO data,DevExpress.XtraTreeList.CustomDrawNodeCellEventArgs e);
    public delegate List<DevExpress.Utils.Menu.DXMenuItem> MenuItems(MedicineTypeInStockADO data);
}
