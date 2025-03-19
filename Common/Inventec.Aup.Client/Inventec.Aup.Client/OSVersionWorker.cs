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
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Aup.Client
{
    public class OSVersionWorker
    {
        const string IS_AUTO_UPDATE_TRUE = "1";
        internal const string currentOSVersionx86 = "x86";
        internal const string currentOSVersionx64 = "x64";

        static string currentOSVersion;
        internal static string CurrentOSVersion
        {
            get
            {
                try
                {
                    bool is64bit = OsInfo.Utils.OperatingSystemBitChecker.Is64BitOperatingSystem();
                    if (is64bit)
                        currentOSVersion = currentOSVersionx64;
                    else
                        currentOSVersion = currentOSVersionx86;
                }
                catch (Exception ex)
                {
                    currentOSVersion = currentOSVersionx86;
                }
                return currentOSVersion;
            }
        }

        static void AddUpdateAppSettings(string key, string value)
        {
            try
            {
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;
                if (settings[key] == null)
                {
                    settings.Add(key, value);
                }
                else
                {
                    settings[key].Value = value;
                }
                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            }
            catch (ConfigurationErrorsException ex)
            {
            }
            catch (Exception ex)
            {
            }
        }

        static bool isAutoUpdate = false;
        public static bool IsAutoUpdate
        {
            get
            {
                try
                {
                    isAutoUpdate = (ConfigurationManager.AppSettings["Aup.IsAutoUpdate"].ToString() == IS_AUTO_UPDATE_TRUE);
                }
                catch (Exception ex)
                {
                    AddUpdateAppSettings("Aup.IsAutoUpdate", "0");
                    isAutoUpdate = false;
                }
                return isAutoUpdate;
            }
        }
    }
}
