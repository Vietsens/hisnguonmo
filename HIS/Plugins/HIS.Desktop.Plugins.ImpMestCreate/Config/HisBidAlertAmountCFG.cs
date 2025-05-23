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

namespace HIS.Desktop.Plugins.ImpMestCreate.Config
{
    internal class HisBidAlertAmountCFG
    {
        private const string CONFIG_KEY_ALERT_AMOUNT = "HIS.DESKTOP.IMP_MEST_CREATE.BID_MEDI_MATE.ALERT_AMOUNT";

        private static long? bid_AlertAmount;
        public static long Bid_AlertAmount
        {
            get
            {
                if (!bid_AlertAmount.HasValue)
                {
                    bid_AlertAmount = GetAmount(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(CONFIG_KEY_ALERT_AMOUNT));
                }
                return bid_AlertAmount.Value;
            }
        }

        private static long GetAmount(string code)
        {
            long result = 0;
            try
            {
                if (!String.IsNullOrEmpty(code))
                {
                    result = Convert.ToInt64(code);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = 0;
            }
            return result;
        }
    }
}
