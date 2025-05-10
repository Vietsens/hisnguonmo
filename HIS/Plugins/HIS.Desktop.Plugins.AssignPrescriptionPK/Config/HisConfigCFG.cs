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

namespace HIS.Desktop.Plugins.AssignPrescriptionPK.Config
{
    class HisConfigCFG
    {
        private const string IS_ALLOW_SIGNATURE_PRINT_MODULELINKS = "HIS.Desktop.Plugins.IsAllowSignaturePrint.ModuleLinks";
        private const string CONFIG_KEY__ASSIGNPRESCRIPTION_EGFROPTION = "HIS.Desktop.Plugins.AssignPrescription.EgfrOption";   
        private const string CONFIG_KEY__ICD_SERVICE__CONTRAINDICATED__WARNING_OPTION = "HIS.ICD_SERVICE.CONTRAINDICATED.WARNING_OPTION";
        private const string CONFIG_KEY__TrackingCreate__UpdateTreatmentIcd = "HIS.Desktop.Plugins.TrackingCreate.UpdateTreatmentIcd";
        private const string CONFIG_KEY__HIS_SERVICE_REQ__DO_NOT_ALLOW_PRES_OUT_PATIENT_IN_CASE_OF_HAVING_DEBT = "MOS.HIS_SERVICE_REQ.DO_NOT_ALLOW_PRES_OUT_PATIENT_IN_CASE_OF_HAVING_DEBT";
        private const string CONFIG_KEY__HIS_PATIENT_PROGRAM__NOT_HAS_EMR_COVER_TYPE__WARNING_OPTION = "HIS.DESKTOP.HIS_PATIENT_PROGRAM.NOT_HAS_EMR_COVER_TYPE.WARNING_OPTION";
        private const string CONFIG_KEY__PRESCRIPTION__ExamServiceReqExecute_IsAutoCloseAfterSolving = "HIS.Desktop.Plugins.ExamServiceReqExecute.IsAutoCloseAfterSolving";
        private const string CONFIG_KEY__PRESCRIPTION__HIS_PATIENT_PROGRAM_NOT_HAS_EMR_COVER_TYPE_WARNING_OPTION = "HIS.DESKTOP.HIS_PATIENT_PROGRAM.NOT_HAS_EMR_COVER_TYPE.WARNING_OPTION";
        private const string CONFIG_KEY__PRESCRIPTION__WarningWhenNotFinishingIncaseOfOutPatient = "HIS.Desktop.Plugins.AssignPrescription.WarningWhenNotFinishingIncaseOfOutPatient";
        private const string CONFIG_KEY__PRESCRIPTION__WARNING_ALLERGENIC_OPTION = "HIS.DESKTOP.PRESCRIPTION.WARNING_ALLERGENIC_OPTION";
        private const string CONFIG_KEY__AutoFocusToAdvise = "HIS.Desktop.Plugins.AssignPrescription.AutoFocusToAdvise";
        private const string CONFIG_KEY__MEDICINE_DEBATE_OPTION = "MOS.HIS_SERVICE_REQ.MEDICINE_DEBATE_OPTION";
        private const string CONFIG_KEY__AssignPrescription__SplitOffset = "HIS.Desktop.Plugins.AssignPrescription.SplitOffset";
        private const string MAX_OF_APPOINTMENT_DAYS = "MOS.HIS_TREATMENT.MAX_OF_APPOINTMENT_DAYS";
        private const string WARNING_OPTION_WHEN_EXCEEDING_MAX_OF_APPOINTMENT_DAYS = "MOS.HIS_TREATMENT.WARNING_OPTION_WHEN_EXCEEDING_MAX_OF_APPOINTMENT_DAYS";
        private const string CONFIG_KEY__HIS_Desktop_Plugins_AssignPrescription_IsNotAutoGenerateTutorial = "HIS.Desktop.Plugins.AssignPrescription.IsNotAutoGenerateTutorial";
        private const string CONFIG_KEY__MOS_HIS_SERVICE_REQ_MANY_DAYS_PRESCRIPTION_OPTION = "MOS.HIS_SERVICE_REQ.MANY_DAYS_PRESCRIPTION_OPTION";
        private const string CONFIG_KEY__IsAllowAssignPresByPackage = "HIS.Desktop.Plugins.IsAllowAssignPresByPackage";
        private const string CONFIG_KEY__IsloadIcdFromExamServiceExecute = "HIS.Desktop.Plugins.IsloadIcdFromExamServiceExecute";
        private const string CONFIG_KEY__ShowRoundAvailableAmount = "His.Desktop.InPatientPrescription.ShowRoundAvailableAmount";
        private const string CONFIG_KEY__IS_AUTO_CREATE_SALE_EXP_MEST = "MOS.HIS_SERVICE_REQ.IS_AUTO_CREATE_SALE_EXP_MEST";
        private const string CONFIG_KEY__ShowRequestUser = "HIS.Desktop.Plugins.AssignConfig.ShowRequestUser";
        private const string CONFIG_KEY__PATIENT_TYPE_CODE__BHYT = "MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BHYT";//Doi tuong BHYT     
        private const string CONFIG_KEY__PATIENT_TYPE_CODE__VP = "MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.HOSPITAL_FEE";//Doi tuong VP
        private const string LIMIT_HEIN_MEDICINE_PRICE__RIGHT_MEDI_ORG = "MOS.BHYT.LIMIT_HEIN_MEDICINE_PRICE.RIGHT_MEDI_ORG";
        private const string LIMIT_HEIN_MEDICINE_PRICE__TRAN_PATI = "MOS.BHYT.LIMIT_HEIN_MEDICINE_PRICE.TRAN_PATI";
        private const string CONFIG_KEY__ICD_GENERA_KEY = "HIS.Desktop.Plugins.AutoCheckIcd";
        private const string CONFIG_KEY__LIMIT_HEIN_MEDICINE_PRICE__RIGHT_MEDI_ORG = "MOS.BHYT.LIMIT_HEIN_MEDICINE_PRICE.RIGHT_MEDI_ORG";
        private const string CONFIG_KEY__LIMIT_HEIN_MEDICINE_PRICE__NOT_RIGHT_MEDI_ORG = "MOS.BHYT.LIMIT_HEIN_MEDICINE_PRICE.NOT_RIGHT_MEDI_ORG";
        private const string LOAD_PATIENT_TYPE_DEFAULT_KEY = "EXE.ASSING_SERVICE_MERGER.LOAD_PATIENT_TYPE_DEFAULT";
        private const string IS_VISILBE_TEMPLATE_MEDICINE_KEY = "EXE.ASSING_SERVICE_MERGER.IS_VISILBE_TEMPLATE_MEDICINE";
        private const string IS_VISILBE_EXECUTE_GROUP_KEY = "HIS.Desktop.Plugins.Assign.IsExecuteGroup";
        private const string CONFIG_KEY__OBLIGATE_ICD = "EXE.ASSIGN_SERVICE_REQUEST__OBLIGATE_ICD";
        private const string Key__IsAllowPrintFinish = "HIS.Desktop.AllowPrint.Finish";
        private const string Key__AcinInteractive__Grade = "HIS.Desktop.Plugins.AssignPrescription.AcinInteractive__Grade";

