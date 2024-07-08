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

namespace His.UC.UCHein.Config
{
    internal class HisConfigCFG
    {
        private const string CONFIG_KEY__IsAllowedRouteTypeByDefault = "HIS.Desktop.Plugins.IsAllowedRouteTypeByDefault";

        internal static string IsAllowedRouteTypeByDefault;

        private const string CONFIG_KEY__NotDisplayedRouteTypeOver = "HIS.Desktop.Plugins.Register.NotDisplayedRouteTypeOver";
        internal static string NotDisplayedRouteTypeOver;

        private const string CONFIG_KEY__IsNotAutoCheck5Y6M = "MOS.HIS_PATIENT_TYPE_ALTER.NOT_AUTO_CHECK_5_YEAR_6_MONTH";
        public static bool IsNotAutoCheck5Y6M;

        internal static void LoadConfig()
        {
            try
            { 
                Inventec.Common.Logging.LogSystem.Debug("LoadConfig => 1");
                IsAllowedRouteTypeByDefault = GetValue(CONFIG_KEY__IsAllowedRouteTypeByDefault);
                NotDisplayedRouteTypeOver = GetValue(CONFIG_KEY__NotDisplayedRouteTypeOver);
                IsNotAutoCheck5Y6M = GetValue(CONFIG_KEY__IsNotAutoCheck5Y6M) == "1";
                Inventec.Common.Logging.LogSystem.Debug("LoadConfig => 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private static string GetValue(string code)
        {
            string result = null;
            try
            {
                return HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(code);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                result = null;
            }
            return result;
        }
    }
}
