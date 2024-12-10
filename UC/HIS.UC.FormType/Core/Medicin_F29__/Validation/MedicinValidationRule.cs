using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.FormType.Medicin.Validation
{
    class MedicinValidationRule : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        //internal DevExpress.XtraEditors.GridLookUpEdit cboTreatmentType;
        internal DevExpress.XtraEditors.TextEdit Medicin;

        public override bool Validate(System.Windows.Forms.Control control, object value)
        {
            bool result = false;
            try
            {
                if (Medicin == null) return result;
                if (Medicin.Text.Trim() == "") return result;
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
