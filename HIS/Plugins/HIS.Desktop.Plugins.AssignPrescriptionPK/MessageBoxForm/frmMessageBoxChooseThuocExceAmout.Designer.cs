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
namespace HIS.Desktop.Plugins.AssignPrescriptionPK.MessageBoxForm
{
    partial class frmMessageBoxChooseThuocExceAmout
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMessageBoxChooseThuocExceAmout));
            this.lblDescription = new DevExpress.XtraEditors.LabelControl();
            this.btnChonThuocCungHoatChat = new DevExpress.XtraEditors.SimpleButton();
            this.btnGiuNguyen = new DevExpress.XtraEditors.SimpleButton();
            this.btnSuaSoluong = new DevExpress.XtraEditors.SimpleButton();
            this.SuspendLayout();
            // 
            // lblDescription
            // 
            this.lblDescription.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.lblDescription.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblDescription.Location = new System.Drawing.Point(5, 4);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(331, 50);
            this.lblDescription.TabIndex = 0;
            this.lblDescription.Text = "Thuốc trong nhà thuốc không đủ khả dụng để kê. Bạn có muốn sử dụng thuốc thay thế" +
    "?";
            // 
            // btnChonThuocCungHoatChat
            // 
            this.btnChonThuocCungHoatChat.Location = new System.Drawing.Point(2, 60);
            this.btnChonThuocCungHoatChat.Name = "btnChonThuocCungHoatChat";
            this.btnChonThuocCungHoatChat.Size = new System.Drawing.Size(125, 23);
            this.btnChonThuocCungHoatChat.TabIndex = 1;
            this.btnChonThuocCungHoatChat.Text = "Thuốc cùng hoạt chất";
            this.btnChonThuocCungHoatChat.Click += new System.EventHandler(this.btnChonThuocCungHoatChat_Click);
            // 
            // btnGiuNguyen
            // 
            this.btnGiuNguyen.Location = new System.Drawing.Point(133, 60);
            this.btnGiuNguyen.Name = "btnGiuNguyen";
            this.btnGiuNguyen.Size = new System.Drawing.Size(90, 23);
            this.btnGiuNguyen.TabIndex = 2;
            this.btnGiuNguyen.Text = "Giữ nguyên";
            this.btnGiuNguyen.Click += new System.EventHandler(this.btnGiuNguyen_Click);
            // 
            // btnSuaSoluong
            // 
            this.btnSuaSoluong.Location = new System.Drawing.Point(229, 60);
            this.btnSuaSoluong.Name = "btnSuaSoluong";
            this.btnSuaSoluong.Size = new System.Drawing.Size(97, 23);
            this.btnSuaSoluong.TabIndex = 3;
            this.btnSuaSoluong.Text = "Sửa số lượng";
            this.btnSuaSoluong.Click += new System.EventHandler(this.btnSuaSoluong_Click);
            // 
            // frmMessageBoxChooseThuocExceAmout
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(338, 88);
            this.Controls.Add(this.btnSuaSoluong);
            this.Controls.Add(this.btnGiuNguyen);
            this.Controls.Add(this.btnChonThuocCungHoatChat);
            this.Controls.Add(this.lblDescription);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmMessageBoxChooseThuocExceAmout";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Thông báo";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmMessageBoxChooseThuocExceAmout_FormClosed);
            this.Load += new System.EventHandler(this.frmMessageBoxChooseThuocExceAmout_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.LabelControl lblDescription;
        private DevExpress.XtraEditors.SimpleButton btnChonThuocCungHoatChat;
        private DevExpress.XtraEditors.SimpleButton btnGiuNguyen;
        private DevExpress.XtraEditors.SimpleButton btnSuaSoluong;
    }
}
