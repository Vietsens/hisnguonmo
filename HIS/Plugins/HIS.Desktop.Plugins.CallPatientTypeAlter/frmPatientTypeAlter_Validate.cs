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
using HIS.Desktop.Plugins.CallPatientTypeAlter.ValidationRule;
using Inventec.Desktop.Common.Controls.ValidationRule;
using System;

namespace HIS.Desktop.Plugins.CallPatientTypeAlter
{
    public partial class frmPatientTypeAlter : HIS.Desktop.Utility.FormBase
	{
		private void ValidControl()
		{
			try
			{
				ValidTreatmentType();
				ValidationSingleControl(dtLogTime);
				layoutControlItem9.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
				ValidationSingleControl(cboClassify);
				if (currentNameControl != null && currentNameControl.Count > 0)
				{
					string bnW = layoutControl1.Name + ".Root." + layoutControlItem9.Name;
					if (currentNameControl.Contains(bnW))
					{
						IsVisibleClassify = true;
						layoutControlItem9.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
						dxValidationProvider1.SetValidationRule(cboClassify, null);
						emptySpaceItem2.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void ValidationSingleControl(BaseEdit control)
		{
			try
			{
				ControlEditValidationRule validRule = new ControlEditValidationRule();
				validRule.editor = control;
				validRule.ErrorText = String.Format(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc));
				validRule.ErrorType = ErrorType.Warning;
				dxValidationProvider1.SetValidationRule(control, validRule);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void ValidTreatmentType()
		{
			try
			{
				TreatmentTypeValidationRule oDobDateRule = new TreatmentTypeValidationRule();
				oDobDateRule.txtTreatmentTypeCode = txtTreatmentTypeCode;
				oDobDateRule.cboTreatmentType = cboTreatmentType;
				oDobDateRule.ErrorText = HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.ThieuTruongDuLieuBatBuoc);
				oDobDateRule.ErrorType = ErrorType.Warning;
				this.dxValidationProvider1.SetValidationRule(txtTreatmentTypeCode, oDobDateRule);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void ValidWorkPlace()
		{
			try
			{
				WorkPlaceValidationRule oDobDateRule = new WorkPlaceValidationRule();
				oDobDateRule.txt = txtWorkplace;
				oDobDateRule.cbo = cboWorkPlace;
				oDobDateRule.ErrorText = HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.ThieuTruongDuLieuBatBuoc);
				oDobDateRule.ErrorType = ErrorType.Warning;
				this.dxValidationProvider1.SetValidationRule(cboWorkPlace, oDobDateRule);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void ValidPrimaryPatientTypeCode()
		{
			try
			{
				PrimaryPatientTypeCodeValidate prim = new PrimaryPatientTypeCodeValidate();
				prim.txtPrimaryPatientTypeCode = txtPrimaryPatientTypeCode;
				prim.cboPrimaryPatientTypeCode = cboPrimaryPatientType;
				prim.ErrorText = HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.ThieuTruongDuLieuBatBuoc);
				prim.ErrorType = ErrorType.Warning;
				this.dxValidationProvider1.SetValidationRule(txtPrimaryPatientTypeCode, prim);

			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void ValidDoiTuong()
		{
			try
			{
				PatientTypeValidationRule oDobDateRule = new PatientTypeValidationRule();
				oDobDateRule.txtMaDoiTuong = txtPatientType;
				oDobDateRule.cboDoiTuong = cboPatientType;
				oDobDateRule.ErrorText = HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.ThieuTruongDuLieuBatBuoc);
				oDobDateRule.ErrorType = ErrorType.Warning;
				this.dxValidationProvider1.SetValidationRule(txtPatientType, oDobDateRule);
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

				if (positionHandleControl == -1)
				{
					positionHandleControl = edit.TabIndex;
					if (edit.Visible)
					{
						edit.SelectAll();
						edit.Focus();
					}
				}
				if (positionHandleControl > edit.TabIndex)
				{
					positionHandleControl = edit.TabIndex;
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
	}
}
