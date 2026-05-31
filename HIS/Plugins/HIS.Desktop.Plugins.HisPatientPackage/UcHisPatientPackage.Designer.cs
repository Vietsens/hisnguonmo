namespace HIS.Desktop.Plugins.HisPatientPackage
{
    partial class UcHisPatientPackage
    {
        /// <summary> Required designer variable. </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> Clean up any resources being used. </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        private void InitializeComponent()
        {
            this.panelControlLeft = new DevExpress.XtraEditors.PanelControl();
            this.btnRefresh = new DevExpress.XtraEditors.SimpleButton();
            this.btnSearch = new DevExpress.XtraEditors.SimpleButton();
            this.btnNextDate = new DevExpress.XtraEditors.SimpleButton();
            this.btnPrevDate = new DevExpress.XtraEditors.SimpleButton();
            this.btnToggleTime = new DevExpress.XtraEditors.SimpleButton();
            this.dteDate = new DevExpress.XtraEditors.DateEdit();
            this.cboTimeType = new DevExpress.XtraEditors.ComboBoxEdit();
            this.lblTime = new DevExpress.XtraEditors.LabelControl();
            this.txtKeyword = new DevExpress.XtraEditors.TextEdit();
            this.txtPatientCode = new DevExpress.XtraEditors.TextEdit();
            this.panelMain = new DevExpress.XtraEditors.PanelControl();
            this.gridControl = new DevExpress.XtraGrid.GridControl();
            this.gridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colSTT = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colEdit = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colDelete = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colLock = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colPay = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colRefund = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colPrint = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colPatientCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colPatientName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colDob = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colGender = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colPackageName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colStatus = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colAddress = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colCreateTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colCreator = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colModifyTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colModifier = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repoEdit = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.repoDelete = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.repoLock = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.repoUnlock = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.repoPay = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.repoRefund = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.repoPrint = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.repoEmpty = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.ucPaging = new Inventec.UC.Paging.UcPaging();
            ((System.ComponentModel.ISupportInitialize)(this.panelControlLeft)).BeginInit();
            this.panelControlLeft.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dteDate.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteDate.Properties.VistaTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboTimeType.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtKeyword.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPatientCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelMain)).BeginInit();
            this.panelMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoEdit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoDelete)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoLock)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoUnlock)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoPay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoRefund)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoPrint)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoEmpty)).BeginInit();
            this.SuspendLayout();
            //
            // panelControlLeft
            //
            this.panelControlLeft.Controls.Add(this.btnRefresh);
            this.panelControlLeft.Controls.Add(this.btnSearch);
            this.panelControlLeft.Controls.Add(this.btnNextDate);
            this.panelControlLeft.Controls.Add(this.btnPrevDate);
            this.panelControlLeft.Controls.Add(this.btnToggleTime);
            this.panelControlLeft.Controls.Add(this.dteDate);
            this.panelControlLeft.Controls.Add(this.cboTimeType);
            this.panelControlLeft.Controls.Add(this.lblTime);
            this.panelControlLeft.Controls.Add(this.txtKeyword);
            this.panelControlLeft.Controls.Add(this.txtPatientCode);
            this.panelControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelControlLeft.Location = new System.Drawing.Point(0, 0);
            this.panelControlLeft.Name = "panelControlLeft";
            this.panelControlLeft.Size = new System.Drawing.Size(240, 600);
            this.panelControlLeft.TabIndex = 0;
            //
            // btnRefresh
            //
            this.btnRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnRefresh.Location = new System.Drawing.Point(122, 556);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(110, 32);
            this.btnRefresh.TabIndex = 8;
            this.btnRefresh.Text = "Làm lại (Ctrl R)";
            //
            // btnSearch
            //
            this.btnSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSearch.Location = new System.Drawing.Point(8, 556);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(108, 32);
            this.btnSearch.TabIndex = 7;
            this.btnSearch.Text = "Tìm (Ctrl F)";
            //
            // btnNextDate
            //
            this.btnNextDate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            this.btnNextDate.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(102)))), ((int)(((byte)(204)))));
            this.btnNextDate.Appearance.Options.UseForeColor = true;
            this.btnNextDate.Location = new System.Drawing.Point(123, 114);
            this.btnNextDate.Name = "btnNextDate";
            this.btnNextDate.Size = new System.Drawing.Size(75, 26);
            this.btnNextDate.TabIndex = 6;
            this.btnNextDate.Text = "►";
            //
            // btnPrevDate
            //
            this.btnPrevDate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            this.btnPrevDate.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(102)))), ((int)(((byte)(204)))));
            this.btnPrevDate.Appearance.Options.UseForeColor = true;
            this.btnPrevDate.Location = new System.Drawing.Point(42, 114);
            this.btnPrevDate.Name = "btnPrevDate";
            this.btnPrevDate.Size = new System.Drawing.Size(75, 26);
            this.btnPrevDate.TabIndex = 5;
            this.btnPrevDate.Text = "◄";
            //
            // btnToggleTime
            //
            this.btnToggleTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnToggleTime.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(102)))), ((int)(((byte)(204)))));
            this.btnToggleTime.Appearance.Options.UseForeColor = true;
            this.btnToggleTime.Location = new System.Drawing.Point(208, 63);
            this.btnToggleTime.Name = "btnToggleTime";
            this.btnToggleTime.Size = new System.Drawing.Size(24, 18);
            this.btnToggleTime.TabIndex = 9;
            this.btnToggleTime.Text = "▲";
            //
            // dteDate
            //
            this.dteDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dteDate.EditValue = null;
            this.dteDate.Location = new System.Drawing.Point(108, 86);
            this.dteDate.Name = "dteDate";
            this.dteDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
                new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dteDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
                new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dteDate.Properties.Mask.EditMask = "dd/MM/yyyy";
            this.dteDate.Properties.Mask.UseMaskAsDisplayFormat = true;
            this.dteDate.Size = new System.Drawing.Size(124, 20);
            this.dteDate.TabIndex = 4;
            //
            // cboTimeType
            //
            this.cboTimeType.Location = new System.Drawing.Point(8, 86);
            this.cboTimeType.Name = "cboTimeType";
            this.cboTimeType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
                new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboTimeType.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.cboTimeType.Size = new System.Drawing.Size(96, 20);
            this.cboTimeType.TabIndex = 3;
            //
            // lblTime
            //
            this.lblTime.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblTime.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(102)))), ((int)(((byte)(204)))));
            this.lblTime.Appearance.Options.UseFont = true;
            this.lblTime.Appearance.Options.UseForeColor = true;
            this.lblTime.Location = new System.Drawing.Point(8, 66);
            this.lblTime.Name = "lblTime";
            this.lblTime.Size = new System.Drawing.Size(75, 13);
            this.lblTime.TabIndex = 2;
            this.lblTime.Text = "Thời gian tạo";
            //
            // txtKeyword
            //
            this.txtKeyword.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtKeyword.Location = new System.Drawing.Point(8, 38);
            this.txtKeyword.Name = "txtKeyword";
            this.txtKeyword.Properties.NullValuePrompt = "Từ khóa tìm kiếm";
            this.txtKeyword.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtKeyword.Size = new System.Drawing.Size(224, 20);
            this.txtKeyword.TabIndex = 1;
            //
            // txtPatientCode
            //
            this.txtPatientCode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPatientCode.Location = new System.Drawing.Point(8, 12);
            this.txtPatientCode.Name = "txtPatientCode";
            this.txtPatientCode.Properties.NullValuePrompt = "Mã bệnh nhân";
            this.txtPatientCode.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtPatientCode.Size = new System.Drawing.Size(224, 20);
            this.txtPatientCode.TabIndex = 0;
            //
            // panelMain
            //
            this.panelMain.Controls.Add(this.gridControl);
            this.panelMain.Controls.Add(this.ucPaging);
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.Location = new System.Drawing.Point(240, 0);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(860, 600);
            this.panelMain.TabIndex = 1;
            //
            // gridControl
            //
            this.gridControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControl.Location = new System.Drawing.Point(2, 2);
            this.gridControl.MainView = this.gridView;
            this.gridControl.Name = "gridControl";
            this.gridControl.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
                this.repoEdit,
                this.repoDelete,
                this.repoLock,
                this.repoUnlock,
                this.repoPay,
                this.repoRefund,
                this.repoPrint,
                this.repoEmpty});
            this.gridControl.Size = new System.Drawing.Size(856, 550);
            this.gridControl.TabIndex = 0;
            this.gridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
                this.gridView});
            //
            // gridView
            //
            this.gridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
                this.colSTT,
                this.colEdit,
                this.colDelete,
                this.colLock,
                this.colPay,
                this.colRefund,
                this.colPrint,
                this.colPatientCode,
                this.colPatientName,
                this.colDob,
                this.colGender,
                this.colPackageName,
                this.colStatus,
                this.colAddress,
                this.colCreateTime,
                this.colCreator,
                this.colModifyTime,
                this.colModifier});
            this.gridView.GridControl = this.gridControl;
            this.gridView.Name = "gridView";
            this.gridView.OptionsBehavior.Editable = true;
            this.gridView.OptionsCustomization.AllowGroup = false;
            this.gridView.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridView.OptionsSelection.MultiSelect = true;
            this.gridView.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;
            this.gridView.OptionsView.ColumnAutoWidth = false;
            this.gridView.OptionsView.ShowGroupPanel = false;
            this.gridView.OptionsView.ShowIndicator = false;
            //
            // colSTT
            //
            this.colSTT.Caption = "STT";
            this.colSTT.FieldName = "STT";
            this.colSTT.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.colSTT.Name = "colSTT";
            this.colSTT.OptionsColumn.AllowEdit = false;
            this.colSTT.OptionsColumn.FixedWidth = true;
            this.colSTT.Visible = true;
            this.colSTT.VisibleIndex = 0;
            this.colSTT.Width = 40;
            //
            // colEdit
            //
            this.colEdit.Caption = "Sửa";
            this.colEdit.ColumnEdit = this.repoEmpty;
            this.colEdit.FieldName = "BTN_EDIT";
            this.colEdit.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.colEdit.Name = "colEdit";
            this.colEdit.OptionsColumn.FixedWidth = true;
            this.colEdit.OptionsColumn.ShowCaption = false;
            this.colEdit.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.colEdit.Visible = true;
            this.colEdit.VisibleIndex = 3;
            this.colEdit.Width = 26;
            //
            // colDelete
            //
            this.colDelete.Caption = "Xóa";
            this.colDelete.ColumnEdit = this.repoEmpty;
            this.colDelete.FieldName = "BTN_DELETE";
            this.colDelete.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.colDelete.Name = "colDelete";
            this.colDelete.OptionsColumn.FixedWidth = true;
            this.colDelete.OptionsColumn.ShowCaption = false;
            this.colDelete.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.colDelete.Visible = true;
            this.colDelete.VisibleIndex = 1;
            this.colDelete.Width = 26;
            //
            // colLock
            //
            this.colLock.Caption = "Khóa";
            this.colLock.ColumnEdit = this.repoEmpty;
            this.colLock.FieldName = "BTN_LOCK";
            this.colLock.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.colLock.Name = "colLock";
            this.colLock.OptionsColumn.FixedWidth = true;
            this.colLock.OptionsColumn.ShowCaption = false;
            this.colLock.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.colLock.Visible = true;
            this.colLock.VisibleIndex = 2;
            this.colLock.Width = 26;
            //
            // colPay
            //
            this.colPay.Caption = "Thanh toán";
            this.colPay.ColumnEdit = this.repoEmpty;
            this.colPay.FieldName = "BTN_PAY";
            this.colPay.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.colPay.Name = "colPay";
            this.colPay.OptionsColumn.FixedWidth = true;
            this.colPay.OptionsColumn.ShowCaption = false;
            this.colPay.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.colPay.Visible = true;
            this.colPay.VisibleIndex = 4;
            this.colPay.Width = 26;
            //
            // colRefund
            //
            this.colRefund.Caption = "Hoàn tiền";
            this.colRefund.ColumnEdit = this.repoEmpty;
            this.colRefund.FieldName = "BTN_REFUND";
            this.colRefund.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.colRefund.Name = "colRefund";
            this.colRefund.OptionsColumn.FixedWidth = true;
            this.colRefund.OptionsColumn.ShowCaption = false;
            this.colRefund.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.colRefund.Visible = true;
            this.colRefund.VisibleIndex = 5;
            this.colRefund.Width = 26;
            //
            // colPrint
            //
            this.colPrint.Caption = "In";
            this.colPrint.ColumnEdit = this.repoEmpty;
            this.colPrint.FieldName = "BTN_PRINT";
            this.colPrint.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.colPrint.Name = "colPrint";
            this.colPrint.OptionsColumn.FixedWidth = true;
            this.colPrint.OptionsColumn.ShowCaption = false;
            this.colPrint.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.colPrint.Visible = true;
            this.colPrint.VisibleIndex = 6;
            this.colPrint.Width = 26;
            //
            // colPatientCode
            //
            this.colPatientCode.Caption = "Mã bệnh nhân";
            this.colPatientCode.FieldName = "PATIENT_CODE";
            this.colPatientCode.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.colPatientCode.Name = "colPatientCode";
            this.colPatientCode.OptionsColumn.AllowEdit = false;
            this.colPatientCode.Visible = true;
            this.colPatientCode.VisibleIndex = 7;
            this.colPatientCode.Width = 95;
            //
            // colPatientName
            //
            this.colPatientName.Caption = "Tên bệnh nhân";
            this.colPatientName.FieldName = "PATIENT_NAME";
            this.colPatientName.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.colPatientName.Name = "colPatientName";
            this.colPatientName.OptionsColumn.AllowEdit = false;
            this.colPatientName.Visible = true;
            this.colPatientName.VisibleIndex = 8;
            this.colPatientName.Width = 160;
            //
            // colDob
            //
            this.colDob.Caption = "Ngày sinh";
            this.colDob.FieldName = "DobDisplay";
            this.colDob.Name = "colDob";
            this.colDob.OptionsColumn.AllowEdit = false;
            this.colDob.Visible = true;
            this.colDob.VisibleIndex = 9;
            this.colDob.Width = 75;
            //
            // colGender
            //
            this.colGender.Caption = "Giới tính";
            this.colGender.FieldName = "GenderName";
            this.colGender.Name = "colGender";
            this.colGender.OptionsColumn.AllowEdit = false;
            this.colGender.Visible = true;
            this.colGender.VisibleIndex = 10;
            this.colGender.Width = 60;
            //
            // colPackageName
            //
            this.colPackageName.Caption = "Gói";
            this.colPackageName.FieldName = "PACKAGE_NAME";
            this.colPackageName.Name = "colPackageName";
            this.colPackageName.OptionsColumn.AllowEdit = false;
            this.colPackageName.Visible = true;
            this.colPackageName.VisibleIndex = 11;
            this.colPackageName.Width = 180;
            //
            // colStatus
            //
            this.colStatus.Caption = "Trạng thái";
            this.colStatus.FieldName = "StatusName";
            this.colStatus.Name = "colStatus";
            this.colStatus.OptionsColumn.AllowEdit = false;
            this.colStatus.Visible = true;
            this.colStatus.VisibleIndex = 12;
            this.colStatus.Width = 100;
            //
            // colAddress
            //
            this.colAddress.Caption = "Địa chỉ";
            this.colAddress.FieldName = "PATIENT_VIR_ADDRESS";
            this.colAddress.Name = "colAddress";
            this.colAddress.OptionsColumn.AllowEdit = false;
            this.colAddress.Visible = true;
            this.colAddress.VisibleIndex = 13;
            this.colAddress.Width = 220;
            //
            // colCreateTime
            //
            this.colCreateTime.Caption = "Thời gian tạo";
            this.colCreateTime.FieldName = "CreateTimeStr";
            this.colCreateTime.Name = "colCreateTime";
            this.colCreateTime.OptionsColumn.AllowEdit = false;
            this.colCreateTime.Visible = true;
            this.colCreateTime.VisibleIndex = 14;
            this.colCreateTime.Width = 120;
            //
            // colCreator
            //
            this.colCreator.Caption = "Người tạo";
            this.colCreator.FieldName = "CREATOR";
            this.colCreator.Name = "colCreator";
            this.colCreator.OptionsColumn.AllowEdit = false;
            this.colCreator.Visible = true;
            this.colCreator.VisibleIndex = 15;
            this.colCreator.Width = 100;
            //
            // colModifyTime
            //
            this.colModifyTime.Caption = "Thời gian sửa";
            this.colModifyTime.FieldName = "ModifyTimeStr";
            this.colModifyTime.Name = "colModifyTime";
            this.colModifyTime.OptionsColumn.AllowEdit = false;
            this.colModifyTime.Visible = true;
            this.colModifyTime.VisibleIndex = 16;
            this.colModifyTime.Width = 120;
            //
            // colModifier
            //
            this.colModifier.Caption = "Người sửa";
            this.colModifier.FieldName = "MODIFIER";
            this.colModifier.Name = "colModifier";
            this.colModifier.OptionsColumn.AllowEdit = false;
            this.colModifier.Visible = true;
            this.colModifier.VisibleIndex = 17;
            this.colModifier.Width = 100;
            //
            // repoEdit
            //
            this.repoEdit.AutoHeight = false;
            this.repoEdit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
                new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph)});
            this.repoEdit.Name = "repoEdit";
            this.repoEdit.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            //
            // repoDelete
            //
            this.repoDelete.AutoHeight = false;
            this.repoDelete.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
                new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph)});
            this.repoDelete.Name = "repoDelete";
            this.repoDelete.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            //
            // repoLock
            //
            this.repoLock.AutoHeight = false;
            this.repoLock.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
                new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph)});
            this.repoLock.Name = "repoLock";
            this.repoLock.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            //
            // repoUnlock
            //
            this.repoUnlock.AutoHeight = false;
            this.repoUnlock.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
                new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph)});
            this.repoUnlock.Name = "repoUnlock";
            this.repoUnlock.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            //
            // repoPay
            //
            this.repoPay.AutoHeight = false;
            this.repoPay.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
                new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph)});
            this.repoPay.Name = "repoPay";
            this.repoPay.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            //
            // repoRefund
            //
            this.repoRefund.AutoHeight = false;
            this.repoRefund.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
                new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph)});
            this.repoRefund.Name = "repoRefund";
            this.repoRefund.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            //
            // repoPrint
            //
            this.repoPrint.AutoHeight = false;
            this.repoPrint.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
                new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph)});
            this.repoPrint.Name = "repoPrint";
            this.repoPrint.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            //
            // repoEmpty
            //
            this.repoEmpty.AutoHeight = false;
            this.repoEmpty.Name = "repoEmpty";
            this.repoEmpty.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            //
            // ucPaging
            //
            this.ucPaging.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ucPaging.Location = new System.Drawing.Point(2, 552);
            this.ucPaging.Name = "ucPaging";
            this.ucPaging.Size = new System.Drawing.Size(856, 46);
            this.ucPaging.TabIndex = 1;
            //
            // UcHisPatientPackage
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelMain);
            this.Controls.Add(this.panelControlLeft);
            this.Name = "UcHisPatientPackage";
            this.Size = new System.Drawing.Size(1100, 600);
            this.Load += new System.EventHandler(this.UcHisPatientPackage_Load);
            ((System.ComponentModel.ISupportInitialize)(this.panelControlLeft)).EndInit();
            this.panelControlLeft.ResumeLayout(false);
            this.panelControlLeft.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dteDate.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteDate.Properties.VistaTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboTimeType.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtKeyword.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPatientCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelMain)).EndInit();
            this.panelMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoEdit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoDelete)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoLock)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoUnlock)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoPay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoRefund)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoPrint)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoEmpty)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelControlLeft;
        private DevExpress.XtraEditors.SimpleButton btnRefresh;
        private DevExpress.XtraEditors.SimpleButton btnSearch;
        private DevExpress.XtraEditors.SimpleButton btnNextDate;
        private DevExpress.XtraEditors.SimpleButton btnPrevDate;
        private DevExpress.XtraEditors.SimpleButton btnToggleTime;
        private DevExpress.XtraEditors.DateEdit dteDate;
        private DevExpress.XtraEditors.ComboBoxEdit cboTimeType;
        private DevExpress.XtraEditors.LabelControl lblTime;
        private DevExpress.XtraEditors.TextEdit txtKeyword;
        private DevExpress.XtraEditors.TextEdit txtPatientCode;
        private DevExpress.XtraEditors.PanelControl panelMain;
        private DevExpress.XtraGrid.GridControl gridControl;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView;
        private DevExpress.XtraGrid.Columns.GridColumn colSTT;
        private DevExpress.XtraGrid.Columns.GridColumn colEdit;
        private DevExpress.XtraGrid.Columns.GridColumn colDelete;
        private DevExpress.XtraGrid.Columns.GridColumn colLock;
        private DevExpress.XtraGrid.Columns.GridColumn colPay;
        private DevExpress.XtraGrid.Columns.GridColumn colRefund;
        private DevExpress.XtraGrid.Columns.GridColumn colPrint;
        private DevExpress.XtraGrid.Columns.GridColumn colPatientCode;
        private DevExpress.XtraGrid.Columns.GridColumn colPatientName;
        private DevExpress.XtraGrid.Columns.GridColumn colDob;
        private DevExpress.XtraGrid.Columns.GridColumn colGender;
        private DevExpress.XtraGrid.Columns.GridColumn colPackageName;
        private DevExpress.XtraGrid.Columns.GridColumn colStatus;
        private DevExpress.XtraGrid.Columns.GridColumn colAddress;
        private DevExpress.XtraGrid.Columns.GridColumn colCreateTime;
        private DevExpress.XtraGrid.Columns.GridColumn colCreator;
        private DevExpress.XtraGrid.Columns.GridColumn colModifyTime;
        private DevExpress.XtraGrid.Columns.GridColumn colModifier;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit repoEdit;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit repoDelete;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit repoLock;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit repoUnlock;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit repoPay;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit repoRefund;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit repoPrint;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit repoEmpty;
        private Inventec.UC.Paging.UcPaging ucPaging;
    }
}
