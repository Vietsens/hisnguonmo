using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ComboCommune.Set.SetValueComboCommune
{
    class SetValueComboCommuneFactory
    {
        internal static ISetValueComboCommune MakeISetValueComboCommune()
        {
            ISetValueComboCommune result = null;
            try
            {
                result = new SetValueComboCommune();
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
