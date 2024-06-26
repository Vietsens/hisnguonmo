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

namespace EMR.Desktop.Plugins.EmrFlow.Validate
{
    class ValidateAllowNullMaxLength:DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.TextEdit txtControl;
        internal int? Maxlangth;
        public override bool Validate(System.Windows.Forms.Control control, object value)
        {
            bool Vali = true;
            try
            {
                Vali = Vali && (txtControl!=null);
                if (Vali)
                {
                    string strError = "";
                    string LenthtxtComtrol = txtControl.Text.Trim();
                    int? CoutLength = Inventec.Common.String.CountVi.Count(LenthtxtComtrol);
                    if (CoutLength > Maxlangth)
                    {
                        Vali=false;
                        strError +=((!string.IsNullOrEmpty(LenthtxtComtrol) ? "\r\n" :"")+ String.Format(ResourcesMassage.TruongDuLieuVuotQuaMaxLength,Maxlangth));
                    }
                    this.ErrorText=strError;
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return Vali;
        }
    }
}
