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
using HIS.Desktop.LocalStorage.LisConfig;
using Inventec.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIS.Desktop.Plugins.SampleInfo.Config
{
    class LisConfigCFG
    {
        private const string CONFIG_KEY__IS_AUTO_CREATE_BARCODE = "LIS.LIS_SAMPLE.IS_AUTO_CREATE_BARCODE";
        private const string CONFIG_KEY__PRINT_BARCODE_BY_BARTENDER = "LIS.LIS_SAMPLE.PRINT_BARCODE.BY_BARTENDER";

        internal static string IS_AUTO_CREATE_BARCODE;
        internal static string PRINT_BARCODE_BY_BARTENDER;

        internal static void LoadConfig()
        {
            try
            {
                IS_AUTO_CREATE_BARCODE = GetValue(CONFIG_KEY__IS_AUTO_CREATE_BARCODE);
                PRINT_BARCODE_BY_BARTENDER = GetValue(CONFIG_KEY__PRINT_BARCODE_BY_BARTENDER);
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
                return LisConfigs.Get<string>(code);
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
