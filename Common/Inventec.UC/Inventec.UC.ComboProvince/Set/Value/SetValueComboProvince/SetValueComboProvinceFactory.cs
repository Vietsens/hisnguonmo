using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ComboProvince.Set.SetValueComboProvince
{
    class SetValueComboProvinceFactory
    {
        internal static ISetValueComboProvince MakeISetValueComboProvince()
        {
            ISetValueComboProvince result = null;
            try
            {
                result = new SetValueComboProvince();
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
