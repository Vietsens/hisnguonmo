using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ListReportType.Base
{
    public class ResouceManager
    {
        public static void ResourceLanguageManager()
        {
            try
            {
                Resources.ResourceLanguageManager.LanguageListReportType = new ResourceManager("Inventec.UC.ListReportType.Resources.Lang", typeof(UC.ListReportType.Design.Template1.Template1).Assembly);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
