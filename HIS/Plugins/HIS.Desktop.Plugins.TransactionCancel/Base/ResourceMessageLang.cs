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

namespace HIS.Desktop.Plugins.TransactionCancel.Base
{
    class ResourceMessageLang
    {
        static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.Desktop.Plugins.TransactionCancel.Resources.Message.Lang", System.Reflection.Assembly.GetExecutingAssembly());

        internal static string TruongDuLieuBatBuoc
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_TransactionDepositCancel__TruongDuLieuBatBuoc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string BanChuaNhapLyDoHuy
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_TransactionDepositCancel__BanChuaNhapLyDoHuy", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
                    return Inventec.Common.Resource.Get.Value("Plugins_TransactionDepositCancel__HuyGiaoDichThanhToanTheThatBai", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string GiaoDichDaDuocHuyQuaTHETruocDo
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_TransactionDepositCancel__GiaoDichDaDuocHuyQuaTHETruocDo", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string GiaoDichTTTUHU1THEQuocGiaHuyNoiBoBanCoMuonTiepTuc
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_TransactionDepositCancel__GiaoDichTTTUHU1THEQuocGiaHuyNoiBoBanCoMuonTiepTuc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string GiaoDichHoanUng1TheQuocGiaKhongTheHuyTiepTucKhong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_TransactionDepositCancel__GiaoDichHoanUng1TheQuocGiaKhongTheHuyTiepTucKhong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string TieuDeThongBao
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_TransactionDepositCancel__TieuDeThongBao", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string ThoiGianHuyGiaoDichLonHonThoiGianGiaoDich
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_TransactionDepositCancel__ThoiGianHuyGiaoDichLonHonThoiGianGiaoDich", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        /// <summary>Máy POS đang cấu hình không hỗ trợ hủy giao dịch tự động — phải hủy trên máy POS trước.</summary>
        internal static string MayPosKhongHoTroHuyGiaoDich
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_TransactionDepositCancel__MayPosKhongHoTroHuyGiaoDich", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        /// <summary>Nhập mã giao dịch hủy (hoặc số biên lai hủy) in trên máy POS.</summary>
        internal static string NhapMaHuyTrenMayPos
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_TransactionDepositCancel__NhapMaHuyTrenMayPos", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        /// <summary>Chưa nhập mã hủy lấy từ máy POS nên không thể tiếp tục hủy giao dịch.</summary>
        internal static string ChuaNhapMaHuyTrenMayPos
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_TransactionDepositCancel__ChuaNhapMaHuyTrenMayPos", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
