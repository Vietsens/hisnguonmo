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
namespace Inventec.UC.ComboNational.Design.Template1
{
    partial class Template1
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
            this.lblQuocTich = new DevExpress.XtraEditors.LabelControl();
            this.txtMaQuocTich = new DevExpress.XtraEditors.TextEdit();
            this.cboQuocTich = new DevExpress.XtraEditors.LookUpEdit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMaQuocTich.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboQuocTich.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // lblQuocTich
            // 
            this.lblQuocTich.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lblQuocTich.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblQuocTich.Location = new System.Drawing.Point(14, 8);
            this.lblQuocTich.Name = "lblQuocTich";
            this.lblQuocTich.Size = new System.Drawing.Size(70, 13);
            this.lblQuocTich.TabIndex = 0;
            this.lblQuocTich.Text = "Quốc tịch:";
            // 
            // txtMaQuocTich
            // 
            this.txtMaQuocTich.Location = new System.Drawing.Point(90, 5);
            this.txtMaQuocTich.Name = "txtMaQuocTich";
            this.txtMaQuocTich.Properties.Appearance.Options.UseTextOptions = true;
            this.txtMaQuocTich.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.txtMaQuocTich.Properties.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtMaQuocTich.Size = new System.Drawing.Size(40, 20);
            this.txtMaQuocTich.TabIndex = 1;
            this.txtMaQuocTich.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtMaQuocTich_PreviewKeyDown);
            // 
            // cboQuocTich
            // 
            this.cboQuocTich.Location = new System.Drawing.Point(130, 5);
            this.cboQuocTich.Name = "cboQuocTich";
            this.cboQuocTich.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboQuocTich.Properties.NullText = "";
            this.cboQuocTich.Properties.PopupSizeable = false;
            this.cboQuocTich.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
            this.cboQuocTich.Size = new System.Drawing.Size(90, 20);
            this.cboQuocTich.TabIndex = 2;
            this.cboQuocTich.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboQuocTich_Closed);
            this.cboQuocTich.KeyUp += new System.Windows.Forms.KeyEventHandler(this.cboQuocTich_KeyUp);
            // 
            // Template1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cboQuocTich);
            this.Controls.Add(this.txtMaQuocTich);
            this.Controls.Add(this.lblQuocTich);
            this.Name = "Template1";
            this.Size = new System.Drawing.Size(220, 30);
            ((System.ComponentModel.ISupportInitialize)(this.txtMaQuocTich.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboQuocTich.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.LabelControl lblQuocTich;
        private DevExpress.XtraEditors.TextEdit txtMaQuocTich;
        private DevExpress.XtraEditors.LookUpEdit cboQuocTich;
    }
}
