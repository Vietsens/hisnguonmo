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
namespace HIS.Desktop.Plugins.AggrMobaImpMests
{
    partial class frmAggrMobaImpMests
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmAggrMobaImpMests));
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.btnSave = new DevExpress.XtraEditors.SimpleButton();
            this.txtMediStockImportCode = new DevExpress.XtraEditors.TextEdit();
            this.gridControlMobaImpMests = new DevExpress.XtraGrid.GridControl();
            this.gridViewMobaImpMests = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColSTT = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemButton_Delete = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.gridColMedicineTypeCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColMedicineTypeName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColAmount = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemSpinEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
            this.gridColServiceUnitName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColMediStockName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.cboMediStockImport = new DevExpress.XtraEditors.LookUpEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.barManager1 = new DevExpress.XtraBars.BarManager();
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.barButton_Save = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.dxValidationProvider1 = new DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtMediStockImportCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlMobaImpMests)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewMobaImpMests)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemButton_Delete)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSpinEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboMediStockImport.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.btnSave);
            this.layoutControl1.Controls.Add(this.txtMediStockImportCode);
            this.layoutControl1.Controls.Add(this.gridControlMobaImpMests);
            this.layoutControl1.Controls.Add(this.cboMediStockImport);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 29);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(791, 75, 250, 350);
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(660, 232);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(536, 208);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(122, 22);
            this.btnSave.StyleController = this.layoutControl1;
            this.btnSave.TabIndex = 8;
            this.btnSave.Text = "Lưu (Ctrl S)";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // txtMediStockImportCode
            // 
            this.txtMediStockImportCode.Location = new System.Drawing.Point(97, 2);
            this.txtMediStockImportCode.Name = "txtMediStockImportCode";
            this.txtMediStockImportCode.Size = new System.Drawing.Size(68, 20);
            this.txtMediStockImportCode.StyleController = this.layoutControl1;
            this.txtMediStockImportCode.TabIndex = 7;
            this.txtMediStockImportCode.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtMediStockImportCode_PreviewKeyDown);
            // 
            // gridControlMobaImpMests
            // 
            this.gridControlMobaImpMests.Location = new System.Drawing.Point(2, 26);
            this.gridControlMobaImpMests.MainView = this.gridViewMobaImpMests;
            this.gridControlMobaImpMests.Name = "gridControlMobaImpMests";
            this.gridControlMobaImpMests.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemSpinEdit1,
            this.repositoryItemButton_Delete});
            this.gridControlMobaImpMests.Size = new System.Drawing.Size(656, 178);
            this.gridControlMobaImpMests.TabIndex = 6;
            this.gridControlMobaImpMests.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewMobaImpMests});
            // 
            // gridViewMobaImpMests
            // 
            this.gridViewMobaImpMests.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColSTT,
            this.gridColumn1,
            this.gridColMedicineTypeCode,
            this.gridColMedicineTypeName,
            this.gridColAmount,
            this.gridColServiceUnitName,
            this.gridColMediStockName});
            this.gridViewMobaImpMests.GridControl = this.gridControlMobaImpMests;
            this.gridViewMobaImpMests.Name = "gridViewMobaImpMests";
            this.gridViewMobaImpMests.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            this.gridViewMobaImpMests.OptionsView.ShowGroupPanel = false;
            this.gridViewMobaImpMests.OptionsView.ShowIndicator = false;
            this.gridViewMobaImpMests.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gridViewMobaImpMests_CellValueChanged);
            this.gridViewMobaImpMests.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewMobaImpMests_CustomUnboundColumnData);
            // 
            // gridColSTT
            // 
            this.gridColSTT.Caption = "STT";
            this.gridColSTT.FieldName = "STT";
            this.gridColSTT.Name = "gridColSTT";
            this.gridColSTT.OptionsColumn.AllowEdit = false;
            this.gridColSTT.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColSTT.Visible = true;
            this.gridColSTT.VisibleIndex = 1;
            this.gridColSTT.Width = 64;
            // 
            // gridColumn1
            // 
            this.gridColumn1.Caption = "gridColumn1";
            this.gridColumn1.ColumnEdit = this.repositoryItemButton_Delete;
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.OptionsColumn.ShowCaption = false;
            this.gridColumn1.Visible = true;
            this.gridColumn1.VisibleIndex = 0;
            this.gridColumn1.Width = 31;
            // 
            // repositoryItemButton_Delete
            // 
            this.repositoryItemButton_Delete.AutoHeight = false;
            this.repositoryItemButton_Delete.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)});
            this.repositoryItemButton_Delete.Name = "repositoryItemButton_Delete";
            this.repositoryItemButton_Delete.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            this.repositoryItemButton_Delete.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repositoryItemButton_Delete_ButtonClick);
            // 
            // gridColMedicineTypeCode
            // 
            this.gridColMedicineTypeCode.Caption = "Mã";
            this.gridColMedicineTypeCode.FieldName = "MEDICINE_TYPE_CODE";
            this.gridColMedicineTypeCode.Name = "gridColMedicineTypeCode";
            this.gridColMedicineTypeCode.OptionsColumn.AllowEdit = false;
            this.gridColMedicineTypeCode.Visible = true;
            this.gridColMedicineTypeCode.VisibleIndex = 2;
            this.gridColMedicineTypeCode.Width = 117;
            // 
            // gridColMedicineTypeName
            // 
            this.gridColMedicineTypeName.Caption = "Tên thuốc, vật tư";
            this.gridColMedicineTypeName.FieldName = "MEDICINE_TYPE_NAME";
            this.gridColMedicineTypeName.Name = "gridColMedicineTypeName";
            this.gridColMedicineTypeName.OptionsColumn.AllowEdit = false;
            this.gridColMedicineTypeName.Visible = true;
            this.gridColMedicineTypeName.VisibleIndex = 3;
            this.gridColMedicineTypeName.Width = 365;
            // 
            // gridColAmount
            // 
            this.gridColAmount.AppearanceCell.Options.UseTextOptions = true;
            this.gridColAmount.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gridColAmount.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColAmount.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColAmount.Caption = "Số lượng (lẻ)";
            this.gridColAmount.ColumnEdit = this.repositoryItemSpinEdit1;
            this.gridColAmount.FieldName = "AMOUNT";
            this.gridColAmount.Name = "gridColAmount";
            this.gridColAmount.Visible = true;
            this.gridColAmount.VisibleIndex = 4;
            this.gridColAmount.Width = 128;
            // 
            // repositoryItemSpinEdit1
            // 
            this.repositoryItemSpinEdit1.AutoHeight = false;
            this.repositoryItemSpinEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemSpinEdit1.MaxValue = new decimal(new int[] {
            99999999,
            0,
            0,
            0});
            this.repositoryItemSpinEdit1.Name = "repositoryItemSpinEdit1";
            // 
            // gridColServiceUnitName
            // 
            this.gridColServiceUnitName.Caption = "Đơn vị tính";
            this.gridColServiceUnitName.FieldName = "SERVICE_UNIT_NAME";
            this.gridColServiceUnitName.Name = "gridColServiceUnitName";
            this.gridColServiceUnitName.OptionsColumn.AllowEdit = false;
            this.gridColServiceUnitName.Visible = true;
            this.gridColServiceUnitName.VisibleIndex = 5;
            this.gridColServiceUnitName.Width = 109;
            // 
            // gridColMediStockName
            // 
            this.gridColMediStockName.Caption = "Kho";
            this.gridColMediStockName.FieldName = "MEDI_STOCK_NAME";
            this.gridColMediStockName.Name = "gridColMediStockName";
            this.gridColMediStockName.OptionsColumn.AllowEdit = false;
            this.gridColMediStockName.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColMediStockName.Visible = true;
            this.gridColMediStockName.VisibleIndex = 6;
            this.gridColMediStockName.Width = 233;
            // 
            // cboMediStockImport
            // 
            this.cboMediStockImport.Location = new System.Drawing.Point(165, 2);
            this.cboMediStockImport.Name = "cboMediStockImport";
            this.cboMediStockImport.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboMediStockImport.Properties.NullText = "";
            this.cboMediStockImport.Size = new System.Drawing.Size(204, 20);
            this.cboMediStockImport.StyleController = this.layoutControl1;
            this.cboMediStockImport.TabIndex = 5;
            this.cboMediStockImport.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboMediStockImport_Closed);
            this.cboMediStockImport.KeyUp += new System.Windows.Forms.KeyEventHandler(this.cboMediStockImport_KeyUp);
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem2,
            this.layoutControlItem3,
            this.layoutControlItem4,
            this.emptySpaceItem1,
            this.layoutControlItem1,
            this.emptySpaceItem2});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "Root";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(660, 232);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.cboMediStockImport;
            this.layoutControlItem2.Location = new System.Drawing.Point(165, 0);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 2, 2, 2);
            this.layoutControlItem2.Size = new System.Drawing.Size(206, 24);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.gridControlMobaImpMests;
            this.layoutControlItem3.Location = new System.Drawing.Point(0, 24);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(660, 182);
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextVisible = false;
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.layoutControlItem4.AppearanceItemCaption.Options.UseForeColor = true;
            this.layoutControlItem4.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem4.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem4.Control = this.txtMediStockImportCode;
            this.layoutControlItem4.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 0, 2, 2);
            this.layoutControlItem4.Size = new System.Drawing.Size(165, 24);
            this.layoutControlItem4.Text = "Kho nhập:";
            this.layoutControlItem4.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem4.TextSize = new System.Drawing.Size(90, 20);
            this.layoutControlItem4.TextToControlDistance = 5;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(371, 0);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(289, 24);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.btnSave;
            this.layoutControlItem1.Location = new System.Drawing.Point(534, 206);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(126, 26);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // emptySpaceItem2
            // 
            this.emptySpaceItem2.AllowHotTrack = false;
            this.emptySpaceItem2.Location = new System.Drawing.Point(0, 206);
            this.emptySpaceItem2.Name = "emptySpaceItem2";
            this.emptySpaceItem2.Size = new System.Drawing.Size(534, 26);
            this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
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
            this.barButton_Save});
            this.barManager1.MaxItemId = 1;
            // 
            // bar1
            // 
            this.bar1.BarName = "Tools";
            this.bar1.DockCol = 0;
            this.bar1.DockRow = 0;
            this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.barButton_Save)});
            this.bar1.Text = "Tools";
            this.bar1.Visible = false;
            // 
            // barButton_Save
            // 
            this.barButton_Save.Caption = "Lưu (Ctrl S)";
            this.barButton_Save.Id = 0;
            this.barButton_Save.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S));
            this.barButton_Save.Name = "barButton_Save";
            this.barButton_Save.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButton_Save_ItemClick);
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Size = new System.Drawing.Size(660, 29);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 261);
            this.barDockControlBottom.Size = new System.Drawing.Size(660, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 29);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 232);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(660, 29);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 232);
            // 
            // dxValidationProvider1
            // 
            this.dxValidationProvider1.ValidationFailed += new DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventHandler(this.dxValidationProvider1_ValidationFailed);
            // 
            // frmAggrMobaImpMests
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(660, 261);
            this.Controls.Add(this.layoutControl1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmAggrMobaImpMests";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Thuốc lẻ";
            this.Load += new System.EventHandler(this.frmAggrMobaImpMests_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtMediStockImportCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlMobaImpMests)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewMobaImpMests)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemButton_Delete)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSpinEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboMediStockImport.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraEditors.TextEdit txtMediStockImportCode;
        private DevExpress.XtraGrid.GridControl gridControlMobaImpMests;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewMobaImpMests;
        private DevExpress.XtraEditors.LookUpEdit cboMediStockImport;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraGrid.Columns.GridColumn gridColSTT;
        private DevExpress.XtraGrid.Columns.GridColumn gridColMedicineTypeCode;
        private DevExpress.XtraGrid.Columns.GridColumn gridColMedicineTypeName;
        private DevExpress.XtraGrid.Columns.GridColumn gridColAmount;
        private DevExpress.XtraGrid.Columns.GridColumn gridColServiceUnitName;
        private DevExpress.XtraGrid.Columns.GridColumn gridColMediStockName;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraEditors.SimpleButton btnSave;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.Bar bar1;
        private DevExpress.XtraBars.BarButtonItem barButton_Save;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProvider1;
        private DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit repositoryItemSpinEdit1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit repositoryItemButton_Delete;
    }
}
