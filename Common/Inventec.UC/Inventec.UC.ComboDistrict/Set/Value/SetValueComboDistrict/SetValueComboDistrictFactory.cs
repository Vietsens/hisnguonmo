using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ComboDistrict.Set.SetValueComboDistrict
{
    class SetValueComboDistrictFactory
    {
        internal static ISetValueComboDistrict MakeISetValueComboDistrict()
        {
            ISetValueComboDistrict result = null;
            try
            {
                result = new SetValueComboDistrict();
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
