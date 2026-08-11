using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using HIS.Desktop.MIMS.Integration.Core;
using HIS.Desktop.MIMS.Integration.Models;
using HIS.Desktop.MIMS.Integration.View;
using MOS.EFMODEL.DataModels;
using Inventec.Common.Adapter;
using Inventec.Core;
using HIS.Desktop.ApiConsumer;

namespace HIS.Desktop.MIMS.Integration.Modules
{
    public class DrugHealthService : BaseService
    {
        public DrugHealthService()
        {
            NameText = "Kiểm tra tương tác thuốc, bệnh liên quan";
        }
        string xmlRequest;

        /// <summary>
        /// PatientProfile của lần Check gần nhất — phục vụ ghi log audit (IS_PREGNANT/IS_LACTATING...).
        /// </summary>
        MimsPatientProfile lastPatientProfile;

        #region Overload tương thích ngược (binary compat với plugin build TRƯỚC khi có PatientProfile)
        // Các plugin cũ compile với chữ ký KHÔNG có tham số patientProfile — nếu bỏ các overload này,
        // môi trường dán lệch bộ DLL (plugin cũ + thư viện mới) sẽ nổ MissingMethodException khi lưu đơn.
        public MimsResult Check(List<DrugItem> drugs, List<string> icd10Codes)
        {
            return this.Check(drugs, null, icd10Codes, null);
        }

        public MimsResult Check(List<DrugItem> drugs, List<AllergyItem> allergies, List<string> icd10Codes)
        {
            return this.Check(drugs, allergies, icd10Codes, null);
        }

        public bool CheckAndAlert(List<DrugItem> drugs, List<string> icd10Codes, HIS_MIMS_INTERACTION_LOG interactionLog, long? treatmentId, long? serviceReqId, long? patientId)
        {
            return this.CheckAndAlert(drugs, null, icd10Codes, interactionLog, treatmentId, serviceReqId, patientId, null);
        }

        public bool CheckAndAlert(List<DrugItem> drugs, List<AllergyItem> allergies, List<string> icd10Codes, HIS_MIMS_INTERACTION_LOG interactionLog, long? treatmentId, long? serviceReqId, long? patientId)
        {
            return this.CheckAndAlert(drugs, allergies, icd10Codes, interactionLog, treatmentId, serviceReqId, patientId, null);
        }

        public void ShowResultAsync(List<DrugItem> drugs, List<string> icd10Codes)
        {
            this.ShowResultAsync(drugs, icd10Codes, null);
        }

        public bool ShowDialog(List<DrugItem> drugs, List<string> icd10Codes)
        {
            return this.ShowDialog(drugs, icd10Codes, null);
        }
        #endregion

        /// <summary>
        /// Kiểm tra Drug-Health Alert (Prescribing + HealthIssueCodes ICD10).
        /// patientProfile != null (BN nữ có tick mang thai/cho con bú) → gửi kèm khối PatientProfile
        /// để MIMS trả thêm cảnh báo Drug Pregnancy / Drug Lactation trong CÙNG request.
        /// </summary>
        public MimsResult Check(List<DrugItem> drugs, List<string> icd10Codes, MimsPatientProfile patientProfile)
        {
            return this.Check(drugs, null, icd10Codes, patientProfile);
        }

