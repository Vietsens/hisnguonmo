using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ComboTHX.Set.SetDelegate.SetDelegateSetValueComboDistrict
{
    class SetDelegateSetValueComboDistrictFactory
    {
        internal static ISetDelegateSetValueComboDistrict MakeISetDelegateSetValueComboDistrict()
        {
            ISetDelegateSetValueComboDistrict result = null;
            try
            {
                result = new SetDelegateSetValueComboDistrict();
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
