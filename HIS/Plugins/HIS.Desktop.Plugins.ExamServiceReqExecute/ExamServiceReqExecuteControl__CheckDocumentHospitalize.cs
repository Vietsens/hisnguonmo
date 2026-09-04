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
    /// Chan nhap vien khi ho so con van ban chua hoan thanh thuoc loai van ban co IS_HOSPITALIZATION = 1.
    ///
    /// Dieu kien kich hoat (phai thoa man ca 3):
    ///  - chkHospitalize duoc tich (xu tri nhap vien);
    ///  - key HIS.Desktop.Plugins.ExamServiceReqExecute.CheckDepaDocument.Hospitalization co gia tri
    ///    (danh sach ma khoa, phan tach boi "|");
    ///  - ma khoa hien tai (khoa cua phong lam viec) nam trong danh sach ma khoa cua key do.
    ///
    /// Ghi chu ve kieu du lieu:
    /// api "api/EmrDocument/MediRecordChecking" tra ve EMR.SDO.MediRecordCheckingResultSDO, trong do
    /// SignatureMissingDocuments la List&lt;EMR.EFMODEL.DataModels.V_EMR_DOCUMENT&gt;. Ban EMR.EFMODEL.dll
    /// hien tai trong lib\EMR khong con chua cac kieu view V_* nen khong tham chieu truc tiep EMR.SDO duoc
    /// (loi CS0570). Vi vay dung ADO cuc bo MediRecordCheckingResultADO - ban tin la JSON nen chi can
    /// trung ten thuoc tinh la Newtonsoft map duoc.
    ///
    /// V_EMR_DOCUMENT khong co cot IS_HOSPITALIZATION nen phai tra cuu them EMR_DOCUMENT_TYPE
    /// de biet loai van ban nao chan nhap vien.
    /// </summary>
    public partial class ExamServiceReqExecuteControl
    {
        private const string URI__EMR_DOCUMENT_MEDI_RECORD_CHECKING = "api/EmrDocument/MediRecordChecking";
        private const string URI__EMR_DOCUMENT_TYPE_GET = "api/EmrDocumentType/Get";

        /// <summary>
        /// Tien to log de tim nhanh trong LogSystem.txt khi kiem tra khong chan nhap vien nhu mong doi.
        /// Moi diem thoat som deu ghi log kem gia tri thuc te vi tat ca cac nhanh do deu tra ve true
        /// (cho phep luu) - khong co log thi khong biet dut o dau.
        /// </summary>
        private const string LOG__CHECK_DOCUMENT_HOSPITALIZE = "CheckEmrDocumentBeforeHospitalize: ";

        /// <summary>
        /// Kiem tra van ban chua hoan thanh truoc khi cho phep nhap vien.
        /// </summary>
        /// <returns>true = duoc phep luu; false = con van ban chua hoan thanh, chan luu.</returns>
        private bool CheckEmrDocumentBeforeHospitalize()
        {
            try
            {
                if (chkHospitalize == null || !chkHospitalize.Checked)
                {
                    Inventec.Common.Logging.LogSystem.Debug(LOG__CHECK_DOCUMENT_HOSPITALIZE + "khong tich nhap vien, khong can kiem tra van ban.");
                    return true;
                }

                if (!IsCurrentDepartmentCheckDocumentHospitalize())
                    return true;

                string treatmentCode = this.HisServiceReqView != null ? this.HisServiceReqView.TDL_TREATMENT_CODE : null;
                if (string.IsNullOrEmpty(treatmentCode))
                {
                    Inventec.Common.Logging.LogSystem.Warn(LOG__CHECK_DOCUMENT_HOSPITALIZE + "TDL_TREATMENT_CODE rong, bo qua kiem tra van ban.");
                    return true;
                }

                MediRecordCheckingResultADO checkingResult = GetMediRecordChecking(treatmentCode);
                if (checkingResult == null)
                {
                    Inventec.Common.Logging.LogSystem.Warn(LOG__CHECK_DOCUMENT_HOSPITALIZE + "api " + URI__EMR_DOCUMENT_MEDI_RECORD_CHECKING
                        + " tra ve null (ma dieu tri " + treatmentCode + "), bo qua kiem tra van ban.");
                    return true;
                }

                // Van ban da tao nhung chua hoan thanh chu ky.
                // Tin theo ket qua cua api (dung nhu HIS.Desktop.Plugins.TransDepartment\frmDepartmentTran.cs):
                // moi phan tu trong SignatureMissingDocuments deu la van ban con thieu chu ky,
                // ke ca truong hop chi con thieu chu ky cua benh nhan.
                List<MediRecordCheckingDocumentADO> unsignedDocuments = (checkingResult.SignatureMissingDocuments ?? new List<MediRecordCheckingDocumentADO>())
                    .Where(o => o != null)
                    .ToList();

                // Van ban bat buoc nhung chua duoc tao (api chi tra ve ten van ban)  
                List<string> mandatoryMissingNames = (checkingResult.MandatoryMissingDocuments ?? new List<string>())
                    .Where(o => !string.IsNullOrWhiteSpace(o))
                    .Select(o => o.Trim())
                    .ToList();

                if (unsignedDocuments.Count == 0 && mandatoryMissingNames.Count == 0)
                {
                    Inventec.Common.Logging.LogSystem.Debug(LOG__CHECK_DOCUMENT_HOSPITALIZE + "ho so " + treatmentCode
                        + " khong con van ban chua hoan thanh, cho phep nhap vien.");
                    return true;
                }

                List<EMR_DOCUMENT_TYPE> blockingTypes = GetHospitalizationBlockingDocumentTypes();
                if (blockingTypes == null || blockingTypes.Count == 0)
                {
                    Inventec.Common.Logging.LogSystem.Warn(LOG__CHECK_DOCUMENT_HOSPITALIZE
                        + "khong lay duoc loai van ban nao co IS_HOSPITALIZATION = 1 (kiem tra lai cau hinh loai van ban tren man HIS.Desktop.Plugins.EmrDocumentType), bo qua kiem tra van ban.");
                    return true;
                }

                List<string> messages = BuildBlockingMessages(unsignedDocuments, mandatoryMissingNames, blockingTypes);
                if (messages.Count == 0)
                {
                    // Ho so con van ban chua hoan thanh nhung khong van ban nao thuoc loai chan nhap vien.
                    Inventec.Common.Logging.LogSystem.Warn(string.Format(
                        LOG__CHECK_DOCUMENT_HOSPITALIZE + "ho so {0} con {1} van ban chua ky du va {2} van ban bat buoc chua tao, "
                        + "nhung khong van ban nao thuoc {3} loai van ban co IS_HOSPITALIZATION = 1. Loai chan nhap vien: [{4}]. Loai van ban chua ky du: [{5}].",
                        treatmentCode,
                        unsignedDocuments.Count,
                        mandatoryMissingNames.Count,
                        blockingTypes.Count,
                        string.Join(", ", blockingTypes.Select(o => string.Format("{0}#{1}", o.ID, o.DOCUMENT_TYPE_CODE))),
                        string.Join(", ", unsignedDocuments.Select(o => string.Format("{0}#{1}", o.DOCUMENT_TYPE_ID, o.DOCUMENT_TYPE_CODE)).Distinct())));
                    return true;
                }

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
                Inventec.Common.Logging.LogSystem.Error(LOG__CHECK_DOCUMENT_HOSPITALIZE + "loi khi kiem tra van ban nen KHONG chan nhap vien.", ex);
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
                {
                    Inventec.Common.Logging.LogSystem.Warn(LOG__CHECK_DOCUMENT_HOSPITALIZE + "key cau hinh "
                        + HisConfigCFG.KEY_CheckDepaDocumentHospitalization
                        + " chua khai bao (hoac de trong) nen KHONG kiem tra van ban. Khai bao danh sach DEPARTMENT_CODE, phan tach boi \"|\".");
                    return false;
                }

                string currentDepartmentCode = GetCurrentDepartmentCode();
                if (string.IsNullOrEmpty(currentDepartmentCode))
                {
                    Inventec.Common.Logging.LogSystem.Warn(LOG__CHECK_DOCUMENT_HOSPITALIZE
                        + "khong xac dinh duoc khoa cua phong lam viec nen KHONG kiem tra van ban.");
                    return false;
                }

                bool isCheck = departmentCodes.Contains(currentDepartmentCode.Trim().ToUpper());
                if (!isCheck)
                {
                    Inventec.Common.Logging.LogSystem.Warn(string.Format(
                        LOG__CHECK_DOCUMENT_HOSPITALIZE + "khoa cua phong lam viec [{0}] khong nam trong danh sach cau hinh [{1}] cua key {2} nen KHONG kiem tra van ban.",
                        currentDepartmentCode.Trim().ToUpper(),
                        string.Join("|", departmentCodes),
                        HisConfigCFG.KEY_CheckDepaDocumentHospitalization));
                }
                return isCheck;
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
                {
                    Inventec.Common.Logging.LogSystem.Warn(LOG__CHECK_DOCUMENT_HOSPITALIZE + "moduleData null, khong lay duoc khoa hien tai.");
                    return null;
                }

                var workPlace = HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkPlaceSDO
                    .FirstOrDefault(o => o.RoomId == this.moduleData.RoomId);
                if (workPlace == null || workPlace.DepartmentId <= 0)
                {
                    Inventec.Common.Logging.LogSystem.Warn(LOG__CHECK_DOCUMENT_HOSPITALIZE
                        + "khong tim thay WorkPlaceSDO (hoac DepartmentId <= 0) cua phong lam viec RoomId = " + this.moduleData.RoomId + ".");
                    return null;
                }

                var department = BackendDataWorker.Get<HIS_DEPARTMENT>()
                    .FirstOrDefault(o => o.ID == workPlace.DepartmentId);
                if (department == null)
                {
                    Inventec.Common.Logging.LogSystem.Warn(LOG__CHECK_DOCUMENT_HOSPITALIZE
                        + "khong tim thay HIS_DEPARTMENT co ID = " + workPlace.DepartmentId + " trong BackendData.");
                    return null;
                }
                return department.DEPARTMENT_CODE;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
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
                if (checkingResult != null)
                {
                    Inventec.Common.Logging.LogSystem.Debug(string.Format(
                        LOG__CHECK_DOCUMENT_HOSPITALIZE + "ho so {0}: {1} van ban chua ky du, {2} van ban bat buoc chua tao.",
                        treatmentCode,
                        checkingResult.SignatureMissingDocuments != null ? checkingResult.SignatureMissingDocuments.Count : 0,
                        checkingResult.MandatoryMissingDocuments != null ? checkingResult.MandatoryMissingDocuments.Count : 0));
                }
                return checkingResult;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(LOG__CHECK_DOCUMENT_HOSPITALIZE + "loi khi goi api "
                    + URI__EMR_DOCUMENT_MEDI_RECORD_CHECKING + " (ma dieu tri " + treatmentCode + ").", ex);
                return null;
            }
        }

        /// <summary>
        /// Lay cac loai van ban chan nhap vien (IS_HOSPITALIZATION = 1).
        ///
        /// Lay toan bo loai van ban dang hoat dong roi loc tai client (giong cac plugin EMR khac,
        /// vi du EMR.Desktop.Plugins.EmrDocumentList) thay vi loc theo filter.IDs:
        ///  - van ban bat buoc chua tao chi duoc api tra ve TEN, khong co DOCUMENT_TYPE_ID de loc;
        ///  - filter cua BackendAdapter.Get di qua query string, truyen danh sach ID vao day la them
        ///    mot duong tra ve null im lang ma diem goi khong phan biet duoc voi "khong co loai nao".
        /// Bang EMR_DOCUMENT_TYPE chi vai chuc dong nen lay het khong dang ke.
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
                {
                    Inventec.Common.Logging.LogSystem.Warn(LOG__CHECK_DOCUMENT_HOSPITALIZE + "api " + URI__EMR_DOCUMENT_TYPE_GET
                        + " tra ve rong, khong xac dinh duoc loai van ban chan nhap vien.");
                    return null;
                }

                List<EMR_DOCUMENT_TYPE> blockingTypes = documentTypes.Where(o => o != null && o.IS_HOSPITALIZATION == 1).ToList();
                Inventec.Common.Logging.LogSystem.Debug(string.Format(
                    LOG__CHECK_DOCUMENT_HOSPITALIZE + "co {0}/{1} loai van ban dang hoat dong co IS_HOSPITALIZATION = 1: [{2}].",
                    blockingTypes.Count,
                    documentTypes.Count,
                    string.Join(", ", blockingTypes.Select(o => string.Format("{0}#{1}", o.ID, o.DOCUMENT_TYPE_CODE)))));
                return blockingTypes;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(LOG__CHECK_DOCUMENT_HOSPITALIZE + "loi khi lay loai van ban chan nhap vien.", ex);
                return null;
            }
        }

        /// <summary>
        /// Tao cac dong thong bao cho nhung van ban chua hoan thanh thuoc loai chan nhap vien.
        /// Moi dong neu dich danh LOAI van ban, TEN van ban, MA van ban va ly do tuong ung:
        /// chua duoc tao / chua duoc ky / chua ky du.
        /// Liet ke DAY DU tung van ban de nguoi dung xu ly mot lan;
        /// cac van ban chua duoc tao dat len truoc vi phai tao xong moi ky duoc.
        /// </summary>
        private List<string> BuildBlockingMessages(
            List<MediRecordCheckingDocumentADO> unsignedDocuments,
            List<string> mandatoryMissingNames,
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
                HashSet<string> blockingTypeNames = new HashSet<string>(blockingTypes
                    .Where(o => !string.IsNullOrEmpty(o.DOCUMENT_TYPE_NAME))
                    .Select(o => o.DOCUMENT_TYPE_NAME.Trim().ToUpper()));

                // 1. Van ban bat buoc nhung chua duoc tao
                foreach (string documentTypeName in mandatoryMissingNames)
                {
                    if (!blockingTypeNames.Contains(documentTypeName.ToUpper()))
                        continue;

                    AddDistinctMessage(messages, string.Format("{0}: chưa được tạo.", documentTypeName));
                }

                // 2. Van ban da tao nhung chua hoan thanh chu ky - liet ke DAY DU tung van ban,
                //    sap xep theo loai roi den ten van ban de nguoi dung de doi chieu.
                var pendingDocuments = unsignedDocuments
                    .Where(o => o.DOCUMENT_TYPE_ID.HasValue && blockingTypeById.ContainsKey(o.DOCUMENT_TYPE_ID.Value))
                    .Select(o => new
                    {
                        Document = o,
                        TypeName = GetBlockingTypeName(blockingTypeById[o.DOCUMENT_TYPE_ID.Value], o)
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
        /// bo phan ma van ban neu khong co (vi du van ban chua duoc tao).
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
        /// chua co ai ky -&gt; "chưa được ký"; da co nguoi ky nhung con thieu -&gt; "chưa ký đủ".
        /// </summary>
        private string GetUnfinishedReason(MediRecordCheckingDocumentADO document)
        {
            return string.IsNullOrWhiteSpace(document.SIGNERS) ? "chưa được ký" : "chưa ký đủ";
        }
    }
}
