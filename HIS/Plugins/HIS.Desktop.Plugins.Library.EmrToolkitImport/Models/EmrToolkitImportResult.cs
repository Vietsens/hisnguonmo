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
namespace HIS.Desktop.Plugins.Library.EmrToolkitImport.Models
{
    /// <summary>
    /// Bước xử lý trong luồng import EMRTOOLKIT — phục vụ hiển thị/log khi lỗi.
    /// </summary>
    public enum EmrToolkitImportStep
    {
        /// <summary>Chưa bắt đầu</summary>
        None = 0,

        /// <summary>Lấy token (CreateToken)</summary>
        CreateToken = 1,

        /// <summary>Mã hóa JSON (MaHoaJson)</summary>
        MaHoaJson = 2,

        /// <summary>Import dữ liệu (Import)</summary>
        Import = 3,

        /// <summary>Hoàn thành</summary>
        Completed = 4
    }

    /// <summary>
    /// Kết quả tổng hợp của 1 lần gọi import EMRTOOLKIT.
    /// Trả về cho plugin gọi để xử lý/hiển thị.
    /// </summary>
    public class EmrToolkitImportResult
    {
        /// <summary>Toàn bộ luồng thành công hay không</summary>
        public bool Success { get; set; }

        /// <summary>Thông báo lỗi (rỗng nếu thành công)</summary>
        public string Message { get; set; }

        /// <summary>Bước dừng lại khi lỗi / bước cuối khi thành công</summary>
        public EmrToolkitImportStep Step { get; set; }

        /// <summary>JSON gốc đã gửi đi (model Giấy Chuyển Viện)</summary>
        public string RawRequestJson { get; set; }

        /// <summary>JSON thô API Import trả về</summary>
        public string RawResponseJson { get; set; }

        /// <summary>Dữ liệu (đã giải mã) API Import trả về — chính là dữ liệu đã gửi</summary>
        public object ImportData { get; set; }
    }
}
