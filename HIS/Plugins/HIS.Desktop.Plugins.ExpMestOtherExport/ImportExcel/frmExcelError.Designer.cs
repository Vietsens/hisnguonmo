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
namespace HIS.Desktop.Plugins.ExpMestOtherExport.ImportExcel
{
    partial class frmExcelError
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
            this.btnExportError = new DevExpress.XtraEditors.SimpleButton();
            this.gridControlImportError = new DevExpress.XtraGrid.GridControl();
            this.gridViewImportError = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn_Stt = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_TypeName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_TypeCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_Error = new DevExpress.XtraGrid.Columns.GridColumn();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.gridColumn_Amount = new DevExpress.XtraGrid.Columns.GridColumn();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlImportError)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewImportError)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.btnExportError);
            this.layoutControl1.Controls.Add(this.gridControlImportError);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(880, 461);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // btnExportError
            // 
            this.btnExportError.Location = new System.Drawing.Point(762, 437);
            this.btnExportError.Name = "btnExportError";
            this.btnExportError.Size = new System.Drawing.Size(116, 22);
            this.btnExportError.StyleController = this.layoutControl1;
            this.btnExportError.TabIndex = 5;
            this.btnExportError.Text = "Xuất dữ liệu lỗi";
            this.btnExportError.Click += new System.EventHandler(this.btnExportError_Click);
            // 
            // gridControlImportError
            // 
            this.gridControlImportError.Location = new System.Drawing.Point(2, 2);
            this.gridControlImportError.MainView = this.gridViewImportError;
            this.gridControlImportError.Name = "gridControlImportError";
            this.gridControlImportError.Size = new System.Drawing.Size(876, 431);
            this.gridControlImportError.TabIndex = 4;
            this.gridControlImportError.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewImportError});
            // 
            // gridViewImportError
            // 
            this.gridViewImportError.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn_Stt,
            this.gridColumn_TypeName,
            this.gridColumn_TypeCode,
            this.gridColumn_Amount,
            this.gridColumn_Error});
            this.gridViewImportError.GridControl = this.gridControlImportError;
            this.gridViewImportError.Name = "gridViewImportError";
            this.gridViewImportError.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            this.gridViewImportError.OptionsView.ShowGroupPanel = false;
            this.gridViewImportError.OptionsView.ShowIndicator = false;
            this.gridViewImportError.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewImportError_CustomUnboundColumnData);
            // 
            // gridColumn_Stt
            // 
            this.gridColumn_Stt.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn_Stt.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gridColumn_Stt.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_Stt.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_Stt.Caption = "STT";
            this.gridColumn_Stt.FieldName = "STT";
            this.gridColumn_Stt.Name = "gridColumn_Stt";
            this.gridColumn_Stt.OptionsColumn.AllowEdit = false;
            this.gridColumn_Stt.OptionsColumn.ReadOnly = true;
            this.gridColumn_Stt.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn_Stt.Visible = true;
            this.gridColumn_Stt.VisibleIndex = 0;
            this.gridColumn_Stt.Width = 32;
            // 
            // gridColumn_TypeName
            // 
            this.gridColumn_TypeName.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_TypeName.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_TypeName.Caption = "Loại";
            this.gridColumn_TypeName.FieldName = "TypeName";
            this.gridColumn_TypeName.Name = "gridColumn_TypeName";
            this.gridColumn_TypeName.OptionsColumn.ReadOnly = true;
            this.gridColumn_TypeName.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn_TypeName.Visible = true;
            this.gridColumn_TypeName.VisibleIndex = 1;
            this.gridColumn_TypeName.Width = 83;
            // 
            // gridColumn_TypeCode
            // 
            this.gridColumn_TypeCode.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_TypeCode.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_TypeCode.Caption = "Mã";
            this.gridColumn_TypeCode.FieldName = "TYPE_CODE";
            this.gridColumn_TypeCode.Name = "gridColumn_TypeCode";
            this.gridColumn_TypeCode.OptionsColumn.ReadOnly = true;
            this.gridColumn_TypeCode.Visible = true;
            this.gridColumn_TypeCode.VisibleIndex = 2;
            this.gridColumn_TypeCode.Width = 83;
            // 
            // gridColumn_Error
            // 
            this.gridColumn_Error.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_Error.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_Error.Caption = "Lỗi";
            this.gridColumn_Error.FieldName = "MESSAGE";
            this.gridColumn_Error.Name = "gridColumn_Error";
            this.gridColumn_Error.OptionsColumn.ReadOnly = true;
            this.gridColumn_Error.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn_Error.Visible = true;
            this.gridColumn_Error.VisibleIndex = 4;
            this.gridColumn_Error.Width = 604;
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
            this.layoutControlGroup1.Size = new System.Drawing.Size(880, 461);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.gridControlImportError;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(880, 435);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.btnExportError;
            this.layoutControlItem2.Location = new System.Drawing.Point(760, 435);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(120, 26);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(0, 435);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(760, 26);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // gridColumn_Amount
            // 
            this.gridColumn_Amount.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn_Amount.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gridColumn_Amount.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_Amount.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_Amount.Caption = "Số lượng";
            this.gridColumn_Amount.DisplayFormat.FormatString = "#,##0.00";
            this.gridColumn_Amount.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.gridColumn_Amount.FieldName = "AMOUNT";
            this.gridColumn_Amount.Name = "gridColumn_Amount";
            this.gridColumn_Amount.Visible = true;
            this.gridColumn_Amount.VisibleIndex = 3;
            this.gridColumn_Amount.Width = 90;
            // 
            // frmExcelError
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(880, 461);
            this.Controls.Add(this.layoutControl1);
            this.Name = "frmExcelError";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Lỗi xuất từ excel";
            this.Load += new System.EventHandler(this.frmExcelError_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControlImportError)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewImportError)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.SimpleButton btnExportError;
        private DevExpress.XtraGrid.GridControl gridControlImportError;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewImportError;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_Stt;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_TypeName;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_TypeCode;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_Error;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_Amount;
    }
}
