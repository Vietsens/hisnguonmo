using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ExpMestSaleCreate.Resources
{
    class ResourceMessage
    {
        static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.Desktop.Plugins.ExamServiceReqExecute.Resources.Message.Lang", System.Reflection.Assembly.GetExecutingAssembly());
       
        internal static string NextTreatmentInstructionKhongDung
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("NextTreatmentInstructionKhongDung", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string MabenhPhuDaDuocSuDungChoMaBenhChinh
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("MabenhPhuDaDuocSuDungChoMaBenhChinh", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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

        internal static string IcdKhongDung
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("IcdKhongDung", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ChuaNhapDayDuThongTinBatBuoc
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_ExamServiceReqExecute__ChuaNhapDayDuThongTinBatBuoc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
                    return Inventec.Common.Resource.Get.Value("Plugin_ExamServiceReqExecute__KhongTimThayIcdTuongUngVoiCacMaSau", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string CanhBaoSaiThongTinTheBHYT
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("CanhBaoSaiThongTinTheBHYT", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
                    return Inventec.Common.Resource.Get.Value("Plugin_ExamServiceReqExecute__TaiKhoanKhongCoQuyenThucHienChucNang", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string NhapQuaKyTuBenhPhu
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_ExamServiceReqExecute__NhapQuaKyTuBenhPhu", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string DaCoDonThuocBanCoMuonTiepTucKhong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_ExamServiceReqExecute__DaCoDonThuocBanCoMuonTiepTucKhong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string DonThuocLanKhamTruoc
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_ExamServiceReqExecute__DonThuocLanKhamTruoc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
                    return Inventec.Common.Resource.Get.Value("Plugin_ExamServiceReqExecute__ThongBao", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ThoiGianNhapVienNhoHonThoiGianVaoVien
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_ExamServiceReqExecute__ThoiGianNhapVienNhoHonThoiGianVaoVien", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ThoiGianKetThucKhamPhaiLonHonThoiGianVaoVien
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_ExamServiceReqExecute__ThoiGianKetThucKhamPhaiLonHonThoiGianVaoVien", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ThoiGianKetThucKhamPhaiNhoHonThoiGianKetThucDieuTri
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_ExamServiceReqExecute__ThoiGianKetThucKhamPhaiNhoHonThoiGianKetThucDieuTri", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ThoiGianKetThucKhamPhaiLonHonThoiGianYLenh
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_ExamServiceReqExecute__ThoiGianKetThucKhamPhaiLonHonThoiGianYLenh", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ThoiGianKetThucKhamPhaiLonHonThoiGianBatDau
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_ExamServiceReqExecute__ThoiGianKetThucKhamPhaiLonHonThoiGianBatDau", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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

        internal static string BenhNhanCoYeuCauCapGiayNghiOm
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("BenhNhanCoYeuCauCapGiayNghiOm", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string CanhbaoKhongChoSuaICDName
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("CanhbaoKhongChoSuaICDName", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string MaICDchuaduocnhap
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_ExamServiceReqExecute__MaICDchuaduocnhap", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ChuaDuocTaoVoBenhAn
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ChuaDuocTaoVoBenhAn", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
