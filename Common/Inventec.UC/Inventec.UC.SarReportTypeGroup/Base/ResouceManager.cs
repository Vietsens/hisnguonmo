using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ListReportTypeGroup.Base
{
    public class ResouceManager
    {
        public static void ResourceLanguageManager()
        {
            try
            {
                Resources.ResourceLanguageManager.LanguageListReportTypeGroup = new ResourceManager("Inventec.UC.ListReportTypeGroup.Resources.Lang", typeof(UC.ListReportTypeGroup.Design.Template1.Template1).Assembly);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
