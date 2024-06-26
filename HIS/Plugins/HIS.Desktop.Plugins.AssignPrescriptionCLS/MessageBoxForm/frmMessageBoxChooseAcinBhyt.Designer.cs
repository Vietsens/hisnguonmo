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
namespace HIS.Desktop.Plugins.AssignPrescriptionCLS.MessageBoxForm
{
    partial class frmMessageBoxChooseAcinBhyt
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMessageBoxChooseAcinBhyt));
            this.lblDescription = new DevExpress.XtraEditors.LabelControl();
            this.btnChonThuocCungHoatChat = new DevExpress.XtraEditors.SimpleButton();
            this.btnChonThuocNgoaiKho = new DevExpress.XtraEditors.SimpleButton();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            this.SuspendLayout();
            // 
            // lblDescription
            // 
            this.lblDescription.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.lblDescription.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblDescription.Location = new System.Drawing.Point(4, 6);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(331, 48);
            this.lblDescription.TabIndex = 0;
            this.lblDescription.Text = "Thuốc trong kho không đủ để kê. Bạn muốn sử dụng thuốc thay thế:";
            // 
            // btnChonThuocCungHoatChat
            // 
            this.btnChonThuocCungHoatChat.Location = new System.Drawing.Point(21, 60);
            this.btnChonThuocCungHoatChat.Name = "btnChonThuocCungHoatChat";
            this.btnChonThuocCungHoatChat.Size = new System.Drawing.Size(127, 23);
            this.btnChonThuocCungHoatChat.TabIndex = 1;
            this.btnChonThuocCungHoatChat.Text = "Thuốc cùng hoạt chất";
            this.btnChonThuocCungHoatChat.Click += new System.EventHandler(this.btnChonThuocCungHoatChat_Click);
            // 
            // btnChonThuocNgoaiKho
            // 
            this.btnChonThuocNgoaiKho.Location = new System.Drawing.Point(150, 60);
            this.btnChonThuocNgoaiKho.Name = "btnChonThuocNgoaiKho";
            this.btnChonThuocNgoaiKho.Size = new System.Drawing.Size(100, 23);
            this.btnChonThuocNgoaiKho.TabIndex = 2;
            this.btnChonThuocNgoaiKho.Text = "Thuốc ngoài kho";
            this.btnChonThuocNgoaiKho.Click += new System.EventHandler(this.btnChonThuocNgoaiKho_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(252, 60);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(68, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Bỏ qua";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // frmMessageBoxChooseAcinBhyt
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(338, 88);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnChonThuocNgoaiKho);
            this.Controls.Add(this.btnChonThuocCungHoatChat);
            this.Controls.Add(this.lblDescription);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmMessageBoxChooseAcinBhyt";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao);
            this.Load += new System.EventHandler(this.frmMessageBoxChooseMedicineTypeAcin_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.LabelControl lblDescription;
        private DevExpress.XtraEditors.SimpleButton btnChonThuocCungHoatChat;
        private DevExpress.XtraEditors.SimpleButton btnChonThuocNgoaiKho;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
    }
}
