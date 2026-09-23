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
using System.Collections.Generic;

namespace HIS.Desktop.Plugins.ExportXmlQD130.ADO
{
    /// <summary>
    /// Trang thai tong the cua mot ho so sau khi soat loi tren MDInsight.
    /// Gia tri KHOP VOI cot HIS_TREATMENT.XML_PRECHECK_RESULT - doi o day la doi hop dong du lieu.
    /// Tham chieu: PTTK muc B.2.2 - bang "Gia tri cua cot trang thai tong the".
    /// </summary>
    public enum EnumXmlPrecheckStatus
    {
        /// <summary>Chua tung gui kiem tra - cot trong CSDL de trong, khong luu gia tri 0</summary>
        NotChecked = 0,

        /// <summary>Khong co loi - ket qua cuoi</summary>
        NoError = 1,

        /// <summary>Co canh bao, khong co loi nghiem trong - ket qua cuoi</summary>
        Warning = 2,

        /// <summary>Co loi nghiem trong - ket qua cuoi</summary>
        Critical = 3,

        /// <summary>Da gui thanh cong nhung vong tra chua co ket qua - TAM THOI, lay lai duoc</summary>
        Pending = 4,

        /// <summary>Khong kiem tra duoc - TRANG THAI CUOI, muon co ket qua phai gui lai</summary>
        CheckFailed = 5
    }

    /// <summary>
    /// Ly do khong kiem tra duoc. Chi co nghia khi trang thai la Pending hoac CheckFailed.
    /// Dung de sinh cau ly do bang ngon ngu nghiep vu - TUYET DOI khong chua du lieu benh nhan.
    /// </summary>
    public enum EnumMdInsightFailReason
    {
        None = 0,

        /// <summary>Chua khai bao ket noi hoac khai thieu thanh phan bat buoc</summary>
        NotConfigured = 1,

        /// <summary>Co so kham chua benh cua ho so chua duoc khai tai khoan - quy tac QT-09b</summary>
        NoAccountForMediOrg = 2,

        /// <summary>Ho so thieu du lieu bat buoc nen khong sinh duoc tep XML - quy tac QT-08</summary>
        ExportFailed = 3,

        /// <summary>Gui tep len he ngoai that bai</summary>
        UploadFailed = 4,

        /// <summary>Sai thong tin dang nhap hoac he ngoai tu choi xac thuc</summary>
        Unauthorized = 5,

        /// <summary>Mot luot goi qua han muc thoi gian</summary>
        Timeout = 6,

        /// <summary>He ngoai bao loi hoac loi ket noi mang</summary>
        SystemError = 7,

        /// <summary>Da gui nhung qua han giu trang thai Chua co ket qua (mac dinh 24 gio)</summary>
        PendingExpired = 8,

        /// <summary>Vong tra chua co ket qua nhung chua du thoi gian giam dinh toi thieu</summary>
        NotReadyYet = 9
    }

    /// <summary>
    /// Mot tai khoan MDInsight, gan chat voi MOT co so kham chua benh.
    /// Kiem chung ngay 2026-09-14: tai khoan bi CO LAP theo co so, khong doc duoc du lieu co so khac.
    /// </summary>
    public class MdInsightAccountADO
    {
        /// <summary>Ma co so kham chua benh - doi chieu voi MEDI_ORG_CODE cua ho so</summary>
        public string MediOrgCode { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }
    }

    /// <summary>
    /// Mot dong loi do MDInsight tra ve.
    ///
    /// SAU thanh phan dau duoc luu xuong cot XML_PRECHECK_DESC.
    /// Rieng <see cref="FullDescription"/> KHONG duoc luu xuong CSDL va KHONG duoc ghi nhat ky:
    /// he ngoai nhet ho ten benh nhan, ma benh nhan va chan doan vao truong nay.
    /// Tham chieu: PTTK muc B.2.2 - "Hop dong du lieu cua cot chi tiet loi", quy tac QT-28.
    /// </summary>
    public class MdInsightErrorADO
    {
        /// <summary>Muc do nghiem trong. Dong khong mang co muc do thi xep canh bao - quy tac QT-12</summary>
        public bool IsCritical { get; set; }

