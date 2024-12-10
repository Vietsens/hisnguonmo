using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.FormType.Core.F37.Validation
{
    class F37ChildValidationRule : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.GridLookUpEdit cboService;

        public override bool Validate(System.Windows.Forms.Control control, object value)
        {
            bool result = false;
            try
            {
                if (cboService == null) return result;
                if (cboService.EditValue == null) return result;
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
