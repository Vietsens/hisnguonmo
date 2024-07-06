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
using Inventec.Common.LocalStorage.SdaConfig;
using Inventec.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HIS.Desktop.LocalStorage.ConfigApplication;

namespace HIS.Desktop.Plugins.CallPatientTypeAlter.Config
{
    internal class HisConfigCFG
    {
        private const string CONFIG_KEY__IS_AUTO_FILL_DATA_RECENT_SERVICE_ROOM = "HIS.Desktop.Plugins.Register.IsAutoFillDataRecentServiceRoom";
        private const string CONFIG_KEY__IS_CHECK_HEIN_CARD = "HIS.Desktop.Plugins.Register.IsCheckHeinCard";
        private const string CONFIG_KEY__IS_CHECK_PREVIOUS_DEBT = "MOS.HIS_TREATMENT.IS_CHECK_PREVIOUS_DEBT";
        private const string CONFIG_KEY__IS_CHECK_PREVIOUS_PRESCRIPTION = "MOS.HIS_TREATMENT.IS_CHECK_PREVIOUS_PRESCRIPTION";
        private const string CONFIG_KEY__IS_DEFAULT_RIGHT_ROUTE_TYPE = "HIS.Desktop.Plugins.Register.IsDefaultRightRouteType";
        private const string CONFIG_KEY__ICD_GENERATE = "HIS.Desktop.Plugins.AutoCheckIcd";
        private const string CONFIG_KEY__PATIENT_TYPE_CODE__BHYT = "MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BHYT";//Doi tuong BHYT
        private const string CONFIG_KEY__PATIENT_TYPE_CODE__QN = "MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.QN";//Doi tuong quan nhan
        private const string CONFIG_KEY__PATIENT_TYPE_CODE__KSK = "MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.KSK";//Doi tuong ksk
        private const string CONFIG_KEY__MOS__HIS_PATIENT__MUST_HAVE_NCS_INFO_FOR_CHILD = "MOS.HIS_PATIENT.MUST_HAVE_NCS_INFO_FOR_CHILD";
        private const string CONFIG_KEY__GENDER_CODE_BASE = "RAE.HIS_GENDER_CODE__BASE";
        private const string CONFIG_KEY__CAREER_CODE__UNDER_6_AGE = "EXE.HIS_CAREER_CODE__UNDER_6_AGE";
        private const string CONFIG_KEY__CAREER_CODE__HOC_SINH = "HIS.DESKTOP.REGISTER.HIS_CAREER.CARRER_CODE_HS";
        private const string HIS_DESKTOP_REGISER__EXECUTE_ROOM_SHOW = "HIS.HIS_DESKTOP_REGISTER.EXECUTE_ROOM_CODE.SHOW";
        private const string CONFIG_KEY__NOT_CHECK_EXPIRED_IS_SHOW = "HIS.DESKTOP.REGISTER.HEIN_CARD.NOT_CHECK_EXPIRED.IS_SHOW";
        private const string Key__WarningOverCeiling__Exam__Out__In = "HIS.Desktop.Plugins.WarningOverCeiling.Exam__Out__In";
        private const string HIS_UC_UCHein_IsTempQN = "HIS.UC.UCHein.IsTempQN";
        private const string HIS_UC_UCHein_IS_OBLIGATORY_TRANFER_MEDI_ORG = "HIS.UC.UCHein.IS_OBLIGATORY_TRANFER_MEDI_ORG";

        // tientv #8559
        private const string CONFIG_KEY__IS_CHECK_EXAM_HISTORY_TODAY = "HIS.Desktop.Plugins.Register.IS_CHECK_EXAM_HISTORY_TODAY";
        private const string CONFIG_KEY__HIS_DESKTOP__PLUGINS_AUTO_CHECK_HEIN_DATE_TO = "CONFIG_KEY__HIS_DESKTOP__PLUGINS_AUTO_CHECK_HEIN_DATE_TO";
        private const string CONFIG_KEY_MOS_SET_PRIMARY_PATIENT_TYPE = "MOS.HIS_SERE_SERV.IS_SET_PRIMARY_PATIENT_TYPE";
        private const string CONFIG_KEY_IsNotRequiredRightTypeInCaseOfHavingAreaCode = "HIS.Desktop.Plugins.Register.IsNotRequiredRightTypeInCaseOfHavingAreaCode";

        private const string IS_BLOCK_INVALID_BHYT = "HIS.Desktop.Plugins.IsBlockingInvalidBhyt";


        const string valueString__true = "1";
        const int valueInt__true = 1;

        // tientv #8559
        internal static bool IsCheckExamHistory;
        public static long CheDoTuDongCheckThongTinTheBHYT { get; set; }

        internal static string IsShowCheckExpired;
        internal static bool IsCheckHeinCard;
        internal static bool IsCheckPreviousDebt;
        internal static bool IsCheckPreviousPrescription;
        internal static string IsDefaultRightRouteType;
        internal static string AutoCheckIcd;
        internal static string PatientTypeCode__BHYT;
        internal static string PatientTypeCode__KSK;
        internal static string PatientTypeCode__QN;
        internal static long PatientTypeId__BHYT;
        internal static long PatientTypeId__KSK;
        internal static long PatientTypeId__QN;
        internal static string CheckTempQN;
        internal static bool IsObligatoryTranferMediOrg;
        internal static string ObligatoryTranferMediOrg;
        internal static string IsSetPrimaryPatientType;
        public static bool IsBlockingInvalidBhyt;

