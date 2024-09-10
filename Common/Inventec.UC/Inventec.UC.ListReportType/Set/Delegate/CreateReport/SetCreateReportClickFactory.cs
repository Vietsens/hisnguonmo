using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ListReportType.Set.Delegate.CreateReport
{
    class SetCreateReportClickFactory
    {
        internal static ISetCreateReportClick MakeISetCreateReportClick(CreateReport_Click data)
        {
            ISetCreateReportClick result = null;
            try
            {
                result = new SetCreateReportClick(data);
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
