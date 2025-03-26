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
namespace Inventec.UC.EventLogControl.Design.Template1
{
    partial class Template1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Template1));
            this.txtKeyWord = new DevExpress.XtraEditors.TextEdit();
            this.nbControlFilter = new DevExpress.XtraNavBar.NavBarControl();
            this.nbGroupTimeCreate = new DevExpress.XtraNavBar.NavBarGroup();
            this.navBarGroupControlContainer1 = new DevExpress.XtraNavBar.NavBarGroupControlContainer();
            this.dtTimeTo = new DevExpress.XtraEditors.DateEdit();
            this.lblTimeTo = new DevExpress.XtraEditors.LabelControl();
            this.dtTimeFrom = new DevExpress.XtraEditors.DateEdit();
            this.lblTimeFrom = new DevExpress.XtraEditors.LabelControl();
            this.btnSearch = new DevExpress.XtraEditors.SimpleButton();
            this.btnRefresh = new DevExpress.XtraEditors.SimpleButton();
            this.gridControlEventLog = new DevExpress.XtraGrid.GridControl();
            this.gridViewEventLog = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn4 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn5 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemMemoEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit();
            this.gridColumn6 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn7 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn8 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn9 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.btnFirstPage = new DevExpress.XtraEditors.SimpleButton();
            this.btnPreviousPage = new DevExpress.XtraEditors.SimpleButton();
            this.txtCurrentPage = new DevExpress.XtraEditors.TextEdit();
            this.lblTotalPage = new DevExpress.XtraEditors.LabelControl();
            this.btnNextPage = new DevExpress.XtraEditors.SimpleButton();
            this.btnLastPage = new DevExpress.XtraEditors.SimpleButton();
            this.btnRefreshPage = new DevExpress.XtraEditors.SimpleButton();
            this.txtPageSize = new DevExpress.XtraEditors.ComboBoxEdit();
            this.lblDisplayPageNo = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)(this.txtKeyWord.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nbControlFilter)).BeginInit();
            this.nbControlFilter.SuspendLayout();
            this.navBarGroupControlContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dtTimeTo.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtTimeTo.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtTimeFrom.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtTimeFrom.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlEventLog)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewEventLog)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemMemoEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCurrentPage.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPageSize.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // txtKeyWord
            // 
            this.txtKeyWord.Location = new System.Drawing.Point(5, 5);
            this.txtKeyWord.Name = "txtKeyWord";
            this.txtKeyWord.Properties.NullValuePrompt = "Từ khóa tìm kiếm";
            this.txtKeyWord.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtKeyWord.Properties.ShowNullValuePromptWhenFocused = true;
            this.txtKeyWord.Size = new System.Drawing.Size(230, 20);
            this.txtKeyWord.TabIndex = 0;
            this.txtKeyWord.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtKeyWord_PreviewKeyDown);
            // 
            // nbControlFilter
            // 
            this.nbControlFilter.ActiveGroup = this.nbGroupTimeCreate;
            this.nbControlFilter.Controls.Add(this.navBarGroupControlContainer1);
            this.nbControlFilter.Groups.AddRange(new DevExpress.XtraNavBar.NavBarGroup[] {
            this.nbGroupTimeCreate});
            this.nbControlFilter.Location = new System.Drawing.Point(5, 30);
            this.nbControlFilter.Name = "nbControlFilter";
            this.nbControlFilter.OptionsNavPane.ExpandedWidth = 230;
            this.nbControlFilter.Size = new System.Drawing.Size(230, 465);
            this.nbControlFilter.TabIndex = 1;
            this.nbControlFilter.Text = "navBarControl1";
            // 
            // nbGroupTimeCreate
            // 
            this.nbGroupTimeCreate.Caption = "Thời gian tạo";
            this.nbGroupTimeCreate.ControlContainer = this.navBarGroupControlContainer1;
            this.nbGroupTimeCreate.Expanded = true;
            this.nbGroupTimeCreate.GroupClientHeight = 80;
            this.nbGroupTimeCreate.GroupStyle = DevExpress.XtraNavBar.NavBarGroupStyle.ControlContainer;
            this.nbGroupTimeCreate.Name = "nbGroupTimeCreate";
            // 
            // navBarGroupControlContainer1
            // 
            this.navBarGroupControlContainer1.Appearance.BackColor = System.Drawing.SystemColors.Control;
            this.navBarGroupControlContainer1.Appearance.Options.UseBackColor = true;
            this.navBarGroupControlContainer1.Controls.Add(this.dtTimeTo);
            this.navBarGroupControlContainer1.Controls.Add(this.lblTimeTo);
            this.navBarGroupControlContainer1.Controls.Add(this.dtTimeFrom);
            this.navBarGroupControlContainer1.Controls.Add(this.lblTimeFrom);
            this.navBarGroupControlContainer1.Name = "navBarGroupControlContainer1";
            this.navBarGroupControlContainer1.Size = new System.Drawing.Size(222, 76);
            this.navBarGroupControlContainer1.TabIndex = 0;
            // 
            // dtTimeTo
            // 
            this.dtTimeTo.EditValue = null;
            this.dtTimeTo.Location = new System.Drawing.Point(70, 42);
            this.dtTimeTo.Name = "dtTimeTo";
            this.dtTimeTo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtTimeTo.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtTimeTo.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
            this.dtTimeTo.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.dtTimeTo.Properties.EditFormat.FormatString = "dd/MM/yyyy";
            this.dtTimeTo.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.dtTimeTo.Size = new System.Drawing.Size(145, 20);
            this.dtTimeTo.TabIndex = 3;
            // 
            // lblTimeTo
            // 
            this.lblTimeTo.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lblTimeTo.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblTimeTo.Location = new System.Drawing.Point(14, 45);
            this.lblTimeTo.Name = "lblTimeTo";
            this.lblTimeTo.Size = new System.Drawing.Size(50, 13);
            this.lblTimeTo.TabIndex = 2;
            this.lblTimeTo.Text = "Đến:";
            // 
            // dtTimeFrom
            // 
            this.dtTimeFrom.EditValue = null;
            this.dtTimeFrom.Location = new System.Drawing.Point(70, 12);
            this.dtTimeFrom.Name = "dtTimeFrom";
            this.dtTimeFrom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtTimeFrom.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtTimeFrom.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
            this.dtTimeFrom.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.dtTimeFrom.Properties.EditFormat.FormatString = "dd/MM/yyyy";
            this.dtTimeFrom.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.dtTimeFrom.Properties.Mask.EditMask = "dd/MM/yyyy";
            this.dtTimeFrom.Size = new System.Drawing.Size(145, 20);
            this.dtTimeFrom.TabIndex = 1;
            this.dtTimeFrom.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.dtTimeFrom_Closed);
            // 
            // lblTimeFrom
            // 
            this.lblTimeFrom.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lblTimeFrom.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblTimeFrom.Location = new System.Drawing.Point(14, 15);
            this.lblTimeFrom.Name = "lblTimeFrom";
            this.lblTimeFrom.Size = new System.Drawing.Size(50, 13);
            this.lblTimeFrom.TabIndex = 0;
            this.lblTimeFrom.Text = "Từ:";
            // 
            // btnSearch
            // 
            this.btnSearch.Image = ((System.Drawing.Image)(resources.GetObject("btnSearch.Image")));
            this.btnSearch.Location = new System.Drawing.Point(5, 500);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(115, 35);
            this.btnSearch.TabIndex = 2;
            this.btnSearch.Text = "Tìm (Ctrl S)";
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Image = ((System.Drawing.Image)(resources.GetObject("btnRefresh.Image")));
            this.btnRefresh.Location = new System.Drawing.Point(120, 500);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(115, 35);
            this.btnRefresh.TabIndex = 3;
            this.btnRefresh.Text = "Làm lại (Ctrl R)";
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // gridControlEventLog
            // 
            this.gridControlEventLog.Location = new System.Drawing.Point(240, 5);
            this.gridControlEventLog.MainView = this.gridViewEventLog;
            this.gridControlEventLog.Name = "gridControlEventLog";
            this.gridControlEventLog.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemMemoEdit1});
            this.gridControlEventLog.Size = new System.Drawing.Size(1110, 490);
            this.gridControlEventLog.TabIndex = 4;
            this.gridControlEventLog.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewEventLog});
            // 
            // gridViewEventLog
            // 
            this.gridViewEventLog.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn1,
            this.gridColumn2,
            this.gridColumn3,
            this.gridColumn4,
            this.gridColumn5,
            this.gridColumn6,
            this.gridColumn7,
            this.gridColumn8,
            this.gridColumn9});
            this.gridViewEventLog.GridControl = this.gridControlEventLog;
            this.gridViewEventLog.Name = "gridViewEventLog";
            this.gridViewEventLog.OptionsBehavior.AllowAddRows = DevExpress.Utils.DefaultBoolean.False;
            this.gridViewEventLog.OptionsBehavior.AllowDeleteRows = DevExpress.Utils.DefaultBoolean.False;
            this.gridViewEventLog.OptionsView.ColumnAutoWidth = false;
            this.gridViewEventLog.OptionsView.RowAutoHeight = true;
            this.gridViewEventLog.OptionsView.ShowIndicator = false;
            this.gridViewEventLog.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewEventLog_CustomUnboundColumnData);
            // 
            // gridColumn1
            // 
            this.gridColumn1.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn1.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn1.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn1.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn1.Caption = "STT";
            this.gridColumn1.FieldName = "STT";
            this.gridColumn1.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.OptionsColumn.AllowEdit = false;
            this.gridColumn1.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn1.Visible = true;
            this.gridColumn1.VisibleIndex = 0;
            this.gridColumn1.Width = 40;
            // 
            // gridColumn2
            // 
            this.gridColumn2.Caption = "Tài khoản";
            this.gridColumn2.FieldName = "LOGIN_NAME";
            this.gridColumn2.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gridColumn2.Name = "gridColumn2";
            this.gridColumn2.OptionsColumn.AllowEdit = false;
            this.gridColumn2.Visible = true;
            this.gridColumn2.VisibleIndex = 1;
            this.gridColumn2.Width = 100;
            // 
            // gridColumn3
            // 
            this.gridColumn3.Caption = "Loại";
            this.gridColumn3.FieldName = "EVENT_LOG_TYPE_NAME";
            this.gridColumn3.Name = "gridColumn3";
            this.gridColumn3.OptionsColumn.AllowEdit = false;
            // 
            // gridColumn4
            // 
            this.gridColumn4.Caption = "IP";
            this.gridColumn4.FieldName = "IP";
            this.gridColumn4.Name = "gridColumn4";
            this.gridColumn4.OptionsColumn.AllowEdit = false;
            this.gridColumn4.Visible = true;
            this.gridColumn4.VisibleIndex = 2;
            this.gridColumn4.Width = 100;
            // 
            // gridColumn5
            // 
            this.gridColumn5.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn5.AppearanceCell.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.gridColumn5.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn5.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn5.Caption = "Mô tả";
            this.gridColumn5.ColumnEdit = this.repositoryItemMemoEdit1;
            this.gridColumn5.FieldName = "DESCRIPTION";
            this.gridColumn5.Name = "gridColumn5";
            this.gridColumn5.OptionsColumn.AllowEdit = false;
            this.gridColumn5.Visible = true;
            this.gridColumn5.VisibleIndex = 3;
            this.gridColumn5.Width = 500;
            // 
            // repositoryItemMemoEdit1
            // 
            this.repositoryItemMemoEdit1.Name = "repositoryItemMemoEdit1";
            // 
            // gridColumn6
            // 
            this.gridColumn6.Caption = "Thời gian tạo";
            this.gridColumn6.FieldName = "LOG_CREATE_TIME";
            this.gridColumn6.Name = "gridColumn6";
            this.gridColumn6.OptionsColumn.AllowEdit = false;
            this.gridColumn6.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn6.Visible = true;
            this.gridColumn6.VisibleIndex = 4;
            this.gridColumn6.Width = 120;
            // 
            // gridColumn7
            // 
            this.gridColumn7.Caption = "Người tạo";
            this.gridColumn7.FieldName = "CREATOR";
            this.gridColumn7.Name = "gridColumn7";
            this.gridColumn7.OptionsColumn.AllowEdit = false;
            this.gridColumn7.Visible = true;
            this.gridColumn7.VisibleIndex = 5;
            this.gridColumn7.Width = 100;
            // 
            // gridColumn8
            // 
            this.gridColumn8.Caption = "Thời gian sửa";
            this.gridColumn8.FieldName = "LOG_MODIFY_TIME";
            this.gridColumn8.Name = "gridColumn8";
            this.gridColumn8.OptionsColumn.AllowEdit = false;
            this.gridColumn8.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn8.Visible = true;
            this.gridColumn8.VisibleIndex = 6;
            this.gridColumn8.Width = 120;
            // 
            // gridColumn9
            // 
            this.gridColumn9.Caption = "Người sửa";
            this.gridColumn9.FieldName = "MODIFIER";
            this.gridColumn9.Name = "gridColumn9";
            this.gridColumn9.OptionsColumn.AllowEdit = false;
            this.gridColumn9.Visible = true;
            this.gridColumn9.VisibleIndex = 7;
            this.gridColumn9.Width = 100;
            // 
            // btnFirstPage
            // 
            this.btnFirstPage.Image = ((System.Drawing.Image)(resources.GetObject("btnFirstPage.Image")));
            this.btnFirstPage.Location = new System.Drawing.Point(240, 505);
            this.btnFirstPage.Name = "btnFirstPage";
            this.btnFirstPage.Size = new System.Drawing.Size(25, 23);
            this.btnFirstPage.TabIndex = 5;
            this.btnFirstPage.Click += new System.EventHandler(this.btnFirstPage_Click);
            // 
            // btnPreviousPage
            // 
            this.btnPreviousPage.Image = ((System.Drawing.Image)(resources.GetObject("btnPreviousPage.Image")));
            this.btnPreviousPage.Location = new System.Drawing.Point(270, 505);
            this.btnPreviousPage.Name = "btnPreviousPage";
            this.btnPreviousPage.Size = new System.Drawing.Size(25, 23);
            this.btnPreviousPage.TabIndex = 6;
            this.btnPreviousPage.Click += new System.EventHandler(this.btnPreviousPage_Click);
            // 
            // txtCurrentPage
            // 
            this.txtCurrentPage.Location = new System.Drawing.Point(300, 505);
            this.txtCurrentPage.Name = "txtCurrentPage";
            this.txtCurrentPage.Properties.AutoHeight = false;
            this.txtCurrentPage.Size = new System.Drawing.Size(50, 23);
            this.txtCurrentPage.TabIndex = 7;
            // 
            // lblTotalPage
            // 
            this.lblTotalPage.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblTotalPage.Location = new System.Drawing.Point(355, 507);
            this.lblTotalPage.Name = "lblTotalPage";
            this.lblTotalPage.Size = new System.Drawing.Size(60, 20);
            this.lblTotalPage.TabIndex = 8;
            this.lblTotalPage.Text = "/1234";
            // 
            // btnNextPage
            // 
            this.btnNextPage.Image = ((System.Drawing.Image)(resources.GetObject("btnNextPage.Image")));
            this.btnNextPage.Location = new System.Drawing.Point(420, 505);
            this.btnNextPage.Name = "btnNextPage";
            this.btnNextPage.Size = new System.Drawing.Size(25, 23);
            this.btnNextPage.TabIndex = 9;
            this.btnNextPage.Click += new System.EventHandler(this.btnNextPage_Click);
            // 
            // btnLastPage
            // 
            this.btnLastPage.Image = ((System.Drawing.Image)(resources.GetObject("btnLastPage.Image")));
            this.btnLastPage.Location = new System.Drawing.Point(450, 505);
            this.btnLastPage.Name = "btnLastPage";
            this.btnLastPage.Size = new System.Drawing.Size(25, 23);
            this.btnLastPage.TabIndex = 10;
            this.btnLastPage.Click += new System.EventHandler(this.btnLastPage_Click);
            // 
            // btnRefreshPage
            // 
            this.btnRefreshPage.Image = ((System.Drawing.Image)(resources.GetObject("btnRefreshPage.Image")));
            this.btnRefreshPage.Location = new System.Drawing.Point(480, 505);
            this.btnRefreshPage.Name = "btnRefreshPage";
            this.btnRefreshPage.Size = new System.Drawing.Size(25, 23);
            this.btnRefreshPage.TabIndex = 11;
            this.btnRefreshPage.Click += new System.EventHandler(this.btnRefreshPage_Click);
            // 
            // txtPageSize
            // 
            this.txtPageSize.EditValue = "100";
            this.txtPageSize.Location = new System.Drawing.Point(510, 505);
            this.txtPageSize.Name = "txtPageSize";
            this.txtPageSize.Properties.AutoHeight = false;
            this.txtPageSize.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.txtPageSize.Properties.Items.AddRange(new object[] {
            "10",
            "20",
            "50",
            "100",
            "500"});
            this.txtPageSize.Size = new System.Drawing.Size(50, 23);
            this.txtPageSize.TabIndex = 12;
            this.txtPageSize.EditValueChanged += new System.EventHandler(this.txtPageSize_EditValueChanged);
            // 
            // lblDisplayPageNo
            // 
            this.lblDisplayPageNo.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lblDisplayPageNo.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblDisplayPageNo.Location = new System.Drawing.Point(1200, 507);
            this.lblDisplayPageNo.Name = "lblDisplayPageNo";
            this.lblDisplayPageNo.Size = new System.Drawing.Size(150, 20);
            this.lblDisplayPageNo.TabIndex = 13;
            this.lblDisplayPageNo.Text = "1-100/1000";
            // 
            // Template1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.lblDisplayPageNo);
            this.Controls.Add(this.txtPageSize);
            this.Controls.Add(this.btnRefreshPage);
            this.Controls.Add(this.btnLastPage);
            this.Controls.Add(this.btnNextPage);
            this.Controls.Add(this.lblTotalPage);
            this.Controls.Add(this.txtCurrentPage);
            this.Controls.Add(this.btnPreviousPage);
            this.Controls.Add(this.btnFirstPage);
            this.Controls.Add(this.gridControlEventLog);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.nbControlFilter);
            this.Controls.Add(this.txtKeyWord);
            this.Name = "Template1";
            this.Size = new System.Drawing.Size(1353, 538);
            ((System.ComponentModel.ISupportInitialize)(this.txtKeyWord.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nbControlFilter)).EndInit();
            this.nbControlFilter.ResumeLayout(false);
            this.navBarGroupControlContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dtTimeTo.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtTimeTo.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtTimeFrom.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtTimeFrom.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlEventLog)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewEventLog)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemMemoEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCurrentPage.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPageSize.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.TextEdit txtKeyWord;
        private DevExpress.XtraNavBar.NavBarControl nbControlFilter;
        private DevExpress.XtraNavBar.NavBarGroup nbGroupTimeCreate;
        private DevExpress.XtraNavBar.NavBarGroupControlContainer navBarGroupControlContainer1;
        private DevExpress.XtraEditors.DateEdit dtTimeTo;
        private DevExpress.XtraEditors.LabelControl lblTimeTo;
        private DevExpress.XtraEditors.DateEdit dtTimeFrom;
        private DevExpress.XtraEditors.LabelControl lblTimeFrom;
        private DevExpress.XtraEditors.SimpleButton btnSearch;
        private DevExpress.XtraEditors.SimpleButton btnRefresh;
        private DevExpress.XtraGrid.GridControl gridControlEventLog;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewEventLog;
        private DevExpress.XtraEditors.SimpleButton btnFirstPage;
        private DevExpress.XtraEditors.SimpleButton btnPreviousPage;
        private DevExpress.XtraEditors.TextEdit txtCurrentPage;
        private DevExpress.XtraEditors.LabelControl lblTotalPage;
        private DevExpress.XtraEditors.SimpleButton btnNextPage;
        private DevExpress.XtraEditors.SimpleButton btnLastPage;
        private DevExpress.XtraEditors.SimpleButton btnRefreshPage;
        private DevExpress.XtraEditors.ComboBoxEdit txtPageSize;
        private DevExpress.XtraEditors.LabelControl lblDisplayPageNo;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn3;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn4;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn5;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn6;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn7;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn8;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn9;
        private DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit repositoryItemMemoEdit1;
    }
}
