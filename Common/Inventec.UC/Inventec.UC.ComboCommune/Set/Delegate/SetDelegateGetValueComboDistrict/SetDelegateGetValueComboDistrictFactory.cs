using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ComboCommune.Set.SetDelegateGetValueComboDistrict
{
    class SetDelegateGetValueComboDistrictFactory
    {
        internal static ISetDelegateGetValueComboDistrict MakeISetDelegateGetValueComboDistrict()
        {
            ISetDelegateGetValueComboDistrict result = null;
            try
            {
                result = new SetDelegateGetValueComboDistrict();
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
