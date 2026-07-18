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
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using MOS.EFMODEL.DataModels;
using Inventec.Common.Logging;

namespace HIS.Desktop.Plugins.EnterKskInfomantionVer2.Run
{
    /// <summary>
    /// UserControl TÁI SỬ DỤNG: cụm chọn mã ICD cho 1 NHÓM TIỀN SỬ.
    /// Gồm: ô mã (chỉ đọc, ghép dấu ;) + ô tên (chỉ đọc, ghép dấu ;) + nút "..." mở popup
    /// "Tìm chọn bệnh" (<see cref="HIS.UC.SecondaryIcd.frmSecondaryIcd"/>) cho phép CHỌN NHIỀU ICD.
    /// Đặt cạnh ô văn bản tự do tiền sử hiện có ở mỗi tab. Sau khi add vào host, GỌI <see cref="InitUc"/>.
    /// UC chỉ là editor "câm" — form host lắng nghe <see cref="IcdChanged"/> để đồng bộ 1 giá trị/nhóm
    /// giữa các tab (R5) và nạp/lưu khi BE bổ sung cột HIS_KSK_GENERAL.
    /// </summary>
    public partial class UcKskHistoryIcd : DevExpress.XtraEditors.XtraUserControl
    {
        // Danh sách ICD để truyền vào popup (lấy từ host, tránh mỗi UC tự query cache).
        private List<HIS_ICD> icdList;
        private int pageSize = 50;
        private bool isInited = false;

        /// <summary>
        /// Khóa nhóm tiền sử mà UC này đại diện (Gia đình / Bản thân / ...). Host gán khi nhúng,
        /// dùng để đồng bộ giá trị giữa nhiều instance cùng nhóm trên các tab khác nhau.
        /// </summary>
        public KskHistoryGroup Group { get; set; }

        /// <summary>Fired sau khi user chọn ICD từ popup. Tham số: (codes ghép ;, names ghép ;).</summary>
        public event Action<KskHistoryGroup, string, string> IcdChanged;

        public UcKskHistoryIcd()
        {
            InitializeComponent();
        }

        /// <summary>Khởi tạo dữ liệu ICD + pageSize cho popup. Gọi 1 lần sau khi add vào host.</summary>
        public void InitUc(List<HIS_ICD> icdData, int popupPageSize)
        {
            if (isInited) return;
            try
            {
                this.icdList = icdData ?? new List<HIS_ICD>();
                if (popupPageSize > 0) this.pageSize = popupPageSize;
                isInited = true;
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        #region ===== Public API =====

        /// <summary>Mã ICD đã chọn (ghép dấu ;). Rỗng nếu chưa chọn.</summary>
        public string GetCodes()
        {
            return this.txtIcdCode.Text ?? "";
        }

        /// <summary>Tên ICD đã chọn (ghép dấu ;). Rỗng nếu chưa chọn.</summary>
        public string GetNames()
        {
            return this.txtIcdName.Text ?? "";
        }

        /// <summary>True nếu đã chọn ít nhất 1 mã ICD.</summary>
        public bool HasIcd
        {
            get { return !string.IsNullOrWhiteSpace(this.txtIcdCode.Text); }
        }

        /// <summary>Đổ giá trị vào 2 ô (không phát sự kiện IcdChanged — dùng khi load/đồng bộ).</summary>
        public void SetData(string codes, string names)
        {
            try
            {
                this.txtIcdCode.Text = codes ?? "";
                this.txtIcdName.Text = names ?? "";
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>Bật/tắt khả năng chỉnh sửa (nút "..."). 2 ô luôn ReadOnly.</summary>
        public void SetReadOnly(bool readOnly)
        {
            try
            {
                this.btnChooseIcd.Enabled = !readOnly;
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        #endregion

        #region ===== Internal =====

        private void btnChooseIcd_Click(object sender, EventArgs e)
        {
            try
            {
                string subCode = this.txtIcdCode.Text ?? "";
                string text = this.txtIcdName.Text ?? "";
                frmSubIcd frm = new frmSubIcd(
                 new HIS.Desktop.Plugins.EnterKskInfomantionVer2.Run.DelegateRefeshIcdChandoanphu(DlgChooseIcd),
                 subCode, text, pageSize, new System.Collections.Generic.List<HIS_ICD>());
                frm.ShowDialog();
            }
            catch (Exception ex) { LogSystem.Error(ex); }
        }

        /// <summary>Callback của popup: nhận mã + tên ICD đã chọn (ghép ;), cập nhật UI + phát sự kiện.</summary>
        private void DlgChooseIcd(string icdCodes, string icdNames)
        {
            try
            {
                this.txtIcdCode.Text = icdCodes ?? "";
                this.txtIcdName.Text = icdNames ?? "";
                if (this.IcdChanged != null) this.IcdChanged(this.Group, GetCodes(), GetNames());
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        #endregion
    }
}
