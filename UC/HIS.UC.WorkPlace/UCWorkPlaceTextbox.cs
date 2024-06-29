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
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors;
using Inventec.Desktop.Common.Controls.ValidationRule;
using HIS.Desktop.LibraryMessage;
using DevExpress.XtraEditors.DXErrorProvider;
using Inventec.Desktop.Common.LanguageManager;
using System.Resources;

namespace HIS.UC.WorkPlace
{
    public partial class UCWorkPlaceTextbox : DevExpress.XtraEditors.XtraUserControl
    {
        DelegateFocusMoveout focusMoveout;
        int positionHandleControlPatientInfo = -1;
        public UCWorkPlaceTextbox(DelegateFocusMoveout focusMoveout)
        {
            InitializeComponent();
            this.focusMoveout = focusMoveout;
            this.SetCaptionByLanguageKey();
        }

        public UCWorkPlaceTextbox(DelegateFocusMoveout focusMoveout, bool validate)
        {
            InitializeComponent();
            this.focusMoveout = focusMoveout;
            this.SetCaptionByLanguageKey();
            if (validate)
            {
                this.layoutControlItem1.AppearanceItemCaption.ForeColor = Color.Maroon;
                ValidationSingleControl(txtWorkPlace);
            }
        }

        public string GetValue()
        {
            string result = "";
            try
            {
                result = txtWorkPlace.Text.Trim();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return result;
        }

        public void FocusControl()
        {
            try
            {
                txtWorkPlace.Focus();
                txtWorkPlace.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void SetValue(object data)
        {
            try
            {
                txtWorkPlace.Text = (string)data;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtWorkPlace_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                     if (focusMoveout != null)
                    {
                        focusMoveout();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationSingleControl(BaseEdit control)
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = control;
                validRule.ErrorText = MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dxValidationProvider1_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandleControlPatientInfo == -1)
                {
                    positionHandleControlPatientInfo = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandleControlPatientInfo > edit.TabIndex)
                {
                    positionHandleControlPatientInfo = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
			
        }

        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện UCWorkPlaceTextbox
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                His.UC.WorkPlace.Resources.ResourceLanguageManager.LanguageResource__UCWorkPlaceTextbox = new ResourceManager("HIS.UC.WorkPlace.Resources.Lang", typeof(UCWorkPlaceTextbox).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCWorkPlaceTextbox.layoutControl1.Text", His.UC.WorkPlace.Resources.ResourceLanguageManager.LanguageResource__UCWorkPlaceTextbox, LanguageManager.GetCulture());
                this.layoutControlItem1.Text = Inventec.Common.Resource.Get.Value("UCWorkPlaceTextbox.layoutControlItem1.Text", His.UC.WorkPlace.Resources.ResourceLanguageManager.LanguageResource__UCWorkPlaceTextbox, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
