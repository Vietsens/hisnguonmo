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
using EMR.EFMODEL.DataModels;
using EMR.Filter;
using EMR.TDO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Plugins.Library.ExpMestAttachFile.ADO;
using HIS.Desktop.Plugins.Library.ExpMestAttachFile.Config;
using Inventec.Common.Adapter;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace HIS.Desktop.Plugins.Library.ExpMestAttachFile.Base
{
    /// <summary>
    /// v57853 - EMR API calls of the prescription attachment.
    /// Link rule (document belongs to the EXPORT TICKET and also carries the right treatment code):
    ///   - TreatmentCode = TDL_TREATMENT_CODE (walk-in sale without treatment -> EXP_MEST_CODE, backend requires not empty)
    ///   - HisCode = "{MaSite} EXP_MEST_CODE:{code} SERVICE_REQ_CODE:{code}|EXP_MEST_CODE:{code}|EXP_STOCK:{code}|REQ_DEPT:{code}"
    ///     EMR backend (HisCodeStockParser) splits the blocks after the first '|' into EMR_DOCUMENT.EXP_MEST_CODE,
    ///     EXP_MEDI_STOCK_CODE, REQ_DEPARTMENT_CODE (same format as MPS AbstractProcessor.BuildEmrStockData).
    ///   - IsOutsideTreatment = true: not part of the treatment record, no treatment lock/store verify on the backend.
    /// </summary>
    internal class AttachDocumentWorker
    {
        private const string URI__DOCUMENT_TYPE_GET = "api/EmrDocumentType/Get";
        private const string URI__DOCUMENT_GET_VIEW = "api/EmrDocument/GetView";
        private const string URI__DOCUMENT_DOWNLOAD_FILE = "api/EmrDocument/DownloadFile";

        /// <summary>HIS_CODE block format - MUST match EMR backend HisCodeStockParser</summary>
        private const char BLOCK_SEPARATOR = '|';
        private const string KEY__EXP_MEST_CODE = "EXP_MEST_CODE";
        private const string KEY__EXP_STOCK = "EXP_STOCK";
        private const string KEY__REQ_DEPT = "REQ_DEPT";

        /// <summary>"EXP_MEST_CODE:xxx" - value ends at a space or '|' (legacy part and block part both match)</summary>
        private static readonly Regex EXP_MEST_CODE_REGEX = new Regex(KEY__EXP_MEST_CODE + @":([^\s|]+)", RegexOptions.Compiled);

        /// <summary>Cached EMR_DOCUMENT_TYPE.ID of EXPSA (null = not loaded yet / not declared)</summary>
        private static long? documentTypeId;
        private static bool isDocumentTypeLoaded;

        /// <summary>ID of document type EXPSA; null when the type is not declared in EMR</summary>
        internal static long? GetDocumentTypeId()
        {
            if (isDocumentTypeLoaded)
                return documentTypeId;
            CommonParam param = new CommonParam();
            try
            {
                EmrDocumentTypeFilter filter = new EmrDocumentTypeFilter();
                filter.DOCUMENT_TYPE_CODE__EXACT = AttachFileConfig.DOCUMENT_TYPE_CODE;
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                var types = new BackendAdapter(param).Get<List<EMR_DOCUMENT_TYPE>>(URI__DOCUMENT_TYPE_GET, ApiConsumers.EmrConsumer, filter, param);
                var type = types != null ? types.FirstOrDefault(o => o.DOCUMENT_TYPE_CODE == AttachFileConfig.DOCUMENT_TYPE_CODE) : null;
                if (type != null)
                {
                    // Cache only when found -> admin can declare the type later without restarting the app
                    documentTypeId = type.ID;
                    isDocumentTypeLoaded = true;
                }
                if (documentTypeId == null)
                    Inventec.Common.Logging.LogSystem.Warn("Khong tim thay EMR_DOCUMENT_TYPE co ma " + AttachFileConfig.DOCUMENT_TYPE_CODE
                        + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => param), param));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return documentTypeId;
        }

        /// <summary>Documents (not deleted) attached to one export ticket, newest first</summary>
        internal static List<V_EMR_DOCUMENT> GetDocuments(ExpMestAttachInfoADO expMest, CommonParam param)
        {
            List<V_EMR_DOCUMENT> result = new List<V_EMR_DOCUMENT>();
            try
            {
                if (expMest == null || String.IsNullOrWhiteSpace(expMest.EXP_MEST_CODE))
                    return result;
                var data = QueryDocuments(new List<ExpMestAttachInfoADO> { expMest }, true, param);
                result = data.Where(o => expMest.EXP_MEST_CODE == GetExpMestCodeOfDocument(o)).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// EXP_MEST_CODEs (among the input tickets) that have at least one attached prescription.
        /// ONE API call for the whole grid page -> no per-row call.
        /// </summary>
        internal static HashSet<string> GetExpMestCodesHasAttach(List<ExpMestAttachInfoADO> expMests)
        {
            HashSet<string> result = new HashSet<string>();
            CommonParam param = new CommonParam();
            try
            {
                if (expMests == null || expMests.Count == 0)
                    return result;
                HashSet<string> requested = new HashSet<string>(expMests
                    .Where(o => o != null && !String.IsNullOrEmpty(o.EXP_MEST_CODE))
                    .Select(o => o.EXP_MEST_CODE));
                foreach (var doc in QueryDocuments(expMests, false, param))
                {
                    string code = GetExpMestCodeOfDocument(doc);
                    if (code != null && requested.Contains(code))
                        result.Add(code);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        /// <summary>
        /// EXPSA documents with TREATMENT_CODEs = treatment codes + export ticket codes
        /// (EmrDocumentViewFilter has no EXP_MEST_CODEs list; the ticket code covers walk-in sales and documents
        /// created before the treatment code was attached). Caller filters the result by GetExpMestCodeOfDocument.
        /// </summary>
        private static List<V_EMR_DOCUMENT> QueryDocuments(List<ExpMestAttachInfoADO> expMests, bool isOrderNewest, CommonParam param)
        {
            long? typeId = GetDocumentTypeId();
            if (!typeId.HasValue)
                return new List<V_EMR_DOCUMENT>();

            HashSet<string> codes = new HashSet<string>();
            foreach (var expMest in expMests)
            {
                if (expMest == null) continue;
                if (!String.IsNullOrEmpty(expMest.EXP_MEST_CODE)) codes.Add(expMest.EXP_MEST_CODE);
                if (!String.IsNullOrEmpty(expMest.TDL_TREATMENT_CODE)) codes.Add(expMest.TDL_TREATMENT_CODE);
            }
            if (codes.Count == 0)
                return new List<V_EMR_DOCUMENT>();

            EmrDocumentViewFilter filter = BuildViewFilter(codes.ToList(), typeId.Value);
            if (isOrderNewest)
            {
                filter.ORDER_FIELD = "CREATE_TIME";
                filter.ORDER_DIRECTION = "DESC";
            }
            var data = new BackendAdapter(param).Get<List<V_EMR_DOCUMENT>>(URI__DOCUMENT_GET_VIEW, ApiConsumers.EmrConsumer, filter, param);
            return data ?? new List<V_EMR_DOCUMENT>();
        }

        /// <summary>
        /// Export ticket code of a document: EMR_DOCUMENT.EXP_MEST_CODE (split by the backend from HIS_CODE);
        /// fallback parse "EXP_MEST_CODE:xxx" in HIS_CODE (backend without the parser / documents created before).
        /// </summary>
        internal static string GetExpMestCodeOfDocument(V_EMR_DOCUMENT doc)
        {
            if (doc == null) return null;
            if (!String.IsNullOrWhiteSpace(doc.EXP_MEST_CODE))
                return doc.EXP_MEST_CODE.Trim();
            if (String.IsNullOrEmpty(doc.HIS_CODE))
                return null;
            var match = EXP_MEST_CODE_REGEX.Match(doc.HIS_CODE);
            return match.Success ? match.Groups[1].Value : null;
        }

        /// <summary>Create one EMR document (merged PDF) for the export ticket</summary>
        internal static DocumentTDO CreateDocument(ExpMestAttachInfoADO expMest, byte[] pdfData, string documentName, CommonParam param)
        {
            DocumentTDO result = null;
            DocumentTDO docCreate = new DocumentTDO();
            try
            {
                docCreate.DocumentName = documentName;
                docCreate.DocumentTypeId = GetDocumentTypeId();
                // Right treatment code of the ticket; walk-in sale has none -> ticket code (backend requires not empty)
                docCreate.TreatmentCode = !String.IsNullOrWhiteSpace(expMest.TDL_TREATMENT_CODE) ? expMest.TDL_TREATMENT_CODE : expMest.EXP_MEST_CODE;
                docCreate.HisCode = BuildHisCode(expMest);
                docCreate.IsOutsideTreatment = true;
                docCreate.IsCapture = true;
                docCreate.FileType = FileType.PDF;
                docCreate.OriginalVersion = new VersionTDO();
                docCreate.OriginalVersion.Base64Data = Convert.ToBase64String(pdfData);

                result = new BackendAdapter(param).Post<DocumentTDO>(EMR.URI.EmrDocument.CREATE_BY_TDO, ApiConsumers.EmrConsumer, docCreate, param);
                // Do not trace Base64Data (large) -> log the ticket + result only
                Inventec.Common.Logging.LogSystem.Debug("CreateByTdo " + (result != null ? "thanh cong" : "that bai")
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => expMest), expMest)
                    + Inventec.Common.Logging.LogUtil.TraceData("pdfBytes", pdfData.Length)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => param), param));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }

        /// <summary>Soft delete (IS_DELETE=1) — EMR keeps MODIFIER/MODIFY_TIME as the audit trail</summary>
        internal static bool DeleteDocument(long documentId, CommonParam param)
        {
            bool result = false;
            try
            {
                result = new BackendAdapter(param).Post<bool>(EMR.URI.EmrDocument.DELETE, ApiConsumers.EmrConsumer, documentId, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        /// <summary>Download the (merged) content of one document</summary>
        internal static EMR.SDO.EmrDocumentFileSDO DownloadDocument(long documentId, CommonParam param)
        {
            EMR.SDO.EmrDocumentFileSDO result = null;
            try
            {
                EMR.SDO.EmrDocumentDownloadFileSDO sdo = new EMR.SDO.EmrDocumentDownloadFileSDO();
                sdo.EmrDocumentViewFilter = new EmrDocumentViewFilter();
                sdo.EmrDocumentViewFilter.ID = documentId;
                sdo.IsMerge = true;
                var files = new BackendAdapter(param).Post<List<EMR.SDO.EmrDocumentFileSDO>>(URI__DOCUMENT_DOWNLOAD_FILE, ApiConsumers.EmrConsumer, sdo, param);
                if (files != null)
                    result = files.FirstOrDefault(o => !String.IsNullOrEmpty(o.Base64Data));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }

        private static EmrDocumentViewFilter BuildViewFilter(List<string> treatmentCodes, long typeId)
        {
            EmrDocumentViewFilter filter = new EmrDocumentViewFilter();
            filter.TREATMENT_CODEs = treatmentCodes;
            filter.DOCUMENT_TYPE_ID = typeId;
            filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
            // EmrDocumentFilter has no IS_DELETE -> must use the view filter to hide soft-deleted documents
            filter.IS_DELETE = false;
            return filter;
        }

        /// <summary>
        /// "{legacy}|EXP_MEST_CODE:x|EXP_STOCK:y|REQ_DEPT:z" - legacy part kept for reading/compatibility,
        /// blocks after the first '|' are split by EMR into EMR_DOCUMENT.EXP_MEST_CODE / EXP_MEDI_STOCK_CODE / REQ_DEPARTMENT_CODE.
        /// </summary>
        internal static string BuildHisCode(ExpMestAttachInfoADO expMest)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0} {1}:{2}", HIS.Desktop.Utility.StringUtil.CustomerCode, KEY__EXP_MEST_CODE, expMest.EXP_MEST_CODE);
            if (!String.IsNullOrEmpty(expMest.TDL_SERVICE_REQ_CODE))
                sb.Append(" SERVICE_REQ_CODE:").Append(expMest.TDL_SERVICE_REQ_CODE);
            AppendBlock(sb, KEY__EXP_MEST_CODE, expMest.EXP_MEST_CODE);
            AppendBlock(sb, KEY__EXP_STOCK, expMest.MEDI_STOCK_CODE);
            AppendBlock(sb, KEY__REQ_DEPT, expMest.REQ_DEPARTMENT_CODE);
            return sb.ToString();
        }

        private static void AppendBlock(StringBuilder sb, string key, string value)
        {
            if (!String.IsNullOrWhiteSpace(value))
                sb.Append(BLOCK_SEPARATOR).Append(key).Append(':').Append(value.Trim());
        }
    }
}
