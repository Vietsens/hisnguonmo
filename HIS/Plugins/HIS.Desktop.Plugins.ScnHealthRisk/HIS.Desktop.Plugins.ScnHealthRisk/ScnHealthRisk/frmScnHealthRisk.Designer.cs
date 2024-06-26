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
namespace HIS.Desktop.Plugins.ScnHealthRisk.ScnHealthRisk
{
    partial class frmScnHealthRisk
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmScnHealthRisk));
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject1 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject2 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject3 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject4 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SuperToolTip superToolTip1 = new DevExpress.Utils.SuperToolTip();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject5 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject6 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject7 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject8 = new DevExpress.Utils.SerializableAppearanceObject();
            this.layoutControl4 = new DevExpress.XtraLayout.LayoutControl();
            this.layoutControl7 = new DevExpress.XtraLayout.LayoutControl();
            this.btnSearch = new DevExpress.XtraEditors.SimpleButton();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.gridControlFormList = new DevExpress.XtraGrid.GridControl();
            this.gridviewFormList = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.STT = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnEdit = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grdColPatientCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grdColHealthRiskTypeId = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grdColIsGet = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemCheck__IsGet = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.grdColIsQuit = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemCheck__IsQuit = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.grdColDescription = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grdColCreateTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grdColCreator = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grdColModifyTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grdColModifier = new DevExpress.XtraGrid.Columns.GridColumn();
            this.btnGDelete__Enable = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.btnGDelete__Disable = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.ucPaging = new Inventec.UC.Paging.UcPaging();
            this.txtKeyword = new DevExpress.XtraEditors.TextEdit();
            this.layoutControlGroup6 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem3 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.lcEditorInfo = new DevExpress.XtraLayout.LayoutControl();
            this.lblPatientCode = new DevExpress.XtraEditors.LabelControl();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.txtDescription = new DevExpress.XtraEditors.MemoEdit();
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
            this.lkHealthRiskTypeId = new DevExpress.XtraEditors.LookUpEdit();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            this.btnAdd = new DevExpress.XtraEditors.SimpleButton();
            this.dnNavigation = new DevExpress.XtraEditors.DataNavigator();
            this.btnEdit = new DevExpress.XtraEditors.SimpleButton();
            this.chkIsGet = new DevExpress.XtraEditors.CheckEdit();
            this.chkIsQuit = new DevExpress.XtraEditors.CheckEdit();
            this.layoutControlGroup4 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem8 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem17 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem18 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem20 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciHealthRiskTypeId = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciIsGet = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciDescription = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.lciIsQuit = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem7 = new DevExpress.XtraLayout.LayoutControlItem();
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
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheck__IsGet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheck__IsQuit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnGDelete__Enable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnGDelete__Disable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtKeyword.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcEditorInfo)).BeginInit();
            this.lcEditorInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDescription.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lkHealthRiskTypeId.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkIsGet.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkIsQuit.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem17)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem18)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem20)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciHealthRiskTypeId)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciIsGet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDescription)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciIsQuit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).BeginInit();
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
            this.layoutControl7.Size = new System.Drawing.Size(792, 478);
            this.layoutControl7.TabIndex = 5;
            this.layoutControl7.Text = "layoutControl7";
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(343, 2);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(81, 22);
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
            this.panelControl1.Size = new System.Drawing.Size(788, 448);
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
            this.btnGDelete__Enable,
            this.btnGDelete__Disable,
            this.repositoryItemCheck__IsGet,
            this.repositoryItemCheck__IsQuit});
            this.gridControlFormList.Size = new System.Drawing.Size(784, 423);
            this.gridControlFormList.TabIndex = 8;
            this.gridControlFormList.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridviewFormList});
            this.gridControlFormList.DoubleClick += new System.EventHandler(this.gridControlFormList_DoubleClick);
            // 
            // gridviewFormList
            // 
            this.gridviewFormList.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.STT,
            this.gridColumnEdit,
            this.grdColPatientCode,
            this.gridColumn1,
            this.grdColHealthRiskTypeId,
            this.grdColIsGet,
            this.grdColIsQuit,
            this.grdColDescription,
            this.grdColCreateTime,
            this.grdColCreator,
            this.grdColModifyTime,
            this.grdColModifier});
            this.gridviewFormList.GridControl = this.gridControlFormList;
            this.gridviewFormList.Name = "gridviewFormList";
            this.gridviewFormList.OptionsBehavior.AllowAddRows = DevExpress.Utils.DefaultBoolean.False;
            this.gridviewFormList.OptionsBehavior.AllowDeleteRows = DevExpress.Utils.DefaultBoolean.True;
            this.gridviewFormList.OptionsFind.AllowFindPanel = false;
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
            this.STT.Width = 42;
            // 
            // gridColumnEdit
            // 
            this.gridColumnEdit.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumnEdit.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumnEdit.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumnEdit.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumnEdit.FieldName = "DELETE";
            this.gridColumnEdit.ImageAlignment = System.Drawing.StringAlignment.Center;
            this.gridColumnEdit.Name = "gridColumnEdit";
            this.gridColumnEdit.OptionsColumn.ShowCaption = false;
            this.gridColumnEdit.ToolTip = "Xóa";
            this.gridColumnEdit.Visible = true;
            this.gridColumnEdit.VisibleIndex = 1;
            this.gridColumnEdit.Width = 20;
            // 
            // grdColPatientCode
            // 
            this.grdColPatientCode.Caption = "Mã bệnh nhân";
            this.grdColPatientCode.FieldName = "PATIENT_CODE";
            this.grdColPatientCode.Name = "grdColPatientCode";
            this.grdColPatientCode.OptionsColumn.AllowEdit = false;
            this.grdColPatientCode.ToolTip = "PATIENT_CODE";
            this.grdColPatientCode.Visible = true;
            this.grdColPatientCode.VisibleIndex = 2;
            this.grdColPatientCode.Width = 105;
            // 
            // gridColumn1
            // 
            this.gridColumn1.Caption = "Mã loại";
            this.gridColumn1.FieldName = "HEALTH_RISK_TYPE_CODE";
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.OptionsColumn.AllowEdit = false;
            this.gridColumn1.Visible = true;
            this.gridColumn1.VisibleIndex = 3;
            this.gridColumn1.Width = 89;
            // 
            // grdColHealthRiskTypeId
            // 
            this.grdColHealthRiskTypeId.Caption = "Tên loại";
            this.grdColHealthRiskTypeId.FieldName = "HEALTH_RISK_TYPE_NAME";
            this.grdColHealthRiskTypeId.Name = "grdColHealthRiskTypeId";
            this.grdColHealthRiskTypeId.OptionsColumn.AllowEdit = false;
            this.grdColHealthRiskTypeId.ToolTip = "HEALTH_RISK_TYPE_NAME";
            this.grdColHealthRiskTypeId.Visible = true;
            this.grdColHealthRiskTypeId.VisibleIndex = 4;
            this.grdColHealthRiskTypeId.Width = 232;
            // 
            // grdColIsGet
            // 
            this.grdColIsGet.Caption = "Mắc phải";
            this.grdColIsGet.ColumnEdit = this.repositoryItemCheck__IsGet;
            this.grdColIsGet.FieldName = "isGetCheck";
            this.grdColIsGet.Name = "grdColIsGet";
            this.grdColIsGet.OptionsColumn.AllowEdit = false;
            this.grdColIsGet.ToolTip = "IS_GET";
            this.grdColIsGet.Visible = true;
            this.grdColIsGet.VisibleIndex = 5;
            this.grdColIsGet.Width = 74;
            // 
            // repositoryItemCheck__IsGet
            // 
            this.repositoryItemCheck__IsGet.AutoHeight = false;
            this.repositoryItemCheck__IsGet.Name = "repositoryItemCheck__IsGet";
            // 
            // grdColIsQuit
            // 
            this.grdColIsQuit.Caption = "Đã bỏ";
            this.grdColIsQuit.ColumnEdit = this.repositoryItemCheck__IsQuit;
            this.grdColIsQuit.FieldName = "isQuitCheck";
            this.grdColIsQuit.Name = "grdColIsQuit";
            this.grdColIsQuit.OptionsColumn.AllowEdit = false;
            this.grdColIsQuit.ToolTip = "IS_QUIT";
            this.grdColIsQuit.Visible = true;
            this.grdColIsQuit.VisibleIndex = 6;
            this.grdColIsQuit.Width = 53;
            // 
            // repositoryItemCheck__IsQuit
            // 
            this.repositoryItemCheck__IsQuit.AutoHeight = false;
            this.repositoryItemCheck__IsQuit.Name = "repositoryItemCheck__IsQuit";
            // 
            // grdColDescription
            // 
            this.grdColDescription.Caption = "Mô tả";
            this.grdColDescription.FieldName = "DESCRIPTION";
            this.grdColDescription.Name = "grdColDescription";
            this.grdColDescription.OptionsColumn.AllowEdit = false;
            this.grdColDescription.ToolTip = "DESCRIPTION";
            this.grdColDescription.Visible = true;
            this.grdColDescription.VisibleIndex = 7;
            this.grdColDescription.Width = 135;
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
            this.grdColCreateTime.VisibleIndex = 8;
            this.grdColCreateTime.Width = 80;
            // 
            // grdColCreator
            // 
            this.grdColCreator.Caption = "Người tạo";
            this.grdColCreator.FieldName = "CREATOR";
            this.grdColCreator.Name = "grdColCreator";
            this.grdColCreator.OptionsColumn.AllowEdit = false;
            this.grdColCreator.ToolTip = "CREATOR";
            this.grdColCreator.Visible = true;
            this.grdColCreator.VisibleIndex = 9;
            this.grdColCreator.Width = 77;
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
            this.grdColModifyTime.VisibleIndex = 10;
            // 
            // grdColModifier
            // 
            this.grdColModifier.Caption = "Người sửa";
            this.grdColModifier.FieldName = "MODIFIER";
            this.grdColModifier.Name = "grdColModifier";
            this.grdColModifier.OptionsColumn.AllowEdit = false;
            this.grdColModifier.ToolTip = "MODIFIER";
            this.grdColModifier.Visible = true;
            this.grdColModifier.VisibleIndex = 11;
            this.grdColModifier.Width = 95;
            // 
            // btnGDelete__Enable
            // 
            this.btnGDelete__Enable.AutoHeight = false;
            this.btnGDelete__Enable.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, ((System.Drawing.Image)(resources.GetObject("btnGDelete__Enable.Buttons"))), new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject1, serializableAppearanceObject2, serializableAppearanceObject3, serializableAppearanceObject4, "Xóa", null, superToolTip1, true)});
            this.btnGDelete__Enable.Name = "btnGDelete__Enable";
            this.btnGDelete__Enable.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            this.btnGDelete__Enable.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.btnGDelete_ButtonClick);
            // 
            // btnGDelete__Disable
            // 
            this.btnGDelete__Disable.AutoHeight = false;
            this.btnGDelete__Disable.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, false, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, ((System.Drawing.Image)(resources.GetObject("btnGDelete__Disable.Buttons"))), new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject5, serializableAppearanceObject6, serializableAppearanceObject7, serializableAppearanceObject8, "Xóa", null, null, true)});
            this.btnGDelete__Disable.Name = "btnGDelete__Disable";
            this.btnGDelete__Disable.ReadOnly = true;
            this.btnGDelete__Disable.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            // 
            // ucPaging
            // 
            this.ucPaging.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ucPaging.Location = new System.Drawing.Point(2, 425);
            this.ucPaging.Name = "ucPaging";
            this.ucPaging.Size = new System.Drawing.Size(784, 21);
            this.ucPaging.TabIndex = 9;
            // 
            // txtKeyword
            // 
            this.txtKeyword.Location = new System.Drawing.Point(2, 2);
            this.txtKeyword.Name = "txtKeyword";
            this.txtKeyword.Properties.NullValuePrompt = "Từ khóa tìm kiếm";
            this.txtKeyword.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtKeyword.Properties.ShowNullValuePromptWhenFocused = true;
            this.txtKeyword.Size = new System.Drawing.Size(337, 20);
            this.txtKeyword.StyleController = this.layoutControl7;
            this.txtKeyword.TabIndex = 1;
            this.txtKeyword.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtKeyword_KeyUp);
            this.txtKeyword.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtKeyword_PreviewKeyDown_1);
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
            this.layoutControlGroup6.Size = new System.Drawing.Size(792, 478);
            this.layoutControlGroup6.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.txtKeyword;
            this.layoutControlItem2.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(341, 26);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.panelControl1;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 26);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(792, 452);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.btnSearch;
            this.layoutControlItem3.Location = new System.Drawing.Point(341, 0);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(85, 26);
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextVisible = false;
            // 
            // emptySpaceItem3
            // 
            this.emptySpaceItem3.AllowHotTrack = false;
            this.emptySpaceItem3.Location = new System.Drawing.Point(426, 0);
            this.emptySpaceItem3.Name = "emptySpaceItem3";
            this.emptySpaceItem3.Size = new System.Drawing.Size(366, 26);
            this.emptySpaceItem3.TextSize = new System.Drawing.Size(0, 0);
            // 
            // lcEditorInfo
            // 
            this.lcEditorInfo.Controls.Add(this.lblPatientCode);
            this.lcEditorInfo.Controls.Add(this.layoutControl1);
            this.lcEditorInfo.Controls.Add(this.txtDescription);
            this.lcEditorInfo.Controls.Add(this.lkHealthRiskTypeId);
            this.lcEditorInfo.Controls.Add(this.btnCancel);
            this.lcEditorInfo.Controls.Add(this.btnAdd);
            this.lcEditorInfo.Controls.Add(this.dnNavigation);
            this.lcEditorInfo.Controls.Add(this.btnEdit);
            this.lcEditorInfo.Controls.Add(this.chkIsGet);
            this.lcEditorInfo.Controls.Add(this.chkIsQuit);
            this.lcEditorInfo.Location = new System.Drawing.Point(798, 2);
            this.lcEditorInfo.Name = "lcEditorInfo";
            this.lcEditorInfo.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(930, 192, 250, 350);
            this.lcEditorInfo.OptionsFocus.EnableAutoTabOrder = false;
            this.lcEditorInfo.Root = this.layoutControlGroup4;
            this.lcEditorInfo.Size = new System.Drawing.Size(436, 478);
            this.lcEditorInfo.TabIndex = 4;
            this.lcEditorInfo.Text = "layoutControl5";
            // 
            // lblPatientCode
            // 
            this.lblPatientCode.Location = new System.Drawing.Point(97, 25);
            this.lblPatientCode.Name = "lblPatientCode";
            this.lblPatientCode.Size = new System.Drawing.Size(337, 20);
            this.lblPatientCode.StyleController = this.lcEditorInfo;
            this.lblPatientCode.TabIndex = 28;
            // 
            // layoutControl1
            // 
            this.layoutControl1.Location = new System.Drawing.Point(2, 171);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.Root;
            this.layoutControl1.Size = new System.Drawing.Size(432, 305);
            this.layoutControl1.TabIndex = 27;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Location = new System.Drawing.Point(0, 0);
            this.Root.Name = "Root";
            this.Root.Size = new System.Drawing.Size(432, 305);
            this.Root.TextVisible = false;
            // 
            // txtDescription
            // 
            this.txtDescription.Location = new System.Drawing.Point(97, 97);
            this.txtDescription.MenuManager = this.barManager1;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(337, 40);
            this.txtDescription.StyleController = this.lcEditorInfo;
            this.txtDescription.TabIndex = 26;
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
            // lkHealthRiskTypeId
            // 
            this.lkHealthRiskTypeId.EnterMoveNextControl = true;
            this.lkHealthRiskTypeId.Location = new System.Drawing.Point(97, 49);
            this.lkHealthRiskTypeId.Name = "lkHealthRiskTypeId";
            this.lkHealthRiskTypeId.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.lkHealthRiskTypeId.Properties.NullText = "";
            this.lkHealthRiskTypeId.Size = new System.Drawing.Size(337, 20);
            this.lkHealthRiskTypeId.StyleController = this.lcEditorInfo;
            this.lkHealthRiskTypeId.TabIndex = 1;
            this.lkHealthRiskTypeId.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.lkHealthRiskTypeId_Closed);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(328, 141);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(106, 26);
            this.btnCancel.StyleController = this.lcEditorInfo;
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "Làm lại (Ctrl C)";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(218, 141);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(106, 26);
            this.btnAdd.StyleController = this.lcEditorInfo;
            this.btnAdd.TabIndex = 6;
            this.btnAdd.Text = "Thêm (Ctrl N)";
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // dnNavigation
            // 
            this.dnNavigation.Buttons.Append.Visible = false;
            this.dnNavigation.Buttons.CancelEdit.Visible = false;
            this.dnNavigation.Buttons.EndEdit.Visible = false;
            this.dnNavigation.Buttons.Remove.Visible = false;
            this.dnNavigation.Location = new System.Drawing.Point(2, 2);
            this.dnNavigation.Name = "dnNavigation";
            this.dnNavigation.Size = new System.Drawing.Size(234, 19);
            this.dnNavigation.StyleController = this.lcEditorInfo;
            this.dnNavigation.TabIndex = 25;
            this.dnNavigation.Text = "dataNavigator1";
            this.dnNavigation.TextLocation = DevExpress.XtraEditors.NavigatorButtonsTextLocation.Center;
            this.dnNavigation.TextStringFormat = "Bản ghi {0} / {1}";
            this.dnNavigation.PositionChanged += new System.EventHandler(this.dnNavigation_PositionChanged);
            // 
            // btnEdit
            // 
            this.btnEdit.Location = new System.Drawing.Point(108, 141);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(106, 26);
            this.btnEdit.StyleController = this.lcEditorInfo;
            this.btnEdit.TabIndex = 5;
            this.btnEdit.Text = "Sửa (Ctrl S)";
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // chkIsGet
            // 
            this.chkIsGet.EnterMoveNextControl = true;
            this.chkIsGet.Location = new System.Drawing.Point(97, 73);
            this.chkIsGet.Name = "chkIsGet";
            this.chkIsGet.Properties.Caption = "";
            this.chkIsGet.Size = new System.Drawing.Size(119, 19);
            this.chkIsGet.StyleController = this.lcEditorInfo;
            this.chkIsGet.TabIndex = 2;
            // 
            // chkIsQuit
            // 
            this.chkIsQuit.EnterMoveNextControl = true;
            this.chkIsQuit.Location = new System.Drawing.Point(315, 73);
            this.chkIsQuit.Name = "chkIsQuit";
            this.chkIsQuit.Properties.Caption = "";
            this.chkIsQuit.Size = new System.Drawing.Size(119, 19);
            this.chkIsQuit.StyleController = this.lcEditorInfo;
            this.chkIsQuit.TabIndex = 3;
            // 
            // layoutControlGroup4
            // 
            this.layoutControlGroup4.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup4.GroupBordersVisible = false;
            this.layoutControlGroup4.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem8,
            this.layoutControlItem17,
            this.layoutControlItem18,
            this.layoutControlItem20,
            this.lciHealthRiskTypeId,
            this.lciIsGet,
            this.lciDescription,
            this.layoutControlItem6,
            this.emptySpaceItem1,
            this.lciIsQuit,
            this.layoutControlItem7});
            this.layoutControlGroup4.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup4.Name = "Root";
            this.layoutControlGroup4.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup4.Size = new System.Drawing.Size(436, 478);
            this.layoutControlGroup4.TextVisible = false;
            // 
            // layoutControlItem8
            // 
            this.layoutControlItem8.Control = this.dnNavigation;
            this.layoutControlItem8.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem8.Name = "layoutControlItem8";
            this.layoutControlItem8.Size = new System.Drawing.Size(436, 23);
            this.layoutControlItem8.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem8.TextVisible = false;
            this.layoutControlItem8.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            // 
            // layoutControlItem17
            // 
            this.layoutControlItem17.Control = this.btnEdit;
            this.layoutControlItem17.Location = new System.Drawing.Point(106, 139);
            this.layoutControlItem17.MaxSize = new System.Drawing.Size(0, 30);
            this.layoutControlItem17.MinSize = new System.Drawing.Size(69, 30);
            this.layoutControlItem17.Name = "layoutControlItem17";
            this.layoutControlItem17.Size = new System.Drawing.Size(110, 30);
            this.layoutControlItem17.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem17.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem17.TextVisible = false;
            // 
            // layoutControlItem18
            // 
            this.layoutControlItem18.Control = this.btnAdd;
            this.layoutControlItem18.Location = new System.Drawing.Point(216, 139);
            this.layoutControlItem18.MaxSize = new System.Drawing.Size(0, 30);
            this.layoutControlItem18.MinSize = new System.Drawing.Size(77, 30);
            this.layoutControlItem18.Name = "layoutControlItem18";
            this.layoutControlItem18.Size = new System.Drawing.Size(110, 30);
            this.layoutControlItem18.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem18.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem18.TextVisible = false;
            // 
            // layoutControlItem20
            // 
            this.layoutControlItem20.Control = this.btnCancel;
            this.layoutControlItem20.Location = new System.Drawing.Point(326, 139);
            this.layoutControlItem20.MaxSize = new System.Drawing.Size(0, 30);
            this.layoutControlItem20.MinSize = new System.Drawing.Size(83, 30);
            this.layoutControlItem20.Name = "layoutControlItem20";
            this.layoutControlItem20.Size = new System.Drawing.Size(110, 30);
            this.layoutControlItem20.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem20.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem20.TextVisible = false;
            // 
            // lciHealthRiskTypeId
            // 
            this.lciHealthRiskTypeId.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.lciHealthRiskTypeId.AppearanceItemCaption.Options.UseForeColor = true;
            this.lciHealthRiskTypeId.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciHealthRiskTypeId.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciHealthRiskTypeId.Control = this.lkHealthRiskTypeId;
            this.lciHealthRiskTypeId.Location = new System.Drawing.Point(0, 47);
            this.lciHealthRiskTypeId.Name = "lciHealthRiskTypeId";
            this.lciHealthRiskTypeId.Size = new System.Drawing.Size(436, 24);
            this.lciHealthRiskTypeId.Text = "Loại:";
            this.lciHealthRiskTypeId.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciHealthRiskTypeId.TextSize = new System.Drawing.Size(90, 20);
            this.lciHealthRiskTypeId.TextToControlDistance = 5;
            // 
            // lciIsGet
            // 
            this.lciIsGet.AppearanceItemCaption.Options.UseForeColor = true;
            this.lciIsGet.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciIsGet.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciIsGet.Control = this.chkIsGet;
            this.lciIsGet.Location = new System.Drawing.Point(0, 71);
            this.lciIsGet.Name = "lciIsGet";
            this.lciIsGet.Size = new System.Drawing.Size(218, 24);
            this.lciIsGet.Text = "Mắc phải";
            this.lciIsGet.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciIsGet.TextSize = new System.Drawing.Size(90, 20);
            this.lciIsGet.TextToControlDistance = 5;
            // 
            // lciDescription
            // 
            this.lciDescription.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciDescription.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciDescription.Control = this.txtDescription;
            this.lciDescription.Location = new System.Drawing.Point(0, 95);
            this.lciDescription.Name = "lciDescription";
            this.lciDescription.Size = new System.Drawing.Size(436, 44);
            this.lciDescription.Text = "Mô tả:";
            this.lciDescription.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciDescription.TextSize = new System.Drawing.Size(90, 20);
            this.lciDescription.TextToControlDistance = 5;
            // 
            // layoutControlItem6
            // 
            this.layoutControlItem6.Control = this.layoutControl1;
            this.layoutControlItem6.Location = new System.Drawing.Point(0, 169);
            this.layoutControlItem6.Name = "layoutControlItem6";
            this.layoutControlItem6.Size = new System.Drawing.Size(436, 309);
            this.layoutControlItem6.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem6.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(0, 139);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(106, 30);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // lciIsQuit
            // 
            this.lciIsQuit.AppearanceItemCaption.Options.UseForeColor = true;
            this.lciIsQuit.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciIsQuit.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciIsQuit.Control = this.chkIsQuit;
            this.lciIsQuit.Location = new System.Drawing.Point(218, 71);
            this.lciIsQuit.Name = "lciIsQuit";
            this.lciIsQuit.Size = new System.Drawing.Size(218, 24);
            this.lciIsQuit.Text = "Đã bỏ";
            this.lciIsQuit.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciIsQuit.TextSize = new System.Drawing.Size(90, 20);
            this.lciIsQuit.TextToControlDistance = 5;
            // 
            // layoutControlItem7
            // 
            this.layoutControlItem7.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem7.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem7.Control = this.lblPatientCode;
            this.layoutControlItem7.Location = new System.Drawing.Point(0, 23);
            this.layoutControlItem7.Name = "layoutControlItem7";
            this.layoutControlItem7.Size = new System.Drawing.Size(436, 24);
            this.layoutControlItem7.Text = "Mã BN:";
            this.layoutControlItem7.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem7.TextSize = new System.Drawing.Size(90, 20);
            this.layoutControlItem7.TextToControlDistance = 5;
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
            this.layoutControlItem4.Location = new System.Drawing.Point(796, 0);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Size = new System.Drawing.Size(440, 482);
            this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem4.TextVisible = false;
            // 
            // layoutControlItem5
            // 
            this.layoutControlItem5.Control = this.layoutControl7;
            this.layoutControlItem5.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem5.Name = "layoutControlItem5";
            this.layoutControlItem5.Size = new System.Drawing.Size(796, 482);
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
            // frmScnHealthRisk
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
            this.Name = "frmScnHealthRisk";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Danh mục";
            this.Load += new System.EventHandler(this.frmScnHealthRisk_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl4)).EndInit();
            this.layoutControl4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl7)).EndInit();
            this.layoutControl7.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControlFormList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridviewFormList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheck__IsGet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheck__IsQuit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnGDelete__Enable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnGDelete__Disable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtKeyword.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcEditorInfo)).EndInit();
            this.lcEditorInfo.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDescription.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lkHealthRiskTypeId.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkIsGet.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkIsQuit.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem17)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem18)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem20)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciHealthRiskTypeId)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciIsGet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDescription)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciIsQuit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).EndInit();
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
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit btnGDelete__Enable;
        private DevExpress.XtraGrid.Columns.GridColumn grdColModifyTime;
        private DevExpress.XtraGrid.Columns.GridColumn grdColModifier;
