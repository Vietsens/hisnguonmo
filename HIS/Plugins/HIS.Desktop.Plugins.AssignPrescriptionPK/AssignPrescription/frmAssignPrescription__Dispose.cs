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
using HIS.Desktop.Plugins.AssignPrescriptionPK.ADO;
using HIS.Desktop.Plugins.AssignPrescriptionPK.Config;
using HIS.Desktop.Plugins.AssignPrescriptionPK.MessageBoxForm;
using HIS.Desktop.Plugins.AssignPrescriptionPK.Resources;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignPrescriptionPK.AssignPrescription
{
    public partial class frmAssignPrescription : HIS.Desktop.Utility.FormBase
    {
        public override void ProcessDisposeModuleDataAfterClose()
        {
            try
            {
                this.FormClosing -= new System.Windows.Forms.FormClosingEventHandler(this.frmAssignPrescription_FormClosing);
                this.Load -= new System.EventHandler(this.frmAssignPrescription_Load);

                this.layoutControl8.SuspendLayout();
                this.layoutControl8.Controls.Clear();
                this.layoutControl8.ResumeLayout(false);

                this.gridControlMediMaty.DataSource = null;
                this.gridControlServiceProcess.DataSource = null;
                //this.gridControlOtherPaySource.DataSource = null;
                //this.customGridControlSubIcdName.DataSource = null;
                //this.gridControlCondition.DataSource = null;

                this.navBarControlChongChiDinhInfo.NavPaneStateChanged -= new System.EventHandler(this.navBarControlChongChiDinhInfo_NavPaneStateChanged);
                this.txtMediMatyForPrescription.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txtMediMatyForPrescription_ButtonClick);
                this.txtMediMatyForPrescription.TextChanged -= new System.EventHandler(this.txtMediMatyForPrescription_TextChanged);
                this.txtMediMatyForPrescription.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.txtMediMatyForPrescription_KeyDown);
                this.rdOpionGroup.SelectedIndexChanged -= new System.EventHandler(this.rdOpionGroup_SelectedIndexChanged);
                this.txtUnitOther.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtUnitOther_PreviewKeyDown);
                this.txtMedicineTypeOther.Leave -= new System.EventHandler(this.txtMedicineTypeOther_Leave);
                this.txtMedicineTypeOther.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtMedicineTypeOther_PreviewKeyDown);
                this.gridViewMediMaty.RowClick -= new DevExpress.XtraGrid.Views.Grid.RowClickEventHandler(this.gridViewMediMaty_RowClick);
                this.gridViewMediMaty.CustomDrawCell -= new DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventHandler(this.OnCustomDrawCell);
                this.gridViewMediMaty.RowCellStyle -= new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gridViewMediMaty_RowCellStyle);
                this.gridViewMediMaty.CustomUnboundColumnData -= new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewMediMaty_CustomUnboundColumnData);
                this.gridViewMediMaty.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.gridViewMediMaty_KeyDown);
                this.gridViewMediMaty.MouseMove -= new System.Windows.Forms.MouseEventHandler(this.gridViewMediMaty_MouseMove);
                this.popupControlContainerMediMaty.CloseUp -= new System.EventHandler(this.popupControlContainerMediMaty_CloseUp);

                this.cboExpMestTemplate.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboTemplate_Medicine_Closed);
                this.cboExpMestTemplate.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboTemplate_Medicine_ButtonClick);
                this.cboExpMestTemplate.KeyUp -= new System.Windows.Forms.KeyEventHandler(this.cboTemplate_Medicine_KeyUp);
                this.cboExpMestTemplate.Leave -= new System.EventHandler(this.cboExpMestTemplate_Leave);

                this.txtLadder.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtLadder_PreviewKeyDown);

                this.chkPDDT.CheckedChanged -= new System.EventHandler(this.chkPDDT_CheckedChanged);
                this.chkSignForDDT.CheckedChanged -= new System.EventHandler(this.chkSignForDDT_CheckedChanged);
                this.chkSignForDTT.CheckedChanged -= new System.EventHandler(this.chkSignForDTT_CheckedChanged);
                this.chkSignForDPK.CheckedChanged -= new System.EventHandler(this.chkSign_CheckedChanged);
                    
                this.spinWeight.EditValueChanged -= new System.EventHandler(this.spinWeight_EditValueChanged);
                this.spinHeight.EditValueChanged -= new System.EventHandler(this.spinHeight_EditValueChanged);

                     
                  
                this.gridControlOtherPaySource.Click -= new System.EventHandler(this.gridControlOtherPaySource_Click);
                this.gridViewOtherPaySource.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.gridViewOtherPaySource_KeyDown);

                this.customGridControlSubIcdName.DoubleClick -= new System.EventHandler(this.customGridControlSubIcdName_DoubleClick);
                this.customGridViewSubIcdName.CellValueChanged -= new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.customGridViewSubIcdName_CellValueChanged);
                this.customGridViewSubIcdName.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.customGridViewSubIcdName_KeyDown);
                this.customGridViewSubIcdName.MouseDown -= new System.Windows.Forms.MouseEventHandler(this.customGridViewSubIcdName_MouseDown);

                this.popupControlContainer1.CloseUp -= new System.EventHandler(this.popupControlContainer1_CloseUp);
                this.repCheckConfig.CheckedChanged -= new System.EventHandler(this.repCheckConfig_CheckedChanged);

                this.dtUseTime.EditValueChanged -= new System.EventHandler(this.dtUseTime_EditValueChanged);
                this.dtUseTime.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.dtUseTime_PreviewKeyDown);

                this.gridViewTutorial.RowClick -= new DevExpress.XtraGrid.Views.Grid.RowClickEventHandler(this.gridViewTutorial_RowClick);
                this.gridViewTutorial.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.gridViewTutorial_KeyDown);
                this.gridControlServiceProcess.DataSourceChanged -= new System.EventHandler(this.gridControlServiceProcess_DataSourceChanged);
                this.gridControlServiceProcess.ProcessGridKey -= new System.Windows.Forms.KeyEventHandler(this.gridControlServiceProcess_ProcessGridKey);
                this.gridControlServiceProcess.DoubleClick -= new System.EventHandler(this.gridControlServiceProcess_DoubleClick);
                this.gridControlServiceProcess.DragOver -= new System.Windows.Forms.DragEventHandler(this.gridControlServiceProcess_DragOver);
                this.gridControlServiceProcess.DragDrop -= new System.Windows.Forms.DragEventHandler(this.gridControlServiceProcess_DragDrop);

                this.gridViewServiceProcess.CustomRowColumnError -= new System.EventHandler<Inventec.Desktop.CustomControl.RowColumnErrorEventArgs>(this.gridViewServiceProcess_CustomRowColumnError);
                this.gridViewServiceProcess.MouseMove -= new System.Windows.Forms.MouseEventHandler(this.gridViewServiceProcess_MouseMove);
                this.gridViewServiceProcess.RowCellClick -= new DevExpress.XtraGrid.Views.Grid.RowCellClickEventHandler(this.gridViewServiceProcess_RowCellClick);
                this.gridViewServiceProcess.RowCellStyle -= new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gridViewServiceProcess_RowCellStyle);
                this.gridViewServiceProcess.CustomRowCellEdit -= new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gridViewServiceProcess_CustomRowCellEdit);
                this.gridViewServiceProcess.PopupMenuShowing -= new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(this.gridViewServiceProcess_PopupMenuShowing);
                this.gridViewServiceProcess.ShowingEditor -= new System.ComponentModel.CancelEventHandler(this.gridViewServiceProcess_ShowingEditor);
                this.gridViewServiceProcess.ShownEditor -= new System.EventHandler(this.gridViewServiceProcess_ShownEditor);
                this.gridViewServiceProcess.CellValueChanged -= new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gridViewServiceProcess_CellValueChanged);
                this.gridViewServiceProcess.CustomUnboundColumnData -= new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewServiceProcess_CustomUnboundColumnData);
                this.gridViewServiceProcess.MouseDown -= new System.Windows.Forms.MouseEventHandler(this.gridViewServiceProcess_MouseDown);

                this.chkPreviewBeforePrint.CheckedChanged -= new System.EventHandler(this.chkPreviewBeforePrint_CheckedChanged);
                this.chkEyeInfo.CheckedChanged -= new System.EventHandler(this.chkThongTinMat_CheckedChanged);
                this.chkPrint.CheckedChanged -= new System.EventHandler(this.chkPrint_CheckedChanged);
                this.dtInstructionTimeForMedi.EditValueChanged -= new System.EventHandler(this.dtInstructionTimeForMedi_EditValueChanged);
                this.dtInstructionTimeForMedi.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.dtInstructionTimeForMedi_PreviewKeyDown);
                this.txtInstructionTimeForMedi.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txtInstructionTimeForMedi_ButtonClick);
                this.txtInstructionTimeForMedi.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtInstructionTimeForMedi_PreviewKeyDown);
                this.chkMultiIntructionTimeForMedi.CheckedChanged -= new System.EventHandler(this.chkMultiIntructionTimeForMedi_CheckedChanged);
                this.chkMultiIntructionTimeForMedi.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.chkMultiIntructionTimeForMedi_PreviewKeyDown);
                this.chkShowLo.CheckedChanged -= new System.EventHandler(this.chkShowLo_CheckedChanged);
                this.txtPreviousUseDay.KeyPress -= new System.Windows.Forms.KeyPressEventHandler(this.txtPreviousUseDay_KeyPress);
                this.txtPreviousUseDay.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtPreviousUseDay_PreviewKeyDown);
                this.txtThoiGianTho.InvalidValue -= new DevExpress.XtraEditors.Controls.InvalidValueExceptionEventHandler(this.txtThoiGianTho_InvalidValue);
                this.txtThoiGianTho.EditValueChanged -= new System.EventHandler(this.txtThoiGianTho_EditValueChanged);
                this.txtThoiGianTho.KeyPress -= new System.Windows.Forms.KeyPressEventHandler(this.txtThoiGianTho_KeyPress);
                this.txtThoiGianTho.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtThoiGianTho_PreviewKeyDown);
                this.txtThoiGianTho.Validating -= new System.ComponentModel.CancelEventHandler(this.txtThoiGianTho_Validating);

                this.cboMediStockExport.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboMediStockExport_TabMedicine_ButtonClick);
                this.cboMediStockExport.EditValueChanged -= new System.EventHandler(this.cboMediStockExport_EditValueChanged);

                this.gridControlCondition.Click -= new System.EventHandler(this.gridControlCondition_Click);
                this.gridViewCondition.CustomUnboundColumnData -= new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewCondition_CustomUnboundColumnData);
                this.gridViewCondition.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.gridViewCondition_KeyDown);

                this.dxValidProviderBoXung.ValidationFailed -= new DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventHandler(this.dxValidProviderBoXung__MedicinePage_ValidationFailed);
                this.dxValidProviderBoXung__DuongDung.ValidationFailed -= new DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventHandler(this.dxValidProviderBoXung__DuongDung_ValidationFailed);
                this.dxValidationProviderMaterialTypeTSD.ValidationFailed -= new DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventHandler(this.dxValidationProviderMaterialTypeTSD_ValidationFailed);
                this.dxValidationProviderControl.ValidationFailed -= new DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventHandler(this.dxValidationProviderControl_ValidationFailed);

                this.timerInitForm.Enabled = false;
                this.timerInitForm.Tick -= new System.EventHandler(this.timerInitForm_Tick);

                this.popupControlContainerSubIcdName.CloseUp -= new System.EventHandler(this.popupControlContainerSubIcdName_CloseUp);

                this.cboExpMestReason.Properties.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboExpMestReason_Properties_ButtonClick);
                this.cboExpMestReason.KeyUp -= new System.Windows.Forms.KeyEventHandler(this.cboExpMestReason_KeyUp);

                this.txtTocDoTho.InvalidValue -= new DevExpress.XtraEditors.Controls.InvalidValueExceptionEventHandler(this.txtTocDoTho_InvalidValue);
                this.txtTocDoTho.EditValueChanged -= new System.EventHandler(this.txtTocDoTho_EditValueChanged);
                this.txtTocDoTho.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtTocDoTho_PreviewKeyDown);
                this.txtTocDoTho.Validating -= new System.ComponentModel.CancelEventHandler(this.txtTocDoTho_Validating);

                this.spinSang.InvalidValue -= new DevExpress.XtraEditors.Controls.InvalidValueExceptionEventHandler(this.spinSang_InvalidValue);
                this.spinSang.EditValueChanged -= new System.EventHandler(this.spinSang_EditValueChanged);
                this.spinSang.KeyPress -= new System.Windows.Forms.KeyPressEventHandler(this.spinSang_KeyPress);
                this.spinSang.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.spinSang_PreviewKeyDown);
                this.spinSang.Validating -= new System.ComponentModel.CancelEventHandler(this.spinSang_Validating);

                this.spinTrua.InvalidValue -= new DevExpress.XtraEditors.Controls.InvalidValueExceptionEventHandler(this.spinTrua_InvalidValue);
                this.spinTrua.EditValueChanged -= new System.EventHandler(this.txtChiaLam_EditValueChanged);
                this.spinTrua.KeyPress -= new System.Windows.Forms.KeyPressEventHandler(this.spinTrua_KeyPress);
                this.spinTrua.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.spinTrua_PreviewKeyDown);
                this.spinTrua.Validating -= new System.ComponentModel.CancelEventHandler(this.spinTrua_Validating);

                this.spinChieu.InvalidValue -= new DevExpress.XtraEditors.Controls.InvalidValueExceptionEventHandler(this.spinChieu_InvalidValue);
                this.spinChieu.EditValueChanged -= new System.EventHandler(this.spinChieu_EditValueChanged);
                this.spinChieu.KeyPress -= new System.Windows.Forms.KeyPressEventHandler(this.spinChieu_KeyPress);
                this.spinChieu.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.spinChieu_PreviewKeyDown);
                this.spinChieu.Validating -= new System.ComponentModel.CancelEventHandler(this.spinChieu_Validating);

                this.spinToi.InvalidValue -= new DevExpress.XtraEditors.Controls.InvalidValueExceptionEventHandler(this.spinToi_InvalidValue);
                this.spinToi.EditValueChanged -= new System.EventHandler(this.spinToi_EditValueChanged);
                this.spinToi.KeyPress -= new System.Windows.Forms.KeyPressEventHandler(this.spinToi_KeyPress);
                this.spinToi.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.spinToi_PreviewKeyDown);
                this.spinToi.Validating -= new System.ComponentModel.CancelEventHandler(this.spinToi_Validating);

                this.spinSoLuongNgay.EditValueChanged -= new System.EventHandler(this.spinSoLuongNgay_EditValueChanged);
                this.spinSoLuongNgay.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.spinSoLuongNgay_PreviewKeyDown);

                this.cboHtu.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboHtu_ButtonClick);
                this.cboHtu.Leave -= new System.EventHandler(this.cboHtu_Leave);
                this.cboHtu.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.cboHtu_PreviewKeyDown);

                this.cboMedicineUseForm.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboDuongDung__Medicine_Closed);
                this.cboMedicineUseForm.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboDuongDung__Medicine_ButtonClick);
                this.cboMedicineUseForm.KeyUp -= new System.Windows.Forms.KeyEventHandler(this.cboDuongDung__Medicine_KeyUp);
                this.cboMedicineUseForm.Leave -= new System.EventHandler(this.cboMedicineUseForm_Leave);

                this.txtTutorial.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txtTutorial_ButtonClick);
                this.txtTutorial.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.txtTutorial_KeyDown);
                this.txtTutorial.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtTutorial_PreviewKeyDown);

                this.btnAddTutorial.Click -= new System.EventHandler(this.btnAddTutorial_Click);

                this.spinAmount.InvalidValue -= new DevExpress.XtraEditors.Controls.InvalidValueExceptionEventHandler(this.spinAmount_InvalidValue);
                this.spinAmount.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.spinAmount_KeyDown);
                this.spinAmount.KeyPress -= new System.Windows.Forms.KeyPressEventHandler(this.spinAmount_KeyPress);
                this.spinAmount.Leave -= new System.EventHandler(this.spinAmount_Leave);
                this.spinAmount.Validating -= new System.ComponentModel.CancelEventHandler(this.spinAmount_Validating);

                this.btnAdd.Click -= new System.EventHandler(this.btnAdd_TabMedicine_Click);
                this.chkHomePres.CheckedChanged -= new System.EventHandler(this.chkHomePres_CheckedChanged);
                this.txtIcdMainTextCause.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtIcdMainTextCause_PreviewKeyDown);
                this.cboIcdsCause.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboIcdsCause_Closed);
                this.cboIcdsCause.TextChanged -= new System.EventHandler(this.cboIcdsCause_TextChanged);
                this.cboIcdsCause.KeyUp -= new System.Windows.Forms.KeyEventHandler(this.cboIcdsCause_KeyUp);

                this.chkEditIcdCause.CheckedChanged -= new System.EventHandler(this.chkEditIcdCause_CheckedChanged);
                this.chkEditIcdCause.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.chkEditIcdCause_PreviewKeyDown);

                this.txtIcdCodeCause.InvalidValue -= new DevExpress.XtraEditors.Controls.InvalidValueExceptionEventHandler(this.txtIcdCodeCause_InvalidValue);
                this.txtIcdCodeCause.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtIcdCodeCause_PreviewKeyDown);
                this.txtIcdCodeCause.Validating -= new System.ComponentModel.CancelEventHandler(this.txtIcdCodeCause_Validating);

                this.cboNhaThuoc.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboNhaThuoc_ButtonClick);
                this.cboNhaThuoc.EditValueChanged -= new System.EventHandler(this.cboNhaThuoc_EditValueChanged);

                this.btnDichVuHenKham.Click -= new System.EventHandler(this.btnDichVuHenKham_Click);

                this.cboEquipment.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboEquipment_Closed);
                this.cboEquipment.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboEquipment_ButtonClick);

                this.tooltipService.GetActiveObjectInfo -= new DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventHandler(this.tooltipService_GetActiveObjectInfo);

                this.arrControlEnableNotChange = null;
                this.dicOrderTabIndexControl = null;
                this.serviceReqParentId = null;
                this.expMestTemplateId = null;
                this.treatmentCode = null;
                this.intructionTimeSelecteds = null;
                this.intructionTimeSelected = null;
                this.UseTimeSelecteds = null;
                this.UseTimeSelected = null;
                this.intructionTimeSelectedsForMedi = null;
                this.intructionTimeSelectedForMedi = null;
                this.currentSereServ = null;
                this.currentSereServInEkip = null;
                this.Service__Main = null;
                this.serviceReqMain = null;
                this.processDataResult = null;
                this.processRefeshIcd = null;
                this.processWhileAutoTreatmentEnd = null;
                this.currentTreatmentWithPatientType = null;
                this.currentHisPatientTypeAlter = null;
                this.currentPatientTypeWithPatientTypeAlter = null;
                this.currentPatientTypes = null;
                this.currentMedicineTypes = null;
                this.currentMaterialTypes = null;
                this.medicineTypeTutSelected = null;
                this.materialTypeTutSelected = null;
                this.sereServsInTreatmentRaw = null;
                this.sereServWithTreatment = null;
                this.serviceReqMetyInDay = null;
                this.serviceReqMatyInDay = null;
                this.mediMatyTypeADOBKs = null;
                this.mediMatyTypeADOs = null;
                this.currentMediMateTypeComboADOs = null;
                this.mediMatyTypeAvailables = null;
                this.currentMedicineTypeADOForEdit = null;
                this.mediStockD1ADOs = null;
                this.icdChoose = null;
                this.currentModule = null;
                this.currentWorkPlace = null;
                this.serviceInPackages = null;
                this.servicePackageByServices = null;
                this.outPrescriptionResultSDOs = null;
                this.inPrescriptionResultSDOs = null;
                this.currentPrescriptions = null;
                this.currentMediStock = null;
                this.currentMediStockNhaThuocSelecteds = null;
                this.currentMediStockByHeaderCard = null;
                this.currentMediStockByNotInHeaderCard = null;
                this.currentWorkingMestRooms = null;
                this.assignPrescriptionEditADO = null;
                this.icdExam = null;
                this.lastInfo = null;
                this.lastColumn = null;
                this.oldExpMest = null;
                this.oldServiceReq = null;
                this.ServiceReqEye = null;
                this.listMedicineBeanForEdits = null;
                this.listMaterialBeanForEdits = null;
                this.paramCommon = null;
                if (this.ucPeriousExpMestList != null)
                {
                    this.periousExpMestListProcessor.DisposeControl(ucPeriousExpMestList);
                    this.ucPeriousExpMestList.Dispose();
                    this.ucPeriousExpMestList = null;
                }
                this.periousExpMestListProcessor = null;
                this.treatmentFinishProcessor = null;
                if (this.ucTreatmentFinish != null)
                {
                    this.ucTreatmentFinish.Dispose();
                    this.ucTreatmentFinish = null;
                }
                this.patientSelectProcessor = null;
                if (this.ucPatientSelect != null)
                {
                    this.ucPatientSelect.Dispose();
                    this.ucPatientSelect = null;
                }
                this.resultDataPrescription = null;
                this.treatmentBedRoomLViewFilterInput = null;
                this.currentIcds = null;
                this.trackingADOs = null;
                this.allergenics = null;
                this.serviceReqPrints = null;
                this.serviceReqPrintAlls = null;
                this.expMestPrints = null;
                this.expMestMedicinePrints = null;
                this.expMestMaterialPrints = null;
                this.expMestMedicineEditPrints = null;
                this.expMestMaterialEditPrints = null;
                this.serviceReqMetys = null;
                this.serviceReqMatys = null;
                this.requestRoom = null;
                this.currentDhst = null;
                this.controlStateWorker = null;
                this.currentControlStateRDO = null;
                this.Listtrackings = null;
                this.currentPrescriptionFilter = null;
                this.mediStockAllows = null;
                this.currentAdviseFormADO = null;
                this._Menu = null;
                this.LstEmrCoverConfig = null;
                this.LstEmrCoverConfigDepartment = null;
                this.treatmentData = null;
                this.emrMenuPopupProcessor = null;
                this.workingServiceConditions = null;
                this.serviceMetyByServices = null;
                this.serviceMatyByServices = null;
                this.icdSubcodeAdoChecks = null;
                this.subIcdPopupSelect = null;
                this.downHitInfo = null;
                this.VHistreatment = null;
                this.epaymentDepositResultSDO = null;
                this.lstDepartmentTran = null;
                this.lstCoTreatment = null;
                this.paramSaveList = null;

                AssignPrescriptionWorker.DisposeInstance();


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


    }
}
