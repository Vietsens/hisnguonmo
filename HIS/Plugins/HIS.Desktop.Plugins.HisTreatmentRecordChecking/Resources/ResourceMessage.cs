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

namespace HIS.Desktop.Plugins.HisTreatmentRecordChecking.Resources
{
    /// <summary>
    /// Plugin specific messages (Message.Lang.vi.resx / Message.Lang.en.resx).
    /// Shared messages must come from MessageUtil / Message.Enum instead.
    /// </summary>
    class ResourceMessage
    {
        static System.Resources.ResourceManager languageMessage =
            new System.Resources.ResourceManager(
                "HIS.Desktop.Plugins.HisTreatmentRecordChecking.Resources.Message.Lang",
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

        /// <summary>Khong xac dinh duoc van ban ky</summary>
        internal static string KhongXacDinhDuocVanBanKy
        {
            get { return GetValue("KhongXacDinhDuocVanBanKy"); }
        }

        /// <summary>Tu ngay phai nho hon hoac bang Den ngay (QT-03)</summary>
        internal static string TuNgayPhaiNhoHonDenNgay
        {
            get { return GetValue("TuNgayPhaiNhoHonDenNgay"); }
        }

        /// <summary>Khoang thoi gian tra soat vuot qua 31 ngay (QT-04)</summary>
        internal static string KhoangThoiGianVuotQua31Ngay
        {
            get { return GetValue("KhoangThoiGianVuotQua31Ngay"); }
        }

        /// <summary>Loai van ban chua duoc cau hinh bieu mau in (QT-16)</summary>
        internal static string LoaiVanBanChuaCauHinhBieuIn
        {
            get { return GetValue("LoaiVanBanChuaCauHinhBieuIn"); }
        }

        /// <summary>Bieu mau chua duoc ho tro tao van ban tu man tra soat (QT-16)</summary>
        internal static string BieuMauChuaDuocHoTro
        {
            get { return GetValue("BieuMauChuaDuocHoTro"); }
        }

        /// <summary>Ket qua tra soat qua lon, nen thu hep khoang thoi gian</summary>
        internal static string KetQuaQuaLon
        {
            get { return GetValue("KetQuaQuaLon"); }
        }

        /// <summary>Khong tim thay y lenh tuong ung</summary>
        internal static string KhongTimThayYLenh
        {
            get { return GetValue("KhongTimThayYLenh"); }
        }

        /// <summary>Khong tim thay ho so dieu tri tuong ung</summary>
        internal static string KhongTimThayHoSo
        {
            get { return GetValue("KhongTimThayHoSo"); }
        }
    }
}
