using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ReturnMicrobiologicalResults.Resources
{
    class ResourceMessage
    {
        static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.Desktop.Plugins.ReturnMicrobiologicalResults.Resources.Message.Lang", System.Reflection.Assembly.GetExecutingAssembly());
        
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

        internal static string BanCoMuonTiepTucKhong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("BanCoMuonTiepTucKhong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string KhongChoPhepTraKetQuaDichVu_Sau_PhutTinhTuThoiDiemRaYLenh
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("KhongChoPhepTraKetQuaDichVu_Sau_PhutTinhTuThoiDiemRaYLenh", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string TraKetQuaDichVu_VuotQua_PhutTinhTuThoiDiemRaYLenh
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("TraKetQuaDichVu_VuotQua_PhutTinhTuThoiDiemRaYLenh", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
