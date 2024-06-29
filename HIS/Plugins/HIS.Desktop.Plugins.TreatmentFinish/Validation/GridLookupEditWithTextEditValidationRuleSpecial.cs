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
using Inventec.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.TreatmentFinish.Validation
{
    public class GridLookupEditWithTextEditValidationRuleSpecial : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        public TextEdit txtTextEdit;

        public GridLookUpEdit cbo;

        public override bool Validate(Control control, object value)
        {
            bool result = false;
            try
            {
                if (txtTextEdit == null || cbo == null)
                {
                    return result;
                }

                if (string.IsNullOrEmpty(((Control)(object)txtTextEdit).Text.Trim()) && (cbo.EditValue == null || cbo.EditValue == ""))
                {
                    return result;
                }

                result = true;
                return result;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                return result;
            }
        }
    }
}
