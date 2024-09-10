using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.EventLogControl.Set.MeShow
{
    class SetMeShowUCFactory
    {
        internal static ISetMeShowUC MakeISetMeShowUC()
        {
            ISetMeShowUC result = null;
            try
            {
                result = new SetMeShowUC();
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
