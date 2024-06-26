using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.TrackingInMediRecord.Resources
{
    public class ResourceLanguageManager
    {
        public static ResourceManager LanguageResource { get; set; }
        internal static void InitResourceLanguageManager()
        {
            try
            {
                ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.TrackingInMediRecord.Resources.Lang", typeof(HIS.Desktop.Plugins.TrackingInMediRecord.TrackingInMediRecord.frmTrackingInMediRecord).Assembly);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
