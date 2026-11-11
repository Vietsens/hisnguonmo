namespace HIS.Desktop.Plugins.SupplyAnticipateCreate
{
    partial class frmSupplyAnticipateCreate
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
            this.panelHeader = new DevExpress.XtraEditors.PanelControl();
            this.txtReturnReason = new DevExpress.XtraEditors.TextEdit();
            this.lblReturnReason = new DevExpress.XtraEditors.LabelControl();
            this.txtDescription = new DevExpress.XtraEditors.TextEdit();
            this.lblDescription = new DevExpress.XtraEditors.LabelControl();
            this.txtCreator = new DevExpress.XtraEditors.TextEdit();
            this.lblCreator = new DevExpress.XtraEditors.LabelControl();
            this.txtStatus = new DevExpress.XtraEditors.TextEdit();
            this.lblStatus = new DevExpress.XtraEditors.LabelControl();
            this.dtPeriod = new DevExpress.XtraEditors.DateEdit();
            this.lblPeriod = new DevExpress.XtraEditors.LabelControl();
            this.cboStock = new DevExpress.XtraEditors.LookUpEdit();
            this.lblStock = new DevExpress.XtraEditors.LabelControl();
            this.txtCode = new DevExpress.XtraEditors.TextEdit();
            this.lblCode = new DevExpress.XtraEditors.LabelControl();
            this.panelAdd = new DevExpress.XtraEditors.PanelControl();
            this.lblItemHint = new DevExpress.XtraEditors.LabelControl();
            this.cboItem = new DevExpress.XtraEditors.GridLookUpEdit();
            this.cboItemView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.lblItem = new DevExpress.XtraEditors.LabelControl();
            this.panelBottom = new DevExpress.XtraEditors.PanelControl();
            this.btnClose = new DevExpress.XtraEditors.SimpleButton();
            this.btnReturn = new DevExpress.XtraEditors.SimpleButton();
            this.btnApprove = new DevExpress.XtraEditors.SimpleButton();
            this.btnPharmacyConfirm = new DevExpress.XtraEditors.SimpleButton();
            this.btnSubmit = new DevExpress.XtraEditors.SimpleButton();
            this.btnSave = new DevExpress.XtraEditors.SimpleButton();
            this.btnReload = new DevExpress.XtraEditors.SimpleButton();
            this.gridControlLine = new DevExpress.XtraGrid.GridControl();
            this.gridViewLine = new DevExpress.XtraGrid.Views.Grid.GridView();
            ((System.ComponentModel.ISupportInitialize)(this.panelHeader)).BeginInit();
            this.panelHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtReturnReason.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDescription.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCreator.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtStatus.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtPeriod.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtPeriod.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboStock.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelAdd)).BeginInit();
            this.panelAdd.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboItem.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboItemView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelBottom)).BeginInit();
            this.panelBottom.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlLine)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewLine)).BeginInit();
            this.SuspendLayout();
            //
            // panelHeader
            //
            this.panelHeader.Controls.Add(this.txtReturnReason);
            this.panelHeader.Controls.Add(this.lblReturnReason);
            this.panelHeader.Controls.Add(this.txtDescription);
            this.panelHeader.Controls.Add(this.lblDescription);
            this.panelHeader.Controls.Add(this.txtCreator);
            this.panelHeader.Controls.Add(this.lblCreator);
            this.panelHeader.Controls.Add(this.txtStatus);
            this.panelHeader.Controls.Add(this.lblStatus);
            this.panelHeader.Controls.Add(this.dtPeriod);
            this.panelHeader.Controls.Add(this.lblPeriod);
            this.panelHeader.Controls.Add(this.cboStock);
            this.panelHeader.Controls.Add(this.lblStock);
            this.panelHeader.Controls.Add(this.txtCode);
            this.panelHeader.Controls.Add(this.lblCode);
            this.panelHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelHeader.Location = new System.Drawing.Point(0, 0);
            this.panelHeader.Name = "panelHeader";
            this.panelHeader.Size = new System.Drawing.Size(1264, 64);
            this.panelHeader.TabIndex = 0;
            //
            // lblCode
            //
            this.lblCode.Location = new System.Drawing.Point(8, 11);
            this.lblCode.Name = "lblCode";
            this.lblCode.Size = new System.Drawing.Size(60, 13);
            this.lblCode.TabIndex = 0;
            this.lblCode.Text = "So phieu:";
            //
            // txtCode
            //
            this.txtCode.Location = new System.Drawing.Point(84, 8);
            this.txtCode.Name = "txtCode";
            this.txtCode.Properties.ReadOnly = true;
            this.txtCode.Size = new System.Drawing.Size(110, 20);
            this.txtCode.TabIndex = 1;
            this.txtCode.TabStop = false;
            //
            // lblStock
            //
            this.lblStock.Location = new System.Drawing.Point(206, 11);
            this.lblStock.Name = "lblStock";
            this.lblStock.Size = new System.Drawing.Size(50, 13);
            this.lblStock.TabIndex = 2;
            this.lblStock.Text = "Kho:";
            //
            // cboStock
            //
            this.cboStock.Location = new System.Drawing.Point(262, 8);
            this.cboStock.Name = "cboStock";
            this.cboStock.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboStock.Size = new System.Drawing.Size(240, 20);
            this.cboStock.TabIndex = 3;
            this.cboStock.EditValueChanged += new System.EventHandler(this.cboStock_EditValueChanged);
            //
            // lblPeriod
            //
            this.lblPeriod.Location = new System.Drawing.Point(514, 11);
            this.lblPeriod.Name = "lblPeriod";
            this.lblPeriod.Size = new System.Drawing.Size(55, 13);
            this.lblPeriod.TabIndex = 4;
            this.lblPeriod.Text = "Ky du tru:";
            //
            // dtPeriod
            //
            this.dtPeriod.EditValue = null;
            this.dtPeriod.Location = new System.Drawing.Point(575, 8);
            this.dtPeriod.Name = "dtPeriod";
            this.dtPeriod.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtPeriod.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.dtPeriod.Properties.DisplayFormat.FormatString = "MM/yyyy";
            this.dtPeriod.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.dtPeriod.Properties.EditFormat.FormatString = "MM/yyyy";
            this.dtPeriod.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.dtPeriod.Properties.Mask.EditMask = "MM/yyyy";
            this.dtPeriod.Properties.VistaCalendarInitialViewStyle = DevExpress.XtraEditors.VistaCalendarInitialViewStyle.YearView;
            this.dtPeriod.Properties.VistaCalendarViewStyle = DevExpress.XtraEditors.VistaCalendarViewStyle.YearView;
            this.dtPeriod.Size = new System.Drawing.Size(90, 20);
            this.dtPeriod.TabIndex = 5;
            this.dtPeriod.EditValueChanged += new System.EventHandler(this.dtPeriod_EditValueChanged);
            //
            // lblStatus
            //
            this.lblStatus.Location = new System.Drawing.Point(677, 11);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(60, 13);
            this.lblStatus.TabIndex = 6;
            this.lblStatus.Text = "Trang thai:";
            //
            // txtStatus
            //
            this.txtStatus.Location = new System.Drawing.Point(743, 8);
            this.txtStatus.Name = "txtStatus";
            this.txtStatus.Properties.ReadOnly = true;
            this.txtStatus.Size = new System.Drawing.Size(150, 20);
            this.txtStatus.TabIndex = 7;
            this.txtStatus.TabStop = false;
            //
            // lblCreator
            //
            this.lblCreator.Location = new System.Drawing.Point(905, 11);
            this.lblCreator.Name = "lblCreator";
            this.lblCreator.Size = new System.Drawing.Size(60, 13);
            this.lblCreator.TabIndex = 8;
            this.lblCreator.Text = "Nguoi lap:";
            //
            // txtCreator
            //
            this.txtCreator.Location = new System.Drawing.Point(971, 8);
            this.txtCreator.Name = "txtCreator";
            this.txtCreator.Properties.ReadOnly = true;
            this.txtCreator.Size = new System.Drawing.Size(200, 20);
            this.txtCreator.TabIndex = 9;
            this.txtCreator.TabStop = false;
            //
            // lblDescription
            //
            this.lblDescription.Location = new System.Drawing.Point(8, 39);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(60, 13);
            this.lblDescription.TabIndex = 10;
            this.lblDescription.Text = "Dien giai:";
            //
            // txtDescription
            //
            this.txtDescription.Location = new System.Drawing.Point(84, 36);
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Properties.MaxLength = 300;
            this.txtDescription.Size = new System.Drawing.Size(581, 20);
            this.txtDescription.TabIndex = 11;
            this.txtDescription.EditValueChanged += new System.EventHandler(this.txtDescription_EditValueChanged);
            //
            // lblReturnReason
            //
            this.lblReturnReason.Location = new System.Drawing.Point(677, 39);
            this.lblReturnReason.Name = "lblReturnReason";
            this.lblReturnReason.Size = new System.Drawing.Size(60, 13);
            this.lblReturnReason.TabIndex = 12;
            this.lblReturnReason.Text = "Ly do tra ve:";
            //
            // txtReturnReason
            //
            this.txtReturnReason.Location = new System.Drawing.Point(743, 36);
            this.txtReturnReason.Name = "txtReturnReason";
            this.txtReturnReason.Properties.Appearance.ForeColor = System.Drawing.Color.Red;
            this.txtReturnReason.Properties.Appearance.Options.UseForeColor = true;
            this.txtReturnReason.Properties.ReadOnly = true;
            this.txtReturnReason.Size = new System.Drawing.Size(428, 20);
            this.txtReturnReason.TabIndex = 13;
            this.txtReturnReason.TabStop = false;
            //
            // panelAdd
            //
            this.panelAdd.Controls.Add(this.lblItemHint);
            this.panelAdd.Controls.Add(this.cboItem);
            this.panelAdd.Controls.Add(this.lblItem);
            this.panelAdd.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelAdd.Location = new System.Drawing.Point(0, 64);
            this.panelAdd.Name = "panelAdd";
            this.panelAdd.Size = new System.Drawing.Size(1264, 34);
            this.panelAdd.TabIndex = 1;
            //
            // lblItem
            //
            this.lblItem.Location = new System.Drawing.Point(8, 10);
            this.lblItem.Name = "lblItem";
            this.lblItem.Size = new System.Drawing.Size(70, 13);
            this.lblItem.TabIndex = 0;
            this.lblItem.Text = "Them mat hang:";
            //
            // cboItem
            //
            this.cboItem.Location = new System.Drawing.Point(84, 7);
            this.cboItem.Name = "cboItem";
            this.cboItem.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboItem.Properties.View = this.cboItemView;
            this.cboItem.Size = new System.Drawing.Size(581, 20);
            this.cboItem.TabIndex = 1;
            this.cboItem.EditValueChanged += new System.EventHandler(this.cboItem_EditValueChanged);
            //
            // cboItemView
            //
            this.cboItemView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.cboItemView.Name = "cboItemView";
            this.cboItemView.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.cboItemView.OptionsView.ShowGroupPanel = false;
            //
            // lblItemHint
            //
            this.lblItemHint.Appearance.ForeColor = System.Drawing.Color.Gray;
            this.lblItemHint.Location = new System.Drawing.Point(677, 10);
            this.lblItemHint.Name = "lblItemHint";
            this.lblItemHint.Size = new System.Drawing.Size(400, 13);
            this.lblItemHint.TabIndex = 2;
            this.lblItemHint.Text = "Go ma hoac ten de tim";
            //
            // panelBottom
            //
            this.panelBottom.Controls.Add(this.btnClose);
            this.panelBottom.Controls.Add(this.btnReturn);
            this.panelBottom.Controls.Add(this.btnApprove);
            this.panelBottom.Controls.Add(this.btnPharmacyConfirm);
            this.panelBottom.Controls.Add(this.btnSubmit);
            this.panelBottom.Controls.Add(this.btnSave);
            this.panelBottom.Controls.Add(this.btnReload);
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottom.Location = new System.Drawing.Point(0, 641);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Size = new System.Drawing.Size(1264, 40);
            this.panelBottom.TabIndex = 3;
            this.panelBottom.Resize += new System.EventHandler(this.panelBottom_Resize);
            //
            // btnReload
            //
            this.btnReload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnReload.Location = new System.Drawing.Point(526, 9);
            this.btnReload.Name = "btnReload";
            this.btnReload.Size = new System.Drawing.Size(100, 22);
            this.btnReload.TabIndex = 0;
            this.btnReload.Text = "Tai lai";
            this.btnReload.Click += new System.EventHandler(this.btnReload_Click);
            //
            // btnSave
            //
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Location = new System.Drawing.Point(632, 9);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(100, 22);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "Luu nhap (Ctrl S)";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            //
            // btnSubmit
            //
            this.btnSubmit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSubmit.Location = new System.Drawing.Point(738, 9);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(100, 22);
            this.btnSubmit.TabIndex = 2;
            this.btnSubmit.Text = "Gui duyet";
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            //
            // btnPharmacyConfirm
            //
            this.btnPharmacyConfirm.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPharmacyConfirm.Location = new System.Drawing.Point(844, 9);
            this.btnPharmacyConfirm.Name = "btnPharmacyConfirm";
            this.btnPharmacyConfirm.Size = new System.Drawing.Size(110, 22);
            this.btnPharmacyConfirm.TabIndex = 3;
            this.btnPharmacyConfirm.Text = "Duoc xac nhan";
            this.btnPharmacyConfirm.Click += new System.EventHandler(this.btnPharmacyConfirm_Click);
            //
            // btnApprove
            //
            this.btnApprove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnApprove.Location = new System.Drawing.Point(960, 9);
            this.btnApprove.Name = "btnApprove";
            this.btnApprove.Size = new System.Drawing.Size(100, 22);
            this.btnApprove.TabIndex = 4;
            this.btnApprove.Text = "Duyet";
            this.btnApprove.Click += new System.EventHandler(this.btnApprove_Click);
            //
            // btnReturn
            //
            this.btnReturn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnReturn.Location = new System.Drawing.Point(1066, 9);
            this.btnReturn.Name = "btnReturn";
            this.btnReturn.Size = new System.Drawing.Size(100, 22);
            this.btnReturn.TabIndex = 5;
            this.btnReturn.Text = "Khong duyet";
            this.btnReturn.Click += new System.EventHandler(this.btnReturn_Click);
            //
            // btnClose
            //
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Location = new System.Drawing.Point(1156, 9);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(100, 22);
            this.btnClose.TabIndex = 6;
            this.btnClose.Text = "Thoat";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            //
            // gridControlLine
            //
            this.gridControlLine.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControlLine.Location = new System.Drawing.Point(0, 98);
            this.gridControlLine.MainView = this.gridViewLine;
            this.gridControlLine.Name = "gridControlLine";
            this.gridControlLine.Size = new System.Drawing.Size(1264, 543);
            this.gridControlLine.TabIndex = 2;
            this.gridControlLine.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewLine});
            //
            // gridViewLine
            //
            this.gridViewLine.GridControl = this.gridControlLine;
            this.gridViewLine.Name = "gridViewLine";
            //
            // frmSupplyAnticipateCreate
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1264, 681);
            this.Controls.Add(this.gridControlLine);
            this.Controls.Add(this.panelAdd);
            this.Controls.Add(this.panelHeader);
            this.Controls.Add(this.panelBottom);
            this.Name = "frmSupplyAnticipateCreate";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Lap du tru cung ung";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmSupplyAnticipateCreate_FormClosing);
            this.Load += new System.EventHandler(this.frmSupplyAnticipateCreate_Load);
            ((System.ComponentModel.ISupportInitialize)(this.panelHeader)).EndInit();
            this.panelHeader.ResumeLayout(false);
            this.panelHeader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtReturnReason.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDescription.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCreator.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtStatus.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtPeriod.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtPeriod.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboStock.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelAdd)).EndInit();
            this.panelAdd.ResumeLayout(false);
            this.panelAdd.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboItem.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboItemView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelBottom)).EndInit();
            this.panelBottom.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControlLine)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewLine)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelHeader;
        private DevExpress.XtraEditors.TextEdit txtReturnReason;
        private DevExpress.XtraEditors.LabelControl lblReturnReason;
        private DevExpress.XtraEditors.TextEdit txtDescription;
        private DevExpress.XtraEditors.LabelControl lblDescription;
        private DevExpress.XtraEditors.TextEdit txtCreator;
        private DevExpress.XtraEditors.LabelControl lblCreator;
        private DevExpress.XtraEditors.TextEdit txtStatus;
        private DevExpress.XtraEditors.LabelControl lblStatus;
        private DevExpress.XtraEditors.DateEdit dtPeriod;
        private DevExpress.XtraEditors.LabelControl lblPeriod;
        private DevExpress.XtraEditors.LookUpEdit cboStock;
        private DevExpress.XtraEditors.LabelControl lblStock;
        private DevExpress.XtraEditors.TextEdit txtCode;
        private DevExpress.XtraEditors.LabelControl lblCode;
        private DevExpress.XtraEditors.PanelControl panelAdd;
        private DevExpress.XtraEditors.LabelControl lblItemHint;
        private DevExpress.XtraEditors.GridLookUpEdit cboItem;
        private DevExpress.XtraGrid.Views.Grid.GridView cboItemView;
        private DevExpress.XtraEditors.LabelControl lblItem;
        private DevExpress.XtraEditors.PanelControl panelBottom;
        private DevExpress.XtraEditors.SimpleButton btnClose;
        private DevExpress.XtraEditors.SimpleButton btnReturn;
        private DevExpress.XtraEditors.SimpleButton btnApprove;
        private DevExpress.XtraEditors.SimpleButton btnPharmacyConfirm;
        private DevExpress.XtraEditors.SimpleButton btnSubmit;
        private DevExpress.XtraEditors.SimpleButton btnSave;
        private DevExpress.XtraEditors.SimpleButton btnReload;
        private DevExpress.XtraGrid.GridControl gridControlLine;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewLine;
    }
}
