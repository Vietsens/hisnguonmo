using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ComboTHX.Set.SetDelegate.SetDelegateSetValueComboProvince
{
    class SetDelegateSetValueComboProvinceFactory
    {
        internal static ISetDelegateSetValueComboProvince MakeISetDelegateSetValueComboProvince()
        {
            ISetDelegateSetValueComboProvince result = null;
            try
            {
                result = new SetDelegateSetValueComboProvince();
            }
            catch (Exception ex)
            {
                result = null;
            }
            return result;
        }
    }
}
