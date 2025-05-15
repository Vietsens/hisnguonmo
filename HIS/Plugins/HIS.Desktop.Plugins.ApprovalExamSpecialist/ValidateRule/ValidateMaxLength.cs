using DevExpress.XtraEditors.DXErrorProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ApprovalExamSpecialist.ValidateRule
{
    public class ValidateMaxLength : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        public DevExpress.XtraEditors.BaseEdit textEdit;
        public int? maxLength;
        public bool isValidNull;

        public override bool Validate(Control control, object value)
        {
            bool valid = false;
            try
            {
                if (textEdit == null) return valid;
                if (!String.IsNullOrEmpty(textEdit.Text) && Encoding.UTF8.GetByteCount(textEdit.Text) > maxLength)
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
