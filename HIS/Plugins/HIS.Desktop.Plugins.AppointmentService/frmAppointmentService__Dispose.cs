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
using DevExpress.XtraEditors;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.BackendData.ADO;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.Plugins.AppointmentService.Base;
using HIS.Desktop.Plugins.AppointmentService.Config;
using HIS.Desktop.Plugins.AppointmentService.Resources;
using HIS.Desktop.Utilities.Extensions;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AppointmentService
{
    public partial class frmAppointmentService : FormBase
    {
        public override void ProcessDisposeModuleDataAfterClose()
        {
            try
            {

                isProcessingAfternodeChecked = false;
                isHandlerResetServiceStateCheckeds = false;
                isHandlerResetServiceStateCheckedForTreeNodes = false;
                notSearch = 0;
                patientTypeIdAls = null;
                listPatientType = null;
                listDatasFix = null;
                instructionTime = 0;
                IscheckAllTreeService = false;
                groupType__PtttGroupName = 0;
                groupType__ServiceTypeName = 0;
                isSingleCheckService = 0;
                branch = null;
                treatment = null;
                PatientTypeAlter = null;
                hideCheckBoxHelper__Service = null;
                records = null;
                ServiceIsleafADOs = null;
                dicService = null;
                appointmentServiceInput = null;
                actSelect = null;
                treatmentId = 0;
                this.cboPriviousServiceReq.GetNotInListValue -= new DevExpress.XtraEditors.Controls.GetNotInListValueEventHandler(this.cboPriviousServiceReq_GetNotInListValue);
                this.cboPriviousServiceReq.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboPriviousServiceReq_Closed);
                this.cboPriviousServiceReq.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboPriviousServiceReq_ButtonClick);
                this.cboPriviousServiceReq.EditValueChanged -= new System.EventHandler(this.cboPriviousServiceReq_EditValueChanged);
                this.cboPriviousServiceReq.KeyUp -= new System.Windows.Forms.KeyEventHandler(this.cboPriviousServiceReq_KeyUp);
                this.barButtonItem1.ItemClick -= new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem1_ItemClick);
                this.barButtonItem2.ItemClick -= new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem2_ItemClick);
                this.cboServiceGroup.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboServiceGroup_Closed);
                this.cboServiceGroup.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboServiceGroup_ButtonClick);
                this.cboServiceGroup.CustomDisplayText -= new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(this.cboServiceGroup_CustomDisplayText);
                this.cboServiceGroup.TextChanged -= new System.EventHandler(this.cboServiceGroup_TextChanged);
                this.cboServiceGroup.Leave -= new System.EventHandler(this.cboServiceGroup_Leave);
                this.cboServiceGroup.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.cboServiceGroup_PreviewKeyDown);
                this.dtIntruction.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.dtIntruction_Closed);
                this.dtIntruction.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.dtIntruction_PreviewKeyDown);
                this.btnLamLai.Click -= new System.EventHandler(this.btnLamLai_Click);
                this.btnMoTatCa.Click -= new System.EventHandler(this.btnMoTatCa_Click);
                this.btnDongTatCa.Click -= new System.EventHandler(this.btnDongTatCa_Click);
                this.btnSave.Click -= new System.EventHandler(this.btnSave_Click);
                this.txtServiceName_Search.EditValueChanged -= new System.EventHandler(this.txtServiceName_Search_EditValueChanged);
                this.txtServiceName_Search.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.txtServiceName_Search_KeyDown);
                this.txtServiceCode_Search.EditValueChanged -= new System.EventHandler(this.txtServiceCode_Search_EditValueChanged);
                this.txtServiceCode_Search.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.txtServiceCode_Search_KeyDown);
                this.gridControlServiceProcess.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.gridControlServiceProcess_KeyDown);
                this.gridViewServiceProcess.CustomRowColumnError -= new System.EventHandler<Inventec.Desktop.CustomControl.RowColumnErrorEventArgs>(this.gridViewServiceProcess_CustomRowColumnError);
                this.gridViewServiceProcess.CustomDrawGroupRow -= new DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventHandler(this.gridViewServiceProcess_CustomDrawGroupRow);
                this.gridViewServiceProcess.CustomRowCellEdit -= new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gridViewServiceProcess_CustomRowCellEdit);
                this.gridViewServiceProcess.ColumnWidthChanged -= new DevExpress.XtraGrid.Views.Base.ColumnEventHandler(this.gridViewServiceProcess_ColumnWidthChanged);
                this.gridViewServiceProcess.CalcRowHeight -= new DevExpress.XtraGrid.Views.Grid.RowHeightEventHandler(this.gridViewServiceProcess_CalcRowHeight);
                this.gridViewServiceProcess.ShownEditor -= new System.EventHandler(this.gridViewServiceProcess_ShownEditor);
                this.gridViewServiceProcess.CellValueChanged -= new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gridViewServiceProcess_CellValueChanged);
                this.gridViewServiceProcess.CustomUnboundColumnData -= new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewServiceProcess_CustomUnboundColumnData);
                this.gridViewServiceProcess.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.gridViewServiceProcess_KeyDown);
                this.gridViewAppointmentServ.CustomUnboundColumnData -= new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewAppointmentServ_CustomUnboundColumnData);
                this.treeService.NodeCellStyle -= new DevExpress.XtraTreeList.GetCustomNodeCellStyleEventHandler(this.treeService_NodeCellStyle);
                this.treeService.BeforeExpand -= new DevExpress.XtraTreeList.BeforeExpandEventHandler(this.treeService_BeforeExpand);
                this.treeService.BeforeCheckNode -= new DevExpress.XtraTreeList.CheckNodeEventHandler(this.treeService_BeforeCheckNode);
                this.treeService.AfterCheckNode -= new DevExpress.XtraTreeList.NodeEventHandler(this.treeService_AfterCheckNode);
                this.treeService.NodeChanged -= new DevExpress.XtraTreeList.NodeChangedEventHandler(this.treeService_NodeChanged);
                this.treeService.Click -= new System.EventHandler(this.treeService_Click);
                this.toggleSwitchDataChecked.Toggled -= new System.EventHandler(this.toggleSwitchDataChecked_Toggled);
                this.Load -= new System.EventHandler(this.frmAppointmentService_Load);
                gridLookUpEdit1View.GridControl.DataSource = null;
                gridViewAppointmentServ.GridControl.DataSource = null;
                gridControlAppointmentServ.DataSource = null;
                repositoryItemcboShareCount.DataSource = null;
                gridView9.GridControl.DataSource = null;
                repositoryItemcboExcuteRoom_TabService.DataSource = null;
                gridView1.GridControl.DataSource = null;
                repositoryItemcboPatientType_TabService.DataSource = null;
                repositoryItemcboPatientType_TabService1.DataSource = null;
                gridViewServiceProcess.GridControl.DataSource = null;
                gridControlServiceProcess.DataSource = null;
                colHEIN_LIMIT_PRICEUnb = null;
                colHEIN_LIMIT_PRICE_DISPLAYUnb = null;
                colPRICE_DISPLAYUnb = null;
                emptySpaceItem2 = null;
                layoutControlItem15 = null;
                layoutControlItem4 = null;
                lblTotalPrice = null;
                lblTotalPriceBHYT = null;
                colPATIENT_TYPE_IDUnb = null;
                gridColumn7 = null;
                gridColumn6 = null;
                colUnb = null;
                gridColumn_ServicePatientType = null;
                colAMOUNTUnb = null;
                colTDL_SERVICE_NAMEUnb = null;
                colTDL_SERVICE_CODEUnb = null;
                colIsCheckedUnb = null;
                txtServiceCode_Search = null;
                txtServiceName_Search = null;
                layoutControlItem14 = null;
                panelControlServiceGrid = null;
                layoutControlItem13 = null;
                cboPriviousServiceReq = null;
                layoutControlItem12 = null;
                gridLookUpEdit1View = null;
                cboServiceGroup = null;
                emptySpaceItem1 = null;
                layoutControlItem11 = null;
                dtIntruction = null;
                barButtonItem2 = null;
                layoutControlItem10 = null;
                btnLamLai = null;
                barDockControlRight = null;
                barDockControlLeft = null;
                barDockControlBottom = null;
                barDockControlTop = null;
                barButtonItem1 = null;
                bar1 = null;
                barManager1 = null;
                layoutControlItem9 = null;
                layoutControlItem8 = null;
                btnDongTatCa = null;
                btnMoTatCa = null;
                gridColumn5 = null;
                gridColumn4 = null;
                gridColumn3 = null;
                gridColumn2 = null;
                layoutControlItem7 = null;
                gridViewAppointmentServ = null;
                gridControlAppointmentServ = null;
                layoutControlItem6 = null;
                btnSave = null;
                layoutControlItem5 = null;
                toggleSwitchDataChecked = null;
                repositoryItemTxtReadOnly = null;
                repositoryItemcboShareCount = null;
                repositoryItemSpinAmount__Disable_TabService = null;
                repositoryItemSpinEditEstimateDuration = null;
                gridView9 = null;
                repositoryItemcboExcuteRoom_TabService = null;
                repositoryItemChkIsOutKtcFee_Disable_TabService = null;
                repositoryItemChkIsOutKtcFee_Enable_TabService = null;
                gridView1 = null;
                repositoryItemcboPatientType_TabService = null;
                repositoryItemSpinAmount_TabService = null;
                repositoryItemChkIsKH_TabService = null;
                repositoryItemChkIsKH_Disable_Testpage = null;
                repositoryItemChkIsExpend_TabService = null;
                repositoryItemcboPatientType_TabService1 = null;
                repositoryItemBtnChecked__TabService = null;
                grcAmount_TabService = null;
                repositoryItemMemoEdit1 = null;
                grcServiceName_TabService = null;
                grcServiceCode_TabService = null;
                repositoryItemchkIsChecked = null;
                grcChecked_TabService = null;
                gridViewServiceProcess = null;
                gridControlServiceProcess = null;
                treeListColumn1 = null;
                layoutControlItem2 = null;
                layoutControlItem1 = null;
                layoutControlItem3 = null;
                Root = null;
                treeService = null;
                layoutControl2 = null;
                layoutControlGroup2 = null;
                layoutControl3 = null;
                layoutControlGroup1 = null;
                layoutControl1 = null;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
