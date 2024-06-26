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
namespace HIS.UC.HisServiceGoup
{
    partial class UCServiceGroup
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
            this.gridControlServiceGroup = new DevExpress.XtraGrid.GridControl();
            this.gridViewServiceGroup = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.RadioE = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.RadioD = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.CheckE = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.CheckD = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.CheckEditIsPublic = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlServiceGroup)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewServiceGroup)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RadioE)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RadioD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CheckE)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CheckD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CheckEditIsPublic)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.gridControlServiceGroup);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(707, 560);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // gridControlServiceGroup
            // 
            this.gridControlServiceGroup.Location = new System.Drawing.Point(2, 2);
            this.gridControlServiceGroup.MainView = this.gridViewServiceGroup;
            this.gridControlServiceGroup.Name = "gridControlServiceGroup";
            this.gridControlServiceGroup.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.RadioE,
            this.RadioD,
            this.CheckE,
            this.CheckD,
            this.CheckEditIsPublic});
            this.gridControlServiceGroup.Size = new System.Drawing.Size(703, 556);
            this.gridControlServiceGroup.TabIndex = 7;
            this.gridControlServiceGroup.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewServiceGroup});
            // 
            // gridViewServiceGroup
            // 
            this.gridViewServiceGroup.GridControl = this.gridControlServiceGroup;
            this.gridViewServiceGroup.Name = "gridViewServiceGroup";
            this.gridViewServiceGroup.OptionsCustomization.AllowFilter = false;
            this.gridViewServiceGroup.OptionsCustomization.AllowSort = false;
            this.gridViewServiceGroup.OptionsView.ShowGroupPanel = false;
            this.gridViewServiceGroup.OptionsView.ShowIndicator = false;
            this.gridViewServiceGroup.RowCellClick += new DevExpress.XtraGrid.Views.Grid.RowCellClickEventHandler(this.gridViewMedicineType_RowCellClick);
            this.gridViewServiceGroup.CustomRowCellEdit += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gridViewMedicineType_CustomRowCellEdit);
            this.gridViewServiceGroup.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewMedicineType_CustomUnboundColumnData);
            this.gridViewServiceGroup.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gridViewServiceGroup_MouseDown);
            // 
            // RadioE
            // 
            this.RadioE.AutoHeight = false;
            this.RadioE.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.RadioE.Name = "RadioE";
            this.RadioE.Click += new System.EventHandler(this.RadioEnable_Click);
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
            this.layoutControlItem4});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(707, 560);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.Control = this.gridControlServiceGroup;
            this.layoutControlItem4.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Size = new System.Drawing.Size(707, 560);
            this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem4.TextVisible = false;
            // 
            // CheckEditIsPublic
            // 
            this.CheckEditIsPublic.AutoHeight = false;
            this.CheckEditIsPublic.Name = "CheckEditIsPublic";
            // 
            // UCServiceGroup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layoutControl1);
            this.Name = "UCServiceGroup";
            this.Size = new System.Drawing.Size(707, 560);
            this.Load += new System.EventHandler(this.UC_MedicineTypeGrid_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControlServiceGroup)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewServiceGroup)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RadioE)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RadioD)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CheckE)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CheckD)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CheckEditIsPublic)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraGrid.GridControl gridControlServiceGroup;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewServiceGroup;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit RadioE;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit RadioD;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit CheckE;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit CheckD;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit CheckEditIsPublic;

    }
}
