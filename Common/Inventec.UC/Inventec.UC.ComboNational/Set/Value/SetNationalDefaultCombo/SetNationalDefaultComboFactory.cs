using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ComboNational.Set.Value.SetNationalDefaultCombo
{
    class SetNationalDefaultComboFactory
    {
        internal static ISetNationalDefaultCombo MakeISetNationalDefaultCombo()
        {
            ISetNationalDefaultCombo result = null;
            try
            {
                result = new SetNationalDefaultCombo();
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