        /// <summary>
        /// Kiểm tra Drug-Health Alert (Prescribing + HealthIssueCodes ICD10).
        /// </summary>
        public MimsResult Check(List<DrugItem> drugs,List<AllergyItem> allergies, List<string> icd10Codes, MimsPatientProfile patientProfile = null)
        {
            Inventec.Common.Logging.LogSystem.Debug(
                "DrugHealthService.Check - start"
                + Inventec.Common.Logging.LogUtil.TraceData(
                    Inventec.Common.Logging.LogUtil.GetMemberName(() => drugs), drugs)
                + Inventec.Common.Logging.LogUtil.TraceData(
                    Inventec.Common.Logging.LogUtil.GetMemberName(() => allergies), allergies)
                + Inventec.Common.Logging.LogUtil.TraceData(
                    Inventec.Common.Logging.LogUtil.GetMemberName(() => icd10Codes), icd10Codes));

            drugs = this.MappingMIMS(drugs);
            var result = new MimsResult();
            if (drugs == null || drugs.Count == 0 || !drugs.Exists(o=>o.MimsGuid!=null))
            {
                Inventec.Common.Logging.LogSystem.Debug(
                    "DrugHealthService.Check - ABORT: không có thuốc mapped MimsGuid sau MappingMIMS");
                result.Success = false;
                result.Message = "Không có thông tin thuốc kiểm tra tương tác";
                result.Html = BuildSimpleHtml(result.Message);
                return result;
            }
            this.lastPatientProfile = patientProfile;
            xmlRequest = MimsRequestBuilder.BuildDrugHealthAlertRequest(drugs, allergies, icd10Codes, true, true, patientProfile);
            Inventec.Common.Logging.LogSystem.Debug(string.Format(
                "DrugHealthService.Check - requestLength={0}", xmlRequest == null ? 0 : xmlRequest.Length));

            bool isTimeout;
            string xmlResponse = MimsClient.PostXml(MimsConfig.CdsApiUrl, xmlRequest, out isTimeout);
            Inventec.Common.Logging.LogSystem.Debug(string.Format(
                "DrugHealthService.Check - isTimeout={0}, responseLength={1}",
                isTimeout, xmlResponse == null ? 0 : xmlResponse.Length));

            result.RawXml = xmlResponse;
            result.IsTimeout = isTimeout;

            if (isTimeout)
            {
                result.Success = false;
                result.Message = "Kiểm tra kết nối MIMS";
                result.Html = BuildSimpleHtml(result.Message);
                return result;
            }

            if (string.IsNullOrEmpty(xmlResponse))
            {
                result.Success = false;
                result.Message = "No response from MIMS API";
                result.Html = BuildSimpleHtml(result.Message);
                return result;
            }

            var trimmed = xmlResponse.TrimStart();
            if (trimmed.StartsWith("<Error", System.StringComparison.OrdinalIgnoreCase))
            {
                result.IsErrorResponse = true;
                try
                {
                    var doc = XDocument.Parse(xmlResponse);
                    result.ErrorMessage = (string)doc.Root.Element("Message");
                }
                catch
                {
                    result.ErrorMessage = xmlResponse;
                }

                result.Success = false;
                result.Message = result.ErrorMessage;
                result.Html = BuildSimpleHtml(result.ErrorMessage ?? "MIMS trả về lỗi.");
                return result;
            }

            result.Html = MimsResponseTransformer.XmlToHtml(xmlResponse);
            result.Success = !string.IsNullOrEmpty(result.Html);

            // Parse chi tiết CDS Drug-Health Alert
            result.DrugHealthAlertDetails = MimsResultDetailParser.ParseDrugHealthAlerts(xmlResponse);
            // Parse chi tiết CDS Drug–Drug Alert
            result.DrugDrugAlertDetails = MimsResultDetailParser.ParseDrugDrugAlerts(xmlResponse);
            // Parse chi tiết Drug-Pregnancy / Drug-Lactation Alert (chỉ có node khi request kèm PatientProfile)
            result.PregnancyAlertDetails = MimsResultDetailParser.ParsePregnancyAlerts(xmlResponse);
            result.LactationAlertDetails = MimsResultDetailParser.ParseLactationAlerts(xmlResponse);

            Inventec.Common.Logging.LogSystem.Debug(string.Format(
                "DrugHealthService.Check - Success={0}, DrugHealthAlertDetails.Count={1}, DrugDrugAlertDetails.Count={2}, PregnancyAlertDetails.Count={3}, LactationAlertDetails.Count={4}",
                result.Success,
                result.DrugHealthAlertDetails == null ? 0 : result.DrugHealthAlertDetails.Count,
                result.DrugDrugAlertDetails == null ? 0 : result.DrugDrugAlertDetails.Count,
                result.PregnancyAlertDetails == null ? 0 : result.PregnancyAlertDetails.Count,
                result.LactationAlertDetails == null ? 0 : result.LactationAlertDetails.Count));

            return result;
        }

