using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ComboProvince.Set.SetDelegateLoadCbDistrictFromProvince
{
    class SetDelegateLoadCbDistrictFromProvinceFactory
    {
        internal static ISetDelegateLoadCbDistrictFromProvince MakeISetDelegateLoadCbDistrictFromProvince()
        {
            ISetDelegateLoadCbDistrictFromProvince result = null;
            try
            {
                result = new SetDelegateLoadCbDistrictFromProvince();
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
