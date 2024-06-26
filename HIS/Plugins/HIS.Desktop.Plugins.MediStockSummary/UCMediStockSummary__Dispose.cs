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
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Print;
using MOS.EFMODEL.DataModels;
using System;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.MediStockSummary
{
    public partial class UCMediStockSummary : HIS.Desktop.Utility.UserControlBase
    {
        public override void ProcessDisposeModuleDataAfterClose()
        {
            try
            {
                isCheckAll = false;
                _MediStocks = null;
                HisMediStockMetyByStocks = null;
                HisMediStockMety = null;
                HisMediStockMatyByStocks = null;
                HisMediStockMaty = null;
                fileName = null;
                mediFilter = null;
                materialTypeAdo = null;
                medicineTypeAdo = null;
                RoomTypeId = 0;
                RoomId = 0;
                moduleData = null;
                isCheck = false;
                mateStockAdo = null;
                mediStockAdo = null;
                dXmenuItem = null;
                mediStockIds = null;
                currentMediStock = null;
                hisBloodProcessor.DisposeControl(ucBloodInfo);
                hisMateInStockProcessor.DisposeControl(ucMaterialInfo);
                hisMediInStockProcessor.DisposeControl(ucMedicineInfo);
                ucBloodInfo = null;
                ucMaterialInfo = null;
                ucMedicineInfo = null;
                hisBloodProcessor = null;
                hisMateInStockProcessor = null;
                hisMediInStockProcessor = null;
                lstBlood = null;
                lstBloodInStocks = null;
                lstMateInStocks = null;
                lstMediInStocks = null;
                dicMaterials = null;
                dicMedicines = null;
                this.cboIsActive.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboIsActive_ButtonClick);
                this.cboIsActive.EditValueChanged -= new System.EventHandler(this.cboIsActive_EditValueChanged);
                this.ChkValidToTime.CheckedChanged -= new System.EventHandler(this.ChkValidToTime_CheckedChanged);
                this.cboPatientType.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboPatientType_ButtonClick);
                this.cboPatientType.EditValueChanged -= new System.EventHandler(this.cboPatientType_EditValueChanged);
                this.ChkNoExpiredDate.CheckedChanged -= new System.EventHandler(this.ChkNoExpiredDate_CheckedChanged);
                this.ChkExpiredDate.CheckedChanged -= new System.EventHandler(this.ChkExpiredDate_CheckedChanged);
                this.chkAlertMinStock.CheckedChanged -= new System.EventHandler(this.chkAlertMinStock_CheckedChanged);
                this.btnXuatExcel.Click -= new System.EventHandler(this.btnXuatExcel_Click);
                this.btnPrint.Click -= new System.EventHandler(this.btnPrint_Click);
                this.chkBlood.CheckedChanged -= new System.EventHandler(this.chkBlood_CheckedChanged);
                this.chkMaterial.CheckedChanged -= new System.EventHandler(this.chkMaterial_CheckedChanged);
                this.chkMedicine.CheckedChanged -= new System.EventHandler(this.chkMedicine_CheckedChanged);
                this.dtExpiredDate.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.dtExpiredDate_Closed);
                this.txtKeyWork.KeyUp -= new System.Windows.Forms.KeyEventHandler(this.txtKeyWork_KeyUp);
                this.btnSearch.Click -= new System.EventHandler(this.btnSearch_Click_1);
                this.btnRefesh.Click -= new System.EventHandler(this.btnRefesh_Click_1);
                this.gridViewMediStock.CustomUnboundColumnData -= new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewMediStock_CustomUnboundColumnData);
                this.gridViewMediStock.MouseDown -= new System.Windows.Forms.MouseEventHandler(this.gridViewMediStock_MouseDown);
                this.Load -= new System.EventHandler(this.UCMediStockSummary_Load);
                gridView1.GridControl.DataSource = null;
                cboIsActive.Properties.DataSource = null;
                gridLookUpEdit1View.GridControl.DataSource = null;
                cboPatientType.Properties.DataSource = null;
                gridViewMediStock.GridControl.DataSource = null;
                gridControlMediStock.DataSource = null;
                layoutControlItem22 = null;
                gridView1 = null;
                cboIsActive = null;
                emptySpaceItem2 = null;
                layoutControlItem21 = null;
                layoutControlItem20 = null;
                ChkValidToTime = null;
                dtValidToTime = null;
                layoutControlItem19 = null;
                gridLookUpEdit1View = null;
                cboPatientType = null;
                layoutControlItem18 = null;
                layoutControlItem17 = null;
                layoutControlItem16 = null;
                dtExpiredDate = null;
                ChkExpiredDate = null;
                ChkNoExpiredDate = null;
                layoutControlItem15 = null;
                chkAlertMinStock = null;
                imageListIcon = null;
                repositoryItemCheck_D = null;
                repositoryItemCheck_E = null;
                gridColumnCheck = null;
                layoutControlItem14 = null;
                txtKeyWork = null;
                layoutControlItem10 = null;
                chkShowLineZero = null;
                saveFileDialog = null;
                layoutControlItem7 = null;
                btnXuatExcel = null;
                layoutControlItem13 = null;
                layoutControlItem12 = null;
                layoutControlItem11 = null;
                btnRefesh = null;
                btnSearch = null;
                layoutControlGroup3 = null;
                layoutControl4 = null;
                layoutControlItem6 = null;
                btnPrint = null;
                layoutControlItem9 = null;
                layoutControlItem8 = null;
                chkMaterial = null;
                chkBlood = null;
                layoutControlItem3 = null;
                emptySpaceItem3 = null;
                chkMedicine = null;
                imageCollection1 = null;
                layoutControlItem5 = null;
                panelControlMediMate = null;
                layoutControlItem4 = null;
                gridColumn4 = null;
                gridColumn3 = null;
                gridColumn2 = null;
                gridColumn1 = null;
                gridViewMediStock = null;
                gridControlMediStock = null;
                layoutControlItem2 = null;
                layoutControlItem1 = null;
                layoutControlGroup1 = null;
                Root = null;
                layoutControl2 = null;
                layoutControlGroup2 = null;
                layoutControl3 = null;
                layoutControl1 = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
