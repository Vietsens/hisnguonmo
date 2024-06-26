using System;

namespace HIS.Desktop.Plugins.MaterialType.MaterialTypeList.Resources
{
    class ResourceMessage
    {
        static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.Desktop.Plugins.MaterialType.Resources.Message.Lang", System.Reflection.Assembly.GetExecutingAssembly());
        internal static string Plugin_XuatFileThanhCong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_XuatFileThanhCong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string Plugin_XuatFileThatBai
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_XuatFileThatBai", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
