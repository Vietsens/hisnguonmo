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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ExpMestSaleCreateV2.Config
{
    internal class HisConfigCFG
    {

        private const string CONFIG_KEY__MUST_BE_FINISHED_BEFORED_PRINTING = "HIS.Desktop.Plugins.ExpMestSale.MustBeFinishedBeforePrinting";

        internal static bool IS_MUST_BE_FINISHED_BEFORED_PRINTING;

        internal static void LoadConfig()
        {
            try
            {
                IS_MUST_BE_FINISHED_BEFORED_PRINTING = GetValue(CONFIG_KEY__MUST_BE_FINISHED_BEFORED_PRINTING) == "1";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal static bool IsMustBeFinishBeforePrinting()
        {
            try
            {
                IS_MUST_BE_FINISHED_BEFORED_PRINTING = GetValue(CONFIG_KEY__MUST_BE_FINISHED_BEFORED_PRINTING) == "1";
                if (IS_MUST_BE_FINISHED_BEFORED_PRINTING)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal static string GetValue(string code)
        {
            string result = null;
            try
            {
                return HisConfigs.Get<string>(code);
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
