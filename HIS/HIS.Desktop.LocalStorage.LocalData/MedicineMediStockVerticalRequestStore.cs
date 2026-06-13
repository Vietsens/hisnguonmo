/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
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

namespace HIS.Desktop.LocalStorage.LocalData
{
    /// <summary>
    /// Yêu cầu "Xem tồn kho theo kho" truyền từ danh mục Thuốc/Vật tư sang plugin
    /// MedicineMediStockSummaryVertical.
    /// </summary>
    public class MedicineMediStockVerticalRequest
    {
        /// <summary>true = thuốc, false = vật tư.</summary>
        public bool IsMedicine { get; set; }

        /// <summary>ID loại (HIS_MEDICINE_TYPE.ID khi thuốc / HIS_MATERIAL_TYPE.ID khi vật tư).</summary>
        public long TypeId { get; set; }
    }

    /// <summary>
    /// Kênh truyền yêu cầu giữa các plugin: danh mục Thuốc/Vật tư phát yêu cầu,
    /// tab UC MedicineMediStockSummaryVertical đang mở lắng nghe để tự chọn loại + tìm lại
    /// (giải quyết trường hợp ShowModule chỉ kích hoạt tab cũ, không chạy lại Processor).
    /// </summary>
    public static class MedicineMediStockVerticalRequestStore
    {
        public static event Action<MedicineMediStockVerticalRequest> RequestRaised;

        public static void Raise(MedicineMediStockVerticalRequest request)
        {
            try
            {
                Action<MedicineMediStockVerticalRequest> handler = RequestRaised;
                if (handler != null)
                {
                    handler(request);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