        private const string TREATMENT_END___APPOINTMENT_TIME_DEFAULT_KEY = "EXE.HIS_TREATMENT_END.APPOINTMENT_TIME_DEFAULT";
        private const string PRESCRIPTION_TIME_AND_APPOINTMENT_TIME_KEY = "HIS.Desktop.Plugins.TreatmentFinish.APPOINTMENT_TIME";
        private const string CONFIG_KEY__TREATMENT_FINISH__CHECK_FINISH_TIME = "HIS.DESKTOP.TREATMENT_FINISH.CHECK_FINISH_TIME";
        private const string CHECK_ASSIGN_SERVICE_BED = "HIS.DESKTOP.TREATMENT_FINISH.CHECK_ASSIGN_SERVICE_BED";
        private const string MUST_FINISH_ALL_SERVICES_BEFORE_FINISH_TREATMENT_CFG = "MOS.HIS_TREATMENT.MUST_FINISH_ALL_SERVICES_BEFORE_FINISH_TREATMENT";
        private const string SERVICE_CODE__AUTO_FINISH = "MOS.HIS_TREATMENT.AUTO_FINISH_SERVICE_REQ.SERVICE_CODE";
        private const string CONFIG_KEY____ISAUTO_CLOSE_FROM_WITH_AUTO_TREATMENT_FINISH = "HIS.Desktop.Plugins.AssignPrescription.IsAutoCloseFormWithAutoConfigTreatmentFinish";
        private const string Key__WarningOverCeiling__Exam__Out__In = "HIS.Desktop.Plugins.WarningOverCeiling.Exam__Out__In";
        public const string ONLY_DISPLAY_MEDIMATE_IS_BUSINESS = "HIS.Desktop.Plugins.AssignPrescription.OnlyDisplayMediMateIsBusiness";

        public const string ICD_SERVICE__HAS_CHECK = "HIS.HIS_ICD_SERVICE.HAS_CHECK";
        public const string ICD_SERVICE__HAS_REQUIRE_CHECK = "HIS.HIS_ICD_SERVICE.HAS_REQUIRE_CHECK";
        public const string HIS_ICD_SERVICE__ALLOW_UPDATE = "HIS.HIS_ICD_SERVICE.ALLOW_UPDATE";

        internal const string SAVE_PRINT_MPS_DEFAULT = "HIS.Desktop.Plugins.Library.PrintPrescription.Mps";
        internal const string CHECK_SAME_HEIN_KEY = "HIS.DESKTOP.TREATMENT_FINISH.CHECK_SAME_HEIN";
        internal const string TUTORIAL_NUMBER_IS_FRAC = "HIS.Desktop.Plugins.AssignPrescription.TutorialNumberIsFrac";
        internal const string WARNING_OVER_TOTAL_PATIENT_PRICE__IS_CHECK = "HIS.Desktop.WarningOverTotalPatientPrice__IsCheck";
        internal const string WARNING_OVER_TOTAL_PATIENT_PRICE = "HIS.Desktop.WarningOverTotalPatientPrice";
        internal const string WARRING_USE_DAY_AND_EXP_TIME_BHYT = "HIS.Desktop.Plugins.AssignPrescription.IsWarringUseDayAndExpTimeBHYT";
        internal const string MEDICINE_HAS_PAYMENT_LIMIT_BHYT = "HIS.Desktop.Plugins.AssignPrescription.MedicineHasPaymentLimitBHYT";
        internal const string WARRING_INTRUCTION_USE_DAY_NUM = "HIS.Desktop.Plugins.AssignPrescription.WarringIntructionUseDayNum";
        internal const string KEY_IS_DEFAULT_TRACKING = "HIS.Desktop.Plugins.AssignPrescription.IsDefaultTracking";
        internal const string TUTORIAL_FORMAT = "HIS.Desktop.Plugins.AssignPrescription.TutorialFormat";
        internal const string MOS__PRESCRIPTION_SPLIT_OUT_MEDISTOCK = "MOS.HIS_SERVICE_REQ.PRESCRIPTION_SPLIT_OUT_MEDISTOCK";
        private const string CONFIG_KEY__BlockingInteractiveGrade = "HIS.Desktop.Plugins.AssignPrescription.BlockingInteractiveGrade";
        private const string MOS__HIS_SERVICE_REQ__IS_NOT_ALLOWING_EXPEND_WITHOUT_HAVING_PARENT = "MOS.HIS_SERVICE_REQ.IS_NOT_ALLOWING_EXPEND_WITHOUT_HAVING_PARENT";
        internal const string IS_USING_SERVER_TIME = "MOS.IS_USING_SERVER_TIME";
        private const string CONFIG_KEY__IsAutoTickExpendWithAssignPresPTTT = "HIS.Desktop.Plugins.AssignPrescription.IsAutoTickExpendWithAssignPresPTTT";
        private const string CONFIG_KEY__IsUsingWarningHeinFee = "His.Desktop.IsUsingWarningHeinFee";
        internal const string IS_CHOOSE_DRUGSTORE = "HIS.Desktop.Plugins.AssignPrescription.DefaultDrugStoreCode"; // co chọn nhà thuoc hay khong//Se bo sau
        internal const string CONFIG_KEY__DONT_PRES_EXPIRED_ITEM = "MOS.HIS_MEDI_STOCK.DONT_PRES_EXPIRED_ITEM";
        internal const string CONFIG_KEY__WARNING_ODD_CONVERT_AMOUNT = "HIS.Desktop.Plugins.AssignPrescription.IsWarningOddConvertAmount";
        private const string CONFIG_KEY__IS_CHECK_PREVIOUS_PRESCRIPTION = "HIS.Desktop.Plugins.AssignPrescriptionPK.IsCheckPreviousPrescripton";
        private const string CONFIG_KEY__IsAutoRoundUpByConvertUnitRatio = "HIS.Desktop.Plugins.AssignPrescriptionPK.IsAutoRoundUpByConvertUnitRatio";
        private const string CONFIG_KEY__MOS_HIS_SERVICE_REQ_REQ_USER_MUST_HAVE_DIPLOMA = "MOS.HIS_SERVICE_REQ.REQ_USER_MUST_HAVE_DIPLOMA";
        private const string CONFIG_KEY__IsShowingInTheSameDepartment = "His.Desktop.Plugins.ReqUser.IsShowingInTheSameDepartment";
        private const string CONFIG_KEY__BhytColorCode = "HIS.Desktop.Plugins.AssignPrescriptionPK.BhytColorCode";
        private const string CONFIG_KEY__OutStockListItemInCaseOfNoStockChosenOption = "HIS.Desktop.Plugins.AssignPrescription.OutStockListItemInCaseOfNoStockChosenOption";
        private const string CONFIG_KEY__MOS_HIS_SERVICE_REQ_PRESCRIPTION_IS_TRACKING_REQUIRED = "MOS.HIS_SERVICE_REQ.PRESCRIPTION.IS_TRACKING_REQUIRED";
        private const string CONFIG_KEY__AmountDecimalNumber = "HIS.Desktop.Plugins.AssignPrescriptionPK.AmountDecimalNumber";
        private const string CONFIG_KEY__PRESCRIPTION_ATC_CODE_OVERLAP_WARNING_OPTION = "HIS.DESKTOP.PRESCRIPTION.ATC_CODE_OVERLAP.WARNING_OPTION";
        private const string CONFIG_KEY__IS_USING_EXECUTE_ROOM_PAYMENT = "MOS.EPAYMENT.IS_USING_EXECUTE_ROOM_PAYMENT";

