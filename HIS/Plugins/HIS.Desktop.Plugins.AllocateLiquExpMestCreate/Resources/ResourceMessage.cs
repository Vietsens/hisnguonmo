using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.AllocateLiquExpMestCreate.Resources
{
    class ResourceMessage
    {
        static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.Desktop.Plugins.AllocateLiquExpMestCreate.Resources.Message.Lang", System.Reflection.Assembly.GetExecutingAssembly());

        internal static string SoLuongXuatLonHonSpoLuongKhadungTrongKho
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AllocateLiquExpMestCreate__SoLuongXuatLonHonSpoLuongKhadungTrongKho", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ThuocVatTuDaCoTrongDanhSachXuatBanCoMuonThayTheThuoc
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_AllocateLiquExpMestCreate__ThuocVatTuDaCoTrongDanhSachXuatBanCoMuonThayTheThuoc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
