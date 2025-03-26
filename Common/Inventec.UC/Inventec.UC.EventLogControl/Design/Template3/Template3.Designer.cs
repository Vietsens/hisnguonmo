/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *  
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *  
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * GNU General Public License for more details.
 *  
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */
namespace Inventec.UC.EventLogControl.Design.Template3
{
    partial class Template3
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.ucPaging = new Inventec.UC.Paging.UcPaging();
            this.BtnRefresh = new DevExpress.XtraEditors.SimpleButton();
            this.BtnSearch = new DevExpress.XtraEditors.SimpleButton();
            this.gridControlEventLog = new DevExpress.XtraGrid.GridControl();
            this.gridViewEventLog = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.Gc_Stt = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Gc_EventTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Gc_LoginName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Gc_Ip = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Gc_AppCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Gc_Description = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemMemoEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit();
            this.Gc_LogCreateTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.DtTimeTo = new DevExpress.XtraEditors.DateEdit();
            this.DtTimeFrom = new DevExpress.XtraEditors.DateEdit();
            this.TxtKeyWord = new DevExpress.XtraEditors.TextEdit();
            this.DropBtnSearch = new DevExpress.XtraEditors.DropDownButton();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.LciDtTimeFrom = new DevExpress.XtraLayout.LayoutControlItem();
            this.LciDtTimeTo = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem7 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.barButtonItem2 = new DevExpress.XtraBars.BarButtonItem();
            this.barButtonItem3 = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.barButtonItem1 = new DevExpress.XtraBars.BarButtonItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlEventLog)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewEventLog)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemMemoEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DtTimeTo.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DtTimeTo.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DtTimeFrom.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DtTimeFrom.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TxtKeyWord.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.LciDtTimeFrom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.LciDtTimeTo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.ucPaging);
            this.layoutControl1.Controls.Add(this.BtnRefresh);
            this.layoutControl1.Controls.Add(this.BtnSearch);
            this.layoutControl1.Controls.Add(this.gridControlEventLog);
            this.layoutControl1.Controls.Add(this.DtTimeTo);
            this.layoutControl1.Controls.Add(this.DtTimeFrom);
            this.layoutControl1.Controls.Add(this.TxtKeyWord);
            this.layoutControl1.Controls.Add(this.DropBtnSearch);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 29);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(1360, 521);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // ucPaging
            // 
            this.ucPaging.Location = new System.Drawing.Point(2, 496);
            this.ucPaging.Name = "ucPaging";
            this.ucPaging.Size = new System.Drawing.Size(1356, 23);
            this.ucPaging.TabIndex = 12;
            // 
            // BtnRefresh
            // 
            this.BtnRefresh.Location = new System.Drawing.Point(1162, 2);
            this.BtnRefresh.Name = "BtnRefresh";
            this.BtnRefresh.Size = new System.Drawing.Size(106, 22);
            this.BtnRefresh.StyleController = this.layoutControl1;
            this.BtnRefresh.TabIndex = 10;
            this.BtnRefresh.Text = "Làm lại (Ctrl R)";
            this.BtnRefresh.Click += new System.EventHandler(this.BtnRefresh_Click);
            // 
            // BtnSearch
            // 
            this.BtnSearch.Location = new System.Drawing.Point(1052, 2);
            this.BtnSearch.Name = "BtnSearch";
            this.BtnSearch.Size = new System.Drawing.Size(106, 22);
            this.BtnSearch.StyleController = this.layoutControl1;
            this.BtnSearch.TabIndex = 9;
            this.BtnSearch.Text = "Tìm (Ctrl F)";
            this.BtnSearch.Click += new System.EventHandler(this.BtnSearch_Click);
            // 
            // gridControlEventLog
            // 
            this.gridControlEventLog.Location = new System.Drawing.Point(2, 28);
            this.gridControlEventLog.MainView = this.gridViewEventLog;
            this.gridControlEventLog.Name = "gridControlEventLog";
            this.gridControlEventLog.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemMemoEdit1});
            this.gridControlEventLog.Size = new System.Drawing.Size(1356, 464);
            this.gridControlEventLog.TabIndex = 8;
            this.gridControlEventLog.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewEventLog});
            // 
            // gridViewEventLog
            // 
            this.gridViewEventLog.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.Gc_Stt,
            this.Gc_EventTime,
            this.Gc_LoginName,
            this.Gc_Ip,
            this.Gc_AppCode,
            this.Gc_Description,
            this.Gc_LogCreateTime});
            this.gridViewEventLog.GridControl = this.gridControlEventLog;
            this.gridViewEventLog.Name = "gridViewEventLog";
            this.gridViewEventLog.OptionsView.RowAutoHeight = true;
            this.gridViewEventLog.OptionsView.ShowGroupPanel = false;
            this.gridViewEventLog.OptionsView.ShowIndicator = false;
            this.gridViewEventLog.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewEventLog_CustomUnboundColumnData);
            // 
            // Gc_Stt
            // 
            this.Gc_Stt.AppearanceCell.Options.UseTextOptions = true;
            this.Gc_Stt.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.Gc_Stt.AppearanceHeader.Options.UseTextOptions = true;
            this.Gc_Stt.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.Gc_Stt.Caption = "STT";
            this.Gc_Stt.FieldName = "STT";
            this.Gc_Stt.Name = "Gc_Stt";
            this.Gc_Stt.OptionsColumn.AllowEdit = false;
            this.Gc_Stt.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True;
            this.Gc_Stt.OptionsColumn.FixedWidth = true;
            this.Gc_Stt.SortMode = DevExpress.XtraGrid.ColumnSortMode.Value;
            this.Gc_Stt.ToolTip = "Số thứ tự";
            this.Gc_Stt.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.Gc_Stt.Visible = true;
            this.Gc_Stt.VisibleIndex = 0;
            this.Gc_Stt.Width = 30;
            // 
            // Gc_EventTime
            // 
            this.Gc_EventTime.Caption = "Thời gian";
            this.Gc_EventTime.FieldName = "EVENT_TIME_DISPLAY";
            this.Gc_EventTime.Name = "Gc_EventTime";
            this.Gc_EventTime.OptionsColumn.AllowEdit = false;
            this.Gc_EventTime.OptionsColumn.FixedWidth = true;
            this.Gc_EventTime.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.Gc_EventTime.Visible = true;
            this.Gc_EventTime.VisibleIndex = 1;
            this.Gc_EventTime.Width = 130;
            // 
            // Gc_LoginName
            // 
            this.Gc_LoginName.Caption = "Tài khoản";
            this.Gc_LoginName.FieldName = "LOGIN_NAME";
            this.Gc_LoginName.Name = "Gc_LoginName";
            this.Gc_LoginName.OptionsColumn.AllowEdit = false;
            this.Gc_LoginName.OptionsColumn.FixedWidth = true;
            this.Gc_LoginName.Visible = true;
            this.Gc_LoginName.VisibleIndex = 2;
            this.Gc_LoginName.Width = 100;
            // 
            // Gc_Ip
            // 
            this.Gc_Ip.Caption = "IP";
            this.Gc_Ip.FieldName = "IP";
            this.Gc_Ip.Name = "Gc_Ip";
            this.Gc_Ip.OptionsColumn.AllowEdit = false;
            this.Gc_Ip.OptionsColumn.FixedWidth = true;
            this.Gc_Ip.Visible = true;
            this.Gc_Ip.VisibleIndex = 3;
            this.Gc_Ip.Width = 100;
            // 
            // Gc_AppCode
            // 
            this.Gc_AppCode.Caption = "Ứng dụng";
            this.Gc_AppCode.FieldName = "APP_CODE";
            this.Gc_AppCode.Name = "Gc_AppCode";
            this.Gc_AppCode.OptionsColumn.AllowEdit = false;
            this.Gc_AppCode.OptionsColumn.FixedWidth = true;
            this.Gc_AppCode.Visible = true;
            this.Gc_AppCode.VisibleIndex = 4;
            this.Gc_AppCode.Width = 60;
            // 
            // Gc_Description
            // 
            this.Gc_Description.AppearanceCell.Options.UseTextOptions = true;
            this.Gc_Description.AppearanceCell.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.Gc_Description.AppearanceHeader.Options.UseTextOptions = true;
            this.Gc_Description.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.Gc_Description.Caption = "Mô tả";
            this.Gc_Description.ColumnEdit = this.repositoryItemMemoEdit1;
            this.Gc_Description.FieldName = "DESCRIPTION";
            this.Gc_Description.Name = "Gc_Description";
            this.Gc_Description.Visible = true;
            this.Gc_Description.VisibleIndex = 5;
            this.Gc_Description.Width = 794;
            // 
            // repositoryItemMemoEdit1
            // 
            this.repositoryItemMemoEdit1.Name = "repositoryItemMemoEdit1";
            this.repositoryItemMemoEdit1.ReadOnly = true;
            // 
            // Gc_LogCreateTime
            // 
            this.Gc_LogCreateTime.Caption = "Thời gian tạo";
            this.Gc_LogCreateTime.FieldName = "LOG_CREATE_TIME";
            this.Gc_LogCreateTime.Name = "Gc_LogCreateTime";
            this.Gc_LogCreateTime.OptionsColumn.AllowEdit = false;
            this.Gc_LogCreateTime.OptionsColumn.FixedWidth = true;
            this.Gc_LogCreateTime.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.Gc_LogCreateTime.Visible = true;
            this.Gc_LogCreateTime.VisibleIndex = 6;
            this.Gc_LogCreateTime.Width = 130;
            // 
            // DtTimeTo
            // 
            this.DtTimeTo.EditValue = null;
            this.DtTimeTo.Location = new System.Drawing.Point(937, 2);
            this.DtTimeTo.Name = "DtTimeTo";
            this.DtTimeTo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.DtTimeTo.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.DtTimeTo.Properties.DisplayFormat.FormatString = "dd/MM/yyyy HH:mm";
            this.DtTimeTo.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.DtTimeTo.Properties.EditFormat.FormatString = "dd/MM/yyyy HH:mm";
            this.DtTimeTo.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.DtTimeTo.Properties.Mask.EditMask = "dd/MM/yyyy HH:mm";
            this.DtTimeTo.Size = new System.Drawing.Size(111, 20);
            this.DtTimeTo.StyleController = this.layoutControl1;
            this.DtTimeTo.TabIndex = 7;
            this.DtTimeTo.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.DtTimeTo_Closed);
            this.DtTimeTo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DtTimeTo_KeyDown);
            // 
            // DtTimeFrom
            // 
            this.DtTimeFrom.EditValue = null;
            this.DtTimeFrom.Location = new System.Drawing.Point(767, 2);
            this.DtTimeFrom.Name = "DtTimeFrom";
            this.DtTimeFrom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.DtTimeFrom.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.DtTimeFrom.Properties.DisplayFormat.FormatString = "dd/MM/yyyy HH:mm";
            this.DtTimeFrom.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.DtTimeFrom.Properties.EditFormat.FormatString = "dd/MM/yyyy HH:mm";
            this.DtTimeFrom.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.DtTimeFrom.Properties.Mask.EditMask = "dd/MM/yyyy HH:mm";
            this.DtTimeFrom.Size = new System.Drawing.Size(111, 20);
            this.DtTimeFrom.StyleController = this.layoutControl1;
            this.DtTimeFrom.TabIndex = 6;
            this.DtTimeFrom.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.DtTimeFrom_Closed);
            this.DtTimeFrom.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.DtTimeFrom_PreviewKeyDown);
            // 
            // TxtKeyWord
            // 
            this.TxtKeyWord.Location = new System.Drawing.Point(162, 2);
            this.TxtKeyWord.Name = "TxtKeyWord";
            this.TxtKeyWord.Properties.NullValuePrompt = "Từ khóa tìm kiếm";
            this.TxtKeyWord.Properties.NullValuePromptShowForEmptyValue = true;
            this.TxtKeyWord.Properties.ShowNullValuePromptWhenFocused = true;
            this.TxtKeyWord.Size = new System.Drawing.Size(546, 20);
            this.TxtKeyWord.StyleController = this.layoutControl1;
            this.TxtKeyWord.TabIndex = 5;
            this.TxtKeyWord.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.TxtKeyWord_PreviewKeyDown);
            // 
            // DropBtnSearch
            // 
            this.DropBtnSearch.Location = new System.Drawing.Point(2, 2);
            this.DropBtnSearch.Name = "DropBtnSearch";
            this.DropBtnSearch.Size = new System.Drawing.Size(156, 20);
            this.DropBtnSearch.StyleController = this.layoutControl1;
            this.DropBtnSearch.TabIndex = 4;
            this.DropBtnSearch.Click += new System.EventHandler(this.DropBtnSearch_Click);
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.layoutControlItem2,
            this.LciDtTimeFrom,
            this.LciDtTimeTo,
            this.layoutControlItem5,
            this.layoutControlItem6,
            this.layoutControlItem7,
            this.emptySpaceItem1,
            this.layoutControlItem4});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(1360, 521);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.DropBtnSearch;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(160, 26);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.TxtKeyWord;
            this.layoutControlItem2.Location = new System.Drawing.Point(160, 0);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(550, 26);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // LciDtTimeFrom
            // 
            this.LciDtTimeFrom.AppearanceItemCaption.Options.UseTextOptions = true;
            this.LciDtTimeFrom.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.LciDtTimeFrom.Control = this.DtTimeFrom;
            this.LciDtTimeFrom.Location = new System.Drawing.Point(710, 0);
            this.LciDtTimeFrom.Name = "LciDtTimeFrom";
            this.LciDtTimeFrom.Size = new System.Drawing.Size(170, 26);
            this.LciDtTimeFrom.Text = "Từ:";
            this.LciDtTimeFrom.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.LciDtTimeFrom.TextSize = new System.Drawing.Size(50, 20);
            this.LciDtTimeFrom.TextToControlDistance = 5;
            // 
            // LciDtTimeTo
            // 
            this.LciDtTimeTo.AppearanceItemCaption.Options.UseTextOptions = true;
            this.LciDtTimeTo.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.LciDtTimeTo.Control = this.DtTimeTo;
            this.LciDtTimeTo.Location = new System.Drawing.Point(880, 0);
            this.LciDtTimeTo.Name = "LciDtTimeTo";
            this.LciDtTimeTo.Size = new System.Drawing.Size(170, 26);
            this.LciDtTimeTo.Text = "Đến:";
            this.LciDtTimeTo.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.LciDtTimeTo.TextSize = new System.Drawing.Size(50, 20);
            this.LciDtTimeTo.TextToControlDistance = 5;
            // 
            // layoutControlItem5
            // 
            this.layoutControlItem5.Control = this.gridControlEventLog;
            this.layoutControlItem5.Location = new System.Drawing.Point(0, 26);
            this.layoutControlItem5.Name = "layoutControlItem5";
            this.layoutControlItem5.Size = new System.Drawing.Size(1360, 468);
            this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem5.TextVisible = false;
            // 
            // layoutControlItem6
            // 
            this.layoutControlItem6.Control = this.BtnSearch;
            this.layoutControlItem6.Location = new System.Drawing.Point(1050, 0);
            this.layoutControlItem6.Name = "layoutControlItem6";
            this.layoutControlItem6.Size = new System.Drawing.Size(110, 26);
            this.layoutControlItem6.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem6.TextVisible = false;
            // 
            // layoutControlItem7
            // 
            this.layoutControlItem7.Control = this.BtnRefresh;
            this.layoutControlItem7.Location = new System.Drawing.Point(1160, 0);
            this.layoutControlItem7.Name = "layoutControlItem7";
            this.layoutControlItem7.Size = new System.Drawing.Size(110, 26);
            this.layoutControlItem7.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem7.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(1270, 0);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(90, 26);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.Control = this.ucPaging;
            this.layoutControlItem4.Location = new System.Drawing.Point(0, 494);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Size = new System.Drawing.Size(1360, 27);
            this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem4.TextVisible = false;
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
            this.barButtonItem1,
            this.barButtonItem2,
            this.barButtonItem3});
            this.barManager1.MaxItemId = 3;
            // 
            // bar1
            // 
            this.bar1.BarName = "Tools";
            this.bar1.DockCol = 0;
            this.bar1.DockRow = 0;
            this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.barButtonItem2),
            new DevExpress.XtraBars.LinkPersistInfo(this.barButtonItem3)});
            this.bar1.Text = "Tools";
            this.bar1.Visible = false;
            // 
            // barButtonItem2
            // 
            this.barButtonItem2.Caption = "Search(ctrl F)";
            this.barButtonItem2.Id = 1;
            this.barButtonItem2.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F));
            this.barButtonItem2.Name = "barButtonItem2";
            this.barButtonItem2.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem2_ItemClick);
            // 
            // barButtonItem3
            // 
            this.barButtonItem3.Caption = "Refresh (Ctrl R)";
            this.barButtonItem3.Id = 2;
            this.barButtonItem3.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R));
            this.barButtonItem3.Name = "barButtonItem3";
            this.barButtonItem3.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem3_ItemClick);
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Size = new System.Drawing.Size(1360, 29);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 550);
            this.barDockControlBottom.Size = new System.Drawing.Size(1360, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 29);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 521);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(1360, 29);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 521);
            // 
            // barButtonItem1
            // 
            this.barButtonItem1.Caption = "barButtonItem1";
            this.barButtonItem1.Id = 0;
            this.barButtonItem1.Name = "barButtonItem1";
            // 
            // Template3
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layoutControl1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Name = "Template3";
            this.Size = new System.Drawing.Size(1360, 550);
            this.Load += new System.EventHandler(this.Template3_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControlEventLog)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewEventLog)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemMemoEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DtTimeTo.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DtTimeTo.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DtTimeFrom.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DtTimeFrom.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TxtKeyWord.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.LciDtTimeFrom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.LciDtTimeTo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.DropDownButton DropBtnSearch;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraEditors.TextEdit TxtKeyWord;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraEditors.DateEdit DtTimeTo;
        private DevExpress.XtraEditors.DateEdit DtTimeFrom;
        private DevExpress.XtraLayout.LayoutControlItem LciDtTimeFrom;
        private DevExpress.XtraLayout.LayoutControlItem LciDtTimeTo;
        private DevExpress.XtraGrid.GridControl gridControlEventLog;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewEventLog;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
        private DevExpress.XtraEditors.SimpleButton BtnRefresh;
        private DevExpress.XtraEditors.SimpleButton BtnSearch;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem6;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem7;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private Paging.UcPaging ucPaging;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraGrid.Columns.GridColumn Gc_Stt;
        private DevExpress.XtraGrid.Columns.GridColumn Gc_EventTime;
        private DevExpress.XtraGrid.Columns.GridColumn Gc_LoginName;
        private DevExpress.XtraGrid.Columns.GridColumn Gc_Ip;
        private DevExpress.XtraGrid.Columns.GridColumn Gc_AppCode;
        private DevExpress.XtraGrid.Columns.GridColumn Gc_Description;
        private DevExpress.XtraGrid.Columns.GridColumn Gc_LogCreateTime;
        private DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit repositoryItemMemoEdit1;
        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.Bar bar1;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraBars.BarButtonItem barButtonItem2;
        private DevExpress.XtraBars.BarButtonItem barButtonItem1;
        private DevExpress.XtraBars.BarButtonItem barButtonItem3;
    }
}
