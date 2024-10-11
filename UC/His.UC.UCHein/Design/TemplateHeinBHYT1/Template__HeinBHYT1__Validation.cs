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
using His.UC.UCHein.Design.TemplateHeinBHYT1.ValidationRule;
using His.UC.UCHein.ValidationRule;
using Inventec.Desktop.Common.Controls.ValidationRule;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace His.UC.UCHein.Design.TemplateHeinBHYT1
{
    public partial class Template__HeinBHYT1 : UserControl
    {
        private void ValidControl()
        {
            try
            {
                this.ValidTxtSoThe();
                this.ValidNoiDKKCBBD();
                this.ValidFreeCoPainTime(true);
                this.ValidHeinCardToTime();
                this.ValidHeinCardFromTime();
                this.ValidRightRouteType();
                this.ValidAddress();
                this.ValidHNCode();
                //this.ValidNoiChuyenDen();
                this.ValidIcd();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidIcd()
        {
            try
            {
                TemplateHeinBHYT1__Icd__ValidationRule oDobDateRule = new TemplateHeinBHYT1__Icd__ValidationRule();
                oDobDateRule.txtMaChanDoanTD = txtMaChanDoanTD;
                oDobDateRule.cboChanDoanTD = cboChanDoanTD;
                oDobDateRule.ErrorText = ResourceMessage.MaBenhChinhKhongHopLe;
                oDobDateRule.ErrorType = ErrorType.Warning;
                this.dxValidationProvider1.SetValidationRule(txtMaChanDoanTD, oDobDateRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidHNCode()
        {
            try
            {
                TemplateHeinBHYT1__HNCode__ValidationRule oDateRule = new TemplateHeinBHYT1__HNCode__ValidationRule();
                oDateRule.txtHNCode = txtHNCode;
                oDateRule.ErrorText = Inventec.Common.Resource.Get.Value("His.UC.UCHein.Message.MaHoNgheoKhongHopLe", Resources.ResourceLanguageManager.LanguageUCHeinBHYT, Base.LanguageManager.GetCulture());
                oDateRule.ErrorType = ErrorType.Warning;
                this.dxValidationProvider1.SetValidationRule(txtHNCode, oDateRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidFreeCoPainTime(bool _isValidate)
        {
            if (_isValidate == true)
            {
                TemplateHeinBHYT1__FreeCoPainTime__ValidationRule oDateRule = new TemplateHeinBHYT1__FreeCoPainTime__ValidationRule();
                oDateRule.txtFreeCoPainTime = this.txtFreeCoPainTime;
                oDateRule.chkJoin5Year = this.chkJoin5Year;
                oDateRule.chkPaid6Month = this.chkPaid6Month;
                oDateRule.ErrorText = His.UC.UCHein.Base.MessageUtil.GetMessage(His.UC.LibraryMessage.Message.Enum.NguoiDungNhapNgayKhongHopLe);
                oDateRule.ErrorType = ErrorType.Warning;
                this.dxValidationProvider1.SetValidationRule(this.txtFreeCoPainTime, oDateRule);
            }
            else
            {
                this.dxValidationProvider1.SetValidationRule(this.txtFreeCoPainTime, null);
            }
            
        }

        private void ValidHeinCardToTime()
        {
            TemplateHeinBHYT1__HeinCardToTime__ValidationRule oDateRule = new TemplateHeinBHYT1__HeinCardToTime__ValidationRule();
            oDateRule.txtHeinCardToTime = this.txtHeinCardToTime;
            oDateRule.txtHeinCardFromTime = this.txtHeinCardFromTime;
            oDateRule.checkKhongKTHSD = this.checkKhongKTHSD;
            oDateRule.isShowCheckKhongKTHSD = this.isShowCheckKhongKTHSD;
            oDateRule.IsEdit = this.IsEdit;
            oDateRule.ExceedDayAllow = this.ExceedDayAllow;
            oDateRule.PatientTypeId = this.PatientTypeId;
            oDateRule.ErrorText = His.UC.UCHein.Base.MessageUtil.GetMessage(His.UC.LibraryMessage.Message.Enum.NguoiDungNhapNgayKhongHopLe);
            oDateRule.ErrorType = ErrorType.Warning;
            this.dxValidationProvider1.SetValidationRule(this.txtHeinCardToTime, oDateRule);
        }

        private void ValidHeinCardFromTime()
        {
            TemplateHeinBHYT1__HeinCardFromTime__ValidationRule oDateRule = new TemplateHeinBHYT1__HeinCardFromTime__ValidationRule();
            oDateRule.txtHeinCardFromTime = this.txtHeinCardFromTime;
            oDateRule.PatientTypeId = this.PatientTypeId;
            oDateRule.ErrorText = His.UC.UCHein.Base.MessageUtil.GetMessage(His.UC.LibraryMessage.Message.Enum.NguoiDungNhapNgayKhongHopLe);
            oDateRule.ErrorType = ErrorType.Warning;
            this.dxValidationProvider1.SetValidationRule(this.txtHeinCardFromTime, oDateRule);
        }

        private void ValidRightRouteType(string heinMediOrgCode)
        {
            try
            {
                TemplateHeinBHYT1__RightRouteType__ValidationRule oDobDateRule = new TemplateHeinBHYT1__RightRouteType__ValidationRule();
                oDobDateRule.txtHeinRightRouteCode = txtHeinRightRouteCode;
                oDobDateRule.cboHeinRightRoute = cboHeinRightRoute;
                oDobDateRule.ErrorText = His.UC.UCHein.Base.MessageUtil.GetMessage(His.UC.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                oDobDateRule.ErrorType = ErrorType.Warning;
                this.dxValidationProvider1.SetValidationRule(txtHeinRightRouteCode, oDobDateRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidRightRouteType()
        {
            try
            {
                lblRightRouteType.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
                TemplateHeinBHYT1__RightRouteType__ValidationRule oDobDateRule = new TemplateHeinBHYT1__RightRouteType__ValidationRule();
                oDobDateRule.txtHeinRightRouteCode = this.txtHeinRightRouteCode;
                oDobDateRule.cboHeinRightRoute = this.cboHeinRightRoute;
                oDobDateRule.ErrorText = His.UC.UCHein.Base.MessageUtil.GetMessage(His.UC.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                oDobDateRule.ErrorType = ErrorType.Warning;
                this.dxValidationProvider1.SetValidationRule(this.txtHeinRightRouteCode, oDobDateRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidAddress()
        {
            try
            {
                TemplateHeinBHYT1__Address__ValidationRule oDobDateRule = new TemplateHeinBHYT1__Address__ValidationRule();
                oDobDateRule.txtAddress = this.txtAddress;
                oDobDateRule.ErrorText = His.UC.UCHein.Base.MessageUtil.GetMessage(His.UC.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                oDobDateRule.ErrorType = ErrorType.Warning;
                this.dxValidationProvider1.SetValidationRule(this.txtAddress, oDobDateRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidNoiChuyenDen()
        {
            try
            {
                TemplateHeinBHYT1__NoiChuyenDen__ValidationRule oDobDateRule = new TemplateHeinBHYT1__NoiChuyenDen__ValidationRule();
                oDobDateRule.cboNoiChuyenDen = this.cboNoiChuyenDen;
                oDobDateRule.txtMaNoiChuyenDen = this.txtMaNoiChuyenDen;
                oDobDateRule.ErrorText = His.UC.UCHein.Base.MessageUtil.GetMessage(His.UC.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                oDobDateRule.ErrorType = ErrorType.Warning;
                this.dxValidationProvider1.SetValidationRule(this.txtMaNoiChuyenDen, oDobDateRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidChuyenTuyen()
        {
            try
            {
                TemplateHeinBHYT1__ChuyenTuyen__ValidationRule oDobDateRule = new TemplateHeinBHYT1__ChuyenTuyen__ValidationRule();
                oDobDateRule.chkMediRecordNoRouteTransfer = this.chkMediRecordNoRouteTransfer;
                oDobDateRule.chkMediRecordRouteTransfer = this.chkMediRecordRouteTransfer;
                oDobDateRule.ErrorText = His.UC.UCHein.Base.MessageUtil.GetMessage(His.UC.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                oDobDateRule.ErrorType = ErrorType.Warning;
                this.dxValidationProvider1.SetValidationRule(this.chkMediRecordRouteTransfer, oDobDateRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidTxtSoThe()
        {
            try
            {
                TemplateHeinBHYT1__HeinCardNumber__ValidationRule oDobDateRule = new TemplateHeinBHYT1__HeinCardNumber__ValidationRule();
                oDobDateRule.txtSoThe = this.txtSoThe;
                oDobDateRule.PatientTypeId = this.PatientTypeId;
                oDobDateRule.chkHasDobCertificate = this.chkHasDobCertificate;
                oDobDateRule.BhytBlackLists = this.entity.BhytBlackLists;
                oDobDateRule.BhytWhiteLists = this.entity.BhytWhiteLists;
                oDobDateRule.ErrorText = His.UC.UCHein.Base.MessageUtil.GetMessage(His.UC.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                oDobDateRule.ErrorType = ErrorType.Warning;
                this.dxValidationProvider1.SetValidationRule(this.txtSoThe, oDobDateRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidNoiDKKCBBD()
        {
            try
            {
                TemplateHeinBHYT1__MediOrg__ValidationRule oDobDateRule = new TemplateHeinBHYT1__MediOrg__ValidationRule();
                oDobDateRule.txtMaDKKCBBD = this.txtMaDKKCBBD;
                oDobDateRule.cboDKKCBBD = this.cboDKKCBBD;
                oDobDateRule.PatientTypeId = this.PatientTypeId;
                oDobDateRule.chkHasDobCertificate = this.chkHasDobCertificate;
                oDobDateRule.ErrorText = His.UC.UCHein.Base.MessageUtil.GetMessage(His.UC.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                oDobDateRule.ErrorType = ErrorType.Warning;
                this.dxValidationProvider1.SetValidationRule(this.txtMaDKKCBBD, oDobDateRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidIcdByDTGT()
        {
            try
            {
                TemplateHeinBHYT1__Icd__ValidationRule_Is_MediOrg oDobDateRule = new TemplateHeinBHYT1__Icd__ValidationRule_Is_MediOrg();
                oDobDateRule.txtIcdName = this.txtDialogText;
                oDobDateRule.ErrorText = ResourceMessage.BatBuocNhapTenBenhVoiTruongHopBenhNhanLaDungTuyenGioiThieu;
                oDobDateRule.ErrorType = ErrorType.Warning;
                this.dxValidationProvider1.SetValidationRule(txtMaChanDoanTD, oDobDateRule);
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

        private void ResetErrorValidateRule()
        {
            try
            {
                IList<Control> invalidControls = this.dxValidationProvider1.GetInvalidControls();
                for (int i = invalidControls.Count - 1; i >= 0; i--)
                {
                    this.dxValidationProvider1.RemoveControlError(invalidControls[i]);
                }
                this.dxErrorProvider1.ClearErrors();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        protected void ValidateLookupWithTextEdit(LookUpEdit cbo, TextEdit textEdit, DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProviderEditor)
        {
            try
            {
                LookupEditWithTextEditValidationRule validRule = new LookupEditWithTextEditValidationRule();
                validRule.txtTextEdit = textEdit;
                validRule.cbo = cbo;
                validRule.ErrorText = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditor.SetValidationRule(textEdit, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        protected void ValidateGridLookupWithTextEdit(GridLookUpEdit cbo, TextEdit textEdit, DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProviderEditor)
        {
            try
            {
                GridLookupEditWithTextEditValidationRule validRule = new GridLookupEditWithTextEditValidationRule();
                validRule.txtTextEdit = textEdit;
                validRule.cbo = cbo;
                validRule.ErrorText = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditor.SetValidationRule(textEdit, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        protected void ValidationSingleControl(BaseEdit control, DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProviderEditor)
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = control;
                validRule.ErrorText = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditor.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        protected void ValidationSingleControl(Control control, DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProviderEditor, string messageErr, IsValidControl isValidControl)
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = control;
                if (isValidControl != null)
                {
                    validRule.isUseOnlyCustomValidControl = true;
                    validRule.isValidControl = isValidControl;
                }
                if (!String.IsNullOrEmpty(messageErr))
                    validRule.ErrorText = messageErr;
                else
                    validRule.ErrorText = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
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
