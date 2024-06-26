using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Resources;

namespace TYT.Desktop.Plugins.TYTFetusExamList.Base
{
    class ResourceLangManager
    {
        internal static ResourceManager LanguageUCListTYTFetusExamList { get; set; }
        internal static ResourceManager LanguageFrmHeinCard { get; set; }
        internal static void InitResourceLanguageManager()
        {
            try
            {
                LanguageUCListTYTFetusExamList = new ResourceManager("TYT.Desktop.Plugins.TYTFetusExamList.Resources.Lang", typeof(TYT.Desktop.Plugins.TYTFetusExamList.UCListTYTFetusExamList).Assembly);
                
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
