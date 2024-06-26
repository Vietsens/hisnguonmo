using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Resources;

namespace TYT.Desktop.Plugins.TYTKhhList.Base
{
    class ResourceLangManager
    {
        internal static ResourceManager LanguageUCListTYTKhhList { get; set; }
        internal static ResourceManager LanguageFrmHeinCard { get; set; }
        internal static void InitResourceLanguageManager()
        {
            try
            {
                LanguageUCListTYTKhhList = new ResourceManager("TYT.Desktop.Plugins.TYTKhhList.Resources.Lang", typeof(TYT.Desktop.Plugins.TYTKhhList.UCListTYTKhhList).Assembly);
                
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
