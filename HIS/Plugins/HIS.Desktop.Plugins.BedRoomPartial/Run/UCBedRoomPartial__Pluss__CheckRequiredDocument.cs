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
using EMR.Filter;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Plugins.BedRoomPartial.ADO;
using HIS.Desktop.Plugins.BedRoomPartial.Key;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.BedRoomPartial
{
    /// <summary>
    /// Canh bao cac Loai van ban bat buoc phai hoan thanh khi benh nhan vao khoa.
    ///
    /// Chay khi nguoi dung chon mot benh nhan tren danh sach buong benh — CA bam TRAI (RowClick) lan
    /// bam PHAI (PopupMenuShowing, duong vao menu ngu canh). Trinh tu, dung ra o buoc dau tien
    /// khong dat - moi diem ra deu IM LANG, khong chan va khong bao loi ra man hinh:
    ///
    ///  1. Cau hinh so phut khong phai so nguyen duong                  -> ra (tinh nang chua bat)
    ///  2. Benh nhan nay da duoc nhac trong phien lam viec hien tai     -> ra (QT10)
    ///  3. Ho so khong co thoi diem nhap vien vao khoa                  -> ra (chua vao khoa / ngoai tru)
    ///  4. Ho so da ket thuc dieu tri                                   -> ra (QT14)
    ///  5. Vao khoa TRUOC lan cap nhat gan nhat cua cau hinh so phut    -> ra (QT13 - khong hoi to)
    ///  6. Chua du so phut ke tu luc vao khoa                           -> ra (QT4)
    ///  7. Khong co loai van ban nao duoc tich "Hoan thanh khi vao khoa"-> ra (QT2)
    ///  8. Khong tra cuu duoc ho so                                     -> ra (QT15)
    ///  9. Moi loai duoc tich deu dat                                   -> ra (QT8)
    /// 10. Con loai thieu -> danh dau da nhac trong phien, hien hop thoai liet ke
    ///
    /// Chi CANH BAO: hien thong bao roi cho nguoi dung lam tiep, KHONG chan thao tac nao (QT12).
    /// Cau hinh muc rang buoc (canh bao / chan) da bo — neu ve sau can chan thi lam bang dau viec rieng.
    ///
    /// Vi sao doc co "Hoan thanh khi vao khoa" qua ADO cuc bo chu khong qua EMR.EFMODEL: xem ghi chu
    /// trong ADO\RequiredDocumentADO.cs. Nho vay tep nay bien dich va chay duoc ngay ca khi chua them cot
    /// va chua gencode - khi do khong loai nao vao dien kiem tra nen tinh nang nam im.
    /// </summary>
    public partial class UCBedRoomPartial
    {
        private const string URI__EMR_DOCUMENT_MEDI_RECORD_CHECKING = "api/EmrDocument/MediRecordChecking";
        private const string URI__EMR_DOCUMENT_GET_VIEW = "api/EmrDocument/GetView";
        private const string URI__EMR_DOCUMENT_TYPE_GET = "api/EmrDocumentType/Get";
        private const string URI__HIS_CONFIG_GET = "api/HisConfig/Get";

        /// <summary>
        /// Tien to log de tim nhanh trong LogSystem.txt khi canh bao khong hien nhu mong doi.
        /// Moi diem thoat som deu ghi log kem gia tri thuc te vi tat ca cac nhanh do deu im lang -
        /// khong co log thi khong biet dut o dau.
        /// </summary>
        private const string LOG__CHECK_REQUIRED_DOCUMENT = "CheckRequiredDocumentWhenInDepartment: ";

        /// <summary>
        /// Dau nhan de doi chieu DLL dang chay voi ban vua build. Doi moi lan sua logic phep kiem tra nay.
        /// </summary>
        private const string STAMP__CHECK_REQUIRED_DOCUMENT = "canh-bao-vao-khoa-v3-chi-canh-bao";

        /// <summary>
        /// TREATMENT_ID cac benh nhan da duoc nhac trong phien lam viec hien tai cua man Buong benh.
        /// QT10: moi benh nhan chi nhac MOT lan trong mot phien; dong mo lai phan mem thi nhac lai.
        /// </summary>
        private readonly HashSet<long> requiredDocumentWarnedTreatmentIds = new HashSet<long>();

        /// <summary>
        /// Lan cap nhat gan nhat cua cau hinh so phut, dang yyyyMMddHHmmss. Tra cuu mot lan cho ca phien.
        /// null = chua tra cuu; 0 = da tra cuu nhung khong tim thay dong cau hinh.
        /// </summary>
        private long? requiredDocumentConfigTimeCache;

        private void TraceCheckRequiredDocument(string message)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Error(LOG__CHECK_REQUIRED_DOCUMENT + message);
            }
            catch
            {
                // Ghi log that bai thi khong duoc anh huong nghiep vu
            }
        }

        /// <summary>
        /// Kiem tra va canh bao cac Loai van ban bat buoc chua hoan thanh cua benh nhan dang chon.
        /// Khong bao gio nem ngoai le ra ngoai va khong bao gio chan thao tac cua nguoi dung.
        /// </summary>
        private void CheckRequiredDocumentWhenInDepartment(L_HIS_TREATMENT_BED_ROOM row)
        {
            try
            {
                if (row == null)
                    return;

                // ---- 1. Cau hinh so phut ----
                int checkMinutes = HisConfigCFG.RequiredDocumentCheckMinutes;
                if (checkMinutes <= 0)
                {
                    // Khong log o day: day la trang thai binh thuong cua moi vien chua bat tinh nang,
                    // ghi log moi lan chon benh nhan se lam ngap LogSystem.txt.
                    return;
                }

                TraceCheckRequiredDocument(string.Format(
                    "BAT DAU [" + STAMP__CHECK_REQUIRED_DOCUMENT + "]. TREATMENT_ID = {0}, ma dieu tri = {1}, "
                    + "so phut cau hinh = {2}.",
                    row.TREATMENT_ID, row.TREATMENT_CODE, checkMinutes));

                // ---- 2. Da nhac trong phien nay chua ----
                if (requiredDocumentWarnedTreatmentIds.Contains(row.TREATMENT_ID))
                {
                    TraceCheckRequiredDocument("KET LUAN: da nhac benh nhan nay trong phien hien tai -> khong nhac lai.");
                    return;
                }

                // ---- 3. Thoi diem nhap vien vao khoa ----
                if (!row.CLINICAL_IN_TIME.HasValue || row.CLINICAL_IN_TIME.Value <= 0)
                {
                    TraceCheckRequiredDocument("KET LUAN: ho so khong co thoi diem nhap vien vao khoa -> khong kiem tra.");
                    return;
                }
                long clinicalInTime = row.CLINICAL_IN_TIME.Value;

                // ---- 4. Da ket thuc dieu tri chua ----
                if (row.OUT_TIME.HasValue && row.OUT_TIME.Value > 0)
                {
                    TraceCheckRequiredDocument("KET LUAN: ho so da ket thuc dieu tri (OUT_TIME = "
                        + row.OUT_TIME.Value + ") -> khong kiem tra.");
                    return;
                }

                // ---- 5. Khong hoi to ----
                long configTime = GetRequiredDocumentConfigTime();
                if (configTime > 0 && clinicalInTime < configTime)
                {
                    TraceCheckRequiredDocument(string.Format(
                        "KET LUAN: benh nhan vao khoa luc {0}, truoc lan khai bao cau hinh luc {1} -> khong hoi to.",
                        clinicalInTime, configTime));
                    return;
                }

                // ---- 6. Da du so phut chua ----
                DateTime? inTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(clinicalInTime);
                if (inTime == null)
                {
                    TraceCheckRequiredDocument("KET LUAN: khong doc duoc thoi diem vao khoa " + clinicalInTime + " -> khong kiem tra.");
                    return;
                }

                DateTime checkFrom = inTime.Value.AddMinutes(checkMinutes);
                if (DateTime.Now < checkFrom)
                {
                    TraceCheckRequiredDocument(string.Format(
                        "KET LUAN: chua den moc kiem tra. Vao khoa {0:dd/MM/yyyy HH:mm}, kiem tra tu {1:dd/MM/yyyy HH:mm}.",
                        inTime.Value, checkFrom));
                    return;
                }

                // ---- 7. Loai van ban thuoc dien bat buoc khi vao khoa ----
                List<RequiredDocumentTypeADO> requiredTypes = GetRequiredWhenInDepartmentDocumentTypes();
                if (requiredTypes == null || requiredTypes.Count == 0)
                {
                    TraceCheckRequiredDocument("KET LUAN: khong co loai van ban nao duoc tich \"Hoan thanh khi vao khoa\" "
                        + "(vao man Danh muc loai van ban tich cho loai can kiem tra) -> khong canh bao.");
                    return;
                }

                // ---- 8. Tra cuu ho so ----
                string treatmentCode = row.TREATMENT_CODE;
                if (string.IsNullOrWhiteSpace(treatmentCode))
                {
                    TraceCheckRequiredDocument("KET LUAN: ma dieu tri rong -> khong kiem tra.");
                    return;
                }

                // Goi MediRecordChecking TRUOC khi lay danh sach van ban de biet dich vu EMR con song:
                // api nay loi thi bo qua kiem tra luon, tranh canh bao oan vi tuong ho so khong co van ban.
                RequiredDocumentCheckingResultADO checkingResult = GetRequiredDocumentChecking(treatmentCode);
                if (checkingResult == null)
                {
                    TraceCheckRequiredDocument("KET LUAN: api " + URI__EMR_DOCUMENT_MEDI_RECORD_CHECKING
                        + " tra ve null (ma dieu tri " + treatmentCode + ") -> khong canh bao.");
                    return;
                }

                List<RequiredDocumentADO> documents = GetTreatmentDocumentsForRequiredCheck(treatmentCode);
                if (documents == null)
                {
                    TraceCheckRequiredDocument("KET LUAN: api " + URI__EMR_DOCUMENT_GET_VIEW
                        + " loi (ma dieu tri " + treatmentCode + ") -> khong canh bao.");
                    return;
                }

                HashSet<long> unfinishedDocumentIds = GetUnfinishedDocumentIdsForRequiredCheck(checkingResult);

                // ---- 9. Xet TUNG loai duoc tich ----
                List<string> missingMessages = new List<string>();
                foreach (var requiredType in requiredTypes.OrderBy(o => (o.DOCUMENT_TYPE_NAME ?? "").Trim()))
                {
                    string typeName = !string.IsNullOrWhiteSpace(requiredType.DOCUMENT_TYPE_NAME)
                        ? requiredType.DOCUMENT_TYPE_NAME.Trim()
                        : (requiredType.DOCUMENT_TYPE_CODE ?? "").Trim();

                    List<RequiredDocumentADO> documentsOfType = documents
                        .Where(o => o.DOCUMENT_TYPE_ID.HasValue && o.DOCUMENT_TYPE_ID.Value == requiredType.ID)
                        .ToList();

                    // QT5: loai do chua co van ban nao trong chi tiet benh an.
                    if (documentsOfType.Count == 0)
                    {
                        missingMessages.Add(string.Format("- {0}: chưa có văn bản.", typeName));
                        TraceCheckRequiredDocument(string.Format(
                            "loai '{0}' (ID {1}): 0 van ban => THIEU VAN BAN.", typeName, requiredType.ID));
                        continue;
                    }

                    // QT6: chi can MOT van ban thuoc loai do da hoan thanh ky thi loai do dat.
                    bool hasFinished = documentsOfType.Any(o => !IsDocumentUnfinishedForRequiredCheck(o, unfinishedDocumentIds));
                    TraceCheckRequiredDocument(string.Format(
                        "loai '{0}' (ID {1}): {2} van ban, co van ban da hoan thanh = {3} => {4}.",
                        typeName, requiredType.ID, documentsOfType.Count, hasFinished,
                        hasFinished ? "DAT" : "CHUA HOAN THANH KY"));

                    if (hasFinished)
                        continue;

                    missingMessages.Add(string.Format("- {0}: văn bản chưa ký xong.", typeName));
                }

                if (missingMessages.Count == 0)
                {
                    TraceCheckRequiredDocument("KET LUAN: moi loai van ban bat buoc deu dat -> khong canh bao.");
                    return;
                }

                // ---- 10. Canh bao ----
                requiredDocumentWarnedTreatmentIds.Add(row.TREATMENT_ID);
                TraceCheckRequiredDocument(string.Format(
                    "KET LUAN: canh bao {0} loai van ban chua hoan thanh: [{1}].",
                    missingMessages.Count, string.Join(" | ", missingMessages)));

                ShowRequiredDocumentWarning(missingMessages);
            }
            catch (Exception ex)
            {
                // QT15: loi cua phep kiem tra KHONG duoc cham vao nghiep vu cua nguoi dung.
                Inventec.Common.Logging.LogSystem.Error(LOG__CHECK_REQUIRED_DOCUMENT + "loi khong mong doi, bo qua phep kiem tra.", ex);
            }
        }

        /// <summary>
        /// Hop thoai liet ke cac loai van ban con thieu. Chi nhac viec, KHONG chan thao tac nao (QT12).
        /// </summary>
        private void ShowRequiredDocumentWarning(List<string> missingMessages)
        {
            try
            {
                string content = "Bệnh nhân còn biểu mẫu bắt buộc chưa hoàn thành khi vào khoa:"
                    + Environment.NewLine + Environment.NewLine
                    + string.Join(Environment.NewLine, missingMessages)
                    + Environment.NewLine + Environment.NewLine
                    + "Đề nghị hoàn thiện các biểu mẫu trên.";

                DevExpress.XtraEditors.XtraMessageBox.Show(
                    content,
                    "Biểu mẫu bắt buộc chưa hoàn thành",
                    System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(LOG__CHECK_REQUIRED_DOCUMENT + "loi khi hien hop thoai canh bao.", ex);
            }
        }

        /// <summary>
        /// Lan cap nhat gan nhat cua dong cau hinh so phut, dang yyyyMMddHHmmss. Tra ve 0 neu khong xac dinh duoc.
        ///
        /// Day la moc "thoi diem khai bao cau hinh" cua quy tac khong hoi to (QT13). Tra cuu mot lan cho ca
        /// phien vi gia tri nay gan nhu khong doi trong mot phien lam viec.
        ///
        /// HisConfigs cuc bo chi giu VALUE nen phai goi api de lay MODIFY_TIME. HisConfigFilter khong co
        /// bo loc theo KEY, chi co KEY_WORD (tim chuoi con tren KEY/VALUE/DEFAULT_VALUE/DESCRIPTION) nen
        /// phai loc lai chinh xac theo KEY o phia nay.
        /// </summary>
        private long GetRequiredDocumentConfigTime()
        {
            try
            {
                if (requiredDocumentConfigTimeCache.HasValue)
                    return requiredDocumentConfigTimeCache.Value;

                requiredDocumentConfigTimeCache = 0;

                CommonParam paramGet = new CommonParam();
                MOS.Filter.HisConfigFilter filter = new MOS.Filter.HisConfigFilter();
                filter.KEY_WORD = HisConfigKeys.HIS_CONFIG_KEY__RequiredDocument;

                var configs = new BackendAdapter(paramGet).Get<List<HIS_CONFIG>>(
                    URI__HIS_CONFIG_GET, ApiConsumers.MosConsumer, filter, paramGet);
                if (configs == null || configs.Count == 0)
                {
                    TraceCheckRequiredDocument("khong tim thay dong cau hinh so phut tren HIS_CONFIG -> bo qua quy tac khong hoi to.");
                    return 0;
                }

                var config = configs.FirstOrDefault(o => o != null && o.KEY == HisConfigKeys.HIS_CONFIG_KEY__RequiredDocument);
                if (config == null)
                {
                    TraceCheckRequiredDocument("khong khop chinh xac KEY cau hinh so phut -> bo qua quy tac khong hoi to.");
                    return 0;
                }

                long configTime = config.MODIFY_TIME ?? config.CREATE_TIME ?? 0;
                requiredDocumentConfigTimeCache = configTime;
                TraceCheckRequiredDocument("moc khai bao cau hinh so phut = " + configTime + ".");
                return configTime;
            }
            catch (Exception ex)
            {
                // Khong xac dinh duoc moc thi bo qua quy tac khong hoi to, van kiem tra binh thuong.
                Inventec.Common.Logging.LogSystem.Error(LOG__CHECK_REQUIRED_DOCUMENT + "loi khi tra cuu moc khai bao cau hinh.", ex);
                requiredDocumentConfigTimeCache = 0;
                return 0;
            }
        }

        /// <summary>
        /// Cac loai van ban dang hoat dong duoc tich "Hoan thanh khi vao khoa".
        /// Tra cuu moi lan kiem tra de quan tri tich/bo tich tren danh muc la co hieu luc ngay.
        /// </summary>
        private List<RequiredDocumentTypeADO> GetRequiredWhenInDepartmentDocumentTypes()
        {
            try
            {
                CommonParam paramType = new CommonParam();
                EmrDocumentTypeFilter filter = new EmrDocumentTypeFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;

                var documentTypes = new BackendAdapter(paramType).Get<List<RequiredDocumentTypeADO>>(
                    URI__EMR_DOCUMENT_TYPE_GET, ApiConsumers.EmrConsumer, filter, paramType);
                if (documentTypes == null || documentTypes.Count == 0)
                {
                    TraceCheckRequiredDocument("api " + URI__EMR_DOCUMENT_TYPE_GET + " tra ve "
                        + (documentTypes == null ? "null" : "rong") + ", khong xac dinh duoc loai van ban bat buoc.");
                    return null;
                }

                List<RequiredDocumentTypeADO> requiredTypes = documentTypes
                    .Where(o => o != null && o.IS_REQUIRED_WHEN_IN_DEPARTMENT == 1)
                    .ToList();
                TraceCheckRequiredDocument(string.Format(
                    "co {0}/{1} loai van ban dang hoat dong duoc tich \"Hoan thanh khi vao khoa\": [{2}].",
                    requiredTypes.Count,
                    documentTypes.Count,
                    string.Join(", ", requiredTypes.Select(o => string.Format("{0}#{1}#{2}", o.ID, o.DOCUMENT_TYPE_CODE, o.DOCUMENT_TYPE_NAME)))));
                return requiredTypes;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(LOG__CHECK_REQUIRED_DOCUMENT + "loi khi tra cuu danh muc loai van ban.", ex);
                return null;
            }
        }

        /// <summary>Ket qua kiem tra ho so benh an. Tra ve null khi goi api that bai.</summary>
        private RequiredDocumentCheckingResultADO GetRequiredDocumentChecking(string treatmentCode)
        {
            try
            {
                CommonParam paramCheck = new CommonParam();
                var checkingResult = new BackendAdapter(paramCheck).Post<RequiredDocumentCheckingResultADO>(
                    URI__EMR_DOCUMENT_MEDI_RECORD_CHECKING, ApiConsumers.EmrConsumer, treatmentCode, paramCheck);
                return checkingResult;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(LOG__CHECK_REQUIRED_DOCUMENT + "loi khi kiem tra ho so benh an.", ex);
                return null;
            }
        }

        /// <summary>
        /// Toan bo van ban chua xoa cua ho so. Tra ve danh sach rong khi ho so chua co van ban nao,
        /// tra ve null khi goi api that bai - hai truong hop nay KHAC nhau: rong thi van canh bao duoc,
        /// null thi phai bo qua phep kiem tra.
        /// </summary>
        private List<RequiredDocumentADO> GetTreatmentDocumentsForRequiredCheck(string treatmentCode)
        {
            try
            {
                CommonParam paramDoc = new CommonParam();
                EmrDocumentViewFilter filter = new EmrDocumentViewFilter();
                filter.TREATMENT_CODE__EXACT = treatmentCode;

                var documents = new BackendAdapter(paramDoc).Get<List<RequiredDocumentADO>>(
                    URI__EMR_DOCUMENT_GET_VIEW, ApiConsumers.EmrConsumer, filter, paramDoc);
                if (documents == null)
                {
                    // api tra ve null khi ho so chua co van ban nao, khong phan biet duoc voi loi goi api.
                    // Ket qua MediRecordChecking o buoc truoc da xac nhan dich vu EMR con song nen coi la rong.
                    TraceCheckRequiredDocument("api " + URI__EMR_DOCUMENT_GET_VIEW
                        + " tra ve null (ma dieu tri " + treatmentCode + "): coi nhu ho so chua co van ban nao.");
                    return new List<RequiredDocumentADO>();
                }

                int countFromApi = documents.Count;
                documents = documents.Where(o => o != null && o.IS_DELETE != 1).ToList();
                TraceCheckRequiredDocument(string.Format(
                    "api {0} tra ve {1} van ban, con {2} van ban chua xoa (IS_DELETE khac 1).",
                    URI__EMR_DOCUMENT_GET_VIEW, countFromApi, documents.Count));
                return documents;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(LOG__CHECK_REQUIRED_DOCUMENT + "loi khi tra cuu van ban cua ho so.", ex);
                return null;
            }
        }

        /// <summary>ID cac van ban chua hoan thanh chu ky theo ket qua kiem tra ho so.</summary>
        private HashSet<long> GetUnfinishedDocumentIdsForRequiredCheck(RequiredDocumentCheckingResultADO checkingResult)
        {
            return new HashSet<long>((checkingResult.SignatureMissingDocuments ?? new List<RequiredDocumentADO>())
                .Where(o => o != null)
                .Select(o => o.ID));
        }

        /// <summary>
        /// Van ban da hoan thanh chua (QT7).
        ///
        /// KHONG chi tin vao SignatureMissingDocuments cua ket qua kiem tra ho so: phep kiem tra do bo qua
        /// nhung van ban khong nam trong luong kiem tra ho so cua EMR, nen mot van ban dang con nguoi phai ky
        /// van co the KHONG xuat hien trong danh sach do. Dung dung bon tin hieu ma dien "Chan nhap vien"
        /// dang dung, de hai tinh nang cho ket qua nhat quan:
        ///  - ket qua kiem tra ho so bao thieu chu ky;
        ///  - con NEXT_SIGNER (den luot nguoi khac phai ky);
        ///  - SIGNERS rong (chua ai ky);
        ///  - con REJECTER (co nguoi tu choi ky).
        /// </summary>
        private bool IsDocumentUnfinishedForRequiredCheck(RequiredDocumentADO document, HashSet<long> apiUnfinishedDocumentIds)
        {
            if (document == null)
                return false;

            return apiUnfinishedDocumentIds.Contains(document.ID)
                || !string.IsNullOrWhiteSpace(document.NEXT_SIGNER)
                || string.IsNullOrWhiteSpace(document.SIGNERS)
                || !string.IsNullOrWhiteSpace(document.REJECTER);
        }
    }
}
