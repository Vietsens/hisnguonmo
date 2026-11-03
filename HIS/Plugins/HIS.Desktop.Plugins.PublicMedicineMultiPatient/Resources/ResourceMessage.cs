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

namespace HIS.Desktop.Plugins.PublicMedicineMultiPatient.Resources
{
    class ResourceMessage
    {
        static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.Desktop.Plugins.PublicMedicineMultiPatient.Resources.Message.Lang", System.Reflection.Assembly.GetExecutingAssembly());

        private static string Get(string key)
        {
            try
            {
                return Inventec.Common.Resource.Get.Value(key, languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return "";
        }

        /// <summary>Chưa chọn bệnh nhân</summary>
        internal static string ChuaChonBenhNhan { get { return Get("ChuaChonBenhNhan"); } }

        /// <summary>Chưa chọn loại cần công khai: Thuốc, Vật tư, Máu, Hóa chất</summary>
        internal static string ChuaChonLoaiCongKhai { get { return Get("ChuaChonLoaiCongKhai"); } }

        /// <summary>Chưa nhập thời gian Tạo từ, Tạo đến</summary>
        internal static string ChuaNhapThoiGian { get { return Get("ChuaNhapThoiGian"); } }

        /// <summary>Tạo từ không được lớn hơn Tạo đến</summary>
        internal static string TaoTuLonHonTaoDen { get { return Get("TaoTuLonHonTaoDen"); } }

        /// <summary>Không có thuốc, vật tư cần công khai của các bệnh nhân đã chọn trong khoảng thời gian</summary>
        internal static string KhongCoDuLieuCongKhai { get { return Get("KhongCoDuLieuCongKhai"); } }

        /// <summary>Các bệnh nhân không có thuốc, vật tư trong khoảng thời gian: {0}</summary>
        internal static string BenhNhanKhongCoDuLieu { get { return Get("BenhNhanKhongCoDuLieu"); } }

        /// <summary>Không tải được dữ liệu công khai thuốc. Vui lòng thử lại</summary>
        internal static string KhongTaiDuocDuLieu { get { return Get("KhongTaiDuocDuLieu"); } }

        /// <summary>Đã ký {0}/{1} phiếu công khai thuốc</summary>
        internal static string KetQuaKySo { get { return Get("KetQuaKySo"); } }

        /// <summary>Bệnh nhân ký lỗi: {0}</summary>
        internal static string BenhNhanKyLoi { get { return Get("BenhNhanKyLoi"); } }
    }
}