        /// <summary>
        /// Kiểm tra Tương tác thuốc, bệnh lý. Hiển thị cảnh báo (nếu có) và ghi log.
        /// </summary>
        public bool CheckAndAlert(List<DrugItem> drugs, List<string> icd10Codes, HIS_MIMS_INTERACTION_LOG interactionLog = null, long? treatmentId = null, long? serviceReqId = null, long? patientId = null, MimsPatientProfile patientProfile = null)
        {
            return this.CheckAndAlert(drugs, null, icd10Codes, interactionLog, treatmentId , serviceReqId, patientId, patientProfile);
        }

        /// <summary>
        /// Kiểm tra Tương tác thuốc, bệnh lý. Hiển thị cảnh báo (nếu có) và ghi log.
        /// patientProfile != null → request kèm PatientProfile, MIMS trả thêm cảnh báo thai kỳ / cho con bú.
        /// </summary>
        public bool CheckAndAlert(List<DrugItem> drugs, List<AllergyItem> allergies, List<string> icd10Codes, HIS_MIMS_INTERACTION_LOG interactionLog = null, long? treatmentId = null, long? serviceReqId = null, long? patientId = null, MimsPatientProfile patientProfile = null)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug(
                    "DrugHealthService.CheckAndAlert - start"
                    + Inventec.Common.Logging.LogUtil.TraceData(
                        Inventec.Common.Logging.LogUtil.GetMemberName(() => drugs), drugs)
                    + Inventec.Common.Logging.LogUtil.TraceData(
                        Inventec.Common.Logging.LogUtil.GetMemberName(() => icd10Codes), icd10Codes));

                // KHÔNG overwrite drugs = MappingMIMS(drugs): việc đó thay danh sách gốc bằng danh sách đã map
                // (RỖNG nếu thuốc không map được MimsGuid) -> mất HisDrugCode -> VN Contraindication
                // (ExtractAtcCodes dò theo HisDrugCode) không còn dữ liệu để kiểm tra.
                // Check tự map nội bộ (giống luồng chuột phải CheckWithVnFallback); VN fallback dùng drugs GỐC.
                MimsResult result = Check(drugs, allergies, icd10Codes, patientProfile);

                if (result.DrugHealthAlertDetails == null) result.DrugHealthAlertDetails = new List<DrugHealthAlertDetail>();
                if (result.DrugDrugAlertDetails == null) result.DrugDrugAlertDetails = new List<DrugDrugAlertDetail>();
                if (result.PregnancyAlertDetails == null) result.PregnancyAlertDetails = new List<DrugPregnancyAlertDetail>();
                if (result.LactationAlertDetails == null) result.LactationAlertDetails = new List<DrugLactationAlertDetail>();

                // Thai kỳ: popup từ mức C/D/X/+ (theo bộ lọc khuyến nghị MIMS DP:C/D/X — Category A/B không popup riêng,
                // nhưng nếu popup mở vì cảnh báo khác thì tab "Thai kỳ" vẫn hiển thị đủ A/B).
                // Cho con bú: popup mọi mức (Caution / Avoid if possible / Contraindicated).
                bool hasCdsAlert = (result.DrugHealthAlertDetails.Count > 0
                        && result.DrugHealthAlertDetails.Exists(o => o.SeverityLevel != DrugHealthSeverity.Unknown))
                    || (result.DrugDrugAlertDetails.Count > 0
                        && result.DrugDrugAlertDetails.Exists(o => o.SeverityLevel != DrugInteractionSeverity.Unknown))
                    || (result.PregnancyAlertDetails.Count > 0
                        && result.PregnancyAlertDetails.Exists(o => o.CategoryLevel >= PregnancyCategory.Plus))
                    || (result.LactationAlertDetails.Count > 0
                        && result.LactationAlertDetails.Exists(o => o.SeverityLevel != LactationSeverity.Unknown));

                Inventec.Common.Logging.LogSystem.Debug(string.Format(
                    "DrugHealthService.CheckAndAlert - hasCdsAlert={0}", hasCdsAlert));

