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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.TreatmentFinish
{
    public class ConfigKeyCFG
    {
        public const string KEY__MOS_HIS_SERVICE_REQ_NUM_ORDER_ISSUE_OPTION = "MOS.HIS_SERVICE_REQ.NUM_ORDER_ISSUE_OPTION";
        /// <summary>
        /// Giá trị loại ra viện khi kết thúc điều trị tại màn hình XỬ LÝ KHÁM, KÊ ĐƠN- Giá trị 0 hoặc không khai báo: mặc định để trống- Giá trị 1: mặc định là hẹn khám- Giá trị 2: Mặc định cấp toa cho về
        /// </summary>
        public const string TREATMENT_END___TREATMENT_END_TYPE_DEFAULT = "HIS.Desktop.Plugins.TreatmentFinish.TreatmentEndTypeDefault";

        public const string AUTO_CHECK_PRINT_BORDEREAU_BY_PATIENT_TYPE = "HIS.DESKTOP.AUTO_CHECK_PRINT_BORDEREAU_BY_PATIENT_TYPE";

        /// <summary>
        /// Thời gian hẹn khám : 1, ưu tiên thời gian hẹn khám theo hạn dùng của đơn thuốc
        /// </summary>
        public const string PRESCRIPTION_TIME_AND_APPOINTMENT_TIME_KEY = "HIS.Desktop.Plugins.TreatmentFinish.APPOINTMENT_TIME";

        /// <summary>
        /// Mặc định thời gian hẹn khám, tính theo ngày
        /// </summary>
        public const string TREATMENT_END___APPOINTMENT_TIME_DEFAULT_KEY = "EXE.HIS_TREATMENT_END.APPOINTMENT_TIME_DEFAULT";

        const string IS__TRUE = "1";

        private const string DefaultCheckedCheckboxIssueOutPatientStoreCodeSTR = "HIS.Desktop.Plugins.TreatmentFinish.DefaultCheckedCheckboxCreateOutPatientMediRecord";
        private const string EnableCheckboxIssueOutPatientStoreCodeSTR = "HIS.Desktop.Plugins.TreatmentFinish.EnableCheckboxCreateOutPatientMediRecord";
        private const string MAX_OF_APPOINTMENT_DAYS = "MOS.HIS_TREATMENT.MAX_OF_APPOINTMENT_DAYS";
        private const string WARNING_OPTION_WHEN_EXCEEDING_MAX_OF_APPOINTMENT_DAYS = "MOS.HIS_TREATMENT.WARNING_OPTION_WHEN_EXCEEDING_MAX_OF_APPOINTMENT_DAYS";
        private const string KEY__TreatmentFinish__MustChooseSeviceInCaseOfAppointment = "HIS.Desktop.Plugins.TreatmentFinish.MustChooseSeviceInCaseOfAppointment";
        
        /// <summary>
        /// Cấu hình tự động tích vào check xuất XML thông tuyến tại chức năng kết thúc điều trị
        /// </summary>
        public const string AUTO_CHECK_AND_DISABLE_EXPORT_XML_COLLINEAR = "HIS.Desktop.Plugins.TreatmentFinish.AutoCheckAndDisable.ExportXmlCollinear";

        public const string AUTO_CREATE_WHEN_APPOINTMENT = "MOS.HIS_TREATMENT.AUTO_CREATE_WHEN_APPOINTMENT";
        private const string CONFIG_KEY__PATIENT_TYPE_CODE__BHYT = "MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BHYT";//Doi tuong BHYT
        private const string KEY__PATIENT_TYPE_CODE__HOSPITAL_FEE = "MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.HOSPITAL_FEE";
        private const string KEY__ALLOW_MANY_TREATMENT_OPENING_OPTION = "MOS.TREATMENT.ALLOW_MANY_TREATMENT_OPENING_OPTION";
        private const string KEY__EndDepartmentSubsHeadOption = "HIS.Desktop.Plugins.TreatmentFinish.EndDepartmentSubsHeadOption";
        internal static string EndDepartmentSubsHeadOption;
        internal static string AllowManyTreatmentOpeningOption;
        internal static long treatmentEndAppointmentTimeDefault;
        internal static bool AppointmentTimeDefault;
        internal static bool IsCheckedCheckboxIssueOutPatientStoreCode;
        internal static bool IsEnableCheckboxIssueOutPatientStoreCode;
        internal static long? MaxOfAppointmentDays;
        internal static long? WarningOptionWhenExceedingMaxOfAppointmentDays;

        internal static bool MustChooseSeviceInCaseOfAppointment;
        internal static string NumOrderIssueOption;

        internal static bool AutoCheckAndDisableExportXmlCollinear;
        internal static string AutoCreateWhenAppointment;
        internal static string PatientTypeCode__BHYT;
        internal static long PatientTypeId__BHYT;
        internal static string PatientTypeCode__HospitalFee;
        internal static long PatientTypeId__HospitalFee;
        internal static string PatientTypeName__HospitalFee;

        internal static void GetConfig()
        {
            try
            {
                EndDepartmentSubsHeadOption = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(KEY__EndDepartmentSubsHeadOption);
                AllowManyTreatmentOpeningOption = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(KEY__ALLOW_MANY_TREATMENT_OPENING_OPTION);
                NumOrderIssueOption = "1";//HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(KEY__MOS_HIS_SERVICE_REQ_NUM_ORDER_ISSUE_OPTION);
                treatmentEndAppointmentTimeDefault = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<long>(TREATMENT_END___APPOINTMENT_TIME_DEFAULT_KEY);
                AppointmentTimeDefault = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(PRESCRIPTION_TIME_AND_APPOINTMENT_TIME_KEY) == IS__TRUE;
                IsCheckedCheckboxIssueOutPatientStoreCode = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(DefaultCheckedCheckboxIssueOutPatientStoreCodeSTR) == IS__TRUE;
                MustChooseSeviceInCaseOfAppointment = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(KEY__TreatmentFinish__MustChooseSeviceInCaseOfAppointment) == IS__TRUE;
                IsEnableCheckboxIssueOutPatientStoreCode = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(EnableCheckboxIssueOutPatientStoreCodeSTR) == IS__TRUE;
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

                AutoCheckAndDisableExportXmlCollinear = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(AUTO_CHECK_AND_DISABLE_EXPORT_XML_COLLINEAR) == IS__TRUE;
                AutoCreateWhenAppointment = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(AUTO_CREATE_WHEN_APPOINTMENT);
                PatientTypeCode__BHYT = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(CONFIG_KEY__PATIENT_TYPE_CODE__BHYT);
                PatientTypeId__BHYT = GetPatientTypeByCode(PatientTypeCode__BHYT).ID;
                PatientTypeCode__HospitalFee = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(KEY__PATIENT_TYPE_CODE__HOSPITAL_FEE);
                PatientTypeId__HospitalFee = GetPatientTypeByCode(PatientTypeCode__HospitalFee).ID;
                PatientTypeName__HospitalFee = GetPatientTypeByCode(PatientTypeCode__HospitalFee).PATIENT_TYPE_NAME;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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

    }
}
