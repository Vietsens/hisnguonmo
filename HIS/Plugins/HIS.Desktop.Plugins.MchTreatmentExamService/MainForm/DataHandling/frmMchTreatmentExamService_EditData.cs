using DevExpress.XtraEditors;
using HIS.Desktop.ApiConsumer;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MCH.EFMODEL.DataModels;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.MchTreatmentExamService.MainForm
{
    public partial class UCMchTreatmentExamService : HIS.Desktop.Utility.FormBase
    {
        #region Base Load Data Methods

        /// <summary>
        /// Base method để load data từ API
        /// </summary>
        private TResult LoadMchData<TResult, TFilter>(string apiPath, long examServiceId)
            where TResult : class, new()
            where TFilter : class, new()
        {
            try
            {
                CommonParam param = new CommonParam();

                // Tạo filter và set EXAM_SERVICE_ID
                var filter = Activator.CreateInstance<TFilter>();
                var examServiceIdProp = typeof(TFilter).GetProperty("EXAM_SERVICE_ID");
                if (examServiceIdProp != null)
                {
                    examServiceIdProp.SetValue(filter, examServiceId);
                }

                var results = new BackendAdapter(param).Get<List<TResult>>(
                    apiPath,
                    ApiConsumers.MchConsumer,
                    filter,
                    param);

                return (results != null && results.Count > 0) ? results[0] : new TResult();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return new TResult();
            }
        }

        /// <summary>
        /// Copy dữ liệu giữa 2 objects cùng kiểu, bỏ qua ID
        /// </summary>
        private void CopyDataWithoutId<T>(T source, T target) where T : class
        {
            try
            {
                if (source == null || target == null) return;

                var properties = typeof(T).GetProperties();
                foreach (var prop in properties)
                {
                    // Bỏ qua các trường ID và EXAM_SERVICE_ID
                    if (prop.Name == "ID" || prop.Name == "EXAM_SERVICE_ID" ||
                        prop.Name == "CREATE_TIME" || prop.Name == "MODIFY_TIME" ||
                        prop.Name == "CREATOR" || prop.Name == "MODIFIER" ||
                        prop.Name == "APP_CREATOR" || prop.Name == "APP_MODIFIER" ||
                        prop.Name == "IS_ACTIVE" || prop.Name == "IS_DELETE" ||
                        prop.Name == "GROUP_CODE")
                        continue;

                    if (prop.CanWrite && prop.CanRead)
                    {
                        var value = prop.GetValue(source);
                        prop.SetValue(target, value);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion

        #region Load Detail Data Methods

        private bool LoadScreeningData(long examServiceId)
        {
            try
            {
                _screening = LoadMchData<MCH_SCREENING, MCH.Filter.MchScreeningFilter>("api/MchScreening/Get", examServiceId);
                LoadChildData(examServiceId);
                return true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }
        }

        private bool LoadAntenatalVisitData(long examServiceId)
        {
            try
            {
                _antenatalVisit = LoadMchData<MCH_ANTENATAL_VISIT, MCH.Filter.MchAntenatalVisitFilter>("api/MchAntenatalVisit/Get", examServiceId);
                return true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }
        }

        private bool LoadBirthInfoData(long examServiceId)
        {
            try
            {
                _birthInfo = LoadMchData<MCH_BIRTH_INFO, MCH.Filter.MchBirthInfoFilter>("api/MchBirthInfo/Get", examServiceId);
                LoadChildData(examServiceId);
                return true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }
        }

        private bool LoadContraceptionData(long examServiceId)
        {
            try
            {
                _contraception = LoadMchData<MCH_CONTRACEPTION, MCH.Filter.MchContraceptionFilter>("api/MchContraception/Get", examServiceId);
                return true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }
        }

        private bool LoadAbortionData(long examServiceId)
        {
            try
            {
                _abortion = LoadMchData<MCH_ABORTION, MCH.Filter.MchAbortionFilter>("api/MchAbortion/Get", examServiceId);
                return true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }
        }

        private bool LoadChildData(long examServiceId)
        {
            try
            {
                _child = LoadMchData<MCH_CHILD, MCH.Filter.MchChildFilter>(
                    "api/MchChild/Get", examServiceId);
                return true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }
        }

        #endregion

        #region Load Data For Copy (No ID)

        private bool LoadScreeningDataForCopy(long examServiceId)
        {
            try
            {
                var source = LoadMchData<MCH_SCREENING, MCH.Filter.MchScreeningFilter>("api/MchScreening/Get", examServiceId);
                _screening = new MCH_SCREENING();
                CopyDataWithoutId(source, _screening);
                LoadChildDataForCopy(examServiceId);
                return true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }
        }

        private bool LoadAntenatalVisitDataForCopy(long examServiceId)
        {
            try
            {
                var source = LoadMchData<MCH_ANTENATAL_VISIT, MCH.Filter.MchAntenatalVisitFilter>("api/MchAntenatalVisit/Get", examServiceId);
                _antenatalVisit = new MCH_ANTENATAL_VISIT();
                CopyDataWithoutId(source, _antenatalVisit);
                return true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }
        }

        private bool LoadBirthInfoDataForCopy(long examServiceId)
        {
            try
            {
                var source = LoadMchData<MCH_BIRTH_INFO, MCH.Filter.MchBirthInfoFilter>("api/MchBirthInfo/Get", examServiceId);
                _birthInfo = new MCH_BIRTH_INFO();
                CopyDataWithoutId(source, _birthInfo);
                LoadChildDataForCopy(examServiceId);
                return true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }
        }

        private bool LoadContraceptionDataForCopy(long examServiceId)
        {
            try
            {
                var source = LoadMchData<MCH_CONTRACEPTION, MCH.Filter.MchContraceptionFilter>("api/MchContraception/Get", examServiceId);
                _contraception = new MCH_CONTRACEPTION();
                CopyDataWithoutId(source, _contraception);
                return true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }
        }

        private bool LoadAbortionDataForCopy(long examServiceId)
        {
            try
            {
                var source = LoadMchData<MCH_ABORTION, MCH.Filter.MchAbortionFilter>(
                    "api/MchAbortion/Get", examServiceId);
                _abortion = new MCH_ABORTION();
                CopyDataWithoutId(source, _abortion);
                return true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }
        }

        private bool LoadChildDataForCopy(long examServiceId)
        {
            try
            {
                var source = LoadMchData<MCH_CHILD, MCH.Filter.MchChildFilter>(
                    "api/MchChild/Get", examServiceId);
                _child = new MCH_CHILD();
                CopyDataWithoutId(source, _child);
                return true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }
        }

        #endregion

        #region Process Edit and Copy

        /// <summary>
        /// Load đầy đủ dữ liệu theo EXAM_SERVICE_TYPE_ID
        /// </summary>
        private bool LoadExamServiceDetailData(V_MCH_EXAM_SERVICE examService)
        {
            bool result = false;
            try
            {
                _examService = new MCH_EXAM_SERVICE
                {
                    ID = examService.ID,
                    TREATMENT_ID = examService.TREATMENT_ID,
                    EXECUTE_LOGINNAME = examService.EXECUTE_LOGINNAME,
                    EXECUTE_USERNAME = examService.EXECUTE_USERNAME,
                    EXECUTE_TYPE = examService.EXECUTE_TYPE,
                    EXAM_SERVICE_TYPE_ID = examService.EXAM_SERVICE_TYPE_ID
                };

                // EXAM_SERVICE_TYPE_ID theo quy ước chuẩn:
                // 1=Khám thai, 2=Sinh đẻ, 3=Tránh thai, 4=Phá thai, 5=Sàng lọc
                switch (examService.EXAM_SERVICE_TYPE_ID)
                {
                    case 1: result = LoadAntenatalVisitData(examService.ID); break; // Khám thai
                    case 2: result = LoadBirthInfoData(examService.ID); break; // Sinh đẻ
                    case 3: result = LoadContraceptionData(examService.ID); break; // Tránh thai
                    case 4: result = LoadAbortionData(examService.ID); break; // Phá thai
                    case 5: result = LoadScreeningData(examService.ID); break; // Sàng lọc
                    default:
                        Inventec.Common.Logging.LogSystem.Warn("LoadExamServiceDetailData - Invalid EXAM_SERVICE_TYPE_ID: " + examService.EXAM_SERVICE_TYPE_ID);
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// Load dữ liệu để copy (không giữ ID)
        /// </summary>
        private bool LoadExamServiceDetailDataForCopy(V_MCH_EXAM_SERVICE examService)
        {
            bool result = false;
            try
            {
                _examService = new MCH_EXAM_SERVICE
                {
                    TREATMENT_ID = Treatment != null && Treatment.ID > 0 ? Treatment.ID : ExamService.TREATMENT_ID,
                    EXECUTE_LOGINNAME = examService.EXECUTE_LOGINNAME,
                    EXECUTE_USERNAME = examService.EXECUTE_USERNAME,
                    EXECUTE_TYPE = examService.EXECUTE_TYPE,
                    EXAM_SERVICE_TYPE_ID = examService.EXAM_SERVICE_TYPE_ID
                };

                switch (examService.EXAM_SERVICE_TYPE_ID)
                {
                    case 5: result = LoadScreeningDataForCopy(examService.ID); break;
                    case 1: result = LoadAntenatalVisitDataForCopy(examService.ID); break;
                    case 2: result = LoadBirthInfoDataForCopy(examService.ID); break;
                    case 3: result = LoadContraceptionDataForCopy(examService.ID); break;
                    case 4: result = LoadAbortionDataForCopy(examService.ID); break;
                    default:
                        Inventec.Common.Logging.LogSystem.Warn("EXAM_SERVICE_TYPE_ID không hợp lệ: " + examService.EXAM_SERVICE_TYPE_ID);
                        result = true;
                        break;
                }

                // Set mặc định ngày khám và người khám cho nút Copy
                SetDefaultExamDateAndUser(examService);

                return result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }
        }

        /// <summary>
        /// Set mặc định Ngày khám và Người khám
        /// </summary>
        private void SetDefaultExamDateAndUser(V_MCH_EXAM_SERVICE examService)
        {
            try
            {
                // Set mặc định Ngày khám
                long examDate = 0;
                if (examService != null && examService.EXECUTE_TIME > 0)
                {
                    // Lấy từ ExamService
                    examDate = examService.EXECUTE_TIME ?? 0;
                }
                else if (ExamService != null && ExamService.ID > 0 && ExamService.EXECUTE_TIME > 0)
                {
                    // Lấy từ ExamService toàn cục
                    examDate = ExamService.EXECUTE_TIME ?? 0;
                }
                else if (Treatment != null && Treatment.ID > 0)
                {
                    // Ưu tiên lấy từ Treatment
                    examDate = Treatment.IN_TIME;
                }


                // Set ngày khám vào các DateEdit tương ứng theo tab
                if (examDate > 0)
                {
                    DateTime examDateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(examDate) ?? DateTime.Now;

                    // Set cho tất cả các DateEdit ngày khám trong các tab
                    if (dteExam1 != null) dteExam1.EditValue = examDateTime;
                    if (dteExam2 != null) dteExam2.EditValue = examDateTime;
                    if (dteExam3 != null) dteExam3.EditValue = examDateTime;
                    if (dteExam4 != null) dteExam4.EditValue = examDateTime;
                    if (dteExam5 != null) dteExam5.EditValue = examDateTime;
                }

                // Set mặc định Người khám
                string executeLoginName = null;

                // Ưu tiên lấy từ ExamService
                if (examService != null && !string.IsNullOrEmpty(examService.EXECUTE_LOGINNAME))
                {
                    executeLoginName = examService.EXECUTE_LOGINNAME;
                }
                else if (ExamService != null && !string.IsNullOrEmpty(ExamService.EXECUTE_LOGINNAME))
                {
                    executeLoginName = ExamService.EXECUTE_LOGINNAME;
                }
                else
                {
                    // Mặc định là user đang đăng nhập
                    executeLoginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                }

                // Set người khám vào các GridLookUpEdit tương ứng theo tab
                if (!string.IsNullOrEmpty(executeLoginName))
                {
                    if (cboUser1 != null) cboUser1.EditValue = executeLoginName;
                    if (cboUser2 != null) cboUser2.EditValue = executeLoginName;
                    if (cboUser3 != null) cboUser3.EditValue = executeLoginName;
                    if (cboUser4 != null) cboUser4.EditValue = executeLoginName;
                    if (cboUser5 != null) cboUser5.EditValue = executeLoginName;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Chuyển sang tab tương ứng và fill dữ liệu
        /// </summary>
        private void SwitchToTabAndFillData(long examServiceTypeId)
        {
            try
            {
                // Map EXAM_SERVICE_TYPE_ID sang tab index theo quy ước chuẩn
                // 1 (Khám thai) → Tab 0
                // 2 (Sinh đẻ) → Tab 1
                // 3 (Tránh thai) → Tab 2
                // 4 (Phá thai) → Tab 3
                // 5 (Sàng lọc) → Tab 4
                int tabIndex = GetTabIndexFromExamServiceTypeId(examServiceTypeId);

                if (xtraTabControl1 != null && xtraTabControl1.TabPages.Count > tabIndex && tabIndex >= 0)
                {
                    xtraTabControl1.SelectedTabPageIndex = tabIndex;
                }

                FillDataToCurrentTab(examServiceTypeId);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Fill dữ liệu vào tab hiện tại
        /// </summary>
        private void FillDataToCurrentTab(long examServiceTypeId)
        {
            try
            {
                switch (examServiceTypeId)
                {
                    case 1: // Sàng lọc
                        FillDataToTab2();
                        break;
                    case 2: // Sinh đẻ
                        FillDataToTab3Mother();
                        FillDataToTab3Child();
                        break;
                    case 3: // Tránh thai
                        FillDataToTab4();
                        break;
                    case 4: // Phá thai
                        FillDataToTab5();
                        break;
                    case 5:
                        // Khám thai
                        FillDataToTab1();
                        break;
                    default:
                        Inventec.Common.Logging.LogSystem.Warn("FillDataToCurrentTab - Invalid EXAM_SERVICE_TYPE_ID: " + examServiceTypeId);
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Reset tất cả data models
        /// </summary>
        private void ResetDataModels()
        {
            try
            {
                _examService = null;
                _screening = null;
                _child = null;
                _birthInfo = null;
                _antenatalVisit = null;
                _contraception = null;
                _abortion = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion
    }
}
