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
using Inventec.Common.Logging;
using Inventec.Common.String;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LIS.Desktop.Plugins.LisAntibioticRange.Validation
{
	public class ValidateMaxLength : ValidationRule
	{
		internal BaseEdit textEdit;

		internal int? maxLength;

		public override bool Validate(Control control, object value)
		{
			bool result = false;
			try
			{
				if (textEdit == null)
				{
					return result;
				}
				if (maxLength.HasValue && !string.IsNullOrEmpty(textEdit.Text) && CountVi.Count(textEdit.Text) > maxLength)
				{
					int? num = maxLength;
					base.ErrorText = "Vượt quá độ dài cho phép " + num + " ký tự!";
					base.ErrorType = ErrorType.Warning;
					return result;
				}
				result = true;
			}
			catch (Exception ex)
			{
				LogSystem.Error(ex);
			}
			return result;
		}
	}
}
