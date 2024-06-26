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
namespace HIS.Desktop.Plugins.AssignPrescriptionPK.MultiDate
{
    partial class UCMultiDate
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
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.dtIntructionTime = new DevExpress.XtraEditors.DateEdit();
            ((System.ComponentModel.ISupportInitialize)(this.dtIntructionTime.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtIntructionTime.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // labelControl1
            // 
            this.labelControl1.Appearance.ForeColor = System.Drawing.Color.Maroon;
            this.labelControl1.Location = new System.Drawing.Point(3, 6);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(86, 13);
            this.labelControl1.TabIndex = 7;
            this.labelControl1.Text = "Thời gian chỉ định:";
            // 
            // dtIntructionTime
            // 
            this.dtIntructionTime.EditValue = null;
            this.dtIntructionTime.Location = new System.Drawing.Point(97, 3);
            this.dtIntructionTime.Name = "dtIntructionTime";
            this.dtIntructionTime.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtIntructionTime.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtIntructionTime.Properties.DisplayFormat.FormatString = "dd/MM/yyyy HH:mm";
            this.dtIntructionTime.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtIntructionTime.Properties.EditFormat.FormatString = "dd/MM/yyyy HH:mm";
            this.dtIntructionTime.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtIntructionTime.Properties.Mask.EditMask = "dd/MM/yyyy HH:mm";
            this.dtIntructionTime.Size = new System.Drawing.Size(136, 20);
            this.dtIntructionTime.TabIndex = 6;
            // 
            // UCMultiDate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.labelControl1);
            this.Controls.Add(this.dtIntructionTime);
            this.Name = "UCMultiDate";
            this.Size = new System.Drawing.Size(243, 27);
            this.Load += new System.EventHandler(this.UCMultiDate_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dtIntructionTime.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtIntructionTime.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.DateEdit dtIntructionTime;
    }
}
