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
namespace Inventec.UC.Loading.Design.Template1
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
            this.progressLoading = new DevExpress.XtraWaitForm.ProgressPanel();
            this.SuspendLayout();
            // 
            // progressLoading
            // 
            this.progressLoading.Appearance.BackColor = System.Drawing.Color.Transparent;
            this.progressLoading.Appearance.Options.UseBackColor = true;
            this.progressLoading.AppearanceCaption.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.progressLoading.AppearanceCaption.Options.UseFont = true;
            this.progressLoading.AppearanceDescription.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold);
            this.progressLoading.AppearanceDescription.Options.UseFont = true;
            this.progressLoading.AppearanceDescription.Options.UseTextOptions = true;
            this.progressLoading.AppearanceDescription.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.progressLoading.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.progressLoading.Caption = "";
            this.progressLoading.ContentAlignment = System.Drawing.ContentAlignment.TopCenter;
            this.progressLoading.Description = " 0%";
            this.progressLoading.Dock = System.Windows.Forms.DockStyle.Fill;
            this.progressLoading.FrameCount = 19000;
            this.progressLoading.Location = new System.Drawing.Point(0, 0);
            this.progressLoading.LookAndFeel.SkinName = "Office 2016 Dark";
            this.progressLoading.Name = "progressLoading";
            this.progressLoading.Size = new System.Drawing.Size(250, 45);
            this.progressLoading.TabIndex = 0;
            this.progressLoading.WaitAnimationType = DevExpress.Utils.Animation.WaitingAnimatorType.Line;
            // 
            // Template1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.progressLoading);
            this.Name = "Template1";
            this.Size = new System.Drawing.Size(250, 45);
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraWaitForm.ProgressPanel progressLoading;
    }
}
