using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ExpMestOtherExport.Resources
{
  class ResourceLanguageManager
    {
        internal static ResourceManager LanguageUCExpMestOtherExport { get; set; }
        internal static void InitResourceLanguageManager()
        {
            try
            {
                LanguageUCExpMestOtherExport = new ResourceManager("HIS.Desktop.Plugins.ExpMestOtherExport.Resources.Lang", typeof(HIS.Desktop.Plugins.ExpMestOtherExport.UCExpMestOtherExport).Assembly);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public static ResourceManager LanguageResource { get; set; }
        public static ResourceManager LanguageResource__frmDetail { get; set; }
    }
}
