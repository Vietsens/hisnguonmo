using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.EventLogControl.Set.Shortcut.ShortcutButtonSearch
{
    class ShortcutButtonSearch : IShortcutButtonSearch
    {
        public void Button_Click(System.Windows.Forms.UserControl UC)
        {
            try
            {
                if (UC.GetType() == typeof(Design.Template1.Template1))
                {
                    Design.Template1.Template1 UCEventLog = (Design.Template1.Template1)UC;
                    UCEventLog.ShortcutButtonSearch();
                }
                else if (UC.GetType() == typeof(Design.Template2.Template2))
                {
                    Design.Template2.Template2 UCEventLog = (Design.Template2.Template2)UC;
                    UCEventLog.ShortcutButtonSearch();
                }
                else if (UC.GetType() == typeof(Design.Template3.Template3))
                {
                    Design.Template3.Template3 UCEventLog = (Design.Template3.Template3)UC;
                    UCEventLog.ShortcutButtonSearch();
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => UC), UC));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
