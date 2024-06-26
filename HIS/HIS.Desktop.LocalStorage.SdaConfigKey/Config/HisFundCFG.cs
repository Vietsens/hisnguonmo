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
using Inventec.Common.LocalStorage.SdaConfig;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.LocalStorage.SdaConfigKey.Config
{
    public class HisFundCFG
    {
        private const string HIS_FUND__FUND_CODE__HCM = "HIS.HIS_FUND.HIS_FUND_CODE.HCM";

        private static long hisFundId__Hcm;
        public static long HisFundId__Hcm
        {
            get
            {
                if (hisFundId__Hcm <= 0)
                {
                    hisFundId__Hcm = GetId(SdaConfigs.Get<string>(HIS_FUND__FUND_CODE__HCM));
                }
                return hisFundId__Hcm;
            }
        }

        private static long GetId(string fundCode)
        {
            long result = 0;
            try
            {
                var fund = BackendDataWorker.Get<HIS_FUND>().FirstOrDefault(o => o.FUND_CODE == fundCode);
                if (fund != null)
                {
                    result = fund.ID;
                }
                if (result == 0) throw new NullReferenceException(fundCode);
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
