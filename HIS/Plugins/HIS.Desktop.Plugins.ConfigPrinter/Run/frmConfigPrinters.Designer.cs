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
namespace HIS.Desktop.Plugins.ConfigPrinter.Run
{
    partial class frmConfigPrinters
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
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject9 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject10 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject11 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject12 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject13 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject14 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject15 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject16 = new DevExpress.Utils.SerializableAppearanceObject();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.btnSave = new DevExpress.XtraEditors.SimpleButton();
            this.gridControlPrintType = new DevExpress.XtraGrid.GridControl();
            this.gridViewPrintType = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn_PrintType_PrintTypeCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemTextEdit__PrintTypeCode = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
            this.gridColumn_PrintType_PrintTypeName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemGridLookUp__PrintType = new DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit();
            this.repositoryItemGridLookUpEdit1View = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn_PrintType_PrinterName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemCboBoxPrinter = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
            this.gridColumnAdd = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemButton__Add = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.repositoryItemButton__Delete = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.barButtonItem_Save = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlPrintType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewPrintType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemTextEdit__PrintTypeCode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemGridLookUp__PrintType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemGridLookUpEdit1View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCboBoxPrinter)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemButton__Add)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemButton__Delete)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.btnSave);
            this.layoutControl1.Controls.Add(this.gridControlPrintType);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 38);
            this.layoutControl1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(793, 140, 250, 350);
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(880, 529);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(736, 499);
            this.btnSave.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(141, 27);
            this.btnSave.StyleController = this.layoutControl1;
            this.btnSave.TabIndex = 6;
            this.btnSave.Text = "Lưu (Ctrl S)";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // gridControlPrintType
            // 
            this.gridControlPrintType.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.gridControlPrintType.Location = new System.Drawing.Point(3, 3);
            this.gridControlPrintType.MainView = this.gridViewPrintType;
            this.gridControlPrintType.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.gridControlPrintType.Name = "gridControlPrintType";
            this.gridControlPrintType.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemCboBoxPrinter,
            this.repositoryItemButton__Add,
            this.repositoryItemButton__Delete,
            this.repositoryItemGridLookUp__PrintType,
            this.repositoryItemTextEdit__PrintTypeCode});
            this.gridControlPrintType.Size = new System.Drawing.Size(874, 490);
            this.gridControlPrintType.TabIndex = 5;
            this.gridControlPrintType.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewPrintType});
            // 
            // gridViewPrintType
            // 
            this.gridViewPrintType.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn_PrintType_PrintTypeCode,
            this.gridColumn_PrintType_PrintTypeName,
            this.gridColumn_PrintType_PrinterName,
            this.gridColumnAdd});
            this.gridViewPrintType.GridControl = this.gridControlPrintType;
            this.gridViewPrintType.Name = "gridViewPrintType";
            this.gridViewPrintType.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            this.gridViewPrintType.OptionsView.ShowGroupPanel = false;
            this.gridViewPrintType.OptionsView.ShowIndicator = false;
            this.gridViewPrintType.CustomRowCellEdit += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gridViewPrintType_CustomRowCellEdit);
            // 
            // gridColumn_PrintType_PrintTypeCode
            // 
            this.gridColumn_PrintType_PrintTypeCode.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_PrintType_PrintTypeCode.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_PrintType_PrintTypeCode.Caption = "Mã loại in";
            this.gridColumn_PrintType_PrintTypeCode.ColumnEdit = this.repositoryItemTextEdit__PrintTypeCode;
            this.gridColumn_PrintType_PrintTypeCode.FieldName = "PRINT_TYPE_CODE";
            this.gridColumn_PrintType_PrintTypeCode.Name = "gridColumn_PrintType_PrintTypeCode";
            this.gridColumn_PrintType_PrintTypeCode.OptionsColumn.AllowFocus = false;
            this.gridColumn_PrintType_PrintTypeCode.Visible = true;
            this.gridColumn_PrintType_PrintTypeCode.VisibleIndex = 0;
            this.gridColumn_PrintType_PrintTypeCode.Width = 161;
            // 
            // repositoryItemTextEdit__PrintTypeCode
            // 
            this.repositoryItemTextEdit__PrintTypeCode.AutoHeight = false;
            this.repositoryItemTextEdit__PrintTypeCode.Name = "repositoryItemTextEdit__PrintTypeCode";
            this.repositoryItemTextEdit__PrintTypeCode.Enter += new System.EventHandler(this.repositoryItemTextEdit__PrintTypeCode_Enter);
            // 
            // gridColumn_PrintType_PrintTypeName
            // 
            this.gridColumn_PrintType_PrintTypeName.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_PrintType_PrintTypeName.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_PrintType_PrintTypeName.Caption = "Tên loại in";
            this.gridColumn_PrintType_PrintTypeName.ColumnEdit = this.repositoryItemGridLookUp__PrintType;
            this.gridColumn_PrintType_PrintTypeName.FieldName = "PRINT_TYPE_ID";
            this.gridColumn_PrintType_PrintTypeName.Name = "gridColumn_PrintType_PrintTypeName";
            this.gridColumn_PrintType_PrintTypeName.Visible = true;
            this.gridColumn_PrintType_PrintTypeName.VisibleIndex = 1;
            this.gridColumn_PrintType_PrintTypeName.Width = 512;
            // 
            // repositoryItemGridLookUp__PrintType
            // 
            this.repositoryItemGridLookUp__PrintType.AutoHeight = false;
            this.repositoryItemGridLookUp__PrintType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemGridLookUp__PrintType.Name = "repositoryItemGridLookUp__PrintType";
            this.repositoryItemGridLookUp__PrintType.NullText = "";
            this.repositoryItemGridLookUp__PrintType.View = this.repositoryItemGridLookUpEdit1View;
            this.repositoryItemGridLookUp__PrintType.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.repositoryItemGridLookUp__PrintType_Closed);
            // 
            // repositoryItemGridLookUpEdit1View
            // 
            this.repositoryItemGridLookUpEdit1View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.repositoryItemGridLookUpEdit1View.Name = "repositoryItemGridLookUpEdit1View";
            this.repositoryItemGridLookUpEdit1View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.repositoryItemGridLookUpEdit1View.OptionsView.ShowGroupPanel = false;
            // 
            // gridColumn_PrintType_PrinterName
            // 
            this.gridColumn_PrintType_PrinterName.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_PrintType_PrinterName.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_PrintType_PrinterName.Caption = "Máy in";
            this.gridColumn_PrintType_PrinterName.ColumnEdit = this.repositoryItemCboBoxPrinter;
            this.gridColumn_PrintType_PrinterName.FieldName = "PRINTER_NAME";
            this.gridColumn_PrintType_PrinterName.Name = "gridColumn_PrintType_PrinterName";
            this.gridColumn_PrintType_PrinterName.Visible = true;
            this.gridColumn_PrintType_PrinterName.VisibleIndex = 2;
            this.gridColumn_PrintType_PrinterName.Width = 370;
            // 
            // repositoryItemCboBoxPrinter
            // 
            this.repositoryItemCboBoxPrinter.AutoHeight = false;
            this.repositoryItemCboBoxPrinter.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemCboBoxPrinter.Name = "repositoryItemCboBoxPrinter";
            // 
            // gridColumnAdd
            // 
            this.gridColumnAdd.Caption = "Thêm";
            this.gridColumnAdd.ColumnEdit = this.repositoryItemButton__Add;
            this.gridColumnAdd.FieldName = "Add";
            this.gridColumnAdd.Name = "gridColumnAdd";
            this.gridColumnAdd.OptionsColumn.ShowCaption = false;
            this.gridColumnAdd.Visible = true;
            this.gridColumnAdd.VisibleIndex = 3;
            this.gridColumnAdd.Width = 31;
            // 
            // repositoryItemButton__Add
            // 
            this.repositoryItemButton__Add.AutoHeight = false;
            this.repositoryItemButton__Add.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Plus, "", -1, true, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, null, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject9, serializableAppearanceObject10, serializableAppearanceObject11, serializableAppearanceObject12, "Thêm", null, null, true)});
            this.repositoryItemButton__Add.Name = "repositoryItemButton__Add";
            this.repositoryItemButton__Add.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            this.repositoryItemButton__Add.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repositoryItemButton__Add_ButtonClick);
            // 
            // repositoryItemButton__Delete
            // 
            this.repositoryItemButton__Delete.AutoHeight = false;
            this.repositoryItemButton__Delete.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Minus, "", -1, true, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, null, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject13, serializableAppearanceObject14, serializableAppearanceObject15, serializableAppearanceObject16, "Xóa", null, null, true)});
            this.repositoryItemButton__Delete.Name = "repositoryItemButton__Delete";
            this.repositoryItemButton__Delete.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.repositoryItemButton__Delete.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repositoryItemButton__Delete_ButtonClick);
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.layoutControlItem2,
            this.emptySpaceItem1});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "Root";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(880, 529);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.gridControlPrintType;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(880, 496);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.btnSave;
            this.layoutControlItem2.Location = new System.Drawing.Point(733, 496);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(147, 33);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(0, 496);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(733, 33);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // barManager1
            // 
            this.barManager1.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
            this.bar1});
            this.barManager1.Form = this;
            this.barManager1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.barButtonItem_Save});
            this.barManager1.MaxItemId = 1;
            // 
            // bar1
            // 
            this.bar1.BarName = "Tools";
            this.bar1.DockCol = 0;
            this.bar1.DockRow = 0;
            this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.barButtonItem_Save)});
            this.bar1.Text = "Tools";
            this.bar1.Visible = false;
            // 
            // barButtonItem_Save
            // 
            this.barButtonItem_Save.Caption = "Lưu (Ctrl S)";
            this.barButtonItem_Save.Id = 0;
            this.barButtonItem_Save.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S));
            this.barButtonItem_Save.Name = "barButtonItem_Save";
            this.barButtonItem_Save.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem_Save_ItemClick);
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 38);
            this.barDockControlTop.Size = new System.Drawing.Size(880, 0);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 567);
            this.barDockControlBottom.Size = new System.Drawing.Size(880, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 38);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 529);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(880, 38);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 529);
            // 
            // frmConfigPrinters
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(880, 567);
            this.Controls.Add(this.layoutControl1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "frmConfigPrinters";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Cấu hình máy in";
            this.Load += new System.EventHandler(this.frmConfigPrinters_Load);
            this.Controls.SetChildIndex(this.barDockControlTop, 0);
            this.Controls.SetChildIndex(this.barDockControlBottom, 0);
            this.Controls.SetChildIndex(this.barDockControlRight, 0);
            this.Controls.SetChildIndex(this.barDockControlLeft, 0);
            this.Controls.SetChildIndex(this.layoutControl1, 0);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControlPrintType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewPrintType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemTextEdit__PrintTypeCode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemGridLookUp__PrintType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemGridLookUpEdit1View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCboBoxPrinter)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemButton__Add)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemButton__Delete)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.SimpleButton btnSave;
        private DevExpress.XtraGrid.GridControl gridControlPrintType;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewPrintType;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_PrintType_PrintTypeCode;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_PrintType_PrintTypeName;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_PrintType_PrinterName;
        private DevExpress.XtraEditors.Repository.RepositoryItemComboBox repositoryItemCboBoxPrinter;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnAdd;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit repositoryItemButton__Add;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit repositoryItemButton__Delete;
        private DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit repositoryItemGridLookUp__PrintType;
        private DevExpress.XtraGrid.Views.Grid.GridView repositoryItemGridLookUpEdit1View;
        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.Bar bar1;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraBars.BarButtonItem barButtonItem_Save;
        private DevExpress.XtraEditors.Repository.RepositoryItemTextEdit repositoryItemTextEdit__PrintTypeCode;
    }
}
