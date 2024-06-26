using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ServiceExecute.Resources
{
    class ResourceLanguageManager
    {
        public static ResourceManager LanguageFormServiceExecute { get; set; }
        public static ResourceManager LanguageResourcefrmEkipTemp { get; set; }
        public static ResourceManager LanguageResourceFormPtttTemp { get; set; }
        public static ResourceManager LanguageResourceUcTelerik { get; set; }
        public static ResourceManager LanguageResourceUcTelerikFullWord { get; set; }
        public static ResourceManager LanguageResourceFormHistoryCLS { get; set; }
        public static ResourceManager LanguageResourceFormImageTemp { get; set; }
        public static ResourceManager LanguageResourceFormViewImage { get; set; }
        public static ResourceManager LanguageResourceFormViewImageV2 { get; set; }
        public static ResourceManager LanguageResourcefrmClsInfo { get; set; }
        public static ResourceManager LanguageResourcefrmSTTNumber { get; set; }
        public static ResourceManager LanguageResourceUCEkipUser { get; set; }
        public static ResourceManager LanguageResourceUcWord { get; set; }
        public static ResourceManager LanguageResourceUcWordFull { get; set; }


        private static System.Globalization.CultureInfo cultureLang = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();

        internal static void InitResourceLanguageManager()
        {
            try
            {
                ResourceLanguageManager.LanguageFormServiceExecute = new ResourceManager("HIS.Desktop.Plugins.ServiceExecute.Resources.Lang", typeof(HIS.Desktop.Plugins.ServiceExecute.UCServiceExecute).Assembly);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal static string GetValue(string key)
        {
            string result = null;
            try
            {
                result = Inventec.Common.Resource.Get.Value(key, LanguageFormServiceExecute, cultureLang);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }
}
