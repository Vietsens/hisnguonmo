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
namespace HIS.UC.HisMedicineInStock
{
    partial class UCMedicineInStock
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
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.txtKeyword = new DevExpress.XtraEditors.TextEdit();
            this.gridControlMedicineInStock = new DevExpress.XtraGrid.GridControl();
            this.gridViewMedicineInStock = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.RadioE = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.RadioD = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.CheckE = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.CheckD = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtKeyword.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlMedicineInStock)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewMedicineInStock)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RadioE)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RadioD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CheckE)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CheckD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.txtKeyword);
            this.layoutControl1.Controls.Add(this.gridControlMedicineInStock);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(707, 560);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // txtKeyword
            // 
            this.txtKeyword.Location = new System.Drawing.Point(2, 2);
            this.txtKeyword.Name = "txtKeyword";
            this.txtKeyword.Properties.NullValuePrompt = "Từ khóa tìm kiếm..";
            this.txtKeyword.Properties.ShowNullValuePromptWhenFocused = true;
            this.txtKeyword.Size = new System.Drawing.Size(703, 20);
            this.txtKeyword.StyleController = this.layoutControl1;
            this.txtKeyword.TabIndex = 8;
            this.txtKeyword.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtKeyword_PreviewKeyDown);
            // 
            // gridControlMedicineInStock
            // 
            this.gridControlMedicineInStock.Location = new System.Drawing.Point(2, 26);
            this.gridControlMedicineInStock.MainView = this.gridViewMedicineInStock;
            this.gridControlMedicineInStock.Name = "gridControlMedicineInStock";
            this.gridControlMedicineInStock.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.RadioE,
            this.RadioD,
            this.CheckE,
            this.CheckD});
            this.gridControlMedicineInStock.Size = new System.Drawing.Size(703, 532);
            this.gridControlMedicineInStock.TabIndex = 7;
            this.gridControlMedicineInStock.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewMedicineInStock});
            // 
            // gridViewMedicineInStock
            // 
            this.gridViewMedicineInStock.GridControl = this.gridControlMedicineInStock;
            this.gridViewMedicineInStock.Name = "gridViewMedicineInStock";
            this.gridViewMedicineInStock.OptionsBehavior.AutoPopulateColumns = false;
            this.gridViewMedicineInStock.OptionsCustomization.AllowFilter = false;
            this.gridViewMedicineInStock.OptionsCustomization.AllowSort = false;
            this.gridViewMedicineInStock.OptionsView.ShowGroupPanel = false;
            this.gridViewMedicineInStock.OptionsView.ShowIndicator = false;
            this.gridViewMedicineInStock.CustomRowCellEdit += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gridViewMedicineType_CustomRowCellEdit);
            this.gridViewMedicineInStock.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewMedicineType_CustomUnboundColumnData);
            this.gridViewMedicineInStock.KeyDown += new System.Windows.Forms.KeyEventHandler(this.gridViewMedicineInStock_KeyDown);
            this.gridViewMedicineInStock.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gridViewService_MouseDown);
            this.gridViewMedicineInStock.Click += new System.EventHandler(this.gridViewService_Click);
            // 
            // RadioE
            // 
            this.RadioE.AutoHeight = false;
            this.RadioE.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.RadioE.Name = "RadioE";
            // 
            // RadioD
            // 
            this.RadioD.AutoHeight = false;
            this.RadioD.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.RadioD.Name = "RadioD";
            this.RadioD.ReadOnly = true;
            // 
            // CheckE
            // 
            this.CheckE.AutoHeight = false;
            this.CheckE.Name = "CheckE";
            // 
            // CheckD
            // 
            this.CheckD.AutoHeight = false;
            this.CheckD.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Style2;
            this.CheckD.Name = "CheckD";
            this.CheckD.ReadOnly = true;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem4,
            this.layoutControlItem1});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(707, 560);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.Control = this.gridControlMedicineInStock;
            this.layoutControlItem4.Location = new System.Drawing.Point(0, 24);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Size = new System.Drawing.Size(707, 536);
            this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem4.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.txtKeyword;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(707, 24);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // UCMedicineInStock
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layoutControl1);
            this.Name = "UCMedicineInStock";
            this.Size = new System.Drawing.Size(707, 560);
            this.Load += new System.EventHandler(this.UC_MedicineTypeGrid_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtKeyword.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlMedicineInStock)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewMedicineInStock)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RadioE)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RadioD)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CheckE)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CheckD)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraGrid.GridControl gridControlMedicineInStock;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewMedicineInStock;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit RadioE;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit RadioD;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit CheckE;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit CheckD;
        private DevExpress.XtraEditors.TextEdit txtKeyword;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;

    }
}
