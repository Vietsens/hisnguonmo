using System;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ApprovaleDebate.ApprovaleDebate
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
                    if (isValidNull && (textEdit == null || string.IsNullOrEmpty(textEdit.Text)))
                    {
                        this.ErrorText = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                        return valid;
                    }

                    if (!String.IsNullOrEmpty(textEdit.Text) && Encoding.UTF8.GetByteCount(textEdit.Text) > maxLength)
                    {
                        this.ErrorText = "Vượt quá độ dài cho phép (" + maxLength + ")";
                        //this.ErrorType = ErrorType.Warning;
                    //this.ErrorType = WarningException; ;
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