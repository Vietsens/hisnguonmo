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

namespace HIS.Desktop.Plugins.AggrImpMestPrintFilter
{
    internal class AppConfigKeys
    {
        #region Public key
        internal const string CONFIG_KEY__HIS_DESKTOP__IN_GOP_GAY_NGHIEN_HUONG_THAN = "CONFIG_KEY__HIS_DESKTOP__IN_GOP_GAY_NGHIEN_HUONG_THAN";
        internal const string CONFIG_KEY__HIS_DESKTOP__CHE_DO_IN_GOP_PHIEU_TRA = "CONFIG_KEY__CHE_DO_IN_GOP_PHIEU_TRA";
        internal const string OderOption = "HIS.Desktop.Plugins.AggrExpMest.OderOption";
        #endregion

        internal static List<string> ListParentMedicine
        {
            get
            {
                return ProcessListParentConfig();
            }
        }

        private static List<string> ProcessListParentConfig()
        {
            List<string> result = null;
            try
            {
                string code = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.AggrExpMestPrint.ParentMety");
                if (!String.IsNullOrWhiteSpace(code))
                {
                    result = code.Split(',').ToList();
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        // HIS.Desktop.Plugins.AggrExpMest.OderOption

        internal static Int64 ProcessOderOption
        {
            get
            {
                return ProcessOderOptionConfig();
            }
        }

        private static Int64 ProcessOderOptionConfig()
        {
            Int64 result;
            try
            {
                
                result = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(OderOption));
                
            }
            catch (Exception ex)
            {
                result = 9999 ;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }
}
