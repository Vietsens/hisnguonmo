using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.RoomTypeModule.Resources
{
    class ResourceMessage
    {
        static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.Desktop.Plugins.RoomTypeModule.Resources.Message.Lang", System.Reflection.Assembly.GetExecutingAssembly());
        internal static string Plugin_TrungDuLieuCopyVaPaste
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_TrungDuLieuCopyVaPaste", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string Plugin_VuiLongChonMenu
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_VuiLongChonMenu", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string Plugin_VuiLongChonPhong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_VuiLongChonPhong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string Plugin_VuiLongCopy
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_VuiLongCopy", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string Plugin_XuLyThatBaiChuaChonChucNang
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_XuLyThatBaiChuaChonChucNang", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string Plugin_XuLyThatBaiChuaChonLoaiPhong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_XuLyThatBaiChuaChonLoaiPhong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string Plugin_XuLyThatBaiKhongCoDuLieuThayDoi
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_XuLyThatBaiKhongCoDuLieuThayDoi", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
