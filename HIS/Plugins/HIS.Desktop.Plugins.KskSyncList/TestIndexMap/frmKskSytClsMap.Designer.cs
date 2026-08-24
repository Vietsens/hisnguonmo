namespace HIS.Desktop.Plugins.KskSyncList.TestIndexMap
{
    partial class frmKskSytClsMap
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
            // Bo cuc dung LayoutMode.Table + ColumnIndex/RowIndex tuong minh cho tung item
            // (khong phu thuoc Location) -> khong bi lech khi doi kich thuoc form.
            DevExpress.XtraLayout.ColumnDefinition colRootOnly = new DevExpress.XtraLayout.ColumnDefinition();
            DevExpress.XtraLayout.RowDefinition rowRootHint = new DevExpress.XtraLayout.RowDefinition();
            DevExpress.XtraLayout.RowDefinition rowRootBody = new DevExpress.XtraLayout.RowDefinition();
            DevExpress.XtraLayout.RowDefinition rowRootFooter = new DevExpress.XtraLayout.RowDefinition();
            DevExpress.XtraLayout.ColumnDefinition colBodyLeft = new DevExpress.XtraLayout.ColumnDefinition();
            DevExpress.XtraLayout.ColumnDefinition colBodyMid = new DevExpress.XtraLayout.ColumnDefinition();
            DevExpress.XtraLayout.ColumnDefinition colBodyRight = new DevExpress.XtraLayout.ColumnDefinition();
            DevExpress.XtraLayout.RowDefinition rowBodyOnly = new DevExpress.XtraLayout.RowDefinition();
            DevExpress.XtraLayout.ColumnDefinition colFootSummary = new DevExpress.XtraLayout.ColumnDefinition();
            DevExpress.XtraLayout.ColumnDefinition colFootExport = new DevExpress.XtraLayout.ColumnDefinition();
            DevExpress.XtraLayout.ColumnDefinition colFootImport = new DevExpress.XtraLayout.ColumnDefinition();
            DevExpress.XtraLayout.ColumnDefinition colFootSave = new DevExpress.XtraLayout.ColumnDefinition();
            DevExpress.XtraLayout.ColumnDefinition colFootClose = new DevExpress.XtraLayout.ColumnDefinition();
            DevExpress.XtraLayout.RowDefinition rowFootOnly = new DevExpress.XtraLayout.RowDefinition();
            DevExpress.XtraLayout.ColumnDefinition colMidOnly = new DevExpress.XtraLayout.ColumnDefinition();
            DevExpress.XtraLayout.RowDefinition rowMidAssign = new DevExpress.XtraLayout.RowDefinition();
            DevExpress.XtraLayout.RowDefinition rowMidUnassign = new DevExpress.XtraLayout.RowDefinition();
            DevExpress.XtraLayout.RowDefinition rowMidRest = new DevExpress.XtraLayout.RowDefinition();

            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.gridTestIndex = new DevExpress.XtraGrid.GridControl();
            this.gridViewTestIndex = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colTiCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colTiName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colTiUnit = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colTiGroup = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridField = new DevExpress.XtraGrid.GridControl();
            this.gridViewField = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colFdGroup = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colFdName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colFdCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colFdTiCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colFdTiName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colFdTiUnit = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colFdNote = new DevExpress.XtraGrid.Columns.GridColumn();
            this.btnAssign = new DevExpress.XtraEditors.SimpleButton();
            this.btnUnassign = new DevExpress.XtraEditors.SimpleButton();
            this.btnExport = new DevExpress.XtraEditors.SimpleButton();
            this.btnImport = new DevExpress.XtraEditors.SimpleButton();
            this.btnSave = new DevExpress.XtraEditors.SimpleButton();
            this.btnClose = new DevExpress.XtraEditors.SimpleButton();
            this.lblSummary = new DevExpress.XtraEditors.LabelControl();
            this.lblScopeHint = new DevExpress.XtraEditors.LabelControl();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lcgBody = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lcgLeft = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lcgMid = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lcgRight = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lcgFooter = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciGridTestIndex = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciGridField = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciBtnAssign = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciBtnUnassign = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceMid = new DevExpress.XtraLayout.EmptySpaceItem();
            this.lciScopeHint = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciSummary = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciBtnExport = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciBtnImport = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciBtnSave = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciBtnClose = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridTestIndex)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewTestIndex)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridField)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewField)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcgBody)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcgLeft)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcgMid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcgRight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcgFooter)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciGridTestIndex)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciGridField)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnAssign)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnUnassign)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceMid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciScopeHint)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciSummary)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnExport)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnImport)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnSave)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnClose)).BeginInit();
            this.SuspendLayout();
            //
            // layoutControl1
            //
            this.layoutControl1.Controls.Add(this.gridTestIndex);
            this.layoutControl1.Controls.Add(this.gridField);
            this.layoutControl1.Controls.Add(this.btnAssign);
            this.layoutControl1.Controls.Add(this.btnUnassign);
            this.layoutControl1.Controls.Add(this.btnExport);
            this.layoutControl1.Controls.Add(this.btnImport);
            this.layoutControl1.Controls.Add(this.btnSave);
            this.layoutControl1.Controls.Add(this.btnClose);
            this.layoutControl1.Controls.Add(this.lblSummary);
            this.layoutControl1.Controls.Add(this.lblScopeHint);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.Root;
            this.layoutControl1.Size = new System.Drawing.Size(1164, 601);
            this.layoutControl1.TabIndex = 0;
            //
            // gridTestIndex
            //
            this.gridTestIndex.MainView = this.gridViewTestIndex;
            this.gridTestIndex.Name = "gridTestIndex";
            this.gridTestIndex.TabIndex = 0;
            this.gridTestIndex.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewTestIndex});
            //
            // gridViewTestIndex
            //
            this.gridViewTestIndex.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colTiCode,
            this.colTiName,
            this.colTiUnit,
            this.colTiGroup});
            this.gridViewTestIndex.GridControl = this.gridTestIndex;
            this.gridViewTestIndex.Name = "gridViewTestIndex";
            this.gridViewTestIndex.OptionsBehavior.Editable = false;
            this.gridViewTestIndex.OptionsFind.AlwaysVisible = true;
            this.gridViewTestIndex.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridViewTestIndex.OptionsView.ShowGroupPanel = false;
            this.gridViewTestIndex.OptionsView.ShowIndicator = false;
            //
            // colTiCode
            //
            this.colTiCode.Caption = "Mã chỉ số";
            this.colTiCode.FieldName = "TEST_INDEX_CODE";
            this.colTiCode.Name = "colTiCode";
            this.colTiCode.Visible = true;
            this.colTiCode.VisibleIndex = 0;
            this.colTiCode.Width = 90;
            //
            // colTiName
            //
            this.colTiName.Caption = "Tên chỉ số";
            this.colTiName.FieldName = "TEST_INDEX_NAME";
            this.colTiName.Name = "colTiName";
            this.colTiName.Visible = true;
            this.colTiName.VisibleIndex = 1;
            this.colTiName.Width = 190;
            //
            // colTiUnit
            //
            this.colTiUnit.Caption = "Đơn vị";
            this.colTiUnit.FieldName = "TEST_INDEX_UNIT_NAME";
            this.colTiUnit.Name = "colTiUnit";
            this.colTiUnit.Visible = true;
            this.colTiUnit.VisibleIndex = 2;
            this.colTiUnit.Width = 70;
            //
            // colTiGroup
            //
            this.colTiGroup.Caption = "Nhóm chỉ số";
            this.colTiGroup.FieldName = "TEST_INDEX_GROUP_NAME";
            this.colTiGroup.Name = "colTiGroup";
            this.colTiGroup.Visible = true;
            this.colTiGroup.VisibleIndex = 3;
            this.colTiGroup.Width = 120;
            //
            // gridField
            //
            this.gridField.MainView = this.gridViewField;
            this.gridField.Name = "gridField";
            this.gridField.TabIndex = 1;
            this.gridField.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewField});
            //
            // gridViewField
            //
            this.gridViewField.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colFdGroup,
            this.colFdName,
            this.colFdCode,
            this.colFdTiCode,
            this.colFdTiName,
            this.colFdTiUnit,
            this.colFdNote});
            this.gridViewField.GridControl = this.gridField;
            this.gridViewField.Name = "gridViewField";
            this.gridViewField.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridViewField.OptionsView.ShowGroupPanel = false;
            this.gridViewField.OptionsView.ShowIndicator = false;
            //
            // colFdGroup
            //
            this.colFdGroup.Caption = "Nhóm";
            this.colFdGroup.FieldName = "GroupName";
            this.colFdGroup.Name = "colFdGroup";
            this.colFdGroup.OptionsColumn.AllowEdit = false;
            this.colFdGroup.Visible = true;
            this.colFdGroup.VisibleIndex = 0;
            this.colFdGroup.Width = 90;
            //
            // colFdName
            //
            this.colFdName.Caption = "Chỉ tiêu của cổng";
            this.colFdName.FieldName = "FieldName";
            this.colFdName.Name = "colFdName";
            this.colFdName.OptionsColumn.AllowEdit = false;
            this.colFdName.Visible = true;
            this.colFdName.VisibleIndex = 1;
            this.colFdName.Width = 175;
            //
            // colFdCode
            //
            this.colFdCode.Caption = "Mã chỉ tiêu";
            this.colFdCode.FieldName = "FieldCode";
            this.colFdCode.Name = "colFdCode";
            this.colFdCode.OptionsColumn.AllowEdit = false;
            this.colFdCode.Visible = true;
            this.colFdCode.VisibleIndex = 2;
            this.colFdCode.Width = 110;
            //
            // colFdTiCode
            //
            this.colFdTiCode.Caption = "Mã chỉ số HIS";
            this.colFdTiCode.FieldName = "TestIndexCode";
            this.colFdTiCode.Name = "colFdTiCode";
            this.colFdTiCode.OptionsColumn.AllowEdit = false;
            this.colFdTiCode.Visible = true;
            this.colFdTiCode.VisibleIndex = 3;
            this.colFdTiCode.Width = 90;
            //
            // colFdTiName
            //
            this.colFdTiName.Caption = "Tên chỉ số HIS";
            this.colFdTiName.FieldName = "TestIndexName";
            this.colFdTiName.Name = "colFdTiName";
            this.colFdTiName.OptionsColumn.AllowEdit = false;
            this.colFdTiName.Visible = true;
            this.colFdTiName.VisibleIndex = 4;
            this.colFdTiName.Width = 165;
            //
            // colFdTiUnit
            //
            this.colFdTiUnit.Caption = "Đơn vị";
            this.colFdTiUnit.FieldName = "TestIndexUnitName";
            this.colFdTiUnit.Name = "colFdTiUnit";
            this.colFdTiUnit.OptionsColumn.AllowEdit = false;
            this.colFdTiUnit.Visible = true;
            this.colFdTiUnit.VisibleIndex = 5;
            this.colFdTiUnit.Width = 70;
            //
            // colFdNote
            //
            this.colFdNote.Caption = "Ghi chú";
            this.colFdNote.FieldName = "Note";
            this.colFdNote.Name = "colFdNote";
            this.colFdNote.Visible = true;
            this.colFdNote.VisibleIndex = 6;
            this.colFdNote.Width = 110;
            //
            // btnAssign
            //
            this.btnAssign.Name = "btnAssign";
            this.btnAssign.StyleController = this.layoutControl1;
            this.btnAssign.TabIndex = 2;
            this.btnAssign.Text = "Gán  >>";
            //
            // btnUnassign
            //
            this.btnUnassign.Name = "btnUnassign";
            this.btnUnassign.StyleController = this.layoutControl1;
            this.btnUnassign.TabIndex = 3;
            this.btnUnassign.Text = "Bỏ gán";
            //
            // btnExport
            //
            this.btnExport.Name = "btnExport";
            this.btnExport.StyleController = this.layoutControl1;
            this.btnExport.TabIndex = 4;
            this.btnExport.Text = "Xuất JSON";
            //
            // btnImport
            //
            this.btnImport.Name = "btnImport";
            this.btnImport.StyleController = this.layoutControl1;
            this.btnImport.TabIndex = 5;
            this.btnImport.Text = "Nhập JSON";
            //
            // btnSave
            //
            this.btnSave.Name = "btnSave";
            this.btnSave.StyleController = this.layoutControl1;
            this.btnSave.TabIndex = 6;
            this.btnSave.Text = "Lưu";
            //
            // btnClose
            //
            this.btnClose.Name = "btnClose";
            this.btnClose.StyleController = this.layoutControl1;
            this.btnClose.TabIndex = 7;
            this.btnClose.Text = "Đóng";
            //
            // lblSummary
            //
            this.lblSummary.Name = "lblSummary";
            this.lblSummary.StyleController = this.layoutControl1;
            this.lblSummary.TabIndex = 8;
            //
            // lblScopeHint
            //
            this.lblScopeHint.Name = "lblScopeHint";
            this.lblScopeHint.StyleController = this.layoutControl1;
            this.lblScopeHint.TabIndex = 9;
            //
            // Root — bang 1 cot x 3 dong: dong chu pham vi / than / chan
            //
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciScopeHint,
            this.lcgBody,
            this.lcgFooter});
            this.Root.LayoutMode = DevExpress.XtraLayout.Utils.LayoutMode.Table;
            this.Root.Name = "Root";
            colRootOnly.SizeType = System.Windows.Forms.SizeType.Percent;
            colRootOnly.Width = 100D;
            this.Root.OptionsTableLayoutGroup.ColumnDefinitions.AddRange(new DevExpress.XtraLayout.ColumnDefinition[] {
            colRootOnly});
            rowRootHint.Height = 24D;
            rowRootHint.SizeType = System.Windows.Forms.SizeType.Absolute;
            rowRootBody.Height = 100D;
            rowRootBody.SizeType = System.Windows.Forms.SizeType.Percent;
            rowRootFooter.Height = 40D;
            rowRootFooter.SizeType = System.Windows.Forms.SizeType.Absolute;
            this.Root.OptionsTableLayoutGroup.RowDefinitions.AddRange(new DevExpress.XtraLayout.RowDefinition[] {
            rowRootHint,
            rowRootBody,
            rowRootFooter});
            this.Root.Size = new System.Drawing.Size(1164, 601);
            this.Root.TextVisible = false;
            //
            // lciScopeHint
            //
            this.lciScopeHint.Control = this.lblScopeHint;
            this.lciScopeHint.Name = "lciScopeHint";
            this.lciScopeHint.OptionsTableLayoutItem.ColumnIndex = 0;
            this.lciScopeHint.OptionsTableLayoutItem.RowIndex = 0;
            this.lciScopeHint.TextSize = new System.Drawing.Size(0, 0);
            this.lciScopeHint.TextVisible = false;
            //
            // lcgBody — bang 3 cot x 1 dong: luoi trai / cot nut / luoi phai
            //
            this.lcgBody.GroupBordersVisible = false;
            this.lcgBody.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lcgLeft,
            this.lcgMid,
            this.lcgRight});
            this.lcgBody.LayoutMode = DevExpress.XtraLayout.Utils.LayoutMode.Table;
            this.lcgBody.Name = "lcgBody";
            this.lcgBody.OptionsTableLayoutItem.ColumnIndex = 0;
            this.lcgBody.OptionsTableLayoutItem.RowIndex = 1;
            colBodyLeft.SizeType = System.Windows.Forms.SizeType.Percent;
            colBodyLeft.Width = 44D;
            colBodyMid.SizeType = System.Windows.Forms.SizeType.Absolute;
            colBodyMid.Width = 120D;
            colBodyRight.SizeType = System.Windows.Forms.SizeType.Percent;
            colBodyRight.Width = 56D;
            this.lcgBody.OptionsTableLayoutGroup.ColumnDefinitions.AddRange(new DevExpress.XtraLayout.ColumnDefinition[] {
            colBodyLeft,
            colBodyMid,
            colBodyRight});
            rowBodyOnly.Height = 100D;
            rowBodyOnly.SizeType = System.Windows.Forms.SizeType.Percent;
            this.lcgBody.OptionsTableLayoutGroup.RowDefinitions.AddRange(new DevExpress.XtraLayout.RowDefinition[] {
            rowBodyOnly});
            this.lcgBody.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.lcgBody.TextVisible = false;
            //
            // lcgLeft
            //
            this.lcgLeft.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciGridTestIndex});
            this.lcgLeft.Name = "lcgLeft";
            this.lcgLeft.OptionsTableLayoutItem.ColumnIndex = 0;
            this.lcgLeft.OptionsTableLayoutItem.RowIndex = 0;
            this.lcgLeft.Text = "Danh mục chỉ số xét nghiệm của HIS";
            //
            // lciGridTestIndex
            //
            this.lciGridTestIndex.Control = this.gridTestIndex;
            this.lciGridTestIndex.Name = "lciGridTestIndex";
            this.lciGridTestIndex.TextSize = new System.Drawing.Size(0, 0);
            this.lciGridTestIndex.TextVisible = false;
            //
            // lcgMid — bang 1 cot x 3 dong: nut Gan / nut Bo gan / khoang trong
            //
            this.lcgMid.GroupBordersVisible = false;
            this.lcgMid.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciBtnAssign,
            this.lciBtnUnassign,
            this.emptySpaceMid});
            this.lcgMid.LayoutMode = DevExpress.XtraLayout.Utils.LayoutMode.Table;
            this.lcgMid.Name = "lcgMid";
            this.lcgMid.OptionsTableLayoutItem.ColumnIndex = 1;
            this.lcgMid.OptionsTableLayoutItem.RowIndex = 0;
            colMidOnly.SizeType = System.Windows.Forms.SizeType.Percent;
            colMidOnly.Width = 100D;
            this.lcgMid.OptionsTableLayoutGroup.ColumnDefinitions.AddRange(new DevExpress.XtraLayout.ColumnDefinition[] {
            colMidOnly});
            rowMidAssign.Height = 34D;
            rowMidAssign.SizeType = System.Windows.Forms.SizeType.Absolute;
            rowMidUnassign.Height = 34D;
            rowMidUnassign.SizeType = System.Windows.Forms.SizeType.Absolute;
            rowMidRest.Height = 100D;
            rowMidRest.SizeType = System.Windows.Forms.SizeType.Percent;
            this.lcgMid.OptionsTableLayoutGroup.RowDefinitions.AddRange(new DevExpress.XtraLayout.RowDefinition[] {
            rowMidAssign,
            rowMidUnassign,
            rowMidRest});
            this.lcgMid.TextVisible = false;
            //
            // lciBtnAssign
            //
            this.lciBtnAssign.Control = this.btnAssign;
            this.lciBtnAssign.Name = "lciBtnAssign";
            this.lciBtnAssign.OptionsTableLayoutItem.ColumnIndex = 0;
            this.lciBtnAssign.OptionsTableLayoutItem.RowIndex = 0;
            this.lciBtnAssign.TextSize = new System.Drawing.Size(0, 0);
            this.lciBtnAssign.TextVisible = false;
            //
            // lciBtnUnassign
            //
            this.lciBtnUnassign.Control = this.btnUnassign;
            this.lciBtnUnassign.Name = "lciBtnUnassign";
            this.lciBtnUnassign.OptionsTableLayoutItem.ColumnIndex = 0;
            this.lciBtnUnassign.OptionsTableLayoutItem.RowIndex = 1;
            this.lciBtnUnassign.TextSize = new System.Drawing.Size(0, 0);
            this.lciBtnUnassign.TextVisible = false;
            //
            // emptySpaceMid
            //
            this.emptySpaceMid.AllowHotTrack = false;
            this.emptySpaceMid.Name = "emptySpaceMid";
            this.emptySpaceMid.OptionsTableLayoutItem.ColumnIndex = 0;
            this.emptySpaceMid.OptionsTableLayoutItem.RowIndex = 2;
            this.emptySpaceMid.TextSize = new System.Drawing.Size(0, 0);
            //
            // lcgRight
            //
            this.lcgRight.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciGridField});
            this.lcgRight.Name = "lcgRight";
            this.lcgRight.OptionsTableLayoutItem.ColumnIndex = 2;
            this.lcgRight.OptionsTableLayoutItem.RowIndex = 0;
            this.lcgRight.Text = "Chỉ tiêu cận lâm sàng mẫu M4 và chỉ số đã nối";
            //
            // lciGridField
            //
            this.lciGridField.Control = this.gridField;
            this.lciGridField.Name = "lciGridField";
            this.lciGridField.TextSize = new System.Drawing.Size(0, 0);
            this.lciGridField.TextVisible = false;
            //
            // lcgFooter — bang 5 cot x 1 dong: dong tong hop / 4 nut
            //
            this.lcgFooter.GroupBordersVisible = false;
            this.lcgFooter.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciSummary,
            this.lciBtnExport,
            this.lciBtnImport,
            this.lciBtnSave,
            this.lciBtnClose});
            this.lcgFooter.LayoutMode = DevExpress.XtraLayout.Utils.LayoutMode.Table;
            this.lcgFooter.Name = "lcgFooter";
            this.lcgFooter.OptionsTableLayoutItem.ColumnIndex = 0;
            this.lcgFooter.OptionsTableLayoutItem.RowIndex = 2;
            colFootSummary.SizeType = System.Windows.Forms.SizeType.Percent;
            colFootSummary.Width = 100D;
            colFootExport.SizeType = System.Windows.Forms.SizeType.Absolute;
            colFootExport.Width = 116D;
            colFootImport.SizeType = System.Windows.Forms.SizeType.Absolute;
            colFootImport.Width = 116D;
            colFootSave.SizeType = System.Windows.Forms.SizeType.Absolute;
            colFootSave.Width = 116D;
            colFootClose.SizeType = System.Windows.Forms.SizeType.Absolute;
            colFootClose.Width = 116D;
            this.lcgFooter.OptionsTableLayoutGroup.ColumnDefinitions.AddRange(new DevExpress.XtraLayout.ColumnDefinition[] {
            colFootSummary,
            colFootExport,
            colFootImport,
            colFootSave,
            colFootClose});
            rowFootOnly.Height = 100D;
            rowFootOnly.SizeType = System.Windows.Forms.SizeType.Percent;
            this.lcgFooter.OptionsTableLayoutGroup.RowDefinitions.AddRange(new DevExpress.XtraLayout.RowDefinition[] {
            rowFootOnly});
            this.lcgFooter.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.lcgFooter.TextVisible = false;
            //
            // lciSummary
            //
            this.lciSummary.Control = this.lblSummary;
            this.lciSummary.Name = "lciSummary";
            this.lciSummary.OptionsTableLayoutItem.ColumnIndex = 0;
            this.lciSummary.OptionsTableLayoutItem.RowIndex = 0;
            this.lciSummary.TextSize = new System.Drawing.Size(0, 0);
            this.lciSummary.TextVisible = false;
            //
            // lciBtnExport
            //
            this.lciBtnExport.Control = this.btnExport;
            this.lciBtnExport.Name = "lciBtnExport";
            this.lciBtnExport.OptionsTableLayoutItem.ColumnIndex = 1;
            this.lciBtnExport.OptionsTableLayoutItem.RowIndex = 0;
            this.lciBtnExport.TextSize = new System.Drawing.Size(0, 0);
            this.lciBtnExport.TextVisible = false;
            //
            // lciBtnImport
            //
            this.lciBtnImport.Control = this.btnImport;
            this.lciBtnImport.Name = "lciBtnImport";
            this.lciBtnImport.OptionsTableLayoutItem.ColumnIndex = 2;
            this.lciBtnImport.OptionsTableLayoutItem.RowIndex = 0;
            this.lciBtnImport.TextSize = new System.Drawing.Size(0, 0);
            this.lciBtnImport.TextVisible = false;
            //
            // lciBtnSave
            //
            this.lciBtnSave.Control = this.btnSave;
            this.lciBtnSave.Name = "lciBtnSave";
            this.lciBtnSave.OptionsTableLayoutItem.ColumnIndex = 3;
            this.lciBtnSave.OptionsTableLayoutItem.RowIndex = 0;
            this.lciBtnSave.TextSize = new System.Drawing.Size(0, 0);
            this.lciBtnSave.TextVisible = false;
            //
            // lciBtnClose
            //
            this.lciBtnClose.Control = this.btnClose;
            this.lciBtnClose.Name = "lciBtnClose";
            this.lciBtnClose.OptionsTableLayoutItem.ColumnIndex = 4;
            this.lciBtnClose.OptionsTableLayoutItem.RowIndex = 0;
            this.lciBtnClose.TextSize = new System.Drawing.Size(0, 0);
            this.lciBtnClose.TextVisible = false;
            //
            // frmKskSytClsMap
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1164, 601);
            this.Controls.Add(this.layoutControl1);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(1000, 540);
            this.Name = "frmKskSytClsMap";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Nối chỉ số cận lâm sàng — Liên thông KSK Sở Y tế TP.HCM (mẫu M4)";
            this.Load += new System.EventHandler(this.frmKskSytClsMap_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmKskSytClsMap_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnClose)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnSave)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnImport)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnExport)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciSummary)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciScopeHint)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceMid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnUnassign)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnAssign)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciGridField)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciGridTestIndex)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcgFooter)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcgRight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcgMid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcgLeft)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcgBody)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewField)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridField)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewTestIndex)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridTestIndex)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraLayout.LayoutControlGroup lcgBody;
        private DevExpress.XtraLayout.LayoutControlGroup lcgLeft;
        private DevExpress.XtraLayout.LayoutControlGroup lcgMid;
        private DevExpress.XtraLayout.LayoutControlGroup lcgRight;
        private DevExpress.XtraLayout.LayoutControlGroup lcgFooter;
        private DevExpress.XtraGrid.GridControl gridTestIndex;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewTestIndex;
        private DevExpress.XtraGrid.Columns.GridColumn colTiCode;
        private DevExpress.XtraGrid.Columns.GridColumn colTiName;
        private DevExpress.XtraGrid.Columns.GridColumn colTiUnit;
        private DevExpress.XtraGrid.Columns.GridColumn colTiGroup;
        private DevExpress.XtraGrid.GridControl gridField;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewField;
        private DevExpress.XtraGrid.Columns.GridColumn colFdGroup;
        private DevExpress.XtraGrid.Columns.GridColumn colFdName;
        private DevExpress.XtraGrid.Columns.GridColumn colFdCode;
        private DevExpress.XtraGrid.Columns.GridColumn colFdTiCode;
        private DevExpress.XtraGrid.Columns.GridColumn colFdTiName;
        private DevExpress.XtraGrid.Columns.GridColumn colFdTiUnit;
        private DevExpress.XtraGrid.Columns.GridColumn colFdNote;
        private DevExpress.XtraEditors.SimpleButton btnAssign;
        private DevExpress.XtraEditors.SimpleButton btnUnassign;
        private DevExpress.XtraEditors.SimpleButton btnExport;
        private DevExpress.XtraEditors.SimpleButton btnImport;
        private DevExpress.XtraEditors.SimpleButton btnSave;
        private DevExpress.XtraEditors.SimpleButton btnClose;
        private DevExpress.XtraEditors.LabelControl lblSummary;
        private DevExpress.XtraEditors.LabelControl lblScopeHint;
        private DevExpress.XtraLayout.LayoutControlItem lciGridTestIndex;
        private DevExpress.XtraLayout.LayoutControlItem lciGridField;
        private DevExpress.XtraLayout.LayoutControlItem lciBtnAssign;
        private DevExpress.XtraLayout.LayoutControlItem lciBtnUnassign;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceMid;
        private DevExpress.XtraLayout.LayoutControlItem lciScopeHint;
        private DevExpress.XtraLayout.LayoutControlItem lciSummary;
        private DevExpress.XtraLayout.LayoutControlItem lciBtnExport;
        private DevExpress.XtraLayout.LayoutControlItem lciBtnImport;
        private DevExpress.XtraLayout.LayoutControlItem lciBtnSave;
        private DevExpress.XtraLayout.LayoutControlItem lciBtnClose;
    }
}
