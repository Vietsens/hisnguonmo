using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ComboCommune.Set.SetDelegateSetValueComboTHX
{
    class SetDelegateSetValueComboTHXFactory
    {
        internal static ISetDelegateSetValueComboTHX MakeISetDelegateSetValueComboTHX()
        {
            ISetDelegateSetValueComboTHX result = null;
            try
            {
                result = new SetDelegateSetValueComboTHX();
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
