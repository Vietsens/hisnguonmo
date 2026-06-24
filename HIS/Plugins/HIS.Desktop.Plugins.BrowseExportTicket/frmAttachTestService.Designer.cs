namespace HIS.Desktop.Plugins.BrowseExportTicket
{
    partial class frmAttachTestService
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
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.btnChon = new DevExpress.XtraEditors.SimpleButton();
            this.btnSearch = new DevExpress.XtraEditors.SimpleButton();
            this.txtKeyword = new DevExpress.XtraEditors.TextEdit();
            this.txtBarcode = new DevExpress.XtraEditors.TextEdit();
            this.gridControlTest = new DevExpress.XtraGrid.GridControl();
            this.gridViewTest = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gcStt = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcCheck = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemCheckEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.gcServiceReqCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcBarcode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcServiceCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcServiceName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcAmount = new DevExpress.XtraGrid.Columns.GridColumn();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciBarcode = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciKeyword = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciSearch = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciGrid = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciChon = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtKeyword.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBarcode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlTest)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewTest)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBarcode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciKeyword)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciSearch)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciChon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            this.SuspendLayout();
            //
            // layoutControl1
            //
            this.layoutControl1.Controls.Add(this.btnChon);
            this.layoutControl1.Controls.Add(this.btnSearch);
            this.layoutControl1.Controls.Add(this.txtKeyword);
            this.layoutControl1.Controls.Add(this.txtBarcode);
            this.layoutControl1.Controls.Add(this.gridControlTest);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(644, 442);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            //
            // btnChon
            //
            this.btnChon.Location = new System.Drawing.Point(517, 414);
            this.btnChon.Name = "btnChon";
            this.btnChon.Size = new System.Drawing.Size(123, 24);
            this.btnChon.StyleController = this.layoutControl1;
            this.btnChon.TabIndex = 4;
            this.btnChon.Text = "Chọn (Ctrl S)";
            this.btnChon.Click += new System.EventHandler(this.btnChon_Click);
            //
            // btnSearch
            //
            this.btnSearch.Location = new System.Drawing.Point(415, 4);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(225, 22);
            this.btnSearch.StyleController = this.layoutControl1;
            this.btnSearch.TabIndex = 2;
            this.btnSearch.Text = "Tìm kiếm (Ctrl F)";
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            //
            // txtKeyword
            //
            this.txtKeyword.Location = new System.Drawing.Point(213, 4);
            this.txtKeyword.Name = "txtKeyword";
            this.txtKeyword.Properties.NullValuePrompt = "Từ khóa tìm kiếm";
            this.txtKeyword.Size = new System.Drawing.Size(198, 20);
            this.txtKeyword.StyleController = this.layoutControl1;
            this.txtKeyword.TabIndex = 1;
            this.txtKeyword.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtKeyword_KeyDown);
            //
            // txtBarcode
            //
            this.txtBarcode.Location = new System.Drawing.Point(4, 4);
            this.txtBarcode.Name = "txtBarcode";
            this.txtBarcode.Properties.NullValuePrompt = "Barcode";
            this.txtBarcode.Size = new System.Drawing.Size(205, 20);
            this.txtBarcode.StyleController = this.layoutControl1;
            this.txtBarcode.TabIndex = 0;
            this.txtBarcode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtBarcode_KeyDown);
            //
            // gridControlTest
            //
            this.gridControlTest.Location = new System.Drawing.Point(4, 30);
            this.gridControlTest.MainView = this.gridViewTest;
            this.gridControlTest.Name = "gridControlTest";
            this.gridControlTest.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemCheckEdit1});
            this.gridControlTest.Size = new System.Drawing.Size(636, 380);
            this.gridControlTest.TabIndex = 3;
            this.gridControlTest.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gridControlTest_MouseDown);
            this.gridControlTest.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewTest});
            //
            // gridViewTest
            //
            this.gridViewTest.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gcStt,
            this.gcCheck,
            this.gcServiceReqCode,
            this.gcBarcode,
            this.gcServiceCode,
            this.gcServiceName,
            this.gcAmount});
            this.gridViewTest.GridControl = this.gridControlTest;
            this.gridViewTest.Name = "gridViewTest";
            this.gridViewTest.OptionsSelection.MultiSelect = true;
            this.gridViewTest.OptionsView.ShowGroupPanel = false;
            this.gridViewTest.OptionsView.ShowIndicator = false;
            this.gridViewTest.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewTest_CustomUnboundColumnData);
            this.gridViewTest.CustomDrawColumnHeader += new DevExpress.XtraGrid.Views.Grid.ColumnHeaderCustomDrawEventHandler(this.gridViewTest_CustomDrawColumnHeader);
            this.gridViewTest.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gridViewTest_CellValueChanged);
            //
            // gcStt
            //
            this.gcStt.AppearanceCell.Options.UseTextOptions = true;
            this.gcStt.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gcStt.AppearanceHeader.Options.UseTextOptions = true;
            this.gcStt.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gcStt.Caption = "STT";
            this.gcStt.FieldName = "STT";
            this.gcStt.Name = "gcStt";
            this.gcStt.OptionsColumn.AllowEdit = false;
            this.gcStt.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gcStt.Visible = true;
            this.gcStt.VisibleIndex = 0;
            this.gcStt.Width = 40;
            //
            // gcCheck
            //
            this.gcCheck.AppearanceHeader.Options.UseTextOptions = true;
            this.gcCheck.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gcCheck.Caption = "Chọn";
            this.gcCheck.ColumnEdit = this.repositoryItemCheckEdit1;
            this.gcCheck.FieldName = "IsCheck";
            this.gcCheck.Name = "gcCheck";
            this.gcCheck.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.gcCheck.Visible = true;
            this.gcCheck.VisibleIndex = 1;
            this.gcCheck.Width = 45;
            //
            // repositoryItemCheckEdit1
            //
            this.repositoryItemCheckEdit1.AutoHeight = false;
            this.repositoryItemCheckEdit1.Name = "repositoryItemCheckEdit1";
            //
            // gcServiceReqCode
            //
            this.gcServiceReqCode.Caption = "Mã y lệnh";
            this.gcServiceReqCode.FieldName = "TDL_SERVICE_REQ_CODE";
            this.gcServiceReqCode.Name = "gcServiceReqCode";
            this.gcServiceReqCode.OptionsColumn.AllowEdit = false;
            this.gcServiceReqCode.Visible = true;
            this.gcServiceReqCode.VisibleIndex = 2;
            this.gcServiceReqCode.Width = 110;
            //
            // gcBarcode
            //
            this.gcBarcode.Caption = "Barcode";
            this.gcBarcode.FieldName = "BARCODE";
            this.gcBarcode.Name = "gcBarcode";
            this.gcBarcode.OptionsColumn.AllowEdit = false;
            this.gcBarcode.Visible = true;
            this.gcBarcode.VisibleIndex = 3;
            this.gcBarcode.Width = 110;
            //
            // gcServiceCode
            //
            this.gcServiceCode.Caption = "Mã dịch vụ";
            this.gcServiceCode.FieldName = "TDL_SERVICE_CODE";
            this.gcServiceCode.Name = "gcServiceCode";
            this.gcServiceCode.OptionsColumn.AllowEdit = false;
            this.gcServiceCode.Visible = true;
            this.gcServiceCode.VisibleIndex = 4;
            this.gcServiceCode.Width = 100;
            //
            // gcServiceName
            //
            this.gcServiceName.Caption = "Tên dịch vụ";
            this.gcServiceName.FieldName = "TDL_SERVICE_NAME";
            this.gcServiceName.Name = "gcServiceName";
            this.gcServiceName.OptionsColumn.AllowEdit = false;
            this.gcServiceName.Visible = true;
            this.gcServiceName.VisibleIndex = 5;
            this.gcServiceName.Width = 230;
            //
            // gcAmount
            //
            this.gcAmount.AppearanceCell.Options.UseTextOptions = true;
            this.gcAmount.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gcAmount.Caption = "Số lượng";
            this.gcAmount.FieldName = "AMOUNT";
            this.gcAmount.Name = "gcAmount";
            this.gcAmount.OptionsColumn.AllowEdit = false;
            this.gcAmount.Visible = true;
            this.gcAmount.VisibleIndex = 6;
            this.gcAmount.Width = 70;
            //
            // layoutControlGroup1
            //
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciBarcode,
            this.lciKeyword,
            this.lciSearch,
            this.lciGrid,
            this.lciChon,
            this.emptySpaceItem1});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "Root";
            this.layoutControlGroup1.Size = new System.Drawing.Size(644, 442);
            this.layoutControlGroup1.TextVisible = false;
            //
            // lciBarcode
            //
            this.lciBarcode.Control = this.txtBarcode;
            this.lciBarcode.Location = new System.Drawing.Point(0, 0);
            this.lciBarcode.Name = "lciBarcode";
            this.lciBarcode.Size = new System.Drawing.Size(209, 26);
            this.lciBarcode.TextSize = new System.Drawing.Size(0, 0);
            this.lciBarcode.TextVisible = false;
            //
            // lciKeyword
            //
            this.lciKeyword.Control = this.txtKeyword;
            this.lciKeyword.Location = new System.Drawing.Point(209, 0);
            this.lciKeyword.Name = "lciKeyword";
            this.lciKeyword.Size = new System.Drawing.Size(202, 26);
            this.lciKeyword.TextSize = new System.Drawing.Size(0, 0);
            this.lciKeyword.TextVisible = false;
            //
            // lciSearch
            //
            this.lciSearch.Control = this.btnSearch;
            this.lciSearch.Location = new System.Drawing.Point(411, 0);
            this.lciSearch.Name = "lciSearch";
            this.lciSearch.Size = new System.Drawing.Size(229, 26);
            this.lciSearch.TextSize = new System.Drawing.Size(0, 0);
            this.lciSearch.TextVisible = false;
            //
            // lciGrid
            //
            this.lciGrid.Control = this.gridControlTest;
            this.lciGrid.Location = new System.Drawing.Point(0, 26);
            this.lciGrid.Name = "lciGrid";
            this.lciGrid.Size = new System.Drawing.Size(640, 384);
            this.lciGrid.TextSize = new System.Drawing.Size(0, 0);
            this.lciGrid.TextVisible = false;
            //
            // lciChon
            //
            this.lciChon.Control = this.btnChon;
            this.lciChon.Location = new System.Drawing.Point(513, 410);
            this.lciChon.Name = "lciChon";
            this.lciChon.Size = new System.Drawing.Size(127, 28);
            this.lciChon.TextSize = new System.Drawing.Size(0, 0);
            this.lciChon.TextVisible = false;
            //
            // emptySpaceItem1
            //
            this.emptySpaceItem1.Location = new System.Drawing.Point(0, 410);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(513, 28);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            //
            // frmAttachTestService
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(644, 442);
            this.Controls.Add(this.layoutControl1);
            this.MinimizeBox = false;
            this.Name = "frmAttachTestService";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Đính kèm dịch vụ xét nghiệm";
            this.Load += new System.EventHandler(this.frmAttachTestService_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtKeyword.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBarcode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlTest)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewTest)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBarcode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciKeyword)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciSearch)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciChon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.TextEdit txtBarcode;
        private DevExpress.XtraEditors.TextEdit txtKeyword;
        private DevExpress.XtraEditors.SimpleButton btnSearch;
        private DevExpress.XtraEditors.SimpleButton btnChon;
        private DevExpress.XtraGrid.GridControl gridControlTest;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewTest;
        private DevExpress.XtraGrid.Columns.GridColumn gcStt;
        private DevExpress.XtraGrid.Columns.GridColumn gcCheck;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheckEdit1;
        private DevExpress.XtraGrid.Columns.GridColumn gcServiceReqCode;
        private DevExpress.XtraGrid.Columns.GridColumn gcBarcode;
        private DevExpress.XtraGrid.Columns.GridColumn gcServiceCode;
        private DevExpress.XtraGrid.Columns.GridColumn gcServiceName;
        private DevExpress.XtraGrid.Columns.GridColumn gcAmount;
        private DevExpress.XtraLayout.LayoutControlItem lciBarcode;
        private DevExpress.XtraLayout.LayoutControlItem lciKeyword;
        private DevExpress.XtraLayout.LayoutControlItem lciSearch;
        private DevExpress.XtraLayout.LayoutControlItem lciGrid;
        private DevExpress.XtraLayout.LayoutControlItem lciChon;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
    }
}
