using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDA.Desktop.Plugins.SdaField_code.Resources
{
    class ResourceMessage
    {
        static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("SDA.Desktop.Plugins.SdaField_code.Resources.Message.Lang", System.Reflection.Assembly.GetExecutingAssembly());

        internal static string BanCoMuonKhoaDuLieuKhong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_SdaField__BanCoMuonKhoaDuLieuKhong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string BanCoMuonXoaDuLieuKhong
        {
            get
            {
                try
                {
                    var a=  Inventec.Common.Resource.Get.Value("Plugin_SdaField__BanCoMuonXoaDuLieuKhong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                    return a;
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string ChuaNhapTruongDuLieuBatBuoc
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_SdaField__ChuaNhapTruongDuLieuBatBuoc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        //internal static string NgayUong
        //{
        //    get
        //    {
        //        try
        //        {
        //            return Inventec.Common.Resource.Get.Value("Plugin_SdaField__NgayUong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
        //        }
        //        catch (Exception ex)
        //        {
        //            Inventec.Common.Logging.LogSystem.Warn(ex);
        //        }
        //        return "";
        //    }
        //}
        //internal static string _NgayXVienBuoiYZ
        //{
        //    get
        //    {
        //        try
        //        {
        //            return Inventec.Common.Resource.Get.Value("Plugin_SSdaField___NgayXVienBuoiYZ", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
        //        }
        //        catch (Exception ex)
        //        {
        //            Inventec.Common.Logging.LogSystem.Warn(ex);
        //        }
        //        return "";
        //    }
        //}
        //internal static string Sang
        //{
        //    get
        //    {
        //        try
        //        {
        //            return Inventec.Common.Resource.Get.Value("Plugin_SdaField__Sang", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
        //        }
        //        catch (Exception ex)
        //        {
        //            Inventec.Common.Logging.LogSystem.Warn(ex);
        //        }
        //        return "";
        //    }
        //}
        //internal static string Trua
        //{
        //    get
        //    {
        //        try
        //        {
        //            return Inventec.Common.Resource.Get.Value("Plugin_SdaField__Trua", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
        //        }
        //        catch (Exception ex)
        //        {
        //            Inventec.Common.Logging.LogSystem.Warn(ex);
        //        }
        //        return "";
        //    }
        //}
        //internal static string Chieu
        //{
        //    get
        //    {
        //        try
        //        {
        //            return Inventec.Common.Resource.Get.Value("Plugin_SdaField__Chieu", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
        //        }
        //        catch (Exception ex)
        //        {
        //            Inventec.Common.Logging.LogSystem.Warn(ex);
        //        }
        //        return "";
        //    }
        //}
        //internal static string Toi
        //{
        //    get
        //    {
        //        try
        //        {
        //            return Inventec.Common.Resource.Get.Value("Plugin_SdaField__Toi", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
        //        }
        //        catch (Exception ex)
        //        {
        //            Inventec.Common.Logging.LogSystem.Warn(ex);
        //        }
        //        return "";
        //    }
        //}
        //internal static string BuoiSang
        //{
        //    get
        //    {
        //        try
        //        {
        //            return Inventec.Common.Resource.Get.Value("Plugin_SSdaField__BuoiSang", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
        //        }
        //        catch (Exception ex)
        //        {
        //            Inventec.Common.Logging.LogSystem.Warn(ex);
        //        }
        //        return "";
        //    }
        //}
        //internal static string BuoiTrua
        //{
        //    get
        //    {
        //        try
        //        {
        //            return Inventec.Common.Resource.Get.Value("Plugin_SdaField__BuoiTrua", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
        //        }
        //        catch (Exception ex)
        //        {
        //            Inventec.Common.Logging.LogSystem.Warn(ex);
        //        }
        //        return "";
        //    }
        //}
        //internal static string BuoiChieu
        //{
        //    get
        //    {
        //        try
        //        {
        //            return Inventec.Common.Resource.Get.Value("Plugin_SdaField__BuoiChieu", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
        //        }
        //        catch (Exception ex)
        //        {
        //            Inventec.Common.Logging.LogSystem.Warn(ex);
        //        }
        //        return "";
        //    }
        //}
        //internal static string BuoiToi
        //{
        //    get
        //    {
        //        try
        //        {
        //            return Inventec.Common.Resource.Get.Value("Plugin_SdaField__BuoiToi", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
        //        }
        //        catch (Exception ex)
        //        {
        //            Inventec.Common.Logging.LogSystem.Warn(ex);
        //        }
        //        return "";
        //    }
        //}
        //internal static string NGayDung
        //{
        //    get
        //    {
        //        try
        //        {
        //            return Inventec.Common.Resource.Get.Value("Plugin_SdaField__NgayDung", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
        //        }
        //        catch (Exception ex)
        //        {
        //            Inventec.Common.Logging.LogSystem.Warn(ex);
        //        }
        //        return "";
        //    }
        //}
        //internal static string NgaySuDung
        //{
        //    get
        //    {
        //        try
        //        {
        //            return Inventec.Common.Resource.Get.Value("Plugin_SdaField__NgaySuDung", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
        //        }
        //        catch (Exception ex)
        //        {
        //            Inventec.Common.Logging.LogSystem.Warn(ex);
        //        }
        //        return "";
        //    }
        //}
    }
}
