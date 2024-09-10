using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ComboCommune.Set.SetTextLabelLanguage
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
