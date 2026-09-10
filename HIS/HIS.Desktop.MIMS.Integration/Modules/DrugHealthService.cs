using System;
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
            return this.Check(drugs, allergies, icd10Codes, patientProfile, null, null, null);
        }

        /// <summary>
        /// Kiểm tra Drug-Health Alert có tính cả thuốc các đơn KHÁC còn hiệu lực trong hồ sơ (việc 52540).
        /// previousDrugs      = thuốc các đơn khác (DrugItem theo MEDICINE_TYPE_CODE, chưa map MIMS).
        /// previousDrugInfos  = thông tin nguồn đơn để dựng khối HTML đầu popup.
        /// crossOption        = cấu hình cách gửi + lọc cảnh báo; null → hành vi như trước.
        /// previousDrugs null/rỗng → request và kết quả GIỐNG HOÀN TOÀN bản hiện tại.
        /// </summary>
        public MimsResult Check(List<DrugItem> drugs, List<AllergyItem> allergies, List<string> icd10Codes,
            MimsPatientProfile patientProfile, List<DrugItem> previousDrugs,
            List<MimsPreviousDrugInfo> previousDrugInfos, MimsCrossPrescriptionOption crossOption)
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

            // Thuốc đơn khác: map MIMS rồi loại GUID đã có ở đơn hiện tại (tránh trùng thẻ trong Prescribing)
            var previousMapped = this.MapPreviousDrugs(previousDrugs, drugs);
            result.PreviousDrugCount = previousMapped.Count;
            result.PreviousDrugGuids = string.Join(";", previousMapped.Select(o => o.MimsGuid));

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
            int requestMode = crossOption != null
                ? crossOption.RequestMode
                : MimsCrossPrescriptionOption.REQUEST_MODE__MERGE_PRESCRIBING;
            xmlRequest = MimsRequestBuilder.BuildDrugHealthAlertRequest(
                drugs, allergies, icd10Codes, true, true, patientProfile, previousMapped, requestMode);
            Inventec.Common.Logging.LogSystem.Debug(string.Format(
                "DrugHealthService.Check - requestLength={0}, previousDrugCount={1}, requestMode={2}",
                xmlRequest == null ? 0 : xmlRequest.Length, previousMapped.Count, requestMode));

            var extraFormParams = this.BuildExtraFormParams(drugs, previousMapped, crossOption, result);

            bool isTimeout;
            string xmlResponse = MimsClient.PostXml(MimsConfig.CdsApiUrl, xmlRequest, extraFormParams, out isTimeout);
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

            // Việc 52540 — QT-16: chỉ giữ cảnh báo liên quan thuốc ĐANG KÊ.
            // Khi đã gửi alertfilterbydrug thì MIMS lọc sẵn phía server, không cần lọc lại.
            if (previousMapped.Count > 0 && !result.IsAlertFilteredByDrug)
            {
                this.FilterAlertsByCurrentDrugs(result, drugs);
            }

            // Việc 52540 — QT-11/QT-17: chèn khối "Thuốc đang dùng từ đơn khác" lên ĐẦU HTML
            result.Html = this.PrependPreviousDrugBanner(result.Html, previousDrugInfos, crossOption);

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
            return this.CheckAndAlert(drugs, allergies, icd10Codes, interactionLog,
                treatmentId, serviceReqId, patientId, patientProfile, null, null, null);
        }

        /// <summary>
        /// Kiểm tra + hiển thị cảnh báo, có tính cả thuốc các đơn KHÁC còn hiệu lực trong hồ sơ (việc 52540).
        /// previousDrugs null/rỗng → hành vi GIỐNG HOÀN TOÀN overload hiện tại.
        /// </summary>
        public bool CheckAndAlert(List<DrugItem> drugs, List<AllergyItem> allergies, List<string> icd10Codes,
            HIS_MIMS_INTERACTION_LOG interactionLog, long? treatmentId, long? serviceReqId, long? patientId,
            MimsPatientProfile patientProfile, List<DrugItem> previousDrugs,
            List<MimsPreviousDrugInfo> previousDrugInfos, MimsCrossPrescriptionOption crossOption)
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
                MimsResult result = Check(drugs, allergies, icd10Codes, patientProfile,
                    previousDrugs, previousDrugInfos, crossOption);

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
                    if (rs && interactionLog != null)
                    {
                        SaveDataInteractionLog(this.MappingMIMS(drugs), result, interactionLog, treatmentId, serviceReqId, patientId);
                        ApplyCrossPrescriptionLog(interactionLog, result, crossOption);
                    }
                    return rs;
                }

                // Không có alert CDS phân loại được -> ưu tiên kiểm tra VN Contraindication.
                // Việc 52540: gộp cả thuốc đơn khác vào phép kiểm tra VN (cặp chống chỉ định theo QĐ
                // trong nước cũng là cảnh báo so CẶP nên cũng bị bỏ sót khi thuốc nằm ở hai đơn).
                Inventec.Common.Logging.LogSystem.Debug(
                    "DrugHealthService.CheckAndAlert - no classified CDS alert -> check VN Contraindication");
                List<DrugItem> drugsForVn = MergeDrugsForVnCheck(drugs, previousDrugs);
                MimsResult vnResult = CheckVnContraindication(drugsForVn);
                bool hasVnAlert = vnResult != null
                    && vnResult.VnContraindicationDetails != null
                    && vnResult.VnContraindicationDetails.Count > 0
                    && !string.IsNullOrEmpty(vnResult.Html);
                if (hasVnAlert)
                {
                    vnResult.PreviousDrugCount = result.PreviousDrugCount;
                    vnResult.PreviousDrugGuids = result.PreviousDrugGuids;
                    string vnHtml = this.PrependPreviousDrugBanner(vnResult.Html, previousDrugInfos, crossOption);
                    bool rsVn = WebViewHelper.ShowDialog(vnHtml, "Kiểm tra tương tác thuốc (VN)");
                    if (rsVn && interactionLog != null)
                    {
                        SaveVnInteractionLog(drugsForVn, vnResult, interactionLog, treatmentId, serviceReqId, patientId);
                        ApplyCrossPrescriptionLog(interactionLog, vnResult, crossOption);
                    }
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
        /// Async show kết quả có tính cả thuốc các đơn KHÁC trong hồ sơ (việc 52540) —
        /// dùng cho menu chuột phải "Đánh giá thông tin thuốc".
        /// </summary>
        public void ShowResultAsync(List<DrugItem> drugs, List<string> icd10Codes,
            MimsPatientProfile patientProfile, List<DrugItem> previousDrugs,
            List<MimsPreviousDrugInfo> previousDrugInfos, MimsCrossPrescriptionOption crossOption)
        {
            WebViewHelper.ShowResultAsync(
                () => CheckWithVnFallback(drugs, icd10Codes, patientProfile, previousDrugs, previousDrugInfos, crossOption),
                NameText);
        }

        /// <summary>
        /// Helper cho ShowResultAsync: CDS check trước, fallback VN nếu CDS không có alert thực sự.
        /// Không show dialog, chỉ trả về MimsResult cho WebViewHelper hiển thị.
        /// </summary>
        private MimsResult CheckWithVnFallback(List<DrugItem> drugs, List<string> icd10Codes, MimsPatientProfile patientProfile = null)
        {
            return CheckWithVnFallback(drugs, icd10Codes, patientProfile, null, null, null);
        }

        /// <summary>
        /// Helper cho ShowResultAsync có tính thuốc đơn khác (việc 52540).
        /// </summary>
        private MimsResult CheckWithVnFallback(List<DrugItem> drugs, List<string> icd10Codes,
            MimsPatientProfile patientProfile, List<DrugItem> previousDrugs,
            List<MimsPreviousDrugInfo> previousDrugInfos, MimsCrossPrescriptionOption crossOption)
        {
            MimsResult cdsResult = Check(drugs, null, icd10Codes, patientProfile,
                previousDrugs, previousDrugInfos, crossOption);

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

            MimsResult vnResult = CheckVnContraindication(MergeDrugsForVnCheck(drugs, previousDrugs));
            bool hasVnAlert = vnResult != null
                && vnResult.VnContraindicationDetails != null
                && vnResult.VnContraindicationDetails.Count > 0
                && !string.IsNullOrEmpty(vnResult.Html);

            Inventec.Common.Logging.LogSystem.Debug(string.Format(
                "DrugHealthService.CheckWithVnFallback - hasVnAlert={0}", hasVnAlert));

            if (hasVnAlert)
            {
                vnResult.Html = this.PrependPreviousDrugBanner(vnResult.Html, previousDrugInfos, crossOption);
                return vnResult;
            }

            return cdsResult;
        }

        #region Việc 52540 — kiểm tra tương tác chéo đơn trong hồ sơ

        /// <summary>Giới hạn độ dài cột CHECKED_GUIDS của HIS_MIMS_INTERACTION_LOG.</summary>
        private const int CHECKED_GUIDS_MAX_LENGTH = 2000;

        /// <summary>Cắt chuỗi về đúng độ dài tối đa của cột log.</summary>
        private static string Truncate(string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value) || value.Length <= maxLength)
                return value;
            return value.Substring(0, maxLength);
        }

        /// <summary>
        /// Map thuốc các đơn khác sang MimsGuid, loại các GUID đã có ở đơn hiện tại
        /// để không sinh thẻ trùng trong request.
        /// </summary>
        private List<DrugItem> MapPreviousDrugs(List<DrugItem> previousDrugs, List<DrugItem> currentMapped)
        {
            var result = new List<DrugItem>();
            try
            {
                if (previousDrugs == null || previousDrugs.Count == 0)
                    return result;

                var currentGuids = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                if (currentMapped != null)
                {
                    foreach (var drug in currentMapped)
                    {
                        if (drug != null && !string.IsNullOrEmpty(drug.MimsGuid))
                            currentGuids.Add(NormalizeGuid(drug.MimsGuid));
                    }
                }

                var previousMapped = this.MappingMIMS(previousDrugs);
                foreach (var drug in previousMapped)
                {
                    if (drug == null || string.IsNullOrEmpty(drug.MimsGuid))
                        continue;
                    if (currentGuids.Contains(NormalizeGuid(drug.MimsGuid)))
                        continue;

                    result.Add(drug);
                }

                Inventec.Common.Logging.LogSystem.Debug(string.Format(
                    "DrugHealthService.MapPreviousDrugs - input={0}, mapped={1}, sau khi loại trùng đơn hiện tại={2}",
                    previousDrugs.Count, previousMapped.Count, result.Count));
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// Dựng tham số form phụ gửi MIMS: "alertfilterbydrug" (GUID thuốc đang kê) và
        /// "alertfilterbyseverity". Trả về null khi không có tham số nào → post như cũ.
        /// </summary>
        private Dictionary<string, string> BuildExtraFormParams(List<DrugItem> currentMapped,
            List<DrugItem> previousMapped, MimsCrossPrescriptionOption crossOption, MimsResult result)
        {
            Dictionary<string, string> extraFormParams = null;
            try
            {
                if (crossOption == null)
                    return null;

                // Chỉ lọc theo thuốc khi thực sự có thuốc đơn khác trong request —
                // không có thì giữ nguyên request như hiện tại (tránh đổi hành vi ngoài ý muốn).
                if (crossOption.UseAlertFilterByDrug && previousMapped != null && previousMapped.Count > 0)
                {
                    string filterByDrug = MimsRequestBuilder.BuildAlertFilterByDrug(currentMapped);
                    if (!string.IsNullOrEmpty(filterByDrug))
                    {
                        extraFormParams = new Dictionary<string, string>();
                        extraFormParams.Add("alertfilterbydrug", filterByDrug);
                        if (result != null) result.IsAlertFilteredByDrug = true;
                    }
                }

                if (!string.IsNullOrWhiteSpace(crossOption.AlertFilterBySeverity))
                {
                    if (extraFormParams == null) extraFormParams = new Dictionary<string, string>();
                    extraFormParams.Add("alertfilterbyseverity", crossOption.AlertFilterBySeverity);
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return extraFormParams;
        }

        /// <summary>
        /// QT-16 — chỉ giữ cảnh báo có liên quan tới ít nhất 1 thuốc ĐANG KÊ.
        /// Dùng khi không gửi được "alertfilterbydrug" (MIMS đã lọc sẵn thì không cần).
        /// Chỉ lọc được trên danh sách detail đã parse; HTML do MIMS transform nên vẫn giữ nguyên.
        /// </summary>
        private void FilterAlertsByCurrentDrugs(MimsResult result, List<DrugItem> currentMapped)
        {
            try
            {
                if (result == null || currentMapped == null || currentMapped.Count == 0)
                    return;

                var currentGuids = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                foreach (var drug in currentMapped)
                {
                    if (drug != null && !string.IsNullOrEmpty(drug.MimsGuid))
                        currentGuids.Add(NormalizeGuid(drug.MimsGuid));
                }
                if (currentGuids.Count == 0) return;

                int beforeDrugDrug = result.DrugDrugAlertDetails == null ? 0 : result.DrugDrugAlertDetails.Count;
                if (result.DrugDrugAlertDetails != null && result.DrugDrugAlertDetails.Count > 0)
                {
                    result.DrugDrugAlertDetails = result.DrugDrugAlertDetails
                        .Where(o => IsCurrentDrugReference(o.PrimaryDrugReference, currentGuids)
                                 || IsCurrentDrugReference(o.InteractingDrugReference, currentGuids))
                        .ToList();
                }

                Inventec.Common.Logging.LogSystem.Debug(string.Format(
                    "DrugHealthService.FilterAlertsByCurrentDrugs - DrugDrugAlertDetails {0} -> {1}",
                    beforeDrugDrug,
                    result.DrugDrugAlertDetails == null ? 0 : result.DrugDrugAlertDetails.Count));
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Reference của MIMS có dạng "{GUID}" — so sánh sau khi bỏ ngoặc nhọn.
        /// Reference rỗng coi như KHÔNG loại (giữ cảnh báo để tránh mất cảnh báo do thiếu dữ liệu).
        /// </summary>
        private static bool IsCurrentDrugReference(string reference, HashSet<string> currentGuids)
        {
            if (string.IsNullOrWhiteSpace(reference)) return true;
            return currentGuids.Contains(NormalizeGuid(reference));
        }

        private static string NormalizeGuid(string guid)
        {
            return guid == null ? string.Empty : guid.Trim().Trim('{', '}');
        }

        /// <summary>
        /// Gộp thuốc đơn hiện tại và thuốc đơn khác cho phép kiểm tra VN Contraindication
        /// (dò theo HisDrugCode nên dùng DrugItem GỐC, chưa map MIMS).
        /// </summary>
        private static List<DrugItem> MergeDrugsForVnCheck(List<DrugItem> drugs, List<DrugItem> previousDrugs)
        {
            if (previousDrugs == null || previousDrugs.Count == 0)
                return drugs;

            var merged = new List<DrugItem>();
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            if (drugs != null)
            {
                foreach (var drug in drugs)
                {
                    if (drug == null) continue;
                    if (!string.IsNullOrWhiteSpace(drug.HisDrugCode) && !seen.Add(drug.HisDrugCode)) continue;
                    merged.Add(drug);
                }
            }

            foreach (var drug in previousDrugs)
            {
                if (drug == null || string.IsNullOrWhiteSpace(drug.HisDrugCode)) continue;
                if (!seen.Add(drug.HisDrugCode)) continue;
                merged.Add(drug);
            }

            return merged;
        }

        /// <summary>
        /// QT-11/QT-17 — chèn khối "Thuốc đang dùng từ đơn khác trong hồ sơ" lên đầu HTML cảnh báo.
        /// Không có thuốc đơn khác hoặc HTML rỗng → trả về HTML gốc, không đổi gì.
        /// </summary>
        private string PrependPreviousDrugBanner(string html,
            List<MimsPreviousDrugInfo> previousDrugInfos, MimsCrossPrescriptionOption crossOption)
        {
            try
            {
                if (string.IsNullOrEmpty(html)
                    || previousDrugInfos == null || previousDrugInfos.Count == 0)
                    return html;

                var option = crossOption ?? new MimsCrossPrescriptionOption();
                var sb = new System.Text.StringBuilder();

                sb.Append("<div style=\"font-family:'Segoe UI',Arial,sans-serif;font-size:13px;");
                sb.Append("border:1px solid #d9822b;background:#fff7e6;color:#4a3000;");
                sb.Append("padding:8px 10px;margin:0 0 10px 0;\">");
                sb.Append("<b>").Append(Escape(option.GetBannerTitle())).Append("</b>");
                sb.Append("<ul style=\"margin:6px 0 0 18px;padding:0;\">");

                foreach (var info in previousDrugInfos)
                {
                    if (info == null) continue;

                    sb.Append("<li><b>").Append(Escape(info.DrugName)).Append("</b>");

                    var source = string.Format(option.GetSourceFormat(),
                        FormatDate(info.IntructionTime), FormatDate(info.UseTimeTo));
                    sb.Append(" — ").Append(Escape(source));

                    var extra = new List<string>();
                    if (!string.IsNullOrWhiteSpace(info.ExpMestCode)) extra.Add(info.ExpMestCode);
                    if (!string.IsNullOrWhiteSpace(info.DepartmentName)) extra.Add(info.DepartmentName);
                    if (!string.IsNullOrWhiteSpace(info.RequestUserName)) extra.Add(info.RequestUserName);
                    if (extra.Count > 0)
                        sb.Append(" (").Append(Escape(string.Join(" - ", extra))).Append(")");

                    sb.Append("</li>");
                }

                sb.Append("</ul></div>");

                // Chèn ngay sau <body ...> nếu có, để không nằm ngoài thân trang
                int bodyIndex = html.IndexOf("<body", StringComparison.OrdinalIgnoreCase);
                if (bodyIndex >= 0)
                {
                    int bodyEnd = html.IndexOf('>', bodyIndex);
                    if (bodyEnd > 0)
                        return html.Insert(bodyEnd + 1, sb.ToString());
                }

                return sb.ToString() + html;
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return html;
            }
        }

        private static string Escape(string value)
        {
            return System.Security.SecurityElement.Escape(value ?? string.Empty);
        }

        /// <summary>Đổi thời gian dạng yyyyMMddHHmmss sang dd/MM/yyyy để hiển thị.</summary>
        private static string FormatDate(long? timeNumber)
        {
            try
            {
                if (timeNumber == null || timeNumber <= 0) return "";
                var value = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(timeNumber.Value);
                return value == null ? "" : value.Value.ToString("dd/MM/yyyy");
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return "";
            }
        }

        /// <summary>
        /// Ghi bổ sung thông tin phạm vi kiểm tra chéo đơn vào log audit (YC-07).
        /// </summary>
        private void ApplyCrossPrescriptionLog(HIS_MIMS_INTERACTION_LOG interactionLog,
            MimsResult result, MimsCrossPrescriptionOption crossOption)
        {
            try
            {
                if (interactionLog == null || result == null || result.PreviousDrugCount <= 0)
                    return;

                if (!string.IsNullOrEmpty(result.PreviousDrugGuids))
                {
                    // Cắt độ dài để không làm đổ bản ghi log khi hồ sơ có nhiều thuốc
                    interactionLog.CHECKED_GUIDS = Truncate(
                        string.IsNullOrEmpty(interactionLog.CHECKED_GUIDS)
                            ? result.PreviousDrugGuids
                            : interactionLog.CHECKED_GUIDS + ";" + result.PreviousDrugGuids,
                        CHECKED_GUIDS_MAX_LENGTH);
                }

                interactionLog.REQUEST_PARAMS = string.Format(
                    "{{\"requestMode\":{0},\"previousDrugCount\":{1},\"useAlertFilterByDrug\":{2}}}",
                    crossOption == null ? MimsCrossPrescriptionOption.REQUEST_MODE__MERGE_PRESCRIBING : crossOption.RequestMode,
                    result.PreviousDrugCount,
                    result.IsAlertFilteredByDrug ? "true" : "false");

                interactionLog.NOTE = string.Format(
                    "52540: kiem tra tuong tac cheo don - {0} thuoc tu don khac", result.PreviousDrugCount);
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion
    }
}
