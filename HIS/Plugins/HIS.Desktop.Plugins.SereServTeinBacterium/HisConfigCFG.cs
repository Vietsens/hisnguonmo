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
using HIS.Desktop.LocalStorage.HisConfig;
using Inventec.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.SereServTeinBacterium.Config
{
    class HisConfigCFG
    {
        private const string CONFIG_KEY__AUTO_RETURN_RESULT_BEFORE_PRINT = "HIS.Desktop.Plugins.ReturnMicrobiologicalResults__AutoReturnResultBeforePrint";
        private const string CONFIG_KEY__RETURN_RESULT_WARNING_TIME = "HIS.Desktop.Plugins.ReturnMicrobiologicalResults__ReturnResultWarningTime";

        private const string CONFIG_KEY__IS_USE_SIGN_EMR = "HIS.HIS.DESKTOP.IS_USE_SIGN_EMR";

        internal static string IS_USE_SIGN_EMR;
        internal static string AUTO_RETURN_RESULT_BEFORE_PRINT;
        internal static string WARNING_TIME_RETURN_RESULT;

        internal static void LoadConfig()
        {
            try
            {
                IS_USE_SIGN_EMR = GetValue(CONFIG_KEY__IS_USE_SIGN_EMR);
                AUTO_RETURN_RESULT_BEFORE_PRINT = GetValue(CONFIG_KEY__AUTO_RETURN_RESULT_BEFORE_PRINT);
                WARNING_TIME_RETURN_RESULT = GetValue(CONFIG_KEY__RETURN_RESULT_WARNING_TIME);
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
