using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ComboNational.Set.Delegate.SetDelegateFocusNextControl
{
    class SetDelegateFocusNextControlFactory
    {
        internal static ISetDelegateFocusNextControl MakeISetDelegateFocusNextControl()
        {
            ISetDelegateFocusNextControl result = null;
            try
            {
                result = new SetDelegateFocusNextControl();
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
