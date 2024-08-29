using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.Loading.Set.Delegate.SetDelegateBWDoWorker
{
    class SetDelegateBWDoWorkerFactory
    {
        internal static ISetDelegateBWDoWorker MakeISetDelegateBWDoWorker()
        {
            ISetDelegateBWDoWorker result = null;
            try
            {
                result = new SetDelegateBWDoWorker();
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
