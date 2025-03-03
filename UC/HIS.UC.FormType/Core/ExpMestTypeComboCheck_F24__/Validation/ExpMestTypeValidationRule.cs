using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.FormType.ExpMestTypeComboCheck.Validation
{
    class ExpMestTypeValidationRule : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.GridLookUpEdit cboExpMestType;
        internal DevExpress.XtraEditors.TextEdit txtExpMestTypeCode;

        public override bool Validate(System.Windows.Forms.Control control, object value)
        {
            bool result = false;
            try
            {
                if (cboExpMestType == null || txtExpMestTypeCode == null) return result;
                if (cboExpMestType.EditValue == null || txtExpMestTypeCode.Text.Trim() == "") return result;
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
