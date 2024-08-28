using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ComboProvince.Set.SetLabelLanguage
{
    class SetLabelLanguageFactory
    {
        internal static ISetLabelLanguage MakeISetLabelLanguage()
        {
            ISetLabelLanguage result = null;
            try
            {
                result = new SetLabelLanguage();
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
