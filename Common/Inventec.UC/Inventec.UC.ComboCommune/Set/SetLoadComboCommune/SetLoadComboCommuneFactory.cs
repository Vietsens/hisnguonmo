using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ComboCommune.Set.SetLoadComboCommune
{
    class SetLoadComboCommuneFactory
    {
        internal static ISetLoadComboCommune MakeISetLoadComboCommune()
        {
            ISetLoadComboCommune result = null;
            try
            {
                result = new SetLoadComboCommune();
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
