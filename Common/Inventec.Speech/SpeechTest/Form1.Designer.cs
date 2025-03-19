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
namespace SpeechTest
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
            this.button1 = new System.Windows.Forms.Button();
            this.tbTenBn = new System.Windows.Forms.TextBox();
            this.tbStt = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tbTenPhong = new System.Windows.Forms.TextBox();
            this.tbMoiBenhNhan = new System.Windows.Forms.TextBox();
            this.tbCoSoTT = new System.Windows.Forms.TextBox();
            this.tbDen = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(181, 234);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(66, 23);
            this.button1.TabIndex = 7;
            this.button1.Text = "Speech";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // tbTenBn
            // 
            this.tbTenBn.Location = new System.Drawing.Point(68, 48);
            this.tbTenBn.Name = "tbTenBn";
            this.tbTenBn.Size = new System.Drawing.Size(179, 20);
            this.tbTenBn.TabIndex = 2;
            this.tbTenBn.Text = "Lê Hoàng Quân";
            // 
            // tbStt
            // 
            this.tbStt.Location = new System.Drawing.Point(68, 121);
            this.tbStt.MaxLength = 8;
            this.tbStt.Name = "tbStt";
            this.tbStt.Size = new System.Drawing.Size(108, 20);
            this.tbStt.TabIndex = 4;
            this.tbStt.Text = "152348";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(30, 124);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(28, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "STT";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 51);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(44, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Tên BN";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(-1, 199);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Tên phòng";
            // 
            // tbTenPhong
            // 
            this.tbTenPhong.Location = new System.Drawing.Point(68, 196);
            this.tbTenPhong.MaxLength = 1000;
            this.tbTenPhong.Name = "tbTenPhong";
            this.tbTenPhong.Size = new System.Drawing.Size(179, 20);
            this.tbTenPhong.TabIndex = 6;
            this.tbTenPhong.Text = "khám mắt";
            // 
            // tbMoiBenhNhan
            // 
            this.tbMoiBenhNhan.Location = new System.Drawing.Point(68, 10);
            this.tbMoiBenhNhan.Name = "tbMoiBenhNhan";
            this.tbMoiBenhNhan.Size = new System.Drawing.Size(179, 20);
            this.tbMoiBenhNhan.TabIndex = 1;
            this.tbMoiBenhNhan.Text = "Mời bệnh nhân";
            // 
            // tbCoSoTT
            // 
            this.tbCoSoTT.Location = new System.Drawing.Point(68, 86);
            this.tbCoSoTT.Name = "tbCoSoTT";
            this.tbCoSoTT.Size = new System.Drawing.Size(179, 20);
            this.tbCoSoTT.TabIndex = 3;
            this.tbCoSoTT.Text = "có số thứ tự";
            // 
            // tbDen
            // 
            this.tbDen.Location = new System.Drawing.Point(68, 155);
            this.tbDen.Name = "tbDen";
            this.tbDen.Size = new System.Drawing.Size(179, 20);
            this.tbDen.TabIndex = 5;
            this.tbDen.Text = "đến phòng";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(257, 270);
            this.Controls.Add(this.tbDen);
            this.Controls.Add(this.tbCoSoTT);
            this.Controls.Add(this.tbMoiBenhNhan);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbTenPhong);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tbStt);
            this.Controls.Add(this.tbTenBn);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.Text = "SpeechTest";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox tbTenBn;
        private System.Windows.Forms.TextBox tbStt;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbTenPhong;
        private System.Windows.Forms.TextBox tbMoiBenhNhan;
        private System.Windows.Forms.TextBox tbCoSoTT;
        private System.Windows.Forms.TextBox tbDen;
    }
}

