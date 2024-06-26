using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.AssignNutritionEdit.Resources
{
    class ResourceLanguageManager
    {
        internal static ResourceManager LanguageResource { get; set; }
        internal static void InitResourceLanguageManager()
        {
            try
            {
                LanguageResource = new ResourceManager("HIS.Desktop.Plugins.AssignNutritionEdit.Resources.Lang", typeof(HIS.Desktop.Plugins.AssignNutritionEdit.Run.frmAssignNutritionEdit).Assembly);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
