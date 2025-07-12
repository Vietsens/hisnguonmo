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
using HIS.Desktop.LocalStorage.LocalData;
namespace HIS.Desktop.Plugins.DrugUsageAnalysis
{
    partial class UCDrugUsageAnalysis
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
            if (GlobalVariables.DicRefreshData != null && GlobalVariables.DicRefreshData.Count > 0 && GlobalVariables.DicRefreshData.ContainsKey(currentModule.RoomId.ToString()))
            {
                GlobalVariables.DicRefreshData.Remove(currentModule.RoomId.ToString());
            }
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UCDrugUsageAnalysis));
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject1 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject2 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject3 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject4 = new DevExpress.Utils.SerializableAppearanceObject();
            this.ucPaging = new Inventec.UC.Paging.UcPaging();
            this.gridControl1 = new DevExpress.XtraGrid.GridControl();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn5 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn6 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn7 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn13 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn8 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn9 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn11 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemDateEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
            this.gridColumn12 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.bbtnSearch = new DevExpress.XtraBars.BarButtonItem();
            this.bbtnRefresh = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.dtCreateTimeTo = new DevExpress.XtraEditors.DateEdit();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.treeListDateTime = new DevExpress.XtraTreeList.TreeList();
            this.treeListColumnSTT = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListColumn_DrugUsageAnalysisDetail = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeColDateTime = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListColumn2 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListColumn3 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListColumn4 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListColumn5 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListColumn7 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListColumn8 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.repositoryItemButtonEdit_DrugUsageAnalysisDetail = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.label1 = new System.Windows.Forms.Label();
            this.cboBedRoom = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridView3 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.cboDepartment = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridView2 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.cboPatientType = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridLookUpEdit2View = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.txtPatientCode = new DevExpress.XtraEditors.TextEdit();
            this.txtTreatmentCode = new DevExpress.XtraEditors.TextEdit();
            this.btnSearch = new DevExpress.XtraEditors.SimpleButton();
            this.txtKeyWord = new DevExpress.XtraEditors.TextEdit();
            this.dtCreateTimeFrom = new DevExpress.XtraEditors.DateEdit();
            this.btnRefresh = new DevExpress.XtraEditors.SimpleButton();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlGroup2 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlGroup3 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem14 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlGroup4 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem7 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem8 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem9 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem3 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.layoutControlGroup6 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem13 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem11 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem10 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem12 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemDateEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemDateEdit1.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtCreateTimeTo.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtCreateTimeTo.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.treeListDateTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemButtonEdit_DrugUsageAnalysisDetail)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboBedRoom.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboDepartment.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboPatientType.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit2View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPatientCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTreatmentCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtKeyWord.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtCreateTimeFrom.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtCreateTimeFrom.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem14)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem13)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem11)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem10)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem12)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).BeginInit();
            this.SuspendLayout();
            // 
            // ucPaging
            // 
            this.ucPaging.Location = new System.Drawing.Point(175, 560);
            this.ucPaging.Name = "ucPaging";
            this.ucPaging.Size = new System.Drawing.Size(614, 20);
            this.ucPaging.TabIndex = 5;
            // 
            // gridControl1
            // 
            this.gridControl1.Location = new System.Drawing.Point(175, 2);
            this.gridControl1.MainView = this.gridView1;
            this.gridControl1.MenuManager = this.barManager1;
            this.gridControl1.Name = "gridControl1";
            this.gridControl1.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemDateEdit1});
            this.gridControl1.Size = new System.Drawing.Size(614, 554);
            this.gridControl1.TabIndex = 4;
            this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
            // 
            // gridView1
            // 
            this.gridView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn1,
            this.gridColumn5,
            this.gridColumn6,
            this.gridColumn7,
            this.gridColumn13,
            this.gridColumn8,
            this.gridColumn9,
            this.gridColumn11,
            this.gridColumn12});
            this.gridView1.GridControl = this.gridControl1;
            this.gridView1.Name = "gridView1";
            this.gridView1.OptionsBehavior.AllowAddRows = DevExpress.Utils.DefaultBoolean.True;
            this.gridView1.OptionsBehavior.AllowDeleteRows = DevExpress.Utils.DefaultBoolean.False;
            this.gridView1.OptionsSelection.CheckBoxSelectorColumnWidth = 30;
            this.gridView1.OptionsView.ColumnAutoWidth = false;
            this.gridView1.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            this.gridView1.OptionsView.ShowGroupPanel = false;
            this.gridView1.OptionsView.ShowIndicator = false;
            this.gridView1.RowClick += new DevExpress.XtraGrid.Views.Grid.RowClickEventHandler(this.gridView1_RowClick);
            this.gridView1.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridView1_CustomUnboundColumnData);
            // 
            // gridColumn1
            // 
            this.gridColumn1.Caption = "STT";
            this.gridColumn1.FieldName = "STT";
            this.gridColumn1.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.OptionsColumn.AllowEdit = false;
            this.gridColumn1.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn1.Visible = true;
            this.gridColumn1.VisibleIndex = 0;
            this.gridColumn1.Width = 42;
            // 
            // gridColumn5
            // 
            this.gridColumn5.Caption = "Mã điều trị";
            this.gridColumn5.FieldName = "TREATMENT_CODE";
            this.gridColumn5.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gridColumn5.Name = "gridColumn5";
            this.gridColumn5.OptionsColumn.AllowEdit = false;
            this.gridColumn5.Visible = true;
            this.gridColumn5.VisibleIndex = 1;
            this.gridColumn5.Width = 117;
            // 
            // gridColumn6
            // 
            this.gridColumn6.Caption = "Mã bệnh nhân";
            this.gridColumn6.FieldName = "TDL_PATIENT_CODE";
            this.gridColumn6.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gridColumn6.Name = "gridColumn6";
            this.gridColumn6.OptionsColumn.AllowEdit = false;
            this.gridColumn6.Visible = true;
            this.gridColumn6.VisibleIndex = 2;
            this.gridColumn6.Width = 130;
            // 
            // gridColumn7
            // 
            this.gridColumn7.Caption = "Tên bệnh nhân";
            this.gridColumn7.FieldName = "TDL_PATIENT_NAME";
            this.gridColumn7.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gridColumn7.Name = "gridColumn7";
            this.gridColumn7.OptionsColumn.AllowEdit = false;
            this.gridColumn7.Visible = true;
            this.gridColumn7.VisibleIndex = 3;
            this.gridColumn7.Width = 175;
            // 
            // gridColumn13
            // 
            this.gridColumn13.Caption = "Diện điều trị ";
            this.gridColumn13.FieldName = "TDL_TREATMENT_TYPE_NAME";
            this.gridColumn13.Name = "gridColumn13";
            this.gridColumn13.OptionsColumn.AllowEdit = false;
            this.gridColumn13.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn13.Visible = true;
            this.gridColumn13.VisibleIndex = 4;
            this.gridColumn13.Width = 118;
            // 
            // gridColumn8
            // 
            this.gridColumn8.Caption = "Ngày sinh";
            this.gridColumn8.FieldName = "TDL_PATIENT_DOB_STR";
            this.gridColumn8.Name = "gridColumn8";
            this.gridColumn8.OptionsColumn.AllowEdit = false;
            this.gridColumn8.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn8.Visible = true;
            this.gridColumn8.VisibleIndex = 5;
            this.gridColumn8.Width = 111;
            // 
            // gridColumn9
            // 
            this.gridColumn9.Caption = "Giới tính";
            this.gridColumn9.FieldName = "TDL_PATIENT_GENDER_NAME";
            this.gridColumn9.Name = "gridColumn9";
            this.gridColumn9.OptionsColumn.AllowEdit = false;
            this.gridColumn9.Visible = true;
            this.gridColumn9.VisibleIndex = 6;
            this.gridColumn9.Width = 82;
            // 
            // gridColumn11
            // 
            this.gridColumn11.Caption = "Ngày vào viện";
            this.gridColumn11.ColumnEdit = this.repositoryItemDateEdit1;
            this.gridColumn11.FieldName = "IN_TIME_STR";
            this.gridColumn11.Name = "gridColumn11";
            this.gridColumn11.OptionsColumn.AllowEdit = false;
            this.gridColumn11.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn11.Visible = true;
            this.gridColumn11.VisibleIndex = 7;
            this.gridColumn11.Width = 130;
            // 
            // repositoryItemDateEdit1
            // 
            this.repositoryItemDateEdit1.AutoHeight = false;
            this.repositoryItemDateEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemDateEdit1.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemDateEdit1.DisplayFormat.FormatString = "dd/MM/yyyy HH:mm:ss";
            this.repositoryItemDateEdit1.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.repositoryItemDateEdit1.EditFormat.FormatString = "yyyyMMddHHmmss";
            this.repositoryItemDateEdit1.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.repositoryItemDateEdit1.Name = "repositoryItemDateEdit1";
            // 
            // gridColumn12
            // 
            this.gridColumn12.Caption = "Khoa";
            this.gridColumn12.FieldName = "LAST_DEPARTMENT_NAME";
            this.gridColumn12.MinWidth = 200;
            this.gridColumn12.Name = "gridColumn12";
            this.gridColumn12.OptionsColumn.AllowEdit = false;
            this.gridColumn12.Visible = true;
            this.gridColumn12.VisibleIndex = 8;
            this.gridColumn12.Width = 224;
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
            this.bbtnSearch,
            this.bbtnRefresh});
            this.barManager1.MaxItemId = 2;
            // 
            // bar1
            // 
            this.bar1.BarName = "Tools";
            this.bar1.DockCol = 0;
            this.bar1.DockRow = 0;
            this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.bbtnSearch),
            new DevExpress.XtraBars.LinkPersistInfo(this.bbtnRefresh)});
            this.bar1.Text = "Tools";
            this.bar1.Visible = false;
            // 
            // bbtnSearch
            // 
            this.bbtnSearch.Caption = "Tìm (Ctrl F)";
            this.bbtnSearch.Id = 0;
            this.bbtnSearch.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F));
            this.bbtnSearch.Name = "bbtnSearch";
            this.bbtnSearch.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bbtnSearch_ItemClick);
            // 
            // bbtnRefresh
            // 
            this.bbtnRefresh.Caption = "Làm lại (Ctrl R)";
            this.bbtnRefresh.Id = 1;
            this.bbtnRefresh.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R));
            this.bbtnRefresh.Name = "bbtnRefresh";
            this.bbtnRefresh.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bbtnRefresh_ItemClick);
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Size = new System.Drawing.Size(1416, 29);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 611);
            this.barDockControlBottom.Size = new System.Drawing.Size(1416, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 29);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 582);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(1416, 29);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 582);
            // 
            // dtCreateTimeTo
            // 
            this.dtCreateTimeTo.EditValue = null;
            this.dtCreateTimeTo.Location = new System.Drawing.Point(41, 248);
            this.dtCreateTimeTo.Name = "dtCreateTimeTo";
            this.dtCreateTimeTo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtCreateTimeTo.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtCreateTimeTo.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
            this.dtCreateTimeTo.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.dtCreateTimeTo.Properties.EditFormat.FormatString = "dd/MM/yyyy";
            this.dtCreateTimeTo.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.dtCreateTimeTo.Size = new System.Drawing.Size(130, 20);
            this.dtCreateTimeTo.StyleController = this.layoutControl1;
            this.dtCreateTimeTo.TabIndex = 5;
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.treeListDateTime);
            this.layoutControl1.Controls.Add(this.label1);
            this.layoutControl1.Controls.Add(this.cboBedRoom);
            this.layoutControl1.Controls.Add(this.cboDepartment);
            this.layoutControl1.Controls.Add(this.cboPatientType);
            this.layoutControl1.Controls.Add(this.gridControl1);
            this.layoutControl1.Controls.Add(this.ucPaging);
            this.layoutControl1.Controls.Add(this.txtPatientCode);
            this.layoutControl1.Controls.Add(this.dtCreateTimeTo);
            this.layoutControl1.Controls.Add(this.txtTreatmentCode);
            this.layoutControl1.Controls.Add(this.btnSearch);
            this.layoutControl1.Controls.Add(this.txtKeyWord);
            this.layoutControl1.Controls.Add(this.dtCreateTimeFrom);
            this.layoutControl1.Controls.Add(this.btnRefresh);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 29);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(1046, 230, 250, 350);
            this.layoutControl1.OptionsView.UseDefaultDragAndDropRendering = false;
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(1416, 582);
            this.layoutControl1.TabIndex = 18;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // treeListDateTime
            // 
            this.treeListDateTime.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] {
            this.treeListColumnSTT,
            this.treeListColumn_DrugUsageAnalysisDetail,
            this.treeColDateTime,
            this.treeListColumn2,
            this.treeListColumn3,
            this.treeListColumn4,
            this.treeListColumn5,
            this.treeListColumn7,
            this.treeListColumn8});
            this.treeListDateTime.Cursor = System.Windows.Forms.Cursors.Default;
            this.treeListDateTime.KeyFieldName = "KeyFieldName";
            this.treeListDateTime.Location = new System.Drawing.Point(793, 2);
            this.treeListDateTime.Name = "treeListDateTime";
            this.treeListDateTime.OptionsBehavior.PopulateServiceColumns = true;
            this.treeListDateTime.OptionsLayout.AddNewColumns = false;
            this.treeListDateTime.OptionsView.AutoWidth = false;
            this.treeListDateTime.OptionsView.ExpandButtonCentered = false;
            this.treeListDateTime.OptionsView.ShowBandsMode = DevExpress.Utils.DefaultBoolean.False;
            this.treeListDateTime.OptionsView.ShowIndicator = false;
            this.treeListDateTime.ParentFieldName = "ParentFieldName";
            this.treeListDateTime.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemButtonEdit_DrugUsageAnalysisDetail});
            this.treeListDateTime.Size = new System.Drawing.Size(621, 578);
            this.treeListDateTime.TabIndex = 21;
            this.treeListDateTime.UseDisabledStatePainter = false;
            this.treeListDateTime.CustomNodeCellEdit += new DevExpress.XtraTreeList.GetCustomNodeCellEditEventHandler(this.treeListDateTime_CustomNodeCellEdit);
            this.treeListDateTime.NodeCellStyle += new DevExpress.XtraTreeList.GetCustomNodeCellStyleEventHandler(this.treeListDateTime_NodeCellStyle);
            this.treeListDateTime.CustomUnboundColumnData += new DevExpress.XtraTreeList.CustomColumnDataEventHandler(this.treeListDateTime_CustomUnboundColumnData);
            // 
            // treeListColumnSTT
            // 
            this.treeListColumnSTT.AppearanceCell.Options.UseTextOptions = true;
            this.treeListColumnSTT.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.treeListColumnSTT.Caption = "STT";
            this.treeListColumnSTT.FieldName = "NUM_ORDER";
            this.treeListColumnSTT.Name = "treeListColumnSTT";
            this.treeListColumnSTT.OptionsColumn.AllowEdit = false;
            this.treeListColumnSTT.OptionsColumn.AllowSort = false;
            this.treeListColumnSTT.OptionsColumn.ReadOnly = true;
            this.treeListColumnSTT.UnboundType = DevExpress.XtraTreeList.Data.UnboundColumnType.Integer;
            this.treeListColumnSTT.Visible = true;
            this.treeListColumnSTT.VisibleIndex = 0;
            this.treeListColumnSTT.Width = 53;
            // 
            // treeListColumn_DrugUsageAnalysisDetail
            // 
            this.treeListColumn_DrugUsageAnalysisDetail.Caption = " ";
            this.treeListColumn_DrugUsageAnalysisDetail.FieldName = "DrugUsageAnalysisDetail";
            this.treeListColumn_DrugUsageAnalysisDetail.Name = "treeListColumn_DrugUsageAnalysisDetail";
            this.treeListColumn_DrugUsageAnalysisDetail.Visible = true;
            this.treeListColumn_DrugUsageAnalysisDetail.VisibleIndex = 1;
            this.treeListColumn_DrugUsageAnalysisDetail.Width = 20;
            // 
            // treeColDateTime
            // 
            this.treeColDateTime.Caption = "Thời gian";
            this.treeColDateTime.FieldName = "TRACKING_TIME_STR";
            this.treeColDateTime.Name = "treeColDateTime";
            this.treeColDateTime.OptionsColumn.AllowSort = false;
            this.treeColDateTime.OptionsColumn.ReadOnly = true;
            this.treeColDateTime.OptionsFilter.AllowFilter = false;
            this.treeColDateTime.UnboundType = DevExpress.XtraTreeList.Data.UnboundColumnType.Object;
            this.treeColDateTime.Visible = true;
            this.treeColDateTime.VisibleIndex = 2;
            this.treeColDateTime.Width = 116;
            // 
            // treeListColumn2
            // 
            this.treeListColumn2.Caption = "Số phiếu";
            this.treeListColumn2.FieldName = "SHEET_ORDER_STR";
            this.treeListColumn2.Name = "treeListColumn2";
            this.treeListColumn2.OptionsColumn.AllowSort = false;
            this.treeListColumn2.OptionsColumn.ReadOnly = true;
            this.treeListColumn2.UnboundType = DevExpress.XtraTreeList.Data.UnboundColumnType.Object;
            this.treeListColumn2.Visible = true;
            this.treeListColumn2.VisibleIndex = 3;
            this.treeListColumn2.Width = 64;
            // 
            // treeListColumn3
            // 
            this.treeListColumn3.Caption = "Trạng thái";
            this.treeListColumn3.FieldName = "ED_STT_NAME";
            this.treeListColumn3.Name = "treeListColumn3";
            this.treeListColumn3.OptionsColumn.AllowSort = false;
            this.treeListColumn3.OptionsColumn.ReadOnly = true;
            this.treeListColumn3.UnboundType = DevExpress.XtraTreeList.Data.UnboundColumnType.Object;
            this.treeListColumn3.Visible = true;
            this.treeListColumn3.VisibleIndex = 4;
            this.treeListColumn3.Width = 125;
            // 
            // treeListColumn4
            // 
            this.treeListColumn4.Caption = "Chẩn đoán chính";
            this.treeListColumn4.FieldName = "ICD_NAME_STR";
            this.treeListColumn4.Name = "treeListColumn4";
            this.treeListColumn4.OptionsColumn.AllowSort = false;
            this.treeListColumn4.OptionsColumn.ReadOnly = true;
            this.treeListColumn4.UnboundType = DevExpress.XtraTreeList.Data.UnboundColumnType.Object;
            this.treeListColumn4.Visible = true;
            this.treeListColumn4.VisibleIndex = 5;
            this.treeListColumn4.Width = 186;
            // 
            // treeListColumn5
            // 
            this.treeListColumn5.Caption = "Chẩn đoán phụ";
            this.treeListColumn5.FieldName = "ICD_TEXT_STR";
            this.treeListColumn5.Name = "treeListColumn5";
            this.treeListColumn5.OptionsColumn.AllowSort = false;
            this.treeListColumn5.OptionsColumn.ReadOnly = true;
            this.treeListColumn5.UnboundType = DevExpress.XtraTreeList.Data.UnboundColumnType.Object;
            this.treeListColumn5.Visible = true;
            this.treeListColumn5.VisibleIndex = 6;
            this.treeListColumn5.Width = 117;
            // 
            // treeListColumn7
            // 
            this.treeListColumn7.Caption = "Khoa";
            this.treeListColumn7.FieldName = "DEPARTMENT_NAME_STR";
            this.treeListColumn7.Name = "treeListColumn7";
            this.treeListColumn7.UnboundType = DevExpress.XtraTreeList.Data.UnboundColumnType.Object;
            this.treeListColumn7.Visible = true;
            this.treeListColumn7.VisibleIndex = 7;
            this.treeListColumn7.Width = 163;
            // 
            // treeListColumn8
            // 
            this.treeListColumn8.Caption = "Phòng";
            this.treeListColumn8.FieldName = "ROOM_NAME_STR";
            this.treeListColumn8.Name = "treeListColumn8";
            this.treeListColumn8.UnboundType = DevExpress.XtraTreeList.Data.UnboundColumnType.Object;
            this.treeListColumn8.Visible = true;
            this.treeListColumn8.VisibleIndex = 8;
            this.treeListColumn8.Width = 110;
            // 
            // repositoryItemButtonEdit_DrugUsageAnalysisDetail
            // 
            this.repositoryItemButtonEdit_DrugUsageAnalysisDetail.AutoHeight = false;
            serializableAppearanceObject1.BorderColor = System.Drawing.Color.Transparent;
            serializableAppearanceObject1.Options.UseBorderColor = true;
            this.repositoryItemButtonEdit_DrugUsageAnalysisDetail.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, ((System.Drawing.Image)(resources.GetObject("repositoryItemButtonEdit_DrugUsageAnalysisDetail.Buttons"))), new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject1, serializableAppearanceObject2, serializableAppearanceObject3, serializableAppearanceObject4, "Phân tích sử dụng thuốc", null, null, true)});
            this.repositoryItemButtonEdit_DrugUsageAnalysisDetail.Name = "repositoryItemButtonEdit_DrugUsageAnalysisDetail";
            this.repositoryItemButtonEdit_DrugUsageAnalysisDetail.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            this.repositoryItemButtonEdit_DrugUsageAnalysisDetail.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repositoryItemButtonEdit_DrugUsageAnalysisDetail_ButtonClick);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.label1.Location = new System.Drawing.Point(113, 208);
            this.label1.Margin = new System.Windows.Forms.Padding(0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 12);
            this.label1.TabIndex = 20;
            // 
            // cboBedRoom
            // 
            this.cboBedRoom.Location = new System.Drawing.Point(4, 136);
            this.cboBedRoom.MenuManager = this.barManager1;
            this.cboBedRoom.Name = "cboBedRoom";
            this.cboBedRoom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)});
            this.cboBedRoom.Properties.NullText = "";
            this.cboBedRoom.Properties.View = this.gridView3;
            this.cboBedRoom.Size = new System.Drawing.Size(165, 20);
            this.cboBedRoom.StyleController = this.layoutControl1;
            this.cboBedRoom.TabIndex = 19;
            // 
            // gridView3
            // 
            this.gridView3.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridView3.Name = "gridView3";
            this.gridView3.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridView3.OptionsView.ShowGroupPanel = false;
            // 
            // cboDepartment
            // 
            this.cboDepartment.Location = new System.Drawing.Point(4, 92);
            this.cboDepartment.MenuManager = this.barManager1;
            this.cboDepartment.Name = "cboDepartment";
            this.cboDepartment.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)});
            this.cboDepartment.Properties.NullText = "";
            this.cboDepartment.Properties.View = this.gridView2;
            this.cboDepartment.Size = new System.Drawing.Size(165, 20);
            this.cboDepartment.StyleController = this.layoutControl1;
            this.cboDepartment.TabIndex = 18;
            // 
            // gridView2
            // 
            this.gridView2.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridView2.Name = "gridView2";
            this.gridView2.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridView2.OptionsView.ShowGroupPanel = false;
            // 
            // cboPatientType
            // 
            this.cboPatientType.Location = new System.Drawing.Point(4, 180);
            this.cboPatientType.Name = "cboPatientType";
            this.cboPatientType.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.cboPatientType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)});
            this.cboPatientType.Properties.NullText = "";
            this.cboPatientType.Properties.View = this.gridLookUpEdit2View;
            this.cboPatientType.Size = new System.Drawing.Size(165, 20);
            this.cboPatientType.StyleController = this.layoutControl1;
            this.cboPatientType.TabIndex = 17;
            // 
            // gridLookUpEdit2View
            // 
            this.gridLookUpEdit2View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridLookUpEdit2View.Name = "gridLookUpEdit2View";
            this.gridLookUpEdit2View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridLookUpEdit2View.OptionsView.ShowGroupPanel = false;
            // 
            // txtPatientCode
            // 
            this.txtPatientCode.Location = new System.Drawing.Point(2, 2);
            this.txtPatientCode.MenuManager = this.barManager1;
            this.txtPatientCode.Name = "txtPatientCode";
            this.txtPatientCode.Properties.NullValuePrompt = "Mã bệnh nhân";
            this.txtPatientCode.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtPatientCode.Size = new System.Drawing.Size(169, 20);
            this.txtPatientCode.StyleController = this.layoutControl1;
            this.txtPatientCode.TabIndex = 11;
            this.txtPatientCode.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtPatientCode_PreviewKeyDown);
            // 
            // txtTreatmentCode
            // 
            this.txtTreatmentCode.Location = new System.Drawing.Point(2, 26);
            this.txtTreatmentCode.Name = "txtTreatmentCode";
            this.txtTreatmentCode.Properties.NullValuePrompt = "Mã điều trị";
            this.txtTreatmentCode.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtTreatmentCode.Size = new System.Drawing.Size(169, 20);
            this.txtTreatmentCode.StyleController = this.layoutControl1;
            this.txtTreatmentCode.TabIndex = 6;
            this.txtTreatmentCode.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtTreatmentCode_PreviewKeyDown);
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(4, 556);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(78, 22);
            this.btnSearch.StyleController = this.layoutControl1;
            this.btnSearch.TabIndex = 9;
            this.btnSearch.Text = "Tìm (Ctrl F)";
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            this.btnSearch.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.btnSearch_PreviewKeyDown);
            // 
            // txtKeyWord
            // 
            this.txtKeyWord.Location = new System.Drawing.Point(2, 50);
            this.txtKeyWord.Name = "txtKeyWord";
            this.txtKeyWord.Properties.NullValuePrompt = "Từ khóa tìm kiếm";
            this.txtKeyWord.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtKeyWord.Size = new System.Drawing.Size(169, 20);
            this.txtKeyWord.StyleController = this.layoutControl1;
            this.txtKeyWord.TabIndex = 4;
            this.txtKeyWord.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtKeyWord_PreviewKeyDown);
            // 
            // dtCreateTimeFrom
            // 
            this.dtCreateTimeFrom.EditValue = null;
            this.dtCreateTimeFrom.Location = new System.Drawing.Point(41, 224);
            this.dtCreateTimeFrom.Name = "dtCreateTimeFrom";
            this.dtCreateTimeFrom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtCreateTimeFrom.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtCreateTimeFrom.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
            this.dtCreateTimeFrom.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.dtCreateTimeFrom.Properties.EditFormat.FormatString = "dd/MM/yyyy";
            this.dtCreateTimeFrom.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.dtCreateTimeFrom.Size = new System.Drawing.Size(130, 20);
            this.dtCreateTimeFrom.StyleController = this.layoutControl1;
            this.dtCreateTimeFrom.TabIndex = 4;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(90, 556);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(79, 22);
            this.btnRefresh.StyleController = this.layoutControl1;
            this.btnRefresh.TabIndex = 10;
            this.btnRefresh.Text = "Làm lại (Ctrl R)";
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            this.btnRefresh.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.btnRefresh_PreviewKeyDown);
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlGroup2,
            this.layoutControlGroup3,
            this.layoutControlGroup4});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "Root";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(1416, 582);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlGroup2
            // 
            this.layoutControlGroup2.GroupBordersVisible = false;
            this.layoutControlGroup2.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem2,
            this.layoutControlItem1});
            this.layoutControlGroup2.Location = new System.Drawing.Point(173, 0);
            this.layoutControlGroup2.Name = "layoutControlGroup2";
            this.layoutControlGroup2.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup2.Size = new System.Drawing.Size(618, 582);
            this.layoutControlGroup2.Spacing = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup2.Text = "Danh sách";
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.ucPaging;
            this.layoutControlItem2.Location = new System.Drawing.Point(0, 558);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(618, 24);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.gridControl1;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(618, 558);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // layoutControlGroup3
            // 
            this.layoutControlGroup3.GroupBordersVisible = false;
            this.layoutControlGroup3.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem14});
            this.layoutControlGroup3.Location = new System.Drawing.Point(791, 0);
            this.layoutControlGroup3.Name = "layoutControlGroup3";
            this.layoutControlGroup3.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup3.Size = new System.Drawing.Size(625, 582);
            this.layoutControlGroup3.Spacing = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup3.Text = "Y lệnh";
            // 
            // layoutControlItem14
            // 
            this.layoutControlItem14.Control = this.treeListDateTime;
            this.layoutControlItem14.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem14.Name = "layoutControlItem14";
            this.layoutControlItem14.Size = new System.Drawing.Size(625, 582);
            this.layoutControlItem14.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem14.TextVisible = false;
            // 
            // layoutControlGroup4
            // 
            this.layoutControlGroup4.GroupBordersVisible = false;
            this.layoutControlGroup4.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem7,
            this.layoutControlItem8,
            this.layoutControlItem9,
            this.emptySpaceItem3,
            this.layoutControlGroup6,
            this.layoutControlItem13,
            this.layoutControlItem11,
            this.layoutControlItem10,
            this.layoutControlItem12,
            this.layoutControlItem5,
            this.layoutControlItem6});
            this.layoutControlGroup4.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup4.Name = "layoutControlGroup4";
            this.layoutControlGroup4.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup4.Size = new System.Drawing.Size(173, 582);
            this.layoutControlGroup4.Spacing = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup4.Text = "Tìm kiếm";
            // 
            // layoutControlItem7
            // 
            this.layoutControlItem7.Control = this.txtKeyWord;
            this.layoutControlItem7.Location = new System.Drawing.Point(0, 48);
            this.layoutControlItem7.Name = "layoutControlItem7";
            this.layoutControlItem7.Size = new System.Drawing.Size(173, 24);
            this.layoutControlItem7.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem7.TextVisible = false;
            // 
            // layoutControlItem8
            // 
            this.layoutControlItem8.Control = this.txtTreatmentCode;
            this.layoutControlItem8.Location = new System.Drawing.Point(0, 24);
            this.layoutControlItem8.Name = "layoutControlItem8";
            this.layoutControlItem8.Size = new System.Drawing.Size(173, 24);
            this.layoutControlItem8.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem8.TextVisible = false;
            // 
            // layoutControlItem9
            // 
            this.layoutControlItem9.Control = this.txtPatientCode;
            this.layoutControlItem9.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem9.Name = "layoutControlItem9";
            this.layoutControlItem9.Size = new System.Drawing.Size(173, 24);
            this.layoutControlItem9.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem9.TextVisible = false;
            // 
            // emptySpaceItem3
            // 
            this.emptySpaceItem3.AllowHotTrack = false;
            this.emptySpaceItem3.Location = new System.Drawing.Point(0, 272);
            this.emptySpaceItem3.Name = "emptySpaceItem3";
            this.emptySpaceItem3.Size = new System.Drawing.Size(173, 282);
            this.emptySpaceItem3.TextSize = new System.Drawing.Size(0, 0);
            // 
            // layoutControlGroup6
            // 
            this.layoutControlGroup6.GroupBordersVisible = false;
            this.layoutControlGroup6.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem4,
            this.layoutControlItem3});
            this.layoutControlGroup6.Location = new System.Drawing.Point(0, 554);
            this.layoutControlGroup6.Name = "layoutControlGroup6";
            this.layoutControlGroup6.Size = new System.Drawing.Size(173, 28);
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.Control = this.btnSearch;
            this.layoutControlItem4.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Size = new System.Drawing.Size(86, 28);
            this.layoutControlItem4.Spacing = new DevExpress.XtraLayout.Utils.Padding(2, 2, 0, 2);
            this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem4.TextVisible = false;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.btnRefresh;
            this.layoutControlItem3.Location = new System.Drawing.Point(86, 0);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(87, 28);
            this.layoutControlItem3.Spacing = new DevExpress.XtraLayout.Utils.Padding(2, 2, 0, 2);
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextVisible = false;
            // 
            // layoutControlItem13
            // 
            this.layoutControlItem13.Control = this.cboDepartment;
            this.layoutControlItem13.Location = new System.Drawing.Point(0, 72);
            this.layoutControlItem13.Name = "layoutControlItem13";
            this.layoutControlItem13.Size = new System.Drawing.Size(173, 44);
            this.layoutControlItem13.Spacing = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 2);
            this.layoutControlItem13.Text = "Khoa điều trị:";
            this.layoutControlItem13.TextLocation = DevExpress.Utils.Locations.Top;
            this.layoutControlItem13.TextSize = new System.Drawing.Size(106, 13);
            // 
            // layoutControlItem11
            // 
            this.layoutControlItem11.Control = this.cboBedRoom;
            this.layoutControlItem11.Location = new System.Drawing.Point(0, 116);
            this.layoutControlItem11.Name = "layoutControlItem11";
            this.layoutControlItem11.Size = new System.Drawing.Size(173, 44);
            this.layoutControlItem11.Spacing = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 2);
            this.layoutControlItem11.Text = "Buồng bệnh:";
            this.layoutControlItem11.TextLocation = DevExpress.Utils.Locations.Top;
            this.layoutControlItem11.TextSize = new System.Drawing.Size(106, 13);
            // 
            // layoutControlItem10
            // 
            this.layoutControlItem10.Control = this.cboPatientType;
            this.layoutControlItem10.Location = new System.Drawing.Point(0, 160);
            this.layoutControlItem10.Name = "layoutControlItem10";
            this.layoutControlItem10.Size = new System.Drawing.Size(173, 44);
            this.layoutControlItem10.Spacing = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 2);
            this.layoutControlItem10.Text = "Đối tượng bệnh nhân:";
            this.layoutControlItem10.TextLocation = DevExpress.Utils.Locations.Top;
            this.layoutControlItem10.TextSize = new System.Drawing.Size(106, 13);
            // 
            // layoutControlItem12
            // 
            this.layoutControlItem12.Control = this.label1;
            this.layoutControlItem12.Location = new System.Drawing.Point(0, 204);
            this.layoutControlItem12.MaxSize = new System.Drawing.Size(0, 20);
            this.layoutControlItem12.MinSize = new System.Drawing.Size(24, 20);
            this.layoutControlItem12.Name = "layoutControlItem12";
            this.layoutControlItem12.Size = new System.Drawing.Size(173, 20);
            this.layoutControlItem12.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem12.Spacing = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 2);
            this.layoutControlItem12.Text = "Thời gian vào viện:";
            this.layoutControlItem12.TextSize = new System.Drawing.Size(106, 13);
            // 
            // layoutControlItem5
            // 
            this.layoutControlItem5.Control = this.dtCreateTimeFrom;
            this.layoutControlItem5.Location = new System.Drawing.Point(0, 224);
            this.layoutControlItem5.MaxSize = new System.Drawing.Size(0, 24);
            this.layoutControlItem5.MinSize = new System.Drawing.Size(157, 24);
            this.layoutControlItem5.Name = "layoutControlItem5";
            this.layoutControlItem5.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 0, 2);
            this.layoutControlItem5.Size = new System.Drawing.Size(173, 24);
            this.layoutControlItem5.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem5.Spacing = new DevExpress.XtraLayout.Utils.Padding(6, 0, 0, 0);
            this.layoutControlItem5.Text = "Từ:";
            this.layoutControlItem5.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem5.TextSize = new System.Drawing.Size(28, 13);
            this.layoutControlItem5.TextToControlDistance = 5;
            // 
            // layoutControlItem6
            // 
            this.layoutControlItem6.Control = this.dtCreateTimeTo;
            this.layoutControlItem6.Location = new System.Drawing.Point(0, 248);
            this.layoutControlItem6.MaxSize = new System.Drawing.Size(0, 24);
            this.layoutControlItem6.MinSize = new System.Drawing.Size(157, 24);
            this.layoutControlItem6.Name = "layoutControlItem6";
            this.layoutControlItem6.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 0, 2);
            this.layoutControlItem6.Size = new System.Drawing.Size(173, 24);
            this.layoutControlItem6.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem6.Spacing = new DevExpress.XtraLayout.Utils.Padding(6, 0, 0, 0);
            this.layoutControlItem6.Text = "đến:";
            this.layoutControlItem6.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem6.TextSize = new System.Drawing.Size(28, 20);
            this.layoutControlItem6.TextToControlDistance = 5;
            // 
            // UCDrugUsageAnalysis
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layoutControl1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Name = "UCDrugUsageAnalysis";
            this.Size = new System.Drawing.Size(1416, 611);
            this.Load += new System.EventHandler(this.UCDrugUsageAnalysis_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemDateEdit1.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemDateEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtCreateTimeTo.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtCreateTimeTo.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.treeListDateTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemButtonEdit_DrugUsageAnalysisDetail)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboBedRoom.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboDepartment.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboPatientType.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit2View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPatientCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTreatmentCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtKeyWord.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtCreateTimeFrom.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtCreateTimeFrom.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem14)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem13)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem11)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem10)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem12)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private DevExpress.XtraEditors.TextEdit txtTreatmentCode;
        private DevExpress.XtraEditors.TextEdit txtKeyWord;
        private DevExpress.XtraEditors.DateEdit dtCreateTimeTo;
        private DevExpress.XtraEditors.DateEdit dtCreateTimeFrom;
        private DevExpress.XtraEditors.SimpleButton btnRefresh;
        private DevExpress.XtraEditors.SimpleButton btnSearch;
        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraBars.BarButtonItem bbtnSearch;
        private DevExpress.XtraBars.BarButtonItem bbtnRefresh;
        private DevExpress.XtraGrid.GridControl gridControl1;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn5;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn6;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn7;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn8;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn9;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn11;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn12;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn13;
        private Inventec.UC.Paging.UcPaging ucPaging;
        private DevExpress.XtraEditors.TextEdit txtPatientCode;
        private DevExpress.XtraBars.Bar bar1;
        private DevExpress.XtraEditors.GridLookUpEdit cboPatientType;
        private DevExpress.XtraGrid.Views.Grid.GridView gridLookUpEdit2View;
        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup3;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup4;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem6;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem7;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem8;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem9;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem10;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem3;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup6;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraEditors.GridLookUpEdit cboDepartment;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem13;
        private DevExpress.XtraEditors.GridLookUpEdit cboBedRoom;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView3;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem11;
        private System.Windows.Forms.Label label1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem12;
        private DevExpress.XtraEditors.Repository.RepositoryItemDateEdit repositoryItemDateEdit1;
        private DevExpress.XtraTreeList.TreeList treeListDateTime;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeColDateTime;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem14;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumnSTT;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit repositoryItemButtonEdit_DrugUsageAnalysisDetail;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn_DrugUsageAnalysisDetail;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn3;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn2;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn4;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn5;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn7;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn8;
    }
}
