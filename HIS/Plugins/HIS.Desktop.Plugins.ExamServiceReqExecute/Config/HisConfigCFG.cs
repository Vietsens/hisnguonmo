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

namespace HIS.Desktop.Plugins.ExamServiceReqExecute.Config
{
    class HisConfigCFG
    {
        public const string KEY__MOS_TREATMENT_ALLOW_MANY_TREATMENT_OPENING_OPTION = "MOS.TREATMENT.ALLOW_MANY_TREATMENT_OPENING_OPTION";
        public const string KEY_HIS_DESKTOP_PLUGINS_EXAMSERVICEREQEXECUTE_ISENABLEEDITSTARTTIME  = "HIS.Desktop.Plugins.ExamServiceReqExecute.IsEnableEditStartTime";
        public static string IsEnableEditStartTime;
        // Doc1: Co lay thong tin benh nhan, ICD tu kham chinh sang kham phu hay khong (1- Co, Khac 1- Khong)
        public const string KEY_HIS_DESKTOP_PLUGINS_EXAMSERVICEREQEXECUTE_ISAUTOFILLINFORMATIONANDICDEXAM = "HIS.Desktop.Plugins.ExamServiceReqExecute.IsAutoFillInformationAndIcdExam";
        public static string IsAutoFillInformationAndIcdExam;
        public const string CONFIG_KEY_IsCheckServiceFollowWhenOut = "HIS.Desktop.Plugins.IsCheckServiceFollowWhenOut";
        internal static bool IsCheckServiceFollowWhenOut;
        public const string REQUIRED_PULSE_BLOOD_PRESSURE = "HIS.UC.DHST__REQUIRED_PULSE_BLOOD_PRESSURE";

        private const string CONFIG_KEY__IsloadIcdFromExamServiceExecute = "HIS.Desktop.Plugins.IsloadIcdFromExamServiceExecute";
        internal static bool IsloadIcdFromExamServiceExecute;
        private const string CONFIG_KEY__ICD_GENERA_KEY = "HIS.Desktop.Plugins.AutoCheckIcd";
        // tự động tắt màn hình xử lý khám sau khi kết thúcASSIGN_SERVICE_SIMULTANEITY_OPTION
        private const string CONFIG_KEY_AutoExitAfterFinish = "HIS.Desktop.Plugins.ExamServiceReqExecute.AutoExitAfterFinish";
        internal static bool IsAutoExitAfterFinish;

        // tự động check vào in phiếu khám bệnh vào viện
        private const string CONFIG_KEY_AutoCheckPrintHospitalizeExam = "HIS.Desktop.Plugins.ExamServiceReqExecute.AutoCheckPrintHopitalizeExam";
        private const string CONFIG_KEY_DefaultIsNotRequireFeeForNonBhyt = "HIS.Desktop.Plugins.ExamServiceReqExecute.AddExam.DefaultIsNotRequireFeeForNonBhyt";
        internal static bool IsAutoCheckPrintHospitalizeExam;
        private const string CONFIG_KEY__IS_CHECK_PREVIOUS_PRESCRIPTION = "HIS.Desktop.Plugins.AssignPrescriptionPK.IsCheckPreviousPrescripton";
        internal static bool IsNotRequiredFee;
        private const string CONFIG_KEY_IS_CHECK_ENABLE_EXAM_TYPE = "HIS.Desktop.Plugins.ExamServiceReqExecute.EnableByExamType";
        private const string CONFIG_KEY__IS_REQUIRED_TREATMENT_METHOD_OPTION = "HIS.Desktop.Plugins.TreatmentFinish.RequiredTreatmentMethodOption"; 
        private const string KEY_TreatmentEndTypeIsTransfer = "HIS.Desktop.Plugins.TreatmentFinish.TreatmentEndTypeIsTransfer";
        private const string KEY_PrinMps000062 = "HIS.Desktop.Plugins.ExamServiceReqExecute.PrinMps000062";
        internal static string keyMps000062;
        internal static string OptionTreatmentEndTypeIsTransfer;
        internal static string enableExamtype;

        internal static bool IsAutoCloseAfterSolving;

        internal static bool IsCheckPreviousPrescription;

        //
        private const string CONFIG_KEY__MPS_PrintPrescription = "HIS.Desktop.Plugins.Library.PrintPrescription.Mps";
        internal static string MPS_PrintPrescription;