                if (hasCdsAlert)
                {
                    Inventec.Common.Logging.LogSystem.Debug(
                        "DrugHealthService.CheckAndAlert - showing CDS dialog (htmlLength="
                        + (result.Html == null ? 0 : result.Html.Length) + ")");
                    bool rs = WebViewHelper.ShowDialog(result.Html, NameText);
                    if (rs && interactionLog != null) SaveDataInteractionLog(this.MappingMIMS(drugs), result, interactionLog, treatmentId, serviceReqId, patientId);
                    return rs;
                }

                // Không có alert CDS phân loại được -> ưu tiên kiểm tra VN Contraindication
                Inventec.Common.Logging.LogSystem.Debug(
                    "DrugHealthService.CheckAndAlert - no classified CDS alert -> check VN Contraindication");
                MimsResult vnResult = CheckVnContraindication(drugs);
                bool hasVnAlert = vnResult != null
                    && vnResult.VnContraindicationDetails != null
                    && vnResult.VnContraindicationDetails.Count > 0
                    && !string.IsNullOrEmpty(vnResult.Html);
                if (hasVnAlert)
                {
                    bool rsVn = WebViewHelper.ShowDialog(vnResult.Html, "Kiểm tra tương tác thuốc (VN)");
                    if (rsVn && interactionLog != null) SaveVnInteractionLog(drugs, vnResult, interactionLog, treatmentId, serviceReqId, patientId);
                    return rsVn;
                }

                // Không có cảnh báo tương tác để hiển thị -> bỏ qua, KHÔNG popup.
                // Bao gồm: thuốc không map được MimsGuid ("Không có thông tin thuốc kiểm tra tương tác"),
                // MIMS phản hồi bình thường nhưng không có alert, response rỗng...
                // CHỈ hiển thị khi MIMS thực sự lỗi kết nối/dịch vụ (timeout hoặc trả về <Error>)
                // để báo người dùng biết bước kiểm tra không chạy được.
                if ((result.IsTimeout || result.IsErrorResponse) && !string.IsNullOrEmpty(result.Html))
                {
                    Inventec.Common.Logging.LogSystem.Debug(string.Format(
                        "DrugHealthService.CheckAndAlert - MIMS lỗi kết nối/dịch vụ -> hiển thị thông báo (isTimeout={0}, isError={1})",
                        result.IsTimeout, result.IsErrorResponse));
                    return WebViewHelper.ShowDialog(result.Html, NameText);
                }

                Inventec.Common.Logging.LogSystem.Debug(
                    "DrugHealthService.CheckAndAlert - không có cảnh báo tương tác -> bỏ qua (không popup)");
                return true;
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return true;
        }

