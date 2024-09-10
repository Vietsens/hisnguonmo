using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ComboEthnic.Set.SetFocusComboEthnic
{
    class SetFocusComboEthnicFactory
    {
        internal static ISetFocusComboEthnic MakeISetFocusComboEthnic()
        {
            ISetFocusComboEthnic result = null;
            try
            {
                result = new SetFocusComboEthnic();
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
