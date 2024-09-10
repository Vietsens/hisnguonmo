using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.EventLogControl.Set.Delegate.SetDelegateHasException
{
    class SetDelegateHasExceptionFactory
    {
        internal static ISetDelegateHasException MakeISetDelegateHasException()
        {
            SetDelegateHasException result = null;
            try
            {
                result = new SetDelegateHasException();
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
