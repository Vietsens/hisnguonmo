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
namespace HIS.Desktop.Plugins.ConfigPrinter
{
    partial class frmConfigPrinter
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
            this.btnSave = new DevExpress.XtraEditors.SimpleButton();
            this.gridControlPrintType = new DevExpress.XtraGrid.GridControl();
            this.gridViewPrintType = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn_PrintType_PrintTypeCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_PrintType_PrintTypeName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_PrintType_PrinterName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemCboBoxPrinter = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlPrintType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewPrintType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCboBoxPrinter)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.btnSave);
            this.layoutControl1.Controls.Add(this.gridControlPrintType);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Margin = new System.Windows.Forms.Padding(4);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(880, 569);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(736, 539);
            this.btnSave.Margin = new System.Windows.Forms.Padding(4);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(141, 27);
            this.btnSave.StyleController = this.layoutControl1;
            this.btnSave.TabIndex = 5;
            this.btnSave.Text = "Lưu (Ctrl S)";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // gridControlPrintType
            // 
            this.gridControlPrintType.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(4);
            this.gridControlPrintType.Location = new System.Drawing.Point(0, 0);
            this.gridControlPrintType.MainView = this.gridViewPrintType;
            this.gridControlPrintType.Margin = new System.Windows.Forms.Padding(4);
            this.gridControlPrintType.Name = "gridControlPrintType";
            this.gridControlPrintType.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemCboBoxPrinter});
            this.gridControlPrintType.Size = new System.Drawing.Size(880, 536);
            this.gridControlPrintType.TabIndex = 4;
            this.gridControlPrintType.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewPrintType});
            // 
            // gridViewPrintType
            // 
            this.gridViewPrintType.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn_PrintType_PrintTypeCode,
            this.gridColumn_PrintType_PrintTypeName,
            this.gridColumn_PrintType_PrinterName});
            this.gridViewPrintType.GridControl = this.gridControlPrintType;
            this.gridViewPrintType.Name = "gridViewPrintType";
            this.gridViewPrintType.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            this.gridViewPrintType.OptionsView.ShowGroupPanel = false;
            this.gridViewPrintType.OptionsView.ShowIndicator = false;
            // 
            // gridColumn_PrintType_PrintTypeCode
            // 
            this.gridColumn_PrintType_PrintTypeCode.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_PrintType_PrintTypeCode.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_PrintType_PrintTypeCode.Caption = "Mã loại in";
            this.gridColumn_PrintType_PrintTypeCode.FieldName = "PRINT_TYPE_CODE";
            this.gridColumn_PrintType_PrintTypeCode.Name = "gridColumn_PrintType_PrintTypeCode";
            this.gridColumn_PrintType_PrintTypeCode.OptionsColumn.AllowEdit = false;
            this.gridColumn_PrintType_PrintTypeCode.OptionsColumn.AllowFocus = false;
            this.gridColumn_PrintType_PrintTypeCode.Visible = true;
            this.gridColumn_PrintType_PrintTypeCode.VisibleIndex = 0;
            this.gridColumn_PrintType_PrintTypeCode.Width = 110;
            // 
            // gridColumn_PrintType_PrintTypeName
            // 
            this.gridColumn_PrintType_PrintTypeName.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_PrintType_PrintTypeName.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_PrintType_PrintTypeName.Caption = "Tên loại in";
            this.gridColumn_PrintType_PrintTypeName.FieldName = "PRINT_TYPE_NAME";
            this.gridColumn_PrintType_PrintTypeName.Name = "gridColumn_PrintType_PrintTypeName";
            this.gridColumn_PrintType_PrintTypeName.OptionsColumn.AllowEdit = false;
            this.gridColumn_PrintType_PrintTypeName.OptionsColumn.AllowFocus = false;
            this.gridColumn_PrintType_PrintTypeName.Visible = true;
            this.gridColumn_PrintType_PrintTypeName.VisibleIndex = 1;
            this.gridColumn_PrintType_PrintTypeName.Width = 350;
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
            this.gridColumn_PrintType_PrinterName.Width = 200;
            // 
            // repositoryItemCboBoxPrinter
            // 
            this.repositoryItemCboBoxPrinter.AutoHeight = false;
            this.repositoryItemCboBoxPrinter.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemCboBoxPrinter.Name = "repositoryItemCboBoxPrinter";
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
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(880, 569);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.gridControlPrintType;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlItem1.Size = new System.Drawing.Size(880, 536);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.btnSave;
            this.layoutControlItem2.Location = new System.Drawing.Point(733, 536);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(147, 33);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(0, 536);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(733, 33);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // frmConfigPrinter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(880, 569);
            this.Controls.Add(this.layoutControl1);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "frmConfigPrinter";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Cấu hình loại in máy in";
            this.Load += new System.EventHandler(this.frmConfigPrinter_Load);
            this.Controls.SetChildIndex(this.layoutControl1, 0);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControlPrintType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewPrintType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCboBoxPrinter)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.SimpleButton btnSave;
        private DevExpress.XtraGrid.GridControl gridControlPrintType;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewPrintType;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_PrintType_PrintTypeCode;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_PrintType_PrintTypeName;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_PrintType_PrinterName;
        private DevExpress.XtraEditors.Repository.RepositoryItemComboBox repositoryItemCboBoxPrinter;
    }
}
