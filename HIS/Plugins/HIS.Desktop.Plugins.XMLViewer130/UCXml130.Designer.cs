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
namespace HIS.Desktop.Plugins.XMLViewer130
{
    partial class UCXml130
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
            this.gridControllXml = new DevExpress.XtraGrid.GridControl();
            this.gridViewXml = new DevExpress.XtraGrid.Views.Grid.GridView();
            ((System.ComponentModel.ISupportInitialize)(this.gridControllXml)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewXml)).BeginInit();
            this.SuspendLayout();
            // 
            // gridControllXml
            // 
            this.gridControllXml.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControllXml.Location = new System.Drawing.Point(0, 0);
            this.gridControllXml.MainView = this.gridViewXml;
            this.gridControllXml.Name = "gridControllXml";
            this.gridControllXml.Size = new System.Drawing.Size(723, 422);
            this.gridControllXml.TabIndex = 0;
            this.gridControllXml.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewXml});
            // 
            // gridViewXml
            // 
            this.gridViewXml.GridControl = this.gridControllXml;
            this.gridViewXml.Name = "gridViewXml";
            this.gridViewXml.OptionsSelection.MultiSelect = true;
            this.gridViewXml.OptionsView.ColumnAutoWidth = false;
            this.gridViewXml.OptionsView.ShowGroupPanel = false;
            this.gridViewXml.CustomColumnDisplayText += new DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventHandler(this.gridViewXml_CustomColumnDisplayText);
            // 
            // UCXml130
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gridControllXml);
            this.Name = "UCXml130";
            this.Size = new System.Drawing.Size(723, 422);
            ((System.ComponentModel.ISupportInitialize)(this.gridControllXml)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewXml)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraGrid.GridControl gridControllXml;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewXml;
    }
}
