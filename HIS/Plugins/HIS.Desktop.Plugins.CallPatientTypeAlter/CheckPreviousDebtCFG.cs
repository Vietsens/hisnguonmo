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
using Inventec.Common.LocalStorage.SdaConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.CallPatientTypeAlter
{
    class CheckPreviousDebtCFG
    {
        private const string CONFIG_KEY = "MOS.HIS_TREATMENT.IS_CHECK_PREVIOUS_DEBT";
        private const string DEFAULT__IS_CHECK_PREVIOUS_DEBT = "1";

        private static bool? isCheckPreviousDebt;
        public static bool IsCheckPreviousDebt
        {
            get
            {
                if (!isCheckPreviousDebt.HasValue)
                {
                    isCheckPreviousDebt = GetIsCheck(SdaConfigs.Get<string>(CONFIG_KEY));
                }
                return isCheckPreviousDebt.Value;
            }
        }

        private static bool GetIsCheck(string value)
        {
            return (value == DEFAULT__IS_CHECK_PREVIOUS_DEBT);
        }
    }
}
