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
using HIS.UC.UCPatientRaw.Valid;
using DevExpress.XtraEditors.DXErrorProvider;
using HIS.Desktop.LocalStorage.HisConfig;

namespace HIS.UC.UCPatientRaw
{
	public partial class UCPatientRaw : HIS.Desktop.Utility.UserControlBase
    {
		#region Outside Validate

		public bool ValidateRequiredField()
		{
			bool valid = true;
			try
			{

				this.positionHandleControl = -1;
				if (!this.dxValidationProviderControl.Validate())
				{
					IList<Control> invalidControls = this.dxValidationProviderControl.GetInvalidControls();
					for (int i = invalidControls.Count - 1; i >= 0; i--)
					{
						Inventec.Common.Logging.LogSystem.Debug((i == 0 ? "InvalidControls:" : "") + "" + invalidControls[i].Name + ",");
					}
					valid = false;
				}

			}
			catch (Exception ex)
			{
				valid = false;
				Inventec.Common.Logging.LogSystem.Error(ex);
			}

			return valid;
		}

		public bool ValidateRequiredFieldCheckInfoBhyt()
		{
			bool valid = true;
			try
			{

				this.positionHandleControl = -1;
				if (!this.dxValidationProviderControl.Validate(txtPatientName))
				{
					valid = false;
				}
				if (!this.dxValidationProviderControl.Validate(txtPatientDob))
				{
					valid = false;
				}
				if (!this.dxValidationProviderControl.Validate(cboGender))
				{
					valid = false;
				}
			}
			catch (Exception ex)
			{
				valid = false;
				Inventec.Common.Logging.LogSystem.Error(ex);
			}

			return valid;
		}

