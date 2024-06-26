using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.TreatmentIcdEdit
{
    class ResourceMessage
    {
        static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.Desktop.Plugins.TreatmentIcdEdit.Resources.Message.Lang", System.Reflection.Assembly.GetExecutingAssembly());

        internal static string NgayRaKhongDuocNhoHonNgayVao
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("NgayRaKhongDuocNhoHonNgayVao", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string MaChanDoanKhongDung
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("MaChanDoanKhongDung", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ThieuTruongDuLieu
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ThieuTruongDuLieu", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string NgayVaoKhongDuocLonHonNgayHienTai
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("NgayVaoKhongDuocLonHonNgayHienTai", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string BenhNhanChuaKetThucDieuTri
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("BenhNhanChuaKetThucDieuTri", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string NgayVaoKhongDuocLonHonNgayVaoNoiTru
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("NgayVaoKhongDuocLonHonNgayVaoNoiTru", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string NgayRaKhongDuocNhoHonNgayVaoNoiTru
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("NgayRaKhongDuocNhoHonNgayVaoNoiTru", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string BenhNhanKoPhaiNoiTruKhongChoPhepNhapNgay
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("BenhNhanKoPhaiNoiTruKhongChoPhepNhapNgay", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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

        internal static string KhongTimThayIcdTuongUngVoiCacMaSau
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("KhongTimThayIcdTuongUngVoiCacMaSau", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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

        internal static string NgayVaoKhongDuocDeTrong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("NgayVaoKhongDuocDeTrong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string CDNhapVien
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("CDNhapVien", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string ChanDoanNhapVien
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ChanDoanNhapVien", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string ChanDoanYHCT
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ChanDoanYHCT", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string NNNgoai
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("NNNgoai", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string NguyenNhanNgoai
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("NguyenNhanNgoai", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string CDPhuNV
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("CDPhuNV", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ChanDoanPhuNhapVien
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ChanDoanPhuNhapVien", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string CDYHCTPhu
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("CDYHCTPhu", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string ChanDoanYHCTKemTheo
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ChanDoanYHCTKemTheo", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string NhanF1DeChonBenh
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("NhanF1DeChonBenh", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string STTHoSoDaTonTai
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("STTHoSoDaTonTai", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
    }
}
