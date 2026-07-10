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
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.Plugins.Library.TreatmentEndTypeExt.Data;
using HIS.Desktop.Utility;
using HIS.UC.WorkPlace;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.LibraryMessage;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.Library.TreatmentEndTypeExt.SickLeave
{
    public partial class frmSickLeave : FormBase
    {
        public void ValidateControl()
        {
            try
            {
                ValidationControlWorkPlace();
                ValidationControlMaxLength(txtRelativeName, 100);
                ValidateGridLookupWithTextEdit(cboUser, txtLoginName, dxValidationProvider1);
                ValidationControlBHXH(txtBhxhCode);
                
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// PTTK_49141 — Config-gated requirement for "Số CCCD/HC" (txtCCCDNumber) and "Ngày cấp" (cboDateCCCD)
        /// on the BHXH sick-leave form.
        /// When HIS_CONFIG "HIS.Desktop.Plugins.Library.TreatmentEndTypeExt.SickLeave.RequireCccdNumber" = 1:
        ///   - both captions turn Maroon, consistent with the other required fields on this form
        ///     (Nơi làm việc, Mã BHXH, Phương pháp điều trị);
        ///   - both controls become required — a warning is shown on the control and Save is blocked when empty.
        /// The rules are registered on dxValidationProvider1, so they run within the existing required group and,
        /// because their TabIndex (26/27) is the highest, they are handled last (Nơi làm việc → Mã BHXH →
        /// Phương pháp điều trị → Số CCCD → Ngày cấp).
        /// When config = 0 or missing, current behavior is unchanged.
        /// </summary>
        private void ConfigRequireCccdNumber()
        {
            try
            {
                isRequireCccdNumber = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<int>(RequireCccdNumberConfigKey) == 1;
                if (!isRequireCccdNumber)
                    return;

                // Highlight the two captions like the other required fields on this form.
                layoutControlItem16.AppearanceItemCaption.ForeColor = Color.Maroon;
                layoutControlItem16.AppearanceItemCaption.Options.UseForeColor = true;
                layoutControlItem17.AppearanceItemCaption.ForeColor = Color.Maroon;
                layoutControlItem17.AppearanceItemCaption.Options.UseForeColor = true;

                // Warning icon on the control + block Save when empty (checked by dxValidationProvider1.Validate()).
                ValidationRequired(txtCCCDNumber);
                ValidationRequired(cboDateCCCD);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationRequired(BaseEdit control)
        {
            try
            {
                Inventec.Desktop.Common.Controls.ValidationRule.ControlEditValidationRule validate = new ControlEditValidationRule();
                validate.editor = control;
                validate.ErrorText = "Trường dữ liệu băt buộc";
                validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                this.dxValidationProvider1.SetValidationRule(control, validate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void ValidationControlBHXH(TextEdit txt)
        {
            try
            {
                Validation.ValidBhxhCode rule = new Validation.ValidBhxhCode();
                rule.bhxhCode = txt;
                this.dxValidationProvider1.SetValidationRule(txtBhxhCode, rule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidationControlWorkPlace()
        {
            try
            {
                Validation.ValidateWorkPlace rule = new Validation.ValidateWorkPlace();
                rule.cboWorkPlace = cboWorkPlace;
                rule.txtWorkPlace = txtPatientWorkPlace;
                this.dxValidationProvider1.SetValidationRule(cboWorkPlace, rule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void ValidationControlAge()
        {
            try
            {
                Validation.ValidateAgeTxt rule = new Validation.ValidateAgeTxt();
                rule.text = txtRelativeName;
                rule.histreatment = treatment;
                rule.ErrorType = ErrorType.Warning;
                if (string.IsNullOrEmpty(txtRelativeName.Text))
                {
                    this.dxValidationProvider1.SetValidationRule(txtRelativeName, rule);
                }


                Validation.ValidateAgeCbo rule_ = new Validation.ValidateAgeCbo();
                rule_.cbo = cboRelativeType;
                rule_.histreatment = treatment;
                rule_.ErrorType = ErrorType.Warning;
                if (cboRelativeType.EditValue == null || string.IsNullOrEmpty(cboRelativeType.Text.ToString())) 
				{
                    this.dxValidationProvider1.SetValidationRule(cboRelativeType, rule_);
				}
               
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void ValidationControlMaxLength(BaseEdit control, int? maxLength, [Optional] bool IsRequest)
        {
            try
            {
                ControlMaxLengthValidationRule validate = new ControlMaxLengthValidationRule();
                validate.editor = control;
                validate.maxLength = maxLength;
                validate.IsRequired = IsRequest;
                validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                this.dxValidationProvider1.SetValidationRule(control, validate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
