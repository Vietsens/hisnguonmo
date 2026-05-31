/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 */
namespace HIS.Desktop.Plugins.HisServiceConsult
{
    partial class frmHisServiceConsult
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
            this.components = new System.ComponentModel.Container();
            this.dxErrorProvider1 = new DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider(this.components);
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.btnSave = new DevExpress.XtraEditors.SimpleButton();
            this.btnReset = new DevExpress.XtraEditors.SimpleButton();
            this.txtDescription = new DevExpress.XtraEditors.MemoEdit();
            this.txtReason = new DevExpress.XtraEditors.MemoEdit();
            this.dteConsultTime = new DevExpress.XtraEditors.DateEdit();
            this.cboResultType = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridViewResultType = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gcResultCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcResultName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.cboConsultantUser = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridViewConsultant = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gcLoginname = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcUsername = new DevExpress.XtraGrid.Columns.GridColumn();
            this.txtConsultantLoginname = new DevExpress.XtraEditors.TextEdit();
            this.gridControlPackage = new DevExpress.XtraGrid.GridControl();
            this.gridViewPackage = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gcChk = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repoChk = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.gcStt = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcPackageCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcPackageName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.txtKeyword = new DevExpress.XtraEditors.TextEdit();
            this.layoutControlGroupRoot = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciKeyword = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciGrid = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciConsultantLoginname = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciConsultant = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciResultType = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciConsultTime = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciReason = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciDescription = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciBtnReset = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciBtnSave = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtDescription.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtReason.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteConsultTime.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteConsultTime.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboResultType.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewResultType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboConsultantUser.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewConsultant)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtConsultantLoginname.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlPackage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewPackage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoChk)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtKeyword.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupRoot)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciKeyword)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciConsultantLoginname)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciConsultant)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciResultType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciConsultTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciReason)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDescription)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnReset)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnSave)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxErrorProvider1)).BeginInit();
            this.SuspendLayout();
            //
            // layoutControl1
            //
            this.layoutControl1.Controls.Add(this.btnSave);
            this.layoutControl1.Controls.Add(this.btnReset);
            this.layoutControl1.Controls.Add(this.txtDescription);
            this.layoutControl1.Controls.Add(this.txtReason);
            this.layoutControl1.Controls.Add(this.dteConsultTime);
            this.layoutControl1.Controls.Add(this.cboResultType);
            this.layoutControl1.Controls.Add(this.cboConsultantUser);
            this.layoutControl1.Controls.Add(this.txtConsultantLoginname);
            this.layoutControl1.Controls.Add(this.gridControlPackage);
            this.layoutControl1.Controls.Add(this.txtKeyword);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroupRoot;
            this.layoutControl1.Size = new System.Drawing.Size(794, 671);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            //
            // btnSave
            //
            this.btnSave.Location = new System.Drawing.Point(685, 644);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(97, 22);
            this.btnSave.StyleController = this.layoutControl1;
            this.btnSave.TabIndex = 9;
            this.btnSave.Text = "Lưu (Ctrl S)";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            //
            // btnReset
            //
            this.btnReset.Location = new System.Drawing.Point(584, 644);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(97, 22);
            this.btnReset.StyleController = this.layoutControl1;
            this.btnReset.TabIndex = 8;
            this.btnReset.Text = "Làm lại (Ctrl R)";
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            //
            // txtDescription
            //
            this.txtDescription.Location = new System.Drawing.Point(83, 568);
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(699, 72);
            this.txtDescription.StyleController = this.layoutControl1;
            this.txtDescription.TabIndex = 7;
            //
            // txtReason
            //
            this.txtReason.Location = new System.Drawing.Point(83, 492);
            this.txtReason.Name = "txtReason";
            this.txtReason.Size = new System.Drawing.Size(699, 72);
            this.txtReason.StyleController = this.layoutControl1;
            this.txtReason.TabIndex = 6;
            //
            // dteConsultTime
            //
            this.dteConsultTime.EditValue = null;
            this.dteConsultTime.Location = new System.Drawing.Point(572, 468);
            this.dteConsultTime.Name = "dteConsultTime";
            this.dteConsultTime.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dteConsultTime.Properties.DisplayFormat.FormatString = "dd/MM/yyyy HH:mm";
            this.dteConsultTime.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.dteConsultTime.Properties.EditFormat.FormatString = "dd/MM/yyyy HH:mm";
            this.dteConsultTime.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.dteConsultTime.Properties.Mask.EditMask = "dd/MM/yyyy HH:mm";
            this.dteConsultTime.Properties.NullDate = "";
            this.dteConsultTime.Size = new System.Drawing.Size(210, 20);
            this.dteConsultTime.StyleController = this.layoutControl1;
            this.dteConsultTime.TabIndex = 5;
            //
            // cboResultType
            //
            this.cboResultType.Location = new System.Drawing.Point(310, 468);
            this.cboResultType.Name = "cboResultType";
            this.cboResultType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboResultType.Properties.NullText = "";
            this.cboResultType.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
            this.cboResultType.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
            this.cboResultType.Properties.View = this.gridViewResultType;
            this.cboResultType.Size = new System.Drawing.Size(152, 20);
            this.cboResultType.StyleController = this.layoutControl1;
            this.cboResultType.TabIndex = 4;
            //
            // gridViewResultType
            //
            this.gridViewResultType.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gcResultCode,
            this.gcResultName});
            this.gridViewResultType.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridViewResultType.Name = "gridViewResultType";
            this.gridViewResultType.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridViewResultType.OptionsView.ShowGroupPanel = false;
            //
            // gcResultCode
            //
            this.gcResultCode.Caption = "Mã";
            this.gcResultCode.FieldName = "CONSULT_RESULT_TYPE_CODE";
            this.gcResultCode.Name = "gcResultCode";
            this.gcResultCode.Visible = true;
            this.gcResultCode.VisibleIndex = 0;
            this.gcResultCode.Width = 70;
            //
            // gcResultName
            //
            this.gcResultName.Caption = "Tên hiển thị";
            this.gcResultName.FieldName = "CONSULT_RESULT_TYPE_NAME";
            this.gcResultName.Name = "gcResultName";
            this.gcResultName.Visible = true;
            this.gcResultName.VisibleIndex = 1;
            this.gcResultName.Width = 200;
            //
            // cboConsultantUser
            //
            this.cboConsultantUser.Location = new System.Drawing.Point(155, 468);
            this.cboConsultantUser.Name = "cboConsultantUser";
            this.cboConsultantUser.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboConsultantUser.Properties.NullText = "";
            this.cboConsultantUser.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
            this.cboConsultantUser.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
            this.cboConsultantUser.Properties.View = this.gridViewConsultant;
            this.cboConsultantUser.Size = new System.Drawing.Size(110, 20);
            this.cboConsultantUser.StyleController = this.layoutControl1;
            this.cboConsultantUser.TabIndex = 2;
            this.cboConsultantUser.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboConsultantUser_Closed);
            this.cboConsultantUser.KeyUp += new System.Windows.Forms.KeyEventHandler(this.cboConsultantUser_KeyUp);
            //
            // gridViewConsultant
            //
            this.gridViewConsultant.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gcLoginname,
            this.gcUsername});
            this.gridViewConsultant.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridViewConsultant.Name = "gridViewConsultant";
            this.gridViewConsultant.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridViewConsultant.OptionsView.ShowGroupPanel = false;
            //
            // gcLoginname
            //
            this.gcLoginname.Caption = "Tên đăng nhập";
            this.gcLoginname.FieldName = "LOGINNAME";
            this.gcLoginname.Name = "gcLoginname";
            this.gcLoginname.Visible = true;
            this.gcLoginname.VisibleIndex = 0;
            this.gcLoginname.Width = 110;
            //
            // gcUsername
            //
            this.gcUsername.Caption = "Họ tên";
            this.gcUsername.FieldName = "USERNAME";
            this.gcUsername.Name = "gcUsername";
            this.gcUsername.Visible = true;
            this.gcUsername.VisibleIndex = 1;
            this.gcUsername.Width = 180;
            //
            // txtConsultantLoginname
            //
            this.txtConsultantLoginname.Location = new System.Drawing.Point(83, 468);
            this.txtConsultantLoginname.Name = "txtConsultantLoginname";
            this.txtConsultantLoginname.Properties.ReadOnly = true;
            this.txtConsultantLoginname.Size = new System.Drawing.Size(68, 20);
            this.txtConsultantLoginname.StyleController = this.layoutControl1;
            this.txtConsultantLoginname.TabIndex = 1;
            //
            // gridControlPackage
            //
            this.gridControlPackage.Location = new System.Drawing.Point(12, 36);
            this.gridControlPackage.MainView = this.gridViewPackage;
            this.gridControlPackage.Name = "gridControlPackage";
            this.gridControlPackage.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repoChk});
            this.gridControlPackage.Size = new System.Drawing.Size(770, 416);
            this.gridControlPackage.TabIndex = 0;
            this.gridControlPackage.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewPackage});
            //
            // gridViewPackage
            //
            this.gridViewPackage.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gcChk,
            this.gcStt,
            this.gcPackageCode,
            this.gcPackageName});
            this.gridViewPackage.GridControl = this.gridControlPackage;
            this.gridViewPackage.Name = "gridViewPackage";
            this.gridViewPackage.OptionsBehavior.Editable = true;
            this.gridViewPackage.OptionsCustomization.AllowSort = false;
            this.gridViewPackage.OptionsView.ShowGroupPanel = false;
            this.gridViewPackage.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridViewPackage.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewPackage_CustomUnboundColumnData);
            //
            // gcChk
            //
            this.gcChk.Caption = " ";
            this.gcChk.ColumnEdit = this.repoChk;
            this.gcChk.FieldName = "IS_CHECKED";
            this.gcChk.Name = "gcChk";
            this.gcChk.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.gcChk.Visible = true;
            this.gcChk.VisibleIndex = 0;
            this.gcChk.Width = 36;
            //
            // repoChk
            //
            this.repoChk.AutoHeight = false;
            this.repoChk.Name = "repoChk";
            //
            // gcStt
            //
            this.gcStt.Caption = "STT";
            this.gcStt.FieldName = "STT";
            this.gcStt.Name = "gcStt";
            this.gcStt.OptionsColumn.AllowEdit = false;
            this.gcStt.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.gcStt.UnboundType = DevExpress.Data.UnboundColumnType.Integer;
            this.gcStt.Visible = true;
            this.gcStt.VisibleIndex = 1;
            this.gcStt.Width = 50;
            //
            // gcPackageCode
            //
            this.gcPackageCode.Caption = "Mã gói";
            this.gcPackageCode.FieldName = "PACKAGE_CODE";
            this.gcPackageCode.Name = "gcPackageCode";
            this.gcPackageCode.OptionsColumn.AllowEdit = false;
            this.gcPackageCode.Visible = true;
            this.gcPackageCode.VisibleIndex = 2;
            this.gcPackageCode.Width = 120;
            //
            // gcPackageName
            //
            this.gcPackageName.Caption = "Tên gói";
            this.gcPackageName.FieldName = "PACKAGE_NAME";
            this.gcPackageName.Name = "gcPackageName";
            this.gcPackageName.OptionsColumn.AllowEdit = false;
            this.gcPackageName.Visible = true;
            this.gcPackageName.VisibleIndex = 3;
            this.gcPackageName.Width = 540;
            //
            // txtKeyword
            //
            this.txtKeyword.Location = new System.Drawing.Point(12, 12);
            this.txtKeyword.Name = "txtKeyword";
            this.txtKeyword.Properties.NullValuePrompt = "Từ khóa tìm kiếm";
            this.txtKeyword.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtKeyword.Size = new System.Drawing.Size(770, 20);
            this.txtKeyword.StyleController = this.layoutControl1;
            this.txtKeyword.TabIndex = 0;
            this.txtKeyword.EditValueChanged += new System.EventHandler(this.txtKeyword_EditValueChanged);
            //
            // layoutControlGroupRoot
            //
            this.layoutControlGroupRoot.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroupRoot.GroupBordersVisible = false;
            this.layoutControlGroupRoot.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciKeyword,
            this.lciGrid,
            this.lciConsultantLoginname,
            this.lciConsultant,
            this.lciResultType,
            this.lciConsultTime,
            this.lciReason,
            this.lciDescription,
            this.lciBtnReset,
            this.lciBtnSave,
            this.emptySpaceItem1});
            this.layoutControlGroupRoot.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroupRoot.Name = "layoutControlGroupRoot";
            this.layoutControlGroupRoot.Size = new System.Drawing.Size(794, 671);
            this.layoutControlGroupRoot.TextVisible = false;
            //
            // lciKeyword
            //
            this.lciKeyword.Control = this.txtKeyword;
            this.lciKeyword.Location = new System.Drawing.Point(0, 0);
            this.lciKeyword.Name = "lciKeyword";
            this.lciKeyword.Size = new System.Drawing.Size(774, 24);
            this.lciKeyword.TextSize = new System.Drawing.Size(0, 0);
            this.lciKeyword.TextVisible = false;
            //
            // lciGrid
            //
            this.lciGrid.Control = this.gridControlPackage;
            this.lciGrid.Location = new System.Drawing.Point(0, 24);
            this.lciGrid.Name = "lciGrid";
            this.lciGrid.Size = new System.Drawing.Size(774, 420);
            this.lciGrid.TextSize = new System.Drawing.Size(0, 0);
            this.lciGrid.TextVisible = false;
            //
            // lciConsultantLoginname
            //
            this.lciConsultantLoginname.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.lciConsultantLoginname.AppearanceItemCaption.Options.UseForeColor = true;
            this.lciConsultantLoginname.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciConsultantLoginname.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciConsultantLoginname.Control = this.txtConsultantLoginname;
            this.lciConsultantLoginname.Location = new System.Drawing.Point(0, 444);
            this.lciConsultantLoginname.Name = "lciConsultantLoginname";
            this.lciConsultantLoginname.Size = new System.Drawing.Size(143, 24);
            this.lciConsultantLoginname.Text = "Người tư vấn:";
            this.lciConsultantLoginname.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciConsultantLoginname.TextSize = new System.Drawing.Size(68, 20);
            //
            // lciConsultant
            //
            this.lciConsultant.Control = this.cboConsultantUser;
            this.lciConsultant.Location = new System.Drawing.Point(143, 444);
            this.lciConsultant.Name = "lciConsultant";
            this.lciConsultant.Size = new System.Drawing.Size(114, 24);
            this.lciConsultant.TextSize = new System.Drawing.Size(0, 0);
            this.lciConsultant.TextVisible = false;
            //
            // lciResultType
            //
            this.lciResultType.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.lciResultType.AppearanceItemCaption.Options.UseForeColor = true;
            this.lciResultType.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciResultType.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciResultType.Control = this.cboResultType;
            this.lciResultType.Location = new System.Drawing.Point(257, 444);
            this.lciResultType.Name = "lciResultType";
            this.lciResultType.Size = new System.Drawing.Size(247, 24);
            this.lciResultType.Text = "Kết quả tư vấn:";
            this.lciResultType.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciResultType.TextSize = new System.Drawing.Size(85, 20);
            //
            // lciConsultTime
            //
            this.lciConsultTime.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciConsultTime.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciConsultTime.Control = this.dteConsultTime;
            this.lciConsultTime.Location = new System.Drawing.Point(504, 444);
            this.lciConsultTime.Name = "lciConsultTime";
            this.lciConsultTime.Size = new System.Drawing.Size(270, 24);
            this.lciConsultTime.Text = "Thời gian:";
            this.lciConsultTime.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciConsultTime.TextSize = new System.Drawing.Size(56, 20);
            //
            // lciReason
            //
            this.lciReason.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciReason.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciReason.AppearanceItemCaption.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.lciReason.Control = this.txtReason;
            this.lciReason.Location = new System.Drawing.Point(0, 468);
            this.lciReason.Name = "lciReason";
            this.lciReason.Size = new System.Drawing.Size(774, 76);
            this.lciReason.Text = "Lý do:";
            this.lciReason.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciReason.TextSize = new System.Drawing.Size(68, 20);
            //
            // lciDescription
            //
            this.lciDescription.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciDescription.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciDescription.AppearanceItemCaption.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.lciDescription.Control = this.txtDescription;
            this.lciDescription.Location = new System.Drawing.Point(0, 544);
            this.lciDescription.Name = "lciDescription";
            this.lciDescription.Size = new System.Drawing.Size(774, 76);
            this.lciDescription.Text = "Mô tả:";
            this.lciDescription.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciDescription.TextSize = new System.Drawing.Size(68, 20);
            //
            // lciBtnReset
            //
            this.lciBtnReset.Control = this.btnReset;
            this.lciBtnReset.Location = new System.Drawing.Point(572, 620);
            this.lciBtnReset.Name = "lciBtnReset";
            this.lciBtnReset.Size = new System.Drawing.Size(101, 26);
            this.lciBtnReset.TextSize = new System.Drawing.Size(0, 0);
            this.lciBtnReset.TextVisible = false;
            //
            // lciBtnSave
            //
            this.lciBtnSave.Control = this.btnSave;
            this.lciBtnSave.Location = new System.Drawing.Point(673, 620);
            this.lciBtnSave.Name = "lciBtnSave";
            this.lciBtnSave.Size = new System.Drawing.Size(101, 26);
            this.lciBtnSave.TextSize = new System.Drawing.Size(0, 0);
            this.lciBtnSave.TextVisible = false;
            //
            // emptySpaceItem1
            //
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(0, 620);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(572, 26);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            //
            // frmHisServiceConsult
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(794, 671);
            this.Controls.Add(this.layoutControl1);
            this.KeyPreview = true;
            this.MinimizeBox = false;
            this.Name = "frmHisServiceConsult";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Kết quả tư vấn dịch vụ";
            this.Load += new System.EventHandler(this.frmHisServiceConsult_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmHisServiceConsult_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtDescription.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtReason.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteConsultTime.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteConsultTime.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboResultType.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewResultType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboConsultantUser.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewConsultant)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtConsultantLoginname.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlPackage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewPackage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoChk)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtKeyword.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupRoot)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciKeyword)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciConsultantLoginname)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciConsultant)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciResultType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciConsultTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciReason)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDescription)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnReset)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnSave)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxErrorProvider1)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroupRoot;
        private DevExpress.XtraEditors.TextEdit txtKeyword;
        private DevExpress.XtraGrid.GridControl gridControlPackage;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewPackage;
        private DevExpress.XtraGrid.Columns.GridColumn gcChk;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repoChk;
        private DevExpress.XtraGrid.Columns.GridColumn gcStt;
        private DevExpress.XtraGrid.Columns.GridColumn gcPackageCode;
        private DevExpress.XtraGrid.Columns.GridColumn gcPackageName;
        private DevExpress.XtraEditors.TextEdit txtConsultantLoginname;
        private DevExpress.XtraEditors.GridLookUpEdit cboConsultantUser;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewConsultant;
        private DevExpress.XtraGrid.Columns.GridColumn gcLoginname;
        private DevExpress.XtraGrid.Columns.GridColumn gcUsername;
        private DevExpress.XtraEditors.GridLookUpEdit cboResultType;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewResultType;
        private DevExpress.XtraGrid.Columns.GridColumn gcResultCode;
        private DevExpress.XtraGrid.Columns.GridColumn gcResultName;
        private DevExpress.XtraEditors.DateEdit dteConsultTime;
        private DevExpress.XtraEditors.MemoEdit txtReason;
        private DevExpress.XtraEditors.MemoEdit txtDescription;
        private DevExpress.XtraEditors.SimpleButton btnReset;
        private DevExpress.XtraEditors.SimpleButton btnSave;
        private DevExpress.XtraLayout.LayoutControlItem lciKeyword;
        private DevExpress.XtraLayout.LayoutControlItem lciGrid;
        private DevExpress.XtraLayout.LayoutControlItem lciConsultantLoginname;
        private DevExpress.XtraLayout.LayoutControlItem lciConsultant;
        private DevExpress.XtraLayout.LayoutControlItem lciResultType;
        private DevExpress.XtraLayout.LayoutControlItem lciConsultTime;
        private DevExpress.XtraLayout.LayoutControlItem lciReason;
        private DevExpress.XtraLayout.LayoutControlItem lciDescription;
        private DevExpress.XtraLayout.LayoutControlItem lciBtnReset;
        private DevExpress.XtraLayout.LayoutControlItem lciBtnSave;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        protected DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider dxErrorProvider1;
    }
}
