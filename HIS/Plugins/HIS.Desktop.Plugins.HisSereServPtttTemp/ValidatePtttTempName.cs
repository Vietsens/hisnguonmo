using HIS.Desktop.LibraryMessage;
using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisSereServPtttTemp
{
    class ValidatePtttTempName : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.TextEdit txt;

        public override bool Validate(Control control, object value)
        {
            bool valid = false;
            try
            {
                string input = control.Text.Trim();

                if (string.IsNullOrEmpty(input))
                {
                    this.ErrorText = MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                    return false;
                }
                else if (input.Length > 50)
                {
                    this.ErrorText = "Trường dữ liệu phải nhỏ hơn hoặc bằng 50 ký tự";
                    return false;
                }
                else if (!Regex.IsMatch(input, @"^[\p{L}\p{N}\s]+$"))
                {
                    this.ErrorText = "Tên không được chứa ký tự đặc biệt";
                    return false;
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