        private const string CONFIG_KEY__CONNECT_DRUG_INTERVENTION_INFO = "HIS.Desktop.Plugins.AssignPrescription.ConnectDrugInterventionInfo";
        private const string CONFIG_KEY__HIS_DRUG_INTERVENTION_CONNECTION_INFO = "MOS.HIS_DRUG_INTERVENTION.CONNECTION_INFO";

        private const string CONFIG_KEY__IS_USING_SUB_PRESCRIPTION_MECHANISM = "MOS.HIS_SERVICE_REQ.IS_USING_SUB_PRESCRIPTION_MECHANISM";

        private const string CONFIG_KEY__EXCEED_AVAILABLE_OUT_STOCK = "HIS.Desktop.Plugins.ExceedAvailableOutStockPrescriptionOption";

        private const string IS_CHECK_DEPARTMENT_IN_TIME_WHEN_PRES_OR_ASSIGN = "HIS.Desktop.Plugins.IsCheckDepartmentInTimeWhenPresOrAssign";

        public const string CONFIG_KEY__MestRoomOption = "MOS.HIS_SERVICE_REQ.PRESCRIPTION.MEST_ROOM_OPTION";

        public const string CONFIG_KEY__DefaultPatientTypeOption = "HIS.Desktop.Plugins.Assign.DefaultPatientTypeOption";

        public const string CONFIG_KEY__GroupOption = "HIS.Desktop.Plugins.AssignPrescription.GroupOption";

        public const string CONFIG_KEY__IS_REASON_REQUIRED = "MOS.EXP_MEST.IS_REASON_REQUIRED";
        private const string CONFIG_KEY__FormClosingOption = "HIS.Desktop.FormClosingOption";
        internal static bool IsFormClosingOption;

        private const string CONFIG_KEY__ModuleLinkApply = "HIS.Desktop.FormClosingOption.ModuleLinkApply";

        private const string CONFIG_KEY__ICD_CODE_TO_APPLY_RESTRICT_PATIENT_TYPE_BY_OTHER_SOURCE_PAID = "MOS.HIS_SERVICE_REQ.ICD_CODE_TO_APPLY_RESTRICT_PATIENT_TYPE_BY_OTHER_SOURCE_PAID";
        private const string CONFIG_KEY__ShowPresAmount = "HIS.Desktop.Plugins.AssignPrescriptionPK.ShowPresAmount";
        private const string CONFIG_KEY__SERVICE_REQ_ICD_OPTION = "HIS.HIS_TRACKING.SERVICE_REQ_ICD_OPTION";
        private const string CONFIG__USING_EXAM_SUB_ICD_WHEN_FINISH = "MOS.HIS_TREATMENT.IS_USING_EXAM_SUB_ICD_WHEN_FINISH";
        private const string CONFIG__AcinInteractiveOption = "HIS.Desktop.Plugins.AssignPrescription.AcinInteractiveOption";
        private const string CONFIG__DrugStoreComboboxOption = "HIS.Desktop.Plugins.AssignPrescription.DrugStoreComboboxOption";
        private const string CONFIG__ShowServerTimeByDefault = "HIS.Desktop.ShowServerTimeByDefault";
        private const string CHECK_ICD_WHEN_SAVE = "HIS.Desktop.Plugins.CheckIcdWhenSave";
        private const string KEY__MustChooseSeviceExam = "HIS.Desktop.Plugins.TreatmentFinish.MustChooseSeviceExam.Option";
        private const string KEY__EXECUTE_ROOM_PAYMENT_OPTION = "MOS.EPAYMENT.EXECUTE_ROOM_PAYMENT_OPTION";
        private const string KEY__RequiredTreatmentMethodOption = "HIS.Desktop.Plugins.TreatmentFinish.RequiredTreatmentMethodOption";
        private const string KEY__InstructionTimeServiceMustBeGreaterThanStartTimeExam = "HIS.Desktop.Plugins.InstructionTimeServiceMustBeGreaterThanStartTimeExam";
        private const string KEY_TreatmentEndTypeIsTransfer = "HIS.Desktop.Plugins.TreatmentFinish.TreatmentEndTypeIsTransfer";
        private const string KEY_TuberculosisOption = "HIS.Desktop.Plugins.AssignPrescriptionPK.TuberculosisOption";
        private const string KEY_ASSIGN_SERVICE_SIMULTANEITY_OPTION = "MOS.HIS_SERVICE_REQ.ASSIGN_SERVICE_SIMULTANEITY_OPTION";
        private const string KEY_CheckSoNgay = "His.Desktop.Plugins.AssignPrescriptionPK.CheckSoNgay";
        private const string KEY_IsCheckSubIcdExceedLimit = "HIS.Desktop.Plugins.IsCheckSubIcdExceedLimit";
        private const string KEY_IsRequiredHtu = "HIS.Desktop.Plugins.AssignPrescription.IsRequiredHtu"; 
        private const string KEY_SaveButtonOption = "HIS.Desktop.Plugins.AssignPrescriptionPK.SaveButtonOption";
        private const string KEY_ALLOW_ASSIGN_OFF_LIST_MEDICINE_MATERIAL__HEIN_CARD_NUMBER_PREFIX = "MOS.BHYT.ALLOW_ASSIGN_OFF_LIST_MEDICINE_MATERIAL__HEIN_CARD_NUMBER_PREFIX";
        internal static string AllowAssignOffListMedicineMaterialHeinCardNumberPrefix;
        internal static string IsSaveButtonOption;
        internal static string IsRequiredHtu;
        internal static string IsCheckSubIcdExceedLimit;
        internal static bool CheckSoNgay;
        internal static string ASSIGN_SERVICE_SIMULTANEITY_OPTION;
        internal static string TuberculosisOption;
        internal static string OptionTreatmentEndTypeIsTransfer;
        internal static string InstructionTimeServiceMustBeGreaterThanStartTimeExam;
        internal static string RequiredTreatmentMethodOption;
        internal static string ExecuteRoomPaymentOption;
        internal static string MustChooseSeviceExamOption;
        internal static string CheckIcdWhenSave;
        internal static bool IsDrugStoreComboboxOption;
        internal static string AcinInteractiveOption;
        internal static string OptionSubIcdWhenFinish;
        internal static bool IsServiceReqIcdOption;
        internal static string ModuleLinkApply;
        internal static string MODULELINKS;
        internal static string ASSIGNPRESCRIPTION_EGFROPTION;

