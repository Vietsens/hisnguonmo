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
namespace HIS.Desktop.Plugins.HisUserEditableData
{
    partial class frmHisUserEditableData
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
            this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
            this.bar2 = new DevExpress.XtraBars.Bar();
            this.itemSearch = new DevExpress.XtraBars.BarButtonItem();
            this.itemEdit = new DevExpress.XtraBars.BarButtonItem();
            this.itemAdd = new DevExpress.XtraBars.BarButtonItem();
            this.itemRedo = new DevExpress.XtraBars.BarButtonItem();
            this.bbtnImport = new DevExpress.XtraBars.BarButtonItem();
            this.barToggleSwitchItem1 = new DevExpress.XtraBars.BarToggleSwitchItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.btnSave = new DevExpress.XtraEditors.SimpleButton();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.toggleSwitch1 = new DevExpress.XtraEditors.ToggleSwitch();
            this.btnSearch = new DevExpress.XtraEditors.SimpleButton();
            this.txtSearch = new DevExpress.XtraEditors.TextEdit();
            this.gridControlFormList = new DevExpress.XtraGrid.GridControl();
            this.gridViewFormList = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumnLOGINNAME = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnUSERNAME = new DevExpress.XtraGrid.Columns.GridColumn();
            this.ucPaging = new Inventec.UC.Paging.UcPaging();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem11 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem15 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem16 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.customGridLookUpEditWithFilterMultiColumn2View = new Inventec.Desktop.CustomControl.CustomGridViewWithFilterMultiColumn();
            this.customGridLookUpEditWithFilterMultiColumn1View = new Inventec.Desktop.CustomControl.CustomGridViewWithFilterMultiColumn();
            this.dxValidationProvider1 = new DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider(this.components);
            this.dxErrorProvider1 = new DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider(this.components);
            this.gridControl1 = new DevExpress.XtraGrid.GridControl();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.toggleSwitch1.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSearch.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlFormList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewFormList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem11)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem15)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem16)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.customGridLookUpEditWithFilterMultiColumn2View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.customGridLookUpEditWithFilterMultiColumn1View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxErrorProvider1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            this.SuspendLayout();
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
            this.barToggleSwitchItem1,
            this.itemSearch,
            this.itemEdit,
            this.itemAdd,
            this.itemRedo,
            this.bbtnImport});
            this.barManager1.MainMenu = this.bar2;
            this.barManager1.MaxItemId = 6;
            // 
            // bar2
            // 
            this.bar2.BarName = "Main menu";
            this.bar2.DockCol = 0;
            this.bar2.DockRow = 0;
            this.bar2.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar2.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.itemSearch),
            new DevExpress.XtraBars.LinkPersistInfo(this.itemEdit),
            new DevExpress.XtraBars.LinkPersistInfo(this.itemAdd),
            new DevExpress.XtraBars.LinkPersistInfo(this.itemRedo),
            new DevExpress.XtraBars.LinkPersistInfo(this.bbtnImport)});
            this.bar2.OptionsBar.Hidden = true;
            this.bar2.OptionsBar.UseWholeRow = true;
            this.bar2.Text = "Main menu";
            this.bar2.Visible = false;
            // 
            // itemSearch
            // 
            this.itemSearch.Caption = "Tìm kiếm(Ctrl F)";
            this.itemSearch.Id = 1;
            this.itemSearch.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F));
            this.itemSearch.Name = "itemSearch";
            this.itemSearch.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.itemSearch_ItemClick);
            // 
            // itemEdit
            // 
            this.itemEdit.Caption = "Sửa(Ctrl S)";
            this.itemEdit.Id = 2;
            this.itemEdit.Name = "itemEdit";
            this.itemEdit.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.itemEdit_ItemClick);
            // 
            // itemAdd
            // 
            this.itemAdd.Caption = "Lưu (Ctrl S)";
            this.itemAdd.Id = 3;
            this.itemAdd.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S));
            this.itemAdd.Name = "itemAdd";
            this.itemAdd.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.itemAdd_ItemClick);
            // 
            // itemRedo
            // 
            this.itemRedo.Caption = "Làm Lại(Ctrl R)";
            this.itemRedo.Id = 4;
            this.itemRedo.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R));
            this.itemRedo.Name = "itemRedo";
            this.itemRedo.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.itemRedo_ItemClick);
            // 
            // bbtnImport
            // 
            this.bbtnImport.Caption = "Nhập khẩu (Ctrl I)";
            this.bbtnImport.Id = 5;
            this.bbtnImport.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.I));
            this.bbtnImport.Name = "bbtnImport";
            this.bbtnImport.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bbtnImport_ItemClick);
            // 
            // barToggleSwitchItem1
            // 
            this.barToggleSwitchItem1.Caption = "barToggleSwitchItem1";
            this.barToggleSwitchItem1.Id = 0;
            this.barToggleSwitchItem1.Name = "barToggleSwitchItem1";
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Size = new System.Drawing.Size(859, 29);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 844);
            this.barDockControlBottom.Size = new System.Drawing.Size(859, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 29);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 815);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(859, 29);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 815);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(698, 785);
            this.btnSave.Margin = new System.Windows.Forms.Padding(4);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(158, 27);
            this.btnSave.StyleController = this.layoutControl1;
            this.btnSave.TabIndex = 15;
            this.btnSave.Text = "Lưu (Ctrl S)";
            this.btnSave.Click += new System.EventHandler(this.btnAdd_Click);
            this.btnSave.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.btnAdd_PreviewKeyDown);
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.toggleSwitch1);
            this.layoutControl1.Controls.Add(this.btnSave);
            this.layoutControl1.Controls.Add(this.btnSearch);
            this.layoutControl1.Controls.Add(this.txtSearch);
            this.layoutControl1.Controls.Add(this.gridControlFormList);
            this.layoutControl1.Controls.Add(this.ucPaging);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 29);
            this.layoutControl1.Margin = new System.Windows.Forms.Padding(4);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(697, 352, 250, 350);
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(859, 815);
            this.layoutControl1.TabIndex = 10;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // toggleSwitch1
            // 
            this.toggleSwitch1.Location = new System.Drawing.Point(423, 785);
            this.toggleSwitch1.Margin = new System.Windows.Forms.Padding(4);
            this.toggleSwitch1.MenuManager = this.barManager1;
            this.toggleSwitch1.Name = "toggleSwitch1";
            this.toggleSwitch1.Properties.OffText = "Tài khoản được chọn";
            this.toggleSwitch1.Properties.OnText = "Tất cả tài khoản";
            this.toggleSwitch1.Size = new System.Drawing.Size(269, 26);
            this.toggleSwitch1.StyleController = this.layoutControl1;
            this.toggleSwitch1.TabIndex = 16;
            this.toggleSwitch1.Toggled += new System.EventHandler(this.toggleSwitch1_Toggled);
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(698, 3);
            this.btnSearch.Margin = new System.Windows.Forms.Padding(4);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(158, 27);
            this.btnSearch.StyleController = this.layoutControl1;
            this.btnSearch.TabIndex = 12;
            this.btnSearch.Text = "Tìm kiếm(Ctrl F)";
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // txtSearch
            // 
            this.txtSearch.Location = new System.Drawing.Point(3, 3);
            this.txtSearch.Margin = new System.Windows.Forms.Padding(4);
            this.txtSearch.MenuManager = this.barManager1;
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Properties.EditValueChangedDelay = 500;
            this.txtSearch.Properties.EditValueChangedFiringMode = DevExpress.XtraEditors.Controls.EditValueChangedFiringMode.Buffered;
            this.txtSearch.Size = new System.Drawing.Size(689, 22);
            this.txtSearch.StyleController = this.layoutControl1;
            this.txtSearch.TabIndex = 11;
            this.txtSearch.EditValueChanged += new System.EventHandler(this.txtSearch_EditValueChanged);
            this.txtSearch.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtSearch_PreviewKeyDown);
            // 
            // gridControlFormList
            // 
            this.gridControlFormList.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(4);
            this.gridControlFormList.Location = new System.Drawing.Point(3, 36);
            this.gridControlFormList.MainView = this.gridViewFormList;
            this.gridControlFormList.Margin = new System.Windows.Forms.Padding(4);
            this.gridControlFormList.MenuManager = this.barManager1;
            this.gridControlFormList.Name = "gridControlFormList";
            this.gridControlFormList.Size = new System.Drawing.Size(853, 696);
            this.gridControlFormList.TabIndex = 13;
            this.gridControlFormList.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewFormList});
            this.gridControlFormList.Click += new System.EventHandler(this.gridControlFormList_Click);
            // 
            // gridViewFormList
            // 
            this.gridViewFormList.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumnLOGINNAME,
            this.gridColumnUSERNAME});
            this.gridViewFormList.GridControl = this.gridControlFormList;
            this.gridViewFormList.Name = "gridViewFormList";
            this.gridViewFormList.OptionsSelection.CheckBoxSelectorColumnWidth = 30;
            this.gridViewFormList.OptionsSelection.MultiSelect = true;
            this.gridViewFormList.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;
            this.gridViewFormList.OptionsView.ShowGroupPanel = false;
            this.gridViewFormList.OptionsView.ShowIndicator = false;
            this.gridViewFormList.VertScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Always;
            this.gridViewFormList.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gridViewFormList_RowCellStyle);
            this.gridViewFormList.CustomRowCellEdit += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gridViewFormList_CustomRowCellEdit);
            this.gridViewFormList.SelectionChanged += new DevExpress.Data.SelectionChangedEventHandler(this.gridViewFormList_SelectionChanged);
            this.gridViewFormList.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewFormList_CustomUnboundColumnData);
            this.gridViewFormList.Click += new System.EventHandler(this.gridViewFormList_Click);
            // 
            // gridColumnLOGINNAME
            // 
            this.gridColumnLOGINNAME.Caption = "Tài khoản";
            this.gridColumnLOGINNAME.FieldName = "LOGINNAME";
            this.gridColumnLOGINNAME.Name = "gridColumnLOGINNAME";
            this.gridColumnLOGINNAME.OptionsColumn.AllowEdit = false;
            this.gridColumnLOGINNAME.OptionsFilter.AllowAutoFilter = false;
            this.gridColumnLOGINNAME.OptionsFilter.AllowFilter = false;
            this.gridColumnLOGINNAME.OptionsFilter.AllowFilterModeChanging = DevExpress.Utils.DefaultBoolean.False;
            this.gridColumnLOGINNAME.Visible = true;
            this.gridColumnLOGINNAME.VisibleIndex = 1;
            this.gridColumnLOGINNAME.Width = 129;
            // 
            // gridColumnUSERNAME
            // 
            this.gridColumnUSERNAME.Caption = "Tên";
            this.gridColumnUSERNAME.FieldName = "USERNAME";
            this.gridColumnUSERNAME.Name = "gridColumnUSERNAME";
            this.gridColumnUSERNAME.OptionsColumn.AllowEdit = false;
            this.gridColumnUSERNAME.OptionsFilter.AllowAutoFilter = false;
            this.gridColumnUSERNAME.OptionsFilter.AllowFilter = false;
            this.gridColumnUSERNAME.OptionsFilter.AllowFilterModeChanging = DevExpress.Utils.DefaultBoolean.False;
            this.gridColumnUSERNAME.Visible = true;
            this.gridColumnUSERNAME.VisibleIndex = 2;
            this.gridColumnUSERNAME.Width = 256;
            // 
            // ucPaging
            // 
            this.ucPaging.Location = new System.Drawing.Point(3, 738);
            this.ucPaging.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ucPaging.Name = "ucPaging";
            this.ucPaging.Size = new System.Drawing.Size(853, 41);
            this.ucPaging.TabIndex = 14;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem11,
            this.layoutControlItem15,
            this.layoutControlItem16,
            this.layoutControlItem1,
            this.layoutControlItem3,
            this.emptySpaceItem1,
            this.layoutControlItem2});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "Root";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(859, 815);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem11
            // 
            this.layoutControlItem11.Control = this.txtSearch;
            this.layoutControlItem11.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem11.Name = "layoutControlItem11";
            this.layoutControlItem11.Size = new System.Drawing.Size(695, 33);
            this.layoutControlItem11.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem11.TextVisible = false;
            // 
            // layoutControlItem15
            // 
            this.layoutControlItem15.Control = this.gridControlFormList;
            this.layoutControlItem15.Location = new System.Drawing.Point(0, 33);
            this.layoutControlItem15.Name = "layoutControlItem15";
            this.layoutControlItem15.Size = new System.Drawing.Size(859, 702);
            this.layoutControlItem15.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem15.TextVisible = false;
            // 
            // layoutControlItem16
            // 
            this.layoutControlItem16.Control = this.ucPaging;
            this.layoutControlItem16.Location = new System.Drawing.Point(0, 735);
            this.layoutControlItem16.Name = "layoutControlItem16";
            this.layoutControlItem16.Size = new System.Drawing.Size(859, 47);
            this.layoutControlItem16.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem16.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.btnSearch;
            this.layoutControlItem1.Location = new System.Drawing.Point(695, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(164, 33);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.btnSave;
            this.layoutControlItem3.Location = new System.Drawing.Point(695, 782);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(164, 33);
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(0, 782);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(420, 33);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.toggleSwitch1;
            this.layoutControlItem2.Location = new System.Drawing.Point(420, 782);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(275, 33);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // customGridLookUpEditWithFilterMultiColumn2View
            // 
            this.customGridLookUpEditWithFilterMultiColumn2View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.customGridLookUpEditWithFilterMultiColumn2View.Name = "customGridLookUpEditWithFilterMultiColumn2View";
            this.customGridLookUpEditWithFilterMultiColumn2View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.customGridLookUpEditWithFilterMultiColumn2View.OptionsView.ShowGroupPanel = false;
            // 
            // customGridLookUpEditWithFilterMultiColumn1View
            // 
            this.customGridLookUpEditWithFilterMultiColumn1View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.customGridLookUpEditWithFilterMultiColumn1View.Name = "customGridLookUpEditWithFilterMultiColumn1View";
            this.customGridLookUpEditWithFilterMultiColumn1View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.customGridLookUpEditWithFilterMultiColumn1View.OptionsView.ShowGroupPanel = false;
            // 
            // dxErrorProvider1
            // 
            this.dxErrorProvider1.ContainerControl = this;
            // 
            // gridControl1
            // 
            this.gridControl1.Location = new System.Drawing.Point(12, 38);
            this.gridControl1.MainView = this.gridView1;
            this.gridControl1.MenuManager = this.barManager1;
            this.gridControl1.Name = "gridControl1";
            this.gridControl1.Size = new System.Drawing.Size(707, 478);
            this.gridControl1.TabIndex = 4;
            this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
            // 
            // gridView1
            // 
            this.gridView1.GridControl = this.gridControl1;
            this.gridView1.Name = "gridView1";
            // 
            // frmHisUserEditableData
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(859, 844);
            this.Controls.Add(this.layoutControl1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "frmHisUserEditableData";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Phân quyền chỉnh sửa dữ liệu";
            this.Load += new System.EventHandler(this.frmEmpUser_Load);
            this.Controls.SetChildIndex(this.barDockControlTop, 0);
            this.Controls.SetChildIndex(this.barDockControlBottom, 0);
            this.Controls.SetChildIndex(this.barDockControlRight, 0);
            this.Controls.SetChildIndex(this.barDockControlLeft, 0);
            this.Controls.SetChildIndex(this.layoutControl1, 0);
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.toggleSwitch1.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSearch.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlFormList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewFormList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem11)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem15)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem16)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.customGridLookUpEditWithFilterMultiColumn2View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.customGridLookUpEditWithFilterMultiColumn1View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxErrorProvider1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.Bar bar2;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraBars.BarButtonItem itemSearch;
        private DevExpress.XtraBars.BarButtonItem itemEdit;
        private DevExpress.XtraBars.BarButtonItem itemAdd;
        private DevExpress.XtraBars.BarButtonItem itemRedo;
        private DevExpress.XtraBars.BarToggleSwitchItem barToggleSwitchItem1;
        private DevExpress.XtraEditors.SimpleButton btnSave;

        private DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProvider1;
        private DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider dxErrorProvider1;
        private DevExpress.XtraGrid.GridControl gridControl1;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;

        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraEditors.SimpleButton btnSearch;
        private DevExpress.XtraEditors.TextEdit txtSearch;
        private DevExpress.XtraGrid.GridControl gridControlFormList;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewFormList;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnLOGINNAME;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnUSERNAME;
        private Inventec.UC.Paging.UcPaging ucPaging;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem11;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem15;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem16;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraBars.BarButtonItem bbtnImport;
        private Inventec.Desktop.CustomControl.CustomGridViewWithFilterMultiColumn customGridLookUpEditWithFilterMultiColumn2View;
        private Inventec.Desktop.CustomControl.CustomGridViewWithFilterMultiColumn customGridLookUpEditWithFilterMultiColumn1View;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraEditors.ToggleSwitch toggleSwitch1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
    }
}
