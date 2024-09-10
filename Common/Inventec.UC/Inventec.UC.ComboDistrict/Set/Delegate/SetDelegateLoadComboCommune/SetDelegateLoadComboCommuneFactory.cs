using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ComboDistrict.Set.SetDelegateLoadComboCommune
{
    class SetDelegateLoadComboCommuneFactory
    {
        internal static ISetDelegateLoadComboCommune MakeISetDelegateLoadComboCommune()
        {
            ISetDelegateLoadComboCommune result = null;
            try
            {
                result = new SetDelegateLoadComboCommune();
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
