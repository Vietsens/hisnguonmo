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
using DevExpress.XtraEditors.DXErrorProvider;
using Inventec.Desktop.Common.Controls.ValidationRule;
using HIS.Desktop.Plugins.ExamServiceReqExecute.Validate.ValidateRule;
using DevExpress.XtraEditors;
using System.Runtime.InteropServices;

namespace HIS.Desktop.Plugins.ExamServiceReqExecute
{
    public partial class ExamServiceReqExecuteControl : UserControl
    {
        // Validate Dich vu - ICd

        private void ValidControl()
        {
            try
            {
                //ValidBenhPhu();
                ValiNhapQuaKyTu();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //private void ValidBenhPhu()
        //{
        //    try
        //    {
        //        BenhPhuValidationRule mainRule = new BenhPhuValidationRule();
        //        mainRule.maBenhPhuTxt = txtIcdExtraCode;
        //        mainRule.tenBenhPhuTxt = txtIcdExtraName;
        //        mainRule.ErrorType = ErrorType.Warning;
        //        this.dxValidationProviderForLeftPanel.SetValidationRule(txtIcdExtraCode, mainRule);
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

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
                ValidationControlMaxLength(txtKhamBoPhan, 500);
                ValidationControlMaxLength(txtTuanHoan, 500);
                ValidationControlMaxLength(txtHoHap, 500);
                ValidationControlMaxLength(txtTieuHoa, 500);
                ValidationControlMaxLength(txtThanTietNieu, 500);
                ValidationControlMaxLength(txtThanKinh, 500);
                ValidationControlMaxLength(txtCoXuongKhop, 500);
                ValidationControlMaxLength(txtThiLucCoKinhPhai, 500);
                ValidationControlMaxLength(txtNhanApPhai, 500);
                ValidationControlMaxLength(txtNhanApTrai, 500);
                ValidationControlMaxLength(txtThiLucCoKinhTrai, 500);
                ValidationControlMaxLength(txtThiLucKhongKinhPhai, 500);
                ValidationControlMaxLength(txtThiLucKhongKinhTrai, 500);
                ValidationControlMaxLength(txtTai, 500);
                ValidationControlMaxLength(txtMui, 500);
                ValidationControlMaxLength(txtHong, 500);
                ValidationControlMaxLength(txtRHM, 500);
                ValidationControlMaxLength(txtMat, 500);
                ValidationControlMaxLength(txtNoiTiet, 500);
                ValidationControlMaxLength(txtPartExamMental, 500);
                ValidationControlMaxLength(txtPartExamNutrition, 500);
                ValidationControlMaxLength(txtPartExamMotion, 500);
                ValidationControlMaxLength(txtPartExanObstetric, 500);
                ValidationControlMaxLength(txtSubclinical, 500);
                //ValidationControlMaxLength(txtNextTreatmentInstruction, 100);
                ValidationControlMaxLength(txtSubclinical, 500);
                ValidationControlMaxLength(txtResultNote, 500);
                ValidationControlMaxLength(txtKhamToanThan, 500);
                ValidationControlMaxLength(txtPathologicalHistory, 3000);
                ValidationControlMaxLength(txtPathologicalHistoryFamily, 3000);
                ValidationControlMaxLength(txtHospitalizationReason, 200);
                ValidationControlMaxLength(txtTreatmentInstruction, 500);
                if (requiredControl == 1)
                {
                    lblCaptionPathologicalProcess.AppearanceItemCaption.ForeColor = Color.Maroon;
                    ValidationControlMaxLength(txtPathologicalProcess, 3000, true);
                }
                else
                {
                    ValidationControlMaxLength(txtPathologicalProcess, 3000);
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
