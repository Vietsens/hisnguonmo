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
        private const string HEIN_PATIENT_TYPE_CODE_LOOKUP_OPTION = "HIS.Desktop.Plugins.Hospitalize.HeinPatientTypeCodeLookupOption";
        internal static string CheckIcdWhenSave;
        /// <summary>
        /// Key = 1 hoac 2: ma doi tuong KCB tren form nhap vien tra danh muc CHI theo o Cap cuu + Dien dieu tri
        /// + ma CSKCB ban dau (bo qua Tuyen va Truong hop cua ho so). Khac: tra nhu cu (tuyen + truong hop ho so trung khit).
        /// </summary>
        internal static bool IsHeinPatientTypeCodeLookup;
        /// <summary>Key = 1: khi Luu, ma tren form khac ma ho so thi hoi xac nhan. Key = 2 hoac khac: khong hoi.</summary>
        internal static bool IsConfirmChangeHeinPatientTypeCode;
        public static bool IsUsingServerTime;
        public static string RelativesInforOption;
        public static bool IsInHospitalizationReasonRequired;
        public static void LoadConfig()
        {
            CheckIcdWhenSave = GetValue(CHECK_ICD_WHEN_SAVE);
            IsUsingServerTime = GetValue(IS_USING_SERVER_TIME) == GlobalVariables.CommonStringTrue;
            RelativesInforOption = GetValue(RELATIVES_INFOR);
            IsInHospitalizationReasonRequired = GetValue(IS_IN_HOSPITALIZATION_REASON_REQUIRED) == GlobalVariables.CommonStringTrue;
            string lookupOption = (GetValue(HEIN_PATIENT_TYPE_CODE_LOOKUP_OPTION) ?? "").Trim();
            IsHeinPatientTypeCodeLookup = lookupOption == "1" || lookupOption == "2";
            IsConfirmChangeHeinPatientTypeCode = lookupOption == "1";
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
