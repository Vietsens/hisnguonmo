using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ComboCommune.Set.SetDelegateSetFocusControl
{
    class SetDelegateSetFocusControlFactory
    {
        internal static ISetDelegateSetFocusControl MakeISetDelegateSetFocusControl()
        {
            ISetDelegateSetFocusControl result = null;
            try
            {
                result = new SetDelegateSetFocusControl();
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