        private const string CONFIG_KEY__IS_AUTO_SET_EXAM_INFO_BY_PREVIOUS_TREATMENT_IN_CASE_OF_OUT_PATIENT = "HIS.Desktop.Plugins.ExamServiceReqExecute.IsAutoSetExamInforByPreviousTreatmentInCaseOfOutPatient";
        internal static bool IsAutoSetExamInforByPreviousTreatmentInCaseOfOutPatient;

        private const string CONFIG_KEY__IS_ALLOW_PRINT_NO_MEDICINE = "HIS.Desktop.Plugins.ExamServiceReqExecute.IsAllowPrintNoMedicinePrescription";
        internal static bool IsAllowPrintNoMedicine;

        private const string AUTO_SET_ICD_WHEN_FINISH_IN_ADDITION_EXAM = "MOS.HIS_TREATMENT.AUTO_SET_ICD_WHEN_FINISH_IN_ADDITION_EXAM";
        internal static bool IsAutoSetIcdWhenFinishInOtherExam;

        private const string CONFIG_KEY__FormClosingOption = "HIS.Desktop.FormClosingOption";
        internal static bool IsFormClosingOption;

        private const string CONFIG_KEY__ModuleLinkApply = "HIS.Desktop.FormClosingOption.ModuleLinkApply";
        internal static string ModuleLinkApply;

        private const string EXECUTE_ROOM_PAYMENT_OPTION = "MOS.EPAYMENT.EXECUTE_ROOM_PAYMENT_OPTION";
        internal static string executeRoomPaymentOption;
        //huannh bo sung key
        private const string ASSIGN_SERVICE_SIMULTANEITY_OPTION = "MOS.HIS_SERVICE_REQ.ASSIGN_SERVICE_SIMULTANEITY_OPTION";
        internal static string AssignServiceSimultaneityOption;
        private const string ASSIGN_SIMULTANEITY_OPTION = "MOS.HIS_SERVICE_REQ.ASSIGN_SIMULTANEITY_OPTION";
        internal static string AssignSimultaneityOption;

        private const string TERMINAL_SYTEM_ADDRESS = "MOS.EPAYMENT.TERMINAL_SYTEM.ADDRESS";
        internal static string terminalSystemAddress;
        private const string TERMINAL_SYTEM_SECURE_KEY = "MOS.EPAYMENT.TERMINAL_SYTEM.SECURE_KEY";
        internal static string terminalSystemSecureKey;

        private const string DISABLE_PART_EXAM_BY_EXECUTOR = "HIS.Desktop.Plugins.ExamServiceReqExecute.DisablePartExamByExecutor";
        internal static bool isDisablePartExamByExecutor;
        private const string CHECK_ICD_WHEN_SAVE = "HIS.Desktop.Plugins.CheckIcdWhenSave";
        internal static string CheckIcdWhenSave;
        internal const string DHST_REQUIRED_OPTION = "HIS.Desktop.Plugins.ExamServiceReqExecute.Dhst.RequiredWeightHeight_Option";
        internal static string RequiredWeightHeight_Option;
        private const string KEY__MustChooseSeviceExam = "HIS.Desktop.Plugins.TreatmentFinish.MustChooseSeviceExam.Option";

