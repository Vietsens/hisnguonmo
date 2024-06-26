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
using Inventec.Desktop.CustomControl;
namespace HIS.Desktop.Plugins.TransactionBillOther
{
    partial class frmTransactionBillOther
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
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmTransactionBillOther));
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject1 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject2 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject3 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject4 = new DevExpress.Utils.SerializableAppearanceObject();
            this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.barBtnSaveAndPrint = new DevExpress.XtraBars.BarButtonItem();
            this.barBtnSave = new DevExpress.XtraBars.BarButtonItem();
            this.barBtnPrint = new DevExpress.XtraBars.BarButtonItem();
            this.barBtnNew = new DevExpress.XtraBars.BarButtonItem();
            this.barBtnSearch = new DevExpress.XtraBars.BarButtonItem();
            this.barBtnAdd = new DevExpress.XtraBars.BarButtonItem();
            this.btnF2 = new DevExpress.XtraBars.BarButtonItem();
            this.barBtnSaveAndSign = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.dxValidationProvider1 = new DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider(this.components);
            this.dxValidationProvider2 = new DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider(this.components);
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.layoutControl2 = new DevExpress.XtraLayout.LayoutControl();
            this.chkHideHddt = new DevExpress.XtraEditors.CheckEdit();
            this.btnSaveAndSign = new DevExpress.XtraEditors.SimpleButton();
            this.label2 = new System.Windows.Forms.Label();
            this.btnNew = new DevExpress.XtraEditors.SimpleButton();
            this.btnPrint = new DevExpress.XtraEditors.SimpleButton();
            this.btnSave = new DevExpress.XtraEditors.SimpleButton();
            this.txtExemptionReason = new DevExpress.XtraEditors.TextEdit();
            this.btnSaveAndPrint = new DevExpress.XtraEditors.SimpleButton();
            this.txtAccountBookCode = new DevExpress.XtraEditors.TextEdit();
            this.spinExemptionRation = new DevExpress.XtraEditors.SpinEdit();
            this.spinExemption = new DevExpress.XtraEditors.SpinEdit();
            this.spinNumOrder = new DevExpress.XtraEditors.SpinEdit();
            this.dtTransactionTime = new DevExpress.XtraEditors.DateEdit();
            this.spinTotalAmount = new DevExpress.XtraEditors.SpinEdit();
            this.cboAccountBook = new DevExpress.XtraEditors.LookUpEdit();
            this.cboPayForm = new DevExpress.XtraEditors.LookUpEdit();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lcAccountBookCode = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcExemption = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcNumOrder = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem19 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcPayForm = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcTransactionTime = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcExemptionReason = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcTotalAmount = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem3 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.layoutControlItem32 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem33 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem34 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem35 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem36 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem12 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciHideHddt = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem4 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.layoutControl4 = new DevExpress.XtraLayout.LayoutControl();
            this.layoutControl5 = new DevExpress.XtraLayout.LayoutControl();
            this.gridControlBillGoods = new Inventec.Desktop.CustomControl.MyGridControl();
            this.gridViewBillGoods = new Inventec.Desktop.CustomControl.MyGridView();
            this.gridColumn_BillGood_Stt = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_BillGood_Delete = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemButtonDelete = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.gridColumn_BillGood_GoodName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemTextGoodName = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
            this.gridColumn_BillGood_GoodUnitName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemTextGoodUnitName = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
            this.gridColumn_BillGood_Amount = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemSpinAmount = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
            this.gridColumn_BillGood_Price = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemSpinPrice = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
            this.gridColumn_BillGood_Discount = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemSpinDiscount = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
            this.gridColumn_BillGood_TotalPrice = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colSTTUnb = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colDeleteUnb = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colGOOD_NAMEUnb = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colGOOD_UNIT_NAMEUnb = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colAMOUNTUnb = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colPRICEUnb = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colDISCOUNTUnb = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colTOTAL_PRICEUnb = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colGOODS_NAMEUnb = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colGOODS_UNIT_NAMEUnb = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colTOTAL_PRICE_WITH_DISCOUNTUnb = new DevExpress.XtraGrid.Columns.GridColumn();
            this.layoutControlGroup4 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem28 = new DevExpress.XtraLayout.LayoutControlItem();
            this.label1 = new System.Windows.Forms.Label();
            this.txtGoodDescription = new DevExpress.XtraEditors.TextEdit();
            this.btnAdd = new System.Windows.Forms.Button();
            this.txtGoodUnitName = new DevExpress.XtraEditors.TextEdit();
            this.spinGoodPrice = new DevExpress.XtraEditors.SpinEdit();
            this.spinGoodAmount = new DevExpress.XtraEditors.SpinEdit();
            this.spinGoodDiscountRatio = new DevExpress.XtraEditors.SpinEdit();
            this.spinGoodDiscount = new DevExpress.XtraEditors.SpinEdit();
            this.cboNoneMediService = new Inventec.Desktop.CustomControl.CustomGridLookUpEditWithFilterMultiColumn();
            this.gridLookUpEdit1View = new Inventec.Desktop.CustomControl.CustomGridViewWithFilterMultiColumn();
            this.layoutControlGroup3 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lcGoodUnitName = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcGoodDiscount = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcGoodAmount = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcGoodPrice = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem26 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcGoodDescription = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem24 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem25 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem27 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.layoutControl3 = new DevExpress.XtraLayout.LayoutControl();
            this.txtBuyerAddress = new DevExpress.XtraEditors.TextEdit();
            this.txtBuyerAccountNumber = new DevExpress.XtraEditors.TextEdit();
            this.txtBuyerTaxCode = new DevExpress.XtraEditors.TextEdit();
            this.btnSearch = new DevExpress.XtraEditors.SimpleButton();
            this.chkCheckXD = new DevExpress.XtraEditors.CheckEdit();
            this.txtDescription = new DevExpress.XtraEditors.TextEdit();
            this.txtBuyerOrganization = new DevExpress.XtraEditors.TextEdit();
            this.txtPatientName = new DevExpress.XtraEditors.TextEdit();
            this.txtTreatmentCodeSearch = new DevExpress.XtraEditors.TextEdit();
            this.layoutControlGroup2 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.LcTreatmentCodeSearch = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcPatientName = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcBuyerOrganization = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcDescription = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcChk = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem9 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcBuyerTaxCode = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcBuyerAccountNumber = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcPatientAddress = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl2)).BeginInit();
            this.layoutControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chkHideHddt.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtExemptionReason.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAccountBookCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinExemptionRation.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinExemption.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinNumOrder.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtTransactionTime.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtTransactionTime.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinTotalAmount.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboAccountBook.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboPayForm.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcAccountBookCode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcExemption)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcNumOrder)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem19)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcPayForm)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcTransactionTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcExemptionReason)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcTotalAmount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem32)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem33)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem34)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem35)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem36)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem12)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciHideHddt)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem4)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl4)).BeginInit();
            this.layoutControl4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl5)).BeginInit();
            this.layoutControl5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlBillGoods)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewBillGoods)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemButtonDelete)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemTextGoodName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemTextGoodUnitName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSpinAmount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSpinPrice)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSpinDiscount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem28)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtGoodDescription.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtGoodUnitName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinGoodPrice.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinGoodAmount.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinGoodDiscountRatio.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinGoodDiscount.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboNoneMediService.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcGoodUnitName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcGoodDiscount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcGoodAmount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcGoodPrice)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem26)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcGoodDescription)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem24)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem25)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem27)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl3)).BeginInit();
            this.layoutControl3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtBuyerAddress.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBuyerAccountNumber.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBuyerTaxCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkCheckXD.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDescription.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBuyerOrganization.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPatientName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTreatmentCodeSearch.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.LcTreatmentCodeSearch)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcPatientName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcBuyerOrganization)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcDescription)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcChk)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcBuyerTaxCode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcBuyerAccountNumber)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcPatientAddress)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            this.SuspendLayout();
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
            this.barBtnSaveAndPrint,
            this.barBtnSave,
            this.barBtnPrint,
            this.barBtnNew,
            this.barBtnSearch,
            this.barBtnAdd,
            this.btnF2,
            this.barBtnSaveAndSign});
            this.barManager1.MaxItemId = 8;
            // 
            // bar1
            // 
            this.bar1.BarName = "Tools";
            this.bar1.DockCol = 0;
            this.bar1.DockRow = 0;
            this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.barBtnSaveAndPrint),
            new DevExpress.XtraBars.LinkPersistInfo(this.barBtnSave),
            new DevExpress.XtraBars.LinkPersistInfo(this.barBtnPrint),
            new DevExpress.XtraBars.LinkPersistInfo(this.barBtnNew),
            new DevExpress.XtraBars.LinkPersistInfo(this.barBtnSearch),
            new DevExpress.XtraBars.LinkPersistInfo(this.barBtnAdd),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnF2),
            new DevExpress.XtraBars.LinkPersistInfo(this.barBtnSaveAndSign)});
            this.bar1.Text = "Tools";
            this.bar1.Visible = false;
            // 
            // barBtnSaveAndPrint
            // 
            this.barBtnSaveAndPrint.Caption = "Lưu in (Ctrl I)";
            this.barBtnSaveAndPrint.Id = 0;
            this.barBtnSaveAndPrint.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.I));
            this.barBtnSaveAndPrint.Name = "barBtnSaveAndPrint";
            this.barBtnSaveAndPrint.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barBtnSaveAndPrint_ItemClick);
            // 
            // barBtnSave
            // 
            this.barBtnSave.Caption = "Lưu (Ctrl S)";
            this.barBtnSave.Id = 1;
            this.barBtnSave.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S));
            this.barBtnSave.Name = "barBtnSave";
            this.barBtnSave.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barBtnSave_ItemClick);
            // 
            // barBtnPrint
            // 
            this.barBtnPrint.Caption = "In (Ctrl P)";
            this.barBtnPrint.Id = 2;
            this.barBtnPrint.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P));
            this.barBtnPrint.Name = "barBtnPrint";
            this.barBtnPrint.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barBtnPrint_ItemClick);
            // 
            // barBtnNew
            // 
            this.barBtnNew.Caption = "Mới (Ctrl N)";
            this.barBtnNew.Id = 3;
            this.barBtnNew.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N));
            this.barBtnNew.Name = "barBtnNew";
            this.barBtnNew.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barBtnNew_ItemClick);
            // 
            // barBtnSearch
            // 
            this.barBtnSearch.Caption = "Tìm (Ctrl F)";
            this.barBtnSearch.Id = 4;
            this.barBtnSearch.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F));
            this.barBtnSearch.Name = "barBtnSearch";
            this.barBtnSearch.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barBtnSearch_ItemClick);
            // 
            // barBtnAdd
            // 
            this.barBtnAdd.Caption = "Bổ sung (Ctrl A)";
            this.barBtnAdd.Id = 5;
            this.barBtnAdd.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A));
            this.barBtnAdd.Name = "barBtnAdd";
            this.barBtnAdd.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barBtnAdd_ItemClick);
            // 
            // btnF2
            // 
            this.btnF2.Caption = "F2";
            this.btnF2.Id = 6;
            this.btnF2.ItemShortcut = new DevExpress.XtraBars.BarShortcut(System.Windows.Forms.Keys.F2);
            this.btnF2.Name = "btnF2";
            this.btnF2.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnF2_ItemClick);
            // 
            // barBtnSaveAndSign
            // 
            this.barBtnSaveAndSign.Caption = "Ctrl D";
            this.barBtnSaveAndSign.Id = 7;
            this.barBtnSaveAndSign.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D));
            this.barBtnSaveAndSign.Name = "barBtnSaveAndSign";
            this.barBtnSaveAndSign.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barBtnSaveAndSign_ItemClick);
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Size = new System.Drawing.Size(880, 29);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 561);
            this.barDockControlBottom.Size = new System.Drawing.Size(880, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 29);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 532);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(880, 29);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 532);
            // 
            // dxValidationProvider1
            // 
            this.dxValidationProvider1.ValidationFailed += new DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventHandler(this.dxValidationProvider1_ValidationFailed);
            // 
            // dxValidationProvider2
            // 
            this.dxValidationProvider2.ValidationFailed += new DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventHandler(this.dxValidationProvider2_ValidationFailed);
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.layoutControl2);
            this.layoutControl1.Controls.Add(this.groupBox2);
            this.layoutControl1.Controls.Add(this.groupBox1);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 29);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(880, 532);
            this.layoutControl1.TabIndex = 8;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // layoutControl2
            // 
            this.layoutControl2.Controls.Add(this.chkHideHddt);
            this.layoutControl2.Controls.Add(this.btnSaveAndSign);
            this.layoutControl2.Controls.Add(this.label2);
            this.layoutControl2.Controls.Add(this.btnNew);
            this.layoutControl2.Controls.Add(this.btnPrint);
            this.layoutControl2.Controls.Add(this.btnSave);
            this.layoutControl2.Controls.Add(this.txtExemptionReason);
            this.layoutControl2.Controls.Add(this.btnSaveAndPrint);
            this.layoutControl2.Controls.Add(this.txtAccountBookCode);
            this.layoutControl2.Controls.Add(this.spinExemptionRation);
            this.layoutControl2.Controls.Add(this.spinExemption);
            this.layoutControl2.Controls.Add(this.spinNumOrder);
            this.layoutControl2.Controls.Add(this.dtTransactionTime);
            this.layoutControl2.Controls.Add(this.spinTotalAmount);
            this.layoutControl2.Controls.Add(this.cboAccountBook);
            this.layoutControl2.Controls.Add(this.cboPayForm);
            this.layoutControl2.Location = new System.Drawing.Point(2, 428);
            this.layoutControl2.Name = "layoutControl2";
            this.layoutControl2.Root = this.Root;
            this.layoutControl2.Size = new System.Drawing.Size(876, 102);
            this.layoutControl2.TabIndex = 22;
            this.layoutControl2.Text = "layoutControl2";
            // 
            // chkHideHddt
            // 
            this.chkHideHddt.Location = new System.Drawing.Point(315, 74);
            this.chkHideHddt.MenuManager = this.barManager1;
            this.chkHideHddt.Name = "chkHideHddt";
            this.chkHideHddt.Properties.Caption = "";
            this.chkHideHddt.Size = new System.Drawing.Size(19, 19);
            this.chkHideHddt.StyleController = this.layoutControl2;
            this.chkHideHddt.TabIndex = 37;
            this.chkHideHddt.CheckedChanged += new System.EventHandler(this.chkHideHddt_CheckedChanged);
            // 
            // btnSaveAndSign
            // 
            this.btnSaveAndSign.Location = new System.Drawing.Point(338, 74);
            this.btnSaveAndSign.Name = "btnSaveAndSign";
            this.btnSaveAndSign.Size = new System.Drawing.Size(105, 22);
            this.btnSaveAndSign.StyleController = this.layoutControl2;
            this.btnSaveAndSign.TabIndex = 36;
            this.btnSaveAndSign.Text = "Lưu ký (Ctrl D)";
            this.btnSaveAndSign.Click += new System.EventHandler(this.btnSaveAndSign_Click);
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(416, 26);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(20, 20);
            this.label2.TabIndex = 31;
            this.label2.Text = "%";
            // 
            // btnNew
            // 
            this.btnNew.Location = new System.Drawing.Point(771, 74);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(103, 22);
            this.btnNew.StyleController = this.layoutControl2;
            this.btnNew.TabIndex = 35;
            this.btnNew.Text = "Mới (Ctrl N)";
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // btnPrint
            // 
            this.btnPrint.Location = new System.Drawing.Point(664, 74);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(103, 22);
            this.btnPrint.StyleController = this.layoutControl2;
            this.btnPrint.TabIndex = 34;
            this.btnPrint.Text = "In (Ctrl P)";
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(556, 74);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(104, 22);
            this.btnSave.StyleController = this.layoutControl2;
            this.btnSave.TabIndex = 33;
            this.btnSave.Text = "Lưu (Ctrl S)";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // txtExemptionReason
            // 
            this.txtExemptionReason.Location = new System.Drawing.Point(512, 26);
            this.txtExemptionReason.MenuManager = this.barManager1;
            this.txtExemptionReason.Name = "txtExemptionReason";
            this.txtExemptionReason.Size = new System.Drawing.Size(362, 20);
            this.txtExemptionReason.StyleController = this.layoutControl2;
            this.txtExemptionReason.TabIndex = 29;
            this.txtExemptionReason.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtExemptionReason_PreviewKeyDown);
            // 
            // btnSaveAndPrint
            // 
            this.btnSaveAndPrint.Location = new System.Drawing.Point(447, 74);
            this.btnSaveAndPrint.Name = "btnSaveAndPrint";
            this.btnSaveAndPrint.Size = new System.Drawing.Size(105, 22);
            this.btnSaveAndPrint.StyleController = this.layoutControl2;
            this.btnSaveAndPrint.TabIndex = 32;
            this.btnSaveAndPrint.Text = "Lưu in (Ctrl I)";
            this.btnSaveAndPrint.Click += new System.EventHandler(this.btnSaveAndPrint_Click);
            // 
            // txtAccountBookCode
            // 
            this.txtAccountBookCode.Location = new System.Drawing.Point(97, 2);
            this.txtAccountBookCode.MenuManager = this.barManager1;
            this.txtAccountBookCode.Name = "txtAccountBookCode";
            this.txtAccountBookCode.Size = new System.Drawing.Size(115, 20);
            this.txtAccountBookCode.StyleController = this.layoutControl2;
            this.txtAccountBookCode.TabIndex = 23;
            this.txtAccountBookCode.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtAccountBookCode_PreviewKeyDown);
            // 
            // spinExemptionRation
            // 
            this.spinExemptionRation.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.spinExemptionRation.Location = new System.Drawing.Point(214, 26);
            this.spinExemptionRation.MenuManager = this.barManager1;
            this.spinExemptionRation.Name = "spinExemptionRation";
            this.spinExemptionRation.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.spinExemptionRation.Properties.DisplayFormat.FormatString = "#,##0.0000";
            this.spinExemptionRation.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.spinExemptionRation.Properties.EditFormat.FormatString = "#,##0.0000";
            this.spinExemptionRation.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.spinExemptionRation.Properties.EditValueChangedFiringMode = DevExpress.XtraEditors.Controls.EditValueChangedFiringMode.Default;
            this.spinExemptionRation.Properties.MaxValue = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.spinExemptionRation.Size = new System.Drawing.Size(198, 20);
            this.spinExemptionRation.StyleController = this.layoutControl2;
            this.spinExemptionRation.TabIndex = 28;
            this.spinExemptionRation.EditValueChanged += new System.EventHandler(this.spinExemptionRation_EditValueChanged);
            this.spinExemptionRation.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.spinExemptionRation_PreviewKeyDown);
            // 
            // spinExemption
            // 
            this.spinExemption.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.spinExemption.Location = new System.Drawing.Point(97, 26);
            this.spinExemption.MenuManager = this.barManager1;
            this.spinExemption.Name = "spinExemption";
            this.spinExemption.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.spinExemption.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.spinExemption.Properties.DisplayFormat.FormatString = "#,##0.0000";
            this.spinExemption.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.spinExemption.Properties.EditFormat.FormatString = "#,##0.0000";
            this.spinExemption.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.spinExemption.Size = new System.Drawing.Size(113, 20);
            this.spinExemption.StyleController = this.layoutControl2;
            this.spinExemption.TabIndex = 27;
            this.spinExemption.EditValueChanged += new System.EventHandler(this.spinExemption_EditValueChanged);
            this.spinExemption.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.spinExemption_PreviewKeyDown);
            // 
            // spinNumOrder
            // 
            this.spinNumOrder.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.spinNumOrder.Location = new System.Drawing.Point(97, 50);
            this.spinNumOrder.MenuManager = this.barManager1;
            this.spinNumOrder.Name = "spinNumOrder";
            this.spinNumOrder.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.spinNumOrder.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.spinNumOrder.Properties.EditValueChangedFiringMode = DevExpress.XtraEditors.Controls.EditValueChangedFiringMode.Default;
            this.spinNumOrder.Properties.MaxValue = new decimal(new int[] {
            276447231,
            23283,
            0,
            0});
            this.spinNumOrder.Size = new System.Drawing.Size(316, 20);
            this.spinNumOrder.StyleController = this.layoutControl2;
            this.spinNumOrder.TabIndex = 30;
            this.spinNumOrder.Spin += new DevExpress.XtraEditors.Controls.SpinEventHandler(this.spinNumOrder_Spin);
            this.spinNumOrder.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.spinNumOrder_PreviewKeyDown);
            // 
            // dtTransactionTime
            // 
            this.dtTransactionTime.EditValue = null;
            this.dtTransactionTime.Location = new System.Drawing.Point(723, 2);
            this.dtTransactionTime.MenuManager = this.barManager1;
            this.dtTransactionTime.Name = "dtTransactionTime";
            this.dtTransactionTime.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.dtTransactionTime.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtTransactionTime.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtTransactionTime.Properties.DisplayFormat.FormatString = "dd/MM/yyyy HH:mm";
            this.dtTransactionTime.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtTransactionTime.Properties.EditFormat.FormatString = "dd/MM/yyyy HH:mm";
            this.dtTransactionTime.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtTransactionTime.Properties.Mask.EditMask = "dd/MM/yyyy HH:mm";
            this.dtTransactionTime.Size = new System.Drawing.Size(151, 20);
            this.dtTransactionTime.StyleController = this.layoutControl2;
            this.dtTransactionTime.TabIndex = 26;
            this.dtTransactionTime.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.dtTransactionTime_PreviewKeyDown);
            // 
            // spinTotalAmount
            // 
            this.spinTotalAmount.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.spinTotalAmount.Location = new System.Drawing.Point(512, 50);
            this.spinTotalAmount.MenuManager = this.barManager1;
            this.spinTotalAmount.Name = "spinTotalAmount";
            this.spinTotalAmount.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.spinTotalAmount.Properties.Appearance.ForeColor = System.Drawing.Color.Red;
            this.spinTotalAmount.Properties.Appearance.Options.UseForeColor = true;
            this.spinTotalAmount.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.spinTotalAmount.Properties.DisplayFormat.FormatString = "#,##0";
            this.spinTotalAmount.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.spinTotalAmount.Properties.EditValueChangedFiringMode = DevExpress.XtraEditors.Controls.EditValueChangedFiringMode.Default;
            this.spinTotalAmount.Properties.ReadOnly = true;
            this.spinTotalAmount.Size = new System.Drawing.Size(255, 20);
            this.spinTotalAmount.StyleController = this.layoutControl2;
            this.spinTotalAmount.TabIndex = 31;
            this.spinTotalAmount.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.spinTotalAmount_PreviewKeyDown);
            // 
            // cboAccountBook
            // 
            this.cboAccountBook.Location = new System.Drawing.Point(212, 2);
            this.cboAccountBook.MenuManager = this.barManager1;
            this.cboAccountBook.Name = "cboAccountBook";
            this.cboAccountBook.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.cboAccountBook.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboAccountBook.Properties.NullText = "";
            this.cboAccountBook.Size = new System.Drawing.Size(201, 20);
            this.cboAccountBook.StyleController = this.layoutControl2;
            this.cboAccountBook.TabIndex = 24;
            this.cboAccountBook.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboAccountBook_Closed);
            this.cboAccountBook.EditValueChanged += new System.EventHandler(this.cboAccountBook_EditValueChanged);
            // 
            // cboPayForm
            // 
            this.cboPayForm.Location = new System.Drawing.Point(512, 2);
            this.cboPayForm.MenuManager = this.barManager1;
            this.cboPayForm.Name = "cboPayForm";
            this.cboPayForm.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.cboPayForm.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboPayForm.Properties.NullText = "";
            this.cboPayForm.Size = new System.Drawing.Size(112, 20);
            this.cboPayForm.StyleController = this.layoutControl2;
            this.cboPayForm.TabIndex = 25;
            this.cboPayForm.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboPayForm_Closed);
            this.cboPayForm.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.cboPayForm_EditValueChanged);
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.False;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lcAccountBookCode,
            this.lcExemption,
            this.lcNumOrder,
            this.layoutControlItem19,
            this.lcPayForm,
            this.lcTransactionTime,
            this.lcExemptionReason,
            this.lcTotalAmount,
            this.emptySpaceItem3,
            this.layoutControlItem32,
            this.layoutControlItem33,
            this.layoutControlItem34,
            this.layoutControlItem35,
            this.layoutControlItem36,
            this.layoutControlItem12,
            this.layoutControlItem5,
            this.lciHideHddt,
            this.emptySpaceItem4});
            this.Root.Location = new System.Drawing.Point(0, 0);
            this.Root.Name = "Root";
            this.Root.Size = new System.Drawing.Size(876, 102);
            this.Root.TextVisible = false;
            // 
            // lcAccountBookCode
            // 
            this.lcAccountBookCode.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.lcAccountBookCode.AppearanceItemCaption.Options.UseForeColor = true;
            this.lcAccountBookCode.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lcAccountBookCode.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lcAccountBookCode.Control = this.txtAccountBookCode;
            this.lcAccountBookCode.Location = new System.Drawing.Point(0, 0);
            this.lcAccountBookCode.Name = "lcAccountBookCode";
            this.lcAccountBookCode.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 0, 2, 2);
            this.lcAccountBookCode.Size = new System.Drawing.Size(212, 24);
            this.lcAccountBookCode.Text = "Sổ thu chi:";
            this.lcAccountBookCode.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lcAccountBookCode.TextSize = new System.Drawing.Size(90, 20);
            this.lcAccountBookCode.TextToControlDistance = 5;
            // 
            // lcExemption
            // 
            this.lcExemption.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lcExemption.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lcExemption.Control = this.spinExemption;
            this.lcExemption.Location = new System.Drawing.Point(0, 24);
            this.lcExemption.Name = "lcExemption";
            this.lcExemption.Size = new System.Drawing.Size(212, 24);
            this.lcExemption.Text = "Miễn giảm:";
            this.lcExemption.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lcExemption.TextSize = new System.Drawing.Size(90, 20);
            this.lcExemption.TextToControlDistance = 5;
            // 
            // lcNumOrder
            // 
            this.lcNumOrder.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lcNumOrder.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lcNumOrder.Control = this.spinNumOrder;
            this.lcNumOrder.Location = new System.Drawing.Point(0, 48);
            this.lcNumOrder.Name = "lcNumOrder";
            this.lcNumOrder.Size = new System.Drawing.Size(415, 24);
            this.lcNumOrder.Text = "Số hóa đơn:";
            this.lcNumOrder.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lcNumOrder.TextSize = new System.Drawing.Size(90, 20);
            this.lcNumOrder.TextToControlDistance = 5;
            // 
            // layoutControlItem19
            // 
            this.layoutControlItem19.Control = this.btnSaveAndPrint;
            this.layoutControlItem19.Location = new System.Drawing.Point(445, 72);
            this.layoutControlItem19.Name = "layoutControlItem19";
            this.layoutControlItem19.Size = new System.Drawing.Size(109, 30);
            this.layoutControlItem19.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem19.TextVisible = false;
            // 
            // lcPayForm
            // 
            this.lcPayForm.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.lcPayForm.AppearanceItemCaption.Options.UseForeColor = true;
            this.lcPayForm.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lcPayForm.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lcPayForm.Control = this.cboPayForm;
            this.lcPayForm.Location = new System.Drawing.Point(415, 0);
            this.lcPayForm.Name = "lcPayForm";
            this.lcPayForm.Size = new System.Drawing.Size(211, 24);
            this.lcPayForm.Text = "Hình thức:";
            this.lcPayForm.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lcPayForm.TextSize = new System.Drawing.Size(90, 20);
            this.lcPayForm.TextToControlDistance = 5;
            // 
            // lcTransactionTime
            // 
            this.lcTransactionTime.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.lcTransactionTime.AppearanceItemCaption.Options.UseForeColor = true;
            this.lcTransactionTime.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lcTransactionTime.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lcTransactionTime.Control = this.dtTransactionTime;
            this.lcTransactionTime.Location = new System.Drawing.Point(626, 0);
            this.lcTransactionTime.Name = "lcTransactionTime";
            this.lcTransactionTime.Size = new System.Drawing.Size(250, 24);
            this.lcTransactionTime.Text = "Tg giao dịch:";
            this.lcTransactionTime.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lcTransactionTime.TextSize = new System.Drawing.Size(90, 20);
            this.lcTransactionTime.TextToControlDistance = 5;
            // 
            // lcExemptionReason
            // 
            this.lcExemptionReason.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lcExemptionReason.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lcExemptionReason.Control = this.txtExemptionReason;
            this.lcExemptionReason.Location = new System.Drawing.Point(438, 24);
            this.lcExemptionReason.Name = "lcExemptionReason";
            this.lcExemptionReason.Size = new System.Drawing.Size(438, 24);
            this.lcExemptionReason.Text = "Lý do:";
            this.lcExemptionReason.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lcExemptionReason.TextSize = new System.Drawing.Size(67, 20);
            this.lcExemptionReason.TextToControlDistance = 5;
            // 
            // lcTotalAmount
            // 
            this.lcTotalAmount.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lcTotalAmount.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lcTotalAmount.Control = this.spinTotalAmount;
            this.lcTotalAmount.Location = new System.Drawing.Point(415, 48);
            this.lcTotalAmount.Name = "lcTotalAmount";
            this.lcTotalAmount.Size = new System.Drawing.Size(354, 24);
            this.lcTotalAmount.Text = "Số tiền:";
            this.lcTotalAmount.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lcTotalAmount.TextSize = new System.Drawing.Size(90, 20);
            this.lcTotalAmount.TextToControlDistance = 5;
            // 
            // emptySpaceItem3
            // 
            this.emptySpaceItem3.AllowHotTrack = false;
            this.emptySpaceItem3.Location = new System.Drawing.Point(769, 48);
            this.emptySpaceItem3.Name = "emptySpaceItem3";
            this.emptySpaceItem3.Size = new System.Drawing.Size(107, 24);
            this.emptySpaceItem3.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.emptySpaceItem3.TextSize = new System.Drawing.Size(0, 0);
            // 
            // layoutControlItem32
            // 
            this.layoutControlItem32.Control = this.spinExemptionRation;
            this.layoutControlItem32.Location = new System.Drawing.Point(212, 24);
            this.layoutControlItem32.Name = "layoutControlItem32";
            this.layoutControlItem32.Size = new System.Drawing.Size(202, 24);
            this.layoutControlItem32.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem32.TextVisible = false;
            // 
            // layoutControlItem33
            // 
            this.layoutControlItem33.Control = this.cboAccountBook;
            this.layoutControlItem33.Location = new System.Drawing.Point(212, 0);
            this.layoutControlItem33.Name = "layoutControlItem33";
            this.layoutControlItem33.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 2, 2, 2);
            this.layoutControlItem33.Size = new System.Drawing.Size(203, 24);
            this.layoutControlItem33.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem33.TextVisible = false;
            // 
            // layoutControlItem34
            // 
            this.layoutControlItem34.Control = this.btnSave;
            this.layoutControlItem34.Location = new System.Drawing.Point(554, 72);
            this.layoutControlItem34.Name = "layoutControlItem34";
            this.layoutControlItem34.Size = new System.Drawing.Size(108, 30);
            this.layoutControlItem34.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem34.TextVisible = false;
            // 
            // layoutControlItem35
            // 
            this.layoutControlItem35.Control = this.btnPrint;
            this.layoutControlItem35.Location = new System.Drawing.Point(662, 72);
            this.layoutControlItem35.Name = "layoutControlItem35";
            this.layoutControlItem35.Size = new System.Drawing.Size(107, 30);
            this.layoutControlItem35.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem35.TextVisible = false;
            // 
            // layoutControlItem36
            // 
            this.layoutControlItem36.Control = this.btnNew;
            this.layoutControlItem36.Location = new System.Drawing.Point(769, 72);
            this.layoutControlItem36.Name = "layoutControlItem36";
            this.layoutControlItem36.Size = new System.Drawing.Size(107, 30);
            this.layoutControlItem36.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem36.TextVisible = false;
            // 
            // layoutControlItem12
            // 
            this.layoutControlItem12.Control = this.label2;
            this.layoutControlItem12.Location = new System.Drawing.Point(414, 24);
            this.layoutControlItem12.Name = "layoutControlItem12";
            this.layoutControlItem12.Size = new System.Drawing.Size(24, 24);
            this.layoutControlItem12.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem12.TextVisible = false;
            // 
            // layoutControlItem5
            // 
            this.layoutControlItem5.Control = this.btnSaveAndSign;
            this.layoutControlItem5.Location = new System.Drawing.Point(336, 72);
            this.layoutControlItem5.Name = "layoutControlItem5";
            this.layoutControlItem5.Size = new System.Drawing.Size(109, 30);
            this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem5.TextVisible = false;
            // 
            // lciHideHddt
            // 
            this.lciHideHddt.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciHideHddt.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciHideHddt.Control = this.chkHideHddt;
            this.lciHideHddt.Location = new System.Drawing.Point(188, 72);
            this.lciHideHddt.Name = "lciHideHddt";
            this.lciHideHddt.OptionsToolTip.ToolTip = "Không hiển thị hóa đơn điện tử";
            this.lciHideHddt.Size = new System.Drawing.Size(148, 30);
            this.lciHideHddt.Text = "Không hiển thị HĐ ĐT:";
            this.lciHideHddt.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciHideHddt.TextSize = new System.Drawing.Size(120, 20);
            this.lciHideHddt.TextToControlDistance = 5;
            // 
            // emptySpaceItem4
            // 
            this.emptySpaceItem4.AllowHotTrack = false;
            this.emptySpaceItem4.Location = new System.Drawing.Point(0, 72);
            this.emptySpaceItem4.Name = "emptySpaceItem4";
            this.emptySpaceItem4.Size = new System.Drawing.Size(188, 30);
            this.emptySpaceItem4.TextSize = new System.Drawing.Size(0, 0);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.layoutControl4);
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(2, 137);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(876, 287);
            this.groupBox2.TabIndex = 10;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Nội dung hóa đơn";
            // 
            // layoutControl4
            // 
            this.layoutControl4.Controls.Add(this.layoutControl5);
            this.layoutControl4.Controls.Add(this.label1);
            this.layoutControl4.Controls.Add(this.txtGoodDescription);
            this.layoutControl4.Controls.Add(this.btnAdd);
            this.layoutControl4.Controls.Add(this.txtGoodUnitName);
            this.layoutControl4.Controls.Add(this.spinGoodPrice);
            this.layoutControl4.Controls.Add(this.spinGoodAmount);
            this.layoutControl4.Controls.Add(this.spinGoodDiscountRatio);
            this.layoutControl4.Controls.Add(this.spinGoodDiscount);
            this.layoutControl4.Controls.Add(this.cboNoneMediService);
            this.layoutControl4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl4.Location = new System.Drawing.Point(3, 17);
            this.layoutControl4.Name = "layoutControl4";
            this.layoutControl4.OptionsFocus.EnableAutoTabOrder = false;
            this.layoutControl4.Root = this.layoutControlGroup3;
            this.layoutControl4.Size = new System.Drawing.Size(870, 267);
            this.layoutControl4.TabIndex = 11;
            this.layoutControl4.Text = "layoutControl4";
            // 
            // layoutControl5
            // 
            this.layoutControl5.Controls.Add(this.gridControlBillGoods);
            this.layoutControl5.Location = new System.Drawing.Point(2, 74);
            this.layoutControl5.Name = "layoutControl5";
            this.layoutControl5.Root = this.layoutControlGroup4;
            this.layoutControl5.Size = new System.Drawing.Size(866, 191);
            this.layoutControl5.TabIndex = 21;
            this.layoutControl5.Text = "layoutControl5";
            // 
            // gridControlBillGoods
            // 
            this.gridControlBillGoods.Location = new System.Drawing.Point(2, 2);
            this.gridControlBillGoods.MainView = this.gridViewBillGoods;
            this.gridControlBillGoods.Name = "gridControlBillGoods";
            this.gridControlBillGoods.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemButtonDelete,
            this.repositoryItemTextGoodName,
            this.repositoryItemSpinAmount,
            this.repositoryItemSpinPrice,
            this.repositoryItemSpinDiscount,
            this.repositoryItemTextGoodUnitName});
            this.gridControlBillGoods.Size = new System.Drawing.Size(862, 187);
            this.gridControlBillGoods.TabIndex = 21;
            this.gridControlBillGoods.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewBillGoods});
            // 
            // gridViewBillGoods
            // 
            this.gridViewBillGoods.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn_BillGood_Stt,
            this.gridColumn_BillGood_Delete,
            this.gridColumn_BillGood_GoodName,
            this.gridColumn_BillGood_GoodUnitName,
            this.gridColumn_BillGood_Amount,
            this.gridColumn_BillGood_Price,
            this.gridColumn_BillGood_Discount,
            this.gridColumn_BillGood_TotalPrice,
            this.colSTTUnb,
            this.colDeleteUnb,
            this.colGOOD_NAMEUnb,
            this.colGOOD_UNIT_NAMEUnb,
            this.colAMOUNTUnb,
            this.colPRICEUnb,
            this.colDISCOUNTUnb,
            this.colTOTAL_PRICEUnb,
            this.colGOODS_NAMEUnb,
            this.colGOODS_UNIT_NAMEUnb,
            this.colTOTAL_PRICE_WITH_DISCOUNTUnb});
            this.gridViewBillGoods.GridControl = this.gridControlBillGoods;
            this.gridViewBillGoods.Name = "gridViewBillGoods";
            this.gridViewBillGoods.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            this.gridViewBillGoods.OptionsView.ShowGroupPanel = false;
            this.gridViewBillGoods.OptionsView.ShowIndicator = false;
            // 
            // gridColumn_BillGood_Stt
            // 
            this.gridColumn_BillGood_Stt.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn_BillGood_Stt.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gridColumn_BillGood_Stt.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_BillGood_Stt.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_BillGood_Stt.Caption = "STT";
            this.gridColumn_BillGood_Stt.FieldName = "STT";
            this.gridColumn_BillGood_Stt.FieldNameSortGroup = "STTUnb";
            this.gridColumn_BillGood_Stt.Name = "gridColumn_BillGood_Stt";
            this.gridColumn_BillGood_Stt.OptionsColumn.AllowEdit = false;
            this.gridColumn_BillGood_Stt.OptionsFilter.FilterBySortField = DevExpress.Utils.DefaultBoolean.True;
            this.gridColumn_BillGood_Stt.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn_BillGood_Stt.Visible = true;
            this.gridColumn_BillGood_Stt.VisibleIndex = 0;
            this.gridColumn_BillGood_Stt.Width = 30;
            // 
            // gridColumn_BillGood_Delete
            // 
            this.gridColumn_BillGood_Delete.Caption = "Xóa";
            this.gridColumn_BillGood_Delete.ColumnEdit = this.repositoryItemButtonDelete;
            this.gridColumn_BillGood_Delete.FieldName = "Delete";
            this.gridColumn_BillGood_Delete.FieldNameSortGroup = "DeleteUnb";
            this.gridColumn_BillGood_Delete.Name = "gridColumn_BillGood_Delete";
            this.gridColumn_BillGood_Delete.OptionsColumn.ShowCaption = false;
            this.gridColumn_BillGood_Delete.OptionsFilter.FilterBySortField = DevExpress.Utils.DefaultBoolean.True;
            this.gridColumn_BillGood_Delete.ToolTip = "Xóa";
            this.gridColumn_BillGood_Delete.Visible = true;
            this.gridColumn_BillGood_Delete.VisibleIndex = 1;
            this.gridColumn_BillGood_Delete.Width = 20;
            // 
            // repositoryItemButtonDelete
            // 
            this.repositoryItemButtonDelete.AutoHeight = false;
            this.repositoryItemButtonDelete.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, ((System.Drawing.Image)(resources.GetObject("repositoryItemButtonDelete.Buttons"))), new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject1, serializableAppearanceObject2, serializableAppearanceObject3, serializableAppearanceObject4, "Xóa", null, null, true)});
            this.repositoryItemButtonDelete.Name = "repositoryItemButtonDelete";
            this.repositoryItemButtonDelete.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            this.repositoryItemButtonDelete.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repositoryItemButtonDelete_ButtonClick);
            // 
            // gridColumn_BillGood_GoodName
            // 
            this.gridColumn_BillGood_GoodName.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_BillGood_GoodName.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_BillGood_GoodName.Caption = "Tên dịch vụ";
            this.gridColumn_BillGood_GoodName.ColumnEdit = this.repositoryItemTextGoodName;
            this.gridColumn_BillGood_GoodName.FieldName = "GOODS_NAME";
            this.gridColumn_BillGood_GoodName.FieldNameSortGroup = "GOODS_NAMEUnb";
            this.gridColumn_BillGood_GoodName.Name = "gridColumn_BillGood_GoodName";
            this.gridColumn_BillGood_GoodName.OptionsFilter.FilterBySortField = DevExpress.Utils.DefaultBoolean.True;
            this.gridColumn_BillGood_GoodName.Visible = true;
            this.gridColumn_BillGood_GoodName.VisibleIndex = 2;
            this.gridColumn_BillGood_GoodName.Width = 250;
            // 
            // repositoryItemTextGoodName
            // 
            this.repositoryItemTextGoodName.AutoHeight = false;
            this.repositoryItemTextGoodName.Name = "repositoryItemTextGoodName";
            // 
            // gridColumn_BillGood_GoodUnitName
            // 
            this.gridColumn_BillGood_GoodUnitName.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_BillGood_GoodUnitName.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_BillGood_GoodUnitName.Caption = "Đơn vị tính";
            this.gridColumn_BillGood_GoodUnitName.ColumnEdit = this.repositoryItemTextGoodUnitName;
            this.gridColumn_BillGood_GoodUnitName.FieldName = "GOODS_UNIT_NAME";
            this.gridColumn_BillGood_GoodUnitName.FieldNameSortGroup = "GOODS_UNIT_NAMEUnb";
            this.gridColumn_BillGood_GoodUnitName.Name = "gridColumn_BillGood_GoodUnitName";
            this.gridColumn_BillGood_GoodUnitName.OptionsFilter.FilterBySortField = DevExpress.Utils.DefaultBoolean.True;
            this.gridColumn_BillGood_GoodUnitName.Visible = true;
            this.gridColumn_BillGood_GoodUnitName.VisibleIndex = 3;
            this.gridColumn_BillGood_GoodUnitName.Width = 70;
            // 
            // repositoryItemTextGoodUnitName
            // 
            this.repositoryItemTextGoodUnitName.AutoHeight = false;
            this.repositoryItemTextGoodUnitName.Name = "repositoryItemTextGoodUnitName";
            // 
            // gridColumn_BillGood_Amount
            // 
            this.gridColumn_BillGood_Amount.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn_BillGood_Amount.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gridColumn_BillGood_Amount.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_BillGood_Amount.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_BillGood_Amount.Caption = "Số lượng";
            this.gridColumn_BillGood_Amount.ColumnEdit = this.repositoryItemSpinAmount;
            this.gridColumn_BillGood_Amount.DisplayFormat.FormatString = "#,##0.00";
            this.gridColumn_BillGood_Amount.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.gridColumn_BillGood_Amount.FieldName = "AMOUNT";
            this.gridColumn_BillGood_Amount.FieldNameSortGroup = "AMOUNTUnb";
            this.gridColumn_BillGood_Amount.Name = "gridColumn_BillGood_Amount";
            this.gridColumn_BillGood_Amount.OptionsFilter.FilterBySortField = DevExpress.Utils.DefaultBoolean.True;
            this.gridColumn_BillGood_Amount.Visible = true;
            this.gridColumn_BillGood_Amount.VisibleIndex = 4;
            this.gridColumn_BillGood_Amount.Width = 100;
            // 
            // repositoryItemSpinAmount
            // 
            this.repositoryItemSpinAmount.AutoHeight = false;
            this.repositoryItemSpinAmount.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemSpinAmount.Name = "repositoryItemSpinAmount";
            // 
            // gridColumn_BillGood_Price
            // 
            this.gridColumn_BillGood_Price.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn_BillGood_Price.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gridColumn_BillGood_Price.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_BillGood_Price.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_BillGood_Price.Caption = "Đơn giá";
            this.gridColumn_BillGood_Price.ColumnEdit = this.repositoryItemSpinPrice;
            this.gridColumn_BillGood_Price.DisplayFormat.FormatString = "#,##0.0000";
            this.gridColumn_BillGood_Price.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.gridColumn_BillGood_Price.FieldName = "PRICE";
            this.gridColumn_BillGood_Price.FieldNameSortGroup = "PRICEUnb";
            this.gridColumn_BillGood_Price.Name = "gridColumn_BillGood_Price";
            this.gridColumn_BillGood_Price.OptionsFilter.FilterBySortField = DevExpress.Utils.DefaultBoolean.True;
            this.gridColumn_BillGood_Price.Visible = true;
            this.gridColumn_BillGood_Price.VisibleIndex = 5;
            this.gridColumn_BillGood_Price.Width = 140;
            // 
            // repositoryItemSpinPrice
            // 
            this.repositoryItemSpinPrice.AutoHeight = false;
            this.repositoryItemSpinPrice.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemSpinPrice.Name = "repositoryItemSpinPrice";
            // 
            // gridColumn_BillGood_Discount
            // 
            this.gridColumn_BillGood_Discount.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn_BillGood_Discount.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gridColumn_BillGood_Discount.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_BillGood_Discount.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_BillGood_Discount.Caption = "Chiết khấu";
            this.gridColumn_BillGood_Discount.ColumnEdit = this.repositoryItemSpinDiscount;
            this.gridColumn_BillGood_Discount.DisplayFormat.FormatString = "#,##0.0000";
            this.gridColumn_BillGood_Discount.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.gridColumn_BillGood_Discount.FieldName = "DISCOUNT";
            this.gridColumn_BillGood_Discount.FieldNameSortGroup = "DISCOUNTUnb";
            this.gridColumn_BillGood_Discount.Name = "gridColumn_BillGood_Discount";
            this.gridColumn_BillGood_Discount.OptionsFilter.FilterBySortField = DevExpress.Utils.DefaultBoolean.True;
            this.gridColumn_BillGood_Discount.Visible = true;
            this.gridColumn_BillGood_Discount.VisibleIndex = 6;
            this.gridColumn_BillGood_Discount.Width = 140;
            // 
            // repositoryItemSpinDiscount
            // 
            this.repositoryItemSpinDiscount.AutoHeight = false;
            this.repositoryItemSpinDiscount.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemSpinDiscount.Name = "repositoryItemSpinDiscount";
            // 
            // gridColumn_BillGood_TotalPrice
            // 
            this.gridColumn_BillGood_TotalPrice.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn_BillGood_TotalPrice.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gridColumn_BillGood_TotalPrice.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_BillGood_TotalPrice.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_BillGood_TotalPrice.Caption = "Thành tiền";
            this.gridColumn_BillGood_TotalPrice.DisplayFormat.FormatString = "#,##0.0000";
            this.gridColumn_BillGood_TotalPrice.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.gridColumn_BillGood_TotalPrice.FieldName = "TOTAL_PRICE_WITH_DISCOUNT";
            this.gridColumn_BillGood_TotalPrice.FieldNameSortGroup = "TOTAL_PRICE_WITH_DISCOUNTUnb";
            this.gridColumn_BillGood_TotalPrice.Name = "gridColumn_BillGood_TotalPrice";
            this.gridColumn_BillGood_TotalPrice.OptionsColumn.ReadOnly = true;
            this.gridColumn_BillGood_TotalPrice.OptionsFilter.FilterBySortField = DevExpress.Utils.DefaultBoolean.True;
            this.gridColumn_BillGood_TotalPrice.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn_BillGood_TotalPrice.Visible = true;
            this.gridColumn_BillGood_TotalPrice.VisibleIndex = 7;
            this.gridColumn_BillGood_TotalPrice.Width = 120;
            // 
            // colSTTUnb
            // 
            this.colSTTUnb.FieldName = "STTUnb";
            this.colSTTUnb.Name = "colSTTUnb";
            this.colSTTUnb.UnboundType = DevExpress.Data.UnboundColumnType.String;
            // 
            // colDeleteUnb
            // 
            this.colDeleteUnb.FieldName = "DeleteUnb";
            this.colDeleteUnb.Name = "colDeleteUnb";
            this.colDeleteUnb.UnboundType = DevExpress.Data.UnboundColumnType.String;
            // 
            // colGOOD_NAMEUnb
            // 
            this.colGOOD_NAMEUnb.FieldName = "GOOD_NAMEUnb";
            this.colGOOD_NAMEUnb.Name = "colGOOD_NAMEUnb";
            this.colGOOD_NAMEUnb.UnboundType = DevExpress.Data.UnboundColumnType.String;
            // 
            // colGOOD_UNIT_NAMEUnb
            // 
            this.colGOOD_UNIT_NAMEUnb.FieldName = "GOOD_UNIT_NAMEUnb";
            this.colGOOD_UNIT_NAMEUnb.Name = "colGOOD_UNIT_NAMEUnb";
            this.colGOOD_UNIT_NAMEUnb.UnboundType = DevExpress.Data.UnboundColumnType.String;
            // 
            // colAMOUNTUnb
            // 
            this.colAMOUNTUnb.FieldName = "AMOUNTUnb";
            this.colAMOUNTUnb.Name = "colAMOUNTUnb";
            this.colAMOUNTUnb.UnboundType = DevExpress.Data.UnboundColumnType.String;
            // 
            // colPRICEUnb
            // 
            this.colPRICEUnb.FieldName = "PRICEUnb";
            this.colPRICEUnb.Name = "colPRICEUnb";
            this.colPRICEUnb.UnboundType = DevExpress.Data.UnboundColumnType.String;
            // 
            // colDISCOUNTUnb
            // 
            this.colDISCOUNTUnb.FieldName = "DISCOUNTUnb";
            this.colDISCOUNTUnb.Name = "colDISCOUNTUnb";
            this.colDISCOUNTUnb.UnboundType = DevExpress.Data.UnboundColumnType.String;
            // 
            // colTOTAL_PRICEUnb
            // 
            this.colTOTAL_PRICEUnb.FieldName = "TOTAL_PRICEUnb";
            this.colTOTAL_PRICEUnb.Name = "colTOTAL_PRICEUnb";
            this.colTOTAL_PRICEUnb.UnboundType = DevExpress.Data.UnboundColumnType.String;
            // 
            // colGOODS_NAMEUnb
            // 
            this.colGOODS_NAMEUnb.FieldName = "GOODS_NAMEUnb";
            this.colGOODS_NAMEUnb.Name = "colGOODS_NAMEUnb";
            this.colGOODS_NAMEUnb.UnboundType = DevExpress.Data.UnboundColumnType.String;
            // 
            // colGOODS_UNIT_NAMEUnb
            // 
            this.colGOODS_UNIT_NAMEUnb.FieldName = "GOODS_UNIT_NAMEUnb";
            this.colGOODS_UNIT_NAMEUnb.Name = "colGOODS_UNIT_NAMEUnb";
            this.colGOODS_UNIT_NAMEUnb.UnboundType = DevExpress.Data.UnboundColumnType.String;
            // 
            // colTOTAL_PRICE_WITH_DISCOUNTUnb
            // 
            this.colTOTAL_PRICE_WITH_DISCOUNTUnb.FieldName = "TOTAL_PRICE_WITH_DISCOUNTUnb";
            this.colTOTAL_PRICE_WITH_DISCOUNTUnb.Name = "colTOTAL_PRICE_WITH_DISCOUNTUnb";
            this.colTOTAL_PRICE_WITH_DISCOUNTUnb.UnboundType = DevExpress.Data.UnboundColumnType.String;
            // 
            // layoutControlGroup4
            // 
            this.layoutControlGroup4.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.False;
            this.layoutControlGroup4.GroupBordersVisible = false;
            this.layoutControlGroup4.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem28});
            this.layoutControlGroup4.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup4.Name = "layoutControlGroup4";
            this.layoutControlGroup4.Size = new System.Drawing.Size(866, 191);
            this.layoutControlGroup4.TextVisible = false;
            // 
            // layoutControlItem28
            // 
            this.layoutControlItem28.Control = this.gridControlBillGoods;
            this.layoutControlItem28.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem28.Name = "layoutControlItem28";
            this.layoutControlItem28.Size = new System.Drawing.Size(866, 191);
            this.layoutControlItem28.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem28.TextVisible = false;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(324, 50);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(20, 20);
            this.label1.TabIndex = 18;
            this.label1.Text = "%";
            // 
            // txtGoodDescription
            // 
            this.txtGoodDescription.Location = new System.Drawing.Point(443, 50);
            this.txtGoodDescription.MenuManager = this.barManager1;
            this.txtGoodDescription.Name = "txtGoodDescription";
            this.txtGoodDescription.Size = new System.Drawing.Size(329, 20);
            this.txtGoodDescription.StyleController = this.layoutControl4;
            this.txtGoodDescription.TabIndex = 19;
            this.txtGoodDescription.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtGoodDescription_KeyDown);
            // 
            // btnAdd
            // 
            this.btnAdd.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAdd.Location = new System.Drawing.Point(776, 2);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(92, 68);
            this.btnAdd.TabIndex = 20;
            this.btnAdd.Text = "Bổ sung (Ctrl A)";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // txtGoodUnitName
            // 
            this.txtGoodUnitName.Location = new System.Drawing.Point(97, 26);
            this.txtGoodUnitName.MenuManager = this.barManager1;
            this.txtGoodUnitName.Name = "txtGoodUnitName";
            this.txtGoodUnitName.Size = new System.Drawing.Size(247, 20);
            this.txtGoodUnitName.StyleController = this.layoutControl4;
            this.txtGoodUnitName.TabIndex = 13;
            this.txtGoodUnitName.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtGoodUnitName_PreviewKeyDown);
            // 
            // spinGoodPrice
            // 
            this.spinGoodPrice.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.spinGoodPrice.Location = new System.Drawing.Point(630, 26);
            this.spinGoodPrice.MenuManager = this.barManager1;
            this.spinGoodPrice.Name = "spinGoodPrice";
            this.spinGoodPrice.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.spinGoodPrice.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.spinGoodPrice.Properties.MaxLength = 15;
            this.spinGoodPrice.Properties.MaxValue = new decimal(new int[] {
            -1530494977,
            232830,
            0,
            0});
            this.spinGoodPrice.Size = new System.Drawing.Size(142, 20);
            this.spinGoodPrice.StyleController = this.layoutControl4;
            this.spinGoodPrice.TabIndex = 15;
            this.spinGoodPrice.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.spinGoodPrice_PreviewKeyDown);
            // 
            // spinGoodAmount
            // 
            this.spinGoodAmount.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.spinGoodAmount.Location = new System.Drawing.Point(443, 26);
            this.spinGoodAmount.MenuManager = this.barManager1;
            this.spinGoodAmount.Name = "spinGoodAmount";
            this.spinGoodAmount.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.spinGoodAmount.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.spinGoodAmount.Properties.MaxLength = 15;
            this.spinGoodAmount.Properties.MaxValue = new decimal(new int[] {
            -1530494977,
            232830,
            0,
            0});
            this.spinGoodAmount.Size = new System.Drawing.Size(88, 20);
            this.spinGoodAmount.StyleController = this.layoutControl4;
            this.spinGoodAmount.TabIndex = 14;
            this.spinGoodAmount.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.spinGoodAmount_PreviewKeyDown);
            // 
            // spinGoodDiscountRatio
            // 
            this.spinGoodDiscountRatio.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.spinGoodDiscountRatio.Location = new System.Drawing.Point(216, 50);
            this.spinGoodDiscountRatio.MenuManager = this.barManager1;
            this.spinGoodDiscountRatio.Name = "spinGoodDiscountRatio";
            this.spinGoodDiscountRatio.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.spinGoodDiscountRatio.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.spinGoodDiscountRatio.Properties.DisplayFormat.FormatString = "#,##0.0000";
            this.spinGoodDiscountRatio.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.spinGoodDiscountRatio.Properties.EditFormat.FormatString = "#,##0.0000";
            this.spinGoodDiscountRatio.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.spinGoodDiscountRatio.Properties.EditValueChangedFiringMode = DevExpress.XtraEditors.Controls.EditValueChangedFiringMode.Default;
            this.spinGoodDiscountRatio.Properties.MaxValue = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.spinGoodDiscountRatio.Size = new System.Drawing.Size(104, 20);
            this.spinGoodDiscountRatio.StyleController = this.layoutControl4;
            this.spinGoodDiscountRatio.TabIndex = 17;
            this.spinGoodDiscountRatio.EditValueChanged += new System.EventHandler(this.spinGoodDiscountRatio_EditValueChanged);
            this.spinGoodDiscountRatio.KeyUp += new System.Windows.Forms.KeyEventHandler(this.spinGoodDiscountRatio_KeyUp);
            this.spinGoodDiscountRatio.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.spinGoodDiscountRatio_PreviewKeyDown);
            // 
            // spinGoodDiscount
            // 
            this.spinGoodDiscount.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.spinGoodDiscount.Location = new System.Drawing.Point(97, 50);
            this.spinGoodDiscount.MenuManager = this.barManager1;
            this.spinGoodDiscount.Name = "spinGoodDiscount";
            this.spinGoodDiscount.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.spinGoodDiscount.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.spinGoodDiscount.Properties.DisplayFormat.FormatString = "#,##0.0000";
            this.spinGoodDiscount.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.spinGoodDiscount.Properties.EditFormat.FormatString = "#,##0";
            this.spinGoodDiscount.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.spinGoodDiscount.Size = new System.Drawing.Size(115, 20);
            this.spinGoodDiscount.StyleController = this.layoutControl4;
            this.spinGoodDiscount.TabIndex = 16;
            this.spinGoodDiscount.EditValueChanged += new System.EventHandler(this.spinGoodDiscount_EditValueChanged);
            this.spinGoodDiscount.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.spinGoodDiscount_PreviewKeyDown);
            // 
            // cboNoneMediService
            // 
            this.cboNoneMediService.Location = new System.Drawing.Point(97, 2);
            this.cboNoneMediService.MenuManager = this.barManager1;
            this.cboNoneMediService.Name = "cboNoneMediService";
            this.cboNoneMediService.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.cboNoneMediService.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Plus)});
            this.cboNoneMediService.Properties.NullText = "";
            this.cboNoneMediService.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
            this.cboNoneMediService.Properties.View = this.gridLookUpEdit1View;
            this.cboNoneMediService.Size = new System.Drawing.Size(675, 20);
            this.cboNoneMediService.StyleController = this.layoutControl4;
            this.cboNoneMediService.TabIndex = 22;
            this.cboNoneMediService.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboNoneMediService_Closed);
            this.cboNoneMediService.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboNoneMediService_ButtonClick);
            // 
            // gridLookUpEdit1View
            // 
            this.gridLookUpEdit1View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridLookUpEdit1View.Name = "gridLookUpEdit1View";
            this.gridLookUpEdit1View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridLookUpEdit1View.OptionsView.ShowGroupPanel = false;
            // 
            // layoutControlGroup3
            // 
            this.layoutControlGroup3.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.False;
            this.layoutControlGroup3.GroupBordersVisible = false;
            this.layoutControlGroup3.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lcGoodUnitName,
            this.lcGoodDiscount,
            this.lcGoodAmount,
            this.lcGoodPrice,
            this.layoutControlItem26,
            this.lcGoodDescription,
            this.layoutControlItem24,
            this.layoutControlItem25,
            this.layoutControlItem27,
            this.layoutControlItem4});
            this.layoutControlGroup3.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup3.Name = "layoutControlGroup3";
            this.layoutControlGroup3.Size = new System.Drawing.Size(870, 267);
            this.layoutControlGroup3.TextVisible = false;
            // 
            // lcGoodUnitName
            // 
            this.lcGoodUnitName.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lcGoodUnitName.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lcGoodUnitName.Control = this.txtGoodUnitName;
            this.lcGoodUnitName.Location = new System.Drawing.Point(0, 24);
            this.lcGoodUnitName.Name = "lcGoodUnitName";
            this.lcGoodUnitName.Size = new System.Drawing.Size(346, 24);
            this.lcGoodUnitName.Text = "Đơn vị tính:";
            this.lcGoodUnitName.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lcGoodUnitName.TextSize = new System.Drawing.Size(90, 20);
            this.lcGoodUnitName.TextToControlDistance = 5;
            // 
            // lcGoodDiscount
            // 
            this.lcGoodDiscount.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lcGoodDiscount.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lcGoodDiscount.Control = this.spinGoodDiscount;
            this.lcGoodDiscount.Location = new System.Drawing.Point(0, 48);
            this.lcGoodDiscount.Name = "lcGoodDiscount";
            this.lcGoodDiscount.Size = new System.Drawing.Size(214, 24);
            this.lcGoodDiscount.Text = "Chiết khấu:";
            this.lcGoodDiscount.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lcGoodDiscount.TextSize = new System.Drawing.Size(90, 20);
            this.lcGoodDiscount.TextToControlDistance = 5;
            // 
            // lcGoodAmount
            // 
            this.lcGoodAmount.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.lcGoodAmount.AppearanceItemCaption.Options.UseForeColor = true;
            this.lcGoodAmount.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lcGoodAmount.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lcGoodAmount.Control = this.spinGoodAmount;
            this.lcGoodAmount.Location = new System.Drawing.Point(346, 24);
            this.lcGoodAmount.Name = "lcGoodAmount";
            this.lcGoodAmount.Size = new System.Drawing.Size(187, 24);
            this.lcGoodAmount.Text = "Số lượng:";
            this.lcGoodAmount.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lcGoodAmount.TextSize = new System.Drawing.Size(90, 20);
            this.lcGoodAmount.TextToControlDistance = 5;
            // 
            // lcGoodPrice
            // 
            this.lcGoodPrice.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.lcGoodPrice.AppearanceItemCaption.Options.UseForeColor = true;
            this.lcGoodPrice.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lcGoodPrice.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lcGoodPrice.Control = this.spinGoodPrice;
            this.lcGoodPrice.Location = new System.Drawing.Point(533, 24);
            this.lcGoodPrice.Name = "lcGoodPrice";
            this.lcGoodPrice.Size = new System.Drawing.Size(241, 24);
            this.lcGoodPrice.Text = "Đơn giá:";
            this.lcGoodPrice.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lcGoodPrice.TextSize = new System.Drawing.Size(90, 20);
            this.lcGoodPrice.TextToControlDistance = 5;
            // 
            // layoutControlItem26
            // 
            this.layoutControlItem26.Control = this.btnAdd;
            this.layoutControlItem26.Location = new System.Drawing.Point(774, 0);
            this.layoutControlItem26.Name = "layoutControlItem26";
            this.layoutControlItem26.Size = new System.Drawing.Size(96, 72);
            this.layoutControlItem26.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem26.TextVisible = false;
            // 
            // lcGoodDescription
            // 
            this.lcGoodDescription.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lcGoodDescription.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lcGoodDescription.Control = this.txtGoodDescription;
            this.lcGoodDescription.Location = new System.Drawing.Point(346, 48);
            this.lcGoodDescription.Name = "lcGoodDescription";
            this.lcGoodDescription.Size = new System.Drawing.Size(428, 24);
            this.lcGoodDescription.Text = "Lý do:";
            this.lcGoodDescription.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lcGoodDescription.TextSize = new System.Drawing.Size(90, 20);
            this.lcGoodDescription.TextToControlDistance = 5;
            // 
            // layoutControlItem24
            // 
            this.layoutControlItem24.Control = this.label1;
            this.layoutControlItem24.Location = new System.Drawing.Point(322, 48);
            this.layoutControlItem24.Name = "layoutControlItem24";
            this.layoutControlItem24.Size = new System.Drawing.Size(24, 24);
            this.layoutControlItem24.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem24.TextVisible = false;
            // 
            // layoutControlItem25
            // 
            this.layoutControlItem25.Control = this.spinGoodDiscountRatio;
            this.layoutControlItem25.Location = new System.Drawing.Point(214, 48);
            this.layoutControlItem25.Name = "layoutControlItem25";
            this.layoutControlItem25.Size = new System.Drawing.Size(108, 24);
            this.layoutControlItem25.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem25.TextVisible = false;
            // 
            // layoutControlItem27
            // 
            this.layoutControlItem27.Control = this.layoutControl5;
            this.layoutControlItem27.Location = new System.Drawing.Point(0, 72);
            this.layoutControlItem27.Name = "layoutControlItem27";
            this.layoutControlItem27.Size = new System.Drawing.Size(870, 195);
            this.layoutControlItem27.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem27.TextVisible = false;
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.layoutControlItem4.AppearanceItemCaption.Options.UseForeColor = true;
            this.layoutControlItem4.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem4.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem4.Control = this.cboNoneMediService;
            this.layoutControlItem4.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Size = new System.Drawing.Size(774, 24);
            this.layoutControlItem4.Text = "Tên dịch vụ:";
            this.layoutControlItem4.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem4.TextSize = new System.Drawing.Size(90, 20);
            this.layoutControlItem4.TextToControlDistance = 5;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.layoutControl3);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(2, 2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(876, 131);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Thông tin người mua";
            // 
            // layoutControl3
            // 
            this.layoutControl3.Controls.Add(this.txtBuyerAddress);
            this.layoutControl3.Controls.Add(this.txtBuyerAccountNumber);
            this.layoutControl3.Controls.Add(this.txtBuyerTaxCode);
            this.layoutControl3.Controls.Add(this.btnSearch);
            this.layoutControl3.Controls.Add(this.chkCheckXD);
            this.layoutControl3.Controls.Add(this.txtDescription);
            this.layoutControl3.Controls.Add(this.txtBuyerOrganization);
            this.layoutControl3.Controls.Add(this.txtPatientName);
            this.layoutControl3.Controls.Add(this.txtTreatmentCodeSearch);
            this.layoutControl3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl3.Location = new System.Drawing.Point(3, 17);
            this.layoutControl3.Name = "layoutControl3";
            this.layoutControl3.Root = this.layoutControlGroup2;
            this.layoutControl3.Size = new System.Drawing.Size(870, 111);
            this.layoutControl3.TabIndex = 0;
            this.layoutControl3.Text = "layoutControl3";
            // 
            // txtBuyerAddress
            // 
            this.txtBuyerAddress.Location = new System.Drawing.Point(490, 52);
            this.txtBuyerAddress.MenuManager = this.barManager1;
            this.txtBuyerAddress.Name = "txtBuyerAddress";
            this.txtBuyerAddress.Size = new System.Drawing.Size(378, 20);
            this.txtBuyerAddress.StyleController = this.layoutControl3;
            this.txtBuyerAddress.TabIndex = 8;
            this.txtBuyerAddress.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtBuyerAddress_PreviewKeyDown_1);
            // 
            // txtBuyerAccountNumber
            // 
            this.txtBuyerAccountNumber.Location = new System.Drawing.Point(724, 28);
            this.txtBuyerAccountNumber.MenuManager = this.barManager1;
            this.txtBuyerAccountNumber.Name = "txtBuyerAccountNumber";
            this.txtBuyerAccountNumber.Size = new System.Drawing.Size(144, 20);
            this.txtBuyerAccountNumber.StyleController = this.layoutControl3;
            this.txtBuyerAccountNumber.TabIndex = 6;
            this.txtBuyerAccountNumber.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtBuyerAccountNumber_PreviewKeyDown);
            // 
            // txtBuyerTaxCode
            // 
            this.txtBuyerTaxCode.Location = new System.Drawing.Point(490, 28);
            this.txtBuyerTaxCode.MenuManager = this.barManager1;
            this.txtBuyerTaxCode.Name = "txtBuyerTaxCode";
            this.txtBuyerTaxCode.Size = new System.Drawing.Size(140, 20);
            this.txtBuyerTaxCode.StyleController = this.layoutControl3;
            this.txtBuyerTaxCode.TabIndex = 5;
            this.txtBuyerTaxCode.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtBuyerTaxCode_PreviewKeyDown);
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(292, 2);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(99, 22);
            this.btnSearch.StyleController = this.layoutControl3;
            this.btnSearch.TabIndex = 2;
            this.btnSearch.Text = "Tìm kiếm (Ctrl F)";
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // chkCheckXD
            // 
            this.chkCheckXD.Location = new System.Drawing.Point(490, 2);
            this.chkCheckXD.MenuManager = this.barManager1;
            this.chkCheckXD.Name = "chkCheckXD";
            this.chkCheckXD.Properties.Caption = "(Check nếu xuất hóa đơn cho đối tượng không phải bệnh nhân)";
            this.chkCheckXD.Properties.FullFocusRect = true;
            this.chkCheckXD.Properties.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
            this.chkCheckXD.Size = new System.Drawing.Size(378, 19);
            this.chkCheckXD.StyleController = this.layoutControl3;
            this.chkCheckXD.TabIndex = 3;
            this.chkCheckXD.CheckedChanged += new System.EventHandler(this.chkCheckXD_CheckedChanged);
            this.chkCheckXD.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.chkCheckXD_PreviewKeyDown);
            // 
            // txtDescription
            // 
            this.txtDescription.Location = new System.Drawing.Point(97, 76);
            this.txtDescription.MenuManager = this.barManager1;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(771, 20);
            this.txtDescription.StyleController = this.layoutControl3;
            this.txtDescription.TabIndex = 9;
            this.txtDescription.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtDescription_PreviewKeyDown);
            // 
            // txtBuyerOrganization
            // 
            this.txtBuyerOrganization.Location = new System.Drawing.Point(97, 52);
            this.txtBuyerOrganization.MenuManager = this.barManager1;
            this.txtBuyerOrganization.Name = "txtBuyerOrganization";
            this.txtBuyerOrganization.Size = new System.Drawing.Size(294, 20);
            this.txtBuyerOrganization.StyleController = this.layoutControl3;
            this.txtBuyerOrganization.TabIndex = 7;
            this.txtBuyerOrganization.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtBuyerOrganization_PreviewKeyDown);
            // 
            // txtPatientName
            // 
            this.txtPatientName.Location = new System.Drawing.Point(97, 28);
            this.txtPatientName.MenuManager = this.barManager1;
            this.txtPatientName.Name = "txtPatientName";
            this.txtPatientName.Properties.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtPatientName.Size = new System.Drawing.Size(294, 20);
            this.txtPatientName.StyleController = this.layoutControl3;
            this.txtPatientName.TabIndex = 4;
            this.txtPatientName.EditValueChanged += new System.EventHandler(this.txtPatientName_EditValueChanged);
            this.txtPatientName.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtPatientName_PreviewKeyDown);
            // 
            // txtTreatmentCodeSearch
            // 
            this.txtTreatmentCodeSearch.Location = new System.Drawing.Point(97, 2);
            this.txtTreatmentCodeSearch.MenuManager = this.barManager1;
            this.txtTreatmentCodeSearch.Name = "txtTreatmentCodeSearch";
            this.txtTreatmentCodeSearch.Properties.NullValuePrompt = "Mã điều trị (F2)";
            this.txtTreatmentCodeSearch.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtTreatmentCodeSearch.Properties.ShowNullValuePromptWhenFocused = true;
            this.txtTreatmentCodeSearch.Size = new System.Drawing.Size(191, 20);
            this.txtTreatmentCodeSearch.StyleController = this.layoutControl3;
            this.txtTreatmentCodeSearch.TabIndex = 1;
            this.txtTreatmentCodeSearch.EditValueChanged += new System.EventHandler(this.txtTreatmentCodeSearch_EditValueChanged);
            this.txtTreatmentCodeSearch.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtTreatmentCodeSearch_PreviewKeyDown);
            // 
            // layoutControlGroup2
            // 
            this.layoutControlGroup2.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.False;
            this.layoutControlGroup2.GroupBordersVisible = false;
            this.layoutControlGroup2.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.LcTreatmentCodeSearch,
            this.lcPatientName,
            this.lcBuyerOrganization,
            this.lcDescription,
            this.lcChk,
            this.layoutControlItem9,
            this.lcBuyerTaxCode,
            this.lcBuyerAccountNumber,
            this.lcPatientAddress});
            this.layoutControlGroup2.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup2.Name = "layoutControlGroup2";
            this.layoutControlGroup2.Size = new System.Drawing.Size(870, 111);
            this.layoutControlGroup2.TextVisible = false;
            // 
            // LcTreatmentCodeSearch
            // 
            this.LcTreatmentCodeSearch.AppearanceItemCaption.Options.UseTextOptions = true;
            this.LcTreatmentCodeSearch.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.LcTreatmentCodeSearch.Control = this.txtTreatmentCodeSearch;
            this.LcTreatmentCodeSearch.Location = new System.Drawing.Point(0, 0);
            this.LcTreatmentCodeSearch.Name = "LcTreatmentCodeSearch";
            this.LcTreatmentCodeSearch.Size = new System.Drawing.Size(290, 26);
            this.LcTreatmentCodeSearch.Text = "Bệnh nhân:";
            this.LcTreatmentCodeSearch.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.LcTreatmentCodeSearch.TextSize = new System.Drawing.Size(90, 20);
            this.LcTreatmentCodeSearch.TextToControlDistance = 5;
            // 
            // lcPatientName
            // 
            this.lcPatientName.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lcPatientName.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lcPatientName.Control = this.txtPatientName;
            this.lcPatientName.Location = new System.Drawing.Point(0, 26);
            this.lcPatientName.Name = "lcPatientName";
            this.lcPatientName.Size = new System.Drawing.Size(393, 24);
            this.lcPatientName.Text = "Họ tên:";
            this.lcPatientName.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lcPatientName.TextSize = new System.Drawing.Size(90, 20);
            this.lcPatientName.TextToControlDistance = 5;
            // 
            // lcBuyerOrganization
            // 
            this.lcBuyerOrganization.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lcBuyerOrganization.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lcBuyerOrganization.Control = this.txtBuyerOrganization;
            this.lcBuyerOrganization.Location = new System.Drawing.Point(0, 50);
            this.lcBuyerOrganization.Name = "lcBuyerOrganization";
            this.lcBuyerOrganization.Size = new System.Drawing.Size(393, 24);
            this.lcBuyerOrganization.Text = "Đơn vị:";
            this.lcBuyerOrganization.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lcBuyerOrganization.TextSize = new System.Drawing.Size(90, 20);
            this.lcBuyerOrganization.TextToControlDistance = 5;
            // 
            // lcDescription
            // 
            this.lcDescription.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lcDescription.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lcDescription.Control = this.txtDescription;
            this.lcDescription.Location = new System.Drawing.Point(0, 74);
            this.lcDescription.Name = "lcDescription";
            this.lcDescription.Size = new System.Drawing.Size(870, 37);
            this.lcDescription.Text = "Mô tả:";
            this.lcDescription.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lcDescription.TextSize = new System.Drawing.Size(90, 20);
            this.lcDescription.TextToControlDistance = 5;
            // 
            // lcChk
            // 
            this.lcChk.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lcChk.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lcChk.Control = this.chkCheckXD;
            this.lcChk.Location = new System.Drawing.Point(393, 0);
            this.lcChk.Name = "lcChk";
            this.lcChk.Size = new System.Drawing.Size(477, 26);
            this.lcChk.Text = "Khác:";
            this.lcChk.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lcChk.TextSize = new System.Drawing.Size(90, 20);
            this.lcChk.TextToControlDistance = 5;
            // 
            // layoutControlItem9
            // 
            this.layoutControlItem9.Control = this.btnSearch;
            this.layoutControlItem9.Location = new System.Drawing.Point(290, 0);
            this.layoutControlItem9.Name = "layoutControlItem9";
            this.layoutControlItem9.Size = new System.Drawing.Size(103, 26);
            this.layoutControlItem9.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem9.TextVisible = false;
            // 
            // lcBuyerTaxCode
            // 
            this.lcBuyerTaxCode.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lcBuyerTaxCode.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lcBuyerTaxCode.Control = this.txtBuyerTaxCode;
            this.lcBuyerTaxCode.Location = new System.Drawing.Point(393, 26);
            this.lcBuyerTaxCode.Name = "lcBuyerTaxCode";
            this.lcBuyerTaxCode.Size = new System.Drawing.Size(239, 24);
            this.lcBuyerTaxCode.Text = "Mã số thuế:";
            this.lcBuyerTaxCode.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lcBuyerTaxCode.TextSize = new System.Drawing.Size(90, 20);
            this.lcBuyerTaxCode.TextToControlDistance = 5;
            // 
            // lcBuyerAccountNumber
            // 
            this.lcBuyerAccountNumber.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lcBuyerAccountNumber.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lcBuyerAccountNumber.Control = this.txtBuyerAccountNumber;
            this.lcBuyerAccountNumber.Location = new System.Drawing.Point(632, 26);
            this.lcBuyerAccountNumber.Name = "lcBuyerAccountNumber";
            this.lcBuyerAccountNumber.Size = new System.Drawing.Size(238, 24);
            this.lcBuyerAccountNumber.Text = "Số tài khoản:";
            this.lcBuyerAccountNumber.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lcBuyerAccountNumber.TextSize = new System.Drawing.Size(85, 20);
            this.lcBuyerAccountNumber.TextToControlDistance = 5;
            // 
            // lcPatientAddress
            // 
            this.lcPatientAddress.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lcPatientAddress.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lcPatientAddress.Control = this.txtBuyerAddress;
            this.lcPatientAddress.Location = new System.Drawing.Point(393, 50);
            this.lcPatientAddress.Name = "lcPatientAddress";
            this.lcPatientAddress.Size = new System.Drawing.Size(477, 24);
            this.lcPatientAddress.Text = "Địa chỉ:";
            this.lcPatientAddress.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lcPatientAddress.TextSize = new System.Drawing.Size(90, 20);
            this.lcPatientAddress.TextToControlDistance = 5;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.False;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.layoutControlItem2,
            this.layoutControlItem3});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Size = new System.Drawing.Size(880, 532);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.groupBox1;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(880, 135);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.groupBox2;
            this.layoutControlItem2.Location = new System.Drawing.Point(0, 135);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(880, 291);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.layoutControl2;
            this.layoutControlItem3.Location = new System.Drawing.Point(0, 426);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(880, 106);
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextVisible = false;
            // 
            // frmTransactionBillOther
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(880, 561);
            this.Controls.Add(this.layoutControl1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Name = "frmTransactionBillOther";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Thanh toán khác";
            this.Load += new System.EventHandler(this.frmTransactionBillOther_Load);
            this.Controls.SetChildIndex(this.barDockControlTop, 0);
            this.Controls.SetChildIndex(this.barDockControlBottom, 0);
            this.Controls.SetChildIndex(this.barDockControlRight, 0);
            this.Controls.SetChildIndex(this.barDockControlLeft, 0);
            this.Controls.SetChildIndex(this.layoutControl1, 0);
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl2)).EndInit();
            this.layoutControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chkHideHddt.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtExemptionReason.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAccountBookCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinExemptionRation.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinExemption.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinNumOrder.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtTransactionTime.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtTransactionTime.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinTotalAmount.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboAccountBook.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboPayForm.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcAccountBookCode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcExemption)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcNumOrder)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem19)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcPayForm)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcTransactionTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcExemptionReason)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcTotalAmount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem32)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem33)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem34)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem35)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem36)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem12)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciHideHddt)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem4)).EndInit();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl4)).EndInit();
            this.layoutControl4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl5)).EndInit();
            this.layoutControl5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControlBillGoods)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewBillGoods)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemButtonDelete)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemTextGoodName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemTextGoodUnitName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSpinAmount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSpinPrice)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSpinDiscount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem28)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtGoodDescription.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtGoodUnitName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinGoodPrice.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinGoodAmount.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinGoodDiscountRatio.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinGoodDiscount.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboNoneMediService.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcGoodUnitName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcGoodDiscount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcGoodAmount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcGoodPrice)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem26)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcGoodDescription)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem24)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem25)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem27)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl3)).EndInit();
            this.layoutControl3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtBuyerAddress.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBuyerAccountNumber.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBuyerTaxCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkCheckXD.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDescription.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBuyerOrganization.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPatientName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTreatmentCodeSearch.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.LcTreatmentCodeSearch)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcPatientName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcBuyerOrganization)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcDescription)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcChk)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcBuyerTaxCode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcBuyerAccountNumber)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcPatientAddress)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProvider1;
        private DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProvider2;
        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.Bar bar1;
        private DevExpress.XtraBars.BarButtonItem barBtnSaveAndPrint;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraBars.BarButtonItem barBtnSave;
        private DevExpress.XtraBars.BarButtonItem barBtnPrint;
        private DevExpress.XtraBars.BarButtonItem barBtnNew;
        private DevExpress.XtraBars.BarButtonItem barBtnSearch;
        private DevExpress.XtraBars.BarButtonItem barBtnAdd;
        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private System.Windows.Forms.GroupBox groupBox1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControl layoutControl2;
        private DevExpress.XtraEditors.SimpleButton btnNew;
        private DevExpress.XtraEditors.SimpleButton btnPrint;
        private DevExpress.XtraEditors.SimpleButton btnSave;
        private DevExpress.XtraEditors.TextEdit txtExemptionReason;
        private DevExpress.XtraEditors.SimpleButton btnSaveAndPrint;
        private DevExpress.XtraEditors.TextEdit txtAccountBookCode;
        private DevExpress.XtraEditors.SpinEdit spinExemptionRation;
        private DevExpress.XtraEditors.SpinEdit spinExemption;
        private DevExpress.XtraEditors.SpinEdit spinNumOrder;
        private DevExpress.XtraEditors.DateEdit dtTransactionTime;
        private DevExpress.XtraEditors.SpinEdit spinTotalAmount;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraLayout.LayoutControlItem lcAccountBookCode;
        private DevExpress.XtraLayout.LayoutControlItem lcExemption;
        private DevExpress.XtraLayout.LayoutControlItem lcNumOrder;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem19;
        private DevExpress.XtraLayout.LayoutControlItem lcPayForm;
        private DevExpress.XtraLayout.LayoutControlItem lcTransactionTime;
        private DevExpress.XtraLayout.LayoutControlItem lcExemptionReason;
        private DevExpress.XtraLayout.LayoutControlItem lcTotalAmount;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem3;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem32;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem33;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem4;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem34;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem35;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem36;
        private System.Windows.Forms.GroupBox groupBox2;
        private DevExpress.XtraLayout.LayoutControl layoutControl4;
        private DevExpress.XtraLayout.LayoutControl layoutControl5;
        private Inventec.Desktop.CustomControl.MyGridControl gridControlBillGoods;
        private Inventec.Desktop.CustomControl.MyGridView gridViewBillGoods;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_BillGood_Stt;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_BillGood_Delete;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit repositoryItemButtonDelete;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_BillGood_GoodName;
        private DevExpress.XtraEditors.Repository.RepositoryItemTextEdit repositoryItemTextGoodName;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_BillGood_GoodUnitName;
        private DevExpress.XtraEditors.Repository.RepositoryItemTextEdit repositoryItemTextGoodUnitName;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_BillGood_Amount;
        private DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit repositoryItemSpinAmount;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_BillGood_Price;
        private DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit repositoryItemSpinPrice;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_BillGood_Discount;
        private DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit repositoryItemSpinDiscount;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_BillGood_TotalPrice;
        private DevExpress.XtraGrid.Columns.GridColumn colSTTUnb;
        private DevExpress.XtraGrid.Columns.GridColumn colDeleteUnb;
        private DevExpress.XtraGrid.Columns.GridColumn colGOOD_NAMEUnb;
        private DevExpress.XtraGrid.Columns.GridColumn colGOOD_UNIT_NAMEUnb;
        private DevExpress.XtraGrid.Columns.GridColumn colAMOUNTUnb;
        private DevExpress.XtraGrid.Columns.GridColumn colPRICEUnb;
        private DevExpress.XtraGrid.Columns.GridColumn colDISCOUNTUnb;
        private DevExpress.XtraGrid.Columns.GridColumn colTOTAL_PRICEUnb;
        private DevExpress.XtraGrid.Columns.GridColumn colGOODS_NAMEUnb;
        private DevExpress.XtraGrid.Columns.GridColumn colGOODS_UNIT_NAMEUnb;
        private DevExpress.XtraGrid.Columns.GridColumn colTOTAL_PRICE_WITH_DISCOUNTUnb;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup4;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem28;
        private System.Windows.Forms.Label label1;
        private DevExpress.XtraEditors.TextEdit txtGoodDescription;
        private System.Windows.Forms.Button btnAdd;
        private DevExpress.XtraEditors.TextEdit txtGoodUnitName;
        private DevExpress.XtraEditors.SpinEdit spinGoodPrice;
        private DevExpress.XtraEditors.SpinEdit spinGoodAmount;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup3;
        private DevExpress.XtraLayout.LayoutControlItem lcGoodUnitName;
        private DevExpress.XtraLayout.LayoutControlItem lcGoodDiscount;
        private DevExpress.XtraLayout.LayoutControlItem lcGoodAmount;
        private DevExpress.XtraLayout.LayoutControlItem lcGoodPrice;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem26;
        private DevExpress.XtraLayout.LayoutControlItem lcGoodDescription;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem24;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem25;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem27;
        private DevExpress.XtraLayout.LayoutControl layoutControl3;
        private DevExpress.XtraEditors.TextEdit txtBuyerAddress;
        private DevExpress.XtraEditors.TextEdit txtBuyerAccountNumber;
        private DevExpress.XtraEditors.TextEdit txtBuyerTaxCode;
        private DevExpress.XtraEditors.SimpleButton btnSearch;
        private DevExpress.XtraEditors.CheckEdit chkCheckXD;
        private DevExpress.XtraEditors.TextEdit txtDescription;
        private DevExpress.XtraEditors.TextEdit txtBuyerOrganization;
        private DevExpress.XtraEditors.TextEdit txtPatientName;
        private DevExpress.XtraEditors.TextEdit txtTreatmentCodeSearch;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup2;
        private DevExpress.XtraLayout.LayoutControlItem LcTreatmentCodeSearch;
        private DevExpress.XtraLayout.LayoutControlItem lcPatientName;
        private DevExpress.XtraLayout.LayoutControlItem lcBuyerOrganization;
        private DevExpress.XtraLayout.LayoutControlItem lcDescription;
        private DevExpress.XtraLayout.LayoutControlItem lcChk;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem9;
        private DevExpress.XtraLayout.LayoutControlItem lcBuyerTaxCode;
        private DevExpress.XtraLayout.LayoutControlItem lcBuyerAccountNumber;
        private DevExpress.XtraLayout.LayoutControlItem lcPatientAddress;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraEditors.SpinEdit spinGoodDiscountRatio;
        private DevExpress.XtraEditors.SpinEdit spinGoodDiscount;
        private DevExpress.XtraEditors.LookUpEdit cboAccountBook;
        private DevExpress.XtraEditors.LookUpEdit cboPayForm;
        private System.Windows.Forms.Label label2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem12;
        private DevExpress.XtraBars.BarButtonItem btnF2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraEditors.SimpleButton btnSaveAndSign;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
        private DevExpress.XtraBars.BarButtonItem barBtnSaveAndSign;
        private DevExpress.XtraEditors.CheckEdit chkHideHddt;
        private DevExpress.XtraLayout.LayoutControlItem lciHideHddt;
        private CustomGridLookUpEditWithFilterMultiColumn cboNoneMediService;
        private CustomGridViewWithFilterMultiColumn gridLookUpEdit1View;
    }
}
