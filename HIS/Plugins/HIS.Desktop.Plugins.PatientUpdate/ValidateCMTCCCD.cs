using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.LibraryMessage;
using System.Text.RegularExpressions;
namespace HIS.Desktop.Plugins.PatientUpdate
{
    class ValidateCMTCCCD : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.TextEdit txtCmndNumber;

        public override bool Validate(Control control, object value)
        {
            bool valid = false;
            try
            {
                if (txtCmndNumber == null) return valid;
                if (!String.IsNullOrWhiteSpace(txtCmndNumber.Text))
                {
                    Int64 k;
                    bool isNumeric = Int64.TryParse(txtCmndNumber.Text, out k);
                    if (isNumeric == false)
                    {
                        string cmndNumber = txtCmndNumber.Text.Trim();
                        if (cmndNumber.Length >9 )
                        {
                            ErrorText = "Hộ chiếu không được vượt quá 9 ký tự";
                            ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                            return valid;
                        }
                        
                        if (Regex.IsMatch(txtCmndNumber.Text, @"^[\p{L}]+$"))
                        {
                            ErrorText = "CMND/CCCD/Hộ chiếu không đúng định dạng";
                            ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                            return valid;
                        }
                    }
                    else
                    {
                        string cmndNumber = txtCmndNumber.Text.Trim();
                        if (cmndNumber.Length > 9 &&  cmndNumber.Length < 12)
                        {
                            ErrorText = "CMND không được vượt quá 9 ký tự";
                            ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                            return valid;
                        }
                        if (cmndNumber.Length > 12 )
                        {
                            ErrorText = "CCCD không được vượt quá 12 ký tự";
                            ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                            return valid;
                        }
                        if (cmndNumber.Length < 9)
                        {
                            ErrorText = "CMNN không được nhỏ hơn 9 kí tự";
                            ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                            return valid;
                        }
                    }
                    



                    //////////////////////////
                    //if (Inventec.Common.String.CountVi.Count(txtCmndNumber.Text.Trim()) == 12)
                    //{
                    //    Int64 k;
                    //    bool isNumeric = Int64.TryParse(txtCmndNumber.Text, out k);
                    //    if (isNumeric == false)
                    //    {
                    //        ErrorText = "Trường CCCD chỉ cho phép nhập 12 ký tự số. Vui lòng nhập lại!";
                    //        ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                    //        return valid;
                    //    }
                    //}

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
