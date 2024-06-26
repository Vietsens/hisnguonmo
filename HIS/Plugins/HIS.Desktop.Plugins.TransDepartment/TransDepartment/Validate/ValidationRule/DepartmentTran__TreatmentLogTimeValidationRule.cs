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
using HIS.Desktop.LibraryMessage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.TransDepartment.Validate.ValidationRule
{
    class DepartmentTran__TreatmentLogTimeValidationRule : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        public DepartmentTran__TreatmentLogTimeValidationRule(long? _departmentInTime)
        {
            this.departmentInTime = _departmentInTime;
        }

        public DepartmentTran__TreatmentLogTimeValidationRule()
        {
        }

        internal DevExpress.XtraEditors.DateEdit dtLogTime;
        internal long? departmentInTime;

        public override bool Validate(System.Windows.Forms.Control control, object value)
        {
            bool valid = false;
            try
            {
                if (dtLogTime.EditValue == null)
                {
                    this.ErrorText = MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                    return valid;
                }

                if (dtLogTime.EditValue != null && departmentInTime!=null)
                {
                    if (Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtLogTime.DateTime) < departmentInTime)
                    {
                        this.ErrorText = "Thời gian vào khoa mới phải lớn hơn thời gian rời khoa trước";
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
