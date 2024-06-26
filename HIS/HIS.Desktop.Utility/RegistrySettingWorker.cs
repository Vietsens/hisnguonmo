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
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Utility
{
    public class RegistrySettingWorker
    {
        const string SOFTWARE_FOLDER = "SOFTWARE";
        const string COMPANY_FOLDER = "INVENTEC";
        static readonly string APP_FOLDER = ConfigurationManager.AppSettings["Inventec.Desktop.ApplicationCode"];

        public const string FONT_KEY = "FontName";
        private static RegistryKey appFolder = Registry.CurrentUser.CreateSubKey(SOFTWARE_FOLDER).CreateSubKey(COMPANY_FOLDER).CreateSubKey(APP_FOLDER);

        public static void ChangeValue(string key, object value)
        {
            try
            {              
                appFolder.SetValue(key, value);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public static object GetValue(string key)
        {
            object value = null;
            try
            {
                var f = appFolder.GetValue(key);
                value = f;
            }
            catch (Exception ex)
            {               
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return value;
        }
    }
}