        internal static string MustChooseSeviceExamOption;
        private const string KEY__IsRequiredTemperatureOption = "HIS.Desktop.Plugins.ExamServiceReqExecute.IsRequiredTemperatureOption";
        private const string KEY__RequiredAddressOption = "HIS.Desktop.Plugins.ExamServiceReqExecute.RequiredAddressOption";
        private const string KEY__HospitalizationReasonRequiredByPatientCode = "HIS.Desktop.Plugins.ExamServiceReqExecute.HospitalizationReasonRequiredByPatientCode";
        internal static string HospitalizationReasonRequiredByPatientCode;
        private const string KEY__AutoCreatePaymentTransactions = "HIS.Desktop.Plugins.ExamServiceReqExecute.AutoCreatePaymentTransactions";
        internal static string AutoCreatePaymentTransactions;
        internal static bool IsRequiredTemperatureOption;
        internal static bool RequiredAddressOption;
        internal static string RequiredTreatmentMethodOption;
        internal static string AutoCheckIcd;
        private const string KEY_IsRequiredPathologicalProcessTransferPatientBHYT = "HIS.Desktop.Plugins.TreatmentFinish.IsRequiredPathologicalProcessTransferPatientBHYT";
        private const string KEY_PathologicalProcessOption = "HIS.Desktop.Plugins.TreatmentFinish.PathologicalProcessOption";
        internal static bool IsRequiredPathologicalProcessTransferPatientBHYT;
        internal static int PathologicalProcessOption;
        private const string KEY_AllowBhxhLeaveOver30days = "His.LeaveDay.AllowBhxhLeaveOver30days";
        internal static string AllowBhxhLeaveOver30days;
        internal static string AllowManyTreatmentOpeningOption;
        private const string KEY_IsCheckValueMaxlengthOption = "HIS.Desktop.Plugins.TreatmentFinish.IsCheckValueMaxlengthOption";
        internal static string IsCheckValueMaxlengthOption;
        private const string KEY_MOS_HIS_SERVICE_REQ_NOT_UPDATE_EXECUTE_LOGINNAME_WHEN_FINISH_EXAM = "MOS.HIS_SERVICE_REQ.NOT_UPDATE_EXECUTE_LOGINNAME_WHEN_FINISH_EXAM";
        internal static string NotUpdateExecuteLoginNameWhenFinishExam;
        private const string KEY_HIS_DESKTOP_PLUGINS_REGISTER_V2_REQUEST_SKIN_CARE = "HIS.Desktop.Plugins.RegisterV2.RequestSkinCare";
        internal static string HisDesktopPluginsRegisterV2RequestSkinCare;

        // PTTK_19083: Bật tính năng phân loại cấp cứu tại phòng cấp cứu
        private const string KEY_MOS_HIS_TREATMENT_EMERGENCY_CLASSIFY = "MOS.HIS_TREATMENT.EMERGENCY_CLASSIFY";
        internal static bool IsEnableEmergencyClassify;

        // MIMS Drug Pregnancy/Lactation: bật tab "Phân loại phụ nữ" (PN mang thai / cho con bú)
        private const string KEY_HIS_DESKTOP_MIMS_IS_CHECK_PREGNANCY_LACTATION = "HIS.Desktop.Mims.IsCheckPregnancyLactation";
        internal static bool IsCheckMimsPregnancyLactation;

        // 2608 - Bệnh nặng xin về: danh sách TREATMENT_END_TYPE_CODE trigger popup Thông tin người bệnh nặng xin về
        private const string KEY__MOS_HIS_SEVERE_ILLNESS_INFO_MUST_INPUT_SEVERE_ILLNESS_HOME_CODES = "MOS.HIS_SEVERE_ILLNESS_INFO.MUST_INPUT_SEVERE_ILLNESS_HOME_CODES";
        internal static List<string> MustInputSevereIllnessHomeCodes = new List<string>();

        // PTTK 4.1.2: Kiểm tra số mã ICD phụ ra viện (HIS.UC.ExamTreatmentFinish)
        // "1" = chặn lưu khi vượt ngưỡng, "2" = cảnh báo Yes/No, khác/không khai = không kiểm tra
        private const string KEY_IsCheckSubIcdExceedLimit = "HIS.Desktop.Plugins.IsCheckSubIcdExceedLimit";
        internal static string IsCheckSubIcdExceedLimit;
        // Ngưỡng tối đa số mã ICD phụ ra viện. Không khai báo hoặc không hợp lệ -> mặc định 12
        private const string KEY_IcdSubMaxCount = "HIS.Desktop.Plugins.IsCheckSubIcdExceedLimit.IcdSubMaxCount";
        internal const int ICD_SUB_MAX_COUNT_DEFAULT = 12;
        internal static int IcdSubMaxCount;

        // Chan nhap vien khi con van ban chua hoan thanh: danh sach DEPARTMENT_CODE ap dung, phan tach boi "|".
        // Khong khai bao/de trong -> khong kiem tra.
        internal const string KEY_CheckDepaDocumentHospitalization = "HIS.Desktop.Plugins.ExamServiceReqExecute.CheckDepaDocument.Hospitalization";
        internal static List<string> CheckDepaDocumentHospitalizationCodes = new List<string>();

