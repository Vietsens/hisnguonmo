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

namespace HIS.Desktop.Plugins.HisFilmSize.Validate
{
    class ValidateMaxLength:DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.TextEdit txtControl;
        internal int? MaxLength;
        internal string messageErro;
        public override bool Validate(Control control, object value)
        {
            bool valid = true;
            try
            {
                valid = valid && (txtControl != null);
                if (valid)
                {
                    string strErro = "";
                    string txtLength = txtControl.Text.Trim();
                    int? countLength = Inventec.Common.String.CountVi.Count(txtLength);
                    if (String.IsNullOrEmpty(txtLength))
                    {
                        valid = false;
                        strErro = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                    }
                    else
                    {
                        if (countLength > MaxLength)
                        {
                            valid = false;
                            strErro += ((!string.IsNullOrEmpty(strErro) ? "\r\n" : "") + string.Format(messageErro, MaxLength));
                        }
                    }
                    this.ErrorText = strErro;
                }

            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);

            }
            return valid;
        }
    }
}
