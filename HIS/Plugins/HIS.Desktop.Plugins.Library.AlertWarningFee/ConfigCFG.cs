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
using Inventec.Common.Logging;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.AlertWarningFee
{
    class ConfigCFG
    {
        private const string CONFIG_KEY__IS_ALERT_HOSPITALFEE_NOTBHYT = "HIS.Desktop.Plugins.Library.AlertHospitalFeeNotBHYT";

        internal const string valueString__true = "1";
        internal const int valueInt__true = 1;

        internal static bool IsAlertHospitalFeeNotBHYT;

        internal static void LoadConfig()
        {
            IsAlertHospitalFeeNotBHYT = (GetValue(CONFIG_KEY__IS_ALERT_HOSPITALFEE_NOTBHYT) == valueString__true);
        }

        private static string GetValue(string key)
        {
            try
            {
                return HisConfigs.Get<string>(key);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return "";
        }
    }
}