private DevExpress.XtraEditors.LookUpEdit lkHealthRiskTypeId;
private DevExpress.XtraLayout.LayoutControlItem lciHealthRiskTypeId;
private DevExpress.XtraEditors.CheckEdit chkIsGet;
private DevExpress.XtraLayout.LayoutControlItem lciIsGet;
private DevExpress.XtraEditors.CheckEdit chkIsQuit;
private DevExpress.XtraLayout.LayoutControlItem lciIsQuit;

        private DevExpress.XtraGrid.Columns.GridColumn grdColPatientCode;
private DevExpress.XtraGrid.Columns.GridColumn grdColHealthRiskTypeId;
private DevExpress.XtraGrid.Columns.GridColumn grdColIsGet;
private DevExpress.XtraGrid.Columns.GridColumn grdColIsQuit;
private DevExpress.XtraGrid.Columns.GridColumn grdColDescription;


        private DevExpress.XtraGrid.Columns.GridColumn grdColCreator;
        private DevExpress.XtraGrid.Columns.GridColumn grdColCreateTime;
        private Inventec.UC.Paging.UcPaging ucPaging;
        private DevExpress.XtraEditors.TextEdit txtKeyword;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup6;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControl lcEditorInfo;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
        private DevExpress.XtraEditors.SimpleButton btnAdd;
        private DevExpress.XtraEditors.DataNavigator dnNavigation;
        private DevExpress.XtraEditors.SimpleButton btnEdit;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup4;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem8;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem17;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem18;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem20;
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
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraEditors.MemoEdit txtDescription;
        private DevExpress.XtraLayout.LayoutControlItem lciDescription;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem6;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit btnGDelete__Disable;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheck__IsGet;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheck__IsQuit;
        private DevExpress.XtraEditors.LabelControl lblPatientCode;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem7;
    }
}
