using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.FormType.MaterialTypeCombo.Validation
{
    class DepartmentValidationRule : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {

        internal DevExpress.XtraEditors.GridLookUpEdit comboBoxEdit2;
        internal DevExpress.XtraEditors.TextEdit textEdit2;

        public override bool Validate(System.Windows.Forms.Control control, object value)
        {
            bool result = false;
            try
            {
                if (comboBoxEdit2 == null || textEdit2 == null) return result;
                if (comboBoxEdit2.EditValue == null || textEdit2.Text.Trim() == "") return result;
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
