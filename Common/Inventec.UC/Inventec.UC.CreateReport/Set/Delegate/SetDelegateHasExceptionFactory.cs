using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.CreateReport.Set.Delegate
{
    class SetDelegateHasExceptionFactory
    {
        internal static ISetDelegateHasException MakeISetDelegateHasException()
        {
            ISetDelegateHasException result = null;
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
