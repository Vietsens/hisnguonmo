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
namespace HIS.Desktop.Plugins.MedicineVaccinBill
{
    partial class frmMedicineVaccinBill : HIS.Desktop.Utility.FormBase
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
            this.chkPrintEInvoice = new DevExpress.XtraEditors.CheckEdit();
            this.barManager1 = new DevExpress.XtraBars.BarManager();
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.barButtonItemSave = new DevExpress.XtraBars.BarButtonItem();
            this.barBtnSavePrint = new DevExpress.XtraBars.BarButtonItem();
            this.barBtnPrint = new DevExpress.XtraBars.BarButtonItem();
            this.barBtnFind = new DevExpress.XtraBars.BarButtonItem();
            this.barBtnFocus = new DevExpress.XtraBars.BarButtonItem();
            this.barBtnNew = new DevExpress.XtraBars.BarButtonItem();
            this.barBtnSaveAndSign = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.btnSaveAndSign = new DevExpress.XtraEditors.SimpleButton();
            this.btnNew = new DevExpress.XtraEditors.SimpleButton();
            this.lblDob = new DevExpress.XtraEditors.LabelControl();
            this.lblGender = new DevExpress.XtraEditors.LabelControl();
            this.lblPatientName = new DevExpress.XtraEditors.LabelControl();
            this.lblPatientCode = new DevExpress.XtraEditors.LabelControl();
            this.btnFind = new DevExpress.XtraEditors.SimpleButton();
            this.txtFindVaccinationCode = new DevExpress.XtraEditors.TextEdit();
            this.txtFindPatientCode = new DevExpress.XtraEditors.TextEdit();
            this.gridControlData = new DevExpress.XtraGrid.GridControl();
            this.gridViewData = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn4 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn5 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn6 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn7 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn8 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.btnSavePrint = new DevExpress.XtraEditors.SimpleButton();
            this.txtBuyerPhone = new DevExpress.XtraEditors.TextEdit();
            this.spinNumOrder = new DevExpress.XtraEditors.SpinEdit();
            this.txtBuyerAccountCode = new DevExpress.XtraEditors.TextEdit();
            this.txtBuyerTaxCode = new DevExpress.XtraEditors.TextEdit();
            this.txtBuyerOgranization = new DevExpress.XtraEditors.TextEdit();
            this.lblDiscount = new DevExpress.XtraEditors.LabelControl();
            this.layoutControl2 = new DevExpress.XtraLayout.LayoutControl();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lblTotalPrice = new DevExpress.XtraEditors.LabelControl();
            this.ddBtnPrint = new DevExpress.XtraEditors.DropDownButton();
            this.cboPayFrom = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridView2 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.txtPayFormCode = new DevExpress.XtraEditors.TextEdit();
            this.cboCashierRoom = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.txtCashierRoomCode = new DevExpress.XtraEditors.TextEdit();
            this.txtDescription = new DevExpress.XtraEditors.TextEdit();
            this.btnSave = new DevExpress.XtraEditors.SimpleButton();
            this.dtTransactionTime = new DevExpress.XtraEditors.DateEdit();
            this.cboAccountBook = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridLookUpEdit1View = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.txtAccountBookCode = new DevExpress.XtraEditors.TextEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutAccountBook = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutDescription = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutCashierRoom = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem8 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutPayForm = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem11 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutTotalPrice = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciBuyerOgranization = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciBuyerTaxCode = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciBuyerAccountCode = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciNumOrder = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutTransactionTime = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem10 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.lciBuyerPhone = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem13 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciFindPatientCode = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciFindVaccinationCode = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem9 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.lciPatientCode = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciPatientName = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciGender = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciDob = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciSaveAndSign = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem12 = new DevExpress.XtraLayout.LayoutControlItem();
            this.dxValidationProviderEditorInfo = new DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider();
            this.dxErrorProvider = new DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider();
            ((System.ComponentModel.ISupportInitialize)(this.lciDiscount)).BeginInit();
            this.lciDiscount.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chkPrintEInvoice.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtFindVaccinationCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtFindPatientCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBuyerPhone.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinNumOrder.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBuyerAccountCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBuyerTaxCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBuyerOgranization.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboPayFrom.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPayFormCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboCashierRoom.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCashierRoomCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDescription.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtTransactionTime.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtTransactionTime.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboAccountBook.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAccountBookCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutAccountBook)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutDescription)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutCashierRoom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutPayForm)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem11)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutTotalPrice)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBuyerOgranization)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBuyerTaxCode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBuyerAccountCode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciNumOrder)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutTransactionTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem10)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBuyerPhone)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem13)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciFindPatientCode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciFindVaccinationCode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPatientCode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPatientName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciGender)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDob)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciSaveAndSign)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem12)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProviderEditorInfo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxErrorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // lciDiscount
            // 
            this.lciDiscount.Controls.Add(this.chkPrintEInvoice);
            this.lciDiscount.Controls.Add(this.btnSaveAndSign);
            this.lciDiscount.Controls.Add(this.btnNew);
            this.lciDiscount.Controls.Add(this.lblDob);
            this.lciDiscount.Controls.Add(this.lblGender);
            this.lciDiscount.Controls.Add(this.lblPatientName);
            this.lciDiscount.Controls.Add(this.lblPatientCode);
            this.lciDiscount.Controls.Add(this.btnFind);
            this.lciDiscount.Controls.Add(this.txtFindVaccinationCode);
            this.lciDiscount.Controls.Add(this.txtFindPatientCode);
            this.lciDiscount.Controls.Add(this.gridControlData);
            this.lciDiscount.Controls.Add(this.btnSavePrint);
            this.lciDiscount.Controls.Add(this.txtBuyerPhone);
            this.lciDiscount.Controls.Add(this.spinNumOrder);
            this.lciDiscount.Controls.Add(this.txtBuyerAccountCode);
            this.lciDiscount.Controls.Add(this.txtBuyerTaxCode);
            this.lciDiscount.Controls.Add(this.txtBuyerOgranization);
            this.lciDiscount.Controls.Add(this.lblDiscount);
            this.lciDiscount.Controls.Add(this.layoutControl2);
            this.lciDiscount.Controls.Add(this.lblTotalPrice);
            this.lciDiscount.Controls.Add(this.ddBtnPrint);
            this.lciDiscount.Controls.Add(this.cboPayFrom);
            this.lciDiscount.Controls.Add(this.txtPayFormCode);
            this.lciDiscount.Controls.Add(this.cboCashierRoom);
            this.lciDiscount.Controls.Add(this.txtCashierRoomCode);
            this.lciDiscount.Controls.Add(this.txtDescription);
            this.lciDiscount.Controls.Add(this.btnSave);
            this.lciDiscount.Controls.Add(this.dtTransactionTime);
            this.lciDiscount.Controls.Add(this.cboAccountBook);
            this.lciDiscount.Controls.Add(this.txtAccountBookCode);
            this.lciDiscount.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lciDiscount.Location = new System.Drawing.Point(0, 29);
            this.lciDiscount.Margin = new System.Windows.Forms.Padding(2);
            this.lciDiscount.Name = "lciDiscount";
            this.lciDiscount.Root = this.layoutControlGroup1;
            this.lciDiscount.Size = new System.Drawing.Size(880, 582);
            this.lciDiscount.TabIndex = 0;
            this.lciDiscount.Text = "Chiết khấu chung:";
            // 
            // chkPrintEInvoice
            // 
            this.chkPrintEInvoice.Location = new System.Drawing.Point(222, 558);
            this.chkPrintEInvoice.MenuManager = this.barManager1;
            this.chkPrintEInvoice.Name = "chkPrintEInvoice";
            this.chkPrintEInvoice.Properties.Caption = "In HĐĐT";
            this.chkPrintEInvoice.Size = new System.Drawing.Size(106, 19);
            this.chkPrintEInvoice.StyleController = this.lciDiscount;
            this.chkPrintEInvoice.TabIndex = 42;
            this.chkPrintEInvoice.ToolTip = "In hóa đơn điện tử khi \"Lưu ký\"";
            this.chkPrintEInvoice.CheckedChanged += new System.EventHandler(this.chkPrintEInvoice_CheckedChanged);
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
            this.barBtnSaveAndSign});
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
            new DevExpress.XtraBars.LinkPersistInfo(this.barBtnSaveAndSign)});
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
            // barBtnSaveAndSign
            // 
            this.barBtnSaveAndSign.Caption = "Lưu ký (Ctrl A)";
            this.barBtnSaveAndSign.Id = 6;
            this.barBtnSaveAndSign.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A));
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
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 611);
            this.barDockControlBottom.Size = new System.Drawing.Size(880, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 29);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 582);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(880, 29);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 582);
            // 
            // btnSaveAndSign
            // 
            this.btnSaveAndSign.Location = new System.Drawing.Point(332, 558);
            this.btnSaveAndSign.Name = "btnSaveAndSign";
            this.btnSaveAndSign.Size = new System.Drawing.Size(106, 22);
            this.btnSaveAndSign.StyleController = this.lciDiscount;
            this.btnSaveAndSign.TabIndex = 41;
            this.btnSaveAndSign.Text = "Lưu ký (Ctrl A)";
            this.btnSaveAndSign.Click += new System.EventHandler(this.btnSaveAndSign_Click);
            // 
            // btnNew
            // 
            this.btnNew.Location = new System.Drawing.Point(772, 558);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(106, 22);
            this.btnNew.StyleController = this.lciDiscount;
            this.btnNew.TabIndex = 40;
            this.btnNew.Text = "Mới (Ctrl N)";
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // lblDob
            // 
            this.lblDob.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblDob.Appearance.ForeColor = System.Drawing.Color.Blue;
            this.lblDob.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblDob.Location = new System.Drawing.Point(757, 432);
            this.lblDob.Name = "lblDob";
            this.lblDob.Size = new System.Drawing.Size(121, 20);
            this.lblDob.StyleController = this.lciDiscount;
            this.lblDob.TabIndex = 39;
            // 
            // lblGender
            // 
            this.lblGender.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblGender.Appearance.ForeColor = System.Drawing.Color.Blue;
            this.lblGender.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblGender.Location = new System.Drawing.Point(537, 432);
            this.lblGender.Name = "lblGender";
            this.lblGender.Size = new System.Drawing.Size(121, 20);
            this.lblGender.StyleController = this.lciDiscount;
            this.lblGender.TabIndex = 38;
            // 
            // lblPatientName
            // 
            this.lblPatientName.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblPatientName.Appearance.ForeColor = System.Drawing.Color.Blue;
            this.lblPatientName.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblPatientName.Location = new System.Drawing.Point(317, 432);
            this.lblPatientName.Name = "lblPatientName";
            this.lblPatientName.Size = new System.Drawing.Size(141, 20);
            this.lblPatientName.StyleController = this.lciDiscount;
            this.lblPatientName.TabIndex = 37;
            // 
            // lblPatientCode
            // 
            this.lblPatientCode.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblPatientCode.Appearance.ForeColor = System.Drawing.Color.Blue;
            this.lblPatientCode.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblPatientCode.Location = new System.Drawing.Point(97, 432);
            this.lblPatientCode.Name = "lblPatientCode";
            this.lblPatientCode.Size = new System.Drawing.Size(121, 20);
            this.lblPatientCode.StyleController = this.lciDiscount;
            this.lblPatientCode.TabIndex = 36;
            // 
            // btnFind
            // 
            this.btnFind.Location = new System.Drawing.Point(250, 2);
            this.btnFind.Name = "btnFind";
            this.btnFind.Size = new System.Drawing.Size(120, 22);
            this.btnFind.StyleController = this.lciDiscount;
            this.btnFind.TabIndex = 35;
            this.btnFind.Text = "Tìm (Ctrl F)";
            this.btnFind.Click += new System.EventHandler(this.btnFind_Click);
            // 
            // txtFindVaccinationCode
            // 
            this.txtFindVaccinationCode.Location = new System.Drawing.Point(126, 2);
            this.txtFindVaccinationCode.MenuManager = this.barManager1;
            this.txtFindVaccinationCode.Name = "txtFindVaccinationCode";
            this.txtFindVaccinationCode.Properties.NullValuePrompt = "Mã chỉ định tiêm";
            this.txtFindVaccinationCode.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtFindVaccinationCode.Properties.ShowNullValuePromptWhenFocused = true;
            this.txtFindVaccinationCode.Size = new System.Drawing.Size(120, 20);
            this.txtFindVaccinationCode.StyleController = this.lciDiscount;
            this.txtFindVaccinationCode.TabIndex = 34;
            this.txtFindVaccinationCode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtFindVaccinationCode_KeyDown);
            // 
            // txtFindPatientCode
            // 
            this.txtFindPatientCode.Location = new System.Drawing.Point(2, 2);
            this.txtFindPatientCode.MenuManager = this.barManager1;
            this.txtFindPatientCode.Name = "txtFindPatientCode";
            this.txtFindPatientCode.Properties.NullValuePrompt = "Mã bệnh nhân F2";
            this.txtFindPatientCode.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtFindPatientCode.Properties.ShowNullValuePromptWhenFocused = true;
            this.txtFindPatientCode.Size = new System.Drawing.Size(120, 20);
            this.txtFindPatientCode.StyleController = this.lciDiscount;
            this.txtFindPatientCode.TabIndex = 33;
            this.txtFindPatientCode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtFindPatientCode_KeyDown);
            // 
            // gridControlData
            // 
            this.gridControlData.Location = new System.Drawing.Point(2, 28);
            this.gridControlData.MainView = this.gridViewData;
            this.gridControlData.MenuManager = this.barManager1;
            this.gridControlData.Name = "gridControlData";
            this.gridControlData.Size = new System.Drawing.Size(876, 400);
            this.gridControlData.TabIndex = 32;
            this.gridControlData.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewData});
            // 
            // gridViewData
            // 
            this.gridViewData.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn2,
            this.gridColumn3,
            this.gridColumn4,
            this.gridColumn5,
            this.gridColumn6,
            this.gridColumn7,
            this.gridColumn8});
            this.gridViewData.GridControl = this.gridControlData;
            this.gridViewData.Name = "gridViewData";
            this.gridViewData.OptionsView.ShowGroupPanel = false;
            this.gridViewData.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewData_CustomUnboundColumnData);
            // 
            // gridColumn2
            // 
            this.gridColumn2.Caption = "STT";
            this.gridColumn2.FieldName = "STT";
            this.gridColumn2.Name = "gridColumn2";
            this.gridColumn2.OptionsColumn.AllowEdit = false;
            this.gridColumn2.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn2.Visible = true;
            this.gridColumn2.VisibleIndex = 0;
            this.gridColumn2.Width = 38;
            // 
            // gridColumn3
            // 
            this.gridColumn3.Caption = "Mã";
            this.gridColumn3.FieldName = "MEDICINE_TYPE_CODE";
            this.gridColumn3.Name = "gridColumn3";
            this.gridColumn3.OptionsColumn.AllowEdit = false;
            this.gridColumn3.Visible = true;
            this.gridColumn3.VisibleIndex = 1;
            this.gridColumn3.Width = 138;
            // 
            // gridColumn4
            // 
            this.gridColumn4.Caption = "Tên";
            this.gridColumn4.FieldName = "MEDICINE_TYPE_NAME";
            this.gridColumn4.Name = "gridColumn4";
            this.gridColumn4.OptionsColumn.AllowEdit = false;
            this.gridColumn4.Visible = true;
            this.gridColumn4.VisibleIndex = 2;
            this.gridColumn4.Width = 125;
            // 
            // gridColumn5
            // 
            this.gridColumn5.Caption = "ĐVT";
            this.gridColumn5.FieldName = "SERVICE_UNIT_NAME";
            this.gridColumn5.Name = "gridColumn5";
            this.gridColumn5.OptionsColumn.AllowEdit = false;
            this.gridColumn5.Visible = true;
            this.gridColumn5.VisibleIndex = 3;
            this.gridColumn5.Width = 175;
            // 
            // gridColumn6
            // 
            this.gridColumn6.Caption = "Số lượng";
            this.gridColumn6.FieldName = "AMOUNT";
            this.gridColumn6.Name = "gridColumn6";
            this.gridColumn6.OptionsColumn.AllowEdit = false;
            this.gridColumn6.Visible = true;
            this.gridColumn6.VisibleIndex = 4;
            this.gridColumn6.Width = 158;
            // 
            // gridColumn7
            // 
            this.gridColumn7.Caption = "Đơn giá";
            this.gridColumn7.FieldName = "IMP_PRICE";
            this.gridColumn7.Name = "gridColumn7";
            this.gridColumn7.OptionsColumn.AllowEdit = false;
            this.gridColumn7.Visible = true;
            this.gridColumn7.VisibleIndex = 5;
            this.gridColumn7.Width = 115;
            // 
            // gridColumn8
            // 
            this.gridColumn8.Caption = "VAT(%)";
            this.gridColumn8.FieldName = "VAT_RATIO_STR";
            this.gridColumn8.Name = "gridColumn8";
            this.gridColumn8.OptionsColumn.AllowEdit = false;
            this.gridColumn8.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn8.Visible = true;
            this.gridColumn8.VisibleIndex = 6;
            this.gridColumn8.Width = 131;
            // 
            // btnSavePrint
            // 
            this.btnSavePrint.Location = new System.Drawing.Point(552, 558);
            this.btnSavePrint.Name = "btnSavePrint";
            this.btnSavePrint.Size = new System.Drawing.Size(106, 22);
            this.btnSavePrint.StyleController = this.lciDiscount;
            this.btnSavePrint.TabIndex = 29;
            this.btnSavePrint.Text = "Lưu in (Ctrl I)";
            this.btnSavePrint.Click += new System.EventHandler(this.btnSavePrint_Click);
            // 
            // txtBuyerPhone
            // 
            this.txtBuyerPhone.Location = new System.Drawing.Point(757, 456);
            this.txtBuyerPhone.MenuManager = this.barManager1;
            this.txtBuyerPhone.Name = "txtBuyerPhone";
            this.txtBuyerPhone.Size = new System.Drawing.Size(121, 20);
            this.txtBuyerPhone.StyleController = this.lciDiscount;
            this.txtBuyerPhone.TabIndex = 8;
            this.txtBuyerPhone.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtBuyerPhone_PreviewKeyDown);
            // 
            // spinNumOrder
            // 
            this.spinNumOrder.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.spinNumOrder.Location = new System.Drawing.Point(390, 506);
            this.spinNumOrder.MenuManager = this.barManager1;
            this.spinNumOrder.Name = "spinNumOrder";
            this.spinNumOrder.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.spinNumOrder.Size = new System.Drawing.Size(194, 20);
            this.spinNumOrder.StyleController = this.lciDiscount;
            this.spinNumOrder.TabIndex = 11;
            // 
            // txtBuyerAccountCode
            // 
            this.txtBuyerAccountCode.Location = new System.Drawing.Point(537, 456);
            this.txtBuyerAccountCode.MenuManager = this.barManager1;
            this.txtBuyerAccountCode.Name = "txtBuyerAccountCode";
            this.txtBuyerAccountCode.Size = new System.Drawing.Size(121, 20);
            this.txtBuyerAccountCode.StyleController = this.lciDiscount;
            this.txtBuyerAccountCode.TabIndex = 7;
            this.txtBuyerAccountCode.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtBuyerAccountCode_PreviewKeyDown);
            // 
            // txtBuyerTaxCode
            // 
            this.txtBuyerTaxCode.Location = new System.Drawing.Point(317, 456);
            this.txtBuyerTaxCode.MenuManager = this.barManager1;
            this.txtBuyerTaxCode.Name = "txtBuyerTaxCode";
            this.txtBuyerTaxCode.Size = new System.Drawing.Size(121, 20);
            this.txtBuyerTaxCode.StyleController = this.lciDiscount;
            this.txtBuyerTaxCode.TabIndex = 6;
            this.txtBuyerTaxCode.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtBuyerTaxCode_PreviewKeyDown);
            // 
            // txtBuyerOgranization
            // 
            this.txtBuyerOgranization.Location = new System.Drawing.Point(97, 456);
            this.txtBuyerOgranization.MenuManager = this.barManager1;
            this.txtBuyerOgranization.Name = "txtBuyerOgranization";
            this.txtBuyerOgranization.Size = new System.Drawing.Size(121, 20);
            this.txtBuyerOgranization.StyleController = this.lciDiscount;
            this.txtBuyerOgranization.TabIndex = 5;
            this.txtBuyerOgranization.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtBuyerOgranization_PreviewKeyDown);
            // 
            // lblDiscount
            // 
            this.lblDiscount.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lblDiscount.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblDiscount.Location = new System.Drawing.Point(436, 480);
            this.lblDiscount.Name = "lblDiscount";
            this.lblDiscount.Size = new System.Drawing.Size(240, 20);
            this.lblDiscount.StyleController = this.lciDiscount;
            this.lblDiscount.TabIndex = 26;
            // 
            // layoutControl2
            // 
            this.layoutControl2.Location = new System.Drawing.Point(2, 480);
            this.layoutControl2.Margin = new System.Windows.Forms.Padding(2);
            this.layoutControl2.Name = "layoutControl2";
            this.layoutControl2.Root = this.Root;
            this.layoutControl2.Size = new System.Drawing.Size(335, 22);
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
            this.Root.Size = new System.Drawing.Size(335, 22);
            this.Root.TextVisible = false;
            // 
            // lblTotalPrice
            // 
            this.lblTotalPrice.Appearance.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalPrice.Appearance.ForeColor = System.Drawing.Color.Blue;
            this.lblTotalPrice.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lblTotalPrice.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblTotalPrice.Location = new System.Drawing.Point(785, 480);
            this.lblTotalPrice.Margin = new System.Windows.Forms.Padding(2);
            this.lblTotalPrice.Name = "lblTotalPrice";
            this.lblTotalPrice.Size = new System.Drawing.Size(93, 22);
            this.lblTotalPrice.StyleController = this.lciDiscount;
            this.lblTotalPrice.TabIndex = 24;
            // 
            // ddBtnPrint
            // 
            this.ddBtnPrint.DropDownArrowStyle = DevExpress.XtraEditors.DropDownArrowStyle.Hide;
            this.ddBtnPrint.Location = new System.Drawing.Point(662, 558);
            this.ddBtnPrint.Margin = new System.Windows.Forms.Padding(2);
            this.ddBtnPrint.MenuManager = this.barManager1;
            this.ddBtnPrint.Name = "ddBtnPrint";
            this.ddBtnPrint.Size = new System.Drawing.Size(106, 22);
            this.ddBtnPrint.StyleController = this.lciDiscount;
            this.ddBtnPrint.TabIndex = 19;
            this.ddBtnPrint.Text = "In (Ctrl P)";
            this.ddBtnPrint.Click += new System.EventHandler(this.ddBtnPrint_Click);
            // 
            // cboPayFrom
            // 
            this.cboPayFrom.Location = new System.Drawing.Point(150, 533);
            this.cboPayFrom.Margin = new System.Windows.Forms.Padding(2);
            this.cboPayFrom.Name = "cboPayFrom";
            this.cboPayFrom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboPayFrom.Properties.NullText = "";
            this.cboPayFrom.Properties.View = this.gridView2;
            this.cboPayFrom.Size = new System.Drawing.Size(140, 20);
            this.cboPayFrom.StyleController = this.lciDiscount;
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
            // txtPayFormCode
            // 
            this.txtPayFormCode.Location = new System.Drawing.Point(98, 533);
            this.txtPayFormCode.Margin = new System.Windows.Forms.Padding(2);
            this.txtPayFormCode.Name = "txtPayFormCode";
            this.txtPayFormCode.Size = new System.Drawing.Size(52, 20);
            this.txtPayFormCode.StyleController = this.lciDiscount;
            this.txtPayFormCode.TabIndex = 14;
            this.txtPayFormCode.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtPayFormCode_PreviewKeyDown);
            // 
            // cboCashierRoom
            // 
            this.cboCashierRoom.Location = new System.Drawing.Point(736, 507);
            this.cboCashierRoom.Margin = new System.Windows.Forms.Padding(2);
            this.cboCashierRoom.Name = "cboCashierRoom";
            this.cboCashierRoom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboCashierRoom.Properties.NullText = "";
            this.cboCashierRoom.Properties.View = this.gridView1;
            this.cboCashierRoom.Size = new System.Drawing.Size(141, 20);
            this.cboCashierRoom.StyleController = this.lciDiscount;
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
            // txtCashierRoomCode
            // 
            this.txtCashierRoomCode.Location = new System.Drawing.Point(684, 507);
            this.txtCashierRoomCode.Margin = new System.Windows.Forms.Padding(2);
            this.txtCashierRoomCode.Name = "txtCashierRoomCode";
            this.txtCashierRoomCode.Size = new System.Drawing.Size(52, 20);
            this.txtCashierRoomCode.StyleController = this.lciDiscount;
            this.txtCashierRoomCode.TabIndex = 12;
            this.txtCashierRoomCode.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtCashierRoomCode_PreviewKeyDown);
            // 
            // txtDescription
            // 
            this.txtDescription.Location = new System.Drawing.Point(683, 532);
            this.txtDescription.Margin = new System.Windows.Forms.Padding(2);
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(195, 20);
            this.txtDescription.StyleController = this.lciDiscount;
            this.txtDescription.TabIndex = 17;
            this.txtDescription.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtDescription_KeyDown);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(442, 558);
            this.btnSave.Margin = new System.Windows.Forms.Padding(2);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(106, 22);
            this.btnSave.StyleController = this.lciDiscount;
            this.btnSave.TabIndex = 18;
            this.btnSave.Text = "Lưu (Ctrl S)";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // dtTransactionTime
            // 
            this.dtTransactionTime.EditValue = null;
            this.dtTransactionTime.Location = new System.Drawing.Point(390, 532);
            this.dtTransactionTime.Margin = new System.Windows.Forms.Padding(2);
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
            this.dtTransactionTime.Size = new System.Drawing.Size(194, 20);
            this.dtTransactionTime.StyleController = this.lciDiscount;
            this.dtTransactionTime.TabIndex = 16;
            this.dtTransactionTime.KeyUp += new System.Windows.Forms.KeyEventHandler(this.dtTransactionTime_KeyUp);
            // 
            // cboAccountBook
            // 
            this.cboAccountBook.Location = new System.Drawing.Point(150, 507);
            this.cboAccountBook.Margin = new System.Windows.Forms.Padding(2);
            this.cboAccountBook.Name = "cboAccountBook";
            this.cboAccountBook.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboAccountBook.Properties.NullText = "";
            this.cboAccountBook.Properties.View = this.gridLookUpEdit1View;
            this.cboAccountBook.Size = new System.Drawing.Size(140, 20);
            this.cboAccountBook.StyleController = this.lciDiscount;
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
            // txtAccountBookCode
            // 
            this.txtAccountBookCode.Location = new System.Drawing.Point(98, 507);
            this.txtAccountBookCode.Margin = new System.Windows.Forms.Padding(2);
            this.txtAccountBookCode.Name = "txtAccountBookCode";
            this.txtAccountBookCode.Size = new System.Drawing.Size(52, 20);
            this.txtAccountBookCode.StyleController = this.lciDiscount;
            this.txtAccountBookCode.TabIndex = 9;
            this.txtAccountBookCode.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtAccountBookCode_PreviewKeyDown);
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutAccountBook,
            this.layoutControlItem6,
            this.layoutDescription,
            this.layoutCashierRoom,
            this.layoutControlItem8,
            this.layoutPayForm,
            this.layoutControlItem11,
            this.layoutTotalPrice,
            this.layoutControlItem2,
            this.layoutControlItem3,
            this.lciBuyerOgranization,
            this.lciBuyerTaxCode,
            this.lciBuyerAccountCode,
            this.lciNumOrder,
            this.layoutTransactionTime,
            this.layoutControlItem10,
            this.layoutControlItem1,
            this.emptySpaceItem1,
            this.lciBuyerPhone,
            this.layoutControlItem4,
            this.layoutControlItem13,
            this.lciFindPatientCode,
            this.lciFindVaccinationCode,
            this.layoutControlItem9,
            this.emptySpaceItem2,
            this.lciPatientCode,
            this.lciPatientName,
            this.lciGender,
            this.lciDob,
            this.layoutControlItem5,
            this.lciSaveAndSign,
            this.layoutControlItem12});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.OptionsItemText.TextToControlDistance = 4;
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(880, 582);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutAccountBook
            // 
            this.layoutAccountBook.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.layoutAccountBook.AppearanceItemCaption.Options.UseForeColor = true;
            this.layoutAccountBook.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutAccountBook.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutAccountBook.Control = this.txtAccountBookCode;
            this.layoutAccountBook.Location = new System.Drawing.Point(0, 504);
            this.layoutAccountBook.MaxSize = new System.Drawing.Size(0, 26);
            this.layoutAccountBook.MinSize = new System.Drawing.Size(130, 26);
            this.layoutAccountBook.Name = "layoutAccountBook";
            this.layoutAccountBook.Padding = new DevExpress.XtraLayout.Utils.Padding(3, 0, 3, 3);
            this.layoutAccountBook.Size = new System.Drawing.Size(150, 26);
            this.layoutAccountBook.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutAccountBook.Text = "Sổ thu chi:";
            this.layoutAccountBook.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutAccountBook.TextSize = new System.Drawing.Size(90, 20);
            this.layoutAccountBook.TextToControlDistance = 5;
            // 
            // layoutControlItem6
            // 
            this.layoutControlItem6.Control = this.cboAccountBook;
            this.layoutControlItem6.Location = new System.Drawing.Point(150, 504);
            this.layoutControlItem6.Name = "layoutControlItem6";
            this.layoutControlItem6.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 3, 3, 3);
            this.layoutControlItem6.Size = new System.Drawing.Size(143, 26);
            this.layoutControlItem6.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem6.TextVisible = false;
            // 
            // layoutDescription
            // 
            this.layoutDescription.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutDescription.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutDescription.Control = this.txtDescription;
            this.layoutDescription.Location = new System.Drawing.Point(586, 530);
            this.layoutDescription.Name = "layoutDescription";
            this.layoutDescription.Size = new System.Drawing.Size(294, 26);
            this.layoutDescription.Text = "Mô tả:";
            this.layoutDescription.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutDescription.TextSize = new System.Drawing.Size(90, 20);
            this.layoutDescription.TextToControlDistance = 5;
            // 
            // layoutCashierRoom
            // 
            this.layoutCashierRoom.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.layoutCashierRoom.AppearanceItemCaption.Options.UseForeColor = true;
            this.layoutCashierRoom.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutCashierRoom.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutCashierRoom.Control = this.txtCashierRoomCode;
            this.layoutCashierRoom.Location = new System.Drawing.Point(586, 504);
            this.layoutCashierRoom.Name = "layoutCashierRoom";
            this.layoutCashierRoom.Padding = new DevExpress.XtraLayout.Utils.Padding(3, 0, 3, 3);
            this.layoutCashierRoom.Size = new System.Drawing.Size(150, 26);
            this.layoutCashierRoom.Text = "Phòng thu ngân:";
            this.layoutCashierRoom.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutCashierRoom.TextSize = new System.Drawing.Size(90, 20);
            this.layoutCashierRoom.TextToControlDistance = 5;
            // 
            // layoutControlItem8
            // 
            this.layoutControlItem8.Control = this.cboCashierRoom;
            this.layoutControlItem8.Location = new System.Drawing.Point(736, 504);
            this.layoutControlItem8.Name = "layoutControlItem8";
            this.layoutControlItem8.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 3, 3, 3);
            this.layoutControlItem8.Size = new System.Drawing.Size(144, 26);
            this.layoutControlItem8.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem8.TextVisible = false;
            // 
            // layoutPayForm
            // 
            this.layoutPayForm.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.layoutPayForm.AppearanceItemCaption.Options.UseForeColor = true;
            this.layoutPayForm.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutPayForm.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutPayForm.Control = this.txtPayFormCode;
            this.layoutPayForm.Location = new System.Drawing.Point(0, 530);
            this.layoutPayForm.MaxSize = new System.Drawing.Size(0, 26);
            this.layoutPayForm.MinSize = new System.Drawing.Size(130, 26);
            this.layoutPayForm.Name = "layoutPayForm";
            this.layoutPayForm.Padding = new DevExpress.XtraLayout.Utils.Padding(3, 0, 3, 3);
            this.layoutPayForm.Size = new System.Drawing.Size(150, 26);
            this.layoutPayForm.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutPayForm.Text = "Hình thức:";
            this.layoutPayForm.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutPayForm.TextSize = new System.Drawing.Size(90, 20);
            this.layoutPayForm.TextToControlDistance = 5;
            // 
            // layoutControlItem11
            // 
            this.layoutControlItem11.Control = this.cboPayFrom;
            this.layoutControlItem11.Location = new System.Drawing.Point(150, 530);
            this.layoutControlItem11.Name = "layoutControlItem11";
            this.layoutControlItem11.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 3, 3, 3);
            this.layoutControlItem11.Size = new System.Drawing.Size(143, 26);
            this.layoutControlItem11.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem11.TextVisible = false;
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
            this.layoutTotalPrice.Location = new System.Drawing.Point(678, 478);
            this.layoutTotalPrice.Name = "layoutTotalPrice";
            this.layoutTotalPrice.Size = new System.Drawing.Size(202, 26);
            this.layoutTotalPrice.Text = "Tổng tiền:";
            this.layoutTotalPrice.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutTotalPrice.TextSize = new System.Drawing.Size(100, 20);
            this.layoutTotalPrice.TextToControlDistance = 5;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.layoutControl2;
            this.layoutControlItem2.Location = new System.Drawing.Point(0, 478);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(339, 26);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem3.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem3.Control = this.lblDiscount;
            this.layoutControlItem3.Location = new System.Drawing.Point(339, 478);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(339, 26);
            this.layoutControlItem3.Text = "Chiết khấu:";
            this.layoutControlItem3.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem3.TextSize = new System.Drawing.Size(90, 20);
            this.layoutControlItem3.TextToControlDistance = 5;
            // 
            // lciBuyerOgranization
            // 
            this.lciBuyerOgranization.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciBuyerOgranization.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciBuyerOgranization.Control = this.txtBuyerOgranization;
            this.lciBuyerOgranization.Location = new System.Drawing.Point(0, 454);
            this.lciBuyerOgranization.Name = "lciBuyerOgranization";
            this.lciBuyerOgranization.Size = new System.Drawing.Size(220, 24);
            this.lciBuyerOgranization.Text = "Đơn vị:";
            this.lciBuyerOgranization.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciBuyerOgranization.TextSize = new System.Drawing.Size(90, 20);
            this.lciBuyerOgranization.TextToControlDistance = 5;
            // 
            // lciBuyerTaxCode
            // 
            this.lciBuyerTaxCode.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciBuyerTaxCode.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciBuyerTaxCode.Control = this.txtBuyerTaxCode;
            this.lciBuyerTaxCode.Location = new System.Drawing.Point(220, 454);
            this.lciBuyerTaxCode.Name = "lciBuyerTaxCode";
            this.lciBuyerTaxCode.Size = new System.Drawing.Size(220, 24);
            this.lciBuyerTaxCode.Text = "Mã số thuế:";
            this.lciBuyerTaxCode.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciBuyerTaxCode.TextSize = new System.Drawing.Size(90, 20);
            this.lciBuyerTaxCode.TextToControlDistance = 5;
            // 
            // lciBuyerAccountCode
            // 
            this.lciBuyerAccountCode.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciBuyerAccountCode.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciBuyerAccountCode.Control = this.txtBuyerAccountCode;
            this.lciBuyerAccountCode.Location = new System.Drawing.Point(440, 454);
            this.lciBuyerAccountCode.Name = "lciBuyerAccountCode";
            this.lciBuyerAccountCode.Size = new System.Drawing.Size(220, 24);
            this.lciBuyerAccountCode.Text = "Số tài khoản:";
            this.lciBuyerAccountCode.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciBuyerAccountCode.TextSize = new System.Drawing.Size(90, 20);
            this.lciBuyerAccountCode.TextToControlDistance = 5;
            // 
            // lciNumOrder
            // 
            this.lciNumOrder.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciNumOrder.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciNumOrder.Control = this.spinNumOrder;
            this.lciNumOrder.Location = new System.Drawing.Point(293, 504);
            this.lciNumOrder.MaxSize = new System.Drawing.Size(0, 24);
            this.lciNumOrder.MinSize = new System.Drawing.Size(130, 24);
            this.lciNumOrder.Name = "lciNumOrder";
            this.lciNumOrder.Size = new System.Drawing.Size(293, 26);
            this.lciNumOrder.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciNumOrder.Text = "Số biên lai:";
            this.lciNumOrder.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciNumOrder.TextSize = new System.Drawing.Size(90, 20);
            this.lciNumOrder.TextToControlDistance = 5;
            // 
            // layoutTransactionTime
            // 
            this.layoutTransactionTime.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.layoutTransactionTime.AppearanceItemCaption.Options.UseForeColor = true;
            this.layoutTransactionTime.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutTransactionTime.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutTransactionTime.Control = this.dtTransactionTime;
            this.layoutTransactionTime.Location = new System.Drawing.Point(293, 530);
            this.layoutTransactionTime.Name = "layoutTransactionTime";
            this.layoutTransactionTime.Size = new System.Drawing.Size(293, 26);
            this.layoutTransactionTime.Text = "Thời gian g/dịch:";
            this.layoutTransactionTime.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutTransactionTime.TextSize = new System.Drawing.Size(90, 13);
            this.layoutTransactionTime.TextToControlDistance = 5;
            // 
            // layoutControlItem10
            // 
            this.layoutControlItem10.Control = this.btnSave;
            this.layoutControlItem10.Location = new System.Drawing.Point(440, 556);
            this.layoutControlItem10.Name = "layoutControlItem10";
            this.layoutControlItem10.Size = new System.Drawing.Size(110, 26);
            this.layoutControlItem10.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem10.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.ddBtnPrint;
            this.layoutControlItem1.Location = new System.Drawing.Point(660, 556);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(110, 26);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(0, 556);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(220, 26);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // lciBuyerPhone
            // 
            this.lciBuyerPhone.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciBuyerPhone.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciBuyerPhone.Control = this.txtBuyerPhone;
            this.lciBuyerPhone.Location = new System.Drawing.Point(660, 454);
            this.lciBuyerPhone.Name = "lciBuyerPhone";
            this.lciBuyerPhone.Size = new System.Drawing.Size(220, 24);
            this.lciBuyerPhone.Text = "Số điện thoại:";
            this.lciBuyerPhone.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciBuyerPhone.TextSize = new System.Drawing.Size(90, 20);
            this.lciBuyerPhone.TextToControlDistance = 5;
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.Control = this.btnSavePrint;
            this.layoutControlItem4.Location = new System.Drawing.Point(550, 556);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Size = new System.Drawing.Size(110, 26);
            this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem4.TextVisible = false;
            // 
            // layoutControlItem13
            // 
            this.layoutControlItem13.Control = this.gridControlData;
            this.layoutControlItem13.Location = new System.Drawing.Point(0, 26);
            this.layoutControlItem13.Name = "layoutControlItem13";
            this.layoutControlItem13.Size = new System.Drawing.Size(880, 404);
            this.layoutControlItem13.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem13.TextVisible = false;
            // 
            // lciFindPatientCode
            // 
            this.lciFindPatientCode.Control = this.txtFindPatientCode;
            this.lciFindPatientCode.Location = new System.Drawing.Point(0, 0);
            this.lciFindPatientCode.Name = "lciFindPatientCode";
            this.lciFindPatientCode.Size = new System.Drawing.Size(124, 26);
            this.lciFindPatientCode.TextSize = new System.Drawing.Size(0, 0);
            this.lciFindPatientCode.TextVisible = false;
            // 
            // lciFindVaccinationCode
            // 
            this.lciFindVaccinationCode.Control = this.txtFindVaccinationCode;
            this.lciFindVaccinationCode.Location = new System.Drawing.Point(124, 0);
            this.lciFindVaccinationCode.Name = "lciFindVaccinationCode";
            this.lciFindVaccinationCode.Size = new System.Drawing.Size(124, 26);
            this.lciFindVaccinationCode.TextSize = new System.Drawing.Size(0, 0);
            this.lciFindVaccinationCode.TextVisible = false;
            // 
            // layoutControlItem9
            // 
            this.layoutControlItem9.Control = this.btnFind;
            this.layoutControlItem9.Location = new System.Drawing.Point(248, 0);
            this.layoutControlItem9.Name = "layoutControlItem9";
            this.layoutControlItem9.Size = new System.Drawing.Size(124, 26);
            this.layoutControlItem9.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem9.TextVisible = false;
            // 
            // emptySpaceItem2
            // 
            this.emptySpaceItem2.AllowHotTrack = false;
            this.emptySpaceItem2.Location = new System.Drawing.Point(372, 0);
            this.emptySpaceItem2.Name = "emptySpaceItem2";
            this.emptySpaceItem2.Size = new System.Drawing.Size(508, 26);
            this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
            // 
            // lciPatientCode
            // 
            this.lciPatientCode.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciPatientCode.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciPatientCode.Control = this.lblPatientCode;
            this.lciPatientCode.Location = new System.Drawing.Point(0, 430);
            this.lciPatientCode.Name = "lciPatientCode";
            this.lciPatientCode.Size = new System.Drawing.Size(220, 24);
            this.lciPatientCode.Text = "Mã bệnh nhân:";
            this.lciPatientCode.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciPatientCode.TextSize = new System.Drawing.Size(90, 20);
            this.lciPatientCode.TextToControlDistance = 5;
            // 
            // lciPatientName
            // 
            this.lciPatientName.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciPatientName.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciPatientName.Control = this.lblPatientName;
            this.lciPatientName.Location = new System.Drawing.Point(220, 430);
            this.lciPatientName.Name = "lciPatientName";
            this.lciPatientName.Size = new System.Drawing.Size(240, 24);
            this.lciPatientName.Text = "Tên bệnh nhân:";
            this.lciPatientName.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciPatientName.TextSize = new System.Drawing.Size(90, 20);
            this.lciPatientName.TextToControlDistance = 5;
            // 
            // lciGender
            // 
            this.lciGender.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciGender.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciGender.Control = this.lblGender;
            this.lciGender.Location = new System.Drawing.Point(460, 430);
            this.lciGender.Name = "lciGender";
            this.lciGender.Size = new System.Drawing.Size(200, 24);
            this.lciGender.Text = "Giới tính:";
            this.lciGender.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciGender.TextSize = new System.Drawing.Size(70, 20);
            this.lciGender.TextToControlDistance = 5;
            // 
            // lciDob
            // 
            this.lciDob.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciDob.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciDob.Control = this.lblDob;
            this.lciDob.Location = new System.Drawing.Point(660, 430);
            this.lciDob.Name = "lciDob";
            this.lciDob.Size = new System.Drawing.Size(220, 24);
            this.lciDob.Text = "Ngày sinh:";
            this.lciDob.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciDob.TextSize = new System.Drawing.Size(90, 20);
            this.lciDob.TextToControlDistance = 5;
            // 
            // layoutControlItem5
            // 
            this.layoutControlItem5.Control = this.btnNew;
            this.layoutControlItem5.Location = new System.Drawing.Point(770, 556);
            this.layoutControlItem5.Name = "layoutControlItem5";
            this.layoutControlItem5.Size = new System.Drawing.Size(110, 26);
            this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem5.TextVisible = false;
            // 
            // lciSaveAndSign
            // 
            this.lciSaveAndSign.Control = this.btnSaveAndSign;
            this.lciSaveAndSign.Location = new System.Drawing.Point(330, 556);
            this.lciSaveAndSign.Name = "lciSaveAndSign";
            this.lciSaveAndSign.Size = new System.Drawing.Size(110, 26);
            this.lciSaveAndSign.TextSize = new System.Drawing.Size(0, 0);
            this.lciSaveAndSign.TextVisible = false;
            // 
            // layoutControlItem12
            // 
            this.layoutControlItem12.Control = this.chkPrintEInvoice;
            this.layoutControlItem12.Location = new System.Drawing.Point(220, 556);
            this.layoutControlItem12.Name = "layoutControlItem12";
            this.layoutControlItem12.Size = new System.Drawing.Size(110, 26);
            this.layoutControlItem12.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem12.TextVisible = false;
            // 
            // dxValidationProviderEditorInfo
            // 
            this.dxValidationProviderEditorInfo.ValidationFailed += new DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventHandler(this.dxValidationProviderEditorInfo_ValidationFailed);
            // 
            // dxErrorProvider
            // 
            this.dxErrorProvider.ContainerControl = this;
            // 
            // frmMedicineVaccinBill
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(880, 611);
            this.Controls.Add(this.lciDiscount);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "frmMedicineVaccinBill";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Thanh toán yêu cầu tiêm vắc xin";
            this.Load += new System.EventHandler(this.frmMedicineVaccinBill_Load);
            this.Controls.SetChildIndex(this.barDockControlTop, 0);
            this.Controls.SetChildIndex(this.barDockControlBottom, 0);
            this.Controls.SetChildIndex(this.barDockControlRight, 0);
            this.Controls.SetChildIndex(this.barDockControlLeft, 0);
            this.Controls.SetChildIndex(this.lciDiscount, 0);
            ((System.ComponentModel.ISupportInitialize)(this.lciDiscount)).EndInit();
            this.lciDiscount.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chkPrintEInvoice.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtFindVaccinationCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtFindPatientCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBuyerPhone.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinNumOrder.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBuyerAccountCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBuyerTaxCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBuyerOgranization.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboPayFrom.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPayFormCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboCashierRoom.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCashierRoomCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDescription.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtTransactionTime.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtTransactionTime.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboAccountBook.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAccountBookCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutAccountBook)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutDescription)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutCashierRoom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutPayForm)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem11)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutTotalPrice)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBuyerOgranization)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBuyerTaxCode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBuyerAccountCode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciNumOrder)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutTransactionTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem10)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBuyerPhone)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem13)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciFindPatientCode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciFindVaccinationCode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPatientCode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPatientName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciGender)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDob)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciSaveAndSign)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem12)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProviderEditorInfo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxErrorProvider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl lciDiscount;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.GridLookUpEdit cboAccountBook;
        private DevExpress.XtraGrid.Views.Grid.GridView gridLookUpEdit1View;
        private DevExpress.XtraEditors.TextEdit txtAccountBookCode;
        private DevExpress.XtraLayout.LayoutControlItem layoutAccountBook;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem6;
        private DevExpress.XtraEditors.SimpleButton btnSave;
        private DevExpress.XtraEditors.DateEdit dtTransactionTime;
        private DevExpress.XtraLayout.LayoutControlItem layoutTransactionTime;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem10;
        private DevExpress.XtraEditors.GridLookUpEdit cboCashierRoom;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraEditors.TextEdit txtCashierRoomCode;
        private DevExpress.XtraEditors.TextEdit txtDescription;
        private DevExpress.XtraLayout.LayoutControlItem layoutDescription;
        private DevExpress.XtraLayout.LayoutControlItem layoutCashierRoom;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem8;
        private DevExpress.XtraEditors.GridLookUpEdit cboPayFrom;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView2;
        private DevExpress.XtraEditors.TextEdit txtPayFormCode;
        private DevExpress.XtraLayout.LayoutControlItem layoutPayForm;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem11;
        private DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProviderEditorInfo;
        private DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider dxErrorProvider;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.Bar bar1;
        private DevExpress.XtraBars.BarButtonItem barButtonItemSave;
        private DevExpress.XtraEditors.DropDownButton ddBtnPrint;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraEditors.LabelControl lblTotalPrice;
        private DevExpress.XtraLayout.LayoutControlItem layoutTotalPrice;
        private DevExpress.XtraLayout.LayoutControl layoutControl2;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraEditors.LabelControl lblDiscount;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraEditors.TextEdit txtBuyerAccountCode;
        private DevExpress.XtraEditors.TextEdit txtBuyerTaxCode;
        private DevExpress.XtraEditors.TextEdit txtBuyerOgranization;
        private DevExpress.XtraLayout.LayoutControlItem lciBuyerOgranization;
        private DevExpress.XtraLayout.LayoutControlItem lciBuyerTaxCode;
        private DevExpress.XtraLayout.LayoutControlItem lciBuyerAccountCode;
        private DevExpress.XtraEditors.SpinEdit spinNumOrder;
        private DevExpress.XtraLayout.LayoutControlItem lciNumOrder;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraEditors.TextEdit txtBuyerPhone;
        private DevExpress.XtraLayout.LayoutControlItem lciBuyerPhone;
        private DevExpress.XtraEditors.SimpleButton btnSavePrint;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraBars.BarButtonItem barBtnSavePrint;
        private DevExpress.XtraBars.BarButtonItem barBtnPrint;
        private DevExpress.XtraBars.BarButtonItem barBtnFind;
        private DevExpress.XtraBars.BarButtonItem barBtnFocus;
        private DevExpress.XtraBars.BarButtonItem barBtnNew;
        private DevExpress.XtraGrid.GridControl gridControlData;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewData;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn3;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn4;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn5;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn6;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn7;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn8;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem13;
        private DevExpress.XtraEditors.SimpleButton btnFind;
        private DevExpress.XtraEditors.TextEdit txtFindVaccinationCode;
        private DevExpress.XtraEditors.TextEdit txtFindPatientCode;
        private DevExpress.XtraLayout.LayoutControlItem lciFindPatientCode;
        private DevExpress.XtraLayout.LayoutControlItem lciFindVaccinationCode;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem9;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
        private DevExpress.XtraEditors.LabelControl lblDob;
        private DevExpress.XtraEditors.LabelControl lblGender;
        private DevExpress.XtraEditors.LabelControl lblPatientName;
        private DevExpress.XtraEditors.LabelControl lblPatientCode;
        private DevExpress.XtraLayout.LayoutControlItem lciPatientCode;
        private DevExpress.XtraLayout.LayoutControlItem lciPatientName;
        private DevExpress.XtraLayout.LayoutControlItem lciGender;
        private DevExpress.XtraLayout.LayoutControlItem lciDob;
        private DevExpress.XtraEditors.CheckEdit chkPrintEInvoice;
        private DevExpress.XtraEditors.SimpleButton btnSaveAndSign;
        private DevExpress.XtraEditors.SimpleButton btnNew;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
        private DevExpress.XtraLayout.LayoutControlItem lciSaveAndSign;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem12;
        private DevExpress.XtraBars.BarButtonItem barBtnSaveAndSign;
    }
}
