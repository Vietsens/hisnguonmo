using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ListReports.Set.MeShow
{
    class SetUCMeShowFactory
    {
        internal static ISetUCMeShow MakeISetUCMeShow()
        {
            ISetUCMeShow result = null;
            try
            {
                result = new SetUCMeShow();
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
