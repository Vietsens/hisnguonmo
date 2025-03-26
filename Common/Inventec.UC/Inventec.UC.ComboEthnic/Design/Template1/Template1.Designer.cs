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
namespace Inventec.UC.ComboEthnic.Design.Template1
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
            this.lblDanToc = new DevExpress.XtraEditors.LabelControl();
            this.txtMaDanToc = new DevExpress.XtraEditors.TextEdit();
            this.cboDanToc = new DevExpress.XtraEditors.LookUpEdit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMaDanToc.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboDanToc.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // lblDanToc
            // 
            this.lblDanToc.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lblDanToc.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblDanToc.Location = new System.Drawing.Point(14, 8);
            this.lblDanToc.Name = "lblDanToc";
            this.lblDanToc.Size = new System.Drawing.Size(70, 13);
            this.lblDanToc.TabIndex = 0;
            this.lblDanToc.Text = "Dân tộc:";
            // 
            // txtMaDanToc
            // 
            this.txtMaDanToc.Location = new System.Drawing.Point(90, 5);
            this.txtMaDanToc.Name = "txtMaDanToc";
            this.txtMaDanToc.Properties.Appearance.Options.UseTextOptions = true;
            this.txtMaDanToc.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.txtMaDanToc.Properties.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtMaDanToc.Size = new System.Drawing.Size(40, 20);
            this.txtMaDanToc.TabIndex = 1;
            this.txtMaDanToc.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtMaDanToc_PreviewKeyDown);
            // 
            // cboDanToc
            // 
            this.cboDanToc.Location = new System.Drawing.Point(130, 5);
            this.cboDanToc.Name = "cboDanToc";
            this.cboDanToc.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboDanToc.Properties.NullText = "";
            this.cboDanToc.Properties.PopupSizeable = false;
            this.cboDanToc.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
            this.cboDanToc.Size = new System.Drawing.Size(90, 20);
            this.cboDanToc.TabIndex = 2;
            this.cboDanToc.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboDanToc_Closed);
            this.cboDanToc.KeyUp += new System.Windows.Forms.KeyEventHandler(this.cboDanToc_KeyUp);
            // 
            // Template1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cboDanToc);
            this.Controls.Add(this.txtMaDanToc);
            this.Controls.Add(this.lblDanToc);
            this.Name = "Template1";
            this.Size = new System.Drawing.Size(220, 30);
            ((System.ComponentModel.ISupportInitialize)(this.txtMaDanToc.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboDanToc.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.LabelControl lblDanToc;
        private DevExpress.XtraEditors.TextEdit txtMaDanToc;
        private DevExpress.XtraEditors.LookUpEdit cboDanToc;
    }
}
