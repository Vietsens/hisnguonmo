/* IVT — HIS.Desktop.Plugins.InfectiousDiseaseReport
 * Ca bệnh gửi lên cổng ECDS — KHỚP schema "DiseaseCaseFastDto" (Swagger:
 *   POST /api/fast/v1/ca-benh/cap-nhat  (đẩy 1 ca)
 *   POST /api/fast/v1/ca-benh/cap-nhat-nhieu  (đẩy danh sách — mảng thô DTO này)
 * Nguồn: https://daotao-gs.vadp.gov.vn/public/swagger-ui/index.html — mục "Ca bệnh".
 *
 * QUY ƯỚC QUAN TRỌNG (khác bản cũ):
 *  - Tên trường camelCase (theo cổng), KHÔNG phải UPPER_SNAKE.
 *  - Danh mục truyền MÃ (string) — VD maIcd10Benh="A97", maDanToc="1" — KHÔNG phải ID nội bộ.
 *    (Đây là lý do bản cũ đẩy ID=0 khiến cổng báo "bạn phải chọn bệnh".)
 *  - Ngày định dạng "dd/MM/yyyy".
 *  - Field null -> KHÔNG serialize (NullValueHandling.Ignore).
 */
using Newtonsoft.Json;

namespace HIS.Desktop.Plugins.InfectiousDiseaseReport.ADO
{
    public class EcdsDiseaseCaseDto
    {
        // ------- Định danh (update ca đã có trên cổng) -------
        /// <summary>ID (UUID) ca bệnh trên cổng — set khi đẩy lại để cập nhật; null khi tạo mới.</summary>
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)] public string Id { get; set; }

        // ------- Bệnh / chẩn đoán -------
        /// <summary>BẮT BUỘC — mã ICD-10 của bệnh (VD "A97").</summary>
        [JsonProperty("maIcd10Benh")] public string MaIcd10Benh { get; set; }
        /// <summary>Mã phân loại lâm sàng (VD "A91.1").</summary>
        [JsonProperty("maPhanLoaiLamSang", NullValueHandling = NullValueHandling.Ignore)] public string MaPhanLoaiLamSang { get; set; }
        /// <summary>Loại chẩn đoán (EcdsPhanLoaiChuanDoan).</summary>
        [JsonProperty("loaiChanDoan", NullValueHandling = NullValueHandling.Ignore)] public int? LoaiChanDoan { get; set; }
        /// <summary>Trạng thái ca bệnh (EcdsTrangThaiCaBenh).</summary>
        [JsonProperty("trangThaiCaBenh", NullValueHandling = NullValueHandling.Ignore)] public int? TrangThaiCaBenh { get; set; }
        /// <summary>Trạng thái lưu (EcdsTrangThaiLuu — 2=Lưu chính thức).</summary>
        [JsonProperty("trangThaiLuu", NullValueHandling = NullValueHandling.Ignore)] public int? TrangThaiLuu { get; set; }

        // ------- Hành chính bệnh nhân -------
        /// <summary>BẮT BUỘC — họ và tên.</summary>
        [JsonProperty("hoVaTen")] public string HoVaTen { get; set; }
        /// <summary>BẮT BUỘC — ngày sinh "dd/MM/yyyy".</summary>
        [JsonProperty("ngaySinh")] public string NgaySinh { get; set; }
        [JsonProperty("tuoi", NullValueHandling = NullValueHandling.Ignore)] public int? Tuoi { get; set; }
        /// <summary>BẮT BUỘC — mã giới tính "M"/"F".</summary>
        [JsonProperty("maGioiTinh")] public string MaGioiTinh { get; set; }
        [JsonProperty("dangMangThai", NullValueHandling = NullValueHandling.Ignore)] public bool? DangMangThai { get; set; }
        [JsonProperty("maDanToc", NullValueHandling = NullValueHandling.Ignore)] public string MaDanToc { get; set; }
        [JsonProperty("maNgheNghiep", NullValueHandling = NullValueHandling.Ignore)] public string MaNgheNghiep { get; set; }
        [JsonProperty("noiLamViec", NullValueHandling = NullValueHandling.Ignore)] public string NoiLamViec { get; set; }
        [JsonProperty("soCccdCmnd", NullValueHandling = NullValueHandling.Ignore)] public string SoCccdCmnd { get; set; }
        [JsonProperty("soDienThoai", NullValueHandling = NullValueHandling.Ignore)] public string SoDienThoai { get; set; }
        [JsonProperty("tenNguoiThan", NullValueHandling = NullValueHandling.Ignore)] public string TenNguoiThan { get; set; }

        // ------- Địa bàn hiện nay -------
        /// <summary>BẮT BUỘC — mã xã/phường hiện nay.</summary>
        [JsonProperty("maXaHienNay")] public string MaXaHienNay { get; set; }
        [JsonProperty("maThonHienNay", NullValueHandling = NullValueHandling.Ignore)] public string MaThonHienNay { get; set; }
        [JsonProperty("diaChiChiTietHienNay", NullValueHandling = NullValueHandling.Ignore)] public string DiaChiChiTietHienNay { get; set; }
        [JsonProperty("maXaPhuongQuanLy", NullValueHandling = NullValueHandling.Ignore)] public string MaXaPhuongQuanLy { get; set; }

        // ------- Diễn biến ca bệnh -------
        [JsonProperty("tinhTrangHienTai", NullValueHandling = NullValueHandling.Ignore)] public int? TinhTrangHienTai { get; set; }
        [JsonProperty("ngayKhoiPhat", NullValueHandling = NullValueHandling.Ignore)] public string NgayKhoiPhat { get; set; }
        [JsonProperty("ngayNhapVien", NullValueHandling = NullValueHandling.Ignore)] public string NgayNhapVien { get; set; }
        [JsonProperty("ngayRaVien", NullValueHandling = NullValueHandling.Ignore)] public string NgayRaVien { get; set; }
        [JsonProperty("chanDoanRaVien", NullValueHandling = NullValueHandling.Ignore)] public string ChanDoanRaVien { get; set; }
        [JsonProperty("thongTinTiemVacXin", NullValueHandling = NullValueHandling.Ignore)] public int? ThongTinTiemVacXin { get; set; }
        [JsonProperty("benhKemTheo", NullValueHandling = NullValueHandling.Ignore)] public string BenhKemTheo { get; set; }
        [JsonProperty("bienChung", NullValueHandling = NullValueHandling.Ignore)] public string BienChung { get; set; }
        [JsonProperty("ghiChuChung", NullValueHandling = NullValueHandling.Ignore)] public string GhiChuChung { get; set; }
        [JsonProperty("tienSuDichTe", NullValueHandling = NullValueHandling.Ignore)] public string TienSuDichTe { get; set; }

        // ------- Xét nghiệm -------
        [JsonProperty("coLayMauXetNghiem", NullValueHandling = NullValueHandling.Ignore)] public bool? CoLayMauXetNghiem { get; set; }
        [JsonProperty("tenXetNghiem", NullValueHandling = NullValueHandling.Ignore)] public string TenXetNghiem { get; set; }
        [JsonProperty("loaiXetNghiemChung", NullValueHandling = NullValueHandling.Ignore)] public int? LoaiXetNghiemChung { get; set; }
        [JsonProperty("ketQuaXetNghiemChung", NullValueHandling = NullValueHandling.Ignore)] public int? KetQuaXetNghiemChung { get; set; }
        [JsonProperty("ngayLayMau", NullValueHandling = NullValueHandling.Ignore)] public string NgayLayMau { get; set; }
        [JsonProperty("ngayTraKetQua", NullValueHandling = NullValueHandling.Ignore)] public string NgayTraKetQua { get; set; }
        [JsonProperty("maDonViXetNghiem", NullValueHandling = NullValueHandling.Ignore)] public string MaDonViXetNghiem { get; set; }

        // ------- Cơ sở điều trị -------
        [JsonProperty("maCoSoDieuTri", NullValueHandling = NullValueHandling.Ignore)] public string MaCoSoDieuTri { get; set; }
        [JsonProperty("maHinhThucDieuTri", NullValueHandling = NullValueHandling.Ignore)] public string MaHinhThucDieuTri { get; set; }

        // ------- Người báo cáo -------
        [JsonProperty("hoTenNguoiBaoCao", NullValueHandling = NullValueHandling.Ignore)] public string HoTenNguoiBaoCao { get; set; }
        [JsonProperty("soDienThoaiNguoiBaoCao", NullValueHandling = NullValueHandling.Ignore)] public string SoDienThoaiNguoiBaoCao { get; set; }
        [JsonProperty("emailNguoiBaoCao", NullValueHandling = NullValueHandling.Ignore)] public string EmailNguoiBaoCao { get; set; }
        [JsonProperty("maDonViNguoiBaoCao", NullValueHandling = NullValueHandling.Ignore)] public string MaDonViNguoiBaoCao { get; set; }
    }
}
