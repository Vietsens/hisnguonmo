using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisMedicalAssessment.Resources
{
    class ResourceMessage
    {
        static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.Desktop.Plugins.HisMedicalAssessment.Resources.Message.Lang", System.Reflection.Assembly.GetExecutingAssembly());

        #region AssType
        internal static string GiamDinhLanDau
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("GiamDinhLanDau", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string GiamDinhLai
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("GiamDinhLai", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string GiamDinhTaiPhat
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("GiamDinhTaiPhat", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string PhucQuyet
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("PhucQuyet", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string PhucQuyetLanCuoi
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("PhucQuyetLanCuoi", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string BoSung
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("BoSung", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string VetThuongConSot
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("VetThuongConSot", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string TongHop
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("TongHop", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        #endregion
        #region WelfareType
        internal static string ThuongBinh
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ThuongBinh", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string BenhTat
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("BenhTat", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string BenhNN
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("BenhNN", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string TaiNanLaoDong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("TaiNanLaoDong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string ChatDocHoaHoc
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ChatDocHoaHoc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string BenhBinh
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("BenhBinh", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string Khac
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Khac", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        #endregion
        #region DisabilityType
        internal static string VanDong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("VanDong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string NgheNoi
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("NgheNoi", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string Nhin
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Nhin", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string ThanKinhTamThan
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ThanKinhTamThan", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string TriTue
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("TriTue", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string KtKhac
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("KtKhac", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        #endregion
        #region DisablitityStatus
        internal static string ThucHienDuoc
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ThucHienDuoc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string CanTroGiup
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("CanTroGiup", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string KhongThucHienDuoc
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("KhongThucHienDuoc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string KhongXacDinhDuoc
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("KhongXacDinhDuoc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        #endregion
        internal static string ThanhVienDuocMoiKhongTrungNhau
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ThanhVienDuocMoiKhongTrungNhau", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string ThanhVienKhongTrungNhau
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ThanhVienKhongTrungNhau", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string ThanhVienDuocMoiTrungThanhVien
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ThanhVienDuocMoiTrungThanhVien", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string ThanhVienKyThay
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ThanhVienKyThay", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string ThanhVienChuTriHoacThuKy
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ThanhVienChuTriHoacThuKy", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string ThanhPhanDuocMoiChuTriHoacThuKy
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ThanhPhanDuocMoiChuTriHoacThuKy", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string KhongNhatTriKhongTrungNhau
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("KhongNhatTriKhongTrungNhau", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string ChuaNhapThanhVien
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ChuaNhapThanhVien", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string ChuaNhapYKien
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ChuaNhapYKien", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string ChuaNhapHoTen
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ChuaNhapHoTen", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
        internal static string GiamDinhYKhoa
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("GiamDinhYKhoa", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string HopGiamDinhYKhoa
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("HopGiamDinhYKhoa", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
