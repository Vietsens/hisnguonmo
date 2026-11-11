namespace HIS.Desktop.Plugins.SupplyAnticipateList
{
    partial class UCSupplyAnticipateList
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

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panelFilter = new DevExpress.XtraEditors.PanelControl();
            this.lblNotConfig = new DevExpress.XtraEditors.LabelControl();
            this.btnDelete = new DevExpress.XtraEditors.SimpleButton();
            this.btnApprove = new DevExpress.XtraEditors.SimpleButton();
            this.btnPharmacyConfirm = new DevExpress.XtraEditors.SimpleButton();
            this.btnOpen = new DevExpress.XtraEditors.SimpleButton();
            this.btnCreate = new DevExpress.XtraEditors.SimpleButton();
            this.btnSearch = new DevExpress.XtraEditors.SimpleButton();
            this.txtKeyword = new DevExpress.XtraEditors.TextEdit();
            this.lblKeyword = new DevExpress.XtraEditors.LabelControl();
            this.cboStatus = new DevExpress.XtraEditors.CheckedComboBoxEdit();
            this.lblStatus = new DevExpress.XtraEditors.LabelControl();
            this.dtPeriodTo = new DevExpress.XtraEditors.DateEdit();
            this.lblPeriodTo = new DevExpress.XtraEditors.LabelControl();
            this.dtPeriodFrom = new DevExpress.XtraEditors.DateEdit();
            this.lblPeriod = new DevExpress.XtraEditors.LabelControl();
            this.cboStock = new DevExpress.XtraEditors.LookUpEdit();
            this.lblStock = new DevExpress.XtraEditors.LabelControl();
            this.gridControlAnticipate = new DevExpress.XtraGrid.GridControl();
            this.gridViewAnticipate = new DevExpress.XtraGrid.Views.Grid.GridView();
            ((System.ComponentModel.ISupportInitialize)(this.panelFilter)).BeginInit();
            this.panelFilter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtKeyword.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboStatus.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtPeriodTo.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtPeriodTo.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtPeriodFrom.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtPeriodFrom.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboStock.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlAnticipate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewAnticipate)).BeginInit();
            this.SuspendLayout();
            //
            // panelFilter
            //
            this.panelFilter.Controls.Add(this.lblNotConfig);
            this.panelFilter.Controls.Add(this.btnDelete);
            this.panelFilter.Controls.Add(this.btnApprove);
            this.panelFilter.Controls.Add(this.btnPharmacyConfirm);
            this.panelFilter.Controls.Add(this.btnOpen);
            this.panelFilter.Controls.Add(this.btnCreate);
            this.panelFilter.Controls.Add(this.btnSearch);
            this.panelFilter.Controls.Add(this.txtKeyword);
            this.panelFilter.Controls.Add(this.lblKeyword);
            this.panelFilter.Controls.Add(this.cboStatus);
            this.panelFilter.Controls.Add(this.lblStatus);
            this.panelFilter.Controls.Add(this.dtPeriodTo);
            this.panelFilter.Controls.Add(this.lblPeriodTo);
            this.panelFilter.Controls.Add(this.dtPeriodFrom);
            this.panelFilter.Controls.Add(this.lblPeriod);
            this.panelFilter.Controls.Add(this.cboStock);
            this.panelFilter.Controls.Add(this.lblStock);
            this.panelFilter.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelFilter.Location = new System.Drawing.Point(0, 0);
            this.panelFilter.Name = "panelFilter";
            this.panelFilter.Size = new System.Drawing.Size(1300, 70);
            this.panelFilter.TabIndex = 0;
            //
            // lblStock
            //
            this.lblStock.Location = new System.Drawing.Point(8, 11);
            this.lblStock.Name = "lblStock";
            this.lblStock.Size = new System.Drawing.Size(70, 13);
            this.lblStock.TabIndex = 0;
            this.lblStock.Text = "Kho:";
            //
            // cboStock
            //
            this.cboStock.Location = new System.Drawing.Point(80, 8);
            this.cboStock.Name = "cboStock";
            this.cboStock.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboStock.Size = new System.Drawing.Size(230, 20);
            this.cboStock.TabIndex = 1;
            //
            // lblPeriod
            //
            this.lblPeriod.Location = new System.Drawing.Point(322, 11);
            this.lblPeriod.Name = "lblPeriod";
            this.lblPeriod.Size = new System.Drawing.Size(50, 13);
            this.lblPeriod.TabIndex = 2;
            this.lblPeriod.Text = "Ky tu:";
            //
            // dtPeriodFrom
            //
            this.dtPeriodFrom.EditValue = null;
            this.dtPeriodFrom.Location = new System.Drawing.Point(376, 8);
            this.dtPeriodFrom.Name = "dtPeriodFrom";
            this.dtPeriodFrom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtPeriodFrom.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.dtPeriodFrom.Properties.DisplayFormat.FormatString = "MM/yyyy";
            this.dtPeriodFrom.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.dtPeriodFrom.Properties.EditFormat.FormatString = "MM/yyyy";
            this.dtPeriodFrom.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.dtPeriodFrom.Properties.Mask.EditMask = "MM/yyyy";
            this.dtPeriodFrom.Properties.VistaCalendarInitialViewStyle = DevExpress.XtraEditors.VistaCalendarInitialViewStyle.YearView;
            this.dtPeriodFrom.Properties.VistaCalendarViewStyle = DevExpress.XtraEditors.VistaCalendarViewStyle.YearView;
            this.dtPeriodFrom.Size = new System.Drawing.Size(85, 20);
            this.dtPeriodFrom.TabIndex = 3;
            //
            // lblPeriodTo
            //
            this.lblPeriodTo.Location = new System.Drawing.Point(467, 11);
            this.lblPeriodTo.Name = "lblPeriodTo";
            this.lblPeriodTo.Size = new System.Drawing.Size(25, 13);
            this.lblPeriodTo.TabIndex = 4;
            this.lblPeriodTo.Text = "den";
            //
            // dtPeriodTo
            //
            this.dtPeriodTo.EditValue = null;
            this.dtPeriodTo.Location = new System.Drawing.Point(496, 8);
            this.dtPeriodTo.Name = "dtPeriodTo";
            this.dtPeriodTo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtPeriodTo.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.dtPeriodTo.Properties.DisplayFormat.FormatString = "MM/yyyy";
            this.dtPeriodTo.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.dtPeriodTo.Properties.EditFormat.FormatString = "MM/yyyy";
            this.dtPeriodTo.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.dtPeriodTo.Properties.Mask.EditMask = "MM/yyyy";
            this.dtPeriodTo.Properties.VistaCalendarInitialViewStyle = DevExpress.XtraEditors.VistaCalendarInitialViewStyle.YearView;
            this.dtPeriodTo.Properties.VistaCalendarViewStyle = DevExpress.XtraEditors.VistaCalendarViewStyle.YearView;
            this.dtPeriodTo.Size = new System.Drawing.Size(85, 20);
            this.dtPeriodTo.TabIndex = 5;
            //
            // lblStatus
            //
            this.lblStatus.Location = new System.Drawing.Point(593, 11);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(60, 13);
            this.lblStatus.TabIndex = 6;
            this.lblStatus.Text = "Trang thai:";
            //
            // cboStatus
            //
            this.cboStatus.EditValue = "";
            this.cboStatus.Location = new System.Drawing.Point(657, 8);
            this.cboStatus.Name = "cboStatus";
            this.cboStatus.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboStatus.Size = new System.Drawing.Size(200, 20);
            this.cboStatus.TabIndex = 7;
            //
            // lblKeyword
            //
            this.lblKeyword.Location = new System.Drawing.Point(869, 11);
            this.lblKeyword.Name = "lblKeyword";
            this.lblKeyword.Size = new System.Drawing.Size(50, 13);
            this.lblKeyword.TabIndex = 8;
            this.lblKeyword.Text = "Tu khoa:";
            //
            // txtKeyword
            //
            this.txtKeyword.Location = new System.Drawing.Point(923, 8);
            this.txtKeyword.Name = "txtKeyword";
            this.txtKeyword.Size = new System.Drawing.Size(200, 20);
            this.txtKeyword.TabIndex = 9;
            this.txtKeyword.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtKeyword_KeyDown);
            //
            // btnSearch
            //
            this.btnSearch.Location = new System.Drawing.Point(1131, 7);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(100, 22);
            this.btnSearch.TabIndex = 10;
            this.btnSearch.Text = "Tim kiem";
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            //
            // btnCreate
            //
            this.btnCreate.Location = new System.Drawing.Point(8, 38);
            this.btnCreate.Name = "btnCreate";
            this.btnCreate.Size = new System.Drawing.Size(110, 22);
            this.btnCreate.TabIndex = 11;
            this.btnCreate.Text = "Lap phieu";
            this.btnCreate.Click += new System.EventHandler(this.btnCreate_Click);
            //
            // btnOpen
            //
            this.btnOpen.Location = new System.Drawing.Point(124, 38);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(110, 22);
            this.btnOpen.TabIndex = 12;
            this.btnOpen.Text = "Xem / Sua";
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            //
            // btnPharmacyConfirm
            //
            this.btnPharmacyConfirm.Location = new System.Drawing.Point(240, 38);
            this.btnPharmacyConfirm.Name = "btnPharmacyConfirm";
            this.btnPharmacyConfirm.Size = new System.Drawing.Size(120, 22);
            this.btnPharmacyConfirm.TabIndex = 13;
            this.btnPharmacyConfirm.Text = "Duoc xac nhan";
            this.btnPharmacyConfirm.Click += new System.EventHandler(this.btnPharmacyConfirm_Click);
            //
            // btnApprove
            //
            this.btnApprove.Location = new System.Drawing.Point(366, 38);
            this.btnApprove.Name = "btnApprove";
            this.btnApprove.Size = new System.Drawing.Size(110, 22);
            this.btnApprove.TabIndex = 14;
            this.btnApprove.Text = "Duyet";
            this.btnApprove.Click += new System.EventHandler(this.btnApprove_Click);
            //
            // btnDelete
            //
            this.btnDelete.Location = new System.Drawing.Point(482, 38);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(110, 22);
            this.btnDelete.TabIndex = 15;
            this.btnDelete.Text = "Xoa";
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            //
            // lblNotConfig
            //
            this.lblNotConfig.Appearance.ForeColor = System.Drawing.Color.Red;
            this.lblNotConfig.Location = new System.Drawing.Point(604, 42);
            this.lblNotConfig.Name = "lblNotConfig";
            this.lblNotConfig.Size = new System.Drawing.Size(300, 13);
            this.lblNotConfig.TabIndex = 16;
            this.lblNotConfig.Text = "Chua cau hinh kho cung ung";
            this.lblNotConfig.Visible = false;
            //
            // gridControlAnticipate
            //
            this.gridControlAnticipate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControlAnticipate.Location = new System.Drawing.Point(0, 70);
            this.gridControlAnticipate.MainView = this.gridViewAnticipate;
            this.gridControlAnticipate.Name = "gridControlAnticipate";
            this.gridControlAnticipate.Size = new System.Drawing.Size(1300, 530);
            this.gridControlAnticipate.TabIndex = 1;
            this.gridControlAnticipate.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewAnticipate});
            //
            // gridViewAnticipate
            //
            this.gridViewAnticipate.GridControl = this.gridControlAnticipate;
            this.gridViewAnticipate.Name = "gridViewAnticipate";
            //
            // UCSupplyAnticipateList
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gridControlAnticipate);
            this.Controls.Add(this.panelFilter);
            this.Name = "UCSupplyAnticipateList";
            this.Size = new System.Drawing.Size(1300, 600);
            this.Load += new System.EventHandler(this.UCSupplyAnticipateList_Load);
            ((System.ComponentModel.ISupportInitialize)(this.panelFilter)).EndInit();
            this.panelFilter.ResumeLayout(false);
            this.panelFilter.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtKeyword.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboStatus.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtPeriodTo.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtPeriodTo.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtPeriodFrom.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtPeriodFrom.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboStock.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlAnticipate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewAnticipate)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelFilter;
        private DevExpress.XtraEditors.LabelControl lblNotConfig;
        private DevExpress.XtraEditors.SimpleButton btnDelete;
        private DevExpress.XtraEditors.SimpleButton btnApprove;
        private DevExpress.XtraEditors.SimpleButton btnPharmacyConfirm;
        private DevExpress.XtraEditors.SimpleButton btnOpen;
        private DevExpress.XtraEditors.SimpleButton btnCreate;
        private DevExpress.XtraEditors.SimpleButton btnSearch;
        private DevExpress.XtraEditors.TextEdit txtKeyword;
        private DevExpress.XtraEditors.LabelControl lblKeyword;
        private DevExpress.XtraEditors.CheckedComboBoxEdit cboStatus;
        private DevExpress.XtraEditors.LabelControl lblStatus;
        private DevExpress.XtraEditors.DateEdit dtPeriodTo;
        private DevExpress.XtraEditors.LabelControl lblPeriodTo;
        private DevExpress.XtraEditors.DateEdit dtPeriodFrom;
        private DevExpress.XtraEditors.LabelControl lblPeriod;
        private DevExpress.XtraEditors.LookUpEdit cboStock;
        private DevExpress.XtraEditors.LabelControl lblStock;
        private DevExpress.XtraGrid.GridControl gridControlAnticipate;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewAnticipate;
    }
}
