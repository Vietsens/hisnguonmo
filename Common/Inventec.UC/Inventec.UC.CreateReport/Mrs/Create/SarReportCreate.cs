using Inventec.Common.Logging;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.CreateReport.Mrs.Create
{
    class SarReportCreate : Inventec.UC.CreateReport.Base.BusinessBase
    {
        internal SarReportCreate()
            : base()
        {

        }

        internal SarReportCreate(CommonParam paramUpdate)
            : base(paramUpdate)
        {

        }

        internal ISarReportCreate Behavior { get; set; }

        internal bool Create(MRS.SDO.CreateReportSDO Data)
        {
            try
            {
                return Behavior.Create(Data);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                param.HasException = true;
            }
            return false;
        }
    }
}
