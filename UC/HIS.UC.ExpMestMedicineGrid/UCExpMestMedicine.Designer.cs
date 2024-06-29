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
namespace HIS.UC.ExpMestMedicineGrid
{
    partial class UCExpMestMedicine
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
            this.gridControlExpMestMedicine = new DevExpress.XtraGrid.GridControl();
            this.gridViewExpMestMedicine = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.txtKeyword = new DevExpress.XtraEditors.TextEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutKeyword = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlExpMestMedicine)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewExpMestMedicine)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtKeyword.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutKeyword)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.gridControlExpMestMedicine);
            this.layoutControl1.Controls.Add(this.txtKeyword);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(660, 440);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // gridControlExpMestMedicine
            // 
            this.gridControlExpMestMedicine.Location = new System.Drawing.Point(2, 26);
            this.gridControlExpMestMedicine.MainView = this.gridViewExpMestMedicine;
            this.gridControlExpMestMedicine.Name = "gridControlExpMestMedicine";
            this.gridControlExpMestMedicine.Size = new System.Drawing.Size(656, 412);
            this.gridControlExpMestMedicine.TabIndex = 5;
            this.gridControlExpMestMedicine.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewExpMestMedicine});
            // 
            // gridViewExpMestMedicine
            // 
            this.gridViewExpMestMedicine.GridControl = this.gridControlExpMestMedicine;
            this.gridViewExpMestMedicine.Name = "gridViewExpMestMedicine";
            this.gridViewExpMestMedicine.OptionsBehavior.AutoPopulateColumns = false;
            this.gridViewExpMestMedicine.OptionsView.ColumnAutoWidth = false;
            this.gridViewExpMestMedicine.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            this.gridViewExpMestMedicine.OptionsView.ShowGroupPanel = false;
            this.gridViewExpMestMedicine.OptionsView.ShowIndicator = false;
            this.gridViewExpMestMedicine.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewExpMestMedicine_CustomUnboundColumnData);
            // 
            // txtKeyword
            // 
            this.txtKeyword.Location = new System.Drawing.Point(2, 2);
            this.txtKeyword.Name = "txtKeyword";
            this.txtKeyword.Size = new System.Drawing.Size(656, 20);
            this.txtKeyword.StyleController = this.layoutControl1;
            this.txtKeyword.TabIndex = 4;
            this.txtKeyword.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtKeyword_KeyUp);
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutKeyword,
            this.layoutControlItem2});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(660, 440);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutKeyword
            // 
            this.layoutKeyword.Control = this.txtKeyword;
            this.layoutKeyword.Location = new System.Drawing.Point(0, 0);
            this.layoutKeyword.Name = "layoutKeyword";
            this.layoutKeyword.Size = new System.Drawing.Size(660, 24);
            this.layoutKeyword.TextSize = new System.Drawing.Size(0, 0);
            this.layoutKeyword.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.gridControlExpMestMedicine;
            this.layoutControlItem2.Location = new System.Drawing.Point(0, 24);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(660, 416);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // UCExpMestMedicine
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layoutControl1);
            this.Name = "UCExpMestMedicine";
            this.Size = new System.Drawing.Size(660, 440);
            this.Load += new System.EventHandler(this.UCExpMestMedicine_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControlExpMestMedicine)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewExpMestMedicine)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtKeyword.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutKeyword)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraGrid.GridControl gridControlExpMestMedicine;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewExpMestMedicine;
        private DevExpress.XtraEditors.TextEdit txtKeyword;
        private DevExpress.XtraLayout.LayoutControlItem layoutKeyword;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
    }
}
