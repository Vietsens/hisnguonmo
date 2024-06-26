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

namespace HIS.Desktop.Plugins.HisCareSum.Config
{
    class HisConfigCFG
    {
        private const string CONFIG_KEY__ALLOW_UPDATING_AFTER_LOCKING_TREATMENT = "MOS.HIS_CARE_SUM.ALLOW_UPDATING_AFTER_LOCKING_TREATMENT";
        public const string CONFIG_KEY__HIS_DESKTOP_PLUGINS_CARE_IS_PRINT_MERGE = "HIS.Desktop.Plugins.EmrDocument.IsPrintMerge";

        internal static string AllowUpdatingAfterLockingTreatment;

        internal static void LoadConfig()
        {
            try
            {
                AllowUpdatingAfterLockingTreatment = GetValue(CONFIG_KEY__ALLOW_UPDATING_AFTER_LOCKING_TREATMENT);
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
                LogSystem.Error(ex);
                result = null;
            }
            return result;
        }


    }
}
