using System;
using System.Resources;

namespace HIS.Desktop.Plugins.HisCheckBeforeTransfusionBlood.Resource
{
    class ResourceMessage
    {
        static System.Resources.ResourceManager languageMessage =
            new System.Resources.ResourceManager(
                "HIS.Desktop.Plugins.HisCheckBeforeTransfusionBlood.Resource.Message.Lang",
                System.Reflection.Assembly.GetExecutingAssembly());

        /// <summary>Loại máu chưa khai báo vị trí ống nghiệm.</summary>
        internal static string LoaiMauChuaKhaiBaoViTriOngNghiem
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value(
                        "LoaiMauChuaKhaiBaoViTriOngNghiem",
                        languageMessage,
                        Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
