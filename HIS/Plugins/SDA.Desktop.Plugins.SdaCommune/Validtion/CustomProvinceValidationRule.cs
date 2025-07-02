using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SDA.Desktop.Plugins.SdaCommune.Validtion
{
    class CustomProvinceValidationRule : DevExpress.XtraEditors. DXErrorProvider.ValidationRule
    {

        public GridLookUpEdit cboProvince { get; set; }
        public GridLookUpEdit cboDistrict { get; set; }

        public override bool Validate(Control control, object value)
        {
            bool valid = false;
            try
            {
                if (cboDistrict.EditValue == null && (cboProvince.EditValue == null || string.IsNullOrWhiteSpace(cboProvince.EditValue.ToString())))
                {
                    this.ErrorText = "Vui lòng chọn Tỉnh/TP khi không chọn Huyện";
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
