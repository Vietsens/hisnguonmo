using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.BidUpdate.Resources
{
    public class ResourceLanguageManager
    {
        public static ResourceManager LanguageResource { get; set; }
        //public static ResourceManager LanguageUCBidCreate { get; set; }
        internal static void InitResourceLanguageManager()
        {
            try
            {
                ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.BidUpdate.Resources.Lang", typeof(HIS.Desktop.Plugins.BidUpdate.frmBidUpdate).Assembly);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
