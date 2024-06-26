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
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.MedicalStoreV2.Config
{
    class HisConfigCFG
    {

        private const string CONFIG_KEY__RED_WARNING_STORE_TIME = "HIS.Desktop.Plugins.MedicalStore.RED_WARNING_STORE_TIME";
        private const string CONFIG_KEY__BLUE_WARNING_STORE_TIME = "HIS.Desktop.Plugins.MedicalStore.BLUE_WARNING_STORE_TIME";
        private const string CONFIG_KEY__ORANGE_WARNING_STORE_TIME = "HIS.Desktop.Plugins.MedicalStore.ORANGE_WARNING_STORE_TIME";

        internal static long? RedWarningStoreTime;
        internal static long? BlueWarningStoreTime;
        internal static long? OrangeWarningStoreTime;

        internal static void LoadConfig()
        {
            try
            {
                string blue = GetValue(CONFIG_KEY__BLUE_WARNING_STORE_TIME);
                string red = GetValue(CONFIG_KEY__RED_WARNING_STORE_TIME);
                string orange = GetValue(CONFIG_KEY__ORANGE_WARNING_STORE_TIME);
                if (!String.IsNullOrWhiteSpace(blue))
                {
                    BlueWarningStoreTime = Convert.ToInt32(blue ?? "");
                }
                else
                {
                    BlueWarningStoreTime = null;
                }
                if (!String.IsNullOrWhiteSpace(red))
                {
                    RedWarningStoreTime = Convert.ToInt32(red ?? "");
                }
                else
                {
                    RedWarningStoreTime = null;
                }
                if (!String.IsNullOrWhiteSpace(orange))
                {
                    OrangeWarningStoreTime = Convert.ToInt32(orange ?? "");
                }
                else
                {
                    OrangeWarningStoreTime = null;
                }
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
                return HisConfigs.Get<string>(code);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
                result = null;
            }
            return result;
        }
    }
}
