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
using HIS.Desktop.LocalStorage.HisConfig;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.PharmacyCashier.Config
{
    class HisConfigCFG
    {
        internal const string HFS_KEY__PAY_FORM_CODE = "HFS_KEY__PAY_FORM_CODE";
        private const string CONFIG_KEY__PATIENT_TYPE_CODE__BHYT = "MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BHYT";
        private const string MOS_CONFIG__PATIENT_TYPE_CODE__FEE = "MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.HOSPITAL_FEE";
        private const string MOS_CONFIG__PATIENT_TYPE_CODE__SERVICE = "MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.SERVICE";
        private const string INTEGRATION_TYPE_CFG = "MOS.OLD_SYSTEM.INTEGRATION_TYPE";

        internal static string PatientTypeCode__BHYT;
        internal static long PatientTypeId__BHYT;
        internal static long PATIENT_TYPE_ID__IS_FEE;
        internal static long PATIENT_TYPE_ID__SERVICE;
        internal static string OLD_SYSTEM__OPTION;

        static bool Get(string code)
        {
            bool result = false;
            try
            {
                if (!String.IsNullOrEmpty(code))
                {
                    result = (code == "1");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        internal static void LoadConfig()
        {
            try
            {
                PatientTypeCode__BHYT = GetValue(CONFIG_KEY__PATIENT_TYPE_CODE__BHYT);
                OLD_SYSTEM__OPTION = GetValue(INTEGRATION_TYPE_CFG);
                PatientTypeId__BHYT = GetPatientTypeByCode(PatientTypeCode__BHYT).ID;
                PATIENT_TYPE_ID__IS_FEE = GetPatientTypeByCode(GetValue(MOS_CONFIG__PATIENT_TYPE_CODE__FEE)).ID;
                PATIENT_TYPE_ID__SERVICE = GetPatientTypeByCode(GetValue(MOS_CONFIG__PATIENT_TYPE_CODE__SERVICE)).ID;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private static string GetValue(string key)
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

        private static HIS_PATIENT_TYPE GetPatientTypeByCode(string code)
        {
           HIS_PATIENT_TYPE result = new HIS_PATIENT_TYPE();
            try
            {
                result = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.PATIENT_TYPE_CODE == code);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result ?? new HIS_PATIENT_TYPE();
        }
    }
}
