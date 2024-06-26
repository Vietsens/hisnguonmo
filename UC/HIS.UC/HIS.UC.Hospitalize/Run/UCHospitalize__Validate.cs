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
using DevExpress.XtraEditors.DXErrorProvider;
using HIS.UC.Hospitalize.ValidateRule;
using HIS.Desktop.LocalStorage.LocalData;
using DevExpress.XtraEditors.DXErrorProvider;

namespace HIS.UC.Hospitalize.Run
{
    public partial class UCHospitalize : UserControl
    {
        private void ValidateForm()
        {
            try
            {
                ValidateLookupWithTextEdit(cboDepartment, txtDepartmentCode);
                ValidateLookupWithTextEdit(cboTreatmentType, txtTreatmentTypeCode);
                //ValidateFinishTimeExam(dtFinishTime, this.hospitalizeInitADO.StartTime, this.hospitalizeInitADO.InTime, this.hospitalizeInitADO.OutTime);
                ValidationICD(10, 500, !this.isAllowNoIcd);
                //ValidBenhPhu();
                ValidateComboCareer();
                ValidationControlMaxLength(txtNote, 2000);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ValidationControlMaxLength(BaseEdit control, int? maxLength)
        {
            ControlMaxLengthValidationRule validate = new ControlMaxLengthValidationRule();
            validate.editor = control;
            validate.maxLength = maxLength;
            validate.ErrorText = String.Format("Trường dữ liệu vượt quá {0} ký tự", maxLength);
            validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
            this.dxValidationProvider1.SetValidationRule(control, validate);
        }
        private void ValidateComboCareer()
        {
            try
            {
                GridLookupEditValidationRule validationRule = new GridLookupEditValidationRule();
                validationRule.cbo = cboCareer;
                //validationRule.ErrorText = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validationRule.ErrorText = "Bắt buộc nhập thông tin nghề nghiệp";
                validationRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(cboCareer, validationRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateRelative(TextEdit txt)
        {
            try
            {
                ValidateRule.ValidateEmptyText x=new ValidateEmptyText();
                x.txt = txt;
                dxValidationProvider1.SetValidationRule(txt, x);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateLookupWithTextEdit(GridLookUpEdit cbo, TextEdit textEdit)
        {
            try
            {
                GridLookupEditWithTextEditValidationRule validRule = new GridLookupEditWithTextEditValidationRule();
                validRule.txtTextEdit = textEdit;
                validRule.cbo = cbo;
                validRule.ErrorText = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(textEdit, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateFinishTimeExam(DateEdit control, long? StartTime, long? InTime, long? FinishTimeTreatment)
        {
            try
            {
                FinishTimeValidationRule validRule = new FinishTimeValidationRule();
                validRule.dtFinishTime = control;
                validRule.StartTime = StartTime;
                validRule.InTime = InTime;
                validRule.OutTime = FinishTimeTreatment;
                dxValidationProvider1.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
