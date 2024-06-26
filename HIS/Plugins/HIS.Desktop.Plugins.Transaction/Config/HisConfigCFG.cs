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

namespace HIS.Desktop.Plugins.Transaction.Config
{
    internal class HisConfigCFG
    {

        private const string IsNotBillString = "HIS.Desktop.Plugins.TransactionRepay.IsNotBill";
        private const string CONFIG_KEY__PATIENT_TYPE_CODE__BHYT = "MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BHYT";
        private const string CONFIG_KEY__PATIENT_TYPE_CODE__VP = "MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.HOSPITAL_FEE";//Doi tuong VP
        private const string CONFIG_KEY__PATIENT_TYPE_CODE__KSK = "MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.KSK";//Doi tuong khám sức khỏe
        private const string CONFIG_KEY__ALLOWAFTERLOCKING = "HIS.Desktop.Plugins.TransactionRepay.AllowAfterLocking";

        private const string His_Desktop_plugins_transactionBill_IsKetChuyen = "HIS.TRANSACTION.BILL.AUTO_CARRY_FORWARD";
        private const string CONFIG_KEY__MUST_FINISHED_FOR_BILLING = "MOS.HIS_BILL.BHYT.MUST_FINISH_TREATMENT_BEFORE_BILLING";
        private const string CONFIG_KEY__DIRECTLY_BILLING_OPTION = "MOS.HIS_TRANSACTION.DIRECTLY_BILLING_OPTION";
        private const string CONFIG_KEY__UNLOCK_FEE_OPTION = "HIS.DESKTOP.HIS_TREATMENT.UNLOCK_FEE_OPTION";
        private const string CONFIG_KEY__CALL_PATIENT_FORMAT = "HIS.TRANSACTION_ROOM.CALL_PATIENT_FORMAT";

        internal static string CallPatientFormat;
        internal static string PatientTypeCode__BHYT;
        internal static long PatientTypeId__BHYT;

        internal static string PatientTypeCode__VP;
        internal static long PatientTypeId__VP;

        internal static string PatientTypeCode__KSK;
        internal static long PatientTypeId__KSK;

        internal static string IsNotBillCFG;

        internal static bool IsAllowAfterLocking;
        internal static string MustFinishedForBilling;

        internal static string IsketChuyenCFG;

        internal static string DirectlyBillingOption;

        internal static string IsSplitTotalReceivePrice
        {
            get
            {
                return GetValue("HIS.Desktop.Plugins.Transaction.IsSplitTotalReceivePrice");
            }
        }

        internal static string UNLOCK_FEE_OPTION;

        internal static void LoadConfig()
        {
            try
            {
                LogSystem.Debug("LoadConfig => 1");
                CallPatientFormat = GetValue(CONFIG_KEY__CALL_PATIENT_FORMAT);
                IsNotBillCFG = GetValue(IsNotBillString);
                DirectlyBillingOption = GetValue(CONFIG_KEY__DIRECTLY_BILLING_OPTION);
                IsAllowAfterLocking = GetValue(CONFIG_KEY__ALLOWAFTERLOCKING) == "1";
                PatientTypeCode__BHYT = GetValue(CONFIG_KEY__PATIENT_TYPE_CODE__BHYT);
                PatientTypeId__BHYT = GetPatientTypeByCode(PatientTypeCode__BHYT).ID;
                PatientTypeCode__KSK = GetValue(CONFIG_KEY__PATIENT_TYPE_CODE__KSK);
                PatientTypeId__KSK = GetPatientTypeByCode(PatientTypeCode__KSK).ID;
                PatientTypeCode__VP = GetValue(CONFIG_KEY__PATIENT_TYPE_CODE__VP);
                PatientTypeId__VP = GetPatientTypeByCode(PatientTypeCode__VP).ID;
                IsketChuyenCFG = GetValue(His_Desktop_plugins_transactionBill_IsKetChuyen);
                MustFinishedForBilling = GetValue(CONFIG_KEY__MUST_FINISHED_FOR_BILLING);
                UNLOCK_FEE_OPTION = GetValue(CONFIG_KEY__UNLOCK_FEE_OPTION);
                LogSystem.Debug("LoadConfig => 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

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

        private static List<string> GetListValue(string key)
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
