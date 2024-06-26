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
using System.Net.Mail;

namespace HIS.Desktop.Plugins.EmpUser
{
    class ValidateEmail : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.TextEdit txt;
        // kiem tra su ton tai cua email
        public override bool Validate(System.Windows.Forms.Control control,Object value)
        {
            bool valid = false;
            try
            {
                if (txt.Text == null || txt.Text=="")
                {
                    valid = true;
                    return valid;
                } 
                if (!String.IsNullOrEmpty(txt.Text))
                {
                    try
                    {
                        MailAddress mail = new MailAddress(txt.Text);

                    }
                    catch (FormatException)
                    {
                        return valid;
                    }
                    return true;
                }
            }
            catch (Exception e)
            { 
                Inventec.Common.Logging.LogSystem.Error(e);
            }
            return valid;
        }

    }
}
