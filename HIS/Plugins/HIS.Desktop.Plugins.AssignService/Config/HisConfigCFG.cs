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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.AssignService.Config
{
    class HisConfigCFG
    {
        private const string CONFIG_KEY__BHYT__EXCEED_DAY_ALLOW_FOR_IN_PATIENT = "MOS.BHYT.EXCEED_DAY_ALLOW_FOR_IN_PATIENT";
        private const string CONFIG_KEY__TrackingCreate__UpdateTreatmentIcd = "HIS.Desktop.Plugins.TrackingCreate.UpdateTreatmentIcd";
        private const string CONFIG_KEY__IsAutoCheckPriorityForPrioritizedExam = "HIS.Desktop.Plugins.AssignService.IsAutoCheckPriorityForPrioritizedExam";
        private const string CONFIG_KEY__IsSingleCheckservice = "HIS.Desktop.Plugins.AssignService.IsSingleCheckservice";
        private const string CONFIG_KEY__IsSearchAll = "HIS.Desktop.Plugins.AssignService.IsSearchAll";
        private const string CONFIG_KEY__ShowRequestUser = "HIS.Desktop.Plugins.AssignConfig.ShowRequestUser";
        private const string CONFIG_KEY__NoDifference = "HIS.Desktop.Plugins.AssignService.NoDifference";
        private const string CONFIG_KEY__HeadCardNumberNoDifference = "HIS.Desktop.Plugins.AssignService.HeadCardNumberNoDifference";
        private const string CONFIG_KEY__DepartmentCodeNoDifference = "HIS.Desktop.Plugins.AssignService.DepartmentCodeNoDifference";
        private const string CONFIG_KEY__OBLIGATE_ICD = "EXE.ASSIGN_SERVICE_REQUEST__OBLIGATE_ICD";
        public const string CONFIG_KEY__HIS_DEPOSIT__DEFAULT_PRICE_FOR_BHYT_OUT_PATIENT = "HIS_RS.HIS_DEPOSIT.DEFAULT_PRICE_FOR_BHYT_OUT_PATIENT";//tính tiền dịch vụ cần tạm ứng(ct MOS.LibraryHein.Bhyt.BhytPriceCalculator.DefaultPriceForBhytOutPatient) ở chức năng tạm ứng dịch vụ theo dịch vụ.
        private const string CONFIG_KEY__PATIENT_TYPE_CODE__BHYT = "MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BHYT";//Doi tuong BHYT
        private const string CONFIG_KEY__PATIENT_TYPE_CODE__VP = "MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.HOSPITAL_FEE";//Doi tuong VP
        private const string CONFIG_KEY__IS_VISILBE_EXECUTE_GROUP_KEY = "HIS.Desktop.Plugins.Assign.IsExecuteGroup";
        private const string CONFIG_KEY__ICD_GENERA_KEY = "HIS.Desktop.Plugins.AutoCheckIcd";
        private const string CONFIG_KEY__AssignServicePrintTEST = "HIS.Desktop.Plugins.AssignServicePrintTEST";
        private const string CONFIG_KEY__Icd_Service_Has_Check = "HIS.HIS_ICD_SERVICE.HAS_CHECK";
        public const string ICD_SERVICE__HAS_REQUIRE_CHECK = "HIS.HIS_ICD_SERVICE.HAS_REQUIRE_CHECK";
        private const string CONFIG_KEY__Icd_Service_Allow_Update = "HIS.HIS_ICD_SERVICE.ALLOW_UPDATE";
        private const string Key__WarningOverCeiling__Exam__Out__In = "HIS.Desktop.Plugins.WarningOverCeiling.Exam__Out__In";
        private const string CONFIG_KEY__WARNING_OVER_TOTAL_PATIENT_PRICE = "HIS.Desktop.WarningOverTotalPatientPrice";
        private const string CONFIG_KEY__WARNING_OVER_TOTAL_PATIENT_PRICE__IS_CHECK = "HIS.Desktop.WarningOverTotalPatientPrice__IsCheck";
        private const string CONFIG_KEY__HIS_SERE_SERV__SET_PRIMARY = "MOS.HIS_SERE_SERV.IS_SET_PRIMARY_PATIENT_TYPE";

        private const string CONFIG_KEY__IS_DEFAULT_TRACKING = "HIS.Desktop.Plugins.AssignPrescription.IsDefaultTracking";
        internal const string SERVICE_HAS_PAYMENT_LIMIT_BHYT = "HIS.Desktop.Plugins.AssignService.ServiceHasPaymentLimitBHYT";
        internal const string AUTO_FILTER_ROW = "HIS.Desktop.Plugins.AssignService.AutoFilterRow";
        private const string CONFIG_KEY__USING_SERVER_TIME = "MOS.IS_USING_SERVER_TIME";
        private const string MOS__HIS_SERVICE_REQ__IS_NOT_ALLOWING_EXPEND_WITHOUT_HAVING_PARENT = "MOS.HIS_SERVICE_REQ.IS_NOT_ALLOWING_EXPEND_WITHOUT_HAVING_PARENT";
        private const string CONFIG_KEY__SERVICE_REQ__IS_SERE_SERV_MIN_DURATION_ALERT = "HIS.Desktop.IsSereServMinDurationAlert";
        private const string CONFIG_KEY__IsUsingWarningHeinFee = "His.Desktop.IsUsingWarningHeinFee";
        private const string CONFIG_KEY__IsNotAutoLoadServiceOpenAssignService = "HIS.Desktop.Plugins.AssignService.IsNotAutoLoadAssignService";
        private const string CONFIG_KEY__IsloadIcdFromExamServiceExecute = "HIS.Desktop.Plugins.IsloadIcdFromExamServiceExecute";
        private const string CONFIG_KEY__IsAllowingChooseServiceWhichInAttachments = "HIS.Desktop.Plugins.AssignService.IsAllowingChooseServiceWhichInAttachments";
        private const string CONFIG_KEY__ReqUserMustHaveDiploma = "MOS.HIS_SERVICE_REQ.REQ_USER_MUST_HAVE_DIPLOMA";
        private const string CONFIG_KEY__IsShowingInTheSameDepartment = "His.Desktop.Plugins.ReqUser.IsShowingInTheSameDepartment";
        private const string CONFIG_KEY__ShowDefaultExecuteRoom = "HIS.Desktop.Plugins.AssignService.ShowDefaultExecuteRoom";
        private const string CONFIG_KEY__BhytColorCode = "HIS.Desktop.Plugins.AssignService.BhytServiceColorCode";
        private const string CONFIG_KEY__SetRequestRoomByBedRoomWhenBeingInSurgery = "HIS.Desktop.Plugins.AssignService.SetRequestRoomByBedRoomWhenBeingInSurgery";
        private const string CONFIG_KEY__IS_TRACKING_REQUIRED = "MOS.HIS_SERVICE_REQ.ASSIGN_SERVICES.IS_TRACKING_REQUIRED";
        private const string CONFIG_KEY__ALLOW_SHOWING_ANAPATHOLY = "HIS.Desktop.Plugins.AssignService.AllowShowingAnapathology";
        private const string CONFIG_KEY__EPAYMENT__IS_USING_EXECUTE_ROOM_PAYMENT = "MOS.EPAYMENT.IS_USING_EXECUTE_ROOM_PAYMENT";
        public const string CONFIG_KEY_HIS_DESKTOP_ASSIGN_SERVICE_WARNING_MAX_PATIENT_BY_DAY_OPTION = "HIS.DESKTOP.ASSIGN_SERVICE.WARNING_MAX_PATIENT_BY_DAY.OPTION";
        public const string CONFIG_KEY_HIS_ICD_SERVICE_CONTRAINDICATED_WARNING_OPTION = "HIS.ICD_SERVICE.CONTRAINDICATED.WARNING_OPTION";
        private const string CONFIG_KEY_CheckDepartmentInTimeWhenPresOrAssign = "HIS.Desktop.Plugins.IsCheckDepartmentInTimeWhenPresOrAssign";
        private const string CONFIG_KEY_AssignBedServiceWithBedInfo = "HIS.Desktop.Plugins.AssignService.AssignBedServiceWithBedInfo";
        private const string CONFIG_KEY_DefaultPatientTypeOption = "HIS.Desktop.Plugins.Assign.DefaultPatientTypeOption";
        private const string CONFIG_KEY_ALLOW_ASSIGN_OXYGEN = "MOS.HIS_SERVICE_REQ.ALLOW_ASSIGN_OXYGEN";

        private const string CONFIG_KEY__FormClosingOption = "HIS.Desktop.FormClosingOption";
        internal static bool IsFormClosingOption;

        private const string CONFIG_KEY__ModuleLinkApply = "HIS.Desktop.FormClosingOption.ModuleLinkApply";

        private const string CONFIG_KEY__BedServiceType_NotAllow_For_OutPatient = "HIS.Desktop.Plugins.AssignService.BedServiceType_NotAllow_For_OutPatient";
        private const string CONFIG_KEY__SERVICE_REQ_ICD_OPTION = "HIS.HIS_TRACKING.SERVICE_REQ_ICD_OPTION";
        private const string CONFIG__ShowServerTimeByDefault = "HIS.Desktop.ShowServerTimeByDefault";
        private const string CHECK_ICD_WHEN_SAVE = "HIS.Desktop.Plugins.CheckIcdWhenSave";
        private const string CONFIG_KEY__INTEGRATION_VERSION = "MOS.LIS.INTEGRATION_VERSION";
        internal const string CONFIG_KEY__INTEGRATE_OPTION = "MOS.LIS.INTEGRATE_OPTION";
        internal const string CONFIG_KEY__INTEGRATION_TYPE = "MOS.LIS.INTEGRATION_TYPE";
        internal const string CONFIG_KEY__AutoDeleteEmrDocumentWhenEditReq = "HIS.Desktop.Plugins.ServiceReqList.AutoDeleteEmrDocumentWhenEditReq";
        private const string KEY__InstructionTimeServiceMustBeGreaterThanStartTimeExam = "HIS.Desktop.Plugins.InstructionTimeServiceMustBeGreaterThanStartTimeExam";
        private const string KEY_ASSIGN_SERVICE_SIMULTANEITY_OPTION = "MOS.HIS_SERVICE_REQ.ASSIGN_SERVICE_SIMULTANEITY_OPTION";
        private const string KEY_ASSIGN_ROOM_BY_PATIENT_TYPE = "MOS.HIS_SERVICE_REQ.ASSIGN_ROOM_BY_PATIENT_TYPE";
        private const string IS_ALLOW_SIGN_NATURE_PRINT = "HIS.Desktop.Plugins.IsAllowSignaturePrint.ModuleLinks";
        internal static bool IsAssignRoomByPatientType;
        internal static string ASSIGN_SERVICE_SIMULTANEITY_OPTION;
        internal static string InstructionTimeServiceMustBeGreaterThanStartTimeExam;
        internal static string AutoDeleteEmrDocumentWhenEditReq;
        internal static string IntegrationVersionValue;
        internal static string IntegrationOptionValue;
        internal static string IntegrationTypeValue;
        internal static string CheckIcdWhenSave;
        internal static bool IsShowServerTimeByDefault;
        internal static bool IsServiceReqIcdOption;
        internal static string ModuleLinkApply;
        internal static bool DefaultPatientTypeOption;
        internal static bool AssignBedServiceWithBedInfo;
        internal static string TrackingCreate__UpdateTreatmentIcd;
        internal static bool IsUsingExecuteRoomPayment;
        internal static bool IsShowingInTheSameDepartment;
        internal static string ShowDefaultExecuteRoom;
        internal static bool IsReqUserMustHaveDiploma;
        internal static bool IsAllowingChooseServiceWhichInAttachments;
        internal static bool IsAutoCheckPriorityForPrioritizedExam;
        internal static bool IsNotAutoLoadServiceOpenAssignService;
        internal static string IsUsingWarningHeinFee;
        internal static string IsAllowSignaturePrint;
        public static int IsSereServMinDurationAlert { get; set; }
        public static decimal WarningOverCeiling__Exam { get; set; }
        public static decimal WarningOverCeiling__Out { get; set; }
        public static decimal WarningOverCeiling__In { get; set; }

        internal static string TreatmentTypeCode__Exam;
        internal static string TreatmentTypeCode__TreatIn;
        internal static string TreatmentTypeCode__TreatOut;

        internal static bool AssignPrintTEST;
        internal static string ObligateIcd;
        internal static string SetDefaultDepositPrice;
        internal static string PatientTypeCode__BHYT;
        internal static long PatientTypeId__BHYT;
        internal static string PatientTypeCode__VP;
        internal static long PatientTypeId__VP;
        internal static string IsVisibleExecuteGroup;
        internal static string AutoCheckIcd;
        internal static string IcdServiceHasCheck;
        internal static bool IcdServiceHasRequireCheck;
        internal static bool IsRequiredTracking;
        internal static string IcdServiceAllowUpdate;
        internal static bool IsSearchAll;
        internal static string BedServiceType_NotAllow_For_OutPatient;
        /// <summary>
        /// "0 (hoặc ko khai báo): Không kiểm tra 
        //+ 1: Có kiểm tra dịch vụ đã yêu cầu có nằm trong danh sách đã được cấu hình tương ứng với ICD của bệnh nhân hay không. Nếu tồn tại dịch vụ không được cấu hình thì hiển thị thông báo và không cho lưu.
        //+ 2: Có kiểm tra, nhưng chỉ hiển thị cảnh báo, và hỏi "Bạn có muốn tiếp tục không". Nếu người dùng chọn "OK" thì vẫn cho phép lưu"
        /// </summary>
        internal static string WarningOverTotalPatientPrice;
        internal static string WarningOverTotalPatientPrice__IsCheck;
        internal static string IsDefaultTracking;
        internal static string ServiceHasPaymentLimitBHYT;
        internal static string IsSetPrimaryPatientType;
        internal static string IsUsingServerTime;
        internal static bool IsNotAllowingExpendWithoutHavingParent;

        /// <summary>
        /// Cấu hình để ẩn/hiện trường người chỉ định tai form chỉ định, kê đơn
        //- Giá trị mặc định (hoặc ko có cấu hình này) sẽ ẩn
        //- Nếu có cấu hình, đặt là 1 thì sẽ hiển thị
        /// </summary>
        internal static string ShowRequestUser;

        /// <summary>
        /// cấu hình hệ thống để hiển thị tủ trực hay không
        ///Đặt 1 là chỉ hiển thị các kho là tủ trực, giá trị khác là hiển thị tất cả các kho
        /// </summary>
        internal static string IsSingleCheckservice;

        internal static string AutoFilterRow;

        internal static string HeadCardNumberNoDifference;
        internal static string DepartmentCodeNoDifference;
        internal static string NoDifference;

        internal static bool IsloadIcdFromExamServiceExecute;
        internal static string BhytColorCode;
        internal static string SetRequestRoomByBedRoomWhenBeingInSurgery;
        internal static long MaxPatientByDay;
        internal static long contraindicated;

        /// <summary>
        /// MOS.BHYT.EXCEED_DAY_ALLOW_FOR_IN_PATIENT
        /// Số ngày tối đa cho phép bệnh nhân điều trị nội trú hoặc điều trị ban ngày được hưởng BHYT sau khi hết hạn thẻ (theo nghị định 146/2018)
        /// </summary>
        internal static long BhytExceedDayAllowForInPatient;

        internal static bool IsCheckDepartmentInTimeWhenPresOrAssign;

        internal static bool AllowAssignOxygen;

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
                IsAssignRoomByPatientType = GetValue(KEY_ASSIGN_ROOM_BY_PATIENT_TYPE) == GlobalVariables.CommonStringTrue;
                ASSIGN_SERVICE_SIMULTANEITY_OPTION = GetValue(KEY_ASSIGN_SERVICE_SIMULTANEITY_OPTION);
                InstructionTimeServiceMustBeGreaterThanStartTimeExam = GetValue(KEY__InstructionTimeServiceMustBeGreaterThanStartTimeExam);
                AutoDeleteEmrDocumentWhenEditReq = GetValue(CONFIG_KEY__AutoDeleteEmrDocumentWhenEditReq);
                IntegrationOptionValue = GetValue(CONFIG_KEY__INTEGRATE_OPTION);
                IntegrationTypeValue = GetValue(CONFIG_KEY__INTEGRATION_TYPE);
                IntegrationVersionValue = GetValue(CONFIG_KEY__INTEGRATION_VERSION);
                CheckIcdWhenSave = GetValue(CHECK_ICD_WHEN_SAVE);
                IsAllowSignaturePrint = GetValue(IS_ALLOW_SIGN_NATURE_PRINT);
                IsShowServerTimeByDefault = GetValue(CONFIG__ShowServerTimeByDefault) == GlobalVariables.CommonStringTrue;
                IsServiceReqIcdOption = GetValue(CONFIG_KEY__SERVICE_REQ_ICD_OPTION) == GlobalVariables.CommonStringTrue;
                BedServiceType_NotAllow_For_OutPatient = GetValue(CONFIG_KEY__BedServiceType_NotAllow_For_OutPatient);
                IsFormClosingOption = GetValue(CONFIG_KEY__FormClosingOption) == GlobalVariables.CommonStringTrue;
                ModuleLinkApply = GetValue(CONFIG_KEY__ModuleLinkApply);
                DefaultPatientTypeOption = GetValue(CONFIG_KEY_DefaultPatientTypeOption) == GlobalVariables.CommonStringTrue;
                AssignBedServiceWithBedInfo = GetValue(CONFIG_KEY_AssignBedServiceWithBedInfo) == GlobalVariables.CommonStringTrue;
                BhytExceedDayAllowForInPatient = HisConfigs.Get<long>(CONFIG_KEY__BHYT__EXCEED_DAY_ALLOW_FOR_IN_PATIENT);
                TrackingCreate__UpdateTreatmentIcd = GetValue(CONFIG_KEY__TrackingCreate__UpdateTreatmentIcd);
                IsUsingExecuteRoomPayment = GetValue(CONFIG_KEY__EPAYMENT__IS_USING_EXECUTE_ROOM_PAYMENT) == GlobalVariables.CommonStringTrue;
                SetRequestRoomByBedRoomWhenBeingInSurgery = GetValue(CONFIG_KEY__SetRequestRoomByBedRoomWhenBeingInSurgery);
                BhytColorCode = GetValue(CONFIG_KEY__BhytColorCode);
                IsAllowingChooseServiceWhichInAttachments = GetValue(CONFIG_KEY__IsAllowingChooseServiceWhichInAttachments) == GlobalVariables.CommonStringTrue;
                IsReqUserMustHaveDiploma = GetValue(CONFIG_KEY__ReqUserMustHaveDiploma) == GlobalVariables.CommonStringTrue;
                IsRequiredTracking = GetValue(CONFIG_KEY__IS_TRACKING_REQUIRED) == GlobalVariables.CommonStringTrue;
                IsShowingInTheSameDepartment = GetValue(CONFIG_KEY__IsShowingInTheSameDepartment) == GlobalVariables.CommonStringTrue;
                IsloadIcdFromExamServiceExecute = GetValue(CONFIG_KEY__IsloadIcdFromExamServiceExecute) == GlobalVariables.CommonStringTrue;
                IsAutoCheckPriorityForPrioritizedExam = GetValue(CONFIG_KEY__IsAutoCheckPriorityForPrioritizedExam) == GlobalVariables.CommonStringTrue;
                IsNotAutoLoadServiceOpenAssignService = GetValue(CONFIG_KEY__IsNotAutoLoadServiceOpenAssignService) == GlobalVariables.CommonStringTrue;
                IsUsingWarningHeinFee = GetValue(CONFIG_KEY__IsUsingWarningHeinFee);
                //IsSereServMinDurationAlert = (GetValue(CONFIG_KEY__SERVICE_REQ__IS_SERE_SERV_MIN_DURATION_ALERT) == GlobalVariables.CommonStringTrue);
                IsSereServMinDurationAlert = Convert.ToInt32(GetValue(CONFIG_KEY__SERVICE_REQ__IS_SERE_SERV_MIN_DURATION_ALERT));
                ShowRequestUser = GetValue(CONFIG_KEY__ShowRequestUser);
                IsSingleCheckservice = GetValue(CONFIG_KEY__IsSingleCheckservice);
                IsSearchAll = (GetValue(CONFIG_KEY__IsSearchAll) == GlobalVariables.CommonStringTrue);
                AutoFilterRow = GetValue(AUTO_FILTER_ROW);
                HeadCardNumberNoDifference = GetValue(CONFIG_KEY__HeadCardNumberNoDifference);
                DepartmentCodeNoDifference = GetValue(CONFIG_KEY__DepartmentCodeNoDifference);
                NoDifference = GetValue(CONFIG_KEY__NoDifference);
                ObligateIcd = GetValue(CONFIG_KEY__OBLIGATE_ICD);
                SetDefaultDepositPrice = GetValue(CONFIG_KEY__HIS_DEPOSIT__DEFAULT_PRICE_FOR_BHYT_OUT_PATIENT);
                PatientTypeCode__BHYT = GetValue(CONFIG_KEY__PATIENT_TYPE_CODE__BHYT);
                PatientTypeId__BHYT = GetPatientTypeByCode(PatientTypeCode__BHYT).ID;
                PatientTypeCode__VP = GetValue(CONFIG_KEY__PATIENT_TYPE_CODE__VP);
                PatientTypeId__VP = GetPatientTypeByCode(PatientTypeCode__VP).ID;
                IcdServiceHasCheck = GetValue(CONFIG_KEY__Icd_Service_Has_Check);
                IcdServiceHasRequireCheck = GetValue(ICD_SERVICE__HAS_REQUIRE_CHECK) == GlobalVariables.CommonStringTrue;
                IsVisibleExecuteGroup = GetValue(CONFIG_KEY__IS_VISILBE_EXECUTE_GROUP_KEY);
                AutoCheckIcd = GetValue(CONFIG_KEY__ICD_GENERA_KEY);
                IcdServiceAllowUpdate = GetValue(CONFIG_KEY__Icd_Service_Allow_Update);
                WarningOverTotalPatientPrice = GetValue(CONFIG_KEY__WARNING_OVER_TOTAL_PATIENT_PRICE);
                WarningOverTotalPatientPrice__IsCheck = GetValue(CONFIG_KEY__WARNING_OVER_TOTAL_PATIENT_PRICE__IS_CHECK);
                IsDefaultTracking = GetValue(CONFIG_KEY__IS_DEFAULT_TRACKING);
                AssignPrintTEST = (GetValue(CONFIG_KEY__AssignServicePrintTEST) == GlobalVariables.CommonStringTrue);
                TreatmentTypeCode__Exam = GetTreatmentTypeById(IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM).TREATMENT_TYPE_CODE;
                TreatmentTypeCode__TreatIn = GetTreatmentTypeById(IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU).TREATMENT_TYPE_CODE;
                TreatmentTypeCode__TreatOut = GetTreatmentTypeById(IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNGOAITRU).TREATMENT_TYPE_CODE;
                ServiceHasPaymentLimitBHYT = GetValue(SERVICE_HAS_PAYMENT_LIMIT_BHYT);
                IsSetPrimaryPatientType = GetValue(CONFIG_KEY__HIS_SERE_SERV__SET_PRIMARY);
                IsUsingServerTime = GetValue(CONFIG_KEY__USING_SERVER_TIME);
                InitWarningOverCeiling();
                IsNotAllowingExpendWithoutHavingParent = (GetValue(MOS__HIS_SERVICE_REQ__IS_NOT_ALLOWING_EXPEND_WITHOUT_HAVING_PARENT) == GlobalVariables.CommonStringTrue);
                ShowDefaultExecuteRoom = GetValue(CONFIG_KEY__ShowDefaultExecuteRoom);
                MaxPatientByDay = HisConfigs.Get<long>(CONFIG_KEY_HIS_DESKTOP_ASSIGN_SERVICE_WARNING_MAX_PATIENT_BY_DAY_OPTION);
                contraindicated = HisConfigs.Get<long>(CONFIG_KEY_HIS_ICD_SERVICE_CONTRAINDICATED_WARNING_OPTION);
                IsCheckDepartmentInTimeWhenPresOrAssign = GetValue(CONFIG_KEY_CheckDepartmentInTimeWhenPresOrAssign) == GlobalVariables.CommonStringTrue;
                AllowAssignOxygen = GetValue(CONFIG_KEY_ALLOW_ASSIGN_OXYGEN) == GlobalVariables.CommonStringTrue;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
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
