using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ModuleButtonCustomize.Resources
{
    class ResourceMessage
    {
        static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.Desktop.Plugins.ModuleButtonCustomize.Resources.Lang", System.Reflection.Assembly.GetExecutingAssembly());

        internal static string KhongChonControlCanAnHien
        {
            get
            {
                try
                {
                   
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
