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
namespace HIS.Desktop.Plugins.ExportMediMatePriceList
{
    partial class frmExportPriceList
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmExportPriceList));
            this.repositoryItemCheckAll = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.trvMediMate = new DevExpress.XtraTreeList.TreeList();
            this.tc_MediMateTypeCode = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.tc_MediMateTypeName = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.tc_ServiceUnitName = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.tc_NationalName = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.tc_ManufacturerName = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.chkShowAllType = new DevExpress.XtraEditors.CheckEdit();
            this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.barButton_Export = new DevExpress.XtraBars.BarButtonItem();
            this.barButton_Find = new DevExpress.XtraBars.BarButtonItem();
            this.cboMediStock = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridLookUpEdit1View = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.chkTachTheoNhom = new DevExpress.XtraEditors.CheckEdit();
            this.btnFind = new DevExpress.XtraEditors.SimpleButton();
            this.txtKeyword = new DevExpress.XtraEditors.TextEdit();
            this.btnExport = new DevExpress.XtraEditors.SimpleButton();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciMediStock = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem7 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.imgMety = new DevExpress.Utils.ImageCollection(this.components);
            this.checkEdit = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckAll)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trvMediMate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkShowAllType.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboMediStock.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkTachTheoNhom.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtKeyword.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciMediStock)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgMety)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkEdit)).BeginInit();
            this.SuspendLayout();
            // 
            // repositoryItemCheckAll
            // 
            this.repositoryItemCheckAll.AutoHeight = false;
            this.repositoryItemCheckAll.Name = "repositoryItemCheckAll";
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.trvMediMate);
            this.layoutControl1.Controls.Add(this.chkShowAllType);
            this.layoutControl1.Controls.Add(this.cboMediStock);
            this.layoutControl1.Controls.Add(this.chkTachTheoNhom);
            this.layoutControl1.Controls.Add(this.btnFind);
            this.layoutControl1.Controls.Add(this.txtKeyword);
            this.layoutControl1.Controls.Add(this.btnExport);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 29);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(660, 432);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // trvMediMate
            // 
            this.trvMediMate.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] {
            this.tc_MediMateTypeCode,
            this.tc_MediMateTypeName,
            this.tc_ServiceUnitName,
            this.tc_NationalName,
            this.tc_ManufacturerName});
            this.trvMediMate.Cursor = System.Windows.Forms.Cursors.Default;
            this.trvMediMate.Location = new System.Drawing.Point(2, 52);
            this.trvMediMate.Name = "trvMediMate";
            this.trvMediMate.OptionsBehavior.AllowPixelScrolling = DevExpress.Utils.DefaultBoolean.True;
            this.trvMediMate.OptionsBehavior.AllowRecursiveNodeChecking = true;
            this.trvMediMate.OptionsBehavior.AutoPopulateColumns = false;
            this.trvMediMate.OptionsBehavior.EnableFiltering = true;
            this.trvMediMate.OptionsFilter.FilterMode = DevExpress.XtraTreeList.FilterMode.Smart;
            this.trvMediMate.OptionsFind.FindDelay = 100;
            this.trvMediMate.OptionsFind.FindNullPrompt = "Nhập chuỗi tìm kiếm ...";
            this.trvMediMate.OptionsFind.ShowClearButton = false;
            this.trvMediMate.OptionsFind.ShowFindButton = false;
            this.trvMediMate.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.trvMediMate.OptionsView.AutoWidth = false;
            this.trvMediMate.OptionsView.FocusRectStyle = DevExpress.XtraTreeList.DrawFocusRectStyle.RowFullFocus;
            this.trvMediMate.OptionsView.ShowCheckBoxes = true;
            this.trvMediMate.OptionsView.ShowHorzLines = false;
            this.trvMediMate.OptionsView.ShowIndicator = false;
            this.trvMediMate.OptionsView.ShowVertLines = false;
            this.trvMediMate.ParentFieldName = "PARENT_ID";
            this.trvMediMate.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.checkEdit});
            this.trvMediMate.Size = new System.Drawing.Size(656, 352);
            this.trvMediMate.TabIndex = 11;
            this.trvMediMate.AfterCheckNode += new DevExpress.XtraTreeList.NodeEventHandler(this.trvMediMate_AfterCheckNode);
            this.trvMediMate.CustomDrawColumnHeader += new DevExpress.XtraTreeList.CustomDrawColumnHeaderEventHandler(this.trvMediMate_CustomDrawColumnHeader);
            this.trvMediMate.MouseUp += new System.Windows.Forms.MouseEventHandler(this.trvMediMate_MouseUp);
            // 
            // tc_MediMateTypeCode
            // 
            this.tc_MediMateTypeCode.Caption = "Mã";
            this.tc_MediMateTypeCode.FieldName = "MEDI_MATE_TYPE_CODE";
            this.tc_MediMateTypeCode.MinWidth = 34;
            this.tc_MediMateTypeCode.Name = "tc_MediMateTypeCode";
            this.tc_MediMateTypeCode.Visible = true;
            this.tc_MediMateTypeCode.VisibleIndex = 0;
            this.tc_MediMateTypeCode.Width = 120;
            // 
            // tc_MediMateTypeName
            // 
            this.tc_MediMateTypeName.Caption = "Tên";
            this.tc_MediMateTypeName.FieldName = "MEDI_MATE_TYPE_NAME";
            this.tc_MediMateTypeName.Name = "tc_MediMateTypeName";
            this.tc_MediMateTypeName.Visible = true;
            this.tc_MediMateTypeName.VisibleIndex = 1;
            this.tc_MediMateTypeName.Width = 300;
            // 
            // tc_ServiceUnitName
            // 
            this.tc_ServiceUnitName.Caption = "Đơn vị";
            this.tc_ServiceUnitName.FieldName = "SERVICE_UNIT_NAME";
            this.tc_ServiceUnitName.Name = "tc_ServiceUnitName";
            this.tc_ServiceUnitName.Visible = true;
            this.tc_ServiceUnitName.VisibleIndex = 2;
            this.tc_ServiceUnitName.Width = 80;
            // 
            // tc_NationalName
            // 
            this.tc_NationalName.Caption = "Quốc gia";
            this.tc_NationalName.FieldName = "NATIONAL_NAME";
            this.tc_NationalName.Name = "tc_NationalName";
            this.tc_NationalName.Visible = true;
            this.tc_NationalName.VisibleIndex = 3;
            this.tc_NationalName.Width = 100;
            // 
            // tc_ManufacturerName
            // 
            this.tc_ManufacturerName.Caption = "Hãng sản xuất";
            this.tc_ManufacturerName.FieldName = "MANUFACTURER_NAME";
            this.tc_ManufacturerName.Name = "tc_ManufacturerName";
            this.tc_ManufacturerName.Visible = true;
            this.tc_ManufacturerName.VisibleIndex = 4;
            this.tc_ManufacturerName.Width = 150;
            // 
            // chkShowAllType
            // 
            this.chkShowAllType.Location = new System.Drawing.Point(552, 2);
            this.chkShowAllType.MenuManager = this.barManager1;
            this.chkShowAllType.Name = "chkShowAllType";
            this.chkShowAllType.Properties.Caption = "Hiển thị dòng hết";
            this.chkShowAllType.Size = new System.Drawing.Size(106, 19);
            this.chkShowAllType.StyleController = this.layoutControl1;
            this.chkShowAllType.TabIndex = 10;
            // 
            // barManager1
            // 
            this.barManager1.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
            this.bar1});
            this.barManager1.Form = this;
            this.barManager1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.barButton_Export,
            this.barButton_Find});
            this.barManager1.MaxItemId = 2;
            // 
            // bar1
            // 
            this.bar1.BarName = "Tools";
            this.bar1.DockCol = 0;
            this.bar1.DockRow = 0;
            this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.barButton_Export),
            new DevExpress.XtraBars.LinkPersistInfo(this.barButton_Find)});
            this.bar1.Text = "Tools";
            this.bar1.Visible = false;
            // 
            // barButton_Export
            // 
            this.barButton_Export.Caption = "Xuất (Ctrl E)";
            this.barButton_Export.Id = 0;
            this.barButton_Export.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.E));
            this.barButton_Export.Name = "barButton_Export";
            this.barButton_Export.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButton_Export_ItemClick);
            // 
            // barButton_Find
            // 
            this.barButton_Find.Caption = "Tìm (Ctrl F)";
            this.barButton_Find.Id = 1;
            this.barButton_Find.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F));
            this.barButton_Find.Name = "barButton_Find";
            this.barButton_Find.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButton_Find_ItemClick);
            // 
            // cboMediStock
            // 
            this.cboMediStock.Location = new System.Drawing.Point(97, 2);
            this.cboMediStock.MenuManager = this.barManager1;
            this.cboMediStock.Name = "cboMediStock";
            this.cboMediStock.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.cboMediStock.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboMediStock.Properties.NullText = "";
            this.cboMediStock.Properties.View = this.gridLookUpEdit1View;
            this.cboMediStock.Size = new System.Drawing.Size(451, 20);
            this.cboMediStock.StyleController = this.layoutControl1;
            this.cboMediStock.TabIndex = 9;
            this.cboMediStock.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboMediStock_Closed);
            this.cboMediStock.CustomDisplayText += new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(this.cboMediStock_CustomDisplayText);
            // 
            // gridLookUpEdit1View
            // 
            this.gridLookUpEdit1View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridLookUpEdit1View.Name = "gridLookUpEdit1View";
            this.gridLookUpEdit1View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridLookUpEdit1View.OptionsView.ShowGroupPanel = false;
            // 
            // chkTachTheoNhom
            // 
            this.chkTachTheoNhom.Location = new System.Drawing.Point(97, 408);
            this.chkTachTheoNhom.MenuManager = this.barManager1;
            this.chkTachTheoNhom.Name = "chkTachTheoNhom";
            this.chkTachTheoNhom.Properties.Caption = "";
            this.chkTachTheoNhom.Size = new System.Drawing.Size(69, 19);
            this.chkTachTheoNhom.StyleController = this.layoutControl1;
            this.chkTachTheoNhom.TabIndex = 8;
            // 
            // btnFind
            // 
            this.btnFind.Location = new System.Drawing.Point(552, 26);
            this.btnFind.Name = "btnFind";
            this.btnFind.Size = new System.Drawing.Size(106, 22);
            this.btnFind.StyleController = this.layoutControl1;
            this.btnFind.TabIndex = 7;
            this.btnFind.Text = "Tìm (Ctrl F)";
            this.btnFind.Click += new System.EventHandler(this.btnFind_Click);
            // 
            // txtKeyword
            // 
            this.txtKeyword.Location = new System.Drawing.Point(2, 26);
            this.txtKeyword.MenuManager = this.barManager1;
            this.txtKeyword.Name = "txtKeyword";
            this.txtKeyword.Properties.NullValuePrompt = "Từ khóa tìm kiếm";
            this.txtKeyword.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtKeyword.Properties.ShowNullValuePromptWhenFocused = true;
            this.txtKeyword.Size = new System.Drawing.Size(546, 20);
            this.txtKeyword.StyleController = this.layoutControl1;
            this.txtKeyword.TabIndex = 6;
            this.txtKeyword.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtKeyword_PreviewKeyDown);
            // 
            // btnExport
            // 
            this.btnExport.Location = new System.Drawing.Point(562, 408);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(96, 22);
            this.btnExport.StyleController = this.layoutControl1;
            this.btnExport.TabIndex = 5;
            this.btnExport.Text = "Xuất (Ctrl E)";
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem2,
            this.emptySpaceItem1,
            this.layoutControlItem3,
            this.layoutControlItem4,
            this.layoutControlItem5,
            this.lciMediStock,
            this.layoutControlItem7,
            this.layoutControlItem6});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(660, 432);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.btnExport;
            this.layoutControlItem2.Location = new System.Drawing.Point(560, 406);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(100, 26);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(168, 406);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(392, 26);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.txtKeyword;
            this.layoutControlItem3.Location = new System.Drawing.Point(0, 24);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(550, 26);
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextVisible = false;
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.Control = this.btnFind;
            this.layoutControlItem4.Location = new System.Drawing.Point(550, 24);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Size = new System.Drawing.Size(110, 26);
            this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem4.TextVisible = false;
            // 
            // layoutControlItem5
            // 
            this.layoutControlItem5.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem5.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem5.Control = this.chkTachTheoNhom;
            this.layoutControlItem5.Location = new System.Drawing.Point(0, 406);
            this.layoutControlItem5.Name = "layoutControlItem5";
            this.layoutControlItem5.Size = new System.Drawing.Size(168, 26);
            this.layoutControlItem5.Text = "Tách theo nhóm";
            this.layoutControlItem5.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem5.TextSize = new System.Drawing.Size(90, 20);
            this.layoutControlItem5.TextToControlDistance = 5;
            // 
            // lciMediStock
            // 
            this.lciMediStock.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciMediStock.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciMediStock.Control = this.cboMediStock;
            this.lciMediStock.Location = new System.Drawing.Point(0, 0);
            this.lciMediStock.Name = "lciMediStock";
            this.lciMediStock.Size = new System.Drawing.Size(550, 24);
            this.lciMediStock.Text = "Kho:";
            this.lciMediStock.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciMediStock.TextSize = new System.Drawing.Size(90, 20);
            this.lciMediStock.TextToControlDistance = 5;
            // 
            // layoutControlItem7
            // 
            this.layoutControlItem7.Control = this.chkShowAllType;
            this.layoutControlItem7.Location = new System.Drawing.Point(550, 0);
            this.layoutControlItem7.Name = "layoutControlItem7";
            this.layoutControlItem7.Size = new System.Drawing.Size(110, 24);
            this.layoutControlItem7.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem7.TextVisible = false;
            // 
            // layoutControlItem6
            // 
            this.layoutControlItem6.Control = this.trvMediMate;
            this.layoutControlItem6.Location = new System.Drawing.Point(0, 50);
            this.layoutControlItem6.Name = "layoutControlItem6";
            this.layoutControlItem6.Size = new System.Drawing.Size(660, 356);
            this.layoutControlItem6.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem6.TextVisible = false;
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 29);
            this.barDockControlTop.Size = new System.Drawing.Size(660, 0);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 461);
            this.barDockControlBottom.Size = new System.Drawing.Size(660, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 29);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 432);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(660, 29);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 432);
            // 
            // imgMety
            // 
            this.imgMety.ImageStream = ((DevExpress.Utils.ImageCollectionStreamer)(resources.GetObject("imgMety.ImageStream")));
            this.imgMety.Images.SetKeyName(0, "checkbox.png");
            this.imgMety.Images.SetKeyName(1, "uncheckbox.png");
            // 
            // checkEdit
            // 
            this.checkEdit.AutoHeight = false;
            this.checkEdit.Name = "checkEdit";
            // 
            // frmExportPriceList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(660, 461);
            this.Controls.Add(this.layoutControl1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Name = "frmExportPriceList";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Xuất bảng giá";
            this.Load += new System.EventHandler(this.frmExportPriceList_Load);
            this.Controls.SetChildIndex(this.barDockControlTop, 0);
            this.Controls.SetChildIndex(this.barDockControlBottom, 0);
            this.Controls.SetChildIndex(this.barDockControlRight, 0);
            this.Controls.SetChildIndex(this.barDockControlLeft, 0);
            this.Controls.SetChildIndex(this.layoutControl1, 0);
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckAll)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.trvMediMate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkShowAllType.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboMediStock.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkTachTheoNhom.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtKeyword.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciMediStock)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgMety)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkEdit)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.SimpleButton btnExport;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.Bar bar1;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraBars.BarButtonItem barButton_Export;
        private DevExpress.XtraEditors.TextEdit txtKeyword;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraEditors.SimpleButton btnFind;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraBars.BarButtonItem barButton_Find;
        private DevExpress.Utils.ImageCollection imgMety;
        private DevExpress.XtraEditors.CheckEdit chkTachTheoNhom;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
        private DevExpress.XtraEditors.CheckEdit chkShowAllType;
        private DevExpress.XtraEditors.GridLookUpEdit cboMediStock;
        private DevExpress.XtraGrid.Views.Grid.GridView gridLookUpEdit1View;
        private DevExpress.XtraLayout.LayoutControlItem lciMediStock;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem7;
        private DevExpress.XtraTreeList.TreeList trvMediMate;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem6;
        private DevExpress.XtraTreeList.Columns.TreeListColumn tc_MediMateTypeCode;
        private DevExpress.XtraTreeList.Columns.TreeListColumn tc_MediMateTypeName;
        private DevExpress.XtraTreeList.Columns.TreeListColumn tc_ServiceUnitName;
        private DevExpress.XtraTreeList.Columns.TreeListColumn tc_NationalName;
        private DevExpress.XtraTreeList.Columns.TreeListColumn tc_ManufacturerName;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheckAll;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit checkEdit;
    }
}
