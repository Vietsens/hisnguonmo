using DevExpress.XtraEditors.DXErrorProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.Exemptions
{
    public class ControlMaxCharLengthValidationRule : ValidationRule
    {
        public Control Editor { get; set; }
        public int MaxLength { get; set; }
        public bool IsRequired { get; set; }

        public override bool Validate(Control control, object value)
        {
            string text = control.Text ?? string.Empty;

            int byteLength = Encoding.UTF8.GetByteCount(text);

            if (byteLength > MaxLength)
            {
                ErrorText = "Không được nhập quá "+MaxLength+" ký tự";
                return false;
            }

            return true;
        }
    }
}
