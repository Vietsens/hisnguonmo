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
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.Plugins.AssignService.Config;
using HIS.Desktop.Plugins.AssignService.Validation;
using HIS.Desktop.Utility.ValidateRule;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.LibraryMessage;
using System;
using System.Linq;

namespace HIS.Desktop.Plugins.AssignService.AssignService
{
    public partial class frmAssignService : HIS.Desktop.Utility.FormBase
    {

        private bool CheckExistServicePaymentLimit(string ServiceCode)
        {
            bool result = false;
            try
            {
                string servicePaymentLimit = HisConfigCFG.ServiceHasPaymentLimitBHYT.ToLower();
                if (!String.IsNullOrEmpty(servicePaymentLimit))
                {
                    string[] serviceArr = servicePaymentLimit.Split(',');
                    if (serviceArr != null && serviceArr.Length > 0)
                    {
                        if (serviceArr.Contains(ServiceCode.ToLower()))
                        {
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void dxValidationProviderControl_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (this.positionHandleControl == -1)
                {
                    this.positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (this.positionHandleControl > edit.TabIndex)
                {
                    this.positionHandleControl = edit.TabIndex;
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

        private void ValdateSecondaryIcd()
        {
            try
            {
                BenhPhuValidationRule mainRule = new BenhPhuValidationRule();
                mainRule.maBenhPhuTxt = txtIcdSubCode;
                mainRule.tenBenhPhuTxt = txtIcdText;
                mainRule.getIcdMain = this.GetIcdMainCode;
                mainRule.listIcd = currentIcds;
                mainRule.ErrorType = ErrorType.Warning;
                this.dxValidationProviderControl.SetValidationRule(txtIcdSubCode, mainRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateForm()
        {
            try
            {
                //this.ValidationSingleControl(this.dtInstructionTime, this.dxValidationProviderControl);
                //if (HisConfigCFG.ObligateIcd == commonString__true) // 1: bắt buộc nhập chẩn đoán khi chỉ định DV
                //{
                //    this.lciIcdMain.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
                //    this.ValidationSingleControl(this.txtIcdMainCode, this.dxValidationProviderControl, "", this.ValidIcdMain);
                //}
                this.ValidControlCboUser();
                this.ValidControlProvisionalDiagnosis();
                this.ValdateSecondaryIcd();
                if (HisConfigCFG.IsRequiredTracking && this.currentHisPatientTypeAlter != null && (this.currentHisPatientTypeAlter.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU || this.currentHisPatientTypeAlter.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNGOAITRU))
                {
                    ValidControlCboTracking(true);
                    lciTracking.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
                }
                else
                {
                    ValidControlCboTracking(false);
                    lciTracking.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //private bool ValidIcdMain()
        //{
        //    bool valid = true;
        //    try
        //    {
        //        valid = (this.cboIcdServiceReq.EditValue != null);
        //        valid = valid && !String.IsNullOrEmpty(this.txtIcdMainCode.Text);
        //        if (this.chkIcdServiceReq.Checked)
        //        {
        //            valid = valid && !String.IsNullOrEmpty(this.txtIcdServiceReq.Text);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }

        //    return valid;
        //}

        private void ValidControlCboUser()
        {
            try
            {
                RequestUserValidationRule rule = new RequestUserValidationRule();
                rule.cboUser = cboUser;
                rule.txtLoginname = txtLoginName;
                dxValidationProviderControl.SetValidationRule(txtLoginName, rule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlCboTracking(bool isRequied)
        {
            try
            {
                TrackingValidationRule rule = new TrackingValidationRule();
                rule.cboTracking = cboTracking;
                rule.isRequired = isRequied;
                dxValidationProviderControl.SetValidationRule(cboTracking, rule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlProvisionalDiagnosis()
        {
            try
            {
                CommonValidateMaxLength rule = new CommonValidateMaxLength();
                rule.textEdit = txtProvisionalDiagnosis;
                rule.maxLength = 1000;
                dxValidationProviderControl.SetValidationRule(txtProvisionalDiagnosis, rule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidateLookupWithTextEdit(LookUpEdit cbo, TextEdit textEdit, DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProviderEditor)
        {
            try
            {
                LookupEditWithTextEditValidationRule validRule = new LookupEditWithTextEditValidationRule();
                validRule.txtTextEdit = textEdit;
                validRule.cbo = cbo;
                validRule.ErrorText = MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditor.SetValidationRule(textEdit, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateGridLookupWithTextEdit(GridLookUpEdit cbo, TextEdit textEdit, DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProviderEditor)
        {
            try
            {
                GridLookupEditWithTextEditValidationRule validRule = new GridLookupEditWithTextEditValidationRule();
                validRule.txtTextEdit = textEdit;
                validRule.cbo = cbo;
                validRule.ErrorText = MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditor.SetValidationRule(textEdit, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationSingleControl(BaseEdit control, DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProviderEditor)
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = control;
                validRule.ErrorText = MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditor.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationSingleControl(BaseEdit control, DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProviderEditor, string messageErr, IsValidControl isValidControl)
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = control;
                if (isValidControl != null)
                {
                    validRule.isValidControl = isValidControl;
                    validRule.isUseOnlyCustomValidControl = true;
                }
                if (!String.IsNullOrEmpty(messageErr))
                    validRule.ErrorText = messageErr;
                else
                    validRule.ErrorText = MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditor.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
