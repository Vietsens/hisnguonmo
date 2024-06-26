using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.AssignServiceEdit.Resources
{
    class ResourceLanguageManager
    {
        public static ResourceManager LanguageFormAssignServiceEdit { get; set; }

        internal static void InitResourceLanguageManager()
        {
            try
            {
                ResourceLanguageManager.LanguageFormAssignServiceEdit = new ResourceManager("HIS.Desktop.Plugins.AssignServiceEdit.Resources.Lang", typeof(HIS.Desktop.Plugins.AssignServiceEdit.FormAssignServiceEdit).Assembly);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
