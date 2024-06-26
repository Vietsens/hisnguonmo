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
namespace HIS.Desktop.Plugins.EmrDocumentType
{
    partial class frmAcsUser
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmAcsUser));
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.ucPaging1 = new Inventec.UC.Paging.UcPaging();
            this.txtKeyword = new DevExpress.XtraEditors.TextEdit();
            this.txtLoginNames = new DevExpress.XtraEditors.TextEdit();
            this.btnChoose = new DevExpress.XtraEditors.SimpleButton();
            this.gridControlSecondaryDisease = new DevExpress.XtraGrid.GridControl();
            this.gridViewSecondaryDisease = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemCheckEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.grdColCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grdColName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciGridControl = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lblIcdText = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.lciPaging = new DevExpress.XtraLayout.LayoutControlItem();
            this.barManager1 = new DevExpress.XtraBars.BarManager();
            this.bar2 = new DevExpress.XtraBars.Bar();
            this.bbtnChoose = new DevExpress.XtraBars.BarButtonItem();
            this.bbtnClose = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtKeyword.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtLoginNames.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlSecondaryDisease)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewSecondaryDisease)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciGridControl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblIcdText)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPaging)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.ucPaging1);
            this.layoutControl1.Controls.Add(this.txtKeyword);
            this.layoutControl1.Controls.Add(this.txtLoginNames);
            this.layoutControl1.Controls.Add(this.btnChoose);
            this.layoutControl1.Controls.Add(this.gridControlSecondaryDisease);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 22);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(388, 63, 250, 350);
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(914, 497);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // ucPaging1
            // 
            this.ucPaging1.Location = new System.Drawing.Point(7, 432);
            this.ucPaging1.Name = "ucPaging1";
            this.ucPaging1.Size = new System.Drawing.Size(900, 22);
            this.ucPaging1.TabIndex = 10;
            // 
            // txtKeyword
            // 
            this.txtKeyword.Location = new System.Drawing.Point(7, 7);
            this.txtKeyword.Name = "txtKeyword";
            this.txtKeyword.Properties.NullValuePrompt = "Từ khóa tìm kiếm";
            this.txtKeyword.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtKeyword.Properties.ShowNullValuePromptWhenFocused = true;
            this.txtKeyword.Size = new System.Drawing.Size(900, 20);
            this.txtKeyword.StyleController = this.layoutControl1;
            this.txtKeyword.TabIndex = 7;
            this.txtKeyword.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtKeyword_KeyUp);
            // 
            // txtLoginNames
            // 
            this.txtLoginNames.Location = new System.Drawing.Point(7, 458);
            this.txtLoginNames.Name = "txtLoginNames";
            this.txtLoginNames.Properties.NullValuePrompt = "Danh sách tài khoản";
            this.txtLoginNames.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtLoginNames.Properties.ShowNullValuePromptWhenFocused = true;
            this.txtLoginNames.Size = new System.Drawing.Size(810, 20);
            this.txtLoginNames.StyleController = this.layoutControl1;
            this.txtLoginNames.TabIndex = 6;
            // 
            // btnChoose
            // 
            this.btnChoose.Location = new System.Drawing.Point(821, 468);
            this.btnChoose.Name = "btnChoose";
            this.btnChoose.Size = new System.Drawing.Size(86, 22);
            this.btnChoose.StyleController = this.layoutControl1;
            this.btnChoose.TabIndex = 5;
            this.btnChoose.Text = "Chọn (Ctrl S)";
            this.btnChoose.Click += new System.EventHandler(this.btnChoose_Click);
            // 
            // gridControlSecondaryDisease
            // 
            this.gridControlSecondaryDisease.Location = new System.Drawing.Point(5, 29);
            this.gridControlSecondaryDisease.MainView = this.gridViewSecondaryDisease;
            this.gridControlSecondaryDisease.Name = "gridControlSecondaryDisease";
            this.gridControlSecondaryDisease.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemCheckEdit1});
            this.gridControlSecondaryDisease.Size = new System.Drawing.Size(904, 401);
            this.gridControlSecondaryDisease.TabIndex = 4;
            this.gridControlSecondaryDisease.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewSecondaryDisease});
            this.gridControlSecondaryDisease.Load += new System.EventHandler(this.frmSecondaryDisease_Load);
            this.gridControlSecondaryDisease.DoubleClick += new System.EventHandler(this.gridControlSecondaryDisease_DoubleClick);
            this.gridControlSecondaryDisease.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.gridControlSecondaryDisease_PreviewKeyDown);
            // 
            // gridViewSecondaryDisease
            // 
            this.gridViewSecondaryDisease.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn1,
            this.grdColCode,
            this.grdColName});
            this.gridViewSecondaryDisease.GridControl = this.gridControlSecondaryDisease;
            this.gridViewSecondaryDisease.Name = "gridViewSecondaryDisease";
            this.gridViewSecondaryDisease.OptionsFind.AllowFindPanel = false;
            this.gridViewSecondaryDisease.OptionsFind.FindDelay = 100;
            this.gridViewSecondaryDisease.OptionsFind.FindMode = DevExpress.XtraEditors.FindMode.Always;
            this.gridViewSecondaryDisease.OptionsFind.FindNullPrompt = "Từ khóa tìm kiếm...";
            this.gridViewSecondaryDisease.OptionsFind.ShowClearButton = false;
            this.gridViewSecondaryDisease.OptionsFind.ShowCloseButton = false;
            this.gridViewSecondaryDisease.OptionsFind.ShowFindButton = false;
            this.gridViewSecondaryDisease.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridViewSecondaryDisease.OptionsView.ShowGroupPanel = false;
            this.gridViewSecondaryDisease.OptionsView.ShowIndicator = false;
            this.gridViewSecondaryDisease.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gridViewSecondaryDisease_CellValueChanged);
            this.gridViewSecondaryDisease.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gridViewSecondaryDisease_MouseDown);
            // 
            // gridColumn1
            // 
            this.gridColumn1.Caption = "gridColumn1";
            this.gridColumn1.ColumnEdit = this.repositoryItemCheckEdit1;
            this.gridColumn1.FieldName = "IsChecked";
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.Visible = true;
            this.gridColumn1.VisibleIndex = 0;
            this.gridColumn1.Width = 33;
            // 
            // repositoryItemCheckEdit1
            // 
            this.repositoryItemCheckEdit1.AutoHeight = false;
            this.repositoryItemCheckEdit1.Name = "repositoryItemCheckEdit1";
            // 
            // grdColCode
            // 
            this.grdColCode.Caption = "Tên đăng nhập";
            this.grdColCode.FieldName = "LOGINNAME";
            this.grdColCode.Name = "grdColCode";
            this.grdColCode.OptionsColumn.AllowEdit = false;
            this.grdColCode.OptionsColumn.ReadOnly = true;
            this.grdColCode.Visible = true;
            this.grdColCode.VisibleIndex = 1;
            this.grdColCode.Width = 211;
            // 
            // grdColName
            // 
            this.grdColName.Caption = "Họ tên";
            this.grdColName.FieldName = "USERNAME";
            this.grdColName.Name = "grdColName";
            this.grdColName.OptionsColumn.AllowEdit = false;
            this.grdColName.OptionsColumn.ReadOnly = true;
            this.grdColName.Visible = true;
            this.grdColName.VisibleIndex = 2;
            this.grdColName.Width = 936;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciGridControl,
            this.layoutControlItem1,
            this.lblIcdText,
            this.layoutControlItem2,
            this.emptySpaceItem1,
            this.lciPaging});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "Root";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(5, 5, 5, 5);
            this.layoutControlGroup1.Size = new System.Drawing.Size(914, 497);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // lciGridControl
            // 
            this.lciGridControl.Control = this.gridControlSecondaryDisease;
            this.lciGridControl.Location = new System.Drawing.Point(0, 24);
            this.lciGridControl.Name = "lciGridControl";
            this.lciGridControl.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.lciGridControl.Size = new System.Drawing.Size(904, 401);
            this.lciGridControl.TextSize = new System.Drawing.Size(0, 0);
            this.lciGridControl.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.btnChoose;
            this.layoutControlItem1.Location = new System.Drawing.Point(814, 461);
            this.layoutControlItem1.MaxSize = new System.Drawing.Size(90, 26);
            this.layoutControlItem1.MinSize = new System.Drawing.Size(90, 26);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(90, 26);
            this.layoutControlItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem1.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextToControlDistance = 0;
            this.layoutControlItem1.TextVisible = false;
            // 
            // lblIcdText
            // 
            this.lblIcdText.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lblIcdText.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lblIcdText.Control = this.txtLoginNames;
            this.lblIcdText.Location = new System.Drawing.Point(0, 451);
            this.lblIcdText.Name = "lblIcdText";
            this.lblIcdText.Size = new System.Drawing.Size(814, 36);
            this.lblIcdText.Text = "Bệnh phụ:";
            this.lblIcdText.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lblIcdText.TextSize = new System.Drawing.Size(0, 0);
            this.lblIcdText.TextToControlDistance = 0;
            this.lblIcdText.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.txtKeyword;
            this.layoutControlItem2.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(904, 24);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(814, 451);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(90, 10);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // lciPaging
            // 
            this.lciPaging.Control = this.ucPaging1;
            this.lciPaging.Location = new System.Drawing.Point(0, 425);
            this.lciPaging.MaxSize = new System.Drawing.Size(0, 26);
            this.lciPaging.MinSize = new System.Drawing.Size(104, 26);
            this.lciPaging.Name = "lciPaging";
            this.lciPaging.Size = new System.Drawing.Size(904, 26);
            this.lciPaging.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciPaging.TextSize = new System.Drawing.Size(0, 0);
            this.lciPaging.TextVisible = false;
            // 
            // barManager1
            // 
            this.barManager1.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
            this.bar2});
            this.barManager1.DockControls.Add(this.barDockControlTop);
            this.barManager1.DockControls.Add(this.barDockControlBottom);
            this.barManager1.DockControls.Add(this.barDockControlLeft);
            this.barManager1.DockControls.Add(this.barDockControlRight);
            this.barManager1.Form = this;
            this.barManager1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.bbtnChoose,
            this.bbtnClose});
            this.barManager1.MainMenu = this.bar2;
            this.barManager1.MaxItemId = 2;
            // 
            // bar2
            // 
            this.bar2.BarName = "Main menu";
            this.bar2.DockCol = 0;
            this.bar2.DockRow = 0;
            this.bar2.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar2.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.bbtnChoose),
            new DevExpress.XtraBars.LinkPersistInfo(this.bbtnClose)});
            this.bar2.OptionsBar.MultiLine = true;
            this.bar2.OptionsBar.UseWholeRow = true;
            this.bar2.Text = "Main menu";
            this.bar2.Visible = false;
            // 
            // bbtnChoose
            // 
            this.bbtnChoose.Caption = "Chọn (Ctrl S)";
            this.bbtnChoose.Id = 0;
            this.bbtnChoose.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S));
            this.bbtnChoose.Name = "bbtnChoose";
            this.bbtnChoose.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bbtnChoose_ItemClick);
            // 
            // bbtnClose
            // 
            this.bbtnClose.Caption = "Đóng (Esc)";
            this.bbtnClose.Id = 1;
            this.bbtnClose.Name = "bbtnClose";
            this.bbtnClose.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bbtnChoose_ItemClick);
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Size = new System.Drawing.Size(914, 22);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 519);
            this.barDockControlBottom.Size = new System.Drawing.Size(914, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 22);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 497);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(914, 22);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 497);
            // 
            // frmAcsUser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(914, 519);
            this.Controls.Add(this.layoutControl1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmAcsUser";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Tìm chọn tài khoản";
            this.Load += new System.EventHandler(this.frmSecondaryDisease_Load);
            this.Controls.SetChildIndex(this.barDockControlTop, 0);
            this.Controls.SetChildIndex(this.barDockControlBottom, 0);
            this.Controls.SetChildIndex(this.barDockControlRight, 0);
            this.Controls.SetChildIndex(this.barDockControlLeft, 0);
            this.Controls.SetChildIndex(this.layoutControl1, 0);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtKeyword.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtLoginNames.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlSecondaryDisease)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewSecondaryDisease)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciGridControl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblIcdText)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPaging)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.SimpleButton btnChoose;
        private DevExpress.XtraGrid.GridControl gridControlSecondaryDisease;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewSecondaryDisease;
        private DevExpress.XtraGrid.Columns.GridColumn grdColCode;
        private DevExpress.XtraGrid.Columns.GridColumn grdColName;
        private DevExpress.XtraLayout.LayoutControlItem lciGridControl;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraEditors.TextEdit txtLoginNames;
        private DevExpress.XtraLayout.LayoutControlItem lblIcdText;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheckEdit1;
        private DevExpress.XtraEditors.TextEdit txtKeyword;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;


        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private Inventec.UC.Paging.UcPaging ucPaging1;
        private DevExpress.XtraLayout.LayoutControlItem lciPaging;
        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.Bar bar2;
        private DevExpress.XtraBars.BarButtonItem bbtnChoose;
        private DevExpress.XtraBars.BarButtonItem bbtnClose;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
    }
}
