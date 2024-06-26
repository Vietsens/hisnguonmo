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
using Inventec.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisAssignBlood.Config
{
    class HisConfigCFG
    {
        private const string CONFIG_KEY__OBLIGATE_ICD = "EXE.ASSIGN_SERVICE_REQUEST__OBLIGATE_ICD";
        private const string CONFIG_KEY__PATIENT_TYPE_CODE__BHYT = "MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BHYT";//Doi tuong BHYT
        private const string CONFIG_KEY__PATIENT_TYPE_CODE__VP = "MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.HOSPITAL_FEE";//Doi tuong VP
        private const string CONFIG_KEY__ICD_GENERA_KEY = "HIS.Desktop.Plugins.AutoCheckIcd";
        private const string CONFIG_KEY__WARNING_OVER_TOTAL_PATIENT_PRICE = "HIS.Desktop.WarningOverTotalPatientPrice";
        private const string CONFIG_KEY__WARNING_OVER_TOTAL_PATIENT_PRICE__IS_CHECK = "HIS.Desktop.WarningOverTotalPatientPrice__IsCheck";
        private const string CONFIG_KEY__IS_DEFAULT_TRACKING = "HIS.Desktop.Plugins.AssignPrescription.IsDefaultTracking";
        private const string CONFIG_KEY__ShowRequestUser = "HIS.Desktop.Plugins.AssignConfig.ShowRequestUser";
        private const string CONFIG_KEY__USING_SERVER_TIME = "MOS.IS_USING_SERVER_TIME";
        private const string CONFIG_KEY__ALLOW_TO_ASSIGN = "HIS.Desktop.Plugins.HisAssignBlood.AllowToAssignDifferenceBloodOption";
        private const string CONFIG_KEY__SERVICE_REQ_ICD_OPTION = "HIS.HIS_TRACKING.SERVICE_REQ_ICD_OPTION";
        internal static bool IsServiceReqIcdOption;
        internal static string ObligateIcd;
        internal static string PatientTypeCode__BHYT;
        internal static long PatientTypeId__BHYT;
        internal static string PatientTypeCode__VP;
        internal static long PatientTypeId__VP;
        internal static string AutoCheckIcd;
        internal static string IsDefaultTracking;

        internal static string WarningOverTotalPatientPrice;
        internal static string WarningOverTotalPatientPrice__IsCheck;
        internal static string IsUsingServerTime;
        internal static bool IsAllowToAssignDifferenceBloodOption;
        /// <summary>
        /// Cấu hình để ẩn/hiện trường người chỉ định tai form chỉ định, kê đơn
        //- Giá trị mặc định (hoặc ko có cấu hình này) sẽ ẩn
        //- Nếu có cấu hình, đặt là 1 thì sẽ hiển thị
        /// </summary>
        internal static string ShowRequestUser;
        
        static MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE GetPatientTypeByCode(string code)
        {
            MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE result = new MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE();
            try
            {
                result = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>().FirstOrDefault(o => o.PATIENT_TYPE_CODE == code);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result ?? new MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE();
        }

        internal static void LoadConfig()
        {
            try
            {
                IsServiceReqIcdOption = GetValue(CONFIG_KEY__SERVICE_REQ_ICD_OPTION) == "1";
                ShowRequestUser = GetValue(CONFIG_KEY__ShowRequestUser);
                ObligateIcd = GetValue(CONFIG_KEY__OBLIGATE_ICD);
                PatientTypeCode__BHYT = GetValue(CONFIG_KEY__PATIENT_TYPE_CODE__BHYT);
                PatientTypeId__BHYT = GetPatientTypeByCode(PatientTypeCode__BHYT).ID;
                PatientTypeCode__VP = GetValue(CONFIG_KEY__PATIENT_TYPE_CODE__VP);
                PatientTypeId__VP = GetPatientTypeByCode(PatientTypeCode__VP).ID;
                AutoCheckIcd = GetValue(CONFIG_KEY__ICD_GENERA_KEY);
                WarningOverTotalPatientPrice = GetValue(CONFIG_KEY__WARNING_OVER_TOTAL_PATIENT_PRICE);
                WarningOverTotalPatientPrice__IsCheck = GetValue(CONFIG_KEY__WARNING_OVER_TOTAL_PATIENT_PRICE__IS_CHECK);
                IsDefaultTracking = GetValue(CONFIG_KEY__IS_DEFAULT_TRACKING);
                IsUsingServerTime = GetValue(CONFIG_KEY__USING_SERVER_TIME);
                IsAllowToAssignDifferenceBloodOption = GetValue(CONFIG_KEY__ALLOW_TO_ASSIGN) == "1";
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
