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
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.Plugins.EnterKskInfomantion.Validtion;
using Inventec.Desktop.Common.Controls.ValidationRule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.EnterKskInfomantion
{
    partial class frmEnterKskInfomantion
    {
        #region Validate
        private void ValidateForm()
        {
            try
            {
                //Validate Tab2
                ValidMaxlengthTextBox(txtConclusionTab2, 4000, false);
                ValidMaxlengthTextBox(txtConclusionClinicalTab2, 4000, false);
                ValidMaxlengthTextBox(txtConclusionConsultationTab2, 4000, false);
                ValidMaxlengthTextBox(txtConclusionSubclinicalTab2, 4000, false);
                ValidMaxlengthTextBox(txtExamConclusionTab2, 4000, false);
                ValidMaxlengthTextBox(txtOccupationalDiseaseTab2, 4000, false);
                ValidMaxlengthTextBox(txtProvisionalDiagnosisTab2, 1000, false);
                //Validate Tab1
                ValidMaxlengthTextBox(txtExamCirculationTab1, 4000, false);
                ValidMaxlengthTextBox(txtExamDermatologyTab1, 4000, false);
                ValidMaxlengthTextBox(txtExamDigestionTab1, 4000, false);
                ValidMaxlengthTextBox(txtExamENTTab1, 4000, false);
                ValidMaxlengthTextBox(txtEyeTab1, 4000, false);
                ValidMaxlengthTextBox(txtExamKidneyUrologyTab1, 4000, false);
                ValidMaxlengthTextBox(txtMentalTab1, 4000, false);
                ValidMaxlengthTextBox(txtExamMuscleBoneTab1, 4000, false);
                ValidMaxlengthTextBox(txtExamNeurologicalTab1, 4000, false);
                ValidMaxlengthTextBox(txtExamOENDTab1, 4000, false);
                ValidMaxlengthTextBox(txtExamRepiratoryTab1, 4000, false);
                ValidMaxlengthTextBox(txtExamStomatologyTab1, 4000, false);
                ValidMaxlengthTextBox(txtSurgeryTab1, 4000, false);
                ValidMaxlengthTextBox(memCDHA, 4000, false);
                ValidMaxlengthTextBox(memNoteBlood, 4000, false);
                ValidMaxlengthTextBox(memNoteTestUre, 4000, false);
                ValidMaxlengthTextBox(memNoteTestOth, 4000, false);
                ValidMaxlengthTextBox(txtDiseasesTab1, 4000, false);
                ValidMaxlengthTextBox(txtTreatmentInstructionTab1, 4000, false);
                //Validate Tab3
                ValidMaxlengthTextBox(txtExamCirculationTab3, 4000, false);
                ValidMaxlengthTextBox(txtExamDermatologyTab3, 4000, false);
                ValidMaxlengthTextBox(txtExamDigestionTab3, 4000, false);
                ValidMaxlengthTextBox(txtExamENTTab3, 4000, false);
                ValidMaxlengthTextBox(txtEyeTab3, 4000, false);
                ValidMaxlengthTextBox(txtExamKidneyUrologyTab3, 4000, false);
                ValidMaxlengthTextBox(txtMentalTab3, 4000, false);
                ValidMaxlengthTextBox(txtExamMuscleBoneTab3, 4000, false);
                ValidMaxlengthTextBox(txtExamNeurologicalTab3, 4000, false);
                ValidMaxlengthTextBox(txtExamOENDTab3, 4000, false);
                ValidMaxlengthTextBox(txtExamRepiratoryTab3, 4000, false);
                ValidMaxlengthTextBox(txtExamStomatologyTab3, 4000, false);
                ValidMaxlengthTextBox(txtSurgeryTab3, 4000, false);
                ValidMaxlengthTextBox(txtExamNailTab3, 4000, false);
                ValidMaxlengthTextBox(txtExamMucosaTab3, 4000, false);
                ValidMaxlengthTextBox(txtExamHematopoieticTab3, 4000, false);
                ValidMaxlengthTextBox(txtExamMotionTab3, 4000, false);
                ValidMaxlengthTextBox(txtExamCardiovascularTab3, 4000, false);
                ValidMaxlengthTextBox(txtExamLymphNodesTab3, 4000, false);
                ValidMaxlengthTextBox(txtExamCapillaryTab3, 4000, false);
                ValidMaxlengthTextBox(txtDiseasesTab3, 4000, false);
                ValidMaxlengthTextBox(txtTreatmentInstructionTab3, 4000, false);
                ValidMaxlengthTextBox(txtConclusionTab3, 4000, false);
                // valid 14 ->>
                ValidMaxlengthTextBox(txtExamObstetic, 4000, false);
                ValidMaxlengthTextBox(txtExamOccupationalTherapy, 4000, false);
                ValidMaxlengthTextBox(txtExamTraditional, 4000, false);
                ValidMaxlengthTextBox(txtExamNutrion, 4000, false);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void ValidMaxlengthTextBox(BaseEdit txtEdit, int? maxLength, bool isRequired)
        {
            try
            {
                ValidateMaxLength validateMaxLength = new ValidateMaxLength();
                validateMaxLength.textEdit = txtEdit;
                validateMaxLength.isRequired = isRequired;
                validateMaxLength.maxLength = maxLength;
                dxValidationProviderEditorInfo.SetValidationRule(txtEdit, validateMaxLength);
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
                validRule.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditorInfo.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

    }
}
