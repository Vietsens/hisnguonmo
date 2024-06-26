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
namespace HIS.UC.RehaServiceType
{
    partial class UCRehaServiceType
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
            this.gridControlRehaServiceType = new DevExpress.XtraGrid.GridControl();
            this.gridViewRehaServiceType = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.repositoryItemCheck__Enable = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.repositoryItemCheck__Disable = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.repositoryItemRadio_Enable = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.repositoryItemRadio_Disable = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlRehaServiceType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewRehaServiceType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheck__Enable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheck__Disable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemRadio_Enable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemRadio_Disable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.gridControlRehaServiceType);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(805, 506);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // gridControlRehaServiceType
            // 
            this.gridControlRehaServiceType.Location = new System.Drawing.Point(3, 3);
            this.gridControlRehaServiceType.MainView = this.gridViewRehaServiceType;
            this.gridControlRehaServiceType.Name = "gridControlRehaServiceType";
            this.gridControlRehaServiceType.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemCheck__Enable,
            this.repositoryItemCheck__Disable,
            this.repositoryItemRadio_Enable,
            this.repositoryItemRadio_Disable});
            this.gridControlRehaServiceType.Size = new System.Drawing.Size(799, 500);
            this.gridControlRehaServiceType.TabIndex = 4;
            this.gridControlRehaServiceType.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewRehaServiceType});
            // 
            // gridViewRehaServiceType
            // 
            this.gridViewRehaServiceType.GridControl = this.gridControlRehaServiceType;
            this.gridViewRehaServiceType.Name = "gridViewRehaServiceType";
            this.gridViewRehaServiceType.OptionsCustomization.AllowFilter = false;
            this.gridViewRehaServiceType.OptionsCustomization.AllowSort = false;
            this.gridViewRehaServiceType.OptionsView.ShowGroupPanel = false;
            this.gridViewRehaServiceType.OptionsView.ShowIndicator = false;
            this.gridViewRehaServiceType.CustomRowCellEdit += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gridViewRehaServiceType_CustomRowCellEdit);
            this.gridViewRehaServiceType.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gridViewRehaServiceType_MouseDown);
            // 
            // repositoryItemCheck__Enable
            // 
            this.repositoryItemCheck__Enable.AutoHeight = false;
            this.repositoryItemCheck__Enable.Name = "repositoryItemCheck__Enable";
            // 
            // repositoryItemCheck__Disable
            // 
            this.repositoryItemCheck__Disable.AutoHeight = false;
            this.repositoryItemCheck__Disable.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Style2;
            this.repositoryItemCheck__Disable.Name = "repositoryItemCheck__Disable";
            this.repositoryItemCheck__Disable.ReadOnly = true;
            // 
            // repositoryItemRadio_Enable
            // 
            this.repositoryItemRadio_Enable.AutoHeight = false;
            this.repositoryItemRadio_Enable.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.repositoryItemRadio_Enable.Name = "repositoryItemRadio_Enable";
            this.repositoryItemRadio_Enable.Click += new System.EventHandler(this.repositoryItemRadio_Enable_Click);
            // 
            // repositoryItemRadio_Disable
            // 
            this.repositoryItemRadio_Disable.AutoHeight = false;
            this.repositoryItemRadio_Disable.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.repositoryItemRadio_Disable.Name = "repositoryItemRadio_Disable";
            this.repositoryItemRadio_Disable.ReadOnly = true;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.OptionsItemText.TextToControlDistance = 4;
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(805, 506);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.gridControlRehaServiceType;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(805, 506);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // UCRehaServiceType
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layoutControl1);
            this.Name = "UCRehaServiceType";
            this.Size = new System.Drawing.Size(805, 506);
            this.Load += new System.EventHandler(this.UCRehaServiceType_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControlRehaServiceType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewRehaServiceType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheck__Enable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheck__Disable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemRadio_Enable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemRadio_Disable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraGrid.GridControl gridControlRehaServiceType;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewRehaServiceType;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheck__Enable;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheck__Disable;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemRadio_Enable;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemRadio_Disable;
    }
}
