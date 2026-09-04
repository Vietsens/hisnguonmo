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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His.UC.UCHein
{
    class ResourceMessage
    {
        internal static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("His.UC.UCHein.Resources.Message.Lang", System.Reflection.Assembly.GetExecutingAssembly());

        internal static string PhaiDatDu5Nam6ThangMoiCoTheChonDTMCCT
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("PhaiDatDu5Nam6ThangMoiCoTheChonDTMCCT", languageMessage, Base.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        
        internal static string MaBenhKhongKhopVoiTenBenh
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("MaBenhKhongKhopVoiTenBenh", languageMessage, Base.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        /// <summary>Bệnh {0} không khuyến khích dùng làm bệnh chính. Bạn có chắc chắn sử dụng không?</summary>
        internal static string BenhKhongKhuyenKhichDungLamBenhChinh
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("BenhKhongKhuyenKhichDungLamBenhChinh", languageMessage, Base.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string MaBenhChinhKhongHopLe
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("MaBenhChinhKhongHopLe", languageMessage, Base.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string BatBuocNhapTenBenhVoiTruongHopBenhNhanLaDungTuyenGioiThieu
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("BatBuocNhapTenBenhVoiTruongHopBenhNhanLaDungTuyenGioiThieu", languageMessage, Base.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ThoiDiemMienCungChiTraPhaiCungNamVoiNamHienTai
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ThoiDiemMienCungChiTraPhaiCungNamVoiNamHienTai", languageMessage, Base.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string SoTheBHYTKhongHopLe
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("SoTheBHYTKhongHopLe", languageMessage, Base.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string SoTheDaDuocSuDung
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("SoTheDaDuocSuDung", languageMessage, Base.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string SoTienLuyKeCungChiTraVuot06ThangLuongCoSo
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("SoTienLuyKeCungChiTraVuot06ThangLuongCoSo", languageMessage, Base.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        #region Tra cuu tien cung chi tra / mien cung chi tra tren cong BHXH

        /// <summary>Cùng chi trả lũy kế trên cổng BHXH: {0}   (hiện tại: {1})</summary>
        internal static string CungChiTraLuyKeTrenCongBHXH
        {
            get { return GetMessageValue("CungChiTraLuyKeTrenCongBHXH"); }
        }

        /// <summary>Đã cùng chi trả 6 tháng lương cơ sở: {0}</summary>
        internal static string DaCungChiTra06ThangLuongCoSo
        {
            get { return GetMessageValue("DaCungChiTra06ThangLuongCoSo"); }
        }

        /// <summary>Thời điểm miễn cùng chi trả trên cổng: {0}   (hiện tại: {1})</summary>
        internal static string ThoiDiemMienCungChiTraTrenCong
        {
            get { return GetMessageValue("ThoiDiemMienCungChiTraTrenCong"); }
        }

        /// <summary>Bạn có muốn lấy thông tin từ cổng BHXH?</summary>
        internal static string BanCoMuonLayThongTinTuCongBHXHKhong
        {
            get { return GetMessageValue("BanCoMuonLayThongTinTuCongBHXHKhong"); }
        }

        /// <summary>
        /// Lũy kế đã vượt ngưỡng nhưng cổng không trả về ngày ra viện của đợt vượt ngưỡng
        /// nên không suy được thời điểm miễn cùng chi trả.
        /// </summary>
        internal static string KhongXacDinhDuocThoiDiemMienCungChiTra
        {
            get { return GetMessageValue("KhongXacDinhDuocThoiDiemMienCungChiTra"); }
        }

        /// <summary>Có</summary>
        internal static string Co
        {
            get { return GetMessageValue("Co"); }
        }

        /// <summary>Không</summary>
        internal static string Khong
        {
            get { return GetMessageValue("Khong"); }
        }

        /// <summary>Không xác định</summary>
        internal static string KhongXacDinh
        {
            get { return GetMessageValue("KhongXacDinh"); }
        }

        /// <summary>đang để trống</summary>
        internal static string DangDeTrong
        {
            get { return GetMessageValue("DangDeTrong"); }
        }

        /// <summary>
        /// Reads one message from the resource file for the current culture.
        /// Returns an empty string instead of throwing, so a missing key never breaks the UI.
        /// </summary>
        private static string GetMessageValue(string key)
        {
            try
            {
                return Inventec.Common.Resource.Get.Value(key, languageMessage, Base.LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return "";
        }

        #endregion
    }
}
