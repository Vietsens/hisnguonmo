using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Tutorial.Resources
{
    class ResourceLanguageManager
    {
        public static ResourceManager LanguageFormTutorial = new ResourceManager("HIS.Desktop.Plugins.Tutorial.Resources.Lang", typeof(HIS.Desktop.Plugins.Tutorial.FormTutorial).Assembly);
    }
}
