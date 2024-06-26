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
namespace HIS.Desktop.Plugins.HisBloodPaty.HisBloodPaty
{
    partial class frmHisBloodPaty
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmHisBloodPaty));
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject1 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject2 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject3 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject4 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SuperToolTip superToolTip1 = new DevExpress.Utils.SuperToolTip();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject5 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject6 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject7 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject8 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject9 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject10 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject11 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject12 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject13 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject14 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject15 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject16 = new DevExpress.Utils.SerializableAppearanceObject();
            this.layoutControl4 = new DevExpress.XtraLayout.LayoutControl();
            this.layoutControl7 = new DevExpress.XtraLayout.LayoutControl();
            this.btnSearch = new DevExpress.XtraEditors.SimpleButton();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.gridControlFormList = new DevExpress.XtraGrid.GridControl();
            this.gridviewFormList = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.STT = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnLock = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnEdit = new DevExpress.XtraGrid.Columns.GridColumn();
            this.buttonEditDeleteEnable = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.grdColBloodId = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnBloodTypeName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grdColPatientTypeId = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grdColExpPrice = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grdColExpVatRatio = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grdColPriority = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grdColFromTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grdColToTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grdColTreatmentFromTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grdColTreatmentToTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grdColCreateTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grdColCreator = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grdColModifyTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grdColModifier = new DevExpress.XtraGrid.Columns.GridColumn();
            this.ButtonEditLock = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.ButtonEditUnlock = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.ButtonEditDeleteDisable = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.ucPaging = new Inventec.UC.Paging.UcPaging();
            this.txtKeyword = new DevExpress.XtraEditors.TextEdit();
            this.layoutControlGroup6 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem3 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.lcEditorInfo = new DevExpress.XtraLayout.LayoutControl();
            this.lkPatientTypeId = new DevExpress.XtraEditors.GridLookUpEdit();
            this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.bbtnSearch = new DevExpress.XtraBars.BarButtonItem();
            this.bbtnEdit = new DevExpress.XtraBars.BarButtonItem();
            this.bbtnAdd = new DevExpress.XtraBars.BarButtonItem();
            this.bbtnReset = new DevExpress.XtraBars.BarButtonItem();
            this.bbtnFocusDefault = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.gridView2 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.lkBloodId = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.lkBloodType = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridLookUpEdit1View = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.spExpPrice = new DevExpress.XtraEditors.SpinEdit();
            this.spExpVatRatio = new DevExpress.XtraEditors.SpinEdit();
            this.spPriority = new DevExpress.XtraEditors.SpinEdit();
            this.dtFromTime = new DevExpress.XtraEditors.DateEdit();
            this.dtToTime = new DevExpress.XtraEditors.DateEdit();
            this.dtTreatmentFromTime = new DevExpress.XtraEditors.DateEdit();
            this.dtTreatmentToTime = new DevExpress.XtraEditors.DateEdit();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            this.btnAdd = new DevExpress.XtraEditors.SimpleButton();
            this.btnEdit = new DevExpress.XtraEditors.SimpleButton();
            this.layoutControlGroup4 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciBtnEdit = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciBtnAdd = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciBtnRefresh = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciExpPrice = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciExpVatRatio = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciFromTime = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciToTime = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciTreatmentFromTime = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciTreatmentToTime = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciPriority = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciBloodType = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciBloodId = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciPatientTypeId = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlGroup3 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            this.dxValidationProviderEditorInfo = new DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider(this.components);
            this.dxErrorProvider = new DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider(this.components);
            this.toolTipControllerGrid = new DevExpress.Utils.ToolTipController(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl4)).BeginInit();
            this.layoutControl4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl7)).BeginInit();
            this.layoutControl7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlFormList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridviewFormList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.buttonEditDeleteEnable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ButtonEditLock)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ButtonEditUnlock)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ButtonEditDeleteDisable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtKeyword.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcEditorInfo)).BeginInit();
            this.lcEditorInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lkPatientTypeId.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lkBloodId.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lkBloodType.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spExpPrice.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spExpVatRatio.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spPriority.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtFromTime.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtFromTime.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtToTime.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtToTime.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtTreatmentFromTime.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtTreatmentFromTime.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtTreatmentToTime.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtTreatmentToTime.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnEdit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnAdd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnRefresh)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciExpPrice)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciExpVatRatio)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciFromTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciToTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTreatmentFromTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTreatmentToTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPriority)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBloodType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBloodId)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPatientTypeId)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProviderEditorInfo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxErrorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl4
            // 
            this.layoutControl4.Controls.Add(this.layoutControl7);
            this.layoutControl4.Controls.Add(this.lcEditorInfo);
            this.layoutControl4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl4.Location = new System.Drawing.Point(0, 29);
            this.layoutControl4.Name = "layoutControl4";
            this.layoutControl4.Root = this.layoutControlGroup3;
            this.layoutControl4.Size = new System.Drawing.Size(1236, 482);
            this.layoutControl4.TabIndex = 2;
            this.layoutControl4.Text = "layoutControl4";
            // 
            // layoutControl7
            // 
            this.layoutControl7.Controls.Add(this.btnSearch);
            this.layoutControl7.Controls.Add(this.panelControl1);
            this.layoutControl7.Controls.Add(this.txtKeyword);
            this.layoutControl7.Location = new System.Drawing.Point(2, 2);
            this.layoutControl7.Name = "layoutControl7";
            this.layoutControl7.Root = this.layoutControlGroup6;
            this.layoutControl7.Size = new System.Drawing.Size(900, 478);
            this.layoutControl7.TabIndex = 5;
            this.layoutControl7.Text = "layoutControl7";
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(390, 2);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(92, 22);
            this.btnSearch.StyleController = this.layoutControl7;
            this.btnSearch.TabIndex = 1;
            this.btnSearch.Text = "Tìm (Ctrl F)";
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.gridControlFormList);
            this.panelControl1.Controls.Add(this.ucPaging);
            this.panelControl1.Location = new System.Drawing.Point(2, 28);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(896, 448);
            this.panelControl1.TabIndex = 36;
            // 
            // gridControlFormList
            // 
            this.gridControlFormList.Cursor = System.Windows.Forms.Cursors.Default;
            this.gridControlFormList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControlFormList.EmbeddedNavigator.Buttons.Append.Visible = false;
            this.gridControlFormList.EmbeddedNavigator.Buttons.CancelEdit.Visible = false;
            this.gridControlFormList.EmbeddedNavigator.Buttons.Edit.Visible = false;
            this.gridControlFormList.EmbeddedNavigator.Buttons.EndEdit.Visible = false;
            this.gridControlFormList.EmbeddedNavigator.Buttons.Remove.Visible = false;
            this.gridControlFormList.EmbeddedNavigator.TextStringFormat = "Bản ghi {0} của {1}";
            this.gridControlFormList.Location = new System.Drawing.Point(2, 2);
            this.gridControlFormList.MainView = this.gridviewFormList;
            this.gridControlFormList.Name = "gridControlFormList";
            this.gridControlFormList.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.buttonEditDeleteEnable,
            this.ButtonEditLock,
            this.ButtonEditUnlock,
            this.ButtonEditDeleteDisable});
            this.gridControlFormList.Size = new System.Drawing.Size(892, 423);
            this.gridControlFormList.TabIndex = 8;
            this.gridControlFormList.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridviewFormList});
            this.gridControlFormList.Click += new System.EventHandler(this.gridControlFormList_Click);
            this.gridControlFormList.DoubleClick += new System.EventHandler(this.gridControlFormList_DoubleClick);
            // 
            // gridviewFormList
            // 
            this.gridviewFormList.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.STT,
            this.gridColumnLock,
            this.gridColumnEdit,
            this.grdColBloodId,
            this.gridColumnBloodTypeName,
            this.grdColPatientTypeId,
            this.grdColExpPrice,
            this.grdColExpVatRatio,
            this.grdColPriority,
            this.grdColFromTime,
            this.grdColToTime,
            this.grdColTreatmentFromTime,
            this.grdColTreatmentToTime,
            this.grdColCreateTime,
            this.grdColCreator,
            this.grdColModifyTime,
            this.grdColModifier});
            this.gridviewFormList.GridControl = this.gridControlFormList;
            this.gridviewFormList.Name = "gridviewFormList";
            this.gridviewFormList.OptionsBehavior.AllowAddRows = DevExpress.Utils.DefaultBoolean.False;
            this.gridviewFormList.OptionsBehavior.AllowDeleteRows = DevExpress.Utils.DefaultBoolean.True;
            this.gridviewFormList.OptionsFind.AllowFindPanel = false;
            this.gridviewFormList.OptionsView.ColumnAutoWidth = false;
            this.gridviewFormList.OptionsView.ShowGroupPanel = false;
            this.gridviewFormList.OptionsView.ShowIndicator = false;
            this.gridviewFormList.CustomRowCellEdit += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gridviewFormList_CustomRowCellEdit);
            this.gridviewFormList.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridviewFormList_CustomUnboundColumnData);
            this.gridviewFormList.KeyDown += new System.Windows.Forms.KeyEventHandler(this.gridviewFormList_KeyDown);
            // 
            // STT
            // 
            this.STT.AppearanceCell.Options.UseTextOptions = true;
            this.STT.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.STT.AppearanceHeader.Options.UseTextOptions = true;
            this.STT.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.STT.Caption = "STT";
            this.STT.FieldName = "STT";
            this.STT.ImageAlignment = System.Drawing.StringAlignment.Far;
            this.STT.Name = "STT";
            this.STT.OptionsColumn.AllowEdit = false;
            this.STT.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.STT.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.STT.Visible = true;
            this.STT.VisibleIndex = 0;
            this.STT.Width = 60;
            // 
            // gridColumnLock
            // 
            this.gridColumnLock.Caption = "Khóa";
            this.gridColumnLock.FieldName = "LOCK_ITEM";
            this.gridColumnLock.Name = "gridColumnLock";
            this.gridColumnLock.OptionsColumn.ShowCaption = false;
            this.gridColumnLock.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumnLock.Visible = true;
            this.gridColumnLock.VisibleIndex = 1;
            this.gridColumnLock.Width = 20;
            // 
            // gridColumnEdit
            // 
            this.gridColumnEdit.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumnEdit.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumnEdit.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumnEdit.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumnEdit.Caption = "Xóa";
            this.gridColumnEdit.ColumnEdit = this.buttonEditDeleteEnable;
            this.gridColumnEdit.FieldName = "gridColumnEdit";
            this.gridColumnEdit.ImageAlignment = System.Drawing.StringAlignment.Center;
            this.gridColumnEdit.Name = "gridColumnEdit";
            this.gridColumnEdit.OptionsColumn.ShowCaption = false;
            this.gridColumnEdit.ToolTip = "Sửa";
            this.gridColumnEdit.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumnEdit.Visible = true;
            this.gridColumnEdit.VisibleIndex = 2;
            this.gridColumnEdit.Width = 20;
            // 
            // buttonEditDeleteEnable
            // 
            this.buttonEditDeleteEnable.AutoHeight = false;
            this.buttonEditDeleteEnable.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, ((System.Drawing.Image)(resources.GetObject("buttonEditDeleteEnable.Buttons"))), new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject1, serializableAppearanceObject2, serializableAppearanceObject3, serializableAppearanceObject4, "Xóa", null, superToolTip1, true)});
            this.buttonEditDeleteEnable.Name = "buttonEditDeleteEnable";
            this.buttonEditDeleteEnable.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            this.buttonEditDeleteEnable.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.btnGDelete_ButtonClick);
            // 
            // grdColBloodId
            // 
            this.grdColBloodId.Caption = "Mã loại máu";
            this.grdColBloodId.FieldName = "BLOOD_TYPE_CODE";
            this.grdColBloodId.Name = "grdColBloodId";
            this.grdColBloodId.OptionsColumn.AllowEdit = false;
            this.grdColBloodId.ToolTip = "Mã máu";
            this.grdColBloodId.Visible = true;
            this.grdColBloodId.VisibleIndex = 3;
            this.grdColBloodId.Width = 80;
            // 
            // gridColumnBloodTypeName
            // 
            this.gridColumnBloodTypeName.Caption = "Tên loại máu";
            this.gridColumnBloodTypeName.FieldName = "BLOOD_TYPE_NAME";
            this.gridColumnBloodTypeName.Name = "gridColumnBloodTypeName";
            this.gridColumnBloodTypeName.OptionsColumn.AllowEdit = false;
            this.gridColumnBloodTypeName.Visible = true;
            this.gridColumnBloodTypeName.VisibleIndex = 4;
            this.gridColumnBloodTypeName.Width = 150;
            // 
            // grdColPatientTypeId
            // 
            this.grdColPatientTypeId.Caption = "Đối tượng BN";
            this.grdColPatientTypeId.FieldName = "PATIENT_TYPE_NAME";
            this.grdColPatientTypeId.Name = "grdColPatientTypeId";
            this.grdColPatientTypeId.OptionsColumn.AllowEdit = false;
            this.grdColPatientTypeId.ToolTip = "Đối tượng BN";
            this.grdColPatientTypeId.Visible = true;
            this.grdColPatientTypeId.VisibleIndex = 5;
            this.grdColPatientTypeId.Width = 100;
            // 
            // grdColExpPrice
            // 
            this.grdColExpPrice.AppearanceCell.Options.UseTextOptions = true;
            this.grdColExpPrice.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.grdColExpPrice.AppearanceHeader.Options.UseTextOptions = true;
            this.grdColExpPrice.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.grdColExpPrice.Caption = "Giá xuất";
            this.grdColExpPrice.DisplayFormat.FormatString = "#,##0.0000";
            this.grdColExpPrice.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.grdColExpPrice.FieldName = "EXP_PRICE";
            this.grdColExpPrice.Name = "grdColExpPrice";
            this.grdColExpPrice.OptionsColumn.AllowEdit = false;
            this.grdColExpPrice.ToolTip = "Giá xuất";
            this.grdColExpPrice.Visible = true;
            this.grdColExpPrice.VisibleIndex = 6;
            this.grdColExpPrice.Width = 80;
            // 
            // grdColExpVatRatio
            // 
            this.grdColExpVatRatio.Caption = "Chiết khấu (%)";
            this.grdColExpVatRatio.FieldName = "EXP_VAT_RATIO_STR";
            this.grdColExpVatRatio.Name = "grdColExpVatRatio";
            this.grdColExpVatRatio.OptionsColumn.AllowEdit = false;
            this.grdColExpVatRatio.ToolTip = "EXP_VAT_RATIO_DISPLAY";
            this.grdColExpVatRatio.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.grdColExpVatRatio.Visible = true;
            this.grdColExpVatRatio.VisibleIndex = 7;
            this.grdColExpVatRatio.Width = 90;
            // 
            // grdColPriority
            // 
            this.grdColPriority.Caption = "Độ ưu tiên";
            this.grdColPriority.FieldName = "PRIORITY";
            this.grdColPriority.Name = "grdColPriority";
            this.grdColPriority.OptionsColumn.AllowEdit = false;
            this.grdColPriority.ToolTip = "PRIORITY";
            this.grdColPriority.Visible = true;
            this.grdColPriority.VisibleIndex = 8;
            this.grdColPriority.Width = 70;
            // 
            // grdColFromTime
            // 
            this.grdColFromTime.AppearanceCell.Options.UseTextOptions = true;
            this.grdColFromTime.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.grdColFromTime.AppearanceHeader.Options.UseTextOptions = true;
            this.grdColFromTime.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.grdColFromTime.Caption = "Thời gian từ";
            this.grdColFromTime.FieldName = "FROM_TIME_STR";
            this.grdColFromTime.Name = "grdColFromTime";
            this.grdColFromTime.OptionsColumn.AllowEdit = false;
            this.grdColFromTime.ToolTip = "FROM_TIME";
            this.grdColFromTime.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.grdColFromTime.Visible = true;
            this.grdColFromTime.VisibleIndex = 9;
            this.grdColFromTime.Width = 120;
            // 
            // grdColToTime
            // 
            this.grdColToTime.AppearanceCell.Options.UseTextOptions = true;
            this.grdColToTime.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.grdColToTime.AppearanceHeader.Options.UseTextOptions = true;
            this.grdColToTime.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.grdColToTime.Caption = "Thời gian đến";
            this.grdColToTime.FieldName = "TO_TIME_STR";
            this.grdColToTime.Name = "grdColToTime";
            this.grdColToTime.OptionsColumn.AllowEdit = false;
            this.grdColToTime.ToolTip = "TO_TIME";
            this.grdColToTime.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.grdColToTime.Visible = true;
            this.grdColToTime.VisibleIndex = 10;
            this.grdColToTime.Width = 120;
            // 
            // grdColTreatmentFromTime
            // 
            this.grdColTreatmentFromTime.AppearanceCell.Options.UseTextOptions = true;
            this.grdColTreatmentFromTime.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.grdColTreatmentFromTime.AppearanceHeader.Options.UseTextOptions = true;
            this.grdColTreatmentFromTime.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.grdColTreatmentFromTime.Caption = "Điều trị từ";
            this.grdColTreatmentFromTime.FieldName = "TREATMENT_FROM_TIME_STR";
            this.grdColTreatmentFromTime.Name = "grdColTreatmentFromTime";
            this.grdColTreatmentFromTime.OptionsColumn.AllowEdit = false;
            this.grdColTreatmentFromTime.ToolTip = "Điều trị từ";
            this.grdColTreatmentFromTime.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.grdColTreatmentFromTime.Visible = true;
            this.grdColTreatmentFromTime.VisibleIndex = 11;
            this.grdColTreatmentFromTime.Width = 120;
            // 
            // grdColTreatmentToTime
            // 
            this.grdColTreatmentToTime.AppearanceCell.Options.UseTextOptions = true;
            this.grdColTreatmentToTime.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.grdColTreatmentToTime.AppearanceHeader.Options.UseTextOptions = true;
            this.grdColTreatmentToTime.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.grdColTreatmentToTime.Caption = "Điều trị đến";
            this.grdColTreatmentToTime.FieldName = "TREATMENT_TO_TIME_STR";
            this.grdColTreatmentToTime.Name = "grdColTreatmentToTime";
            this.grdColTreatmentToTime.OptionsColumn.AllowEdit = false;
            this.grdColTreatmentToTime.ToolTip = "Điều trị đến";
            this.grdColTreatmentToTime.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.grdColTreatmentToTime.Visible = true;
            this.grdColTreatmentToTime.VisibleIndex = 12;
            this.grdColTreatmentToTime.Width = 120;
            // 
            // grdColCreateTime
            // 
            this.grdColCreateTime.AppearanceCell.Options.UseTextOptions = true;
            this.grdColCreateTime.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.grdColCreateTime.AppearanceHeader.Options.UseTextOptions = true;
            this.grdColCreateTime.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.grdColCreateTime.Caption = "Thời gian tạo";
            this.grdColCreateTime.FieldName = "CREATE_TIME_STR";
            this.grdColCreateTime.Name = "grdColCreateTime";
            this.grdColCreateTime.OptionsColumn.AllowEdit = false;
            this.grdColCreateTime.ToolTip = "CREATE_TIME";
            this.grdColCreateTime.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.grdColCreateTime.Visible = true;
            this.grdColCreateTime.VisibleIndex = 13;
            this.grdColCreateTime.Width = 120;
            // 
            // grdColCreator
            // 
            this.grdColCreator.Caption = "Người tạo";
            this.grdColCreator.FieldName = "CREATOR";
            this.grdColCreator.Name = "grdColCreator";
            this.grdColCreator.OptionsColumn.AllowEdit = false;
            this.grdColCreator.ToolTip = "CREATOR";
            this.grdColCreator.Visible = true;
            this.grdColCreator.VisibleIndex = 14;
            this.grdColCreator.Width = 100;
            // 
            // grdColModifyTime
            // 
            this.grdColModifyTime.AppearanceCell.Options.UseTextOptions = true;
            this.grdColModifyTime.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.grdColModifyTime.AppearanceHeader.Options.UseTextOptions = true;
            this.grdColModifyTime.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.grdColModifyTime.Caption = "Thời gian sửa";
            this.grdColModifyTime.FieldName = "MODIFY_TIME_STR";
            this.grdColModifyTime.Name = "grdColModifyTime";
            this.grdColModifyTime.OptionsColumn.AllowEdit = false;
            this.grdColModifyTime.ToolTip = "MODIFY_TIME";
            this.grdColModifyTime.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.grdColModifyTime.Visible = true;
            this.grdColModifyTime.VisibleIndex = 15;
            this.grdColModifyTime.Width = 120;
            // 
            // grdColModifier
            // 
            this.grdColModifier.Caption = "Người sửa";
            this.grdColModifier.FieldName = "MODIFIER";
            this.grdColModifier.Name = "grdColModifier";
            this.grdColModifier.OptionsColumn.AllowEdit = false;
            this.grdColModifier.ToolTip = "MODIFIER";
            this.grdColModifier.Visible = true;
            this.grdColModifier.VisibleIndex = 16;
            this.grdColModifier.Width = 100;
            // 
            // ButtonEditLock
            // 
            this.ButtonEditLock.AutoHeight = false;
            this.ButtonEditLock.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, ((System.Drawing.Image)(resources.GetObject("ButtonEditLock.Buttons"))), new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject5, serializableAppearanceObject6, serializableAppearanceObject7, serializableAppearanceObject8, "Mở khóa", null, null, true)});
            this.ButtonEditLock.Name = "ButtonEditLock";
            this.ButtonEditLock.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            this.ButtonEditLock.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.ButtonEditLock_ButtonClick);
            // 
            // ButtonEditUnlock
            // 
            this.ButtonEditUnlock.AutoHeight = false;
            this.ButtonEditUnlock.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, ((System.Drawing.Image)(resources.GetObject("ButtonEditUnlock.Buttons"))), new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject9, serializableAppearanceObject10, serializableAppearanceObject11, serializableAppearanceObject12, "Khóa", null, null, true)});
            this.ButtonEditUnlock.Name = "ButtonEditUnlock";
            this.ButtonEditUnlock.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            this.ButtonEditUnlock.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.ButtonEditUnlock_ButtonClick);
            // 
            // ButtonEditDeleteDisable
            // 
            this.ButtonEditDeleteDisable.AutoHeight = false;
            this.ButtonEditDeleteDisable.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, false, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, ((System.Drawing.Image)(resources.GetObject("ButtonEditDeleteDisable.Buttons"))), new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject13, serializableAppearanceObject14, serializableAppearanceObject15, serializableAppearanceObject16, "Xóa", null, null, true)});
            this.ButtonEditDeleteDisable.Name = "ButtonEditDeleteDisable";
            this.ButtonEditDeleteDisable.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            // 
            // ucPaging
            // 
            this.ucPaging.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ucPaging.Location = new System.Drawing.Point(2, 425);
            this.ucPaging.Name = "ucPaging";
            this.ucPaging.Size = new System.Drawing.Size(892, 21);
            this.ucPaging.TabIndex = 9;
            // 
            // txtKeyword
            // 
            this.txtKeyword.Location = new System.Drawing.Point(2, 2);
            this.txtKeyword.Name = "txtKeyword";
            this.txtKeyword.Properties.NullValuePrompt = "Từ khóa tìm kiếm";
            this.txtKeyword.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtKeyword.Properties.ShowNullValuePromptWhenFocused = true;
            this.txtKeyword.Size = new System.Drawing.Size(384, 20);
            this.txtKeyword.StyleController = this.layoutControl7;
            this.txtKeyword.TabIndex = 0;
            this.txtKeyword.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtKeyword_KeyUp);
            // 
            // layoutControlGroup6
            // 
            this.layoutControlGroup6.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup6.GroupBordersVisible = false;
            this.layoutControlGroup6.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem2,
            this.layoutControlItem1,
            this.layoutControlItem3,
            this.emptySpaceItem3});
            this.layoutControlGroup6.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup6.Name = "layoutControlGroup6";
            this.layoutControlGroup6.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup6.Size = new System.Drawing.Size(900, 478);
            this.layoutControlGroup6.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.txtKeyword;
            this.layoutControlItem2.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(388, 26);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.panelControl1;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 26);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(900, 452);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.btnSearch;
            this.layoutControlItem3.Location = new System.Drawing.Point(388, 0);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(96, 26);
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextVisible = false;
            // 
            // emptySpaceItem3
            // 
            this.emptySpaceItem3.AllowHotTrack = false;
            this.emptySpaceItem3.Location = new System.Drawing.Point(484, 0);
            this.emptySpaceItem3.Name = "emptySpaceItem3";
            this.emptySpaceItem3.Size = new System.Drawing.Size(416, 26);
            this.emptySpaceItem3.TextSize = new System.Drawing.Size(0, 0);
            // 
            // lcEditorInfo
            // 
            this.lcEditorInfo.Controls.Add(this.lkPatientTypeId);
            this.lcEditorInfo.Controls.Add(this.lkBloodId);
            this.lcEditorInfo.Controls.Add(this.lkBloodType);
            this.lcEditorInfo.Controls.Add(this.spExpPrice);
            this.lcEditorInfo.Controls.Add(this.spExpVatRatio);
            this.lcEditorInfo.Controls.Add(this.spPriority);
            this.lcEditorInfo.Controls.Add(this.dtFromTime);
            this.lcEditorInfo.Controls.Add(this.dtToTime);
            this.lcEditorInfo.Controls.Add(this.dtTreatmentFromTime);
            this.lcEditorInfo.Controls.Add(this.dtTreatmentToTime);
            this.lcEditorInfo.Controls.Add(this.btnCancel);
            this.lcEditorInfo.Controls.Add(this.btnAdd);
            this.lcEditorInfo.Controls.Add(this.btnEdit);
            this.lcEditorInfo.Location = new System.Drawing.Point(906, 2);
            this.lcEditorInfo.Name = "lcEditorInfo";
            this.lcEditorInfo.OptionsFocus.EnableAutoTabOrder = false;
            this.lcEditorInfo.Root = this.layoutControlGroup4;
            this.lcEditorInfo.Size = new System.Drawing.Size(328, 478);
            this.lcEditorInfo.TabIndex = 4;
            this.lcEditorInfo.Text = "layoutControl5";
            // 
            // lkPatientTypeId
            // 
            this.lkPatientTypeId.Location = new System.Drawing.Point(97, 50);
            this.lkPatientTypeId.MenuManager = this.barManager1;
            this.lkPatientTypeId.Name = "lkPatientTypeId";
            this.lkPatientTypeId.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.lkPatientTypeId.Properties.NullText = "";
            this.lkPatientTypeId.Properties.View = this.gridView2;
            this.lkPatientTypeId.Size = new System.Drawing.Size(229, 20);
            this.lkPatientTypeId.StyleController = this.lcEditorInfo;
            this.lkPatientTypeId.TabIndex = 17;
            this.lkPatientTypeId.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.lkPatientTypeId_Closed);
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
            this.bbtnEdit,
            this.bbtnAdd,
            this.bbtnReset,
            this.bbtnFocusDefault});
            this.barManager1.MaxItemId = 5;
            // 
            // bar1
            // 
            this.bar1.BarName = "Tools";
            this.bar1.DockCol = 0;
            this.bar1.DockRow = 0;
            this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.bbtnSearch),
            new DevExpress.XtraBars.LinkPersistInfo(this.bbtnEdit),
            new DevExpress.XtraBars.LinkPersistInfo(this.bbtnAdd),
            new DevExpress.XtraBars.LinkPersistInfo(this.bbtnReset),
            new DevExpress.XtraBars.LinkPersistInfo(this.bbtnFocusDefault)});
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
            // bbtnEdit
            // 
            this.bbtnEdit.Caption = "Sửa (Ctrl S)";
            this.bbtnEdit.Id = 1;
            this.bbtnEdit.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S));
            this.bbtnEdit.Name = "bbtnEdit";
            this.bbtnEdit.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bbtnEdit_ItemClick);
            // 
            // bbtnAdd
            // 
            this.bbtnAdd.Caption = "Thêm (Ctrl N)";
            this.bbtnAdd.Id = 2;
            this.bbtnAdd.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N));
            this.bbtnAdd.Name = "bbtnAdd";
            this.bbtnAdd.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bbtnAdd_ItemClick);
            // 
            // bbtnReset
            // 
            this.bbtnReset.Caption = "Làm lại (Ctrl C)";
            this.bbtnReset.Id = 3;
            this.bbtnReset.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C));
            this.bbtnReset.Name = "bbtnReset";
            this.bbtnReset.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bbtnReset_ItemClick);
            // 
            // bbtnFocusDefault
            // 
            this.bbtnFocusDefault.Caption = "F2";
            this.bbtnFocusDefault.Id = 4;
            this.bbtnFocusDefault.ItemShortcut = new DevExpress.XtraBars.BarShortcut(System.Windows.Forms.Keys.F2);
            this.bbtnFocusDefault.Name = "bbtnFocusDefault";
            this.bbtnFocusDefault.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bbtnFocusDefault_ItemClick);
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Size = new System.Drawing.Size(1236, 29);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 511);
            this.barDockControlBottom.Size = new System.Drawing.Size(1236, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 29);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 482);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(1236, 29);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 482);
            // 
            // gridView2
            // 
            this.gridView2.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridView2.Name = "gridView2";
            this.gridView2.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridView2.OptionsView.ShowGroupPanel = false;
            // 
            // lkBloodId
            // 
            this.lkBloodId.Location = new System.Drawing.Point(97, 26);
            this.lkBloodId.MenuManager = this.barManager1;
            this.lkBloodId.Name = "lkBloodId";
            this.lkBloodId.Properties.AutoComplete = false;
            this.lkBloodId.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.lkBloodId.Properties.NullText = "";
            this.lkBloodId.Properties.View = this.gridView1;
            this.lkBloodId.Size = new System.Drawing.Size(229, 20);
            this.lkBloodId.StyleController = this.lcEditorInfo;
            this.lkBloodId.TabIndex = 16;
            this.lkBloodId.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.lkBloodId_Closed);
            this.lkBloodId.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.lkBloodId_PreviewKeyDown);
            // 
            // gridView1
            // 
            this.gridView1.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridView1.Name = "gridView1";
            this.gridView1.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridView1.OptionsView.ShowGroupPanel = false;
            // 
            // lkBloodType
            // 
            this.lkBloodType.Location = new System.Drawing.Point(97, 2);
            this.lkBloodType.MenuManager = this.barManager1;
            this.lkBloodType.Name = "lkBloodType";
            this.lkBloodType.Properties.AutoComplete = false;
            this.lkBloodType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.lkBloodType.Properties.NullText = "";
            this.lkBloodType.Properties.View = this.gridLookUpEdit1View;
            this.lkBloodType.Size = new System.Drawing.Size(229, 20);
            this.lkBloodType.StyleController = this.lcEditorInfo;
            this.lkBloodType.TabIndex = 15;
            this.lkBloodType.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.lkBloodType_Closed);
            this.lkBloodType.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.lkBloodType_PreviewKeyDown);
            // 
            // gridLookUpEdit1View
            // 
            this.gridLookUpEdit1View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridLookUpEdit1View.Name = "gridLookUpEdit1View";
            this.gridLookUpEdit1View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridLookUpEdit1View.OptionsView.ShowGroupPanel = false;
            // 
            // spExpPrice
            // 
            this.spExpPrice.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.spExpPrice.Location = new System.Drawing.Point(97, 74);
            this.spExpPrice.Name = "spExpPrice";
            this.spExpPrice.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.spExpPrice.Properties.DisplayFormat.FormatString = "#,##0.0000";
            this.spExpPrice.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.spExpPrice.Properties.EditFormat.FormatString = "#,##0.0000";
            this.spExpPrice.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.spExpPrice.Size = new System.Drawing.Size(229, 20);
            this.spExpPrice.StyleController = this.lcEditorInfo;
            this.spExpPrice.TabIndex = 5;
            this.spExpPrice.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.spExpPrice_PreviewKeyDown);
            // 
            // spExpVatRatio
            // 
            this.spExpVatRatio.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.spExpVatRatio.Location = new System.Drawing.Point(97, 98);
            this.spExpVatRatio.Name = "spExpVatRatio";
            this.spExpVatRatio.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.spExpVatRatio.Size = new System.Drawing.Size(229, 20);
            this.spExpVatRatio.StyleController = this.lcEditorInfo;
            this.spExpVatRatio.TabIndex = 6;
            this.spExpVatRatio.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.spExpVatRatio_PreviewKeyDown);
            // 
            // spPriority
            // 
            this.spPriority.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.spPriority.Location = new System.Drawing.Point(97, 218);
            this.spPriority.Name = "spPriority";
            this.spPriority.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.spPriority.Size = new System.Drawing.Size(229, 20);
            this.spPriority.StyleController = this.lcEditorInfo;
            this.spPriority.TabIndex = 11;
            this.spPriority.KeyDown += new System.Windows.Forms.KeyEventHandler(this.spPriority_KeyDown);
            // 
            // dtFromTime
            // 
            this.dtFromTime.EditValue = null;
            this.dtFromTime.Location = new System.Drawing.Point(97, 122);
            this.dtFromTime.Name = "dtFromTime";
            this.dtFromTime.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtFromTime.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtFromTime.Properties.DisplayFormat.FormatString = "dd/MM/yyyy HH:mm";
            this.dtFromTime.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.dtFromTime.Properties.EditFormat.FormatString = "dd/MM/yyyy HH:mm";
            this.dtFromTime.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.dtFromTime.Properties.Mask.EditMask = "dd/MM/yyyy HH:mm";
            this.dtFromTime.Properties.Mask.UseMaskAsDisplayFormat = true;
            this.dtFromTime.Size = new System.Drawing.Size(229, 20);
            this.dtFromTime.StyleController = this.lcEditorInfo;
            this.dtFromTime.TabIndex = 7;
            this.dtFromTime.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.dtFromTime_Closed);
            // 
            // dtToTime
            // 
            this.dtToTime.EditValue = null;
            this.dtToTime.Location = new System.Drawing.Point(97, 146);
            this.dtToTime.Name = "dtToTime";
            this.dtToTime.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtToTime.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtToTime.Properties.DisplayFormat.FormatString = "dd/MM/yyyy HH:mm";
            this.dtToTime.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.dtToTime.Properties.EditFormat.FormatString = "dd/MM/yyyy HH:mm";
            this.dtToTime.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.dtToTime.Properties.Mask.EditMask = "dd/MM/yyyy HH:mm";
            this.dtToTime.Properties.Mask.UseMaskAsDisplayFormat = true;
            this.dtToTime.Properties.ShowNullValuePromptWhenFocused = true;
            this.dtToTime.Size = new System.Drawing.Size(229, 20);
            this.dtToTime.StyleController = this.lcEditorInfo;
            this.dtToTime.TabIndex = 8;
            this.dtToTime.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.dtToTime_Closed);
            // 
            // dtTreatmentFromTime
            // 
            this.dtTreatmentFromTime.EditValue = null;
            this.dtTreatmentFromTime.Location = new System.Drawing.Point(97, 170);
            this.dtTreatmentFromTime.Name = "dtTreatmentFromTime";
            this.dtTreatmentFromTime.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtTreatmentFromTime.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtTreatmentFromTime.Properties.DisplayFormat.FormatString = "dd/MM/yyyy HH:mm";
            this.dtTreatmentFromTime.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.dtTreatmentFromTime.Properties.EditFormat.FormatString = "dd/MM/yyyy HH:mm";
            this.dtTreatmentFromTime.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.dtTreatmentFromTime.Properties.Mask.EditMask = "dd/MM/yyyy HH:mm";
            this.dtTreatmentFromTime.Properties.Mask.UseMaskAsDisplayFormat = true;
            this.dtTreatmentFromTime.Properties.ShowNullValuePromptWhenFocused = true;
            this.dtTreatmentFromTime.Size = new System.Drawing.Size(229, 20);
            this.dtTreatmentFromTime.StyleController = this.lcEditorInfo;
            this.dtTreatmentFromTime.TabIndex = 9;
            this.dtTreatmentFromTime.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.dtTreatmentFromTime_Closed);
            // 
            // dtTreatmentToTime
            // 
            this.dtTreatmentToTime.EditValue = null;
            this.dtTreatmentToTime.Location = new System.Drawing.Point(97, 194);
            this.dtTreatmentToTime.Name = "dtTreatmentToTime";
            this.dtTreatmentToTime.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtTreatmentToTime.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtTreatmentToTime.Properties.DisplayFormat.FormatString = "dd/MM/yyyy HH:mm";
            this.dtTreatmentToTime.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.dtTreatmentToTime.Properties.EditFormat.FormatString = "dd/MM/yyyy HH:mm";
            this.dtTreatmentToTime.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.dtTreatmentToTime.Properties.Mask.EditMask = "dd/MM/yyyy HH:mm";
            this.dtTreatmentToTime.Properties.Mask.UseMaskAsDisplayFormat = true;
            this.dtTreatmentToTime.Properties.ShowNullValuePromptWhenFocused = true;
            this.dtTreatmentToTime.Size = new System.Drawing.Size(229, 20);
            this.dtTreatmentToTime.StyleController = this.lcEditorInfo;
            this.dtTreatmentToTime.TabIndex = 10;
            this.dtTreatmentToTime.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.dtTreatmentToTime_Closed);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(222, 242);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(104, 26);
            this.btnCancel.StyleController = this.lcEditorInfo;
            this.btnCancel.TabIndex = 14;
            this.btnCancel.Text = "Làm lại (Ctrl C)";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(112, 242);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(106, 26);
            this.btnAdd.StyleController = this.lcEditorInfo;
            this.btnAdd.TabIndex = 13;
            this.btnAdd.Text = "Thêm (Ctrl N)";
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.Location = new System.Drawing.Point(2, 242);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(106, 26);
            this.btnEdit.StyleController = this.lcEditorInfo;
            this.btnEdit.TabIndex = 12;
            this.btnEdit.Text = "Sửa (Ctrl S)";
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // layoutControlGroup4
            // 
            this.layoutControlGroup4.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup4.GroupBordersVisible = false;
            this.layoutControlGroup4.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciBtnEdit,
            this.lciBtnAdd,
            this.lciBtnRefresh,
            this.lciExpPrice,
            this.lciExpVatRatio,
            this.lciFromTime,
            this.lciToTime,
            this.lciTreatmentFromTime,
            this.lciTreatmentToTime,
            this.lciPriority,
            this.lciBloodType,
            this.lciBloodId,
            this.lciPatientTypeId});
            this.layoutControlGroup4.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup4.Name = "layoutControlGroup4";
            this.layoutControlGroup4.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup4.Size = new System.Drawing.Size(328, 478);
            this.layoutControlGroup4.TextVisible = false;
            // 
            // lciBtnEdit
            // 
            this.lciBtnEdit.Control = this.btnEdit;
            this.lciBtnEdit.Location = new System.Drawing.Point(0, 240);
            this.lciBtnEdit.MaxSize = new System.Drawing.Size(0, 30);
            this.lciBtnEdit.MinSize = new System.Drawing.Size(69, 30);
            this.lciBtnEdit.Name = "lciBtnEdit";
            this.lciBtnEdit.Size = new System.Drawing.Size(110, 238);
            this.lciBtnEdit.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciBtnEdit.TextSize = new System.Drawing.Size(0, 0);
            this.lciBtnEdit.TextVisible = false;
            // 
            // lciBtnAdd
            // 
            this.lciBtnAdd.Control = this.btnAdd;
            this.lciBtnAdd.Location = new System.Drawing.Point(110, 240);
            this.lciBtnAdd.MaxSize = new System.Drawing.Size(0, 30);
            this.lciBtnAdd.MinSize = new System.Drawing.Size(77, 30);
            this.lciBtnAdd.Name = "lciBtnAdd";
            this.lciBtnAdd.Size = new System.Drawing.Size(110, 238);
            this.lciBtnAdd.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciBtnAdd.TextSize = new System.Drawing.Size(0, 0);
            this.lciBtnAdd.TextVisible = false;
            // 
            // lciBtnRefresh
            // 
            this.lciBtnRefresh.Control = this.btnCancel;
            this.lciBtnRefresh.Location = new System.Drawing.Point(220, 240);
            this.lciBtnRefresh.MaxSize = new System.Drawing.Size(0, 30);
            this.lciBtnRefresh.MinSize = new System.Drawing.Size(83, 30);
            this.lciBtnRefresh.Name = "lciBtnRefresh";
            this.lciBtnRefresh.Size = new System.Drawing.Size(108, 238);
            this.lciBtnRefresh.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciBtnRefresh.TextSize = new System.Drawing.Size(0, 0);
            this.lciBtnRefresh.TextVisible = false;
            // 
            // lciExpPrice
            // 
            this.lciExpPrice.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.lciExpPrice.AppearanceItemCaption.Options.UseForeColor = true;
            this.lciExpPrice.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciExpPrice.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciExpPrice.Control = this.spExpPrice;
            this.lciExpPrice.Location = new System.Drawing.Point(0, 72);
            this.lciExpPrice.Name = "lciExpPrice";
            this.lciExpPrice.Size = new System.Drawing.Size(328, 24);
            this.lciExpPrice.Text = "Giá xuất:";
            this.lciExpPrice.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciExpPrice.TextSize = new System.Drawing.Size(90, 20);
            this.lciExpPrice.TextToControlDistance = 5;
            // 
            // lciExpVatRatio
            // 
            this.lciExpVatRatio.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.lciExpVatRatio.AppearanceItemCaption.Options.UseForeColor = true;
            this.lciExpVatRatio.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciExpVatRatio.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciExpVatRatio.Control = this.spExpVatRatio;
            this.lciExpVatRatio.Location = new System.Drawing.Point(0, 96);
            this.lciExpVatRatio.Name = "lciExpVatRatio";
            this.lciExpVatRatio.Size = new System.Drawing.Size(328, 24);
            this.lciExpVatRatio.Text = "Chiết khấu (%):";
            this.lciExpVatRatio.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciExpVatRatio.TextSize = new System.Drawing.Size(90, 20);
            this.lciExpVatRatio.TextToControlDistance = 5;
            // 
            // lciFromTime
            // 
            this.lciFromTime.AppearanceItemCaption.Options.UseForeColor = true;
            this.lciFromTime.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciFromTime.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciFromTime.Control = this.dtFromTime;
            this.lciFromTime.Location = new System.Drawing.Point(0, 120);
            this.lciFromTime.Name = "lciFromTime";
            this.lciFromTime.Size = new System.Drawing.Size(328, 24);
            this.lciFromTime.Text = "Từ:";
            this.lciFromTime.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciFromTime.TextSize = new System.Drawing.Size(90, 20);
            this.lciFromTime.TextToControlDistance = 5;
            // 
            // lciToTime
            // 
            this.lciToTime.AppearanceItemCaption.Options.UseForeColor = true;
            this.lciToTime.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciToTime.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciToTime.Control = this.dtToTime;
            this.lciToTime.Location = new System.Drawing.Point(0, 144);
            this.lciToTime.Name = "lciToTime";
            this.lciToTime.Size = new System.Drawing.Size(328, 24);
            this.lciToTime.Text = "Đến:";
            this.lciToTime.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciToTime.TextSize = new System.Drawing.Size(90, 20);
            this.lciToTime.TextToControlDistance = 5;
            // 
            // lciTreatmentFromTime
            // 
            this.lciTreatmentFromTime.AppearanceItemCaption.Options.UseForeColor = true;
            this.lciTreatmentFromTime.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciTreatmentFromTime.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciTreatmentFromTime.Control = this.dtTreatmentFromTime;
            this.lciTreatmentFromTime.Location = new System.Drawing.Point(0, 168);
            this.lciTreatmentFromTime.Name = "lciTreatmentFromTime";
            this.lciTreatmentFromTime.Size = new System.Drawing.Size(328, 24);
            this.lciTreatmentFromTime.Text = "Điều trị từ:";
            this.lciTreatmentFromTime.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciTreatmentFromTime.TextSize = new System.Drawing.Size(90, 20);
            this.lciTreatmentFromTime.TextToControlDistance = 5;
            // 
            // lciTreatmentToTime
            // 
            this.lciTreatmentToTime.AppearanceItemCaption.Options.UseForeColor = true;
            this.lciTreatmentToTime.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciTreatmentToTime.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciTreatmentToTime.Control = this.dtTreatmentToTime;
            this.lciTreatmentToTime.Location = new System.Drawing.Point(0, 192);
            this.lciTreatmentToTime.Name = "lciTreatmentToTime";
            this.lciTreatmentToTime.Size = new System.Drawing.Size(328, 24);
            this.lciTreatmentToTime.Text = "Điều trị đến:";
            this.lciTreatmentToTime.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciTreatmentToTime.TextSize = new System.Drawing.Size(90, 20);
            this.lciTreatmentToTime.TextToControlDistance = 5;
            // 
            // lciPriority
            // 
            this.lciPriority.AppearanceItemCaption.Options.UseForeColor = true;
            this.lciPriority.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciPriority.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciPriority.Control = this.spPriority;
            this.lciPriority.Location = new System.Drawing.Point(0, 216);
            this.lciPriority.Name = "lciPriority";
            this.lciPriority.Size = new System.Drawing.Size(328, 24);
            this.lciPriority.Text = "Độ ưu tiên:";
            this.lciPriority.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciPriority.TextSize = new System.Drawing.Size(90, 20);
            this.lciPriority.TextToControlDistance = 5;
            // 
            // lciBloodType
            // 
            this.lciBloodType.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciBloodType.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciBloodType.Control = this.lkBloodType;
            this.lciBloodType.Location = new System.Drawing.Point(0, 0);
            this.lciBloodType.Name = "lciBloodType";
            this.lciBloodType.Size = new System.Drawing.Size(328, 24);
            this.lciBloodType.Text = "Loại:";
            this.lciBloodType.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciBloodType.TextSize = new System.Drawing.Size(90, 20);
            this.lciBloodType.TextToControlDistance = 5;
            // 
            // lciBloodId
            // 
            this.lciBloodId.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.lciBloodId.AppearanceItemCaption.Options.UseForeColor = true;
            this.lciBloodId.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciBloodId.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciBloodId.Control = this.lkBloodId;
            this.lciBloodId.Location = new System.Drawing.Point(0, 24);
            this.lciBloodId.Name = "lciBloodId";
            this.lciBloodId.Size = new System.Drawing.Size(328, 24);
            this.lciBloodId.Text = "Máu:";
            this.lciBloodId.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciBloodId.TextSize = new System.Drawing.Size(90, 20);
            this.lciBloodId.TextToControlDistance = 5;
            // 
            // lciPatientTypeId
            // 
            this.lciPatientTypeId.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.lciPatientTypeId.AppearanceItemCaption.Options.UseForeColor = true;
            this.lciPatientTypeId.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciPatientTypeId.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciPatientTypeId.Control = this.lkPatientTypeId;
            this.lciPatientTypeId.Location = new System.Drawing.Point(0, 48);
            this.lciPatientTypeId.Name = "lciPatientTypeId";
            this.lciPatientTypeId.Size = new System.Drawing.Size(328, 24);
            this.lciPatientTypeId.Text = "Đối tượng BN:";
            this.lciPatientTypeId.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciPatientTypeId.TextSize = new System.Drawing.Size(90, 20);
            this.lciPatientTypeId.TextToControlDistance = 5;
            // 
            // layoutControlGroup3
            // 
            this.layoutControlGroup3.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup3.GroupBordersVisible = false;
            this.layoutControlGroup3.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem4,
            this.layoutControlItem5});
            this.layoutControlGroup3.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup3.Name = "layoutControlGroup3";
            this.layoutControlGroup3.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup3.Size = new System.Drawing.Size(1236, 482);
            this.layoutControlGroup3.TextVisible = false;
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.Control = this.lcEditorInfo;
            this.layoutControlItem4.Location = new System.Drawing.Point(904, 0);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Size = new System.Drawing.Size(332, 482);
            this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem4.TextVisible = false;
            // 
            // layoutControlItem5
            // 
            this.layoutControlItem5.Control = this.layoutControl7;
            this.layoutControlItem5.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem5.Name = "layoutControlItem5";
            this.layoutControlItem5.Size = new System.Drawing.Size(904, 482);
            this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem5.TextVisible = false;
            // 
            // dxValidationProviderEditorInfo
            // 
            this.dxValidationProviderEditorInfo.ValidationFailed += new DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventHandler(this.dxValidationProvider_ValidationFailed);
            // 
            // dxErrorProvider
            // 
            this.dxErrorProvider.ContainerControl = this;
            // 
            // toolTipControllerGrid
            // 
            this.toolTipControllerGrid.AllowHtmlText = true;
            this.toolTipControllerGrid.GetActiveObjectInfo += new DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventHandler(this.toolTipControllerGrid_GetActiveObjectInfo);
            // 
            // frmHisBloodPaty
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1236, 511);
            this.Controls.Add(this.layoutControl4);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmHisBloodPaty";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Chính sách giá máu";
            this.Load += new System.EventHandler(this.frmHisBloodPaty_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl4)).EndInit();
            this.layoutControl4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl7)).EndInit();
            this.layoutControl7.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControlFormList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridviewFormList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.buttonEditDeleteEnable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ButtonEditLock)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ButtonEditUnlock)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ButtonEditDeleteDisable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtKeyword.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcEditorInfo)).EndInit();
            this.lcEditorInfo.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.lkPatientTypeId.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lkBloodId.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lkBloodType.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spExpPrice.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spExpVatRatio.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spPriority.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtFromTime.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtFromTime.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtToTime.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtToTime.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtTreatmentFromTime.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtTreatmentFromTime.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtTreatmentToTime.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtTreatmentToTime.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnEdit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnAdd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnRefresh)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciExpPrice)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciExpVatRatio)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciFromTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciToTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTreatmentFromTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTreatmentToTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPriority)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBloodType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBloodId)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPatientTypeId)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProviderEditorInfo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxErrorProvider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl4;
        private DevExpress.XtraLayout.LayoutControl layoutControl7;
        internal DevExpress.XtraGrid.GridControl gridControlFormList;
        internal DevExpress.XtraGrid.Views.Grid.GridView gridviewFormList;
        private DevExpress.XtraGrid.Columns.GridColumn STT;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnEdit;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit buttonEditDeleteEnable;
        private DevExpress.XtraGrid.Columns.GridColumn grdColModifyTime;
        private DevExpress.XtraGrid.Columns.GridColumn grdColModifier;
