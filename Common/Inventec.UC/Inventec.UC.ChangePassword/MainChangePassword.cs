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
using Inventec.UC.ChangePassword.Init;
using Inventec.UC.ChangePassword.Set.SetDelegateChangePassSuccess;
using Inventec.UC.ChangePassword.Set.SetDelegateHasException;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.UC.ChangePassword
{
    public partial class MainChangePassword
    {
        public enum EnumTemplate
        {
            TEMPLATE2
        }

        public UserControl Init(EnumTemplate Template, Data.DataInitChangePass Data)
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

        public bool SetDelegateHasExceptionApiChangePass(UserControl UC, HasExceptionApi HasExcep)
        {
            bool result = false;
            try
            {
                result = SetDelegateHasExceptionFactory.MakeISetDelegateHasException().SetDelegateException(UC, HasExcep);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        public bool SetDelegateChangeSuccess(UserControl UC, ChangePasswordSuccess ChangeSuccess)
        {
            bool result = false;
            try
            {
                result = SetDelegateChangePassSuccessFactory.MakeISetDelegateChangePassSuccess().SetDelegateChangeSucess(UC, ChangeSuccess);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }
    }
}
