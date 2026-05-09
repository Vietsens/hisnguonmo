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
using Inventec.Common.Logging;
using HIS.Desktop.LocalStorage.HisConfig;

namespace HIS.Desktop.Plugins.BidCreate.Config
{
    class HisConfigCFG
    {
        private const string IS_SET_BHYT_INFO_FROM_TYPE_BY_DEFAULT = "MOS.HIS_MEDICINE.IS_SET_BHYT_INFO_FROM_TYPE_BY_DEFAULT";
        private const string ALLOW_ZERO_AMOUNT_IMPORT = "MOS.HIS_BID.ALLOW_ZERO_AMOUNT_IMPORT";

        internal static string IsSet__BHYT;

        /// <summary>
        /// Cho phép import dòng có số lượng = 0 (không đưa vào danh sách lỗi).
        /// Bật khi giá trị = "1".
        /// </summary>
        internal static bool AllowZeroAmountImport;

        internal static void LoadConfig()
        {
            try
            {
                LogSystem.Debug("LoadConfig => 1");
                IsSet__BHYT = GetValue(IS_SET_BHYT_INFO_FROM_TYPE_BY_DEFAULT);

                string allowZeroAmount = GetValue(ALLOW_ZERO_AMOUNT_IMPORT);
                AllowZeroAmountImport = !string.IsNullOrWhiteSpace(allowZeroAmount) && allowZeroAmount.Trim() == "1";
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