        private void SaveDataInteractionLog(List<DrugItem> drugs, MimsResult result, HIS_MIMS_INTERACTION_LOG interactionLog, long? treatmentId, long? serviceReqId, long? patientId)
        {
            try
            {
                interactionLog.TREATMENT_ID = treatmentId;
                interactionLog.SERVICE_REQ_ID = serviceReqId;
                interactionLog.PATIENT_ID = patientId;
                interactionLog.MODULE_TYPE = 4; //4=Drug-Health
                interactionLog.REQUEST_TYPE = "INTERACTION";
                interactionLog.REQUEST_ENDPOINT = MimsConfig.CdsApiUrl;
                interactionLog.REQUEST_XML = xmlRequest;
                interactionLog.CHECKED_GUIDS = string.Join(";", drugs.Where(d => !string.IsNullOrWhiteSpace(d.MimsGuid)).Select(d => d.MimsGuid));
                interactionLog.DRUG_COUNT = (short)(drugs.Where(d => !string.IsNullOrWhiteSpace(d.MimsGuid)).Count());
                interactionLog.UNMAPPED_DRUG_COUNT = (short)(drugs.Where(d => string.IsNullOrWhiteSpace(d.MimsGuid)).Count());
                interactionLog.RESPONSE_XML = result.RawXml;
                interactionLog.RESPONSE_HTML = "";
                interactionLog.RESPONSE_TYPE = "xml";
                interactionLog.HAS_ALERT = 1;
                interactionLog.ALERT_COUNT = 1;
                // Thông tin PatientProfile + số cảnh báo thai kỳ / cho con bú (cột có sẵn trong bảng log)
                interactionLog.PREGNANCY_COUNT = (short)result.PregnancyAlertDetails.Count;
                interactionLog.LACTATION_COUNT = (short)result.LactationAlertDetails.Count;
                if (this.lastPatientProfile != null)
                {
                    interactionLog.IS_PREGNANT = this.lastPatientProfile.IsPregnant ? (short?)1 : (short?)0;
                    interactionLog.IS_LACTATING = this.lastPatientProfile.IsNursing ? (short?)1 : (short?)0;
                    interactionLog.PATIENT_AGE = this.lastPatientProfile.AgeYear.HasValue ? (short?)this.lastPatientProfile.AgeYear.Value : null;
                    interactionLog.PATIENT_GENDER = this.lastPatientProfile.GenderCode;
                }
                // Mức nghiêm trọng theo XPath guide MIMS: Pregnancy D/X/+ và Lactation Contraindicated/Avoid if possible
                bool pregnancySevere = result.PregnancyAlertDetails.Exists(o =>
                    o.CategoryLevel == PregnancyCategory.D
                    || o.CategoryLevel == PregnancyCategory.X
                    || o.CategoryLevel == PregnancyCategory.Plus);
                bool lactationSevere = result.LactationAlertDetails.Exists(o =>
                    o.SeverityLevel == LactationSeverity.Contraindicated
                    || o.SeverityLevel == LactationSeverity.AvoidIfPossible);
                interactionLog.HAS_SEVERE_ALERT = (result.DrugDrugAlertDetails.Exists(o => o.SeverityLevel == DrugInteractionSeverity.Severe)
                                        || result.DrugHealthAlertDetails.Exists(o => o.SeverityLevel == DrugHealthSeverity.Contraindicated)
                                        || pregnancySevere || lactationSevere)
                                        ? (short?)1 : null;
                string highestSeverity = null;
                if (result.DrugDrugAlertDetails.Exists(o => o.SeverityLevel == DrugInteractionSeverity.Severe))
                {
                    highestSeverity = "SEVERE";
                }
                else if (result.DrugHealthAlertDetails.Exists(o => o.SeverityLevel == DrugHealthSeverity.Contraindicated)
                    || result.PregnancyAlertDetails.Exists(o => o.CategoryLevel == PregnancyCategory.X)
                    || result.LactationAlertDetails.Exists(o => o.SeverityLevel == LactationSeverity.Contraindicated))
                {
                    highestSeverity = "CONTRAINDICATED";
                }
                else if (result.DrugDrugAlertDetails.Exists(o => o.SeverityLevel == DrugInteractionSeverity.Moderate))
                {
                    highestSeverity = "MODERATE";
                }
                else if (result.DrugHealthAlertDetails.Exists(o => o.SeverityLevel == DrugHealthSeverity.ExtremeCaution)
                    || result.PregnancyAlertDetails.Exists(o => o.CategoryLevel == PregnancyCategory.D || o.CategoryLevel == PregnancyCategory.Plus)
                    || result.LactationAlertDetails.Exists(o => o.SeverityLevel == LactationSeverity.AvoidIfPossible))
                {
                    highestSeverity = "EXTREMECAUTION";
                }
                else if (result.DrugDrugAlertDetails.Exists(o => o.SeverityLevel == DrugInteractionSeverity.Minor))
                {
                    highestSeverity = "MINOR";
                }
                else if (result.DrugDrugAlertDetails.Exists(o => o.SeverityLevel == DrugInteractionSeverity.Caution)
                    || result.PregnancyAlertDetails.Exists(o => o.CategoryLevel == PregnancyCategory.C)
                    || result.LactationAlertDetails.Exists(o => o.SeverityLevel == LactationSeverity.Caution))
                {
                    highestSeverity = "CAUTION";
                }
                interactionLog.HIGHEST_SEVERITY = highestSeverity;
                interactionLog.IS_SUCCESS = result.Success ? (short?)1 : (short?)0;
                interactionLog.ERROR_MESSAGE = result.Message;
                interactionLog.USER_ACKNOWLEDGED = 1;
                interactionLog.USER_OVERRIDE = 1;
                interactionLog.OVERRIDE_BY = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName();
                interactionLog.OVERRIDE_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(System.DateTime.Now);
                CommonParam param = new CommonParam();
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("MimsInteractionLog", interactionLog));
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public bool ShowDialog(List<DrugItem> drugs, List<string> icd10Codes, MimsPatientProfile patientProfile = null)
        {
            try
            {
                MimsResult result = Check(drugs, icd10Codes, patientProfile);

                bool hasCdsAlert = result != null && result.Success
                    && ((result.DrugHealthAlertDetails != null
                            && result.DrugHealthAlertDetails.Exists(o => o.SeverityLevel != DrugHealthSeverity.Unknown))
                        || (result.DrugDrugAlertDetails != null
                            && result.DrugDrugAlertDetails.Exists(o => o.SeverityLevel != DrugInteractionSeverity.Unknown))
                        || (result.PregnancyAlertDetails != null
                            && result.PregnancyAlertDetails.Exists(o => o.CategoryLevel >= PregnancyCategory.Plus))
                        || (result.LactationAlertDetails != null
                            && result.LactationAlertDetails.Exists(o => o.SeverityLevel != LactationSeverity.Unknown)));

                if (hasCdsAlert && !string.IsNullOrEmpty(result.Html))
                {
                    return WebViewHelper.ShowDialog(result.Html, NameText);
                }

                // Fallback VN Contraindication — không có log (caller không truyền log object)
                return CheckAndShowVnContraindication(drugs);
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return true;
        }

        /// <summary>
        /// Async show kết quả tương tác — dùng cho menu chuột phải "Đánh giá thông tin thuốc".
        /// Logic: CDS trước; nếu CDS không có alert thì check VN Contraindication.
        /// Trả về HTML phù hợp (CDS hoặc VN) để WebViewHelper hiển thị async.
        /// </summary>
        public void ShowResultAsync(List<DrugItem> drugs, List<string> icd10Codes, MimsPatientProfile patientProfile = null)
        {
            WebViewHelper.ShowResultAsync(() => CheckWithVnFallback(drugs, icd10Codes, patientProfile), NameText);
        }

        /// <summary>
        /// Helper cho ShowResultAsync: CDS check trước, fallback VN nếu CDS không có alert thực sự.
        /// Không show dialog, chỉ trả về MimsResult cho WebViewHelper hiển thị.
        /// </summary>
        private MimsResult CheckWithVnFallback(List<DrugItem> drugs, List<string> icd10Codes, MimsPatientProfile patientProfile = null)
        {
            MimsResult cdsResult = Check(drugs, null, icd10Codes, patientProfile);

            bool hasCdsAlert = cdsResult != null && cdsResult.Success
                && ((cdsResult.DrugHealthAlertDetails != null
                        && cdsResult.DrugHealthAlertDetails.Exists(o => o.SeverityLevel != DrugHealthSeverity.Unknown))
                    || (cdsResult.DrugDrugAlertDetails != null
                        && cdsResult.DrugDrugAlertDetails.Exists(o => o.SeverityLevel != DrugInteractionSeverity.Unknown))
                    || (cdsResult.PregnancyAlertDetails != null
                        && cdsResult.PregnancyAlertDetails.Exists(o => o.CategoryLevel >= PregnancyCategory.Plus))
                    || (cdsResult.LactationAlertDetails != null
                        && cdsResult.LactationAlertDetails.Exists(o => o.SeverityLevel != LactationSeverity.Unknown)));

            Inventec.Common.Logging.LogSystem.Debug(string.Format(
                "DrugHealthService.CheckWithVnFallback - hasCdsAlert={0}", hasCdsAlert));

            if (hasCdsAlert) return cdsResult;

            MimsResult vnResult = CheckVnContraindication(drugs);
            bool hasVnAlert = vnResult != null
                && vnResult.VnContraindicationDetails != null
                && vnResult.VnContraindicationDetails.Count > 0
                && !string.IsNullOrEmpty(vnResult.Html);

            Inventec.Common.Logging.LogSystem.Debug(string.Format(
                "DrugHealthService.CheckWithVnFallback - hasVnAlert={0}", hasVnAlert));

            return hasVnAlert ? vnResult : cdsResult;
        }
    }
}
