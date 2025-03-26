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
using HIS.UC.UCOtherServiceReqInfo.Valid;
using DevExpress.XtraEditors.DXErrorProvider;
using HIS.UC.UCOtherServiceReqInfo.Resources;
using DevExpress.XtraEditors;
using HIS.Desktop.Utility;

namespace HIS.UC.UCOtherServiceReqInfo
{
    public partial class UCOtherServiceReqInfo : UserControlBase
    {

        public bool ValidateRequiredField()
        {
            bool valid = true;
            try
            {
                valid = this.dxValidationUCOtherReqInfo.Validate();
                if (HIS.UC.UCOtherServiceReqInfo.Config.HisConfig.IsRequiredPriorityTypeInCaseOfCheckingPriority)
                {
                    if (chkPriority.Checked && cboPriorityType.EditValue == null)
                    {
                        valid = false;
                        XtraMessageBox.Show("Bạn cần chọn trường hợp ưu tiên");
                    }
                }
                if (workingPatientType != null && !String.IsNullOrEmpty(workingPatientType.OTHER_PAY_SOURCE_IDS) && (cboOtherPaySource.EditValue == null || (cboOtherPaySource.EditValue ?? "").ToString() == "0"))
                {
                    valid = false;
                    XtraMessageBox.Show("Bạn cần chọn nguồn chi trả khác");
                    cboOtherPaySource.Focus();
                    //cboOtherPaySource.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                valid = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        public void ResetRequiredField()
        {
            try
            {
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(this.dxValidationUCOtherReqInfo, this.dxErrorProviderControl);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateIntructionTime()
        {
            IntructionTime__ValidationRule icdMainRule = new IntructionTime__ValidationRule();
            icdMainRule.txtIntructionTime = this.txtIntructionTime;
            icdMainRule.ErrorText = HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
            icdMainRule.ErrorType = ErrorType.Warning;
            dxValidationUCOtherReqInfo.SetValidationRule(this.txtIntructionTime, icdMainRule);
        }

        private void ValidateTextHosReason()
        {
            TextValidationRule rule = new TextValidationRule();
            rule.txtText = this.txtHosReason;
            rule.ErrorText = HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
            rule.ErrorType = ErrorType.Warning;
            dxValidationUCOtherReqInfo.SetValidationRule(this.txtHosReason, rule);
        }

        private void ValidateFrmFun()
        {
            try
            {
                FrmFunValidationRule _Rule = new FrmFunValidationRule();
                _Rule.cboCCT = this.cboCTT;
                _Rule.frm = this;
                _Rule.ErrorText = ResourceMessage.ChuaNhapThongTinDoiTuongCungChiTra;
                _Rule.ErrorType = ErrorType.Warning;
                dxValidationUCOtherReqInfo.SetValidationRule(this.cboCTT, _Rule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidateFrmInCode()
        {
            try
            {
                TextEditMaxLengthValidationRule _Rule = new TextEditMaxLengthValidationRule();
                _Rule.txtEdit = this.txtIncode;
                _Rule.maxlength = 10;
                _Rule.isVali = true;
                _Rule.ErrorText = HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                _Rule.ErrorType = ErrorType.Warning;
                dxValidationUCOtherReqInfo.SetValidationRule(this.txtIncode, _Rule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidatePriorityType()
        {
            try
            {
                ValidatecboPriorityType();
                ValidatechkPriority();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidatecboPriorityType()
        {
            try
            {
                PriorityType__ValidationRule _Rule = new PriorityType__ValidationRule();
                _Rule.cboPriorityType = this.cboPriorityType;
                _Rule.hasDataAutoCheckPriority = this.hasDataAutoCheckPriority;
                _Rule.ErrorText = HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                _Rule.ErrorType = ErrorType.Warning;
                dxValidationUCOtherReqInfo.SetValidationRule(this.cboPriorityType, _Rule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidatechkPriority()
        {
            try
            {
                Priority__ValidationRule _Rule = new Priority__ValidationRule();
                _Rule.chkPriority = this.chkPriority;
                _Rule.hasDataAutoCheckPriority = this.hasDataAutoCheckPriority;
                _Rule.ErrorText = HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                _Rule.ErrorType = ErrorType.Warning;
                dxValidationUCOtherReqInfo.SetValidationRule(this.chkPriority, _Rule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidateTreatmentType()
        {
            try
            {
                TreatmentType__ValidationRule validRule = new TreatmentType__ValidationRule();
                validRule.cboTreatmentType = this.cboTreatmentType;
                validRule.ErrorText = HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationUCOtherReqInfo.SetValidationRule(this.cboTreatmentType, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateNumOrderPriority()
        {
            try
            {
                ServiceReqNumOrder__ValidationRule validRule = new ServiceReqNumOrder__ValidationRule();
                validRule.spinNumOrderPriority = this.txtSTTPriority;
                validRule.ErrorText = HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationUCOtherReqInfo.SetValidationRule(this.txtSTTPriority, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
