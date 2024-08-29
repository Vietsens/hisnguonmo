using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ComboCommune.Set.SetDelegateGetValueComboProvince
{
    class SetDelegateGetValueComboProvinceFactory
    {
        internal static ISetDelegateGetValueComboProvince MakeISetDelegateGetValueComboProvince()
        {
            ISetDelegateGetValueComboProvince result = null;
            try
            {
                result = new SetDelegateGetValueComboProvince();
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
