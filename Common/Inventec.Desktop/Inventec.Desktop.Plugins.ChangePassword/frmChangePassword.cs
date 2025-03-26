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
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Plugins.ChangePassword;
using Inventec.UC.ChangePassword.Data;
using System;
using System.Resources;
using System.Windows.Forms;
using System.Drawing;
using Inventec.Desktop.Common.LocalStorage.Location;
using System.Configuration;
using Inventec.Common.Logging;

namespace Inventec.Desktop.Plugins.ChangePassword
{
    public delegate void ProcessTokenLostDelegate(CommonParam param);
    public delegate void ChangePasswordSuccessDelegate();
    public partial class frmChangePassword : DevExpress.XtraEditors.XtraForm
    {
        private Inventec.UC.ChangePassword.MainChangePassword MainChangePassword = new Inventec.UC.ChangePassword.MainChangePassword();
        private UserControl UCChangePassword;
        Inventec.UC.ChangePassword.HasExceptionApi hasExceptionApi;
        ChangePasswordSuccessDelegate dlgChangePasswordSuccess;

        public frmChangePassword(Inventec.UC.ChangePassword.HasExceptionApi _hasExceptionApi)
        {
            try
            {
                InitializeComponent();
                hasExceptionApi = _hasExceptionApi;
                ResourceLanguageManager.LanguageFrmChangePassword = new ResourceManager("Inventec.Desktop.Plugins.ChangePassword.Resources.Lang", typeof(frmChangePassword).Assembly);
                DataInitChangePass dataChangePass = new DataInitChangePass(Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager, ChangePasswordConfig.SdaConsumer);
                UCChangePassword = MainChangePassword.Init(Inventec.UC.ChangePassword.MainChangePassword.EnumTemplate.TEMPLATE2, dataChangePass);
                UCChangePassword.Dock = DockStyle.Fill;
                this.Controls.Add(UCChangePassword);

                LoadKeysFromlanguage();

                try
                {
                    this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
                }
                catch (Exception ex)
                {
                    LogSystem.Warn(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public frmChangePassword(Inventec.UC.ChangePassword.HasExceptionApi _hasExceptionApi, ChangePasswordSuccessDelegate dlgChangePasswordSuccess)
        {
            try
            {
                InitializeComponent();
                hasExceptionApi = _hasExceptionApi;
                this.dlgChangePasswordSuccess = dlgChangePasswordSuccess;
                ResourceLanguageManager.LanguageFrmChangePassword = new ResourceManager("Inventec.Desktop.Plugins.ChangePassword.Resources.Lang", typeof(frmChangePassword).Assembly);
                DataInitChangePass dataChangePass = new DataInitChangePass(Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager, ChangePasswordConfig.SdaConsumer);
                UCChangePassword = MainChangePassword.Init(Inventec.UC.ChangePassword.MainChangePassword.EnumTemplate.TEMPLATE2, dataChangePass);
                UCChangePassword.Dock = DockStyle.Fill;
                this.Controls.Add(UCChangePassword);

                LoadKeysFromlanguage();

                try
                {
                    this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
                }
                catch (Exception ex)
                {
                    LogSystem.Warn(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public void LoadKeysFromlanguage()
        {
            try
            {
                this.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_FRM_CHANGE_PASSWORD_FRM_CHANGE_PASSWORD", ResourceLanguageManager.LanguageFrmChangePassword, LanguageManager.GetCulture());

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Khoi tao tat a cac ham delegate cho usercontrol login
        /// </summary>
        private void SetDelegateForUserControl()
        {
            try
            {
                if (!MainChangePassword.SetDelegateChangeSuccess(UCChangePassword, ChangePasswordSuccess))
                {
                    Inventec.Common.Logging.LogSystem.Debug("Set SetDelegateChangeSuccess khong thanh cong. Time=" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss fff"));
                }
                if (!MainChangePassword.SetDelegateHasExceptionApiChangePass(UCChangePassword, hasExceptionApi))
                {
                    Inventec.Common.Logging.LogSystem.Debug("Set SetDelegateHasExceptionApiChangePass khong thanh cong. Time=" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss fff"));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ChangePasswordSuccess()
        {
            try
            {
                if (this.dlgChangePasswordSuccess != null)
                    this.dlgChangePasswordSuccess();
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmChangePassword_Load(object sender, EventArgs e)
        {
            try
            {
                SetDelegateForUserControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
