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
using Inventec.Common.Logging;
using Inventec.Core;
using MOS.Filter;
using SDA.EFMODEL.DataModels;
using System;
using System.Linq;
using SDA.Filter;
using System.Collections.Generic;
using HIS.Desktop.LocalStorage.LocalData;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.LocalStorage.SdaConfig;

namespace HIS.Desktop.LocalStorage.SdaConfigKey.Config
{
    public class HisMediOrgCFG
    {
        private static string mediOrgValueCurrent;
        public static string MEDI_ORG_VALUE__CURRENT
        {
            get
            {
                if (mediOrgValueCurrent == null)
                {
                    mediOrgValueCurrent = GetBranch().HEIN_MEDI_ORG_CODE;
                }
                return mediOrgValueCurrent;
            }
            set
            {
                mediOrgValueCurrent = value;
            }
        }

        private static List<string> mediOrgCodesAccept;
        public static List<string> MEDI_ORG_CODES__ACCEPT
        {
            get
            {
                if (mediOrgCodesAccept == null || mediOrgCodesAccept.Count == 0)
                {
                    mediOrgCodesAccept = GetCodesAccept(GetBranch().ACCEPT_HEIN_MEDI_ORG_CODE);
                }
                return mediOrgCodesAccept;
            }
            set
            {
                mediOrgCodesAccept = value;
            }
        }

        private static MOS.EFMODEL.DataModels.HIS_BRANCH GetBranch()
        {
            MOS.EFMODEL.DataModels.HIS_BRANCH result = null;
            try
            {
                result = BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == WorkPlace.GetBranchId());
                if (result == null)
                    result = new MOS.EFMODEL.DataModels.HIS_BRANCH();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                result = new MOS.EFMODEL.DataModels.HIS_BRANCH();
            }
            return result;
        }

        private static List<string> GetCodesAccept(string codes)
        {
            List<string> result = new List<string>();
            try
            {
                //string value = SdaConfigs.Get<string>(codes);
                if (String.IsNullOrEmpty(codes)) throw new ArgumentNullException(codes);

                char[] delimiterChars = { ',' };

                string[] acceptCodes = codes.Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries);
                if (acceptCodes != null && acceptCodes.Count() > 0)
                {
                    result.AddRange(acceptCodes.Where(o => (!String.IsNullOrEmpty(o))));
                }
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
