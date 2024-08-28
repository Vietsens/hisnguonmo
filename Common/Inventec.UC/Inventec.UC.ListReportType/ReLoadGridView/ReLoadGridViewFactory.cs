using SAR.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ListReportType.ReLoadGridView
{
    class ReLoadGridViewFactory
    {
        internal static IReLoadGridView MakeIReLoadGridView(long ReportTypeGroupId)
        {
            IReLoadGridView result = null;
            try
            {
                result = new ReLoadGridViewBehavior(ReportTypeGroupId);
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
