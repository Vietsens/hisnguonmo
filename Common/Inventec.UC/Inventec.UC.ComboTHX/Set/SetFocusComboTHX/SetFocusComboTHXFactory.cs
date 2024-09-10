using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ComboTHX.Set.SetFocusComboTHX
{
    class SetFocusComboTHXFactory
    {
        internal static ISetFocusComboTHX MakeISetFocusComboTHX()
        {
            ISetFocusComboTHX result = null;
            try
            {
                result = new SetFocusComboTHX();
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
