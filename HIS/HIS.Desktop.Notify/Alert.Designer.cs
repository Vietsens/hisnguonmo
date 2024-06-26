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
namespace HIS.Desktop.Notify
{
    partial class Alert
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Alert));
            this.lblNotify = new System.Windows.Forms.Label();
            this.pictureBoxInfo = new System.Windows.Forms.PictureBox();
            this.pictureBoxClose = new System.Windows.Forms.PictureBox();
            this.timeOut = new System.Windows.Forms.Timer();
            this.Show = new System.Windows.Forms.Timer();
            this.Close = new System.Windows.Forms.Timer();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxInfo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxClose)).BeginInit();
            this.SuspendLayout();
            // 
            // lblNotify
            // 
            this.lblNotify.Font = new System.Drawing.Font("Times New Roman", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.lblNotify.ForeColor = System.Drawing.Color.White;
            this.lblNotify.Location = new System.Drawing.Point(96, 0);
            this.lblNotify.Name = "lblNotify";
            this.lblNotify.Size = new System.Drawing.Size(319, 159);
            this.lblNotify.TabIndex = 0;
            this.lblNotify.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pictureBoxInfo
            // 
            this.pictureBoxInfo.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxInfo.Image")));
            this.pictureBoxInfo.Location = new System.Drawing.Point(12, 46);
            this.pictureBoxInfo.Name = "pictureBoxInfo";
            this.pictureBoxInfo.Size = new System.Drawing.Size(78, 66);
            this.pictureBoxInfo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxInfo.TabIndex = 1;
            this.pictureBoxInfo.TabStop = false;
            // 
            // pictureBoxClose
            // 
            this.pictureBoxClose.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxClose.Image")));
            this.pictureBoxClose.Location = new System.Drawing.Point(421, 0);
            this.pictureBoxClose.Name = "pictureBoxClose";
            this.pictureBoxClose.Size = new System.Drawing.Size(32, 32);
            this.pictureBoxClose.TabIndex = 2;
            this.pictureBoxClose.TabStop = false;
            this.pictureBoxClose.Click += new System.EventHandler(this.pictureBoxClose_Click);
            // 
            // timeOut
            // 
            this.timeOut.Interval = 1000;
            this.timeOut.Tick += new System.EventHandler(this.timeOut_Tick);
            // 
            // Show
            // 
            this.Show.Interval = 10;
            this.Show.Tick += new System.EventHandler(this.Show_Tick);
            // 
            // Close
            // 
            this.Close.Tick += new System.EventHandler(this.Close_Tick);
            // 
            // Alert
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.SeaGreen;
            this.ClientSize = new System.Drawing.Size(454, 159);
            this.Controls.Add(this.pictureBoxClose);
            this.Controls.Add(this.pictureBoxInfo);
            this.Controls.Add(this.lblNotify);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Alert";
            this.Text = "Alert";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.Alert_Load);
            this.MouseHover += new System.EventHandler(this.Alert_MouseHover);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxInfo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxClose)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblNotify;
        private System.Windows.Forms.PictureBox pictureBoxInfo;
        private System.Windows.Forms.PictureBox pictureBoxClose;
        private System.Windows.Forms.Timer timeOut;
        private System.Windows.Forms.Timer Show;
        private System.Windows.Forms.Timer Close;

    }
}
