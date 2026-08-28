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
using System.Collections.Generic;

namespace HIS.Desktop.Plugins.EnterKskInfomantionVer2.ADO
{
    /// <summary>
    /// Toàn bộ nội dung 1 file xuất/nhập thiết lập của form ⚙ Thiết lập — gộp CẢ 2 TAB để cóp
    /// sang máy khác trong 1 lần. Gom vào 1 object thay vì nhiều tham số out cho dễ đọc.
    /// Danh sách nào null/rỗng thì phần đó không có trong file và khi nhập sẽ được BỎ QUA
    /// (không xóa thiết lập đang có của phần đó).
    /// </summary>
    public class KskSettingFileADO
    {
        public KskSettingFileADO()
        {
            this.ROWS = new List<KskDefaultRowADO>();
            this.AUTO_CLS_BLOOD = new List<KskServiceRefADO>();
            this.AUTO_CLS_URINE = new List<KskServiceRefADO>();
            this.AUTO_CLS_DIIM = new List<KskServiceRefADO>();
        }

        // ===== Tab "Mặc định nhập KSK (trẻ dưới 6 tuổi)" =====

        /// <summary>Cờ "Tự động điền mặc định khi mở bản ghi mới".</summary>
        public bool AUTO_APPLY { get; set; }

        /// <summary>Các dòng thiết lập mặc định (kèm cờ IS_USED của cột "Dùng").</summary>
        public List<KskDefaultRowADO> ROWS { get; set; }

        // ===== Tab "Tự động lấy CLS" =====

        /// <summary>Dịch vụ xét nghiệm cho ô "Máu".</summary>
        public List<KskServiceRefADO> AUTO_CLS_BLOOD { get; set; }

        /// <summary>Dịch vụ xét nghiệm cho ô "Nước tiểu".</summary>
        public List<KskServiceRefADO> AUTO_CLS_URINE { get; set; }

        /// <summary>Dịch vụ chẩn đoán hình ảnh cho ô "Chẩn đoán hình ảnh".</summary>
        public List<KskServiceRefADO> AUTO_CLS_DIIM { get; set; }

        /// <summary>File có phần "Tự động lấy CLS" hay không (dùng để báo và để bỏ qua khi nhập).</summary>
        public bool HasAutoCls
        {
            get
            {
                return (AUTO_CLS_BLOOD != null && AUTO_CLS_BLOOD.Count > 0)
                    || (AUTO_CLS_URINE != null && AUTO_CLS_URINE.Count > 0)
                    || (AUTO_CLS_DIIM != null && AUTO_CLS_DIIM.Count > 0);
            }
        }
    }
}
