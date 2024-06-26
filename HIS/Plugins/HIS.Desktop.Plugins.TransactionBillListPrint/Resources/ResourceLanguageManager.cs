using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.TransactionBillListPrint.Resources
{
    class ResourceLanguageManager
    {
        internal static ResourceManager LanguageResource = new ResourceManager("HIS.Desktop.Plugins.TransactionBillListPrint.Resources.Lang", typeof(HIS.Desktop.Plugins.TransactionBillListPrint.FormTransactionBillListPrint).Assembly);

        internal static string BanChuaChonDuLieuIn
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("BanChuaChonDuLieuIn", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
