using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.EventLogControl.Set.Shortcut.ShortcutButtonSearch
{
    class ShortcutButtonSearchFactory
    {
        internal static IShortcutButtonSearch MakeIShortcutButtonSearch()
        {
            IShortcutButtonSearch result = null;
            try
            {
                result = new ShortcutButtonSearch();
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
