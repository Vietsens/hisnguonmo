using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ComboTHX.Set.SetDelegate.SetDelegateFocusComboProvince
{
    class SetDelegateFocusComboProvinceFactory
    {
        internal static ISetDelegateFocusComboProvince MakeISetDelegateFocusComboProvince()
        {
            ISetDelegateFocusComboProvince result = null;
            try
            {
                result = new SetDelegateFocusComboProvince();
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
