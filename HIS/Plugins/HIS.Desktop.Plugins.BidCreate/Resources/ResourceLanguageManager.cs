using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.BidCreate.Resources
{
    class ResourceLanguageManager
    {
        public static ResourceManager LanguageUCBidCreate { get; set; }

        internal static void InitResourceLanguageManager()
        {
            try
            {
                ResourceLanguageManager.LanguageUCBidCreate = new ResourceManager("HIS.Desktop.Plugins.BidCreate.Resources.Lang", typeof(HIS.Desktop.Plugins.BidCreate.UCBidCreate).Assembly);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
