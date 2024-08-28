using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.CreateReport.Sar.SarReport.Lock
{
    internal class SarReportLockFactory : Inventec.UC.CreateReport.Base.GetBase
    {
        internal SarReportLockFactory()
            : base()
        {

        }

        internal SarReportLockFactory(CommonParam paramFactory)
            : base(paramFactory)
        {

        }

        internal SarReportLock createFactory(SAR.EFMODEL.DataModels.SAR_REPORT Data)
        {
            try
            {
                SarReportLock lockConcrete = new SarReportLock(param);
                lockConcrete.Behavior = new SarReportLockBehaviorDefault(param, Data);
                return lockConcrete;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }
    }
}
