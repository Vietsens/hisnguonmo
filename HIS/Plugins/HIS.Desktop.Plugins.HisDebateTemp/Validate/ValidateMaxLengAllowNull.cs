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
using Inventec.Common.Logging;

namespace HIS.Desktop.Plugins.HisDebateTemp.Validate
{
    class ValidateMaxLengAllowNull :DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.TextEdit txtcontrol;
        internal int? Maxlangth;
        public override bool Validate(System.Windows.Forms.Control control, object value)
        {
            bool vali = true;
            try
            {
                vali = vali && (txtcontrol != null);
                if (vali)
                {
                    string strError = "";
                    string LengthtxtControl = txtcontrol.Text.Trim();
                    int? CountLength = Inventec.Common.String.CountVi.Count(LengthtxtControl);
                    if (CountLength > Maxlangth)
                    {
                        vali = false;
                        strError += ((!string.IsNullOrEmpty(LengthtxtControl) ? "\r\n" : "") + String.Format("Trường dữ liệu vượt quá giới hạn.", Maxlangth));
                    }
                    this.ErrorText = strError;
                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
            return vali;
        }
    }
}
