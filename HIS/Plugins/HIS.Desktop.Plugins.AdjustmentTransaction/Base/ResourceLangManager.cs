using HIS.Desktop.Plugins.AdjustmentTransaction.AdjustmentTransaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.AdjustmentTransaction.Base
{
    class ResourceLangManager
    {
        internal static ResourceManager LanguageFrmAdjustmentTransaction { get; set; }

        internal static void InitResourceLanguageManager()
        {
            try
            {
                LanguageFrmAdjustmentTransaction = new ResourceManager("HIS.Desktop.Plugins.AdjustmentTransaction.Resources.Lang", typeof(AdjustmentTransaction.frmAdjustmentTransaction).Assembly);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
