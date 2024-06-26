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
namespace HIS.UC.MedicineTypeAcinGrid
{
    partial class UC_MedicineTypeAcinGrid
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
            this.gridControlMedicineTypeAcin = new DevExpress.XtraGrid.GridControl();
            this.gridViewMedicineTypeAcin = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.RadioEA = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.RadioDA = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.CheckEA = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.CheckDA = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlMedicineTypeAcin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewMedicineTypeAcin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RadioEA)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RadioDA)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CheckEA)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CheckDA)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.gridControlMedicineTypeAcin);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(707, 560);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // gridControlMedicineTypeAcin
            // 
            this.gridControlMedicineTypeAcin.Location = new System.Drawing.Point(2, 2);
            this.gridControlMedicineTypeAcin.MainView = this.gridViewMedicineTypeAcin;
            this.gridControlMedicineTypeAcin.Name = "gridControlMedicineTypeAcin";
            this.gridControlMedicineTypeAcin.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.RadioEA,
            this.RadioDA,
            this.CheckEA,
            this.CheckDA});
            this.gridControlMedicineTypeAcin.Size = new System.Drawing.Size(703, 556);
            this.gridControlMedicineTypeAcin.TabIndex = 7;
            this.gridControlMedicineTypeAcin.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewMedicineTypeAcin});
            // 
            // gridViewMedicineTypeAcin
            // 
            this.gridViewMedicineTypeAcin.GridControl = this.gridControlMedicineTypeAcin;
            this.gridViewMedicineTypeAcin.Name = "gridViewMedicineTypeAcin";
            this.gridViewMedicineTypeAcin.OptionsView.ColumnAutoWidth = false;
            this.gridViewMedicineTypeAcin.OptionsView.ShowGroupPanel = false;
            this.gridViewMedicineTypeAcin.OptionsView.ShowIndicator = false;
            this.gridViewMedicineTypeAcin.CustomRowCellEdit += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gridViewMedicineTypeAcin_CustomRowCellEdit);
            this.gridViewMedicineTypeAcin.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewMedicineTypeAcin_CustomUnboundColumnData);
            // 
            // RadioEA
            // 
            this.RadioEA.AutoHeight = false;
            this.RadioEA.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.RadioEA.Name = "RadioEA";
            // 
            // RadioDA
            // 
            this.RadioDA.AutoHeight = false;
            this.RadioDA.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.RadioDA.Name = "RadioDA";
            this.RadioDA.ReadOnly = true;
            // 
            // CheckEA
            // 
            this.CheckEA.AutoHeight = false;
            this.CheckEA.Name = "CheckEA";
            // 
            // CheckDA
            // 
            this.CheckDA.AutoHeight = false;
            this.CheckDA.Name = "CheckDA";
            this.CheckDA.ReadOnly = true;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem4});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(707, 560);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.Control = this.gridControlMedicineTypeAcin;
            this.layoutControlItem4.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Size = new System.Drawing.Size(707, 560);
            this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem4.TextVisible = false;
            // 
            // UC_MedicineTypeAcinGrid
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layoutControl1);
            this.Name = "UC_MedicineTypeAcinGrid";
            this.Size = new System.Drawing.Size(707, 560);
            this.Load += new System.EventHandler(this.UC_MedicineTypeAcinGrid_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControlMedicineTypeAcin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewMedicineTypeAcin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RadioEA)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RadioDA)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CheckEA)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CheckDA)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraGrid.GridControl gridControlMedicineTypeAcin;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewMedicineTypeAcin;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit RadioEA;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit RadioDA;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit CheckEA;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit CheckDA;

    }
}
