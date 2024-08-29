using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ComboDistrict.Set.SetDelegateGetValueProvince
{
    class SetDelegateGetValueProvinceFactory
    {
        internal static ISetDelegateGetValueProvince MakeISetDelegateGetValueProvince()
        {
            ISetDelegateGetValueProvince result = null;
            try
            {
                result = new SetDelegateGetValueProvince();
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
