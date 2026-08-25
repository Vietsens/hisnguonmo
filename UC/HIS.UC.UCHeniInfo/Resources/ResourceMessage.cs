using Inventec.Desktop.Common.LanguageManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.UCHeniInfo
{
    class ResourceMessage
    {
        internal static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.UC.UCHeniInfo.Resources.Message.Lang", System.Reflection.Assembly.GetExecutingAssembly());

        internal static string PhaiCoGiayChungNhanKhongCungChiTra
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("PhaiCoGiayChungNhanKhongCungChiTra", languageMessage, LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string PhaiDatDu5Nam6ThangMoiCoTheChonDTMCCT
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("PhaiDatDu5Nam6ThangMoiCoTheChonDTMCCT", languageMessage, LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string MaBenhKhongKhopVoiTenBenh
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("MaBenhKhongKhopVoiTenBenh", languageMessage, LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string MaBenhChinhKhongHopLe
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("MaBenhChinhKhongHopLe", languageMessage, LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string BatBuocNhapTenBenhVoiTruongHopBenhNhanLaDungTuyenGioiThieu
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("BatBuocNhapTenBenhVoiTruongHopBenhNhanLaDungTuyenGioiThieu", languageMessage, LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }



        internal static string ThoiDiemMienCungChiTraPhaiCungNamVoiNamHienTai
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ThoiDiemMienCungChiTraPhaiCungNamVoiNamHienTai", languageMessage, LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string SoTheBHYTKhongHopLe
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("SoTheBHYTKhongHopLe", languageMessage, LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string SoTheDaDuocSuDung
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("SoTheDaDuocSuDung", languageMessage, LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string BatBuocPhaiChonTruongHop
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("BatBuocPhaiChonTruongHop", languageMessage, LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string NguoiDungNhapNgayKhongHopLe
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("NguoiDungNhapNgayKhongHopLe", languageMessage, LanguageManager.GetCulture());
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
                    return Inventec.Common.Resource.Get.Value("TruongDuLieuBatBuoc", languageMessage, LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string MaDangKyKCBBDKhacVoiCuaVien
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("MaDangKyKCBBDKhacVoiCuaVien", languageMessage, LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string DotKhamTruocCuaBenhNhanConNoTienVienPhi
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("DotKhamTruocCuaBenhNhanConNoTienVienPhi", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string TieuDeCuaSoThongBaoLaThongBao
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("HIS_UC_UCPatientRaw_TieuDeCuaSoThongBaoLaThongBao", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
                return "";
            }
        }
        internal static string ThuocCoThoiSuDungDen
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ThuocCoThoiSuDungDen", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        /// <summary>Thẻ BHYT đã hết hạn sử dụng. Bạn có muốn tiếp tục không?</summary>
        internal static string TheBHYTDaHetHanSuDungBanCoMuonTiepTucKhong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("TheBHYTDaHetHanSuDungBanCoMuonTiepTucKhong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string DotKhamTruocCuaBenhNhanCoThuocChuaUongHet
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("DotKhamTruocCuaBenhNhanCoThuocChuaUongHet", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        #region Tra cuu tien cung chi tra / mien cung chi tra tren cong BHXH

        /// <summary>Cùng chi trả lũy kế trên cổng BHXH: {0}   (hiện tại: {1})</summary>
        internal static string CungChiTraLuyKeTrenCongBHXH
        {
            get { return GetMessageValue("CungChiTraLuyKeTrenCongBHXH"); }
        }

        /// <summary>Đã cùng chi trả 6 tháng lương cơ sở: {0}</summary>
        internal static string DaCungChiTra06ThangLuongCoSo
        {
            get { return GetMessageValue("DaCungChiTra06ThangLuongCoSo"); }
        }

        /// <summary>Thời điểm miễn cùng chi trả trên cổng: {0}   (hiện tại: {1})</summary>
        internal static string ThoiDiemMienCungChiTraTrenCong
        {
            get { return GetMessageValue("ThoiDiemMienCungChiTraTrenCong"); }
        }

        /// <summary>Bạn có muốn lấy thông tin từ cổng BHXH?</summary>
        internal static string BanCoMuonLayThongTinTuCongBHXHKhong
        {
            get { return GetMessageValue("BanCoMuonLayThongTinTuCongBHXHKhong"); }
        }

        /// <summary>
        /// Lũy kế đã vượt ngưỡng nhưng cổng không trả về ngày ra viện của đợt vượt ngưỡng
        /// nên không suy được thời điểm miễn cùng chi trả.
        /// </summary>
        internal static string KhongXacDinhDuocThoiDiemMienCungChiTra
        {
            get { return GetMessageValue("KhongXacDinhDuocThoiDiemMienCungChiTra"); }
        }

        /// <summary>Số tiền lũy kế cùng chi trả bắt đầu vượt 06 tháng lương cơ sở...</summary>
        internal static string SoTienLuyKeCungChiTraVuot06ThangLuongCoSo
        {
            get { return GetMessageValue("SoTienLuyKeCungChiTraVuot06ThangLuongCoSo"); }
        }

        /// <summary>Có</summary>
        internal static string Co
        {
            get { return GetMessageValue("Co"); }
        }

        /// <summary>Không</summary>
        internal static string Khong
        {
            get { return GetMessageValue("Khong"); }
        }

        /// <summary>Không xác định</summary>
        internal static string KhongXacDinh
        {
            get { return GetMessageValue("KhongXacDinh"); }
        }

        /// <summary>đang để trống</summary>
        internal static string DangDeTrong
        {
            get { return GetMessageValue("DangDeTrong"); }
        }

        /// <summary>
        /// Đọc một câu thông báo theo ngôn ngữ hiện tại.
        /// Trả về chuỗi rỗng thay vì ném lỗi, để thiếu key không làm vỡ giao diện.
        /// </summary>
        private static string GetMessageValue(string key)
        {
            try
            {
                return Inventec.Common.Resource.Get.Value(key, languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return "";
        }

        #endregion
    }
}