private DevExpress.XtraEditors.SpinEdit spExpPrice;
private DevExpress.XtraLayout.LayoutControlItem lciExpPrice;
private DevExpress.XtraEditors.SpinEdit spExpVatRatio;
private DevExpress.XtraLayout.LayoutControlItem lciExpVatRatio;
private DevExpress.XtraEditors.SpinEdit spPriority;
private DevExpress.XtraLayout.LayoutControlItem lciPriority;
private DevExpress.XtraEditors.DateEdit dtFromTime;
private DevExpress.XtraLayout.LayoutControlItem lciFromTime;
private DevExpress.XtraEditors.DateEdit dtToTime;
private DevExpress.XtraLayout.LayoutControlItem lciToTime;
private DevExpress.XtraEditors.DateEdit dtTreatmentFromTime;
private DevExpress.XtraLayout.LayoutControlItem lciTreatmentFromTime;
private DevExpress.XtraEditors.DateEdit dtTreatmentToTime;
private DevExpress.XtraLayout.LayoutControlItem lciTreatmentToTime;

        private DevExpress.XtraGrid.Columns.GridColumn grdColBloodId;
private DevExpress.XtraGrid.Columns.GridColumn grdColPatientTypeId;
private DevExpress.XtraGrid.Columns.GridColumn grdColExpPrice;
private DevExpress.XtraGrid.Columns.GridColumn grdColExpVatRatio;
private DevExpress.XtraGrid.Columns.GridColumn grdColPriority;
private DevExpress.XtraGrid.Columns.GridColumn grdColFromTime;
private DevExpress.XtraGrid.Columns.GridColumn grdColToTime;
private DevExpress.XtraGrid.Columns.GridColumn grdColTreatmentFromTime;
private DevExpress.XtraGrid.Columns.GridColumn grdColTreatmentToTime;


        private DevExpress.XtraGrid.Columns.GridColumn grdColCreator;
        private DevExpress.XtraGrid.Columns.GridColumn grdColCreateTime;
        private Inventec.UC.Paging.UcPaging ucPaging;
        private DevExpress.XtraEditors.TextEdit txtKeyword;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup6;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControl lcEditorInfo;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
        private DevExpress.XtraEditors.SimpleButton btnAdd;
        private DevExpress.XtraEditors.SimpleButton btnEdit;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup4;
        private DevExpress.XtraLayout.LayoutControlItem lciBtnEdit;
        private DevExpress.XtraLayout.LayoutControlItem lciBtnAdd;
        private DevExpress.XtraLayout.LayoutControlItem lciBtnRefresh;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup3;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProviderEditorInfo;
        private DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider dxErrorProvider;
        private DevExpress.Utils.ToolTipController toolTipControllerGrid;
        private DevExpress.XtraEditors.SimpleButton btnSearch;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem3;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.Bar bar1;
        private DevExpress.XtraBars.BarButtonItem bbtnSearch;
        private DevExpress.XtraBars.BarButtonItem bbtnEdit;
        private DevExpress.XtraBars.BarButtonItem bbtnAdd;
        private DevExpress.XtraBars.BarButtonItem bbtnReset;
        private DevExpress.XtraBars.BarButtonItem bbtnFocusDefault;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnLock;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit ButtonEditLock;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit ButtonEditUnlock;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnBloodTypeName;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit ButtonEditDeleteDisable;
        private DevExpress.XtraEditors.GridLookUpEdit lkBloodType;
        private DevExpress.XtraGrid.Views.Grid.GridView gridLookUpEdit1View;
        private DevExpress.XtraLayout.LayoutControlItem lciBloodType;
        private DevExpress.XtraEditors.GridLookUpEdit lkBloodId;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraLayout.LayoutControlItem lciBloodId;
        private DevExpress.XtraEditors.GridLookUpEdit lkPatientTypeId;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView2;
        private DevExpress.XtraLayout.LayoutControlItem lciPatientTypeId;
    }
}
