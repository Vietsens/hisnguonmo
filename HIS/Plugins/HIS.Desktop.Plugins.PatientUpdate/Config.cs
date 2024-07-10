using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.PatientUpdate
{
	class Config
	{
        private const string IsPatientClassifyOption = "HIS.Desktop.Plugins.PatientClassifyOption";
        public static bool IsPatientClassify
        {
            get
            {
                return HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(IsPatientClassifyOption) == "1";
            }
        }
    }
}
