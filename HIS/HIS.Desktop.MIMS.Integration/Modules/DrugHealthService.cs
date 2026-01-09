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
        /// Kiểm tra Drug-Health Alert theo tài liệu MIMS (Prescribing + HealthIssueCodes ICD10).
        /// </summary>
        public MimsResult Check(List<DrugItem> drugs, List<string> icd10Codes)
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
            xmlRequest = MimsRequestBuilder.BuildDrugHealthAlertRequest(drugs, icd10Codes);

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
        public bool CheckAndAlert(List<DrugItem> drugs, List<string> icd10Codes, long? treatmentId = null, long? serviceReqId = null, long? patientId = null)
        {
            try
            {
                MimsResult result = Check(drugs, icd10Codes);
                if (!result.Success) return true;
                if (result.DrugHealthAlertDetails == null) result.DrugHealthAlertDetails = new List<DrugHealthAlertDetail>();
                if (result.DrugDrugAlertDetails == null) result.DrugDrugAlertDetails = new List<DrugDrugAlertDetail>();

                if ((result.DrugHealthAlertDetails.Count > 0 && result.DrugHealthAlertDetails.Exists(o => o.SeverityLevel != DrugHealthSeverity.Unknown))
                    || (result.DrugDrugAlertDetails.Count > 0 && result.DrugDrugAlertDetails.Exists(o => o.SeverityLevel != DrugInteractionSeverity.Unknown)))
                {
                    bool rs = WebViewHelper.ShowDialog(result.Html, NameText);
                    if (rs)
                    {
                        HIS_MIMS_INTERACTION_LOG data = new HIS_MIMS_INTERACTION_LOG();
                        data.TREATMENT_ID = treatmentId;
                        data.SERVICE_REQ_ID = serviceReqId;
                        data.PATIENT_ID = patientId;
                        data.MODULE_TYPE = 4; //4=Drug-Health
                        data.REQUEST_TYPE = "INTERACTION";
                        data.REQUEST_ENDPOINT = MimsConfig.CdsApiUrl;
                        data.REQUEST_XML = xmlRequest;
                        data.CHECKED_GUIDS = string.Join(";", drugs.Where(d => !string.IsNullOrWhiteSpace(d.MimsGuid)).Select(d => d.MimsGuid));
                        data.DRUG_COUNT = (short)(drugs.Where(d => !string.IsNullOrWhiteSpace(d.MimsGuid)).Count());
                        data.UNMAPPED_DRUG_COUNT = (short)(drugs.Where(d => string.IsNullOrWhiteSpace(d.MimsGuid)).Count());
                        data.RESPONSE_XML = result.RawXml;
                        data.RESPONSE_HTML = "";
                        data.RESPONSE_TYPE = "xml";
                        data.HAS_ALERT = 1;
                        data.ALERT_COUNT = 1;
                        data.HAS_SEVERE_ALERT = (result.DrugDrugAlertDetails.Exists(o => o.SeverityLevel == DrugInteractionSeverity.Severe)
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
                        data.HIGHEST_SEVERITY = highestSeverity;
                        data.IS_SUCCESS = result.Success ? (short?)1 : (short?)0;
                        data.ERROR_MESSAGE = result.Message;
                        data.USER_ACKNOWLEDGED = 1;
                        data.USER_OVERRIDE = 1;
                        data.OVERRIDE_BY = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName();
                        data.OVERRIDE_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(System.DateTime.Now);
                        CommonParam param = new CommonParam();
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("MimsInteractionLog",data));
                        bool logCreated = new BackendAdapter(param).Post<bool>("api/HisMimsInteractionLog/Create", ApiConsumers.MosConsumer, data, param);
                    }
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return true;
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
