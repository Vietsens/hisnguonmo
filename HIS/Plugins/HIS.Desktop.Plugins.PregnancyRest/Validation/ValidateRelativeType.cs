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
using DevExpress.XtraEditors.DXErrorProvider;

namespace HIS.Desktop.Plugins.PregnancyRest.Validation
{
    class ValidateRelativeType : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.GridLookUpEdit txtControl;
        internal DevExpress.XtraEditors.GridLookUpEdit cbo;
        internal long age;
        public override bool Validate(System.Windows.Forms.Control control, object value)
        {
            bool success = false;
            try
            {
                if (cbo.EditValue != null)
                {
                    long TreatmentEndTypeExtId = Inventec.Common.TypeConvert.Parse.ToInt64(cbo.EditValue.ToString() ?? "");
                    if (age < 7 && TreatmentEndTypeExtId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE_EXT.ID__NGHI_OM && txtControl.EditValue==null)
                    {
                        this.ErrorText = "Trường dữ liệu bắt buộc";
                        this.ErrorType = ErrorType.Warning;
                        return success;
                    }
                }
                success = true;
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return success;
        }
    }
}