        public static decimal WarningOverCeiling__Exam { get; set; }
        public static decimal WarningOverCeiling__Out { get; set; }
        public static decimal WarningOverCeiling__In { get; set; }


        /// <summary>
        /// true - Bắt buộc nhập thông tin Người nhà, quan hệ, địa chỉ với trẻ em nhỏ hơn 6t
        /// false - Không bắt buộc nhập thông tin Người nhà, quan hệ, địa chỉ với mọi đối tượng
        /// </summary>
        internal static bool MustHaveNCSInfoForChild;

        public static bool IsNotRequiredRightTypeInCaseOfHavingAreaCode;

        internal static MOS.EFMODEL.DataModels.HIS_CAREER CareerHS;
        internal static MOS.EFMODEL.DataModels.HIS_CAREER CareerUnder6Age;

        internal static void LoadConfig()
        {
            try
            {
                LogSystem.Debug("LoadConfig => 1");
                IsNotRequiredRightTypeInCaseOfHavingAreaCode = GetValue(CONFIG_KEY_IsNotRequiredRightTypeInCaseOfHavingAreaCode) == valueString__true;
                MustHaveNCSInfoForChild = (GetValue(CONFIG_KEY__MOS__HIS_PATIENT__MUST_HAVE_NCS_INFO_FOR_CHILD) == valueString__true);
                PatientTypeCode__BHYT = GetValue(CONFIG_KEY__PATIENT_TYPE_CODE__BHYT);
                PatientTypeId__BHYT = GetPatientTypeByCode(PatientTypeCode__BHYT).ID;
                PatientTypeCode__KSK = GetValue(CONFIG_KEY__PATIENT_TYPE_CODE__KSK);
                PatientTypeId__KSK = GetPatientTypeByCode(PatientTypeCode__KSK).ID;
                AutoCheckIcd = GetValue(CONFIG_KEY__ICD_GENERATE);
                IsShowCheckExpired = GetValue(CONFIG_KEY__NOT_CHECK_EXPIRED_IS_SHOW);
                IsCheckHeinCard = (GetValue(CONFIG_KEY__IS_CHECK_HEIN_CARD) == valueString__true);
                IsCheckPreviousDebt = (GetValue(CONFIG_KEY__IS_CHECK_PREVIOUS_DEBT) == valueString__true);
                IsCheckPreviousPrescription = (GetValue(CONFIG_KEY__IS_CHECK_PREVIOUS_PRESCRIPTION) == valueString__true);
                CheckTempQN = GetValue(HIS_UC_UCHein_IsTempQN);
                ObligatoryTranferMediOrg = GetValue(HIS_UC_UCHein_IS_OBLIGATORY_TRANFER_MEDI_ORG);
                IsObligatoryTranferMediOrg = (ObligatoryTranferMediOrg == valueString__true);
                PatientTypeCode__QN = GetValue(CONFIG_KEY__PATIENT_TYPE_CODE__QN);
                PatientTypeId__QN = GetPatientTypeByCode(PatientTypeCode__QN).ID;
                IsDefaultRightRouteType = GetValue(CONFIG_KEY__IS_DEFAULT_RIGHT_ROUTE_TYPE);
                CareerHS = GetCareerByCode(GetValue(CONFIG_KEY__CAREER_CODE__HOC_SINH));
                CareerUnder6Age = GetCareerByCode(GetValue(CONFIG_KEY__CAREER_CODE__UNDER_6_AGE));
                IsSetPrimaryPatientType = GetValue(CONFIG_KEY_MOS_SET_PRIMARY_PATIENT_TYPE);
                // tientv #8559
                IsCheckExamHistory = (GetValue(CONFIG_KEY__IS_CHECK_EXAM_HISTORY_TODAY) == valueString__true);
                IsBlockingInvalidBhyt = (GetValue(IS_BLOCK_INVALID_BHYT) == valueString__true);

                try
                {
                    CheDoTuDongCheckThongTinTheBHYT = ConfigApplicationWorker.Get<long>(CONFIG_KEY__HIS_DESKTOP__PLUGINS_AUTO_CHECK_HEIN_DATE_TO);
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }

                LogSystem.Debug("LoadConfig => 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
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

        static MOS.EFMODEL.DataModels.HIS_CAREER GetCareerByCode(string code)
        {
            MOS.EFMODEL.DataModels.HIS_CAREER result = new MOS.EFMODEL.DataModels.HIS_CAREER();
            try
            {
                result = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_CAREER>().FirstOrDefault(o => o.CAREER_CODE == code);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result ?? new MOS.EFMODEL.DataModels.HIS_CAREER();
        }

        static MOS.EFMODEL.DataModels.HIS_GENDER GetGenderByCode(string code)
        {
            MOS.EFMODEL.DataModels.HIS_GENDER result = new MOS.EFMODEL.DataModels.HIS_GENDER();
            try
            {
                result = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_GENDER>().FirstOrDefault(o => o.GENDER_CODE == code);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result ?? new MOS.EFMODEL.DataModels.HIS_GENDER();
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

        private static List<string> GetListValue(string key)
        {
            try
            {
                return HisConfigs.Get<List<string>>(key);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return null;
        }
    }
}
