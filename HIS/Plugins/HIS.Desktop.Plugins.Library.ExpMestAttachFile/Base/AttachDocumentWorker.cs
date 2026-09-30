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

namespace HIS.Desktop.Plugins.Library.ExpMestAttachFile.Base
{
    /// <summary>
    /// v57853 - EMR API calls of the prescription attachment.
    /// Link rule: DocumentTDO.TreatmentCode = EXP_MEST_CODE (lets the list screen query a whole page with TREATMENT_CODEs),
    ///            HisCode = "{MaSite} EXP_MEST_CODE:{code} SERVICE_REQ_CODE:{code}", type = EXPSA, outside treatment.
    /// </summary>
    internal class AttachDocumentWorker
    {
        private const string URI__DOCUMENT_TYPE_GET = "api/EmrDocumentType/Get";
        private const string URI__DOCUMENT_GET_VIEW = "api/EmrDocument/GetView";
        private const string URI__DOCUMENT_DOWNLOAD_FILE = "api/EmrDocument/DownloadFile";

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
        internal static List<V_EMR_DOCUMENT> GetDocuments(string expMestCode, CommonParam param)
        {
            List<V_EMR_DOCUMENT> result = new List<V_EMR_DOCUMENT>();
            try
            {
                long? typeId = GetDocumentTypeId();
                if (!typeId.HasValue || String.IsNullOrWhiteSpace(expMestCode))
                    return result;

                EmrDocumentViewFilter filter = BuildViewFilter(new List<string> { expMestCode }, typeId.Value);
                filter.ORDER_FIELD = "CREATE_TIME";
                filter.ORDER_DIRECTION = "DESC";
                var data = new BackendAdapter(param).Get<List<V_EMR_DOCUMENT>>(URI__DOCUMENT_GET_VIEW, ApiConsumers.EmrConsumer, filter, param);
                if (data != null)
                    result = data;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// EXP_MEST_CODEs (among the input) that have at least one attached prescription.
        /// ONE API call for the whole grid page -> no per-row call.
        /// </summary>
        internal static HashSet<string> GetExpMestCodesHasAttach(List<string> expMestCodes)
        {
            HashSet<string> result = new HashSet<string>();
            CommonParam param = new CommonParam();
            try
            {
                if (expMestCodes == null || expMestCodes.Count == 0)
                    return result;
                long? typeId = GetDocumentTypeId();
                if (!typeId.HasValue)
                    return result;

                EmrDocumentViewFilter filter = BuildViewFilter(expMestCodes.Distinct().ToList(), typeId.Value);
                var data = new BackendAdapter(param).Get<List<V_EMR_DOCUMENT>>(URI__DOCUMENT_GET_VIEW, ApiConsumers.EmrConsumer, filter, param);
                if (data != null)
                {
                    foreach (var doc in data)
                    {
                        if (!String.IsNullOrEmpty(doc.TREATMENT_CODE))
                            result.Add(doc.TREATMENT_CODE);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
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
                // Backend verify requires TreatmentCode != empty even for outside-treatment documents
                docCreate.TreatmentCode = expMest.EXP_MEST_CODE;
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

        private static EmrDocumentViewFilter BuildViewFilter(List<string> expMestCodes, long typeId)
        {
            EmrDocumentViewFilter filter = new EmrDocumentViewFilter();
            filter.TREATMENT_CODEs = expMestCodes;
            filter.DOCUMENT_TYPE_ID = typeId;
            filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
            // EmrDocumentFilter has no IS_DELETE -> must use the view filter to hide soft-deleted documents
            filter.IS_DELETE = false;
            return filter;
        }

        private static string BuildHisCode(ExpMestAttachInfoADO expMest)
        {
            string hisCode = String.Format("{0} EXP_MEST_CODE:{1}", HIS.Desktop.Utility.StringUtil.CustomerCode, expMest.EXP_MEST_CODE);
            if (!String.IsNullOrEmpty(expMest.TDL_SERVICE_REQ_CODE))
                hisCode += " SERVICE_REQ_CODE:" + expMest.TDL_SERVICE_REQ_CODE;
            return hisCode;
        }
    }
}
