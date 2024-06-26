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
using HIS.UC.UCHeniInfo.ADO;
using HIS.Desktop.DelegateRegister;
using HIS.UC.UCHeniInfo.CustomValidateRule;
using DevExpress.XtraEditors.DXErrorProvider;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.UC.UCHeniInfo.Data;
using HIS.Desktop.Utility;

namespace HIS.UC.UCHeniInfo
{
    public partial class UCHeinInfo : UserControlBase
    {
        #region Public Method
        public void InitValidateRule(long patientTypeID)
        {
            try
            {
                if (patientTypeID == HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.PatientTypeId__BHYT)
                {
                    this.ValidateAddress(this.txtAddress, 500);
                    this.ValidTxtSoThe();
                    this.ValidHeinCardToTime();
                    this.ValidHeinCardFromTime();
                    this.ValidRightRouteTypeA();
                    this.ValidFreeCoPainTime(false);
                    this.ValidComboDKKCBBD();
                }
                else
                {
                    ResetValidate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void ValidComboDKKCBBD()
        {
            try
            {
                TemplateHeinBHYT1__HeinMediOrg__ValidationRule rule = new TemplateHeinBHYT1__HeinMediOrg__ValidationRule();
                rule.cboDKKCBBD = cboDKKCBBD;
                rule.txtMaDKKCBBD = txtMaDKKCBBD;
                this.dxValidationProviderControl.SetValidationRule(txtMaDKKCBBD,rule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public bool ValidateRequiredField()
        {
            bool valid = true;
            try
            {
                valid = this.dxValidationProviderControl.Validate();
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
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(this.dxValidationProviderControl, this.dxErrorProviderControl);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void ResetRequiredFieldWhenCardTempIsQN()
        {
            try
            {
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(this.dxValidationProviderControl, this.dxErrorProviderControl);
                this.ResetValidateWhenCardTempIsQN();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        private void ValidateAddress(DevExpress.XtraEditors.TextEdit txtEdit, int maxlength)
        {
            TemplateHeinBHYT1__Address__ValidationRule _rule = new TemplateHeinBHYT1__Address__ValidationRule();
            _rule.txtEdit = txtEdit;
            _rule.maxlength = maxlength;
            _rule.ErrorText = MessageUtil.GetMessage(His.UC.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
            _rule.ErrorType = ErrorType.Warning;
            this.dxValidationProviderControl.SetValidationRule(txtEdit, _rule);
        }

        private void ResetValidate()
        {
            try
            {
              
                this.dxValidationProviderControl.SetValidationRule(this.txtAddress, null);
                this.dxValidationProviderControl.SetValidationRule(this.txtMaDKKCBBD, null);
                this.dxValidationProviderControl.SetValidationRule(this.txtSoThe, null);
                this.dxValidationProviderControl.SetValidationRule(this.txtHeinCardToTime, null);
                this.dxValidationProviderControl.SetValidationRule(this.txtHeinCardFromTime, null);
                this.dxValidationProviderControl.SetValidationRule(this.cboHeinRightRoute, null);
                this.dxValidationProviderControl.SetValidationRule(this.txtFreeCoPainTime, null);
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(this.dxValidationProviderControl, this.dxErrorProviderControl);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ResetValidateWhenCardTempIsQN()
        {
            try
            {
                this.ValidateAddress(this.txtAddress, 500);
                this.ValidComboDKKCBBD();
                this.ValidRightRouteTypeA();
                this.ValidFreeCoPainTime(false);
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
                oDateRule.ErrorText = MessageUtil.GetMessage(His.UC.LibraryMessage.Message.Enum.NguoiDungNhapNgayKhongHopLe);
                oDateRule.ErrorType = ErrorType.Warning;
                this.dxValidationProviderControl.SetValidationRule(this.txtFreeCoPainTime, oDateRule);
            }
            else
            {
                this.dxValidationProviderControl.SetValidationRule(this.txtFreeCoPainTime, null);
            }
        }

        private void ValidHeinCardToTime()
        {
            try
            {
                TemplateHeinBHYT1__HeinCardToTime__ValidationRule oDateRule = new TemplateHeinBHYT1__HeinCardToTime__ValidationRule();
                oDateRule.txtHeinCardToTime = this.txtHeinCardToTime;
                oDateRule.txtHeinCardFromTime = this.txtHeinCardFromTime;
                oDateRule.checkKhongKTHSD = this.chkKhongKTHSD;
                oDateRule.isShowCheckKhongKTHSD = this.isShowCheckKhongKTHSD;
                oDateRule.IsEdit = this.isEdit;
                oDateRule.PatientTypeId = HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.PatientTypeId__BHYT;
                oDateRule.ErrorText = MessageUtil.GetMessage(His.UC.LibraryMessage.Message.Enum.NguoiDungNhapNgayKhongHopLe);
                oDateRule.ErrorType = ErrorType.Warning;
                this.dxValidationProviderControl.SetValidationRule(this.txtHeinCardToTime, oDateRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ClearValidHeinCardToTime()
        {
            try
            {
                this.dxValidationProviderControl.SetValidationRule(this.txtHeinCardToTime, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidHeinCardFromTime()
        {
            TemplateHeinBHYT1__HeinCardFromTime__ValidationRule oDateRule = new TemplateHeinBHYT1__HeinCardFromTime__ValidationRule();
            oDateRule.txtHeinCardFromTime = this.txtHeinCardFromTime;
            oDateRule.PatientTypeId = HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.PatientTypeId__BHYT;
            oDateRule.ErrorText = MessageUtil.GetMessage(His.UC.LibraryMessage.Message.Enum.NguoiDungNhapNgayKhongHopLe);
            oDateRule.ErrorType = ErrorType.Warning;
            this.dxValidationProviderControl.SetValidationRule(this.txtHeinCardFromTime, oDateRule);
        }

        private void ValidRightRouteTypeA()
        {
            try
            {
                UCHeinInfo__RightRouteType__ValidationRule oDobDateRule = new UCHeinInfo__RightRouteType__ValidationRule();
                oDobDateRule.cboHeinRightRoute = cboHeinRightRoute;
                oDobDateRule.IsTempQN = isTempQN;
                oDobDateRule.chkTempQN = chkHasCardTemp;
                oDobDateRule.ErrorText = MessageUtil.GetMessage(His.UC.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                oDobDateRule.ErrorType = ErrorType.Warning;
                this.dxValidationProviderControl.SetValidationRule(cboHeinRightRoute, oDobDateRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn("Tiep don: UCHeinInfo/ValidRightRouteTypeA:\n" + ex);
            }
        }

        private void ValidTxtSoThe()
        {
            try
            {
                TemplateHeinBHYT1__HeinCardNumber__ValidationRule oDobDateRule = new TemplateHeinBHYT1__HeinCardNumber__ValidationRule();
                oDobDateRule.txtSoThe = this.txtSoThe;
                oDobDateRule.PatientTypeId = HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.PatientTypeId__BHYT;
                oDobDateRule.chkHasDobCertificate = this.chkHasCardTemp;
                oDobDateRule.BhytBlackLists = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BHYT_BLACKLIST>();
                oDobDateRule.BhytWhiteLists = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BHYT_WHITELIST>();
                oDobDateRule.ErrorText = MessageUtil.GetMessage(His.UC.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                oDobDateRule.ErrorType = ErrorType.Warning;
                this.dxValidationProviderControl.SetValidationRule(this.txtSoThe, oDobDateRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}
