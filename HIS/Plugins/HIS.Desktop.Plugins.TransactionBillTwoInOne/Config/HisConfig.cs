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
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.Logging;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.TransactionBillTwoInOne.Config
{
    class HisConfig
    {
        private const string CONFIG_KEY__PATIENT_TYPE_CODE__BHYT = "MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BHYT";
        private const string MOS_CONFIG__PATIENT_TYPE_CODE__FEE = "MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.HOSPITAL_FEE";
        private const string MOS_CONFIG__PATIENT_TYPE_CODE__SERVICE = "MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.SERVICE";
        private const string HIS_FUND__FUND_CODE__HCM = "HIS.HIS_FUND.HIS_FUND_CODE.HCM";

        private const string VCN_ACCEPTED_SERVICE_CODE_CFG = "MOS.BHYT.HCM_POOR_FUND.VCN_ACCEPTED_SERVICE_CODE";
        private const string INTEGRATION_TYPE_CFG = "MOS.OLD_SYSTEM.INTEGRATION_TYPE";

        private const string His_Desktop_plugins_transactionBill_IsKetChuyen = "HIS.TRANSACTION.BILL.AUTO_CARRY_FORWARD";
        private const string His_Desktop_plugins_transactionBill_IsEditTransactionTime = "HIS.Desktop.Plugins.TransactionBill_Depo_Repa.IsEditTransactionTime";
        //cấu hình hồ sơ điều trị phải kết thúc mới cho thanh toán dịch vụ bhyt
        private const string HIS_BILL__MUST_FINISH_TREATMENT = "MOS.HIS_BILL.BHYT.MUST_FINISH_TREATMENT_BEFORE_BILLING";
        //private const string IsFinishBeforeBill = "1";
        private const string HIS_TRANSACTION_SAVE_AND_PRINT_NOW_SERVICE_DETAIL = "HIS.Desktop.Print.TransactionDetail_PrintNow";
        private const string CFG_TRANSACTION_BILL_TWO_BOOK__OPTION = "MOS.HIS_TRANSACTION.BILL_TWO_BOOK.OPTION";
        private const string CONFIG__IS_USING_PAYLATER = "MOS.HIS_TRANSACTION.IS_USING_PAYLATER";
        private const string HIS_IS_CHECK_AUTO_REPAY_AS_DEFAULT = "HIS.Desktop.Plugins.TransactionBill.IsCheckedAutoRepayAsDefault";

        private const string ELECTRONIC_BILL__PRINT_NUM_COPY = "CONFIG_KEY__HIS_DESKTOP__ELECTRONIC_BILL__PRINT_NUM_COPY";
        private const string PlatformOptionCFG = "Inventec.Common.DocumentViewer.PlatformOption";

        private const string ElectronicInvoicePublishingDelayTimeCFG = "HIS.Desktop.Plugins.TransactionBill.ElectronicInvoicePublishingDelayTime";

        internal static int E_BILL__PRINT_NUM_COPY;
        internal static int PlatformOption;

        internal enum BILL_OPTION
        {
            CTO_TW = 1,
            HCM_115 = 2,
            QBH_CUBA = 3,
        }

        internal static string PatientTypeCode__BHYT;
        internal static long PatientTypeId__BHYT;
        internal static string IsketChuyenCFG;
        internal static string IsEditTransactionBillCFG;
        internal static List<string> HcmPoorFund__Vcn;
        internal static bool TransactionDetail_PrintNow;
        internal static long HisFundId__Hcm;
        internal static List<long> VCN_ACCEPT_SERVICE_IDS;
        internal const short IS_TRUE = 1;
        internal static bool IsUsingPaylater;

        /// <summary>
        /// 1 - chỉ với đối tượng BHYT
        /// 2 - Tất cả các đối tượng
        /// </summary>
        internal static string MustFinishTreatmentForBill;
        internal static long PATIENT_TYPE_ID__IS_FEE;
        internal static long PATIENT_TYPE_ID__SERVICE;
        internal static int BILL_TWO_BOOK__OPTION;
        internal static string OLD_SYSTEM__OPTION;
        internal static bool IsCheckAutoRepayAsDefault;
        internal static decimal ElectronicInvoicePublishingDelayTime;

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
                LogSystem.Debug("LoadConfig => 1");
                TransactionDetail_PrintNow = Get(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(HIS_TRANSACTION_SAVE_AND_PRINT_NOW_SERVICE_DETAIL));
                Inventec.Common.Logging.LogSystem.Error("HIS_TRANSACTION_SAVE_AND_PRINT_NOW_SERVICE_DETAIL>>>>>>>>>>>>>>" + HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(HIS_TRANSACTION_SAVE_AND_PRINT_NOW_SERVICE_DETAIL));

                PatientTypeCode__BHYT = GetValue(CONFIG_KEY__PATIENT_TYPE_CODE__BHYT);
                BILL_TWO_BOOK__OPTION = HisConfigs.Get<int>(CFG_TRANSACTION_BILL_TWO_BOOK__OPTION);
                OLD_SYSTEM__OPTION = GetValue(INTEGRATION_TYPE_CFG);

                HcmPoorFund__Vcn = GetListValue(VCN_ACCEPTED_SERVICE_CODE_CFG);
                IsketChuyenCFG = GetValue(His_Desktop_plugins_transactionBill_IsKetChuyen);
                IsEditTransactionBillCFG = GetValue(His_Desktop_plugins_transactionBill_IsEditTransactionTime) ;
                MustFinishTreatmentForBill = GetValue(HIS_BILL__MUST_FINISH_TREATMENT);
                HisFundId__Hcm = GetFundId(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(HIS_FUND__FUND_CODE__HCM));
                VCN_ACCEPT_SERVICE_IDS = GetServiceIds(VCN_ACCEPTED_SERVICE_CODE_CFG);
                PatientTypeId__BHYT = GetPatientTypeByCode(PatientTypeCode__BHYT).ID;
                PATIENT_TYPE_ID__IS_FEE = GetPatientTypeByCode(GetValue(MOS_CONFIG__PATIENT_TYPE_CODE__FEE)).ID;
                PATIENT_TYPE_ID__SERVICE = GetPatientTypeByCode(GetValue(MOS_CONFIG__PATIENT_TYPE_CODE__SERVICE)).ID;
                IsUsingPaylater = GetValue(CONFIG__IS_USING_PAYLATER) == "1";
                IsCheckAutoRepayAsDefault = GetValue(HIS_IS_CHECK_AUTO_REPAY_AS_DEFAULT) == "1";

                E_BILL__PRINT_NUM_COPY = HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplicationWorker.Get<int>(ELECTRONIC_BILL__PRINT_NUM_COPY);
                PlatformOption = HisConfigs.Get<int>(PlatformOptionCFG);

                string delayTime = HisConfigs.Get<string>(ElectronicInvoicePublishingDelayTimeCFG) ?? "0";
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

        private static long GetFundId(string fundCode)
        {
            long result = 0;
            try
            {
                var fund = BackendDataWorker.Get<HIS_FUND>().FirstOrDefault(o => o.FUND_CODE == fundCode);
                if (fund != null)
                {
                    result = fund.ID;
                }
                if (result == 0) throw new NullReferenceException(fundCode);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = 0;
            }
            return result;
        }

        private static List<long> GetServiceIds(string code)
        {
            List<long> result = null;
            try
            {
                result = new List<long>();
                List<string> data = HisConfigs.Get<List<string>>(code);
                foreach (string t in data)
                {
                    string[] tmp = t.Split(':');
                    if (tmp != null && tmp.Length >= 2)
                    {
                        V_HIS_SERVICE service = BackendDataWorker.Get<V_HIS_SERVICE>().Where(o => o.SERVICE_TYPE_CODE == tmp[0] && o.SERVICE_CODE == tmp[1]).FirstOrDefault();
                        if (service != null)
                        {
                            result.Add(service.ID);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
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
