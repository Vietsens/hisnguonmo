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
using HIS.UC.MedicineType.ADO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.UC.MedicineType
{
    public delegate void MedicineType_NodeCellStyle(object data, DevExpress.Utils.AppearanceObject appearanceObject);
    public delegate void MedicineTypeHandler(MedicineTypeADO data);
    public delegate void MedicineType_GetStateImage(MedicineTypeADO data, DevExpress.XtraTreeList.GetStateImageEventArgs e);
    public delegate void MedicineType_GetSelectImage(MedicineTypeADO data, DevExpress.XtraTreeList.GetSelectImageEventArgs e);
    public delegate void MedicineType_CustomUnboundColumnData(MedicineTypeADO data, DevExpress.XtraTreeList.TreeListCustomColumnDataEventArgs e);
    public delegate void MedicineType_AfterCheck(TreeListNode node, MedicineTypeADO data);
    public delegate void MedicineType_BeforeCheck(TreeListNode node);
    public delegate void MedicineType_CheckAllNode(TreeListNodes node);
    public delegate void MedicineType_CustomDrawNodeCell(MedicineTypeADO data,DevExpress.XtraTreeList.CustomDrawNodeCellEventArgs e);
    public delegate void CheckThieuThongTinBHYT_CheckChange(CheckState checkStateAll, CheckState checkStateDuThongTinBHYT, CheckState checkStateThieuThongTinBHYT);
    public delegate void BtnExportExcel(CheckState btnExportExcel);

    public delegate List<DevExpress.Utils.Menu.DXMenuItem> MenuItems(MedicineTypeADO data);
    public delegate void MedicineType_ExportExcel();
    public delegate void MedicineType_Import();
    public delegate void MedicineType_Save();

}
