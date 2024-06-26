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
namespace HIS.Desktop.Plugins.MetyMaty.Change
{
    partial class frmChangePrepa
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
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject9 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject10 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject11 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject12 = new DevExpress.Utils.SerializableAppearanceObject();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.btnSave = new DevExpress.XtraEditors.SimpleButton();
            this.gridControlMaty = new DevExpress.XtraGrid.GridControl();
            this.gridViewMaty = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.Gc_Maty_Stt = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Gc_Maty_Delete = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemButton__Delete = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.Gc_Maty_MaterialTypeCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Gc_Maty_MaterialTypeName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Gc_Maty_Amount = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemSpin__Amount = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
            this.cboPrepaMaty = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridLookUpEdit1View = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.txtPrepaMatyCode = new DevExpress.XtraEditors.TextEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciPrepaMaty = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.barManager1 = new DevExpress.XtraBars.BarManager();
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.barBtnSave = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.dxValidationProvider1 = new DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlMaty)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewMaty)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemButton__Delete)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSpin__Amount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboPrepaMaty.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPrepaMatyCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPrepaMaty)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.btnSave);
            this.layoutControl1.Controls.Add(this.gridControlMaty);
            this.layoutControl1.Controls.Add(this.cboPrepaMaty);
            this.layoutControl1.Controls.Add(this.txtPrepaMatyCode);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 29);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.OptionsView.UseDefaultDragAndDropRendering = false;
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(663, 432);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(552, 408);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(109, 22);
            this.btnSave.StyleController = this.layoutControl1;
            this.btnSave.TabIndex = 7;
            this.btnSave.Text = "Lưu (Ctrl S)";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // gridControlMaty
            // 
            this.gridControlMaty.Location = new System.Drawing.Point(0, 24);
            this.gridControlMaty.MainView = this.gridViewMaty;
            this.gridControlMaty.Name = "gridControlMaty";
            this.gridControlMaty.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemButton__Delete,
            this.repositoryItemSpin__Amount});
            this.gridControlMaty.Size = new System.Drawing.Size(663, 382);
            this.gridControlMaty.TabIndex = 6;
            this.gridControlMaty.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewMaty});
            // 
            // gridViewMaty
            // 
            this.gridViewMaty.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.Gc_Maty_Stt,
            this.Gc_Maty_Delete,
            this.Gc_Maty_MaterialTypeCode,
            this.Gc_Maty_MaterialTypeName,
            this.Gc_Maty_Amount});
            this.gridViewMaty.GridControl = this.gridControlMaty;
            this.gridViewMaty.Name = "gridViewMaty";
            this.gridViewMaty.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            this.gridViewMaty.OptionsView.ShowGroupPanel = false;
            this.gridViewMaty.OptionsView.ShowIndicator = false;
            this.gridViewMaty.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewMaty_CustomUnboundColumnData);
            // 
            // Gc_Maty_Stt
            // 
            this.Gc_Maty_Stt.AppearanceCell.Options.UseTextOptions = true;
            this.Gc_Maty_Stt.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.Gc_Maty_Stt.AppearanceHeader.Options.UseTextOptions = true;
            this.Gc_Maty_Stt.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.Gc_Maty_Stt.Caption = "STT";
            this.Gc_Maty_Stt.FieldName = "STT";
            this.Gc_Maty_Stt.Name = "Gc_Maty_Stt";
            this.Gc_Maty_Stt.OptionsColumn.AllowEdit = false;
            this.Gc_Maty_Stt.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.Gc_Maty_Stt.OptionsColumn.ReadOnly = true;
            this.Gc_Maty_Stt.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.Gc_Maty_Stt.Visible = true;
            this.Gc_Maty_Stt.VisibleIndex = 0;
            this.Gc_Maty_Stt.Width = 40;
            // 
            // Gc_Maty_Delete
            // 
            this.Gc_Maty_Delete.AppearanceCell.Options.UseTextOptions = true;
            this.Gc_Maty_Delete.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.Gc_Maty_Delete.AppearanceHeader.Options.UseTextOptions = true;
            this.Gc_Maty_Delete.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.Gc_Maty_Delete.Caption = "Xóa";
            this.Gc_Maty_Delete.ColumnEdit = this.repositoryItemButton__Delete;
            this.Gc_Maty_Delete.FieldName = "Delete";
            this.Gc_Maty_Delete.MaxWidth = 25;
            this.Gc_Maty_Delete.Name = "Gc_Maty_Delete";
            this.Gc_Maty_Delete.OptionsColumn.ShowCaption = false;
            this.Gc_Maty_Delete.Visible = true;
            this.Gc_Maty_Delete.VisibleIndex = 1;
            this.Gc_Maty_Delete.Width = 20;
            // 
            // repositoryItemButton__Delete
            // 
            this.repositoryItemButton__Delete.AutoHeight = false;
            this.repositoryItemButton__Delete.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete, "", -1, true, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, null, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject9, serializableAppearanceObject10, serializableAppearanceObject11, serializableAppearanceObject12, "Không thay đổi chế phẩm", null, null, true)});
            this.repositoryItemButton__Delete.Name = "repositoryItemButton__Delete";
            this.repositoryItemButton__Delete.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            this.repositoryItemButton__Delete.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repositoryItemButton__Delete_ButtonClick);
            // 
            // Gc_Maty_MaterialTypeCode
            // 
            this.Gc_Maty_MaterialTypeCode.AppearanceHeader.Options.UseTextOptions = true;
            this.Gc_Maty_MaterialTypeCode.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.Gc_Maty_MaterialTypeCode.Caption = "Mã thuốc thành phẩm";
            this.Gc_Maty_MaterialTypeCode.FieldName = "MEDICINE_TYPE_CODE";
            this.Gc_Maty_MaterialTypeCode.Name = "Gc_Maty_MaterialTypeCode";
            this.Gc_Maty_MaterialTypeCode.OptionsColumn.AllowEdit = false;
            this.Gc_Maty_MaterialTypeCode.Visible = true;
            this.Gc_Maty_MaterialTypeCode.VisibleIndex = 2;
            this.Gc_Maty_MaterialTypeCode.Width = 140;
            // 
            // Gc_Maty_MaterialTypeName
            // 
            this.Gc_Maty_MaterialTypeName.AppearanceHeader.Options.UseTextOptions = true;
            this.Gc_Maty_MaterialTypeName.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.Gc_Maty_MaterialTypeName.Caption = "Tên thuốc thành phẩm";
            this.Gc_Maty_MaterialTypeName.FieldName = "MEDICINE_TYPE_NAME";
            this.Gc_Maty_MaterialTypeName.Name = "Gc_Maty_MaterialTypeName";
            this.Gc_Maty_MaterialTypeName.OptionsColumn.AllowEdit = false;
            this.Gc_Maty_MaterialTypeName.Visible = true;
            this.Gc_Maty_MaterialTypeName.VisibleIndex = 3;
            this.Gc_Maty_MaterialTypeName.Width = 300;
            // 
            // Gc_Maty_Amount
            // 
            this.Gc_Maty_Amount.AppearanceCell.Options.UseTextOptions = true;
            this.Gc_Maty_Amount.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.Gc_Maty_Amount.AppearanceHeader.Options.UseTextOptions = true;
            this.Gc_Maty_Amount.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.Gc_Maty_Amount.Caption = "Số lượng";
            this.Gc_Maty_Amount.ColumnEdit = this.repositoryItemSpin__Amount;
            this.Gc_Maty_Amount.FieldName = "MATERIAL_TYPE_AMOUNT";
            this.Gc_Maty_Amount.Name = "Gc_Maty_Amount";
            this.Gc_Maty_Amount.Visible = true;
            this.Gc_Maty_Amount.VisibleIndex = 4;
            this.Gc_Maty_Amount.Width = 160;
            // 
            // repositoryItemSpin__Amount
            // 
            this.repositoryItemSpin__Amount.AutoHeight = false;
            this.repositoryItemSpin__Amount.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemSpin__Amount.Mask.EditMask = "f2";
            this.repositoryItemSpin__Amount.MaxValue = new decimal(new int[] {
            999999999,
            0,
            0,
            0});
            this.repositoryItemSpin__Amount.Name = "repositoryItemSpin__Amount";
            // 
            // cboPrepaMaty
            // 
            this.cboPrepaMaty.EditValue = "";
            this.cboPrepaMaty.Location = new System.Drawing.Point(250, 2);
            this.cboPrepaMaty.Name = "cboPrepaMaty";
            this.cboPrepaMaty.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
            this.cboPrepaMaty.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboPrepaMaty.Properties.NullText = "";
            this.cboPrepaMaty.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
            this.cboPrepaMaty.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
            this.cboPrepaMaty.Properties.View = this.gridLookUpEdit1View;
            this.cboPrepaMaty.Size = new System.Drawing.Size(411, 20);
            this.cboPrepaMaty.StyleController = this.layoutControl1;
            this.cboPrepaMaty.TabIndex = 5;
            this.cboPrepaMaty.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboPrepaMaty_Closed);
            this.cboPrepaMaty.EditValueChanged += new System.EventHandler(this.cboPrepaMaty_EditValueChanged);
            // 
            // gridLookUpEdit1View
            // 
            this.gridLookUpEdit1View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridLookUpEdit1View.Name = "gridLookUpEdit1View";
            this.gridLookUpEdit1View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridLookUpEdit1View.OptionsView.ShowGroupPanel = false;
            // 
            // txtPrepaMatyCode
            // 
            this.txtPrepaMatyCode.Location = new System.Drawing.Point(157, 2);
            this.txtPrepaMatyCode.Name = "txtPrepaMatyCode";
            this.txtPrepaMatyCode.Size = new System.Drawing.Size(93, 20);
            this.txtPrepaMatyCode.StyleController = this.layoutControl1;
            this.txtPrepaMatyCode.TabIndex = 4;
            this.txtPrepaMatyCode.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtPrepaMatyCode_PreviewKeyDown);
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciPrepaMaty,
            this.layoutControlItem2,
            this.layoutControlItem3,
            this.layoutControlItem4,
            this.emptySpaceItem1});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(663, 432);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // lciPrepaMaty
            // 
            this.lciPrepaMaty.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.lciPrepaMaty.AppearanceItemCaption.Options.UseForeColor = true;
            this.lciPrepaMaty.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciPrepaMaty.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciPrepaMaty.Control = this.txtPrepaMatyCode;
            this.lciPrepaMaty.Location = new System.Drawing.Point(0, 0);
            this.lciPrepaMaty.Name = "lciPrepaMaty";
            this.lciPrepaMaty.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 0, 2, 2);
            this.lciPrepaMaty.Size = new System.Drawing.Size(250, 24);
            this.lciPrepaMaty.Text = "Chế phẩm mới:";
            this.lciPrepaMaty.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciPrepaMaty.TextSize = new System.Drawing.Size(150, 20);
            this.lciPrepaMaty.TextToControlDistance = 5;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.cboPrepaMaty;
            this.layoutControlItem2.Location = new System.Drawing.Point(250, 0);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 2, 2, 2);
            this.layoutControlItem2.Size = new System.Drawing.Size(413, 24);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.gridControlMaty;
            this.layoutControlItem3.Location = new System.Drawing.Point(0, 24);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlItem3.Size = new System.Drawing.Size(663, 382);
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextVisible = false;
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.Control = this.btnSave;
            this.layoutControlItem4.Location = new System.Drawing.Point(550, 406);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Size = new System.Drawing.Size(113, 26);
            this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem4.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(0, 406);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(550, 26);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
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
            this.barBtnSave});
            this.barManager1.MaxItemId = 1;
            // 
            // bar1
            // 
            this.bar1.BarName = "Tools";
            this.bar1.DockCol = 0;
            this.bar1.DockRow = 0;
            this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.barBtnSave)});
            this.bar1.Text = "Tools";
            this.bar1.Visible = false;
            // 
            // barBtnSave
            // 
            this.barBtnSave.Caption = "Lưu (Ctrl S)";
            this.barBtnSave.Id = 0;
            this.barBtnSave.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S));
            this.barBtnSave.Name = "barBtnSave";
            this.barBtnSave.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barBtnSave_ItemClick);
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Size = new System.Drawing.Size(663, 29);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 461);
            this.barDockControlBottom.Size = new System.Drawing.Size(663, 0);
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
            this.barDockControlRight.Location = new System.Drawing.Point(663, 29);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 432);
            // 
            // dxValidationProvider1
            // 
            this.dxValidationProvider1.ValidationFailed += new DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventHandler(this.dxValidationProvider1_ValidationFailed);
            // 
            // frmChangePrepa
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(663, 461);
            this.Controls.Add(this.layoutControl1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Name = "frmChangePrepa";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Chuyển đối chế phẩm định mức đóng gói";
            this.Load += new System.EventHandler(this.frmChangePrepa_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControlMaty)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewMaty)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemButton__Delete)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSpin__Amount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboPrepaMaty.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPrepaMatyCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPrepaMaty)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.SimpleButton btnSave;
        private DevExpress.XtraGrid.GridControl gridControlMaty;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewMaty;
        private DevExpress.XtraEditors.GridLookUpEdit cboPrepaMaty;
        private DevExpress.XtraGrid.Views.Grid.GridView gridLookUpEdit1View;
        private DevExpress.XtraEditors.TextEdit txtPrepaMatyCode;
        private DevExpress.XtraLayout.LayoutControlItem lciPrepaMaty;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraGrid.Columns.GridColumn Gc_Maty_Stt;
        private DevExpress.XtraGrid.Columns.GridColumn Gc_Maty_Delete;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit repositoryItemButton__Delete;
        private DevExpress.XtraGrid.Columns.GridColumn Gc_Maty_MaterialTypeCode;
        private DevExpress.XtraGrid.Columns.GridColumn Gc_Maty_MaterialTypeName;
        private DevExpress.XtraGrid.Columns.GridColumn Gc_Maty_Amount;
        private DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit repositoryItemSpin__Amount;
        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.Bar bar1;
        private DevExpress.XtraBars.BarButtonItem barBtnSave;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProvider1;
    }
}