        /// <summary>
        /// :Có tự động focus vào ô lời dặn không. 1-có, 0-không
        ///Mặc định= 0
        /// </summary>
        internal static bool AutoFocusToAdvise;

        internal static string TrackingCreate__UpdateTreatmentIcd;

        /// <summary>
        /// Tùy chọn Cảnh báo dịch vụ/thuốc/hoạt chất chống chỉ dịnh với ICD:
        ///0 - Không chặn/cảnh báo.
        ///1 - Chặn.
        ///2 - Cảnh báo.
        ///Mặc định 0       
        /// </summary>
        internal static string ContraindicaterWarningOption;

        /// <summary>
        /// giá trị 1 và hồ sơ có thông tin chương trình (PROGRAM_ID trong HIS_TREATMENT có giá trị) và chưa có thông tin Vỏ bệnh án (EMR_COVER_TYPE_ID trong HIS_TREATMENT không có giá trị) thì show cảnh báo: "Bệnh nhân chương trình chưa được tạo Vỏ bệnh án"
        /// </summary>
        internal static string HisPatientProgramNotHasEmrCoverTypeWarningOption;

        /// <summary>
        /// Bổ sung cấu hình hệ thống: MOS.HIS_SERVICE_REQ.MEDICINE_DEBATE_OPTION:
        /// Khi người dùng kê thuốc có dấu sao hoặc thuốc có hoạt chất được đánh dấu sao thì:
        ///- 1: Cảnh báo và chặn nếu khoa chỉ định chưa có biên bản hội chẩn tương ứng với thuốc.
        ///- 2: Cảnh báo và chặn nếu khoa chỉ định chưa có biên bản hội chẩn tương ứng với hoạt chất.
        ///- Khác: Không xử lý
        /// </summary>
        internal static string MedicineDebateOption;

        /// <summary>
        /// Cấu hình tách phần bù thuốc/vật tư (phần để làm tròn trong trường hợp kê lẻ)
        /// Khi cấu hình hệ thống trên có giá trị = 1 thì với các trường hợp có nghiệp vụ xử lý làm tròn số lượng thuốc/vật tư (vd: người dùng kê số lượng lẻ, nhưng thuốc/vật tư cấu hình không cho phép kê lẻ) thì thực hiện xử lý:
        ///- Trong trường hợp kê đơn mới, thì gửi thông tin kê thành 2 dòng:
        ///+ 1 dòng là số lượng người dùng kê
        ///+ 1 dòng là số lượng phần bù, và có đánh dấu là IsNotPres = true
        /// </summary>
        internal static string SplitOffset;
        internal static bool IsTrackingRequired;

        /// <summary>
        /// 1: Tự động làm tròn căn cứ theo đơn vị quy đổi. Trong trường hợp kê thuốc/vật tư theo đơn vị quy đổi và tỷ lệ quy đổi > 1 , thì tự động làm tròn để tính ra số lượng theo đơn vị gốc luôn là số nguyên. 0: Không tự động
        /// vd:
        ///- Đơn vị là UI
        ///- Đơn vị quy đổi là Lọ
        ///- Tỷ lệ quy đổi là 1 phần 400 (1 Lọ = 400 UI)
        ///- Người dùng kê số lượng là 700
        ///--> làm tròn lên thành X = (ceiling(700 * chia 400)) * (400) = 800
        /// </summary>
        internal static bool IsAutoRoundUpByConvertUnitRatio;
        /// <summary>
        /// combobox "Người chỉ định" chỉ hiển thị các tài khoản nhân viên có thông tin "Chứng chỉ hành nghề" (DIPLOMA trong his_employee khác null)
        /// </summary>
        internal static bool UserMustHaveDiploma;
        /// <summary>
        /// 1: Khi chỉ định DVKT, hay kê đơn, danh sách "người chỉ định" chỉ hiển thị các tài khoản thuộc khoa mà người dùng đang làm việc (căn cứ vào DEPARTMENT_ID trong HIS_EMPLOYEE)
        /// </summary>
        internal static bool IsShowingInTheSameDepartment;
        internal static string BhytColorCode;
        internal static bool IsWarningOddConvertAmount;
        internal static bool IsDontPresExpiredTime;
        internal static string DefaultDrugStoreCode;//Se bo sau
        internal static string IsUsingWarningHeinFee;
        internal static bool IsAutoCreateSaleExpMest;
        internal static bool IsAutoTickExpendWithAssignPresPTTT;
        internal static bool IsNotAllowingExpendWithoutHavingParent;
        public static int AmountDecimalNumber { get; set; }
        public static decimal WarningOverCeiling__Exam { get; set; }
        public static decimal WarningOverCeiling__Out { get; set; }
        public static decimal WarningOverCeiling__In { get; set; }
        internal static bool IsShowPresAmount;
        internal static string ShowRequestUser;
        internal static string PatientTypeCode__BHYT;
        internal static long PatientTypeId__BHYT;
        internal static string PatientTypeCode__VP;
        internal static long PatientTypeId__VP;
        /// <summary>
        /// 1: Với đối tượng bệnh nhân không phải là BHYT và đối tượng điều trị là Khám thì tự động focus vào tab "Thuốc - vật tư mua ngoài".
        ///2: Với đối tượng bệnh nhân là Viện phí và diện điều trị là khám thì tự động focus vào tab "Thuốc - vật tư mua ngoài".
        ///Còn lại: Không tự động.
        /// </summary>
        internal static string AutoCheckIcd;
        internal static decimal LimitHeinMedicinePrice__RightMediOrg;
        internal static decimal LimitHeinMedicinePrice__NotRightMediOrg;
        internal static string IsVisilbleExecuteGroup;
        internal static string LoadPatientTypeDefault;
        internal static string IsVisilbleTemplateMedicine;
        internal static long? AcinInteractive__Grade;
        internal static string TreatmentTypeCode__Exam;
        internal static string TreatmentTypeCode__TreatIn;
        internal static string TreatmentTypeCode__TreatOut;
        internal static string ObligateIcd;
        internal static bool IsAutoCloseFormWithAutoConfigTreatmentFinish;
        internal static bool IsAllowPrintFinish;
        internal static bool IsTutorialNumberIsFrac;
        internal static long icdServiceHasCheck;
        internal static bool icdServiceHasRequireCheck;
        internal static long icdServiceAllowUpdate;
        internal static long CheckSameHein;
        internal static bool IsWarningOverTotalPatientPrice;
        internal static bool IsWarringUseDayAndExpTimeBHYT;
        internal static bool IsDefaultTracking;
        internal static bool IsUsingServiceTime;
        internal static bool IsShowServerTimeByDefault;

