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
            this.panelHeader = new DevExpress.XtraEditors.PanelControl();
            this.lblFailValue = new DevExpress.XtraEditors.LabelControl();
            this.lblFail = new DevExpress.XtraEditors.LabelControl();
            this.lblSuccessValue = new DevExpress.XtraEditors.LabelControl();
            this.lblSuccess = new DevExpress.XtraEditors.LabelControl();
            this.lblTotalValue = new DevExpress.XtraEditors.LabelControl();
            this.lblTotal = new DevExpress.XtraEditors.LabelControl();
            this.gridControl1 = new DevExpress.XtraGrid.GridControl();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colPatientCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colResult = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colTransaction = new DevExpress.XtraGrid.Columns.GridColumn();
            this.btnClose = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.panelHeader)).BeginInit();
            this.panelHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            this.SuspendLayout();
            //
            // panelHeader
            //
            this.panelHeader.Controls.Add(this.lblFailValue);
            this.panelHeader.Controls.Add(this.lblFail);
            this.panelHeader.Controls.Add(this.lblSuccessValue);
            this.panelHeader.Controls.Add(this.lblSuccess);
            this.panelHeader.Controls.Add(this.lblTotalValue);
            this.panelHeader.Controls.Add(this.lblTotal);
            this.panelHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelHeader.Location = new System.Drawing.Point(0, 0);
            this.panelHeader.Name = "panelHeader";
            this.panelHeader.Size = new System.Drawing.Size(564, 64);
            this.panelHeader.TabIndex = 0;
            //
            // lblTotal
            //
            this.lblTotal.Location = new System.Drawing.Point(30, 18);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(74, 13);
            this.lblTotal.TabIndex = 0;
            this.lblTotal.Text = "Tổng hồ sơ đẩy:";
            //
            // lblTotalValue
            //
            this.lblTotalValue.Appearance.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblTotalValue.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(102)))), ((int)(((byte)(204)))));
            this.lblTotalValue.Location = new System.Drawing.Point(120, 16);
            this.lblTotalValue.Name = "lblTotalValue";
            this.lblTotalValue.Size = new System.Drawing.Size(12, 16);
            this.lblTotalValue.TabIndex = 1;
            this.lblTotalValue.Text = "0";
            //
            // lblSuccess
            //
            this.lblSuccess.Location = new System.Drawing.Point(320, 18);
            this.lblSuccess.Name = "lblSuccess";
            this.lblSuccess.Size = new System.Drawing.Size(56, 13);
            this.lblSuccess.TabIndex = 2;
            this.lblSuccess.Text = "Thành công:";
            //
            // lblSuccessValue
            //
            this.lblSuccessValue.Appearance.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblSuccessValue.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(60)))));
            this.lblSuccessValue.Location = new System.Drawing.Point(400, 16);
            this.lblSuccessValue.Name = "lblSuccessValue";
            this.lblSuccessValue.Size = new System.Drawing.Size(12, 16);
            this.lblSuccessValue.TabIndex = 3;
            this.lblSuccessValue.Text = "0";
            //
            // lblFail
            //
            this.lblFail.Location = new System.Drawing.Point(320, 40);
            this.lblFail.Name = "lblFail";
            this.lblFail.Size = new System.Drawing.Size(40, 13);
            this.lblFail.TabIndex = 4;
            this.lblFail.Text = "Thất bại:";
            //
            // lblFailValue
            //
            this.lblFailValue.Appearance.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblFailValue.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.lblFailValue.Location = new System.Drawing.Point(400, 38);
            this.lblFailValue.Name = "lblFailValue";
            this.lblFailValue.Size = new System.Drawing.Size(12, 16);
            this.lblFailValue.TabIndex = 5;
            this.lblFailValue.Text = "0";
            //
            // gridControl1
            //
            this.gridControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControl1.Location = new System.Drawing.Point(0, 64);
            this.gridControl1.MainView = this.gridView1;
            this.gridControl1.Name = "gridControl1";
            this.gridControl1.Size = new System.Drawing.Size(564, 241);
            this.gridControl1.TabIndex = 1;
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
            this.gridView1.OptionsView.ColumnAutoWidth = false;
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
            this.colResult.Width = 300;
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
            // btnClose
            //
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(215)))));
            this.btnClose.Appearance.ForeColor = System.Drawing.Color.White;
            this.btnClose.Appearance.Options.UseBackColor = true;
            this.btnClose.Appearance.Options.UseForeColor = true;
            this.btnClose.Location = new System.Drawing.Point(477, 311);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 24);
            this.btnClose.TabIndex = 2;
            this.btnClose.Text = "Đóng";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            //
            // frmKskSyncResult
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(564, 347);
            this.Controls.Add(this.gridControl1);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.panelHeader);
            this.MinimizeBox = false;
            this.MaximizeBox = false;
            this.Name = "frmKskSyncResult";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Kết quả đồng bộ lên Cổng dữ liệu BYT";
            this.Load += new System.EventHandler(this.frmKskSyncResult_Load);
            ((System.ComponentModel.ISupportInitialize)(this.panelHeader)).EndInit();
            this.panelHeader.ResumeLayout(false);
            this.panelHeader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelHeader;
        private DevExpress.XtraEditors.LabelControl lblFailValue;
        private DevExpress.XtraEditors.LabelControl lblFail;
        private DevExpress.XtraEditors.LabelControl lblSuccessValue;
        private DevExpress.XtraEditors.LabelControl lblSuccess;
        private DevExpress.XtraEditors.LabelControl lblTotalValue;
        private DevExpress.XtraEditors.LabelControl lblTotal;
        private DevExpress.XtraGrid.GridControl gridControl1;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraGrid.Columns.GridColumn colPatientCode;
        private DevExpress.XtraGrid.Columns.GridColumn colResult;
        private DevExpress.XtraGrid.Columns.GridColumn colTransaction;
        private DevExpress.XtraEditors.SimpleButton btnClose;
    }
}
