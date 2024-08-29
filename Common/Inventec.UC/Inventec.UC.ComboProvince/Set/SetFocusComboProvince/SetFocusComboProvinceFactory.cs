using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ComboProvince.Set.SetFocusComboProvince
{
    class SetFocusComboProvinceFactory
    {
        internal static ISetFocusComboProvince MakeISetFocusComboProvince()
        {
            ISetFocusComboProvince result = null;
            try
            {
                result = new SetFocusComboProvince();
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
