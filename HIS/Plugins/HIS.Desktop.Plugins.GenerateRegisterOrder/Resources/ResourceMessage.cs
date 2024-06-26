using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.GenerateRegisterOrder.Resources
{
    class ResourceMessage
    {
        internal static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.Desktop.Plugins.GenerateRegisterOrder.Resources.Message", System.Reflection.Assembly.GetExecutingAssembly());

        internal static string SoThuTuBatDauKhongDuocPhepNhoHonKhong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("SoThuTuBatDauKhongDuocPhepNhoHonKhong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
