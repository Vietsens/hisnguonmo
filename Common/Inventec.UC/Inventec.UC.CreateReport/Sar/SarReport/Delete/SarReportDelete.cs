using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.CreateReport.Sar.SarReport.Delete
{
    class SarReportDelete : Inventec.UC.CreateReport.Base.BusinessBase
    {
        internal SarReportDelete()
            : base()
        {

        }

        internal SarReportDelete(CommonParam paramGet)
            : base(paramGet)
        {

        }

        internal ISarReportDelete Behavior { get; set; }

        internal bool Delete()
        {
            bool result = false;
            try
            {
                result = Behavior.Delete();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }
    }
}
