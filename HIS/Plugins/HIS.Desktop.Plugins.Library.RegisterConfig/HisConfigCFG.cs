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
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.RegisterConfig
{
    public class HisConfigCFG
    {
        private const string CONFIG_KEY__ModuleLinks = "HIS.Desktop.ApplyRestoreLayout.ModuleLinks";
        private const string CONFIG_KEY__PhoneRequired = "HIS.Desktop.Plugins.RegisterV2.PhoneRequired";
        private const string CONFIG_KEY__FocusExecuteRoomOption = "HIS.Desktop.Plugins.RegisterV2.FocusExecuteRoomOption";
        private const string CONFIG_KEY__IsNotAutoFocusOnExistsPatient = "HIS.Desktop.Plugins.Register.IsNotAutoFocusOnExistsPatient";
        /// <summary>
        /// Khai báo các mã đối tượng bệnh nhân ngăn cách bằng dấu phẩy. Trong trường hợp khai báo giá trị thì khi tiếp đón các đối tượng được khai báo sẽ mặc định tick vào checkbox "In phiếu khám" và các đối tượng không được khai báo sẽ mặc định bỏ tick
        /// </summary>
        private const string CONFIG_KEY__AutoCheckPrintExam__PatientTypeCode = "HIS.Desktop.Plugins.Register.AutoCheckPrintExam.PatientTypeCode";
        private const string CONFIG_KEY__UsingPatientTypeOfPreviousPatient = "HIS.Desktop.Plugins.Register.UsingPatientTypeOfPreviousPatient";
        private const string CONFIG_KEY__SetDefaultRequestRoomByExamRoomWhenAssigningService = "HIS.Desktop.Plugins.Register.SetDefaultRequestRoomByExamRoomWhenAssigningService";
        private const string CONFIG_KEY__MIS_MANUAL_IN_CODE = "MOS.HIS_TREATMENT.IS_MANUAL_IN_CODE";
        private const string CONFIG_KEY__IS_CHECK_EXAM_HISTORY_TODAY = "HIS.Desktop.Plugins.Register.IS_CHECK_EXAM_HISTORY_TODAY";
        private const string CONFIG_KEY__IS_CHECK_HEIN_CARD = "HIS.Desktop.Plugins.Register.IsCheckHeinCard";
        private const string CONFIG_KEY__IS_CHECK_PREVIOUS_DEBT = "MOS.HIS_TREATMENT.CHECK_PREVIOUS_DEBT_OPTION";
        private const string CONFIG_KEY__IS_CHECK_PREVIOUS_PRESCRIPTION = "MOS.HIS_TREATMENT.IS_CHECK_PREVIOUS_PRESCRIPTION";
        private const string CONFIG_KEY__IS_DEFAULT_RIGHT_ROUTE_TYPE = "HIS.Desktop.Plugins.Register.IsDefaultRightRouteType";
        private const string CONFIG_KEY__VISIBILITY_CONTROL = "HIS.HIS_DESKTOP_REGISTER.VISIBILITY_CONTROL_FOR_TIM";
        private const string CONFIG_KEY__NOT_CHECK_EXPIRED_IS_SHOW = "HIS.DESKTOP.REGISTER.HEIN_CARD.NOT_CHECK_EXPIRED.IS_SHOW";
        private const string CONFIG_KEY__ICD_GENERATE = "HIS.Desktop.Plugins.AutoCheckIcd";
        internal const string CONFIG_KEY__PATIENT_TYPE_CODE__BHYT = "MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BHYT";//Doi tuong BHYT
        private const string CONFIG_KEY__PATIENT_TYPE_CODE__KSK = "MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.KSK";//Doi tuong KSK
        private const string CONFIG_KEY__PATIENT_TYPE_CODE__QN = "MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.QN"; // Doi tuong Quan Nhan
        private const string CONFIG_KEY__HIS_TREATMENT_IS_CHECK_TODAY_FINISH_TREATMENT = "MOS.HIS_TREATMENT.IS_CHECK_TODAY_FINISH_TREATMENT";
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
        private const string HIS_UC_UCHein_IS_OBLIGATORY_TRANFER_MEDI_ORG = "HIS.UC.UCHein.IS_OBLIGATORY_TRANFER_MEDI_ORG";

        private const string CONFIG_KEY__IS_USE_HID_SYNC = "CONFIG_KEY__IS_USE_HID_SYNC";
        private const string CONFIG_KEY__WarningOverExamBhyt = "HIS.Desktop.WarningOverExamBhyt";
        private const string DEFAULT_PATIENT_TYPE_CODE_IS_NOT_REQUIRE_EXAM_FEE = "HIS.DESKTOP.REGISTER__DEFAULT_PATIENT_TYPE_CODE_IS_NOT_REQUIRE_EXAM_FEE";

        private const string CONFIG_KEY_CALL_PATIENT_NUM_ORDER_OPTION = "HIS.DESKTOP.CALL_PATIENT_CPA.OPTION";

        private const string HIS_UC_UCHein_IsTempQN = "HIS.UC.UCHein.IsTempQN";

        private const string HIS_DESKTOP_REGISER__EXAMINATION = "HIS.Desktop.Plugins.Register.IsCheckExamination";

        private const string HIS_DESKTOP_MOS_HR_ADDRESS = "MOS.HR.ADDRESS";

        private const string HIS_DESKTOP_REGISTER_VALIDATE__T_H_X = "HIS.DESKTOP.REGISTER.VALIDATE__T_H_X";

        private const string HIS_DESKTOP_REGISTER_VALIDATE__ETHNIC = "HIS.DESKTOP.REGISTER.VALIDATE__ETHNIC";

        private const string HIS_DESKTOP_REGISTER__ROOM_CODES__PATIENT_TYPE = "HIS_DESKTOP_REGISTER__ROOM_CODES__PATIENT_TYPE";

        private const string CONFIG_KEY_MOS_SET_PRIMARY_PATIENT_TYPE = "MOS.HIS_SERE_SERV.IS_SET_PRIMARY_PATIENT_TYPE";
        private const string CONFIG_KEY_MOS_HIS_SERVICE_REQ_RESERVED_NUM_ORDER = "MOS.HIS_SERVICE_REQ.RESERVED_NUM_ORDER";

        private const string CONFIG_KEY_HIS_DESKTOP_PLUGIN_REGISTER_BY_PASS_TEXT_BOX_ROOM_CODE = "HIS.Desktop.Plugins.Register.ByPassTextboxRoomCode";
        private const string CONFIG_KEY_IsNotRequiredRightTypeInCaseOfHavingAreaCode = "HIS.Desktop.Plugins.Register.IsNotRequiredRightTypeInCaseOfHavingAreaCode";
        private const string CONFIG_KEY_RelativesInforOption = "HIS.Desktop.Plugins.Register.RelativesInforOption";
        private const string CONFIG_KEY_IsShowingExamRoomInArea = "HIS.Desktop.Plugins.Register.IsShowingExamRoomInArea";
        private const string CONFIG_KEY_IsShowingExamRoomInDepartment = "HIS.Desktop.Plugins.Register.IsShowingExamRoomInDepartment";
        private const string CONFIG_KEY_AutoFocusToSavePrintAfterChoosingExam = "HIS.Desktop.Plugins.Register.AutoFocusToSavePrintAfterChoosingExam";
        private const string CONFIG_KEY_IsAutoShowTransferFormInCaseOfAppointment = "HIS.Desktop.Plugins.Register.IsAutoShowTransferFormInCaseOfAppointment";

        private const string IS_BLOCK_INVALID_BHYT = "HIS.Desktop.Plugins.IsBlockingInvalidBhyt";
        private const string IS_USING_RECOGNITION = "HIS.DESKTOP.VVN_KYC.IS_USING_RECOGNITION";


        private const string CONFIG_KEY_AUTO_CALL_REGISTER_REQ= "CONFIG_KEY__HIS_DESKTOP__REGISTER__TIME__AUTO___CALL_REGISTER_REQ";

        private const string CONFIG_KEY_NUM_ORDER_ISSUE_OPTION = "MOS.HIS_SERVICE_REQ.NUM_ORDER_ISSUE_OPTION";
        private const string CONFIG_KEY_IsRequiredToUpdateNewBhytCardInCaseOfExpiry = "HIS.Desktop.Plugins.RegisterV2.IsRequiredToUpdateNewBhytCardInCaseOfExpiry";

        private const string CONFIG_KEY__FormClosingOption = "HIS.Desktop.FormClosingOption";
        public static bool IsFormClosingOption;

        private const string CONFIG_KEY__ModuleLinkApply = "HIS.Desktop.FormClosingOption.ModuleLinkApply";
        public static string ModuleLinkApply;

        private const string CONFIG_KEY__IsAllowedRouteTypeByDefault = "HIS.Desktop.Plugins.IsAllowedRouteTypeByDefault";
        public static bool IsAllowedRouteTypeByDefault;

        private const string CONFIG_KEY__NotDisplayedRouteTypeOver = "HIS.Desktop.Plugins.Register.NotDisplayedRouteTypeOver";
        public static bool IsNotDisplayedRouteTypeOver;
        private const string CONFIG_KEY__SuggestCardHolderInformationByUsingPhoneNumber = "HIS.Desktop.Plugins.Register.SuggestCardHolderInformationByUsingPhoneNumber";
        private const string CONFIG_KEY__IsDefaultTreatmentTypeExam = "HIS.Desktop.Plugins.RegisterV2.IsDefaultTreatmentTypeExam";
        private const string CONFIG_KEY__IsNotAutoCheck5Y6M = "MOS.HIS_PATIENT_TYPE_ALTER.NOT_AUTO_CHECK_5_YEAR_6_MONTH";
        private const string CONFIG_KEY__InHospitalizationReasonRequired = "HIS.Desktop.Plugins.ExamServiceReqExecute.InHospitalizationReasonRequired";
        private const string CONFIG_KEY__EditOldPatientInformationOption = "HIS.Desktop.Plugins.RegisterV2.EditOldPatientInformationOption"; 
        private const string CONFIG_KEY__WarningInvalidCheckHistoryHeinCard = "HIS.Desktop.Plugins.Register.WarningInvalidCheckHistoryHeinCard";

        private const string CONFIG_KEY__CHECK_DUPLICATION = "MOS.HIS_PATIENT.CCCD_NUMBER.CHECK_DUPLICATION";
        public static string CHECK_DUPLICATION;
        public static string MODULELINKS;

        public static bool WarningInvalidCheckHistoryHeinCard;
        public static bool EditOldPatientInformationOption;

        public static bool InHospitalizationReasonRequired;
        public static bool IsNotAutoCheck5Y6M;
        public static bool IsDefaultTreatmentTypeExam;
        public static bool IsSuggestCardHolderInformationByUsingPhoneNumber;
        const string valueString__true = "1";
        const int valueInt__true = 1;

        /// <summary>
        /// + Nếu đặt là 1: Hiển thị popup và foucs vào ô cho phép nhập từ khóa tìm kiếm, muốn chọn 1 phòng thì nhấn phím mũi tên xuống sau đó chọn
        /// + Nếu đặt khác 1: Hiển thị popup và focus vào dòng đầu tiên của danh sách phòng, cho phép chọn phòng luôn, nếu muốn tìm kiếm thì phải dùng chuột click vào ô chọn sau đó mới nhập từ khóa tìm kiếm
        /// </summary>
        public static string FocusExecuteRoomOption;
        public static bool IsNotAutoFocusOnExistsPatient;
        public static bool UsingPatientTypeOfPreviousPatient;
        public static bool SetDefaultRequestRoomByExamRoomWhenAssigningService;
        public static List<string> ReservedNumOders;//Danh sach phong dc chon dttt
        public static string ReservedNumOder;
        public static string CheckTempQN;
        public static string IsShowCheckExpired;
        public static bool IsCheckHeinCard;
        public static string IsCheckPreviousDebt;
        public static bool IsCheckPreviousPrescription;
        public static string IsDefaultRightRouteType;
        public static bool VisibilityControl;
        public static long KeyValueObligatoryTranferMediOrg;
        public static bool IsObligatoryTranferMediOrg;
        public static bool IsSyncHID { get; set; } // cau hinh nhap tinh khai sinh
        public static bool IsWarningOverExamBhyt;
        public static bool IsAutoFocusToSavePrintAfterChoosingExam;
        public static string IsSetPrimaryPatientType;
        public static bool IsByPassTextBoxRoomCode;
        public static bool IsNotRequiredRightTypeInCaseOfHavingAreaCode;
        public static bool IsShowingExamRoomInArea;
        public static bool IsShowingExamRoomInDepartment;
        public static bool IsAutoShowTransferFormInCaseOfAppointment;//đúng tuyến hẹn khám hiển thị popup chuyển tuyến
        /// <summary>
        /// 1: Các trường "Người nhà", "Quan hệ", "CMND", "Địa chỉ" đều bắt buộc nhập. 2: Chỉ bắt buộc nhập với trường "Người nhà
        /// </summary>
        public static string RelativesInforOption;
        /// <summary>
        /// Cấu hình Kiểm tra hồ sơ điều trị gần nhất của bn. Nếu có ngày ra = ngày hiện tại thì cảnh báo. 
        /// 1: có, 0: không
        /// </summary>
        public static bool IsCheckTodayFinishTreatment { get; set; }
        /// <summary>
        /// - Cấu hình chế độ kiểm tra thẻ bhyt trên cổng bhxh. Đặt 1 là tự động kiểm tra khi tìm thấy bệnh nhân cũ, đặt số khác là không kiểm tra.
        /// </summary>
        public static bool IsCheckExamHistory;
        public static string AutoCheckIcd;
        public static string PatientTypeCode__BHYT;
        public static string PatientTypeCode__KSK;
        public static long PatientTypeId__BHYT;
        public static long PatientTypeId__KSK;
        public static string PatientTypeCode__QN;
        public static long PatientTypeId__QN;
        public static string IsPrintAfterSave;
        public static string IsVisibleBill;
        public static bool IsSetDefaultDepositPrice;
        public static List<string> ExecuteRoomShow;
        public static List<string> PatientCodeIsNotRequireExamFee;
        /// <summary>
        /// Khai báo các mã đối tượng bệnh nhân ngăn cách bằng dấu phẩy. Trong trường hợp khai báo giá trị thì khi tiếp đón các đối tượng được khai báo sẽ mặc định tick vào checkbox "In phiếu khám" và các đối tượng không được khai báo sẽ mặc định bỏ tick
        /// </summary>
        public static List<long> AutoCheckPrintExam__PatientTypeIds;
        public static List<long> PatientIdIsNotRequireExamFee;
        public static int CallCpaOption;

        public static bool IsCheckExamination;//Checck cong kham

        public static string AddressConnectHrm;

        /// <summary>
        /// true - Bắt buộc nhập thông tin Người nhà, quan hệ, địa chỉ với trẻ em nhỏ hơn 6t
        /// false - Không bắt buộc nhập thông tin Người nhà, quan hệ, địa chỉ với mọi đối tượng
        /// </summary>
        public static bool MustHaveNCSInfoForChild;

        public static string Validate__T_H_X;//Bat Buoc nhap t,H,x

        public static bool IsValidate__Ethnic;//Bat Buoc nhap Dan Toc

        public static List<string> RoomCodeChoosePatientType;//Danh sach phong dc chon dttt
        public static string IsBlockingInvalidBhyt;
        public static long ValueAutoCallRegisterReq;
        public static bool IsRequiredToUpdateNewBhytCardInCaseOfExpiry;
        public static List<string> MaKetQuaBlockings
        {
            get
            {
                return new List<string>() { "010", "051", "052", "053", "050", "060", "061", "070", "100", "110", "120", "121", "122", "123", "124", "205", "9999","003" };
            }
        }

        public static bool IsManualInCode { get; set; }
        public static bool IsUsingRecognition { get; set; }
        /// <summary>
        /// Bắt buộc nhập số điện thoại hay không
        ///1 - bắt buộc
        ///khác 1 - không bắt buộc
        /// </summary>
        public static string PhoneRequired { get; set; }

        /// <summary>
        /// Cấu hình tự động fill yêu cầu - phòng khám gần nhất khi tìm bệnh nhân cũ
        /// Đặt 1 là tự động fill
        /// Mặc định là không tự động
        /// </summary>
        // public static string IsAutoFillDataRecentServiceRoom;//Chuyen sang AppConfig

        public static MOS.EFMODEL.DataModels.HIS_GENDER GenderBase;
        public static MOS.EFMODEL.DataModels.HIS_CAREER CareerBase;
        public static MOS.EFMODEL.DataModels.HIS_CAREER CareerHS;
        public static MOS.EFMODEL.DataModels.HIS_CAREER CareerUnder6Age;
        public static SDA.EFMODEL.DataModels.SDA_NATIONAL NationalBase;
        public static SDA.EFMODEL.DataModels.SDA_ETHNIC EthinicBase;

        [Obsolete("Dùng thiết lập tại phòng để hiển thị chọn khung giờ thay cho cấu hình")]
        public static string NumOrderIssueOption { get; set; }

        public static void LoadConfig()
        {
            try
            {
                LogSystem.Debug("LoadConfig => 1");
                //Get BHYT
                BHXHLoginCFG.LoadConfig();
                MODULELINKS = GetValue(CONFIG_KEY__ModuleLinks);    
                CHECK_DUPLICATION = GetValue(CONFIG_KEY__CHECK_DUPLICATION);
                WarningInvalidCheckHistoryHeinCard = GetValue(CONFIG_KEY__WarningInvalidCheckHistoryHeinCard) == valueString__true;
                EditOldPatientInformationOption = GetValue(CONFIG_KEY__EditOldPatientInformationOption) == valueString__true;
                InHospitalizationReasonRequired = GetValue(CONFIG_KEY__InHospitalizationReasonRequired) == valueString__true;
                IsNotAutoCheck5Y6M = GetValue(CONFIG_KEY__IsNotAutoCheck5Y6M) == valueString__true;
                IsDefaultTreatmentTypeExam = GetValue(CONFIG_KEY__IsDefaultTreatmentTypeExam) == valueString__true;
                IsSuggestCardHolderInformationByUsingPhoneNumber = GetValue(CONFIG_KEY__SuggestCardHolderInformationByUsingPhoneNumber) == valueString__true;
                IsNotDisplayedRouteTypeOver = GetValue(CONFIG_KEY__NotDisplayedRouteTypeOver) == valueString__true;
                IsAllowedRouteTypeByDefault = GetValue(CONFIG_KEY__IsAllowedRouteTypeByDefault) == valueString__true;
                IsFormClosingOption = GetValue(CONFIG_KEY__FormClosingOption) == valueString__true;
                ModuleLinkApply = GetValue(CONFIG_KEY__ModuleLinkApply);
                IsRequiredToUpdateNewBhytCardInCaseOfExpiry = GetValue(CONFIG_KEY_IsRequiredToUpdateNewBhytCardInCaseOfExpiry) == valueString__true;
                PhoneRequired = GetValue(CONFIG_KEY__PhoneRequired);
                FocusExecuteRoomOption = GetValue(CONFIG_KEY__FocusExecuteRoomOption);
                IsNotAutoFocusOnExistsPatient = GetValue(CONFIG_KEY__IsNotAutoFocusOnExistsPatient) == valueString__true;
                UsingPatientTypeOfPreviousPatient = GetValue(CONFIG_KEY__UsingPatientTypeOfPreviousPatient) == valueString__true;
                IsManualInCode = HisConfigs.Get<string>(CONFIG_KEY__MIS_MANUAL_IN_CODE) == valueString__true;
                SetDefaultRequestRoomByExamRoomWhenAssigningService = GetValue(CONFIG_KEY__SetDefaultRequestRoomByExamRoomWhenAssigningService) == valueString__true;
                IsNotRequiredRightTypeInCaseOfHavingAreaCode = GetValue(CONFIG_KEY_IsNotRequiredRightTypeInCaseOfHavingAreaCode) == valueString__true;
                IsShowingExamRoomInArea = GetValue(CONFIG_KEY_IsShowingExamRoomInArea) == valueString__true;
                IsShowingExamRoomInDepartment = GetValue(CONFIG_KEY_IsShowingExamRoomInDepartment) == valueString__true;
                RelativesInforOption = GetValue(CONFIG_KEY_RelativesInforOption);
                ReservedNumOder = GetValue(CONFIG_KEY_MOS_HIS_SERVICE_REQ_RESERVED_NUM_ORDER);
                ReservedNumOders = GetListValue(CONFIG_KEY_MOS_HIS_SERVICE_REQ_RESERVED_NUM_ORDER);
                RoomCodeChoosePatientType = GetListValue(HIS_DESKTOP_REGISTER__ROOM_CODES__PATIENT_TYPE);
                ExecuteRoomShow = GetListValue(HIS_DESKTOP_REGISER__EXECUTE_ROOM_SHOW);
                IsSetDefaultDepositPrice = (Inventec.Common.TypeConvert.Parse.ToInt32(GetValue(CONFIG_KEY__HIS_DEPOSIT__DEFAULT_PRICE_FOR_BHYT_OUT_PATIENT)) == valueInt__true);
                MustHaveNCSInfoForChild = (GetValue(CONFIG_KEY__MOS__HIS_PATIENT__MUST_HAVE_NCS_INFO_FOR_CHILD) == valueString__true);
                IsPrintAfterSave = GetValue(CONFIG_KEY__IS_PRINT_AFTER_SAVE__KEY);
                IsVisibleBill = GetValue(CONFIG_KEY__IS_VISIBLE_BILL__KEY);
                KeyValueObligatoryTranferMediOrg = Inventec.Common.TypeConvert.Parse.ToInt64(GetValue(HIS_UC_UCHein_IS_OBLIGATORY_TRANFER_MEDI_ORG));
                IsObligatoryTranferMediOrg = (GetValue(HIS_UC_UCHein_IS_OBLIGATORY_TRANFER_MEDI_ORG) == valueString__true || GetValue(HIS_UC_UCHein_IS_OBLIGATORY_TRANFER_MEDI_ORG) == "2");
                PatientTypeCode__BHYT = GetValue(CONFIG_KEY__PATIENT_TYPE_CODE__BHYT);
                PatientTypeCode__KSK = GetValue(CONFIG_KEY__PATIENT_TYPE_CODE__KSK);
                PatientTypeId__BHYT = GetPatientTypeByCode(PatientTypeCode__BHYT).ID;
                PatientTypeId__KSK = GetPatientTypeByCode(PatientTypeCode__KSK).ID;
                PatientTypeCode__QN = GetValue(CONFIG_KEY__PATIENT_TYPE_CODE__QN);
                PatientTypeId__QN = GetPatientTypeByCode(PatientTypeCode__QN).ID;
                AutoCheckIcd = GetValue(CONFIG_KEY__ICD_GENERATE);
                IsShowCheckExpired = GetValue(CONFIG_KEY__NOT_CHECK_EXPIRED_IS_SHOW);
                IsCheckHeinCard = (GetValue(CONFIG_KEY__IS_CHECK_HEIN_CARD) == valueString__true);
                IsCheckPreviousDebt = GetValue(CONFIG_KEY__IS_CHECK_PREVIOUS_DEBT);
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
                CheckTempQN = GetValue(HIS_UC_UCHein_IsTempQN);
                IsSetPrimaryPatientType = GetValue(CONFIG_KEY_MOS_SET_PRIMARY_PATIENT_TYPE);

                IsSyncHID = (GetValue(CONFIG_KEY__IS_USE_HID_SYNC) == valueString__true);
                IsWarningOverExamBhyt = (GetValue(CONFIG_KEY__WarningOverExamBhyt) == valueString__true);
                IsAutoFocusToSavePrintAfterChoosingExam = (GetValue(CONFIG_KEY_AutoFocusToSavePrintAfterChoosingExam) == valueString__true);

                IsCheckExamination = (GetValue(HIS_DESKTOP_REGISER__EXAMINATION) == valueString__true);

                PatientCodeIsNotRequireExamFee = new List<string>();
                PatientIdIsNotRequireExamFee = new List<long>();
                string patientCodeIsNotRequireExamFeeCFG = GetValue(DEFAULT_PATIENT_TYPE_CODE_IS_NOT_REQUIRE_EXAM_FEE);
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
                    else
                    {
                        PatientCodeIsNotRequireExamFee = new List<string>();
                        PatientIdIsNotRequireExamFee = new List<long>();
                    }
                }
                else
                {
                    PatientCodeIsNotRequireExamFee = new List<string>();
                    PatientIdIsNotRequireExamFee = new List<long>();
                }

                string patientTypeCodeIsAutoCheckPrintExamCFG = GetValue(CONFIG_KEY__AutoCheckPrintExam__PatientTypeCode);
                if (!String.IsNullOrEmpty(patientTypeCodeIsAutoCheckPrintExamCFG))
                {
                    string[] patientTypeCodeAutoCheckPrintExamArr = patientTypeCodeIsAutoCheckPrintExamCFG.Split(',');
                    if (patientTypeCodeAutoCheckPrintExamArr != null && patientTypeCodeAutoCheckPrintExamArr.Length > 0)
                    {
                        AutoCheckPrintExam__PatientTypeIds = new List<long>();
                        foreach (var item in patientTypeCodeAutoCheckPrintExamArr)
                        {
                            HIS_PATIENT_TYPE hisPatientType = GetPatientTypeByCode(item);
                            if (hisPatientType != null)
                            {
                                AutoCheckPrintExam__PatientTypeIds.Add(hisPatientType.ID);
                            }
                        }
                    }
                    else
                    {
                        AutoCheckPrintExam__PatientTypeIds = null;
                    }
                }
                else
                {
                    AutoCheckPrintExam__PatientTypeIds = null;
                }

                AddressConnectHrm = GetValue(HIS_DESKTOP_MOS_HR_ADDRESS);

                IsCheckTodayFinishTreatment = (GetValue(CONFIG_KEY__HIS_TREATMENT_IS_CHECK_TODAY_FINISH_TREATMENT) == valueString__true);
                Validate__T_H_X = GetValue(HIS_DESKTOP_REGISTER_VALIDATE__T_H_X);
                IsValidate__Ethnic = (GetValue(HIS_DESKTOP_REGISTER_VALIDATE__ETHNIC) == valueString__true);
                CallCpaOption = HisConfigs.Get<int>(CONFIG_KEY_CALL_PATIENT_NUM_ORDER_OPTION);
                IsByPassTextBoxRoomCode = (GetValue(CONFIG_KEY_HIS_DESKTOP_PLUGIN_REGISTER_BY_PASS_TEXT_BOX_ROOM_CODE) == valueString__true);
                IsAutoShowTransferFormInCaseOfAppointment = GetValue(CONFIG_KEY_IsAutoShowTransferFormInCaseOfAppointment) == valueString__true;
                IsBlockingInvalidBhyt = GetValue(IS_BLOCK_INVALID_BHYT);
                BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_SERVICE_FOLLOW>();
                IsUsingRecognition = GetValue(IS_USING_RECOGNITION) == valueString__true;

                ValueAutoCallRegisterReq = Int64.Parse(GetValue(CONFIG_KEY_AUTO_CALL_REGISTER_REQ));

                NumOrderIssueOption = GetValue(CONFIG_KEY_NUM_ORDER_ISSUE_OPTION);

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
                result = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_CAREER>().FirstOrDefault(o => o.CAREER_CODE == code);
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

        public enum OptionKey
        {
            Option1 = 1,
            Option2 = 2,
            Option3 = 3,
            Option4 = 4,
            Option5 = 5
        }
    }
}