        /// <summary>
        /// Bệnh nhân đang thiếu viện phí ({0} đồng). Bạn có muốn tiếp tục?
        /// </summary>
        internal static decimal WarningOverTotalPatientPrice;

        /// <summary>
        /// Cấu hình định dạng hướng dẫn sử dụng ở màn hình kê đơn
        /// - 1: Ngày <đường dùng> <số lượng tổng số 1 ngày> <đơn vị> / số lần : thời điểm trong ngày : số lượng <Dạng dùng>
        /// Ví dụ: ngày uống 4 viên/ 4 lần: sáng 1, trưa 1, chiều 1, tối 1 sau ăn
        /// - 2: Ngày <đường dùng> số lần : thời điểm trong ngày : số lượng <Dạng dùng>
        /// Ví dụ: ngày uống 4 lần: sáng 1, trưa 1, chiều 1, tối 1 sau ăn
        /// (Không nhập sáng trưa chiều tối sẽ không ra hướng dẫn sử dụng)
        /// - 3:
        /// + Đối với màn hình kê đơn không phải YHCT: <Số lượng> <đơn vị>/lần * <Số lần>/ngày
        /// Ví dụ: Thuốc A được kê là Ngày uống 2 viên chia 2 lần, sáng 1 viên, chiều 1 viên thì trên xml thể hiện: 1 viên/lần * 2 lần/ngày
        /// + Đối với màn hình kê đơn YHCT: <số lượng> <đơn vị> * 1 thang * <số ngày>
        /// Ví dụ: Thuốc thang A: 12g/thang, uống 5 thang: Trên xml thể hiện: 12g*1 thang*5 ngày
        /// - 4: < Đường dùng> <Tổng số ngày> ngày. Ngày <đường dùng> <số lượng tổng số 1 ngày> <đơn vị> / số lần : thời điểm trong ngày : số lượng <Dạng dùng>
        /// ví dụ : Uống 4 ngày. Ngày uống 4 lần sáng 1, trưa 1, chiều 1, tối 1 sau ăn
        /// </summary>
        internal static long TutorialFormat;

        internal static string MedicineHasPaymentLimitBHYT;
        internal static bool IsCheckPreviousPrescription;

        /// <summary>
        /// + Nếu key cấu hình trên có giá trị là 1 thì xử lý như hiện tại: nếu không chọn kho thì hiển thị toàn bộ thuốc/vật tư (không bị khóa) trong danh mục
        ///+ Nếu key cấu hình trên có giá trị là 2 thì xử lý: lấy toàn bộ thông tin thuốc/vật tư theo thông tin khả dụng của tất cả các nhà thuốc (các kho xuất hiện ở combobox "Nhà thuốc"), và dữ liệu chọn thuốc/vật tư cần bổ sung thêm cột "khả dụng", với số liệu "khả dụng" là tổng khả dụng của thuốc/vật tư đó tại tất cả các nhà thuốc.
        ///(vd: thuốc A có khả dụng 5 tại kho X, thuốc A có khả dụng 3 tại kho Y. --> trên d/s chọn thuốc, sẽ hiển thị: Thuốc A - khả dụng 8)
        /// </summary>
        internal static string OutStockListItemInCaseOfNoStockChosenOption;

        /// <summary>
        /// "Cấu hình ưu tiên lây thời gian hẹn khám (kiểu số):
        //- 1: Ưu tiên thời gian hẹn khám theo thời gian kê đơn thuốc
        //- 0: Ưu tiên theo thời gian được cấu hình"
        /// </summary>
        internal static string AppointmentTimeDefault;

        /// <summary>
        /// "Cấu hình ưu tiên lây thời gian hẹn khám (kiểu bool):
        //- true: Ưu tiên thời gian hẹn khám theo thời gian kê đơn thuốc
        //- false: Ưu tiên theo thời gian được cấu hình"
        /// </summary>
        internal static bool TreatmentEndHasAppointmentTimeDefault;

        /// <summary>
        /// Cấu hình giá trị ngày hẹn khám lại cho bệnh nhân
        /// </summary>
        internal static long TreatmentEndAppointmentTimeDefault;
        internal static long? WarringIntructionUseDayNum;

        internal static bool IsCheckFinishTime;
        internal static bool isCheckBedService;
        internal static bool isPrescriptionSplitOutMediStock;
        internal static bool mustFinishAllServicesBeforeFinishTreatment;
        internal static List<long> autoFinishServiceIds;
        internal static string BlockingInteractiveGrade;
        internal static bool IsloadIcdFromExamServiceExecute;

        /// <summary>
        /// Cấu hình cho phép kê thuốc/vật tư theo lô hoặc theo loại. Đăt 1: cho phép kê theo lô. Đặt khác 1: kê theo loại.
        /// </summary>
        internal static bool IsAllowAssignPresByPackage;

        /// <summary>
        /// 1: Kê nhiều ngày theo cả đơn; 2: Kê nhiều ngày theo từng thuốc
        /// </summary>
        internal static long ManyDayPrescriptionOption;
        internal static bool InPatientPrescription__ShowRoundAvailableAmount;
        /// <summary>
        /// 1: Không tự động sinh trường HDSD khi kê đơn
        /// </summary>
        internal static bool IsNotAutoGenerateTutorial;

        /// <summary>
        /// Cấu hình số ngày hẹn khám tối đa tính từ thời điểm kết thúc khám
        /// </summary>
        internal static long? MaxOfAppointmentDays;

        /// <summary>
        /// Tùy chọn xử lý trong trường hợp vượt quá số ngày hẹn khám. 1: Cảnh báo. 2: Chặn, không cho xử trí
        /// </summary>
        internal static long? WarningOptionWhenExceedingMaxOfAppointmentDays;

