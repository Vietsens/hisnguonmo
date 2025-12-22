using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.MchTreatmentExamService.TestData;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MCH.EFMODEL.DataModels;
using MCH.SDO;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using SDA.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.MchTreatmentExamService.MainForm
{
    public partial class UCMchTreatmentExamService : HIS.Desktop.Utility.FormBase
    {
        private void FillDataToForm()
        {
            try
            {
                btnNew_Click(null, null);
                // Kiểm tra nếu đã có ExamService thì không cần gọi API lấy Treatment
                bool hasExamService = ExamService != null && ExamService.ID > 0;
                V_MCH_EXAM_SERVICE examDisplay = ExamService;
                ExamServiceEdit = ExamService;
                if (!hasExamService)
                {
                    // Chỉ xử lý khi chưa có ExamService
                    // Tự động điền đủ 12 ký tự cho mã hồ sơ điều trị
                    string treatmentCode = !string.IsNullOrEmpty(txtTreatmentCode.Text.Trim()) ? txtTreatmentCode.Text.Trim().PadLeft(12, '0') : null;
                    txtTreatmentCode.Text = treatmentCode;
                    // Tự động điền đủ 10 ký tự cho mã bệnh nhân
                    string patientCode = string.IsNullOrEmpty(treatmentCode) && !string.IsNullOrEmpty(txtPatientCode.Text.Trim()) ? txtPatientCode.Text.Trim().PadLeft(10, '0') : null;
                    txtPatientCode.Text = patientCode;
                    try
                    {
                        if (!string.IsNullOrEmpty(treatmentCode) || !string.IsNullOrEmpty(patientCode))
                        {
                            CommonParam paramMch = new CommonParam();
                            MCH.Filter.MchExamServiceViewFilter mchFilter = new MCH.Filter.MchExamServiceViewFilter();
                            mchFilter.TREATMENT_CODE = treatmentCode;
                            // Note: Nếu MCH_TREATMENT có field PATIENT_CODE thì uncomment dòng dưới
                            mchFilter.PATIENT_CODE = patientCode;

                            var examDisplays = new BackendAdapter(paramMch).Get<List<V_MCH_EXAM_SERVICE>>(
                                "api/MchExamService/GetView",
                                ApiConsumers.MchConsumer,
                                mchFilter,
                                paramMch);

                            if (examDisplays != null && examDisplays.Count > 0)
                            {
                                examDisplay = examDisplays.OrderByDescending(o => o.ID).ToList()[0];
                                MCH.Filter.MchPatientFilter mchPatientFilter = new MCH.Filter.MchPatientFilter();
                                mchPatientFilter.PATIENT_CODE = examDisplay.PATIENT_CODE;
                                _patient = new BackendAdapter(paramMch).Get<List<MCH_PATIENT>>(
                                    "api/MchPatient/Get",
                                    ApiConsumers.MchConsumer,
                                    mchPatientFilter,
                                     paramMch).FirstOrDefault();
                                MCH.Filter.MchTreatmentFilter mchTreatmentFilter = new MCH.Filter.MchTreatmentFilter();
                                mchTreatmentFilter.TREATMENT_CODE = examDisplay.TREATMENT_CODE;
                                _treatment = new BackendAdapter(paramMch).Get<List<MCH_TREATMENT>>(
                                    "api/MchTreatment/Get",
                                    ApiConsumers.MchConsumer,
                                    mchTreatmentFilter,
                                    paramMch).FirstOrDefault();

                            }
                        }
                    }
                    catch (Exception exMch)
                    {
                        Inventec.Common.Logging.LogSystem.Warn("Error loading MCH_TREATMENT: " + exMch.Message);
                    }
                    // Bước 2: Nếu không tìm thấy trong MCH_TREATMENT, mới tìm trong HIS_TREATMENT
                    if (examDisplay == null)
                    {
                        _patient = null;
                        _treatment = null;
                        CommonParam param = new CommonParam();
                        HisTreatmentFilter filter = new HisTreatmentFilter();
                        filter.TREATMENT_CODE__EXACT = treatmentCode;
                        filter.TDL_PATIENT_CODE__EXACT = patientCode;

                        var apiResult = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_TREATMENT>>(
                            "api/HisTreatment/Get",
                            ApiConsumers.MosConsumer,
                            filter,
                            param);

                        if (apiResult != null && apiResult.Count > 0)
                        {
                            // Gắn Treatment với hồ sơ có ID lớn nhất
                            Treatment = apiResult.OrderByDescending(o => o.ID).FirstOrDefault();
                            Inventec.Common.Logging.LogSystem.Debug("Found HIS_TREATMENT - TreatmentCode: " + treatmentCode);
                        }
                    }
                }

                // Kiểm tra có dữ liệu để hiển thị không
                if ((Treatment == null || Treatment.ID < 0) && (ExamService == null || ExamService.ID < 0) && (examDisplay == null || examDisplay.ID < 0))
                {
                    ClearInfo();
                    return;
                }

                // Kiểm tra thông tin bệnh nhân
                if (!ValidatePatientInfo())
                {
                    return;
                }

                // Set Treatment cho TreeSereServ - Ưu tiên V_MCH_EXAM_SERVICE -> MCH_TREATMENT -> HIS_TREATMENT
                if (ucSereServ != null)
                {
                    HIS_TREATMENT treatmentForTree = null;
                    long treatmentId = 0;

                    if ((ExamService != null && ExamService.TREATMENT_ID > 0) || (examDisplay != null && examDisplay.ID > 0))
                    {
                        // Cần tìm TREATMENT_ID trong HIS từ TREATMENT_CODE của MCH

                        CommonParam param = new CommonParam();
                        HisTreatmentFilter filter = new HisTreatmentFilter();
                        filter.TREATMENT_CODE__EXACT = examDisplay != null && examDisplay.ID > 0 ? examDisplay.TREATMENT_CODE : ExamService.TREATMENT_CODE;

                        var hisTreatments = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_TREATMENT>>(
                            "api/HisTreatment/Get",
                            ApiConsumers.MosConsumer,
                            filter,
                            param);

                        if (hisTreatments != null && hisTreatments.Count > 0)
                        {
                            var hisTreatment = hisTreatments.OrderByDescending(o => o.ID).FirstOrDefault();
                            treatmentId = hisTreatment.ID;

                            // Gán luôn vào Treatment để dùng cho các mục đích khác
                            if (Treatment == null || Treatment.ID <= 0)
                            {
                                Treatment = hisTreatment;
                            }

                            Inventec.Common.Logging.LogSystem.Debug("TreeSereServ: Using MCH_TREATMENT -> HIS_TREATMENT.ID = " + treatmentId);
                        }
                    }
                    // Ưu tiên 3: Lấy từ HIS_TREATMENT
                    else if (Treatment != null && Treatment.ID > 0)
                    {
                        treatmentId = Treatment.ID;
                        Inventec.Common.Logging.LogSystem.Debug("TreeSereServ: Using HIS_TREATMENT.ID = " + treatmentId);
                    }

                    // Tạo HIS_TREATMENT object để truyền vào TreeSereServ
                    if (treatmentId > 0)
                    {
                        // Nếu đã có Treatment đầy đủ thì dùng luôn
                        if (Treatment != null && Treatment.ID == treatmentId)
                        {
                            treatmentForTree = Treatment;
                        }
                        else
                        {
                            // Tạo object tối thiểu chỉ có ID
                            treatmentForTree = new MOS.EFMODEL.DataModels.HIS_TREATMENT() { ID = treatmentId };
                        }

                        treeSereServ7Processor.SetTreatment(ucSereServ, treatmentForTree);
                        Inventec.Common.Logging.LogSystem.Debug("TreeSereServ: SetTreatment success with ID = " + treatmentId);
                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Warn("TreeSereServ: No valid TREATMENT_ID found");
                    }
                }

                // Hiện thị thông tin hồ sơ điều trị
                DisplayTreatmentInfo(examDisplay);
                LoadDataSereServ7();
                ReloadExamServiceGrid(examDisplay);
                if (examDisplay != null)
                    examDisplay.EXECUTE_LOGINNAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                // Set mặc định ngày khám và người khám cho nút Edit
                SetDefaultExamDateAndUser(examDisplay);
                xtraTabControl1.SelectedTabPageIndex = GetTabIndexFromExamServiceTypeId(examDisplay.EXAM_SERVICE_TYPE_ID);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void DisplayTreatmentInfo(V_MCH_EXAM_SERVICE examDisplay)
        {
            try
            {
                bool hasExamService = ExamService != null && ExamService.ID > 0;
                bool hasExamServiceDisplay = examDisplay != null && examDisplay.ID > 0;
                bool hasHisTreatment = Treatment != null && Treatment.ID > 0;

                // Ưu tiên 1: V_MCH_EXAM_SERVICE
                if (hasExamService)
                {
                    lblPatientCode.Text = ExamService.PATIENT_CODE;
                    lblPatientName.Text = ExamService.VIR_PATIENT_NAME;

                    lblDob.Text = ExamService.IS_HAS_NOT_DAY_DOB == 1
                        ? ExamService.DOB.ToString().Substring(0, 4)
                        : Inventec.Common.DateTime.Convert.TimeNumberToDateString(ExamService.DOB);
                    lblGenderName.Text = ExamService.GENDER_NAME;
                    lblTreatmentCode.Text = ExamService.TREATMENT_CODE;
                    lblHeinCardNumber.Text = ExamService.HEIN_CARD_NUMBER;
                    lblAddress.Text = !string.IsNullOrEmpty(ExamService.VIR_ADDRESS) ? ExamService.VIR_ADDRESS : ExamService.ADDRESS;
                    lblMediOrgName.Text = ExamService.HEIN_MEDI_ORG_NAME;

                    string heinDateFrom = Inventec.Common.DateTime.Convert.TimeNumberToDateString(ExamService.HEIN_CARD_FROM_TIME ?? 0);
                    string heinDateTo = Inventec.Common.DateTime.Convert.TimeNumberToDateString(ExamService.HEIN_CARD_TO_TIME ?? 0);
                    lblHeinDate.Text = heinDateFrom + " - " + heinDateTo;

                    Inventec.Common.Logging.LogSystem.Debug("DisplayTreatmentInfo: Using V_MCH_EXAM_SERVICE data");
                } // Ưu tiên 2: V_MCH_EXAM_SERVICE
                if (hasExamServiceDisplay)
                {
                    lblPatientCode.Text = examDisplay.PATIENT_CODE;
                    lblPatientName.Text = examDisplay.VIR_PATIENT_NAME;

                    lblDob.Text = examDisplay.IS_HAS_NOT_DAY_DOB == 1
                        ? examDisplay.DOB.ToString().Substring(0, 4)
                        : Inventec.Common.DateTime.Convert.TimeNumberToDateString(examDisplay.DOB);
                    lblGenderName.Text = examDisplay.GENDER_NAME;
                    lblTreatmentCode.Text = examDisplay.TREATMENT_CODE;
                    lblHeinCardNumber.Text = examDisplay.HEIN_CARD_NUMBER;
                    lblAddress.Text = !string.IsNullOrEmpty(examDisplay.VIR_ADDRESS) ? examDisplay.VIR_ADDRESS : examDisplay.ADDRESS;
                    lblMediOrgName.Text = examDisplay.HEIN_MEDI_ORG_NAME;

                    string heinDateFrom = Inventec.Common.DateTime.Convert.TimeNumberToDateString(examDisplay.HEIN_CARD_FROM_TIME ?? 0);
                    string heinDateTo = Inventec.Common.DateTime.Convert.TimeNumberToDateString(examDisplay.HEIN_CARD_TO_TIME ?? 0);
                    lblHeinDate.Text = heinDateFrom + " - " + heinDateTo;

                    Inventec.Common.Logging.LogSystem.Debug("DisplayTreatmentInfo: Using V_MCH_EXAM_SERVICE data");
                }
                // Ưu tiên 3: HIS_TREATMENT
                else if (hasHisTreatment)
                {
                    lblPatientCode.Text = Treatment.TDL_PATIENT_CODE;
                    lblPatientName.Text = Treatment.TDL_PATIENT_NAME;
                    lblDob.Text = Treatment.TDL_PATIENT_IS_HAS_NOT_DAY_DOB == 1
                        ? Treatment.TDL_PATIENT_DOB.ToString().Substring(0, 4)
                        : Inventec.Common.DateTime.Convert.TimeNumberToDateString(Treatment.TDL_PATIENT_DOB);
                    lblGenderName.Text = Treatment.TDL_PATIENT_GENDER_NAME;
                    lblTreatmentCode.Text = Treatment.TREATMENT_CODE;
                    lblHeinCardNumber.Text = Treatment.TDL_HEIN_CARD_NUMBER;
                    lblAddress.Text = Treatment.TDL_PATIENT_ADDRESS;
                    lblMediOrgName.Text = Treatment.TDL_HEIN_MEDI_ORG_NAME;

                    string heinDateFrom = Inventec.Common.DateTime.Convert.TimeNumberToDateString(Treatment.TDL_HEIN_CARD_FROM_TIME ?? 0);
                    string heinDateTo = Inventec.Common.DateTime.Convert.TimeNumberToDateString(Treatment.TDL_HEIN_CARD_TO_TIME ?? 0);
                    lblHeinDate.Text = heinDateFrom + " - " + heinDateTo;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool ValidatePatientInfo()
        {
            try
            {
                // Lấy thông tin giới tính và tuổi (tháng) theo thứ tự ưu tiên
                long? genderId = null;
                long dob = 0;
                xtraTabPage2.PageEnabled = true;
                xtraTabPage3.PageEnabled = true;
                xtraTabPage4.PageEnabled = true;
                xtraTabPage5.PageEnabled = true;
                xtraTabPage6.PageEnabled = true;
                xtraTabPage7.PageEnabled = true;
                // Ưu tiên 1: V_MCH_EXAM_SERVICE
                if (ExamService != null && ExamService.ID > 0)
                {
                    genderId = BackendDataWorker.Get<HIS_GENDER>().FirstOrDefault(o => o.GENDER_CODE == ExamService.GENDER_CODE)?.ID;
                    dob = ExamService.DOB;
                    Inventec.Common.Logging.LogSystem.Debug("ValidatePatientInfo: Using V_MCH_EXAM_SERVICE data");
                }
                // Ưu tiên 2: MCH_PATIENT
                else if (_patient != null && _patient.ID > 0)
                {
                    genderId = BackendDataWorker.Get<HIS_GENDER>().FirstOrDefault(o => o.GENDER_CODE == _patient.GENDER_CODE)?.ID;
                    dob = _patient.DOB;
                    Inventec.Common.Logging.LogSystem.Debug("ValidatePatientInfo: Using MCH_PATIENT data");
                }
                // Ưu tiên 3: HIS_TREATMENT
                else if (Treatment != null && Treatment.ID > 0)
                {
                    genderId = Treatment.TDL_PATIENT_GENDER_ID;
                    dob = Treatment.TDL_PATIENT_DOB;
                    Inventec.Common.Logging.LogSystem.Debug("ValidatePatientInfo: Using HIS_TREATMENT data");
                }

                if (dob == 0)
                {
                    Inventec.Common.Logging.LogSystem.Warn("ValidatePatientInfo: No valid DOB found");
                    return true; // Không có thông tin để validate thì cho qua
                }

                long? ageInMonths = CalculateAgeInMonths(dob);

                // Kiểm tra điều kiện: Giới tính là nam HOẶC (Giới tính là nam VÀ tuổi > 72 tháng)
                const long MALE_GENDER_ID = IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__MALE;

                bool isMale = genderId == MALE_GENDER_ID;
                bool isOlderThan6Years = ageInMonths.HasValue && ageInMonths.Value > 72;

                if (isMale && isOlderThan6Years)
                {
                    // Hiển thị cảnh báo
                    DevExpress.XtraEditors.XtraMessageBox.Show(
                        "Hồ sơ không phù hợp sử dụng chức năng",
                        "Cảnh báo",
                        System.Windows.Forms.MessageBoxButtons.OK,
                        System.Windows.Forms.MessageBoxIcon.Warning);

                    // Disable nút lưu
                    if (btnSave != null)
                    {
                        btnSave.Enabled = false;
                    }

                    return false;
                }
                if(isMale)
                {
                    xtraTabPage2.PageEnabled = false;
                    xtraTabPage3.PageEnabled = false;
                    xtraTabPage4.PageEnabled = false;
                    xtraTabPage5.PageEnabled = false;
                    xtraTabPage6.PageEnabled = false;
                    xtraTabPage7.PageEnabled = false;
                }    

                // Enable nút lưu nếu hợp lệ
                if (btnSave != null)
                {
                    btnSave.Enabled = true;
                }

                return true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }
        }

        private long CalculateAgeInMonths(long dob)
        {
            try
            {
                // Chuyển đổi DOB từ format yyyyMMddHHmmss sang DateTime
                string dobStr = dob.ToString();
                if (dobStr.Length < 8)
                {
                    return 0;
                }

                int year = int.Parse(dobStr.Substring(0, 4));
                int month = int.Parse(dobStr.Substring(4, 2));
                int day = int.Parse(dobStr.Substring(6, 2));

                DateTime birthDate = new DateTime(year, month, day);
                DateTime now = DateTime.Now;

                int months = ((now.Year - birthDate.Year) * 12) + now.Month - birthDate.Month;

                if (now.Day < birthDate.Day)
                {
                    months--;
                }

                return months;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return 0;
            }
        }
        private void ClearInfo()
        {
            lblAddress.Text = string.Empty;
            lblDob.Text = string.Empty;
            lblGenderName.Text = string.Empty;
            lblHeinCardNumber.Text = string.Empty;
            lblMediOrgName.Text = string.Empty;
            lblPatientCode.Text = string.Empty;
            lblTreatmentCode.Text = string.Empty;
            lblPatientName.Text = string.Empty;
            lblHeinDate.Text = string.Empty;
            ResetDataModels();
            gridControl1.DataSource = null;
        }

        private void ProcessSave()
        {
            try
            {
                // Validate tab hiện tại trước khi save
                if (!ValidateCurrentTab())
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(
                        "Vui lòng nhập đầy đủ thông tin bắt buộc (Ngày khám, Người khám, Trình độ)",
                        "Thông báo",
                        System.Windows.Forms.MessageBoxButtons.OK,
                        System.Windows.Forms.MessageBoxIcon.Warning);
                    return;
                }

                // Xác định Create hay Update dựa vào ExamServiceEdit
                bool isUpdate = (ExamServiceEdit != null && ExamServiceEdit.ID > 0);

                // Map Tab Index sang EXAM_SERVICE_TYPE_ID theo quy ước chuẩn
                // Tab 0 (Khám thai) → EXAM_SERVICE_TYPE_ID = 1
                // Tab 1 (Sinh đẻ) → EXAM_SERVICE_TYPE_ID = 2  
                // Tab 2 (Tránh thai) → EXAM_SERVICE_TYPE_ID = 3
                // Tab 3 (Phá thai) → EXAM_SERVICE_TYPE_ID = 4
                // Tab 4 (Sàng lọc) → EXAM_SERVICE_TYPE_ID = 5
                long currentExamServiceTypeId = GetExamServiceTypeIdFromTabIndex(xtraTabControl1.SelectedTabPageIndex);

                if (isUpdate && ExamServiceEdit.EXAM_SERVICE_TYPE_ID != currentExamServiceTypeId)
                {
                    string oldTypeName = GetExamServiceTypeName(ExamServiceEdit.EXAM_SERVICE_TYPE_ID);
                    string newTypeName = GetExamServiceTypeName(currentExamServiceTypeId);

                    DevExpress.XtraEditors.XtraMessageBox.Show(
                        string.Format("Không thể thay đổi loại dịch vụ khám từ '{0}' sang '{1}'.\n\nVui lòng chọn đúng tab '{0}' để cập nhật hoặc tạo mới dịch vụ khám khác.",
                            oldTypeName, newTypeName),
                        "Cảnh báo",
                        System.Windows.Forms.MessageBoxButtons.OK,
                        System.Windows.Forms.MessageBoxIcon.Warning);

                    // Tự động chuyển sang tab đúng
                    xtraTabControl1.SelectedTabPageIndex = GetTabIndexFromExamServiceTypeId(ExamServiceEdit.EXAM_SERVICE_TYPE_ID);
                    return;
                }

                if (!isUpdate)
                    ResetDataModels();

                // Map dữ liệu từ Treatment hoặc ExamService vào _patient, _treatment, _examService
                MapDataFromTreatmentOrExamService();

                // Get data from tab hiện tại theo EXAM_SERVICE_TYPE_ID
                GetDataFromCurrentTab(currentExamServiceTypeId);


                // ===== TH UPDATE: Gắn lại ExamServiceEdit sang ExamService và ràng buộc ID =====
                if (isUpdate)
                {
                    // Ràng buộc EXAM_SERVICE_ID vào dữ liệu các tab
                    BindExamServiceIdToTabData(ExamServiceEdit.ID);
                }

                CommonParam param = new CommonParam();
                bool apiResult = false;
                MCH_EXAM_SERVICE examService = null;

                if (isUpdate)
                {
                    // TH Update: Gọi api/MchExamService/UpdateBySdo
                    MCH.SDO.MchExamServiceUpdateBySDO examServiceUpdateBySDO = new MCH.SDO.MchExamServiceUpdateBySDO();
                    examServiceUpdateBySDO.ExamService = _examService;
                    examServiceUpdateBySDO.Screening = _screening;
                    examServiceUpdateBySDO.Child = _child;
                    examServiceUpdateBySDO.BirthInfo = _birthInfo;
                    examServiceUpdateBySDO.AntenatalVisit = _antenatalVisit;
                    examServiceUpdateBySDO.Contraception = _contraception;
                    examServiceUpdateBySDO.Abortion = _abortion;

                    Inventec.Common.Logging.LogSystem.Debug(ApiConsumers.MchConsumer.GetTokenCode() + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => examServiceUpdateBySDO), examServiceUpdateBySDO));

                    var apiResultUpdate = new BackendAdapter(param).Post<MchExamServiceUpdateBySdoResult>(
                        "api/MchExamService/UpdateBySdo",
                        ApiConsumers.MchConsumer,
                        examServiceUpdateBySDO,
                        param);
                    apiResult = apiResultUpdate != null;
                    if (apiResult)
                    {
                        examService = apiResultUpdate.ExamService;
                    }
                }
                else
                {
                    // TH Create: Gọi api/MchExamService/CreateBySdo
                    MCH.SDO.MchExamServiceCreateBySDO examServiceCreateBySDO = new MCH.SDO.MchExamServiceCreateBySDO();
                    _patient.ID = 0; // Đảm bảo tạo mới Patient
                    _treatment.ID = 0; // Đảm bảo tạo mới Treatment
                    _treatment.PATIENT_ID = 0; // Đảm bảo tạo mới Treatment
                    _examService.ID = 0; // Đảm bảo tạo mới ExamService
                    examServiceCreateBySDO.Patient = _patient;
                    examServiceCreateBySDO.Treatment = _treatment;
                    examServiceCreateBySDO.ExamService = _examService;
                    examServiceCreateBySDO.Screening = _screening;
                    examServiceCreateBySDO.Child = _child;
                    examServiceCreateBySDO.BirthInfo = _birthInfo;
                    examServiceCreateBySDO.AntenatalVisit = _antenatalVisit;
                    examServiceCreateBySDO.Contraception = _contraception;
                    examServiceCreateBySDO.Abortion = _abortion;

                    Inventec.Common.Logging.LogSystem.Debug(ApiConsumers.MchConsumer.GetTokenCode() + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => examServiceCreateBySDO), examServiceCreateBySDO));

                    var apiResultUpdate = new BackendAdapter(param).Post<MchExamServiceCreateBySdoResult>(
                        "api/MchExamService/CreateBySdo",
                        ApiConsumers.MchConsumer,
                        examServiceCreateBySDO,
                        param);
                    apiResult = apiResultUpdate != null;
                    if (apiResult)
                    {
                        examService = apiResultUpdate.ExamService;
                        _patient = apiResultUpdate.Patient;
                        _treatment = apiResultUpdate.Treatment;
                    }
                }

                #region Show message
                MessageManager.Show(this, param, apiResult);
                #endregion

                #region Process has exception
                SessionManager.ProcessTokenLost(param);
                #endregion

                Inventec.Common.Logging.LogSystem.Debug("Save SUCCESS - MCH_EXAM_SERVICE.ID: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => apiResult), apiResult));

                // Xử lý sau khi lưu thành công
                if (apiResult)
                {
                    // Reload lại danh sách V_MCH_EXAM_SERVICE
                    ReloadExamServiceGrid(new MCH.EFMODEL.DataModels.V_MCH_EXAM_SERVICE() { ID = examService.ID, PATIENT_CODE = _patient.PATIENT_CODE });
                    btnSave.Enabled = false;
                    //// Clear ExamServiceEdit để lần sau là Create
                    //ExamServiceEdit = null;

                    //// Clear dữ liệu form các tab
                    //ClearAllTabsData();
                    //InitAllSpinEditDefaultValue();
                    //SetDefaultExamDateAndUser(new V_MCH_EXAM_SERVICE() { IN_TIME = _treatment.IN_TIME, EXECUTE_LOGINNAME = examService.EXECUTE_LOGINNAME });
                    // Focus vào row vừa lưu trong grid
                    FocusToSavedRow(examService.ID);

                    if (dlgRefresh != null)
                        dlgRefresh();

                    Inventec.Common.Logging.LogSystem.Debug("ProcessSave completed successfully");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                DevExpress.XtraEditors.XtraMessageBox.Show(
                    "Có lỗi xảy ra: " + ex.Message,
                    "Lỗi",
                    System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Ràng buộc EXAM_SERVICE_ID vào dữ liệu các tab khi Update
        /// </summary>
        private void BindExamServiceIdToTabData(long examServiceId)
        {
            try
            {
                // Ràng buộc ID cho Screening (Tab Sàng lọc)
                if (_screening != null)
                {
                    _screening.EXAM_SERVICE_ID = examServiceId;
                }

                // Ràng buộc ID cho AntenatalVisit (Tab Khám thai)
                if (_antenatalVisit != null)
                {
                    _antenatalVisit.EXAM_SERVICE_ID = examServiceId;
                }

                // Ràng buộc ID cho BirthInfo (Tab Sinh đẻ - Mẹ)
                if (_birthInfo != null)
                {
                    _birthInfo.EXAM_SERVICE_ID = examServiceId;
                }

                // Ràng buộc ID cho Child (Tab Sinh đẻ - Con)
                if (_child != null)
                {
                    _child.EXAM_SERVICE_ID = examServiceId;
                }

                // Ràng buộc ID cho Contraception (Tab Tránh thai)
                if (_contraception != null)
                {
                    _contraception.EXAM_SERVICE_ID = examServiceId;
                }

                // Ràng buộc ID cho Abortion (Tab Phá thai)
                if (_abortion != null)
                {
                    _abortion.EXAM_SERVICE_ID = examServiceId;
                    Inventec.Common.Logging.LogSystem.Debug("Bound _abortion.EXAM_SERVICE_ID = " + examServiceId);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Get data from tab hiện tại theo EXAM_SERVICE_TYPE_ID
        /// </summary>
        private void GetDataFromCurrentTab(long examServiceTypeId)
        {
            try
            {
                switch (examServiceTypeId)
                {
                    case 1: // Khám thai
                        GetDataFromTab2();
                        break;
                    case 2: // Sinh đẻ
                        GetDataFromTab3Mother();
                        GetDataFromTab3Child();
                        break;
                    case 3: // Tránh thai
                        GetDataFromTab4();
                        break;
                    case 4: // Phá thai
                        GetDataFromTab5();
                        break;
                    case 5: // Sàng lọc
                        GetDataFromTab1();
                        break;
                    default:
                        Inventec.Common.Logging.LogSystem.Warn("Invalid EXAM_SERVICE_TYPE_ID: " + examServiceTypeId);
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Map EXAM_SERVICE_TYPE_ID sang Tab Index
        /// 1 (Khám thai) → Tab 1
        /// 2 (Sinh đẻ) → Tab 2
        /// 3 (Tránh thai) → Tab 3
        /// 4 (Phá thai) → Tab 4
        /// 5 (Sàng lọc) → Tab 0
        /// </summary>
        private int GetTabIndexFromExamServiceTypeId(long examServiceTypeId)
        {
            switch (examServiceTypeId)
            {
                case 1: return 1; // Khám thai
                case 2: return 2; // Sinh đẻ
                case 3: return 3; // Tránh thai
                case 4: return 4; // Phá thai
                case 5: return 0; // Sàng lọc
                default: return 0;
            }
        }

        /// <summary>
        /// Map Tab Index sang EXAM_SERVICE_TYPE_ID
        /// Tab 0 → 5 (Sàng lọc)
        /// Tab 1 → 1 (Khám thai)
        /// Tab 2 → 2 (Sinh đẻ)
        /// Tab 3 → 3 (Tránh thai)
        /// Tab 4 → 4 (Phá thai)
        /// </summary>
        private long GetExamServiceTypeIdFromTabIndex(int tabIndex)
        {
            switch (tabIndex)
            {
                case 0: return 5; // Sàng lọc
                case 1: return 1; // Khám thai
                case 2: return 2; // Sinh đẻ
                case 3: return 3; // Tránh thai
                case 4: return 4; // Phá thai
                default: return 5;
            }
        }

        // =============== Mapping Data =================
        private void MapDataFromTreatmentOrExamService()
        {
            try
            {
                // Xác định thứ tự ưu tiên dữ liệu: ExamServiceEdit > ExamService > _patient/_treatment > Treatment
                bool hasExamServiceEdit = ExamServiceEdit != null && ExamServiceEdit.ID > 0;
                bool hasExamService = ExamService != null && ExamService.ID > 0;
                bool hasMchPatient = _patient != null && _patient.ID > 0;
                bool hasMchTreatment = _treatment != null && _treatment.ID > 0;
                bool hasHisTreatment = Treatment != null && Treatment.ID > 0;

                // ============ Map MCH_PATIENT ============
                if (_patient == null)
                {
                    _patient = new MCH_PATIENT();
                }

                // Ưu tiên 1: ExamServiceEdit
                if (hasExamServiceEdit)
                {
                    if (ExamServiceEdit.PATIENT_ID > 0)
                    {
                        _patient.ID = ExamServiceEdit.PATIENT_ID;
                    }

                    _patient.PATIENT_CODE = ExamServiceEdit.PATIENT_CODE;
                    _patient.FIRST_NAME = ExamServiceEdit.FIRST_NAME;
                    _patient.LAST_NAME = ExamServiceEdit.LAST_NAME;
                    _patient.DOB = ExamServiceEdit.DOB;
                    _patient.IS_HAS_NOT_DAY_DOB = ExamServiceEdit.IS_HAS_NOT_DAY_DOB;
                    _patient.GENDER_CODE = ExamServiceEdit.GENDER_CODE;
                    _patient.GENDER_NAME = ExamServiceEdit.GENDER_NAME;
                    _patient.CCCD_NUMBER = ExamServiceEdit.CCCD_NUMBER;
                    _patient.CCCD_DATE = ExamServiceEdit.CCCD_DATE;
                    _patient.CCCD_PLACE = ExamServiceEdit.CCCD_PLACE;
                    _patient.ETHNIC_CODE = ExamServiceEdit.ETHNIC_CODE;
                    _patient.ETHNIC_NAME = ExamServiceEdit.ETHNIC_NAME;
                    _patient.CAREER_CODE = ExamServiceEdit.CAREER_CODE;
                    _patient.CAREER_NAME = ExamServiceEdit.CAREER_NAME;
                    _patient.ADDRESS = ExamServiceEdit.ADDRESS;
                    _patient.COMMUNE_CODE = ExamServiceEdit.COMMUNE_CODE;
                    _patient.COMMUNE_NAME = ExamServiceEdit.COMMUNE_NAME;
                    _patient.DISTRICT_CODE = ExamServiceEdit.DISTRICT_CODE;
                    _patient.DISTRICT_NAME = ExamServiceEdit.DISTRICT_NAME;
                    _patient.PROVINCE_CODE = ExamServiceEdit.PROVINCE_CODE;
                    _patient.PROVINCE_NAME = ExamServiceEdit.PROVINCE_NAME;
                    _patient.PHONE_NUMBER = ExamServiceEdit.PHONE_NUMBER;
                    _patient.HT_ADDRESS = ExamServiceEdit.HT_ADDRESS;
                    _patient.HT_COMMUNE_CODE = ExamServiceEdit.HT_COMMUNE_CODE;
                    _patient.HT_COMMUNE_NAME = ExamServiceEdit.HT_COMMUNE_NAME;
                    _patient.HT_DISTRICT_CODE = ExamServiceEdit.HT_DISTRICT_CODE;
                    _patient.HT_DISTRICT_NAME = ExamServiceEdit.HT_DISTRICT_NAME;
                    _patient.HT_PROVINCE_CODE = ExamServiceEdit.HT_PROVINCE_CODE;
                    _patient.HT_PROVINCE_NAME = ExamServiceEdit.HT_PROVINCE_NAME;

                    Inventec.Common.Logging.LogSystem.Debug("MapMchPatient: Using ExamServiceEdit data");
                }
                // Ưu tiên 2: ExamService
                else if (hasExamService)
                {
                    if (ExamService.PATIENT_ID > 0)
                    {
                        _patient.ID = ExamService.PATIENT_ID;
                    }

                    _patient.PATIENT_CODE = ExamService.PATIENT_CODE;
                    _patient.FIRST_NAME = ExamService.FIRST_NAME;
                    _patient.LAST_NAME = ExamService.LAST_NAME;
                    _patient.DOB = ExamService.DOB;
                    _patient.IS_HAS_NOT_DAY_DOB = ExamService.IS_HAS_NOT_DAY_DOB;
                    _patient.GENDER_CODE = ExamService.GENDER_CODE;
                    _patient.GENDER_NAME = ExamService.GENDER_NAME;
                    _patient.CCCD_NUMBER = ExamService.CCCD_NUMBER;
                    _patient.CCCD_DATE = ExamService.CCCD_DATE;
                    _patient.CCCD_PLACE = ExamService.CCCD_PLACE;
                    _patient.ETHNIC_CODE = ExamService.ETHNIC_CODE;
                    _patient.ETHNIC_NAME = ExamService.ETHNIC_NAME;
                    _patient.CAREER_CODE = ExamService.CAREER_CODE;
                    _patient.CAREER_NAME = ExamService.CAREER_NAME;
                    _patient.ADDRESS = ExamService.ADDRESS;
                    _patient.COMMUNE_CODE = ExamService.COMMUNE_CODE;
                    _patient.COMMUNE_NAME = ExamService.COMMUNE_NAME;
                    _patient.DISTRICT_CODE = ExamService.DISTRICT_CODE;
                    _patient.DISTRICT_NAME = ExamService.DISTRICT_NAME;
                    _patient.PROVINCE_CODE = ExamService.PROVINCE_CODE;
                    _patient.PROVINCE_NAME = ExamService.PROVINCE_NAME;
                    _patient.PHONE_NUMBER = ExamService.PHONE_NUMBER;
                    _patient.HT_ADDRESS = ExamService.HT_ADDRESS;
                    _patient.HT_COMMUNE_CODE = ExamService.HT_COMMUNE_CODE;
                    _patient.HT_COMMUNE_NAME = ExamService.HT_COMMUNE_NAME;
                    _patient.HT_DISTRICT_CODE = ExamService.HT_DISTRICT_CODE;
                    _patient.HT_DISTRICT_NAME = ExamService.HT_DISTRICT_NAME;
                    _patient.HT_PROVINCE_CODE = ExamService.HT_PROVINCE_CODE;
                    _patient.HT_PROVINCE_NAME = ExamService.HT_PROVINCE_NAME;

                    Inventec.Common.Logging.LogSystem.Debug("MapMchPatient: Using ExamService data");
                }
                // Ưu tiên 3: _patient (đã có sẵn từ query MCH)
                else if (hasMchPatient)
                {
                    // Không cần map lại, _patient đã có dữ liệu
                    Inventec.Common.Logging.LogSystem.Debug("MapMchPatient: Using existing MCH_PATIENT data");
                }
                // Ưu tiên 4: Treatment
                else if (hasHisTreatment)
                {
                    _patient.PATIENT_CODE = Treatment.TDL_PATIENT_CODE;
                    _patient.FIRST_NAME = Treatment.TDL_PATIENT_FIRST_NAME;
                    _patient.LAST_NAME = Treatment.TDL_PATIENT_LAST_NAME;
                    _patient.DOB = Treatment.TDL_PATIENT_DOB;
                    _patient.IS_HAS_NOT_DAY_DOB = Treatment.TDL_PATIENT_IS_HAS_NOT_DAY_DOB;
                    var gender = BackendDataWorker.Get<HIS_GENDER>().FirstOrDefault(o => o.ID == Treatment.TDL_PATIENT_GENDER_ID);
                    _patient.GENDER_CODE = gender != null ? gender.GENDER_CODE : null;
                    _patient.GENDER_NAME = Treatment.TDL_PATIENT_GENDER_NAME;
                    _patient.CCCD_NUMBER = Treatment.TDL_PATIENT_CCCD_NUMBER;
                    _patient.CCCD_DATE = Treatment.TDL_PATIENT_CCCD_DATE;
                    _patient.CCCD_PLACE = Treatment.TDL_PATIENT_CCCD_PLACE;
                    var ethenic = BackendDataWorker.Get<SDA_ETHNIC>().FirstOrDefault(o => o.ETHNIC_NAME == Treatment.TDL_PATIENT_ETHNIC_NAME);
                    _patient.ETHNIC_CODE = ethenic != null ? ethenic.ETHNIC_CODE : null;
                    _patient.ETHNIC_NAME = Treatment.TDL_PATIENT_ETHNIC_NAME;
                    _patient.CAREER_CODE = Treatment.TDL_PATIENT_CAREER_CODE;
                    _patient.CAREER_NAME = Treatment.TDL_PATIENT_CAREER_NAME;
                    _patient.ADDRESS = Treatment.TDL_PATIENT_ADDRESS;
                    _patient.COMMUNE_CODE = Treatment.TDL_PATIENT_COMMUNE_CODE;
                    _patient.COMMUNE_NAME = Treatment.TDL_PATIENT_COMMUNE_NAME;
                    _patient.DISTRICT_CODE = Treatment.TDL_PATIENT_DISTRICT_CODE;
                    _patient.DISTRICT_NAME = Treatment.TDL_PATIENT_DISTRICT_NAME;
                    _patient.PROVINCE_CODE = Treatment.TDL_PATIENT_PROVINCE_CODE;
                    _patient.PROVINCE_NAME = Treatment.TDL_PATIENT_PROVINCE_NAME;
                    _patient.PHONE_NUMBER = Treatment.TDL_PATIENT_PHONE;

                    Inventec.Common.Logging.LogSystem.Debug("MapMchPatient: Using HIS_TREATMENT data");
                }

                // ============ Map MCH_TREATMENT ============
                if (_treatment == null)
                {
                    _treatment = new MCH_TREATMENT();
                }

                // Ưu tiên 1: ExamServiceEdit
                if (hasExamServiceEdit)
                {
                    if (ExamServiceEdit.PATIENT_ID > 0)
                    {
                        _treatment.PATIENT_ID = ExamServiceEdit.PATIENT_ID;
                    }
                    else if (_patient != null && _patient.ID > 0)
                    {
                        _treatment.PATIENT_ID = _patient.ID;
                    }

                    _treatment.TREATMENT_CODE = ExamServiceEdit.TREATMENT_CODE;
                    _treatment.BRANCH_MEDI_ORG_CODE = ExamServiceEdit.BRANCH_MEDI_ORG_CODE;
                    _treatment.BRANCH_MEDI_ORG_NAME = ExamServiceEdit.BRANCH_MEDI_ORG_NAME;
                    _treatment.DIRECTOR_LOGINNAME = ExamServiceEdit.DIRECTOR_LOGINNAME;
                    _treatment.DIRECTOR_USERNAME = ExamServiceEdit.DIRECTOR_USERNAME;
                    _treatment.HOSP_SUBS_DIRECTOR_LOGINNAME = ExamServiceEdit.HOSP_SUBS_DIRECTOR_LOGINNAME;
                    _treatment.HOSP_SUBS_DIRECTOR_USERNAME = ExamServiceEdit.HOSP_SUBS_DIRECTOR_USERNAME;
                    _treatment.IN_TIME = ExamServiceEdit.IN_TIME;
                    _treatment.IN_DATE = ExamServiceEdit.IN_DATE;
                    _treatment.HEIN_CARD_ADDRESS = ExamServiceEdit.HEIN_CARD_ADDRESS;
                    _treatment.HEIN_CARD_FROM_TIME = ExamServiceEdit.HEIN_CARD_FROM_TIME;
                    _treatment.HEIN_CARD_NUMBER = ExamServiceEdit.HEIN_CARD_NUMBER;
                    _treatment.HEIN_CARD_TO_TIME = ExamServiceEdit.HEIN_CARD_TO_TIME;
                    _treatment.HEIN_MEDI_ORG_CODE = ExamServiceEdit.HEIN_MEDI_ORG_CODE;
                    _treatment.HEIN_MEDI_ORG_NAME = ExamServiceEdit.HEIN_MEDI_ORG_NAME;
                    _treatment.JOIN_5_YEAR = ExamServiceEdit.JOIN_5_YEAR;
                    _treatment.LIVE_AREA_CODE = ExamServiceEdit.LIVE_AREA_CODE;
                    _treatment.PAID_6_MONTH = ExamServiceEdit.PAID_6_MONTH;

                    _treatment.DIRECTOR_LOGINNAME = ExamServiceEdit.DIRECTOR_LOGINNAME;
                    _treatment.DIRECTOR_USERNAME = ExamServiceEdit.DIRECTOR_USERNAME;
                    Inventec.Common.Logging.LogSystem.Debug("MapMchTreatment: Using ExamServiceEdit data");
                }
                // Ưu tiên 2: ExamService
                else if (hasExamService)
                {
                    if (ExamService.PATIENT_ID > 0)
                    {
                        _treatment.PATIENT_ID = ExamService.PATIENT_ID;
                    }
                    else if (_patient != null && _patient.ID > 0)
                    {
                        _treatment.PATIENT_ID = _patient.ID;
                    }

                    _treatment.TREATMENT_CODE = ExamService.TREATMENT_CODE;
                    _treatment.BRANCH_MEDI_ORG_CODE = ExamService.BRANCH_MEDI_ORG_CODE;
                    _treatment.BRANCH_MEDI_ORG_NAME = ExamService.BRANCH_MEDI_ORG_NAME;
                    _treatment.DIRECTOR_LOGINNAME = ExamService.DIRECTOR_LOGINNAME;
                    _treatment.DIRECTOR_USERNAME = ExamService.DIRECTOR_USERNAME;
                    _treatment.HOSP_SUBS_DIRECTOR_LOGINNAME = ExamService.HOSP_SUBS_DIRECTOR_LOGINNAME;
                    _treatment.HOSP_SUBS_DIRECTOR_USERNAME = ExamService.HOSP_SUBS_DIRECTOR_USERNAME;
                    _treatment.IN_TIME = ExamService.IN_TIME;
                    _treatment.IN_DATE = ExamService.IN_DATE;
                    _treatment.HEIN_CARD_ADDRESS = ExamService.HEIN_CARD_ADDRESS;
                    _treatment.HEIN_CARD_FROM_TIME = ExamService.HEIN_CARD_FROM_TIME;
                    _treatment.HEIN_CARD_NUMBER = ExamService.HEIN_CARD_NUMBER;
                    _treatment.HEIN_CARD_TO_TIME = ExamService.HEIN_CARD_TO_TIME;
                    _treatment.HEIN_MEDI_ORG_CODE = ExamService.HEIN_MEDI_ORG_CODE;
                    _treatment.HEIN_MEDI_ORG_NAME = ExamService.HEIN_MEDI_ORG_NAME;
                    _treatment.JOIN_5_YEAR = ExamService.JOIN_5_YEAR;
                    _treatment.LIVE_AREA_CODE = ExamService.LIVE_AREA_CODE;
                    _treatment.PAID_6_MONTH = ExamService.PAID_6_MONTH;
                    _treatment.DIRECTOR_LOGINNAME = ExamService.DIRECTOR_LOGINNAME;
                    _treatment.DIRECTOR_USERNAME = ExamService.DIRECTOR_USERNAME;
                    Inventec.Common.Logging.LogSystem.Debug("MapMchTreatment: Using ExamService data");
                }
                // Ưu tiên 3: _treatment (đã có sẵn từ query MCH)
                else if (hasMchTreatment)
                {
                    // Không cần map lại, _treatment đã có dữ liệu
                    // Chỉ cần map PATIENT_ID nếu có
                    if (_patient != null && _patient.ID > 0)
                    {
                        _treatment.PATIENT_ID = _patient.ID;
                    }
                    Inventec.Common.Logging.LogSystem.Debug("MapMchTreatment: Using existing MCH_TREATMENT data");
                }
                // Ưu tiên 4: Treatment
                else if (hasHisTreatment)
                {
                    _treatment.TREATMENT_CODE = Treatment.TREATMENT_CODE;

                    if (_patient != null && _patient.ID > 0)
                    {
                        _treatment.PATIENT_ID = _patient.ID;
                    }

                    var branch = BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == Treatment.BRANCH_ID);
                    if (branch != null)
                    {
                        _treatment.BRANCH_MEDI_ORG_CODE = branch.BRANCH_CODE;
                        _treatment.BRANCH_MEDI_ORG_NAME = branch.BRANCH_NAME;
                        _treatment.DIRECTOR_LOGINNAME = branch.DIRECTOR_LOGINNAME;
                        _treatment.DIRECTOR_USERNAME = branch.DIRECTOR_USERNAME;
                    }
                    _treatment.IN_TIME = Treatment.IN_TIME;
                    _treatment.IN_DATE = Treatment.IN_DATE;
                    if (Treatment != null && Treatment.ID > 0)
                        LoadLatestPatientTypeAlter(Treatment.ID);

                    Inventec.Common.Logging.LogSystem.Debug("MapMchTreatment: Using HIS_TREATMENT data");
                }

                // ============ Map MCH_EXAM_SERVICE ============
                if (_examService == null)
                {
                    _examService = new MCH_EXAM_SERVICE();
                }

                // Map EXAM_SERVICE_TYPE_ID từ tab hiện tại
                _examService.EXAM_SERVICE_TYPE_ID = GetExamServiceTypeIdFromTabIndex(xtraTabControl1.SelectedTabPageIndex);

                // Ưu tiên 1: ExamServiceEdit (đang edit)
                if (hasExamServiceEdit)
                {
                    _examService.ID = ExamServiceEdit.ID;
                    _examService.TREATMENT_ID = ExamServiceEdit.TREATMENT_ID;
                    _examService.MEDI_ORG_CODE = ExamServiceEdit.MEDI_ORG_CODE;
                    _examService.MEDI_ORG_NAME = ExamServiceEdit.MEDI_ORG_NAME;
                    _examService.EXECUTE_LOGINNAME = ExamServiceEdit.EXECUTE_LOGINNAME;
                    _examService.EXECUTE_USERNAME = ExamServiceEdit.EXECUTE_USERNAME;
                    _examService.EXECUTE_TYPE = ExamServiceEdit.EXECUTE_TYPE;
                    _examService.SYNC_STATUS = null;
                    _examService.SYNC_TIME = null;
                    _examService.SYNC_DESCRIPTION = null;
                    if (ExamServiceEdit.EXAM_SERVICE_TYPE_ID > 0)
                    {
                        _examService.EXAM_SERVICE_TYPE_ID = ExamServiceEdit.EXAM_SERVICE_TYPE_ID;
                    }

                    Inventec.Common.Logging.LogSystem.Debug("MapMchExamService: Using ExamServiceEdit data");
                }
                // Ưu tiên 2: ExamService
                else if (hasExamService)
                {
                    _examService.TREATMENT_ID = ExamService.TREATMENT_ID;
                    _examService.MEDI_ORG_CODE = ExamService.MEDI_ORG_CODE;
                    _examService.MEDI_ORG_NAME = ExamService.MEDI_ORG_NAME;
                    _examService.EXECUTE_LOGINNAME = ExamService.EXECUTE_LOGINNAME;
                    _examService.EXECUTE_USERNAME = ExamService.EXECUTE_USERNAME;
                    _examService.EXECUTE_TYPE = ExamService.EXECUTE_TYPE;
                    _examService.SYNC_STATUS = null;
                    _examService.SYNC_TIME = null;
                    _examService.SYNC_DESCRIPTION = null;
                    if (ExamService.EXAM_SERVICE_TYPE_ID > 0)
                    {
                        _examService.EXAM_SERVICE_TYPE_ID = ExamService.EXAM_SERVICE_TYPE_ID;
                    }

                    Inventec.Common.Logging.LogSystem.Debug("MapMchExamService: Using ExamService data");
                }
                // Ưu tiên 3: Treatment
                else if (hasHisTreatment)
                {
                    var branch = BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == Treatment.BRANCH_ID);
                    if (branch != null)
                    {
                        _examService.MEDI_ORG_CODE = branch.HEIN_MEDI_ORG_CODE;
                        _examService.MEDI_ORG_NAME = BackendDataWorker.Get<HIS_MEDI_ORG>().FirstOrDefault(o=>o.MEDI_ORG_CODE == branch.HEIN_MEDI_ORG_CODE).MEDI_ORG_NAME;
                    }

                    Inventec.Common.Logging.LogSystem.Debug("MapMchExamService: Using HIS_TREATMENT data");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadLatestPatientTypeAlter(long treatmentId)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisPatientTypeAlterViewFilter filter = new HisPatientTypeAlterViewFilter();
                filter.TREATMENT_ID = treatmentId;
                filter.ORDER_FIELD = "LOG_TIME";
                filter.ORDER_DIRECTION = "DESC";

                var patientTypeAlters = new BackendAdapter(param).Get<List<HIS_PATIENT_TYPE_ALTER>>(
                    "api/HisPatientTypeAlter/Get",
                    ApiConsumers.MosConsumer,
                    filter,
                    param);

                if (patientTypeAlters != null && patientTypeAlters.Count > 0)
                {
                    var latestAlter = patientTypeAlters.OrderByDescending(o => o.LOG_TIME).FirstOrDefault();

                    if (latestAlter != null && _treatment != null)
                    {
                        _treatment.HEIN_CARD_ADDRESS = latestAlter.ADDRESS;
                        _treatment.HEIN_CARD_FROM_TIME = latestAlter.HEIN_CARD_FROM_TIME;
                        _treatment.HEIN_CARD_NUMBER = latestAlter.HEIN_CARD_NUMBER;
                        _treatment.HEIN_CARD_TO_TIME = latestAlter.HEIN_CARD_TO_TIME;
                        _treatment.HEIN_MEDI_ORG_CODE = latestAlter.HEIN_MEDI_ORG_CODE;
                        _treatment.HEIN_MEDI_ORG_NAME = latestAlter.HEIN_MEDI_ORG_NAME;
                        _treatment.JOIN_5_YEAR = latestAlter.JOIN_5_YEAR;
                        _treatment.LIVE_AREA_CODE = latestAlter.LIVE_AREA_CODE;
                        _treatment.PAID_6_MONTH = latestAlter.PAID_6_MONTH;

                        Inventec.Common.Logging.LogSystem.Debug("LoadLatestPatientTypeAlter SUCCESS - TreatmentId: " + treatmentId);
                    }
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Warn("No PatientTypeAlter found for TreatmentId: " + treatmentId);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Lấy tên loại dịch vụ khám theo EXAM_SERVICE_TYPE_ID
        /// </summary>
        private string GetExamServiceTypeName(long examServiceTypeId)
        {
            switch (examServiceTypeId)
            {
                case 1: return "Khám thai";
                case 2: return "Sinh đẻ";
                case 3: return "Tránh thai";
                case 4: return "Phá thai";
                case 5: return "Sàng lọc ung thư cổ tử cung";
                default: return "Không xác định";
            }
        }

        /// <summary>
        /// Lấy tên loại dịch vụ khám theo xtraTabPage.Text (ưu tiên)
        /// </summary>
        private string GetExamServiceTypeNameByTabPage(long examServiceTypeId)
        {
            try
            {
                if (xtraTabControl1 != null && xtraTabControl1.TabPages.Count > 0)
                {
                    int tabIndex = GetTabIndexFromExamServiceTypeId(examServiceTypeId);
                    if (tabIndex >= 0 && tabIndex < xtraTabControl1.TabPages.Count)
                    {
                        var tabPage = xtraTabControl1.TabPages[tabIndex];
                        if (tabPage != null && !string.IsNullOrEmpty(tabPage.Text))
                        {
                            return tabPage.Text;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            // Fallback về hàm cũ nếu không lấy được từ TabPage
            return GetExamServiceTypeName(examServiceTypeId);
        }


        /// <summary>
        /// Reload lại danh sách V_MCH_EXAM_SERVICE trong grid
        /// </summary>
        private void ReloadExamServiceGrid(V_MCH_EXAM_SERVICE examDisplay)
        {
            try
            {
                WaitingManager.Show();

                // Lấy PATIENT_CODE để filter
                string patientCode = null;

                if (ExamService != null && !string.IsNullOrEmpty(ExamService.PATIENT_CODE))
                {
                    patientCode = ExamService.PATIENT_CODE;
                }
                else if (examDisplay != null && !string.IsNullOrEmpty(examDisplay.PATIENT_CODE))
                {
                    patientCode = examDisplay.PATIENT_CODE;
                }
                else if (_patient != null && !string.IsNullOrEmpty(_patient.PATIENT_CODE))
                {
                    patientCode = _patient.PATIENT_CODE;
                }

                if (string.IsNullOrEmpty(patientCode))
                {
                    Inventec.Common.Logging.LogSystem.Warn("ReloadExamServiceGrid: PATIENT_CODE is null, cannot reload");
                    WaitingManager.Hide();
                    return;
                }
                // Gọi API lấy danh sách V_MCH_EXAM_SERVICE
                MCH.Filter.MchExamServiceViewFilter filter = new MCH.Filter.MchExamServiceViewFilter();
                filter.PATIENT_CODE = patientCode;
                var dataSource = new BackendAdapter(new CommonParam()).Get<List<MCH.EFMODEL.DataModels.V_MCH_EXAM_SERVICE>>(
                    "api/MchExamService/GetView",
                    ApiConsumers.MchConsumer,
                    filter,
                    new CommonParam());

                if (dataSource != null && dataSource.Count > 0)
                {
                    dataSource = dataSource.OrderByDescending(o => o.EXECUTE_TIME).ThenByDescending(o => o.ID).ToList();
                    gridControl1.DataSource = dataSource;
                    Inventec.Common.Logging.LogSystem.Debug("ReloadExamServiceGrid: Loaded " + dataSource.Count + " records");
                }
                else
                {
                    gridControl1.DataSource = null;
                }

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Focus vào row vừa lưu trong grid
        /// </summary>
        private void FocusToSavedRow(long examServiceId)
        {
            try
            {
                if (examServiceId <= 0) return;

                // Tìm row trong grid
                for (int i = 0; i < gridView1.DataRowCount; i++)
                {
                    var data = gridView1.GetRow(i) as V_MCH_EXAM_SERVICE;
                    if (data != null && data.ID == examServiceId)
                    {
                        gridView1.FocusedRowHandle = i;
                        gridView1.MakeRowVisible(i);
                        Inventec.Common.Logging.LogSystem.Debug("FocusToSavedRow: Focused to row with ID = " + examServiceId);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
