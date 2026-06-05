using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.SurgServiceReqExecute.Resources
{
    class ResourceMessage
    {
        static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.Desktop.Plugins.SurgServiceReqExecute.Resources.Message.Lang", System.Reflection.Assembly.GetExecutingAssembly());
        /// <summary>Thời gian thực hiện thuốc/vật tư lớn hơn thời gian Kết thúc dịch vụ. Mã y lệnh: {0}, Thuốc/ vật tư: {1}, Thời gian thực hiện: {2}</summary>
        internal static string ThoiGianThucHienVtytLonHonThoiGianKetThuc
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_SurgServiceReqExecute__ThoiGianThucHienVtytLonHonThoiGianKetThuc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        /// <summary>Bạn có muốn tiếp tục?</summary>
        internal static string BanCoMuonTiepTuc
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_SurgServiceReqExecute__BanCoMuonTiepTuc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        //qtcode
        internal static string ThoiGianYLenhKhongThuocKhoangThoiGianTrongKhoa
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignService__ThoiGianYLenhKhongThuocKhoangThoiGianTrongKhoa", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
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
                    return Inventec.Common.Resource.Get.Value("Plugin_AssignService__ChuaChonNgayChiDinh", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string BanCoMuonSuaThoiGianYLenhBangThoiGianBatDauPTTT
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("BanCoMuonSuaThoiGianYLenhBangThoiGianBatDauPTTT", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ThoiGianKetThucThoiGianRaVien
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ThoiGianKetThucThoiGianRaVien", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ThoiGianBatDauThoiGianRaVien
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ThoiGianBatDauThoiGianRaVien", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ThoiGianKetThucThoiGianVaoVien
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ThoiGianKetThucThoiGianVaoVien", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        
        internal static string ThoiGianBatDauThoiGianKetThuc
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_SurgServiceReqExecute__ThoiGianBatDauThoiGianKetThuc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ThoiGianBatDauThoiGianVaoVien
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_SurgServiceReqExecute__ThoiGianBatDauThoiGianVaoVien", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string KhongTimThayICDTuongUng
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_SurgServiceReqExecute__KhongTimThayICDTuongUng", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ThoiGianBatDauKhongDuocLonHonThoiGianKetThuc
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_SurgServiceReqExecute__ThoiGianBatDauKhongDuocLonHonThoiGianKetThuc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ThoiGianBatDauPhaiLonHonThoiGianYLenh
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_SurgServiceReqExecute__ThoiGianBatDauPhaiLonHonThoiGianYLenh", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ThoiGianKetThucKhongDuocNhoHonThoiGianBatDau
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_SurgServiceReqExecute__ThoiGianKetThucKhongDuocNhoHonThoiGianBatDau", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ThoiGianKetThucKhongDuocLonHonThoiGianHeThong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_SurgServiceReqExecute__ThoiGianKetThucKhongDuocLonHonThoiGianHeThong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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

        internal static string DichVuThieuThongTinKhongChoKetThucXuLy
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("DichVuThieuThongTinKhongChoKetThucXuLy", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ThoiGianKetThucKhongDuocNhoHonThoiGianYLenh
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ThoiGianKetThucKhongDuocNhoHonThoiGianYLenh", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string BanChuaChonLuocDo
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("BanChuaChonLuocDo", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string KhongCoNoiDungLuuMau
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("KhongCoNoiDungLuuMau", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

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

        internal static string BanChuaNhapThongTinTuongUngVoiCacVaiTRo
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("BanChuaNhapThongTinTuongUngVoiCacVaiTRo", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string TaiKhoanDuocThietLapVoiCacVaiTro
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("TaiKhoanDuocThietLapVoiCacVaiTro", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string BanChuaChonPhuongPhapNao
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("BanChuaChonPhuongPhapNao", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string KhongCoDuLieuMau
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("KhongCoDuLieuMau", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string VuiLongNhapThongTinkipThucHien
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("VuiLongNhapThongTinkipThucHien", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string DuLieuEkipTrung
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("DuLieuEkipTrung", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string BanKhongPhaiLaBacSyKhongDuocKetThuc
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("BanKhongPhaiLaBacSyKhongDuocKetThuc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string DichVuKhongCoThoiGianKetThucKhongChoKetThucXuLy
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("DichVuKhongCoThoiGianKetThucKhongChoKetThucXuLy", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string DichVuChuaThucHienKhongChoKetThucXuLy
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("DichVuChuaThucHienKhongChoKetThucXuLy", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string UploadFileThatBaiVuiLongLienHeQuanTriheThongDeDuocHoTro
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("UploadFileThatBaiVuiLongLienHeQuanTriheThongDeDuocHoTro", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string MaBenhChinhVuotQuaKyTuChoPhep
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("MaBenhChinhVuotQuaKyTuChoPhep", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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

        internal static string MaICDKhongDungVuiLongKiemTraLai
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("MaICDKhongDungVuiLongKiemTraLai", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string TenBenhChinhVuotQuaKyTuChoPhep
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("TenBenhChinhVuotQuaKyTuChoPhep", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string KhongChoPhepTraKetQuaDichVu_Sau_PhutTinhTuThoiDiemRaYLenh
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("KhongChoPhepTraKetQuaDichVu_Sau_PhutTinhTuThoiDiemRaYLenh", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string TraKetQuaDichVu_VuotQua_PhutTinhTuThoiDiemRaYLenh
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("TraKetQuaDichVu_VuotQua_PhutTinhTuThoiDiemRaYLenh", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string BanCoMuonTiepTucKhong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("BanCoMuonTiepTucKhong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        /// <summary>Y lệnh VTYT ({0}): Thời gian y lệnh VTYT không được để trống</summary>
        internal static string ThoiGianYLenhVTYTKhongDuocDeTrong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ThoiGianYLenhVTYTKhongDuocDeTrong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        /// <summary>Y lệnh VTYT ({0}): Thời gian bắt đầu VTYT không được để trống</summary>
        internal static string ThoiGianBatDauVTYTKhongDuocDeTrong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ThoiGianBatDauVTYTKhongDuocDeTrong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        /// <summary>Y lệnh VTYT ({0}): Thời gian kết thúc VTYT không được để trống</summary>
        internal static string ThoiGianKetThucVTYTKhongDuocDeTrong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ThoiGianKetThucVTYTKhongDuocDeTrong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        /// <summary>Y lệnh VTYT ({0}): Thời gian y lệnh VTYT ({1}) phải sau thời gian y lệnh PT ({2})</summary>
        internal static string ThoiGianYLenhVTYTPHaiSauThoiGianYLenhPT
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ThoiGianYLenhVTYTPHaiSauThoiGianYLenhPT", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        /// <summary>Y lệnh VTYT ({0}): Thời gian y lệnh VTYT ({1}) phải trước thời gian bắt đầu PT ({2})</summary>
        internal static string ThoiGianYLenhVTYTPhaiTruocThoiGianBatDauPT
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ThoiGianYLenhVTYTPhaiTruocThoiGianBatDauPT", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        /// <summary>Y lệnh VTYT ({0}): Thời gian bắt đầu VTYT ({1}) phải sau thời gian bắt đầu PT ({2})</summary>
        internal static string ThoiGianBatDauVTYTPhaiSauThoiGianBatDauPT
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ThoiGianBatDauVTYTPhaiSauThoiGianBatDauPT", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        /// <summary>Y lệnh VTYT ({0}): Thời gian kết thúc VTYT ({1}) phải sau thời gian bắt đầu VTYT ({2})</summary>
        internal static string ThoiGianKetThucVTYTPhaiSauThoiGianBatDauVTYT
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ThoiGianKetThucVTYTPhaiSauThoiGianBatDauVTYT", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        /// <summary>Y lệnh VTYT ({0}): Thời gian kết thúc VTYT ({1}) phải trước thời gian kết thúc PT ({2})</summary>
        internal static string ThoiGianKetThucVTYTPhaiTruocThoiGianKetThucPT
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ThoiGianKetThucVTYTPhaiTruocThoiGianKetThucPT", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        /// <summary>Thời gian bắt đầu không hợp lệ so với y lệnh VTYT con</summary>
        internal static string ThoiGianBatDauPTKhongHopLeVoiVTYTCon
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ThoiGianBatDauPTKhongHopLeVoiVTYTCon", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        /// <summary>Thời gian kết thúc không hợp lệ so với y lệnh VTYT con</summary>
        internal static string ThoiGianKetThucPTKhongHopLeVoiVTYTCon
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ThoiGianKetThucPTKhongHopLeVoiVTYTCon", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
