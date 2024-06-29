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
using DevExpress.XtraEditors.DXErrorProvider;
using HIS.UC.ExamTreatmentFinish.Run.Validate;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.ExamTreatmentFinish
{
    public partial class frmICDInformation
    {
        private void ValidateForm()
        {
            ValidBenhPhu();
            ValidationICD(10, 500, !this.isAllowNoIcd);
        }

        public void ValidationICD(int? maxLengthCode, int? maxLengthText, bool isRequired)
        {
            try
            {
                if (isRequired)
                {
                    lciICDCode.AppearanceItemCaption.ForeColor = Color.Maroon;

                    IcdValidationRuleControl icdMainRule = new IcdValidationRuleControl();
                    icdMainRule.txtIcdCode = txtIcdCode;
                    icdMainRule.btnBenhChinh = cboIcds;
                    icdMainRule.txtMainText = txtIcdMainText;
                    icdMainRule.chkCheck = chkEditIcd;
                    icdMainRule.maxLengthCode = maxLengthCode;
                    icdMainRule.maxLengthText = maxLengthText;
                    icdMainRule.IsObligatoryTranferMediOrg = this.IsObligatoryTranferMediOrg;
                    icdMainRule.ErrorText = "Trường dữ liệu bắt buộc nhập";
                    icdMainRule.ErrorType = ErrorType.Warning;
                    icdMainRule.icds = currentIcds;
                    dxValidationProvider1.SetValidationRule(txtIcdCode, icdMainRule);
                }
                else
                {
                    lciICDCode.AppearanceItemCaption.ForeColor = new System.Drawing.Color();

                    IcdValidationRuleControl icdMainRule = new IcdValidationRuleControl();
                    icdMainRule.txtIcdCode = txtIcdCode;
                    icdMainRule.btnBenhChinh = cboIcds;
                    icdMainRule.txtMainText = txtIcdMainText;
                    icdMainRule.chkCheck = chkEditIcd;
                    icdMainRule.maxLengthCode = maxLengthCode;
                    icdMainRule.maxLengthText = maxLengthText;
                    icdMainRule.IsObligatoryTranferMediOrg = this.IsObligatoryTranferMediOrg;
                    icdMainRule.IsRequired = false;
                    icdMainRule.ErrorType = ErrorType.Warning;
                    dxValidationProvider1.SetValidationRule(txtIcdCode, icdMainRule);
                }
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
                BenhPhuValidationRule mainRule = new BenhPhuValidationRule();
                mainRule.maBenhPhuTxt = txtIcdSubCode;
                mainRule.tenBenhPhuTxt = txtIcdText;
                mainRule.getIcdMain = this.GetIcdMainCode;
                mainRule.listIcd = currentIcds;
                mainRule.ErrorType = ErrorType.Warning;
                this.dxValidationProvider1.SetValidationRule(txtIcdSubCode, mainRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private string GetIcdMainCode()
        {
            string mainCode = "";
            try
            {
                mainCode = txtIcdCode.Text.Trim();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return mainCode;
        }
    }
}
