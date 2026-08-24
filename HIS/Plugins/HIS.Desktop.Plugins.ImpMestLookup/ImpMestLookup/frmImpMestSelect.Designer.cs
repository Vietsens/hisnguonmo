namespace HIS.Desktop.Plugins.ImpMestLookup.ImpMestLookup
{
    partial class frmImpMestSelect
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
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
            this.gridControlImpMest = new DevExpress.XtraGrid.GridControl();
            this.gridViewImpMest = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumnImpMestCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnMediStockName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnImpTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnImpUserName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnDocumentPrice = new DevExpress.XtraGrid.Columns.GridColumn();
            this.panelBottom = new DevExpress.XtraEditors.PanelControl();
            this.btnSelect = new DevExpress.XtraEditors.SimpleButton();
            this.btnClose = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlImpMest)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewImpMest)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelBottom)).BeginInit();
            this.panelBottom.SuspendLayout();
            this.SuspendLayout();
            //
            // gridControlImpMest
            //
            this.gridControlImpMest.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControlImpMest.Location = new System.Drawing.Point(0, 0);
            this.gridControlImpMest.MainView = this.gridViewImpMest;
            this.gridControlImpMest.Name = "gridControlImpMest";
            this.gridControlImpMest.Size = new System.Drawing.Size(784, 320);
            this.gridControlImpMest.TabIndex = 0;
            this.gridControlImpMest.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewImpMest});
            //
            // gridViewImpMest
            //
            this.gridViewImpMest.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumnImpMestCode,
            this.gridColumnMediStockName,
            this.gridColumnImpTime,
            this.gridColumnImpUserName,
            this.gridColumnDocumentPrice});
            this.gridViewImpMest.GridControl = this.gridControlImpMest;
            this.gridViewImpMest.Name = "gridViewImpMest";
            this.gridViewImpMest.OptionsBehavior.Editable = false;
            this.gridViewImpMest.OptionsSelection.MultiSelect = false;
            this.gridViewImpMest.OptionsView.ShowGroupPanel = false;
            this.gridViewImpMest.OptionsView.ColumnAutoWidth = false;
            this.gridViewImpMest.DoubleClick += new System.EventHandler(this.gridViewImpMest_DoubleClick);
            //
            // gridColumnImpMestCode
            //
            this.gridColumnImpMestCode.Caption = "Mã nhập";
            this.gridColumnImpMestCode.FieldName = "IMP_MEST_CODE";
            this.gridColumnImpMestCode.Name = "gridColumnImpMestCode";
            this.gridColumnImpMestCode.Visible = true;
            this.gridColumnImpMestCode.VisibleIndex = 0;
            this.gridColumnImpMestCode.Width = 130;
            //
            // gridColumnMediStockName
            //
            this.gridColumnMediStockName.Caption = "Kho nhập";
            this.gridColumnMediStockName.FieldName = "MEDI_STOCK_NAME";
            this.gridColumnMediStockName.Name = "gridColumnMediStockName";
            this.gridColumnMediStockName.Visible = true;
            this.gridColumnMediStockName.VisibleIndex = 1;
            this.gridColumnMediStockName.Width = 230;
            //
            // gridColumnImpTime
            //
            this.gridColumnImpTime.Caption = "Thời gian nhập";
            this.gridColumnImpTime.FieldName = "IMP_TIME_STR";
            this.gridColumnImpTime.Name = "gridColumnImpTime";
            this.gridColumnImpTime.Visible = true;
            this.gridColumnImpTime.VisibleIndex = 2;
            this.gridColumnImpTime.Width = 120;
            //
            // gridColumnImpUserName
            //
            this.gridColumnImpUserName.Caption = "Người nhập";
            this.gridColumnImpUserName.FieldName = "IMP_USER_NAME";
            this.gridColumnImpUserName.Name = "gridColumnImpUserName";
            this.gridColumnImpUserName.Visible = true;
            this.gridColumnImpUserName.VisibleIndex = 3;
            this.gridColumnImpUserName.Width = 180;
            //
            // gridColumnDocumentPrice
            //
            this.gridColumnDocumentPrice.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gridColumnDocumentPrice.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumnDocumentPrice.Caption = "Số tiền hóa đơn";
            this.gridColumnDocumentPrice.DisplayFormat.FormatString = "#,##0";
            this.gridColumnDocumentPrice.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumnDocumentPrice.FieldName = "DOCUMENT_PRICE";
            this.gridColumnDocumentPrice.Name = "gridColumnDocumentPrice";
            this.gridColumnDocumentPrice.Visible = true;
            this.gridColumnDocumentPrice.VisibleIndex = 4;
            this.gridColumnDocumentPrice.Width = 120;
            //
            // panelBottom
            //
            this.panelBottom.Controls.Add(this.btnClose);
            this.panelBottom.Controls.Add(this.btnSelect);
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottom.Location = new System.Drawing.Point(0, 320);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Size = new System.Drawing.Size(784, 40);
            this.panelBottom.TabIndex = 1;
            //
            // btnSelect
            //
            this.btnSelect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelect.Location = new System.Drawing.Point(580, 9);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(90, 23);
            this.btnSelect.TabIndex = 0;
            this.btnSelect.Text = "Chọn";
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            //
            // btnClose
            //
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Location = new System.Drawing.Point(680, 9);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(90, 23);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "Đóng";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            //
            // frmImpMestSelect
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 360);
            this.Controls.Add(this.gridControlImpMest);
            this.Controls.Add(this.panelBottom);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmImpMestSelect";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Chọn phiếu nhập";
            this.Load += new System.EventHandler(this.frmImpMestSelect_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridControlImpMest)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewImpMest)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelBottom)).EndInit();
            this.panelBottom.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraGrid.GridControl gridControlImpMest;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewImpMest;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnImpMestCode;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnMediStockName;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnImpTime;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnImpUserName;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnDocumentPrice;
        private DevExpress.XtraEditors.PanelControl panelBottom;
        private DevExpress.XtraEditors.SimpleButton btnSelect;
        private DevExpress.XtraEditors.SimpleButton btnClose;
    }
}
