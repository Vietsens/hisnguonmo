using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.CareCreate.Resources
{
    class ResourceMessage
    {
        static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.Desktop.Plugins.CareCreate.Resources.Message.Lang", System.Reflection.Assembly.GetExecutingAssembly());

        internal static string ChamSoc_DuLieuChiTietKhongTheCungLoaiChamSoc
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ChamSoc_DuLieuChiTietKhongTheCungLoaiChamSoc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string SpinEdit__Dhst__KhongDuocNhapSoAm
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_CareCreate__SpinEdit__Dhst__KhongDuocNhapSoAm", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string Validate_Date_Time
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_CareCreate__Validate_Tracking_Time", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
                    return Inventec.Common.Resource.Get.Value("Plugin_CareCreate__Validate_Thong_Bao", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string NgayThang
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_CareCreate__NgayThang", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string Gio
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_CareCreate__Gio", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string YThuc
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_CareCreate__YThuc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string DaNiemMac
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_CareCreate__DaNiemMac", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string Mach
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_CareCreate__Mach", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
                    return Inventec.Common.Resource.Get.Value("Plugin_CareCreate__Khac", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string DauHieuSinhTon
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_CareCreate__DauHieuSinhTon", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string NhietDo
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_CareCreate__NhietDo", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string HuyetAp
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_CareCreate__HuyetAp", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string NhipTho
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_CareCreate__NhipTho", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string NuocTieu
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_CareCreate__NuocTieu", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string Phan
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_CareCreate__Phan", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string CanNang
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_CareCreate__CanNang", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ThucHienYLenh
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_CareCreate__ThucHienYLenh", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ThuocThuongQuy
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_CareCreate__ThuocThuongQuy", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ThuocBoSung
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_CareCreate__ThuocBoSung", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string XetNghiem
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_CareCreate__XetNghiem", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string CheDoAn
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_CareCreate__CheDoAn", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string VeSinh
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_CareCreate__VeSinh", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string HuongDanNoiQuy
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_CareCreate__HuongDanNoiQuy", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string GiaoDucSucKhoe
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_CareCreate__GiaoDucSucKhoe", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string TheoDoiChamSoc
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_CareCreate__TheoDoiChamSoc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string DienBien
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_CareCreate__DienBien", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string NgayDoPhaiBangNgayXuLyPhieu
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_CareCreate__NgayDoPhaiBangNgayXuLyPhieu", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string XulyThanhCong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_CareCreate__XulyThanhCong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string DoDaiVuotQua
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_CareCreate__DoDaiVuotQua", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ThieuTruongDuLieuBatBuoc
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_CareCreate__ThieuTruongDuLieuBatBuoc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
