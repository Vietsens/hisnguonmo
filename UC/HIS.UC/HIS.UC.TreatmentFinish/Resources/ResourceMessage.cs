using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.TreatmentFinish.Resources
{
    class ResourceMessage
    {
        static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.UC.TreatmentFinish.Resources.Message.Lang", System.Reflection.Assembly.GetExecutingAssembly());

        internal static string BanBatBuocChonDichVuHenKhamKhiKetThucDieuTri
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("BanBatBuocChonDichVuHenKhamKhiKetThucDieuTri", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        
        internal static string KhungGioVuotQuaSoLuong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("UC_ExamTreatmentFinish_KhungGioVuotQuaSoLuong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string PhongKhamCoSoLuotKhamVuotDinhMuc
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("UC_ExamTreatmentFinish_PhongKhamCoSoLuotKhamVuotDinhMuc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string CanhBaoNgayHenToiDa
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("UC_ExamTreatmentFinish_CanhBaoNgayHenToiDa", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ThoiGianKetThucKhongDuocNhoHonThoiGianYLenhCuaHoSo
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ThoiGianKetThucKhongDuocNhoHonThoiGianYLenhCuaHoSo", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ThuocDaduocKeVaoDanhSachThuocChobenNhanBanCoMuonChuyenSangBNKhac
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignPrescription__ThuocDaduocKeVaoDanhSachThuocChobenNhanBanCoMuonChuyenSangBNKhac", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string CanhBaoNgayHenLaChuNhat
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("CanhBaoNgayHenLaChuNhat", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string TruongThongTinCoDoDaiVuotQuaGioiHan
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("TruongThongTinCoDoDaiVuotQuaGioiHan", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string CanhBaoNgayHenLaThuBay
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("CanhBaoNgayHenLaThuBay", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }        

        internal static string CanhBaoThoiGianHenKhamSoVoiThoiGianKetThucDieuTri
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("CanhBaoThoiGianHenKhamSoVoiThoiGianKetThucDieuTri", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string TaiKhoanKhongCoQuyenThucHienChucNang
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("TaiKhoanKhongCoQuyenThucHienChucNang", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ChucNangDangPhatTrienVuiLongThuLaiSau
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ChucNangDangPhatTrienVuiLongThuLaiSau", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string HeThongTBCuaSoThongBaoBanCoMuonHuyDuLieuKhong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("HeThongTBCuaSoThongBaoBanCoMuonHuyDuLieuKhong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ChuaNhapThongTinTuVong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ChuaNhapThongTinTuVong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string TruongDuLieuVuotQuaKyTu
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("TruongDuLieuVuotQuaKyTu", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ChuaNhapThongTinChuyenVien
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ChuaNhapThongTinChuyenVien", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ChuaNhapThongTinBatBuocKhiChonNGhiViecHuongBHXH
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ChuaNhapThongTinBatBuocKhiChonNGhiViecHuongBHXH", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ChuaNhapThoiGianHenKham
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ChuaNhapThoiGianHenKham", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ThoiGianKetThucNhoHoiThoiGianYLenhCuaCacMaYeuCauSau
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ThoiGianKetThucNhoHoiThoiGianYLenhCuaCacMaYeuCauSau", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string CacPhieuChiDinhSauChuaKetThucKhongChoPhepKetThuc
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("CacPhieuChiDinhSauChuaKetThucKhongChoPhepKetThuc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ChuaLinhHetThuoc
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ChuaLinhHetThuoc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string BenhNhanNoiTruChuaDuocChiDinhDichVuGiuong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("BenhNhanNoiTruChuaDuocChiDinhDichVuGiuong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string SoNgayGiuongNhoHonSoNgayDieuTri
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("SoNgayGiuongNhoHonSoNgayDieuTri", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string SoNgayGiuongLonHonSoNgayDieuTri
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("SoNgayGiuongLonHonSoNgayDieuTri", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ThoiGianTuVongKhongDuocLonHonThoiGianHienTai
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ThoiGianTuVongKhongDuocLonHonThoiGianHienTai", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ThoiGianTuVongKhongDuocBeHonThoiGianVaoVien
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ThoiGianTuVongKhongDuocBeHonThoiGianVaoVien", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        
        internal static string TongTienHaoPhiVuotQuaDinhMucHaoPhiKhongTheChonDichVuNay
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignPrescription__TongTienHaoPhiVuotQuaDinhMucHaoPhiKhongTheChonDichVuNay", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ThuocDaChonTrongKhoKhongDuKe_BanComuonKeThuocNgoaiKho
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignPrescription__ThuocDaChonTrongKhoKhongDuKe_BanComuonKeThuocNgoaiKho", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string ThuocKhongCoHoatChat
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignPrescription__ThuocKhongCoHoatChat", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string TruongSoLuongNhapSaiDinhDang
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignPrescription__TruongSoLuongNhapSaiDinhDang", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string BuoiSang
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignPrescription__BuoiSang", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string BuoiTrua
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignPrescription__BuoiTrua", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string BuoiChieu
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignPrescription__BuoiChieu", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string BuoiToi
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignPrescription__BuoiToi", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string _NgayXVienBuoiYZ
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignPrescription__NgayXVienBuoiYZ", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string KhongTimThayIcdTuongUngVoiCacMaSau
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignPrescription__KhongTimThayIcdTuongUngVoiCacMaSau", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string KhongChoPhepChuyenVien
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignPrescription__KhongChoPhepChuyenVien", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string SuaDonThuocDuLieuTruyenVaoKhongHopLe
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignPrescription__SuaDonThuocDuLieuTruyenVaoKhongHopLe", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string ChuaChonNgayChiDinh
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignPrescription__ChuaChonNgayChiDinh", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string AcinInteractive__OverGrade
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignPrescription__AcinInteractive__OverGrade", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string ThuocDaduocKe
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignPrescription__ThuocDaduocKe", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string NgayUong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignPrescription__NgayUong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string Sang
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignPrescription__Sang", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string Trua
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignPrescription__Trua", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string Chieu
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignPrescription__Chieu", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string Toi
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignPrescription__Toi", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string CanhBaoVatTu
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignPrescription__VatTu", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string CanhBaoThuoc
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignPrescription__Thuoc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string SoTienDaKeChoBHYTDaVuotquaMucGioiHan
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignPrescription__SoTienDaKeChoBHYTDaVuotquaMucGioiHan", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string VuotQuaSoLuongKhaDungTrongKho__CoMuonTiepTuc
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignPrescription__Message__VuotQuaSoLuongKhaDungTrongKho__CoMuonTiepTuc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string KhongTimThayDoiTuongThanhToanTrongThoiGianYLenh
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignPrescription__KhongTimThayDoiTuongThanhToanTrongThoiGianYLenh", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string BenhNhanDoiTuongTTBhytBatBuocPhaiNhapDuongDung
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignPrescription__BenhNhanDoiTuongTTBhytBatBuocPhaiNhapDuongDung", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string CanhBaoThuocDaKeTrongNgay
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignPrescription__CanhBaoThuocDaKeTrongNgay", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string ThuocKhongCoTrongKho
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignPrescription__ThuocKhongCoTrongKho", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string KhongNhapSoLuong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignPrescription__KhongNhapSoLuong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string KhongCoDoiTuongThanhToan
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignPrescription__KhongCoDoiTuongThanhToan", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string SoLuongXuatLonHonSpoLuongKhadungTrongKho
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignPrescription__SoLuongXuatLonHonSpoLuongKhadungTrongKho", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        //Tự động đổi sang đối tượng
        internal static string TuDongDoiSangDoiTuong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignPrescription__TuDongDoiSangDoiTuong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
    }
}
