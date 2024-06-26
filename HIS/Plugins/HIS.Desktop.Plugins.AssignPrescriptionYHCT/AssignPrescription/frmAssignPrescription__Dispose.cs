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
using DevExpress.XtraEditors.DXErrorProvider;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.AssignPrescriptionYHCT.ADO;
using HIS.Desktop.Plugins.AssignPrescriptionYHCT.Config;
using HIS.Desktop.Plugins.AssignPrescriptionYHCT.Resources;
using HIS.Desktop.Plugins.AssignPrescriptionYHCT.Util;
using HIS.Desktop.Utilities.Extensions;
using HIS.UC.Icd.ADO;
using HIS.UC.SecondaryIcd.ADO;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignPrescriptionYHCT.AssignPrescription
{
    public partial class frmAssignPrescription : HIS.Desktop.Utility.FormBase
    {
        public override void ProcessDisposeModuleDataAfterClose()
        {
            try
            {
                this.arrControlEnableNotChange = null;
                this.dicOrderTabIndexControl = null;
                this.serviceReqParentId = null;
                this.treatmentCode = null;
                this.intructionTimeSelecteds = null;
                this.currentSereServ = null;
                this.currentSereServInEkip = null;
                this.Service__Main = null;
                this.processDataResult = null;
                this.processRefeshIcd = null;
                this.processWhileAutoTreatmentEnd = null;
                this.currentTreatmentWithPatientType = null;
                this.currentHisPatientTypeAlter = null;
                this.currentPatientTypeWithPatientTypeAlter = null;
                this.currentPatientTypes = null;
                this.medicineTypeTutSelected = null;
                this.sereServWithTreatment = null;
                this.mediMatyTypeADOs = null;
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
                this.currentMediStock = null;
                this.currentMediStockNhaThuocSelecteds = null;
                this.currentMediStockByHeaderCard = null;
                this.currentMediStockByNotInHeaderCard = null;
                this.assignPrescriptionEditADO = null;
                this.icdExam = null;
                this.lastInfo = null;
                this.lastColumn = null;
                this.oldExpMest = null;
                this.oldServiceReq = null;
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
                this.trackingADOs = null;
                this.allergenics = null;
                this.serviceReqPrints = null;
                this.expMestPrints = null;
                this.expMestMedicinePrints = null;
                this.expMestMaterialPrints = null;
                this.expMestMedicineEditPrints = null;
                this.expMestMaterialEditPrints = null;
                this.serviceReqMetys = null;
                this.serviceReqMatys = null;
                this.requestRoom = null;
                this.controlStateWorker = null;
                this.currentControlStateRDO = null;
                this.currentPrescriptionFilter = null;
                this.mediStockAllows = null;
                this.paramSaveList = null;

                this.gridViewMediMaty.RowClick -= new DevExpress.XtraGrid.Views.Grid.RowClickEventHandler(this.gridViewMediMaty_RowClick);
                this.gridViewMediMaty.CustomDrawCell -= new DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventHandler(this.OnCustomDrawCell);
                this.gridViewMediMaty.RowCellStyle -= new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gridViewMediMaty_RowCellStyle);
                this.gridViewMediMaty.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.gridViewMediMaty_KeyDown);
                this.popupControlContainerMediMaty.CloseUp -= new System.EventHandler(this.popupControlContainerMediMaty_CloseUp);

                this.cboExpMestReason.Properties.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboExpMestReason_Properties_ButtonClick);

                this.gridControlServiceProcess.DataSourceChanged -= new System.EventHandler(this.gridControlServiceProcess_DataSourceChanged);
                this.gridControlServiceProcess.ProcessGridKey -= new System.Windows.Forms.KeyEventHandler(this.gridControlServiceProcess_ProcessGridKey);
                this.gridControlServiceProcess.DoubleClick -= new System.EventHandler(this.gridControlServiceProcess_DoubleClick);

                this.gridViewServiceProcess.RowCellStyle -= new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gridViewServiceProcess_RowCellStyle);
                this.gridViewServiceProcess.CustomRowCellEdit -= new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gridViewServiceProcess_CustomRowCellEdit);
                this.gridViewServiceProcess.ShowingEditor -= new System.ComponentModel.CancelEventHandler(this.gridViewServiceProcess_ShowingEditor);
                this.gridViewServiceProcess.ShownEditor -= new System.EventHandler(this.gridViewServiceProcess_ShownEditor);
                this.gridViewServiceProcess.CellValueChanged -= new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gridViewServiceProcess_CellValueChanged);
                this.gridViewServiceProcess.CustomUnboundColumnData -= new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewServiceProcess_CustomUnboundColumnData);
                this.gridViewServiceProcess.MouseDown -= new System.Windows.Forms.MouseEventHandler(this.gridViewServiceProcess_MouseDown);

                this.cboMedicineUseForm.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboDuongDung__Medicine_Closed);
                this.cboMedicineUseForm.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboDuongDung__Medicine_ButtonClick);
                this.cboMedicineUseForm.KeyUp -= new System.Windows.Forms.KeyEventHandler(this.cboDuongDung__Medicine_KeyUp);
                this.cboMedicineUseForm.Leave -= new System.EventHandler(this.cboMedicineUseForm_Leave);

                this.spinAmount.KeyPress -= new System.Windows.Forms.KeyPressEventHandler(this.spinAmount_KeyPress);
                this.spinAmount.Validating -= new System.ComponentModel.CancelEventHandler(this.spinAmount_Validating);

                this.btnAdd.Click -= new System.EventHandler(this.btnAdd_TabMedicine_Click);

                this.cboNhaThuoc.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboNhaThuoc_ButtonClick);
                this.cboNhaThuoc.EditValueChanged -= new System.EventHandler(this.cboNhaThuoc_EditValueChanged);

                this.btnDichVuHenKham.Click -= new System.EventHandler(this.btnDichVuHenKham_Click);

                this.tooltipService.GetActiveObjectInfo -= new DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventHandler(this.tooltipService_GetActiveObjectInfo);
                this.dxValidationProviderControl.ValidationFailed -= new DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventHandler(this.dxValidationProviderControl_ValidationFailed);
                this.gridControlOtherPaySource.Click -= new System.EventHandler(this.gridControlOtherPaySource_Click);
                this.gridViewOtherPaySource.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.gridViewOtherPaySource_KeyDown);

                this.cboPhieuDieuTri.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboPhieuDieuTri_Closed);
                this.cboPhieuDieuTri.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboPhieuDieuTri_ButtonClick);

                this.txtMediMatyForPrescription.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txtMediMatyForPrescription_ButtonClick);
                this.txtMediMatyForPrescription.TextChanged -= new System.EventHandler(this.txtMediMatyForPrescription_TextChanged);
                this.txtMediMatyForPrescription.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.txtMediMatyForPrescription_KeyDown);
                this.rdOpionGroup.SelectedIndexChanged -= new System.EventHandler(this.rdOpionGroup_SelectedIndexChanged);

                this.txtUnitOther.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtUnitOther_PreviewKeyDown);
                this.txtMedicineTypeOther.Leave -= new System.EventHandler(this.txtMedicineTypeOther_Leave);
                this.txtMedicineTypeOther.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtMedicineTypeOther_PreviewKeyDown);

                this.spinAmount.EditValueChanged -= new System.EventHandler(this.spinAmount_EditValueChanged);
                this.spinAmount.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.spinAmount__MedicinePage_KeyDown);
                this.txtLoginName.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtLoginName_PreviewKeyDown);

                this.spinSoNgay.EditValueChanged -= new System.EventHandler(this.spinSoNgay_EditValueChanged);
                this.spinSoNgay.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.spinSoNgay_PreviewKeyDown);

                this.txtAdvise.KeyUp -= new System.Windows.Forms.KeyEventHandler(this.txtLoiDanBacSi_KeyUp);

                this.txtLadder.Leave -= new System.EventHandler(this.txtLadder_Leave);
                this.txtLadder.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtLadder_PreviewKeyDown);

                this.cboExpMestTemplate.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboTemplate_Medicine_Closed);
                this.cboExpMestTemplate.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboTemplate_Medicine_ButtonClick);
                this.cboExpMestTemplate.KeyUp -= new System.Windows.Forms.KeyEventHandler(this.cboTemplate_Medicine_KeyUp);
                this.cboExpMestTemplate.Leave -= new System.EventHandler(this.cboExpMestTemplate_Leave);

                this.txtExpMestTemplateCode.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtTemplate_Medicine_PreviewKeyDown);

                this.cboUser.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboUser_Closed);
                this.cboUser.EditValueChanged -= new System.EventHandler(this.cboUser_EditValueChanged);
                this.cboUser.KeyUp -= new System.Windows.Forms.KeyEventHandler(this.cboUser_KeyUp);

                this.txtHuongDan.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.txtHuongDan_KeyDown);
                this.cboExpMestReason.KeyUp -= new System.Windows.Forms.KeyEventHandler(this.cboExpMestReason_KeyUp);

                this.dxValidProviderBoXung.ValidationFailed -= new DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventHandler(this.dxValidProviderBoXung__MedicinePage_ValidationFailed);
                this.dxValidProviderBoXung__DuongDung.ValidationFailed -= new DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventHandler(this.dxValidProviderBoXung__DuongDung_ValidationFailed);

                AssignPrescriptionWorker.DisposeInstance();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
