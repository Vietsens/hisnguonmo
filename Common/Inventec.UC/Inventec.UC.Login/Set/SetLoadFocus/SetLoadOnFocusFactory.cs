using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.Login.Set.SetLoadFocus
{
    class SetLoadOnFocusFactory
    {
        internal static ISetLoadOnFocus MakeISetLoadOnFocus()
        {
            ISetLoadOnFocus result = null;
            try
            {
                result = new SetLoadOnFocus();
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
