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
using ACS.EFMODEL.DataModels;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.BackendData.ADO;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.AssignService.ADO;
using HIS.Desktop.Plugins.AssignService.Config;
using HIS.Desktop.Plugins.AssignService.Resources;
using HIS.Desktop.Plugins.Library.AlertWarningFee;
using HIS.Desktop.Print;
using HIS.Desktop.Utilities.Extensions;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.LibraryMessage;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignService.AssignService
{
    public partial class frmAssignService : HIS.Desktop.Utility.FormBase
    {
        public override void ProcessDisposeModuleDataAfterClose()
        {
            try
            {

                Inventec.Common.Logging.LogSystem.Debug("DisposeForm.1");
                arrControlEnableNotChange = null;
                dicOrderTabIndexControl = null;
                periodSeparators = null;
                icdSeparators = null;
                intructionTimeSelecteds = null;
                intructionTimeSelected = null;
                currentSereServ = null;
                currentSereServInEkip = null;
                processDataResult = null;
                processRefeshIcd = null;
                //currentHisTreatment = null;
                serviceReqMain = null;
                currentHisPatientTypeAlter = null;
                currentPatientTypeWithPatientTypeAlter = null;
                currentPreServiceReqs = null;
                //serviceReqComboResultSDO = null;
                dicServiceReqList = null;
                hideCheckBoxHelper__Service = null;
                records = null;
                ServiceIsleafADOs = null;
                ServiceParentADOs = null;
                ServiceParentADOForGridServices = null;
                ServiceAllADOs = null;
                SereServADO__Main = null;
                sereServWithTreatment = null;
                sereServsInTreatment = null;
                sereServsInTreatmentRaw = null;
                //currentModule = null;
                servicePatyInBranchs = null;
                dicServices = null;
                icdServicePhacDos = null;
                hisRoomCounters = null;
                currentIcds = null;
                currentPatientTypes = null;
                currentPatientTypeAllows = null;
                currentDhst = null;
                icdChoose = null;
                roomTimes = null;
                exroRooms = null;
                trackingAdos = null;
                requestRoom = null;
                currentDepartment = null;
                serviceTypeIdAllows = null;
                patientTypeIdAls = null;
                currentExecuteRooms = null;
                allDataExecuteRooms = null;
                currentServiceSames = null;
                icdExam = null;
                controlStateWorker = null;
                currentControlStateRDO = null;
                assRoomsWorks = null;
                currentRowSereServADO = null;
                serviceConditions = null;
                lastInfo = null;
                lastColumn = null;
                lstLoaiPhieu = null;
                workingAssignServiceADO = null;
                treatmentPrint = null;
                patientPrint = null;
                currentPatient = null;
                currentTreatment = null;
                selectedSeviceGroups = null;
                selectedSeviceGroupCopys = null;
                workingServiceGroupADOs = null;
                serviceDeleteWhileSelectSeviceGroups = null;
                tracking = null;
                trackings = null;
                patientTypeByPT = null;
                icdSubcodeAdoChecks = null;
                subIcdPopupSelect = null;
                TreatmentBedRooms = null;
                currentWorkingRoom = null;
                PatientKskCode = null;
                hisSereServForGetPatientType = null;
                lstService = null;
                Listtrackings = null;

                this.cboKsk.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboKsk_ButtonClick);
                this.cboKsk.EditValueChanged -= new System.EventHandler(this.cboKsk_EditValueChanged);
                this.barbtnSaveShortcut.ItemClick -= new DevExpress.XtraBars.ItemClickEventHandler(this.barbtnSaveShortcut_ItemClick);
                this.barbtnSaveAndPrintShortcut.ItemClick -= new DevExpress.XtraBars.ItemClickEventHandler(this.barbtnSaveAndPrintShortcut_ItemClick);
                this.barbtnPrintShortcut.ItemClick -= new DevExpress.XtraBars.ItemClickEventHandler(this.barbtnPrintShortcut_ItemClick);
                this.barbtnNew.ItemClick -= new DevExpress.XtraBars.ItemClickEventHandler(this.barbtnNew_ItemClick);
                this.BarSavePrint_PrintTH.ItemClick -= new DevExpress.XtraBars.ItemClickEventHandler(this.BarSavePrint_PrintTH_ItemClick);
                this.barBtnSaveNShow.ItemClick -= new DevExpress.XtraBars.ItemClickEventHandler(this.barBtnSaveNShow_ItemClick);
                this.bbtnEditCtrlU.ItemClick -= new DevExpress.XtraBars.ItemClickEventHandler(this.bbtnEditCtrlU_ItemClick);
                this.barButtonItem1.ItemClick -= new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem1_ItemClick);
                this.btnCreateServiceGroup.Click -= new System.EventHandler(this.btnCreateServiceGroup_Click);
                this.ckTK.CheckedChanged -= new System.EventHandler(this.ckTK_CheckedChanged);
                this.beditRoom.Properties.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.beditRoom_Properties_ButtonClick);
                this.beditRoom.TextChanged -= new System.EventHandler(this.beditRoom_TextChanged);
                this.beditRoom.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.beditRoom_KeyDown);
                this.popupControlContainerRoom.CloseUp -= new System.EventHandler(this.popupControlContainerRoom_CloseUp);
                this.gridControlContainerRoom.ProcessGridKey -= new System.Windows.Forms.KeyEventHandler(this.gridControlContainerRoom_ProcessGridKey);
                this.gridControlContainerRoom.Click -= new System.EventHandler(this.gridControlContainerRoom_Click);
                this.gridViewContainerRoom.RowStyle -= new DevExpress.XtraGrid.Views.Grid.RowStyleEventHandler(this.gridViewContainerRoom_RowStyle);
                this.gridViewContainerRoom.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.gridViewContainerRoom_KeyDown);
                this.gridViewContainerRoom.MouseDown -= new System.Windows.Forms.MouseEventHandler(this.gridViewContainerRoom_MouseDown);
                this.btnEdit.Click -= new System.EventHandler(this.btnEdit_Click);
                this.btnConfiguration.Click -= new System.EventHandler(this.btnConfiguration_Click);
                this.Switch_ExpendAll.Toggled -= new System.EventHandler(this.Switch_ExpendAll_Toggled);
                this.cboPackage.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboPackage_Closed);
                this.cboPackage.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboPackage_ButtonClick);
                this.cboPackage.EditValueChanged -= new System.EventHandler(this.cboPackage_EditValueChanged);
                this.cboPackage.KeyUp -= new System.Windows.Forms.KeyEventHandler(this.cboPackage_KeyUp);
                this.txtConsultantLoginname.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtConsultantLoginname_PreviewKeyDown);
                this.cboConsultantUser.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboConsultantUser_Closed);
                this.cboConsultantUser.KeyUp -= new System.Windows.Forms.KeyEventHandler(this.cboConsultantUser_KeyUp);
                this.txtAssignRoomCode.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtAssignRoomCode_PreviewKeyDown);
                this.cboAssignRoom.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboAssignRoom_Closed);
                this.cboAssignRoom.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboAssignRoom_ButtonClick);
                this.cboAssignRoom.EditValueChanged -= new System.EventHandler(this.cboAssignRoom_EditValueChanged);
                this.cboAssignRoom.KeyUp -= new System.Windows.Forms.KeyEventHandler(this.cboAssignRoom_KeyUp);

                this.btnDepositService.Click -= new System.EventHandler(this.btnDepositService_Click);
                this.chkPrintDocumentSigned.CheckedChanged -= new System.EventHandler(this.chkPrintDocumentSigned_CheckedChanged);
                this.chkSign.CheckedChanged -= new System.EventHandler(this.chkSign_CheckedChanged);
                this.chkPrint.CheckedChanged -= new System.EventHandler(this.chkPrint_CheckedChanged);
                this.BtnPrint.Click -= new System.EventHandler(this.BtnPrint_Click);
                this.cboIcdsCause.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboIcdsCause_Closed);
                this.cboIcdsCause.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboIcdsCause_Properties_ButtonClick);
                this.cboIcdsCause.TextChanged -= new System.EventHandler(this.cboIcdsCause_TextChanged);
                this.cboIcdsCause.KeyUp -= new System.Windows.Forms.KeyEventHandler(this.cboIcdsCause_KeyUp);
                this.txtIcdMainTextCause.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtIcdMainTextCause_PreviewKeyDown);
                this.chkEditIcdCause.CheckedChanged -= new System.EventHandler(this.chkEditIcdCause_CheckedChanged);
                this.chkEditIcdCause.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.chkEditIcdCause_PreviewKeyDown);
                this.txtIcdCodeCause.InvalidValue -= new DevExpress.XtraEditors.Controls.InvalidValueExceptionEventHandler(this.txtIcdCodeCause_InvalidValue);
                this.txtIcdCodeCause.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtIcdCodeCause_PreviewKeyDown);
                this.txtIcdCodeCause.Validating -= new System.ComponentModel.CancelEventHandler(this.txtIcdCodeCause_Validating);
                this.popupControlContainerSubIcdName.CloseUp -= new System.EventHandler(this.popupControlContainerSubIcdName_CloseUp);
                this.customGridControlSubIcdName.DoubleClick -= new System.EventHandler(this.customGridControlSubIcdName_DoubleClick);
                this.customGridViewSubIcdName.CellValueChanged -= new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.customGridViewSubIcdName_CellValueChanged);
                this.customGridViewSubIcdName.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.customGridViewSubIcdName_KeyDown);
                this.customGridViewSubIcdName.MouseDown -= new System.Windows.Forms.MouseEventHandler(this.customGridViewSubIcdName_MouseDown);
                this.gridControlOtherPaySource.Click -= new System.EventHandler(this.gridControlOtherPaySource_Click);
                this.gridViewOtherPaySource.CustomUnboundColumnData -= new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewOtherPaySource_CustomUnboundColumnData);
                this.gridViewOtherPaySource.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.gridViewOtherPaySource_KeyDown);
                this.gridControlCondition.Click -= new System.EventHandler(this.gridControlCondition_Click);
                this.gridViewCondition.CustomUnboundColumnData -= new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewCondition_CustomUnboundColumnData);
                this.gridViewCondition.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.gridViewCondition_KeyDown);
                this.gridView7.CellValueChanged -= new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gridView7_CellValueChanged);
                this.repositoryItemCheckEdit1.CheckedChanged -= new System.EventHandler(this.repositoryItemCheckEdit1_CheckedChanged);
                this.gridControlServiceProcess.ProcessGridKey -= new System.Windows.Forms.KeyEventHandler(this.gridControlServiceProcess_ProcessGridKey);
                this.gridViewServiceProcess.CustomRowColumnError -= new System.EventHandler<Inventec.Desktop.CustomControl.RowColumnErrorEventArgs>(this.gridViewServiceProcess_CustomRowColumnError);
                this.gridViewServiceProcess.CustomDrawGroupRow -= new DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventHandler(this.gridViewServiceProcess_CustomDrawGroupRow);
                this.gridViewServiceProcess.RowCellStyle -= new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gridViewServiceProcess_RowCellStyle);
                this.gridViewServiceProcess.CustomRowCellEdit -= new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gridViewServiceProcess_CustomRowCellEdit);
                this.gridViewServiceProcess.ShownEditor -= new System.EventHandler(this.gridViewServiceProcess_ShownEditor);
                this.gridViewServiceProcess.CellValueChanged -= new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gridViewServiceProcess_CellValueChanged);
                this.gridViewServiceProcess.ColumnFilterChanged -= new System.EventHandler(this.gridViewServiceProcess_ColumnFilterChanged);
                this.gridViewServiceProcess.CustomUnboundColumnData -= new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewServiceProcess_CustomUnboundColumnData);
                this.gridViewServiceProcess.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.gridViewServiceProcess_KeyDown);
                this.gridViewServiceProcess.MouseDown -= new System.Windows.Forms.MouseEventHandler(this.gridViewServiceProcess_MouseDown);
                this.gridViewServiceProcess.DoubleClick -= new System.EventHandler(this.gridViewServiceProcess_DoubleClick);
                this.repositoryItemIsView.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repositoryItemIsView_ButtonClick);
                this.repositoryItemMemoExEdit_IntructionNote.Popup -= new System.EventHandler(this.repositoryItemMemoExEdit_IntructionNote_Popup);
                this.repositoryItemcboExcuteRoom_TabService.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.repositoryItemcboExcuteRoom_TabService_Closed);
                this.repositoryItemButtonEditOtherPaySource.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repositoryItemButtonEditOtherPaySource_ButtonClick);
                this.repositoryItemButtonCondition.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repositoryItemButtonCondition_ButtonClick);
                this.repositoryItemcboPatientType_TabService1.EditValueChanged -= new System.EventHandler(this.repositoryItemcboPatientType_TabService1_EditValueChanged);
                this.repositoryItemCboPrimaryPatientType.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repositoryItemCboPrimaryPatientType_ButtonClick);
                this.repositoryItembtnEditDonGia_TextDisable.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repositoryItembtnEditDonGia_TextDisable_ButtonClick);
                this.repositoryItembtnEditGiaGoi_TextDisable.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repositoryItembtnEditGiaGoi_TextDisable_ButtonClick);
                this.repositoryItemcboExcuteRoomPlus_TabService.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.repositoryItemcboExcuteRoom_TabService_Closed);
                this.repositoryItemcboExcuteRoomPlus_TabService.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repositoryItemcboExcuteRoomPlus_TabService_ButtonClick);
                this.repositoryItemButtonEkipTempEn.Click -= new System.EventHandler(this.repositoryItemButtonEkipTempEn_Click);
                this.btnPrintPhieuHuongDanBN.Click -= new System.EventHandler(this.btnPrintPhieuHuongDanBN_Click);
                this.chkIsEmergency.EditValueChanged -= new System.EventHandler(this.chkIsEmergency_EditValueChanged);
                this.chkIsEmergency.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.chkIsEmergency_PreviewKeyDown);
                this.btnBoSungPhacDo.Click -= new System.EventHandler(this.btnBoSungPhacDo_Click);
                this.cboTracking.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboTracking_ButtonClick);
                this.cboTracking.EditValueChanged -= new System.EventHandler(this.cboTracking_EditValueChanged);
                this.cboTracking.CustomDisplayText -= new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(this.cboTracking_CustomDisplayText);
                this.cboTracking.KeyUp -= new System.Windows.Forms.KeyEventHandler(this.cboTracking_KeyUp);
                this.cboCashierRoom.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboCashierRoom_Closed);
                this.btnCreateBill.Click -= new System.EventHandler(this.btnCreateBill_Click);
                this.dtInstructionTime.EditValueChanged -= new System.EventHandler(this.dtInstructionTime_EditValueChanged);
                this.dtInstructionTime.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.dtInstructionTime_PreviewKeyDown);
                this.txtInstructionTime.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txtInstructionTime_ButtonClick);
                this.txtInstructionTime.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtInstructionTime_PreviewKeyDown);
                this.timeIntruction.Leave -= new System.EventHandler(this.timeIntruction_Leave);
                this.timeIntruction.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.timeIntruction_PreviewKeyDown);
                this.chkMultiIntructionTime.CheckedChanged -= new System.EventHandler(this.chkMultiIntructionTime_CheckedChanged);
                this.chkMultiIntructionTime.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.chkMultiIntructionTime_PreviewKeyDown);
                this.txtIcdText.TextChanged -= new System.EventHandler(this.txtIcdText_TextChanged);
                this.txtIcdText.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.txtIcdText_KeyDown);
                this.txtIcdText.KeyPress -= new System.Windows.Forms.KeyPressEventHandler(this.txtIcdText_KeyPress);
                this.txtIcdText.KeyUp -= new System.Windows.Forms.KeyEventHandler(this.txtIcdText_KeyUp);
                this.txtIcdSubCode.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.txtIcdSubCode_KeyDown);
                this.txtIcdSubCode.KeyUp -= new System.Windows.Forms.KeyEventHandler(this.txtIcdSubCode_KeyUp);
                this.cboIcds.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboIcds_Closed);
                this.cboIcds.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboIcds_Properties_ButtonClick);
                this.cboIcds.EditValueChanged -= new System.EventHandler(this.cboIcds_EditValueChanged);
                this.cboIcds.TextChanged -= new System.EventHandler(this.cboIcds_TextChanged);
                this.cboIcds.KeyUp -= new System.Windows.Forms.KeyEventHandler(this.cboIcds_KeyUp);
                this.txtIcdMainText.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtIcdMainText_PreviewKeyDown);
                this.chkEditIcd.CheckedChanged -= new System.EventHandler(this.chkEditIcd_CheckedChanged);
                this.chkEditIcd.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.chkEditIcd_PreviewKeyDown);
                this.txtIcdCode.InvalidValue -= new DevExpress.XtraEditors.Controls.InvalidValueExceptionEventHandler(this.txtIcdCode_InvalidValue);
                this.txtIcdCode.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtIcdCode_PreviewKeyDown);
                this.txtIcdCode.Validating -= new System.ComponentModel.CancelEventHandler(this.txtIcdCode_Validating);
                this.txtLoginName.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtLoginName_PreviewKeyDown);
               
                this.cboPriviousServiceReq.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboPriviousServiceReq_ButtonClick);
                this.cboPriviousServiceReq.KeyUp -= new System.Windows.Forms.KeyEventHandler(this.cboPriviousServiceReq_KeyUp);
                this.cboPriviousServiceReq.Leave -= new System.EventHandler(this.cboPriviousServiceReq_Leave);
                this.btnServiceReqList.Click -= new System.EventHandler(this.btnServiceReqList_Click);
                this.btnNew.Click -= new System.EventHandler(this.btnNew_Click);
                this.toggleSwitchDataChecked.Toggled -= new System.EventHandler(this.toggleSwitchDataChecked_Toggled);
                this.chkExpendAll.CheckedChanged -= new System.EventHandler(this.chkExpendAll_CheckedChanged);
                this.chkExpendAll.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.chkExpendAll_PreviewKeyDown);
                this.cboExecuteGroup.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboExecuteGroup_Closed);
                this.cboExecuteGroup.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboExecuteGroup_ButtonClick);
                this.cboExecuteGroup.KeyUp -= new System.Windows.Forms.KeyEventHandler(this.cboExecuteGroup_KeyUp);
                this.chkIsNotRequireFee.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.chkEmergency_PreviewKeyDown);
                this.chkPriority.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.chkPriority_PreviewKeyDown);
                this.btnSaveAndPrint.Click -= new System.EventHandler(this.btnSaveAndPrint_Click);
                this.btnShowDetail.Click -= new System.EventHandler(this.btnShowDetail_Click);
                this.btnSave.Click -= new System.EventHandler(this.btnSave_Click);
                this.treeService.NodeCellStyle -= new DevExpress.XtraTreeList.GetCustomNodeCellStyleEventHandler(this.treeService_NodeCellStyle);
                this.treeService.BeforeExpand -= new DevExpress.XtraTreeList.BeforeExpandEventHandler(this.treeService_BeforeExpand);
                this.treeService.BeforeCheckNode -= new DevExpress.XtraTreeList.CheckNodeEventHandler(this.treeService_BeforeCheckNode);
                this.treeService.AfterCheckNode -= new DevExpress.XtraTreeList.NodeEventHandler(this.treeService_AfterCheckNode);
                this.treeService.NodeChanged -= new DevExpress.XtraTreeList.NodeChangedEventHandler(this.treeService_NodeChanged);
                this.treeService.Click -= new System.EventHandler(this.treeService_Click);
                this.treeService.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.treeService_KeyDown);
                this.cboUser.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboUser_Closed);
                this.cboUser.KeyUp -= new System.Windows.Forms.KeyEventHandler(this.cboUser_KeyUp);
                this.txtDescription.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtDescription_PreviewKeyDown);
                this.tooltipService.GetActiveObjectInfo -= new DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventHandler(this.tooltipService_GetActiveObjectInfo);
                this.timerInitForm.Tick -= new System.EventHandler(this.timerInitForm_Tick);
                this.backgroundWorker1.DoWork -= new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
                this.backgroundWorker1.RunWorkerCompleted -= new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
                SereServGet ss = new SereServGet();
                ss.DisposeSereServ();
                ss = null;
                Inventec.Common.Logging.LogSystem.Debug("DisposeForm.2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}
