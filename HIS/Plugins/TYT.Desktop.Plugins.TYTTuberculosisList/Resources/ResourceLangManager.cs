using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Resources;

namespace TYT.Desktop.Plugins.TYTTuberculosisList.Base
{
    class ResourceLangManager
    {
        internal static ResourceManager LanguageUCListTytTuberculosisList { get; set; }
        internal static ResourceManager LanguageFrmHeinCard { get; set; }
        internal static void InitResourceLanguageManager()
        {
            try
            {
                LanguageUCListTytTuberculosisList = new ResourceManager("TYT.Desktop.Plugins.TytTuberculosisList.Resources.Lang", typeof(TYT.Desktop.Plugins.TYTTuberculosisList.UCListTYTTuberculosisList).Assembly);
                
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
