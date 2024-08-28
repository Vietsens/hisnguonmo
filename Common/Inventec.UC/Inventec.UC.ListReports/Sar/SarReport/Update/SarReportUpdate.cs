using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ListReports.Sar.SarReport.Update
{
    class SarReportUpdate : Inventec.UC.ListReports.Base.BusinessBase
    {
        internal SarReportUpdate()
            : base()
        {

        }

        internal SarReportUpdate(CommonParam paramUpdate)
            : base(paramUpdate)
        {

        }

        internal ISarReportUpdate Behavior { get; set; }

        internal SAR.EFMODEL.DataModels.SAR_REPORT Update()
        {
            SAR.EFMODEL.DataModels.SAR_REPORT result = null;
            try
            {
                result = Behavior.Update();

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
