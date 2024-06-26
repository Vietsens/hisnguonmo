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
using HIS.Desktop.Plugins.AntibioticRequest.Validate;
using Inventec.Common.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AntibioticRequest.Run
{
	public partial class frmAntibioticRequest
	{
		private void SetValidMaxLengthControl()
		{
			try
			{
				ValidMaxlengthControl(txtAllergy, 1000);
				ValidMaxlengthControl(memInfectionEntry, 1000);
				ValidMaxlengthControl(txtIcdSubCode,500);
				ValidMaxlengthControl(txtIcdText, 4000);
				ValidMaxlengthControl(memClinalCondition, 4000);
				ValidMaxlengthControl(txtWhiteBloodCell, 100);
				ValidMaxlengthControl(txtCrp, 100);
				ValidMaxlengthControl(txtPct, 100);
				ValidMaxlengthControl(memSubclinicalResult, 4000);
				//-
				
				//--

				//---
			}
			catch (Exception ex)
			{
				LogSystem.Warn(ex);
			}
		}
		private void ValidMaxlengthControl(BaseEdit txtEdit, int? maxLength)
		{
			try
			{
				ValidateMaxLength validateMaxLength = new ValidateMaxLength();
				validateMaxLength.textEdit = txtEdit;
				validateMaxLength.maxLength = maxLength;
				dxValidationProvider1.SetValidationRule(txtEdit, validateMaxLength);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}
	}
}
