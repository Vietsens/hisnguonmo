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
using HIS.Desktop.Plugins.HisServiceCondition.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisServiceCondition.Validations
{
    class ValidationSpinHeinPrice : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.SpinEdit spnNumber;

        internal string Errtext = "";
        public override bool Validate(System.Windows.Forms.Control control, object value)
        {
            bool valid = false;
            try
            {
                string stringSpinValue = spnNumber.Value.ToString();
                if ((float)spnNumber.Value < 0)
                {
                    ErrorText = ResourceMessage.GiaTriPhaiLonHon0;
                    Errtext = ResourceMessage.GiaTriPhaiLonHon0;
                    return valid;

                }
                //if ((long)spnNumberLimitDemacialNo.Value < 0) return valid;
                //if ((long)spnNumberLimitDemacialNo.Value > 100) return valid;
                //if (string.IsNullOrEmpty(spnNumberLimitValue.Text)) return valid;
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
