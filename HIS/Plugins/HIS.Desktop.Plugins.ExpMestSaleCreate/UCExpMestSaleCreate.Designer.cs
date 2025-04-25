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
namespace HIS.Desktop.Plugins.ExpMestSaleCreate
{
    partial class UCExpMestSaleCreate
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            ProcessReleaseAllMedi();
            ProcessReleaseAllMaty();
            Inventec.Common.Logging.LogSystem.Debug("Dispose HIS.Desktop.Plugins.ExpMestSaleCreate");

            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UCExpMestSaleCreate));
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject1 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject2 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject3 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject4 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject5 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject6 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject7 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject8 = new DevExpress.Utils.SerializableAppearanceObject();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.txtEmail = new DevExpress.XtraEditors.TextEdit();
            this.barManager1 = new DevExpress.XtraBars.BarManager();
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.btnCtrlA = new DevExpress.XtraBars.BarButtonItem();
            this.btnCtrlI = new DevExpress.XtraBars.BarButtonItem();
            this.btnCtrlS = new DevExpress.XtraBars.BarButtonItem();
            this.btnCtrlN = new DevExpress.XtraBars.BarButtonItem();
            this.barButtonItemCtrlF = new DevExpress.XtraBars.BarButtonItem();
            this.bbtnSallBill_Manager = new DevExpress.XtraBars.BarButtonItem();
            this.barButtonCancelExport = new DevExpress.XtraBars.BarButtonItem();
            this.barButtonF2 = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.btnQr = new DevExpress.XtraEditors.DropDownButton();
            this.chkEditUser = new DevExpress.XtraEditors.CheckEdit();
            this.btnDonCu = new DevExpress.XtraEditors.SimpleButton();
            this.layoutControlIcd = new DevExpress.XtraLayout.LayoutControl();
            this.panelIcd = new System.Windows.Forms.Panel();
            this.layoutControlGroup2 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem39 = new DevExpress.XtraLayout.LayoutControlItem();
            this.btnSubIcd = new DevExpress.XtraEditors.SimpleButton();
            this.txtIcd = new DevExpress.XtraEditors.TextEdit();
            this.txtSubIcdCode = new DevExpress.XtraEditors.TextEdit();
            this.btnSearchPres = new DevExpress.XtraEditors.SimpleButton();
            this.txtPatientPhone = new DevExpress.XtraEditors.TextEdit();
            this.txtPatientCode = new DevExpress.XtraEditors.TextEdit();
            this.chkAutoShow = new DevExpress.XtraEditors.CheckEdit();
            this.btnDebt = new DevExpress.XtraEditors.SimpleButton();
            this.layoutControl2 = new DevExpress.XtraLayout.LayoutControl();
            this.labelControl5 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
            this.spinQuetThe = new DevExpress.XtraEditors.SpinEdit();
            this.btnCauHinh = new DevExpress.XtraEditors.SimpleButton();
            this.ChkKetNoiPOS = new DevExpress.XtraEditors.CheckEdit();
            this.lblTransactionCode = new DevExpress.XtraEditors.LabelControl();
            this.lblTotalReceivable = new DevExpress.XtraEditors.LabelControl();
            this.spinBaseValue = new DevExpress.XtraEditors.SpinEdit();
            this.chkRoundPrice = new DevExpress.XtraEditors.CheckEdit();
            this.spinBillNumOrder = new DevExpress.XtraEditors.SpinEdit();
            this.cboBillAccountBook = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridLookUpEdit2View = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.cboBillCashierRoom = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.chkCreateBill = new DevExpress.XtraEditors.CheckEdit();
            this.lblPresNumber = new DevExpress.XtraEditors.LabelControl();
            this.cboPayForm = new DevExpress.XtraEditors.LookUpEdit();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.lblTotalPrice = new DevExpress.XtraEditors.LabelControl();
            this.spinDiscount = new DevExpress.XtraEditors.SpinEdit();
            this.spinTransferAmount = new DevExpress.XtraEditors.SpinEdit();
            this.treeListResult = new DevExpress.XtraTreeList.TreeList();
            this.treeListColumn_Result_NAME = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListColumn_Result_ExpAmount = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListColumn_Result_ExpPrice = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListColumn_Result_TotalPrice = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListColumn_Result_ActiveIngrName = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListColumn_Result_ServiceUnitName = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListColumn_Result_Concentra = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListColumn_Result_NationalName = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.spinDiscountRatio = new DevExpress.XtraEditors.SpinEdit();
            this.lblPayPrice = new DevExpress.XtraEditors.LabelControl();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem13 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem16 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciTranferAmount = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem8 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem14 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem9 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem11 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem12 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem32 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciCreateBill = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciBillCashierRoom = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciBillAccountBook = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciBillNumOrder = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciRoundPrice = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciBaseValue = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciTotalReceivable = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciTransactionCode = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem41 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem46 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem3 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.lcQuetthe = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem47 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem48 = new DevExpress.XtraLayout.LayoutControlItem();
            this.chkPrintNow = new DevExpress.XtraEditors.CheckEdit();
            this.btnNewExpMest = new DevExpress.XtraEditors.SimpleButton();
            this.btnCancelExport = new DevExpress.XtraEditors.SimpleButton();
            this.popupControlContainerMediMaty = new DevExpress.XtraBars.PopupControlContainer();
            this.gridControlMediMaty = new Inventec.Desktop.CustomControl.CustomGridControlWithFilterMultiColumn();
            this.gridViewMediMaty = new Inventec.Desktop.CustomControl.CustomGridViewWithFilterMultiColumn();
            this.txtMediMatyForPrescription = new DevExpress.XtraEditors.ButtonEdit();
            this.popupControlContainer1 = new DevExpress.XtraBars.PopupControlContainer();
            this.gridControlPopupUser = new DevExpress.XtraGrid.GridControl();
            this.gridViewPopupUser = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.cboAge = new DevExpress.XtraEditors.LookUpEdit();
            this.txtAge = new DevExpress.XtraEditors.TextEdit();
            this.cboTHX = new DevExpress.XtraEditors.LookUpEdit();
            this.txtMaTHX = new DevExpress.XtraEditors.TextEdit();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.spinProfit = new DevExpress.XtraEditors.SpinEdit();
            this.spinDayNum = new DevExpress.XtraEditors.SpinEdit();
            this.dtIntructionTime = new DevExpress.XtraEditors.DateEdit();
            this.btnSaleBill = new DevExpress.XtraEditors.SimpleButton();
            this.txtLoginName = new DevExpress.XtraEditors.TextEdit();
            this.spinDiscountDetailRatio = new DevExpress.XtraEditors.SpinEdit();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.spinDiscountDetail = new DevExpress.XtraEditors.SpinEdit();
            this.lblExpMestCode = new DevExpress.XtraEditors.LabelControl();
            this.panel1 = new System.Windows.Forms.Panel();
            this.txtPatientDob = new DevExpress.XtraEditors.ButtonEdit();
            this.dtPatientDob = new DevExpress.XtraEditors.DateEdit();
            this.txtAddress = new DevExpress.XtraEditors.TextEdit();
            this.btnSavePrint = new DevExpress.XtraEditors.SimpleButton();
            this.ddBtnPrint = new DevExpress.XtraEditors.DropDownButton();
            this.btnNew = new DevExpress.XtraEditors.SimpleButton();
            this.btnSave = new DevExpress.XtraEditors.SimpleButton();
            this.btnAdd = new DevExpress.XtraEditors.SimpleButton();
            this.txtNote = new DevExpress.XtraEditors.TextEdit();
            this.txtTutorial = new DevExpress.XtraEditors.TextEdit();
            this.spinExpVatRatio = new DevExpress.XtraEditors.SpinEdit();
            this.spinExpPrice = new DevExpress.XtraEditors.SpinEdit();
            this.checkImpExpPrice = new DevExpress.XtraEditors.CheckEdit();
            this.spinAmount = new DevExpress.XtraEditors.SpinEdit();
            this.txtDescription = new DevExpress.XtraEditors.TextEdit();
            this.txtVirPatientName = new DevExpress.XtraEditors.TextEdit();
            this.txtPrescriptionCode = new DevExpress.XtraEditors.TextEdit();
            this.checkIsVisitor = new DevExpress.XtraEditors.CheckEdit();
            this.cboPatientType = new DevExpress.XtraEditors.LookUpEdit();
            this.txtExpMediStock = new DevExpress.XtraEditors.TextEdit();
            this.txtPresUser = new DevExpress.XtraEditors.ButtonEdit();
            this.treeListMediMate = new DevExpress.XtraTreeList.TreeList();
            this.treeListColumn1 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListColumn6 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListColumn10 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListColumn2 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListColumn3 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListColumn4 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListColumn5 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListColumn9 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListColumn7 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListColumn8 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.repositoryItemSpinEdit__Amount = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
            this.repositoryItemBtnView = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.imageCollection1 = new DevExpress.Utils.ImageCollection();
            this.cboGender = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridLookUpEdit1View = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.txtTreatmentCode = new DevExpress.XtraEditors.TextEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutExpMediStock = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutPatientType = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutVisitor = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutExpPrice = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem23 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem24 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem25 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem7 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciExpMestCode = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem20 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem26 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.layoutControlItem29 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem30 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutPatient = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciTHX = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem35 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem34 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem36 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem22 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciMediMatyForPrescription = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutTutorial = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem38 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem27 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutAmount = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem28 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutExpVatRatio = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem15 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem19 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem17 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutNote = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem21 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem10 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem18 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem31 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutPrescriptionCode = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem37 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciPatientCode = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciPhone = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutDescription = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutImportExpPrice = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem4 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.layoutControlItem33 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem42 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem43 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem44 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem45 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.layoutControlItem40 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem49 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem50 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem51 = new DevExpress.XtraLayout.LayoutControlItem();
            this.dxValidationProvider_Save = new DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider();
            this.dxValidationProvider_Add = new DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtEmail.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkEditUser.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlIcd)).BeginInit();
            this.layoutControlIcd.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem39)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtIcd.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSubIcdCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPatientPhone.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPatientCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkAutoShow.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl2)).BeginInit();
            this.layoutControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spinQuetThe.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ChkKetNoiPOS.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinBaseValue.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkRoundPrice.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinBillNumOrder.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboBillAccountBook.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit2View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboBillCashierRoom.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkCreateBill.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboPayForm.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinDiscount.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinTransferAmount.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.treeListResult)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinDiscountRatio.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem13)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem16)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTranferAmount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem14)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem11)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem12)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem32)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciCreateBill)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBillCashierRoom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBillAccountBook)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBillNumOrder)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciRoundPrice)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBaseValue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTotalReceivable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTransactionCode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem41)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem46)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcQuetthe)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem47)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem48)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkPrintNow.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.popupControlContainerMediMaty)).BeginInit();
            this.popupControlContainerMediMaty.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlMediMaty)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewMediMaty)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMediMatyForPrescription.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.popupControlContainer1)).BeginInit();
            this.popupControlContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlPopupUser)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewPopupUser)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboAge.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAge.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboTHX.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMaTHX.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinProfit.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinDayNum.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtIntructionTime.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtIntructionTime.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtLoginName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinDiscountDetailRatio.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinDiscountDetail.Properties)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtPatientDob.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtPatientDob.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtPatientDob.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAddress.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtNote.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTutorial.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinExpVatRatio.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinExpPrice.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkImpExpPrice.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinAmount.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDescription.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtVirPatientName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPrescriptionCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkIsVisitor.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboPatientType.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtExpMediStock.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPresUser.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.treeListMediMate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSpinEdit__Amount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemBtnView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageCollection1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboGender.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTreatmentCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutExpMediStock)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutPatientType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutVisitor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutExpPrice)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem23)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem24)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem25)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciExpMestCode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem20)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem26)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem29)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem30)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutPatient)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTHX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem35)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem34)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem36)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem22)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciMediMatyForPrescription)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutTutorial)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem38)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem27)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutAmount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem28)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutExpVatRatio)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem15)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem19)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem17)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutNote)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem21)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem10)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem18)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem31)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutPrescriptionCode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem37)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPatientCode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPhone)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutDescription)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutImportExpPrice)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem33)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem42)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem43)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem44)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem45)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem40)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem49)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem50)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem51)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider_Save)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider_Add)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.txtEmail);
            this.layoutControl1.Controls.Add(this.btnQr);
            this.layoutControl1.Controls.Add(this.chkEditUser);
            this.layoutControl1.Controls.Add(this.btnDonCu);
            this.layoutControl1.Controls.Add(this.layoutControlIcd);
            this.layoutControl1.Controls.Add(this.btnSubIcd);
            this.layoutControl1.Controls.Add(this.txtIcd);
            this.layoutControl1.Controls.Add(this.txtSubIcdCode);
            this.layoutControl1.Controls.Add(this.btnSearchPres);
            this.layoutControl1.Controls.Add(this.txtPatientPhone);
            this.layoutControl1.Controls.Add(this.txtPatientCode);
            this.layoutControl1.Controls.Add(this.chkAutoShow);
            this.layoutControl1.Controls.Add(this.btnDebt);
            this.layoutControl1.Controls.Add(this.layoutControl2);
            this.layoutControl1.Controls.Add(this.chkPrintNow);
            this.layoutControl1.Controls.Add(this.btnNewExpMest);
            this.layoutControl1.Controls.Add(this.btnCancelExport);
            this.layoutControl1.Controls.Add(this.popupControlContainerMediMaty);
            this.layoutControl1.Controls.Add(this.txtMediMatyForPrescription);
            this.layoutControl1.Controls.Add(this.popupControlContainer1);
            this.layoutControl1.Controls.Add(this.cboAge);
            this.layoutControl1.Controls.Add(this.txtAge);
            this.layoutControl1.Controls.Add(this.cboTHX);
            this.layoutControl1.Controls.Add(this.txtMaTHX);
            this.layoutControl1.Controls.Add(this.labelControl2);
            this.layoutControl1.Controls.Add(this.spinProfit);
            this.layoutControl1.Controls.Add(this.spinDayNum);
            this.layoutControl1.Controls.Add(this.dtIntructionTime);
            this.layoutControl1.Controls.Add(this.btnSaleBill);
            this.layoutControl1.Controls.Add(this.txtLoginName);
            this.layoutControl1.Controls.Add(this.spinDiscountDetailRatio);
            this.layoutControl1.Controls.Add(this.labelControl1);
            this.layoutControl1.Controls.Add(this.spinDiscountDetail);
            this.layoutControl1.Controls.Add(this.lblExpMestCode);
            this.layoutControl1.Controls.Add(this.panel1);
            this.layoutControl1.Controls.Add(this.txtAddress);
            this.layoutControl1.Controls.Add(this.btnSavePrint);
            this.layoutControl1.Controls.Add(this.ddBtnPrint);
            this.layoutControl1.Controls.Add(this.btnNew);
            this.layoutControl1.Controls.Add(this.btnSave);
            this.layoutControl1.Controls.Add(this.btnAdd);
            this.layoutControl1.Controls.Add(this.txtNote);
            this.layoutControl1.Controls.Add(this.txtTutorial);
            this.layoutControl1.Controls.Add(this.spinExpVatRatio);
            this.layoutControl1.Controls.Add(this.spinExpPrice);
            this.layoutControl1.Controls.Add(this.checkImpExpPrice);
            this.layoutControl1.Controls.Add(this.spinAmount);
            this.layoutControl1.Controls.Add(this.txtDescription);
            this.layoutControl1.Controls.Add(this.txtVirPatientName);
            this.layoutControl1.Controls.Add(this.txtPrescriptionCode);
            this.layoutControl1.Controls.Add(this.checkIsVisitor);
            this.layoutControl1.Controls.Add(this.cboPatientType);
            this.layoutControl1.Controls.Add(this.txtExpMediStock);
            this.layoutControl1.Controls.Add(this.txtPresUser);
            this.layoutControl1.Controls.Add(this.treeListMediMate);
            this.layoutControl1.Controls.Add(this.cboGender);
            this.layoutControl1.Controls.Add(this.txtTreatmentCode);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 29);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(46, 204, 250, 350);
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(1420, 641);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // txtEmail
            // 
            this.txtEmail.Enabled = false;
            this.txtEmail.Location = new System.Drawing.Point(750, 54);
            this.txtEmail.MenuManager = this.barManager1;
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(124, 20);
            this.txtEmail.StyleController = this.layoutControl1;
            this.txtEmail.TabIndex = 89;
            // 
            // barManager1
            // 
            this.barManager1.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
            this.bar1});
            this.barManager1.DockControls.Add(this.barDockControlTop);
            this.barManager1.DockControls.Add(this.barDockControlBottom);
            this.barManager1.DockControls.Add(this.barDockControlLeft);
            this.barManager1.DockControls.Add(this.barDockControlRight);
            this.barManager1.Form = this;
            this.barManager1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.btnCtrlA,
            this.btnCtrlI,
            this.btnCtrlS,
            this.btnCtrlN,
            this.barButtonItemCtrlF,
            this.bbtnSallBill_Manager,
            this.barButtonCancelExport,
            this.barButtonF2});
            this.barManager1.MaxItemId = 8;
            // 
            // bar1
            // 
            this.bar1.BarName = "Tools";
            this.bar1.DockCol = 0;
            this.bar1.DockRow = 0;
            this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.btnCtrlA),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnCtrlI),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnCtrlS),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnCtrlN),
            new DevExpress.XtraBars.LinkPersistInfo(this.barButtonItemCtrlF),
            new DevExpress.XtraBars.LinkPersistInfo(this.bbtnSallBill_Manager),
            new DevExpress.XtraBars.LinkPersistInfo(this.barButtonCancelExport),
            new DevExpress.XtraBars.LinkPersistInfo(this.barButtonF2)});
            this.bar1.Text = "Tools";
            this.bar1.Visible = false;
            // 
            // btnCtrlA
            // 
            this.btnCtrlA.Caption = "Ctrl A";
            this.btnCtrlA.Id = 0;
            this.btnCtrlA.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A));
            this.btnCtrlA.Name = "btnCtrlA";
            this.btnCtrlA.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnCtrlA_ItemClick);
            // 
            // btnCtrlI
            // 
            this.btnCtrlI.Caption = "Ctrl I";
            this.btnCtrlI.Id = 1;
            this.btnCtrlI.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.I));
            this.btnCtrlI.Name = "btnCtrlI";
            this.btnCtrlI.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnCtrlI_ItemClick);
            // 
            // btnCtrlS
            // 
            this.btnCtrlS.Caption = "Ctrl S";
            this.btnCtrlS.Id = 2;
            this.btnCtrlS.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S));
            this.btnCtrlS.Name = "btnCtrlS";
            this.btnCtrlS.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnCtrlS_ItemClick);
            // 
            // btnCtrlN
            // 
            this.btnCtrlN.Caption = "Ctrl N";
            this.btnCtrlN.Id = 3;
            this.btnCtrlN.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N));
            this.btnCtrlN.Name = "btnCtrlN";
            this.btnCtrlN.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnCtrlN_ItemClick);
            // 
            // barButtonItemCtrlF
            // 
            this.barButtonItemCtrlF.Caption = "Ctrl F";
            this.barButtonItemCtrlF.Id = 4;
            this.barButtonItemCtrlF.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F));
            this.barButtonItemCtrlF.Name = "barButtonItemCtrlF";
            this.barButtonItemCtrlF.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItemCtrlF_ItemClick);
            // 
            // bbtnSallBill_Manager
            // 
            this.bbtnSallBill_Manager.Caption = "Ctrl T";
            this.bbtnSallBill_Manager.Id = 5;
            this.bbtnSallBill_Manager.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.T));
            this.bbtnSallBill_Manager.Name = "bbtnSallBill_Manager";
            this.bbtnSallBill_Manager.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bbtnSallBill_Manager_ItemClick);
            // 
            // barButtonCancelExport
            // 
            this.barButtonCancelExport.Caption = "Hủy xuất (Ctrl H)";
            this.barButtonCancelExport.Id = 6;
            this.barButtonCancelExport.Name = "barButtonCancelExport";
            this.barButtonCancelExport.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonCancelExport_ItemClick);
            // 
            // barButtonF2
            // 
            this.barButtonF2.Caption = "F3";
            this.barButtonF2.Id = 7;
            this.barButtonF2.ItemShortcut = new DevExpress.XtraBars.BarShortcut(System.Windows.Forms.Keys.F3);
            this.barButtonF2.Name = "barButtonF2";
            this.barButtonF2.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem1_ItemClick);
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Size = new System.Drawing.Size(1420, 29);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 670);
            this.barDockControlBottom.Size = new System.Drawing.Size(1420, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 29);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 641);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(1420, 29);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 641);
            // 
            // btnQr
            // 
            this.btnQr.Enabled = false;
            this.btnQr.Location = new System.Drawing.Point(1158, 617);
            this.btnQr.MenuManager = this.barManager1;
            this.btnQr.Name = "btnQr";
            this.btnQr.Size = new System.Drawing.Size(44, 22);
            this.btnQr.StyleController = this.layoutControl1;
            this.btnQr.TabIndex = 88;
            this.btnQr.Text = "QR";
            this.btnQr.ToolTip = "Thanh toán bằng QR";
            this.btnQr.Click += new System.EventHandler(this.btnQr_Click);
            // 
            // chkEditUser
            // 
            this.chkEditUser.Location = new System.Drawing.Point(647, 78);
            this.chkEditUser.MenuManager = this.barManager1;
            this.chkEditUser.Name = "chkEditUser";
            this.chkEditUser.Properties.Caption = "Sửa";
            this.chkEditUser.Properties.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
            this.chkEditUser.Size = new System.Drawing.Size(58, 19);
            this.chkEditUser.StyleController = this.layoutControl1;
            this.chkEditUser.TabIndex = 87;
            this.chkEditUser.CheckedChanged += new System.EventHandler(this.chkEditUser_CheckedChanged);
            // 
            // btnDonCu
            // 
            this.btnDonCu.Location = new System.Drawing.Point(586, 2);
            this.btnDonCu.Name = "btnDonCu";
            this.btnDonCu.Size = new System.Drawing.Size(77, 22);
            this.btnDonCu.StyleController = this.layoutControl1;
            this.btnDonCu.TabIndex = 86;
            this.btnDonCu.Text = "Đơn cũ";
            this.btnDonCu.Click += new System.EventHandler(this.btnDonCu_Click);
            // 
            // layoutControlIcd
            // 
            this.layoutControlIcd.Controls.Add(this.panelIcd);
            this.layoutControlIcd.Location = new System.Drawing.Point(0, 100);
            this.layoutControlIcd.Margin = new System.Windows.Forms.Padding(0);
            this.layoutControlIcd.Name = "layoutControlIcd";
            this.layoutControlIcd.Root = this.layoutControlGroup2;
            this.layoutControlIcd.Size = new System.Drawing.Size(498, 24);
            this.layoutControlIcd.TabIndex = 85;
            this.layoutControlIcd.Text = "layoutControl3";
            // 
            // panelIcd
            // 
            this.panelIcd.Location = new System.Drawing.Point(0, 0);
            this.panelIcd.Name = "panelIcd";
            this.panelIcd.Size = new System.Drawing.Size(498, 24);
            this.panelIcd.TabIndex = 4;
            // 
            // layoutControlGroup2
            // 
            this.layoutControlGroup2.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup2.GroupBordersVisible = false;
            this.layoutControlGroup2.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem39});
            this.layoutControlGroup2.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup2.Name = "layoutControlGroup2";
            this.layoutControlGroup2.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup2.Size = new System.Drawing.Size(498, 24);
            this.layoutControlGroup2.TextVisible = false;
            // 
            // layoutControlItem39
            // 
            this.layoutControlItem39.Control = this.panelIcd;
            this.layoutControlItem39.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem39.Name = "layoutControlItem39";
            this.layoutControlItem39.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlItem39.Size = new System.Drawing.Size(498, 24);
            this.layoutControlItem39.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem39.TextVisible = false;
            // 
            // btnSubIcd
            // 
            this.btnSubIcd.Location = new System.Drawing.Point(1028, 102);
            this.btnSubIcd.Name = "btnSubIcd";
            this.btnSubIcd.Size = new System.Drawing.Size(33, 20);
            this.btnSubIcd.StyleController = this.layoutControl1;
            this.btnSubIcd.TabIndex = 84;
            this.btnSubIcd.Text = "...";
            this.btnSubIcd.Click += new System.EventHandler(this.btnSubIcd_Click);
            // 
            // txtIcd
            // 
            this.txtIcd.Location = new System.Drawing.Point(688, 102);
            this.txtIcd.MenuManager = this.barManager1;
            this.txtIcd.Name = "txtIcd";
            this.txtIcd.Properties.MaxLength = 4000;
            this.txtIcd.Properties.NullValuePrompt = "Nhấn F1 để chọn bệnh";
            this.txtIcd.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtIcd.Size = new System.Drawing.Size(336, 20);
            this.txtIcd.StyleController = this.layoutControl1;
            this.txtIcd.TabIndex = 83;
            this.txtIcd.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtIcd_KeyDown);
            this.txtIcd.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtIcd_KeyUp);
            this.txtIcd.Leave += new System.EventHandler(this.txtIcd_Leave);
            // 
            // txtSubIcdCode
            // 
            this.txtSubIcdCode.Location = new System.Drawing.Point(575, 102);
            this.txtSubIcdCode.MenuManager = this.barManager1;
            this.txtSubIcdCode.Name = "txtSubIcdCode";
            this.txtSubIcdCode.Size = new System.Drawing.Size(113, 20);
            this.txtSubIcdCode.StyleController = this.layoutControl1;
            this.txtSubIcdCode.TabIndex = 82;
            this.txtSubIcdCode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSubIcdCode_KeyDown);
            this.txtSubIcdCode.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtSubIcdCode_KeyUp);
            // 
            // btnSearchPres
            // 
            this.btnSearchPres.Location = new System.Drawing.Point(298, 28);
            this.btnSearchPres.Name = "btnSearchPres";
            this.btnSearchPres.Size = new System.Drawing.Size(62, 22);
            this.btnSearchPres.StyleController = this.layoutControl1;
            this.btnSearchPres.TabIndex = 78;
            this.btnSearchPres.Text = "DS (F3)";
            this.btnSearchPres.ToolTip = "Danh sách đơn";
            this.btnSearchPres.Click += new System.EventHandler(this.btnSearchPres_Click);
            // 
            // txtPatientPhone
            // 
            this.txtPatientPhone.Location = new System.Drawing.Point(953, 54);
            this.txtPatientPhone.MenuManager = this.barManager1;
            this.txtPatientPhone.Name = "txtPatientPhone";
            this.txtPatientPhone.Size = new System.Drawing.Size(108, 20);
            this.txtPatientPhone.StyleController = this.layoutControl1;
            this.txtPatientPhone.TabIndex = 14;
            this.txtPatientPhone.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtPatientPhone_KeyDown);
            // 
            // txtPatientCode
            // 
            this.txtPatientCode.Location = new System.Drawing.Point(439, 28);
            this.txtPatientCode.MenuManager = this.barManager1;
            this.txtPatientCode.Name = "txtPatientCode";
            this.txtPatientCode.Properties.NullValuePrompt = "Mã bệnh nhân";
            this.txtPatientCode.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtPatientCode.Properties.ShowNullValuePromptWhenFocused = true;
            this.txtPatientCode.Size = new System.Drawing.Size(59, 20);
            this.txtPatientCode.StyleController = this.layoutControl1;
            this.txtPatientCode.TabIndex = 6;
            this.txtPatientCode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtPatientCode_KeyDown);
            // 
            // chkAutoShow
            // 
            this.chkAutoShow.Location = new System.Drawing.Point(324, 617);
            this.chkAutoShow.MenuManager = this.barManager1;
            this.chkAutoShow.Name = "chkAutoShow";
            this.chkAutoShow.Properties.Caption = "";
            this.chkAutoShow.Size = new System.Drawing.Size(86, 19);
            this.chkAutoShow.StyleController = this.layoutControl1;
            this.chkAutoShow.TabIndex = 77;
            this.chkAutoShow.CheckedChanged += new System.EventHandler(this.chkAutoShow_CheckedChanged);
            // 
            // btnDebt
            // 
            this.btnDebt.Location = new System.Drawing.Point(1336, 617);
            this.btnDebt.Name = "btnDebt";
            this.btnDebt.Size = new System.Drawing.Size(82, 22);
            this.btnDebt.StyleController = this.layoutControl1;
            this.btnDebt.TabIndex = 76;
            this.btnDebt.Text = "Xác nhận nợ";
            this.btnDebt.Click += new System.EventHandler(this.btnDebt_Click);
            // 
            // layoutControl2
            // 
            this.layoutControl2.Controls.Add(this.labelControl5);
            this.layoutControl2.Controls.Add(this.labelControl4);
            this.layoutControl2.Controls.Add(this.spinQuetThe);
            this.layoutControl2.Controls.Add(this.btnCauHinh);
            this.layoutControl2.Controls.Add(this.ChkKetNoiPOS);
            this.layoutControl2.Controls.Add(this.lblTransactionCode);
            this.layoutControl2.Controls.Add(this.lblTotalReceivable);
            this.layoutControl2.Controls.Add(this.spinBaseValue);
            this.layoutControl2.Controls.Add(this.chkRoundPrice);
            this.layoutControl2.Controls.Add(this.spinBillNumOrder);
            this.layoutControl2.Controls.Add(this.cboBillAccountBook);
            this.layoutControl2.Controls.Add(this.cboBillCashierRoom);
            this.layoutControl2.Controls.Add(this.chkCreateBill);
            this.layoutControl2.Controls.Add(this.lblPresNumber);
            this.layoutControl2.Controls.Add(this.cboPayForm);
            this.layoutControl2.Controls.Add(this.labelControl3);
            this.layoutControl2.Controls.Add(this.lblTotalPrice);
            this.layoutControl2.Controls.Add(this.spinDiscount);
            this.layoutControl2.Controls.Add(this.spinTransferAmount);
            this.layoutControl2.Controls.Add(this.treeListResult);
            this.layoutControl2.Controls.Add(this.spinDiscountRatio);
            this.layoutControl2.Controls.Add(this.lblPayPrice);
            this.layoutControl2.Location = new System.Drawing.Point(1063, 0);
            this.layoutControl2.Name = "layoutControl2";
            this.layoutControl2.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(1366, 415, 250, 350);
            this.layoutControl2.Root = this.Root;
            this.layoutControl2.Size = new System.Drawing.Size(357, 615);
            this.layoutControl2.TabIndex = 75;
            this.layoutControl2.Text = "layoutControl2";
            // 
            // labelControl5
            // 
            this.labelControl5.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.labelControl5.Location = new System.Drawing.Point(339, 196);
            this.labelControl5.Name = "labelControl5";
            this.labelControl5.Size = new System.Drawing.Size(16, 20);
            this.labelControl5.StyleController = this.layoutControl2;
            this.labelControl5.TabIndex = 87;
            // 
            // labelControl4
            // 
            this.labelControl4.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.labelControl4.Location = new System.Drawing.Point(339, 248);
            this.labelControl4.Name = "labelControl4";
            this.labelControl4.Size = new System.Drawing.Size(16, 20);
            this.labelControl4.StyleController = this.layoutControl2;
            this.labelControl4.TabIndex = 86;
            // 
            // spinQuetThe
            // 
            this.spinQuetThe.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.spinQuetThe.Location = new System.Drawing.Point(147, 300);
            this.spinQuetThe.MenuManager = this.barManager1;
            this.spinQuetThe.Name = "spinQuetThe";
            this.spinQuetThe.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.spinQuetThe.Size = new System.Drawing.Size(208, 20);
            this.spinQuetThe.StyleController = this.layoutControl2;
            this.spinQuetThe.TabIndex = 85;
            // 
            // btnCauHinh
            // 
            this.btnCauHinh.Image = ((System.Drawing.Image)(resources.GetObject("btnCauHinh.Image")));
            this.btnCauHinh.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnCauHinh.Location = new System.Drawing.Point(317, 2);
            this.btnCauHinh.Name = "btnCauHinh";
            this.btnCauHinh.Size = new System.Drawing.Size(26, 22);
            this.btnCauHinh.StyleController = this.layoutControl2;
            this.btnCauHinh.TabIndex = 84;
            this.btnCauHinh.Click += new System.EventHandler(this.btnCauHinh_Click);
            // 
            // ChkKetNoiPOS
            // 
            this.ChkKetNoiPOS.Location = new System.Drawing.Point(282, 2);
            this.ChkKetNoiPOS.MenuManager = this.barManager1;
            this.ChkKetNoiPOS.Name = "ChkKetNoiPOS";
            this.ChkKetNoiPOS.Properties.Caption = "";
            this.ChkKetNoiPOS.Size = new System.Drawing.Size(31, 19);
            this.ChkKetNoiPOS.StyleController = this.layoutControl2;
            this.ChkKetNoiPOS.TabIndex = 83;
            this.ChkKetNoiPOS.ToolTip = "Kết nối đến POS thanh toán của ngân hàng khi chọn hình thức thanh toán là \"Quẹt t" +
    "hẻ ngân hàng\" hoặc \"Tiền mặt/Quẹt thẻ";
            this.ChkKetNoiPOS.CheckedChanged += new System.EventHandler(this.ChkKetNoiPOS_CheckedChanged);
            // 
            // lblTransactionCode
            // 
            this.lblTransactionCode.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblTransactionCode.Location = new System.Drawing.Point(147, 148);
            this.lblTransactionCode.Name = "lblTransactionCode";
            this.lblTransactionCode.Size = new System.Drawing.Size(208, 20);
            this.lblTransactionCode.StyleController = this.layoutControl2;
            this.lblTransactionCode.TabIndex = 82;
            // 
            // lblTotalReceivable
            // 
            this.lblTotalReceivable.Appearance.Font = new System.Drawing.Font("Tahoma", 15F, System.Drawing.FontStyle.Bold);
            this.lblTotalReceivable.Appearance.ForeColor = System.Drawing.Color.Red;
            this.lblTotalReceivable.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblTotalReceivable.Location = new System.Drawing.Point(147, 248);
            this.lblTotalReceivable.Name = "lblTotalReceivable";
            this.lblTotalReceivable.Size = new System.Drawing.Size(133, 24);
            this.lblTotalReceivable.StyleController = this.layoutControl2;
            this.lblTotalReceivable.TabIndex = 81;
            // 
            // spinBaseValue
            // 
            this.spinBaseValue.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.spinBaseValue.Location = new System.Drawing.Point(295, 224);
            this.spinBaseValue.MenuManager = this.barManager1;
            this.spinBaseValue.Name = "spinBaseValue";
            this.spinBaseValue.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.spinBaseValue.Size = new System.Drawing.Size(60, 20);
            this.spinBaseValue.StyleController = this.layoutControl2;
            this.spinBaseValue.TabIndex = 80;
            this.spinBaseValue.EditValueChanged += new System.EventHandler(this.spinBaseValue_EditValueChanged);
            // 
            // chkRoundPrice
            // 
            this.chkRoundPrice.Location = new System.Drawing.Point(147, 224);
            this.chkRoundPrice.MenuManager = this.barManager1;
            this.chkRoundPrice.Name = "chkRoundPrice";
            this.chkRoundPrice.Properties.Caption = "";
            this.chkRoundPrice.Size = new System.Drawing.Size(99, 19);
            this.chkRoundPrice.StyleController = this.layoutControl2;
            this.chkRoundPrice.TabIndex = 79;
            this.chkRoundPrice.CheckedChanged += new System.EventHandler(this.chkRoundPrice_CheckedChanged);
            // 
            // spinBillNumOrder
            // 
            this.spinBillNumOrder.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.spinBillNumOrder.Location = new System.Drawing.Point(147, 76);
            this.spinBillNumOrder.MenuManager = this.barManager1;
            this.spinBillNumOrder.Name = "spinBillNumOrder";
            this.spinBillNumOrder.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.spinBillNumOrder.Properties.MaxValue = new decimal(new int[] {
            -1530494977,
            232830,
            0,
            0});
            this.spinBillNumOrder.Size = new System.Drawing.Size(208, 20);
            this.spinBillNumOrder.StyleController = this.layoutControl2;
            this.spinBillNumOrder.TabIndex = 78;
            this.spinBillNumOrder.KeyDown += new System.Windows.Forms.KeyEventHandler(this.spinBillNumOrder_KeyDown);
            // 
            // cboBillAccountBook
            // 
            this.cboBillAccountBook.Location = new System.Drawing.Point(147, 52);
            this.cboBillAccountBook.MenuManager = this.barManager1;
            this.cboBillAccountBook.Name = "cboBillAccountBook";
            this.cboBillAccountBook.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboBillAccountBook.Properties.NullText = "";
            this.cboBillAccountBook.Properties.View = this.gridLookUpEdit2View;
            this.cboBillAccountBook.Size = new System.Drawing.Size(208, 20);
            this.cboBillAccountBook.StyleController = this.layoutControl2;
            this.cboBillAccountBook.TabIndex = 77;
            this.cboBillAccountBook.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboBillAccountBook_Closed);
            this.cboBillAccountBook.EditValueChanged += new System.EventHandler(this.cboBillAccountBook_EditValueChanged);
            this.cboBillAccountBook.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cboBillAccountBook_KeyDown);
            // 
            // gridLookUpEdit2View
            // 
            this.gridLookUpEdit2View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridLookUpEdit2View.Name = "gridLookUpEdit2View";
            this.gridLookUpEdit2View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridLookUpEdit2View.OptionsView.ShowGroupPanel = false;
            // 
            // cboBillCashierRoom
            // 
            this.cboBillCashierRoom.Location = new System.Drawing.Point(147, 28);
            this.cboBillCashierRoom.MenuManager = this.barManager1;
            this.cboBillCashierRoom.Name = "cboBillCashierRoom";
            this.cboBillCashierRoom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboBillCashierRoom.Properties.NullText = "";
            this.cboBillCashierRoom.Properties.View = this.gridView1;
            this.cboBillCashierRoom.Size = new System.Drawing.Size(208, 20);
            this.cboBillCashierRoom.StyleController = this.layoutControl2;
            this.cboBillCashierRoom.TabIndex = 76;
            this.cboBillCashierRoom.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboBillCashierRoom_Closed);
            this.cboBillCashierRoom.EditValueChanged += new System.EventHandler(this.cboBillCashierRoom_EditValueChanged);
            this.cboBillCashierRoom.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cboBillCashierRoom_KeyDown);
            // 
            // gridView1
            // 
            this.gridView1.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridView1.Name = "gridView1";
            this.gridView1.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridView1.OptionsView.ShowGroupPanel = false;
            // 
            // chkCreateBill
            // 
            this.chkCreateBill.Location = new System.Drawing.Point(147, 2);
            this.chkCreateBill.MenuManager = this.barManager1;
            this.chkCreateBill.Name = "chkCreateBill";
            this.chkCreateBill.Properties.Caption = "";
            this.chkCreateBill.Size = new System.Drawing.Size(36, 19);
            this.chkCreateBill.StyleController = this.layoutControl2;
            this.chkCreateBill.TabIndex = 75;
            this.chkCreateBill.CheckedChanged += new System.EventHandler(this.chkCreateBill_CheckedChanged);
            this.chkCreateBill.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.chkCreateBill_PreviewKeyDown);
            // 
            // lblPresNumber
            // 
            this.lblPresNumber.Location = new System.Drawing.Point(147, 324);
            this.lblPresNumber.Name = "lblPresNumber";
            this.lblPresNumber.Size = new System.Drawing.Size(208, 20);
            this.lblPresNumber.StyleController = this.layoutControl2;
            this.lblPresNumber.TabIndex = 74;
            // 
            // cboPayForm
            // 
            this.cboPayForm.Location = new System.Drawing.Point(147, 100);
            this.cboPayForm.MenuManager = this.barManager1;
            this.cboPayForm.Name = "cboPayForm";
            this.cboPayForm.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboPayForm.Properties.NullText = "";
            this.cboPayForm.Size = new System.Drawing.Size(208, 20);
            this.cboPayForm.StyleController = this.layoutControl2;
            this.cboPayForm.TabIndex = 0;
            this.cboPayForm.ProcessNewValue += new DevExpress.XtraEditors.Controls.ProcessNewValueEventHandler(this.cboPayForm_ProcessNewValue);
            this.cboPayForm.EditValueChanged += new System.EventHandler(this.cboPayForm_EditValueChanged);
            // 
            // labelControl3
            // 
            this.labelControl3.Location = new System.Drawing.Point(344, 172);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(11, 13);
            this.labelControl3.StyleController = this.layoutControl2;
            this.labelControl3.TabIndex = 73;
            this.labelControl3.Text = "%";
            // 
            // lblTotalPrice
            // 
            this.lblTotalPrice.Appearance.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalPrice.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblTotalPrice.Location = new System.Drawing.Point(147, 124);
            this.lblTotalPrice.Name = "lblTotalPrice";
            this.lblTotalPrice.Size = new System.Drawing.Size(208, 20);
            this.lblTotalPrice.StyleController = this.layoutControl2;
            this.lblTotalPrice.TabIndex = 51;
            // 
            // spinDiscount
            // 
            this.spinDiscount.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.spinDiscount.Location = new System.Drawing.Point(147, 172);
            this.spinDiscount.Name = "spinDiscount";
            this.spinDiscount.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.spinDiscount.Properties.DisplayFormat.FormatString = "#,##0.0000";
            this.spinDiscount.Properties.EditFormat.FormatString = "#,##0.0000";
            this.spinDiscount.Properties.Mask.EditMask = "n";
            this.spinDiscount.Properties.Mask.UseMaskAsDisplayFormat = true;
            this.spinDiscount.Size = new System.Drawing.Size(133, 20);
            this.spinDiscount.StyleController = this.layoutControl2;
            this.spinDiscount.TabIndex = 1;
            this.spinDiscount.EditValueChanged += new System.EventHandler(this.spinDiscount_EditValueChanged);
            this.spinDiscount.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.spinDiscount_PreviewKeyDown_1);
            // 
            // spinTransferAmount
            // 
            this.spinTransferAmount.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.spinTransferAmount.Location = new System.Drawing.Point(147, 276);
            this.spinTransferAmount.MenuManager = this.barManager1;
            this.spinTransferAmount.Name = "spinTransferAmount";
            this.spinTransferAmount.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.spinTransferAmount.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.spinTransferAmount.Properties.DisplayFormat.FormatString = "#,##0";
            this.spinTransferAmount.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.spinTransferAmount.Properties.EditFormat.FormatString = "#,##0";
            this.spinTransferAmount.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.spinTransferAmount.Size = new System.Drawing.Size(208, 20);
            this.spinTransferAmount.StyleController = this.layoutControl2;
            this.spinTransferAmount.TabIndex = 3;
            // 
            // treeListResult
            // 
            this.treeListResult.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] {
            this.treeListColumn_Result_NAME,
            this.treeListColumn_Result_ExpAmount,
            this.treeListColumn_Result_ExpPrice,
            this.treeListColumn_Result_TotalPrice,
            this.treeListColumn_Result_ActiveIngrName,
            this.treeListColumn_Result_ServiceUnitName,
            this.treeListColumn_Result_Concentra,
            this.treeListColumn_Result_NationalName});
            this.treeListResult.Cursor = System.Windows.Forms.Cursors.Default;
            this.treeListResult.Location = new System.Drawing.Point(2, 348);
            this.treeListResult.Name = "treeListResult";
            this.treeListResult.OptionsView.AutoWidth = false;
            this.treeListResult.OptionsView.ShowIndicator = false;
            this.treeListResult.Size = new System.Drawing.Size(353, 265);
            this.treeListResult.TabIndex = 67;
            this.treeListResult.NodeCellStyle += new DevExpress.XtraTreeList.GetCustomNodeCellStyleEventHandler(this.treeListResult_NodeCellStyle);
            this.treeListResult.CustomUnboundColumnData += new DevExpress.XtraTreeList.CustomColumnDataEventHandler(this.treeListResult_CustomUnboundColumnData);
            // 
            // treeListColumn_Result_NAME
            // 
            this.treeListColumn_Result_NAME.Caption = "Tên";
            this.treeListColumn_Result_NAME.FieldName = "MEDI_MATE_TYPE_NAME";
            this.treeListColumn_Result_NAME.Name = "treeListColumn_Result_NAME";
            this.treeListColumn_Result_NAME.OptionsColumn.AllowEdit = false;
            this.treeListColumn_Result_NAME.Visible = true;
            this.treeListColumn_Result_NAME.VisibleIndex = 0;
            this.treeListColumn_Result_NAME.Width = 250;
            // 
            // treeListColumn_Result_ExpAmount
            // 
            this.treeListColumn_Result_ExpAmount.AppearanceCell.Options.UseTextOptions = true;
            this.treeListColumn_Result_ExpAmount.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.treeListColumn_Result_ExpAmount.AppearanceHeader.Options.UseTextOptions = true;
            this.treeListColumn_Result_ExpAmount.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.treeListColumn_Result_ExpAmount.Caption = "Số lượng";
            this.treeListColumn_Result_ExpAmount.FieldName = "EXP_AMOUNT";
            this.treeListColumn_Result_ExpAmount.Name = "treeListColumn_Result_ExpAmount";
            this.treeListColumn_Result_ExpAmount.OptionsColumn.AllowEdit = false;
            this.treeListColumn_Result_ExpAmount.Visible = true;
            this.treeListColumn_Result_ExpAmount.VisibleIndex = 1;
            this.treeListColumn_Result_ExpAmount.Width = 70;
            // 
            // treeListColumn_Result_ExpPrice
            // 
            this.treeListColumn_Result_ExpPrice.AppearanceCell.Options.UseTextOptions = true;
            this.treeListColumn_Result_ExpPrice.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.treeListColumn_Result_ExpPrice.AppearanceHeader.Options.UseTextOptions = true;
            this.treeListColumn_Result_ExpPrice.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.treeListColumn_Result_ExpPrice.Caption = "Giá";
            this.treeListColumn_Result_ExpPrice.FieldName = "EXP_PRICE_DISPLAY";
            this.treeListColumn_Result_ExpPrice.Name = "treeListColumn_Result_ExpPrice";
            this.treeListColumn_Result_ExpPrice.OptionsColumn.AllowEdit = false;
            this.treeListColumn_Result_ExpPrice.UnboundType = DevExpress.XtraTreeList.Data.UnboundColumnType.Object;
            this.treeListColumn_Result_ExpPrice.Visible = true;
            this.treeListColumn_Result_ExpPrice.VisibleIndex = 2;
            this.treeListColumn_Result_ExpPrice.Width = 70;
            // 
            // treeListColumn_Result_TotalPrice
            // 
            this.treeListColumn_Result_TotalPrice.AppearanceCell.Options.UseTextOptions = true;
            this.treeListColumn_Result_TotalPrice.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.treeListColumn_Result_TotalPrice.AppearanceHeader.Options.UseTextOptions = true;
            this.treeListColumn_Result_TotalPrice.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.treeListColumn_Result_TotalPrice.Caption = "Thành tiền";
            this.treeListColumn_Result_TotalPrice.FieldName = "TOTAL_PRICE_DISPLAY";
            this.treeListColumn_Result_TotalPrice.Name = "treeListColumn_Result_TotalPrice";
            this.treeListColumn_Result_TotalPrice.OptionsColumn.AllowEdit = false;
            this.treeListColumn_Result_TotalPrice.UnboundType = DevExpress.XtraTreeList.Data.UnboundColumnType.Object;
            this.treeListColumn_Result_TotalPrice.Visible = true;
            this.treeListColumn_Result_TotalPrice.VisibleIndex = 3;
            this.treeListColumn_Result_TotalPrice.Width = 120;
            // 
            // treeListColumn_Result_ActiveIngrName
            // 
            this.treeListColumn_Result_ActiveIngrName.Caption = "Hoạt chất";
            this.treeListColumn_Result_ActiveIngrName.FieldName = "ACTIVE_INGR_BHYT_NAME";
            this.treeListColumn_Result_ActiveIngrName.Name = "treeListColumn_Result_ActiveIngrName";
            this.treeListColumn_Result_ActiveIngrName.OptionsColumn.AllowEdit = false;
            this.treeListColumn_Result_ActiveIngrName.Visible = true;
            this.treeListColumn_Result_ActiveIngrName.VisibleIndex = 4;
            // 
            // treeListColumn_Result_ServiceUnitName
            // 
            this.treeListColumn_Result_ServiceUnitName.Caption = "ĐVT";
            this.treeListColumn_Result_ServiceUnitName.FieldName = "SERVICE_UNIT_NAME";
            this.treeListColumn_Result_ServiceUnitName.Name = "treeListColumn_Result_ServiceUnitName";
            this.treeListColumn_Result_ServiceUnitName.OptionsColumn.AllowEdit = false;
            this.treeListColumn_Result_ServiceUnitName.Visible = true;
            this.treeListColumn_Result_ServiceUnitName.VisibleIndex = 5;
            this.treeListColumn_Result_ServiceUnitName.Width = 60;
            // 
            // treeListColumn_Result_Concentra
            // 
            this.treeListColumn_Result_Concentra.Caption = "Hàm lượng";
            this.treeListColumn_Result_Concentra.FieldName = "CONCENTRA";
            this.treeListColumn_Result_Concentra.Name = "treeListColumn_Result_Concentra";
            this.treeListColumn_Result_Concentra.OptionsColumn.AllowEdit = false;
            this.treeListColumn_Result_Concentra.Visible = true;
            this.treeListColumn_Result_Concentra.VisibleIndex = 6;
            // 
            // treeListColumn_Result_NationalName
            // 
            this.treeListColumn_Result_NationalName.Caption = "Nước sản xuất";
            this.treeListColumn_Result_NationalName.FieldName = "NATIONAL_NAME";
            this.treeListColumn_Result_NationalName.Name = "treeListColumn_Result_NationalName";
            this.treeListColumn_Result_NationalName.OptionsColumn.AllowEdit = false;
            this.treeListColumn_Result_NationalName.Visible = true;
            this.treeListColumn_Result_NationalName.VisibleIndex = 7;
            this.treeListColumn_Result_NationalName.Width = 120;
            // 
            // spinDiscountRatio
            // 
            this.spinDiscountRatio.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.spinDiscountRatio.Location = new System.Drawing.Point(284, 172);
            this.spinDiscountRatio.Name = "spinDiscountRatio";
            this.spinDiscountRatio.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.spinDiscountRatio.Size = new System.Drawing.Size(56, 20);
            this.spinDiscountRatio.StyleController = this.layoutControl2;
            this.spinDiscountRatio.TabIndex = 2;
            this.spinDiscountRatio.EditValueChanged += new System.EventHandler(this.spinDiscountRatio_EditValueChanged);
            this.spinDiscountRatio.KeyDown += new System.Windows.Forms.KeyEventHandler(this.spinDiscountRatio_KeyDown);
            // 
            // lblPayPrice
            // 
            this.lblPayPrice.Appearance.Font = new System.Drawing.Font("Tahoma", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPayPrice.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.lblPayPrice.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblPayPrice.Location = new System.Drawing.Point(147, 196);
            this.lblPayPrice.Name = "lblPayPrice";
            this.lblPayPrice.Size = new System.Drawing.Size(133, 24);
            this.lblPayPrice.StyleController = this.layoutControl2;
            this.lblPayPrice.TabIndex = 53;
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem13,
            this.layoutControlItem16,
            this.lciTranferAmount,
            this.layoutControlItem8,
            this.layoutControlItem14,
            this.layoutControlItem9,
            this.layoutControlItem11,
            this.layoutControlItem12,
            this.layoutControlItem32,
            this.lciCreateBill,
            this.lciBillCashierRoom,
            this.lciBillAccountBook,
            this.lciBillNumOrder,
            this.lciRoundPrice,
            this.lciBaseValue,
            this.lciTotalReceivable,
            this.lciTransactionCode,
            this.layoutControlItem41,
            this.layoutControlItem46,
            this.emptySpaceItem3,
            this.lcQuetthe,
            this.layoutControlItem47,
            this.layoutControlItem48});
            this.Root.Location = new System.Drawing.Point(0, 0);
            this.Root.Name = "Root";
            this.Root.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.Root.Size = new System.Drawing.Size(357, 615);
            this.Root.TextVisible = false;
            // 
            // layoutControlItem13
            // 
            this.layoutControlItem13.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem13.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem13.Control = this.spinDiscount;
            this.layoutControlItem13.Location = new System.Drawing.Point(0, 170);
            this.layoutControlItem13.Name = "layoutControlItem13";
            this.layoutControlItem13.Size = new System.Drawing.Size(282, 24);
            this.layoutControlItem13.Text = "Chiết khấu theo đơn:";
            this.layoutControlItem13.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem13.TextSize = new System.Drawing.Size(140, 20);
            this.layoutControlItem13.TextToControlDistance = 5;
            // 
            // layoutControlItem16
            // 
            this.layoutControlItem16.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.layoutControlItem16.AppearanceItemCaption.Options.UseFont = true;
            this.layoutControlItem16.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem16.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem16.Control = this.lblPayPrice;
            this.layoutControlItem16.Location = new System.Drawing.Point(0, 194);
            this.layoutControlItem16.Name = "layoutControlItem16";
            this.layoutControlItem16.Size = new System.Drawing.Size(282, 28);
            this.layoutControlItem16.Text = "Phải thanh toán:";
            this.layoutControlItem16.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem16.TextSize = new System.Drawing.Size(140, 20);
            this.layoutControlItem16.TextToControlDistance = 5;
            // 
            // lciTranferAmount
            // 
            this.lciTranferAmount.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciTranferAmount.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciTranferAmount.Control = this.spinTransferAmount;
            this.lciTranferAmount.Location = new System.Drawing.Point(0, 274);
            this.lciTranferAmount.Name = "lciTranferAmount";
            this.lciTranferAmount.Size = new System.Drawing.Size(357, 24);
            this.lciTranferAmount.Text = "Số tiền chuyển khoản:";
            this.lciTranferAmount.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciTranferAmount.TextSize = new System.Drawing.Size(140, 20);
            this.lciTranferAmount.TextToControlDistance = 5;
            // 
            // layoutControlItem8
            // 
            this.layoutControlItem8.Control = this.treeListResult;
            this.layoutControlItem8.Location = new System.Drawing.Point(0, 346);
            this.layoutControlItem8.Name = "layoutControlItem8";
            this.layoutControlItem8.Size = new System.Drawing.Size(357, 269);
            this.layoutControlItem8.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem8.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem8.TextToControlDistance = 0;
            this.layoutControlItem8.TextVisible = false;
            // 
            // layoutControlItem14
            // 
            this.layoutControlItem14.Control = this.spinDiscountRatio;
            this.layoutControlItem14.Location = new System.Drawing.Point(282, 170);
            this.layoutControlItem14.Name = "layoutControlItem14";
            this.layoutControlItem14.Size = new System.Drawing.Size(60, 24);
            this.layoutControlItem14.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem14.TextVisible = false;
            // 
            // layoutControlItem9
            // 
            this.layoutControlItem9.Control = this.labelControl3;
            this.layoutControlItem9.Location = new System.Drawing.Point(342, 170);
            this.layoutControlItem9.Name = "layoutControlItem9";
            this.layoutControlItem9.Size = new System.Drawing.Size(15, 24);
            this.layoutControlItem9.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem9.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem9.TextToControlDistance = 0;
            this.layoutControlItem9.TextVisible = false;
            // 
            // layoutControlItem11
            // 
            this.layoutControlItem11.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.layoutControlItem11.AppearanceItemCaption.Options.UseForeColor = true;
            this.layoutControlItem11.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem11.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem11.Control = this.cboPayForm;
            this.layoutControlItem11.Location = new System.Drawing.Point(0, 98);
            this.layoutControlItem11.Name = "layoutControlItem11";
            this.layoutControlItem11.Size = new System.Drawing.Size(357, 24);
            this.layoutControlItem11.Text = "Hình thức:";
            this.layoutControlItem11.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem11.TextSize = new System.Drawing.Size(140, 20);
            this.layoutControlItem11.TextToControlDistance = 5;
            // 
            // layoutControlItem12
            // 
            this.layoutControlItem12.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem12.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem12.Control = this.lblTotalPrice;
            this.layoutControlItem12.Location = new System.Drawing.Point(0, 122);
            this.layoutControlItem12.Name = "layoutControlItem12";
            this.layoutControlItem12.Size = new System.Drawing.Size(357, 24);
            this.layoutControlItem12.Text = "Tổng tiền:";
            this.layoutControlItem12.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem12.TextSize = new System.Drawing.Size(140, 20);
            this.layoutControlItem12.TextToControlDistance = 5;
            // 
            // layoutControlItem32
            // 
            this.layoutControlItem32.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem32.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem32.Control = this.lblPresNumber;
            this.layoutControlItem32.Location = new System.Drawing.Point(0, 322);
            this.layoutControlItem32.Name = "layoutControlItem32";
            this.layoutControlItem32.Size = new System.Drawing.Size(357, 24);
            this.layoutControlItem32.Text = "Số toa:";
            this.layoutControlItem32.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem32.TextSize = new System.Drawing.Size(140, 20);
            this.layoutControlItem32.TextToControlDistance = 5;
            // 
            // lciCreateBill
            // 
            this.lciCreateBill.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciCreateBill.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciCreateBill.Control = this.chkCreateBill;
            this.lciCreateBill.Location = new System.Drawing.Point(0, 0);
            this.lciCreateBill.Name = "lciCreateBill";
            this.lciCreateBill.Size = new System.Drawing.Size(185, 26);
            this.lciCreateBill.Text = "Xuất biên lai/hóa đơn:";
            this.lciCreateBill.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciCreateBill.TextSize = new System.Drawing.Size(140, 20);
            this.lciCreateBill.TextToControlDistance = 5;
            // 
            // lciBillCashierRoom
            // 
            this.lciBillCashierRoom.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.lciBillCashierRoom.AppearanceItemCaption.Options.UseForeColor = true;
            this.lciBillCashierRoom.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciBillCashierRoom.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciBillCashierRoom.Control = this.cboBillCashierRoom;
            this.lciBillCashierRoom.Location = new System.Drawing.Point(0, 26);
            this.lciBillCashierRoom.Name = "lciBillCashierRoom";
            this.lciBillCashierRoom.Size = new System.Drawing.Size(357, 24);
            this.lciBillCashierRoom.Text = "Điểm thu tiền:";
            this.lciBillCashierRoom.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciBillCashierRoom.TextSize = new System.Drawing.Size(140, 20);
            this.lciBillCashierRoom.TextToControlDistance = 5;
            this.lciBillCashierRoom.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            // 
            // lciBillAccountBook
            // 
            this.lciBillAccountBook.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.lciBillAccountBook.AppearanceItemCaption.Options.UseForeColor = true;
            this.lciBillAccountBook.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciBillAccountBook.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciBillAccountBook.Control = this.cboBillAccountBook;
            this.lciBillAccountBook.Location = new System.Drawing.Point(0, 50);
            this.lciBillAccountBook.Name = "lciBillAccountBook";
            this.lciBillAccountBook.Size = new System.Drawing.Size(357, 24);
            this.lciBillAccountBook.Text = "Sổ biên lai/hóa đơn:";
            this.lciBillAccountBook.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciBillAccountBook.TextSize = new System.Drawing.Size(140, 20);
            this.lciBillAccountBook.TextToControlDistance = 5;
            this.lciBillAccountBook.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            // 
            // lciBillNumOrder
            // 
            this.lciBillNumOrder.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciBillNumOrder.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciBillNumOrder.Control = this.spinBillNumOrder;
            this.lciBillNumOrder.Location = new System.Drawing.Point(0, 74);
            this.lciBillNumOrder.Name = "lciBillNumOrder";
            this.lciBillNumOrder.Size = new System.Drawing.Size(357, 24);
            this.lciBillNumOrder.Text = "Số biên lai/hóa đơn:";
            this.lciBillNumOrder.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciBillNumOrder.TextSize = new System.Drawing.Size(140, 20);
            this.lciBillNumOrder.TextToControlDistance = 5;
            // 
            // lciRoundPrice
            // 
            this.lciRoundPrice.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciRoundPrice.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciRoundPrice.Control = this.chkRoundPrice;
            this.lciRoundPrice.Location = new System.Drawing.Point(0, 222);
            this.lciRoundPrice.Name = "lciRoundPrice";
            this.lciRoundPrice.Size = new System.Drawing.Size(248, 24);
            this.lciRoundPrice.Text = "Làm tròn:";
            this.lciRoundPrice.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciRoundPrice.TextSize = new System.Drawing.Size(140, 20);
            this.lciRoundPrice.TextToControlDistance = 5;
            this.lciRoundPrice.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            // 
            // lciBaseValue
            // 
            this.lciBaseValue.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciBaseValue.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciBaseValue.Control = this.spinBaseValue;
            this.lciBaseValue.Location = new System.Drawing.Point(248, 222);
            this.lciBaseValue.Name = "lciBaseValue";
            this.lciBaseValue.OptionsToolTip.ToolTip = "Giá trị cơ sở phục vụ làm tròn";
            this.lciBaseValue.Size = new System.Drawing.Size(109, 24);
            this.lciBaseValue.Text = "GTCS:";
            this.lciBaseValue.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciBaseValue.TextSize = new System.Drawing.Size(40, 20);
            this.lciBaseValue.TextToControlDistance = 5;
            this.lciBaseValue.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            // 
            // lciTotalReceivable
            // 
            this.lciTotalReceivable.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.lciTotalReceivable.AppearanceItemCaption.Options.UseFont = true;
            this.lciTotalReceivable.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciTotalReceivable.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciTotalReceivable.Control = this.lblTotalReceivable;
            this.lciTotalReceivable.Location = new System.Drawing.Point(0, 246);
            this.lciTotalReceivable.Name = "lciTotalReceivable";
            this.lciTotalReceivable.Size = new System.Drawing.Size(282, 28);
            this.lciTotalReceivable.Text = "Thực thu:";
            this.lciTotalReceivable.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciTotalReceivable.TextSize = new System.Drawing.Size(140, 20);
            this.lciTotalReceivable.TextToControlDistance = 5;
            this.lciTotalReceivable.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            // 
            // lciTransactionCode
            // 
            this.lciTransactionCode.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciTransactionCode.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciTransactionCode.Control = this.lblTransactionCode;
            this.lciTransactionCode.Location = new System.Drawing.Point(0, 146);
            this.lciTransactionCode.Name = "lciTransactionCode";
            this.lciTransactionCode.Size = new System.Drawing.Size(357, 24);
            this.lciTransactionCode.Text = "Mã giao dịch:";
            this.lciTransactionCode.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciTransactionCode.TextSize = new System.Drawing.Size(140, 20);
            this.lciTransactionCode.TextToControlDistance = 5;
            this.lciTransactionCode.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            // 
            // layoutControlItem41
            // 
            this.layoutControlItem41.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem41.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem41.Control = this.ChkKetNoiPOS;
            this.layoutControlItem41.Location = new System.Drawing.Point(185, 0);
            this.layoutControlItem41.Name = "layoutControlItem41";
            this.layoutControlItem41.OptionsToolTip.ToolTip = "Kết nối đến POS thanh toán của ngân hàng khi chọn hình thức thanh toán là \"Quẹt t" +
    "hẻ ngân hàng\" hoặc \"Tiền mặt/Quẹt thẻ";
            this.layoutControlItem41.Size = new System.Drawing.Size(130, 26);
            this.layoutControlItem41.Text = "Kết nối POS:";
            this.layoutControlItem41.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem41.TextSize = new System.Drawing.Size(90, 0);
            this.layoutControlItem41.TextToControlDistance = 5;
            // 
            // layoutControlItem46
            // 
            this.layoutControlItem46.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem46.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.layoutControlItem46.Control = this.btnCauHinh;
            this.layoutControlItem46.Location = new System.Drawing.Point(315, 0);
            this.layoutControlItem46.Name = "layoutControlItem46";
            this.layoutControlItem46.Size = new System.Drawing.Size(30, 26);
            this.layoutControlItem46.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem46.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem46.TextToControlDistance = 0;
            this.layoutControlItem46.TextVisible = false;
            // 
            // emptySpaceItem3
            // 
            this.emptySpaceItem3.AllowHotTrack = false;
            this.emptySpaceItem3.Location = new System.Drawing.Point(345, 0);
            this.emptySpaceItem3.Name = "emptySpaceItem3";
            this.emptySpaceItem3.Size = new System.Drawing.Size(12, 26);
            this.emptySpaceItem3.TextSize = new System.Drawing.Size(0, 0);
            // 
            // lcQuetthe
            // 
            this.lcQuetthe.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lcQuetthe.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lcQuetthe.Control = this.spinQuetThe;
            this.lcQuetthe.Location = new System.Drawing.Point(0, 298);
            this.lcQuetthe.Name = "lcQuetthe";
            this.lcQuetthe.Size = new System.Drawing.Size(357, 24);
            this.lcQuetthe.Text = "Số tiền quẹt thẻ";
            this.lcQuetthe.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lcQuetthe.TextSize = new System.Drawing.Size(140, 20);
            this.lcQuetthe.TextToControlDistance = 5;
            this.lcQuetthe.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            // 
            // layoutControlItem47
            // 
            this.layoutControlItem47.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Underline);
            this.layoutControlItem47.AppearanceItemCaption.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.layoutControlItem47.AppearanceItemCaption.Options.UseFont = true;
            this.layoutControlItem47.AppearanceItemCaption.Options.UseForeColor = true;
            this.layoutControlItem47.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem47.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem47.Control = this.labelControl4;
            this.layoutControlItem47.Location = new System.Drawing.Point(282, 246);
            this.layoutControlItem47.Name = "layoutControlItem47";
            this.layoutControlItem47.Size = new System.Drawing.Size(75, 28);
            this.layoutControlItem47.Text = "Chi tiết";
            this.layoutControlItem47.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem47.TextSize = new System.Drawing.Size(50, 20);
            this.layoutControlItem47.TextToControlDistance = 5;
            this.layoutControlItem47.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            this.layoutControlItem47.Click += new System.EventHandler(this.layoutControlItem47_Click);
            // 
            // layoutControlItem48
            // 
            this.layoutControlItem48.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Underline);
            this.layoutControlItem48.AppearanceItemCaption.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.layoutControlItem48.AppearanceItemCaption.Options.UseFont = true;
            this.layoutControlItem48.AppearanceItemCaption.Options.UseForeColor = true;
            this.layoutControlItem48.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem48.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem48.Control = this.labelControl5;
            this.layoutControlItem48.Location = new System.Drawing.Point(282, 194);
            this.layoutControlItem48.Name = "layoutControlItem48";
            this.layoutControlItem48.Size = new System.Drawing.Size(75, 28);
            this.layoutControlItem48.Text = "Chi tiết";
            this.layoutControlItem48.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem48.TextSize = new System.Drawing.Size(50, 20);
            this.layoutControlItem48.TextToControlDistance = 5;
            this.layoutControlItem48.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            this.layoutControlItem48.Click += new System.EventHandler(this.layoutControlItem48_Click);
            // 
            // chkPrintNow
            // 
            this.chkPrintNow.Location = new System.Drawing.Point(509, 617);
            this.chkPrintNow.MenuManager = this.barManager1;
            this.chkPrintNow.Name = "chkPrintNow";
            this.chkPrintNow.Properties.Caption = "";
            this.chkPrintNow.Size = new System.Drawing.Size(44, 19);
            this.chkPrintNow.StyleController = this.layoutControl1;
            this.chkPrintNow.TabIndex = 73;
            this.chkPrintNow.CheckedChanged += new System.EventHandler(this.chkPrintNow_CheckedChanged);
            // 
            // btnNewExpMest
            // 
            this.btnNewExpMest.Location = new System.Drawing.Point(1003, 617);
            this.btnNewExpMest.Name = "btnNewExpMest";
            this.btnNewExpMest.Size = new System.Drawing.Size(93, 22);
            this.btnNewExpMest.StyleController = this.layoutControl1;
            this.btnNewExpMest.TabIndex = 68;
            this.btnNewExpMest.Text = "Đơn mới (Ctrl D)";
            this.btnNewExpMest.Click += new System.EventHandler(this.btnNewExpMest_Click);
            // 
            // btnCancelExport
            // 
            this.btnCancelExport.Location = new System.Drawing.Point(557, 617);
            this.btnCancelExport.Name = "btnCancelExport";
            this.btnCancelExport.Size = new System.Drawing.Size(127, 22);
            this.btnCancelExport.StyleController = this.layoutControl1;
            this.btnCancelExport.TabIndex = 64;
            this.btnCancelExport.Text = "Hủy xuất (Ctrl H)";
            this.btnCancelExport.Click += new System.EventHandler(this.btnCancelExport_Click);
            // 
            // popupControlContainerMediMaty
            // 
            this.popupControlContainerMediMaty.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.popupControlContainerMediMaty.Controls.Add(this.gridControlMediMaty);
            this.popupControlContainerMediMaty.Location = new System.Drawing.Point(20, 274);
            this.popupControlContainerMediMaty.Manager = this.barManager1;
            this.popupControlContainerMediMaty.Name = "popupControlContainerMediMaty";
            this.popupControlContainerMediMaty.Size = new System.Drawing.Size(583, 213);
            this.popupControlContainerMediMaty.TabIndex = 63;
            this.popupControlContainerMediMaty.Visible = false;
            this.popupControlContainerMediMaty.CloseUp += new System.EventHandler(this.popupControlContainerMediMaty_CloseUp);
            // 
            // gridControlMediMaty
            // 
            this.gridControlMediMaty.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControlMediMaty.Location = new System.Drawing.Point(0, 0);
            this.gridControlMediMaty.MainView = this.gridViewMediMaty;
            this.gridControlMediMaty.MenuManager = this.barManager1;
            this.gridControlMediMaty.Name = "gridControlMediMaty";
            this.gridControlMediMaty.Size = new System.Drawing.Size(583, 213);
            this.gridControlMediMaty.TabIndex = 0;
            this.gridControlMediMaty.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewMediMaty});
            // 
            // gridViewMediMaty
            // 
            this.gridViewMediMaty.GridControl = this.gridControlMediMaty;
            this.gridViewMediMaty.Name = "gridViewMediMaty";
            this.gridViewMediMaty.OptionsView.ShowGroupPanel = false;
            this.gridViewMediMaty.RowClick += new DevExpress.XtraGrid.Views.Grid.RowClickEventHandler(this.gridViewMediMaty_RowClick);
            this.gridViewMediMaty.CustomDrawCell += new DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventHandler(this.OnCustomDrawCell);
            this.gridViewMediMaty.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gridViewMediMaty_RowCellStyle);
            this.gridViewMediMaty.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewMediMaty_CustomUnboundColumnData);
            this.gridViewMediMaty.KeyDown += new System.Windows.Forms.KeyEventHandler(this.gridViewMediMaty_KeyDown);
            // 
            // txtMediMatyForPrescription
            // 
            this.txtMediMatyForPrescription.Location = new System.Drawing.Point(97, 126);
            this.txtMediMatyForPrescription.MenuManager = this.barManager1;
            this.txtMediMatyForPrescription.Name = "txtMediMatyForPrescription";
            this.txtMediMatyForPrescription.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Down)});
            this.txtMediMatyForPrescription.Properties.NullValuePrompt = "Chọn thuốc/ vật tư (Ctrl F)";
            this.txtMediMatyForPrescription.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtMediMatyForPrescription.Size = new System.Drawing.Size(399, 20);
            this.txtMediMatyForPrescription.StyleController = this.layoutControl1;
            this.txtMediMatyForPrescription.TabIndex = 19;
            this.txtMediMatyForPrescription.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txtMediMatyForPrescription_ButtonClick);
            this.txtMediMatyForPrescription.TextChanged += new System.EventHandler(this.txtMediMatyForPrescription_TextChanged);
            this.txtMediMatyForPrescription.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtMediMatyForPrescription_KeyDown);
            // 
            // popupControlContainer1
            // 
            this.popupControlContainer1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.popupControlContainer1.Controls.Add(this.gridControlPopupUser);
            this.popupControlContainer1.Location = new System.Drawing.Point(609, 392);
            this.popupControlContainer1.Manager = this.barManager1;
            this.popupControlContainer1.Name = "popupControlContainer1";
            this.popupControlContainer1.Size = new System.Drawing.Size(400, 191);
            this.popupControlContainer1.TabIndex = 6;
            this.popupControlContainer1.Visible = false;
            this.popupControlContainer1.CloseUp += new System.EventHandler(this.popupControlContainer1_CloseUp);
            // 
            // gridControlPopupUser
            // 
            this.gridControlPopupUser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControlPopupUser.Location = new System.Drawing.Point(0, 0);
            this.gridControlPopupUser.MainView = this.gridViewPopupUser;
            this.gridControlPopupUser.MenuManager = this.barManager1;
            this.gridControlPopupUser.Name = "gridControlPopupUser";
            this.gridControlPopupUser.Size = new System.Drawing.Size(400, 191);
            this.gridControlPopupUser.TabIndex = 0;
            this.gridControlPopupUser.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewPopupUser});
            // 
            // gridViewPopupUser
            // 
            this.gridViewPopupUser.GridControl = this.gridControlPopupUser;
            this.gridViewPopupUser.Name = "gridViewPopupUser";
            this.gridViewPopupUser.OptionsBehavior.Editable = false;
            this.gridViewPopupUser.OptionsView.ShowGroupPanel = false;
            this.gridViewPopupUser.OptionsView.ShowIndicator = false;
            this.gridViewPopupUser.RowClick += new DevExpress.XtraGrid.Views.Grid.RowClickEventHandler(this.gridViewPopupUser_RowClick);
            this.gridViewPopupUser.KeyDown += new System.Windows.Forms.KeyEventHandler(this.gridViewPopupUser_KeyDown);
            // 
            // cboAge
            // 
            this.cboAge.Enabled = false;
            this.cboAge.Location = new System.Drawing.Point(1007, 28);
            this.cboAge.MenuManager = this.barManager1;
            this.cboAge.Name = "cboAge";
            this.cboAge.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboAge.Properties.NullText = "";
            this.cboAge.Size = new System.Drawing.Size(54, 20);
            this.cboAge.StyleController = this.layoutControl1;
            this.cboAge.TabIndex = 10;
            // 
            // txtAge
            // 
            this.txtAge.Enabled = false;
            this.txtAge.Location = new System.Drawing.Point(972, 28);
            this.txtAge.MenuManager = this.barManager1;
            this.txtAge.Name = "txtAge";
            this.txtAge.Size = new System.Drawing.Size(35, 20);
            this.txtAge.StyleController = this.layoutControl1;
            this.txtAge.TabIndex = 9;
            // 
            // cboTHX
            // 
            this.cboTHX.Location = new System.Drawing.Point(201, 54);
            this.cboTHX.MenuManager = this.barManager1;
            this.cboTHX.Name = "cboTHX";
            this.cboTHX.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.cboTHX.Properties.AutoHeight = false;
            this.cboTHX.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete, "", -1, true, false, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, null, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject1, serializableAppearanceObject2, serializableAppearanceObject3, serializableAppearanceObject4, "", null, null, true)});
            this.cboTHX.Properties.NullText = "";
            this.cboTHX.Properties.PopupSizeable = false;
            this.cboTHX.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
            this.cboTHX.Properties.GetNotInListValue += new DevExpress.XtraEditors.Controls.GetNotInListValueEventHandler(this.cboTHX_Properties_GetNotInListValue);
            this.cboTHX.Size = new System.Drawing.Size(295, 20);
            this.cboTHX.StyleController = this.layoutControl1;
            this.cboTHX.TabIndex = 12;
            this.cboTHX.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboTHX_Closed);
            this.cboTHX.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboTHX_ButtonClick);
            this.cboTHX.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cboTHX_KeyDown);
            // 
            // txtMaTHX
            // 
            this.txtMaTHX.Location = new System.Drawing.Point(97, 54);
            this.txtMaTHX.MenuManager = this.barManager1;
            this.txtMaTHX.Name = "txtMaTHX";
            this.txtMaTHX.Size = new System.Drawing.Size(104, 20);
            this.txtMaTHX.StyleController = this.layoutControl1;
            this.txtMaTHX.TabIndex = 11;
            this.txtMaTHX.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtMaTHX_KeyDown);
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(164, 150);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(11, 13);
            this.labelControl2.StyleController = this.layoutControl1;
            this.labelControl2.TabIndex = 24;
            this.labelControl2.Text = "%";
            // 
            // spinProfit
            // 
            this.spinProfit.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.spinProfit.Location = new System.Drawing.Point(97, 150);
            this.spinProfit.MenuManager = this.barManager1;
            this.spinProfit.Name = "spinProfit";
            this.spinProfit.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.spinProfit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.spinProfit.Properties.MaxValue = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.spinProfit.Size = new System.Drawing.Size(63, 20);
            this.spinProfit.StyleController = this.layoutControl1;
            this.spinProfit.TabIndex = 23;
            this.spinProfit.EditValueChanged += new System.EventHandler(this.spinProfit_EditValueChanged);
            this.spinProfit.KeyDown += new System.Windows.Forms.KeyEventHandler(this.spinProfit_KeyDown);
            // 
            // spinDayNum
            // 
            this.spinDayNum.EditValue = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.spinDayNum.Location = new System.Drawing.Point(734, 126);
            this.spinDayNum.MenuManager = this.barManager1;
            this.spinDayNum.Name = "spinDayNum";
            this.spinDayNum.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.spinDayNum.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.spinDayNum.Properties.MaxValue = new decimal(new int[] {
            1215752191,
            23,
            0,
            0});
            this.spinDayNum.Properties.MinValue = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.spinDayNum.Size = new System.Drawing.Size(63, 20);
            this.spinDayNum.StyleController = this.layoutControl1;
            this.spinDayNum.TabIndex = 21;
            this.spinDayNum.Visible = false;
            this.spinDayNum.KeyDown += new System.Windows.Forms.KeyEventHandler(this.spinDayNum_KeyDown);
            this.spinDayNum.Leave += new System.EventHandler(this.spinDayNum_Leave);
            // 
            // dtIntructionTime
            // 
            this.dtIntructionTime.EditValue = null;
            this.dtIntructionTime.Location = new System.Drawing.Point(205, 78);
            this.dtIntructionTime.MenuManager = this.barManager1;
            this.dtIntructionTime.Name = "dtIntructionTime";
            this.dtIntructionTime.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtIntructionTime.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtIntructionTime.Properties.DisplayFormat.FormatString = "dd/MM/yyyy HH:mm";
            this.dtIntructionTime.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtIntructionTime.Properties.EditFormat.FormatString = "dd/MM/yyyy HH:mm";
            this.dtIntructionTime.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtIntructionTime.Properties.Mask.EditMask = "dd/MM/yyyy HH:mm";
            this.dtIntructionTime.Size = new System.Drawing.Size(122, 20);
            this.dtIntructionTime.StyleController = this.layoutControl1;
            this.dtIntructionTime.TabIndex = 15;
            this.dtIntructionTime.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.dtIntructionTime_Closed);
            this.dtIntructionTime.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dtIntructionTime_KeyDown);
            // 
            // btnSaleBill
            // 
            this.btnSaleBill.Location = new System.Drawing.Point(1206, 617);
            this.btnSaleBill.Name = "btnSaleBill";
            this.btnSaleBill.Size = new System.Drawing.Size(126, 22);
            this.btnSaleBill.StyleController = this.layoutControl1;
            this.btnSaleBill.TabIndex = 46;
            this.btnSaleBill.Text = "Xuất hóa đơn (Ctrl T)";
            this.btnSaleBill.Click += new System.EventHandler(this.btnSaleBill_Click);
            // 
            // txtLoginName
            // 
            this.txtLoginName.Enabled = false;
            this.txtLoginName.Location = new System.Drawing.Point(406, 78);
            this.txtLoginName.MenuManager = this.barManager1;
            this.txtLoginName.Name = "txtLoginName";
            this.txtLoginName.Size = new System.Drawing.Size(92, 20);
            this.txtLoginName.StyleController = this.layoutControl1;
            this.txtLoginName.TabIndex = 16;
            this.txtLoginName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtLoginName_KeyDown);
            // 
            // spinDiscountDetailRatio
            // 
            this.spinDiscountDetailRatio.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.spinDiscountDetailRatio.Location = new System.Drawing.Point(791, 150);
            this.spinDiscountDetailRatio.Name = "spinDiscountDetailRatio";
            this.spinDiscountDetailRatio.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.spinDiscountDetailRatio.Properties.MaxValue = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.spinDiscountDetailRatio.Size = new System.Drawing.Size(78, 20);
            this.spinDiscountDetailRatio.StyleController = this.layoutControl1;
            this.spinDiscountDetailRatio.TabIndex = 28;
            this.spinDiscountDetailRatio.EditValueChanged += new System.EventHandler(this.spinDiscountDetailRatio_EditValueChanged);
            this.spinDiscountDetailRatio.KeyDown += new System.Windows.Forms.KeyEventHandler(this.spinDiscountDetailRatio_KeyDown);
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(873, 150);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(11, 13);
            this.labelControl1.StyleController = this.layoutControl1;
            this.labelControl1.TabIndex = 41;
            this.labelControl1.Text = "%";
            // 
            // spinDiscountDetail
            // 
            this.spinDiscountDetail.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.spinDiscountDetail.Location = new System.Drawing.Point(575, 150);
            this.spinDiscountDetail.Name = "spinDiscountDetail";
            this.spinDiscountDetail.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.spinDiscountDetail.Properties.DisplayFormat.FormatString = "#,##0.00";
            this.spinDiscountDetail.Properties.EditFormat.FormatString = "#,##0.00";
            this.spinDiscountDetail.Properties.Mask.EditMask = "n";
            this.spinDiscountDetail.Properties.Mask.UseMaskAsDisplayFormat = true;
            this.spinDiscountDetail.Size = new System.Drawing.Size(212, 20);
            this.spinDiscountDetail.StyleController = this.layoutControl1;
            this.spinDiscountDetail.TabIndex = 27;
            this.spinDiscountDetail.EditValueChanged += new System.EventHandler(this.spinDiscountDetail_EditValueChanged);
            this.spinDiscountDetail.KeyDown += new System.Windows.Forms.KeyEventHandler(this.spinDiscountDetail_KeyDown);
            // 
            // lblExpMestCode
            // 
            this.lblExpMestCode.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblExpMestCode.Appearance.ForeColor = System.Drawing.Color.Blue;
            this.lblExpMestCode.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblExpMestCode.Location = new System.Drawing.Point(97, 78);
            this.lblExpMestCode.Name = "lblExpMestCode";
            this.lblExpMestCode.Size = new System.Drawing.Size(49, 20);
            this.lblExpMestCode.StyleController = this.layoutControl1;
            this.lblExpMestCode.TabIndex = 37;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.txtPatientDob);
            this.panel1.Controls.Add(this.dtPatientDob);
            this.panel1.Location = new System.Drawing.Point(852, 28);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(116, 22);
            this.panel1.TabIndex = 36;
            // 
            // txtPatientDob
            // 
            this.txtPatientDob.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtPatientDob.Location = new System.Drawing.Point(0, 0);
            this.txtPatientDob.Name = "txtPatientDob";
            this.txtPatientDob.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Down)});
            this.txtPatientDob.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.txtPatientDob.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.txtPatientDob.Size = new System.Drawing.Size(116, 20);
            this.txtPatientDob.TabIndex = 0;
            this.txtPatientDob.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txtPatientDob_ButtonClick);
            this.txtPatientDob.Click += new System.EventHandler(this.txtPatientDob_Click);
            this.txtPatientDob.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtPatientDob_KeyDown);
            this.txtPatientDob.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtPatientDob_KeyPress);
            this.txtPatientDob.Validating += new System.ComponentModel.CancelEventHandler(this.txtPatientDob_Validating);
            // 
            // dtPatientDob
            // 
            this.dtPatientDob.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dtPatientDob.EditValue = null;
            this.dtPatientDob.Location = new System.Drawing.Point(0, 0);
            this.dtPatientDob.Name = "dtPatientDob";
            this.dtPatientDob.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtPatientDob.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtPatientDob.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
            this.dtPatientDob.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.dtPatientDob.Properties.EditFormat.FormatString = "dd/MM/yyyy";
            this.dtPatientDob.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.dtPatientDob.Properties.Mask.EditMask = "dd/MM/yyyy";
            this.dtPatientDob.Properties.Mask.UseMaskAsDisplayFormat = true;
            this.dtPatientDob.Size = new System.Drawing.Size(116, 20);
            this.dtPatientDob.TabIndex = 0;
            this.dtPatientDob.Visible = false;
            this.dtPatientDob.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.dtPatientDob_Closed);
            this.dtPatientDob.EditValueChanged += new System.EventHandler(this.dtPatientDob_EditValueChanged);
            this.dtPatientDob.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dtPatientDob_KeyDown);
            // 
            // txtAddress
            // 
            this.txtAddress.Location = new System.Drawing.Point(575, 54);
            this.txtAddress.Name = "txtAddress";
            this.txtAddress.Properties.MaxLength = 600;
            this.txtAddress.Size = new System.Drawing.Size(116, 20);
            this.txtAddress.StyleController = this.layoutControl1;
            this.txtAddress.TabIndex = 13;
            this.txtAddress.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtAddress_KeyDown);
            // 
            // btnSavePrint
            // 
            this.btnSavePrint.Location = new System.Drawing.Point(688, 617);
            this.btnSavePrint.Name = "btnSavePrint";
            this.btnSavePrint.Size = new System.Drawing.Size(97, 22);
            this.btnSavePrint.StyleController = this.layoutControl1;
            this.btnSavePrint.TabIndex = 26;
            this.btnSavePrint.Text = "Lưu in (Ctrl I)";
            this.btnSavePrint.Click += new System.EventHandler(this.btnSavePrint_Click);
            // 
            // ddBtnPrint
            // 
            this.ddBtnPrint.Location = new System.Drawing.Point(1100, 617);
            this.ddBtnPrint.Name = "ddBtnPrint";
            this.ddBtnPrint.Size = new System.Drawing.Size(54, 22);
            this.ddBtnPrint.StyleController = this.layoutControl1;
            this.ddBtnPrint.TabIndex = 29;
            this.ddBtnPrint.Text = "In";
            this.ddBtnPrint.Click += new System.EventHandler(this.ddBtnPrint_Click);
            // 
            // btnNew
            // 
            this.btnNew.Location = new System.Drawing.Point(878, 617);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(121, 22);
            this.btnNew.StyleController = this.layoutControl1;
            this.btnNew.TabIndex = 28;
            this.btnNew.Text = "Mới (Ctrl N)";
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(789, 617);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(85, 22);
            this.btnSave.StyleController = this.layoutControl1;
            this.btnSave.TabIndex = 27;
            this.btnSave.Text = "Lưu (Ctrl S)";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(952, 174);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(109, 22);
            this.btnAdd.StyleController = this.layoutControl1;
            this.btnAdd.TabIndex = 31;
            this.btnAdd.Text = "Thêm (Ctrl A)";
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // txtNote
            // 
            this.txtNote.Location = new System.Drawing.Point(575, 174);
            this.txtNote.Name = "txtNote";
            this.txtNote.Properties.MaxLength = 200;
            this.txtNote.Size = new System.Drawing.Size(373, 20);
            this.txtNote.StyleController = this.layoutControl1;
            this.txtNote.TabIndex = 30;
            this.txtNote.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtNote_KeyDown);
            // 
            // txtTutorial
            // 
            this.txtTutorial.Location = new System.Drawing.Point(97, 174);
            this.txtTutorial.Name = "txtTutorial";
            this.txtTutorial.Properties.MaxLength = 1000;
            this.txtTutorial.Size = new System.Drawing.Size(399, 20);
            this.txtTutorial.StyleController = this.layoutControl1;
            this.txtTutorial.TabIndex = 29;
            this.txtTutorial.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtTutorial_KeyDown);
            // 
            // spinExpVatRatio
            // 
            this.spinExpVatRatio.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.spinExpVatRatio.Location = new System.Drawing.Point(418, 150);
            this.spinExpVatRatio.Name = "spinExpVatRatio";
            this.spinExpVatRatio.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.spinExpVatRatio.Properties.MaxValue = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.spinExpVatRatio.Size = new System.Drawing.Size(78, 20);
            this.spinExpVatRatio.StyleController = this.layoutControl1;
            this.spinExpVatRatio.TabIndex = 26;
            this.spinExpVatRatio.KeyDown += new System.Windows.Forms.KeyEventHandler(this.spinExpVatRatio_KeyDown);
            // 
            // spinExpPrice
            // 
            this.spinExpPrice.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.spinExpPrice.Location = new System.Drawing.Point(234, 150);
            this.spinExpPrice.Name = "spinExpPrice";
            this.spinExpPrice.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.spinExpPrice.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.spinExpPrice.Properties.DisplayFormat.FormatString = "#,##0.0000";
            this.spinExpPrice.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.spinExpPrice.Properties.MaxValue = new decimal(new int[] {
            -1530494977,
            232830,
            0,
            0});
            this.spinExpPrice.Size = new System.Drawing.Size(105, 20);
            this.spinExpPrice.StyleController = this.layoutControl1;
            this.spinExpPrice.TabIndex = 25;
            this.spinExpPrice.EditValueChanged += new System.EventHandler(this.spinExpPrice_EditValueChanged);
            this.spinExpPrice.KeyDown += new System.Windows.Forms.KeyEventHandler(this.spinExpPrice_KeyDown);
            // 
            // checkImpExpPrice
            // 
            this.checkImpExpPrice.Location = new System.Drawing.Point(876, 126);
            this.checkImpExpPrice.Name = "checkImpExpPrice";
            this.checkImpExpPrice.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Default;
            this.checkImpExpPrice.Properties.Caption = "";
            this.checkImpExpPrice.Properties.FullFocusRect = true;
            this.checkImpExpPrice.Size = new System.Drawing.Size(185, 19);
            this.checkImpExpPrice.StyleController = this.layoutControl1;
            this.checkImpExpPrice.TabIndex = 22;
            this.checkImpExpPrice.CheckedChanged += new System.EventHandler(this.checkImpExpPrice_CheckedChanged);
            this.checkImpExpPrice.KeyDown += new System.Windows.Forms.KeyEventHandler(this.checkImpExpPrice_KeyDown);
            // 
            // spinAmount
            // 
            this.spinAmount.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.spinAmount.Location = new System.Drawing.Point(575, 126);
            this.spinAmount.Name = "spinAmount";
            this.spinAmount.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.spinAmount.Properties.Mask.EditMask = "d";
            this.spinAmount.Properties.MaxValue = new decimal(new int[] {
            1569325055,
            23283064,
            0,
            0});
            this.spinAmount.Size = new System.Drawing.Size(70, 20);
            this.spinAmount.StyleController = this.layoutControl1;
            this.spinAmount.TabIndex = 20;
            this.spinAmount.EditValueChanged += new System.EventHandler(this.spinAmount_EditValueChanged);
            this.spinAmount.KeyDown += new System.Windows.Forms.KeyEventHandler(this.spinAmount_KeyDown);
            // 
            // txtDescription
            // 
            this.txtDescription.Location = new System.Drawing.Point(794, 78);
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Properties.MaxLength = 500;
            this.txtDescription.Size = new System.Drawing.Size(267, 20);
            this.txtDescription.StyleController = this.layoutControl1;
            this.txtDescription.TabIndex = 18;
            this.txtDescription.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtDescription_KeyDown);
            // 
            // txtVirPatientName
            // 
            this.txtVirPatientName.EditValue = "";
            this.txtVirPatientName.Location = new System.Drawing.Point(498, 28);
            this.txtVirPatientName.Name = "txtVirPatientName";
            this.txtVirPatientName.Properties.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtVirPatientName.Properties.MaxLength = 150;
            this.txtVirPatientName.Size = new System.Drawing.Size(123, 20);
            this.txtVirPatientName.StyleController = this.layoutControl1;
            this.txtVirPatientName.TabIndex = 7;
            this.txtVirPatientName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtVirPatientName_KeyDown);
            // 
            // txtPrescriptionCode
            // 
            this.txtPrescriptionCode.Location = new System.Drawing.Point(97, 28);
            this.txtPrescriptionCode.Name = "txtPrescriptionCode";
            this.txtPrescriptionCode.Properties.Appearance.Options.UseTextOptions = true;
            this.txtPrescriptionCode.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.txtPrescriptionCode.Properties.NullValuePrompt = "Mã y lệnh";
            this.txtPrescriptionCode.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtPrescriptionCode.Properties.ShowNullValuePromptWhenFocused = true;
            this.txtPrescriptionCode.Size = new System.Drawing.Size(102, 20);
            this.txtPrescriptionCode.StyleController = this.layoutControl1;
            this.txtPrescriptionCode.TabIndex = 4;
            this.txtPrescriptionCode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtPrescriptionCode_KeyDown);
            // 
            // checkIsVisitor
            // 
            this.checkIsVisitor.Location = new System.Drawing.Point(555, 2);
            this.checkIsVisitor.Name = "checkIsVisitor";
            this.checkIsVisitor.Properties.Caption = "";
            this.checkIsVisitor.Size = new System.Drawing.Size(27, 19);
            this.checkIsVisitor.StyleController = this.layoutControl1;
            this.checkIsVisitor.TabIndex = 2;
            this.checkIsVisitor.CheckedChanged += new System.EventHandler(this.checkIsVisitor_CheckedChanged);
            this.checkIsVisitor.KeyDown += new System.Windows.Forms.KeyEventHandler(this.checkIsVisitor_KeyDown);
            // 
            // cboPatientType
            // 
            this.cboPatientType.Location = new System.Drawing.Point(406, 2);
            this.cboPatientType.Name = "cboPatientType";
            this.cboPatientType.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.cboPatientType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboPatientType.Properties.NullText = "";
            this.cboPatientType.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
            this.cboPatientType.Size = new System.Drawing.Size(90, 20);
            this.cboPatientType.StyleController = this.layoutControl1;
            this.cboPatientType.TabIndex = 1;
            this.cboPatientType.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboPatientType_Closed);
            this.cboPatientType.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cboPatientType_KeyDown);
            // 
            // txtExpMediStock
            // 
            this.txtExpMediStock.EditValue = "";
            this.txtExpMediStock.Enabled = false;
            this.txtExpMediStock.Location = new System.Drawing.Point(97, 2);
            this.txtExpMediStock.Name = "txtExpMediStock";
            this.txtExpMediStock.Properties.NullText = "[EditValue is null]";
            this.txtExpMediStock.Size = new System.Drawing.Size(230, 20);
            this.txtExpMediStock.StyleController = this.layoutControl1;
            this.txtExpMediStock.TabIndex = 0;
            // 
            // txtPresUser
            // 
            this.txtPresUser.EditValue = "";
            this.txtPresUser.Location = new System.Drawing.Point(498, 78);
            this.txtPresUser.MenuManager = this.barManager1;
            this.txtPresUser.Name = "txtPresUser";
            this.txtPresUser.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.DropDown)});
            this.txtPresUser.Size = new System.Drawing.Size(145, 20);
            this.txtPresUser.StyleController = this.layoutControl1;
            this.txtPresUser.TabIndex = 17;
            this.txtPresUser.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txtPresUser_ButtonClick);
            this.txtPresUser.TextChanged += new System.EventHandler(this.txtPresUser_TextChanged);
            this.txtPresUser.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtPresUser_KeyDown);
            // 
            // treeListMediMate
            // 
            this.treeListMediMate.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] {
            this.treeListColumn1,
            this.treeListColumn6,
            this.treeListColumn10,
            this.treeListColumn2,
            this.treeListColumn3,
            this.treeListColumn4,
            this.treeListColumn5,
            this.treeListColumn9,
            this.treeListColumn7,
            this.treeListColumn8});
            this.treeListMediMate.Cursor = System.Windows.Forms.Cursors.Default;
            this.treeListMediMate.KeyFieldName = "CONCRETE_ID__IN_SETY";
            this.treeListMediMate.Location = new System.Drawing.Point(2, 200);
            this.treeListMediMate.Name = "treeListMediMate";
            this.treeListMediMate.OptionsBehavior.PopulateServiceColumns = true;
            this.treeListMediMate.OptionsView.AutoWidth = false;
            this.treeListMediMate.OptionsView.ShowCheckBoxes = true;
            this.treeListMediMate.ParentFieldName = "PARENT_ID__IN_SETY";
            this.treeListMediMate.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemSpinEdit__Amount,
            this.repositoryItemBtnView});
            this.treeListMediMate.SelectImageList = this.imageCollection1;
            this.treeListMediMate.Size = new System.Drawing.Size(1059, 413);
            this.treeListMediMate.TabIndex = 33;
            this.treeListMediMate.SelectImageClick += new DevExpress.XtraTreeList.NodeClickEventHandler(this.treeListMediMate_SelectImageClick);
            this.treeListMediMate.CustomNodeCellEdit += new DevExpress.XtraTreeList.GetCustomNodeCellEditEventHandler(this.treeListMediMate_CustomNodeCellEdit);
            this.treeListMediMate.NodeCellStyle += new DevExpress.XtraTreeList.GetCustomNodeCellStyleEventHandler(this.treeListMediMate_NodeCellStyle);
            this.treeListMediMate.CustomUnboundColumnData += new DevExpress.XtraTreeList.CustomColumnDataEventHandler(this.treeListMediMate_CustomUnboundColumnData);
            this.treeListMediMate.BeforeCheckNode += new DevExpress.XtraTreeList.CheckNodeEventHandler(this.treeListMediMate_BeforeCheckNode);
            this.treeListMediMate.PopupMenuShowing += new DevExpress.XtraTreeList.PopupMenuShowingEventHandler(this.treeListMediMate_PopupMenuShowing);
            this.treeListMediMate.CellValueChanged += new DevExpress.XtraTreeList.CellValueChangedEventHandler(this.treeListMediMate_CellValueChanged);
            this.treeListMediMate.Click += new System.EventHandler(this.treeListMediMate_Click);
            // 
            // treeListColumn1
            // 
            this.treeListColumn1.Caption = "Tên";
            this.treeListColumn1.FieldName = "MEDI_MATE_TYPE_NAME_STR";
            this.treeListColumn1.MinWidth = 52;
            this.treeListColumn1.Name = "treeListColumn1";
            this.treeListColumn1.OptionsColumn.AllowEdit = false;
            this.treeListColumn1.UnboundType = DevExpress.XtraTreeList.Data.UnboundColumnType.Object;
            this.treeListColumn1.Visible = true;
            this.treeListColumn1.VisibleIndex = 0;
            this.treeListColumn1.Width = 300;
            // 
            // treeListColumn6
            // 
            this.treeListColumn6.AppearanceHeader.Options.UseTextOptions = true;
            this.treeListColumn6.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.treeListColumn6.Caption = "Hàm lượng";
            this.treeListColumn6.FieldName = "CONCENTRA";
            this.treeListColumn6.Name = "treeListColumn6";
            this.treeListColumn6.OptionsColumn.AllowEdit = false;
            this.treeListColumn6.Visible = true;
            this.treeListColumn6.VisibleIndex = 1;
            // 
            // treeListColumn10
            // 
            this.treeListColumn10.Caption = "Hoạt chất";
            this.treeListColumn10.FieldName = "ACTIVE_INGR_BHYT_NAME";
            this.treeListColumn10.Name = "treeListColumn10";
            this.treeListColumn10.OptionsColumn.AllowEdit = false;
            this.treeListColumn10.Visible = true;
            this.treeListColumn10.VisibleIndex = 2;
            // 
            // treeListColumn2
            // 
            this.treeListColumn2.AppearanceCell.Options.UseTextOptions = true;
            this.treeListColumn2.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.treeListColumn2.Caption = "Số lượng";
            this.treeListColumn2.FieldName = "EXP_AMOUNT";
            this.treeListColumn2.Name = "treeListColumn2";
            this.treeListColumn2.UnboundType = DevExpress.XtraTreeList.Data.UnboundColumnType.Object;
            this.treeListColumn2.Visible = true;
            this.treeListColumn2.VisibleIndex = 3;
            this.treeListColumn2.Width = 80;
            // 
            // treeListColumn3
            // 
            this.treeListColumn3.Caption = "ĐVT";
            this.treeListColumn3.FieldName = "SERVICE_UNIT_NAME";
            this.treeListColumn3.Name = "treeListColumn3";
            this.treeListColumn3.OptionsColumn.AllowEdit = false;
            this.treeListColumn3.Visible = true;
            this.treeListColumn3.VisibleIndex = 4;
            this.treeListColumn3.Width = 71;
            // 
            // treeListColumn4
            // 
            this.treeListColumn4.AppearanceCell.Options.UseTextOptions = true;
            this.treeListColumn4.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.treeListColumn4.Caption = "Đơn giá";
            this.treeListColumn4.FieldName = "EXP_PRICE_STR";
            this.treeListColumn4.Name = "treeListColumn4";
            this.treeListColumn4.OptionsColumn.AllowEdit = false;
            this.treeListColumn4.UnboundType = DevExpress.XtraTreeList.Data.UnboundColumnType.Object;
            this.treeListColumn4.Visible = true;
            this.treeListColumn4.VisibleIndex = 5;
            this.treeListColumn4.Width = 73;
            // 
            // treeListColumn5
            // 
            this.treeListColumn5.AppearanceCell.Options.UseTextOptions = true;
            this.treeListColumn5.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.treeListColumn5.Caption = "VAT(%)";
            this.treeListColumn5.FieldName = "EXP_VAT_RATIO_STR";
            this.treeListColumn5.Name = "treeListColumn5";
            this.treeListColumn5.OptionsColumn.AllowEdit = false;
            this.treeListColumn5.UnboundType = DevExpress.XtraTreeList.Data.UnboundColumnType.Object;
            this.treeListColumn5.Visible = true;
            this.treeListColumn5.VisibleIndex = 6;
            this.treeListColumn5.Width = 73;
            // 
            // treeListColumn9
            // 
            this.treeListColumn9.AppearanceCell.Options.UseTextOptions = true;
            this.treeListColumn9.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.treeListColumn9.Caption = "Giá sau VAT";
            this.treeListColumn9.FieldName = "PRICE_WITH_VAT";
            this.treeListColumn9.Name = "treeListColumn9";
            this.treeListColumn9.OptionsColumn.AllowEdit = false;
            this.treeListColumn9.UnboundType = DevExpress.XtraTreeList.Data.UnboundColumnType.Object;
            this.treeListColumn9.Visible = true;
            this.treeListColumn9.VisibleIndex = 7;
            this.treeListColumn9.Width = 80;
            // 
            // treeListColumn7
            // 
            this.treeListColumn7.AppearanceCell.Options.UseTextOptions = true;
            this.treeListColumn7.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.treeListColumn7.Caption = "Thành tiền";
            this.treeListColumn7.FieldName = "TOTAL_PRICE_STR";
            this.treeListColumn7.Name = "treeListColumn7";
            this.treeListColumn7.OptionsColumn.AllowEdit = false;
            this.treeListColumn7.UnboundType = DevExpress.XtraTreeList.Data.UnboundColumnType.Object;
            this.treeListColumn7.Visible = true;
            this.treeListColumn7.VisibleIndex = 8;
            this.treeListColumn7.Width = 150;
            // 
            // treeListColumn8
            // 
            this.treeListColumn8.Caption = " ";
            this.treeListColumn8.FieldName = "ERROR_VIEW";
            this.treeListColumn8.Name = "treeListColumn8";
            this.treeListColumn8.UnboundType = DevExpress.XtraTreeList.Data.UnboundColumnType.Object;
            this.treeListColumn8.Visible = true;
            this.treeListColumn8.VisibleIndex = 9;
            this.treeListColumn8.Width = 30;
            // 
            // repositoryItemSpinEdit__Amount
            // 
            this.repositoryItemSpinEdit__Amount.AutoHeight = false;
            this.repositoryItemSpinEdit__Amount.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemSpinEdit__Amount.Mask.EditMask = "d";
            this.repositoryItemSpinEdit__Amount.Name = "repositoryItemSpinEdit__Amount";
            // 
            // repositoryItemBtnView
            // 
            this.repositoryItemBtnView.AutoHeight = false;
            this.repositoryItemBtnView.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, ((System.Drawing.Image)(resources.GetObject("repositoryItemBtnView.Buttons"))), new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject5, serializableAppearanceObject6, serializableAppearanceObject7, serializableAppearanceObject8, "", null, null, true)});
            this.repositoryItemBtnView.Name = "repositoryItemBtnView";
            this.repositoryItemBtnView.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            this.repositoryItemBtnView.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repositoryItemBtnView_ButtonClick);
            // 
            // imageCollection1
            // 
            this.imageCollection1.ImageStream = ((DevExpress.Utils.ImageCollectionStreamer)(resources.GetObject("imageCollection1.ImageStream")));
            this.imageCollection1.Images.SetKeyName(0, "clear.png");
            // 
            // cboGender
            // 
            this.cboGender.Location = new System.Drawing.Point(710, 28);
            this.cboGender.Name = "cboGender";
            this.cboGender.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.cboGender.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboGender.Properties.NullText = "";
            this.cboGender.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
            this.cboGender.Properties.View = this.gridLookUpEdit1View;
            this.cboGender.Size = new System.Drawing.Size(63, 20);
            this.cboGender.StyleController = this.layoutControl1;
            this.cboGender.TabIndex = 8;
            this.cboGender.KeyUp += new System.Windows.Forms.KeyEventHandler(this.cboGender_KeyUp);
            // 
            // gridLookUpEdit1View
            // 
            this.gridLookUpEdit1View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridLookUpEdit1View.Name = "gridLookUpEdit1View";
            this.gridLookUpEdit1View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridLookUpEdit1View.OptionsView.ShowGroupPanel = false;
            // 
            // txtTreatmentCode
            // 
            this.txtTreatmentCode.Location = new System.Drawing.Point(203, 28);
            this.txtTreatmentCode.MenuManager = this.barManager1;
            this.txtTreatmentCode.Name = "txtTreatmentCode";
            this.txtTreatmentCode.Properties.Appearance.Options.UseTextOptions = true;
            this.txtTreatmentCode.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.txtTreatmentCode.Properties.NullValuePrompt = "Mã điều trị";
            this.txtTreatmentCode.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtTreatmentCode.Properties.ShowNullValuePromptWhenFocused = true;
            this.txtTreatmentCode.Size = new System.Drawing.Size(91, 20);
            this.txtTreatmentCode.StyleController = this.layoutControl1;
            this.txtTreatmentCode.TabIndex = 5;
            this.txtTreatmentCode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtTreatmentCode_KeyDown);
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutExpMediStock,
            this.layoutPatientType,
            this.layoutVisitor,
            this.layoutExpPrice,
            this.layoutControlItem23,
            this.layoutControlItem24,
            this.layoutControlItem25,
            this.layoutControlItem4,
            this.layoutControlItem7,
            this.lciExpMestCode,
            this.layoutControlItem20,
            this.layoutControlItem26,
            this.emptySpaceItem1,
            this.layoutControlItem29,
            this.layoutControlItem30,
            this.layoutControlItem5,
            this.layoutPatient,
            this.lciTHX,
            this.layoutControlItem35,
            this.layoutControlItem34,
            this.layoutControlItem6,
            this.layoutControlItem36,
            this.layoutControlItem22,
            this.lciMediMatyForPrescription,
            this.layoutTutorial,
            this.layoutControlItem38,
            this.layoutControlItem3,
            this.layoutControlItem27,
            this.layoutAmount,
            this.layoutControlItem28,
            this.layoutExpVatRatio,
            this.layoutControlItem15,
            this.layoutControlItem19,
            this.layoutControlItem17,
            this.layoutNote,
            this.layoutControlItem21,
            this.layoutControlItem10,
            this.layoutControlItem2,
            this.layoutControlItem18,
            this.layoutControlItem1,
            this.layoutControlItem31,
            this.layoutPrescriptionCode,
            this.layoutControlItem37,
            this.lciPatientCode,
            this.lciPhone,
            this.layoutDescription,
            this.layoutImportExpPrice,
            this.emptySpaceItem4,
            this.layoutControlItem33,
            this.layoutControlItem42,
            this.layoutControlItem43,
            this.layoutControlItem44,
            this.layoutControlItem45,
            this.emptySpaceItem2,
            this.layoutControlItem40,
            this.layoutControlItem49,
            this.layoutControlItem50,
            this.layoutControlItem51});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "Root";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(1420, 641);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutExpMediStock
            // 
            this.layoutExpMediStock.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.layoutExpMediStock.AppearanceItemCaption.Options.UseForeColor = true;
            this.layoutExpMediStock.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutExpMediStock.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutExpMediStock.Control = this.txtExpMediStock;
            this.layoutExpMediStock.Location = new System.Drawing.Point(0, 0);
            this.layoutExpMediStock.Name = "layoutExpMediStock";
            this.layoutExpMediStock.Size = new System.Drawing.Size(329, 26);
            this.layoutExpMediStock.Text = "Kho xuất:";
            this.layoutExpMediStock.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutExpMediStock.TextSize = new System.Drawing.Size(90, 20);
            this.layoutExpMediStock.TextToControlDistance = 5;
            // 
            // layoutPatientType
            // 
            this.layoutPatientType.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.layoutPatientType.AppearanceItemCaption.Options.UseForeColor = true;
            this.layoutPatientType.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutPatientType.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutPatientType.Control = this.cboPatientType;
            this.layoutPatientType.Location = new System.Drawing.Point(329, 0);
            this.layoutPatientType.Name = "layoutPatientType";
            this.layoutPatientType.Size = new System.Drawing.Size(169, 26);
            this.layoutPatientType.Text = "Đối tượng:";
            this.layoutPatientType.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutPatientType.TextSize = new System.Drawing.Size(70, 20);
            this.layoutPatientType.TextToControlDistance = 5;
            // 
            // layoutVisitor
            // 
            this.layoutVisitor.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutVisitor.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutVisitor.Control = this.checkIsVisitor;
            this.layoutVisitor.Location = new System.Drawing.Point(498, 0);
            this.layoutVisitor.Name = "layoutVisitor";
            this.layoutVisitor.Size = new System.Drawing.Size(86, 26);
            this.layoutVisitor.Text = "Vãng lai:";
            this.layoutVisitor.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutVisitor.TextSize = new System.Drawing.Size(50, 20);
            this.layoutVisitor.TextToControlDistance = 5;
            // 
            // layoutExpPrice
            // 
            this.layoutExpPrice.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutExpPrice.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutExpPrice.Control = this.spinExpPrice;
            this.layoutExpPrice.Location = new System.Drawing.Point(177, 148);
            this.layoutExpPrice.Name = "layoutExpPrice";
            this.layoutExpPrice.Size = new System.Drawing.Size(164, 24);
            this.layoutExpPrice.Text = "Giá bán:";
            this.layoutExpPrice.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutExpPrice.TextSize = new System.Drawing.Size(50, 20);
            this.layoutExpPrice.TextToControlDistance = 5;
            // 
            // layoutControlItem23
            // 
            this.layoutControlItem23.Control = this.btnSave;
            this.layoutControlItem23.Location = new System.Drawing.Point(787, 615);
            this.layoutControlItem23.Name = "layoutControlItem23";
            this.layoutControlItem23.Size = new System.Drawing.Size(89, 26);
            this.layoutControlItem23.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem23.TextVisible = false;
            // 
            // layoutControlItem24
            // 
            this.layoutControlItem24.Control = this.btnNew;
            this.layoutControlItem24.Location = new System.Drawing.Point(876, 615);
            this.layoutControlItem24.Name = "layoutControlItem24";
            this.layoutControlItem24.Size = new System.Drawing.Size(125, 26);
            this.layoutControlItem24.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem24.TextVisible = false;
            // 
            // layoutControlItem25
            // 
            this.layoutControlItem25.Control = this.ddBtnPrint;
            this.layoutControlItem25.Location = new System.Drawing.Point(1098, 615);
            this.layoutControlItem25.Name = "layoutControlItem25";
            this.layoutControlItem25.Size = new System.Drawing.Size(58, 26);
            this.layoutControlItem25.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem25.TextVisible = false;
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.Control = this.btnSavePrint;
            this.layoutControlItem4.Location = new System.Drawing.Point(686, 615);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Size = new System.Drawing.Size(101, 26);
            this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem4.TextVisible = false;
            // 
            // layoutControlItem7
            // 
            this.layoutControlItem7.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem7.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem7.Control = this.cboGender;
            this.layoutControlItem7.Location = new System.Drawing.Point(623, 26);
            this.layoutControlItem7.Name = "layoutControlItem7";
            this.layoutControlItem7.Size = new System.Drawing.Size(152, 26);
            this.layoutControlItem7.Text = "Giới tính:";
            this.layoutControlItem7.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem7.TextSize = new System.Drawing.Size(80, 20);
            this.layoutControlItem7.TextToControlDistance = 5;
            // 
            // lciExpMestCode
            // 
            this.lciExpMestCode.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciExpMestCode.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciExpMestCode.Control = this.lblExpMestCode;
            this.lciExpMestCode.Location = new System.Drawing.Point(0, 76);
            this.lciExpMestCode.Name = "lciExpMestCode";
            this.lciExpMestCode.OptionsToolTip.ToolTip = "Mã phiếu xuất";
            this.lciExpMestCode.Size = new System.Drawing.Size(148, 24);
            this.lciExpMestCode.Text = "Mã PX:";
            this.lciExpMestCode.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciExpMestCode.TextSize = new System.Drawing.Size(90, 20);
            this.lciExpMestCode.TextToControlDistance = 5;
            // 
            // layoutControlItem20
            // 
            this.layoutControlItem20.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem20.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem20.Control = this.txtLoginName;
            this.layoutControlItem20.Location = new System.Drawing.Point(329, 76);
            this.layoutControlItem20.Name = "layoutControlItem20";
            this.layoutControlItem20.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 0, 2, 2);
            this.layoutControlItem20.Size = new System.Drawing.Size(169, 24);
            this.layoutControlItem20.Text = "Bác sĩ:";
            this.layoutControlItem20.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem20.TextSize = new System.Drawing.Size(70, 20);
            this.layoutControlItem20.TextToControlDistance = 5;
            // 
            // layoutControlItem26
            // 
            this.layoutControlItem26.Control = this.btnSaleBill;
            this.layoutControlItem26.Location = new System.Drawing.Point(1204, 615);
            this.layoutControlItem26.Name = "layoutControlItem26";
            this.layoutControlItem26.Size = new System.Drawing.Size(130, 26);
            this.layoutControlItem26.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem26.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(0, 615);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(132, 26);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // layoutControlItem29
            // 
            this.layoutControlItem29.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem29.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem29.Control = this.spinProfit;
            this.layoutControlItem29.Location = new System.Drawing.Point(0, 148);
            this.layoutControlItem29.Name = "layoutControlItem29";
            this.layoutControlItem29.Size = new System.Drawing.Size(162, 24);
            this.layoutControlItem29.Text = "Lợi nhuận:";
            this.layoutControlItem29.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem29.TextSize = new System.Drawing.Size(90, 20);
            this.layoutControlItem29.TextToControlDistance = 5;
            // 
            // layoutControlItem30
            // 
            this.layoutControlItem30.Control = this.labelControl2;
            this.layoutControlItem30.Location = new System.Drawing.Point(162, 148);
            this.layoutControlItem30.Name = "layoutControlItem30";
            this.layoutControlItem30.Size = new System.Drawing.Size(15, 24);
            this.layoutControlItem30.Text = " ";
            this.layoutControlItem30.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem30.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem30.TextToControlDistance = 0;
            this.layoutControlItem30.TextVisible = false;
            // 
            // layoutControlItem5
            // 
            this.layoutControlItem5.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem5.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem5.Control = this.txtAddress;
            this.layoutControlItem5.Location = new System.Drawing.Point(498, 52);
            this.layoutControlItem5.Name = "layoutControlItem5";
            this.layoutControlItem5.Size = new System.Drawing.Size(195, 24);
            this.layoutControlItem5.Text = "Địa chỉ:";
            this.layoutControlItem5.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem5.TextSize = new System.Drawing.Size(70, 20);
            this.layoutControlItem5.TextToControlDistance = 5;
            // 
            // layoutPatient
            // 
            this.layoutPatient.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.layoutPatient.AppearanceItemCaption.Options.UseForeColor = true;
            this.layoutPatient.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutPatient.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutPatient.Control = this.txtVirPatientName;
            this.layoutPatient.Location = new System.Drawing.Point(498, 26);
            this.layoutPatient.Name = "layoutPatient";
            this.layoutPatient.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 2, 2, 2);
            this.layoutPatient.Size = new System.Drawing.Size(125, 26);
            this.layoutPatient.Text = "Bệnh nhân:";
            this.layoutPatient.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutPatient.TextSize = new System.Drawing.Size(0, 0);
            this.layoutPatient.TextToControlDistance = 0;
            this.layoutPatient.TextVisible = false;
            // 
            // lciTHX
            // 
            this.lciTHX.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciTHX.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciTHX.Control = this.txtMaTHX;
            this.lciTHX.Location = new System.Drawing.Point(0, 52);
            this.lciTHX.Name = "lciTHX";
            this.lciTHX.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 0, 2, 2);
            this.lciTHX.Size = new System.Drawing.Size(201, 24);
            this.lciTHX.Text = "T/H/X:";
            this.lciTHX.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciTHX.TextSize = new System.Drawing.Size(90, 20);
            this.lciTHX.TextToControlDistance = 5;
            // 
            // layoutControlItem35
            // 
            this.layoutControlItem35.Control = this.cboTHX;
            this.layoutControlItem35.Location = new System.Drawing.Point(201, 52);
            this.layoutControlItem35.Name = "layoutControlItem35";
            this.layoutControlItem35.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 2, 2, 2);
            this.layoutControlItem35.Size = new System.Drawing.Size(297, 24);
            this.layoutControlItem35.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem35.TextVisible = false;
            // 
            // layoutControlItem34
            // 
            this.layoutControlItem34.Control = this.txtAge;
            this.layoutControlItem34.Location = new System.Drawing.Point(970, 26);
            this.layoutControlItem34.MaxSize = new System.Drawing.Size(0, 24);
            this.layoutControlItem34.MinSize = new System.Drawing.Size(35, 24);
            this.layoutControlItem34.Name = "layoutControlItem34";
            this.layoutControlItem34.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 0, 2, 2);
            this.layoutControlItem34.Size = new System.Drawing.Size(37, 26);
            this.layoutControlItem34.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem34.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem34.TextVisible = false;
            // 
            // layoutControlItem6
            // 
            this.layoutControlItem6.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem6.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem6.Control = this.panel1;
            this.layoutControlItem6.Location = new System.Drawing.Point(775, 26);
            this.layoutControlItem6.Name = "layoutControlItem6";
            this.layoutControlItem6.Size = new System.Drawing.Size(195, 26);
            this.layoutControlItem6.Text = "Ngày sinh:";
            this.layoutControlItem6.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem6.TextSize = new System.Drawing.Size(70, 20);
            this.layoutControlItem6.TextToControlDistance = 5;
            // 
            // layoutControlItem36
            // 
            this.layoutControlItem36.Control = this.cboAge;
            this.layoutControlItem36.Location = new System.Drawing.Point(1007, 26);
            this.layoutControlItem36.Name = "layoutControlItem36";
            this.layoutControlItem36.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 2, 2, 2);
            this.layoutControlItem36.Size = new System.Drawing.Size(56, 26);
            this.layoutControlItem36.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem36.TextVisible = false;
            // 
            // layoutControlItem22
            // 
            this.layoutControlItem22.Control = this.txtPresUser;
            this.layoutControlItem22.Location = new System.Drawing.Point(498, 76);
            this.layoutControlItem22.Name = "layoutControlItem22";
            this.layoutControlItem22.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 2, 2, 2);
            this.layoutControlItem22.Size = new System.Drawing.Size(147, 24);
            this.layoutControlItem22.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem22.TextVisible = false;
            // 
            // lciMediMatyForPrescription
            // 
            this.lciMediMatyForPrescription.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.lciMediMatyForPrescription.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
            this.lciMediMatyForPrescription.AppearanceItemCaption.Options.UseFont = true;
            this.lciMediMatyForPrescription.AppearanceItemCaption.Options.UseForeColor = true;
            this.lciMediMatyForPrescription.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciMediMatyForPrescription.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciMediMatyForPrescription.Control = this.txtMediMatyForPrescription;
            this.lciMediMatyForPrescription.Location = new System.Drawing.Point(0, 124);
            this.lciMediMatyForPrescription.Name = "lciMediMatyForPrescription";
            this.lciMediMatyForPrescription.Size = new System.Drawing.Size(498, 24);
            this.lciMediMatyForPrescription.Text = "Chọn:";
            this.lciMediMatyForPrescription.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciMediMatyForPrescription.TextSize = new System.Drawing.Size(90, 20);
            this.lciMediMatyForPrescription.TextToControlDistance = 5;
            // 
            // layoutTutorial
            // 
            this.layoutTutorial.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutTutorial.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutTutorial.Control = this.txtTutorial;
            this.layoutTutorial.Location = new System.Drawing.Point(0, 172);
            this.layoutTutorial.Name = "layoutTutorial";
            this.layoutTutorial.Size = new System.Drawing.Size(498, 26);
            this.layoutTutorial.Text = "HDSD:";
            this.layoutTutorial.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutTutorial.TextSize = new System.Drawing.Size(90, 20);
            this.layoutTutorial.TextToControlDistance = 5;
            // 
            // layoutControlItem38
            // 
            this.layoutControlItem38.Control = this.treeListMediMate;
            this.layoutControlItem38.Location = new System.Drawing.Point(0, 198);
            this.layoutControlItem38.Name = "layoutControlItem38";
            this.layoutControlItem38.Size = new System.Drawing.Size(1063, 417);
            this.layoutControlItem38.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem38.TextVisible = false;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.btnNewExpMest;
            this.layoutControlItem3.Location = new System.Drawing.Point(1001, 615);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(97, 26);
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextVisible = false;
            // 
            // layoutControlItem27
            // 
            this.layoutControlItem27.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.layoutControlItem27.AppearanceItemCaption.Options.UseForeColor = true;
            this.layoutControlItem27.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem27.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem27.Control = this.dtIntructionTime;
            this.layoutControlItem27.Location = new System.Drawing.Point(148, 76);
            this.layoutControlItem27.Name = "layoutControlItem27";
            this.layoutControlItem27.Size = new System.Drawing.Size(181, 24);
            this.layoutControlItem27.Text = "Ngày kê:";
            this.layoutControlItem27.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem27.TextSize = new System.Drawing.Size(50, 20);
            this.layoutControlItem27.TextToControlDistance = 5;
            // 
            // layoutAmount
            // 
            this.layoutAmount.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.layoutAmount.AppearanceItemCaption.Options.UseForeColor = true;
            this.layoutAmount.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutAmount.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutAmount.Control = this.spinAmount;
            this.layoutAmount.Location = new System.Drawing.Point(498, 124);
            this.layoutAmount.Name = "layoutAmount";
            this.layoutAmount.Size = new System.Drawing.Size(149, 24);
            this.layoutAmount.Text = "Số lượng:";
            this.layoutAmount.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutAmount.TextSize = new System.Drawing.Size(70, 20);
            this.layoutAmount.TextToControlDistance = 5;
            // 
            // layoutControlItem28
            // 
            this.layoutControlItem28.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem28.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem28.Control = this.spinDayNum;
            this.layoutControlItem28.Location = new System.Drawing.Point(647, 124);
            this.layoutControlItem28.Name = "layoutControlItem28";
            this.layoutControlItem28.Size = new System.Drawing.Size(152, 24);
            this.layoutControlItem28.Text = "Số ngày:";
            this.layoutControlItem28.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem28.TextSize = new System.Drawing.Size(80, 20);
            this.layoutControlItem28.TextToControlDistance = 5;
            // 
            // layoutExpVatRatio
            // 
            this.layoutExpVatRatio.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutExpVatRatio.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutExpVatRatio.Control = this.spinExpVatRatio;
            this.layoutExpVatRatio.Location = new System.Drawing.Point(341, 148);
            this.layoutExpVatRatio.Name = "layoutExpVatRatio";
            this.layoutExpVatRatio.Size = new System.Drawing.Size(157, 24);
            this.layoutExpVatRatio.Text = "VAT (%):";
            this.layoutExpVatRatio.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutExpVatRatio.TextSize = new System.Drawing.Size(70, 20);
            this.layoutExpVatRatio.TextToControlDistance = 5;
            // 
            // layoutControlItem15
            // 
            this.layoutControlItem15.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem15.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem15.Control = this.spinDiscountDetail;
            this.layoutControlItem15.Location = new System.Drawing.Point(498, 148);
            this.layoutControlItem15.Name = "layoutControlItem15";
            this.layoutControlItem15.OptionsToolTip.ToolTip = "Chiết khấu theo thuốc";
            this.layoutControlItem15.Size = new System.Drawing.Size(291, 24);
            this.layoutControlItem15.Text = "Chiết khấu:";
            this.layoutControlItem15.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem15.TextSize = new System.Drawing.Size(70, 20);
            this.layoutControlItem15.TextToControlDistance = 5;
            // 
            // layoutControlItem19
            // 
            this.layoutControlItem19.Control = this.spinDiscountDetailRatio;
            this.layoutControlItem19.Location = new System.Drawing.Point(789, 148);
            this.layoutControlItem19.Name = "layoutControlItem19";
            this.layoutControlItem19.Size = new System.Drawing.Size(82, 24);
            this.layoutControlItem19.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem19.TextVisible = false;
            // 
            // layoutControlItem17
            // 
            this.layoutControlItem17.Control = this.labelControl1;
            this.layoutControlItem17.Location = new System.Drawing.Point(871, 148);
            this.layoutControlItem17.Name = "layoutControlItem17";
            this.layoutControlItem17.Size = new System.Drawing.Size(15, 24);
            this.layoutControlItem17.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem17.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem17.TextToControlDistance = 0;
            this.layoutControlItem17.TextVisible = false;
            // 
            // layoutNote
            // 
            this.layoutNote.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutNote.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutNote.Control = this.txtNote;
            this.layoutNote.Location = new System.Drawing.Point(498, 172);
            this.layoutNote.Name = "layoutNote";
            this.layoutNote.Size = new System.Drawing.Size(452, 26);
            this.layoutNote.Text = "Chú thích:";
            this.layoutNote.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutNote.TextSize = new System.Drawing.Size(70, 20);
            this.layoutNote.TextToControlDistance = 5;
            // 
            // layoutControlItem21
            // 
            this.layoutControlItem21.Control = this.btnAdd;
            this.layoutControlItem21.Location = new System.Drawing.Point(950, 172);
            this.layoutControlItem21.Name = "layoutControlItem21";
            this.layoutControlItem21.Size = new System.Drawing.Size(113, 26);
            this.layoutControlItem21.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem21.TextVisible = false;
            // 
            // layoutControlItem10
            // 
            this.layoutControlItem10.Control = this.layoutControl2;
            this.layoutControlItem10.Location = new System.Drawing.Point(1063, 0);
            this.layoutControlItem10.Name = "layoutControlItem10";
            this.layoutControlItem10.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlItem10.Size = new System.Drawing.Size(357, 615);
            this.layoutControlItem10.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem10.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem2.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem2.Control = this.chkPrintNow;
            this.layoutControlItem2.Location = new System.Drawing.Point(412, 615);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(143, 26);
            this.layoutControlItem2.Text = "Xem trước khi in:";
            this.layoutControlItem2.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem2.TextSize = new System.Drawing.Size(90, 20);
            this.layoutControlItem2.TextToControlDistance = 5;
            // 
            // layoutControlItem18
            // 
            this.layoutControlItem18.Control = this.btnDebt;
            this.layoutControlItem18.Location = new System.Drawing.Point(1334, 615);
            this.layoutControlItem18.Name = "layoutControlItem18";
            this.layoutControlItem18.Size = new System.Drawing.Size(86, 26);
            this.layoutControlItem18.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem18.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.btnCancelExport;
            this.layoutControlItem1.Location = new System.Drawing.Point(555, 615);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(131, 26);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // layoutControlItem31
            // 
            this.layoutControlItem31.Control = this.chkAutoShow;
            this.layoutControlItem31.Location = new System.Drawing.Point(132, 615);
            this.layoutControlItem31.Name = "layoutControlItem31";
            this.layoutControlItem31.Size = new System.Drawing.Size(280, 26);
            this.layoutControlItem31.Text = "Tự động hiển thị tồn kho các nhà thuốc";
            this.layoutControlItem31.TextSize = new System.Drawing.Size(187, 13);
            // 
            // layoutPrescriptionCode
            // 
            this.layoutPrescriptionCode.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutPrescriptionCode.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutPrescriptionCode.Control = this.txtPrescriptionCode;
            this.layoutPrescriptionCode.Location = new System.Drawing.Point(0, 26);
            this.layoutPrescriptionCode.Name = "layoutPrescriptionCode";
            this.layoutPrescriptionCode.Size = new System.Drawing.Size(201, 26);
            this.layoutPrescriptionCode.Text = "Mã:";
            this.layoutPrescriptionCode.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutPrescriptionCode.TextSize = new System.Drawing.Size(90, 20);
            this.layoutPrescriptionCode.TextToControlDistance = 5;
            // 
            // layoutControlItem37
            // 
            this.layoutControlItem37.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem37.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem37.Control = this.txtTreatmentCode;
            this.layoutControlItem37.Location = new System.Drawing.Point(201, 26);
            this.layoutControlItem37.Name = "layoutControlItem37";
            this.layoutControlItem37.Size = new System.Drawing.Size(95, 26);
            this.layoutControlItem37.Text = "Mã điều trị:";
            this.layoutControlItem37.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem37.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem37.TextToControlDistance = 0;
            // 
            // lciPatientCode
            // 
            this.lciPatientCode.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.lciPatientCode.AppearanceItemCaption.Options.UseForeColor = true;
            this.lciPatientCode.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciPatientCode.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciPatientCode.Control = this.txtPatientCode;
            this.lciPatientCode.Location = new System.Drawing.Point(362, 26);
            this.lciPatientCode.Name = "lciPatientCode";
            this.lciPatientCode.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 0, 2, 2);
            this.lciPatientCode.Size = new System.Drawing.Size(136, 26);
            this.lciPatientCode.Text = "Bệnh nhân:";
            this.lciPatientCode.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciPatientCode.TextSize = new System.Drawing.Size(70, 20);
            this.lciPatientCode.TextToControlDistance = 5;
            // 
            // lciPhone
            // 
            this.lciPhone.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciPhone.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciPhone.Control = this.txtPatientPhone;
            this.lciPhone.Location = new System.Drawing.Point(876, 52);
            this.lciPhone.Name = "lciPhone";
            this.lciPhone.Size = new System.Drawing.Size(187, 24);
            this.lciPhone.Text = "Điện thoại:";
            this.lciPhone.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciPhone.TextSize = new System.Drawing.Size(70, 20);
            this.lciPhone.TextToControlDistance = 5;
            // 
            // layoutDescription
            // 
            this.layoutDescription.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutDescription.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutDescription.Control = this.txtDescription;
            this.layoutDescription.Location = new System.Drawing.Point(707, 76);
            this.layoutDescription.Name = "layoutDescription";
            this.layoutDescription.Size = new System.Drawing.Size(356, 24);
            this.layoutDescription.Text = "Mô tả:";
            this.layoutDescription.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutDescription.TextSize = new System.Drawing.Size(80, 20);
            this.layoutDescription.TextToControlDistance = 5;
            // 
            // layoutImportExpPrice
            // 
            this.layoutImportExpPrice.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutImportExpPrice.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutImportExpPrice.Control = this.checkImpExpPrice;
            this.layoutImportExpPrice.Location = new System.Drawing.Point(799, 124);
            this.layoutImportExpPrice.Name = "layoutImportExpPrice";
            this.layoutImportExpPrice.Size = new System.Drawing.Size(264, 24);
            this.layoutImportExpPrice.Text = "Nhập lại giá:";
            this.layoutImportExpPrice.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutImportExpPrice.TextSize = new System.Drawing.Size(70, 20);
            this.layoutImportExpPrice.TextToControlDistance = 5;
            // 
            // emptySpaceItem4
            // 
            this.emptySpaceItem4.AllowHotTrack = false;
            this.emptySpaceItem4.Location = new System.Drawing.Point(886, 148);
            this.emptySpaceItem4.Name = "emptySpaceItem4";
            this.emptySpaceItem4.Size = new System.Drawing.Size(177, 24);
            this.emptySpaceItem4.TextSize = new System.Drawing.Size(0, 0);
            // 
            // layoutControlItem33
            // 
            this.layoutControlItem33.Control = this.btnSearchPres;
            this.layoutControlItem33.Location = new System.Drawing.Point(296, 26);
            this.layoutControlItem33.Name = "layoutControlItem33";
            this.layoutControlItem33.Size = new System.Drawing.Size(66, 26);
            this.layoutControlItem33.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem33.TextVisible = false;
            // 
            // layoutControlItem42
            // 
            this.layoutControlItem42.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem42.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem42.Control = this.txtSubIcdCode;
            this.layoutControlItem42.Location = new System.Drawing.Point(498, 100);
            this.layoutControlItem42.Name = "layoutControlItem42";
            this.layoutControlItem42.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 0, 2, 2);
            this.layoutControlItem42.Size = new System.Drawing.Size(190, 24);
            this.layoutControlItem42.Text = "Bệnh phụ:";
            this.layoutControlItem42.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem42.TextSize = new System.Drawing.Size(70, 20);
            this.layoutControlItem42.TextToControlDistance = 5;
            // 
            // layoutControlItem43
            // 
            this.layoutControlItem43.Control = this.txtIcd;
            this.layoutControlItem43.Location = new System.Drawing.Point(688, 100);
            this.layoutControlItem43.Name = "layoutControlItem43";
            this.layoutControlItem43.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 2, 2, 2);
            this.layoutControlItem43.Size = new System.Drawing.Size(338, 24);
            this.layoutControlItem43.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem43.TextVisible = false;
            // 
            // layoutControlItem44
            // 
            this.layoutControlItem44.Control = this.btnSubIcd;
            this.layoutControlItem44.Location = new System.Drawing.Point(1026, 100);
            this.layoutControlItem44.MaxSize = new System.Drawing.Size(0, 24);
            this.layoutControlItem44.MinSize = new System.Drawing.Size(25, 24);
            this.layoutControlItem44.Name = "layoutControlItem44";
            this.layoutControlItem44.Size = new System.Drawing.Size(37, 24);
            this.layoutControlItem44.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem44.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem44.TextVisible = false;
            // 
            // layoutControlItem45
            // 
            this.layoutControlItem45.Control = this.layoutControlIcd;
            this.layoutControlItem45.Location = new System.Drawing.Point(0, 100);
            this.layoutControlItem45.Name = "layoutControlItem45";
            this.layoutControlItem45.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlItem45.Size = new System.Drawing.Size(498, 24);
            this.layoutControlItem45.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem45.TextVisible = false;
            // 
            // emptySpaceItem2
            // 
            this.emptySpaceItem2.AllowHotTrack = false;
            this.emptySpaceItem2.Location = new System.Drawing.Point(665, 0);
            this.emptySpaceItem2.Name = "emptySpaceItem2";
            this.emptySpaceItem2.Size = new System.Drawing.Size(398, 26);
            this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
            // 
            // layoutControlItem40
            // 
            this.layoutControlItem40.Control = this.btnDonCu;
            this.layoutControlItem40.Location = new System.Drawing.Point(584, 0);
            this.layoutControlItem40.Name = "layoutControlItem40";
            this.layoutControlItem40.Size = new System.Drawing.Size(81, 26);
            this.layoutControlItem40.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem40.TextVisible = false;
            // 
            // layoutControlItem49
            // 
            this.layoutControlItem49.Control = this.chkEditUser;
            this.layoutControlItem49.Location = new System.Drawing.Point(645, 76);
            this.layoutControlItem49.Name = "layoutControlItem49";
            this.layoutControlItem49.Size = new System.Drawing.Size(62, 24);
            this.layoutControlItem49.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem49.TextVisible = false;
            // 
            // layoutControlItem50
            // 
            this.layoutControlItem50.Control = this.btnQr;
            this.layoutControlItem50.Location = new System.Drawing.Point(1156, 615);
            this.layoutControlItem50.Name = "layoutControlItem50";
            this.layoutControlItem50.Size = new System.Drawing.Size(48, 26);
            this.layoutControlItem50.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem50.TextVisible = false;
            this.layoutControlItem50.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            // 
            // layoutControlItem51
            // 
            this.layoutControlItem51.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem51.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem51.Control = this.txtEmail;
            this.layoutControlItem51.Location = new System.Drawing.Point(693, 52);
            this.layoutControlItem51.Name = "layoutControlItem51";
            this.layoutControlItem51.Size = new System.Drawing.Size(183, 24);
            this.layoutControlItem51.Text = "Email";
            this.layoutControlItem51.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem51.TextSize = new System.Drawing.Size(50, 20);
            this.layoutControlItem51.TextToControlDistance = 5;
            // 
            // dxValidationProvider_Save
            // 
            this.dxValidationProvider_Save.ValidationFailed += new DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventHandler(this.dxValidationProvider1_ValidationFailed);
            // 
            // dxValidationProvider_Add
            // 
            this.dxValidationProvider_Add.ValidationFailed += new DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventHandler(this.dxValidationProvider2_ValidationFailed);
            // 
            // UCExpMestSaleCreate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layoutControl1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Name = "UCExpMestSaleCreate";
            this.Size = new System.Drawing.Size(1420, 670);
            this.Load += new System.EventHandler(this.UCExpMestSaleCreate_Load);
            this.Leave += new System.EventHandler(this.UCExpMestSaleCreate_Leave);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtEmail.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkEditUser.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlIcd)).EndInit();
            this.layoutControlIcd.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem39)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtIcd.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSubIcdCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPatientPhone.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPatientCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkAutoShow.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl2)).EndInit();
            this.layoutControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.spinQuetThe.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ChkKetNoiPOS.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinBaseValue.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkRoundPrice.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinBillNumOrder.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboBillAccountBook.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit2View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboBillCashierRoom.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkCreateBill.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboPayForm.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinDiscount.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinTransferAmount.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.treeListResult)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinDiscountRatio.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem13)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem16)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTranferAmount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem14)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem11)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem12)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem32)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciCreateBill)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBillCashierRoom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBillAccountBook)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBillNumOrder)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciRoundPrice)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBaseValue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTotalReceivable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTransactionCode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem41)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem46)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcQuetthe)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem47)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem48)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkPrintNow.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.popupControlContainerMediMaty)).EndInit();
            this.popupControlContainerMediMaty.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControlMediMaty)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewMediMaty)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMediMatyForPrescription.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.popupControlContainer1)).EndInit();
            this.popupControlContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControlPopupUser)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewPopupUser)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboAge.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAge.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboTHX.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMaTHX.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinProfit.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinDayNum.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtIntructionTime.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtIntructionTime.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtLoginName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinDiscountDetailRatio.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinDiscountDetail.Properties)).EndInit();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtPatientDob.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtPatientDob.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtPatientDob.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAddress.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtNote.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTutorial.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinExpVatRatio.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinExpPrice.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkImpExpPrice.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinAmount.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDescription.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtVirPatientName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPrescriptionCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkIsVisitor.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboPatientType.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtExpMediStock.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPresUser.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.treeListMediMate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSpinEdit__Amount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemBtnView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageCollection1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboGender.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTreatmentCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutExpMediStock)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutPatientType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutVisitor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutExpPrice)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem23)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem24)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem25)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciExpMestCode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem20)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem26)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem29)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem30)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutPatient)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTHX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem35)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem34)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem36)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem22)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciMediMatyForPrescription)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutTutorial)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem38)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem27)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutAmount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem28)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutExpVatRatio)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem15)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem19)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem17)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutNote)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem21)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem10)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem18)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem31)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutPrescriptionCode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem37)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPatientCode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPhone)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutDescription)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutImportExpPrice)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem33)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem42)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem43)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem44)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem45)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem40)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem49)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem50)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem51)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider_Save)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider_Add)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.DropDownButton ddBtnPrint;
        private DevExpress.XtraEditors.SimpleButton btnNew;
        private DevExpress.XtraEditors.SimpleButton btnSave;
        private DevExpress.XtraEditors.SimpleButton btnAdd;
        private DevExpress.XtraEditors.TextEdit txtNote;
        private DevExpress.XtraEditors.TextEdit txtTutorial;
        private DevExpress.XtraEditors.SpinEdit spinExpVatRatio;
        private DevExpress.XtraEditors.SpinEdit spinExpPrice;
        private DevExpress.XtraEditors.CheckEdit checkImpExpPrice;
        private DevExpress.XtraEditors.SpinEdit spinAmount;
        private DevExpress.XtraEditors.TextEdit txtDescription;
        private DevExpress.XtraEditors.TextEdit txtVirPatientName;
        private DevExpress.XtraEditors.TextEdit txtPrescriptionCode;
        private DevExpress.XtraEditors.CheckEdit checkIsVisitor;
        private DevExpress.XtraEditors.LookUpEdit cboPatientType;
        private DevExpress.XtraLayout.LayoutControlItem layoutExpMediStock;
        private DevExpress.XtraLayout.LayoutControlItem layoutPatientType;
        private DevExpress.XtraLayout.LayoutControlItem layoutVisitor;
        private DevExpress.XtraLayout.LayoutControlItem layoutPrescriptionCode;
        private DevExpress.XtraLayout.LayoutControlItem layoutPatient;
        private DevExpress.XtraLayout.LayoutControlItem layoutDescription;
        private DevExpress.XtraLayout.LayoutControlItem layoutAmount;
        private DevExpress.XtraLayout.LayoutControlItem layoutImportExpPrice;
        private DevExpress.XtraLayout.LayoutControlItem layoutExpPrice;
        private DevExpress.XtraLayout.LayoutControlItem layoutExpVatRatio;
        private DevExpress.XtraLayout.LayoutControlItem layoutTutorial;
        private DevExpress.XtraLayout.LayoutControlItem layoutNote;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem21;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem23;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem24;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem25;
        private DevExpress.XtraEditors.TextEdit txtExpMediStock;
        private DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProvider_Save;
        private DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProvider_Add;
        private DevExpress.XtraEditors.SimpleButton btnSavePrint;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem7;
        private DevExpress.XtraEditors.TextEdit txtAddress;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
        private System.Windows.Forms.Panel panel1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem6;
        private DevExpress.XtraEditors.DateEdit dtPatientDob;
        private DevExpress.XtraEditors.ButtonEdit txtPatientDob;
        private DevExpress.XtraEditors.LabelControl lblExpMestCode;
        private DevExpress.XtraLayout.LayoutControlItem lciExpMestCode;
        private DevExpress.XtraEditors.SpinEdit spinDiscount;
        private DevExpress.XtraEditors.SpinEdit spinDiscountDetail;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem15;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem17;
        private DevExpress.XtraEditors.SpinEdit spinDiscountDetailRatio;
        private DevExpress.XtraEditors.SpinEdit spinDiscountRatio;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem19;
        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.Bar bar1;
        private DevExpress.XtraBars.BarButtonItem btnCtrlA;
        private DevExpress.XtraBars.BarButtonItem btnCtrlI;
        private DevExpress.XtraBars.BarButtonItem btnCtrlS;
        private DevExpress.XtraBars.BarButtonItem btnCtrlN;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraBars.BarButtonItem barButtonItemCtrlF;
        private DevExpress.XtraEditors.TextEdit txtLoginName;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem20;
        private DevExpress.XtraEditors.SimpleButton btnSaleBill;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem26;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraBars.BarButtonItem bbtnSallBill_Manager;
        private DevExpress.XtraEditors.DateEdit dtIntructionTime;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem27;
        private DevExpress.XtraEditors.SpinEdit spinDayNum;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem28;
        private DevExpress.XtraEditors.SpinEdit spinProfit;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem29;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem30;
        private DevExpress.XtraEditors.LabelControl lblPayPrice;
        private DevExpress.XtraEditors.LabelControl lblTotalPrice;
        private DevExpress.XtraEditors.LookUpEdit cboTHX;
        private DevExpress.XtraEditors.TextEdit txtMaTHX;
        private DevExpress.XtraLayout.LayoutControlItem lciTHX;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem35;
        private DevExpress.XtraEditors.LookUpEdit cboAge;
        private DevExpress.XtraEditors.TextEdit txtAge;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem34;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem36;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem22;
        private DevExpress.XtraBars.PopupControlContainer popupControlContainer1;
        private DevExpress.XtraGrid.GridControl gridControlPopupUser;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewPopupUser;
        private DevExpress.XtraEditors.ButtonEdit txtPresUser;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem37;
        private DevExpress.XtraTreeList.TreeList treeListMediMate;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem38;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn1;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn2;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn3;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn4;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn5;
        private DevExpress.Utils.ImageCollection imageCollection1;
        private DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit repositoryItemSpinEdit__Amount;
        private DevExpress.XtraBars.PopupControlContainer popupControlContainerMediMaty;
        private DevExpress.XtraEditors.ButtonEdit txtMediMatyForPrescription;
        private DevExpress.XtraLayout.LayoutControlItem lciMediMatyForPrescription;
        private Inventec.Desktop.CustomControl.CustomGridControlWithFilterMultiColumn gridControlMediMaty;
        private Inventec.Desktop.CustomControl.CustomGridViewWithFilterMultiColumn gridViewMediMaty;
        private DevExpress.XtraEditors.SimpleButton btnCancelExport;
        private DevExpress.XtraBars.BarButtonItem barButtonCancelExport;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn6;
        private DevExpress.XtraTreeList.TreeList treeListResult;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn_Result_NAME;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn_Result_ExpAmount;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn_Result_ExpPrice;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn_Result_TotalPrice;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn_Result_ActiveIngrName;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn_Result_ServiceUnitName;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn_Result_Concentra;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn_Result_NationalName;
        private DevExpress.XtraEditors.SimpleButton btnNewExpMest;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn7;
        private DevExpress.XtraEditors.SpinEdit spinTransferAmount;
        private DevExpress.XtraEditors.CheckEdit chkPrintNow;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControl layoutControl2;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem12;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem13;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem16;
        private DevExpress.XtraLayout.LayoutControlItem lciTranferAmount;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem8;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem14;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem10;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem9;
        private DevExpress.XtraEditors.LookUpEdit cboPayForm;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem11;
        private DevExpress.XtraEditors.SimpleButton btnDebt;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem18;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn8;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit repositoryItemBtnView;
        private DevExpress.XtraEditors.CheckEdit chkAutoShow;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem31;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn9;
        private DevExpress.XtraEditors.LabelControl lblPresNumber;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem32;
        private DevExpress.XtraEditors.TextEdit txtPatientCode;
        private DevExpress.XtraLayout.LayoutControlItem lciPatientCode;
        private DevExpress.XtraEditors.TextEdit txtPatientPhone;
        private DevExpress.XtraLayout.LayoutControlItem lciPhone;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem4;
        private DevExpress.XtraEditors.GridLookUpEdit cboGender;
        private DevExpress.XtraGrid.Views.Grid.GridView gridLookUpEdit1View;
        private DevExpress.XtraEditors.SimpleButton btnSearchPres;
        private DevExpress.XtraEditors.TextEdit txtTreatmentCode;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem33;
        private DevExpress.XtraEditors.SimpleButton btnSubIcd;
        private DevExpress.XtraEditors.TextEdit txtIcd;
        private DevExpress.XtraEditors.TextEdit txtSubIcdCode;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem42;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem43;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem44;
        private DevExpress.XtraLayout.LayoutControl layoutControlIcd;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem45;
        private DevExpress.XtraEditors.LabelControl lblTotalReceivable;
        private DevExpress.XtraEditors.SpinEdit spinBaseValue;
        private DevExpress.XtraEditors.CheckEdit chkRoundPrice;
        private DevExpress.XtraEditors.SpinEdit spinBillNumOrder;
        private DevExpress.XtraEditors.GridLookUpEdit cboBillAccountBook;
        private DevExpress.XtraGrid.Views.Grid.GridView gridLookUpEdit2View;
        private DevExpress.XtraEditors.GridLookUpEdit cboBillCashierRoom;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraEditors.CheckEdit chkCreateBill;
        private DevExpress.XtraLayout.LayoutControlItem lciCreateBill;
        private DevExpress.XtraLayout.LayoutControlItem lciBillCashierRoom;
        private DevExpress.XtraLayout.LayoutControlItem lciBillAccountBook;
        private DevExpress.XtraLayout.LayoutControlItem lciBillNumOrder;
        private DevExpress.XtraLayout.LayoutControlItem lciRoundPrice;
        private DevExpress.XtraLayout.LayoutControlItem lciBaseValue;
        private DevExpress.XtraLayout.LayoutControlItem lciTotalReceivable;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
        private System.Windows.Forms.Panel panelIcd;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem39;
        private DevExpress.XtraEditors.LabelControl lblTransactionCode;
        private DevExpress.XtraLayout.LayoutControlItem lciTransactionCode;
        private DevExpress.XtraEditors.SimpleButton btnDonCu;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem40;
        private DevExpress.XtraBars.BarButtonItem barButtonF2;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn10;
        private DevExpress.XtraEditors.SimpleButton btnCauHinh;
        private DevExpress.XtraEditors.CheckEdit ChkKetNoiPOS;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem41;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem46;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem3;
        private DevExpress.XtraEditors.SpinEdit spinQuetThe;
        private DevExpress.XtraLayout.LayoutControlItem lcQuetthe;
        private DevExpress.XtraEditors.LabelControl labelControl4;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem47;
        private DevExpress.XtraEditors.LabelControl labelControl5;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem48;
		private DevExpress.XtraEditors.CheckEdit chkEditUser;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem49;
        private DevExpress.XtraEditors.DropDownButton btnQr;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem50;
        private DevExpress.XtraEditors.TextEdit txtEmail;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem51;
    }
}
