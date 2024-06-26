using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ExpMestSaleTransactionList.Base
{
    class ResourceLangManager
    {
        internal static ResourceManager LanguageFrmExpMestSaleTransactionList { get; set; }

        internal static void InitResourceLanguageManager()
        {
            try
            {
                LanguageFrmExpMestSaleTransactionList = new ResourceManager("HIS.Desktop.Plugins.ExpMestSaleTransactionList.Resources.Lang", typeof(HIS.Desktop.Plugins.ExpMestSaleTransactionList.frmExpMestSaleTransactionList).Assembly);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
