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
namespace HIS.Desktop.Plugins.CoordinationServiceReqCLS
{
    /// <summary>
    /// Mức độ cảnh báo chỉ số xét nghiệm (trường WARNING trả về từ thủ tục PRO_GET_SERVICE_REQ_CLS).
    /// Dùng để tô màu dòng trong lưới danh sách bệnh nhân (mục 5.5 tài liệu).
    /// Không có trong IMSys.DbConfig nên khai báo Enum riêng.
    /// </summary>
    public enum EnumCoordinationWarning
    {
        /// <summary>Bình thường — tất cả chỉ số trong khoảng bình thường (tô màu trắng).</summary>
        Normal = 1,

        /// <summary>Bất thường — có chỉ số ngoài giá trị bình thường nhưng chưa vượt ngưỡng (tô màu vàng).</summary>
        Abnormal = 2,

        /// <summary>Vượt ngưỡng — có chỉ số vượt ngưỡng cảnh báo nguy hiểm (tô màu đỏ).</summary>
        OverThreshold = 3
    }
}
