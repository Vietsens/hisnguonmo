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
    public class HcmPoorFundCFG
    {
        private const string VCN_ACCEPTED_SERVICE_CODE_CFG = "MOS.HCM_POOR_FUND.VCN_ACCEPTED_SERVICE_CODE";

        private static List<long> vcnAcceptServiceIds;
        public static List<long> VCN_ACCEPT_SERVICE_IDS
        {
            get
            {
                if (vcnAcceptServiceIds == null)
                {
                    vcnAcceptServiceIds = GetIds(VCN_ACCEPTED_SERVICE_CODE_CFG);
                }
                return vcnAcceptServiceIds;
            }
            set
            {
                vcnAcceptServiceIds = value;
            }
        }

        private static List<long> GetIds(string code)
        {
            List<long> result = null;
            try
            {
                result = new List<long>();
                List<string> data = SdaConfigs.Get<List<string>>(code);
                foreach (string t in data)
                {
                    string[] tmp = t.Split(':');
                    if (tmp != null && tmp.Length >= 2)
                    {
                        V_HIS_SERVICE service = BackendDataWorker.Get<V_HIS_SERVICE>().Where(o => o.SERVICE_TYPE_CODE == tmp[0] && o.SERVICE_CODE == tmp[1]).FirstOrDefault();
                        if (service != null)
                        {
                            result.Add(service.ID);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }
    }
}
