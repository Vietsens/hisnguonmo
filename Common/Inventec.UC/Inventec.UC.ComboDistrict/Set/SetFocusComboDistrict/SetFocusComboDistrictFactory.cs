using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ComboDistrict.Set.SetFocusComboDistrict
{
    class SetFocusComboDistrictFactory
    {
        internal static ISetFocusComboDistrict MakeISetFocusComboDistrict()
        {
            ISetFocusComboDistrict result = null;
            try
            {
                result = new SetFocusComboDistrict();
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
