using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.FormType.TreatmentTypeComboCheck.Validation
{
    class TreatmentTypeValidationRule : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.GridLookUpEdit cboTreatmentType;

        public override bool Validate(System.Windows.Forms.Control control, object value)
        {
            bool result = false;
            try
            {
                if (cboTreatmentType == null) return result;
                if (cboTreatmentType.EditValue == null) return result;
                HIS.UC.FormType.Base.GridCheckMarksSelection gridCheckMark = cboTreatmentType.Properties.Tag as HIS.UC.FormType.Base.GridCheckMarksSelection;
                if (gridCheckMark == null || gridCheckMark.Selection == null || gridCheckMark.Selection.Count == 0)
                    return result;

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
