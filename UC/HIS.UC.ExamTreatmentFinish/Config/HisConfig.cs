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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.ExamTreatmentFinish.Config
{
    class HisConfig
    {
        private const string KEY__HIS_DESKTOP_PLUGINS_TREATMENTFINISH_ENDDEPARTMENTSUBSHEADOPTION = "HIS.Desktop.Plugins.TreatmentFinish.EndDepartmentSubsHeadOption";

        public const string KEY__MOS_HIS_TREATMENT_AUTO_CREATE_WHEN_APPOINTMENT = "MOS.HIS_TREATMENT.AUTO_CREATE_WHEN_APPOINTMENT";
        public const string KEY__MOS_HIS_SERVICE_REQ_NUM_ORDER_ISSUE_OPTION = "MOS.HIS_SERVICE_REQ.NUM_ORDER_ISSUE_OPTION";
        public const string KEY__MOS_TREATMENT_ALLOW_MANY_TREATMENT_OPENING_OPTION = "MOS.TREATMENT.ALLOW_MANY_TREATMENT_OPENING_OPTION";


        private const string TREATMENT_END___APPOINTMENT_TIME_DEFAULT_KEY = "EXE.HIS_TREATMENT_END.APPOINTMENT_TIME_DEFAULT";
        private const string PRESCRIPTION_TIME_AND_APPOINTMENT_TIME_KEY = "HIS.Desktop.Plugins.ExamTreatmentFinish.APPOINTMENT_TIME";
        const string IS__TRUE = "1";

        private const string DefaultCheckedCheckboxIssueOutPatientStoreCodeSTR = "HIS.Desktop.Plugins.TreatmentFinish.DefaultCheckedCheckboxCreateOutPatientMediRecord";
        private const string EnableCheckboxIssueOutPatientStoreCodeSTR = "HIS.Desktop.Plugins.TreatmentFinish.EnableCheckboxCreateOutPatientMediRecord";
        private const string MAX_OF_APPOINTMENT_DAYS = "MOS.HIS_TREATMENT.MAX_OF_APPOINTMENT_DAYS";
        private const string WARNING_OPTION_WHEN_EXCEEDING_MAX_OF_APPOINTMENT_DAYS = "MOS.HIS_TREATMENT.WARNING_OPTION_WHEN_EXCEEDING_MAX_OF_APPOINTMENT_DAYS";

        private const string MUST_CHOOSE_SERVICE = "HIS.Desktop.Plugins.TreatmentFinish.MustChooseSeviceInCaseOfAppointment";

        private const string EXPORT_XML_COLLINEAR= "HIS.Desktop.Plugins.TreatmentFinish.AutoCheckAndDisable.ExportXmlCollinear";
        private const string CONFIG__USING_EXAM_SUB_ICD_WHEN_FINISH = "MOS.HIS_TREATMENT.IS_USING_EXAM_SUB_ICD_WHEN_FINISH";
        private const string CHECK_ICD_WHEN_SAVE = "HIS.Desktop.Plugins.CheckIcdWhenSave";
        internal static string CheckIcdWhenSave;
        internal static string OptionSubIcdWhenFinish;
        internal static long treatmentEndAppointmentTimeDefault;
        internal static bool AppointmentTimeDefault;
        internal static bool IsCheckedCheckboxIssueOutPatientStoreCode;
        internal static bool IsEnableCheckboxIssueOutPatientStoreCode;
        internal static long? MaxOfAppointmentDays;
        internal static long? WarningOptionWhenExceedingMaxOfAppointmentDays;

        internal static string MustChooseSeviceInCaseOfAppointment;
        internal static string NumOrderIssueOption;
        internal static bool IsExportXmlCollinear;
        internal static string AutoCreateWhenAppoinment;
        internal static string AllowManyOpeningOption;

        internal static string ENDDEPARTMENTSUBSHEADOPTION;


        internal static void GetConfig()
        {
            try
            {
                ENDDEPARTMENTSUBSHEADOPTION = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(KEY__HIS_DESKTOP_PLUGINS_TREATMENTFINISH_ENDDEPARTMENTSUBSHEADOPTION);

                CheckIcdWhenSave = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(CHECK_ICD_WHEN_SAVE);
                OptionSubIcdWhenFinish = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(CONFIG__USING_EXAM_SUB_ICD_WHEN_FINISH);
                AutoCreateWhenAppoinment = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(KEY__MOS_HIS_TREATMENT_AUTO_CREATE_WHEN_APPOINTMENT);
                NumOrderIssueOption = "1";//HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(KEY__MOS_HIS_SERVICE_REQ_NUM_ORDER_ISSUE_OPTION);
                MustChooseSeviceInCaseOfAppointment = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(MUST_CHOOSE_SERVICE);

                treatmentEndAppointmentTimeDefault = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<long>(TREATMENT_END___APPOINTMENT_TIME_DEFAULT_KEY);
                AppointmentTimeDefault = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(PRESCRIPTION_TIME_AND_APPOINTMENT_TIME_KEY) == IS__TRUE;
                AllowManyOpeningOption = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(KEY__MOS_TREATMENT_ALLOW_MANY_TREATMENT_OPENING_OPTION);
                IsCheckedCheckboxIssueOutPatientStoreCode = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(DefaultCheckedCheckboxIssueOutPatientStoreCodeSTR) == IS__TRUE;
                IsEnableCheckboxIssueOutPatientStoreCode = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(EnableCheckboxIssueOutPatientStoreCodeSTR) == IS__TRUE;
                IsExportXmlCollinear = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(EXPORT_XML_COLLINEAR) == IS__TRUE;
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
            
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
