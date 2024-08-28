using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ComboTHX.Set.SetValueComboTHX
{
    class SetValueComboTHXFactory
    {
        internal static ISetValueComboTHX MakeISetValueComboTHX()
        {
            ISetValueComboTHX result = null;
            try
            {
                result = new SetValueComboTHX();
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
