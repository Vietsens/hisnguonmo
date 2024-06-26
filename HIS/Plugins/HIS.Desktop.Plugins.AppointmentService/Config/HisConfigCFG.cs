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
using Inventec.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.AppointmentService.Config
{
    class HisConfigCFG
    {
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
        private const string CONFIG_KEY__Icd_Service_Allow_Update = "HIS.HIS_ICD_SERVICE.ALLOW_UPDATE";
        private const string Key__WarningOverCeiling__Exam__Out__In = "HIS.Desktop.Plugins.WarningOverCeiling.Exam__Out__In";
        private const string CONFIG_KEY__WARNING_OVER_TOTAL_PATIENT_PRICE = "HIS.Desktop.WarningOverTotalPatientPrice";
        private const string CONFIG_KEY__WARNING_OVER_TOTAL_PATIENT_PRICE__IS_CHECK = "HIS.Desktop.WarningOverTotalPatientPrice__IsCheck";
        private const string CONFIG_KEY__HIS_SERE_SERV__SET_PRIMARY = "MOS.HIS_SERE_SERV.IS_SET_PRIMARY_PATIENT_TYPE";

        private const string CONFIG_KEY__IS_DEFAULT_TRACKING = "HIS.Desktop.Plugins.AssignPrescription.IsDefaultTracking";
        internal const string SERVICE_HAS_PAYMENT_LIMIT_BHYT = "HIS.Desktop.Plugins.AssignService.ServiceHasPaymentLimitBHYT";
        internal const string AUTO_FILTER_ROW = "HIS.Desktop.Plugins.AssignService.AutoFilterRow";
        private const string CONFIG_KEY__USING_SERVER_TIME = "MOS.IS_USING_SERVER_TIME";


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
        internal static string IcdServiceAllowUpdate;
        internal static bool IsSearchAll;

        internal static string WarningOverTotalPatientPrice;
        internal static string WarningOverTotalPatientPrice__IsCheck;
        internal static string IsDefaultTracking;
        internal static string ServiceHasPaymentLimitBHYT;
        internal static string IsSetPrimaryPatientType;
        internal static string IsUsingServerTime;


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
                ShowRequestUser = GetValue(CONFIG_KEY__ShowRequestUser);
                IsSingleCheckservice = GetValue(CONFIG_KEY__IsSingleCheckservice);
                IsSearchAll = (GetValue(CONFIG_KEY__IsSearchAll) == "1");
                AutoFilterRow = GetValue(AUTO_FILTER_ROW);
                Inventec.Common.Logging.LogSystem.Info("AutoFilterRow " + AutoFilterRow);
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
                IsVisibleExecuteGroup = GetValue(CONFIG_KEY__IS_VISILBE_EXECUTE_GROUP_KEY);
                AutoCheckIcd = GetValue(CONFIG_KEY__ICD_GENERA_KEY);
                IcdServiceAllowUpdate = GetValue(CONFIG_KEY__Icd_Service_Allow_Update);
                WarningOverTotalPatientPrice = GetValue(CONFIG_KEY__WARNING_OVER_TOTAL_PATIENT_PRICE);
                WarningOverTotalPatientPrice__IsCheck = GetValue(CONFIG_KEY__WARNING_OVER_TOTAL_PATIENT_PRICE__IS_CHECK);
                IsDefaultTracking = GetValue(CONFIG_KEY__IS_DEFAULT_TRACKING);
                AssignPrintTEST = (GetValue(CONFIG_KEY__AssignServicePrintTEST) == "1");
                TreatmentTypeCode__Exam = GetTreatmentTypeById(IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM).TREATMENT_TYPE_CODE;
                TreatmentTypeCode__TreatIn = GetTreatmentTypeById(IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU).TREATMENT_TYPE_CODE;
                TreatmentTypeCode__TreatOut = GetTreatmentTypeById(IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNGOAITRU).TREATMENT_TYPE_CODE;
                ServiceHasPaymentLimitBHYT = GetValue(SERVICE_HAS_PAYMENT_LIMIT_BHYT);
                IsSetPrimaryPatientType = GetValue(CONFIG_KEY__HIS_SERE_SERV__SET_PRIMARY);
                IsUsingServerTime = GetValue(CONFIG_KEY__USING_SERVER_TIME);
                InitWarningOverCeiling();
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
