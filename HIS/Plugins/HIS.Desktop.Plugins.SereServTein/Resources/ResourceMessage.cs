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

namespace HIS.Desktop.Plugins.SereServTein.Resources
{
    /// <summary>
    /// Thông báo riêng của plugin. Thông báo dùng chung toàn hệ thống lấy qua MessageUtil.
    /// </summary>
    class ResourceMessage
    {
        static System.Resources.ResourceManager languageMessage =
            new System.Resources.ResourceManager(
                "HIS.Desktop.Plugins.SereServTein.Resources.Message.Lang",
                System.Reflection.Assembly.GetExecutingAssembly());

        private static string GetValue(string key)
        {
            try
            {
                return Inventec.Common.Resource.Get.Value(
                    key,
                    languageMessage,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return "";
        }

        /// <summary>Bệnh nhân chưa có số định danh (CCCD/CMND).</summary>
        internal static string BenhNhanChuaCoSoDinhDanh { get { return GetValue("BenhNhanChuaCoSoDinhDanh"); } }

        /// <summary>Bệnh nhân không còn kết quả CLS hiệu lực trên cổng.</summary>
        internal static string KhongCoKetQuaConHieuLuc { get { return GetValue("KhongCoKetQuaConHieuLuc"); } }

        /// <summary>Vui lòng chọn bản ghi kết quả cần xem.</summary>
        internal static string ChuaChonBanGhiDeXem { get { return GetValue("ChuaChonBanGhiDeXem"); } }

        /// <summary>Cổng giới hạn xin mã OTP tối thiểu 5 giây một lần.</summary>
        internal static string XinMaOtpQuaNhanh { get { return GetValue("XinMaOtpQuaNhanh"); } }

        /// <summary>Đã tải {0} phiếu kết quả, các phiếu còn lại nằm tại {1}.</summary>
        internal static string DaTaiNhieuPhieuKetQua { get { return GetValue("DaTaiNhieuPhieuKetQua"); } }
    }
}
