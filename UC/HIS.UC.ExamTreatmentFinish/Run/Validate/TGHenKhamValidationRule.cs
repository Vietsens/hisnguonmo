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

namespace HIS.UC.ExamTreatmentFinish.Run.Validate
{
    class TGHenKhamValidationRule : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.DateEdit dtTGHenKham;
        internal DevExpress.XtraEditors.DateEdit dtTGRaVien;
        internal DevExpress.XtraEditors.GridLookUpEdit cboTET;
        public override bool Validate(System.Windows.Forms.Control control, object value)
        {
            bool valid = false;
            try
            {
                if (dtTGHenKham == null || dtTGRaVien == null) return valid;

                if (dtTGHenKham.Enabled)
                {
                    if (dtTGHenKham.EditValue != null && dtTGRaVien.EditValue != null && dtTGHenKham.DateTime < dtTGRaVien.DateTime)
                    {
                        this.ErrorText = "Thời gian hẹn khám không được nhỏ hơn thời gian ra viện";
                        return valid;
                    }

                    if (cboTET.EditValue != null && Inventec.Common.TypeConvert.Parse.ToInt64(cboTET.EditValue.ToString()) == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__HEN && dtTGHenKham.EditValue == null)
                    {
                        this.ErrorText = "Loại ra viện là hẹn khám bắt buộc nhập thời gian hẹn khám";
                        return valid;
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
