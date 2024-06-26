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
using DevExpress.Data;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utility;
using HIS.UC.TreeSereServ7;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using Inventec.UC.Paging;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.TreatmentHistory
{
    public partial class frmTreatmentHistory : HIS.Desktop.Utility.FormBase
    {
        public override void ProcessDisposeModuleDataAfterClose()
        {
            try
            {
                currentModule = null;
                currentInput = null;
                rowCount = 0;
                dataTotal = 0;
                pagingGrid = null;
                this.panelControlTreeSere7.Controls.Clear();
                treeSereServ7Processor.DisposeControl(ucSereServ);
                ucSereServ = null;
                treeSereServ7Processor = null;
                lastRowHandle = 0;
                lastColumn = null;
                lastInfo = null;
                rowCellClick = null;
                serviceReq2Focus = null;
                listServiceReq_CurrentTreatment = null;
                this.cboStatus.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboStatus_Closed);
                this.cboStatus.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboStatus_ButtonClick);
                this.barButton__F1.ItemClick -= new DevExpress.XtraBars.ItemClickEventHandler(this.barButton__F1_ItemClick);
                this.barButton__F2.ItemClick -= new DevExpress.XtraBars.ItemClickEventHandler(this.barButton__F2_ItemClick);
                this.barButton__F3.ItemClick -= new DevExpress.XtraBars.ItemClickEventHandler(this.barButton__F3_ItemClick);
                this.barButton__CrtlF.ItemClick -= new DevExpress.XtraBars.ItemClickEventHandler(this.barButton__CrtlF_ItemClick);
                this.btnSearch.Click -= new System.EventHandler(this.btnSearch_Click);
                this.tree_HisServiceReq2.FocusedNodeChanged -= new DevExpress.XtraTreeList.FocusedNodeChangedEventHandler(this.tree_HisServiceReq2_FocusedNodeChanged);
                this.tree_HisServiceReq2.Click -= new System.EventHandler(this.tree_HisServiceReq2_Click);
                this.gridControlHisTreatment5.Click -= new System.EventHandler(this.gridControlHisTreatment5_Click);
                this.gridViewHisTreatment5.RowCellClick -= new DevExpress.XtraGrid.Views.Grid.RowCellClickEventHandler(this.gridViewHisTreatment5_RowCellClick);
                this.gridViewHisTreatment5.CustomUnboundColumnData -= new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewHisTreatment5_CustomUnboundColumnData);
                this.repositoryItemButtonEdit__VienPhi.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repositoryItemButtonEdit__VienPhi_ButtonClick);
                this.repositoryItemButton__Send.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repositoryItemButton__Send_ButtonClick);
                this.repositoryEditServiceReq__Enable.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repositoryEditServiceReq__Enable_ButtonClick);
                this.txtKeyWord.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtKeyWord_PreviewKeyDown);
                this.txtPatientCode.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtPatientCode_PreviewKeyDown);
                this.txtTreatmentCode.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtTreatmentCode_PreviewKeyDown);
                this.toolTipController.GetActiveObjectInfo -= new DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventHandler(this.toolTipController_GetActiveObjectInfo);
                this.Load -= new System.EventHandler(this.frmTreatmentHistory_Load);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
