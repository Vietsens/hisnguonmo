using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.MIMS.Integration.Models;
using HIS.Desktop.MIMS.Integration.View;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.MIMS.Integration.Core
{
    public class BaseService
    {
        public string NameText { get; set; }

        public static string BuildSimpleHtml(string message)
        {
            string safe = System.Security.SecurityElement.Escape(message ?? string.Empty);
            return "<html><head><meta charset=\"utf-8\"/></head><body><h3>" + safe + "</h3></body></html>";
        }

        public List<DrugItem> MappingMIMS(DrugItem drug)
        {
            return MappingMIMS(new List<DrugItem> { drug });
        }

        public List<DrugItem> MappingMIMS(List<DrugItem> drugs)
        {
            var result = new List<DrugItem>();
            try
            {
                Inventec.Common.Logging.LogSystem.Debug(
                    "BaseService.MappingMIMS - start"
                    + Inventec.Common.Logging.LogUtil.TraceData(
                        Inventec.Common.Logging.LogUtil.GetMemberName(() => drugs), drugs));

                if (drugs == null || drugs.Count == 0) return result;

                var allMedType = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>();
                var allAtc = BackendDataWorker.Get<HIS_ATC>();
                var allAcin = BackendDataWorker.Get<HIS_MEDICINE_TYPE_ACIN>();
                var allAcIng = BackendDataWorker.Get<HIS_ACTIVE_INGREDIENT>();

                Inventec.Common.Logging.LogSystem.Debug(string.Format(
                    "BaseService.MappingMIMS - cache: V_HIS_MEDICINE_TYPE={0}, HIS_ATC={1}, HIS_MEDICINE_TYPE_ACIN={2}, HIS_ACTIVE_INGREDIENT={3}",
                    allMedType == null ? 0 : allMedType.Count,
                    allAtc == null ? 0 : allAtc.Count,
                    allAcin == null ? 0 : allAcin.Count,
                    allAcIng == null ? 0 : allAcIng.Count));

                var seenGuids = new HashSet<string>();

                foreach (var drug in drugs)
                {
                    if (drug == null || drug.HisDrugCode == null) continue;

                    var med = allMedType.FirstOrDefault(o => o.MEDICINE_TYPE_CODE == drug.HisDrugCode);
                    if (med == null) continue;

                    // Expand via ATC codes
                    if (!string.IsNullOrEmpty(med.ATC_CODES))
                    {
                        var atcCodes = med.ATC_CODES.Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (var raw in atcCodes)
                        {
                            var atcCode = raw.Trim();
                            if (string.IsNullOrEmpty(atcCode)) continue;
                            var atcRow = allAtc.FirstOrDefault(o => o.ATC_CODE == atcCode && o.IS_MIMS_MAPPED == 1);
                            if (atcRow == null || string.IsNullOrEmpty(atcRow.MIMS_GUID)) continue;
                            if (!seenGuids.Add(atcRow.MIMS_GUID)) continue;
                            result.Add(new DrugItem
                            {
                                HisDrugCode = drug.HisDrugCode,
                                Name = atcRow.MIMS_NAME ?? med.MEDICINE_TYPE_NAME,
                                MimsGuid = atcRow.MIMS_GUID,
                                DrugType = ConvertToMimsType(atcRow.MIMS_TYPE)
                            });
                        }
                    }

                    // Expand via active ingredients
                    var acinRows = allAcin.Where(o => o.MEDICINE_TYPE_ID == med.ID).ToList();
                    foreach (var acin in acinRows)
                    {
                        var acIng = allAcIng.FirstOrDefault(o => o.ID == acin.ACTIVE_INGREDIENT_ID && o.IS_MIMS_MAPPED == 1);
                        if (acIng == null || string.IsNullOrEmpty(acIng.MIMS_GUID)) continue;
                        if (!seenGuids.Add(acIng.MIMS_GUID)) continue;
                        result.Add(new DrugItem
                        {
                            HisDrugCode = drug.HisDrugCode,
                            Name = acIng.MIMS_NAME ?? med.MEDICINE_TYPE_NAME,
                            MimsGuid = acIng.MIMS_GUID,
                            DrugType = ConvertToMimsType(acIng.MIMS_TYPE)
                        });
                    }
                }

                Inventec.Common.Logging.LogSystem.Debug(string.Format(
                    "BaseService.MappingMIMS - mapped {0} input drugs -> {1} DrugItems with MimsGuid",
                    drugs.Count, result.Count)
                    + Inventec.Common.Logging.LogUtil.TraceData(
                        Inventec.Common.Logging.LogUtil.GetMemberName(() => result), result));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        public List<string> ExtractAtcCodes(List<DrugItem> drugs)
        {
            var result = new List<string>();
            try
            {
                Inventec.Common.Logging.LogSystem.Debug(
                    "BaseService.ExtractAtcCodes - start"
                    + Inventec.Common.Logging.LogUtil.TraceData(
                        Inventec.Common.Logging.LogUtil.GetMemberName(() => drugs), drugs));

                if (drugs == null || drugs.Count == 0) return result;
                var allMedType = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>();
                var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                foreach (var drug in drugs)
                {
                    if (drug == null || drug.HisDrugCode == null) continue;
                    var med = allMedType.FirstOrDefault(o => o.MEDICINE_TYPE_CODE == drug.HisDrugCode);
                    if (med == null || string.IsNullOrEmpty(med.ATC_CODES)) continue;
                    var codes = med.ATC_CODES.Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var raw in codes)
                    {
                        var atc = raw.Trim();
                        if (!string.IsNullOrEmpty(atc) && seen.Add(atc))
                            result.Add(atc);
                    }
                }

                Inventec.Common.Logging.LogSystem.Debug(
                    "BaseService.ExtractAtcCodes - result"
                    + Inventec.Common.Logging.LogUtil.TraceData(
                        Inventec.Common.Logging.LogUtil.GetMemberName(() => result), result));
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); }
            return result;
        }

        private MimsType ConvertToMimsType(short? mimsType)
        {
            switch (mimsType)
            {
                case 1: return MimsType.GGPI;
                case 2: return MimsType.Product;
                case 3: return MimsType.GenericItem;
                case 4: return MimsType.Molecule;
                case 5: return MimsType.SubstanceClass;
                default: return MimsType.GGPI;
            }
        }

        public void ShowResult(MimsResult result)
        {
            if (result != null && !string.IsNullOrEmpty(result.Html))
            {
                WebViewHelper.ShowHtml(result.Html, NameText);
            }
        }

        /// <summary>
        /// Last VN Contraindication request XML — populated by CheckVnContraindication.
        /// Used for interaction logging when user confirms VN dialog.
        /// </summary>
        protected string lastVnRequestXml;

        /// <summary>
        /// Pure check: gọi VN Contraindication API và parse response, KHÔNG show UI.
        /// Trả về MimsResult với VnContraindicationDetails + Html nếu có tương tác.
        /// </summary>
        protected MimsResult CheckVnContraindication(List<DrugItem> drugs)
        {
            var result = new MimsResult();
            try
            {
                Inventec.Common.Logging.LogSystem.Debug(
                    "BaseService.CheckVnContraindication - start"
                    + Inventec.Common.Logging.LogUtil.TraceData(
                        Inventec.Common.Logging.LogUtil.GetMemberName(() => drugs), drugs));

                var atcCodes = ExtractAtcCodes(drugs);
                Inventec.Common.Logging.LogSystem.Debug(
                    "VN check - atcCodes"
                    + Inventec.Common.Logging.LogUtil.TraceData(
                        Inventec.Common.Logging.LogUtil.GetMemberName(() => atcCodes), atcCodes));
                if (atcCodes.Count == 0)
                {
                    Inventec.Common.Logging.LogSystem.Debug("VN check - SKIP (no ATC codes resolved from input drugs)");
                    result.Success = false;
                    return result;
                }

                lastVnRequestXml = MimsRequestBuilder.BuildVnContraindicationRequest(atcCodes);
                Inventec.Common.Logging.LogSystem.Debug("VN check - request XML: " + lastVnRequestXml);

                bool isTimeout;
                string xmlResponse = MimsClient.PostXml(MimsConfig.VnContraApiUrl, lastVnRequestXml, out isTimeout);
                Inventec.Common.Logging.LogSystem.Debug(string.Format(
                    "VN check - isTimeout={0}, responseLength={1}",
                    isTimeout, xmlResponse == null ? 0 : xmlResponse.Length));

                result.RawXml = xmlResponse;
                result.IsTimeout = isTimeout;

                if (isTimeout || string.IsNullOrEmpty(xmlResponse))
                {
                    result.Success = false;
                    return result;
                }

                result.VnContraindicationDetails = MimsResultDetailParser.ParseVnContraindicationInteractions(xmlResponse);
                Inventec.Common.Logging.LogSystem.Debug(string.Format(
                    "VN check - parsed details.Count={0}",
                    result.VnContraindicationDetails == null ? 0 : result.VnContraindicationDetails.Count));

                if (result.VnContraindicationDetails != null && result.VnContraindicationDetails.Count > 0)
                {
                    result.Html = MimsResponseTransformer.XmlToHtml(xmlResponse);
                    result.Success = !string.IsNullOrEmpty(result.Html);
                }
                else
                {
                    result.Success = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error("CheckVnContraindication exception", ex);
            }
            return result;
        }

        /// <summary>
        /// Check VN Contraindication và hiển thị dialog Yes/No (giống CDS path).
        /// Nếu user click Yes và có interactionLog → ghi log audit.
        /// </summary>
        /// <returns>true = user accept tiếp tục / không có tương tác; false = user reject</returns>
        protected bool CheckAndShowVnContraindication(List<DrugItem> drugs,
            HIS_MIMS_INTERACTION_LOG interactionLog = null,
            long? treatmentId = null, long? serviceReqId = null, long? patientId = null)
        {
            try
            {
                var vnResult = CheckVnContraindication(drugs);

                bool hasVnAlert = vnResult != null
                    && vnResult.VnContraindicationDetails != null
                    && vnResult.VnContraindicationDetails.Count > 0
                    && !string.IsNullOrEmpty(vnResult.Html);

                Inventec.Common.Logging.LogSystem.Debug(string.Format(
                    "CheckAndShowVnContraindication - hasVnAlert={0}", hasVnAlert));

                if (!hasVnAlert) return true;

                bool rs = WebViewHelper.ShowDialog(vnResult.Html, "Kiểm tra tương tác thuốc (VN)");
                Inventec.Common.Logging.LogSystem.Debug(string.Format(
                    "CheckAndShowVnContraindication - dialog returned {0}", rs));

                if (rs && interactionLog != null)
                    SaveVnInteractionLog(drugs, vnResult, interactionLog, treatmentId, serviceReqId, patientId);

                return rs;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error("CheckAndShowVnContraindication exception", ex);
            }
            return true;
        }

        /// <summary>
        /// Ghi log audit cho VN Contraindication interaction (MODULE_TYPE=5 cho VN).
        /// Pattern tương tự DrugHealthService.SaveDataInteractionLog (MODULE_TYPE=4 cho Drug-Health).
        /// </summary>
        protected void SaveVnInteractionLog(List<DrugItem> drugs, MimsResult vnResult,
            HIS_MIMS_INTERACTION_LOG interactionLog,
            long? treatmentId, long? serviceReqId, long? patientId)
        {
            try
            {
                interactionLog.TREATMENT_ID = treatmentId;
                interactionLog.SERVICE_REQ_ID = serviceReqId;
                interactionLog.PATIENT_ID = patientId;
                interactionLog.MODULE_TYPE = 5; // 5 = VN Contraindication
                interactionLog.REQUEST_TYPE = "INTERACTION";
                interactionLog.REQUEST_ENDPOINT = MimsConfig.VnContraApiUrl;
                interactionLog.REQUEST_XML = lastVnRequestXml;
                interactionLog.CHECKED_GUIDS = string.Join(";",
                    drugs.Where(d => !string.IsNullOrWhiteSpace(d.HisDrugCode)).Select(d => d.HisDrugCode));
                interactionLog.DRUG_COUNT = (short)(drugs.Where(d => !string.IsNullOrWhiteSpace(d.HisDrugCode)).Count());
                interactionLog.UNMAPPED_DRUG_COUNT = 0;
                interactionLog.RESPONSE_XML = vnResult.RawXml;
                interactionLog.RESPONSE_HTML = "";
                interactionLog.RESPONSE_TYPE = "xml";
                interactionLog.HAS_ALERT = 1;
                interactionLog.ALERT_COUNT = (short?)(vnResult.VnContraindicationDetails == null ? 0 : vnResult.VnContraindicationDetails.Count);

                bool hasContraindicated = vnResult.VnContraindicationDetails != null
                    && vnResult.VnContraindicationDetails.Exists(o =>
                        !string.IsNullOrEmpty(o.InteractionLevel)
                        && o.InteractionLevel.IndexOf("Chống chỉ định", StringComparison.OrdinalIgnoreCase) >= 0);
                interactionLog.HAS_SEVERE_ALERT = hasContraindicated ? (short?)1 : null;
                interactionLog.HIGHEST_SEVERITY = hasContraindicated ? "CONTRAINDICATED" : null;

                interactionLog.IS_SUCCESS = vnResult.Success ? (short?)1 : (short?)0;
                interactionLog.ERROR_MESSAGE = vnResult.Message;
                interactionLog.USER_ACKNOWLEDGED = 1;
                interactionLog.USER_OVERRIDE = 1;
                interactionLog.OVERRIDE_BY = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName();
                interactionLog.OVERRIDE_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now);

                Inventec.Common.Logging.LogSystem.Debug(
                    Inventec.Common.Logging.LogUtil.TraceData("MimsVnInteractionLog", interactionLog));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
