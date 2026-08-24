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
 */

namespace HIS.Desktop.Plugins.EnterKskInfomantionVer2.ADO
{
    /// <summary>
    /// 1 dòng của lưới thiết lập mặc định khám lâm sàng (tab "Mặc định khám lâm sàng" trong form Thiết lập).
    /// Người dùng tự thêm/bớt dòng bằng cột nút +/- nên KHÔNG seed sẵn 49 ô.
    /// Cột "Mục" → "Nội dung" → "Giá trị mặc định" phụ thuộc nhau theo thứ tự đó.
    /// </summary>
    public class KskDefaultRowADO
    {
        /// <summary>
        /// Cột 1 "Dùng" — chỉ dòng được tích mới đem áp vào form. Dòng KHÔNG tích vẫn được LƯU
        /// nguyên giá trị đã thiết lập (bỏ tích = tạm tắt, không phải xóa cấu hình).
        /// Dòng mới thêm bằng nút + mặc định là đang dùng.
        /// </summary>
        public bool IS_USED { get; set; }

        /// <summary>Cột 2 "Mục" — khóa nhóm layout đã chọn. VD "lcgMat8".</summary>
        public string GROUP_NAME { get; set; }

        /// <summary>Cột 3 "Nội dung" — tên RadioGroup đã chọn, phụ thuộc GROUP_NAME. VD "rdoStrabismus8".</summary>
        public string FIELD_NAME { get; set; }

        /// <summary>Cột 4 "Giá trị mặc định" — khóa ghép "FIELD_NAME|VALUE", phụ thuộc FIELD_NAME.</summary>
        public string VALUE_KEY { get; set; }

        /// <summary>
        /// Cột 5 — chỗ đặt nút +/- (dòng cuối là +, các dòng trên là -). Không mang dữ liệu nghiệp vụ;
        /// tồn tại để cột có FieldName hợp lệ, tránh phải xử lý cột unbound.
        /// </summary>
        public string ACTION { get; set; }
    }
}
