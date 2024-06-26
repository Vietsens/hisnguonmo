using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.PublicMedicineGeneral.Resources
{
    class ResourceLanguageManager
    {
        public static ResourceManager LanguageFormExaminationReqEdit { get; set; }

        internal static void InitResourceLanguageManager()
        {
            try
            {
                ResourceLanguageManager.LanguageFormExaminationReqEdit = new ResourceManager("HIS.Desktop.Plugins.PublicMedicineGeneral.Resources.Lang", typeof(HIS.Desktop.Plugins.PublicMedicineGeneral.FormPublicMedicineGeneral).Assembly);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
