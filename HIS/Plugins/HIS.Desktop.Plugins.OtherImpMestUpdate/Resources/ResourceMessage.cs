using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.OtherImpMestUpdate.Resources
{
    class ResourceMessage
    {
        static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.Desktop.Plugins.OtherImpMestUpdate.Resources.Message.Lang", System.Reflection.Assembly.GetExecutingAssembly());

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

        internal static string NguoiDungNhapNgayKhongHopLe
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("NguoiDungNhapNgayKhongHopLe", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string SoLuongPhaiLonHonKhong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("SoLuongPhaiLonHonKhong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string NguoiDungChuaChonLoaiNhap
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("NguoiDungChuaChonLoaiNhap", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string NguoiDungChuaChonKhoNhap
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("NguoiDungChuaChonKhoNhap", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string MaNhapKhongHopLe
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("MaNhapKhongHopLe", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
