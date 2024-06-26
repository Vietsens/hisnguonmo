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
namespace HIS.Desktop.Plugins.PatientDocumentIssued
{
    partial class UCPatientDocumentIssued
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UCPatientDocumentIssued));
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.treeEmrDocument = new DevExpress.XtraTreeList.TreeList();
            this.treeEmpmtyColumn = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListColumn1 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.repositoryItemPictureEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit();
            this.tc_DOCUMENT_CODE = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.tc_DOCUMENT_NAME = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.tc_DOCUMENT_TYPE_NAME = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.tc_CREATE_TIME = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.tc_REQUEST_LOGINNAME = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.imageCollection1 = new DevExpress.Utils.ImageCollection(this.components);
            this.toolTipControllerGrid = new DevExpress.Utils.ToolTipController(this.components);
            this.layoutControl7 = new DevExpress.XtraLayout.LayoutControl();
            this.txtPatientCode = new DevExpress.XtraEditors.TextEdit();
            this.navBarControl1 = new DevExpress.XtraNavBar.NavBarControl();
            this.navCreateTime = new DevExpress.XtraNavBar.NavBarGroup();
            this.navBarGroupControlContainer1 = new DevExpress.XtraNavBar.NavBarGroupControlContainer();
            this.layoutControl2 = new DevExpress.XtraLayout.LayoutControl();
            this.btnNextDayInHospital = new DevExpress.XtraEditors.SimpleButton();
            this.btnPreDayInHospital = new DevExpress.XtraEditors.SimpleButton();
            this.btnCodeFind = new DevExpress.XtraEditors.DropDownButton();
            this.dtInHospital = new DevExpress.XtraEditors.DateEdit();
            this.dtInDateCome = new DevExpress.XtraEditors.DateEdit();
            this.layoutControlGroup2 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem7 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceInHospital = new DevExpress.XtraLayout.EmptySpaceItem();
            this.layoutInDateCome = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcibtnPreviousInHospital = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcibtnNextInHospital = new DevExpress.XtraLayout.LayoutControlItem();
            this.navBarGroupControlContainer2 = new DevExpress.XtraNavBar.NavBarGroupControlContainer();
            this.layoutControl3 = new DevExpress.XtraLayout.LayoutControl();
            this.btnNextDayOutHospital = new DevExpress.XtraEditors.SimpleButton();
            this.btnPreDayOutHospital = new DevExpress.XtraEditors.SimpleButton();
            this.cboOutOfHospital = new DevExpress.XtraEditors.DropDownButton();
            this.dtOutHospital = new DevExpress.XtraEditors.DateEdit();
            this.dtOutDateCome = new DevExpress.XtraEditors.DateEdit();
            this.layoutControlGroup3 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem10 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem11 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutOutDateCome = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceOutHospital = new DevExpress.XtraLayout.EmptySpaceItem();
            this.lciPreviousOutHospital = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciNextOutHospital = new DevExpress.XtraLayout.LayoutControlItem();
            this.navBarGroupControlContainer3 = new DevExpress.XtraNavBar.NavBarGroupControlContainer();
            this.layoutControl4 = new DevExpress.XtraLayout.LayoutControl();
            this.cboEmrDocumentType = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridLookUpEdit3View = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.layoutControlGroup4 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem14 = new DevExpress.XtraLayout.LayoutControlItem();
            this.navBarGroupControlContainer4 = new DevExpress.XtraNavBar.NavBarGroupControlContainer();
            this.layoutControl5 = new DevExpress.XtraLayout.LayoutControl();
            this.cboSignatureStatus = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridLookUpEdit4View = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.layoutControlGroup5 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem15 = new DevExpress.XtraLayout.LayoutControlItem();
            this.navBarGroupControlContainer5 = new DevExpress.XtraNavBar.NavBarGroupControlContainer();
            this.layoutControl6 = new DevExpress.XtraLayout.LayoutControl();
            this.rdoPrinted = new DevExpress.XtraEditors.CheckEdit();
            this.rdoNotPrinted = new DevExpress.XtraEditors.CheckEdit();
            this.layoutControlGroup6 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem8 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem9 = new DevExpress.XtraLayout.LayoutControlItem();
            this.navBarGroupControlContainer6 = new DevExpress.XtraNavBar.NavBarGroupControlContainer();
            this.layoutControl8 = new DevExpress.XtraLayout.LayoutControl();
            this.cboDepartmentFinish = new DevExpress.XtraEditors.GridLookUpEdit();
            this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.bbtnFind = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.gridLookUpEdit1View = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.layoutControlGroup7 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem12 = new DevExpress.XtraLayout.LayoutControlItem();
            this.navOutTime = new DevExpress.XtraNavBar.NavBarGroup();
            this.navDocumentType = new DevExpress.XtraNavBar.NavBarGroup();
            this.navSignatureStatus = new DevExpress.XtraNavBar.NavBarGroup();
            this.navPrintingStatus = new DevExpress.XtraNavBar.NavBarGroup();
            this.navBarDepartmentEnd = new DevExpress.XtraNavBar.NavBarGroup();
            this.txtPatientName = new DevExpress.XtraEditors.TextEdit();
            this.txtTreatmentCode = new DevExpress.XtraEditors.TextEdit();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem21 = new DevExpress.XtraLayout.LayoutControlItem();
            this.btnPrintAllCheck = new DevExpress.XtraEditors.SimpleButton();
            this.btnFind = new DevExpress.XtraEditors.SimpleButton();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem19 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem18 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem20 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.treeEmrDocument)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemPictureEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageCollection1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl7)).BeginInit();
            this.layoutControl7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtPatientCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.navBarControl1)).BeginInit();
            this.navBarControl1.SuspendLayout();
            this.navBarGroupControlContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl2)).BeginInit();
            this.layoutControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dtInHospital.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtInHospital.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtInDateCome.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtInDateCome.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceInHospital)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutInDateCome)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcibtnPreviousInHospital)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcibtnNextInHospital)).BeginInit();
            this.navBarGroupControlContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl3)).BeginInit();
            this.layoutControl3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dtOutHospital.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtOutHospital.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtOutDateCome.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtOutDateCome.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem10)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem11)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutOutDateCome)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceOutHospital)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPreviousOutHospital)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciNextOutHospital)).BeginInit();
            this.navBarGroupControlContainer3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl4)).BeginInit();
            this.layoutControl4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboEmrDocumentType.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit3View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem14)).BeginInit();
            this.navBarGroupControlContainer4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl5)).BeginInit();
            this.layoutControl5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboSignatureStatus.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit4View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem15)).BeginInit();
            this.navBarGroupControlContainer5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl6)).BeginInit();
            this.layoutControl6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rdoPrinted.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rdoNotPrinted.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).BeginInit();
            this.navBarGroupControlContainer6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl8)).BeginInit();
            this.layoutControl8.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboDepartmentFinish.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem12)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPatientName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTreatmentCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem21)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem19)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem18)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem20)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.treeEmrDocument);
            this.layoutControl1.Controls.Add(this.layoutControl7);
            this.layoutControl1.Controls.Add(this.btnPrintAllCheck);
            this.layoutControl1.Controls.Add(this.btnFind);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 29);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(1044, 576);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // treeEmrDocument
            // 
            this.treeEmrDocument.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] {
            this.treeEmpmtyColumn,
            this.treeListColumn1,
            this.tc_DOCUMENT_CODE,
            this.tc_DOCUMENT_NAME,
            this.tc_DOCUMENT_TYPE_NAME,
            this.tc_CREATE_TIME,
            this.tc_REQUEST_LOGINNAME});
            this.treeEmrDocument.Cursor = System.Windows.Forms.Cursors.Default;
            this.treeEmrDocument.KeyFieldName = "CONCRETE_ID__IN_SETY";
            this.treeEmrDocument.Location = new System.Drawing.Point(195, 2);
            this.treeEmrDocument.Name = "treeEmrDocument";
            this.treeEmrDocument.OptionsBehavior.AllowIndeterminateCheckState = true;
            this.treeEmrDocument.OptionsBehavior.AllowPixelScrolling = DevExpress.Utils.DefaultBoolean.True;
            this.treeEmrDocument.OptionsBehavior.AllowRecursiveNodeChecking = true;
            this.treeEmrDocument.OptionsBehavior.AutoPopulateColumns = false;
            this.treeEmrDocument.OptionsBehavior.EnableFiltering = true;
            this.treeEmrDocument.OptionsBehavior.PopulateServiceColumns = true;
            this.treeEmrDocument.OptionsFilter.FilterMode = DevExpress.XtraTreeList.FilterMode.Smart;
            this.treeEmrDocument.OptionsFind.FindDelay = 100;
            this.treeEmrDocument.OptionsFind.FindNullPrompt = "Nhập chuỗi tìm kiếm ...";
            this.treeEmrDocument.OptionsFind.ShowClearButton = false;
            this.treeEmrDocument.OptionsFind.ShowFindButton = false;
            this.treeEmrDocument.OptionsSelection.MultiSelect = true;
            this.treeEmrDocument.OptionsView.FocusRectStyle = DevExpress.XtraTreeList.DrawFocusRectStyle.RowFullFocus;
            this.treeEmrDocument.OptionsView.ShowCheckBoxes = true;
            this.treeEmrDocument.OptionsView.ShowHorzLines = false;
            this.treeEmrDocument.OptionsView.ShowIndicator = false;
            this.treeEmrDocument.OptionsView.ShowVertLines = false;
            this.treeEmrDocument.ParentFieldName = "PARENT_ID__IN_SETY";
            this.treeEmrDocument.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemPictureEdit1});
            this.treeEmrDocument.SelectImageList = this.imageCollection1;
            this.treeEmrDocument.ShowButtonMode = DevExpress.XtraTreeList.ShowButtonModeEnum.ShowAlways;
            this.treeEmrDocument.Size = new System.Drawing.Size(847, 572);
            this.treeEmrDocument.StateImageList = this.imageCollection1;
            this.treeEmrDocument.TabIndex = 2;
            this.treeEmrDocument.ToolTipController = this.toolTipControllerGrid;
            this.treeEmrDocument.GetStateImage += new DevExpress.XtraTreeList.GetStateImageEventHandler(this.treeEmrDocument_GetStateImage);
            this.treeEmrDocument.GetSelectImage += new DevExpress.XtraTreeList.GetSelectImageEventHandler(this.treeEmrDocument_GetSelectImage);
            this.treeEmrDocument.StateImageClick += new DevExpress.XtraTreeList.NodeClickEventHandler(this.treeEmrDocument_StateImageClick);
            this.treeEmrDocument.NodeCellStyle += new DevExpress.XtraTreeList.GetCustomNodeCellStyleEventHandler(this.treeEmrDocument_NodeCellStyle);
            this.treeEmrDocument.CustomUnboundColumnData += new DevExpress.XtraTreeList.CustomColumnDataEventHandler(this.treeEmrDocument_CustomUnboundColumnData);
            this.treeEmrDocument.BeforeCheckNode += new DevExpress.XtraTreeList.CheckNodeEventHandler(this.treeEmrDocument_BeforeCheckNode);
            this.treeEmrDocument.CustomDrawColumnHeader += new DevExpress.XtraTreeList.CustomDrawColumnHeaderEventHandler(this.treeEmrDocument_CustomDrawColumnHeader);
            // 
            // treeEmpmtyColumn
            // 
            this.treeEmpmtyColumn.Caption = " ";
            this.treeEmpmtyColumn.FieldName = "EmpmtyColumn";
            this.treeEmpmtyColumn.MinWidth = 70;
            this.treeEmpmtyColumn.Name = "treeEmpmtyColumn";
            this.treeEmpmtyColumn.OptionsColumn.AllowEdit = false;
            this.treeEmpmtyColumn.Visible = true;
            this.treeEmpmtyColumn.VisibleIndex = 0;
            this.treeEmpmtyColumn.Width = 71;
            // 
            // treeListColumn1
            // 
            this.treeListColumn1.Caption = " ";
            this.treeListColumn1.ColumnEdit = this.repositoryItemPictureEdit1;
            this.treeListColumn1.FieldName = "IMG";
            this.treeListColumn1.MinWidth = 25;
            this.treeListColumn1.Name = "treeListColumn1";
            this.treeListColumn1.OptionsColumn.AllowEdit = false;
            this.treeListColumn1.UnboundType = DevExpress.XtraTreeList.Data.UnboundColumnType.Object;
            this.treeListColumn1.Visible = true;
            this.treeListColumn1.VisibleIndex = 1;
            this.treeListColumn1.Width = 26;
            // 
            // repositoryItemPictureEdit1
            // 
            this.repositoryItemPictureEdit1.Name = "repositoryItemPictureEdit1";
            this.repositoryItemPictureEdit1.NullText = " ";
            // 
            // tc_DOCUMENT_CODE
            // 
            this.tc_DOCUMENT_CODE.Caption = "Mã văn bản";
            this.tc_DOCUMENT_CODE.FieldName = "DOCUMENT_CODE";
            this.tc_DOCUMENT_CODE.MinWidth = 70;
            this.tc_DOCUMENT_CODE.Name = "tc_DOCUMENT_CODE";
            this.tc_DOCUMENT_CODE.OptionsColumn.AllowEdit = false;
            this.tc_DOCUMENT_CODE.Visible = true;
            this.tc_DOCUMENT_CODE.VisibleIndex = 2;
            this.tc_DOCUMENT_CODE.Width = 514;
            // 
            // tc_DOCUMENT_NAME
            // 
            this.tc_DOCUMENT_NAME.Caption = "Tên văn bản";
            this.tc_DOCUMENT_NAME.FieldName = "DOCUMENT_NAME";
            this.tc_DOCUMENT_NAME.Name = "tc_DOCUMENT_NAME";
            this.tc_DOCUMENT_NAME.OptionsColumn.AllowEdit = false;
            this.tc_DOCUMENT_NAME.Visible = true;
            this.tc_DOCUMENT_NAME.VisibleIndex = 3;
            this.tc_DOCUMENT_NAME.Width = 262;
            // 
            // tc_DOCUMENT_TYPE_NAME
            // 
            this.tc_DOCUMENT_TYPE_NAME.Caption = "Loại văn bản";
            this.tc_DOCUMENT_TYPE_NAME.FieldName = "DOCUMENT_TYPE_NAME";
            this.tc_DOCUMENT_TYPE_NAME.Name = "tc_DOCUMENT_TYPE_NAME";
            this.tc_DOCUMENT_TYPE_NAME.OptionsColumn.AllowEdit = false;
            this.tc_DOCUMENT_TYPE_NAME.Visible = true;
            this.tc_DOCUMENT_TYPE_NAME.VisibleIndex = 4;
            this.tc_DOCUMENT_TYPE_NAME.Width = 259;
            // 
            // tc_CREATE_TIME
            // 
            this.tc_CREATE_TIME.Caption = "Thời gian tạo";
            this.tc_CREATE_TIME.FieldName = "CREATE_TIME_STR";
            this.tc_CREATE_TIME.Name = "tc_CREATE_TIME";
            this.tc_CREATE_TIME.OptionsColumn.AllowEdit = false;
            this.tc_CREATE_TIME.UnboundType = DevExpress.XtraTreeList.Data.UnboundColumnType.Object;
            this.tc_CREATE_TIME.Visible = true;
            this.tc_CREATE_TIME.VisibleIndex = 5;
            this.tc_CREATE_TIME.Width = 258;
            // 
            // tc_REQUEST_LOGINNAME
            // 
            this.tc_REQUEST_LOGINNAME.Caption = "Người tạo";
            this.tc_REQUEST_LOGINNAME.FieldName = "REQUEST_LOGINNAME";
            this.tc_REQUEST_LOGINNAME.Name = "tc_REQUEST_LOGINNAME";
            this.tc_REQUEST_LOGINNAME.OptionsColumn.AllowEdit = false;
            this.tc_REQUEST_LOGINNAME.Visible = true;
            this.tc_REQUEST_LOGINNAME.VisibleIndex = 6;
            this.tc_REQUEST_LOGINNAME.Width = 258;
            // 
            // imageCollection1
            // 
            this.imageCollection1.ImageStream = ((DevExpress.Utils.ImageCollectionStreamer)(resources.GetObject("imageCollection1.ImageStream")));
            this.imageCollection1.InsertGalleryImage("show_16x16.png", "office2013/actions/show_16x16.png", DevExpress.Images.ImageResourceCache.Default.GetImage("office2013/actions/show_16x16.png"), 0);
            this.imageCollection1.Images.SetKeyName(0, "show_16x16.png");
            // 
            // toolTipControllerGrid
            // 
            this.toolTipControllerGrid.GetActiveObjectInfo += new DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventHandler(this.toolTipControllerGrid_GetActiveObjectInfo);
            // 
            // layoutControl7
            // 
            this.layoutControl7.Controls.Add(this.txtPatientCode);
            this.layoutControl7.Controls.Add(this.navBarControl1);
            this.layoutControl7.Controls.Add(this.txtPatientName);
            this.layoutControl7.Controls.Add(this.txtTreatmentCode);
            this.layoutControl7.HiddenItems.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem3});
            this.layoutControl7.Location = new System.Drawing.Point(2, 2);
            this.layoutControl7.Name = "layoutControl7";
            this.layoutControl7.Root = this.Root;
            this.layoutControl7.Size = new System.Drawing.Size(189, 546);
            this.layoutControl7.TabIndex = 0;
            this.layoutControl7.Text = "layoutControl7";
            // 
            // txtPatientCode
            // 
            this.txtPatientCode.EditValue = "";
            this.txtPatientCode.Location = new System.Drawing.Point(85, 2);
            this.txtPatientCode.Name = "txtPatientCode";
            this.txtPatientCode.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.txtPatientCode.Properties.Appearance.Options.UseForeColor = true;
            this.txtPatientCode.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtPatientCode.Size = new System.Drawing.Size(102, 20);
            this.txtPatientCode.StyleController = this.layoutControl7;
            this.txtPatientCode.TabIndex = 10;
            this.txtPatientCode.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtPatientCode_PreviewKeyDown);
            // 
            // navBarControl1
            // 
            this.navBarControl1.ActiveGroup = this.navCreateTime;
            this.navBarControl1.Controls.Add(this.navBarGroupControlContainer1);
            this.navBarControl1.Controls.Add(this.navBarGroupControlContainer2);
            this.navBarControl1.Controls.Add(this.navBarGroupControlContainer3);
            this.navBarControl1.Controls.Add(this.navBarGroupControlContainer4);
            this.navBarControl1.Controls.Add(this.navBarGroupControlContainer5);
            this.navBarControl1.Controls.Add(this.navBarGroupControlContainer6);
            this.navBarControl1.Groups.AddRange(new DevExpress.XtraNavBar.NavBarGroup[] {
            this.navCreateTime,
            this.navOutTime,
            this.navDocumentType,
            this.navSignatureStatus,
            this.navPrintingStatus,
            this.navBarDepartmentEnd});
            this.navBarControl1.Location = new System.Drawing.Point(2, 50);
            this.navBarControl1.Name = "navBarControl1";
            this.navBarControl1.OptionsNavPane.ExpandedWidth = 185;
            this.navBarControl1.Size = new System.Drawing.Size(185, 494);
            this.navBarControl1.TabIndex = 9;
            this.navBarControl1.Text = "navBarControl1";
            // 
            // navCreateTime
            // 
            this.navCreateTime.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.navCreateTime.Appearance.Options.UseFont = true;
            this.navCreateTime.Caption = "Thời gian tạo";
            this.navCreateTime.ControlContainer = this.navBarGroupControlContainer1;
            this.navCreateTime.Expanded = true;
            this.navCreateTime.GroupClientHeight = 90;
            this.navCreateTime.GroupStyle = DevExpress.XtraNavBar.NavBarGroupStyle.ControlContainer;
            this.navCreateTime.Name = "navCreateTime";
            // 
            // navBarGroupControlContainer1
            // 
            this.navBarGroupControlContainer1.Appearance.BackColor = System.Drawing.SystemColors.Control;
            this.navBarGroupControlContainer1.Appearance.Options.UseBackColor = true;
            this.navBarGroupControlContainer1.Controls.Add(this.layoutControl2);
            this.navBarGroupControlContainer1.Name = "navBarGroupControlContainer1";
            this.navBarGroupControlContainer1.Size = new System.Drawing.Size(177, 86);
            this.navBarGroupControlContainer1.TabIndex = 0;
            // 
            // layoutControl2
            // 
            this.layoutControl2.Controls.Add(this.btnNextDayInHospital);
            this.layoutControl2.Controls.Add(this.btnPreDayInHospital);
            this.layoutControl2.Controls.Add(this.btnCodeFind);
            this.layoutControl2.Controls.Add(this.dtInHospital);
            this.layoutControl2.Controls.Add(this.dtInDateCome);
            this.layoutControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl2.Location = new System.Drawing.Point(0, 0);
            this.layoutControl2.Name = "layoutControl2";
            this.layoutControl2.Root = this.layoutControlGroup2;
            this.layoutControl2.Size = new System.Drawing.Size(177, 86);
            this.layoutControl2.TabIndex = 0;
            this.layoutControl2.Text = "layoutControl2";
            // 
            // btnNextDayInHospital
            // 
            this.btnNextDayInHospital.Image = ((System.Drawing.Image)(resources.GetObject("btnNextDayInHospital.Image")));
            this.btnNextDayInHospital.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnNextDayInHospital.Location = new System.Drawing.Point(150, 52);
            this.btnNextDayInHospital.Name = "btnNextDayInHospital";
            this.btnNextDayInHospital.Size = new System.Drawing.Size(25, 22);
            this.btnNextDayInHospital.StyleController = this.layoutControl2;
            this.btnNextDayInHospital.TabIndex = 8;
            this.btnNextDayInHospital.Click += new System.EventHandler(this.btnNextDayInHospital_Click);
            // 
            // btnPreDayInHospital
            // 
            this.btnPreDayInHospital.Image = ((System.Drawing.Image)(resources.GetObject("btnPreDayInHospital.Image")));
            this.btnPreDayInHospital.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnPreDayInHospital.Location = new System.Drawing.Point(121, 52);
            this.btnPreDayInHospital.Name = "btnPreDayInHospital";
            this.btnPreDayInHospital.Size = new System.Drawing.Size(25, 22);
            this.btnPreDayInHospital.StyleController = this.layoutControl2;
            this.btnPreDayInHospital.TabIndex = 7;
            this.btnPreDayInHospital.Click += new System.EventHandler(this.btnPreDayInHospital_Click);
            // 
            // btnCodeFind
            // 
            this.btnCodeFind.Location = new System.Drawing.Point(2, 2);
            this.btnCodeFind.Name = "btnCodeFind";
            this.btnCodeFind.Size = new System.Drawing.Size(86, 22);
            this.btnCodeFind.StyleController = this.layoutControl2;
            this.btnCodeFind.TabIndex = 4;
            this.btnCodeFind.Text = "Trong ngày";
            this.btnCodeFind.Click += new System.EventHandler(this.btnCodeFind_Click);
            // 
            // dtInHospital
            // 
            this.dtInHospital.EditValue = null;
            this.dtInHospital.Location = new System.Drawing.Point(92, 2);
            this.dtInHospital.Name = "dtInHospital";
            this.dtInHospital.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtInHospital.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtInHospital.Properties.CalendarTimeProperties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtInHospital.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
            this.dtInHospital.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtInHospital.Properties.EditFormat.FormatString = "dd/MM/yyyy";
            this.dtInHospital.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtInHospital.Properties.Mask.EditMask = "dd/MM/yyyy";
            this.dtInHospital.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
            this.dtInHospital.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.dtInHospital.Size = new System.Drawing.Size(83, 20);
            this.dtInHospital.StyleController = this.layoutControl2;
            this.dtInHospital.TabIndex = 5;
            // 
            // dtInDateCome
            // 
            this.dtInDateCome.EditValue = "";
            this.dtInDateCome.Location = new System.Drawing.Point(117, 28);
            this.dtInDateCome.Name = "dtInDateCome";
            this.dtInDateCome.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtInDateCome.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtInDateCome.Properties.DisplayFormat.FormatString = "";
            this.dtInDateCome.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.dtInDateCome.Properties.EditFormat.FormatString = "";
            this.dtInDateCome.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.dtInDateCome.Properties.Mask.EditMask = "";
            this.dtInDateCome.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
            this.dtInDateCome.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.dtInDateCome.Size = new System.Drawing.Size(58, 20);
            this.dtInDateCome.StyleController = this.layoutControl2;
            this.dtInDateCome.TabIndex = 8;
            // 
            // layoutControlGroup2
            // 
            this.layoutControlGroup2.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.False;
            this.layoutControlGroup2.GroupBordersVisible = false;
            this.layoutControlGroup2.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem5,
            this.layoutControlItem7,
            this.emptySpaceInHospital,
            this.layoutInDateCome,
            this.lcibtnPreviousInHospital,
            this.lcibtnNextInHospital});
            this.layoutControlGroup2.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup2.Name = "layoutControlGroup2";
            this.layoutControlGroup2.Size = new System.Drawing.Size(177, 86);
            this.layoutControlGroup2.TextVisible = false;
            // 
            // layoutControlItem5
            // 
            this.layoutControlItem5.Control = this.btnCodeFind;
            this.layoutControlItem5.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem5.Name = "layoutControlItem5";
            this.layoutControlItem5.Size = new System.Drawing.Size(90, 26);
            this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem5.TextVisible = false;
            // 
            // layoutControlItem7
            // 
            this.layoutControlItem7.Control = this.dtInHospital;
            this.layoutControlItem7.Location = new System.Drawing.Point(90, 0);
            this.layoutControlItem7.Name = "layoutControlItem7";
            this.layoutControlItem7.Size = new System.Drawing.Size(87, 26);
            this.layoutControlItem7.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem7.TextVisible = false;
            // 
            // emptySpaceInHospital
            // 
            this.emptySpaceInHospital.AllowHotTrack = false;
            this.emptySpaceInHospital.Location = new System.Drawing.Point(0, 50);
            this.emptySpaceInHospital.Name = "emptySpaceInHospital";
            this.emptySpaceInHospital.Size = new System.Drawing.Size(119, 36);
            this.emptySpaceInHospital.TextSize = new System.Drawing.Size(0, 0);
            // 
            // layoutInDateCome
            // 
            this.layoutInDateCome.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutInDateCome.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutInDateCome.Control = this.dtInDateCome;
            this.layoutInDateCome.Location = new System.Drawing.Point(0, 26);
            this.layoutInDateCome.Name = "layoutInDateCome";
            this.layoutInDateCome.Size = new System.Drawing.Size(177, 24);
            this.layoutInDateCome.Text = "đến ngày:";
            this.layoutInDateCome.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutInDateCome.TextSize = new System.Drawing.Size(110, 20);
            this.layoutInDateCome.TextToControlDistance = 5;
            // 
            // lcibtnPreviousInHospital
            // 
            this.lcibtnPreviousInHospital.Control = this.btnPreDayInHospital;
            this.lcibtnPreviousInHospital.Location = new System.Drawing.Point(119, 50);
            this.lcibtnPreviousInHospital.Name = "lcibtnPreviousInHospital";
            this.lcibtnPreviousInHospital.Size = new System.Drawing.Size(29, 36);
            this.lcibtnPreviousInHospital.TextSize = new System.Drawing.Size(0, 0);
            this.lcibtnPreviousInHospital.TextVisible = false;
            // 
            // lcibtnNextInHospital
            // 
            this.lcibtnNextInHospital.Control = this.btnNextDayInHospital;
            this.lcibtnNextInHospital.Location = new System.Drawing.Point(148, 50);
            this.lcibtnNextInHospital.Name = "lcibtnNextInHospital";
            this.lcibtnNextInHospital.Size = new System.Drawing.Size(29, 36);
            this.lcibtnNextInHospital.TextSize = new System.Drawing.Size(0, 0);
            this.lcibtnNextInHospital.TextVisible = false;
            // 
            // navBarGroupControlContainer2
            // 
            this.navBarGroupControlContainer2.Appearance.BackColor = System.Drawing.SystemColors.Control;
            this.navBarGroupControlContainer2.Appearance.Options.UseBackColor = true;
            this.navBarGroupControlContainer2.Controls.Add(this.layoutControl3);
            this.navBarGroupControlContainer2.Name = "navBarGroupControlContainer2";
            this.navBarGroupControlContainer2.Size = new System.Drawing.Size(177, 86);
            this.navBarGroupControlContainer2.TabIndex = 1;
            // 
            // layoutControl3
            // 
            this.layoutControl3.Controls.Add(this.btnNextDayOutHospital);
            this.layoutControl3.Controls.Add(this.btnPreDayOutHospital);
            this.layoutControl3.Controls.Add(this.cboOutOfHospital);
            this.layoutControl3.Controls.Add(this.dtOutHospital);
            this.layoutControl3.Controls.Add(this.dtOutDateCome);
            this.layoutControl3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl3.Location = new System.Drawing.Point(0, 0);
            this.layoutControl3.Name = "layoutControl3";
            this.layoutControl3.Root = this.layoutControlGroup3;
            this.layoutControl3.Size = new System.Drawing.Size(177, 86);
            this.layoutControl3.TabIndex = 0;
            this.layoutControl3.Text = "layoutControl3";
            // 
            // btnNextDayOutHospital
            // 
            this.btnNextDayOutHospital.Image = ((System.Drawing.Image)(resources.GetObject("btnNextDayOutHospital.Image")));
            this.btnNextDayOutHospital.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnNextDayOutHospital.Location = new System.Drawing.Point(150, 52);
            this.btnNextDayOutHospital.Name = "btnNextDayOutHospital";
            this.btnNextDayOutHospital.Size = new System.Drawing.Size(25, 22);
            this.btnNextDayOutHospital.StyleController = this.layoutControl3;
            this.btnNextDayOutHospital.TabIndex = 7;
            this.btnNextDayOutHospital.Click += new System.EventHandler(this.btnNextDayOutHospital_Click);
            // 
            // btnPreDayOutHospital
            // 
            this.btnPreDayOutHospital.Image = ((System.Drawing.Image)(resources.GetObject("btnPreDayOutHospital.Image")));
            this.btnPreDayOutHospital.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnPreDayOutHospital.Location = new System.Drawing.Point(121, 52);
            this.btnPreDayOutHospital.Name = "btnPreDayOutHospital";
            this.btnPreDayOutHospital.Size = new System.Drawing.Size(25, 22);
            this.btnPreDayOutHospital.StyleController = this.layoutControl3;
            this.btnPreDayOutHospital.TabIndex = 6;
            this.btnPreDayOutHospital.Click += new System.EventHandler(this.btnPreDayOutHospital_Click);
            // 
            // cboOutOfHospital
            // 
            this.cboOutOfHospital.Location = new System.Drawing.Point(2, 2);
            this.cboOutOfHospital.Name = "cboOutOfHospital";
            this.cboOutOfHospital.Size = new System.Drawing.Size(87, 22);
            this.cboOutOfHospital.StyleController = this.layoutControl3;
            this.cboOutOfHospital.TabIndex = 4;
            this.cboOutOfHospital.Text = "Trong ngày";
            this.cboOutOfHospital.Click += new System.EventHandler(this.cboOutOfHospital_Click);
            // 
            // dtOutHospital
            // 
            this.dtOutHospital.EditValue = null;
            this.dtOutHospital.Location = new System.Drawing.Point(93, 2);
            this.dtOutHospital.Name = "dtOutHospital";
            this.dtOutHospital.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtOutHospital.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtOutHospital.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
            this.dtOutHospital.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtOutHospital.Properties.EditFormat.FormatString = "dd/MM/yyyy";
            this.dtOutHospital.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtOutHospital.Properties.Mask.EditMask = "dd/MM/yyyy";
            this.dtOutHospital.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
            this.dtOutHospital.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.dtOutHospital.Size = new System.Drawing.Size(82, 20);
            this.dtOutHospital.StyleController = this.layoutControl3;
            this.dtOutHospital.TabIndex = 5;
            // 
            // dtOutDateCome
            // 
            this.dtOutDateCome.EditValue = null;
            this.dtOutDateCome.Location = new System.Drawing.Point(117, 28);
            this.dtOutDateCome.Name = "dtOutDateCome";
            this.dtOutDateCome.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtOutDateCome.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtOutDateCome.Properties.DisplayFormat.FormatString = "";
            this.dtOutDateCome.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.dtOutDateCome.Properties.EditFormat.FormatString = "";
            this.dtOutDateCome.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.dtOutDateCome.Properties.Mask.EditMask = "";
            this.dtOutDateCome.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
            this.dtOutDateCome.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.dtOutDateCome.Size = new System.Drawing.Size(58, 20);
            this.dtOutDateCome.StyleController = this.layoutControl3;
            this.dtOutDateCome.TabIndex = 8;
            // 
            // layoutControlGroup3
            // 
            this.layoutControlGroup3.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.False;
            this.layoutControlGroup3.GroupBordersVisible = false;
            this.layoutControlGroup3.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem10,
            this.layoutControlItem11,
            this.layoutOutDateCome,
            this.emptySpaceOutHospital,
            this.lciPreviousOutHospital,
            this.lciNextOutHospital});
            this.layoutControlGroup3.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup3.Name = "layoutControlGroup3";
            this.layoutControlGroup3.Size = new System.Drawing.Size(177, 86);
            this.layoutControlGroup3.TextVisible = false;
            // 
            // layoutControlItem10
            // 
            this.layoutControlItem10.Control = this.cboOutOfHospital;
            this.layoutControlItem10.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem10.Name = "layoutControlItem10";
            this.layoutControlItem10.Size = new System.Drawing.Size(91, 26);
            this.layoutControlItem10.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem10.TextVisible = false;
            // 
            // layoutControlItem11
            // 
            this.layoutControlItem11.Control = this.dtOutHospital;
            this.layoutControlItem11.Location = new System.Drawing.Point(91, 0);
            this.layoutControlItem11.Name = "layoutControlItem11";
            this.layoutControlItem11.Size = new System.Drawing.Size(86, 26);
            this.layoutControlItem11.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem11.TextVisible = false;
            // 
            // layoutOutDateCome
            // 
            this.layoutOutDateCome.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutOutDateCome.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutOutDateCome.Control = this.dtOutDateCome;
            this.layoutOutDateCome.Location = new System.Drawing.Point(0, 26);
            this.layoutOutDateCome.Name = "layoutOutDateCome";
            this.layoutOutDateCome.Size = new System.Drawing.Size(177, 24);
            this.layoutOutDateCome.Text = "đến ngày:";
            this.layoutOutDateCome.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutOutDateCome.TextSize = new System.Drawing.Size(110, 20);
            this.layoutOutDateCome.TextToControlDistance = 5;
            // 
            // emptySpaceOutHospital
            // 
            this.emptySpaceOutHospital.AllowHotTrack = false;
            this.emptySpaceOutHospital.Location = new System.Drawing.Point(0, 50);
            this.emptySpaceOutHospital.Name = "emptySpaceOutHospital";
            this.emptySpaceOutHospital.Size = new System.Drawing.Size(119, 36);
            this.emptySpaceOutHospital.TextSize = new System.Drawing.Size(0, 0);
            // 
            // lciPreviousOutHospital
            // 
            this.lciPreviousOutHospital.Control = this.btnPreDayOutHospital;
            this.lciPreviousOutHospital.Location = new System.Drawing.Point(119, 50);
            this.lciPreviousOutHospital.Name = "lciPreviousOutHospital";
            this.lciPreviousOutHospital.Size = new System.Drawing.Size(29, 36);
            this.lciPreviousOutHospital.TextSize = new System.Drawing.Size(0, 0);
            this.lciPreviousOutHospital.TextVisible = false;
            // 
            // lciNextOutHospital
            // 
            this.lciNextOutHospital.Control = this.btnNextDayOutHospital;
            this.lciNextOutHospital.Location = new System.Drawing.Point(148, 50);
            this.lciNextOutHospital.Name = "lciNextOutHospital";
            this.lciNextOutHospital.Size = new System.Drawing.Size(29, 36);
            this.lciNextOutHospital.TextSize = new System.Drawing.Size(0, 0);
            this.lciNextOutHospital.TextVisible = false;
            // 
            // navBarGroupControlContainer3
            // 
            this.navBarGroupControlContainer3.Appearance.BackColor = System.Drawing.SystemColors.Control;
            this.navBarGroupControlContainer3.Appearance.Options.UseBackColor = true;
            this.navBarGroupControlContainer3.Controls.Add(this.layoutControl4);
            this.navBarGroupControlContainer3.Name = "navBarGroupControlContainer3";
            this.navBarGroupControlContainer3.Size = new System.Drawing.Size(177, 46);
            this.navBarGroupControlContainer3.TabIndex = 2;
            // 
            // layoutControl4
            // 
            this.layoutControl4.Controls.Add(this.cboEmrDocumentType);
            this.layoutControl4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl4.Location = new System.Drawing.Point(0, 0);
            this.layoutControl4.Name = "layoutControl4";
            this.layoutControl4.Root = this.layoutControlGroup4;
            this.layoutControl4.Size = new System.Drawing.Size(177, 46);
            this.layoutControl4.TabIndex = 0;
            this.layoutControl4.Text = "layoutControl4";
            // 
            // cboEmrDocumentType
            // 
            this.cboEmrDocumentType.EditValue = "";
            this.cboEmrDocumentType.Location = new System.Drawing.Point(2, 2);
            this.cboEmrDocumentType.Name = "cboEmrDocumentType";
            this.cboEmrDocumentType.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.cboEmrDocumentType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)});
            this.cboEmrDocumentType.Properties.NullText = "";
            this.cboEmrDocumentType.Properties.View = this.gridLookUpEdit3View;
            this.cboEmrDocumentType.Properties.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboEmrDocumentType_Properties_ButtonClick);
            this.cboEmrDocumentType.Size = new System.Drawing.Size(173, 20);
            this.cboEmrDocumentType.StyleController = this.layoutControl4;
            this.cboEmrDocumentType.TabIndex = 4;
            this.cboEmrDocumentType.CustomDisplayText += new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(this.cboEmrDocumentType_CustomDisplayText);
            // 
            // gridLookUpEdit3View
            // 
            this.gridLookUpEdit3View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridLookUpEdit3View.Name = "gridLookUpEdit3View";
            this.gridLookUpEdit3View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridLookUpEdit3View.OptionsView.ShowGroupPanel = false;
            // 
            // layoutControlGroup4
            // 
            this.layoutControlGroup4.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.False;
            this.layoutControlGroup4.GroupBordersVisible = false;
            this.layoutControlGroup4.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem14});
            this.layoutControlGroup4.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup4.Name = "layoutControlGroup4";
            this.layoutControlGroup4.Size = new System.Drawing.Size(177, 46);
            this.layoutControlGroup4.TextVisible = false;
            // 
            // layoutControlItem14
            // 
            this.layoutControlItem14.Control = this.cboEmrDocumentType;
            this.layoutControlItem14.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem14.Name = "layoutControlItem14";
            this.layoutControlItem14.Size = new System.Drawing.Size(177, 46);
            this.layoutControlItem14.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem14.TextVisible = false;
            // 
            // navBarGroupControlContainer4
            // 
            this.navBarGroupControlContainer4.Appearance.BackColor = System.Drawing.SystemColors.Control;
            this.navBarGroupControlContainer4.Appearance.Options.UseBackColor = true;
            this.navBarGroupControlContainer4.Controls.Add(this.layoutControl5);
            this.navBarGroupControlContainer4.Name = "navBarGroupControlContainer4";
            this.navBarGroupControlContainer4.Size = new System.Drawing.Size(177, 46);
            this.navBarGroupControlContainer4.TabIndex = 3;
            // 
            // layoutControl5
            // 
            this.layoutControl5.Controls.Add(this.cboSignatureStatus);
            this.layoutControl5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl5.Location = new System.Drawing.Point(0, 0);
            this.layoutControl5.Name = "layoutControl5";
            this.layoutControl5.Root = this.layoutControlGroup5;
            this.layoutControl5.Size = new System.Drawing.Size(177, 46);
            this.layoutControl5.TabIndex = 0;
            this.layoutControl5.Text = "layoutControl5";
            // 
            // cboSignatureStatus
            // 
            this.cboSignatureStatus.Location = new System.Drawing.Point(2, 2);
            this.cboSignatureStatus.Name = "cboSignatureStatus";
            this.cboSignatureStatus.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.cboSignatureStatus.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)});
            this.cboSignatureStatus.Properties.NullText = "";
            this.cboSignatureStatus.Properties.View = this.gridLookUpEdit4View;
            this.cboSignatureStatus.Properties.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboSignatureStatus_Properties_ButtonClick);
            this.cboSignatureStatus.Size = new System.Drawing.Size(173, 20);
            this.cboSignatureStatus.StyleController = this.layoutControl5;
            this.cboSignatureStatus.TabIndex = 4;
            this.cboSignatureStatus.CustomDisplayText += new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(this.cboSignatureStatus_CustomDisplayText);
            // 
            // gridLookUpEdit4View
            // 
            this.gridLookUpEdit4View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridLookUpEdit4View.Name = "gridLookUpEdit4View";
            this.gridLookUpEdit4View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridLookUpEdit4View.OptionsView.ShowGroupPanel = false;
            // 
            // layoutControlGroup5
            // 
            this.layoutControlGroup5.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.False;
            this.layoutControlGroup5.GroupBordersVisible = false;
            this.layoutControlGroup5.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem15});
            this.layoutControlGroup5.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup5.Name = "layoutControlGroup5";
            this.layoutControlGroup5.Size = new System.Drawing.Size(177, 46);
            this.layoutControlGroup5.TextVisible = false;
            // 
            // layoutControlItem15
            // 
            this.layoutControlItem15.Control = this.cboSignatureStatus;
            this.layoutControlItem15.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem15.Name = "layoutControlItem15";
            this.layoutControlItem15.Size = new System.Drawing.Size(177, 46);
            this.layoutControlItem15.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem15.TextVisible = false;
            // 
            // navBarGroupControlContainer5
            // 
            this.navBarGroupControlContainer5.Appearance.BackColor = System.Drawing.SystemColors.Control;
            this.navBarGroupControlContainer5.Appearance.Options.UseBackColor = true;
            this.navBarGroupControlContainer5.Controls.Add(this.layoutControl6);
            this.navBarGroupControlContainer5.Name = "navBarGroupControlContainer5";
            this.navBarGroupControlContainer5.Size = new System.Drawing.Size(177, 76);
            this.navBarGroupControlContainer5.TabIndex = 4;
            // 
            // layoutControl6
            // 
            this.layoutControl6.Controls.Add(this.rdoPrinted);
            this.layoutControl6.Controls.Add(this.rdoNotPrinted);
            this.layoutControl6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl6.Location = new System.Drawing.Point(0, 0);
            this.layoutControl6.Name = "layoutControl6";
            this.layoutControl6.Root = this.layoutControlGroup6;
            this.layoutControl6.Size = new System.Drawing.Size(177, 76);
            this.layoutControl6.TabIndex = 0;
            this.layoutControl6.Text = "layoutControl6";
            // 
            // rdoPrinted
            // 
            this.rdoPrinted.Location = new System.Drawing.Point(2, 25);
            this.rdoPrinted.Name = "rdoPrinted";
            this.rdoPrinted.Properties.Caption = "Đã in";
            this.rdoPrinted.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.rdoPrinted.Size = new System.Drawing.Size(173, 19);
            this.rdoPrinted.StyleController = this.layoutControl6;
            this.rdoPrinted.TabIndex = 5;
            this.rdoPrinted.CheckedChanged += new System.EventHandler(this.rdoPrinted_CheckedChanged);
            // 
            // rdoNotPrinted
            // 
            this.rdoNotPrinted.EditValue = true;
            this.rdoNotPrinted.Location = new System.Drawing.Point(2, 2);
            this.rdoNotPrinted.Name = "rdoNotPrinted";
            this.rdoNotPrinted.Properties.Caption = "Chưa in";
            this.rdoNotPrinted.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.rdoNotPrinted.Size = new System.Drawing.Size(173, 19);
            this.rdoNotPrinted.StyleController = this.layoutControl6;
            this.rdoNotPrinted.TabIndex = 4;
            this.rdoNotPrinted.CheckedChanged += new System.EventHandler(this.rdoNotPrinted_CheckedChanged);
            // 
            // layoutControlGroup6
            // 
            this.layoutControlGroup6.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.False;
            this.layoutControlGroup6.GroupBordersVisible = false;
            this.layoutControlGroup6.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem8,
            this.layoutControlItem9});
            this.layoutControlGroup6.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup6.Name = "layoutControlGroup6";
            this.layoutControlGroup6.Size = new System.Drawing.Size(177, 76);
            this.layoutControlGroup6.TextVisible = false;
            // 
            // layoutControlItem8
            // 
            this.layoutControlItem8.Control = this.rdoNotPrinted;
            this.layoutControlItem8.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem8.Name = "layoutControlItem8";
            this.layoutControlItem8.Size = new System.Drawing.Size(177, 23);
            this.layoutControlItem8.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem8.TextVisible = false;
            // 
            // layoutControlItem9
            // 
            this.layoutControlItem9.Control = this.rdoPrinted;
            this.layoutControlItem9.Location = new System.Drawing.Point(0, 23);
            this.layoutControlItem9.Name = "layoutControlItem9";
            this.layoutControlItem9.Size = new System.Drawing.Size(177, 53);
            this.layoutControlItem9.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem9.TextVisible = false;
            // 
            // navBarGroupControlContainer6
            // 
            this.navBarGroupControlContainer6.Appearance.BackColor = System.Drawing.SystemColors.Control;
            this.navBarGroupControlContainer6.Appearance.Options.UseBackColor = true;
            this.navBarGroupControlContainer6.Controls.Add(this.layoutControl8);
            this.navBarGroupControlContainer6.Name = "navBarGroupControlContainer6";
            this.navBarGroupControlContainer6.Size = new System.Drawing.Size(177, 76);
            this.navBarGroupControlContainer6.TabIndex = 5;
            // 
            // layoutControl8
            // 
            this.layoutControl8.Controls.Add(this.cboDepartmentFinish);
            this.layoutControl8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl8.Location = new System.Drawing.Point(0, 0);
            this.layoutControl8.Margin = new System.Windows.Forms.Padding(0);
            this.layoutControl8.Name = "layoutControl8";
            this.layoutControl8.Root = this.layoutControlGroup7;
            this.layoutControl8.Size = new System.Drawing.Size(177, 76);
            this.layoutControl8.TabIndex = 0;
            this.layoutControl8.Text = "layoutControl8";
            // 
            // cboDepartmentFinish
            // 
            this.cboDepartmentFinish.Location = new System.Drawing.Point(2, 2);
            this.cboDepartmentFinish.MenuManager = this.barManager1;
            this.cboDepartmentFinish.Name = "cboDepartmentFinish";
            this.cboDepartmentFinish.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)});
            this.cboDepartmentFinish.Properties.NullText = "";
            this.cboDepartmentFinish.Properties.PopupSizeable = false;
            this.cboDepartmentFinish.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
            this.cboDepartmentFinish.Properties.View = this.gridLookUpEdit1View;
            this.cboDepartmentFinish.Size = new System.Drawing.Size(173, 20);
            this.cboDepartmentFinish.StyleController = this.layoutControl8;
            this.cboDepartmentFinish.TabIndex = 4;
            this.cboDepartmentFinish.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboDepartmentFinish_ButtonClick);
            this.cboDepartmentFinish.CustomDisplayText += new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(this.cboDepartmentFinish_CustomDisplayText);
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
            this.bbtnFind});
            this.barManager1.MaxItemId = 1;
            // 
            // bar1
            // 
            this.bar1.BarName = "Tools";
            this.bar1.DockCol = 0;
            this.bar1.DockRow = 0;
            this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.bbtnFind)});
            this.bar1.Text = "Tools";
            this.bar1.Visible = false;
            // 
            // bbtnFind
            // 
            this.bbtnFind.Caption = "Tìm (Ctrl F)";
            this.bbtnFind.Id = 0;
            this.bbtnFind.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F));
            this.bbtnFind.Name = "bbtnFind";
            this.bbtnFind.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bbtnFind_ItemClick);
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Size = new System.Drawing.Size(1044, 29);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 605);
            this.barDockControlBottom.Size = new System.Drawing.Size(1044, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 29);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 576);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(1044, 29);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 576);
            // 
            // gridLookUpEdit1View
            // 
            this.gridLookUpEdit1View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridLookUpEdit1View.Name = "gridLookUpEdit1View";
            this.gridLookUpEdit1View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridLookUpEdit1View.OptionsView.ShowGroupPanel = false;
            // 
            // layoutControlGroup7
            // 
            this.layoutControlGroup7.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.False;
            this.layoutControlGroup7.GroupBordersVisible = false;
            this.layoutControlGroup7.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem12});
            this.layoutControlGroup7.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup7.Name = "layoutControlGroup7";
            this.layoutControlGroup7.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup7.Size = new System.Drawing.Size(177, 76);
            this.layoutControlGroup7.TextVisible = false;
            // 
            // layoutControlItem12
            // 
            this.layoutControlItem12.Control = this.cboDepartmentFinish;
            this.layoutControlItem12.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem12.Name = "layoutControlItem12";
            this.layoutControlItem12.Size = new System.Drawing.Size(177, 76);
            this.layoutControlItem12.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem12.TextVisible = false;
            // 
            // navOutTime
            // 
            this.navOutTime.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.navOutTime.Appearance.Options.UseFont = true;
            this.navOutTime.Caption = "Thời gian ra viện";
            this.navOutTime.ControlContainer = this.navBarGroupControlContainer2;
            this.navOutTime.GroupClientHeight = 90;
            this.navOutTime.GroupStyle = DevExpress.XtraNavBar.NavBarGroupStyle.ControlContainer;
            this.navOutTime.Name = "navOutTime";
            // 
            // navDocumentType
            // 
            this.navDocumentType.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.navDocumentType.Appearance.Options.UseFont = true;
            this.navDocumentType.Caption = "Loại văn bản";
            this.navDocumentType.ControlContainer = this.navBarGroupControlContainer3;
            this.navDocumentType.Expanded = true;
            this.navDocumentType.GroupClientHeight = 50;
            this.navDocumentType.GroupStyle = DevExpress.XtraNavBar.NavBarGroupStyle.ControlContainer;
            this.navDocumentType.Name = "navDocumentType";
            // 
            // navSignatureStatus
            // 
            this.navSignatureStatus.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.navSignatureStatus.Appearance.Options.UseFont = true;
            this.navSignatureStatus.Caption = "Trạng thái ký";
            this.navSignatureStatus.ControlContainer = this.navBarGroupControlContainer4;
            this.navSignatureStatus.GroupClientHeight = 50;
            this.navSignatureStatus.GroupStyle = DevExpress.XtraNavBar.NavBarGroupStyle.ControlContainer;
            this.navSignatureStatus.Name = "navSignatureStatus";
            // 
            // navPrintingStatus
            // 
            this.navPrintingStatus.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.navPrintingStatus.Appearance.Options.UseFont = true;
            this.navPrintingStatus.Caption = "Trạng thái in";
            this.navPrintingStatus.ControlContainer = this.navBarGroupControlContainer5;
            this.navPrintingStatus.Expanded = true;
            this.navPrintingStatus.GroupClientHeight = 80;
            this.navPrintingStatus.GroupStyle = DevExpress.XtraNavBar.NavBarGroupStyle.ControlContainer;
            this.navPrintingStatus.Name = "navPrintingStatus";
            // 
            // navBarDepartmentEnd
            // 
            this.navBarDepartmentEnd.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.navBarDepartmentEnd.Appearance.Options.UseFont = true;
            this.navBarDepartmentEnd.Caption = "Khoa kết thúc";
            this.navBarDepartmentEnd.ControlContainer = this.navBarGroupControlContainer6;
            this.navBarDepartmentEnd.Expanded = true;
            this.navBarDepartmentEnd.GroupClientHeight = 80;
            this.navBarDepartmentEnd.GroupStyle = DevExpress.XtraNavBar.NavBarGroupStyle.ControlContainer;
            this.navBarDepartmentEnd.Name = "navBarDepartmentEnd";
            // 
            // txtPatientName
            // 
            this.txtPatientName.EditValue = "";
            this.txtPatientName.Location = new System.Drawing.Point(2, 26);
            this.txtPatientName.Name = "txtPatientName";
            this.txtPatientName.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.txtPatientName.Properties.Appearance.Options.UseForeColor = true;
            this.txtPatientName.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtPatientName.Size = new System.Drawing.Size(185, 20);
            this.txtPatientName.StyleController = this.layoutControl7;
            this.txtPatientName.TabIndex = 7;
            this.txtPatientName.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtKeyWord_PreviewKeyDown);
            // 
            // txtTreatmentCode
            // 
            this.txtTreatmentCode.EditValue = "";
            this.txtTreatmentCode.Location = new System.Drawing.Point(2, 2);
            this.txtTreatmentCode.Name = "txtTreatmentCode";
            this.txtTreatmentCode.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.txtTreatmentCode.Properties.Appearance.Options.UseForeColor = true;
            this.txtTreatmentCode.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtTreatmentCode.Size = new System.Drawing.Size(79, 20);
            this.txtTreatmentCode.StyleController = this.layoutControl7;
            this.txtTreatmentCode.TabIndex = 5;
            this.txtTreatmentCode.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtTreatmentCode_PreviewKeyDown);
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Location = new System.Drawing.Point(156, 0);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(156, 24);
            this.layoutControlItem3.TextSize = new System.Drawing.Size(50, 20);
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.False;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem6,
            this.layoutControlItem4,
            this.layoutControlItem2,
            this.layoutControlItem21});
            this.Root.Location = new System.Drawing.Point(0, 0);
            this.Root.Name = "Root";
            this.Root.Size = new System.Drawing.Size(189, 546);
            this.Root.TextVisible = false;
            // 
            // layoutControlItem6
            // 
            this.layoutControlItem6.Control = this.navBarControl1;
            this.layoutControlItem6.Location = new System.Drawing.Point(0, 48);
            this.layoutControlItem6.Name = "layoutControlItem6";
            this.layoutControlItem6.Size = new System.Drawing.Size(189, 498);
            this.layoutControlItem6.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem6.TextVisible = false;
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.Control = this.txtPatientName;
            this.layoutControlItem4.Location = new System.Drawing.Point(0, 24);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Size = new System.Drawing.Size(189, 24);
            this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem4.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.txtTreatmentCode;
            this.layoutControlItem2.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(83, 24);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // layoutControlItem21
            // 
            this.layoutControlItem21.Control = this.txtPatientCode;
            this.layoutControlItem21.Location = new System.Drawing.Point(83, 0);
            this.layoutControlItem21.Name = "layoutControlItem21";
            this.layoutControlItem21.Size = new System.Drawing.Size(106, 24);
            this.layoutControlItem21.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem21.TextVisible = false;
            // 
            // btnPrintAllCheck
            // 
            this.btnPrintAllCheck.Enabled = false;
            this.btnPrintAllCheck.Location = new System.Drawing.Point(68, 552);
            this.btnPrintAllCheck.Name = "btnPrintAllCheck";
            this.btnPrintAllCheck.Size = new System.Drawing.Size(123, 22);
            this.btnPrintAllCheck.StyleController = this.layoutControl1;
            this.btnPrintAllCheck.TabIndex = 4;
            this.btnPrintAllCheck.Text = "In cấp cho bệnh nhân";
            this.btnPrintAllCheck.Click += new System.EventHandler(this.btnPrintAllCheck_Click);
            // 
            // btnFind
            // 
            this.btnFind.Location = new System.Drawing.Point(2, 552);
            this.btnFind.Name = "btnFind";
            this.btnFind.Size = new System.Drawing.Size(62, 22);
            this.btnFind.StyleController = this.layoutControl1;
            this.btnFind.TabIndex = 3;
            this.btnFind.Text = "Tìm (Ctrl F)";
            this.btnFind.Click += new System.EventHandler(this.btnFind_Click);
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.False;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem19,
            this.layoutControlItem18,
            this.layoutControlItem20,
            this.layoutControlItem1});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Size = new System.Drawing.Size(1044, 576);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem19
            // 
            this.layoutControlItem19.Control = this.btnFind;
            this.layoutControlItem19.Location = new System.Drawing.Point(0, 550);
            this.layoutControlItem19.Name = "layoutControlItem19";
            this.layoutControlItem19.Size = new System.Drawing.Size(66, 26);
            this.layoutControlItem19.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem19.TextVisible = false;
            // 
            // layoutControlItem18
            // 
            this.layoutControlItem18.Control = this.btnPrintAllCheck;
            this.layoutControlItem18.Location = new System.Drawing.Point(66, 550);
            this.layoutControlItem18.Name = "layoutControlItem18";
            this.layoutControlItem18.Size = new System.Drawing.Size(127, 26);
            this.layoutControlItem18.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem18.TextVisible = false;
            // 
            // layoutControlItem20
            // 
            this.layoutControlItem20.Control = this.layoutControl7;
            this.layoutControlItem20.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem20.Name = "layoutControlItem20";
            this.layoutControlItem20.Size = new System.Drawing.Size(193, 550);
            this.layoutControlItem20.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem20.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.treeEmrDocument;
            this.layoutControlItem1.Location = new System.Drawing.Point(193, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(851, 576);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "circle-16_red.png");
            this.imageList1.Images.SetKeyName(1, "circle-white.png");
            this.imageList1.Images.SetKeyName(2, "circle-yellow.png");
            // 
            // UCPatientDocumentIssued
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layoutControl1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Name = "UCPatientDocumentIssued";
            this.Size = new System.Drawing.Size(1044, 605);
            this.Load += new System.EventHandler(this.UCPatientDocumentIssued_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.treeEmrDocument)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemPictureEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageCollection1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl7)).EndInit();
            this.layoutControl7.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtPatientCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.navBarControl1)).EndInit();
            this.navBarControl1.ResumeLayout(false);
            this.navBarGroupControlContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl2)).EndInit();
            this.layoutControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dtInHospital.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtInHospital.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtInDateCome.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtInDateCome.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceInHospital)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutInDateCome)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcibtnPreviousInHospital)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcibtnNextInHospital)).EndInit();
            this.navBarGroupControlContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl3)).EndInit();
            this.layoutControl3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dtOutHospital.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtOutHospital.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtOutDateCome.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtOutDateCome.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem10)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem11)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutOutDateCome)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceOutHospital)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPreviousOutHospital)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciNextOutHospital)).EndInit();
            this.navBarGroupControlContainer3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl4)).EndInit();
            this.layoutControl4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cboEmrDocumentType.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit3View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem14)).EndInit();
            this.navBarGroupControlContainer4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl5)).EndInit();
            this.layoutControl5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cboSignatureStatus.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit4View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem15)).EndInit();
            this.navBarGroupControlContainer5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl6)).EndInit();
            this.layoutControl6.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.rdoPrinted.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rdoNotPrinted.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).EndInit();
            this.navBarGroupControlContainer6.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl8)).EndInit();
            this.layoutControl8.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cboDepartmentFinish.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem12)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPatientName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTreatmentCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem21)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem19)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem18)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem20)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.TextEdit txtTreatmentCode;
        private DevExpress.XtraEditors.TextEdit txtPatientName;
        private DevExpress.XtraNavBar.NavBarControl navBarControl1;
        private DevExpress.XtraNavBar.NavBarGroup navCreateTime;
        private DevExpress.XtraNavBar.NavBarGroupControlContainer navBarGroupControlContainer1;
        private DevExpress.XtraLayout.LayoutControl layoutControl2;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup2;
        private DevExpress.XtraEditors.DropDownButton btnCodeFind;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem7;
        private DevExpress.XtraEditors.DateEdit dtInHospital;
        private DevExpress.XtraEditors.SimpleButton btnNextDayInHospital;
        private DevExpress.XtraEditors.SimpleButton btnPreDayInHospital;
        private DevExpress.XtraLayout.LayoutControlItem lcibtnPreviousInHospital;
        private DevExpress.XtraLayout.LayoutControlItem lcibtnNextInHospital;
        private DevExpress.XtraNavBar.NavBarGroupControlContainer navBarGroupControlContainer2;
        private DevExpress.XtraLayout.LayoutControl layoutControl3;
        private DevExpress.XtraEditors.DropDownButton cboOutOfHospital;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup3;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem10;
        private DevExpress.XtraNavBar.NavBarGroup navOutTime;
        private DevExpress.XtraEditors.SimpleButton btnNextDayOutHospital;
        private DevExpress.XtraEditors.SimpleButton btnPreDayOutHospital;
        private DevExpress.XtraEditors.DateEdit dtOutHospital;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem11;
        private DevExpress.XtraLayout.LayoutControlItem lciPreviousOutHospital;
        private DevExpress.XtraLayout.LayoutControlItem lciNextOutHospital;
        private DevExpress.XtraNavBar.NavBarGroupControlContainer navBarGroupControlContainer3;
        private DevExpress.XtraLayout.LayoutControl layoutControl4;
        private DevExpress.XtraEditors.GridLookUpEdit cboEmrDocumentType;
        private DevExpress.XtraGrid.Views.Grid.GridView gridLookUpEdit3View;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup4;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem14;
        private DevExpress.XtraNavBar.NavBarGroup navDocumentType;
        private DevExpress.XtraNavBar.NavBarGroupControlContainer navBarGroupControlContainer4;
        private DevExpress.XtraLayout.LayoutControl layoutControl5;
        private DevExpress.XtraEditors.GridLookUpEdit cboSignatureStatus;
        private DevExpress.XtraGrid.Views.Grid.GridView gridLookUpEdit4View;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup5;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem15;
        private DevExpress.XtraNavBar.NavBarGroup navSignatureStatus;
        private DevExpress.XtraNavBar.NavBarGroup navPrintingStatus;
        private DevExpress.XtraEditors.SimpleButton btnPrintAllCheck;
        private DevExpress.XtraEditors.SimpleButton btnFind;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceOutHospital;
        private DevExpress.XtraNavBar.NavBarGroupControlContainer navBarGroupControlContainer5;
        private DevExpress.XtraLayout.LayoutControl layoutControl6;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup6;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem19;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem18;
        private DevExpress.XtraLayout.LayoutControl layoutControl7;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem6;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem20;
        private DevExpress.XtraEditors.TextEdit txtPatientCode;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem21;
        private DevExpress.XtraEditors.DateEdit dtInDateCome;
        private DevExpress.XtraLayout.LayoutControlItem layoutInDateCome;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceInHospital;
        private DevExpress.XtraEditors.DateEdit dtOutDateCome;
        private DevExpress.XtraLayout.LayoutControlItem layoutOutDateCome;
        private DevExpress.XtraEditors.CheckEdit rdoPrinted;
        private DevExpress.XtraEditors.CheckEdit rdoNotPrinted;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem8;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem9;
        private DevExpress.XtraTreeList.TreeList treeEmrDocument;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraTreeList.Columns.TreeListColumn tc_DOCUMENT_CODE;
        private DevExpress.XtraTreeList.Columns.TreeListColumn tc_DOCUMENT_NAME;
        private DevExpress.XtraTreeList.Columns.TreeListColumn tc_DOCUMENT_TYPE_NAME;
        private DevExpress.XtraTreeList.Columns.TreeListColumn tc_CREATE_TIME;
        private DevExpress.XtraTreeList.Columns.TreeListColumn tc_REQUEST_LOGINNAME;
        private DevExpress.Utils.ImageCollection imageCollection1;
        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.Bar bar1;
        private DevExpress.XtraBars.BarButtonItem bbtnFind;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeEmpmtyColumn;
        private DevExpress.XtraNavBar.NavBarGroup navBarDepartmentEnd;
        private DevExpress.XtraNavBar.NavBarGroupControlContainer navBarGroupControlContainer6;
        private DevExpress.XtraLayout.LayoutControl layoutControl8;
        private DevExpress.XtraEditors.GridLookUpEdit cboDepartmentFinish;
        private DevExpress.XtraGrid.Views.Grid.GridView gridLookUpEdit1View;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup7;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem12;
        private System.Windows.Forms.ImageList imageList1;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn1;
        private DevExpress.Utils.ToolTipController toolTipControllerGrid;
        private DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit repositoryItemPictureEdit1;
    }
}

