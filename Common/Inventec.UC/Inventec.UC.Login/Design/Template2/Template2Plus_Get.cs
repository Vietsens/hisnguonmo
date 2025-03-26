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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.Login.Design.Template2
{
    internal partial class Template2
    {
        internal UCD.LoginSuccessUCD GetAccountLogin()
        {
            UCD.LoginSuccessUCD result = null;
            try
            {
                result = new UCD.LoginSuccessUCD();
                result.LOGINNAME = txtLoginName.Text.Trim();
                result.PASSWORD = txtPassword.Text;
                //result.IS_AUTOLOGIN = chkAutoLogin.Checked ? "1" : "0";
                result.LANGUAGE = (cbbLanguage.EditValue != null ? cbbLanguage.EditValue.ToString().ToLower() : Inventec.UC.Login.Base.LanguageWorker.languageVi);
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
