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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UCKskSyncList));
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject25 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject26 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject27 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject28 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject29 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject30 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject31 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject32 = new DevExpress.Utils.SerializableAppearanceObject();
            this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.bbtnSearch = new DevExpress.XtraBars.BarButtonItem();
            this.bbtnRefresh = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.btnSettings = new DevExpress.XtraEditors.SimpleButton();
            this.btnExportPath = new DevExpress.XtraEditors.SimpleButton();
            this.btnClsMap = new DevExpress.XtraEditors.SimpleButton();
            this.cboKskType = new DevExpress.XtraEditors.ComboBoxEdit();
            this.dtConclusionFrom = new DevExpress.XtraEditors.DateEdit();
            this.dtConclusionTo = new DevExpress.XtraEditors.DateEdit();
            this.cboSyncStatus = new DevExpress.XtraEditors.ComboBoxEdit();
            this.txtPatientCode = new DevExpress.XtraEditors.TextEdit();
            this.txtTreatmentCode = new DevExpress.XtraEditors.TextEdit();
            this.txtKeyWord = new DevExpress.XtraEditors.TextEdit();
            this.btnSearch = new DevExpress.XtraEditors.SimpleButton();
            this.btnRefresh = new DevExpress.XtraEditors.SimpleButton();
            this.chkSign = new DevExpress.XtraEditors.CheckEdit();
            this.btnPreview = new DevExpress.XtraEditors.SimpleButton();
            this.btnSync = new DevExpress.XtraEditors.SimpleButton();
            this.btnVlgStatus = new DevExpress.XtraEditors.SimpleButton();
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
            this.colPreview = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemButtonEdit_PREVIEW = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.ucPaging = new Inventec.UC.Paging.UcPaging();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.grpFilter = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciKskType = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciConclusionFrom = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciConclusionTo = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciSyncStatus = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciBtnVlgStatus = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptyFilterTop = new DevExpress.XtraLayout.EmptySpaceItem();
            this.lciPatientCode = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciTreatmentCode = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciKeyWord = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciBtnSearch = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciBtnRefresh = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciChkSign = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciBtnPreview = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciBtnSync = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciBtnExportPath = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciBtnClsMap = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciGrid = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciPaging = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
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
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemButtonEdit_PREVIEW)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpFilter)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciKskType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciConclusionFrom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciConclusionTo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciSyncStatus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnVlgStatus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptyFilterTop)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPatientCode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTreatmentCode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciKeyWord)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnSearch)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnRefresh)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciChkSign)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnPreview)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnSync)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnExportPath)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnClsMap)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPaging)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
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
            this.barDockControlTop.Size = new System.Drawing.Size(1184, 29);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 769);
            this.barDockControlBottom.Size = new System.Drawing.Size(1184, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 29);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 740);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(1184, 29);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 740);
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.btnSettings);
            this.layoutControl1.Controls.Add(this.btnExportPath);
            this.layoutControl1.Controls.Add(this.btnClsMap);
            this.layoutControl1.Controls.Add(this.cboKskType);
            this.layoutControl1.Controls.Add(this.dtConclusionFrom);
            this.layoutControl1.Controls.Add(this.dtConclusionTo);
            this.layoutControl1.Controls.Add(this.cboSyncStatus);
            this.layoutControl1.Controls.Add(this.txtPatientCode);
            this.layoutControl1.Controls.Add(this.txtTreatmentCode);
            this.layoutControl1.Controls.Add(this.txtKeyWord);
            this.layoutControl1.Controls.Add(this.btnSearch);
            this.layoutControl1.Controls.Add(this.btnRefresh);
            this.layoutControl1.Controls.Add(this.chkSign);
            this.layoutControl1.Controls.Add(this.btnPreview);
            this.layoutControl1.Controls.Add(this.btnSync);
            this.layoutControl1.Controls.Add(this.btnVlgStatus);
            this.layoutControl1.Controls.Add(this.gridControl1);
            this.layoutControl1.Controls.Add(this.ucPaging);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 29);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.Root;
            this.layoutControl1.Size = new System.Drawing.Size(1184, 740);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // btnSettings
            // 
            this.btnSettings.Image = ((System.Drawing.Image)(resources.GetObject("btnSettings.Image")));
            this.btnSettings.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnSettings.Location = new System.Drawing.Point(891, 32);
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Size = new System.Drawing.Size(24, 22);
            this.btnSettings.StyleController = this.layoutControl1;
            this.btnSettings.TabIndex = 16;
            this.btnSettings.ToolTip = "Cài đặt đẩy dữ liệu liên thông khám sức khỏe";
            this.btnSettings.Click += new System.EventHandler(this.btnSettings_Click);
            // 
            // btnExportPath
            // 
            this.btnExportPath.Image = ((System.Drawing.Image)(resources.GetObject("btnExportPath.Image")));
            this.btnExportPath.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnExportPath.Location = new System.Drawing.Point(919, 42);
            this.btnExportPath.Name = "btnExportPath";
            this.btnExportPath.Size = new System.Drawing.Size(24, 22);
            this.btnExportPath.StyleController = this.layoutControl1;
            this.btnExportPath.TabIndex = 17;
            this.btnExportPath.ToolTip = "Thiết lập đường dẫn xuất xml";
            this.btnExportPath.Click += new System.EventHandler(this.btnExportPath_Click);
            // 
            // btnClsMap
            // 
            this.btnClsMap.Location = new System.Drawing.Point(1070, 6);
            this.btnClsMap.Name = "btnClsMap";
            this.btnClsMap.Size = new System.Drawing.Size(108, 22);
            this.btnClsMap.StyleController = this.layoutControl1;
            this.btnClsMap.TabIndex = 18;
            this.btnClsMap.Text = "Nối chỉ số CLS HCM";
            this.btnClsMap.ToolTip = "Nối chỉ số cận lâm sàng của HIS với chỉ tiêu mẫu M4 — Liên thông KSK Sở Y tế TP.H" +
    "CM";
            this.btnClsMap.Click += new System.EventHandler(this.btnClsMap_Click);
            // 
            // cboKskType
            // 
            this.cboKskType.Location = new System.Drawing.Point(85, 6);
            this.cboKskType.MenuManager = this.barManager1;
            this.cboKskType.Name = "cboKskType";
            this.cboKskType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboKskType.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.cboKskType.Size = new System.Drawing.Size(157, 20);
            this.cboKskType.StyleController = this.layoutControl1;
            this.cboKskType.TabIndex = 1;
            // 
            // dtConclusionFrom
            // 
            this.dtConclusionFrom.EditValue = null;
            this.dtConclusionFrom.Location = new System.Drawing.Point(345, 6);
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
            this.dtConclusionFrom.Size = new System.Drawing.Size(171, 20);
            this.dtConclusionFrom.StyleController = this.layoutControl1;
            this.dtConclusionFrom.TabIndex = 3;
            // 
            // dtConclusionTo
            // 
            this.dtConclusionTo.EditValue = null;
            this.dtConclusionTo.Location = new System.Drawing.Point(566, 6);
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
            this.dtConclusionTo.Size = new System.Drawing.Size(175, 20);
            this.dtConclusionTo.StyleController = this.layoutControl1;
            this.dtConclusionTo.TabIndex = 5;
            // 
            // cboSyncStatus
            // 
            this.cboSyncStatus.Location = new System.Drawing.Point(846, 6);
            this.cboSyncStatus.MenuManager = this.barManager1;
            this.cboSyncStatus.Name = "cboSyncStatus";
            this.cboSyncStatus.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboSyncStatus.Properties.Items.AddRange(new object[] {
            "(Tất cả)",
            "Chưa đồng bộ",
            "Đã đồng bộ",
            "Thất bại"});
            this.cboSyncStatus.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.cboSyncStatus.Size = new System.Drawing.Size(67, 20);
            this.cboSyncStatus.StyleController = this.layoutControl1;
            this.cboSyncStatus.TabIndex = 7;
            // 
            // txtPatientCode
            // 
            this.txtPatientCode.Location = new System.Drawing.Point(6, 32);
            this.txtPatientCode.MenuManager = this.barManager1;
            this.txtPatientCode.Name = "txtPatientCode";
            this.txtPatientCode.Properties.NullValuePrompt = "Mã bệnh nhân";
            this.txtPatientCode.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtPatientCode.Size = new System.Drawing.Size(167, 20);
            this.txtPatientCode.StyleController = this.layoutControl1;
            this.txtPatientCode.TabIndex = 8;
            this.txtPatientCode.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txt_PreviewKeyDown);
            // 
            // txtTreatmentCode
            // 
            this.txtTreatmentCode.Location = new System.Drawing.Point(177, 32);
            this.txtTreatmentCode.MenuManager = this.barManager1;
            this.txtTreatmentCode.Name = "txtTreatmentCode";
            this.txtTreatmentCode.Properties.NullValuePrompt = "Mã điều trị";
            this.txtTreatmentCode.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtTreatmentCode.Size = new System.Drawing.Size(167, 20);
            this.txtTreatmentCode.StyleController = this.layoutControl1;
            this.txtTreatmentCode.TabIndex = 9;
            this.txtTreatmentCode.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txt_PreviewKeyDown);
            // 
            // txtKeyWord
            // 
            this.txtKeyWord.Location = new System.Drawing.Point(348, 32);
            this.txtKeyWord.MenuManager = this.barManager1;
            this.txtKeyWord.Name = "txtKeyWord";
            this.txtKeyWord.Properties.NullValuePrompt = "Từ khóa (tên BN, mã y lệnh...)";
            this.txtKeyWord.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtKeyWord.Size = new System.Drawing.Size(238, 20);
            this.txtKeyWord.StyleController = this.layoutControl1;
            this.txtKeyWord.TabIndex = 10;
            this.txtKeyWord.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txt_PreviewKeyDown);
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(590, 32);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(127, 22);
            this.btnSearch.StyleController = this.layoutControl1;
            this.btnSearch.TabIndex = 11;
            this.btnSearch.Text = "Tìm kiếm (Ctrl F)";
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(721, 32);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(96, 22);
            this.btnRefresh.StyleController = this.layoutControl1;
            this.btnRefresh.TabIndex = 12;
            this.btnRefresh.Text = "Làm lại";
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // chkSign
            // 
            this.chkSign.Location = new System.Drawing.Point(821, 32);
            this.chkSign.MenuManager = this.barManager1;
            this.chkSign.Name = "chkSign";
            this.chkSign.Properties.Caption = "Ký số";
            this.chkSign.Size = new System.Drawing.Size(66, 19);
            this.chkSign.StyleController = this.layoutControl1;
            this.chkSign.TabIndex = 13;
            this.chkSign.CheckedChanged += new System.EventHandler(this.chkSign_CheckedChanged);
            // 
            // btnPreview
            // 
            this.btnPreview.Location = new System.Drawing.Point(947, 42);
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.Size = new System.Drawing.Size(100, 22);
            this.btnPreview.StyleController = this.layoutControl1;
            this.btnPreview.TabIndex = 14;
            this.btnPreview.Text = "Xuất XML";
            this.btnPreview.Click += new System.EventHandler(this.btnPreview_Click);
            // 
            // btnSync
            // 
            this.btnSync.Enabled = false;
            this.btnSync.Location = new System.Drawing.Point(1051, 32);
            this.btnSync.Name = "btnSync";
            this.btnSync.Size = new System.Drawing.Size(127, 22);
            this.btnSync.StyleController = this.layoutControl1;
            this.btnSync.TabIndex = 15;
            this.btnSync.Text = "Đồng bộ lên cổng  (0)";
            this.btnSync.ToolTip = "Đồng bộ các hồ sơ đã chọn lên Cổng dữ liệu BYT (QĐ 1551)";
            this.btnSync.Click += new System.EventHandler(this.btnSync_Click);
            // 
            // btnVlgStatus
            // 
            this.btnVlgStatus.Location = new System.Drawing.Point(917, 6);
            this.btnVlgStatus.Name = "btnVlgStatus";
            this.btnVlgStatus.Size = new System.Drawing.Size(139, 22);
            this.btnVlgStatus.StyleController = this.layoutControl1;
            this.btnVlgStatus.TabIndex = 18;
            this.btnVlgStatus.Text = "Cập nhật KQ cổng VLg";
            this.btnVlgStatus.ToolTip = "Tra cứu kết quả xử lý thật trên Cổng tiếp nhận KDLYT Vĩnh Long và cập nhật trạng " +
    "thái hồ sơ (hồ sơ tích chọn; không tích thì toàn bộ hồ sơ đã đẩy trên trang hiện" +
    " tại)";
            this.btnVlgStatus.Click += new System.EventHandler(this.btnVlgStatus_Click);
            // 
            // gridControl1
            // 
            this.gridControl1.Location = new System.Drawing.Point(4, 70);
            this.gridControl1.MainView = this.gridView1;
            this.gridControl1.MenuManager = this.barManager1;
            this.gridControl1.Name = "gridControl1";
            this.gridControl1.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemButtonEdit_PUSH,
            this.repositoryItemButtonEdit_PREVIEW});
            this.gridControl1.Size = new System.Drawing.Size(1176, 642);
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
            this.colPush,
            this.colPreview});
            this.gridView1.GridControl = this.gridControl1;
            this.gridView1.Name = "gridView1";
            this.gridView1.OptionsSelection.CheckBoxSelectorColumnWidth = 32;
            this.gridView1.OptionsSelection.MultiSelect = true;
            this.gridView1.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;
            this.gridView1.OptionsView.ColumnAutoWidth = false;
            this.gridView1.OptionsView.ShowGroupPanel = false;
            this.gridView1.OptionsView.ShowIndicator = false;
            this.gridView1.RowCellClick += new DevExpress.XtraGrid.Views.Grid.RowCellClickEventHandler(this.gridView1_RowCellClick);
            this.gridView1.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gridView1_RowCellStyle);
            this.gridView1.SelectionChanged += new DevExpress.Data.SelectionChangedEventHandler(this.gridView1_SelectionChanged);
            this.gridView1.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridView1_CustomUnboundColumnData);
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
            this.colKskType.Width = 130;
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
            this.colConclusion.Width = 220;
            // 
            // colConcluder
            // 
            this.colConcluder.Caption = "Người kết luận";
            this.colConcluder.FieldName = "CONCLUDER_USERNAME";
            this.colConcluder.Name = "colConcluder";
            this.colConcluder.OptionsColumn.AllowEdit = false;
            this.colConcluder.Visible = true;
            this.colConcluder.VisibleIndex = 10;
            this.colConcluder.Width = 150;
            // 
            // colRoom
            // 
            this.colRoom.Caption = "Phòng thực hiện";
            this.colRoom.FieldName = "EXECUTE_ROOM_NAME";
            this.colRoom.Name = "colRoom";
            this.colRoom.OptionsColumn.AllowEdit = false;
            this.colRoom.Visible = true;
            this.colRoom.VisibleIndex = 11;
            this.colRoom.Width = 150;
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
            this.colSyncTime.Width = 100;
            // 
            // colFailReason
            // 
            this.colFailReason.Caption = "Lý do lỗi";
            this.colFailReason.FieldName = "SYNC_FAILD_REASON";
            this.colFailReason.Name = "colFailReason";
            this.colFailReason.OptionsColumn.AllowEdit = false;
            this.colFailReason.Visible = true;
            this.colFailReason.VisibleIndex = 14;
            this.colFailReason.Width = 250;
            // 
            // colTransaction
            // 
            this.colTransaction.Caption = "Mã giao dịch";
            this.colTransaction.FieldName = "TRANSACTION_CODE";
            this.colTransaction.Name = "colTransaction";
            this.colTransaction.OptionsColumn.AllowEdit = false;
            this.colTransaction.Visible = true;
            this.colTransaction.VisibleIndex = 15;
            this.colTransaction.Width = 150;
            // 
            // colPush
            // 
            this.colPush.Caption = "Đẩy";
            this.colPush.ColumnEdit = this.repositoryItemButtonEdit_PUSH;
            this.colPush.FieldName = "colPush";
            this.colPush.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Right;
            this.colPush.MaxWidth = 30;
            this.colPush.MinWidth = 30;
            this.colPush.Name = "colPush";
            this.colPush.OptionsColumn.ShowCaption = false;
            this.colPush.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.colPush.Visible = true;
            this.colPush.VisibleIndex = 16;
            this.colPush.Width = 30;
            // 
            // repositoryItemButtonEdit_PUSH
            // 
            this.repositoryItemButtonEdit_PUSH.AutoHeight = false;
            this.repositoryItemButtonEdit_PUSH.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, ((System.Drawing.Image)(resources.GetObject("repositoryItemButtonEdit_PUSH.Buttons"))), new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject25, serializableAppearanceObject26, serializableAppearanceObject27, serializableAppearanceObject28, "Đẩy / đẩy lại riêng hồ sơ này lên cổng", null, null, true)});
            this.repositoryItemButtonEdit_PUSH.Name = "repositoryItemButtonEdit_PUSH";
            this.repositoryItemButtonEdit_PUSH.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            // 
            // colPreview
            // 
            this.colPreview.Caption = "Xem";
            this.colPreview.ColumnEdit = this.repositoryItemButtonEdit_PREVIEW;
            this.colPreview.FieldName = "colPreview";
            this.colPreview.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Right;
            this.colPreview.MaxWidth = 30;
            this.colPreview.MinWidth = 30;
            this.colPreview.Name = "colPreview";
            this.colPreview.OptionsColumn.ShowCaption = false;
            this.colPreview.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.colPreview.Visible = true;
            this.colPreview.VisibleIndex = 17;
            this.colPreview.Width = 30;
            // 
            // repositoryItemButtonEdit_PREVIEW
            // 
            this.repositoryItemButtonEdit_PREVIEW.AutoHeight = false;
            this.repositoryItemButtonEdit_PREVIEW.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, ((System.Drawing.Image)(resources.GetObject("repositoryItemButtonEdit_PREVIEW.Buttons"))), new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject29, serializableAppearanceObject30, serializableAppearanceObject31, serializableAppearanceObject32, "Xem dữ liệu sẽ đẩy", null, null, true)});
            this.repositoryItemButtonEdit_PREVIEW.Name = "repositoryItemButtonEdit_PREVIEW";
            this.repositoryItemButtonEdit_PREVIEW.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            // 
            // ucPaging
            // 
            this.ucPaging.Location = new System.Drawing.Point(4, 716);
            this.ucPaging.Name = "ucPaging";
            this.ucPaging.Size = new System.Drawing.Size(1176, 20);
            this.ucPaging.TabIndex = 2;
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.grpFilter,
            this.lciGrid,
            this.lciPaging});
            this.Root.Location = new System.Drawing.Point(0, 0);
            this.Root.Name = "Root";
            this.Root.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 2);
            this.Root.Size = new System.Drawing.Size(1184, 740);
            this.Root.TextVisible = false;
            // 
            // grpFilter
            // 
            this.grpFilter.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.grpFilter.GroupBordersVisible = false;
            this.grpFilter.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciKskType,
            this.lciConclusionFrom,
            this.lciConclusionTo,
            this.lciSyncStatus,
            this.lciBtnVlgStatus,
            this.emptyFilterTop,
            this.lciPatientCode,
            this.lciTreatmentCode,
            this.lciKeyWord,
            this.lciBtnSearch,
            this.lciBtnRefresh,
            this.lciChkSign,
            this.lciBtnPreview,
            this.lciBtnSync,
            this.layoutControlItem1,
            this.lciBtnExportPath,
            this.lciBtnClsMap,
            this.emptySpaceItem1});
            this.grpFilter.Location = new System.Drawing.Point(0, 0);
            this.grpFilter.Name = "grpFilter";
            this.grpFilter.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.grpFilter.Size = new System.Drawing.Size(1180, 66);
            this.grpFilter.TextVisible = false;
            // 
            // lciKskType
            // 
            this.lciKskType.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciKskType.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciKskType.Control = this.cboKskType;
            this.lciKskType.Location = new System.Drawing.Point(0, 0);
            this.lciKskType.Name = "lciKskType";
            this.lciKskType.Size = new System.Drawing.Size(240, 26);
            this.lciKskType.Text = "Loại KSK:";
            this.lciKskType.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciKskType.TextSize = new System.Drawing.Size(74, 13);
            this.lciKskType.TextToControlDistance = 5;
            // 
            // lciConclusionFrom
            // 
            this.lciConclusionFrom.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciConclusionFrom.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciConclusionFrom.Control = this.dtConclusionFrom;
            this.lciConclusionFrom.Location = new System.Drawing.Point(240, 0);
            this.lciConclusionFrom.Name = "lciConclusionFrom";
            this.lciConclusionFrom.Size = new System.Drawing.Size(274, 26);
            this.lciConclusionFrom.Text = "Ngày kết luận từ:";
            this.lciConclusionFrom.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciConclusionFrom.TextSize = new System.Drawing.Size(94, 13);
            this.lciConclusionFrom.TextToControlDistance = 5;
            // 
            // lciConclusionTo
            // 
            this.lciConclusionTo.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciConclusionTo.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciConclusionTo.Control = this.dtConclusionTo;
            this.lciConclusionTo.Location = new System.Drawing.Point(514, 0);
            this.lciConclusionTo.Name = "lciConclusionTo";
            this.lciConclusionTo.Size = new System.Drawing.Size(225, 26);
            this.lciConclusionTo.Text = "đến:";
            this.lciConclusionTo.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciConclusionTo.TextSize = new System.Drawing.Size(41, 13);
            this.lciConclusionTo.TextToControlDistance = 5;
            // 
            // lciSyncStatus
            // 
            this.lciSyncStatus.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciSyncStatus.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciSyncStatus.Control = this.cboSyncStatus;
            this.lciSyncStatus.Location = new System.Drawing.Point(739, 0);
            this.lciSyncStatus.Name = "lciSyncStatus";
            this.lciSyncStatus.Size = new System.Drawing.Size(172, 26);
            this.lciSyncStatus.Text = "Trạng thái đẩy:";
            this.lciSyncStatus.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciSyncStatus.TextSize = new System.Drawing.Size(96, 13);
            this.lciSyncStatus.TextToControlDistance = 5;
            // 
            // lciBtnVlgStatus
            // 
            this.lciBtnVlgStatus.Control = this.btnVlgStatus;
            this.lciBtnVlgStatus.Location = new System.Drawing.Point(911, 0);
            this.lciBtnVlgStatus.Name = "lciBtnVlgStatus";
            this.lciBtnVlgStatus.Size = new System.Drawing.Size(143, 26);
            this.lciBtnVlgStatus.TextSize = new System.Drawing.Size(0, 0);
            this.lciBtnVlgStatus.TextVisible = false;
            this.lciBtnVlgStatus.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            // 
            // emptyFilterTop
            // 
            this.emptyFilterTop.AllowHotTrack = false;
            this.emptyFilterTop.Location = new System.Drawing.Point(913, 26);
            this.emptyFilterTop.Name = "emptyFilterTop";
            this.emptyFilterTop.Size = new System.Drawing.Size(132, 10);
            this.emptyFilterTop.TextSize = new System.Drawing.Size(0, 0);
            // 
            // lciPatientCode
            // 
            this.lciPatientCode.Control = this.txtPatientCode;
            this.lciPatientCode.Location = new System.Drawing.Point(0, 26);
            this.lciPatientCode.Name = "lciPatientCode";
            this.lciPatientCode.Size = new System.Drawing.Size(171, 36);
            this.lciPatientCode.TextSize = new System.Drawing.Size(0, 0);
            this.lciPatientCode.TextVisible = false;
            // 
            // lciTreatmentCode
            // 
            this.lciTreatmentCode.Control = this.txtTreatmentCode;
            this.lciTreatmentCode.Location = new System.Drawing.Point(171, 26);
            this.lciTreatmentCode.Name = "lciTreatmentCode";
            this.lciTreatmentCode.Size = new System.Drawing.Size(171, 36);
            this.lciTreatmentCode.TextSize = new System.Drawing.Size(0, 0);
            this.lciTreatmentCode.TextVisible = false;
            // 
            // lciKeyWord
            // 
            this.lciKeyWord.Control = this.txtKeyWord;
            this.lciKeyWord.Location = new System.Drawing.Point(342, 26);
            this.lciKeyWord.Name = "lciKeyWord";
            this.lciKeyWord.Size = new System.Drawing.Size(242, 36);
            this.lciKeyWord.TextSize = new System.Drawing.Size(0, 0);
            this.lciKeyWord.TextVisible = false;
            // 
            // lciBtnSearch
            // 
            this.lciBtnSearch.Control = this.btnSearch;
            this.lciBtnSearch.Location = new System.Drawing.Point(584, 26);
            this.lciBtnSearch.Name = "lciBtnSearch";
            this.lciBtnSearch.Size = new System.Drawing.Size(131, 36);
            this.lciBtnSearch.TextSize = new System.Drawing.Size(0, 0);
            this.lciBtnSearch.TextVisible = false;
            // 
            // lciBtnRefresh
            // 
            this.lciBtnRefresh.Control = this.btnRefresh;
            this.lciBtnRefresh.Location = new System.Drawing.Point(715, 26);
            this.lciBtnRefresh.Name = "lciBtnRefresh";
            this.lciBtnRefresh.Size = new System.Drawing.Size(100, 36);
            this.lciBtnRefresh.TextSize = new System.Drawing.Size(0, 0);
            this.lciBtnRefresh.TextVisible = false;
            // 
            // lciChkSign
            // 
            this.lciChkSign.Control = this.chkSign;
            this.lciChkSign.Location = new System.Drawing.Point(815, 26);
            this.lciChkSign.Name = "lciChkSign";
            this.lciChkSign.Size = new System.Drawing.Size(70, 36);
            this.lciChkSign.TextSize = new System.Drawing.Size(0, 0);
            this.lciChkSign.TextVisible = false;
            // 
            // lciBtnPreview
            // 
            this.lciBtnPreview.Control = this.btnPreview;
            this.lciBtnPreview.Location = new System.Drawing.Point(941, 36);
            this.lciBtnPreview.Name = "lciBtnPreview";
            this.lciBtnPreview.Size = new System.Drawing.Size(104, 26);
            this.lciBtnPreview.TextSize = new System.Drawing.Size(0, 0);
            this.lciBtnPreview.TextVisible = false;
            // 
            // lciBtnSync
            // 
            this.lciBtnSync.Control = this.btnSync;
            this.lciBtnSync.Location = new System.Drawing.Point(1045, 26);
            this.lciBtnSync.Name = "lciBtnSync";
            this.lciBtnSync.Size = new System.Drawing.Size(131, 36);
            this.lciBtnSync.TextSize = new System.Drawing.Size(0, 0);
            this.lciBtnSync.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.btnSettings;
            this.layoutControlItem1.Location = new System.Drawing.Point(885, 26);
            this.layoutControlItem1.MaxSize = new System.Drawing.Size(28, 26);
            this.layoutControlItem1.MinSize = new System.Drawing.Size(28, 26);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(28, 36);
            this.layoutControlItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem1.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextToControlDistance = 0;
            this.layoutControlItem1.TextVisible = false;
            // 
            // lciBtnExportPath
            // 
            this.lciBtnExportPath.Control = this.btnExportPath;
            this.lciBtnExportPath.Location = new System.Drawing.Point(913, 36);
            this.lciBtnExportPath.MaxSize = new System.Drawing.Size(28, 26);
            this.lciBtnExportPath.MinSize = new System.Drawing.Size(28, 26);
            this.lciBtnExportPath.Name = "lciBtnExportPath";
            this.lciBtnExportPath.Size = new System.Drawing.Size(28, 26);
            this.lciBtnExportPath.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciBtnExportPath.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciBtnExportPath.TextSize = new System.Drawing.Size(0, 0);
            this.lciBtnExportPath.TextToControlDistance = 0;
            this.lciBtnExportPath.TextVisible = false;
            // 
            // lciBtnClsMap
            // 
            this.lciBtnClsMap.Control = this.btnClsMap;
            this.lciBtnClsMap.Location = new System.Drawing.Point(1064, 0);
            this.lciBtnClsMap.Name = "lciBtnClsMap";
            this.lciBtnClsMap.Size = new System.Drawing.Size(112, 26);
            this.lciBtnClsMap.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciBtnClsMap.TextSize = new System.Drawing.Size(0, 0);
            this.lciBtnClsMap.TextToControlDistance = 0;
            this.lciBtnClsMap.TextVisible = false;
            // 
            // lciGrid
            // 
            this.lciGrid.Control = this.gridControl1;
            this.lciGrid.Location = new System.Drawing.Point(0, 66);
            this.lciGrid.Name = "lciGrid";
            this.lciGrid.Size = new System.Drawing.Size(1180, 646);
            this.lciGrid.TextSize = new System.Drawing.Size(0, 0);
            this.lciGrid.TextVisible = false;
            // 
            // lciPaging
            // 
            this.lciPaging.Control = this.ucPaging;
            this.lciPaging.Location = new System.Drawing.Point(0, 712);
            this.lciPaging.Name = "lciPaging";
            this.lciPaging.Size = new System.Drawing.Size(1180, 24);
            this.lciPaging.TextSize = new System.Drawing.Size(0, 0);
            this.lciPaging.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(1054, 0);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(10, 26);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // UCKskSyncList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layoutControl1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Name = "UCKskSyncList";
            this.Size = new System.Drawing.Size(1184, 769);
            this.Load += new System.EventHandler(this.UCKskSyncList_Load);
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
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
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemButtonEdit_PREVIEW)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpFilter)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciKskType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciConclusionFrom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciConclusionTo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciSyncStatus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnVlgStatus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptyFilterTop)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPatientCode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTreatmentCode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciKeyWord)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnSearch)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnRefresh)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciChkSign)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnPreview)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnSync)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnExportPath)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnClsMap)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPaging)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
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
        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraEditors.ComboBoxEdit cboKskType;
        private DevExpress.XtraEditors.DateEdit dtConclusionFrom;
        private DevExpress.XtraEditors.DateEdit dtConclusionTo;
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
        private DevExpress.XtraGrid.Columns.GridColumn colPreview;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit repositoryItemButtonEdit_PREVIEW;
        private Inventec.UC.Paging.UcPaging ucPaging;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraLayout.LayoutControlGroup grpFilter;
        private DevExpress.XtraLayout.LayoutControlItem lciKskType;
        private DevExpress.XtraLayout.LayoutControlItem lciConclusionFrom;
        private DevExpress.XtraLayout.LayoutControlItem lciConclusionTo;
        private DevExpress.XtraLayout.LayoutControlItem lciSyncStatus;
        private DevExpress.XtraEditors.SimpleButton btnVlgStatus;
        private DevExpress.XtraLayout.LayoutControlItem lciBtnVlgStatus;
        private DevExpress.XtraLayout.EmptySpaceItem emptyFilterTop;
        private DevExpress.XtraLayout.LayoutControlItem lciPatientCode;
        private DevExpress.XtraLayout.LayoutControlItem lciTreatmentCode;
        private DevExpress.XtraLayout.LayoutControlItem lciKeyWord;
        private DevExpress.XtraLayout.LayoutControlItem lciBtnSearch;
        private DevExpress.XtraLayout.LayoutControlItem lciBtnRefresh;
        private DevExpress.XtraLayout.LayoutControlItem lciChkSign;
        private DevExpress.XtraLayout.LayoutControlItem lciBtnPreview;
        private DevExpress.XtraLayout.LayoutControlItem lciBtnSync;
        private DevExpress.XtraLayout.LayoutControlItem lciGrid;
        private DevExpress.XtraLayout.LayoutControlItem lciPaging;
        private DevExpress.XtraEditors.SimpleButton btnSettings;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraEditors.SimpleButton btnExportPath;
        private DevExpress.XtraLayout.LayoutControlItem lciBtnExportPath;
        private DevExpress.XtraEditors.SimpleButton btnClsMap;
        private DevExpress.XtraLayout.LayoutControlItem lciBtnClsMap;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
    }
}
