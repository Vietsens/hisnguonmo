using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisReceptionRoom.Validation
{
    class ValidMaxlengthTKCT : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.TextEdit txtTKCT;

        public override bool Validate(System.Windows.Forms.Control control, object value)
        {
            bool valid = false;
            try
            {
                if (Inventec.Common.String.CountVi.Count(txtTKCT.Text) > 100)
                {
                    this.ErrorText = "Độ dài vượt quá " + 100;
                    return valid;
                }
                else
                {
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
