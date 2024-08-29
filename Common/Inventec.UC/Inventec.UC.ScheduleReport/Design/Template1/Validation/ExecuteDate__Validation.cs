using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ScheduleReport.Design.Template1.Validation
{
    class ExecuteDate__Validation : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.DateEdit dtExecuteTime;

        public override bool Validate(System.Windows.Forms.Control control, object value)
        {
            bool valid = true;
            try
            {
                valid = valid && (dtExecuteTime != null);
                if (valid)
                {
                    var date = dtExecuteTime.DateTime;
                    valid = valid && (date != DateTime.MinValue);
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
