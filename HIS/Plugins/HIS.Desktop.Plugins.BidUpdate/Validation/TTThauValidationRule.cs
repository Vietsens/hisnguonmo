using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.BidUpdate.Validation
{
    class TTThauValidationRule : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.TextEdit txtTTThau;
        public override bool Validate(System.Windows.Forms.Control control, object value)
        {
            bool valid = false;

            try
            {
                if (txtTTThau == null) return valid;

                //if (string.IsNullOrEmpty(txtTTThau.Text))
                //{
                //    this.ErrorText = "Trường dữ liệu bắt buộc";
                //    return valid;
                //}

                if (!string.IsNullOrEmpty(txtTTThau.Text) && Encoding.UTF8.GetBytes(txtTTThau.Text).Length > 50)
                {
                    this.ErrorText = "Trường dữ liệu nhập quá giới hạn";
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
