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
namespace HIS.Desktop.Plugins.ServiceDefaultPaty.frmServiceDefaultPaty
{
    partial class frmServiceDefaultPaty
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
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject1 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject2 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject3 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject4 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject5 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject6 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject7 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject8 = new DevExpress.Utils.SerializableAppearanceObject();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmServiceDefaultPaty));
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject9 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject10 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject11 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject12 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject13 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject14 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject15 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject16 = new DevExpress.Utils.SerializableAppearanceObject();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.ucPaging = new Inventec.UC.Paging.UcPaging();
            this.btnReset = new DevExpress.XtraEditors.SimpleButton();
            this.btnSave = new DevExpress.XtraEditors.SimpleButton();
            this.btnEdit = new DevExpress.XtraEditors.SimpleButton();
            this.txtServiceCode = new DevExpress.XtraEditors.TextEdit();
            this.grcListConfig = new DevExpress.XtraGrid.GridControl();
            this.grvListConfig = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gcStt = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcLock = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcDelete = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcServiceCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcServiceName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcPatientType = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcPrimaryPatientType = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcDefaultPatientType = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcCreateTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcCreator = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcModifyTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcModifier = new DevExpress.XtraGrid.Columns.GridColumn();
            this.btnGLock = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.btnGUnlock = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.btnEDelete = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.btnDDelete = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.btnSearch = new DevExpress.XtraEditors.SimpleButton();
            this.txtSearchValue = new DevExpress.XtraEditors.TextEdit();
            this.cboServiceName = new DevExpress.XtraEditors.LookUpEdit();
            this.cboPatientType = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridViewPatientType = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.cboPrimaryPatientType = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridViewPrimaryPatientType = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.cboDefaultPatientType = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridViewDefaultPatientType = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciSearchValue = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciGridList = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciSearch = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciServiceCode = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciServiceName = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciPatientType = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciPrimaryPatientType = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciDefaultPatientType = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciEdit = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciSave = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciReset = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciPaging = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.emptySpaceItem3 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.dxErrorProvider1 = new DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtServiceCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grcListConfig)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grvListConfig)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnGLock)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnGUnlock)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnEDelete)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnDDelete)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSearchValue.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboServiceName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboPatientType.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewPatientType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboPrimaryPatientType.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewPrimaryPatientType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboDefaultPatientType.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewDefaultPatientType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciSearchValue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciGridList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciSearch)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciServiceCode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciServiceName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPatientType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPrimaryPatientType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDefaultPatientType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciEdit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciSave)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciReset)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPaging)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxErrorProvider1)).BeginInit();
            this.SuspendLayout();
            //
            // layoutControl1
            //
            this.layoutControl1.Controls.Add(this.ucPaging);
            this.layoutControl1.Controls.Add(this.btnReset);
            this.layoutControl1.Controls.Add(this.btnSave);
            this.layoutControl1.Controls.Add(this.btnEdit);
            this.layoutControl1.Controls.Add(this.txtServiceCode);
            this.layoutControl1.Controls.Add(this.grcListConfig);
            this.layoutControl1.Controls.Add(this.btnSearch);
            this.layoutControl1.Controls.Add(this.txtSearchValue);
            this.layoutControl1.Controls.Add(this.cboServiceName);
            this.layoutControl1.Controls.Add(this.cboPatientType);
            this.layoutControl1.Controls.Add(this.cboPrimaryPatientType);
            this.layoutControl1.Controls.Add(this.cboDefaultPatientType);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(3, 3);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(1027, 418);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            //
            // ucPaging
            //
            this.ucPaging.Location = new System.Drawing.Point(2, 389);
            this.ucPaging.Name = "ucPaging";
            this.ucPaging.Padding = new System.Windows.Forms.Padding(1);
            this.ucPaging.Size = new System.Drawing.Size(709, 27);
            this.ucPaging.TabIndex = 16;
            //
            // btnReset
            //
            this.btnReset.Location = new System.Drawing.Point(946, 98);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(79, 22);
            this.btnReset.StyleController = this.layoutControl1;
            this.btnReset.TabIndex = 15;
            this.btnReset.Text = "Làm lại (Ctrl R)";
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            //
            // btnSave
            //
            this.btnSave.Location = new System.Drawing.Point(869, 98);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(73, 22);
            this.btnSave.StyleController = this.layoutControl1;
            this.btnSave.TabIndex = 14;
            this.btnSave.Text = "Thêm (Ctrl N)";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            //
            // btnEdit
            //
            this.btnEdit.Location = new System.Drawing.Point(778, 98);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(87, 22);
            this.btnEdit.StyleController = this.layoutControl1;
            this.btnEdit.TabIndex = 13;
            this.btnEdit.Text = "Sửa (Ctrl S)";
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            //
            // txtServiceCode
            //
            this.txtServiceCode.Location = new System.Drawing.Point(790, 2);
            this.txtServiceCode.Name = "txtServiceCode";
            this.txtServiceCode.Size = new System.Drawing.Size(77, 20);
            this.txtServiceCode.StyleController = this.layoutControl1;
            this.txtServiceCode.TabIndex = 7;
            this.txtServiceCode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtServiceCode_KeyDown);
            this.txtServiceCode.Validated += new System.EventHandler(this.txtServiceCode_Validated);
            //
            // grcListConfig
            //
            this.grcListConfig.Location = new System.Drawing.Point(2, 28);
            this.grcListConfig.MainView = this.grvListConfig;
            this.grcListConfig.Name = "grcListConfig";
            this.grcListConfig.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.btnGLock,
            this.btnGUnlock,
            this.btnEDelete,
            this.btnDDelete});
            this.grcListConfig.Size = new System.Drawing.Size(709, 357);
            this.grcListConfig.TabIndex = 6;
            this.grcListConfig.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.grvListConfig});
            //
            // grvListConfig
            //
            this.grvListConfig.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gcStt,
            this.gcLock,
            this.gcDelete,
            this.gcServiceCode,
            this.gcServiceName,
            this.gcPatientType,
            this.gcPrimaryPatientType,
            this.gcDefaultPatientType,
            this.gcCreateTime,
            this.gcCreator,
            this.gcModifyTime,
            this.gcModifier});
            this.grvListConfig.GridControl = this.grcListConfig;
            this.grvListConfig.HorzScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Always;
            this.grvListConfig.Name = "grvListConfig";
            this.grvListConfig.OptionsView.ColumnAutoWidth = false;
            this.grvListConfig.OptionsView.ShowGroupPanel = false;
            this.grvListConfig.OptionsView.ShowHorizontalLines = DevExpress.Utils.DefaultBoolean.True;
            this.grvListConfig.RowCellClick += new DevExpress.XtraGrid.Views.Grid.RowCellClickEventHandler(this.grvListConfig_RowCellClick);
            this.grvListConfig.CustomRowCellEdit += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.grvListConfig_CustomRowCellEdit);
            this.grvListConfig.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.grvListConfig_CustomUnboundColumnData);
            //
            // gcStt
            //
            this.gcStt.Caption = "STT";
            this.gcStt.FieldName = "STT";
            this.gcStt.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gcStt.Name = "gcStt";
            this.gcStt.OptionsColumn.AllowEdit = false;
            this.gcStt.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gcStt.Visible = true;
            this.gcStt.VisibleIndex = 0;
            this.gcStt.Width = 40;
            //
            // gcLock
            //
            this.gcLock.Caption = "LOCK";
            this.gcLock.FieldName = "LOCK";
            this.gcLock.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gcLock.Name = "gcLock";
            this.gcLock.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.gcLock.OptionsColumn.ShowCaption = false;
            this.gcLock.Visible = true;
            this.gcLock.VisibleIndex = 1;
            this.gcLock.Width = 30;
            //
            // gcDelete
            //
            this.gcDelete.Caption = "DELETE";
            this.gcDelete.FieldName = "DELETE";
            this.gcDelete.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gcDelete.Name = "gcDelete";
            this.gcDelete.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.gcDelete.OptionsColumn.ShowCaption = false;
            this.gcDelete.Visible = true;
            this.gcDelete.VisibleIndex = 2;
            this.gcDelete.Width = 30;
            //
            // gcServiceCode
            //
            this.gcServiceCode.Caption = "Mã dịch vụ";
            this.gcServiceCode.FieldName = "SERVICE_CODE";
            this.gcServiceCode.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gcServiceCode.Name = "gcServiceCode";
            this.gcServiceCode.OptionsColumn.AllowEdit = false;
            this.gcServiceCode.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.gcServiceCode.Visible = true;
            this.gcServiceCode.VisibleIndex = 3;
            this.gcServiceCode.Width = 80;
            //
            // gcServiceName
            //
            this.gcServiceName.Caption = "Tên dịch vụ";
            this.gcServiceName.FieldName = "SERVICE_NAME";
            this.gcServiceName.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gcServiceName.Name = "gcServiceName";
            this.gcServiceName.OptionsColumn.AllowEdit = false;
            this.gcServiceName.Visible = true;
            this.gcServiceName.VisibleIndex = 4;
            this.gcServiceName.Width = 170;
            //
            // gcPatientType
            //
            this.gcPatientType.Caption = "ĐT bệnh nhân";
            this.gcPatientType.FieldName = "PATIENT_TYPE_NAME_STR";
            this.gcPatientType.Name = "gcPatientType";
            this.gcPatientType.OptionsColumn.AllowEdit = false;
            this.gcPatientType.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.gcPatientType.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gcPatientType.Visible = true;
            this.gcPatientType.VisibleIndex = 5;
            this.gcPatientType.Width = 110;
            //
            // gcPrimaryPatientType
            //
            this.gcPrimaryPatientType.Caption = "ĐT phụ thu";
            this.gcPrimaryPatientType.FieldName = "PRIMARY_PATIENT_TYPE_NAME_STR";
            this.gcPrimaryPatientType.Name = "gcPrimaryPatientType";
            this.gcPrimaryPatientType.OptionsColumn.AllowEdit = false;
            this.gcPrimaryPatientType.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.gcPrimaryPatientType.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gcPrimaryPatientType.Visible = true;
            this.gcPrimaryPatientType.VisibleIndex = 6;
            this.gcPrimaryPatientType.Width = 110;
            //
            // gcDefaultPatientType
            //
            this.gcDefaultPatientType.Caption = "ĐTTT mặc định";
            this.gcDefaultPatientType.FieldName = "DEFAULT_PATIENT_TYPE_NAME";
            this.gcDefaultPatientType.Name = "gcDefaultPatientType";
            this.gcDefaultPatientType.OptionsColumn.AllowEdit = false;
            this.gcDefaultPatientType.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.gcDefaultPatientType.Visible = true;
            this.gcDefaultPatientType.VisibleIndex = 7;
            this.gcDefaultPatientType.Width = 120;
            //
            // gcCreateTime
            //
            this.gcCreateTime.Caption = "Thời gian tạo";
            this.gcCreateTime.FieldName = "CREATE_TIME_STR";
            this.gcCreateTime.Name = "gcCreateTime";
            this.gcCreateTime.OptionsColumn.AllowEdit = false;
            this.gcCreateTime.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gcCreateTime.Visible = true;
            this.gcCreateTime.VisibleIndex = 8;
            this.gcCreateTime.Width = 120;
            //
            // gcCreator
            //
            this.gcCreator.Caption = "Người tạo";
            this.gcCreator.FieldName = "CREATOR";
            this.gcCreator.Name = "gcCreator";
            this.gcCreator.OptionsColumn.AllowEdit = false;
            this.gcCreator.Visible = true;
            this.gcCreator.VisibleIndex = 9;
            this.gcCreator.Width = 100;
            //
            // gcModifyTime
            //
            this.gcModifyTime.Caption = "Thời gian sửa";
            this.gcModifyTime.FieldName = "MODIFY_TIME_STR";
            this.gcModifyTime.Name = "gcModifyTime";
            this.gcModifyTime.OptionsColumn.AllowEdit = false;
            this.gcModifyTime.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gcModifyTime.Visible = true;
            this.gcModifyTime.VisibleIndex = 10;
            this.gcModifyTime.Width = 120;
            //
            // gcModifier
            //
            this.gcModifier.Caption = "Người sửa";
            this.gcModifier.FieldName = "MODIFIER";
            this.gcModifier.Name = "gcModifier";
            this.gcModifier.OptionsColumn.AllowEdit = false;
            this.gcModifier.Visible = true;
            this.gcModifier.VisibleIndex = 11;
            this.gcModifier.Width = 100;
            //
            // btnGLock
            //
            this.btnGLock.AutoHeight = false;
            this.btnGLock.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, global::HIS.Desktop.Plugins.ServiceDefaultPaty.Properties.Resources.Lock_Lock_icon, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject1, serializableAppearanceObject2, serializableAppearanceObject3, serializableAppearanceObject4, "Mở khóa dữ liệu", null, null, true)});
            this.btnGLock.Name = "btnGLock";
            this.btnGLock.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            this.btnGLock.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.btnGLock_ButtonClick);
            //
            // btnGUnlock
            //
            this.btnGUnlock.AutoHeight = false;
            this.btnGUnlock.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, global::HIS.Desktop.Plugins.ServiceDefaultPaty.Properties.Resources.Lock_Unlock_icon, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject5, serializableAppearanceObject6, serializableAppearanceObject7, serializableAppearanceObject8, "Khóa dữ liệu", null, null, true)});
            this.btnGUnlock.Name = "btnGUnlock";
            this.btnGUnlock.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            this.btnGUnlock.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.btnGUnlock_ButtonClick);
            //
            // btnEDelete
            //
            this.btnEDelete.AutoHeight = false;
            this.btnEDelete.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, ((System.Drawing.Image)(resources.GetObject("btnEDelete.Buttons"))), new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject9, serializableAppearanceObject10, serializableAppearanceObject11, serializableAppearanceObject12, "Xóa dữ liệu", null, null, true)});
            this.btnEDelete.Name = "btnEDelete";
            this.btnEDelete.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            this.btnEDelete.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.btnEDelete_ButtonClick);
            //
            // btnDDelete
            //
            this.btnDDelete.AutoHeight = false;
            this.btnDDelete.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, ((System.Drawing.Image)(resources.GetObject("btnDDelete.Buttons"))), new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject13, serializableAppearanceObject14, serializableAppearanceObject15, serializableAppearanceObject16, "", null, null, true)});
            this.btnDDelete.Name = "btnDDelete";
            this.btnDDelete.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            //
            // btnSearch
            //
            this.btnSearch.Location = new System.Drawing.Point(242, 2);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(110, 22);
            this.btnSearch.StyleController = this.layoutControl1;
            this.btnSearch.TabIndex = 5;
            this.btnSearch.Text = "Tìm (Ctrl F)";
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            //
            // txtSearchValue
            //
            this.txtSearchValue.EditValue = "";
            this.txtSearchValue.Location = new System.Drawing.Point(2, 2);
            this.txtSearchValue.Name = "txtSearchValue";
            this.txtSearchValue.Properties.NullText = "Nhập mã, tên dịch vụ để tìm kiếm";
            this.txtSearchValue.Properties.NullValuePrompt = "Nhập mã, tên dịch vụ để tìm kiếm";
            this.txtSearchValue.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtSearchValue.Size = new System.Drawing.Size(236, 20);
            this.txtSearchValue.StyleController = this.layoutControl1;
            this.txtSearchValue.TabIndex = 4;
            this.txtSearchValue.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSearchValue_KeyDown);
            //
            // cboServiceName
            //
            this.cboServiceName.EnterMoveNextControl = true;
            this.cboServiceName.Location = new System.Drawing.Point(867, 2);
            this.cboServiceName.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.cboServiceName.Name = "cboServiceName";
            this.cboServiceName.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboServiceName.Properties.NullText = "";
            this.cboServiceName.Size = new System.Drawing.Size(158, 20);
            this.cboServiceName.StyleController = this.layoutControl1;
            this.cboServiceName.TabIndex = 8;
            this.cboServiceName.EditValueChanged += new System.EventHandler(this.cboServiceName_EditValueChanged);
            //
            // cboPatientType
            //
            this.cboPatientType.Location = new System.Drawing.Point(790, 26);
            this.cboPatientType.Name = "cboPatientType";
            this.cboPatientType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboPatientType.Properties.NullText = "Tất cả";
            this.cboPatientType.Properties.View = this.gridViewPatientType;
            this.cboPatientType.Size = new System.Drawing.Size(235, 20);
            this.cboPatientType.StyleController = this.layoutControl1;
            this.cboPatientType.TabIndex = 9;
            this.cboPatientType.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cboPatientType_KeyDown);
            //
            // gridViewPatientType
            //
            this.gridViewPatientType.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridViewPatientType.Name = "gridViewPatientType";
            this.gridViewPatientType.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridViewPatientType.OptionsView.ShowGroupPanel = false;
            //
            // cboPrimaryPatientType
            //
            this.cboPrimaryPatientType.Location = new System.Drawing.Point(790, 50);
            this.cboPrimaryPatientType.Name = "cboPrimaryPatientType";
            this.cboPrimaryPatientType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboPrimaryPatientType.Properties.NullText = "Tất cả";
            this.cboPrimaryPatientType.Properties.View = this.gridViewPrimaryPatientType;
            this.cboPrimaryPatientType.Size = new System.Drawing.Size(235, 20);
            this.cboPrimaryPatientType.StyleController = this.layoutControl1;
            this.cboPrimaryPatientType.TabIndex = 10;
            this.cboPrimaryPatientType.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cboPrimaryPatientType_KeyDown);
            //
            // gridViewPrimaryPatientType
            //
            this.gridViewPrimaryPatientType.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridViewPrimaryPatientType.Name = "gridViewPrimaryPatientType";
            this.gridViewPrimaryPatientType.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridViewPrimaryPatientType.OptionsView.ShowGroupPanel = false;
            //
            // cboDefaultPatientType
            //
            this.cboDefaultPatientType.Location = new System.Drawing.Point(790, 74);
            this.cboDefaultPatientType.Name = "cboDefaultPatientType";
            this.cboDefaultPatientType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboDefaultPatientType.Properties.NullText = "";
            this.cboDefaultPatientType.Properties.View = this.gridViewDefaultPatientType;
            this.cboDefaultPatientType.Size = new System.Drawing.Size(235, 20);
            this.cboDefaultPatientType.StyleController = this.layoutControl1;
            this.cboDefaultPatientType.TabIndex = 11;
            this.cboDefaultPatientType.EditValueChanged += new System.EventHandler(this.cboDefaultPatientType_EditValueChanged);
            this.cboDefaultPatientType.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cboDefaultPatientType_KeyDown);
            //
            // gridViewDefaultPatientType
            //
            this.gridViewDefaultPatientType.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridViewDefaultPatientType.Name = "gridViewDefaultPatientType";
            this.gridViewDefaultPatientType.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridViewDefaultPatientType.OptionsView.ShowGroupPanel = false;
            //
            // layoutControlGroup1
            //
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciSearchValue,
            this.lciGridList,
            this.lciSearch,
            this.lciServiceCode,
            this.lciServiceName,
            this.lciPatientType,
            this.lciPrimaryPatientType,
            this.lciDefaultPatientType,
            this.lciEdit,
            this.lciSave,
            this.lciReset,
            this.lciPaging,
            this.emptySpaceItem1,
            this.emptySpaceItem2,
            this.emptySpaceItem3});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(1027, 418);
            this.layoutControlGroup1.TextVisible = false;
            //
            // lciSearchValue
            //
            this.lciSearchValue.Control = this.txtSearchValue;
            this.lciSearchValue.Location = new System.Drawing.Point(0, 0);
            this.lciSearchValue.Name = "lciSearchValue";
            this.lciSearchValue.Size = new System.Drawing.Size(240, 26);
            this.lciSearchValue.TextSize = new System.Drawing.Size(0, 0);
            this.lciSearchValue.TextVisible = false;
            //
            // lciGridList
            //
            this.lciGridList.Control = this.grcListConfig;
            this.lciGridList.Location = new System.Drawing.Point(0, 26);
            this.lciGridList.Name = "lciGridList";
            this.lciGridList.Size = new System.Drawing.Size(713, 361);
            this.lciGridList.TextSize = new System.Drawing.Size(0, 0);
            this.lciGridList.TextVisible = false;
            //
            // lciSearch
            //
            this.lciSearch.Control = this.btnSearch;
            this.lciSearch.Location = new System.Drawing.Point(240, 0);
            this.lciSearch.Name = "lciSearch";
            this.lciSearch.Size = new System.Drawing.Size(114, 26);
            this.lciSearch.TextSize = new System.Drawing.Size(0, 0);
            this.lciSearch.TextVisible = false;
            //
            // lciServiceCode
            //
            this.lciServiceCode.AppearanceItemCaption.ForeColor = System.Drawing.Color.Brown;
            this.lciServiceCode.AppearanceItemCaption.Options.UseForeColor = true;
            this.lciServiceCode.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciServiceCode.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciServiceCode.Control = this.txtServiceCode;
            this.lciServiceCode.Location = new System.Drawing.Point(713, 0);
            this.lciServiceCode.Name = "lciServiceCode";
            this.lciServiceCode.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 0, 2, 2);
            this.lciServiceCode.Size = new System.Drawing.Size(154, 24);
            this.lciServiceCode.Text = "Dịch vụ:";
            this.lciServiceCode.TextSize = new System.Drawing.Size(72, 13);
            //
            // lciServiceName
            //
            this.lciServiceName.Control = this.cboServiceName;
            this.lciServiceName.Location = new System.Drawing.Point(867, 0);
            this.lciServiceName.Name = "lciServiceName";
            this.lciServiceName.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 2, 2, 2);
            this.lciServiceName.Size = new System.Drawing.Size(160, 24);
            this.lciServiceName.TextSize = new System.Drawing.Size(0, 0);
            this.lciServiceName.TextVisible = false;
            //
            // lciPatientType
            //
            this.lciPatientType.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciPatientType.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciPatientType.Control = this.cboPatientType;
            this.lciPatientType.Location = new System.Drawing.Point(713, 24);
            this.lciPatientType.Name = "lciPatientType";
            this.lciPatientType.OptionsToolTip.ToolTip = "Đối tượng của bệnh nhân. Để trống là áp dụng cho mọi đối tượng.";
            this.lciPatientType.Size = new System.Drawing.Size(314, 24);
            this.lciPatientType.Text = "ĐT bệnh nhân:";
            this.lciPatientType.TextSize = new System.Drawing.Size(72, 13);
            //
            // lciPrimaryPatientType
            //
            this.lciPrimaryPatientType.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciPrimaryPatientType.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciPrimaryPatientType.Control = this.cboPrimaryPatientType;
            this.lciPrimaryPatientType.Location = new System.Drawing.Point(713, 48);
            this.lciPrimaryPatientType.Name = "lciPrimaryPatientType";
            this.lciPrimaryPatientType.OptionsToolTip.ToolTip = "Đối tượng phụ thu của bệnh nhân. Để trống là áp dụng cho mọi trường hợp, kể cả hồ sơ không có phụ thu.";
            this.lciPrimaryPatientType.Size = new System.Drawing.Size(314, 24);
            this.lciPrimaryPatientType.Text = "ĐT phụ thu:";
            this.lciPrimaryPatientType.TextSize = new System.Drawing.Size(72, 13);
            //
            // lciDefaultPatientType
            //
            this.lciDefaultPatientType.AppearanceItemCaption.ForeColor = System.Drawing.Color.Brown;
            this.lciDefaultPatientType.AppearanceItemCaption.Options.UseForeColor = true;
            this.lciDefaultPatientType.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciDefaultPatientType.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciDefaultPatientType.Control = this.cboDefaultPatientType;
            this.lciDefaultPatientType.Location = new System.Drawing.Point(713, 72);
            this.lciDefaultPatientType.Name = "lciDefaultPatientType";
            this.lciDefaultPatientType.OptionsToolTip.ToolTip = "Đối tượng thanh toán được điền sẵn khi chỉ định dịch vụ này.";
            this.lciDefaultPatientType.Size = new System.Drawing.Size(314, 24);
            this.lciDefaultPatientType.Text = "ĐTTT mặc định:";
            this.lciDefaultPatientType.TextSize = new System.Drawing.Size(72, 13);
            //
            // lciEdit
            //
            this.lciEdit.Control = this.btnEdit;
            this.lciEdit.Location = new System.Drawing.Point(776, 96);
            this.lciEdit.Name = "lciEdit";
            this.lciEdit.Size = new System.Drawing.Size(91, 26);
            this.lciEdit.TextSize = new System.Drawing.Size(0, 0);
            this.lciEdit.TextVisible = false;
            //
            // lciSave
            //
            this.lciSave.Control = this.btnSave;
            this.lciSave.Location = new System.Drawing.Point(867, 96);
            this.lciSave.Name = "lciSave";
            this.lciSave.Size = new System.Drawing.Size(77, 26);
            this.lciSave.TextSize = new System.Drawing.Size(0, 0);
            this.lciSave.TextVisible = false;
            //
            // lciReset
            //
            this.lciReset.Control = this.btnReset;
            this.lciReset.Location = new System.Drawing.Point(944, 96);
            this.lciReset.Name = "lciReset";
            this.lciReset.Size = new System.Drawing.Size(83, 26);
            this.lciReset.TextSize = new System.Drawing.Size(0, 0);
            this.lciReset.TextVisible = false;
            //
            // lciPaging
            //
            this.lciPaging.Control = this.ucPaging;
            this.lciPaging.Location = new System.Drawing.Point(0, 387);
            this.lciPaging.Name = "lciPaging";
            this.lciPaging.Size = new System.Drawing.Size(713, 31);
            this.lciPaging.TextSize = new System.Drawing.Size(0, 0);
            this.lciPaging.TextVisible = false;
            //
            // emptySpaceItem1
            //
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(713, 122);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(314, 296);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            //
            // emptySpaceItem2
            //
            this.emptySpaceItem2.AllowHotTrack = false;
            this.emptySpaceItem2.Location = new System.Drawing.Point(354, 0);
            this.emptySpaceItem2.Name = "emptySpaceItem2";
            this.emptySpaceItem2.Size = new System.Drawing.Size(359, 26);
            this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
            //
            // emptySpaceItem3
            //
            this.emptySpaceItem3.AllowHotTrack = false;
            this.emptySpaceItem3.Location = new System.Drawing.Point(713, 96);
            this.emptySpaceItem3.Name = "emptySpaceItem3";
            this.emptySpaceItem3.Size = new System.Drawing.Size(63, 26);
            this.emptySpaceItem3.TextSize = new System.Drawing.Size(0, 0);
            //
            // dxErrorProvider1
            //
            this.dxErrorProvider1.ContainerControl = this;
            //
            // frmServiceDefaultPaty
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1033, 424);
            this.Controls.Add(this.layoutControl1);
            this.KeyPreview = true;
            this.Name = "frmServiceDefaultPaty";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Thiết lập đối tượng thanh toán cho dịch vụ";
            this.Load += new System.EventHandler(this.frmServiceDefaultPaty_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmServiceDefaultPaty_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtServiceCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grcListConfig)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grvListConfig)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnGLock)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnGUnlock)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnEDelete)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnDDelete)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSearchValue.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboServiceName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboPatientType.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewPatientType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboPrimaryPatientType.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewPrimaryPatientType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboDefaultPatientType.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewDefaultPatientType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciSearchValue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciGridList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciSearch)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciServiceCode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciServiceName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPatientType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPrimaryPatientType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDefaultPatientType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciEdit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciSave)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciReset)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPaging)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxErrorProvider1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraEditors.SimpleButton btnReset;
        private DevExpress.XtraEditors.SimpleButton btnSave;
        private DevExpress.XtraEditors.SimpleButton btnEdit;
        private DevExpress.XtraEditors.TextEdit txtServiceCode;
        private DevExpress.XtraGrid.GridControl grcListConfig;
        private DevExpress.XtraGrid.Views.Grid.GridView grvListConfig;
        private DevExpress.XtraEditors.SimpleButton btnSearch;
        private DevExpress.XtraEditors.TextEdit txtSearchValue;
        private DevExpress.XtraEditors.LookUpEdit cboServiceName;
        private DevExpress.XtraEditors.GridLookUpEdit cboPatientType;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewPatientType;
        private DevExpress.XtraEditors.GridLookUpEdit cboPrimaryPatientType;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewPrimaryPatientType;
        private DevExpress.XtraEditors.GridLookUpEdit cboDefaultPatientType;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewDefaultPatientType;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraLayout.LayoutControlItem lciSearchValue;
        private DevExpress.XtraLayout.LayoutControlItem lciGridList;
        private DevExpress.XtraLayout.LayoutControlItem lciSearch;
        private DevExpress.XtraLayout.LayoutControlItem lciServiceCode;
        private DevExpress.XtraLayout.LayoutControlItem lciServiceName;
        private DevExpress.XtraLayout.LayoutControlItem lciPatientType;
        private DevExpress.XtraLayout.LayoutControlItem lciPrimaryPatientType;
        private DevExpress.XtraLayout.LayoutControlItem lciDefaultPatientType;
        private DevExpress.XtraLayout.LayoutControlItem lciEdit;
        private DevExpress.XtraLayout.LayoutControlItem lciSave;
        private DevExpress.XtraLayout.LayoutControlItem lciReset;
        private DevExpress.XtraLayout.LayoutControlItem lciPaging;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem3;
        private DevExpress.XtraGrid.Columns.GridColumn gcStt;
        private DevExpress.XtraGrid.Columns.GridColumn gcLock;
        private DevExpress.XtraGrid.Columns.GridColumn gcDelete;
        private DevExpress.XtraGrid.Columns.GridColumn gcServiceCode;
        private DevExpress.XtraGrid.Columns.GridColumn gcServiceName;
        private DevExpress.XtraGrid.Columns.GridColumn gcPatientType;
        private DevExpress.XtraGrid.Columns.GridColumn gcPrimaryPatientType;
        private DevExpress.XtraGrid.Columns.GridColumn gcDefaultPatientType;
        private DevExpress.XtraGrid.Columns.GridColumn gcCreateTime;
        private DevExpress.XtraGrid.Columns.GridColumn gcCreator;
        private DevExpress.XtraGrid.Columns.GridColumn gcModifyTime;
        private DevExpress.XtraGrid.Columns.GridColumn gcModifier;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit btnGLock;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit btnGUnlock;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit btnEDelete;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit btnDDelete;
        private Inventec.UC.Paging.UcPaging ucPaging;
        private DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider dxErrorProvider1;
    }
}
