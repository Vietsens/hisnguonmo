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

namespace HIS.Desktop.Plugins.RegisterGate.Resource
{
    class ResourceMessage
    {
        static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.Desktop.Plugins.HisRegisterGate.Resource.Message.Lang", System.Reflection.Assembly.GetExecutingAssembly());

        internal static string ThongBao
        {
            get
            {
                string tb = "";
                try
                {
                    return tb = Inventec.Common.Resource.Get.Value("ThongBao", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return tb;
            }
        }

        internal static string TaiKhoanKhongCoQuyenThucHienChucNang
        {
            get
            {
                string tb = "";
                try
                {
                    return  tb = Inventec.Common.Resource.Get.Value("TaiKhoanKhongCoQuyenThucHienChucNang", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return tb;
            }
        }

        internal static string ChucNangDangPhatTrienVuiLongThuLaiSau
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ChucNangDangPhatTrienVuiLongThuLaiSau", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong
        {
            get
            {
                string tb = "";
                try
                {
                    return tb = Inventec.Common.Resource.Get.Value("HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return tb;
            }
        }

        internal static string HeThongTBCuaSoThongBaoBanCoMuonThucNhapDuLieuKhong
        {
            get
            {
                string tb = "";
                try
                {
                    return tb = Inventec.Common.Resource.Get.Value("HeThongTBCuaSoThongBaoBanCoMuonThucNhapDuLieuKhong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return tb;
            }
        }

        internal static string KhongTimThayBieuMauIn
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("KhongTimThayBieuMauIn", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string TaiThanhCong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("TaiThanhCong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string BanCoMuonMoFile
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("BanCoMuonMoFile", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
                    return Inventec.Common.Resource.Get.Value("XuLyThatBai", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string BanChuaChonThoiGianThucXuat
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("BanChuaChonThoiGianThucXuat", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string BieuMauDangMo
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("BieuMauDangMo", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
                    return Inventec.Common.Resource.Get.Value("NguoiDungNhapNgayKhongHopLe", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string BanCoMuonXoaDuLieu
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("BanCoMuonXoaDuLieu", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string BanCoMuonKhoaDuLieu
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("BanCoMuonKhoaDuLieu", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string BanCoMuonMoKhoaDuLieu
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("BanCoMuonMoKhoaDuLieu", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