        /// <summary>
        ///  Khi key cấu hình trên có giá trị = 1 thì xử lý:
        ///Trong trường hợp loại là "Kê đơn phòng khám" và công khám đang xử lý là khám chính (HIS_SERVICE_REQ có IS_MAIN_EXAM = 1), và khi người dùng nhấn "Lưu" hoặc "Lưu in" nhưng ko nhập thông tin kết thúc điều trị thì hiển thị cảnh báo:
        ///"Bạn có muốn nhập thông tin kết thúc điều trị không?"
        ///+ Nếu người dùng chọn "Đồng ý" thì tắt thông báo vào tự động check vào checkbox "Kết thúc điều trị" và mặc định focus vào combobox "Loại ra viện" để người dùng nhập
        ///+ Nếu người dùng chọn "Không" thì thực hiện gọi api để xử lý lưu đơn như hiện tại
        /// </summary>
        internal static string WarningWhenNotFinishingIncaseOfOutPatient;

        /// <summary>
        ///- Cấu hình hệ thống: HIS.DESKTOP.PRESCRIPTION.WARNING_ALLERGENIC_OPTION
        ///- Tùy chọn cảnh báo dị ứng thuốc: 1 - Cảnh báo theo thẻ dị ứng của bệnh nhân. 0 - Không cảnh báo. Mặc định 0.
        /// </summary>
        internal static string WarningAllegericOption;

        internal static string DoNotAllowPresOutPatietInCaseOfHavingDebt;

        /// <summary>
        /// có giá trị 1 và hồ sơ có thông tin chương trình (PROGRAM_ID) và chưa có thông tin Vỏ bệnh án (EMR_COVER_TYPE_ID trong HIS_TREATMENT) thì show cảnh báo: "Bệnh nhân chương trình chưa được tạo Vỏ bệnh án".
        /// </summary>
        internal static string NotHasEmrCoverTypeWarningOption;

        internal static string AtcCodeOverlarWarningOption;

        internal static string IsUsingExecuteRoomPayment;

        internal static string ConnectDrugInterventionInfo;

        internal static string ConnectionInfo;

        /// <summary>
        ///  Có áp dụng cơ chế đơn phòng khám phụ hay không.
        ///- 0: Không áp dụng
        ///- 1: Có áp dụng. Lúc này các phòng khám thêm lúc kê đơn trong kho thì sẽ tạo ra "đơn phụ" không chiếm khả dụng. Khi phòng khám chính kê đơn, phần mềm sẽ tự động chọn thuốc/vật tư theo các thuốc/vật tư đã được kê trong các "đơn phụ" cho phép người dùng chỉnh sửa, bổ sung.
        /// </summary>
        internal static string IsUsingSubPrescriptionMechanism;

        internal static bool IsExceedAvailableOutStock;

        internal static bool IsCheckDepartmentInTimeWhenPresOrAssign;

        internal static long MestRoomOption;

        internal static bool DefaultPatientTypeOption;

        internal static bool GroupOption;

        internal static bool IsReasonRequired;

        internal static string IcdCodeToApplyRestrictPatientTypeByOtherSourcePaid;

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

