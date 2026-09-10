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
        public const string CONFIG_KEY__HIS_DESKTOP_PLUGINS_CARE_IS_PRINT_MERGE = "HIS.Desktop.Plugins.Care.IsPrintMerge";
        public const string CONFIG_KEY__HIS_DESKTOP_PLUGINS_EMR_DOCUMENT_IS_PRINT_MERGE = "HIS.Desktop.Plugins.EmrDocument.IsPrintMerge";

        internal static string AllowUpdatingAfterLockingTreatment;

        /// <summary>
        /// Merge-print flag for care sheets. Reads the dedicated key first;
        /// falls back to the legacy shared key (EmrDocument.IsPrintMerge) when not configured,
        /// so hospitals without the new key keep the current behavior.
        /// </summary>
        public static long GetKeyPrintMerge()
        {
            long result = 0;
            try
            {
                string value = HisConfigs.Get<string>(CONFIG_KEY__HIS_DESKTOP_PLUGINS_CARE_IS_PRINT_MERGE);
                if (String.IsNullOrWhiteSpace(value))
                {
                    value = HisConfigs.Get<string>(CONFIG_KEY__HIS_DESKTOP_PLUGINS_EMR_DOCUMENT_IS_PRINT_MERGE);
                }
                result = Inventec.Common.TypeConvert.Parse.ToInt64(value);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
                result = 0;
            }
            return result;
        }

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
