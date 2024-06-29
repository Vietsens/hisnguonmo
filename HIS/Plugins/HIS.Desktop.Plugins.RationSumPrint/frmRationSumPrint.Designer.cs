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
namespace HIS.Desktop.Plugins.RationSumPrint
{
    partial class frmRationSumPrint
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
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.btnInTem = new DevExpress.XtraEditors.SimpleButton();
            this.btnInPhieuChiTiet = new DevExpress.XtraEditors.SimpleButton();
            this.gridControlRationGroup = new DevExpress.XtraGrid.GridControl();
            this.gridViewRationGroup = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridRationGroup_RationGroupCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridRationGroup_RationGroupName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridControlDepartment = new DevExpress.XtraGrid.GridControl();
            this.gridViewDepartment = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridDeparment_DeparmentCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridDeparment_DeparmentName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.btnInPhieuTongHop = new DevExpress.XtraEditors.SimpleButton();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            this.barManager1 = new DevExpress.XtraBars.BarManager();
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.bbtnRCPrint = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.btnPrintGroup = new DevExpress.XtraEditors.SimpleButton();
            this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlRationGroup)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewRationGroup)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlDepartment)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewDepartment)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.btnPrintGroup);
            this.layoutControl1.Controls.Add(this.btnInTem);
            this.layoutControl1.Controls.Add(this.btnInPhieuChiTiet);
            this.layoutControl1.Controls.Add(this.gridControlRationGroup);
            this.layoutControl1.Controls.Add(this.gridControlDepartment);
            this.layoutControl1.Controls.Add(this.btnInPhieuTongHop);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 29);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(424, 263);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // btnInTem
            // 
            this.btnInTem.Location = new System.Drawing.Point(72, 239);
            this.btnInTem.Name = "btnInTem";
            this.btnInTem.Size = new System.Drawing.Size(85, 22);
            this.btnInTem.StyleController = this.layoutControl1;
            this.btnInTem.TabIndex = 9;
            this.btnInTem.Text = "In tem";
            this.btnInTem.Click += new System.EventHandler(this.btnInTem_Click);
            // 
            // btnInPhieuChiTiet
            // 
            this.btnInPhieuChiTiet.Location = new System.Drawing.Point(161, 239);
            this.btnInPhieuChiTiet.Name = "btnInPhieuChiTiet";
            this.btnInPhieuChiTiet.Size = new System.Drawing.Size(83, 22);
            this.btnInPhieuChiTiet.StyleController = this.layoutControl1;
            this.btnInPhieuChiTiet.TabIndex = 8;
            this.btnInPhieuChiTiet.Text = "In phiếu chi tiết";
            this.btnInPhieuChiTiet.Click += new System.EventHandler(this.btnInPhieuChiTiet_Click);
            // 
            // gridControlRationGroup
            // 
            this.gridControlRationGroup.Location = new System.Drawing.Point(2, 115);
            this.gridControlRationGroup.MainView = this.gridViewRationGroup;
            this.gridControlRationGroup.Name = "gridControlRationGroup";
            this.gridControlRationGroup.Size = new System.Drawing.Size(420, 120);
            this.gridControlRationGroup.TabIndex = 0;
            this.gridControlRationGroup.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewRationGroup});
            // 
            // gridViewRationGroup
            // 
            this.gridViewRationGroup.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridRationGroup_RationGroupCode,
            this.gridRationGroup_RationGroupName});
            this.gridViewRationGroup.GridControl = this.gridControlRationGroup;
            this.gridViewRationGroup.Name = "gridViewRationGroup";
            this.gridViewRationGroup.OptionsSelection.CheckBoxSelectorColumnWidth = 30;
            this.gridViewRationGroup.OptionsSelection.MultiSelect = true;
            this.gridViewRationGroup.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;
            this.gridViewRationGroup.OptionsView.ShowGroupPanel = false;
            this.gridViewRationGroup.OptionsView.ShowIndicator = false;
            this.gridViewRationGroup.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewRationGroup_CustomUnboundColumnData);
            // 
            // gridRationGroup_RationGroupCode
            // 
            this.gridRationGroup_RationGroupCode.Caption = "Mã nhóm";
            this.gridRationGroup_RationGroupCode.FieldName = "RATION_GROUP_CODE";
            this.gridRationGroup_RationGroupCode.Name = "gridRationGroup_RationGroupCode";
            this.gridRationGroup_RationGroupCode.OptionsColumn.AllowEdit = false;
            this.gridRationGroup_RationGroupCode.Visible = true;
            this.gridRationGroup_RationGroupCode.VisibleIndex = 1;
            this.gridRationGroup_RationGroupCode.Width = 80;
            // 
            // gridRationGroup_RationGroupName
            // 
            this.gridRationGroup_RationGroupName.Caption = "Tên nhóm";
            this.gridRationGroup_RationGroupName.FieldName = "RATION_GROUP_NAME";
            this.gridRationGroup_RationGroupName.Name = "gridRationGroup_RationGroupName";
            this.gridRationGroup_RationGroupName.OptionsColumn.AllowEdit = false;
            this.gridRationGroup_RationGroupName.Visible = true;
            this.gridRationGroup_RationGroupName.VisibleIndex = 2;
            this.gridRationGroup_RationGroupName.Width = 200;
            // 
            // gridControlDepartment
            // 
            this.gridControlDepartment.Location = new System.Drawing.Point(2, 2);
            this.gridControlDepartment.MainView = this.gridViewDepartment;
            this.gridControlDepartment.Name = "gridControlDepartment";
            this.gridControlDepartment.Size = new System.Drawing.Size(420, 109);
            this.gridControlDepartment.TabIndex = 0;
            this.gridControlDepartment.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewDepartment});
            // 
            // gridViewDepartment
            // 
            this.gridViewDepartment.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridDeparment_DeparmentCode,
            this.gridDeparment_DeparmentName});
            this.gridViewDepartment.GridControl = this.gridControlDepartment;
            this.gridViewDepartment.Name = "gridViewDepartment";
            this.gridViewDepartment.OptionsSelection.CheckBoxSelectorColumnWidth = 30;
            this.gridViewDepartment.OptionsSelection.MultiSelect = true;
            this.gridViewDepartment.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;
            this.gridViewDepartment.OptionsView.ShowGroupPanel = false;
            this.gridViewDepartment.OptionsView.ShowIndicator = false;
            this.gridViewDepartment.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridDepartment_CustomUnboundColumnData);
            // 
            // gridDeparment_DeparmentCode
            // 
            this.gridDeparment_DeparmentCode.Caption = "Mã khoa";
            this.gridDeparment_DeparmentCode.FieldName = "DEPARTMENT_CODE";
            this.gridDeparment_DeparmentCode.Name = "gridDeparment_DeparmentCode";
            this.gridDeparment_DeparmentCode.OptionsColumn.AllowEdit = false;
            this.gridDeparment_DeparmentCode.Visible = true;
            this.gridDeparment_DeparmentCode.VisibleIndex = 1;
            this.gridDeparment_DeparmentCode.Width = 80;
            // 
            // gridDeparment_DeparmentName
            // 
            this.gridDeparment_DeparmentName.Caption = "Tên khoa";
            this.gridDeparment_DeparmentName.FieldName = "DEPARTMENT_NAME";
            this.gridDeparment_DeparmentName.Name = "gridDeparment_DeparmentName";
            this.gridDeparment_DeparmentName.OptionsColumn.AllowEdit = false;
            this.gridDeparment_DeparmentName.Visible = true;
            this.gridDeparment_DeparmentName.VisibleIndex = 2;
            this.gridDeparment_DeparmentName.Width = 200;
            // 
            // btnInPhieuTongHop
            // 
            this.btnInPhieuTongHop.Location = new System.Drawing.Point(248, 239);
            this.btnInPhieuTongHop.Name = "btnInPhieuTongHop";
            this.btnInPhieuTongHop.Size = new System.Drawing.Size(94, 22);
            this.btnInPhieuTongHop.StyleController = this.layoutControl1;
            this.btnInPhieuTongHop.TabIndex = 7;
            this.btnInPhieuTongHop.Text = "In phiếu tổng hợp";
            this.btnInPhieuTongHop.Click += new System.EventHandler(this.btnInTongHop_Click);
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.emptySpaceItem1,
            this.layoutControlItem2,
            this.layoutControlItem3,
            this.layoutControlItem4,
            this.layoutControlItem1,
            this.layoutControlItem5,
            this.layoutControlItem6});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(424, 263);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(0, 237);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(70, 26);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.btnInPhieuTongHop;
            this.layoutControlItem2.Location = new System.Drawing.Point(246, 237);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(98, 26);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.gridControlDepartment;
            this.layoutControlItem3.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(424, 113);
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextVisible = false;
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.Control = this.gridControlRationGroup;
            this.layoutControlItem4.Location = new System.Drawing.Point(0, 113);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Size = new System.Drawing.Size(424, 124);
            this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem4.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.btnInPhieuChiTiet;
            this.layoutControlItem1.Location = new System.Drawing.Point(159, 237);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(87, 26);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // layoutControlItem5
            // 
            this.layoutControlItem5.Control = this.btnInTem;
            this.layoutControlItem5.Location = new System.Drawing.Point(70, 237);
            this.layoutControlItem5.Name = "layoutControlItem5";
            this.layoutControlItem5.Size = new System.Drawing.Size(89, 26);
            this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem5.TextVisible = false;
            // 
            // barManager1
            // 
            this.barManager1.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
            this.bar1});
            this.barManager1.Form = this;
            this.barManager1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.bbtnRCPrint});
            this.barManager1.MaxItemId = 1;
            // 
            // bar1
            // 
            this.bar1.BarName = "Tools";
            this.bar1.DockCol = 0;
            this.bar1.DockRow = 0;
            this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.bbtnRCPrint)});
            this.bar1.Text = "Tools";
            this.bar1.Visible = false;
            // 
            // bbtnRCPrint
            // 
            this.bbtnRCPrint.Caption = "In (Ctrl P)";
            this.bbtnRCPrint.Id = 0;
            this.bbtnRCPrint.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P));
            this.bbtnRCPrint.Name = "bbtnRCPrint";
            this.bbtnRCPrint.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bbtnRCPrint_ItemClick);
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 29);
            this.barDockControlTop.Size = new System.Drawing.Size(424, 0);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 292);
            this.barDockControlBottom.Size = new System.Drawing.Size(424, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 29);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 263);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(424, 29);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 263);
            // 
            // btnPrintGroup
            // 
            this.btnPrintGroup.Location = new System.Drawing.Point(346, 239);
            this.btnPrintGroup.Name = "btnPrintGroup";
            this.btnPrintGroup.Size = new System.Drawing.Size(76, 22);
            this.btnPrintGroup.StyleController = this.layoutControl1;
            this.btnPrintGroup.TabIndex = 10;
            this.btnPrintGroup.Text = "In phiếu gộp";
            this.btnPrintGroup.Click += new System.EventHandler(this.btnPrintGroup_Click);
            // 
            // layoutControlItem6
            // 
            this.layoutControlItem6.Control = this.btnPrintGroup;
            this.layoutControlItem6.Location = new System.Drawing.Point(344, 237);
            this.layoutControlItem6.Name = "layoutControlItem6";
            this.layoutControlItem6.Size = new System.Drawing.Size(80, 26);
            this.layoutControlItem6.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem6.TextVisible = false;
            // 
            // frmRationSumPrint
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(424, 292);
            this.Controls.Add(this.layoutControl1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Name = "frmRationSumPrint";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "In phiếu tổng hợp suất ăn";
            this.Load += new System.EventHandler(this.frmRationSumPrint_Load);
            this.Controls.SetChildIndex(this.barDockControlTop, 0);
            this.Controls.SetChildIndex(this.barDockControlBottom, 0);
            this.Controls.SetChildIndex(this.barDockControlRight, 0);
            this.Controls.SetChildIndex(this.barDockControlLeft, 0);
            this.Controls.SetChildIndex(this.layoutControl1, 0);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControlRationGroup)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewRationGroup)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlDepartment)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewDepartment)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraGrid.GridControl gridControlDepartment;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewDepartment;
        private DevExpress.XtraGrid.GridControl gridControlRationGroup;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewRationGroup;
        private DevExpress.XtraGrid.Columns.GridColumn gridDeparment_DeparmentCode;
        private DevExpress.XtraGrid.Columns.GridColumn gridDeparment_DeparmentName;
        private DevExpress.XtraGrid.Columns.GridColumn gridRationGroup_RationGroupCode;
        private DevExpress.XtraGrid.Columns.GridColumn gridRationGroup_RationGroupName;
        private DevExpress.XtraEditors.SimpleButton btnInPhieuTongHop;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.Bar bar1;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraBars.BarButtonItem bbtnRCPrint;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraEditors.SimpleButton btnInTem;
        private DevExpress.XtraEditors.SimpleButton btnInPhieuChiTiet;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
        private DevExpress.XtraEditors.SimpleButton btnPrintGroup;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem6;
    }
}
