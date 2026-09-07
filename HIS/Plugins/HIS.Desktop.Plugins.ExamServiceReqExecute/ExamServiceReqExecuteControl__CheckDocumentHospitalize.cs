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
using DevExpress.XtraEditors;
using EMR.EFMODEL.DataModels;
using EMR.Filter;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.ExamServiceReqExecute.ADO;
using HIS.Desktop.Plugins.ExamServiceReqExecute.Config;
using HIS.Desktop.Plugins.ExamServiceReqExecute.Resources;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ExamServiceReqExecute
{
    /// <summary>
    /// Kiem tra van ban EMR truoc khi xu tri nhap vien, dua tren loai van ban co IS_HOSPITALIZATION = 1 
    /// (loai duoc tich "chan nhap vien").
    ///
    /// Dieu kien kich hoat (phai thoa man ca 3):
    ///  - chkHospitalize duoc tich (xu tri nhap vien);
    ///  - key HIS.Desktop.Plugins.ExamServiceReqExecute.CheckDepaDocument.Hospitalization co gia tri
    ///    (danh sach ma khoa, phan tach boi "|");
    ///  - ma khoa hien tai (khoa cua phong lam viec) nam trong danh sach ma khoa cua key do.
    ///
    /// Van ban cua y lenh Kham khong can ky nen y lenh Kham KHONG duoc tinh la "co chi dinh dich vu":
    /// chi kiem tra van ban khi benh nhan co y lenh khac Kham (SERVICE_REQ_TYPE_ID != ID__KH).
    ///
    /// Khi da co chi dinh dich vu thi xet 3 truong hop:
    ///  - TH1: khong co bat ky van ban nao thuoc loai chan nhap vien -> CANH BAO (Yes/No),
    ///         nguoi dung tu quyet dinh co nhap vien hay khong;
    ///  - TH2: co van ban thuoc loai chan nhap vien nhung chua duoc ky (SIGNERS rong) -> CHAN;
    ///  - TH3: co van ban thuoc loai chan nhap vien nhung chua ky du -> CHAN.
    /// Van ban thuoc loai chan nhap vien va da ky day du -> cho phep luu.
    ///
    /// Ghi chu ve kieu du lieu:
    /// api "api/EmrDocument/MediRecordChecking" tra ve EMR.SDO.MediRecordCheckingResultSDO va
    /// api "api/EmrDocument/GetView" tra ve List&lt;EMR.EFMODEL.DataModels.V_EMR_DOCUMENT&gt;. Ban
    /// EMR.EFMODEL.dll hien tai trong lib\EMR khong con chua cac kieu view V_* nen khong tham chieu
    /// truc tiep duoc (loi CS0570). Vi vay dung ADO cuc bo MediRecordCheckingDocumentADO cho ca hai
    /// api - ban tin la JSON nen chi can trung ten thuoc tinh la Newtonsoft map duoc.
    ///
    /// V_EMR_DOCUMENT khong co cot IS_HOSPITALIZATION nen phai tra cuu them EMR_DOCUMENT_TYPE
    /// de biet loai van ban nao chan nhap vien.
    /// </summary>
    public partial class ExamServiceReqExecuteControl
    {
        private const string URI__EMR_DOCUMENT_MEDI_RECORD_CHECKING = "api/EmrDocument/MediRecordChecking";
        private const string URI__EMR_DOCUMENT_GET_VIEW = "api/EmrDocument/GetView";
        private const string URI__EMR_DOCUMENT_TYPE_GET = "api/EmrDocumentType/Get";
        private const string URI__HIS_SERVICE_REQ_GET = "api/HisServiceReq/Get";

        /// <summary>
        /// Kiem tra van ban thuoc loai chan nhap vien truoc khi cho phep nhap vien.
        /// </summary>
        /// <returns>true = duoc phep luu; false = chan luu.</returns>
        private bool CheckEmrDocumentBeforeHospitalize()
        {
            try
            {
                if (chkHospitalize == null || !chkHospitalize.Checked)
                    return true;

                if (!IsCurrentDepartmentCheckDocumentHospitalize())
                    return true;

                if (this.HisServiceReqView == null)
                {
                    Inventec.Common.Logging.LogSystem.Warn("CheckEmrDocumentBeforeHospitalize: HisServiceReqView null, bo qua kiem tra van ban.");
                    return true;
                }

                string treatmentCode = this.HisServiceReqView.TDL_TREATMENT_CODE;
                if (string.IsNullOrEmpty(treatmentCode))
                {
                    Inventec.Common.Logging.LogSystem.Warn("CheckEmrDocumentBeforeHospitalize: TDL_TREATMENT_CODE rong, bo qua kiem tra van ban.");
                    return true;
                }

                // Van ban cua y lenh Kham khong can ky nen y lenh Kham khong tinh la "co chi dinh dich vu".
                // Benh nhan chua co y lenh nao ngoai Kham -> khong kiem tra van ban.
                if (!HasServiceReqExceptExam(this.HisServiceReqView.TREATMENT_ID))
                    return true;

                List<EMR_DOCUMENT_TYPE> blockingTypes = GetHospitalizationBlockingDocumentTypes();
                if (blockingTypes == null || blockingTypes.Count == 0)
                {
                    Inventec.Common.Logging.LogSystem.Warn("CheckEmrDocumentBeforeHospitalize: khong co loai van ban nao duoc tich chan nhap vien, bo qua kiem tra.");
                    return true;
                }

                // Goi MediRecordChecking TRUOC GetView de biet dich vu EMR con song:
                // neu api nay loi thi bo qua kiem tra, tranh canh bao oan o TH1.
                MediRecordCheckingResultADO checkingResult = GetMediRecordChecking(treatmentCode);
                if (checkingResult == null)
                    return true;

                // TH1: chua co bat ky van ban nao thuoc loai chan nhap vien -> canh bao, nguoi dung tu quyet dinh.
                List<MediRecordCheckingDocumentADO> blockingDocuments =
                    GetTreatmentDocumentsByDocumentTypes(treatmentCode, blockingTypes.Select(o => o.ID).ToList());
                if (blockingDocuments == null || blockingDocuments.Count == 0)
                    return ConfirmHospitalizeWithoutBlockingDocument(checkingResult, blockingTypes);

                // TH2 + TH3: da co van ban thuoc loai chan nhap vien nhung chua duoc ky / chua ky du -> chan.
                List<MediRecordCheckingDocumentADO> unfinishedDocuments =
                    GetUnfinishedBlockingDocuments(checkingResult, blockingDocuments, blockingTypes);
                if (unfinishedDocuments.Count == 0)
                    return true;

                List<string> messages = BuildUnfinishedDocumentMessages(unfinishedDocuments, blockingDocuments, blockingTypes);
                if (messages.Count == 0)
                    return true;

                StringBuilder message = new StringBuilder();
                message.AppendLine("Không thể nhập viện. Hồ sơ còn văn bản chưa hoàn thành:");
                message.AppendLine();
                for (int i = 0; i < messages.Count; i++)
                {
                    message.AppendLine(string.Format("{0}. {1}", i + 1, messages[i]));
                }
                message.AppendLine();
                message.Append("Vui lòng hoàn thiện trước khi nhập viện!");

                XtraMessageBox.Show(message.ToString(), ResourceMessage.ThongBao, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            catch (Exception ex)
            {
                // Loi khi kiem tra thi khong chan nghiep vu luu
                Inventec.Common.Logging.LogSystem.Error(ex);
                return true;
            }
        }

        /// <summary>
        /// Khoa hien tai (khoa cua phong lam viec) co nam trong danh sach ma khoa cua key cau hinh hay khong.
        /// </summary>
        private bool IsCurrentDepartmentCheckDocumentHospitalize()
        {
            try
            {
                List<string> departmentCodes = HisConfigCFG.CheckDepaDocumentHospitalizationCodes;
                if (departmentCodes == null || departmentCodes.Count == 0)
                    return false;

                string currentDepartmentCode = GetCurrentDepartmentCode();
                if (string.IsNullOrEmpty(currentDepartmentCode))
                    return false;

                return departmentCodes.Contains(currentDepartmentCode.Trim().ToUpper());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }
        }

        /// <summary>
        /// Lay DEPARTMENT_CODE cua khoa ung voi phong lam viec hien tai.
        /// </summary>
        private string GetCurrentDepartmentCode()
        {
            try
            {
                if (this.moduleData == null)
                    return null;

                var workPlace = HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkPlaceSDO
                    .FirstOrDefault(o => o.RoomId == this.moduleData.RoomId);
                if (workPlace == null || workPlace.DepartmentId <= 0)
                    return null;

                var department = BackendDataWorker.Get<HIS_DEPARTMENT>()
                    .FirstOrDefault(o => o.ID == workPlace.DepartmentId);
                return department != null ? department.DEPARTMENT_CODE : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }

        /// <summary>
        /// Benh nhan co chi dinh dich vu (y lenh) nao ngoai y lenh Kham hay khong.
        /// Van ban cua y lenh Kham khong can ky nen loai Kham bi loai khoi phep dem nay.
        /// </summary>
        /// <returns>true = co y lenh khac Kham -> phai kiem tra van ban.</returns>
        private bool HasServiceReqExceptExam(long treatmentId)
        {
            try
            {
                if (treatmentId <= 0)
                {
                    Inventec.Common.Logging.LogSystem.Warn("HasServiceReqExceptExam: TREATMENT_ID khong hop le, bo qua kiem tra van ban.");
                    return false;
                }

                CommonParam param = new CommonParam();
                MOS.Filter.HisServiceReqFilter filter = new MOS.Filter.HisServiceReqFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                filter.TREATMENT_ID = treatmentId;
                filter.NOT_IN_SERVICE_REQ_TYPE_IDs = new List<long> { IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH };

                var serviceReqs = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>(
                    URI__HIS_SERVICE_REQ_GET, ApiConsumers.MosConsumer, filter, param);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(
                    "HasServiceReqExceptExam: so y lenh khac Kham", serviceReqs == null ? 0 : serviceReqs.Count));
                return serviceReqs != null && serviceReqs.Count > 0;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }
        }

        /// <summary>
        /// Goi api kiem tra ho so benh an.
        /// </summary>
        private MediRecordCheckingResultADO GetMediRecordChecking(string treatmentCode)
        {
            try
            {
                CommonParam paramCheck = new CommonParam();
                var checkingResult = new BackendAdapter(paramCheck).Post<MediRecordCheckingResultADO>(
                    URI__EMR_DOCUMENT_MEDI_RECORD_CHECKING, ApiConsumers.EmrConsumer, treatmentCode, paramCheck);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("MediRecordChecking result", checkingResult));
                return checkingResult;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }

        /// <summary>
        /// Lay toan bo loai van ban dang hoat dong duoc tich chan nhap vien (IS_HOSPITALIZATION = 1).
        /// EmrDocumentTypeFilter khong co dieu kien IS_HOSPITALIZATION nen phai loc phia client.
        /// </summary>
        private List<EMR_DOCUMENT_TYPE> GetHospitalizationBlockingDocumentTypes()
        {
            try
            {
                CommonParam paramType = new CommonParam();
                EmrDocumentTypeFilter filter = new EmrDocumentTypeFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;

                var documentTypes = new BackendAdapter(paramType).Get<List<EMR_DOCUMENT_TYPE>>(
                    URI__EMR_DOCUMENT_TYPE_GET, ApiConsumers.EmrConsumer, filter, paramType);
                if (documentTypes == null || documentTypes.Count == 0)
                    return null;

                return documentTypes.Where(o => o != null && o.IS_HOSPITALIZATION == 1).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }

        /// <summary>
        /// Lay cac van ban (chua bi xoa) cua dot dieu tri thuoc dung nhung loai van ban chan nhap vien.
        /// Danh sach rong = benh nhan chua co van ban chan nhap vien nao -> TH1 canh bao.
        /// </summary>
        private List<MediRecordCheckingDocumentADO> GetTreatmentDocumentsByDocumentTypes(string treatmentCode, List<long> documentTypeIds)
        {
            try
            {
                if (string.IsNullOrEmpty(treatmentCode) || documentTypeIds == null || documentTypeIds.Count == 0)
                    return null;

                CommonParam param = new CommonParam();
                EmrDocumentViewFilter filter = new EmrDocumentViewFilter();
                filter.TREATMENT_CODE__EXACT = treatmentCode;
                filter.DOCUMENT_TYPE_IDs = documentTypeIds;
                filter.IS_DELETE = false;

                var documents = new BackendAdapter(param).Get<List<MediRecordCheckingDocumentADO>>(
                    URI__EMR_DOCUMENT_GET_VIEW, ApiConsumers.EmrConsumer, filter, param);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(
                    "Van ban thuoc loai chan nhap vien cua dot dieu tri", documents));
                return documents;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }

        /// <summary>
        /// Cac van ban thuoc loai chan nhap vien nhung chua hoan thanh chu ky (TH2 + TH3).
        ///
        /// Tin theo ket qua cua api (dung nhu HIS.Desktop.Plugins.TransDepartment\frmDepartmentTran.cs):
        /// moi phan tu trong SignatureMissingDocuments deu la van ban con thieu chu ky,
        /// ke ca truong hop chi con thieu chu ky cua benh nhan.
        ///
        /// Doi chieu theo ID van ban (chac chan thuoc loai chan nhap vien vi lay tu GetView da loc theo loai),
        /// du phong doi chieu theo DOCUMENT_TYPE_ID cho van ban khong khop ID.
        /// </summary>
        private List<MediRecordCheckingDocumentADO> GetUnfinishedBlockingDocuments(
            MediRecordCheckingResultADO checkingResult,
            List<MediRecordCheckingDocumentADO> blockingDocuments,
            List<EMR_DOCUMENT_TYPE> blockingTypes)
        {
            HashSet<long> blockingDocumentIds = new HashSet<long>(blockingDocuments.Select(o => o.ID));
            HashSet<long> blockingTypeIds = new HashSet<long>(blockingTypes.Select(o => o.ID));

            return (checkingResult.SignatureMissingDocuments ?? new List<MediRecordCheckingDocumentADO>())
                .Where(o => o != null
                    && (blockingDocumentIds.Contains(o.ID)
                        || (o.DOCUMENT_TYPE_ID.HasValue && blockingTypeIds.Contains(o.DOCUMENT_TYPE_ID.Value))))
                .ToList();
        }

        /// <summary>
        /// TH1: benh nhan co chi dinh dich vu nhung chua co van ban nao thuoc loai chan nhap vien.
        /// Chi canh bao Yes/No, khong chan.
        /// </summary>
        /// <returns>true = nguoi dung chon tiep tuc nhap vien.</returns>
        private bool ConfirmHospitalizeWithoutBlockingDocument(
            MediRecordCheckingResultADO checkingResult, List<EMR_DOCUMENT_TYPE> blockingTypes)
        {
            // Uu tien neu ten cac van ban BAT BUOC chua duoc tao (api chi tra ve ten van ban) vi day dung la
            // nhung van ban EMR dang cho; khong co thi liet ke toan bo loai van ban chan nhap vien.
            HashSet<string> blockingTypeNames = new HashSet<string>(blockingTypes
                .Where(o => !string.IsNullOrEmpty(o.DOCUMENT_TYPE_NAME))
                .Select(o => o.DOCUMENT_TYPE_NAME.Trim().ToUpper()));

            List<string> documentTypeNames = (checkingResult.MandatoryMissingDocuments ?? new List<string>())
                .Where(o => !string.IsNullOrWhiteSpace(o))
                .Select(o => o.Trim())
                .Where(o => blockingTypeNames.Contains(o.ToUpper()))
                .Distinct()
                .OrderBy(o => o)
                .ToList();

            if (documentTypeNames.Count == 0)
            {
                documentTypeNames = blockingTypes
                    .Where(o => !string.IsNullOrEmpty(o.DOCUMENT_TYPE_NAME))
                    .Select(o => o.DOCUMENT_TYPE_NAME.Trim())
                    .Distinct()
                    .OrderBy(o => o)
                    .ToList();
            }

            StringBuilder message = new StringBuilder();
            if (documentTypeNames.Count == 0)
            {
                message.Append("Bệnh nhân có chỉ định dịch vụ nhưng chưa có văn bản thuộc loại chặn nhập viện. Bạn có chắc chắn muốn nhập viện không?");
            }
            else
            {
                message.AppendLine("Bệnh nhân có chỉ định dịch vụ nhưng chưa có văn bản thuộc loại chặn nhập viện:");
                message.AppendLine();
                for (int i = 0; i < documentTypeNames.Count; i++)
                {
                    message.AppendLine(string.Format("{0}. {1}", i + 1, documentTypeNames[i]));
                }
                message.AppendLine();
                message.Append("Bạn có chắc chắn muốn nhập viện không?");
            }

            return XtraMessageBox.Show(message.ToString(), ResourceMessage.ThongBao,
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes;
        }

        /// <summary>
        /// Tao cac dong thong bao cho nhung van ban chan nhap vien chua hoan thanh chu ky.
        /// Moi dong neu dich danh LOAI van ban, TEN van ban, MA van ban va ly do tuong ung:
        /// chua duoc ky / chua ky du. Liet ke DAY DU tung van ban de nguoi dung xu ly mot lan.
        /// </summary>
        private List<string> BuildUnfinishedDocumentMessages(
            List<MediRecordCheckingDocumentADO> unfinishedDocuments,
            List<MediRecordCheckingDocumentADO> blockingDocuments,
            List<EMR_DOCUMENT_TYPE> blockingTypes)
        {
            List<string> messages = new List<string>();
            try
            {
                Dictionary<long, EMR_DOCUMENT_TYPE> blockingTypeById = new Dictionary<long, EMR_DOCUMENT_TYPE>();
                foreach (var documentType in blockingTypes)
                {
                    if (!blockingTypeById.ContainsKey(documentType.ID))
                        blockingTypeById.Add(documentType.ID, documentType);
                }

                Dictionary<long, MediRecordCheckingDocumentADO> blockingDocumentById = new Dictionary<long, MediRecordCheckingDocumentADO>();
                foreach (var document in blockingDocuments)
                {
                    if (!blockingDocumentById.ContainsKey(document.ID))
                        blockingDocumentById.Add(document.ID, document);
                }

                // Sap xep theo loai roi den ten van ban de nguoi dung de doi chieu.
                var pendingDocuments = unfinishedDocuments
                    .Select(o => new
                    {
                        Document = o,
                        TypeName = GetBlockingTypeName(ResolveBlockingDocumentType(o, blockingDocumentById, blockingTypeById), o)
                    })
                    .OrderBy(o => o.TypeName)
                    .ThenBy(o => o.Document.DOCUMENT_NAME ?? "")
                    .ThenBy(o => o.Document.DOCUMENT_CODE ?? "")
                    .ToList();

                foreach (var item in pendingDocuments)
                {
                    AddDistinctMessage(messages, string.Format("{0}: {1}.",
                        GetDocumentDisplayName(item.TypeName, item.Document.DOCUMENT_NAME, item.Document.DOCUMENT_CODE),
                        GetUnfinishedReason(item.Document)));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return messages;
        }

        /// <summary>
        /// Loai van ban chan nhap vien cua mot van ban chua hoan thanh.
        /// Ket qua MediRecordChecking co the khong co DOCUMENT_TYPE_ID nen tra cuu bu theo ID van ban.
        /// </summary>
        private EMR_DOCUMENT_TYPE ResolveBlockingDocumentType(
            MediRecordCheckingDocumentADO document,
            Dictionary<long, MediRecordCheckingDocumentADO> blockingDocumentById,
            Dictionary<long, EMR_DOCUMENT_TYPE> blockingTypeById)
        {
            long? documentTypeId = document.DOCUMENT_TYPE_ID;
            if (!documentTypeId.HasValue && blockingDocumentById.ContainsKey(document.ID))
                documentTypeId = blockingDocumentById[document.ID].DOCUMENT_TYPE_ID;

            return documentTypeId.HasValue && blockingTypeById.ContainsKey(documentTypeId.Value)
                ? blockingTypeById[documentTypeId.Value]
                : null;
        }

        private void AddDistinctMessage(List<string> messages, string message)
        {
            if (!messages.Contains(message))
                messages.Add(message);
        }

        /// <summary>
        /// Ten LOAI van ban de hien thi: uu tien lay tu EMR_DOCUMENT_TYPE, du phong lay tu ket qua api.
        /// </summary>
        private string GetBlockingTypeName(EMR_DOCUMENT_TYPE documentType, MediRecordCheckingDocumentADO document)
        {
            if (documentType != null && !string.IsNullOrEmpty(documentType.DOCUMENT_TYPE_NAME))
                return documentType.DOCUMENT_TYPE_NAME.Trim();

            return (document.DOCUMENT_TYPE_NAME ?? "").Trim();
        }

        /// <summary>
        /// Hien thi "Loai van ban - Ten van ban (Ma van ban)".
        /// Bo phan ten van ban neu trong hoac trung ten loai de khong lap chu;
        /// bo phan ma van ban neu khong co.
        /// DOCUMENT_CODE chinh la cot "Ma van ban" tren man EmrDocument / EmrDocumentListAll.
        /// </summary>
        private string GetDocumentDisplayName(string documentTypeName, string documentName, string documentCode)
        {
            documentTypeName = (documentTypeName ?? "").Trim();
            documentName = (documentName ?? "").Trim();
            documentCode = (documentCode ?? "").Trim();

            if (documentTypeName.Length == 0 && documentName.Length == 0)
                return documentCode.Length > 0 ? documentCode : "(không xác định)";

            string display;
            if (documentTypeName.Length == 0)
                display = documentName;
            else if (documentName.Length == 0
                || string.Equals(documentName, documentTypeName, StringComparison.CurrentCultureIgnoreCase))
                display = documentTypeName;
            else
                display = string.Format("{0} - {1}", documentTypeName, documentName);

            return documentCode.Length > 0
                ? string.Format("{0} ({1})", display, documentCode)
                : display;
        }

        /// <summary>
        /// Ly do van ban chua hoan thanh:
        /// TH2 - chua co ai ky -&gt; "chưa được ký"; TH3 - da co nguoi ky nhung con thieu -&gt; "chưa ký đủ".
        /// </summary>
        private string GetUnfinishedReason(MediRecordCheckingDocumentADO document)
        {
            return string.IsNullOrWhiteSpace(document.SIGNERS) ? "chưa được ký" : "chưa ký đủ";
        }
    }
}
