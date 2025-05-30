using DevExpress.XtraEditors.DXErrorProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisHolidayPolicies.Validation
{
    class ValidationControlTextEditName : ValidationRule
    {
        internal DevExpress.XtraEditors.TextEdit txtTen;

        public override bool Validate(System.Windows.Forms.Control control, object value)
        {
            bool valid = false;
            try
            {
                if (Inventec.Common.String.CountVi.Count(txtTen.Text) > 2000)
                {
                    this.ErrorText = "Độ dài tên vượt quá 2000 ký tự";
                    return valid;
                }
                valid = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }
    }
}
