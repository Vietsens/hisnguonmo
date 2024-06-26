using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ServiceReqResultView.Resources
{
    public class ResourceLanguageManager
    {
        public static ResourceManager LanguageResource = new System.Resources.ResourceManager("HIS.Desktop.Plugins.ServiceReqResultView.Resources.Lang", System.Reflection.Assembly.GetExecutingAssembly());
        private static System.Globalization.CultureInfo cultureLang = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();

        internal static string GetValue(string key)
        {
            string result = null;
            try
            {
                result = Inventec.Common.Resource.Get.Value(key, LanguageResource, cultureLang);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }
}
