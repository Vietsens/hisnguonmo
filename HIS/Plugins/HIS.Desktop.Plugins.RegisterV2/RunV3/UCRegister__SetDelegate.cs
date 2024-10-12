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
using HIS.Desktop.Plugins.Library.RegisterConfig;
using HIS.UC.UCImageInfo.ADO;
using MOS.SDO;

namespace HIS.Desktop.Plugins.RegisterV2.Run2
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
				this.ucPatientRaw1.GetDataBySearchPatient(FillDataAfterSearchPatientInUCPatientRaw, FillDataPreviewForSearchByQrcodeInUCPatientRaw, InitExamServiceRoomByAppoimentTime);
				this.ucPatientRaw1.InitDelegateProcessChangePatientDob(ProcessWhileChangeDOb);
				this.ucPatientRaw1.SetDelegateVisibleUCHein(IsVisibleUCHein);
				this.ucPatientRaw1.SetDelegateShowControlHrmKskCode(this.ShowControlHrmKskCode);
				this.ucPatientRaw1.SetDelegateShowControlHrmKskCodeNotValid(this.ShowControlHrmKskCodeNotValid);
				this.ucPatientRaw1.SetDelegateShowControlGuaranteeLoginname(this.ShowControlGuaranteeLoginname);
				this.ucPatientRaw1.SetDelegateSendPatientName(this.SendPatientName);
				this.ucPatientRaw1.SetDelegateSendPatientSDO(this.SendPatientSDO);
				this.ucPatientRaw1.SetDelegateShowCheckWorkingLetter(this.ucHeinInfo1.ShowCheckWorkingLetter);
				this.ucPatientRaw1.SetDelegateCheckSS(this.ucHeinInfo1.ShowCheckSS);
				this.ucPatientRaw1.SetDelegateShowOrtherPaySource(this.ucOtherServiceReqInfo1.ShowOrtherPay);
				this.ucPatientRaw1.SetDelegateSendTypeFind(ChangeFindTypeInPatientRaw);
				this.ucPatientRaw1.SetDelegateCheckboxExamOnline(this.ucOtherServiceReqInfo1.CheckExamOnline);
                this.ucOtherServiceReqInfo1.SetDelegateHeinRightRouteType(this.SetRightRouteEmergencyWhenRegisterOutTime);
				this.ucOtherServiceReqInfo1.SetDelegatePriorityNumberChanged(this.SetServuceRoomAddButtonWhenRegisterHasPriorityNumber);
				this.ucOtherServiceReqInfo1.FillDataOweTypeDefault();
				
				if (HisConfigCFG.IsAutoFocusToSavePrintAfterChoosingExam)
				{
					this.ucServiceRoomInfo1.FocusNextUserControl(focusToBtnSaveAndPrint);
				}
				else
				{
					this.ucServiceRoomInfo1.FocusNextUserControl(focusToBtnSave);
				}

				//this.ucServiceRoomInfo1.FocusNextUserControl(focusToBtnSaveAndPrint);

				await this.ucPlusInfo1.UCPlusInfoOnLoadAsync();
				this.ucPlusInfo1.FocusNextUserControl(focusToUCImageInfo);

				UCImageInfoADO dataImage = new UCImageInfoADO();
				dataImage._FocusNextUserControl = focusOutUserControlImageInfo;
				dataImage._ReloadDataByCmndAfter = ReloadDataByCmndAfter;
				dataImage._ReloadDataByCmndBefore = ReloadDataByCmndBefore;
				this.ucImageInfo1.SetValue(dataImage);

				this.ValidationRelative();
				this.EnableLciBenhNhanMoiAfterSearchPatient();
				this.SetDelegateCheckTT();
				this.SetDelegateEnableButtonSave();
				long patientTypeId = this.ucPatientRaw1.GetValue().PATIENTTYPE_ID;
				if (patientTypeId == HisConfigCFG.PatientTypeId__BHYT
					|| patientTypeId == HisConfigCFG.PatientTypeId__QN)
				{
					this.ucAddressCombo1.FocusNextUserControl(focusToUCHeinInfo);
				}
				else if (patientTypeId == HisConfigCFG.PatientTypeId__KSK)
				{
					if (this.ucKskContract != null && this.kskContractProcessor != null)
					{
						DelegateFocusNextUserControl dlg = FocusInKskContract;
						this.ucAddressCombo1.FocusNextUserControl(dlg);
					}
				}
				else
				{
					if (this.ucServiceRoomInfo1 != null)
					{
						DelegateFocusNextUserControl dlg = this.ucServiceRoomInfo1.FocusUserControl;
						this.ucAddressCombo1.FocusNextUserControl(dlg);
					}
				}
				
				long? treatmentTypeId = this.ucPatientRaw1.GetValue().TREATMENT_TYPE_ID;
				Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("FocusNextUserControl treatmentTypeId ", treatmentTypeId));
				if (HisConfigCFG.IsDefaultTreatmentTypeExam)
				{
					AutoSetTreatmentTypeCombo(IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM);
				}
				else if (treatmentTypeId.HasValue && treatmentTypeId.Value == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNGOAITRU)
				{
					AutoSetTreatmentTypeCombo(treatmentTypeId);
				}


				this.ucAddressCombo1.SetDelegateSetAddressUCHein(this.SetDelegateSetAddressUCHein);
				this.ucAddressCombo1.SetDelegateSetAddressUCPlusInfo(this.SetDelegateSetAddressUCProvinceOfBirth);
				this.ucAddressCombo1.SetDelegateSendProvince(this.SendCodeProvince);
				this.ucAddressCombo1.SetDelegateSendCardSDO(SendCardSDO);
				this.ucHeinInfo1.SetEnableControlEmergency(this.SetEnableEmergency);
				this.ucHeinInfo1.SetShowThongTinChuyenTuyen(this.ShowFormThongTinChuyenTuyen);
				this.ucHeinInfo1.SetCareerByHeinCardNumber(this.SetCareerByCardNumber);
				this.ucHeinInfo1.SetDelegateDisableBtnTTCT(this.SetDisableBtnTTCT);//#15753
				this.ucHeinInfo1.FocusNextUserControl(focusToUCServiceRoomInfo);
				this.ucHeinInfo1.SetDelegateChangePatientDob(ProcessWhileChangeDOb);
				this.ucHeinInfo1.SetInformationPatientRawFromUC(this.ucPatientRaw1);
				this.ucHeinInfo1.SetCurrentModule(this.currentModule);
				this.ucHeinInfo1.Send3WBhytCode(Send3WCode);
				this.EnableOrDisablechkTheTam();
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

        private void SendCardSDO(HisCardSDO cardSDO)
        {
            try
            {
				this.ucPatientRaw1.GetCardSDO(cardSDO);
            }
            catch (Exception ex)
            {
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
        }

        private void Send3WCode(string code)
		{
			try
			{
				this.ucPatientRaw1.Set3WBhytCode(code);
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

		private void focusToUCHeinInfo()
		{
			try
			{
				this.ucHeinInfo1.FocusUserControl();
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void focusToUCServiceRoomInfo()
		{
			try
			{
				this.ucServiceRoomInfo1.FocusUserControl();
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void focusToUCOtherServiceReqInfo()
		{
			try
			{
				this.ucOtherServiceReqInfo1.FocusUserControl();
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

		private void focusToUCImageInfo()
		{
			try
			{
				this.ucImageInfo1.FocusUserControl();
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

		private void focusToBtnSaveAndPrint()
		{
			this.btnSaveAndPrint.Focus();
		}

		private void SetDelegateFocusWhenPatientIsChild(bool _isPatientChild)
		{
			try
			{
				if (_isPatientChild == true)
				{
					this.ucRelativeInfo1.FocusNextUserControl(focusToUCAddressCombo);
				}

				else
				{
					this.ucRelativeInfo1.FocusNextUserControl(focusToUCOtherServiceReqInfo);
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}
		#endregion

		#region Validate - Enable Control
		private void EnableOrDisablechkTheTam()
		{
			try
			{
				DelegateEnableOrDisableControl _isEnable;
				_isEnable = this.ucHeinInfo1.ChangeCheckCardTemp;
				this.ucPatientRaw1.EnableOrDisableControl(_isEnable);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

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
				if (patientTypeID != null && patientTypeID > 0)
					this.SuspendLayoutWithPatientTypeChanged(patientTypeID);
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
				this.ucHeinInfo1.SetTypeCardTemp(patientTypeID);
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
				// this.btnTTChuyenTuyen.Enabled = _isDisable;
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
				this.ucPatientRaw1.SetDelegateCheckTT(CheckTTFull);
				this.ucHeinInfo1.SetDelegateCheckTT(CheckTTFull);
				this.ucPatientRaw1.SetIsReadQrCode(SetIsReadQrCode);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

        private void SetDelegateEnableButtonSave()
		{
			try
			{
				this.ucPatientRaw1.SetDelegateEnableButtonSave(EnableSave);
				this.ucPatientRaw1.SetDelegateHeinEnableButtonSave(HeinEnableSave);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}
		bool IsEnablePatientKey = false;
		bool IsRunDelegateEnableSave = false;
		private void EnableSave(bool isEnable)
		{
			try
			{
				IsRunDelegateEnableSave = true;
				this.IsEnablePatientKey = isEnable;
				btnSave.Enabled = IsEnablePatientKey;
				btnSaveAndPrint.Enabled = IsEnablePatientKey;
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}
		private void HeinEnableSave(bool isEnable)
		{
			try
			{
				if (!IsRunDelegateEnableSave)
				{
					IsEnablePatientKey = true;
				}
				btnSave.Enabled = IsEnablePatientKey && isEnable;
				btnSaveAndPrint.Enabled = IsEnablePatientKey && isEnable;
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
				this.ucAddressCombo1.isReadCard = _isReadQrCode;
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
				this.ucHeinInfo1.SetValueAddress(address);
				this.ucRelativeInfo1.SetValueAddress(address);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void SendCodeProvince(string codeProvince)
		{
			try
			{
				this.ucHeinInfo1.SetCodeProvince(codeProvince);
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
				this.ucOtherServiceReqInfo1.SetEnableControl(isEnable);
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

		private void SetDelegateForResetRegister()
		{
			try
			{
				this.ucPatientRaw1.SetDelegateForResetRegisterForm(RefreshUserControl);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

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
				this.ucHeinInfo1.RightRouteEmergencyWhenRegisterOutTime(_isOutTime);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void SetServuceRoomAddButtonWhenRegisterHasPriorityNumber(long? priorityNumber)
		{
			try
			{
				this.ucServiceRoomInfo1.PriorityChanged(priorityNumber);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}
	}
}
