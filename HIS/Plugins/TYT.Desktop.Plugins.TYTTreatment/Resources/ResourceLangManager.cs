using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Resources;

namespace TYT.Desktop.Plugins.TYTTreatment.Base
{
    class ResourceLangManager
    {
        internal static ResourceManager LanguageUCListTYTTreatment { get; set; }
        internal static ResourceManager LanguageFrmHeinCard { get; set; }
        internal static void InitResourceLanguageManager()
        {
            try
            {
                LanguageUCListTYTTreatment = new ResourceManager("TYT.Desktop.Plugins.TYTTreatment.Resources.Lang", typeof(TYT.Desktop.Plugins.TYTTreatment.UCListTYTTreatment).Assembly);
                
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
