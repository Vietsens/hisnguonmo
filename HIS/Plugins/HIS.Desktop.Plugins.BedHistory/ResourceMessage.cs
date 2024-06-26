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

namespace HIS.Desktop.Plugins.BedHistory
{
    class ResourceMessage
    {
        static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.Desktop.Plugins.BedHistory.Resources.Message.Lang", System.Reflection.Assembly.GetExecutingAssembly());

        internal static string ERROR_END_FINISH_TIME
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FORM_BED_HISTORY__ERROR_END_FINISH_TIME", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ERROR_SERVICE_REQ
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FORM_BED_HISTORY__ERROR_SERVICE_REQ", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ERROR_SERVICE_REQ_NULL
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FORM_BED_HISTORY__ERROR_SERVICE_REQ_NULL", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ERROR_USER_DELETE
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FORM_BED_HISTORY__ERROR_USER_DELETE", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string TaiKhoanKhongCoQuyenThucHienChucNang
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("TaiKhoanKhongCoQuyenThucHienChucNang", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ThongBao
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ThongBao", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string HeThongTBCuaSoThongBaoBanCoMuonHuyDuLieuKhong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("HeThongTBCuaSoThongBaoBanCoMuonHuyDuLieuKhong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ChiDinhDichVu_KhongCoDoiTuongThanhToan
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ChiDinhDichVu_KhongCoDoiTuongThanhToan", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ERROR_FINISH_TIME_PTTT
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FORM_BED_ASSIGN__ERROR_FINISH_TIME_PTTT", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ERROR_FROM_TO_TIME
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FORM_BED_ASSIGN__ERROR_FROM_TO_TIME", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string ERROR_OVERLAP_FINISH_TIME
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FORM_BED_ASSIGN__ERROR_OVERLAP_FINISH_TIME", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string ERROR_OVERLAP_START_TIME
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FORM_BED_ASSIGN__ERROR_OVERLAP_START_TIME", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string GiuongDaCoBenhNhanNam
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("HIS_Desktop_Plugins_BedAssign__GiuongDaCoBenhNhanNam", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string KhongDuocNamGhepGiuong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("HIS_Desktop_Plugins_BedAssign__KhongDuocNamGhepGiuong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string GiuongVuotQuaSoLuong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("HIS_Desktop_Plugins_BedAssign__GiuongVuotQuaSoLuong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ERROR_BED_ID
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FORM_BED_HISTORY__ERROR_BED_ID", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ThieuTruongDuLieuBatBuoc
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ThieuTruongDuLieuBatBuoc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string KhongXacDinhDuocDuLieu
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("KhongXacDinhDuocDuLieu", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ThoiGianBatDauKhongDuocNhoHonThoiGianVaoVien
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ThoiGianBatDauKhongDuocNhoHonThoiGianVaoVien", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string TatCaDuLieuDaLuu
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("TatCaDuLieuDaLuu", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string DichVuCoDTPTBatBuocNhungKhongCoChinhSachGia
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("DichVuCoDTPTBatBuocNhungKhongCoChinhSachGia", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string DichVuCoDTTTBatBuocNhungKhongCoChinhSachGia
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("DichVuCoDTTTBatBuocNhungKhongCoChinhSachGia", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string DichVuCoDTTTBatBuocKhongChoPhepSua
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("DichVuCoDTTTBatBuocKhongChoPhepSua", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string DichVuCoDTPTBatBuocKhongChoPhepSua
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("DichVuCoDTPTBatBuocKhongChoPhepSua", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ChiDinhDichVu_BatBuocChonNguonKhac
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ChiDinhDichVu_BatBuocChonNguonKhac", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
    }
}
