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
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.EnterKskInfomantionQD831.Run
{
    public partial class frmEnterKskInfomantionQD831
    {
        /// <summary>
        /// Duyệt đệ quy toàn bộ control của form, đảm bảo MỌI GridLookUpEdit đều có nút Delete
        /// (ButtonPredefines.Delete) trên Properties.Buttons. Khi người dùng bấm nút này sẽ xóa
        /// (clear) giá trị đang chọn của ô đó. Gọi 1 lần lúc Load.
        /// </summary>
        private void InitClearButtonForGridLookUpEdits(Control parent)
        {
            if (parent == null) return;
            foreach (Control c in parent.Controls)
            {
                GridLookUpEdit gle = c as GridLookUpEdit;
                if (gle != null)
                {
                    EnsureDeleteButton(gle);
                }
                if (c.HasChildren)
                {
                    InitClearButtonForGridLookUpEdits(c);
                }
            }
        }

        /// <summary>
        /// Thêm nút Delete vào GridLookUpEdit (nếu chưa có) và gắn handler clear giá trị.
        /// Nút Delete CHỈ hiển thị khi ô đang có dữ liệu, ẩn khi ô rỗng.
        /// </summary>
        private void EnsureDeleteButton(GridLookUpEdit gle)
        {
            try
            {
                if (gle == null || gle.Properties == null) return;
                // Combo multi-select (checkbox GridCheckMarksSelection) tự quản nút Xóa riêng —
                // KHÔNG gắn handler EditValueChanged toggle nút ở đây (sẽ re-layout editor -> đóng popup khi tick).
                if (gle.Properties.Tag is HIS.Desktop.Utilities.Extensions.GridCheckMarksSelection) return;

                // BẮT BUỘC cho phép null thì set EditValue=null mới "dính" (nếu không DevExpress khôi phục
                // lại giá trị cũ khi validate -> bấm Delete như không xóa được).
                gle.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;

                EditorButton deleteButton = null;
                foreach (EditorButton btn in gle.Properties.Buttons)
                {
                    if (btn.Kind == ButtonPredefines.Delete)
                    {
                        deleteButton = btn;
                        break;
                    }
                }

                if (deleteButton == null)
                {
                    deleteButton = new EditorButton(ButtonPredefines.Delete);
                    deleteButton.ToolTip = "Xóa giá trị đang chọn";
                    gle.Properties.Buttons.Add(deleteButton);
                }

                // Tránh gắn handler trùng lặp (phòng khi method được gọi nhiều lần).
                gle.ButtonClick -= GridLookUpEdit_ClearButtonClick;
                gle.ButtonClick += GridLookUpEdit_ClearButtonClick;
                gle.EditValueChanged -= GridLookUpEdit_EditValueChanged;
                gle.EditValueChanged += GridLookUpEdit_EditValueChanged;

                // Trạng thái hiển thị ban đầu theo giá trị hiện có.
                UpdateDeleteButtonVisibility(gle);
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Khi bấm nút Delete trên GridLookUpEdit → clear giá trị của ô.
        /// </summary>
        private void GridLookUpEdit_ClearButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e == null || e.Button == null || e.Button.Kind != ButtonPredefines.Delete) return;
                GridLookUpEdit gle = sender as GridLookUpEdit;
                if (gle == null) return;
                // AllowNullInput=true đã set ở EnsureDeleteButton nên gán null là "dính". Clear thêm Text cho chắc.
                gle.Text = null;
                gle.EditValue = null;
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Mỗi khi giá trị ô thay đổi → cập nhật lại trạng thái ẩn/hiện nút Delete.
        /// </summary>
        private void GridLookUpEdit_EditValueChanged(object sender, System.EventArgs e)
        {
            UpdateDeleteButtonVisibility(sender as GridLookUpEdit);
        }

        /// <summary>
        /// Nút Delete chỉ hiển thị khi ô có dữ liệu (EditValue khác null/DBNull/rỗng).
        /// </summary>
        private void UpdateDeleteButtonVisibility(GridLookUpEdit gle)
        {
            try
            {
                if (gle == null || gle.Properties == null) return;

                object value = gle.EditValue;
                bool hasValue = value != null
                    && value != System.DBNull.Value
                    && !string.IsNullOrEmpty(value.ToString());

                foreach (EditorButton btn in gle.Properties.Buttons)
                {
                    if (btn.Kind == ButtonPredefines.Delete)
                    {
                        btn.Visible = hasValue;
                        break;
                    }
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
