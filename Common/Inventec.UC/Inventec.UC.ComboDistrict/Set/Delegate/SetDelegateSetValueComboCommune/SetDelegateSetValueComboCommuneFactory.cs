using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ComboDistrict.Set.SetDelegateSetValueComboCommune
{
    class SetDelegateSetValueComboCommuneFactory
    {
        internal static ISetDelegateSetValueComboCommune MakeISetDelegateSetValueComboCommune()
        {
            ISetDelegateSetValueComboCommune result = null;
            try
            {
                result = new SetDelegateSetValueComboCommune();
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
