using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ListReports.Base
{
    public class ResouceManager
    {
        public static void ResourceLanguageManager()
        {
            try
            {
                Resources.ResourceLanguageManager.LanguageListReport = new ResourceManager("Inventec.UC.ListReports.Resources.Lang", typeof(UC.ListReports.Design.Template2.Template2).Assembly);
                //Resources.ResourceLanguageManager.LanguageListReport = new ResourceManager
                //("Inventec.UC.ListReports.Resources.Lang", typeof
                //(UC.ListReports.Design.Template3.Template3).Assembly);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
