using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ComboTHX.Set.SetDelegate.SetDelegateFocusControlNext
{
    class SetDelegateFocusControlNextFactory
    {
        internal static ISetDelegateFocusControlNext MakeISetDelegateFocusControlNext()
        {
            ISetDelegateFocusControlNext result = null;
            try
            {
                result = new SetDelegateFocusControlNext();
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
