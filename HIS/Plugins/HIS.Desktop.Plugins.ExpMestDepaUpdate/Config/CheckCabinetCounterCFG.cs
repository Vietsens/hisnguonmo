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

namespace HIS.Desktop.Plugins.ExpMestDepaUpdate.Config
{
    class CheckCabinetCounterCFG
    {
        private const string CONFIG_KEY = "HIS.DESKTOP.CHMS_EXP_MEST.CHECK_CABINET_COUNTER";
        private const string IS_CHECK = "1";

        private static bool? isCheckCabinetCounter;
        public static bool IsCheckCabinetCounter
        {
            get
            {
                if (!isCheckCabinetCounter.HasValue)
                {
                    isCheckCabinetCounter = GetBool(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(CONFIG_KEY));
                }
                return isCheckCabinetCounter.Value;
            }
        }

        private static bool GetBool(string value)
        {
            try
            {
                return (value == IS_CHECK);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }
        }
    }
}
