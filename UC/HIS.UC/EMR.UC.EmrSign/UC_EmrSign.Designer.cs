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
namespace EMR.UC.EmrSign
{
    partial class UC_EmrSign
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
            this.gridControlEmrSign = new DevExpress.XtraGrid.GridControl();
            this.gridViewEmrSign = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.RadioEnable = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.RadioDisable = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.CheckEnable = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.CheckDisable = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlEmrSign)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewEmrSign)).BeginInit();
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
            this.layoutControl1.Controls.Add(this.gridControlEmrSign);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(679, 467);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // gridControlEmrSign
            // 
            this.gridControlEmrSign.Location = new System.Drawing.Point(2, 2);
            this.gridControlEmrSign.MainView = this.gridViewEmrSign;
            this.gridControlEmrSign.Margin = new System.Windows.Forms.Padding(2);
            this.gridControlEmrSign.Name = "gridControlEmrSign";
            this.gridControlEmrSign.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.RadioEnable,
            this.RadioDisable,
            this.CheckEnable,
            this.CheckDisable});
            this.gridControlEmrSign.Size = new System.Drawing.Size(675, 463);
            this.gridControlEmrSign.TabIndex = 4;
            this.gridControlEmrSign.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewEmrSign});
            // 
            // gridViewEmrSign
            // 
            this.gridViewEmrSign.GridControl = this.gridControlEmrSign;
            this.gridViewEmrSign.Name = "gridViewEmrSign";
            this.gridViewEmrSign.OptionsCustomization.AllowFilter = false;
            this.gridViewEmrSign.OptionsCustomization.AllowSort = false;
            this.gridViewEmrSign.OptionsView.ShowGroupPanel = false;
            this.gridViewEmrSign.OptionsView.ShowIndicator = false;
            this.gridViewEmrSign.CustomRowCellEdit += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gridViewEmrSign_CustomRowCellEdit);
            this.gridViewEmrSign.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gridViewEmrSign_CellValueChanged);
            this.gridViewEmrSign.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gridViewEmrSign_MouseDown);
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
            this.layoutControlItem1.Control = this.gridControlEmrSign;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(679, 467);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // UC_EmrSign
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layoutControl1);
            this.Name = "UC_EmrSign";
            this.Size = new System.Drawing.Size(679, 467);
            this.Load += new System.EventHandler(this.UC_EmrSign_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControlEmrSign)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewEmrSign)).EndInit();
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
        private DevExpress.XtraGrid.GridControl gridControlEmrSign;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewEmrSign;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit RadioEnable;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit RadioDisable;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit CheckEnable;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit CheckDisable;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
    }
}