        /// <summary>
        /// Cấu hình: HIS.Desktop.Plugins.AssignPrescription.ENABLE_TREATMENT_PRESCRIPTION
        /// - BẬT (= 1): cho phép kê đơn điều trị -> hiển thị mục "Kê đơn điều trị" trong menu Khác
        ///   và mở form kê đơn ở chế độ đơn điều trị (IsExecutePTTT = true).
        /// - TẮT (= 0 / null — mặc định): ẩn mục "Kê đơn điều trị", luồng kê đơn giữ nguyên hoàn toàn.
        /// </summary>
        private const string CONFIG_KEY__ENABLE_TREATMENT_PRESCRIPTION = "HIS.Desktop.Plugins.AssignPrescription.ENABLE_TREATMENT_PRESCRIPTION";
        internal static bool EnableTreatmentPrescription;

        internal static void LoadConfig()
        {
            try
            {
                // Doc som: LoadConfig dung chung mot try/catch, neu mot key phia sau nem loi
                // thi cac key con lai se khong duoc doc -> tinh nang chan nhap vien bi tat am tham.
                keyMps000062 = GetValue(KEY_PrinMps000062);
                string rawCheckDepaDocHospitalize = GetValue(KEY_CheckDepaDocumentHospitalization);
                CheckDepaDocumentHospitalizationCodes = string.IsNullOrWhiteSpace(rawCheckDepaDocHospitalize)
                    ? new List<string>()
                    : rawCheckDepaDocHospitalize.Split('|').Select(o => (o ?? "").Trim().ToUpper()).Where(o => o.Length > 0).ToList();

                EnableTreatmentPrescription = GetValue(CONFIG_KEY__ENABLE_TREATMENT_PRESCRIPTION) == GlobalVariables.CommonStringTrue;

                IsCheckValueMaxlengthOption = GetValue(KEY_IsCheckValueMaxlengthOption);
                IsCheckServiceFollowWhenOut = GetValue(CONFIG_KEY_IsCheckServiceFollowWhenOut) == GlobalVariables.CommonStringTrue;
                AutoCreatePaymentTransactions = GetValue(KEY__AutoCreatePaymentTransactions);
                OptionTreatmentEndTypeIsTransfer = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(KEY_TreatmentEndTypeIsTransfer);
                HospitalizationReasonRequiredByPatientCode = GetValue(KEY__HospitalizationReasonRequiredByPatientCode);
                IsRequiredTemperatureOption = GetValue(KEY__IsRequiredTemperatureOption) == GlobalVariables.CommonStringTrue;
                RequiredAddressOption = GetValue(KEY__RequiredAddressOption) == GlobalVariables.CommonStringTrue;
                MustChooseSeviceExamOption = GetValue(KEY__MustChooseSeviceExam);
                RequiredWeightHeight_Option = GetValue(DHST_REQUIRED_OPTION);
                CheckIcdWhenSave = GetValue(CHECK_ICD_WHEN_SAVE);
                isDisablePartExamByExecutor = GetValue(DISABLE_PART_EXAM_BY_EXECUTOR) == GlobalVariables.CommonStringTrue;
                enableExamtype = GetValue(CONFIG_KEY_IS_CHECK_ENABLE_EXAM_TYPE);
                IsloadIcdFromExamServiceExecute = GetValue(CONFIG_KEY__IsloadIcdFromExamServiceExecute) == GlobalVariables.CommonStringTrue;
                IsAutoExitAfterFinish = GetValue(CONFIG_KEY_AutoExitAfterFinish) == GlobalVariables.CommonStringTrue;
                IsAutoCheckPrintHospitalizeExam = GetValue(CONFIG_KEY_AutoCheckPrintHospitalizeExam) == GlobalVariables.CommonStringTrue;
                IsCheckPreviousPrescription = (GetValue(CONFIG_KEY__IS_CHECK_PREVIOUS_PRESCRIPTION) == GlobalVariables.CommonStringTrue);
                IsNotRequiredFee = GetValue(CONFIG_KEY_DefaultIsNotRequireFeeForNonBhyt) == GlobalVariables.CommonStringTrue;
                IsAutoSetExamInforByPreviousTreatmentInCaseOfOutPatient = GetValue(CONFIG_KEY__IS_AUTO_SET_EXAM_INFO_BY_PREVIOUS_TREATMENT_IN_CASE_OF_OUT_PATIENT) == GlobalVariables.CommonStringTrue;
                IsAllowPrintNoMedicine = GetValue(CONFIG_KEY__IS_ALLOW_PRINT_NO_MEDICINE) == GlobalVariables.CommonStringTrue;
                MPS_PrintPrescription = GetValue(CONFIG_KEY__MPS_PrintPrescription);
                IsAutoSetIcdWhenFinishInOtherExam = GetValue(AUTO_SET_ICD_WHEN_FINISH_IN_ADDITION_EXAM) == GlobalVariables.CommonStringTrue;
                IsFormClosingOption = GetValue(CONFIG_KEY__FormClosingOption) == GlobalVariables.CommonStringTrue;
                ModuleLinkApply = GetValue(CONFIG_KEY__ModuleLinkApply);
                executeRoomPaymentOption = GetValue(EXECUTE_ROOM_PAYMENT_OPTION);
                AssignServiceSimultaneityOption = GetValue(ASSIGN_SERVICE_SIMULTANEITY_OPTION);
                AssignSimultaneityOption = GetValue(ASSIGN_SIMULTANEITY_OPTION);
                terminalSystemAddress = GetValue(TERMINAL_SYTEM_ADDRESS);
                terminalSystemSecureKey = GetValue(TERMINAL_SYTEM_SECURE_KEY);
                RequiredTreatmentMethodOption = GetValue(CONFIG_KEY__IS_REQUIRED_TREATMENT_METHOD_OPTION);
                AutoCheckIcd = GetValue(CONFIG_KEY__ICD_GENERA_KEY);
                IsRequiredPathologicalProcessTransferPatientBHYT = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(KEY_IsRequiredPathologicalProcessTransferPatientBHYT) == GlobalVariables.CommonStringTrue;
                PathologicalProcessOption = int.Parse(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(KEY_PathologicalProcessOption) ?? "0");
                AllowBhxhLeaveOver30days = GetValue(KEY_AllowBhxhLeaveOver30days);
                IsEnableEditStartTime = GetValue(KEY_HIS_DESKTOP_PLUGINS_EXAMSERVICEREQEXECUTE_ISENABLEEDITSTARTTIME);
                IsAutoFillInformationAndIcdExam = GetValue(KEY_HIS_DESKTOP_PLUGINS_EXAMSERVICEREQEXECUTE_ISAUTOFILLINFORMATIONANDICDEXAM);
                AllowManyTreatmentOpeningOption = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(KEY__MOS_TREATMENT_ALLOW_MANY_TREATMENT_OPENING_OPTION);
                NotUpdateExecuteLoginNameWhenFinishExam = GetValue(KEY_MOS_HIS_SERVICE_REQ_NOT_UPDATE_EXECUTE_LOGINNAME_WHEN_FINISH_EXAM);
                HisDesktopPluginsRegisterV2RequestSkinCare = GetValue(KEY_HIS_DESKTOP_PLUGINS_REGISTER_V2_REQUEST_SKIN_CARE);
                IsEnableEmergencyClassify = GetValue(KEY_MOS_HIS_TREATMENT_EMERGENCY_CLASSIFY) == GlobalVariables.CommonStringTrue;
                IsCheckMimsPregnancyLactation = GetValue(KEY_HIS_DESKTOP_MIMS_IS_CHECK_PREGNANCY_LACTATION) == GlobalVariables.CommonStringTrue;

                string rawSevereCodes = GetValue(KEY__MOS_HIS_SEVERE_ILLNESS_INFO_MUST_INPUT_SEVERE_ILLNESS_HOME_CODES);
                MustInputSevereIllnessHomeCodes = string.IsNullOrWhiteSpace(rawSevereCodes)
                    ? new List<string>()
                    : rawSevereCodes.Split(',').Select(o => (o ?? "").Trim().ToUpper()).Where(o => o.Length > 0).ToList();

                // PTTK 4.1.2 - Kiểm tra số ICD phụ ra viện
                IsCheckSubIcdExceedLimit = GetValue(KEY_IsCheckSubIcdExceedLimit);
                int parsedMaxCount;
                IcdSubMaxCount = (int.TryParse(GetValue(KEY_IcdSubMaxCount), out parsedMaxCount) && parsedMaxCount > 0)
                    ? parsedMaxCount
                    : ICD_SUB_MAX_COUNT_DEFAULT;

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

    }
}
