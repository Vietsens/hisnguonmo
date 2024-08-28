using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.Loading.Set.Delegate.SetDelegateBWRunWorkerCompleted
{
    class SetDelegateBWRunWorkerCompletedFactory
    {
        internal static ISetDelegateBWRunWorkerCompleted MakeISetDelegateBWRunWorkerCompleted()
        {
            ISetDelegateBWRunWorkerCompleted result = null;
            try
            {
                result = new SetDelegateBWRunWorkerCompleted();
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
