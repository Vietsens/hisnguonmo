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
    /// api "api/EmrDocument/MediRecordChecking" tra ve EMR.SDO.MediRecordCheckingResultSDO, trong do
    /// SignatureMissingDocuments la List&lt;EMR.EFMODEL.DataModels.V_EMR_DOCUMENT&gt;; api
    /// "api/EmrDocument/GetView" cung tra ve List&lt;V_EMR_DOCUMENT&gt;. Ban EMR.EFMODEL.dll
    /// hien tai trong lib\EMR khong con chua cac kieu view V_* nen khong tham chieu truc tiep EMR.SDO duoc
    /// (loi CS0570). Vi vay dung ADO cuc bo MediRecordCheckingResultADO cho ca hai api - ban tin la JSON
    /// nen chi can trung ten thuoc tinh la Newtonsoft map duoc.
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
        /// Tien to log de tim nhanh trong LogSystem.txt khi kiem tra khong chan nhap vien nhu mong doi.
        /// Moi diem thoat som deu ghi log kem gia tri thuc te vi tat ca cac nhanh do deu tra ve true
        /// (cho phep luu) - khong co log thi khong biet dut o dau.
        /// </summary>
        private const string LOG__CHECK_DOCUMENT_HOSPITALIZE = "CheckEmrDocumentBeforeHospitalize: ";

        /// <summary>
        /// Dau nhan de doi chieu DLL dang chay voi ban vua build. Doi moi lan sua logic phep kiem tra nay.
        /// </summary>
        private const string STAMP__CHECK_DOCUMENT_HOSPITALIZE = "3-truong-hop-v1";

        /// <summary>
        /// Ghi mot dong dien bien cua phep kiem tra.
        ///
        /// PHAI ghi o muc Error: appender LogSystem trong HIS.exe.config dat
        /// &lt;param name="Threshold" value="ERROR"/&gt; nen log4net loai bo sach moi ban tin duoi Error -
        /// Debug/Info/Warn KHONG bao gio ra duoc file Logs\LogSystem.txt. Toan bo phep kiem tra nay chi
        /// thoat som va tra ve true (cho phep luu), khong co log thi khong biet dut o dau.
        ///
        /// Muon ha xuong Debug thi sua dung mot cho nay (va doi Threshold trong HIS.exe.config thanh DEBUG).
        /// </summary>
        private void TraceCheckDocumentHospitalize(string message)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Error(LOG__CHECK_DOCUMENT_HOSPITALIZE + message);
            }
            catch
            {
                // Ghi log that bai thi khong duoc anh huong nghiep vu
            }
        }

        /// <summary>
        /// Kiem tra van ban thuoc loai chan nhap vien truoc khi cho phep nhap vien.
        /// </summary>
        /// <returns>true = duoc phep luu; false = chan luu.</returns>
        private bool CheckEmrDocumentBeforeHospitalize()
        {
            try
            {
                // Dau nhan phien ban: khong thay dong "BAT DAU ..." kem dau nhan nay trong Logs\LogSystem.txt
                // thi DLL dang chay KHONG phai ban vua build (xem lai thu muc Plugins cua ban chay).
                TraceCheckDocumentHospitalize(string.Format(
                    "BAT DAU [" + STAMP__CHECK_DOCUMENT_HOSPITALIZE + "]. chkHospitalize = {0}, ma dieu tri = {1}, TREATMENT_ID = {2}, SERVICE_REQ_ID = {3}, RoomId = {4}.",
                    chkHospitalize == null ? "(null control)" : chkHospitalize.Checked.ToString(),
                    this.HisServiceReqView != null ? this.HisServiceReqView.TDL_TREATMENT_CODE : "(HisServiceReqView null)",
                    this.HisServiceReqView != null ? this.HisServiceReqView.TREATMENT_ID.ToString() : "(HisServiceReqView null)",
                    this.HisServiceReqView != null ? this.HisServiceReqView.ID.ToString() : "(HisServiceReqView null)",
                    this.moduleData != null ? this.moduleData.RoomId.ToString() : "(moduleData null)"));

                if (chkHospitalize == null || !chkHospitalize.Checked)
                {
                    TraceCheckDocumentHospitalize("KET LUAN: khong tich nhap vien nen khong can kiem tra van ban -> cho phep luu.");
                    return true;
                }

                if (!IsCurrentDepartmentCheckDocumentHospitalize())
                {
                    TraceCheckDocumentHospitalize("KET LUAN: khoa hien tai khong duoc cau hinh kiem tra van ban -> cho phep luu.");
                    return true;
                }

                string treatmentCode = this.HisServiceReqView != null ? this.HisServiceReqView.TDL_TREATMENT_CODE : null;
                if (string.IsNullOrEmpty(treatmentCode))
                {
                    TraceCheckDocumentHospitalize("KET LUAN: TDL_TREATMENT_CODE rong -> cho phep luu.");
                    return true;
                }

                // Van ban cua y lenh Kham khong can ky nen y lenh Kham khong tinh la "co chi dinh dich vu".
                // Benh nhan chua co y lenh nao ngoai Kham -> khong kiem tra van ban.
                if (!HasServiceReqExceptExam(this.HisServiceReqView.TREATMENT_ID))
                {
                    TraceCheckDocumentHospitalize("KET LUAN: khong co chi dinh dich vu nao ngoai Kham -> cho phep luu.");
                    return true;
                }

                List<EMR_DOCUMENT_TYPE> blockingTypes = GetHospitalizationBlockingDocumentTypes();
                if (blockingTypes == null || blockingTypes.Count == 0)
                {
                    TraceCheckDocumentHospitalize("KET LUAN: khong co loai van ban nao co IS_HOSPITALIZATION = 1 "
                        + "(vao man Danh muc loai van ban EMR tich \"Chan nhap vien\" cho loai van ban can chan) -> cho phep luu.");
                    return true;
                }

                // Goi MediRecordChecking TRUOC khi lay danh sach van ban de biet dich vu EMR con song:
                // api nay loi thi bo qua kiem tra luon, tranh canh bao oan o TH1 (khong co van ban).
                MediRecordCheckingResultADO checkingResult = GetMediRecordChecking(treatmentCode);
                if (checkingResult == null)
                {
                    TraceCheckDocumentHospitalize("KET LUAN: api " + URI__EMR_DOCUMENT_MEDI_RECORD_CHECKING
                        + " tra ve null (ma dieu tri " + treatmentCode + ") -> cho phep luu.");
                    return true;
                }

                // TH1: chua co bat ky van ban nao thuoc loai chan nhap vien -> canh bao, nguoi dung tu quyet dinh.
                List<MediRecordCheckingDocumentADO> blockingDocuments = GetBlockingDocumentsOfTreatment(treatmentCode, blockingTypes);
                if (blockingDocuments.Count == 0)
                    return ConfirmHospitalizeWithoutBlockingDocument(treatmentCode, checkingResult, blockingTypes);

                // TH2 + TH3: da co van ban thuoc loai chan nhap vien nhung chua duoc ky / chua ky du -> chan.
                List<MediRecordCheckingDocumentADO> unfinishedDocuments =
                    GetUnfinishedBlockingDocuments(checkingResult, blockingDocuments, blockingTypes);
                if (unfinishedDocuments.Count == 0)
                {
                    TraceCheckDocumentHospitalize(string.Format(
                        "KET LUAN: ho so {0} co {1} van ban thuoc loai chan nhap vien va da ky day du -> cho phep luu.",
                        treatmentCode, blockingDocuments.Count));
                    return true;
                }

                List<string> messages = BuildUnfinishedDocumentMessages(unfinishedDocuments, blockingDocuments, blockingTypes);
                if (messages.Count == 0)
                {
                    TraceCheckDocumentHospitalize(string.Format(
                        "KET LUAN: ho so {0} co {1} van ban chan nhap vien chua ky du nhung khong dung duoc dong thong bao nao -> cho phep luu. Van ban: [{2}].",
                        treatmentCode,
                        unfinishedDocuments.Count,
                        string.Join(", ", unfinishedDocuments.Select(o => string.Format("{0}#{1}", o.ID, o.DOCUMENT_CODE)))));
                    return true;
                }

                TraceCheckDocumentHospitalize(string.Format(
                    "KET LUAN: CHAN nhap vien ho so {0} - {1}/{2} van ban thuoc loai chan nhap vien chua hoan thanh chu ky: [{3}].",
                    treatmentCode, unfinishedDocuments.Count, blockingDocuments.Count, string.Join(" | ", messages)));

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
                    TraceCheckDocumentHospitalize("key cau hinh " + HisConfigCFG.KEY_CheckDepaDocumentHospitalization
                        + " chua khai bao (hoac de trong) nen KHONG kiem tra van ban. Khai bao danh sach DEPARTMENT_CODE, phan tach boi \"|\".");
                    return false;
                }

                string currentDepartmentCode = GetCurrentDepartmentCode();
                if (string.IsNullOrEmpty(currentDepartmentCode))
                {
                    TraceCheckDocumentHospitalize("khong xac dinh duoc khoa cua phong lam viec nen KHONG kiem tra van ban. Danh sach cau hinh: ["
                        + string.Join("|", departmentCodes) + "].");
                    return false;
                }

                bool isCheck = departmentCodes.Contains(currentDepartmentCode.Trim().ToUpper());
                TraceCheckDocumentHospitalize(string.Format(
                    "khoa cua phong lam viec [{0}] {1} danh sach cau hinh [{2}] cua key {3}.",
                    currentDepartmentCode.Trim().ToUpper(),
                    isCheck ? "NAM TRONG" : "KHONG nam trong",
                    string.Join("|", departmentCodes),
                    HisConfigCFG.KEY_CheckDepaDocumentHospitalization));
                return isCheck;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(LOG__CHECK_DOCUMENT_HOSPITALIZE + "loi khi doi chieu khoa cau hinh.", ex);
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
                    TraceCheckDocumentHospitalize("moduleData null, khong lay duoc khoa hien tai.");
                    return null;
                }

                var workPlace = HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkPlaceSDO
                    .FirstOrDefault(o => o.RoomId == this.moduleData.RoomId);
                if (workPlace == null || workPlace.DepartmentId <= 0)
                {
                    TraceCheckDocumentHospitalize("khong tim thay WorkPlaceSDO (hoac DepartmentId <= 0) cua phong lam viec RoomId = "
                        + this.moduleData.RoomId + ".");
                    return null;
                }

                var department = BackendDataWorker.Get<HIS_DEPARTMENT>()
                    .FirstOrDefault(o => o.ID == workPlace.DepartmentId);
                if (department == null)
                {
                    TraceCheckDocumentHospitalize("khong tim thay HIS_DEPARTMENT co ID = " + workPlace.DepartmentId + " trong BackendData.");
                    return null;
                }
                return department.DEPARTMENT_CODE;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(LOG__CHECK_DOCUMENT_HOSPITALIZE + "loi khi lay khoa cua phong lam viec.", ex);
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
                    TraceCheckDocumentHospitalize("TREATMENT_ID = " + treatmentId + " khong hop le, bo qua kiem tra van ban.");
                    return false;
                }

                CommonParam param = new CommonParam();
                MOS.Filter.HisServiceReqFilter filter = new MOS.Filter.HisServiceReqFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                filter.TREATMENT_ID = treatmentId;
                filter.NOT_IN_SERVICE_REQ_TYPE_IDs = new List<long> { IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH };

                var serviceReqs = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>(
                    URI__HIS_SERVICE_REQ_GET, ApiConsumers.MosConsumer, filter, param);
                int count = serviceReqs != null ? serviceReqs.Count : 0;
                if (count == 0)
                {
                    TraceCheckDocumentHospitalize("api " + URI__HIS_SERVICE_REQ_GET + " (TREATMENT_ID = " + treatmentId
                        + ", NOT_IN_SERVICE_REQ_TYPE_IDs = [" + IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH
                        + "]) tra ve " + (serviceReqs == null ? "null" : "rong") + ": khong co y lenh nao ngoai Kham.");
                    return false;
                }

                TraceCheckDocumentHospitalize(string.Format(
                    "dot dieu tri TREATMENT_ID = {0} co {1} y lenh khac Kham (SERVICE_REQ_TYPE_ID: [{2}]) -> tien hanh kiem tra van ban.",
                    treatmentId,
                    count,
                    string.Join(", ", serviceReqs.Select(o => o.SERVICE_REQ_TYPE_ID).Distinct())));
                return true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(LOG__CHECK_DOCUMENT_HOSPITALIZE + "loi khi goi api "
                    + URI__HIS_SERVICE_REQ_GET + " (TREATMENT_ID = " + treatmentId + ") nen KHONG kiem tra van ban.", ex);
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
                if (checkingResult != null)
                {
                    TraceCheckDocumentHospitalize(string.Format(
                        "api {0} ho so {1}: {2} van ban chua ky du [{3}], {4} van ban bat buoc chua tao [{5}].",
                        URI__EMR_DOCUMENT_MEDI_RECORD_CHECKING,
                        treatmentCode,
                        checkingResult.SignatureMissingDocuments != null ? checkingResult.SignatureMissingDocuments.Count : 0,
                        checkingResult.SignatureMissingDocuments == null ? "" : string.Join(", ", checkingResult.SignatureMissingDocuments
                            .Where(o => o != null).Select(o => string.Format("{0}#{1}#loai {2}", o.ID, o.DOCUMENT_CODE, o.DOCUMENT_TYPE_ID))),
                        checkingResult.MandatoryMissingDocuments != null ? checkingResult.MandatoryMissingDocuments.Count : 0,
                        checkingResult.MandatoryMissingDocuments == null ? "" : string.Join(", ", checkingResult.MandatoryMissingDocuments)));
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
                    TraceCheckDocumentHospitalize("api " + URI__EMR_DOCUMENT_TYPE_GET + " tra ve "
                        + (documentTypes == null ? "null" : "rong") + ", khong xac dinh duoc loai van ban chan nhap vien.");
                    return null;
                }

                List<EMR_DOCUMENT_TYPE> blockingTypes = documentTypes.Where(o => o != null && o.IS_HOSPITALIZATION == 1).ToList();
                TraceCheckDocumentHospitalize(string.Format(
                    "co {0}/{1} loai van ban dang hoat dong co IS_HOSPITALIZATION = 1: [{2}].",
                    blockingTypes.Count,
                    documentTypes.Count,
                    string.Join(", ", blockingTypes.Select(o => string.Format("{0}#{1}#{2}", o.ID, o.DOCUMENT_TYPE_CODE, o.DOCUMENT_TYPE_NAME)))));
                return blockingTypes;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(LOG__CHECK_DOCUMENT_HOSPITALIZE + "loi khi lay loai van ban chan nhap vien.", ex);
                return null;
            }
        }

        /// <summary>
        /// Cac van ban (chua bi xoa) cua dot dieu tri thuoc loai chan nhap vien.
        ///
        /// Lay toan bo van ban cua dot dieu tri roi loc theo loai tai client, KHONG truyen
        /// filter.DOCUMENT_TYPE_IDs: filter cua BackendAdapter.Get di qua query string nen truyen
        /// danh sach ID vao day la them mot duong tra ve null im lang - ma o TH1 diem goi buoc phai
        /// phan biet duoc "khong co van ban" voi "goi api that bai".
        ///
        /// Tra ve list rong (khong bao gio null) de diem goi chi con mot nhanh TH1 duy nhat:
        /// khong co van ban nao thuoc loai chan nhap vien.
        /// </summary>
        private List<MediRecordCheckingDocumentADO> GetBlockingDocumentsOfTreatment(string treatmentCode, List<EMR_DOCUMENT_TYPE> blockingTypes)
        {
            List<MediRecordCheckingDocumentADO> blockingDocuments = new List<MediRecordCheckingDocumentADO>();
            try
            {
                CommonParam param = new CommonParam();
                EmrDocumentViewFilter filter = new EmrDocumentViewFilter();
                filter.TREATMENT_CODE__EXACT = treatmentCode;
                filter.IS_DELETE = false;

                var documents = new BackendAdapter(param).Get<List<MediRecordCheckingDocumentADO>>(
                    URI__EMR_DOCUMENT_GET_VIEW, ApiConsumers.EmrConsumer, filter, param);
                if (documents == null || documents.Count == 0)
                {
                    TraceCheckDocumentHospitalize("api " + URI__EMR_DOCUMENT_GET_VIEW + " tra ve "
                        + (documents == null ? "null" : "rong") + " (ma dieu tri " + treatmentCode + "): ho so chua co van ban nao.");
                    return blockingDocuments;
                }

                HashSet<long> blockingTypeIds = new HashSet<long>(blockingTypes.Select(o => o.ID));
                blockingDocuments = documents
                    .Where(o => o != null && o.DOCUMENT_TYPE_ID.HasValue && blockingTypeIds.Contains(o.DOCUMENT_TYPE_ID.Value))
                    .ToList();

                // Liet ke DAY DU van ban cua ho so kem loai: khi ket qua ra 0 van ban chan nhap vien thi
                // day la cho duy nhat doi chieu duoc "loai van ban nao dang co" voi "loai nao duoc tich chan".
                TraceCheckDocumentHospitalize(string.Format(
                    "ho so {0} co {1}/{2} van ban thuoc loai chan nhap vien. Van ban chan nhap vien: [{3}]. Toan bo van ban cua ho so: [{4}].",
                    treatmentCode,
                    blockingDocuments.Count,
                    documents.Count,
                    string.Join(", ", blockingDocuments.Select(o => string.Format("{0}#{1}", o.ID, o.DOCUMENT_CODE))),
                    string.Join(", ", documents.Where(o => o != null).Select(o => string.Format(
                        "{0}#{1}#loai {2}#{3}", o.ID, o.DOCUMENT_CODE, o.DOCUMENT_TYPE_ID, o.DOCUMENT_TYPE_CODE)))));
                return blockingDocuments;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(LOG__CHECK_DOCUMENT_HOSPITALIZE + "loi khi goi api "
                    + URI__EMR_DOCUMENT_GET_VIEW + " (ma dieu tri " + treatmentCode + ").", ex);
                return blockingDocuments;
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
            string treatmentCode, MediRecordCheckingResultADO checkingResult, List<EMR_DOCUMENT_TYPE> blockingTypes)
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

            TraceCheckDocumentHospitalize(string.Format(
                "KET LUAN: CANH BAO ho so {0} - co chi dinh dich vu nhung chua co van ban thuoc loai chan nhap vien. Loai can co: [{1}].",
                treatmentCode,
                string.Join(", ", documentTypeNames)));

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

            bool isContinue = XtraMessageBox.Show(message.ToString(), ResourceMessage.ThongBao,
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes;
            TraceCheckDocumentHospitalize("nguoi dung chon " + (isContinue ? "TIEP TUC" : "KHONG")
                + " nhap vien cho ho so " + treatmentCode + ".");
            return isContinue;
        }

        /// <summary>
        /// Tao cac dong thong bao cho nhung van ban chan nhap vien chua hoan thanh chu ky.
        /// Moi dong neu dich danh LOAI van ban, TEN van ban, MA van ban va ly do tuong ung:
        /// chua duoc ky (TH2) / chua ky du (TH3).
        /// Liet ke DAY DU tung van ban de nguoi dung xu ly mot lan.
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
