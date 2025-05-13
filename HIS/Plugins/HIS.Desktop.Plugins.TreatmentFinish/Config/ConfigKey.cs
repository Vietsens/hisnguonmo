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
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.TreatmentFinish.Config
{
    class ConfigKey
    {
        private const string KEY__HIS_DESKTOP_PLUGINS_TREATMENTFINISH_ENDDEAPRTMENTSUBSHEADOPTIOIN = "HIS.Desktop.Plugins.TreatmentFinish.EndDepartmentSubsHeadOption";

        public const string KEY__MOS_HIS_SERVICE_REQ_NUM_ORDER_ISSUE_OPTION = "MOS.HIS_SERVICE_REQ.NUM_ORDER_ISSUE_OPTION";
        internal const string SaveTemp = "HIS.Desktop.Plugins.TreatmentFinish.SaveTemp";
        internal const string TREATMENT_RESULT_CODE_DEFAULT_OF_EXAM = "MOS.HIS_TREATMENT_RESULT.TREATMENT_RESULT_CODE.DEFAULT_OF_EXAM";
        internal const string PATIENT_TYPE_CODE_BHYT = "MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BHYT";

        private const string TreatmentEndTypeDefault = "HIS.Desktop.Plugins.TreatmentFinish.TreatmentEndTypeDefault";
        private const string HIS_DESKTOP_ALLOW_PRINT_FINISH = "HIS.Desktop.AllowPrint.Finish";
        private const string ShowDoctor = "HIS.Desktop.Plugins.AssignConfig.ShowRequestUser";
        private const string MAX_OF_APPOINTMENT_DAYS = "MOS.HIS_TREATMENT.MAX_OF_APPOINTMENT_DAYS";
        private const string WARNING_OPTION_WHEN_EXCEEDING_MAX_OF_APPOINTMENT_DAYS = "MOS.HIS_TREATMENT.WARNING_OPTION_WHEN_EXCEEDING_MAX_OF_APPOINTMENT_DAYS";
        private const string KEY_CONFIG_XML2076_EXPORT_OPTION = "MOS.HIS_TREATMENT.XML2076.EXPORT_OPTION";


        internal const string WARNING_OVER_TOTAL_PATIENT_PRICE__IS_CHECK = "HIS.Desktop.WarningOverTotalPatientPrice__IsCheck";
        internal const string WARNING_OVER_TOTAL_PATIENT_PRICE = "HIS.Desktop.WarningOverTotalPatientPrice";
        internal const string MUST_SET_PROGRAM_WHEN_FINISHING_IN_PATIENT = "MOS.HIS_TREATMENT.MUST_SET_PROGRAM_WHEN_FINISHING_IN_PATIENT";
        internal const string materialInvoiceInfo = "MOS.HIS_TREATMENT.IS_WARNING_NO_INVOICE_INFO_MATERIAL_WHEN_FINISHING";

        private const string WarningOptionIncaseOfHavingUnsignDocument = "HIS.Desktop.Plugins.TreatmentFinish.WarningOptionIncaseOfHavingUnsignDocument";
        private const string AutoCheckExportXmlCollinear = "HIS.Desktop.Plugins.TreatmentFinish.AutoCheckAndDisable.ExportXmlCollinear";

        private const string CONFIG_KEY__IS_REQUIRED_TREATMENT_METHOD_OPTION = "HIS.Desktop.Plugins.TreatmentFinish.RequiredTreatmentMethodOption";
        internal const string IS__TRUE = "1";

        internal const string WARNING_OPTION = "HIS.DESKTOP.HIS_PATIENT_PROGRAM.NOT_HAS_EMR_COVER_TYPE.WARNING_OPTION";
        private const string SET_DEFAULT_TREATMENT_END_TYPE = "HIS.Desktop.Plugins.TreatmentFinish.SetDefaultTreatmentEndType";
        private const string TREATMENT_RESULT_DEFAULT = "HIS.Desktop.Plugins.TreatmentFinish.TreatmentResultDefault";
        private const string MUST_CHOOSE_INCASE_APPOINT = "HIS.Desktop.Plugins.TreatmentFinish.MustChooseSeviceInCaseOfAppointment";

        public const string KEY__MOS_HIS_TREATMENT_AUTO_CREATE_WHEN_APPOINTMENT = "MOS.HIS_TREATMENT.AUTO_CREATE_WHEN_APPOINTMENT";
        public const string KEY__MOS_HIS_PATIENT_TYPE_PATIENT_TYPE_CODE_HOSPITAL_FEE = "MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.HOSPITAL_FEE";
        public const string KEY__MOS_TREATMENT_ALLOW_MANY_TREATMENT_OPENING_OPTION = "MOS.TREATMENT.ALLOW_MANY_TREATMENT_OPENING_OPTION";

        private const string CHECKING_RATION_OPTION = "HIS.Desktop.Plugins.TreatmentFinish.CheckingRationOption";
        private const string KEY__SUBCLINICAL_RESULT_OPTION = "HIS.Desktop.Plugins.TreatmentFinish.SubclinicalResultOption";
        private const string KEY__TIME_TO_CREATE_NEW_TREATMENT_IN_NEW_MONTH = "HIS.Desktop.Plugins.TreatmentFinish.TimeToCreateNewTreatmentInNewMonth";

        private const string KEY__PATHOLOCICAL_PROCESS_OPTION = "HIS.Desktop.Plugins.TreatmentFinish.PathologicalProcessOption";
        private const string KEY__WARNING_UNFINISHED_SERVICE_OPTION = "HIS.Desktop.Plugins.TreatmentFinish.WarningUnfinishedServiceOption";
        private const string KEY__MustChooseSeviceExam = "HIS.Desktop.Plugins.TreatmentFinish.MustChooseSeviceExam.Option";
        private const string KEY__IsAllowTreatmentFinishDepartmentIsActiveFee = "HIS.Desktop.Plugins.TreatmentFinish.IsAllowTreatmentFinishDepartmentIsActiveFee";
        private const string KEY_TreatmentEndTypeIsTransfer = "HIS.Desktop.Plugins.TreatmentFinish.TreatmentEndTypeIsTransfer";
        private const string KEY_IsCheckSubIcdExceedLimit = "HIS.Desktop.Plugins.IsCheckSubIcdExceedLimit";
        private const string KEY_WarnNotRequiredCompleteHasNoSample = "HIS.Desktop.Plugins.TreatmentFinish.WarnNotRequiredCompleteHasNoSample";

        internal static string OptionTreatmentEndTypeIsTransfer;
        internal static string MustChooseSeviceExamOption;
        internal static string WarningUnfinishedServiceOption;
        internal static string PathologicalProcessOption;
        internal static string CheckingRationOption;
        internal static string SetDefaultTreatmentEndType;
        internal static string TreatmentResultDefault;
        internal static string TimeToCreateNewTreatmentInNewMonth;
        internal static long TreatmentEndTypeDefaultID { get; set; }
        internal static long ALOW_PRINT_FINISH { get; set; }
        internal static string IsShowDoctor { get; set; }
        internal static string PatienTypeCode_BHYT { get; set; }
        internal static bool IsWarningOverTotalPatientPrice;
        internal static decimal WarningOverTotalPatientPrice;
        internal static bool IsMustSetProgramWhenFinishingInPatient;
        internal static string IsAutoCheckExportXmlCollinear;
        internal static bool IsNoMaterialInvoiceInfo;
        internal static long? MaxOfAppointmentDays;
        internal static long? WarningOptionWhenExceedingMaxOfAppointmentDays;
        internal static string ExportXml2076Option { get; set; }
        internal static string MustChooseSeviceInCaseOfAppointment;
        internal static string WarnNotRequiredCompleteHasNoSample;


        internal static long WarningOption { get; set; }

        internal static string RequiredTreatmentMethodOption;
        internal static string NumOrderIssueOption;

        internal static string AutoCreateWhenAppointment;
        internal static string patientTypeCodeHospitalFee;
        internal static string SubclinicalResultOption;
        internal static string AllowManyOpeningOption;
        internal static string IsAllowTreatmentFinishDepartmentIsActiveFee;

        internal static string IsCheckSubIcdExceedLimit;

        internal static string ENDDEAPRTMENTSUBSHEADOPTIOIN;

        internal static void GetConfigKey()
        {
            try
            {
                ENDDEAPRTMENTSUBSHEADOPTIOIN = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(KEY__HIS_DESKTOP_PLUGINS_TREATMENTFINISH_ENDDEAPRTMENTSUBSHEADOPTIOIN);
                IsCheckSubIcdExceedLimit = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(KEY_IsCheckSubIcdExceedLimit);
                OptionTreatmentEndTypeIsTransfer = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(KEY_TreatmentEndTypeIsTransfer);
                MustChooseSeviceExamOption = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(KEY__MustChooseSeviceExam);
                WarningUnfinishedServiceOption = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(KEY__WARNING_UNFINISHED_SERVICE_OPTION);
                PathologicalProcessOption = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(KEY__PATHOLOCICAL_PROCESS_OPTION);
                CheckingRationOption = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(CHECKING_RATION_OPTION);
                NumOrderIssueOption = "1";
                AutoCreateWhenAppointment = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(KEY__MOS_HIS_TREATMENT_AUTO_CREATE_WHEN_APPOINTMENT);
                patientTypeCodeHospitalFee = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(KEY__MOS_HIS_PATIENT_TYPE_PATIENT_TYPE_CODE_HOSPITAL_FEE);
                AllowManyOpeningOption = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(KEY__MOS_TREATMENT_ALLOW_MANY_TREATMENT_OPENING_OPTION);
                SetDefaultTreatmentEndType = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(SET_DEFAULT_TREATMENT_END_TYPE);
                TreatmentResultDefault = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(TREATMENT_RESULT_DEFAULT);
                MustChooseSeviceInCaseOfAppointment = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(MUST_CHOOSE_INCASE_APPOINT);
                IsAutoCheckExportXmlCollinear = GetValue(AutoCheckExportXmlCollinear);
                PatienTypeCode_BHYT = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(PATIENT_TYPE_CODE_BHYT);
                TreatmentEndTypeDefaultID = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<long>(TreatmentEndTypeDefault);
                ALOW_PRINT_FINISH = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<long>(HIS_DESKTOP_ALLOW_PRINT_FINISH);
                IsShowDoctor = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(ShowDoctor);
                WarnNotRequiredCompleteHasNoSample = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(KEY_WarnNotRequiredCompleteHasNoSample);

                WarningOption = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<long>(WARNING_OPTION);

                IsWarningOverTotalPatientPrice = GetValue(WARNING_OVER_TOTAL_PATIENT_PRICE__IS_CHECK) == GlobalVariables.CommonStringTrue;
                WarningOverTotalPatientPrice = Inventec.Common.TypeConvert.Parse.ToDecimal(GetValue(WARNING_OVER_TOTAL_PATIENT_PRICE));
                IsMustSetProgramWhenFinishingInPatient = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(MUST_SET_PROGRAM_WHEN_FINISHING_IN_PATIENT) == IS__TRUE;
                IsNoMaterialInvoiceInfo = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(materialInvoiceInfo) == IS__TRUE;
                RequiredTreatmentMethodOption = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(CONFIG_KEY__IS_REQUIRED_TREATMENT_METHOD_OPTION);
                string maxDayStr = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(MAX_OF_APPOINTMENT_DAYS);
                if (!String.IsNullOrWhiteSpace(maxDayStr))
                {
                    MaxOfAppointmentDays = Inventec.Common.TypeConvert.Parse.ToInt64(maxDayStr);
                }
                else
                {
                    MaxOfAppointmentDays = null;
                }

                string maxDayOption = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(WARNING_OPTION_WHEN_EXCEEDING_MAX_OF_APPOINTMENT_DAYS);
                if (!String.IsNullOrWhiteSpace(maxDayOption))
                {
                    WarningOptionWhenExceedingMaxOfAppointmentDays = Inventec.Common.TypeConvert.Parse.ToInt64(maxDayOption);
                }
                else
                {
                    WarningOptionWhenExceedingMaxOfAppointmentDays = null;
                }
                TimeToCreateNewTreatmentInNewMonth = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(KEY__TIME_TO_CREATE_NEW_TREATMENT_IN_NEW_MONTH);
                SubclinicalResultOption = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(KEY__SUBCLINICAL_RESULT_OPTION);
                ExportXml2076Option = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(KEY_CONFIG_XML2076_EXPORT_OPTION);
                IsAllowTreatmentFinishDepartmentIsActiveFee = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(KEY__IsAllowTreatmentFinishDepartmentIsActiveFee);
                TreatmentEndCFG.GetConfig();
                CheckFinishTimeCFG.GetConfig();
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
