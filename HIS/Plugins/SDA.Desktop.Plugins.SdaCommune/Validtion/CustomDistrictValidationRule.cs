using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SDA.Desktop.Plugins.SdaCommune.Validtion
{
    class CustomDistrictValidationRule : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        public GridLookUpEdit cboDistrict { get; set; }
        public GridLookUpEdit cboProvince { get; set; }

        public override bool Validate(Control control, object value)
        {
            bool valid = false;
            try
            {
                if (cboProvince.EditValue == null && (cboDistrict.EditValue == null || string.IsNullOrWhiteSpace(cboDistrict.EditValue.ToString())))
                {
                    this.ErrorText = "Vui lòng chọn Huyện khi không chọn Tỉnh/TP";
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
