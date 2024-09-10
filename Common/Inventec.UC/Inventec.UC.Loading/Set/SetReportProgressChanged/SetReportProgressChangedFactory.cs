using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.Loading.Set.SetReportProgressChanged
{
    class SetReportProgressChangedFactory
    {
        internal static ISetReportProgressChanged MakeISetReportProgressChanged()
        {
            ISetReportProgressChanged result = null;
            try
            {
                result = new SetReportProgressChanged();
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
