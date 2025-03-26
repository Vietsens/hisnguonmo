/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *  
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *  
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * GNU General Public License for more details.
 *  
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */
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
