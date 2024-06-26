using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisMedicineTypeTut.Resources
{
    class ResourceMessage
    {
        static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.Desktop.Plugins.HisMedicineTypeTut.Resources.Message.Lang", System.Reflection.Assembly.GetExecutingAssembly());

        internal static string BanCoMuonKhoaDuLieuKhong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_HisMedicineTypeTut__BanCoMuonKhoaDuLieuKhong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string BanCoMuonMoKhoaDuLieuKhong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_HisMedicineTypeTut__BanCoMuonMoKhoaDuLieuKhong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
                    return Inventec.Common.Resource.Get.Value("Plugin_HisMedicineTypeTut__BanCoMuonXoaDuLieuKhong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
                    return Inventec.Common.Resource.Get.Value("Plugin_HisMedicineTypeTut__ChuaNhapTruongDuLieuBatBuoc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string NgayUong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_HisMedicineTypeTut__NgayUong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string _NgayXVienBuoiYZ
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_HisMedicineTypeTut___NgayXVienBuoiYZ", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string Sang
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_HisMedicineTypeTut__Sang", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string Trua
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_HisMedicineTypeTut__Trua", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string Chieu
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_HisMedicineTypeTut__Chieu", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string Toi
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_HisMedicineTypeTut__Toi", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string BuoiSang
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_HisMedicineTypeTut__BuoiSang", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string BuoiTrua
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_HisMedicineTypeTut__BuoiTrua", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string BuoiChieu
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_HisMedicineTypeTut__BuoiChieu", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string BuoiToi
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_HisMedicineTypeTut__BuoiToi", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string NGayDung
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_HisMedicineTypeTut__NgayDung", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string NgaySuDung
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_HisMedicineTypeTut__NgaySuDung", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
