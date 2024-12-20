using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.UC.FormType.HourFromTo.Validation
{
    class ToHourValidationRule : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.TimeEdit dtToHour;
        internal DevExpress.XtraEditors.TimeEdit dtFromHour;
        public override bool Validate(System.Windows.Forms.Control control, object value)
        {
            bool valid = false;
            try
            {
                if (dtToHour == null) return valid;
                if (dtToHour.EditValue == null) return valid;
                if (dtFromHour.EditValue != null)
                {
                    if (dtFromHour.Time.TimeOfDay > dtToHour.Time.TimeOfDay)
                    {
                        return valid;
                    }
                }

                valid = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                valid = false;
            }
            return valid;
        }
    }
}
