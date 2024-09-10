using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.UC.Login.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.UC.Login.Design.Template1
{
    internal partial class Template1
    {
        
        private void cbbLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cbbLanguage.SelectedText != null)
                {
                    if (cbbLanguage.SelectedIndex == 0)
                    {
                        LanguageWorker.SetLanguage(LanguageWorker.languageVi);

                    }
                    else
                    {
                        LanguageWorker.SetLanguage(LanguageWorker.languageEn);
                    }

                    try
                    {
                        LoadLabelLanguage();
                    }
                    catch (Exception ex)
                    {
                        LogSystem.Error("Set label and global message fail", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtLoginName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtPassword.Focus();
                    txtPassword.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPassword_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnLogin_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadLabelLanguage()
        {
            try
            {
                //progressLoading.Caption = MessageUtil.GetTextLabel(Label.ManagerLanguage.Enum.IVT_LANGUAGE_KEY_FRMLOGIN_PROGRESS_LOADING);
                lblAccount.Text = MessageUtil.GetTextLabel(Label.ManagerLanguage.Enum.IVT_LANGUAGE_KEY_FRMLOGIN_LBL_ACCOUNT);
                lblPassword.Text = MessageUtil.GetTextLabel(Label.ManagerLanguage.Enum.IVT_LANGUAGE_KEY_FRMLOGIN_LBL_PASSWORD);
                lblLanguage.Text = MessageUtil.GetTextLabel(Label.ManagerLanguage.Enum.IVT_LANGUAGE_KEY_FRMLOGIN_LBL_LANGUAGE);
                lblForgotPassword.Text = MessageUtil.GetTextLabel(Label.ManagerLanguage.Enum.IVT_LANGUAGE_KEY_FRMLOGIN_LBL_FORGOT_PASSWORD);
                //chkAutoLogin.Text = MessageUtil.GetTextLabel(Label.ManagerLanguage.Enum.IVT_LANGUAGE_KEY_FRMLOGIN_CHK_AUTO_LOGIN);
                btnLogin.Text = MessageUtil.GetTextLabel(Label.ManagerLanguage.Enum.IVT_LANGUAGE_KEY_FRMLOGIN_BTN_LOGIN);
                btnExit.Text = MessageUtil.GetTextLabel(Label.ManagerLanguage.Enum.IVT_LANGUAGE_KEY_FRMLOGIN_BTN_EXIT);
                btnConfig.Text = MessageUtil.GetTextLabel(Label.ManagerLanguage.Enum.IVT_LANGUAGE_KEY_FRMLOGIN_BTN_CONFIG);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
