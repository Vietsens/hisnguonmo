using System;
using System.Xml.Serialization;

namespace HIS.Desktop.Plugins.EmpUser.XMLData
{
    /// <summary>
    /// Model chi tiết 24 trường TT12 dùng để serialize ra XML.
    /// Mỗi property mapping 1 thẻ XML đúng tên chỉ tiêu TT.
    /// </summary>
    [Serializable]
    public class XMLEmployeeTT12DetailData
    {
        [XmlElement("STT", Order = 1)]
        public int STT { get; set; }

        [XmlElement("MA_KHOA", Order = 2)]
        public string MA_KHOA { get; set; }

        [XmlElement("TEN_KHOA", Order = 3)]
        public string TEN_KHOA { get; set; }

        [XmlElement("HO_TEN", Order = 4)]
        public string HO_TEN { get; set; }

        [XmlElement("GIOI_TINH", Order = 5)]
        public string GIOI_TINH { get; set; }

        [XmlElement("SO_DINH_DANH", Order = 6)]
        public string SO_DINH_DANH { get; set; }

        [XmlElement("CHUCDANH_NN", Order = 7)]
        public string CHUCDANH_NN { get; set; }

        [XmlElement("VI_TRI", Order = 8)]
        public string VI_TRI { get; set; }

        [XmlElement("MACCHN", Order = 9)]
        public string MACCHN { get; set; }

        [XmlElement("NGAYCAP_CCHN", Order = 10)]
        public string NGAYCAP_CCHN { get; set; }

        [XmlElement("NOICAP_CCHN", Order = 11)]
        public string NOICAP_CCHN { get; set; }

        [XmlElement("PHAMVI_CM", Order = 12)]
        public string PHAMVI_CM { get; set; }

        [XmlElement("PHAMVI_CMBS", Order = 13)]
        public string PHAMVI_CMBS { get; set; }

        [XmlElement("DVKT_KHAC", Order = 14)]
        public string DVKT_KHAC { get; set; }

        [XmlElement("VB_PHANCONG", Order = 15)]
        public string VB_PHANCONG { get; set; }

        [XmlElement("THOIGIAN_DK", Order = 16)]
        public string THOIGIAN_DK { get; set; }

        [XmlElement("THOIGIAN_NGAY", Order = 17)]
        public string THOIGIAN_NGAY { get; set; }

        [XmlElement("THOIGIAN_TUAN", Order = 18)]
        public string THOIGIAN_TUAN { get; set; }

        [XmlElement("CSKCB_KHAC", Order = 19)]
        public string CSKCB_KHAC { get; set; }

        [XmlElement("CSKCB_CGKT", Order = 20)]
        public string CSKCB_CGKT { get; set; }

        [XmlElement("QD_CGKT", Order = 21)]
        public string QD_CGKT { get; set; }

        [XmlElement("TU_NGAY", Order = 22)]
        public string TU_NGAY { get; set; }

        [XmlElement("DEN_NGAY", Order = 23)]
        public string DEN_NGAY { get; set; }

        [XmlElement("MA_CSKCB", Order = 24)]
        public string MA_CSKCB { get; set; }

        #region Alias (không serialize, dùng cho code)
        [XmlIgnore]
        public int Stt { get => STT; set => STT = value; }

        [XmlIgnore]
        public string MaKhoa { get => MA_KHOA; set => MA_KHOA = value; }

        [XmlIgnore]
        public string TenKhoa { get => TEN_KHOA; set => TEN_KHOA = value; }

        [XmlIgnore]
        public string HoTen { get => HO_TEN; set => HO_TEN = value; }

        [XmlIgnore]
        public string GioiTinh { get => GIOI_TINH; set => GIOI_TINH = value; }

        [XmlIgnore]
        public string SoDinhDanh { get => SO_DINH_DANH; set => SO_DINH_DANH = value; }

        [XmlIgnore]
        public string ChucDanhNN { get => CHUCDANH_NN; set => CHUCDANH_NN = value; }

        [XmlIgnore]
        public string ViTri { get => VI_TRI; set => VI_TRI = value; }

        [XmlIgnore]
        public string MaCCHN { get => MACCHN; set => MACCHN = value; }

        [XmlIgnore]
        public string NgayCapCCHN { get => NGAYCAP_CCHN; set => NGAYCAP_CCHN = value; }

        [XmlIgnore]
        public string NoiCapCCHN { get => NOICAP_CCHN; set => NOICAP_CCHN = value; }

        [XmlIgnore]
        public string PhamViCM { get => PHAMVI_CM; set => PHAMVI_CM = value; }

        [XmlIgnore]
        public string PhamViCMBS { get => PHAMVI_CMBS; set => PHAMVI_CMBS = value; }

        [XmlIgnore]
        public string DvktKhac { get => DVKT_KHAC; set => DVKT_KHAC = value; }

        [XmlIgnore]
        public string VanBanPhanCong { get => VB_PHANCONG; set => VB_PHANCONG = value; }

        [XmlIgnore]
        public string ThoiGianDangKy { get => THOIGIAN_DK; set => THOIGIAN_DK = value; }

        [XmlIgnore]
        public string ThoiGianNgay { get => THOIGIAN_NGAY; set => THOIGIAN_NGAY = value; }

        [XmlIgnore]
        public string ThoiGianTuan { get => THOIGIAN_TUAN; set => THOIGIAN_TUAN = value; }

        [XmlIgnore]
        public string CskcbKhac { get => CSKCB_KHAC; set => CSKCB_KHAC = value; }

        [XmlIgnore]
        public string CskcbCgkt { get => CSKCB_CGKT; set => CSKCB_CGKT = value; }

        [XmlIgnore]
        public string QdCgkt { get => QD_CGKT; set => QD_CGKT = value; }

        [XmlIgnore]
        public string TuNgayTT12 { get => TU_NGAY; set => TU_NGAY = value; }

        [XmlIgnore]
        public string DenNgayTT12 { get => DEN_NGAY; set => DEN_NGAY = value; }

        [XmlIgnore]
        public string MaCoSoKCB { get => MA_CSKCB; set => MA_CSKCB = value; }
        #endregion
    }
}