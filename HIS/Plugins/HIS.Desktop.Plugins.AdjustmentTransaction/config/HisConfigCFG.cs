using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.HisConfig;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.AdjustmentTransaction.config
{
    internal class HisConfigCFG
    {
        private const string IsFinishBeforeBill = "1";
        private const string CONFIG_KEY__PATIENT_TYPE_CODE__BHYT = "MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BHYT";
        private const string HIS_BILL__MUST_FINISH_TREATMENT = "MOS.HIS_BILL.BHYT.MUST_FINISH_TREATMENT_BEFORE_BILLING";
        private const string HIS_Desktop_ShowServerTimeByDefault = "HIS.Desktop.ShowServerTimeByDefault";
        private const string ALLOW_TO_CREATE_NO_PRICE_TRANSACTION = "HIS.Desktop.Plugins.TransactionBill.AllowToCreateNoPriceTransaction";
        private const string ELECTRONIC_BILL__PRINT_NUM_COPY = "CONFIG_KEY__HIS_DESKTOP__ELECTRONIC_BILL__PRINT_NUM_COPY";
        private const string PlatformOptionCFG = "Inventec.Common.DocumentViewer.PlatformOption";
        private const string CONFIG_KEY_AttachAssignPrintWarningOption = "HIS.Desktop.Plugins.TransactionBill.AttachAssignPrintWarningOption";
        private const string HIS_TRANSACTION_SAVE_AND_PRINT_NOW_SERVICE_DETAIL = "HIS.Desktop.Print.TransactionDetail_PrintNow";
        private const string ENABLE_SAVE_OPTION = "HIS.Desktop.Plugins.TransactionBill.EnableSaveOption";
        private const string ElectronicInvoicePublishingDelayTimeCFG = "HIS.Desktop.Plugins.TransactionBill.ElectronicInvoicePublishingDelayTime";
        private const string CONFIG_KEY__PATIENT_TYPE_CODE__VP = "MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.HOSPITAL_FEE";//Doi tuong 
        private const string AutoSelectAccountBookIfHasOne = "HIS.Desktop.Plugins.TransactionBill.AutoSelectAccountBookIfHasOne";


        internal static string PatientTypeCode__BHYT;
        internal static long PatientTypeId__BHYT;
        internal static string MustFinishTreatmentForBill;
        internal static string ShowServerTimeByDefault;
        internal static string AllowToCreateNoPriceTransaction;
        internal static int E_BILL__PRINT_NUM_COPY;
        internal static int PlatformOption;
        internal static string AttachAssignPrintWarningOption;
        internal static bool TransactionDetail_PrintNow;
        internal static string EnableSaveOption;
        internal static decimal ElectronicInvoicePublishingDelayTime;
        internal static string PatientTypeCode__VP;
        internal static long PatientTypeId__VP;
        internal static bool IsAutoSelectAccountBookIfHasOne;

        internal static void LoadConfig()
        {
            try
            {
                PatientTypeCode__BHYT = GetValue(CONFIG_KEY__PATIENT_TYPE_CODE__BHYT);
                PatientTypeId__BHYT = GetPatientTypeByCode(PatientTypeCode__BHYT).ID;
                MustFinishTreatmentForBill = GetValue(HIS_BILL__MUST_FINISH_TREATMENT);
                ShowServerTimeByDefault = GetValue(HIS_Desktop_ShowServerTimeByDefault);
                AllowToCreateNoPriceTransaction = GetValue(ALLOW_TO_CREATE_NO_PRICE_TRANSACTION);
                E_BILL__PRINT_NUM_COPY = LocalStorage.ConfigApplication.ConfigApplicationWorker.Get<int>(ELECTRONIC_BILL__PRINT_NUM_COPY);
                PlatformOption = HisConfigs.Get<int>(PlatformOptionCFG);
                AttachAssignPrintWarningOption = GetValue(CONFIG_KEY_AttachAssignPrintWarningOption);
                TransactionDetail_PrintNow = Get(LocalStorage.HisConfig.HisConfigs.Get<string>(HIS_TRANSACTION_SAVE_AND_PRINT_NOW_SERVICE_DETAIL));
                EnableSaveOption = GetValue(ENABLE_SAVE_OPTION);
                string delayTime = HisConfigs.Get<string>(ElectronicInvoicePublishingDelayTimeCFG);
                ElectronicInvoicePublishingDelayTime = Decimal.Parse(delayTime, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);
                PatientTypeCode__VP = GetValue(CONFIG_KEY__PATIENT_TYPE_CODE__VP);
                PatientTypeId__VP = GetPatientTypeByCode(PatientTypeCode__VP).ID;
                IsAutoSelectAccountBookIfHasOne = GetValue(AutoSelectAccountBookIfHasOne) == "1";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

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
