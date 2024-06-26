using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.MedicalStoreV2.Resources
{
    public class ResourceLanguageManager
    {
        public static ResourceManager LanguageResource { get; set; }
        public static ResourceManager LanguageResource_frmContentRefused { get; set; }
        public static ResourceManager LanguageResource_frmRejectStore { get; set; }
        public static ResourceManager LanguageResource_frmSaveStore { get; set; }
        public static ResourceManager LanguageResource_TreatmentBorrow { get; set; }
    }
}
