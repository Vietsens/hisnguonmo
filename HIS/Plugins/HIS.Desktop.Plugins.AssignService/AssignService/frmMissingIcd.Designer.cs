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
namespace HIS.Desktop.Plugins.AssignService.AssignService
{
    partial class frmMissingIcd
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMissingIcd));
            this.gridControlMissingIcd = new DevExpress.XtraGrid.GridControl();
            this.gridViewMissingIcd = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumnServiceName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnIcdName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnIsMainIcd = new DevExpress.XtraGrid.Columns.GridColumn();
            this.CheckEditMainICD = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.gridColumnIsCauseIcd = new DevExpress.XtraGrid.Columns.GridColumn();
            this.CheckEditICDCause = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.btnSearch = new DevExpress.XtraEditors.SimpleButton();
            this.txtKeyword = new DevExpress.XtraEditors.TextEdit();
            this.barManager2 = new DevExpress.XtraBars.BarManager();
            this.barDockControl1 = new DevExpress.XtraBars.BarDockControl();
            this.barDockControl2 = new DevExpress.XtraBars.BarDockControl();
            this.barDockControl3 = new DevExpress.XtraBars.BarDockControl();
            this.barDockControl4 = new DevExpress.XtraBars.BarDockControl();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.btnAddIcd = new DevExpress.XtraEditors.SimpleButton();
            this.btnPrint = new DevExpress.XtraEditors.SimpleButton();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.barManager1 = new DevExpress.XtraBars.BarManager();
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.bbtnPrint = new DevExpress.XtraBars.BarButtonItem();
            this.bbtnSearch = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.btnSkip = new DevExpress.XtraEditors.SimpleButton();
            this.lciSkip = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlMissingIcd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewMissingIcd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CheckEditMainICD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CheckEditICDCause)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtKeyword.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciSkip)).BeginInit();
            this.SuspendLayout();
            // 
            // gridControlMissingIcd
            // 
            this.gridControlMissingIcd.Location = new System.Drawing.Point(2, 51);
            this.gridControlMissingIcd.MainView = this.gridViewMissingIcd;
            this.gridControlMissingIcd.Name = "gridControlMissingIcd";
            this.gridControlMissingIcd.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.CheckEditMainICD,
            this.CheckEditICDCause});
            this.gridControlMissingIcd.Size = new System.Drawing.Size(640, 218);
            this.gridControlMissingIcd.TabIndex = 29;
            this.gridControlMissingIcd.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewMissingIcd});
            // 
            // gridViewMissingIcd
            // 
            this.gridViewMissingIcd.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumnServiceName,
            this.gridColumnIcdName,
            this.gridColumnIsMainIcd,
            this.gridColumnIsCauseIcd});
            this.gridViewMissingIcd.GridControl = this.gridControlMissingIcd;
            this.gridViewMissingIcd.Name = "gridViewMissingIcd";
            this.gridViewMissingIcd.OptionsSelection.CheckBoxSelectorColumnWidth = 30;
            this.gridViewMissingIcd.OptionsView.ShowGroupPanel = false;
            this.gridViewMissingIcd.OptionsView.ShowIndicator = false;
            this.gridViewMissingIcd.CustomRowCellEdit += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gridViewMissingIcd_CustomRowCellEdit);
            this.gridViewMissingIcd.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gridViewMissingIcd_CellValueChanged);
            this.gridViewMissingIcd.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewMissingIcd_CustomUnboundColumnData);
            this.gridViewMissingIcd.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gridViewMissingIcd_MouseDown);
            // 
            // gridColumnServiceName
            // 
            this.gridColumnServiceName.Caption = "Dịch vụ";
            this.gridColumnServiceName.FieldName = "SERVICE_NAME";
            this.gridColumnServiceName.Name = "gridColumnServiceName";
            this.gridColumnServiceName.OptionsColumn.AllowEdit = false;
            this.gridColumnServiceName.Visible = true;
            this.gridColumnServiceName.VisibleIndex = 0;
            this.gridColumnServiceName.Width = 200;
            // 
            // gridColumnIcdName
            // 
            this.gridColumnIcdName.Caption = "Chẩn đoán (ICD)";
            this.gridColumnIcdName.FieldName = "ICD_NAME";
            this.gridColumnIcdName.Name = "gridColumnIcdName";
            this.gridColumnIcdName.OptionsColumn.AllowEdit = false;
            this.gridColumnIcdName.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumnIcdName.Visible = true;
            this.gridColumnIcdName.VisibleIndex = 1;
            this.gridColumnIcdName.Width = 302;
            // 
            // gridColumnIsMainIcd
            // 
            this.gridColumnIsMainIcd.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumnIsMainIcd.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumnIsMainIcd.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumnIsMainIcd.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumnIsMainIcd.Caption = "CĐ chính";
            this.gridColumnIsMainIcd.ColumnEdit = this.CheckEditMainICD;
            this.gridColumnIsMainIcd.FieldName = "ICD_MAIN_CHECK";
            this.gridColumnIsMainIcd.Name = "gridColumnIsMainIcd";
            this.gridColumnIsMainIcd.ToolTip = "Chẩn đoán chính";
            this.gridColumnIsMainIcd.Visible = true;
            this.gridColumnIsMainIcd.VisibleIndex = 2;
            this.gridColumnIsMainIcd.Width = 74;
            // 
            // CheckEditMainICD
            // 
            this.CheckEditMainICD.AutoHeight = false;
            this.CheckEditMainICD.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.CheckEditMainICD.Name = "CheckEditMainICD";
            // 
            // gridColumnIsCauseIcd
            // 
            this.gridColumnIsCauseIcd.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumnIsCauseIcd.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumnIsCauseIcd.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumnIsCauseIcd.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumnIsCauseIcd.Caption = "CĐ phụ";
            this.gridColumnIsCauseIcd.ColumnEdit = this.CheckEditICDCause;
            this.gridColumnIsCauseIcd.FieldName = "ICD_CAUSE_CHECK";
            this.gridColumnIsCauseIcd.Name = "gridColumnIsCauseIcd";
            this.gridColumnIsCauseIcd.ToolTip = "Chẩn đoán phụ";
            this.gridColumnIsCauseIcd.Visible = true;
            this.gridColumnIsCauseIcd.VisibleIndex = 3;
            this.gridColumnIsCauseIcd.Width = 62;
            // 
            // CheckEditICDCause
            // 
            this.CheckEditICDCause.AutoHeight = false;
            this.CheckEditICDCause.Name = "CheckEditICDCause";
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.btnSkip);
            this.layoutControl1.Controls.Add(this.btnSearch);
            this.layoutControl1.Controls.Add(this.txtKeyword);
            this.layoutControl1.Controls.Add(this.labelControl1);
            this.layoutControl1.Controls.Add(this.btnAddIcd);
            this.layoutControl1.Controls.Add(this.btnPrint);
            this.layoutControl1.Controls.Add(this.gridControlMissingIcd);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 29);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(271, 144, 250, 350);
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(644, 297);
            this.layoutControl1.TabIndex = 30;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(324, 25);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(93, 22);
            this.btnSearch.StyleController = this.layoutControl1;
            this.btnSearch.TabIndex = 34;
            this.btnSearch.Text = "Tìm (Ctrl F)";
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // txtKeyword
            // 
            this.txtKeyword.Location = new System.Drawing.Point(2, 25);
            this.txtKeyword.MenuManager = this.barManager2;
            this.txtKeyword.Name = "txtKeyword";
            this.txtKeyword.Size = new System.Drawing.Size(318, 20);
            this.txtKeyword.StyleController = this.layoutControl1;
            this.txtKeyword.TabIndex = 33;
            // 
            // barManager2
            // 
            this.barManager2.DockControls.Add(this.barDockControl1);
            this.barManager2.DockControls.Add(this.barDockControl2);
            this.barManager2.DockControls.Add(this.barDockControl3);
            this.barManager2.DockControls.Add(this.barDockControl4);
            this.barManager2.Form = this;
            this.barManager2.MaxItemId = 0;
            // 
            // barDockControl1
            // 
            this.barDockControl1.CausesValidation = false;
            this.barDockControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControl1.Location = new System.Drawing.Point(0, 29);
            this.barDockControl1.Size = new System.Drawing.Size(644, 0);
            // 
            // barDockControl2
            // 
            this.barDockControl2.CausesValidation = false;
            this.barDockControl2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControl2.Location = new System.Drawing.Point(0, 326);
            this.barDockControl2.Size = new System.Drawing.Size(644, 0);
            // 
            // barDockControl3
            // 
            this.barDockControl3.CausesValidation = false;
            this.barDockControl3.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControl3.Location = new System.Drawing.Point(0, 29);
            this.barDockControl3.Size = new System.Drawing.Size(0, 297);
            // 
            // barDockControl4
            // 
            this.barDockControl4.CausesValidation = false;
            this.barDockControl4.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControl4.Location = new System.Drawing.Point(644, 29);
            this.barDockControl4.Size = new System.Drawing.Size(0, 297);
            // 
            // labelControl1
            // 
            this.labelControl1.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.labelControl1.Location = new System.Drawing.Point(2, 5);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(640, 13);
            this.labelControl1.StyleController = this.layoutControl1;
            this.labelControl1.TabIndex = 32;
            this.labelControl1.Text = "Các dịch vụ chỉ cho phép chỉ định trong các trường hợp chẩn đoán sau:";
            // 
            // btnAddIcd
            // 
            this.btnAddIcd.Location = new System.Drawing.Point(456, 273);
            this.btnAddIcd.Name = "btnAddIcd";
            this.btnAddIcd.Size = new System.Drawing.Size(105, 22);
            this.btnAddIcd.StyleController = this.layoutControl1;
            this.btnAddIcd.TabIndex = 31;
            this.btnAddIcd.Text = "Bổ sung chẩn đoán";
            this.btnAddIcd.Click += new System.EventHandler(this.btnAddIcd_Click);
            // 
            // btnPrint
            // 
            this.btnPrint.Location = new System.Drawing.Point(565, 273);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(77, 22);
            this.btnPrint.StyleController = this.layoutControl1;
            this.btnPrint.TabIndex = 30;
            this.btnPrint.Text = "Đóng";
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.layoutControlItem2,
            this.emptySpaceItem1,
            this.layoutControlItem3,
            this.layoutControlItem4,
            this.layoutControlItem5,
            this.layoutControlItem6,
            this.emptySpaceItem2,
            this.lciSkip});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "Root";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(644, 297);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.gridControlMissingIcd;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 49);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(644, 222);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.btnPrint;
            this.layoutControlItem2.Location = new System.Drawing.Point(563, 271);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(81, 26);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(0, 271);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(386, 26);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.btnAddIcd;
            this.layoutControlItem3.Location = new System.Drawing.Point(454, 271);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(109, 26);
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextVisible = false;
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.Control = this.labelControl1;
            this.layoutControlItem4.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 5, 5);
            this.layoutControlItem4.Size = new System.Drawing.Size(644, 23);
            this.layoutControlItem4.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem4.TextToControlDistance = 0;
            this.layoutControlItem4.TextVisible = false;
            // 
            // layoutControlItem5
            // 
            this.layoutControlItem5.Control = this.txtKeyword;
            this.layoutControlItem5.Location = new System.Drawing.Point(0, 23);
            this.layoutControlItem5.Name = "layoutControlItem5";
            this.layoutControlItem5.Size = new System.Drawing.Size(322, 26);
            this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem5.TextVisible = false;
            // 
            // layoutControlItem6
            // 
            this.layoutControlItem6.Control = this.btnSearch;
            this.layoutControlItem6.Location = new System.Drawing.Point(322, 23);
            this.layoutControlItem6.Name = "layoutControlItem6";
            this.layoutControlItem6.Size = new System.Drawing.Size(97, 26);
            this.layoutControlItem6.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem6.TextVisible = false;
            // 
            // emptySpaceItem2
            // 
            this.emptySpaceItem2.AllowHotTrack = false;
            this.emptySpaceItem2.Location = new System.Drawing.Point(419, 23);
            this.emptySpaceItem2.Name = "emptySpaceItem2";
            this.emptySpaceItem2.Size = new System.Drawing.Size(225, 26);
            this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
            // 
            // barManager1
            // 
            this.barManager1.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
            this.bar1});
            this.barManager1.Form = this;
            this.barManager1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.bbtnPrint,
            this.bbtnSearch});
            this.barManager1.MaxItemId = 2;
            // 
            // bar1
            // 
            this.bar1.BarName = "Tools";
            this.bar1.DockCol = 0;
            this.bar1.DockRow = 0;
            this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.bbtnPrint),
            new DevExpress.XtraBars.LinkPersistInfo(this.bbtnSearch)});
            this.bar1.Text = "Tools";
            this.bar1.Visible = false;
            // 
            // bbtnPrint
            // 
            this.bbtnPrint.Caption = "In (Ctrl P)";
            this.bbtnPrint.Id = 0;
            this.bbtnPrint.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P));
            this.bbtnPrint.Name = "bbtnPrint";
            this.bbtnPrint.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bbtnPrint_ItemClick);
            // 
            // bbtnSearch
            // 
            this.bbtnSearch.Caption = "Tìm (Ctrl F)";
            this.bbtnSearch.Id = 1;
            this.bbtnSearch.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F));
            this.bbtnSearch.Name = "bbtnSearch";
            this.bbtnSearch.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bbtnSearch_ItemClick);
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 29);
            this.barDockControlTop.Size = new System.Drawing.Size(644, 0);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 326);
            this.barDockControlBottom.Size = new System.Drawing.Size(644, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 29);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 297);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(644, 29);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 297);
            // 
            // btnSkip
            // 
            this.btnSkip.Location = new System.Drawing.Point(388, 273);
            this.btnSkip.Name = "btnSkip";
            this.btnSkip.Size = new System.Drawing.Size(64, 22);
            this.btnSkip.StyleController = this.layoutControl1;
            this.btnSkip.TabIndex = 35;
            this.btnSkip.Text = "Bỏ qua";
            this.btnSkip.Click += new System.EventHandler(this.btnSkip_Click);
            // 
            // lciSkip
            // 
            this.lciSkip.Control = this.btnSkip;
            this.lciSkip.Location = new System.Drawing.Point(386, 271);
            this.lciSkip.Name = "lciSkip";
            this.lciSkip.Size = new System.Drawing.Size(68, 26);
            this.lciSkip.TextSize = new System.Drawing.Size(0, 0);
            this.lciSkip.TextVisible = false;
            this.lciSkip.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            // 
            // frmMissingIcd
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(644, 326);
            this.Controls.Add(this.layoutControl1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Controls.Add(this.barDockControl3);
            this.Controls.Add(this.barDockControl4);
            this.Controls.Add(this.barDockControl2);
            this.Controls.Add(this.barDockControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmMissingIcd";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Thiếu thông tin chẩn đoán";
            this.Load += new System.EventHandler(this.frmDetail_Load);
            this.Controls.SetChildIndex(this.barDockControl1, 0);
            this.Controls.SetChildIndex(this.barDockControl2, 0);
            this.Controls.SetChildIndex(this.barDockControl4, 0);
            this.Controls.SetChildIndex(this.barDockControl3, 0);
            this.Controls.SetChildIndex(this.barDockControlTop, 0);
            this.Controls.SetChildIndex(this.barDockControlBottom, 0);
            this.Controls.SetChildIndex(this.barDockControlRight, 0);
            this.Controls.SetChildIndex(this.barDockControlLeft, 0);
            this.Controls.SetChildIndex(this.layoutControl1, 0);
            ((System.ComponentModel.ISupportInitialize)(this.gridControlMissingIcd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewMissingIcd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CheckEditMainICD)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CheckEditICDCause)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtKeyword.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciSkip)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraGrid.GridControl gridControlMissingIcd;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewMissingIcd;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnServiceName;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnIcdName;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnIsMainIcd;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnIsCauseIcd;
        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraBars.BarManager barManager2;
        private DevExpress.XtraBars.BarDockControl barDockControl1;
        private DevExpress.XtraBars.BarDockControl barDockControl2;
        private DevExpress.XtraBars.BarDockControl barDockControl3;
        private DevExpress.XtraBars.BarDockControl barDockControl4;
        private DevExpress.XtraEditors.SimpleButton btnPrint;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.Bar bar1;
        private DevExpress.XtraBars.BarButtonItem bbtnPrint;
        private DevExpress.XtraEditors.SimpleButton btnAddIcd;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraEditors.SimpleButton btnSearch;
        private DevExpress.XtraEditors.TextEdit txtKeyword;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem6;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
        private DevExpress.XtraBars.BarButtonItem bbtnSearch;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit CheckEditMainICD;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit CheckEditICDCause;
        private DevExpress.XtraEditors.SimpleButton btnSkip;
        private DevExpress.XtraLayout.LayoutControlItem lciSkip;
    }
}
