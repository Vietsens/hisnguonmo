using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ComboNational.Set.SetFocusComboNational
{
    class SetFocusComboNationalFactory
    {
        internal static ISetFocusComboNational MakeISetFocusComboNational()
        {
            ISetFocusComboNational result = null;
            try
            {
                result = new SetFocusComboNational();
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
