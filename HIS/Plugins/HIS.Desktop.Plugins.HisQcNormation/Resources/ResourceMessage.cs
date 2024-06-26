using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisQcNormation.Resources
{
    class ResourceMessage
    {
        static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.Desktop.Plugins.HisQcNormation.Resources.Message.Lang", System.Reflection.Assembly.GetExecutingAssembly());

        internal static string BanChuaChonPhong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("HisQcNormation_BanChuaChonPhong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string KhongCoSoLuong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("HisQcNormation_KhongCoSoLuong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string BanChuaNhapDanhSachDinhMuc
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("HisQcNormation_BanChuaNhapDanhSachDinhMuc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string BanChuaChonHoaChat
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("HisQcNormation_BanChuaChonHoaChat", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
