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
namespace HIS.Desktop.Plugins.AssignPrescriptionPK
{
    partial class frmChooseIntructionTime
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmChooseIntructionTime));
            this.btnAdd = new DevExpress.XtraEditors.SimpleButton();
            this.pnlMultiDate = new System.Windows.Forms.Panel();
            this.btnChoose = new DevExpress.XtraEditors.SimpleButton();
            this.SuspendLayout();
            // 
            // btnAdd
            // 
            this.btnAdd.Image = ((System.Drawing.Image)(resources.GetObject("btnAdd.Image")));
            this.btnAdd.Location = new System.Drawing.Point(333, 12);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(23, 23);
            this.btnAdd.TabIndex = 0;
            this.btnAdd.Text = " ";
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // pnlMultiDate
            // 
            this.pnlMultiDate.AutoScroll = true;
            this.pnlMultiDate.Location = new System.Drawing.Point(12, 12);
            this.pnlMultiDate.Name = "pnlMultiDate";
            this.pnlMultiDate.Size = new System.Drawing.Size(315, 116);
            this.pnlMultiDate.TabIndex = 1;
            // 
            // btnChoose
            // 
            this.btnChoose.Location = new System.Drawing.Point(293, 134);
            this.btnChoose.Name = "btnChoose";
            this.btnChoose.Size = new System.Drawing.Size(63, 23);
            this.btnChoose.TabIndex = 2;
            this.btnChoose.Text = "Chọn";
            this.btnChoose.Click += new System.EventHandler(this.btnChoose_Click);
            // 
            // frmChooseIntructionTime
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(368, 162);
            this.Controls.Add(this.btnChoose);
            this.Controls.Add(this.pnlMultiDate);
            this.Controls.Add(this.btnAdd);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmChooseIntructionTime";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Chọn nhiều ngày y lệnh";
            this.Load += new System.EventHandler(this.frmChooseIntructionTime_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.SimpleButton btnAdd;
        private System.Windows.Forms.Panel pnlMultiDate;
        private DevExpress.XtraEditors.SimpleButton btnChoose;

    }
}
