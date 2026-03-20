using System.Xml.Serialization;

namespace HIS.Desktop.Plugins.EmpUser.XMLData
{
    /// <summary>
    /// Base ADO (nếu sau này cần dùng chung cho nhiều XML khác).
    /// Hiện không bắt buộc, nhưng giữ lại để dễ mở rộng.
    /// </summary>
    public class EmployeeAdo
    {
        public int Stt { get; set; }
        public string MaCoSoKCB { get; set; }
    }

    /// <summary>
    /// ADO trung gian sinh từ HIS_EMPLOYEE theo thiết kế TT12.
    /// Mỗi property tương ứng với 1 chỉ tiêu trong bảng TT.
    /// </summary>
    public class EmployeeTT12Ado : EmployeeAdo
    {
        // 24 chỉ tiêu
        public int STT { get; set; }                // 1
        public string MA_KHOA { get; set; }        // 2
        public string TEN_KHOA { get; set; }       // 3
        public string HO_TEN { get; set; }         // 4
        public string GIOI_TINH { get; set; }      // 5  (1/2/3)
        public string SO_DINH_DANH { get; set; }   // 6
        public string CHUCDANH_NN { get; set; }    // 7
        public string VI_TRI { get; set; }         // 8
        public string MACCHN { get; set; }         // 9
        public string NGAYCAP_CCHN { get; set; }   // 10 (yyyyMMdd)
        public string NOICAP_CCHN { get; set; }    // 11
        public string PHAMVI_CM { get; set; }      // 12
        public string PHAMVI_CMBS { get; set; }    // 13
        public string DVKT_KHAC { get; set; }      // 14
        public string VB_PHANCONG { get; set; }    // 15
        public string THOIGIAN_DK { get; set; }    // 16 (1/2)
        public string THOIGIAN_NGAY { get; set; }  // 17
        public string THOIGIAN_TUAN { get; set; }  // 18
        public string CSKCB_KHAC { get; set; }     // 19
        public string CSKCB_CGKT { get; set; }     // 20
        public string QD_CGKT { get; set; }        // 21
        public string TU_NGAY { get; set; }        // 22 (yyyyMMdd)
        public string DEN_NGAY { get; set; }       // 23 (yyyyMMdd)
        public string MA_CSKCB { get; set; }       // 24

        #region Alias (cho code C# dễ đọc, không serialize)
        [XmlIgnore]
        public int SttTT12 { get => STT; set => STT = value; }

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
        public string MaCoSoKCBTT12 { get => MA_CSKCB; set => MA_CSKCB = value; }
        #endregion
    }
}