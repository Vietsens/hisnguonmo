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
namespace HIS.Desktop.Plugins.NotTakenApprove
{
    partial class frmNotTakenApprove
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
            this.cboType = new DevExpress.XtraEditors.GridLookUpEdit();
            this.barManager1 = new DevExpress.XtraBars.BarManager();
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.barBtnSearch = new DevExpress.XtraBars.BarButtonItem();
            this.barBtnApprove = new DevExpress.XtraBars.BarButtonItem();
            this.barBtnNew = new DevExpress.XtraBars.BarButtonItem();
            this.gridLookUpEdit1View = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.dtApproveTimeTo = new DevExpress.XtraEditors.DateEdit();
            this.dtApproveTimeFrom = new DevExpress.XtraEditors.DateEdit();
            this.lblMediStock = new DevExpress.XtraEditors.LabelControl();
            this.btnApprove = new DevExpress.XtraEditors.SimpleButton();
            this.btnSearch = new DevExpress.XtraEditors.SimpleButton();
            this.layoutControl3 = new DevExpress.XtraLayout.LayoutControl();
            this.ucPagingUnexport = new Inventec.UC.Paging.UcPaging();
            this.gridControlExpMestUnexport = new DevExpress.XtraGrid.GridControl();
            this.gridViewExpMestUnexport = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn_Unexport_stt = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_Unexport_ExpMestCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_Unexport_ServiceReqCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_Unexport_TreatmentCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_Unexport_PatientName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_Unexport_GenderName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_Unexport_Dob = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_Unexport_CreateTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_Unexport_Creator = new DevExpress.XtraGrid.Columns.GridColumn();
            this.layoutControlGroup2 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem9 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem7 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControl2 = new DevExpress.XtraLayout.LayoutControl();
            this.ucPagingExported = new Inventec.UC.Paging.UcPaging();
            this.gridControlExpMestExported = new DevExpress.XtraGrid.GridControl();
            this.gridViewExpMestExported = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn_Exported_Stt = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_Exported_ExpMestCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_Exported_ServiceReqCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_Exported_TreatmentCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_Exported_PatientName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_Exported_GenderName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_Exported_Dob = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_Exported_ExpTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_Exported_CreateTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem8 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.lciMediStock = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciApproveTime = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem10 = new DevExpress.XtraLayout.LayoutControlItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboType.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtApproveTimeTo.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtApproveTimeTo.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtApproveTimeFrom.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtApproveTimeFrom.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl3)).BeginInit();
            this.layoutControl3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlExpMestUnexport)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewExpMestUnexport)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl2)).BeginInit();
            this.layoutControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlExpMestExported)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewExpMestExported)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciMediStock)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciApproveTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem10)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.cboType);
            this.layoutControl1.Controls.Add(this.dtApproveTimeTo);
            this.layoutControl1.Controls.Add(this.dtApproveTimeFrom);
            this.layoutControl1.Controls.Add(this.lblMediStock);
            this.layoutControl1.Controls.Add(this.btnApprove);
            this.layoutControl1.Controls.Add(this.btnSearch);
            this.layoutControl1.Controls.Add(this.layoutControl3);
            this.layoutControl1.Controls.Add(this.layoutControl2);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 29);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.OptionsView.UseDefaultDragAndDropRendering = false;
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(1100, 532);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // cboType
            // 
            this.cboType.EditValue = "";
            this.cboType.Location = new System.Drawing.Point(380, 2);
            this.cboType.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.cboType.MenuManager = this.barManager1;
            this.cboType.Name = "cboType";
            this.cboType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboType.Properties.NullText = "";
            this.cboType.Properties.View = this.gridLookUpEdit1View;
            this.cboType.Size = new System.Drawing.Size(168, 20);
            this.cboType.StyleController = this.layoutControl1;
            this.cboType.TabIndex = 11;
            this.cboType.CustomDisplayText += new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(this.cboType_CustomDisplayText);
            // 
            // barManager1
            // 
            this.barManager1.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
            this.bar1});
            this.barManager1.Form = this;
            this.barManager1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.barBtnSearch,
            this.barBtnApprove,
            this.barBtnNew});
            this.barManager1.MaxItemId = 3;
            // 
            // bar1
            // 
            this.bar1.BarName = "Tools";
            this.bar1.DockCol = 0;
            this.bar1.DockRow = 0;
            this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.barBtnSearch),
            new DevExpress.XtraBars.LinkPersistInfo(this.barBtnApprove)});
            this.bar1.Text = "Tools";
            this.bar1.Visible = false;
            // 
            // barBtnSearch
            // 
            this.barBtnSearch.Caption = "Tìm(Ctrl F)";
            this.barBtnSearch.Id = 0;
            this.barBtnSearch.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F));
            this.barBtnSearch.Name = "barBtnSearch";
            this.barBtnSearch.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barBtnSearch_ItemClick);
            // 
            // barBtnApprove
            // 
            this.barBtnApprove.Caption = "Duyệt (Ctrl S)";
            this.barBtnApprove.Id = 1;
            this.barBtnApprove.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S));
            this.barBtnApprove.Name = "barBtnApprove";
            this.barBtnApprove.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barBtnApprove_ItemClick);
            // 
            // barBtnNew
            // 
            this.barBtnNew.Caption = "Mới (Ctrl N)";
            this.barBtnNew.Id = 2;
            this.barBtnNew.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N));
            this.barBtnNew.Name = "barBtnNew";
            // 
            // gridLookUpEdit1View
            // 
            this.gridLookUpEdit1View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridLookUpEdit1View.Name = "gridLookUpEdit1View";
            this.gridLookUpEdit1View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridLookUpEdit1View.OptionsView.ShowGroupPanel = false;
            // 
            // dtApproveTimeTo
            // 
            this.dtApproveTimeTo.EditValue = null;
            this.dtApproveTimeTo.Location = new System.Drawing.Point(780, 2);
            this.dtApproveTimeTo.MenuManager = this.barManager1;
            this.dtApproveTimeTo.Name = "dtApproveTimeTo";
            this.dtApproveTimeTo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtApproveTimeTo.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtApproveTimeTo.Properties.DisplayFormat.FormatString = "dd/MM/yyyy HH:mm";
            this.dtApproveTimeTo.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtApproveTimeTo.Properties.EditFormat.FormatString = "dd/MM/yyyy HH:mm";
            this.dtApproveTimeTo.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtApproveTimeTo.Properties.Mask.EditMask = "dd/MM/yyyy HH:mm";
            this.dtApproveTimeTo.Size = new System.Drawing.Size(136, 20);
            this.dtApproveTimeTo.StyleController = this.layoutControl1;
            this.dtApproveTimeTo.TabIndex = 6;
            this.dtApproveTimeTo.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.dtApproveTimeTo_Closed);
            this.dtApproveTimeTo.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.dtApproveTimeTo_PreviewKeyDown);
            // 
            // dtApproveTimeFrom
            // 
            this.dtApproveTimeFrom.EditValue = null;
            this.dtApproveTimeFrom.Location = new System.Drawing.Point(647, 2);
            this.dtApproveTimeFrom.Name = "dtApproveTimeFrom";
            this.dtApproveTimeFrom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtApproveTimeFrom.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtApproveTimeFrom.Properties.DisplayFormat.FormatString = "dd/MM/yyyy HH:mm";
            this.dtApproveTimeFrom.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtApproveTimeFrom.Properties.EditFormat.FormatString = "dd/MM/yyyy HH:mm";
            this.dtApproveTimeFrom.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtApproveTimeFrom.Properties.Mask.EditMask = "dd/MM/yyyy HH:mm";
            this.dtApproveTimeFrom.Size = new System.Drawing.Size(129, 20);
            this.dtApproveTimeFrom.StyleController = this.layoutControl1;
            this.dtApproveTimeFrom.TabIndex = 5;
            this.dtApproveTimeFrom.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.dtApproveTime_Closed);
            this.dtApproveTimeFrom.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.dtApproveTimeFrom_PreviewKeyDown);
            // 
            // lblMediStock
            // 
            this.lblMediStock.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblMediStock.Location = new System.Drawing.Point(97, 2);
            this.lblMediStock.Name = "lblMediStock";
            this.lblMediStock.Size = new System.Drawing.Size(184, 20);
            this.lblMediStock.StyleController = this.layoutControl1;
            this.lblMediStock.TabIndex = 10;
            // 
            // btnApprove
            // 
            this.btnApprove.Location = new System.Drawing.Point(992, 508);
            this.btnApprove.Name = "btnApprove";
            this.btnApprove.Size = new System.Drawing.Size(106, 22);
            this.btnApprove.StyleController = this.layoutControl1;
            this.btnApprove.TabIndex = 8;
            this.btnApprove.Text = "Duyệt (Ctrl S)";
            this.btnApprove.Click += new System.EventHandler(this.btnApprove_Click);
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(920, 2);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(62, 22);
            this.btnSearch.StyleController = this.layoutControl1;
            this.btnSearch.TabIndex = 7;
            this.btnSearch.Text = "Tìm (Ctrl F)";
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // layoutControl3
            // 
            this.layoutControl3.Controls.Add(this.ucPagingUnexport);
            this.layoutControl3.Controls.Add(this.gridControlExpMestUnexport);
            this.layoutControl3.Location = new System.Drawing.Point(552, 28);
            this.layoutControl3.Name = "layoutControl3";
            this.layoutControl3.OptionsView.UseDefaultDragAndDropRendering = false;
            this.layoutControl3.Root = this.layoutControlGroup2;
            this.layoutControl3.Size = new System.Drawing.Size(546, 476);
            this.layoutControl3.TabIndex = 5;
            this.layoutControl3.Text = "layoutControl3";
            // 
            // ucPagingUnexport
            // 
            this.ucPagingUnexport.Location = new System.Drawing.Point(2, 453);
            this.ucPagingUnexport.Name = "ucPagingUnexport";
            this.ucPagingUnexport.Size = new System.Drawing.Size(542, 21);
            this.ucPagingUnexport.TabIndex = 5;
            // 
            // gridControlExpMestUnexport
            // 
            this.gridControlExpMestUnexport.Location = new System.Drawing.Point(2, 2);
            this.gridControlExpMestUnexport.MainView = this.gridViewExpMestUnexport;
            this.gridControlExpMestUnexport.Name = "gridControlExpMestUnexport";
            this.gridControlExpMestUnexport.Size = new System.Drawing.Size(542, 447);
            this.gridControlExpMestUnexport.TabIndex = 4;
            this.gridControlExpMestUnexport.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewExpMestUnexport});
            // 
            // gridViewExpMestUnexport
            // 
            this.gridViewExpMestUnexport.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn_Unexport_stt,
            this.gridColumn_Unexport_ExpMestCode,
            this.gridColumn1,
            this.gridColumn_Unexport_ServiceReqCode,
            this.gridColumn_Unexport_TreatmentCode,
            this.gridColumn_Unexport_PatientName,
            this.gridColumn_Unexport_GenderName,
            this.gridColumn_Unexport_Dob,
            this.gridColumn_Unexport_CreateTime,
            this.gridColumn_Unexport_Creator});
            this.gridViewExpMestUnexport.GridControl = this.gridControlExpMestUnexport;
            this.gridViewExpMestUnexport.Name = "gridViewExpMestUnexport";
            this.gridViewExpMestUnexport.OptionsSelection.CheckBoxSelectorColumnWidth = 25;
            this.gridViewExpMestUnexport.OptionsSelection.MultiSelect = true;
            this.gridViewExpMestUnexport.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;
            this.gridViewExpMestUnexport.OptionsView.ColumnAutoWidth = false;
            this.gridViewExpMestUnexport.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            this.gridViewExpMestUnexport.OptionsView.ShowGroupPanel = false;
            this.gridViewExpMestUnexport.OptionsView.ShowIndicator = false;
            this.gridViewExpMestUnexport.OptionsView.ShowViewCaption = true;
            this.gridViewExpMestUnexport.ViewCaption = "Chưa xuất";
            this.gridViewExpMestUnexport.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewExpMestUnexport_CustomUnboundColumnData);
            // 
            // gridColumn_Unexport_stt
            // 
            this.gridColumn_Unexport_stt.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn_Unexport_stt.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gridColumn_Unexport_stt.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_Unexport_stt.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_Unexport_stt.Caption = "STT";
            this.gridColumn_Unexport_stt.FieldName = "STT";
            this.gridColumn_Unexport_stt.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gridColumn_Unexport_stt.Name = "gridColumn_Unexport_stt";
            this.gridColumn_Unexport_stt.OptionsColumn.AllowEdit = false;
            this.gridColumn_Unexport_stt.OptionsColumn.ReadOnly = true;
            this.gridColumn_Unexport_stt.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn_Unexport_stt.Visible = true;
            this.gridColumn_Unexport_stt.VisibleIndex = 1;
            this.gridColumn_Unexport_stt.Width = 35;
            // 
            // gridColumn_Unexport_ExpMestCode
            // 
            this.gridColumn_Unexport_ExpMestCode.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn_Unexport_ExpMestCode.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gridColumn_Unexport_ExpMestCode.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_Unexport_ExpMestCode.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_Unexport_ExpMestCode.Caption = "Mã phiếu xuất";
            this.gridColumn_Unexport_ExpMestCode.FieldName = "EXP_MEST_CODE";
            this.gridColumn_Unexport_ExpMestCode.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gridColumn_Unexport_ExpMestCode.Name = "gridColumn_Unexport_ExpMestCode";
            this.gridColumn_Unexport_ExpMestCode.OptionsColumn.ReadOnly = true;
            this.gridColumn_Unexport_ExpMestCode.Visible = true;
            this.gridColumn_Unexport_ExpMestCode.VisibleIndex = 2;
            this.gridColumn_Unexport_ExpMestCode.Width = 90;
            // 
            // gridColumn1
            // 
            this.gridColumn1.Caption = "Loại xuất";
            this.gridColumn1.FieldName = "EXP_MEST_TYPE_DISPLAY";
            this.gridColumn1.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.OptionsColumn.AllowEdit = false;
            this.gridColumn1.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn1.Visible = true;
            this.gridColumn1.VisibleIndex = 3;
            this.gridColumn1.Width = 90;
            // 
            // gridColumn_Unexport_ServiceReqCode
            // 
            this.gridColumn_Unexport_ServiceReqCode.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn_Unexport_ServiceReqCode.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gridColumn_Unexport_ServiceReqCode.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_Unexport_ServiceReqCode.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_Unexport_ServiceReqCode.Caption = "Mã y lệnh";
            this.gridColumn_Unexport_ServiceReqCode.FieldName = "TDL_SERVICE_REQ_CODE";
            this.gridColumn_Unexport_ServiceReqCode.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gridColumn_Unexport_ServiceReqCode.Name = "gridColumn_Unexport_ServiceReqCode";
            this.gridColumn_Unexport_ServiceReqCode.OptionsColumn.ReadOnly = true;
            this.gridColumn_Unexport_ServiceReqCode.Visible = true;
            this.gridColumn_Unexport_ServiceReqCode.VisibleIndex = 4;
            this.gridColumn_Unexport_ServiceReqCode.Width = 90;
            // 
            // gridColumn_Unexport_TreatmentCode
            // 
            this.gridColumn_Unexport_TreatmentCode.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn_Unexport_TreatmentCode.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gridColumn_Unexport_TreatmentCode.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_Unexport_TreatmentCode.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_Unexport_TreatmentCode.Caption = "Mã điều trị";
            this.gridColumn_Unexport_TreatmentCode.FieldName = "TDL_TREATMENT_CODE";
            this.gridColumn_Unexport_TreatmentCode.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gridColumn_Unexport_TreatmentCode.Name = "gridColumn_Unexport_TreatmentCode";
            this.gridColumn_Unexport_TreatmentCode.OptionsColumn.ReadOnly = true;
            this.gridColumn_Unexport_TreatmentCode.Visible = true;
            this.gridColumn_Unexport_TreatmentCode.VisibleIndex = 5;
            this.gridColumn_Unexport_TreatmentCode.Width = 90;
            // 
            // gridColumn_Unexport_PatientName
            // 
            this.gridColumn_Unexport_PatientName.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_Unexport_PatientName.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_Unexport_PatientName.Caption = "Tên bệnh nhân";
            this.gridColumn_Unexport_PatientName.FieldName = "TDL_PATIENT_NAME";
            this.gridColumn_Unexport_PatientName.Name = "gridColumn_Unexport_PatientName";
            this.gridColumn_Unexport_PatientName.OptionsColumn.ReadOnly = true;
            this.gridColumn_Unexport_PatientName.Visible = true;
            this.gridColumn_Unexport_PatientName.VisibleIndex = 6;
            this.gridColumn_Unexport_PatientName.Width = 100;
            // 
            // gridColumn_Unexport_GenderName
            // 
            this.gridColumn_Unexport_GenderName.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_Unexport_GenderName.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_Unexport_GenderName.Caption = "Giới tính";
            this.gridColumn_Unexport_GenderName.FieldName = "TDL_PATIENT_GENDER_NAME";
            this.gridColumn_Unexport_GenderName.Name = "gridColumn_Unexport_GenderName";
            this.gridColumn_Unexport_GenderName.OptionsColumn.ReadOnly = true;
            this.gridColumn_Unexport_GenderName.Visible = true;
            this.gridColumn_Unexport_GenderName.VisibleIndex = 7;
            this.gridColumn_Unexport_GenderName.Width = 50;
            // 
            // gridColumn_Unexport_Dob
            // 
            this.gridColumn_Unexport_Dob.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_Unexport_Dob.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_Unexport_Dob.Caption = "Ngày sinh";
            this.gridColumn_Unexport_Dob.FieldName = "DOB_DISPLAY";
            this.gridColumn_Unexport_Dob.Name = "gridColumn_Unexport_Dob";
            this.gridColumn_Unexport_Dob.OptionsColumn.ReadOnly = true;
            this.gridColumn_Unexport_Dob.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn_Unexport_Dob.Visible = true;
            this.gridColumn_Unexport_Dob.VisibleIndex = 8;
            // 
            // gridColumn_Unexport_CreateTime
            // 
            this.gridColumn_Unexport_CreateTime.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn_Unexport_CreateTime.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_Unexport_CreateTime.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_Unexport_CreateTime.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gridColumn_Unexport_CreateTime.Caption = "Thời gian tạo";
            this.gridColumn_Unexport_CreateTime.FieldName = "CREATE_TIME_DISPLAY";
            this.gridColumn_Unexport_CreateTime.Name = "gridColumn_Unexport_CreateTime";
            this.gridColumn_Unexport_CreateTime.OptionsColumn.ReadOnly = true;
            this.gridColumn_Unexport_CreateTime.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn_Unexport_CreateTime.Visible = true;
            this.gridColumn_Unexport_CreateTime.VisibleIndex = 9;
            this.gridColumn_Unexport_CreateTime.Width = 120;
            // 
            // gridColumn_Unexport_Creator
            // 
            this.gridColumn_Unexport_Creator.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_Unexport_Creator.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_Unexport_Creator.Caption = "Người tạo";
            this.gridColumn_Unexport_Creator.FieldName = "CREATOR";
            this.gridColumn_Unexport_Creator.Name = "gridColumn_Unexport_Creator";
            this.gridColumn_Unexport_Creator.OptionsColumn.AllowEdit = false;
            this.gridColumn_Unexport_Creator.Visible = true;
            this.gridColumn_Unexport_Creator.VisibleIndex = 10;
            this.gridColumn_Unexport_Creator.Width = 100;
            // 
            // layoutControlGroup2
            // 
            this.layoutControlGroup2.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup2.GroupBordersVisible = false;
            this.layoutControlGroup2.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem9,
            this.layoutControlItem7});
            this.layoutControlGroup2.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup2.Name = "layoutControlGroup2";
            this.layoutControlGroup2.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup2.Size = new System.Drawing.Size(546, 476);
            this.layoutControlGroup2.TextVisible = false;
            // 
            // layoutControlItem9
            // 
            this.layoutControlItem9.Control = this.gridControlExpMestUnexport;
            this.layoutControlItem9.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem9.Name = "layoutControlItem9";
            this.layoutControlItem9.Size = new System.Drawing.Size(546, 451);
            this.layoutControlItem9.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem9.TextVisible = false;
            // 
            // layoutControlItem7
            // 
            this.layoutControlItem7.Control = this.ucPagingUnexport;
            this.layoutControlItem7.Location = new System.Drawing.Point(0, 451);
            this.layoutControlItem7.Name = "layoutControlItem7";
            this.layoutControlItem7.Size = new System.Drawing.Size(546, 25);
            this.layoutControlItem7.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem7.TextVisible = false;
            // 
            // layoutControl2
            // 
            this.layoutControl2.Controls.Add(this.ucPagingExported);
            this.layoutControl2.Controls.Add(this.gridControlExpMestExported);
            this.layoutControl2.Location = new System.Drawing.Point(2, 28);
            this.layoutControl2.Name = "layoutControl2";
            this.layoutControl2.OptionsView.UseDefaultDragAndDropRendering = false;
            this.layoutControl2.Root = this.Root;
            this.layoutControl2.Size = new System.Drawing.Size(546, 476);
            this.layoutControl2.TabIndex = 4;
            this.layoutControl2.Text = "layoutControl2";
            // 
            // ucPagingExported
            // 
            this.ucPagingExported.Location = new System.Drawing.Point(2, 453);
            this.ucPagingExported.Name = "ucPagingExported";
            this.ucPagingExported.Size = new System.Drawing.Size(542, 21);
            this.ucPagingExported.TabIndex = 5;
            // 
            // gridControlExpMestExported
            // 
            this.gridControlExpMestExported.Location = new System.Drawing.Point(2, 2);
            this.gridControlExpMestExported.MainView = this.gridViewExpMestExported;
            this.gridControlExpMestExported.Name = "gridControlExpMestExported";
            this.gridControlExpMestExported.Size = new System.Drawing.Size(542, 447);
            this.gridControlExpMestExported.TabIndex = 4;
            this.gridControlExpMestExported.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewExpMestExported});
            // 
            // gridViewExpMestExported
            // 
            this.gridViewExpMestExported.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn_Exported_Stt,
            this.gridColumn_Exported_ExpMestCode,
            this.gridColumn_Exported_ServiceReqCode,
            this.gridColumn_Exported_TreatmentCode,
            this.gridColumn_Exported_PatientName,
            this.gridColumn_Exported_GenderName,
            this.gridColumn_Exported_Dob,
            this.gridColumn_Exported_ExpTime,
            this.gridColumn_Exported_CreateTime});
            this.gridViewExpMestExported.GridControl = this.gridControlExpMestExported;
            this.gridViewExpMestExported.Name = "gridViewExpMestExported";
            this.gridViewExpMestExported.OptionsView.ColumnAutoWidth = false;
            this.gridViewExpMestExported.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            this.gridViewExpMestExported.OptionsView.ShowGroupPanel = false;
            this.gridViewExpMestExported.OptionsView.ShowIndicator = false;
            this.gridViewExpMestExported.OptionsView.ShowViewCaption = true;
            this.gridViewExpMestExported.ViewCaption = "Đã xuất";
            this.gridViewExpMestExported.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewExpMestExported_CustomUnboundColumnData);
            // 
            // gridColumn_Exported_Stt
            // 
            this.gridColumn_Exported_Stt.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn_Exported_Stt.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gridColumn_Exported_Stt.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_Exported_Stt.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_Exported_Stt.Caption = "STT";
            this.gridColumn_Exported_Stt.FieldName = "STT";
            this.gridColumn_Exported_Stt.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gridColumn_Exported_Stt.Name = "gridColumn_Exported_Stt";
            this.gridColumn_Exported_Stt.OptionsColumn.AllowEdit = false;
            this.gridColumn_Exported_Stt.OptionsColumn.ReadOnly = true;
            this.gridColumn_Exported_Stt.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn_Exported_Stt.Visible = true;
            this.gridColumn_Exported_Stt.VisibleIndex = 0;
            this.gridColumn_Exported_Stt.Width = 35;
            // 
            // gridColumn_Exported_ExpMestCode
            // 
            this.gridColumn_Exported_ExpMestCode.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn_Exported_ExpMestCode.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gridColumn_Exported_ExpMestCode.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_Exported_ExpMestCode.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_Exported_ExpMestCode.Caption = "Mã xuất";
            this.gridColumn_Exported_ExpMestCode.FieldName = "EXP_MEST_CODE";
            this.gridColumn_Exported_ExpMestCode.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gridColumn_Exported_ExpMestCode.Name = "gridColumn_Exported_ExpMestCode";
            this.gridColumn_Exported_ExpMestCode.OptionsColumn.ReadOnly = true;
            this.gridColumn_Exported_ExpMestCode.Visible = true;
            this.gridColumn_Exported_ExpMestCode.VisibleIndex = 1;
            this.gridColumn_Exported_ExpMestCode.Width = 90;
            // 
            // gridColumn_Exported_ServiceReqCode
            // 
            this.gridColumn_Exported_ServiceReqCode.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn_Exported_ServiceReqCode.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gridColumn_Exported_ServiceReqCode.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_Exported_ServiceReqCode.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_Exported_ServiceReqCode.Caption = "Mã y lệnh";
            this.gridColumn_Exported_ServiceReqCode.FieldName = "TDL_SERVICE_REQ_CODE";
            this.gridColumn_Exported_ServiceReqCode.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gridColumn_Exported_ServiceReqCode.Name = "gridColumn_Exported_ServiceReqCode";
            this.gridColumn_Exported_ServiceReqCode.OptionsColumn.ReadOnly = true;
            this.gridColumn_Exported_ServiceReqCode.Visible = true;
            this.gridColumn_Exported_ServiceReqCode.VisibleIndex = 2;
            this.gridColumn_Exported_ServiceReqCode.Width = 90;
            // 
            // gridColumn_Exported_TreatmentCode
            // 
            this.gridColumn_Exported_TreatmentCode.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn_Exported_TreatmentCode.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gridColumn_Exported_TreatmentCode.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_Exported_TreatmentCode.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_Exported_TreatmentCode.Caption = "Mã điều trị";
            this.gridColumn_Exported_TreatmentCode.FieldName = "TDL_TREATMENT_CODE";
            this.gridColumn_Exported_TreatmentCode.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gridColumn_Exported_TreatmentCode.Name = "gridColumn_Exported_TreatmentCode";
            this.gridColumn_Exported_TreatmentCode.OptionsColumn.ReadOnly = true;
            this.gridColumn_Exported_TreatmentCode.Visible = true;
            this.gridColumn_Exported_TreatmentCode.VisibleIndex = 3;
            this.gridColumn_Exported_TreatmentCode.Width = 90;
            // 
            // gridColumn_Exported_PatientName
            // 
            this.gridColumn_Exported_PatientName.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_Exported_PatientName.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_Exported_PatientName.Caption = "Tên bệnh nhân";
            this.gridColumn_Exported_PatientName.FieldName = "TDL_PATIENT_NAME";
            this.gridColumn_Exported_PatientName.Name = "gridColumn_Exported_PatientName";
            this.gridColumn_Exported_PatientName.OptionsColumn.ReadOnly = true;
            this.gridColumn_Exported_PatientName.Visible = true;
            this.gridColumn_Exported_PatientName.VisibleIndex = 4;
            this.gridColumn_Exported_PatientName.Width = 100;
            // 
            // gridColumn_Exported_GenderName
            // 
            this.gridColumn_Exported_GenderName.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_Exported_GenderName.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_Exported_GenderName.Caption = "Giới tính";
            this.gridColumn_Exported_GenderName.FieldName = "TDL_PATIENT_GENDER_NAME";
            this.gridColumn_Exported_GenderName.Name = "gridColumn_Exported_GenderName";
            this.gridColumn_Exported_GenderName.OptionsColumn.ReadOnly = true;
            this.gridColumn_Exported_GenderName.Visible = true;
            this.gridColumn_Exported_GenderName.VisibleIndex = 5;
            this.gridColumn_Exported_GenderName.Width = 50;
            // 
            // gridColumn_Exported_Dob
            // 
            this.gridColumn_Exported_Dob.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn_Exported_Dob.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_Exported_Dob.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_Exported_Dob.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_Exported_Dob.Caption = "Ngày sinh";
            this.gridColumn_Exported_Dob.FieldName = "DOB_DISPLAY";
            this.gridColumn_Exported_Dob.Name = "gridColumn_Exported_Dob";
            this.gridColumn_Exported_Dob.OptionsColumn.ReadOnly = true;
            this.gridColumn_Exported_Dob.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn_Exported_Dob.Visible = true;
            this.gridColumn_Exported_Dob.VisibleIndex = 6;
            // 
            // gridColumn_Exported_ExpTime
            // 
            this.gridColumn_Exported_ExpTime.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn_Exported_ExpTime.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_Exported_ExpTime.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_Exported_ExpTime.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_Exported_ExpTime.Caption = "Thời gian xuất";
            this.gridColumn_Exported_ExpTime.FieldName = "EXP_TIME_DISPLAY";
            this.gridColumn_Exported_ExpTime.Name = "gridColumn_Exported_ExpTime";
            this.gridColumn_Exported_ExpTime.OptionsColumn.ReadOnly = true;
            this.gridColumn_Exported_ExpTime.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn_Exported_ExpTime.Visible = true;
            this.gridColumn_Exported_ExpTime.VisibleIndex = 7;
            this.gridColumn_Exported_ExpTime.Width = 120;
            // 
            // gridColumn_Exported_CreateTime
            // 
            this.gridColumn_Exported_CreateTime.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn_Exported_CreateTime.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_Exported_CreateTime.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_Exported_CreateTime.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_Exported_CreateTime.Caption = "Thời gian tạo";
            this.gridColumn_Exported_CreateTime.FieldName = "CREATE_TIME_DISPLAY";
            this.gridColumn_Exported_CreateTime.Name = "gridColumn_Exported_CreateTime";
            this.gridColumn_Exported_CreateTime.OptionsColumn.ReadOnly = true;
            this.gridColumn_Exported_CreateTime.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn_Exported_CreateTime.Visible = true;
            this.gridColumn_Exported_CreateTime.VisibleIndex = 8;
            this.gridColumn_Exported_CreateTime.Width = 120;
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem8,
            this.layoutControlItem3});
            this.Root.Location = new System.Drawing.Point(0, 0);
            this.Root.Name = "Root";
            this.Root.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.Root.Size = new System.Drawing.Size(546, 476);
            this.Root.TextVisible = false;
            // 
            // layoutControlItem8
            // 
            this.layoutControlItem8.Control = this.gridControlExpMestExported;
            this.layoutControlItem8.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem8.Name = "layoutControlItem8";
            this.layoutControlItem8.Size = new System.Drawing.Size(546, 451);
            this.layoutControlItem8.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem8.TextVisible = false;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.ucPagingExported;
            this.layoutControlItem3.Location = new System.Drawing.Point(0, 451);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(546, 25);
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextVisible = false;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.layoutControlItem2,
            this.layoutControlItem4,
            this.layoutControlItem5,
            this.emptySpaceItem1,
            this.lciMediStock,
            this.lciApproveTime,
            this.emptySpaceItem2,
            this.layoutControlItem6,
            this.layoutControlItem10});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(1100, 532);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.layoutControl2;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 26);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(550, 480);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.layoutControl3;
            this.layoutControlItem2.Location = new System.Drawing.Point(550, 26);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(550, 480);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.Control = this.btnSearch;
            this.layoutControlItem4.Location = new System.Drawing.Point(918, 0);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Size = new System.Drawing.Size(66, 26);
            this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem4.TextVisible = false;
            // 
            // layoutControlItem5
            // 
            this.layoutControlItem5.Control = this.btnApprove;
            this.layoutControlItem5.Location = new System.Drawing.Point(990, 506);
            this.layoutControlItem5.Name = "layoutControlItem5";
            this.layoutControlItem5.Size = new System.Drawing.Size(110, 26);
            this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem5.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(0, 506);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(990, 26);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // lciMediStock
            // 
            this.lciMediStock.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciMediStock.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciMediStock.Control = this.lblMediStock;
            this.lciMediStock.Location = new System.Drawing.Point(0, 0);
            this.lciMediStock.Name = "lciMediStock";
            this.lciMediStock.Size = new System.Drawing.Size(283, 26);
            this.lciMediStock.Text = "Kho:";
            this.lciMediStock.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciMediStock.TextSize = new System.Drawing.Size(90, 20);
            this.lciMediStock.TextToControlDistance = 5;
            // 
            // lciApproveTime
            // 
            this.lciApproveTime.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.lciApproveTime.AppearanceItemCaption.Options.UseForeColor = true;
            this.lciApproveTime.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciApproveTime.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciApproveTime.Control = this.dtApproveTimeFrom;
            this.lciApproveTime.Location = new System.Drawing.Point(550, 0);
            this.lciApproveTime.Name = "lciApproveTime";
            this.lciApproveTime.Size = new System.Drawing.Size(228, 26);
            this.lciApproveTime.Text = "T/g Duyệt:";
            this.lciApproveTime.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciApproveTime.TextSize = new System.Drawing.Size(90, 20);
            this.lciApproveTime.TextToControlDistance = 5;
            // 
            // emptySpaceItem2
            // 
            this.emptySpaceItem2.AllowHotTrack = false;
            this.emptySpaceItem2.Location = new System.Drawing.Point(984, 0);
            this.emptySpaceItem2.Name = "emptySpaceItem2";
            this.emptySpaceItem2.Size = new System.Drawing.Size(116, 26);
            this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
            // 
            // layoutControlItem6
            // 
            this.layoutControlItem6.Control = this.dtApproveTimeTo;
            this.layoutControlItem6.Location = new System.Drawing.Point(778, 0);
            this.layoutControlItem6.Name = "layoutControlItem6";
            this.layoutControlItem6.Size = new System.Drawing.Size(140, 26);
            this.layoutControlItem6.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem6.TextVisible = false;
            // 
            // layoutControlItem10
            // 
            this.layoutControlItem10.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem10.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem10.Control = this.cboType;
            this.layoutControlItem10.Location = new System.Drawing.Point(283, 0);
            this.layoutControlItem10.Name = "layoutControlItem10";
            this.layoutControlItem10.Size = new System.Drawing.Size(267, 26);
            this.layoutControlItem10.Text = "Loại xuất:";
            this.layoutControlItem10.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem10.TextSize = new System.Drawing.Size(90, 20);
            this.layoutControlItem10.TextToControlDistance = 5;
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 29);
            this.barDockControlTop.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.barDockControlTop.Size = new System.Drawing.Size(1100, 0);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 561);
            this.barDockControlBottom.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.barDockControlBottom.Size = new System.Drawing.Size(1100, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 29);
            this.barDockControlLeft.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 532);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(1100, 29);
            this.barDockControlRight.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 532);
            // 
            // frmNotTakenApprove
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1100, 561);
            this.Controls.Add(this.layoutControl1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Name = "frmNotTakenApprove";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Duyệt đơn không lấy";
            this.Load += new System.EventHandler(this.frmNotTakenApprove_Load);
            this.Controls.SetChildIndex(this.barDockControlTop, 0);
            this.Controls.SetChildIndex(this.barDockControlBottom, 0);
            this.Controls.SetChildIndex(this.barDockControlRight, 0);
            this.Controls.SetChildIndex(this.barDockControlLeft, 0);
            this.Controls.SetChildIndex(this.layoutControl1, 0);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cboType.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtApproveTimeTo.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtApproveTimeTo.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtApproveTimeFrom.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtApproveTimeFrom.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl3)).EndInit();
            this.layoutControl3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControlExpMestUnexport)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewExpMestUnexport)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl2)).EndInit();
            this.layoutControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControlExpMestExported)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewExpMestExported)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciMediStock)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciApproveTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem10)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.LabelControl lblMediStock;
        private DevExpress.XtraEditors.SimpleButton btnApprove;
        private DevExpress.XtraEditors.SimpleButton btnSearch;
        private DevExpress.XtraLayout.LayoutControl layoutControl3;
        private DevExpress.XtraGrid.GridControl gridControlExpMestUnexport;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewExpMestUnexport;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem9;
        private DevExpress.XtraLayout.LayoutControl layoutControl2;
        private DevExpress.XtraGrid.GridControl gridControlExpMestExported;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewExpMestExported;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem8;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraLayout.LayoutControlItem lciMediStock;
        private DevExpress.XtraEditors.DateEdit dtApproveTimeFrom;
        private DevExpress.XtraLayout.LayoutControlItem lciApproveTime;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_Exported_Stt;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_Exported_ExpMestCode;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_Exported_ServiceReqCode;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_Exported_TreatmentCode;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_Exported_PatientName;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_Exported_GenderName;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_Exported_Dob;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_Exported_ExpTime;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_Exported_CreateTime;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_Unexport_stt;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_Unexport_ExpMestCode;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_Unexport_ServiceReqCode;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_Unexport_TreatmentCode;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_Unexport_PatientName;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_Unexport_GenderName;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_Unexport_Dob;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_Unexport_CreateTime;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_Unexport_Creator;
        private Inventec.UC.Paging.UcPaging ucPagingUnexport;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem7;
        private Inventec.UC.Paging.UcPaging ucPagingExported;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.Bar bar1;
        private DevExpress.XtraBars.BarButtonItem barBtnSearch;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraBars.BarButtonItem barBtnApprove;
        private DevExpress.XtraBars.BarButtonItem barBtnNew;
        private DevExpress.XtraEditors.DateEdit dtApproveTimeTo;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem6;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraEditors.GridLookUpEdit cboType;
        private DevExpress.XtraGrid.Views.Grid.GridView gridLookUpEdit1View;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem10;
    }
}
