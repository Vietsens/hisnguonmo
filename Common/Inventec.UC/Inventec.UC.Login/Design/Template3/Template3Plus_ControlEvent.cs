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
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.UC.Login.Base;
using THE.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.UC.Login.Design.Template3
{
    internal partial class Template3
    {
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
                //Caption = MessageUtil.GetTextLabel(Label.ManagerLanguage.Enum.IVT_LANGUAGE_KEY_FRMLOGIN_PROGRESS_LOADING);
                lblAccount.Text = MessageUtil.GetTextLabel(Label.ManagerLanguage.Enum.IVT_LANGUAGE_KEY_FRMLOGIN_LBL_ACCOUNT);
                lblPassword.Text = MessageUtil.GetTextLabel(Label.ManagerLanguage.Enum.IVT_LANGUAGE_KEY_FRMLOGIN_LBL_PASSWORD);
                lblLanguage.Text = MessageUtil.GetTextLabel(Label.ManagerLanguage.Enum.IVT_LANGUAGE_KEY_FRMLOGIN_LBL_LANGUAGE);
                lblForgotPassword.Text = MessageUtil.GetTextLabel(Label.ManagerLanguage.Enum.IVT_LANGUAGE_KEY_FRMLOGIN_LBL_FORGOT_PASSWORD);
                lblBranch.Text = MessageUtil.GetTextLabel(Label.ManagerLanguage.Enum.IVT_LANGUAGE_KEY_FRMLOGIN_LBL_BRANCH);
                btnLogin.Text = MessageUtil.GetTextLabel(Label.ManagerLanguage.Enum.IVT_LANGUAGE_KEY_FRMLOGIN_BTN_LOGIN);
                btnExit.Text = MessageUtil.GetTextLabel(Label.ManagerLanguage.Enum.IVT_LANGUAGE_KEY_FRMLOGIN_BTN_EXIT);
                btnConfig.Text = MessageUtil.GetTextLabel(Label.ManagerLanguage.Enum.IVT_LANGUAGE_KEY_FRMLOGIN_BTN_CONFIG);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitBranchCombo(object branchs, long? firstBranchId)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("BRANCH_CODE", "", 50, 1));
                columnInfos.Add(new ColumnInfo("BRANCH_NAME", "", 200, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("BRANCH_NAME", "ID", columnInfos, false, 250);
                if (branchs == null)
                {
                    Inventec.Common.Logging.LogSystem.Debug("Khong tim thay danh sach chi nhanh");
                }
                
                ControlEditorLoader.Load(cboBranch, branchs, controlEditorADO);
                InitDataForm(firstBranchId, branchs);

                if (!firstBranchId.HasValue || firstBranchId == 0)
                    throw new Exception("Khong tim thay chi nhanh dau tien");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboBranch_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                //txtLoginName.Focus();
                //txtLoginName.SelectAll();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void cbbLanguage_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cbbLanguage.EditValue != null)
                {
                    LanguageWorker.SetLanguage((cbbLanguage.EditValue != null ? cbbLanguage.EditValue.ToString().ToLower() : LanguageWorker.languageVi));

                    try
                    {
                        LoadLabelLanguage();
                        if (this.processFormOwner != null)
                            this.processFormOwner(LanguageWorker.GetLanguage());
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
    }
}
