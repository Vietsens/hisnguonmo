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
namespace HIS.Desktop.Plugins.DepositService.DepositService
{
    partial class frmDepositService
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmDepositService));
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.btnQR = new DevExpress.XtraEditors.SimpleButton();
            this.btnSavePrintAndTrans = new DevExpress.XtraEditors.SimpleButton();
            this.txtLastDigitsOfBankCardCode = new DevExpress.XtraEditors.TextEdit();
            this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.bbtnSave = new DevExpress.XtraBars.BarButtonItem();
            this.bbtnNew = new DevExpress.XtraBars.BarButtonItem();
            this.bbtnSaveAndPrint = new DevExpress.XtraBars.BarButtonItem();
            this.bbtnSavePrintAndTrans = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.btnRefreshCardCode = new DevExpress.XtraEditors.SimpleButton();
            this.chkHidePatientPrice = new DevExpress.XtraEditors.CheckEdit();
            this.chkHideServiceBHYT = new DevExpress.XtraEditors.CheckEdit();
            this.chkHideServiceNoCost = new DevExpress.XtraEditors.CheckEdit();
            this.lblAmountReturnToPatient = new DevExpress.XtraEditors.LabelControl();
            this.btnPosConfig = new DevExpress.XtraEditors.SimpleButton();
            this.chkConnectionPOS = new DevExpress.XtraEditors.CheckEdit();
            this.chkAutoClose = new DevExpress.XtraEditors.CheckEdit();
            this.spinTransferAmount = new DevExpress.XtraEditors.SpinEdit();
            this.txtAmount = new DevExpress.XtraEditors.TextEdit();
            this.spinTongTuDen = new DevExpress.XtraEditors.SpinEdit();
            this.dtTransactionTime = new DevExpress.XtraEditors.DateEdit();
            this.btnSaveAndPrint = new DevExpress.XtraEditors.SimpleButton();
            this.panelControlTreeSereServ = new DevExpress.XtraEditors.PanelControl();
            this.cboPayForm = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.cboAccountBook = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridLookUpEdit1View = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.btnAdd = new DevExpress.XtraEditors.SimpleButton();
            this.btnSave = new DevExpress.XtraEditors.SimpleButton();
            this.ddbPrint = new DevExpress.XtraEditors.DropDownButton();
            this.txtDescription = new DevExpress.XtraEditors.TextEdit();
            this.txtTransactionCode = new DevExpress.XtraEditors.TextEdit();
            this.txtAccountBookCode = new DevExpress.XtraEditors.TextEdit();
            this.spnAmountGiveByPatient = new DevExpress.XtraEditors.SpinEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem10 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lblAccountBook = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem8 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciTransactionTime = new DevExpress.XtraLayout.LayoutControlItem();
            this.lblAmount = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciTranferAmount = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem7 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem9 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lblDescription = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutTongTuDen = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem12 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.lblTransactionCode = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem11 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem13 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem14 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem3 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.layoutControlItem15 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem16 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem17 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem18 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem19 = new DevExpress.XtraLayout.LayoutControlItem();
            this.panelControl2 = new DevExpress.XtraEditors.PanelControl();
            this.barButtonItem2 = new DevExpress.XtraBars.BarButtonItem();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.dxErrorProvider1 = new DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider(this.components);
            this.dxValidationProvider1 = new DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider(this.components);
            this.imageCollection1 = new DevExpress.Utils.ImageCollection(this.components);
            this.timerInitForm = new System.Windows.Forms.Timer(this.components);
            this.popupMenu1 = new DevExpress.XtraBars.PopupMenu(this.components);
            this.btnAddSign = new DevExpress.XtraEditors.SimpleButton();
            this.layoutControlItem20 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtLastDigitsOfBankCardCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkHidePatientPrice.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkHideServiceBHYT.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkHideServiceNoCost.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkConnectionPOS.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkAutoClose.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinTransferAmount.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAmount.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinTongTuDen.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtTransactionTime.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtTransactionTime.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControlTreeSereServ)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboPayForm.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboAccountBook.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDescription.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTransactionCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAccountBookCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spnAmountGiveByPatient.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem10)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblAccountBook)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTransactionTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblAmount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTranferAmount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblDescription)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutTongTuDen)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem12)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblTransactionCode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem11)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem13)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem14)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem15)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem16)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem17)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem18)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem19)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dxErrorProvider1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageCollection1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.popupMenu1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem20)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.btnAddSign);
            this.layoutControl1.Controls.Add(this.btnQR);
            this.layoutControl1.Controls.Add(this.btnSavePrintAndTrans);
            this.layoutControl1.Controls.Add(this.txtLastDigitsOfBankCardCode);
            this.layoutControl1.Controls.Add(this.btnRefreshCardCode);
            this.layoutControl1.Controls.Add(this.chkHidePatientPrice);
            this.layoutControl1.Controls.Add(this.chkHideServiceBHYT);
            this.layoutControl1.Controls.Add(this.chkHideServiceNoCost);
            this.layoutControl1.Controls.Add(this.lblAmountReturnToPatient);
            this.layoutControl1.Controls.Add(this.btnPosConfig);
            this.layoutControl1.Controls.Add(this.chkConnectionPOS);
            this.layoutControl1.Controls.Add(this.chkAutoClose);
            this.layoutControl1.Controls.Add(this.spinTransferAmount);
            this.layoutControl1.Controls.Add(this.txtAmount);
            this.layoutControl1.Controls.Add(this.spinTongTuDen);
            this.layoutControl1.Controls.Add(this.dtTransactionTime);
            this.layoutControl1.Controls.Add(this.btnSaveAndPrint);
            this.layoutControl1.Controls.Add(this.panelControlTreeSereServ);
            this.layoutControl1.Controls.Add(this.cboPayForm);
            this.layoutControl1.Controls.Add(this.cboAccountBook);
            this.layoutControl1.Controls.Add(this.btnAdd);
            this.layoutControl1.Controls.Add(this.btnSave);
            this.layoutControl1.Controls.Add(this.ddbPrint);
            this.layoutControl1.Controls.Add(this.txtDescription);
            this.layoutControl1.Controls.Add(this.txtTransactionCode);
            this.layoutControl1.Controls.Add(this.txtAccountBookCode);
            this.layoutControl1.Controls.Add(this.spnAmountGiveByPatient);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(266, 264, 250, 350);
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(1100, 583);
            this.layoutControl1.TabIndex = 22;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // btnQR
            // 
            this.btnQR.Location = new System.Drawing.Point(464, 559);
            this.btnQR.Name = "btnQR";
            this.btnQR.Size = new System.Drawing.Size(61, 22);
            this.btnQR.StyleController = this.layoutControl1;
            this.btnQR.TabIndex = 55;
            this.btnQR.Text = "QR";
            this.btnQR.ToolTip = "Thanh toán bằng QR";
            // 
            // btnSavePrintAndTrans
            // 
            this.btnSavePrintAndTrans.Location = new System.Drawing.Point(765, 559);
            this.btnSavePrintAndTrans.Name = "btnSavePrintAndTrans";
            this.btnSavePrintAndTrans.Size = new System.Drawing.Size(150, 22);
            this.btnSavePrintAndTrans.StyleController = this.layoutControl1;
            this.btnSavePrintAndTrans.TabIndex = 54;
            this.btnSavePrintAndTrans.Text = "Lưu In và Thanh toán (Ctrl T)";
            this.btnSavePrintAndTrans.ToolTip = "Tự động mở chức năng Thanh toán sau khi Lưu, in phiếu tạm ứng theo dịch vụ";
            this.btnSavePrintAndTrans.Click += new System.EventHandler(this.btnSavePrintAndTrans_Click);
            // 
            // txtLastDigitsOfBankCardCode
            // 
            this.txtLastDigitsOfBankCardCode.Location = new System.Drawing.Point(559, 509);
            this.txtLastDigitsOfBankCardCode.MenuManager = this.barManager1;
            this.txtLastDigitsOfBankCardCode.Name = "txtLastDigitsOfBankCardCode";
            this.txtLastDigitsOfBankCardCode.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.txtLastDigitsOfBankCardCode.Properties.MaxLength = 4;
            this.txtLastDigitsOfBankCardCode.Size = new System.Drawing.Size(76, 20);
            this.txtLastDigitsOfBankCardCode.StyleController = this.layoutControl1;
            this.txtLastDigitsOfBankCardCode.TabIndex = 53;
            this.txtLastDigitsOfBankCardCode.EditValueChanged += new System.EventHandler(this.txtLastDigitsOfBankCardCode_EditValueChanged);
            this.txtLastDigitsOfBankCardCode.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtLastDigitsOfBankCardCode_KeyPress);
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
            this.bbtnSave,
            this.bbtnNew,
            this.bbtnSaveAndPrint,
            this.bbtnSavePrintAndTrans});
            this.barManager1.MaxItemId = 4;
            // 
            // bar1
            // 
            this.bar1.BarName = "Tools";
            this.bar1.DockCol = 0;
            this.bar1.DockRow = 0;
            this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.bbtnSave),
            new DevExpress.XtraBars.LinkPersistInfo(this.bbtnNew),
            new DevExpress.XtraBars.LinkPersistInfo(this.bbtnSaveAndPrint),
            new DevExpress.XtraBars.LinkPersistInfo(this.bbtnSavePrintAndTrans)});
            this.bar1.Text = "Tools";
            this.bar1.Visible = false;
            // 
            // bbtnSave
            // 
            this.bbtnSave.Caption = "Lưu (Ctrl S)";
            this.bbtnSave.Id = 0;
            this.bbtnSave.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S));
            this.bbtnSave.Name = "bbtnSave";
            this.bbtnSave.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bbtnSave_ItemClick);
            // 
            // bbtnNew
            // 
            this.bbtnNew.Caption = "Mới (Ctrl N)";
            this.bbtnNew.Id = 1;
            this.bbtnNew.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N));
            this.bbtnNew.Name = "bbtnNew";
            this.bbtnNew.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bbtnNew_ItemClick);
            // 
            // bbtnSaveAndPrint
            // 
            this.bbtnSaveAndPrint.Caption = "Lưu và in";
            this.bbtnSaveAndPrint.Id = 2;
            this.bbtnSaveAndPrint.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.I));
            this.bbtnSaveAndPrint.Name = "bbtnSaveAndPrint";
            this.bbtnSaveAndPrint.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bbtnSaveAndPrint_ItemClick);
            // 
            // bbtnSavePrintAndTrans
            // 
            this.bbtnSavePrintAndTrans.Caption = "Lưu in và thanh toán";
            this.bbtnSavePrintAndTrans.Id = 3;
            this.bbtnSavePrintAndTrans.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.T));
            this.bbtnSavePrintAndTrans.Name = "bbtnSavePrintAndTrans";
            this.bbtnSavePrintAndTrans.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bbtnSavePrintAndTrans_ItemClick);
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Size = new System.Drawing.Size(1100, 29);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 612);
            this.barDockControlBottom.Size = new System.Drawing.Size(1100, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 29);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 583);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(1100, 29);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 583);
            // 
            // btnRefreshCardCode
            // 
            this.btnRefreshCardCode.Image = ((System.Drawing.Image)(resources.GetObject("btnRefreshCardCode.Image")));
            this.btnRefreshCardCode.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnRefreshCardCode.Location = new System.Drawing.Point(1074, 509);
            this.btnRefreshCardCode.MaximumSize = new System.Drawing.Size(25, 0);
            this.btnRefreshCardCode.Name = "btnRefreshCardCode";
            this.btnRefreshCardCode.Size = new System.Drawing.Size(24, 22);
            this.btnRefreshCardCode.StyleController = this.layoutControl1;
            this.btnRefreshCardCode.TabIndex = 52;
            this.btnRefreshCardCode.ToolTip = "Cập nhật số dư tài khoản thẻ";
            this.btnRefreshCardCode.Click += new System.EventHandler(this.btnRefreshCardCode_Click);
            // 
            // chkHidePatientPrice
            // 
            this.chkHidePatientPrice.Location = new System.Drawing.Point(617, 2);
            this.chkHidePatientPrice.MenuManager = this.barManager1;
            this.chkHidePatientPrice.Name = "chkHidePatientPrice";
            this.chkHidePatientPrice.Properties.Caption = "";
            this.chkHidePatientPrice.Properties.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
            this.chkHidePatientPrice.Size = new System.Drawing.Size(53, 19);
            this.chkHidePatientPrice.StyleController = this.layoutControl1;
            this.chkHidePatientPrice.TabIndex = 51;
            this.chkHidePatientPrice.CheckedChanged += new System.EventHandler(this.chkHidePatientPrice_CheckedChanged);
            // 
            // chkHideServiceBHYT
            // 
            this.chkHideServiceBHYT.Location = new System.Drawing.Point(107, 2);
            this.chkHideServiceBHYT.MenuManager = this.barManager1;
            this.chkHideServiceBHYT.Name = "chkHideServiceBHYT";
            this.chkHideServiceBHYT.Properties.Caption = "";
            this.chkHideServiceBHYT.Size = new System.Drawing.Size(80, 19);
            this.chkHideServiceBHYT.StyleController = this.layoutControl1;
            this.chkHideServiceBHYT.TabIndex = 50;
            this.chkHideServiceBHYT.CheckedChanged += new System.EventHandler(this.chkHideServiceBHYT_CheckedChanged);
            this.chkHideServiceBHYT.EditValueChanged += new System.EventHandler(this.chkHideServiceBHYT_EditValueChanged);
            // 
            // chkHideServiceNoCost
            // 
            this.chkHideServiceNoCost.Location = new System.Drawing.Point(323, 2);
            this.chkHideServiceNoCost.MenuManager = this.barManager1;
            this.chkHideServiceNoCost.Name = "chkHideServiceNoCost";
            this.chkHideServiceNoCost.Properties.Caption = "";
            this.chkHideServiceNoCost.Size = new System.Drawing.Size(35, 19);
            this.chkHideServiceNoCost.StyleController = this.layoutControl1;
            this.chkHideServiceNoCost.TabIndex = 49;
            this.chkHideServiceNoCost.CheckedChanged += new System.EventHandler(this.chkHideServiceNoCost_CheckedChanged);
            this.chkHideServiceNoCost.EditValueChanged += new System.EventHandler(this.chkHideServiceNoCost_EditValueChanged);
            // 
            // lblAmountReturnToPatient
            // 
            this.lblAmountReturnToPatient.Appearance.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAmountReturnToPatient.Appearance.ForeColor = System.Drawing.Color.Blue;
            this.lblAmountReturnToPatient.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblAmountReturnToPatient.Location = new System.Drawing.Point(1014, 509);
            this.lblAmountReturnToPatient.Name = "lblAmountReturnToPatient";
            this.lblAmountReturnToPatient.Size = new System.Drawing.Size(56, 20);
            this.lblAmountReturnToPatient.StyleController = this.layoutControl1;
            this.lblAmountReturnToPatient.TabIndex = 48;
            this.lblAmountReturnToPatient.ToolTip = "Số tiền cần trả lại bệnh nhân";
            // 
            // btnPosConfig
            // 
            this.btnPosConfig.Image = ((System.Drawing.Image)(resources.GetObject("btnPosConfig.Image")));
            this.btnPosConfig.Location = new System.Drawing.Point(314, 559);
            this.btnPosConfig.Name = "btnPosConfig";
            this.btnPosConfig.Size = new System.Drawing.Size(24, 22);
            this.btnPosConfig.StyleController = this.layoutControl1;
            this.btnPosConfig.TabIndex = 46;
            this.btnPosConfig.ToolTip = "Cấu hình kết nối thiết bị POS";
            this.btnPosConfig.Click += new System.EventHandler(this.btnPosConfig_Click);
            // 
            // chkConnectionPOS
            // 
            this.chkConnectionPOS.Location = new System.Drawing.Point(276, 559);
            this.chkConnectionPOS.MenuManager = this.barManager1;
            this.chkConnectionPOS.Name = "chkConnectionPOS";
            this.chkConnectionPOS.Properties.Caption = "";
            this.chkConnectionPOS.Properties.FullFocusRect = true;
            this.chkConnectionPOS.Properties.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
            this.chkConnectionPOS.Size = new System.Drawing.Size(34, 19);
            this.chkConnectionPOS.StyleController = this.layoutControl1;
            this.chkConnectionPOS.TabIndex = 45;
            this.chkConnectionPOS.CheckedChanged += new System.EventHandler(this.chkConnectionPOS_CheckedChanged);
            // 
            // chkAutoClose
            // 
            this.chkAutoClose.Location = new System.Drawing.Point(342, 559);
            this.chkAutoClose.MenuManager = this.barManager1;
            this.chkAutoClose.Name = "chkAutoClose";
            this.chkAutoClose.Properties.Appearance.Options.UseTextOptions = true;
            this.chkAutoClose.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.chkAutoClose.Properties.Caption = "     :Tự động đóng";
            this.chkAutoClose.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.chkAutoClose.Size = new System.Drawing.Size(118, 19);
            this.chkAutoClose.StyleController = this.layoutControl1;
            this.chkAutoClose.TabIndex = 44;
            this.chkAutoClose.ToolTip = "Tự động đóng chức năng khi \'Lưu\' hoặc \'Lưu in\' thành công";
            this.chkAutoClose.CheckedChanged += new System.EventHandler(this.chkAutoClose_CheckedChanged);
            // 
            // spinTransferAmount
            // 
            this.spinTransferAmount.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.spinTransferAmount.Location = new System.Drawing.Point(358, 509);
            this.spinTransferAmount.MenuManager = this.barManager1;
            this.spinTransferAmount.Name = "spinTransferAmount";
            this.spinTransferAmount.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.spinTransferAmount.Size = new System.Drawing.Size(102, 20);
            this.spinTransferAmount.StyleController = this.layoutControl1;
            this.spinTransferAmount.TabIndex = 43;
            this.spinTransferAmount.EditValueChanged += new System.EventHandler(this.spinTransferAmount_EditValueChanged);
            this.spinTransferAmount.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.spinTransferAmount_PreviewKeyDown);
            // 
            // txtAmount
            // 
            this.txtAmount.EditValue = "";
            this.txtAmount.Location = new System.Drawing.Point(97, 509);
            this.txtAmount.MenuManager = this.barManager1;
            this.txtAmount.Name = "txtAmount";
            this.txtAmount.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.txtAmount.Properties.Appearance.ForeColor = System.Drawing.Color.Red;
            this.txtAmount.Properties.Appearance.Options.UseFont = true;
            this.txtAmount.Properties.Appearance.Options.UseForeColor = true;
            this.txtAmount.Properties.Appearance.Options.UseTextOptions = true;
            this.txtAmount.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.txtAmount.Properties.ReadOnly = true;
            this.txtAmount.Size = new System.Drawing.Size(162, 20);
            this.txtAmount.StyleController = this.layoutControl1;
            this.txtAmount.TabIndex = 42;
            this.txtAmount.EditValueChanged += new System.EventHandler(this.txtAmount_EditValueChanged);
            // 
            // spinTongTuDen
            // 
            this.spinTongTuDen.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.spinTongTuDen.Location = new System.Drawing.Point(1014, 485);
            this.spinTongTuDen.MenuManager = this.barManager1;
            this.spinTongTuDen.Name = "spinTongTuDen";
            this.spinTongTuDen.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.spinTongTuDen.Size = new System.Drawing.Size(84, 20);
            this.spinTongTuDen.StyleController = this.layoutControl1;
            this.spinTongTuDen.TabIndex = 41;
            this.spinTongTuDen.Spin += new DevExpress.XtraEditors.Controls.SpinEventHandler(this.spinTongTuDen_Spin);
            this.spinTongTuDen.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.spinTongTuDen_PreviewKeyDown);
            // 
            // dtTransactionTime
            // 
            this.dtTransactionTime.EditValue = null;
            this.dtTransactionTime.Location = new System.Drawing.Point(559, 485);
            this.dtTransactionTime.MenuManager = this.barManager1;
            this.dtTransactionTime.Name = "dtTransactionTime";
            this.dtTransactionTime.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtTransactionTime.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtTransactionTime.Properties.DisplayFormat.FormatString = "dd/MM/yyyy HH:mm";
            this.dtTransactionTime.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtTransactionTime.Properties.EditFormat.FormatString = "dd/MM/yyyy HH:mm";
            this.dtTransactionTime.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtTransactionTime.Properties.Mask.EditMask = "dd/MM/yyyy HH:mm";
            this.dtTransactionTime.Properties.VistaCalendarViewStyle = DevExpress.XtraEditors.VistaCalendarViewStyle.MonthView;
            this.dtTransactionTime.Size = new System.Drawing.Size(356, 20);
            this.dtTransactionTime.StyleController = this.layoutControl1;
            this.dtTransactionTime.TabIndex = 7;
            this.dtTransactionTime.ToolTip = "Thời gian giao dịch";
            this.dtTransactionTime.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.dtTransactionTime_Closed);
            this.dtTransactionTime.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.dtTransactionTime_PreviewKeyDown);
            // 
            // btnSaveAndPrint
            // 
            this.btnSaveAndPrint.Location = new System.Drawing.Point(686, 559);
            this.btnSaveAndPrint.Name = "btnSaveAndPrint";
            this.btnSaveAndPrint.Size = new System.Drawing.Size(75, 22);
            this.btnSaveAndPrint.StyleController = this.layoutControl1;
            this.btnSaveAndPrint.TabIndex = 10;
            this.btnSaveAndPrint.Text = "Lưu In(Ctrl I)";
            this.btnSaveAndPrint.Click += new System.EventHandler(this.btnSaveAndPrint_Click);
            // 
            // panelControlTreeSereServ
            // 
            this.panelControlTreeSereServ.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.panelControlTreeSereServ.Location = new System.Drawing.Point(2, 26);
            this.panelControlTreeSereServ.Name = "panelControlTreeSereServ";
            this.panelControlTreeSereServ.Size = new System.Drawing.Size(1096, 455);
            this.panelControlTreeSereServ.TabIndex = 40;
            // 
            // cboPayForm
            // 
            this.cboPayForm.Location = new System.Drawing.Point(358, 485);
            this.cboPayForm.Name = "cboPayForm";
            this.cboPayForm.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboPayForm.Properties.NullText = "";
            this.cboPayForm.Properties.View = this.gridView1;
            this.cboPayForm.Size = new System.Drawing.Size(102, 20);
            this.cboPayForm.StyleController = this.layoutControl1;
            this.cboPayForm.TabIndex = 6;
            this.cboPayForm.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboPayForm_Closed);
            this.cboPayForm.KeyUp += new System.Windows.Forms.KeyEventHandler(this.cboPayForm_KeyUp);
            // 
            // gridView1
            // 
            this.gridView1.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridView1.Name = "gridView1";
            this.gridView1.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridView1.OptionsView.ShowGroupPanel = false;
            // 
            // cboAccountBook
            // 
            this.cboAccountBook.EditValue = "";
            this.cboAccountBook.Location = new System.Drawing.Point(179, 485);
            this.cboAccountBook.Name = "cboAccountBook";
            this.cboAccountBook.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboAccountBook.Properties.NullText = "";
            this.cboAccountBook.Properties.View = this.gridLookUpEdit1View;
            this.cboAccountBook.Size = new System.Drawing.Size(80, 20);
            this.cboAccountBook.StyleController = this.layoutControl1;
            this.cboAccountBook.TabIndex = 4;
            this.cboAccountBook.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboAccountBook_Closed);
            this.cboAccountBook.KeyUp += new System.Windows.Forms.KeyEventHandler(this.cboAccountBook_KeyUp);
            // 
            // gridLookUpEdit1View
            // 
            this.gridLookUpEdit1View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridLookUpEdit1View.Name = "gridLookUpEdit1View";
            this.gridLookUpEdit1View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridLookUpEdit1View.OptionsView.ShowGroupPanel = false;
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(1014, 559);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(84, 22);
            this.btnAdd.StyleController = this.layoutControl1;
            this.btnAdd.TabIndex = 12;
            this.btnAdd.Text = "Mới (Ctrl N)";
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(609, 559);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(73, 22);
            this.btnSave.StyleController = this.layoutControl1;
            this.btnSave.TabIndex = 9;
            this.btnSave.Text = "Lưu (Ctrl S)";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // ddbPrint
            // 
            this.ddbPrint.Location = new System.Drawing.Point(919, 559);
            this.ddbPrint.Name = "ddbPrint";
            this.ddbPrint.Size = new System.Drawing.Size(91, 22);
            this.ddbPrint.StyleController = this.layoutControl1;
            this.ddbPrint.TabIndex = 11;
            this.ddbPrint.Text = "In ấn";
            this.ddbPrint.Click += new System.EventHandler(this.ddbPrint_Click);
            // 
            // txtDescription
            // 
            this.txtDescription.Location = new System.Drawing.Point(97, 535);
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Properties.MaxLength = 2000;
            this.txtDescription.Size = new System.Drawing.Size(363, 20);
            this.txtDescription.StyleController = this.layoutControl1;
            this.txtDescription.TabIndex = 8;
            this.txtDescription.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtDescription_KeyDown);
            this.txtDescription.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtDescription_PreviewKeyDown);
            // 
            // txtTransactionCode
            // 
            this.txtTransactionCode.Location = new System.Drawing.Point(559, 535);
            this.txtTransactionCode.Name = "txtTransactionCode";
            this.txtTransactionCode.Properties.ReadOnly = true;
            this.txtTransactionCode.Size = new System.Drawing.Size(356, 20);
            this.txtTransactionCode.StyleController = this.layoutControl1;
            this.txtTransactionCode.TabIndex = 38;
            // 
            // txtAccountBookCode
            // 
            this.txtAccountBookCode.EditValue = "";
            this.txtAccountBookCode.Location = new System.Drawing.Point(97, 485);
            this.txtAccountBookCode.Name = "txtAccountBookCode";
            this.txtAccountBookCode.Properties.MaxLength = 6;
            this.txtAccountBookCode.Size = new System.Drawing.Size(82, 20);
            this.txtAccountBookCode.StyleController = this.layoutControl1;
            this.txtAccountBookCode.TabIndex = 3;
            this.txtAccountBookCode.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtAccountBookCode_PreviewKeyDown);
            // 
            // spnAmountGiveByPatient
            // 
            this.spnAmountGiveByPatient.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.spnAmountGiveByPatient.Location = new System.Drawing.Point(734, 509);
            this.spnAmountGiveByPatient.MenuManager = this.barManager1;
            this.spnAmountGiveByPatient.Name = "spnAmountGiveByPatient";
            this.spnAmountGiveByPatient.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.spnAmountGiveByPatient.Properties.Mask.EditMask = "n0";
            this.spnAmountGiveByPatient.Properties.Mask.UseMaskAsDisplayFormat = true;
            this.spnAmountGiveByPatient.Properties.MaxValue = new decimal(new int[] {
            -1530494977,
            232830,
            0,
            0});
            this.spnAmountGiveByPatient.Size = new System.Drawing.Size(181, 20);
            this.spnAmountGiveByPatient.StyleController = this.layoutControl1;
            this.spnAmountGiveByPatient.TabIndex = 47;
            this.spnAmountGiveByPatient.ToolTip = "Số tiền bệnh nhân đưa";
            this.spnAmountGiveByPatient.EditValueChanged += new System.EventHandler(this.spnAmountGiveByPatient_EditValueChanged);
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem10,
            this.emptySpaceItem1,
            this.layoutControlItem2,
            this.layoutControlItem3,
            this.layoutControlItem4,
            this.layoutControlItem5,
            this.lblAccountBook,
            this.layoutControlItem8,
            this.layoutControlItem1,
            this.lciTransactionTime,
            this.lblAmount,
            this.lciTranferAmount,
            this.layoutControlItem6,
            this.layoutControlItem7,
            this.layoutControlItem9,
            this.lblDescription,
            this.layoutTongTuDen,
            this.layoutControlItem12,
            this.emptySpaceItem2,
            this.lblTransactionCode,
            this.layoutControlItem11,
            this.layoutControlItem13,
            this.layoutControlItem14,
            this.emptySpaceItem3,
            this.layoutControlItem15,
            this.layoutControlItem16,
            this.layoutControlItem17,
            this.layoutControlItem18,
            this.layoutControlItem19,
            this.layoutControlItem20});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "Root";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(1100, 583);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem10
            // 
            this.layoutControlItem10.Control = this.ddbPrint;
            this.layoutControlItem10.Location = new System.Drawing.Point(917, 557);
            this.layoutControlItem10.MaxSize = new System.Drawing.Size(0, 26);
            this.layoutControlItem10.MinSize = new System.Drawing.Size(48, 26);
            this.layoutControlItem10.Name = "layoutControlItem10";
            this.layoutControlItem10.Size = new System.Drawing.Size(95, 26);
            this.layoutControlItem10.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem10.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem10.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem10.TextToControlDistance = 0;
            this.layoutControlItem10.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(0, 557);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(189, 26);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.btnSave;
            this.layoutControlItem2.Location = new System.Drawing.Point(607, 557);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(77, 26);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.btnAdd;
            this.layoutControlItem3.Location = new System.Drawing.Point(1012, 557);
            this.layoutControlItem3.MaxSize = new System.Drawing.Size(0, 26);
            this.layoutControlItem3.MinSize = new System.Drawing.Size(67, 26);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(88, 26);
            this.layoutControlItem3.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem3.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextToControlDistance = 0;
            this.layoutControlItem3.TextVisible = false;
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.Control = this.cboAccountBook;
            this.layoutControlItem4.Location = new System.Drawing.Point(179, 483);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 2, 2, 2);
            this.layoutControlItem4.Size = new System.Drawing.Size(82, 24);
            this.layoutControlItem4.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem4.TextToControlDistance = 0;
            this.layoutControlItem4.TextVisible = false;
            // 
            // layoutControlItem5
            // 
            this.layoutControlItem5.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.layoutControlItem5.AppearanceItemCaption.Options.UseForeColor = true;
            this.layoutControlItem5.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem5.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem5.Control = this.cboPayForm;
            this.layoutControlItem5.Location = new System.Drawing.Point(261, 483);
            this.layoutControlItem5.Name = "layoutControlItem5";
            this.layoutControlItem5.Size = new System.Drawing.Size(201, 24);
            this.layoutControlItem5.Text = "Hình thức:";
            this.layoutControlItem5.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem5.TextSize = new System.Drawing.Size(90, 20);
            this.layoutControlItem5.TextToControlDistance = 5;
            // 
            // lblAccountBook
            // 
            this.lblAccountBook.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.lblAccountBook.AppearanceItemCaption.Options.UseForeColor = true;
            this.lblAccountBook.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lblAccountBook.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lblAccountBook.Control = this.txtAccountBookCode;
            this.lblAccountBook.Location = new System.Drawing.Point(0, 483);
            this.lblAccountBook.MaxSize = new System.Drawing.Size(0, 24);
            this.lblAccountBook.MinSize = new System.Drawing.Size(127, 24);
            this.lblAccountBook.Name = "lblAccountBook";
            this.lblAccountBook.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 0, 2, 2);
            this.lblAccountBook.Size = new System.Drawing.Size(179, 24);
            this.lblAccountBook.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lblAccountBook.Text = "Sổ :";
            this.lblAccountBook.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lblAccountBook.TextSize = new System.Drawing.Size(90, 13);
            this.lblAccountBook.TextToControlDistance = 5;
            // 
            // layoutControlItem8
            // 
            this.layoutControlItem8.Control = this.panelControlTreeSereServ;
            this.layoutControlItem8.Location = new System.Drawing.Point(0, 24);
            this.layoutControlItem8.Name = "layoutControlItem8";
            this.layoutControlItem8.Size = new System.Drawing.Size(1100, 459);
            this.layoutControlItem8.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem8.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem8.TextToControlDistance = 0;
            this.layoutControlItem8.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.btnSaveAndPrint;
            this.layoutControlItem1.Location = new System.Drawing.Point(684, 557);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(79, 26);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // lciTransactionTime
            // 
            this.lciTransactionTime.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.lciTransactionTime.AppearanceItemCaption.Options.UseForeColor = true;
            this.lciTransactionTime.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciTransactionTime.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciTransactionTime.Control = this.dtTransactionTime;
            this.lciTransactionTime.Location = new System.Drawing.Point(462, 483);
            this.lciTransactionTime.Name = "lciTransactionTime";
            this.lciTransactionTime.OptionsToolTip.ToolTip = "Thời gian giao dịch";
            this.lciTransactionTime.Size = new System.Drawing.Size(455, 24);
            this.lciTransactionTime.Text = "TG giao dịch:";
            this.lciTransactionTime.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciTransactionTime.TextSize = new System.Drawing.Size(90, 20);
            this.lciTransactionTime.TextToControlDistance = 5;
            // 
            // lblAmount
            // 
            this.lblAmount.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lblAmount.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lblAmount.Control = this.txtAmount;
            this.lblAmount.Location = new System.Drawing.Point(0, 507);
            this.lblAmount.Name = "lblAmount";
            this.lblAmount.Size = new System.Drawing.Size(261, 26);
            this.lblAmount.Text = "Số tiền:";
            this.lblAmount.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lblAmount.TextSize = new System.Drawing.Size(90, 20);
            this.lblAmount.TextToControlDistance = 5;
            // 
            // lciTranferAmount
            // 
            this.lciTranferAmount.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciTranferAmount.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciTranferAmount.Control = this.spinTransferAmount;
            this.lciTranferAmount.Location = new System.Drawing.Point(261, 507);
            this.lciTranferAmount.Name = "lciTranferAmount";
            this.lciTranferAmount.OptionsToolTip.ToolTip = "Số tiền chuyển khoản";
            this.lciTranferAmount.Size = new System.Drawing.Size(201, 26);
            this.lciTranferAmount.Text = "Số tiền CK:";
            this.lciTranferAmount.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciTranferAmount.TextSize = new System.Drawing.Size(90, 20);
            this.lciTranferAmount.TextToControlDistance = 5;
            // 
            // layoutControlItem6
            // 
            this.layoutControlItem6.Control = this.chkAutoClose;
            this.layoutControlItem6.Location = new System.Drawing.Point(340, 557);
            this.layoutControlItem6.Name = "layoutControlItem6";
            this.layoutControlItem6.Size = new System.Drawing.Size(122, 26);
            this.layoutControlItem6.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem6.TextVisible = false;
            // 
            // layoutControlItem7
            // 
            this.layoutControlItem7.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem7.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem7.Control = this.chkConnectionPOS;
            this.layoutControlItem7.Location = new System.Drawing.Point(189, 557);
            this.layoutControlItem7.Name = "layoutControlItem7";
            this.layoutControlItem7.OptionsToolTip.ToolTip = "Kết nối đến POS thanh toán của ngân hàng khi chọn hình thức thanh toán là \"Quẹt t" +
    "hẻ ngân hàng\" hoặc \"Tiền mặt/Quẹt thẻ\"";
            this.layoutControlItem7.Size = new System.Drawing.Size(123, 26);
            this.layoutControlItem7.Text = "Kết nối POS:";
            this.layoutControlItem7.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem7.TextSize = new System.Drawing.Size(80, 20);
            this.layoutControlItem7.TextToControlDistance = 5;
            // 
            // layoutControlItem9
            // 
            this.layoutControlItem9.Control = this.btnPosConfig;
            this.layoutControlItem9.Location = new System.Drawing.Point(312, 557);
            this.layoutControlItem9.Name = "layoutControlItem9";
            this.layoutControlItem9.Size = new System.Drawing.Size(28, 26);
            this.layoutControlItem9.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem9.TextVisible = false;
            // 
            // lblDescription
            // 
            this.lblDescription.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lblDescription.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lblDescription.Control = this.txtDescription;
            this.lblDescription.Location = new System.Drawing.Point(0, 533);
            this.lblDescription.MaxSize = new System.Drawing.Size(0, 24);
            this.lblDescription.MinSize = new System.Drawing.Size(129, 24);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(462, 24);
            this.lblDescription.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lblDescription.Text = "Mô tả :";
            this.lblDescription.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lblDescription.TextSize = new System.Drawing.Size(90, 13);
            this.lblDescription.TextToControlDistance = 5;
            // 
            // layoutTongTuDen
            // 
            this.layoutTongTuDen.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutTongTuDen.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutTongTuDen.Control = this.spinTongTuDen;
            this.layoutTongTuDen.Location = new System.Drawing.Point(917, 483);
            this.layoutTongTuDen.Name = "layoutTongTuDen";
            this.layoutTongTuDen.Size = new System.Drawing.Size(183, 24);
            this.layoutTongTuDen.Text = "Số chứng từ:";
            this.layoutTongTuDen.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutTongTuDen.TextSize = new System.Drawing.Size(90, 20);
            this.layoutTongTuDen.TextToControlDistance = 5;
            // 
            // layoutControlItem12
            // 
            this.layoutControlItem12.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.layoutControlItem12.AppearanceItemCaption.Options.UseFont = true;
            this.layoutControlItem12.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem12.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem12.Control = this.lblAmountReturnToPatient;
            this.layoutControlItem12.Location = new System.Drawing.Point(917, 507);
            this.layoutControlItem12.Name = "layoutControlItem12";
            this.layoutControlItem12.OptionsToolTip.ToolTip = "Số tiền cần trả lại bệnh nhân";
            this.layoutControlItem12.Size = new System.Drawing.Size(155, 26);
            this.layoutControlItem12.Text = "Số tiền trả BN:";
            this.layoutControlItem12.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem12.TextSize = new System.Drawing.Size(90, 20);
            this.layoutControlItem12.TextToControlDistance = 5;
            // 
            // emptySpaceItem2
            // 
            this.emptySpaceItem2.AllowHotTrack = false;
            this.emptySpaceItem2.Location = new System.Drawing.Point(917, 533);
            this.emptySpaceItem2.Name = "emptySpaceItem2";
            this.emptySpaceItem2.Size = new System.Drawing.Size(183, 24);
            this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
            // 
            // lblTransactionCode
            // 
            this.lblTransactionCode.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lblTransactionCode.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lblTransactionCode.Control = this.txtTransactionCode;
            this.lblTransactionCode.Location = new System.Drawing.Point(462, 533);
            this.lblTransactionCode.MaxSize = new System.Drawing.Size(0, 24);
            this.lblTransactionCode.MinSize = new System.Drawing.Size(129, 24);
            this.lblTransactionCode.Name = "lblTransactionCode";
            this.lblTransactionCode.Size = new System.Drawing.Size(455, 24);
            this.lblTransactionCode.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lblTransactionCode.Text = "Mã giao dịch :";
            this.lblTransactionCode.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lblTransactionCode.TextSize = new System.Drawing.Size(90, 13);
            this.lblTransactionCode.TextToControlDistance = 5;
            // 
            // layoutControlItem11
            // 
            this.layoutControlItem11.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.layoutControlItem11.AppearanceItemCaption.Options.UseFont = true;
            this.layoutControlItem11.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem11.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem11.Control = this.spnAmountGiveByPatient;
            this.layoutControlItem11.Location = new System.Drawing.Point(637, 507);
            this.layoutControlItem11.Name = "layoutControlItem11";
            this.layoutControlItem11.OptionsToolTip.ToolTip = "Số tiền bệnh nhân đưa";
            this.layoutControlItem11.Size = new System.Drawing.Size(280, 26);
            this.layoutControlItem11.Text = "Số tiền BN đưa:";
            this.layoutControlItem11.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem11.TextSize = new System.Drawing.Size(90, 20);
            this.layoutControlItem11.TextToControlDistance = 5;
            // 
            // layoutControlItem13
            // 
            this.layoutControlItem13.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem13.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem13.Control = this.chkHideServiceNoCost;
            this.layoutControlItem13.Location = new System.Drawing.Point(189, 0);
            this.layoutControlItem13.Name = "layoutControlItem13";
            this.layoutControlItem13.OptionsToolTip.ToolTip = "Không hiển thị các dịch vụ có giá 0đ";
            this.layoutControlItem13.Size = new System.Drawing.Size(171, 24);
            this.layoutControlItem13.Text = "Ẩn dịch vụ không có chi phí";
            this.layoutControlItem13.TextSize = new System.Drawing.Size(129, 13);
            // 
            // layoutControlItem14
            // 
            this.layoutControlItem14.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem14.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem14.Control = this.chkHideServiceBHYT;
            this.layoutControlItem14.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem14.Name = "layoutControlItem14";
            this.layoutControlItem14.OptionsToolTip.ToolTip = "Không hiển thị các dịch vụ có số tiền BHYT chi trả";
            this.layoutControlItem14.Size = new System.Drawing.Size(189, 24);
            this.layoutControlItem14.Text = "Ẩn dịch vụ BHYT";
            this.layoutControlItem14.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem14.TextSize = new System.Drawing.Size(100, 20);
            this.layoutControlItem14.TextToControlDistance = 5;
            // 
            // emptySpaceItem3
            // 
            this.emptySpaceItem3.AllowHotTrack = false;
            this.emptySpaceItem3.Location = new System.Drawing.Point(672, 0);
            this.emptySpaceItem3.Name = "emptySpaceItem3";
            this.emptySpaceItem3.Size = new System.Drawing.Size(428, 24);
            this.emptySpaceItem3.TextSize = new System.Drawing.Size(0, 0);
            // 
            // layoutControlItem15
            // 
            this.layoutControlItem15.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem15.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem15.Control = this.chkHidePatientPrice;
            this.layoutControlItem15.Location = new System.Drawing.Point(360, 0);
            this.layoutControlItem15.Name = "layoutControlItem15";
            this.layoutControlItem15.Size = new System.Drawing.Size(312, 24);
            this.layoutControlItem15.Text = "Ẩn dịch vụ không có chi phí bệnh nhân trả";
            this.layoutControlItem15.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem15.TextSize = new System.Drawing.Size(250, 20);
            this.layoutControlItem15.TextToControlDistance = 5;
            // 
            // layoutControlItem16
            // 
            this.layoutControlItem16.Control = this.btnRefreshCardCode;
            this.layoutControlItem16.Location = new System.Drawing.Point(1072, 507);
            this.layoutControlItem16.Name = "layoutControlItem16";
            this.layoutControlItem16.Size = new System.Drawing.Size(28, 26);
            this.layoutControlItem16.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem16.TextVisible = false;
            // 
            // layoutControlItem17
            // 
            this.layoutControlItem17.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.layoutControlItem17.AppearanceItemCaption.Options.UseForeColor = true;
            this.layoutControlItem17.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem17.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem17.Control = this.txtLastDigitsOfBankCardCode;
            this.layoutControlItem17.Location = new System.Drawing.Point(462, 507);
            this.layoutControlItem17.Name = "layoutControlItem17";
            this.layoutControlItem17.Size = new System.Drawing.Size(175, 26);
            this.layoutControlItem17.Text = "4 số cuối thẻ NH:";
            this.layoutControlItem17.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem17.TextSize = new System.Drawing.Size(90, 20);
            this.layoutControlItem17.TextToControlDistance = 5;
            // 
            // layoutControlItem18
            // 
            this.layoutControlItem18.Control = this.btnSavePrintAndTrans;
            this.layoutControlItem18.Location = new System.Drawing.Point(763, 557);
            this.layoutControlItem18.Name = "layoutControlItem18";
            this.layoutControlItem18.Size = new System.Drawing.Size(154, 26);
            this.layoutControlItem18.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem18.TextVisible = false;
            // 
            // layoutControlItem19
            // 
            this.layoutControlItem19.Control = this.btnQR;
            this.layoutControlItem19.Location = new System.Drawing.Point(462, 557);
            this.layoutControlItem19.Name = "layoutControlItem19";
            this.layoutControlItem19.Size = new System.Drawing.Size(65, 26);
            this.layoutControlItem19.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem19.TextVisible = false;
            // 
            // panelControl2
            // 
            this.panelControl2.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.panelControl2.Location = new System.Drawing.Point(13, 79);
            this.panelControl2.Name = "panelControl2";
            this.panelControl2.Size = new System.Drawing.Size(416, 38);
            this.panelControl2.TabIndex = 17;
            // 
            // barButtonItem2
            // 
            this.barButtonItem2.Id = -1;
            this.barButtonItem2.Name = "barButtonItem2";
            // 
            // panelControl1
            // 
            this.panelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.panelControl1.Controls.Add(this.layoutControl1);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl1.Location = new System.Drawing.Point(0, 29);
            this.panelControl1.Margin = new System.Windows.Forms.Padding(0);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(1100, 583);
            this.panelControl1.TabIndex = 2;
            // 
            // dxErrorProvider1
            // 
            this.dxErrorProvider1.ContainerControl = this;
            // 
            // dxValidationProvider1
            // 
            this.dxValidationProvider1.ValidationFailed += new DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventHandler(this.dxValidationProvider1_ValidationFailed);
            // 
            // imageCollection1
            // 
            this.imageCollection1.ImageStream = ((DevExpress.Utils.ImageCollectionStreamer)(resources.GetObject("imageCollection1.ImageStream")));
            // 
            // popupMenu1
            // 
            this.popupMenu1.Manager = this.barManager1;
            this.popupMenu1.Name = "popupMenu1";
            // 
            // btnAddSign
            // 
            this.btnAddSign.Location = new System.Drawing.Point(529, 559);
            this.btnAddSign.Name = "btnAddSign";
            this.btnAddSign.Size = new System.Drawing.Size(76, 22);
            this.btnAddSign.StyleController = this.layoutControl1;
            this.btnAddSign.TabIndex = 56;
            this.btnAddSign.Text = "Lưu ký";
            this.btnAddSign.Click += new System.EventHandler(this.btnAddSign_Click);
            // 
            // layoutControlItem20
            // 
            this.layoutControlItem20.Control = this.btnAddSign;
            this.layoutControlItem20.Location = new System.Drawing.Point(527, 557);
            this.layoutControlItem20.Name = "layoutControlItem20";
            this.layoutControlItem20.Size = new System.Drawing.Size(80, 26);
            this.layoutControlItem20.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem20.TextVisible = false;
            // 
            // frmDepositService
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1100, 612);
            this.Controls.Add(this.panelControl1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmDepositService";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Tạm ứng theo dịch vụ";
            this.Load += new System.EventHandler(this.frmDepositService_Load);
            this.Controls.SetChildIndex(this.barDockControlTop, 0);
            this.Controls.SetChildIndex(this.barDockControlBottom, 0);
            this.Controls.SetChildIndex(this.barDockControlRight, 0);
            this.Controls.SetChildIndex(this.barDockControlLeft, 0);
            this.Controls.SetChildIndex(this.panelControl1, 0);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtLastDigitsOfBankCardCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkHidePatientPrice.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkHideServiceBHYT.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkHideServiceNoCost.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkConnectionPOS.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkAutoClose.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinTransferAmount.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAmount.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinTongTuDen.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtTransactionTime.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtTransactionTime.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControlTreeSereServ)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboPayForm.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboAccountBook.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDescription.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTransactionCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAccountBookCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spnAmountGiveByPatient.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem10)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblAccountBook)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTransactionTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblAmount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTranferAmount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblDescription)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutTongTuDen)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem12)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblTransactionCode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem11)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem13)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem14)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem15)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem16)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem17)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem18)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem19)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dxErrorProvider1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageCollection1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.popupMenu1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem20)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraBars.BarButtonItem barButtonItem2;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        internal DevExpress.XtraEditors.TextEdit txtAccountBookCode;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraLayout.LayoutControlItem lblAccountBook;
        internal DevExpress.XtraLayout.LayoutControl layoutControl1;
        internal DevExpress.XtraEditors.TextEdit txtTransactionCode;
        private DevExpress.XtraLayout.LayoutControlItem lblTransactionCode;
        private DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider dxErrorProvider1;
        private DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProvider1;
        private DevExpress.XtraLayout.LayoutControlItem lblDescription;
        internal DevExpress.XtraEditors.TextEdit txtDescription;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem10;
        internal DevExpress.XtraEditors.DropDownButton ddbPrint;
        private DevExpress.XtraGrid.Columns.GridColumn grdColFundCode;
        private DevExpress.XtraGrid.Columns.GridColumn grdColFundName;
        private DevExpress.XtraGrid.Columns.GridColumn grdColBillFundAmount;
        private DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit repositoryItemGridFund;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraEditors.PanelControl panelControl2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        internal DevExpress.XtraEditors.SimpleButton btnAdd;
        internal DevExpress.XtraEditors.SimpleButton btnSave;
        private DevExpress.XtraGrid.Views.Grid.GridView gridLookUpEdit1View;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
        internal DevExpress.XtraEditors.GridLookUpEdit cboAccountBook;
        internal DevExpress.XtraEditors.GridLookUpEdit cboPayForm;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.Bar bar1;
        private DevExpress.XtraBars.BarButtonItem bbtnSave;
        private DevExpress.XtraBars.BarButtonItem bbtnNew;
        private DevExpress.Utils.ImageCollection imageCollection1;
        private DevExpress.XtraEditors.PanelControl panelControlTreeSereServ;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem8;
        private DevExpress.XtraEditors.SimpleButton btnSaveAndPrint;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraBars.BarButtonItem bbtnSaveAndPrint;
        private DevExpress.XtraEditors.DateEdit dtTransactionTime;
        private DevExpress.XtraLayout.LayoutControlItem lciTransactionTime;
        private DevExpress.XtraEditors.SpinEdit spinTongTuDen;
        private DevExpress.XtraLayout.LayoutControlItem layoutTongTuDen;
        private DevExpress.XtraEditors.TextEdit txtAmount;
        private DevExpress.XtraLayout.LayoutControlItem lblAmount;
        private System.Windows.Forms.Timer timerInitForm;
        private DevExpress.XtraEditors.SpinEdit spinTransferAmount;
        private DevExpress.XtraLayout.LayoutControlItem lciTranferAmount;
        private DevExpress.XtraEditors.CheckEdit chkAutoClose;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem6;
        private DevExpress.XtraEditors.CheckEdit chkConnectionPOS;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem7;
        private DevExpress.XtraEditors.SimpleButton btnPosConfig;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem9;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem11;
        private DevExpress.XtraEditors.LabelControl lblAmountReturnToPatient;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem12;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
        private DevExpress.XtraEditors.SpinEdit spnAmountGiveByPatient;
        private DevExpress.XtraEditors.CheckEdit chkHideServiceBHYT;
        private DevExpress.XtraEditors.CheckEdit chkHideServiceNoCost;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem13;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem14;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem3;
		private DevExpress.XtraEditors.CheckEdit chkHidePatientPrice;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem15;
        private DevExpress.XtraEditors.SimpleButton btnRefreshCardCode;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem16;
        private DevExpress.XtraEditors.TextEdit txtLastDigitsOfBankCardCode;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem17;
        private DevExpress.XtraEditors.SimpleButton btnSavePrintAndTrans;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem18;
        private DevExpress.XtraBars.BarButtonItem bbtnSavePrintAndTrans;
        private DevExpress.XtraEditors.SimpleButton btnQR;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem19;
        private DevExpress.XtraBars.PopupMenu popupMenu1;
        private DevExpress.XtraEditors.SimpleButton btnAddSign;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem20;
    }
}
