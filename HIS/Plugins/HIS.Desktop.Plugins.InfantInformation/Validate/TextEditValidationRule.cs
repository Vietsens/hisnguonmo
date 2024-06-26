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
    class TextEditValidationRule : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.SpinEdit spin;

        public override bool Validate(Control control, object value)
        {
            bool valid = false;
            try
            {
                if (spin == null)
                    return valid;
           
                valid = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return valid;
        }
    }

    //class TextEditValidationRule1 : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    //{
    //    internal DevExpress.XtraEditors.TextEdit txtPatientsothang;
    //    int max = 9;
    //    public TextEditValidationRule1()
    //    {
    //    }
    //    private void textEdit1_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
    //    {
    //        if (e.NewValue != null)
    //            if (e.NewValue.ToString().Length > 50)
    //                e.Cancel = true;
    //    }
    //}



    //    public override bool Validate(Control control, object value)
    //    {
    //        bool valid = false;
    //        try
    //        {
    //            if (String.IsNullOrEmpty(txtPatientsothang.Text))
    //                return valid;
    //            int number = 0;
    //            int.TryParse(txtPatientsothang.Text, out number);
    //            if (number > max) return false;
    //            valid = true;
    //        }
    //        catch (Exception ex)
    //        {
    //            Inventec.Common.Logging.LogSystem.Error(ex);
    //        }
    //        return valid;
    //    }
    //}
