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
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.SupportRequestManagement
{
    /// <summary>
    /// vCong57682 - Hop thoai chuyen yeu cau len cong ty.
    /// Cho bo phan quan tri sua lai tieu de va noi dung truoc khi gui,
    /// de bo sung thong tin da ra soat thay vi day nguyen van cua nguoi dung.
    /// </summary>
    public partial class frmForward : Form
    {
        readonly string requestCode;
        readonly int fileCount;

        public string ForwardTitle { get; private set; }
        public string ForwardContent { get; private set; }
        public string Reason { get; private set; }

        public frmForward(string requestCode, string title, string content, int fileCount)
        {
            InitializeComponent();
            this.requestCode = requestCode;
            this.ForwardTitle = title;
            this.ForwardContent = content;
            this.fileCount = fileCount;
        }

        private void frmForward_Load(object sender, EventArgs e)
        {
            try
            {
                lblCode.Text = requestCode;
                lblFile.Text = fileCount > 0
                    ? string.Format("{0} tệp — sẽ được tải lên kho tệp của công ty", fileCount)
                    : "Không có";
                txtTitle.Text = ForwardTitle;
                memContent.Text = ForwardContent;
                memReason.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtTitle.Text))
                {
                    XtraMessageBox.Show("Tiêu đề không được để trống", "Thông báo");
                    txtTitle.Focus();
                    return;
                }
                if (string.IsNullOrWhiteSpace(memContent.Text))
                {
                    XtraMessageBox.Show("Nội dung không được để trống", "Thông báo");
                    memContent.Focus();
                    return;
                }

                if (XtraMessageBox.Show(
                        "Yêu cầu sẽ được gửi sang hệ thống của công ty và không thu hồi được. Bạn có chắc không?",
                        "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                    return;

                this.ForwardTitle = txtTitle.Text.Trim();
                this.ForwardContent = memContent.Text;
                this.Reason = memReason.Text;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
