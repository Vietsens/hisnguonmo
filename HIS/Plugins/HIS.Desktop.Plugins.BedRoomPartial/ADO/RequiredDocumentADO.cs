using System.Collections.Generic;

namespace HIS.Desktop.Plugins.BedRoomPartial.ADO
{
    /// <summary>
    /// Ket qua tra ve cua api "api/EmrDocument/MediRecordChecking" (EMR.SDO.MediRecordCheckingResultSDO).
    ///
    /// Khai bao lai o day thay vi tham chieu truc tiep EMR.SDO vi kieu phan tu cua
    /// SignatureMissingDocuments trong EMR.SDO la EMR.EFMODEL.DataModels.V_EMR_DOCUMENT,
    /// ma ban EMR.EFMODEL.dll trong lib\EMR khong con chua cac kieu view V_*.
    /// Ban tin di tren day la JSON nen chi can trung ten thuoc tinh la Newtonsoft map duoc.
    /// </summary>
    public class RequiredDocumentCheckingResultADO
    {
        /// <summary>Cac van ban chua hoan thanh chu ky.</summary>
        public List<RequiredDocumentADO> SignatureMissingDocuments { get; set; }

        /// <summary>Ten cac van ban thuoc dien "Bat buoc" nhung chua duoc tao. Khong dung o day.</summary>
        public List<string> MandatoryMissingDocuments { get; set; }
    }

    /// <summary>
    /// Mot van ban EMR (V_EMR_DOCUMENT). Dung cho ca ket qua "api/EmrDocument/MediRecordChecking"
    /// va "api/EmrDocument/GetView" vi hai api tra ve cung kieu phan tu.
    /// Chi khai bao cac truong duoc su dung; cac truong con lai bi bo qua khi deserialize.
    /// </summary>
    public class RequiredDocumentADO
    {
        public long ID { get; set; }

        public string DOCUMENT_CODE { get; set; }

        public string DOCUMENT_NAME { get; set; }

        /// <summary>Khoa ngoai toi EMR_DOCUMENT_TYPE.</summary>
        public long? DOCUMENT_TYPE_ID { get; set; }

        /// <summary>
        /// Co xoa mem. Tren V_EMR_DOCUMENT day la NUMBER nullable (short?), KHONG phai boolean:
        /// van ban binh thuong de NULL chu khong phai 0, nen "chua xoa" = khac 1.
        /// </summary>
        public short? IS_DELETE { get; set; }

        /// <summary>Danh sach loginname da ky, phan tach boi ",". Rong = chua ai ky.</summary>
        public string SIGNERS { get; set; }

        /// <summary>Loginname nguoi phai ky tiep theo. Co gia tri = con nguoi phai ky.</summary>
        public string NEXT_SIGNER { get; set; }

        /// <summary>Loginname nguoi tu choi ky. Co gia tri = van ban bi tu choi.</summary>
        public string REJECTER { get; set; }
    }

    /// <summary>
    /// Gia tri khoa cau hinh "RequiredDocument", dang "{so phut}|{co kiem ca benh nhan cu}".
    ///
    ///   "30"    -> kiem tra tu phut thu 30, CHI voi benh nhan vao khoa TU LUC bat cau hinh tro di;
    ///   "30|1"  -> kiem tra tu phut thu 30, voi MOI benh nhan con dang dieu tri, ke ca nguoi da
    ///              nam khoa tu truoc khi bat cau hinh.
    ///
    /// Chi dung "1" o doan sau moi bo luat hoi to. Thieu doan sau, hoac ghi gia tri khac,
    /// deu giu luat hoi to — day la hanh vi mac dinh, dung Muc 3.5 cua yeu cau goc va tranh
    /// canh bao hang loat trong ngay dau bat cau hinh.
    /// </summary>
    public class RequiredDocumentConfigADO
    {
        /// <summary>
        /// So phut ke tu khi nhap vien vao khoa thi bat dau kiem tra.
        /// 0 = khong kiem tra (chua khai bao, khong phai so, hoac so am / so 0).
        /// </summary>
        public int CheckMinutes { get; set; }

        /// <summary>
        /// true  = kiem ca benh nhan vao khoa TRUOC luc bat cau hinh (doan sau dau "|" la "1");
        /// false = chi kiem benh nhan vao khoa tu luc bat cau hinh tro di (mac dinh).
        /// </summary>
        public bool IsCheckPatientAdmittedBeforeConfig { get; set; }

        /// <summary>Tinh nang co dang bat khong. Chi can so phut hop le la bat.</summary>
        public bool IsEnabled
        {
            get { return this.CheckMinutes > 0; }
        }
    }

    /// <summary>
    /// Mot dong danh muc Loai van ban (EMR_DOCUMENT_TYPE) lay qua "api/EmrDocumentType/Get".
    ///
    /// Khai bao lai thay vi dung truc tiep EMR.EFMODEL.DataModels.EMR_DOCUMENT_TYPE de phep kiem tra nay
    /// bien dich va chay duoc NGAY CA KHI chua them cot IS_REQUIRED_WHEN_IN_DEPARTMENT va chua gencode lai
    /// EMR.EFMODEL.dll: khi do ban tin khong co truong nay, Newtonsoft de null, khong loai van ban nao vao
    /// dien kiem tra -> tinh nang nam im. Do la dung trang thai mac dinh an toan cho cac vien chua bat.
    /// </summary>
    public class RequiredDocumentTypeADO
    {
        public long ID { get; set; }

        public string DOCUMENT_TYPE_CODE { get; set; }

        public string DOCUMENT_TYPE_NAME { get; set; }

        /// <summary>
        /// Co "Hoan thanh khi vao khoa" tren danh muc Loai van ban.
        /// 1 = loai van ban nay bat buoc phai hoan thanh khi benh nhan vao khoa.
        /// </summary>
        public short? IS_REQUIRED_WHEN_IN_DEPARTMENT { get; set; }
    }
}
