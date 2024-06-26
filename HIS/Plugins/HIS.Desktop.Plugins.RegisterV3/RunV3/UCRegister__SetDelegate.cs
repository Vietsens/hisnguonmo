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

namespace HIS.Desktop.Plugins.RegisterV3.Run3
{
    public partial class UCRegister : UserControlBase
    {
        #region focus UserControl

        private async Task FocusNextUserControl()
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

                this.ucAddressCombo1.FocusNextUserControl(focuschkPhongTiem);

                //this.ucRelativeInfo1.FocusNextUserControl(focuschkPhongTiem);
                await this.ucPlusInfo1.UCPlusInfoOnLoadAsync();


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void focuschkPhongTiem()
        {
            try
            {
                chkPhongTiem.Focus();
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
                this.ucRelativeInfo1.FocusUserControl();
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
                this.ucRelativeInfo1.FocusUserControl();
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
                if (_isPatientChild == true)
                {
                    if (this.ucRoomVitaminA != null)
                    {
                        this.ucRoomVitaminA.chkTreEm.Checked = true;
                        this.ucRoomVitaminA.layoutControlItem3.Visibility = LayoutVisibility.Always;
                        this.ucRoomVitaminA.layoutControlItem9.Visibility = LayoutVisibility.Always;
                        this.ucRoomVitaminA.chkOneMonthBorn.Checked = false;
                        this.ucRoomVitaminA.layoutControlItem2.Visibility = LayoutVisibility.Never;
                        this.ucRoomVitaminA.chkPhuNuSauSinh.Checked = false;
                        this.ucRoomVitaminA.chkKhac.Checked = false;
                    }
                    //this.ucRelativeInfo1.FocusNextUserControl(focusToUCAddressCombo);
                }

                else
                {
                    //this.ucRelativeInfo1.FocusNextUserControl(focusToUCOtherServiceReqInfo);
                }
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
                DelegateValidationUserControl _isValidate;
                _isValidate = this.ucRelativeInfo1.SetValidateControl;
                this.ucPatientRaw1.ValidateRelative(_isValidate);
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

        private void SetDisableBtnTTCT(bool _isDisable)
        {
            try
            {
                //this.btnTTChuyenTuyen.Enabled = _isDisable;
                if (_isDisable == false)
                    this.transPatiADO = null;
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
                this.ucRelativeInfo1.SetValueAddress(address);
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
                this.ucPlusInfo1.SetAddressNow(address);
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
                this.ucPlusInfo1.SetDataAddress(data, isCallByUCAddress);
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

        private void SetShowThongTinChuyenTuyen(bool isShowForm)
        {
            try
            {
                this.ShowFormThongTinChuyenTuyen(isShowForm);
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
                this.ucPlusInfo1.SetCurrentModuleAgain(this.currentModule);
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
