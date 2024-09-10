using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.CreateReport.Set.Delegate.SetGetObjectFilter
{
    class SetDelegateGetObjectFilterFactory
    {
        internal static ISetDelegateGetObjectFilter MakeISetDelegateGetObjectFilter()
        {
            ISetDelegateGetObjectFilter result = null;
            try
            {
                result = new SetDelegateGetObjectFilter();
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
