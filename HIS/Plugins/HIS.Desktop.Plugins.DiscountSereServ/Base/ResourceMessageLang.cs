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

namespace HIS.Desktop.Plugins.DiscountSereServ.Base
{
    class ResourceMessageLang
    {
        static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.Desktop.Plugins.DiscountSereServ.Resources.Message.Lang", System.Reflection.Assembly.GetExecutingAssembly());

        internal static string PhanTramChietKhauBeHonKhong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_DiscountSereServ__PhanTramChietKhauBeHonKhong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
                return "";
            }
        }

        internal static string PhanTramChietKhauLonHonMoTram
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_DiscountSereServ__PhanTramChietKhauLonHonMoTram", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
                return "";
            }
        }

        internal static string SoTienChietKhauBeHonKhong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_DiscountSereServ__SoTienChietKhauBeHonKhong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
                return "";
            }
        }

        internal static string SoTienChietKhauLonHonSoTienBenhNhanPhaiTra
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_DiscountSereServ__SoTienChietKhauLonHonSoTienBenhNhanPhaiTra", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
                return "";
            }
        }
    }
}
