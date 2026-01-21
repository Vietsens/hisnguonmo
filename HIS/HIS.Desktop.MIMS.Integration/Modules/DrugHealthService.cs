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
        /// Kiểm tra Drug-Health Alert (Prescribing + HealthIssueCodes ICD10).
        /// </summary>
        public MimsResult Check(List<DrugItem> drugs, List<string> icd10Codes)
        {
            return this.Check(drugs, null, icd10Codes);
        }

        /// <summary>
        /// Kiểm tra Drug-Health Alert (Prescribing + HealthIssueCodes ICD10).
        /// </summary>
        public MimsResult Check(List<DrugItem> drugs,List<AllergyItem> allergies, List<string> icd10Codes)
        {
            this.MappingMIMS(drugs);
            var result = new MimsResult();
            if (drugs == null || drugs.Count == 0 || !drugs.Exists(o=>o.MimsGuid!=null))
            {
                result.Success = false;
                result.Message = "Không có thông tin thuốc kiểm tra tương tác";
                result.Html = BuildSimpleHtml(result.Message);
                return result;
            }
            xmlRequest = MimsRequestBuilder.BuildDrugHealthAlertRequest(drugs, allergies, icd10Codes, true,true);

            bool isTimeout;
            string xmlResponse = MimsClient.PostXml(MimsConfig.CdsApiUrl, xmlRequest, out isTimeout);

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

            return result;
        }

        /// <summary>
        /// Kiểm tra Tương tác thuốc, bệnh lý. Hiển thị cảnh báo (nếu có) và ghi log.
        /// </summary>
        public bool CheckAndAlert(List<DrugItem> drugs, List<string> icd10Codes, HIS_MIMS_INTERACTION_LOG interactionLog = null, long? treatmentId = null, long? serviceReqId = null, long? patientId = null)
        {
            return this.CheckAndAlert(drugs, icd10Codes, interactionLog, treatmentId , serviceReqId, patientId);
        }

        /// <summary>
        /// Kiểm tra Tương tác thuốc, bệnh lý. Hiển thị cảnh báo (nếu có) và ghi log.
        /// </summary>
        public bool CheckAndAlert(List<DrugItem> drugs, List<AllergyItem> allergies, List<string> icd10Codes, HIS_MIMS_INTERACTION_LOG interactionLog = null, long? treatmentId = null, long? serviceReqId = null, long? patientId = null)
        {
            try
            {
                MimsResult result = Check(drugs,allergies, icd10Codes);
                if (!result.Success) return true;
                if (result.DrugHealthAlertDetails == null) result.DrugHealthAlertDetails = new List<DrugHealthAlertDetail>();
                if (result.DrugDrugAlertDetails == null) result.DrugDrugAlertDetails = new List<DrugDrugAlertDetail>();

                if ((result.DrugHealthAlertDetails.Count > 0 && result.DrugHealthAlertDetails.Exists(o => o.SeverityLevel != DrugHealthSeverity.Unknown))
                    || (result.DrugDrugAlertDetails.Count > 0 && result.DrugDrugAlertDetails.Exists(o => o.SeverityLevel != DrugInteractionSeverity.Unknown)))
                {
                    bool rs = WebViewHelper.ShowDialog(result.Html, NameText);
                    if (rs && interactionLog != null) SaveDataInteractionLog(drugs, result, interactionLog, treatmentId, serviceReqId, patientId);
                    return rs;
                }
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
                interactionLog.HAS_SEVERE_ALERT = (result.DrugDrugAlertDetails.Exists(o => o.SeverityLevel == DrugInteractionSeverity.Severe)
                                        || result.DrugHealthAlertDetails.Exists(o => o.SeverityLevel == DrugHealthSeverity.Contraindicated))
                                        ? (short?)1 : null;
                string highestSeverity = null;
                if (result.DrugDrugAlertDetails.Exists(o => o.SeverityLevel == DrugInteractionSeverity.Severe))
                {
                    highestSeverity = "SEVERE";
                }
                else if (result.DrugHealthAlertDetails.Exists(o => o.SeverityLevel == DrugHealthSeverity.Contraindicated))
                {
                    highestSeverity = "CONTRAINDICATED";
                }
                else if (result.DrugDrugAlertDetails.Exists(o => o.SeverityLevel == DrugInteractionSeverity.Moderate))
                {
                    highestSeverity = "MODERATE";
                }
                else if (result.DrugHealthAlertDetails.Exists(o => o.SeverityLevel == DrugHealthSeverity.ExtremeCaution))
                {
                    highestSeverity = "EXTREMECAUTION";
                }
                else if (result.DrugDrugAlertDetails.Exists(o => o.SeverityLevel == DrugInteractionSeverity.Minor))
                {
                    highestSeverity = "MINOR";
                }
                else if (result.DrugDrugAlertDetails.Exists(o => o.SeverityLevel == DrugInteractionSeverity.Caution))
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

        public bool ShowDialog(List<DrugItem> drugs, List<string> icd10Codes)
        {
            try
            {
                MimsResult result = Check(drugs, icd10Codes);
                if (result != null && !string.IsNullOrEmpty(result.Html))
                {
                    return WebViewHelper.ShowDialog(result.Html, NameText);
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return true;
        }

        public void ShowResultAsync(List<DrugItem> drugs, List<string> icd10Codes)
        {
            WebViewHelper.ShowResultAsync(() => Check(drugs, icd10Codes), NameText);
        }
    }
}
