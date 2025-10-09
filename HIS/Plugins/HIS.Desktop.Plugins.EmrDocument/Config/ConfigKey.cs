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
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.EmrDocument.Config
{
    class ConfigKey
    {
        private const string CONFIG_KEY__EMR_PATIENT_SIGN_OPTION = "EMR.EMR_DOCUMENT.PATIENT_SIGN.OPTION";
        private const string IS_HAS_CONNECTION_EMR = "MOS.HAS_CONNECTION_EMR";
        private const string IS_STORED_MUST_REQ_TO_VIEW = "EMR.EMR_TREATMENT.STORED_MUST_REQ_TO_VIEW";
        private const string IS_DELETE_FILE_OPTION = "EMR.EMR_DOCUMENT.DELETE_FILE_OPTION";
        private const string DO_NOT_ALLOW_DELETING_IF_EXIST_SERVICE_REQ = "EMR.EMR_DOCUMENT.DO_NOT_ALLOW_DELETING_IF_EXIST_SERVICE_REQ"; 
        private const string PRINT_USING_WATERMARK = "EMR.DOCUMENT.PRINT_USING_WARTERMARK.OPTION";

        internal static bool IsStoredMustReqToView;
        internal static bool IsHasConnectionEmr;
        internal static string patientSignOption;
        internal static string deleteFileOption;
        internal static string DoNotAllowDeletingIfExistServiceReq;
        internal static string PrintUsingWatermark;

        internal static void GetConfigKey()
        {
            try
            {
                IsStoredMustReqToView = GetValue(IS_STORED_MUST_REQ_TO_VIEW) == "1";
                IsHasConnectionEmr = GetValueHis(IS_HAS_CONNECTION_EMR) == "1";
                patientSignOption = GetValue(CONFIG_KEY__EMR_PATIENT_SIGN_OPTION);
                deleteFileOption = GetValue(IS_DELETE_FILE_OPTION);
                DoNotAllowDeletingIfExistServiceReq = GetValue(DO_NOT_ALLOW_DELETING_IF_EXIST_SERVICE_REQ);
                PrintUsingWatermark = GetValue(PRINT_USING_WATERMARK);
                Inventec.Common.Logging.LogSystem.Debug(patientSignOption);
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
                return HIS.Desktop.LocalStorage.EmrConfig.EmrConfigs.Get<string>(code);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                result = null;
            }
            return result;
        }
        private static string GetValueHis(string code)
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
    }
}
