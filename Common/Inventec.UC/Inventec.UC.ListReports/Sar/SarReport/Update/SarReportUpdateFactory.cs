using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ListReports.Sar.SarReport.Update
{
    class SarReportUpdateFactory : Inventec.UC.ListReports.Base.GetBase
    {
        internal SarReportUpdateFactory()
            : base()
        {

        }

        internal SarReportUpdateFactory(CommonParam paramFactory)
            : base(paramFactory)
        {

        }

        internal SarReportUpdate createFactory(SAR.EFMODEL.DataModels.SAR_REPORT Data)
        {
            try
            {
                SarReportUpdate updateConcrete = new SarReportUpdate(param);
                updateConcrete.Behavior = new SarReportUpdateBehaviorDefault(param, Data);
                return updateConcrete;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }
    }
}
