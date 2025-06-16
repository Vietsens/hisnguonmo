using DevExpress.XtraEditors.DXErrorProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisPatientBankAccount.Validate
{
    class ValidControlTextEdit : ValidationRule
    {
        internal DevExpress.XtraEditors.TextEdit txtEdit;

        public override bool Validate(System.Windows.Forms.Control control, object value)
        {
            bool valid = false;
            try
            {
                
                if (Inventec.Common.String.CountVi.Count(txtEdit.Text) > 100)
                {
                    this.ErrorText = "Độ dài mã vượt quá 100 ký tự";
                    return valid;
                }
                valid = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }
    }
}
