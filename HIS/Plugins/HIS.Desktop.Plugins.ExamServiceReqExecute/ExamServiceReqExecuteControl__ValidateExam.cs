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
using HIS.Desktop.Plugins.ExamServiceReqExecute.Validate.ValidateRule;
using HIS.Desktop.Utility;
using Inventec.Desktop.Common.Controls.ValidationRule;
using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace HIS.Desktop.Plugins.ExamServiceReqExecute
{
    public partial class ExamServiceReqExecuteControl : UserControlBase
    {
        // Validate Dich vu - ICd

        private void ValidControl()
        {
            try
            {
                ValidBenhPhu();
                ValiNhapQuaKyTu();
                ValidationICD(10, 500, !this.isAllowNoIcd);
                ValidationSingleControlWithMaxLength(txtIcdCodeCause, false, 10);
                ValidationSingleControlWithMaxLength(txtIcdMainTextCause, false, 500);
                //ValidationICDCause(10, 500, isRequired);
                UCNextTreatmentInstructionValid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidBenhPhu()
        {
            try
            {
                //BenhPhuValidationRule mainRule = new BenhPhuValidationRule();
                //mainRule.maBenhPhuTxt = txtIcdSubCode;
                //mainRule.tenBenhPhuTxt = txtIcdText;
                //mainRule.getIcdMain = this.GetIcdMainCode;
                //mainRule.ErrorType = ErrorType.Warning;
                //this.dxValidationProviderForLeftPanel.SetValidationRule(txtIcdSubCode, mainRule);

                BenhPhuValidationRule mainRule = new BenhPhuValidationRule();
                mainRule.maBenhPhuTxt = txtIcdSubCode;
                mainRule.tenBenhPhuTxt = txtIcdText;
                mainRule.getIcdMain = this.GetIcdMainCode;
                mainRule.listIcd = currentIcds;
                mainRule.ErrorType = ErrorType.Warning;
                this.dxValidationProviderForLeftPanel.SetValidationRule(txtIcdSubCode, mainRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidPathological()
        {
            try
            {
                lblCaptionPathologicalProcess.AppearanceItemCaption.ForeColor = Color.Maroon;

                ControlEditValidationRule icdExam = new ControlEditValidationRule();
                icdExam.editor = txtPathologicalProcess;
                icdExam.ErrorText = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.HeThongTBTruongDuLieuBatBuocPhaiNhap);
                icdExam.ErrorType = ErrorType.Warning;

                this.dxValidationProviderForLeftPanel.SetValidationRule(txtPathologicalProcess, icdExam);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValiNhapQuaKyTu()
        {
            try
            {
                ValidationControlMaxLength(txtKhamBoPhan, 4000);
                ValidationControlMaxLength(txtTuanHoan, 4000);
                ValidationControlMaxLength(txtHoHap, 4000);
                ValidationControlMaxLength(txtTieuHoa, 4000);
                ValidationControlMaxLength(txtThanTietNieu, 4000);
                ValidationControlMaxLength(txtThanKinh, 4000);
                ValidationControlMaxLength(txtCoXuongKhop, 4000);
                ValidationControlMaxLength(txtPartExamEyeSightGlassLeft, 500);
                ValidationControlMaxLength(txtNhanApPhai, 500);
                ValidationControlMaxLength(txtNhanApTrai, 500);
                ValidationControlMaxLength(txtPartExamEyeSightGlassRight, 500);
                ValidationControlMaxLength(txtThiLucKhongKinhPhai, 500);
                ValidationControlMaxLength(txtThiLucKhongKinhTrai, 500);
                ValidationControlMaxLength(txtTai, 4000);
                ValidationControlMaxLength(txtMui, 4000);
                ValidationControlMaxLength(txtHong, 4000);
                ValidationControlMaxLength(txtRHM, 4000);
                ValidationControlMaxLength(txtMat, 4000);
                ValidationControlMaxLength(txtNoiTiet, 4000);
                ValidationControlMaxLength(txtPartExamMental, 4000);
                ValidationControlMaxLength(txtPartExamNutrition, 4000);
                ValidationControlMaxLength(txtPartExamMotion, 4000);
                ValidationControlMaxLength(txtPartExanObstetric, 4000);
                ValidationControlMaxLength(txtSubclinical, 4000);

                ValidationControlMaxLength(txtDaLieu, 4000);

                //ValidationControlMaxLength(txtNextTreatmentInstruction, 100);
                ValidationControlMaxLength(txtResultNote, 500);
                ValidationControlMaxLength(txtKhamToanThan, 4000);
                ValidationControlMaxLength(txtPathologicalHistory, 3000);
                ValidationControlMaxLength(txtPathologicalHistoryFamily, 3000);
                ValidationControlMaxLength(txtHospitalizationReason, 200);
                ValidationControlMaxLength(txtTreatmentInstruction, 500);
                ValidationControlMaxLength(txtProvisionalDianosis, 1000);
                if (this.requiredControl == 1)
                {
                    lblCaptionPathologicalProcess.AppearanceItemCaption.ForeColor = Color.Maroon;
                    ValidationControlMaxLength(txtPathologicalProcess, 4000, true);
                }
                else
                {
                    ValidationControlMaxLength(txtPathologicalProcess, 4000);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationControlMaxLength(BaseEdit control, int? maxLength, [Optional] bool IsRequest)
        {
            ControlMaxLengthValidationRule validate = new ControlMaxLengthValidationRule();
            validate.editor = control;
            validate.maxLength = maxLength;
            validate.IsRequired = IsRequest;
            validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
            this.dxValidationProviderForLeftPanel.SetValidationRule(control, validate);
        }

        private void ValidationSingleControl(BaseEdit control)
        {
            ControlEditValidationRule validate = new ControlEditValidationRule();
            validate.editor = control;
            validate.ErrorText = "Trường dữ liệu bắt buộc nhập";
            validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
            this.dxValidationProviderForLeftPanel.SetValidationRule(control, validate);
        }
    }
}
