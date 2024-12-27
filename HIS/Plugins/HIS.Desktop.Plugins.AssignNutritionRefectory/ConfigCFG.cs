using HIS.Desktop.LocalStorage.HisConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.AssignNutritionRefectory
{
    public class ConfigCFG
    {
        public static string ALLOW_MODIFYING_OF_STARTED = HisConfigs.Get<string>("MOS.HIS_SERVICE_REQ.ALLOW_MODIFYING_OF_STARTED");
       
    }
}
