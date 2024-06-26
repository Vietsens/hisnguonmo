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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.TransactionBill.Config
{
    internal class HisConfigCFG
    {
        private const string CONFIG_KEY__PATIENT_TYPE_CODE__BHYT = "MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BHYT";
        private const string CONFIG_KEY__PATIENT_TYPE_CODE__VP = "MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.HOSPITAL_FEE";//Doi tuong 

        public const string HIS_FUND__FUND_CODE__HCM = "HIS.HIS_FUND.HIS_FUND_CODE.HCM";

        private const string VCN_ACCEPTED_SERVICE_CODE_CFG = "MOS.BHYT.HCM_POOR_FUND.VCN_ACCEPTED_SERVICE_CODE";

        private const string His_Desktop_plugins_transactionBill_IsKetChuyen = "HIS.TRANSACTION.BILL.AUTO_CARRY_FORWARD";
        private const string His_Desktop_plugins_transactionBill_IsEditTransactionTime = "HIS.Desktop.Plugins.TransactionBill_Depo_Repa.IsEditTransactionTime";
        //cấu hình hồ sơ điều trị phải kết thúc mới cho thanh toán dịch vụ bhyt
        private const string HIS_BILL__MUST_FINISH_TREATMENT = "MOS.HIS_BILL.BHYT.MUST_FINISH_TREATMENT_BEFORE_BILLING";
        private const string HIS_IS_CHECK_AUTO_REPAY_AS_DEFAULT = "HIS.Desktop.Plugins.TransactionBill.IsCheckedAutoRepayAsDefault";
        private const string IsFinishBeforeBill = "1";
        private const string HIS_TRANSACTION_SAVE_AND_PRINT_NOW_SERVICE_DETAIL = "HIS.Desktop.Print.TransactionDetail_PrintNow";
        private const string AutoSelectAccountBookIfHasOne = "HIS.Desktop.Plugins.TransactionBill.AutoSelectAccountBookIfHasOne";
        private const string CONFIG_KEY_AttachAssignPrintWarningOption = "HIS.Desktop.Plugins.TransactionBill.AttachAssignPrintWarningOption";

        private const string ELECTRONIC_BILL__PRINT_NUM_COPY = "CONFIG_KEY__HIS_DESKTOP__ELECTRONIC_BILL__PRINT_NUM_COPY";
        private const string PlatformOptionCFG = "Inventec.Common.DocumentViewer.PlatformOption";

        private const string ENABLE_SAVE_OPTION = "HIS.Desktop.Plugins.TransactionBill.EnableSaveOption";

        private const string ALLOW_TO_CREATE_NO_PRICE_TRANSACTION = "HIS.Desktop.Plugins.TransactionBill.AllowToCreateNoPriceTransaction";
        private const string ElectronicInvoicePublishingDelayTimeCFG = "HIS.Desktop.Plugins.TransactionBill.ElectronicInvoicePublishingDelayTime";

        private const string HIS_Desktop_ShowServerTimeByDefault = "HIS.Desktop.ShowServerTimeByDefault";
        private const string HIS_Desktop_AutoCreateDepositTransaction = "HIS.Desktop.Plugins.TransactionBill.AutoCreateDepositTransaction";

        internal static bool AutoCreateDepositTransaction;
        internal static string PatientTypeCode__BHYT;
        internal static long PatientTypeId__BHYT;
        internal static string IsketChuyenCFG;
        internal static string IsEditTransactionBillCFG;
        internal static List<string> HcmPoorFund__Vcn;
        internal static bool TransactionDetail_PrintNow;
        internal static string PatientTypeCode__VP;
        internal static long PatientTypeId__VP;
        internal static bool IsCheckAutoRepayAsDefault;
        internal static bool IsAutoSelectAccountBookIfHasOne;
        internal static string AttachAssignPrintWarningOption;

        internal static string MustFinishTreatmentForBill;
        internal static string EnableSaveOption;
        internal static int E_BILL__PRINT_NUM_COPY;
        internal static int PlatformOption;
        internal static string AllowToCreateNoPriceTransaction;
        internal static decimal ElectronicInvoicePublishingDelayTime;

        internal static string ShowServerTimeByDefault;
        static bool Get(string code)
        {
            bool result = false;
            try
            {
                if (!String.IsNullOrEmpty(code))
                {
                    result = (code == IsFinishBeforeBill);
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
                LogSystem.Debug("LoadConfig => 1");
                AutoCreateDepositTransaction = GetValue(HIS_Desktop_AutoCreateDepositTransaction) == "1";
                TransactionDetail_PrintNow = Get(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(HIS_TRANSACTION_SAVE_AND_PRINT_NOW_SERVICE_DETAIL));
                Inventec.Common.Logging.LogSystem.Debug("HIS_TRANSACTION_SAVE_AND_PRINT_NOW_SERVICE_DETAIL>>>>>>>>>>>>>>" + HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(HIS_TRANSACTION_SAVE_AND_PRINT_NOW_SERVICE_DETAIL));
                EnableSaveOption = GetValue(ENABLE_SAVE_OPTION);
                PatientTypeCode__BHYT = GetValue(CONFIG_KEY__PATIENT_TYPE_CODE__BHYT);
                PatientTypeId__BHYT = GetPatientTypeByCode(PatientTypeCode__BHYT).ID;
                HcmPoorFund__Vcn = GetListValue(VCN_ACCEPTED_SERVICE_CODE_CFG);
                IsketChuyenCFG = GetValue(His_Desktop_plugins_transactionBill_IsKetChuyen);
                IsEditTransactionBillCFG = GetValue(His_Desktop_plugins_transactionBill_IsEditTransactionTime);
                MustFinishTreatmentForBill = GetValue(HIS_BILL__MUST_FINISH_TREATMENT);
                PatientTypeCode__VP = GetValue(CONFIG_KEY__PATIENT_TYPE_CODE__VP);
                PatientTypeId__VP = GetPatientTypeByCode(PatientTypeCode__VP).ID;
                IsCheckAutoRepayAsDefault = GetValue(HIS_IS_CHECK_AUTO_REPAY_AS_DEFAULT) == "1";
                IsAutoSelectAccountBookIfHasOne = GetValue(AutoSelectAccountBookIfHasOne) == "1";
                AttachAssignPrintWarningOption = GetValue(CONFIG_KEY_AttachAssignPrintWarningOption);
                E_BILL__PRINT_NUM_COPY = HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplicationWorker.Get<int>(ELECTRONIC_BILL__PRINT_NUM_COPY);
                PlatformOption = HisConfigs.Get<int>(PlatformOptionCFG);
                AllowToCreateNoPriceTransaction = GetValue(ALLOW_TO_CREATE_NO_PRICE_TRANSACTION);
                ShowServerTimeByDefault = GetValue(HIS_Desktop_ShowServerTimeByDefault);

                string delayTime = HisConfigs.Get<string>(ElectronicInvoicePublishingDelayTimeCFG);
                ElectronicInvoicePublishingDelayTime = Decimal.Parse(delayTime, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);
                LogSystem.Debug("LoadConfig => 2");
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

    }
}
