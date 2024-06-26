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
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ServiceReqList
{
    public partial class frmServiceReqList : HIS.Desktop.Utility.FormBase
    {
        public override void ProcessDisposeModuleDataAfterClose()
        {
            try
            {
                loginName = null;
                lastInfo = null;
                lastColumn = null;
                currentRoom = null;
                serviceReqTypeSelecteds = null;
                serviceReqSttSelecteds = null;
                currentServiceReqPrint = null;
                currentServiceReq = null;
                serviceReqPrintRaw = null;
                currentPatient = null;
                treatment = null;
                prescriptionPrint = null;
                currentPrescription = null;
                currentModule = null;
                currentWorkPlace = null;
                PrintPopupMenuProcessor = null;
                listServiceReq = null;
                _listMedicine = null;
                lsRationTime = null;
                sereServPrint = null;
                rightClickData = null;
                treatmentCode = null;
                controlStateWorker = null;
                currentControlStateRDO = null;
                TreatmentWithPatientTypeAlter = null;
                dicParam = null;
                dicImage = null;
                _SereServNumOders = null;
                lstSereServTein = null;
                sereServExtPrint = null;
                sereServ = null;
                keyPrint = null;
                ConfigIds = null;
                lstConfig = null;

                this.popupControlContainer1.CloseUp -= new System.EventHandler(this.popupControlContainer1_CloseUp);
                this.repCheckConfig.CheckedChanged -= new System.EventHandler(this.repCheckConfig_CheckedChanged);
                this.bbtnRCFind.ItemClick -= new DevExpress.XtraBars.ItemClickEventHandler(this.bbtnRCFind_ItemClick);
                this.barButtonPrintTemBarcode.ItemClick -= new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonPrintTemBarcode_ItemClick);
                this.btnConfig.Click -= new System.EventHandler(this.btnConfig_Click);
                this.txtStoreCode.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.txtStoreCode_KeyDown);
                this.chkPK.CheckedChanged -= new System.EventHandler(this.chkPK_CheckedChanged);
                this.cboExecuteRoom.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboExecuteRoom_Closed);
                this.cboExecuteRoom.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboExecuteRoom_ButtonClick);
                this.txtPatientCode.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.txtPatientCode_KeyDown);
                this.btnPrintTemBarcode.Click -= new System.EventHandler(this.btnPrintTemBarcode_Click);
                this.btnDropDownPrint.Click -= new System.EventHandler(this.btnDropDownPrint_Click);
                this.btnPrintMedicine.Click -= new System.EventHandler(this.btnPrintMedicine_Click);
                this.btnPrintTotal.Click -= new System.EventHandler(this.btnPrintTotal_Click);
                this.txtTreatmentCode.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.txtTreatmentCode_KeyDown);
                this.btnMobaCreate.Click -= new System.EventHandler(this.btnMobaCreate_Click);
                this.btnAggrExpMest.Click -= new System.EventHandler(this.btnAggrExpMest_Click);
                this.cboServiceReqType.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboServiceReqType_Closed);
                this.cboServiceReqType.CustomDisplayText -= new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(this.cboServiceReqType_CustomDisplayText);
                this.gridView1.KeyUp -= new System.Windows.Forms.KeyEventHandler(this.gridView1_KeyUp);
                this.cboServiceReqStt.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboServiceReqStt_Closed);
                this.cboServiceReqStt.CustomDisplayText -= new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(this.cboServiceReqStt_CustomDisplayText);
                this.gridLookUpEdit1View.KeyUp -= new System.Windows.Forms.KeyEventHandler(this.gridLookUpEdit1View_KeyUp);
                this.grdSereServServiceReq.DataSourceChanged -= new System.EventHandler(this.grdSereServServiceReq_DataSourceChanged);
                this.grdViewSereServServiceReq.CustomDrawGroupRow -= new DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventHandler(this.grdViewSereServServiceReq_CustomDrawGroupRow);
                this.grdViewSereServServiceReq.RowCellStyle -= new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.grdViewSereServServiceReq_RowCellStyle);
                this.grdViewSereServServiceReq.CustomRowCellEdit -= new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.grdViewSereServServiceReq_CustomRowCellEdit);
                this.grdViewSereServServiceReq.PopupMenuShowing -= new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(this.grdViewSereServServiceReq_PopupMenuShowing);
                this.grdViewSereServServiceReq.CustomUnboundColumnData -= new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.grdViewSereServServiceReq_CustomUnboundColumnData);
                this.repositoryItemButtonView.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repositoryItemButtonView_ButtonClick);
                this.repositoryItemButtonPrint.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repositoryItemButtonPrint_ButtonClick);
                this.repositoryItemButtonEditDeleteEna.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repositoryItemButtonEditDeleteEna_ButtonClick);
                this.txtServiceReqCode.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtServiceReqCode_PreviewKeyDown);
                this.txtKeyword.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtKeyword_PreviewKeyDown);
                this.btnFind.Click -= new System.EventHandler(this.btnFind_Click);
                this.gridControlServiceReq.Click -= new System.EventHandler(this.gridControlServiceReq_Click);
                this.gridViewServiceReq.RowCellStyle -= new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gridViewServiceReq_RowCellStyle);
                this.gridViewServiceReq.CustomRowCellEdit -= new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gridViewServiceReq_CustomRowCellEdit);
                this.gridViewServiceReq.PopupMenuShowing -= new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(this.gridViewServiceReq_PopupMenuShowing);
                this.gridViewServiceReq.CustomUnboundColumnData -= new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewServiceReq_CustomUnboundColumnData);
                this.gridViewServiceReq.MouseDown -= new System.Windows.Forms.MouseEventHandler(this.gridViewServiceReq_MouseDown);
                this.repositoryItemCheckEditChoose.CheckedChanged -= new System.EventHandler(this.repositoryItemCheckEditChoose_CheckedChanged);
                this.repositoryItemButtonEditAllowNotExecute_Enable.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repositoryItemButtonEditAllowNotExecute_Enable_ButtonClick);
                this.repositoryItemButtonEditIntructionTime.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repositoryItemButtonEditIntructionTime_ButtonClick);
                this.repositoryItemButton__BieuMauKhac.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repositoryItemButton__BieuMauKhac_ButtonClick);
                this.Btn_EvenLog.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.Btn_EvenLog_ButtonClick);
                this.repositoryItemTextEdit.Click -= new System.EventHandler(this.repositoryItemTextEdit_Click);
                this.repositoryItemBtnDeleteServiceReq.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repositoryItemBtnServiceReqDelete_ButtonClick);
                this.repositoryItemBtnEditServiceReq.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repositoryItemBtnServiceReqEdit_ButtonClick);
                this.repositoryItemBtnPrintServiceReq.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repositoryItemBtnServiceReqPrint_ButtonClick);
                this.repositoryItemBtnBieuMauKhac.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repositoryItemBtnBieuMauKhac_ButtonClick);
                this.tooltipServiceRequest.GetActiveObjectInfo -= new DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventHandler(this.tooltipServiceRequest_GetActiveObjectInfo);
                this.Load -= new System.EventHandler(this.frmServiceReqList_Load);


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
