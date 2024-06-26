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
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.EmpUser
{

    public class ValidateMaxLengthList : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal int count = 0;
        internal object listString = null;
        public override bool Validate(System.Windows.Forms.Control control, object value)
        {
            bool valid = false;
            try
            {
                if (listString != null)
                {
                    string Strcount = null;
                    if (listString is List<HIS_SPECIALITY>)
                    {
                        Strcount = string.Join(";", (listString as List<HIS_SPECIALITY>).Select(o => o.SPECIALITY_CODE).ToList());
                    }
                    else if (listString is List<HIS_MEDI_ORG>)
                    {
                        Strcount = string.Join(";", (listString as List<HIS_MEDI_ORG>).Select(o => o.MEDI_ORG_CODE).ToList());
                    }
                    if (!string.IsNullOrEmpty(Strcount))
                    {
                        var countStr = Strcount.Length;
                        if (countStr > count)
                        {
                            this.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                            this.ErrorText = "Không được nhập quá " + count + " kí tự";
                            return valid;
                        }
                    }
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
