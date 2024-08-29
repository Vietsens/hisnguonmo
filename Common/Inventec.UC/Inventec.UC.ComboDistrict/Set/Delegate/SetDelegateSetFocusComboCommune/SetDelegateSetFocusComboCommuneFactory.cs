using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ComboDistrict.Set.SetDelegateSetFocusComboCommune
{
    class SetDelegateSetFocusComboCommuneFactory
    {
        internal static ISetDelegateSetFocusComboCommune MakeISetDelegateSetFocusComboCommune()
        {
            ISetDelegateSetFocusComboCommune result = null;
            try
            {
                result = new SetDelegateSetFocusComboCommune();
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
