using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ComboCommune.Set.SetFocusComboCommune
{
    class SetFocusComboCommuneFactory
    {
        internal static ISetFocusComboCommune MakeISetFocusComboCommune()
        {
            ISetFocusComboCommune result = null;
            try
            {
                result = new SetFocusComboCommune();
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
