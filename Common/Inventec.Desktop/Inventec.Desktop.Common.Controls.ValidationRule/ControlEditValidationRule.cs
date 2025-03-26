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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.Desktop.Common.Controls.ValidationRule
{
    public class ControlEditValidationRule : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        public object editor;
        public IsValidControl isValidControl;
        public GetMessageErrorValidControl getMessageErrorValidControl;
        public bool isUseOnlyCustomValidControl = false;

        public override bool Validate(System.Windows.Forms.Control control, object value)
        {
            bool valid = false;
            try
            {
                if (editor == null) return valid;

                if (getMessageErrorValidControl != null)
                    this.ErrorText = getMessageErrorValidControl();

                if (!isUseOnlyCustomValidControl)
                {
                    if (editor is DevExpress.XtraEditors.LookUpEdit)
                    {
                        if (((DevExpress.XtraEditors.LookUpEdit)editor).EditValue == null)
                            return valid;
                    }
                    else if (editor is DevExpress.XtraEditors.GridLookUpEdit)
                    {
                        if (((DevExpress.XtraEditors.GridLookUpEdit)editor).EditValue == null)
                            return valid;
                    }
                    else if (editor is DevExpress.XtraEditors.TextEdit)
                    {
                        if (String.IsNullOrEmpty(((DevExpress.XtraEditors.TextEdit)editor).Text.Trim()))
                            return valid;
                    }
                    else if (editor is DevExpress.XtraEditors.MemoEdit)
                    {
                        if ((String.IsNullOrEmpty(((DevExpress.XtraEditors.MemoEdit)editor).Text.Trim())))
                            return valid;
                    }
                    else if (editor is DevExpress.XtraEditors.ComboBoxEdit)
                    {
                        if (((DevExpress.XtraEditors.ComboBoxEdit)editor).EditValue == null
                            || Convert.ToInt64(((DevExpress.XtraEditors.ComboBoxEdit)editor).EditValue) == 0)
                            return valid;
                    }
                    else if (editor is DevExpress.XtraEditors.ButtonEdit)
                    {
                        if (((DevExpress.XtraEditors.ButtonEdit)editor).Text == null)
                            return valid;
                    }
                    else if (editor is DevExpress.XtraEditors.DateEdit)
                    {
                        if (((DevExpress.XtraEditors.DateEdit)editor).EditValue == null
                            || ((DevExpress.XtraEditors.DateEdit)editor).DateTime == DateTime.MinValue)
                            return valid;
                    }
                    else if (editor is DevExpress.XtraEditors.SpinEdit)
                    {
                        if (((DevExpress.XtraEditors.SpinEdit)editor).EditValue == null)
                            return valid;
                    }
                }

                if (isValidControl != null && !isValidControl())
                {
                    return valid;
                }
                valid = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return valid;
        }
    }
}
