using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using HIS.Desktop.LibraryMessage;
//using HIS.Desktop.LibraryMessage;
using Inventec.Common.Logging;
using System;
using System.Windows.Forms;
namespace HIS.Desktop.Plugins.HisHtu
{
    public class ValidateHtu : ValidationRule
    {
        internal TextEdit txtHtuCode;
        internal bool IsExist;
        public override bool Validate(Control control, object value)
        {
            bool flag = false;
            bool result;
            try
            {
                bool flag2 = this.txtHtuCode == null;
                if (flag2)
                {
                    result = flag;
                    return result;
                }
                bool flag3 = string.IsNullOrEmpty(this.txtHtuCode.Text.Trim());
                if (flag3)
                {
                    base.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                    base.ErrorType = ErrorType.Warning;
                    result = flag;
                    return result;
                }
                bool isExist = this.IsExist;
                if (isExist)
                {
                    base.ErrorText = "Mã đã tồn tại trên hệ thống";
                    base.ErrorType = ErrorType.Warning;
                    result = flag;
                    return result;
                }
                flag = true;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            result = flag;
            return result;
        }
    }
}
