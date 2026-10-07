using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ServiceReqList.Resources
{
    class ResourceMessage
    {
        static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.Desktop.Plugins.ServiceReqList.Resources.Message.Lang", System.Reflection.Assembly.GetExecutingAssembly());

        internal static string ThongBao
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ThongBao", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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

        internal static string TruongDuLieuBatBuoc
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("TruongDuLieuBatBuoc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string CanhBao
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("CanhBao", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string KhongCoQuyenXoaYeuCauBenhNhan
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("KhongCoQuyenXoaYeuCauBenhNhan", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string YeuCau
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("YeuCau", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string Duyet
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Duyet", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ThuocVatTuNgoaiDanhMuc
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ThuocVatTuNgoaiDanhMuc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string BanChuaChonDichVu
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("BanChuaChonDichVu", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string DichVuKhongCungHoSoDieuTri
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("DichVuKhongCungHoSoDieuTri", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string DichVuKhongCungThoiGianChiDinh
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("DichVuKhongCungThoiGianChiDinh", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string DichVuLaThuoc
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("DichVuLaThuoc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string DonMauKhongChoPhepSua
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("DonMauKhongChoPhepSua", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string KhongTimThayMaXuat
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("KhongTimThayMaXuat", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string HoSoDieuTriDangTamKhoa
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("HoSoDieuTriDangTamKhoa", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string KhongTimThayHoSoDieuTri
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("KhongTimThayHoSoDieuTri", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string DonKhongLayKhongChoPhepSua
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("DonKhongLayKhongChoPhepSua", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        /// <summary>Chưa chọn y lệnh xét nghiệm nào.</summary>
        internal static string ChuaChonYLenhXetNghiem
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ChuaChonYLenhXetNghiem", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        /// <summary>Các y lệnh không thuộc cùng một bệnh nhân.</summary>
        internal static string CacYLenhKhongCungBenhNhan
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("CacYLenhKhongCungBenhNhan", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        /// <summary>Có xét nghiệm chưa có kết quả (kèm danh sách y lệnh).</summary>
        internal static string CoXetNghiemChuaCoKetQua
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("CoXetNghiemChuaCoKetQua", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string Ngaynhapphailonhon0
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Ngaynhapphailonhon0", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string XacNhanGuiLaiChiDinhSangHeThongTichHop
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("XacNhanGuiLaiChiDinhSangHeThongTichHop", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string KetQuaGuiLaiChiDinh
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("KetQuaGuiLaiChiDinh", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string DanhSachYLenhGuiKhongThanhCong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("DanhSachYLenhGuiKhongThanhCong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string GuiLaiChiDinhKhongThanhCong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("GuiLaiChiDinhKhongThanhCong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        /// <summary>Viec 55703: Không xóa được y lệnh theo quyền xóa trong cùng khoa:</summary>
        internal static string XoaCungKhoaKhongXoaDuoc
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("XoaCungKhoaKhongXoaDuoc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        /// <summary>Viec 55703: - Y lệnh {0}: văn bản "{1}" đã được ký số (người ký: {2}). Chỉ người chỉ định, người tạo y lệnh hoặc quản trị hệ thống mới được xóa.</summary>
        internal static string XoaCungKhoaYLenhDaKySo
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("XoaCungKhoaYLenhDaKySo", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        /// <summary>Viec 55703: - Y lệnh {0}: văn bản "{1}" do tài khoản {2} tạo, chưa ký. Người tạo văn bản cần xóa văn bản trên bệnh án điện tử trước khi xóa y lệnh.</summary>
        internal static string XoaCungKhoaVanBanDoNguoiKhacTao
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("XoaCungKhoaVanBanDoNguoiKhacTao", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        /// <summary>Viec 55703: Không kiểm tra được văn bản ký trên bệnh án điện tử của y lệnh {0} nên chưa xóa. Vui lòng thử lại.</summary>
        internal static string XoaCungKhoaKhongKiemTraDuocVanBanKy
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("XoaCungKhoaKhongKiemTraDuocVanBanKy", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
