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
using DevExpress.XtraEditors.Controls;
using Inventec.Common.Logging;
using System;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.SupportRequestManagement
{
    /// <summary>
    /// vCong57682 - Hop thoai Chuyen viec trong noi bo nhom quan tri.
    /// Ly do la BAT BUOC de nguoi nhan biet viec duoc day sang minh vi sao
    /// va da lam toi dau.
    /// </summary>
    public partial class frmTransfer : Form
    {
        readonly string requestCode;
        readonly string currentHolderLoginname;
        readonly bool allowEmptyTarget;

        /// <summary>Tai khoan nguoi nhan viec. Rong = tra viec ve danh sach chung.</summary>
        public string TargetLoginname { get; private set; }
        public string Reason { get; private set; }

        public frmTransfer(string requestCode, string currentHolderLoginname, bool allowEmptyTarget)
        {
            InitializeComponent();
            this.requestCode = requestCode;
            this.currentHolderLoginname = currentHolderLoginname;
            this.allowEmptyTarget = allowEmptyTarget;
        }

        private void frmTransfer_Load(object sender, EventArgs e)
        {
            try
            {
                lblCode.Text = requestCode;

                var admins = UCSupportRequestManagement.GetAdminEmployees();
                var holder = admins.FirstOrDefault(o => o.CODE == currentHolderLoginname);
                lblHolder.Text = holder != null ? holder.NAME : "(chưa có ai giữ việc)";

                // Loai nguoi dang giu viec ra khoi danh sach chon cho khoi chuyen vao chinh minh.
                var target = admins.Where(o => o.CODE != currentHolderLoginname).ToList();
                UCSupportRequestManagement.InitEmployeeLookUp(cboTarget, target, "Người nhận việc");

                cboTarget.Properties.NullText = allowEmptyTarget
                    ? "(để trống — trả việc về danh sách chung)"
                    : "";

                memReason.Focus();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void cboTarget_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                    UCSupportRequestManagement.ClearLookUp(cboTarget);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(memReason.Text))
                {
                    XtraMessageBox.Show("Vui lòng nhập lý do chuyển việc", "Thông báo");
                    memReason.Focus();
                    return;
                }

                // Viec dang xu ly thi bat buoc phai co nguoi nhan, khong duoc de trong.
                if (!allowEmptyTarget && cboTarget.EditValue == null)
                {
                    XtraMessageBox.Show("Vui lòng chọn người nhận việc", "Thông báo");
                    cboTarget.Focus();
                    return;
                }

                this.TargetLoginname = cboTarget.EditValue != null ? cboTarget.EditValue.ToString() : null;
                this.Reason = memReason.Text.Trim();
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
