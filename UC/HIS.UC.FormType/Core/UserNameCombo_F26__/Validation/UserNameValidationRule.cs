using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.FormType.UserNameCombo.Validation
{
    class UserNameValidationRule : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {

        internal DevExpress.XtraEditors.GridLookUpEdit comboBoxEdit1;
        internal DevExpress.XtraEditors.TextEdit textEdit1;

        public override bool Validate(System.Windows.Forms.Control control, object value)
        {
            bool result = false;
            try
            {
                if (comboBoxEdit1 == null || textEdit1 == null) return result;
                if (comboBoxEdit1.EditValue == null || textEdit1.Text.Trim() == "") return result;
                result = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

    }
}
