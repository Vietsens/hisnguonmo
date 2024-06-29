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
namespace HIS.UC.ActiveIngredent
{
    partial class UC_ActiveIngredent
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
            this.gridControlActiveIngredent = new DevExpress.XtraGrid.GridControl();
            this.gridViewActiveIngredent = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.RadioEnable = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.RadioDisable = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.CheckEnable = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.CheckDisable = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlActiveIngredent)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewActiveIngredent)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RadioEnable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RadioDisable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CheckEnable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CheckDisable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.gridControlActiveIngredent);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(679, 467);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // gridControlActiveIngredent
            // 
            this.gridControlActiveIngredent.Location = new System.Drawing.Point(2, 2);
            this.gridControlActiveIngredent.MainView = this.gridViewActiveIngredent;
            this.gridControlActiveIngredent.Margin = new System.Windows.Forms.Padding(2);
            this.gridControlActiveIngredent.Name = "gridControlActiveIngredent";
            this.gridControlActiveIngredent.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.RadioEnable,
            this.RadioDisable,
            this.CheckEnable,
            this.CheckDisable});
            this.gridControlActiveIngredent.Size = new System.Drawing.Size(675, 463);
            this.gridControlActiveIngredent.TabIndex = 4;
            this.gridControlActiveIngredent.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewActiveIngredent});
            // 
            // gridViewActiveIngredent
            // 
            this.gridViewActiveIngredent.GridControl = this.gridControlActiveIngredent;
            this.gridViewActiveIngredent.Name = "gridViewActiveIngredent";
            this.gridViewActiveIngredent.OptionsCustomization.AllowFilter = false;
            this.gridViewActiveIngredent.OptionsCustomization.AllowSort = false;
            this.gridViewActiveIngredent.OptionsView.ColumnAutoWidth = false;
            this.gridViewActiveIngredent.OptionsView.ShowGroupPanel = false;
            this.gridViewActiveIngredent.OptionsView.ShowIndicator = false;
            this.gridViewActiveIngredent.CustomRowCellEdit += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gridViewActiveIngredent_CustomRowCellEdit);
            this.gridViewActiveIngredent.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gridViewActiveIngredent_CellValueChanged);
            this.gridViewActiveIngredent.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gridViewActiveIngredent_MouseDown);
            // 
            // RadioEnable
            // 
            this.RadioEnable.AutoHeight = false;
            this.RadioEnable.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.RadioEnable.Name = "RadioEnable";
            this.RadioEnable.Click += new System.EventHandler(this.RadioEnable_Click);
            // 
            // RadioDisable
            // 
            this.RadioDisable.AutoHeight = false;
            this.RadioDisable.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.RadioDisable.Name = "RadioDisable";
            this.RadioDisable.ReadOnly = true;
            // 
            // CheckEnable
            // 
            this.CheckEnable.AutoHeight = false;
            this.CheckEnable.Name = "CheckEnable";
            this.CheckEnable.CheckedChanged += new System.EventHandler(this.CheckEnable_CheckedChanged);
            this.CheckEnable.Click += new System.EventHandler(this.CheckEnable_Click);
            // 
            // CheckDisable
            // 
            this.CheckDisable.AutoHeight = false;
            this.CheckDisable.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Style2;
            this.CheckDisable.Name = "CheckDisable";
            this.CheckDisable.ReadOnly = true;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(679, 467);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.gridControlActiveIngredent;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(679, 467);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // UC_ActiveIngredent
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layoutControl1);
            this.Name = "UC_ActiveIngredent";
            this.Size = new System.Drawing.Size(679, 467);
            this.Load += new System.EventHandler(this.UC_ActiveIngredent_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControlActiveIngredent)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewActiveIngredent)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RadioEnable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RadioDisable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CheckEnable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CheckDisable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraGrid.GridControl gridControlActiveIngredent;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewActiveIngredent;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit RadioEnable;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit RadioDisable;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit CheckEnable;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit CheckDisable;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
    }
}
