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
namespace HIS.Desktop.Plugins.TestDeviceSample
{
    partial class frmTestDeviceSample
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
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.lblHint = new DevExpress.XtraEditors.LabelControl();
            this.btnRefesh = new DevExpress.XtraEditors.SimpleButton();
            this.gridControlTestDeviceSample = new DevExpress.XtraGrid.GridControl();
            this.gridViewTestDeviceSample = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gcStt = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcPortName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcLoginname = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcUsername = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcSessionTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcLogout = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemButtonLogout = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciHint = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.barButtonItem1 = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.timerRefresh = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlTestDeviceSample)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewTestDeviceSample)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemButtonLogout)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciHint)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            this.SuspendLayout();
            //
            // layoutControl1
            //
            this.layoutControl1.Controls.Add(this.lblHint);
            this.layoutControl1.Controls.Add(this.btnRefesh);
            this.layoutControl1.Controls.Add(this.gridControlTestDeviceSample);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 29);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(720, 410);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            //
            // lblHint
            //
            this.lblHint.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(84)))), ((int)(((byte)(147)))));
            this.lblHint.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.lblHint.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
            this.lblHint.Location = new System.Drawing.Point(12, 12);
            this.lblHint.Name = "lblHint";
            this.lblHint.Size = new System.Drawing.Size(696, 26);
            this.lblHint.StyleController = this.layoutControl1;
            this.lblHint.TabIndex = 6;
            //
            // btnRefesh
            //
            this.btnRefesh.Location = new System.Drawing.Point(598, 376);
            this.btnRefesh.Name = "btnRefesh";
            this.btnRefesh.Size = new System.Drawing.Size(110, 22);
            this.btnRefesh.StyleController = this.layoutControl1;
            this.btnRefesh.TabIndex = 5;
            this.btnRefesh.Text = "Tải lại (Ctrl R)";
            this.btnRefesh.Click += new System.EventHandler(this.btnRefesh_Click);
            //
            // gridControlTestDeviceSample
            //
            this.gridControlTestDeviceSample.Location = new System.Drawing.Point(12, 42);
            this.gridControlTestDeviceSample.MainView = this.gridViewTestDeviceSample;
            this.gridControlTestDeviceSample.Name = "gridControlTestDeviceSample";
            this.gridControlTestDeviceSample.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemButtonLogout});
            this.gridControlTestDeviceSample.Size = new System.Drawing.Size(696, 330);
            this.gridControlTestDeviceSample.TabIndex = 4;
            this.gridControlTestDeviceSample.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewTestDeviceSample});
            //
            // gridViewTestDeviceSample
            //
            this.gridViewTestDeviceSample.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gcStt,
            this.gcPortName,
            this.gcLoginname,
            this.gcUsername,
            this.gcSessionTime,
            this.gcLogout});
            this.gridViewTestDeviceSample.GridControl = this.gridControlTestDeviceSample;
            this.gridViewTestDeviceSample.Name = "gridViewTestDeviceSample";
            this.gridViewTestDeviceSample.OptionsCustomization.AllowSort = false;
            this.gridViewTestDeviceSample.OptionsView.ShowGroupPanel = false;
            this.gridViewTestDeviceSample.OptionsView.ShowIndicator = false;
            this.gridViewTestDeviceSample.RowHeight = 28;
            this.gridViewTestDeviceSample.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewTestDeviceSample_CustomUnboundColumnData);
            //
            // gcStt
            //
            this.gcStt.Caption = "STT";
            this.gcStt.FieldName = "STT";
            this.gcStt.Name = "gcStt";
            this.gcStt.OptionsColumn.AllowEdit = false;
            this.gcStt.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gcStt.Visible = true;
            this.gcStt.VisibleIndex = 0;
            this.gcStt.Width = 40;
            //
            // gcPortName
            //
            this.gcPortName.Caption = "Cổng";
            this.gcPortName.FieldName = "TeminalCode";
            this.gcPortName.Name = "gcPortName";
            this.gcPortName.OptionsColumn.AllowEdit = false;
            this.gcPortName.Visible = true;
            this.gcPortName.VisibleIndex = 1;
            this.gcPortName.Width = 80;
            //
            // gcLoginname
            //
            this.gcLoginname.Caption = "Tài khoản";
            this.gcLoginname.FieldName = "Loginname";
            this.gcLoginname.Name = "gcLoginname";
            this.gcLoginname.OptionsColumn.AllowEdit = false;
            this.gcLoginname.Visible = true;
            this.gcLoginname.VisibleIndex = 2;
            this.gcLoginname.Width = 120;
            //
            // gcUsername
            //
            this.gcUsername.Caption = "Họ tên";
            this.gcUsername.FieldName = "Username";
            this.gcUsername.Name = "gcUsername";
            this.gcUsername.OptionsColumn.AllowEdit = false;
            this.gcUsername.Visible = true;
            this.gcUsername.VisibleIndex = 3;
            this.gcUsername.Width = 210;
            //
            // gcSessionTime
            //
            this.gcSessionTime.Caption = "Đăng nhập lúc";
            this.gcSessionTime.FieldName = "SessionTimeStartDisplay";
            this.gcSessionTime.Name = "gcSessionTime";
            this.gcSessionTime.OptionsColumn.AllowEdit = false;
            this.gcSessionTime.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gcSessionTime.Visible = true;
            this.gcSessionTime.VisibleIndex = 4;
            this.gcSessionTime.Width = 130;
            //
            // gcLogout
            //
            this.gcLogout.Caption = " ";
            this.gcLogout.ColumnEdit = this.repositoryItemButtonLogout;
            this.gcLogout.FieldName = "LOGOUT";
            this.gcLogout.Name = "gcLogout";
            this.gcLogout.OptionsColumn.AllowSize = false;
            this.gcLogout.OptionsColumn.FixedWidth = true;
            this.gcLogout.OptionsColumn.ShowCaption = false;
            this.gcLogout.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            this.gcLogout.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gcLogout.Visible = true;
            this.gcLogout.VisibleIndex = 5;
            this.gcLogout.Width = 100;
            //
            // repositoryItemButtonLogout
            //
            this.repositoryItemButtonLogout.AutoHeight = false;
            this.repositoryItemButtonLogout.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph)});
            this.repositoryItemButtonLogout.Buttons[0].Caption = "Đăng xuất";
            this.repositoryItemButtonLogout.Name = "repositoryItemButtonLogout";
            this.repositoryItemButtonLogout.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            this.repositoryItemButtonLogout.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repositoryItemButtonLogout_ButtonClick);
            //
            // layoutControlGroup1
            //
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciHint,
            this.layoutControlItem1,
            this.emptySpaceItem1,
            this.layoutControlItem2});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "Root";
            this.layoutControlGroup1.Size = new System.Drawing.Size(720, 410);
            this.layoutControlGroup1.TextVisible = false;
            //
            // lciHint
            //
            this.lciHint.Control = this.lblHint;
            this.lciHint.Location = new System.Drawing.Point(0, 0);
            this.lciHint.MaxSize = new System.Drawing.Size(0, 30);
            this.lciHint.MinSize = new System.Drawing.Size(100, 30);
            this.lciHint.Name = "lciHint";
            this.lciHint.Size = new System.Drawing.Size(700, 30);
            this.lciHint.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciHint.TextSize = new System.Drawing.Size(0, 0);
            this.lciHint.TextVisible = false;
            //
            // layoutControlItem1
            //
            this.layoutControlItem1.Control = this.gridControlTestDeviceSample;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 30);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(700, 334);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            //
            // emptySpaceItem1
            //
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(0, 364);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(586, 26);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            //
            // layoutControlItem2
            //
            this.layoutControlItem2.Control = this.btnRefesh;
            this.layoutControlItem2.Location = new System.Drawing.Point(586, 364);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(114, 26);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
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
            this.barButtonItem1});
            this.barManager1.MaxItemId = 1;
            //
            // bar1
            //
            this.bar1.BarName = "Tools";
            this.bar1.DockCol = 0;
            this.bar1.DockRow = 0;
            this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.barButtonItem1)});
            this.bar1.Text = "Tools";
            this.bar1.Visible = false;
            //
            // barButtonItem1
            //
            this.barButtonItem1.Caption = "Ctrl R";
            this.barButtonItem1.Id = 0;
            this.barButtonItem1.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R));
            this.barButtonItem1.Name = "barButtonItem1";
            this.barButtonItem1.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem1_ItemClick);
            //
            // barDockControlTop
            //
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Size = new System.Drawing.Size(720, 29);
            //
            // barDockControlBottom
            //
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 439);
            this.barDockControlBottom.Size = new System.Drawing.Size(720, 0);
            //
            // barDockControlLeft
            //
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 29);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 410);
            //
            // barDockControlRight
            //
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(720, 29);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 410);
            //
            // timerRefresh
            //
            this.timerRefresh.Interval = 3000;
            this.timerRefresh.Tick += new System.EventHandler(this.timerRefresh_Tick);
            //
            // frmTestDeviceSample
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(720, 439);
            this.Controls.Add(this.layoutControl1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Name = "frmTestDeviceSample";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Thiết bị lấy thông tin mẫu bệnh phẩm";
            this.Load += new System.EventHandler(this.frmTestDeviceSample_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControlTestDeviceSample)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewTestDeviceSample)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemButtonLogout)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciHint)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraEditors.LabelControl lblHint;
        private DevExpress.XtraGrid.GridControl gridControlTestDeviceSample;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewTestDeviceSample;
        private DevExpress.XtraGrid.Columns.GridColumn gcStt;
        private DevExpress.XtraGrid.Columns.GridColumn gcPortName;
        private DevExpress.XtraGrid.Columns.GridColumn gcLoginname;
        private DevExpress.XtraGrid.Columns.GridColumn gcUsername;
        private DevExpress.XtraGrid.Columns.GridColumn gcSessionTime;
        private DevExpress.XtraGrid.Columns.GridColumn gcLogout;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit repositoryItemButtonLogout;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraLayout.LayoutControlItem lciHint;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.Bar bar1;
        private DevExpress.XtraBars.BarButtonItem barButtonItem1;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraEditors.SimpleButton btnRefesh;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private System.Windows.Forms.Timer timerRefresh;
    }
}
