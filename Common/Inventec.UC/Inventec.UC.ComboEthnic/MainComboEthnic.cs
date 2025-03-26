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
using Inventec.UC.ComboEthnic.Data;
using Inventec.UC.ComboEthnic.Get.GetValueEthnicCode;
using Inventec.UC.ComboEthnic.Get.GetValueEthnicName;
using Inventec.UC.ComboEthnic.Init;
using Inventec.UC.ComboEthnic.Set.ResetValueControl;
using Inventec.UC.ComboEthnic.Set.SetDelegateFocusControlNext;
using Inventec.UC.ComboEthnic.Set.SetFocusComboEthnic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.UC.ComboEthnic
{
    public partial class MainComboEthnic
    {
        public static string TEMPLATE1 = "Template1";
        public static string TEMPLATE2 = "Template2";

        #region Init UserControl
        public UserControl Init(string template, DataInitEthnic Data)
        {
            UserControl result = null;
            try
            {
                result = InitFactory.MakeIInit().InitCombo(template, Data);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }
        #endregion

        #region Set Delegate
        public bool SetDelegateFocusNext(UserControl UC, FocusNextControl FocusNext)
        {
            bool result = false;
            try
            {
                result = SetDelegateFocusControlNextFactory.MakeISetDelegateFocusControlNext().SetDelegateFocusNext(UC, FocusNext);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }
        #endregion

        #region Set Focus
        public void SetFocus(UserControl UC)
        {
            try
            {
                SetFocusComboEthnicFactory.MakeISetFocusComboEthnic().SetFocus(UC);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Get Value
        public string GetMaDanToc(UserControl UC)
        {
            string result = null;
            try
            {
                result = GetValueEthnicCodeFactory.MakeIGetValueEthnicCode().GetEthnicCode(UC);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }

        public string GetTenDanToc(UserControl UC)
        {
            string result = null;
            try
            {
                result = GetValueEthnicNameFactory.MakeIGetValueEthnicName().GetEthnicName(UC);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }
        #endregion

        #region Reset Value
        public void ResetValue(UserControl UC)
        {
            try
            {
                ResetValueControlFactory.MakeIResetValueControl().ResetValue(UC);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion
    }
}
