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
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ConnectionTest.Config
{
    class HisConfigCFG
    {
        private const string CONFIG_KEY__AUTO_RETURN_RESULT_BEFORE_PRINT = "HIS.Desktop.Plugins.ConnectionTest__AutoReturnResultBeforePrint";
        private const string CONFIG_KEY__RETURN_RESULT_WARNING_TIME = "HIS.Desktop.Plugins.ConnectionTest__ReturnResultWarningTime";

        private const string CONFIG_KEY__IS_USE_SIGN_EMR = "HIS.HIS.DESKTOP.IS_USE_SIGN_EMR";
        private const string CONFIG_KEY__TEST_SERVICE_GROUP_PRINT_RESULT = "HIS.DESKTOP.PRINT_TEST_RESULT.PARENT_SERVICE_CODE.GROUP";

        private const string CONFIG_KEY__IS_SHOWING_RESULT_GENERAL = "HIS.Desktop.Plugins.ConnectionTest.IsShowingResultGeneral";
        private const string CONFIG_KEY_CHECK_VALUE_MAXLENGTH_OPTION = "HIS.Desktop.Plugins.Test.CheckValueMaxlengthOption";

        private const string CONFIG_KEY__PRINT_TEST_RESULT_SPLIT_BY_TYPE = "HIS.Desktop.Plugins.ConnectionTest.PrintTestResultSplitByType";
        private const string CONFIG_KEY__AllowToEnterSampleTime = "HIS.Desktop.Plugins.ConnectionTest.AllowToEnterSampleTime";
        private const string CONFIG_KEY__ProcessTimeMustBeGreaterThanTotalProcessTime = "HIS.Desktop.Plugins.ProcessTimeMustBeLessThanMaxTotalProcessTime";
        private const string CONFIG_KEY__SignWarningOption = "HIS.Desktop.Plugins.ExecuteServiceReq.SignWarningOption";
        private const string CONFIG_KEY__PATIENT_TYPE_OPTION = "HIS.DESKTOP.HIS_MACHINE.MAX_SERVICE_PER_DAY.PATIENT_TYPE_OPTION";
        private const string CONFIG_KEY__MAX_SERVICE_PER_DAY_WARNING_OPTION = "HIS.DESKTOP.HIS_MACHINE.MAX_SERVICE_PER_DAY.WARNING_OPTION";
        private const string CONFIG_KEY__PATIENT_TYPE_CODE__BHYT = "MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BHYT";
        private const string CONFIG_KEY__AUTO_DELETE_EMR_DOCUMENT = "HIS.Desktop.Plugins.ServiceReqList.AutoDeleteEmrDocumentWhenEditReq";
        private const string CONFIG_KEY__IS_REQUIRED_SAMPLED = "HIS.Desktop.Plugins.ConnectionTest.IsRequiredSampled";
        private const string CONFIG_KEY__START_TIME_MUST_BE_GREATER_THAN_INSTRUCTION_TIME = "HIS.Desktop.Plugins.StartTimeMustBeGreaterThanInstructionTime";

        internal static string IS_USE_SIGN_EMR;
        internal static string AUTO_RETURN_RESULT_BEFORE_PRINT;
        internal static string WARNING_TIME_RETURN_RESULT;
        internal static string CHECK_VALUE_MAXLENGTH_OPTION;
        internal static List<long> PARENT_SERVICE_ID__GROUP_PRINT;

        internal static string IS_SHOWING_RESULT_GENERAL;
        internal static string ProcessTimeMustBeGreaterThanTotalProcessTime;
        internal static bool PRINT_TEST_RESULT;
        internal static string SignWarningOption;

        internal static string PatientTypeOption;
        internal static string MaxServicePerDayWarningOption;

        internal static bool AllowToEnterSampleTime;
        internal static long PatientTypeId__BHYT;
        internal static string AutoDeleteEmrDocumentWhenEditReq;

        internal static bool IsRequiredSampled;

        internal static string StartTimeMustBeGreaterThanInstructionTime;

        internal static void LoadConfig()
        {
            try
            {
                AutoDeleteEmrDocumentWhenEditReq = GetValue(CONFIG_KEY__AUTO_DELETE_EMR_DOCUMENT);
                IS_USE_SIGN_EMR = GetValue(CONFIG_KEY__IS_USE_SIGN_EMR);
                AUTO_RETURN_RESULT_BEFORE_PRINT = GetValue(CONFIG_KEY__AUTO_RETURN_RESULT_BEFORE_PRINT);
                WARNING_TIME_RETURN_RESULT = GetValue(CONFIG_KEY__RETURN_RESULT_WARNING_TIME);
                PARENT_SERVICE_ID__GROUP_PRINT = GetIds(CONFIG_KEY__TEST_SERVICE_GROUP_PRINT_RESULT);
                CHECK_VALUE_MAXLENGTH_OPTION = GetValue(CONFIG_KEY_CHECK_VALUE_MAXLENGTH_OPTION);
                IS_SHOWING_RESULT_GENERAL = GetValue(CONFIG_KEY__IS_SHOWING_RESULT_GENERAL);
                PRINT_TEST_RESULT = GetValue(CONFIG_KEY__PRINT_TEST_RESULT_SPLIT_BY_TYPE) == "1" ? true : false;
                AllowToEnterSampleTime = GetValue(CONFIG_KEY__AllowToEnterSampleTime) == "1" ? true : false;
                ProcessTimeMustBeGreaterThanTotalProcessTime = GetValue(CONFIG_KEY__ProcessTimeMustBeGreaterThanTotalProcessTime);
                SignWarningOption = GetValue(CONFIG_KEY__SignWarningOption);
                PatientTypeOption = GetValue(CONFIG_KEY__PATIENT_TYPE_OPTION);
                MaxServicePerDayWarningOption = GetValue(CONFIG_KEY__MAX_SERVICE_PER_DAY_WARNING_OPTION);
                PatientTypeId__BHYT = GetValueLong(CONFIG_KEY__PATIENT_TYPE_CODE__BHYT);
                IsRequiredSampled = GetValue(CONFIG_KEY__IS_REQUIRED_SAMPLED) == "1";
                StartTimeMustBeGreaterThanInstructionTime = GetValue(CONFIG_KEY__START_TIME_MUST_BE_GREATER_THAN_INSTRUCTION_TIME);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private static List<long> GetIds(string keyConfig)
        {
            List<long> rs = new List<long>();
            try
            {
                string valueCFG = GetValue(keyConfig);
                if (!String.IsNullOrWhiteSpace(valueCFG))
                {
                    List<string> serviceCodes = valueCFG.Split(',').ToList();
                    foreach (var code in serviceCodes)
                    {
                        if (String.IsNullOrWhiteSpace(code)) continue;
                        V_HIS_SERVICE s = BackendDataWorker.Get<V_HIS_SERVICE>().FirstOrDefault(o => o.SERVICE_CODE == code);
                        if (s != null)
                        {
                            rs.Add(s.ID);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                rs = new List<long>();
            }
            return rs;
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
                LogSystem.Error(ex);
                result = null;
            }
            return result;
        }
        private static long GetValueLong(string code)
        {
            long result = 0;
            try
            {
                long value = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<long>(code);
                result = value;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                result = 0;
            }
            return result;
        }

    }
}
