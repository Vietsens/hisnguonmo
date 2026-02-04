using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisHolidayPolicies.Validation
{
    class ValidationTimeToGreaterThanFrom : ValidationRule
    {
        internal TimeEdit dtTimeFrom;
        internal TimeEdit dtTimeTo;

        public override bool Validate(System.Windows.Forms.Control control, object value)
        {
            try
            {
                if (!(dtTimeFrom?.EditValue is DateTime from) || !(dtTimeTo?.EditValue is DateTime to))
                    return true;

                if (to.TimeOfDay < from.TimeOfDay)
                {
                    this.ErrorText = "Thời gian đến phải lớn hơn thời gian bắt đầu.";
                    this.ErrorType = ErrorType.Warning;
                    return false;
                }

                return true;
            }
            catch
            {
                return true;
            }
        }
    }
}
