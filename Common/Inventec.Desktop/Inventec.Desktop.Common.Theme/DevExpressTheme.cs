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
using Inventec.Common.Logging;
using Inventec.Desktop.Common.LanguageManager;
using Microsoft.Win32;
using System;

namespace Inventec.Desktop.Common.Theme
{
    public class DevExpressTheme
    {
        private static string themeName = "";
        public static string THEME_NAME
        {
            get
            {
                try
                {
                    RegistryKey hfsFolder = Registry.CurrentUser.CreateSubKey(RegistryConstant.SOFTWARE_FOLDER).CreateSubKey(RegistryConstant.COMPANY_FOLDER).CreateSubKey(RegistryConstant.APP_FOLDER);
                    themeName = (string)hfsFolder.GetValue(RegistryConstant.THEME_KEY);
                    return themeName;
                }
                catch (Exception ex)
                {
                    LogSystem.Error(ex);
                    return null;
                }
            }
            set
            {
                try
                {
                    RegistryKey hfsFolder = Registry.CurrentUser.CreateSubKey(RegistryConstant.SOFTWARE_FOLDER).CreateSubKey(RegistryConstant.COMPANY_FOLDER).CreateSubKey(RegistryConstant.APP_FOLDER);
                    hfsFolder.SetValue(RegistryConstant.THEME_KEY, value);
                }
                catch (Exception ex)
                {
                    LogSystem.Error(ex);
                }
            }
        }
    }
}
