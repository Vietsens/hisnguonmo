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

namespace HIS.Desktop.Plugins.ExportXmlQD4210.Base
{
    public class ResourceMessageLang
    {
        public static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.Desktop.Plugins.ExportXmlQD4210.Resources.Message.Lang", System.Reflection.Assembly.GetExecutingAssembly());

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

        internal static string TruongDuLieuBatBuoc
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_ExportXml__TruongDuLieuBatBuoc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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

        internal static string ChuaThietLapFolderLuu
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_ExportXml__ChuaThietLapFolderLuu", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ChuaThietLapChuKyThoiGianTai
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_ExportXml__ChuaThietLapChuKyThoiGianTai", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string LuuFileVaoThuMucMayTramThatBai
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_ExportXml__LuuFileVaoThuMucMayTramThatBai", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string TaiFileVeMayTramThatBai
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_ExportXml__TaiFileVeMayTramThatBai", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
                    return Inventec.Common.Resource.Get.Value("Plugins_ExportXml__KhongTimThayFile", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string FileDangMo
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_ExportXml__FileDangMo", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
                    return Inventec.Common.Resource.Get.Value("Plugins_ExportXml__BanCoMuonMoFile", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string LoiKhiLayDuLieuLoc
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_ExportXml__LoiKhiLayDuLieuLoc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string KhongCoDuongDanTaiFileVeMayTram
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_ExportXml__KhongCoDuongDanTaiFileVeMayTram", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string TuDongXuatFileThanhCong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_ExportXml__TuDongXuatFileThanhCong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string TuDongXuatFileThatBai
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_ExportXml__TuDongXuatFileThatBai", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string TaiFileVeMayTramThanhCong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_ExportXml__TaiFileVeMayTramThanhCong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
