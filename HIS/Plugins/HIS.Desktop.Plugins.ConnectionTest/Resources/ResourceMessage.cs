using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ConnectionTest.Resources
{
    class ResourceMessage
    {
        static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.Desktop.Plugins.ConnectionTest.Resources.Message.Lang", System.Reflection.Assembly.GetExecutingAssembly());
        internal static string DichVuChuaChonMayTraKetQuaBanCoMuonTiepTuc
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("DichVuChuaChonMayTraKetQuaBanCoMuonTiepTuc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string DichVuChuaChonMayTraKetQua
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("DichVuChuaChonMayTraKetQua", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ChuaChonChiSoLuuKetQua
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ChuaChonChiSoLuuKetQua", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ChiSoChuaNhapGiaTriBanCoMuonTiepTuc
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ChiSoChuaNhapGiaTriBanCoMuonTiepTuc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ChiSoChuaNhapGiaTri
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ChiSoChuaNhapGiaTri", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ChuaChonChiSoIn
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ChuaChonChiSoIn", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string YLenhChuaCoVanBanQues
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("YLenhChuaCoVanBanQues", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string YLenhChuaCoVanBan
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("YLenhChuaCoVanBan", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string YLenhDaTonTaiVanBanKy
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("YLenhDaTonTaiVanBanKy", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        /// <summary>Chưa chọn mẫu xét nghiệm.</summary>
        internal static string ChuaChonMauXetNghiem
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ChuaChonMauXetNghiem", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        /// <summary>Các mẫu không thuộc cùng một bệnh nhân.</summary>
        internal static string MauKhongCungMotBenhNhan
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("MauKhongCungMotBenhNhan", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        /// <summary>Có mẫu xét nghiệm chưa có kết quả: {0} (template String.Format).</summary>
        internal static string CoMauChuaCoKetQua
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("CoMauChuaCoKetQua", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        /// <summary>Nhãn nhóm "Khác" trong dropdown Nhóm XN.</summary>
        internal static string NhomXNKhac
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("NhomXNKhac", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        /// <summary>Không ghi được dữ liệu sang máy CG300 vào thư mục {0}. Vui lòng kiểm tra cấu hình/quyền thư mục.</summary>
        internal static string CG300KhongGhiDuocFile
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("CG300KhongGhiDuocFile", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        /// <summary>Các mẫu chưa có barcode, không gửi sang CG300 (mã y lệnh): {0}</summary>
        internal static string CG300MauChuaCoBarcode
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("CG300MauChuaCoBarcode", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        /// <summary>Dịch vụ chưa được gán loại mẫu bệnh phẩm (không xác định được ống): {0}</summary>
        internal static string CG300DichVuChuaGanLoaiMau
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("CG300DichVuChuaGanLoaiMau", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        /// <summary>Không có dữ liệu hợp lệ để gửi sang CG300.</summary>
        internal static string CG300KhongCoDuLieuIn
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("CG300KhongCoDuLieuIn", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        /// <summary>Mất kết nối máy CG300: {0}. Vui lòng kiểm tra phần mềm CG và thư mục trao đổi.</summary>
        internal static string CG300MatKetNoi
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("CG300MatKetNoi", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        /// <summary>Thư mục {0} không tồn tại</summary>
        internal static string CG300LyDoThuMucKhongTonTai
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("CG300LyDoThuMucKhongTonTai", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        /// <summary>Không có quyền ghi thư mục {0}</summary>
        internal static string CG300LyDoKhongGhiDuoc
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("CG300LyDoKhongGhiDuoc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        /// <summary>Phần mềm điều khiển CG300 ({0}) chưa chạy</summary>
        internal static string CG300LyDoPhanMemChuaChay
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("CG300LyDoPhanMemChuaChay", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        /// <summary>Barcode {0} đã được in tem qua CG300. Bạn có muốn in lại (máy sẽ nhả thêm ống) không?</summary>
        internal static string CG300MauDaInTemInLai
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("CG300MauDaInTemInLai", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        /// <summary>Có kết quả chưa lưu. Vui lòng lưu kết quả trước khi duyệt.</summary>
        internal static string VuiLongLuuTruocKhiDuyet
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("VuiLongLuuTruocKhiDuyet", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        /// <summary>Duyệt kết quả áp dụng cho toàn bộ dịch vụ của mẫu {0}. Bạn có muốn tiếp tục?</summary>
        internal static string DuyetApDungToanBoMau
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("DuyetApDungToanBoMau", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        /// <summary>Bạn có muốn hủy duyệt kết quả mẫu {0}?</summary>
        internal static string BanCoMuonHuyDuyetKetQuaMau
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("BanCoMuonHuyDuyetKetQuaMau", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
