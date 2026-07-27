/* IVT — HIS.Desktop.Plugins.InfectiousDiseaseReport
 * Enum theo TÀI LIỆU CHÍNH THỨC cổng ECDS.
 * ⚠ LƯU Ý POLARITY: một số trường có 0 = Có (ngược trực giác) — xem chú thích.
 */
namespace HIS.Desktop.Plugins.InfectiousDiseaseReport
{
    /// <summary>Giới tính — trường GIOITINH.</summary>
    public enum EcdsGioiTinh
    {
        /// <summary>Nữ</summary>
        Nu = 0,
        /// <summary>Nam</summary>
        Nam = 1
    }

    /// <summary>Tình trạng mang thai — trường IS_MANGTHAI.</summary>
    public enum EcdsMangThai
    {
        /// <summary>Không mang thai</summary>
        Khong = 0,
        /// <summary>Có mang thai</summary>
        Co = 1
    }

    /// <summary>Tiêm/uống vắc xin — trường SUDUNGVACXIN. ⚠ POLARITY ĐẢO: 0 = Có.</summary>
    public enum EcdsSuDungVacXin
    {
        /// <summary>Có tiêm/uống vắc xin</summary>
        Co = 0,
        /// <summary>Không tiêm/uống</summary>
        Khong = 1,
        /// <summary>Không rõ</summary>
        KhongRo = 2
    }

    /// <summary>Phân loại chẩn đoán — trường PHANLOAICHUANDOAN.</summary>
    public enum EcdsPhanLoaiChuanDoan
    {
        /// <summary>Nghi ngờ</summary>
        NghiNgo = 0,
        /// <summary>Xác định</summary>
        XacDinh = 1
    }

    /// <summary>Có lấy mẫu xét nghiệm — trường LAYMAUXETNGHIEM. ⚠ POLARITY ĐẢO: 0 = Có.</summary>
    public enum EcdsLayMauXetNghiem
    {
        /// <summary>Có lấy mẫu</summary>
        Co = 0,
        /// <summary>Không lấy mẫu</summary>
        Khong = 1
    }

    /// <summary>Loại xét nghiệm — trường LOAIXETNGHIEM.</summary>
    public enum EcdsLoaiXetNghiem
    {
        /// <summary>Test nhanh</summary>
        TestNhanh = 0,
        /// <summary>Mac-ELISA</summary>
        MacElisa = 1,
        /// <summary>PCR</summary>
        Pcr = 2,
        /// <summary>Khác (nhập LOAIXETNGHIEMKHAC)</summary>
        Khac = 3
    }

    /// <summary>Kết quả xét nghiệm — trường KETQUAXETNGHIEM.</summary>
    public enum EcdsKetQuaXetNghiem
    {
        /// <summary>Dương tính</summary>
        DuongTinh = 0,
        /// <summary>Âm tính</summary>
        AmTinh = 1,
        /// <summary>Chưa có kết quả</summary>
        ChuaCoKetQua = 2
    }

    /// <summary>Tình trạng hiện nay — trường TINHTRANGHIENNAY.</summary>
    public enum EcdsTinhTrangHienNay
    {
        /// <summary>Ngoại trú</summary>
        NgoaiTru = 0,
        /// <summary>Nội trú</summary>
        NoiTru = 1,
        /// <summary>Ra viện</summary>
        RaVien = 2,
        /// <summary>Tử vong</summary>
        TuVong = 3,
        /// <summary>Chuyển viện</summary>
        ChuyenVien = 4,
        /// <summary>Khác (nhập TINHTRANGKHAC)</summary>
        Khac = 5
    }

    /// <summary>Loại cơ sở phát hiện/điều trị — trường LOAIPHATHIEN.</summary>
    public enum EcdsLoaiPhatHien
    {
        /// <summary>Trạm y tế</summary>
        TramYTe = 0,
        /// <summary>Tại nhà</summary>
        TaiNha = 1,
        /// <summary>Y tế cơ quan</summary>
        YTeCoQuan = 2,
        /// <summary>Khác</summary>
        Khac = 3
    }

    /// <summary>Trạng thái đẩy nội bộ HIS (cột PUSH_STATE của HIS_ECDS_DISEASE_CASE).</summary>
    public enum EcdsPushState
    {
        /// <summary>Chưa đẩy</summary>
        ChuaDay = 0,
        /// <summary>Đã đẩy thành công</summary>
        DaDay = 1,
        /// <summary>Đẩy lỗi</summary>
        Loi = 2
    }
}
