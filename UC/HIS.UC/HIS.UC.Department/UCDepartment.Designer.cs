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
namespace HIS.UC.Department
{
    partial class UCDepartment
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
            this.gridControlDepartment = new DevExpress.XtraGrid.GridControl();
            this.gridViewDepartment = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.repositoryItemCheck__Disable = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.repositoryItemCheck__Enable = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.repositoryItemRadio_Enable = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.repositoryItemRadio_Disable = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlDepartment)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewDepartment)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheck__Disable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheck__Enable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemRadio_Enable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemRadio_Disable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.gridControlDepartment);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(656, 417);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // gridControlDepartment
            // 
            this.gridControlDepartment.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.gridControlDepartment.Location = new System.Drawing.Point(2, 2);
            this.gridControlDepartment.MainView = this.gridViewDepartment;
            this.gridControlDepartment.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.gridControlDepartment.Name = "gridControlDepartment";
            this.gridControlDepartment.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemCheck__Disable,
            this.repositoryItemCheck__Enable,
            this.repositoryItemRadio_Enable,
            this.repositoryItemRadio_Disable});
            this.gridControlDepartment.Size = new System.Drawing.Size(652, 413);
            this.gridControlDepartment.TabIndex = 1;
            this.gridControlDepartment.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewDepartment});
            // 
            // gridViewDepartment
            // 
            this.gridViewDepartment.GridControl = this.gridControlDepartment;
            this.gridViewDepartment.Name = "gridViewDepartment";
            this.gridViewDepartment.OptionsCustomization.AllowFilter = false;
            this.gridViewDepartment.OptionsCustomization.AllowSort = false;
            this.gridViewDepartment.OptionsView.ShowGroupPanel = false;
            this.gridViewDepartment.OptionsView.ShowIndicator = false;
            this.gridViewDepartment.CustomRowCellEdit += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gridViewDepartment_CustomRowCellEdit);
            this.gridViewDepartment.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gridViewDepartment_MouseDown);
            // 
            // repositoryItemCheck__Disable
            // 
            this.repositoryItemCheck__Disable.AutoHeight = false;
            this.repositoryItemCheck__Disable.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Style2;
            this.repositoryItemCheck__Disable.Name = "repositoryItemCheck__Disable";
            this.repositoryItemCheck__Disable.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
            this.repositoryItemCheck__Disable.ReadOnly = true;
            // 
            // repositoryItemCheck__Enable
            // 
            this.repositoryItemCheck__Enable.AutoHeight = false;
            this.repositoryItemCheck__Enable.Name = "repositoryItemCheck__Enable";
            this.repositoryItemCheck__Enable.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
            // 
            // repositoryItemRadio_Enable
            // 
            this.repositoryItemRadio_Enable.AutoHeight = false;
            this.repositoryItemRadio_Enable.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.repositoryItemRadio_Enable.Name = "repositoryItemRadio_Enable";
            this.repositoryItemRadio_Enable.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
            this.repositoryItemRadio_Enable.Click += new System.EventHandler(this.repositoryItemRadio_Enable_Click);
            // 
            // repositoryItemRadio_Disable
            // 
            this.repositoryItemRadio_Disable.AutoHeight = false;
            this.repositoryItemRadio_Disable.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.repositoryItemRadio_Disable.Name = "repositoryItemRadio_Disable";
            this.repositoryItemRadio_Disable.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
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
            this.layoutControlGroup1.Size = new System.Drawing.Size(656, 417);
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.gridControlDepartment;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(656, 417);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // UCDepartment
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layoutControl1);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "UCDepartment";
            this.Size = new System.Drawing.Size(656, 417);
            this.Load += new System.EventHandler(this.UCDepartment_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControlDepartment)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewDepartment)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheck__Disable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheck__Enable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemRadio_Enable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemRadio_Disable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraGrid.GridControl gridControlDepartment;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewDepartment;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheck__Disable;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheck__Enable;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemRadio_Enable;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemRadio_Disable;
    }
}
