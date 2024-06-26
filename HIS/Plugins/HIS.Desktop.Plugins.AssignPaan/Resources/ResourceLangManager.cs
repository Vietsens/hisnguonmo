using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.AssignPaan.Resources
{
    class ResourceLangManager
    {
        internal static ResourceManager LanguageFrmAssignPaan { get; set; }
        internal static void InitResourceLanguageManager()
        {
            try
            {
                LanguageFrmAssignPaan = new ResourceManager("HIS.Desktop.Plugins.AssignPaan.Resources.Lang", typeof(HIS.Desktop.Plugins.AssignPaan.frmAssignPaan).Assembly);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
