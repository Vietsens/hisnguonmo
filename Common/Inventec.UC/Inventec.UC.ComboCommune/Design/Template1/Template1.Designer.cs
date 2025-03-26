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
namespace Inventec.UC.ComboCommune.Design.Template1
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
            this.cboPhuongXa = new DevExpress.XtraEditors.LookUpEdit();
            this.txtMaPhuongXa = new DevExpress.XtraEditors.TextEdit();
            this.lblPhuongXa = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)(this.cboPhuongXa.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMaPhuongXa.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // cboPhuongXa
            // 
            this.cboPhuongXa.Location = new System.Drawing.Point(130, 5);
            this.cboPhuongXa.Name = "cboPhuongXa";
            this.cboPhuongXa.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboPhuongXa.Properties.NullText = "";
            this.cboPhuongXa.Properties.PopupSizeable = false;
            this.cboPhuongXa.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
            this.cboPhuongXa.Size = new System.Drawing.Size(90, 20);
            this.cboPhuongXa.TabIndex = 2;
            this.cboPhuongXa.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboPhuongXa_Closed);
            this.cboPhuongXa.KeyUp += new System.Windows.Forms.KeyEventHandler(this.cboPhuongXa_KeyUp);
            // 
            // txtMaPhuongXa
            // 
            this.txtMaPhuongXa.Location = new System.Drawing.Point(90, 5);
            this.txtMaPhuongXa.Name = "txtMaPhuongXa";
            this.txtMaPhuongXa.Properties.Appearance.Options.UseTextOptions = true;
            this.txtMaPhuongXa.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.txtMaPhuongXa.Properties.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtMaPhuongXa.Size = new System.Drawing.Size(40, 20);
            this.txtMaPhuongXa.TabIndex = 1;
            this.txtMaPhuongXa.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtMaPhuongXa_PreviewKeyDown);
            // 
            // lblPhuongXa
            // 
            this.lblPhuongXa.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lblPhuongXa.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblPhuongXa.Location = new System.Drawing.Point(14, 8);
            this.lblPhuongXa.Name = "lblPhuongXa";
            this.lblPhuongXa.Size = new System.Drawing.Size(70, 13);
            this.lblPhuongXa.TabIndex = 0;
            this.lblPhuongXa.Text = "Xã:";
            // 
            // Template1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblPhuongXa);
            this.Controls.Add(this.txtMaPhuongXa);
            this.Controls.Add(this.cboPhuongXa);
            this.Name = "Template1";
            this.Size = new System.Drawing.Size(220, 30);
            ((System.ComponentModel.ISupportInitialize)(this.cboPhuongXa.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMaPhuongXa.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.LookUpEdit cboPhuongXa;
        private DevExpress.XtraEditors.TextEdit txtMaPhuongXa;
        private DevExpress.XtraEditors.LabelControl lblPhuongXa;
    }
}