		public void ResetRequiredField()
		{
			try
			{
				Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(this.dxValidationProviderControl, this.dxErrorProviderControl);

				//IList<Control> invalidControls = this.dxValidationProviderControl.GetInvalidControls();
				//for (int i = invalidControls.Count - 1; i >= 0; i--)
				//{
				//    this.dxValidationProviderControl.RemoveControlError(invalidControls[i]);
				//}
				//this.dxErrorProviderControl.ClearErrors();
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		#endregion

		#region Insid Validate

		private void ResetValidate()
		{
			try
			{
				dxValidationProviderControl.SetValidationRule(txtCareerCode, null);
				dxValidationProviderControl.SetValidationRule(cboGender, null);
				dxValidationProviderControl.SetValidationRule(txtPatientDob, null);
				dxValidationProviderControl.SetValidationRule(txtPatientName, null);
				dxValidationProviderControl.SetValidationRule(txtPatientTypeCode, null);
				dxValidationProviderControl.SetValidationRule(txtMilitaryRank, null);
				dxValidationProviderControl.SetValidationRule(txtPatientClassify, null);
				dxValidationProviderControl.SetValidationRule(txtPosition, null);
				dxValidationProviderControl.SetValidationRule(txtWorkPlace, null);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void ValidateControl()
		{
			try
			{
				if (HisConfigs.Get<string>("HIS.Desktop.Plugins.RegisterV2.IsNotCareerRequired") != "1")
				{
					lciMaNgheNghiep.AppearanceItemCaption.ForeColor = Color.Maroon;
					this.ValidateCareer();
				}
				this.ValidateGender();
				this.ValidatePatientDob();
				this.ValidatePatientName();
				this.ValidatePatientType();
				this.ValidatePatientClassify(txtPatientClassify);

				if (HIS.Desktop.Plugins.Library.RegisterConfig.AppConfigs.ChangeEthnic != 0)
				{
					this.IsValidateCombo(HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.IsValidate__Ethnic);
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void ValidateNewControl(DevExpress.XtraLayout.LayoutControlItem layout, DevExpress.XtraEditors.ButtonEdit control, bool isValidate)
		{
			try
			{
				if (isValidate == true)
				{
					layout.AppearanceItemCaption.ForeColor = Color.Maroon;
					this.ValidatePatientClassify(control);
				}
				else
				{
					layout.AppearanceItemCaption.ForeColor = Color.Black;
					dxValidationProviderControl.SetValidationRule(control, null);
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void ValidatePatientClassify(DevExpress.XtraEditors.ButtonEdit control)
		{
			try
			{
				Valid_Txt_Control validate = new Valid_Txt_Control();
				validate.txt = control;
				validate.ErrorText = MessageUtil.GetMessage(His.UC.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
				validate.ErrorType = ErrorType.Warning;
				this.dxValidationProviderControl.SetValidationRule(control, validate);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		public void IsValidateCombo(bool _isValidate)
		{
			try
			{
				if (_isValidate == true)
				{
					lciFortxtEthnicCode.AppearanceItemCaption.ForeColor = Color.Maroon;
					ValidateEthnic();
				}
				else
				{
					lciFortxtEthnicCode.AppearanceItemCaption.ForeColor = Color.Black;
					ResetRequiredField();
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void ValidateEthnic()
		{
			Valid_Ethnic_Control validateProvince = new Valid_Ethnic_Control();
			validateProvince.cboEthnic = this.cboEthnic;
			validateProvince.txtEthnic = this.txtEthnicCode;
			validateProvince.ErrorText = MessageUtil.GetMessage(His.UC.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
			validateProvince.ErrorType = ErrorType.Warning;
			this.dxValidationProviderControl.SetValidationRule(txtEthnicCode, validateProvince);
		}

		private void ValidateCareer()
		{
			Valid_Career_Control icdMainRule = new Valid_Career_Control();
			icdMainRule.cboCareer = this.cboCareer;
			icdMainRule.txtCareer = this.txtCareerCode;
			icdMainRule.ErrorText = MessageUtil.GetMessage(His.UC.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
			icdMainRule.ErrorType = ErrorType.Warning;
			dxValidationProviderControl.SetValidationRule(txtCareerCode, icdMainRule);
		}

		private void ValidateGender()
		{
			Valid_GenderCode_Control icdMainRule = new Valid_GenderCode_Control();
			icdMainRule.cboGenderCode = this.cboGender;
			//icdMainRule.txtGenderCode = this.txtGenderCode;
			icdMainRule.ErrorText = MessageUtil.GetMessage(His.UC.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
			icdMainRule.ErrorType = ErrorType.Warning;
			dxValidationProviderControl.SetValidationRule(cboGender, icdMainRule);
		}

		private void ValidatePatientDob()
		{
			Valid_PatientDob_Control icdMainRule = new Valid_PatientDob_Control();
			icdMainRule.dtPatientDob = this.dtPatientDob;
			icdMainRule.txtPatientDob = this.txtPatientDob;
			icdMainRule.ErrorText = MessageUtil.GetMessage(His.UC.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
			icdMainRule.ErrorType = ErrorType.Warning;
			dxValidationProviderControl.SetValidationRule(txtPatientDob, icdMainRule);
		}

		private void ValidatePatientName()
		{
			Valid_PatientName_Control icdMainRule = new Valid_PatientName_Control();
			icdMainRule.txtPatientName = this.txtPatientName;
			icdMainRule.ErrorType = ErrorType.Warning;
			dxValidationProviderControl.SetValidationRule(txtPatientName, icdMainRule);
		}

		private void ValidatePatientType()
		{
			Valid_PatientType_Control icdMainRule = new Valid_PatientType_Control();
			icdMainRule.cboPatientType = this.cboPatientType;
			icdMainRule.txtPatientType = this.txtPatientTypeCode;
			icdMainRule.TD3 = this.TD3;
			icdMainRule.ErrorText = MessageUtil.GetMessage(His.UC.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
			icdMainRule.ErrorType = ErrorType.Warning;
			dxValidationProviderControl.SetValidationRule(txtPatientTypeCode, icdMainRule);
		}

		private void ValidatePrimaryPatientType()
		{
			Valid_PatientType_Control icdMainRule = new Valid_PatientType_Control();
			icdMainRule.cboPatientType = this.cboPrimaryPatientType;
			icdMainRule.txtPatientType = this.txtPrimaryPatientTypeCode;
			icdMainRule.ErrorText = MessageUtil.GetMessage(His.UC.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
			icdMainRule.ErrorType = ErrorType.Warning;
			dxValidationProviderControl.SetValidationRule(txtPrimaryPatientTypeCode, icdMainRule);
		}

		private void ValidateTextAge()
		{
			try
			{
				Valid_Text_Age_Control _Rule = new Valid_Text_Age_Control();
				_Rule.txtAge = this.txtAge;
				_Rule.ErrorText = MessageUtil.GetMessage(His.UC.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
				_Rule.ErrorType = ErrorType.Warning;
				dxValidationProviderControl.SetValidationRule(this.txtAge, _Rule);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		#endregion
	}
}
