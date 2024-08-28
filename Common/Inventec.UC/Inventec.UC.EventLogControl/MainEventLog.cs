using Inventec.UC.EventLogControl.Data;
using Inventec.UC.EventLogControl.Init;
using Inventec.UC.EventLogControl.Set.Delegate.SetDelegateHasException;
using Inventec.UC.EventLogControl.Set.MeShow;
using Inventec.UC.EventLogControl.Set.Shortcut.ShortcutButtonRefresh;
using Inventec.UC.EventLogControl.Set.Shortcut.ShortcutButtonSearch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.UC.EventLogControl
{
    public partial class MainEventLog
    {
        public enum EnumTemplate
        {
            TEMPLATE1,
            TEMPLATE2,
            TEMPLATE3
        }

        public UserControl Init(EnumTemplate Template, Data.DataInit Data)
        {
            UserControl result = null;
            try
            {
                result = InitFactory.MakeIInit().InitUC(Template, Data);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }

        public UserControl Init3(DataInit3 Data)
        {
            try
            {
                return InitFactory3.MakeIInit().InitUC(Data);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }

        public bool SetDelegateHasException(UserControl UC, ProcessHasException HasException)
        {
            bool result = false;
            try
            {
                result = SetDelegateHasExceptionFactory.MakeISetDelegateHasException().SetDelegate(UC, HasException);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        public void MeShow(UserControl UC)
        {
            try
            {
                SetMeShowUCFactory.MakeISetMeShowUC().MeShow(UC);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void ShortcutButtonSearchClick(UserControl UC)
        {
            try
            {
                ShortcutButtonSearchFactory.MakeIShortcutButtonSearch().Button_Click(UC);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void ShortcutButtonRefreshClick(UserControl UC)
        {
            try
            {
                ShortcutButtonRefreshFactory.MakeIShortcutButtonRefresh().Button_Click(UC);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
