using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisBedMapping.Resources
{
    class ResourceLanguageManager
    {
        internal static ResourceManager LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisBedMapping.Resources.Lang", typeof(HIS.Desktop.Plugins.HisBedMapping.UCBedMapping).Assembly);
    }
}
