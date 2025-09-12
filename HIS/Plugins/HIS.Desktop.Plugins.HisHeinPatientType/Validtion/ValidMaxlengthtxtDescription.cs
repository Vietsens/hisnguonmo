using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisHeinPatientType.Validtion
{
    class ValidMaxlengthtxtDescription : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.TextEdit txtDescription;

        public override bool Validate(System.Windows.Forms.Control control, object value)
        {
            bool valid = false;
            try
            {
                if (Inventec.Common.String.CountVi.Count(txtDescription.Text) > 4000)
                {
                    this.ErrorText = "Độ dài mã vượt quá " + 4000;
                    return valid;
                }
                else
                {
                    if (Inventec.Common.String.CountVi.Count(txtDescription.Text) > 4000)
                    {
                        this.ErrorText = "Độ dài mã vượt quá " + 4000;
                        return valid;
                    }

                    else
                        valid = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return valid;
        }
    }
}
