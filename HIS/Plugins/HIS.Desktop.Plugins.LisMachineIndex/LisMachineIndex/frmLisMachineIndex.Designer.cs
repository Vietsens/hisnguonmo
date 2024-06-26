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
using System.Globalization;
namespace HIS.Desktop.Plugins.LisMachineIndex
{
    partial class frmLisMachineIndex
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmLisMachineIndex));
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
            this.btnImport = new DevExpress.XtraEditors.SimpleButton();
            this.btnSearch = new DevExpress.XtraEditors.SimpleButton();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.gridControlFormList = new DevExpress.XtraGrid.GridControl();
            this.gridviewFormList = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.STT = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnEdit = new DevExpress.XtraGrid.Columns.GridColumn();
            this.btnGEdit = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.grdColCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grdColName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn4 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn5 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn6 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn7 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemCheckEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grdColCreateTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grdColCreator = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grdColModifyTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grdColModifier = new DevExpress.XtraGrid.Columns.GridColumn();
            this.btnGunLock = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.btnGLock = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.repositoryItemButtonEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.ucPaging = new Inventec.UC.Paging.UcPaging();
            this.txtKeyword = new DevExpress.XtraEditors.TextEdit();
            this.layoutControlGroup6 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem3 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.layoutControlItem11 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcEditorInfo = new DevExpress.XtraLayout.LayoutControl();
            this.chkIsMeetIsoStandard = new DevExpress.XtraEditors.CheckEdit();
            this.barManager1 = new DevExpress.XtraBars.BarManager();
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
            this.txtIsoProcessCode = new DevExpress.XtraEditors.TextEdit();
            this.spResultRatio = new DevExpress.XtraEditors.SpinEdit();
            this.txtFomatValue = new DevExpress.XtraEditors.TextEdit();
            this.cboMachine = new DevExpress.XtraEditors.LookUpEdit();
            this.btnRefresh = new DevExpress.XtraEditors.SimpleButton();
            this.btnAdd = new DevExpress.XtraEditors.SimpleButton();
            this.btnEdit = new DevExpress.XtraEditors.SimpleButton();
            this.txtMachineIndexCode = new DevExpress.XtraEditors.TextEdit();
            this.txtMachineIndexName = new DevExpress.XtraEditors.TextEdit();
            this.layoutControlGroup4 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciBedTypeCode = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciBedTypeName = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem7 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem9 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem10 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem12 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem13 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciProcessCode = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciIsMeetIsoStandard = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlGroup3 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            this.dxValidationProviderEditorInfo = new DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider();
            this.dxErrorProvider = new DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider();
            this.toolTipControllerGrid = new DevExpress.Utils.ToolTipController();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl4)).BeginInit();
            this.layoutControl4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl7)).BeginInit();
            this.layoutControl7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlFormList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridviewFormList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnGEdit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnGunLock)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnGLock)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemButtonEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtKeyword.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem11)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcEditorInfo)).BeginInit();
            this.lcEditorInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chkIsMeetIsoStandard.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtIsoProcessCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spResultRatio.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtFomatValue.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboMachine.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMachineIndexCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMachineIndexName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBedTypeCode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBedTypeName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem10)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem12)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem13)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciProcessCode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciIsMeetIsoStandard)).BeginInit();
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
            this.layoutControl7.Controls.Add(this.btnImport);
            this.layoutControl7.Controls.Add(this.btnSearch);
            this.layoutControl7.Controls.Add(this.panelControl1);
            this.layoutControl7.Controls.Add(this.txtKeyword);
            this.layoutControl7.Location = new System.Drawing.Point(2, 2);
            this.layoutControl7.Name = "layoutControl7";
            this.layoutControl7.Root = this.layoutControlGroup6;
            this.layoutControl7.Size = new System.Drawing.Size(896, 478);
            this.layoutControl7.TabIndex = 5;
            this.layoutControl7.Text = "layoutControl7";
            // 
            // btnImport
            // 
            this.btnImport.Location = new System.Drawing.Point(499, 2);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(89, 22);
            this.btnImport.StyleController = this.layoutControl7;
            this.btnImport.TabIndex = 37;
            this.btnImport.Text = "Import";
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(388, 2);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(107, 22);
            this.btnSearch.StyleController = this.layoutControl7;
            this.btnSearch.TabIndex = 2;
            this.btnSearch.Text = "Tìm (Ctrl F)";
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.gridControlFormList);
            this.panelControl1.Controls.Add(this.ucPaging);
            this.panelControl1.Location = new System.Drawing.Point(2, 28);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(892, 448);
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
            this.btnGEdit,
            this.btnGunLock,
            this.btnGLock,
            this.repositoryItemButtonEdit1,
            this.repositoryItemCheckEdit1});
            this.gridControlFormList.Size = new System.Drawing.Size(888, 423);
            this.gridControlFormList.TabIndex = 8;
            this.gridControlFormList.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridviewFormList});
            // 
            // gridviewFormList
            // 
            this.gridviewFormList.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.STT,
            this.gridColumn1,
            this.gridColumnEdit,
            this.grdColCode,
            this.grdColName,
            this.gridColumn4,
            this.gridColumn3,
            this.gridColumn5,
            this.gridColumn6,
            this.gridColumn7,
            this.gridColumn2,
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
            this.gridviewFormList.RowCellClick += new DevExpress.XtraGrid.Views.Grid.RowCellClickEventHandler(this.gridviewFormList_RowCellClick);
            this.gridviewFormList.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gridviewFormList_RowCellStyle);
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
            this.STT.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.STT.ImageAlignment = System.Drawing.StringAlignment.Far;
            this.STT.Name = "STT";
            this.STT.OptionsColumn.AllowEdit = false;
            this.STT.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.STT.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.STT.Visible = true;
            this.STT.VisibleIndex = 0;
            this.STT.Width = 40;
            // 
            // gridColumn1
            // 
            this.gridColumn1.Caption = "gridColumnisLock";
            this.gridColumn1.FieldName = "isLock";
            this.gridColumn1.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.OptionsColumn.ShowCaption = false;
            this.gridColumn1.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn1.Visible = true;
            this.gridColumn1.VisibleIndex = 1;
            this.gridColumn1.Width = 20;
            // 
            // gridColumnEdit
            // 
            this.gridColumnEdit.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumnEdit.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumnEdit.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumnEdit.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumnEdit.ColumnEdit = this.btnGEdit;
            this.gridColumnEdit.FieldName = "Delete";
            this.gridColumnEdit.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gridColumnEdit.ImageAlignment = System.Drawing.StringAlignment.Center;
            this.gridColumnEdit.Name = "gridColumnEdit";
            this.gridColumnEdit.OptionsColumn.ShowCaption = false;
            this.gridColumnEdit.ToolTip = "Sửa";
            this.gridColumnEdit.Visible = true;
            this.gridColumnEdit.VisibleIndex = 2;
            this.gridColumnEdit.Width = 20;
            // 
            // btnGEdit
            // 
            this.btnGEdit.AutoHeight = false;
            this.btnGEdit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, ((System.Drawing.Image)(resources.GetObject("btnGEdit.Buttons"))), new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject1, serializableAppearanceObject2, serializableAppearanceObject3, serializableAppearanceObject4, "Xóa", null, superToolTip1, true)});
            this.btnGEdit.Name = "btnGEdit";
            this.btnGEdit.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            this.btnGEdit.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.btnGDelete_ButtonClick);
            // 
            // grdColCode
            // 
            this.grdColCode.Caption = "Mã";
            this.grdColCode.FieldName = "MACHINE_INDEX_CODE";
            this.grdColCode.Name = "grdColCode";
            this.grdColCode.OptionsColumn.AllowEdit = false;
            this.grdColCode.ToolTip = "BED_TYPE_CODE";
            this.grdColCode.Visible = true;
            this.grdColCode.VisibleIndex = 3;
            this.grdColCode.Width = 101;
            // 
            // grdColName
            // 
            this.grdColName.Caption = "Tên";
            this.grdColName.FieldName = "MACHINE_INDEX_NAME";
            this.grdColName.Name = "grdColName";
            this.grdColName.OptionsColumn.AllowEdit = false;
            this.grdColName.ToolTip = "BED_TYPE_NAME";
            this.grdColName.Visible = true;
            this.grdColName.VisibleIndex = 4;
            this.grdColName.Width = 201;
            // 
            // gridColumn4
            // 
            this.gridColumn4.Caption = "Tên máy";
            this.gridColumn4.FieldName = "MACHINE_NAME";
            this.gridColumn4.Name = "gridColumn4";
            this.gridColumn4.OptionsColumn.AllowEdit = false;
            this.gridColumn4.ToolTip = "Loại máy";
            this.gridColumn4.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn4.Visible = true;
            this.gridColumn4.VisibleIndex = 6;
            this.gridColumn4.Width = 141;
            // 
            // gridColumn3
            // 
            this.gridColumn3.Caption = "Định dạng";
            this.gridColumn3.FieldName = "FORMAT_VALUE";
            this.gridColumn3.Name = "gridColumn3";
            this.gridColumn3.OptionsColumn.AllowEdit = false;
            this.gridColumn3.ToolTip = "Định dạng";
            this.gridColumn3.Visible = true;
            this.gridColumn3.VisibleIndex = 5;
            this.gridColumn3.Width = 100;
            // 
            // gridColumn5
            // 
            this.gridColumn5.Caption = "Hệ số kết quả";
            this.gridColumn5.FieldName = "RESULT_COEFFICIENT_STR";
            this.gridColumn5.Name = "gridColumn5";
            this.gridColumn5.OptionsColumn.AllowEdit = false;
            this.gridColumn5.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn5.Visible = true;
            this.gridColumn5.VisibleIndex = 7;
            this.gridColumn5.Width = 81;
            // 
            // gridColumn6
            // 
            this.gridColumn6.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn6.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn6.Caption = "Mã quy trình";
            this.gridColumn6.FieldName = "ISO_PROCESS_CODE";
            this.gridColumn6.Name = "gridColumn6";
            this.gridColumn6.OptionsColumn.AllowEdit = false;
            this.gridColumn6.Visible = true;
            this.gridColumn6.VisibleIndex = 8;
            this.gridColumn6.Width = 90;
            // 
            // gridColumn7
            // 
            this.gridColumn7.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn7.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn7.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn7.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn7.Caption = "Đạt chuẩn ISO";
            this.gridColumn7.ColumnEdit = this.repositoryItemCheckEdit1;
            this.gridColumn7.FieldName = "IS_MEET_ISO";
            this.gridColumn7.Name = "gridColumn7";
            this.gridColumn7.OptionsColumn.AllowEdit = false;
            this.gridColumn7.ToolTip = "Chỉ số đạt chuẩn ISO";
            this.gridColumn7.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn7.Visible = true;
            this.gridColumn7.VisibleIndex = 9;
            this.gridColumn7.Width = 80;
            // 
            // repositoryItemCheckEdit1
            // 
            this.repositoryItemCheckEdit1.AutoHeight = false;
            this.repositoryItemCheckEdit1.Name = "repositoryItemCheckEdit1";
            this.repositoryItemCheckEdit1.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
            // 
            // gridColumn2
            // 
            this.gridColumn2.Caption = "Trạng thái";
            this.gridColumn2.FieldName = "IS_ACTIVE_STR";
            this.gridColumn2.Name = "gridColumn2";
            this.gridColumn2.OptionsColumn.AllowEdit = false;
            this.gridColumn2.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn2.Visible = true;
            this.gridColumn2.VisibleIndex = 10;
            this.gridColumn2.Width = 77;
            // 
            // grdColCreateTime
            // 
            this.grdColCreateTime.Caption = "Thời gian tạo";
            this.grdColCreateTime.FieldName = "CREATE_TIME_STR";
            this.grdColCreateTime.Name = "grdColCreateTime";
            this.grdColCreateTime.OptionsColumn.AllowEdit = false;
            this.grdColCreateTime.ToolTip = "CREATE_TIME";
            this.grdColCreateTime.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.grdColCreateTime.Visible = true;
            this.grdColCreateTime.VisibleIndex = 11;
            this.grdColCreateTime.Width = 118;
            // 
            // grdColCreator
            // 
            this.grdColCreator.Caption = "Người tạo";
            this.grdColCreator.FieldName = "CREATOR";
            this.grdColCreator.Name = "grdColCreator";
            this.grdColCreator.OptionsColumn.AllowEdit = false;
            this.grdColCreator.ToolTip = "CREATOR";
            this.grdColCreator.Visible = true;
            this.grdColCreator.VisibleIndex = 12;
            this.grdColCreator.Width = 90;
            // 
            // grdColModifyTime
            // 
            this.grdColModifyTime.Caption = "Thời gian sửa";
            this.grdColModifyTime.FieldName = "MODIFY_TIME_STR";
            this.grdColModifyTime.Name = "grdColModifyTime";
            this.grdColModifyTime.OptionsColumn.AllowEdit = false;
            this.grdColModifyTime.ToolTip = "MODIFY_TIME";
            this.grdColModifyTime.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.grdColModifyTime.Visible = true;
            this.grdColModifyTime.VisibleIndex = 13;
            this.grdColModifyTime.Width = 118;
            // 
            // grdColModifier
            // 
            this.grdColModifier.Caption = "Người sửa";
            this.grdColModifier.FieldName = "MODIFIER";
            this.grdColModifier.Name = "grdColModifier";
            this.grdColModifier.OptionsColumn.AllowEdit = false;
            this.grdColModifier.ToolTip = "MODIFIER";
            this.grdColModifier.Visible = true;
            this.grdColModifier.VisibleIndex = 14;
            this.grdColModifier.Width = 90;
            // 
            // btnGunLock
            // 
            this.btnGunLock.AutoHeight = false;
            this.btnGunLock.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, ((System.Drawing.Image)(resources.GetObject("btnGunLock.Buttons"))), new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject5, serializableAppearanceObject6, serializableAppearanceObject7, serializableAppearanceObject8, "", null, null, true)});
            this.btnGunLock.Name = "btnGunLock";
            this.btnGunLock.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            this.btnGunLock.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.btnGunLock_ButtonClick);
            // 
            // btnGLock
            // 
            this.btnGLock.AutoHeight = false;
            this.btnGLock.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, ((System.Drawing.Image)(resources.GetObject("btnGLock.Buttons"))), new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject9, serializableAppearanceObject10, serializableAppearanceObject11, serializableAppearanceObject12, "", null, null, true)});
            this.btnGLock.Name = "btnGLock";
            this.btnGLock.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            this.btnGLock.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.btnGLock_ButtonClick);
            // 
            // repositoryItemButtonEdit1
            // 
            this.repositoryItemButtonEdit1.AutoHeight = false;
            this.repositoryItemButtonEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, false, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, ((System.Drawing.Image)(resources.GetObject("repositoryItemButtonEdit1.Buttons"))), new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject13, serializableAppearanceObject14, serializableAppearanceObject15, serializableAppearanceObject16, "", null, null, true)});
            this.repositoryItemButtonEdit1.Name = "repositoryItemButtonEdit1";
            this.repositoryItemButtonEdit1.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            // 
            // ucPaging
            // 
            this.ucPaging.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ucPaging.Location = new System.Drawing.Point(2, 425);
            this.ucPaging.Name = "ucPaging";
            this.ucPaging.Size = new System.Drawing.Size(888, 21);
            this.ucPaging.TabIndex = 9;
            // 
            // txtKeyword
            // 
            this.txtKeyword.Location = new System.Drawing.Point(2, 2);
            this.txtKeyword.Name = "txtKeyword";
            this.txtKeyword.Properties.NullValuePrompt = "Từ khóa tìm kiếm";
            this.txtKeyword.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtKeyword.Properties.ShowNullValuePromptWhenFocused = true;
            this.txtKeyword.Size = new System.Drawing.Size(382, 20);
            this.txtKeyword.StyleController = this.layoutControl7;
            this.txtKeyword.TabIndex = 1;
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
            this.emptySpaceItem3,
            this.layoutControlItem11});
            this.layoutControlGroup6.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup6.Name = "layoutControlGroup6";
            this.layoutControlGroup6.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup6.Size = new System.Drawing.Size(896, 478);
            this.layoutControlGroup6.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.txtKeyword;
            this.layoutControlItem2.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(386, 26);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.panelControl1;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 26);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(896, 452);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.btnSearch;
            this.layoutControlItem3.Location = new System.Drawing.Point(386, 0);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(111, 26);
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextVisible = false;
            // 
            // emptySpaceItem3
            // 
            this.emptySpaceItem3.AllowHotTrack = false;
            this.emptySpaceItem3.Location = new System.Drawing.Point(590, 0);
            this.emptySpaceItem3.Name = "emptySpaceItem3";
            this.emptySpaceItem3.Size = new System.Drawing.Size(306, 26);
            this.emptySpaceItem3.TextSize = new System.Drawing.Size(0, 0);
            // 
            // layoutControlItem11
            // 
            this.layoutControlItem11.Control = this.btnImport;
            this.layoutControlItem11.Location = new System.Drawing.Point(497, 0);
            this.layoutControlItem11.Name = "layoutControlItem11";
            this.layoutControlItem11.Size = new System.Drawing.Size(93, 26);
            this.layoutControlItem11.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem11.TextVisible = false;
            // 
            // lcEditorInfo
            // 
            this.lcEditorInfo.Controls.Add(this.chkIsMeetIsoStandard);
            this.lcEditorInfo.Controls.Add(this.txtIsoProcessCode);
            this.lcEditorInfo.Controls.Add(this.spResultRatio);
            this.lcEditorInfo.Controls.Add(this.txtFomatValue);
            this.lcEditorInfo.Controls.Add(this.cboMachine);
            this.lcEditorInfo.Controls.Add(this.btnRefresh);
            this.lcEditorInfo.Controls.Add(this.btnAdd);
            this.lcEditorInfo.Controls.Add(this.btnEdit);
            this.lcEditorInfo.Controls.Add(this.txtMachineIndexCode);
            this.lcEditorInfo.Controls.Add(this.txtMachineIndexName);
            this.lcEditorInfo.Location = new System.Drawing.Point(902, 2);
            this.lcEditorInfo.Name = "lcEditorInfo";
            this.lcEditorInfo.OptionsFocus.EnableAutoTabOrder = false;
            this.lcEditorInfo.Root = this.layoutControlGroup4;
            this.lcEditorInfo.Size = new System.Drawing.Size(332, 478);
            this.lcEditorInfo.TabIndex = 4;
            this.lcEditorInfo.Text = "layoutControl5";
            // 
            // chkIsMeetIsoStandard
            // 
            this.chkIsMeetIsoStandard.Location = new System.Drawing.Point(97, 146);
            this.chkIsMeetIsoStandard.MenuManager = this.barManager1;
            this.chkIsMeetIsoStandard.Name = "chkIsMeetIsoStandard";
            this.chkIsMeetIsoStandard.Properties.Caption = "";
            this.chkIsMeetIsoStandard.Properties.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
            this.chkIsMeetIsoStandard.Size = new System.Drawing.Size(233, 19);
            this.chkIsMeetIsoStandard.StyleController = this.lcEditorInfo;
            this.chkIsMeetIsoStandard.TabIndex = 33;
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
            this.bbtnReset.Caption = "Làm lại (Ctrl R)";
            this.bbtnReset.Id = 3;
            this.bbtnReset.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R));
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
            // txtIsoProcessCode
            // 
            this.txtIsoProcessCode.Location = new System.Drawing.Point(97, 122);
            this.txtIsoProcessCode.MenuManager = this.barManager1;
            this.txtIsoProcessCode.Name = "txtIsoProcessCode";
            this.txtIsoProcessCode.Size = new System.Drawing.Size(233, 20);
            this.txtIsoProcessCode.StyleController = this.lcEditorInfo;
            this.txtIsoProcessCode.TabIndex = 32;
            // 
            // spResultRatio
            // 
            this.spResultRatio.EditValue = new decimal(new int[] {
            101234,
            0,
            0,
            262144});
            this.spResultRatio.Location = new System.Drawing.Point(97, 98);
            this.spResultRatio.MenuManager = this.barManager1;
            this.spResultRatio.Name = "spResultRatio";
            this.spResultRatio.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.spResultRatio.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.spResultRatio.Properties.Mask.EditMask = "###################.######";
            this.spResultRatio.Properties.Mask.UseMaskAsDisplayFormat = true;
            this.spResultRatio.Properties.MaxLength = 30;
            this.spResultRatio.Size = new System.Drawing.Size(233, 20);
            this.spResultRatio.StyleController = this.lcEditorInfo;
            this.spResultRatio.TabIndex = 31;
            this.spResultRatio.KeyUp += new System.Windows.Forms.KeyEventHandler(this.spResultRatio_KeyUp);
            // 
            // txtFomatValue
            // 
            this.txtFomatValue.Location = new System.Drawing.Point(97, 74);
            this.txtFomatValue.MenuManager = this.barManager1;
            this.txtFomatValue.Name = "txtFomatValue";
            this.txtFomatValue.Size = new System.Drawing.Size(233, 20);
            this.txtFomatValue.StyleController = this.lcEditorInfo;
            this.txtFomatValue.TabIndex = 30;
            this.txtFomatValue.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textEdit1_KeyUp);
            // 
            // cboMachine
            // 
            this.cboMachine.Location = new System.Drawing.Point(97, 50);
            this.cboMachine.MenuManager = this.barManager1;
            this.cboMachine.Name = "cboMachine";
            this.cboMachine.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboMachine.Properties.NullText = "";
            this.cboMachine.Size = new System.Drawing.Size(233, 20);
            this.cboMachine.StyleController = this.lcEditorInfo;
            this.cboMachine.TabIndex = 29;
            this.cboMachine.KeyUp += new System.Windows.Forms.KeyEventHandler(this.cboMachine_KeyUp);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(225, 170);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(105, 22);
            this.btnRefresh.StyleController = this.lcEditorInfo;
            this.btnRefresh.TabIndex = 28;
            this.btnRefresh.Text = "Làm lại (Ctrl R)";
            this.btnRefresh.Click += new System.EventHandler(this.btnRefesh_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(114, 170);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(107, 22);
            this.btnAdd.StyleController = this.lcEditorInfo;
            this.btnAdd.TabIndex = 27;
            this.btnAdd.Text = "Thêm (Ctrl N)";
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.Location = new System.Drawing.Point(2, 170);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(108, 22);
            this.btnEdit.StyleController = this.lcEditorInfo;
            this.btnEdit.TabIndex = 26;
            this.btnEdit.Text = "Sửa (Ctrl S)";
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // txtMachineIndexCode
            // 
            this.txtMachineIndexCode.Location = new System.Drawing.Point(97, 2);
            this.txtMachineIndexCode.Name = "txtMachineIndexCode";
            this.txtMachineIndexCode.Size = new System.Drawing.Size(233, 20);
            this.txtMachineIndexCode.StyleController = this.lcEditorInfo;
            this.txtMachineIndexCode.TabIndex = 0;
            this.txtMachineIndexCode.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtMachineIndexCode_KeyUp);
            // 
            // txtMachineIndexName
            // 
            this.txtMachineIndexName.Location = new System.Drawing.Point(97, 26);
            this.txtMachineIndexName.Name = "txtMachineIndexName";
            this.txtMachineIndexName.Size = new System.Drawing.Size(233, 20);
            this.txtMachineIndexName.StyleController = this.lcEditorInfo;
            this.txtMachineIndexName.TabIndex = 1;
            this.txtMachineIndexName.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtMachineIndexName_KeyUp);
            // 
            // layoutControlGroup4
            // 
            this.layoutControlGroup4.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup4.GroupBordersVisible = false;
            this.layoutControlGroup4.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciBedTypeCode,
            this.lciBedTypeName,
            this.emptySpaceItem1,
            this.layoutControlItem6,
            this.layoutControlItem7,
            this.layoutControlItem9,
            this.layoutControlItem10,
            this.layoutControlItem12,
            this.layoutControlItem13,
            this.lciProcessCode,
            this.lciIsMeetIsoStandard});
            this.layoutControlGroup4.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup4.Name = "layoutControlGroup4";
            this.layoutControlGroup4.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup4.Size = new System.Drawing.Size(332, 478);
            this.layoutControlGroup4.TextVisible = false;
            // 
            // lciBedTypeCode
            // 
            this.lciBedTypeCode.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.lciBedTypeCode.AppearanceItemCaption.Options.UseForeColor = true;
            this.lciBedTypeCode.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciBedTypeCode.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciBedTypeCode.Control = this.txtMachineIndexCode;
            this.lciBedTypeCode.Location = new System.Drawing.Point(0, 0);
            this.lciBedTypeCode.Name = "lciBedTypeCode";
            this.lciBedTypeCode.Size = new System.Drawing.Size(332, 24);
            this.lciBedTypeCode.Text = "Mã:";
            this.lciBedTypeCode.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciBedTypeCode.TextSize = new System.Drawing.Size(90, 20);
            this.lciBedTypeCode.TextToControlDistance = 5;
            // 
            // lciBedTypeName
            // 
            this.lciBedTypeName.AppearanceItemCaption.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.lciBedTypeName.AppearanceItemCaption.Options.UseForeColor = true;
            this.lciBedTypeName.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciBedTypeName.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciBedTypeName.Control = this.txtMachineIndexName;
            this.lciBedTypeName.Location = new System.Drawing.Point(0, 24);
            this.lciBedTypeName.Name = "lciBedTypeName";
            this.lciBedTypeName.Size = new System.Drawing.Size(332, 24);
            this.lciBedTypeName.Text = "Tên:";
            this.lciBedTypeName.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciBedTypeName.TextSize = new System.Drawing.Size(90, 20);
            this.lciBedTypeName.TextToControlDistance = 5;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(0, 194);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(332, 284);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // layoutControlItem6
            // 
            this.layoutControlItem6.Control = this.btnEdit;
            this.layoutControlItem6.Location = new System.Drawing.Point(0, 168);
            this.layoutControlItem6.Name = "layoutControlItem6";
            this.layoutControlItem6.Size = new System.Drawing.Size(112, 26);
            this.layoutControlItem6.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem6.TextVisible = false;
            // 
            // layoutControlItem7
            // 
            this.layoutControlItem7.Control = this.btnAdd;
            this.layoutControlItem7.Location = new System.Drawing.Point(112, 168);
            this.layoutControlItem7.Name = "layoutControlItem7";
            this.layoutControlItem7.Size = new System.Drawing.Size(111, 26);
            this.layoutControlItem7.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem7.TextVisible = false;
            // 
            // layoutControlItem9
            // 
            this.layoutControlItem9.Control = this.btnRefresh;
            this.layoutControlItem9.Location = new System.Drawing.Point(223, 168);
            this.layoutControlItem9.Name = "layoutControlItem9";
            this.layoutControlItem9.Size = new System.Drawing.Size(109, 26);
            this.layoutControlItem9.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem9.TextVisible = false;
            // 
            // layoutControlItem10
            // 
            this.layoutControlItem10.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.layoutControlItem10.AppearanceItemCaption.Options.UseForeColor = true;
            this.layoutControlItem10.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem10.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem10.Control = this.cboMachine;
            this.layoutControlItem10.Location = new System.Drawing.Point(0, 48);
            this.layoutControlItem10.Name = "layoutControlItem10";
            this.layoutControlItem10.Size = new System.Drawing.Size(332, 24);
            this.layoutControlItem10.Text = "Máy xét nghiệm:";
            this.layoutControlItem10.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem10.TextSize = new System.Drawing.Size(90, 20);
            this.layoutControlItem10.TextToControlDistance = 5;
            // 
            // layoutControlItem12
            // 
            this.layoutControlItem12.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem12.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem12.Control = this.txtFomatValue;
            this.layoutControlItem12.Location = new System.Drawing.Point(0, 72);
            this.layoutControlItem12.Name = "layoutControlItem12";
            this.layoutControlItem12.Size = new System.Drawing.Size(332, 24);
            this.layoutControlItem12.Text = "Định dạng:";
            this.layoutControlItem12.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem12.TextSize = new System.Drawing.Size(90, 20);
            this.layoutControlItem12.TextToControlDistance = 5;
            // 
            // layoutControlItem13
            // 
            this.layoutControlItem13.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem13.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem13.Control = this.spResultRatio;
            this.layoutControlItem13.Location = new System.Drawing.Point(0, 96);
            this.layoutControlItem13.Name = "layoutControlItem13";
            this.layoutControlItem13.Size = new System.Drawing.Size(332, 24);
            this.layoutControlItem13.Text = "Hệ số kết quả:";
            this.layoutControlItem13.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem13.TextSize = new System.Drawing.Size(90, 20);
            this.layoutControlItem13.TextToControlDistance = 5;
            // 
            // lciProcessCode
            // 
            this.lciProcessCode.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciProcessCode.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciProcessCode.Control = this.txtIsoProcessCode;
            this.lciProcessCode.Location = new System.Drawing.Point(0, 120);
            this.lciProcessCode.Name = "lciProcessCode";
            this.lciProcessCode.Size = new System.Drawing.Size(332, 24);
            this.lciProcessCode.Text = "Mã quy trình:";
            this.lciProcessCode.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciProcessCode.TextSize = new System.Drawing.Size(90, 20);
            this.lciProcessCode.TextToControlDistance = 5;
            // 
            // lciIsMeetIsoStandard
            // 
            this.lciIsMeetIsoStandard.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciIsMeetIsoStandard.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciIsMeetIsoStandard.Control = this.chkIsMeetIsoStandard;
            this.lciIsMeetIsoStandard.Location = new System.Drawing.Point(0, 144);
            this.lciIsMeetIsoStandard.Name = "lciIsMeetIsoStandard";
            this.lciIsMeetIsoStandard.OptionsToolTip.ToolTip = "Chỉ số đạt chuẩn ISO";
            this.lciIsMeetIsoStandard.Size = new System.Drawing.Size(332, 24);
            this.lciIsMeetIsoStandard.Text = "Đạt chuẩn ISO:";
            this.lciIsMeetIsoStandard.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciIsMeetIsoStandard.TextSize = new System.Drawing.Size(90, 20);
            this.lciIsMeetIsoStandard.TextToControlDistance = 5;
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
            this.layoutControlItem4.Location = new System.Drawing.Point(900, 0);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Size = new System.Drawing.Size(336, 482);
            this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem4.TextVisible = false;
            // 
            // layoutControlItem5
            // 
            this.layoutControlItem5.Control = this.layoutControl7;
            this.layoutControlItem5.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem5.Name = "layoutControlItem5";
            this.layoutControlItem5.Size = new System.Drawing.Size(900, 482);
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
            // frmLisMachineIndex
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
            this.Name = "frmLisMachineIndex";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Danh mục";
            this.Load += new System.EventHandler(this.frmHisAwareness_Load);
            this.Controls.SetChildIndex(this.barDockControlTop, 0);
            this.Controls.SetChildIndex(this.barDockControlBottom, 0);
            this.Controls.SetChildIndex(this.barDockControlRight, 0);
            this.Controls.SetChildIndex(this.barDockControlLeft, 0);
            this.Controls.SetChildIndex(this.layoutControl4, 0);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl4)).EndInit();
            this.layoutControl4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl7)).EndInit();
            this.layoutControl7.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControlFormList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridviewFormList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnGEdit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnGunLock)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnGLock)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemButtonEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtKeyword.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem11)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcEditorInfo)).EndInit();
            this.lcEditorInfo.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chkIsMeetIsoStandard.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtIsoProcessCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spResultRatio.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtFomatValue.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboMachine.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMachineIndexCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMachineIndexName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBedTypeCode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBedTypeName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem10)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem12)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem13)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciProcessCode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciIsMeetIsoStandard)).EndInit();
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
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit btnGEdit;
        private DevExpress.XtraGrid.Columns.GridColumn grdColModifyTime;
        private DevExpress.XtraGrid.Columns.GridColumn grdColModifier;

        private DevExpress.XtraEditors.TextEdit txtMachineIndexCode;
        private DevExpress.XtraLayout.LayoutControlItem lciBedTypeCode;
        private DevExpress.XtraEditors.TextEdit txtMachineIndexName;
        private DevExpress.XtraLayout.LayoutControlItem lciBedTypeName;

        private DevExpress.XtraGrid.Columns.GridColumn grdColCode;
        private DevExpress.XtraGrid.Columns.GridColumn grdColName;


        private DevExpress.XtraGrid.Columns.GridColumn grdColCreator;
        private DevExpress.XtraGrid.Columns.GridColumn grdColCreateTime;
        private Inventec.UC.Paging.UcPaging ucPaging;
        private DevExpress.XtraEditors.TextEdit txtKeyword;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup6;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControl lcEditorInfo;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup4;
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
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit btnGunLock;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit btnGLock;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        private DevExpress.XtraEditors.SimpleButton btnRefresh;
        private DevExpress.XtraEditors.SimpleButton btnAdd;
        private DevExpress.XtraEditors.SimpleButton btnEdit;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem6;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem7;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem9;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit repositoryItemButtonEdit1;
        private DevExpress.XtraEditors.LookUpEdit cboMachine;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem10;
        private DevExpress.XtraEditors.SimpleButton btnImport;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem11;
        private DevExpress.XtraEditors.TextEdit txtFomatValue;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem12;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn4;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn3;
        private DevExpress.XtraEditors.SpinEdit spResultRatio;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem13;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn5;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn6;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn7;
        private DevExpress.XtraEditors.CheckEdit chkIsMeetIsoStandard;
        private DevExpress.XtraEditors.TextEdit txtIsoProcessCode;
        private DevExpress.XtraLayout.LayoutControlItem lciProcessCode;
        private DevExpress.XtraLayout.LayoutControlItem lciIsMeetIsoStandard;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheckEdit1;
    }
}
