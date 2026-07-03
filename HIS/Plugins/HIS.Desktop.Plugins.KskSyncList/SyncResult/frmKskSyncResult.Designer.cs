namespace HIS.Desktop.Plugins.KskSyncList.SyncResult
{
    partial class frmKskSyncResult
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.btnClose = new DevExpress.XtraEditors.SimpleButton();
            this.gridControl1 = new DevExpress.XtraGrid.GridControl();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colPatientCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colResult = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colTransaction = new DevExpress.XtraGrid.Columns.GridColumn();
            this.lblFailValue = new DevExpress.XtraEditors.LabelControl();
            this.lblSuccessValue = new DevExpress.XtraEditors.LabelControl();
            this.lblTotalValue = new DevExpress.XtraEditors.LabelControl();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciTotal = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciSuccess = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciFail = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciGrid = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.lciBtnClose = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTotal)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciSuccess)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciFail)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnClose)).BeginInit();
            this.SuspendLayout();
            //
            // layoutControl1
            //
            this.layoutControl1.Controls.Add(this.btnClose);
            this.layoutControl1.Controls.Add(this.gridControl1);
            this.layoutControl1.Controls.Add(this.lblFailValue);
            this.layoutControl1.Controls.Add(this.lblSuccessValue);
            this.layoutControl1.Controls.Add(this.lblTotalValue);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.Root;
            this.layoutControl1.Size = new System.Drawing.Size(604, 396);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            //
            // btnClose
            //
            this.btnClose.Location = new System.Drawing.Point(506, 362);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(86, 22);
            this.btnClose.StyleController = this.layoutControl1;
            this.btnClose.TabIndex = 4;
            this.btnClose.Text = "Đóng";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            //
            // gridControl1
            //
            this.gridControl1.Location = new System.Drawing.Point(12, 42);
            this.gridControl1.MainView = this.gridView1;
            this.gridControl1.Name = "gridControl1";
            this.gridControl1.Size = new System.Drawing.Size(580, 306);
            this.gridControl1.TabIndex = 3;
            this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
            //
            // gridView1
            //
            this.gridView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colPatientCode,
            this.colResult,
            this.colTransaction});
            this.gridView1.GridControl = this.gridControl1;
            this.gridView1.Name = "gridView1";
            this.gridView1.OptionsBehavior.Editable = false;
            this.gridView1.OptionsView.ColumnAutoWidth = true;
            this.gridView1.OptionsView.ShowGroupPanel = false;
            this.gridView1.OptionsView.ShowIndicator = false;
            this.gridView1.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gridView1_RowCellStyle);
            //
            // colPatientCode
            //
            this.colPatientCode.Caption = "Mã BN";
            this.colPatientCode.FieldName = "PATIENT_CODE";
            this.colPatientCode.Name = "colPatientCode";
            this.colPatientCode.OptionsColumn.AllowEdit = false;
            this.colPatientCode.Visible = true;
            this.colPatientCode.VisibleIndex = 0;
            this.colPatientCode.Width = 120;
            //
            // colResult
            //
            this.colResult.Caption = "Kết quả";
            this.colResult.FieldName = "ResultText";
            this.colResult.Name = "colResult";
            this.colResult.OptionsColumn.AllowEdit = false;
            this.colResult.Visible = true;
            this.colResult.VisibleIndex = 1;
            this.colResult.Width = 320;
            //
            // colTransaction
            //
            this.colTransaction.Caption = "Mã giao dịch";
            this.colTransaction.FieldName = "TransactionDisplay";
            this.colTransaction.Name = "colTransaction";
            this.colTransaction.OptionsColumn.AllowEdit = false;
            this.colTransaction.Visible = true;
            this.colTransaction.VisibleIndex = 2;
            this.colTransaction.Width = 130;
            //
            // lblFailValue
            //
            this.lblFailValue.Appearance.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblFailValue.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.lblFailValue.Appearance.Options.UseFont = true;
            this.lblFailValue.Appearance.Options.UseForeColor = true;
            this.lblFailValue.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblFailValue.Location = new System.Drawing.Point(457, 12);
            this.lblFailValue.Name = "lblFailValue";
            this.lblFailValue.Size = new System.Drawing.Size(135, 16);
            this.lblFailValue.StyleController = this.layoutControl1;
            this.lblFailValue.TabIndex = 2;
            this.lblFailValue.Text = "0";
            //
            // lblSuccessValue
            //
            this.lblSuccessValue.Appearance.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblSuccessValue.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(60)))));
            this.lblSuccessValue.Appearance.Options.UseFont = true;
            this.lblSuccessValue.Appearance.Options.UseForeColor = true;
            this.lblSuccessValue.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblSuccessValue.Location = new System.Drawing.Point(258, 12);
            this.lblSuccessValue.Name = "lblSuccessValue";
            this.lblSuccessValue.Size = new System.Drawing.Size(74, 16);
            this.lblSuccessValue.StyleController = this.layoutControl1;
            this.lblSuccessValue.TabIndex = 1;
            this.lblSuccessValue.Text = "0";
            //
            // lblTotalValue
            //
            this.lblTotalValue.Appearance.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblTotalValue.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(102)))), ((int)(((byte)(204)))));
            this.lblTotalValue.Appearance.Options.UseFont = true;
            this.lblTotalValue.Appearance.Options.UseForeColor = true;
            this.lblTotalValue.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblTotalValue.Location = new System.Drawing.Point(101, 12);
            this.lblTotalValue.Name = "lblTotalValue";
            this.lblTotalValue.Size = new System.Drawing.Size(73, 16);
            this.lblTotalValue.StyleController = this.layoutControl1;
            this.lblTotalValue.TabIndex = 0;
            this.lblTotalValue.Text = "0";
            //
            // Root
            //
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciTotal,
            this.lciSuccess,
            this.lciFail,
            this.lciGrid,
            this.emptySpaceItem1,
            this.lciBtnClose});
            this.Root.Name = "Root";
            this.Root.Size = new System.Drawing.Size(604, 396);
            this.Root.TextVisible = false;
            //
            // lciTotal
            //
            this.lciTotal.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciTotal.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciTotal.Control = this.lblTotalValue;
            this.lciTotal.Location = new System.Drawing.Point(0, 0);
            this.lciTotal.Name = "lciTotal";
            this.lciTotal.Size = new System.Drawing.Size(250, 30);
            this.lciTotal.Text = "Tổng hồ sơ đẩy:";
            this.lciTotal.TextSize = new System.Drawing.Size(85, 13);
            //
            // lciSuccess
            //
            this.lciSuccess.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciSuccess.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciSuccess.Control = this.lblSuccessValue;
            this.lciSuccess.Location = new System.Drawing.Point(250, 0);
            this.lciSuccess.Name = "lciSuccess";
            this.lciSuccess.Size = new System.Drawing.Size(160, 30);
            this.lciSuccess.Text = "Thành công:";
            this.lciSuccess.TextSize = new System.Drawing.Size(65, 13);
            //
            // lciFail
            //
            this.lciFail.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciFail.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciFail.Control = this.lblFailValue;
            this.lciFail.Location = new System.Drawing.Point(410, 0);
            this.lciFail.Name = "lciFail";
            this.lciFail.Size = new System.Drawing.Size(174, 30);
            this.lciFail.Text = "Thất bại:";
            this.lciFail.TextSize = new System.Drawing.Size(45, 13);
            //
            // lciGrid
            //
            this.lciGrid.Control = this.gridControl1;
            this.lciGrid.Location = new System.Drawing.Point(0, 30);
            this.lciGrid.Name = "lciGrid";
            this.lciGrid.Size = new System.Drawing.Size(584, 316);
            this.lciGrid.TextSize = new System.Drawing.Size(0, 0);
            this.lciGrid.TextVisible = false;
            //
            // emptySpaceItem1
            //
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(0, 346);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(494, 30);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            //
            // lciBtnClose
            //
            this.lciBtnClose.Control = this.btnClose;
            this.lciBtnClose.Location = new System.Drawing.Point(494, 346);
            this.lciBtnClose.Name = "lciBtnClose";
            this.lciBtnClose.Size = new System.Drawing.Size(90, 30);
            this.lciBtnClose.TextSize = new System.Drawing.Size(0, 0);
            this.lciBtnClose.TextVisible = false;
            //
            // frmKskSyncResult
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(604, 396);
            this.Controls.Add(this.layoutControl1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmKskSyncResult";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Kết quả đồng bộ lên Cổng dữ liệu BYT";
            this.Load += new System.EventHandler(this.frmKskSyncResult_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTotal)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciSuccess)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciFail)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnClose)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraEditors.LabelControl lblTotalValue;
        private DevExpress.XtraEditors.LabelControl lblSuccessValue;
        private DevExpress.XtraEditors.LabelControl lblFailValue;
        private DevExpress.XtraGrid.GridControl gridControl1;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraGrid.Columns.GridColumn colPatientCode;
        private DevExpress.XtraGrid.Columns.GridColumn colResult;
        private DevExpress.XtraGrid.Columns.GridColumn colTransaction;
        private DevExpress.XtraEditors.SimpleButton btnClose;
        private DevExpress.XtraLayout.LayoutControlItem lciTotal;
        private DevExpress.XtraLayout.LayoutControlItem lciSuccess;
        private DevExpress.XtraLayout.LayoutControlItem lciFail;
        private DevExpress.XtraLayout.LayoutControlItem lciGrid;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraLayout.LayoutControlItem lciBtnClose;
    }
}
