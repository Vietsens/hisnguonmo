/* IVT — HIS.Desktop.Plugins.InfectiousDiseaseSyncList
 * DTO ca bệnh gửi lên cổng ECDS — tên trường theo TÀI LIỆU CHÍNH THỨC (UPPER_SNAKE).
 * Dùng Newtonsoft [JsonProperty] để serialize đúng tên field API.
 * ⚠ Xác nhận endpoint (fast/v1 vs template import) trước khi dùng thật.
 */
using Newtonsoft.Json;

namespace HIS.Desktop.Plugins.InfectiousDiseaseSyncList.ADO
{
    public class EcdsDiseaseCaseDto
    {
        // ------- Định danh / đối soát -------
        /// <summary>ID ca bệnh trên cổng (cập nhật tránh trùng); rỗng khi tạo mới.</summary>
        [JsonProperty("ID")] public string Id { get; set; }

        // ------- Hành chính (nguồn HIS_PATIENT) -------
        [JsonProperty("HOTEN")] public string HoTen { get; set; }
        [JsonProperty("NGAYSINH")] public string NgaySinh { get; set; }            // yyyy-MM-dd
        [JsonProperty("GIOITINH")] public int GioiTinh { get; set; }               // EcdsGioiTinh
        [JsonProperty("DANTOC_ID")] public long? DanTocId { get; set; }            // danh mục dantoc
        [JsonProperty("CCCD")] public string Cccd { get; set; }
        [JsonProperty("IS_MANGTHAI")] public int? IsMangThai { get; set; }         // EcdsMangThai
        [JsonProperty("DIENTHOAI")] public string DienThoai { get; set; }
        [JsonProperty("NGHENGHIEP_ID")] public long? NgheNghiepId { get; set; }    // danh mục nghenghiep
        [JsonProperty("DIACHI")] public string DiaChi { get; set; }
        [JsonProperty("TINH_ID")] public long? TinhId { get; set; }               // danh mục tinh
        [JsonProperty("XA_ID")] public long? XaId { get; set; }                   // danh mục xa
        [JsonProperty("THON_ID")] public long? ThonId { get; set; }               // danh mục thon
        [JsonProperty("TINH_ID_THUONGTRU")] public long? TinhIdThuongTru { get; set; }
        [JsonProperty("XA_ID_THUONGTRU")] public long? XaIdThuongTru { get; set; }
        [JsonProperty("DIACHI_THUONGTRU")] public string DiaChiThuongTru { get; set; }
        [JsonProperty("NOILAMVIEC")] public string NoiLamViec { get; set; }

        // ------- Trường hợp bệnh -------
        [JsonProperty("SUDUNGVACXIN")] public int? SuDungVacXin { get; set; }      // EcdsSuDungVacXin (0=Có)
        [JsonProperty("SOLANSUDUNG")] public int? SoLanSuDung { get; set; }
        [JsonProperty("PHANLOAICHUANDOAN")] public int PhanLoaiChuanDoan { get; set; } // EcdsPhanLoaiChuanDoan
        [JsonProperty("DONVITHUCHIENXN")] public long? DonViThucHienXn { get; set; }   // danh mục coso
        [JsonProperty("LAYMAUXETNGHIEM")] public int? LayMauXetNghiem { get; set; }    // EcdsLayMauXetNghiem (0=Có)
        [JsonProperty("LOAIXETNGHIEM")] public int? LoaiXetNghiem { get; set; }        // EcdsLoaiXetNghiem
        [JsonProperty("LOAIXETNGHIEMKHAC")] public string LoaiXetNghiemKhac { get; set; }
        [JsonProperty("KETQUAXETNGHIEM")] public int? KetQuaXetNghiem { get; set; }    // EcdsKetQuaXetNghiem
        [JsonProperty("NGAYTHUCHIENXN")] public string NgayThucHienXn { get; set; }
        [JsonProperty("NGAYTRAKETQUAXN")] public string NgayTraKetQuaXn { get; set; }
        [JsonProperty("NGAYKHOIPHAT")] public string NgayKhoiPhat { get; set; }
        [JsonProperty("NGAYNHAPVIEN")] public string NgayNhapVien { get; set; }
        [JsonProperty("TINHTRANGHIENNAY")] public int TinhTrangHienNay { get; set; }   // EcdsTinhTrangHienNay
        [JsonProperty("BENHVIENCHUYENTOI")] public string BenhVienChuyenToi { get; set; }
        [JsonProperty("TINHTRANGKHAC")] public string TinhTrangKhac { get; set; }
        [JsonProperty("BENHVIENCHUYENTOI_ID")] public long? BenhVienChuyenToiId { get; set; } // coso
        [JsonProperty("NGAYRAVIEN")] public string NgayRaVien { get; set; }
        [JsonProperty("TINHTRANGRAVIEN")] public int? TinhTrangRaVien { get; set; }
        [JsonProperty("NGAYTUVONG")] public string NgayTuVong { get; set; }
        [JsonProperty("BENHCHUANDOAN_ID")] public long BenhChuanDoanId { get; set; }   // danh mục benhchuandoan
        [JsonProperty("DM_CAPDOBENH_ID")] public long? CapDoBenhId { get; set; }        // danh mục capdobenh (liên thông)
        [JsonProperty("BENHCHUANDOANPHU")] public string BenhChuanDoanPhu { get; set; }
        [JsonProperty("CHUANDOANBIENCHUNG")] public string ChuanDoanBienChung { get; set; }
        [JsonProperty("GHICHU")] public string GhiChu { get; set; }

        // ------- Người báo cáo -------
        [JsonProperty("NGUOIBAOCAO")] public string NguoiBaoCao { get; set; }
        [JsonProperty("EMAILNGUOIBAOCAO")] public string EmailNguoiBaoCao { get; set; }
        [JsonProperty("DIENTHOAINGUOIBAOCAO")] public string DienThoaiNguoiBaoCao { get; set; }
        [JsonProperty("LOAIPHATHIEN")] public int LoaiPhatHien { get; set; }           // EcdsLoaiPhatHien
        [JsonProperty("CHAN_DOAN_RA_VIEN")] public string ChanDoanRaVien { get; set; }
        [JsonProperty("TIEN_SU_DICH_TE")] public string TienSuDichTe { get; set; }
        [JsonProperty("CO_SO_DIEU_TRI")] public string CoSoDieuTri { get; set; }        // = HIS_BRANCH.BRANCH_NAME
    }
}
