using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ListReports.Set.ShortcutButton.Refresh
{
    class SetShortcutButtonRefreshFactory
    {
        internal static ISetShortcutButtonRefresh MakeISetShortcutButtonRefresh()
        {
            ISetShortcutButtonRefresh result = null;
            try
            {
                result = new SetShortcutButtonRefresh();
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
