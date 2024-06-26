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
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.Hospitalize.Config
{
    public class HisConfig
    {
        private const string IS_USING_SERVER_TIME = "MOS.IS_USING_SERVER_TIME";

        private const string RELATIVES_INFOR="HIS.Desktop.Plugins.Hospitalize.RelativesInforOption";
        private const string IS_IN_HOSPITALIZATION_REASON_REQUIRED = "HIS.Desktop.Plugins.ExamServiceReqExecute.InHospitalizationReasonRequired";
        private const string CHECK_ICD_WHEN_SAVE = "HIS.Desktop.Plugins.CheckIcdWhenSave";
        internal static string CheckIcdWhenSave;
        public static bool IsUsingServerTime;
        public static string RelativesInforOption;
        public static bool IsInHospitalizationReasonRequired;
        public static void LoadConfig()
        {
            CheckIcdWhenSave = GetValue(CHECK_ICD_WHEN_SAVE);
            IsUsingServerTime = GetValue(IS_USING_SERVER_TIME) == GlobalVariables.CommonStringTrue;
            RelativesInforOption = GetValue(RELATIVES_INFOR);
            IsInHospitalizationReasonRequired = GetValue(IS_IN_HOSPITALIZATION_REASON_REQUIRED) == GlobalVariables.CommonStringTrue;
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
