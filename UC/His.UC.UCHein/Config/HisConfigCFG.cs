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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His.UC.UCHein.Config
{
    internal class HisConfigCFG
    {
        private const string CONFIG_KEY__WARNINGHEINPATIENTTYPECODE = "HIS.Desktop.Plugins.RegisterV2.WarningHeinPatientTypeCode";
        private const string CONFIG_KEY__HIS_PATIENT_TYPE_PATIENT_TYPE_CODE_BHYT = "HIS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BHYT";
        private const string CONFIG_KEY__IsAllowedRouteTypeByDefault = "HIS.Desktop.Plugins.IsAllowedRouteTypeByDefault";

        internal static string IsAllowedRouteTypeByDefault;
        internal const string CONFIG_KEY__PATIENT_TYPE_CODE__BHYT = "HIS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BHYT";//Doi tuong BHYT

        private const string CONFIG_KEY__NotDisplayedRouteTypeOver = "HIS.Desktop.Plugins.Register.NotDisplayedRouteTypeOver";
        internal static string NotDisplayedRouteTypeOver;
        

        private const string CONFIG_KEY__IsNotAutoCheck5Y6M = "MOS.HIS_PATIENT_TYPE_ALTER.NOT_AUTO_CHECK_5_YEAR_6_MONTH";
        public static bool IsNotAutoCheck5Y6M;
        internal static string WarningHeinPatientTypeCode;
        internal static string PatientTypeCodeBHYT;
        public static long PatientTypeId__BHYT;
        public static string PatientTypeCode__BHYT;

        internal static void LoadConfig()
        {
            try
            { 
                Inventec.Common.Logging.LogSystem.Debug("LoadConfig => 1");
                IsAllowedRouteTypeByDefault = GetValue(CONFIG_KEY__IsAllowedRouteTypeByDefault);
                NotDisplayedRouteTypeOver = GetValue(CONFIG_KEY__NotDisplayedRouteTypeOver);
                WarningHeinPatientTypeCode = GetValue(CONFIG_KEY__WARNINGHEINPATIENTTYPECODE);
                PatientTypeCode__BHYT = GetValue(CONFIG_KEY__PATIENT_TYPE_CODE__BHYT);
                PatientTypeId__BHYT = GetPatientTypeByCode(PatientTypeCode__BHYT).ID;
                IsNotAutoCheck5Y6M = GetValue(CONFIG_KEY__IsNotAutoCheck5Y6M) == "1";
                Inventec.Common.Logging.LogSystem.Debug("LoadConfig => 2");
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
                return HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(code);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                result = null;
            }
            return result;
        }
        static MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE GetPatientTypeByCode(string code)
        {
            MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE result = new MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE();
            try
            {
                result = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>().FirstOrDefault(o => o.PATIENT_TYPE_CODE.ToLower() == code.ToLower().Trim());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result ?? new MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE();
        }
    }
}
