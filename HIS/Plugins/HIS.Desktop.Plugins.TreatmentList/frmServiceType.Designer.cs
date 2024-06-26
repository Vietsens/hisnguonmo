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
namespace HIS.Desktop.Plugins.TreatmentList
{
    partial class frmServiceType
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
			this.btnPrintMergeXN = new DevExpress.XtraEditors.SimpleButton();
			this.btnPrint = new DevExpress.XtraEditors.SimpleButton();
			this.gridControl1 = new DevExpress.XtraGrid.GridControl();
			this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
			this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
			this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
			this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
			this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
			this.lciBtnPrintMergeXN = new DevExpress.XtraLayout.LayoutControlItem();
			this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
			this.bar1 = new DevExpress.XtraBars.Bar();
			this.barButtonItem1 = new DevExpress.XtraBars.BarButtonItem();
			this.barDockControl1 = new DevExpress.XtraBars.BarDockControl();
			this.barDockControl2 = new DevExpress.XtraBars.BarDockControl();
			this.barDockControl3 = new DevExpress.XtraBars.BarDockControl();
			this.barDockControl4 = new DevExpress.XtraBars.BarDockControl();
			this.btnSplitServiceReq = new DevExpress.XtraEditors.SimpleButton();
			this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
			this.layoutControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciBtnPrintMergeXN)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
			this.SuspendLayout();
			// 
			// layoutControl1
			// 
			this.layoutControl1.Controls.Add(this.btnSplitServiceReq);
			this.layoutControl1.Controls.Add(this.btnPrintMergeXN);
			this.layoutControl1.Controls.Add(this.btnPrint);
			this.layoutControl1.Controls.Add(this.gridControl1);
			this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.layoutControl1.Location = new System.Drawing.Point(0, 29);
			this.layoutControl1.Name = "layoutControl1";
			this.layoutControl1.Root = this.layoutControlGroup1;
			this.layoutControl1.Size = new System.Drawing.Size(401, 329);
			this.layoutControl1.TabIndex = 0;
			this.layoutControl1.Text = "layoutControl1";
			// 
			// btnPrintMergeXN
			// 
			this.btnPrintMergeXN.Location = new System.Drawing.Point(268, 305);
			this.btnPrintMergeXN.Name = "btnPrintMergeXN";
			this.btnPrintMergeXN.Size = new System.Drawing.Size(68, 22);
			this.btnPrintMergeXN.StyleController = this.layoutControl1;
			this.btnPrintMergeXN.TabIndex = 6;
			this.btnPrintMergeXN.Text = "In gộp";
			this.btnPrintMergeXN.Click += new System.EventHandler(this.btnPrintMergeXN_Click);
			// 
			// btnPrint
			// 
			this.btnPrint.Location = new System.Drawing.Point(340, 305);
			this.btnPrint.Name = "btnPrint";
			this.btnPrint.Size = new System.Drawing.Size(59, 22);
			this.btnPrint.StyleController = this.layoutControl1;
			this.btnPrint.TabIndex = 5;
			this.btnPrint.Text = "In (Ctrl P)";
			this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
			// 
			// gridControl1
			// 
			this.gridControl1.Location = new System.Drawing.Point(2, 2);
			this.gridControl1.MainView = this.gridView1;
			this.gridControl1.Name = "gridControl1";
			this.gridControl1.Size = new System.Drawing.Size(397, 299);
			this.gridControl1.TabIndex = 4;
			this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
			// 
			// gridView1
			// 
			this.gridView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn1,
            this.gridColumn2});
			this.gridView1.GridControl = this.gridControl1;
			this.gridView1.Name = "gridView1";
			this.gridView1.OptionsSelection.CheckBoxSelectorColumnWidth = 30;
			this.gridView1.OptionsSelection.MultiSelect = true;
			this.gridView1.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;
			this.gridView1.OptionsView.ShowGroupPanel = false;
			this.gridView1.OptionsView.ShowIndicator = false;
			this.gridView1.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridView1_CustomUnboundColumnData);
			// 
			// gridColumn1
			// 
			this.gridColumn1.Caption = "STT";
			this.gridColumn1.FieldName = "STT";
			this.gridColumn1.Name = "gridColumn1";
			this.gridColumn1.OptionsColumn.AllowEdit = false;
			this.gridColumn1.UnboundType = DevExpress.Data.UnboundColumnType.Object;
			this.gridColumn1.Visible = true;
			this.gridColumn1.VisibleIndex = 1;
			this.gridColumn1.Width = 35;
			// 
			// gridColumn2
			// 
			this.gridColumn2.Caption = "Tên dịch vụ";
			this.gridColumn2.FieldName = "SERVICE_REQ_TYPE_NAME";
			this.gridColumn2.Name = "gridColumn2";
			this.gridColumn2.Visible = true;
			this.gridColumn2.VisibleIndex = 2;
			this.gridColumn2.Width = 284;
			// 
			// layoutControlGroup1
			// 
			this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.False;
			this.layoutControlGroup1.GroupBordersVisible = false;
			this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.layoutControlItem2,
            this.emptySpaceItem1,
            this.lciBtnPrintMergeXN,
            this.layoutControlItem3});
			this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
			this.layoutControlGroup1.Name = "layoutControlGroup1";
			this.layoutControlGroup1.Size = new System.Drawing.Size(401, 329);
			this.layoutControlGroup1.TextVisible = false;
			// 
			// layoutControlItem1
			// 
			this.layoutControlItem1.Control = this.gridControl1;
			this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
			this.layoutControlItem1.Name = "layoutControlItem1";
			this.layoutControlItem1.Size = new System.Drawing.Size(401, 303);
			this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem1.TextVisible = false;
			// 
			// layoutControlItem2
			// 
			this.layoutControlItem2.Control = this.btnPrint;
			this.layoutControlItem2.Location = new System.Drawing.Point(338, 303);
			this.layoutControlItem2.MaxSize = new System.Drawing.Size(102, 26);
			this.layoutControlItem2.MinSize = new System.Drawing.Size(60, 26);
			this.layoutControlItem2.Name = "layoutControlItem2";
			this.layoutControlItem2.Size = new System.Drawing.Size(63, 26);
			this.layoutControlItem2.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem2.TextVisible = false;
			// 
			// emptySpaceItem1
			// 
			this.emptySpaceItem1.AllowHotTrack = false;
			this.emptySpaceItem1.Location = new System.Drawing.Point(0, 303);
			this.emptySpaceItem1.Name = "emptySpaceItem1";
			this.emptySpaceItem1.Size = new System.Drawing.Size(163, 26);
			this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
			// 
			// lciBtnPrintMergeXN
			// 
			this.lciBtnPrintMergeXN.Control = this.btnPrintMergeXN;
			this.lciBtnPrintMergeXN.Location = new System.Drawing.Point(266, 303);
			this.lciBtnPrintMergeXN.MaxSize = new System.Drawing.Size(104, 26);
			this.lciBtnPrintMergeXN.MinSize = new System.Drawing.Size(44, 26);
			this.lciBtnPrintMergeXN.Name = "lciBtnPrintMergeXN";
			this.lciBtnPrintMergeXN.Size = new System.Drawing.Size(72, 26);
			this.lciBtnPrintMergeXN.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.lciBtnPrintMergeXN.TextSize = new System.Drawing.Size(0, 0);
			this.lciBtnPrintMergeXN.TextVisible = false;
			// 
			// barManager1
			// 
			this.barManager1.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
            this.bar1});
			this.barManager1.DockControls.Add(this.barDockControl1);
			this.barManager1.DockControls.Add(this.barDockControl2);
			this.barManager1.DockControls.Add(this.barDockControl3);
			this.barManager1.DockControls.Add(this.barDockControl4);
			this.barManager1.Form = this;
			this.barManager1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.barButtonItem1});
			this.barManager1.MaxItemId = 2;
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
			this.barButtonItem1.Caption = "bbtnPrint";
			this.barButtonItem1.Id = 0;
			this.barButtonItem1.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P));
			this.barButtonItem1.Name = "barButtonItem1";
			this.barButtonItem1.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem1_ItemClick);
			// 
			// barDockControl1
			// 
			this.barDockControl1.CausesValidation = false;
			this.barDockControl1.Dock = System.Windows.Forms.DockStyle.Top;
			this.barDockControl1.Location = new System.Drawing.Point(0, 0);
			this.barDockControl1.Size = new System.Drawing.Size(401, 29);
			// 
			// barDockControl2
			// 
			this.barDockControl2.CausesValidation = false;
			this.barDockControl2.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.barDockControl2.Location = new System.Drawing.Point(0, 358);
			this.barDockControl2.Size = new System.Drawing.Size(401, 0);
			// 
			// barDockControl3
			// 
			this.barDockControl3.CausesValidation = false;
			this.barDockControl3.Dock = System.Windows.Forms.DockStyle.Left;
			this.barDockControl3.Location = new System.Drawing.Point(0, 29);
			this.barDockControl3.Size = new System.Drawing.Size(0, 329);
			// 
			// barDockControl4
			// 
			this.barDockControl4.CausesValidation = false;
			this.barDockControl4.Dock = System.Windows.Forms.DockStyle.Right;
			this.barDockControl4.Location = new System.Drawing.Point(401, 29);
			this.barDockControl4.Size = new System.Drawing.Size(0, 329);
			// 
			// btnSplitServiceReq
			// 
			this.btnSplitServiceReq.Location = new System.Drawing.Point(165, 305);
			this.btnSplitServiceReq.Name = "btnSplitServiceReq";
			this.btnSplitServiceReq.Size = new System.Drawing.Size(99, 22);
			this.btnSplitServiceReq.StyleController = this.layoutControl1;
			this.btnSplitServiceReq.TabIndex = 7;
			this.btnSplitServiceReq.Text = "In tách y lệnh";
			this.btnSplitServiceReq.Click += new System.EventHandler(this.btnSplitServiceReq_Click);
			// 
			// layoutControlItem3
			// 
			this.layoutControlItem3.Control = this.btnSplitServiceReq;
			this.layoutControlItem3.Location = new System.Drawing.Point(163, 303);
			this.layoutControlItem3.Name = "layoutControlItem3";
			this.layoutControlItem3.Size = new System.Drawing.Size(103, 26);
			this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem3.TextVisible = false;
			// 
			// frmServiceType
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(401, 358);
			this.Controls.Add(this.layoutControl1);
			this.Controls.Add(this.barDockControl3);
			this.Controls.Add(this.barDockControl4);
			this.Controls.Add(this.barDockControl2);
			this.Controls.Add(this.barDockControl1);
			this.Name = "frmServiceType";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Loại dịch vụ";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmServiceType_FormClosing);
			this.Load += new System.EventHandler(this.frmServiceType_Load);
			this.Controls.SetChildIndex(this.barDockControl1, 0);
			this.Controls.SetChildIndex(this.barDockControl2, 0);
			this.Controls.SetChildIndex(this.barDockControl4, 0);
			this.Controls.SetChildIndex(this.barDockControl3, 0);
			this.Controls.SetChildIndex(this.layoutControl1, 0);
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
			this.layoutControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciBtnPrintMergeXN)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraGrid.GridControl gridControl1;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraEditors.SimpleButton btnPrint;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.Bar bar1;
        private DevExpress.XtraBars.BarButtonItem barButtonItem1;
        private DevExpress.XtraBars.BarDockControl barDockControl1;
        private DevExpress.XtraBars.BarDockControl barDockControl2;
        private DevExpress.XtraBars.BarDockControl barDockControl3;
        private DevExpress.XtraBars.BarDockControl barDockControl4;
        private DevExpress.XtraEditors.SimpleButton btnPrintMergeXN;
        private DevExpress.XtraLayout.LayoutControlItem lciBtnPrintMergeXN;
        private DevExpress.XtraBars.BarButtonItem bbtnHideBtnPrintMergeXN;
		private DevExpress.XtraEditors.SimpleButton btnSplitServiceReq;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
	}
}