        internal static void LoadConfig()
        {
            try
            {
                ASSIGNPRESCRIPTION_EGFROPTION = GetValue(CONFIG_KEY__ASSIGNPRESCRIPTION_EGFROPTION);
                MODULELINKS = GetValue(IS_ALLOW_SIGNATURE_PRINT_MODULELINKS);
                AllowAssignOffListMedicineMaterialHeinCardNumberPrefix = GetValue(KEY_ALLOW_ASSIGN_OFF_LIST_MEDICINE_MATERIAL__HEIN_CARD_NUMBER_PREFIX);
                IsSaveButtonOption = GetValue(KEY_SaveButtonOption);
                IsRequiredHtu = GetValue(KEY_IsRequiredHtu);
                IsCheckSubIcdExceedLimit = GetValue(KEY_IsCheckSubIcdExceedLimit);
                CheckSoNgay = GetValue(KEY_CheckSoNgay) == GlobalVariables.CommonStringTrue;
                ASSIGN_SERVICE_SIMULTANEITY_OPTION = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(KEY_ASSIGN_SERVICE_SIMULTANEITY_OPTION);
                TuberculosisOption = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(KEY_TuberculosisOption);
                OptionTreatmentEndTypeIsTransfer = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(KEY_TreatmentEndTypeIsTransfer);
                InstructionTimeServiceMustBeGreaterThanStartTimeExam = GetValue(KEY__InstructionTimeServiceMustBeGreaterThanStartTimeExam);
                RequiredTreatmentMethodOption = GetValue(KEY__RequiredTreatmentMethodOption);
                ExecuteRoomPaymentOption = GetValue(KEY__EXECUTE_ROOM_PAYMENT_OPTION);
                MustChooseSeviceExamOption = GetValue(KEY__MustChooseSeviceExam);
                CheckIcdWhenSave = GetValue(CHECK_ICD_WHEN_SAVE);
                IsShowServerTimeByDefault = GetValue(CONFIG__ShowServerTimeByDefault) == GlobalVariables.CommonStringTrue;
                IsDrugStoreComboboxOption = GetValue(CONFIG__DrugStoreComboboxOption) == GlobalVariables.CommonStringTrue;
                AcinInteractiveOption = GetValue(CONFIG__AcinInteractiveOption);
                OptionSubIcdWhenFinish = GetValue(CONFIG__USING_EXAM_SUB_ICD_WHEN_FINISH);
                IsServiceReqIcdOption = GetValue(CONFIG_KEY__SERVICE_REQ_ICD_OPTION) == GlobalVariables.CommonStringTrue;
                IsShowPresAmount = GetValue(CONFIG_KEY__ShowPresAmount) == GlobalVariables.CommonStringTrue;
                IsFormClosingOption = GetValue(CONFIG_KEY__FormClosingOption) == GlobalVariables.CommonStringTrue;
                ModuleLinkApply = GetValue(CONFIG_KEY__ModuleLinkApply);
                IsExceedAvailableOutStock = GetValue(CONFIG_KEY__EXCEED_AVAILABLE_OUT_STOCK) == "1" ? true : false;
                TrackingCreate__UpdateTreatmentIcd = GetValue(CONFIG_KEY__TrackingCreate__UpdateTreatmentIcd);
                ContraindicaterWarningOption = GetValue(CONFIG_KEY__ICD_SERVICE__CONTRAINDICATED__WARNING_OPTION);
                NotHasEmrCoverTypeWarningOption = GetValue(CONFIG_KEY__HIS_PATIENT_PROGRAM__NOT_HAS_EMR_COVER_TYPE__WARNING_OPTION);
                DoNotAllowPresOutPatietInCaseOfHavingDebt = GetValue(CONFIG_KEY__HIS_SERVICE_REQ__DO_NOT_ALLOW_PRES_OUT_PATIENT_IN_CASE_OF_HAVING_DEBT);
                HisPatientProgramNotHasEmrCoverTypeWarningOption = GetValue(CONFIG_KEY__PRESCRIPTION__HIS_PATIENT_PROGRAM_NOT_HAS_EMR_COVER_TYPE_WARNING_OPTION);
                WarningWhenNotFinishingIncaseOfOutPatient = GetValue(CONFIG_KEY__PRESCRIPTION__WarningWhenNotFinishingIncaseOfOutPatient);
                WarningAllegericOption = GetValue(CONFIG_KEY__PRESCRIPTION__WARNING_ALLERGENIC_OPTION);
                AutoFocusToAdvise = GetValue(CONFIG_KEY__AutoFocusToAdvise) == GlobalVariables.CommonStringTrue;

                MedicineDebateOption = GetValue(CONFIG_KEY__MEDICINE_DEBATE_OPTION);
                AtcCodeOverlarWarningOption = GetValue(CONFIG_KEY__PRESCRIPTION_ATC_CODE_OVERLAP_WARNING_OPTION);

                SplitOffset = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(CONFIG_KEY__AssignPrescription__SplitOffset);

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
                IsTrackingRequired = GetValue(CONFIG_KEY__MOS_HIS_SERVICE_REQ_PRESCRIPTION_IS_TRACKING_REQUIRED) == GlobalVariables.CommonStringTrue;
                BhytColorCode = GetValue(CONFIG_KEY__BhytColorCode);
                UserMustHaveDiploma = GetValue(CONFIG_KEY__MOS_HIS_SERVICE_REQ_REQ_USER_MUST_HAVE_DIPLOMA) == GlobalVariables.CommonStringTrue;
                IsShowingInTheSameDepartment = GetValue(CONFIG_KEY__IsShowingInTheSameDepartment) == GlobalVariables.CommonStringTrue;
                IsAutoRoundUpByConvertUnitRatio = GetValue(CONFIG_KEY__IsAutoRoundUpByConvertUnitRatio) == GlobalVariables.CommonStringTrue;
                IsNotAutoGenerateTutorial = GetValue(CONFIG_KEY__HIS_Desktop_Plugins_AssignPrescription_IsNotAutoGenerateTutorial) == GlobalVariables.CommonStringTrue;
                IsWarningOddConvertAmount = GetValue(CONFIG_KEY__WARNING_ODD_CONVERT_AMOUNT) == GlobalVariables.CommonStringTrue;
                IsDontPresExpiredTime = GetValue(CONFIG_KEY__DONT_PRES_EXPIRED_ITEM) == GlobalVariables.CommonStringTrue;
                IsUsingWarningHeinFee = GetValue(CONFIG_KEY__IsUsingWarningHeinFee);
                InPatientPrescription__ShowRoundAvailableAmount = GetValue(CONFIG_KEY__ShowRoundAvailableAmount) == GlobalVariables.CommonStringTrue;
                ManyDayPrescriptionOption = Inventec.Common.TypeConvert.Parse.ToInt64(GetValue(CONFIG_KEY__MOS_HIS_SERVICE_REQ_MANY_DAYS_PRESCRIPTION_OPTION));
                IsAllowAssignPresByPackage = GetValue(CONFIG_KEY__IsAllowAssignPresByPackage) == GlobalVariables.CommonStringTrue;
                IsloadIcdFromExamServiceExecute = GetValue(CONFIG_KEY__IsloadIcdFromExamServiceExecute) == GlobalVariables.CommonStringTrue;
                IsAutoCreateSaleExpMest = GetValue(CONFIG_KEY__IS_AUTO_CREATE_SALE_EXP_MEST) == GlobalVariables.CommonStringTrue;
                IsAutoTickExpendWithAssignPresPTTT = GetValue(CONFIG_KEY__IsAutoTickExpendWithAssignPresPTTT) == GlobalVariables.CommonStringTrue;
                icdServiceHasCheck = Inventec.Common.TypeConvert.Parse.ToInt64(GetValue(ICD_SERVICE__HAS_CHECK));
                icdServiceHasRequireCheck = GetValue(ICD_SERVICE__HAS_REQUIRE_CHECK) == GlobalVariables.CommonStringTrue;
                icdServiceAllowUpdate = Inventec.Common.TypeConvert.Parse.ToInt64(GetValue(HIS_ICD_SERVICE__ALLOW_UPDATE));
                AcinInteractive__Grade = Inventec.Common.TypeConvert.Parse.ToInt64(GetValue(Key__AcinInteractive__Grade));
                IsVisilbleExecuteGroup = GetValue(IS_VISILBE_EXECUTE_GROUP_KEY);
                LoadPatientTypeDefault = GetValue(LOAD_PATIENT_TYPE_DEFAULT_KEY);
                OutStockListItemInCaseOfNoStockChosenOption = GetValue(CONFIG_KEY__OutStockListItemInCaseOfNoStockChosenOption);
                IsVisilbleTemplateMedicine = GetValue(IS_VISILBE_TEMPLATE_MEDICINE_KEY);

                ObligateIcd = GetValue(CONFIG_KEY__OBLIGATE_ICD);
                ShowRequestUser = GetValue(CONFIG_KEY__ShowRequestUser);
                AppointmentTimeDefault = (GetValue(TREATMENT_END___APPOINTMENT_TIME_DEFAULT_KEY));
                TreatmentEndHasAppointmentTimeDefault = (GetValue(PRESCRIPTION_TIME_AND_APPOINTMENT_TIME_KEY) == GlobalVariables.CommonStringTrue);
                TreatmentEndAppointmentTimeDefault = Inventec.Common.TypeConvert.Parse.ToInt64(AppointmentTimeDefault);

                string warringIntructionUseDayNumStr = GetValue(WARRING_INTRUCTION_USE_DAY_NUM);
                if (!String.IsNullOrEmpty(warringIntructionUseDayNumStr))
                {
                    WarringIntructionUseDayNum = Inventec.Common.TypeConvert.Parse.ToInt64(warringIntructionUseDayNumStr);
                }
                else
                {
                    WarringIntructionUseDayNum = null;
                }

                //IsMultiCheckservice = GetValue(CONFIG_KEY__IsMultiCheckservice);
                PatientTypeCode__BHYT = GetValue(CONFIG_KEY__PATIENT_TYPE_CODE__BHYT);
                PatientTypeId__BHYT = GetPatientTypeByCode(PatientTypeCode__BHYT).ID;
                PatientTypeCode__VP = GetValue(CONFIG_KEY__PATIENT_TYPE_CODE__VP);
                PatientTypeId__VP = GetPatientTypeByCode(PatientTypeCode__VP).ID;
                AutoCheckIcd = GetValue(CONFIG_KEY__ICD_GENERA_KEY);
                LimitHeinMedicinePrice__RightMediOrg = Inventec.Common.TypeConvert.Parse.ToDecimal(GetValue(CONFIG_KEY__LIMIT_HEIN_MEDICINE_PRICE__RIGHT_MEDI_ORG));
                LimitHeinMedicinePrice__NotRightMediOrg = Inventec.Common.TypeConvert.Parse.ToDecimal(GetValue(CONFIG_KEY__LIMIT_HEIN_MEDICINE_PRICE__NOT_RIGHT_MEDI_ORG));

                AmountDecimalNumber = Inventec.Common.TypeConvert.Parse.ToInt32(GetValue(CONFIG_KEY__AmountDecimalNumber));

                TreatmentTypeCode__Exam = GetTreatmentTypeById(IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM).TREATMENT_TYPE_CODE;
                TreatmentTypeCode__TreatIn = GetTreatmentTypeById(IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU).TREATMENT_TYPE_CODE;
                TreatmentTypeCode__TreatOut = GetTreatmentTypeById(IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNGOAITRU).TREATMENT_TYPE_CODE;

                IsCheckFinishTime = GetValue(CONFIG_KEY__TREATMENT_FINISH__CHECK_FINISH_TIME) == GlobalVariables.CommonStringTrue;
                isCheckBedService = GetValue(CHECK_ASSIGN_SERVICE_BED) == GlobalVariables.CommonStringTrue;
                isPrescriptionSplitOutMediStock = GetValue(MOS__PRESCRIPTION_SPLIT_OUT_MEDISTOCK) == GlobalVariables.CommonStringTrue;
                mustFinishAllServicesBeforeFinishTreatment = GetValue(MUST_FINISH_ALL_SERVICES_BEFORE_FINISH_TREATMENT_CFG) == GlobalVariables.CommonStringTrue;
                IsUsingServiceTime = GetValue(IS_USING_SERVER_TIME) == GlobalVariables.CommonStringTrue;
                BlockingInteractiveGrade = GetValue(CONFIG_KEY__BlockingInteractiveGrade);
                DefaultDrugStoreCode = GetValue(IS_CHOOSE_DRUGSTORE);
                string listCode = GetValue(SERVICE_CODE__AUTO_FINISH);
                if (!string.IsNullOrEmpty(listCode))
                {
                    var codes = listCode.Split(';');
                    autoFinishServiceIds = GetServiceIds(codes);
                }
                IsAutoCloseFormWithAutoConfigTreatmentFinish = GetValue(CONFIG_KEY____ISAUTO_CLOSE_FROM_WITH_AUTO_TREATMENT_FINISH) == GlobalVariables.CommonStringTrue;
                IsAllowPrintFinish = GetValue(Key__IsAllowPrintFinish) == GlobalVariables.CommonStringTrue;
                IsTutorialNumberIsFrac = GetValue(TUTORIAL_NUMBER_IS_FRAC) == GlobalVariables.CommonStringTrue;
                CheckSameHein = Inventec.Common.TypeConvert.Parse.ToInt64(GetValue(CHECK_SAME_HEIN_KEY));
                InitWarningOverCeiling();

                TutorialFormat = Inventec.Common.TypeConvert.Parse.ToInt64(GetValue(TUTORIAL_FORMAT));

                IsWarningOverTotalPatientPrice = GetValue(WARNING_OVER_TOTAL_PATIENT_PRICE__IS_CHECK) == GlobalVariables.CommonStringTrue;
                WarningOverTotalPatientPrice = Inventec.Common.TypeConvert.Parse.ToDecimal(GetValue(WARNING_OVER_TOTAL_PATIENT_PRICE));
                IsWarringUseDayAndExpTimeBHYT = GetValue(WARRING_USE_DAY_AND_EXP_TIME_BHYT) == GlobalVariables.CommonStringTrue;
                IsDefaultTracking = GetValue(KEY_IS_DEFAULT_TRACKING) == GlobalVariables.CommonStringTrue;
                MedicineHasPaymentLimitBHYT = GetValue(MEDICINE_HAS_PAYMENT_LIMIT_BHYT);
                IsNotAllowingExpendWithoutHavingParent = (GetValue(MOS__HIS_SERVICE_REQ__IS_NOT_ALLOWING_EXPEND_WITHOUT_HAVING_PARENT) == "1");
                IsCheckPreviousPrescription = (GetValue(CONFIG_KEY__IS_CHECK_PREVIOUS_PRESCRIPTION) == GlobalVariables.CommonStringTrue);
                IsUsingExecuteRoomPayment = GetValue(CONFIG_KEY__IS_USING_EXECUTE_ROOM_PAYMENT);

                ConnectDrugInterventionInfo = GetValue(CONFIG_KEY__CONNECT_DRUG_INTERVENTION_INFO);
                ConnectionInfo = GetValue(CONFIG_KEY__HIS_DRUG_INTERVENTION_CONNECTION_INFO);

                IsUsingSubPrescriptionMechanism = GetValue(CONFIG_KEY__IS_USING_SUB_PRESCRIPTION_MECHANISM);

                IsCheckDepartmentInTimeWhenPresOrAssign = (GetValue(IS_CHECK_DEPARTMENT_IN_TIME_WHEN_PRES_OR_ASSIGN) == "1");
                MestRoomOption = Inventec.Common.TypeConvert.Parse.ToInt64(GetValue(CONFIG_KEY__MestRoomOption));
                DefaultPatientTypeOption = (GetValue(CONFIG_KEY__DefaultPatientTypeOption) == "1");

                GroupOption = (GetValue(CONFIG_KEY__GroupOption) == "1");
                IsReasonRequired = (GetValue(CONFIG_KEY__IS_REASON_REQUIRED) == "1");
                IcdCodeToApplyRestrictPatientTypeByOtherSourcePaid = GetValue(CONFIG_KEY__ICD_CODE_TO_APPLY_RESTRICT_PATIENT_TYPE_BY_OTHER_SOURCE_PAID);
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

        static void InitWarningOverCeiling()
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

        private static List<long> GetServiceIds(string[] value)
        {
            List<long> result = new List<long>();
            try
            {
                if (value != null && value.Count() > 0)
                {
                    List<V_HIS_SERVICE> services = BackendDataWorker.Get<V_HIS_SERVICE>().Where(o => value.Contains(o.SERVICE_CODE)).ToList();
                    if (services != null && services.Count > 0)
                        result = services.Select(s => s.ID).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = new List<long>();
            }
            return result;
        }
    }
}
