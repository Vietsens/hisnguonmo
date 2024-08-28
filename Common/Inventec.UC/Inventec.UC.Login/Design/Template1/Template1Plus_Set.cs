using Inventec.Common.Logging;
using Inventec.UC.Login.Base;
using Inventec.UC.Login.UCD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.Login.Design.Template1
{
    internal partial class Template1
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

        internal void SetEnableButton(bool valid)
        {
            try
            {
                btnLogin.Enabled = valid;
                btnConfig.Enabled = valid;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

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
