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
namespace Inventec.UC.ListReports.Form
{
    partial class frmShare
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
            this.components = new System.ComponentModel.Container();
            this.LayoutReportCode = new DevExpress.XtraEditors.LabelControl();
            this.lblReportCode = new DevExpress.XtraEditors.LabelControl();
            this.LayoutReportName = new DevExpress.XtraEditors.LabelControl();
            this.lblReportName = new DevExpress.XtraEditors.LabelControl();
            this.LayoutReportStt = new DevExpress.XtraEditors.LabelControl();
            this.lblReportStt = new DevExpress.XtraEditors.LabelControl();
            this.gridControlUserList = new DevExpress.XtraGrid.GridControl();
            this.gridViewUserList = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn4 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn5 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.checkSelected = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.gridControlSelectedUser = new DevExpress.XtraGrid.GridControl();
            this.gridViewSelectedUser = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn6 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn7 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn8 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn9 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.btnSave = new DevExpress.XtraEditors.SimpleButton();
            this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.bbtnSave = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.txtSearch = new DevExpress.XtraEditors.TextEdit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlUserList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewUserList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkSelected)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlSelectedUser)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewSelectedUser)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSearch.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // LayoutReportCode
            // 
            this.LayoutReportCode.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.LayoutReportCode.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.LayoutReportCode.Location = new System.Drawing.Point(24, 10);
            this.LayoutReportCode.Name = "LayoutReportCode";
            this.LayoutReportCode.Size = new System.Drawing.Size(70, 13);
            this.LayoutReportCode.TabIndex = 0;
            this.LayoutReportCode.Text = "Mã báo cáo:";
            // 
            // lblReportCode
            // 
            this.lblReportCode.Location = new System.Drawing.Point(100, 10);
            this.lblReportCode.Name = "lblReportCode";
            this.lblReportCode.Size = new System.Drawing.Size(63, 13);
            this.lblReportCode.TabIndex = 1;
            this.lblReportCode.Text = "labelControl1";
            // 
            // LayoutReportName
            // 
            this.LayoutReportName.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.LayoutReportName.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.LayoutReportName.Location = new System.Drawing.Point(244, 10);
            this.LayoutReportName.Name = "LayoutReportName";
            this.LayoutReportName.Size = new System.Drawing.Size(70, 13);
            this.LayoutReportName.TabIndex = 2;
            this.LayoutReportName.Text = "Tên báo cáo:";
            // 
            // lblReportName
            // 
            this.lblReportName.Location = new System.Drawing.Point(320, 10);
            this.lblReportName.Name = "lblReportName";
            this.lblReportName.Size = new System.Drawing.Size(63, 13);
            this.lblReportName.TabIndex = 3;
            this.lblReportName.Text = "labelControl1";
            // 
            // LayoutReportStt
            // 
            this.LayoutReportStt.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.LayoutReportStt.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.LayoutReportStt.Location = new System.Drawing.Point(590, 10);
            this.LayoutReportStt.Name = "LayoutReportStt";
            this.LayoutReportStt.Size = new System.Drawing.Size(70, 13);
            this.LayoutReportStt.TabIndex = 4;
            this.LayoutReportStt.Text = "Trạng thái:";
            // 
            // lblReportStt
            // 
            this.lblReportStt.Location = new System.Drawing.Point(666, 10);
            this.lblReportStt.Name = "lblReportStt";
            this.lblReportStt.Size = new System.Drawing.Size(63, 13);
            this.lblReportStt.TabIndex = 5;
            this.lblReportStt.Text = "labelControl1";
            // 
            // gridControlUserList
            // 
            this.gridControlUserList.Location = new System.Drawing.Point(5, 55);
            this.gridControlUserList.MainView = this.gridViewUserList;
            this.gridControlUserList.Name = "gridControlUserList";
            this.gridControlUserList.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.checkSelected});
            this.gridControlUserList.Size = new System.Drawing.Size(435, 300);
            this.gridControlUserList.TabIndex = 6;
            this.gridControlUserList.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewUserList});
            // 
            // gridViewUserList
            // 
            this.gridViewUserList.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn1,
            this.gridColumn2,
            this.gridColumn3,
            this.gridColumn4,
            this.gridColumn5});
            this.gridViewUserList.GridControl = this.gridControlUserList;
            this.gridViewUserList.Name = "gridViewUserList";
            this.gridViewUserList.OptionsBehavior.AllowAddRows = DevExpress.Utils.DefaultBoolean.False;
            this.gridViewUserList.OptionsBehavior.AllowDeleteRows = DevExpress.Utils.DefaultBoolean.False;
            this.gridViewUserList.OptionsFind.AllowFindPanel = false;
            this.gridViewUserList.OptionsFind.FindFilterColumns = "";
            this.gridViewUserList.OptionsFind.FindNullPrompt = "";
            this.gridViewUserList.OptionsFind.HighlightFindResults = false;
            this.gridViewUserList.OptionsFind.ShowClearButton = false;
            this.gridViewUserList.OptionsFind.ShowCloseButton = false;
            this.gridViewUserList.OptionsFind.ShowFindButton = false;
            this.gridViewUserList.OptionsSelection.CheckBoxSelectorColumnWidth = 30;
            this.gridViewUserList.OptionsSelection.MultiSelect = true;
            this.gridViewUserList.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;
            this.gridViewUserList.OptionsSelection.ResetSelectionClickOutsideCheckboxSelector = true;
            this.gridViewUserList.OptionsView.ColumnAutoWidth = false;
            this.gridViewUserList.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            this.gridViewUserList.OptionsView.ShowGroupPanel = false;
            this.gridViewUserList.OptionsView.ShowIndicator = false;
            this.gridViewUserList.CustomRowCellEdit += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gridViewUserList_CustomRowCellEdit);
            this.gridViewUserList.SelectionChanged += new DevExpress.Data.SelectionChangedEventHandler(this.gridViewUserList_SelectionChanged);
            // 
            // gridColumn1
            // 
            this.gridColumn1.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn1.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn1.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn1.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn1.FieldName = "SELECTED";
            this.gridColumn1.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.Width = 30;
            // 
            // gridColumn2
            // 
            this.gridColumn2.Caption = "Tên đăng nhập";
            this.gridColumn2.FieldName = "LOGINNAME";
            this.gridColumn2.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gridColumn2.Name = "gridColumn2";
            this.gridColumn2.OptionsColumn.AllowEdit = false;
            this.gridColumn2.Visible = true;
            this.gridColumn2.VisibleIndex = 1;
            this.gridColumn2.Width = 100;
            // 
            // gridColumn3
            // 
            this.gridColumn3.Caption = "Họ tên";
            this.gridColumn3.FieldName = "USERNAME";
            this.gridColumn3.Name = "gridColumn3";
            this.gridColumn3.OptionsColumn.AllowEdit = false;
            this.gridColumn3.Visible = true;
            this.gridColumn3.VisibleIndex = 2;
            this.gridColumn3.Width = 170;
            // 
            // gridColumn4
            // 
            this.gridColumn4.Caption = "Email";
            this.gridColumn4.FieldName = "EMAIL";
            this.gridColumn4.Name = "gridColumn4";
            this.gridColumn4.OptionsColumn.AllowEdit = false;
            this.gridColumn4.Visible = true;
            this.gridColumn4.VisibleIndex = 3;
            this.gridColumn4.Width = 100;
            // 
            // gridColumn5
            // 
            this.gridColumn5.Caption = "Điện thoại";
            this.gridColumn5.FieldName = "MOBILE";
            this.gridColumn5.Name = "gridColumn5";
            this.gridColumn5.OptionsColumn.AllowEdit = false;
            this.gridColumn5.Visible = true;
            this.gridColumn5.VisibleIndex = 4;
            this.gridColumn5.Width = 100;
            // 
            // checkSelected
            // 
            this.checkSelected.AutoHeight = false;
            this.checkSelected.Name = "checkSelected";
            this.checkSelected.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
            this.checkSelected.CheckedChanged += new System.EventHandler(this.checkSelected_CheckedChanged);
            // 
            // gridControlSelectedUser
            // 
            this.gridControlSelectedUser.Location = new System.Drawing.Point(445, 55);
            this.gridControlSelectedUser.MainView = this.gridViewSelectedUser;
            this.gridControlSelectedUser.Name = "gridControlSelectedUser";
            this.gridControlSelectedUser.Size = new System.Drawing.Size(435, 300);
            this.gridControlSelectedUser.TabIndex = 7;
            this.gridControlSelectedUser.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewSelectedUser});
            // 
            // gridViewSelectedUser
            // 
            this.gridViewSelectedUser.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn6,
            this.gridColumn7,
            this.gridColumn8,
            this.gridColumn9});
            this.gridViewSelectedUser.GridControl = this.gridControlSelectedUser;
            this.gridViewSelectedUser.Name = "gridViewSelectedUser";
            this.gridViewSelectedUser.OptionsView.ColumnAutoWidth = false;
            this.gridViewSelectedUser.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            this.gridViewSelectedUser.OptionsView.ShowGroupPanel = false;
            this.gridViewSelectedUser.OptionsView.ShowIndicator = false;
            // 
            // gridColumn6
            // 
            this.gridColumn6.Caption = "Tên đăng nhập";
            this.gridColumn6.FieldName = "LOGINNAME";
            this.gridColumn6.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gridColumn6.Name = "gridColumn6";
            this.gridColumn6.OptionsColumn.AllowEdit = false;
            this.gridColumn6.Visible = true;
            this.gridColumn6.VisibleIndex = 0;
            this.gridColumn6.Width = 100;
            // 
            // gridColumn7
            // 
            this.gridColumn7.Caption = "Họ tên";
            this.gridColumn7.FieldName = "USERNAME";
            this.gridColumn7.Name = "gridColumn7";
            this.gridColumn7.OptionsColumn.AllowEdit = false;
            this.gridColumn7.Visible = true;
            this.gridColumn7.VisibleIndex = 1;
            this.gridColumn7.Width = 170;
            // 
            // gridColumn8
            // 
            this.gridColumn8.Caption = "Email";
            this.gridColumn8.FieldName = "EMAIL";
            this.gridColumn8.Name = "gridColumn8";
            this.gridColumn8.OptionsColumn.AllowEdit = false;
            this.gridColumn8.Visible = true;
            this.gridColumn8.VisibleIndex = 2;
            this.gridColumn8.Width = 100;
            // 
            // gridColumn9
            // 
            this.gridColumn9.Caption = "Điện thoại";
            this.gridColumn9.FieldName = "MOBILE";
            this.gridColumn9.Name = "gridColumn9";
            this.gridColumn9.OptionsColumn.AllowEdit = false;
            this.gridColumn9.Visible = true;
            this.gridColumn9.VisibleIndex = 3;
            this.gridColumn9.Width = 100;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(770, 361);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(110, 26);
            this.btnSave.TabIndex = 8;
            this.btnSave.Text = "Lưu (Ctrl S)";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
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
            this.bbtnSave});
            this.barManager1.MaxItemId = 1;
            // 
            // bar1
            // 
            this.bar1.BarName = "Tools";
            this.bar1.DockCol = 0;
            this.bar1.DockRow = 0;
            this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.bbtnSave)});
            this.bar1.Text = "Tools";
            this.bar1.Visible = false;
            // 
            // bbtnSave
            // 
            this.bbtnSave.Caption = "Lưu (Ctrl S)";
            this.bbtnSave.Id = 0;
            this.bbtnSave.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S));
            this.bbtnSave.Name = "bbtnSave";
            this.bbtnSave.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bbtnSave_ItemClick);
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Size = new System.Drawing.Size(884, 29);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 391);
            this.barDockControlBottom.Size = new System.Drawing.Size(884, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 29);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 362);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(884, 29);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 362);
            // 
            // txtSearch
            // 
            this.txtSearch.Location = new System.Drawing.Point(5, 29);
            this.txtSearch.MenuManager = this.barManager1;
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Properties.NullValuePrompt = "Từ khóa tìm kiếm";
            this.txtSearch.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtSearch.Properties.ShowNullValuePromptWhenFocused = true;
            this.txtSearch.Size = new System.Drawing.Size(435, 20);
            this.txtSearch.TabIndex = 13;
            this.txtSearch.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtSearch_PreviewKeyDown);
            // 
            // frmShare
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(884, 391);
            this.Controls.Add(this.txtSearch);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.gridControlSelectedUser);
            this.Controls.Add(this.gridControlUserList);
            this.Controls.Add(this.lblReportStt);
            this.Controls.Add(this.LayoutReportStt);
            this.Controls.Add(this.lblReportName);
            this.Controls.Add(this.LayoutReportName);
            this.Controls.Add(this.lblReportCode);
            this.Controls.Add(this.LayoutReportCode);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Name = "frmShare";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Gửi";
            this.Load += new System.EventHandler(this.frmShare_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridControlUserList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewUserList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkSelected)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlSelectedUser)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewSelectedUser)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSearch.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.LabelControl LayoutReportCode;
        private DevExpress.XtraEditors.LabelControl lblReportCode;
        private DevExpress.XtraEditors.LabelControl LayoutReportName;
        private DevExpress.XtraEditors.LabelControl lblReportName;
        private DevExpress.XtraEditors.LabelControl LayoutReportStt;
        private DevExpress.XtraEditors.LabelControl lblReportStt;
        private DevExpress.XtraGrid.GridControl gridControlUserList;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewUserList;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit checkSelected;
        private DevExpress.XtraGrid.GridControl gridControlSelectedUser;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewSelectedUser;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn6;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn7;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn8;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn9;
        private DevExpress.XtraEditors.SimpleButton btnSave;
        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.Bar bar1;
        private DevExpress.XtraBars.BarButtonItem bbtnSave;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn3;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn4;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn5;
        private DevExpress.XtraEditors.TextEdit txtSearch;
    }
}
