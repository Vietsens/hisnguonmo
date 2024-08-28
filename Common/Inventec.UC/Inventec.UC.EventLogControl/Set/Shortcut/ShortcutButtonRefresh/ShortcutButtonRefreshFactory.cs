using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.EventLogControl.Set.Shortcut.ShortcutButtonRefresh
{
    class ShortcutButtonRefreshFactory
    {
        internal static IShortcutButtonRefresh MakeIShortcutButtonRefresh()
        {
            IShortcutButtonRefresh result = null;
            try
            {
                result = new ShortcutButtonRefresh();
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
