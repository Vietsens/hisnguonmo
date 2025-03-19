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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Inventec.UC.Login.UCD;
using Inventec.UC.Login.Base;
using Inventec.Common.Logging;
using Microsoft.Win32;
using Inventec.UC.Login.Sda;
using DevExpress.XtraEditors.Controls;
using THE.EFMODEL.DataModels;
using System.Collections;

namespace Inventec.UC.Login.Design.Template1
{
    internal partial class Template1 : UserControl
    {
        private LoginInfor _LoginInfor;
        private EventButtonConfig _BtnConfig_Click;
        Inventec.UC.Login.UCD.InitUCD.ProcessFormOwner processFormOwner;
        List<SDA.EFMODEL.DataModels.SDA_LANGUAGE> languages;
        public Template1(InitUCD data)
        {
            try
            {
                InitializeComponent();

                InitLabelControl(data);
                ApiConsumerStore.SdaConsumer = data.sdaCosumer;
                Inventec.UC.Login.Base.RegistryConstant.SYSTEM_FOLDER = data.SYSTEM_FOLDER;
                Inventec.UC.Login.Base.RegistryConstant.APP_FOLDER = data.APP_FOLDER;
                AppConfig.APPLICATION_CODE = data.APPLICATION_CODE;
                if (data.processFormOwner != null)
                {
                    this.processFormOwner = data.processFormOwner;
                }
                InitBranchCombo(data.Branchs, data.FirstBranchId);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void Template1_Load(object sender, EventArgs e)
        {
            try
            {
                LoadDataComboLanguage();
                txtLoginName.Focus();
                txtLoginName.SelectAll();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void InitLabelControl(InitUCD data)
        {
            try
            {
                if (data != null && data.LabelString != null)
                {
                    if (!String.IsNullOrEmpty(data.LabelString.LblBranch))
                    {
                        lblBranch.Text = data.LabelString.LblBranch;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataComboLanguage()
        {
            try
            {
                this.languages = new SdaLanguageGet().Get();
                cbbLanguage.Properties.DataSource = this.languages;
                cbbLanguage.Properties.DisplayMember = "LANGUAGE_NAME";
                cbbLanguage.Properties.ValueMember = "LANGUAGE_CODE";
                cbbLanguage.Properties.ForceInitialize();
                cbbLanguage.Properties.Columns.Clear();
                cbbLanguage.Properties.Columns.Add(new LookUpColumnInfo("LANGUAGE_CODE", "", 80));
                cbbLanguage.Properties.Columns.Add(new LookUpColumnInfo("LANGUAGE_NAME", "", 200));
                cbbLanguage.Properties.ShowHeader = false;
                cbbLanguage.Properties.ImmediatePopup = true;
                cbbLanguage.Properties.DropDownRows = 10;
                cbbLanguage.Properties.PopupWidth = 300;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void InitDataForm(long? firstBranchId, object branchs)
        {
            try
            {
                RegistryKey appFolder = Registry.CurrentUser.CreateSubKey(RegistryConstant.SOFTWARE_FOLDER).CreateSubKey(RegistryConstant.COMPANY_FOLDER).CreateSubKey(RegistryConstant.APP_FOLDER);
                try
                {
                    long branchIdInRegis = Convert.ToInt64((appFolder.GetValue(RegistryConstant.BRANCH_KEY) ?? 0).ToString());

                    if (branchIdInRegis > 0)
                    {
                        cboBranch.EditValue = branchIdInRegis;
                    }
                    else
                    {
                        cboBranch.EditValue = firstBranchId;
                    }
                }
                catch (Exception ex)
                {
                    LogSystem.Debug("Branch key save in registry invalid", ex);
                }

                try
                {
                    string language = (appFolder.GetValue(RegistryConstant.LANGUAGE_KEY) ?? "").ToString();
                    if (!String.IsNullOrEmpty(language))
                    {
                        cbbLanguage.EditValue = language.ToUpper();
                    }
                }
                catch (Exception ex)
                {
                    LogSystem.Debug("Language key save in registry invalid", ex);
                }

                if (cbbLanguage.EditValue == null)
                {
                    var languageDefaultBase = (this.languages.FirstOrDefault(o => o.IS_BASE == 1));
                    var languageDefault = (languageDefaultBase != null ? languageDefaultBase : this.languages[0]);
                    cbbLanguage.EditValue = languageDefault.LANGUAGE_CODE.ToUpper();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

    }
}
