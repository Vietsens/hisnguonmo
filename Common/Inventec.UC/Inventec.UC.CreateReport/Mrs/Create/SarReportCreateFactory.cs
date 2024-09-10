using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.CreateReport.Mrs.Create
{
    class SarReportCreateFactory : Inventec.UC.CreateReport.Base.GetBase
    {
        internal SarReportCreateFactory()
            : base()
        {

        }

        internal SarReportCreateFactory(CommonParam paramFactory)
            : base(paramFactory)
        {

        }

        internal SarReportCreate createFactory(MRS.SDO.CreateReportSDO dataForm)
        {
            try
            {
                SarReportCreate createConcrete = new SarReportCreate(param);
                createConcrete.Behavior = new SarReportCreateBehaviorDefault(param, dataForm);
                return createConcrete;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }
    }
}
