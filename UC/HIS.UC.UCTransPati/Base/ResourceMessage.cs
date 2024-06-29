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

namespace HIS.UC.UCTransPati
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
    }
}
