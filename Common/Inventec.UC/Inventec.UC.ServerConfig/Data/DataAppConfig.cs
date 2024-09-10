using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ServerConfig.Data
{
    class DataAppConfig
    {
        internal static string ACS_BASE_URI = System.Configuration.ConfigurationSettings.AppSettings["Inventec.Token.ClientSystem.Acs.Base.Uri"] ?? "";
        internal static string FSS_BASE_URI = System.Configuration.ConfigurationSettings.AppSettings["fss.uri.base"] ?? "";
    }
}
