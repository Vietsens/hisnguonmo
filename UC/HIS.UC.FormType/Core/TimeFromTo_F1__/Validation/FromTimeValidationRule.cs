using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.UC.FormType.TimeFromTo.Validation
{
    class FromTimeValidationRule : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.DateEdit dtToTime;
        internal DevExpress.XtraEditors.DateEdit dtFromTime;
        public override bool Validate(System.Windows.Forms.Control control, object value)
        {
            bool valid = false;
            try
            {
                if (dtFromTime == null) return valid;
                if (dtFromTime.EditValue == null || dtFromTime.DateTime == System.DateTime.MinValue) return valid;
                if (dtFromTime.EditValue != null && dtFromTime.DateTime != System.DateTime.MinValue)
                {
                    if (dtToTime != null && dtToTime.EditValue != null && dtToTime.DateTime != System.DateTime.MinValue && dtFromTime.DateTime > dtToTime.DateTime)
                    {
                        this.ErrorText = "Thời gian không hợp lệ.";
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
