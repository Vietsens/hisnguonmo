using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ComboTHX.Set.SetDelegate.SetDelegateLoadComboDistrict
{
    class SetDelegateLoadComboDistrictFactory
    {
        internal static ISetDelegateLoadComboDistrict MakeISetDelegateLoadComboDistrict()
        {
            ISetDelegateLoadComboDistrict result = null;
            try
            {
                result = new SetDelegateLoadComboDistrict();
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
