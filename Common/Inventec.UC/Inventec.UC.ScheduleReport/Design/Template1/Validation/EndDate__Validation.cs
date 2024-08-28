using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ScheduleReport.Design.Template1.Validation
{
    class EndDate__Validation : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.DateEdit dtEndTime;

        public override bool Validate(System.Windows.Forms.Control control, object value)
        {
            bool valid = true;
            try
            {
                valid = valid && (dtEndTime != null);
                if (valid)
                {
                    valid = valid && (dtEndTime.DateTime != null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return valid;
        }
    }
}
