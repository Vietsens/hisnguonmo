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

namespace HIS.Desktop.Plugins.InsuranceExpertise.Base
{
    class ResourceMessageLang
    {
        static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.Desktop.Plugins.InsuranceExpertise.Resources.Message.Lang", System.Reflection.Assembly.GetExecutingAssembly());

        internal static string TruongDuLieuBatBuoc
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_InsuranceExpertise__TruongDuLieuBatBuoc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string TieuDeCuaSoThongBaoLaThongBao
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_InsuranceExpertise__TieuDeCuaSoThongBaoLaThongBao", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string TieuDeCuaSoThongBaoLaCanhBao
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_InsuranceExpertise__TieuDeCuaSoThongBaoLaCanhBao", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string XuLyThatBai
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_InsuranceExpertise__XuLyThatBai", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string KhongCoHoSoDieuTriNaoChuaDuocDuyet
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_InsuranceExpertise__KhongCoHoSoDieuTriNaoChuaDuocDuyet", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string KhongCoHoSoDieuTriNaoDaDuocDuyet
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_InsuranceExpertise__KhongCoHoSoDieuTriNaoDaDuocDuyet", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string HoSoDieuTriDaDuocKhoaBhyt
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_InsuranceExpertise__HoSoDieuTriDaDuocKhoaBhyt", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string HoSoDieuTriChuaDuocDuyetGiamDinhBhyt
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_InsuranceExpertise__HoSoDieuTriChuaDuocDuyetGiamDinhBhyt", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string HoSoDieuTriDangDuocMoKhoaBhyt
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_InsuranceExpertise__HoSoDieuTriDangDuocMoKhoaBhyt", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string KhongTaiDuocFileXml
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_InsuranceExpertise__KhongTaiDuocFileXml", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string KhongTaoDuocFolderLuuXml
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_ExportXml__KhongTaoDuocFolderLuuXml", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string KhongThieLapDuocCauHinhDuLieuXuatXml
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_ExportXml__KhongThieLapDuocCauHinhDuLieuXuatXml", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string CacMaDieuTriKhongXuatDuocXml
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_ExportXml__CacMaDieuTriKhongXuatDuocXml", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string KhongTimThayFileImport
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_InsuranceExpertise__KhongTimThayFileImport", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string TaiFileThanhCong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_InsuranceExpertise__TaiFileThanhCong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string TaiFileThatBai
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_InsuranceExpertise__TaiFileThatBai", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string DuLieuLocKhongDuocDeTrong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_InsuranceExpertise__DuLieuLocKhongDuocDeTrong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string CoLoiKhiLayDuLieuLoc
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_InsuranceExpertise__CoLoiKhiLayDuLieuLoc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string ChuaCoFile
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_InsuranceExpertise__ChuaCoFile", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string ChucNangChuaHoTroPhienBanHienTai
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_InsuranceExpertise__ChucNangChuaHoTroPhienBanHienTai", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string NgayVaoKhongHopLe
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_InsuranceExpertise__NgayVaoKhongHopLe", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string NgayRaKhongHopLe
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_InsuranceExpertise__Plugins_InsuranceExpertise__NgayRaKhongHopLe", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string ThoiGianKhoaVienPhiTu
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_InsuranceExpertise__ThoiGianKhoaVienPhiTu", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string ThoiGianKetThucDieuTriTu
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_InsuranceExpertise__ThoiGianKetThucDieuTriTu", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string ThoiGianDuyetKhoaBHYTTu
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_InsuranceExpertise__ThoiGianDuyetKhoaBHYTTu", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
