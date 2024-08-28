using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.CreateReport.Sar.SarReport.Lock
{
    class SarReportLock : Inventec.UC.CreateReport.Base.BusinessBase
    {
        internal SarReportLock()
            : base()
        {

        }

        internal SarReportLock(CommonParam paramGet)
            : base(paramGet)
        {

        }

        internal ISarReportLock Behavior { get; set; }

        internal SAR.EFMODEL.DataModels.SAR_REPORT Lock()
        {
            SAR.EFMODEL.DataModels.SAR_REPORT result = null;
            try
            {
                result = Behavior.Lock();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                param.HasException = true;
                result = null;
            }
            return result;
        }
    }
}
