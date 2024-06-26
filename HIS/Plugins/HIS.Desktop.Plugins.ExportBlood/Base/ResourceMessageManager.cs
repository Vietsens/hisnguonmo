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

namespace HIS.Desktop.Plugins.ExportBlood.Base
{
    class ResourceMessageManager
    {
        static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.Desktop.Plugins.ExportBlood.Resources.Message.Lang", System.Reflection.Assembly.GetExecutingAssembly());

        internal static string HanDungKhongDuocBeHonHienTai
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_ExportBlood__HanDungKhongDuocBeHonHienTai", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string TuiMauKhongCoChinhSachGiaChoDoiTuong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_ExportBlood__TuiMauKhongCoChinhSachGiaChoDoiTuong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string SoLuongTuiMauCuaLoaiLonHonSoLuongYeuCau
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_ExportBlood__SoLuongTuiMauCuaLoaiLonHonSoLuongYeuCau_BanMuonTiepTuc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string MaVachKhongChinhXac
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_ExportBlood__MaVachKhongChinhXac", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string LoaiMauKhongCoTuiMauNaoTrongDanhSachXuat
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_ExportBlood__LoaiMauKhongCoTuiMauNaoTrongDanhSachXuat_BanMuonTiepTuc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string SoLuongTuiMauXuatNhoHonSoLuongYeuCau
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_ExportBlood__SoLuongTuiMauXuatNhoHonSoLuongYeuCau_BanMuonTiepTuc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string TuiMauKhongThuocLoaiMauDangChon
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_ExportBlood__TuiMauKhongThuocLoaiMauDangChon_BanMuonTiepTuc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string TuiMauChuaDuocThietLapChinhSachGia
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_ExportBlood__TuiMauChuaDuocThietLapChinhSachGia", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string TruongDuLieuBatBuoc
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_ExportBlood__TruongDuLieuBatBuoc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
                    return Inventec.Common.Resource.Get.Value("Plugins_ExportBlood__ThieuTruongDuLieuBatBuoc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string TuiMauDaDuocThemVaoDanhSachXuat
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_ExportBlood__TuiMauDaDuocThemVaoDanhSachXuat", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
                    return Inventec.Common.Resource.Get.Value("Plugins_ExportBlood__TieuDeCuaSoThongBaoLaThongBao", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
                    return Inventec.Common.Resource.Get.Value("Plugins_ExportBlood__TieuDeCuaSoThongBaoLaCanhBao", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string NguoiDungNhapNgayKhongHopLe
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_ExportBlood__NguoiDungNhapNgayKhongHopLe", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
