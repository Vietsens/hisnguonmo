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
using HIS.Desktop.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.InfusionCreate
{
    public partial class frmInfusionCreate : FormBase
    {
        public override void ProcessDisposeModuleDataAfterClose()
        {
            try
            {
                idCombo = 0;
                infusion = null;
                treatment = null;
                flat = 0;
                IDUpdate = 0;
                ActionType = 0;
                positionHandleControl = 0;
                _ConfigADO = null;
                currentConfigAppUser = null;
                _currentConfigApp = null;
                moduleLink = null;
                currentControlStateRDO = null;
                controlStateWorker = null;
                isNotLoadWhileChangeControlStateInFirst = false;
                glstSpeedUnit = null;
                lstExpMestMedicine6 = null;
                lstAdoTemp = null;
                lstAdo = null;
                lstEmrDocumentStt = null;
                glstMediTypeSort = null;
                glstMediType = null;
                acsUsers = null;
                glstMedicine = null;
                glstSereServ = null;
                glstServiceReq = null;
                thissereServ = null;
                listInfusionsSelected = null;
                ListInfusion = null;
                data = null;
                moduleData = null;
                param = null;
                serviceUnitProcessor = null;
                this.barButtonItem1.ItemClick -= new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem1_ItemClick);
                this.barButtonItem2.ItemClick -= new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem2_ItemClick);
                this.chkSignForPrint.CheckedChanged -= new System.EventHandler(this.chkSignForPrint_CheckedChanged);
                this.btnCboPrint.Click -= new System.EventHandler(this.btnCboPrint_Click);
                this.gridView1.RowCellClick -= new DevExpress.XtraGrid.Views.Grid.RowCellClickEventHandler(this.gridView1_RowCellClick);
                this.gridView1.CustomUnboundColumnData -= new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridView1_CustomUnboundColumnData);
                this.GridLookUpEdit_SelectMedicine.Click -= new System.EventHandler(this.repositoryItemButtonEdit1_Click);
                this.grdInfusionMedicine.ProcessGridKey -= new System.Windows.Forms.KeyEventHandler(this.grdInfusionMedicine_ProcessGridKey);
                this.grvInfusionMedicine.CustomRowCellEdit -= new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.grvInfusionMedicine_CustomRowCellEdit);
                this.grvInfusionMedicine.ShownEditor -= new System.EventHandler(this.grvInfusionMedicine_ShownEditor);
                this.grvInfusionMedicine.FocusedColumnChanged -= new DevExpress.XtraGrid.Views.Base.FocusedColumnChangedEventHandler(this.grvInfusionMedicine_FocusedColumnChanged);
                this.grvInfusionMedicine.CellValueChanged -= new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.grvInfusionMedicine_CellValueChanged);
                this.grvInfusionMedicine.CustomUnboundColumnData -= new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.grvInfusionMedicine_CustomUnboundColumnData);
                this.cboSelectMedicine.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboSelectMedicine_Closed);
                this.TextEdit__TenThuoc.EditValueChanged -= new System.EventHandler(this.TextEdit__TenThuoc_EditValueChanged);
                this.TextEdit__SoLo.EditValueChanged -= new System.EventHandler(this.TextEdit__SoLo_EditValueChanged);
                this.TextEdit__DungTich.EditValueChanged -= new System.EventHandler(this.TextEdit__DungTich_EditValueChanged);
                this.TextEdit__DungTich.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.TextEdit__DungTich_KeyDown);
                this.TextEdit__DungTich.KeyPress -= new System.Windows.Forms.KeyPressEventHandler(this.TextEdit__DungTich_KeyPress);
                this.TextEdit__SoLuong.EditValueChanged -= new System.EventHandler(this.TextEdit__SoLuong_EditValueChanged);
                this.TextEdit__SoLuong.KeyPress -= new System.Windows.Forms.KeyPressEventHandler(this.TextEdit__SoLuong_KeyPress);
                this.btnAdd.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.btnAdd_ButtonClick);
                this.btnDelete.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.btnDelete_ButtonClick);
                this.dtDateInfusion.EditValueChanged -= new System.EventHandler(this.dtDateInfusion_EditValueChanged);
                this.chkSign.CheckedChanged -= new System.EventHandler(this.chkSign_CheckedChanged);
                this.spinEditConvertVolumnRatio.EditValueChanged -= new System.EventHandler(this.spinEditConvertVolumnRatio_EditValueChanged);
                this.spinEditConvertVolumnRatio.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.spinEditConvertVolumnRatio_KeyDown);
                this.spinEditVolumn.EditValueChanged -= new System.EventHandler(this.spinEditVolumn_EditValueChanged);
                this.spinEditVolumn.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.spinEditVolumn_KeyDown);
                this.cboSpeedUnit.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboSpeedUnit_Closed);
                this.cboSpeedUnit.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboSpeedUnit_ButtonClick);
                this.cboSpeedUnit.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.cboSpeedUnit_KeyDown);
                this.dtNgayChidinh.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.dtNgayChidinh_Closed);
                this.dtNgayChidinh.TextChanged -= new System.EventHandler(this.dtNgayChidinh_TextChanged);
                this.dtNgayChidinh.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.dtNgayChidinh_KeyDown);
                this.dtExpiredDate.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.dtExpiredDate_KeyDown);
                this.txtPackageNumber.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.txtPackageNumber_KeyDown);
                this.txtMedicine.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.txtMedicine_KeyDown);
                this.txtNote.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.txtNote_KeyDown);
                this.cboExeUsername.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboExeUsername_Closed);
                this.cboReqUsername.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboReqUsername_Closed);
                this.txtMedicinetype.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtMedicinetype_PreviewKeyDown);
                this.spinSpeed.EditValueChanged -= new System.EventHandler(this.spinSpeed_EditValueChanged);
                this.spinSpeed.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.spinSpeed_KeyDown);
                this.dtEndTime.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.dateEdit2_KeyDown);
                this.dtStartTime.EditValueChanged -= new System.EventHandler(this.dtStartTime_EditValueChanged);
                this.dtStartTime.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.dateEdit1_KeyDown);
                this.spinAmount.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.spinAmount_KeyDown);
                this.btnRefesh.Click -= new System.EventHandler(this.btnRefesh_Click);
                this.btnSave.Click -= new System.EventHandler(this.btnSave_Click);
                this.btnSave.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.btnSave_KeyDown);
                this.cboMedicineType.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboMedicineType_Closed);
                this.cboMedicineType.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboMedicineType_ButtonClick);
                this.cboMedicineType.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.cboMedicineType_KeyDown);
                this.dxValidationProvider1.ValidationFailed -= new DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventHandler(this.dxValidationProvider1_ValidationFailed);
                this.Load -= new System.EventHandler(this.frmInfusionCreate_Load);
                customGridView1.GridControl.DataSource = null;
                GU_Medicine.DataSource = null;
                repositoryItemCustomGridLookUpEdit1View.GridControl.DataSource = null;
                cboSelectMedicine.DataSource = null;
                grvInfusionMedicine.GridControl.DataSource = null;
                grdInfusionMedicine.DataSource = null;
                gridView3.GridControl.DataSource = null;
                cboSpeedUnit.Properties.DataSource = null;
                gridView2.GridControl.DataSource = null;
                cboReqUsername.Properties.DataSource = null;
                gridLookUpEdit2View.GridControl.DataSource = null;
                cboExeUsername.Properties.DataSource = null;
                gridLookUpEdit1View.GridControl.DataSource = null;
                cboMedicineType.Properties.DataSource = null;
                gridView1.GridControl.DataSource = null;
                gridControl1.DataSource = null;
                gridColumn14 = null;
                gridColumn13 = null;
                gridColumn12 = null;
                TextEdit__DVTDisable = null;
                TextEdit__DVT = null;
                TextEdit__SoLuongDisable = null;
                TextEdit__DungTichDisable = null;
                TextEdit__SoLuong = null;
                grdColServiceUnitName = null;
                grdColAmount = null;
                gridColumn11 = null;
                emptySpaceItem2 = null;
                gridColumn10 = null;
                gridColumn9 = null;
                TextEdit__SoLo = null;
                TextEdit__TenThuoc = null;
                TextEdit__DungTich = null;
                customGridView1 = null;
                GU_Medicine = null;
                gridColumn8 = null;
                btnDelete = null;
                repositoryItemCustomGridLookUpEdit1View = null;
                cboSelectMedicine = null;
                btnAdd = null;
                dtDateInfusion = null;
                gridColumn7 = null;
                gridColumn6 = null;
                gridColumn5 = null;
                gridColumn4 = null;
                gridColumn3 = null;
                layoutControlItem32 = null;
                layoutControlItem31 = null;
                layoutControlItem30 = null;
                layoutControlGroup2 = null;
                layoutControl3 = null;
                grvInfusionMedicine = null;
                grdInfusionMedicine = null;
                layoutControlItem29 = null;
                layoutControlItem28 = null;
                chkSignForPrint = null;
                chkPrintDocumentSignedForPrint = null;
                layoutControlItem27 = null;
                btnCboPrint = null;
                emptySpaceItem3 = null;
                MixedMedicine = null;
                layoutControlItem26 = null;
                layoutControlItem25 = null;
                layoutControlItem24 = null;
                chkPrint = null;
                chkSign = null;
                chkPrintDocumentSigned = null;
                dxErrorProvider1 = null;
                ConvertVolumn = null;
                ConvertTime = null;
                Volumn = null;
                SpeedUnit = null;
                layoutControlItem23 = null;
                layoutControlItem22 = null;
                layoutControlItem21 = null;
                gridView3 = null;
                cboSpeedUnit = null;
                spinEditVolumn = null;
                spinEditConvertVolumnRatio = null;
                layoutControlItem20 = null;
                dtNgayChidinh = null;
                layoutControlItem19 = null;
                panelControlUCUnitService = null;
                gridColumn2 = null;
                gridColumn1 = null;
                layoutControlItem18 = null;
                dtExpiredDate = null;
                layoutControlItem17 = null;
                layoutControlItem9 = null;
                txtMedicine = null;
                txtPackageNumber = null;
                layoutControlItem10 = null;
                txtNote = null;
                layoutControlItem16 = null;
                layoutControlItem13 = null;
                gridView2 = null;
                cboReqUsername = null;
                gridLookUpEdit2View = null;
                cboExeUsername = null;
                layoutControlItem2 = null;
                txtMedicinetype = null;
                gridLookUpEdit1View = null;
                cboMedicineType = null;
                emptySpaceItem1 = null;
                dxValidationProvider1 = null;
                barButtonItem2 = null;
                bar2 = null;
                barButtonItem1 = null;
                bar1 = null;
                layoutControlItem8 = null;
                spinSpeed = null;
                layoutControlItem12 = null;
                layoutControlItem11 = null;
                dtStartTime = null;
                dtEndTime = null;
                layoutControlItem7 = null;
                spinAmount = null;
                layoutControlItem5 = null;
                layoutControlItem1 = null;
                layoutControlGroup1 = null;
                layoutControlItem4 = null;
                layoutControlItem3 = null;
                Root = null;
                layoutControlItem15 = null;
                layoutControlItem14 = null;
                layoutControlItem6 = null;
                layoutControlGroup3 = null;
                btnSave = null;
                btnRefesh = null;
                layoutControl4 = null;
                layoutControlGroup4 = null;
                FinishTime = null;
                StartTime = null;
                Executor = null;
                RequestUsername = null;
                Speed = null;
                Amount = null;
                Unit = null;
                MedicineTypeName = null;
                MedicineTypeCode = null;
                GridLookUpEdit_SelectMedicine = null;
                Delete = null;
                STT = null;
                gridView1 = null;
                gridControl1 = null;
                layoutControl5 = null;
                layoutControl2 = null;
                layoutControl1 = null;
                barDockControlRight = null;
                barDockControlLeft = null;
                barDockControlBottom = null;
                barDockControlTop = null;
                barManager1 = null;
                if (this.ucServiceUnit != null)
                {
                    this.ucServiceUnit.Dispose();
                    this.ucServiceUnit = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
