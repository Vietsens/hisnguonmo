using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.Login.Set.SetDelegateButtonConfig
{
    class SetDelefateButtonConfigFactory
    {
        internal static ISetDelefateButtonConfig MakeISetDelefateButtonConfig()
        {
            ISetDelefateButtonConfig result = null;
            try
            {
                result = new SetDelefateButtonConfig();
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
