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
using Inventec.Desktop.CustomControl.NoFocus;

namespace HIS.UC.WorkPlace
{
    /// <summary>
    /// Combo nơi làm việc: lọc LIKE trên mọi cột chuỗi (mã + tên) như CustomGridLookUpEditWithFilterMultiColumnNoFocus
    /// nhưng TẮT auto-complete. Control gốc ép IsAutoComplete = true nên khi text gõ vào trùng đầu tên một đơn vị
    /// (ví dụ gõ "Công ty " khớp "Công ty ABCD") DevExpress gán luôn EditValue = đơn vị đó và thay text đang gõ.
    /// Tắt auto-complete thì text giữ nguyên, danh sách chỉ lọc; chọn bằng Enter/Tab/click (UCWorkPlaceCombo1 tự focus dòng đầu khi gõ).
    /// </summary>
    public class WorkPlaceGridLookUpEdit : CustomGridLookUpEditWithFilterMultiColumnNoFocus
    {
        public WorkPlaceGridLookUpEdit() : base() { }

        protected override bool IsAutoComplete
        {
            get { return false; }
        }
    }
}
