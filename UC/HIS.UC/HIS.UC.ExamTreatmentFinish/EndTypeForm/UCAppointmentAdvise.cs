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
using DevExpress.XtraEditors;
using Inventec.Desktop.Common.Controls.ValidationRule;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;

namespace HIS.UC.ExamTreatmentFinish.EndTypeForm
{
    public partial class UCAppointmentAdvise : UserControl
    {

        private int positionHandle = -1;
        public UCAppointmentAdvise()
        {
            InitializeComponent();
        }

        public void SetValueAdvise(string content)
        {
            try
            {
                txtAdvise.Text = content;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public string GetValueAdvise()
        {
            string content = "";
            try
            {
                content = txtAdvise.Text.Trim();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return content;
        }
        public bool GetValidate()
        {
            this.positionHandle = -1;
            if (!dxValidationProvider1.Validate())
            {
                return false;
            }
            return true;
        }
        private void setValidate()
        {
            try
            {
                ValidationControlMaxLength(this.txtAdvise, 500);
            }
            catch (Exception ex)
            {
                  Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidationControlMaxLength(BaseEdit control, int? maxLength)
        {
            ControlMaxLengthValidationRule validate = new ControlMaxLengthValidationRule();
            validate.editor = control;
            validate.maxLength = maxLength;
            validate.ErrorText = String.Format(Resources.ResourceMessage.TruongDuLieuVuotQuaKyTu, maxLength);
            validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
            this.dxValidationProvider1.SetValidationRule(control, validate);
        }

        private void dxValidationProvider1_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                DevExpress.XtraEditors.BaseEdit edit = e.InvalidControl as DevExpress.XtraEditors.BaseEdit;
                if (edit == null)
                    return;

                DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo viewInfo = edit.GetViewInfo() as DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandle == -1)
                {
                    positionHandle = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandle > edit.TabIndex)
                {
                    positionHandle = edit.TabIndex;
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

        private void UCAppointmentAdvise_Load(object sender, EventArgs e)
        {
            try
            {
                setValidate();
                SetCaptionByLanguageKey();
            }
            catch (Exception ex)
            {
               Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện UCAppointmentAdvise
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.UC.ExamTreatmentFinish.Resources.Lang", typeof(UCAppointmentAdvise).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCAppointmentAdvise.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtAdvise.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCAppointmentAdvise.txtAdvise.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem1.Text = Inventec.Common.Resource.Get.Value("UCAppointmentAdvise.layoutControlItem1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


    }
}
