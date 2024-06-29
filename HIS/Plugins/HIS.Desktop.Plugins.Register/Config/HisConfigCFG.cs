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
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Register.Config
{
    internal class HisConfigCFG
    {
        //private const string CONFIG_KEY__IS_AUTO_FILL_DATA_RECENT_SERVICE_ROOM = "HIS.IS_AUTO_FILL_DATA_RECENT_SERVICE_ROOM";
        private const string CONFIG_KEY__IS_CHECK_EXAM_HISTORY_TODAY = "HIS.Desktop.Plugins.Register.IS_CHECK_EXAM_HISTORY_TODAY";
        private const string CONFIG_KEY__IS_CHECK_HEIN_CARD = "HIS.Desktop.Plugins.Register.IsCheckHeinCard";
        private const string CONFIG_KEY__IS_CHECK_PREVIOUS_DEBT = "MOS.HIS_TREATMENT.IS_CHECK_PREVIOUS_DEBT";
        private const string CONFIG_KEY__IS_CHECK_PREVIOUS_PRESCRIPTION = "MOS.HIS_TREATMENT.IS_CHECK_PREVIOUS_PRESCRIPTION";
        private const string CONFIG_KEY__IS_DEFAULT_RIGHT_ROUTE_TYPE = "HIS.Desktop.Plugins.Register.IsDefaultRightRouteType";
        private const string CONFIG_KEY__VISIBILITY_CONTROL = "HIS.HIS_DESKTOP_REGISTER.VISIBILITY_CONTROL_FOR_TIM";
        private const string CONFIG_KEY__NOT_CHECK_EXPIRED_IS_SHOW = "HIS.DESKTOP.REGISTER.HEIN_CARD.NOT_CHECK_EXPIRED.IS_SHOW";
        private const string CONFIG_KEY__ICD_GENERATE = "HIS.Desktop.Plugins.AutoCheckIcd";
        private const string CONFIG_KEY__PATIENT_TYPE_CODE__BHYT = "MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BHYT";//Doi tuong BHYT
        private const string CONFIG_KEY__PATIENT_TYPE_CODE__QN = "MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.QN";//Doi tuong quan nhan
        private const string CONFIG_KEY__PATIENT_TYPE_CODE__KSK = "MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.KSK";//Doi tuong KSK
        private const string CONFIG_KEY__IS_PRINT_AFTER_SAVE__KEY = "EXE.SERVICE_REQUEST_REGISTER.IS_PRINT_AFTER_SAVE";
        private const string CONFIG_KEY__IS_VISIBLE_BILL__KEY = "EXE.SERVICE_REQUEST_REGISTER.IS_VISIBLE_BILL";
        private const string CONFIG_KEY__MOS__HIS_PATIENT__MUST_HAVE_NCS_INFO_FOR_CHILD = "MOS.HIS_PATIENT.MUST_HAVE_NCS_INFO_FOR_CHILD";
        private const string CONFIG_KEY__HIS_DEPOSIT__DEFAULT_PRICE_FOR_BHYT_OUT_PATIENT = "HIS_RS.HIS_DEPOSIT.DEFAULT_PRICE_FOR_BHYT_OUT_PATIENT";//tính tiền dịch vụ cần tạm ứng(ct MOS.LibraryHein.Bhyt.BhytPriceCalculator.DefaultPriceForBhytOutPatient) ở chức năng tạm ứng dịch vụ theo dịch vụ.
        private const string CONFIG_KEY__GENDER_CODE_BASE = "RAE.HIS_GENDER_CODE__BASE";
        private const string CONFIG_KEY__HIS_CAREER_CODE__BASE = "EXE.HIS_CAREER_CODE__BASE";
        private const string CONFIG_KEY__ETHNIC_CODE_BASE = "EXE.ETHNIC_CODE_BASE";
        private const string CONFIG_KEY__NATIONAL_CODE_BASE = "EXE.NATIONAL_CODE_BASE";
        private const string CONFIG_KEY__CAREER_CODE__UNDER_6_AGE = "EXE.HIS_CAREER_CODE__UNDER_6_AGE";
        private const string CONFIG_KEY__CAREER_CODE__HOC_SINH = "HIS.DESKTOP.REGISTER.HIS_CAREER.CARRER_CODE_HS";
        private const string HIS_DESKTOP_REGISER__EXECUTE_ROOM_SHOW = "HIS.HIS_DESKTOP_REGISTER.EXECUTE_ROOM_CODE.SHOW";
        private const string HIS_UC_UCHein_IsTempQN = "HIS.UC.UCHein.IsTempQN";
        private const string HIS_UC_UCHein_IS_OBLIGATORY_TRANFER_MEDI_ORG = "HIS.UC.UCHein.IS_OBLIGATORY_TRANFER_MEDI_ORG";
        private const string CONFIG_KEY__IS_USE_HID_SYNC = "CONFIG_KEY__IS_USE_HID_SYNC";
        private const string CONFIG_KEY__WarningOverExamBhyt = "HIS.Desktop.WarningOverExamBhyt";
        private const string DEFAULT_PATIENT_TYPE_CODE_IS_NOT_REQUIRE_EXAM_FEE = "HIS.DESKTOP.REGISTER__DEFAULT_PATIENT_TYPE_CODE_IS_NOT_REQUIRE_EXAM_FEE";

        const string valueString__true = "1";
        const int valueInt__true = 1;

        internal static string IsShowCheckExpired;
        internal static bool IsCheckHeinCard;
        internal static bool IsCheckPreviousDebt;
        internal static bool IsCheckPreviousPrescription;
        internal static string IsDefaultRightRouteType;
        internal static bool VisibilityControl;
        internal static bool IsSyncHID { get; set; }

        /// <summary>
        /// - Cấu hình chế độ kiểm tra thẻ bhyt trên cổng bhxh. Đặt 1 là tự động kiểm tra khi tìm thấy bệnh nhân cũ, đặt số khác là không kiểm tra.
        /// </summary>
        internal static bool IsCheckExamHistory;
        internal static string AutoCheckIcd;
        internal static string PatientTypeCode__BHYT;
        internal static string PatientTypeCode__KSK;
        internal static string PatientTypeCode__QN;
        internal static long PatientTypeId__BHYT;
        internal static long PatientTypeId__KSK;
        internal static long PatientTypeId__QN;
        internal static string IsPrintAfterSave;
        internal static string IsVisibleBill;
        internal static bool IsSetDefaultDepositPrice;
        internal static List<string> ExecuteRoomShow;
        internal static string CheckTempQN;
        internal static bool IsObligatoryTranferMediOrg;
        internal static bool IsWarningOverExamBhyt;

        internal static List<string> PatientCodeIsNotRequireExamFee;
        internal static List<long> PatientIdIsNotRequireExamFee;

        /// <summary>
        /// true - Bắt buộc nhập thông tin Người nhà, quan hệ, địa chỉ với trẻ em nhỏ hơn 6t
        /// false - Không bắt buộc nhập thông tin Người nhà, quan hệ, địa chỉ với mọi đối tượng
        /// </summary>
        internal static bool MustHaveNCSInfoForChild;

        /// <summary>
        /// Cấu hình tự động fill yêu cầu - phòng khám gần nhất khi tìm bệnh nhân cũ
        /// Đặt 1 là tự động fill
        /// Mặc định là không tự động
        /// </summary>
        // internal static string IsAutoFillDataRecentServiceRoom;//Chuyen sang AppConfig

        internal static MOS.EFMODEL.DataModels.HIS_GENDER GenderBase;
        internal static MOS.EFMODEL.DataModels.HIS_CAREER CareerBase;
        internal static MOS.EFMODEL.DataModels.HIS_CAREER CareerHS;
        internal static MOS.EFMODEL.DataModels.HIS_CAREER CareerUnder6Age;
        internal static SDA.EFMODEL.DataModels.SDA_NATIONAL NationalBase;
        internal static SDA.EFMODEL.DataModels.SDA_ETHNIC EthinicBase;

        internal static void LoadConfig()
        {
            try
            {
                LogSystem.Debug("LoadConfig => 1");
                ExecuteRoomShow = GetListValue(HIS_DESKTOP_REGISER__EXECUTE_ROOM_SHOW);
                IsSetDefaultDepositPrice = (Inventec.Common.TypeConvert.Parse.ToInt32(GetValue(CONFIG_KEY__HIS_DEPOSIT__DEFAULT_PRICE_FOR_BHYT_OUT_PATIENT)) == valueInt__true);
                MustHaveNCSInfoForChild = (GetValue(CONFIG_KEY__MOS__HIS_PATIENT__MUST_HAVE_NCS_INFO_FOR_CHILD) == valueString__true);
                IsPrintAfterSave = GetValue(CONFIG_KEY__IS_PRINT_AFTER_SAVE__KEY);
                IsVisibleBill = GetValue(CONFIG_KEY__IS_VISIBLE_BILL__KEY);
                PatientTypeCode__BHYT = GetValue(CONFIG_KEY__PATIENT_TYPE_CODE__BHYT);
                PatientTypeCode__KSK = GetValue(CONFIG_KEY__PATIENT_TYPE_CODE__KSK);
                PatientTypeCode__QN = GetValue(CONFIG_KEY__PATIENT_TYPE_CODE__QN);
                PatientTypeId__BHYT = GetPatientTypeByCode(PatientTypeCode__BHYT).ID;
                PatientTypeId__KSK = GetPatientTypeByCode(PatientTypeCode__KSK).ID;
                PatientTypeId__QN = GetPatientTypeByCode(PatientTypeCode__QN).ID;
                CheckTempQN = GetValue(HIS_UC_UCHein_IsTempQN);
                IsObligatoryTranferMediOrg = (GetValue(HIS_UC_UCHein_IS_OBLIGATORY_TRANFER_MEDI_ORG) == valueString__true);
                AutoCheckIcd = GetValue(CONFIG_KEY__ICD_GENERATE);
                IsShowCheckExpired = GetValue(CONFIG_KEY__NOT_CHECK_EXPIRED_IS_SHOW);
                IsCheckHeinCard = (GetValue(CONFIG_KEY__IS_CHECK_HEIN_CARD) == valueString__true);
                IsCheckPreviousDebt = (GetValue(CONFIG_KEY__IS_CHECK_PREVIOUS_DEBT) == valueString__true);
                IsCheckPreviousPrescription = (GetValue(CONFIG_KEY__IS_CHECK_PREVIOUS_PRESCRIPTION) == valueString__true);
                IsDefaultRightRouteType = GetValue(CONFIG_KEY__IS_DEFAULT_RIGHT_ROUTE_TYPE);
                VisibilityControl = (GetValue(CONFIG_KEY__VISIBILITY_CONTROL) == valueString__true);
                // IsAutoFillDataRecentServiceRoom = GetValue(CONFIG_KEY__IS_AUTO_FILL_DATA_RECENT_SERVICE_ROOM);
                IsCheckExamHistory = (GetValue(CONFIG_KEY__IS_CHECK_EXAM_HISTORY_TODAY) == valueString__true);
                GenderBase = GetGenderByCode(GetValue(CONFIG_KEY__GENDER_CODE_BASE));
                CareerBase = GetCareerByCode(GetValue(CONFIG_KEY__HIS_CAREER_CODE__BASE));
                CareerHS = GetCareerByCode(GetValue(CONFIG_KEY__CAREER_CODE__HOC_SINH));
                CareerUnder6Age = GetCareerByCode(GetValue(CONFIG_KEY__CAREER_CODE__UNDER_6_AGE));
                EthinicBase = GetEthnicByCode(GetValue(CONFIG_KEY__ETHNIC_CODE_BASE));
                NationalBase = GetNationalByCode(GetValue(CONFIG_KEY__NATIONAL_CODE_BASE));
                IsSyncHID = (GetValue(CONFIG_KEY__IS_USE_HID_SYNC) == valueString__true);
                IsWarningOverExamBhyt = (GetValue(CONFIG_KEY__WarningOverExamBhyt) == valueString__true);

                string patientCodeIsNotRequireExamFeeCFG = GetValue(DEFAULT_PATIENT_TYPE_CODE_IS_NOT_REQUIRE_EXAM_FEE);
                PatientCodeIsNotRequireExamFee = new List<string>();
                PatientIdIsNotRequireExamFee = new List<long>();
                if (!String.IsNullOrEmpty(patientCodeIsNotRequireExamFeeCFG))
                {
                    string[] patientCodeIsNotRequireExamFeeArr = patientCodeIsNotRequireExamFeeCFG.Split('|');
                    if (patientCodeIsNotRequireExamFeeArr != null && patientCodeIsNotRequireExamFeeArr.Length > 0)
                    {
                        
                        foreach (var item in patientCodeIsNotRequireExamFeeArr)
                        {
                            HIS_PATIENT_TYPE hisPatientType = GetPatientTypeByCode(item);
                            if (hisPatientType != null)
                            {
                                PatientIdIsNotRequireExamFee.Add(hisPatientType.ID);
                                PatientCodeIsNotRequireExamFee.Add(hisPatientType.PATIENT_TYPE_CODE);
                            }
                        }
                    }
                }

                LogSystem.Debug("LoadConfig => 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        static MOS.EFMODEL.DataModels.HIS_CAREER GetCareerByCode(string code)
        {
            MOS.EFMODEL.DataModels.HIS_CAREER result = new MOS.EFMODEL.DataModels.HIS_CAREER();
            try
            {
                result = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_CAREER>().FirstOrDefault(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.CAREER_CODE == code);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result ?? new MOS.EFMODEL.DataModels.HIS_CAREER();
        }

        static SDA.EFMODEL.DataModels.SDA_ETHNIC GetEthnicByCode(string code)
        {
            SDA.EFMODEL.DataModels.SDA_ETHNIC result = new SDA.EFMODEL.DataModels.SDA_ETHNIC();
            try
            {
                result = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_ETHNIC>().FirstOrDefault(o => o.ETHNIC_CODE == code);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result ?? new SDA.EFMODEL.DataModels.SDA_ETHNIC();
        }

        static SDA.EFMODEL.DataModels.SDA_NATIONAL GetNationalByCode(string code)
        {
            SDA.EFMODEL.DataModels.SDA_NATIONAL result = new SDA.EFMODEL.DataModels.SDA_NATIONAL();
            try
            {
                result = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_NATIONAL>().FirstOrDefault(o => o.NATIONAL_CODE == code);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result ?? new SDA.EFMODEL.DataModels.SDA_NATIONAL();
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
                result = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>().FirstOrDefault(o => o.PATIENT_TYPE_CODE.ToLower() == code.ToLower().Trim());
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
