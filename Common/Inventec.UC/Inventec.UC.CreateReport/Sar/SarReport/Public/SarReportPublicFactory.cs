using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.CreateReport.Sar.SarReport.Public
{
    class SarReportPublicFactory : Inventec.UC.CreateReport.Base.GetBase
    {
        internal SarReportPublicFactory()
            : base()
        {

        }

        internal SarReportPublicFactory(CommonParam paramFactory)
            : base(paramFactory)
        {

        }

        internal SarReportPublic createFactory(SAR.EFMODEL.DataModels.SAR_REPORT Data)
        {
            try
            {
                SarReportPublic publicConcrete = new SarReportPublic(param);
                publicConcrete.Behavior = new SarReportPublicBehaviorDefault(param, Data);
                return publicConcrete;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }
    }
}
