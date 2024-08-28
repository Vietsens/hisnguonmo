using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ComboProvince.Set.SetDelegateFocus
{
    class SetDelegateFocusFactory
    {
        internal static ISetDelegateFocus MakeISetDelegateFocus()
        {
            ISetDelegateFocus result = null;
            try
            {
                result = new SetDelegateFocus();
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
