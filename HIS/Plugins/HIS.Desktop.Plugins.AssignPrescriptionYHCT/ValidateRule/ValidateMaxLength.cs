using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors.DXErrorProvider;

namespace HIS.Desktop.Plugins.AssignPrescriptionYHCT.ValidateRule
{
    public class ValidateMaxLength : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.BaseEdit textEdit;
        internal int? maxLength;
        internal bool IsRequired;
        public override bool Validate(Control control, object value)
        {
            bool valid = false;
            try
            {
                if (textEdit == null) return valid;
                if (IsRequired && String.IsNullOrEmpty(textEdit.Text.Trim()))
                {
                    this.ErrorText = "Trường dữ liệu bắt buộc";
                    this.ErrorType = ErrorType.Warning;
                    return valid;
                }
                if (!String.IsNullOrEmpty(textEdit.Text.Trim()) && Encoding.UTF8.GetByteCount(textEdit.Text.Trim()) > maxLength)
                {
                    this.ErrorText = "Vượt quá độ dài cho phép (" + maxLength + ")";
                    this.ErrorType = ErrorType.Warning;
                    return valid;
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
