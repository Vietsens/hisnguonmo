using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ComboProvince.Set.SetDelegateValueCboDistrictAndCommune
{
    class SetDelegateValueCboDistrictAndCommuneFactory
    {
        internal static ISetDelegateValueCboDistrictAndCommune MakeISetDelegateValueCboDistrictAndCommune()
        {
            ISetDelegateValueCboDistrictAndCommune result = null;
            try
            {
                result = new SetDelegateValueCboDistrictAndCommune();
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
