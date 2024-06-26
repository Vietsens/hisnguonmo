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

namespace HIS.Desktop.Plugins.TransactionBillTwoInOne.Base
{
    class ResourceMessageLang
    {
        static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.Desktop.Plugins.TransactionBillTwoInOne.Resources.Message.Lang", System.Reflection.Assembly.GetExecutingAssembly());

        internal static string TruongDuLieuBatBuoc
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_TransactionBillTwoInOne__TruongDuLieuBatBuoc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string KhongTimThayMaDieuTri
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_TransactionBillTwoInOne__KhongTimThayMaDieuTri", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
                    return Inventec.Common.Resource.Get.Value("Plugins_TransactionBillTwoInOne__ThieuTruongDuLieuBatBuoc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
                    return Inventec.Common.Resource.Get.Value("Plugins_TransactionBillTwoInOne__TieuDeCuaSoThongBaoLaThongBao", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
                    return Inventec.Common.Resource.Get.Value("Plugins_TransactionBillTwoInOne__TieuDeCuaSoThongBaoLaCanhBao", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
                    return Inventec.Common.Resource.Get.Value("Plugins_TransactionBillTwoInOne__XuLyThatBai", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string NguoiDungChuaChonDichVuDeThanhToan
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_TransactionBillTwoInOne__NguoiDungChuaChonDichVuDeThanhToan", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string ChucNangNayChuDuocHoTroTrongPhienBanNay
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_TransactionBillTwoInOne__ChucNangNayChuDuocHoTroTrongPhienBanNay", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string SoHoaDonVienPhiPhaiLaSo
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_TransactionBillTwoInOne__SoHoaDonVienPhiPhaiLaSo", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string SoHoaDonDichVuPhaiLaSo
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_TransactionBillTwoInOne__SoHoaDonDichVuPhaiLaSo", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string DoDaiKhongDuocVuotQua
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_TransactionBillTwoInOne__DoDaiKhongDuocVuotQua", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string BenhNhanDaXuatHoaDonBanCoMuonXuatHoaDonMoi
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_TransactionBillTwoInOne__BenhNhanDaXuatHoaDonBanCoMuonXuatHoaDonMoi", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string HuyGiaoDichThanhToanTheThatBai
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_TransactionBillTwoInOne__HuyGiaoDichThanhToanTheThatBai", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string KhongChoPhepHaiHoaDonCoHinhThucThanhToanKhacNhau
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_TransactionBillTwoInOne__KhongChoPhepHaiHoaDonCoHinhThucThanhToanKhacNhau", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
