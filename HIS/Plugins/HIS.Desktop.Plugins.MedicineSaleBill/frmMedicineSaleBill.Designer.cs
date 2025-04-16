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
namespace HIS.Desktop.Plugins.MedicineSaleBill
{
    partial class frmMedicineSaleBill : HIS.Desktop.Utility.FormBase
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
            this.lciDiscount = new DevExpress.XtraLayout.LayoutControl();
            this.btnQR = new DevExpress.XtraEditors.SimpleButton();
            this.chkHideHddt = new DevExpress.XtraEditors.CheckEdit();
            this.barManager1 = new DevExpress.XtraBars.BarManager();
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.barButtonItemSave = new DevExpress.XtraBars.BarButtonItem();
            this.barBtnSavePrint = new DevExpress.XtraBars.BarButtonItem();
            this.barBtnPrint = new DevExpress.XtraBars.BarButtonItem();
            this.barBtnFind = new DevExpress.XtraBars.BarButtonItem();
            this.barBtnFocus = new DevExpress.XtraBars.BarButtonItem();
            this.barBtnNew = new DevExpress.XtraBars.BarButtonItem();
            this.barBtnSaveSign = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.BtnSaveSign = new DevExpress.XtraEditors.SimpleButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.layoutControl3 = new DevExpress.XtraLayout.LayoutControl();
            this.txtDescription = new DevExpress.XtraEditors.TextEdit();
            this.cboAccountBook = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridLookUpEdit1View = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.spinNumOrder = new DevExpress.XtraEditors.SpinEdit();
            this.txtCashierRoomCode = new DevExpress.XtraEditors.TextEdit();
            this.cboCashierRoom = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.cboPayFrom = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridView2 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.dtTransactionTime = new DevExpress.XtraEditors.DateEdit();
            this.layoutControlGroup3 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem21 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem22 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem8 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem11 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem23 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem24 = new DevExpress.XtraLayout.LayoutControlItem();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.txtEmail = new DevExpress.XtraEditors.TextEdit();
            this.txtBuyerOgranization = new DevExpress.XtraEditors.TextEdit();
            this.txtAddress = new DevExpress.XtraEditors.TextEdit();
            this.txtBuyerPhone = new DevExpress.XtraEditors.TextEdit();
            this.txtBuyerAccountCode = new DevExpress.XtraEditors.TextEdit();
            this.txtBuyerTaxCode = new DevExpress.XtraEditors.TextEdit();
            this.txtName = new DevExpress.XtraEditors.TextEdit();
            this.layoutControlGroup2 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem15 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem17 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem18 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem20 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem16 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem19 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem25 = new DevExpress.XtraLayout.LayoutControlItem();
            this.checkOverTime = new DevExpress.XtraEditors.CheckEdit();
            this.ddBtnPrint = new DevExpress.XtraEditors.DropDownButton();
            this.txtTreatmentCode = new DevExpress.XtraEditors.TextEdit();
            this.btnNew = new DevExpress.XtraEditors.SimpleButton();
            this.btnSavePrint = new DevExpress.XtraEditors.SimpleButton();
            this.btnFind = new DevExpress.XtraEditors.SimpleButton();
            this.txtExpMestCode = new DevExpress.XtraEditors.TextEdit();
            this.lblDiscount = new DevExpress.XtraEditors.LabelControl();
            this.layoutControl2 = new DevExpress.XtraLayout.LayoutControl();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lblTotalPrice = new DevExpress.XtraEditors.LabelControl();
            this.gridControlExpMestDetail = new Inventec.Desktop.CustomControl.MyGridControl();
            this.gridViewExpMestDetail = new Inventec.Desktop.CustomControl.MyGridView();
            this.gridColumn_ExpMestDetail_MediMateTypeName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_ExpMestDetail_ServiceUnitName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_ExpMestDetail_ExpMestCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_ExpMestDetail_ExpAmount = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_ExpMestDetail_AdvisoryPrice = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_ExpMestDetail_AdvisoryTotalPrice = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_ExpMestDetail_NationalName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_ExpMestDetail_ManufacturerName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_ExpMestDetail_VatRatio = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_ExpMestDetail_Discount = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colSTTUnb = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colMEDI_MATE_TYPE_NAMEUnb = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colSERVICE_UNIT_NAMEUnb = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colEXP_AMOUNTUnb = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colADVISORY_PRICE_DISPLAYUnb = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colVAT_RATIO_STRUnb = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colDISCOUNT_STRUnb = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colADVISORY_TOTAL_PRICE_DISPLAYUnb = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colNATIONAL_NAMEUnb = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colMANUFACTURER_NAMEUnb = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colUnb = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colEXP_MEST_CODEUnb = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colCheckUnb = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.btnSave = new DevExpress.XtraEditors.SimpleButton();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem9 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutTotalPrice = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem10 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciExpMestCode = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem7 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem12 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem13 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciOverTime = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem14 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcibtnSaveAndSign = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciHideHddt = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.layoutbtnQRCe = new DevExpress.XtraLayout.LayoutControlItem();
            this.dxValidationProviderEditorInfo = new DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider();
            this.dxErrorProvider = new DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider();
            this.popupMenu1 = new DevExpress.XtraBars.PopupMenu();
            ((System.ComponentModel.ISupportInitialize)(this.lciDiscount)).BeginInit();
            this.lciDiscount.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chkHideHddt.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl3)).BeginInit();
            this.layoutControl3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtDescription.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboAccountBook.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinNumOrder.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCashierRoomCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboCashierRoom.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboPayFrom.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtTransactionTime.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtTransactionTime.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem21)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem22)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem11)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem23)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem24)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtEmail.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBuyerOgranization.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAddress.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBuyerPhone.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBuyerAccountCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBuyerTaxCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem15)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem17)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem18)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem20)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem16)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem19)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem25)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkOverTime.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTreatmentCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtExpMestCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlExpMestDetail)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewExpMestDetail)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutTotalPrice)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem10)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciExpMestCode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem12)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem13)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciOverTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem14)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcibtnSaveAndSign)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciHideHddt)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutbtnQRCe)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProviderEditorInfo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxErrorProvider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.popupMenu1)).BeginInit();
            this.SuspendLayout();
            // 
            // lciDiscount
            // 
            this.lciDiscount.Controls.Add(this.btnQR);
            this.lciDiscount.Controls.Add(this.chkHideHddt);
            this.lciDiscount.Controls.Add(this.BtnSaveSign);
            this.lciDiscount.Controls.Add(this.groupBox2);
            this.lciDiscount.Controls.Add(this.groupBox1);
            this.lciDiscount.Controls.Add(this.checkOverTime);
            this.lciDiscount.Controls.Add(this.ddBtnPrint);
            this.lciDiscount.Controls.Add(this.txtTreatmentCode);
            this.lciDiscount.Controls.Add(this.btnNew);
            this.lciDiscount.Controls.Add(this.btnSavePrint);
            this.lciDiscount.Controls.Add(this.btnFind);
            this.lciDiscount.Controls.Add(this.txtExpMestCode);
            this.lciDiscount.Controls.Add(this.lblDiscount);
            this.lciDiscount.Controls.Add(this.layoutControl2);
            this.lciDiscount.Controls.Add(this.lblTotalPrice);
            this.lciDiscount.Controls.Add(this.gridControlExpMestDetail);
            this.lciDiscount.Controls.Add(this.btnSave);
            this.lciDiscount.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lciDiscount.Location = new System.Drawing.Point(0, 38);
            this.lciDiscount.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.lciDiscount.Name = "lciDiscount";
            this.lciDiscount.Root = this.layoutControlGroup1;
            this.lciDiscount.Size = new System.Drawing.Size(1203, 707);
            this.lciDiscount.TabIndex = 0;
            this.lciDiscount.Text = "Chiết khấu chung:";
            // 
            // btnQR
            // 
            this.btnQR.Location = new System.Drawing.Point(472, 677);
            this.btnQR.Margin = new System.Windows.Forms.Padding(4);
            this.btnQR.Name = "btnQR";
            this.btnQR.Size = new System.Drawing.Size(57, 27);
            this.btnQR.StyleController = this.lciDiscount;
            this.btnQR.TabIndex = 38;
            this.btnQR.Text = "QR";
            this.btnQR.ToolTip = "Thanh toán bằng QR";
            this.btnQR.Click += new System.EventHandler(this.btnQR_Click);
            // 
            // chkHideHddt
            // 
            this.chkHideHddt.Location = new System.Drawing.Point(400, 677);
            this.chkHideHddt.Margin = new System.Windows.Forms.Padding(4);
            this.chkHideHddt.MenuManager = this.barManager1;
            this.chkHideHddt.Name = "chkHideHddt";
            this.chkHideHddt.Properties.Caption = "";
            this.chkHideHddt.Size = new System.Drawing.Size(66, 19);
            this.chkHideHddt.StyleController = this.lciDiscount;
            this.chkHideHddt.TabIndex = 37;
            this.chkHideHddt.CheckedChanged += new System.EventHandler(this.chkHideHddt_CheckedChanged);
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
            this.barButtonItemSave,
            this.barBtnSavePrint,
            this.barBtnPrint,
            this.barBtnFind,
            this.barBtnFocus,
            this.barBtnNew,
            this.barBtnSaveSign});
            this.barManager1.MaxItemId = 7;
            // 
            // bar1
            // 
            this.bar1.BarName = "Tools";
            this.bar1.DockCol = 0;
            this.bar1.DockRow = 0;
            this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.barButtonItemSave),
            new DevExpress.XtraBars.LinkPersistInfo(this.barBtnSavePrint),
            new DevExpress.XtraBars.LinkPersistInfo(this.barBtnPrint),
            new DevExpress.XtraBars.LinkPersistInfo(this.barBtnFind),
            new DevExpress.XtraBars.LinkPersistInfo(this.barBtnFocus),
            new DevExpress.XtraBars.LinkPersistInfo(this.barBtnNew),
            new DevExpress.XtraBars.LinkPersistInfo(this.barBtnSaveSign)});
            this.bar1.Text = "Tools";
            this.bar1.Visible = false;
            // 
            // barButtonItemSave
            // 
            this.barButtonItemSave.Caption = "Lưu (Ctrl S)";
            this.barButtonItemSave.Id = 0;
            this.barButtonItemSave.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S));
            this.barButtonItemSave.Name = "barButtonItemSave";
            this.barButtonItemSave.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItemSave_ItemClick);
            // 
            // barBtnSavePrint
            // 
            this.barBtnSavePrint.Caption = "Lưu in (Ctrl I)";
            this.barBtnSavePrint.Id = 1;
            this.barBtnSavePrint.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.I));
            this.barBtnSavePrint.Name = "barBtnSavePrint";
            this.barBtnSavePrint.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barBtnSavePrint_ItemClick);
            // 
            // barBtnPrint
            // 
            this.barBtnPrint.Caption = "In (Ctrl P)";
            this.barBtnPrint.Id = 2;
            this.barBtnPrint.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P));
            this.barBtnPrint.Name = "barBtnPrint";
            this.barBtnPrint.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barBtnPrint_ItemClick);
            // 
            // barBtnFind
            // 
            this.barBtnFind.Caption = "Tìm (Ctrl F)";
            this.barBtnFind.Id = 3;
            this.barBtnFind.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F));
            this.barBtnFind.Name = "barBtnFind";
            this.barBtnFind.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barBtnFind_ItemClick);
            // 
            // barBtnFocus
            // 
            this.barBtnFocus.Caption = "Focus (F2)";
            this.barBtnFocus.Id = 4;
            this.barBtnFocus.ItemShortcut = new DevExpress.XtraBars.BarShortcut(System.Windows.Forms.Keys.F2);
            this.barBtnFocus.Name = "barBtnFocus";
            this.barBtnFocus.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barBtnFocus_ItemClick);
            // 
            // barBtnNew
            // 
            this.barBtnNew.Caption = "Mới (Ctrl N)";
            this.barBtnNew.Id = 5;
            this.barBtnNew.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N));
            this.barBtnNew.Name = "barBtnNew";
            this.barBtnNew.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barBtnNew_ItemClick);
            // 
            // barBtnSaveSign
            // 
            this.barBtnSaveSign.Caption = "lưu ký (Ctrl A)";
            this.barBtnSaveSign.Id = 6;
            this.barBtnSaveSign.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A));
            this.barBtnSaveSign.Name = "barBtnSaveSign";
            this.barBtnSaveSign.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barBtnSaveSign_ItemClick);
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Size = new System.Drawing.Size(1203, 38);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 745);
            this.barDockControlBottom.Size = new System.Drawing.Size(1203, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 38);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 707);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(1203, 38);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 707);
            // 
            // BtnSaveSign
            // 
            this.BtnSaveSign.Location = new System.Drawing.Point(535, 677);
            this.BtnSaveSign.Margin = new System.Windows.Forms.Padding(4);
            this.BtnSaveSign.Name = "BtnSaveSign";
            this.BtnSaveSign.Size = new System.Drawing.Size(107, 27);
            this.BtnSaveSign.StyleController = this.lciDiscount;
            this.BtnSaveSign.TabIndex = 36;
            this.BtnSaveSign.Text = "Lưu ký (Ctrl A)";
            this.BtnSaveSign.Click += new System.EventHandler(this.BtnSaveSign_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.layoutControl3);
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(3, 528);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox2.Size = new System.Drawing.Size(1197, 143);
            this.groupBox2.TabIndex = 35;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Thông tin hóa đơn:";
            // 
            // layoutControl3
            // 
            this.layoutControl3.Controls.Add(this.txtDescription);
            this.layoutControl3.Controls.Add(this.cboAccountBook);
            this.layoutControl3.Controls.Add(this.spinNumOrder);
            this.layoutControl3.Controls.Add(this.txtCashierRoomCode);
            this.layoutControl3.Controls.Add(this.cboCashierRoom);
            this.layoutControl3.Controls.Add(this.cboPayFrom);
            this.layoutControl3.Controls.Add(this.dtTransactionTime);
            this.layoutControl3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl3.Location = new System.Drawing.Point(4, 23);
            this.layoutControl3.Margin = new System.Windows.Forms.Padding(4);
            this.layoutControl3.Name = "layoutControl3";
            this.layoutControl3.Root = this.layoutControlGroup3;
            this.layoutControl3.Size = new System.Drawing.Size(1189, 116);
            this.layoutControl3.TabIndex = 0;
            this.layoutControl3.Text = "layoutControl3";
            // 
            // txtDescription
            // 
            this.txtDescription.Location = new System.Drawing.Point(831, 38);
            this.txtDescription.Margin = new System.Windows.Forms.Padding(4);
            this.txtDescription.MenuManager = this.barManager1;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(356, 22);
            this.txtDescription.StyleController = this.layoutControl3;
            this.txtDescription.TabIndex = 17;
            // 
            // cboAccountBook
            // 
            this.cboAccountBook.Location = new System.Drawing.Point(97, 7);
            this.cboAccountBook.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cboAccountBook.Name = "cboAccountBook";
            this.cboAccountBook.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboAccountBook.Properties.NullText = "";
            this.cboAccountBook.Properties.View = this.gridLookUpEdit1View;
            this.cboAccountBook.Size = new System.Drawing.Size(304, 22);
            this.cboAccountBook.StyleController = this.layoutControl3;
            this.cboAccountBook.TabIndex = 10;
            this.cboAccountBook.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboAccountBook_Closed);
            this.cboAccountBook.EditValueChanged += new System.EventHandler(this.cboAccountBook_EditValueChanged);
            this.cboAccountBook.KeyUp += new System.Windows.Forms.KeyEventHandler(this.cboAccountBook_KeyUp);
            // 
            // gridLookUpEdit1View
            // 
            this.gridLookUpEdit1View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridLookUpEdit1View.Name = "gridLookUpEdit1View";
            this.gridLookUpEdit1View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridLookUpEdit1View.OptionsView.ShowGroupPanel = false;
            // 
            // spinNumOrder
            // 
            this.spinNumOrder.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.spinNumOrder.Location = new System.Drawing.Point(490, 7);
            this.spinNumOrder.Margin = new System.Windows.Forms.Padding(4);
            this.spinNumOrder.MenuManager = this.barManager1;
            this.spinNumOrder.Name = "spinNumOrder";
            this.spinNumOrder.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.spinNumOrder.Properties.DisplayFormat.FormatString = "#,##0.";
            this.spinNumOrder.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.spinNumOrder.Properties.MaxLength = 15;
            this.spinNumOrder.Properties.MaxValue = new decimal(new int[] {
            -1530494977,
            232830,
            0,
            0});
            this.spinNumOrder.Size = new System.Drawing.Size(252, 22);
            this.spinNumOrder.StyleController = this.layoutControl3;
            this.spinNumOrder.TabIndex = 11;
            // 
            // txtCashierRoomCode
            // 
            this.txtCashierRoomCode.Location = new System.Drawing.Point(831, 7);
            this.txtCashierRoomCode.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtCashierRoomCode.Name = "txtCashierRoomCode";
            this.txtCashierRoomCode.Size = new System.Drawing.Size(165, 22);
            this.txtCashierRoomCode.StyleController = this.layoutControl3;
            this.txtCashierRoomCode.TabIndex = 12;
            this.txtCashierRoomCode.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtCashierRoomCode_PreviewKeyDown);
            // 
            // cboCashierRoom
            // 
            this.cboCashierRoom.Location = new System.Drawing.Point(996, 7);
            this.cboCashierRoom.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cboCashierRoom.Name = "cboCashierRoom";
            this.cboCashierRoom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboCashierRoom.Properties.NullText = "";
            this.cboCashierRoom.Properties.View = this.gridView1;
            this.cboCashierRoom.Size = new System.Drawing.Size(191, 22);
            this.cboCashierRoom.StyleController = this.layoutControl3;
            this.cboCashierRoom.TabIndex = 13;
            this.cboCashierRoom.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboCashierRoom_Closed);
            this.cboCashierRoom.EditValueChanged += new System.EventHandler(this.cboCashierRoom_EditValueChanged);
            this.cboCashierRoom.KeyUp += new System.Windows.Forms.KeyEventHandler(this.cboCashierRoom_KeyUp);
            // 
            // gridView1
            // 
            this.gridView1.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridView1.Name = "gridView1";
            this.gridView1.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridView1.OptionsView.ShowGroupPanel = false;
            // 
            // cboPayFrom
            // 
            this.cboPayFrom.Location = new System.Drawing.Point(97, 38);
            this.cboPayFrom.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cboPayFrom.Name = "cboPayFrom";
            this.cboPayFrom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboPayFrom.Properties.NullText = "";
            this.cboPayFrom.Properties.View = this.gridView2;
            this.cboPayFrom.Size = new System.Drawing.Size(304, 22);
            this.cboPayFrom.StyleController = this.layoutControl3;
            this.cboPayFrom.TabIndex = 15;
            this.cboPayFrom.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboPayFrom_Closed);
            this.cboPayFrom.KeyUp += new System.Windows.Forms.KeyEventHandler(this.cboPayFrom_KeyUp);
            // 
            // gridView2
            // 
            this.gridView2.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridView2.Name = "gridView2";
            this.gridView2.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridView2.OptionsView.ShowGroupPanel = false;
            // 
            // dtTransactionTime
            // 
            this.dtTransactionTime.EditValue = null;
            this.dtTransactionTime.Location = new System.Drawing.Point(490, 38);
            this.dtTransactionTime.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
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
            this.dtTransactionTime.Size = new System.Drawing.Size(252, 22);
            this.dtTransactionTime.StyleController = this.layoutControl3;
            this.dtTransactionTime.TabIndex = 16;
            this.dtTransactionTime.KeyUp += new System.Windows.Forms.KeyEventHandler(this.dtTransactionTime_KeyUp);
            // 
            // layoutControlGroup3
            // 
            this.layoutControlGroup3.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup3.GroupBordersVisible = false;
            this.layoutControlGroup3.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem6,
            this.layoutControlItem21,
            this.layoutControlItem22,
            this.layoutControlItem8,
            this.layoutControlItem11,
            this.layoutControlItem23,
            this.layoutControlItem24});
            this.layoutControlGroup3.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup3.Name = "layoutControlGroup3";
            this.layoutControlGroup3.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup3.Size = new System.Drawing.Size(1189, 116);
            this.layoutControlGroup3.TextVisible = false;
            // 
            // layoutControlItem6
            // 
            this.layoutControlItem6.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.layoutControlItem6.AppearanceItemCaption.Options.UseForeColor = true;
            this.layoutControlItem6.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem6.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem6.Control = this.cboAccountBook;
            this.layoutControlItem6.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem6.Name = "layoutControlItem6";
            this.layoutControlItem6.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 7, 2);
            this.layoutControlItem6.Size = new System.Drawing.Size(403, 31);
            this.layoutControlItem6.Text = "Số thu chi:";
            this.layoutControlItem6.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem6.TextSize = new System.Drawing.Size(90, 20);
            this.layoutControlItem6.TextToControlDistance = 5;
            // 
            // layoutControlItem21
            // 
            this.layoutControlItem21.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem21.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem21.Control = this.spinNumOrder;
            this.layoutControlItem21.Location = new System.Drawing.Point(403, 0);
            this.layoutControlItem21.Name = "layoutControlItem21";
            this.layoutControlItem21.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 7, 2);
            this.layoutControlItem21.Size = new System.Drawing.Size(341, 31);
            this.layoutControlItem21.Text = "Số biên lai:";
            this.layoutControlItem21.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem21.TextSize = new System.Drawing.Size(80, 20);
            this.layoutControlItem21.TextToControlDistance = 5;
            // 
            // layoutControlItem22
            // 
            this.layoutControlItem22.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.layoutControlItem22.AppearanceItemCaption.Options.UseForeColor = true;
            this.layoutControlItem22.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem22.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem22.Control = this.txtCashierRoomCode;
            this.layoutControlItem22.Location = new System.Drawing.Point(744, 0);
            this.layoutControlItem22.Name = "layoutControlItem22";
            this.layoutControlItem22.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 0, 7, 2);
            this.layoutControlItem22.Size = new System.Drawing.Size(252, 31);
            this.layoutControlItem22.Text = "Phòng TN:";
            this.layoutControlItem22.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem22.TextSize = new System.Drawing.Size(80, 20);
            this.layoutControlItem22.TextToControlDistance = 5;
            // 
            // layoutControlItem8
            // 
            this.layoutControlItem8.Control = this.cboCashierRoom;
            this.layoutControlItem8.Location = new System.Drawing.Point(996, 0);
            this.layoutControlItem8.Name = "layoutControlItem8";
            this.layoutControlItem8.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 2, 7, 2);
            this.layoutControlItem8.Size = new System.Drawing.Size(193, 31);
            this.layoutControlItem8.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem8.TextVisible = false;
            // 
            // layoutControlItem11
            // 
            this.layoutControlItem11.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.layoutControlItem11.AppearanceItemCaption.Options.UseForeColor = true;
            this.layoutControlItem11.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem11.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem11.Control = this.cboPayFrom;
            this.layoutControlItem11.Location = new System.Drawing.Point(0, 31);
            this.layoutControlItem11.Name = "layoutControlItem11";
            this.layoutControlItem11.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 7, 2);
            this.layoutControlItem11.Size = new System.Drawing.Size(403, 85);
            this.layoutControlItem11.Text = "Hình thức:";
            this.layoutControlItem11.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem11.TextSize = new System.Drawing.Size(90, 20);
            this.layoutControlItem11.TextToControlDistance = 5;
            // 
            // layoutControlItem23
            // 
            this.layoutControlItem23.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.layoutControlItem23.AppearanceItemCaption.Options.UseForeColor = true;
            this.layoutControlItem23.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem23.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem23.Control = this.dtTransactionTime;
            this.layoutControlItem23.Location = new System.Drawing.Point(403, 31);
            this.layoutControlItem23.Name = "layoutControlItem23";
            this.layoutControlItem23.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 7, 2);
            this.layoutControlItem23.Size = new System.Drawing.Size(341, 85);
            this.layoutControlItem23.Text = "Thời gian GD:";
            this.layoutControlItem23.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem23.TextSize = new System.Drawing.Size(80, 20);
            this.layoutControlItem23.TextToControlDistance = 5;
            // 
            // layoutControlItem24
            // 
            this.layoutControlItem24.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem24.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem24.Control = this.txtDescription;
            this.layoutControlItem24.Location = new System.Drawing.Point(744, 31);
            this.layoutControlItem24.Name = "layoutControlItem24";
            this.layoutControlItem24.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 7, 2);
            this.layoutControlItem24.Size = new System.Drawing.Size(445, 85);
            this.layoutControlItem24.Text = "Mô tả:";
            this.layoutControlItem24.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem24.TextSize = new System.Drawing.Size(80, 20);
            this.layoutControlItem24.TextToControlDistance = 5;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.layoutControl1);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(3, 424);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(1197, 98);
            this.groupBox1.TabIndex = 34;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Thông tin người mua:";
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.txtEmail);
            this.layoutControl1.Controls.Add(this.txtBuyerOgranization);
            this.layoutControl1.Controls.Add(this.txtAddress);
            this.layoutControl1.Controls.Add(this.txtBuyerPhone);
            this.layoutControl1.Controls.Add(this.txtBuyerAccountCode);
            this.layoutControl1.Controls.Add(this.txtBuyerTaxCode);
            this.layoutControl1.Controls.Add(this.txtName);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(4, 23);
            this.layoutControl1.Margin = new System.Windows.Forms.Padding(4);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup2;
            this.layoutControl1.Size = new System.Drawing.Size(1189, 71);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // txtEmail
            // 
            this.txtEmail.Location = new System.Drawing.Point(1010, 34);
            this.txtEmail.MenuManager = this.barManager1;
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(176, 22);
            this.txtEmail.StyleController = this.layoutControl1;
            this.txtEmail.TabIndex = 11;
            // 
            // txtBuyerOgranization
            // 
            this.txtBuyerOgranization.Location = new System.Drawing.Point(97, 38);
            this.txtBuyerOgranization.Margin = new System.Windows.Forms.Padding(4);
            this.txtBuyerOgranization.MenuManager = this.barManager1;
            this.txtBuyerOgranization.Name = "txtBuyerOgranization";
            this.txtBuyerOgranization.Size = new System.Drawing.Size(192, 22);
            this.txtBuyerOgranization.StyleController = this.layoutControl1;
            this.txtBuyerOgranization.TabIndex = 10;
            this.txtBuyerOgranization.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtBuyerOgranization_KeyUp);
            // 
            // txtAddress
            // 
            this.txtAddress.Location = new System.Drawing.Point(378, 36);
            this.txtAddress.Margin = new System.Windows.Forms.Padding(4);
            this.txtAddress.MenuManager = this.barManager1;
            this.txtAddress.Name = "txtAddress";
            this.txtAddress.Size = new System.Drawing.Size(542, 22);
            this.txtAddress.StyleController = this.layoutControl1;
            this.txtAddress.TabIndex = 9;
            this.txtAddress.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtAddress_KeyUp);
            // 
            // txtBuyerPhone
            // 
            this.txtBuyerPhone.Location = new System.Drawing.Point(1009, 5);
            this.txtBuyerPhone.Margin = new System.Windows.Forms.Padding(4);
            this.txtBuyerPhone.MenuManager = this.barManager1;
            this.txtBuyerPhone.Name = "txtBuyerPhone";
            this.txtBuyerPhone.Size = new System.Drawing.Size(178, 22);
            this.txtBuyerPhone.StyleController = this.layoutControl1;
            this.txtBuyerPhone.TabIndex = 8;
            this.txtBuyerPhone.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtBuyerPhone_KeyUp);
            // 
            // txtBuyerAccountCode
            // 
            this.txtBuyerAccountCode.Location = new System.Drawing.Point(677, 7);
            this.txtBuyerAccountCode.Margin = new System.Windows.Forms.Padding(4);
            this.txtBuyerAccountCode.MenuManager = this.barManager1;
            this.txtBuyerAccountCode.Name = "txtBuyerAccountCode";
            this.txtBuyerAccountCode.Size = new System.Drawing.Size(243, 22);
            this.txtBuyerAccountCode.StyleController = this.layoutControl1;
            this.txtBuyerAccountCode.TabIndex = 7;
            this.txtBuyerAccountCode.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtBuyerAccountCode_KeyUp);
            // 
            // txtBuyerTaxCode
            // 
            this.txtBuyerTaxCode.Location = new System.Drawing.Point(378, 7);
            this.txtBuyerTaxCode.Margin = new System.Windows.Forms.Padding(4);
            this.txtBuyerTaxCode.MenuManager = this.barManager1;
            this.txtBuyerTaxCode.Name = "txtBuyerTaxCode";
            this.txtBuyerTaxCode.Size = new System.Drawing.Size(210, 22);
            this.txtBuyerTaxCode.StyleController = this.layoutControl1;
            this.txtBuyerTaxCode.TabIndex = 6;
            this.txtBuyerTaxCode.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtBuyerTaxCode_KeyUp);
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(97, 7);
            this.txtName.Margin = new System.Windows.Forms.Padding(4);
            this.txtName.MenuManager = this.barManager1;
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(192, 22);
            this.txtName.StyleController = this.layoutControl1;
            this.txtName.TabIndex = 4;
            this.txtName.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtName_KeyUp);
            // 
            // layoutControlGroup2
            // 
            this.layoutControlGroup2.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup2.GroupBordersVisible = false;
            this.layoutControlGroup2.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem15,
            this.layoutControlItem17,
            this.layoutControlItem18,
            this.layoutControlItem20,
            this.layoutControlItem16,
            this.layoutControlItem19,
            this.layoutControlItem25});
            this.layoutControlGroup2.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup2.Name = "layoutControlGroup2";
            this.layoutControlGroup2.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup2.Size = new System.Drawing.Size(1189, 71);
            this.layoutControlGroup2.TextVisible = false;
            // 
            // layoutControlItem15
            // 
            this.layoutControlItem15.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem15.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem15.Control = this.txtName;
            this.layoutControlItem15.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem15.Name = "layoutControlItem15";
            this.layoutControlItem15.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 7, 2);
            this.layoutControlItem15.Size = new System.Drawing.Size(291, 31);
            this.layoutControlItem15.Text = "Họ tên:";
            this.layoutControlItem15.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem15.TextSize = new System.Drawing.Size(90, 20);
            this.layoutControlItem15.TextToControlDistance = 5;
            // 
            // layoutControlItem17
            // 
            this.layoutControlItem17.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem17.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem17.Control = this.txtBuyerTaxCode;
            this.layoutControlItem17.Location = new System.Drawing.Point(291, 0);
            this.layoutControlItem17.Name = "layoutControlItem17";
            this.layoutControlItem17.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 7, 2);
            this.layoutControlItem17.Size = new System.Drawing.Size(299, 31);
            this.layoutControlItem17.Text = "Mã số thuế:";
            this.layoutControlItem17.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem17.TextSize = new System.Drawing.Size(80, 20);
            this.layoutControlItem17.TextToControlDistance = 5;
            // 
            // layoutControlItem18
            // 
            this.layoutControlItem18.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem18.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem18.Control = this.txtBuyerAccountCode;
            this.layoutControlItem18.Location = new System.Drawing.Point(590, 0);
            this.layoutControlItem18.Name = "layoutControlItem18";
            this.layoutControlItem18.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 7, 2);
            this.layoutControlItem18.Size = new System.Drawing.Size(332, 31);
            this.layoutControlItem18.Text = "Số tài khoản:";
            this.layoutControlItem18.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem18.TextSize = new System.Drawing.Size(80, 20);
            this.layoutControlItem18.TextToControlDistance = 5;
            // 
            // layoutControlItem20
            // 
            this.layoutControlItem20.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem20.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem20.Control = this.txtAddress;
            this.layoutControlItem20.Location = new System.Drawing.Point(291, 31);
            this.layoutControlItem20.Name = "layoutControlItem20";
            this.layoutControlItem20.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 5, 2);
            this.layoutControlItem20.Size = new System.Drawing.Size(631, 40);
            this.layoutControlItem20.Text = "Địa chỉ:";
            this.layoutControlItem20.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem20.TextSize = new System.Drawing.Size(80, 20);
            this.layoutControlItem20.TextToControlDistance = 5;
            // 
            // layoutControlItem16
            // 
            this.layoutControlItem16.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem16.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem16.Control = this.txtBuyerOgranization;
            this.layoutControlItem16.Location = new System.Drawing.Point(0, 31);
            this.layoutControlItem16.Name = "layoutControlItem16";
            this.layoutControlItem16.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 7, 2);
            this.layoutControlItem16.Size = new System.Drawing.Size(291, 40);
            this.layoutControlItem16.Text = "Đơn vị:";
            this.layoutControlItem16.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem16.TextSize = new System.Drawing.Size(90, 20);
            this.layoutControlItem16.TextToControlDistance = 5;
            // 
            // layoutControlItem19
            // 
            this.layoutControlItem19.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem19.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem19.Control = this.txtBuyerPhone;
            this.layoutControlItem19.Location = new System.Drawing.Point(922, 0);
            this.layoutControlItem19.Name = "layoutControlItem19";
            this.layoutControlItem19.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 5, 2);
            this.layoutControlItem19.Size = new System.Drawing.Size(267, 31);
            this.layoutControlItem19.Text = "Số điện thoại:";
            this.layoutControlItem19.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem19.TextSize = new System.Drawing.Size(80, 20);
            this.layoutControlItem19.TextToControlDistance = 5;
            // 
            // layoutControlItem25
            // 
            this.layoutControlItem25.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem25.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem25.Control = this.txtEmail;
            this.layoutControlItem25.Location = new System.Drawing.Point(922, 31);
            this.layoutControlItem25.Name = "layoutControlItem25";
            this.layoutControlItem25.Size = new System.Drawing.Size(267, 40);
            this.layoutControlItem25.Text = "Email:";
            this.layoutControlItem25.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem25.TextSize = new System.Drawing.Size(80, 20);
            this.layoutControlItem25.TextToControlDistance = 5;
            // 
            // checkOverTime
            // 
            this.checkOverTime.Location = new System.Drawing.Point(205, 677);
            this.checkOverTime.Margin = new System.Windows.Forms.Padding(4);
            this.checkOverTime.MenuManager = this.barManager1;
            this.checkOverTime.Name = "checkOverTime";
            this.checkOverTime.Properties.Caption = " ";
            this.checkOverTime.Size = new System.Drawing.Size(64, 20);
            this.checkOverTime.StyleController = this.lciDiscount;
            this.checkOverTime.TabIndex = 33;
            this.checkOverTime.CheckedChanged += new System.EventHandler(this.checkOverTime_CheckedChanged);
            // 
            // ddBtnPrint
            // 
            this.ddBtnPrint.Location = new System.Drawing.Point(927, 677);
            this.ddBtnPrint.Margin = new System.Windows.Forms.Padding(4);
            this.ddBtnPrint.MenuManager = this.barManager1;
            this.ddBtnPrint.Name = "ddBtnPrint";
            this.ddBtnPrint.Size = new System.Drawing.Size(133, 27);
            this.ddBtnPrint.StyleController = this.lciDiscount;
            this.ddBtnPrint.TabIndex = 32;
            this.ddBtnPrint.Text = "In (Ctrl P)";
            this.ddBtnPrint.Click += new System.EventHandler(this.ddBtnPrint_Click);
            // 
            // txtTreatmentCode
            // 
            this.txtTreatmentCode.Location = new System.Drawing.Point(3, 3);
            this.txtTreatmentCode.Margin = new System.Windows.Forms.Padding(4);
            this.txtTreatmentCode.MenuManager = this.barManager1;
            this.txtTreatmentCode.Name = "txtTreatmentCode";
            this.txtTreatmentCode.Properties.NullValuePrompt = "Mã điều trị (F2)";
            this.txtTreatmentCode.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtTreatmentCode.Size = new System.Drawing.Size(141, 22);
            this.txtTreatmentCode.StyleController = this.lciDiscount;
            this.txtTreatmentCode.TabIndex = 31;
            this.txtTreatmentCode.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtTreatmentCode_PreviewKeyDown);
            // 
            // btnNew
            // 
            this.btnNew.Location = new System.Drawing.Point(1066, 677);
            this.btnNew.Margin = new System.Windows.Forms.Padding(4);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(134, 27);
            this.btnNew.StyleController = this.lciDiscount;
            this.btnNew.TabIndex = 30;
            this.btnNew.Text = "Mới (Ctrl N)";
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // btnSavePrint
            // 
            this.btnSavePrint.Location = new System.Drawing.Point(787, 677);
            this.btnSavePrint.Margin = new System.Windows.Forms.Padding(4);
            this.btnSavePrint.Name = "btnSavePrint";
            this.btnSavePrint.Size = new System.Drawing.Size(134, 27);
            this.btnSavePrint.StyleController = this.lciDiscount;
            this.btnSavePrint.TabIndex = 29;
            this.btnSavePrint.Text = "Lưu in (Ctrl I)";
            this.btnSavePrint.Click += new System.EventHandler(this.btnSavePrint_Click);
            // 
            // btnFind
            // 
            this.btnFind.Location = new System.Drawing.Point(297, 3);
            this.btnFind.Margin = new System.Windows.Forms.Padding(4);
            this.btnFind.Name = "btnFind";
            this.btnFind.Size = new System.Drawing.Size(141, 27);
            this.btnFind.StyleController = this.lciDiscount;
            this.btnFind.TabIndex = 28;
            this.btnFind.Text = "Tìm (Ctrl F)";
            this.btnFind.Click += new System.EventHandler(this.btnFind_Click);
            // 
            // txtExpMestCode
            // 
            this.txtExpMestCode.Location = new System.Drawing.Point(150, 3);
            this.txtExpMestCode.Margin = new System.Windows.Forms.Padding(4);
            this.txtExpMestCode.MenuManager = this.barManager1;
            this.txtExpMestCode.Name = "txtExpMestCode";
            this.txtExpMestCode.Properties.NullValuePrompt = "Mã phiếu xuất";
            this.txtExpMestCode.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtExpMestCode.Properties.ShowNullValuePromptWhenFocused = true;
            this.txtExpMestCode.Size = new System.Drawing.Size(141, 22);
            this.txtExpMestCode.StyleController = this.lciDiscount;
            this.txtExpMestCode.TabIndex = 27;
            this.txtExpMestCode.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtExpMestCode_PreviewKeyDown);
            // 
            // lblDiscount
            // 
            this.lblDiscount.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblDiscount.Location = new System.Drawing.Point(367, 389);
            this.lblDiscount.Margin = new System.Windows.Forms.Padding(4);
            this.lblDiscount.Name = "lblDiscount";
            this.lblDiscount.Size = new System.Drawing.Size(134, 20);
            this.lblDiscount.StyleController = this.lciDiscount;
            this.lblDiscount.TabIndex = 26;
            // 
            // layoutControl2
            // 
            this.layoutControl2.Location = new System.Drawing.Point(3, 389);
            this.layoutControl2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.layoutControl2.Name = "layoutControl2";
            this.layoutControl2.Root = this.Root;
            this.layoutControl2.Size = new System.Drawing.Size(263, 29);
            this.layoutControl2.TabIndex = 25;
            this.layoutControl2.Text = "layoutControl2";
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Location = new System.Drawing.Point(0, 0);
            this.Root.Name = "Root";
            this.Root.OptionsItemText.TextToControlDistance = 4;
            this.Root.Size = new System.Drawing.Size(263, 29);
            this.Root.TextVisible = false;
            // 
            // lblTotalPrice
            // 
            this.lblTotalPrice.Appearance.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalPrice.Appearance.ForeColor = System.Drawing.Color.Blue;
            this.lblTotalPrice.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblTotalPrice.Location = new System.Drawing.Point(612, 389);
            this.lblTotalPrice.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.lblTotalPrice.Name = "lblTotalPrice";
            this.lblTotalPrice.Size = new System.Drawing.Size(588, 29);
            this.lblTotalPrice.StyleController = this.lciDiscount;
            this.lblTotalPrice.TabIndex = 24;
            // 
            // gridControlExpMestDetail
            // 
            this.gridControlExpMestDetail.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.gridControlExpMestDetail.Location = new System.Drawing.Point(3, 36);
            this.gridControlExpMestDetail.MainView = this.gridViewExpMestDetail;
            this.gridControlExpMestDetail.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.gridControlExpMestDetail.Name = "gridControlExpMestDetail";
            this.gridControlExpMestDetail.Size = new System.Drawing.Size(1197, 347);
            this.gridControlExpMestDetail.TabIndex = 19;
            this.gridControlExpMestDetail.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewExpMestDetail});
            // 
            // gridViewExpMestDetail
            // 
            this.gridViewExpMestDetail.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn_ExpMestDetail_MediMateTypeName,
            this.gridColumn_ExpMestDetail_ServiceUnitName,
            this.gridColumn_ExpMestDetail_ExpMestCode,
            this.gridColumn_ExpMestDetail_ExpAmount,
            this.gridColumn_ExpMestDetail_AdvisoryPrice,
            this.gridColumn_ExpMestDetail_AdvisoryTotalPrice,
            this.gridColumn_ExpMestDetail_NationalName,
            this.gridColumn_ExpMestDetail_ManufacturerName,
            this.gridColumn_ExpMestDetail_VatRatio,
            this.gridColumn_ExpMestDetail_Discount,
            this.colSTTUnb,
            this.colMEDI_MATE_TYPE_NAMEUnb,
            this.colSERVICE_UNIT_NAMEUnb,
            this.colEXP_AMOUNTUnb,
            this.colADVISORY_PRICE_DISPLAYUnb,
            this.colVAT_RATIO_STRUnb,
            this.colDISCOUNT_STRUnb,
            this.colADVISORY_TOTAL_PRICE_DISPLAYUnb,
            this.colNATIONAL_NAMEUnb,
            this.colMANUFACTURER_NAMEUnb,
            this.colUnb,
            this.colEXP_MEST_CODEUnb,
            this.colCheckUnb,
            this.gridColumn1});
            this.gridViewExpMestDetail.GridControl = this.gridControlExpMestDetail;
            this.gridViewExpMestDetail.GroupCount = 1;
            this.gridViewExpMestDetail.Name = "gridViewExpMestDetail";
            this.gridViewExpMestDetail.OptionsBehavior.AutoExpandAllGroups = true;
            this.gridViewExpMestDetail.OptionsSelection.CheckBoxSelectorColumnWidth = 25;
            this.gridViewExpMestDetail.OptionsSelection.MultiSelect = true;
            this.gridViewExpMestDetail.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;
            this.gridViewExpMestDetail.OptionsSelection.ShowCheckBoxSelectorInGroupRow = DevExpress.Utils.DefaultBoolean.True;
            this.gridViewExpMestDetail.OptionsView.ColumnAutoWidth = false;
            this.gridViewExpMestDetail.OptionsView.GroupDrawMode = DevExpress.XtraGrid.Views.Grid.GroupDrawMode.Office;
            this.gridViewExpMestDetail.OptionsView.HeaderFilterButtonShowMode = DevExpress.XtraEditors.Controls.FilterButtonShowMode.SmartTag;
            this.gridViewExpMestDetail.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            this.gridViewExpMestDetail.OptionsView.ShowGroupPanel = false;
            this.gridViewExpMestDetail.OptionsView.ShowIndicator = false;
            this.gridViewExpMestDetail.SortInfo.AddRange(new DevExpress.XtraGrid.Columns.GridColumnSortInfo[] {
            new DevExpress.XtraGrid.Columns.GridColumnSortInfo(this.gridColumn_ExpMestDetail_ExpMestCode, DevExpress.Data.ColumnSortOrder.Ascending)});
            this.gridViewExpMestDetail.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gridViewExpMestDetail_RowCellStyle);
            this.gridViewExpMestDetail.SelectionChanged += new DevExpress.Data.SelectionChangedEventHandler(this.gridViewExpMestDetail_SelectionChanged);
            this.gridViewExpMestDetail.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewExpMestDetail_CustomUnboundColumnData);
            this.gridViewExpMestDetail.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gridViewExpMestDetail_MouseDown);
            // 
            // gridColumn_ExpMestDetail_MediMateTypeName
            // 
            this.gridColumn_ExpMestDetail_MediMateTypeName.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_ExpMestDetail_MediMateTypeName.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gridColumn_ExpMestDetail_MediMateTypeName.Caption = "Tên";
            this.gridColumn_ExpMestDetail_MediMateTypeName.FieldName = "MEDI_MATE_TYPE_NAME";
            this.gridColumn_ExpMestDetail_MediMateTypeName.FieldNameSortGroup = "MEDI_MATE_TYPE_NAMEUnb";
            this.gridColumn_ExpMestDetail_MediMateTypeName.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gridColumn_ExpMestDetail_MediMateTypeName.Name = "gridColumn_ExpMestDetail_MediMateTypeName";
            this.gridColumn_ExpMestDetail_MediMateTypeName.OptionsColumn.AllowEdit = false;
            this.gridColumn_ExpMestDetail_MediMateTypeName.OptionsFilter.FilterBySortField = DevExpress.Utils.DefaultBoolean.True;
            this.gridColumn_ExpMestDetail_MediMateTypeName.Visible = true;
            this.gridColumn_ExpMestDetail_MediMateTypeName.VisibleIndex = 1;
            this.gridColumn_ExpMestDetail_MediMateTypeName.Width = 150;
            // 
            // gridColumn_ExpMestDetail_ServiceUnitName
            // 
            this.gridColumn_ExpMestDetail_ServiceUnitName.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_ExpMestDetail_ServiceUnitName.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_ExpMestDetail_ServiceUnitName.Caption = "ĐVT";
            this.gridColumn_ExpMestDetail_ServiceUnitName.FieldName = "SERVICE_UNIT_NAME";
            this.gridColumn_ExpMestDetail_ServiceUnitName.FieldNameSortGroup = "SERVICE_UNIT_NAMEUnb";
            this.gridColumn_ExpMestDetail_ServiceUnitName.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gridColumn_ExpMestDetail_ServiceUnitName.Name = "gridColumn_ExpMestDetail_ServiceUnitName";
            this.gridColumn_ExpMestDetail_ServiceUnitName.OptionsColumn.AllowEdit = false;
            this.gridColumn_ExpMestDetail_ServiceUnitName.OptionsFilter.FilterBySortField = DevExpress.Utils.DefaultBoolean.True;
            this.gridColumn_ExpMestDetail_ServiceUnitName.Visible = true;
            this.gridColumn_ExpMestDetail_ServiceUnitName.VisibleIndex = 2;
            // 
            // gridColumn_ExpMestDetail_ExpMestCode
            // 
            this.gridColumn_ExpMestDetail_ExpMestCode.Caption = "Mã y lệnh (mã phiếu xuất)";
            this.gridColumn_ExpMestDetail_ExpMestCode.FieldName = "EXP_MEST_CODE";
            this.gridColumn_ExpMestDetail_ExpMestCode.FieldNameSortGroup = "EXP_MEST_CODEUnb";
            this.gridColumn_ExpMestDetail_ExpMestCode.Name = "gridColumn_ExpMestDetail_ExpMestCode";
            this.gridColumn_ExpMestDetail_ExpMestCode.OptionsColumn.AllowEdit = false;
            this.gridColumn_ExpMestDetail_ExpMestCode.OptionsFilter.FilterBySortField = DevExpress.Utils.DefaultBoolean.True;
            this.gridColumn_ExpMestDetail_ExpMestCode.Visible = true;
            this.gridColumn_ExpMestDetail_ExpMestCode.VisibleIndex = 2;
            // 
            // gridColumn_ExpMestDetail_ExpAmount
            // 
            this.gridColumn_ExpMestDetail_ExpAmount.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn_ExpMestDetail_ExpAmount.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gridColumn_ExpMestDetail_ExpAmount.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_ExpMestDetail_ExpAmount.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_ExpMestDetail_ExpAmount.Caption = "Số lượng";
            this.gridColumn_ExpMestDetail_ExpAmount.DisplayFormat.FormatString = "#,##0.00";
            this.gridColumn_ExpMestDetail_ExpAmount.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.gridColumn_ExpMestDetail_ExpAmount.FieldName = "EXP_AMOUNT";
            this.gridColumn_ExpMestDetail_ExpAmount.FieldNameSortGroup = "EXP_AMOUNTUnb";
            this.gridColumn_ExpMestDetail_ExpAmount.Name = "gridColumn_ExpMestDetail_ExpAmount";
            this.gridColumn_ExpMestDetail_ExpAmount.OptionsColumn.AllowEdit = false;
            this.gridColumn_ExpMestDetail_ExpAmount.OptionsFilter.FilterBySortField = DevExpress.Utils.DefaultBoolean.True;
            this.gridColumn_ExpMestDetail_ExpAmount.Visible = true;
            this.gridColumn_ExpMestDetail_ExpAmount.VisibleIndex = 3;
            this.gridColumn_ExpMestDetail_ExpAmount.Width = 80;
            // 
            // gridColumn_ExpMestDetail_AdvisoryPrice
            // 
            this.gridColumn_ExpMestDetail_AdvisoryPrice.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn_ExpMestDetail_AdvisoryPrice.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gridColumn_ExpMestDetail_AdvisoryPrice.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_ExpMestDetail_AdvisoryPrice.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_ExpMestDetail_AdvisoryPrice.Caption = "Đơn giá";
            this.gridColumn_ExpMestDetail_AdvisoryPrice.FieldName = "ADVISORY_PRICE_DISPLAY";
            this.gridColumn_ExpMestDetail_AdvisoryPrice.FieldNameSortGroup = "ADVISORY_PRICE_DISPLAYUnb";
            this.gridColumn_ExpMestDetail_AdvisoryPrice.Name = "gridColumn_ExpMestDetail_AdvisoryPrice";
            this.gridColumn_ExpMestDetail_AdvisoryPrice.OptionsColumn.AllowEdit = false;
            this.gridColumn_ExpMestDetail_AdvisoryPrice.OptionsFilter.FilterBySortField = DevExpress.Utils.DefaultBoolean.True;
            this.gridColumn_ExpMestDetail_AdvisoryPrice.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn_ExpMestDetail_AdvisoryPrice.Visible = true;
            this.gridColumn_ExpMestDetail_AdvisoryPrice.VisibleIndex = 4;
            this.gridColumn_ExpMestDetail_AdvisoryPrice.Width = 80;
            // 
            // gridColumn_ExpMestDetail_AdvisoryTotalPrice
            // 
            this.gridColumn_ExpMestDetail_AdvisoryTotalPrice.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn_ExpMestDetail_AdvisoryTotalPrice.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gridColumn_ExpMestDetail_AdvisoryTotalPrice.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_ExpMestDetail_AdvisoryTotalPrice.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_ExpMestDetail_AdvisoryTotalPrice.Caption = "Thành tiền";
            this.gridColumn_ExpMestDetail_AdvisoryTotalPrice.FieldName = "ADVISORY_TOTAL_PRICE_DISPLAY";
            this.gridColumn_ExpMestDetail_AdvisoryTotalPrice.FieldNameSortGroup = "ADVISORY_TOTAL_PRICE_DISPLAYUnb";
            this.gridColumn_ExpMestDetail_AdvisoryTotalPrice.Name = "gridColumn_ExpMestDetail_AdvisoryTotalPrice";
            this.gridColumn_ExpMestDetail_AdvisoryTotalPrice.OptionsColumn.AllowEdit = false;
            this.gridColumn_ExpMestDetail_AdvisoryTotalPrice.OptionsFilter.FilterBySortField = DevExpress.Utils.DefaultBoolean.True;
            this.gridColumn_ExpMestDetail_AdvisoryTotalPrice.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn_ExpMestDetail_AdvisoryTotalPrice.Visible = true;
            this.gridColumn_ExpMestDetail_AdvisoryTotalPrice.VisibleIndex = 7;
            this.gridColumn_ExpMestDetail_AdvisoryTotalPrice.Width = 90;
            // 
            // gridColumn_ExpMestDetail_NationalName
            // 
            this.gridColumn_ExpMestDetail_NationalName.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_ExpMestDetail_NationalName.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_ExpMestDetail_NationalName.Caption = "Nước sản xuất";
            this.gridColumn_ExpMestDetail_NationalName.FieldName = "NATIONAL_NAME";
            this.gridColumn_ExpMestDetail_NationalName.FieldNameSortGroup = "NATIONAL_NAMEUnb";
            this.gridColumn_ExpMestDetail_NationalName.Name = "gridColumn_ExpMestDetail_NationalName";
            this.gridColumn_ExpMestDetail_NationalName.OptionsColumn.AllowEdit = false;
            this.gridColumn_ExpMestDetail_NationalName.OptionsFilter.FilterBySortField = DevExpress.Utils.DefaultBoolean.True;
            this.gridColumn_ExpMestDetail_NationalName.Visible = true;
            this.gridColumn_ExpMestDetail_NationalName.VisibleIndex = 8;
            this.gridColumn_ExpMestDetail_NationalName.Width = 100;
            // 
            // gridColumn_ExpMestDetail_ManufacturerName
            // 
            this.gridColumn_ExpMestDetail_ManufacturerName.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_ExpMestDetail_ManufacturerName.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_ExpMestDetail_ManufacturerName.Caption = "Hãng sản xuất";
            this.gridColumn_ExpMestDetail_ManufacturerName.FieldName = "MANUFACTURER_NAME";
            this.gridColumn_ExpMestDetail_ManufacturerName.FieldNameSortGroup = "MANUFACTURER_NAMEUnb";
            this.gridColumn_ExpMestDetail_ManufacturerName.Name = "gridColumn_ExpMestDetail_ManufacturerName";
            this.gridColumn_ExpMestDetail_ManufacturerName.OptionsColumn.AllowEdit = false;
            this.gridColumn_ExpMestDetail_ManufacturerName.OptionsFilter.FilterBySortField = DevExpress.Utils.DefaultBoolean.True;
            this.gridColumn_ExpMestDetail_ManufacturerName.Visible = true;
            this.gridColumn_ExpMestDetail_ManufacturerName.VisibleIndex = 9;
            this.gridColumn_ExpMestDetail_ManufacturerName.Width = 120;
            // 
            // gridColumn_ExpMestDetail_VatRatio
            // 
            this.gridColumn_ExpMestDetail_VatRatio.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn_ExpMestDetail_VatRatio.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gridColumn_ExpMestDetail_VatRatio.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_ExpMestDetail_VatRatio.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_ExpMestDetail_VatRatio.Caption = "Vat (%)";
            this.gridColumn_ExpMestDetail_VatRatio.DisplayFormat.FormatString = "#,##0.00";
            this.gridColumn_ExpMestDetail_VatRatio.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.gridColumn_ExpMestDetail_VatRatio.FieldName = "VAT_RATIO_STR";
            this.gridColumn_ExpMestDetail_VatRatio.FieldNameSortGroup = "VAT_RATIO_STRUnb";
            this.gridColumn_ExpMestDetail_VatRatio.Name = "gridColumn_ExpMestDetail_VatRatio";
            this.gridColumn_ExpMestDetail_VatRatio.OptionsColumn.AllowEdit = false;
            this.gridColumn_ExpMestDetail_VatRatio.OptionsFilter.FilterBySortField = DevExpress.Utils.DefaultBoolean.True;
            this.gridColumn_ExpMestDetail_VatRatio.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn_ExpMestDetail_VatRatio.Visible = true;
            this.gridColumn_ExpMestDetail_VatRatio.VisibleIndex = 5;
            this.gridColumn_ExpMestDetail_VatRatio.Width = 60;
            // 
            // gridColumn_ExpMestDetail_Discount
            // 
            this.gridColumn_ExpMestDetail_Discount.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn_ExpMestDetail_Discount.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gridColumn_ExpMestDetail_Discount.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_ExpMestDetail_Discount.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_ExpMestDetail_Discount.Caption = "Chiết khấu";
            this.gridColumn_ExpMestDetail_Discount.FieldName = "DISCOUNT_STR";
            this.gridColumn_ExpMestDetail_Discount.FieldNameSortGroup = "DISCOUNT_STRUnb";
            this.gridColumn_ExpMestDetail_Discount.Name = "gridColumn_ExpMestDetail_Discount";
            this.gridColumn_ExpMestDetail_Discount.OptionsColumn.AllowEdit = false;
            this.gridColumn_ExpMestDetail_Discount.OptionsFilter.FilterBySortField = DevExpress.Utils.DefaultBoolean.True;
            this.gridColumn_ExpMestDetail_Discount.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn_ExpMestDetail_Discount.Visible = true;
            this.gridColumn_ExpMestDetail_Discount.VisibleIndex = 6;
            this.gridColumn_ExpMestDetail_Discount.Width = 90;
            // 
            // colSTTUnb
            // 
            this.colSTTUnb.FieldName = "STTUnb";
            this.colSTTUnb.Name = "colSTTUnb";
            this.colSTTUnb.UnboundType = DevExpress.Data.UnboundColumnType.String;
            // 
            // colMEDI_MATE_TYPE_NAMEUnb
            // 
            this.colMEDI_MATE_TYPE_NAMEUnb.FieldName = "MEDI_MATE_TYPE_NAMEUnb";
            this.colMEDI_MATE_TYPE_NAMEUnb.Name = "colMEDI_MATE_TYPE_NAMEUnb";
            this.colMEDI_MATE_TYPE_NAMEUnb.UnboundType = DevExpress.Data.UnboundColumnType.String;
            // 
            // colSERVICE_UNIT_NAMEUnb
            // 
            this.colSERVICE_UNIT_NAMEUnb.FieldName = "SERVICE_UNIT_NAMEUnb";
            this.colSERVICE_UNIT_NAMEUnb.Name = "colSERVICE_UNIT_NAMEUnb";
            this.colSERVICE_UNIT_NAMEUnb.UnboundType = DevExpress.Data.UnboundColumnType.String;
            // 
            // colEXP_AMOUNTUnb
            // 
            this.colEXP_AMOUNTUnb.FieldName = "EXP_AMOUNTUnb";
            this.colEXP_AMOUNTUnb.Name = "colEXP_AMOUNTUnb";
            this.colEXP_AMOUNTUnb.UnboundType = DevExpress.Data.UnboundColumnType.String;
            // 
            // colADVISORY_PRICE_DISPLAYUnb
            // 
            this.colADVISORY_PRICE_DISPLAYUnb.FieldName = "ADVISORY_PRICE_DISPLAYUnb";
            this.colADVISORY_PRICE_DISPLAYUnb.Name = "colADVISORY_PRICE_DISPLAYUnb";
            this.colADVISORY_PRICE_DISPLAYUnb.UnboundType = DevExpress.Data.UnboundColumnType.String;
            // 
            // colVAT_RATIO_STRUnb
            // 
            this.colVAT_RATIO_STRUnb.FieldName = "VAT_RATIO_STRUnb";
            this.colVAT_RATIO_STRUnb.Name = "colVAT_RATIO_STRUnb";
            this.colVAT_RATIO_STRUnb.UnboundType = DevExpress.Data.UnboundColumnType.String;
            // 
            // colDISCOUNT_STRUnb
            // 
            this.colDISCOUNT_STRUnb.FieldName = "DISCOUNT_STRUnb";
            this.colDISCOUNT_STRUnb.Name = "colDISCOUNT_STRUnb";
            this.colDISCOUNT_STRUnb.UnboundType = DevExpress.Data.UnboundColumnType.String;
            // 
            // colADVISORY_TOTAL_PRICE_DISPLAYUnb
            // 
            this.colADVISORY_TOTAL_PRICE_DISPLAYUnb.FieldName = "ADVISORY_TOTAL_PRICE_DISPLAYUnb";
            this.colADVISORY_TOTAL_PRICE_DISPLAYUnb.Name = "colADVISORY_TOTAL_PRICE_DISPLAYUnb";
            this.colADVISORY_TOTAL_PRICE_DISPLAYUnb.UnboundType = DevExpress.Data.UnboundColumnType.String;
            // 
            // colNATIONAL_NAMEUnb
            // 
            this.colNATIONAL_NAMEUnb.FieldName = "NATIONAL_NAMEUnb";
            this.colNATIONAL_NAMEUnb.Name = "colNATIONAL_NAMEUnb";
            this.colNATIONAL_NAMEUnb.UnboundType = DevExpress.Data.UnboundColumnType.String;
            // 
            // colMANUFACTURER_NAMEUnb
            // 
            this.colMANUFACTURER_NAMEUnb.FieldName = "MANUFACTURER_NAMEUnb";
            this.colMANUFACTURER_NAMEUnb.Name = "colMANUFACTURER_NAMEUnb";
            this.colMANUFACTURER_NAMEUnb.UnboundType = DevExpress.Data.UnboundColumnType.String;
            // 
            // colUnb
            // 
            this.colUnb.FieldName = "Unb";
            this.colUnb.Name = "colUnb";
            this.colUnb.UnboundType = DevExpress.Data.UnboundColumnType.String;
            // 
            // colEXP_MEST_CODEUnb
            // 
            this.colEXP_MEST_CODEUnb.FieldName = "EXP_MEST_CODEUnb";
            this.colEXP_MEST_CODEUnb.Name = "colEXP_MEST_CODEUnb";
            this.colEXP_MEST_CODEUnb.UnboundType = DevExpress.Data.UnboundColumnType.String;
            // 
            // colCheckUnb
            // 
            this.colCheckUnb.FieldName = "CheckUnb";
            this.colCheckUnb.Name = "colCheckUnb";
            this.colCheckUnb.UnboundType = DevExpress.Data.UnboundColumnType.String;
            // 
            // gridColumn1
            // 
            this.gridColumn1.FieldName = "DX$CheckboxSelectorColumnUnb";
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.UnboundType = DevExpress.Data.UnboundColumnType.String;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(648, 677);
            this.btnSave.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(133, 27);
            this.btnSave.StyleController = this.lciDiscount;
            this.btnSave.TabIndex = 18;
            this.btnSave.Text = "Lưu (Ctrl S)";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem9,
            this.layoutTotalPrice,
            this.layoutControlItem2,
            this.layoutControlItem3,
            this.layoutControlItem10,
            this.lciExpMestCode,
            this.layoutControlItem5,
            this.emptySpaceItem2,
            this.layoutControlItem4,
            this.layoutControlItem7,
            this.layoutControlItem12,
            this.layoutControlItem13,
            this.lciOverTime,
            this.layoutControlItem1,
            this.layoutControlItem14,
            this.lcibtnSaveAndSign,
            this.lciHideHddt,
            this.emptySpaceItem1,
            this.layoutbtnQRCe});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.OptionsItemText.TextToControlDistance = 4;
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(1203, 707);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem9
            // 
            this.layoutControlItem9.Control = this.gridControlExpMestDetail;
            this.layoutControlItem9.Location = new System.Drawing.Point(0, 33);
            this.layoutControlItem9.Name = "layoutControlItem9";
            this.layoutControlItem9.Size = new System.Drawing.Size(1203, 353);
            this.layoutControlItem9.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem9.TextVisible = false;
            // 
            // layoutTotalPrice
            // 
            this.layoutTotalPrice.AppearanceItemCaption.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.layoutTotalPrice.AppearanceItemCaption.ForeColor = System.Drawing.Color.Blue;
            this.layoutTotalPrice.AppearanceItemCaption.Options.UseFont = true;
            this.layoutTotalPrice.AppearanceItemCaption.Options.UseForeColor = true;
            this.layoutTotalPrice.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutTotalPrice.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutTotalPrice.Control = this.lblTotalPrice;
            this.layoutTotalPrice.Location = new System.Drawing.Point(504, 386);
            this.layoutTotalPrice.Name = "layoutTotalPrice";
            this.layoutTotalPrice.Size = new System.Drawing.Size(699, 35);
            this.layoutTotalPrice.Text = "Tổng tiền:";
            this.layoutTotalPrice.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutTotalPrice.TextSize = new System.Drawing.Size(100, 20);
            this.layoutTotalPrice.TextToControlDistance = 5;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.layoutControl2;
            this.layoutControlItem2.Location = new System.Drawing.Point(0, 386);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(269, 35);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem3.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem3.Control = this.lblDiscount;
            this.layoutControlItem3.Location = new System.Drawing.Point(269, 386);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(235, 35);
            this.layoutControlItem3.Text = "Chiết khấu:";
            this.layoutControlItem3.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem3.TextSize = new System.Drawing.Size(90, 20);
            this.layoutControlItem3.TextToControlDistance = 5;
            // 
            // layoutControlItem10
            // 
            this.layoutControlItem10.Control = this.btnSave;
            this.layoutControlItem10.Location = new System.Drawing.Point(645, 674);
            this.layoutControlItem10.Name = "layoutControlItem10";
            this.layoutControlItem10.Size = new System.Drawing.Size(139, 33);
            this.layoutControlItem10.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem10.TextVisible = false;
            // 
            // lciExpMestCode
            // 
            this.lciExpMestCode.Control = this.txtExpMestCode;
            this.lciExpMestCode.Location = new System.Drawing.Point(147, 0);
            this.lciExpMestCode.Name = "lciExpMestCode";
            this.lciExpMestCode.Size = new System.Drawing.Size(147, 33);
            this.lciExpMestCode.TextSize = new System.Drawing.Size(0, 0);
            this.lciExpMestCode.TextVisible = false;
            // 
            // layoutControlItem5
            // 
            this.layoutControlItem5.Control = this.btnFind;
            this.layoutControlItem5.Location = new System.Drawing.Point(294, 0);
            this.layoutControlItem5.Name = "layoutControlItem5";
            this.layoutControlItem5.Size = new System.Drawing.Size(147, 33);
            this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem5.TextVisible = false;
            // 
            // emptySpaceItem2
            // 
            this.emptySpaceItem2.AllowHotTrack = false;
            this.emptySpaceItem2.Location = new System.Drawing.Point(441, 0);
            this.emptySpaceItem2.Name = "emptySpaceItem2";
            this.emptySpaceItem2.Size = new System.Drawing.Size(762, 33);
            this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.Control = this.btnSavePrint;
            this.layoutControlItem4.Location = new System.Drawing.Point(784, 674);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Size = new System.Drawing.Size(140, 33);
            this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem4.TextVisible = false;
            // 
            // layoutControlItem7
            // 
            this.layoutControlItem7.Control = this.btnNew;
            this.layoutControlItem7.Location = new System.Drawing.Point(1063, 674);
            this.layoutControlItem7.Name = "layoutControlItem7";
            this.layoutControlItem7.Size = new System.Drawing.Size(140, 33);
            this.layoutControlItem7.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem7.TextVisible = false;
            // 
            // layoutControlItem12
            // 
            this.layoutControlItem12.Control = this.txtTreatmentCode;
            this.layoutControlItem12.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem12.Name = "layoutControlItem12";
            this.layoutControlItem12.Size = new System.Drawing.Size(147, 33);
            this.layoutControlItem12.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem12.TextVisible = false;
            // 
            // layoutControlItem13
            // 
            this.layoutControlItem13.Control = this.ddBtnPrint;
            this.layoutControlItem13.Location = new System.Drawing.Point(924, 674);
            this.layoutControlItem13.Name = "layoutControlItem13";
            this.layoutControlItem13.Size = new System.Drawing.Size(139, 33);
            this.layoutControlItem13.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem13.TextVisible = false;
            // 
            // lciOverTime
            // 
            this.lciOverTime.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciOverTime.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciOverTime.Control = this.checkOverTime;
            this.lciOverTime.Location = new System.Drawing.Point(107, 674);
            this.lciOverTime.Name = "lciOverTime";
            this.lciOverTime.Size = new System.Drawing.Size(165, 33);
            this.lciOverTime.Text = "Ngoài giờ:";
            this.lciOverTime.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciOverTime.TextSize = new System.Drawing.Size(90, 20);
            this.lciOverTime.TextToControlDistance = 5;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.groupBox1;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 421);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(1203, 104);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // layoutControlItem14
            // 
            this.layoutControlItem14.Control = this.groupBox2;
            this.layoutControlItem14.Location = new System.Drawing.Point(0, 525);
            this.layoutControlItem14.Name = "layoutControlItem14";
            this.layoutControlItem14.Size = new System.Drawing.Size(1203, 149);
            this.layoutControlItem14.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem14.TextVisible = false;
            // 
            // lcibtnSaveAndSign
            // 
            this.lcibtnSaveAndSign.Control = this.BtnSaveSign;
            this.lcibtnSaveAndSign.Location = new System.Drawing.Point(532, 674);
            this.lcibtnSaveAndSign.Name = "lcibtnSaveAndSign";
            this.lcibtnSaveAndSign.Size = new System.Drawing.Size(113, 33);
            this.lcibtnSaveAndSign.TextSize = new System.Drawing.Size(0, 0);
            this.lcibtnSaveAndSign.TextVisible = false;
            // 
            // lciHideHddt
            // 
            this.lciHideHddt.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciHideHddt.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciHideHddt.Control = this.chkHideHddt;
            this.lciHideHddt.Location = new System.Drawing.Point(272, 674);
            this.lciHideHddt.Name = "lciHideHddt";
            this.lciHideHddt.Size = new System.Drawing.Size(197, 33);
            this.lciHideHddt.Text = "Không hiển thị HĐ ĐT:";
            this.lciHideHddt.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciHideHddt.TextSize = new System.Drawing.Size(120, 20);
            this.lciHideHddt.TextToControlDistance = 5;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(0, 674);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(107, 33);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // layoutbtnQRCe
            // 
            this.layoutbtnQRCe.Control = this.btnQR;
            this.layoutbtnQRCe.Location = new System.Drawing.Point(469, 674);
            this.layoutbtnQRCe.Name = "layoutbtnQRCe";
            this.layoutbtnQRCe.Size = new System.Drawing.Size(63, 33);
            this.layoutbtnQRCe.TextSize = new System.Drawing.Size(0, 0);
            this.layoutbtnQRCe.TextVisible = false;
            // 
            // dxValidationProviderEditorInfo
            // 
            this.dxValidationProviderEditorInfo.ValidationFailed += new DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventHandler(this.dxValidationProviderEditorInfo_ValidationFailed);
            // 
            // dxErrorProvider
            // 
            this.dxErrorProvider.ContainerControl = this;
            // 
            // popupMenu1
            // 
            this.popupMenu1.Manager = this.barManager1;
            this.popupMenu1.Name = "popupMenu1";
            // 
            // frmMedicineSaleBill
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1203, 745);
            this.Controls.Add(this.lciDiscount);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "frmMedicineSaleBill";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Thanh toán phiếu xuất bán";
            this.Load += new System.EventHandler(this.frmMedicineSaleBill_Load);
            this.Controls.SetChildIndex(this.barDockControlTop, 0);
            this.Controls.SetChildIndex(this.barDockControlBottom, 0);
            this.Controls.SetChildIndex(this.barDockControlRight, 0);
            this.Controls.SetChildIndex(this.barDockControlLeft, 0);
            this.Controls.SetChildIndex(this.lciDiscount, 0);
            ((System.ComponentModel.ISupportInitialize)(this.lciDiscount)).EndInit();
            this.lciDiscount.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chkHideHddt.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl3)).EndInit();
            this.layoutControl3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtDescription.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboAccountBook.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinNumOrder.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCashierRoomCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboCashierRoom.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboPayFrom.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtTransactionTime.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtTransactionTime.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem21)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem22)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem11)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem23)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem24)).EndInit();
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtEmail.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBuyerOgranization.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAddress.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBuyerPhone.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBuyerAccountCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBuyerTaxCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem15)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem17)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem18)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem20)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem16)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem19)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem25)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkOverTime.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTreatmentCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtExpMestCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlExpMestDetail)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewExpMestDetail)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutTotalPrice)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem10)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciExpMestCode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem12)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem13)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciOverTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem14)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcibtnSaveAndSign)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciHideHddt)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutbtnQRCe)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProviderEditorInfo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxErrorProvider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.popupMenu1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl lciDiscount;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.GridLookUpEdit cboAccountBook;
        private DevExpress.XtraGrid.Views.Grid.GridView gridLookUpEdit1View;
        private DevExpress.XtraEditors.SimpleButton btnSave;
        private DevExpress.XtraEditors.DateEdit dtTransactionTime;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem10;
        private DevExpress.XtraEditors.GridLookUpEdit cboCashierRoom;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraEditors.TextEdit txtCashierRoomCode;
        private Inventec.Desktop.CustomControl.MyGridControl gridControlExpMestDetail;
        private Inventec.Desktop.CustomControl.MyGridView gridViewExpMestDetail;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_ExpMestDetail_MediMateTypeName;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_ExpMestDetail_ServiceUnitName;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_ExpMestDetail_ExpAmount;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_ExpMestDetail_AdvisoryPrice;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_ExpMestDetail_AdvisoryTotalPrice;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_ExpMestDetail_NationalName;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_ExpMestDetail_ManufacturerName;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem9;
        private DevExpress.XtraEditors.GridLookUpEdit cboPayFrom;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView2;
        private DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProviderEditorInfo;
        private DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider dxErrorProvider;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.Bar bar1;
        private DevExpress.XtraBars.BarButtonItem barButtonItemSave;
        private DevExpress.XtraEditors.LabelControl lblTotalPrice;
        private DevExpress.XtraLayout.LayoutControlItem layoutTotalPrice;
        private DevExpress.XtraLayout.LayoutControl layoutControl2;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_ExpMestDetail_VatRatio;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_ExpMestDetail_Discount;
        private DevExpress.XtraEditors.LabelControl lblDiscount;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraEditors.SpinEdit spinNumOrder;
        private DevExpress.XtraEditors.TextEdit txtExpMestCode;
        private DevExpress.XtraLayout.LayoutControlItem lciExpMestCode;
        private DevExpress.XtraEditors.SimpleButton btnFind;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
        private DevExpress.XtraEditors.SimpleButton btnSavePrint;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraBars.BarButtonItem barBtnSavePrint;
        private DevExpress.XtraEditors.SimpleButton btnNew;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem7;
        private DevExpress.XtraBars.BarButtonItem barBtnPrint;
        private DevExpress.XtraBars.BarButtonItem barBtnFind;
        private DevExpress.XtraBars.BarButtonItem barBtnFocus;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_ExpMestDetail_ExpMestCode;
        private DevExpress.XtraGrid.Columns.GridColumn colSTTUnb;
        private DevExpress.XtraGrid.Columns.GridColumn colMEDI_MATE_TYPE_NAMEUnb;
        private DevExpress.XtraGrid.Columns.GridColumn colSERVICE_UNIT_NAMEUnb;
        private DevExpress.XtraGrid.Columns.GridColumn colEXP_AMOUNTUnb;
        private DevExpress.XtraGrid.Columns.GridColumn colADVISORY_PRICE_DISPLAYUnb;
        private DevExpress.XtraGrid.Columns.GridColumn colVAT_RATIO_STRUnb;
        private DevExpress.XtraGrid.Columns.GridColumn colDISCOUNT_STRUnb;
        private DevExpress.XtraGrid.Columns.GridColumn colADVISORY_TOTAL_PRICE_DISPLAYUnb;
        private DevExpress.XtraGrid.Columns.GridColumn colNATIONAL_NAMEUnb;
        private DevExpress.XtraGrid.Columns.GridColumn colMANUFACTURER_NAMEUnb;
        private DevExpress.XtraGrid.Columns.GridColumn colUnb;
        private DevExpress.XtraGrid.Columns.GridColumn colEXP_MEST_CODEUnb;
        private DevExpress.XtraGrid.Columns.GridColumn colCheckUnb;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraEditors.TextEdit txtTreatmentCode;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem12;
        private DevExpress.XtraBars.BarButtonItem barBtnNew;
        private DevExpress.XtraEditors.DropDownButton ddBtnPrint;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem13;
        private DevExpress.XtraEditors.CheckEdit checkOverTime;
        private DevExpress.XtraLayout.LayoutControlItem lciOverTime;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox1;
        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraEditors.TextEdit txtAddress;
        private DevExpress.XtraEditors.TextEdit txtBuyerPhone;
        private DevExpress.XtraEditors.TextEdit txtBuyerAccountCode;
        private DevExpress.XtraEditors.TextEdit txtBuyerTaxCode;
        private DevExpress.XtraEditors.TextEdit txtName;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem15;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem17;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem18;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem19;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem20;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem14;
        private DevExpress.XtraLayout.LayoutControl layoutControl3;
        private DevExpress.XtraEditors.TextEdit txtDescription;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup3;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem6;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem21;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem22;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem8;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem11;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem23;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem24;
        private DevExpress.XtraEditors.TextEdit txtBuyerOgranization;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem16;
        private DevExpress.XtraEditors.SimpleButton BtnSaveSign;
        private DevExpress.XtraLayout.LayoutControlItem lcibtnSaveAndSign;
        private DevExpress.XtraBars.BarButtonItem barBtnSaveSign;
        private DevExpress.XtraEditors.CheckEdit chkHideHddt;
        private DevExpress.XtraLayout.LayoutControlItem lciHideHddt;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraEditors.SimpleButton btnQR;
        private DevExpress.XtraLayout.LayoutControlItem layoutbtnQRCe;
        private DevExpress.XtraBars.PopupMenu popupMenu1;
        private DevExpress.XtraEditors.TextEdit txtEmail;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem25;
    }
}
