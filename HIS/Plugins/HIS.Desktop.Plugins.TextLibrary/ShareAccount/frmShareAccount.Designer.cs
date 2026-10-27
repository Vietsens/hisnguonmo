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
namespace HIS.Desktop.Plugins.TextLibrary.ShareAccount
{
    partial class frmShareAccount
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
            this.layoutControlRoot = new DevExpress.XtraLayout.LayoutControl();
            this.btnClose = new DevExpress.XtraEditors.SimpleButton();
            this.btnOk = new DevExpress.XtraEditors.SimpleButton();
            this.chkSelectAll = new DevExpress.XtraEditors.CheckEdit();
            this.cboDepartment = new DevExpress.XtraEditors.GridLookUpEdit();
            this.cboDepartmentView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.txtKeyword = new DevExpress.XtraEditors.TextEdit();
            this.gridControlAccount = new DevExpress.XtraGrid.GridControl();
            this.gridViewAccount = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gcIsChecked = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemChkChoose = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.gcEmployeeCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcUserName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcLoginName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcDepartmentName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciKeyword = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciDepartment = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciSelectAll = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciGrid = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciBtnOk = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciBtnClose = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItemButton = new DevExpress.XtraLayout.EmptySpaceItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlRoot)).BeginInit();
            this.layoutControlRoot.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chkSelectAll.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboDepartment.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboDepartmentView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtKeyword.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlAccount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewAccount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemChkChoose)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciKeyword)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDepartment)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciSelectAll)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnOk)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnClose)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItemButton)).BeginInit();
            this.SuspendLayout();
            //
            // layoutControlRoot
            //
            this.layoutControlRoot.Controls.Add(this.btnClose);
            this.layoutControlRoot.Controls.Add(this.btnOk);
            this.layoutControlRoot.Controls.Add(this.chkSelectAll);
            this.layoutControlRoot.Controls.Add(this.cboDepartment);
            this.layoutControlRoot.Controls.Add(this.txtKeyword);
            this.layoutControlRoot.Controls.Add(this.gridControlAccount);
            this.layoutControlRoot.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControlRoot.Location = new System.Drawing.Point(0, 0);
            this.layoutControlRoot.Name = "layoutControlRoot";
            this.layoutControlRoot.Root = this.Root;
            this.layoutControlRoot.Size = new System.Drawing.Size(700, 450);
            this.layoutControlRoot.TabIndex = 0;
            this.layoutControlRoot.Text = "layoutControl1";
            //
            // btnClose
            //
            this.btnClose.Location = new System.Drawing.Point(612, 426);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(86, 22);
            this.btnClose.StyleController = this.layoutControlRoot;
            this.btnClose.TabIndex = 6;
            this.btnClose.Text = "Đóng";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            //
            // btnOk
            //
            this.btnOk.Location = new System.Drawing.Point(522, 426);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(86, 22);
            this.btnOk.StyleController = this.layoutControlRoot;
            this.btnOk.TabIndex = 5;
            this.btnOk.Text = "Đồng ý";
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            //
            // chkSelectAll
            //
            this.chkSelectAll.Location = new System.Drawing.Point(572, 2);
            this.chkSelectAll.Name = "chkSelectAll";
            this.chkSelectAll.Properties.Caption = "Chọn tất cả";
            this.chkSelectAll.Size = new System.Drawing.Size(126, 19);
            this.chkSelectAll.StyleController = this.layoutControlRoot;
            this.chkSelectAll.TabIndex = 3;
            this.chkSelectAll.CheckedChanged += new System.EventHandler(this.chkSelectAll_CheckedChanged);
            //
            // cboDepartment
            //
            this.cboDepartment.Location = new System.Drawing.Point(377, 2);
            this.cboDepartment.Name = "cboDepartment";
            this.cboDepartment.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.cboDepartment.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)});
            this.cboDepartment.Properties.NullText = "";
            this.cboDepartment.Properties.View = this.cboDepartmentView;
            this.cboDepartment.Size = new System.Drawing.Size(191, 20);
            this.cboDepartment.StyleController = this.layoutControlRoot;
            this.cboDepartment.TabIndex = 2;
            this.cboDepartment.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboDepartment_ButtonClick);
            this.cboDepartment.EditValueChanged += new System.EventHandler(this.cboDepartment_EditValueChanged);
            //
            // cboDepartmentView
            //
            this.cboDepartmentView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.cboDepartmentView.Name = "cboDepartmentView";
            this.cboDepartmentView.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.cboDepartmentView.OptionsView.ShowGroupPanel = false;
            //
            // txtKeyword
            //
            this.txtKeyword.Location = new System.Drawing.Point(2, 2);
            this.txtKeyword.Name = "txtKeyword";
            this.txtKeyword.Properties.NullValuePrompt = "Mã NV, họ tên, tên đăng nhập";
            this.txtKeyword.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtKeyword.Properties.ShowNullValuePromptWhenFocused = true;
            this.txtKeyword.Size = new System.Drawing.Size(296, 20);
            this.txtKeyword.StyleController = this.layoutControlRoot;
            this.txtKeyword.TabIndex = 1;
            this.txtKeyword.TextChanged += new System.EventHandler(this.txtKeyword_TextChanged);
            //
            // gridControlAccount
            //
            this.gridControlAccount.Location = new System.Drawing.Point(2, 28);
            this.gridControlAccount.MainView = this.gridViewAccount;
            this.gridControlAccount.Name = "gridControlAccount";
            this.gridControlAccount.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemChkChoose});
            this.gridControlAccount.Size = new System.Drawing.Size(696, 394);
            this.gridControlAccount.TabIndex = 4;
            this.gridControlAccount.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewAccount});
            //
            // gridViewAccount
            //
            this.gridViewAccount.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gcIsChecked,
            this.gcEmployeeCode,
            this.gcUserName,
            this.gcLoginName,
            this.gcDepartmentName});
            this.gridViewAccount.GridControl = this.gridControlAccount;
            this.gridViewAccount.Name = "gridViewAccount";
            this.gridViewAccount.OptionsBehavior.EditorShowMode = DevExpress.Utils.EditorShowMode.MouseDown;
            this.gridViewAccount.OptionsView.ShowGroupPanel = false;
            this.gridViewAccount.OptionsView.ShowIndicator = false;
            //
            // gcIsChecked
            //
            this.gcIsChecked.Caption = " ";
            this.gcIsChecked.ColumnEdit = this.repositoryItemChkChoose;
            this.gcIsChecked.FieldName = "IsChecked";
            this.gcIsChecked.Name = "gcIsChecked";
            this.gcIsChecked.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.gcIsChecked.OptionsColumn.FixedWidth = true;
            this.gcIsChecked.Visible = true;
            this.gcIsChecked.VisibleIndex = 0;
            this.gcIsChecked.Width = 30;
            //
            // repositoryItemChkChoose
            //
            this.repositoryItemChkChoose.AutoHeight = false;
            this.repositoryItemChkChoose.Name = "repositoryItemChkChoose";
            this.repositoryItemChkChoose.CheckedChanged += new System.EventHandler(this.repositoryItemChkChoose_CheckedChanged);
            //
            // gcEmployeeCode
            //
            this.gcEmployeeCode.Caption = "Mã NV";
            this.gcEmployeeCode.FieldName = "EMPLOYEE_CODE";
            this.gcEmployeeCode.Name = "gcEmployeeCode";
            this.gcEmployeeCode.OptionsColumn.AllowEdit = false;
            this.gcEmployeeCode.Visible = true;
            this.gcEmployeeCode.VisibleIndex = 1;
            this.gcEmployeeCode.Width = 100;
            //
            // gcUserName
            //
            this.gcUserName.Caption = "Họ tên";
            this.gcUserName.FieldName = "TDL_USERNAME";
            this.gcUserName.Name = "gcUserName";
            this.gcUserName.OptionsColumn.AllowEdit = false;
            this.gcUserName.Visible = true;
            this.gcUserName.VisibleIndex = 2;
            this.gcUserName.Width = 190;
            //
            // gcLoginName
            //
            this.gcLoginName.Caption = "Tên đăng nhập";
            this.gcLoginName.FieldName = "LOGINNAME";
            this.gcLoginName.Name = "gcLoginName";
            this.gcLoginName.OptionsColumn.AllowEdit = false;
            this.gcLoginName.Visible = true;
            this.gcLoginName.VisibleIndex = 3;
            this.gcLoginName.Width = 130;
            //
            // gcDepartmentName
            //
            this.gcDepartmentName.Caption = "Khoa/phòng";
            this.gcDepartmentName.FieldName = "DEPARTMENT_NAME";
            this.gcDepartmentName.Name = "gcDepartmentName";
            this.gcDepartmentName.OptionsColumn.AllowEdit = false;
            this.gcDepartmentName.Visible = true;
            this.gcDepartmentName.VisibleIndex = 4;
            this.gcDepartmentName.Width = 230;
            //
            // Root
            //
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciKeyword,
            this.lciDepartment,
            this.lciSelectAll,
            this.lciGrid,
            this.lciBtnOk,
            this.lciBtnClose,
            this.emptySpaceItemButton});
            this.Root.Location = new System.Drawing.Point(0, 0);
            this.Root.Name = "Root";
            this.Root.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.Root.Size = new System.Drawing.Size(700, 450);
            this.Root.TextVisible = false;
            //
            // lciKeyword
            //
            this.lciKeyword.Control = this.txtKeyword;
            this.lciKeyword.Location = new System.Drawing.Point(0, 0);
            this.lciKeyword.Name = "lciKeyword";
            this.lciKeyword.Size = new System.Drawing.Size(300, 26);
            this.lciKeyword.TextSize = new System.Drawing.Size(0, 0);
            this.lciKeyword.TextVisible = false;
            //
            // lciDepartment
            //
            this.lciDepartment.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciDepartment.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciDepartment.Control = this.cboDepartment;
            this.lciDepartment.Location = new System.Drawing.Point(300, 0);
            this.lciDepartment.Name = "lciDepartment";
            this.lciDepartment.Size = new System.Drawing.Size(270, 26);
            this.lciDepartment.Text = "Khoa:";
            this.lciDepartment.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciDepartment.TextSize = new System.Drawing.Size(70, 20);
            this.lciDepartment.TextToControlDistance = 5;
            //
            // lciSelectAll
            //
            this.lciSelectAll.Control = this.chkSelectAll;
            this.lciSelectAll.Location = new System.Drawing.Point(570, 0);
            this.lciSelectAll.MaxSize = new System.Drawing.Size(130, 26);
            this.lciSelectAll.MinSize = new System.Drawing.Size(130, 26);
            this.lciSelectAll.Name = "lciSelectAll";
            this.lciSelectAll.Size = new System.Drawing.Size(130, 26);
            this.lciSelectAll.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciSelectAll.TextSize = new System.Drawing.Size(0, 0);
            this.lciSelectAll.TextVisible = false;
            //
            // lciGrid
            //
            this.lciGrid.Control = this.gridControlAccount;
            this.lciGrid.Location = new System.Drawing.Point(0, 26);
            this.lciGrid.Name = "lciGrid";
            this.lciGrid.Size = new System.Drawing.Size(700, 398);
            this.lciGrid.TextSize = new System.Drawing.Size(0, 0);
            this.lciGrid.TextVisible = false;
            //
            // lciBtnOk
            //
            this.lciBtnOk.Control = this.btnOk;
            this.lciBtnOk.Location = new System.Drawing.Point(520, 424);
            this.lciBtnOk.MaxSize = new System.Drawing.Size(90, 26);
            this.lciBtnOk.MinSize = new System.Drawing.Size(90, 26);
            this.lciBtnOk.Name = "lciBtnOk";
            this.lciBtnOk.Size = new System.Drawing.Size(90, 26);
            this.lciBtnOk.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciBtnOk.TextSize = new System.Drawing.Size(0, 0);
            this.lciBtnOk.TextVisible = false;
            //
            // lciBtnClose
            //
            this.lciBtnClose.Control = this.btnClose;
            this.lciBtnClose.Location = new System.Drawing.Point(610, 424);
            this.lciBtnClose.MaxSize = new System.Drawing.Size(90, 26);
            this.lciBtnClose.MinSize = new System.Drawing.Size(90, 26);
            this.lciBtnClose.Name = "lciBtnClose";
            this.lciBtnClose.Size = new System.Drawing.Size(90, 26);
            this.lciBtnClose.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciBtnClose.TextSize = new System.Drawing.Size(0, 0);
            this.lciBtnClose.TextVisible = false;
            //
            // emptySpaceItemButton
            //
            this.emptySpaceItemButton.AllowHotTrack = false;
            this.emptySpaceItemButton.Location = new System.Drawing.Point(0, 424);
            this.emptySpaceItemButton.Name = "emptySpaceItemButton";
            this.emptySpaceItemButton.Size = new System.Drawing.Size(520, 26);
            this.emptySpaceItemButton.TextSize = new System.Drawing.Size(0, 0);
            //
            // frmShareAccount
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(700, 450);
            this.Controls.Add(this.layoutControlRoot);
            this.Name = "frmShareAccount";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Danh sách tài khoản";
            this.Load += new System.EventHandler(this.frmShareAccount_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlRoot)).EndInit();
            this.layoutControlRoot.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chkSelectAll.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboDepartment.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboDepartmentView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtKeyword.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlAccount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewAccount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemChkChoose)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciKeyword)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDepartment)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciSelectAll)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnOk)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnClose)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItemButton)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControlRoot;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraEditors.TextEdit txtKeyword;
        private DevExpress.XtraEditors.GridLookUpEdit cboDepartment;
        private DevExpress.XtraGrid.Views.Grid.GridView cboDepartmentView;
        private DevExpress.XtraEditors.CheckEdit chkSelectAll;
        private DevExpress.XtraGrid.GridControl gridControlAccount;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewAccount;
        private DevExpress.XtraGrid.Columns.GridColumn gcIsChecked;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemChkChoose;
        private DevExpress.XtraGrid.Columns.GridColumn gcEmployeeCode;
        private DevExpress.XtraGrid.Columns.GridColumn gcUserName;
        private DevExpress.XtraGrid.Columns.GridColumn gcLoginName;
        private DevExpress.XtraGrid.Columns.GridColumn gcDepartmentName;
        private DevExpress.XtraEditors.SimpleButton btnOk;
        private DevExpress.XtraEditors.SimpleButton btnClose;
        private DevExpress.XtraLayout.LayoutControlItem lciKeyword;
        private DevExpress.XtraLayout.LayoutControlItem lciDepartment;
        private DevExpress.XtraLayout.LayoutControlItem lciSelectAll;
        private DevExpress.XtraLayout.LayoutControlItem lciGrid;
        private DevExpress.XtraLayout.LayoutControlItem lciBtnOk;
        private DevExpress.XtraLayout.LayoutControlItem lciBtnClose;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItemButton;
    }
}
