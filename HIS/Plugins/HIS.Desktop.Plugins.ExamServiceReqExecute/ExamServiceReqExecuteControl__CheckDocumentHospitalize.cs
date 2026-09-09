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
    /// Kiem tra van ban EMR truoc khi xu tri nhap vien.
    ///
    /// Dieu kien kich hoat (phai thoa man ca 3):
    ///  - chkHospitalize duoc tich (xu tri nhap vien);
    ///  - key HIS.Desktop.Plugins.ExamServiceReqExecute.CheckDepaDocument.Hospitalization co gia tri
    ///    (danh sach ma khoa, phan tach boi "|");
    ///  - ma khoa hien tai (khoa cua phong lam viec) nam trong danh sach ma khoa cua key do.
    ///
    /// Kiem tra hai buoc, CA HAI buoc deu chay roi moi thong bao MOT lan (moi buoc mot muc trong
    /// cung hop thoai) de nguoi dung thay het viec phai lam, khong bi sua xong buoc 1 lai chan tiep buoc 2:
    ///
    /// BUOC 1 - theo TUNG y lenh chi dinh (bo qua y lenh Kham va cac loai DON vi khong can van ban ky):
    ///  - moi y lenh chi dinh phai co van ban di kem, doi chieu qua
    ///    EMR_DOCUMENT.HIS_CODE co chua "SERVICE_REQ_CODE:&lt;ma y lenh&gt;";
    ///  - y lenh chua co van ban -> CHAN;
    ///  - y lenh co van ban nhung van ban chua hoan thanh chu ky -> CHAN.
    ///
    /// BUOC 2 - theo TUNG LOAI van ban duoc tich "chan nhap vien" (IS_HOSPITALIZATION = 1):
    /// MOI loai phai co IT NHAT MOT van ban da hoan thanh moi cho nhap vien. Co 5 loai duoc tich thi
    /// thieu bat ky mot loai nao cung CHAN, du 4 loai con lai da du. Voi tung loai:
    ///  - loai do chua co van ban nao -> CHAN;
    ///  - loai do co van ban nhung con van ban chua hoan thanh -> CHAN.
    /// Rieng loai "Phieu chi dinh" (EMR_DOCUMENT_TYPE.ID__SERVICE_ASSIGN) bi bo qua khi ho so khong co
    /// y lenh chi dinh nao, vi van ban do di kem y lenh - khong co y lenh thi khong the tao duoc.
    ///
    /// MIEN TRU cho nut ky tren UC.Hospitalize: neu buoc 1 sach va buoc 2 chi con DUNG MOT loai chua dat
    /// la "Phieu kham benh vao vien" theo kieu CHUA CO VAN BAN NAO, ma nguoi dung dang tich nut ky, thi
    /// cho phep luu - van ban se duoc tao va ky ngay sau khi luu (thu tu thuc te: kiem tra -> luu -> ky).
    /// Khong mien khi loai do da co van ban nhung con cai chua hoan thanh, vi viec ky chi tao va ky MOT
    /// van ban, khong lam xong duoc van ban cung loai dang do dang san.
    /// Xet TOAN BO van ban cua ho so, KHONG bo qua van ban nao - ke ca van ban dang gan vao y lenh
    /// va da duoc buoc 1 neu ten.
    ///
    /// "Chua hoan thanh" = thoa BAT KY dieu nao: api MediRecordChecking bao thieu chu ky, hoac con
    /// NEXT_SIGNER, hoac SIGNERS rong, hoac con REJECTER. Khong chi tin api vi api bo qua nhung van ban
    /// khong nam trong luong kiem tra ho so cua EMR - xem ghi chu o IsDocumentUnfinished.
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
        private const string STAMP__CHECK_DOCUMENT_HOSPITALIZE = "mien-tru-tich-ky-hep-v13";

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

                // Goi MediRecordChecking TRUOC khi lay danh sach van ban de biet dich vu EMR con song:
                // api nay loi thi bo qua kiem tra luon, tranh chan oan vi tuong ho so khong co van ban.
                MediRecordCheckingResultADO checkingResult = GetMediRecordChecking(treatmentCode);
                if (checkingResult == null)
                {
                    TraceCheckDocumentHospitalize("KET LUAN: api " + URI__EMR_DOCUMENT_MEDI_RECORD_CHECKING
                        + " tra ve null (ma dieu tri " + treatmentCode + ") -> cho phep luu.");
                    return true;
                }

                List<MediRecordCheckingDocumentADO> documents = GetTreatmentDocuments(treatmentCode);
                HashSet<long> unfinishedDocumentIds = GetUnfinishedDocumentIds(checkingResult);

                // Chay CA HAI buoc roi moi thong bao mot lan, de nguoi dung thay het viec phai lam
                // trong mot hop thoai thay vi sua xong buoc 1 lai bi chan tiep boi buoc 2.

                // ---- BUOC 1: tung y lenh chi dinh phai co van ban di kem va van ban do phai hoan thanh ----
                List<string> serviceReqMessages = new List<string>();

                List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ> serviceReqs = GetServiceReqsExceptExam(this.HisServiceReqView.TREATMENT_ID);
                if (serviceReqs != null && serviceReqs.Count > 0)
                {
                    serviceReqMessages = BuildServiceReqDocumentMessages(serviceReqs, documents, unfinishedDocumentIds);

                    TraceCheckDocumentHospitalize(string.Format(
                        "buoc 1: {0} y lenh chi dinh, {1} dong thong bao: [{2}].",
                        serviceReqs.Count, serviceReqMessages.Count, string.Join(" | ", serviceReqMessages)));
                }

                // ---- BUOC 2: van ban thuoc loai duoc tich "chan nhap vien" phai hoan thanh ----
                List<string> blockingDocumentMessages = new List<string>();

                // Ghi rieng hai kieu chua dat: thieu han van ban, va co van ban nhung con cai chua hoan
                // thanh. Phan mien tru cho nut ky o duoi CHI duoc ap dung cho kieu thu nhat.
                List<EMR_DOCUMENT_TYPE> typesWithoutDocument = new List<EMR_DOCUMENT_TYPE>();
                List<EMR_DOCUMENT_TYPE> typesWithUnfinishedDocument = new List<EMR_DOCUMENT_TYPE>();

                List<EMR_DOCUMENT_TYPE> blockingTypes = GetHospitalizationBlockingDocumentTypes();
                if (blockingTypes == null || blockingTypes.Count == 0)
                {
                    TraceCheckDocumentHospitalize("buoc 2 bo qua: khong co loai van ban nao co IS_HOSPITALIZATION = 1 "
                        + "(vao man Danh muc loai van ban EMR tich \"Chan nhap vien\" cho loai van ban can chan).");
                }
                else
                {
                    // Xet TOAN BO van ban cua ho so, chi can loai duoc tich "chan nhap vien" la kiem tra -
                    // KHONG bo qua van ban nao, ke ca van ban dang gan vao y lenh va da duoc buoc 1 neu ten.
                    HashSet<long> blockingTypeIds = new HashSet<long>(blockingTypes.Select(o => o.ID));
                    List<MediRecordCheckingDocumentADO> blockingDocuments = documents
                        .Where(o => o.DOCUMENT_TYPE_ID.HasValue && blockingTypeIds.Contains(o.DOCUMENT_TYPE_ID.Value))
                        .ToList();

                    List<MediRecordCheckingDocumentADO> unfinishedDocuments = blockingDocuments
                        .Where(o => IsDocumentUnfinished(o, unfinishedDocumentIds))
                        .ToList();

                    // In ca hai phia cua phep doi chieu: loai duoc tich chan va loai thuc te cua tung van ban.
                    // Khi ra 0 van ban thuoc loai chan nhap vien thi day la cho duy nhat thay duoc lech o dau.
                    TraceCheckDocumentHospitalize(string.Format(
                        "buoc 2: ho so {0} co {1}/{2} van ban thuoc loai chan nhap vien, trong do {3} van ban chua hoan thanh. "
                        + "Loai duoc tich chan = [{4}]. Loai thuc te cua tung van ban = [{5}].",
                        treatmentCode,
                        blockingDocuments.Count,
                        documents.Count,
                        unfinishedDocuments.Count,
                        string.Join(", ", blockingTypeIds),
                        string.Join(", ", documents.Select(o => string.Format("{0}#loai {1}", o.DOCUMENT_CODE, o.DOCUMENT_TYPE_ID)))));

                    // Tin hieu quyet dinh cua TUNG van ban chan nhap vien - day la cho doi chieu khi
                    // ket qua chan/khong chan khac mong doi.
                    foreach (var document in blockingDocuments)
                    {
                        TraceCheckDocumentHospitalize("buoc 2 van ban chan nhap vien: " + DescribeDocumentForLog(document, unfinishedDocumentIds));
                    }

                    // MOI loai duoc tich "chan nhap vien" phai co IT NHAT MOT van ban da hoan thanh.
                    // Nen phai xet theo TUNG LOAI, khong duoc gop chung: 5 loai duoc tich thi thieu
                    // bat ky mot loai nao cung chan, du cac loai con lai da du.
                    foreach (var blockingType in blockingTypes.OrderBy(o => (o.DOCUMENT_TYPE_NAME ?? "").Trim()))
                    {
                        string typeName = !string.IsNullOrWhiteSpace(blockingType.DOCUMENT_TYPE_NAME)
                            ? blockingType.DOCUMENT_TYPE_NAME.Trim()
                            : (blockingType.DOCUMENT_TYPE_CODE ?? "").Trim();

                        // "Phieu chi dinh" la van ban di kem y lenh chi dinh: khong co y lenh chi dinh nao
                        // thi khong the tao duoc van ban nay, doi la doi mot thu khong the co -> bo qua loai nay.
                        // So theo ID va hang IMSys thay vi so chuoi DOCUMENT_TYPE_CODE: co ten nghia ro rang,
                        // va khong the sai vi "21" / "021" / co khoang trang.
                        // serviceReqs == null nghia la goi api that bai (khong biet co y lenh hay khong)
                        // nen KHONG bo qua, de nghieng ve phia chan cho an toan.
                        if (serviceReqs != null && serviceReqs.Count == 0
                            && blockingType.ID == IMSys.DbConfig.EMR_RS.EMR_DOCUMENT_TYPE.ID__SERVICE_ASSIGN)
                        {
                            TraceCheckDocumentHospitalize(string.Format(
                                "buoc 2 loai '{0}' (ID {1}, code {2}): BO QUA vi ho so khong co y lenh chi dinh nao.",
                                typeName, blockingType.ID, blockingType.DOCUMENT_TYPE_CODE));
                            continue;
                        }

                        List<MediRecordCheckingDocumentADO> documentsOfType = blockingDocuments
                            .Where(o => o.DOCUMENT_TYPE_ID.Value == blockingType.ID)
                            .ToList();
                        List<MediRecordCheckingDocumentADO> unfinishedOfType = documentsOfType
                            .Where(o => IsDocumentUnfinished(o, unfinishedDocumentIds))
                            .ToList();

                        TraceCheckDocumentHospitalize(string.Format(
                            "buoc 2 loai '{0}' (ID {1}): {2} van ban, {3} chua hoan thanh => {4}.",
                            typeName,
                            blockingType.ID,
                            documentsOfType.Count,
                            unfinishedOfType.Count,
                            documentsOfType.Count == 0 ? "THIEU VAN BAN" : (unfinishedOfType.Count > 0 ? "CHUA HOAN THANH" : "DAT")));

                        if (documentsOfType.Count == 0)
                        {
                            typesWithoutDocument.Add(blockingType);
                            AddDistinctMessage(blockingDocumentMessages, string.Format("{0}: chưa có văn bản.", typeName));
                            continue;
                        }

                        if (unfinishedOfType.Count == 0)
                            continue;

                        typesWithUnfinishedDocument.Add(blockingType);
                        foreach (var document in unfinishedOfType
                            .OrderBy(o => o.DOCUMENT_NAME ?? "").ThenBy(o => o.DOCUMENT_CODE ?? ""))
                        {
                            AddDistinctMessage(blockingDocumentMessages, string.Format("{0}: văn bản {1} {2}.",
                                typeName,
                                GetDocumentDisplayName("", document.DOCUMENT_NAME, document.DOCUMENT_CODE),
                                GetUnfinishedReason(document)));
                        }
                    }

                    // Co loai chua dat ma khong dung duoc dong nao thi KHONG duoc de mat viec chan.
                    int unsatisfiedTypeCount = typesWithoutDocument.Count + typesWithUnfinishedDocument.Count;
                    if (unsatisfiedTypeCount > 0 && blockingDocumentMessages.Count == 0)
                    {
                        blockingDocumentMessages.Add(string.Format(
                            "Còn {0} loại văn bản chặn nhập viện chưa có văn bản đã hoàn thành.", unsatisfiedTypeCount));
                    }

                    TraceCheckDocumentHospitalize(string.Format(
                        "buoc 2: {0} dong thong bao: [{1}].",
                        blockingDocumentMessages.Count, string.Join(" | ", blockingDocumentMessages)));
                }

                if (serviceReqMessages.Count == 0 && blockingDocumentMessages.Count == 0)
                {
                    TraceCheckDocumentHospitalize("KET LUAN: ca hai buoc deu dat -> cho phep luu.");
                    return true;
                }

                // Mien tru cho nut ky tren UC.Hospitalize.
                //
                // Thu tu thuc te khi bam luu: kiem tra (cho nay) -> luu -> MOI ky. Nen neu bat buoc phai co
                // "Phieu kham benh vao vien" da hoan thanh, nguoi dung tich ky de ky luon se khong bao gio
                // thoat ra duoc: luc kiem tra van ban con chua duoc tao.
                //
                // Mien RAT HEP, phai thoa het:
                //  - buoc 1 sach (khong y lenh chi dinh nao thieu van ban);
                //  - buoc 2 chi con DUNG MOT loai chua dat, va loai do la phieu kham benh vao vien;
                //  - loai do chua dat theo kieu "CHUA CO VAN BAN NAO" - khong phai "co van ban nhung con
                //    cai chua hoan thanh". Viec ky chi tao va ky MOT van ban, nen no khong the lam xong
                //    mot van ban cung loai dang do dang san; mien ca loai la bo lot van ban do;
                //  - nguoi dung dang tich nut ky.
                if (serviceReqMessages.Count == 0
                    && typesWithUnfinishedDocument.Count == 0
                    && typesWithoutDocument.Count == 1
                    && IsHospitalizeExamDocumentType(typesWithoutDocument[0])
                    && IsSignHospitalizeExamChecked())
                {
                    TraceCheckDocumentHospitalize(string.Format(
                        "KET LUAN: chi con thieu loai '{0}' (ID {1}, code {2}) va nguoi dung da tich ky tren UC.Hospitalize "
                        + "-> cho phep luu, van ban se duoc tao va ky ngay sau khi luu.",
                        typesWithoutDocument[0].DOCUMENT_TYPE_NAME,
                        typesWithoutDocument[0].ID,
                        typesWithoutDocument[0].DOCUMENT_TYPE_CODE));
                    return true;
                }

                TraceCheckDocumentHospitalize(string.Format(
                    "KET LUAN: CHAN nhap vien ho so {0} - buoc 1 co {1} dong, buoc 2 co {2} dong.",
                    treatmentCode, serviceReqMessages.Count, blockingDocumentMessages.Count));

                ShowHospitalizeBlockedMessage(serviceReqMessages, blockingDocumentMessages);
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
        /// Cac y lenh CHI DINH cua dot dieu tri - moi y lenh tru y lenh Kham va cac loai DON (ke don),
        /// vi van ban cua kham va cua don thuoc khong can ky kem theo y lenh
        /// (loai Kham lam giong HIS.Desktop.Plugins.ExecuteRoom\UCExecuteRoom___Process.cs khi canh bao
        /// y lenh chua co van ban ky).
        ///
        /// Nhom "don" gom DUNG 4 loai: DONK, DONTT, DONDT, DONM - khong con loai don nao khac.
        /// </summary>
        /// <returns>null neu goi api that bai (bo qua buoc 1), list rong neu that su khong co y lenh nao.</returns>
        private List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ> GetServiceReqsExceptExam(long treatmentId)
        {
            try
            {
                if (treatmentId <= 0)
                {
                    TraceCheckDocumentHospitalize("TREATMENT_ID = " + treatmentId + " khong hop le, bo qua kiem tra theo y lenh.");
                    return null;
                }

                List<long> notInServiceReqTypeIds = new List<long>
                {
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONTT,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONM
                };

                CommonParam param = new CommonParam();
                MOS.Filter.HisServiceReqFilter filter = new MOS.Filter.HisServiceReqFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                filter.TREATMENT_ID = treatmentId;
                filter.NOT_IN_SERVICE_REQ_TYPE_IDs = notInServiceReqTypeIds;

                var serviceReqs = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ>>(
                    URI__HIS_SERVICE_REQ_GET, ApiConsumers.MosConsumer, filter, param);
                if (serviceReqs == null || serviceReqs.Count == 0)
                {
                    TraceCheckDocumentHospitalize("api " + URI__HIS_SERVICE_REQ_GET + " (TREATMENT_ID = " + treatmentId
                        + ", NOT_IN_SERVICE_REQ_TYPE_IDs = [" + string.Join(", ", notInServiceReqTypeIds)
                        + "]) tra ve " + (serviceReqs == null ? "null" : "rong") + ": khong co y lenh chi dinh nao -> bo qua buoc 1.");
                    return new List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ>();
                }

                TraceCheckDocumentHospitalize(string.Format(
                    "dot dieu tri TREATMENT_ID = {0} co {1} y lenh chi dinh: [{2}].",
                    treatmentId,
                    serviceReqs.Count,
                    string.Join(", ", serviceReqs.Select(o => string.Format("{0}#loai {1}", o.SERVICE_REQ_CODE, o.SERVICE_REQ_TYPE_ID)))));
                return serviceReqs;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(LOG__CHECK_DOCUMENT_HOSPITALIZE + "loi khi goi api "
                    + URI__HIS_SERVICE_REQ_GET + " (TREATMENT_ID = " + treatmentId + ") nen bo qua buoc 1.", ex);
                return null;
            }
        }

        /// <summary>
        /// ID cac van ban chua hoan thanh chu ky theo ket qua MediRecordChecking.
        /// Tin theo ket qua cua api (dung nhu HIS.Desktop.Plugins.TransDepartment\frmDepartmentTran.cs):
        /// moi phan tu trong SignatureMissingDocuments deu la van ban con thieu chu ky,
        /// ke ca truong hop chi con thieu chu ky cua benh nhan.
        /// </summary>
        private HashSet<long> GetUnfinishedDocumentIds(MediRecordCheckingResultADO checkingResult)
        {
            return new HashSet<long>((checkingResult.SignatureMissingDocuments ?? new List<MediRecordCheckingDocumentADO>())
                .Where(o => o != null)
                .Select(o => o.ID));
        }

        /// <summary>
        /// Loai van ban nay co phai "Phieu kham benh vao vien" - van ban ma nut ky tren UC.Hospitalize
        /// se tao va ky - hay khong.
        ///
        /// So theo DOCUMENT_TYPE_CODE = "02" vi IMSys.DbConfig.EMR_RS.EMR_DOCUMENT_TYPE KHONG co hang cho
        /// loai nay (chi co ID__SERVICE_ASSIGN, ID__INFUSION, ID__PRESCRIPTION, ID__TRACKING, ID__CARE,
        /// ID__DHST, ID__MEDI_REACT, ID__DEBATE, ID__SERVICE_RESULT, ID__TRANSFUSION).
        ///
        /// Vien nao khai ma khac "02" thi phep mien tru nay khong chay - chi lam nguoi dung bi chan nhu
        /// truoc, khong sai lech nguy hiem; sua thi doi dung chuoi o day.
        /// </summary>
        private bool IsHospitalizeExamDocumentType(EMR_DOCUMENT_TYPE documentType)
        {
            return documentType != null
                && string.Equals((documentType.DOCUMENT_TYPE_CODE ?? "").Trim(), "02", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Nguoi dung co tich nut ky tren UC.Hospitalize hay khong (chkSign -> HospitalizeExamADO.IsSign).
        ///
        /// KHONG dung field this.isSign: field do chi duoc gan trong ProcessExamServiceReqExecute, chay SAU
        /// phep kiem tra nay trong btnSaveFinish_Click_Action (va bi reset ve false o dau ham do), nen luc
        /// kiem tra no luon la false. Phai doc thang tu UC giong cach lam o cac diem goi GetValue khac.
        ///
        /// Chi goi o nhanh cuoi cua phep kiem tra, khong goi som: GetValue cua UC co goi
        /// dxValidationProvider1.Validate() nen co tac dung phu ve giao dien, va tra ve null neu validate
        /// that bai - luc do coi nhu khong tich ky, tuc van chan.
        ///
        /// Khong can kiem them chkPrintHospitalizeExam: UC chi cho bat chkSign khi chkPrintHospitalizeExam
        /// da tich, va bo tich in la tu bo tich ky (xem UCHospitalize.UpdateCheckPrintAndSign).
        /// </summary>
        /// <returns>false khi khong doc duoc - de nghieng ve phia chan cho an toan.</returns>
        private bool IsSignHospitalizeExamChecked()
        {
            try
            {
                if (this.ucHospitalize == null || this.hospitalizeProcessor == null)
                {
                    TraceCheckDocumentHospitalize("chua nap UC.Hospitalize, coi nhu KHONG tich ky.");
                    return false;
                }

                HIS.UC.Hospitalize.ADO.HospitalizeExamADO hospitalizeADO =
                    this.hospitalizeProcessor.GetValue(this.ucHospitalize) as HIS.UC.Hospitalize.ADO.HospitalizeExamADO;
                if (hospitalizeADO == null)
                {
                    TraceCheckDocumentHospitalize("GetValue cua UC.Hospitalize tra ve null (co the do validate that bai), coi nhu KHONG tich ky.");
                    return false;
                }

                TraceCheckDocumentHospitalize("UC.Hospitalize IsSign = " + hospitalizeADO.IsSign
                    + ", IsPrintHospitalizeExam = " + hospitalizeADO.IsPrintHospitalizeExam + ".");
                return hospitalizeADO.IsSign;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(LOG__CHECK_DOCUMENT_HOSPITALIZE
                    + "loi khi doc co ky tren UC.Hospitalize, coi nhu KHONG tich ky.", ex);
                return false;
            }
        }

        /// <summary>
        /// Van ban da hoan thanh chua.
        ///
        /// KHONG chi tin vao SignatureMissingDocuments cua api MediRecordChecking: api do bo qua nhung 
        /// van ban khong nam trong luong kiem tra ho so cua EMR (vi du loai van ban khong bat buoc),
        /// nen mot van ban dang con nguoi phai ky van co the KHONG xuat hien trong danh sach do -
        /// da gap dung ca nay: 2 van ban chan nhap vien, 1 chua ky xong, ma van cho nhap vien.
        ///
        /// Nen coi la CHUA hoan thanh khi thoa BAT KY dieu nao duoi day. Ba dieu sau doc truc tiep tren
        /// van ban, dung dung tin hieu EMR dung trong filter EmrDocumentViewFilter
        /// (HAS_NEXT_SIGNER_OR_NOT_SIGNERS, HAS_REJECTER):
        ///  - api MediRecordChecking bao thieu chu ky;
        ///  - con NEXT_SIGNER (den luot nguoi khac phai ky);
        ///  - SIGNERS rong (chua ai ky);
        ///  - con REJECTER (co nguoi tu choi ky).
        /// </summary>
        private bool IsDocumentUnfinished(MediRecordCheckingDocumentADO document, HashSet<long> apiUnfinishedDocumentIds)
        {
            if (document == null)
                return false;

            return apiUnfinishedDocumentIds.Contains(document.ID)
                || !string.IsNullOrWhiteSpace(document.NEXT_SIGNER)
                || string.IsNullOrWhiteSpace(document.SIGNERS)
                || !string.IsNullOrWhiteSpace(document.REJECTER);
        }

        /// <summary>
        /// Mot dong log day du tin hieu quyet dinh cua mot van ban, de doi chieu khi ket qua chan/khong chan
        /// khac voi mong doi. In nguyen van gia tri thay vi chi in ket luan.
        /// </summary>
        private string DescribeDocumentForLog(MediRecordCheckingDocumentADO document, HashSet<long> apiUnfinishedDocumentIds)
        {
            return string.Format("{0}#{1}#loai {2}#{3}: SIGNERS='{4}', NEXT_SIGNER='{5}', UN_SIGNERS='{6}', REJECTER='{7}', MediRecordChecking={8} => {9}",
                document.ID,
                document.DOCUMENT_CODE,
                document.DOCUMENT_TYPE_ID,
                document.DOCUMENT_TYPE_CODE,
                document.SIGNERS,
                document.NEXT_SIGNER,
                document.UN_SIGNERS,
                document.REJECTER,
                apiUnfinishedDocumentIds.Contains(document.ID) ? "THIEU-CHU-KY" : "khong-bao",
                IsDocumentUnfinished(document, apiUnfinishedDocumentIds) ? "CHUA HOAN THANH" : "da hoan thanh");
        }

        /// <summary>
        /// Van ban co gan voi y lenh nay khong.
        ///
        /// Quy uoc dung chung toan he: EMR_DOCUMENT.HIS_CODE chua chuoi "SERVICE_REQ_CODE:&lt;ma y lenh&gt;"
        /// (xem HIS.Desktop.Plugins.ExecuteRoom, HIS.Desktop.Plugins.ServiceExecute...). HIS_CODE con co the
        /// ghep them SER_SERV_ID / TREATMENT_CODE / BAR_CODE nen phai do theo kieu "chua chuoi con".
        ///
        /// Chan truong hop ma y lenh nay la TIEN TO cua ma y lenh khac (vi du 123 khop nham vao 1234)
        /// bang cach doi hoi ky tu ngay sau ma khong duoc la chu so.
        /// </summary>
        private bool IsDocumentOfServiceReq(MediRecordCheckingDocumentADO document, string serviceReqCode)
        {
            if (document == null || string.IsNullOrEmpty(document.HIS_CODE) || string.IsNullOrWhiteSpace(serviceReqCode))
                return false;

            string hisCode = document.HIS_CODE;
            string token = "SERVICE_REQ_CODE:" + serviceReqCode.Trim();

            int index = hisCode.IndexOf(token, StringComparison.OrdinalIgnoreCase);
            while (index >= 0)
            {
                int next = index + token.Length;
                if (next >= hisCode.Length || !char.IsDigit(hisCode[next]))
                    return true;

                index = hisCode.IndexOf(token, index + 1, StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }

        /// <summary>
        /// BUOC 1: moi y lenh chi dinh phai co van ban di kem va van ban do phai hoan thanh.
        /// Mot dong thong bao cho moi y lenh thieu van ban / moi van ban chua hoan thanh cua y lenh.
        /// </summary>
        private List<string> BuildServiceReqDocumentMessages(
            List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ> serviceReqs,
            List<MediRecordCheckingDocumentADO> documents,
            HashSet<long> unfinishedDocumentIds)
        {
            List<string> messages = new List<string>();
            try
            {
                // Tra cuu ten loai y lenh mot lan, tranh goi BackendDataWorker.Get trong vong lap.
                // Bao rieng try/catch: hong cho nay chi lam xau ten hien thi, KHONG duoc lam mat phep chan.
                Dictionary<long, string> serviceReqTypeNameById = new Dictionary<long, string>();
                try
                {
                    var serviceReqTypes = BackendDataWorker.Get<HIS_SERVICE_REQ_TYPE>();
                    if (serviceReqTypes != null)
                    {
                        foreach (var serviceReqType in serviceReqTypes)
                        {
                            if (serviceReqType != null && !serviceReqTypeNameById.ContainsKey(serviceReqType.ID))
                                serviceReqTypeNameById.Add(serviceReqType.ID, serviceReqType.SERVICE_REQ_TYPE_NAME);
                        }
                    }
                    else
                    {
                        TraceCheckDocumentHospitalize("BackendDataWorker.Get<HIS_SERVICE_REQ_TYPE>() tra ve null, thong bao chi hien ma y lenh.");
                    }
                }
                catch (Exception exServiceReqType)
                {
                    Inventec.Common.Logging.LogSystem.Error(LOG__CHECK_DOCUMENT_HOSPITALIZE
                        + "loi khi tra cuu ten loai y lenh, thong bao chi hien ma y lenh.", exServiceReqType);
                }

                foreach (var serviceReq in serviceReqs.OrderBy(o => o.SERVICE_REQ_CODE ?? ""))
                {
                    string serviceReqName = GetServiceReqDisplayName(serviceReq, serviceReqTypeNameById);

                    List<MediRecordCheckingDocumentADO> documentsOfServiceReq = documents
                        .Where(o => IsDocumentOfServiceReq(o, serviceReq.SERVICE_REQ_CODE))
                        .ToList();

                    if (documentsOfServiceReq.Count == 0)
                    {
                        AddDistinctMessage(messages, string.Format("{0}: chưa có văn bản.", serviceReqName));
                        continue;
                    }

                    foreach (var document in documentsOfServiceReq.Where(o => IsDocumentUnfinished(o, unfinishedDocumentIds))
                        .OrderBy(o => o.DOCUMENT_NAME ?? "").ThenBy(o => o.DOCUMENT_CODE ?? ""))
                    {
                        AddDistinctMessage(messages, string.Format("{0}: văn bản {1} {2}.",
                            serviceReqName,
                            GetDocumentDisplayName((document.DOCUMENT_TYPE_NAME ?? "").Trim(), document.DOCUMENT_NAME, document.DOCUMENT_CODE),
                            GetUnfinishedReason(document)));
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(LOG__CHECK_DOCUMENT_HOSPITALIZE + "loi khi dung thong bao buoc 1.", ex);
            }
            return messages;
        }

        /// <summary>
        /// Hien thi y lenh dang "Loai y lenh (Ma y lenh)" de nguoi dung tim duoc dong tuong ung tren man hinh.
        /// </summary>
        private string GetServiceReqDisplayName(
            MOS.EFMODEL.DataModels.HIS_SERVICE_REQ serviceReq, Dictionary<long, string> serviceReqTypeNameById)
        {
            string code = (serviceReq.SERVICE_REQ_CODE ?? "").Trim();
            string typeName = serviceReqTypeNameById.ContainsKey(serviceReq.SERVICE_REQ_TYPE_ID)
                ? (serviceReqTypeNameById[serviceReq.SERVICE_REQ_TYPE_ID] ?? "").Trim()
                : "";

            if (typeName.Length == 0)
                return code.Length > 0 ? code : "(không xác định)";

            return code.Length > 0 ? string.Format("{0} ({1})", typeName, code) : typeName;
        }

        /// <summary>
        /// Hop thoai chan nhap vien: gom ket qua ca hai buoc vao MOT thong bao, moi buoc mot muc.
        /// So thu tu chay lien tuc qua ca hai muc de nguoi dung dem duoc tong so viec phai lam.
        /// Muc nao khong co dong nao thi khong in tieu de muc do.
        /// </summary>
        private void ShowHospitalizeBlockedMessage(List<string> serviceReqMessages, List<string> blockingDocumentMessages)
        {
            StringBuilder message = new StringBuilder();
            message.AppendLine("Không thể nhập viện. Hồ sơ còn nội dung chưa hoàn thành:");

            int order = 0;

            if (serviceReqMessages.Count > 0)
            {
                message.AppendLine();
                message.AppendLine("Y lệnh chỉ định chưa có văn bản hoặc văn bản chưa hoàn thành:");
                foreach (string item in serviceReqMessages)
                {
                    order++;
                    message.AppendLine(string.Format("{0}. {1}", order, item));
                }
            }

            if (blockingDocumentMessages.Count > 0)
            {
                message.AppendLine();
                message.AppendLine("Văn bản thuộc loại chặn nhập viện:");
                foreach (string item in blockingDocumentMessages)
                {
                    order++;
                    message.AppendLine(string.Format("{0}. {1}", order, item));
                }
            }

            message.AppendLine();
            message.Append("Vui lòng hoàn thiện trước khi nhập viện!");

            XtraMessageBox.Show(message.ToString(), ResourceMessage.ThongBao, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        /// Toan bo van ban (chua bi xoa) cua dot dieu tri. Ca hai buoc kiem tra deu dung chung danh sach nay
        /// nen chi goi api mot lan.
        ///
        /// KHONG truyen filter.DOCUMENT_TYPE_IDs de loc san theo loai: filter cua BackendAdapter.Get di qua
        /// query string nen truyen danh sach ID vao day la them mot duong tra ve null im lang. Loc tai client.
        ///
        /// Cung KHONG truyen filter.IS_DELETE: cot IS_DELETE tren V_EMR_DOCUMENT la NUMBER nullable
        /// (short?) trong khi filter la bool?, van ban binh thuong de NULL chu khong phai 0 - loc phia
        /// server de mat luon nhung van ban chua xoa. Loc tai client theo "khac 1".
        ///
        /// Tra ve list rong (khong bao gio null) - da goi MediRecordChecking thanh cong ngay truoc do nen
        /// dich vu EMR chac chan con song, rong o day dung nghia la ho so chua co van ban.
        /// </summary>
        private List<MediRecordCheckingDocumentADO> GetTreatmentDocuments(string treatmentCode)
        {
            try
            {
                CommonParam param = new CommonParam();
                EmrDocumentViewFilter filter = new EmrDocumentViewFilter();
                filter.TREATMENT_CODE__EXACT = treatmentCode;

                var documents = new BackendAdapter(param).Get<List<MediRecordCheckingDocumentADO>>(
                    URI__EMR_DOCUMENT_GET_VIEW, ApiConsumers.EmrConsumer, filter, param);
                if (documents == null || documents.Count == 0)
                {
                    TraceCheckDocumentHospitalize("api " + URI__EMR_DOCUMENT_GET_VIEW + " tra ve "
                        + (documents == null ? "null" : "rong") + " (ma dieu tri " + treatmentCode + "): ho so chua co van ban nao.");
                    return new List<MediRecordCheckingDocumentADO>();
                }

                int countFromApi = documents.Count;
                documents = documents.Where(o => o != null && o.IS_DELETE != 1).ToList();
                TraceCheckDocumentHospitalize(string.Format(
                    "api {0} tra ve {1} van ban, con {2} van ban chua xoa (IS_DELETE khac 1).",
                    URI__EMR_DOCUMENT_GET_VIEW, countFromApi, documents.Count));

                // Liet ke DAY DU van ban kem loai va HIS_CODE: day la cho duy nhat doi chieu duoc
                // "van ban dang gan vao y lenh nao" voi "y lenh nao dang bi bao thieu van ban".
                TraceCheckDocumentHospitalize(string.Format(
                    "ho so {0} co {1} van ban: [{2}].",
                    treatmentCode,
                    documents.Count,
                    string.Join(", ", documents.Select(o => string.Format(
                        "{0}#{1}#loai {2}#{3}#HIS_CODE={4}", o.ID, o.DOCUMENT_CODE, o.DOCUMENT_TYPE_ID, o.DOCUMENT_TYPE_CODE, o.HIS_CODE)))));
                return documents;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(LOG__CHECK_DOCUMENT_HOSPITALIZE + "loi khi goi api "
                    + URI__EMR_DOCUMENT_GET_VIEW + " (ma dieu tri " + treatmentCode + ").", ex);
                return new List<MediRecordCheckingDocumentADO>();
            }
        }

        private void AddDistinctMessage(List<string> messages, string message)
        {
            if (!messages.Contains(message))
                messages.Add(message);
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
        /// Ly do van ban chua hoan thanh, theo dung thu tu uu tien de cau thong bao noi ro viec phai lam:
        /// bi tu choi ky -&gt; chua ai ky -&gt; con nguoi phai ky -&gt; con lai la chua ky du.
        /// </summary>
        private string GetUnfinishedReason(MediRecordCheckingDocumentADO document)
        {
            if (!string.IsNullOrWhiteSpace(document.REJECTER))
                return "đã bị từ chối ký";

            if (string.IsNullOrWhiteSpace(document.SIGNERS))
                return "chưa được ký";

            if (!string.IsNullOrWhiteSpace(document.NEXT_SIGNER))
                return "chưa ký đủ (còn người phải ký)";

            return "chưa ký đủ";
        }
    }
}
