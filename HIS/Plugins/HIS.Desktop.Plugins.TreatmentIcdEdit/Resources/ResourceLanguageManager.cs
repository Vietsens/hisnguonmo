using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.TreatmentIcdEdit.Resources
{
    class ResourceLanguageManager
    {
        public static ResourceManager LanguageFormTreatmentIcdEdit = new ResourceManager(
            "HIS.Desktop.Plugins.TreatmentIcdEdit.Resources.Lang",
            typeof(HIS.Desktop.Plugins.TreatmentIcdEdit.FormTreatmentIcdEdit).Assembly);
    }
}
