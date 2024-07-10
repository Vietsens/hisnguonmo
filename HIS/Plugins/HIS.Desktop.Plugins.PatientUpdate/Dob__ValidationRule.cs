using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.PatientUpdate
{
    class Dob__ValidationRule : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.DateEdit txtPeopleDob;
        public override bool Validate(Control control, object value)
        {
            bool valid = false;
            try
            {
                if (txtPeopleDob == null) return valid;
                DateTime? dt = (DateTime)txtPeopleDob.DateTime;
                if (dt.Value > DateTime.Now)
                {
                    ErrorText = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.NguoiDungNhapNgayPhaiNhoHonNgayHienTai);
                    ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
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
