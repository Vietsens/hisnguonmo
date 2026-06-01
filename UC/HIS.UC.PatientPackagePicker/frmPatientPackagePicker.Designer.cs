/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 */
namespace HIS.UC.PatientPackagePicker
{
    partial class frmPatientPackagePicker
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
            this.splitContainer = new DevExpress.XtraEditors.SplitContainerControl();
            this.grpPackage = new DevExpress.XtraEditors.GroupControl();
            this.layoutControlPackage = new DevExpress.XtraLayout.LayoutControl();
            this.txtKeywordPackage = new DevExpress.XtraEditors.TextEdit();
            this.gridControlPackage = new DevExpress.XtraGrid.GridControl();
            this.gridViewPackage = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colPackageName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colRegisterDate = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colNote = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colCreateTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colCreator = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colModifyTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colModifier = new DevExpress.XtraGrid.Columns.GridColumn();
            this.lcgRootPackage = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciKeywordPackage = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciGridPackage = new DevExpress.XtraLayout.LayoutControlItem();
            this.grpDetail = new DevExpress.XtraEditors.GroupControl();
            this.layoutControlDetail = new DevExpress.XtraLayout.LayoutControl();
            this.txtKeywordDetail = new DevExpress.XtraEditors.TextEdit();
            this.gridControlDetail = new DevExpress.XtraGrid.GridControl();
            this.gridViewDetail = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colCheck = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repoCheck = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.colServiceCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colServiceName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colServiceTypeName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colAmount = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colAmountUsed = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colAmountThisTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repoAmountThisTime = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
            this.lcgRootDetail = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciKeywordDetail = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciGridDetail = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlBottom = new DevExpress.XtraLayout.LayoutControl();
            this.btnChoose = new DevExpress.XtraEditors.SimpleButton();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            this.lcgRootBottom = new DevExpress.XtraLayout.LayoutControlGroup();
            this.emptySpaceBottom = new DevExpress.XtraLayout.EmptySpaceItem();
            this.lciBtnChoose = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciBtnCancel = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grpPackage)).BeginInit();
            this.grpPackage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlPackage)).BeginInit();
            this.layoutControlPackage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtKeywordPackage.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlPackage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewPackage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcgRootPackage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciKeywordPackage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciGridPackage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpDetail)).BeginInit();
            this.grpDetail.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlDetail)).BeginInit();
            this.layoutControlDetail.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtKeywordDetail.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlDetail)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewDetail)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoCheck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoAmountThisTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcgRootDetail)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciKeywordDetail)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciGridDetail)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlBottom)).BeginInit();
            this.layoutControlBottom.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lcgRootBottom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceBottom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnChoose)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnCancel)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.Name = "splitContainer";
            this.splitContainer.Panel1.Controls.Add(this.grpPackage);
            this.splitContainer.Panel1.Text = "Panel1";
            this.splitContainer.Panel2.Controls.Add(this.grpDetail);
            this.splitContainer.Panel2.Text = "Panel2";
            this.splitContainer.Size = new System.Drawing.Size(1370, 548);
            this.splitContainer.SplitterPosition = 670;
            this.splitContainer.TabIndex = 0;
            this.splitContainer.Text = "splitContainer";
            // 
            // grpPackage
            // 
            this.grpPackage.Controls.Add(this.layoutControlPackage);
            this.grpPackage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpPackage.Location = new System.Drawing.Point(0, 0);
            this.grpPackage.Name = "grpPackage";
            this.grpPackage.Size = new System.Drawing.Size(670, 548);
            this.grpPackage.TabIndex = 0;
            this.grpPackage.Text = "Danh sách gói dịch vụ";
            // 
            // layoutControlPackage
            // 
            this.layoutControlPackage.Controls.Add(this.txtKeywordPackage);
            this.layoutControlPackage.Controls.Add(this.gridControlPackage);
            this.layoutControlPackage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControlPackage.Location = new System.Drawing.Point(2, 20);
            this.layoutControlPackage.Name = "layoutControlPackage";
            this.layoutControlPackage.Root = this.lcgRootPackage;
            this.layoutControlPackage.Size = new System.Drawing.Size(666, 526);
            this.layoutControlPackage.TabIndex = 0;
            // 
            // txtKeywordPackage
            // 
            this.txtKeywordPackage.Location = new System.Drawing.Point(87, 2);
            this.txtKeywordPackage.Name = "txtKeywordPackage";
            this.txtKeywordPackage.Properties.EditValueChangedDelay = 400;
            this.txtKeywordPackage.Properties.EditValueChangedFiringMode = DevExpress.XtraEditors.Controls.EditValueChangedFiringMode.Buffered;
            this.txtKeywordPackage.Properties.NullValuePrompt = "Tên gói...";
            this.txtKeywordPackage.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtKeywordPackage.Properties.ShowNullValuePromptWhenFocused = true;
            this.txtKeywordPackage.Size = new System.Drawing.Size(577, 20);
            this.txtKeywordPackage.StyleController = this.layoutControlPackage;
            this.txtKeywordPackage.TabIndex = 0;
            this.txtKeywordPackage.EditValueChanged += new System.EventHandler(this.txtKeywordPackage_EditValueChanged);
            // 
            // gridControlPackage
            // 
            this.gridControlPackage.Location = new System.Drawing.Point(2, 28);
            this.gridControlPackage.MainView = this.gridViewPackage;
            this.gridControlPackage.Name = "gridControlPackage";
            this.gridControlPackage.Size = new System.Drawing.Size(662, 496);
            this.gridControlPackage.TabIndex = 1;
            this.gridControlPackage.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewPackage});
            // 
            // gridViewPackage
            // 
            this.gridViewPackage.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colPackageName,
            this.colRegisterDate,
            this.colNote,
            this.colCreateTime,
            this.colCreator,
            this.colModifyTime,
            this.colModifier});
            this.gridViewPackage.GridControl = this.gridControlPackage;
            this.gridViewPackage.Name = "gridViewPackage";
            this.gridViewPackage.OptionsBehavior.Editable = false;
            this.gridViewPackage.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridViewPackage.OptionsView.ColumnAutoWidth = false;
            this.gridViewPackage.OptionsView.RowAutoHeight = true;
            this.gridViewPackage.OptionsView.ShowGroupPanel = false;
            this.gridViewPackage.OptionsView.ShowIndicator = false;
            this.gridViewPackage.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.gridViewPackage_FocusedRowChanged);
            this.gridViewPackage.CustomColumnDisplayText += new DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventHandler(this.gridViewPackage_CustomColumnDisplayText);
            // 
            // colPackageName
            // 
            this.colPackageName.Caption = "Tên gói";
            this.colPackageName.FieldName = "PACKAGE_NAME";
            this.colPackageName.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.colPackageName.Name = "colPackageName";
            this.colPackageName.OptionsColumn.AllowEdit = false;
            this.colPackageName.OptionsColumn.ReadOnly = true;
            this.colPackageName.Visible = true;
            this.colPackageName.VisibleIndex = 0;
            this.colPackageName.Width = 180;
            // 
            // colRegisterDate
            // 
            this.colRegisterDate.Caption = "Ngày ĐK";
            this.colRegisterDate.FieldName = "REGISTER_DATE";
            this.colRegisterDate.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.colRegisterDate.Name = "colRegisterDate";
            this.colRegisterDate.OptionsColumn.AllowEdit = false;
            this.colRegisterDate.OptionsColumn.ReadOnly = true;
            this.colRegisterDate.Visible = true;
            this.colRegisterDate.VisibleIndex = 1;
            this.colRegisterDate.Width = 106;
            // 
            // colNote
            // 
            this.colNote.Caption = "Ghi chú";
            this.colNote.FieldName = "NOTE";
            this.colNote.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.colNote.Name = "colNote";
            this.colNote.OptionsColumn.AllowEdit = false;
            this.colNote.OptionsColumn.ReadOnly = true;
            this.colNote.Visible = true;
            this.colNote.VisibleIndex = 2;
            this.colNote.Width = 180;
            // 
            // colCreateTime
            // 
            this.colCreateTime.Caption = "Ngày tạo";
            this.colCreateTime.FieldName = "CREATE_TIME";
            this.colCreateTime.Name = "colCreateTime";
            this.colCreateTime.OptionsColumn.AllowEdit = false;
            this.colCreateTime.OptionsColumn.ReadOnly = true;
            this.colCreateTime.Visible = true;
            this.colCreateTime.VisibleIndex = 3;
            this.colCreateTime.Width = 114;
            // 
            // colCreator
            // 
            this.colCreator.Caption = "Người tạo";
            this.colCreator.FieldName = "CREATOR";
            this.colCreator.Name = "colCreator";
            this.colCreator.OptionsColumn.AllowEdit = false;
            this.colCreator.OptionsColumn.ReadOnly = true;
            this.colCreator.Visible = true;
            this.colCreator.VisibleIndex = 4;
            this.colCreator.Width = 140;
            // 
            // colModifyTime
            // 
            this.colModifyTime.Caption = "Ngày sửa";
            this.colModifyTime.FieldName = "MODIFY_TIME";
            this.colModifyTime.Name = "colModifyTime";
            this.colModifyTime.OptionsColumn.AllowEdit = false;
            this.colModifyTime.OptionsColumn.ReadOnly = true;
            this.colModifyTime.Visible = true;
            this.colModifyTime.VisibleIndex = 5;
            this.colModifyTime.Width = 130;
            // 
            // colModifier
            // 
            this.colModifier.Caption = "Người sửa";
            this.colModifier.FieldName = "MODIFIER";
            this.colModifier.Name = "colModifier";
            this.colModifier.OptionsColumn.AllowEdit = false;
            this.colModifier.OptionsColumn.ReadOnly = true;
            this.colModifier.Visible = true;
            this.colModifier.VisibleIndex = 6;
            this.colModifier.Width = 140;
            // 
            // lcgRootPackage
            // 
            this.lcgRootPackage.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.lcgRootPackage.GroupBordersVisible = false;
            this.lcgRootPackage.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciKeywordPackage,
            this.lciGridPackage});
            this.lcgRootPackage.Location = new System.Drawing.Point(0, 0);
            this.lcgRootPackage.Name = "lcgRootPackage";
            this.lcgRootPackage.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.lcgRootPackage.Size = new System.Drawing.Size(666, 526);
            this.lcgRootPackage.TextVisible = false;
            // 
            // lciKeywordPackage
            // 
            this.lciKeywordPackage.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciKeywordPackage.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciKeywordPackage.Control = this.txtKeywordPackage;
            this.lciKeywordPackage.Location = new System.Drawing.Point(0, 0);
            this.lciKeywordPackage.MaxSize = new System.Drawing.Size(0, 26);
            this.lciKeywordPackage.MinSize = new System.Drawing.Size(120, 26);
            this.lciKeywordPackage.Name = "lciKeywordPackage";
            this.lciKeywordPackage.Size = new System.Drawing.Size(666, 26);
            this.lciKeywordPackage.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciKeywordPackage.Text = "Tìm kiếm:";
            this.lciKeywordPackage.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciKeywordPackage.TextSize = new System.Drawing.Size(80, 20);
            this.lciKeywordPackage.TextToControlDistance = 5;
            // 
            // lciGridPackage
            // 
            this.lciGridPackage.Control = this.gridControlPackage;
            this.lciGridPackage.Location = new System.Drawing.Point(0, 26);
            this.lciGridPackage.Name = "lciGridPackage";
            this.lciGridPackage.Size = new System.Drawing.Size(666, 500);
            this.lciGridPackage.TextSize = new System.Drawing.Size(0, 0);
            this.lciGridPackage.TextVisible = false;
            // 
            // grpDetail
            // 
            this.grpDetail.Controls.Add(this.layoutControlDetail);
            this.grpDetail.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpDetail.Location = new System.Drawing.Point(0, 0);
            this.grpDetail.Name = "grpDetail";
            this.grpDetail.Size = new System.Drawing.Size(695, 548);
            this.grpDetail.TabIndex = 0;
            this.grpDetail.Text = "Dịch vụ trong gói";
            // 
            // layoutControlDetail
            // 
            this.layoutControlDetail.Controls.Add(this.txtKeywordDetail);
            this.layoutControlDetail.Controls.Add(this.gridControlDetail);
            this.layoutControlDetail.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControlDetail.Location = new System.Drawing.Point(2, 20);
            this.layoutControlDetail.Name = "layoutControlDetail";
            this.layoutControlDetail.Root = this.lcgRootDetail;
            this.layoutControlDetail.Size = new System.Drawing.Size(691, 526);
            this.layoutControlDetail.TabIndex = 0;
            // 
            // txtKeywordDetail
            // 
            this.txtKeywordDetail.Location = new System.Drawing.Point(87, 2);
            this.txtKeywordDetail.Name = "txtKeywordDetail";
            this.txtKeywordDetail.Properties.EditValueChangedDelay = 400;
            this.txtKeywordDetail.Properties.EditValueChangedFiringMode = DevExpress.XtraEditors.Controls.EditValueChangedFiringMode.Buffered;
            this.txtKeywordDetail.Properties.NullValuePrompt = "Mã / tên dịch vụ...";
            this.txtKeywordDetail.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtKeywordDetail.Properties.ShowNullValuePromptWhenFocused = true;
            this.txtKeywordDetail.Size = new System.Drawing.Size(602, 20);
            this.txtKeywordDetail.StyleController = this.layoutControlDetail;
            this.txtKeywordDetail.TabIndex = 0;
            this.txtKeywordDetail.EditValueChanged += new System.EventHandler(this.txtKeywordDetail_EditValueChanged);
            //
            // gridControlDetail
            //
            this.gridControlDetail.Location = new System.Drawing.Point(2, 28);
            this.gridControlDetail.MainView = this.gridViewDetail;
            this.gridControlDetail.Name = "gridControlDetail";
            this.gridControlDetail.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repoCheck,
            this.repoAmountThisTime});
            this.gridControlDetail.Size = new System.Drawing.Size(687, 496);
            this.gridControlDetail.TabIndex = 1;
            this.gridControlDetail.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewDetail});
            // 
            // gridViewDetail
            // 
            this.gridViewDetail.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colCheck,
            this.colServiceCode,
            this.colServiceName,
            this.colServiceTypeName,
            this.colAmount,
            this.colAmountUsed,
            this.colAmountThisTime});
            this.gridViewDetail.GridControl = this.gridControlDetail;
            this.gridViewDetail.Name = "gridViewDetail";
            this.gridViewDetail.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridViewDetail.OptionsView.RowAutoHeight = true;
            this.gridViewDetail.OptionsView.ShowGroupPanel = false;
            this.gridViewDetail.OptionsView.ShowIndicator = false;
            this.gridViewDetail.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gridViewDetail_CellValueChanged);
            this.gridViewDetail.CustomDrawColumnHeader += new DevExpress.XtraGrid.Views.Grid.ColumnHeaderCustomDrawEventHandler(this.gridViewDetail_CustomDrawColumnHeader);
            this.gridViewDetail.Click += new System.EventHandler(this.gridViewDetail_Click);
            // 
            // colCheck
            // 
            this.colCheck.Caption = " ";
            this.colCheck.ColumnEdit = this.repoCheck;
            this.colCheck.FieldName = "IS_CHECKED";
            this.colCheck.Name = "colCheck";
            this.colCheck.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.colCheck.Visible = true;
            this.colCheck.VisibleIndex = 0;
            this.colCheck.Width = 30;
            // 
            // repoCheck
            // 
            this.repoCheck.AutoHeight = false;
            this.repoCheck.Name = "repoCheck";
            // 
            // colServiceCode
            // 
            this.colServiceCode.Caption = "Mã DV";
            this.colServiceCode.FieldName = "SERVICE_CODE";
            this.colServiceCode.Name = "colServiceCode";
            this.colServiceCode.OptionsColumn.AllowEdit = false;
            this.colServiceCode.OptionsColumn.ReadOnly = true;
            this.colServiceCode.Visible = true;
            this.colServiceCode.VisibleIndex = 1;
            this.colServiceCode.Width = 80;
            // 
            // colServiceName
            // 
            this.colServiceName.Caption = "Tên dịch vụ";
            this.colServiceName.FieldName = "SERVICE_NAME";
            this.colServiceName.Name = "colServiceName";
            this.colServiceName.OptionsColumn.AllowEdit = false;
            this.colServiceName.OptionsColumn.ReadOnly = true;
            this.colServiceName.Visible = true;
            this.colServiceName.VisibleIndex = 2;
            this.colServiceName.Width = 280;
            // 
            // colServiceTypeName
            // 
            this.colServiceTypeName.Caption = "Loại DV";
            this.colServiceTypeName.FieldName = "SERVICE_TYPE_NAME";
            this.colServiceTypeName.Name = "colServiceTypeName";
            this.colServiceTypeName.OptionsColumn.AllowEdit = false;
            this.colServiceTypeName.OptionsColumn.ReadOnly = true;
            this.colServiceTypeName.Visible = true;
            this.colServiceTypeName.VisibleIndex = 3;
            this.colServiceTypeName.Width = 90;
            // 
            // colAmount
            // 
            this.colAmount.AppearanceCell.Options.UseTextOptions = true;
            this.colAmount.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.colAmount.Caption = "Trong gói";
            this.colAmount.DisplayFormat.FormatString = "{0:n0}";
            this.colAmount.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.colAmount.FieldName = "AMOUNT";
            this.colAmount.Name = "colAmount";
            this.colAmount.OptionsColumn.AllowEdit = false;
            this.colAmount.OptionsColumn.ReadOnly = true;
            this.colAmount.Visible = true;
            this.colAmount.VisibleIndex = 4;
            this.colAmount.Width = 70;
            // 
            // colAmountUsed
            // 
            this.colAmountUsed.AppearanceCell.Options.UseTextOptions = true;
            this.colAmountUsed.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.colAmountUsed.Caption = "Đã dùng";
            this.colAmountUsed.DisplayFormat.FormatString = "{0:n0}";
            this.colAmountUsed.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.colAmountUsed.FieldName = "AMOUNT_USED";
            this.colAmountUsed.Name = "colAmountUsed";
            this.colAmountUsed.OptionsColumn.AllowEdit = false;
            this.colAmountUsed.OptionsColumn.ReadOnly = true;
            this.colAmountUsed.Visible = true;
            this.colAmountUsed.VisibleIndex = 5;
            this.colAmountUsed.Width = 70;
            // 
            // colAmountThisTime
            // 
            this.colAmountThisTime.AppearanceCell.Options.UseTextOptions = true;
            this.colAmountThisTime.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.colAmountThisTime.Caption = "Lần này";
            this.colAmountThisTime.ColumnEdit = this.repoAmountThisTime;
            this.colAmountThisTime.FieldName = "AMOUNT_THIS_TIME";
            this.colAmountThisTime.Name = "colAmountThisTime";
            this.colAmountThisTime.Visible = true;
            this.colAmountThisTime.VisibleIndex = 6;
            this.colAmountThisTime.Width = 70;
            // 
            // repoAmountThisTime
            // 
            this.repoAmountThisTime.AutoHeight = false;
            this.repoAmountThisTime.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Up),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Down)});
            this.repoAmountThisTime.DisplayFormat.FormatString = "{0:n0}";
            this.repoAmountThisTime.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.repoAmountThisTime.EditFormat.FormatString = "{0:n0}";
            this.repoAmountThisTime.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.repoAmountThisTime.MaxValue = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.repoAmountThisTime.Name = "repoAmountThisTime";
            // 
            // lcgRootDetail
            // 
            this.lcgRootDetail.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.lcgRootDetail.GroupBordersVisible = false;
            this.lcgRootDetail.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciKeywordDetail,
            this.lciGridDetail});
            this.lcgRootDetail.Location = new System.Drawing.Point(0, 0);
            this.lcgRootDetail.Name = "lcgRootDetail";
            this.lcgRootDetail.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.lcgRootDetail.Size = new System.Drawing.Size(691, 526);
            this.lcgRootDetail.TextVisible = false;
            // 
            // lciKeywordDetail
            // 
            this.lciKeywordDetail.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciKeywordDetail.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciKeywordDetail.Control = this.txtKeywordDetail;
            this.lciKeywordDetail.Location = new System.Drawing.Point(0, 0);
            this.lciKeywordDetail.MaxSize = new System.Drawing.Size(0, 26);
            this.lciKeywordDetail.MinSize = new System.Drawing.Size(120, 26);
            this.lciKeywordDetail.Name = "lciKeywordDetail";
            this.lciKeywordDetail.Size = new System.Drawing.Size(691, 26);
            this.lciKeywordDetail.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciKeywordDetail.Text = "Tìm kiếm:";
            this.lciKeywordDetail.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciKeywordDetail.TextSize = new System.Drawing.Size(80, 20);
            this.lciKeywordDetail.TextToControlDistance = 5;
            //
            // lciGridDetail
            //
            this.lciGridDetail.Control = this.gridControlDetail;
            this.lciGridDetail.Location = new System.Drawing.Point(0, 26);
            this.lciGridDetail.Name = "lciGridDetail";
            this.lciGridDetail.Size = new System.Drawing.Size(691, 500);
            this.lciGridDetail.TextSize = new System.Drawing.Size(0, 0);
            this.lciGridDetail.TextVisible = false;
            // 
            // layoutControlBottom
            // 
            this.layoutControlBottom.Controls.Add(this.btnChoose);
            this.layoutControlBottom.Controls.Add(this.btnCancel);
            this.layoutControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.layoutControlBottom.Location = new System.Drawing.Point(0, 548);
            this.layoutControlBottom.Name = "layoutControlBottom";
            this.layoutControlBottom.Root = this.lcgRootBottom;
            this.layoutControlBottom.Size = new System.Drawing.Size(1370, 32);
            this.layoutControlBottom.TabIndex = 1;
            // 
            // btnChoose
            // 
            this.btnChoose.Location = new System.Drawing.Point(1209, 2);
            this.btnChoose.Name = "btnChoose";
            this.btnChoose.Size = new System.Drawing.Size(80, 20);
            this.btnChoose.StyleController = this.layoutControlBottom;
            this.btnChoose.TabIndex = 0;
            this.btnChoose.Text = "Chọn";
            this.btnChoose.Click += new System.EventHandler(this.btnChoose_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(1293, 2);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 20);
            this.btnCancel.StyleController = this.layoutControlBottom;
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Hủy bỏ";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lcgRootBottom
            // 
            this.lcgRootBottom.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.lcgRootBottom.GroupBordersVisible = false;
            this.lcgRootBottom.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.emptySpaceBottom,
            this.lciBtnChoose,
            this.lciBtnCancel});
            this.lcgRootBottom.Location = new System.Drawing.Point(0, 0);
            this.lcgRootBottom.Name = "lcgRootBottom";
            this.lcgRootBottom.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.lcgRootBottom.Size = new System.Drawing.Size(1370, 32);
            this.lcgRootBottom.TextVisible = false;
            // 
            // emptySpaceBottom
            // 
            this.emptySpaceBottom.AllowHotTrack = false;
            this.emptySpaceBottom.Location = new System.Drawing.Point(0, 0);
            this.emptySpaceBottom.Name = "emptySpaceBottom";
            this.emptySpaceBottom.Size = new System.Drawing.Size(1207, 32);
            this.emptySpaceBottom.TextSize = new System.Drawing.Size(0, 0);
            // 
            // lciBtnChoose
            // 
            this.lciBtnChoose.Control = this.btnChoose;
            this.lciBtnChoose.Location = new System.Drawing.Point(1207, 0);
            this.lciBtnChoose.MaxSize = new System.Drawing.Size(84, 24);
            this.lciBtnChoose.MinSize = new System.Drawing.Size(84, 24);
            this.lciBtnChoose.Name = "lciBtnChoose";
            this.lciBtnChoose.Size = new System.Drawing.Size(84, 32);
            this.lciBtnChoose.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciBtnChoose.TextSize = new System.Drawing.Size(0, 0);
            this.lciBtnChoose.TextVisible = false;
            // 
            // lciBtnCancel
            // 
            this.lciBtnCancel.Control = this.btnCancel;
            this.lciBtnCancel.Location = new System.Drawing.Point(1291, 0);
            this.lciBtnCancel.MaxSize = new System.Drawing.Size(79, 24);
            this.lciBtnCancel.MinSize = new System.Drawing.Size(79, 24);
            this.lciBtnCancel.Name = "lciBtnCancel";
            this.lciBtnCancel.Size = new System.Drawing.Size(79, 32);
            this.lciBtnCancel.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciBtnCancel.TextSize = new System.Drawing.Size(0, 0);
            this.lciBtnCancel.TextVisible = false;
            // 
            // frmPatientPackagePicker
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1370, 580);
            this.Controls.Add(this.splitContainer);
            this.Controls.Add(this.layoutControlBottom);
            this.KeyPreview = true;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(900, 450);
            this.Name = "frmPatientPackagePicker";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Chọn dịch vụ trong gói";
            this.Load += new System.EventHandler(this.frmPatientPackagePicker_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmPatientPackagePicker_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grpPackage)).EndInit();
            this.grpPackage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlPackage)).EndInit();
            this.layoutControlPackage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtKeywordPackage.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlPackage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewPackage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcgRootPackage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciKeywordPackage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciGridPackage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpDetail)).EndInit();
            this.grpDetail.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlDetail)).EndInit();
            this.layoutControlDetail.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtKeywordDetail.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlDetail)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewDetail)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoCheck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoAmountThisTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcgRootDetail)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciKeywordDetail)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciGridDetail)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlBottom)).EndInit();
            this.layoutControlBottom.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.lcgRootBottom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceBottom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnChoose)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnCancel)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.SplitContainerControl splitContainer;
        private DevExpress.XtraEditors.GroupControl grpPackage;
        private DevExpress.XtraEditors.GroupControl grpDetail;
        private DevExpress.XtraLayout.LayoutControl layoutControlPackage;
        private DevExpress.XtraLayout.LayoutControlGroup lcgRootPackage;
        private DevExpress.XtraLayout.LayoutControlItem lciKeywordPackage;
        private DevExpress.XtraLayout.LayoutControlItem lciGridPackage;
        private DevExpress.XtraLayout.LayoutControl layoutControlDetail;
        private DevExpress.XtraLayout.LayoutControlGroup lcgRootDetail;
        private DevExpress.XtraLayout.LayoutControlItem lciKeywordDetail;
        private DevExpress.XtraLayout.LayoutControlItem lciGridDetail;
        private DevExpress.XtraLayout.LayoutControl layoutControlBottom;
        private DevExpress.XtraLayout.LayoutControlGroup lcgRootBottom;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceBottom;
        private DevExpress.XtraLayout.LayoutControlItem lciBtnChoose;
        private DevExpress.XtraLayout.LayoutControlItem lciBtnCancel;
        private DevExpress.XtraGrid.GridControl gridControlPackage;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewPackage;
        private DevExpress.XtraGrid.Columns.GridColumn colPackageName;
        private DevExpress.XtraGrid.Columns.GridColumn colRegisterDate;
        private DevExpress.XtraGrid.Columns.GridColumn colNote;
        private DevExpress.XtraGrid.Columns.GridColumn colCreateTime;
        private DevExpress.XtraGrid.Columns.GridColumn colCreator;
        private DevExpress.XtraGrid.Columns.GridColumn colModifyTime;
        private DevExpress.XtraGrid.Columns.GridColumn colModifier;
        private DevExpress.XtraEditors.TextEdit txtKeywordPackage;
        private DevExpress.XtraGrid.GridControl gridControlDetail;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewDetail;
        private DevExpress.XtraGrid.Columns.GridColumn colCheck;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repoCheck;
        private DevExpress.XtraGrid.Columns.GridColumn colServiceCode;
        private DevExpress.XtraGrid.Columns.GridColumn colServiceName;
        private DevExpress.XtraGrid.Columns.GridColumn colServiceTypeName;
        private DevExpress.XtraGrid.Columns.GridColumn colAmount;
        private DevExpress.XtraGrid.Columns.GridColumn colAmountUsed;
        private DevExpress.XtraGrid.Columns.GridColumn colAmountThisTime;
        private DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit repoAmountThisTime;
        private DevExpress.XtraEditors.TextEdit txtKeywordDetail;
        private DevExpress.XtraEditors.SimpleButton btnChoose;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
    }
}
