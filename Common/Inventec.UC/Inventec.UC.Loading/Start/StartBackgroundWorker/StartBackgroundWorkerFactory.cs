using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.Loading.Start.StartBackgroundWorker
{
    class StartBackgroundWorkerFactory
    {
        internal static IStartBackgroundWorker MakeIStartBackgroundWorker()
        {
            IStartBackgroundWorker result = null;
            try
            {
                result = new StartBackgroundWorker();
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
