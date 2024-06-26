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
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.Library.TreatmentEndTypeExt.Validation
{
    class ValidateAgeTxt : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {


        internal HIS_TREATMENT histreatment;
 
        internal DevExpress.XtraEditors.TextEdit text;
        public override bool Validate(Control control, object value)
        {
            bool valid = false;
            try
            {

                if (string.IsNullOrEmpty(text.Text) )
                {
                    if (histreatment != null)
                    {
                        if (histreatment.TDL_PATIENT_DOB != null)
                        {

                            if (Inventec.Common.DateTime.Calculation.Age(histreatment.TDL_PATIENT_DOB) < 7)
                            {

                                this.ErrorText = " Bệnh nhân là trẻ em, bắt buộc nhập thông tin người thân và quan hệ";
                                return valid;

                            }
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
