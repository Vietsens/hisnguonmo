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

namespace HIS.Desktop.Plugins.InsuranceExpertise.Config
{
    class HisConfigCFG
    {
        public const string MRS_HIS_REPORT_BHYT_NDS_ICD_CODE__OTHER = "MRS.HIS_REPORT_BHYT_NDS_ICD_CODE__OTHER";
        public const string MRS_HIS_REPORT_BHYT_NDS_ICD_CODE__TE = "MRS.HIS_REPORT_BHYT_NDS_ICD_CODE__TE";
        public const string MOS_HIS_HEIN_APPROVAL__IS_AUTO_EXPORT_XML = "MOS.HIS_HEIN_APPROVAL.IS_AUTO_EXPORT_XML";
        public const string MOS_GENERATE_STORE_BORDEREAU_CODE_WHEN_LOCK_HEIN = "MOS.HIS_TREATMENT.GENERATE_STORE_BORDEREAU_CODE_WHEN_LOCK_HEIN";
        public const string MOS_STORE_BORDEREAU_CODE_OPTION = "MOS.HIS_TREATMENT.STORE_BORDEREAU_CODE_OPTION";

        public static bool isGenerateStoreBordereauCodeWhenLockHein = GetValue(MOS_GENERATE_STORE_BORDEREAU_CODE_WHEN_LOCK_HEIN) == "1";
        public static string OptionStoreBordereauCode = GetValue(MOS_STORE_BORDEREAU_CODE_OPTION);
        internal static string GetValue(string key)
        {
            try
            {
                return HisConfigs.Get<string>(key);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return "";
        }

        internal static List<string> GetListValue(string key)
        {
            try
            {
                return HisConfigs.Get<List<string>>(key);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return null;
        }
    }
}