        /// <summary>Tep thanh phan chua loi (XML1, XML2...)</summary>
        public string FileName { get; set; }

        /// <summary>The du lieu chua loi</summary>
        public string TagName { get; set; }

        /// <summary>So thu tu dong trong tep thanh phan</summary>
        public string RowIndex { get; set; }

        /// <summary>Gia tri hien tai cua the - de nguoi sua doi chieu</summary>
        public string CurrentValue { get; set; }

        /// <summary>Noi dung loi</summary>
        public string ErrorContent { get; set; }

        /// <summary>
        /// Dien giai day du do he ngoai tra ve (truong errorDesc).
        /// CHUA DU LIEU BENH NHAN - chi ton tai trong phien, khong luu, khong ghi nhat ky.
        /// </summary>
        public string FullDescription { get; set; }
    }

    /// <summary>
    /// Ket qua soat loi cua mot ho so dieu tri.
    /// Vua la don vi hien thi tren luoi, vua la don vi gui xuong giao dien luu ket qua.
    /// </summary>
    public class MdInsightResultADO
    {
        public MdInsightResultADO()
        {
            this.Errors = new List<MdInsightErrorADO>();
            this.Status = EnumXmlPrecheckStatus.NotChecked;
            this.FailReason = EnumMdInsightFailReason.None;
        }

        public long TreatmentId { get; set; }

        public string TreatmentCode { get; set; }

        /// <summary>Ten benh nhan - lay tu dong tuong ung tren luoi chinh, CHI de hien thi</summary>
        public string PatientName { get; set; }

        /// <summary>Ma co so kham chua benh cua ho so - khoa de chon tai khoan</summary>
        public string MediOrgCode { get; set; }

        /// <summary>Ten tep da gui - khoa tra cuu ben MDInsight, quy tac QT-06</summary>
        public string SentFileName { get; set; }

        /// <summary>Thoi diem gui, yyyyMMddHHmmss. Chi dien o luot ghi nhan da gui</summary>
        public long? SendTime { get; set; }

        /// <summary>Thoi diem chot ket luan, yyyyMMddHHmmss. De trong khi trang thai la 4</summary>
        public long? CheckTime { get; set; }

        public EnumXmlPrecheckStatus Status { get; set; }

        /// <summary>Tong so dong loi - do tram dem san, quy tac QT-13</summary>
        public long? ErrorNum { get; set; }

        /// <summary>So dong loi nghiem trong - do tram dem san, luon &lt;= ErrorNum</summary>
        public long? CriticalNum { get; set; }

        /// <summary>Ly do dang van ban nghiep vu khi trang thai la 4 hoac 5. Khong chua du lieu benh nhan</summary>
        public string Reason { get; set; }

        public EnumMdInsightFailReason FailReason { get; set; }

        /// <summary>Danh sach dong loi. Rong khi trang thai la 1, 4 hoac 5</summary>
        public List<MdInsightErrorADO> Errors { get; set; }

        /// <summary>Danh sach loi da bi cat bot khi luu do vuot suc chua cot</summary>
        public bool IsTruncated { get; set; }

        /// <summary>So dong loi bi cat bot - chi co nghia khi IsTruncated</summary>
        public int TruncatedCount { get; set; }

        /// <summary>Trang thai da chot, khong con doi nua</summary>
        public bool IsFinalStatus
        {
            get
            {
                return this.Status == EnumXmlPrecheckStatus.NoError
                    || this.Status == EnumXmlPrecheckStatus.Warning
                    || this.Status == EnumXmlPrecheckStatus.Critical
                    || this.Status == EnumXmlPrecheckStatus.CheckFailed;
            }
        }
    }
}
