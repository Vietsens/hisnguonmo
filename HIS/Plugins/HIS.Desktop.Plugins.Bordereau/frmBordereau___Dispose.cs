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
using DevExpress.Utils.Menu;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.Bordereau.Base;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
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

namespace HIS.Desktop.Plugins.Bordereau
{
    public partial class frmBordereau : FormBase
    {
        public override void ProcessDisposeModuleDataAfterClose()
        {
            try
            {
                moduleLink = null;
                currentControlStateRDO = null;
                controlStateWorker = null;
                menu = null;
                MaterialList = null;
                MedicineList = null;
                serviceReqs = null;
                IsAllowNoExecuteForPaid = false;
                IsSetPrimaryPatientType = false;
                cboPayTypeDefault = null;
                AutoClosePrintAndForm = false;
                AllowCheckIsNoExecute = false;
                numOrder = null;
                equipmentId = null;
                currentDepartment = null;
                DepartmentPremissionEdits = null;
                currentModule = null;
                SereServBills = null;
                SereServADOs = null;
                statusTreatmentOut = null;
                userNameReturnResult = null;
                totalDayTreatment = 0;
                rooms = null;
                treatmentFees = null;
                departmentTrans = null;
                patient = null;
                packages = null;
                OtherPaySources = null;
                sereServIsPackages = null;
                PatientTypes = null;
                Services = null;
                Departments = null;
                ServiceTypes = null;
                currentPatientTypeWithPatientTypeAlter = null;
                currentHisPatientTypeAlters = null;
                updatePatientType = null;
                currentDepartmentId = 0;
                sereServDeposits = null;
                currentTreatment = null;
                treatmentId = 0;
                isReturningBackOldValue = false;
                lastInfo = null;
                lastRowHandle = 0;
                shareCount = null;
                otherPaySourceChoose = null;
                dicOtherPaySource = null;
                primariPatientType = null;
                patientTypeChoose = null;
                sereServChooseService = null;
                this.cboPayType.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboPayType_Closed);
                this.cboPayType.EditValueChanged -= new System.EventHandler(this.cboPayType_EditValueChanged);
                this.cboPayType.KeyUp -= new System.Windows.Forms.KeyEventHandler(this.cboPayType_KeyUp);
                this.chkAssignBlood.CheckedChanged -= new System.EventHandler(this.chkAssignBlood_CheckedChanged);
                this.chkAmount.CheckedChanged -= new System.EventHandler(this.chkAmount_CheckedChanged);
                this.cboHSBA.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboHSBA_Closed);
                this.btnFind.Click -= new System.EventHandler(this.btnFind_Click);
                this.btnFind.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.btnFind_KeyDown);
                this.txtKeyword.EditValueChanged -= new System.EventHandler(this.txtKeyword_EditValueChanged);
                this.txtKeyword.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.txtKeyword_KeyDown);
                this.txtKeyword.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtKeyword_PreviewKeyDown);
                this.txtLoginName.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtLoginName_PreviewKeyDown);
                this.cboLoginName.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboLoginName_Closed);
                this.cboLoginName.KeyUp -= new System.Windows.Forms.KeyEventHandler(this.cboLoginName_KeyUp);
                this.btnPrint.Click -= new System.EventHandler(this.btnPrint_Click);
                this.gridViewBordereau.RowCellClick -= new DevExpress.XtraGrid.Views.Grid.RowCellClickEventHandler(this.gridViewBordereau_RowCellClick);
                this.gridViewBordereau.RowCellStyle -= new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gridViewBordereau_RowCellStyle);
                this.gridViewBordereau.CustomRowCellEdit -= new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gridViewBordereau_CustomRowCellEdit);
                this.gridViewBordereau.PopupMenuShowing -= new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(this.gridViewBordereau_PopupMenuShowing);
                this.gridViewBordereau.SelectionChanged -= new DevExpress.Data.SelectionChangedEventHandler(this.gridViewBordereau_SelectionChanged);
                this.gridViewBordereau.ShownEditor -= new System.EventHandler(this.gridViewBordereau_ShownEditor);
                this.gridViewBordereau.CellValueChanged -= new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gridViewBordereau_CellValueChanged);
                this.gridViewBordereau.ShowFilterPopupListBox -= new DevExpress.XtraGrid.Views.Grid.FilterPopupListBoxEventHandler(this.gridViewBordereau_ShowFilterPopupListBox);
                this.gridViewBordereau.CustomUnboundColumnData -= new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewBordereau_CustomUnboundColumnData);
                this.gridViewBordereau.MouseDown -= new System.Windows.Forms.MouseEventHandler(this.gridViewBordereau_MouseDown);
                this.repositoryItemGridLookUpEdit_PatientType.EditValueChanged -= new System.EventHandler(this.repositoryItemGridLookUpEdit_PatientType_EditValueChanged);
                this.repositoryItemLookUpEditSereServPackage_Enabled.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.repositoryItemLookUpEditSereServPackage_Enabled_Closed);
                this.repositoryItemButtonEditEquipment.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repositoryItemButtonEditEquipment_ButtonClick);
                this.repositoryItembtnEditDonGia_TextDisable.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repositoryItembtnEditDonGia_TextDisable_ButtonClick);
                this.repositoryItembtnEditGiaGoi_TextDisable.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repositoryItembtnEditGiaGoi_TextDisable_ButtonClick);
                this.repositoryItemGridLookUpEdit_OtherPaySource.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repositoryItemGridLookUpEdit_OtherPaySource_ButtonClick);
                this.repositoryItemGridLookUpEdit_OtherPaySource.EditValueChanged -= new System.EventHandler(this.repositoryItemGridLookUpEdit_OtherPaySource_EditValueChanged);
                this.barButtonItemFind.ItemClick -= new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItemFind_ItemClick);
                this.timer1.Tick -= new System.EventHandler(this.timer1_Tick);
                this.toolTipController1.GetActiveObjectInfo -= new DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventHandler(this.toolTipController1_GetActiveObjectInfo);
                this.Load -= new System.EventHandler(this.frmBordereau_Load);
                gridView8.GridControl.DataSource = null;
                cboHSBA.Properties.DataSource = null;
                gridView7.GridControl.DataSource = null;
                cboPayType.Properties.DataSource = null;
                gridView6.GridControl.DataSource = null;
                repositoryItemGridLookUpEdit_OtherPaySource_Disable.DataSource = null;
                gridView5.GridControl.DataSource = null;
                repositoryItemGridLookUpEdit_OtherPaySource.DataSource = null;
                gridView4.GridControl.DataSource = null;
                repositoryItemGridLookUpEditPrimaryPatientType_Disabled.DataSource = null;
                gridView3.GridControl.DataSource = null;
                repositoryItemGridLookUpEditPrimaryPatientType.DataSource = null;
                gridView2.GridControl.DataSource = null;
                repositoryItemGridLookUpEditEquipment_Disabled.DataSource = null;
                gridView1.GridControl.DataSource = null;
                repositoryItemGridLookUpEditEquipment.DataSource = null;
                repositoryItemLookUpEditSereServPackage_Enabled.DataSource = null;
                repositoryItemLookUpEditSereServPackage_Disabled.DataSource = null;
                repositoryItemLookUpEdit_PatientType_Disable.DataSource = null;
                gridLookUpEdit1View.GridControl.DataSource = null;
                cboLoginName.Properties.DataSource = null;
                repositoryItemGridLookUpEdit1View.GridControl.DataSource = null;
                repositoryItemGridLookUpEdit_PatientType.DataSource = null;
                repositoryItemLookUpEdit_DiKemPTTT.DataSource = null;
                repositoryItemLookUpEdit_PatientType.DataSource = null;
                gridViewBordereau.GridControl.DataSource = null;
                gridControlBordereau.DataSource = null;
                emptySpaceItem1 = null;
                layoutControlItem11 = null;
                layoutControlItem10 = null;
                chkAmount = null;
                chkAssignBlood = null;
                emptySpaceItem2 = null;
                layoutControlItem32 = null;
                layoutControlItem31 = null;
                layoutControlItem30 = null;
                layoutControlItem29 = null;
                layoutControlItem28 = null;
                layoutControlItem27 = null;
                lblTreatmentCode = null;
                lblPatientName = null;
                lblAddress = null;
                lblDob = null;
                lblPatientType = null;
                lblFundName = null;
                layoutControlItem26 = null;
                lblCt = null;
                layoutControlItem25 = null;
                lblMri = null;
                layoutControlItem24 = null;
                lblXquang = null;
                layoutControlItem23 = null;
                layoutControlItem18 = null;
                labelControl1 = null;
                labelControl2 = null;
                gridColumnIsNotUseBHYT = null;
                repositoryItemCheckEditIsNotUseBHYT_Disable = null;
                repositoryItemCheckEditIsNotUseBHYT = null;
                layoutControlItem21 = null;
                gridView8 = null;
                cboHSBA = null;
                lciPayType = null;
                gridView7 = null;
                cboPayType = null;
                gridColumn18 = null;
                gridCol_Package = null;
                gridColumn17 = null;
                gridView6 = null;
                repositoryItemGridLookUpEdit_OtherPaySource_Disable = null;
                gridView5 = null;
                repositoryItemGridLookUpEdit_OtherPaySource = null;
                gridColumnOtherPaySource = null;
                toolTipController1 = null;
                gridColumn16 = null;
                gridColumn15 = null;
                timer1 = null;
                repositoryItemchkIsFundAccepted = null;
                gridColIsFundAccepted = null;
                gridColumn14 = null;
                repositoryItemTxtReadOnly = null;
                repositoryItembtnEditGiaGoi_TextDisable = null;
                repositoryItembtnEditDonGia_TextDisable = null;
                gridColumn13 = null;
                gridColumn12 = null;
                repositoryItem_expend_type_id_disable = null;
                repositoryItem_expend_type_id_enable = null;
                grcExpendTypeId = null;
                gridView4 = null;
                repositoryItemGridLookUpEditPrimaryPatientType_Disabled = null;
                gridView3 = null;
                repositoryItemGridLookUpEditPrimaryPatientType = null;
                gridColumnDoiTuongPT = null;
                gridColumnEquipmentId = null;
                repositoryItemButtonEditEquipment_Disabled = null;
                repositoryItemButtonEditEquipment = null;
                gridView2 = null;
                repositoryItemGridLookUpEditEquipment_Disabled = null;
                gridView1 = null;
                repositoryItemGridLookUpEditEquipment = null;
                gridColumnEquipment = null;
                repositoryItemSpinEditShareCount_Disabled = null;
                repositoryItemSpinEditShareCount = null;
                gridColumnShareCount = null;
                barButtonItemFind = null;
                bar1 = null;
                barManager2 = null;
                barDockControl1 = null;
                barDockControl2 = null;
                barDockControl4 = null;
                barDockControl3 = null;
                layoutControlItem17 = null;
                btnFind = null;
                repositoryItemSpinEditStentOrder_Disabled = null;
                repositoryItemSpinEditStentOrder = null;
                gridColumnStentOrder = null;
                barDockControlRight = null;
                barDockControlLeft = null;
                barDockControlBottom = null;
                barDockControlTop = null;
                barManager1 = null;
                layoutControlItem13 = null;
                layoutControlItem15 = null;
                layoutControlItem16 = null;
                layoutControlItem14 = null;
                Root = null;
                txtKeyword = null;
                dtFrom = null;
                dtTo = null;
                layoutControl2 = null;
                repositoryItemLookUpEditSereServPackage_Enabled = null;
                repositoryItemLookUpEditSereServPackage_Disabled = null;
                repositoryItemLookUpEdit_PatientType_Disable = null;
                repositoryItemChkIsExpend_Disable = null;
                gridColumn6 = null;
                lciReturnResult = null;
                layoutControlItem9 = null;
                gridLookUpEdit1View = null;
                cboLoginName = null;
                txtLoginName = null;
                layoutControlItem7 = null;
                pnlPrint = null;
                repositoryItemCheckEditIsNoExecute_Disable = null;
                repositoryItemCheckEditIsNoExecute = null;
                gridColumnIsNoExecute = null;
                layoutControlItem6 = null;
                layoutControlItem5 = null;
                layoutControlItem4 = null;
                layoutControlItem3 = null;
                layoutControlItem2 = null;
                layoutControlItem1 = null;
                layoutControlGroup1 = null;
                repositoryItemChkOutKtcFee_Disable = null;
                repositoryItemChkOutKtcFee_Enable = null;
                repositoryItemGridLookUpEdit1View = null;
                repositoryItemGridLookUpEdit_PatientType = null;
                repositoryItemLookUpEdit_DiKemPTTT = null;
                repositoryItemPictureEdit1 = null;
                repositoryItemChkIsKH_Disable = null;
                repositoryItemChkIsKH = null;
                repositoryItemLookUpEdit_PatientType = null;
                gridColumnDescription = null;
                gridColumn9 = null;
                gridColumn8 = null;
                gridColumn7 = null;
                gridColumnDvDinhKem = null;
                repositoryItemChkIsExpend = null;
                gridColumn11 = null;
                gridColumnPatientTypeName = null;
                gridColumn5 = null;
                gridColumn4 = null;
                gridColumn3 = null;
                gridColumn10 = null;
                gridColumn2 = null;
                gridColumn1 = null;
                gridColumnIsOutKtcFee = null;
                gridViewBordereau = null;
                gridControlBordereau = null;
                btnPrint = null;
                lblTotalPrice = null;
                lblTotalPatientPrice = null;
                lblTotalObtainedPrice = null;
                lblTotalDepositPrice = null;
                layoutControl1 = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
