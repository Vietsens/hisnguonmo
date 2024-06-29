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
namespace HIS.UC.TotalPriceInfo
{
    partial class UCTotalPriceInfo1
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.gridControlTotalPrice = new DevExpress.XtraGrid.GridControl();
            this.gridViewTotalPrice = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlTotalPrice)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewTotalPrice)).BeginInit();
            this.SuspendLayout();
            // 
            // gridControlTotalPrice
            // 
            this.gridControlTotalPrice.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControlTotalPrice.Location = new System.Drawing.Point(0, 0);
            this.gridControlTotalPrice.MainView = this.gridViewTotalPrice;
            this.gridControlTotalPrice.Name = "gridControlTotalPrice";
            this.gridControlTotalPrice.Size = new System.Drawing.Size(220, 285);
            this.gridControlTotalPrice.TabIndex = 0;
            this.gridControlTotalPrice.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewTotalPrice});
            // 
            // gridViewTotalPrice
            // 
            this.gridViewTotalPrice.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.gridViewTotalPrice.ColumnPanelRowHeight = 24;
            this.gridViewTotalPrice.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn1,
            this.gridColumn2});
            this.gridViewTotalPrice.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.None;
            this.gridViewTotalPrice.GridControl = this.gridControlTotalPrice;
            this.gridViewTotalPrice.Name = "gridViewTotalPrice";
            this.gridViewTotalPrice.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridViewTotalPrice.OptionsSelection.EnableAppearanceFocusedRow = false;
            this.gridViewTotalPrice.OptionsView.ShowColumnHeaders = false;
            this.gridViewTotalPrice.OptionsView.ShowGroupPanel = false;
            this.gridViewTotalPrice.OptionsView.ShowHorizontalLines = DevExpress.Utils.DefaultBoolean.False;
            this.gridViewTotalPrice.OptionsView.ShowIndicator = false;
            this.gridViewTotalPrice.OptionsView.ShowPreviewRowLines = DevExpress.Utils.DefaultBoolean.False;
            this.gridViewTotalPrice.OptionsView.ShowVerticalLines = DevExpress.Utils.DefaultBoolean.False;
            this.gridViewTotalPrice.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gridViewTotalPrice_RowCellStyle);
            // 
            // gridColumn1
            // 
            this.gridColumn1.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn1.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gridColumn1.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn1.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gridColumn1.Caption = "FieldLable";
            this.gridColumn1.FieldName = "FieldLable";
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.OptionsColumn.AllowEdit = false;
            this.gridColumn1.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.gridColumn1.OptionsColumn.ShowCaption = false;
            this.gridColumn1.Visible = true;
            this.gridColumn1.VisibleIndex = 0;
            this.gridColumn1.Width = 100;
            // 
            // gridColumn2
            // 
            this.gridColumn2.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn2.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gridColumn2.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn2.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gridColumn2.Caption = "FieldValue";
            this.gridColumn2.FieldName = "FieldValue";
            this.gridColumn2.Name = "gridColumn2";
            this.gridColumn2.OptionsColumn.AllowEdit = false;
            this.gridColumn2.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.gridColumn2.OptionsColumn.ShowCaption = false;
            this.gridColumn2.Visible = true;
            this.gridColumn2.VisibleIndex = 1;
            this.gridColumn2.Width = 128;
            // 
            // UCTotalPriceInfo1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoSize = true;
            this.Controls.Add(this.gridControlTotalPrice);
            this.Name = "UCTotalPriceInfo1";
            this.Size = new System.Drawing.Size(220, 285);
            this.Load += new System.EventHandler(this.UCTotalPriceInfo1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridControlTotalPrice)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewTotalPrice)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraGrid.GridControl gridControlTotalPrice;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewTotalPrice;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
    }
}
