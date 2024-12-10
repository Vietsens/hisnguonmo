using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.FormType.MaterialTypeCombo.Validation
{
    class MediStockValidationRule : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {

        internal DevExpress.XtraEditors.GridLookUpEdit comboBoxEdit3;
        internal DevExpress.XtraEditors.TextEdit textEdit3;

        public override bool Validate(System.Windows.Forms.Control control, object value)
        {
            bool result = false;
            try
            {
                if (comboBoxEdit3 == null || textEdit3 == null) return result;
                if (comboBoxEdit3.EditValue == null || textEdit3.Text.Trim() == "") return result;
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
