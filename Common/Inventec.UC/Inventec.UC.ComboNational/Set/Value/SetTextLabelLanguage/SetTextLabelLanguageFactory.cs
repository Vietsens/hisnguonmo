using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ComboNational.Set.Value.SetTextLabelLanguage
{
    class SetTextLabelLanguageFactory
    {
        internal static ISetTextLabelLanguage MakeISetTextLabelLanguage()
        {
            ISetTextLabelLanguage result = null;
            try
            {
                result = new SetTextLabelLanguage();
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
