using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ListReports.Sar.SarReport.Public
{
    class SarReportPublic : Inventec.UC.ListReports.Base.BusinessBase
    {
        internal SarReportPublic()
            : base()
        {

        }

        internal SarReportPublic(CommonParam paramPublic)
            : base(paramPublic)
        {

        }

        internal ISarReportPublic Behavior { get; set; }

        internal SAR.EFMODEL.DataModels.SAR_REPORT Public()
        {
            SAR.EFMODEL.DataModels.SAR_REPORT result = null;
            try
            {
                result = Behavior.Public();
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
