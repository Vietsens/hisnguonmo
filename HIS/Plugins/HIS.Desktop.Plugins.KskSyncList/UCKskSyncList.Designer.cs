/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 */
using HIS.Desktop.LocalStorage.LocalData;
namespace HIS.Desktop.Plugins.KskSyncList
{
    partial class UCKskSyncList
    {
        /// <summary> Required designer variable. </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> Clean up any resources being used. </summary>
        protected override void Dispose(bool disposing)
        {
            if (GlobalVariables.DicRefreshData != null && currentModule != null && GlobalVariables.DicRefreshData.Count > 0 && GlobalVariables.DicRefreshData.ContainsKey(currentModule.RoomId.ToString()))
            {
                GlobalVariables.DicRefreshData.Remove(currentModule.RoomId.ToString());
            }
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.bbtnSearch = new DevExpress.XtraBars.BarButtonItem();
            this.bbtnRefresh = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.panelFilter = new DevExpress.XtraEditors.PanelControl();
            this.lblKskType = new DevExpress.XtraEditors.LabelControl();
            this.cboKskType = new DevExpress.XtraEditors.ComboBoxEdit();
            this.lblConclusionFrom = new DevExpress.XtraEditors.LabelControl();
            this.dtConclusionFrom = new DevExpress.XtraEditors.DateEdit();
            this.lblConclusionTo = new DevExpress.XtraEditors.LabelControl();
            this.dtConclusionTo = new DevExpress.XtraEditors.DateEdit();
            this.lblSyncStatus = new DevExpress.XtraEditors.LabelControl();
            this.cboSyncStatus = new DevExpress.XtraEditors.ComboBoxEdit();
            this.txtPatientCode = new DevExpress.XtraEditors.TextEdit();
            this.txtTreatmentCode = new DevExpress.XtraEditors.TextEdit();
            this.txtKeyWord = new DevExpress.XtraEditors.TextEdit();
            this.btnSearch = new DevExpress.XtraEditors.SimpleButton();
            this.btnRefresh = new DevExpress.XtraEditors.SimpleButton();
            this.chkSign = new DevExpress.XtraEditors.CheckEdit();
            this.btnPreview = new DevExpress.XtraEditors.SimpleButton();
            this.btnSync = new DevExpress.XtraEditors.SimpleButton();
            this.gridControl1 = new DevExpress.XtraGrid.GridControl();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colStt = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colKskType = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colTreatmentCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colPatientCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colPatientName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colDob = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colGender = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colConclusionTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colConclusion = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colConcluder = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colRoom = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colSyncStatus = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colSyncTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colFailReason = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colTransaction = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colPush = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemButtonEdit_PUSH = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.ucPaging = new Inventec.UC.Paging.UcPaging();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelFilter)).BeginInit();
            this.panelFilter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboKskType.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtConclusionFrom.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtConclusionFrom.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtConclusionTo.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtConclusionTo.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboSyncStatus.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPatientCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTreatmentCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtKeyWord.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkSign.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemButtonEdit_PUSH)).BeginInit();
            this.SuspendLayout();
            //
            // barManager1
            //
            this.barManager1.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
            this.bar1});
            this.barManager1.DockControls.Add(this.barDockControlTop);
            this.barManager1.DockControls.Add(this.barDockControlBottom);
            this.barManager1.DockControls.Add(this.barDockControlLeft);
            this.barManager1.DockControls.Add(this.barDockControlRight);
            this.barManager1.Form = this;
            this.barManager1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.bbtnSearch,
            this.bbtnRefresh});
            this.barManager1.MaxItemId = 2;
            //
            // bar1
            //
            this.bar1.BarName = "Tools";
            this.bar1.DockCol = 0;
            this.bar1.DockRow = 0;
            this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.bbtnSearch),
            new DevExpress.XtraBars.LinkPersistInfo(this.bbtnRefresh)});
            this.bar1.Text = "Tools";
            this.bar1.Visible = false;
            //
            // bbtnSearch
            //
            this.bbtnSearch.Caption = "Tìm kiếm (Ctrl F)";
            this.bbtnSearch.Id = 0;
            this.bbtnSearch.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F));
            this.bbtnSearch.Name = "bbtnSearch";
            this.bbtnSearch.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bbtnSearch_ItemClick);
            //
            // bbtnRefresh
            //
            this.bbtnRefresh.Caption = "Làm lại (Ctrl R)";
            this.bbtnRefresh.Id = 1;
            this.bbtnRefresh.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R));
            this.bbtnRefresh.Name = "bbtnRefresh";
            this.bbtnRefresh.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bbtnRefresh_ItemClick);
            //
            // barDockControlTop
            //
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Size = new System.Drawing.Size(1184, 0);
            //
            // barDockControlBottom
            //
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 611);
            this.barDockControlBottom.Size = new System.Drawing.Size(1184, 0);
            //
            // barDockControlLeft
            //
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 0);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 611);
            //
            // barDockControlRight
            //
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(1184, 0);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 611);
            //
            // panelFilter
            //
            this.panelFilter.Controls.Add(this.lblKskType);
            this.panelFilter.Controls.Add(this.cboKskType);
            this.panelFilter.Controls.Add(this.lblConclusionFrom);
            this.panelFilter.Controls.Add(this.dtConclusionFrom);
            this.panelFilter.Controls.Add(this.lblConclusionTo);
            this.panelFilter.Controls.Add(this.dtConclusionTo);
            this.panelFilter.Controls.Add(this.lblSyncStatus);
            this.panelFilter.Controls.Add(this.cboSyncStatus);
            this.panelFilter.Controls.Add(this.txtPatientCode);
            this.panelFilter.Controls.Add(this.txtTreatmentCode);
            this.panelFilter.Controls.Add(this.txtKeyWord);
            this.panelFilter.Controls.Add(this.btnSearch);
            this.panelFilter.Controls.Add(this.btnRefresh);
            this.panelFilter.Controls.Add(this.chkSign);
            this.panelFilter.Controls.Add(this.btnPreview);
            this.panelFilter.Controls.Add(this.btnSync);
            this.panelFilter.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelFilter.Location = new System.Drawing.Point(0, 0);
            this.panelFilter.Name = "panelFilter";
            this.panelFilter.Size = new System.Drawing.Size(1184, 78);
            this.panelFilter.TabIndex = 0;
            //
            // lblKskType
            //
            this.lblKskType.Location = new System.Drawing.Point(8, 13);
            this.lblKskType.Name = "lblKskType";
            this.lblKskType.Size = new System.Drawing.Size(45, 13);
            this.lblKskType.TabIndex = 0;
            this.lblKskType.Text = "Loại KSK:";
            //
            // cboKskType
            //
            this.cboKskType.Location = new System.Drawing.Point(85, 10);
            this.cboKskType.MenuManager = this.barManager1;
            this.cboKskType.Name = "cboKskType";
            this.cboKskType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboKskType.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.cboKskType.Size = new System.Drawing.Size(170, 20);
            this.cboKskType.TabIndex = 1;
            //
            // lblConclusionFrom
            //
            this.lblConclusionFrom.Location = new System.Drawing.Point(270, 13);
            this.lblConclusionFrom.Name = "lblConclusionFrom";
            this.lblConclusionFrom.Size = new System.Drawing.Size(77, 13);
            this.lblConclusionFrom.TabIndex = 2;
            this.lblConclusionFrom.Text = "Ngày kết luận từ:";
            //
            // dtConclusionFrom
            //
            this.dtConclusionFrom.EditValue = null;
            this.dtConclusionFrom.Location = new System.Drawing.Point(360, 10);
            this.dtConclusionFrom.MenuManager = this.barManager1;
            this.dtConclusionFrom.Name = "dtConclusionFrom";
            this.dtConclusionFrom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtConclusionFrom.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtConclusionFrom.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
            this.dtConclusionFrom.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.dtConclusionFrom.Properties.EditFormat.FormatString = "dd/MM/yyyy";
            this.dtConclusionFrom.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.dtConclusionFrom.Size = new System.Drawing.Size(110, 20);
            this.dtConclusionFrom.TabIndex = 3;
            //
            // lblConclusionTo
            //
            this.lblConclusionTo.Location = new System.Drawing.Point(485, 13);
            this.lblConclusionTo.Name = "lblConclusionTo";
            this.lblConclusionTo.Size = new System.Drawing.Size(20, 13);
            this.lblConclusionTo.TabIndex = 4;
            this.lblConclusionTo.Text = "đến:";
            //
            // dtConclusionTo
            //
            this.dtConclusionTo.EditValue = null;
            this.dtConclusionTo.Location = new System.Drawing.Point(515, 10);
            this.dtConclusionTo.MenuManager = this.barManager1;
            this.dtConclusionTo.Name = "dtConclusionTo";
            this.dtConclusionTo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtConclusionTo.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtConclusionTo.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
            this.dtConclusionTo.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.dtConclusionTo.Properties.EditFormat.FormatString = "dd/MM/yyyy";
            this.dtConclusionTo.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.dtConclusionTo.Size = new System.Drawing.Size(110, 20);
            this.dtConclusionTo.TabIndex = 5;
            //
            // lblSyncStatus
            //
            this.lblSyncStatus.Location = new System.Drawing.Point(645, 13);
            this.lblSyncStatus.Name = "lblSyncStatus";
            this.lblSyncStatus.Size = new System.Drawing.Size(67, 13);
            this.lblSyncStatus.TabIndex = 6;
            this.lblSyncStatus.Text = "Trạng thái đẩy:";
            //
            // cboSyncStatus
            //
            this.cboSyncStatus.Location = new System.Drawing.Point(725, 10);
            this.cboSyncStatus.MenuManager = this.barManager1;
            this.cboSyncStatus.Name = "cboSyncStatus";
            this.cboSyncStatus.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboSyncStatus.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.cboSyncStatus.Properties.Items.AddRange(new object[] {
            "(Tất cả)",
            "Chưa đồng bộ",
            "Đã đồng bộ",
            "Thất bại"});
            this.cboSyncStatus.Size = new System.Drawing.Size(150, 20);
            this.cboSyncStatus.TabIndex = 7;
            //
            // txtPatientCode
            //
            this.txtPatientCode.Location = new System.Drawing.Point(8, 44);
            this.txtPatientCode.MenuManager = this.barManager1;
            this.txtPatientCode.Name = "txtPatientCode";
            this.txtPatientCode.Properties.NullValuePrompt = "Mã bệnh nhân";
            this.txtPatientCode.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtPatientCode.Size = new System.Drawing.Size(160, 20);
            this.txtPatientCode.TabIndex = 8;
            this.txtPatientCode.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txt_PreviewKeyDown);
            //
            // txtTreatmentCode
            //
            this.txtTreatmentCode.Location = new System.Drawing.Point(174, 44);
            this.txtTreatmentCode.MenuManager = this.barManager1;
            this.txtTreatmentCode.Name = "txtTreatmentCode";
            this.txtTreatmentCode.Properties.NullValuePrompt = "Mã điều trị";
            this.txtTreatmentCode.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtTreatmentCode.Size = new System.Drawing.Size(160, 20);
            this.txtTreatmentCode.TabIndex = 9;
            this.txtTreatmentCode.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txt_PreviewKeyDown);
            //
            // txtKeyWord
            //
            this.txtKeyWord.Location = new System.Drawing.Point(340, 44);
            this.txtKeyWord.MenuManager = this.barManager1;
            this.txtKeyWord.Name = "txtKeyWord";
            this.txtKeyWord.Properties.NullValuePrompt = "Từ khóa (tên BN, mã y lệnh...)";
            this.txtKeyWord.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtKeyWord.Size = new System.Drawing.Size(230, 20);
            this.txtKeyWord.TabIndex = 10;
            this.txtKeyWord.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txt_PreviewKeyDown);
            //
            // btnSearch
            //
            this.btnSearch.Location = new System.Drawing.Point(578, 43);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(120, 22);
            this.btnSearch.TabIndex = 11;
            this.btnSearch.Text = "Tìm kiếm (Ctrl F)";
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            //
            // btnRefresh
            //
            this.btnRefresh.Location = new System.Drawing.Point(704, 43);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(90, 22);
            this.btnRefresh.TabIndex = 12;
            this.btnRefresh.Text = "Làm lại";
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            //
            // chkSign
            //
            this.chkSign.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkSign.Location = new System.Drawing.Point(838, 44);
            this.chkSign.MenuManager = this.barManager1;
            this.chkSign.Name = "chkSign";
            this.chkSign.Properties.Caption = "Ký số";
            this.chkSign.Size = new System.Drawing.Size(60, 19);
            this.chkSign.TabIndex = 13;
            this.chkSign.CheckedChanged += new System.EventHandler(this.chkSign_CheckedChanged);
            //
            // btnPreview
            //
            this.btnPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPreview.Location = new System.Drawing.Point(902, 43);
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.Size = new System.Drawing.Size(140, 22);
            this.btnPreview.TabIndex = 14;
            this.btnPreview.Text = "Xem dữ liệu sẽ đẩy";
            this.btnPreview.Click += new System.EventHandler(this.btnPreview_Click);
            //
            // btnSync
            //
            this.btnSync.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSync.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(215)))));
            this.btnSync.Appearance.ForeColor = System.Drawing.Color.White;
            this.btnSync.Appearance.Options.UseBackColor = true;
            this.btnSync.Appearance.Options.UseForeColor = true;
            this.btnSync.Enabled = false;
            this.btnSync.Location = new System.Drawing.Point(1046, 43);
            this.btnSync.Name = "btnSync";
            this.btnSync.Size = new System.Drawing.Size(130, 22);
            this.btnSync.TabIndex = 15;
            this.btnSync.Text = "Đồng bộ lên cổng  (0)";
            this.btnSync.ToolTip = "Đồng bộ các hồ sơ đã chọn lên Cổng dữ liệu BYT (QĐ 1551)";
            this.btnSync.Click += new System.EventHandler(this.btnSync_Click);
            //
            // gridControl1
            //
            this.gridControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControl1.Location = new System.Drawing.Point(0, 78);
            this.gridControl1.MainView = this.gridView1;
            this.gridControl1.MenuManager = this.barManager1;
            this.gridControl1.Name = "gridControl1";
            this.gridControl1.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemButtonEdit_PUSH});
            this.gridControl1.Size = new System.Drawing.Size(1184, 513);
            this.gridControl1.TabIndex = 1;
            this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
            //
            // gridView1
            //
            this.gridView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colStt,
            this.colKskType,
            this.colTreatmentCode,
            this.colPatientCode,
            this.colPatientName,
            this.colDob,
            this.colGender,
            this.colConclusionTime,
            this.colConclusion,
            this.colConcluder,
            this.colRoom,
            this.colSyncStatus,
            this.colSyncTime,
            this.colFailReason,
            this.colTransaction,
            this.colPush});
            this.gridView1.GridControl = this.gridControl1;
            this.gridView1.Name = "gridView1";
            this.gridView1.OptionsSelection.CheckBoxSelectorColumnWidth = 32;
            this.gridView1.OptionsSelection.MultiSelect = true;
            this.gridView1.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;
            this.gridView1.OptionsView.ColumnAutoWidth = false;
            this.gridView1.OptionsView.ShowGroupPanel = false;
            this.gridView1.OptionsView.ShowIndicator = false;
            this.gridView1.SelectionChanged += new DevExpress.Data.SelectionChangedEventHandler(this.gridView1_SelectionChanged);
            this.gridView1.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridView1_CustomUnboundColumnData);
            this.gridView1.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gridView1_RowCellStyle);
            //
            // colStt
            //
            this.colStt.AppearanceCell.Options.UseTextOptions = true;
            this.colStt.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.colStt.Caption = "STT";
            this.colStt.FieldName = "STT";
            this.colStt.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.colStt.Name = "colStt";
            this.colStt.OptionsColumn.AllowEdit = false;
            this.colStt.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.colStt.Visible = true;
            this.colStt.VisibleIndex = 1;
            this.colStt.Width = 45;
            //
            // colKskType
            //
            this.colKskType.Caption = "Loại KSK";
            this.colKskType.FieldName = "KSK_TYPE_NAME";
            this.colKskType.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.colKskType.Name = "colKskType";
            this.colKskType.OptionsColumn.AllowEdit = false;
            this.colKskType.Visible = true;
            this.colKskType.VisibleIndex = 2;
            this.colKskType.Width = 110;
            //
            // colTreatmentCode
            //
            this.colTreatmentCode.Caption = "Mã điều trị";
            this.colTreatmentCode.FieldName = "TDL_TREATMENT_CODE";
            this.colTreatmentCode.Name = "colTreatmentCode";
            this.colTreatmentCode.OptionsColumn.AllowEdit = false;
            this.colTreatmentCode.Visible = true;
            this.colTreatmentCode.VisibleIndex = 3;
            this.colTreatmentCode.Width = 100;
            //
            // colPatientCode
            //
            this.colPatientCode.Caption = "Mã BN";
            this.colPatientCode.FieldName = "TDL_PATIENT_CODE";
            this.colPatientCode.Name = "colPatientCode";
            this.colPatientCode.OptionsColumn.AllowEdit = false;
            this.colPatientCode.Visible = true;
            this.colPatientCode.VisibleIndex = 4;
            this.colPatientCode.Width = 100;
            //
            // colPatientName
            //
            this.colPatientName.Caption = "Tên bệnh nhân";
            this.colPatientName.FieldName = "TDL_PATIENT_NAME";
            this.colPatientName.Name = "colPatientName";
            this.colPatientName.OptionsColumn.AllowEdit = false;
            this.colPatientName.Visible = true;
            this.colPatientName.VisibleIndex = 5;
            this.colPatientName.Width = 150;
            //
            // colDob
            //
            this.colDob.Caption = "Ngày sinh";
            this.colDob.FieldName = "TDL_PATIENT_DOB_STR";
            this.colDob.Name = "colDob";
            this.colDob.OptionsColumn.AllowEdit = false;
            this.colDob.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.colDob.Visible = true;
            this.colDob.VisibleIndex = 6;
            this.colDob.Width = 90;
            //
            // colGender
            //
            this.colGender.Caption = "Giới tính";
            this.colGender.FieldName = "TDL_PATIENT_GENDER_NAME";
            this.colGender.Name = "colGender";
            this.colGender.OptionsColumn.AllowEdit = false;
            this.colGender.Visible = true;
            this.colGender.VisibleIndex = 7;
            this.colGender.Width = 70;
            //
            // colConclusionTime
            //
            this.colConclusionTime.Caption = "Ngày kết luận";
            this.colConclusionTime.FieldName = "CONCLUSION_TIME_STR";
            this.colConclusionTime.Name = "colConclusionTime";
            this.colConclusionTime.OptionsColumn.AllowEdit = false;
            this.colConclusionTime.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.colConclusionTime.Visible = true;
            this.colConclusionTime.VisibleIndex = 8;
            this.colConclusionTime.Width = 100;
            //
            // colConclusion
            //
            this.colConclusion.Caption = "Kết luận";
            this.colConclusion.FieldName = "CONCLUSION";
            this.colConclusion.Name = "colConclusion";
            this.colConclusion.OptionsColumn.AllowEdit = false;
            this.colConclusion.Visible = true;
            this.colConclusion.VisibleIndex = 9;
            this.colConclusion.Width = 180;
            //
            // colConcluder
            //
            this.colConcluder.Caption = "Người kết luận";
            this.colConcluder.FieldName = "CONCLUDER_NAME";
            this.colConcluder.Name = "colConcluder";
            this.colConcluder.OptionsColumn.AllowEdit = false;
            this.colConcluder.Visible = true;
            this.colConcluder.VisibleIndex = 10;
            this.colConcluder.Width = 120;
            //
            // colRoom
            //
            this.colRoom.Caption = "Phòng thực hiện";
            this.colRoom.FieldName = "EXECUTE_ROOM_NAME";
            this.colRoom.Name = "colRoom";
            this.colRoom.OptionsColumn.AllowEdit = false;
            this.colRoom.Visible = true;
            this.colRoom.VisibleIndex = 11;
            this.colRoom.Width = 120;
            //
            // colSyncStatus
            //
            this.colSyncStatus.AppearanceCell.Options.UseTextOptions = true;
            this.colSyncStatus.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colSyncStatus.Caption = "Trạng thái đẩy";
            this.colSyncStatus.FieldName = "SYNC_RESULT_TYPE_STR";
            this.colSyncStatus.Name = "colSyncStatus";
            this.colSyncStatus.OptionsColumn.AllowEdit = false;
            this.colSyncStatus.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.colSyncStatus.Visible = true;
            this.colSyncStatus.VisibleIndex = 12;
            this.colSyncStatus.Width = 110;
            //
            // colSyncTime
            //
            this.colSyncTime.Caption = "Thời gian đẩy";
            this.colSyncTime.FieldName = "SYNC_TIME_STR";
            this.colSyncTime.Name = "colSyncTime";
            this.colSyncTime.OptionsColumn.AllowEdit = false;
            this.colSyncTime.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.colSyncTime.Visible = true;
            this.colSyncTime.VisibleIndex = 13;
            this.colSyncTime.Width = 120;
            //
            // colFailReason
            //
            this.colFailReason.Caption = "Lý do lỗi";
            this.colFailReason.FieldName = "SYNC_FAILD_REASON";
            this.colFailReason.Name = "colFailReason";
            this.colFailReason.OptionsColumn.AllowEdit = false;
            this.colFailReason.Visible = true;
            this.colFailReason.VisibleIndex = 14;
            this.colFailReason.Width = 170;
            //
            // colTransaction
            //
            this.colTransaction.Caption = "Mã giao dịch";
            this.colTransaction.FieldName = "TRANSACTION_CODE";
            this.colTransaction.Name = "colTransaction";
            this.colTransaction.OptionsColumn.AllowEdit = false;
            this.colTransaction.Visible = true;
            this.colTransaction.VisibleIndex = 15;
            this.colTransaction.Width = 120;
            //
            // colPush
            //
            this.colPush.Caption = "Đẩy";
            this.colPush.ColumnEdit = this.repositoryItemButtonEdit_PUSH;
            this.colPush.FieldName = "colPush";
            this.colPush.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Right;
            this.colPush.Name = "colPush";
            this.colPush.OptionsColumn.ShowCaption = false;
            this.colPush.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.colPush.Visible = true;
            this.colPush.VisibleIndex = 16;
            this.colPush.Width = 40;
            //
            // repositoryItemButtonEdit_PUSH
            //
            this.repositoryItemButtonEdit_PUSH.AutoHeight = false;
            DevExpress.XtraEditors.Controls.EditorButton editorButtonPush = new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph);
            editorButtonPush.Caption = "Đẩy";
            editorButtonPush.ToolTip = "Đẩy / đẩy lại riêng hồ sơ này lên cổng";
            this.repositoryItemButtonEdit_PUSH.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            editorButtonPush});
            this.repositoryItemButtonEdit_PUSH.Name = "repositoryItemButtonEdit_PUSH";
            this.repositoryItemButtonEdit_PUSH.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            this.repositoryItemButtonEdit_PUSH.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repositoryItemButtonEdit_PUSH_ButtonClick);
            //
            // ucPaging
            //
            this.ucPaging.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ucPaging.Location = new System.Drawing.Point(0, 591);
            this.ucPaging.Name = "ucPaging";
            this.ucPaging.Size = new System.Drawing.Size(1184, 20);
            this.ucPaging.TabIndex = 2;
            //
            // UCKskSyncList
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gridControl1);
            this.Controls.Add(this.ucPaging);
            this.Controls.Add(this.panelFilter);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Name = "UCKskSyncList";
            this.Size = new System.Drawing.Size(1184, 611);
            this.Load += new System.EventHandler(this.UCKskSyncList_Load);
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelFilter)).EndInit();
            this.panelFilter.ResumeLayout(false);
            this.panelFilter.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboKskType.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtConclusionFrom.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtConclusionFrom.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtConclusionTo.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtConclusionTo.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboSyncStatus.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPatientCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTreatmentCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtKeyWord.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkSign.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemButtonEdit_PUSH)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.Bar bar1;
        private DevExpress.XtraBars.BarButtonItem bbtnSearch;
        private DevExpress.XtraBars.BarButtonItem bbtnRefresh;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraEditors.PanelControl panelFilter;
        private DevExpress.XtraEditors.LabelControl lblKskType;
        private DevExpress.XtraEditors.ComboBoxEdit cboKskType;
        private DevExpress.XtraEditors.LabelControl lblConclusionFrom;
        private DevExpress.XtraEditors.DateEdit dtConclusionFrom;
        private DevExpress.XtraEditors.LabelControl lblConclusionTo;
        private DevExpress.XtraEditors.DateEdit dtConclusionTo;
        private DevExpress.XtraEditors.LabelControl lblSyncStatus;
        private DevExpress.XtraEditors.ComboBoxEdit cboSyncStatus;
        private DevExpress.XtraEditors.TextEdit txtPatientCode;
        private DevExpress.XtraEditors.TextEdit txtTreatmentCode;
        private DevExpress.XtraEditors.TextEdit txtKeyWord;
        private DevExpress.XtraEditors.SimpleButton btnSearch;
        private DevExpress.XtraEditors.SimpleButton btnRefresh;
        private DevExpress.XtraEditors.CheckEdit chkSign;
        private DevExpress.XtraEditors.SimpleButton btnPreview;
        private DevExpress.XtraEditors.SimpleButton btnSync;
        private DevExpress.XtraGrid.GridControl gridControl1;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraGrid.Columns.GridColumn colStt;
        private DevExpress.XtraGrid.Columns.GridColumn colKskType;
        private DevExpress.XtraGrid.Columns.GridColumn colTreatmentCode;
        private DevExpress.XtraGrid.Columns.GridColumn colPatientCode;
        private DevExpress.XtraGrid.Columns.GridColumn colPatientName;
        private DevExpress.XtraGrid.Columns.GridColumn colDob;
        private DevExpress.XtraGrid.Columns.GridColumn colGender;
        private DevExpress.XtraGrid.Columns.GridColumn colConclusionTime;
        private DevExpress.XtraGrid.Columns.GridColumn colConclusion;
        private DevExpress.XtraGrid.Columns.GridColumn colConcluder;
        private DevExpress.XtraGrid.Columns.GridColumn colRoom;
        private DevExpress.XtraGrid.Columns.GridColumn colSyncStatus;
        private DevExpress.XtraGrid.Columns.GridColumn colSyncTime;
        private DevExpress.XtraGrid.Columns.GridColumn colFailReason;
        private DevExpress.XtraGrid.Columns.GridColumn colTransaction;
        private DevExpress.XtraGrid.Columns.GridColumn colPush;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit repositoryItemButtonEdit_PUSH;
        private Inventec.UC.Paging.UcPaging ucPaging;
    }
}
