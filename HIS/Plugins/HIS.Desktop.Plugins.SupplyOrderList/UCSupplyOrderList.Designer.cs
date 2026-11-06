namespace HIS.Desktop.Plugins.SupplyOrderList
{
    partial class UCSupplyOrderList
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
            this.btnDeleteDraft = new DevExpress.XtraEditors.SimpleButton();
            this.btnCancelOrder = new DevExpress.XtraEditors.SimpleButton();
            this.btnSendMail = new DevExpress.XtraEditors.SimpleButton();
            this.btnView = new DevExpress.XtraEditors.SimpleButton();
            this.btnCreate = new DevExpress.XtraEditors.SimpleButton();
            this.btnSearch = new DevExpress.XtraEditors.SimpleButton();
            this.txtCode = new DevExpress.XtraEditors.TextEdit();
            this.lblCode = new DevExpress.XtraEditors.LabelControl();
            this.dtTo = new DevExpress.XtraEditors.DateEdit();
            this.lblTo = new DevExpress.XtraEditors.LabelControl();
            this.dtFrom = new DevExpress.XtraEditors.DateEdit();
            this.lblFrom = new DevExpress.XtraEditors.LabelControl();
            this.cboMailStatus = new DevExpress.XtraEditors.CheckedComboBoxEdit();
            this.lblMailStatus = new DevExpress.XtraEditors.LabelControl();
            this.cboStatus = new DevExpress.XtraEditors.CheckedComboBoxEdit();
            this.lblStatus = new DevExpress.XtraEditors.LabelControl();
            this.cboSupplier = new DevExpress.XtraEditors.LookUpEdit();
            this.lblSupplier = new DevExpress.XtraEditors.LabelControl();
            this.cboStock = new DevExpress.XtraEditors.LookUpEdit();
            this.lblStock = new DevExpress.XtraEditors.LabelControl();
            this.splitContainer = new DevExpress.XtraEditors.SplitContainerControl();
            this.gridControlOrder = new DevExpress.XtraGrid.GridControl();
            this.gridViewOrder = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridControlDetail = new DevExpress.XtraGrid.GridControl();
            this.gridViewDetail = new DevExpress.XtraGrid.Views.Grid.GridView();
            ((System.ComponentModel.ISupportInitialize)(this.panelFilter)).BeginInit();
            this.panelFilter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtTo.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtTo.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtFrom.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtFrom.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboMailStatus.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboStatus.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboSupplier.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboStock.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlOrder)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewOrder)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlDetail)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewDetail)).BeginInit();
            this.SuspendLayout();
            //
            // panelFilter
            //
            this.panelFilter.Controls.Add(this.lblNotConfig);
            this.panelFilter.Controls.Add(this.btnDeleteDraft);
            this.panelFilter.Controls.Add(this.btnCancelOrder);
            this.panelFilter.Controls.Add(this.btnSendMail);
            this.panelFilter.Controls.Add(this.btnView);
            this.panelFilter.Controls.Add(this.btnCreate);
            this.panelFilter.Controls.Add(this.btnSearch);
            this.panelFilter.Controls.Add(this.txtCode);
            this.panelFilter.Controls.Add(this.lblCode);
            this.panelFilter.Controls.Add(this.dtTo);
            this.panelFilter.Controls.Add(this.lblTo);
            this.panelFilter.Controls.Add(this.dtFrom);
            this.panelFilter.Controls.Add(this.lblFrom);
            this.panelFilter.Controls.Add(this.cboMailStatus);
            this.panelFilter.Controls.Add(this.lblMailStatus);
            this.panelFilter.Controls.Add(this.cboStatus);
            this.panelFilter.Controls.Add(this.lblStatus);
            this.panelFilter.Controls.Add(this.cboSupplier);
            this.panelFilter.Controls.Add(this.lblSupplier);
            this.panelFilter.Controls.Add(this.cboStock);
            this.panelFilter.Controls.Add(this.lblStock);
            this.panelFilter.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelFilter.Location = new System.Drawing.Point(0, 0);
            this.panelFilter.Name = "panelFilter";
            this.panelFilter.Size = new System.Drawing.Size(1300, 98);
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
            this.cboStock.Location = new System.Drawing.Point(84, 8);
            this.cboStock.Name = "cboStock";
            this.cboStock.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboStock.Size = new System.Drawing.Size(230, 20);
            this.cboStock.TabIndex = 1;
            //
            // lblSupplier
            //
            this.lblSupplier.Location = new System.Drawing.Point(326, 11);
            this.lblSupplier.Name = "lblSupplier";
            this.lblSupplier.Size = new System.Drawing.Size(80, 13);
            this.lblSupplier.TabIndex = 2;
            this.lblSupplier.Text = "Nha cung cap:";
            //
            // cboSupplier
            //
            this.cboSupplier.Location = new System.Drawing.Point(410, 8);
            this.cboSupplier.Name = "cboSupplier";
            this.cboSupplier.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboSupplier.Size = new System.Drawing.Size(250, 20);
            this.cboSupplier.TabIndex = 3;
            //
            // lblStatus
            //
            this.lblStatus.Location = new System.Drawing.Point(672, 11);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(60, 13);
            this.lblStatus.TabIndex = 4;
            this.lblStatus.Text = "Trang thai:";
            //
            // cboStatus
            //
            this.cboStatus.EditValue = "";
            this.cboStatus.Location = new System.Drawing.Point(736, 8);
            this.cboStatus.Name = "cboStatus";
            this.cboStatus.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboStatus.Size = new System.Drawing.Size(200, 20);
            this.cboStatus.TabIndex = 5;
            //
            // lblMailStatus
            //
            this.lblMailStatus.Location = new System.Drawing.Point(948, 11);
            this.lblMailStatus.Name = "lblMailStatus";
            this.lblMailStatus.Size = new System.Drawing.Size(60, 13);
            this.lblMailStatus.TabIndex = 6;
            this.lblMailStatus.Text = "Thu:";
            //
            // cboMailStatus
            //
            this.cboMailStatus.EditValue = "";
            this.cboMailStatus.Location = new System.Drawing.Point(1012, 8);
            this.cboMailStatus.Name = "cboMailStatus";
            this.cboMailStatus.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboMailStatus.Size = new System.Drawing.Size(200, 20);
            this.cboMailStatus.TabIndex = 7;
            //
            // lblFrom
            //
            this.lblFrom.Location = new System.Drawing.Point(8, 39);
            this.lblFrom.Name = "lblFrom";
            this.lblFrom.Size = new System.Drawing.Size(70, 13);
            this.lblFrom.TabIndex = 8;
            this.lblFrom.Text = "Ngay dat tu:";
            //
            // dtFrom
            //
            this.dtFrom.EditValue = null;
            this.dtFrom.Location = new System.Drawing.Point(84, 36);
            this.dtFrom.Name = "dtFrom";
            this.dtFrom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtFrom.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.dtFrom.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
            this.dtFrom.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.dtFrom.Properties.EditFormat.FormatString = "dd/MM/yyyy";
            this.dtFrom.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.dtFrom.Properties.Mask.EditMask = "dd/MM/yyyy";
            this.dtFrom.Size = new System.Drawing.Size(100, 20);
            this.dtFrom.TabIndex = 9;
            //
            // lblTo
            //
            this.lblTo.Location = new System.Drawing.Point(192, 39);
            this.lblTo.Name = "lblTo";
            this.lblTo.Size = new System.Drawing.Size(25, 13);
            this.lblTo.TabIndex = 10;
            this.lblTo.Text = "den";
            //
            // dtTo
            //
            this.dtTo.EditValue = null;
            this.dtTo.Location = new System.Drawing.Point(214, 36);
            this.dtTo.Name = "dtTo";
            this.dtTo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtTo.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.dtTo.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
            this.dtTo.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.dtTo.Properties.EditFormat.FormatString = "dd/MM/yyyy";
            this.dtTo.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.dtTo.Properties.Mask.EditMask = "dd/MM/yyyy";
            this.dtTo.Size = new System.Drawing.Size(100, 20);
            this.dtTo.TabIndex = 11;
            //
            // lblCode
            //
            this.lblCode.Location = new System.Drawing.Point(326, 39);
            this.lblCode.Name = "lblCode";
            this.lblCode.Size = new System.Drawing.Size(80, 13);
            this.lblCode.TabIndex = 12;
            this.lblCode.Text = "So don:";
            //
            // txtCode
            //
            this.txtCode.Location = new System.Drawing.Point(410, 36);
            this.txtCode.Name = "txtCode";
            this.txtCode.Size = new System.Drawing.Size(150, 20);
            this.txtCode.TabIndex = 13;
            this.txtCode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtCode_KeyDown);
            //
            // btnSearch
            //
            this.btnSearch.Location = new System.Drawing.Point(572, 35);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(100, 22);
            this.btnSearch.TabIndex = 14;
            this.btnSearch.Text = "Tim kiem";
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            //
            // btnCreate
            //
            this.btnCreate.Location = new System.Drawing.Point(8, 66);
            this.btnCreate.Name = "btnCreate";
            this.btnCreate.Size = new System.Drawing.Size(110, 22);
            this.btnCreate.TabIndex = 15;
            this.btnCreate.Text = "Tao don moi";
            this.btnCreate.Click += new System.EventHandler(this.btnCreate_Click);
            //
            // btnView
            //
            this.btnView.Location = new System.Drawing.Point(124, 66);
            this.btnView.Name = "btnView";
            this.btnView.Size = new System.Drawing.Size(110, 22);
            this.btnView.TabIndex = 16;
            this.btnView.Text = "Xem don";
            this.btnView.Click += new System.EventHandler(this.btnView_Click);
            //
            // btnSendMail
            //
            this.btnSendMail.Location = new System.Drawing.Point(240, 66);
            this.btnSendMail.Name = "btnSendMail";
            this.btnSendMail.Size = new System.Drawing.Size(110, 22);
            this.btnSendMail.TabIndex = 17;
            this.btnSendMail.Text = "Gui email";
            this.btnSendMail.Click += new System.EventHandler(this.btnSendMail_Click);
            //
            // btnCancelOrder
            //
            this.btnCancelOrder.Location = new System.Drawing.Point(356, 66);
            this.btnCancelOrder.Name = "btnCancelOrder";
            this.btnCancelOrder.Size = new System.Drawing.Size(110, 22);
            this.btnCancelOrder.TabIndex = 18;
            this.btnCancelOrder.Text = "Huy don";
            this.btnCancelOrder.Click += new System.EventHandler(this.btnCancelOrder_Click);
            //
            // btnDeleteDraft
            //
            this.btnDeleteDraft.Location = new System.Drawing.Point(472, 66);
            this.btnDeleteDraft.Name = "btnDeleteDraft";
            this.btnDeleteDraft.Size = new System.Drawing.Size(110, 22);
            this.btnDeleteDraft.TabIndex = 19;
            this.btnDeleteDraft.Text = "Xoa nhap";
            this.btnDeleteDraft.Click += new System.EventHandler(this.btnDeleteDraft_Click);
            //
            // lblNotConfig
            //
            this.lblNotConfig.Appearance.ForeColor = System.Drawing.Color.Red;
            this.lblNotConfig.Location = new System.Drawing.Point(600, 70);
            this.lblNotConfig.Name = "lblNotConfig";
            this.lblNotConfig.Size = new System.Drawing.Size(300, 13);
            this.lblNotConfig.TabIndex = 20;
            this.lblNotConfig.Text = "Chua cau hinh kho cung ung";
            this.lblNotConfig.Visible = false;
            //
            // splitContainer
            //
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Horizontal = false;
            this.splitContainer.Location = new System.Drawing.Point(0, 98);
            this.splitContainer.Name = "splitContainer";
            this.splitContainer.Panel1.Controls.Add(this.gridControlOrder);
            this.splitContainer.Panel1.Text = "Panel1";
            this.splitContainer.Panel2.Controls.Add(this.gridControlDetail);
            this.splitContainer.Panel2.Text = "Panel2";
            this.splitContainer.Size = new System.Drawing.Size(1300, 502);
            this.splitContainer.SplitterPosition = 300;
            this.splitContainer.TabIndex = 1;
            //
            // gridControlOrder
            //
            this.gridControlOrder.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControlOrder.Location = new System.Drawing.Point(0, 0);
            this.gridControlOrder.MainView = this.gridViewOrder;
            this.gridControlOrder.Name = "gridControlOrder";
            this.gridControlOrder.Size = new System.Drawing.Size(1300, 300);
            this.gridControlOrder.TabIndex = 0;
            this.gridControlOrder.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewOrder});
            //
            // gridViewOrder
            //
            this.gridViewOrder.GridControl = this.gridControlOrder;
            this.gridViewOrder.Name = "gridViewOrder";
            //
            // gridControlDetail
            //
            this.gridControlDetail.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControlDetail.Location = new System.Drawing.Point(0, 0);
            this.gridControlDetail.MainView = this.gridViewDetail;
            this.gridControlDetail.Name = "gridControlDetail";
            this.gridControlDetail.Size = new System.Drawing.Size(1300, 197);
            this.gridControlDetail.TabIndex = 0;
            this.gridControlDetail.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewDetail});
            //
            // gridViewDetail
            //
            this.gridViewDetail.GridControl = this.gridControlDetail;
            this.gridViewDetail.Name = "gridViewDetail";
            //
            // UCSupplyOrderList
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer);
            this.Controls.Add(this.panelFilter);
            this.Name = "UCSupplyOrderList";
            this.Size = new System.Drawing.Size(1300, 600);
            this.Load += new System.EventHandler(this.UCSupplyOrderList_Load);
            ((System.ComponentModel.ISupportInitialize)(this.panelFilter)).EndInit();
            this.panelFilter.ResumeLayout(false);
            this.panelFilter.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtTo.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtTo.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtFrom.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtFrom.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboMailStatus.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboStatus.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboSupplier.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboStock.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControlOrder)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewOrder)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlDetail)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewDetail)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelFilter;
        private DevExpress.XtraEditors.LabelControl lblNotConfig;
        private DevExpress.XtraEditors.SimpleButton btnDeleteDraft;
        private DevExpress.XtraEditors.SimpleButton btnCancelOrder;
        private DevExpress.XtraEditors.SimpleButton btnSendMail;
        private DevExpress.XtraEditors.SimpleButton btnView;
        private DevExpress.XtraEditors.SimpleButton btnCreate;
        private DevExpress.XtraEditors.SimpleButton btnSearch;
        private DevExpress.XtraEditors.TextEdit txtCode;
        private DevExpress.XtraEditors.LabelControl lblCode;
        private DevExpress.XtraEditors.DateEdit dtTo;
        private DevExpress.XtraEditors.LabelControl lblTo;
        private DevExpress.XtraEditors.DateEdit dtFrom;
        private DevExpress.XtraEditors.LabelControl lblFrom;
        private DevExpress.XtraEditors.CheckedComboBoxEdit cboMailStatus;
        private DevExpress.XtraEditors.LabelControl lblMailStatus;
        private DevExpress.XtraEditors.CheckedComboBoxEdit cboStatus;
        private DevExpress.XtraEditors.LabelControl lblStatus;
        private DevExpress.XtraEditors.LookUpEdit cboSupplier;
        private DevExpress.XtraEditors.LabelControl lblSupplier;
        private DevExpress.XtraEditors.LookUpEdit cboStock;
        private DevExpress.XtraEditors.LabelControl lblStock;
        private DevExpress.XtraEditors.SplitContainerControl splitContainer;
        private DevExpress.XtraGrid.GridControl gridControlOrder;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewOrder;
        private DevExpress.XtraGrid.GridControl gridControlDetail;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewDetail;
    }
}
