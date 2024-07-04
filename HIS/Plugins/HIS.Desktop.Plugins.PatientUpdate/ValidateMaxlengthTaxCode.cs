using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.PatientUpdate
{
    public class ValidateMaxlengthTaxCode : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.TextEdit txt;
        internal int maxLength;
        public override bool Validate(System.Windows.Forms.Control control, object value)
        {
            bool valid = false;
            try
            {
                if (txt == null) return valid;
                if (!string.IsNullOrEmpty(txt.Text) && Inventec.Common.String.CheckString.IsOverMaxLengthUTF8(txt.Text, maxLength))
                {
                    this.ErrorText = String.Format("Trường dữ liệu vượt quá ký tự cho phép ({0} ký tự)", maxLength);
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
