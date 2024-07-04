using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.PatientUpdate
{
    public class ValidateMaxlength : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.MemoEdit mme;
        internal DevExpress.XtraEditors.TextEdit txt;
        internal int maxLength;
        internal int exactLength;
        public override bool Validate(System.Windows.Forms.Control control, object value)
        {
            bool valid = false;
            try
            {
                if (mme == null && txt == null) return valid;
                if (mme != null && Inventec.Common.String.CheckString.IsOverMaxLengthUTF8(mme.Text, maxLength))
                {
                    this.ErrorText = "Trường dữ liệu vượt quá ký tự cho phép";
                    return valid;
                }
                else if (txt != null && !string.IsNullOrEmpty(txt.Text) && txt.Text.Length < exactLength)
                {
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
