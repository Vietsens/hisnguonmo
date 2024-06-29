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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MOS.EFMODEL.DataModels;
using Inventec.Core;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraTreeList.Data;
using System.Collections;
using DevExpress.XtraTreeList;
using DevExpress.Utils.Menu;
using DevExpress.XtraTreeList.Nodes;
using HIS.Desktop.LocalStorage.BackendData;
using System.Drawing.Drawing2D;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;

namespace HIS.UC.TreeSereServ7.Run
{
    public partial class UCTreeSereServ7 : UserControl
    {
        internal void DisposeControl()
        {
            try
            {
                TreeSereServ7ADO = null;
                records = null;
                SereServADOs = null;
                columnButtonEdits = null;
                sereServNodeCellStyle = null;
                sereServTree_CustomNodeCellEdit = null;
                sereServTree_CustomUnboundColumnData = null;
                sereServTreeClick = null;
                sereServTree_GetStateImage = null;
                sereServTree_GetSelectImage = null;
                sereServTree_StateImageClick = null;
                sereServTree_DoubleClick = null;
                sereServTree_SelectImageClick = null;
                sereServTree_AfterCheck = null;
                sereServTree_BeforeCheck = null;
                sereServTree_CheckAllNode = null;
                sereServTree_CustomDrawNodeCell = null;
                IsShowCheckNode = false;
                isAutoWidth = false;
                selectImageCollection = null;
                stateImageCollection = null;
                updateSingleRow = null;
                menuItems = null;
                isCreateParentNodeWithSereServExpend = false;
                _DepartmentInput = 0;
                this.trvService.GetStateImage -= new DevExpress.XtraTreeList.GetStateImageEventHandler(this.trvService_GetStateImage);
                this.trvService.GetSelectImage -= new DevExpress.XtraTreeList.GetSelectImageEventHandler(this.trvService_GetSelectImage);
                this.trvService.StateImageClick -= new DevExpress.XtraTreeList.NodeClickEventHandler(this.trvService_StateImageClick);
                this.trvService.SelectImageClick -= new DevExpress.XtraTreeList.NodeClickEventHandler(this.trvService_SelectImageClick);
                this.trvService.CustomNodeCellEdit -= new DevExpress.XtraTreeList.GetCustomNodeCellEditEventHandler(this.trvService_CustomNodeCellEdit);
                this.trvService.NodeCellStyle -= new DevExpress.XtraTreeList.GetCustomNodeCellStyleEventHandler(this.trvService_NodeCellStyle);
                this.trvService.CustomUnboundColumnData -= new DevExpress.XtraTreeList.CustomColumnDataEventHandler(this.trvService_CustomUnboundColumnData);
                this.trvService.BeforeCheckNode -= new DevExpress.XtraTreeList.CheckNodeEventHandler(this.trvService_BeforeCheckNode);
                this.trvService.AfterCheckNode -= new DevExpress.XtraTreeList.NodeEventHandler(this.trvService_AfterCheckNode);
                this.trvService.CustomDrawNodeCell -= new DevExpress.XtraTreeList.CustomDrawNodeCellEventHandler(this.trvService_CustomDrawNodeCell);
                this.trvService.PopupMenuShowing -= new DevExpress.XtraTreeList.PopupMenuShowingEventHandler(this.trvService_PopupMenuShowing);
                this.trvService.Click -= new System.EventHandler(this.trvService_Click);
                this.trvService.DoubleClick -= new System.EventHandler(this.trvService_DoubleClick);
                this.trvService.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.trvService_KeyDown);
                this.repositoryItemchkIsExpend__Enable.CheckedChanged -= new System.EventHandler(this.repositoryItemchkIsExpend__Enable_CheckedChanged);
                this.cboFilterByDepartment.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboFilterByDepartment_Closed);
                this.btnThuGon.Click -= new System.EventHandler(this.btnThuGon_Click);
                this.btnChiTiet.Click -= new System.EventHandler(this.btnChiTiet_Click);
                this.txtKeyword.KeyUp -= new System.Windows.Forms.KeyEventHandler(this.txtKeyword_KeyUp);
                this.toolTipController1.GetActiveObjectInfo -= new DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventHandler(this.toolTipController1_GetActiveObjectInfo);
                this.Load -= new System.EventHandler(this.UCServiceTree_Load);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
