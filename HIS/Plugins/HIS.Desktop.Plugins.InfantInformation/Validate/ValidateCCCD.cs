using DevExpress.XtraEditors.DXErrorProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.InfantInformation.Validate
{
    class ValidateCCCD : ValidationRule
    {
        internal DevExpress.XtraEditors.BaseControl textEdit;
        public override bool Validate(Control control, object value)
        {
            bool valid = false;
            try
            {
                if (textEdit == null) return valid;
                
                string input = textEdit.Text.Trim();
                
                // Nếu trường bắt buộc và rỗng
                if (String.IsNullOrEmpty(input))
                {
                    this.ErrorText = "Trường dữ liệu bắt buộc";
                    this.ErrorType = ErrorType.Warning;
                    return valid;
                }
                
                // Nếu có dữ liệu
                if (!String.IsNullOrEmpty(input))
                {
                    int length = (int)Inventec.Common.String.CountVi.Count(input);
                    
                    // Kiểm tra độ dài bằng 9 (chỉ số)
                    if (length == 9)
                    {
                        if (!Regex.IsMatch(input, @"^\d{9}$"))
                        {
                            this.ErrorText = "CMND phải là 9 chữ số";
                            this.ErrorType = ErrorType.Warning;
                            return valid;
                        }
                    }
                    // Kiểm tra độ dài bằng 12 (chỉ số)
                    else if (length == 12)
                    {
                        if (!Regex.IsMatch(input, @"^\d{12}$"))
                        {
                            this.ErrorText = "CCCD phải là 12 chữ số";
                            this.ErrorType = ErrorType.Warning;
                            return valid;
                        }
                    }
                    // Kiểm tra độ dài nhỏ hơn 10 (cho phép cả chữ và số)
                    else if (length < 10)
                    {
                        // Cho phép cả chữ và số, không cần validate thêm
                        valid = true;
                    }
                    // Độ dài không hợp lệ (10, 11, hoặc > 12)
                    else
                    {
                        this.ErrorText = "Độ dài không hợp lệ. Cho phép: 9 số (CMND), 12 số (CCCD) hoặc dưới 10 ký tự";
                        this.ErrorType = ErrorType.Warning;
                        return valid;
                    }
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
