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
using Inventec.UC.Login.Design;
using Inventec.UC.Login.Get.GetAccountLogin;
using Inventec.UC.Login.Init;
using Inventec.UC.Login.Set.SetBranch;
using Inventec.UC.Login.Set.SetDelegateButtonConfig;
using Inventec.UC.Login.Set.SetDelegateLoginInfor;
using Inventec.UC.Login.Set.SetEnableButton;
using Inventec.UC.Login.Set.SetLanguageForUC;
using Inventec.UC.Login.Set.SetLoadFocus;
using Inventec.UC.Login.UCD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.UC.Login
{
    public partial class MainLogin
    {
        public enum EnumTemplate
        {
            TEMPLATE1,
            TEMPLATE2,
            TEMPLATE3
        }

        #region Init UserControl Login
        public UserControl Init(EnumTemplate Template, InitUCD data)
        {
            UserControl result = null;
            try
            {
                result = InitUCFactory.MakeIInitUC().Init(Template, data);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                result = null;
            }
            return result;
        }
        #endregion


        #region Set

        #region Set Delegate
        public bool SetDelegateInforLogin(UserControl UC, LoginInfor loginInfor)
        {
            bool result = false;
            try
            {
                result = SetDelegateLoginInforFactory.MakeISetDelegateLoginInfor().SetDelegateLogin(UC, loginInfor);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                result = false;
            }

            return result;

        }

        public bool SetDelegateButtonConfig(UserControl UC, EventButtonConfig btnConfig)
        {
            bool result = false;
            try
            {
                result = SetDelefateButtonConfigFactory.MakeISetDelefateButtonConfig().SetDelegateConfig(UC, btnConfig);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                result = false;
            }

            return result;
        }
        #endregion

        #region Set Boolean Controls
        public void SetButtonEnable(UserControl UC, bool valid)
        {
            try
            {
                SetEnableButtonFactory.MakeISetEnableButton().SetButtonEnable(UC, valid);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Set Language
        public bool SetLanguage(UserControl UC, string Language)
        {
            bool result = false;
            try
            {
                result = SetLanguageForUCFactory.MakeISetLanguageForUC().SetLanguage(UC, Language);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }
        #endregion

        public void SetSdaConsumer(Inventec.Common.WebApiClient.ApiConsumer Consumer)
        {
            try
            {
                Inventec.UC.Login.Base.ApiConsumerStore.SdaConsumer = Consumer;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void SetLoadFocus(UserControl UC)
        {
            try
            {
                SetLoadOnFocusFactory.MakeISetLoadOnFocus().LoadFocus(UC);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Get UCD
        public bool Login(UserControl UC, ref LoginSuccessUCD dataLoginResult)
        {
            bool success = false;
            try
            {
                if (UC.GetType() == typeof(Design.Template1.Template1))
                {
                    Design.Template1.Template1 UCLogin = (Design.Template1.Template1)UC;
                    UCLogin.btnLogin_Click(null, null);
                    success = true;
                }
                else if (UC.GetType() == typeof(Design.Template2.Template2))
                {
                    Design.Template2.Template2 UCLogin = (Design.Template2.Template2)UC;
                    success = UCLogin.LoginClick(ref  dataLoginResult);
                }
                else if (UC.GetType() == typeof(Design.Template3.Template3))
                {
                    Design.Template3.Template3 UCLogin = (Design.Template3.Template3)UC;
                    UCLogin.btnLogin_Click(null, null);
                    success = true;
                }
            }
            catch (Exception ex)
            {
                success = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return success;
        }

        public UCD.LoginSuccessUCD GetLoginAccount(UserControl UC)
        {
            LoginSuccessUCD result = null;
            try
            {
                result = GetAccountLoginFactory.MakeIGetAccountLogin().GetLoginAccount(UC);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }

        public void SetBranch(UserControl uc, object obj, long? firstBranchId)
        {
            try
            {
                BranchFactory.MakeISetBranch(uc,obj, firstBranchId);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion
    }
}
