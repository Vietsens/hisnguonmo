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
namespace Inventec.UC.ComboTHX.Design.Template1
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
            this.lblTHX = new DevExpress.XtraEditors.LabelControl();
            this.txtMaTHX = new DevExpress.XtraEditors.TextEdit();
            this.cboTHX = new DevExpress.XtraEditors.LookUpEdit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMaTHX.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboTHX.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // lblTHX
            // 
            this.lblTHX.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lblTHX.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblTHX.Location = new System.Drawing.Point(14, 8);
            this.lblTHX.Name = "lblTHX";
            this.lblTHX.Size = new System.Drawing.Size(70, 13);
            this.lblTHX.TabIndex = 0;
            this.lblTHX.Text = "T/H/X:";
            // 
            // txtMaTHX
            // 
            this.txtMaTHX.Location = new System.Drawing.Point(90, 5);
            this.txtMaTHX.Name = "txtMaTHX";
            this.txtMaTHX.Properties.Appearance.Options.UseTextOptions = true;
            this.txtMaTHX.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.txtMaTHX.Properties.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtMaTHX.Size = new System.Drawing.Size(80, 20);
            this.txtMaTHX.TabIndex = 1;
            this.txtMaTHX.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtMaTHX_PreviewKeyDown);
            // 
            // cboTHX
            // 
            this.cboTHX.Location = new System.Drawing.Point(170, 5);
            this.cboTHX.Name = "cboTHX";
            this.cboTHX.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboTHX.Properties.NullText = "";
            this.cboTHX.Properties.PopupSizeable = false;
            this.cboTHX.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
            this.cboTHX.Properties.GetNotInListValue += new DevExpress.XtraEditors.Controls.GetNotInListValueEventHandler(this.cboTHX_Properties_GetNotInListValue);
            this.cboTHX.Size = new System.Drawing.Size(490, 20);
            this.cboTHX.TabIndex = 2;
            this.cboTHX.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboTHX_Closed);
            this.cboTHX.KeyUp += new System.Windows.Forms.KeyEventHandler(this.cboTHX_KeyUp);
            // 
            // Template1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cboTHX);
            this.Controls.Add(this.txtMaTHX);
            this.Controls.Add(this.lblTHX);
            this.Name = "Template1";
            this.Size = new System.Drawing.Size(660, 30);
            ((System.ComponentModel.ISupportInitialize)(this.txtMaTHX.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboTHX.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.LabelControl lblTHX;
        private DevExpress.XtraEditors.TextEdit txtMaTHX;
        private DevExpress.XtraEditors.LookUpEdit cboTHX;
    }
}
