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
using HIS.Desktop.Utility;
using HIS.Desktop.DelegateRegister;
using Inventec.Common.QrCodeBHYT;
using DevExpress.XtraLayout.Utils;
using HIS.Desktop.Plugins.RegisterVaccination.ValidationRule;
using DevExpress.XtraEditors.DXErrorProvider;

namespace HIS.Desktop.Plugins.RegisterVaccination.Run3
{
    public partial class UCRegister : UserControlBase
    {
        #region focus UserControl

        private void FocusNextUserControl()
        {
            try
            {
                this.ucPatientRaw1.FocusNextUserControl(focusToUCAddressCombo);
                this.ucPatientRaw1.FocusToUCRelativeWhenPatientIsChild(focusToUCPersonHomeInfo);
                this.ucPatientRaw1.SetDelegateFocusNextUserControlWhenPatientIsChild(this.SetDelegateFocusWhenPatientIsChild);
                this.ucPatientRaw1.GetDataBySearchPatient(FillDataAfterSearchPatientInUCPatientRaw);
                this.ucPatientRaw1.SetDelegateVisibleUCHein(IsVisibleUCHein);

                this.ValidationRelative();
                this.EnableLciBenhNhanMoiAfterSearchPatient();
                this.SetDelegateCheckTT();

                this.ucAddressCombo1.SetDelegateSetAddressUCHein(this.SetDelegateSetAddressUCHein);
                this.ucAddressCombo1.SetDelegateSetAddressUCPlusInfo(this.SetDelegateSetAddressUCProvinceOfBirth);

                this.ucAddressCombo1.FocusNextUserControl(focuschkDoiTuong);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void focuschkDoiTuong()
        {
            try
            {
                txtPatientTypeCode.Focus();
                txtPatientTypeCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void focusToUCAddressCombo()
        {
            try
            {
                this.ucAddressCombo1.FocusUserControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void focusToUCPersonHomeInfo()
        {
            try
            {
                // this.ucRelativeInfo1.FocusUserControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void focusToUCPersonHomeInfoV3(string str)
        {
            try
            {
                //this.ucRelativeInfo1.FocusUserControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void focusOutUserControlImageInfo(object obj)
        {
            try
            {
                if (btnSave.Enabled == true)
                {
                    btnSave.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void focusToBtnSave()
        {
            try
            {
                this.btnSave.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDelegateFocusWhenPatientIsChild(bool _isPatientChild)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        #endregion

        #region Validate - Enable Control

        private void ValidationRelative()
        {
            try
            {
                this.ucPatientRaw1.ValidateRelative((DelegateValidationUserControl)SetValidateControl);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        //Vali ng nha
        private void SetValidateControl(bool _isObligatory)
        {
            try
            {
                ValidControl(this.txtDiaChi, 200, false);
                ValidControl(this.txtQuanHe, 50, false);
                ValidControl(this.txtNguoiNha, 100, _isObligatory);
                if (_isObligatory == true)
                {
                    this.layoutControlItem25.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
                }
                else
                {
                    this.layoutControlItem25.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidControl(DevExpress.XtraEditors.TextEdit txtEdit, int maxlength, bool isVali)
        {
            try
            {
                TextEditMaxLengthValidationRule _Rule = new TextEditMaxLengthValidationRule();
                _Rule.txtEdit = txtEdit;
                _Rule.maxlength = maxlength;
                _Rule.isVali = isVali;
                _Rule.ErrorText = "Trường dữ liệu bắt buộc";
                _Rule.ErrorType = ErrorType.Warning;
                this.dxValidationProvider1.SetValidationRule(txtEdit, _Rule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void EnableLciBenhNhanMoiAfterSearchPatient()
        {
            try
            {
                DelegateEnableOrDisableBtnPatientNew _isEnableLciBenhNhanMoi;
                _isEnableLciBenhNhanMoi = this.EnableLciBenhNhanMoi;
                this.ucPatientRaw1.EnableOrDisableBtnPatientNew(_isEnableLciBenhNhanMoi);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void EnableLciBenhNhanMoi(bool _enableLciBenhNhanMoi)
        {
            try
            {
                this.lcibtnPatientNewInfo.Enabled = _enableLciBenhNhanMoi;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void IsVisibleUCHein(long patientTypeID)
        {
            try
            {
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetTypeCardTemp(long patientTypeID)
        {
            try
            {
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        #endregion

        #region CheckTT - Set Address

        private void SetDelegateCheckTT()
        {
            try
            {
                this.ucPatientRaw1.SetIsReadQrCode(SetIsReadQrCode);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetIsReadQrCode(bool _isReadQrCode)
        {
            try
            {
                this.isReadQrCode = _isReadQrCode;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDelegateSetAddressUCHein(string address)
        {
            try
            {
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDelegateSetAddressUCPlusInfo(string address)
        {
            try
            {
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDelegateSetAddressUCProvinceOfBirth(object data, bool isCallByUCAddress)
        {
            try
            {
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetEnableEmergency(bool isEnable)
        {
            try
            {
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }



        #endregion

        #region Set Du Lieu Cho UCPatientRaw

        private void SetCareerByCardNumber(string heinCardNumber)
        {
            try
            {
                this.ucPatientRaw1.SetCareerByCardNumber(heinCardNumber);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        /// <summary>
        /// Hàm gán module hiện đang làm việc cho UCWorkPlace
        /// </summary>
        private void SetCurrentModuleForUCWorkPlace()
        {
            try
            {
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Hàm set trường hợp khi đăng ký ngoài giờ
        /// </summary>
        /// <param name="_isOutTime"></param>
        private void SetRightRouteEmergencyWhenRegisterOutTime(bool _isOutTime)
        {
            try
            {
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
