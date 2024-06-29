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
using HIS.UC.MaterialType.ADO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.UC.MaterialType
{
    public delegate void MaterialType_NodeCellStyle(object data, DevExpress.Utils.AppearanceObject appearanceObject);
    public delegate void MaterialTypeHandler(MaterialTypeADO data);
    public delegate void MaterialType_GetStateImage(MaterialTypeADO data, DevExpress.XtraTreeList.GetStateImageEventArgs e);
    public delegate void MaterialType_GetSelectImage(MaterialTypeADO data, DevExpress.XtraTreeList.GetSelectImageEventArgs e);
    public delegate void MaterialType_CustomUnboundColumnData(MaterialTypeADO data, DevExpress.XtraTreeList.TreeListCustomColumnDataEventArgs e);
    public delegate void MaterialType_AfterCheck(TreeListNode node, MaterialTypeADO data);
    public delegate void MaterialType_BeforeCheck(TreeListNode node);
    public delegate void MaterialType_CheckAllNode(TreeListNodes node);
    public delegate void MaterialType_CustomDrawNodeCell(MaterialTypeADO data,DevExpress.XtraTreeList.CustomDrawNodeCellEventArgs e);
    public delegate List<DevExpress.Utils.Menu.DXMenuItem> MenuItems(MaterialTypeADO data);
    public delegate void MaterialType_RefeshData(List<MOS.EFMODEL.DataModels.V_HIS_MATERIAL_TYPE> data);
    public delegate void MaterialType_NewClick(MaterialType_RefeshData refesh);
    public delegate void MaterialType_ExportExcel();
    public delegate void MaterialType_Import();
    public delegate void MaterialType_Save();
    public delegate void cboBid_EditValueChanged(long? data);
    public delegate void ChkLock_CheckChange(CheckState checkState);
    public delegate void MaterialType_PrintPriceList(MaterialTypeADO data);
    public delegate void cboContract_EditValueChanged(long? data);
    public delegate void cboIsReusable_EditValueChanged(int? data);
}
