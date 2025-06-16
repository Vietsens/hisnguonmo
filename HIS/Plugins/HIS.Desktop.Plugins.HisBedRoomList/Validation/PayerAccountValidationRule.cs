using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisBedRoomList.Validation
{
    class PayerAccountValidationRule : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.TextEdit txtPayerAccount;
        public override bool Validate(System.Windows.Forms.Control control, object value)
        {
            bool valid = false;
            try
            {
                if (txtPayerAccount == null) return valid;
                if (Inventec.Common.String.CountVi.Count(txtPayerAccount.Text) > 100)
                {
                    this.ErrorText = "Độ dài mã vượt quá " + 100;
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
