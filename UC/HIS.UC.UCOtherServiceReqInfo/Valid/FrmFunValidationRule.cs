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

namespace HIS.UC.UCOtherServiceReqInfo.Valid
{
    class FrmFunValidationRule : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.GridLookUpEdit cboCCT;
        internal UCOtherServiceReqInfo frm;
        public override bool Validate(Control control, object value)
        {
            bool valid = false;
            try
            {
                if (cboCCT != null && cboCCT.EditValue != null)
                {
                    MOS.EFMODEL.DataModels.HIS_TREATMENT _hisTreatment = frm != null ? frm._HisTreatment : null;
                    if (_hisTreatment == null) return valid;
                    if (String.IsNullOrEmpty(_hisTreatment.FUND_NUMBER))
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
