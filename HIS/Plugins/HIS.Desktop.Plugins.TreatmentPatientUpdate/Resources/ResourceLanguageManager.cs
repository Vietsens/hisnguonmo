using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.TreatmentPatientUpdate.Resources
{
    class ResourceLanguageManager
    {
        public static ResourceManager LanguageFormTreatmentPatientChange { get; set; }

        internal static void InitResourceLanguageManager()
        {
            try
            {
                ResourceLanguageManager.LanguageFormTreatmentPatientChange = new ResourceManager("HIS.Desktop.Plugins.TreatmentPatientUpdate.Resources.Lang", typeof(HIS.Desktop.Plugins.TreatmentPatientUpdate.FormTreatmentPatientUpdate).Assembly);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
