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
namespace HIS.Desktop.Plugins.HisCacheMonitor.HisCacheMonitor
{
    partial class frmRefeshOne
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
            this.rdReloadNewOrEdit = new DevExpress.XtraEditors.CheckEdit();
            this.rdReloadAllForHasDelete = new DevExpress.XtraEditors.CheckEdit();
            this.btnOk = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.rdReloadNewOrEdit.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rdReloadAllForHasDelete.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // rdReloadNewOrEdit
            // 
            this.rdReloadNewOrEdit.EditValue = true;
            this.rdReloadNewOrEdit.Location = new System.Drawing.Point(66, 12);
            this.rdReloadNewOrEdit.Name = "rdReloadNewOrEdit";
            this.rdReloadNewOrEdit.Properties.Caption = "Chỉ tải lại dữ liệu thêm mới hoặc chỉnh sửa";
            this.rdReloadNewOrEdit.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.rdReloadNewOrEdit.Properties.RadioGroupIndex = 1;
            this.rdReloadNewOrEdit.Size = new System.Drawing.Size(287, 19);
            this.rdReloadNewOrEdit.TabIndex = 0;
            // 
            // rdReloadAllForHasDelete
            // 
            this.rdReloadAllForHasDelete.Location = new System.Drawing.Point(66, 37);
            this.rdReloadAllForHasDelete.Name = "rdReloadAllForHasDelete";
            this.rdReloadAllForHasDelete.Properties.Caption = "Tải lại toàn bộ (trong trường hợp có xóa dữ liệu)";
            this.rdReloadAllForHasDelete.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.rdReloadAllForHasDelete.Properties.RadioGroupIndex = 1;
            this.rdReloadAllForHasDelete.Size = new System.Drawing.Size(287, 19);
            this.rdReloadAllForHasDelete.TabIndex = 0;
            this.rdReloadAllForHasDelete.TabStop = false;
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(148, 88);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 1;
            this.btnOk.Text = "Đồng ý";
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // frmRefeshOne
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(379, 123);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.rdReloadAllForHasDelete);
            this.Controls.Add(this.rdReloadNewOrEdit);
            this.Name = "frmRefeshOne";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Tải lại";
            this.Load += new System.EventHandler(this.frmRefeshOne_Load);
            ((System.ComponentModel.ISupportInitialize)(this.rdReloadNewOrEdit.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rdReloadAllForHasDelete.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.CheckEdit rdReloadNewOrEdit;
        private DevExpress.XtraEditors.CheckEdit rdReloadAllForHasDelete;
        private DevExpress.XtraEditors.SimpleButton btnOk;
    }
}
