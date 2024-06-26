using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Resources;

namespace TYT.Desktop.Plugins.TYTFetusBornList.Base
{
    class ResourceLangManager
    {
        internal static ResourceManager LanguageUCListTYTFetusExamList { get; set; }
        internal static ResourceManager LanguageFrmHeinCard { get; set; }
        internal static void InitResourceLanguageManager()
        {
            try
            {
                LanguageUCListTYTFetusExamList = new ResourceManager("TYT.Desktop.Plugins.TYTFetusBornList.Resources.Lang", typeof(TYT.Desktop.Plugins.TYTFetusBornList.UCListTYTFetusBornList).Assembly);
                
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
