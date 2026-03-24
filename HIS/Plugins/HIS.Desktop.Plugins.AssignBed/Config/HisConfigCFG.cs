using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.AssignBed.Config
{
    internal class HisConfigCFG
    {
        private const string CONFIG_KEY__PATIENT_TYPE_CODE__BHYT = "MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BHYT";//Doi tuong BHYT
        private const string CONFIG_KEY__WARNING_OVER_TOTAL_PATIENT_PRICE__IS_CHECK = "HIS.Desktop.WarningOverTotalPatientPrice__IsCheck";
        private const string CONFIG_KEY__WARNING_OVER_TOTAL_PATIENT_PRICE = "HIS.Desktop.WarningOverTotalPatientPrice";
        private const string CONFIG_KEY__EPAYMENT__IS_USING_EXECUTE_ROOM_PAYMENT = "MOS.EPAYMENT.IS_USING_EXECUTE_ROOM_PAYMENT";
        private const string CONFIG_KEY__USING_SERVER_TIME = "MOS.IS_USING_SERVER_TIME";
        private const string KEY_ASSIGN_ROOM_BY_PATIENT_TYPE = "MOS.HIS_SERVICE_REQ.ASSIGN_ROOM_BY_PATIENT_TYPE";
        private const string CONFIG_KEY__TrackingCreate__UpdateTreatmentIcd = "HIS.Desktop.Plugins.TrackingCreate.UpdateTreatmentIcd";
        private const string CONFIG_KEY__IsloadIcdFromExamServiceExecute = "HIS.Desktop.Plugins.IsloadIcdFromExamServiceExecute";
        private const string CONFIG_KEY__ICD_GENERA_KEY = "HIS.Desktop.Plugins.AutoCheckIcd";
        private const string CONFIG_KEY__Icd_Service_Has_Check = "HIS.HIS_ICD_SERVICE.HAS_CHECK";
        private const string Key__WarningOverCeiling__Exam__Out__In = "HIS.Desktop.Plugins.WarningOverCeiling.Exam__Out__In";
        private const string CONFIG_KEY__SERVICE_REQ__IS_SERE_SERV_MIN_DURATION_ALERT = "HIS.Desktop.IsSereServMinDurationAlert";
        private const string KEY_ASSIGN_SERVICE_SIMULTANEITY_OPTION = "MOS.HIS_SERVICE_REQ.ASSIGN_SERVICE_SIMULTANEITY_OPTION";
        private const string KEY_ASSIGN_SIMULTANEITY_OPTION = "MOS.HIS_SERVICE_REQ.ASSIGN_SIMULTANEITY_OPTION";
        private const string CONFIG_KEY__IsNotAutoLoadServiceOpenAssignService = "HIS.Desktop.Plugins.AssignService.IsNotAutoLoadAssignService";
        private const string CONFIG_KEY__HIS_SERE_SERV__SET_PRIMARY = "MOS.HIS_SERE_SERV.IS_SET_PRIMARY_PATIENT_TYPE";
        private const string CONFIG_KEY__IsUsingWarningHeinFee = "His.Desktop.IsUsingWarningHeinFee";
        private const string CONFIG__ShowServerTimeByDefault = "HIS.Desktop.ShowServerTimeByDefault";
        private const string CONFIG_KEY__ReqUserMustHaveDiploma = "MOS.HIS_SERVICE_REQ.REQ_USER_MUST_HAVE_DIPLOMA";
        private const string CONFIG_KEY__IsShowingInTheSameDepartment = "His.Desktop.Plugins.ReqUser.IsShowingInTheSameDepartment";
        private const string CONFIG_KEY__ShowRequestUser = "HIS.Desktop.Plugins.AssignConfig.ShowRequestUser";
        private const string CONFIG_KEY__OBLIGATE_ICD = "EXE.ASSIGN_SERVICE_REQUEST__OBLIGATE_ICD";
        public const string CONFIG_KEY_HIS_DESKTOP_ASSIGN_SERVICE_WARNING_MAX_PATIENT_BY_DAY_OPTION = "HIS.DESKTOP.ASSIGN_SERVICE.WARNING_MAX_PATIENT_BY_DAY.OPTION";
        private const string CONFIG_KEY__INTEGRATION_VERSION = "MOS.LIS.INTEGRATION_VERSION";
        internal const string CONFIG_KEY__INTEGRATE_OPTION = "MOS.LIS.INTEGRATE_OPTION";
        internal const string CONFIG_KEY__INTEGRATION_TYPE = "MOS.LIS.INTEGRATION_TYPE";
        internal const string SERVICE_HAS_PAYMENT_LIMIT_BHYT = "HIS.Desktop.Plugins.AssignService.ServiceHasPaymentLimitBHYT";
        private const string CONFIG_KEY_DefaultPatientTypeOption = "HIS.Desktop.Plugins.Assign.DefaultPatientTypeOption";
        private const string CONFIG_KEY__BHYT__EXCEED_DAY_ALLOW_FOR_IN_PATIENT = "MOS.BHYT.EXCEED_DAY_ALLOW_FOR_IN_PATIENT";
        private const string CONFIG_KEY__ShowDefaultExecuteRoom = "HIS.Desktop.Plugins.AssignService.ShowDefaultExecuteRoom";
        private const string CONFIG_KEY__NoDifference = "HIS.Desktop.Plugins.AssignService.NoDifference";
        private const string CONFIG_KEY__HeadCardNumberNoDifference = "HIS.Desktop.Plugins.AssignService.HeadCardNumberNoDifference";
        private const string CONFIG_KEY__DepartmentCodeNoDifference = "HIS.Desktop.Plugins.AssignService.DepartmentCodeNoDifference";
        private const string CONFIG_KEY__PATIENT_TYPE_CODE__VP = "MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.HOSPITAL_FEE";//Doi tuong VP
        private const string CONFIG_KEY__PATY_FOR_PACKAGE = "His.Desktop.AssignService.HisPackage.ServicePatyForServicePackage";
        private const string IS_ALLOW_SIGN_NATURE_PRINT = "HIS.Desktop.Plugins.IsAllowSignaturePrint.ModuleLinks";
        private const string KEY__InstructionTimeServiceMustBeGreaterThanStartTimeExam = "HIS.Desktop.Plugins.InstructionTimeServiceMustBeGreaterThanStartTimeExam";
        private const string CHECK_ICD_WHEN_SAVE = "HIS.Desktop.Plugins.CheckIcdWhenSave";
        public const string CONFIG_KEY__ICD_SERVICE_HAS_REQUIRE_PATIENT_BHYT = "HIS.HIS_ICD_SERVICE.HAS_REQUIRE.PATIENT.BHYT";
        private const string CONFIG_KEY__Icd_Service_Allow_Update = "HIS.HIS_ICD_SERVICE.ALLOW_UPDATE";
        public const string ICD_SERVICE__HAS_REQUIRE_CHECK = "HIS.HIS_ICD_SERVICE.HAS_REQUIRE_CHECK";
        private const string CONFIG_KEY__BedServiceType_NotAllow_For_OutPatient = "HIS.Desktop.Plugins.AssignService.BedServiceType_NotAllow_For_OutPatient";
        public const string CONFIG_KEY_HIS_ICD_SERVICE_CONTRAINDICATED_WARNING_OPTION = "HIS.ICD_SERVICE.CONTRAINDICATED.WARNING_OPTION";
        private const string CONFIG_KEY_AssignBedServiceWithBedInfo = "HIS.Desktop.Plugins.AssignService.AssignBedServiceWithBedInfo";
        private const string CONFIG_KEY__IsAllowingChooseServiceWhichInAttachments = "HIS.Desktop.Plugins.AssignService.IsAllowingChooseServiceWhichInAttachments";
        internal const string CONFIG_KEY__AutoDeleteEmrDocumentWhenEditReq = "HIS.Desktop.Plugins.ServiceReqList.AutoDeleteEmrDocumentWhenEditReq";
        private const string CONFIG_KEY_CheckDepartmentInTimeWhenPresOrAssign = "HIS.Desktop.Plugins.IsCheckDepartmentInTimeWhenPresOrAssign";

        public static decimal WarningOverCeiling__Exam { get; set; }
        public static decimal WarningOverCeiling__Out { get; set; }
        public static decimal WarningOverCeiling__In { get; set; }
        public static int IsSereServMinDurationAlert { get; set; }

        internal static string PatientTypeCode__BHYT;
        internal static long PatientTypeId__BHYT;
        internal static string WarningOverTotalPatientPrice__IsCheck;
        internal static string WarningOverTotalPatientPrice;
        internal static bool IsUsingExecuteRoomPayment;
        internal static string IsUsingServerTime;
        internal static bool IsAssignRoomByPatientType;
        internal static string TrackingCreate__UpdateTreatmentIcd;
        internal static bool IsloadIcdFromExamServiceExecute;
        internal static string AutoCheckIcd;
        internal static string IcdServiceHasCheck;
        internal static string ASSIGN_SERVICE_SIMULTANEITY_OPTION;
        internal static string ASSIGN_SIMULTANEITY_OPTION; 
        internal static bool IsNotAutoLoadServiceOpenAssignService;
        internal static string IsSetPrimaryPatientType;
        internal static string IsUsingWarningHeinFee;
        internal static bool IsShowServerTimeByDefault;
        internal static bool IsShowingInTheSameDepartment;
        internal static bool IsReqUserMustHaveDiploma;
        internal static string ShowRequestUser;
        internal static string ObligateIcd;
        internal static long MaxPatientByDay;
        internal static string IntegrationVersionValue;
        internal static string IntegrationOptionValue;
        internal static string IntegrationTypeValue;
        internal static string ServiceHasPaymentLimitBHYT;
        internal static bool DefaultPatientTypeOption;
        internal static long BhytExceedDayAllowForInPatient;
        internal static string ShowDefaultExecuteRoom;
        internal static string NoDifference;
        internal static string HeadCardNumberNoDifference;
        internal static string DepartmentCodeNoDifference;
        internal static string PatientTypeCode__VP;
        internal static long PatientTypeId__VP;
        internal static string ServicePatyForServicePackage;
        internal static string IsAllowSignaturePrint;
        internal static string InstructionTimeServiceMustBeGreaterThanStartTimeExam;
        internal static string CheckIcdWhenSave;
        internal static bool IsIcdServiceHasRequireCheckPatientBHYT;
        internal static string IcdServiceAllowUpdate;
        internal static bool IcdServiceHasRequireCheck;
        internal static string BedServiceType_NotAllow_For_OutPatient;
        internal static long contraindicated;
        internal static bool AssignBedServiceWithBedInfo;
        internal static bool IsAllowingChooseServiceWhichInAttachments;
        internal static string AutoDeleteEmrDocumentWhenEditReq;
        internal static bool IsCheckDepartmentInTimeWhenPresOrAssign;

        internal static void LoadConfig()
        {
            try
            {
                PatientTypeCode__BHYT = GetValue(CONFIG_KEY__PATIENT_TYPE_CODE__BHYT);
                PatientTypeId__BHYT = GetPatientTypeByCode(PatientTypeCode__BHYT).ID;
                WarningOverTotalPatientPrice__IsCheck = GetValue(CONFIG_KEY__WARNING_OVER_TOTAL_PATIENT_PRICE__IS_CHECK);
                WarningOverTotalPatientPrice = GetValue(CONFIG_KEY__WARNING_OVER_TOTAL_PATIENT_PRICE);
                IsUsingExecuteRoomPayment = GetValue(CONFIG_KEY__EPAYMENT__IS_USING_EXECUTE_ROOM_PAYMENT) == GlobalVariables.CommonStringTrue;
                IsUsingServerTime = GetValue(CONFIG_KEY__USING_SERVER_TIME);
                IsAssignRoomByPatientType = GetValue(KEY_ASSIGN_ROOM_BY_PATIENT_TYPE) == GlobalVariables.CommonStringTrue;
                TrackingCreate__UpdateTreatmentIcd = GetValue(CONFIG_KEY__TrackingCreate__UpdateTreatmentIcd);
                IsloadIcdFromExamServiceExecute = GetValue(CONFIG_KEY__IsloadIcdFromExamServiceExecute) == GlobalVariables.CommonStringTrue;
                AutoCheckIcd = GetValue(CONFIG_KEY__ICD_GENERA_KEY);
                IcdServiceHasCheck = GetValue(CONFIG_KEY__Icd_Service_Has_Check);
                IsSereServMinDurationAlert = Convert.ToInt32(GetValue(CONFIG_KEY__SERVICE_REQ__IS_SERE_SERV_MIN_DURATION_ALERT));
                ASSIGN_SERVICE_SIMULTANEITY_OPTION = GetValue(KEY_ASSIGN_SERVICE_SIMULTANEITY_OPTION);
                ASSIGN_SIMULTANEITY_OPTION = GetValue(KEY_ASSIGN_SIMULTANEITY_OPTION);
                IsNotAutoLoadServiceOpenAssignService = GetValue(CONFIG_KEY__IsNotAutoLoadServiceOpenAssignService) == GlobalVariables.CommonStringTrue;
                IsSetPrimaryPatientType = GetValue(CONFIG_KEY__HIS_SERE_SERV__SET_PRIMARY);
                IsUsingWarningHeinFee = GetValue(CONFIG_KEY__IsUsingWarningHeinFee);
                IsShowServerTimeByDefault = GetValue(CONFIG__ShowServerTimeByDefault) == GlobalVariables.CommonStringTrue;
                IsReqUserMustHaveDiploma = GetValue(CONFIG_KEY__ReqUserMustHaveDiploma) == GlobalVariables.CommonStringTrue;
                IsShowingInTheSameDepartment = GetValue(CONFIG_KEY__IsShowingInTheSameDepartment) == GlobalVariables.CommonStringTrue;
                ShowRequestUser = GetValue(CONFIG_KEY__ShowRequestUser);
                ObligateIcd = GetValue(CONFIG_KEY__OBLIGATE_ICD);
                MaxPatientByDay = HisConfigs.Get<long>(CONFIG_KEY_HIS_DESKTOP_ASSIGN_SERVICE_WARNING_MAX_PATIENT_BY_DAY_OPTION);
                IntegrationVersionValue = GetValue(CONFIG_KEY__INTEGRATION_VERSION);
                IntegrationOptionValue = GetValue(CONFIG_KEY__INTEGRATE_OPTION);
                IntegrationTypeValue = GetValue(CONFIG_KEY__INTEGRATION_TYPE);
                ServiceHasPaymentLimitBHYT = GetValue(SERVICE_HAS_PAYMENT_LIMIT_BHYT);
                DefaultPatientTypeOption = GetValue(CONFIG_KEY_DefaultPatientTypeOption) == GlobalVariables.CommonStringTrue;
                BhytExceedDayAllowForInPatient = HisConfigs.Get<long>(CONFIG_KEY__BHYT__EXCEED_DAY_ALLOW_FOR_IN_PATIENT);
                ShowDefaultExecuteRoom = GetValue(CONFIG_KEY__ShowDefaultExecuteRoom);
                NoDifference = GetValue(CONFIG_KEY__NoDifference);
                HeadCardNumberNoDifference = GetValue(CONFIG_KEY__HeadCardNumberNoDifference);
                DepartmentCodeNoDifference = GetValue(CONFIG_KEY__DepartmentCodeNoDifference);
                PatientTypeCode__VP = GetValue(CONFIG_KEY__PATIENT_TYPE_CODE__VP);
                PatientTypeId__VP = GetPatientTypeByCode(PatientTypeCode__VP).ID;
                ServicePatyForServicePackage = GetValue(CONFIG_KEY__PATY_FOR_PACKAGE);
                IsAllowSignaturePrint = GetValue(IS_ALLOW_SIGN_NATURE_PRINT);
                InstructionTimeServiceMustBeGreaterThanStartTimeExam = GetValue(KEY__InstructionTimeServiceMustBeGreaterThanStartTimeExam);
                CheckIcdWhenSave = GetValue(CHECK_ICD_WHEN_SAVE);
                IsIcdServiceHasRequireCheckPatientBHYT = GetValue(CONFIG_KEY__ICD_SERVICE_HAS_REQUIRE_PATIENT_BHYT) == GlobalVariables.CommonStringTrue;
                IcdServiceAllowUpdate = GetValue(CONFIG_KEY__Icd_Service_Allow_Update);
                IcdServiceHasRequireCheck = GetValue(ICD_SERVICE__HAS_REQUIRE_CHECK) == GlobalVariables.CommonStringTrue;
                BedServiceType_NotAllow_For_OutPatient = GetValue(CONFIG_KEY__BedServiceType_NotAllow_For_OutPatient);
                contraindicated = HisConfigs.Get<long>(CONFIG_KEY_HIS_ICD_SERVICE_CONTRAINDICATED_WARNING_OPTION);
                AssignBedServiceWithBedInfo = GetValue(CONFIG_KEY_AssignBedServiceWithBedInfo) == GlobalVariables.CommonStringTrue;
                IsAllowingChooseServiceWhichInAttachments = GetValue(CONFIG_KEY__IsAllowingChooseServiceWhichInAttachments) == GlobalVariables.CommonStringTrue;
                AutoDeleteEmrDocumentWhenEditReq = GetValue(CONFIG_KEY__AutoDeleteEmrDocumentWhenEditReq);
                IsCheckDepartmentInTimeWhenPresOrAssign = GetValue(CONFIG_KEY_CheckDepartmentInTimeWhenPresOrAssign) == GlobalVariables.CommonStringTrue;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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

        public static void InitWarningOverCeiling()
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
    }
}
