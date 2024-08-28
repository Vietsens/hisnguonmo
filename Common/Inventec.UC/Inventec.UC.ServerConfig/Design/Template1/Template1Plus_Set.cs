using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ServerConfig.Design.Template1
{
    internal partial class Template1
    {
        internal void SetLanguage(string lang)
        {
            try
            {
                Data.DataStore.language = (lang != null) ? lang : "vi";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
