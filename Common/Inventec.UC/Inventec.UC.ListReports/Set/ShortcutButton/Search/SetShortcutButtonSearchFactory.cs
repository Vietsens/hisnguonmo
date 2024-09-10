using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ListReports.Set.ShortcutButton.Search
{
    class SetShortcutButtonSearchFactory
    {
        internal static ISetShortcutButtonSearch MakeISetShortcutButtonSearch()
        {
            ISetShortcutButtonSearch result = null;
            try
            {
                result = new SetShortcutButtonSearch();
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
