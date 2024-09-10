using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.Feedback.Set.Delegate.SetDelegateHasExceptionApi
{
    class SetDelegateHasExceptionApiFactory
    {
        internal static ISetDelegateHasExceptionApi MakeISetDelegateHasExceptionApi()
        {
            ISetDelegateHasExceptionApi result = null;
            try
            {
                result = new SetDelegateHasExceptionApi();
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
