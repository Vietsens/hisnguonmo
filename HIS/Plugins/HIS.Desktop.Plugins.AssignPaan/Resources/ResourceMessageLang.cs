using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.AssignPaan.Resources
{
    class ResourceMessageLang
    {
        static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.Desktop.Plugins.AssignPaan.Resources.Message.Lang", System.Reflection.Assembly.GetExecutingAssembly());

        internal static string KhongTimThayIcdTuongUngVoiCacMa
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_AssignPaan__KhongTimThayIcdTuongUngVoiCacMa", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
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
                    return Inventec.Common.Resource.Get.Value("Plugins_AssignPaan__TruongDuLieuBatBuoc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
                return "";
            }
        }

        internal static string HeThongKhongTimThayPluginCuaChucNangNay
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_AssignPaan__HeThongKhongTimThayPluginCuaChucNangNay", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
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
                    return Inventec.Common.Resource.Get.Value("Plugins_AssignPaan__TieuDeCuaSoThongBaoLaThongBao", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
                return "";
            }
        }

        internal static string TieuDeCuaSoThongBaoLaCanhBao
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_AssignPaan__TieuDeCuaSoThongBaoLaCanhBao", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
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
                    return Inventec.Common.Resource.Get.Value("Plugins_AssignPaan__KhongTimThayDoiTuongThanhToanTrongThoiGianYLenh", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
                return "";
            }
        }

        /// <summary>Viec 59656: - To dieu tri {0} do {1} tao, khong phai cua nguoi chi dinh {2}.</summary>
        internal static string TrackingOwner__ToDieuTriCuaNguoiKhac
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("TrackingOwner__ToDieuTriCuaNguoiKhac", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        /// <summary>Viec 59656: - Nguoi chi dinh {0} chua co to dieu tri ngay {1}.</summary>
        internal static string TrackingOwner__NguoiChiDinhChuaCoToDieuTri
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("TrackingOwner__NguoiChiDinhChuaCoToDieuTri", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        /// <summary>Viec 59656: chan luu khi to dieu tri khong phai cua nguoi chi dinh.</summary>
        internal static string TrackingOwner__ChanLuu
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("TrackingOwner__ChanLuu", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        /// <summary>Viec 59656: canh bao, hoi co tiep tuc khong.</summary>
        internal static string TrackingOwner__CanhBaoTiepTuc
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("TrackingOwner__CanhBaoTiepTuc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
