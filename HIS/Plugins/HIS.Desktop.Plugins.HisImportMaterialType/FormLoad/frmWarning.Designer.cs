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
namespace HIS.Desktop.Plugins.HisImportMaterialType.FormLoad
{
    partial class frmWarning
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.gridControlServiceUnit = new DevExpress.XtraGrid.GridControl();
            this.gridViewServiceUnit = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colSttSu = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colServiceUnitCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repoServiceUnitCode = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
            this.colServiceUnitName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repoServiceUnitName = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
            this.gridControlManufacturer = new DevExpress.XtraGrid.GridControl();
            this.gridViewManufacturer = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colSttManu = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colManufacturerCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repoManufacturerCode = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
            this.colManufacturerName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repoManufacturerName = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
            this.btnAdd = new DevExpress.XtraEditors.SimpleButton();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lcgServiceUnit = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciServiceUnit = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcgManufacturer = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciManufacturer = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciBtnAdd = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciBtnCancel = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlServiceUnit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewServiceUnit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoServiceUnitCode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoServiceUnitName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlManufacturer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewManufacturer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoManufacturerCode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoManufacturerName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcgServiceUnit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciServiceUnit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcgManufacturer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciManufacturer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnAdd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnCancel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            this.SuspendLayout();
            //
            // layoutControl1
            //
            this.layoutControl1.Controls.Add(this.gridControlServiceUnit);
            this.layoutControl1.Controls.Add(this.gridControlManufacturer);
            this.layoutControl1.Controls.Add(this.btnAdd);
            this.layoutControl1.Controls.Add(this.btnCancel);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(504, 440);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            //
            // gridControlServiceUnit
            //
            this.gridControlServiceUnit.Location = new System.Drawing.Point(5, 24);
            this.gridControlServiceUnit.MainView = this.gridViewServiceUnit;
            this.gridControlServiceUnit.Name = "gridControlServiceUnit";
            this.gridControlServiceUnit.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repoServiceUnitCode,
            this.repoServiceUnitName});
            this.gridControlServiceUnit.Size = new System.Drawing.Size(494, 166);
            this.gridControlServiceUnit.TabIndex = 0;
            this.gridControlServiceUnit.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewServiceUnit});
            //
            // gridViewServiceUnit
            //
            this.gridViewServiceUnit.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colSttSu,
            this.colServiceUnitCode,
            this.colServiceUnitName});
            this.gridViewServiceUnit.GridControl = this.gridControlServiceUnit;
            this.gridViewServiceUnit.Name = "gridViewServiceUnit";
            this.gridViewServiceUnit.OptionsView.ShowGroupPanel = false;
            this.gridViewServiceUnit.OptionsView.ShowIndicator = false;
            this.gridViewServiceUnit.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewServiceUnit_CustomUnboundColumnData);
            this.gridViewServiceUnit.ValidatingEditor += new DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventHandler(this.gridView_ValidatingEditor);
            this.gridViewServiceUnit.InvalidValueException += new DevExpress.XtraEditors.Controls.InvalidValueExceptionEventHandler(this.gridView_InvalidValueException);
            //
            // colSttSu
            //
            this.colSttSu.Caption = "STT";
            this.colSttSu.FieldName = "STT";
            this.colSttSu.Name = "colSttSu";
            this.colSttSu.OptionsColumn.AllowEdit = false;
            this.colSttSu.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.colSttSu.Visible = true;
            this.colSttSu.VisibleIndex = 0;
            this.colSttSu.Width = 48;
            //
            // colServiceUnitCode
            //
            this.colServiceUnitCode.Caption = "Mã đơn vị tính";
            this.colServiceUnitCode.ColumnEdit = this.repoServiceUnitCode;
            this.colServiceUnitCode.FieldName = "SERVICE_UNIT_CODE";
            this.colServiceUnitCode.Name = "colServiceUnitCode";
            this.colServiceUnitCode.Visible = true;
            this.colServiceUnitCode.VisibleIndex = 1;
            this.colServiceUnitCode.Width = 130;
            //
            // repoServiceUnitCode
            //
            this.repoServiceUnitCode.AutoHeight = false;
            this.repoServiceUnitCode.MaxLength = 3;
            this.repoServiceUnitCode.Name = "repoServiceUnitCode";
            //
            // colServiceUnitName
            //
            this.colServiceUnitName.Caption = "Tên đơn vị tính";
            this.colServiceUnitName.ColumnEdit = this.repoServiceUnitName;
            this.colServiceUnitName.FieldName = "SERVICE_UNIT_NAME";
            this.colServiceUnitName.Name = "colServiceUnitName";
            this.colServiceUnitName.Visible = true;
            this.colServiceUnitName.VisibleIndex = 2;
            this.colServiceUnitName.Width = 300;
            //
            // repoServiceUnitName
            //
            this.repoServiceUnitName.AutoHeight = false;
            this.repoServiceUnitName.Name = "repoServiceUnitName";
            //
            // gridControlManufacturer
            //
            this.gridControlManufacturer.Location = new System.Drawing.Point(5, 218);
            this.gridControlManufacturer.MainView = this.gridViewManufacturer;
            this.gridControlManufacturer.Name = "gridControlManufacturer";
            this.gridControlManufacturer.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repoManufacturerCode,
            this.repoManufacturerName});
            this.gridControlManufacturer.Size = new System.Drawing.Size(494, 166);
            this.gridControlManufacturer.TabIndex = 1;
            this.gridControlManufacturer.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewManufacturer});
            //
            // gridViewManufacturer
            //
            this.gridViewManufacturer.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colSttManu,
            this.colManufacturerCode,
            this.colManufacturerName});
            this.gridViewManufacturer.GridControl = this.gridControlManufacturer;
            this.gridViewManufacturer.Name = "gridViewManufacturer";
            this.gridViewManufacturer.OptionsView.ShowGroupPanel = false;
            this.gridViewManufacturer.OptionsView.ShowIndicator = false;
            this.gridViewManufacturer.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewManufacturer_CustomUnboundColumnData);
            this.gridViewManufacturer.ValidatingEditor += new DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventHandler(this.gridView_ValidatingEditor);
            this.gridViewManufacturer.InvalidValueException += new DevExpress.XtraEditors.Controls.InvalidValueExceptionEventHandler(this.gridView_InvalidValueException);
            //
            // colSttManu
            //
            this.colSttManu.Caption = "STT";
            this.colSttManu.FieldName = "STT";
            this.colSttManu.Name = "colSttManu";
            this.colSttManu.OptionsColumn.AllowEdit = false;
            this.colSttManu.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.colSttManu.Visible = true;
            this.colSttManu.VisibleIndex = 0;
            this.colSttManu.Width = 48;
            //
            // colManufacturerCode
            //
            this.colManufacturerCode.Caption = "Mã hãng sản xuất";
            this.colManufacturerCode.ColumnEdit = this.repoManufacturerCode;
            this.colManufacturerCode.FieldName = "MANUFACTURER_CODE";
            this.colManufacturerCode.Name = "colManufacturerCode";
            this.colManufacturerCode.Visible = true;
            this.colManufacturerCode.VisibleIndex = 1;
            this.colManufacturerCode.Width = 130;
            //
            // repoManufacturerCode
            //
            this.repoManufacturerCode.AutoHeight = false;
            this.repoManufacturerCode.MaxLength = 6;
            this.repoManufacturerCode.Name = "repoManufacturerCode";
            //
            // colManufacturerName
            //
            this.colManufacturerName.Caption = "Tên hãng sản xuất";
            this.colManufacturerName.ColumnEdit = this.repoManufacturerName;
            this.colManufacturerName.FieldName = "MANUFACTURER_NAME";
            this.colManufacturerName.Name = "colManufacturerName";
            this.colManufacturerName.Visible = true;
            this.colManufacturerName.VisibleIndex = 2;
            this.colManufacturerName.Width = 300;
            //
            // repoManufacturerName
            //
            this.repoManufacturerName.AutoHeight = false;
            this.repoManufacturerName.Name = "repoManufacturerName";
            //
            // btnAdd
            //
            this.btnAdd.Location = new System.Drawing.Point(265, 412);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(150, 22);
            this.btnAdd.StyleController = this.layoutControl1;
            this.btnAdd.TabIndex = 2;
            this.btnAdd.Text = "Bổ sung vào danh mục";
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            //
            // btnCancel
            //
            this.btnCancel.Location = new System.Drawing.Point(419, 412);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(80, 22);
            this.btnCancel.StyleController = this.layoutControl1;
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Bỏ qua";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            //
            // layoutControlGroup1
            //
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lcgServiceUnit,
            this.lcgManufacturer,
            this.lciBtnAdd,
            this.lciBtnCancel,
            this.emptySpaceItem1});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "Root";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(504, 440);
            this.layoutControlGroup1.TextVisible = false;
            //
            // lcgServiceUnit
            //
            this.lcgServiceUnit.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciServiceUnit});
            this.lcgServiceUnit.Location = new System.Drawing.Point(0, 0);
            this.lcgServiceUnit.Name = "lcgServiceUnit";
            this.lcgServiceUnit.Size = new System.Drawing.Size(504, 194);
            this.lcgServiceUnit.Text = "Đơn vị tính mới";
            //
            // lciServiceUnit
            //
            this.lciServiceUnit.Control = this.gridControlServiceUnit;
            this.lciServiceUnit.Location = new System.Drawing.Point(0, 0);
            this.lciServiceUnit.Name = "lciServiceUnit";
            this.lciServiceUnit.Size = new System.Drawing.Size(498, 170);
            this.lciServiceUnit.TextSize = new System.Drawing.Size(0, 0);
            this.lciServiceUnit.TextVisible = false;
            //
            // lcgManufacturer
            //
            this.lcgManufacturer.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciManufacturer});
            this.lcgManufacturer.Location = new System.Drawing.Point(0, 194);
            this.lcgManufacturer.Name = "lcgManufacturer";
            this.lcgManufacturer.Size = new System.Drawing.Size(504, 194);
            this.lcgManufacturer.Text = "Hãng sản xuất mới";
            //
            // lciManufacturer
            //
            this.lciManufacturer.Control = this.gridControlManufacturer;
            this.lciManufacturer.Location = new System.Drawing.Point(0, 0);
            this.lciManufacturer.Name = "lciManufacturer";
            this.lciManufacturer.Size = new System.Drawing.Size(498, 170);
            this.lciManufacturer.TextSize = new System.Drawing.Size(0, 0);
            this.lciManufacturer.TextVisible = false;
            //
            // lciBtnAdd
            //
            this.lciBtnAdd.Control = this.btnAdd;
            this.lciBtnAdd.Location = new System.Drawing.Point(260, 388);
            this.lciBtnAdd.Name = "lciBtnAdd";
            this.lciBtnAdd.Size = new System.Drawing.Size(154, 32);
            this.lciBtnAdd.TextSize = new System.Drawing.Size(0, 0);
            this.lciBtnAdd.TextVisible = false;
            //
            // lciBtnCancel
            //
            this.lciBtnCancel.Control = this.btnCancel;
            this.lciBtnCancel.Location = new System.Drawing.Point(414, 388);
            this.lciBtnCancel.Name = "lciBtnCancel";
            this.lciBtnCancel.Size = new System.Drawing.Size(90, 32);
            this.lciBtnCancel.TextSize = new System.Drawing.Size(0, 0);
            this.lciBtnCancel.TextVisible = false;
            //
            // emptySpaceItem1
            //
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(0, 388);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(260, 32);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            //
            // frmWarning
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(504, 440);
            this.Controls.Add(this.layoutControl1);
            this.Name = "frmWarning";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Dữ liệu chưa có trong danh mục - xác nhận bổ sung";
            this.Load += new System.EventHandler(this.frmWarning_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControlServiceUnit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewServiceUnit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoServiceUnitCode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoServiceUnitName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlManufacturer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewManufacturer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoManufacturerCode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoManufacturerName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcgServiceUnit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciServiceUnit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcgManufacturer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciManufacturer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnAdd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnCancel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraGrid.GridControl gridControlServiceUnit;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewServiceUnit;
        private DevExpress.XtraGrid.Columns.GridColumn colSttSu;
        private DevExpress.XtraGrid.Columns.GridColumn colServiceUnitCode;
        private DevExpress.XtraGrid.Columns.GridColumn colServiceUnitName;
        private DevExpress.XtraEditors.Repository.RepositoryItemTextEdit repoServiceUnitCode;
        private DevExpress.XtraEditors.Repository.RepositoryItemTextEdit repoServiceUnitName;
        private DevExpress.XtraGrid.GridControl gridControlManufacturer;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewManufacturer;
        private DevExpress.XtraGrid.Columns.GridColumn colSttManu;
        private DevExpress.XtraGrid.Columns.GridColumn colManufacturerCode;
        private DevExpress.XtraGrid.Columns.GridColumn colManufacturerName;
        private DevExpress.XtraEditors.Repository.RepositoryItemTextEdit repoManufacturerCode;
        private DevExpress.XtraEditors.Repository.RepositoryItemTextEdit repoManufacturerName;
        private DevExpress.XtraEditors.SimpleButton btnAdd;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraLayout.LayoutControlGroup lcgServiceUnit;
        private DevExpress.XtraLayout.LayoutControlItem lciServiceUnit;
        private DevExpress.XtraLayout.LayoutControlGroup lcgManufacturer;
        private DevExpress.XtraLayout.LayoutControlItem lciManufacturer;
        private DevExpress.XtraLayout.LayoutControlItem lciBtnAdd;
        private DevExpress.XtraLayout.LayoutControlItem lciBtnCancel;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
    }
}
