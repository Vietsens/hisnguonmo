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
    /// 1 dịch vụ trong file xuất/nhập thiết lập (tab "Tự động lấy CLS").
    /// Ghi cả ID và CODE: nhập ưu tiên khớp ID, không thấy thì khớp CODE — máy khác cùng CSDL thì
    /// ID trùng, còn copy sang môi trường khác (ID lệch) vẫn nhận được theo mã dịch vụ.
    /// </summary>
    public class KskServiceRefADO
    {
        public long ID { get; set; }

        /// <summary>Mã dịch vụ (V_HIS_SERVICE.SERVICE_CODE) — dùng để khớp dự phòng khi ID lệch.</summary>
        public string CODE { get; set; }

        /// <summary>Tên dịch vụ — chỉ để người đọc file hiểu, KHÔNG dùng để khớp.</summary>
        public string NAME { get; set; }
    }
}
