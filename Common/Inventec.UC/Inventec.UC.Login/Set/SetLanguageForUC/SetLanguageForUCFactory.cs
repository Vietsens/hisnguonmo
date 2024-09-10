using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.Login.Set.SetLanguageForUC
{
    class SetLanguageForUCFactory
    {
        internal static ISetLanguageForUC MakeISetLanguageForUC()
        {
            ISetLanguageForUC result = null;
            try
            {
                result = new SetLanguageForUC();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }
    }
}
