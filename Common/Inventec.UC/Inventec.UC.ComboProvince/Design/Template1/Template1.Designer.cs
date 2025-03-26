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
namespace Inventec.UC.ComboProvince.Design.Template1
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
            this.cboTinh = new DevExpress.XtraEditors.LookUpEdit();
            this.txtMaTinh = new DevExpress.XtraEditors.TextEdit();
            this.lblTinh = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)(this.cboTinh.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMaTinh.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // cboTinh
            // 
            this.cboTinh.Location = new System.Drawing.Point(130, 5);
            this.cboTinh.Name = "cboTinh";
            this.cboTinh.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboTinh.Properties.NullText = "";
            this.cboTinh.Properties.PopupSizeable = false;
            this.cboTinh.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
            this.cboTinh.Size = new System.Drawing.Size(90, 20);
            this.cboTinh.TabIndex = 2;
            this.cboTinh.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboTinh_Closed);
            this.cboTinh.KeyUp += new System.Windows.Forms.KeyEventHandler(this.cboTinh_KeyUp);
            // 
            // txtMaTinh
            // 
            this.txtMaTinh.Location = new System.Drawing.Point(90, 5);
            this.txtMaTinh.Name = "txtMaTinh";
            this.txtMaTinh.Properties.Appearance.Options.UseTextOptions = true;
            this.txtMaTinh.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.txtMaTinh.Properties.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtMaTinh.Size = new System.Drawing.Size(40, 20);
            this.txtMaTinh.TabIndex = 1;
            this.txtMaTinh.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtMaTinh_PreviewKeyDown);
            // 
            // lblTinh
            // 
            this.lblTinh.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lblTinh.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblTinh.Location = new System.Drawing.Point(14, 8);
            this.lblTinh.Name = "lblTinh";
            this.lblTinh.Size = new System.Drawing.Size(70, 13);
            this.lblTinh.TabIndex = 1;
            this.lblTinh.Text = "Tỉnh:";
            // 
            // Template1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblTinh);
            this.Controls.Add(this.txtMaTinh);
            this.Controls.Add(this.cboTinh);
            this.Name = "Template1";
            this.Size = new System.Drawing.Size(220, 30);
            ((System.ComponentModel.ISupportInitialize)(this.cboTinh.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMaTinh.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.LookUpEdit cboTinh;
        private DevExpress.XtraEditors.TextEdit txtMaTinh;
        private DevExpress.XtraEditors.LabelControl lblTinh;
    }
}
