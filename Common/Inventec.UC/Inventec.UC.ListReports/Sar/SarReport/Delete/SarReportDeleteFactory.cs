using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ListReports.Sar.SarReport.Delete
{
    class SarReportDeleteFactory : Inventec.UC.ListReports.Base.GetBase
    {
        internal SarReportDeleteFactory()
            : base()
        {

        }

        internal SarReportDeleteFactory(CommonParam paramFactory)
            : base(paramFactory)
        {

        }

        internal SarReportDelete createFactory(SAR.EFMODEL.DataModels.SAR_REPORT Data)
        {
            try
            {
                SarReportDelete deleteConcrete = new SarReportDelete(param);
                deleteConcrete.Behavior = new SarReportDeleteBehaviorDefault(param, Data);
                return deleteConcrete;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }
    }
}
