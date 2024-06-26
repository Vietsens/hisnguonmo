using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TYT.Desktop.Plugins.TYTFetusBorn.Resources
{
    class ResourceMessage
    {
        static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("TYT.Desktop.Plugins.TYTFetusBorn.Resources.Message.Lang", System.Reflection.Assembly.GetExecutingAssembly());

        internal static string TruongDuLieuBatBuocPhaiNhap
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_TYTFetusExam__TruongDuLieuBatBuocPhaiNhap", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
                    return Inventec.Common.Resource.Get.Value("DoDaiVuotQua", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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

        internal static string HeThongTBCuaSoThongBaoBanCoMuonHuyDuLieuKhong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("HeThongTBCuaSoThongBaoBanCoMuonHuyDuLieuKhong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
