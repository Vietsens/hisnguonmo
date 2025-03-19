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
namespace Inventec.UC.ComboDistrict.Desgin.Template1
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
            this.cboHuyen = new DevExpress.XtraEditors.LookUpEdit();
            this.txtMaHuyen = new DevExpress.XtraEditors.TextEdit();
            this.lblHuyen = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)(this.cboHuyen.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMaHuyen.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // cboHuyen
            // 
            this.cboHuyen.Location = new System.Drawing.Point(130, 5);
            this.cboHuyen.Name = "cboHuyen";
            this.cboHuyen.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboHuyen.Properties.NullText = "";
            this.cboHuyen.Properties.PopupSizeable = false;
            this.cboHuyen.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
            this.cboHuyen.Size = new System.Drawing.Size(90, 20);
            this.cboHuyen.TabIndex = 2;
            this.cboHuyen.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboHuyen_Closed);
            this.cboHuyen.KeyUp += new System.Windows.Forms.KeyEventHandler(this.cboHuyen_KeyUp);
            // 
            // txtMaHuyen
            // 
            this.txtMaHuyen.Location = new System.Drawing.Point(90, 5);
            this.txtMaHuyen.Name = "txtMaHuyen";
            this.txtMaHuyen.Properties.Appearance.Options.UseTextOptions = true;
            this.txtMaHuyen.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.txtMaHuyen.Properties.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtMaHuyen.Size = new System.Drawing.Size(40, 20);
            this.txtMaHuyen.TabIndex = 1;
            this.txtMaHuyen.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtMaHuyen_PreviewKeyDown);
            // 
            // lblHuyen
            // 
            this.lblHuyen.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lblHuyen.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblHuyen.Location = new System.Drawing.Point(14, 8);
            this.lblHuyen.Name = "lblHuyen";
            this.lblHuyen.Size = new System.Drawing.Size(70, 13);
            this.lblHuyen.TabIndex = 1;
            this.lblHuyen.Text = "Huyện:";
            // 
            // Template1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblHuyen);
            this.Controls.Add(this.txtMaHuyen);
            this.Controls.Add(this.cboHuyen);
            this.Name = "Template1";
            this.Size = new System.Drawing.Size(220, 30);
            ((System.ComponentModel.ISupportInitialize)(this.cboHuyen.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMaHuyen.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.LookUpEdit cboHuyen;
        private DevExpress.XtraEditors.TextEdit txtMaHuyen;
        private DevExpress.XtraEditors.LabelControl lblHuyen;
    }
}
