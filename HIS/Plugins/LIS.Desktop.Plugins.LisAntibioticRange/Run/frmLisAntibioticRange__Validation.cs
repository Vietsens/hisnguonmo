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
using LIS.Desktop.Plugins.LisAntibioticRange.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LIS.Desktop.Plugins.LisAntibioticRange.Run
{
	public partial class frmLisAntibioticRange
	{

		private void ValidateForm()
		{
			try
			{
				ValidateBacterium();
				ValidateAntibiotic();
				SetMaxLengthMin();
				SetMaxLengthMax();
				SetMaxLengthSri();
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void SetMaxLengthMin()
		{
			try
			{
				ValidateMaxLength valid = new ValidateMaxLength();
				valid.textEdit = txtMinValue;
				valid.maxLength = 100;
				this.dxValidationProvider1.SetValidationRule(txtMinValue, valid);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void SetMaxLengthSri()
		{
			try
			{
				ValidateMaxLength valid = new ValidateMaxLength();
				valid.textEdit = txtSriValue;
				valid.maxLength = 100;
				this.dxValidationProvider1.SetValidationRule(txtSriValue, valid);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void SetMaxLengthMax()
		{
			try
			{
				ValidateMaxLength valid = new ValidateMaxLength();
				valid.textEdit = txtMaxValue;
				valid.maxLength = 100;
				this.dxValidationProvider1.SetValidationRule(txtMaxValue, valid);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void ValidateBacterium()
		{
			try
			{
				ValidationControls valid = new ValidationControls();
				valid.cbo = this.cboBacterium;
				valid.txt = this.txtBacterium;
				valid.ErrorText = "Trường dữ liệu bắt buộc";
				valid.ErrorType = ErrorType.Warning;
				this.dxValidationProvider1.SetValidationRule(txtBacterium, valid);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void ValidateAntibiotic()
		{
			try
			{
				ValidationControls valid = new ValidationControls();
				valid.cbo = this.cboAntibiotic;
				valid.txt = this.txtActibiotic;
				valid.ErrorText = "Trường dữ liệu bắt buộc";
				valid.ErrorType = ErrorType.Warning;
				this.dxValidationProvider1.SetValidationRule(txtActibiotic, valid);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}
	}
}
