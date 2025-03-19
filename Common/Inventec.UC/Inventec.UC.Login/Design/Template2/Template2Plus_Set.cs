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
using Inventec.Common.Logging;
using Inventec.UC.Login.Base;
using Inventec.UC.Login.UCD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.Login.Design.Template2
{
    internal partial class Template2
    {

        internal bool SetDelegateLoginInfor(LoginInfor Infor)
        {
            bool result = false;
            try
            {
                this._LoginInfor = Infor;
                if (_LoginInfor != null)
                {
                    result = true;
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => Infor), Infor));
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        internal bool SetDelegateButtonConfig(EventButtonConfig btnConfigClick)
        {
            bool result = false;
            try
            {
                this._BtnConfig_Click = btnConfigClick;
                if (_BtnConfig_Click != null)
                {
                    result = true;
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => btnConfigClick), btnConfigClick));
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        //internal void SetEnableButton(bool valid)
        //{
        //    try
        //    {
        //        btnLogin.Enabled = valid;
        //        btnConfig.Enabled = valid;
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        internal void SetLoadOnFocus()
        {
            try
            {
                txtLoginName.Text = "";
                txtPassword.Text = "";
                txtLoginName.Focus();
                txtLoginName.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal bool SetLanguage(string language)
        {
            bool valid = false;
            try
            {
                valid = LanguageWorker.SetLanguage(language);
                LoadLabelLanguage();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return valid;
        }
    }
}
