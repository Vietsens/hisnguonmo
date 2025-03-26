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
namespace Inventec.Common.WitAI
{
    partial class Form1
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnSaveContent = new System.Windows.Forms.Button();
            this.tbContent = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblYou = new System.Windows.Forms.Label();
            this.btnCopyToClipboast = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.chkRepeat = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // btnSaveContent
            // 
            this.btnSaveContent.Location = new System.Drawing.Point(187, 237);
            this.btnSaveContent.Name = "btnSaveContent";
            this.btnSaveContent.Size = new System.Drawing.Size(106, 35);
            this.btnSaveContent.TabIndex = 0;
            this.btnSaveContent.Text = "Sử dụng văn bản";
            this.btnSaveContent.UseVisualStyleBackColor = true;
            this.btnSaveContent.Click += new System.EventHandler(this.btnSaveContent_Click);
            // 
            // tbContent
            // 
            this.tbContent.Location = new System.Drawing.Point(12, 32);
            this.tbContent.Multiline = true;
            this.tbContent.Name = "tbContent";
            this.tbContent.Size = new System.Drawing.Size(474, 128);
            this.tbContent.TabIndex = 1;
            this.tbContent.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tbYou_KeyUp);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(95, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Nội dung văn bản:";
            // 
            // lblYou
            // 
            this.lblYou.Location = new System.Drawing.Point(9, 170);
            this.lblYou.Name = "lblYou";
            this.lblYou.Size = new System.Drawing.Size(393, 58);
            this.lblYou.TabIndex = 3;
            // 
            // btnCopyToClipboast
            // 
            this.btnCopyToClipboast.Location = new System.Drawing.Point(292, 237);
            this.btnCopyToClipboast.Name = "btnCopyToClipboast";
            this.btnCopyToClipboast.Size = new System.Drawing.Size(120, 35);
            this.btnCopyToClipboast.TabIndex = 0;
            this.btnCopyToClipboast.Text = "Copy vào bộ nhớ đệm";
            this.btnCopyToClipboast.UseVisualStyleBackColor = true;
            this.btnCopyToClipboast.Click += new System.EventHandler(this.btnCopyToClipboast_Click);
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(411, 237);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(75, 35);
            this.btnReset.TabIndex = 0;
            this.btnReset.Text = "Làm lại";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // chkRepeat
            // 
            this.chkRepeat.AutoSize = true;
            this.chkRepeat.Location = new System.Drawing.Point(422, 176);
            this.chkRepeat.Name = "chkRepeat";
            this.chkRepeat.Size = new System.Drawing.Size(67, 17);
            this.chkRepeat.TabIndex = 4;
            this.chkRepeat.Text = "Tự động";
            this.chkRepeat.UseVisualStyleBackColor = true;
            this.chkRepeat.Visible = false;
            this.chkRepeat.CheckedChanged += new System.EventHandler(this.chkRepeat_CheckedChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(498, 279);
            this.Controls.Add(this.chkRepeat);
            this.Controls.Add(this.lblYou);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbContent);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.btnCopyToClipboast);
            this.Controls.Add(this.btnSaveContent);
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Nhận dạng tiếng nói";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSaveContent;
        private System.Windows.Forms.TextBox tbContent;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblYou;
        private System.Windows.Forms.Button btnCopyToClipboast;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.CheckBox chkRepeat;
    }
}

