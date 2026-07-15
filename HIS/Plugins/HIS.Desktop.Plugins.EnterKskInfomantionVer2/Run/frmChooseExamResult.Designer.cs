namespace HIS.Desktop.Plugins.EnterKskInfomantionVer2.Run
{
    partial class frmChooseExamResult
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.gridControl1 = new DevExpress.XtraGrid.GridControl();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colTen = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colKetQua = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colChon = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repoMemo = new DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit();
            this.repoCheck = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.pnlBottom = new DevExpress.XtraEditors.PanelControl();
            this.btnChon = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoMemo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoCheck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlBottom)).BeginInit();
            this.pnlBottom.SuspendLayout();
            this.SuspendLayout();
            //
            // gridControl1
            //
            this.gridControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControl1.Location = new System.Drawing.Point(0, 0);
            this.gridControl1.MainView = this.gridView1;
            this.gridControl1.Name = "gridControl1";
            this.gridControl1.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repoMemo,
            this.repoCheck});
            this.gridControl1.Size = new System.Drawing.Size(624, 366);
            this.gridControl1.TabIndex = 0;
            this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
            //
            // gridView1
            //
            this.gridView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colTen,
            this.colKetQua,
            this.colChon});
            this.gridView1.GridControl = this.gridControl1;
            this.gridView1.Name = "gridView1";
            this.gridView1.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridView1.OptionsView.RowAutoHeight = true;
            this.gridView1.OptionsView.ShowGroupPanel = false;
            this.gridView1.OptionsView.ShowIndicator = false;
            //
            // colTen
            //
            this.colTen.Caption = "Tên";
            this.colTen.FieldName = "Ten";
            this.colTen.Name = "colTen";
            this.colTen.OptionsColumn.AllowEdit = false;
            this.colTen.Visible = true;
            this.colTen.VisibleIndex = 0;
            this.colTen.Width = 170;
            //
            // colKetQua
            //
            this.colKetQua.Caption = "Kết quả";
            this.colKetQua.ColumnEdit = this.repoMemo;
            this.colKetQua.FieldName = "KetQua";
            this.colKetQua.Name = "colKetQua";
            this.colKetQua.OptionsColumn.ReadOnly = true;
            this.colKetQua.Visible = true;
            this.colKetQua.VisibleIndex = 1;
            this.colKetQua.Width = 380;
            //
            // colChon
            //
            this.colChon.Caption = "Chọn";
            this.colChon.ColumnEdit = this.repoCheck;
            this.colChon.FieldName = "Chon";
            this.colChon.Name = "colChon";
            this.colChon.OptionsColumn.ShowCaption = false;
            this.colChon.Visible = true;
            this.colChon.VisibleIndex = 2;
            this.colChon.Width = 60;
            //
            // repoMemo
            //
            this.repoMemo.Name = "repoMemo";
            this.repoMemo.ReadOnly = true;
            //
            // repoCheck
            //
            this.repoCheck.Name = "repoCheck";
            //
            // pnlBottom
            //
            this.pnlBottom.Controls.Add(this.btnChon);
            this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlBottom.Location = new System.Drawing.Point(0, 366);
            this.pnlBottom.Name = "pnlBottom";
            this.pnlBottom.Size = new System.Drawing.Size(624, 42);
            this.pnlBottom.TabIndex = 1;
            //
            // btnChon
            //
            this.btnChon.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnChon.Location = new System.Drawing.Point(518, 9);
            this.btnChon.Name = "btnChon";
            this.btnChon.Size = new System.Drawing.Size(94, 26);
            this.btnChon.TabIndex = 0;
            this.btnChon.Text = "Chọn";
            this.btnChon.Click += new System.EventHandler(this.btnChon_Click);
            //
            // frmChooseExamResult
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 408);
            this.Controls.Add(this.gridControl1);
            this.Controls.Add(this.pnlBottom);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmChooseExamResult";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Chọn kết quả khám điền vào Bệnh tật";
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoMemo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoCheck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlBottom)).EndInit();
            this.pnlBottom.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private DevExpress.XtraGrid.GridControl gridControl1;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraGrid.Columns.GridColumn colTen;
        private DevExpress.XtraGrid.Columns.GridColumn colKetQua;
        private DevExpress.XtraGrid.Columns.GridColumn colChon;
        private DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit repoMemo;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repoCheck;
        private DevExpress.XtraEditors.PanelControl pnlBottom;
        private DevExpress.XtraEditors.SimpleButton btnChon;
    }
}
