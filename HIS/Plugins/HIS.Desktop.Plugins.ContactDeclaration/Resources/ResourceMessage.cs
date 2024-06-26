using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ContactDeclaration.Resources
{
    class ResourceMessage
    {
        static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.Desktop.Plugins.ContactDeclaration.Resources.Message.Lang", System.Reflection.Assembly.GetExecutingAssembly());

        internal static string CapNhatlaiThongTinPhanLoai
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ContactDeclaration_CapNhatlaiThongTinPhanLoai", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string BanCoMuonNhapNguoiBenhMoiKhong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ContactDeclaration_BanCoMuonNhapNguoiBenhMoiKhong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
