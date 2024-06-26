using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.SamplePathologyReq.Resources
{
    public class ResourceMessage
    {
        static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.Desktop.Plugins.SamplePathologyReq.Resources.Message.Lang", System.Reflection.Assembly.GetExecutingAssembly());

        internal static string ChuaNhapBlock
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_SAMPLE_PATHOLOGY_REQ_KHONG_CO_BLOCK", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
