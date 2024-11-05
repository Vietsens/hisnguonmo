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
        public const string REQUIRED_PULSE_BLOOD_PRESSURE = "HIS.UC.DHST__REQUIRED_PULSE_BLOOD_PRESSURE";

        private const string CONFIG_KEY__IsloadIcdFromExamServiceExecute = "HIS.Desktop.Plugins.IsloadIcdFromExamServiceExecute";
        internal static bool IsloadIcdFromExamServiceExecute;
        private const string CONFIG_KEY__ICD_GENERA_KEY = "HIS.Desktop.Plugins.AutoCheckIcd";
        // tự động tắt màn hình xử lý khám sau khi kết thúc
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
        internal static void LoadConfig()
        {
            try
            {
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
                terminalSystemAddress = GetValue(TERMINAL_SYTEM_ADDRESS);
                terminalSystemSecureKey = GetValue(TERMINAL_SYTEM_SECURE_KEY);
                RequiredTreatmentMethodOption = GetValue(CONFIG_KEY__IS_REQUIRED_TREATMENT_METHOD_OPTION);
                AutoCheckIcd = GetValue(CONFIG_KEY__ICD_GENERA_KEY);
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
