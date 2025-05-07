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
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.Logging;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.AssignPrescriptionCLS.Config
{
    class HisConfigCFG
    {
        private const string CONFIG_KEY__MOS_HIS_SERVICE_REQ_MANY_DAYS_PRESCRIPTION_OPTION = "MOS.HIS_SERVICE_REQ.MANY_DAYS_PRESCRIPTION_OPTION";
        private const string CONFIG_KEY__IsAllowAssignPresByPackage = "HIS.Desktop.Plugins.IsAllowAssignPresByPackage";
        private const string CONFIG_KEY__IsloadIcdFromExamServiceExecute = "HIS.Desktop.Plugins.IsloadIcdFromExamServiceExecute";
        private const string CONFIG_KEY__ShowRoundAvailableAmount = "His.Desktop.InPatientPrescription.ShowRoundAvailableAmount";
        private const string CONFIG_KEY__IS_AUTO_CREATE_SALE_EXP_MEST = "MOS.HIS_SERVICE_REQ.IS_AUTO_CREATE_SALE_EXP_MEST";
        private const string CONFIG_KEY__IsDefaultFocusMedicineTabPage = "HIS.Desktop.Plugins.AssignPrescription.IsDefaultFocusMedicineTabPage";
        private const string CONFIG_KEY__ShowRequestUser = "HIS.Desktop.Plugins.AssignConfig.ShowRequestUser";
        private const string CONFIG_KEY__PATIENT_TYPE_CODE__BHYT = "MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BHYT";//Doi tuong BHYT     
        private const string CONFIG_KEY__PATIENT_TYPE_CODE__VP = "MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.HOSPITAL_FEE";//Doi tuong VP
        private const string LIMIT_HEIN_MEDICINE_PRICE__RIGHT_MEDI_ORG = "MOS.BHYT.LIMIT_HEIN_MEDICINE_PRICE.RIGHT_MEDI_ORG";
        private const string LIMIT_HEIN_MEDICINE_PRICE__TRAN_PATI = "MOS.BHYT.LIMIT_HEIN_MEDICINE_PRICE.TRAN_PATI";
        private const string CONFIG_KEY__ICD_GENERA_KEY = "HIS.Desktop.Plugins.AutoCheckIcd";
        private const string CONFIG_KEY__LIMIT_HEIN_MEDICINE_PRICE__RIGHT_MEDI_ORG = "MOS.BHYT.LIMIT_HEIN_MEDICINE_PRICE.RIGHT_MEDI_ORG";
        private const string CONFIG_KEY__LIMIT_HEIN_MEDICINE_PRICE__NOT_RIGHT_MEDI_ORG = "MOS.BHYT.LIMIT_HEIN_MEDICINE_PRICE.NOT_RIGHT_MEDI_ORG";
        private const string LOAD_PATIENT_TYPE_DEFAULT_KEY = "EXE.ASSING_SERVICE_MERGER.LOAD_PATIENT_TYPE_DEFAULT";
        private const string VIEW_MEDICINE_MATERIAL_OUT_OF_STOCK_KEY = "EXE.ASSING_SERVICE_MERGER.VIEW_MEDICINE_MATERIAL_OUT_OF_STOCK";
        private const string IS_VISILBE_TEMPLATE_MEDICINE_KEY = "EXE.ASSING_SERVICE_MERGER.IS_VISILBE_TEMPLATE_MEDICINE";
        private const string IS_VISILBE_EXECUTE_GROUP_KEY = "HIS.Desktop.Plugins.Assign.IsExecuteGroup";
        private const string CONFIG_KEY__OBLIGATE_ICD = "EXE.ASSIGN_SERVICE_REQUEST__OBLIGATE_ICD";
        private const string Key__IsAllowPrintFinish = "HIS.Desktop.AllowPrint.Finish";
        private const string Key__AcinInteractive__Grade = "HIS.Desktop.Plugins.AssignPrescription.AcinInteractive__Grade";

        private const string TREATMENT_END___APPOINTMENT_TIME_DEFAULT_KEY = "EXE.HIS_TREATMENT_END.APPOINTMENT_TIME_DEFAULT";
        private const string PRESCRIPTION_TIME_AND_APPOINTMENT_TIME_KEY = "HIS.Desktop.Plugins.TreatmentFinish.APPOINTMENT_TIME";
        private const string CONFIG_KEY__TREATMENT_FINISH__CHECK_FINISH_TIME = "HIS.DESKTOP.TREATMENT_FINISH.CHECK_FINISH_TIME";
        private const string CHECK_ASSIGN_SERVICE_BED = "HIS.DESKTOP.TREATMENT_FINISH.CHECK_ASSIGN_SERVICE_BED";
        private const string MUST_FINISH_ALL_SERVICES_BEFORE_FINISH_TREATMENT_CFG = "MOS.HIS_TREATMENT.MUST_FINISH_ALL_SERVICES_BEFORE_FINISH_TREATMENT";
        private const string SERVICE_CODE__AUTO_FINISH = "MOS.HIS_TREATMENT.AUTO_FINISH_SERVICE_REQ.SERVICE_CODE";
        private const string CONFIG_KEY____ISAUTO_CLOSE_FROM_WITH_AUTO_TREATMENT_FINISH = "HIS.Desktop.Plugins.AssignPrescription.IsAutoCloseFormWithAutoConfigTreatmentFinish";
        private const string Key__WarningOverCeiling__Exam__Out__In = "HIS.Desktop.Plugins.WarningOverCeiling.Exam__Out__In";
        public const string ONLY_DISPLAY_MEDIMATE_IS_BUSINESS = "HIS.Desktop.Plugins.AssignPrescription.OnlyDisplayMediMateIsBusiness";

        public const string ICD_SERVICE__HAS_CHECK = "HIS.HIS_ICD_SERVICE.HAS_CHECK";
        public const string ICD_SERVICE__HAS_REQUIRE_CHECK = "HIS.HIS_ICD_SERVICE.HAS_REQUIRE_CHECK";
        public const string HIS_ICD_SERVICE__ALLOW_UPDATE = "HIS.HIS_ICD_SERVICE.ALLOW_UPDATE";

        //qtcode
        private const string IS_ALLOW_SIGN_NATURE_PRINT = "HIS.Desktop.Plugins.IsAllowSignaturePrint.ModuleLinks";
        internal static string AllowSignaturePrintModuleLinks;
        //qtcode

        internal const string SAVE_PRINT_MPS_DEFAULT = "HIS.Desktop.Plugins.Library.PrintPrescription.Mps";
        internal const string CHECK_SAME_HEIN_KEY = "HIS.DESKTOP.TREATMENT_FINISH.CHECK_SAME_HEIN";
        internal const string TUTORIAL_NUMBER_IS_FRAC = "HIS.Desktop.Plugins.AssignPrescription.TutorialNumberIsFrac";
        internal const string WARNING_OVER_TOTAL_PATIENT_PRICE__IS_CHECK = "HIS.Desktop.WarningOverTotalPatientPrice__IsCheck";
        internal const string WARNING_OVER_TOTAL_PATIENT_PRICE = "HIS.Desktop.WarningOverTotalPatientPrice";
        internal const string WARRING_USE_DAY_AND_EXP_TIME_BHYT = "HIS.Desktop.Plugins.AssignPrescription.IsWarringUseDayAndExpTimeBHYT";
        internal const string MEDICINE_HAS_PAYMENT_LIMIT_BHYT = "HIS.Desktop.Plugins.AssignPrescription.MedicineHasPaymentLimitBHYT";
        internal const string WARRING_INTRUCTION_USE_DAY_NUM = "HIS.Desktop.Plugins.AssignPrescription.WarringIntructionUseDayNum";
        internal const string KEY_IS_DEFAULT_TRACKING = "HIS.Desktop.Plugins.AssignPrescription.IsDefaultTracking";
        internal const string TUTORIAL_FORMAT = "HIS.Desktop.Plugins.AssignPrescription.TutorialFormat";
        internal const string MOS__PRESCRIPTION_SPLIT_OUT_MEDISTOCK = "MOS.HIS_SERVICE_REQ.PRESCRIPTION_SPLIT_OUT_MEDISTOCK";
        private const string CONFIG_KEY__IsBlockWhileAcinByMedicineType = "HIS.Desktop.Plugins.AssignPrescription.IsBlockWhileAcinByMedicineType";
        private const string MOS__HIS_SERVICE_REQ__IS_NOT_ALLOWING_EXPEND_WITHOUT_HAVING_PARENT = "MOS.HIS_SERVICE_REQ.IS_NOT_ALLOWING_EXPEND_WITHOUT_HAVING_PARENT";
        internal const string IS_USING_SERVER_TIME = "MOS.IS_USING_SERVER_TIME";
        private const string CONFIG_KEY__IsAutoTickExpendWithAssignPresPTTT = "HIS.Desktop.Plugins.AssignPrescription.IsAutoTickExpendWithAssignPresPTTT";
        private const string CONFIG_KEY__IsUsingWarningHeinFee = "His.Desktop.IsUsingWarningHeinFee";
        internal const string IS_CHOOSE_DRUGSTORE = "HIS.Desktop.Plugins.AssignPrescription.DefaultDrugStoreCode"; // co chọn nhà thuoc hay khong//Se bo sau
        internal const string CONFIG_KEY__DONT_PRES_EXPIRED_ITEM = "MOS.HIS_MEDI_STOCK.DONT_PRES_EXPIRED_ITEM";
        internal const string CONFIG_KEY__WARNING_ODD_CONVERT_AMOUNT = "HIS.Desktop.Plugins.AssignPrescription.IsWarningOddConvertAmount";

        public const string CONFIG_KEY__IS_REASON_REQUIRED = "MOS.EXP_MEST.IS_REASON_REQUIRED";
        public const string CONFIG_KEY__DEFAULT_TYPE_OPTION = "HIS.Desktop.Plugins.Assign.DefaultPatientTypeOption";
        private const string CONFIG_KEY__PRESCRIPTION_ATC_CODE_OVERLAP_WARNING_OPTION = "HIS.DESKTOP.PRESCRIPTION.ATC_CODE_OVERLAP.WARNING_OPTION";
        private const string CONFIG_KEY__IS_TRACKING_REQUIRED = "MOS.HIS_SERVICE_REQ.PRESCRIPTION.IS_TRACKING_REQUIRED";
        internal static bool IsTrackingRequired;
        internal static bool IsDefaultPatientTypeOption;
        internal static bool IsWarningOddConvertAmount;  
        internal static bool IsDontPresExpiredTime;  
        internal static string DefaultDrugStoreCode;//Se bo sau
        internal static string IsUsingWarningHeinFee;  
        internal static bool IsAutoCreateSaleExpMest;
        internal static bool IsAutoTickExpendWithAssignPresPTTT;
        internal static bool IsNotAllowingExpendWithoutHavingParent;
        public static decimal WarningOverCeiling__Exam { get; set; }
        public static decimal WarningOverCeiling__Out { get; set; }
        public static decimal WarningOverCeiling__In { get; set; }
        internal static string ShowRequestUser;
        internal static string PatientTypeName__BHYT;
        internal static string PatientTypeCode__BHYT;
        internal static long PatientTypeId__BHYT;
        internal static string PatientTypeName__VP;
        internal static string PatientTypeCode__VP;
        internal static long PatientTypeId__VP;
        internal static string IsDefaultFocusMedicineTabPage;
        internal static string AutoCheckIcd;
        internal static decimal LimitHeinMedicinePrice__RightMediOrg;
        internal static decimal LimitHeinMedicinePrice__NotRightMediOrg;
        internal static string IsVisilbleExecuteGroup;
        internal static string LoadPatientTypeDefault;
        internal static string ViewMedicineMaterialOutOfStock;
        internal static string IsVisilbleTemplateMedicine;
        internal static long? AcinInteractive__Grade;
        internal static string TreatmentTypeCode__Exam;
        internal static string TreatmentTypeCode__TreatIn;
        internal static string TreatmentTypeCode__TreatOut;
        internal static string ObligateIcd;
        internal static bool IsAutoCloseFormWithAutoConfigTreatmentFinish;
        internal static bool IsAllowPrintFinish;
        internal static bool IsTutorialNumberIsFrac;
        internal static long icdServiceHasCheck;
        internal static bool icdServiceHasRequireCheck;
        internal static long icdServiceAllowUpdate;
        internal static long CheckSameHein;
        internal static bool IsWarningOverTotalPatientPrice;
        internal static bool IsWarringUseDayAndExpTimeBHYT;
        internal static bool IsDefaultTracking;
        internal static bool IsUsingServiceTime;
        internal static decimal WarningOverTotalPatientPrice;
        internal static long TutorialFormat;
        internal static string MedicineHasPaymentLimitBHYT;
        internal static string AtcCodeOverlarWarningOption;

        /// <summary>
        /// "Cấu hình ưu tiên lây thời gian hẹn khám (kiểu số):
        //- 1: Ưu tiên thời gian hẹn khám theo thời gian kê đơn thuốc
        //- 0: Ưu tiên theo thời gian được cấu hình"
        /// </summary>
        internal static string AppointmentTimeDefault;

        /// <summary>
        /// "Cấu hình ưu tiên lây thời gian hẹn khám (kiểu bool):
        //- true: Ưu tiên thời gian hẹn khám theo thời gian kê đơn thuốc
        //- false: Ưu tiên theo thời gian được cấu hình"
        /// </summary>
        internal static bool TreatmentEndHasAppointmentTimeDefault;

        /// <summary>
        /// Cấu hình giá trị ngày hẹn khám lại cho bệnh nhân
        /// </summary>
        internal static long TreatmentEndAppointmentTimeDefault;
        internal static long? WarringIntructionUseDayNum;

        internal static bool IsCheckFinishTime;
        internal static bool isCheckBedService;
        internal static bool isPrescriptionSplitOutMediStock;
        internal static bool mustFinishAllServicesBeforeFinishTreatment;
        internal static List<long> autoFinishServiceIds;
        internal static bool IsBlockWhileAcinByMedicineType;
        internal static bool IsloadIcdFromExamServiceExecute;
        /// <summary>
        /// Cấu hình cho phép kê thuốc/vật tư theo lô hoặc theo loại. Đăt 1: cho phép kê theo lô. Đặt khác 1: kê theo loại.
        /// </summary>
        internal static bool IsAllowAssignPresByPackage;

        /// <summary>
        /// 1: Kê nhiều ngày theo cả đơn; 2: Kê nhiều ngày theo từng thuốc
        /// </summary>
        internal static long ManyDayPrescriptionOption;

        internal static bool InPatientPrescription__ShowRoundAvailableAmount;

        internal static bool IsReasonRequired;

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

        static MOS.EFMODEL.DataModels.HIS_TREATMENT_TYPE GetTreatmentTypeById(long id)
        {
            MOS.EFMODEL.DataModels.HIS_TREATMENT_TYPE result = new MOS.EFMODEL.DataModels.HIS_TREATMENT_TYPE();
            try
            {
                result = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_TREATMENT_TYPE>().FirstOrDefault(o => o.ID == id);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result ?? new MOS.EFMODEL.DataModels.HIS_TREATMENT_TYPE();
        }

        internal static void LoadConfig()
        {
            try
            {
                //qtcode
                AllowSignaturePrintModuleLinks = GetValue(IS_ALLOW_SIGN_NATURE_PRINT);
                //qtcode
                IsTrackingRequired = GetValue(CONFIG_KEY__IS_TRACKING_REQUIRED) == GlobalVariables.CommonStringTrue;
                IsDefaultPatientTypeOption = GetValue(CONFIG_KEY__DEFAULT_TYPE_OPTION) == GlobalVariables.CommonStringTrue;
                IsWarningOddConvertAmount = GetValue(CONFIG_KEY__WARNING_ODD_CONVERT_AMOUNT) == GlobalVariables.CommonStringTrue;  
                IsDontPresExpiredTime = GetValue(CONFIG_KEY__DONT_PRES_EXPIRED_ITEM) == GlobalVariables.CommonStringTrue;
                IsUsingWarningHeinFee = GetValue(CONFIG_KEY__IsUsingWarningHeinFee);
                InPatientPrescription__ShowRoundAvailableAmount = GetValue(CONFIG_KEY__ShowRoundAvailableAmount) == GlobalVariables.CommonStringTrue;
                ManyDayPrescriptionOption = Inventec.Common.TypeConvert.Parse.ToInt64(GetValue(CONFIG_KEY__MOS_HIS_SERVICE_REQ_MANY_DAYS_PRESCRIPTION_OPTION));
                IsAllowAssignPresByPackage = GetValue(CONFIG_KEY__IsAllowAssignPresByPackage) == GlobalVariables.CommonStringTrue;
                IsloadIcdFromExamServiceExecute = GetValue(CONFIG_KEY__IsloadIcdFromExamServiceExecute) == GlobalVariables.CommonStringTrue;
                IsAutoCreateSaleExpMest = GetValue(CONFIG_KEY__IS_AUTO_CREATE_SALE_EXP_MEST) == GlobalVariables.CommonStringTrue;
                IsAutoTickExpendWithAssignPresPTTT = GetValue(CONFIG_KEY__IsAutoTickExpendWithAssignPresPTTT) == GlobalVariables.CommonStringTrue;
                icdServiceHasCheck = Inventec.Common.TypeConvert.Parse.ToInt64(GetValue(ICD_SERVICE__HAS_CHECK));
                icdServiceHasRequireCheck = GetValue(ICD_SERVICE__HAS_REQUIRE_CHECK) == GlobalVariables.CommonStringTrue;
                icdServiceAllowUpdate = Inventec.Common.TypeConvert.Parse.ToInt64(GetValue(HIS_ICD_SERVICE__ALLOW_UPDATE));
                AcinInteractive__Grade = Inventec.Common.TypeConvert.Parse.ToInt64(GetValue(Key__AcinInteractive__Grade));
                IsVisilbleExecuteGroup = GetValue(IS_VISILBE_EXECUTE_GROUP_KEY);
                LoadPatientTypeDefault = GetValue(LOAD_PATIENT_TYPE_DEFAULT_KEY);
                ViewMedicineMaterialOutOfStock = GetValue(VIEW_MEDICINE_MATERIAL_OUT_OF_STOCK_KEY);
                IsVisilbleTemplateMedicine = GetValue(IS_VISILBE_TEMPLATE_MEDICINE_KEY);

                ObligateIcd = GetValue(CONFIG_KEY__OBLIGATE_ICD);
                ShowRequestUser = GetValue(CONFIG_KEY__ShowRequestUser);
                AppointmentTimeDefault = (GetValue(TREATMENT_END___APPOINTMENT_TIME_DEFAULT_KEY));
                TreatmentEndHasAppointmentTimeDefault = (GetValue(PRESCRIPTION_TIME_AND_APPOINTMENT_TIME_KEY) == GlobalVariables.CommonStringTrue);
                TreatmentEndAppointmentTimeDefault = Inventec.Common.TypeConvert.Parse.ToInt64(AppointmentTimeDefault);

                string warringIntructionUseDayNumStr = GetValue(WARRING_INTRUCTION_USE_DAY_NUM);
                if (!String.IsNullOrEmpty(warringIntructionUseDayNumStr))
                {
                    WarringIntructionUseDayNum = Inventec.Common.TypeConvert.Parse.ToInt64(warringIntructionUseDayNumStr);
                }
                else
                {
                    WarringIntructionUseDayNum = null;
                }

                IsDefaultFocusMedicineTabPage = GetValue(CONFIG_KEY__IsDefaultFocusMedicineTabPage);

                //IsMultiCheckservice = GetValue(CONFIG_KEY__IsMultiCheckservice);
                PatientTypeCode__BHYT = GetValue(CONFIG_KEY__PATIENT_TYPE_CODE__BHYT);
                PatientTypeId__BHYT = GetPatientTypeByCode(PatientTypeCode__BHYT).ID;
                PatientTypeName__BHYT = GetPatientTypeByCode(PatientTypeCode__BHYT).PATIENT_TYPE_NAME;
                PatientTypeCode__VP = GetValue(CONFIG_KEY__PATIENT_TYPE_CODE__VP);
                PatientTypeId__VP = GetPatientTypeByCode(PatientTypeCode__VP).ID;
                PatientTypeName__VP = GetPatientTypeByCode(PatientTypeCode__VP).PATIENT_TYPE_NAME;
                AutoCheckIcd = GetValue(CONFIG_KEY__ICD_GENERA_KEY);
                LimitHeinMedicinePrice__RightMediOrg = Inventec.Common.TypeConvert.Parse.ToDecimal(GetValue(CONFIG_KEY__LIMIT_HEIN_MEDICINE_PRICE__RIGHT_MEDI_ORG));
                LimitHeinMedicinePrice__NotRightMediOrg = Inventec.Common.TypeConvert.Parse.ToDecimal(GetValue(CONFIG_KEY__LIMIT_HEIN_MEDICINE_PRICE__NOT_RIGHT_MEDI_ORG));

                TreatmentTypeCode__Exam = GetTreatmentTypeById(IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM).TREATMENT_TYPE_CODE;
                TreatmentTypeCode__TreatIn = GetTreatmentTypeById(IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU).TREATMENT_TYPE_CODE;
                TreatmentTypeCode__TreatOut = GetTreatmentTypeById(IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNGOAITRU).TREATMENT_TYPE_CODE;

                IsCheckFinishTime = GetValue(CONFIG_KEY__TREATMENT_FINISH__CHECK_FINISH_TIME) == GlobalVariables.CommonStringTrue;
                isCheckBedService = GetValue(CHECK_ASSIGN_SERVICE_BED) == GlobalVariables.CommonStringTrue;
                isPrescriptionSplitOutMediStock = GetValue(MOS__PRESCRIPTION_SPLIT_OUT_MEDISTOCK) == GlobalVariables.CommonStringTrue;
                mustFinishAllServicesBeforeFinishTreatment = GetValue(MUST_FINISH_ALL_SERVICES_BEFORE_FINISH_TREATMENT_CFG) == GlobalVariables.CommonStringTrue;
                IsUsingServiceTime = GetValue(IS_USING_SERVER_TIME) == GlobalVariables.CommonStringTrue;
                IsBlockWhileAcinByMedicineType = GetValue(CONFIG_KEY__IsBlockWhileAcinByMedicineType) == GlobalVariables.CommonStringTrue;
                DefaultDrugStoreCode = GetValue(IS_CHOOSE_DRUGSTORE);
                string listCode = GetValue(SERVICE_CODE__AUTO_FINISH);
                if (!string.IsNullOrEmpty(listCode))
                {
                    var codes = listCode.Split(';');
                    autoFinishServiceIds = GetServiceIds(codes);
                }
                IsAutoCloseFormWithAutoConfigTreatmentFinish = GetValue(CONFIG_KEY____ISAUTO_CLOSE_FROM_WITH_AUTO_TREATMENT_FINISH) == GlobalVariables.CommonStringTrue;
                IsAllowPrintFinish = GetValue(Key__IsAllowPrintFinish) == GlobalVariables.CommonStringTrue;
                IsTutorialNumberIsFrac = GetValue(TUTORIAL_NUMBER_IS_FRAC) == GlobalVariables.CommonStringTrue;
                CheckSameHein = Inventec.Common.TypeConvert.Parse.ToInt64(GetValue(CHECK_SAME_HEIN_KEY));
                InitWarningOverCeiling();

                TutorialFormat = Inventec.Common.TypeConvert.Parse.ToInt64(GetValue(TUTORIAL_FORMAT));

                IsWarningOverTotalPatientPrice = GetValue(WARNING_OVER_TOTAL_PATIENT_PRICE__IS_CHECK) == GlobalVariables.CommonStringTrue;
                WarningOverTotalPatientPrice = Inventec.Common.TypeConvert.Parse.ToDecimal(GetValue(WARNING_OVER_TOTAL_PATIENT_PRICE));
                IsWarringUseDayAndExpTimeBHYT = GetValue(WARRING_USE_DAY_AND_EXP_TIME_BHYT) == GlobalVariables.CommonStringTrue;
                IsDefaultTracking = GetValue(KEY_IS_DEFAULT_TRACKING) == GlobalVariables.CommonStringTrue;
                MedicineHasPaymentLimitBHYT = GetValue(MEDICINE_HAS_PAYMENT_LIMIT_BHYT);
                IsNotAllowingExpendWithoutHavingParent = (GetValue(MOS__HIS_SERVICE_REQ__IS_NOT_ALLOWING_EXPEND_WITHOUT_HAVING_PARENT) == "1");
                IsReasonRequired = (GetValue(CONFIG_KEY__IS_REASON_REQUIRED) == "1");

                AtcCodeOverlarWarningOption = GetValue(CONFIG_KEY__PRESCRIPTION_ATC_CODE_OVERLAP_WARNING_OPTION);
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

        static void InitWarningOverCeiling()
        {
            try
            {
                var vl = GetValue(Key__WarningOverCeiling__Exam__Out__In);
                if (!String.IsNullOrEmpty(vl))
                {
                    var arrVl = vl.Split(new String[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                    if (arrVl != null && arrVl.Length == 3)
                    {
                        WarningOverCeiling__Exam = Inventec.Common.TypeConvert.Parse.ToDecimal(arrVl[0]);
                        WarningOverCeiling__Out = Inventec.Common.TypeConvert.Parse.ToDecimal(arrVl[1]);
                        WarningOverCeiling__In = Inventec.Common.TypeConvert.Parse.ToDecimal(arrVl[2]);
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private static List<long> GetServiceIds(string[] value)
        {
            List<long> result = new List<long>();
            try
            {
                if (value != null && value.Count() > 0)
                {
                    List<V_HIS_SERVICE> services = BackendDataWorker.Get<V_HIS_SERVICE>().Where(o => value.Contains(o.SERVICE_CODE)).ToList();
                    if (services != null && services.Count > 0)
                        result = services.Select(s => s.ID).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = new List<long>();
            }
            return result;
        }
    }
}
