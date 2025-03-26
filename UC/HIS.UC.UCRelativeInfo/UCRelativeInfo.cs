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
using HIS.UC.UCRelativeInfo.ADO;
using HIS.UC.UCRelativeInfo;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using HIS.Desktop.DelegateRegister;
using HIS.UC.WorkPlace;
using HIS.Desktop.Utility;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;

namespace HIS.UC.UCRelativeInfo
{
    public partial class UCRelativeInfo : UserControlBase
    {
        DelegateFocusNextUserControl dlgFocusNextUserControl;
        int positionHandleControl = -1;
        bool IsObligatory = false;
        bool IsChild = false;
        Inventec.Desktop.Common.Modules.Module CurrentModule;

        #region Constructor

        public UCRelativeInfo()
            : base("HIS.Desktop.Plugins.RegisterV2", "UCRelativeInfo")
        {
            Inventec.Common.Logging.LogSystem.Debug("UCRelativeInfo .1");
            InitializeComponent();
            Inventec.Common.Logging.LogSystem.Debug("UCRelativeInfo .2");
        }

        private void UCRelativeInfo_Load(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("UCRelativeInfo_Load .1");
                //InitFieldFromAsync();
                Inventec.Common.Logging.LogSystem.Debug("UCRelativeInfo_Load .2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public async Task InitFieldFromAsync()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("UCRelativeInfo_InitFieldFromAsync .1");
                //SetCaptionByLanguageKey();
                this.SetValidateControl(false);
                SetCaptionByLanguageKeyNew();
                Inventec.Common.Logging.LogSystem.Debug("UCRelativeInfo_InitFieldFromAsync .2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện UCRelativeInfo
        /// </summary>
        private void SetCaptionByLanguageKeyNew()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.UC.UCRelativeInfo.Resources.Lang", typeof(UCRelativeInfo).Assembly);
                Inventec.Common.Logging.LogSystem.Debug("SetCaptionByLanguageKeyNew .1");
                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCRelativeInfo.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                Inventec.Common.Logging.LogSystem.Debug("SetCaptionByLanguageKeyNew .2");
                this.chkCapGiayNghiOm.Properties.Caption = Inventec.Common.Resource.Get.Value("UCRelativeInfo.chkCapGiayNghiOm.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                Inventec.Common.Logging.LogSystem.Debug("SetCaptionByLanguageKeyNew .3");
                //toolTipItem2.Text = Inventec.Common.Resource.Get.Value("toolTipItem2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                Inventec.Common.Logging.LogSystem.Debug("SetCaptionByLanguageKeyNew .4");
                this.lcgHomePerson.Text = Inventec.Common.Resource.Get.Value("UCRelativeInfo.lcgHomePerson.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                Inventec.Common.Logging.LogSystem.Debug("SetCaptionByLanguageKeyNew .5");
                this.lciHomPerson.Text = Inventec.Common.Resource.Get.Value("UCRelativeInfo.lciHomPerson.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                Inventec.Common.Logging.LogSystem.Debug("SetCaptionByLanguageKeyNew .6");
                this.lciRelative.Text = Inventec.Common.Resource.Get.Value("UCRelativeInfo.lciRelative.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                Inventec.Common.Logging.LogSystem.Debug("SetCaptionByLanguageKeyNew .7");
                this.lciAddress.Text = Inventec.Common.Resource.Get.Value("UCRelativeInfo.lciAddress.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                Inventec.Common.Logging.LogSystem.Debug("SetCaptionByLanguageKeyNew .8");
                this.lciCMND.Text = Inventec.Common.Resource.Get.Value("UCRelativeInfo.lciCMND.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                Inventec.Common.Logging.LogSystem.Debug("SetCaptionByLanguageKeyNew .9");
                this.lciFortxtRelativePhone.Text = Inventec.Common.Resource.Get.Value("UCRelativeInfo.lciFortxtRelativePhone.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                Inventec.Common.Logging.LogSystem.Debug("SetCaptionByLanguageKeyNew .10");
                this.lciForchkCapGiayNghiOm.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCRelativeInfo.lciForchkCapGiayNghiOm.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                Inventec.Common.Logging.LogSystem.Debug("SetCaptionByLanguageKeyNew .11");
                this.lciForchkCapGiayNghiOm.Text = Inventec.Common.Resource.Get.Value("UCRelativeInfo.lciForchkCapGiayNghiOm.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                Inventec.Common.Logging.LogSystem.Debug("SetCaptionByLanguageKeyNew .12");
                this.lciFather.Text = Inventec.Common.Resource.Get.Value("UCRelativeInfo.lciFather.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                Inventec.Common.Logging.LogSystem.Debug("SetCaptionByLanguageKeyNew .13");
                this.lciMother.Text = Inventec.Common.Resource.Get.Value("UCRelativeInfo.lciMother.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageUCRelativeInfo = new ResourceManager("HIS.UC.UCRelativeInfo.Resources.Lang", typeof(HIS.UC.UCRelativeInfo.UCRelativeInfo).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCRelativeInfo.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageUCRelativeInfo, LanguageManager.GetCulture());
                this.lcgHomePerson.Text = Inventec.Common.Resource.Get.Value("UCRelativeInfo.lcgHomePerson.Text", Resources.ResourceLanguageManager.LanguageUCRelativeInfo, LanguageManager.GetCulture());
                this.lciHomPerson.Text = Inventec.Common.Resource.Get.Value("UCRelativeInfo.lciHomPerson.Text", Resources.ResourceLanguageManager.LanguageUCRelativeInfo, LanguageManager.GetCulture());
                this.lciRelative.Text = Inventec.Common.Resource.Get.Value("UCRelativeInfo.lciRelative.Text", Resources.ResourceLanguageManager.LanguageUCRelativeInfo, LanguageManager.GetCulture());
                this.lciAddress.Text = Inventec.Common.Resource.Get.Value("UCRelativeInfo.lciAddress.Text", Resources.ResourceLanguageManager.LanguageUCRelativeInfo, LanguageManager.GetCulture());
                this.lciCMND.Text = Inventec.Common.Resource.Get.Value("UCRelativeInfo.lciCMND.Text", Resources.ResourceLanguageManager.LanguageUCRelativeInfo, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Event Control

        private void txtHomePerson_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    FocusToCorrelated();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtCorrelated_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    FocusToRelativeCMNDNumber();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtRelativeCMNDNumber_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    FocusToRelativePhone();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtRelativePhone_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    FocusToRelativeAddress();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtRelativeAddress_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (lciForchkCapGiayNghiOm.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                    {
                        chkCapGiayNghiOm.Focus();
                    }
                    else if (dlgFocusNextUserControl != null)
                    {
                        dlgFocusNextUserControl();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        private void txtRelativeCMNDNumber_Validated(object sender, EventArgs e)
        {
            try
            {
                if (this.IsObligatory == true)
                    return;
                bool validPatientInfo = dxValidationProviderControl.Validate();
                if (!validPatientInfo)
                {
                    IList<Control> invalidControls = dxValidationProviderControl.GetInvalidControls();
                    for (int i = invalidControls.Count - 1; i >= 0; i--)
                    {
                        Inventec.Common.Logging.LogSystem.Info((i == 0 ? "InvalidControls:" : "") + "" + invalidControls[i].Name + ",");
                    }
                    return;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtRelativeCMNDNumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!Char.IsDigit(e.KeyChar) && !Char.IsControl(e.KeyChar))
                    e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtRelativeCMNDNumber_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void txtRelativeCMNDNumber_TextChanged(object sender, EventArgs e)
        {
            try
            {
                //if (this.txtRelativeCMNDNumber.Text.Length > 0)
                //    this.ValidRelativeCMNDNumber();
                //else if(this.IsObligatory == false)
                //    this.dxValidationProviderControl.SetValidationRule(this.txtRelativeCMNDNumber, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void SetValueAddress(string __Address)
        {
            try
            {
                if (this.lciAddress.AppearanceItemCaption.ForeColor == System.Drawing.Color.Maroon)
                {
                    this.txtRelativeAddress.Text = __Address ?? "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkCapGiayNghiOm_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                //txtCorrelated.Enabled = chkCapGiayNghiOm.Checked;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkCapGiayNghiOm_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (dlgFocusNextUserControl != null)
                    {
                        dlgFocusNextUserControl();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtFather_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    FocusToMother();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtMother_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    FocusToHomePerson();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
