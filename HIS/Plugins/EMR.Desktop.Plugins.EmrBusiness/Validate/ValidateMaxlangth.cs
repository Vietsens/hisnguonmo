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

namespace EMR.Desktop.Plugins.EmrBusiness.Validate
{
    class ValidateMaxlangth:DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.TextEdit txtControl;
        internal string message;
        internal int? Maxlength;
        public override bool Validate(System.Windows.Forms.Control control, object value)
        {
            bool vali = true;
            try
            {
                vali = vali && (txtControl != null);
                if (vali)
                {
                    string strError = "";
                    string Lengthtxt= txtControl.Text.Trim();
                    int? CountLength = Inventec.Common.String.CountVi.Count(Lengthtxt);
                   
                    if (String.IsNullOrEmpty(Lengthtxt))
                    {
                        vali = false;
                        strError = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);

                    }
                    else
                    {
                        if (CountLength > Maxlength)
                        {
                            vali = false;
                            strError += "Vượt quá ký tự cho phép " + Maxlength;
                        }
                    }
                    this.ErrorText = strError;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return vali;
        }
    }
}
