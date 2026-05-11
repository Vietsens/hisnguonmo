using System;
using System.Resources;

namespace HIS.Desktop.Plugins.MedicineTypeCreate.Resources
{
    internal class ResourceMessage
    {
        static System.Resources.ResourceManager languageMessage =
            new System.Resources.ResourceManager(
                "HIS.Desktop.Plugins.MedicineTypeCreate.Resources.Message.Lang",
                System.Reflection.Assembly.GetExecutingAssembly());

        /// <summary>Mã CSKCB chuyển tối đa 10 ký tự</summary>
        internal static string MaCSKCBChuyenToiDa10KyTu
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value(
                        "MaCSKCBChuyenToiDa10KyTu",
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
